function GridLabAgreeRules_BeginCallback(e, c) {
}

function GridLabAgreeRules_EndCallback(s, e) {
    if (s.IsEditing()) {
        hasChanges(true);
    } else {
        if (s.cpAction == "ROWINSERTING" || s.cpAction == "ROWUPDATING" || s.cpAction == "ROWDELETE") {
            hasChanges(true);
        }
    }
}

function GridLabAgreeRules_OnRowDblClick(s, e) {
    GridLabAgreeRulesClient.GetRowValues(e.visibleIndex, 'IDAccrualRule', loadAccrualRule);
}

function GridLabAgreeRules_FocusedRowChanged(s, e) {
}

function GridLabAgreeRules_CustomButtonClick(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        GridLabAgreeRulesClient.GetRowValues(e.visibleIndex, 'IDAccrualRule', loadAccrualRule);
    }
}

function loadAccrualRule(id) {
    actualLabAgreeRule = id;
    var oParameters = {};
    oParameters.aTab = 1
    oParameters.ID = actualLabAgreeRule;
    oParameters.IDLabAgree = actualLabAgrees;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETLABAGREERULE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxRuleCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Agregar nueva fila en el grid de incidencias
function AddNewLabAgreeRule(s, e) {
    loadAccrualRule(-1);
}

function GridStartupValues_BeginCallback(e, c) {
}

function GridStartupValues_EndCallback(s, e) {
    if (s.IsEditing()) {
        hasChanges(true);
    } else {
        if (s.cpAction == "ROWINSERTING" || s.cpAction == "ROWUPDATING" || s.cpAction == "ROWDELETE") {
            hasChanges(true);
        }
    }
}

function GridStartupValues_OnRowDblClick(s, e) {
    GridStartupValuesClient.GetRowValues(e.visibleIndex, 'ID', loadStartupValue);
}

function GridStartupValues_FocusedRowChanged(s, e) {
}

function GridStartupValues_CustomButtonClick(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        GridStartupValuesClient.GetRowValues(e.visibleIndex, 'ID', loadStartupValue);
    }
}

function loadStartupValue(id) {
    actualStartupValue = id;
    var oParameters = {};
    oParameters.aTab = 1
    oParameters.ID = actualStartupValue;
    oParameters.IDLabAgree = actualLabAgrees;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETLABAGREESTARTUP";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxStartupValueCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Agregar nueva fila en el grid de incidencias
function AddNewStartupValue(s, e) {
    loadStartupValue(-1);
}

function GridLabAgreeCauseLimit_BeginCallback(e, c) {
}

function GridLabAgreeCauseLimit_EndCallback(s, e) {
    if (s.IsEditing()) {
        hasChanges(true);
    } else {
        if (s.cpAction == "ROWINSERTING" || s.cpAction == "ROWUPDATING" || s.cpAction == "ROWDELETE") {
            hasChanges(true);
        }
    }
}

function GridLabAgreeCauseLimit_OnRowDblClick(s, e) {
    GridLabAgreeCauseLimitClient.GetRowValues(e.visibleIndex, 'IDCauseLimitValue', loadCauseLimitValue);
}

function GridLabAgreeCauseLimit_FocusedRowChanged(s, e) {
}

function GridLabAgreeCauseLimit_CustomButtonClick(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        GridLabAgreeCauseLimitClient.GetRowValues(e.visibleIndex, 'IDCauseLimitValue', loadCauseLimitValue);
    }
}

function loadCauseLimitValue(id) {
    actualLabAgreeCauseLimit = id;
    var oParameters = {};
    oParameters.aTab = 1
    oParameters.ID = actualLabAgreeCauseLimit;
    oParameters.IDLabAgree = actualLabAgrees;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETCAUSELIMITVALUE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxCauseLimitCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Agregar nueva fila en el grid de incidencias
function AddNewLabAgreeCauseLimit(s, e) {
    loadCauseLimitValue(-1);
}

function GridLabAgreeRequestValidation_BeginCallback(e, c) {
}

function GridLabAgreeRequestValidation_EndCallback(s, e) {
    if (s.IsEditing()) {
        hasChanges(true);
    } else {
        if (s.cpAction == "ROWINSERTING" || s.cpAction == "ROWUPDATING" || s.cpAction == "ROWDELETE") {
            hasChanges(true);
        }
    }
}

function GridLabAgreeRequestValidation_OnRowDblClick(s, e) {
    GridLabAgreeRequestValidationClient.GetRowValues(e.visibleIndex, 'IDRule', loadRequestValidation);
}

function GridLabAgreeRequestValidation_FocusedRowChanged(s, e) {
}

function GridLabAgreeRequestValidation_CustomButtonClick(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        GridLabAgreeRequestValidationClient.GetRowValues(e.visibleIndex, 'IDRule', loadRequestValidation);
    }
}

function AddNewLabAgreeRequestValidation(s, e) {
    loadRequestValidation(-1);
}

function loadRequestValidation(id) {
    actualRequestValidationId = id;
    var oParameters = {};
    oParameters.aTab = 1;
    oParameters.ID = actualRequestValidationId;
    oParameters.IDLabAgree = actualLabAgrees;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETLABAGREEREQUUESTVALIDATION";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxRequestValidationCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function GridLabAgreeScheduleRules_BeginCallback(e, c) {
}

function GridLabAgreeScheduleRules_EndCallback(s, e) {
    if (s.IsEditing()) {
        hasChanges(true);
    } else {
        if (s.cpAction == "ROWINSERTING" || s.cpAction == "ROWUPDATING" || s.cpAction == "ROWDELETE") {
            hasChanges(true);
        }
    }
}

function GridLabAgreeScheduleRules_OnRowDblClick(s, e) {
    GridLabAgreeScheduleRulesClient.GetRowValues(e.visibleIndex, 'Id', loadScheduleRules);
}

function GridLabAgreeScheduleRules_FocusedRowChanged(s, e) {
}

function GridLabAgreeScheduleRules_CustomButtonClick(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        GridLabAgreeScheduleRulesClient.GetRowValues(e.visibleIndex, 'Id', loadScheduleRules);
    }
}

function AddNewLabAgreeScheduleRule(s, e) {
    loadScheduleRules(-1);
}

function loadScheduleRules(id) {
    initScheduleRuleControl('ctl00_contentMainBody_ASPxCallbackPanelContenido', document.getElementById('ctl00_contentMainBody_hdnModeEdit').value);
    actualScheduleRuleId = id;
    var oParameters = {};
    oParameters.aTab = 1;
    oParameters.ID = actualScheduleRuleId;
    oParameters.IDLabAgree = actualLabAgrees;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETLABAGREERESCHEDULERULE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxScheduleRulesCallbackPanelContenidoClient.PerformCallback(strParameters);
}