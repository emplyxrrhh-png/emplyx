var hdnAddSirenModeEditId = 'ctl00_contentMainBody_hdnModeEdit';

function frmAddSiren_Show(arrValues) {
    try {
        //load fields
        var n;
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddSiren1_";
        var oDis = document.getElementById(hdnAddSirenModeEditId).value;

        loadAddSirenBlanks();

        for (n = 0; n < arrValues.length; n++) {
            switch (arrValues[n].attname.toLowerCase()) {
                case "jsgridatt_id":
                    loadGenericData("ID", "hdnAddSirenID", arrValues[n].value, "X_NUMBER", "", oDis, false);
                    break;
                case "jsgridatt_weekdayname": //nom semana
                    break;
                case "jsgridatt_weekday":
                    cmbSirenWeekDayClient.SetValue(arrValues[n].value);
                    break;
                case "jsgridatt_hour":
                    txtSirenTimeClient.SetValue(new Date('1900/01/01 ' + arrValues[n].value + ':00'));
                    break;
                case "jsgridatt_duration":
                    txtSirenDurationClient.SetValue(arrValues[n].value);
                    break;
            }
        }

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddSiren1', true);
    } catch (e) { showError("frmAddSiren_Show", e); }
}

//Validacio del formulari
function frmAddSiren_Validate() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddSiren1_";

        if (cmbSirenWeekDayClient.GetSelectedItem() == null || cmbSirenWeekDayClient.GetValue() == "") {
            showErrorPopup("Error.frmAddSiren.cmbWeekDayTitle", "ERROR", "Error.frmAddSiren.cmbWeekDayDesc", "", "Error.frmAddSiren.OK", "Error.frmAddSiren.OKDesc", "");
            return false;
        }

        if (txtSirenDurationClient.GetValue() == "" || txtSirenDurationClient.GetValue() <= 0) {
            showErrorPopup("Error.frmAddSiren.txtSirenDurationTitle", "ERROR", "Error.frmAddSiren.txtSirenDurationDesc", "", "Error.frmAddSiren.OK", "Error.frmAddSiren.OKDesc", "");
            return false;
        }

        return true;
    } catch (e) {
        showError("frmAddSiren_Validate", e);
        return false;
    }
}

function frmAddSiren_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddSiren1', false);
    } catch (e) { showError("frmAddSiren_Close", e); }
}

function frmAddSiren_Load() {
    try {
        //Carrega dels Camps
        frmAddSiren_Show();
    } catch (e) { showError("frmAddSiren_Load", e); }
}

//Grabació del formulari en el roTimeLine
function frmAddSiren_Save() {
    try {
        if (document.getElementById(hdnAddSirenModeEditId).value != "false") { frmAddSiren_Close(); return; }
        if (frmAddSiren_Validate()) {
            var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddSiren1_";
            var oMF = AddSiren_FieldsToJSON();

            var rowID = document.getElementById('hdnAddSirenIDRow').value;
            updateAddSirenRow(rowID, oMF);

            hasChanges(true);
            frmAddSiren_Close();
        }
    } catch (e) { showError("frmAddSiren_Save", e); }
}

function frmAddSiren_ShowNew() {
    try {
        loadAddSirenBlanks();

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddSiren1', true);
    } catch (e) { showError("frmAddSiren_ShowNew", e); }
}

function loadAddSirenBlanks() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddSiren1_";
        var oDis = document.getElementById(hdnAddSirenModeEditId).value;

        loadGenericData("ID", "hdnAddSirenID", "-1", "X_NUMBER", "", oDis, false);
        cmbSirenWeekDayClient.SetValue("");
        txtSirenTimeClient.SetValue(new Date('1900/01/01 00:00:00'));
        txtSirenDurationClient.SetValue(1);
    } catch (e) { showError("loadAddSirenBlanks", e); }
}
//Carrega de tots els camps en un objecte JSON
function AddSiren_FieldsToJSON() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddSiren1_";

        var ID = document.getElementById("hdnAddSirenID").value;

        var oAtts = [{ 'attname': 'id', 'value': ID },
        { 'attname': 'weekdayname', 'value': cmbSirenWeekDayClient.GetSelectedItem().text },
        { 'attname': 'weekday', 'value': cmbSirenWeekDayClient.GetSelectedItem().value },
        { 'attname': 'hour', 'value': txtSirenTimeClient.GetValue().format2Time() },
        { 'attname': 'duration', 'value': txtSirenDurationClient.GetValue() }
        ];

        return oAtts;
    } catch (e) {
        showError("AddSiren_FieldsToJSON", e);
        return null;
    }
}