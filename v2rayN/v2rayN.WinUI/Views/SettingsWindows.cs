namespace v2rayN.WinUI.Views;

public sealed class OptionSettingWindow : ReactiveWindow<OptionSettingViewModel>
{
    private readonly TextBox _localPort;
    private readonly ToggleSwitch _secondPort;
    private readonly ToggleSwitch _udp;
    private readonly ToggleSwitch _sniff;
    private readonly ToggleSwitch _routeOnly;
    private readonly ToggleSwitch _lan;
    private readonly ToggleSwitch _newPort;
    private readonly TextBox _user;
    private readonly TextBox _pass;
    private readonly ToggleSwitch _log;
    private readonly AutoSuggestBox _logLevel;
    private readonly AutoSuggestBox _fingerprint;
    private readonly AutoSuggestBox _agent;
    private readonly TextBox _sendThrough;
    private readonly AutoSuggestBox _bind;
    private readonly TextBox _muxRay;
    private readonly TextBox _muxXudp;
    private readonly AutoSuggestBox _muxUdp443;
    private readonly AutoSuggestBox _muxSbox;
    private readonly ToggleSwitch _cache;
    private readonly TextBox _up;
    private readonly TextBox _down;
    private readonly ToggleSwitch _fragment;
    private readonly ToggleSwitch _finalFragment;
    private readonly AutoSuggestBox _fragPackets;
    private readonly TextBox _fragLen;
    private readonly TextBox _fragDelay;
    private readonly TextBox _fragSplit;
    private readonly ToggleSwitch _autoRun;
    private readonly ToggleSwitch _stats;
    private readonly ToggleSwitch _speed;
    private readonly ToggleSwitch _dedup;
    private readonly ToggleSwitch _autofit;
    private readonly ToggleSwitch _hideStartup;
    private readonly ToggleSwitch _drag;
    private readonly ToggleSwitch _dbl;
    private readonly TextBox _update;
    private readonly TextBox _trayLimit;
    private readonly AutoSuggestBox _font;
    private readonly AutoSuggestBox _timeout;
    private readonly AutoSuggestBox _speedUrl;
    private readonly AutoSuggestBox _pingUrl;
    private readonly AutoSuggestBox _udpTarget;
    private readonly AutoSuggestBox _mixed;
    private readonly ToggleSwitch _hwa;
    private readonly AutoSuggestBox _convert;
    private readonly ComboBox _orientation;
    private readonly AutoSuggestBox _geo;
    private readonly AutoSuggestBox _srs;
    private readonly AutoSuggestBox _rules;
    private readonly AutoSuggestBox _ipapi;
    private readonly AutoSuggestBox _cert;
    private readonly ToggleSwitch _localBypass;
    private readonly AutoSuggestBox _proxyProtocol;
    private readonly TextBox _exceptions;
    private readonly TextBox _pac;
    private readonly ToggleSwitch _autoRoute;
    private readonly ToggleSwitch _strict;
    private readonly AutoSuggestBox _stack;
    private readonly AutoSuggestBox _mtu;
    private readonly ToggleSwitch _ipv6;
    private readonly AutoSuggestBox _icmp;
    private readonly ToggleSwitch _legacy;
    private readonly AutoSuggestBox _dnsMode;
    private readonly TextBox _exclude;
    private readonly AutoSuggestBox _v4;
    private readonly AutoSuggestBox _v6;
    private readonly AutoSuggestBox _c1;
    private readonly AutoSuggestBox _c2;
    private readonly AutoSuggestBox _c3;
    private readonly AutoSuggestBox _c4;
    private readonly AutoSuggestBox _c5;
    private readonly AutoSuggestBox _c6;
    private readonly AutoSuggestBox _c7;
    private readonly AutoSuggestBox _c9;
    private readonly AutoSuggestBox _c14;
    private readonly ListView _dest;
    private readonly Button _save;

