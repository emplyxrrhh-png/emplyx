var actualStartupValue = -1;
var defaultQuery = '';

function changeLabAgreeStartupTabs(numTab) {
    var AbsArrButtons = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_TABBUTTON_LASV00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_TABBUTTON_LASA01');
    var AbsArrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_Content_LASV00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_Content_LASA01');

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
}

function cmbConceptChanged(checkRelatedObjects) {
    var status = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optInitializaWith_chkButton").checked;

    if (cmbIDConceptClient.GetSelectedIndex() == -1) {
        defaultQuery = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_hdnDefaultQuery').value;
    } else {
        defaultQuery = cmbIDConceptClient.GetSelectedItem().value.split("_")[2];
    }

    if (status) {

        if (defaultQuery == 'M' || defaultQuery == 'Y' || defaultQuery == 'L') {
            rbCalculatedStartupValueClient.SetEnabled(true);
            ckYearClient.SetEnabled(true);
            ckAntClient.SetEnabled(true);
        } else {
            rbCalculatedStartupValueClient.SetEnabled(false);
            ckYearClient.SetEnabled(false);
            ckAntClient.SetEnabled(false);
        }
    } else {
        rbCalculatedStartupValueClient.SetEnabled(false);
        ckYearClient.SetEnabled(false);
        ckAntClient.SetEnabled(false);
    }

    if (typeof checkRelatedObjects == 'undefined' || checkRelatedObjects) checkInitializeStatus();
}

function AcceptAntInitialValueClick() {
    rbCalculatedStartupValueClient.SetChecked(true);
    ckYearClient.SetEnabled(true);
    ckAntClient.SetEnabled(true);
    antStartupPopupClient.Hide();
}

function CloseAntInitialValueClick() {
    antStartupPopupClient.Hide();
}
function AcceptCalculatedInitialValueClick() {
    rbCalculatedStartupValueClient.SetChecked(true);
    ckYearClient.SetEnabled(true);
    ckAntClient.SetEnabled(true);
    calculatedStartupPopupClient.Hide();
}

function CloseCalculatedInitialValueClick() {
    calculatedStartupPopupClient.Hide();
}

//var startupValueDialog = null;

function showStartupValue_Ant() {
    antStartupPopupClient.Show();
}
function showLabAgreeStartupValue_Calculated() {
    if (rbCalculatedStartupValueClient.GetEnabled()) {
        var idType = '';
        if (cmbIDConceptClient.GetSelectedIndex() == -1) {
            idType = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_hdnIDType').value;
        } else {
            idType = cmbIDConceptClient.GetSelectedItem().value.split("_")[1];
        }

        if (idType == 'O') {
            rbAutomaticAccrualUFClient.SetEnabled(false);
            cmbAutomaticAccrualUFClient.SetEnabled(false);
            rbAutomaticAccrualFixClient.SetEnabled(false);
            txtAutomaticAccrualFixClient.SetEnabled(false);
        } else {
            rbAutomaticAccrualUFClient.SetEnabled(true);
            cmbAutomaticAccrualUFClient.SetEnabled(true);
            rbAutomaticAccrualFixClient.SetEnabled(true);
            txtAutomaticAccrualFixClient.SetEnabled(true);
        }

        calculatedStartupPopupClient.Show();

        //startupValueDialog = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_labAgreeStartupCalculatedValue").dialog({
        //    autoOpen: false,
        //    height: 'auto',
        //    width: '550px',
        //    modal: true,
        //    resizable: false,
        //    draggable: false,
        //    buttons: [{
        //        text: "Accept",
        //        "class": 'btnFlat btnFlatBlack',
        //        click: function () {
        //            rbCalculatedStartupValueClient.SetChecked(true);
        //            startupValueDialog.dialog("close");
        //            startupValueDialog = null;
        //        },

        //    }, {
        //        text: "Cancelar",
        //        "class": 'btnFlat btnFlatBlack',
        //        click: function () {
        //            startupValueDialog.dialog("close");
        //            startupValueDialog = null;
        //        },
        //    }],
        //    close: function () {
        //    }
        //});
        //startupValueDialog.dialog('open');
    }
}

function frmEditStartupValue_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue', false);
    } catch (e) { showError("frmEditStartupValue_Close", e); }
}

function frmEditStartupValue_Validate() {
    try {
        var objPrTab = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_";

        if (ASPxClientEdit.ValidateEditorsInContainer(ASPxStartupValueCallbackPanelContenidoClient.GetMainElement())) {
            var oParameters = {};
            oParameters.aTab = 1;
            oParameters.ID = actualStartupValue;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVELABAGREESTARTUP";
            oParameters.acumValues = acumValues;
            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxStartupValueCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        }
        return true;
    } catch (e) {
        showError("frmEditStartupValue_Validate", e);
        return false;
    }
}

