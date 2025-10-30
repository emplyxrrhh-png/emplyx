function save() {
    frmNewPeriod_Save();
    return false;
}
function close() {
    frmNewPeriod_Close();
    return false;
}

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

function frmNewPeriod_Show(arrValues) {
    try {
        //load fields
        var n;
        var objPrFrm = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_";
        var objPrOpt1 = "opTypePeriodNormal_";
        var objPrOpt2 = "opTypePeriodEspecific_";
        var optSel = "0";
        var optDaily = true;

        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        loadNewPeriodBlanks();

        for (n = 0; n < arrValues.length; n++) {
            switch (arrValues[n].attname.toLowerCase()) {
                case "jsgridatt_idaccessperiod":
                    loadGenericData("IDACCESSPERIOD", "hdnAddPeriodIDZone", arrValues[n].value, "X_NUMBER", "", oDis, false);
                    break;

                case "jsgridatt_dayofweek":
                    cmbWeekDayClient.SetSelectedItem(cmbWeekDayClient.FindItemByValue(arrValues[n].value));
                    cmbWeekDayClient.SetEnabled((oDis == 'false' ? true : false));
                    break;

                case "jsgridatt_begintime":
                    txtNormalHourBeginClient.SetDate(BuildDateFromStringTime(arrValues[n].value));
                    txtNormalHourBeginClient.SetEnabled((oDis == 'false' ? true : false));
                    txtSpecificHourBeginClient.SetDate(BuildDateFromStringTime(arrValues[n].value));
                    txtSpecificHourBeginClient.SetEnabled((oDis == 'false' ? true : false));
                    break;

                case "jsgridatt_endtime":
                    txtNormalHourEndClient.SetDate(BuildDateFromStringTime(arrValues[n].value));
                    txtNormalHourEndClient.SetEnabled((oDis == 'false' ? true : false));
                    txtSpecificHourEndClient.SetDate(BuildDateFromStringTime(arrValues[n].value));
                    txtSpecificHourEndClient.SetEnabled((oDis == 'false' ? true : false));
                    break;

                case "jsgridatt_day":
                    if (arrValues[n].value != "") {
                        optDaily = false;
                        optSel = "1";
                        loadGenericData("DAY", objPrFrm + objPrOpt2 + "txtDay", arrValues[n].value, "X_NUMBER", "", oDis, false);
                    }
                    break;

                case "jsgridatt_month":
                    cmbMonthsClient.SetSelectedItem(cmbMonthsClient.FindItemByValue(arrValues[n].value));
                    cmbMonthsClient.SetEnabled((oDis == 'false' ? true : false));
                    break;
            }
        }

        if (optDaily) {
            loadGenericData("OPTIONSEL", objPrFrm + "opTypePeriodNormal," + objPrFrm + "opTypePeriodEspecific", "0", "X_OPTIONGROUP", "", oDis, false);

            txtSpecificHourBeginClient.SetDate(BuildDateFromStringTime("00:00"));
            txtSpecificHourBeginClient.SetEnabled((oDis == 'false' ? true : false));

            txtSpecificHourEndClient.SetDate(BuildDateFromStringTime("00:00"));
            txtSpecificHourEndClient.SetEnabled((oDis == 'false' ? true : false));

            chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_opTypePeriodNormal,ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_opTypePeriodEspecific', 'undefined');
        }
        else {
            loadGenericData("OPTIONSEL", objPrFrm + "opTypePeriodNormal," + objPrFrm + "opTypePeriodEspecific", "1", "X_OPTIONGROUP", "", oDis, false);

            var txtDay = document.getElementById(objPrFrm + objPrOpt2 + "txtDay").value;

            if (txtDay != 0) {
                document.getElementById('optDay').checked = true;
                document.getElementById('optEvent').checked = false;
            }
            else {
                document.getElementById('optDay').checked = false;
                document.getElementById('optEvent').checked = true;
            }

            txtNormalHourBeginClient.SetDate(BuildDateFromStringTime("00:00"));
            txtNormalHourBeginClient.SetEnabled((oDis == 'false' ? true : false));

            txtNormalHourEndClient.SetDate(BuildDateFromStringTime("00:00"));
            txtNormalHourEndClient.SetEnabled((oDis == 'false' ? true : false));

            var oTimeBegin = txtSpecificHourBeginClient.GetDate();
            var oTimeEnd = txtSpecificHourEndClient.GetDate();

            if (oTimeBegin.format2Time() != "00:00" && oTimeEnd.format2Time() != "00:00") {
                document.getElementById('optGrantAccess').checked = true;
                document.getElementById('optDeniedAccess').checked = false;
            }
            else {
                if (txtDay == 0 && (oTimeBegin.format2Time() != "00:00" || oTimeEnd.format2Time() != "00:00")) {
                    document.getElementById('optGrantAccess').checked = true;
                    document.getElementById('optDeniedAccess').checked = false;
                }
                else {
                    document.getElementById('optGrantAccess').checked = false;
                    document.getElementById('optDeniedAccess').checked = true;

                    txtSpecificHourBeginClient.SetDate(BuildDateFromStringTime("00:00"));
                    txtSpecificHourBeginClient.SetEnabled((oDis == 'false' ? true : false));

                    txtSpecificHourEndClient.SetDate(BuildDateFromStringTime("00:00"));
                    txtSpecificHourEndClient.SetEnabled((oDis == 'false' ? true : false));
                }
            }
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_opTypePeriodEspecific_txtDay").removeClass("x-item-disabled");
        }

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1', true);

        chgOPCItems(optSel, 'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_opTypePeriodNormal,ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_opTypePeriodEspecific', 'undefined');
    } catch (e) { showError("frmNewPeriod_Show", e); }
}

