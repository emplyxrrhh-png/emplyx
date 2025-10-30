<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="true" Inherits="VTLive40.Alerts" Title="" EnableEventValidation="false" CodeBehind="Alerts.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            setTimeout(function () { initializeScreen(); }, 150);

            window.parent.setUPReportsAndWizards({ HasReports: false, HasAssistants: false });
        }

        function initializeScreen() {
            setInterval(alerts.loadData, 60000);
        }

        function showAlertDetailPopUp(contentUrl) {
            try {
                parent.showLoader(true);
                AlertDetailsPopup_Client.SetContentUrl(contentUrl);
                AlertDetailsPopup_Client.Show();
            } catch (e) { showError('showAlertDetailPopUp', e); }
        }
    </script>

    <input type="hidden" runat="server" id="noRegs" value="" />

    <div id="divMainBody">

        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divTabAlert" class="blackRibbonTitle">
                <div class="blackRibbonIcon">
                    <img src="Images/Alerts80.png" alt="" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Alertas" CssClass="NameText"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="En esta pantalla se mostrarán las alertas y tareas pendientes que deben realizarse."></asp:Label>
                </div>
                <div id="divAlertsStatusContent" style="width: 175px; margin-top: 10px; float: right;">
                    <div class="NewsSupportPosition">
                        <asp:Label ID="lblSupportText" runat="server" Font-Bold="true" Style="color: #333333" Text=""></asp:Label>
                        <br />
                        <div id="divLastUpdateSite" style="color: #333333">
                            <asp:Label ID="lastUpdateText" runat="server"></asp:Label>
                        </div>

                        <asp:ImageButton ID="ibtRefreshAlerts" runat="server" ImageUrl="~/Base/Images/PortalRequests/refreshAlerts.png" Style="cursor: pointer; padding-top: 5px;" OnClientClick="refreshAlertsSite(); return false;" />

                        <br />
                    </div>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>

        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial;" class="maxHeight">

                    <dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxCallbackPanelContenidoClient">
                        <SettingsLoadingPanel Enabled="false" />
                        <ClientSideEvents EndCallback="alerts.ASPxCallbackPanelContenidoClient_EndCallBack" />
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent1" runat="server">
                                <input type="hidden" id="hidden_refresh" value="1" class="hidden_refresh" runat="server" />
                                <div id="NoAlerts" runat="server" style="display: none; height: 100%; padding-top: 100px;">
                                    <div class="mainBlock" style="width: 20%; float: left">
                                        <div class="mainCentered">
                                            <div class="alertBlock greenCircle">
                                                <div class="centered">OK</div>
                                            </div>
                                        </div>
                                    </div>

                                    <div id="noAlertMsg" runat="server" style="width: 69%; float: right">
                                    </div>
                                </div>

                                <div id="Alerts" runat="server" style="display: none; height: 100%; clear: both">
                                    <div>
                                        <div>
                                            <asp:Label runat="server" ID="lblPanelTitle" class="Alerts_Description" Text="Hay alertas que requieren de su atención. Pulse en el detalle para gestionarlas"></asp:Label>
                                        </div>
                                        <div style="" runat="server" id="SubDescription">
                                            <asp:Label runat="server" ID="lblPanelSubTitle" class="Alerts_SubDescription" Text="La gestión de las alertas infromadas conlleva un cálculo interno que prodría dar lugar a una relentización de VisualTime, por lo que el detalle de las alertas no se actualizará de forma automática hasta que el cálculo haya finalizado."></asp:Label>
                                        </div>
                                    </div>
                                    <div id="userAlerts" runat="server" style="clear: both">
                                        <div class="mainBlock" style="width: 20%; float: left">
                                            <div class="mainCentered">
                                                <div class="alertBlock redCircle">
                                                    <div id="userAlertsCount" runat="server" class="centered">0</div>
                                                </div>
                                            </div>
                                        </div>

                                        <div style="width: 79%; float: right" class="contentBlock">
                                            <div class="contentCentered" id="userAlertsContent" runat="server"></div>
                                        </div>
                                    </div>

                                    <div id="systemAlerts" runat="server" style="clear: both; padding-top: 10px;">
                                        <div class="mainBlock" style="width: 20%; float: left">
                                            <div class="mainCentered">
                                                <div class="alertBlock yellowCircle">
                                                    <div id="systemAlertsCount" runat="server" class="centered">0</div>
                                                </div>
                                            </div>
                                        </div>

                                        <div style="width: 79%; float: right" class="contentBlock">
                                            <div class="contentCentered" id="systemAlertsContent" runat="server"></div>
                                        </div>
                                    </div>
                                </div>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxCallbackPanel>
                </div>
            </div>
        </div>
    </div>
    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="AlertDetailsPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Alerts/AlertsDetails.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" MinWidth="870px" Width="870px" MinHeight="370px" Height="370px" CssClass="bodyPopupExtended overwriteBodyPE"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="AlertDetailsPopup_Client" PopupAnimationType="None" ContentStyle-Paddings-Padding="0px" ShowShadow="false">
        <ClientSideEvents Shown="function(s,e){ s.SetWidth(870); }" />
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>
</asp:Content>