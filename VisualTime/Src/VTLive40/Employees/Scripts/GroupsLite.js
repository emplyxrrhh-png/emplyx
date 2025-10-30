var groupArrButtons = new Array('TABBUTTON_UserFields', 'TABBUTTON_GroupDocuments');
var groupArrDivs = new Array('panEmployees', 'panGroupDocs');
var groupDivsLoaded = new Array(false, false);

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

    Url = "Handlers/srvGroups.ashx?action=getBarButtonsLite&ID=" + IDGrupo;
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

    Url = "Handlers/srvGroups.ashx?action=getGroupsTabLite&aTab=" + actualTabGrupo + "&ID=" + IDGrupo;
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

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function cargaEmpleado2(IDEmpleado) {    
    // Reposiciona l'arbre principal

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
    
    parent.window.open(url + "?IDEmployee=" + IDEmpleado + "&TabEmployee=1");    
}

function EnablePanelsFunctionallity() {
}

function refreshEmployeeTree(bolReload) {
    var ctlPrefix = "ctl00_contentMainBody_roTrees1";
    if (bolReload) {
        eval(ctlPrefix + "_roTrees.LoadTreeViews(true,true,true,true);");
    } else {
        eval(ctlPrefix + "_roTrees.LoadTreeViews(false,false,false,false);");
    }
}