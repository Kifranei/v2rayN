using System.Runtime.InteropServices;

namespace v2rayN.WinUI.Platform;

internal sealed class HotkeyHub
{
    public static HotkeyHub Instance { get; } = new();

    private readonly Dictionary<int, List<EGlobalHotkey>> _map = new();
    private nint _hwnd;
    private int _nextId = 1;

    public bool IsPause { get; set; }
    public event Action<EGlobalHotkey>? Triggered;

    public void Attach(nint hwnd)
    {
        _hwnd = hwnd;
        Reload();
    }

    public void Reload()
    {
        if (_hwnd == nint.Zero)
        {
            return;
        }

        foreach (var id in _map.Keys.ToArray())
        {
            Native.UnregisterHotKey(_hwnd, id);
        }

        _map.Clear();
        _nextId = 1;
        foreach (var item in AppManager.Instance.Config.GlobalHotkeys)
        {
            if (WpfKeys.ToVirtual(item.KeyCode) is not int vk)
            {
                continue;
            }

            var mods = 0;
            if (item.Control)
            {
                mods |= Native.ModControl;
            }

            if (item.Shift)
            {
                mods |= Native.ModShift;
            }

            if (item.Alt)
            {
                mods |= Native.ModAlt;
            }

            var id = _nextId++;
            if (!Native.RegisterHotKey(_hwnd, id, mods, vk))
            {
                var err = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message;
                Logging.SaveLog($"Register hotkey {item.EGlobalHotkey} failed: {err}");
                continue;
            }

            if (!_map.TryGetValue(id, out var list))
            {
                list = [];
                _map[id] = list;
            }

            list.Add(item.EGlobalHotkey);
        }
    }

    public void OnHotkey(int id)
    {
        if (IsPause || !_map.TryGetValue(id, out var list))
        {
            return;
        }

        foreach (var key in list)
        {
            Triggered?.Invoke(key);
        }
    }
}
