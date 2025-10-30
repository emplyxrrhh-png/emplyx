var selectedTC = [];
var selectedTCItems = [];

var selectedAbsence = [];
var selectedAbsenceItems = [];

var selectedStatus = [];
var selectedStatusItems = [];

function showPopover() {
    setTimeout(function () { $("#news1").dxPopover("show"); }, 500)
    setTimeout(function () { $("#news2").dxPopover("show"); }, 1000)
    setTimeout(function () { $("#news3").dxPopover("show"); }, 1500)
}

function hasData() {
    var refreshPressed = localStorage.getItem("refreshPressed");
    localStorage.removeItem("refreshPressed");
    return refreshPressed;

}

function closePopupWarning() {
    var popup = $("#warningGroups").dxPopup("instance");
    popup.hide();
}
function selectedAllGroups(e) {

    if (e.value == true) {
        var popup = $("#warningGroups").dxPopup("instance");
        popup.show();
    }
    
}

function refreshGrid(e) {

    var data = document.getElementById('employeesResumeWithData');
    var noData = document.getElementById('employeesResumeWithoutData');
    var groupSelected = localStorage.getItem("groupSelected");
    var arrayGroups = [];


    if (groupSelected != 'ninguno' && groupSelected != null && typeof groupSelected != 'undefined') {
        arrayGroups = JSON.parse(groupSelected);
    }

    if ($("#GroupsList").dxTagBox('instance').option("selectedItems").length == 0 && (arrayGroups.length == e.removedItems.length)) {
        data.style.display = 'none';
        noData.style.display = '';
        localStorage.setItem("groupSelected", "ninguno");
    }
    else {
        if (e.addedItems.length > 0 || e.removedItems.length > 0 || (groupSelected != null && typeof groupSelected != 'undefined' && groupSelected != 'null' && groupSelected != 'ninguno' && groupSelected != "" && groupSelected != "[]")) {
            data.style.display = '';
            noData.style.display = 'none';

            var values = null;
            if (e.addedItems.length == 0 && e.removedItems.length == 0) {
                values = JSON.parse(groupSelected);
            }
            else {
                values = jQuery.map($("#GroupsList").dxTagBox('instance').option("selectedItems"), function (n) { return n.IdGroup; });
            }


            localStorage.setItem("groupSelected", JSON.stringify(values));


            try {
                $("#gridStatusEmployees").dxDataGrid("instance").refresh();
            }
            catch (err) {

            }


        }
        else {
            data.style.display = 'none';
            noData.style.display = '';
            localStorage.setItem("groupSelected", "ninguno");
        }
    }

}

function restoreFiltersTelecommute(e) {
    selectedTC = [];
    $("#listResumeEmployeesTC").dxList("instance").unselectItem(e.itemIndex);
}

function filterGridTelecommute(e) {
    selectedStatus = [];
    selectedAbsence = [];
    if (e.addedItems.length > 0) {

        $("#listResumeEmployees").dxList("instance").option("selectedItemKeys", selectedStatus);
        $("#listResumeAbsence").dxList("instance").option("selectedItemKeys", selectedAbsence);

        if (selectedTCItems.length > 0 && e.addedItems[0].ImageSrc == selectedTCItems[0].ImageSrc) { }
        else {
            if (e.addedItems[0].ImageSrc == "Base/Images/PortalRequests/icons8-office-48.png") {
                $("#gridStatusEmployees").dxDataGrid("instance").option("filterValue", ["InRealTimeTC", "=", "0"]);
            }
            else {
                $("#gridStatusEmployees").dxDataGrid("instance").option("filterValue", ["InRealTimeTC", "=", "1"]);
            }
        }
    }
    else {

        $("#listResumeEmployeesTC").dxList("instance").option("selectedItemKeys", selectedTC);
        if (selectedTC.length == 0) {
            $("#gridStatusEmployees").dxDataGrid("instance").option("filterValue", []);
        }
    }
}

function restoreFiltersAbsences(e) {
    selectedAbsence = [];
    $("#listResumeAbsence").dxList("instance").unselectItem(e.itemIndex);
}

