@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim enableAdvancedMode As Boolean = ViewBag.enableAdvancedMode
    Dim enableAdvancedFilters As Boolean = ViewBag.advancedFilterEnabled
    Dim allowSelectAll As Boolean = ViewBag.allowSelectAll
    Dim allowSelectNone As Boolean = ViewBag.allowSelectNone
    Dim unionType As String = ViewBag.unionType

    Dim empDS = If(ViewBag.AvailableGroups Is Nothing, {}, ViewBag.AvailableEmployees)
    Dim groupDS = If(ViewBag.AvailableGroups Is Nothing, {}, ViewBag.AvailableGroups)
    Dim colDS = If(ViewBag.AvailableCollectives Is Nothing, {}, ViewBag.AvailableCollectives)
    Dim labagreeDS = If(ViewBag.AvailableLabAgrees Is Nothing, {}, ViewBag.AvailableLabAgrees)

End Code

<div id="simpleUniversalSelector" style="display: flex; flex-direction: column;max-width:990px;gap:10px">

    <div class="panHeader4" style="padding: 10px;">
        @Html.Raw(labels("EmployeeSelector#roDestination"))
    </div>

    <div style="display: @Html.Raw(If(allowSelectAll, "flex", "none"))">
        <label>@Html.RadioButton("rbSelectionMode", "All", False, New With {.onchange = "updateUniversalSelectorMode(this)"})  @Html.Raw(labels("EmployeeSelector#selectall"))</label>
    </div>



    <div style="display: @Html.Raw(If(allowSelectAll, "flex", "none"))">
        <label>@Html.RadioButton("rbSelectionMode", "Custom", False, New With {.onchange = "updateUniversalSelectorMode(this)"}) @Html.Raw(labels("EmployeeSelector#selectcustom"))</label>
    </div>

    <div class="usCustomFilter" style="display:none;flex-direction:row">
        <div style="display: @Html.Raw(If(allowSelectAll, "flex", "none"));min-width:20px">&nbsp;</div>


        <div style="display: flex; flex-direction: column; gap:20px" class="list-containerRequests">
            <div style="display: flex; flex-direction: column; width: 95%; margin: 0 auto; gap: 10px; padding-top:10px">
                <div style="display: @Html.Raw(If(ViewBag.AvailableGroups Is Nothing, "none", "flex")); flex-direction: column; padding: 10px; border-radius: 5px; background-color: #f7f7f7; gap: 10px ">
                    <div class="selectorField" style="display:flex ;justify-content: center;">
                        <div class="dx-field-label">
                            @Html.Raw(labels("EmployeeSelector#roGroupsDestination"))
                        </div>
                        <div class="dx-field-value">
                            @(Html.DevExtreme().TagBox() _
                .ID("GroupText") _
                .Width(660) _
                .Placeholder(labels("EmployeeSelector#rogroupplaceholder")) _
                .ShowSelectionControls(True) _
                .ValueExpr("IdGroup") _
                .DisplayExpr("Name") _
                .Multiline(True) _
                .ShowClearButton(True) _
                .SearchEnabled(True) _
                .ApplyValueMode(EditorApplyValueMode.UseButtons) _
                .SearchExpr("Name") _
                .OnSelectionChanged("employeeMVCSelector_groupSelected") _
                .DataSource(groupDS))
                        </div>
                    </div>

                    <div class="selectorField" style="display: flex;justify-content: center;">
                        <div class="dx-field-label">
                            @Html.Raw(labels("EmployeeSelector#roEmployeeDestination"))
                        </div>
                        <div class="dx-field-value">
                            @(Html.DevExtreme().TagBox() _
                    .ID("EmployeeText") _
                    .Width(660) _
                    .Placeholder(labels("EmployeeSelector#roemployeeplaceholder")) _
                    .ShowSelectionControls(True) _
                    .ValueExpr("IdEmployee") _
                    .DisplayExpr("EmployeeName") _
                    .Multiline(True) _
                    .ShowClearButton(True) _
                    .SearchEnabled(True) _
                    .ApplyValueMode(EditorApplyValueMode.UseButtons) _
                    .SearchExpr("EmployeeName") _
                    .OnSelectionChanged("employeeMVCSelector_empSelected") _
                    .ItemTemplate("<div class='customEmployee-item'><img style='float:left;' src=' <%- Image %>' ><div style='margin-left: 40px; line-height: 34px;'><%- EmployeeName %></div></div>") _
                    .DataSource(empDS))
                        </div>
                    </div>

                    <div style="display: @Html.Raw(If(enableAdvancedMode AndAlso enableAdvancedFilters, "flex", "none")); padding-bottom: 10px; flex-direction: row-reverse; gap:10px;">


                        @(Html.DevExtreme().CheckBox() _
                        .ID("userFieldsFilter") _
                        .Disabled(True) _
                        .Text(labels("EmployeeSelector#advancedfilterincluded")))
                    </div>

                </div>


                <div style="display: @Html.Raw(If(ViewBag.AvailableCollectives Is Nothing, "none", "flex")); flex-direction: column; padding: 10px; border-radius: 5px; background-color: #f7f7f7;">
                    <div class="selectorField" style="clear:both;display: flex;justify-content: center;">
                        <div class="dx-field-label">
                            @Html.Raw(labels("EmployeeSelector#rocollectivedestination"))
                        </div>
                        <div class="dx-field-value">
                            @(Html.DevExtreme().TagBox() _
                .ID("CollectiveText") _
                .Width(660) _
                .Placeholder(labels("EmployeeSelector#rocollectiveplaceholder")) _
                .ShowSelectionControls(True) _
                .ValueExpr("FieldValue") _
                .DisplayExpr("FieldName") _
                .Multiline(True) _
                .ShowClearButton(True) _
                .SearchEnabled(True) _
                .ApplyValueMode(EditorApplyValueMode.UseButtons) _
                .SearchExpr("FieldName") _
                .OnSelectionChanged("employeeMVCSelector_collectiveSelected") _
                .DataSource(colDS))
                        </div>
                    </div>
                </div>

                <div style="display: @Html.Raw(If(ViewBag.AvailableLabAgrees Is Nothing, "none", "flex")); flex-direction: column; padding: 10px; border-radius: 5px; background-color: #f7f7f7;">
                    <div class="selectorField" style="display: flex;justify-content: center;">
                        <div class="dx-field-label">
                            @Html.Raw(labels("EmployeeSelector#rolabagreedestination"))
                        </div>
                        <div class="dx-field-value">
                            @(Html.DevExtreme().TagBox() _
                .ID("LabagreeText") _
                .Width(660) _
                .Placeholder(labels("EmployeeSelector#rolabagreeplaceholder")) _
                .ShowSelectionControls(True) _
                .ValueExpr("FieldValue") _
                .DisplayExpr("FieldName") _
                .Multiline(True) _
                .ShowClearButton(True) _
                .SearchEnabled(True) _
                .ApplyValueMode(EditorApplyValueMode.UseButtons) _
                .SearchExpr("FieldName") _
                .OnSelectionChanged("employeeMVCSelector_labagreeSelected") _
                .DataSource(labagreeDS))
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>


    <div class="usNoneFilter" style="display: @Html.Raw(If(allowSelectAll AndAlso allowSelectNone, "flex", "none"))">
        <label>@Html.RadioButton("rbSelectionMode", "None", False, New With {.onchange = "updateUniversalSelectorMode(this)"}) @Html.Raw(labels("EmployeeSelector#selectnone"))</label>
    </div>

    <div class="filterActionButtons" style="display:flex; flex-direction:row">
        <div style="min-width:200px">
            @(Html.DevExtreme().Switch() _
                                    .ID("selectOperation") _
                                    .Visible((unionType = "custom")) _
                                    .Width(150) _
                                    .SwitchedOnText(labels("EmployeeSelector#intersection")) _
                                    .SwitchedOffText(labels("EmployeeSelector#union")))

        </div>

        <div style="display: flex; flex-direction: row-reverse; gap:10px; width: @Html.Raw(ViewBag.ClientWidth.ToString());">

            @(Html.DevExtreme().Button() _
                                    .ID("saveDestination") _
                                    .Icon("todo") _
                                    .OnClick("saveDestination") _
                                    .Type(ButtonType.Default))
            @(Html.DevExtreme().Button() _
                            .ID("cleanFilter") _
                            .Icon("trash") _
                            .OnClick("cleanFilter") _
                            .Type(ButtonType.Danger))
            @(Html.DevExtreme().Button() _
                                        .ID("showAdvanced") _
                                        .Icon("preferences") _
                                        .Visible(enableAdvancedMode) _
                                        .OnClick("showAdvancedSelector") _
                                        .Type(ButtonType.Success))
        </div>
    </div>
</div>