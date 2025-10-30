@Imports Robotics.Web.Base
@Imports Robotics.Web.Base.roLanguageWeb
@Imports Robotics.Base.VTBusiness.Zone
@ModelType roZone

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim ZonesController As VTLive40.ZonesController = New VTLive40.ZonesController()
    Dim baseURL = Url.Content("~")
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim scriptVersion As String = ZonesController.ScriptsVersion
    Dim bIpStatus As Boolean = ViewBag.ZoneRestrictedByIP
    Dim mapsKey As String = ZonesController.GetGoogleAPIKey() & "&libraries=drawing,places&language=" & HelperWeb.GetCookie("VTLive_Language") & "&loading=async"
End Code
<script>var BASE_URL ="@baseURL"; </script>
<link href="@Url.Content("~/Base/Styles/roStart.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<script src="@Url.Content("~/Base/Scripts/Ajax.js")" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Zones/roZonesScript.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Zones/roEditZoneScript.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<div id="news" style="float:left"></div>
<div id="newsOk" style="float:right; margin-top:60px;"></div>

<div style="display:flex;">
    <div style="border-left: 1vw solid white;border-top: 1vw solid white; width:100%;border-right: 1vw solid white;">
        <div class="panHeader5" style="display: flex;align-items: center;">
            <div style="float: left; width: 90%;">
                <div style="float:left;width:50px;margin-right:0.5vw;margin-top:5px;"><img src="~/Access/Images/AccessZones.png" width="48" height="48" /></div>
                <div style="float:left;width:90%;margin-left:10px;">
                    <div style="font-size:20px;width:100%">@Html.Raw(labels("Zones#headerTitle"))</div>
                    <div>
                        <span id="zonesDescriptionInformation" style="font-size: 11px;font-weight: 100;">@Html.Raw(labels("Zones#description"))</span>
                    </div>
                </div>
            </div>
            <div class="blackRibbonButtons" style="width:170px;">
                @*<div id="zoneStatus" class="bTabZonesMenu bTabZonesMenu-active" style="cursor: pointer;">
                        @Html.Raw(labels("Zones#zoneStatus"))
                    </div>*@
                @*<div id="zoneAdministration" class="bTabZonesMenu bTabZonesMenu-active" style="cursor: pointer;">
                        @Html.Raw(labels("Zones#zoneAdministration"))
                    </div>*@
            </div>
        </div>
    </div>
</div>
<br />

<input id="idZoneEdit" type="hidden" />

<div id="divMainBody" style="min-height: unset !important; height: 85vh;">
    <!-- TAB SUPERIOR -->
    <div id="divTabData" style="height:100%">
        <input id="idList" type="hidden" value="" />
        <input id="zoneRemoved" type="hidden" value="" />
        <div id="divContenido" Class="divAllContentZones twoWindowsFlexLayout" style="height:100%">
            <div id="administrationDiv" style="" class="twoWindowsMainPanel maxHeight divRightContent">

                <div class="form ro-tab-section container" style="display: flex;height:100%;">

                    <div class="sectionZones" style="flex-grow: 1; height: 100%" id="divSectionZones">

                        <div id="zonesWorkingMode" class="panHeader4">@Html.Raw(labels("Zones#zonesWorkingMode"))</div>
                        <br />

                        <div id="zonesWorkingModeInfo" style="display: flex;justify-content: right; height: auto; width: 100%; margin-left:25px">
                            <div class="dx-field-label" style="overflow:inherit; width: 100%;">
                                @Html.Raw(labels("Zones#zonesWorkingModeLbl"))
                                <div style="margin-left: 1em;display:inline;">
                                    @(Html.DevExtreme().Switch() _
.ID("ckZoneLocationWorkingMode") _
.Value(bIpStatus) _
.OnValueChanged("onZoneLocationWorkingModeChanged") _
.SwitchedOffText(labels("Zones#inactiveLbl")) _
.SwitchedOnText(labels("Zones#activeLbl")))
                                </div>
                                <div class="OptionPanelDescStyle" style="overflow:inherit;padding-left:0px;padding-top:10px;">
                                    @Html.Raw(labels("Zones#zonesWorkingModeLblDesc"))
                                </div>
                            </div>
                        </div>
                        <br />

                        <div id="zonesStatusResume" class="panHeader4">@Html.Raw(labels("Zones#zonesManagement"))</div>
                        <br />

                        <div  style="display: flex;justify-content: right; height: auto; width: 100%">
                            <div id="divNewZone" style=" margin-left: auto; margin-right: 0;">
                                @(Html.DevExtreme().Button() _
.ID("btnAddZones") _
.Icon("plus") _
.OnClick("openAddNewZonePopUp") _
.Text(labels("Zones#createZone")) _
.Type(ButtonType.Default)
                                )
                            </div>
                        </div>
                        <br />
                        <div id="divZones" runat="server" Class="jsGridContentZones dextremeGrid" style="float:left; height:100%;">
                            @Code
                                Html.DevExtreme().DataGrid() _
