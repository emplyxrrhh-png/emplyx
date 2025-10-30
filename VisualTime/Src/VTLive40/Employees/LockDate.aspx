<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.LockDate" CodeBehind="LockDate.aspx.vb" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fecha de Bloqueo</title>
</head>
<body class="bodyPopup">
    <form id="frmLockDate" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <script language="javascript" type="text/javascript">

            var monitor = -1;

            function showCaptcha() {
                var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=SAVELOCKDATE";
                CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                CaptchaObjectPopup_Client.Show();
            }

            function captchaCallback(action) {
                switch (action) {
                    case "SAVELOCKDATE":
                        AspxLoadingPopup_Client.Show();
                        PerformAction();
                        break;
                    case "ERROR":
                        window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
                        break;
                }
            }

            function PerformValidation() {
                PerformActionCallbackClient.PerformCallback("VALIDATE");
            }

            function PerformAction() {
                PerformActionCallbackClient.PerformCallback("PERFORM_ACTION");
            }

            function PerformActionCallback_CallbackComplete(s, e) {
                if (s.cpAction == "VALIDATE") {
                    if (s.cpResult == true) {
                        showCaptcha();
                    } else {
                        window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", s.cpErrorMessageKey, "Error.OK", "Error.OKDesc", "");
                    }
                } else if (s.cpAction == "PERFORM_ACTION") {
                    monitor = setInterval(function () { PerformActionCallbackClient.PerformCallback("CHECKPROGRESS"); }, 5000);
                } else if (s.cpAction == "ERROR") {
                    clearInterval(monitor);
                    AspxLoadingPopup_Client.Hide();
                } else if (s.cpAction == "CHECKPROGRESS") {
                    if (s.cpActionResult == "OK") {
                        clearInterval(monitor);
                        AspxLoadingPopup_Client.Hide();
                        redirectToSave();
                    }
                }

            }

            function redirectToSave() {
                __doPostBack('btnSaveAndEdit', '');
            }
        </script>
        <div style="padding-top: 10px;">
            <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>
            <asp:UpdatePanel ID="updLockDate" runat="server">
                <ContentTemplate>
                    <div style="min-height: 500px;">
                        <table style="width: 100%" cellspacing="0" class="bodyPopup">
                            <tr style="height: 20px;">
                                <td>
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label runat="server" ID="lblLockDateTitle" Text="Indique qué fecha determina la fecha de bloqueo del empleado"></asp:Label>
                                        </span>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <roUserControls:roGroupBox ID="roLockDateGroup" runat="server">
                                        <Content>
                                            <div class="divRow" style="margin-left: 5px;">
                                                <div class="componentFormWithoutSize">
                                                    <div class="panBottomMargin">
                                                        <div style="clear: both">
                                                            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; padding-bottom: 5px;">
                                                                <tbody>
                                                                    <tr>
                                                                        <td>
                                                                            <dx:ASPxLabel runat="server" ID="lblLockDate" Text="Fecha de bloqueo" CssClass="OptionPanelCheckBoxStyle" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <dx:ASPxLabel runat="server" ID="lblLockDateDescription" CssClass="OptionPanelDescStyle" Text="Fecha que determinará el bloqueo establecido en el empleado" Style="width: 100%; padding-left: 0px;" />
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="divRow">
                                                <div class="componentFormWithoutSize">
                                                    <div class="panBottomMargin">
                                                        <div style="clear: both">
                                                            <div style="float: left">
                                                                <dx:ASPxRadioButton GroupName="gLD" Text="Fecha de bloqueo general" runat="server" ID="rbLDG" ClientInstanceName="rbLDGClient" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="componentFormWithoutSize">
                                                    <div class="panBottomMargin">
                                                        <div style="clear: both">
                                                            <div style="float: left; padding-top: 5px">
                                                                <dx:ASPxRadioButton GroupName="gLD" Text="Fecha de bloqueo específica para el empleado" runat="server" ID="rbLDS" ClientInstanceName="rbLDSClient" />
                                                            </div>
                                                            <div style="float: left; padding-top: 8px; padding-left: 3px;">
                                                                <dx:ASPxDateEdit runat="server" PopupVerticalAlign="WindowCenter" EditFormat="Date" ID="txtLockDateSpecific" Width="150px" ClientInstanceName="txtLockDateSpecificClient">
                                                                </dx:ASPxDateEdit>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </Content>
                                    </roUserControls:roGroupBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="popupWizardButtons" align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnAccept" Text="${Button.Accept}" runat="server" OnClientClick="PerformValidation();return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                <asp:Button ID="btnSaveAndEdit" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" Visible="false" />
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