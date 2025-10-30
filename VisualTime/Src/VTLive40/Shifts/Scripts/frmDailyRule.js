let conditionsNumber = 1;
let actionsNumber = 1;

let conditionsEnabled = [true, false, false];
let actionsEnabled = [true, false, false];

let conditionTabActive = 0;
let actionTabActive = 0;

let actualID = -1;
let actualPriority = -1;
let bIsNew = true;

//1 standard shift rule, 2 single rulesgroup admin
let ruleSource = 1;

function frmDailyRule_Show(dailyRule) {
    try {

        if (typeof stdWorkingMode !== 'undefined' && stdWorkingMode == 'advRule') ruleSource = 2;

        PreparePage();

        initializeDailyRule_Blank();
        if (ruleSource == 1) actualPriority = dailyRules.length + 1;
        else actualPriority = 1;

        bIsNew = true;
        actualID = -1;

        if (typeof dailyRule != 'undefined' && dailyRule != null) {
            bIsNew = false;
            loadDailyRule(dailyRule);
        }

        //show te form
        if (ruleSource == 1) showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDailyRule1', true);
        else showWUF('frmDailyRule1', true);

    } catch (e) { showError("frmDailyRule_Show", e); }
}

function fillOldShiftsTB(actualShift, actual) {
    let totalShifts = parseInt(hdnSourceShiftListClient.Get("hdnShiftNumber"), 10);
    let actualValues = actual.split(",");

    tbOldShiftInListClient.ClearItems();
    for (let i = 0; i < totalShifts; i++) {
        let sConf = hdnSourceShiftListClient.Get("sInfo_" + i).split("@@");

        let tmpIndex = parseInt(sConf[0], 10);
        let tmpObsolete = parseInt(sConf[1], 10);

        if (tmpIndex != actualShift && (tmpObsolete == 0 || (tmpObsolete == 1 && actualValues.includes("" + tmpIndex) )) ) tbOldShiftInListClient.AddItem(sConf[2], tmpIndex);

    }
}

function PreparePage() {
    $("#divConditionClick").click(function () {
        //getting the next element
        $content = $("#collapsableCondition");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });

    $("#divActionClick").click(function () {
        //getting the next element
        $content = $("#collapsableAction");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });

    $("#divGeneralClick").click(function () {
        //getting the next element
        $content = $("#collapsableGeneral");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });
}

function initializeDailyRule_Blank() {
    conditionsNumber = 1;
    actionsNumber = 1;

    conditionTabActive = 0;
    actionTabActive = 0;

    conditionsEnabled = [true, false, false];
    actionsEnabled = [true, false, false];

    txtName_Client.SetText('');
    txtDescription_Client.SetText('');
    cmbApplyWhenClient.SetSelectedItem(cmbApplyWhenClient.GetItem(0));


    fillOldShiftsTB(actualShift, '');
    rbOldShiftAnyClient.SetChecked(true);
    tbOldShiftInListClient.SetValue('');
    tbOldShiftInListClient.SetEnabled(false);


    cmbApplyScheduleValidationRuleClient.SetSelectedItem(cmbApplyScheduleValidationRuleClient.GetItem(0));
    tbScheduleValidationRuleClient.SetValue('');
    tbScheduleValidationRuleClient.SetEnabled(false);

    //Si tbScheduleValidationRuleClient no tiene elementos en el dataSource, lo ocultamos
    if (parseInt(hdnScheduleValidationRuleCountClient.Get("count"), 10) == 0) {
        disableScheduleValidationRuleCombos();
    } else {
        enableScheduleValidationRuleCombos();
    }

    $("#conditionsAccordion").find('.conditionTab').each(function (index) {
        if (typeof $(this).attr('data-idTab') == 'undefined') $(this).attr('data-idTab', index);
        if (conditionsEnabled[index]) $(this).show();
        else $(this).hide();

        let cmbCompare = eval('cmbCompareClient_' + index);
        cmbCompare.SetSelectedIndex(0);
        let cmbType = eval('cmbTypeValueClient_' + index);
        cmbType.SetSelectedIndex(0);
        let textField = eval('txtValueTypeClient_' + index);
        textField.SetText('00:00');
        let textFieldTo = eval("txtValueTypeToClient_" + index);
        textFieldTo.SetText('00:00');
    });

    $("#actionsAccordion").find('.actionTab').each(function (index) {
        if (typeof $(this).attr('data-idTab') == 'undefined') $(this).attr('data-idTab', index);
        if (actionsEnabled[index]) $(this).show();
        else $(this).hide();

        eval('cmbActionsClient_' + index).SetSelectedIndex(0);

        /* CarryOver */
        eval("cmbCarryOverActionClient_" + index).SetSelectedIndex(0);
        eval("txtCarryOverValueClient_" + index).SetText('00:00');
        eval("cmbCarryOverConditionPartClient_" + index).SetSelectedIndex(0);
        eval("cmbCarryOverConditionNumberClient_" + index).SetSelectedIndex(0);
        eval("cmbCauseUFieldCarryOverClient_" + index).SetSelectedIndex(0);

        eval("cmbCarryOverActionResultClient_" + index).SetSelectedIndex(0);
        eval("txtCarryOverResultValueClient_" + index).SetText('00:00');
        eval("cmbCarryOverConditionPartResultClient_" + index).SetSelectedIndex(0);
        eval("cmbCarryOverConditionNumberResultClient_" + index).SetSelectedIndex(0);
        eval("cmbCauseUFieldCarryOverResultClient_" + index).SetSelectedIndex(0);

        eval("cmbCarryOverCauseFromClient_" + index).SetSelectedIndex(0);
        eval("cmbCarryOverCauseToClient_" + index).SetSelectedIndex(0);

        /* Plus */
        eval('cmbPlusCauseClient_' + index).SetSelectedIndex(0);

        eval("cmbPlusActionsClient_" + index).SetSelectedIndex(0);
        eval("txtPlusValueTimeClient_" + index).SetText('00:00');
        eval("txtPlusValueNumberClient_" + index).SetText('0');
        eval("cmbPlusConditionPartClient_" + index).SetSelectedIndex(0);
        eval("cmbPlusConditionNumberClient_" + index).SetSelectedIndex(0);
        eval("cmbCauseUFieldPlusClient_" + index).SetSelectedIndex(0);

        eval("cmbPlusResultActionsClient_" + index).SetSelectedIndex(0);
        eval("txtPlusValueResultTimeClient_" + index).SetText('00:00');
        eval("txtPlusValueResultNumberClient_" + index).SetText('0');
        eval("cmbPlusConditionPartResultClient_" + index).SetSelectedIndex(0);
        eval("cmbPlusConditionNumberResultClient_" + index).SetSelectedIndex(0);
        eval("cmbCauseUFieldPlusResultClient_" + index).SetSelectedIndex(0);

        eval("cmbPlusSignClient_" + index).SetSelectedIndex(0);

        if (parseInt(eval('cmbPlusCauseClient_' + index).GetSelectedItem().value.split("_")[1], 10) == 0) {
            document.getElementById('ActionDirectValue_Time_' + index).style.display = '';
            document.getElementById('ActionDirectValue_Number_' + index).style.display = 'none';
            document.getElementById('ActionDirectValueResult_Time_' + index).style.display = '';
            document.getElementById('ActionDirectValueResult_Number_' + index).style.display = 'none';
        } else {
            document.getElementById('ActionDirectValue_Time_' + index).style.display = 'none';
            document.getElementById('ActionDirectValue_Number_' + index).style.display = '';
            document.getElementById('ActionDirectValueResult_Time_' + index).style.display = 'none';
            document.getElementById('ActionDirectValueResult_Number_' + index).style.display = '';
        }
    });

    try { $("#conditionsAccordion").accordion("destroy"); } catch (e) { }
    $("#conditionsAccordion").accordion({
        active: 0,
        header: 'h3',
        heightStyle: "content",
        animate: false,
        activate: function (event, ui) {
            conditionTabActive = parseInt($(ui.newPanel.parent()).attr('data-idTab'), 10);
        }
    });

    try { $("#actionsAccordion").accordion("destroy"); } catch (e) { }
    $("#actionsAccordion").accordion({
        active: 0,
        header: 'h3',
        heightStyle: "content",
        animate: false,
        activate: function (event, ui) {
            actionTabActive = parseInt($(ui.newPanel.parent()).attr('data-idTab'), 10);
        }
    });

    $('#btnAddCondition').show();
    $('#btnAddCondition').addClass('btnFlat');

    $('#btnRemoveCondition').hide();
    $('#btnRemoveCondition').removeClass('btnFlat');

    $('#btnAddAction').show();
    $('#btnAddAction').addClass('btnFlat');

    $('#btnRemoveAction').hide();
    $('#btnRemoveAction').removeClass('btnFlat');

    for (var i = 0; i < actionsEnabled.length; i++) {
        var tableName = 'divActionsCauses_';
        document.getElementById(tableName + i).innerHTML = '';

        var oTable = document.getElementById('htTableAction_' + i + "_" + 0);

        if (oTable == null) {
            oTable = document.createElement("TABLE");
            oTable.id = 'htTableAction_' + i + "_" + 0;
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            document.getElementById(tableName + i).appendChild(oTable);
            oTable.setAttribute("style", "width: 100%;");
            oTable.style.width = "100%";

            var oTR = oTable.insertRow(-1);

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.textContent = document.getElementById("origen").value;

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.width = 200;
            oTD.setAttribute("style", "border-right: 0pt none;");
            oTD.textContent = document.getElementById("destino").value;
        }

        var action = eval("cmbActionsClient_" + i);
        action.SetSelectedIndex(0);
        showActionConfiguration(i, action, true);
    }

    for (var i = 0; i < conditionsEnabled.length; i++) {
        var tableName = 'divConditionsCauses_';
        document.getElementById(tableName + i).innerHTML = '';

        var oTable = document.getElementById('htTableCondition_' + i + "_" + 0);

        if (oTable == null) {
            oTable = document.createElement("TABLE");
            oTable.id = 'htTableCondition_' + i + "_" + 0;
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            document.getElementById(tableName + i).appendChild(oTable);
            oTable.setAttribute("style", "width: 100%;");
            oTable.style.width = "100%";

            var oTR = oTable.insertRow(-1);

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.textContent = document.getElementById("header2").value;

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.width = 200;
            oTD.setAttribute("style", "border-right: 0pt none;");
            oTD.textContent = document.getElementById("header1").value;
        }

        tableName = 'divConditionsCausesValue_';
        document.getElementById(tableName + i).innerHTML = '';

        var oTable = document.getElementById('htTableCondition_' + i + "_" + 1);

        if (oTable == null) {
            oTable = document.createElement("TABLE");
            oTable.id = 'htTableCondition_' + i + "_" + 1;
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            document.getElementById(tableName + i).appendChild(oTable);
            oTable.setAttribute("style", "width: 100%;");
            oTable.style.width = "100%";

            var oTR = oTable.insertRow(-1);

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.textContent = document.getElementById("header2").value;

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.width = 200;
            oTD.setAttribute("style", "border-right: 0pt none;");
            oTD.textContent = document.getElementById("header1").value;
        }

        tableName = 'divConditionTimeZones_';
        document.getElementById(tableName + i).innerHTML = '';

        var oTable = document.getElementById('htTableTimeZoneCondition_' + i + "_" + 0);

        if (oTable == null) {
            oTable = document.createElement("TABLE");
            oTable.id = 'htTableTimeZoneCondition_' + i + "_" + 0;
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            document.getElementById(tableName + i).appendChild(oTable);
            oTable.setAttribute("style", "width: 100%;");
            oTable.style.width = "100%";

            var oTR = oTable.insertRow(-1);
            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.width = 200;
            oTD.setAttribute("style", "border-right: 0pt none;");
            oTD.textContent = document.getElementById("header3").value;
        }

        tableName = 'divConditionTimeZonesValue_';
        document.getElementById(tableName + i).innerHTML = '';

        var oTable = document.getElementById('htTableTimeZoneCondition_' + i + "_" + 1);

        if (oTable == null) {
            oTable = document.createElement("TABLE");
            oTable.id = 'htTableTimeZoneCondition_' + i + "_" + 1;
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            document.getElementById(tableName + i).appendChild(oTable);
            oTable.setAttribute("style", "width: 100%;");
            oTable.style.width = "100%";

            var oTR = oTable.insertRow(-1);
            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.width = 200;
            oTD.setAttribute("style", "border-right: 0pt none;");
            oTD.textContent = document.getElementById("header3").value;
        }

        var cmbCompare = eval('cmbCompareClient_' + i);
        var cmbType = eval('cmbTypeValueClient_' + i);

        cmbCompare.SetSelectedIndex(0);
        cmbType.SetSelectedIndex(0);
        showTypeValue(i, cmbType, false);
    }
}

