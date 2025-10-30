var actualScheduleRuleId = -1;
var bIsReadOnly = false;
var editControlPrefix = 'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditScheduleRules';
var dxcollection = [];
var contador = 1;

function initScheduleRuleControl(prefix, isEditable, isReadOnly) {
    editControlPrefix = prefix + '_frmEditScheduleRules';

    bIsReadOnly = typeof isReadOnly != 'undefined' ? isReadOnly : false;

    document.getElementById(editControlPrefix + '_ASPxScheduleRulesCallbackPanelContenido_hdnModeEdit').value = isEditable;
}

function enableTC() {
    if (ckTelecommuteYes_client.GetValue() == false) {
        txtCanTelecommuteFromClient.SetDate(null);
        txtCanTelecommuteToClient.SetDate(null);
        txtTelecommutingMaxOptional_Client.SetValue("");
        cmbWeekOrMonthClient.SetEnabled(false);
        cmbDaysOrPercentClient.SetEnabled(false);
        txtCanTelecommuteFromClient.SetEnabled(false);
        txtCanTelecommuteToClient.SetEnabled(false);
        txtTelecommutingMaxOptional_Client.SetEnabled(false);
        unsetTelecommutingPattern();
    }
    else {
        txtCanTelecommuteFromClient.SetEnabled(true);
        cmbWeekOrMonthClient.SetEnabled(true);
        cmbDaysOrPercentClient.SetEnabled(true);
        txtCanTelecommuteToClient.SetEnabled(true);
        txtTelecommutingMaxOptional_Client.SetEnabled(true);
        setTelecommutingPattern();
    }
}

function frmEditScheduleRules_Close() {
    try {
        //show te form
        showWUF(editControlPrefix, false);
    } catch (e) { showError("frmEditScheduleRules_Close", e); }
}

function ckFixedValuePeriod_client_CheckedChanged(s, e) {
    if (s.GetValue() == true) {
        cmbUserFieldPeriodClient.SetEnabled(false);
        txtMaxExpectedHoursInPeriodRepeat_client.SetEnabled(true);
    } else {
        cmbUserFieldPeriodClient.SetEnabled(true);
        txtMaxExpectedHoursInPeriodRepeat_client.SetEnabled(false);
    }
}

function ckUserFieldPeriod_client_CheckedChanged(s, e) {
    if (s.GetValue() == true) {
        cmbUserFieldPeriodClient.SetEnabled(true);
        txtMaxExpectedHoursInPeriodRepeat_client.SetEnabled(false);
    } else {
        cmbUserFieldPeriodClient.SetEnabled(false);
        txtMaxExpectedHoursInPeriodRepeat_client.SetEnabled(true);
    }
}

function frmEditScheduleRules_Validate() {
    try {
        var objPrTab = editControlPrefix + "_ASPxRequestValidationCallbackPanelContenido_";

        if (ASPxClientEdit.ValidateEditorsInContainer(ASPxScheduleRulesCallbackPanelContenidoClient.GetMainElement())) {
            var oParameters = {};
            oParameters.aTab = 1;
            oParameters.ID = actualScheduleRuleId;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVELABAGREESCHEDULERULE";
            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxScheduleRulesCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            if (typeof showErrorPopup == 'function') showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
            else {
                DevExpress.ui.dialog.alert(Globalize.formatMessage('roDataNeeded'), Globalize.formatMessage('roJsError'));
            }
        }
        return true;
    } catch (e) {
        showError("frmEditScheduleRules_Validate", e);
        return false;
    }
}

function frmEditScheduleRules_Save() {
    try {
        ///if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'false') { frmEditScheduleRules_Close(); return; }
        if (document.getElementById(editControlPrefix + '_ASPxScheduleRulesCallbackPanelContenido_hdnModeEdit').value != 'false') { frmEditScheduleRules_Close(); return; }

        frmEditScheduleRules_Validate();
    } catch (e) { showError("frmEditScheduleRules_Save", e); }
}

// Recarrega dels combos a traves de json

function enableTCMain() {
    try {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_chkButton').checked == false) {
            ckTelecommuteYes_client.SetEnabled(false);
            ckTelecommuteNo_client.SetEnabled(false);
        }
        else {
            ckTelecommuteYes_client.SetEnabled(true);
            ckTelecommuteNo_client.SetEnabled(true);
        }
        enableTC();
    } catch (e) {
    }
}

function enablePeriod(s, e) {
    var divEntre = document.getElementById('divEntre');
    var divHasta = document.getElementById('divHasta');
    var dateFrom = document.getElementById('divFrom');
    var dateTo = document.getElementById('divTo');
    var divPeriod = document.getElementById('divPeriod');
    var ckPeriod = document.getElementById('divAlways');

    if (s.GetValue() == true) {
        divEntre.style.display = "none";
        divHasta.style.display = "";
        dateFrom.style.display = "";
        dateTo.style.display = "";
        ckPeriod.style.display = "";
    }
    else {
        divEntre.style.display = "none";
        divHasta.style.display = "none";
        dateFrom.style.display = "none";
        dateTo.style.display = "none";
        ckPeriod.style.display = "";
    }
}

