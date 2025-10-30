var actualTab = "00"; // TAB per mostrar
var actualIdDatalinkType = '';

function showLoadingGrid(loading) { parent.showLoader(loading); }

//Cambia els Tabs i els divs
function changeTabs(numTab) {
    var arrDivs = new Array('div00', 'div01', 'div02');
    var arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (div.id == "ctl00_contentMainBody_ASPxCallbackPanelContenido_div" + numTab) {
            tab.className = 'bTab-active';
            div.style.display = '';
            actualTab = numTab;
        } else {
            if (tab != null) {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }
    if (actualTab == "02") {
        loadAdvTemplateContent();
    }


}

function getAvailableTab(selectedTab) {
    var arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02');
    var tab = document.getElementById('TABBUTTON_' + selectedTab);
    if (tab != null) {
        return selectedTab;
    }
    else {
        for (n = 0; n < arrButtons.length; n++) {
            var tab = document.getElementById(arrButtons[n]);
            if (tab != null) 
                {
                return arrButtons[n].replace('TABBUTTON_','');
                }
        }
    }

}

function cargaNodo(Nodo) {
    showLoadingGrid(false);
    if (Nodo.id == "source") Nodo.id = "";
    cargaDatalink(Nodo.id);
}

function cargaDatalink(IDDatalinkType) {
    actualIdDatalinkType = IDDatalinkType;
    if (actualIdDatalinkType != '') {
        showLoadingGrid(true);
        cargaTabSuperior(actualIdDatalinkType);
    }
}

var responseObj = null;
function cargaTabSuperior(IDDatalinkType) {
    try {
        var Url = "Handlers/srvDatalinkBusiness.ashx?action=getDatalinkTab&aTab=" + actualTab + "&Concept=" + IDDatalinkType;
        AsyncCall("POST", Url, "CONTAINER", "divDatalinkBusiness", "cargaDatalinkDivs('" + IDDatalinkType + "')");
    } catch (e) { showError("cargaTabSuperior", e); }
}

function cargaDatalinkDivs(IDDatalinkType) {
    var oParameters = {};
    actualTab = getAvailableTab(actualTab);
    oParameters.aTab = actualTab
    oParameters.Concept = IDDatalinkType;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETDATALINK";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    showLoadingGrid(false);

    enableAutomaticImport(ckImportEnableClient.GetChecked());
    //enableAutomaticExport(ckExportEnableClient.GetChecked());

    var oError = JSON.parse(s.cpError)

    if (oError.Error) {
        Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
            { text: '', key: 'roJsError' }, { text: oError.ErrorText, key: '' },
            { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
            Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
    }

    switch (s.cpAction) {
        default:
            hasChanges(false);
    }
}

function ASPxCallbackPanelContenidoClient_BeginCallBack(s, e) {
    showLoadingGrid(true);
}

function saveChanges() {
}

function undoChanges() {
    try {
        showLoadingGrid(true);
        hasChanges(false);
        cargaDatalink(actualIdDatalinkType);
    }
    catch (e) { showError("undoChanges", e); }
}

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesDataLinkExports";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, false, false);");
}

//Mostra el ToolTip a la barra d'eines
function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

//Amaga el ToolTip a la barra d'eines
function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function hasChanges(bolChanges) {
    //var divTop = document.getElementById('divMsgTop');
    //var divBottom = document.getElementById('divMsgBottom');

    //var tagHasChanges = document.getElementById('msgHasChanges');
    //var msgChanges = '<changes>';
    //if (tagHasChanges != null) {
    //    msgChanges = tagHasChanges.value;
    //}

    //setMessage(msgChanges);

    //if (bolChanges) {
    //    document.getElementById('divContentPanels').className = "divContentPanelsWithMessage";
    //    divTop.style.display = '';
    //    divBottom.style.display = '';
    //} else {
    //    document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
    //    divTop.style.display = 'none';
    //    divBottom.style.display = 'none';
    //}
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    }
    catch (e) { alert('setMessage: ' + e); }
}

