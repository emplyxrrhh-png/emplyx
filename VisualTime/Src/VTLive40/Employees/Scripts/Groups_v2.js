var groupArrButtons = new Array('TABBUTTON_GeneralGroups', 'TABBUTTON_UserFields', 'TABBUTTON_GroupIndicators', 'TABBUTTON_CostCenters', 'TABBUTTON_CompanyDocuments', 'TABBUTTON_CompanyAccessAuthorizations', 'TABBUTTON_GroupDocuments');
var groupArrDivs = new Array('panCompanyGeneral', 'panEmployees', 'panGrpIndicators', 'panGrpCostCenters', 'panCompanyDocs', 'panCompanyAccess', 'panGroupDocs');
var groupDivsLoaded = new Array(false, false, false, false, false, false, false);

function checkCompanyEmptyName(newName) {
    //document.getElementById("readOnlyNameEmp").textContent = newName;
    hasChanges(true);
}

function cargaGrupo(IDGrupo) {
    for (n = 0; n < groupDivsLoaded.length; n++) {
        groupDivsLoaded[n] = false;
    }
    actualGrupo = IDGrupo;
    //TAB Gris Superior
    actualType = "G";
    showLoadingGrid(true);
    cargaGrupoTabSuperior(IDGrupo);
}

// Carrega la part del TAB grisa superior
function cargaGrupoTabSuperior(IDGrupo) {
    var Url = "";

    Url = "Handlers/srvGroups.ashx?action=getBarButtons&ID=" + IDGrupo;
    AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtonsGroups(objContainerId," + IDGrupo + ")");
}

function parseResponseBarButtonsGroups(oResponse, IDGrupo) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaGrupoDivCentral(IDGrupo);
}

function cargaGrupoDivCentral(IDGrupo) {
    var Url = "";

    Url = "Handlers/srvGroups.ashx?action=getGroupsTab&aTab=" + actualTabGrupo + "&ID=" + IDGrupo;
    AsyncCall2("POST", Url, "CONTAINER", "divEmpleados", "changeCompanyTabs(" + actualTabGrupo + ")");
}

//Cambia els Tabs i els divs
function changeCompanyTabs(numTab) {
    for (n = 0; n < groupArrButtons.length; n++) {
        var tab = document.getElementById(groupArrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + groupArrDivs[n]);
        if (n == numTab) {
            if (tab != null) tab.className = 'bTab-active';
            if (div != null) div.style.display = '';
        } else {
            if (tab != null) tab.className = 'bTab';
            if (div != null) div.style.display = 'none';
        }
    }
    actualTabGrupo = numTab;

    if (numTab == 0) {
    }

    if (groupDivsLoaded[numTab] == false) {
        showLoadingGrid(true);
        cargaGrupoDivs(actualGrupo, actualTabGrupo);
    }
}

// Carrega els apartats dels divs de l'usuari
function cargaGrupoDivs(IDGrupo, actualTabGrupo) {
    var oParameters = {};

    oParameters.aTab = actualTabGrupo;
    oParameters.ID = IDGrupo;
    oParameters.oType = "G";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETGROUP";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function EditIndicators(idGroup) {
    var Title = ''; //document.getElementById('spanMobility').innerHTML;
    parent.ShowExternalForm2('Employees/EmployeeGroupEditIndicators.aspx', 400, 400, '', 'GroupId=' + idGroup, false, false, false);
}

//Nou usuari wizard
function ShowNewGroupWizard() {
    var Title = '';
    var groupName = $("#readOnlyNameGroup").html();
    var url = 'Employees/Wizards/NewGroupWizard.aspx?GroupID=' + actualGrupo + '&GroupName=' + encodeURIComponent(groupName);
    top.ShowExternalForm2(url, 500, 450, Title, '', false, false, false);
}

//Nova empresa wizard
function ShowNewCompanyWizard() {
    var Title = '';
    top.ShowExternalForm2('Employees/Wizards/NewCompanyWizard.aspx', 500, 450, Title, '', false, false, false);
}

//Posa el nom del grup en mode edicio
function EditNameGroup(editMode) {
    if (editMode == 'true') {
        document.getElementById('NameReadOnly').style.display = 'none';
        document.getElementById('NameChange').style.display = '';
        document.getElementById('txtNameGroup').focus();
    } else {
        document.getElementById('NameReadOnly').style.display = '';
        document.getElementById('NameChange').style.display = 'none';
    }
}

