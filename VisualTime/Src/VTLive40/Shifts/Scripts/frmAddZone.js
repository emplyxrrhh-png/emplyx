function frmAddZone_Show(arrNames, arrValues) {
    try {
        //load fields
        var n;
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        loadAddZoneBlanks();

        for (n = 0; n < arrNames.length; n++) {
            switch (arrNames[n].toLowerCase()) {
                case "rolt_tlid":
                    loadGenericData("LAYERID", "hdnAddZoneLayer", arrValues[n], "X_NUMBER", "",oDis,false);
                    loadGenericData("TYPEZONE", "hdnAddZoneType", "HourZone" + arrValues[n].split("_")[0], "X_TEXT", "", oDis, false);
                    cmbTypeAddZoneClient.SetValue(arrValues[n].split("_")[0]);
                    break;
                case "rolt_begin":
                    txtFromTimeAddZoneClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    break;
                case "rolt_beginday":
                    cmbShiftFromTimeAddZoneClient.SetValue(arrValues[n]);
                    break;
                case "rolt_finish":
                    txtToTimeAddZoneClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    break;
                case "rolt_finishday":
                    cmbShiftToTimeAddZoneClient.SetValue(arrValues[n]);
                    break;
                case "rolt_islocked":
                    loadGenericData("ISLOCKED", objPrTab + "chkIsLocked", arrValues[n], "X_CHECKBOX", "", oDis, false);
                    break;
            }
        }

        var oDivCmb = document.getElementById('panTypeCmb');
        var oDivRO = document.getElementById('panTypeRO');
        var oDivTools = document.getElementById('divZoneListActions');

        oDivCmb.style.display = "none";
        oDivRO.style.display = "";
        oDivTools.style.display = "none";

        // Si el horarios es flotante mostramos el chk de bloqueo
        var chkIsLocked = document.getElementById(objPrTab + "chkIsLocked");
        var lblIsLocked = document.getElementById(objPrTab + "lblIsLocked");
        if (getIsFloating() == false) {
            chkIsLocked.style.display = "none";
            lblIsLocked.style.display = "none";
        }
        else {
            chkIsLocked.style.display = "";
            lblIsLocked.style.display = "";
        }

        document.getElementById(objPrTab + "lblCmbTypeDesc").textContent = cmbTypeAddZoneClient.GetSelectedItem().text;

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1', true);
    } catch (e) { showError("frmAddZone_Show", e); }
}

function frmAddZone_Validate() {
    try {
        if (AddZone_CheckTime(txtToTimeAddZoneClient)) { return false; }
        if (AddZone_CheckTime(txtFromTimeAddZoneClient)) { return false; }

        if (cmbTypeAddZoneClient.GetValue() == null) {
            showErrorPopup("Error.frmAddZone.cmbTypeTitle", "ERROR", "Error.frmAddZone.cmbTypeTitleDesc", "", "Error.frmAddZone.OK", "Error.frmAddZone.OKDesc", "");
            return false;
        }
		var startTicks = (txtFromTimeAddZoneClient.GetValue().getTime() + (cmbShiftFromTimeAddZoneClient.GetValue() * (24*60*60*10000000)));
		var endTicks = (txtToTimeAddZoneClient.GetValue().getTime() + (cmbShiftToTimeAddZoneClient.GetValue() * (24*60*60*10000000)));
        if (startTicks >= endTicks) {
            showErrorPopup("Error.frmAddZone.cmbDateTitle", "ERROR", "Error.frmAddZone.InvalidDateRange", "", "Error.frmAddZone.OK", "Error.frmAddZone.OKDesc", "");
            return false;
        }

        return true;
    } catch (e) {
        showError("frmAddZone_Validate", e);
        return false;
    }
}

function frmAddZone_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1', false);
    } catch (e) { showError("frmAddZone_Close", e); }
}

function frmAddZone_Load() {
    try {
        //Carrega dels Camps

        frmAddZone_Show();
    } catch (e) { showError("frmAddZone_Load", e); }
}

