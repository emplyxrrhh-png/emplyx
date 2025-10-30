var actualScheduleRuleId = -1;

function showActualIdContractScheduleRules() {
    showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEmployeeScheduleRules', true);
    initScheduleRuleControl('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEmployeeScheduleRules', false, true);
}

function closeActualIdContractScheduleRules() {
    showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEmployeeScheduleRules', false);
}

function QueryEmployeeRules_BeginCallback(e, c) {
}

function QueryEmployeeRules_EndCallback(s, e) {
    if (s.IsEditing()) {
        hasChanges(true);
    } else {
        if (s.cpAction == "ROWINSERTING" || s.cpAction == "ROWUPDATING" || s.cpAction == "ROWDELETE") {
            hasChanges(true);
        }
    }
}

function QueryEmployeeRules_OnRowDblClick(s, e) {
    QueryEmployeeRulesClient.GetRowValues(e.visibleIndex, 'Id', loadQueryScheduleRules);
}

function QueryEmployeeRules_FocusedRowChanged(s, e) {
}

function QueryEmployeeRules_CustomButtonClick(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        QueryEmployeeRulesClient.GetRowValues(e.visibleIndex, 'Id', loadQueryScheduleRules);
    }
}

function loadQueryScheduleRules(id) {
    actualScheduleRuleId = id;
    var oParameters = {};
    oParameters.aTab = 1;
    oParameters.ID = actualScheduleRuleId;
    oParameters.IDLabAgree = -1;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETLABAGREERESCHEDULERULE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxScheduleRulesCallbackPanelContenidoClient.PerformCallback(strParameters);
}