.ID("gridZones") _
.Width("100%") _
.Height("500px") _
.DataSource(Function(ds)
                                    Return ds.Mvc() _
                                .Controller("Zones") _
                                .LoadAction("GetZones") _
                                .Key("ID") _
                                .DeleteAction("DeleteZone")
                                End Function) _
.ShowColumnLines(False) _
.ShowRowLines(True) _
.Editing(Sub(edit)
                                 edit.AllowDeleting(True)
                                 edit.AllowUpdating(False)
                                 edit.Mode(GridEditMode.Cell)
                                 edit.RefreshMode(GridEditRefreshMode.Reshape)
                                 edit.Texts(Sub(texts)
                                                texts.ConfirmDeleteMessage(labels("Zones#deleteZoneInfo"))
                                                texts.ConfirmDeleteTitle(labels("Zones#deleteZoneHeader"))
                                            End Sub)
                                 edit.UseIcons(True)
                             End Sub) _
.Selection(Sub(columns)
                                   columns.Mode(SelectionMode.Multiple).ShowCheckBoxesMode(GridSelectionShowCheckBoxesMode.Always).AllowSelectAll(True)
                               End Sub) _
.RowAlternationEnabled(False) _
.ShowBorders(False) _
.ColumnHidingEnabled(False) _
.ColumnAutoWidth(False) _
.AllowColumnResizing(True) _
.OnRowRemoved("RefreshZonesList") _
.Export(Sub(columns)
                                columns.Enabled(False)
                                columns.AllowExportSelectedData(True)
                            End Sub) _
.NoDataText(labels("Zones#empty").ToString()) _
.LoadPanel(Sub(columns)
                                   columns.Text(labels("Zones#loading"))
                               End Sub) _
.Paging(Sub(columns)
                                columns.PageSize(10)
                            End Sub) _
.OnCellClick("OnZoneClick") _
.OnContentReady("selectZoneByDefault") _
.OnSelectionChanged("selection_changed") _
.FilterRow(Sub(columns)
                                   columns.Visible(True)
                                   columns.Visible(True)
                               End Sub) _
.HeaderFilter(Sub(columns)
                                      columns.Visible(True)
                                  End Sub) _
.Columns(Sub(columns)
                                 columns.Add().DataField("ID").Visible(False)
                                 columns.Add().DataField("GoogleMapInfo").Visible(False)
                                 columns.Add().DataField("Color").Visible(False)
                                 columns.Add().DataField("Name").SortIndex(0).SortOrder(SortOrder.Asc).AllowEditing(False).Caption(labels("Zones#name"))
                                 columns.Add().DataField("Description").AllowEditing(False).Caption(labels("Zones#descriptionLabel"))
                                 columns.Add().DataField("WorkCenter").AllowEditing(False).Caption(labels("Zones#zoneWorkCenter"))
                                 columns.Add().DataField("IpsRestriction").Visible(bIpStatus).AllowFiltering(False).AllowEditing(False).Caption(labels("Zones#zoneAllowedIp"))
                                 columns.Add().DataField("CurrentCapacity").Visible(ViewBag.TelecommutingInstalled).CalculateSortValue("CurrentCapacityInt").AllowEditing(False).Caption(labels("Zones#zoneCurrentCapacity"))
                             End Sub) _
.Render()

                            End Code
                            @Code
                                Html.DevExtreme().Popup() _
                                                                                                    .ID("editZonePopup") _
                                                                                                    .Width(New JS("getWidth")) _
                                                                                                    .Height(New JS("getHeight")) _
                                                                                                    .ShowTitle(False) _
                                                                                                    .DragEnabled(False) _
                                                                                                    .OnShown("popUpShown") _
                                                                                                    .OnHidden("popUpHidden") _
                                                                                                    .ContentTemplate(Sub()
                                                                                                    @<text>
                                                                                                        @Html.Partial("EditZone")
                                                                                                    </text>
                                                                                                End Sub) _
.Render()
                            End Code
                        </div>
                    </div>

                    <div class="sectionMap" style="flex-grow: 2;" id="section2">

                        <div id="zonesStatusResume" class="panHeader4">@Html.Raw(labels("Zones#maps"))</div>
                        <br />
                        <div id="gmapsDiv" style="display: flex;justify-content: center; height:100%; width:100%;">
                            @(Html.DevExtreme().Map() _
                    .ID("globalMap") _
                    .Zoom(10) _
                    .Height("100%") _
                    .Width("100%") _
                    .OnReady("initializeGlobalMap") _
                    .Provider(GeoMapProvider.Google) _
                    .Type(GeoMapType.Roadmap) _
                    .ApiKey(Sub(k)
                                k.Google(mapsKey)
                                k.GoogleStatic(mapsKey)
                            End Sub))
                        </div>


                    </div>
                </div>
            </div>
        </div>
    </div>
</div>