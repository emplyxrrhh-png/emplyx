@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base.roLanguageWeb

@ModelType roStart

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim StartController As VTLive40.StartController = New VTLive40.StartController()
    Dim sRootURL As String = Request.ApplicationPath + "#/" + Robotics.Web.Base.Configuration.RootUrl
    Dim oPassport As roPassportTicket = ViewBag.CurrentPassport
    Dim oCompanyCode = ViewBag.CompanyCode
    Dim oCompanyName = ViewBag.CompanyName
    Dim showUserPilot As Boolean = ViewBag.ShowUserPilot
    Dim webLinks As Generic.List(Of roWebLink) = ViewBag.WebLinks
End Code

<link href="@Url.Content("~/Base/Styles/roStart.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Base/Scripts/Ajax.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/rocontrols/roTrees/roTrees.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/rocontrols/roTrees/roTreeState.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/roStart.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Start/roStart.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Scripts/exceljs.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Scripts/FileSaver.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<script>
    var RootUrl = "@sRootURL";
    var Genius = "@Html.Raw(ViewBag.Genius)";
@If (ViewBag.ShowAIChatBot.ToString.ToUpper <> "TRUE") Then
@<text>
    window.fwSettings = { 'widget_id': 31000000108 }; !function () { if ("function" != typeof window.FreshworksWidget) { var n = function () { n.q.push(arguments) }; n.q = [], window.FreshworksWidget = n } }()
</text>
    End If
</script>
@If (ViewBag.ShowAIChatBot.ToString.ToUpper <> "TRUE") Then
    @<script type='text/javascript' src='https://widget.freshworks.com/widgets/31000000108.js' async defer></script>
End If

<!--TODO: QUITAR VALIDACIÓN DESCRIPTION USERPILOT-->
@If (showUserPilot.ToString.ToUpper = "TRUE" AndAlso oPassport IsNot Nothing AndAlso oPassport.Description.ToString.ToLower = "userpilot") Then
    @<script>window.userpilotSettings = { token: "61kn9p1" };</script>
    @<script src="https://js.userpilot.io/sdk/latest.js"></script>
    @<script>
    userpilot.identify(
        "@oPassport.ID",
        {
            name: "@oPassport.Name",
            email: "@oPassport.Email",
            hostname: location.hostname,
            product: "visualtime",
            created_at: '2019-10-17',
            company: {
                id: "@oCompanyCode",
                name: "@oCompanyName"
            },
        }
    );
    //userpilot.reload();
    </script>
End If

<div id="news" style="float:left"></div>
<div id="newsOk" style="float:right; margin-top:60px;"></div>

<div style="display:flex;">
    <div style="border-left: 1vw solid white;border-top: 1vw solid white; width:90%">
        <div class="panHeader5" style="display: flex;align-items: center;">
            <div style="margin-right:0.5vw"><img src="~/Base/Images/PortalRequests/icons8-clock-48.png" /></div>
            <div style="width:100%">
                <div style="font-size:20px;width:100%"> @Html.Raw(StartController.HelloMessage())</div>
                <div style="font-size: 10px;width: 100%;font-style:italic;color:#0046FE; font-weight:100 !important"> @Html.Raw(StartController.LastLoginMessage())</div>
            </div>
            <div id="time" style="font-size: 20px;margin-left:0.5vw"></div>
        </div>
    </div>

    <div style="border-right: 1vw solid white; border-left: 1vw solid white;border-top: 1vw solid white;margin-left: 0.5vw; padding: 0px; display:flex;">
        <div class="panHeader5" style="font-size:20px"><img src="~/Base/Images/PortalRequests/icons8-company-48.png" style="cursor:pointer" onclick="navigateCompany()" title="@StartController.GetServerLanguage().Translate("roStartOrg", "Start")" /></div>
        <div class="panHeader5" style="font-size:20px;margin-left:0.5vw;"><img src="~/Base/Images/PortalRequests/icons8-team-48.png" style="cursor:pointer" onclick="navigateEmployees()" title="@StartController.GetServerLanguage().Translate("roStartEmp", "Start")" /></div>
        <div class="panHeader5" style="font-size:20px;margin-left:0.5vw"><img src="~/Base/Images/PortalRequests/icons8-calendar-48.png" style="cursor:pointer" onclick="navigateCalendar()" title="@StartController.GetServerLanguage().Translate("roStartCal", "Start")" /></div>

        @If (ViewBag.Genius.ToString.ToUpper = "TRUE") Then
            @<div Class="panHeader5" style="font-size:20px;margin-left:0.5vw"><img src="~/Base/Images/PortalRequests/Genius01.png" style="cursor:pointer" onclick="navigateAnalytic()" title="Genius" /></div>
        ElseIf (ViewBag.Genius.ToString.ToUpper = "STARTER") Then
            @<div></div>
        Else

            @<div Class="panHeader5" style="font-size:20px;margin-left:0.5vw"><img src="~/Base/Images/PortalRequests/icons8-combo-chart-48.png" style="cursor:pointer" onclick="navigateAnalytic()" title="@StartController.GetServerLanguage().Translate("roStartAna", "Start")" /></div>
        End If
    </div>