function frmAddZone_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "false") { frmAddZone_Close(); return; }
        if (frmAddZone_Validate()) {
            var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1_";
            var oMF = AddZone_FieldsToJSON();

            var IdLayer = cmbTypeAddZoneClient.GetValue();
            var oDivCmb = document.getElementById('panTypeCmb');

            //Si es visible el combo, create
            if (oDivCmb.style.display == "") {
                oTMZones.createTimeZone("HourZone" + IdLayer, oMF);
            } else { //si no, update
                IdLayerComp = document.getElementById('hdnAddZoneLayer').value;
                oTMZones.updateTimeZone("HourZone" + IdLayer, IdLayerComp, oMF);
            }
            hasChanges(true);
            frmAddZone_Close();
        }
    } catch (e) { showError("frmAddZone_Save", e); }
}

function frmAddZone_ShowNew() {
    try {
        loadAddZoneBlanks();

        var oDivCmb = document.getElementById('panTypeCmb');
        var oDivRO = document.getElementById('panTypeRO');
        var oDivTools = document.getElementById('divZoneListActions');

        oDivCmb.style.display = "";
        oDivRO.style.display = "none";
        oDivTools.style.display = "";

        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1_";

        // Si el horarios es flotante mostramos el chk de bloqueo
        var chkIsLocked = document.getElementById(objPrTab + "chkIsLocked");
        var lblIsLocked = document.getElementById(objPrTab + "lblIsLocked");
        if (getIsFloating() == false) {
            chkIsLocked.style.display = "none";
            lblIsLocked.style.display = "none";
        }
        else {
            chkIsLocked.style.display = "";
            lblIsLocked.style.display = "";
        }

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1', true);
    } catch (e) { showError("frmAddZone_ShowNew", e); }
}

function loadAddZoneBlanks() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        //blank fields
        loadGenericData("LAYERID", "hdnAddZoneLayer", "-1", "X_NUMBER", "",oDis, false);
        cmbTypeAddZoneClient.SetValue('');
        txtFromTimeAddZoneClient.SetValue(new Date('1900/01/01 00:00:00'));
        cmbShiftFromTimeAddZoneClient.SetValue(1);
        txtToTimeAddZoneClient.SetValue(new Date('1900/01/01 00:00:00'));
        cmbShiftToTimeAddZoneClient.SetValue(1);
        loadGenericData("ISLOCKED", objPrTab + "chkIsLocked", "true", "X_CHECKBOX", "", oDis, false);

        if (oDis == "true") {
            cmbTypeAddZoneClient.SetEnabled(false);
            txtFromTimeAddZoneClient.SetEnabled(false);
            cmbShiftFromTimeAddZoneClient.SetEnabled(false);
            txtToTimeAddZoneClient.SetEnabled(false);
            cmbShiftToTimeAddZoneClient.SetEnabled(false);
        }
    } catch (e) { showError("loadAddZoneBlanks", e); }
}

function AddZone_CheckTime(obj, permitBlank) {
    try {
        if (permitBlank == false) {
            if (obj.GetValue() == "") {
                showErrorPopup("Error.frmAddZone." + obj.id + "Title", "ERROR", "Error.frmAddZone." + obj.id + "Desc", "", "Error.frmAddZone.OK", "Error.frmAddZone.OKDesc", "");
                return true;
            }
        }

        return false;
    } catch (e) { showError("AddZone_CheckTime", e); }
}

function AddZone_FieldsToJSON() {
    try {
        var oAtts = [{ 'attname': 'rolt_tlid', 'value': cmbTypeAddZoneClient.GetValue() },
                       { 'attname': 'rolt_begin', 'value': txtFromTimeAddZoneClient.GetValue().format2Time() },
                       { 'attname': 'rolt_beginday', 'value': cmbShiftFromTimeAddZoneClient.GetValue() },
                       { 'attname': 'rolt_finish', 'value': txtToTimeAddZoneClient.GetValue().format2Time() },
                       { 'attname': 'rolt_finishday', 'value': cmbShiftToTimeAddZoneClient.GetValue() },
                       { 'attname': 'rolt_datediffmin', 'value': 0 },
                       { 'attname': 'rolt_title', 'value': cmbTypeAddZoneClient.GetSelectedItem().text },
                       { 'attname': 'rolt_islocked', 'value': document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddZone1_chkIsLocked").checked }
                      ];

        return oAtts;
    } catch (e) {
        showError("AddZone_FieldsToJSON", e);
        return null;
    }
}