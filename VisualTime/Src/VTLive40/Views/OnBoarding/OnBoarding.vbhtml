@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base.roLanguageWeb

@ModelType roToDoList

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim OnBoardingController As VTLive40.OnBoardingController = New VTLive40.OnBoardingController()
    Dim baseURL = Url.Content("~")
End Code
<script>var BASE_URL ="@baseURL"; </script>
<link href="@Url.Content("~/Base/Styles/roStart.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")" rel="stylesheet" type="text/css" />
<script src="@Url.Content("~/Base/Scripts/Ajax.js")" type="text/javascript"></script>

<div id="news" style="float:left"></div>
<div id="newsOk" style="float:right; margin-top:60px;"></div>

<div style="display:flex;">
    <div style="border-left: 1vw solid white;border-top: 1vw solid white; width:100%;border-right: 1vw solid white;">
        <div class="panHeader5" style="display: flex;align-items: center;">
            <div style="margin-right:0.5vw"><img src="~/Base/Images/StartMenuIcos/OnBoarding96.png" width="48" height="48" /></div>
            <div>
                <div style="font-size:20px;width:100%">OnBoarding</div>
                <div><span id="readOnlyDescritionCompany" style="font-size: 11px;font-weight: 100;">@OnBoardingController.GetServerLanguage().Translate("roOnBoardingInfo", "OnBoarding")</span></div>
            </div>
        </div>
    </div>
</div>
<br />

