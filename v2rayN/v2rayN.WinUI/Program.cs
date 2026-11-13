using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace v2rayN.WinUI;

public static class Program
{
    public static EventWaitHandle? ProgramStarted;

    [STAThread]
    public static void Main(string[] args)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();

        var exePathKey = Utils.GetMd5(Utils.GetExePath());
        var rebootAs = args.Any(t => t == Global.RebootAs);
        ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, exePathKey, out var createdNew);
        if (!rebootAs && !createdNew)
        {
            ProgramStarted.Set();
            return;
        }

        if (!AppManager.Instance.InitApp())
        {
            Native.MessageBox(nint.Zero, "Loading GUI configuration file is abnormal, please restart the application\n加载 GUI 配置文件异常，请重启应用", Global.AppName, Native.MbOk | Native.MbIconWarning);
            return;
        }

        Application.Start(_ =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            AppHost.Queue = DispatcherQueue.GetForCurrentThread();
            new App();
        });
    }
}
