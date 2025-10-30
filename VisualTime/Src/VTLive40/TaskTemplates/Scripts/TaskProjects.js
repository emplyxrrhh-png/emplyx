var actualTabProject = 0; // TAB per mostrar els grups
var actualProject; // Grup actual

var actualTabTask = 0; // TAB per mostrar els grups
var actualTask; // Grup actual

var actualType = "";
var newObjectName = "";
var createType = "T";

function cargaNodo(Nodo) {
    if (Nodo.id.toUpperCase() == "SOURCE") {
        newProject();
        return;
    }

    var id = Nodo.id.substring(1);
    if (Nodo.id.substring(0, 1) == "A") { //Proyecto
        cargaProject(id);
    } else { //Tarea
        cargaTaskTemplate(id);
    }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        if (createType == "T") {
            cargaTaskTemplate(-1);
        } else if (createType == "P") {
            cargaProject(-1);
        }
        newObjectName = ObjName;
    } catch (e) { showError('newConcept', e); }
}

async function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    ConvertControls('divContenido');

    await getroTreeState('objContainerTreeV3_treeEmpDetailTaskGrid').then(roState => roState.reset());

    if (actualType == "T") {
        if (s.cpActionRO == "GETTASKTEMPLATE" || s.cpActionRO == "SAVETASKTEMPLATE") {
            EnablePanelsFunctionallity();

            txtProjectName_Client.SetValue("___");

            if (s.cpResultRO == "OK") {
                if (s.cpIsNewRO == true) {
                    refreshTree();
                }

                if (s.cpNameRO != null && s.cpNameRO != "") {
                    var arrF = null;
                    if (s.cpFieldsGridRO != "") eval("arrF = [" + s.cpFieldsGridRO + "]");
                    else arrF = new Array();
                    createTaskTemplateGrid(arrF);

                    document.getElementById("readOnlyNameTaskTemplate").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    createTaskTemplateGrid(new Array());
                    document.getElementById("readOnlyNameTaskTemplate").textContent = newObjectName;
                    hasChanges(true);
                    txtTaskTemplateName_Client.SetValue(newObjectName);
                }
            } else {
                var result = null;
                eval("result  = [" + s.cpMessageRO + "]");
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameTaskTemplate").textContent = s.cpNameRO;
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    document.getElementById("readOnlyNameTaskTemplate").textContent = newObjectName;
                    txtTaskTemplateName_Client.SetValue(newObjectName);
                }
                var arrF = null;
                if (s.cpFieldsGridRO != "") eval("arrF = [" + s.cpFieldsGridRO + "]");
                else arrF = new Array();
                createTaskTemplateGrid(arrF);
                hasChanges(true);

                checkStatusTask(result);
            }
        }
    } else {
        if (s.cpActionRO == "GETPROJECT" || s.cpActionRO == "SAVEPROJECT") {
            txtTaskTemplateName_Client.SetValue("___");
            txtShortName_Client.SetValue("___");

            if (s.cpResultRO == "OK") {
                if (s.cpIsNewRO == true) {
                    refreshTree();
                }

                if (s.cpNameRO != null && s.cpNameRO != "") {
                    var arrF = null;
                    if (s.cpFieldsGridRO != "") eval("arrF = [" + s.cpFieldsGridRO + "]");
                    else arrF = new Array();
                    createProjectTemplateGrid(arrF);

                    document.getElementById("readOnlyNameProject").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    createProjectTemplateGrid(new Array());

                    document.getElementById("readOnlyNameProject").textContent = newObjectName;
                    hasChanges(true);
                    txtProjectName_Client.SetValue(newObjectName);
                }
            } else {
                var result = null;
                eval("result  = [" + s.cpMessageRO + "]");
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameProject").textContent = s.cpNameRO;
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    document.getElementById("readOnlyNameProject").textContent = newObjectName;
                    txtProjectName_Client.SetValue(newObjectName);
                }
                var arrF = null;
                if (s.cpFieldsGridRO != "") eval("arrF = [" + s.cpFieldsGridRO + "]");
                else arrF = new Array();
                createProjectTemplateGrid(arrF);
                hasChanges(true);

                checkStatusProjects(result);
            }
        }
    }
    showLoadingGrid(false);
}

function hasChanges(bolChanges) {
    if (actualType == "T") {
        hasChangesTask(bolChanges);
    } else if (actualType == "P") {
        hasChangesProjects(bolChanges);
    }
}