function frmEditStartupValue_Save() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'false') { frmEditStartupValue_Close(); return; }
        frmEditStartupValue_Validate();
    } catch (e) { showError("frmEditStartupValue_Save", e); }
}

// Recarrega dels combos a traves de json

function ASPxFormCallbackPanelContenidoClient_EndCallBack(s, e) {
    showLoadingGrid(false);

    switch (s.cpActionRO) {
        case "GETLABAGREESTARTUP":
            checkInitializeStatus();
            checkAlertMinStatus();
            checkAlertWithStatus();

            showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue', true);
            ASPxClientEdit.ValidateGroup(null, true);

            LoadAntValues(s);

            break;
        case "SAVELABAGREESTARTUP":
            if (s.cpResultRO == "KO") {
                LoadAntValues(s);
                showErrorPopup("Error.Title", "error", "", s.cpErrorRO.ErrorText, "Error.OK", "Error.OKDesc", "");
            } else {
                hasChanges(true);
                frmEditStartupValue_Close();
                GridStartupValuesClient.PerformCallback("REFRESH");
            }
            break;
    }
}

function checkInitializeStatus() {
    var status = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optInitializaWith_chkButton").checked;
    rbStartValueUFClient.SetEnabled(status);
    rbStartValueFixClient.SetEnabled(status);

    cmbStartValueClient.SetEnabled(status);
    txtStartValueClient.SetEnabled(status);

    ckStartContractExceptionClient.SetEnabled(status);

    ckYearClient.SetEnabled(status);
    ckAntClient.SetEnabled(status);

    if (status) {
        cmbConceptChanged(false);
        setroUserCtlFieldCriteriaEnabled('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optInitializaWith_contractExceptionCriteria', ckStartContractExceptionClient.GetChecked());
    } else {
        setroUserCtlFieldCriteriaEnabled('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optInitializaWith_contractExceptionCriteria', false);
        rbCalculatedStartupValueClient.SetEnabled(false);
        ckYearClient.SetEnabled(false);
        ckAntClient.SetEnabled(false);
    }


    if (defaultQuery != 'L') status = false;

    txtExpirationPeriodValueClient.SetEnabled(status);
    cmbExpirationPeriodTypeClient.SetEnabled(status);
    txtEnjoymentPeriodValueClient.SetEnabled(status);
    cmbEnjoymentPeriodTypeClient.SetEnabled(status);

    if (!status) {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualExpiration_chkButton').checked = false;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualEnjoyment_chkButton').checked = false;
        disableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualExpiration_chkButton'));
        disableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualEnjoyment_chkButton'));
    } else {
        enableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualExpiration_chkButton'));
        enableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualEnjoyment_chkButton'));
    }

    clickOPC(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualExpiration_chkButton'), 'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualExpiration');
    clickOPC(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualEnjoyment_chkButton'), 'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAccrualEnjoyment');

}

function checkCtlFieldCriteriaVisible(s, e) {
    setroUserCtlFieldCriteriaEnabled('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optInitializaWith_contractExceptionCriteria', ckStartContractExceptionClient.GetChecked());
}
function checkAnt(s, e) {
}

function checkAlertWithStatus() {
    var status = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAlertWith_chkButton").checked;
    rbAlertValueUFClient.SetEnabled(status);
    rbAlertValueFixClient.SetEnabled(status);
    cmbMaximumValueClient.SetEnabled(status);
    txtMaximumValueClient.SetEnabled(status);
}

function checkAlertMinStatus() {
    var status = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optAlertMin_chkButton").checked;
    rbAlertMInValueUFClient.SetEnabled(status);
    rbAlertMInValueFixClient.SetEnabled(status);
    cmbMinimumValueClient.SetEnabled(status);
    txtMinimumValueClient.SetEnabled(status);
}

var gridValues = null;
var actualValues = [];
var acumValues = [];

function LoadAntValues(s) {
    acumValues = [];

    if (s.cpValues != null) {
        for (var i = 0; i < s.cpValues.length; i++) {
            acumValues.push({ UserField: s.cpValues[i].UserField, AccumValue: s.cpValues[i].AccumValue });
        }
    }
    gridValues = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_antStartupPopup_RoGroupBox2_divValuesGrid").dxDataGrid({
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,

        dataSource: {
            store: acumValues
        },
        editing: {
            mode: "row",
            allowUpdating: true,
            allowAdding: true,
            allowDeleting: true,
            texts: { deleteRow: 'Delete', editRow: 'Edit', confirmDeleteMessage: "" }
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
            pageSize: 12
        },
        pager: {
            showPageSizeSelector: false
        },
        columns: [
            { caption: "Antigüedad", dataField: "UserField", allowEditing: true, allowDeleting: true, dataType: "number", validationRules: [{ type: "required" }] },
            { caption: "Valor a acumular", dataField: "AccumValue", allowEditing: true, allowDeleting: true, dataType: "number", validationRules: [{ type: "required" }] },
        ]
    }).dxDataGrid("instance");
}