function validateOldShiftRadio(s, e) {
    if (rbOldShiftAnyClient.GetChecked()) {
        tbOldShiftInListClient.SetEnabled(false);
    } else {
        tbOldShiftInListClient.SetEnabled(true);
    }
}

function validateTBScheduleValidationRule() {
    if (cmbApplyScheduleValidationRuleClient.GetSelectedItem().value > 0) {
        tbScheduleValidationRuleClient.SetEnabled(true);
    } else {
        tbScheduleValidationRuleClient.SetEnabled(false);
    }
}

function loadDailyRule(oDailyRule) {
    txtName_Client.SetText(oDailyRule.Name);
    txtDescription_Client.SetText(oDailyRule.Description);
    cmbApplyWhenClient.SetValue(oDailyRule.DayValidationRule);
    cmbApplyScheduleValidationRuleClient.SetValue(oDailyRule.ApplyScheduleValidationRule);

    if (oDailyRule.PreviousShiftValidationRule == null || oDailyRule.PreviousShiftValidationRule.length == 0 || (oDailyRule.PreviousShiftValidationRule.length == 1 && oDailyRule.PreviousShiftValidationRule[0] <= 0)) {
        fillOldShiftsTB(actualShift, '');
        tbOldShiftInListClient.SetEnabled(false);
        rbOldShiftAnyClient.SetChecked(true);
        tbOldShiftInListClient.SetValue('');
    } else {
        fillOldShiftsTB(actualShift, oDailyRule.PreviousShiftValidationRule.join(','));
        tbOldShiftInListClient.SetEnabled(true);
        rbOldShiftInListClient.SetChecked(true);
        tbOldShiftInListClient.SetValue(oDailyRule.PreviousShiftValidationRule.join(','));
    }

        enableScheduleValidationRuleCombos();
        if (oDailyRule.ApplyScheduleValidationRule > 0) {
            tbScheduleValidationRuleClient.SetEnabled(true);
            tbScheduleValidationRuleClient.SetValue(oDailyRule.ScheduleRulesValidationRule.join(','));
        } else {
            tbScheduleValidationRuleClient.SetEnabled(false);
            tbScheduleValidationRuleClient.SetValue('');
        }

    loadConditions(oDailyRule.Conditions);
    loadActions(oDailyRule.Actions);
    actualPriority = oDailyRule.Priority;
    actualID = oDailyRule.ID;

    ASPxClientEdit.ValidateGroup(null, true);
}

function disableScheduleValidationRuleCombos() {
    cmbApplyScheduleValidationRuleClient.SetSelectedItem(cmbApplyScheduleValidationRuleClient.GetItem(0));
    cmbApplyScheduleValidationRuleClient.SetEnabled(false);
    tbScheduleValidationRuleClient.SetVisible(false);
    tbScheduleValidationRuleClient.SetValue('');
    tbScheduleValidationRuleClient.SetEnabled(false);
    $('#lblScheduleValidationRuleNotFound').show();
}

function enableScheduleValidationRuleCombos() {
    cmbApplyScheduleValidationRuleClient.SetEnabled(true);
    tbScheduleValidationRuleClient.SetVisible(true);
    $('#lblScheduleValidationRuleNotFound').hide();
}

function frmDailyRule_Close() {
    try {
        if (ruleSource == 1) showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDailyRule1', false);
        else {
            showWUF('frmDailyRule1', false);
            onCancelEdition();
        }

    } catch (e) { showError("frmDailyRule_Close", e); }
}

