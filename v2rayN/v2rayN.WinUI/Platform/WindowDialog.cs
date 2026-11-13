using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace v2rayN.WinUI.Platform;

public sealed class WindowDialog : IWindowDialog
{
    public async Task<bool> ShowDialogAsync<TViewModel>(TViewModel vm)
        where TViewModel : class
    {
        return await AppHost.Run(async () =>
        {
            if (Create(vm) is not Window window)
            {
                return false;
            }

            if (window is IViewFor view)
            {
                view.ViewModel = vm;
            }

            var accepted = false;
            if (vm is ServiceLib.Base.ICloseable closeable)
            {
                closeable.RequestClose += (_, _) =>
                {
                    accepted = true;
                    window.Close();
                };
            }

            var owner = AppHost.Hwnd;
            var child = WindowNative.GetWindowHandle(window);
            AppHost.Present(window, show: true, stealFocus: false);
            if (owner != nint.Zero)
            {
                Native.SetWindowLongPtr(child, Native.GwlpHwndParent, owner);
                Native.EnableWindow(owner, false);
                AppHost.Present(window, show: true, stealFocus: false);
            }

            var done = new TaskCompletionSource<bool>();
            window.Closed += (_, _) =>
            {
                if (!accepted && IsModified(vm))
                {
                    accepted = true;
                }

                if (owner != nint.Zero)
                {
                    Native.EnableWindow(owner, true);
                    if (VirtualDesktops.IsOnTarget())
                    {
                        AppHost.Shell?.Activate();
                    }
                }

                done.TrySetResult(accepted);
            };

            return await done.Task;
        });
    }

    private static bool IsModified(object vm)
    {
        if (vm is not (SubSettingViewModel or RoutingSettingViewModel))
        {
            return false;
        }

        var property = vm.GetType().GetProperty("IsModified");
        return property?.GetValue(vm) is true;
    }

    private static Window? Create(object vm) => vm switch
    {
        AddServerViewModel => new AddServerWindow(),
        AddServer2ViewModel => new AddServer2Window(),
        AddGroupServerViewModel => new AddGroupServerWindow(),
        OptionSettingViewModel => new OptionSettingWindow(),
        RoutingSettingViewModel => new RoutingSettingWindow(),
        RoutingRuleSettingViewModel => new RoutingRuleSettingWindow(),
        RoutingRuleDetailsViewModel => new RoutingRuleDetailsWindow(),
        DNSSettingViewModel => new DnsSettingWindow(),
        FullConfigTemplateViewModel => new FullConfigTemplateWindow(),
        GlobalHotkeySettingViewModel => new GlobalHotkeySettingWindow(),
        SubSettingViewModel => new SubSettingWindow(),
        SubEditViewModel => new SubEditWindow(),
        ProfilesSelectViewModel => new ProfilesSelectWindow(),
        _ => null
    };
}
