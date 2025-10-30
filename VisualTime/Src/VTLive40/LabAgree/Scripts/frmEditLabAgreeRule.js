var actualLabAgreeRule = -1;

function frmEditLabAgreeRule_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditLabAgreeRule', false);
    } catch (e) { showError("frmEditLabAgreeRule_Close", e); }
}

function frmEditLabAgreeRule_Load() {
    try {
        //Carrega dels Camps

        frmEditLabAgreeRule_Show();
    } catch (e) { showError("frmEditLabAgreeRule_Load", e); }
}

function frmEditLabAgreeRule_Validate() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditLabAgreeRule_ASPxRuleCallbackPanelContenido_";
        if (ASPxClientEdit.ValidateEditorsInContainer(ASPxRuleCallbackPanelContenidoClient.GetMainElement())) {
            var oParameters = {};
            oParameters.aTab = 1;
            oParameters.ID = actualLabAgreeRule;
            oParameters.IDLabAgree = actualLabAgrees;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVELABAGREERULE";
            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxRuleCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        }
        return true;
    } catch (e) {
        showError("frmEditLabAgreeRule_Validate", e);
        return false;
    }
}

function frmEditLabAgreeRule_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'false') { frmEditLabAgreeRule_Close(); return; }
        frmEditLabAgreeRule_Validate();
    } catch (e) { showError("frmEditLabAgreeRule_Save", e); }
}

// Recarrega dels combos a traves de json
function ASPxRuleCallbackPanelContenidoClient_EndCallBack(s, e) {
    showLoadingGrid(false);

    switch (s.cpActionRO) {
        case "RELOADCOMBOS":
            break;
        case "SAVELABAGREERULE":
            if (s.cpResultRO == "KO") {
                showErrorPopup("Error.Title", "error", "", s.cpErrorRO.ErrorText, "Error.OK", "Error.OKDesc", "");
            } else {
                hasChanges(true);
                frmEditLabAgreeRule_Close();
                GridLabAgreeRulesClient.PerformCallback("REFRESH");
            }
            break;
        case "GETLABAGREERULE":
            showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditLabAgreeRule', true);
            if (cmbMainAccrual_Client.GetSelectedItem() != null) {
                changeValueType(cmbMainAccrual_Client.GetSelectedItem().value.split('_')[1]);
            }
            ShowTheValue(cmbValueTypes_Client.GetSelectedIndex());
            if (cmbDif_Client.GetSelectedItem() != null) { changeComboDif(cmbDif_Client.GetSelectedItem().value); }
            ASPxClientEdit.ValidateGroup(null, true);
            break;
    }
}

function ShowTheValue(ValueType) {
    try {
        var divValueTime = document.getElementById('divValueTime');
        var divValueUserFields = document.getElementById('divValueUserFields');
        var divValueConcepts = document.getElementById('divValueConcepts');
        if (divValueTime != null) {
            if (ValueType == 0) {
                divValueTime.style.display = '';
            } else {
                divValueTime.style.display = 'none';
            }
        }
        if (divValueUserFields != null) {
            var divValueUserFieldsH = document.getElementById('divValueUserFieldsH');
            var divValueUserFieldsO = document.getElementById('divValueUserFieldsO');
            if (ValueType == 1) {
                divValueUserFields.style.display = '';
                if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditLabAgreeRule_ASPxRuleCallbackPanelContenido_RoGroupBox1_hdnIDType').value == 'H') {
                    divValueUserFieldsH.style.display = '';
                    divValueUserFieldsO.style.display = 'none';
                }
                else {
                    divValueUserFieldsH.style.display = 'none';
                    divValueUserFieldsO.style.display = '';
                }
            }
            else {
                divValueUserFields.style.display = 'none';
            }
        }
        if (divValueConcepts != null) {
            if (ValueType == 2) {
                divValueConcepts.style.display = '';
            } else {
                divValueConcepts.style.display = 'none';
            }
        }
    } catch (e) { showError("ShowTheValue", e); }
}

function changeComboDif(iValue) {
    try {
        var oValue = "" + iValue + "";
        var divUntilTime = document.getElementById('divUntilTime');
        var divUntilUserFields = document.getElementById('divUntilUserFields');

        if (oValue == document.getElementById('hdnDifUntilValue').value ||
            oValue == document.getElementById('hdnDifValue').value) {
            divUntilTime.style.display = '';
        } else {
            divUntilTime.style.display = 'none';
        }
        if (oValue == document.getElementById('hdnDifUserFieldUntilValue').value ||
            oValue == document.getElementById('hdnDifUserFieldValue').value) {
            divUntilUserFields.style.display = '';
            var divUntilUserFieldsH = document.getElementById('divUntilUserFieldsH');
            var divUntilUserFieldsO = document.getElementById('divUntilUserFieldsO');
            if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditLabAgreeRule_ASPxRuleCallbackPanelContenido_RoGroupBox1_hdnIDType').value == 'H') {
                divUntilUserFieldsH.style.display = '';
                divUntilUserFieldsO.style.display = 'none';
            }
            else {
                divUntilUserFieldsH.style.display = 'none';
                divUntilUserFieldsO.style.display = '';
            }
        }
        else {
            divUntilUserFields.style.display = 'none';
        }
    } catch (e) { showError("changeComboDif", e); }
}

function changeValueType(IDType) {
    try {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditLabAgreeRule_ASPxRuleCallbackPanelContenido_RoGroupBox1_hdnIDType').value = IDType;

        var divValueTime1 = document.getElementById('divValueTime1');
        var divValueOnce1 = document.getElementById('divValueOnce1');
        if (IDType == "H") {
            divValueTime1.style.display = "";
            divValueOnce1.style.display = "none";
        } else {
            divValueTime1.style.display = "none";
            divValueOnce1.style.display = "";
        }

        var divValueUserFieldsH = document.getElementById('divValueUserFieldsH');
        var divValueUserFieldsO = document.getElementById('divValueUserFieldsO');
        if (IDType == "H") {
            divValueUserFieldsH.style.display = "";
            divValueUserFieldsO.style.display = "none";
        } else {
            divValueUserFieldsH.style.display = "none";
            divValueUserFieldsO.style.display = "";
        }

        var divUntilTime1 = document.getElementById('divUntilTime1');
        var divUntilOnce1 = document.getElementById('divUntilOnce1');
        if (IDType == "H") {
            divUntilTime1.style.display = "";
            divUntilOnce1.style.display = "none";
        } else {
            divUntilTime1.style.display = "none";
            divUntilOnce1.style.display = "";
        }

        var divUntilUserFieldsH = document.getElementById('divUntilUserFieldsH');
        var divUntilUserFieldsO = document.getElementById('divUntilUserFieldsO');
        if (IDType == "H") {
            divUntilUserFieldsH.style.display = "";
            divUntilUserFieldsO.style.display = "none";
        } else {
            divUntilUserFieldsH.style.display = "none";
            divUntilUserFieldsO.style.display = "";
        }

        if (cmbDif_Client.GetSelectedItem() != null) changeComboDif(cmbDif_Client.GetSelectedItem().value);
    } catch (e) { showError("changeValueType", e); }
}

function setScheduleVisibility(AccrualType) {
    try {
        if (AccrualType == 'L') {
        } else {
        }
    } catch
    { }
}