function frmDailyRule_Validate() {
    try {
        if (rbOldShiftInListClient.GetChecked() && tbOldShiftInListClient.GetValue() == '') {
            showErrorPopup("Error.DailyRule", "error", "Error.MustSelectShiftsInList", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        
        if (cmbApplyScheduleValidationRuleClient.GetSelectedItem().value > 0 && tbScheduleValidationRuleClient.GetValue() == '') {
            showErrorPopup("Error.DailyRule", "error", "Error.MustSelectLabAgreeRulesInList", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (ASPxClientEdit.ValidateGroup('rulesInfo', true)) {
            let oConditions = getActiveConditions()
            let oActions = getActiveActions()

            for (var i = 0; i < oConditions.length; i++) {
                if (!frmDailyRule_ValidateCondition(oConditions[i])) return false;
            }

            for (var i = 0; i < oActions.length; i++) {
                if (!frmDailyRule_ValidateActions(oActions[i])) return false;
            }

            return true;
        } else {
            showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleNameMustExists", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }
    } catch (e) { showError("frmDailyRule_Validate", e); }
}

function frmDailyRule_ValidateActions(oAction) {
    if (parseInt(oAction.Action, 10) == 0) { //arrastrar
        if (parseInt(oAction.CarryOverAction, 10) == 2) {
            if (oAction.CarryOverConditionNumber >= conditionsNumber) {
                showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleConditionNotExists", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        if (parseInt(oAction.CarryOverActionResult, 10) == 2) {
            if (oAction.CarryOverConditionNumberResult >= conditionsNumber) {
                showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleConditionNotExists", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        if (oAction.CarryOverIDCauseFrom == oAction.CarryOverIDCauseTo) {
            showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleActionCauseError", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }
    } else {//prima
        if (parseInt(oAction.PlusAction, 10) == 2) {
            if (oAction.PlusConditionNumber >= conditionsNumber) {
                showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleConditionNotExists", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        if (parseInt(oAction.PlusActionResult, 10) == 2) {
            if (oAction.PlusConditionNumberResult >= conditionsNumber) {
                showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleConditionNotExists", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }
    }

    return true;
}

function frmDailyRule_ValidateCondition(oCondition) {
    if (oCondition.ConditionCauses == null || oCondition.ConditionCauses.length == 0) {
        showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleConditionNoCauseSpecified", "", "Error.OK", "Error.OKDesc", "");
        return false;
    }

    if (oCondition.ConditionTimeZones == null || oCondition.ConditionTimeZones.length == 0) {
        showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleConditionNoTimeZoneSpecified", "", "Error.OK", "Error.OKDesc", "");
        return false;
    }

    if (parseInt(oCondition.Compare, 10) == 6) {
        if (FrmDailyRule_ConvertHoursToMinutes(oCondition.FromValue) > FrmDailyRule_ConvertHoursToMinutes(oCondition.ToValue)) {
            showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleBetweenLimits", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }
    }

    if (parseInt(oCondition.Type, 10) == 1) {
        if (oCondition.UserField == '') {
            showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleNoUserFieldSelected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }
    }

    if (parseInt(oCondition.Type, 10) == 2) {
        if (oCondition.CompareCauses == null || oCondition.CompareCauses.length == 0) {
            showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleResultNoCauseSpecified", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (oCondition.CompareTimeZones == null || oCondition.CompareTimeZones.length == 0) {
            showErrorPopup("Error.DailyRule", "error", "Error.DailyRuleResultNoTimeZoneSpecified", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }
    }

    return true;
}

function frmDailyRule_Save() {
    try {
        if (ruleSource == 1 && document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "false") {
            frmDailyRule_Close();
            return;
        } 

        if (ruleSource == 2 && document.getElementById('hdnModeEdit').value != "false") {
            frmDailyRule_Close();
            return;
        }


        if (frmDailyRule_Validate()) {


            let oldShiftsList = new Array();
            oldShiftsList.push(0);
            if (rbOldShiftInListClient.GetChecked()) {
                oldShiftsList = tbOldShiftInListClient.GetValue().split(",").map(Number);
            }

            let lstScheduleValidationRule = new Array();
            if (cmbApplyScheduleValidationRuleClient.GetSelectedItem().value > 0) {
                lstScheduleValidationRule = tbScheduleValidationRuleClient.GetValue().split(",").map(Number);
            }

            let oDailyRule = {
                ID: actualID,
                IDShift: actualShift,
                Name: txtName_Client.GetText(),
                Description: txtDescription_Client.GetText(),
                DayValidationRule: cmbApplyWhenClient.GetSelectedItem().value,
                Conditions: getActiveConditions(),
                Actions: getActiveActions(),
                Priority: actualPriority,
                PreviousShiftValidationRule: oldShiftsList,
                ApplyScheduleValidationRule: cmbApplyScheduleValidationRuleClient.GetSelectedItem().value,
                ScheduleRulesValidationRule: lstScheduleValidationRule
            }


            if (typeof createGridDailyRulesLine === 'function') {

                if (bIsNew) {
                    dailyRules.push(oDailyRule);
                    dailyRules = dailyRules.sort(function (a, b) { return (a.Priority > b.Priority) ? 1 : ((b.Priority > a.Priority) ? -1 : 0); });

                    createGridDailyRulesLine(oDailyRule);
                } else {
                    dailyRules[actualPriority - 1] = oDailyRule;
                    editGridDailyRulesLine(oDailyRule);
                }

                hasChangesShifts(true);
                setDailyRuleChanges(true);

            } else {
                onAcceptRule(oDailyRule);
            }

            frmDailyRule_Close();
        }
    } catch (e) { showError("frmDailyRule_Save", e); }
}

// ==========================================
// SECTION: Actions and conditions management
// ==========================================
//#region Actions and conditions management


function AddDailyCondition() {
    conditionsNumber += 1;

    for (var i = 0; i < conditionsEnabled.length; i++) {
        var itemHeader = $($($("#conditionsAccordion").find('.conditionTab')[i]).find('.headerText'));

        itemHeader.html(itemHeader.html().trim().split(' ')[0] + ' ' + (i + 1))

        if (conditionsEnabled[i] == false) {
            conditionsEnabled[i] = true;

            $($("#conditionsAccordion").find('.conditionTab')[i]).show();

            if (i != (conditionsNumber - 1)) {
                var mainItem = $("#conditionsAccordion").find('.conditionTab')[i];

                var destiny = null;

                $("#conditionsAccordion").find('.conditionTab').each(function (index) {
                    if ($(this).is(':visible')) destiny = this;
                });

                if (destiny != null) $(mainItem).insertAfter($(destiny));
            }

            break;
        }
    }

    if (conditionsNumber == 3) {
        $('#btnAddCondition').hide();
        $('#btnAddCondition').removeClass('btnFlat');
    }

    if (conditionsNumber > 1) {
        $('#btnRemoveCondition').show();
        $('#btnRemoveCondition').addClass('btnFlat');
    }

    var nextVisible = -1;
    for (var i = 0; i < conditionsEnabled.length; i++) {
        if (conditionsEnabled[i]) {
            nextVisible = i;
        } else {
            break;
        }
    }

    nextVisible = parseInt($($("#conditionsAccordion").find('.conditionTab')[nextVisible]).attr('data-idTab'), 10);

    $("#conditionsAccordion").accordion({ active: nextVisible });

    //$('.conditionContent').each(function () {
    //    $(this).height($(this).height() - 30);
    //});
}

function RemoveDailyCondition() {
    var visible = -1;
    $("#conditionsAccordion").find('.conditionTab').each(function (index) {
        if (parseInt($(this).attr('data-idTab'), 10) == conditionTabActive) visible = index;
    });

    var isLast = (visible == (conditionsNumber - 1));

    if (isLast) {
        $($("#conditionsAccordion").find('.conditionTab')[conditionsNumber - 1]).hide();
        conditionsEnabled[conditionsNumber - 1] = false;
    } else {
        var mainItem = $("#conditionsAccordion").find('.conditionTab')[visible];

        var lastIndexVisible = -1;
        $("#conditionsAccordion").find('.conditionTab').each(function (index) {
            if ($(this).is(':visible')) lastIndexVisible = lastIndexVisible + 1;
        });

        var lastItem = $("#conditionsAccordion").find('.conditionTab')[lastIndexVisible];
        conditionsEnabled[lastIndexVisible] = false;
        $(mainItem).insertAfter($(lastItem));

        $($("#conditionsAccordion").find('.conditionTab')[lastIndexVisible]).hide();
    }

    $("#conditionsAccordion").find('.conditionTab').each(function (index) {
        var itemHeader = $(this).find('.headerText');
        itemHeader.html(itemHeader.html().trim().split(' ')[0] + ' ' + (index + 1))
    });

    conditionsNumber -= 1;

    if (conditionsNumber < 3) {
        $('#btnAddCondition').show();
        $('#btnAddCondition').addClass('btnFlat');
    }

    if (conditionsNumber == 1) {
        $('#btnRemoveCondition').hide();
        $('#btnRemoveCondition').removeClass('btnFlat');
    }

    var nextVisible = -1;
    for (var i = 0; i < conditionsEnabled.length; i++) {
        if (conditionsEnabled[i]) {
            nextVisible = i;
        } else {
            break;
        }
    }

    nextVisible = parseInt($($("#conditionsAccordion").find('.conditionTab')[nextVisible]).attr('data-idTab'), 10);

    $("#conditionsAccordion").accordion({ active: nextVisible });

    //$('.conditionContent').each(function () {
    //    $(this).height($(this).height() + 30);
    //});
}

function AddDailyAction() {
    actionsNumber += 1;

    for (var i = 0; i < actionsEnabled.length; i++) {
        var itemHeader = $($($("#actionsAccordion").find('.actionTab')[i]).find('.headerText'));
        itemHeader.html(itemHeader.html().trim().split(' ')[0] + ' ' + (i + 1))

        if (actionsEnabled[i] == false) {
            actionsEnabled[i] = true;

            $($("#actionsAccordion").find('.actionTab')[i]).show();

            if (i != (actionsNumber - 1)) {
                var mainItem = $("#actionsAccordion").find('.actionTab')[i];

                var destiny = null;

                $("#actionsAccordion").find('.actionTab').each(function (index) {
                    if ($(this).is(':visible')) destiny = this;
                });

                if (destiny != null) $(mainItem).insertAfter($(destiny));
            }

            break;
        }
    }

    if (actionsNumber == 3) {
        $('#btnAddAction').hide();
        $('#btnAddAction').removeClass('btnFlat');
    }

    if (actionsNumber > 1) {
        $('#btnRemoveAction').show();
        $('#btnRemoveAction').addClass('btnFlat');
    }

    var nextVisible = -1;
    for (var i = 0; i < actionsEnabled.length; i++) {
        if (actionsEnabled[i]) {
            nextVisible = i;
        } else {
            break;
        }
    }

    nextVisible = parseInt($($("#actionsAccordion").find('.actionTab')[nextVisible]).attr('data-idTab'), 10);

    $("#actionsAccordion").accordion({ active: nextVisible });

    //$('.actionContent').each(function () {
    //    $(this).height($(this).height() - 30);
    //});
}

function RemoveDailyAction() {
    var visible = -1;
    $("#actionsAccordion").find('.actionTab').each(function (index) {
        if (parseInt($(this).attr('data-idTab'), 10) == actionTabActive) visible = index;
    });

    var isLast = (visible == (actionsNumber - 1));

    if (isLast) {
        $($("#actionsAccordion").find('.actionTab')[actionsNumber - 1]).hide();
        actionsEnabled[actionsNumber - 1] = false;
    } else {
        var mainItem = $("#actionsAccordion").find('.actionTab')[visible];

        var lastIndexVisible = -1;
        $("#actionsAccordion").find('.actionTab').each(function (index) {
            if ($(this).is(':visible')) lastIndexVisible = lastIndexVisible + 1;
        });

        var lastItem = $("#actionsAccordion").find('.actionTab')[lastIndexVisible];
        actionsEnabled[lastIndexVisible] = false;
        $(mainItem).insertAfter($(lastItem));

        $($("#actionsAccordion").find('.actionTab')[lastIndexVisible]).hide();
    }

    $("#actionsAccordion").find('.actionTab').each(function (index) {
        var itemHeader = $(this).find('.headerText');
        itemHeader.html(itemHeader.html().trim().split(' ')[0] + ' ' + (index + 1))
    });

    actionsNumber -= 1;

    if (actionsNumber < 3) {
        $('#btnAddAction').show();
        $('#btnAddAction').addClass('btnFlat');
    }

    if (actionsNumber == 1) {
        $('#btnRemoveAction').hide();
        $('#btnRemoveAction').removeClass('btnFlat');
    }

    var nextVisible = -1;
    for (var i = 0; i < actionsEnabled.length; i++) {
        if (actionsEnabled[i]) {
            nextVisible = i;
        } else {
            break;
        }
    }

    nextVisible = parseInt($($("#actionsAccordion").find('.actionTab')[nextVisible]).attr('data-idTab'), 10);

    $("#actionsAccordion").accordion({ active: nextVisible });

    //$('.actionContent').each(function () {
    //    $(this).height($(this).height() + 30);
    //});
}

function ShowWindow(objId, bol) {
    try {
        var objWnd = document.getElementById(objId);
        var oDivBg;
        if (objWnd != null) {
            if (bol == true) {
                oDivBg = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDailyRule1_BgS');
                oDivBg.style.display = "";
                oDivBg.style.top = "0";
                oDivBg.style.left = "0";
                oDivBg.style.height = "2000px";  //document.body.offsetHeight;
                oDivBg.style.width = "3000px";  //document.body.offsetWidth;
                oDivBg.style.backgroundColor = "transparent";

                objWnd.style.display = "";
            } else {
                oDivBg = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDailyRule1_BgS');
                oDivBg.style.display = "none";
                objWnd.style.display = "none";
            }
        }
    } catch (e) { showError("ShowWindow", e); }
}

function AddCauseActionOK(instance_name, isResult, windowID) {
    try {
        var cmb = null;
        var cmb2 = null;

        if (isResult == 0) {
            cmb = eval("cmbCauseActionAddClient_" + instance_name);
            cmb2 = eval("cmbCauseActionAdd2Client_" + instance_name);
        }

        var opStr = '0';

        if (cmb == null || cmb2 == null) { return; }

        if (cmb.GetSelectedItem().value == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoCauseSelected", "", "Error.OK", "Error.OKDesc", "");
            return;
        }
        if (cmb2.GetSelectedItem().value == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoCauseSelected", "", "Error.OK", "Error.OKDesc", "");
            return;
        }

        var bolRet = AddListValueAction(cmb.GetSelectedItem().value, cmb.GetSelectedItem().text, cmb2.GetSelectedItem().value, cmb2.GetSelectedItem().text, instance_name, isResult);

        //Cerramos la ventana si todo ok
        if (bolRet == true) {
            ShowWindow(windowID, false);
        }
    } catch (e) { showError("AddCauseActionOK", e); }
}

function AddCauseOK(instance_name, isResult, windowID) {
    try {
        var cmb = null;

        var opOperation = null;

        if (isResult == 0) {
            opOperation = document.getElementById("opPlus_" + instance_name);
            cmb = eval("cmbCauseAddClient_" + instance_name);
        } else {
            opOperation = document.getElementById("opPlusValue_" + instance_name);
            cmb = eval("cmbAddValueCauseClient_" + instance_name);
        }

        var opStr = '0';

        if (cmb == null) { return; }

        if (cmb.GetSelectedItem().value == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoCauseSelected", "", "Error.OK", "Error.OKDesc", "");
            return;
        }

        if (opOperation.checked == false) {
            opStr = '1';
        }

        var bolRet = AddListValue(cmb.GetSelectedItem().value, cmb.GetSelectedItem().text, opStr, instance_name, isResult);

        //Cerramos la ventana si todo ok
        if (bolRet == true) {
            ShowWindow(windowID, false);
        }
    } catch (e) { showError("AddCauseOK", e); }
}

function checkIfExistValueinListAction(IDCause, instance_name, isResult) {
    try {
        var hTable = document.getElementById('htTableAction_' + instance_name + "_" + isResult);

        //Si no hi ha taula
        if (hTable == null) { return; }
        if (IDCause == "") { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsAction_" + instance_name + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsAction_" + instance_name + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htRowsAction_" + instance_name + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsAction_" + instance_name + '_' + isResult);
                }
                break;
        }

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("IDCause") == IDCause) {
                //si troba el row, l'eliminem (n+1, per la capcelera)
                return true;
            }
        }

        return false;

        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("checkIfExistValueinList", e); return false; }
}

function checkIfExistValueinList(IDCause, instance_name, isResult) {
    try {
        var hTable = document.getElementById('htTableCondition_' + instance_name + "_" + isResult);

        //Si no hi ha taula
        if (hTable == null) { return; }
        if (IDCause == "") { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRows_" + instance_name + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRows_" + instance_name + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htRows_" + instance_name + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRows_" + instance_name + '_' + isResult);
                }
                break;
        }

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("IDCause") == IDCause) {
                //si troba el row, l'eliminem (n+1, per la capcelera)
                return true;
            }
        }

        return false;

        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("checkIfExistValueinList", e); return false; }
}

function AddListValueAction(IDCause, Name, IDCause2, Name2, instance_name, isResult) {
    try {
        //Si existe en la lista, devolvemos un error
        if (checkIfExistValueinListAction(IDCause, instance_name, isResult)) {
            showErrorPopup("Error.DailyRule", "error", "Error.CauseAlreadyExists", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (IDCause == IDCause2) {
            showErrorPopup("Error.DailyRule", "error", "No puedes elegir la misma justificación de origen y destino", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }
        var tableName = 'divActionsCauses_';
        if (isResult == 1) tableName = 'divActionsCausesValue_';

        var oTable = document.getElementById('htTableAction_' + instance_name + "_" + isResult);

        if (oTable == null) {
            oTable = document.createElement("TABLE");
            oTable.id = 'htTableAction_' + instance_name + "_" + isResult;
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            document.getElementById(tableName + instance_name).appendChild(oTable);
            oTable.setAttribute("style", "width: 100%;");
            oTable.style.width = "100%";

            var oTR = oTable.insertRow(-1);

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.textContent = document.getElementById("origen").value;

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.width = 200;
            oTD.setAttribute("style", "border-right: 0pt none;");
            oTD.textContent = document.getElementById("destino").value;
        }

        var oNewId = oTable.rows.length;
        var altRow = 1;

        /*Afegim el tr */
        var oTR = oTable.insertRow(-1);
        oTR.id = 'htRowAction_' + instance_name + '_' + isResult + '_' + oNewId;
        oTR.setAttribute("name", "htRowsAction_" + instance_name + '_' + isResult);
        oTR.setAttribute("IDCause", IDCause);
        oTR.setAttribute("IDCause2", IDCause2);
        oTR.setAttribute("CauseName", Name);
        oTR.setAttribute("CauseName2", Name2);

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
            oTR.setAttribute("onmouseover", "javascript: rowOver('" + oTR.id + "');");
            oTR.setAttribute("onmouseout", "javascript: rowOut('" + oTR.id + "');");
            oTR.setAttribute("onclick", "javascript: rowClick('" + oTR.id + "','" + IDCause + "','htTableAction_" + instance_name + "_" + isResult + "'," + instance_name + "," + isResult + ");");
        } else { // IE
            oTR.onmouseover = function () { rowOver(this.id); }
            oTR.onmouseout = function () { rowOut(this.id); }
            oTR.onclick = function () { rowClick(this.id, this.getAttribute("IDCause"), 'htTableAction_' + instance_name + '_' + isResult, instance_name, isResult); }
        }

        if ((oNewId % 2) != 0) {
            altRow = "1";
        } else {
            altRow = "2";
        }

        var oTD = oTR.insertCell(-1); //Name
        oTD.className = "GridStyle-cell" + altRow;
        oTD.textContent = Name;
        var oTD = oTR.insertCell(-1); //Name
        oTD.className = "GridStyle-cell" + altRow;
        oTD.textContent = Name2;

        return true;
    } catch (e) {
        showError("AddListValue", e);
        return false;
    }
}
function AddListValue(IDCause, Name, Operation, instance_name, isResult) {
    try {
        //Si existe en la lista, devolvemos un error
        if (checkIfExistValueinList(IDCause, instance_name, isResult)) {
            showErrorPopup("Error.DailyRule", "error", "Error.CauseAlreadyExists", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        var tableName = 'divConditionsCauses_';
        if (isResult == 1) tableName = 'divConditionsCausesValue_';

        var oTable = document.getElementById('htTableCondition_' + instance_name + "_" + isResult);

        if (oTable == null) {
            oTable = document.createElement("TABLE");
            oTable.id = 'htTableCondition_' + instance_name + "_" + isResult;
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            document.getElementById(tableName + instance_name).appendChild(oTable);
            oTable.setAttribute("style", "width: 100%;");
            oTable.style.width = "100%";

            var oTR = oTable.insertRow(-1);

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.textContent = document.getElementById("header2").value;

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.width = 200;
            oTD.setAttribute("style", "border-right: 0pt none;");
            oTD.textContent = document.getElementById("header1").value;
        }

        var oNewId = oTable.rows.length;
        var altRow = 1;

        /*Afegim el tr */
        var oTR = oTable.insertRow(-1);
        oTR.id = 'htRow_' + instance_name + '_' + isResult + '_' + oNewId;
        oTR.setAttribute("name", "htRows_" + instance_name + '_' + isResult);
        oTR.setAttribute("IDCause", IDCause);
        oTR.setAttribute("Operation", Operation);
        oTR.setAttribute("CauseName", Name);

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
            oTR.setAttribute("onmouseover", "javascript: rowOver('" + oTR.id + "');");
            oTR.setAttribute("onmouseout", "javascript: rowOut('" + oTR.id + "');");
            oTR.setAttribute("onclick", "javascript: rowClick('" + oTR.id + "','" + IDCause + "','htTableCondition_" + instance_name + "_" + isResult + "'," + instance_name + "," + isResult + ");");
        } else { // IE
            oTR.onmouseover = function () { rowOver(this.id); }
            oTR.onmouseout = function () { rowOut(this.id); }
            oTR.onclick = function () { rowClick(this.id, this.getAttribute("IDCause"), 'htTableCondition_' + instance_name + '_' + isResult, instance_name, isResult); }
        }

        if ((oNewId % 2) != 0) {
            altRow = "1";
        } else {
            altRow = "2";
        }

        var oTD = oTR.insertCell(-1); //Name
        oTD.className = "GridStyle-cell" + altRow;

        if (Operation == "0") {
            oTD.textContent = document.getElementById('OperationPlus').value;
        } else {
            oTD.textContent = document.getElementById('OperationMinus').value;
        }

        var oTD = oTR.insertCell(-1); //Name
        oTD.className = "GridStyle-cell" + altRow;
        oTD.textContent = Name;

        return true;
    } catch (e) {
        showError("AddListValue", e);
        return false;
    }
}

function rowClick(rowID, ID, dTable, instance_name, isResult) {
    if (isResult == 0) document.getElementById('selectedIdx_' + instance_name).value = ID;
    else if (isResult == 1) document.getElementById('selectedIdxValue_' + instance_name).value = ID;

    var tParent = document.getElementById(dTable);
    var tCells = tParent.getElementsByTagName("td");
    for (var i = 0; i < tCells.length; i++) {
        removeCssClass(tCells[i], "gridRowOver");
        removeCssClass(tCells[i], "gridRowSelected");
    }

    var table = document.getElementById(rowID);
    var cells = table.getElementsByTagName("td");
    for (var i = 0; i < cells.length; i++) {
        removeCssClass(cells[i], "gridRowOver");
        addCssClass(cells[i], "gridRowSelected");
    }
}

function rowOver(rowID) {
    var table = document.getElementById(rowID);
    var cells = table.getElementsByTagName("td");
    for (var i = 0; i < cells.length; i++) {
        addCssClass(cells[i], "gridRowOver");
    }
}

function rowOut(rowID) {
    var table = document.getElementById(rowID);
    var cells = table.getElementsByTagName("td");
    for (var i = 0; i < cells.length; i++) {
        removeCssClass(cells[i], "gridRowOver");
    }
}
function RemoveListValueAction(instance_name, isResult, windowID, mustClose) {
    try {
        var IDCause = null;//document.getElementById('selectedIdx').value;
        var hTable = null;//document.getElementById('htTableAction');

        if (isResult == 0) {
            IDCause = document.getElementById('selectedIdx_' + instance_name).value;
            hTable = document.getElementById('htTableAction_' + instance_name + "_" + isResult);
        } else if (isResult == 1) {
            IDCause = document.getElementById('selectedIdxValue_' + instance_name).value;
            hTable = document.getElementById('htTableAction_' + instance_name + "_" + isResult);
        }

        //Si no hi ha taula
        if (hTable == null) { return; }
        if (IDCause == "") { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsAction_" + instance_name + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsAction_" + instance_name + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htRowsAction_" + instance_name + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsAction_" + instance_name + '_' + isResult);
                }
                break;
        }

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("IDCause") == IDCause) {
                hTable.deleteRow(n + 1);
                return;
            }
        }

        //oComposition.deleteCauseAction(IDCause);
    } catch (e) { showError("RemoveListValue", e); }
}
function RemoveListValue(instance_name, isResult, windowID, mustClose) {
    try {
        var IDCause = null;//document.getElementById('selectedIdx').value;
        var hTable = null;//document.getElementById('htTableCondition');

        if (isResult == 0) {
            IDCause = document.getElementById('selectedIdx_' + instance_name).value;
            hTable = document.getElementById('htTableCondition_' + instance_name + "_" + isResult);
        } else if (isResult == 1) {
            IDCause = document.getElementById('selectedIdxValue_' + instance_name).value;
            hTable = document.getElementById('htTableCondition_' + instance_name + "_" + isResult);
        }

        //Si no hi ha taula
        if (hTable == null) { return; }
        if (IDCause == "") { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRows_" + instance_name + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRows_" + instance_name + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htRows_" + instance_name + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRows_" + instance_name + '_' + isResult);
                }
                break;
        }

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("IDCause") == IDCause) {
                hTable.deleteRow(n + 1);
                return;
            }
        }

        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("RemoveListValue", e); }
}

