document.onkeydown = function (event) {
    DoProcessEnterKeyEx(event);
}

function DoProcessEnterKey(htmlEvent) {
    if (htmlEvent.keyCode == 13) {
        ASPxClientUtils.PreventEventAndBubble(htmlEvent);
    }
}

function DoProcessEnterKeyEx(event) {
    if (event && event.keyCode == 13) {
        event.preventDefault();
        event.stopPropagation();
        return false;
    }
}

function DoProcessEnterKeyDetails(htmlEvent) {
    if (htmlEvent.keyCode == 13) {
        ASPxClientUtils.PreventEventAndBubble(htmlEvent);
    }
}

//=== COMBO VISTA ==================================================
//Modifica en el servidor datos relacionados con la vista seleccionada y recarga los datos en la pantalla en funcion de la Vista seleccionada
function cmbView_SelectedIndexChanged(s, e) {
    var miCallback = ASPxClientCallback.Cast("CallbackSessionClient");
    var cmb = ASPxClientComboBox.Cast(cmbViewClient);
    var cmbValue = cmb.GetSelectedItem().value;
    var jasonificado = jasonifica("VISTA", cmbValue, "UpdateVista('" + cmbValue + "');");

    if (!miCallback.InCallback()) miCallback.PerformCallback(jasonificado);
}

//Actuliza controles en la pantalla despues de modificar la vista
function UpdateVista(valueSelected) {
    reLoadView();
}
//=== FIN COMBO VISTA ===================================================

//=== SELECTOR ==========================================================
//Comprueba si puede mostrar el selector para cuando hay cambios pendientes en la pantalla
function CanOpenSelector(vis) {
    try {
        var bResult = IsGridEditingAndWarn();
        if (!bResult) {
            var miCallback = ASPxClientCallback.Cast("CallbackSessionClient");
            var jasonificado = jasonifica("ISCHANGEALLOWED", "", "OpenSelector(" + vis + ");", "TRUE");
            if (!miCallback.InCallback()) miCallback.PerformCallback(jasonificado);
        }
    }
    catch (e) {
        showError("CanOpenSelector", e);
    }
}

//Muestra u oculta grid del selector de empleado-fecha
function OpenSelector(vis) {
    var panSelector = document.getElementById("ASPxMovesNewPanel_divSelector");
    var bolVisible;
    if (vis == null) {
        if (panSelector.style.display == 'none') {
            panSelector.style.display = '';
            bolVisible = true;
        }
        else {
            panSelector.style.display = 'none';
            bolVisible = false;
        }
    }
    else {
        if (vis) {
            panSelector.style.display = '';
            bolVisible = true;
        }
        else {
            panSelector.style.display = 'none';
            bolVisible = false;
        }
    }
}
var initialSelectorValue = -1;
//Carga los datos en la pantalla al seleccionar un empleado del selector
function GridSelector_FocusedRowChanged(s, e) {
    try {
        var selRow = GridSelectorClient.GetFocusedRowIndex();
        if (selRow < 0) return;

        navigateToSelectorChoice(s, e);
        //if (initialSelectorValue != -1 && initialSelectorValue != selRow) navigateToSelectorChoice(s, e);
        //else initialSelectorValue = selRow;
    }
    catch (e) {
    }
}
//=== FIN SELECTOR =============================================================

//=== SHIFTS ===================================================================
//Comprueba si puede mostrar la pàgina de selección de horario
function CanShowShiftSelector() {
    try {
        var miCallback = ASPxClientCallback.Cast("CallbackSessionClient");
        var jasonificado = jasonifica("ISCHANGEALLOWED", "", "ShowShiftSelector();", "TRUE");
        if (!miCallback.InCallback()) miCallback.PerformCallback(jasonificado);
    }
    catch (e) {
        showError("CanShowShiftSelector", e);
    }
}

// Muestra la pàgina de selección de horarios
function ShowShiftSelector() {
    try {
        var Title = '';
        var IDEmployeePage = document.getElementById("ASPxMovesNewPanel_hdnIDEmployeePage").value;
        var IDShiftPage = document.getElementById("ASPxMovesNewPanel_CallbackPanelShift_hdnIDShiftPage").value;
        ShowExternalForm2('ShiftSelector.aspx?IDEmployee=' + IDEmployeePage + '&IDShift=' + IDShiftPage, 475, 415, Title, '', false, false, false);
    }
    catch (e) {
        showError("ShowShiftSelector", e);
    }
}

