function BuildDateFromStringTime(strTime) {
    var arrHour = strTime.toString().split(":");
    if (arrHour.length != 2) {
        return null;
    }
    else {
        var iHour = parseInt(arrHour[0]);
        var iMinute = parseInt(arrHour[1]);
    }
    return new Date(1900, 1, 1, iHour, iMinute);
}

function frmAddPeriod_Show(arrValues) {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
        loadAddPeriodBlanks();

        for (var n = 0; n < arrValues.length; n++) {
            switch (arrValues[n].attname.toLowerCase()) {
                case "jsgridatt_idzone":
                    loadGenericData("IDZONE", "hdnAddPeriodIDZone", arrValues[n].value, "X_NUMBER", "", oDis, false);
                    break;
                case "jsgridatt_weekday":
                    cmbPeriodWeekDayClient.SetSelectedItem(cmbPeriodWeekDayClient.FindItemByValue(arrValues[n].value));
                    cmbPeriodWeekDayClient.SetEnabled((oDis == 'false' ? true : false));

                    break;
                case "jsgridatt_begin":
                    txtPeriodFromTimeClient.SetDate(BuildDateFromStringTime(arrValues[n].value));
                    txtPeriodFromTimeClient.SetEnabled((oDis == 'false' ? true : false));

                    break;
                case "jsgridatt_end":
                    txtPeriodToTimeClient.SetDate(BuildDateFromStringTime(arrValues[n].value));
                    txtPeriodToTimeClient.SetEnabled((oDis == 'false' ? true : false));
                    break;
            }
        }

        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddPeriod1', true);
    }
    catch (e) { showError("frmAddPeriod_Show", e); }
}

function frmAddPeriod_Validate() {
    try {
        if (cmbPeriodWeekDayClient.GetSelectedItem().value == "0") {
            showErrorPopup("Error.frmAddPeriod.cmbPeriodWeekDayTitle", "ERROR", "Error.frmAddPeriod.cmbPeriodWeekDayTitleDesc", "Error.frmAddPeriod.OK", "Error.frmAddPeriod.OKDesc", "");
            return false;
        }

        if (txtPeriodFromTimeClient.GetDate().getTime() > txtPeriodToTimeClient.GetDate().getTime()) {
            showErrorPopup("Error.frmAddPeriod.errIncorrectPeriodsTitle", "ERROR", "Error.frmAddPeriod.errIncorrectPeriodsTitleDesc", "Error.frmAddPeriod.OK", "Error.frmAddPeriod.OKDesc", "");
            return false;
        }

        return true;
    }
    catch (e) {
        showError("frmAddPeriod_Validate", e);
        return false;
    }
}

function frmAddPeriod_Close() {
    try {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddPeriod1', false);
    }
    catch (e) { showError("frmAddPeriod_Close", e); }
}

function frmAddPeriod_Load() {
    try {
        frmAddPeriod_Show();
    }
    catch (e) { showError("frmAddPeriod_Load", e); }
}

function frmAddPeriod_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "false") { frmAddPeriod_Close(); return; }
        if (frmAddPeriod_Validate()) {
            var oMF = AddPeriod_FieldsToJSON();
            var rowID = document.getElementById('hdnAddPeriodIDRow').value;
            updateAddPeriodRow(rowID, oMF);

            hasChanges(true);
            frmAddPeriod_Close();
        }
    }
    catch (e) { showError("frmAddPeriod_Save", e); }
}

function frmAddPeriod_ShowNew() {
    try {
        loadAddPeriodBlanks();
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddPeriod1', true);
    }
    catch (e) { showError("frmAddPeriod_ShowNew", e); }
}

function loadAddPeriodBlanks() {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        loadGenericData("IDZONE", "hdnAddPeriodIDZone", "", "X_NUMBER", "", oDis, false);

        cmbPeriodWeekDayClient.SetSelectedItem(cmbPeriodWeekDayClient.FindItemByValue(0));
        cmbPeriodWeekDayClient.SetEnabled((oDis == 'false' ? true : false));

        txtPeriodFromTimeClient.SetDate(BuildDateFromStringTime("00:00"))
        txtPeriodFromTimeClient.SetEnabled((oDis == 'false' ? true : false));

        txtPeriodToTimeClient.SetDate(BuildDateFromStringTime("00:00"))
        txtPeriodToTimeClient.SetEnabled((oDis == 'false' ? true : false));
    }
    catch (e) { showError("loadAddPeriodBlanks", e); }
}

function AddPeriod_FieldsToJSON() {
    try {
        var IDZone = document.getElementById("hdnAddPeriodIDZone").value;
        var Begin = txtPeriodFromTimeClient.GetDate().format2Time();
        var End = txtPeriodToTimeClient.GetDate().format2Time();
        var WeekDay = cmbPeriodWeekDayClient.GetSelectedItem().value;
        var WeekDayName = cmbPeriodWeekDayClient.GetSelectedItem().text;

        var oAtts = [{ 'attname': 'idzone', 'value': IDZone },
        { 'attname': 'weekdayname', 'value': WeekDayName },
        { 'attname': 'weekday', 'value': WeekDay },
        { 'attname': 'begin', 'value': Begin },
        { 'attname': 'end', 'value': End }
        ];

        return oAtts;
    }
    catch (e) {
        showError("AddPeriod_FieldsToJSON", e);
        return null;
    }
}