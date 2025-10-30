//Carga Tabs y contenido Empleados
function cargaDocumentTemplate(IDDocumentTemplate) {
    actualTabDocument = 0;
    changeDocumentTabs(actualTabDocument);
    actualType = "D";
    ShowDetail();
    clearDocumentTemplateControls()
    actualDocument = IDDocumentTemplate;
    showLoadingGrid(true);
    cargaDocumentBarButtons(IDDocumentTemplate);
}

var responseObj = null;
// Carrega la part del TAB grisa superior
function cargaDocumentBarButtons(IDDocumentTemplate) {
    try {
        var Url = "Handlers/srvDocumentTemplates.ashx?action=getBarButtons&ID=" + IDDocumentTemplate;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDDocumentTemplate + ")");
    }
    catch (e) {
        showError("cargaDocumentBarButtons", e);
    }
}

function parseResponseBarButtons(oResponse, IDDocumentTemplate) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaDocumentTemplateDivs(IDDocumentTemplate);
}

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

function checkDocumentEmptyName(newName) {
    document.getElementById("readOnlyNameDocumentTemplate").textContent = newName;
    hasChanges(true);
}

function changeDocumentTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03', 'TABBUTTON_04', 'TABBUTTON_05');
    arrDivs = new Array('panDocGeneral', 'panDocControl', 'panDocScope', 'panDocApprove', 'panDocNotifications', 'panDocLOPD');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
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
    actualTabDocument = numTab;
}

function hasChangesDocument(bolChanges) {
    var divTop = document.getElementById('divMsgTop');
    var divContenido = document.getElementById('divContenido');

    var tagHasChanges = document.getElementById('msgHasChanges');
    var msgChanges = '<changes>';
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setMessage(msgChanges);

    if (bolChanges) {
        divTop.style.display = '';
        document.getElementById('divContentPanels').className = "divContentPanelsWithMessage";
    }
    else {
        divTop.style.display = 'none';
        document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
    }
}

function saveChangesDocument() {
    try {
        if (ASPxClientEdit.ValidateGroup("Document", true)) {
            showLoadingGrid(true);

            var oParameters = {};
            oParameters.aTab = actualTabDocument;
            oParameters.ID = actualDocument;
            oParameters.Name = document.getElementById("readOnlyNameDocumentTemplate").textContent.trim();
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

function newDocumentTemplate() {
    createType = "D";
    var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=DocumentTemplate";
    NewObjectPopup_Client.SetContentUrl(contentUrl);
    NewObjectPopup_Client.Show();
}

function clearDocumentTemplateControls() {
    txtDocName_Client.SetText("");
    txtDocShortName_Client.SetText("");
    txtDocDescription_Client.SetText("");
    rblDocumentArea_Client.SetSelectedIndex(0);
    rblScope_Client.SetSelectedIndex(0);
}

function ShowRemoveDocumentTemplate() {
    if (actualDocument < 1) { return; }

    var url = "Documents/srvMsgBoxDocument.aspx?action=Message";
    url = url + "&TitleKey=deleteDocument.Title";
    url = url + "&DescriptionKey=deleteDocument.Description";
    url = url + "&Option1TextKey=deleteDocument.Option1Text";
    url = url + "&Option1DescriptionKey=deleteDocument.Option1Description";
    url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].removeDocumentTemplate('" + actualDocument + "'); return false;";
    url = url + "&Option2TextKey=deleteDocument.Option2Text";
    url = url + "&Option2DescriptionKey=deleteDocument.Option2Description";
    url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function removeDocumentTemplate(ID) {
    try {
        if (ID > 0) {
            showLoadingGrid(true);
            AsyncCall("POST", "Handlers/srvDocumentTemplates.ashx?action=deleteDocumentTemplate&ID=" + ID, "json", "arrStatus", "checkStatusDocuments(arrStatus); if(arrStatus[0].error == 'false'){ showLoadingGrid(false); BackToGrid(); } else {showLoadingGrid(false);}")
        }
    } catch (e) { showError('removeActivity', e); }
}

function undoChangesDocument() {
    try {
        if (actualDocument > 0) {
            showLoadingGrid(true);
            cargaDocumentTemplate(actualDocument);
        } else {
            BackToGrid();
        }
    } catch (e) { showError("undoChanges", e); }
}

function rblScope_SelectedIndexChanged(s, e) {
    //if (s.savedSelectedIndex==3 || s.savedSelectedIndex==4) { //3 y 4 son de tipo absencia
    if (s.GetSelectedIndex() == 3) {
        //deshabilitar aprobaciones
        //$('#aprobaciones *').prop('disabled', true);
        //y seleccionar radiobox "No requiere aprobacion"
        //$('#ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoApprove_rButton').prop('checked', 'checked');
        //$('#ctl00_contentMainBody_ASPxCallbackPanelContenido_optApproveRequiered_rButton').prop('checked', '');

        rbnNonCriticality_Client.SetChecked(true);
        rbnNonCriticality_Client.SetEnabled(false);
        rbnAdviceCriticality_Client.SetEnabled(false);
        rbnDeniedCriticality_Client.SetEnabled(false);
    } else if (s.GetSelectedIndex() == 5 || s.GetSelectedIndex() == 6) {
        rbnDeniedCriticality_Client.SetChecked(true);

        rbnNonCriticality_Client.SetEnabled(true);
        rbnAdviceCriticality_Client.SetEnabled(true);
        rbnDeniedCriticality_Client.SetEnabled(true);
    } else {
        $('#aprobaciones *').prop('disabled', false);

        rbnNonCriticality_Client.SetChecked(true);

        rbnNonCriticality_Client.SetEnabled(false);
        rbnAdviceCriticality_Client.SetEnabled(false);
        rbnDeniedCriticality_Client.SetEnabled(false);
    }

    hasChanges(true);
}