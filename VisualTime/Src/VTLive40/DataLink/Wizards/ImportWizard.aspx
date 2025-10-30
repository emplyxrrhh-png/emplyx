<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_ImportWizard" Culture="auto" UICulture="auto" EnableEventValidation="false" CodeBehind="ImportWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para la importación de datos</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmEmployeeCopyWizard" runat="server">
        <div>

            <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" AsyncPostBackTimeout="360000"></asp:ScriptManager>

            <script language="javascript" type="text/javascript">

                var bolLoaded = false;

                async function PageBase_Load() {
                    if (!bolLoaded) {
                        await getroTreeState('objContainerTreeV3_treeEmployeesImportWizard').then(roState => roState.reset());
                        await getroTreeState('objContainerTreeV3_treeEmployeesImportWizardGrid').then(roState => roState.reset());
                    }
                    
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);

                    if (!bolLoaded) bolLoaded = true;
                }

                //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
                function endRequestHandler() {
                    hidePopupLoader();
                }

                function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
                    var hdnEmployeesSelected = document.getElementById('<%= me.hdnEmployeesSelected.ClientID %>');
                    hdnEmployeesSelected.value = oParm1;

                    var hdnFilter = document.getElementById('<%= me.hdnFilter.ClientID %>');
                    hdnFilter.value = oParm2;

                    var hdnFilterUser = document.getElementById('<%= me.hdnFilterUser.ClientID %>');
                    hdnFilterUser.value = oParm3;
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
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                }

                function hidePopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                }

                function CheckFrame(intFrameIndex) {
                    var bolRet = true;

                    if (intFrameIndex == null) {
                        intFrameIndex = parseInt(document.getElementById('<%= hdnActiveFrame.ClientID %>').value);
                    }

                    if (intFrameIndex == 1) {
                        document.getElementById('ifUploads').contentWindow.SendFiles();
                        return false;
                    }

                    if (CheckConvertControls('divStep' + intFrameIndex) == false) {
                        bolRet = false;
                    }

                    if (!bolRet) hidePopupLoader();

                    return bolRet;
                }

                function ContinueSend() {
                    __doPostBack('btNext', '')
                }

                function ShowResult(eventArgument, context) {

                    var bolFinished = false;

                    if (eventArgument != '') {
                        var arrResult = eventArgument.split("&");

                        bolFinished = (arrResult[0] == "FINISHED");
                    }

                    if (bolFinished == true) {
                        // Guardar resultado
                        $get('<%= me.hdnResult.ClientID %>').value = eventArgument;

                    //ButtonClick($get('<%= Me.btEnd.ClientID %>'));
                        __doPostBack('btEnd', '');

                    }
                    else {
                        //setTimeout("CallServer()", 5000); //Periodic call to server.
                    }

                }

                var monitor = -1;

                function showCaptcha() {
                    var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=EXECUTEIMPORT";
                    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                    CaptchaObjectPopup_Client.Show();
                }

                function captchaCallback(action) {
                    switch (action) {
                        case "EXECUTEIMPORT":
                            AspxLoadingPopup_Client.Show();
                            PerformAction();
                            break;
                        case "ERROR":
                            window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
                            break;
                    }
                }

                function closeImport() {
                    try {
                        window.parent.frames["ifPrincipal"].ShowTreeImport();
                    } catch (e) { }
                    Close();
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
                        monitor = setInterval(function () {
                            PerformActionCallbackClient.PerformCallback("CHECKPROGRESS");
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
                    }

                }
            </script>

            <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>

            <input type="hidden" runat="server" id="hdnIsBusiness" value="0" />
            <div class="popupWizardContent">

                <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>

                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block;">
                            <table id="tbStep0" style="" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgNewMultiEmployeeWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzmens.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para importación de datos."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le permite importar ${Employees} desde diferentes formatos." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <div>
                                            <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                        </div>
                                        </br>
                                    <div>
                                        <textarea id="txtErrors" runat="server" class="textClass" style="height: 200px; width: 300px; color: Red;" visible="false"></textarea>
                                    </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="NewEmployeeWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:Label ID="hdnStepTitle" Text="Asistente para la importación de datos. " runat="server" Style="display: none; visibility: hidden" />
                <asp:Label ID="hdnStepTitle2" Text="Paso {0} de {1}." runat="server" Style="display: none; visibility: hidden" />
                <asp:Label ID="hdnSetpInfo" Text="Este asistente le permite importar ${Employees} desde diferentes formatos." runat="server" Style="display: none; visibility: hidden" />

                <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>

                        <div id="divStep1" runat="server" style="display: none;">
                            <table style="" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewEmployeeWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 50px;">
                                                    <asp:Label ID="lblStep1Info" runat="server" Text="" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewEmployeeWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewEmployeeWizards_StepContent">

                                        <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label ID="lblStep1Info2" runat="server" Text="Especifique inicialmente el tipo de importación y seguidamente los parametros de configuración." />
                                                            </span>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 5px" valign="top">
                                                    <div style="height: 100%; width: 100%;">
                                                        <iframe id="ifUploads" name="ifUploads" runat="server" style="background-color: Transparent"
                                                            height="250" width="100%" scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                                    </div>
                                                </td>
                                            </tr>

                                            <tr id="advancedImportOptions" runat="server">
                                                <td>
                                                    <div style="width: 100%">
                                                        <div class="panBottomMargin">
                                                            <div class="panHeader2 panBottomMargin">
                                                                <span class="panelTitleSpan">
                                                                    <asp:Label runat="server" ID="lblPlanType" Text="¿Qué tipo de planificación desea realizar?"></asp:Label>
                                                                </span>
                                                            </div>
                                                        </div>

                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <div class="">
                                                                    <dx:ASPxRadioButton ID="rbExcelType1" GroupName="rdExcelImportType" runat="server" Checked="true" Text="Aplicar a cada empleado seleccionado la planificación reseñada en la hoja Excel" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <div class="">
                                                                    <dx:ASPxRadioButton ID="rbExcelType2" GroupName="rdExcelImportType" runat="server" Checked="false" Text="Aplicar una plantilla de planificación a todos los empleados seleccionados" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div style="width: 100%">
                                                        <div class="panBottomMargin">
                                                            <div class="panHeader2 panBottomMargin">
                                                                <span class="panelTitleSpan">
                                                                    <asp:Label runat="server" ID="Label1" Text="¿Qué desea copiar?"></asp:Label>
                                                                </span>
                                                            </div>
                                                        </div>
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <div class="">
                                                                    <dx:ASPxCheckBox ID="chkCopyMainShifts" runat="server" Checked="true" Text="Copiar horarios principales" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <div class="">
                                                                    <dx:ASPxCheckBox ID="chkCopyHolidays" runat="server" Checked="false" Text="Copiar vacaciones" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div style="width: 100%">
                                                        <div class="panBottomMargin">
                                                            <div class="panHeader2 panBottomMargin">
                                                                <span class="panelTitleSpan">
                                                                    <asp:Label runat="server" ID="Label2" Text="¿Qué desea mantener?"></asp:Label>
                                                                </span>
                                                            </div>
                                                        </div>
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <div class="">
                                                                    <dx:ASPxCheckBox ID="chkKeepHolidays" runat="server" Checked="true" Text="Vacaciones actuales" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <div class="">
                                                                    <dx:ASPxCheckBox ID="chkKeepLockedDays" runat="server" Checked="true" Text="Días bloqueados" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
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

                        <!-- EmployeeSelector -->
                        <div id="divStep2" runat="server" style="display: none;">
                            <table style="" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewEmployeeWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 50px;">
                                                    <asp:Label ID="lblStep2Info" runat="server" Text="" />
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

                                        <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label ID="lblStep2Info2" runat="server" Text="Ahora seleccione los ${Employees} sobre los que desea extraer los datos." />
                                                            </span>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 5px" valign="top">

                                                    <div style="height: 100%;">
                                                        <input type="hidden" id="hdnEmployeesSelected" runat="server" value="" />
                                                        <input type="hidden" id="hdnFilter" runat="server" value="" />
                                                        <input type="hidden" id="hdnFilterUser" runat="server" value="" />
                                                        <iframe id="ifEmployeesSelector" runat="server" style="background-color: Transparent" height="290" width="100%"
                                                            scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                                    </div>
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
                            <table style="" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewEmployeeWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep3Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 50px;">
                                                    <asp:Label ID="lblStep3Info" runat="server" Text="" />
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
                                    <td class="NewEmployeeWizards_StepContent">

                                        <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label ID="Label3" runat="server" Text="Seleccione el intervalo de fechas entre las que desea importar la información." />
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <table width="100%" style="padding-top: 20px;">
                                                        <tr>
                                                            <td align="center">
                                                                <table>
                                                                    <tr>
                                                                        <td align="right">
                                                                            <asp:Label ID="lblScheduleBeginDate" Text="Importar desde el día" runat="server"></asp:Label>
                                                                        </td>
                                                                        <td align="left" style="padding-left: 10px;">
                                                                            <dx:ASPxDateEdit ID="txtScheduleBeginDate" PopupVerticalAlign="WindowCenter" runat="server" ClientInstanceName="txtScheduleBeginDateClient" AllowNull="false">
                                                                                <ClientSideEvents KeyPress="function(s,e){ return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}'); }" />
                                                                            </dx:ASPxDateEdit>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right">
                                                                            <asp:Label ID="lblScheduleEndDate" Text="hasta el día" runat="server"></asp:Label>
                                                                        </td>
                                                                        <td align="left" style="padding-left: 10px;">
                                                                            <dx:ASPxDateEdit ID="txtScheduleEndDate" PopupVerticalAlign="WindowCenter" runat="server" ClientInstanceName="txtScheduleEndDateClient" AllowNull="false">
                                                                                <ClientSideEvents KeyPress="function(s,e){ return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}'); }" />
                                                                            </dx:ASPxDateEdit>
                                                                        </td>
                                                                    </tr>
                                                                </table>
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
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:UpdatePanel ID="updStep4" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>

                        <div id="divStep4" runat="server" style="display: none;">
                            <table style="" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewEmployeeWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep4Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 50px;">
                                                    <asp:Label ID="lblStep4Info" runat="server" Text="" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewEmployeeWizards_StepError">
                                        <asp:Label ID="lblStep4Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewEmployeeWizards_StepContent">

                                        <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep4Info2" runat="server" Text="El asistente ya dispone de suficientes datos para la importación. Pulse Finalizar para iniciar el proceso." />
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
                            <tr class="NewEmployeeWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();return CheckFrame();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" OnClientClick="PerformValidation();return false;" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <%--<asp:Button ID="btEnd" Text="${Button.End}" runat="server" OnClientClick="ExecuteImport(); return false;" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />--%>
                                    <asp:Button ID="btResume" Text="" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="closeImport(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                    <asp:HiddenField ID="hdnResult" Value="" runat="server" />
                                    <asp:HiddenField ID="hdnIDImport" Value="" runat="server" />
                                    <asp:HiddenField ID="hdnConcept" Value="" runat="server" />
                                </td>
                            </tr>
                        </table>

                        <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
                        <input type="hidden" id="hdnIDEmployeeSource" value="" runat="server" />
                        <input type="hidden" id="hdnFrames" value="" runat="server" />
                        <input type="hidden" id="hdnFramesOnlyClient" value="" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
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
        </div>
    </form>
</body>
</html>