using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;
using WinRT.Interop;

namespace v2rayN.WinUI.Views;

public sealed class ShellWindow : WinUIWindow, IViewFor<MainWindowViewModel>
{
    private readonly ProfilesPane _profiles = new();
    private readonly MsgPane _msg = new();
    private readonly ClashProxiesPane _proxies = new();
    private readonly ClashConnectionsPane _connections = new();
    private readonly StatusPane _status = new();
    private readonly InfoBar _snack = new() { IsClosable = true, Severity = InfoBarSeverity.Informational };
    private readonly Button _update = new() { Content = ResUI.menuCheckUpdate, HorizontalAlignment = HorizontalAlignment.Left };
    private ContentControl? _fontHost;
    private readonly Grid _body = new();
    private readonly TabView _sideTabs = new();
    private readonly TabView _mainTabs = new();
    private readonly ColumnDefinition _colLeft = new() { Width = new GridLength(1, GridUnitType.Star) };
    private readonly ColumnDefinition _colRight = new() { Width = new GridLength(1, GridUnitType.Star) };
    private readonly RowDefinition _rowTop = new() { Height = new GridLength(1, GridUnitType.Star) };
    private readonly RowDefinition _rowBottom = new() { Height = new GridLength(1, GridUnitType.Star) };
    private readonly TabViewItem _msgTab;
    private readonly TabViewItem _proxyTab;
    private readonly TabViewItem _connTab;
    private readonly TabViewItem _msgTabMain;
    private readonly TabViewItem _proxyTabMain;
    private readonly TabViewItem _connTabMain;
    private bool _exit;
    private EGirdOrientation _orientation = EGirdOrientation.Vertical;

    private readonly MenuFlyoutItem _addVmess = Item(ResUI.menuAddVmessServer);
    private readonly MenuFlyoutItem _addVless = Item(ResUI.menuAddVlessServer);
    private readonly MenuFlyoutItem _addSs = Item(ResUI.menuAddShadowsocksServer);
    private readonly MenuFlyoutItem _addSocks = Item(ResUI.menuAddSocksServer);
    private readonly MenuFlyoutItem _addHttp = Item(ResUI.menuAddHttpServer);
    private readonly MenuFlyoutItem _addTrojan = Item(ResUI.menuAddTrojanServer);
    private readonly MenuFlyoutItem _addHy2 = Item(ResUI.menuAddHysteria2Server);
    private readonly MenuFlyoutItem _addTuic = Item(ResUI.menuAddTuicServer);
    private readonly MenuFlyoutItem _addWg = Item(ResUI.menuAddWireguardServer);
    private readonly MenuFlyoutItem _addAnytls = Item(ResUI.menuAddAnytlsServer);
    private readonly MenuFlyoutItem _addNaive = Item(ResUI.menuAddNaiveServer);
    private readonly MenuFlyoutItem _addMasque = Item(ResUI.menuAddMasqueServer);
    private readonly MenuFlyoutItem _addCustom = Item(ResUI.menuAddCustomServer);
    private readonly MenuFlyoutItem _addOutbound = Item(ResUI.menuAddCustomOutboundServer);
    private readonly MenuFlyoutItem _addPolicy = Item(ResUI.menuAddPolicyGroupServer);
    private readonly MenuFlyoutItem _addChain = Item(ResUI.menuAddProxyChainServer);
    private readonly MenuFlyoutItem _addClip = Item(ResUI.menuAddServerViaClipboard);
    private readonly MenuFlyoutItem _addScan = Item(ResUI.menuAddServerViaScan);
    private readonly MenuFlyoutItem _addImage = Item(ResUI.menuAddServerViaImage);
    private readonly MenuFlyoutItem _subSetting = Item(ResUI.menuSubSetting);
    private readonly MenuFlyoutItem _subUpdate = Item(ResUI.menuSubUpdate);
    private readonly MenuFlyoutItem _subUpdateProxy = Item(ResUI.menuSubUpdateViaProxy);
    private readonly MenuFlyoutItem _subGroup = Item(ResUI.menuSubGroupUpdate);
    private readonly MenuFlyoutItem _subGroupProxy = Item(ResUI.menuSubGroupUpdateViaProxy);
    private readonly MenuFlyoutItem _option = Item(ResUI.menuOptionSetting);
    private readonly MenuFlyoutItem _routing = Item(ResUI.menuRoutingSetting);
    private readonly MenuFlyoutItem _dns = Item(ResUI.menuDNSSetting);
    private readonly MenuFlyoutItem _template = Item(ResUI.menuFullConfigTemplate);
    private readonly MenuFlyoutItem _hotkey = Item(ResUI.menuGlobalHotkeySetting);
    private readonly MenuFlyoutItem _admin = Item(ResUI.menuRebootAsAdmin);
    private readonly MenuFlyoutItem _clearStats = Item(ResUI.menuClearServerStatistics);
    private readonly MenuFlyoutItem _presetDefault = Item(ResUI.menuRegionalPresetsDefault);
    private readonly MenuFlyoutItem _presetRussia = Item(ResUI.menuRegionalPresetsRussia);
    private readonly MenuFlyoutItem _presetIran = Item(ResUI.menuRegionalPresetsIran);
    private readonly MenuFlyoutItem _reload = Item(ResUI.menuReload);

