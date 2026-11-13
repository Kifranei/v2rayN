using System.Runtime.InteropServices;
using System.Text;

namespace v2rayN.WinUI.Platform;

internal static class Native
{
    public const uint MbOk = 0x0;
    public const uint MbYesNo = 0x4;
    public const uint MbIconInformation = 0x40;
    public const uint MbIconQuestion = 0x20;
    public const uint MbIconWarning = 0x30;
    public const int IdYes = 6;

    public const int SwHide = 0;
    public const int SwShowNoActivate = 4;
    public const int SwShow = 5;
    public const int SwRestore = 9;

    public const uint WmHotkey = 0x0312;
    public const int ModAlt = 0x1;
    public const int ModControl = 0x2;
    public const int ModShift = 0x4;
    public const int ModWin = 0x8;

    public const int GwlpHwndParent = -8;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBoxW(nint hWnd, string text, string caption, uint type);

    public static void MessageBox(nint hwnd, string text, string caption, uint type) => MessageBoxW(hwnd, text, caption, type);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool EnableWindow(nint hWnd, bool enable);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(nint hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsIconic(nint hWnd);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    public static extern nint SetWindowLongPtr(nint hWnd, int nIndex, nint dwNewLong);

    [DllImport("user32.dll")]
    public static extern nint GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern uint GetDpiForWindow(nint hwnd);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(nint hWnd, out Rect rect);

    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll")]
    public static extern short GetKeyState(int nVirtKey);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool RegisterHotKey(nint hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnregisterHotKey(nint hWnd, int id);

    [DllImport("user32.dll")]
    public static extern nint DefWindowProcW(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern ushort RegisterClassExW(ref WndClassEx wc);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern nint CreateWindowExW(int exStyle, string className, string windowName, int style, int x, int y, int w, int h, nint parent, nint menu, nint instance, nint param);

    [DllImport("user32.dll")]
    public static extern bool DestroyWindow(nint hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern uint RegisterWindowMessageW(string message);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern nint GetModuleHandleW(string? name);

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out Point pt);

    [DllImport("user32.dll")]
    public static extern bool PostMessageW(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll")]
    public static extern bool DestroyIcon(nint hIcon);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int index);

    public const int SmXVirtualScreen = 76;
    public const int SmYVirtualScreen = 77;
    public const int SmCxVirtualScreen = 78;
    public const int SmCyVirtualScreen = 79;

    public delegate nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WndClassEx
    {
        public int cbSize;
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
        public nint hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }

    public const int NimAdd = 0;
    public const int NimModify = 1;
    public const int NimDelete = 2;
    public const int NifMessage = 0x1;
    public const int NifIcon = 0x2;
    public const int NifTip = 0x4;
    public const int NifInfo = 0x10;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NotifyIconData
    {
        public int cbSize;
        public nint hWnd;
        public int uID;
        public int uFlags;
        public int uCallbackMessage;
        public nint hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public int dwState;
        public int dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        public int uTimeoutOrVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public int dwInfoFlags;
        public Guid guidItem;
        public nint hBalloonIcon;
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern bool Shell_NotifyIconW(int message, ref NotifyIconData data);

    public const uint MfString = 0x0;
    public const uint MfGrayed = 0x1;
    public const uint MfChecked = 0x8;
    public const uint MfPopup = 0x10;
    public const uint MfSeparator = 0x800;
    public const uint TpmRightButton = 0x2;
    public const uint TpmReturnCmd = 0x100;
    public const uint TpmNoNotify = 0x80;

    [DllImport("user32.dll")]
    public static extern nint CreatePopupMenu();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool AppendMenuW(nint hMenu, uint flags, nint idOrSub, string? text);

    [DllImport("user32.dll")]
    public static extern int TrackPopupMenuEx(nint hMenu, uint flags, int x, int y, nint hWnd, nint tpm);

    [DllImport("user32.dll")]
    public static extern bool DestroyMenu(nint hMenu);

    public const int OfNExplorer = 0x80000;
    public const int OfNFileMustExist = 0x1000;
    public const int OfNPathMustExist = 0x800;
    public const int OfNOverwritePrompt = 0x2;
    public const int OfNNoChangeDir = 0x8;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct OpenFileName
    {
        public int lStructSize;
        public nint hwndOwner;
        public nint hInstance;
        public nint lpstrFilter;
        public nint lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public nint lpstrFile;
        public int nMaxFile;
        public nint lpstrFileTitle;
        public int nMaxFileTitle;
        public nint lpstrInitialDir;
        public nint lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public nint lpstrDefExt;
        public nint lCustData;
        public nint lpfnHook;
        public nint lpTemplateName;
        public nint pvReserved;
        public int dwReserved;
        public int FlagsEx;
    }

    [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool GetOpenFileNameW(ref OpenFileName ofn);

    [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool GetSaveFileNameW(ref OpenFileName ofn);

    public const uint CfUnicodeText = 13;
    public const uint GmemMoveable = 0x0002;

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool OpenClipboard(nint hWnd);

    [DllImport("user32.dll")]
    public static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    public static extern bool EmptyClipboard();

    [DllImport("user32.dll")]
    public static extern nint GetClipboardData(uint format);

    [DllImport("user32.dll")]
    public static extern nint SetClipboardData(uint format, nint hMem);

    [DllImport("kernel32.dll")]
    public static extern nint GlobalAlloc(uint flags, nuint bytes);

    [DllImport("kernel32.dll")]
    public static extern nint GlobalLock(nint hMem);

    [DllImport("kernel32.dll")]
    public static extern bool GlobalUnlock(nint hMem);

    [DllImport("kernel32.dll")]
    public static extern nint GlobalFree(nint hMem);

    public static string? GetClipboardText()
    {
        if (!OpenClipboard(nint.Zero))
        {
            return null;
        }

        try
        {
            var handle = GetClipboardData(CfUnicodeText);
            if (handle == nint.Zero)
            {
                return null;
            }

            var ptr = GlobalLock(handle);
            if (ptr == nint.Zero)
            {
                return null;
            }

            try
            {
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                GlobalUnlock(handle);
            }
        }
        finally
        {
            CloseClipboard();
        }
    }

    public static void SetClipboardText(string? text)
    {
        text ??= string.Empty;
        if (!OpenClipboard(nint.Zero))
        {
            return;
        }

        try
        {
            EmptyClipboard();
            var bytes = (text.Length + 1) * 2;
            var hMem = GlobalAlloc(GmemMoveable, (nuint)bytes);
            if (hMem == nint.Zero)
            {
                return;
            }

            var ptr = GlobalLock(hMem);
            if (ptr == nint.Zero)
            {
                GlobalFree(hMem);
                return;
            }

            try
            {
                Marshal.Copy(text.ToCharArray(), 0, ptr, text.Length);
                Marshal.WriteInt16(ptr, text.Length * 2, 0);
            }
            finally
            {
                GlobalUnlock(hMem);
            }

            if (SetClipboardData(CfUnicodeText, hMem) == nint.Zero)
            {
                GlobalFree(hMem);
            }
        }
        finally
        {
            CloseClipboard();
        }
    }

    public static bool? OpenFile(out string fileName, string filter)
    {
        return ShowFileDialog(out fileName, filter, save: false);
    }

    public static bool? SaveFile(out string fileName, string filter)
    {
        return ShowFileDialog(out fileName, filter, save: true);
    }

    private static bool? ShowFileDialog(out string fileName, string filter, bool save)
    {
        fileName = string.Empty;
        var filterPtr = FilterToPointer(filter);
        var filePtr = Marshal.AllocHGlobal(520 * 2);
        try
        {
            for (var i = 0; i < 520 * 2; i++)
            {
                Marshal.WriteByte(filePtr, i, 0);
            }

            var ofn = new OpenFileName
            {
                hwndOwner = AppHost.Hwnd,
                lpstrFilter = filterPtr,
                lpstrFile = filePtr,
                nMaxFile = 520,
                Flags = OfNExplorer | OfNPathMustExist | OfNNoChangeDir | (save ? OfNOverwritePrompt : OfNFileMustExist)
            };
            ofn.lStructSize = Marshal.SizeOf<OpenFileName>();

            var ok = save ? GetSaveFileNameW(ref ofn) : GetOpenFileNameW(ref ofn);
            if (!ok)
            {
                return false;
            }

            fileName = Marshal.PtrToStringUni(filePtr) ?? string.Empty;
            return fileName.Length > 0;
        }
        finally
        {
            Marshal.FreeHGlobal(filterPtr);
            Marshal.FreeHGlobal(filePtr);
        }
    }

    private static nint FilterToPointer(string spec)
    {
        var parts = (string.IsNullOrWhiteSpace(spec) ? "All|*.*" : spec).Split('|');
        var sb = new StringBuilder();
        for (var i = 0; i + 1 < parts.Length; i += 2)
        {
            sb.Append(parts[i]).Append('\0').Append(parts[i + 1]).Append('\0');
        }

        if (sb.Length == 0)
        {
            sb.Append("All\0*.*\0");
        }

        sb.Append('\0');
        var chars = sb.ToString().ToCharArray();
        var ptr = Marshal.AllocHGlobal((chars.Length + 1) * 2);
        Marshal.Copy(chars, 0, ptr, chars.Length);
        Marshal.WriteInt16(ptr, chars.Length * 2, 0);
        return ptr;
    }

    public static byte[]? CaptureScreen()
    {
        try
        {
            var x = GetSystemMetrics(SmXVirtualScreen);
            var y = GetSystemMetrics(SmYVirtualScreen);
            var w = GetSystemMetrics(SmCxVirtualScreen);
            var h = GetSystemMetrics(SmCyVirtualScreen);
            if (w <= 0 || h <= 0)
            {
                return null;
            }

            using var bitmap = new System.Drawing.Bitmap(w, h);
            using var graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(x, y, 0, 0, bitmap.Size, System.Drawing.CopyPixelOperation.SourceCopy);
            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            Logging.SaveLog("CaptureScreen", ex);
            return null;
        }
    }
}
