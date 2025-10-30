<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_EmployeeMassPunchWizard" Culture="auto" UICulture="auto" EnableEventValidation="false" CodeBehind="EmployeeMassPunchWizard.aspx.vb" %>

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
                        await getroTreeState('objContainerTreeV3_treeEmpEmployeeMassPunchWizard').then(roState => roState.reset());
                        await getroTreeState('objContainerTreeV3_treeEmpEmployeeMassPunchWizardGrid').then(roState => roState.reset());
                    }
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);
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
                    } else {
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
                    var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=MASSASSIGNPUNCH";
                    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                    CaptchaObjectPopup_Client.Show();
                }

                function captchaCallback(action) {
                    switch (action) {
                        case "MASSASSIGNPUNCH":
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

                                        <asp:Label ID="lblCopyScheduleWelcome1" runat="server" Text="Bienvenido al asistente para la introducción masiva de fichajes."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblCopyScheduleWelcome2" runat="server" Text="El asistente le permite insertar masivamente fichajes que no se han podido realizar por fallo electrico de tornos." Font-Bold="true"></asp:Label>
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

                        <asp:Label ID="hdnStepTitle" Text="Asistente para la introducción masiva de fichajes." runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none">
                            <table id="tbStep1" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                            </table>
                        </div>

                        <div id="divStep2" runat="server" style="display: none">
                            <table id="btStep2" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="SchedulerWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="Paso 2 de 5." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep2Info" runat="server" Text="Ahora seleccione los empleados a los que quiere añadir algún fichaje" />
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
                                        <asp:Label ID="lblStep2Info2" runat="server" Text="Ahora seleccione los empleados a los que quiere justificar añadir algún fichaje. " /><br />
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
                                                    <asp:Label ID="lblStep3Title" runat="server" Text="Paso 3 de 5." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep3Info" runat="server" Text="Introduzca los datos del fichaje." />
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
                                    <td class="SchedulerWizards_StepContent">
                                        <asp:Label ID="lblStep3Info2" runat="server" Text="Indique el terminal, la hora y justificación del fichaje. La zona del fichaje viene determinada por el terminal utilizado." />
                                        <br />
                                        <table>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <asp:Label ID="lblLocationTerminal" runat="server" Text="En el terminal:"></asp:Label>&nbsp;
                                                </td>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <dx:ASPxComboBox ID="cmbTerminal" runat="server" Width="250px" Font-Size="11px" CssClass="editTextFormat"
                                                        ClientInstanceName="cmbTerminalClient" Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                        <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                    </dx:ASPxComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <asp:Label ID="Label2" runat="server" Text="Fecha del fichaje: "></asp:Label>
                                                </td>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <dx:ASPxDateEdit runat="server" ID="txtPunchDate" PopupVerticalAlign="WindowCenter" Width="250px" ClientInstanceName="txtPunchDateClient" AllowNull="false">
                                                        <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                    </dx:ASPxDateEdit>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <asp:Label ID="lblDetailsTimeCaption" runat="server" Text="Hora del fichaje:"></asp:Label>&nbsp;
                                                </td>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <dx:ASPxTextBox ID="txtDetailsTime" runat="server" Width="250px" MaskSettings-Mask="HH:mm" CssClass="editTextFormat textClass x-form-text x-form-field"
                                                        Font-Size="11px" Font-Names="Arial;Verdana;Sans-Serif" ClientInstanceName="txtDetailsTimeClient">
                                                        <MaskSettings Mask="HH:mm"></MaskSettings>
                                                        <ValidationSettings Display="None"></ValidationSettings>
                                                        <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                    </dx:ASPxTextBox>
                                                </td>
                                            </tr>
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

                        <div id="divStep4" runat="server" style="display: none">
                            <table id="tbStep4" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="SchedulerWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep4Title" runat="server" Text="Paso 4 de 5." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep4Info" runat="server" Text="Información adicional" />
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
                                        <asp:Label ID="lblStep4Info2" runat="server" Text="Indique la información adicional necesária para introducir el fichaje" /><br />

                                        <table id="tbPunchDirection" runat="server">
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <asp:Label ID="Label1" runat="server" Text="Dirección:"></asp:Label>&nbsp;
                                                </td>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <dx:ASPxComboBox ID="cmbDetailsType" runat="server" Width="250px" Font-Size="11px" CssClass="editTextFormat"
                                                        ClientInstanceName="cmbDetailsTypeClient" Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                        <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                    </dx:ASPxComboBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep5" runat="server" style="display: none">
                            <table id="tbStep5" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="SchedulerWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep5Title" runat="server" Text="Paso 5 de 5." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep5Info" runat="server" Text="Resumen del fichaje" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep5Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepContent">
                                        <span id="lblStep5Info2" runat="server" style="font-size: 15px;"></span>
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