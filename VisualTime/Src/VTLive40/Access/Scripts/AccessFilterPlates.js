//============ POPUP SAVE VIEWS ====================================
function btnOpenPopupSaveViewClient_Click() {
    txtPopupSaveViewName_Client.SetValue("");
    txtPopupSaveViewDescription_Client.SetValue("");
    PopupSaveViewClient.Show();
}

function btnPopupSaveViewSaveClient_Click() {
    let nameView = txtPopupSaveViewName_Client.GetValue();
    if (nameView == "" || nameView == null) {
        $("#divError").show();
        setTimeout(function () { $("#divError").hide(); }, 2000);
    }
    else {
        //obtener y guardar datos de la pantalla
        let oParameters = {};
        oParameters.nameView = nameView
        oParameters.description = txtPopupSaveViewDescription_Client.GetValue()

        let strEmployees = document.getElementById('CallbackPanelPivot_hdnEmployees').value + "#" +
            document.getElementById('CallbackPanelPivot_hdnFilter').value + "#" +
            document.getElementById('CallbackPanelPivot_hdnFilterUser').value;
        if (strEmployees == "##") strEmployees = ""
        oParameters.employees = strEmployees

        oParameters.dateInf = formatDateYMD(txtDateInfClient.GetDate());  //en formato yyyy/mm/dd
        oParameters.dateSup = formatDateYMD(txtDateSupClient.GetDate());  //en formato yyyy/mm/dd

        let tmpPlate1 = txtPlate1Client.GetValue();
        if (tmpPlate1 == "" || tmpPlate1 == null) tmpPlate1 = "";
        let tmpPlate2 = txtPlate2Client.GetValue();
        if (tmpPlate2 == "" || tmpPlate2 == null) tmpPlate2 = "";
        let tmpPlate3 = txtPlate3Client.GetValue();
        if (tmpPlate3 == "" || tmpPlate3 == null) tmpPlate3 = "";
        let tmpPlate4 = txtPlate4Client.GetValue();
        if (tmpPlate4 == "" || tmpPlate4 == null) tmpPlate4 = "";

        if (tmpPlate1.length + tmpPlate2.length + tmpPlate3.length + tmpPlate4.length > 0) {
            oParameters.filterData = tmpPlate1 + "¬" + tmpPlate2 + "¬" + tmpPlate3 + "¬" + tmpPlate4;
        }
        else {
            oParameters.filterData = "";
        }

        let strParameters = JSON.stringify(oParameters);
        strParameters = "SAVEVIEW#" + encodeURIComponent(strParameters);

        CallbackSessionClient.PerformCallback(strParameters)

        PopupSaveViewClient.Hide();
    }
}

function formatDateYMD(initialDate) {
    if (initialDate == "" || initialDate == null) {
        return "";
    }
    else {
        let dia = initialDate.getDate();
        if (dia.toString().length == 1) dia = "0" + dia.toString();
        let mes = initialDate.getMonth() + 1; //Months are zero based
        if (mes.toString().length == 1) mes = "0" + mes.toString();
        let ejer = initialDate.getFullYear();
        let finalDate = ejer + "#" + mes + "#" + dia;
        return finalDate;
    }
}
//==================================================================
//==================================================================

//============ POPUP GRID VIEWS ====================================
function btnOpenPopupViewsClient_Click() {
    PopupViewsClient.Show();
}

function PopupViewsClient_PopUp(e) {
    GridViewsClient.PerformCallback("RELOAD");
}

function btnPopupViewsAcceptClient_Click() {
    let nIndex = parseInt(GridViewsClient.focusedRowIndex);
    if (nIndex > -1)
        GridViewsClient.GetRowValues(nIndex, 'ID', LoadSelectedView);
}

function LoadSelectedView(ID) {
    PopupViewsClient.Hide()
    CallbackPanelPivotClient.PerformCallback("LOADVIEW#" + ID)
}
//==================================================================
//==================================================================

//============ SELECTOR DE EMPLEADOS ===============================
function btnOpenPopupSelectorEmployeesClient_Click() {
    PopupSelectorEmployeesClient.Show();
}

function btnPopupSelectorEmployeesAcceptClient_Click() {
    CallbackSessionClient.PerformCallback("GETINFOSELECTED")
    PopupSelectorEmployeesClient.Hide();
}
//==================================================================

//============ RETORNO DE LLAMADAS DE CALLBACKS SIMPLES ============
async function CallbackSession_CallbackComplete(s, e) {
    if (e.parameter == "GETINFOSELECTED") {
        let Limit = e.result.indexOf(";");
        document.getElementById('CallbackPanelPivot_hdnEmployeesSelected').value = e.result.substring(0, Limit);
        txtEmployeesClient.SetText(e.result.substring(Limit + 1));
        if (txtEmployeesClient.GetText() == "")
            txtEmployeesClient.SetText(hdnAllEmployees.value);
    }

    if (e.result.substring(0, 12) == "VIEWRETURNED") {
        //Cargar la vista en la pantalla
        let strParameters = e.result.substring(13)
        let objParameters = JSON.parse(strParameters)

        //Empleados
        let strEmployees = objParameters.employees;
        if (strEmployees != "") {
            let arrEmployees = strEmployees.split("#");
            document.getElementById('CallbackPanelPivot_hdnEmployees').value = arrEmployees[0];
            document.getElementById('CallbackPanelPivot_hdnFilter').value = arrEmployees[1];
            document.getElementById('CallbackPanelPivot_hdnFilterUser').value = arrEmployees[2];

            let oTreeState = await getroTreeState('objContainerTreeV3_treeEmpFilterPlatesGrid');
            oTreeState.setSelected(arrEmployees[0], "1");

            //arbol roTree
            oTreeState = await getroTreeState('objContainerTreeV3_treeEmpFilterPlates');
            oTreeState.setSelected("", "1");
            oTreeState.setSelectedPath("", "1");
            oTreeState.setFilter(arrEmployees[1], arrEmployees[2]);
        }
        else {
            document.getElementById('CallbackPanelPivot_hdnEmployees').value = "";
            document.getElementById('CallbackPanelPivot_hdnFilter').value = "";
            document.getElementById('CallbackPanelPivot_hdnFilterUser').value = "";
        }

        //Fechas
        let arrFecha = objParameters.dateInf.split("#");
        let auxFecha = new Date(arrFecha[0], arrFecha[1] - 1, arrFecha[2]);
        txtDateInfClient.SetDate(auxFecha);

        arrFecha = objParameters.dateSup.split("#");
        auxFecha = new Date(arrFecha[0], arrFecha[1] - 1, arrFecha[2]);
        txtDateSupClient.SetDate(auxFecha);

        ThrowRefreshClient();
    }
}
//==================================================================

//============ OBTENER DATOS =================================
function btnRefreshClient_Click() {
    ThrowRefreshClient();
}

function PopupWarningYes() {
    PopupWarningClient.Hide();
    ThrowRefreshClient();
}

function ThrowRefreshClient() {
    GridFilterPlatesClient.Refresh();
}
