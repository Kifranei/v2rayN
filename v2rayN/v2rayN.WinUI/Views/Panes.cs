using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace v2rayN.WinUI.Views;

public sealed class MsgPane : ReactiveUserControl<MsgViewModel>
{
    private readonly TextBox _log = new()
    {
        IsReadOnly = true,
        AcceptsReturn = true,
        TextWrapping = TextWrapping.Wrap,
        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas")
    };
    private readonly AutoSuggestBox _filter = new()
    {
        Width = 200,
        PlaceholderText = ResUI.MsgFilterTitle,
        ItemsSource = Global.PresetMsgFilters
    };
    public ToggleSwitch AutoRefresh { get; } = new() { Header = ResUI.TbAutoRefresh };
    public ToggleSwitch ScrollEnd { get; } = new() { IsOn = true, Header = ResUI.TbAutoScrollToEnd };

    public MsgPane()
    {
        _filter.TextChanged += (_, e) =>
        {
            if (e.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            var query = _filter.Text ?? string.Empty;
            _filter.ItemsSource = string.IsNullOrEmpty(query)
                ? Global.PresetMsgFilters
                : Global.PresetMsgFilters.Where(item => item.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        };

        var bar = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        bar.Children.Add(_filter);
        bar.Children.Add(AutoRefresh);
        bar.Children.Add(ScrollEnd);
        var copy = new Button { Content = ResUI.menuMsgViewCopyAll };
        var clear = new Button { Content = ResUI.menuMsgViewClear };
        copy.Click += (_, _) => Native.SetClipboardText(_log.Text);
        clear.Click += (_, _) => _log.Text = "----- Message cleared -----\n";
        bar.Children.Add(copy);
        bar.Children.Add(clear);

        var root = new Grid { RowSpacing = 8 };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(_log, 1);
        root.Children.Add(bar);
        root.Children.Add(_log);
        Content = root;

        this.WhenActivated(d =>
        {
            this.BindUnsafe(ViewModel, vm => vm.MsgFilter, v => v._filter.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AutoRefresh, v => v.AutoRefresh.IsOn).DisposeWith(d);
            ViewModel.ShowMsgInteraction.RegisterHandler(ctx =>
            {
                var count = _log.Text?.Split('\n').Length ?? 0;
                if (count > (ViewModel?.NumMaxMsg ?? 500))
                {
                    _log.Text = "----- Message cleared -----\n";
                }

                _log.Text += ctx.Input;
                if (ScrollEnd.IsOn)
                {
                    _log.SelectionStart = _log.Text?.Length ?? 0;
                }

                ctx.SetOutput(RxVoid.Default);
            }).DisposeWith(d);
        });
    }
}

public sealed class ClashProxiesPane : ReactiveUserControl<ClashProxiesViewModel>
{
    private readonly ListView _groups = new() { DisplayMemberPath = nameof(ClashProxyModel.Name) };
    private readonly ListView _details = new() { DisplayMemberPath = nameof(ClashProxyModel.Name) };
    public Button Reload { get; } = new() { Content = ResUI.menuProxiesReload };
    public Button GroupDelay { get; } = new() { Content = ResUI.menuDelaytest };
    public Button Delay { get; } = new() { Content = ResUI.menuDelaytest };
    public Button Select { get; } = new() { Content = ResUI.menuProxiesSelectActivity };
    public ComboBox Mode { get; } = new() { MinWidth = 140 };
    public ComboBox Sorting { get; } = new() { MinWidth = 140 };
    public ToggleSwitch AutoRefresh { get; } = new() { Header = ResUI.TbAutoRefresh };

    public ClashProxiesPane()
    {
        Sorting.Items.Add(ResUI.TbSortingDelay);
        Sorting.Items.Add(ResUI.TbSortingName);
        Sorting.Items.Add(ResUI.TbSortingDefault);
        var bar = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        bar.Children.Add(Reload);
        bar.Children.Add(GroupDelay);
        bar.Children.Add(Delay);
        bar.Children.Add(Select);
        bar.Children.Add(Mode);
        bar.Children.Add(Sorting);
        bar.Children.Add(AutoRefresh);
        var split = new Grid { ColumnSpacing = 8 };
        split.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(280) });
        split.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Grid.SetColumn(_details, 1);
        split.Children.Add(_groups);
        split.Children.Add(_details);
        var root = new Grid { RowSpacing = 8 };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(split, 1);
        root.Children.Add(bar);
        root.Children.Add(split);
        Content = root;

        this.WhenActivated(d =>
        {
            this.OneWayBindUnsafe(ViewModel, vm => vm.ProxyGroups, v => v._groups.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedGroup, v => v._groups.SelectedItem).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.ProxyDetails, v => v._details.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedDetail, v => v._details.SelectedItem).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.ClashModes, v => v.Mode.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.RuleModeSelected, v => v.Mode.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SortingSelected, v => v.Sorting.SelectedIndex).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AutoRefresh, v => v.AutoRefresh.IsOn).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ProxiesReloadCmd, v => v.Reload, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.GroupProxiesDelayTestCmd, v => v.GroupDelay, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ProxyDelayTestCmd, v => v.Delay, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ProxiesSelectActivityCmd, v => v.Select, "Click").DisposeWith(d);
            _details.DoubleTapped += (_, _) => Select.Command?.Execute(null);
        });
    }
}

