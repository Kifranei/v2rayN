using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;

namespace v2rayN.WinUI.Views;

public sealed class FullConfigTemplateWindow : ReactiveWindow<FullConfigTemplateViewModel>
{
    private readonly ToggleSwitch _rayOn;
    private readonly TextBox _ray;
    private readonly TextBox _rayTun;
    private readonly ToggleSwitch _rayOnly;
    private readonly TextBox _rayDetour;
    private readonly ToggleSwitch _sbOn;
    private readonly TextBox _sb;
    private readonly TextBox _sbTun;
    private readonly ToggleSwitch _sbOnly;
    private readonly TextBox _sbDetour;
    private readonly Button _save;

    public FullConfigTemplateWindow()
    {
        Title = ResUI.menuFullConfigTemplate;
        AppHost.Place(this, "FullConfigTemplateWindow", 900, 720);
        var form = UiKit.Stack();
        _rayOn = UiKit.Toggle(form, "Xray");
        _ray = UiKit.Text(form, "Xray", multi: true);
        _rayTun = UiKit.Text(form, "Xray TUN", multi: true);
        _rayOnly = UiKit.Toggle(form, "Proxy outbound only");
        _rayDetour = UiKit.Text(form, "Proxy detour");
        _sbOn = UiKit.Toggle(form, "sing-box");
        _sb = UiKit.Text(form, "sing-box", multi: true);
        _sbTun = UiKit.Text(form, "sing-box TUN", multi: true);
        _sbOnly = UiKit.Toggle(form, "Proxy outbound only");
        _sbDetour = UiKit.Text(form, "Proxy detour");
        (_save, Content) = SaveBar(form);
        this.WhenActivated(d =>
        {
            this.BindUnsafe(ViewModel, vm => vm.EnableFullConfigTemplate4Ray, v => v._rayOn.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FullConfigTemplate4Ray, v => v._ray.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FullTunConfigTemplate4Ray, v => v._rayTun.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AddProxyOnly4Ray, v => v._rayOnly.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.ProxyDetour4Ray, v => v._rayDetour.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableFullConfigTemplate4Singbox, v => v._sbOn.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FullConfigTemplate4Singbox, v => v._sb.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FullTunConfigTemplate4Singbox, v => v._sbTun.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AddProxyOnly4Singbox, v => v._sbOnly.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.ProxyDetour4Singbox, v => v._sbDetour.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);
        });
    }

    internal static (Button Save, Grid Root) SaveBar(StackPanel form)
    {
        var save = new Button { Content = ResUI.TbConfirm, MinWidth = 120, HorizontalAlignment = HorizontalAlignment.Left };
        var bar = new StackPanel { Orientation = Orientation.Horizontal, Padding = new Thickness(12) };
        bar.Children.Add(save);
        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        Grid.SetRow(bar, 1);
        grid.Children.Add(UiKit.Scroll(form));
        grid.Children.Add(bar);
        return (save, grid);
    }
}

public sealed class GlobalHotkeySettingWindow : ReactiveWindow<GlobalHotkeySettingViewModel>
{
    private readonly TextBox[] _boxes;
    private readonly Button _save;
    private readonly Button _reset;

