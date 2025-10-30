function showLoadingGrid(loading) { parent.showLoader(loading); }

function cmbViewClient_ValueChanged() {
    if (cmbViewClient.GetSelectedIndex() == 0) {
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_divEmptyAnalytics').style.top = '227px';
    } else if (cmbViewClient.GetSelectedIndex() == 1) {
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_divEmptyAnalytics').style.top = '255px';
    }

    document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_divEmptyAnalytics').style.display = '';

    CallbackPanelPivotClient.PerformCallback("LOADLAYOUT")
}

//============ SELECTOR DE EMPLEADOS ===============================
function btnOpenPopupSelectorEmployeesClient_Click() {
    PopupSelectorEmployeesClient.Show();
}

function btnPopupSelectorEmployeesAcceptClient_Click() {
    showLoadingGrid(true);
    CallbackSessionClient.PerformCallback("GETINFOSELECTED")
    PopupSelectorEmployeesClient.Hide();
}

//============ OBTENER DATOS =================================
function btnRefreshClient_Click() {
    if (txtDateInfClient.GetDate() > txtDateSupClient.GetDate()) {
        Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
            { text: '', key: 'roJsError' }, { text: '', key: 'roInvalidDatePeriod' },
            { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
            Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
    } else {
        PerformAction();
    }
}

//============ POPUP SAVE VIEWS ====================================
function btnOpenPopupSaveViewClient_Click() {
    txtPopupSaveViewName_Client.SetValue("");
    txtPopupSaveViewDescription_Client.SetValue("");
    PopupSaveViewClient.Show();
}

function btnPopupSaveViewSaveClient_Click() {
    var nameView = txtPopupSaveViewName_Client.GetValue();
    if (nameView == "" || nameView == null) {
        $("#divError").show();
        setTimeout(function () { $("#divError").hide(); }, 2000);
    }
    else {
        //obtener y guardar datos de la pantalla
        var oParameters = {};
        oParameters.idView = cmbViewClient.GetSelectedItem().value;
        oParameters.nameView = nameView
        oParameters.description = txtPopupSaveViewDescription_Client.GetValue()

        var strEmployees = document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnEmployees').value + "#" +
            document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilter').value + "#" +
            document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilterUser').value;
        if (strEmployees == "##") strEmployees = ""
        oParameters.employees = strEmployees

        oParameters.dateInf = formatDateYMD(txtDateInfClient.GetDate());  //en formato yyyy/mm/dd
        oParameters.dateSup = formatDateYMD(txtDateSupClient.GetDate());  //en formato yyyy/mm/dd

        if (document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnUserFieldsSelected').value == "") {
            oParameters.userFields = "";
        } else {
            oParameters.userFields = document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnUserFieldsSelected').value;
        }

        if (typeof ChartType_Client != 'undefined' && ChartType_Client.GetSelectedItem() != null) {
            var chartFilter = '';
            chartFilter = ChartType_Client.GetSelectedItem().value + '#';
            chartFilter += (ShowColumnGrandTotals_Client.GetChecked() ? '1' : '0') + '#';
            chartFilter += (ChartDataVertical_client.GetChecked() ? '1' : '0') + '#';
            chartFilter += (ShowRowGrandTotals_Client.GetChecked() ? '1' : '0') + '#';
            chartFilter += (PointLabels_Client.GetChecked() ? '1' : '0');

            oParameters.graphOptions = chartFilter;
        } else {
            oParameters.graphOptions = '';
        }

        var strParameters = JSON.stringify(oParameters);
        strParameters = "SAVEVIEW#" + encodeURIComponent(strParameters);

        CallbackSessionClient.PerformCallback(strParameters)

        PopupSaveViewClient.Hide();
    }
}

//============ POPUP lISTA de la ficha ========================
function btnOpenPopupUserFields_Click(s, e) {
    PopupUserFieldsClient.Show();
}

function btnPopupUserFieldsAcceptClient_Click(s, e) {
    showLoadingGrid(true);

    var arrUserFieldsValues = ckUserFieldsListClient.GetSelectedValues();
    var hdnUserFieldSelected = document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnUserFieldsSelected')

    if (arrUserFieldsValues.length > 0) {
        hdnUserFieldSelected.value = arrUserFieldsValues;
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_txtUserFields').value = arrUserFieldsValues.length +
            document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnUserFieldsSelectedText').value;
    } else {
        hdnUserFieldSelected.value = "";
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_txtUserFields').value = "";
    }
    PopupUserFieldsClient.Hide();
    showLoadingGrid(false);
}

function ckUserFieldsList_SelectedIndexChanged(s, e) {
    //Controlar que no seleccionen más de 10
    var arrUserFieldsValues = ckUserFieldsListClient.GetSelectedValues();
    if (arrUserFieldsValues.length > 10) {
        var arrayindex = new Array();
        arrayindex.push(e.index);
        s.UnselectIndices(arrayindex);
        document.getElementById("spanErrorSelected").style.display = "";
    } else {
        document.getElementById("spanErrorSelected").style.display = "none";
    }
}

function PopupUserFieldsClient_PopUp(s, e) {
}

//============ POPUP GRID VIEWS ====================================
function btnOpenPopupViewsClient_Click() {
    PopupViewsClient.Show();
}

function PopupViewsClient_PopUp(e) {
    GridViewsClient.PerformCallback("RELOAD");
}

function btnPopupViewsAcceptClient_Click() {
    var nIndex = parseInt(GridViewsClient.focusedRowIndex);
    if (nIndex > -1)
        GridViewsClient.GetRowValues(nIndex, 'ID', LoadSelectedView);
}

function LoadSelectedView(ID) {
    PopupViewsClient.Hide()
    CallbackPanelPivotClient.PerformCallback("LOADVIEW#" + ID)
}

//============ RETORNO DE LLAMADAS DE CALLBACKS SIMPLES ============
function CallbackSession_CallbackComplete(s, e) {
    if (e.parameter == "GETINFOSELECTED") {
        var Limit = e.result.indexOf(";");
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnEmployeesSelected').value = e.result.substring(0, Limit);
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_txtEmployees').value = e.result.substring(Limit + 1);
    } else if (e.result.substring(0, 12) == "VIEWRETURNED") {
        //Cargar la vista en la pantalla
        var strParameters = e.result.substring(13)
        var objParameters = JSON.parse(strParameters)

        //Fechas
        var arrFecha = objParameters.dateInf.split("#");
        var auxFecha = new Date(arrFecha[0], arrFecha[1] - 1, arrFecha[2]);
        txtDateInfClient.SetDate(auxFecha);

        arrFecha = objParameters.dateSup.split("#");
        auxFecha = new Date(arrFecha[0], arrFecha[1] - 1, arrFecha[2]);
        txtDateSupClient.SetDate(auxFecha);

        // ThrowRefreshClient();
    } else if (e.parameter == "PAGEKEEPALIVE") {
        setTimeout(function () {
            if (typeof CallbackSessionClient != 'undefined') CallbackSessionClient.PerformCallback("PAGEKEEPALIVE");
        }, 3 * 60 * 1000);
    }

    showLoadingGrid(false);
}
//==================================================================

function ThrowRefreshClient() {
    showLoadingGrid(true);

    document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_divEmptyAnalytics').style.display = 'none';
    PivotTareasClient.PerformCallback("REFRESH");
    setTimeout(function () {
        if (typeof CallbackSessionClient != 'undefined') CallbackSessionClient.PerformCallback("PAGEKEEPALIVE");
    }, 3 * 60 * 1000);
}

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

function reloadChart(s, e) {
    ThrowRefreshClient();
}

function CallbackPanelPivotClient_EndCallback(s, e) {
    if (cmbViewClient.GetSelectedIndex() == 0) {
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_divEmptyAnalytics').style.top = '227px';
    } else if (cmbViewClient.GetSelectedIndex() == 1) {
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_divEmptyAnalytics').style.top = '255px';
    }
}

function validateData(layoutLoaded) {
    if (typeof layoutLoaded != 'undefined' && !layoutLoaded) {
        Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
            { text: '', key: 'roJsError' }, { text: '', key: 'roProfileCorrupted' },
            { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
            Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
    }
}