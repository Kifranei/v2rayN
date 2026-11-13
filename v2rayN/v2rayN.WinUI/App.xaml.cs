using Microsoft.UI.Xaml;

namespace v2rayN.WinUI;

public partial class App : Application
{
    private bool _reactiveReady;

    public App()
    {
        // RequestedTheme is only legal before the application starts. Setting it in OnLaunched fail-fasts.
        RequestedTheme = ThemeChrome.Resolve(AppManager.Instance.Config.UiItem.CurrentTheme) == ElementTheme.Dark
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;

        InitializeComponent();
        UnhandledException += (_, e) =>
        {
            Logging.SaveLog("App_UnhandledException", e.Exception);
            e.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                Logging.SaveLog("CurrentDomain_UnhandledException", ex);
            }
        };
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Logging.SaveLog("TaskScheduler_UnobservedTaskException", e.Exception);
            e.SetObserved();
        };
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        if (!_reactiveReady)
        {
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithWinUI()
                .WithRegistrationOnBuild(static resolver =>
                    resolver.Register<IActivationForViewFetcher>(static () => new WindowActivationFetcher()))
                .BuildApp();
            _reactiveReady = true;
        }

        try
        {
            AppManager.Instance.WindowDialog = new WindowDialog();
            AppManager.Instance.InitComponents();

            var shell = new ShellWindow();
            AppHost.Shell = shell;
            AppHost.Present(shell, show: true, stealFocus: false);
        }
        catch (Exception ex)
        {
            Logging.SaveLog("OnLaunched", ex);
            Native.MessageBox(nint.Zero, ex.Message, Global.AppName, Native.MbOk | Native.MbIconWarning);
        }
    }
}