    public GlobalHotkeySettingWindow()
    {
        Title = ResUI.menuGlobalHotkeySetting;
        AppHost.Place(this, "GlobalHotkeySettingWindow", 640, 420);
        var form = UiKit.Stack();
        form.Children.Add(new TextBlock { Text = ResUI.TbGlobalHotkeySettingTip, TextWrapping = TextWrapping.Wrap });
        var labels = new[] { ResUI.TbDisplayGUI, ResUI.TbClearSystemProxy, ResUI.TbSetSystemProxy, ResUI.TbNotChangeSystemProxy, ResUI.TbSystemProxyPac };
        _boxes = labels.Select(label =>
        {
            var box = UiKit.Text(form, label);
            box.IsReadOnly = true;
            return box;
        }).ToArray();
        _reset = new Button { Content = ResUI.TbReset, MinWidth = 120 };
        form.Children.Add(_reset);
        (_save, Content) = FullConfigTemplateWindow.SaveBar(form);
        for (var i = 0; i < _boxes.Length; i++)
        {
            var index = i;
            _boxes[i].KeyDown += (_, e) => Capture(index, e);
        }

        _reset.Click += (_, _) =>
        {
            ViewModel?.ResetKeyEventItem();
            Paint();
        };
        Closed += (_, _) => HotkeyHub.Instance.IsPause = false;
        HotkeyHub.Instance.IsPause = true;
        this.WhenActivated(d =>
        {
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);
            Paint();
        });
    }

    private void Paint()
    {
        for (var i = 0; i < _boxes.Length; i++)
        {
            _boxes[i].Text = WpfKeys.Format(ViewModel?.GetKeyEventItem((EGlobalHotkey)i));
        }
    }

    private void Capture(int index, KeyRoutedEventArgs e)
    {
        e.Handled = true;
        if (ViewModel is null)
        {
            return;
        }

        if (e.Key is VirtualKey.Control or VirtualKey.Shift or VirtualKey.Menu or VirtualKey.LeftWindows or VirtualKey.RightWindows)
        {
            return;
        }

        var item = ViewModel.GetKeyEventItem((EGlobalHotkey)index);
        item.KeyCode = WpfKeys.ToWpf((int)e.Key);
        item.Control = Down(VirtualKey.Control);
        item.Shift = Down(VirtualKey.Shift);
        item.Alt = Down(VirtualKey.Menu);
        _boxes[index].Text = WpfKeys.Format(item);
    }

    private static bool Down(VirtualKey key) => InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(CoreVirtualKeyStates.Down);
}

public sealed class SubSettingWindow : ReactiveWindow<SubSettingViewModel>
{
    private readonly ListView _list;
    private readonly MenuFlyoutItem _add = UiKit.Item(ResUI.menuSubAdd);
    private readonly MenuFlyoutItem _edit = UiKit.Item(ResUI.menuSubEdit);
    private readonly MenuFlyoutItem _delete = UiKit.Item(ResUI.menuSubDelete);
    private readonly MenuFlyoutItem _share = UiKit.Item(ResUI.menuSubShare);

    public SubSettingWindow()
    {
        Title = ResUI.menuSubSetting;
        AppHost.Place(this, "SubSettingWindow", 760, 560);
        var form = UiKit.Stack();
        var bar = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        bar.Children.Add(new Button { Content = ResUI.menuSubAdd, Command = _add.Command });
        foreach (var item in new[] { _add, _edit, _delete, _share })
        {
            var button = new Button { Content = item.Text };
            button.Click += (_, _) => item.Command?.Execute(null);
            bar.Children.Add(button);
        }

        form.Children.Add(bar);
        _list = UiKit.List(form, nameof(SubItem.Remarks), 420);
        _list.ContextFlyout = new MenuFlyout { Items = { _add, _edit, _delete, _share } };
        _list.DoubleTapped += (_, _) => _edit.Command?.Execute(null);
        Content = UiKit.Scroll(form);
        this.WhenActivated(d =>
        {
            this.OneWayBindUnsafe(ViewModel, vm => vm.SubItems, v => v._list.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource, v => v._list.SelectedItem).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SubAddCmd, v => v._add, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SubEditCmd, v => v._edit, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SubDeleteCmd, v => v._delete, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SubShareCmd, v => v._share, "Click").DisposeWith(d);
            ViewModel.ShowYesNoInteraction.RegisterHandler(async interaction =>
            {
                interaction.SetOutput(await UiKit.YesNo(interaction.Input));
            }).DisposeWith(d);
            ViewModel.ShareSubInteraction.RegisterHandler(async interaction =>
            {
                if (interaction.Input.IsNotEmpty())
                {
                    await UiKit.ShowQrAsync(interaction.Input);
                }

                interaction.SetOutput(RxVoid.Default);
            }).DisposeWith(d);
        });
    }
}

public sealed class SubEditWindow : ReactiveWindow<SubEditViewModel>
{
    private readonly TextBox _remarks;
    private readonly TextBox _url;
    private readonly TextBox _more;
    private readonly ToggleSwitch _enabled;
    private readonly TextBox _interval;
    private readonly TextBox _agent;
    private readonly TextBox _headers;
    private readonly TextBox _sort;
    private readonly TextBox _filter;
    private readonly AutoSuggestBox _convert;
    private readonly TextBox _prev;
    private readonly TextBox _next;
    private readonly TextBox _socks;
    private readonly TextBox _memo;
    private readonly AutoSuggestBox _core;
    private readonly AutoSuggestBox _resolve;
    private readonly Button _pickPrev;
    private readonly Button _pickNext;
    private readonly Button _save;

