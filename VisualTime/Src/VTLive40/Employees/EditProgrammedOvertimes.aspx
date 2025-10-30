<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_EditProgrammedOvertimes" EnableViewState="True" CodeBehind="EditProgrammedOvertimes.aspx.vb" EnableEventValidation="false" %>

<%@ Register Src="~/Employees/WebUserControls/DocumentManagment.ascx" TagPrefix="roForms" TagName="DocumentManagment" %>
<%@ Register Src="~/Employees/WebUserControls/DocumentPendingManagment.ascx" TagPrefix="roForms" TagName="DocumentPendingManagment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ausencia Prolongada</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {
                ConvertControls();
            }

            function ClearEndDate() {
            }
            function ClearMaxDays() {
            }

            function endHourChanged(s, e) {
                var inithourDate = moment(txtNormalHourBegin_Client.GetValue());
                var startHour = inithourDate.format("HH:mm");
                var endHourDate = moment(txtNormalHourEnd_Client.GetValue());
                var endHour = endHourDate.format("HH:mm");

                var strToShift = '1970-01-';
                var strFromShift = '1970-01-30 ';

                if (inithourDate > endHourDate) strToShift += '31 ';
                else strToShift += '30 ';

                var fromTime = moment(strFromShift + startHour, "YYYY-MM-DD HH:mm");
                var toTime = moment(strToShift + endHour, "YYYY-MM-DD HH:mm");

                txtDuration_Client.SetText(moment.utc(toTime.diff(fromTime)).format("HH:mm"));
            }

            function changeOvertimesTabs(numTab) {
                var AbsArrButtons = new Array('TABBUTTON_ABS00', 'TABBUTTON_ABS01');
                var AbsArrDivs = new Array('Content_ABS00', 'Content_ABS01');

                if (parseInt(document.getElementById("hdnIdProgrammedOvertime").value, 10) < 0 && numTab == 1) {

                    showErrorPopup("Info.SaveOvertime", "INFO", "Info.OvertimeMustBeSaved", "", "Info.OK", "Info.OKDesc", "if(typeof window.frames[1].redirectToSave != 'undefined'){ window.frames[1].redirectToSave(); } else { window.top.frames[1].frames[1].redirectToSave(); }", "Info.CancelDoc", "Info.CancelDocDesc", "");
                } else {

                    for (n = 0; n < AbsArrButtons.length; n++) {
                        var tab = document.getElementById(AbsArrButtons[n]);
                        var div = document.getElementById(AbsArrDivs[n]);
                        if (tab != null && div != null) {
                            if (n == numTab) {
                                tab.className = 'bTab-active';
                                div.style.display = '';
                            } else {
                                tab.className = 'bTab';
                                div.style.display = 'none';
                            }
                        }
                    }
                }
            }

            function reloadparent() {
                __doPostBack('btnReload', '');
            }

            function redirectToSave() {
                __doPostBack('btnSaveAndEdit', '');
            }

            function showErrorPopup(Title, typeIcon, Description, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
                try {
                    var url = "./Scheduler/srvMsgBoxScheduler.aspx?action=Message";
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
        </script>

        <asp:HiddenField ID="hdnIdEmployee" runat="server" />
        <asp:HiddenField ID="hdnIdCause" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:HiddenField ID="hdnIdProgrammedOvertime" runat="server" />
                <div style="width: 100%; height: 370px; padding-top: 0px;" class="DetailFrame_TopMid">
                    <div style="min-height: 315px">
                        <table cellpadding="1" cellspacing="1" border="0" style="width: 100%">
                            <tr>
                                <td>
                                    <div class="panHeader2">
                                        <asp:Label ID="lblTitle" Text="Horas de exceso" Font-Bold="true" runat="server" CssClass="panHeaderLabel" />
                                    </div>
                                </td>
                            </tr>
                        </table>

                        <div style="width: 100%; margin-left: auto; margin-right: auto; padding-top: 8px;">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div class="RoundCornerFrame">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <a id="TABBUTTON_ABS00" href="javascript: void(0);" runat="server" class="bTab-active" onclick="javascript: changeOvertimesTabs(0);" style="margin: -1px;">
                                                            <%=Me.Language.Translate("tabAbsenceDefinition", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_ABS01" href="javascript: void(0);" runat="server" class="bTab" onclick="javascript: changeOvertimesTabs(1);">
                                                            <%=Me.Language.Translate("tabAbsenceDocuments", Me.DefaultScope)%></a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="Content_ABS00" runat="server" style="width: calc(100% - 7px); margin-top: 5px; margin-left: 1px; border-radius: 5px;">
                            <table cellpadding="1" cellspacing="1" border="0" style="height: 30px;">
                                <tr>
                                    <td style="min-width: 97px; text-align: right;">
                                        <asp:Label ID="lblCause" runat="server" Text="Justificación"></asp:Label>
                                    </td>
                                    <td align="left" style="padding-left: 10px">
                                        <dx:ASPxComboBox ID="cmbCausesList" AutoPostBack="true" runat="server" Width="250" />
                                    </td>
                                </tr>
                            </table>
                            <table cellpadding="1" cellspacing="1" border="0" style="height: 35px;">
                                <tr>
                                    <td style="min-width: 97px; text-align: right;">
                                        <asp:Label ID="lblBeginDate" runat="server" Text="Fecha inicio"></asp:Label>
                                    </td>
                                    <td align="left" style="padding-left: 10px">
                                        <dx:ASPxDateEdit ID="txtBeginDate" runat="server" Width="150" PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="OutsideRight">
                                            <CalendarProperties ShowClearButton="false" />
                                        </dx:ASPxDateEdit>
                                    </td>
                                    <td style="padding-left: 11px;">
                                        <asp:Label ID="lblEndDate" runat="server" Text="Fecha fin"></asp:Label>
                                    </td>
                                    <td align="left" style="padding-left: 10px">
                                        <dx:ASPxDateEdit ID="txtEndDate" runat="server" Width="150" PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="OutsideRight">
                                            <CalendarProperties ShowClearButton="false" />
                                        </dx:ASPxDateEdit>
                                    </td>
                                </tr>
                            </table>
                            <table cellpadding="1" cellspacing="1" border="0" style="height: 30px;">
                                <tr>
                                    <td style="min-width: 97px; text-align: right;">
                                        <asp:Label ID="lblperiodTime" runat="server" Text="Entre "></asp:Label>
                                    </td>
                                    <td align="left" style="padding-left: 10px">
                                        <dx:ASPxTimeEdit ID="txtNormalHourBegin" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtNormalHourBegin_Client">
                                            <ClientSideEvents ValueChanged="endHourChanged" />
                                        </dx:ASPxTimeEdit>
                                    </td>
                                    <td align="left" style="padding-left: 10px">
                                        <asp:Label ID="lblperiodTimeFinish" runat="server" Text=" y las "></asp:Label>
                                    </td>
                                    <td align="left" style="padding-left: 10px">
                                        <dx:ASPxTimeEdit ID="txtNormalHourEnd" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtNormalHourEnd_Client">
                                            <ClientSideEvents ValueChanged="endHourChanged" />
                                        </dx:ASPxTimeEdit>
                                    </td>
                                    <td align="right" style="padding-left: 10px">
                                        <div style="float: left; padding-left: 10px;">
                                            <asp:Label ID="txtNormalHourLast" runat="server" Text=" inclusive."></asp:Label>
                                            <asp:Label ID="lblOpen" runat="server" Text="("></asp:Label>
                                            <asp:Label ID="lblMaxTime" runat="server" Text="Duración máxima"></asp:Label>
                                        </div>
                                        <div style="float: left; padding-left: 5px;">
                                            <dx:ASPxLabel ID="txtDuration" runat="server" ClientInstanceName="txtDuration_Client" Font-Size="11px" />
                                            <asp:Label ID="lblClose" runat="server" Text=")"></asp:Label>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table cellpadding="1" cellspacing="1" border="0" style="height: 30px;">
                                <tr>
                                    <td style="min-width: 97px; text-align: right;">
                                        <asp:Label ID="lblMinTime" runat="server" Text="Duración mínima "></asp:Label>
                                    </td>
                                    <td align="left" style="padding-left: 10px">
                                        <dx:ASPxTimeEdit ID="txtMinDuration" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr valign="top">
                                    <td style="min-width: 97px; text-align: right; padding-top: 5px;">
                                        <asp:Label ID="lblDescription" runat="server" Text="Descripción"></asp:Label>
                                    </td>
                                    <td align="left" style="padding-top: 5px; padding-left: 10px; width: 480px;">
                                        <dx:ASPxMemo ID="txtDescription" runat="server" Rows="10" Width="100%" Height="70px" MaxLength="250">
                                        </dx:ASPxMemo>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="Content_ABS01" runat="server" style="display: none; width: calc(100% - 7px); margin-top: -3px; margin-left: 1px; border-radius: 5px;">
                            <div>
                                <div class="panHeader2">
                                    <span style="">
                                        <asp:Label ID="lblPendingDocs" runat="server" Text="Alertas de documentación"></asp:Label></span>
                                </div>
                                <div class="panBottomMargin" style="clear: both">
                                    <roForms:DocumentPendingManagment runat="server" ID="AbsDocumentPendingManagment" />
                                </div>
                            </div>
                            <div style="clear: both;">
                                <div class="panHeader2">
                                    <span style="">
                                        <asp:Label ID="lblDeliveredDocs" runat="server" Text="Documentos de la ausencia"></asp:Label></span>
                                </div>
                                <div class="panBottomMargin" style="clear: both; padding-top: 15px;">
                                    <roForms:DocumentManagment runat="server" ID="AbsDocumentManagment" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div style="">
                        <table cellpadding="1" cellspacing="1" border="0" style="height: 30px; width: 100%;">
                            <tr style="height: 40px">
                                <td></td>
                                <td align="right" style="padding-right: 20px; padding-top: 20px;">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btOK" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                            <asp:Button ID="btnSaveAndEdit" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" Visible="false" />
                                            <asp:Button ID="btnReload" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" Visible="false" />
                                            <td>
                                                <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                        </tr>
                                    </table>
                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                    <asp:HiddenField ID="hdnParams_PageBase" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <Local:MessageFrame ID="MessageFrame1" runat="server" />
    </form>
</body>
</html>