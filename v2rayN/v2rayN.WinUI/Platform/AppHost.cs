using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinRT.Interop;

namespace v2rayN.WinUI.Platform;

internal static class AppHost
{
    public static DispatcherQueue Queue { get; set; } = null!;
    public static ShellWindow? Shell { get; set; }

    public static nint Hwnd => Shell is null ? nint.Zero : WindowNative.GetWindowHandle(Shell);

    public static XamlRoot? Root => Shell?.Content is FrameworkElement element ? element.XamlRoot : null;

    public static void Place(Window window, string sizeKey, int width, int height)
    {
        var hwnd = WindowNative.GetWindowHandle(window);
        var id = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(id);
        var size = ConfigHandler.GetWindowSizeItem(AppManager.Instance.Config, sizeKey);
        var w = size is null ? width : (int)Math.Min(size.Width, 2400);
        var h = size is null ? height : (int)Math.Min(size.Height, 1600);
        if (w < 480)
        {
            w = width;
        }

        if (h < 320)
        {
            h = height;
        }

        appWindow.Resize(new SizeInt32(w, h));
        if (DisplayArea.GetFromWindowId(id, DisplayAreaFallback.Primary) is { } area)
        {
            var x = area.WorkArea.X + Math.Max(0, (area.WorkArea.Width - w) / 2);
            var y = area.WorkArea.Y + Math.Max(0, (area.WorkArea.Height - h) / 2);
            appWindow.Move(new PointInt32(x, y));
        }

        var icon = Path.Combine(AppContext.BaseDirectory, "v2rayN.ico");
        if (File.Exists(icon))
        {
            appWindow.SetIcon(icon);
        }

        window.Closed += (_, _) =>
        {
            ConfigHandler.SaveWindowSizeItem(AppManager.Instance.Config, sizeKey, appWindow.Size.Width, appWindow.Size.Height);
        };

        VirtualDesktops.Move(hwnd);
    }

    public static void Show(Window window, bool show)
    {
        Present(window, show, stealFocus: show);
    }

    public static void Present(Window window, bool show, bool stealFocus)
    {
        var hwnd = WindowNative.GetWindowHandle(window);
        if (!show)
        {
            Native.ShowWindow(hwnd, Native.SwHide);
            WindowActivationFetcher.Notify(window, false);
            return;
        }

        ThemeChrome.Apply(window, AppManager.Instance.Config.UiItem.CurrentTheme);

        VirtualDesktops.Move(hwnd);
        var focus = stealFocus && VirtualDesktops.IsOnTarget();
        if (focus)
        {
            Native.ShowWindow(hwnd, Native.IsIconic(hwnd) ? Native.SwRestore : Native.SwShow);
            window.Activate();
            Native.SetForegroundWindow(hwnd);
        }
        else
        {
            Native.ShowWindow(hwnd, Native.SwShowNoActivate);
            VirtualDesktops.Move(hwnd);
        }

        WindowActivationFetcher.Notify(window, true);
    }

    public static bool IsShown(Window window)
    {
        return Native.IsWindowVisible(WindowNative.GetWindowHandle(window));
    }

    public static Task Run(Action action)
    {
        if (Queue.HasThreadAccess)
        {
            action();
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource();
        Queue.TryEnqueue(() =>
        {
            try
            {
                action();
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        return tcs.Task;
    }

    public static Task<T> Run<T>(Func<T> action)
    {
        if (Queue.HasThreadAccess)
        {
            return Task.FromResult(action());
        }

        var tcs = new TaskCompletionSource<T>();
        Queue.TryEnqueue(() =>
        {
            try
            {
                tcs.SetResult(action());
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        return tcs.Task;
    }

    public static Task<T> Run<T>(Func<Task<T>> action)
    {
        if (Queue.HasThreadAccess)
        {
            return action();
        }

        var tcs = new TaskCompletionSource<T>();
        Queue.TryEnqueue(async () =>
        {
            try
            {
                tcs.SetResult(await action());
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        return tcs.Task;
    }
}
