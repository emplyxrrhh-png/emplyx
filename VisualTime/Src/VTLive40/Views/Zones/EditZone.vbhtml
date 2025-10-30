@Imports Robotics.Web.Base

@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim ZonesController As VTLive40.ZonesController = New VTLive40.ZonesController()
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)

    Dim mapsKey As String = ZonesController.GetGoogleAPIKey() & "&libraries=drawing,places&language=" & HelperWeb.GetCookie("VTLive_Language") & "&loading=async"
End Code
@Code
    Html.DevExtreme().ScrollView() _
.ID("scrollView") _
.ShowScrollbar(ShowScrollbarMode.Always) _
.Direction(ScrollDirection.Both) _
.Content(Sub()
@<text>
    <div class="aParent" style="float:left; margin-bottom:20px;width:70%;">
        <div id="zoneConfiguration" class="bTabZones" style="height:35px !important; cursor: pointer;">
            @Html.Raw(labels("Zones#roZoneConfiguration"))
        </div>
        <div id="zoneAdvanced" class="bTabZones" style="height: 35px !important; cursor: pointer;">
            @Html.Raw(labels("Zones#roZoneAdvanced"))
        </div>
    </div>
    <div class="aParent" style="float:right; margin-top:1px;margin-bottom:20px;width:20%;margin-right:10px">
        <div id="zoneSave" style="display: inline-block;margin-left:10px; float:right;">
            @(Html.DevExtreme().Button() _
                                                                                       .ID("btnAddNewZone") _
                                                                                       .Icon("todo") _
                                                                                       .OnClick("addNewZone") _
                                                                                       .Type(ButtonType.Default) _
            )
            @(Html.DevExtreme().Button() _
                                                   .ID("btnEditZone") _
                                                   .Icon("todo") _
                                                   .Type(ButtonType.Default) _
                                                   .OnClick("saveEditedZone") _
            )
        </div>
        <div id="zoneCancel" style="display: inline-block; margin-left: 10px;float:right;">
            @(Html.DevExtreme().Button() _
                                               .ID("btnCancelSurvey") _
                                               .Icon("close") _
                                               .OnClick("cancelZone") _
                                               .Type(ButtonType.Danger) _
            )
        </div>
    </div>
    <div id="configDiv" class="leftPane">
        <div class="panHeader4" style="float:left;width:100%;padding: 10px;">
            @Html.Raw(labels("Zones#headerTitleNewZone"))
        </div>

        <div class="list-containerRequests" style="height:90%;width:100%;float:left;">
            <br />
            <div style="width: 100%;height:100%;margin: 0 auto;">
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                    <div class="dx-field-label">
                        @Html.Raw(labels("Zones#name"))
                    </div>
                    <div class="dx-field-value">
                        @(Html.DevExtreme().TextBox() _
.ID("txtName") _
                    )
                    </div>
                </div>
                <br />
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                    <div class="dx-field-label">
                        @Html.Raw(labels("Zones#descriptionLabel"))
                    </div>
                    <div class="dx-field-value">
                        @(Html.DevExtreme.TextArea() _
.ID("txtDescription") _
.Height(35)
                    )
                    </div>
                </div>
                <br />
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                    <div class="dx-field-label">
                        @Html.Raw(labels("Zones#color"))
                    </div>
                    <div class="dx-field-value">
                        @(Html.DevExtreme().ColorBox() _
.ID("selectColor") _
.ApplyButtonText(ZonesController.GetServerLanguage().Translate("AccessZones.dxColorPicker.AcceptButton", "AccessZones")) _
.CancelButtonText(ZonesController.GetServerLanguage().Translate("AccessZones.dxColorPicker.CancelButton", "AccessZones")) _
.ApplyValueMode(EditorApplyValueMode.Instantly) _
.OpenOnFieldClick("true") _
.OnValueChanged("onColorChanged")
                    )
                    </div>
                </div>
                <br />
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                    <div class="dx-field-label">
                        @Html.Raw(labels("Zones#timeZone"))
                    </div>
                    <div class="dx-field-value">
                        @(Html.DevExtreme.SelectBox() _
.ID("selectTimeZone") _
.DataSource(ViewBag.DefaultTimeZones) _
.ValueExpr("Id") _
.DisplayExpr("DisplayName") _
.ShowSelectionControls(True) _
.ShowClearButton(True) _
.SearchEnabled(True) _
.SearchExpr("DisplayName") _
.Placeholder(labels("Zones#selectTimeZone")) _
.OnOpened("onTimeZoneOpened") _
                    )
                    </div>
                </div>
                <br />

                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                    <div class="dx-field-label" style="white-space:normal;">
                        @Html.Raw(labels("Zones#isEmergencyZone"))
                    </div>
                    <div class="dx-field-value" style="display:flex">
                        @(Html.DevExtreme().Switch() _
.ID("chkIsEmergencyZone") _
.Value(False) _
.SwitchedOffText(labels("Zones#lblNo")) _
.SwitchedOnText(labels("Zones#lblYes")) _
                )
                    </div>
                </div>
                <br />
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                    @If (ViewBag.TelecommutingInstalled) Then
                        @<div class="dx-field-label" style="white-space:normal;">
                            @Html.Raw(labels("Zones#isTelecommutingZone"))
                        </div>
                    End If
                    <div class="dx-field-value" style="display:flex">
                        @(Html.DevExtreme().SelectBox() _
.ID("selectTelecommutingType") _
.Placeholder(labels("Zones#selectTelecommutingType")) _
.ShowSelectionControls(True) _
.ValueExpr("ID") _
.DisplayExpr("Name") _
.SearchExpr("Name") _
.DataSource(ViewBag.ZoneTypes) _
.Visible(ViewBag.TelecommutingInstalled) _
.ShowSelectionControls("False")
                )
                    </div>
                </div>
                <br />
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                    <div class="dx-field-label" style="white-space:normal;">
                        @Html.Raw(labels("Zones#zoneLocation"))
                    </div>
                    <div class="dx-field-value" style="display:flex">
                        @(Html.DevExtreme().Switch() _
.ID("chkZoneNameAsLocation") _
.Value(False) _
.SwitchedOffText(labels("Zones#lblNo")) _
.SwitchedOnText(labels("Zones#lblYes")) _
                )
                    </div>
                </div>
                <br />
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                    <div class="dx-field-label" style="white-space:normal;">
                        @Html.Raw(labels("Zones#zoneWorkCenter"))
                    </div>
                    <div class="dx-field-value" style="display:flex">
                        @(Html.DevExtreme().SelectBox() _
.ID("selectWorkCenter") _
.Placeholder(labels("Zones#selectZoneWorkCenter")) _
.ShowSelectionControls(True) _
.ValueExpr("Name") _
.DisplayExpr("Name") _
.ShowClearButton(True) _
.SearchEnabled(True) _
.SearchExpr("Name") _
.DataSource(ViewBag.AvailableWorkCenters) _
.ShowSelectionControls("False")
                )
                    </div>
                </div>
                <br />
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">

                    <div class="dx-field-label">
                        @(Html.DevExtreme().CheckBox() _
.ID("chkZoneCapacity") _
.Value(False) _
.OnValueChanged("maxCapacity") _
.Text(labels("Zones#maxCapacity")) _
.Visible(ViewBag.TelecommutingInstalled AndAlso ViewBag.CapacityManagementInstalled) _
                    )
                    </div>
                    <div id="divMaxCapacity" class="dx-field-value">

                        @(Html.DevExtreme().TextBox() _
.ID("txtZoneCapacity") _
.Disabled(True) _
.Width(100) _
.Visible(ViewBag.TelecommutingInstalled AndAlso ViewBag.CapacityManagementInstalled) _
                    )
                        @If (ViewBag.TelecommutingInstalled AndAlso ViewBag.CapacityManagementInstalled) Then
                            @<div class="dx-field-label" style="float:left;margin-left:10px">
                                @Html.Raw(labels("Zones#numberPeople"))
                            </div>
                        End If
                    </div>
                </div>
                <br />
                <div class="divShowZoneCapacity" style="display:none;padding-left:50px;justify-content: center;float:left;width:100%;">

                    @If (ViewBag.TelecommutingInstalled AndAlso ViewBag.CapacityManagementInstalled) Then
                        @<div Class="dx-field-label" style="white-space: normal;">
                            @Html.Raw(labels("Zones#showZoneCapacity"))
                        </div>
                    End If
                    <div class="dx-field-value" style="display:flex">

                        @(Html.DevExtreme().Switch() _
.ID("chkShowZoneCapacity") _
.Value(False) _
.SwitchedOffText(labels("Zones#lblNo")) _
.SwitchedOnText(labels("Zones#lblYes")) _
.Visible(ViewBag.TelecommutingInstalled AndAlso ViewBag.CapacityManagementInstalled) _
                )
                    </div>
                    <br />
                </div>
                <br />
                <div class="divShowZoneCapacity" style="display:none;padding-left:50px;justify-content: center;float:left;width:100%;margin-top:5px;margin-bottom:5px;">

                    @If (ViewBag.TelecommutingInstalled AndAlso ViewBag.CapacityManagementInstalled) Then
                        @<div Class="dx-field-label" style="white-space: normal;">
                            @Html.Raw(labels("Zones#zoneSupervisor"))
                        </div>
                    End If
                    <div class="dx-field-value" style="display:flex">

                        @(Html.DevExtreme().SelectBox() _
.ID("selectZoneSupervisor") _
.Placeholder(labels("Zones#selectZoneSupervisor")) _
.ShowSelectionControls(True) _
.ValueExpr("ID") _
.DisplayExpr("Name") _
.ShowClearButton(True) _
.SearchEnabled(True) _
.SearchExpr("Name") _
.DataSource(ViewBag.AvailableSupervisors) _
.ShowSelectionControls("False")
                )
                    </div>
                    <br />
                </div>
                <br />
            </div>
        </div>
    </div>
    <div id="advancedDiv" class="leftPane">
        <div class="panHeaderListOptional" style="float:left;width:100%;padding: 10px;">
            @Html.Raw(labels("Zones#lblZoneType"))
        </div>
        <br />
        <br />
        <div class="list-containerRequests" style="width:100%;float:left;">

            <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">
                <div class="dx-field-label" style="white-space:normal;">
                    @Html.Raw(labels("Zones#isWorkingZone"))
                </div>
                <div class="dx-field-value" style="display:flex">
                    @(Html.DevExtreme().Switch() _
.ID("selectType") _
.Value(True) _
.SwitchedOffText(labels("Zones#lblNo")) _
.SwitchedOnText(labels("Zones#lblYes")) _
                )
                </div>
            </div>
        </div>
        <div class="panHeaderListOptional" style="float:left;width:100%;padding: 10px;">
            @Html.Raw(labels("Zones#lblAccessZonesMainTitlePeriods"))
        </div>
        <br />
        <br />

        <div class="list-containerRequests" style="width:100%;float:left;">
            <div class="panHeaderListOptional" style="float:left;width:100%;padding: 5px;margin-top:10px;margin-bottom:10px;">
                @Html.Raw(labels("Zones#lblZonesInactivityTitle"))
            </div>
            <div style="width: 100%;height:100%;margin: 0 auto;">
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">

                    @Code
                        Html.DevExtreme().DataGrid() _
                        .ID("gridInactiviyZones") _
                        .OnInitNewRow("inactivityZonesNewRow") _
                        .OnRowInserting("inactivityZoneInserting") _
                        .OnRowUpdating("inactivityZoneUpdating") _
                        .Columns(Sub(columns)
                                                            columns.Add().DataField("IDZone").Visible(False)
                                                            columns.Add().DataField("WeekDay").Visible(True).Option("editCellTemplate", New JS("editSelectDayCell")).Option("customizeText", New JS("calculateWeekDay")).Caption(labels("Zones#weekDayName"))
                                                            columns.Add().DataField("Begin").Format(Format.ShortTime).AllowEditing(True).Option("editCellTemplate", New JS("editTimeCell")).Caption(labels("Zones#dateBegin"))
                                                            columns.Add().DataField("End").Format(Format.ShortTime).AllowEditing(True).Option("editCellTemplate", New JS("editTimeCell")).Caption(labels("Zones#dateEnd"))
                                                        End Sub) _
        .ShowColumnLines(False) _
.ShowRowLines(True) _
.Editing(Sub(edit)
                               edit.AllowDeleting(True)
                               edit.AllowUpdating(True)
                               edit.AllowAdding(True)
                               edit.Mode(GridEditMode.Cell)
                               edit.RefreshMode(GridEditRefreshMode.Reshape)
                               edit.Texts(Sub(texts)
                                              texts.ConfirmDeleteMessage(labels("Zones#deleteInactivityZoneInfo"))
                                              texts.ConfirmDeleteTitle(labels("Zones#deleteInactivityZoneHeader"))
                                          End Sub)
                               edit.UseIcons(True)
                           End Sub) _
    .RowAlternationEnabled(False) _
.ShowBorders(False) _
.ColumnHidingEnabled(False) _
.ColumnAutoWidth(False) _
.AllowColumnResizing(True) _
.NoDataText(labels("Zones#inactivityEmpty").ToString()) _
.LoadPanel(Sub(columns)
                                 columns.Text(labels("Zones#loading"))
                             End Sub) _
.Paging(Sub(columns)
                              columns.PageSize(25)
                          End Sub) _
.Pager(Sub(columns)
                             columns.ShowPageSizeSelector(True)
                             columns.AllowedPageSizes({25, 50, 100})
                             columns.ShowInfo(False)
                         End Sub) _
.Render()

                    End Code
                </div>
            </div>
            <br />
        </div>
        <div class="list-containerRequests" style="width:100%;float:left;">
            <div class="panHeaderListOptional" style="float:left;width:100%;padding: 5px;margin-top:10px;margin-bottom:10px;">
                @Html.Raw(labels("Zones#lblZonesExceptionTitle"))
            </div>
            <div style="width: 100%;height:100%;margin: 0 auto;">
                <div class="configField" style="padding-left:50px;display: flex;justify-content: center;">

                    @Code
                        Html.DevExtreme().DataGrid() _
                                .ID("gridExceptionZones") _
                                .OnInitNewRow("exceptionZonesNewRow") _
                                .OnRowInserting("exceptionZoneInserting") _
                                .Columns(Sub(columns)
                                                                    columns.Add().DataField("IDZone").Visible(False)
                                                                    columns.Add().DataField("ExceptionDate").Format(Format.ShortDate).AllowEditing(True).Option("editCellTemplate", New JS("editDateCell")).Caption(labels("Zones#dateException"))
                                                                End Sub) _
                                         .ShowColumnLines(False) _
                            .ShowRowLines(True) _
                            .Editing(Sub(edit)
                                                                edit.AllowDeleting(True)
                                                                edit.AllowUpdating(True)
                                                                edit.AllowAdding(True)
                                                                edit.Mode(GridEditMode.Cell)
                                                                edit.RefreshMode(GridEditRefreshMode.Reshape)
                                                                edit.Texts(Sub(texts)
                                                                               texts.ConfirmDeleteMessage(labels("Zones#deleteExceptionZoneInfo"))
                                                                               texts.ConfirmDeleteTitle(labels("Zones#deleteExceptionZoneHeader"))
                                                                           End Sub)
                                                                edit.UseIcons(True)
                                                            End Sub) _
                                     .RowAlternationEnabled(False) _
                            .ShowBorders(False) _
                            .ColumnHidingEnabled(False) _
                            .ColumnAutoWidth(False) _
                            .AllowColumnResizing(True) _
                            .NoDataText(labels("Zones#exceptionEmpty").ToString()) _
                            .LoadPanel(Sub(columns)
                                                                  columns.Text(labels("Zones#loading"))
                                                              End Sub) _
                            .Paging(Sub(columns)
                                                               columns.PageSize(25)
                                                           End Sub) _
                            .Pager(Sub(columns)
                                                              columns.ShowPageSizeSelector(True)
                                                              columns.AllowedPageSizes({25, 50, 100})
                                                              columns.ShowInfo(False)
                                                          End Sub) _
                .Render()

                    End Code
                </div>
            </div>
            <br />
        </div>
    </div>
    <div class="rightPane">
        <div id="ipsStatus" class="editZoneIps" style="padding-left:15px ;justify-content: center; height:100%; width:100%;">
            <div class="panHeader4" style="width:100%;padding: 10px;">
                @Html.Raw(labels("Zones#zoneAllowedIp"))
            </div>
            <br />
            <div class="list-containerRequests" style="height:90%;width:100%">
                @Code
                    Html.DevExtreme().DataGrid() _
          .ID("gridIpRestrictions") _
          .Columns(Sub(columns)
                                          columns.Add().DataField("Ip").Visible(True).Caption(labels("Zones#ips"))
                                      End Sub) _
                .ShowColumnLines(False) _
                .ShowRowLines(True) _
                .Editing(Sub(edit)
                                                edit.AllowDeleting(True)
                                                edit.AllowUpdating(True)
                                                edit.AllowAdding(True)
                                                edit.Mode(GridEditMode.Row)
                                                edit.RefreshMode(GridEditRefreshMode.Reshape)
                                                edit.UseIcons(True)
                                            End Sub) _
                .RowAlternationEnabled(False) _
                .ShowBorders(False) _
                .ColumnHidingEnabled(False) _
                .ColumnAutoWidth(False) _
                .AllowColumnResizing(True) _
                .NoDataText(labels("Zones#ipsEmpty").ToString()) _
                .LoadPanel(Sub(columns)
                                                  columns.Text(labels("Zones#loading"))
                                              End Sub) _
                .Paging(Sub(columns)
                                               columns.PageSize(25)
                                           End Sub) _
                .Pager(Sub(columns)
                                              columns.ShowPageSizeSelector(True)
                                              columns.AllowedPageSizes({25, 50, 100})
                                              columns.ShowInfo(False)
                                          End Sub).Render()

                End Code
            </div>

        </div>

        <div class="editZoneMap" style="display: flex;justify-content: center; height:100%; width:100%;">
            @(Html.DevExtreme().Map() _
                              .ID("map") _
                              .Zoom(10) _
                              .Height("90%") _
                              .Width("90%") _
                              .Provider(GeoMapProvider.Google) _
                              .Type(GeoMapType.Roadmap) _
                              .ApiKey(Sub(k)
                                          k.Google(mapsKey)
                                          k.GoogleStatic(mapsKey)
                                      End Sub) _
                             .OnReady("onMapReady") _
                             .OnInitialized("onInitialized") )
            <input id="googlePolCords" type="hidden" value="" />
            <input id="googleRectCords" type="hidden" value="" />
            <input id="currentLocation" type="hidden" value="{ lat: 0, lng: 0 }" />
            <input id="googleMapCenter" type="hidden" value="" />
            <input id="googleMapZoom" type="hidden" value="10" />
            <input id="zoneShape" type="hidden" value="" />
            <input id="zoneArea" type="hidden" value="" />
        </div>
    </div>
</text>
End Sub).Render()
End Code