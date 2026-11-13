using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Windows.System;
using Windows.UI.Core;

namespace v2rayN.WinUI.Views;

public sealed class ProfilesPane : ReactiveUserControl<ProfilesViewModel>
{
    private readonly StackPanel _groups = new() { Orientation = Orientation.Horizontal, Spacing = 6 };
    private readonly ListView _list;
    private readonly TextBox _filter = new() { Width = 220, PlaceholderText = ResUI.MsgServerTitle };
    public Button EditSub { get; } = new() { Content = ResUI.menuSubEdit };
    public Button AddSub { get; } = new() { Content = ResUI.menuSubAdd };
    public Button FastPing { get; } = new() { Content = ResUI.menuFastRealPing };
    public MenuFlyoutItem EditServer { get; } = UiKit.Item(ResUI.menuEditServer);
    public MenuFlyoutItem RemoveServer { get; } = UiKit.Item(ResUI.menuRemoveServer);
    public MenuFlyoutItem RemoveDuplicate { get; } = UiKit.Item(ResUI.menuRemoveDuplicateServer);
    public MenuFlyoutItem CopyServer { get; } = UiKit.Item(ResUI.menuCopyServer);
    public MenuFlyoutItem SetDefault { get; } = UiKit.Item(ResUI.menuSetDefaultServer);
    public MenuFlyoutItem ShareServer { get; } = UiKit.Item(ResUI.menuShareServer);
    public MenuFlyoutItem GenGroupAll { get; } = UiKit.Item(ResUI.menuAllServers);
    public MenuFlyoutItem GenGroupRegion { get; } = UiKit.Item(ResUI.menuGenRegionGroup);
    public MenuFlyoutItem MoveTop { get; } = UiKit.Item(ResUI.menuMoveTop);
    public MenuFlyoutItem MoveUp { get; } = UiKit.Item(ResUI.menuMoveUp);
    public MenuFlyoutItem MoveDown { get; } = UiKit.Item(ResUI.menuMoveDown);
    public MenuFlyoutItem MoveBottom { get; } = UiKit.Item(ResUI.menuMoveBottom);
    public MenuFlyoutItem MixedTest { get; } = UiKit.Item(ResUI.menuMixedTestServer);
    public MenuFlyoutItem Tcping { get; } = UiKit.Item(ResUI.menuTcpingServer);
    public MenuFlyoutItem RealPing { get; } = UiKit.Item(ResUI.menuRealPingServer);
    public MenuFlyoutItem UdpTest { get; } = UiKit.Item(ResUI.menuUdpTestServer);
    public MenuFlyoutItem SpeedTest { get; } = UiKit.Item(ResUI.menuSpeedServer);
    public MenuFlyoutItem SortResult { get; } = UiKit.Item(ResUI.menuSortServerResult);
    public MenuFlyoutItem RemoveInvalid { get; } = UiKit.Item(ResUI.menuRemoveInvalidServerResult);
    public MenuFlyoutItem ExportConfig { get; } = UiKit.Item(ResUI.menuExport2ClientConfig);
    public MenuFlyoutItem ExportConfigClip { get; } = UiKit.Item(ResUI.menuExport2ClientConfigClipboard);
    public MenuFlyoutItem ExportUrl { get; } = UiKit.Item(ResUI.menuExport2ShareUrl);
    public MenuFlyoutItem ExportUrl64 { get; } = UiKit.Item(ResUI.menuExport2ShareUrlBase64);
    public MenuFlyoutItem ExportInner { get; } = UiKit.Item(ResUI.menuExport2InnerUri);
    public MenuFlyoutItem SelectAll { get; } = UiKit.Item(ResUI.menuSelectAll);
    public ComboBox MoveToGroup { get; } = new() { MinWidth = 160, DisplayMemberPath = nameof(SubItem.Remarks) };

