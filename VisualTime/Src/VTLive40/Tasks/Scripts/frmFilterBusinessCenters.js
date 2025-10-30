//var selectFistNode = false;
function frmFilterBusinessCenters_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmFilterBusinessCenters', false);
    } catch (e) { showError("frmFilterBusinessCenters_Close", e); }
}

function frmFilterBusinessCenters_Show() {
    try {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmFilterBusinessCenters', true);
    }
    catch (e) { showError("frmFilterBusinessCenters_Show", e); }
}

function ASPxBusinessCentersCallbackPanelContenidoClient_EndCallBack(s, e) {
    showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmFilterBusinessCenters', false);
    eraseCookie('TreeState_ctl00_contentMainBody_roTreesBusinessCenters');
    cargaBusinessCentersBarButtons(-1);
    setTimeout(function () { refreshTree(); }, 200);
}

function frmFilterBusinessCenters_Cancel() {
    showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmFilterBusinessCenters', false);
}

function frmFilterBusinessCenters_Save() {
    showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmFilterBusinessCenters', false);
}

function frmFilterBusinessCenters_LoadFilter() {
    try {
        if (ASPxClientEdit.ValidateEditorsInContainer(ASPxBusinessCentersCallbackPanelContenidoClient.GetMainElement())) {
            var oParameters = {};
            var state = cmbStateClient.GetValue();
            var name = txtBusinessCenterNameClient.GetValue();
            oParameters.state = (state == null || state === "") ? "" : state;
            oParameters.name = (name == null || name === "") ? "" : name;
            oParameters.fieldFilters = BCFieldsFilter();
            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxBusinessCentersCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        }
        return true;
    } catch (e) {
        showError("frmEditCauseLimit_Validate", e);
        return false;
    }
}

function BCFieldsFilter() {
    var oFiltersParameteres = [];
    var oFilterParameter1 = FillBCFieldValues(cmbBCFieldsValues1Client, cmbBCCriteria1Client, txtValue1Client);
    var andOr1 = rblAndOr1Client.GetValue();
    oFiltersParameteres.push(oFilterParameter1);
    if (andOr1 != null) {
        oFilterParameter1.Condition = andOr1;
        var oFilterParameter2 = FillBCFieldValues(cmbBCFieldsValues2Client, cmbBCCriteria2Client, txtValue2Client);
        var andOr2 = rblAndOr2Client.GetValue();
        oFiltersParameteres.push(oFilterParameter2);
        if (andOr2 != null) {
            oFilterParameter2.Condition = andOr2;
            var oFilterParameter3 = FillBCFieldValues(cmbBCFieldsValues3Client, cmbBCCriteria3Client, txtValue3Client);
            var andOr3 = rblAndOr3Client.GetValue();
            oFiltersParameteres.push(oFilterParameter3);
            if (andOr3 != null) {
                oFilterParameter3.Condition = andOr3;
                var oFilterParameter4 = FillBCFieldValues(cmbBCFieldsValues4Client, cmbBCCriteria4Client, txtValue4Client);
                var andOr4 = rblAndOr4Client.GetValue();
                oFiltersParameteres.push(oFilterParameter4);
                if (andOr4 != null) {
                    oFilterParameter4.Condition = andOr4;
                    var oFilterParameter5 = FillBCFieldValues(cmbBCFieldsValues5Client, cmbBCCriteria5Client, txtValue5Client);
                    oFiltersParameteres.push(oFilterParameter5);
                }
            }
        }
    }
    return oFiltersParameteres;
}

function FillBCFieldValues(cmbBCFieldsValuesClient, cmbBCCriteriaClient, txtValueClient) {
    var oFilterParameter = {}
    var fieldName = cmbBCFieldsValuesClient.GetValue();
    var criteriaContains = cmbBCCriteriaClient.GetValue();
    var value = txtValueClient.GetValue();
    if (fieldName != null && fieldName != "") {
        oFilterParameter.FieldName = fieldName;
        oFilterParameter.CriteriaContains = (criteriaContains != null && criteriaContains != "") ? criteriaContains : "";
        oFilterParameter.Value = (value != null && value != "") ? value : "";
    }

    return oFilterParameter;
}

function frmFilterBusinessCenters_CleanFilter() {
    cmbStateClient.SetSelectedIndex(-1);
    txtBusinessCenterNameClient.SetValue("");

    cmbBCFieldsValues1Client.SetSelectedIndex(-1);
    cmbBCCriteria1Client.SetSelectedIndex(-1);
    rblAndOr1Client.SetSelectedIndex(-1);
    txtValue1Client.SetValue("");

    cmbBCFieldsValues2Client.SetSelectedIndex(-1);
    cmbBCCriteria2Client.SetSelectedIndex(-1);
    rblAndOr2Client.SetSelectedIndex(-1);
    txtValue2Client.SetValue("");

    cmbBCFieldsValues3Client.SetSelectedIndex(-1);
    cmbBCCriteria3Client.SetSelectedIndex(-1);
    rblAndOr3Client.SetSelectedIndex(-1);
    txtValue3Client.SetValue("");

    cmbBCFieldsValues4Client.SetSelectedIndex(-1);
    cmbBCCriteria4Client.SetSelectedIndex(-1);
    rblAndOr4Client.SetSelectedIndex(-1);
    txtValue4Client.SetValue("");

    cmbBCFieldsValues5Client.SetSelectedIndex(-1);
    cmbBCCriteria5Client.SetSelectedIndex(-1);
    txtValue5Client.SetValue("");
}