function AddTimeZoneOK(instance_name, isResult, windowID) {
    try {
        var cmb = null;

        if (isResult == 0) {
            cmb = eval("cmbTimeZoneAddClient_" + instance_name);
        } else {
            cmb = eval("cmbTimeZoneValueAddClient_" + instance_name);
        }

        if (cmb == null) { return; }

        if (cmb.GetSelectedItem().value == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoCauseSelected", "", "Error.OK", "Error.OKDesc", "");
            return;
        }

        var bolRet = AddTimeZoneValue(cmb.GetSelectedItem().value, cmb.GetSelectedItem().text, instance_name, isResult);

        //Cerramos la ventana si todo ok
        if (bolRet == true) {
            ShowWindow(windowID, false);
        }
    } catch (e) { showError("AddTimeZoneOK", e); }
}

function checkIfAnyZoneAdded(instance_name, isResult) {
    try {
        var hTable = document.getElementById('htTableTimeZoneCondition_' + instance_name + "_" + isResult);

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htTimeZoneRows_" + instance_name + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htTimeZoneRows_" + instance_name + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htTimeZoneRows_" + instance_name + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htTimeZoneRows_" + instance_name + '_' + isResult);
                }
                break;
        }

        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (parseInt(hRow.getAttribute("IDTimeZone"), 10) == -1) {
                return true;
            }
        }

        return false;
    } catch (e) { showError("checkIfExistTimeZoneinList", e); return false; }
}

function checkIfExistTimeZoneinList(IDTimeZone, instance_name, isResult) {
    try {
        var hTable = document.getElementById('htTableTimeZoneCondition_' + instance_name + "_" + isResult);

        //Si no hi ha taula
        if (hTable == null) { return; }
        if (IDTimeZone == "") { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htTimeZoneRows_" + instance_name + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htTimeZoneRows_" + instance_name + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htTimeZoneRows_" + instance_name + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htTimeZoneRows_" + instance_name + '_' + isResult);
                }
                break;
        }

        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("IDTimeZone") == IDTimeZone) {
                return true;
            }
        }

        return false;
    } catch (e) { showError("checkIfExistTimeZoneinList", e); return false; }
}

