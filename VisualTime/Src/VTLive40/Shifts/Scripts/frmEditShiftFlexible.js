function frmEditShiftFlexible_Show(arrNames, arrValues) {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible_tbCont1_";
        var n;

        loadFlexibleBlanks();
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
        for (n = 0; n < arrNames.length; n++) {
            switch (arrNames[n].toLowerCase()) {
                case "rolt_tlid":
                    loadGenericData("LAYERID", "hdnFlexibleLayerID", arrValues[n], "X_NUMBER", "", oDis, false);
                    break;
                case "rolt_begin":
                    txtPresFromTimeFlexClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    break;
                case "rolt_beginday":
                    cmbPresFromTimeFlexClient.SetValue(arrValues[n]);
                    break;
                case "rolt_finish":
                    txtPresToTimeFlexClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    break;
                case "rolt_finishday":
                    cmbPresToTimeFlexClient.SetValue(arrValues[n]);
                    break;
                case "rolt_parentid":
                    if (arrValues[n] != "") {
                        loadGenericData("PARENTID", "hdnFlexibleParentID", arrValues[n], "X_NUMBER", "", oDis, false);
                    }
                    break;
                case "rolt_working_maxtime":
                    if (arrValues[n] != "") {
                        txtDurTime2FlexClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    }
                    break;
                case "rolt_working_maxtimeaction":
                    var oMaxTimeAction = "0";
                    if (arrValues[n].toLowerCase() != "overtime") { oMaxTimeAction = "1"; }
                    cmbMaxThenFlexClient.SetValue(oMaxTimeAction);
                    break;
                case "rolt_working_mintime":
                    if (arrValues[n] != "") {
                        txtDurTime1FlexClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    }
                    break;
            } //end switch
        } //end for

        //load fields
        activeTabContainer('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible_tbCont1', 0);

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible', true);
    } catch (e) { showError("frmEditShiftFlexible_Show", e); }
}

function frmEditShiftFlexible_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible', false);
    } catch (e) { showError("frmEditShiftFlexible_Close", e); }
}

function frmEditShiftFlexible_Load() {
    try {
        //Carrega dels Camps

        frmEditShiftFlexible_Show();
    } catch (e) { showError("frmEditShiftFlexible_Load", e); }
}

function frmEditShiftFlexible_Validate() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible_tbCont1_";

        if (txtDurTime1FlexClient.GetValue() == null || txtDurTime1FlexClient.GetValue() == "") { txtDurTime1FlexClient.SetValue(new Date('1900/01/01 00:00:00')); }
        if (txtDurTime2FlexClient.GetValue() == null || txtDurTime2FlexClient.GetValue() == "") { txtDurTime2FlexClient.SetValue(new Date('1900/01/01 00:00:00')); }

        if (Flexible_CheckTime(txtPresFromTimeFlexClient)) { return false; }
        if (Flexible_CheckTime(txtPresToTimeFlexClient)) { return false; }

        if (Flexible_CheckTime(txtDurTime1FlexClient, true)) { return false; }
        if (Flexible_CheckTime(txtDurTime2FlexClient, true)) { return false; }

        var minutesFrom = jsDate_retMinutesToTime(txtPresFromTimeFlexClient.GetValue().format2Time(), false);
        var minutesTo = jsDate_retMinutesToTime(txtPresToTimeFlexClient.GetValue().format2Time(), false);

        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible_tbCont1_";
        var BeginDay = cmbPresFromTimeFlexClient.GetValue();
        var FinishDay = cmbPresToTimeFlexClient.GetValue();

        minutesFrom = minutesFrom + ((BeginDay - 1) * 1440);
        minutesTo = minutesTo + ((FinishDay - 1) * 1440);

        var maxMinutes = parseInt(minutesTo) - parseInt(minutesFrom);

        var durMinutes1 = jsDate_retMinutesToTime(txtDurTime1FlexClient.GetValue().format2Time(), false);
        var durMinutes2 = jsDate_retMinutesToTime(txtDurTime2FlexClient.GetValue().format2Time(), false);

        if (durMinutes1 > maxMinutes) {
            showErrorPopup("Error.frmFlexible.DurMinutes1Title", "ERROR", "Error.frmFlexible.DurMinutes1Desc", "", "Error.frmFlexible.OK", "Error.frmFlexible.OKDesc", "");
            return false;
        }

        if (durMinutes2 > maxMinutes) {
            showErrorPopup("Error.frmFlexible.DurMinutes2Title", "ERROR", "Error.frmFlexible.DurMinutes2Desc", "", "Error.frmFlexible.OK", "Error.frmFlexible.OKDesc", "");
            return false;
        }

        if (durMinutes1 > durMinutes2) {
            showErrorPopup("Error.frmFlexible.DurMinutes2Major1Title", "ERROR", "Error.frmFlexible.DurMinutes2Major1Desc", "", "Error.frmFlexible.OK", "Error.frmFlexible.OKDesc", "");
            return false;
        }

        return true;
    } catch (e) {
        showError("frmEditShiftFlexible_Validate", e);
        return false;
    }
}