//lanza callback al servidor para grabar el shift que se ha cambiado
function ShiftChange(IDShiftSelected, StartShiftSelected, IDAssignmentSelected) {
    try {
        document.getElementById("ASPxMovesNewPanel_CallbackPanelShift_hdnIDShiftChange").value = IDShiftSelected;
        document.getElementById("ASPxMovesNewPanel_CallbackPanelShift_hdnStartShiftChange").value = StartShiftSelected;
        document.getElementById("ASPxMovesNewPanel_CallbackPanelShift_hdnIDAssignmentChange").value = IDAssignmentSelected;

        var jasonificado = jasonifica("SHIFTCHANGE");
        CallbackPanelShiftClient.PerformCallback(jasonificado);
    }
    catch (e) {
        showError("ShiftChange", e);
    }
}

function RemoveHolidays() {
    try {
        var jasonificado = jasonifica("SHIFTREMOVEHOLIDAYS");
        CallbackPanelShiftClient.PerformCallback(jasonificado);
    }
    catch (e) {
        showError("RemoveHolidays", e);
    }
}

function CallbackPanelShiftClient_EndCallback(s, e) {
    if (s.cpReturnValue != "") {
        var hdnMustRefresh_PageBase = document.getElementById("ASPxMovesNewPanel_hdnMustRefresh_PageBase");
        document.getElementById("ASPxMovesNewPanel_hdnParams_PageBase").value = hasMovesNewChanges;
        hdnMustRefresh_PageBase.value = "movesNew";
        setTimeout(function () { RefreshPageComplete(true); }, 1000);
    }
}
//=== FIN SHIFTS ==================================================================

//=== AUSENCIAS PROGRAMADAS =======================================================
//Comprueba si puede abrir la pantalla modal de ausencias para crear una nueva
function CanAddNewAbsence() {
    try {
        var miCallback = ASPxClientCallback.Cast("CallbackSessionClient");
        var jasonificado = jasonifica("ISCHANGEALLOWED", "", "AddNewAbsence();", "TRUE");
        if (!miCallback.InCallback()) miCallback.PerformCallback(jasonificado);
    }
    catch (e) {
        showError("CanShowShiftSelector", e);
    }
}

var absencesPopover = null;
var absencesList = null;
var absencesDS = null;

function ShowCurrentForecasts() {
    try {
        absencesList = $('#listAbsencesPopover').dxPopover({
            target: '#ASPxMovesNewPanel_CallbackPanelAbsence_lblAbsenceDetailsInfo',
            showEvent: "dxclick",
            position: "bottom",
            width: 650,
            shading: false,
            onShown: function () {
                absencesDS = JSON.parse(hdnAbsencesInfoClient.Get("cpAbsencesData"))

                $("#divForecastsGrid").dxDataGrid({
                    showColumnLines: true,
                    showRowLines: true,
                    rowAlternationEnabled: true,
                    showBorders: true,
                    dataSource: {
                        store: Object.clone(absencesDS, true)
                    },
                    editing: {
                        mode: "row",
                        allowUpdating: true,
                        allowDeleting: true,
                        texts: { deleteRow: 'Delete', editRow: 'Edit', confirmDeleteMessage: '' }
                    },
                    onCellPrepared: function (e) {
                        if (e.rowType === "data" && e.column.command === "edit") {
                            var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                            $links.text("");

                            if (isEditing) {
                                $links.filter(".dx-link-save").addClass("dx-icon-save");
                                $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                            } else {
                                $links.filter(".dx-link-edit").addClass("dx-icon-edit");
                                $links.filter(".dx-link-delete").addClass("dx-icon-trash");
                            }
                        }
                    },
                    onRowRemoved: function (e) {
                        absencesList.hide();
                        removeSelectedForecast(e.data);
                    },
                    onEditingStart: function (e) {
                        absencesList.hide();
                        editSelectedForecast(e.data);
                    },
                    remoteOperations: {
                        sorting: true,
                        paging: true
                    },
                    paging: {
                        pageSize: 12
                    },
                    pager: {
                        showPageSizeSelector: false
                    },
                    columns: [
                        { caption: "Descripción", dataField: "UserDescription", allowEditing: false }
                    ]
                }).dxDataGrid("instance");
            }
        }).dxPopover("instance");

        absencesList.show();
    }
    catch (e) {
        showError("ShowCurrentForecasts", e);
    }
}

var selectedForecast = null;

function editSelectedForecast(cForecast) {
    selectedForecast = cForecast;
    CanEditAbsence();
}