    public SubEditWindow()
    {
        Title = ResUI.menuSubEdit;
        AppHost.Place(this, "SubEditWindow", 760, 720);
        var form = UiKit.Stack();
        _remarks = UiKit.Text(form, ResUI.TbRemarks);
        _url = UiKit.Text(form, "URL");
        _more = UiKit.Text(form, "More URL", multi: true);
        _enabled = UiKit.Toggle(form, ResUI.TbSettingsStatistics);
        _interval = UiKit.Text(form, ResUI.TbSettingsAutoUpdateInterval);
        _agent = UiKit.Text(form, ResUI.TbSettingsDefUserAgent);
        _headers = UiKit.Text(form, "Headers", multi: true);
        _sort = UiKit.Text(form, "Sort");
        _filter = UiKit.Text(form, "Filter");
        _convert = UiKit.Suggest(form, "Convert", Global.SubConvertTargets);
        _prev = UiKit.Text(form, "Previous profile");
        _pickPrev = UiKit.Button(form, "...");
        _next = UiKit.Text(form, "Next profile");
        _pickNext = UiKit.Button(form, "...");
        _socks = UiKit.Text(form, ResUI.TbPreSocksPort4Sub);
        _memo = UiKit.Text(form, "Memo", multi: true);
        _core = UiKit.Suggest(form, ResUI.TbSettingsCoreType, Utils.GetEnumNames<ECoreType>().Where(t => t != nameof(ECoreType.v2rayN)).AppendEmpty());
        _resolve = UiKit.Suggest(form, "Pre-resolve", Global.DomainDNS4PreResolveAddress);
        (_save, Content) = FullConfigTemplateWindow.SaveBar(form);
        this.WhenActivated(d =>
        {
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Remarks, v => v._remarks.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Url, v => v._url.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.MoreUrl, v => v._more.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Enabled, v => v._enabled.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.AutoUpdateInterval, v => v._interval.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.UserAgent, v => v._agent.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.RequestHeaders, v => v._headers.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Sort, v => v._sort.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Filter, v => v._filter.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.ConvertTarget, v => v._convert.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.PrevProfile, v => v._prev.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.NextProfile, v => v._next.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.PreSocksPort, v => v._socks.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Memo, v => v._memo.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CustomCoreType, v => v._core.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.PreResolver, v => v._resolve.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SelectPrevProfileCmd, v => v._pickPrev, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SelectNextProfileCmd, v => v._pickNext, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);
        });
    }
}

public sealed class RoutingSettingWindow : ReactiveWindow<RoutingSettingViewModel>
{
    private readonly ListView _list;
    private readonly AutoSuggestBox _strategy;
    private readonly AutoSuggestBox _strategySb;
    private readonly MenuFlyoutItem _add = UiKit.Item(ResUI.menuRoutingAdvancedAdd);
    private readonly MenuFlyoutItem _remove = UiKit.Item(ResUI.menuRoutingAdvancedRemove);
    private readonly MenuFlyoutItem _default = UiKit.Item(ResUI.menuRoutingAdvancedSetDefault);
    private readonly MenuFlyoutItem _import = UiKit.Item(ResUI.menuRoutingAdvancedImportRules);

