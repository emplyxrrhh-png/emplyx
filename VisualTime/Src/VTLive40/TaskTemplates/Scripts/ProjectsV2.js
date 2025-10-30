var jsUserFieldsProjectTemplate = null;

function checkProjectEmptyName(newName) {
    document.getElementById("readOnlyNameProject").textContent = newName;
    hasChanges(true);
}

//Carga Tabs y contenido Empleados
function cargaProject(IDProject) {
    actualType = "P";
    actualProject = IDProject;
    showLoadingGrid(true);
    cargaProjectTabSuperior(IDProject);
}

// Carrega la part del TAB grisa superior
function cargaProjectTabSuperior(IDProject) {
    try {
        var Url = "Handlers/srvProjects.ashx?action=getProjectsTab&aTab=" + actualTabProject + "&ID=" + IDProject;
        AsyncCall2("POST", Url, "CONTAINER", "divShifts", "cargaProjectTabButtons(" + IDProject + ")");
    }
    catch (e) {
        showError("cargaProjectTabSuperior", e);
    }
}
var responseObj = null;
// Carrega la part del TAB grisa superior
function cargaProjectTabButtons(IDProject) {
    try {
        var Url = "Handlers/srvProjects.ashx?action=getBarButtons&ID=" + IDProject;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDProject + ")");
    }
    catch (e) {
        showError("cargaProjectTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IDProject) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaProjectDivs(IDProject);
}

// Carrega els apartats dels divs de l'usuari
function cargaProjectDivs(IDProject) {
    var oParameters = {};
    oParameters.aTab = actualTabProject;
    oParameters.ID = actualProject;
    oParameters.oType = "P";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETPROJECT";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function changeProjectTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('div00', 'div01');

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
    actualTabProject = numTab;
}

function changeProjectTabsByName(nameTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('div00', 'div01');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (tab != null && div != null) {
            if (div.id == nameTab) {
                tab.className = 'bTab-active';
                div.style.display = '';
                actualTabProject = n;
            } else {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }
}

function hasChangesProjects(bolChanges) {
    var divTop = document.getElementById('divMsgTop');
    var divBottom = document.getElementById('divMsgBottom');
    var divContenido = document.getElementById('divContenido');

    var tagHasChanges = document.getElementById('msgHasChanges');
    var msgChanges = '<changes>';
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setMessage(msgChanges);

    if (bolChanges) {
        divTop.style.display = '';
        divBottom.style.display = '';
        document.getElementById('divContentPanels').className = "divContentPanelsWithMessage";
    }
    else {
        divTop.style.display = 'none';
        divBottom.style.display = 'none';
        document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
    }
}

function saveChangesProjects() {
    try {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            showLoadingGrid(true);
            var arrTaskFields = null;
            if (jsUserFieldsProjectTemplate != null) { arrTaskFields = jsUserFieldsProjectTemplate.toJSONStructureAdvanced(); }

            var oParameters = {};
            oParameters.aTab = actualTabProject;
            oParameters.ID = actualProject;
            oParameters.Name = document.getElementById("readOnlyNameProject").textContent.trim();
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.oType = "P";
            oParameters.action = "SAVEPROJECT";
            if (arrTaskFields != "undefined" && arrTaskFields != null) oParameters.taskFields = arrTaskFields;

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);
            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        }
    } catch (e) { showError("saveChangesGroup", e); }
}

function undoChangesProjects() {
    try {
        if (actualProject == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesTaskTemplates";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaProject(actualProject);
        }
    } catch (e) { showError("undoChangesConcepts", e); }
}

function checkStatusProjects(oStatus) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "TaskTemplates/srvMsgBoxTask.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            }
            else { //Missatge estil inline
            }
        }
    }
    catch (e) {
        showError("checkStatus", e);
    }
}

function createProjectTemplateGrid(arrCC) {
    try {
        if (arrCC == null) { return; }

        var hdGridUFields = [{ 'fieldname': 'Name', 'description': '', 'size': '100%' }];
        hdGridUFields[0].description = document.getElementById('hdnHdrZoneName').value;
        jsUserFieldsProjectTemplate = new jsGrid('ctl00_contentMainBody_ASPxCallbackPanelContenido_grdUserFieldsProject', hdGridUFields, arrCC, false, true, false, 'TaskFields');
    } catch (e) { showError('createTaskTemplateGrid', e); }
}

function ShowNewProject() {
    createType = "P";
    var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=Project";
    NewObjectPopup_Client.SetContentUrl(contentUrl);
    NewObjectPopup_Client.Show();
}

//Mostra el MsgBoxForm per confirmar eliminacio de l'usuari
function ShowRemoveProject() {
    if (actualProject < 1) { return; }

    var url = "TaskTemplates/srvMsgBoxTask.aspx?action=Message";
    url = url + "&TitleKey=deleteProjectTemplate.Title";
    url = url + "&DescriptionKey=deleteProjectTemplate.Description";
    url = url + "&Option1TextKey=deleteProjectTemplate.Option1Text";
    url = url + "&Option1DescriptionKey=deleteProjectTemplate.Option1Description";
    url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].removeProject('" + actualProject + "'); return false;";
    url = url + "&Option2TextKey=deleteProjectTemplate.Option2Text";
    url = url + "&Option2DescriptionKey=deleteProjectTemplate.Option2Description";
    url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function removeProject(ID) {
    try {
        if (ID > 0) {
            showLoadingGrid(true);
            AsyncCall("POST", "Handlers/srvProjects.ashx?action=deleteProject&ID=" + ID, "json",
                "arrStatus",
                "checkStatusProjects(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode();cargaProject(-1); }");
        }
    } catch (e) { showError('deleteProject', e); }
}