function AddTimeZoneValue(IDTimeZone, Name, instance_name, isResult) {
    try {
        //Si existe en la lista, devolvemos un error
        if (checkIfExistTimeZoneinList(IDTimeZone, instance_name, isResult)) {
            showErrorPopup("Error.DailyRule", "error", "Error.TimeZoneAlreadyExists", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (parseInt(IDTimeZone, 10) != -1 && checkIfAnyZoneAdded(instance_name, isResult)) {
            showErrorPopup("Error.DailyRule", "error", "Error.AnyTimezoneAdded", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        var tableName = 'divConditionTimeZones_';
        if (isResult == 1) tableName = 'divConditionTimeZonesValue_';

        var oTable = document.getElementById('htTableTimeZoneCondition_' + instance_name + "_" + isResult);

        if (oTable == null) {
            oTable = document.createElement("TABLE");
            oTable.id = 'htTableTimeZoneCondition_' + instance_name + "_" + isResult;
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            document.getElementById(tableName + instance_name).appendChild(oTable);
            oTable.setAttribute("style", "width: 100%;");
            oTable.style.width = "100%";

            var oTR = oTable.insertRow(-1);
            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.width = 200;
            oTD.setAttribute("style", "border-right: 0pt none;");
            oTD.textContent = document.getElementById("header3").value;
        }

        var oNewId = oTable.rows.length;
        var altRow = 1;

        /*Afegim el tr */
        var oTR = oTable.insertRow(-1);
        oTR.id = 'htTimeZoneRow_' + instance_name + '_' + isResult + '_' + oNewId;
        oTR.setAttribute("name", "htTimeZoneRows_" + instance_name + '_' + isResult);
        oTR.setAttribute("IDTimeZone", IDTimeZone);
        oTR.setAttribute("TimeZoneName", Name);

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
            oTR.setAttribute("onmouseover", "javascript: rowOver('" + oTR.id + "');");
            oTR.setAttribute("onmouseout", "javascript: rowOut('" + oTR.id + "');");
            oTR.setAttribute("onclick", "javascript: rowTimeZoneClick('" + oTR.id + "','" + IDTimeZone + "','htTableTimeZoneCondition_" + instance_name + "_" + isResult + "'," + instance_name + "," + isResult + ");");
        } else { // IE
            oTR.onmouseover = function () { rowOver(this.id); }
            oTR.onmouseout = function () { rowOut(this.id); }
            oTR.onclick = function () { rowTimeZoneClick(this.id, this.getAttribute("IDTimeZone"), 'htTableTimeZoneCondition_' + instance_name + '_' + isResult, instance_name, isResult); }
        }

        if ((oNewId % 2) != 0) {
            altRow = "1";
        } else {
            altRow = "2";
        }

        var oTD = oTR.insertCell(-1); //Name
        oTD.className = "GridStyle-cell" + altRow;
        oTD.textContent = Name;

        return true;
    } catch (e) {
        showError("AddListValue", e);
        return false;
    }
}

function rowTimeZoneClick(rowID, ID, dTable, instance_name, isResult) {
    if (isResult == 0) document.getElementById('selectedIdxTimeZone_' + instance_name).value = ID;
    else if (isResult == 1) document.getElementById('selectedIdxTimeZoneValue_' + instance_name).value = ID;

    var tParent = document.getElementById(dTable);
    var tCells = tParent.getElementsByTagName("td");
    for (var i = 0; i < tCells.length; i++) {
        removeCssClass(tCells[i], "gridRowOver");
        removeCssClass(tCells[i], "gridRowSelected");
    }

    var table = document.getElementById(rowID);
    var cells = table.getElementsByTagName("td");
    for (var i = 0; i < cells.length; i++) {
        removeCssClass(cells[i], "gridRowOver");
        addCssClass(cells[i], "gridRowSelected");
    }
}

function RemoveTimeZoneValue(instance_name, isResult, windowID, mustClose) {
    try {
        var IDTimeZone = null;//document.getElementById('selectedIdx').value;
        var hTable = null;//document.getElementById('htTableCondition');

        if (isResult == 0) {
            IDTimeZone = document.getElementById('selectedIdxTimeZone_' + instance_name).value;
            hTable = document.getElementById('htTableTimeZoneCondition_' + instance_name + "_" + isResult);
        } else if (isResult == 1) {
            IDTimeZone = document.getElementById('selectedIdxTimeZoneValue_' + instance_name).value;
            hTable = document.getElementById('htTableTimeZoneCondition_' + instance_name + "_" + isResult);
        }

        //Si no hi ha taula
        if (hTable == null) { return; }
        if (IDTimeZone == "") { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htTimeZoneRows_" + instance_name + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htTimeZoneRows_" + instance_name + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htTimeZoneRows_" + instance_name + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htTimeZoneRows_" + instance_name + '_' + isResult);
                }
                break;
        }

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("IDTimeZone") == IDTimeZone) {
                hTable.deleteRow(n + 1);
                return;
            }
        }
    } catch (e) { showError("RemoveListValue", e); }
}

function showTypeValue(instance, sender, showSecond) {
    document.getElementById('uniqueValue_' + instance).style.display = 'none';
    document.getElementById('causeValue_' + instance).style.display = 'none';
    document.getElementById('between_' + instance).style.display = 'none';

    var cmbType = eval('cmbTypeValueClient_' + instance);
    var textField = eval("txtValueTypeClient_" + instance);
    var userField = eval("cmbCauseUFieldClient_" + instance);

    textField.SetVisible(false);
    userField.SetVisible(false);

    switch (parseInt(sender.GetSelectedItem().value, 10)) {
        case 0:
            textField.SetVisible(true);
            document.getElementById('uniqueValue_' + instance).style.display = '';
            if (showSecond || parseInt(cmbType.GetSelectedItem().value, 10) == 6) {
                document.getElementById('between_' + instance).style.display = '';
            }
            break;
        case 1:
            userField.SetVisible(true);
            document.getElementById('uniqueValue_' + instance).style.display = '';
            break;
        case 2:
            document.getElementById('causeValue_' + instance).style.display = '';
            break;
    }
}

function showCompareValue(instance, sender) {
    var cmbType = eval('cmbTypeValueClient_' + instance);

    if (parseInt(sender.GetSelectedItem().value, 10) == 6) {
        cmbType.SetSelectedIndex(0);
        showTypeValue(instance, cmbType, true);
        cmbType.SetEnabled(false);
    } else {
        showTypeValue(instance, cmbType, false);
        cmbType.SetEnabled(true);
    }
}

function getActiveConditions() {
    var activeConditions = [];

    $("#conditionsAccordion").find('.conditionTab').each(function (index) {
        if (index < conditionsNumber) {
            var actualIndex = parseInt($(this).attr('data-idTab'), 10);
            var iCondition = {};

            iCondition.ConditionCauses = retrieveJSONCausesGrid(actualIndex, 0);
            iCondition.ConditionTimeZones = retrieveJSONTimeZonesGrid(actualIndex, 0);

            var cmbCompare = eval('cmbCompareClient_' + actualIndex);
            iCondition.Compare = parseInt(cmbCompare.GetSelectedItem().value, 10);

            var cmbType = eval('cmbTypeValueClient_' + actualIndex);
            iCondition.Type = parseInt(cmbType.GetSelectedItem().value, 10);

            var textField = eval("txtValueTypeClient_" + actualIndex);
            iCondition.FromValue = textField.GetText();

            var userField = eval("cmbCauseUFieldClient_" + actualIndex);
            if (userField.GetSelectedItem() != null) {
                iCondition.UserField = userField.GetSelectedItem().value;
            } else {
                iCondition.UserField = '';
            }

            var textFieldTo = eval("txtValueTypeToClient_" + actualIndex);
            iCondition.ToValue = textFieldTo.GetText();

            iCondition.CompareCauses = retrieveJSONCausesGrid(actualIndex, 1);
            iCondition.CompareTimeZones = retrieveJSONTimeZonesGrid(actualIndex, 1);

            switch (iCondition.Type) {
                case 0:
                    if (iCondition.Compare != 6) {
                        iCondition.ToValue = "00:00";
                    }
                    iCondition.UserField = '';
                    iCondition.CompareCauses = [];
                    iCondition.CompareTimeZones = [];

                    break;
                case 1:
                    iCondition.FromValue = "00:00";
                    iCondition.ToValue = "00:00";
                    iCondition.CompareCauses = [];
                    iCondition.CompareTimeZones = [];

                    break;
                case 2:
                    iCondition.FromValue = "00:00";
                    iCondition.ToValue = "00:00";
                    iCondition.UserField = '';
                    break;
            }

            activeConditions.push(iCondition);
        }
    });

    return activeConditions;
}

function loadConditions(objConditions) {
    if (objConditions != null) {
        conditionsNumber = objConditions.length;

        for (var index = 0; index < objConditions.length; index++) {
            conditionsEnabled[index] = true;
            $($("#conditionsAccordion").find('.conditionTab')[index]).show();
            var iCondition = objConditions[index];

            var actualIndex = -1;// parseInt($(this).attr('data-idTab'), 10);

            $("#conditionsAccordion").find('.conditionTab').each(function (screenIndex) {
                if (screenIndex == index) {
                    actualIndex = parseInt($(this).attr('data-idTab'), 10);
                }
            });

            if (typeof iCondition.ConditionCauses != 'undefined' && iCondition.ConditionCauses != null) fillJSONCausesGrid(iCondition.ConditionCauses, actualIndex, 0);
            if (typeof iCondition.ConditionTimeZones != 'undefined' && iCondition.ConditionTimeZones != null) fillJSONTimeZonesGrid(iCondition.ConditionTimeZones, actualIndex, 0);

            var cmbCompare = eval('cmbCompareClient_' + actualIndex);
            if (iCondition.Compare >= 0) cmbCompare.SetSelectedItem(cmbCompare.FindItemByValue(iCondition.Compare));
            else cmbCompare.SetSelectedIndex(0);

            var cmbType = eval('cmbTypeValueClient_' + actualIndex);
            if (iCondition.Type >= 0) cmbType.SetSelectedItem(cmbType.FindItemByValue(iCondition.Type));
            else cmbType.SetSelectedIndex(0);

            var textField = eval('txtValueTypeClient_' + actualIndex);
            if (iCondition.FromValue != "") textField.SetText(iCondition.FromValue);
            else textField.SetText('00:00');

            var textFieldTo = eval("txtValueTypeToClient_" + actualIndex);
            if (iCondition.ToValue != "") textFieldTo.SetText(iCondition.ToValue);
            else textFieldTo.SetText('00:00');

            var userField = eval("cmbCauseUFieldClient_" + actualIndex);
            if (iCondition.UserField != '') {
                userField.SetSelectedItem(userField.FindItemByValue(iCondition.UserField));
            } else {
                userField.SetSelectedIndex(0);
            }

            if (typeof iCondition.CompareCauses != 'undefined' && iCondition.CompareCauses != null) fillJSONCausesGrid(iCondition.CompareCauses, actualIndex, 1);
            if (typeof iCondition.CompareTimeZones != 'undefined' && iCondition.CompareTimeZones != null) fillJSONTimeZonesGrid(iCondition.CompareTimeZones, actualIndex, 1);

            showCompareValue(actualIndex, cmbCompare);
        }

        if (conditionsNumber == 3) {
            $('#btnAddCondition').hide();
            $('#btnAddCondition').removeClass('btnFlat');
        }

        if (conditionsNumber > 1) {
            $('#btnRemoveCondition').show();
            $('#btnRemoveCondition').addClass('btnFlat');
        }
    }
}

function retrieveJSONActionCausesGrid(instance, isResult) {
    try {
        var hTable = document.getElementById('htTableAction_' + instance + "_" + isResult);
        var newArr = new Array();

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
            case 'Chrome':
                hRows = document.getElementsByName("htRowsAction_" + instance + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsAction_" + instance + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htRowsAction_" + instance + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsAction_" + instance + '_' + isResult);
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            var oComp = {};
            oComp.IDCause = parseInt(hRow.getAttribute("IDCause"), 10);
            oComp.IDCause2 = parseInt(hRow.getAttribute("IDCause2"), 10);
            oComp.Name = hRow.getAttribute("CauseName");
            oComp.Name2 = hRow.getAttribute("CauseName2");
            newArr.push(oComp);
        }

        return newArr;
    } catch (e) { showError("retrieveJSONActionCausesGrid", e); }
}

function retrieveJSONCausesGrid(instance, isResult) {
    try {
        var hTable = document.getElementById('htTableCondition_' + instance + "_" + isResult);
        var newArr = new Array();

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
            case 'Chrome':
                hRows = document.getElementsByName("htRows_" + instance + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRows_" + instance + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htRows_" + instance + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRows_" + instance + '_' + isResult);
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            var oComp = {};
            oComp.IDCause = parseInt(hRow.getAttribute("IDCause"), 10);
            oComp.Operation = hRow.getAttribute("Operation");
            oComp.Name = hRow.getAttribute("CauseName");
            newArr.push(oComp);
        }

        return newArr;
    } catch (e) { showError("retrieveJSONCausesGrid", e); }
}

function retrieveJSONTimeZonesGrid(instance, isResult) {
    try {
        var hTable = document.getElementById('htTableTimeZoneCondition_' + instance + "_" + isResult);
        var newArr = new Array();

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
            case 'Chrome':
                hRows = document.getElementsByName("htTimeZoneRows_" + instance + '_' + isResult);
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htTimeZoneRows_" + instance + '_' + isResult);
                break;
            default:
                hRows = document.getElementsByName("htTimeZoneRows_" + instance + '_' + isResult);
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htTimeZoneRows_" + instance + '_' + isResult);
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            var oComp = {};
            oComp.IDTimeZone = parseInt(hRow.getAttribute("IDTimeZone"), 10);
            oComp.Name = hRow.getAttribute("TimeZoneName");

            newArr.push(oComp);
        }

        return newArr;
    } catch (e) { showError("retrieveJSONTimeZonesGrid", e); }
}