    public RoutingSettingWindow()
    {
        Title = ResUI.menuRoutingSetting;
        AppHost.Place(this, "RoutingSettingWindow", 860, 640);
        var form = UiKit.Stack();
        _strategy = UiKit.Suggest(form, ResUI.TbSettingsDomainStrategy4Freedom, Global.DomainStrategy);
        _strategySb = UiKit.Suggest(form, ResUI.TbSettingsDomainStrategy4Out, Global.DomainStrategies4Sbox);
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        foreach (var item in new[] { _add, _remove, _default, _import })
        {
            var button = new Button { Content = item.Text };
            button.Click += (_, _) => item.Command?.Execute(null);
            buttons.Children.Add(button);
        }

        form.Children.Add(buttons);
        _list = UiKit.List(form, nameof(RoutingItem.Remarks), 420);
        _list.SelectionMode = ListViewSelectionMode.Extended;
        _list.DoubleTapped += (_, _) => ViewModel?.RoutingAdvancedEditAsync(false);
        _list.SelectionChanged += (_, _) =>
        {
            if (ViewModel is not null)
            {
                ViewModel.SelectedSources = _list.SelectedItems.Cast<RoutingItemModel>().ToList();
            }
        };
        Content = form;
        this.WhenActivated(d =>
        {
            this.OneWayBindUnsafe(ViewModel, vm => vm.RoutingItems, v => v._list.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource, v => v._list.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DomainStrategy, v => v._strategy.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DomainStrategy4Singbox, v => v._strategySb.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RoutingAdvancedAddCmd, v => v._add, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RoutingAdvancedRemoveCmd, v => v._remove, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RoutingAdvancedSetDefaultCmd, v => v._default, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RoutingAdvancedImportRulesCmd, v => v._import, "Click").DisposeWith(d);
            ViewModel.ShowYesNoInteraction.RegisterHandler(async interaction => interaction.SetOutput(await UiKit.YesNo(interaction.Input))).DisposeWith(d);
        });
    }
}

public sealed class RoutingRuleSettingWindow : ReactiveWindow<RoutingRuleSettingViewModel>
{
    private readonly TextBox _remarks;
    private readonly AutoSuggestBox _strategy;
    private readonly AutoSuggestBox _strategySb;
    private readonly TextBox _url;
    private readonly TextBox _icon;
    private readonly TextBox _ruleset;
    private readonly TextBox _sort;
    private readonly ListView _list;
    private readonly MenuFlyoutItem _add = UiKit.Item(ResUI.menuRuleAdd);
    private readonly MenuFlyoutItem _file = UiKit.Item(ResUI.menuImportRulesFromFile);
    private readonly MenuFlyoutItem _clip = UiKit.Item("Clipboard");
    private readonly MenuFlyoutItem _urlImport = UiKit.Item("URL");
    private readonly MenuFlyoutItem _remove = UiKit.Item(ResUI.menuRemoveServer);
    private readonly MenuFlyoutItem _export = UiKit.Item(ResUI.menuExport2ShareUrl);
    private readonly MenuFlyoutItem _top = UiKit.Item(ResUI.menuMoveTop);
    private readonly MenuFlyoutItem _up = UiKit.Item(ResUI.menuMoveUp);
    private readonly MenuFlyoutItem _down = UiKit.Item(ResUI.menuMoveDown);
    private readonly MenuFlyoutItem _bottom = UiKit.Item(ResUI.menuMoveBottom);
    private readonly Button _save;

