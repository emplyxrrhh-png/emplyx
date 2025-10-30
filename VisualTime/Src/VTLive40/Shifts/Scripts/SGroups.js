var actualShiftTab = 0; // TAB per mostrar
var actualShift; // Concept actual

var actualShiftGroupTab = 0; // TAB per mostrar
var actualShiftGroup; // Concept actual

var actualType = ""

var nodeLoaded = false;
var recalcBeforeSave = "0";

function cargaNodo(Nodo) {
    nodeLoaded = false;

    if (Nodo.id.toUpperCase() == "SOURCE") {
        actualShift = -1;
        newShift();
        return;
    }

    var id = Nodo.id.substring(1);
    if (Nodo.id.substring(0, 1) == "A") { //Grupo de Horarios
        cargaShiftGroup(id);
        hasChanges(false);
    } else { //Horario
        cargaShift(id);
        hasChanges(false);
    }
}

function ShowTreesConceptGroups() {
    var dvI = $("#dvTreeConcepts");
    var dvE = $("#dvTreeConceptGroups");
    dvE.show();
    dvI.hide();
    cargaConceptGroup(actualConceptGroup);
}

function ShowTreesConcepts() {
    var dvI = $("#dvTreeConcepts");
    var dvE = $("#dvTreeConceptGroups");
    dvI.show();
    dvE.hide();
    cargaConcept(actualConcept);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    ConvertControls('divContenido');
    if (actualType == "S") {
        if (s.cpActionRO == "GETSHIFT" || s.cpActionRO == "SAVESHIFT") {
            EnablePanelsFunctionallity();
            changeHolidayTypeVisibility();

            txtShiftGroupName_Client.SetValue("___");

            if (s.cpResultRO == "OK") {
                if (s.cpIsNewRO == true) {
                    refreshTree();
                }

                if (s.cpNameRO != null && s.cpNameRO != "") {
                    if (s.cpCombosIndexRO != "") {
                        setSelectedIndexes("ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria_visibilityCriteria", s.cpCombosIndexRO);
                    }

                    var arrC = null;
                    if (s.cpShiftRulesRO != "") eval("arrC = [" + s.cpShiftRulesRO + "]");

                    var arrA = new Array();
                    if (s.cpShiftAssignmentsRO != "") eval("arrA = [" + s.cpShiftAssignmentsRO + "]");
                    else eval('arrA = [ { "assignments": []} ]');

                    var arrTZ = new Array();
                    if (s.cpShiftTLZonesRO != "") eval("arrTZ = [" + s.cpShiftTLZonesRO + "]");

                    var arrTH = new Array();
                    if (s.cpShiftHourZonesRO != "") eval("arrTH = [" + s.cpShiftHourZonesRO + "]");

                    var arrDailyRules = new Array();
                    if (s.cpShiftDailyRules != "") arrDailyRules = JSON.parse(s.cpShiftDailyRules)

                    initializeGridsAndTimelines(arrC, arrA, arrTZ, arrTH, arrDailyRules);

                    document.getElementById("readOnlyNameShift").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    var arrA = null;
                    eval('arrA = [ { "assignments": []} ]');

                    initializeGridsAndTimelines(null, arrA, new Array(), new Array(), new Array());

                    document.getElementById("readOnlyNameShift").textContent = newObjectName;
                    hasChanges(true);
                    txtShiftName_Client.SetValue(newObjectName);
                }
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value = "0";

                changeHolidayTypeVisibility();
            } else {
                var arrC = null;
                if (s.cpShiftRulesRO != "") eval("arrC = [" + s.cpShiftRulesRO + "]");

                var arrA = new Array();
                if (s.cpShiftAssignmentsRO != "") eval("arrA = [" + s.cpShiftAssignmentsRO + "]");

                var arrTZ = new Array();
                if (s.cpShiftTLZonesRO != "") eval("arrTZ = [" + s.cpShiftTLZonesRO + "]");

                var arrTH = new Array();
                if (s.cpShiftHourZonesRO != "") eval("arrTH = [" + s.cpShiftHourZonesRO + "]");

                var arrDailyRules = new Array();
                if (s.cpShiftDailyRules != "") arrDailyRules = JSON.parse(s.cpShiftDailyRules)

                initializeGridsAndTimelines(arrC, arrA, arrTZ, arrTH, arrDailyRules);

                var result = new Array();
                result.push(s.cpMessageRO);
                checkShiftStatus(result);
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value = recalcBeforeSave;
            }
        }
    } else {
        if (s.cpActionRO == "GETSHIFTGROUP" || s.cpActionRO == "SAVESHIFTGROUP") {
            txtShiftName_Client.SetValue("___");
            txtShortName_Client.SetValue("___");

            if (s.cpResultRO == "OK") {
                hasChanges(false);
            } else {
                hasChanges(true);
                showErrorPopup("SaveName.Error.Text", "error", "", s.cpMessageRO, "SaveName.Error.Option1Text", "SaveName.Error.Option1Description", "")
            }
        }
    }
    showLoadingGrid(false);
    activeTabContainer('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01', 0);
    nodeLoaded = true;
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function hasChanges(bolChanges, bolRecalc) {
    if (nodeLoaded == true) {
        if (actualType == "S") {
            hasChangesShifts(bolChanges, bolRecalc);
        } else if (actualType == "G") {
            hasChangesShiftGroups(bolChanges);
        }
    }
}

function saveChanges() {
    try {
        if (actualType == "S") {
            recalcBeforeSave = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value;
            saveChangesShift();
        } else if (actualType == "G") {
            saveChangesShiftGroups();
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChanges() {
    try {
        if (actualType == "S") {
            undoChangesShift();
        } else if (actualType == "G") {
            undoChangesShiftGroups();
        }
    } catch (e) { showError("undoChanges", e); }
}

function changeTabs(numTab) {
    if (actualType == "S") {
        changeTabsShifts(numTab);
    } else if (actualType == "G") {
        changeTabsShiftGroups(numTab);
    }
}

//Cambia els Tabs i els divs (per nom)
function changeTabsByName(nameTab) {
    if (actualType == "S") {
        changeTabsByNameShifts(nameTab);
    } else if (actualType == "G") {
        changeTabsByNameShiftGroups(nameTab);
    }
}

//Mostra el ToolTip a la barra d'eines
function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

//Amaga el ToolTip a la barra d'eines
function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { showError('setMessage', e); }
}

/* funcio per bloquejar sols l'area menu */
function disableScreen(bol) {
    var divBg = document.getElementById('divModalBgDisabled');
    if (divBg != null) {
        if (bol == true) {
            /*divBg.style.height = document.body.offsetHeight;
            divBg.style.width = document.body.offsetWidth;*/
            document.body.style.overflow = "hidden";

            divBg.style.height = 2000;  //document.body.offsetHeight;
            divBg.style.width = 3000;  //document.body.offsetWidth;

            divBg.style.display = '';
        } else {
            document.body.style.overflow = "";
            divBg.style.display = 'none';
        }
    }
}

//Nou shiftgroup wizard
function ShowNewShiftGroupWizard() {
    var Title = '';
    top.ShowExternalForm2('Shifts/Wizards/NewShiftGroupWizard.aspx', 500, 450, Title, '', false, false, false);
}

//Refresh de les pantalles (RETORN)
function RefreshScreen(DataType) {
    refreshTree();
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
    var ctlPrefix = "ctl00_contentMainBody_roTreesShifts";
    eval(ctlPrefix + "_roTrees.RenameSelectedNode('" + txt + "');");
}

async function refreshTree() {
    await getroTreeState('ctl00_contentMainBody_roTreesShifts').then(roState => roState.reset());
    eval('ctl00_contentMainBody_roTreesShifts_roTrees.LoadTreeViews(true, true, true);');
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesShifts";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Shifts/srvMsgBoxShifts.aspx?action=Message";
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

function rowOver(rowID) {
    var table = document.getElementById(rowID);
    var cells = table.getElementsByTagName("td");
    for (var i = 0; i < cells.length; i++) {
        addCssClass(cells[i], "gridRowOver");
    }
}

function rowOut(rowID) {
    var table = document.getElementById(rowID);
    var cells = table.getElementsByTagName("td");
    for (var i = 0; i < cells.length; i++) {
        removeCssClass(cells[i], "gridRowOver");
    }
}