function setStyleMessage(classMsg) {
    try {
        //divContainers styles
        var divTop = document.getElementById('divMsgTop');
        var divBottom = document.getElementById('divMsgBottom');

        divTop.className = classMsg;
        divBottom.className = classMsg;
    }
    catch (e) { alert('setStyleMessage: ' + e); }
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "DataLink/srvMsgBoxDataLink.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        url = url + "&DescriptionKey=" + Description;
        url = url + "&Option1TextKey=" + Opt1Text;
        url = url + "&Option1DescriptionKey=" + Opt1Desc;
        url = url + "&Option1OnClickScript=HideMsgBoxForm();" + strScript1 + "; return false;";
        if (Opt2Text != null) {
            url = url + "&Option2TextKey=" + Opt2Text;
            url = url + "&Option2DescriptionKey=" + Opt2Desc;
            url = url + "&Option2OnClickScript=HideMsgBoxForm();" + strScript2 + "; return false;";
        }
        if (Opt3Text != null) {
            url = url + "&Option3TextKey=" + Opt3Text;
            url = url + "&Option3DescriptionKey=" + Opt3Desc;
            url = url + "&Option3OnClickScript=HideMsgBoxForm();" + strScript3 + "; return false;";
        }
        if (typeIcon.toUpperCase() == "TRASH") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
        } else if (typeIcon.toUpperCase() == "ERROR") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
        } else if (typeIcon.toUpperCase() == "INFO") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
        }

        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
    catch (e) { showError("showErrorPopup", e); }
}

function ShowExportWizard(onlyProfile) {
    try {
        hasChanges(false);
        var Title = '';

        top.ShowExternalForm2('DataLink/Wizards/ExportWizard.aspx?IDExport=-1&Concept=' + actualIdDatalinkType + '&isBusiness=' + 1, 800, 550, Title, '', false, false, false);
    }
    catch (e) { showErrorPopup("ShowExportWizard", e); }
}

function ShowImportWizard() {
    try {
        var Title = '';
        showLoadingGrid(false);

        if (actualIdDatalinkType == 'Schedule') {
            sessionStorage.setItem("actualImport", -1);
            sessionStorage.setItem("actualConcept", actualIdDatalinkType);
            top.ShowExternalForm2('DataLink/Wizards/ImportWizard.aspx?IDImport=-1&Concept=' + actualIdDatalinkType + '&isBusiness=' + 1, 800, 710, Title, '', false, false, false);
        } else {
            sessionStorage.setItem("actualImport", -1);
            sessionStorage.setItem("actualConcept", actualIdDatalinkType);
            top.ShowExternalForm2('DataLink/Wizards/ImportWizard.aspx?IDImport=-1&Concept=' + actualIdDatalinkType + '&isBusiness=' + 1, 800, 450, Title, '', false, false, false);
        }
    }
    catch (e) { showErrorPopup("ShowImportWizard", e); }
}

//function ShowImportWizard() {
//    try {
//        hasChanges(false);

//        showLoadingGrid(true);
//        if (fImportUploadClient.GetSelectedFiles() != null && fImportUploadClient.GetSelectedFiles().length > 0) {
//            var file = fImportUploadClient.GetSelectedFiles()[0].sourceFileObject;  // file from input
//            var formData = new FormData();

//            formData.append("action", "uploadImportFile")
//            formData.append("importFile", file);

//            $.ajax({
//                type: 'POST',
//                url: 'Handlers/srvDatalinkBusiness.ashx', // Location of the service
//                timeout: 60000,
//                data: formData, //Data sent to server
//                cache: false,
//                contentType: false,
//                processData: false,
//                success: finallyImportFile,
//                error: function () { onDatalinkError('roNotUploadedFile'); }
//            });
//        } else {
//            onDatalinkError('roNoFilesSelected');
//        }

//    } catch (e) { }
//}

