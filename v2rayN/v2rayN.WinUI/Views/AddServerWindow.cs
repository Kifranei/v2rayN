namespace v2rayN.WinUI.Views;

public sealed class AddServerWindow : ReactiveWindow<AddServerViewModel>
{
    private readonly AutoSuggestBox _core;
    private readonly TextBox _remarks;
    private readonly TextBox _address;
    private readonly TextBox _port;
    private readonly StackPanel _vmess;
    private readonly TextBox _id;
    private readonly TextBox _alterId;
    private readonly AutoSuggestBox _vmessSecurity;
    private readonly ToggleSwitch _mux;
    private readonly StackPanel _ss;
    private readonly TextBox _ssPass;
    private readonly AutoSuggestBox _ssMethod;
    private readonly ToggleSwitch _ssUot;
    private readonly ToggleSwitch _ssMux;
    private readonly StackPanel _socks;
    private readonly TextBox _socksUser;
    private readonly TextBox _socksPass;
    private readonly TextBox _httpHeaders;
    private readonly StackPanel _vless;
    private readonly TextBox _vlessId;
    private readonly AutoSuggestBox _flow;
    private readonly TextBox _vlessEnc;
    private readonly ToggleSwitch _vlessMux;
    private readonly StackPanel _trojan;
    private readonly TextBox _trojanPass;
    private readonly AutoSuggestBox _trojanFlow;
    private readonly ToggleSwitch _trojanMux;
    private readonly StackPanel _hy2;
    private readonly TextBox _hyPass;
    private readonly TextBox _salamander;
    private readonly TextBox _ports;
    private readonly TextBox _hop;
    private readonly TextBox _up;
    private readonly TextBox _down;
    private readonly StackPanel _tuic;
    private readonly TextBox _tuicUser;
    private readonly TextBox _tuicPass;
    private readonly AutoSuggestBox _tuicCongestion;
    private readonly StackPanel _wg;
    private readonly TextBox _wgSecret;
    private readonly TextBox _wgPublic;
    private readonly TextBox _wgPsk;
    private readonly TextBox _wgReserved;
    private readonly TextBox _wgAddress;
    private readonly TextBox _wgMtu;
    private readonly TextBox _wgDns;
    private readonly StackPanel _anytls;
    private readonly TextBox _anyPass;
    private readonly StackPanel _naive;
    private readonly TextBox _naiveUser;
    private readonly TextBox _naivePass;
    private readonly ToggleSwitch _naiveQuic;
    private readonly AutoSuggestBox _naiveCongestion;
    private readonly TextBox _naiveConcurrency;
    private readonly ToggleSwitch _naiveUot;
    private readonly StackPanel _masque;
    private readonly TextBox _masqueUser;
    private readonly TextBox _masquePass;
    private readonly TextBox _masquePath;
    private readonly TextBox _masqueHeaders;
    private readonly StackPanel _transport;
    private readonly AutoSuggestBox _network;
    private readonly StackPanel _raw;
    private readonly AutoSuggestBox _rawHeader;
    private readonly TextBox _rawHost;
    private readonly TextBox _rawPath;
    private readonly StackPanel _kcp;
    private readonly AutoSuggestBox _kcpHeader;
    private readonly TextBox _kcpSeed;
    private readonly TextBox _kcpMtu;
    private readonly StackPanel _ws;
    private readonly TextBox _wsHost;
    private readonly TextBox _wsPath;
    private readonly StackPanel _http;
    private readonly TextBox _httpHost;
    private readonly TextBox _httpPath;
    private readonly StackPanel _xhttp;
    private readonly AutoSuggestBox _xhttpMode;
    private readonly TextBox _xhttpHost;
    private readonly TextBox _xhttpPath;
    private readonly TextBox _xhttpExtra;
    private readonly StackPanel _grpc;
    private readonly AutoSuggestBox _grpcMode;
    private readonly TextBox _grpcHost;
    private readonly TextBox _grpcPath;
    private readonly StackPanel _tls;
    private readonly AutoSuggestBox _security;
    private readonly StackPanel _tlsMore;
    private readonly TextBox _sni;
    private readonly ToggleSwitch _insecure;
    private readonly AutoSuggestBox _fingerprint;
    private readonly AutoSuggestBox _alpn;
    private readonly TextBox _certSha;
    private readonly TextBlock _certTip;
    private readonly TextBox _cert;
    private readonly TextBox _ech;
    private readonly TextBox _verifyName;
    private readonly StackPanel _reality;
    private readonly TextBox _sni2;
    private readonly AutoSuggestBox _fingerprint2;
    private readonly TextBox _publicKey;
    private readonly TextBox _shortId;
    private readonly TextBox _spiderX;
    private readonly TextBox _mldsa;
    private readonly TextBox _finalmask;
    private readonly Button _fetch;
    private readonly Button _fetchChain;
    private readonly Button _save;
    private readonly MultipleDisposable _typeBinds = new();

