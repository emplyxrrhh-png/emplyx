<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_CopyScheduleWizard" Culture="auto" UICulture="auto" EnableEventValidation="false" CodeBehind="CopyScheduleWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para copiar planificaciones</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmSopyScheduleWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                var bolLoaded = false;

                async function PageBase_Load() {
                    if (!bolLoaded) {
                        await getroTreeState('objContainerTreeV3_treeEmpCopyScheduleWizard').then(roState => roState.reset());
                        await getroTreeState('objContainerTreeV3_treeEmpCopyScheduleWizardGrid').then(roState => roState.reset());
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

                var monitor = -1;

                function showCaptcha() {
                    var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=MASSCOPY";
                    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                    CaptchaObjectPopup_Client.Show();
                }

                function captchaCallback(action) {
                    switch (action) {
                        case "MASSCOPY":
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

            <asp:UpdatePanel ID="upCopyScheduleWizard" runat="server">
                <ContentTemplate>
                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgCopyScheduleWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzschedule.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblCopyScheduleWelcome1" runat="server" Text="Bienvenido al asistente para copiar planificaciones."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblCopyScheduleWelcome2" runat="server" Text="El asistente le permite copiar la planificación del empleado {0} a varios empleados." Font-Bold="true"></asp:Label>
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

                        <asp:Label ID="hdnStepTitle" Text="Asistente para copiar planificaciones. " runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none">
                            <table id="tbStep1" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="SchedulerWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="Paso 1 de 4." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblSetp1Info" runat="server" Text="Primero debe seleccionar los emplados a los que quiere copiar la planificación." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepContent">

                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Info2" runat="server" Text="Seleccione el tipo de filtro que desea aplicar al árbol de selección." />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <roUserControls:roOptionPanelContainer ID="optAllEmployee" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                                        Text="Ver todos los ${Employees}"
                                                        Description="Mostrar en el árbol de selección todos los ${Employees}."
                                                        Checked="true">
                                                        <Content></Content>
                                                    </roUserControls:roOptionPanelContainer>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <roUserControls:roOptionPanelContainer ID="optCondition" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                                        Text="Ver sólo los que cumplan la condición"
                                                        Description="Mostrar en el árbol de selección sólo los ${Employees} que cumplan la condición de la ficha."
                                                        Checked="false">
                                                        <Content>
                                                            <div style="text-align: left">
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblUserField" runat="server" Text="Que en el campo de la ficha"></asp:Label></td>
                                                                        <td>
                                                                            <roWebControls:roComboBox ID="cmbUserFields" runat="server" ItemsRunAtServer="False" ParentWidth="240px" EnableViewState="true" HiddenText="cmbUserFields_Text" AutoResizeChildsWidth="False" HiddenValue="cmbUserFields_Value" />
                                                                            <asp:HiddenField ID="cmbUserFields_Text" runat="server" />
                                                                            <asp:HiddenField ID="cmbUserFields_Value" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblValue" runat="server" Text="tenga el valor"></asp:Label></td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtValue" runat="server" CssClass="textClass"></asp:TextBox></td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </Content>
                                                    </roUserControls:roOptionPanelContainer>
                                                </td>
                                            </tr>
                                        </table>
                                        <roUserControls:roOptionPanelGroup ID="GroupFilterType" runat="server" />
                                    </td>
                                </tr>
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
                                                    <asp:Label ID="lblStep2Info" runat="server" Text="Ahora seleccione los empleados a los que quiere copiar la planificación." />
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
                                        <asp:Label ID="lblStep2Info2" runat="server" Text="Ahora seleccione los empleados a los que quiere copiar la planificación del empleado {0}. " /><br />
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
                                                    <asp:Label ID="lblStep3Info" runat="server" Text="Seleccione el periodo de fechas entre las cuales quiere copiar la planificación." />
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

                                        <asp:Label ID="lblStep3Info2" runat="server" Text="Finalmente seleccione el intervalo de fechas entre las que desea copiar la planificación." />
                                        <br />

                                        <table>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <asp:Label ID="lblBeginDate" runat="server" Text="Generar la planificación desde el día"></asp:Label>
                                                </td>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <dx:ASPxDateEdit runat="server" ID="txtBeginDate" Width="150" ClientInstanceName="txtBeginDateClient" AllowNull="false">
                                                        <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                    </dx:ASPxDateEdit>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 5px">
                                                    <asp:Label ID="lblEndDate" runat="server" Text="hasta el día"></asp:Label>
                                                </td>
                                                <td style="padding-left: 10px; padding-top: 5px">
                                                    <dx:ASPxDateEdit runat="server" ID="txtEndDate" Width="150" ClientInstanceName="txtEndDateClient" AllowNull="false">
                                                        <ClientSideEvents KeyDown="function(s,e){return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');}" />
                                                    </dx:ASPxDateEdit>
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
                                                    <asp:Label ID="lblStep4Info" runat="server" Text="Finalmente, seleccione qué quiere copiar." />
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
                                        <asp:Label ID="lblStep4Info2" runat="server" Text="Finalmente seleccione qué horarios desea copiar. Puede copiar sólo el horario principal, sólo los horarios alternativos o el horario principal y los horarios alternativos." />
                                        <br />
                                        <div style="padding-top: 10px">
                                            <div style="float: left; width: 100%">
                                                <div class="panBottomMargin">
                                                    <div class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblCopyTitle" Text="¿Qué desea copiar?"></asp:Label>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <dx:ASPxCheckBox ID="ckCopyMainShifts" runat="server" Checked="true" Text="Copiar horarios principales" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <dx:ASPxCheckBox ID="ckCopyAlternativeShifts" runat="server" Checked="true" Text="Copiar horarios alternativos" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <dx:ASPxCheckBox ID="ckCopyHolidays" runat="server" Checked="false" Text="Copiar vacaciones" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div style="float: left; width: 100%">
                                                <div class="panBottomMargin">
                                                    <div class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblMaintainTitle" Text="¿Qué desea mantener?"></asp:Label>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <dx:ASPxCheckBox ID="ckKeepHolidays" runat="server" Checked="true" Text="Vacaciones actuales" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <dx:ASPxCheckBox ID="ckKeepBloquedDays" runat="server" Checked="true" Text="Días bloqueados" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
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
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="return CheckFrame();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
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