public sealed class ClashConnectionsPane : ReactiveUserControl<ClashConnectionsViewModel>
{
    private readonly ListView _list = new();
    private readonly TextBox _filter = new() { Width = 220, PlaceholderText = "Host" };
    public Button Close { get; } = new() { Content = ResUI.menuConnectionClose };
    public Button CloseAll { get; } = new() { Content = ResUI.menuConnectionCloseAll };
    public ToggleSwitch AutoRefresh { get; } = new() { Header = ResUI.TbAutoRefresh };

    public ClashConnectionsPane()
    {
        _list.ItemTemplate = (Microsoft.UI.Xaml.DataTemplate)Microsoft.UI.Xaml.Markup.XamlReader.Load("""
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Grid ColumnSpacing="8">
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="70" />
      <ColumnDefinition Width="220" />
      <ColumnDefinition Width="90" />
      <ColumnDefinition Width="90" />
      <ColumnDefinition Width="80" />
      <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
    <TextBlock Text="{Binding Network}" />
    <TextBlock Grid.Column="1" Text="{Binding Host}" />
    <TextBlock Grid.Column="2" Text="{Binding UploadTraffic}" />
    <TextBlock Grid.Column="3" Text="{Binding DownloadTraffic}" />
    <TextBlock Grid.Column="4" Text="{Binding Elapsed}" />
    <TextBlock Grid.Column="5" Text="{Binding Chain}" />
  </Grid>
</DataTemplate>
""");
        var bar = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        bar.Children.Add(_filter);
        bar.Children.Add(Close);
        bar.Children.Add(CloseAll);
        bar.Children.Add(AutoRefresh);
        var root = new Grid { RowSpacing = 8 };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(_list, 1);
        root.Children.Add(bar);
        root.Children.Add(_list);
        Content = root;

        this.WhenActivated(d =>
        {
            this.OneWayBindUnsafe(ViewModel, vm => vm.ConnectionItems, v => v._list.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource, v => v._list.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.HostFilter, v => v._filter.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AutoRefresh, v => v.AutoRefresh.IsOn).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ConnectionCloseCmd, v => v.Close, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ConnectionCloseAllCmd, v => v.CloseAll, "Click").DisposeWith(d);
        });
    }
}

public sealed class StatusPane : ReactiveUserControl<StatusBarViewModel>
{
    public ComboBox SystemProxy { get; } = new() { MinWidth = 150 };
    public ComboBox Routings { get; } = new() { MinWidth = 180, DisplayMemberPath = nameof(RoutingItem.Remarks) };
    public ToggleSwitch Tun { get; } = new() { Header = ResUI.TbEnableTunAs };
    private readonly TextBlock _inbound = new();
    private readonly TextBlock _lan = new();
    private readonly TextBlock _server = new();
    private readonly TextBlock _info = new();
    private readonly TextBlock _proxySpeed = new();
    private readonly TextBlock _directSpeed = new();
    public TrayIcon? Tray { get; private set; }