function filterGridAbsences(e) {
    selectedStatus = [];
    selectedTC = [];
    if (e.addedItems.length > 0) {

        if (typeof $("#listResumeEmployeesTC").dxList("instance") != 'undefined') {
            $("#listResumeEmployeesTC").dxList("instance").option("selectedItemKeys", selectedTC);
        }
        $("#listResumeEmployees").dxList("instance").option("selectedItemKeys", selectedStatus);

        if (selectedAbsenceItems.length > 0 && e.addedItems[0].ImageSrc == selectedAbsenceItems[0].ImageSrc) { }
        else {
            if (e.addedItems[0].ImageSrc == "Base/Images/PortalRequests/ico_Holidays.png") {
                $("#gridStatusEmployees").dxDataGrid("instance").option("filterValue", ["InAnyHoliday", "=", "1"]);
            }
            else {
                $("#gridStatusEmployees").dxDataGrid("instance").option("filterValue", ["InAnyAbsence", "=", "1"]);
            }
        }
    }
    else {

        $("#listResumeAbsence").dxList("instance").option("selectedItemKeys", selectedAbsence);
        if (selectedAbsence.length == 0) {
            $("#gridStatusEmployees").dxDataGrid("instance").option("filterValue", []);
        }
    }
}

function restoreFiltersStatus(e) {
    selectedStatus = [];
    $("#listResumeEmployees").dxList("instance").unselectItem(e.itemIndex);
}

function filterGridStatus(e) {
    selectedTC = [];
    selectedAbsence = [];
    if (e.addedItems.length > 0) {

        if (typeof $("#listResumeEmployeesTC").dxList("instance") != 'undefined') {
            $("#listResumeEmployeesTC").dxList("instance").option("selectedItemKeys", selectedTC);
        }
        $("#listResumeAbsence").dxList("instance").option("selectedItemKeys", selectedAbsence);

        if (selectedStatusItems.length > 0 && e.addedItems[0].ImageSrc == selectedStatusItems[0].ImageSrc) { }
        else {
            if (e.addedItems[0].ImageSrc == "Base/Images/PortalRequests/WX_circle_green.png") {
                $("#gridStatusEmployees").dxDataGrid("instance").option("filterValue", ["PresenceStatus", "=", "In"]);
            }
            else {
                $("#gridStatusEmployees").dxDataGrid("instance").option("filterValue", ["PresenceStatus", "<>", "In"]);
            }
        }
    }
    else {

        $("#listResumeEmployees").dxList("instance").option("selectedItemKeys", selectedStatus);
        if (selectedStatus.length == 0) {
            $("#gridStatusEmployees").dxDataGrid("instance").option("filterValue", []);
        }
    }
}

var firstRender = true;

function onContentReady(e) {
    var data = document.getElementById('employeesResumeWithData');
    var noData = document.getElementById('employeesResumeWithoutData');
    var groupSelected = localStorage.getItem("groupSelected");

    if (firstRender) {
        e.component.getDataSource().load().done(function (res) {

            if (groupSelected != null && groupSelected != 'undefined' && groupSelected != 'null' && groupSelected != 'ninguno' && groupSelected != "") {
                data.style.display = '';
                noData.style.display = 'none';

                groupSelected = JSON.parse(groupSelected);

                $("#GroupsList").dxTagBox('instance').option("value", jQuery.map(groupSelected, function (n) { return {IdGroup: n}}) )

            }
            else if (groupSelected == null || groupSelected == "") {
                data.style.display = '';
                noData.style.display = 'none';
                e.component.option("value", res[0]);
                return;
            }
            else {
                data.style.display = 'none';
                noData.style.display = '';
            }

        });
    }
    firstRender = false;

}

function beforeSend(operation, ajaxSettings) {
    var refreshPressed = localStorage.getItem("refreshPressed");
    localStorage.removeItem("refreshPressed");
    if ($("#GroupsList").dxTagBox('instance').option('value').length > 0) {
        ajaxSettings.method = "POST";
        ajaxSettings.data.idGroup = localStorage.getItem("groupSelected");
    }
}