    public OptionSettingWindow()
    {
        Title = ResUI.menuOptionSetting;
        AppHost.Place(this, "OptionSettingWindow", 920, 760);
        var form = UiKit.Stack();
        _save = new Button { Content = ResUI.TbConfirm, MinWidth = 120, HorizontalAlignment = HorizontalAlignment.Left };

        UiKit.Header(form, ResUI.TbSettingsSocksPort);
        _localPort = UiKit.Text(form, ResUI.TbSettingsSocksPort);
        _secondPort = UiKit.Toggle(form, ResUI.TbSettingsSecondLocalPortEnabled);
        _udp = UiKit.Toggle(form, ResUI.TbSettingsUdpEnabled);
        _sniff = UiKit.Toggle(form, ResUI.TbSettingsSniffingEnabled);
        _routeOnly = UiKit.Toggle(form, ResUI.TbSettingsRouteOnly);
        _lan = UiKit.Toggle(form, ResUI.TbSettingsAllowLAN);
        _newPort = UiKit.Toggle(form, ResUI.TbSettingsNewPort4LAN);
        _user = UiKit.Text(form, ResUI.TbSettingsUser);
        _pass = UiKit.Text(form, ResUI.TbSettingsPass);
        _log = UiKit.Toggle(form, ResUI.TbSettingsLogEnabled);
        _logLevel = UiKit.Suggest(form, ResUI.TbSettingsLogLevel, Global.LogLevels);
        _fingerprint = UiKit.Suggest(form, ResUI.TbSettingsDefFingerprint, Global.Fingerprints);
        _agent = UiKit.Suggest(form, ResUI.TbSettingsDefUserAgent, Global.UserAgent);
        _sendThrough = UiKit.Text(form, ResUI.TbSettingsSendThrough);
        _bind = UiKit.Suggest(form, ResUI.TbSettingsBindInterface, NetworkUtils.GetBindInterfaceNames());
        _muxRay = UiKit.Text(form, ResUI.TbSettingsMux4RayConcurrency);
        _muxXudp = UiKit.Text(form, ResUI.TbSettingsMux4RayXudpConcurrency);
        _muxUdp443 = UiKit.Suggest(form, ResUI.TbSettingsMux4RayXudpProxyUDP443, ["reject", "skip"]);
        _muxSbox = UiKit.Suggest(form, ResUI.TbSettingsMux4SboxProtocol, Global.SingboxMuxs);
        _cache = UiKit.Toggle(form, ResUI.TbSettingsEnableCacheFile4Sbox);
        _up = UiKit.Text(form, ResUI.TbSettingsHysteriaBandwidth);
        _down = UiKit.Text(form, ResUI.TbSettingsHysteriaBandwidth);
        _fragment = UiKit.Toggle(form, ResUI.TbSettingsEnableFragment);
        _finalFragment = UiKit.Toggle(form, ResUI.TbEnableFinalFragment);
        _fragPackets = UiKit.Suggest(form, ResUI.TbSettingsFragmentPackets, Global.FragmentPacketsOptions);
        _fragLen = UiKit.Text(form, ResUI.TbSettingsFragmentLength);
        _fragDelay = UiKit.Text(form, ResUI.TbSettingsFragmentInterval);
        _fragSplit = UiKit.Text(form, ResUI.TbSettingsFragmentMaxSplit);
        UiKit.Header(form, ResUI.menuSetting);
        _autoRun = UiKit.Toggle(form, ResUI.TbSettingsStartBoot);
        _stats = UiKit.Toggle(form, ResUI.TbSettingsStatistics);
        _speed = UiKit.Toggle(form, ResUI.TbSettingsDisplayRealTimeSpeed);
        _dedup = UiKit.Toggle(form, ResUI.TbSettingsKeepOlderDedupl);
        _autofit = UiKit.Toggle(form, ResUI.TbSettingsEnableAutoAdjustMainLvColWidth);
        _hideStartup = UiKit.Toggle(form, ResUI.TbSettingsAutoHideStartup);
        _drag = UiKit.Toggle(form, ResUI.TbSettingsEnableDragDropSort);
        _dbl = UiKit.Toggle(form, ResUI.TbSettingsDoubleClick2Activate);
        _update = UiKit.Text(form, ResUI.TbSettingsAutoUpdateInterval);
        _trayLimit = UiKit.Text(form, ResUI.TbSettingsTrayMenuServersLimit);
        _font = UiKit.Suggest(form, ResUI.TbSettingsCurrentFontFamily, FontNames());
        _timeout = UiKit.Suggest(form, ResUI.TbSettingsSpeedTestTimeout, Enumerable.Range(2, 5).Select(i => (i * 5).ToString()));
        _speedUrl = UiKit.Suggest(form, ResUI.TbSettingsSpeedTestUrl, Global.SpeedTestUrls);
        _pingUrl = UiKit.Suggest(form, ResUI.TbSettingsSpeedPingTestUrl, Global.SpeedPingTestUrls);
        _udpTarget = UiKit.Suggest(form, ResUI.TbSettingsUdpTestUrl, Global.UdpTestTargets);
        _mixed = UiKit.Suggest(form, ResUI.TbSettingsMixedConcurrencyCount, Enumerable.Range(Global.SpeedTestConcurrencyCountMin, 20).Select(i => i.ToString()));
        _hwa = UiKit.Toggle(form, ResUI.TbSettingsEnableHWA);
        _convert = UiKit.Suggest(form, ResUI.TbSettingsSubConvert, Global.SubConvertUrls);
        _orientation = UiKit.Combo(form, ResUI.TbSettingsMainGirdOrientation, Utils.GetEnumNames<EGirdOrientation>());
        _geo = UiKit.Suggest(form, ResUI.TbSettingsGeoFilesSource, Global.GeoFilesSources);
        _srs = UiKit.Suggest(form, ResUI.TbSettingsSrsFilesSource, Global.SingboxRulesetSources);
        _rules = UiKit.Suggest(form, ResUI.TbSettingsRoutingRulesSource, Global.RoutingRulesSources);
        _ipapi = UiKit.Suggest(form, ResUI.TbSettingsIPAPIUrl, Global.IPAPIUrls);
        _cert = UiKit.Suggest(form, "Root certificate", Global.RootCertProviders);
        UiKit.Header(form, ResUI.TbSettingsSystemproxy);
        _localBypass = UiKit.Toggle(form, ResUI.TbSettingsNotProxyLocalAddress);
        _proxyProtocol = UiKit.Suggest(form, ResUI.TbSettingsAdvancedProtocol, Global.IEProxyProtocols);
        _exceptions = UiKit.Text(form, ResUI.TbSettingsException, multi: true);
        _pac = UiKit.Text(form, ResUI.TbSettingsCustomSystemProxyPacPath);
        UiKit.Header(form, ResUI.TbSettingsTunMode);
        _autoRoute = UiKit.Toggle(form, ResUI.TbSettingsTunAutoRoute);
        _strict = UiKit.Toggle(form, ResUI.TbSettingsTunStrictRoute);
        _stack = UiKit.Suggest(form, ResUI.TbSettingsTunStack, Global.TunStacks);
        _mtu = UiKit.Suggest(form, "MTU", Global.TunMtus.Select(i => i.ToString()));
        _ipv6 = UiKit.Toggle(form, ResUI.TbSettingsEnableIPv6Address);
        _icmp = UiKit.Suggest(form, "ICMP", Global.TunIcmpRoutingPolicies);
        _legacy = UiKit.Toggle(form, "Legacy protect");
        _dnsMode = UiKit.Suggest(form, ResUI.TbSettingsTunDnsMode, Global.TunDnsModes);
        _exclude = UiKit.Text(form, "Route exclude", multi: true);
        _v4 = UiKit.Suggest(form, "IPv4", Global.TunIPv4Address);
        _v6 = UiKit.Suggest(form, "IPv6", Global.TunIPv6Address);
        UiKit.Header(form, ResUI.TbSettingsCore);
        var cores = Global.CoreTypes;
        _c1 = UiKit.Suggest(form, "VMess", cores);
        _c2 = UiKit.Suggest(form, "Custom", cores);
        _c3 = UiKit.Suggest(form, "Shadowsocks", cores);
        _c4 = UiKit.Suggest(form, "Socks", cores);
        _c5 = UiKit.Suggest(form, "VLESS", cores);
        _c6 = UiKit.Suggest(form, "Trojan", cores);
        _c7 = UiKit.Suggest(form, "Hysteria2", cores);
        _c9 = UiKit.Suggest(form, "WireGuard", cores);
        _c14 = UiKit.Suggest(form, "MASQUE", cores);
        UiKit.Header(form, ResUI.TbSettingsDestOverride);
        _dest = new ListView { SelectionMode = ListViewSelectionMode.Multiple, MaxHeight = 140 };
        _dest.ItemsSource = Global.destOverrideProtocols;
        form.Children.Add(_dest);

        var bar = new StackPanel { Orientation = Orientation.Horizontal, Padding = new Thickness(12) };
        bar.Children.Add(_save);
        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        var scroll = UiKit.Scroll(form);
        Grid.SetRow(bar, 1);
        grid.Children.Add(scroll);
        grid.Children.Add(bar);
        Content = grid;

        this.WhenActivated(d =>
        {
            this.BindUnsafe(ViewModel, vm => vm.LocalPort, v => v._localPort.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SecondLocalPortEnabled, v => v._secondPort.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.UdpEnabled, v => v._udp.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SniffingEnabled, v => v._sniff.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.RouteOnly, v => v._routeOnly.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AllowLANConn, v => v._lan.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.NewPort4LAN, v => v._newPort.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.NewPort4LAN, v => v._user.IsEnabled).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.NewPort4LAN, v => v._pass.IsEnabled).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.User, v => v._user.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Pass, v => v._pass.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.LogEnabled, v => v._log.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Loglevel, v => v._logLevel.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DefFingerprint, v => v._fingerprint.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DefUserAgent, v => v._agent.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SendThrough, v => v._sendThrough.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.BindInterface, v => v._bind.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Mux4RayConcurrency, v => v._muxRay.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Mux4RayXudpConcurrency, v => v._muxXudp.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Mux4RayXudpProxyUDP443, v => v._muxUdp443.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Mux4SboxProtocol, v => v._muxSbox.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableCacheFile4Sbox, v => v._cache.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.HyUpMbps, v => v._up.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.HyDownMbps, v => v._down.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableFragment, v => v._fragment.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableFinalFragment, v => v._finalFragment.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FragmentPackets, v => v._fragPackets.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FragmentLengths, v => v._fragLen.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FragmentDelays, v => v._fragDelay.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FragmentMaxSplit, v => v._fragSplit.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AutoRun, v => v._autoRun.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableStatistics, v => v._stats.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DisplayRealTimeSpeed, v => v._speed.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.KeepOlderDedupl, v => v._dedup.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableAutoAdjustMainLvColWidth, v => v._autofit.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AutoHideStartup, v => v._hideStartup.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableDragDropSort, v => v._drag.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DoubleClick2Activate, v => v._dbl.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AutoUpdateInterval, v => v._update.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TrayMenuServersLimit, v => v._trayLimit.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CurrentFontFamily, v => v._font.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SpeedTestTimeout, v => v._timeout.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SpeedTestUrl, v => v._speedUrl.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SpeedPingTestUrl, v => v._pingUrl.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.UdpTestTarget, v => v._udpTarget.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.MixedConcurrencyCount, v => v._mixed.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableHWA, v => v._hwa.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SubConvertUrl, v => v._convert.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.MainGirdOrientation, v => v._orientation.SelectedIndex).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.GeoFileSourceUrl, v => v._geo.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SrsFileSourceUrl, v => v._srs.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.RoutingRulesSourceUrl, v => v._rules.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.IPAPIUrl, v => v._ipapi.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.RootCertProvider, v => v._cert.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.NotProxyLocalAddress, v => v._localBypass.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SystemProxyAdvancedProtocol, v => v._proxyProtocol.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SystemProxyExceptions, v => v._exceptions.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CustomSystemProxyPacPath, v => v._pac.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunAutoRoute, v => v._autoRoute.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunStrictRoute, v => v._strict.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunStack, v => v._stack.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunMtu, v => v._mtu.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunEnableIPv6Address, v => v._ipv6.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunIcmpRouting, v => v._icmp.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunEnableLegacyProtect, v => v._legacy.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunDnsMode, v => v._dnsMode.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunRouteExcludeAddress, v => v._exclude.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunIPv4Address, v => v._v4.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunIPv6Address, v => v._v6.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType1, v => v._c1.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType2, v => v._c2.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType3, v => v._c3.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType4, v => v._c4.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType5, v => v._c5.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType6, v => v._c6.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType7, v => v._c7.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType9, v => v._c9.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CoreType14, v => v._c14.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);
            foreach (var it in ViewModel?.DestOverride ?? [])
            {
                _dest.SelectedItems.Add(it);
            }