    public RoutingRuleSettingWindow()
    {
        Title = ResUI.menuRoutingSetting;
        AppHost.Place(this, "RoutingRuleSettingWindow", 900, 720);
        var form = UiKit.Stack();
        _remarks = UiKit.Text(form, ResUI.TbRemarks);
        _strategy = UiKit.Suggest(form, ResUI.TbSettingsDomainStrategy4Freedom, Global.DomainStrategy);
        _strategySb = UiKit.Suggest(form, ResUI.TbSettingsDomainStrategy4Out, Global.DomainStrategies4Sbox);
        _url = UiKit.Text(form, "URL");
        _icon = UiKit.Text(form, "Icon");
        _ruleset = UiKit.Text(form, "Ruleset");
        _sort = UiKit.Text(form, "Sort");
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        foreach (var item in new[] { _add, _file, _clip, _urlImport, _remove, _export, _top, _up, _down, _bottom })
        {
            var button = new Button { Content = item.Text };
            button.Click += (_, _) => item.Command?.Execute(null);
            buttons.Children.Add(button);
        }

        form.Children.Add(buttons);
        _list = UiKit.List(form, nameof(RulesItem.Remarks), 280);
        _list.SelectionMode = ListViewSelectionMode.Extended;
        _list.DoubleTapped += (_, _) => ViewModel?.RuleEditAsync(false);
        _list.SelectionChanged += (_, _) =>
        {
            if (ViewModel is not null)
            {
                ViewModel.SelectedSources = _list.SelectedItems.Cast<RulesItemModel>().ToList();
            }
        };
        (_save, Content) = FullConfigTemplateWindow.SaveBar(form);
        this.WhenActivated(d =>
        {
            this.OneWayBindUnsafe(ViewModel, vm => vm.RulesItems, v => v._list.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource, v => v._list.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedRouting.Remarks, v => v._remarks.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedRouting.DomainStrategy, v => v._strategy.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedRouting.DomainStrategy4Singbox, v => v._strategySb.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedRouting.Url, v => v._url.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedRouting.CustomIcon, v => v._icon.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedRouting.CustomRulesetPath4Singbox, v => v._ruleset.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedRouting.Sort, v => v._sort.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RuleAddCmd, v => v._add, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ImportRulesFromFileCmd, v => v._file, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ImportRulesFromClipboardCmd, v => v._clip, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ImportRulesFromUrlCmd, v => v._urlImport, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RuleRemoveCmd, v => v._remove, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RuleExportSelectedCmd, v => v._export, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveTopCmd, v => v._top, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveUpCmd, v => v._up, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveDownCmd, v => v._down, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveBottomCmd, v => v._bottom, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);
            ViewModel.ShowYesNoInteraction.RegisterHandler(async interaction => interaction.SetOutput(await UiKit.YesNo(interaction.Input))).DisposeWith(d);
            ViewModel.SetClipboardDataInteraction.RegisterHandler(interaction =>
            {
                Native.SetClipboardText(interaction.Input);
                interaction.SetOutput(RxVoid.Default);
            }).DisposeWith(d);
            ViewModel.ReadTextFromClipboardInteraction.RegisterHandler(interaction => interaction.SetOutput(Native.GetClipboardText())).DisposeWith(d);
            ViewModel.BrowseRulesFileInteraction.RegisterHandler(interaction =>
            {
                interaction.SetOutput(Native.OpenFile(out var file, "Rules|*.json|All|*.*") == true ? file : null);
            }).DisposeWith(d);
        });
    }
}

public sealed class RoutingRuleDetailsWindow : ReactiveWindow<RoutingRuleDetailsViewModel>
{
    private readonly TextBox _remarks;
    private readonly AutoSuggestBox _outbound;
    private readonly TextBox _port;
    private readonly AutoSuggestBox _network;
    private readonly ToggleSwitch _enabled;
    private readonly TextBox _domain;
    private readonly TextBox _ip;
    private readonly TextBox _process;
    private readonly ToggleSwitch _autoSort;
    private readonly AutoSuggestBox _type;
    private readonly ListView _protocol;
    private readonly ListView _inbound;
    private readonly Button _profile;
    private readonly Button _save;

    public RoutingRuleDetailsWindow()
    {
        Title = ResUI.menuRuleAdd;
        AppHost.Place(this, "RoutingRuleDetailsWindow", 760, 720);
        var form = UiKit.Stack();
        _remarks = UiKit.Text(form, ResUI.TbRemarks);
        _outbound = UiKit.Suggest(form, "Outbound", Global.OutboundTags);
        _port = UiKit.Text(form, ResUI.TbPort);
        _network = UiKit.Suggest(form, ResUI.TbNetwork, Global.RuleNetworks);
        _enabled = UiKit.Toggle(form, "Enabled");
        _domain = UiKit.Text(form, "Domain", multi: true);
        _ip = UiKit.Text(form, "IP", multi: true);
        _process = UiKit.Text(form, "Process", multi: true);
        _autoSort = UiKit.Toggle(form, "Auto sort");
        _type = UiKit.Suggest(form, "Type", Utils.GetEnumNames<ERuleType>().AppendEmpty());
        UiKit.Header(form, "Protocol");
        _protocol = new ListView { SelectionMode = ListViewSelectionMode.Multiple, MaxHeight = 120, ItemsSource = Global.RuleProtocols };
        form.Children.Add(_protocol);
        UiKit.Header(form, "Inbound");
        _inbound = new ListView { SelectionMode = ListViewSelectionMode.Multiple, MaxHeight = 120, ItemsSource = Global.InboundTags };
        form.Children.Add(_inbound);
        _profile = UiKit.Button(form, ResUI.menuServers);
        (_save, Content) = FullConfigTemplateWindow.SaveBar(form);
        _protocol.SelectionChanged += (_, _) =>
        {
            if (ViewModel is not null)
            {
                ViewModel.ProtocolItems = _protocol.SelectedItems.Cast<string>().ToList();
            }
        };
        _inbound.SelectionChanged += (_, _) =>
        {
            if (ViewModel is not null)
            {
                ViewModel.InboundTagItems = _inbound.SelectedItems.Cast<string>().ToList();
            }
        };
        this.WhenActivated(d =>
        {
            this.BindUnsafe(ViewModel, vm => vm.Remarks, v => v._remarks.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.OutboundTag, v => v._outbound.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Port, v => v._port.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Network, v => v._network.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Enabled, v => v._enabled.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Domain, v => v._domain.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.IP, v => v._ip.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Process, v => v._process.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AutoSort, v => v._autoSort.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.RuleType, v => v._type.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SelectProfileCmd, v => v._profile, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.SelectedSource).Subscribe(rule =>
            {
                if (rule is null)
                {
                    return;
                }

                _protocol.SelectedItems.Clear();
                rule.Protocol?.ForEach(it => _protocol.SelectedItems.Add(it));
                _inbound.SelectedItems.Clear();
                rule.InboundTag?.ForEach(it => _inbound.SelectedItems.Add(it));
            }).DisposeWith(d);
        });
    }
}