function fillJSONCausesGridAction(elements, instance_name, isResult) {
    try {
        var tableName = 'divActionsCauses_';
        if (isResult == 1) tableName = 'divActionsCausesValue_';

        var oTable = document.getElementById('htTableAction_' + instance_name + "_" + isResult);
        var altRow = 1;

        for (var index = 0; index < elements.length; index++) {
            var IDCause = elements[index].IDCause;
            var IDCause2 = elements[index].IDCause2;
            var CauseName = elements[index].Name;
            var CauseName2 = elements[index].Name2;

            var oNewId = oTable.rows.length;

            /*Afegim el tr */
            var oTR = oTable.insertRow(-1);
            oTR.id = 'htRowAction_' + instance_name + '_' + isResult + '_' + oNewId;
            oTR.setAttribute("name", "htRowsAction_" + instance_name + '_' + isResult);
            oTR.setAttribute("IDCause", IDCause);
            oTR.setAttribute("IDCause2", IDCause2);
            oTR.setAttribute("CauseName", CauseName);
            oTR.setAttribute("CauseName2", CauseName2);

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                oTR.setAttribute("onmouseover", "javascript: rowOver('" + oTR.id + "');");
                oTR.setAttribute("onmouseout", "javascript: rowOut('" + oTR.id + "');");
                oTR.setAttribute("onclick", "javascript: rowClick('" + oTR.id + "','" + IDCause + "','htTableAction_" + instance_name + "_" + isResult + "'," + instance_name + "," + isResult + ");");
            } else { // IE
                oTR.onmouseover = function () { rowOver(this.id); }
                oTR.onmouseout = function () { rowOut(this.id); }
                oTR.onclick = function () { rowClick(this.id, this.getAttribute("IDCause"), 'htTableAction_' + instance_name + '_' + isResult, instance_name, isResult); }
            }

            if ((oNewId % 2) != 0) {
                altRow = "1";
            } else {
                altRow = "2";
            }

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cell" + altRow;
            oTD.textContent = CauseName;

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cell" + altRow;
            oTD.textContent = CauseName2;
        }
    } catch (e) { showError("fillJSONCausesGridAction", e); }
}

function fillJSONCausesGrid(elements, instance_name, isResult) {
    try {
        var tableName = 'divConditionsCauses_';
        if (isResult == 1) tableName = 'divConditionsCausesValue_';

        var oTable = document.getElementById('htTableCondition_' + instance_name + "_" + isResult);
        var altRow = 1;

        for (var index = 0; index < elements.length; index++) {
            var IDCause = elements[index].IDCause;
            var Operation = elements[index].Operation;
            var CauseName = elements[index].Name;

            var oNewId = oTable.rows.length;

            /*Afegim el tr */
            var oTR = oTable.insertRow(-1);
            oTR.id = 'htRow_' + instance_name + '_' + isResult + '_' + oNewId;
            oTR.setAttribute("name", "htRows_" + instance_name + '_' + isResult);
            oTR.setAttribute("IDCause", IDCause);
            oTR.setAttribute("CauseName", CauseName);
            oTR.setAttribute("Operation", Operation);

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                oTR.setAttribute("onmouseover", "javascript: rowOver('" + oTR.id + "');");
                oTR.setAttribute("onmouseout", "javascript: rowOut('" + oTR.id + "');");
                oTR.setAttribute("onclick", "javascript: rowClick('" + oTR.id + "','" + IDCause + "','htTableCondition_" + instance_name + "_" + isResult + "'," + instance_name + "," + isResult + ");");
            } else { // IE
                oTR.onmouseover = function () { rowOver(this.id); }
                oTR.onmouseout = function () { rowOut(this.id); }
                oTR.onclick = function () { rowClick(this.id, this.getAttribute("IDCause"), 'htTableCondition_' + instance_name + '_' + isResult, instance_name, isResult); }
            }

            if ((oNewId % 2) != 0) {
                altRow = "1";
            } else {
                altRow = "2";
            }

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cell" + altRow;

            if (Operation == "0") {
                oTD.textContent = document.getElementById('OperationPlus').value;
            } else {
                oTD.textContent = document.getElementById('OperationMinus').value;
            }

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cell" + altRow;
            oTD.textContent = CauseName;
        }
    } catch (e) { showError("fillJSONCausesGrid", e); }
}

function fillJSONTimeZonesGrid(elements, instance_name, isResult) {
    try {
        var tableName = 'divConditionTimeZones_';
        if (isResult == 1) tableName = 'divConditionTimeZonesValue_';
        var oTable = document.getElementById('htTableTimeZoneCondition_' + instance_name + "_" + isResult);
        var altRow = 1;

        for (var index = 0; index < elements.length; index++) {
            var oNewId = oTable.rows.length;
            var IDTimeZone = elements[index].IDTimeZone;
            var TimeZoneName = elements[index].Name;

            /*Afegim el tr */
            var oTR = oTable.insertRow(-1);
            oTR.id = 'htTimeZoneRow_' + instance_name + '_' + isResult + '_' + oNewId;
            oTR.setAttribute("name", "htTimeZoneRows_" + instance_name + '_' + isResult);
            oTR.setAttribute("IDTimeZone", IDTimeZone);
            oTR.setAttribute("TimeZoneName", TimeZoneName);

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                oTR.setAttribute("onmouseover", "javascript: rowOver('" + oTR.id + "');");
                oTR.setAttribute("onmouseout", "javascript: rowOut('" + oTR.id + "');");
                oTR.setAttribute("onclick", "javascript: rowTimeZoneClick('" + oTR.id + "','" + IDTimeZone + "','htTableTimeZoneCondition_" + instance_name + "_" + isResult + "'," + instance_name + "," + isResult + ");");
            } else { // IE
                oTR.onmouseover = function () { rowOver(this.id); }
                oTR.onmouseout = function () { rowOut(this.id); }
                oTR.onclick = function () { rowTimeZoneClick(this.id, this.getAttribute("IDTimeZone"), 'htTableTimeZoneCondition_' + instance_name + '_' + isResult, instance_name, isResult); }
            }

            if ((oNewId % 2) != 0) {
                altRow = "1";
            } else {
                altRow = "2";
            }

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cell" + altRow;
            oTD.textContent = TimeZoneName;
        }
    } catch (e) { showError("fillJSONTimeZonesGrid", e); }
}

function showActionConfiguration(instance, sender, initialLoad) {
    document.getElementById('carryover_' + instance).style.display = 'none';
    document.getElementById('plus_' + instance).style.display = 'none';
    document.getElementById('carryoversingle_' + instance).style.display = 'none';

    if (parseInt(sender.GetSelectedItem().value, 10) == 0) {
        document.getElementById('carryover_' + instance).style.display = '';

        var carryOverConfig = eval("cmbCarryOverActionClient_" + instance);
        if (initialLoad) carryOverConfig.SetSelectedIndex(0);
        showCarryOverConfiguration(instance, carryOverConfig, initialLoad);
    } else if (parseInt(sender.GetSelectedItem().value, 10) == 1) {
        document.getElementById('plus_' + instance).style.display = '';

        var plusConfig = eval("cmbPlusActionsClient_" + instance);
        if (initialLoad) plusConfig.SetSelectedIndex(0);
        showPlusConfiguration(instance, plusConfig, initialLoad);
    }
    else {
        document.getElementById('carryoversingle_' + instance).style.display = '';

        var carryOverSingleConfig = eval("cmbCarryOverSingleActionClient_" + instance);
        if (initialLoad) carryOverSingleConfig.SetSelectedIndex(0);
        showCarryOverSingleConfiguration(instance, carryOverSingleConfig, initialLoad);
    }
}

function showPlusConfiguration(instance, sender, initialLoad) {
    document.getElementById('plusStep1_' + instance).style.display = '';

    document.getElementById('plusStep1PlusAction' + instance).style.display = '';
    document.getElementById('plusStep1PlusActionResult' + instance).style.display = 'none';
    document.getElementById('plusStep1Sign' + instance).style.display = 'none';

    document.getElementById('plusStep2_' + instance).style.display = 'none';

    document.getElementById('plusStep2PlusValue_' + instance).style.display = 'none';
    document.getElementById('ActionDirectValue_' + instance).style.display = 'none';
    document.getElementById('ActionDifference_' + instance).style.display = 'none';
    document.getElementById('ActionUserField_' + instance).style.display = 'none';

    document.getElementById('plusStep2PlusValueResult_' + instance).style.display = 'none';
    document.getElementById('ActionDirectValueResult_' + instance).style.display = 'none';
    document.getElementById('ActionDifferenceResult_' + instance).style.display = 'none';
    document.getElementById('ActionUserFieldResult_' + instance).style.display = 'none';

    if (parseInt(sender.GetSelectedItem().value, 10) == 0) {
        document.getElementById('plusStep2_' + instance).style.display = '';
        document.getElementById('plusStep2PlusValue_' + instance).style.display = '';
        document.getElementById('ActionDirectValue_' + instance).style.display = '';
    } else if (parseInt(sender.GetSelectedItem().value, 10) == 1) {
        document.getElementById('plusStep2_' + instance).style.display = '';
        document.getElementById('plusStep2PlusValue_' + instance).style.display = '';
        document.getElementById('ActionUserField_' + instance).style.display = '';
    } else if (parseInt(sender.GetSelectedItem().value, 10) == 2) {
        document.getElementById('plusStep2_' + instance).style.display = '';
        document.getElementById('plusStep2PlusValue_' + instance).style.display = '';
        document.getElementById('ActionDifference_' + instance).style.display = '';
        document.getElementById('plusStep1PlusActionResult' + instance).style.display = '';
        document.getElementById('plusStep1Sign' + instance).style.display = '';

        var plusResultConfig = eval("cmbPlusResultActionsClient_" + instance);
        if (initialLoad) plusResultConfig.SetSelectedIndex(0);
        showPlusResultConfiguration(instance, plusResultConfig);
    }
}

function showCauseValueByType(instance, sender) {
    var typeCause = parseInt(sender.GetSelectedItem().value.split("_")[1], 10);

    if (typeCause == 0) {
        document.getElementById('ActionDirectValue_Time_' + instance).style.display = '';
        document.getElementById('ActionDirectValue_Number_' + instance).style.display = 'none';
        document.getElementById('ActionDirectValueResult_Time_' + instance).style.display = '';
        document.getElementById('ActionDirectValueResult_Number_' + instance).style.display = 'none';
    } else {
        document.getElementById('ActionDirectValue_Time_' + instance).style.display = 'none';
        document.getElementById('ActionDirectValue_Number_' + instance).style.display = '';
        document.getElementById('ActionDirectValueResult_Time_' + instance).style.display = 'none';
        document.getElementById('ActionDirectValueResult_Number_' + instance).style.display = '';
    }
}

function showPlusResultConfiguration(instance, sender) {
    document.getElementById('plusStep2PlusValueResult_' + instance).style.display = 'none';
    document.getElementById('ActionDirectValueResult_' + instance).style.display = 'none';
    document.getElementById('ActionDifferenceResult_' + instance).style.display = 'none';
    document.getElementById('ActionUserFieldResult_' + instance).style.display = 'none';

    if (parseInt(sender.GetSelectedItem().value, 10) == 0) {
        document.getElementById('plusStep2PlusValueResult_' + instance).style.display = '';
        document.getElementById('ActionDirectValueResult_' + instance).style.display = '';
    } else if (parseInt(sender.GetSelectedItem().value, 10) == 1) {
        document.getElementById('plusStep2PlusValueResult_' + instance).style.display = '';
        document.getElementById('ActionUserFieldResult_' + instance).style.display = '';
    } else if (parseInt(sender.GetSelectedItem().value, 10) == 2) {
        document.getElementById('plusStep2PlusValueResult_' + instance).style.display = '';
        document.getElementById('ActionDifferenceResult_' + instance).style.display = '';
    }
}