    public AddServerWindow()
    {
        Title = ResUI.menuAddCustomServer;
        AppHost.Place(this, "AddServerWindow", 860, 760);
        var form = UiKit.Stack();
        _core = UiKit.Suggest(form, ResUI.TbSettingsCoreType, Global.CoreTypes.AppendEmpty());
        _remarks = UiKit.Text(form, ResUI.TbRemarks);
        _address = UiKit.Text(form, ResUI.TbAddress);
        _port = UiKit.Text(form, ResUI.TbPort);
        var guid = UiKit.Button(form, ResUI.TbGUID);
        guid.Click += (_, _) =>
        {
            var id = Utils.GetGuid();
            _id.Text = id;
            _vlessId.Text = id;
        };

        _vmess = Panel(form);
        _id = UiKit.Text(_vmess, ResUI.TbId);
        _alterId = UiKit.Text(_vmess, ResUI.TbAlterId);
        _vmessSecurity = UiKit.Suggest(_vmess, ResUI.TbSecurity, Global.VmessSecurities);
        _mux = UiKit.Toggle(_vmess, ResUI.TbSettingsMuxEnabled);
        _ss = Panel(form);
        _ssPass = UiKit.Text(_ss, ResUI.TbId3);
        _ssMethod = UiKit.Suggest(_ss, ResUI.TbSecurity3, null);
        _ssUot = UiKit.Toggle(_ss, "UDP over TCP");
        _ssMux = UiKit.Toggle(_ss, ResUI.TbSettingsMuxEnabled);
        _socks = Panel(form);
        _socksUser = UiKit.Text(_socks, ResUI.TbSecurity4);
        _socksPass = UiKit.Text(_socks, ResUI.TbId4);
        _httpHeaders = UiKit.Text(_socks, "HTTP headers", multi: true);
        _vless = Panel(form);
        _vlessId = UiKit.Text(_vless, ResUI.TbId5);
        _flow = UiKit.Suggest(_vless, ResUI.TbFlow5, Global.Flows);
        _vlessEnc = UiKit.Text(_vless, ResUI.TbSecurity5);
        _vlessMux = UiKit.Toggle(_vless, ResUI.TbSettingsMuxEnabled);
        _trojan = Panel(form);
        _trojanPass = UiKit.Text(_trojan, ResUI.TbId);
        _trojanFlow = UiKit.Suggest(_trojan, ResUI.TbFlow5, Global.Flows);
        _trojanMux = UiKit.Toggle(_trojan, ResUI.TbSettingsMuxEnabled);
        _hy2 = Panel(form);
        _hyPass = UiKit.Text(_hy2, ResUI.TbId);
        _salamander = UiKit.Text(_hy2, ResUI.TbPath7);
        _ports = UiKit.Text(_hy2, ResUI.TbPorts7);
        _hop = UiKit.Text(_hy2, "Hop interval");
        _up = UiKit.Text(_hy2, "Up Mbps");
        _down = UiKit.Text(_hy2, "Down Mbps");
        _tuic = Panel(form);
        _tuicUser = UiKit.Text(_tuic, ResUI.TbSecurity4);
        _tuicPass = UiKit.Text(_tuic, ResUI.TbId);
        _tuicCongestion = UiKit.Suggest(_tuic, "Congestion", Global.TuicCongestionControls);
        _wg = Panel(form);
        _wgSecret = UiKit.Text(_wg, ResUI.TbId);
        _wgPublic = UiKit.Text(_wg, ResUI.TbPublicKey);
        _wgPsk = UiKit.Text(_wg, "Preshared key");
        _wgReserved = UiKit.Text(_wg, "Reserved");
        _wgAddress = UiKit.Text(_wg, ResUI.TbAddress);
        _wgMtu = UiKit.Text(_wg, "MTU");
        _wgDns = UiKit.Text(_wg, "DNS");
        _anytls = Panel(form);
        _anyPass = UiKit.Text(_anytls, ResUI.TbId);
        _naive = Panel(form);
        _naiveUser = UiKit.Text(_naive, ResUI.TbSecurity4);
        _naivePass = UiKit.Text(_naive, ResUI.TbId);
        _naiveQuic = UiKit.Toggle(_naive, "QUIC");
        _naiveCongestion = UiKit.Suggest(_naive, "Congestion", Global.NaiveCongestionControls);
        _naiveConcurrency = UiKit.Text(_naive, "Insecure concurrency");
        _naiveUot = UiKit.Toggle(_naive, "UDP over TCP");
        _masque = Panel(form);
        _masqueUser = UiKit.Text(_masque, ResUI.TbSecurity4);
        _masquePass = UiKit.Text(_masque, ResUI.TbId);
        _masquePath = UiKit.Text(_masque, ResUI.TbPath);
        _masqueHeaders = UiKit.Text(_masque, "Headers", multi: true);

        _transport = Panel(form);
        UiKit.Header(_transport, ResUI.TbNetwork);
        _network = UiKit.Suggest(_transport, ResUI.TbNetwork, Global.Networks);
        _raw = Panel(_transport);
        _rawHeader = UiKit.Suggest(_raw, "Header", ["", "none", "http"]);
        _rawHost = UiKit.Text(_raw, ResUI.TbHost);
        _rawPath = UiKit.Text(_raw, ResUI.TbPath);
        _kcp = Panel(_transport);
        _kcpHeader = UiKit.Suggest(_kcp, "Header", Global.KcpHeaderTypes);
        _kcpSeed = UiKit.Text(_kcp, "Seed");
        _kcpMtu = UiKit.Text(_kcp, "MTU");
        _ws = Panel(_transport);
        _wsHost = UiKit.Text(_ws, ResUI.TbHost);
        _wsPath = UiKit.Text(_ws, ResUI.TbPath);
        _http = Panel(_transport);
        _httpHost = UiKit.Text(_http, ResUI.TbHost);
        _httpPath = UiKit.Text(_http, ResUI.TbPath);
        _xhttp = Panel(_transport);
        _xhttpMode = UiKit.Suggest(_xhttp, "Mode", Global.XhttpMode);
        _xhttpHost = UiKit.Text(_xhttp, ResUI.TbHost);
        _xhttpPath = UiKit.Text(_xhttp, ResUI.TbPath);
        _xhttpExtra = UiKit.Text(_xhttp, "Extra", multi: true);
        _grpc = Panel(_transport);
        _grpcMode = UiKit.Suggest(_grpc, "Mode", ["gun", "multi"]);
        _grpcHost = UiKit.Text(_grpc, ResUI.TbHost);
        _grpcPath = UiKit.Text(_grpc, ResUI.TbPath);

        _tls = Panel(form);
        UiKit.Header(_tls, ResUI.TbStreamSecurity);
        _security = UiKit.Suggest(_tls, ResUI.TbStreamSecurity, [string.Empty, Global.StreamSecurity]);
        _tlsMore = Panel(_tls);
        _sni = UiKit.Text(_tlsMore, ResUI.TbSNI);
        _insecure = UiKit.Toggle(_tlsMore, ResUI.TbAllowInsecure);
        _fingerprint = UiKit.Suggest(_tlsMore, ResUI.TbFingerprint, Global.Fingerprints);
        _alpn = UiKit.Suggest(_tlsMore, ResUI.TbAlpn, Global.Alpns);
        _certSha = UiKit.Text(_tlsMore, ResUI.TbCertPinning);
        _certTip = new TextBlock { TextWrapping = TextWrapping.Wrap };
        _tlsMore.Children.Add(_certTip);
        _cert = UiKit.Text(_tlsMore, "Cert", multi: true);
        _ech = UiKit.Text(_tlsMore, "ECH");
        _verifyName = UiKit.Text(_tlsMore, "Verify name");
        _fetch = UiKit.Button(_tlsMore, ResUI.TbFetchCert);
        _fetchChain = UiKit.Button(_tlsMore, ResUI.TbFetchCertChain);
        _reality = Panel(_tls);
        _sni2 = UiKit.Text(_reality, ResUI.TbSNI);
        _fingerprint2 = UiKit.Suggest(_reality, ResUI.TbFingerprint, Global.Fingerprints);
        _publicKey = UiKit.Text(_reality, ResUI.TbPublicKey);
        _shortId = UiKit.Text(_reality, ResUI.TbShortId);
        _spiderX = UiKit.Text(_reality, "SpiderX");
        _mldsa = UiKit.Text(_reality, "ML-DSA");
        _finalmask = UiKit.Text(form, "Finalmask", multi: true);
        (_save, Content) = FullConfigTemplateWindow.SaveBar(form);

        this.WhenActivated(d =>
        {
            this.BindUnsafe(ViewModel, vm => vm.CoreType, v => v._core.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Remarks, v => v._remarks.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Address, v => v._address.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Port, v => v._port.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Network, v => v._network.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.RawHeaderType, v => v._rawHeader.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.KcpHeaderType, v => v._kcpHeader.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.KcpSeed, v => v._kcpSeed.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.KcpMtu, v => v._kcpMtu.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.XhttpMode, v => v._xhttpMode.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.XhttpExtra, v => v._xhttpExtra.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.GrpcMode, v => v._grpcMode.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.GrpcAuthority, v => v._grpcHost.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.GrpcServiceName, v => v._grpcPath.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.StreamSecurity, v => v._security.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.AllowInsecure, v => v._insecure.IsOn).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.CertSha, v => v._certSha.Text).DisposeWith(d);
            this.OneWayBindUnsafe(ViewModel, vm => vm.CertTip, v => v._certTip.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.Cert, v => v._cert.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.EchConfigList, v => v._ech.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.VerifyPeerCertByName, v => v._verifyName.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.PublicKey, v => v._publicKey.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.ShortId, v => v._shortId.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.SpiderX, v => v._spiderX.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Mldsa65Verify, v => v._mldsa.Text).DisposeWith(d);
            this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Finalmask, v => v._finalmask.Text).DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.FetchCertCmd, v => v._fetch, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.FetchCertChainCmd, v => v._fetchChain, "Click").DisposeWith(d);
            this.BindCommandUnsafe(ViewModel, vm => vm.SaveCmd, v => v._save, "Click").DisposeWith(d);

