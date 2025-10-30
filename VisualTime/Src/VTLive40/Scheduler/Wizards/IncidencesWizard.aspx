<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_IncidencesWizard" Culture="auto" UICulture="auto" EnableEventValidation="false" CodeBehind="IncidencesWizard.aspx.vb" %>

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

                function downloadFile() {
                    //btnDownloadClient.SetVisible(false);
                    $("#divDownload").hide();

                    top.ShowExternalForm2('DataLink/Wizards/downloadFile.aspx', 900, 515, '', '', false, false, false);
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
                                    <td style="height: 360px">
                                        <asp:Image ID="imgIncidencesWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wztemplate.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblCopyScheduleWelcome1" runat="server" Text="Bienvenido al asistente para la justificación masiva de incidencias."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblCopyScheduleWelcome2" runat="server" Text="El asistente le permite justificar de forma masiva incidencias de empleados en un periodo determinado." Font-Bold="true"></asp:Label>
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

                        <asp:Label ID="hdnStepTitle" Text="Asistente para la justificación masiva de incidencias." runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none">
                            <table id="tbStep1" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                            </table>
                        </div>

                        <div id="DivStep2" runat="server" style="display: none">
                            <table id="btStep2" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepTitle2">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="Paso 2 de 7." Font-Bold="True" />
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
                                    <td class="SchedulerWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepContent">
                                        <asp:Label ID="lblStep2Info2" runat="server" Text="Ahora seleccione los empleados a los que quiere justificar las incidencias. " /><br />
                                        <br />
                                        <input type="hidden" id="hdnEmployeesSelected" runat="server" value="" />
                                        <input type="hidden" id="hdnFilter" runat="server" value="" />
                                        <input type="hidden" id="hdnFilterUser" runat="server" value="" />
                                        <iframe id="ifEmployeeSelector" runat="server" style="background-color: Transparent" height="400" width="100%"
                                            scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep3" runat="server" style="display: none">
                            <table id="tbStep3" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepTitle2">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep3Title" runat="server" Text="Paso 3 de 7." Font-Bold="True" />
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
                                    <td class="SchedulerWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepContent" valign="middle">

                                        <asp:Label ID="lblStep3Info2" runat="server" Text="Seleccione el periodo donde quiere realizar la búsqueda. " />
                                        <br />

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
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep4" runat="server" style="display: none">
                            <table id="tbStep4" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepTitle2">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep4Title" runat="server" Text="Paso 4 de 7." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep4Info" runat="server" Text=" " />
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
                                        <asp:Label ID="lblStep4Info2" runat="server" Text="Seleccione las incidencias que quiere justificar. " />
                                        <br />
                                        <table style="width: 100%">
                                            <tr style="padding-top: 10px">
                                                <td>
                                                    <asp:Panel ID="panIncidence" Height="250" BackColor="white" ScrollBars="Vertical" runat="server">
                                                        <asp:TreeView ID="treeIncidences" ShowCheckBoxes="All" ShowExpandCollapse="false" runat="server"></asp:TreeView>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 10px">
                                                <td>

                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <input type="checkbox" runat="server" id="chkApplyFilter" checked="checked" /></td>
                                                            <td>
                                                                <asp:Label ID="lblIncPlusAccValue" Text="el valor" runat="server" class="SectionDescription"></asp:Label></td>
                                                            <td>
                                                                <dx:ASPxComboBox ID="cmbIncPlusAccSup" runat="server" Width="325px" Font-Size="11px" CssClass="editTextFormat"
                                                                    Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbIncPlusAccSupClient">
                                                                </dx:ASPxComboBox>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblIncPlusAccSup" Text="Que" runat="server" class="SectionDescription"></asp:Label></td>
                                                            <td>
                                                                <dx:ASPxTextBox ID="txtIncPlusAccValue" runat="server" Width="90" MaxLength="7">
                                                                    <MaskSettings Mask="<0..9999>:<00..59>" />
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep5" runat="server" style="display: none">
                            <table id="tbStep5" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepTitle2">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep5Title" runat="server" Text="Paso 5 de 7." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep5Info" runat="server" Text=" " />
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
                                    <td class="SchedulerWizards_StepContent" valign="middle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td class="LaunchTaskTemplateWizard_StepContent" valign="top" style="padding-top: 10px;">
                                                    <div style="height: 375px; overflow: auto">
                                                        <dx:ASPxGridView ID="grdCauses" runat="server" Cursor="pointer" AutoGenerateColumns="False" ClientInstanceName="grdCausesClient" KeyboardSupport="True" Width="98%" Settings-ShowTitlePanel="True">
                                                            <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="false" AllowSort="False" />
                                                            <Styles>
                                                                <Cell Wrap="False"></Cell>
                                                                <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                                                            </Styles>
                                                            <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                            <Border BorderColor="#CDCDCD" />
                                                            <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                                                            <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="325" />
                                                        </dx:ASPxGridView>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep6" runat="server" style="display: none">
                            <table id="tbStep6" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepTitle2">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep6Title" runat="server" Text="Paso 6 de 7." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep6Info" runat="server" Text=" " />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep6Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SchedulerWizards_StepContent" valign="middle">
                                        <asp:Label ID="lblStep6Info2" runat="server" Text="Seleccione los centros de coste que quiere mostrar. " />
                                        <table style="width: 100%">
                                            <tr>
                                                <td class="LaunchTaskTemplateWizard_StepContent" valign="top" style="padding-top: 10px;">
                                                    <div style="height: 375px; overflow: auto">
                                                        <dx:ASPxGridView ID="grdCenters" runat="server" Cursor="pointer" AutoGenerateColumns="False" ClientInstanceName="grdCentersClient" KeyboardSupport="True" Width="98%" Settings-ShowTitlePanel="True">
                                                            <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="false" AllowSort="False" />
                                                            <Styles>
                                                                <Cell Wrap="False"></Cell>
                                                                <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                                                            </Styles>
                                                            <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                            <Border BorderColor="#CDCDCD" />
                                                            <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                                                            <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="325" />
                                                        </dx:ASPxGridView>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep7" runat="server" style="display: none">
                            <table id="tbStep7" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="AssignScheduleTemplateWizards_StepTitle2">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep7Title" runat="server" Text="Paso 7 de 7." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep7Info" runat="server" Text="Finalmente, justifique las incidencias seleccionadas. " />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep7Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 20px; padding-right: 40px">
                                        <div style="float: right; padding-right: 10px;">
                                            <dx:ASPxButton ID="btnExportToXls" runat="server" CausesValidation="False" Text="Exportar" ToolTip="Exportar a Excel" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                <Image Url="~/Base/Images/Grid/ExportToExcel16.png"></Image>
                                            </dx:ASPxButton>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-left: 20px; padding-right: 40px">
                                        <div id="divDownload" style="float: right; padding-right: 10px;">
                                            <dx:ASPxButton ID="btnDownload" runat="server" AutoPostBack="False" CausesValidation="False" Text="Descargar fichero" ToolTip="Descargar fichero" ClientInstanceName="btnDownloadClient"
                                                HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" Visible="false">
                                                <ClientSideEvents Click="function(s, e) { downloadFile(); return false; }" />
                                            </dx:ASPxButton>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepContent" valign="top" style="padding-top: 10px;">
                                        <div style="max-height: 325px; overflow: auto">
                                            <dx:ASPxGridView ID="GridIncidences" runat="server" Cursor="pointer" AutoGenerateColumns="False" ClientInstanceName="GridTasksClient" KeyboardSupport="True" Width="98%" ClientSideEvents-BeginCallback="GridIncidences_beginCallback" Settings-ShowTitlePanel="True">
                                                <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="False" AllowSort="False" />
                                                <Styles>
                                                    <Cell Wrap="False"></Cell>
                                                    <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                                                </Styles>
                                                <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                <Border BorderColor="#CDCDCD" />
                                                <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                                                <ClientSideEvents EndCallback="GridIncidences_EndCallback" RowDblClick="GridIncidences_OnRowDblClick" FocusedRowChanged="GridIncidences_FocusedRowChanged" SelectionChanged="GridIncidences_SelectionChanged" />
                                                <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="285" />
                                            </dx:ASPxGridView>
                                            <dx:ASPxGridViewExporter ID="ASPxGridViewExporter1" runat="server"></dx:ASPxGridViewExporter>
                                        </div>
                                        <br />
                                        <div style="clear: both">
                                            <div style="float: left; margin-top: 6px;">
                                                <asp:Label ID="lblSelectedHours" runat="server" Text="Total de horas seleccionadas:" Font-Bold="True" />
                                            </div>
                                            <div style="float: left">
                                                <dx:ASPxTextBox runat="server" ID="txtTimeSelected" MaxLength="9" Width="75px" ClientInstanceName="txtTimeSelectedClient" ReadOnly="true">
                                                    <MaskSettings Mask="<000000..999999>:<00..59>" />
                                                    <ClientSideEvents TextChanged="function(s,e){ }" />
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                        <div style="clear: both">
                                            <div style="float: left; margin-top: 3px;">
                                                <input type="checkbox" id="ckMoveAllTo" runat="server" />
                                            </div>
                                            <div style="float: left; margin-top: 3px;">
                                                <asp:Label ID="lblMoveTo" runat="server" Text="Asignar a todas las horas seleccionadas la justificación:" Font-Bold="True" />
                                            </div>
                                            <div style="float: left">
                                                <dx:ASPxComboBox ID="cmbCauses" runat="server" ClientInstanceName="cmbCausesClient" Width="250px"></dx:ASPxComboBox>
                                            </div>
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
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="6" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

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