//Validacio del formulari
function frmNewPeriod_Validate() {
    try {
        var objPrFrm = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_";
        var objPrOpt1 = "opTypePeriodNormal_";
        var objPrOpt2 = "opTypePeriodEspecific_";

        if (document.getElementById(objPrFrm + objPrOpt1 + "rButton").checked) {
            if (cmbWeekDayClient.GetSelectedItem().value == "0") {
                showErrorPopup("Error.frmNewPeriod.cmbWeekDayTitle", "ERROR", "Error.frmNewPeriod.cmbWeekDayTitleDesc", "Error.frmNewPeriod.OK", "Error.frmNewPeriod.OKDesc", "");
                return false;
            }

            if (txtNormalHourBeginClient.GetDate().getTime() > txtNormalHourEndClient.GetDate().getTime()) {
                showErrorPopup("Error.frmNewPeriod.TimePeriodIncorrectTitle", "ERROR", "Error.frmNewPeriod.TimePeriodIncorrectDesc", "Error.frmNewPeriod.OK", "Error.frmNewPeriod.OKDesc", "");
                return false;
            }
        }
        else {
            if (document.getElementById('optDay').checked == true) {
                var txtDay = document.getElementById(objPrFrm + objPrOpt2 + "txtDay").value;

                if (isNaN(txtDay)) {
                    showErrorPopup("Error.frmNewPeriod.txtDayTitle", "ERROR", "Error.frmNewPeriod.txtDayTitleDesc", "Error.frmNewPeriod.OK", "Error.frmNewPeriod.OKDesc", "");
                    return false;
                }

                if (txtDay < 1 || txtDay > 31) {
                    showErrorPopup("Error.frmNewPeriod.txtDayTitle", "ERROR", "Error.frmNewPeriod.txtDayTitleDesc", "Error.frmNewPeriod.OK", "Error.frmNewPeriod.OKDesc", "");
                    return false;
                }

                if (cmbMonthsClient.GetSelectedItem().value == "0") {
                    showErrorPopup("Error.frmNewPeriod.cmbMonthTitle", "ERROR", "Error.frmNewPeriod.cmbMonthTitleDesc", "Error.frmNewPeriod.OK", "Error.frmNewPeriod.OKDesc", "");
                    return false;
                }

                if (document.getElementById('optGrantAccess').checked == true) {
                    if (txtSpecificHourBeginClient.GetDate().getTime() > txtSpecificHourEndClient.GetDate().getTime()) {
                        showErrorPopup("Error.frmNewPeriod.TimePeriodIncorrectTitle", "ERROR", "Error.frmNewPeriod.TimePeriodIncorrectDesc", "Error.frmNewPeriod.OK", "Error.frmNewPeriod.OKDesc", "");
                        return false;
                    }
                }
            }
        }

        return true;
    } catch (e) {
        showError("frmNewPeriod_Validate", e);
        return false;
    }
}

function frmNewPeriod_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1', false);
    } catch (e) { showError("frmNewPeriod_Close", e); }
}

function frmNewPeriod_Load() {
    try {
        //Carrega dels Camps
        frmNewPeriod_Show();
    } catch (e) { showError("frmNewPeriod_Load", e); }
}

//Grabació del formulari en el roTimeLine
function frmNewPeriod_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "false") { frmNewPeriod_Close(); return; }
        if (frmNewPeriod_Validate()) {
            var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_";
            var oMF = AddNewPeriod_FieldsToJSON();

            var rowID = document.getElementById('hdnAddPeriodIDRow').value;
            updateAddPeriodRow(rowID, oMF);

            hasChanges(true);
            frmNewPeriod_Close();
        }
    } catch (e) { showError("frmNewPeriod_Save", e); }
}

