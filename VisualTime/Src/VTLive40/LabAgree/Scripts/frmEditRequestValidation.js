var actualRequestValidationId = -1;

function changeLabAgreeRequestValidationTabs(numTab) {
    var AbsArrButtons = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_TABBUTTON_LARV01',
        'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_TABBUTTON_LARV00');
    var AbsArrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_Content_LARV01',
        'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_Content_LARV00');

    for (n = 0; n < AbsArrButtons.length; n++) {
        var tab = document.getElementById(AbsArrButtons[n]);
        var div = document.getElementById(AbsArrDivs[n]);
        if (tab != null && div != null) {
            if (n == numTab) {
                tab.className = 'bTab-active';
                div.style.display = '';
            } else {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }

    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_frm').css('margin-top', "-450px")
}

function frmEditRequestValidation_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation', false);
    } catch (e) { showError("frmEditRequestValidation_Close", e); }
}

function frmEditRequestValidation_Validate() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_";

        if (ASPxClientEdit.ValidateEditorsInContainer(ASPxRequestValidationCallbackPanelContenidoClient.GetMainElement())) {
            var oParameters = {};
            oParameters.aTab = 1;
            oParameters.ID = actualRequestValidationId;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVELABAGREEREQUESTVALIDATION";
            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxRequestValidationCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        }
        return true;
    } catch (e) {
        showError("frmEditRequestValidation_Validate", e);
        return false;
    }
}

function frmEditRequestValidation_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'false') { frmEditRequestValidation_Close(); return; }
        frmEditRequestValidation_Validate();
    } catch (e) { showError("frmEditRequestValidation_Save", e); }
}

// Recarrega dels combos a traves de json

function buildAutomaticRulesDiv(rulesDS) {
    var ds = [];

    for (var i = 0; i < rulesDS.length; i++) {
        ds.push({ ruleName: rulesDS[i] });
    }

    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_grdAutomaticRequests").dxDataGrid({
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,
        dataSource: {
            store: ds
        },
        editing: {
            mode: "row",
            allowUpdating: false,
            allowDeleting: false,
            texts: { deleteRow: 'Delete', editRow: 'Edit' }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                $links.text("");

                if (isEditing) {
                    $links.filter(".dx-link-save").addClass("dx-icon-save");
                    $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                } else {
                    $links.filter(".dx-link-edit").addClass("dx-icon-edit");
                    $links.filter(".dx-link-delete").addClass("dx-icon-trash");
                }
            }
        },
        remoteOperations: {
            sorting: true,
            paging: true
        },
        paging: {
            pageSize: 3
        },
        pager: {
            showPageSizeSelector: false
        },
        columns: [
            { caption: "Nombre", dataField: "ruleName", allowEditing: false }
        ]
    }).dxDataGrid("instance");
}

function loadComponentsVisibility() {
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_opVisibilityAll');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_opVisibilityCriteria');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_opVisibilityAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_opVisibilityCriteria');
}

function checkCriteriaVisibility() {
    if (document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_opVisibilityCriteria_rButton").checked == false) {
        document.getElementById("criteriaCell").style.display = "none";
    } else {
        document.getElementById("criteriaCell").style.display = "";
    }
}

function ASPxRequestValidationCallbackPanelContenido_EndCallBack(s, e) {
    showLoadingGrid(false);

    switch (s.cpActionRO) {
        case "GETLABAGREEREQUUESTVALIDATION":
            showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation', true);
            changeLabAgreeRequestValidationTabs(0);
            ASPxClientEdit.ValidateGroup(null, true);
            loadComponentsVisibility();
            loadRuleConfigurationDiv();
            buildAutomaticRulesDiv(s.cpUsedInRules);
            checkCriteriaVisibility();
            if (s.cpCombosIndexRO != "") {
                setSelectedIndexes("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_opVisibilityCriteria_visibilityCriteria", s.cpCombosIndexRO);
            }

            break;
        case "SAVELABAGREEREQUESTVALIDATION":
            if (s.cpResultRO == "KO") {
                loadRuleConfigurationDiv();
                showErrorPopup("Error.Title", "error", "", s.cpErrorRO.ErrorText, "Error.OK", "Error.OKDesc", "");
            } else {
                hasChanges(true);
                frmEditRequestValidation_Close();
                GridLabAgreeRequestValidationClient.PerformCallback("REFRESH");
            }
            buildAutomaticRulesDiv(s.cpUsedInRules);
            break;
        case "RELOADRULETYPES":
            loadRuleConfigurationDiv();
            ASPxClientEdit.ValidateGroup(null, true);
            break;
        case "LOADRULECONFIGURATION":
            ASPxClientEdit.ValidateGroup(null, true);
            break;
        case "CHECKAUTOMATICRULES":
            loadRuleConfigurationDiv();
            buildAutomaticRulesDiv(s.cpUsedInRules);
            break;
    }
}

