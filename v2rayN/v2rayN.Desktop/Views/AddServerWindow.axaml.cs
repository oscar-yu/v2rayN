﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;

namespace v2rayN.Desktop.Views
{
    public partial class AddServerWindow : ReactiveWindow<AddServerViewModel>
    {
        public AddServerWindow()
        {
            InitializeComponent();
        }

        public AddServerWindow(ProfileItem profileItem)
        {
            InitializeComponent();

            this.Loaded += Window_Loaded;
            btnCancel.Click += (s, e) => this.Close();
            cmbNetwork.SelectionChanged += CmbNetwork_SelectionChanged;
            cmbStreamSecurity.SelectionChanged += CmbStreamSecurity_SelectionChanged;
            btnGUID.Click += btnGUID_Click;
            btnGUID5.Click += btnGUID_Click;

            ViewModel = new AddServerViewModel(profileItem, UpdateViewHandler);

            Global.CoreTypes.ForEach(it =>
            {
                cmbCoreType.Items.Add(it);
            });
            cmbCoreType.Items.Add(string.Empty);

            cmbStreamSecurity.Items.Add(string.Empty);
            cmbStreamSecurity.Items.Add(Global.StreamSecurity);

            Global.Networks.ForEach(it =>
            {
                cmbNetwork.Items.Add(it);
            });
            Global.Fingerprints.ForEach(it =>
            {
                cmbFingerprint.Items.Add(it);
                cmbFingerprint2.Items.Add(it);
            });
            Global.AllowInsecure.ForEach(it =>
            {
                cmbAllowInsecure.Items.Add(it);
            });
            Global.Alpns.ForEach(it =>
            {
                cmbAlpn.Items.Add(it);
            });

            switch (profileItem.ConfigType)
            {
                case EConfigType.VMess:
                    gridVMess.IsVisible = true;
                    Global.VmessSecurities.ForEach(it =>
                    {
                        cmbSecurity.Items.Add(it);
                    });
                    if (profileItem.Security.IsNullOrEmpty())
                    {
                        profileItem.Security = Global.DefaultSecurity;
                    }
                    break;

                case EConfigType.Shadowsocks:
                    gridSs.IsVisible = true;
                    AppHandler.Instance.GetShadowsocksSecurities(profileItem).ForEach(it =>
                    {
                        cmbSecurity3.Items.Add(it);
                    });
                    break;

                case EConfigType.SOCKS:
                case EConfigType.HTTP:
                    gridSocks.IsVisible = true;
                    break;

                case EConfigType.VLESS:
                    gridVLESS.IsVisible = true;
                    cmbStreamSecurity.Items.Add(Global.StreamSecurityReality);
                    Global.Flows.ForEach(it =>
                    {
                        cmbFlow5.Items.Add(it);
                    });
                    if (profileItem.Security.IsNullOrEmpty())
                    {
                        profileItem.Security = Global.None;
                    }
                    break;

                case EConfigType.Trojan:
                    gridTrojan.IsVisible = true;
                    cmbStreamSecurity.Items.Add(Global.StreamSecurityReality);
                    Global.Flows.ForEach(it =>
                    {
                        cmbFlow6.Items.Add(it);
                    });
                    break;

                case EConfigType.Hysteria2:
                    gridHysteria2.IsVisible = true;
                    sepa2.IsVisible = false;
                    gridTransport.IsVisible = false;
                    cmbCoreType.IsEnabled = false;
                    cmbFingerprint.IsEnabled = false;
                    cmbFingerprint.SelectedValue = string.Empty;
                    break;

                case EConfigType.TUIC:
                    gridTuic.IsVisible = true;
                    sepa2.IsVisible = false;
                    gridTransport.IsVisible = false;
                    cmbCoreType.IsEnabled = false;
                    cmbFingerprint.IsEnabled = false;
                    cmbFingerprint.SelectedValue = string.Empty;

                    Global.TuicCongestionControls.ForEach(it =>
                    {
                        cmbHeaderType8.Items.Add(it);
                    });
                    break;

                case EConfigType.WireGuard:
                    gridWireguard.IsVisible = true;

                    sepa2.IsVisible = false;
                    gridTransport.IsVisible = false;
                    gridTls.IsVisible = false;
                    cmbCoreType.IsEnabled = false;

                    break;
            }

            gridTlsMore.IsVisible = false;

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.CoreType, v => v.cmbCoreType.SelectedValue).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.Remarks, v => v.txtRemarks.Text).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.Address, v => v.txtAddress.Text).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.Port, v => v.txtPort.Text).DisposeWith(disposables);

                switch (profileItem.ConfigType)
                {
                    case EConfigType.VMess:
                        this.Bind(ViewModel, vm => vm.SelectedSource.Id, v => v.txtId.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.AlterId, v => v.txtAlterId.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.Security, v => v.cmbSecurity.SelectedValue).DisposeWith(disposables);
                        break;

                    case EConfigType.Shadowsocks:
                        this.Bind(ViewModel, vm => vm.SelectedSource.Id, v => v.txtId3.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.Security, v => v.cmbSecurity3.SelectedValue).DisposeWith(disposables);
                        break;

                    case EConfigType.SOCKS:
                    case EConfigType.HTTP:
                        this.Bind(ViewModel, vm => vm.SelectedSource.Id, v => v.txtId4.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.Security, v => v.txtSecurity4.Text).DisposeWith(disposables);
                        break;

                    case EConfigType.VLESS:
                        this.Bind(ViewModel, vm => vm.SelectedSource.Id, v => v.txtId5.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.Flow, v => v.cmbFlow5.SelectedValue).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.Security, v => v.txtSecurity5.Text).DisposeWith(disposables);
                        break;

                    case EConfigType.Trojan:
                        this.Bind(ViewModel, vm => vm.SelectedSource.Id, v => v.txtId6.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.Flow, v => v.cmbFlow6.SelectedValue).DisposeWith(disposables);
                        break;

                    case EConfigType.Hysteria2:
                        this.Bind(ViewModel, vm => vm.SelectedSource.Id, v => v.txtId7.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.Path, v => v.txtPath7.Text).DisposeWith(disposables);
                        break;

                    case EConfigType.TUIC:
                        this.Bind(ViewModel, vm => vm.SelectedSource.Id, v => v.txtId8.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.Security, v => v.txtSecurity8.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.HeaderType, v => v.cmbHeaderType8.SelectedValue).DisposeWith(disposables);
                        break;

                    case EConfigType.WireGuard:
                        this.Bind(ViewModel, vm => vm.SelectedSource.Id, v => v.txtId9.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.PublicKey, v => v.txtPublicKey9.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.Path, v => v.txtPath9.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.RequestHost, v => v.txtRequestHost9.Text).DisposeWith(disposables);
                        this.Bind(ViewModel, vm => vm.SelectedSource.ShortId, v => v.txtShortId9.Text).DisposeWith(disposables);
                        break;
                }
                this.Bind(ViewModel, vm => vm.SelectedSource.Network, v => v.cmbNetwork.SelectedValue).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.HeaderType, v => v.cmbHeaderType.SelectedValue).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.RequestHost, v => v.txtRequestHost.Text).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.Path, v => v.txtPath.Text).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.Extra, v => v.txtExtra.Text).DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SelectedSource.StreamSecurity, v => v.cmbStreamSecurity.SelectedValue).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.Sni, v => v.txtSNI.Text).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.AllowInsecure, v => v.cmbAllowInsecure.SelectedValue).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.Fingerprint, v => v.cmbFingerprint.SelectedValue).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.Alpn, v => v.cmbAlpn.SelectedValue).DisposeWith(disposables);
                //reality
                this.Bind(ViewModel, vm => vm.SelectedSource.Sni, v => v.txtSNI2.Text).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.Fingerprint, v => v.cmbFingerprint2.SelectedValue).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.PublicKey, v => v.txtPublicKey.Text).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.ShortId, v => v.txtShortId.Text).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedSource.SpiderX, v => v.txtSpiderX.Text).DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.SaveCmd, v => v.btnSave).DisposeWith(disposables);
            });

            this.Title = $"{profileItem.ConfigType}";
        }

        private async Task<bool> UpdateViewHandler(EViewAction action, object? obj)
        {
            switch (action)
            {
                case EViewAction.CloseWindow:
                    this.Close(true);
                    break;
            }
            return await Task.FromResult(true);
        }

        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            txtRemarks.Focus();
        }

        private void CmbNetwork_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            SetHeaderType();
            SetTips();
        }

        private void CmbStreamSecurity_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var security = cmbStreamSecurity.SelectedItem.ToString();
            if (security == Global.StreamSecurityReality)
            {
                gridRealityMore.IsVisible = true;
                gridTlsMore.IsVisible = false;
            }
            else if (security == Global.StreamSecurity)
            {
                gridRealityMore.IsVisible = false;
                gridTlsMore.IsVisible = true;
            }
            else
            {
                gridRealityMore.IsVisible = false;
                gridTlsMore.IsVisible = false;
            }
        }

        private void btnGUID_Click(object? sender, RoutedEventArgs e)
        {
            txtId.Text =
            txtId5.Text = Utils.GetGuid();
        }

        private void SetHeaderType()
        {
            cmbHeaderType.Items.Clear();

            var network = cmbNetwork.SelectedItem.ToString();
            if (Utils.IsNullOrEmpty(network))
            {
                cmbHeaderType.Items.Add(Global.None);
                return;
            }

            if (network == nameof(ETransport.tcp))
            {
                cmbHeaderType.Items.Add(Global.None);
                cmbHeaderType.Items.Add(Global.TcpHeaderHttp);
            }
            else if (network is nameof(ETransport.kcp) or nameof(ETransport.quic))
            {
                cmbHeaderType.Items.Add(Global.None);
                Global.KcpHeaderTypes.ForEach(it =>
                {
                    cmbHeaderType.Items.Add(it);
                });
            }
            else if (network is nameof(ETransport.xhttp))
            {
                Global.XhttpMode.ForEach(it =>
                {
                    cmbHeaderType.Items.Add(it);
                });
            }
            else if (network == nameof(ETransport.grpc))
            {
                cmbHeaderType.Items.Add(Global.GrpcGunMode);
                cmbHeaderType.Items.Add(Global.GrpcMultiMode);
            }
            else
            {
                cmbHeaderType.Items.Add(Global.None);
            }
            cmbHeaderType.SelectedIndex = 0;
        }

        private void SetTips()
        {
            var network = cmbNetwork.SelectedItem.ToString();
            if (Utils.IsNullOrEmpty(network))
            {
                network = Global.DefaultNetwork;
            }
            labHeaderType.IsVisible = true;
            btnExtra.IsVisible = false;
            tipRequestHost.Text =
            tipPath.Text =
            tipHeaderType.Text = string.Empty;

            switch (network)
            {
                case nameof(ETransport.tcp):
                    tipRequestHost.Text = ResUI.TransportRequestHostTip1;
                    tipHeaderType.Text = ResUI.TransportHeaderTypeTip1;
                    break;

                case nameof(ETransport.kcp):
                    tipHeaderType.Text = ResUI.TransportHeaderTypeTip2;
                    tipPath.Text = ResUI.TransportPathTip5;
                    break;

                case nameof(ETransport.ws):
                case nameof(ETransport.httpupgrade):
                    tipRequestHost.Text = ResUI.TransportRequestHostTip2;
                    tipPath.Text = ResUI.TransportPathTip1;
                    break;

                case nameof(ETransport.xhttp):
                    tipRequestHost.Text = ResUI.TransportRequestHostTip2;
                    tipPath.Text = ResUI.TransportPathTip1;
                    tipHeaderType.Text = ResUI.TransportHeaderTypeTip5;
                    labHeaderType.IsVisible = false;
                    btnExtra.IsVisible = true;
                    break;

                case nameof(ETransport.h2):
                    tipRequestHost.Text = ResUI.TransportRequestHostTip3;
                    tipPath.Text = ResUI.TransportPathTip2;
                    break;

                case nameof(ETransport.quic):
                    tipRequestHost.Text = ResUI.TransportRequestHostTip4;
                    tipPath.Text = ResUI.TransportPathTip3;
                    tipHeaderType.Text = ResUI.TransportHeaderTypeTip3;
                    break;

                case nameof(ETransport.grpc):
                    tipRequestHost.Text = ResUI.TransportRequestHostTip5;
                    tipPath.Text = ResUI.TransportPathTip4;
                    tipHeaderType.Text = ResUI.TransportHeaderTypeTip4;
                    labHeaderType.IsVisible = false;
                    break;
            }
        }
    }
}