function frmEditShiftFlexible_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'false') { frmEditShiftFlexible_Close(); return; }
        if (frmEditShiftFlexible_Validate()) {
            var oMF = Flexible_FieldsToJSON();

            var IdLayer = document.getElementById('hdnFlexibleLayerID').value;
            if (IdLayer != "-1") {
                oTMDefinitions.updateTimeZone("roLTWorking", IdLayer, oMF);
            } else {
                oTMDefinitions.createTimeZone("roLTWorking", oMF);
            }
            hasChanges(true);
            frmEditShiftFlexible_Close();
        }
    } catch (e) { showError("frmEditShiftFlexible_Save", e); }
}

function frmEditShiftFlexible_ShowNew() {
    try {
        let shiftType = 'unknown';

        if (typeof curShiftType != 'undefined') {
            shiftType = curShiftType;
        }

        if (shiftType == 'PerHours') {
            showErrorPopup("Error.frmFlexible.NotAllowed", "ERROR", "Error.frmFlexible.PerHoursNotAllowed", "", "Error.frmFlexible.OK", "Error.frmFlexible.OKDesc", "");
            return;
        } else {
            loadFlexibleBlanks();

            activeTabContainer('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible_tbCont1', 0);

            showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible', true);
        }


        
    } catch (e) { showError("frmEditShiftFlexible_ShowNew", e); }
}

function loadFlexibleBlanks() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible_tbCont1_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
        //blank fields
        loadGenericData("LAYERID", "hdnFlexibleLayerID", "-1", "X_NUMBER", "", oDis, false);
        loadGenericData("PARENTID", "hdnFlexibleParentID", "-1", "X_NUMBER", "", oDis, false);

        txtPresFromTimeFlexClient.SetValue(new Date('1900/01/01 00:00:00'));
        txtPresToTimeFlexClient.SetValue(new Date('1900/01/01 00:00:00'));
        txtDurTime2FlexClient.SetValue(null);
        txtDurTime1FlexClient.SetValue(null);
        cmbPresFromTimeFlexClient.SetValue(1);
        cmbPresToTimeFlexClient.SetValue(1);
        cmbMaxThenFlexClient.SetValue(0);
    } catch (e) { showError("loadFlexibleBlanks", e); }
}

function Flexible_FieldsToJSON() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftFlexible_tbCont1_";

        var ParentID = document.getElementById('hdnFlexibleParentID').value;
        var Working_ParentID = "";
        var Working_MaxTimeAction = "";

        if (cmbMaxThenFlexClient.GetValue() == 0) {
            Working_MaxTimeAction = "Overtime"
        } else {
            Working_MaxTimeAction = "Incidence"
        }

        var oAtts = [{ 'attname': 'rolt_tlid', 'value': document.getElementById('hdnFlexibleLayerID').value },
        { 'attname': 'rolt_begin', 'value': txtPresFromTimeFlexClient.GetValue().format2Time() },
        { 'attname': 'rolt_beginday', 'value': cmbPresFromTimeFlexClient.GetValue() },
        { 'attname': 'rolt_finish', 'value': txtPresToTimeFlexClient.GetValue().format2Time() },
        { 'attname': 'rolt_finishday', 'value': cmbPresToTimeFlexClient.GetValue() },
        { 'attname': 'rolt_datediffmin', 'value': 0 },
        { 'attname': 'rolt_working_parentid', 'value': Working_ParentID },
        { 'attname': 'rolt_working_beginday', 'value': cmbPresFromTimeFlexClient.GetValue() },
        { 'attname': 'rolt_working_begin', 'value': txtPresFromTimeFlexClient.GetValue().format2Time() },
        { 'attname': 'rolt_working_finishday', 'value': cmbPresToTimeFlexClient.GetValue() },
        { 'attname': 'rolt_working_finish', 'value': txtPresToTimeFlexClient.GetValue().format2Time() },
        { 'attname': 'rolt_working_maxtime', 'value': txtDurTime2FlexClient.GetValue().format2Time() },
        { 'attname': 'rolt_working_maxtimeaction', 'value': Working_MaxTimeAction },
        { 'attname': 'rolt_working_mintime', 'value': txtDurTime1FlexClient.GetValue().format2Time() }
        ];

        return oAtts;
    } catch (e) {
        showError("Flexible_FieldsToJSON", e);
        return null;
    }
}

function Flexible_CheckTime(obj, permitBlank) {
    try {
        if (permitBlank == false) {
            if (obj.GetValue() == "") {
                showErrorPopup("Error.frmFlexible." + obj.id + "Title", "ERROR", "Error.frmFlexible." + obj.id + "Desc", "", "Error.frmFlexible.OK", "Error.frmFlexible.OKDesc", "");
                return true;
            }
        }
        return false;
    } catch (e) { showError("Flexible_CheckTime", e); }
}