function onDatalinkError(errorKey) {
    showLoadingGrid(false);

    Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
        { text: '', key: 'roJsError' }, { text: '', key: errorKey },
        { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
        Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
}

function downloadSampleTemplate() {
    var fileName = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnSampleFileName').value;
    window.open("../Alerts/downloadFile.aspx?Action=template&filename=" + fileName);
}

function enableAutomaticImport(bEnable) {
    cmbImportTypeClient.SetEnabled(bEnable);
    txtfileOrigClient.SetEnabled(bEnable);
}

function enableAutomaticExport(bEnable) {
    ckApplyLockDateClient.SetEnabled(bEnable);
    cmbExportTypeClient.SetEnabled(bEnable);
    txtExportfileOrigClient.SetEnabled(bEnable);
    setSchedulePriodStatus(bEnable);
    optSchedule2_setEnabled(bEnable, 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optSchedule1');
    btnOpenPopupSelectorEmployeesClient.SetEnabled(bEnable);
}

function ShowCaptchaActivate() {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=SAVEAUTOMATICIMPORT";
    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
}

function ShowCaptchaActivateExport() {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=SAVEAUTOMATICEXPORT";
    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
}

function captchaCallback(action) {
    switch (action) {
        case "SAVEAUTOMATICIMPORT":
            try {
                saveImportGuide();
            } catch (e) { showError("saveChanges", e); }

            break;
        case "SAVEAUTOMATICEXPORT":
            try {
                saveExportGuide();
            } catch (e) { showError("saveChanges", e); }
            break;
        case "ERROR":
            window.parent.frames["ifPrincipal"].showErrorPopup("Error.Title", "ERROR", "Error.ValidationCodeFailed", "", "Error.OK", "Error.OKDesc", "");
            break;
    }
}

function saveImportGuide() {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.Concept = actualIdDatalinkType;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEIMPORTGUIDE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

async function btnOpenPopupSelectorEmployeesClient_Click() {
    PopupSelectorEmployeesClient.Show();
}

function profileChanged(s, e) {
    var eConcept = actualIdDatalinkType.toLowerCase();
    var eFormatDiv = document.getElementById('ctl00_contentMainBody_ExportScheduleEdit_ASPxPanel4_CallbackPopupOperations_divFormatExport');

    if (eConcept != "schedule" && eConcept != "requests" && eConcept != "customargal") {
        if (eConcept == "absences") {
            if (s.GetSelectedItem().value > 10) {
                $(eFormatDiv).show();
                cmbIsASCII();
            } else {
                $(eFormatDiv).hide();
                $('.trSeparator').hide();
            }
        } else {
            $(eFormatDiv).show();
            cmbIsASCII();
        }
    } else {
        $(eFormatDiv).hide();
        $('.trSeparator').hide();
    }
}

function cmbIsASCII() {
    var isASCII = false;
    if (cmbFormatExportClient.GetSelectedItem() != null && cmbFormatExportClient.GetSelectedItem().value == "2") isASCII = true;

    if (isASCII) {
        $('.divSeparator').show();
    } else {
        $('.divSeparator').hide();
    }
}

function GridExports_BeginCallback(e, c) {
    
}

function GridExports_EndCallback(s, e) {
    if (s.IsEditing()) { }
    else {
        if (s.cpAction == "ROWINSERTING" || s.cpAction == "ROWUPDATING" || s.cpAction == "ROWDELETE") {
            cargaDatalink(actualIdDatalinkType);
        }
    }
}

function GridExports_OnRowDblClick(s, e) {
    GridExportsClient.GetRowValues(e.visibleIndex, 'Id', loadExportSchedule);
}

function GridExports_CustomButtonClick(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        GridExportsClient.GetRowValues(e.visibleIndex, 'Id', loadExportSchedule);
    } else if (e.buttonID = "ViewLogDetailButton") {
        GridExportsClient.GetRowValues(e.visibleIndex, 'LastExecutionLog', showLastLog);
    }
}

function showLastLog(sLastLog) {
    txtLogExportClient.SetValue(sLastLog);
    PopupExportLogsViewClient.Show();
}

async function loadExportSchedule(id) {
    await getroTreeState('objContainerTreeV3_treeEmpDatalinkBusinessExport').then(roState => roState.reset());
    await getroTreeState('objContainerTreeV3_treeEmpDatalinkBusinessExportGrid').then(roState => roState.reset());
    showLoadingGrid(true);

    if (hdnScheduleIdClient.Contains("Id")) {
        hdnScheduleIdClient.Set("Id", id);
    } else {
        hdnScheduleIdClient.Add("Id", id);
    }

    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.Concept = actualIdDatalinkType;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETDATALINK";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    CallbackPopupOperationsClient.PerformCallback(strParameters);
}

function GridExports_FocusedRowChanged(s, e) {
}

function AcceptExportScheduleClick(s, e) {
    saveExportGuide();
}

function CancelExportScheduleClick(s, e) {
    hdnScheduleIdClient.Set("Id", 0);
    ExportScheduleEditClient.Hide();
}

//Agregar nueva fila en el grid de incidencias
async function AddNewExport(s, e) {
    showLoadingGrid(true);
    await getroTreeState('objContainerTreeV3_treeEmpDatalinkBusinessExport').then(roState => roState.reset());
    await getroTreeState('objContainerTreeV3_treeEmpDatalinkBusinessExportGrid').then(roState => roState.reset());

    if (hdnScheduleIdClient.Contains("Id")) {
        hdnScheduleIdClient.Set("Id", -1);
    } else {
        hdnScheduleIdClient.Add("Id", -1);
    }

    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.Concept = actualIdDatalinkType;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETDATALINK";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    CallbackPopupOperationsClient.PerformCallback(strParameters);
}

function CallbackPopupOperations_EndCallBack(s, e) {
    showLoadingGrid(false);

    if (s.cpResult == "NOK") {
        if (s.cpAction == "SAVEEXPORTGUIDE") {
            DevExpress.ui.notify("No se han indicado todos los datos necesarios para guaradar la planificación", "error", 3000);
        }
    } else {
        if (s.cpAction == "GETINFOSELECTED") {
            PopupSelectorEmployeesClient.Hide();
        } else if (s.cpAction == "GETDATALINK") {
            ExportScheduleEditClient.Show();
        } else if (s.cpAction == "SAVEEXPORTGUIDE") {
            ExportScheduleEditClient.Hide();
            cargaDatalink(actualIdDatalinkType);
        }
    }
}

function btnPopupSelectorEmployeesAcceptClient_Click() {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.Concept = actualIdDatalinkType;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETINFOSELECTED";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    CallbackPopupOperationsClient.PerformCallback(strParameters);
}

function saveExportGuide() {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.Concept = actualIdDatalinkType;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEEXPORTGUIDE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    CallbackPopupOperationsClient.PerformCallback(strParameters);
}

function cmbImportIsASCII() {
    var isASCII = cmbFormatImportClient.GetSelectedItem().value == "2";

    if ((actualIdDatalinkType == 'Employees' && isASCII) || (actualIdDatalinkType == 'Absences' && isASCII)) {
        $('.divImportType').hide();
    } else {
        $('.divImportType').show();
    }

    if (isASCII) {
        $('.divImportSeparator').show();
        $('.divFileTemplate').show();
    } else {
        $('.divImportSeparator').hide();
        $('.divFileTemplate').hide();
    }
}

function loadExportTemplates() {
    var oParameters = {};
    showLoadingGrid(true);

    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "LoadExportTemplates";
    oParameters.Concept = actualIdDatalinkType;
    oParameters.aTab = actualTab;

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

var overrideTemplateName = "";

function loadAdvTemplateNames() {
    var oParameters = {};
    showLoadingGrid(true);
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "LoadExportTemplatesAvailable";
    oParameters.newTemplateName = overrideTemplateName;
    oParameters.Concept = actualIdDatalinkType;
    oParameters.aTab = actualTab;
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function loadAdvTemplateContent() {
    var oParameters = {};
    showLoadingGrid(true);
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "LoadFileTemplateContent";
    oParameters.aTab = actualTab;
    oParameters.Concept = actualIdDatalinkType;
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
    var oParameters = {};
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
        oParameters.aTab = actualTab;
        oParameters.Concept = actualIdDatalinkType;


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
    var oParameters = {};
    overrideTemplateName = newObjectName_Client.GetText();
    if (overrideTemplateName != "") {
        showLoadingGrid(true);
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "DUPLICATE";
        oParameters.newTemplateName = overrideTemplateName;
        oParameters.aTab = actualTab;
        oParameters.Concept = actualIdDatalinkType;


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