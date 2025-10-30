var jsGridTaskFields = null;

function checkTaskTemplateEmptyName(newName) {
    document.getElementById("readOnlyNameTaskTemplate").textContent = newName;
    hasChanges(true);
}

//Carga Tabs y contenido Empleados
function cargaTaskTemplate(IDTaskTemplate) {
    actualType = "T";
    actualTask = IDTaskTemplate;
    actualTabTask = 0;
    showLoadingGrid(true);
    cargaTaskTemplateTabSuperior(IDTaskTemplate);
}

// Carrega la part del TAB grisa superior
function cargaTaskTemplateTabSuperior(IDTaskTemplate) {
    try {
        var Url = "Handlers/srvTaskTemplates.ashx?action=getTaskTemplateTab&aTab=" + actualTabTask + "&ID=" + IDTaskTemplate;
        AsyncCall("POST", Url, "CONTAINER", "divShifts", "cargaTaskBarButtons(" + IDTaskTemplate + ");");
    }
    catch (e) {
        showError("cargaTaskTemplateTabSuperior", e);
    }
}
var responseObjTask = null;
// Carrega la part del TAB grisa superior
function cargaTaskBarButtons(IDTaskTemplate) {
    try {
        var Url = "Handlers/srvTaskTemplates.ashx?action=getBarButtons&ID=" + IDTaskTemplate;
        AsyncCall("POST", Url, "JSON3", responseObjTask, "parseResponseBarButtonsTask(objContainerId," + IDTaskTemplate + ")");
    }
    catch (e) {
        showError("cargaTaskTemplateTabSuperior", e);
    }
}

function parseResponseBarButtonsTask(oResponse, IDTaskTemplate) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaTaskTemplateDivs(IDTaskTemplate);
}

// Carrega els apartats dels divs de l'usuari
function cargaTaskTemplateDivs(IDTaskTemplate) {
    var oParameters = {};
    oParameters.aTab = actualTabTask;
    oParameters.ID = actualTask;
    oParameters.oType = "T";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETTASKTEMPLATE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function changeTaskTemplateTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03');
    arrDivs = new Array('div20', 'div21', 'div22', 'div23');

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
    actualTabTask = numTab;
}

function changeTaskTemplateTabsByName(nameTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03');
    arrDivs = new Array('div20', 'div21', 'div22', 'div23');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (tab != null && div != null) {
            if (div.id == nameTab) {
                tab.className = 'bTab-active';
                div.style.display = '';
                actualTabTask = n;
            } else {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }
}

function SetTrackbarValue(s, e) {
    TrackBarPriority.SetValue(s.GetPosition());
}

function hasChangesTask(bolChanges) {
    var txtNameShift = document.getElementById('txtNameShift');

    var divTop = document.getElementById('divMsgTop');
    var divBottom = document.getElementById('divMsgBottom');

    var tagHasChanges = document.getElementById('msgHasChanges');
    var msgChanges = '<changes>';
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setMessage(msgChanges); //'Se han realizado cambios en el acumulado.');

    if (bolChanges) {
        divTop.style.display = '';
        divBottom.style.display = '';
        document.getElementById('divContentPanels').className = "divContentPanelsWithMessage";
    } else {
        divTop.style.display = 'none';
        divBottom.style.display = 'none';
        document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
    }
}

function deleteGridTaskFields(idRow) {
    try {
        var url = "TaskTemplates/srvMsgBoxTask.aspx?action=Message";
        url = url + "&TitleKey=deleteTaskField.Title";
        url = url + "&DescriptionKey=deleteTaskField.Description";
        url = url + "&Option1TextKey=deleteTaskField.Option1Text";
        url = url + "&Option1DescriptionKey=deleteTaskField.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelTaskFields('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deleteTaskField.Option2Text";
        url = url + "&Option2DescriptionKey=deleteTaskField.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteGridTaskFieldsList", e); }
}

function delSelTaskFields(idRow) {
    try {
        try {
            if (actualType == "T") {
                jsGridTaskFields.deleteRow(idRow);
            }
            else {
                jsUserFieldsProjectTemplate.deleteRow(idRow);
            }

            hasChanges(true);
        } catch (e) { }
    } catch (e) { showError("delSelTaskFields", e); }
}