    public MainWindowViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (MainWindowViewModel?)value;
    }

    public ShellWindow()
    {
        var config = AppManager.Instance.Config;
        Title = $"{Utils.GetVersion()} - {(Utils.IsAdministrator() ? ResUI.RunAsAdmin : ResUI.NotRunAsAdmin)}";
        AppHost.Place(this, "MainWindow", 1100, 760);

        // A control can have only one parent. The other tab set stays empty until ApplyLayout attaches it.
        _msgTab = Tab(ResUI.MsgInformationTitle, _msg);
        _proxyTab = Tab(ResUI.TbProxies, _proxies);
        _connTab = Tab(ResUI.TbConnections, _connections);
        _msgTabMain = new TabViewItem { Header = ResUI.MsgInformationTitle };
        _proxyTabMain = new TabViewItem { Header = ResUI.TbProxies };
        _connTabMain = new TabViewItem { Header = ResUI.TbConnections };
        _sideTabs.TabItems.Add(_msgTab);
        _sideTabs.TabItems.Add(_proxyTab);
        _sideTabs.TabItems.Add(_connTab);

        var root = new Grid();
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.Children.Add(BuildMenu());
        Grid.SetRow(_body, 1);
        root.Children.Add(_body);
        var statusRow = new Grid();
        statusRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        statusRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        statusRow.Children.Add(_status);
        Grid.SetColumn(_update, 1);
        _update.Margin = new Thickness(8);
        _update.Visibility = Visibility.Collapsed;
        statusRow.Children.Add(_update);
        Grid.SetRow(statusRow, 2);
        root.Children.Add(statusRow);
        Grid.SetRow(_snack, 3);
        root.Children.Add(_snack);
        var host = new ContentControl
        {
            Content = root,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch
        };
        if (config.UiItem.CurrentFontSize >= Global.MinFontSize)
        {
            host.FontSize = config.UiItem.CurrentFontSize;
        }

        _fontHost = host;
        var fill = new Grid();
        fill.Children.Add(host);
        Content = fill;
        root.KeyDown += OnKeyDown;
        _update.Click += (_, _) => OpenCheckUpdate();

        ViewModel = new MainWindowViewModel();
        _profiles.ViewModel = ViewModel.ProfilesViewModel;
        _msg.ViewModel = ViewModel.MsgViewModel;
        _proxies.ViewModel = ViewModel.ClashProxiesViewModel;
        _connections.ViewModel = ViewModel.ClashConnectionsViewModel;
        _status.ViewModel = ViewModel.StatusBarViewModel;

        this.WhenActivated(d =>
        {
            this.BindCommandUnsafe(ViewModel, vm => vm.AddVmessServerCmd, v => v._addVmess, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddVlessServerCmd, v => v._addVless, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddShadowsocksServerCmd, v => v._addSs, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddSocksServerCmd, v => v._addSocks, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddHttpServerCmd, v => v._addHttp, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddTrojanServerCmd, v => v._addTrojan, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddHysteria2ServerCmd, v => v._addHy2, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddTuicServerCmd, v => v._addTuic, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddWireguardServerCmd, v => v._addWg, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddAnytlsServerCmd, v => v._addAnytls, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddNaiveServerCmd, v => v._addNaive, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddMasqueServerCmd, v => v._addMasque, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddCustomServerCmd, v => v._addCustom, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddCustomOutboundServerCmd, v => v._addOutbound, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddPolicyGroupServerCmd, v => v._addPolicy, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddProxyChainServerCmd, v => v._addChain, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddServerViaClipboardCmd, v => v._addClip, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddServerViaScanCmd, v => v._addScan, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddServerViaImageCmd, v => v._addImage, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SubSettingCmd, v => v._subSetting, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SubUpdateCmd, v => v._subUpdate, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SubUpdateViaProxyCmd, v => v._subUpdateProxy, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SubGroupUpdateCmd, v => v._subGroup, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SubGroupUpdateViaProxyCmd, v => v._subGroupProxy, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.OptionSettingCmd, v => v._option, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RoutingSettingCmd, v => v._routing, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.DNSSettingCmd, v => v._dns, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.FullConfigTemplateCmd, v => v._template, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RebootAsAdminCmd, v => v._admin, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ClearServerStatisticsCmd, v => v._clearStats, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RegionalPresetDefaultCmd, v => v._presetDefault, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RegionalPresetRussiaCmd, v => v._presetRussia, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RegionalPresetIranCmd, v => v._presetIran, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ReloadCmd, v => v._reload, "Click").DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.BlReloadEnabled, v => v._reload.IsEnabled).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.BlNewUpdate, v => v._update.Visibility, show => show ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.MainGirdOrientation)
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(ApplyLayout)
                .DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.ShowClashUI)
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(show =>
                {
                    var visibility = show ? Visibility.Visible : Visibility.Collapsed;
                    _msgTab.Visibility = visibility;
                    _proxyTab.Visibility = visibility;
                    _connTab.Visibility = visibility;
                    _proxyTabMain.Visibility = visibility;
                    _connTabMain.Visibility = visibility;
                })
                .DisposeWith(d);

            ViewModel.ReadTextFromClipboardInteraction.RegisterHandler(interaction =>
            {
                interaction.SetOutput(Native.GetClipboardText());
            }).DisposeWith(d);
            ViewModel.ScanScreenInteraction.RegisterHandler(interaction =>
            {
                ShowHide(false);
                interaction.SetOutput(Native.CaptureScreen());
                ShowHide(true);
            }).DisposeWith(d);
            ViewModel.BrowseImageFileInteraction.RegisterHandler(interaction =>
            {
                interaction.SetOutput(Native.OpenFile(out var file, "PNG|*.png|All|*.*") == true ? file : null);
            }).DisposeWith(d);
            ViewModel.ShowHideWindowInteraction.RegisterHandler(interaction =>
            {
                ShowHide(interaction.Input);
                interaction.SetOutput(RxVoid.Default);
            }).DisposeWith(d);

            AppEvents.SendSnackMsgRequested.AsObservable()
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(text =>
                {
                    _snack.Message = text;
                    _snack.IsOpen = true;
                })
                .DisposeWith(d);
            AppEvents.AppExitRequested.AsObservable()
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(_ => StoreUi())
                .DisposeWith(d);
            AppEvents.ShutdownRequested.AsObservable()
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    _exit = true;
                    Close();
                    Environment.Exit(0);
                })
                .DisposeWith(d);
        });

        _hotkey.Click += async (_, _) =>
        {
            HotkeyHub.Instance.IsPause = true;
            try
            {
                if (ViewModel is not null)
                {
                    await ViewModel.GlobalHotkeySettingCmd.Execute().ToTask();
                }
            }
            finally
            {
                HotkeyHub.Instance.IsPause = false;
                HotkeyHub.Instance.Reload();
            }
        };

        var hwnd = WindowNative.GetWindowHandle(this);
        HotkeyHub.Instance.Attach(hwnd);
        HotkeyHub.Instance.Triggered += OnHotkey;
        var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hwnd));
        appWindow.Closing += (_, args) =>
        {
            if (_exit)
            {
                return;
            }

            args.Cancel = true;
            ShowHide(false);
        };

        if (Program.ProgramStarted is not null)
        {
            ThreadPool.RegisterWaitForSingleObject(Program.ProgramStarted, (_, _) =>
            {
                AppHost.Queue.TryEnqueue(() => ShowHide(true));
            }, null, -1, false);
        }

        if (config.UiItem.AutoHideStartup)
        {
            AppHost.Show(this, false);
        }

        ApplyTheme(config.UiItem.CurrentTheme);
    }

    private static MenuFlyoutItem Item(string text) => new() { Text = text };

    private static TabViewItem Tab(string header, UIElement content) => new() { Header = header, Content = content };

    private MenuBar BuildMenu()
    {
        var bar = new MenuBar();
        bar.Items.Add(Menu(ResUI.menuServers, _addClip, _addScan, _addImage, null, _addCustom, _addPolicy, _addChain, _addOutbound, null, _addVmess, _addVless, _addSs, _addTrojan, _addHy2, _addWg, _addMasque, _addSocks, _addHttp, null, _addTuic, _addAnytls, _addNaive));
        bar.Items.Add(Menu(ResUI.menuSubscription, _subSetting, null, _subUpdate, _subUpdateProxy, _subGroup, _subGroupProxy));
        var settings = Menu(ResUI.menuSetting, _option, _routing, _dns, _template, _hotkey, null, _admin);
        settings.Items.Add(new MenuFlyoutItem { Text = ResUI.TbSettingsSetUWP });
        ((MenuFlyoutItem)settings.Items[^1]).Click += (_, _) => ProcUtils.ProcessStart(Utils.GetBinPath("EnableLoopback.exe"));
        settings.Items.Add(_clearStats);
        settings.Items.Add(new MenuFlyoutSeparator());
        var presets = new MenuFlyoutSubItem { Text = ResUI.menuRegionalPresets };
        presets.Items.Add(_presetDefault);
        presets.Items.Add(_presetRussia);
        presets.Items.Add(_presetIran);
        settings.Items.Add(presets);
        var backup = new MenuFlyoutItem { Text = ResUI.menuBackupAndRestore };
        backup.Click += (_, _) => OpenBackup();
        settings.Items.Add(backup);
        var openDir = new MenuFlyoutItem { Text = ResUI.menuOpenTheFileLocation };
        openDir.Click += (_, _) => ViewModel?.OpenTheFileLocationCmd.Execute().Subscribe();
        settings.Items.Add(openDir);
        settings.Items.Add(new MenuFlyoutSeparator());
        settings.Items.Add(ThemeMenu());
        bar.Items.Add(settings);

        var help = new MenuBarItem { Title = ResUI.menuHelp };
        var check = new MenuFlyoutItem { Text = ResUI.menuCheckUpdate };
        check.Click += (_, _) => OpenCheckUpdate();
        help.Items.Add(check);
        foreach (var core in CoreInfoManager.Instance.GetCoreInfo().Where(t => t.CoreType is not ECoreType.v2fly and not ECoreType.hysteria))
        {
            var url = core.Url?.Replace("/releases", "");
            var item = new MenuFlyoutItem
            {
                Text = string.Format(ResUI.menuWebsiteItem, core.CoreType.ToString().Replace("_", "")).UpperFirstChar()
            };
            item.Click += (_, _) => ProcUtils.ProcessStart(url);
            help.Items.Add(item);
        }

        bar.Items.Add(help);
        var reload = new MenuBarItem { Title = ResUI.menuReload };
        reload.Items.Add(_reload);
        bar.Items.Add(reload);
        var promo = new MenuBarItem { Title = ResUI.menuPromotion };
        var promoItem = new MenuFlyoutItem { Text = ResUI.menuPromotion };
        promoItem.Click += (_, _) => ProcUtils.ProcessStart($"{Utils.Base64Decode(Global.PromotionUrl)}?t={DateTime.Now.Ticks}");
        promo.Items.Add(promoItem);
        bar.Items.Add(promo);
        var close = new MenuBarItem { Title = ResUI.menuClose };
        var closeItem = new MenuFlyoutItem { Text = ResUI.menuClose };
        closeItem.Click += (_, _) =>
        {
            StoreUi();
            ShowHide(false);
        };
        close.Items.Add(closeItem);
        bar.Items.Add(close);
        return bar;
    }

    private static MenuBarItem Menu(string title, params MenuFlyoutItem?[] items)
    {
        var menu = new MenuBarItem { Title = title };
        foreach (var item in items)
        {
            menu.Items.Add(item is null ? new MenuFlyoutSeparator() : item);
        }

        return menu;
    }

    private MenuFlyoutSubItem ThemeMenu()
    {
        var theme = new MenuFlyoutSubItem { Text = ResUI.TbSettingsTheme };
        foreach (var name in new[] { nameof(ETheme.FollowSystem), nameof(ETheme.Dark), nameof(ETheme.Light) })
        {
            var item = new MenuFlyoutItem { Text = name };
            item.Click += async (_, _) =>
            {
                var config = AppManager.Instance.Config;
                config.UiItem.CurrentTheme = name;
                ApplyTheme(name);
                await ConfigHandler.SaveConfig(config);
            };
            theme.Items.Add(item);
        }

        var language = new MenuFlyoutSubItem { Text = ResUI.TbSettingsLanguage };
        foreach (var option in Global.LanguageOptions)
        {
            var item = new MenuFlyoutItem { Text = option.Display };
            item.Click += async (_, _) =>
            {
                var config = AppManager.Instance.Config;
                if (config.UiItem.CurrentLanguage == option.Value)
                {
                    return;
                }

                config.UiItem.CurrentLanguage = option.Value;
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(option.Value);
                await ConfigHandler.SaveConfig(config);
                _snack.Message = ResUI.NeedRebootTips;
                _snack.IsOpen = true;
            };
            language.Items.Add(item);
        }

        theme.Items.Add(language);
        var fonts = new MenuFlyoutSubItem { Text = ResUI.TbSettingsFontSize };
        foreach (var size in Enumerable.Range(Global.MinFontSize, Global.MinFontSizeCount))
        {
            var item = new MenuFlyoutItem { Text = size.ToString() };
            item.Click += async (_, _) =>
            {
                var config = AppManager.Instance.Config;
                config.UiItem.CurrentFontSize = size;
                if (_fontHost is not null)
                {
                    _fontHost.FontSize = size;
                }

                await ConfigHandler.SaveConfig(config);
            };
            fonts.Items.Add(item);
        }

        theme.Items.Add(fonts);
        return theme;
    }

    private void ApplyTheme(string? name)
    {
        ThemeChrome.Apply(this, name);
    }

    private void ApplyLayout(EGirdOrientation orientation)
    {
        _orientation = orientation;
        _body.Children.Clear();
        _sideTabs.TabItems.Clear();
        _mainTabs.TabItems.Clear();
        Detach(_profiles);
        Detach(_msg);
        Detach(_proxies);
        Detach(_connections);

        var config = AppManager.Instance.Config;
        var first = config.UiItem.MainGirdHeight1 > 0 ? config.UiItem.MainGirdHeight1 : 1;
        var second = config.UiItem.MainGirdHeight2 > 0 ? config.UiItem.MainGirdHeight2 : 1;

        if (orientation == EGirdOrientation.Tab)
        {
            _mainTabs.TabItems.Add(Tab(ResUI.menuServers, _profiles));
            _msgTabMain.Content = _msg;
            _proxyTabMain.Content = _proxies;
            _connTabMain.Content = _connections;
            _mainTabs.TabItems.Add(_msgTabMain);
            _mainTabs.TabItems.Add(_proxyTabMain);
            _mainTabs.TabItems.Add(_connTabMain);
            _mainTabs.SelectedIndex = Math.Clamp(ViewModel?.TabMainSelectedIndex ?? 0, 0, _mainTabs.TabItems.Count - 1);
            _mainTabs.SelectionChanged += TabsChanged;
            _body.Children.Add(_mainTabs);
            return;
        }

        var grid = new Grid();
        if (orientation == EGirdOrientation.Horizontal)
        {
            _colLeft.Width = new GridLength(first, GridUnitType.Star);
            _colRight.Width = new GridLength(second, GridUnitType.Star);
            grid.ColumnDefinitions.Add(_colLeft);
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(_colRight);
            Grid.SetColumn(_profiles, 0);
            grid.Children.Add(_profiles);
            var split = new PaneSplitter(_colLeft, null) { Width = 8 };
            Grid.SetColumn(split, 1);
            grid.Children.Add(split);
            _msgTab.Content = _msg;
            _proxyTab.Content = _proxies;
            _connTab.Content = _connections;
            _sideTabs.TabItems.Add(_msgTab);
            _sideTabs.TabItems.Add(_proxyTab);
            _sideTabs.TabItems.Add(_connTab);
            Grid.SetColumn(_sideTabs, 2);
            grid.Children.Add(_sideTabs);
        }
        else
        {
            _rowTop.Height = new GridLength(first, GridUnitType.Star);
            _rowBottom.Height = new GridLength(second, GridUnitType.Star);
            grid.RowDefinitions.Add(_rowTop);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(_rowBottom);
            Grid.SetRow(_profiles, 0);
            grid.Children.Add(_profiles);
            var split = new PaneSplitter(null, _rowTop) { Height = 8 };
            Grid.SetRow(split, 1);
            grid.Children.Add(split);
            _msgTab.Content = _msg;
            _proxyTab.Content = _proxies;
            _connTab.Content = _connections;
            _sideTabs.TabItems.Add(_msgTab);
            _sideTabs.TabItems.Add(_proxyTab);
            _sideTabs.TabItems.Add(_connTab);
            Grid.SetRow(_sideTabs, 2);
            grid.Children.Add(_sideTabs);
        }

        _body.Children.Add(grid);
    }

    private void TabsChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel is not null && sender is TabView tabs)
        {
            ViewModel.TabMainSelectedIndex = tabs.SelectedIndex;
        }
    }

    private static void Detach(FrameworkElement element)
    {
        switch (element.Parent)
        {
            case Panel panel:
                panel.Children.Remove(element);
                break;
            case ContentControl control when ReferenceEquals(control.Content, element):
                control.Content = null;
                break;
        }
    }

    private void OnHotkey(EGlobalHotkey key)
    {
        AppHost.Queue.TryEnqueue(() =>
        {
            switch (key)
            {
                case EGlobalHotkey.ShowForm:
                    ShowHide(null);
                    break;
                case EGlobalHotkey.SystemProxyClear:
                case EGlobalHotkey.SystemProxySet:
                case EGlobalHotkey.SystemProxyUnchanged:
                case EGlobalHotkey.SystemProxyPac:
                    AppEvents.SysProxyChangeRequested.Publish((ESysProxyType)((int)key - 1));
                    break;
            }
        });
    }

    private void ShowHide(bool? show)
    {
        var visible = show ?? !AppManager.Instance.ShowInTaskbar;
        if (visible)
        {
            AppHost.Show(this, true);
        }
        else
        {
            AppHost.Show(this, false);
        }

        AppManager.Instance.ShowInTaskbar = visible;
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var ctrl = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
        if (ctrl && e.Key == VirtualKey.V && e.OriginalSource is not TextBox)
        {
            ViewModel?.AddServerViaClipboardCmd.Execute().Subscribe();
            e.Handled = true;
        }
        else if (ctrl && e.Key == VirtualKey.S)
        {
            ViewModel?.AddServerViaScanCmd.Execute().Subscribe();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.F5)
        {
            ViewModel?.ReloadCmd.Execute().Subscribe();
            e.Handled = true;
        }
    }

    private void StoreUi()
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hwnd));
        var config = AppManager.Instance.Config;
        ConfigHandler.SaveWindowSizeItem(config, "MainWindow", appWindow.Size.Width, appWindow.Size.Height);
        if (_orientation == EGirdOrientation.Horizontal)
        {
            ConfigHandler.SaveMainGirdHeight(config, _colLeft.ActualWidth, _colRight.ActualWidth);
        }
        else if (_orientation == EGirdOrientation.Vertical)
        {
            ConfigHandler.SaveMainGirdHeight(config, _rowTop.ActualHeight, _rowBottom.ActualHeight);
        }
    }

    private void OpenCheckUpdate()
    {
        var window = new CheckUpdateWindow();
        window.ViewModel = ViewModel?.CheckUpdateViewModel;
        AppHost.Place(window, "CheckUpdateWindow", 760, 560);
        AppHost.Present(window, show: true, stealFocus: false);
        AppEvents.HasUpdateNotified.Publish(false);
    }

    private void OpenBackup()
    {
        var window = new BackupAndRestoreWindow();
        window.ViewModel = ViewModel?.BackupAndRestoreViewModel;
        AppHost.Place(window, "BackupAndRestoreWindow", 720, 520);
        AppHost.Present(window, show: true, stealFocus: false);
    }

    private sealed class PaneSplitter : ContentControl
    {
        private readonly ColumnDefinition? _column;
        private readonly RowDefinition? _row;
        private bool _drag;
        private double _origin;
        private double _start;

        public PaneSplitter(ColumnDefinition? column, RowDefinition? row)
        {
            _column = column;
            _row = row;
            Background = new SolidColorBrush(Windows.UI.Color.FromArgb(32, 128, 128, 128));
            PointerPressed += (_, e) =>
            {
                _drag = true;
                var point = e.GetCurrentPoint(this).Position;
                _origin = _column is null ? point.Y : point.X;
                _start = _column?.ActualWidth ?? _row?.ActualHeight ?? 0;
                CapturePointer(e.Pointer);
            };
            PointerReleased += (_, e) =>
            {
                _drag = false;
                ReleasePointerCapture(e.Pointer);
            };
            PointerMoved += (_, e) =>
            {
                if (!_drag)
                {
                    return;
                }

                var point = e.GetCurrentPoint(this).Position;
                var next = Math.Max(80, _start + (_column is null ? point.Y - _origin : point.X - _origin));
                if (_column is not null)
                {
                    _column.Width = new GridLength(next);
                }

                if (_row is not null)
                {
                    _row.Height = new GridLength(next);
                }
            };
        }
    }
}
