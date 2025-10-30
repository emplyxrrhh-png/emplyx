//Carga Tabs y contenido Empleados
function cargaDocumentTemplate(IDDocumentTemplate) {
    actualTabDocument = 0;
    changeDocumentTabs(actualTabDocument);
    actualType = "D";
    actualDocument = IDDocumentTemplate;
    cargaDocumentTemplateDivs(IDDocumentTemplate);
}

var responseObj = null;

// Carrega els apartats dels divs de l'usuari
function cargaDocumentTemplateDivs(IDDocumentTemplate) {
    var oParameters = {};
    oParameters.aTab = actualTabDocument;
    oParameters.ID = actualDocument;
    oParameters.oType = "D";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETDOCUMENT";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function changeDocumentTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03', 'TABBUTTON_04', 'TABBUTTON_05');
    arrDivs = new Array('panDocGeneral', 'panDocControl', 'panDocScope', 'panDocApprove', 'panDocNotifications', 'panDocLOPD');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (tab != null && div != null) {
            if (n == numTab) {
                tab.className = 'bTabDocuments-active';
                div.style.display = '';
            } else {
                tab.className = 'bTabDocuments';
                div.style.display = 'none';
            }
        }
    }
    actualTabDocument = numTab;
}

function saveChangesDocument() {
    try {
        if (ASPxClientEdit.ValidateGroup("Document", true)) {
            var oParameters = {};
            oParameters.aTab = actualTabDocument;
            oParameters.ID = actualDocument;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.oType = "D";
            oParameters.action = "SAVEDOCUMENT";

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);
            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        }
    } catch (e) { alert("saveChangesDocument: " + e); }
}

function checkStatusDocuments(oStatus) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Documents/srvMsgBoxDocument.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            }
        }
    }
    catch (e) {
        showError("checkStatus", e);
    }
}

function rblAccessAuthorization_Checked(s, e) {
    try {
        if (rblCompanyAccessAuthorization_Client.GetChecked() || rblEmployeeAccessAuthorization_Client.GetChecked()) {
            rbnNonCriticality_Client.SetEnabled(true);
            rbnAdviceCriticality_Client.SetEnabled(true);
            rbnDeniedCriticality_Client.SetEnabled(true);
        } else {
            rbnNonCriticality_Client.SetEnabled(false);
            rbnAdviceCriticality_Client.SetEnabled(false);
            rbnDeniedCriticality_Client.SetEnabled(false);
        }
    } catch { }
}

function rblAmbit_SelectedIndexChanged(s, e) {
    ckCanAddDocumentSupervisorClient.SetEnabled(false);
    ckCanAddDocumentSupervisorClient.SetChecked(true);

    if (s.GetValue() == "1") { //Es empresa
        $("#pnlScope .rbgScopeCompany").each((i, item) => { $(item).show(); $(item).closest('table.dxeValidStEditorTable').show() });
        $("#pnlScope .rbgScopeEmployee").each((i, item) => { $(item).hide(); $(item).closest('table.dxeValidStEditorTable').hide() });

        ckCanAddDocumentEmployeeClient.SetChecked(false);
        ckCanAddDocumentEmployeeClient.SetEnabled(false);
        ckNotification700_Client.SetChecked(false);
        ckNotification700_Client.SetEnabled(false);
        ckRequieresSign_Client.SetEnabled(false);
        ckRequieresSign_Client.SetChecked(false);
        if (isNew()) {
            rblCompany_Client.SetChecked(true);
            rblAccessAuthorization_Checked();
            ckRequieresSign_Client.SetEnabled(false);
            ckRequieresSign_Client.SetChecked(false);
        }
    } else { //Es empleado
        $("#pnlScope .rbgScopeCompany").each((i, item) => { $(item).hide(); $(item).closest('table.dxeValidStEditorTable').hide() });
        $("#pnlScope .rbgScopeEmployee").each((i, item) => { $(item).show(); $(item).closest('table.dxeValidStEditorTable').show() });

        ckCanAddDocumentEmployeeClient.SetEnabled(true);
        ckRequieresSign_Client.SetEnabled(false);

        if (isNew()) {
            ckNotification700_Client.SetEnabled(ckCanAddDocumentEmployeeClient.GetChecked());
            ckNotification700_Client.SetChecked(false);
            rblEmployeeContract_Client.SetChecked(true);
            ckRequieresSign_Client.SetEnabled(true);
            rblAccessAuthorization_Checked();
        }
    }
}

function rblMandatory_SelectedIndexChanged(s, e) {
    var value = false;

    if (s.GetSelectedItem() != null) {
        value = s.GetSelectedItem().value == "True";
    }
    else {
        value = false;
    }

    ckValidPeriod_Client.SetEnabled(value);
    dpPeriodStart_Client.SetEnabled(value);

    if (isNew()) {
        ckNotification701_Client.SetChecked(false);
        ckNotification702_Client.SetChecked(false);
    }
    if (isNew() /*|| dpPeriodStart_Client.GetValue() == null*/) {
        if (value) {
            ckValidPeriod_Client.SetChecked(true);
            dpPeriodStart_Client.SetDate(new Date());
        }
        else {
            dpPeriodStart_Client.SetDate(null);
            ckValidPeriod_Client.SetChecked(false);
        }
    }
    else if (!value) {
        dpPeriodStart_Client.SetDate(null);
        ckValidPeriod_Client.SetChecked(false);
        ckNotification701_Client.SetChecked(false);
        ckNotification702_Client.SetChecked(false);
    }

    ckNotification701_Client.SetEnabled(value);
    ckNotification702_Client.SetEnabled(value);
}

function ckValidPeriod_Checked(s, e) {
    dpPeriodStart_Client.SetEnabled(s.GetChecked());

    if (s.GetChecked()) {
        if (isNew() || dpPeriodStart_Client.GetValue() == null) {
            var currentDate = new Date();
            if (!s.GetChecked()) {
                currentDate = null;
            }
            dpPeriodStart_Client.SetDate(currentDate);
        }
    }
    else {
        dpPeriodStart_Client.SetDate(null);
    }
}

function ckApproveRequiered_Checked(s, e) {
    txtRequieredSupervisorLevelClient.SetEnabled(s.GetChecked());
    if (!s.GetChecked()) {
        txtRequieredSupervisorLevelClient.SetValue("1");
        ckNotification703_Client.SetEnabled(false);
        ckNotification703_Client.SetChecked(false);
    } else {
        ckNotification703_Client.SetEnabled(true);
    }
}

function rbValidUntil_Checked(s, e) {
    if (rbAlways_Client.GetChecked()) {
        txtExpireDaysClient.SetEnabled(false);
        txtExpireDaysClient.SetValue("0");
    } else {
        txtExpireDaysClient.SetEnabled(true);
    }
}