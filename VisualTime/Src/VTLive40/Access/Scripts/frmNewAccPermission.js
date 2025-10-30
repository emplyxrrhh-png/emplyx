function save() {
    frmNewAccPermission_Save();
    return false;
}
function close() {
    frmNewAccPermission_Close();
    return false;
}

function frmNewAccPermission_Show(arrValues) {
    try {
        //load fields
        var n;
        var objPrFrm = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewAccPermission1_";

        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        loadNewAccPermissionBlanks();

        for (n = 0; n < arrValues.length; n++) {
            switch (arrValues[n].attname.toLowerCase()) {
                case "jsgridatt_idaccesspermission":
                    loadGenericData("IDACCESSPERMISSION", "hdnAddNewAccPermissionID", arrValues[n].value, "X_NUMBER", "", oDis, false);
                    break;
                case "jsgridatt_idzone":
                    cmbZoneClient.SetValue(parseInt(arrValues[n].value, 10));
                    cmbZoneClient.SetEnabled((oDis == 'false' ? true : false));
                    //loadGenericData("IDZONE", objPrFrm + "cmbZone," + objPrFrm + "cmbZone_Text," + objPrFrm + "cmbZone_Value", arrValues[n].value, "X_COMBOBOX", "", oDis, false);
                    break;
                case "jsgridatt_idaccessperiod":
                    cmbPeriodClient.SetValue(parseInt(arrValues[n].value), 10);
                    cmbPeriodClient.SetEnabled((oDis == 'false' ? true : false));
                    //loadGenericData("IDPERIOD", objPrFrm + "cmbPeriod," + objPrFrm + "cmbPeriod_Text," + objPrFrm + "cmbPeriod_Value", arrValues[n].value, "X_COMBOBOX", "", oDis, false);
                    break;
            }
        }

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewAccPermission1', true);
    } catch (e) { showError("frmNewAccPermission_Show", e); }
}

//Validacio del formulari
function frmNewAccPermission_Validate() {
    try {
        var objPrFrm = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewAccPermission1_";

        //if (document.getElementById(objPrFrm + "cmbZone_Value").value == "") {
        if (cmbZoneClient.GetSelectedItem().value == -1) {
            showErrorPopup("Error.frmNewAccPermission.cmbZoneTitle", "ERROR", "Error.frmNewAccPermission.cmbZoneTitleDesc", "Error.frmNewAccPermission.OK", "Error.frmNewAccPermission.OKDesc", "");
            return false;
        }

        //if (document.getElementById(objPrFrm + "cmbPeriod_Value").value == "") {
        if (cmbPeriodClient.GetSelectedItem().value == -1) {
            showErrorPopup("Error.frmNewAccPermission.cmbPeriodTitle", "ERROR", "Error.frmNewAccPermission.cmbPeriodTitleDesc", "Error.frmNewAccPermission.OK", "Error.frmNewAccPermission.OKDesc", "");
            return false;
        }

        return true;
    } catch (e) {
        showError("frmNewAccPermission_Validate", e);
        return false;
    }
}

function frmNewAccPermission_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewAccPermission1', false);
    } catch (e) { showError("frmNewAccPermission_Close", e); }
}

function frmNewAccPermission_Load() {
    try {
        //Carrega dels Camps
        frmNewAccPermission_Show();
    } catch (e) { showError("frmNewAccPermission_Load", e); }
}

//Grabació del formulari en el roTimeLine
function frmNewAccPermission_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "false") { frmNewAccPermission_Close(); return; }
        if (frmNewAccPermission_Validate()) {
            var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewAccPermission1_";
            var oMF = AddNewAccPermission_FieldsToJSON();

            var rowID = document.getElementById('hdnAddAccPermissionIDRow').value;
            updateAddAccPermissionRow(rowID, oMF);

            hasChanges(true);
            frmNewAccPermission_Close();
        }
    } catch (e) { showError("frmNewAccPermission_Save", e); }
}

function frmNewAccPermission_ShowNew() {
    try {
        loadNewAccPermissionBlanks();

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewAccPermission1', true);
    } catch (e) { showError("frmNewAccPermission_ShowNew", e); }
}

function loadNewAccPermissionBlanks() {
    try {
        var objPrFrm = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewAccPermission1_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        loadGenericData("IDACCESSPERMISSION", "hdnAddNewAccPermissionID", "", "X_NUMBER", "", oDis, false);
        //loadGenericData("IDZONE", objPrFrm + "cmbZone," + objPrFrm + "cmbZone_Text," + objPrFrm + "cmbZone_Value", "", "X_COMBOBOX", "", oDis, false);
        cmbZoneClient.SetValue(-1);
        cmbZoneClient.SetEnabled((oDis == 'false' ? true : false));

        //loadGenericData("IDPERIOD", objPrFrm + "cmbPeriod," + objPrFrm + "cmbPeriod_Text," + objPrFrm + "cmbPeriod_Value", "", "X_COMBOBOX", "", oDis, false);
        cmbPeriodClient.SetValue(-1);
        cmbPeriodClient.SetEnabled((oDis == 'false' ? true : false));
    } catch (e) { showError("loadNewAccPermissionBlanks", e); }
}

//Carrega de tots els camps en un objecte JSON
function AddNewAccPermission_FieldsToJSON() {
    try {
        var objPrFrm = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewAccPermission1_";
        var objPrOpt1 = "opTypePeriodNormal_";
        var objPrOpt2 = "opTypePeriodEspecific_";

        var IDAccessPermission = document.getElementById("hdnAddNewAccPermissionID").value;
        var IDZone;
        var ZoneName;
        var IDPeriod;
        var PeriodName;

        //IDZone = document.getElementById(objPrFrm + "cmbZone_Value").value;
        //ZoneName = document.getElementById(objPrFrm + "cmbZone_Text").value;
        //IDPeriod = document.getElementById(objPrFrm + "cmbPeriod_Value").value;
        //PeriodName = document.getElementById(objPrFrm + "cmbPeriod_Text").value;

        var item = cmbZoneClient.GetSelectedItem();
        IDZone = item.value;
        ZoneName = item.text;
        item = cmbPeriodClient.GetSelectedItem();
        IDPeriod = item.value;
        PeriodName = item.text;

        var oAtts = [{ 'attname': 'idaccesspermission', 'value': IDAccessPermission },
        { 'attname': 'idzone', 'value': IDZone },
        { 'attname': 'zonename', 'value': ZoneName },
        { 'attname': 'idaccessperiod', 'value': IDPeriod },
        { 'attname': 'periodname', 'value': PeriodName }
        ];

        return oAtts;
    } catch (e) {
        showError("AddNewAccPermission_FieldsToJSON", e);
        return null;
    }
}