public sealed class ProfilesSelectWindow : ReactiveWindow<ProfilesSelectViewModel>
{
    private readonly ListView _groups;
    private readonly ListView _list;
    private readonly TextBox _filter;
    private readonly Button _save;

    public ProfilesSelectWindow()
    {
        Title = ResUI.menuServers;
        AppHost.Place(this, "ProfilesSelectWindow", 860, 640);
        var form = UiKit.Stack();
        _filter = UiKit.Text(form, ResUI.MsgServerTitle);
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(220) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _groups = UiKit.List(null, nameof(SubItem.Remarks), 420);
        _list = UiKit.List(null, nameof(ProfileItem.Remarks), 420);
        Grid.SetColumn(_list, 1);
        grid.Children.Add(_groups);
        grid.Children.Add(_list);
        form.Children.Add(grid);
        (_save, Content) = FullConfigTemplateWindow.SaveBar(form);
        _list.SelectionChanged += (_, _) =>
        {
            if (ViewModel is not null)
            {
                ViewModel.SelectedProfiles = _list.SelectedItems.Cast<ProfileItemModel>().ToList();
            }
        };
        this.WhenActivated(d =>
        {
            this.OneWayBindUnsafe(ViewModel, vm => vm.ProfileItems, v => v._list.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedProfile, v => v._list.SelectedItem).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.SubItems, v => v._groups.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSub, v => v._groups.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.ServerFilter, v => v._filter.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.MultiSelect).Subscribe(multi =>
            {
                _list.SelectionMode = multi ? ListViewSelectionMode.Extended : ListViewSelectionMode.Single;
            }).DisposeWith(d);
        });
    }
}

public sealed class AddGroupServerWindow : ReactiveWindow<AddGroupServerViewModel>
{
    private readonly TextBox _remarks;
    private readonly AutoSuggestBox _core;
    private readonly StackPanel _policy;
    private readonly AutoSuggestBox _policyType;
    private readonly ComboBox _sub;
    private readonly AutoSuggestBox _filter;
    private readonly ListView _children;
    private readonly ListView _preview;
    private readonly MenuFlyoutItem _add = UiKit.Item(ResUI.menuSubAdd);
    private readonly MenuFlyoutItem _remove = UiKit.Item(ResUI.menuRemoveChildServer);
    private readonly MenuFlyoutItem _top = UiKit.Item(ResUI.menuMoveTop);
    private readonly MenuFlyoutItem _up = UiKit.Item(ResUI.menuMoveUp);
    private readonly MenuFlyoutItem _down = UiKit.Item(ResUI.menuMoveDown);
    private readonly MenuFlyoutItem _bottom = UiKit.Item(ResUI.menuMoveBottom);
    private readonly Button _save;

