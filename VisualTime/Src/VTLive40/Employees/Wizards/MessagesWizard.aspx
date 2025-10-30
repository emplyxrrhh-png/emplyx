<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_MessagesWizard" Culture="auto" UICulture="auto" EnableEventValidation="false" CodeBehind="MessagesWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para el envío de mensajes a empleados</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmSopyScheduleWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>

            <script language="javascript" type="text/javascript">

                var bolLoaded = false;

                async function PageBase_Load() {
                    if (!bolLoaded) {
                        await getroTreeState('objContainerTreeV3_treeEmpIncidencesWizard').then(roState => roState.reset());
                        await getroTreeState('objContainerTreeV3_treeEmpIncidencesWizardGrid').then(roState => roState.reset());
                    }

                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);

                    venableOPC('chkSendOnNextPunch');
                    venableOPC('chkSendOnDate');
                    linkOPCItems('chkSendOnNextPunch,chkSendOnDate');

                    cmbUserFieldClient.SetEnabled(false);

                    if (!bolLoaded) bolLoaded = true;
                }

                function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
                    var hdnEmployeesSelected = document.getElementById('<%= me.hdnEmployeesSelected.ClientID %>');
                    hdnEmployeesSelected.value = oParm1;

                    var hdnFilter = document.getElementById('<%= me.hdnFilter.ClientID %>');
                    hdnFilter.value = oParm2;

                    var hdnFilterUser = document.getElementById('<%= me.hdnFilterUser.ClientID %>');
                    hdnFilterUser.value = oParm3;
                }

                function CheckFrame() {
                    var bolRet = true;
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;

                    if (CheckConvertControls('divStep' + oActiveFrameIndex) == false) {
                        bolRet = false;
                    }
                    else {
                        bolRet = true;
                    }

                    if (!bolRet) hidePopupLoader();
                    return bolRet;
                }

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(requestEndHandler);
                //Funcion que captura el requestEndHandler
                function requestEndHandler(sender, args) {
                    if (args.get_error()) {
                        if (args.get_error().name == "Sys.WebForms.PageRequestManagerTimeoutException") {
                            args.set_errorHandled(true);
                            alert("Tiempo maxímo de espera alcanzado. El servidor seguirá procesando.");
                        }
                    }
                }

                //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
                function endRequestHandler() {
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                }

                function hidePopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                }

                var monitor = -1;

                function showCaptcha() {
                    var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=MASS_SEND";
                    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                    CaptchaObjectPopup_Client.Show();
                }

                function captchaCallback(action) {
                    switch (action) {
                        case "MASS_SEND":
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
                        }
                        else {
                            window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", s.cpErrorMessageKey, "Error.OK", "Error.OKDesc", "");
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

                function changeEnabledOptions() {
                    if (document.getElementById('chkSendOnNextPunch_rButton').checked == true) {
                        ckFixedValue_client.SetEnabled(false);
                        ckUserField_client.SetEnabled(false);
                        ckSendOnPeriod_client.SetEnabled(false);
                        txtOnFixedDateClient.SetEnabled(false);
                        cmbUserFieldClient.SetEnabled(false);
                        txtInitPeriod_Client.SetEnabled(false);
                        txtEndPeriod_Client.SetEnabled(false);
                    } else {
                        ckFixedValue_client.SetEnabled(true);
                        ckUserField_client.SetEnabled(true);
                        ckSendOnPeriod_client.SetEnabled(true);

                        if (ckFixedValue_client.GetValue() == true) txtOnFixedDateClient.SetEnabled(true);
                        if (ckUserField_client.GetValue() == true) cmbUserFieldClient.SetEnabled(true);

                        if (ckSendOnPeriod_client.GetValue() == true) {
                            txtInitPeriod_Client.SetEnabled(false);
                            txtEndPeriod_Client.SetEnabled(false);
                        }
                    }
                }

                function ckFixedValue_client_CheckedChanged(s, e) {
                    if (s.GetValue() == true) {
                        txtOnFixedDateClient.SetEnabled(true);
                        cmbUserFieldClient.SetValue('');
                    } else {
                        txtOnFixedDateClient.SetEnabled(false);
                    }
                }

                function ckUserField_client_CheckedChanged(s, e) {
                    if (s.GetValue() == true) {
                        cmbUserFieldClient.SetEnabled(true);
                    } else {
                        cmbUserFieldClient.SetEnabled(false);
                    }
                }

                function ckSendOnPeriod_client_CheckedChanged(s, e) {
                    if (s.GetValue() == true) {
                        txtInitPeriod_Client.SetEnabled(true);
                        txtEndPeriod_Client.SetEnabled(true);
                    } else {
                        txtInitPeriod_Client.SetEnabled(false);
                        txtEndPeriod_Client.SetEnabled(false);
                    }
                }
            </script>

            <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>

            <asp:UpdatePanel ID="upIncidencesWizard" runat="server">
                <ContentTemplate>
                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 420px">
                                        <asp:Image ID="imgMessagesWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzmens.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para el envío de mensajes para empleados."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="El asistente le permite crear mensajes que serán leídos por los empleados en los terminales o el portal del empleado." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="NewEmployeeWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>

                        <asp:Label ID="hdnStepTitle" Text="Asistente para el envío de mensajes." runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none">
                            <table id="tbStep1" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                            </table>
                        </div>

                        <div id="DivStep2" runat="server" style="display: none">
                            <table id="btStep2" style="width: 800px;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewEmployeeWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="Paso 2 de 3." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep2Info" runat="server" Text=" " />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewEmployeeWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewEmployeeWizards_StepContent">
                                        <asp:Label ID="lblStep2Info2" runat="server" Text="Ahora seleccione los empleados a los que quiere enviar el mensaje. " /><br />
                                        <br />
                                        <input type="hidden" id="hdnEmployeesSelected" runat="server" value="" />
                                        <input type="hidden" id="hdnFilter" runat="server" value="" />
                                        <input type="hidden" id="hdnFilterUser" runat="server" value="" />
                                        <iframe id="ifEmployeeSelector" runat="server" style="background-color: Transparent" height="350px" width="100%"
                                            scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep3" runat="server" style="display: none">
                            <table id="tbStep3" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewEmployeeWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep3Title" runat="server" Text="Paso 3 de 3." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep3Info" runat="server" Text=" " />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewEmployeeWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewEmployeeWizards_StepContent" valign="middle">

                                        <asp:Label ID="lblStep3Info2" runat="server" Text="Introduzca el contenido del mensaje a enviar. Este solo puede tener una longitud máxima de 64 carácteres." />
                                        <br />

                                        <table style="width: 100%; padding-top: 20px;">
                                            <tr>
                                                <td colspan="2">
                                                    <div>
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label runat="server" ID="lblMessageToSend" Text="Mensaje a enviar"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div class="panBottomMargin">
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <td style="padding-left: 10px; width: 10%;">
                                                                    <asp:Label ID="lblMessage" runat="server" Text="Mensaje"></asp:Label>
                                                                </td>
                                                                <td style="padding-left: 10px;">
                                                                    <dx:ASPxTextBox ID="txtMessage" runat="server" MaxLength="64" Width="100%" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label runat="server" ID="lblScheduleMessage" Text="Planificar mensaje"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div style="vertical-align: top;">
                                                        <roUserControls:roOptionPanelClient ID="chkSendOnNextPunch" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="changeEnabledOptions()">
                                                            <Title>
                                                                <asp:Label ID="chkSendOnNextPunchTitle" runat="server" Text="Inmediatamente"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="chkSendOnNextPunchDesc" runat="server" Text="El menseja se enviará en el siguiente fichaje que realice el empleado en un terminal compatible"></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </div>
                                                    <div style="vertical-align: top;">
                                                        <roUserControls:roOptionPanelClient ID="chkSendOnDate" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="changeEnabledOptions()">
                                                            <Title>
                                                                <asp:Label ID="chkSendOnDateTitle" runat="server" Text="Programar envío"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="chkSendOnDateDesc" runat="server" Text="El menseja se enviará en el siguiente fichaje que realice el empleado en la fecha especificada y en un terminal compatible"></asp:Label>
                                                            </Description>
                                                            <Content>
                                                                <table style="padding-left: 20px; width: 100%">
                                                                    <tr>
                                                                        <td>
                                                                            <div style="clear: both; padding-bottom: 10px;">
                                                                                <div style="float: left; width: 40%">
                                                                                    <dx:ASPxRadioButton GroupName="OnDateGroup" ID="ckFixedValue" runat="server" ClientInstanceName="ckFixedValue_client" Text="Fecha fija">
                                                                                        <ClientSideEvents CheckedChanged="ckFixedValue_client_CheckedChanged" />
                                                                                    </dx:ASPxRadioButton>
                                                                                </div>
                                                                                <div style="float: right; width: calc(60% - 24px)">
                                                                                    <dx:ASPxDateEdit runat="server" ID="txtOnFixedDate" PopupVerticalAlign="WindowCenter" DisplayFormatString="dd/MM" EditFormat="Custom" EditFormatString="dd/MM" Width="150" ClientInstanceName="txtOnFixedDateClient">
                                                                                    </dx:ASPxDateEdit>
                                                                                </div>
                                                                                <div style="float: left; width: 40%; clear: both">
                                                                                    <dx:ASPxRadioButton GroupName="OnDateGroup" ID="ckUserField" runat="server" ClientInstanceName="ckUserField_client" Text="Según campo de la ficha">
                                                                                        <ClientSideEvents CheckedChanged="ckUserField_client_CheckedChanged" />
                                                                                    </dx:ASPxRadioButton>
                                                                                </div>
                                                                                <div style="float: right; width: calc(60% - 24px)">
                                                                                    <dx:ASPxComboBox ID="cmbUserField" runat="server" ClientInstanceName="cmbUserFieldClient" Width="200px">
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                    </dx:ASPxComboBox>
                                                                                </div>

                                                                                <div style="float: left; width: 40%; clear: both">
                                                                                    <dx:ASPxCheckBox ID="ckSendOnPeriod" runat="server" Text="Solo enviar en el intervalo" ClientInstanceName="ckSendOnPeriod_client">
                                                                                        <ClientSideEvents CheckedChanged="ckSendOnPeriod_client_CheckedChanged" />
                                                                                    </dx:ASPxCheckBox>
                                                                                </div>
                                                                                <div style="float: right; width: calc(60% - 24px)">
                                                                                    <div style="float: left">
                                                                                        <dx:ASPxTimeEdit ID="txtInitPeriod" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px" ClientInstanceName="txtInitPeriod_Client">
                                                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                                                        </dx:ASPxTimeEdit>
                                                                                    </div>
                                                                                    <div style="float: left">
                                                                                        <dx:ASPxTimeEdit ID="txtEndPeriod" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px" ClientInstanceName="txtEndPeriod_Client">
                                                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                                                        </dx:ASPxTimeEdit>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div class="popupWizardButtons">
                        <table align="right" cellpadding="0" cellspacing="0">
                            <tr class="SchedulerWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();return CheckFrame();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" Visible="false" OnClientClick="PerformValidation();return false;" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btResume" Text="" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>

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
        </div>
    </form>
</body>
</html>