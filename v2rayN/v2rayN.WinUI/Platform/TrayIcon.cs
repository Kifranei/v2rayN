using System.Runtime.InteropServices;

namespace v2rayN.WinUI.Platform;

public sealed class TrayMenu
{
    public string? Text { get; init; }
    public bool Enabled { get; init; } = true;
    public bool Checked { get; init; }
    public Action? Click { get; init; }
    public List<TrayMenu>? Children { get; init; }
    public static TrayMenu Separator { get; } = new();
}

public sealed class TrayIcon : IDisposable
{
    private const uint WmTray = 0x8000 + 0x21;
    private const uint WmLButtonDblClk = 0x203;
    private const uint WmRButtonUp = 0x205;
    private const uint WmNull = 0;

    private static readonly Native.WndProc Proc = WndProc;
    private static readonly Dictionary<nint, TrayIcon> Instances = new();
    private static bool _classReady;
    private static uint _taskbarCreated;

    private readonly nint _hwnd;
    private nint _icon;
    private System.Drawing.Icon? _ownedIcon;
    private string _tip = string.Empty;
    private bool _added;
    private bool _disposed;

    public event Action? DoubleClick;
    public Func<List<TrayMenu>>? BuildMenu;

    public TrayIcon()
    {
        var instance = Native.GetModuleHandleW(null);
        if (!_classReady)
        {
            var wc = new Native.WndClassEx
            {
                cbSize = Marshal.SizeOf<Native.WndClassEx>(),
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(Proc),
                hInstance = instance,
                lpszClassName = "v2rayN.WinUI.Tray"
            };
            Native.RegisterClassExW(ref wc);
            _taskbarCreated = Native.RegisterWindowMessageW("TaskbarCreated");
            _classReady = true;
        }

        _hwnd = Native.CreateWindowExW(0, "v2rayN.WinUI.Tray", "v2rayN.WinUI.Tray", 0, 0, 0, 0, 0, nint.Zero, nint.Zero, instance, nint.Zero);
        Instances[_hwnd] = this;
        HotkeyHub.Instance.Attach(_hwnd);
        SetIcon(1);
        Add();
    }

    public string Tip
    {
        get => _tip;
        set
        {
            value ??= string.Empty;
            if (value.Length > 127)
            {
                value = value[..127];
            }

            _tip = value;
            var data = Data(Native.NifTip);
            Native.Shell_NotifyIconW(Native.NimModify, ref data);
        }
    }

    public void SetProxyIcon(Config config)
    {
        try
        {
            var index = (int)config.SystemProxyItem.SysProxyType + 1;
            if (index < 1 || index > 4)
            {
                index = 1;
            }

            SetIcon(index);
            var appIcon = Path.Combine(AppContext.BaseDirectory, $"NotifyIcon{index}.ico");
            if (File.Exists(appIcon) && AppHost.Shell is not null)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(AppHost.Shell);
                var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id).SetIcon(appIcon);
            }
        }
        catch (Exception ex)
        {
            Logging.SaveLog("SetProxyIcon", ex);
        }
    }

    private void SetIcon(int index)
    {
        var path = Path.Combine(AppContext.BaseDirectory, $"NotifyIcon{index}.ico");
        if (!File.Exists(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, "v2rayN.ico");
        }

        if (!File.Exists(path))
        {
            return;
        }

        var icon = new System.Drawing.Icon(path);
        _ownedIcon?.Dispose();
        _ownedIcon = icon;
        _icon = icon.Handle;
        if (_added)
        {
            var data = Data(Native.NifIcon);
            Native.Shell_NotifyIconW(Native.NimModify, ref data);
        }
    }

    private void Add()
    {
        var data = Data(Native.NifMessage | Native.NifIcon | Native.NifTip);
        _added = Native.Shell_NotifyIconW(Native.NimAdd, ref data);
    }

    private Native.NotifyIconData Data(int flags)
    {
        return new Native.NotifyIconData
        {
            cbSize = Marshal.SizeOf<Native.NotifyIconData>(),
            hWnd = _hwnd,
            uID = 1,
            uFlags = flags,
            uCallbackMessage = (int)WmTray,
            hIcon = _icon,
            szTip = _tip,
            szInfo = string.Empty,
            szInfoTitle = string.Empty
        };
    }

    private static nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        if (Instances.TryGetValue(hwnd, out var tray))
        {
            if (msg == WmTray)
            {
                var mouse = (uint)(lParam.ToInt64() & 0xFFFF);
                if (mouse == WmLButtonDblClk)
                {
                    tray.DoubleClick?.Invoke();
                }
                else if (mouse == WmRButtonUp)
                {
                    tray.ShowMenu();
                }

                return nint.Zero;
            }

            if (msg == Native.WmHotkey)
            {
                HotkeyHub.Instance.OnHotkey(wParam.ToInt32());
                return nint.Zero;
            }

            if (msg == _taskbarCreated && _taskbarCreated != 0)
            {
                tray.Add();
                return nint.Zero;
            }
        }

        return Native.DefWindowProcW(hwnd, msg, wParam, lParam);
    }

    private void ShowMenu()
    {
        var items = BuildMenu?.Invoke();
        if (items is null || items.Count == 0)
        {
            return;
        }

        var actions = new Dictionary<int, Action?>();
        var next = 1;
        var root = Build(items, actions, ref next);
        try
        {
            Native.GetCursorPos(out var pt);
            Native.SetForegroundWindow(_hwnd);
            var cmd = Native.TrackPopupMenuEx(root, Native.TpmReturnCmd | Native.TpmRightButton | Native.TpmNoNotify, pt.X, pt.Y, _hwnd, nint.Zero);
            Native.PostMessageW(_hwnd, WmNull, nint.Zero, nint.Zero);
            if (cmd > 0 && actions.TryGetValue(cmd, out var action))
            {
                AppHost.Queue.TryEnqueue(() => action?.Invoke());
            }
        }
        finally
        {
            Native.DestroyMenu(root);
        }
    }

    private static nint Build(List<TrayMenu> items, Dictionary<int, Action?> actions, ref int next)
    {
        var menu = Native.CreatePopupMenu();
        foreach (var item in items)
        {
            if (item.Text is null)
            {
                Native.AppendMenuW(menu, Native.MfSeparator, nint.Zero, null);
                continue;
            }

            var flags = item.Enabled ? Native.MfString : Native.MfString | Native.MfGrayed;
            if (item.Checked)
            {
                flags |= Native.MfChecked;
            }

            if (item.Children is not null)
            {
                var sub = Build(item.Children, actions, ref next);
                Native.AppendMenuW(menu, flags | Native.MfPopup, sub, item.Text);
                continue;
            }

            var id = next++;
            actions[id] = item.Click;
            Native.AppendMenuW(menu, flags, id, item.Text);
        }

        return menu;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_added)
        {
            var data = Data(0);
            Native.Shell_NotifyIconW(Native.NimDelete, ref data);
        }

        Instances.Remove(_hwnd);
        Native.DestroyWindow(_hwnd);
        _ownedIcon?.Dispose();
    }
}
