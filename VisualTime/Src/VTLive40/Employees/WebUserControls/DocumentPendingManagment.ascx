<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DocumentPendingManagment.ascx.vb" Inherits="VTLive40.DocumentPendingManagment" %>

<script type="text/javascript">
    <%--function <%= Me.ClientID %>__CallbackSessionDocPenClient_BeginCallback(e, c) {
    }--%>

    function <%= Me.ClientID %>_CallbackSessionDocPenClient_CallbackComplete(s, e) {

    }
</script>

<dx:ASPxCallback ID="CallbackSessionDocPen" runat="server"></dx:ASPxCallback>
<dx:ASPxHiddenField ID="hdnScopePendingInfo" runat="server"></dx:ASPxHiddenField>

<dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" Height="100%">
    <SettingsLoadingPanel Enabled="false" />
    <PanelCollection>
        <dx:PanelContent ID="PanelContent1" runat="server">

            <div id="NoAlerts" runat="server" style="display: none;">
                <div class="mainBlock" style="width: 20%; float: left; height: 200px !important" runat="server" id="noAlertCount">
                    <div class="mainCentered">
                        <div class="alertBlock greenCircle">
                            <div style="font-size: 5em; margin-left: -5px;" class="centered">OK</div>
                        </div>
                    </div>
                </div>

                <div id="noAlertMsg" runat="server">
                </div>
            </div>

            <div id="Alerts" runat="server" style="display: none; height: 100%; clear: both">
                <div id="alertsTitle" runat="server">
                    <div>
                        <asp:Label runat="server" ID="lblDocumentAlertsTitle" class="Alerts_Description" Text="El documentos que requieren su atención. Pulse en el detalle para gestionarlas"></asp:Label>
                    </div>
                    <div style="" runat="server" id="SubDescription">
                        <asp:Label runat="server" ID="lblDocumentAlertsDesc" class="Alerts_SubDescription" Text="El empleado puede tener documentos pendientes de validar o que hayan expirado. Aquí también podrá ver las alertas de los documentos que falten de Gestión Proactiva de Absentismo como los documentos obligatorios no presentados"></asp:Label>
                    </div>
                </div>

                <div id="userAlerts" runat="server" style="clear: both">
                    <div class="mainBlock" style="width: 20%; float: left; height: 200px !important" runat="server" id="alertCount">
                        <div class="mainCentered">
                            <div class="alertBlock redCircle">
                                <div id="userAlertsCount" runat="server" style="font-size: 5em; margin-left: -5px;" class="centered">0</div>
                            </div>
                        </div>
                    </div>

                    <div class="contentBlock" runat="server" id="alertsBlock" style="width: 79%; float: right; height: 200px !important">
                        <div class="contentCentered" id="userAlertsContent" runat="server"></div>
                    </div>
                </div>
            </div>
            <div id="alertSpacing" runat="server" style="clear: both; height: 30px">
                &nbsp;
            </div>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxCallbackPanel>