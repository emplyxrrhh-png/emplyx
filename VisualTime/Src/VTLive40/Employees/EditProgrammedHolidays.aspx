<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_EditProgrammedHolidays" EnableViewState="True" CodeBehind="EditProgrammedHolidays.aspx.vb" %>

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

                venableOPC('optCompleteDay');
                venableOPC('optPanelOnlyHours');
                linkOPCItems('optCompleteDay,optPanelOnlyHours');
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
        </script>

        <asp:HiddenField ID="hdnIdEmployee" runat="server" />
        <asp:HiddenField ID="hdnBeginDate" runat="server" />
        <asp:HiddenField ID="hdnIdCause" runat="server" />
        <asp:HiddenField ID="hdnIdAbsence" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div style="width: 100%; height: 440px; padding-top: 0px;" class="DetailFrame_TopMid">
                    <div style="min-height: 385px">
                        <table cellpadding="1" cellspacing="1" border="0" style="width: 100%;">
                            <tr>
                                <td>
                                    <div class="panHeader2">
                                        <asp:Label ID="lblTitle" Text="Vacaciones por horas previstas" Font-Bold="true" runat="server" CssClass="panHeaderLabel" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="divEmployee-Class">
                                        <table border="0" width="100%" height="50px">
                                            <tr>
                                                <td width="48px" height="48px">
                                                    <img alt="" id="Img8" src="~/Base/Images/Employees/ProgrammedHolidays.png" style="border: 0;" runat="server" /></td>
                                                <td valign="middle" align="left">
                                                    <span id="span3" runat="server" class="spanEmp-Class">
                                                        <asp:Label ID="lblInfo" runat="server" CssClass="editTextFormat"
                                                            Text="Aquí puede indicar los datos de las previsiones de vacaciones por horas."></asp:Label>
                                                    </span>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="1" cellspacing="1" border="0" style="height: 30px;">
                            <tr>
                                <td style="min-width: 97px; text-align: right;">
                                    <asp:Label ID="lblCause" runat="server" Text="Justificación"></asp:Label>
                                </td>
                                <td align="left" style="padding-left: 10px">
                                    <dx:ASPxComboBox ID="cmbCausesList" runat="server" Width="250" AutoPostBack="true" OnSelectedIndexChanged="cmbCausesList_ValueChanged" />
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
                                <td style="min-width: 97px; text-align: right;">
                                    <asp:Label ID="lblEndDate" runat="server" Text="Fecha final"></asp:Label>
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
                                <td style="min-width: 97px; text-align: right; vertical-align: top; padding-top: 5px;">
                                    <asp:Label ID="lblPeriodDuration" runat="server" Text="Duración"></asp:Label>
                                </td>
                                <td style="padding-left: 10px;">
                                    <roUserControls:roOptionPanelClient ID="optCompleteDay" runat="server" TypeOPanel="RadioOption" Width="600px" Height="Auto" Checked="false" Enabled="True" Border="True" Value="0" CConClick="">
                                        <Title>
                                            <asp:Label ID="lbloptCompleteDay" runat="server" Text="Día completo"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lbloptCompleteDayDesc" runat="server" Text="Se solicitan vacaciones por el total de horas teóricas del día"></asp:Label>
                                        </Description>
                                        <Content>
                                            <table style="width: 600px">
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>
                                            </table>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                </td>
                            </tr>
                        </table>
                        <table>
                            <tr>
                                <td style="min-width: 97px; text-align: right;">&nbsp;</td>
                                <td style="padding-left: 10px;">
                                    <roUserControls:roOptionPanelClient ID="optPanelOnlyHours" runat="server" TypeOPanel="RadioOption" Width="600px" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="">
                                        <Title>
                                            <asp:Label ID="lbloptPanelOnlyHours" runat="server" Text="Periodo"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lbloptPanelOnlyHoursDesc" runat="server" Text="Se solicitan vacaciones solo por el total de horas comprendido en el periodo seleccionado"></asp:Label>
                                        </Description>
                                        <Content>
                                            <div style="float: left; width: 600px; padding-top: 5px;">
                                                <div style="float: left; padding-top: 5px; padding-left: 25px">
                                                    <asp:Label ID="lblperiodTime" runat="server" Text="Entre " CssClass="editTextFormat"></asp:Label>
                                                </div>
                                                <div style="float: left; padding-left: 10px">
                                                    <dx:ASPxTimeEdit ID="txtNormalHourBegin" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtNormalHourBegin_Client">
                                                        <ClientSideEvents ValueChanged="endHourChanged" />
                                                    </dx:ASPxTimeEdit>
                                                </div>
                                                <div style="float: left; padding-left: 10px; padding-top: 5px">
                                                    <asp:Label ID="lblperiodTimeFinish" runat="server" Text=" y las " CssClass="editTextFormat"></asp:Label>
                                                </div>
                                                <div style="float: left; padding-left: 10px">
                                                    <dx:ASPxTimeEdit ID="txtNormalHourEnd" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtNormalHourEnd_Client">
                                                        <ClientSideEvents ValueChanged="endHourChanged" />
                                                    </dx:ASPxTimeEdit>
                                                </div>
                                                <div style="float: left; padding-left: 10px; padding-top: 5px">
                                                    <asp:Label ID="txtNormalHourLast" runat="server" Text=" inclusive." CssClass="editTextFormat"></asp:Label>
                                                </div>
                                                <div style="float: left; padding-left: 10px; padding-top: 5px">
                                                    <asp:Label ID="lblOpen" runat="server" Text="(" CssClass="editTextFormat"></asp:Label>
                                                    <asp:Label ID="lblMaxTime" runat="server" Text="Duración" CssClass="editTextFormat"></asp:Label>
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 5px;">
                                                    <dx:ASPxLabel ID="txtDuration" runat="server" ClientInstanceName="txtDuration_Client" Font-Size="11px" CssClass="editTextFormat" />
                                                    <asp:Label ID="lblClose" runat="server" Text=")" CssClass="editTextFormat"></asp:Label>
                                                </div>
                                            </div>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
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
                    <div style="">
                        <table cellpadding="1" cellspacing="1" border="0" style="height: 30px; width: 100%;">
                            <tr style="height: 40px">
                                <td></td>
                                <td align="right" style="padding-right: 20px;">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btOK" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
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