<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.PrintAbsences" CodeBehind="PrintAbsences.aspx.vb" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fecha de Bloqueo</title>
</head>
<body class="bodyPopup">
    <form id="frmPrintAbsences" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <script language="javascript" type="text/javascript">

            var monitor = -1;

            function LoadAbsences() {
                var oParameters = {};
                oParameters.data = 0;
                oParameters.StampParam = new Date().getMilliseconds();
                oParameters.action = "GETABSENCES";
                var strParameters = JSON.stringify(oParameters);
                strParameters = encodeURIComponent(strParameters);

                //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
                PerformActionCallbackClient.PerformCallback(strParameters);
            }

            function PerformActionCallback_CallbackComplete(s, e) {
                if (s.cpAction == "PRINTABSENCES") {
                    LoadEmployeeAbsences(s);
                } else if (s.cpAction == "EXPORT") {
                    alert('download ok')
                }

            }
        </script>
        <div style="padding-top: 10px;">
            <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>
            <asp:UpdatePanel ID="updPrintAbsences" runat="server">
                <ContentTemplate>
                    <div>
                        <table style="width: 100%" cellspacing="0" class="bodyPopup">
                            <tr style="height: 20px;">
                                <td>
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label runat="server" ID="lblPrintAbsencesTitle" Text="Indique el tipo de justificación de la que extraer los detalles"></asp:Label>
                                        </span>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                    <div id="divAbsencesGrid" runat="server" class="jsGridContent dextremeGrid">
                                        <!-- Carrega del Grid Usuari General -->
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="popupWizardButtons" align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                        <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <dx:ASPxPopupControl ID="AspxLoadingPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/PerformingAction.aspx" CssClass="captchaFirst"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="460" Height="260"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="AspxLoadingPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />
        </dx:ASPxPopupControl>

        <!-- POPUP NEW OBJECT -->
        <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx" CssClass="captchaFirst"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />
        </dx:ASPxPopupControl>
        <Local:MessageFrame ID="MessageFrame1" runat="server" />
    </form>
</body>
</html>