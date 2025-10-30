<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" EnableEventValidation="false" AutoEventWireup="false" ValidateRequest="false" Inherits="VTLive40.Calendar" Title="Calendario" CodeBehind="Calendar.aspx.vb" %>

<%@ Register Src="~/Base/WebUserControls/roCalendar/roCalendar.ascx" TagName="roCalendar" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roTreeV3.ascx" TagName="roTreeV3" TagPrefix="rws" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <input id="hdnReportsTitleControlID" type="hidden" value="<%= hdnReportsTitleReads.ClientID %>" />
    <input id="hdnReportsType" type="hidden" value="Reads" />

    <asp:HiddenField ID="hdnReportsTitleReads" Value="" runat="server" />
    <asp:HiddenField ID="hdnReportsTitleShifts" Value="" runat="server" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divCalendar" runat="server" class="blackRibbonTitle">
                <div id="divCalendarTitle" runat="server" style="float: left; width: 20%;">
                    <div class="blackRibbonIcon">
                        <img src="Images/Calendar490_80.png" alt="calendar" />
                    </div>
                    <div class="blackRibbonDescription" style="max-width: inherit">
                        <div class="NameText" style="text-align: left;"><%= Me.Language.Translate("Scheduler", Me.DefaultScope)%> </div>
                    </div>
                </div>
                <div id="divCalendarTabs" runat="server" style="float: right; width: 20%;">
                </div>

                <div id="divShiftsPalette" runat="server" style="margin: 0 auto; width: 60%;">
                    <table style="width: 100%">
                        <tr>
                            <td style="text-align: left; white-space: nowrap; width: 10%">
                                <dx:ASPxLabel ID="lblGroupShifts" runat="server" Text="Grupo de horarios:  " />
                            </td>
                            <td style="text-align: left; white-space: nowrap;">
                                <dx:ASPxComboBox runat="server" ID="cmbShiftGroups" ClientInstanceName="cmbShiftGroupsClient">
                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" SelectedIndexChanged="loadShifts" />
                                </dx:ASPxComboBox>
                            </td>
                        </tr>
                    </table>
                    <div style="width: 100%; height: 50px">
                        <div class="shiftGroups">
                            <dx:ASPxCallbackPanel ID="ASPxSelectorShiftsCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxSelectorShiftsCallbackPanelContenidoClient">
                                <SettingsLoadingPanel Enabled="false" />
                                <ClientSideEvents EndCallback="ASPxSelectorShiftsCallbackPanelContenido_EndCallback" />
                                <PanelCollection>
                                    <dx:PanelContent ID="PanelContent2" runat="server">
                                        <div runat="server" id="divShiftsServer">
                                        </div>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxCallbackPanel>
                        </div>
                    </div>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <dx:ASPxHiddenField ID="hdnCalendarConfig" runat="server" ClientInstanceName="hdnCalendarConfigClient"></dx:ASPxHiddenField>
        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divContenido" class="divAllContent">
                <div id="divContent">
                    <div id="divtopBar" class="calendarFilters">

                        <div class="filters">
                            <div style="margin-left: 50px; float: left; width: 310px;">
                                <dx:ASPxTextBox ID="txtEmployees" runat="server" ReadOnly="true" Font-Size="11px" Height="25px" CssClass="editTextFormat" Width="300px" ClientInstanceName="txtEmployeesClient">
                                    <ClientSideEvents GotFocus="btnOpenPopupSelectorEmployeesClient_Click" />
                                </dx:ASPxTextBox>
                            </div>

                            <div class="btnDateFilter1" id="divDayCalendar" style="float: left; background-image: url(Images/OneDay.png);" title="<%= Me.Language.Translate("aOneDayCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divWeekCalendar" style="float: left; background-image: url(Images/OneWeek.png);" title="<%= Me.Language.Translate("aOneWeekCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divTwoCalendar" style="float: left; background-image: url(Images/TwoWeek.png);" title="<%= Me.Language.Translate("aTwoWeekCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divMonthCalendar" style="float: left; background-image: url(Images/OneMonth.png);" title="<%= Me.Language.Translate("aMonthCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divFreeCalendar" style="float: left; background-image: url(Images/FreeSelection.png);" title="<%= Me.Language.Translate("aFreeCalendar", Me.DefaultScope) %>"></div>
                            <div style="width: 20px; text-align: center; float: left"><a href="javascript: void(0);" class="btnSchedulerMove flaticon-left stock-nextDates" style="border: none;" onclick="SchedulerNavigateV2('previous');" title="<%= Me.Language.Translate("aPreviousPeriod", Me.DefaultScope) %>"></a></div>
                            <div style="width: 20px; text-align: center; float: left"><a href="javascript: void(0);" class="btnSchedulerMove flaticon-right stock-previousDates" style="border: none;" onclick="SchedulerNavigateV2('next');" title="<%= Me.Language.Translate("aNextPeriod", Me.DefaultScope) %>"></a></div>
                            <div class="centerMiddle" style="float: left; width: 190px; height: 20px; padding-left: 10px; padding-top: 4px;">
                                <dx:ASPxLabel ID="txtDateRange" runat="server" ReadOnly="true" Font-Size="14px" Height="25px" Width="190px" ClientInstanceName="txtDateRangeClient">
                                </dx:ASPxLabel>
                            </div>

                            <%--Falta añadir el control del selector de fechas aquí--%>
                        </div>
                        <div class="saveBar">
                            <div id="divMsgTop" class="divMsg2 divMessageTop" style="display: none">
                                <div class="divImageMsg">
                                    <img alt="" id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" />
                                </div>
                                <div class="messageText">
                                    <span id="msgTop"></span>
                                </div>
                                <div align="right" class="messageActions">
                                    <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                    &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                    <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="bottomBar" class="calendarControl">
                        <div class="calendarLeftDiv" tabindex="0">
                            <dx:ASPxCallbackPanel ID="ASPxCallbackPanelBarButtons" runat="server" Height="100%" ClientInstanceName="ASPxCallbackPanelBarButtonsClient">
                                <SettingsLoadingPanel Enabled="false" />
                                <ClientSideEvents EndCallback="ASPxCallbackPanelBarButtonsClient_EndCallBack" />
                                <PanelCollection>
                                    <dx:PanelContent ID="panelBarButtons" runat="server">
                                        <div id="divButtons" class="divMiddleButtons">
                                            <div id="divBarButtons" runat="server" class="maxHeight"></div>
                                        </div>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxCallbackPanel>
                        </div>

                        <div class="calendarRightDiv" style="overflow: hidden">
                            <div id="loadingCalendar" style="width: 100%; height: 100%; background: white;">&nbsp</div>
                            <roForms:roCalendar ID="oCalendar" WorkMode="roCalendar" runat="server" Feature="Calendar" ClientInstanceName="roScheduleCalendar" Height="100%"
                                roCalendar_EndCallback="CallbackCalendar_CallbackComplete" performAction_EndCallback="PerformActionCallback_CallbackComplete"
                                complementary_EndCallback="complementaryDefinitionCallback_CallbackComplete" assignments_EndCallback="assignmentDefinitionCallback_CallbackComplete" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- POPUP DEL SELECTOR DE EMPLEADOS -->
    <dx:ASPxPopupControl ID="PopupSelectorEmployees" runat="server" AllowDragging="True" CloseAction="OuterMouseClick" Modal="True" ClientInstanceName="PopupSelectorEmployeesClient"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="660px" Width="990px" ClientSideEvents-Init="OnInitGroupSelector" CloseOnEscape="false"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                <dx:ASPxPanel ID="ASPxPanel3" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent3" runat="server">
                            <div class=".transparentPopupExtended" style="width: 980px; height: 650px">
                                <table id="tbPopupFrame" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <iframe id="ifEmployeeSelector" name="ifEmployeeSelector" runat="server" style="background-color: Transparent;" height="640" width="940" scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="AspxLoadingPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/PerformingAction.aspx" CssClass="captchaFirst"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="460" Height="260"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="AspxLoadingPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>

    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx" CssClass="captchaFirst"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>
</asp:Content>