function enablePeriodSequence(s, e) {
    var divEntre = document.getElementById('divEntreSequence');
    var divHasta = document.getElementById('divHastaSequence');
    var dateFrom = document.getElementById('divFromSequence');
    var dateTo = document.getElementById('divToSequence');
    var divPeriod = document.getElementById('divPeriodSequence');
    var ckPeriod = document.getElementById('divAlwaysSequence');

    if (s.GetValue() == true) {
        divEntre.style.display = "none";
        divHasta.style.display = "";
        dateFrom.style.display = "";
        dateTo.style.display = "";
        ckPeriod.style.display = "";
    }
    else {
        divEntre.style.display = "none";
        divHasta.style.display = "none";
        dateFrom.style.display = "none";
        dateTo.style.display = "none";
        ckPeriod.style.display = "";
    }
}

function ASPxScheduleRulesCallbackPanelContenido_EndCallBack(s, e) {
    var divEntre = document.getElementById('divEntre');
    var divHasta = document.getElementById('divHasta');
    var dateFrom = document.getElementById('divFrom');
    var dateTo = document.getElementById('divTo');
    var divPeriod = document.getElementById('divPeriod');
    var ckPeriod = document.getElementById('divAlways');

    if (s.cpScope != "Selection") {
        if (txtDateFromClient.GetDate() != null) {
            divEntre.style.display = "none";
            divHasta.style.display = "";
            dateFrom.style.display = "";
            dateTo.style.display = "";
            divPeriod.style.display = "";
            ckPeriod.style.display = "";
        }
        else {
            divEntre.style.display = "none";
            divHasta.style.display = "none";
            dateFrom.style.display = "none";
            dateTo.style.display = "none";
            divPeriod.style.display = "";
            ckPeriod.style.display = "";
        }
    }
    else {
        divEntre.style.display = "";
        divHasta.style.display = "";
        dateFrom.style.display = "";
        dateTo.style.display = "";
        divPeriod.style.display = "none";
        ckPeriod.style.display = "none";
    }

    var divEntreSeq = document.getElementById('divEntreSequence');
    var divHastaSeq = document.getElementById('divHastaSequence');
    var dateFromSeq = document.getElementById('divFromSequence');
    var dateToSeq = document.getElementById('divToSequence');
    var divPeriodSeq = document.getElementById('divPeriodSequence');
    var ckPeriodSeq = document.getElementById('divAlwaysSequence');

    if (s.cpScope != "Selection") {
        if (txtDateSequenceFromClient.GetDate() != null) {
            divEntreSeq.style.display = "none";
            divHastaSeq.style.display = "";
            dateFromSeq.style.display = "";
            dateToSeq.style.display = "";
            divPeriodSeq.style.display = "none";
            ckPeriodSeq.style.display = "";
        }
        else {
            divEntreSeq.style.display = "none";
            divHastaSeq.style.display = "none";
            dateFromSeq.style.display = "none";
            dateToSeq.style.display = "none";
            divPeriodSeq.style.display = "";
            ckPeriodSeq.style.display = "";
        }
    }
    else {
        divEntreSeq.style.display = "";
        divHastaSeq.style.display = "";
        dateFromSeq.style.display = "";
        dateToSeq.style.display = "";
        divPeriodSeq.style.display = "none";
        ckPeriodSeq.style.display = "none";
    }

    if (typeof showLoadingGrid == 'function') showLoadingGrid(false);

    switch (s.cpActionRO) {
        case "GETLABAGREERESCHEDULERULE":
            showWUF(editControlPrefix, true);

            document.getElementById('tdSaveReaOnlyScheduleRule').style.display = bIsReadOnly ? 'none' : '';

            for (var i = 0; i < dxcollection.length; i++) {
                dxcollection[i].SetEnabled(bIsReadOnly == false);
            }

            $(".divRowDescription").each(function (i, val) {
                $(this).attr('style', 'display:flex;text-align: left;');
            });
            $(".componentForm").each(function (i, val) {
                $(this).attr('style', 'display:flex;text-align: left;');
            });

            ASPxClientEdit.ValidateGroup(null, true);
            loadScheduleRuleConfigurationDiv();
            break;
        case "SAVELABAGREESCHEDULERULE":
            if (s.cpResultRO == "KO") {
                loadScheduleRuleConfigurationDiv();
                if (typeof showErrorPopup == 'function') showErrorPopup("Error.Title", "error", "", s.cpErrorRO.ErrorText, "Error.OK", "Error.OKDesc", "");
                else {
                    DevExpress.ui.dialog.alert(s.cpErrorRO.ErrorText, Globalize.formatMessage('roJsError'));
                }
            } else {
                frmEditScheduleRules_Close();
                if (typeof GridLabAgreeScheduleRulesClient != 'undefined') {
                    hasChanges(true);
                    GridLabAgreeScheduleRulesClient.PerformCallback("REFRESH");
                }

                if (typeof GridContractScheduleRulesClient != 'undefined') {
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteScheduleRules_chkButton').checked = true;
                    GridContractScheduleRulesClient.PerformCallback("REFRESH");
                }
            }
            break;
        case "LOADRULECONFIGURATION":
            ASPxClientEdit.ValidateGroup(null, true);
            break;
    }
}