function saveChanges() {
    try {
        if (actualType == "T") {
            saveChangesTask();
        } else if (actualType == "P") {
            saveChangesProjects();
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChanges() {
    try {
        if (actualType == "T") {
            undoChangesTask();
        } else if (actualType == "P") {
            undoChangesProjects();
        }
    } catch (e) { showError("undoChanges", e); }
}

function changeTabs(numTab) {
    if (actualType == "T") {
        changeTaskTemplateTabs(numTab);
    } else if (actualType == "P") {
        changeProjectTabs(numTab);
    }
}

function changeTabsByName(nameTab) {
    if (actualType == "T") {
        changeTabsByNameShifts(nameTab);
    } else if (actualType == "P") {
        changeProjectTabsByName(nameTab);
    }
}

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function renameSelectedNode(txt) {
    // Modificamos el nombre en los árboles d'empleado
    var ctlPrefix = "ctl00_contentMainBody_roTreesTaskTemplates";
    eval(ctlPrefix + "_roTrees.RenameSelectedNode('" + txt + "');");
}

function refreshTree() {
    eval('ctl00_contentMainBody_roTreesTaskTemplates_roTrees.LoadTreeViews(true, true, true);');
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesTaskTemplates";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "TaskTemplates/srvMsgBoxTask.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        if (DescriptionKey != "") url = url + "&DescriptionKey=" + DescriptionKey;
        if (DescriptionText != "") url = url + "&DescriptionText=" + DescriptionText;
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
    } catch (e) { showError("showErrorPopup", e); }
}

function LaunchTaskTemplateWizard() {
    let Title = '';
    top.ShowExternalForm2('TaskTemplates/Wizards/LaunchTaskTemplateWizard.aspx?IDEmployeeSource=' + 4, 850, 515, Title, '', false, false, false);
}

function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
            let oTasks;
            if (oParms != "") {
                oTasks = eval(oParms);
                if (actualType == "P") {
                    if (jsUserFieldsProjectTemplate == null) { createGridProjectFields(); }
                    jsUserFieldsProjectTemplate.addRows(oTasks, 'idtasktemplatefield', true);
                } else {
                    if (jsGridTaskFields == null) { createGridTaskFields(); }
                    jsGridTaskFields.addRows(oTasks, 'idtasktemplatefield', true);
                }
                hasChanges(true);
            }
        }
    } catch (e) { showError("RefreshScreen", e); }
}

function ShowTasksSelector(TypeActivationTask) {
    try {
        hasChanges(true);
        let Title = '';
        $("#hdnControl").val(TypeActivationTask);
        let hBase = "Base/WebUserControls/roFilterTaskTemplateSelector.aspx?IDActualProject=" + ProjectTaskConfigClient.Get("IDProjectTemplate") + "&IDActualTask=" + actualTask;
        top.ShowExternalForm2(hBase, 300, 270, Title, '', false, false, false);
    }
    catch (e) {
        showError("ShowTasksSelector", e);
    }
}

function TaskDetailShowSelector() {
    try {
        let Title = '';
        let iFrm = document.getElementById('ctl00_contentMainBody_RoPopupFrame1_GroupSelectorFrame');
        iFrm.style.width = "475px";
        iFrm.style.height = "290px";

        iFrm.style.top = "5px";
        iFrm.style.left = "5px";

        let strBase = "../Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" +
            "PrefixTree=treeEmpDetailTask&FeatureAlias=Tasks&PrefixCookie=objContainerTreeV3_treeEmpDetailTaskGrid";
        strBase += '&FilterFixed=Employees.Type="J"';
        iFrm.src = strBase;

        $find('RoPopupFrame1Behavior').show();
        document.getElementById("ctl00_contentMainBody_RoPopupFrame1").style.display = "";
    }
    catch (e) {
        showError("TaskDetailShowSelector", e);
    }
}

async function HideGroupSelector() {
    try {
        $find('RoPopupFrame1Behavior').hide();
        document.getElementById("ctl00_contentMainBody_RoPopupFrame1").style.display = "none";

        //comprobar si se ha seleccionado algo
        await getroTreeState('objContainerTreeV3_treeEmpDetailTaskGrid').then(roState => roState.reload());
        let oTreeState = await getroTreeState('objContainerTreeV3_treeEmpDetailTaskGrid'); 
        let nodes = oTreeState.getSelected('1');

        hasChanges(true);

        if (nodes == "") {
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_optAutEmpSelect_lblEmpSelect").text($("#hdnSeleccionar").val().split(",")[0]); //Seleccionar...
        }
        else {
            //Obtener cantidad total de empleados asignados a la tarea
            showLoadingGrid(true);
            AsyncCall("POST", "Handlers/srvTaskTemplates.ashx?action=getEmployeesSelected&NodesSelected=" + nodes + "&IDTaskTemplate=" + actualTask, "json", "arrStatus", "AfterSelectEmployees(arrStatus);");
        }
    }

    catch (e) {
        showLoadingGrid(false);
        showError("HideGroupSelector", e);
    }
}

function AfterSelectEmployees(arrStatus) {
    try {
        showLoadingGrid(false);
        if (arrStatus[0].error == 'false') {
            arrStatus[0].msg.substring(3)
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_optAutEmpSelect_lblEmpSelect").text(arrStatus[0].msg.substring(3) + " " + $("#hdnSeleccionar").val().split(",")[1]);
        }
    }
    catch (e) {
        showLoadingGrid(false);
        showError('AfterSelectEmployees', e);
    }
}

function ShowTemplateUserFieldsWizard(bIsProject) {
    try {
        let Title = '';
        let IDObject = -1;
        let currentNodes = "";

        let tableRows;
        let parentId;
        if (bIsProject) {
            IDObject = actualProject;
            tableRows = jsUserFieldsProjectTemplate.getRows();
            parentId = "-1";
        } else {
            IDObject = actualTask;
            tableRows = jsGridTaskFields.getRows();
            parentId = ProjectTaskConfigClient.Get("IDProjectTemplate");
        }

        for (let index = 0; index < tableRows.length; index++) {
            let curElem = tableRows[index];
            if (index > 0) currentNodes = currentNodes + ",";
            currentNodes = currentNodes + curElem.getAttribute("jsgridatt_idtasktemplatefield");
        }

        top.ShowExternalForm2('TaskTemplates/Wizards/SelectTaskFieldsWizard.aspx?IsProject=' + bIsProject + "&ID=" + IDObject + "&ParentId=" + parentId + "&SelectedNodes=" + currentNodes, 500, 450, Title, '', false, false, false);
    } catch (e) { showError("ShowTemplateUserFieldsWizard", e); }
}