function showCarryOverConfiguration(instance, sender, initialLoad) {
    document.getElementById('carryOverStep0_' + instance).style.display = '';
    document.getElementById('carryOverStep1_' + instance).style.display = '';

    document.getElementById('carryOverAction_' + instance).style.display = '';
    document.getElementById('carryOverActionResult_' + instance).style.display = 'none';
    document.getElementById('carryOverCauseFrom_' + instance).style.display = '';
    document.getElementById('carryOverCauseTo_' + instance).style.display = '';

    document.getElementById('CarryOverDirectValue_' + instance).style.display = 'none';
    document.getElementById('CarryOverDifference_' + instance).style.display = 'none';
    document.getElementById('CarryOverUserField_' + instance).style.display = 'none';

    document.getElementById('CarryOverDirectValueResult_' + instance).style.display = 'none';
    document.getElementById('CarryOverDifferenceResult_' + instance).style.display = 'none';
    document.getElementById('CarryOverUserFieldResult_' + instance).style.display = 'none';

    if (parseInt(sender.GetSelectedItem().value, 10) == 0) {
        document.getElementById('CarryOverDirectValue_' + instance).style.display = '';
    } else if (parseInt(sender.GetSelectedItem().value, 10) == 1) {
        document.getElementById('CarryOverUserField_' + instance).style.display = '';
    } else if (parseInt(sender.GetSelectedItem().value, 10) == 2) {
        document.getElementById('CarryOverDifference_' + instance).style.display = '';
        document.getElementById('carryOverActionResult_' + instance).style.display = '';
        var carryOverResultConfig = eval("cmbCarryOverActionResultClient_" + instance);
        if (initialLoad) carryOverResultConfig.SetSelectedIndex(0);
        showCarryOverResultConfiguration(instance, carryOverResultConfig);
    }
}

function showCarryOverResultConfiguration(instance, sender) {
    document.getElementById('CarryOverDirectValueResult_' + instance).style.display = 'none';
    document.getElementById('CarryOverDifferenceResult_' + instance).style.display = 'none';
    document.getElementById('CarryOverUserFieldResult_' + instance).style.display = 'none';

    if (parseInt(sender.GetSelectedItem().value, 10) == 0) {
        document.getElementById('CarryOverDirectValueResult_' + instance).style.display = '';
    } else if (parseInt(sender.GetSelectedItem().value, 10) == 1) {
        document.getElementById('CarryOverUserFieldResult_' + instance).style.display = '';
    } else if (parseInt(sender.GetSelectedItem().value, 10) == 2) {
        document.getElementById('CarryOverDifferenceResult_' + instance).style.display = '';
    }
}

function showCarryOverSingleConfiguration(instance, sender, initialLoad) {
    document.getElementById('carryOverSingleStep0_' + instance).style.display = '';
    document.getElementById('carryOverSingleAction_' + instance).style.display = '';
    //document.getElementById('carryOverActionCauses_' + instance).style.display = '';
}

function showCarryOverSingleCausesConfiguration(instance, sender) {
}

function getActiveActions() {
    var activeActions = [];
    $("#actionsAccordion").find('.actionTab').each(function (index) {
        if (index < actionsNumber) {
            var actualIndex = parseInt($(this).attr('data-idTab'), 10);

            var iAction = {};

            iAction.Action = parseInt(eval('cmbActionsClient_' + actualIndex).GetSelectedItem().value, 10);

            /* CarryOver  Action = 0 */

            if (iAction.Action == 0) {
                iAction.CarryOverAction = parseInt(eval("cmbCarryOverActionClient_" + actualIndex).GetSelectedItem().value, 10);

                switch (iAction.CarryOverAction) {
                    case 0:
                        iAction.CarryOverDirectValue = eval("txtCarryOverValueClient_" + actualIndex).GetText();
                        iAction.CarryOverUserFieldValue = "";

                        iAction.CarryOverConditionPart = 0;
                        iAction.CarryOverConditionNumber = 0;

                        iAction.CarryOverActionResult = 0;
                        iAction.CarryOverDirectValueResult = "00:00";
                        iAction.CarryOverUserFieldValueResult = "";
                        iAction.CarryOverConditionPartResult = 0;
                        iAction.CarryOverConditionNumberResult = 0;

                        break;
                    case 1:
                        iAction.CarryOverDirectValue = "00:00";

                        if (eval("cmbCauseUFieldCarryOverClient_" + actualIndex).GetSelectedItem() != null) {
                            iAction.CarryOverUserFieldValue = eval("cmbCauseUFieldCarryOverClient_" + actualIndex).GetSelectedItem().value;
                        } else {
                            iAction.CarryOverUserFieldValue = "";
                        }

                        iAction.CarryOverConditionPart = 0;
                        iAction.CarryOverConditionNumber = 0;

                        iAction.CarryOverActionResult = 0;
                        iAction.CarryOverDirectValueResult = "00:00";
                        iAction.CarryOverUserFieldValueResult = "";
                        iAction.CarryOverConditionPartResult = 0;
                        iAction.CarryOverConditionNumberResult = 0;

                        break;
                    case 2:
                        iAction.CarryOverDirectValue = "00:00";
                        iAction.CarryOverUserFieldValue = "";

                        iAction.CarryOverConditionPart = parseInt(eval("cmbCarryOverConditionPartClient_" + actualIndex).GetSelectedItem().value, 10);
                        iAction.CarryOverConditionNumber = parseInt(eval("cmbCarryOverConditionNumberClient_" + actualIndex).GetSelectedItem().value, 10);

                        iAction.CarryOverActionResult = parseInt(eval("cmbCarryOverActionResultClient_" + actualIndex).GetSelectedItem().value, 10);

                        switch (iAction.CarryOverActionResult) {
                            case 0:
                                iAction.CarryOverDirectValueResult = eval("txtCarryOverResultValueClient_" + actualIndex).GetText();
                                iAction.CarryOverUserFieldValueResult = "";
                                iAction.CarryOverConditionPartResult = 0;
                                iAction.CarryOverConditionNumberResult = 0;
                                break;
                            case 1:
                                iAction.CarryOverDirectValueResult = "00:00";
                                if (eval("cmbCauseUFieldCarryOverResultClient_" + actualIndex).GetSelectedItem() != null) {
                                    iAction.CarryOverUserFieldValueResult = eval("cmbCauseUFieldCarryOverResultClient_" + actualIndex).GetSelectedItem().value;
                                } else {
                                    iAction.CarryOverUserFieldValueResult = "";
                                }
                                iAction.CarryOverConditionPartResult = 0;
                                iAction.CarryOverConditionNumberResult = 0;
                                break;
                            case 2:
                                iAction.CarryOverDirectValueResult = "00:00";
                                iAction.CarryOverUserFieldValueResult = "";

                                iAction.CarryOverConditionPartResult = parseInt(eval("cmbCarryOverConditionPartResultClient_" + actualIndex).GetSelectedItem().value, 10);
                                iAction.CarryOverConditionNumberResult = parseInt(eval("cmbCarryOverConditionNumberResultClient_" + actualIndex).GetSelectedItem().value, 10);

                                break;
                        }
                        break;
                }

                iAction.CarryOverIDCauseFrom = parseInt(eval("cmbCarryOverCauseFromClient_" + actualIndex).GetSelectedItem().value, 10);
                iAction.CarryOverIDCauseTo = parseInt(eval("cmbCarryOverCauseToClient_" + actualIndex).GetSelectedItem().value, 10);
            } else {
                iAction.CarryOverDirectValue = "00:00";
                iAction.CarryOverUserFieldValue = "";

                iAction.CarryOverConditionPart = 0;
                iAction.CarryOverConditionNumber = 0;

                iAction.CarryOverActionResult = 0;
                iAction.CarryOverDirectValueResult = "00:00";
                iAction.CarryOverUserFieldValueResult = "";
                iAction.CarryOverConditionPartResult = 0;
                iAction.CarryOverConditionNumberResult = 0;

                iAction.CarryOverIDCauseFrom = 0;
                iAction.CarryOverIDCauseTo = 0;
            }

            /* Plus Action=1*/
            if (iAction.Action == 1) {
                iAction.PlusIDCause = parseInt(eval('cmbPlusCauseClient_' + actualIndex).GetSelectedItem().value.split("_")[0], 10);
                iAction.PlusAction = parseInt(eval("cmbPlusActionsClient_" + actualIndex).GetSelectedItem().value, 10);

                switch (iAction.PlusAction) {
                    case 0:
                        if (parseInt(eval('cmbPlusCauseClient_' + actualIndex).GetSelectedItem().value.split("_")[1], 10) == 0)
                            iAction.PlusDirectValue = eval("txtPlusValueTimeClient_" + actualIndex).GetText();
                        else
                            iAction.PlusDirectValue = eval("txtPlusValueNumberClient_" + actualIndex).GetText();

                        iAction.PlusUserFieldValue = "";
                        iAction.PlusConditionPart = 0;
                        iAction.PlusConditionNumber = 0;
                        iAction.PlusActionResult = 0;

                        iAction.PlusDirectValueResult = "00:00";
                        iAction.PlusUserFieldValueResult = "";
                        iAction.PlusConditionPartResult = 0;
                        iAction.PlusConditionNumberResult = 0;
                        break;
                    case 1:
                        iAction.PlusDirectValue = "00:00";
                        if (eval("cmbCauseUFieldPlusClient_" + actualIndex).GetSelectedItem() != null) {
                            iAction.PlusUserFieldValue = eval("cmbCauseUFieldPlusClient_" + actualIndex).GetSelectedItem().value;
                        } else {
                            iAction.PlusUserFieldValue = "";
                        }
                        iAction.PlusConditionPart = 0;
                        iAction.PlusConditionNumber = 0;
                        iAction.PlusActionResult = 0;

                        iAction.PlusDirectValueResult = "00:00";
                        iAction.PlusUserFieldValueResult = "";
                        iAction.PlusConditionPartResult = 0;
                        iAction.PlusConditionNumberResult = 0;
                        break;
                    case 2:
                        iAction.PlusDirectValue = "00:00";
                        iAction.PlusUserFieldValue = "";
                        iAction.PlusConditionPart = parseInt(eval("cmbPlusConditionPartClient_" + actualIndex).GetSelectedItem().value, 10);
                        iAction.PlusConditionNumber = parseInt(eval("cmbPlusConditionNumberClient_" + actualIndex).GetSelectedItem().value, 10);

                        iAction.PlusActionResult = parseInt(eval("cmbPlusResultActionsClient_" + actualIndex).GetSelectedItem().value, 10);
                        switch (iAction.PlusActionResult) {
                            case 0:
                                if (parseInt(eval('cmbPlusCauseClient_' + actualIndex).GetSelectedItem().value.split("_")[1], 10) == 0)
                                    iAction.PlusDirectValueResult = eval("txtPlusValueResultTimeClient_" + actualIndex).GetText();
                                else
                                    iAction.PlusDirectValueResult = eval("txtPlusValueResultNumberClient_" + actualIndex).GetText();

                                iAction.PlusUserFieldValueResult = "";

                                iAction.PlusConditionPartResult = 0;
                                iAction.PlusConditionNumberResult = 0;
                                break;
                            case 1:
                                iAction.PlusDirectValueResult = "00:00";
                                if (eval("cmbCauseUFieldPlusResultClient_" + actualIndex).GetSelectedItem() != null) {
                                    iAction.PlusUserFieldValueResult = eval("cmbCauseUFieldPlusResultClient_" + actualIndex).GetSelectedItem().value;
                                } else {
                                    iAction.PlusUserFieldValueResult = "";
                                }

                                iAction.PlusConditionPartResult = 0;
                                iAction.PlusConditionNumberResult = 0;

                                break;
                            case 2:
                                iAction.PlusDirectValueResult = "00:00";
                                iAction.PlusUserFieldValueResult = "";
                                iAction.PlusConditionPartResult = parseInt(eval("cmbPlusConditionPartResultClient_" + actualIndex).GetSelectedItem().value, 10);
                                iAction.PlusConditionNumberResult = parseInt(eval("cmbPlusConditionNumberResultClient_" + actualIndex).GetSelectedItem().value, 10);
                                break;
                        }
                        break;
                }

                iAction.PlusActionSign = eval("cmbPlusSignClient_" + actualIndex).GetSelectedItem().value;
            } else {
                iAction.PlusIDCause = 0;
                iAction.PlusAction = 0;

                iAction.PlusDirectValue = "00:00";

                iAction.PlusUserFieldValue = "";
                iAction.PlusConditionPart = 0;
                iAction.PlusConditionNumber = 0;
                iAction.PlusActionResult = 0;

                iAction.PlusDirectValueResult = "00:00";
                iAction.PlusUserFieldValueResult = "";
                iAction.PlusConditionPartResult = 0;
                iAction.PlusConditionNumberResult = 0;
            }

            if (iAction.Action == 2) {
                iAction.CarryOverSingleCause = parseInt(eval("cmbCarryOverSingleActionCausesClient_" + actualIndex).GetSelectedItem().value, 10);
                iAction.ActionCauses = retrieveJSONActionCausesGrid(actualIndex, 0);
            }
            else {
            }

            activeActions.push(iAction);
        }
    });
    return activeActions;
}