function hideThePopover() {
    if ($("#newsOk")) { $("#newsOk").dxPopover("hide"); } 
    if ($("#news1")) { $("#news1").dxPopover("hide"); } 
    if ($("#news2")) { $("#news2").dxPopover("hide"); } 
    if ($("#news3")) { $("#news3").dxPopover("hide"); } 

    $.ajax({
        url: 'Start/UpdateHelpVersion',
        type: "POST",
        success: function () {

        }
    });
}

function exportDashboard(e) {
    var workbook = new ExcelJS.Workbook();
    var worksheet = workbook.addWorksheet('Main sheet');
    DevExpress.excelExporter.exportDataGrid({
        worksheet: worksheet,
        component: e.component,
        customizeCell: function (options) {
            options.excelCell.font = { name: 'Arial', size: 12 };
            options.excelCell.alignment = { horizontal: 'left' };
        }
    }).then(function () {
        workbook.xlsx.writeBuffer().then(function (buffer) {
            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'estado.xlsx');
        });
    });
}
function selection_changed(selectedItems) {
    if (selectedItems.columnIndex == 0) {
        let idEmployee = selectedItems.row.data.IdEmployee;

        $.ajax({
            url: `/Employee/GetEmployeeTreeSelectionPath/${idEmployee}`,
            data: {},
            type: "GET",
            dataType: "json",
            success: async (data) => {
                if (typeof data != 'string') {

                    let treeState = await getroTreeState("ctl00_contentMainBody_roTrees1", true);
                    await treeState.setRedirectData(data.EmployeePath, data.GroupSelectionPath);

                    window.open(RootUrl + '/Employees/Employees?IDEmployee=' + idEmployee, "_blank"); return false;

                } else {
                    DevExpress.ui.notify(data, 'error', 2000);
                }
            },
            error: (error) => console.error(error),
        });


        
    }

}

function cell_prepared(selectedItems) {
    if (selectedItems.columnIndex == 0) {
        if (typeof selectedItems.row !== 'undefined') {
            if (selectedItems.row.data.PresenceStatus == "In") {
                selectedItems.cellElement.addClass("employeeListPhoto");
            }
            else {
                if (selectedItems.row.data.LastPunchFormattedDateTime != "") {
                    selectedItems.cellElement.addClass("employeeListPhotoOut");
                }
                else {
                    selectedItems.cellElement.addClass("employeeListNA");

                    var arrayLength = selectedItems.row.cells.length;
                    for (var i = 0; i < arrayLength; i++) {
                        if (typeof selectedItems.row.cells[i].cellElement !== 'undefined') {
                            selectedItems.row.cells[i].cellElement.addClass("naUser");
                        }
                    }

                }
                // selectedItems.cellElement.append('<div class="employeeListAbsence"></div>');
            }
        }
    }

}

function reenviaRequests() {
    window.open(RootUrl + '/Requests/Requests', "_blank"); return false;
}

function navigateCompany() {
    window.open(RootUrl + '/Employees/Company', "_blank"); return false;
}

function navigateEmployees() {
    //parent.reenviaFrame('/' + RootUrl + '/Employees/Employees', 'MainMenu_Global_MainMenu_Button1', 'Usuarios', 'Portal\Users'); return false;

    window.open(RootUrl + '/Employees/Employees', "_blank"); return false;
}

function navigateAnalytic() {
    if (Genius.toUpperCase() == "TRUE") {
        window.open(RootUrl + '//Genius', "_blank"); return false;
    }
    else {
        window.open(RootUrl + '/Scheduler/AnalyticsScheduler', "_blank"); return false;
    }
}

function navigateCalendar() {

        window.open(RootUrl + '/Scheduler/Calendar', "_blank"); return false;
  
}

function reenviaAlerts() {
    window.open(RootUrl + '/Alerts/Alerts', "_blank"); return false;
    // window.location.reload();
}