function loadRuleConfigurationDiv() {
    if (cmbRuleType_Client.GetSelectedItem() != null) {
        var typeDiv = parseInt(cmbRuleType_Client.GetSelectedItem().value, 10);
        for (var i = 1; i <= 11; i++) {
            if (i == typeDiv) {
                $('#ruleType_' + i).show();
            } else {
                $('#ruleType_' + i).hide();
            }
        }

        if (typeDiv == 4 || typeDiv == 9) {
            $('#ActiveEnabled').hide();
            $('#divRuleWhen_' + typeDiv).show();
            $('#divRuleWhen').hide();

            if (typeDiv == 4) {
                $('#divRuleWhen_9').hide();
                $('#divRuleAction_' + typeDiv).show();
                $('#divRuleAction').hide();
            }
            else {
                $('#divRuleWhen_4').hide();
                $('#divRuleAction').hide();
                $('#divActionGlobal').hide();
            }
        } else {
            $('#ActiveEnabled').show();
            $('#divRuleWhen_4').hide();
            $('#divRuleWhen_9').hide();
            $('#divRuleWhen').show();

            $('#divRuleAction_4').hide();
            $('#divRuleAction').show();
        }
    } else {
        $('#ActiveEnabled').show();
        $('#divRuleWhen_4').hide();
        $('#divRuleWhen_9').hide();
        $('#divRuleWhen').show();

        $('#divRuleAction_4').hide();
        $('#divRuleAction').show();
    }

    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_frm').css('margin-top', "-450px")

    loadComponentsVisibility()
}

function loadRuleSpecificConfigurationDiv() {
    var oParameters = {};
    oParameters.aTab = 1;
    oParameters.ID = actualRequestValidationId;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "LOADRULECONFIGURATION";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxRequestValidationCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function availableRequestChanged(s, e) {
    var oParameters = {};
    oParameters.aTab = 1;
    oParameters.ID = actualRequestValidationId;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "RELOADRULETYPES";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxRequestValidationCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function checkActivatedrules(s, e) {
    var oParameters = {};
    oParameters.aTab = 1;
    oParameters.ID = actualRequestValidationId;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.Tokens = s.GetValue();
    oParameters.IDRequestType = cmbAvailableRequests_Client.GetSelectedItem().value;
    oParameters.action = "CHECKAUTOMATICRULES";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxRequestValidationCallbackContenidoClient.PerformCallback(strParameters);
}

function onChangePlanificationRules(s, e) {
    var values = s.GetValue ? s.GetValue() : "";
    if (!values) return;

    var items = values.split(',');
    var todosIndex = items.indexOf("-1");

    if (todosIndex > -1 && items.length > 1) {
        s.SetValue("-1");
    }
}

function onChangeShiftPlanificationRules(s, e) {
    var values = s.GetValue ? s.GetValue() : "";
    if (!values) return;

    var items = values.split(',');
    var todosIndex = items.indexOf("-1");

    if (todosIndex > -1 && items.length > 1) {
        s.SetValue("-1");
    }
}

function ASPxRequestValidationCallbackContenidoClient_CallbackComplete(s, e) {
    switch (s.cpActionRO) {
        case "CHECKAUTOMATICRULES":
            buildAutomaticRulesDiv(s.cpUsedInRules);
            break;
    }
}