</div>

@*<div style="border-right: 1vw solid white; border-left: 1vw solid white;border-top: 1vw solid white;">
        <div class="panHeader5" style="font-size:20px;">@Html.Raw(StartController.HelloMessage()) <div id="time" style="font-size: 20px;float:right">  </div></div>
    </div>*@

<div id="divMainBody" style="min-height:unset !important; height:unset !important">
    <!-- TAB SUPERIOR -->
    <div id="divTabData" style="padding: 0 1vw 0 .5vw;">
        <div id="divContenido" Class="divAllContentStart twoWindowsFlexLayout">

            <div class="twoWindowsSidePanelNews maxHeight treeSizeStart" style="margin-right: .5vw; max-height: 89vh; overflow-y: auto; min-width: fit-content;" id="divTree">
                @For Each oWebLink As roWebLink In webLinks
                    @If oWebLink.ShowOnLiveDashboard Then
                        @Html.Partial("_webLinkWrapper", oWebLink)
                    End If
                Next
                <div>
                    <div style="display:none" class="panHeaderRobotics">
                        <div class="">@StartController.GetServerLanguage().Translate("roStartNews", "Start")</div>
                    </div>
                    <br />
                    <div id="newsXmlFeed" style="display:none" class="feedSize">
                        <div id="loadingFeed" runat="server" class="feedLoading"></div>
                    </div>
                </div>
            </div>

            <div id="divContenidoMain" class="twoWindowsMainPanel maxHeight divRightContent">

                <div style="display:flex;">
                    <div id="employeeStatusResume" style="margin-right: 15px; width:100%" class="panHeaderDBP">
                        <div id="divHelpMain" style="float:left; margin-top: 15px; margin-left: 15px; font-size: 13px;">
                            @StartController.GetServerLanguage().Translate("roStartEmployeesStatus", "Start")
                        </div>
                        <div id="divSelectorGroup" style="margin: 5px; float:right">
                            <div>

                                @(Html.DevExtreme().TagBox() _
                                .ID("GroupsList") _
                                .ShowSelectionControls(True) _
                                .ValueExpr("IdGroup") _
                                .DisplayExpr("Name") _
                                .Multiline(False) _
                                .Width(350) _
                                .ShowClearButton(True) _
                                .SearchEnabled(True) _
                                .ApplyValueMode(EditorApplyValueMode.UseButtons) _
                                .SearchExpr("Name") _
                                .OnSelectionChanged("refreshGrid") _
                                .OnSelectAllValueChanged("selectedAllGroups") _
                                .OnContentReady("onContentReady") _
                                .DataSource(Function(ds)
                                Return ds.Mvc() _
                                .Controller("Start") _
                                .LoadAction("GetGroups") _
                                .Key("IdGroup")
                                End Function))
                                @*@(Html.DevExtreme().SelectBox() _
                                    .DataSource(Function(ds)
                                    Return ds.Mvc() _
                                    .Controller("Start") _
                                    .LoadAction("GetGroups") _
                                    .Key("IdGroup")
                                    End Function) _
                                    .DisplayExpr("Name") _
                                    .Width(350) _
                                    .ID("GroupsList") _
                                    .DeferRendering(False) _
                                    .SearchEnabled(True) _
                                    .ShowClearButton(True) _
                                    .ShowSelectionControls(True) _
                                    .OnContentReady("onContentReady") _
                                    .OnValueChanged("refreshGrid") _
                                    )*@
                            </div>
                        </div>
                    </div>
                    @*<div class="panHeaderDashboardSmallButton" style="margin-left:0.5vw; padding: 0px;">
                        </div>*@
                </div>
                <br />
                <div class="form ro-tab-section container" style="display: flex;">

                    <div class="section1" id="section1">
                        <div id="employeesResumeWithData" style="display:none;">

                            <div id="section2">
                                @*<div class="panHeader4">@StartController.GetServerLanguage().Translate("roStartMyRequests", "Start")</div>*@

                                <div id="divFilters" style="text-align:center;">

                                    @If (ViewBag.TelecommutingInstalled.ToString.ToUpper = "TRUE") Then
                                        @<div id="employeeStatus" style="width: 31%; display: inline-block; margin-right: 10px; ">@Html.Partial("employeeResume")</div>
                                        @<div id="absenceStatus" style="width: 31%; display: inline-block; margin-right: 10px; ">@Html.Partial("absenceResume")</div>
                                        @<div id="telecommutingStatus" style="width: 31%; display: inline-block; ">@Html.Partial("telecommutingResume")</div>
                                    Else
                                        @<div id="employeeStatus" style="width: 45%; display: inline-block; margin-right: 10px; ">@Html.Partial("employeeResume")</div>
                                        @<div id="absenceStatus" style="width: 45%; display: inline-block; margin-right: 10px; ">@Html.Partial("absenceResume")</div>
                                    End If
                                </div>
                            </div>

                            <div id="divEmployees" runat="server" class="jsGridContentStart dextremeGrid">
                                @(Html.DevExtreme().DataGrid() _
                                .ID("gridStatusEmployees") _
                                .DataSource(Function(ds)
                                                Return ds.Mvc() _
                                                .Controller("Start") _
                                                .LoadAction("GetEmployees") _
                                                .LoadParams(New With {.hasData = New JS("hasData")}) _
                                                .OnBeforeSend("beforeSend")
                                            End Function) _
                                .ShowColumnLines(False) _
                                .ShowRowLines(True) _
                                .Selection(Sub(columns)
                                               columns.Mode(SelectionMode.Single)
                                           End Sub) _
                                .RowAlternationEnabled(False) _
                                .ShowBorders(False) _
                                .ColumnHidingEnabled(False) _
                                .ColumnAutoWidth(False) _
                                .AllowColumnResizing(True) _
                                .Export(Sub(columns)
                                            columns.Enabled(True)
                                            columns.AllowExportSelectedData(True)
                                        End Sub) _
                                .NoDataText(StartController.GetServerLanguage().Translate("roNoData", "Start")) _
                                .OnContentReady("refreshPartial") _
                                .LoadPanel(Sub(columns)
                                               columns.Text(StartController.GetServerLanguage().Translate("roStartLoading", "Start"))
                                           End Sub) _
                                .Paging(Sub(columns)
                                            columns.PageSize(25)
                                        End Sub) _
                                .Pager(Sub(columns)
                                           columns.ShowPageSizeSelector(True)
                                           columns.AllowedPageSizes({25, 50, 100})
                                           columns.ShowInfo(False)
                                       End Sub) _
                                .OnCellClick("selection_changed") _
                                .OnExporting("exportDashboard") _
                                .OnCellPrepared("cell_prepared") _
                                .FilterRow(Sub(columns)
                                               columns.Visible(True)
                                           End Sub) _
                                .HeaderFilter(Sub(columns)
                                                  columns.Visible(True)
                                              End Sub) _
                                .Columns(Sub(columns)
                                             columns.Add().DataField("Image").Caption("").AllowResizing(False).AllowFiltering(True).Width(45).AllowSorting(False) _
                                             .HeaderFilter(Sub(filter)
                                                               filter.DataSource(New JS("headerFilter"))
                                                           End Sub) _
                                             .CellTemplate("<div class='photoStart' style='cursor: pointer;'><div id='absenceIcon'><img src=' <%- value %>' height='32' style='border-radius:50%' /></div></div>")
                                             columns.Add().DataField("EmployeeName").Caption(StartController.GetServerLanguage().Translate("roStartUser", "Start"))
                                             columns.Add().DataField("PresenceStatus").Caption("Estado").Visible(False)
                                             columns.Add().DataField("InAbsence").Caption("InAbsence").Visible(False)
                                             columns.Add().DataField("InHolidays").Caption("InHolidays").Visible(False)
                                             columns.Add().DataField("InAnyAbsence").Caption("InAnyAbsence").Visible(False)
                                             columns.Add().DataField("InAnyHoliday").Caption("InAnyHoliday").Visible(False)
                                             columns.Add().DataField("InRealTimeTC").Caption("InRealTimeTC").Visible(False)
                                             columns.Add().DataField("InTelecommute").Visible(False).Caption("InTelecommute")
                                             columns.Add().DataField("InHoursAbsence").Caption("InHoursAbsence").Visible(False)
                                             columns.Add().DataField("DaysAbsenceRequested").Caption("DaysAbsenceRequested").Visible(False)
                                             columns.Add().DataField("HoursAbsenceRequested").Caption("HoursAbsenceRequested").Visible(False)
                                             columns.Add().DataField("LastPunchFormattedDateTime").CalculateSortValue("RealLastPunch").Caption(StartController.GetServerLanguage().Translate("roStartHour", "Start"))
                                             columns.Add().DataField("Details").Caption(StartController.GetServerLanguage().Translate("roStartDetails", "Start"))
                                             columns.Add().DataField("ZoneName").Caption(StartController.GetServerLanguage().Translate("roStartZone", "Start"))
                                             columns.Add().DataField("LocationName").Caption(StartController.GetServerLanguage().Translate("roStartLocalization", "Start"))
                                             columns.Add().DataField("GroupPath").Caption("Grupo").Visible(False)
                                             columns.Add().DataField("InTelecommuteImage").HeaderFilter(Sub(filter)
                                                                                                            filter.DataSource(New JS("headerFilterTelecommute"))
                                                                                                        End Sub) _
                                .Caption(StartController.GetServerLanguage().Translate("roStartTelecommute", "Start")).CellTemplate("<div class='photoStart'><img src=' <%- value %>' height='32' /></div>").Visible(ViewBag.TelecommutingInstalled)
                                             columns.Add().DataField("CostCenterName").Caption(StartController.GetServerLanguage().Translate("roStartCC", "Start"))
                                             columns.Add().DataField("ShiftName").Caption(StartController.GetServerLanguage().Translate("roStartShift", "Start"))
                                             columns.Add().DataField("TaskName").Caption(StartController.GetServerLanguage().Translate("roStartTask", "Start"))
                                             columns.Add().DataField("InAbsenceImage").HeaderFilter(Sub(filter)
                                                                                                        filter.DataSource(New JS("headerFilterAbsence"))
                                                                                                    End Sub) _
                                             .Caption(StartController.GetServerLanguage().Translate("roStartAbsence", "Start")).CellTemplate("<div class='photoStart'><img src=' <%- value %>' height='32' /></div>")
                                         End Sub) _
                                .OnToolbarPreparing("toolbar_preparing") _
                                )
                            </div>
                        </div>

                        <div id="employeesResumeWithoutData" style="display:none;">
                            <div style="font-size: 16px; text-align: center; color: #525252; ">
                                @StartController.GetServerLanguage().Translate("roStartNoGroupSelected", "Start")
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="twoWindowsSidePanel maxHeight treeSizeStart" id="divTree">
                <div style="display:flex;">
                    <div class="panHeaderDashboardSmallGlobal" style="width: 100%">
                        @StartController.GetServerLanguage().Translate("roStartMyRequests", "Start")
                    </div>
                    <div class="panHeaderDashboardSmallButton" style="margin-left:0.5vw; padding: 0px;">
                        <div style="margin: 5px; position: relative;">
                            @(Html.DevExtreme().Button() _
                            .Icon("chevronright") _
                            .ID("buttonRequests") _
                            .OnClick("reenviaRequests") _
                            .Type(ButtonType.Default) _
                            )
                        </div>
                    </div>
                </div>

                <div class="list-containerRequests">

                    @(Html.DevExtreme().List() _
                    .Height("100%") _
                    .DataSource(Function(ds)
                    Return ds.Mvc() _
                    .Controller("Start") _
                    .LoadAction("GetRequests")
                    End Function) _
                    .PageLoadingText(StartController.GetServerLanguage().Translate("roStartLoading", "Start")) _
                    .FocusStateEnabled(False) _
                    .ActiveStateEnabled(False) _
                    .NoDataText(StartController.GetServerLanguage().Translate("roNoData", "Start")) _
                    .ItemTemplate("<div class='requestDB' style='cursor: default;'><img src=' <%- ImageSrc %>'><div style='cursor: default;'><%- Description %></div><div class='image' style='cursor: default;'><%- Count %></div></div>")
                    )
                </div>

                <div style="display:flex;">
                    <div id="myAlertsResume" class="panHeaderDashboardSmallGlobal" style="width:100%">
                        @StartController.GetServerLanguage().Translate("roStartMyAlerts", "Start")
                    </div>
                    <div class="panHeaderDashboardSmallButton" style="margin-left:0.5vw; padding: 0px;">
                        <div style="margin: 5px; position: relative;">
                            @(Html.DevExtreme().Button() _
                            .Icon("chevronright") _
                            .ID("buttonAlerts") _
                            .OnClick("reenviaAlerts") _
                            .Type(ButtonType.Default) _
                            )
                        </div>
                    </div>
                </div>

                <div class="list-containerRequests">

                    @(Html.DevExtreme().List() _
                    .Height("100%") _
                    .DataSource(Function(ds)
                    Return ds.Mvc() _
                    .Controller("Start") _
                    .LoadAction("GetAlerts")
                    End Function) _
                    .PageLoadingText(StartController.GetServerLanguage().Translate("roStartLoading", "Start")) _
                    .FocusStateEnabled(False) _
                    .ActiveStateEnabled(False) _
                    .NoDataText(StartController.GetServerLanguage().Translate("roNoData", "Start")) _
                    .ItemTemplate("<div class='requestDB' style='cursor: default;'><img src=' <%- ImageSrc %>'><div style='cursor: default;'><%- Description %></div><div class='image' style='cursor: default;'><%- Count %></div></div>")
                    )
                </div>
            </div>
        </div>
    </div>

    @(Html.DevExtreme().Popover() _
                .Target("#newsOk") _
                .ID("newsOk") _
                .Visible(ViewBag.ShowHelp) _
                .hideOnOutsideClick(False) _
                .ShowCloseButton(True) _
                .OnHiding("hideThePopover") _
                .OnShown("showPopover") _
                .Shading(True) _
                .ShowTitle(False) _
                .Title("Entendido") _
                .ShadingColor("rgba(0, 0, 0, 0.5)") _
                .Position(DevExtreme.AspNet.Mvc.Position.Right) _
                .Width(310) _
                .Height(105) _
                .ContentTemplate("<div style='display: flex; cursor:pointer' onclick='hideThePopover()'><div class='minisection1'><img src='./Base/Images/PortalRequests/BannerVTLive.png' style='cursor:pointer' onclick='hideThePopover()' /></div><div class='minisection2' style='color:white'>" & StartController.GetServerLanguage().Translate("roStartNewsOk", "Start") & "</div></div>")
    )
    @*.ContentTemplate("<div style='display: flex; cursor:pointer' onclick='hidePopover()'><div class='minisection1'><img src='/Base/Images/PortalRequests/icons8-circled-play-48green.png' style='cursor:pointer' onclick='hidePopover()' /></div><div class='minisection2'>" & StartController.GetServerLanguage().Translate("roStartNewsOk", "Start") & "</div></div>")*@

    @(Html.DevExtreme().Popover() _
                .Target("#divHelpMain") _
                .ID("news1") _
                .Visible(False) _
                .HideOnOutsideClick(False) _
                .ShowCloseButton(True) _
                .Shading(False) _
                .ShowTitle(False) _
                .Title("Menu navegacion") _
                .ShadingColor("rgba(0, 0, 0, 0.5)") _
                .Position(DevExtreme.AspNet.Mvc.Position.Top) _
                .Width(300) _
                .Height(90) _
                .ContentTemplate("<div style='display: flex'><div class='minisection1'><img src='./Base/Images/PortalRequests/icons8-help-48.png' /></div><div class='minisection2'>" & StartController.GetServerLanguage().Translate("roStartNews1", "Start") & "</div></div>")
    )
    @(Html.DevExtreme().Popover() _
                .Target("#divSelectorGroup") _
                .ID("news2") _
                .Visible(False) _
                .HideOnOutsideClick(False) _
                .ShowCloseButton(True) _
                .ShowTitle(False) _
                .Title("Resumen estado") _
                .Shading(False) _
                .Position(DevExtreme.AspNet.Mvc.Position.Top) _
                .Width(300) _
                .Height(90) _
                .ContentTemplate("<div style='display: flex'><div class='minisection1'><img src='./Base/Images/PortalRequests/icons8-help-48.png' /></div><div class='minisection2'>" & StartController.GetServerLanguage().Translate("roStartNews2", "Start") & "</div></div>")
    )
    @(Html.DevExtreme().Popover() _
                .Target("#section1") _
                .ID("news3") _
                .Visible(False) _
                .HideOnOutsideClick(False) _
                .ShowCloseButton(True) _
                .ShowTitle(False) _
                .Title("Resumen estado") _
                .Shading(False) _
                .Position(DevExtreme.AspNet.Mvc.Position.Top) _
                .Width(300) _
                .Height(90) _
                .ContentTemplate("<div style='display: flex'><div class='minisection1'><img src='./Base/Images/PortalRequests/icons8-help-48.png' /></div><div class='minisection2'>" & StartController.GetServerLanguage().Translate("roStartNews3", "Start") & "</div></div>")
    )
    @(Html.DevExtreme().Popup() _
                .ID("updateNotificationPopup") _
                .WrapperAttr(New Dictionary(Of String, Object) From {
                                {"id", "updateNotificationPopup"},
                                {"class", "update-popup-class"}
                            }) _
                .Visible(ViewBag.ShowVersionNotification) _
                .Shading(True) _
                .ShadingColor("rgba(0, 0, 0, 0.5)") _
                .CloseOnOutsideClick(False) _
                .Title(StartController.GetServerLanguage().Translate("roVersionUpdatedTitle", "Start")) _
                .Width("30%") _
                .Height("auto") _
                .ShowCloseButton(False) _
                .ToolbarItems(DirectCast(Sub(toolbarItems)
                                             toolbarItems.Add() _
                                     .Toolbar(DevExtreme.AspNet.Mvc.Toolbar.Bottom) _
                                     .Location(DevExtreme.AspNet.Mvc.ToolbarItemLocation.Center) _
                                     .Widget(Function(widget) widget.Button() _
                                         .Text(StartController.GetServerLanguage().Translate("roAcceptButton", "Start")) _
                                         .StylingMode(DevExtreme.AspNet.Mvc.ButtonStylingMode.Outlined) _
                                         .Type(DevExtreme.AspNet.Mvc.ButtonType.Normal) _
                                         .OnClick("acceptUpdateNotification"))
                                         End Sub, Action(Of DevExtreme.AspNet.Mvc.Factories.CollectionFactory(Of DevExtreme.AspNet.Mvc.Builders.PopupToolbarItemBuilder)))) _
                .ContentTemplate(
                    "<div style='text-align: center; padding: 10px;'>" &
                    "<p>" & StartController.GetServerLanguage().Translate("roVersionUpdatedMessage", "Start") & ": <strong>" & ViewBag.VersionInfo & "</strong></p>" &
                    "<a href='https://helpcenter.ila.cegid.com/es/visualtime/novedades/' target='_blank' rel='noopener noreferrer' style='color: #007bff; text-decoration: underline;'>" & StartController.GetServerLanguage().Translate("roViewVersionChanges", "Start") & "</a>" &
                    "<div style='margin-top: 10px;'>" &
                    "<input type='checkbox' id='dontShowUpdateAgainCheckbox' />" &
                    "<label for='dontShowAgainCheckbox'>" &
                    StartController.GetServerLanguage().Translate("roDontShowAgain", "Start") & "</label>" &
                    "</div>" &
                    "</div>"
                )
)

    @Code
        Html.DevExtreme().Popup() _
.ID("warningGroups") _
.Width(350) _
.Height(140) _
.ShowTitle(False) _
.DragEnabled(False) _
.HideOnOutsideClick(True) _
.ContentTemplate(Sub()
@<text>
    @Html.Partial("PopupWarning")
</text>
End Sub) _
.Render()
    End Code
</div>

<script>
    (function () {
        function checkTime(i) {
            return (i < 10) ? "0" + i : i;
        }

        function startTime() {
            var today = new Date(),
                h = checkTime(today.getHours()),
                m = checkTime(today.getMinutes()),
                s = checkTime(today.getSeconds());
            document.getElementById('time').innerHTML = h + ":" + m + ":" + s;
            t = setTimeout(function () {
                startTime()
            }, 500);
        }
        startTime();
    })();
</script>