function refreshPartial(e) {

    if (typeof $("#listResumeEmployeesTC").dxList("instance") != 'undefined') {
        selectedTC = $("#listResumeEmployeesTC").dxList("instance").option("selectedItemKeys");
        selectedTCItems = $("#listResumeEmployeesTC").dxList("instance").option("selectedItems");
        $("#listResumeEmployeesTC").dxList("instance").reload();
    }

    if (typeof $("#listResumeAbsence").dxList("instance") != 'undefined') {
        selectedAbsence = $("#listResumeAbsence").dxList("instance").option("selectedItemKeys");
        selectedAbsenceItems = $("#listResumeAbsence").dxList("instance").option("selectedItems");
        $("#listResumeAbsence").dxList("instance").reload();
    }

    if (typeof $("#listResumeEmployees").dxList("instance") != 'undefined') {
        selectedStatus = $("#listResumeEmployees").dxList("instance").option("selectedItemKeys");
        selectedStatusItems = $("#listResumeEmployees").dxList("instance").option("selectedItems");
        $("#listResumeEmployees").dxList("instance").reload();
    }

}



function toolbar_preparing(e) {
    var dataGrid = e.component;

    e.toolbarOptions.items.unshift({
        location: "after",
        widget: "dxButton",
        options: {
            icon: "refresh",
            onClick: function () {
                localStorage.setItem("refreshPressed", true)
                dataGrid.refresh();
            }
        }
    },
        {
            location: "after",
            widget: "dxButton",
            options: {
                icon: "fullscreen",
                onClick: function () {
                    hideTree();
                }
            }
        });
}

function hideTree() {
    let treeIsVisible = !$("#section2").is(":visible");
    if (treeIsVisible) {
        document.getElementById("section1").style.width = "100%";
    }
    else {
        document.getElementById("section1").style.width = "100%";
    }

    $("#section2").animate({ width: 'toggle', duration: 100 });
}

var headerFilter = {
    load: function (loadOptions) {

        return [{
            text: viewUtilsManager.DXTranslate('roPresent'),
            value: [['PresenceStatus', '=', 'In']]
        }, {
            text: viewUtilsManager.DXTranslate('roAbsent'),
            value: [['PresenceStatus', '<>', 'In']]
        }];
    }
}

var headerFilterTelecommute = {
    load: function (loadOptions) {

        return [{
            text: viewUtilsManager.DXTranslate('roHome'),
            value: [['InTelecommute', '=', true]]
        }, {
            text: viewUtilsManager.DXTranslate('roOffice'),
            value: [['InTelecommute', '=', false]]
        }];
    }
}

var headerFilterAbsence = {
    load: function (loadOptions) {

        return [{
            text: viewUtilsManager.DXTranslate('roDaysAbsence'),
            value: [['InAbsence', '=', '1']]
        }, {
            text: viewUtilsManager.DXTranslate('roHolidaysAbsence'),
            value: [['InHolidays', '=', '1']]
        },
        {
            text: viewUtilsManager.DXTranslate('roHoursAbsence'),
            value: [['InHoursAbsence', '=', '1']]
        },
        {
            text: viewUtilsManager.DXTranslate('roDaysRequest'),
            value: [['DaysAbsenceRequested', '=', '1']]
        },
        {
            text: viewUtilsManager.DXTranslate('roHoursRequest'),
            value: [['HoursAbsenceRequested', '=', '1']]
        }];
    }
}
function showUpdateNotification() {

}

function acceptUpdateNotification() {
    if ($("#updateNotificationPopup")) { $("#updateNotificationPopup").dxPopup("hide"); }

    if ($("#dontShowUpdateAgainCheckbox").prop("checked")) {
        $.ajax({
            url: 'Start/UpdateVersionNotification',
            type: "POST",
            success: function () {

            }
        });
    }
}

//var parms = "";
//parms = { "action": "LoadXmlFeed" };
//AjaxCall("POST", "json", "Base/Handlers/srvMain.ashx", parms, "CONTAINER", "newsXmlFeed", "", 0);

