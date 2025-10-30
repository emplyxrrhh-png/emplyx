<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_TemplateAssignWizardV2" Culture="auto" UICulture="auto" EnableViewState="True" EnableEventValidation="false" CodeBehind="TemplateAssignWizardV2.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para asignar plantillas de ${Shifts}</title>
</head>

<body class="bodyPopup" style="background-attachment: fixed;">
    <form id="frmAssignTemplateWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">
                var oTemplateManagement = null;
                var bolLoaded = false;

                function reloadShifts() {
                    cmbTemplatesClient.PerformCallback();
                    cmbAssignShiftsClient.PerformCallback('AddEmpty');
                };

                function cmbTemplateChange(s, e) {
                    if (s.GetSelectedItem() != null) {
                        var selValue = s.GetSelectedItem().value.split('_');

                        if (selValue.length > 1 && selValue[1] == '0') {
                            $('#shiftTemplateInfo').show();
                            $('#feastTemplateInfo').hide();
                            cmbAssignShiftsClient.PerformCallback('RemoveEmpty');
                        } else {
                            $('#shiftTemplateInfo').hide();
                            $('#feastTemplateInfo').show();
                            cmbAssignShiftsClient.PerformCallback('AddEmpty');
                            //cmbYearClient.SetEnabled(false);
                        }
                    } else {
                        $('#shiftTemplateInfo').hide();
                        $('#feastTemplateInfo').hide();
                        //cmbYearClient.SetEnabled(true);
                    }
                }

                function CheckShowStartShift(s, e) {

                    if (s.GetSelectedItem() != null) {

                        var sItem = s.GetSelectedItem().value.split('_');

                        if (sItem[1] == '0') {
                            $('#tdStartShift').hide();
                            document.getElementById('<%= Me.hdnIsFloating.ClientID %>').value = '0';
                        } else {

                            if (sItem[2] != '') {
                                switch (sItem[2].substr(6, 2)) {
                                    case '29':
                                        txtStartFloatingClient.SetDate(new Date(1899, 12, 29, sItem[2].substr(8, 2), sItem[2].substr(10, 2), 0));
                                        break;
                                    case '30':
                                        txtStartFloatingClient.SetDate(new Date(1899, 12, 30, sItem[2].substr(8, 2), sItem[2].substr(10, 2), 0));
                                        break;
                                    case '31':
                                        txtStartFloatingClient.SetDate(new Date(1899, 12, 31, sItem[2].substr(8, 2), sItem[2].substr(10, 2), 0));
                                        break;
                                }
                            } else {
                                txtStartFloatingClient.SetDate(new Date(1899, 12, 31, 0, 0, 0));
                            }

                            $('#tdStartShift').show();
                            document.getElementById('<%= Me.hdnIsFloating.ClientID %>').value = '1';
                        }

                    } else {
                        $('#tdStartShift').hide();
                        document.getElementById('<%= Me.hdnIsFloating.ClientID %>').value = '0';
                    }

                    txtStartFloatingClient.SetVisible(true);
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);
                }

                async function PageBase_Load() {
                    if (!bolLoaded) {
                        await getroTreeState('objContainerTreeV3_treeEmployeesEmployeesAssignTemplateWizard').then(roState => roState.reset());
                        await getroTreeState('objContainerTreeV3_treeEmployeesEmployeesAssignTemplateWizardGrid').then(roState => roState.reset());
                    }

                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);

                    cmbTemplateChange(cmbTemplatesClient);
                    CheckShowStartShift(cmbAssignShiftsClient);


                    if (!bolLoaded) bolLoaded = true;
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

                function showEditTemplate() {
                    oTemplateManagement = new templateManagementPopup();
                    oTemplateManagement.init();
                    oTemplateManagement.show();
                }

                var monitor = -1;

                function showCaptcha() {
                    var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=TEMPLATEASSIGN";
                    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                    CaptchaObjectPopup_Client.Show();
                }

                function captchaCallback(action) {
                    switch (action) {
                        case "TEMPLATEASSIGN":
                            AspxLoadingPopup_Client.Show();
                            PerformAction();
                            break;
                        case "ERROR":
                            window.parent.frames["ifPrincipal"].showErrorPopup2("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "", "");
                            break;
                    }
                }

                function PerformValidation() {

                    var oParameters = {};
                    oParameters.action = "VALIDATE";
                    oParameters.templates = [];
                    oParameters.StampParam = new Date().getMilliseconds();
                    var strParameters = JSON.stringify(oParameters);
                    strParameters = encodeURIComponent(strParameters);

                    PerformActionCallbackClient.PerformCallback(strParameters);

                }

                function PerformAction() {
                    var oParameters = {};
                    oParameters.action = "PERFORM_ACTION";
                    oParameters.templates = [];
                    oParameters.StampParam = new Date().getMilliseconds();
                    var strParameters = JSON.stringify(oParameters);
                    strParameters = encodeURIComponent(strParameters);

                    PerformActionCallbackClient.PerformCallback(strParameters);
                }
                //function CmbAssignEndCallback(s, e) {
                //    if (e.Parameter == "AddEmpty") {
                //        cmbAssignShiftsClient.AddItem("Ninguno", -1);
                //    }
                //}

                function PerformActionCallback_CallbackComplete(s, e) {
                    if (s.cpAction == "VALIDATE" && s.cpResult == true) {
                        showCaptcha();
                    } else if (s.cpAction == "PERFORM_ACTION") {
                        monitor = setInterval(function () {
                            var oParameters = {};
                            oParameters.action = "CHECKPROGRESS";
                            oParameters.templates = [];
                            oParameters.StampParam = new Date().getMilliseconds();
                            var strParameters = JSON.stringify(oParameters);
                            strParameters = encodeURIComponent(strParameters);

                            PerformActionCallbackClient.PerformCallback(strParameters);
                        }, 5000);
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
                    } else if (s.cpAction == "LOADTEMPLATESLIST" || s.cpAction == 'GETTEMPLATE' || s.cpAction == 'SAVETEMPLATE' || s.cpAction == 'DELETETEMPLATE') {
                        oTemplateManagement.parseResponse(s, e);
                    }
                }

                function reloadTemplates() {
                    //  __doPostBack('<%= btnReload.ClientID %>', '');
                    reloadShifts();
                }
            </script>

            <div class="popupWizardContent">

                <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>

                <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>

                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block;">
                            <table id="tbStep0" style="width: 100%;" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td style="height: 440px" valign="top">
                                        <asp:Image ID="imgNewMultiEmployeeWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wztemplate.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para asignar plantillas de ${Shifts}."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="El asistente le ayudará a asignar una plantilla de ${Shifts} a uno o varios ${Employees}." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="AssignScheduleTemplateWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:Label ID="hdnStepTitle" Text="Asistente para la asignación de plantillas de ${Shifts}. " runat="server" Style="display: none; visibility: hidden" />
                <asp:Label ID="hdnStepTitle2" Text="Paso {0} de {1}." runat="server" Style="display: none; visibility: hidden" />

                <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>

                        <div id="divStep1" runat="server" style="display: none;">
                            <table style="width: 99%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblSetp1Info" runat="server" Text="En primer lugar, debe seleccionar los ${Employees} a los que quiere asignar la plantilla de ${Shifts}." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepContent">

                                        <table style="width: 100%; height: 350px" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Info2" runat="server" Text="Seleccione un ${Group}." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 5px;">

                                                    <input type="hidden" id="hdnIDGroupSelected" runat="server" value="" />
                                                    <input type="hidden" id="hdnEmployeesSelected" runat="server" value="" />
                                                    <input type="hidden" id="hdnFilter" runat="server" value="" />
                                                    <input type="hidden" id="hdnFilterUser" runat="server" value="" />
                                                    <iframe id="ifEmployeesSelector" runat="server" style="background-color: Transparent" height="290" width="100%"
                                                        scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
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
                            <table style="width: 99%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblSetp2Info" runat="server" Text="Ahora debe parametrizar la forma de aplicar la plantilla seleccionada." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepContent" valign="top">

                                        <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td colspan="3" align="left" style="padding-left: 5px; padding-top: 5px;">
                                                    <roUserControls:roGroupBox ID="RoGroupBox1" runat="server">
                                                        <Content>
                                                            <div style="width: 100%">
                                                                <div class="panBottomMargin">
                                                                    <div class="panHeader2 panBottomMargin">
                                                                        <span class="panelTitleSpan">
                                                                            <asp:Label runat="server" ID="lblStep2Info2" Text="Seleccione la acción a realizar."></asp:Label>
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                                <div class="">
                                                                    <div class="divRow noBottomPadding">
                                                                        <div class="divRowDescription midWidthDescription">
                                                                            <asp:Label ID="lblTemplateDescription" runat="server" Text="Seleccione la plantilla que desea asignar"></asp:Label>
                                                                        </div>
                                                                        <asp:Label ID="lblTemplateName" runat="server" Text="Plantilla:" CssClass="labelForm midWidth"></asp:Label>
                                                                        <div class="componentForm" style="width: calc(100% - 70px)">
                                                                            <div style="float: left">
                                                                                <dx:ASPxComboBox ID="cmbTemplates" runat="server" Width="170" ClientInstanceName="cmbTemplatesClient">
                                                                                    <ClientSideEvents SelectedIndexChanged="cmbTemplateChange" />
                                                                                </dx:ASPxComboBox>
                                                                            </div>
                                                                            <div class="editTemplates" style="float: left;" onclick="showEditTemplate();" title="<%=Me.Language.Translate("EditTemplates", Me.DefaultScope) %>">
                                                                            </div>
                                                                            <div id="shiftTemplateInfo" style="float: left; display: none">
                                                                                <asp:Label ID="lblShiftTemplateInfo" runat="server" Text="Plantilla de asignación de horarios" CssClass="labelForm maxWidthInfo"></asp:Label>
                                                                            </div>
                                                                            <div id="feastTemplateInfo" style="float: left; display: none">
                                                                                <asp:Label ID="lblFeastTemplateInfo" runat="server" Text="Plantilla de asignación de festivos" CssClass="labelForm maxWidthInfo"></asp:Label>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="">
                                                                    <div class="divRow noBottomPadding">
                                                                        <div class="divRowDescription midWidthDescription">
                                                                            <asp:Label ID="lblTemplateShiftDesc" runat="server" Text="Seleccione el horario que quiere asignar con la plantilla"></asp:Label>
                                                                        </div>
                                                                        <asp:Label ID="lblTemplateShif" runat="server" Text="Horario:" CssClass="labelForm midWidth"></asp:Label>
                                                                        <div class="componentForm">
                                                                            <div id="divAssignShifts" style="float: left">
                                                                                <dx:ASPxComboBox ID="cmbAssignShifts" runat="server" Width="170" ClientInstanceName="cmbAssignShiftsClient">
                                                                                    <ClientSideEvents SelectedIndexChanged="CheckShowStartShift" />
                                                                                </dx:ASPxComboBox>
                                                                            </div>
                                                                            <div id="tdStartShift" style="float: left; display: none;">
                                                                                <input type="hidden" id="hdnIsFloating" value="0" runat="server" />
                                                                                <div style="float: left">
                                                                                    <dx:ASPxTimeEdit Width="85" EditFormatString="HH:mm" EditFormat="Custom" ID="txtStartFloating" runat="server" ClientInstanceName="txtStartFloatingClient">
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                    </dx:ASPxTimeEdit>
                                                                                </div>
                                                                                <div style="float: left">
                                                                                    <dx:ASPxComboBox ID="cmbStartFloating" runat="server" Width="150" ClientInstanceName="cmbStartFloatingClient">
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                    </dx:ASPxComboBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <%--<div class="">
                                                                    <div class="divRow noBottomPadding">
                                                                        <div class="divRowDescription midWidthDescription">
                                                                            <asp:Label ID="lblShiftTemplateParamsDesc" runat="server" Text="Solo se puede seleccionar el año en plantillas de asignación de horarios."></asp:Label>
                                                                        </div>
                                                                        <asp:Label ID="ShiftTemplateParams" runat="server" Text="Año:" CssClass="labelForm midWidth"></asp:Label>
                                                                        <div class="componentForm">
                                                                            <dx:ASPxComboBox ID="cmbYear" runat="server" Width="70" ClientInstanceName="cmbYearClient">
                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                            </dx:ASPxComboBox>
                                                                        </div>
                                                                    </div>
                                                                </div>--%>

                                                                <div class="">
                                                                    <div class="divRow noBottomPadding">
                                                                        <div class="divRowDescription midWidthDescription">
                                                                            <asp:Label ID="lblLockTemplateDaysDesc" runat="server" Text="Los días bloqueados durante el proceso no se podrán modificar."></asp:Label>
                                                                        </div>
                                                                        <asp:Label ID="lblLockTemplateDays" runat="server" Text="Bloquear:" CssClass="labelForm midWidth"></asp:Label>
                                                                        <div class="componentForm">
                                                                            <dx:ASPxCheckBox runat="server" Text="Bloquear los días que modifique esta plantilla." ID="ckLockDays" Checked="True" Wrap="False" ClientInstanceName="ckLockDaysClient">
                                                                            </dx:ASPxCheckBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </Content>
                                                    </roUserControls:roGroupBox>
                                                </td>
                                            </tr>
                                            <tr>

                                                <td colspan="3" align="left" style="padding-left: 5px; padding-top: 5px;">
                                                    <roUserControls:roGroupBox ID="gbCopyOptions" runat="server">
                                                        <Content>
                                                            <div style="width: 100%">
                                                                <div class="panBottomMargin">
                                                                    <div class="panHeader2 panBottomMargin">
                                                                        <span class="panelTitleSpan">
                                                                            <asp:Label runat="server" ID="lblMaintainTitle" Text="¿Qué desea mantener?"></asp:Label>
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                                <div class="">
                                                                    <div class="divRow noBottomPadding">
                                                                        <div class="">
                                                                            <dx:ASPxCheckBox ID="ckKeepHolidays" runat="server" Checked="true" Text="Vacaciones actuales" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="">
                                                                    <div class="divRow noBottomPadding">
                                                                        <div class="">
                                                                            <dx:ASPxCheckBox ID="ckKeepBloquedDays" runat="server" Checked="true" Text="Días bloqueados" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </Content>
                                                    </roUserControls:roGroupBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" align="left" style="padding-left: 5px; padding-top: 5px;">
                                                    <roUserControls:roGroupBox ID="gbUserField" runat="server">
                                                        <Content>
                                                            <div style="width: 100%">
                                                                <div class="panBottomMargin">
                                                                    <div class="panHeader2 panBottomMargin">
                                                                        <span class="panelTitleSpan">
                                                                            <asp:Label runat="server" ID="Label1" Text="Campo de la ficha a actualizar"></asp:Label>
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                                <div class="">
                                                                    <div class="divRow noBottomPadding">
                                                                        <div class="divRowDescription midWidthDescription">
                                                                            <asp:Label ID="lbTitleUserfield" runat="server" Text="Seleccione el campo de la ficha a actualizar"></asp:Label>
                                                                        </div>
                                                                        <asp:Label ID="lbUserfield" runat="server" Text="Campo:" CssClass="labelForm midWidth"></asp:Label>
                                                                        <div class="componentForm" style="width: calc(100% - 70px)">
                                                                            <div style="float: left">
                                                                                <dx:ASPxComboBox ID="cmbUserFields" runat="server" Width="170" ClientInstanceName="cmbUserFieldClient">
                                                                                </dx:ASPxComboBox>
                                                                            </div>
                                                                            <div id="userFieldLabel" style="float: left; padding: 4px;">
                                                                                <asp:Label ID="userfieldLabelText" Text="Valor:" runat="server"></asp:Label>
                                                                            </div>
                                                                            <div id="userFieldValue" style="float: left; padding: 3px;">
                                                                                <asp:TextBox ID="userFieldValueText" runat="server"></asp:TextBox>
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
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:UpdatePanel ID="updStep3" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>

                        <div id="divStep3" runat="server" style="display: none;">
                            <table style="width: 99%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep3Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblSetp3Info" runat="server" Text="Resumen." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepContent" valign="top">

                                        <table style="width: 100%;" cellspacing="0" cellpadding="0" border="0">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep3Info2" runat="server" Text="." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 10px">
                                                    <asp:Label ID="lblResumeAll" runat="server" Text="Se asignará el ${Shift} <b>{1}</b> a todos los ${Employees} seleccionados en los días que especifica la plantilla <b>{2}</b>." />
                                                </td>
                                            </tr>
                                        </table>
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
                            <tr class="AssignScheduleTemplateWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();return CheckFrame();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="Asignar" runat="server" OnClientClick="PerformValidation();return false;" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btLoop" Text="Volver a asignar" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btResume" Text="" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btnReload" Text="" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </td>
                            </tr>
                        </table>

                        <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
                        <input type="hidden" id="hdnLockedMsg" value="" runat="server" />
                        <input type="hidden" id="hdnLockedEmployee" value="" runat="server" />
                        <input type="hidden" id="hdnLockedDay" value="" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div id="templateManagerContainer" style="display: none">
                <div style="clear: both">
                    <div style="float: left; width: calc(40% - 5px); border-right: 2px solid #444;">
                        <div>
                            <div id="dxAddTemplateText" style="float: left; width: calc(80%)"></div>
                            <div id="imgBtnAdd" style="float: left; width: calc(20%)"></div>
                        </div>

                        <div id="dxTemplatesList" style="clear: both">
                        </div>
                    </div>
                    <div id="dxItemTemplate" style="float: left; width: calc(55% - 5px); margin-left: 30px;">
                        <div>
                            <div id="dxUpdateNameText" style="float: left; width: calc(80%)"></div>
                            <div id="imgBtnSave" style="float: left; width: calc(20%)"></div>
                        </div>

                        <div style="clear: both; padding-top: 15px">
                            <div id="dxTemplatesDays"></div>
                        </div>
                        <div id="dxCkIsFeast" style="clear: both; padding: 5px;"></div>
                    </div>
                </div>

                <div style="clear: both">
                    <div id="btnOkEditNode" class="acceptButton" style="float: right; margin-right: 15px;"></div>
                </div>
            </div>

            <div id="cellEditPopover" style="display: none">
                <div>
                    <div id="dxDayDescriptionText" style="float: left; width: calc(80%)"></div>
                    <div id="btnRemoveDay" class="" style="float: left; width: calc(20%)"></div>
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

            <Local:ExternalForm ID="externalform1" DragEnabled="false" runat="server" />
        </div>
    </form>
</body>
</html>