var actualLabAgreeCauseLimit = -1;

function frmEditCauseLimit_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditCauseLimit', false);
    } catch (e) { showError("frmEditCauseLimit_Close", e); }
}

function frmEditCauseLimit_Load() {
    try {
        //Carrega dels Camps

        frmEditCauseLimit_Show();
    } catch (e) { showError("frmEditCauseLimit_Load", e); }
}

function frmEditCauseLimit_Validate() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditCauseLimit_ASPxRuleCallbackPanelContenido_";
        if (ASPxClientEdit.ValidateEditorsInContainer(ASPxCauseLimitCallbackPanelContenidoClient.GetMainElement())) {
            var oParameters = {};
            oParameters.aTab = 1;
            oParameters.ID = actualLabAgreeCauseLimit;
            oParameters.IDLabAgree = actualLabAgrees;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVECAUSELIMITVALUE";
            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxCauseLimitCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        }
        return true;
    } catch (e) {
        showError("frmEditCauseLimit_Validate", e);
        return false;
    }
}

function frmEditCauseLimit_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'false') { frmEditCauseLimit_Close(); return; }
        frmEditCauseLimit_Validate();
    } catch (e) { showError("frmEditCauseLimit_Save", e); }
}

// Recarrega dels combos a traves de json
function ASPxCauseLimitCallbackPanelContenido_EndCallBack(s, e) {
    showLoadingGrid(false);

    switch (s.cpActionRO) {
        case "RELOADCOMBOS":
            break;
        case "SAVECAUSELIMITVALUE":
            if (s.cpResultRO == "KO") {
                showErrorPopup("Error.Title", "error", "", s.cpErrorRO.ErrorText, "Error.OK", "Error.OKDesc", "");
            } else {
                hasChanges(true);
                frmEditCauseLimit_Close();
                GridLabAgreeCauseLimitClient.PerformCallback("REFRESH");
            }
            break;
        case "GETCAUSELIMITVALUE":
            checkAnnualStatus();
            checkMonthlyStatus();
            showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditCauseLimit', true);
            ASPxClientEdit.ValidateGroup(null, true);
            break;
    }
}

function checkAnnualStatus() {
    var status = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditCauseLimit_ASPxCauseLimitCallbackPanelContenido_optAnnual_chkButton").checked;
    rbAnnualUFClient.SetEnabled(status);
    rbAnnualFixClient.SetEnabled(status);
    cmbAnnualValueClient.SetEnabled(status);
    txtAnnualValueClient.SetEnabled(status);
}

function checkMonthlyStatus() {
    var status = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditCauseLimit_ASPxCauseLimitCallbackPanelContenido_optMonthly_chkButton").checked;
    rbMonthlyUFClient.SetEnabled(status);
    rbMonthlyFixClient.SetEnabled(status);
    cmbMonthlyValueClient.SetEnabled(status);
    txtMonthlyValueClient.SetEnabled(status);
}