    public AddGroupServerWindow()
    {
        Title = ResUI.TbConfigTypePolicyGroup;
        AppHost.Place(this, "AddGroupServerWindow", 860, 680);
        var form = UiKit.Stack();
        _remarks = UiKit.Text(form, ResUI.TbRemarks);
        _core = UiKit.Suggest(form, ResUI.TbSettingsCoreType, Global.CoreTypes);
        _policy = new StackPanel { Spacing = 8 };
        _policyType = UiKit.Suggest(_policy, "Policy", [ResUI.TbLeastPing, ResUI.TbFallback, ResUI.TbRandom, ResUI.TbRoundRobin, ResUI.TbLeastLoad]);
        _sub = UiKit.Combo(_policy, ResUI.menuSubscription, display: nameof(SubItem.Remarks));
        _filter = UiKit.Suggest(_policy, "Filter", Global.PolicyGroupDefaultFilterList);
        form.Children.Add(_policy);
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        foreach (var item in new[] { _add, _remove, _top, _up, _down, _bottom })
        {
            var button = new Button { Content = item.Text };
            button.Click += (_, _) => item.Command?.Execute(null);
            buttons.Children.Add(button);
        }

        form.Children.Add(buttons);
        _children = UiKit.List(form, nameof(ProfileItem.Remarks), 180);
        _preview = UiKit.List(form, nameof(ProfileItem.Remarks), 140);
        (_save, Content) = FullConfigTemplateWindow.SaveBar(form);
        this.WhenActivated(d =>
        {
            this.WhenAnyValue(v => v.ViewModel!.SelectedSource).Subscribe(item =>
            {
                if (item is null)
                {
                    return;
                }

                Title = item.ConfigType == EConfigType.ProxyChain ? ResUI.TbConfigTypeProxyChain : ResUI.TbConfigTypePolicyGroup;
                DialogPage.Show(_policy, item.ConfigType != EConfigType.ProxyChain);
            }).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Remarks, v => v._remarks.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType, v => v._core.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.PolicyGroupType, v => v._policyType.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.SubItems, v => v._sub.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSubItem, v => v._sub.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Filter, v => v._filter.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.ChildItemsObs, v => v._children.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedChild, v => v._children.SelectedItem).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.AllProfilePreviewItemsObs, v => v._preview.ItemsSource).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddCmd, v => v._add, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RemoveCmd, v => v._remove, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveTopCmd, v => v._top, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveUpCmd, v => v._up, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveDownCmd, v => v._down, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveBottomCmd, v => v._bottom, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);
        });
    }
}

public sealed class AddServer2Window : ReactiveWindow<AddServer2ViewModel>
{
    private readonly TextBox _remarks;
    private readonly TextBox _address;
    private readonly AutoSuggestBox _core;
    private readonly ToggleSwitch _log;
    private readonly TextBox _socks;
    private readonly ToggleSwitch _endpoint;
    private readonly Button _browse;
    private readonly Button _edit;
    private readonly Button _save;

    public AddServer2Window()
    {
        Title = ResUI.menuAddCustomServer;
        AppHost.Place(this, "AddServer2Window", 760, 560);
        var form = UiKit.Stack();
        _remarks = UiKit.Text(form, ResUI.TbRemarks);
        _address = UiKit.Text(form, ResUI.TbAddress, multi: true);
        _core = UiKit.Suggest(form, ResUI.TbSettingsCoreType, Utils.GetEnumNames<ECoreType>().Where(t => t != nameof(ECoreType.v2rayN)).AppendEmpty());
        _log = UiKit.Toggle(form, ResUI.TbSettingsLogEnabled);
        _socks = UiKit.Text(form, ResUI.TbPreSocksPort);
        _endpoint = UiKit.Toggle(form, "sing-box endpoint");
        _browse = UiKit.Button(form, "...");
        _edit = UiKit.Button(form, ResUI.menuEditServer);
        (_save, Content) = FullConfigTemplateWindow.SaveBar(form);
        this.WhenActivated(d =>
        {
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Remarks, v => v._remarks.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Address, v => v._address.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType, v => v._core.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.DisplayLog, v => v._log.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.PreSocksPort, v => v._socks.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.IsSingboxEndpoint, v => v._endpoint.IsOn).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.BrowseServerCmd, v => v._browse, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.EditServerCmd, v => v._edit, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveServerCmd, v => v._save, "Click").DisposeWith(d);
            ViewModel.BrowseConfigFileInteraction.RegisterHandler(interaction =>
            {
                interaction.SetOutput(Native.OpenFile(out var file, "Config|*.json|YAML|*.yaml;*.yml|All|*.*") == true ? file : null);
            }).DisposeWith(d);
        });
    }
}

