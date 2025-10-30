<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.EmployeeRemoveData" EnableEventValidation="false" EnableViewState="True" CodeBehind="EmployeeRemoveData.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edición autorizaciones</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">
    <form id="frmEmployeeEditMobility" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {
                ConvertControls('');
                venableOPC('ctl00_frmEmployeeEditmobility_optRemoveEmployeePhoto');
                venableOPC('ctl00_frmEmployeeEditmobility_optRemoveBiometricData');
                venableOPC('ctl00_frmEmployeeEditmobility_optRemovePunchPhoto');
            }

            var monitor = -1;

            function showCaptcha() {
                var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=REMOVEEMPLOYEEDATA";
                CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                CaptchaObjectPopup_Client.Show();
            }

            function captchaCallback(action) {
                switch (action) {
                    case "REMOVEEMPLOYEEDATA":
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
                    clearInterval(monitor);
                    AspxLoadingPopup_Client.Hide();

                    if (s.cpActionResult == "OK") {
                        window.parent.frames["ifPrincipal"].showErrorPopup("RemoveEmployeeData.OK", "ERROR", "RemoveEmployeeData.OKDesc", "Error.OK", "Error.OKDesc", "window.parent.frames['ifPrincipal'].refreshEmployeeTree(true);Close();");
                    } else {
                        window.parent.frames["ifPrincipal"].showErrorPopup("RemoveEmployeeData.Error", "ERROR", "RemoveEmployeeData.ErrorDesc", "Error.Ok", "Error.OKDesc", "Close();");
                    }
                }
            }
        </script>

        <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>
        <div>

            <table style="width: 100%;" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" height="20px" valign="top">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="lblTitle" runat="server" Text="Borrado de datos de empleado"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="~/Employees/Images/employee_removeData.png" alt="" runat="server" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Seleccione los datos que desea borrar del empleado. Los datos serán borrados de forma definitiva y no se podrán recuperar."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <roUserControls:roOptionPanelClient ID="optRemoveEmployeePhoto" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="">
                            <Title>
                                <asp:Label ID="lbloptRemoveEmployeePhoto" runat="server" Text="Foto de empleado."></asp:Label>
                            </Title>
                            <Description>
                                <asp:Label ID="lbloptRemoveEmployeePhotoDesc" runat="server" Text="Se borrará la foto de empleado que se utiliza en los distintos portales."></asp:Label>
                            </Description>
                            <Content>
                            </Content>
                        </roUserControls:roOptionPanelClient>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <roUserControls:roOptionPanelClient ID="optRemoveBiometricData" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="">
                            <Title>
                                <asp:Label ID="lbloptRemoveBiometricData" runat="server" Text="Huellas del empleado."></asp:Label>
                            </Title>
                            <Description>
                                <asp:Label ID="lbloptRemoveBiometricDataDesc" runat="server" Text="Se borrará las huellas del empleado del servidor y todos los terminales."></asp:Label>
                            </Description>
                            <Content>
                            </Content>
                        </roUserControls:roOptionPanelClient>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <roUserControls:roOptionPanelClient ID="optRemovePunchPhoto" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="">
                            <Title>
                                <asp:Label ID="lbloptRemovePunchPhoto" runat="server" Text="Fotos de fichajes."></asp:Label>
                            </Title>
                            <Description>
                                <asp:Label ID="lbloptRemovePunchPhotoDesc" runat="server" Text="Se borrarán todas las fotos asociadas a los fichajes del empleado."></asp:Label>
                            </Description>
                            <Content>
                            </Content>
                        </roUserControls:roOptionPanelClient>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2" style="padding-right: 5px;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <div style="height: 145px">
                                    <table style="position: relative; top: 100px;">
                                        <tr>
                                            <td>
                                                <asp:Button ID="btEnd" Text="${Button.End}" runat="server" OnClientClick="PerformValidation();return false;" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                        </tr>
                                    </table>
                                </div>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>

        <!-- POPUP NEW OBJECT -->
        <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />
        </dx:ASPxPopupControl>

        <!-- POPUP NEW OBJECT -->
        <dx:ASPxPopupControl ID="AspxLoadingPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/PerformingAction.aspx"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="460" Height="260"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="AspxLoadingPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />
        </dx:ASPxPopupControl>
    </form>
</body>
</html>