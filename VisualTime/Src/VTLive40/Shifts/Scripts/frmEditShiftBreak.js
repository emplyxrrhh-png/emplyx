function frmEditShiftBreak_Show(arrNames, arrValues) {
    try {
        //load fields
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak_tbCont1_";
        var n;
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
        loadBreakBlanks();

        for (n = 0; n < arrNames.length; n++) {
            switch (arrNames[n].toLowerCase()) {
                case "rolt_tlid":
                    loadGenericData("LAYERID", "hdnBreakLayerID", arrValues[n], "X_NUMBER", "", oDis, false);
                    break;
                case "rolt_begin":
                    txtCanAbsFromBreakClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    break;
                case "rolt_beginday":
                    cmbCanAbsFromBreakClient.SetValue(arrValues[n]);
                    break;
                case "rolt_finish":
                    txtCanAbsToBreakClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    break;
                case "rolt_finishday":
                    cmbCanAbsToBreakClient.SetValue(arrValues[n]);
                    break;
                case "rolt_maxbreaktime":
                    if (arrValues[n] != "") {
                        txtChkCanAbsBreakClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                        loadGenericData("CHKCANABS", "chkCanAbs", "true", "X_CHECKBOX", "", oDis, false);
                    }
                    break;
                case "rolt_minbreakaction": // = "SubstractAttendanceTime"
                    if (arrValues[n].toLowerCase() == "substractattendancetime") {
                        loadGenericData("OPTDEBINC", "optDebInc", "false", "X_RADIO", "", oDis, false);
                        loadGenericData("OPTDEBDESC", "optDebDesc", "true", "X_RADIO", "", oDis, false);
                    } else if (arrValues[n].toLowerCase() == "createincidence") {
                        loadGenericData("OPTDEBINC", "optDebInc", "true", "X_RADIO", "", oDis, false);
                        loadGenericData("OPTDEBDESC", "optDebDesc", "false", "X_RADIO", "", oDis, false);
                    }
                    break;
                case "rolt_minbreaktime":
                    if (arrValues[n] != "") {
                        txtChkDebAbsBreakClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                        loadGenericData("CHKDEBABS", "chkDebAbs", "true", "X_CHECKBOX", "", oDis, false);
                    }
                    break;
                case "rolt_nopunchbreaktime":
                    if (arrValues[n] != "") {
                        txtChkBreakTimeBreakClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                        loadGenericData("CHKBREAKTIME", "chkBreakTime", "true", "X_CHECKBOX", "", oDis, false);
                    }
                    break;
                case "rolt_paid_1040_parentid":
                    break;
                case "rolt_paid_1040_id":
                    break;
                case "rolt_paid_1040_value":
                    if (arrValues[n] != "") {
                        txtChkTimeAbonBreakClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                        loadGenericData("CHKTIMEABON", "chkTimeAbon", "true", "X_CHECKBOX", "", oDis, false);
                    }
                    break;
                case "rolt_paid_1040_action":
                    break;
                case "rolt_notificationforuser":

                    loadGenericData("CHKNOTIFICATIONUSER", "chkNotificationUser", arrValues[n], "X_CHECKBOX", "", oDis, false);
                    break;
                case "rolt_notificationforsupervisor":
                    loadGenericData("CHKNOTIFICATIONSUPERVISOR", "chkNotificationSupervisor", arrValues[n], "X_CHECKBOX", "", oDis, false);
                    break;
                case "rolt_notificationforuserbeforetime":
                    if (arrValues[n] != "") {
                        txtNotificationUserBeforeClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    }
                    break;
                case "rolt_notificationforuseraftertime":
                    if (arrValues[n] != "") {
                        txtNotificationUserAfterClient.SetValue(new Date('1900/01/01 ' + arrValues[n] + ':00'));
                    }
                    break;
            }
        }

        // activeTabContainer('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak_tbCont1', 0);

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak', true);
    } catch (e) { showError("frmEditShiftBreak_Show", e); }
}

