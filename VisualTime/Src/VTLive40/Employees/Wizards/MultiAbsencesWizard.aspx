<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_MultiAbsencesWizard" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableViewState="True" CodeBehind="MultiAbsencesWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="~/Employees/WebUserControls/EmployeeType.ascx" TagName="EmployeeType" TagPrefix="Local" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para el alta de múltiples ausencias previstas</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmMultiAbsencesWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            var bolLoaded = false;

            async function PageBase_Load() {
                if (!bolLoaded) {
                    await getroTreeState('objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizard').then(roState => roState.reset());
                    await getroTreeState('objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizardGrid').then(roState => roState.reset());
                    await getroTreeState('objContainerTreeV3_treeMultiEmployeeMobilityGroupWizard').then(roState => roState.reset());
                }
                var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                ConvertControls('divStep' + oActiveFrameIndex);
                if (oActiveFrameIndex == 2) CheckAbsenceType();

                if (!bolLoaded) bolLoaded = true;

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

            function CheckFrame() {
                var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;

                if (oActiveFrameIndex == "1") {
                    if (opAbsenceDaysClient.GetValue() == null && opOvertimeHoursClient.GetValue() == null && opAbsenceHoursClient.GetValue() == null) {
                        opAbsenceDaysClient.SetValue(true);
                        if (txtBeginDateClient.GetValue() == null) txtBeginDateClient.SetValue(new Date());

                        CheckAbsenceType(opAbsenceDaysClient, null);

                    }
                }

                if (CheckConvertControls('divStep' + oActiveFrameIndex) == false) {
                    return false;
                }
                else {
                    return true;
                }

            }

            function devKeyPress(s, e) {
                return onEnterPress(e, 'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');
            }

            function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
                var hdnEmployeesSelected = document.getElementById('<%= me.hdnEmployeesSelected.ClientID %>');
                hdnEmployeesSelected.value = oParm1;

                var hdnFilter = document.getElementById('<%= me.hdnFilter.ClientID %>');
                hdnFilter.value = oParm2;

                var hdnFilterUser = document.getElementById('<%= me.hdnFilterUser.ClientID %>');
                hdnFilterUser.value = oParm3;
            }

            function ClearEndDate(s, e) {
                if (txtMaxDaysClient.GetValue() != "0") {
                    //txtEndDateClient.SetValue(null);
                }
            }
            function ClearMaxDays(s, e) {
                if (txtEndDateClient.GetValue() != null) {
                    txtMaxDaysClient.SetValue("0");
                }
            }

            //function changeAbsenceTabs(numTab) {
            //    var AbsArrButtons = new Array('TABBUTTON_ABS00', 'TABBUTTON_ABS01');
            //    var AbsArrDivs = new Array('Content_ABS00', 'Content_ABS01');

            //    if (parseInt(document.getElementById("hdnIdAbsence").value, 10) < 0 && numTab == 1) {

            //        showErrorPopup("Info.SaveAbsence", "INFO", "Info.AbsenceMustBeSaved", "", "Info.OK", "Info.OKDesc", "if(typeof window.frames[1].redirectToSave != 'undefined'){ window.frames[1].redirectToSave(); } else { window.top.frames[1].frames[1].redirectToSave(); }", "Info.CancelDoc", "Info.CancelDocDesc", "");
            //    } else {

            //        for (n = 0; n < AbsArrButtons.length; n++) {
            //            var tab = document.getElementById(AbsArrButtons[n]);
            //            var div = document.getElementById(AbsArrDivs[n]);
            //            if (tab != null && div != null) {
            //                if (n == numTab) {
            //                    tab.className = 'bTab-active';
            //                    div.style.display = '';
            //                } else {
            //                    tab.className = 'bTab';
            //                    div.style.display = 'none';
            //                }
            //            }
            //        }
            //    }
            //}

            function redirectActionResult() {
                __doPostBack('btnActionResult', '');
            }

            function showErrorPopup(Title, typeIcon, Description, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
                try {
                    var url = "../Employees/srvMsgBoxEmployees.aspx?action=Message";
                    url = url + "&TitleKey=" + Title;

                    if (Description != "") { url = url + "&DescriptionKey=" + Description; }
                    else { url = url + "&DescriptionText=" + DescriptionText; }

                    url = url + "&Option1TextKey=" + Opt1Text;
                    url = url + "&Option1DescriptionKey=" + Opt1Desc;
                    url = url + "&Option1OnClickScript=HideMsgBoxForm();" + strScript1 + "; return false;";
                    if (Opt2Text != null) {
                        url = url + "&Option2TextKey=" + Opt2Text;
                        url = url + "&Option2DescriptionKey=" + Opt2Desc;
                        url = url + "&Option2OnClickScript=HideMsgBoxForm();" + strScript2 + "; return false;";
                    }
                    if (Opt3Text != null) {
                        url = url + "&Option3TextKey=" + Opt3Text;
                        url = url + "&Option3DescriptionKey=" + Opt3Desc;
                        url = url + "&Option3OnClickScript=HideMsgBoxForm();" + strScript3 + "; return false;";
                    }
                    if (typeIcon.toUpperCase() == "TRASH") {
                        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
                    } else if (typeIcon.toUpperCase() == "ERROR") {
                        url = url + "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
                    } else if (typeIcon.toUpperCase() == "INFO") {
                        url = url + "&IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
                    }

                    if (typeof parent.parent.ShowMsgBoxForm != 'undefined') parent.parent.ShowMsgBoxForm(url, 400, 300, '');
                    else parent.ShowMsgBoxForm(url, 400, 300, '');
                } catch (e) { showError("showErrorPopup", e); }
            }

            function CloseAndRefresh() {
                if (typeof window.parent.RefreshScreen != 'undefined') window.parent.RefreshScreen('1');
                Close();
            }

            var monitor = -1;

            function showCaptcha() {
                var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=MASSABS";
                CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                CaptchaObjectPopup_Client.Show();
            }

            function captchaCallback(action) {
                switch (action) {
                    case "MASSABS":
                        AspxLoadingPopup_Client.Show();
                        PerformAction();
                        break;
                    case "ERROR":
                        showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "", "");
                        break;
                }
            }

            function PerformValidation() {
                PerformActionCallbackClient.PerformCallback("VALIDATE");
            }

            function PerformAction() {
                var iDays = 0;
                if (opAbsenceDaysClient.GetValue() == true) iDays = 1;
                if (opOvertimeHoursClient.GetValue() == true) iDays = 2;

                PerformActionCallbackClient.PerformCallback("PERFORM_ACTION#" + iDays);
            }

            function PerformActionCallback_CallbackComplete(s, e) {
                if (s.cpAction == "VALIDATE") {
                    if (s.cpResult == true) {
                        showCaptcha();
                    } else {
                        showErrorPopup("Error.DatesPeriod", "ERROR", "Error.DatesPeriodDesc", "Error.OK", "", "");
                    }
                } else if (s.cpAction == "PERFORM_ACTION") {
                    monitor = setInterval(function () { PerformActionCallbackClient.PerformCallback("CHECKPROGRESS"); }, 5000);
                } else if (s.cpAction == "ERROR") {
                    clearInterval(monitor);
                    AspxLoadingPopup_Client.Hide();
                    redirectActionResult();
                } else if (s.cpAction == "CHECKPROGRESS") {
                    if (s.cpActionResult == "OK") {
                        clearInterval(monitor);
                        AspxLoadingPopup_Client.Hide();
                        redirectActionResult();
                    }
                }

            }

            function CheckAbsenceType(s, e) {
                txtNormalHourBeginClient.SetEnabled(false);
                txtNormalHourEndClient.SetEnabled(false);
                txtMinDurationClient.SetEnabled(false);
                txtDurationClient.SetEnabled(false);
                txtMaxDaysClient.SetEnabled(false);
                chkEndDateClient.SetEnabled(false);
                chkMaxDaysClient.SetEnabled(false);
                txtEndDateClient.SetEnabled(false);

                if (opAbsenceDaysClient.GetChecked() == true) {

                    chkEndDateClient.SetEnabled(true);
                    if (chkEndDateClient.GetChecked() == false && chkMaxDaysClient.GetChecked() == false) {
                        chkEndDateClient.SetChecked(true);
                        txtEndDateClient.SetValue(new Date());
                    }
                    txtEndDateClient.SetEnabled(true);

                    txtMaxDaysClient.SetEnabled(true);
                    chkMaxDaysClient.SetEnabled(true);
                } else if (opOvertimeHoursClient.GetChecked() == true || opAbsenceHoursClient.GetChecked() == true) {
                    chkEndDateClient.SetChecked(true);
                    chkEndDateClient.SetEnabled(true);
                    txtEndDateClient.SetEnabled(true);

                    txtNormalHourBeginClient.SetEnabled(true);
                    txtNormalHourEndClient.SetEnabled(true);
                    txtMinDurationClient.SetEnabled(true);
                    if (opOvertimeHoursClient.GetChecked() == true) endHourChanged();
                    if (opAbsenceHoursClient.GetValue() == true) txtDurationClient.SetEnabled(true);
                }
            }

            function padTo2Digits(num) {
                return num.toString().padStart(2, '0');
            }

            function convertMsToHM(milliseconds) {
                let seconds = Math.floor(milliseconds / 1000);
                let minutes = Math.floor(seconds / 60);
                let hours = Math.floor(minutes / 60);

                seconds = seconds % 60;
                minutes = seconds >= 30 ? minutes + 1 : minutes;
                minutes = minutes % 60;

                hours = hours % 24;

                return `${padTo2Digits(hours)}:${padTo2Digits(minutes)}`;
            }

            function endHourChanged(s, e) {
                var inithourDate = moment(txtNormalHourBeginClient.GetValue());
                var startHour = inithourDate.format("HH:mm");
                var endHourDate = moment(txtNormalHourEndClient.GetValue());
                var endHour = endHourDate.format("HH:mm");

                var strToShift = '1970-01-';
                var strFromShift = '1970-01-30 ';

                if (inithourDate > endHourDate) strToShift += '31 ';
                else strToShift += '30 ';

                if (opOvertimeHoursClient.GetChecked() == true) {
                    var fromTime = moment(strFromShift + startHour, "YYYY-MM-DD HH:mm");
                    var toTime = moment(strToShift + endHour, "YYYY-MM-DD HH:mm");

                    txtDurationClient.SetValue(moment(strFromShift + convertMsToHM(toTime.diff(fromTime)), "YYYY-MM-DD HH:mm").toDate());

                    if (txtMinDurationClient.GetValue() == null) {
                        txtMinDurationClient.SetValue(moment(strFromShift + "00:00", "YYYY-MM-DD HH:mm").toDate());
                    }
                } else if (opAbsenceHoursClient.GetChecked() == true) {
                    if (txtDurationClient.GetValue() == null) {
                        txtDurationClient.SetValue(moment(strFromShift + "00:00", "YYYY-MM-DD HH:mm").toDate());
                    }
                    if (txtMinDurationClient.GetValue() == null) {
                        txtMinDurationClient.SetValue(moment(strFromShift + "00:00", "YYYY-MM-DD HH:mm").toDate());
                    }
                }
            }
        </script>

        <asp:Label ID="hdnStepTitle" Text="Asistente para dar de alta múltiples ausencias previstas. " runat="server" Style="display: none; visibility: hidden" />
        <asp:Label ID="hdnStepTitle2" Text="Paso {0} de {1}." runat="server" Style="display: none; visibility: hidden" />

        <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>

        <div class="popupWizardContent">
            <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep0" runat="server" style="display: block;">
                        <table id="tbStep0" style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td style="height: 360px">
                                    <asp:Image ID="imgNewMultiAbsencesWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzmenslong.gif" />
                                </td>
                                <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">
                                    <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para introducción masiva de ausencias"
                                        Font-Bold="True" Font-Size="Large"></asp:Label>
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le ayudará a realizar múltiples ausencias."
                                        Font-Bold="true"></asp:Label>
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
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep1" runat="server" style="display: none; width: 500px;">
                        <table style="width: 790px; height: 410px;" cellspacing="0" cellpadding="0">
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
                                <td class="NewEmployeeWizards_StepContent">

                                    <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep1Info2" runat="server" Text="Ahora seleccione los ${Employees} sobre los que desea realizar la introducción de ausencias." />
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
                            <tr>
                                <td class="NewEmployeeWizards_StepError">
                                    <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
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
                    <div id="divStep2" runat="server" style="display: none; width: 500px;">
                        <table style="width: 790px;" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep2Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblSetp2Info" runat="server" Text="" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <asp:HiddenField ID="hdnIdAbsence" runat="server" />

                                    <table style="width: 98%; height: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep2Info2" runat="server" Text="Ahora introduzca las ausencias." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px; padding-top: 5px" valign="top">
                                                <div style="width: 100%; height: 400px; padding-top: 0px;" class="DetailFrame_TopMid">
                                                    <div style="min-height: 385px">

                                                        <table cellpadding="1" cellspacing="1" border="0" style="height: 30px;">
                                                            <tr>
                                                                <td style="min-width: 150px; text-align: right;">
                                                                    <asp:Label ID="lblCause" runat="server" Text="Justificación"></asp:Label>
                                                                </td>
                                                                <td align="left" style="padding-left: 10px">
                                                                    <dx:ASPxComboBox ID="cmbCausesList" runat="server" Width="250" AutoPostBack="true" OnSelectedIndexChanged="cmbCausesList_ValueChanged" />
                                                                </td>
                                                            </tr>
                                                        </table>

                                                        <table cellpadding="1" cellspacing="1" border="0" style="height: 30px;">
                                                            <tr>
                                                                <td style="min-width: 150px; text-align: right;">
                                                                    <asp:Label ID="lblBeginDate" runat="server" Text="Fecha de inicio"></asp:Label>
                                                                </td>
                                                                <td align="left" style="padding-left: 10px">
                                                                    <dx:ASPxDateEdit runat="server" ID="txtBeginDate" PopupVerticalAlign="WindowCenter" Width="150" ClientInstanceName="txtBeginDateClient">
                                                                        <CalendarProperties ShowClearButton="false" />
                                                                    </dx:ASPxDateEdit>
                                                                </td>
                                                            </tr>
                                                        </table>

                                                        <table cellpadding="1" cellspacing="1" border="0">
                                                            <tr>
                                                                <td style="min-width: 150px; text-align: right;" valign="top">
                                                                    <asp:Label ID="lblDescription" runat="server" Text="Descripción"></asp:Label>
                                                                </td>
                                                                <td align="left" style="padding-left: 10px; width: 480px;">
                                                                    <dx:ASPxMemo ID="txtDescription" runat="server" Width="100%" MaxLength="250" Rows="3" />
                                                                    <%--<asp:TextBox ID="txtDescription" runat="server" Width="100%" Height="70px" MaxLength="250" TextMode="MultiLine" class="textClass x-form-text x-form-field x-form-num-field"></asp:TextBox>--%>
                                                                </td>
                                                            </tr>
                                                            <tr style="height: 10px;">
                                                                <td colspan="2">&nbsp;</td>
                                                            </tr>
                                                        </table>

                                                        <table cellpadding="1" cellspacing="1" border="0" style="width: 100%">
                                                            <tr>
                                                                <td>
                                                                    <div>
                                                                        <div>
                                                                            <table cellpadding="1" cellspacing="1" border="0">
                                                                                <tr>
                                                                                    <td style="min-width: 150px; text-align: right; padding-top: 10px" valign="top">
                                                                                        <asp:Label ID="lblForecastType" runat="server" Text="Tipo de previsión"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px; width: 480px;">
                                                                                        <div>
                                                                                            <dx:ASPxRadioButton ClientInstanceName="opAbsenceDaysClient" ID="opAbsenceDays" runat="server" Text="Ausencia por días" GroupName="rbAbsenceType">
                                                                                                <ClientSideEvents ValueChanged="CheckAbsenceType" />
                                                                                            </dx:ASPxRadioButton>
                                                                                        </div>
                                                                                        <div>
                                                                                            <dx:ASPxRadioButton ClientInstanceName="opAbsenceHoursClient" ID="opAbsenceHours" runat="server" Text="Ausencia por horas" GroupName="rbAbsenceType">
                                                                                                <ClientSideEvents ValueChanged="CheckAbsenceType" />
                                                                                            </dx:ASPxRadioButton>
                                                                                        </div>
                                                                                        <div>
                                                                                            <dx:ASPxRadioButton ClientInstanceName="opOvertimeHoursClient" ID="opOvertimeHours" runat="server" Text="Horas de exceso" GroupName="rbAbsenceType">
                                                                                                <ClientSideEvents ValueChanged="CheckAbsenceType" />
                                                                                            </dx:ASPxRadioButton>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                        <div style="clear: both">
                                                                            <table cellpadding="1" cellspacing="1" border="0" style="height: 30px;">
                                                                                <tr>
                                                                                    <td style="min-width: 150px; text-align: right;">
                                                                                        <asp:Label ID="lblEndDate" runat="server" Text="Fecha de finalizción"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px">
                                                                                        <dx:ASPxRadioButton GroupName="EndGroup" ID="chkEndDate" ClientInstanceName="chkEndDateClient" runat="server" Checked="false" Text="Fecha final concreta" />
                                                                                    </td>
                                                                                    <td style="padding-left: 10px" colspan="2">
                                                                                        <dx:ASPxDateEdit runat="server" ID="txtEndDate" PopupVerticalAlign="WindowCenter" Width="150" ClientInstanceName="txtEndDateClient">
                                                                                            <ClientSideEvents DateChanged="ClearMaxDays" />
                                                                                        </dx:ASPxDateEdit>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="min-width: 150px; text-align: right;">&nbsp;</td>
                                                                                    <td align="left" style="padding-left: 10px">
                                                                                        <dx:ASPxRadioButton GroupName="EndGroup" ID="chkMaxDays" ClientInstanceName="chkMaxDaysClient" runat="server" Checked="false" Text="Final abierto, durante máximo" />
                                                                                    </td>
                                                                                    <td style="padding-left: 10px; padding-right: 10px">
                                                                                        <dx:ASPxTextBox runat="server" ID="txtMaxDays" Width="150" ClientInstanceName="txtMaxDaysClient">
                                                                                            <MaskSettings Mask="<0..999>" IncludeLiterals="DecimalSymbol" />
                                                                                            <ClientSideEvents TextChanged="ClearEndDate" />
                                                                                            <ValidationSettings Display="None" />
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:Label ID="lblDays" runat="server" Text="días"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                        <div style="clear: both">
                                                                            <table cellpadding="1" cellspacing="1" border="0" style="height: 30px;">
                                                                                <tr>
                                                                                    <td style="min-width: 150px; text-align: right;">
                                                                                        <asp:Label ID="lblperiodTime" runat="server" Text="Entre "></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px">
                                                                                        <dx:ASPxTimeEdit ID="txtNormalHourBegin" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtNormalHourBeginClient" runat="server" Width="85">
                                                                                            <ClientSideEvents DateChanged="endHourChanged" />
                                                                                        </dx:ASPxTimeEdit>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px">
                                                                                        <asp:Label ID="lblperiodTimeFinish" runat="server" Text=" y las "></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px">
                                                                                        <dx:ASPxTimeEdit ID="txtNormalHourEnd" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtNormalHourEndClient" runat="server" Width="85">
                                                                                            <ClientSideEvents DateChanged="endHourChanged" />
                                                                                        </dx:ASPxTimeEdit>
                                                                                    </td>
                                                                                    <td align="right" style="padding-left: 10px">
                                                                                        <asp:Label ID="txtNormalHourLast" runat="server" Text=" inclusive."></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <table cellpadding="1" cellspacing="1" border="0" style="height: 30px;">
                                                                                <tr>
                                                                                    <td style="min-width: 150px; text-align: right;">
                                                                                        <asp:Label ID="lblMinTime" runat="server" Text="Duración mínima "></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px">
                                                                                        <dx:ASPxTimeEdit ID="txtMinDuration" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtMinDurationClient" runat="server" Width="85" />
                                                                                    </td>
                                                                                    <td style="padding-left: 10px;">
                                                                                        <asp:Label ID="lblMaxTime" runat="server" Text="Duración máxima "></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px">
                                                                                        <dx:ASPxTimeEdit ID="txtDuration" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtDurationClient" runat="server" Width="85" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                    </div>

                                                                    <div>
                                                                    </div>

                                                                    <%--<roUserControls:roOptionPanelClient ID="opAbsenceDays" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="True" Enabled="True" CConClick="AbsenceTypeChange();">
                                                                        <Title>
                                                                            <asp:Label ID="lblAbsenceDays" runat="server" Text="Ausencia por días"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>--%>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="padding-bottom: 10px;">
                                                                    <%--<roUserControls:roOptionPanelClient ID="opAbsenceHours" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="AbsenceTypeChange();">
                                                                        <Title>
                                                                            <asp:Label ID="lblAbsenceHours" runat="server" Text="Ausencia por horas"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>--%>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError">
                                    <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
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
                    <asp:AsyncPostBackTrigger ControlID="btnActionResult" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep3" runat="server" style="display: none; width: 500px;">
                        <table style="width: 790px; height: 410px;" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep3Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px; padding-right: 40px">
                                                <asp:Label ID="lblStep3Info" runat="server" Text="" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">

                                    <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td style="height: 100%; vertical-align: top;">
                                                <asp:Label ID="lblResume" runat="server" Text="Ya disponemos de todos los datos necesarios para crear la ausencia" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError">
                                    <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
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
                                <asp:Button ID="btEnd" Text="${Button.End}" runat="server" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" OnClientClick="PerformValidation();return false;" />
                            </td>
                            <td>
                                <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                <asp:Button ID="btnActionResult" Text="" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
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

        <Local:MessageFrame ID="MessageForm" runat="server" />
    </form>
</body>
</html>