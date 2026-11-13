using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace v2rayN.WinUI.Platform;

/// <summary>
/// Puts v2rayN windows on virtual desktop 2 without switching the desktop the user is on.
/// Desktop ids come from the Explorer registry list. Moving uses the public IVirtualDesktopManager.
/// </summary>
internal static class VirtualDesktops
{
    private const int DesktopIndex = 1;
    private static Guid? _target;
    private static IVirtualDesktopManager? _manager;

    public static bool TryGetTarget(out Guid id)
    {
        if (_target is Guid cached && cached != Guid.Empty)
        {
            id = cached;
            return true;
        }

        id = Guid.Empty;
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops");
            if (key?.GetValue("VirtualDesktopIDs") is not byte[] bytes || bytes.Length < 32)
            {
                return false;
            }

            id = new Guid(bytes.AsSpan(DesktopIndex * 16, 16));
            _target = id;
            return true;
        }
        catch (Exception ex)
        {
            Logging.SaveLog("VirtualDesktops.TryGetTarget", ex);
            return false;
        }
    }

    public static bool IsOnTarget()
    {
        if (!TryGetTarget(out var target))
        {
            return true;
        }

        return ReadCurrent() is Guid current && current == target;
    }

    public static void Move(nint hwnd)
    {
        if (hwnd == nint.Zero || !TryGetTarget(out var id))
        {
            return;
        }

        try
        {
            Manager()?.MoveWindowToDesktop(hwnd, ref id);
        }
        catch (Exception ex)
        {
            Logging.SaveLog("VirtualDesktops.Move", ex);
        }
    }

    private static Guid? ReadCurrent()
    {
        var session = System.Diagnostics.Process.GetCurrentProcess().SessionId;
        var path = $@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\SessionInfo\{session}\VirtualDesktops";
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(path);
            if (key?.GetValue("CurrentVirtualDesktop") is byte[] bytes && bytes.Length >= 16)
            {
                return new Guid(bytes.AsSpan(0, 16));
            }
        }
        catch (Exception ex)
        {
            Logging.SaveLog("VirtualDesktops.ReadCurrent", ex);
        }

        return null;
    }

    private static IVirtualDesktopManager? Manager()
    {
        if (_manager is not null)
        {
            return _manager;
        }

        try
        {
            _manager = (IVirtualDesktopManager)new VirtualDesktopManager();
        }
        catch (Exception ex)
        {
            Logging.SaveLog("VirtualDesktops.Manager", ex);
        }

        return _manager;
    }

    [ComImport]
    [Guid("aa509086-5ca9-4c25-8f95-589d3c07b48a")]
    private class VirtualDesktopManager
    {
    }

    [ComImport]
    [Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IVirtualDesktopManager
    {
        [PreserveSig]
        int IsWindowOnCurrentVirtualDesktop(nint topLevelWindow, out int onCurrent);

        [PreserveSig]
        int GetWindowDesktopId(nint topLevelWindow, out Guid desktopId);

        [PreserveSig]
        int MoveWindowToDesktop(nint topLevelWindow, ref Guid desktopId);
    }
}
