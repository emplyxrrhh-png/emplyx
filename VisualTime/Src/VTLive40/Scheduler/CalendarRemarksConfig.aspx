<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.CalendarRemarksConfig" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableSessionState="True" CodeBehind="CalendarRemarksConfig.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Configuración resaltes calendario</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmRemarksConfig" runat="server">
        <script language="javascript" type="text/javascript">
            window.onload = PageBase_Load;

            function PageBase_Load() {
                CloseIfNeeded();
            }

            function CloseIfNeeded() {
                var _CanClose = document.getElementById("hdnCanClose");
                if (_CanClose.value == '1') {
                    window.parent.RefreshScreen('fullRefresh');
                    Close();
                }
            }
        </script>

        <div>
            <asp:HiddenField ID="hdnCanClose" runat="server" />

            <!-- Este div es el header -->
            <div class="panBottomMargin">
                <div class="panHeader2 panBottomMargin">
                    <span class="panelTitleSpan">
                        <asp:Label runat="server" ID="LblTitle" Text="Resaltes"></asp:Label>
                    </span>
                </div>
                <!-- La descripción es opcional -->
                <div class="panelHeaderContent">
                    <div class="panelDescriptionImage">
                        <img alt="" src="<%=Me.Page.ResolveUrl("~/Scheduler/Images/SchedulerRemarks.png")%>" />
                    </div>
                    <div class="panelDescriptionText">
                        <asp:Label ID="lblDescription" runat="server" Text="Cree las reglas para resaltar casillas del calendario dependiendo de su contenido."></asp:Label>
                    </div>
                </div>
            </div>
            <!-- Este div es un formulario -->
            <div class="panBottomMargin">
                <div class="divRow">
                    <roUserControls:roTabContainerClient ID="tbCont1" runat="server">
                        <TabTitle1>
                            <asp:Label ID="lblRemark1Title" runat="server" Text="Resalte 1"></asp:Label>
                        </TabTitle1>
                        <TabContainer1>
                            <div style="clear: both; padding: 10px; padding-bottom: 30px">
                                <div style="float: left">
                                    <asp:Label ID="lblRemark1" runat="server" Text="El valor de la justificación"></asp:Label>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxComboBox ID="cmbCauseRemark1" runat="server" ValidationSettings-ErrorDisplayMode="None" ClearButton-DisplayMode="OnHover">
                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxComboBox ID="cmbComparisonRemark1" Width="75px" runat="server" ValidationSettings-ErrorDisplayMode="None">
                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxTimeEdit ID="txtValueRemark1" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" runat="server" ValidationSettings-ErrorDisplayMode="None">
                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxTimeEdit>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxColorEdit ID="txtColorRemark1" runat="server" ValidationSettings-ErrorDisplayMode="None" EnableCustomColors="true" Width="14px">
                                        <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display = 'none';}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxColorEdit>
                                </div>
                            </div>
                        </TabContainer1>
                        <TabTitle2>
                            <asp:Label ID="lblRemark2Title" runat="server" Text="Resalte 2"></asp:Label>
                        </TabTitle2>
                        <TabContainer2>
                            <div style="clear: both; padding: 10px; padding-bottom: 30px">
                                <div style="float: left">
                                    <asp:Label ID="lblRemark2" runat="server" Text="El valor de la justificación."></asp:Label>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxComboBox ID="cmbCauseRemark2" runat="server" ValidationSettings-ErrorDisplayMode="None" ClearButton-DisplayMode="OnHover">
                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxComboBox ID="cmbComparisonRemark2" Width="75px" runat="server" ValidationSettings-ErrorDisplayMode="None">
                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxTimeEdit ID="txtValueRemark2" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" runat="server" ValidationSettings-ErrorDisplayMode="None">
                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxTimeEdit>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxColorEdit ID="txtColorRemark2" runat="server" ValidationSettings-ErrorDisplayMode="None" EnableCustomColors="true" Width="14px">
                                        <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display = 'none';}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxColorEdit>
                                </div>
                            </div>
                        </TabContainer2>
                        <TabTitle3>
                            <asp:Label ID="lblRemark3Title" runat="server" Text="Resalte 3"></asp:Label>
                        </TabTitle3>
                        <TabContainer3>
                            <div style="clear: both; padding: 10px; padding-bottom: 30px">
                                <div style="float: left">
                                    <asp:Label ID="lblRemark3" runat="server" Text="El valor de la justificación."></asp:Label>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxComboBox ID="cmbCauseRemark3" runat="server" ValidationSettings-ErrorDisplayMode="None" ClearButton-DisplayMode="OnHover">
                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxComboBox ID="cmbComparisonRemark3" Width="75px" runat="server" ValidationSettings-ErrorDisplayMode="None">
                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxTimeEdit ID="txtValueRemark3" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" runat="server" ValidationSettings-ErrorDisplayMode="None">
                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxTimeEdit>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <dx:ASPxColorEdit ID="txtColorRemark3" runat="server" ValidationSettings-ErrorDisplayMode="None" EnableCustomColors="true" Width="14px">
                                        <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display = 'none';}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    </dx:ASPxColorEdit>
                                </div>
                            </div>
                        </TabContainer3>
                    </roUserControls:roTabContainerClient>
                </div>
            </div>

            <!-- Este div es el header -->
            <div class="panBottomMargin">
                <div class="panHeader2 panBottomMargin">
                    <span class="panelTitleSpan">
                        <asp:Label runat="server" ID="LblAccruals" Text="Saldos del calendario"></asp:Label>
                    </span>
                </div>
                <!-- La descripción es opcional -->
                <div class="panelHeaderContent">
                    <div class="panelDescriptionImage">
                        <img alt="" width="48" height="48" src="<%=Me.Page.ResolveUrl("~/Concepts/Images/acumulados80.png")%>" />
                    </div>
                    <div class="panelDescriptionText">
                        <asp:Label ID="LblAccrualsDescription" runat="server" Text="Seleccione los saldos que desea visualizar en el calendario"></asp:Label>
                    </div>
                </div>
            </div>

            <!-- Este div es un formulario -->
            <div class="panBottomMargin" style="margin-left: 25px">
                <roUserControls:roGroupBox ID="GroupBox2" runat="server">
                    <Content>
                        <div class="divRow">
                            <div class="divRowDescription mediumPadding">
                                <asp:Label ID="lblAccCalendarDescription" runat="server" Text="Este saldo se mostrará en la columna central del calendario a modo de referéncia al planificar"></asp:Label>
                            </div>
                            <asp:Label ID="lblAccCalendarTitle" runat="server" Text="Calendadrio:" CssClass="labelForm mediumWidth"></asp:Label>
                            <div class="componentForm">
                                <dx:ASPxComboBox ID="cmbCalendarConcept" runat="server">
                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                </dx:ASPxComboBox>
                            </div>
                        </div>

                        <div class="divRow">
                            <div class="divRowDescription mediumPadding">
                                <asp:Label ID="lblAccHolidayDescription" runat="server" Text="Este saldo se utilizará como referéncia en la columna de vacaciones al planificar vacaciones"></asp:Label>
                            </div>
                            <asp:Label ID="lblAccHolidayTitle" runat="server" Text="Vacaciones:" CssClass="labelForm mediumWidth"></asp:Label>
                            <div class="componentForm">
                                <dx:ASPxComboBox ID="cmbCalendarHolidays" runat="server">
                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                </dx:ASPxComboBox>
                            </div>
                        </div>

                        <div class="divRow">
                            <div class="divRowDescription mediumPadding">
                                <asp:Label ID="lblAccWorkingDescription" runat="server" Text="Este saldo se utilizará como horas trabajadas en la barra de progreso de revisión"></asp:Label>
                            </div>
                            <asp:Label ID="lblAccWorkingTitle" runat="server" Text="Trabajado:" CssClass="labelForm mediumWidth"></asp:Label>
                            <div class="componentForm">
                                <dx:ASPxComboBox ID="cmbCalendarWorking" runat="server">
                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                </dx:ASPxComboBox>
                            </div>
                        </div>

                        <div class="divRow">
                            <div class="divRowDescription mediumPadding">
                                <asp:Label ID="lblAccOvertimeDescription" runat="server" Text="Este saldo se utilizará como horas extras en la barra de progreso de revisión"></asp:Label>
                            </div>
                            <asp:Label ID="lblAccOvertimeTitle" runat="server" Text="Extras:" CssClass="labelForm mediumWidth"></asp:Label>
                            <div class="componentForm">
                                <dx:ASPxComboBox ID="cmbCalendarOvertime" runat="server">
                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                </dx:ASPxComboBox>
                            </div>
                        </div>

                        <div class="divRow">
                            <div class="divRowDescription mediumPadding">
                                <asp:Label ID="lblAccNotJustifiedDescription" runat="server" Text="Este saldo se utilizará como ausencias en la barra de progreso de revisión"></asp:Label>
                            </div>
                            <asp:Label ID="lblAccNotJustifiedTitle" runat="server" Text="Ausencia:" CssClass="labelForm mediumWidth"></asp:Label>
                            <div class="componentForm">
                                <dx:ASPxComboBox ID="cmbCalendarNotJustified" runat="server">
                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                </dx:ASPxComboBox>
                            </div>
                        </div>
                    </Content>
                </roUserControls:roGroupBox>
            </div>

            <div style="width: 100%;">
                <table border="0" style="width: 100%;">
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width: 110px;" align="right">
                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                        </td>
                        <td style="width: 110px;" align="left">
                            <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>