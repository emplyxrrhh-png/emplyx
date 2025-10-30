<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Security_Wizards_PassportSecurityActions" EnableEventValidation="false" Culture="auto" UICulture="auto" CodeBehind="PassportSecurityActions.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para modificar parámetros de seguridad</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmSecurityActions" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                var bolLoaded = false;

                async function PageBase_Load() {
                    if (!bolLoaded) {
                        await getroTreeState('objContainerTreeV3_treeEmpFilterPlates').then(roState => roState.reset());
                        await getroTreeState('objContainerTreeV3_treeEmpFilterPlatesGrid').then(roState => roState.reset());
                        bolLoaded = true;
                    }

                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);

                    checkOPCPanelClients();

                    //Enllaç dels OptionPanelClients
                    linkOPCItems('<%= opResetPassword.ClientID %>,<%= opSendUsername.ClientID %>,<%= opLockAccount.ClientID %>,<%= opUnlockAccount.ClientID %>,<%= opApplications.ClientID%>,<%= opPermissions.ClientID%>,<%= opSendPin.ClientID%>');

                    if (!bolLoaded) bolLoaded = true;
                }

                function checkOPCPanelClients() {
                    venableOPC('<%= opResetPassword.ClientID %>');
                    venableOPC('<%= opSendUsername.ClientID %>');
                    venableOPC('<%= opLockAccount.ClientID %>');
                    venableOPC('<%= opUnlockAccount.ClientID %>');
                    venableOPC('<%= opApplications.ClientID%>');
                    venableOPC('<%= opPermissions.ClientID%>');
                    venableOPC('<%= opSendPin.ClientID%>');
                }

                //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
                function endRequestHandler() {
                    checkOPCPanelClients();
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    }
                }

                function hidePopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    }
                }

                function CheckFrame() {
                    var bolRet = true;
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;

                    if (CheckConvertControls('divStep' + oActiveFrameIndex) == false) {
                        bolRet = false;
                    } else {
                        bolRet = true;
                    }
                    if (!bolRet) hidePopupLoader();
                    return bolRet;
                }

                function UserSelected(Nodo) {
                    var hdnSelected = document.getElementById('hdnUsersSelected');
                    hdnSelected.value = Nodo.id;
                }

                function EmployeesSelected(oParm1, oParm2, oParm3) {
                    var hdnSelected = document.getElementById('hdnEmployeesSelected');
                    hdnSelected.value = oParm1;
                }

                var monitor = -1;

                function showCaptcha() {
                    var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=SECURITYACTION";
                    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                    CaptchaObjectPopup_Client.Show();
                }

                function captchaCallback(action) {
                    switch (action) {
                        case "SECURITYACTION":
                            AspxLoadingPopup_Client.Show();
                            PerformAction();
                            break;
                        case "ERROR":
                            window.parent.frames["ifPrincipal"].showErrorPopup2("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "", "");
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
                        }
                        else {
                            window.parent.frames["ifPrincipal"].showErrorPopup2("Error.ValidationFailed", "ERROR", "Error.IncorrectData", "Error.OK", "", "");
                        }

                    } else if (s.cpAction == "PERFORM_ACTION") {
                        monitor = setInterval(function () { PerformActionCallbackClient.PerformCallback("CHECKPROGRESS"); }, 5000);
                    } else if (s.cpAction == "ERROR") {
                        clearInterval(monitor);
                        AspxLoadingPopup_Client.Hide();
                        __doPostBack('<%= btResume.ClientID%>', '');
                    } else if (s.cpAction == "CHECKPROGRESS") {
                        if (s.cpActionResult == "OK") {
                            clearInterval(monitor);
                            AspxLoadingPopup_Client.Hide();
                            __doPostBack('<%= btResume.ClientID%>', '');
                        }
                    }
                }
            </script>

            <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>

            <div class="popupWizardContent">

                <!-- Welcome -->
                <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="divStep0" runat="server" style="display: block; width: 500px;">
                            <table id="tbStep0" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td style="height: 510px">
                                        <asp:Image ID="imgSecurityActionWizard" runat="server" Height="530px" Width="180px" ImageUrl="~/Base/Images/Wizards/wizard.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">
                                        <asp:Label ID="lblWelcomeEmployees" Style="display: block;" runat="server" Text="Bienvenido al asistente para realizar acciones o modificar parámetros de seguridad de usuarios. de ${Employees}." Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <!-- <asp:Label ID="lblWelcomeUsers" style="display:block;" runat="server" Text="Bienvenido al asistente para modificar parámetros de seguridad de usuarios." Font-Bold="True" Font-Size="Large"></asp:Label> -->
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le ayudará a realizar acciones y modificar parámetros relacionados con permisos de acceso a la aplicación." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="PassportWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <!-- Selector de Empleados o Usuarios -->
                <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="divStep1" runat="server" style="display: none;">
                            <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep2InfoEmployees" runat="server" Text="Seleccione los ${Employees} que desea tratar." />
                                                    <!-- <asp:Label ID="lblStep2InfoUsers" runat="server" Text="Seleccione los usuarios que desea tratar." /> -->
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <table style="margin-top: 10px; margin-left: auto; margin-right: auto;">
                                            <tr>
                                                <td>
                                                    <asp:HiddenField ID="hdnEmployeesSelected" runat="server" Value="" />
                                                    <iframe id="ifEmployeesSelector" runat="server" style="background-color: Transparent;" height="440" width="475" scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:UpdatePanel ID="updStep2" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="divStep2" runat="server" style="display: none;">
                            <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="Seleccione la acción a realizar sobre la selección anterior." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px"></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <table width="100%" style="margin-top: 10px;">
                                            <tr>
                                                <td>
                                                    <table style="width: 100%;">
                                                        <tr>
                                                            <td style="padding-left: 5px;">
                                                                <roUserControls:roOptionPanelClient ID="opResetPassword" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True">
                                                                    <Title>
                                                                        <asp:Label ID="lbloptResetPassword" runat="server" Text="Regenerar contraseña"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lbloptResetPasswordDesc" runat="server" Text="Se enviará un mensaje al usuario y en el momento de validarse se le pedirá que cambie la contraseña."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                    </Content>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-left: 5px;">
                                                                <roUserControls:roOptionPanelClient ID="opSendUsername" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True">
                                                                    <Title>
                                                                        <asp:Label ID="lbloptSendUsername" runat="server" Text="Enviar usuario"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lbloptSendUsernameDesc" runat="server" Text="Se enviará un mail al usuario con su nombre de usuario."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                    </Content>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-left: 5px; padding-top: 5px">
                                                                <roUserControls:roOptionPanelClient ID="opLockAccount" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True">
                                                                    <Title>
                                                                        <asp:Label ID="lbloptLockAccount" runat="server" Text="Bloquear cuentas"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lbloptLockAccountDesc" runat="server" Text="La cuenta asociada se bloqueará para denegar el acceso a la aplicación."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                    </Content>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-left: 5px; padding-top: 5px">
                                                                <roUserControls:roOptionPanelClient ID="opUnlockAccount" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True">
                                                                    <Title>
                                                                        <asp:Label ID="lbloptUnlockAccount" runat="server" Text="Desbloquear cuentas"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lbloptUnlockAccountDesc" runat="server" Text="La cuenta asociada se desbloqueará para permitir el acceso a la aplicación."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                    </Content>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-left: 5px; padding-top: 5px">
                                                                <roUserControls:roOptionPanelClient ID="opApplications" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True">
                                                                    <Title>
                                                                        <asp:Label ID="lbloptConfigureApps" runat="server" Text="Configurar aplicaciones"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lbloptConfigureAppsDesc" runat="server" Text="Se configuraran las mismas aplicaciones y opciones que el empleado seleccionado."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                        <div style="padding-left: 20px">
                                                                            <dx:ASPxComboBox ID="cmbEmployeeApplications" runat="server" Width="400px" ClientInstanceName="cmbEmployeeApplicationsClient">
                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                            </dx:ASPxComboBox>
                                                                        </div>
                                                                    </Content>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-left: 5px; padding-top: 5px">
                                                                <roUserControls:roOptionPanelClient ID="opPermissions" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True">
                                                                    <Title>
                                                                        <asp:Label ID="lbloptCopyPermissions" runat="server" Text="Copiar permisos"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lbloptCopyPermissionsDesc" runat="server" Text="Se asignaran los mismos permisos que el empleado seleccionado."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                        <div style="padding-left: 20px">
                                                                            <dx:ASPxComboBox ID="cmbEmployeePermissions" runat="server" Width="400px" ClientInstanceName="cmbEmployeePermissionsClient">
                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                            </dx:ASPxComboBox>
                                                                        </div>
                                                                    </Content>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-left: 5px; padding-top: 5px">
                                                                <roUserControls:roOptionPanelClient ID="opSendPin" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True">
                                                                    <Title>
                                                                        <asp:Label ID="lbloptSendPin" runat="server" Text="Crear PIN de acceso a terminales"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lbloptSendPinDesc" runat="server" Text="Se sobre escribirán los PINs existentes para el grupo seleccionado y se enviarán por email a los usuarios."></asp:Label>
                                                                    </Description>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>

                                        <roUserControls:roOptPanelClientGroup ID="optGroup" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="popupWizardButtons">
                <asp:UpdatePanel ID="updButtons" runat="server" RenderMode="Inline">
                    <ContentTemplate>
                        <table align="right" cellpadding="0" cellspacing="0">
                            <tr class="PassportWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();return CheckFrame();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" Visible="false" TabIndex="4" OnClientClick="PerformValidation();return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btResume" Text="" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
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
        <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
    </form>
</body>
</html>