<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_ExportWizard" Culture="auto" UICulture="auto" EnableEventValidation="false" CodeBehind="ExportWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para la extracción de datos</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmEmployeeCopyWizard" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" AsyncPostBackTimeout="360000"></asp:ScriptManager>

        <script language="javascript" type="text/javascript">

            var bolLoaded = false;

            async function PageBase_Load() {
                if (!bolLoaded) {
                    await getroTreeState('objContainerTreeV3_treeEmpExportWizard').then(roState => roState.reset());
                    await getroTreeState('objContainerTreeV3_treeEmpExportWizardGrid').then(roState => roState.reset());
                }

                var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                ConvertControls('divStep' + oActiveFrameIndex);
                profileChanged(cmbProfilesExportClient);

                if (!bolLoaded) bolLoaded = true;

                cmbIsASCII();
            }

            function cmbIsASCII() {
                var isASCII = false;
                if (cmbFormatExportClient.GetSelectedItem() != null && cmbFormatExportClient.GetSelectedItem().value == "2") isASCII = true;

                if (isASCII) {
                    $('.trSeparator').show();
                } else {
                    $('.trSeparator').hide();
                }
            }

            function profileChanged(s, e) {

                var eConcept = document.getElementById('<%= hdnConceptProfile.ClientID %>').value;
                var eFormatDiv = document.getElementById('<%= formatExportTr.ClientID %>');

                if (eConcept != 2 && eConcept != 5 && eConcept != 10 && eConcept != 12) {
                    if (eConcept == 6) {
                        if (s.GetSelectedItem().value.split('@')[0] != '') {
                            $(eFormatDiv).show();
                            cmbIsASCII();
                        } else {
                            $(eFormatDiv).hide();
                            $('.trSeparator').hide();
                        }
                    } else {
                        $(eFormatDiv).show();
                        cmbIsASCII();
                    }
                } else {
                    $(eFormatDiv).hide();
                    $('.trSeparator').hide();
                }

                if (s.GetSelectedItem() != null && s.GetSelectedItem().value.includes("AtDate")) {
                    $("#beginDateTr").hide();
                }
                else {
                    $("#beginDateTr").show();
                }

                //beginDateTr.SetVisible(false);
            }
            //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
            function endRequestHandler() {
                var hdnFileExport = document.getElementById('<%= me.hdnFileExport.ClientID %>');
                if (hdnFileExport != null) {
                    if (hdnFileExport.value == '1') {
                        top.ShowExternalForm2('DataLink/Wizards/downloadFile.aspx', 500, 450, '', '', false, false, false);
                    }
                }
                hidePopupLoader();

                var hdnAskConfirmation = document.getElementById('<%= me.hdnAskOverwritePeriodConfirmation.ClientID %>');
                if (hdnAskConfirmation != null) {
                    if (hdnAskConfirmation.value == '1') {
                        Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
                            { text: '', key: 'roJsWarning' },
                            { text: '', key: 'roPeriodAlreadyExported' },
                            { text: '', textkey: 'roExport', desc: '', desckey: '', script: 'document.getElementById("<%= Me.hdnOverwritePeriod.ClientID %>").value = "1";__doPostBack("btNext","");' },
                            { text: '', textkey: 'roCancel', desc: '', desckey: '', script: '' },
                            Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton());
                    }
                }
            }

            function showPopupLoader() {
                window.parent.frames["ifPrincipal"].showLoadingGrid(true);
            }

            function hidePopupLoader() {
                window.parent.frames["ifPrincipal"].showLoadingGrid(false);
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

            function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
                var hdnEmployeesSelected = document.getElementById('<%= me.hdnEmployeesSelected.ClientID %>');
                hdnEmployeesSelected.value = oParm1;

                var hdnFilter = document.getElementById('<%= me.hdnFilter.ClientID %>');
                hdnFilter.value = oParm2;

                var hdnFilterUser = document.getElementById('<%= me.hdnFilterUser.ClientID %>');
                hdnFilterUser.value = oParm3;
            }

            function CheckFrame(intFrameIndex) {
                var bolRet = true;

                if (intFrameIndex == null) {
                    intFrameIndex = parseInt(document.getElementById('<%= hdnActiveFrame.ClientID %>').value);
                }

                if (CheckConvertControls('divStep' + intFrameIndex) == false) {
                    bolRet = false;
                }

                if (bolRet) {
                    if (intFrameIndex == '1') {
                        //Verificar que haya algún empleado seleccionado
                        bolRet = (document.getElementById('<%= hdnEmployeesSelected.ClientID %>').value != '');
                        if (!bolRet) {
                            document.getElementById('<%= lblStep1Error.ClientID %>').innerHTML = '<%= Me.Language.TranslateJavaScript("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope) %>';
                        }
                        else {
                            document.getElementById('<%= lblStep1Error.ClientID %>').innerHTML = '';
                        }
                    }
                }

                if (!bolRet) hidePopupLoader();

                return bolRet;
            }

            function ShowResult(eventArgument, context) {

                var bolFinished = false;

                if (eventArgument != '') {
                    var arrResult = eventArgument.split("&");

                    bolFinished = (arrResult[0] == "FINISHED");
                }

                if (bolFinished == true) {

                    if (arrResult[2] == "1") {
                        top.ShowExternalForm2('DataLink/Wizards/downloadFile.aspx', 500, 450, '', '', false, false, false);
                    }

                    // Guardar resultado
                    $get('<%= me.hdnResult.ClientID %>').value = eventArgument;

                    //ButtonClick($get('<%= Me.btEnd.ClientID %>'));
                    __doPostBack('btEnd', '');

                }
                else {
                    //setTimeout("CallServer()", 5000); //Periodic call to server.
                }

            }

            function closeExport() {
                try {
                    var bRefresh = document.getElementById("<%= me.hdnOnlyProfile.ClientID %>").value.toUpperCase() == "TRUE" ? false : true

                    window.parent.frames["ifPrincipal"].ShowTreeExport(bRefresh);
                    if (!bRefresh) {
                        window.parent.frames["ifPrincipal"].txtEmployeeFilterClient.SetText(Globalize.formatMessage("roSelectedEmployees"));
                    }
                } catch (e) { }
                Close();
            }

            var monitor = -1;

            function showCaptcha() {
                var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=EXECUTEEXPORT";
                CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                CaptchaObjectPopup_Client.Show();
            }

            function captchaCallback(action) {
                switch (action) {
                    case "EXECUTEEXPORT":
                        AspxLoadingPopup_Client.Show();
                        PerformAction();
                        break;
                    case "ERROR":
                        window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
                        break;
                }
            }

            function PerformValidation(bExecute) {
                if (bExecute) {
                    PerformActionCallbackClient.PerformCallback("VALIDATE");
                } else {
                    PerformActionCallbackClient.PerformCallback("SAVE_PROFILE");
                }
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
                } else if (s.cpAction == "SAVE_PROFILE") {
                    if (s.cpActionResult == "OK") {
                        closeExport();
                    } else {
                        window.parent.frames["ifPrincipal"].showErrorPopup("Error.NoEmployeesSelected", "ERROR", "Error.NoEmployeesSelectedDesc", "Error.OK", "Error.OKDesc", "");
                    }
                }

            }

            function downloadFile() {
                btnDownloadClient.SetVisible(false);
                top.ShowExternalForm2('DataLink/Wizards/downloadFile.aspx', 800, 450, '', '', false, false, false);
            }
        </script>

        <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>

        <input type="hidden" runat="server" id="hdnExcelInstalled" value="0" />
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
                        <table id="tbStep0" style="width: 100%;" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="height: 360px">
                                    <asp:Image ID="imgNewMultiEmployeeWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzmens.gif" Height="392px" />
                                </td>
                                <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px; overflow: hidden" valign="top">

                                    <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para la extracción de datos." Font-Bold="True" Font-Size="Large"></asp:Label>
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le permite realizar una extracción de datos." Font-Bold="true"></asp:Label>
                                    <br />
                                    <br />
                                    <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente." Width="280px"></asp:Label>
                                    <textarea id="txtErrors" runat="server" class="textClass" style="height: 200px; color: Red;" visible="false"></textarea>
                                    <br />
                                    <br />
                                    <dx:ASPxButton ID="btnDownload" runat="server" AutoPostBack="False" CausesValidation="False" Text="Descargar fichero" ToolTip="Descargar fichero" ClientInstanceName="btnDownloadClient"
                                        HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" Visible="false">
                                        <ClientSideEvents Click="function(s, e) { downloadFile(); return false; }" />
                                    </dx:ASPxButton>
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 10px" colspan="2" class="NewEmployeeWizards_ButtonsPanel"></td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:Label ID="hdnStepTitle" Text="Asistente para la extracción de datos. " runat="server" Style="display: none; visibility: hidden" />
            <asp:Label ID="hdnStepTitle2" Text="Paso {0} de {1}." runat="server" Style="display: none; visibility: hidden" />
            <asp:Label ID="hdnSetpInfo" Text="Este asistente le permite extraer información de VisualTime que seleccione." runat="server" Style="display: none; visibility: hidden" />

            <!-- EmployeeSelector -->
            <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>

                    <!-- EmployeeSelector -->
                    <div id="divStep1" runat="server" style="display: none;">
                        <asp:Panel runat="server" ID="newEmployeeSelectorPanel">
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
                                                    <asp:Label ID="lblStep1Info2" runat="server" Text="Ahora seleccione los ${Employees} sobre los que desea extraer los datos." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 3px" valign="top">

                                                    <div style="height: 100%;">
                                                        <input type="hidden" id="hdnOnlyProfile" runat="server" value="" />
                                                        <input type="hidden" id="hdnEmployeesSelected" runat="server" value="" />
                                                        <input type="hidden" id="hdnFilter" runat="server" value="" />
                                                        <input type="hidden" id="hdnFilterUser" runat="server" value="" />
                                                        <iframe id="ifEmployeesSelector" runat="server" style="background-color: Transparent" height="415" width="100%"
                                                            scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="excelErrorTable" Visible="false">
                            <table style="width: 500px; height: 410px;">
                                <tr>
                                    <td class="NewEmployeeWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="excelErrorTitle" runat="server" Text="No puede proceder con la exportación" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 50px;">
                                                    <asp:Label ID="excelErrorSub" runat="server" Text="Pida a su administrador que instale Excel 2003 professinal o superior en el servidor de Visual Time" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <!--Intervalo de fechas -->
            <asp:UpdatePanel ID="updStep2" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <!--intervalo de fechas -->
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

                                    <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep2Info2" runat="server" Text="Seleccione el intervalo de fechas entre las que desea exportar la información." />
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
                                                                        <asp:Label ID="lblProfileName" runat="server" Text="Plantilla:"></asp:Label>
                                                                    </td>

                                                                    <td align="left" style="padding-left: 10px;">
                                                                        <input type="hidden" value="-1" id="hdnConceptProfile" runat="server" />
                                                                        <dx:ASPxComboBox ID="cmbProfilesExport" runat="server" Width="200px" Font-Size="11px" CssClass="editTextFormat" Font-Names="Arial;Verdana;Sans-Serif"
                                                                            IncrementalFilteringMode="Contains" ClientInstanceName="cmbProfilesExportClient">
                                                                            <ClientSideEvents SelectedIndexChanged="profileChanged" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                                <tr id="formatExportTr" runat="server">
                                                                    <td align="right">
                                                                        <asp:Label ID="lblFormatExport" runat="server" Text="Tipo de exportación:"></asp:Label>
                                                                    </td>

                                                                    <td align="left" style="padding-left: 10px;">
                                                                        <dx:ASPxComboBox ID="cmbFormatExport" runat="server" Width="200px" Font-Size="11px" CssClass="cmbFormatExport" Font-Names="Arial;Verdana;Sans-Serif"
                                                                            IncrementalFilteringMode="Contains" ClientInstanceName="cmbFormatExportClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e) {profileChanged; cmbIsASCII();}" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                                <tr id="trSeparator" class="trSeparator" runat="server">
                                                                    <td align="right">
                                                                        <asp:Label ID="lblExportSeparator" runat="server" Text="Separador" Visible="true"></asp:Label>
                                                                    </td>
                                                                    <td align="left" style="padding-left: 10px;">
                                                                        <dx:ASPxTextBox ID="txtExportSeparator" runat="server" Width="30px" MaxLength="1" Visible="true">
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr id="beginDateTr">
                                                                    <td align="right">
                                                                        <asp:Label ID="lblScheduleBeginDate" Text="Exportar desde el día" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td align="left" style="padding-left: 10px;">
                                                                        <dx:ASPxDateEdit ID="txtScheduleBeginDate" runat="server" ClientInstanceName="txtScheduleBeginDateClient" AllowNull="false" PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="OutsideRight">
                                                                            <ClientSideEvents KeyPress="function(s,e){ return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}'); }" />
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right">
                                                                        <asp:Label ID="lblScheduleEndDate" Text="hasta el día" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td align="left" style="padding-left: 10px;">
                                                                        <dx:ASPxDateEdit ID="txtScheduleEndDate" runat="server" ClientInstanceName="txtScheduleEndDateClient" AllowNull="false" PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="OutsideRight">
                                                                            <ClientSideEvents KeyPress="function(s,e){ return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}'); }" />
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                </tr>
                                                                <tr style="padding-top: 50px">
                                                                    <td align="right">
                                                                        <asp:Label ID="lblConceptsGroups" runat="server" Text="Grupo de saldos:"></asp:Label>
                                                                    </td>
                                                                    <td align="left" style="padding-left: 10px;">
                                                                        <dx:ASPxComboBox ID="cmbConceptsGroups" runat="server" Width="200px" Font-Size="11px" CssClass="editTextFormat" Font-Names="Arial;Verdana;Sans-Serif"
                                                                            IncrementalFilteringMode="Contains" ClientInstanceName="cmbConceptsGroupsClient">
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                                <tr runat="server" id="trApplyLockDate" style="display: none">
                                                                    <td align="right"></td>

                                                                    <td align="left" style="padding-left: 10px;">
                                                                        <dx:ASPxCheckBox ID="ckApplyLockDate" runat="server" MaxLength="" Text="Aplicar fecha de cierre por empleado al finalizar la explortación" ClientInstanceName="ckApplyLockDateClient">
                                                                            <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true)}" />
                                                                        </dx:ASPxCheckBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>

                                                <table id="tbProfile" runat="server" width="100%" style="padding-top: 10px;">
                                                </table>

                                                <table id="tbExcelVersion" runat="server" width="100%" style="padding-top: 10px;">
                                                    <tr>
                                                        <td align="center">
                                                            <input type="checkbox" runat="server" id="chk2007Version" checked="checked" />
                                                            <asp:Label ID="lblVersionExcel" runat="server" Text="Generar fichero compatible con versión Excell 2007 o superior"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>

                                                <asp:HiddenField ID="hdn2007Version" Value="1" runat="server" />
                                                <asp:HiddenField ID="hdnAskOverwritePeriodConfirmation" Value="0" runat="server" />
                                                <asp:HiddenField ID="hdnOverwritePeriod" Value="0" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <!--Fin -->
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
                                                <asp:Label ID="lblStep3Info2" runat="server" Text="El asistente ya dispone de suficientes datos para poder iniciar la exportación. Pulse en Finalizar para iniciar el proceso." />
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
                                <asp:Button ID="btEnd" Text="${Button.End}" runat="server" OnClientClick="PerformValidation(true);return false;" Visible="false" TabIndex="3" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                <%--<asp:Button ID="btEnd" Text="${Button.End}" runat="server" OnClientClick="ExecuteExport(); return false;" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />--%>
                                <asp:Button ID="btResume" Text="" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                            </td>
                            <td>
                                <asp:Button ID="btSaveProfile" Text="Guardar perfil" runat="server" OnClientClick="PerformValidation(false);return false;" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                            </td>
                            <td>
                                <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="closeExport(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                <asp:HiddenField ID="hdnResult" Value="" runat="server" />
                                <asp:HiddenField ID="hdnIDExport" Value="" runat="server" />
                                <asp:HiddenField ID="hdnConcept" Value="" runat="server" />
                                <asp:HiddenField ID="hdnFileType" Value="" runat="server" />
                                <asp:HiddenField ID="hdnExcelTemplateFile" Value="" runat="server" />
                                <asp:HiddenField ID="hdnProfileType" Value="" runat="server" />
                                <asp:HiddenField ID="hdnApplyCloseDate" Value="" runat="server" />
                            </td>
                        </tr>
                    </table>

                    <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
                    <input type="hidden" id="hdnIDEmployeeSource" value="" runat="server" />
                    <input type="hidden" id="hdnFrames" value="" runat="server" />
                    <input type="hidden" id="hdnFramesOnlyClient" value="" runat="server" />
                    <input type="hidden" id="hdnFileExport" value="0" runat="server" />
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
    </form>
</body>
</html>