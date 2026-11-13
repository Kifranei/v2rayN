using Microsoft.Win32;
using WinRT.Interop;
using Windows.UI;

namespace v2rayN.WinUI.Platform;

// The WinUI island clears to black unless the page paints its own background.
// ActualTheme can stay on the system theme after a runtime switch, so the page color follows the saved choice.
internal static class ThemeChrome
{
    public static ElementTheme Resolve(string? name)
    {
        return name switch
        {
            nameof(ETheme.Light) or nameof(ETheme.Aquatic) or nameof(ETheme.Desert) => ElementTheme.Light,
            nameof(ETheme.FollowSystem) => SystemUsesLightApps() ? ElementTheme.Light : ElementTheme.Dark,
            _ => ElementTheme.Dark
        };
    }

    private static readonly HashSet<FrameworkElement> Fitted = [];

    public static void Apply(Window window, string? name)
    {
        if (window.Content is not FrameworkElement root)
        {
            return;
        }

        var theme = Resolve(name);
        root.RequestedTheme = theme;
        root.HorizontalAlignment = HorizontalAlignment.Stretch;
        root.VerticalAlignment = VerticalAlignment.Stretch;
        Paint(root, theme);
        if (root is ContentControl { Content: FrameworkElement inner })
        {
            inner.RequestedTheme = theme;
            inner.HorizontalAlignment = HorizontalAlignment.Stretch;
            inner.VerticalAlignment = VerticalAlignment.Stretch;
            Paint(inner, theme);
        }

        Fit(window, root);
        WatchSystem();
    }

    private static void Fit(Window window, FrameworkElement root)
    {
        void ApplySize()
        {
            var hwnd = WindowNative.GetWindowHandle(window);
            if (!Native.GetClientRect(hwnd, out var rect))
            {
                return;
            }

            var dpi = Native.GetDpiForWindow(hwnd);
            var scale = (dpi == 0 ? 96 : dpi) / 96.0;
            var width = (rect.Right - rect.Left) / scale;
            var height = (rect.Bottom - rect.Top) / scale;
            if (width <= 1 || height <= 1)
            {
                return;
            }

            root.Width = width;
            root.Height = height;
        }

        if (!Fitted.Add(root))
        {
            ApplySize();
            return;
        }

        void Hook()
        {
            ApplySize();
            if (root.XamlRoot is not null)
            {
                root.XamlRoot.Changed += (_, _) => ApplySize();
            }
        }

        if (root.IsLoaded)
        {
            Hook();
        }
        else
        {
            root.Loaded += (_, _) => Hook();
        }

        root.Unloaded += (_, _) => Fitted.Remove(root);
    }

    private static bool _systemHooked;

    private static void WatchSystem()
    {
        if (_systemHooked)
        {
            return;
        }

        _systemHooked = true;
        SystemEvents.UserPreferenceChanged += (_, e) =>
        {
            if (e.Category is not (UserPreferenceCategory.General or UserPreferenceCategory.Color))
            {
                return;
            }

            AppHost.Queue?.TryEnqueue(() =>
            {
                var name = AppManager.Instance.Config.UiItem.CurrentTheme;
                if (name == nameof(ETheme.FollowSystem) && AppHost.Shell is Window shell)
                {
                    Apply(shell, name);
                }
            });
        };
    }

    private static void Paint(FrameworkElement root, ElementTheme theme)
    {
        var color = theme == ElementTheme.Dark
            ? Color.FromArgb(255, 32, 32, 32)
            : Color.FromArgb(255, 243, 243, 243);
        var brush = new SolidColorBrush(color);
        switch (root)
        {
            case Panel panel:
                panel.Background = brush;
                break;
            case Control control:
                control.Background = brush;
                break;
        }
    }

    private static bool SystemUsesLightApps()
    {
        try
        {
            return Registry.GetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                "AppsUseLightTheme",
                1) is int light && light != 0;
        }
        catch
        {
            return true;
        }
    }
}