function saveChangesTask() {
    try {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            showLoadingGrid(true);

            if (ProjectTaskConfigClient.Get("IDProjectTemplate") < 0) {
                showErrorPopup("Error.NoProjectSelected", "error", "Error.Description.NoProjectSelected", "Error.OK", "Error.OKDesc", "");
            }
            else {
                var arrTaskFields = null;
                if (jsGridTaskFields != null) { arrTaskFields = jsGridTaskFields.toJSONStructureAdvanced(); }

                var oParameters = {};
                oParameters.aTab = actualTabTask;
                oParameters.ID = actualTask;
                oParameters.Name = document.getElementById("readOnlyNameTaskTemplate").textContent.trim();
                oParameters.StampParam = new Date().getMilliseconds();
                oParameters.oType = "T";
                oParameters.action = "SAVETASKTEMPLATE";
                if (arrTaskFields != "undefined" && arrTaskFields != null) oParameters.taskFields = arrTaskFields;

                var strParameters = JSON.stringify(oParameters);
                strParameters = encodeURIComponent(strParameters);
                ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
            }
        } else {
            showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        }
    } catch (e) { alert("saveChanges: " + e); }
}

function undoChangesTask() {
    try {
        if (actualTask == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesTaskTemplates";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaTaskTemplate(actualTask);
        }
    } catch (e) { showError("undoChangesConcepts", e); }
}

function checkStatusTask(oStatus) {
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
        }
    }
    catch (e) {
        showError("checkStatus", e);
    }
}

function createTaskTemplateGrid(arrCC) {
    try {
        if (arrCC == null) { return; }

        var hdGridUFields = [{ 'fieldname': 'Name', 'description': '', 'size': '100%' }];
        hdGridUFields[0].description = document.getElementById('hdnHdrZoneName').value;
        jsGridTaskFields = new jsGrid('ctl00_contentMainBody_ASPxCallbackPanelContenido_grdUserFieldsTaskTemplate', hdGridUFields, arrCC, false, true, false, 'TaskFields');
    } catch (e) { showError('createTaskTemplateGrid', e); }
}

function EnablePanelsFunctionallity() {
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivAllways');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByDate');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByEndTask');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByIniTask');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivAllways,ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByDate,ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByEndTask,ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByIniTask');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optClosingAllways');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optClosingByDate');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optClosingAllways,ctl00_contentMainBody_ASPxCallbackPanelContenido_optClosingByDate');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optColabOnlyOneEmp_optTypeCollabAny');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optColabOnlyOneEmp_optTypeCollabFirst');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optColabOnlyOneEmp_optTypeCollabAny,ctl00_contentMainBody_ASPxCallbackPanelContenido_optColabOnlyOneEmp_optTypeCollabFirst');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optColabOnlyOneEmp');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optColabAllEmp');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optColabOnlyOneEmp,ctl00_contentMainBody_ASPxCallbackPanelContenido_optColabAllEmp');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAutEmpAll');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAutEmpSelect');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAutEmpAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_optAutEmpSelect');
}

function TaskTemplateChange(IDTask, NameTask) {
    if (document.getElementById("hdnControl").value == 1) {
        document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByIniTask_txtIniTask").value = "";
        document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByEndTask_txtEndTask").value = NameTask;
    } else if (document.getElementById("hdnControl").value == 2) {
        document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByIniTask_txtIniTask").value = NameTask;
        document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_optActivByEndTask_txtEndTask").value = "";
    }

    ProjectTaskConfigClient.Set("IdTaskActivationType", IDTask);
    document.getElementById("hdnControl").value = 0;
}

function newTaskTemplate() {
    try {
        if (ProjectTaskConfigClient.Get("IDProjectTemplate") != -1) {
            createType = "T";
            var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=TaskTemplate";
            NewObjectPopup_Client.SetContentUrl(contentUrl);
            NewObjectPopup_Client.Show();
        } else {
            showErrorPopup("Error.NoProjectSelected", "error", "Error.Description.NoProjectSelected", "Error.OK", "Error.OKDesc", "");
        }
    } catch (e) { showError('newTaskTemplate', e); }
}

function ShowRemoveTaskTemplate() {
    if (actualTask < 0) { return; }

    var url = "TaskTemplates/srvMsgBoxTask.aspx?action=Message";
    url = url + "&TitleKey=deleteTaskTemplate.Title";
    url = url + "&DescriptionKey=deleteTaskTemplate.Description";
    url = url + "&Option1TextKey=deleteTaskTemplate.Option1Text";
    url = url + "&Option1DescriptionKey=deleteTaskTemplate.Option1Description";
    url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteTaskTemplate('" + actualTask + "'); return false;";
    url = url + "&Option2TextKey=deleteTaskTemplate.Option2Text";
    url = url + "&Option2DescriptionKey=deleteTaskTemplate.Option2Description";
    url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function deleteTaskTemplate(Id) {
    try {
        if (Id > 0) {
            showLoadingGrid(true);
            AsyncCall("POST", "Handlers/srvTaskTemplates.ashx?action=deleteXTaskTemplate&ID=" + Id, "json", "arrStatus", "checkStatusTask(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
        }
    } catch (e) { showError('deleteTaskTemplate', e); }
}