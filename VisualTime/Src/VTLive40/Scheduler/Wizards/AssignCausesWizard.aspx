<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_AssignCausesWizard" Culture="auto" UICulture="auto" EnableEventValidation="false" CodeBehind="AssignCausesWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para introducción masiva de justificaciones</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmSopyScheduleWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                var bolLoaded = false;

                async function PageBase_Load() {
                    if (!bolLoaded) {
                        await getroTreeState('objContainerTreeV3_treeEmpAssignCausesWizard').then(roState => roState.reset());
                        await getroTreeState('objContainerTreeV3_treeEmpAssignCausesWizardGrid').then(roState => roState.reset());
                    }

                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);
                    linkOPCItems('<%= optCompletedDays.ClientID%>,<%= optPartialDays.ClientID%>');
                    checkOPCPanelClients();

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

                function checkOPCPanelClients() {
                    venableOPC('<%= optCompletedDays.ClientID%>');
                    venableOPC('<%= optPartialDays.ClientID%>');

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

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(requestEndHandler);
                //Funcion que captura el requestEndHandler
                function requestEndHandler(sender, args) {
                    checkOPCPanelClients();
                    if (args.get_error()) {
                        if (args.get_error().name == "Sys.WebForms.PageRequestManagerTimeoutException") {
                            args.set_errorHandled(true);
                            alert("Tiempo maxímo de espera alcanzado. El servidor seguirá procesando.");
                        }
                    }
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

                var monitor = -1;

                function showCaptcha() {
                    var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=MASSASSIGN";
                    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                    CaptchaObjectPopup_Client.Show();
                }

                function captchaCallback(action) {
                    switch (action) {
                        case "MASSASSIGN":
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
                    if (s.cpAction == "VALIDATE" && s.cpResult == true) {
                        showCaptcha();
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

            <asp:UpdatePanel ID="upAssignCausesWizard" runat="server">
                <ContentTemplate>
                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgAssignCausesWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzschedule.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblCopyScheduleWelcome1" runat="server" Text="Bienvenido al asistente para la introducción masiva de justificaciones."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblCopyScheduleWelcome2" runat="server" Text="El asistente le permite justificar de forma masiva a empleados en un periodo concreto." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblCopyScheduleWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="SchedulerWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>

                        <asp:Label ID="hdnStepTitle" Text="Asistente para la introducción masiva de justificaciones." runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none">
                            <table id="tbStep1" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                            </table>
                        </div>

                        <div id="DivStep2" runat="server" style="display: none">
                            <table id="btStep2" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="SchedulerWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="Paso 2 de 4." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep2Info" runat="server" Text="Ahora seleccione los empleados a los que quiere justificar algún día" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepContent">
                                        <asp:Label ID="lblStep2Info2" runat="server" Text="Ahora seleccione los empleados a los que quiere justificar algún día. " /><br />
                                        <br />
                                        <input type="hidden" id="hdnEmployeesSelected" runat="server" value="" />
                                        <input type="hidden" id="hdnFilter" runat="server" value="" />
                                        <input type="hidden" id="hdnFilterUser" runat="server" value="" />
                                        <iframe id="ifEmployeeSelector" runat="server" style="background-color: Transparent" height="290px" width="100%"
                                            scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep3" runat="server" style="display: none">
                            <table id="tbStep3" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="SchedulerWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep3Title" runat="server" Text="Paso 3 de 4." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep3Info" runat="server" Text="Seleccione la forma de justificar y el periodo donde desea que se aplique." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepContent" valign="middle">

                                        <asp:Label ID="lblStep3Info2" runat="server" Text="Indique qué quiere justificar y el periodo donde desea que se aplique." />
                                        <br />

                                        <table>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 10px">
                                                    <roUserControls:roOptionPanelClient ID="optCompletedDays" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto">
                                                        <Title>
                                                            <asp:Label ID="lblCompletedDaysTitle" runat="server" Text="Días con ausencias completas"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <asp:Label ID="lblCompletedDaysDescription" runat="server" Text="Se justificarán aquellos días con ausencias completas entre las fechas seleccionadas."></asp:Label>
                                                        </Description>
                                                        <Content>
                                                            <table>
                                                                <tr>
                                                                    <td style="padding-left: 10px; padding-top: 20px">
                                                                        <asp:Label ID="lblBeginDate" runat="server" Text="Desde el día"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 10px; padding-top: 20px">
                                                                        <dx:ASPxDateEdit runat="server" ID="txtBeginDate" PopupVerticalAlign="WindowCenter" Width="150" ClientInstanceName="txtBeginDateClient" AllowNull="false">
                                                                            <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="padding-left: 10px; padding-top: 5px">
                                                                        <asp:Label ID="lblEndDate" runat="server" Text="hasta el día"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 10px; padding-top: 5px">
                                                                        <dx:ASPxDateEdit runat="server" ID="txtEndDate" PopupVerticalAlign="WindowCenter" Width="150" ClientInstanceName="txtEndDateClient" AllowNull="false">
                                                                            <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 10px">
                                                    <roUserControls:roOptionPanelClient ID="optPartialDays" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto">
                                                        <Title>
                                                            <asp:Label ID="lblPartialDaysTitle" runat="server" Text="Fichajes"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <asp:Label ID="lblPartialDaysDescription" runat="server" Text="Se justificarán aquellos fichajes de entrada y salida que se hayan producido entre las fechas y horas seleccionadas."></asp:Label>
                                                        </Description>
                                                        <Content>
                                                            <table>
                                                                <tr>
                                                                    <td style="padding-left: 10px; padding-top: 20px">
                                                                        <asp:Label ID="lblBeginDatePartial" runat="server" Text="Desde el día"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 10px; padding-top: 20px">
                                                                        <dx:ASPxDateEdit runat="server" ID="txtBeginDatePartial" PopupVerticalAlign="WindowCenter" Width="150" ClientInstanceName="txtBeginDatePartialClient" AllowNull="false">
                                                                            <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                    <td style="padding-left: 10px; padding-top: 20px">
                                                                        <asp:Label ID="lblBeginDateDescPartial" runat="server" Text=" a las"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 10px; padding-top: 20px">
                                                                        <dx:ASPxTimeEdit ID="txtBeginTime" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtBeginTimeClient"></dx:ASPxTimeEdit>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="padding-left: 10px; padding-top: 5px">
                                                                        <asp:Label ID="lblEndDatePartial" runat="server" Text="hasta el día"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 10px; padding-top: 5px">
                                                                        <dx:ASPxDateEdit runat="server" ID="txtEndDatePartial" PopupVerticalAlign="WindowCenter" Width="150" ClientInstanceName="txtEndDatePartialClient" AllowNull="false">
                                                                            <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                    <td style="padding-left: 10px; padding-top: 5px">
                                                                        <asp:Label ID="lblEndDateDescPartial" runat="server" Text=" a las"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 10px; padding-top: 5px">
                                                                        <dx:ASPxTimeEdit ID="txtEndTime" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtEndTimeClient"></dx:ASPxTimeEdit>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep4" runat="server" style="display: none">
                            <table id="tbStep4" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="SchedulerWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep4Title" runat="server" Text="Paso 4 de 4." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep4Info" runat="server" Text="Finalmente, seleccione la justificación a utilizar." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep4Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepContent">
                                        <asp:Label ID="lblStep4Info2" runat="server" Text="Finalmente seleccione qué justificación desea utilizar. " />
                                        <br />
                                        <table>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <asp:Label ID="lblCause" runat="server" Text="Justificación: "></asp:Label>
                                                </td>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <dx:ASPxComboBox ID="cmbCause" runat="server" ClientInstanceName="cmbCauseClient" Width="250px">
                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                    </dx:ASPxComboBox>
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