            this.WhenAnyValue(v => v.ViewModel!.SelectedSource).Subscribe(ApplyType).DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.Network).Subscribe(ShowTransport).DisposeWith(d);
            this.WhenAnyValue(v => v.ViewModel!.SelectedSource.StreamSecurity).Subscribe(ShowSecurity).DisposeWith(d);
            _typeBinds.DisposeWith(d);
        });
    }

    private static StackPanel Panel(Panel parent)
    {
        var panel = new StackPanel { Spacing = 8, Visibility = Visibility.Collapsed };
        parent.Children.Add(panel);
        return panel;
    }

    private void ApplyType(ProfileItem? item)
    {
        _typeBinds.Clear();
        foreach (var panel in new UIElement[] { _vmess, _ss, _socks, _vless, _trojan, _hy2, _tuic, _wg, _anytls, _naive, _masque })
        {
            DialogPage.Show(panel, false);
        }

        DialogPage.Show(_httpHeaders, false);
        DialogPage.Show(_transport, true);
        DialogPage.Show(_tls, true);
        DialogPage.Show(_finalmask, true);
        _core.IsEnabled = true;
        if (item is null || ViewModel is null)
        {
            return;
        }

        Title = item.ConfigType.ToString();
        var securities = new List<string> { string.Empty, Global.StreamSecurity };
        switch (item.ConfigType)
        {
            case EConfigType.VMess:
                DialogPage.Show(_vmess, true);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._id.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.AlterId, v => v._alterId.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.VmessSecurity, v => v._vmessSecurity.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.MuxEnabled, v => v._mux.IsOn).DisposeWith(_typeBinds);
                break;
            case EConfigType.Shadowsocks:
                DialogPage.Show(_ss, true);
                _ssMethod.ItemsSource = AppManager.Instance.GetShadowsocksSecurities(item);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._ssPass.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.SsMethod, v => v._ssMethod.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.Uot, v => v._ssUot.IsOn).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.MuxEnabled, v => v._ssMux.IsOn).DisposeWith(_typeBinds);
                break;
            case EConfigType.SOCKS:
            case EConfigType.HTTP:
                DialogPage.Show(_socks, true);
                DialogPage.Show(_httpHeaders, item.ConfigType == EConfigType.HTTP);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Username, v => v._socksUser.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._socksPass.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.HttpHeadersJson, v => v._httpHeaders.Text).DisposeWith(_typeBinds);
                break;
            case EConfigType.VLESS:
                DialogPage.Show(_vless, true);
                securities.Add(Global.StreamSecurityReality);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._vlessId.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.Flow, v => v._flow.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.VlessEncryption, v => v._vlessEnc.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.MuxEnabled, v => v._vlessMux.IsOn).DisposeWith(_typeBinds);
                break;
            case EConfigType.Trojan:
                DialogPage.Show(_trojan, true);
                securities.Add(Global.StreamSecurityReality);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._trojanPass.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.Flow, v => v._trojanFlow.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.MuxEnabled, v => v._trojanMux.IsOn).DisposeWith(_typeBinds);
                break;
            case EConfigType.Hysteria2:
                DialogPage.Show(_hy2, true);
                DialogPage.Show(_transport, false);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._hyPass.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.SalamanderPass, v => v._salamander.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.Ports, v => v._ports.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.HopInterval, v => v._hop.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.UpMbps, v => v._up.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.DownMbps, v => v._down.Text).DisposeWith(_typeBinds);
                break;
            case EConfigType.TUIC:
                DialogPage.Show(_tuic, true);
                DialogPage.Show(_transport, false);
                DialogPage.Show(_finalmask, false);
                _core.IsEnabled = false;
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Username, v => v._tuicUser.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._tuicPass.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.CongestionControl, v => v._tuicCongestion.Text).DisposeWith(_typeBinds);
                break;
            case EConfigType.WireGuard:
                DialogPage.Show(_wg, true);
                DialogPage.Show(_transport, false);
                DialogPage.Show(_tls, false);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._wgSecret.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.WgPublicKey, v => v._wgPublic.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.WgPresharedKey, v => v._wgPsk.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.WgReserved, v => v._wgReserved.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.WgInterfaceAddress, v => v._wgAddress.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.WgMtu, v => v._wgMtu.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.WgDns, v => v._wgDns.Text).DisposeWith(_typeBinds);
                break;
            case EConfigType.Anytls:
                DialogPage.Show(_anytls, true);
                DialogPage.Show(_transport, false);
                DialogPage.Show(_finalmask, false);
                _core.IsEnabled = false;
                securities.Add(Global.StreamSecurityReality);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._anyPass.Text).DisposeWith(_typeBinds);
                break;
            case EConfigType.Naive:
                DialogPage.Show(_naive, true);
                DialogPage.Show(_transport, false);
                DialogPage.Show(_finalmask, false);
                _core.IsEnabled = false;
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Username, v => v._naiveUser.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._naivePass.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.NaiveQuic, v => v._naiveQuic.IsOn).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.CongestionControl, v => v._naiveCongestion.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.InsecureConcurrency, v => v._naiveConcurrency.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.Uot, v => v._naiveUot.IsOn).DisposeWith(_typeBinds);
                break;
            case EConfigType.MASQUE:
                DialogPage.Show(_masque, true);
                DialogPage.Show(_transport, false);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Username, v => v._masqueUser.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.SelectedSource.Password, v => v._masquePass.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.MasquePath, v => v._masquePath.Text).DisposeWith(_typeBinds);
                this.BindUnsafe(ViewModel, vm => vm.MasqueHeaders, v => v._masqueHeaders.Text).DisposeWith(_typeBinds);
                break;
        }

        _security.ItemsSource = securities;
        ShowSecurity(item.StreamSecurity);
    }

    private void ShowTransport(string? network)
    {
        if (network.IsNullOrEmpty())
        {
            network = Global.DefaultNetwork;
        }

        foreach (var panel in new UIElement[] { _raw, _kcp, _ws, _http, _xhttp, _grpc })
        {
            DialogPage.Show(panel, false);
        }

        var host = _rawHost;
        var path = _rawPath;
        switch (network)
        {
            case nameof(ETransport.kcp):
                DialogPage.Show(_kcp, true);
                return;
            case nameof(ETransport.ws):
                DialogPage.Show(_ws, true);
                host = _wsHost;
                path = _wsPath;
                break;
            case nameof(ETransport.httpupgrade):
                DialogPage.Show(_http, true);
                host = _httpHost;
                path = _httpPath;
                break;
            case nameof(ETransport.xhttp):
                DialogPage.Show(_xhttp, true);
                host = _xhttpHost;
                path = _xhttpPath;
                break;
            case nameof(ETransport.grpc):
                DialogPage.Show(_grpc, true);
                return;
            default:
                DialogPage.Show(_raw, true);
                break;
        }

        if (ViewModel is null)
        {
            return;
        }

        host.Text = ViewModel.Host;
        path.Text = ViewModel.Path;
        host.TextChanged += (_, _) => ViewModel.Host = host.Text;
        path.TextChanged += (_, _) => ViewModel.Path = path.Text;
    }

    private void ShowSecurity(string? security)
    {
        var reality = security == Global.StreamSecurityReality;
        var tls = security == Global.StreamSecurity;
        DialogPage.Show(_reality, reality);
        DialogPage.Show(_tlsMore, tls);
        if (ViewModel is null)
        {
            return;
        }

        var box = reality ? _sni2 : _sni;
        var print = reality ? _fingerprint2 : _fingerprint;
        box.Text = ViewModel.SelectedSource?.Sni;
        print.Text = ViewModel.SelectedSource?.Fingerprint;
    }
}