<div id="divMainBody" style="min-height:unset !important; height:unset !important">
    <!-- TAB SUPERIOR -->
    <div id="divTabData">
        <input id="idList" type="hidden" value="" />
        <input id="idEmployeeSelected" type="hidden" value="" />
        <input id="idCopySelected" type="hidden" value="" />
        <input id="dateSelected" type="hidden" value="" />
        <input id="onboardingRemoved" type="hidden" value="" />
        <div id="divContenido" Class="divAllContentOnBoarding twoWindowsFlexLayout">

            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent">

                <div class="form ro-tab-section container" style="display: flex;">

                    <div class="sectionOnBoarding" style="flex-grow: 1;" id="section2">

                        <div id="onBoardingStatusResume" class="panHeader4">@OnBoardingController.GetServerLanguage().Translate("roOnBoardingStatus", "OnBoarding")</div>
                        <br />

                        <div id="actionBar" style="display: flex;justify-content: right;">
                            @If (ViewBag.PermissionOverEmployees > 3) Then
                            @<div id="employeeStatus" style=" margin-left: auto; margin-right: 0;">
                                @(Html.DevExtreme().Button() _
                                            .ID("addOnBoarding") _
                                            .Icon("plus") _
                                            .OnClick("addNewList") _
                                            .Text(OnBoardingController.GetServerLanguage().Translate("roOnBoardingNew", "OnBoarding")) _
                                            .Type(ButtonType.Default))
                            </div>
                            End If
                        </div>
                        <br />

                        <div id="divOnBoardings" runat="server" Class="jsGridContentOnBoarding dextremeGrid">
                            @(Html.DevExtreme().DataGrid() _
.ID("gridStatusOnBoardings") _
.DataSource(Function(ds)
                Return ds.Mvc() _
                .Controller("OnBoarding") _
                .LoadAction("GetOnBoardings") _
                .Key("IdList") _
                .LoadParams(New With {.hasData = New JS("hasData")}) _
                .OnBeforeSend("beforeSend") _
                .UpdateAction("UpdateOnBoarding") _
                .DeleteAction("DeleteOnBoarding") _
                .OnModified("RefreshOnBoardingList")
            End Function) _
.ShowColumnLines(False) _
.ShowRowLines(True) _
.Editing(Sub(edit)
             edit.AllowDeleting(New JS("AllowModify"))
             edit.AllowUpdating(New JS("AllowModify"))
             edit.Mode(GridEditMode.Cell)
             edit.RefreshMode(GridEditRefreshMode.Reshape)
             edit.Texts(Sub(texts)
                            texts.ConfirmDeleteMessage(OnBoardingController.GetServerLanguage().Translate("roOnBoardingDelete", "OnBoarding"))
                            texts.ConfirmDeleteTitle(OnBoardingController.GetServerLanguage().Translate("roOnBoardingDeleteTitle", "OnBoarding"))
                        End Sub)
             edit.UseIcons(True)
         End Sub) _
.Selection(Sub(columns)
               columns.Mode(SelectionMode.Single)
           End Sub) _
.RowAlternationEnabled(False) _
.ShowBorders(False) _
.ColumnHidingEnabled(False) _
.ColumnAutoWidth(False) _
.AllowColumnResizing(True) _
.OnRowRemoved("OnBoardingRemoved") _
.Export(Sub(columns)
            columns.Enabled(False)
            columns.AllowExportSelectedData(True)
        End Sub) _
.NoDataText(OnBoardingController.GetServerLanguage().Translate("roNoData", "OnBoarding")) _
.LoadPanel(Sub(columns)
               columns.Text(OnBoardingController.GetServerLanguage().Translate("roOnBoardingLoading", "OnBoarding"))
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
.OnCellPrepared("cell_prepared") _
.OnContextMenuPreparing("context_menu") _
.FilterRow(Sub(columns)
               columns.Visible(True)
           End Sub) _
.HeaderFilter(Sub(columns)
                  columns.Visible(True)
              End Sub) _
.Columns(Sub(columns)
             columns.Add().DataField("Image").Caption("").AllowResizing(False).AllowEditing(False).AllowFiltering(False).Width(45).AllowSorting(False).CellTemplate("<div class='photoOnBoarding'style='cursor: pointer;'><div id='absenceIcon'><img src = ' <%- value %>' height='32' style='border-radius:50%' /></div></div>")
             columns.Add().DataField("IdEmployee").Visible(False)
             columns.Add().DataField("EmployeeName").SortIndex(0).SortOrder(SortOrder.Asc).AllowEditing(False).Caption(OnBoardingController.GetServerLanguage().Translate("roOnBoardingUser", "OnBoarding"))
             columns.Add().DataField("Group").AllowEditing(False).Caption(OnBoardingController.GetServerLanguage().Translate("roOnBoardingGroup", "OnBoarding"))
             columns.Add().DataField("StartDate").Width(150).DataType(GridColumnDataType.Date).Format("dd/MM/yyyy").Alignment(HorizontalAlignment.Center).Caption(OnBoardingController.GetServerLanguage().Translate("roOnBoardingStartDate", "OnBoarding"))
             columns.Add().DataField("Status").AllowEditing(False).Caption(OnBoardingController.GetServerLanguage().Translate("roOnBoardingTasks", "OnBoarding")).CellTemplate(New JS("progressBar"))
             columns.Add().DataField("Comments").Caption(OnBoardingController.GetServerLanguage().Translate("roOnBoardingComments", "OnBoarding"))
         End Sub) _
.OnToolbarPreparing("toolbar_preparing") _
                                                                                )

                            @Code
                                Html.DevExtreme().Popup() _
                        .ID("tasksPopup") _
                        .Width(600) _
                        .Height(510) _
                        .ShowTitle(False) _
                        .DragEnabled(False) _
                        .OnShowing("RefreshTaskList") _
                        .OnHiding("RefreshOnBoardingList") _
                        .HideOnOutsideClick(True) _
                        .ContentTemplate(Sub()
                                @<text>
                                    @Html.Partial("EditTasks")
                                </text>
                     End Sub) _
.Render()
                            End Code

                            @Code
                                Html.DevExtreme().Popup() _
                    .ID("newListPopup") _
                    .Width(600) _
                    .Height(375) _
                    .ShowTitle(False) _
                    .Title("Tareas") _
                    .OnShown("RefreshAutoCompletes") _
                    .DragEnabled(False) _
                    .HideOnOutsideClick(True) _
                    .ContentTemplate(Sub()
                                @<text>
                                    @Html.Partial("NewList")
                                </text>
                 End Sub) _
.Render()
                            End Code
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<Script>
    var RootUrl = "@Html.Raw(ViewBag.RootUrl)";
var Permission = "@Html.Raw(ViewBag.PermissionOverEmployees)";
                        function toolbar_preparing(e) {

                        }

                        function addNewList() {
                            var popup = $("#newListPopup").dxPopup("instance");
                            popup.show();
                        }

                        function selection_changed(selectedItems) {
                            if (selectedItems.rowType!= "header") {
                                if (selectedItems.columnIndex == 4 && selectedItems.row.rowType == "data") {
                                    var popup = $("#tasksPopup").dxPopup("instance");
                                    $('#idList').val(selectedItems.data.IdList);
                                    popup.show();
                                }

                                if (selectedItems.columnIndex == 0) {
                                    var idEmployee = selectedItems.row.data.IdEmployee;
                                    parent.reenviaFrame('/' + "@Html.Raw(ViewBag.RootUrl)" + '/Employees/Employees.aspx' + "?IDEmployee=" + idEmployee, 'MainMenu_Global_MainMenu_Button1', 'Empleados', 'Portal\Users'); return false;
                                }
                            }

                        }

    function RefreshAutoCompletes() {
        $('#idCopySelected').val("");
                            if ($('#onboardingRemoved').val() == "true") {
                                $('#onboardingRemoved').val("false");
                                $.ajax({
                                    type: "POST",
                                    url: BASE_URL + 'OnBoarding/GetAvailableEmployees',
contentType: "application/json; charset=utf-8",
                                    success: function (data) {

                                        $("#EmployeeText").dxAutocomplete("instance").option("dataSource", data);
                                        var ds = $("#EmployeeText").dxAutocomplete("instance").getDataSource();
                                        ds.reload();

                                        $('#EmployeeText').dxAutocomplete('instance').reset();
                                        $('#EmployeeText').dxAutocomplete('instance').repaint();

                                    },
                                    error: function () { }
                                });

                                $.ajax({
                                    type: "POST",
                                    url: BASE_URL + 'OnBoarding/GetUsedEmployees',
contentType: "application/json; charset=utf-8",
                                    success: function (data) {
                                        $("#EmployeeTextCopy").dxAutocomplete("instance").option("dataSource", data);
                                        var ds = $("#EmployeeTextCopy").dxAutocomplete("instance").getDataSource();
                                        ds.reload();

                                        $('#EmployeeTextCopy').dxAutocomplete('instance').reset();
                                        $('#EmployeeTextCopy').dxAutocomplete('instance').repaint();

                                        //$("#EmployeeTextCopy").dxList("instance").option("dataSource", data);
                                        //var ds = $("#EmployeeTextCopy").dxList("instance").getDataSource();
                                        //ds.reload();
                                        //$('#EmployeeTextCopy').dxList('instance').reload();

                                    },
                                    error: function () { }
                                });
                            }
                        }
                        function OnBoardingRemoved() {
                            $('#onboardingRemoved').val("true");
                            $.ajax({
                                type: "GET",
                                url: BASE_URL + 'OnBoarding/LoadInitialData',
contentType: "application/json; charset=utf-8",
                                success: function (data) {
                                },
                                error: function () { }
                            });
                        }

                        function progressBar(container, options) {
                            if (options.displayValue!= null) {
                                var divisiones = options.displayValue.split("/");

                                $("<div style='cursor: pointer;' />").dxProgressBar({
                                    min: 0,
                                    max: parseInt(divisiones[1], 10),
                                    value: parseInt(divisiones[0], 10),
                                    width: 300,
                                    statusFormat: function (ratio, value) { return viewUtilsManager.DXTranslate('roStatus') + ' ' +  options.displayValue + ' (' + Math.round(ratio * 100) + '%' + ')' }
                                }).appendTo(container);
                            }
                        }

                        function cell_prepared(selectedItems) {

                        }

                        function context_menu(e) {
                            //if (e.columnIndex == 3 && e.row.rowType == "data") {
                            //    if (!e.items) e.items = [];

                            //    // Add a custom menu item
                            //    e.items.push({
                            //        text: "Copiar tareas de otro usuario",
                            //        onItemClick: function () {
                            //            var popup = $("#copyTasksPopup").dxPopup("instance");
                            //            popup.show();
                            //        }
                            //    });
                            //}

                        }
                        function RefreshTaskList() {
                            var tasklist = $("#listResumeTasks").dxDataGrid("instance");
                            tasklist.refresh();
                        }
                        function RefreshOnBoardingList() {
                            var onBoardingList = $("#gridStatusOnBoardings").dxDataGrid("instance");
                            onBoardingList.refresh();
                        }

    function AllowModify() {
        if ("@Html.Raw(ViewBag.PermissionOverEmployees)" > 3) {
            return true;
        }
        else {
            return false;
        }
    }
                        function hasData() {
                            var refreshPressed = localStorage.getItem("refreshPressed");
                            localStorage.removeItem("refreshPressed");
                            return refreshPressed;

                        }
                        function GetIdList() {
                            return $('#idList').val();

                        }
                        function addNewTask() {
                            if ($('#idList').val() != "") {
                                $.ajax({
                                    type: "POST",
                                    url: BASE_URL + 'OnBoarding/InsertTask',
contentType: "application/json; charset=utf-8",
                                    data: JSON.stringify({List:  $('#idList').val(), Task: $('#TaskText').dxTextBox('instance').option('value') }),
                                    success: function () {
                                        DevExpress.ui.notify("¡Tarea creada!", "success", 1000);
                                        $('#TaskText').dxTextBox('instance').option("value", '');
                                        var list = $("#listResumeTasks").dxDataGrid("instance");
                                        list.refresh(); },
                                    error: function () { DevExpress.ui.notify(viewUtilsManager.DXTranslate('roOnBoardingErrorTask'), "error", 1000); }
                                });
                            }
                            else {

                            }
                        }

    function addNewOnBoarding() {
        var selectedEmployee = $("#EmployeeText").dxAutocomplete("instance").option("selectedItem");
        var selectedEmployeeCopy = $("#EmployeeTextCopy").dxAutocomplete("instance").option("selectedItem") == null ? "" : $("#EmployeeTextCopy").dxAutocomplete("instance").option("selectedItem").IdEmployee;

        if (selectedEmployee != null) {
            var fecha = moment($("#dateSelector").dxDateBox("instance").option("value")).format("YYYY-MM-DD");

                                $.ajax({
                                    type: "POST",
                                    url: BASE_URL + 'OnBoarding/InsertOnBoarding',
contentType: "application/json; charset=utf-8",
                                    data: JSON.stringify({ Employee: selectedEmployee.IdEmployee, StartDate: fecha, CopyEmp: selectedEmployeeCopy}),
                                    success: function () {
                                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roOnBoardingCreated'), "success", 1000);
                                        var popup = $("#newListPopup").dxPopup("instance");
                                        popup.hide();
                                        $('#idEmployeeSelected').val("");
                                        $('#idCopySelected').val("");
                                        $('#dateSelected').val("");

                                        $.ajax({
                                            type: "POST",
                                            url: BASE_URL + 'OnBoarding/GetAvailableEmployees',
contentType: "application/json; charset=utf-8",
                                            success: function (data) {

                                                $("#EmployeeText").dxAutocomplete("instance").option("dataSource", data);
                                                var ds = $("#EmployeeText").dxAutocomplete("instance").getDataSource();
                                                ds.reload();

                                                $('#EmployeeText').dxAutocomplete('instance').reset();
                                                $('#EmployeeText').dxAutocomplete('instance').repaint();

                                            },
                                            error: function () {  }
                                        });

                                        $.ajax({
                                            type: "POST",
                                            url: BASE_URL + 'OnBoarding/GetUsedEmployees',
contentType: "application/json; charset=utf-8",
                                            success: function (data) {
                                                $("#EmployeeTextCopy").dxAutocomplete("instance").option("dataSource", data);
                                                var ds = $("#EmployeeTextCopy").dxAutocomplete("instance").getDataSource();
                                                ds.reload();

                                                $('#EmployeeTextCopy').dxAutocomplete('instance').reset();
                                                $('#EmployeeTextCopy').dxAutocomplete('instance').repaint();

                                                //$("#EmployeeTextCopy").dxList("instance").option("dataSource", data);
                                                //var ds = $("#EmployeeTextCopy").dxList("instance").getDataSource();
                                                //ds.reload();
                                                //$('#EmployeeTextCopy').dxList('instance').reload();

                                            },
                                            error: function () { }
                                        });

                                        var onBoardingList = $("#gridStatusOnBoardings").dxDataGrid("instance");
                                        onBoardingList.refresh();
                                    },
                                    error: function () { DevExpress.ui.notify(viewUtilsManager.DXTranslate('roOnBoardingErrorOnboarding'), "error", 1000); }
                                });
                            }
                            else {

                            }
                        }

                        function employeeSelected(e) {
                            if (e.selectedItem!= null) {
                                $('#idEmployeeSelected').val(e.selectedItem.IdEmployee);
                                $("#dateSelector").dxDateBox("instance").option("value", e.selectedItem.BeginContractDate);
                                $("#dateSelector").dxDateBox("instance").repaint();
                            }
                        }
                        function copySelected(e) {
                            if (e.selectedItem!= null) {
                                $('#idCopySelected').val(e.selectedItem.IdEmployee);
                            }
                                //$('#idCopySelected').val(e.component.option("selectedItemKeys")[0].IdEmployee);
                        }
                        function dateSelected(e) {

                                $('#dateSelected').val(e.value);

                        }
                        function SetGridHeight(e) {
                            e.rowElement.css({height:  70 });
                        }

                        function cell_prepared(selectedItems) {

                                if (typeof selectedItems.row!== 'undefined') {
                                    if (selectedItems.row.data.Done == true) {
                                        var arrayLength = selectedItems.row.cells.length;
                                        for (var i = 0; i < arrayLength; i++) {
                                            if (typeof selectedItems.row.cells[i].cellElement!== 'undefined') {
                                                selectedItems.row.cells[i].cellElement.addClass("taskDone");
                                            }
                                        }
                                    }
                                    else {
                                        var arrayLength = selectedItems.row.cells.length;
                                        for (var i = 0; i < arrayLength; i++) {
                                            if (typeof selectedItems.row.cells[i].cellElement!== 'undefined') {
                                                selectedItems.row.cells[i].cellElement.addClass("taskNotDone");
                                            }
                                        }
                                    }
                                }

                        }

                        //function selectedValues() {

                        //    const list = $("#listResumeTasks").dxList("instance");
                        //    list.option("selectedItemKeys", []);

                        //    let selectedKeys = list.option("selectedItemKeys");

                        //    list._dataSource._items.forEach(function (item) {
                        //        if (item.Done == true) {
                        //            selectedKeys.push(item.Id);
                        //        }
                        //    });

                        //    list.option("selectedItemKeys", selectedKeys);
                        //}
                            //function itemListLine(e) {

                        //    if (e.Done == false) {
                        //        return "<div  class='listTask' style='cursor: default;'><div id='listTaskItem' style='cursor: default;'>" + e.TaskName + "</div><div class='image' style='cursor: default;'></div></div>";
                        //    }
                        //    else {
                        //        return "<div  class='listTask' style='cursor: default;'><div id='listTaskItem' style='cursor: default;color:lightgrey;'>" + e.TaskName + "</div><div class='image' style='cursor: default;'> " + e.SupervisorName + " " + e.LastChangeDate + "</div></div>";
                        //    }

                        //}
                        function beforeSend(operation, ajaxSettings) {
                            var refreshPressed = localStorage.getItem("refreshPressed");
                            localStorage.removeItem("refreshPressed");
                            ajaxSettings.data.hasData2 = refreshPressed;
                        }
</Script>