            _dest.SelectionChanged += (_, _) =>
            {
                if (ViewModel is not null)
                {
                    ViewModel.DestOverride = _dest.SelectedItems.Cast<string>().ToList();
                }
            };
        });
    }

    private static IEnumerable<string> FontNames()
    {
        var path = Utils.GetFontsPath();
        if (!Directory.Exists(path))
        {
            return [];
        }

        return Directory.GetFiles(path, "*.ttf").Concat(Directory.GetFiles(path, "*.ttc")).Select(Path.GetFileNameWithoutExtension)!;
    }
}

public sealed class DnsSettingWindow : ReactiveWindow<DNSSettingViewModel>
{
    private readonly StackPanel _simple = new() { Spacing = 8 };
    private readonly StackPanel _advanced = new() { Spacing = 8 };
    private readonly ToggleSwitch _useHosts;
    private readonly ToggleSwitch _commonHosts;
    private readonly ToggleSwitch _fake;
    private readonly AutoSuggestBox _fakeRange;
    private readonly ToggleSwitch _fake6;
    private readonly AutoSuggestBox _fake6Range;
    private readonly ToggleSwitch _blockBind;
    private readonly ToggleSwitch _blockAaaa;
    private readonly AutoSuggestBox _direct;
    private readonly AutoSuggestBox _remote;
    private readonly AutoSuggestBox _bootstrap;
    private readonly ComboBox _directStrategy;
    private readonly ComboBox _remoteStrategy;
    private readonly ComboBox _dialStrategy;
    private readonly TextBox _hosts;
    private readonly AutoSuggestBox _expected;
    private readonly ToggleSwitch _parallel;
    private readonly ToggleSwitch _stale;
    private readonly ToggleSwitch _happy;
    private readonly ToggleSwitch _rayCustom;
    private readonly ToggleSwitch _sbCustom;
    private readonly ToggleSwitch _hosts2;
    private readonly AutoSuggestBox _strategy;
    private readonly AutoSuggestBox _dns;
    private readonly TextBox _normal;
    private readonly TextBox _tun;
    private readonly AutoSuggestBox _strategy2;
    private readonly AutoSuggestBox _dns2;
    private readonly TextBox _normal2;
    private readonly TextBox _tun2;
    private readonly Button _importRay;
    private readonly Button _importSb;
    private readonly Button _save;