public sealed class CheckUpdateWindow : ReactiveWindow<CheckUpdateViewModel>
{
    private readonly ToggleSwitch _proxy;
    private readonly Button _check;
    private readonly Button _update;
    private readonly ListView _list;

    public CheckUpdateWindow()
    {
        Title = ResUI.menuCheckUpdate;
        var form = UiKit.Stack();
        _proxy = UiKit.Toggle(form, ResUI.TbSettingsEnableUpdateViaProxy);
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        _check = new Button { Content = ResUI.menuCheckOnly, MinWidth = 120 };
        _update = new Button { Content = ResUI.menuCheckAndUpdate, MinWidth = 120 };
        row.Children.Add(_check);
        row.Children.Add(_update);
        form.Children.Add(row);
        _list = UiKit.List(form, nameof(CheckUpdateModel.Remarks), 360);
        Content = form;
        this.WhenActivated(d =>
        {
            this.OneWayBindUnsafe(ViewModel, vm => vm.CheckUpdateModels, v => v._list.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableUpdateViaProxy, v => v._proxy.IsOn).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.CheckOnlyCmd, v => v._check, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.CheckUpdateCmd, v => v._update, "Click").DisposeWith(d);
        });
    }
}

public sealed class BackupAndRestoreWindow : ReactiveWindow<BackupAndRestoreViewModel>
{
    private readonly TextBox _msg;
    private readonly TextBox _url;
    private readonly TextBox _user;
    private readonly TextBox _pass;
    private readonly TextBox _dir;
    private readonly Button _check;
    private readonly Button _remoteBackup;
    private readonly Button _remoteRestore;
    private readonly Button _localBackup;
    private readonly Button _localRestore;

    public BackupAndRestoreWindow()
    {
        Title = ResUI.menuBackupAndRestore;
        var form = UiKit.Stack();
        _msg = UiKit.Text(form, ResUI.MsgInformationTitle, multi: true);
        _msg.IsReadOnly = true;
        _url = UiKit.Text(form, ResUI.LvWebDavUrl);
        _user = UiKit.Text(form, ResUI.LvWebDavUserName);
        _pass = UiKit.Text(form, ResUI.LvWebDavPassword);
        _dir = UiKit.Text(form, ResUI.LvWebDavDirName);
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        _check = new Button { Content = ResUI.LvWebDavCheck };
        _remoteBackup = new Button { Content = ResUI.menuRemoteBackup };
        _remoteRestore = new Button { Content = ResUI.menuRemoteRestore };
        _localBackup = new Button { Content = ResUI.menuLocalBackup };
        _localRestore = new Button { Content = ResUI.menuLocalRestore };
        foreach (var button in new[] { _check, _remoteBackup, _remoteRestore, _localBackup, _localRestore })
        {
            row.Children.Add(button);
        }

        form.Children.Add(row);
        Content = UiKit.Scroll(form);
        _localBackup.Click += (_, _) =>
        {
            if (Native.SaveFile(out var file, "Zip|*.zip") == true)
            {
                ViewModel?.LocalBackup(file);
            }
        };
        _localRestore.Click += (_, _) =>
        {
            if (Native.OpenFile(out var file, "Zip|*.zip|All|*.*") == true)
            {
                ViewModel?.LocalRestore(file);
            }
        };
        this.WhenActivated(d =>
        {
            this.BindUnsafe(ViewModel, vm => vm.OperationMsg, v => v._msg.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Url, v => v._url.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.UserName, v => v._user.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._pass.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.DirName, v => v._dir.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.WebDavCheckCmd, v => v._check, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RemoteBackupCmd, v => v._remoteBackup, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RemoteRestoreCmd, v => v._remoteRestore, "Click").DisposeWith(d);
        });
    }
}