//Graba el nom de l'usuari
function saveNameGroup(ID) {
    var txtName = document.getElementById('txtNameGroup').value;

    var stamp = '&StampParam=' + new Date().getMilliseconds();

    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvGroups.ashx?action=chgNameGroup&ID=" + ID + "&NewName=" + encodeURIComponent(txtName) + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            var strResponse = ajax.responseText;

            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.substr(7, strResponse.length - 7));
                parent.ShowMsgBoxForm(url, 500, 300, '');
            }
            else {
                if (strResponse == 'OK') {
                    document.getElementById('readOnlyNameGroup').textContent = txtName;
                    EditName('false');
                    // Modificamos el nombre en los árboles d'empleado
                    refreshEmployeeTree(true);
                }
            }
        }
    }
    ajax.send(null)
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function editGroupFieldsGrid(IDGroup) {
    showLoadingUserFields(true);

    var oParameters = {};
    oParameters.aTab = actualTabGrupo;
    oParameters.ID = actualGrupo;
    oParameters.oType = "G";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "EDITGRID";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function saveGroupFieldsGrid(IDGroup) {
    showLoadingGrid(true);

    var contenedor;
    contenedor = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divCompanyFields');

    arrInputs = new Array();
    arrInputs = contenedor.getElementsByTagName('input');
    var nTotalFields = arrInputs.length;

    var postvars = '';
    for (n = 0; n < nTotalFields; n++) {
        if (arrInputs[n].id.toLowerCase().endsWith("_state") == false) {
            postvars += '&USR_' + arrInputs[n].id.replace('ctl00_contentMainBody_ASPxCallbackPanelContenido_', '') + '=' + arrInputs[n].value;
        }
    }

    var oParameters = {};
    oParameters.aTab = actualTabGrupo;
    oParameters.ID = actualGrupo;
    oParameters.oType = "G";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVECOMPANYFIELDS";
    oParameters.resultClientAction = postvars;
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cancelGroupFieldsGrid() {
    showLoadingGrid(true);
    actualTabGrupo = 1;
    cargaGrupoDivs(actualGrupo, actualTabGrupo);
}

function saveChangesGroup() {
    try {
        if (actualTabGrupo == 0) {
            if (ASPxClientEdit.ValidateGroup('employeenameGroup', true)) {
                if (saveGroup() == true) {
                    hasChanges(false);
                }
            } else {
                showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
            }
        }
        if (actualTabGrupo != 0) {
            showCaptcha();
        }
    } catch (e) { showError("saveChanges", e); }
}

function showCaptcha() {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=CHANGECOSTCENTER&ShowFreezingDate=1";
    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
}

function captchaCallback(action) {
    switch (action) {
        case "CHANGECOSTCENTER":
            if (saveGroup() == true) {
                hasChanges(false);
            }
            break;
        case "ERROR":
            showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "", "");
            break;
    }
}

function undoChangesGroup() {
    try {
        showLoadingGrid(true);
        cargaGrupoDivs(actualGrupo, actualTabGrupo);
    } catch (e) { showError("undoChanges", e); }
}

function saveGroup() {
    var oParameters = {};
    oParameters.aTab = actualTabGrupo;
    oParameters.ID = actualGrupo;
    oParameters.oType = "G";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEGROUP";
    oParameters.resultClientAction = "";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Carga empleado y Tabs desde la pantalla de grupos (reposiciona el arbol)
function cargaEmpleado2(IDEmpleado) {
    //Reposiciona l'arbre principal
    let url = $("#ctl00_contentMainBody_EmployeeURI").val();

    $.ajax({
        url: `/Employee/GetEmployeeTreeSelectionPath/${IDEmpleado}`,
        data: {},
        type: "GET",
        dataType: "json",
        success: async (data) => {
            if (typeof data != 'string') {

                let treeState = await getroTreeState("ctl00_contentMainBody_roTrees1", true);
                await treeState.setRedirectData(data.EmployeePath, data.GroupSelectionPath);

                parent.window.open(url + "?IDEmployee=" + IDEmpleado + "&TabEmployee=1");

            } else {
                DevExpress.ui.notify(data, 'error', 2000);
            }
        },
        error: (error) => console.error(error),
    });

}

//Mostra el MsgBoxForm per confirmar eliminacio de l'usuari
function ShowRemoveGroup(ID) {
    var url = "Employees/srvMsgBoxEmployees.aspx?action=DeleteGroup&ID=" + ID;
    top.ShowMsgBoxForm(url, 400, 300, '');
}

function removeGroup(ID) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvGroups.ashx?action=deleteGroup&ID=" + ID + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            var strResponse = ajax.responseText;

            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.substr(7, strResponse.length - 7));
                parent.ShowMsgBoxForm(url, 500, 300, '');
            }
            else {
                if (strResponse == 'OK') {
                    //ACTUALIZAR A OTRO GRUPO, etc.
                    refreshEmployeeTree(true);
                }
            }
        }
    }

    ajax.send(null)
}

function EnablePanelsFunctionallity() {
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optOneCost');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHere');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHere,ctl00_contentMainBody_ASPxCallbackPanelContenido_optOneCost');
}

function ShowActivities() {
    if (actualGrupo > 0) {
        parent.ShowExternalForm2('../Documents/ActivitiesTreeSelector.aspx?source=company&id=' + actualGrupo, 400, 400, '', '', false, false);
    }
}

function AddNewCompanyDocument(idScope, newRecord, scope) {
    var url = 'Employees/EditDocument.aspx?ScopeID=' + idScope + '&NewRecord=' + newRecord + '&Scope=' + scope;
    var Title = '';
    parent.ShowExternalForm2(url, 440, 500, Title, '', false, false, false);
}

function refreshEmployeeTree(bolReload) {
    var ctlPrefix = "ctl00_contentMainBody_roTreeGroups1";
    if (bolReload) {
        eval(ctlPrefix + "_roTrees.LoadTreeViews(true,true,true,true);");
    } else {
        eval(ctlPrefix + "_roTrees.LoadTreeViews(false,false,false,false);");
    }
}