function loadActions(objActions) {
    if (objActions != null) {
        actionsNumber = objActions.length;
        for (var index = 0; index < objActions.length; index++) {
            actionsEnabled[index] = true;
            $($("#actionsAccordion").find('.actionTab')[index]).show();
            var iAction = objActions[index];

            var actualIndex = -1;// parseInt($(this).attr('data-idTab'), 10);

            $("#actionsAccordion").find('.actionTab').each(function (screenIndex) {
                if (screenIndex == index) {
                    actualIndex = parseInt($(this).attr('data-idTab'), 10);
                }
            });

            eval('cmbActionsClient_' + actualIndex).SetSelectedItem(eval('cmbActionsClient_' + actualIndex).FindItemByValue(iAction.Action));

            /* CarryOver */
            eval("cmbCarryOverActionClient_" + actualIndex).SetSelectedItem(eval("cmbCarryOverActionClient_" + actualIndex).FindItemByValue(iAction.CarryOverAction));
            eval("txtCarryOverValueClient_" + actualIndex).SetText(iAction.CarryOverDirectValue);
            eval("cmbCarryOverConditionPartClient_" + actualIndex).SetSelectedItem(eval("cmbCarryOverConditionPartClient_" + actualIndex).FindItemByValue(iAction.CarryOverConditionPart));
            eval("cmbCarryOverConditionNumberClient_" + actualIndex).SetSelectedItem(eval("cmbCarryOverConditionNumberClient_" + actualIndex).FindItemByValue(iAction.CarryOverConditionNumber));
            if (iAction.CarryOverUserFieldValueResult != "") {
                eval("cmbCauseUFieldCarryOverClient_" + actualIndex).SetSelectedItem(eval("cmbCauseUFieldCarryOverClient_" + actualIndex).FindItemByValue(iAction.CarryOverUserFieldValue));
            } else {
                eval("cmbCauseUFieldCarryOverClient_" + actualIndex).SetSelectedIndex(0);
            }

            eval("cmbCarryOverActionResultClient_" + actualIndex).SetSelectedItem(eval("cmbCarryOverActionResultClient_" + actualIndex).FindItemByValue(iAction.CarryOverActionResult));
            eval("txtCarryOverResultValueClient_" + actualIndex).SetText(iAction.CarryOverDirectValueResult);
            eval("cmbCarryOverConditionPartResultClient_" + actualIndex).SetSelectedItem(eval("cmbCarryOverConditionPartResultClient_" + actualIndex).FindItemByValue(iAction.CarryOverConditionPartResult));
            eval("cmbCarryOverConditionNumberResultClient_" + actualIndex).SetSelectedItem(eval("cmbCarryOverConditionNumberResultClient_" + actualIndex).FindItemByValue(iAction.CarryOverConditionNumberResult));
            if (iAction.CarryOverUserFieldValueResult != "") {
                eval("cmbCauseUFieldCarryOverResultClient_" + actualIndex).SetSelectedItem(eval("cmbCauseUFieldCarryOverResultClient_" + actualIndex).FindItemByValue(iAction.CarryOverUserFieldValueResult));
            } else {
                eval("cmbCauseUFieldCarryOverResultClient_" + actualIndex).SetSelectedIndex(0);
            }

            var sItem = eval("cmbCarryOverCauseFromClient_" + actualIndex).FindItemByValue(iAction.CarryOverIDCauseFrom)
            if (sItem != null) eval("cmbCarryOverCauseFromClient_" + actualIndex).SetSelectedItem(sItem);
            else eval("cmbCarryOverCauseFromClient_" + index).SetSelectedIndex(0);

            sItem = eval("cmbCarryOverCauseToClient_" + actualIndex).FindItemByValue(iAction.CarryOverIDCauseTo);
            if (sItem != null) eval("cmbCarryOverCauseToClient_" + actualIndex).SetSelectedItem(sItem);
            else eval("cmbCarryOverCauseToClient_" + index).SetSelectedIndex(0);

            /* Plus */
            var plusCauseType = 0;
            if (eval('cmbPlusCauseClient_' + actualIndex).FindItemByValue(iAction.PlusIDCause + "_0") != null) {
                eval('cmbPlusCauseClient_' + actualIndex).SetSelectedItem(eval('cmbPlusCauseClient_' + actualIndex).FindItemByValue(iAction.PlusIDCause + "_0"));
                plusCauseType = 0;
            } else {
                eval('cmbPlusCauseClient_' + actualIndex).SetSelectedItem(eval('cmbPlusCauseClient_' + actualIndex).FindItemByValue(iAction.PlusIDCause + "_1"));
                plusCauseType = 1;
            }

            eval("cmbPlusActionsClient_" + actualIndex).SetSelectedItem(eval("cmbPlusActionsClient_" + actualIndex).FindItemByValue(iAction.PlusAction))

            if (plusCauseType == 0) {
                eval("txtPlusValueTimeClient_" + actualIndex).SetText(iAction.PlusDirectValue);
                eval("txtPlusValueResultTimeClient_" + actualIndex).SetText(iAction.PlusDirectValueResult);

                document.getElementById('ActionDirectValue_Time_' + actualIndex).style.display = '';
                document.getElementById('ActionDirectValue_Number_' + actualIndex).style.display = 'none';
                document.getElementById('ActionDirectValueResult_Time_' + actualIndex).style.display = '';
                document.getElementById('ActionDirectValueResult_Number_' + actualIndex).style.display = 'none';
            }
            if (plusCauseType == 1) {
                eval("txtPlusValueNumberClient_" + actualIndex).SetText(iAction.PlusDirectValue);
                eval("txtPlusValueResultNumberClient_" + actualIndex).SetText(iAction.PlusDirectValueResult);

                document.getElementById('ActionDirectValue_Time_' + actualIndex).style.display = 'none';
                document.getElementById('ActionDirectValue_Number_' + actualIndex).style.display = '';
                document.getElementById('ActionDirectValueResult_Time_' + actualIndex).style.display = 'none';
                document.getElementById('ActionDirectValueResult_Number_' + actualIndex).style.display = '';
            }

            eval("cmbPlusConditionPartClient_" + actualIndex).SetSelectedItem(eval("cmbPlusConditionPartClient_" + actualIndex).FindItemByValue(iAction.PlusConditionPart));
            eval("cmbPlusConditionNumberClient_" + actualIndex).SetSelectedItem(eval("cmbPlusConditionNumberClient_" + actualIndex).FindItemByValue(iAction.PlusConditionNumber));

            if (iAction.PlusUserFieldValue != "") {
                eval("cmbCauseUFieldPlusClient_" + actualIndex).SetSelectedItem(eval("cmbCauseUFieldPlusClient_" + actualIndex).FindItemByValue(iAction.PlusUserFieldValue))
            } else {
                eval("cmbCauseUFieldPlusClient_" + actualIndex).SetSelectedIndex(0);
            }

            eval("cmbPlusResultActionsClient_" + actualIndex).SetSelectedItem(eval("cmbPlusResultActionsClient_" + actualIndex).FindItemByValue(iAction.PlusActionResult));

            eval("cmbPlusConditionPartResultClient_" + actualIndex).SetSelectedItem(eval("cmbPlusConditionPartResultClient_" + actualIndex).FindItemByValue(iAction.PlusConditionPartResult));
            eval("cmbPlusConditionNumberResultClient_" + actualIndex).SetSelectedItem(eval("cmbPlusConditionNumberResultClient_" + actualIndex).FindItemByValue(iAction.PlusConditionNumberResult));

            if (iAction.PlusUserFieldValueResult != "") {
                eval("cmbCauseUFieldPlusResultClient_" + actualIndex).SetSelectedItem(eval("cmbCauseUFieldPlusResultClient_" + actualIndex).FindItemByValue(iAction.PlusUserFieldValueResult));
            } else {
                eval("cmbCauseUFieldPlusResultClient_" + actualIndex).SetSelectedIndex(0);
            }

            eval("cmbPlusSignClient_" + actualIndex).SetSelectedItem(eval("cmbPlusSignClient_" + actualIndex).FindItemByValue(iAction.PlusActionSign));

            if (iAction.CarryOverSingleCause != "") {
                eval("cmbCarryOverSingleActionCausesClient_" + actualIndex).SetSelectedItem(eval("cmbCarryOverSingleActionCausesClient_" + actualIndex).FindItemByValue(iAction.CarryOverSingleCause))
            } else {
                eval("cmbCarryOverSingleActionCausesClient_" + actualIndex).SetSelectedIndex(0);
            }

            if (typeof iAction.ActionCauses != 'undefined' && iAction.ActionCauses != null) fillJSONCausesGridAction(iAction.ActionCauses, actualIndex, 0);

            showActionConfiguration(actualIndex, eval('cmbActionsClient_' + index), false);
        }

        if (actionsNumber == 3) {
            $('#btnAddAction').hide();
            $('#btnAddAction').removeClass('btnFlat');
        }

        if (actionsNumber > 1) {
            $('#btnRemoveAction').show();
            $('#btnRemoveAction').addClass('btnFlat');
        }
    }
}

function FrmDailyRule_ConvertHoursToMinutes(Hours) {
    try {
        var isNegative = false;

        if (Hours.startsWith("-")) {
            isNegative = true
            Hours = Hours.substr(1);
        }
        var Hour = parseInt(Hours.split(":")[0], 10);
        var Minutes = parseInt(Hours.split(":")[1], 10);
        var oResult = 0;

        oResult = Hour * 60;
        oResult = parseInt(oResult + Minutes);

        if (isNegative) oResult = oResult * -1;
        return oResult;
    } catch (e) { showError("FrmDailyRule_ConvertHoursToMinutes", e); return 0; }
}

//#endregion