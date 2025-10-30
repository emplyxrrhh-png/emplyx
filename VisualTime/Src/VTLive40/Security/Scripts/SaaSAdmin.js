var oParameters = {};

function showLoadingGrid(loading) { parent.showLoader(loading); }

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    if (s.cpResultRO == "OK") {
        overrideTemplateName = "";
    } else {
        var message = s.cpResultRO.substr(7, s.cpResultRO.length - 7)
        var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(message);
        parent.ShowMsgBoxForm(url, 500, 300, '');
    }

    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_saasConfig').hide();
    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_templateAdmin').hide();

    var tab = $('#ctl00_contentMainBody_SaaSAdmin_TabVisibleName').val();

    $('#' + tab).show();

    showLoadingGrid(false);
}

function activateService() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "ActivateService";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();

    //ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cancelService() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "CancelService";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();

    //ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function regeneratePwd() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "RegeneratePwd";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();

    //ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function changeMode() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "ChangeMode";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function changeGenius() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "ChangeGenius";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function deactivatePunchRequests() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "DeactivatePunchRequests";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function activatePunchRequests() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "ActivatePunchRequests";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function deleteSupervisorExceptions() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "DeleteExceptions";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function generateNodes() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GenerateNode";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function saveParameter() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SaveParameter";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function saveUser() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SaveUser";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function queryParameter() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "queryParameter";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function doSimpleBackup() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "DoSimpleBackup";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();

    //ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function doFullBackup() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "DoFullBackup";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();

    //ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function PerformAction() {
    showLoadingGrid(true);
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);;
}

function cmbPassportsClient_ValueChanged(S, E) {
}

function cmbLevelClient_ValueChanged(S, E) {
}

function deleteAlternativeShifts() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "DeleteAlternativeShifts";

    var contentUrl = "../Security/SaaSAdminCaptcha.aspx";
    PopupCaptcha_Client.SetContentUrl(contentUrl);
    PopupCaptcha_Client.Show();
}

function loadExportTemplates() {
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "LoadExportTemplates";

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

var overrideTemplateName = "";

function loadAdvTemplateNames() {
    showLoadingGrid(true);
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "LoadExportTemplatesAvailable";
    oParameters.newTemplateName = overrideTemplateName;

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function loadAdvTemplateContent() {
    showLoadingGrid(true);
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "LoadFileTemplateContent";

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function CallbackExcel_CallbackComplete(s, e) {
    if (s.cpResultRO == "ERROR") {
        if (s.cpActionRO == "DUPLICATE") {
            DevExpress.ui.notify("El nombre de la plantilla ya existe", "error", 3000);
        } else {
            DevExpress.ui.notify("Se ha producido un error desconocido al guardar el fichero", "error", 3000);
        }
    } else {
        PopupNewTemplateNameClient.Hide();
        DevExpress.ui.notify("Operación realizada con éxito", "success", 3000);
        if (s.cpActionRO == "DUPLICATE") {
            loadAdvTemplateNames();
        }
    }

    showLoadingGrid(false);
}

function saveCurrentTemplateFile() {
    advTemplateClient.ApplyCellEdit();
    setTimeout(function () {
        if (!!advTemplateClient.documentChanged) {
            advTemplateClient.buttonIsClicked = true;
        } 
    }, 0);      
    if (cmbAdvTemplateNameClient.GetSelectedItem() != null) {
        showLoadingGrid(true);
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SAVEEDITOR";

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);
        CallbackExcelClient.PerformCallback(strParameters);
    } else {
        DevExpress.ui.notify("No hay ninguna plantilla seleccionada", "error", 3000);
    }
}

function duplicateCurrentTemplate() {
    advTemplateClient.ApplyCellEdit();
    setTimeout(function () {
        if (!!advTemplateClient.documentChanged) {
            advTemplateClient.buttonIsClicked = true;
        }
    }, 0);      
    if (cmbAdvTemplateNameClient.GetSelectedItem() != null) {
        PopupNewTemplateNameClient.Show();
    } else {
        DevExpress.ui.notify("No hay ninguna plantilla seleccionada", "error", 3000);
    }
}

function AcceptTemplateNameClick(s, e) {
    overrideTemplateName = newObjectName_Client.GetText();
    if (overrideTemplateName != "") {
        showLoadingGrid(true);
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "DUPLICATE";
        oParameters.newTemplateName = overrideTemplateName;

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);
        CallbackExcelClient.PerformCallback(strParameters);
    } else {
        DevExpress.ui.notify("El nombre no puede estar vacío", "error", 3000);
    }
}

function CancelTemplateNameClick(s, e) {
    PopupNewTemplateNameClient.Hide();
}

function onDocumentChanged(s, e) {    
    s.documentChanged = true;
}
function onEndSynchronization(s, e) {    
    if (!!s.buttonIsClicked) {        
        s.buttonIsClicked = false;
    }
    s.documentChanged = false;
}  