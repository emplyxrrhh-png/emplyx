function FilterDocumentTemplatesByState(s, e, i) {
    if (hdnFilterStatusClient.contains("StatusFilter")) {
        hdnFilterStatusClient("StatusFilter", hdnFilterStatusClient("StatusFilter").substr(0, i) + (s.checked == false ? "1" : "0") + hdnFilterStatusClient("StatusFilter").substr(i + 1));
    } else {
        var initalFilter = "00000";
        hdnFilterStatusClient("StatusFilter", initalFilter.substr(0, i) + (s.checked == false ? "1" : "0") + initalFilter.substr(i + 1));
    }

    GridDocumentTemplate.Refresh();
}

function GridDocumentTemplatesClientCustomButton_Click(s, e) {
    if (e.buttonID == "GridDocuments_DeleteSelected") {
        GridDocumentTemplate.GetRowValues(e.visibleIndex, 'Id', DeleteSelectedDocumentTemplate)
    }
}

function DeleteSelectedDocumentTemplate(idDocumentTemplate) {
    try {
        actualDocument = idDocumentTemplate;
        ShowRemoveDocumentTemplate();
    } catch (e) {
        showError("CargaGridDocumentTemplate", e);
    }
}

//Oculta los detalles de una tarea y muestra el grid
function BackToGrid() {
    var Url = "Handlers/srvDocumentTemplates.ashx?action=getBarButtons&ID=-1";
    AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtonsMin(objContainerId,-1)");
    //AsyncCall("POST", Url, "CONTAINER", "ctl00_contentMainBody_tbButtons", "");

    var divGridTasks = document.getElementById("ctl00_contentMainBody_divGridDocumentTemplates");
    var divMenuTask = document.getElementById("documentRow");
    divGridTasks.style.display = '';
    divMenuTask.style.display = 'none';
    ASPxCallbackPanelContenidoClient.SetVisible(false);

    $("#readOnlyNameDocumentTemplate").text($("#hdnCaptionGrid").val());
    document.getElementById("ctl00_contentMainBody_IDLoadDocumentTemplate").value = "-1";
    refreshGrid();
}

function parseResponseBarButtonsMin(oResponse, IDDocumentTemplate) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);
}

function ShowDetail() {
    var divGridTasks = document.getElementById("ctl00_contentMainBody_divGridDocumentTemplates");
    var divMenuTask = document.getElementById("documentRow");
    divGridTasks.style.display = 'none';
    divMenuTask.style.display = '';
    ASPxCallbackPanelContenidoClient.SetVisible(true);
}

function refreshGrid() {
    try {
        GridDocumentTemplate.Refresh();
    } catch (e) {
        showError("refreshGrid", e);
    }
}

function GetDocumentTemplateDetails(s, e) {
    s.GetRowValues(e.visibleIndex, 'Id', CargaGridDocumentTemplate);
}

function CargaGridDocumentTemplate(idDocumentTemplate) {
    try {
        if (idDocumentTemplate == null) {
            showError("CargaGridDocumentTemplate", e);
        } else {
            cargaDocumentTemplate(idDocumentTemplate);
        }
    } catch (e) {
        showError("CargaGridDocumentTemplate", e);
    }
}