function editSelectedForecastFinally() {
    var url = '';
    switch (selectedForecast.AbsenceType) {
        case 'ProgrammedAbsence':
            url = '../Employees/EditProgrammedAbsence.aspx?EmployeeID=' + selectedForecast.IDEmployee + '&BeginDate=' + encodeURIComponent(moment(selectedForecast.BeginDate).format("YYYY-MM-DD"));
            break;
        case 'ProgrammedCause':
            url = '../Employees/EditProgrammedIncidence.aspx?EmployeeID=' + selectedForecast.IDEmployee + '&BeginDate=' + encodeURIComponent(moment(selectedForecast.BeginDate).format("YYYY-MM-DD")) + '&AbsenceID=' + selectedForecast.AbsenceID;
            break;
        case 'ProgrammedOverTime':
            url = '../Employees/EditProgrammedOvertimes.aspx?ProgrammedHolidaysID=' + selectedForecast.AbsenceID + '&EmployeeID=' + selectedForecast.IDEmployee;
            break;
        case 'ProgrammedHoliday':
            url = '../Employees/EditProgrammedHolidays.aspx?ProgrammedHolidaysID=' + selectedForecast.AbsenceID + '&EmployeeID=' + selectedForecast.IDEmployee;
            break;
    }

    ShowExternalForm2(url, 830, 485, '', '', false, false, false);
}

function removeSelectedForecast(cForecast) {
    selectedForecast = cForecast;
    CanConfirmDeleteAbsence();
}

function removeSelectedForecastFinally() {
    var url = '';
    switch (selectedForecast.AbsenceType) {
        case 'ProgrammedAbsence':
            url = "../Employees/Handlers/srvEmployees.ashx?action=deleteProgrammedAbsence&IDCause=0&ID=" + selectedForecast.IDEmployee + "&BeginDate=" + encodeURIComponent(moment(selectedForecast.BeginDate).format("YYYY-MM-DD"));
            break;
        case 'ProgrammedCause':
            url = "../Employees/Handlers/srvEmployees.ashx?action=deleteProgInc&IDCause=0&ID=" + selectedForecast.IDEmployee + "&BeginDate=" + encodeURIComponent(moment(selectedForecast.BeginDate).format("YYYY-MM-DD")) + "&IDAbsence=" + selectedForecast.AbsenceID;
            break;
        case 'ProgrammedOverTime':
            url = '../Employees/Handlers/srvEmployees.ashx?action=deleteProgOvertime&IDProgOvertime=' + selectedForecast.AbsenceID;
            break;
        case 'ProgrammedHoliday':
            url = '../Employees/Handlers/srvEmployees.ashx?action=deleteProgHolidays&IDProgHoliday=' + selectedForecast.AbsenceID;
            break;
    }

    AsyncCall("POST", url, "json", "arrStatus", "DeleteAbsenceAfter(arrStatus);");
}

//Muestra la pantalla modal para crear una ausencia nueva
function AddNewAbsence() {
    try {
        var oAvailableAbsences = [
            { Icon: 'ProgrammedAbsence', Text: Globalize.formatMessage('roProgrammedAbsence'), Action: 0 },
            { Icon: 'ProgrammedCause', Text: Globalize.formatMessage('roProgrammedCause'), Action: 1 },
            { Icon: 'ProgrammedOvertimes', Text: Globalize.formatMessage('roProgrammedOvertimes'), Action: 2 },
            { Icon: 'ProgrammedHolidays', Text: Globalize.formatMessage('roProgrammedHolidays'), Action: 3 }
        ];

        absencesPopover = $('#addAbsencePopover').dxPopover({
            target: '#ASPxMovesNewPanel_CallbackPanelAbsence_btnAddNewAbsence',
            showEvent: "dxclick",
            position: "bottom",
            width: 300,
            shading: false,
            onShown: function () {
                $('#absencesAvailableList').dxList({
                    dataSource: oAvailableAbsences,
                    itemTemplate: function (data, index) {
                        var result = $("<div>").addClass("assistantLine");

                        $("<div>").attr("class", data.Icon).attr('style', 'float:left;width:32px').appendTo(result);

                        var content = $("<div>").attr('style', 'float:left;width:calc(100% - 64px);border-bottom: 2px inset;padding-bottom: 5px;');
                        content.append($("<div>").attr("class", 'popoverText').text(data.Text));
                        content.append($("<div>").attr("class", 'popoverDesc').append($("<span>").text('')));
                        content.appendTo(result);

                        $("<div>").attr("class", 'flaticon-right2').appendTo(result);

                        return result;
                    },
                    onItemClick: function (data, index) {
                        absencesPopover.hide();
                        var IDEmployeePage = document.getElementById("ASPxMovesNewPanel_hdnIDEmployeePage").value;
                        var DatePage = document.getElementById("ASPxMovesNewPanel_hdnDatePage").value;
                        var url = '';
                        switch (data.itemData.Action) {
                            case 0:
                                url = '../Employees/EditProgrammedAbsence.aspx?EmployeeID=' + IDEmployeePage + '&NewRecord=1';
                                break;
                            case 1:
                                url = '../Employees/EditProgrammedIncidence.aspx?EmployeeID=' + IDEmployeePage + '&NewRecord=1';
                                break;
                            case 2:
                                url = '../Employees/EditProgrammedOvertimes.aspx?EmployeeID=' + IDEmployeePage + '&ProgrammedHolidaysID=-1';
                                break;
                            case 3:
                                url = '../Employees/EditProgrammedHolidays.aspx?EmployeeID=' + IDEmployeePage + '&ProgrammedHolidaysID=-1';
                                break;
                        }

                        var Title = '';
                        ShowExternalForm2(url, 830, 485, Title, '', false, false, false);
                    }
                }).dxList("instance");
            }
        }).dxPopover("instance");

        absencesPopover.show();
    }
    catch (e) {
        showError("AddNewAbsence", e);
    }
}