    public StatusPane()
    {
        SystemProxy.Items.Add(ResUI.menuSystemProxyClear);
        SystemProxy.Items.Add(ResUI.menuSystemProxySet);
        SystemProxy.Items.Add(ResUI.menuSystemProxyNothing);
        SystemProxy.Items.Add(ResUI.menuSystemProxyPac);
        var root = new Grid { ColumnSpacing = 12, Padding = new Thickness(8, 4, 8, 4) };
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var left = new StackPanel();
        left.Children.Add(_inbound);
        left.Children.Add(_lan);
        var mid = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        mid.Children.Add(Tun);
        mid.Children.Add(SystemProxy);
        mid.Children.Add(Routings);
        var running = new StackPanel();
        running.Children.Add(_server);
        running.Children.Add(_info);
        mid.Children.Add(running);
        var right = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
        right.Children.Add(_proxySpeed);
        right.Children.Add(_directSpeed);
        Grid.SetColumn(mid, 1);
        Grid.SetColumn(right, 2);
        root.Children.Add(left);
        root.Children.Add(mid);
        root.Children.Add(right);
        Content = root;
        _server.DoubleTapped += (_, _) => ViewModel?.TestServerAvailability();
        _info.DoubleTapped += (_, _) => ViewModel?.TestServerAvailability();

        this.WhenActivated(d =>
        {
            this.OneWayBindUnsafe(ViewModel, vm => vm.InboundDisplay, v => v._inbound.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.InboundLanDisplay, v => v._lan.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.RunningServerDisplay, v => v._server.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.RunningInfoDisplay, v => v._info.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.SpeedProxyDisplay, v => v._proxySpeed.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.SpeedDirectDisplay, v => v._directSpeed.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableTun, v => v.Tun.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SystemProxySelected, v => v.SystemProxy.SelectedIndex).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.RoutingItems, v => v.Routings.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedRouting, v => v.Routings.SelectedItem).DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.BlRouting).Subscribe(show => Routings.Visibility = show ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
            ViewModel.SetClipboardDataInteraction.RegisterHandler(ctx =>
            {
                Native.SetClipboardText(ctx.Input);
                ctx.SetOutput(RxVoid.Default);
            }).DisposeWith(d);
            ViewModel.PasswordInputInteraction.RegisterHandler(async ctx =>
            {
                ctx.SetOutput(await UiKit.PasswordAsync(ResUI.TbSettingsLinuxSudoPassword));
            }).DisposeWith(d);
            ViewModel.DispatcherRefreshIconInteraction.RegisterHandler(ctx =>
            {
                Tray?.SetProxyIcon(AppManager.Instance.Config);
                ctx.SetOutput(RxVoid.Default);
            }).DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.RunningServerToolTipText).Subscribe(text =>
            {
                if (Tray is not null)
                {
                    Tray.Tip = text ?? Global.AppName;
                }
            }).DisposeWith(d);

            Tray = new TrayIcon();
            Tray.SetProxyIcon(AppManager.Instance.Config);
            Tray.DoubleClick += () => ViewModel?.NotifyLeftClickCmd.Execute().Subscribe();
            Tray.BuildMenu = BuildTray;
            UiKit.Defer(() => Tray?.Dispose()).DisposeWith(d);
        });
    }

    private List<TrayMenu> BuildTray()
    {
        var vm = ViewModel;
        if (vm is null)
        {
            return [];
        }

        void Run(ICommand command) => command.Execute(null);
        var menu = new List<TrayMenu>
        {
            new() { Text = ResUI.menuSystemProxyClear, Checked = vm.BlSystemProxyClear, Click = () => Run(vm.SystemProxyClearCmd) },
            new() { Text = ResUI.menuSystemProxySet, Checked = vm.BlSystemProxySet, Click = () => Run(vm.SystemProxySetCmd) },
            new() { Text = ResUI.menuSystemProxyNothing, Checked = vm.BlSystemProxyNothing, Click = () => Run(vm.SystemProxyNothingCmd) },
            new() { Text = ResUI.menuSystemProxyPac, Checked = vm.BlSystemProxyPac, Click = () => Run(vm.SystemProxyPacCmd) },
            TrayMenu.Separator,
            new() { Text = ResUI.menuAddServerViaClipboard, Click = () => Run(vm.AddServerViaClipboardCmd) },
            new() { Text = ResUI.menuAddServerViaScan, Click = () => Run(vm.AddServerViaScanCmd) },
            new() { Text = ResUI.menuSubUpdate, Click = () => Run(vm.SubUpdateCmd) },
            new() { Text = ResUI.menuSubUpdateViaProxy, Click = () => Run(vm.SubUpdateViaProxyCmd) },
            new() { Text = ResUI.menuCopyProxyCmdToClipboard, Click = () => Run(vm.CopyProxyCmdToClipboardCmd) },
        };

        if (vm.BlRouting && vm.RoutingItems.Count > 0)
        {
            menu.Add(TrayMenu.Separator);
            menu.Add(new TrayMenu
            {
                Text = ResUI.menuRouting,
                Children = vm.RoutingItems.Select(item => new TrayMenu
                {
                    Text = item.Remarks,
                    Checked = ReferenceEquals(item, vm.SelectedRouting),
                    Click = () => vm.SelectedRouting = item
                }).ToList()
            });
        }

        if (vm.BlServers && vm.Servers.Count > 0)
        {
            menu.Add(new TrayMenu
            {
                Text = ResUI.menuServers,
                Children = vm.Servers.Take(40).Select(item => new TrayMenu
                {
                    Text = item.Text,
                    Checked = ReferenceEquals(item, vm.SelectedServer),
                    Click = () => vm.SelectedServer = item
                }).ToList()
            });
        }

        menu.Add(TrayMenu.Separator);
        menu.Add(new TrayMenu
        {
            Text = ResUI.menuExit,
            Click = () => _ = AppManager.Instance.AppExitAsync(true)
        });
        return menu;
    }
}
