@Imports Robotics.Base.DTOs
@imports Robotics.Web.Base
@Imports Robotics.Web.Base.roLanguageWeb
@Imports Robotics.Base.VTBusiness.Zone
@ModelType roZone

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim ZonesStatusController As VTLive40.ZonesStatusController = New VTLive40.ZonesStatusController()
    Dim ZonesController As VTLive40.ZonesController = New VTLive40.ZonesController()
    Dim baseURL = Url.Content("~")
    Dim scriptVersion As String = ZonesStatusController.ScriptsVersion
    Dim mapsKey As String = ZonesController.GetGoogleAPIKey() & "&libraries=places&language=" & HelperWeb.GetCookie("VTLive_Language")
End Code

<script>
    var RootUrl = "@Html.Raw(ViewBag.RootUrl)";
    var Zones = JSON.parse('@Html.Raw(ViewBag.Zones)');
    var ZonaGlobal = JSON.parse('@Html.Raw(ViewBag.ZonaGlobal)');
    var Permission = "@Html.Raw(ViewBag.PermissionOverEmployees)";
    var BASE_URL = "@baseURL";
</script>

<link href="@Url.Content("~/Base/Styles/roStart.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<script src="@Url.Content("~/Base/Scripts/Ajax.js")" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Zones/ZonesStatus.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<div style="display:flex;">
    <div style="border-left: 1vw solid white;border-top: 1vw solid white; width:100%;border-right: 1vw solid white;">
        <div class="panHeader5" style="display: flex;align-items: center;">
            <div style="margin-right:0.5vw"><img src="~/Base/Images/StartMenuIcos/ZonesStatus.png" width="48" height="48" /></div>
            <div>
                <div style="font-size:20px;width:100%">@Html.Raw(labels("Zone#roZonesTitle"))</div>
                <div><span id="readOnlyDescritionCompany" style="font-size: 11px;font-weight: 100;">@Html.Raw(labels("Zone#roZoneInfo"))</span></div>
            </div>
        </div>
    </div>
</div>
<br />