//Comprueba si puede editar la pantalla modal de ausencias para crear una nueva
function CanEditAbsence() {
    try {
        var miCallback = ASPxClientCallback.Cast("CallbackSessionClient");
        var jasonificado = jasonifica("ISCHANGEALLOWED", "", "editSelectedForecastFinally();", "TRUE");
        if (!miCallback.InCallback()) miCallback.PerformCallback(jasonificado);
    }
    catch (e) {
        showError("CanShowShiftSelector", e);
    }
}

//Muestra la pantalla modal para editar una ausencia existente
//function EditAbsence() {
//    try {
//        var IDEmployeePage = document.getElementById("hdnIDEmployeePage").value;
//        var BeginDateCause = document.getElementById("CallbackPanelAbsence_lblAbsenceDetailsInfo").getAttribute("begindatecause");
//        var url = '../Employees/EditProgrammedAbsence.aspx?EmployeeID=' + IDEmployeePage + '&BeginDate=' + encodeURIComponent(BeginDateCause);
//        var Title = '';
//        ShowExternalForm2(url, 830, 440, Title, '', false, false, false);
//    }
//    catch (e) {
//        showError("EditAbsence", e);
//    }
//}

//Comprueba si puede eliminar una ausencia
function CanConfirmDeleteAbsence() {
    try {
        var miCallback = ASPxClientCallback.Cast("CallbackSessionClient");
        var jasonificado = jasonifica("ISCHANGEALLOWED", "", "ConfirmDeleteAbsence();", "TRUE");
        if (!miCallback.InCallback()) miCallback.PerformCallback(jasonificado);
    }
    catch (e) {
        showError("CanShowShiftSelector", e);
    }
}

//Muestra pantalla modal de confirmacion para eliminar una ausencia
function ConfirmDeleteAbsence() {
    var url = "Scheduler/srvMsgBoxScheduler.aspx?action=DeleteProgrammedAbsence&Navigator=" + BrowserDetect.browser;
    parent.ShowMsgBoxForm(url, 400, 300, '');
}

//Lanza llamada ajax al servidor para eliminar una ausencia
function DeleteAbsenceFromMoves(IDEmployee, BeginDateCause, IDCause) {
    try {
        //var url = "../Employees/Handlers/srvEmployees.ashx?action=deleteProgrammedAbsence&IDCause=" + IDCause + "&ID=" + IDEmployee + "&BeginDate=" + encodeURIComponent(BeginDateCause);
        removeSelectedForecastFinally();
    }
    catch (e) {
        showError("DeleteAbsence", e);
    }
}

//Retorno de la eliminacion de una ausencia
function DeleteAbsenceAfter(arrStatus) {
    var hdnMustRefresh_PageBase = document.getElementById("ASPxMovesNewPanel_hdnMustRefresh_PageBase");
    document.getElementById("ASPxMovesNewPanel_hdnParams_PageBase").value = hasMovesNewChanges;
    hdnMustRefresh_PageBase.value = "movesNew";
    setTimeout(function () { RefreshPageComplete(true); }, 1000);
}
//=== FIN AUSENCIAS PROGRAMADAS =======================================================