    public DnsSettingWindow()
    {
        Title = ResUI.menuDNSSetting;
        AppHost.Place(this, "DNSSettingWindow", 860, 720);
        var form = UiKit.Stack();
        form.Children.Add(_simple);
        form.Children.Add(_advanced);
        _useHosts = UiKit.Toggle(_simple, ResUI.TbSettingsUseSystemHosts);
        _commonHosts = UiKit.Toggle(_simple, "Add common hosts");
        _fake = UiKit.Toggle(_simple, "Fake IP");
        _fakeRange = UiKit.Suggest(_simple, "Fake IP range", Global.FakeIPRanges);
        _fake6 = UiKit.Toggle(_simple, "Fake IPv6");
        _fake6Range = UiKit.Suggest(_simple, "Fake IPv6 range", Global.FakeIPv6Ranges);
        _blockBind = UiKit.Toggle(_simple, "Block binding query");
        _blockAaaa = UiKit.Toggle(_simple, "Block AAAA");
        _direct = UiKit.Suggest(_simple, "Direct DNS", Global.DomainDirectDNSAddress);
        _remote = UiKit.Suggest(_simple, "Remote DNS", Global.DomainRemoteDNSAddress);
        _bootstrap = UiKit.Suggest(_simple, "Bootstrap DNS", Global.DomainPureIPDNSAddress);
        _directStrategy = UiKit.Combo(_simple, ResUI.TbSettingsDomainStrategy4Freedom, Global.DomainStrategy);
        _remoteStrategy = UiKit.Combo(_simple, "Remote strategy", Global.DomainStrategy);
        _dialStrategy = UiKit.Combo(_simple, "Proxy dial strategy", Global.DomainStrategy);
        _hosts = UiKit.Text(_simple, "Hosts", multi: true);
        _expected = UiKit.Suggest(_simple, "Expected IPs", Global.ExpectedIPs);
        _parallel = UiKit.Toggle(_simple, "Parallel query");
        _stale = UiKit.Toggle(_simple, "Serve stale");
        _happy = UiKit.Toggle(_simple, "Happy Eyeballs");
        _rayCustom = UiKit.Toggle(_advanced, "Ray custom DNS");
        _sbCustom = UiKit.Toggle(_advanced, "sing-box custom DNS");
        _hosts2 = UiKit.Toggle(_advanced, ResUI.TbSettingsUseSystemHosts);
        _strategy = UiKit.Suggest(_advanced, ResUI.TbSettingsDomainStrategy4Freedom, Global.DomainStrategy);
        _dns = UiKit.Suggest(_advanced, ResUI.TbSettingsDomainDNSAddress, Global.DomainPureIPDNSAddress);
        _normal = UiKit.Text(_advanced, "Normal DNS", multi: true);
        _tun = UiKit.Text(_advanced, "TUN DNS", multi: true);
        _strategy2 = UiKit.Suggest(_advanced, ResUI.TbSettingsDomainStrategy4Out, Global.DomainStrategies4Sbox);
        _dns2 = UiKit.Suggest(_advanced, ResUI.TbSettingsDomainDNSAddress, Global.DomainPureIPDNSAddress);
        _normal2 = UiKit.Text(_advanced, "Normal DNS 2", multi: true);
        _tun2 = UiKit.Text(_advanced, "TUN DNS 2", multi: true);
        _importRay = UiKit.Button(_advanced, "Import v2ray");
        _importSb = UiKit.Button(_advanced, "Import sing-box");
        _save = new Button { Content = ResUI.TbConfirm, MinWidth = 120, HorizontalAlignment = HorizontalAlignment.Left };
        var bar = new StackPanel { Orientation = Orientation.Horizontal, Padding = new Thickness(12) };
        bar.Children.Add(_save);
        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        Grid.SetRow(bar, 1);
        grid.Children.Add(UiKit.Scroll(form));
        grid.Children.Add(bar);
        Content = grid;

        this.WhenActivated(d =>
        {
            this.BindUnsafe(ViewModel, vm => vm.UseSystemHosts, v => v._useHosts.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AddCommonHosts, v => v._commonHosts.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FakeIP, v => v._fake.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FakeIPRange, v => v._fakeRange.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FakeIPv6, v => v._fake6.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.FakeIPv6Range, v => v._fake6Range.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.IsFakeIPv6Available, v => v._fake6.IsEnabled).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.IsFakeIPv6Available, v => v._fake6Range.IsEnabled).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.BlockBindingQuery, v => v._blockBind.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.BlockAAAAQuery, v => v._blockAaaa.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DirectDNS, v => v._direct.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.RemoteDNS, v => v._remote.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.BootstrapDNS, v => v._bootstrap.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Strategy4Freedom, v => v._directStrategy.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Strategy4Proxy, v => v._remoteStrategy.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Strategy4ProxyDial, v => v._dialStrategy.SelectedItem).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Hosts, v => v._hosts.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DirectExpectedIPs, v => v._expected.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.ParallelQuery, v => v._parallel.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.ServeStale, v => v._stale.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.EnableHappyEyeballs, v => v._happy.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.RayCustomDNSEnableCompatible, v => v._rayCustom.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SBCustomDNSEnableCompatible, v => v._sbCustom.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.UseSystemHostsCompatible, v => v._hosts2.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DomainStrategy4FreedomCompatible, v => v._strategy.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DomainDNSAddressCompatible, v => v._dns.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.NormalDNSCompatible, v => v._normal.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunDNSCompatible, v => v._tun.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DomainStrategy4Freedom2Compatible, v => v._strategy2.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.DomainDNSAddress2Compatible, v => v._dns2.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.NormalDNS2Compatible, v => v._normal2.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.TunDNS2Compatible, v => v._tun2.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ImportDefConfig4V2rayCompatibleCmd, v => v._importRay, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.ImportDefConfig4SingboxCompatibleCmd, v => v._importSb, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.IsSimpleDNSEnabled).Subscribe(on =>
            {
                _simple.IsHitTestVisible = on;
                _simple.Opacity = on ? 1 : 0.45;
                _advanced.IsHitTestVisible = !on;
                _advanced.Opacity = on ? 0.45 : 1;
            }).DisposeWith(d);
        });
    }
}