<div id="divMainBody" style="min-height:unset !important; height:unset !important">
    <!-- TAB SUPERIOR -->
    <div id="divTabData">

        <div class="form ro-tab-section container" style="display: flex;">

            <div class="sectionOnBoarding" style="flex-grow: 1;" id="section2">
                <div id="zoneStatusResume" class="panHeader4">@Html.Raw(labels("Zone#roZoneStatus"))</div>

                <div class="float-container">

                    <div class="float-child">
                        <div id="divMap" style="border-left: 1vw solid white;">
                            <div id="sectionMap" style="width:100%;height:65vh">

                                @*.OnMarkerAdded("selectFirstMarker") _*@
                                @(Html.DevExtreme().Map() _
.ID("globalMap") _
.Zoom(10) _
.Height("100%") _
.Width("100%") _
.Controls(True) _
.OnReady("initializeGlobalMap") _
.Provider(GeoMapProvider.Google) _
.Type(GeoMapType.Roadmap) _
.ApiKey(Sub(k)
            k.Google(mapsKey)
            k.GoogleStatic(mapsKey)
        End Sub) _
                            )
                            </div>
                        </div>
                    </div>

                    <div class="float-child2">
                        <div id="divGrid" style="border-left: 1vw solid white;">

                            <div style="display:flex;">
                                <div id="employeeStatusResume" style="margin-right: 15px; width:100%" class="panHeaderZonesSmall">
                                    <div id="divHelpMain" style="float:left; margin-top: 15px; margin-left: 15px; font-size: 13px;">
                                        @Html.Raw(labels("Zone#roCurrentZone"))
                                    </div>
                                    <div id="divSelectorGroup" style="margin: 5px; float:right">
                                        <div>
                                            @(Html.DevExtreme().SelectBox() _
.DataSource(ViewBag.ZonesLite) _
.DisplayExpr("NameExtended") _
.Width(250) _
.ID("ZonesList") _
.DeferRendering(False) _
.SearchEnabled(True) _
.ShowClearButton(True) _
.OnContentReady("onContentReady") _
.OnValueChanged("refreshGrid") _
                            )
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />

                            <div class="panHeaderZoneSmall2" style="margin-right: 15px;">
                                @Html.Raw(labels("Zone#roZoneStatusNowPresent"))
                            </div>
                            <br />
                            <div id="divEmployees" runat="server" class="jsGridContentStart dextremeGrid">
                                @(Html.DevExtreme().DataGrid() _
.ID("gridStatusEmployees") _
.DataSource(Function(ds)
                Return ds.Mvc() _
                .Controller("ZonesStatus") _
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
.OnCellClick("selection_changed") _
.ColumnAutoWidth(False) _
.AllowColumnResizing(True) _
.NoDataText(ZonesStatusController.GetServerLanguage().Translate("roNoData", "Zone")) _
.LoadPanel(Sub(columns)
               columns.Text(labels("Zone#roStartLoading"))
           End Sub) _
.Paging(Sub(columns)
            columns.PageSize(10)
        End Sub) _
.Pager(Sub(columns)
           columns.ShowPageSizeSelector(True)
           columns.AllowedPageSizes({25, 50, 100})
           columns.ShowInfo(False)
       End Sub) _
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
             .CellTemplate("<div class='photoStart'style='cursor: pointer;'><div id='absenceIcon'><img src = ' <%- value %>' height='32' style='border-radius:50%' /></div></div>")
             columns.Add().DataField("EmployeeName").Caption(labels("Zone#roStartUser"))
             columns.Add().DataField("PresenceStatus").Caption("Estado").Visible(False)
             columns.Add().DataField("InRealTimeTC").Caption("InRealTimeTC").Visible(False)
             columns.Add().DataField("InTelecommute").Visible(False).Caption("InTelecommute")
             columns.Add().DataField("LastPunchFormattedDateTime").CalculateSortValue("RealLastPunch").Caption(labels("Zone#roStartHour"))
             columns.Add().DataField("InTelecommuteImage").HeaderFilter(Sub(filter)
                                                                            filter.DataSource(New JS("headerFilterTelecommute"))
                                                                        End Sub) _
             .Caption(labels("Zone#roStartTelecommute")).CellTemplate("<div class='photoStart'><img src = ' <%- value %>' height='32'  /></div>")
             columns.Add().DataField("CostCenterName").Caption(labels("Zone#roStartCC"))
             columns.Add().DataField("TaskName").Caption(labels("Zone#roStartTask"))
         End Sub) _
                                                                                )
                            </div>
                            <br />
                            <div class="panHeaderZoneSmall2" style="margin-right: 15px; margin-top: 15px;">
                                @Html.Raw(labels("Zone#roZoneStatusLastHour"))
                            </div>
                            <br />
                            <div id="divEmployeesLastHour" runat="server" class="jsGridContentStart dextremeGrid">
                                @(Html.DevExtreme().DataGrid() _
.ID("gridStatusEmployeesLastHour") _
.DataSource(Function(ds)
                Return ds.Mvc() _
                .Controller("ZonesStatus") _
                .LoadAction("GetEmployeesInZoneDuringLastHour") _
                .LoadParams(New With {.hasData = New JS("hasData")}) _
                .OnBeforeSend("beforeSendLastHour")
            End Function) _
.ShowColumnLines(False) _
.ShowRowLines(True) _
.Selection(Sub(columns)
               columns.Mode(SelectionMode.Single)
           End Sub) _
.RowAlternationEnabled(False) _
.ShowBorders(False) _
.ColumnHidingEnabled(False) _
.OnCellClick("selection_changed") _
.ColumnAutoWidth(False) _
.AllowColumnResizing(True) _
.NoDataText(ZonesStatusController.GetServerLanguage().Translate("roNoData", "Zone")) _
.LoadPanel(Sub(columns)
               columns.Text(labels("Zone#roStartLoading"))
           End Sub) _
.Paging(Sub(columns)
            columns.PageSize(10)
        End Sub) _
.Pager(Sub(columns)
           columns.ShowPageSizeSelector(True)
           columns.AllowedPageSizes({25, 50, 100})
           columns.ShowInfo(False)
       End Sub) _
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
             .CellTemplate("<div class='photoStart'style='cursor: pointer;'><div id='absenceIcon'><img src = ' <%- value %>' height='32' style='border-radius:50%' /></div></div>")
             columns.Add().DataField("EmployeeName").Caption(labels("Zone#roStartUser"))
             columns.Add().DataField("PresenceStatus").Caption("Estado").Visible(False)
             columns.Add().DataField("LastPunchFormattedDateTime").CalculateSortValue("RealLastPunch").Caption(labels("Zone#roStartHour"))
         End Sub) _
                                                                                )
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<script>
</script>