function frmNewPeriod_ShowNew() {
    try {
        loadNewPeriodBlanks();

        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1', true);
    } catch (e) { showError("frmNewPeriod_ShowNew", e); }
}

function loadNewPeriodBlanks() {
    try {
        var objPrFrm = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_";
        var objPrOpt1 = "opTypePeriodNormal_";
        var objPrOpt2 = "opTypePeriodEspecific_";
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        loadGenericData("IDACCESSPERIOD", "hdnAddPeriodIDZone", "", "X_NUMBER", "", oDis, false);

        cmbWeekDayClient.SetSelectedItem(cmbWeekDayClient.FindItemByValue("0"));
        cmbWeekDayClient.SetEnabled((oDis == 'false' ? true : false));

        txtNormalHourBeginClient.SetDate(BuildDateFromStringTime("00:00"))
        txtNormalHourBeginClient.SetEnabled((oDis == 'false' ? true : false));

        txtSpecificHourBeginClient.SetDate(BuildDateFromStringTime("00:00"));
        txtSpecificHourBeginClient.SetEnabled((oDis == 'false' ? true : false));

        txtNormalHourEndClient.SetDate(BuildDateFromStringTime("00:00"));
        txtNormalHourEndClient.SetEnabled((oDis == 'false' ? true : false));

        txtSpecificHourEndClient.SetDate(BuildDateFromStringTime("00:00"));
        txtSpecificHourEndClient.SetEnabled((oDis == 'false' ? true : false));
        loadGenericData("DAY", objPrFrm + objPrOpt2 + "txtDay", "1", "X_NUMBER", "", oDis, false);

        document.getElementById('optDay').checked = true;
        document.getElementById('optEvent').checked = false;

        document.getElementById('optGrantAccess').checked = true;
        document.getElementById('optDeniedAccess').checked = false;

        cmbMonthsClient.SetSelectedItem(cmbMonthsClient.FindItemByValue("0"));
        cmbMonthsClient.SetEnabled((oDis == 'false' ? true : false));

        loadGenericData("OPTIONSEL", objPrFrm + "opTypePeriodNormal," + objPrFrm + "opTypePeriodEspecific", "0", "X_OPTIONGROUP", "", oDis, false);
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_opTypePeriodNormal');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_opTypePeriodEspecific');
        linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_opTypePeriodNormal,ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_opTypePeriodEspecific');
    } catch (e) { showError("loadNewPeriodBlanks", e); }
}

//Carrega de tots els camps en un objecte JSON
function AddNewPeriod_FieldsToJSON() {
    try {
        var objPrFrm = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmNewPeriod1_";
        var objPrOpt1 = "opTypePeriodNormal_";
        var objPrOpt2 = "opTypePeriodEspecific_";

        var IDAccessPeriod = document.getElementById("hdnAddPeriodIDZone").value;
        var DayofWeek;
        var WeekDayName;
        var BeginTime;
        var EndTime;
        var Day;
        var Month;

        if (document.getElementById(objPrFrm + objPrOpt1 + "rButton").checked) {
            DayofWeek = cmbWeekDayClient.GetSelectedItem().value;
            BeginTime = txtNormalHourBeginClient.GetDate().format2Time();
            EndTime = txtNormalHourEndClient.GetDate().format2Time();
            Day = "";
            Month = "";
        }
        else {
            DayofWeek = "";
            BeginTime = txtSpecificHourBeginClient.GetDate().format2Time();
            EndTime = txtSpecificHourEndClient.GetDate().format2Time();

            if (document.getElementById('optEvent').checked == true) {
                Month = "0";
                Day = "0";
            }
            else {
                Month = cmbMonthsClient.GetSelectedItem().value;
                Day = document.getElementById(objPrFrm + objPrOpt2 + "txtDay").value;
            }

            if (!document.getElementById('optGrantAccess').checked) {
                BeginTime = "00:00";
                EndTime = "00:00";
            }
        }

        var oAtts = [{ 'attname': 'idaccessperiod', 'value': IDAccessPeriod },
        { 'attname': 'description', 'value': '' },
        { 'attname': 'dayofweek', 'value': DayofWeek },
        { 'attname': 'begintime', 'value': BeginTime },
        { 'attname': 'endtime', 'value': EndTime },
        { 'attname': 'day', 'value': Day },
        { 'attname': 'month', 'value': Month }
        ];

        return oAtts;
    } catch (e) {
        showError("AddNewPeriod_FieldsToJSON", e);
        return null;
    }
}