    public ProfilesPane()
    {
        _list = new ListView
        {
            SelectionMode = ListViewSelectionMode.Extended,
            ItemTemplate = (DataTemplate)XamlReader.Load(RowTemplate),
            ItemContainerStyle = StretchItem()
        };
        _list.ContextFlyout = BuildMenu();
        _list.DoubleTapped += (_, e) =>
        {
            if (e.OriginalSource is not FrameworkElement)
            {
                return;
            }

            if (AppManager.Instance.Config.UiItem.DoubleClick2Activate)
            {
                ViewModel?.SetDefaultServer();
            }
            else
            {
                _ = ViewModel?.EditServerAsync();
            }
        };
        _list.SelectionChanged += (_, _) =>
        {
            if (ViewModel is not null)
            {
                ViewModel.SelectedProfiles = _list.SelectedItems.OfType<ProfileItemModel>().ToList();
            }
        };
        _list.KeyDown += OnKeyDown;
        _filter.KeyDown += (_, e) =>
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel?.RefreshServers();
            }
        };

        var bar = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        bar.Children.Add(new ScrollViewer { Content = _groups, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, MaxHeight = 40 });
        bar.Children.Add(EditSub);
        bar.Children.Add(AddSub);
        bar.Children.Add(_filter);
        bar.Children.Add(FastPing);
        bar.Children.Add(MixedTestButton());
        bar.Children.Add(MoveToGroup);
        var barScroll = new ScrollViewer
        {
            Content = bar,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
        };

        var root = new Grid { RowSpacing = 8 };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(barScroll, 0);
        var headers = HeaderRow();
        Grid.SetRow(headers, 1);
        Grid.SetRow(_list, 2);
        root.Children.Add(barScroll);
        root.Children.Add(headers);
        root.Children.Add(_list);
        Content = root;

        this.WhenActivated(d =>
        {
            this.OneWayBindUnsafe(ViewModel, vm => vm.ProfileItems, v => v._list.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedProfile, v => v._list.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.ServerFilter, v => v._filter.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.SubItems, v => v.MoveToGroup.ItemsSource).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedMoveToGroup, v => v.MoveToGroup.SelectedItem).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.AddSubCmd, v => v.AddSub, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.EditSubCmd, v => v.EditSub, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.EditServerCmd, v => v.EditServer, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RemoveServerCmd, v => v.RemoveServer, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RemoveDuplicateServerCmd, v => v.RemoveDuplicate, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.CopyServerCmd, v => v.CopyServer, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SetDefaultServerCmd, v => v.SetDefault, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ShareServerCmd, v => v.ShareServer, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.GenGroupAllServerCmd, v => v.GenGroupAll, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.GenGroupRegionServerCmd, v => v.GenGroupRegion, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveTopCmd, v => v.MoveTop, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveUpCmd, v => v.MoveUp, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveDownCmd, v => v.MoveDown, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MoveBottomCmd, v => v.MoveBottom, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.MixedTestServerCmd, v => v.MixedTest, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.TcpingServerCmd, v => v.Tcping, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RealPingServerCmd, v => v.RealPing, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.UdpTestServerCmd, v => v.UdpTest, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SpeedServerCmd, v => v.SpeedTest, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SortServerResultCmd, v => v.SortResult, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.RemoveInvalidServerResultCmd, v => v.RemoveInvalid, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.FastRealPingCmd, v => v.FastPing, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.Export2ClientConfigCmd, v => v.ExportConfig, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.Export2ClientConfigClipboardCmd, v => v.ExportConfigClip, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.Export2ShareUrlCmd, v => v.ExportUrl, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.Export2ShareUrlBase64Cmd, v => v.ExportUrl64, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.Export2InnerUriCmd, v => v.ExportInner, "Click").DisposeWith(d);

            ViewModel.ShowYesNoInteraction.RegisterHandler(async ctx => ctx.SetOutput(await UiKit.YesNo(ctx.Input))).DisposeWith(d);
            ViewModel.SetClipboardDataInteraction.RegisterHandler(ctx =>
            {
                Native.SetClipboardText(ctx.Input);
                ctx.SetOutput(RxVoid.Default);
            }).DisposeWith(d);
            ViewModel.SaveFileDialogInteraction.RegisterHandler(async ctx =>
            {
                if (Native.SaveFile(out var fileName, "Config|*.json") != true || ViewModel is null)
                {
                    ctx.SetOutput(false);
                    return;
                }

                await ViewModel.Export2ClientConfigResult(fileName, ctx.Input);
                ctx.SetOutput(true);
            }).DisposeWith(d);
            ViewModel.ProfilesFocusInteraction.RegisterHandler(ctx =>
            {
                _list.Focus(FocusState.Programmatic);
                ctx.SetOutput(RxVoid.Default);
            }).DisposeWith(d);
            ViewModel.ShareServerInteraction.RegisterHandler(async ctx =>
            {
                if (ctx.Input.IsNotEmpty())
                {
                    await UiKit.ShowQrAsync(ctx.Input);
                }

                ctx.SetOutput(RxVoid.Default);
            }).DisposeWith(d);
            ViewModel.DispatcherRefreshServersBizInteraction.RegisterHandler(ctx =>
            {
                if (_list.SelectedItem is not null)
                {
                    _list.ScrollIntoView(_list.SelectedItem);
                }

                ctx.SetOutput(RxVoid.Default);
            }).DisposeWith(d);
            ViewModel.AdjustMainLvColWidthInteraction.RegisterHandler(ctx => ctx.SetOutput(RxVoid.Default)).DisposeWith(d);

            NotifyCollectionChangedEventHandler changed = (_, _) => RenderGroups();
            ViewModel.SubItems.CollectionChanged += changed;
            UiKit.Defer(() => ViewModel.SubItems.CollectionChanged -= changed).DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.SelectedSub).Subscribe(_ => RenderGroups()).DisposeWith(d);
            RenderGroups();
        });
    }

    private Button MixedTestButton()
    {
        var button = new Button { Content = ResUI.menuMixedTestServer };
        button.Click += (_, _) => MixedTest.Command?.Execute(null);
        return button;
    }

    private MenuFlyout BuildMenu()
    {
        var menu = new MenuFlyout();
        menu.Items.Add(SetDefault);
        menu.Items.Add(EditServer);
        menu.Items.Add(RemoveServer);
        menu.Items.Add(RemoveDuplicate);
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(CopyServer);
        menu.Items.Add(ShareServer);
        menu.Items.Add(SelectAll);
        SelectAll.Click += (_, _) => _list.SelectAll();
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(MoveTop);
        menu.Items.Add(MoveUp);
        menu.Items.Add(MoveDown);
        menu.Items.Add(MoveBottom);
        MoveToGroup.SelectionChanged += (_, _) =>
        {
            if (MoveToGroup.SelectedItem is SubItem sub && ViewModel is not null)
            {
                ViewModel.MoveToGroupCmd.Execute(sub).Subscribe();
            }
        };
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(Tcping);
        menu.Items.Add(RealPing);
        menu.Items.Add(UdpTest);
        menu.Items.Add(SpeedTest);
        menu.Items.Add(MixedTest);
        menu.Items.Add(SortResult);
        menu.Items.Add(RemoveInvalid);
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(ExportConfig);
        menu.Items.Add(ExportConfigClip);
        menu.Items.Add(ExportUrl);
        menu.Items.Add(ExportUrl64);
        menu.Items.Add(ExportInner);
        menu.Items.Add(GenGroupAll);
        menu.Items.Add(GenGroupRegion);
        return menu;
    }

    private Grid HeaderRow()
    {
        var grid = new Grid { ColumnSpacing = 4, HorizontalAlignment = HorizontalAlignment.Stretch };
        for (var i = 0; i < Columns.Length; i++)
        {
            var column = Columns[i];
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = column.Width, MinWidth = column.Min });
            var name = column.Name;
            var button = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(4, 2, 4, 2),
                Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
                Content = new TextBlock { Text = column.Title, TextTrimming = TextTrimming.CharacterEllipsis }
            };
            button.Click += (_, _) => ViewModel?.SortServer(name);
            Grid.SetColumn(button, i);
            grid.Children.Add(button);
        }

        return grid;
    }

    private static Style StretchItem()
    {
        var style = new Style(typeof(ListViewItem));
        if (Application.Current.Resources["DefaultListViewItemStyle"] is Style based)
        {
            style.BasedOn = based;
        }

        style.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
        return style;
    }

    private void RenderGroups()
    {
        _groups.Children.Clear();
        if (ViewModel is null)
        {
            return;
        }

        foreach (var sub in ViewModel.SubItems)
        {
            var toggle = new ToggleButton
            {
                Content = sub.Remarks,
                IsChecked = ReferenceEquals(sub, ViewModel.SelectedSub)
            };
            toggle.Click += (_, _) => ViewModel.SelectedSub = sub;
            _groups.Children.Add(toggle);
        }
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var ctrl = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
        if (ctrl)
        {
            switch (e.Key)
            {
                case VirtualKey.A:
                    _list.SelectAll();
                    e.Handled = true;
                    break;
                case VirtualKey.C:
                    _ = ViewModel?.Export2ShareUrlAsync(false);
                    e.Handled = true;
                    break;
                case VirtualKey.D:
                    _ = ViewModel?.EditServerAsync();
                    e.Handled = true;
                    break;
                case VirtualKey.F:
                    _ = ViewModel?.ShareServerAsync();
                    e.Handled = true;
                    break;
                case VirtualKey.O:
                    ViewModel?.ServerSpeedtest(ESpeedActionType.Tcping);
                    e.Handled = true;
                    break;
                case VirtualKey.R:
                    ViewModel?.ServerSpeedtest(ESpeedActionType.Realping);
                    e.Handled = true;
                    break;
                case VirtualKey.T:
                    ViewModel?.ServerSpeedtest(ESpeedActionType.Speedtest);
                    e.Handled = true;
                    break;
                case VirtualKey.E:
                    ViewModel?.ServerSpeedtest(ESpeedActionType.Mixedtest);
                    e.Handled = true;
                    break;
            }

            return;
        }

        switch (e.Key)
        {
            case VirtualKey.Enter:
                ViewModel?.SetDefaultServer();
                e.Handled = true;
                break;
            case VirtualKey.Delete:
            case VirtualKey.Back:
                _ = ViewModel?.RemoveServerAsync();
                e.Handled = true;
                break;
            case VirtualKey.T:
                ViewModel?.MoveServer(EMove.Top);
                break;
            case VirtualKey.U:
                ViewModel?.MoveServer(EMove.Up);
                break;
            case VirtualKey.D:
                ViewModel?.MoveServer(EMove.Down);
                break;
            case VirtualKey.B:
                ViewModel?.MoveServer(EMove.Bottom);
                break;
            case VirtualKey.Escape:
                ViewModel?.ServerSpeedtestStop();
                break;
        }
    }

    // Remarks and address take the leftover width so TLS and IP stay on screen at the default window size.
    private static readonly (string Name, string Title, GridLength Width, double Min)[] Columns =
    [
        (nameof(EServerColName.ConfigType), ResUI.LvServiceType, new(76), 76),
        (nameof(EServerColName.Remarks), ResUI.LvRemarks, new(1, GridUnitType.Star), 72),
        (nameof(EServerColName.Address), ResUI.LvAddress, new(1, GridUnitType.Star), 72),
        (nameof(EServerColName.Port), ResUI.LvPort, new(52), 52),
        (nameof(EServerColName.DelayVal), ResUI.LvTestDelay, new(72), 72),
        (nameof(EServerColName.SpeedVal), ResUI.LvTestSpeed, new(112), 112),
        (nameof(EServerColName.SubRemarks), ResUI.LvSubscription, new(88), 88),
        (nameof(EServerColName.StreamSecurity), ResUI.LvTLS, new(56), 56),
        (nameof(EServerColName.IpInfo), ResUI.LvTestIpInfo, new(76), 76),
    ];

    private const string RowTemplate = """
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Grid ColumnSpacing="4" Padding="2" HorizontalAlignment="Stretch">
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="76" MinWidth="76" />
      <ColumnDefinition Width="*" MinWidth="72" />
      <ColumnDefinition Width="*" MinWidth="72" />
      <ColumnDefinition Width="52" MinWidth="52" />
      <ColumnDefinition Width="72" MinWidth="72" />
      <ColumnDefinition Width="112" MinWidth="112" />
      <ColumnDefinition Width="88" MinWidth="88" />
      <ColumnDefinition Width="56" MinWidth="56" />
      <ColumnDefinition Width="76" MinWidth="76" />
    </Grid.ColumnDefinitions>
    <TextBlock Text="{Binding ConfigType}" />
    <TextBlock Grid.Column="1" Text="{Binding Remarks}" TextTrimming="CharacterEllipsis" />
    <TextBlock Grid.Column="2" Text="{Binding Address}" TextTrimming="CharacterEllipsis" />
    <TextBlock Grid.Column="3" Text="{Binding Port}" />
    <TextBlock Grid.Column="4" Text="{Binding DelayVal}" />
    <TextBlock Grid.Column="5" Text="{Binding SpeedVal}" />
    <TextBlock Grid.Column="6" Text="{Binding SubRemarks}" TextTrimming="CharacterEllipsis" />
    <TextBlock Grid.Column="7" Text="{Binding StreamSecurity}" />
    <TextBlock Grid.Column="8" Text="{Binding IpInfo}" TextTrimming="CharacterEllipsis" />
  </Grid>
</DataTemplate>
""";
}