function loadScheduleRuleConfigurationDiv() {
    if (cmbScheduleRuleType_Client.GetSelectedItem() != null) {
        var typeDiv = parseInt(cmbScheduleRuleType_Client.GetSelectedItem().value, 10);

        for (var i = 0; i <= 15; i++) {
            if (i == typeDiv) {
                document.getElementById('scheduleRuleType_' + i).style.display = '';
            } else {
                document.getElementById('scheduleRuleType_' + i).style.display = 'none';
            }
        }
    }

    cmbOneShiftOneDayWhenChanged(cmbOneShiftOneDayWhenChangedClient);
}

function loadSpecificScheduleRuleConfigurationDiv() {
    var oParameters = {};
    oParameters.aTab = 1;
    oParameters.ID = actualScheduleRuleId;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "LOADRULECONFIGURATION";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxScheduleRulesCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cmbOneShiftOneDayWhenChanged(s, e) {
    if (s.GetSelectedItem() != null) {
        var typeDiv = parseInt(s.GetSelectedItem().value, 10);
        document.getElementById('cmbOneShiftOneDayWhenDay').style.display = 'none';
        document.getElementById('cmbOneShiftOneDayWhenWeek').style.display = 'none';
        document.getElementById('cmbOneShiftOneDayWhenYear').style.display = 'none';

        if (typeDiv == 1) document.getElementById('cmbOneShiftOneDayWhenDay').style.display = '';
        if (typeDiv == 2) document.getElementById('cmbOneShiftOneDayWhenWeek').style.display = '';
        if (typeDiv == 3) document.getElementById('cmbOneShiftOneDayWhenYear').style.display = '';
    }
}

function typeChangedSequence(type) {
    var cmbShift = document.getElementById('divcmbShiftSequence');
    var cmbType = document.getElementById('divcmbTypeSequence');
    if (type == 0) {
        cmbShift.style.display = "";
        cmbType.style.display = "none";
    }
    else {
        cmbShift.style.display = "none";
        cmbType.style.display = "";
    }
}
function shiftChangedSequence(shift) {
}
function addShift() {
    if (cmbTypeSequenceClient.GetSelectedItem().value == 0) {
        tbMinMaxShiftsSequenceClient.AddToken((tbMinMaxShiftsSequenceClient.tokens.count() + 1) + "-" + cmbShiftSequenceClient.GetSelectedItem().text, cmbShiftSequenceClient.GetSelectedItem().value);
    }
    else {
        tbMinMaxShiftsSequenceClient.AddToken((tbMinMaxShiftsSequenceClient.tokens.count() + 1) + "-" + '*' + txtTypeSequenceClient.GetValue() + '*');
    }
}

function deleteTokens() {
    tbMinMaxShiftsSequenceClient.ClearTokenCollection();
}
function periodChanged(period) {
    try {
        var divEntre = document.getElementById('divEntre');
        var divHasta = document.getElementById('divHasta');
        var dateFrom = document.getElementById('divFrom');
        var dateTo = document.getElementById('divTo');
        var divPeriod = document.getElementById('divPeriod');
        var ckPeriod = document.getElementById('divAlways');

        if (period == 4) {
            divEntre.style.display = "";
            divHasta.style.display = "";
            dateFrom.style.display = "";
            dateTo.style.display = "";
            divPeriod.style.display = "none";
            ckPeriod.style.display = "none";
        } else {
            divEntre.style.display = "none";
            divHasta.style.display = "none";
            dateFrom.style.display = "none";
            dateTo.style.display = "none";
            divPeriod.style.display = "";
            ckPeriod.style.display = "";

            clAlwaysClient.SetChecked(false);
            //document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditScheduleRules_ASPxScheduleRulesCallbackPanelContenido_ckAlways_S_D').checked = false;
        }
    } catch (e) { showError("changeValueType", e); }
}

function periodChangedSequence(period) {
    try {
        var divEntre = document.getElementById('divEntreSequence');
        var divHasta = document.getElementById('divHastaSequence');
        var dateFrom = document.getElementById('divFromSequence');
        var dateTo = document.getElementById('divToSequence');
        var divPeriod = document.getElementById('divPeriodSequence');
        var ckPeriod = document.getElementById('divAlwaysSequence');

        if (period == 4) {
            divEntre.style.display = "";
            divHasta.style.display = "";
            dateFrom.style.display = "";
            dateTo.style.display = "";
            divPeriod.style.display = "none";
            ckPeriod.style.display = "none";
        } else {
            divEntre.style.display = "none";
            divHasta.style.display = "none";
            dateFrom.style.display = "none";
            dateTo.style.display = "none";
            divPeriod.style.display = "";
            ckPeriod.style.display = "";

            clAlwaysClient.SetChecked(false);
            //document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditScheduleRules_ASPxScheduleRulesCallbackPanelContenido_ckAlways_S_D').checked = false;
        }
    } catch (e) { showError("changeValueType", e); }
}