function frmEditShiftBreak_Validate() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak_tbCont1_";

        if (Break_CheckTime(txtCanAbsFromBreakClient)) { return false; }
        if (Break_CheckTime(txtCanAbsToBreakClient)) { return false; }
        if (curShiftType != 'PerHours') {
            if (cmbCanAbsFromBreakClient.GetValue() == '3' || cmbCanAbsToBreakClient.GetValue() == '3') {
                showErrorPopup("Error.LayerBreakTimeInterval", "error", "Error.LayerBreakTimeIntervalDesc", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        if ((cmbCanAbsFromBreakClient.GetValue() == '3' && cmbCanAbsToBreakClient.GetValue() != '3') || (cmbCanAbsFromBreakClient.GetValue() != '3' && cmbCanAbsToBreakClient.GetValue() == '3')) {
            showErrorPopup("Error.LayerBreakTimeInterval", "error", "Error.LayerBreakTimeIntervalTypeDesc", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }
        var chkCanAbs = document.getElementById('chkCanAbs');
        var chkDebAbs = document.getElementById('chkDebAbs');

        var chkTimeAbon = document.getElementById('chkTimeAbon');
        var chkBreakTime = document.getElementById('chkBreakTime');

        var dFrom = txtCanAbsFromBreakClient.GetValue().format2Time();
        var dTo = txtCanAbsToBreakClient.GetValue().format2Time();

        var iFrom = jsDate_retMinutesToTime(dFrom);
        var iTo = jsDate_retMinutesToTime(dTo);

        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak_tbCont1_";
        var BeginDay = cmbCanAbsFromBreakClient.GetValue();
        var FinishDay = cmbCanAbsToBreakClient.GetValue();

        iFrom = iFrom + ((BeginDay - 1) * 1440);
        iTo = iTo + ((FinishDay - 1) * 1440);

        var resMinutes = parseInt(iTo) - parseInt(iFrom);

        if (chkCanAbs.checked == true) {
            if (Break_CheckTime(txtChkCanAbsBreakClient, false)) { return false; }
            var dCanAbs = txtChkCanAbsBreakClient.GetValue().format2Time();
            var iCanAbs = jsDate_retMinutesToTime(dCanAbs);
            if (iCanAbs > resMinutes) {
                showErrorPopup("Error.LayerBreakTimeCanAbsTitle", "error", "Error.LayerBreakTimeCanAbsTitleDesc", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        if (chkDebAbs.checked) {
            if (Break_CheckTime(txtChkDebAbsBreakClient, false)) { return false; }
            var dDebAbs = txtChkDebAbsBreakClient.GetValue().format2Time();
            var iDebAbs = jsDate_retMinutesToTime(dDebAbs);
            if (iDebAbs > resMinutes) {
                showErrorPopup("Error.LayerBreakTimeDebAbsTitle", "error", "Error.LayerBreakTimeDebAbsTitleDesc", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        if (chkTimeAbon.checked) {
            if (Break_CheckTime(txtChkTimeAbonBreakClient, false)) { return false; }
        }

        if (chkBreakTime.checked) {
            if (Break_CheckTime(txtChkBreakTimeBreakClient, false)) { return false; }
        }

        return true;
    } catch (e) {
        showError("frmEditShiftBreak_Validate", e);
        return false;
    }
}

function frmEditShiftBreak_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak', false);
    } catch (e) { showError("frmEditShiftBreak_Close", e); }
}

function frmEditShiftBreak_Load() {
    try {
        //Carrega dels Camps

        frmEditShiftBreak_Show();
    } catch (e) { showError("frmEditShiftBreak_Load", e); }
}

function frmEditShiftBreak_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'false') { frmEditShiftBreak_Close(); return; }
        if (frmEditShiftBreak_Validate()) {
            var oMF = Break_FieldsToJSON();

            var IdLayer = document.getElementById('hdnBreakLayerID').value;
            if (IdLayer != "-1") {
                oTMDefinitions.updateTimeZone("roLTBreak", IdLayer, oMF);
            } else {
                oTMDefinitions.createTimeZone("roLTBreak", oMF);
            }
            hasChanges(true);
            frmEditShiftBreak_Close();
        }
    } catch (e) { showError("frmEditShiftBreak_Save", e); }
}

function frmEditShiftBreak_ShowNew() {
    try {
        var found = false;
        if (curShiftType == 'PerHours') {
            var currentoJSONLayers = oTMDefinitions.retrieveJSONLayers();

            if (currentoJSONLayers != null && currentoJSONLayers.length != 0) {
                for (var x = 0; x < currentoJSONLayers.length; x++) {
                    if (currentoJSONLayers[x].layer[0].value == "roLTMandatory") {
                        loadBreakBlanks();
                        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak', true);
                        found = true;
                        break;
                    }
                }
                if (found == false) {
                    showErrorPopup("Error.LayerBreakTimeBreakHoursError", "error", "Error.LayerBreakTimeBreakHoursErrorDesc", "", "Error.OK", "Error.OKDesc", "");
                }
            }
            else {
                showErrorPopup("Error.LayerBreakTimeBreakHoursError", "error", "Error.LayerBreakTimeBreakHoursErrorDesc", "", "Error.OK", "Error.OKDesc", "");
            }
        }
        else {
            loadBreakBlanks();
            showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak', true);
        }
    } catch (e) { showError("frmEditShiftBreak_ShowNew", e); }
}

function loadBreakBlanks() {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        //blank the fields
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak_tbCont1_";

        //blank fields
        loadGenericData("LAYERID", "hdnBreakLayerID", "-1", "X_NUMBER", "", oDis, false);
        loadGenericData("PARENTID", "hdnBreakParentID", "-1", "X_NUMBER", "", oDis, false);

        txtCanAbsFromBreakClient.SetValue(new Date('1900/01/01 00:00:00'));
        txtCanAbsToBreakClient.SetValue(new Date('1900/01/01 00:00:00'));
        txtNotificationUserBeforeClient.SetValue(new Date('1900/01/01 00:00:00'));
        txtNotificationUserAfterClient.SetValue(new Date('1900/01/01 00:00:00'));
        cmbCanAbsFromBreakClient.SetValue(1);
        cmbCanAbsToBreakClient.SetValue(1);

        //Filtres
        loadGenericData("CHKNOTIFICATIONUSER", "chkNotificationUser", "false", "X_CHECKBOX", "", oDis, false);
        loadGenericData("CHKNOTIFICATIONSUPERVISOR", "chkNotificationSupervisor", "false", "X_CHECKBOX", "", oDis, false);
        loadGenericData("CHKCANABS", "chkCanAbs", "false", "X_CHECKBOX", "", oDis, false);
        loadGenericData("CHKDEBABS", "chkDebAbs", "false", "X_CHECKBOX", "", oDis, false);

        txtChkCanAbsBreakClient.SetValue(new Date('1900/01/01 00:00:00'));
        txtChkDebAbsBreakClient.SetValue(new Date('1900/01/01 00:00:00'));

        loadGenericData("OPTDEBINC", "optDebInc", "false", "X_RADIO", "", oDis, false);
        loadGenericData("OPTDEBDESC", "optDebDesc", "false", "X_RADIO", "", oDis, false);

        loadGenericData("CHKTIMEABON", "chkTimeAbon", "false", "X_CHECKBOX", "", oDis, false);
        txtChkTimeAbonBreakClient.SetValue(new Date('1900/01/01 00:00:00'));

        loadGenericData("CHKBREAKTIME", "chkBreakTime", "false", "X_CHECKBOX", "", oDis, false);
        txtChkBreakTimeBreakClient.SetValue(new Date('1900/01/01 00:00:00'));
    } catch (e) { showError("loadFlexibleBlanks", e); }
}

function Break_FieldsToJSON() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditShiftBreak_tbCont1_";

        var TLId = document.getElementById('hdnBreakLayerID').value;
        var ParentID = document.getElementById('hdnBreakParentID').value;

        var MaxBreakTime = "";
        var MinBreakAction = "";
        var MinBreakTime = "";
        var NoPunchBreakTime = "";
        var NotificationForUser = "";
        var NotificationForSupervisor = "";
        var NotificationForUserBeforeTime = "";
        var NotificationForUserAfterTime = "";
        var RealBegin = "";
        var RealFinish = "";

        var Paid_1040_Id = "-1";
        var Paid_1040_Value = "";
        var Paid_1040_Action = "";
        var Paid_1040_Begin = "";
        var Paid_1040_Finish = "";
        var BreakHours = false;

        //tiempos permitidos
        var chkCanAbs = document.getElementById('chkCanAbs');
        if (chkCanAbs.checked) {
            MaxBreakTime = txtChkCanAbsBreakClient.GetValue().format2Time();
        }

        var chkDebAbs = document.getElementById('chkDebAbs');
        if (chkDebAbs.checked) {
            MinBreakTime = txtChkDebAbsBreakClient.GetValue().format2Time();
            if (document.getElementById('optDebInc').checked) {
                MinBreakAction = "CreateIncidence";
            } else {
                MinBreakAction = "SubstractAttendanceTime";
            }
        }

        //tiempos abonados
        var chkTimeAbon = document.getElementById('chkTimeAbon');
        if (chkTimeAbon.checked) {
            Paid_1040_Value = txtChkTimeAbonBreakClient.GetValue().format2Time();
            Paid_1040_Begin = txtCanAbsFromBreakClient.GetValue().format2Time();
            Paid_1040_Finish = txtCanAbsToBreakClient.GetValue().format2Time();
        }

        //penalizaciones
        var chkBreakTime = document.getElementById('chkBreakTime');
        if (chkBreakTime.checked) {
            NoPunchBreakTime = txtChkBreakTimeBreakClient.GetValue().format2Time();
        }

        //notificaciones
        var chkNotificationUser = document.getElementById('chkNotificationUser');
        if (chkNotificationUser.checked) {
            NotificationForUserBeforeTime = txtNotificationUserBeforeClient.GetValue().format2Time();
            NotificationForUserAfterTime = txtNotificationUserAfterClient.GetValue().format2Time();
            NotificationForUser = "true";
        }
        else {
            NotificationForUser = "false";
        }

        var chkNotificationSupervisor = document.getElementById('chkNotificationSupervisor');
        if (chkNotificationSupervisor.checked) {
            NotificationForSupervisor = "true";
        }
        else {
            NotificationForSupervisor = "false";
        }

        if (cmbCanAbsFromBreakClient.GetValue() == '3') {
            var currentoJSONLayers = oTMDefinitions.retrieveJSONLayers();

            if (currentoJSONLayers != null && currentoJSONLayers.length != 0) {
                for (var x = 0; x < currentoJSONLayers.length; x++) {
                    if (currentoJSONLayers[x].layer[0].value == "roLTMandatory") {
                        var horaInicioMandatorio = moment(new Date('1900/01/01 ' + currentoJSONLayers[x].layer[2].value + ':00'));
                        var horaInicioDescansoCalculada = horaInicioMandatorio.add(moment(txtCanAbsFromBreakClient.GetValue()).diff(moment(txtCanAbsFromBreakClient.GetValue()).startOf('day'), 'minutes'), 'minutes');
                        var horaFinMandatorio = moment(new Date('1900/01/01 ' + currentoJSONLayers[x].layer[2].value + ':00'));
                        var horaFinDescansoCalculada = horaFinMandatorio.add(moment(txtCanAbsToBreakClient.GetValue()).diff(moment(txtCanAbsToBreakClient.GetValue()).startOf('day'), 'minutes'), 'minutes');
                        RealBegin = horaInicioDescansoCalculada.toDate().format2Time();
                        RealFinish = horaFinDescansoCalculada.toDate().format2Time();

                        break;
                    }
                }
            }
        }

        var oAtts = [{ 'attname': 'rolt_tlid', 'value': document.getElementById('hdnBreakLayerID').value },
        { 'attname': 'rolt_begin', 'value': txtCanAbsFromBreakClient.GetValue().format2Time() },
        { 'attname': 'rolt_beginday', 'value': cmbCanAbsFromBreakClient.GetValue() },
        { 'attname': 'rolt_finish', 'value': txtCanAbsToBreakClient.GetValue().format2Time() },
        { 'attname': 'rolt_finishday', 'value': cmbCanAbsToBreakClient.GetValue() },
        { 'attname': 'rolt_datediffmin', 'value': 0 },
        { 'attname': 'rolt_maxbreaktime', 'value': MaxBreakTime },
        { 'attname': 'rolt_minbreakaction', 'value': MinBreakAction },
        { 'attname': 'rolt_minbreaktime', 'value': MinBreakTime },
        { 'attname': 'rolt_nopunchbreaktime', 'value': NoPunchBreakTime },
        { 'attname': 'rolt_NotificationForUser', 'value': NotificationForUser },
        { 'attname': 'rolt_NotificationForSupervisor', 'value': NotificationForSupervisor },
        { 'attname': 'rolt_NotificationForUserBeforeTime', 'value': NotificationForUserBeforeTime },
        { 'attname': 'rolt_NotificationForUserAfterTime', 'value': NotificationForUserAfterTime },
        { 'attname': 'rolt_realbegin', 'value': RealBegin },
        { 'attname': 'rolt_realfinish', 'value': RealFinish },
        { 'attname': 'rolt_paid_1040_parentid', 'value': document.getElementById('hdnBreakLayerID').value },
        { 'attname': 'rolt_paid_1040_id', 'value': -1 },
        { 'attname': 'rolt_paid_1040_value', 'value': Paid_1040_Value },
        { 'attname': 'rolt_paid_1040_action', 'value': Paid_1040_Action },
        { 'attname': 'rolt_paid_1040_begin', 'value': Paid_1040_Begin },
        { 'attname': 'rolt_paid_1040_beginday', 'value': cmbCanAbsFromBreakClient.GetValue() },
        { 'attname': 'rolt_paid_1040_finish', 'value': Paid_1040_Finish },
        { 'attname': 'rolt_paid_1040_finishday', 'value': cmbCanAbsToBreakClient.GetValue() },
        { 'attname': 'rolt_breakhours', 'value': BreakHours }

        ];

        return oAtts;
    } catch (e) {
        showError("Break_FieldsToJSON", e);
        return null;
    }
}

function Break_CheckTime(obj, permitBlank) {
    try {
        if (permitBlank == false) {
            if (obj.GetValue() == "") {
                showErrorPopup("Error.frmBreak." + obj.id + "Title", "ERROR", "Error.frmBreak." + obj.id + "Desc", "", "Error.frmBreak.OK", "Error.frmBreak.OKDesc", "");
                return true;
            }
        }

        return false;
    } catch (e) { showError("Break_CheckTime", e); }
}