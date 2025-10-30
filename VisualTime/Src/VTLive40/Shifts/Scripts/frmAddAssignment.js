function frmAddAssignment_Show(arrValues) {
    try {
        loadAddAssignmentBlanks();

        for (var n = 0; n < arrValues.length; n++) {
            switch (arrValues[n].attname.toLowerCase()) {
                case "jsgridatt_idassignment":
                    cmbAddAssignmentClient.SetValue(arrValues[n].value);
                    break;
                case "jsgridatt_coverage":
                    txtAddCoverageClient.SetValue(arrValues[n].value);
                    break;
            }
        }

        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAssignment1', true);
    } catch (e) { showError("frmAddAssignment_Show", e); }
}

//Validacio del formulari
function frmAddAssignment_Validate() {
    try {
        //if (txtAddCoverageClient.GetValue() == "") {
        //    showErrorPopup("Error.frmAddAssignment.txtCoverageTitle", "ERROR", "Error.frmAddAssignment.txtCoverageDesc", "", "Error.frmAddAssignment.OK", "Error.frmAddAssignment.OKDesc", "");
        //    return false;
        //}

        if (cmbAddAssignmentClient.GetValue() == "") {
            showErrorPopup("Error.frmAddAssignment.cmbAssignmentTitle", "ERROR", "Error.frmAddAssignment.cmbAssignmentDesc", "", "Error.frmAddAssignment.OK", "Error.frmAddAssignment.OKDesc", "");
            return false;
        }

        return true;
    } catch (e) {
        showError("frmAddAssignment_Validate", e);
        return false;
    }
}

function frmAddAssignment_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAssignment1', false);
    } catch (e) { showError("frmAddAssignment_Close", e); }
}

function frmAddAssignment_Load() {
    try {
        //Carrega dels Camps
        frmAddAssignment_Show();
    } catch (e) { showError("frmAddAssignment_Load", e); }
}

//Grabació del formulari en el roTimeLine
function frmAddAssignment_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "false") { frmAddAssignment_Close(); return; }
        if (frmAddAssignment_Validate()) {
            var oMF = AddAssignment_FieldsToJSON();

            var rowID = document.getElementById('hdnAddAssignmentIDRow').value;
            updateAddAssignmentRow(rowID, oMF);

            hasChanges(true,false);
            frmAddAssignment_Close();
        }
    } catch (e) { showError("frmAddAssignment_Save", e); }
}

function frmAddAssignment_ShowNew() {
    try {
        loadAddAssignmentBlanks();

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAssignment1', true);
    } catch (e) { showError("frmAddAssignment_ShowNew", e); }
}

function loadAddAssignmentBlanks() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAssignment1_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
        loadGenericData("IDSHIFT", "hdnAddAssignmentIDShift", "", "X_NUMBER", "", oDis, false);
        cmbAddAssignmentClient.SetValue("");
        //txtAddCoverageClient.SetValue("");
        if (oDis == "true") {
            cmbAddAssignmentClient.SetEnabled(false);
            //txtAddCoverageClient.SetEnabled(false);
        }
    } catch (e) { showError("loadAddAssignmentBlanks", e); }
}

//Carrega de tots els camps en un objecte JSON
function AddAssignment_FieldsToJSON() {
    try {
        var oAtts = [{ 'attname': 'idassignment', 'value': cmbAddAssignmentClient.GetSelectedItem().value },
                       { 'attname': 'assignmentname', 'value': cmbAddAssignmentClient.GetSelectedItem().text },
                       { 'attname': 'coverage', 'value': '100' }]

        return oAtts;
    } catch (e) {
        showError("AddAssignment_FieldsToJSON", e);
        return null;
    }
}