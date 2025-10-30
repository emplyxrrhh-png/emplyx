<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" ValidateRequest="false" Inherits="VTLive40.Budget" Title="${Budgets}" CodeBehind="Budget.aspx.vb" %>

<%@ Register Src="~/Base/WebUserControls/roCalendar/roCalendar.ascx" TagName="roCalendar" TagPrefix="roForms" %>

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
                        <img src="Images/Budget80.png" alt="calendar" />
                    </div>
                    <div class="blackRibbonDescription" style="max-width: inherit">
                        <div class="NameText" style="text-align: left;"><%= Me.Language.Translate("Budget", Me.DefaultScope)%> </div>
                    </div>
                </div>
                <div id="divCalendarTabs" runat="server" style="float: right; width: 20%;">
                </div>

                <div id="divShiftsPalette" runat="server" style="margin: 0 auto; width: 60%;">
                    <table style="width: 100%">
                        <tr>
                            <td style="text-align: left; white-space: nowrap; width: 10%">
                                <dx:aspxlabel id="lblGroupShifts" runat="server" text="Grupo de horarios:  " />
                            </td>
                            <td style="text-align: left; white-space: nowrap">
                                <dx:aspxlabel id="lblProductiveUnitName" font-bold="true" runat="server" text="" clientinstancename="lblProductiveUnitNameClient" />
                                <div style="display: none">
                                    <dx:aspxcombobox runat="server" id="cmbProductiveUnits" clientinstancename="cmbProductiveUnitsClient">
                                        <clientsideevents gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" selectedindexchanged="loadProductiveUnits" />
                                    </dx:aspxcombobox>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <div>
                        <div style="width: 5%; float: left; margin-top: 10px; margin-left: 2px;">
                            <dx:aspxlabel id="lblModesTitle" runat="server" text="Modos" font-bold="true" />
                        </div>
                        <div style="width: 94%; float: left; height: 50px">
                            <div class="pUnitGroups">
                                <dx:aspxcallbackpanel id="ASPxProductiveUnitSelector" runat="server" width="100%" height="100%" clientinstancename="ASPxProductiveUnitSelectorClient">
                                    <settingsloadingpanel enabled="false" />
                                    <clientsideevents endcallback="ASPxProductiveUnitSelector_EndCallback" />
                                    <panelcollection>
                                        <dx:panelcontent id="PanelContent2" runat="server">
                                            <div runat="server" id="divPUnitsServer">
                                            </div>
                                        </dx:panelcontent>
                                    </panelcollection>
                                </dx:aspxcallbackpanel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <dx:aspxhiddenfield id="hdnCalendarConfig" runat="server" clientinstancename="hdnCalendarConfigClient"></dx:aspxhiddenfield>
        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divContenido" class="divAllContent">
                <div id="divContent">
                    <div id="divtopBar" class="calendarFilters">

                        <div class="filters">
                            <div style="margin-left: 50px; float: left; width: 310px;">
                                <dx:aspxtextbox id="txtOrgChartSelection" runat="server" readonly="true" font-size="11px" height="25px" cssclass="editTextFormat" width="300px" clientinstancename="txtOrgChartSelectionClient">
                                    <clientsideevents gotfocus="btnOpenOrgChartSelector" />
                                </dx:aspxtextbox>
                            </div>

                            <div class="btnDateFilter1" id="divDayDetailCalendar" style="float: left; background-image: url(Images/OneDayDetail.png);" title="<%= Me.Language.Translate("aOneDayCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divWeekDetailCalendar" style="float: left; background-image: url(Images/OneWeekDetail.png);" title="<%= Me.Language.Translate("aOneWeekCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divDayCalendar" style="float: left; background-image: url(Images/OneDay.png);" title="<%= Me.Language.Translate("aOneDayCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divWeekCalendar" style="float: left; background-image: url(Images/OneWeek.png);" title="<%= Me.Language.Translate("aOneWeekCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divTwoCalendar" style="float: left; background-image: url(Images/TwoWeek.png);" title="<%= Me.Language.Translate("aTwoWeekCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divMonthCalendar" style="float: left; background-image: url(Images/OneMonth.png);" title="<%= Me.Language.Translate("aMonthCalendar", Me.DefaultScope) %>"></div>
                            <div class="btnDateFilter1" id="divFreeCalendar" style="float: left; background-image: url(Images/FreeSelection.png);" title="<%= Me.Language.Translate("aFreeCalendar", Me.DefaultScope) %>"></div>
                            <div style="width: 20px; text-align: center; float: left"><a href="javascript: void(0);" class="btnSchedulerMove flaticon-left stock-nextDates" style="border: none;" onclick="SchedulerNavigateV2('previous');" title="<%= Me.Language.Translate("aPreviousPeriod", Me.DefaultScope) %>"></a></div>
                            <div style="width: 20px; text-align: center; float: left"><a href="javascript: void(0);" class="btnSchedulerMove flaticon-right stock-previousDates" style="border: none;" onclick="SchedulerNavigateV2('next');" title="<%= Me.Language.Translate("aNextPeriod", Me.DefaultScope) %>"></a></div>
                            <div class="centerMiddle" style="float: left; width: 190px; height: 20px; padding-left: 10px; padding-top: 4px;">
                                <dx:aspxlabel id="txtDateRange" runat="server" readonly="true" font-size="14px" height="25px" width="190px" clientinstancename="txtDateRangeClient">
                                </dx:aspxlabel>
                            </div>
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
                            <dx:aspxcallbackpanel id="ASPxCallbackPanelBarButtons" runat="server" height="100%" clientinstancename="ASPxCallbackPanelBarButtonsClient">
                                <settingsloadingpanel enabled="false" />
                                <clientsideevents endcallback="ASPxCallbackPanelBarButtonsClient_EndCallBack" />
                                <panelcollection>
                                    <dx:panelcontent id="panelBarButtons" runat="server">
                                        <div id="divButtons" class="divMiddleButtons">
                                            <div id="divBarButtons" runat="server" class="maxHeight"></div>
                                        </div>
                                    </dx:panelcontent>
                                </panelcollection>
                            </dx:aspxcallbackpanel>
                        </div>

                        <div id="oBudgetCalendar" class="calendarRightDiv" style="overflow: hidden">
                            <div id="loadingCalendar" style="width: 100%; height: 100%; background: white;">&nbsp</div>
                            <roforms:rocalendar id="oCalendar" workmode="roBudget" runat="server" feature="Calendar" clientinstancename="roBudget" height="100%"
                                rocalendar_endcallback="roBudget_CallbackComplete" performaction_endcallback="PerformActionCallback_CallbackComplete"
                                complementary_endcallback="roBudgetComplentary_CallbackComplete" assignments_endcallback="roBudgetAssignments_CallbackComplete" />

                            <roforms:rocalendar id="oOrgCalendar" workmode="roCalendar" runat="server" feature="Calendar" clientinstancename="roScheduleCalendar" height="100%"
                                rocalendar_endcallback="roCalendar_CallbackComplete" performaction_endcallback="PerformActionCallback_CallbackComplete"
                                complementary_endcallback="complementaryDefinitionCallback_CallbackComplete" assignments_endcallback="assignmentDefinitionCallback_CallbackComplete" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- POPUP roUnitMode -->
        <div style="display: none">
            <div id="dialogCellDetail" class="ui-dialog-content">
                <!-- Este div es el header -->
                <div class="panBottomMargin">
                    <div class="panHeader2 panBottomMargin">
                        <span class="panelTitleSpan">
                            <asp:Label runat="server" ID="lblUnitModeGeneral" Text="General"></asp:Label>
                        </span>
                    </div>
                    <!-- La descripción es opcional -->
                    <div class="panelHeaderContent">
                        <div class="descriptionAssignDiv">
                            <div id="lblDayInformation" runat="server">
                            </div>
                        </div>
                    </div>
                </div>

                <div class="panBottomMargin">
                    <div class="panHeader2 panBottomMargin">
                        <span class="panelTitleSpan">
                            <asp:Label runat="server" ID="lblTeam" Text="Equipo"></asp:Label>
                        </span>
                    </div>
                </div>

                <div class="panBottomMargin">
                    <div class="jsGrid">
                        <asp:Label ID="lblAddPosition" runat="server" CssClass="jsGridTitle" Text="Posiciones"></asp:Label>
                    </div>
                    <div class="jsGridContent" style="overflow: hidden; height: 400px">
                        <roforms:rocalendar id="oDetailCalendar" workmode="roDayDetail" runat="server" feature="Calendar" clientinstancename="oDetailCalendar" height="100%"
                            rocalendar_endcallback="oDetailCalendar_calendar_CallbackComplete" performaction_endcallback="PerformActionCallback_CallbackComplete"
                            complementary_endcallback="oDetailCalendar_Complementary_CallbackComplete" assignments_endcallback="oDetailCalendar_assignment_CallbackComplete" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- POPUP roUnitMode -->
    <div id="orgChartDiv" style="display: none">
        <div>
            <%--'TODO Replace roSecurityNodes For Something--%>
        </div>
        <div>
            <div id="btnCancelOrgChart" style="float: right">
            </div>
            <div id="btnAcceptOrgChart" class="acceptButton" style="float: right; margin-right: 15px;">
            </div>
        </div>
    </div>

    <!-- POPUP NEW OBJECT -->
    <dx:aspxpopupcontrol id="AspxLoadingPopup" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="~/Base/Popups/PerformingAction.aspx" cssclass="captchaFirst"
        popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" modalbackgroundstyle-opacity="0" width="460" height="260"
        showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="AspxLoadingPopup_Client" popupanimationtype="None" backcolor="Transparent" contentstyle-paddings-padding="0px" border-bordercolor="Transparent" showshadow="false">
        <settingsloadingpanel enabled="false" />
    </dx:aspxpopupcontrol>

    <!-- POPUP NEW OBJECT -->
    <dx:aspxpopupcontrol id="CaptchaObjectPopup" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="~/Base/Popups/GenericCaptchaValidator.aspx" cssclass="captchaFirst"
        popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" modalbackgroundstyle-opacity="0" width="500" height="320"
        showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="CaptchaObjectPopup_Client" popupanimationtype="None" backcolor="Transparent" contentstyle-paddings-padding="0px" border-bordercolor="Transparent" showshadow="false">
        <settingsloadingpanel enabled="false" />
    </dx:aspxpopupcontrol>
</asp:Content>