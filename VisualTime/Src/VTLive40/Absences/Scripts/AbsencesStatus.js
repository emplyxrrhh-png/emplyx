var bClearFilter = true;

//============ POPUP SAVE VIEWS ====================================
function formatDateYMD(initialDate) {
    if (initialDate == "" || initialDate == null) {
        return "";
    }
    else {
        var dia = initialDate.getDate();
        if (dia.toString().length == 1) dia = "0" + dia.toString();
        var mes = initialDate.getMonth() + 1; //Months are zero based
        if (mes.toString().length == 1) mes = "0" + mes.toString();
        var ejer = initialDate.getFullYear();
        var finalDate = ejer + "#" + mes + "#" + dia;
        return finalDate;
    }
}

//==================================================================

//============ SELECTOR DE EMPLEADOS ===============================
function btnOpenPopupSelectorEmployeesClient_Click() {
    PopupSelectorEmployeesClient.Show();
}

function btnPopupSelectorEmployeesAcceptClient_Click() {
    CallbackSessionClient.PerformCallback("GETINFOSELECTED")
    PopupSelectorEmployeesClient.Hide();
}

function btCloseRequestClient_Click() {
    PopupEditRequestClient.Hide();
    ThrowRefreshClient(false);
}
//==================================================================

//============ SELECTOR DE INCIDENCIAS =============================
function btnOpenPopupSelectorCausesClient_Click() {
    PopupSelectorCausesClient.Show();
}

function btnPopupSelectorCausesAcceptClient_Click() {
    let arrCausesValues = ListCauses_Client.GetSelectedValues();

    let hdnCausesSelected = document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnCausesSelected');

    if (arrCausesValues.length > 0) {
        hdnCausesSelected.value = arrCausesValues;
        if (arrCausesValues.length == 1) {
            CallbackSessionClient.PerformCallback("UPDATECAUSETEXT")
        } else {
            txtCauses_Client.SetText(arrCausesValues.length + " " + document.getElementById('ctl00_contentMainBody_hdnCauseSelectedText').value);
        }
    }
    else {
        hdnCausesSelected.value = "";
        txtCauses_Client.SetText(document.getElementById('ctl00_contentMainBody_hdnAllCauses').value);
    }

    PopupSelectorCausesClient.Hide();
}
//==================================================================

//============ RETORNO DE LLAMADAS DE CALLBACKS SIMPLES ============
async function CallbackSession_CallbackComplete(s, e) {
    if (e.parameter == "GETINFOSELECTED") {
        let sInfo = e.result.split(';');
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnEmployeesSelected').value = sInfo[0];
        if (sInfo.length > 1 && sInfo[1] != " ") txtEmployees_Client.SetText(sInfo[1]);
        else txtEmployees_Client.SetText(document.getElementById('ctl00_contentMainBody_hdnAllEmployees').value);

    } else if (e.parameter == "CLIENTSHOWDETAILS") {
        let typePos = e.result.indexOf(";");
        let type = e.result.substring(0, typePos);
        let url = e.result.substring(typePos + 1);
        ShowDetailGrid(url, type);
    } else if (e.parameter == "UPDATECAUSETEXT") {
        txtCauses_Client.SetText(e.result);
    } else if (e.parameter == "CLIENTEMPLOYEEDETAILS") {
        let typePos = e.result.indexOf("@");
        let idEmployee = e.result.substring(0, typePos);
        let rootUrl = e.result.substring(typePos + 1);

        let url = "../#/" + rootUrl + "/Employees/Employees"

        $.ajax({
            url: `/Employee/GetEmployeeTreeSelectionPath/${idEmployee}`,
            data: {},
            type: "GET",
            dataType: "json",
            success: async (data) => {
                if (typeof data != 'string') {

                    let treeState = await getroTreeState("ctl00_contentMainBody_roTrees1", true);
                    await treeState.setRedirectData(data.EmployeePath, data.GroupSelectionPath);

                    parent.window.open(url, "_blank");

                } else {
                    DevExpress.ui.notify(data, 'error', 2000);
                }
            },
            error: (error) => console.error(error),
        });
    }
}
//==================================================================

//============ OBTENER DATOS =================================
function btnRefreshClient_Click() {
    ThrowRefreshClient(true);
}

function PopupWarningYes() {
    PopupWarningClient.Hide();
    ThrowRefreshClient(true);
}

function ThrowRefreshClient(clearFilter) {
    if (typeof (clearFilter) != 'undefined') {
        bClearFilter = clearFilter;
    }
    GridAbsencesStatusClient.PerformCallback("REFRESHGRID");
    parent.showLoader(false);
}

function GridAbsencesClient_EndCallback(s, e) {
}

//============ Eventos combo cliente =================================
function cmbCausesFilter_ValueChanged() {
}

function cmbStatusClient_ValueChanged() {
    if (cmbStatusClient.GetValue() == "2" || cmbStatusClient.GetValue() == "3") {
        $('#dateFilters').css('display', '');
    } else {
        $('#dateFilters').css('display', 'none');
    }
}

function GridAbsencesClientCustomButton_Click(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        s.GetRowValues(e.visibleIndex, 'IDEmployee;BeginDate;IDRelatedObject;AbsenceType', GetGridValues);
    } else if (e.buttonID == "ShowEmployeeButton") {
        s.GetRowValues(e.visibleIndex, 'IDEmployee;BeginDate;IDRelatedObject;AbsenceType', GetGridValues2);
    }
}
function GetGridValues(Parameters) {
    document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnSelectedRowVisibleIndex').value = JSON.stringify(Parameters);
    CallbackSessionClient.PerformCallback("CLIENTSHOWDETAILS");
}
function GetGridValues2(Parameters) {
    document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnSelectedRowVisibleIndex').value = JSON.stringify(Parameters);
    CallbackSessionClient.PerformCallback("CLIENTEMPLOYEEDETAILS");
}

function ShowDetailGrid(url, type) {
    var Title = '';
    if (type == 1) { //ProgrammedAbsence
        parent.ShowExternalForm2(url, 830, 450, Title, '', false, false, false);
    } else if (type == 2) { //ProgramedCause
        parent.ShowExternalForm2(url, 830, 450, Title, '', false, false, false);
    } else if (type == 3) {
        PopupEditRequestClient.Show();
        loadRequest(url, -1);
    }
}

function RefreshScreen(MustRefresh, Params) {
    ThrowRefreshClient(false);
}