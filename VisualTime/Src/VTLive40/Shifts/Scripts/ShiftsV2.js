var curShiftType = "";
var oTMDefinitions = null;
var oTMZones = null;
var jsGridAssignments = null;
var listParameters = [];
var layerNumber = 0;

var dailyRules = [];

function checkShiftEmptyName(newName) {
    document.getElementById("readOnlyNameShift").textContent = newName;
    hasChanges(true, false);
}

//Carga Tabs y contenido Empleadso
function cargaShift(IDShift) {
    resetUserCriteriaControl('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria_visibilityCriteria');
    layerNumber = 0;
    listParameters = [];
    actualShift = IDShift;
    actualType = "S";
    //TAB Gris Superior
    showLoadingGrid(true);
    cargaShiftTabSuperior(IDShift);
}

// Carrega la part del TAB grisa superior
function cargaShiftTabSuperior(IDShift) {
    try {
        var parms = "";

        parms = { "action": "getShiftTab", "aTab": actualShiftTab, "ID": IDShift };
        AjaxCall("POST", "json", "Handlers/srvShifts.ashx", parms, "CONTAINER", "divShifts", "cargaShiftBarButtons(" + IDShift + ")");
    }
    catch (e) {
        showError("cargaConceptTabSuperior", e);
    }
}

var responseObj2 = null;
function cargaShiftBarButtons(IDShift) {
    try {
        var Url = "";

        Url = "Handlers/srvShifts.ashx?action=getBarButtons&ID=" + IDShift;
        AsyncCall("POST", Url, "JSON3", responseObj2, "parseResponseBarButtons(objContainerId," + IDShift + ")");
    }
    catch (e) {
        showError("cargaConceptTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IDEmpleado) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaShiftDivs(IDEmpleado);
}

// Carrega els apartats dels divs de l'usuari
function cargaShiftDivs(IDShift) {
    var oParameters = {};
    oParameters.aTab = actualShiftTab;
    oParameters.ID = IDShift;
    oParameters.oType = "S";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETSHIFT";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Cambia els Tabs i els divs
function changeTabsShifts(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03', 'TABBUTTON_04', 'TABBUTTON_05', 'TABBUTTON_06', 'TABBUTTON_07', 'TABBUTTON_08');
    arrDivs = new Array('div00', 'div01', 'div02', 'div03', 'div04', 'div05', 'div06', 'div07', 'div08');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (tab != null && div != null) {
            if (n == numTab) {
                if (n == 8) {
                    var hrScheduling = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_isHRScheduling").value;
                    tab.className = 'bTab-active';
                    var div8 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_div09");
                    if (hrScheduling == "1") {
                        div.style.display = '';
                        div8.style.display = 'none';
                    } else {
                        div.style.display = 'none';
                        div8.style.display = '';
                    }
                } else {
                    tab.className = 'bTab-active';
                    div.style.display = '';
                }
            } else {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }
    actualShiftTab = numTab;

    if (numTab == 1) {
        if (oTMDefinitions != null) {
            oTMDefinitions.resetScrollPos();
        }
    } else if (numTab == 2) {
        if (oTMZones != null) {
            oTMZones.resetScrollPos();
        }
    }
    ShiftTypeChanged();
}

//Cambia els Tabs i els divs (per nom)
function changeTabsByNameShifts(nameTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03', 'TABBUTTON_04', 'TABBUTTON_05', 'TABBUTTON_06', 'TABBUTTON_07', 'TABBUTTON_08');
    arrDivs = new Array('div00', 'div01', 'div02', 'div03', 'div04', 'div05', 'div06', 'div07', 'div08');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (tab != null && div != null) {
            if (div.id == nameTab) {
                if (n == 8) {
                    tab.className = 'bTab-active';
                    actualShiftTab = n;

                    var hrScheduling = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_isHRScheduling").value;
                    var div8 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_div09");
                    if (hrScheduling == "1") {
                        div.style.display = '';
                        div8.style.display = 'none';
                    } else {
                        div.style.display = 'none';
                        div8.style.display = '';
                    }
                } else {
                    tab.className = 'bTab-active';
                    div.style.display = '';
                }
            } else {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }

    if (numTab == 1) {
        if (oTMDefinitions != null) {
            oTMDefinitions.resetScrollPos();
        }
    } else if (numTab == 2) {
        if (oTMZones != null) {
            oTMZones.resetScrollPos();
        }
    }
    ShiftTypeChanged();
}

function hasChangesShifts(bolChanges, markRecalc) {
    var needToRecalc = false;
    if (typeof (markRecalc) != "undefined") {
        needToRecalc = markRecalc;
    }
    else needToRecalc = true;

    var txtNameShift = document.getElementById('txtNameShift');

    if (bolChanges && needToRecalc == true) {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value = "1";
    }
    //else {
    //Se comenta la línea para que no resetee el valor cuando ya haya pasado por el needToRecalc almenos una vez
    //document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value = "0";
    //}

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

function undoChangesShift() {
    try {
        if (actualShift == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesShifts";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaShiftDivs(actualShift);
        }
    } catch (e) { showError("undoChangesConcepts", e); }
}

function EnablePanelsFunctionallity() {
    $(".availableShiftType").show();
    $(".unavailableShiftType").hide();


    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optVacations');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNormal');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNormalFloating');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optPerHours');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optUnique');
    
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optVacations,ctl00_contentMainBody_ASPxCallbackPanelContenido_optNormal,ctl00_contentMainBody_ASPxCallbackPanelContenido_optNormalFloating,ctl00_contentMainBody_ASPxCallbackPanelContenido_optPerHours,ctl00_contentMainBody_ASPxCallbackPanelContenido_optUnique');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityAll');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityNobody');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optVisibilityCollectives');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityNobody,ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria,ctl00_contentMainBody_ASPxCallbackPanelContenido_optVisibilityCollectives');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAutoDetect');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optManualDetect');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAutoDetect,ctl00_contentMainBody_ASPxCallbackPanelContenido_optManualDetect');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optChkShowShift');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optShiftChange');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optTypePerms');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optShiftChange,ctl00_contentMainBody_ASPxCallbackPanelContenido_optTypePerms');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate,ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCostOnlyWork');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCostAll');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCostOnlyWork,ctl00_contentMainBody_ASPxCallbackPanelContenido_opCostAll');
}

function checkCriteriaVisibility() {
    if (document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria_rButton").checked == false) {
        document.getElementById("visibilityCriteriaTable").style.display = "none";
    } else {
        document.getElementById("visibilityCriteriaTable").style.display = "";
    }
}

function checkCollectivesVisibility() {
    if (document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_optVisibilityCollectives_rButton").checked == false) {
        document.getElementById("collectivesCell").style.display = "none";
    } else {
        document.getElementById("collectivesCell").style.display = "";
    }
}

function ShiftTypeChanged() {
    var oldShiftType = curShiftType;

    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optVacations_rButton').checked) curShiftType = "Vacations";
    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNormal_rButton').checked) curShiftType = "Normal";
    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNormalFloating_rButton').checked) curShiftType = "NormalFloating";
    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optPerHours_rButton').checked) curShiftType = "PerHours";
    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optUnique_rButton').checked) curShiftType = "Unique";

    if (oldShiftType != '' && oldShiftType != curShiftType) {
        if (oTMDefinitions == null) oTMDefinitions = new roTimeLine('ctl00_contentMainBody_ASPxCallbackPanelContenido_tmDefinitions', true);
        else oTMDefinitions.clearLayers();
    }

    if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == 'true') {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_panTbRules').style.display = '';
    }

    var arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03', 'TABBUTTON_04', 'TABBUTTON_05', 'TABBUTTON_06', 'TABBUTTON_07', 'TABBUTTON_08');
    var arrButtonsVisible = new Array(true, true, true, true, true, true, true, true, true);
    switch (curShiftType) {
        case 'Normal':
            arrButtonsVisible = new Array(true, true, true, true, true, true, true, true, true);
            hideAdvancedChecks(true);
            break;
        case 'Vacations':
            arrButtonsVisible = new Array(true, true, false, false, true, true, true, true, true);
            //document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_panTbRules').style.display = 'none';
            hideAdvancedChecks(true);
            break;
        case 'NormalFloating':
            arrButtonsVisible = new Array(true, true, true, true, true, true, true, true, true);
            hideAdvancedChecks(true);
            break;
        case 'Unique':
            arrButtonsVisible = new Array(true, false, false, false, true, false, false, false, true);
            hideAdvancedChecks(true);
            break;
        case 'PerHours':
            arrButtonsVisible = new Array(true, true, true, true, true, true, true, true, true);
            hideAdvancedChecks(false);
            break;
    }

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        if (tab != null) {
            if (arrButtonsVisible[n] == true) {
                tab.style.display = '';
                tab.parentElement.style.height = '';
            }
            else {
                tab.style.display = 'none';
                if (n < 3) tab.parentElement.style.height = '27px';
                else tab.parentElement.style.height = '28px';
            }
        }
    }
    if (arrButtonsVisible[actualShiftTab] == false) {
        changeTabs(0);
    }
}

function hideAdvancedChecks(state) {
    if (state) {
        document.getElementById('trChkmodifyini').style.display = 'none';
        document.getElementById('trchkmodifyduration').style.display = 'none';
    }
    else {
        document.getElementById('trChkmodifyini').style.display = '';
        document.getElementById('trchkmodifyduration').style.display = '';
    }
}

function ShowAdvancedCheckIni(s) {
    if (curShiftType === 'PerHours') {
        if (s.GetSelectedIndex() == 0) {
            document.getElementById('trChkmodifyini').style.display = '';
            document.getElementById('trchkmodifyduration').style.display = '';
        }
        else {
            document.getElementById('trChkmodifyini').style.display = 'none';
            document.getElementById('trchkmodifyduration').style.display = 'none';
        }
    }
}

function ShowAdvancedCheckEnd(s) {
    if (curShiftType === 'PerHours') {
        if (s.GetSelectedIndex() == 0)
            document.getElementById('trchkmodifyduration').style.display = 'none';
        else
            document.getElementById('trchkmodifyduration').style.display = '';
    }
}

function initializeGridsAndTimelines(arrCC, arrA, arrTZ, arrTH, arrDailyRules) {
    try {
        createTimeZones(arrTZ, arrTH, 'ctl00_contentMainBody_ASPxCallbackPanelContenido_tmDefinitions', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_tmZones');

        if (arrCC != null && arrCC[0].error == "true") {
            checkShiftStatus(arrCC);
            return;
        }

        if (arrCC != null && arrCC.length != 0) {
            for (var n = 0; n < arrCC.length; n++) {
                createGridLine(arrCC[n].simplerules);
            }
        }

        if (arrA != null && arrA.length != 0) {
            createGridAssignments(arrA);
        }

        dailyRules = [];
        if (arrDailyRules != null && arrDailyRules.length != 0) {
            dailyRules = arrDailyRules;

            for (var n = 0; n < arrDailyRules.length; n++) {
                createGridDailyRulesLine(arrDailyRules[n]);
            }
        }
    } catch (e) { showError('createGridRules', e); }
}

function createGridAssignments(arrA) {
    var hdGridAssignment = [{ 'fieldname': 'AssignmentName', 'description': '', 'size': '100%' }];
    //{ 'fieldname': 'Coverage', 'description': '', 'size': '20%' }];

    var arrGridAssignments = arrA[0].assignments;

    hdGridAssignment[0].description = document.getElementById('hdnLngAssignmentName').value;
    //hdGridAssignment[1].description = document.getElementById('hdnLngCoverage').value;

    var edtRow = false;
    var delRow = false;

    if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "true") {
        edtRow = true;
        delRow = true;
    }

    jsGridAssignments = new jsGrid('ctl00_contentMainBody_ASPxCallbackPanelContenido_grdAssignments', hdGridAssignment, arrGridAssignments, edtRow, delRow, false, 'Assignments');
}

function createTimeZones(arrTZ, arrTH, CtlID, CtlHID) {
    try {
        if (arrTZ != null && arrTZ.length > 0) {
            if (arrTZ[0].error == "true") {
                checkShiftStatus(arrTZ);
                return;
            }
        }

        if (arrTH != null && arrTH.length > 0) {
            if (arrTH[0].error == "true") {
                checkShiftStatus(arrTH);
                return;
            }
        }

        if (oTMDefinitions == null) oTMDefinitions = new roTimeLine(CtlID, true);
        else oTMDefinitions.clearLayers();

        if (arrTZ != null) oTMDefinitions.loadLayers(arrTZ);

        if (oTMZones == null) oTMZones = new roTimeLine(CtlHID, false);
        else oTMZones.clearLayers();
        if (arrTH != null) oTMZones.loadLayers(arrTH);
    } catch (e) { showError("createTimeZones", e); }
}

function createGridDailyRulesLine(dailyRule) {
    try {
        //Carreguem el array global per mantenir els valors
        var n;

        var oTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridDailyRules');

        oTable.setAttribute("border", "0");
        oTable.setAttribute("cellpadding", "0");
        oTable.setAttribute("cellspacing", "0");

        var oNewId = oTable.rows.length;
        var altRow = 1;

        /*Afegim el tr */
        var oTR = oTable.insertRow(-1);
        oTR.id = "htRowDR_" + oNewId;
        oTR.setAttribute("name", "htRowsDailyRules");
        oTR.setAttribute("idtr", "htRowDR_" + oNewId);

        if ((oNewId % 2) != 0) {
            altRow = "1";
        } else {
            altRow = "2";
        }

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
        } else { // IE
        }

        oTR.setAttribute("idshift", dailyRule.IDShift);
        oTR.setAttribute("idrule", dailyRule.ID);
        oTR.setAttribute("priority", dailyRule.Priority);

        var oTD = oTR.insertCell(-1); //Name
        oTD.id = "DescDailyRuleGrid_" + oNewId;
        oTD.className = "GridStyle-cell" + altRow;
        oTD.textContent = dailyRule.Name;
        oTD.width = "95%";

        oTD.setAttribute("idTr", "htRowDR_" + oNewId);
        oTR.setAttribute("idTr", "htRowDR_" + oNewId);

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
            oTD.setAttribute("onmouseover", "javascript: rowOver('htRowDR_" + oNewId + "');");
            oTD.setAttribute("onmouseout", "javascript: rowOut('htRowDR_" + oNewId + "');");
            oTD.setAttribute("onclick", "javascript: editDailyRule('htRowDR_" + oNewId + "');");
        } else { // IE
            oTR.onmouseover = function () { rowOver(this.getAttribute("idTr")); }
            oTR.onmouseout = function () { rowOut(this.getAttribute("idTr")); }

            oTD.onclick = function () { editDailyRule(this.getAttribute("idTr")); }
        }

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'true') {
            //Creem la barra d'eines al TR
            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            //oTD.style.borderRight = "solid 1px silver";
            oTD.width = "50px";
            oTD.style.whiteSpace = "nowrap";
            //oTD.style.borderRight = "solid 1px silver";

            var aEdit = document.createElement("A");
            var aDelete = document.createElement("A");

            aEdit.href = "javascript: void(0);";
            aDelete.href = "javascript: void(0);";
            aEdit.title = document.getElementById("tagEditTitle").value;
            aDelete.title = document.getElementById("tagRemoveTitle").value;

            aEdit.setAttribute("row", "htRowDR_" + oNewId);
            aDelete.setAttribute("row", "htRowDR_" + oNewId);

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                aEdit.setAttribute("onclick", "javascript: editDailyRule('" + oTR.id + "');");
                aDelete.setAttribute("onclick", "javascript: deleteDailyRule('" + oTR.id + "');");
            } else { // IE
                aEdit.onclick = function () { editRule(this.getAttribute("row")); }
                aDelete.onclick = function () { deleteRule(this.getAttribute("row")); }
            }

            var imgEdit = document.createElement("IMG");
            var imgDelete = document.createElement("IMG");
            imgEdit.src = hBaseRef + "Images/Grid/edit.png";
            imgDelete.src = hBaseRef + "Images/Grid/remove.png";

            aEdit.appendChild(imgEdit);
            aDelete.appendChild(imgDelete);
            oTD.appendChild(aEdit);
            oTD.appendChild(aDelete);
            oTD.style.backgroundColor = "#E8EEF7";

            // ---------------------------------------------------- Up / Down
            //Creem la barra d'eines al TR
            var oTD2 = oTR.insertCell(-1); //Name
            oTD2.className = "GridStyle-cellheader";
            //oTD.style.borderRight = "solid 1px silver";
            oTD2.width = "50px";
            oTD2.style.whiteSpace = "nowrap";
            oTD2.style.borderRight = "solid 1px silver";

            var aMUP = document.createElement("A");
            var aMDown = document.createElement("A");

            aMUP.href = "javascript: void(0);";
            aMDown.href = "javascript: void(0);";

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                aMUP.setAttribute("onclick", "javascript: moveRow('tblGridDailyRules',this.parentNode, -1);");
                aMDown.setAttribute("onclick", "javascript: moveRow('tblGridDailyRules',this.parentNode, 1);");
            } else { // IE
                aMUP.onclick = function () { moveRow('tblGridDailyRules', this.parentNode, -1); }
                aMDown.onclick = function () { moveRow('tblGridDailyRules', this.parentNode, 1); }
            }

            var imgUP = document.createElement("IMG");
            var imgDown = document.createElement("IMG");
            imgUP.src = hBaseRef + "Images/Grid/uparrow.png";
            imgDown.src = hBaseRef + "Images/Grid/downarrow.png";

            aMUP.appendChild(imgUP);
            aMDown.appendChild(imgDown);
            oTD2.appendChild(aMUP);
            oTD2.appendChild(aMDown);
            oTD2.style.backgroundColor = "#E8EEF7";
        }

        return true;
    } catch (e) {
        showError("createGridDailyRulesLine", e);
        return false;
    } //end try
}

function editGridDailyRulesLine(dailyRule) {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridDailyRules');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsDailyRules");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsDailyRules");
                break;
            default:
                hRows = document.getElementsByName("htRowsDailyRules");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsDailyRules");
                }
                break;
        }

        var oDailyRule = null;

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute('priority') == dailyRule.Priority) {
                hRow.setAttribute("idshift", dailyRule.IDSHift);
                hRow.setAttribute("idrule", dailyRule.ID);
                hRow.setAttribute("priority", dailyRule.Priority);

                var oDest = document.getElementById('DescDailyRuleGrid_' + hRow.id.split('_')[1]);
                if (oDest != null) {
                    oDest.textContent = dailyRule.Name;
                }
                break;
            }
        }
    } catch (e) { showError("EditGridLine", e); }
}

function createGridLine(arrFields) {
    try {
        //Carreguem el array global per mantenir els valors
        var n;

        var oTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridRules');

        oTable.setAttribute("border", "0");
        oTable.setAttribute("cellpadding", "0");
        oTable.setAttribute("cellspacing", "0");

        var oNewId = 0;

        //Comprobem el maxid
        if (oTable.rows.length > 1) {
            for (nR = 1; nR < oTable.rows.length; nR++) {
                var oTRx = oTable.rows[nR];
                oNewId = parseInt(oTRx.id.split("_")[1]);
            }
        }

        oNewId += 1;

        var altRow = 1;

        /*Afegim el tr */
        var oTR = oTable.insertRow(-1);
        oTR.id = "htRowC_" + oNewId;
        oTR.setAttribute("name", "htRowsRules");
        oTR.setAttribute("idtr", "htRowC_" + oNewId);

        if (((oTable.rows.length - 1) % 2) != 0) {
            altRow = "1";
        } else {
            altRow = "2";
        }

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
        } else { // IE
        }

        for (n = 0; n < arrFields.length; n++) {
            var fieldName = arrFields[n].field.toUpperCase();
            var controls = arrFields[n].control;
            var value = arrFields[n].value;
            var typeControl = arrFields[n].type.toUpperCase();
            var list = arrFields[n].list;

            switch (fieldName.toUpperCase()) {
                case "IDSHIFT":
                    oTR.setAttribute("idshift", value);
                    break;
                case "IDRULE":
                    oTR.setAttribute("idrule", value);
                    break;
                case "RULEDESCRIPTION":
                    var oTD = oTR.insertCell(-1); //Name
                    oTD.id = "DescRuleGrid_" + oNewId;
                    oTD.className = "GridStyle-cell" + altRow;
                    if (value == " ") {
                        retrieveRuleDescription(arrFields, "DescRuleGrid_" + oNewId); //Recuperem desde el server el nom
                    }
                    oTD.innerHTML = value;
                    oTD.width = "95%";
                    oTD.setAttribute("idTr", "htRowC_" + oNewId);
                    oTR.setAttribute("idTr", "htRowC_" + oNewId);
                    if (window.addEventListener) { // Mozilla, Netscape, Firefox
                        oTD.setAttribute("onmouseover", "javascript: rowOver('htRowC_" + oNewId + "');");
                        oTD.setAttribute("onmouseout", "javascript: rowOut('htRowC_" + oNewId + "');");
                        oTD.setAttribute("onclick", "javascript: editRule('htRowC_" + oNewId + "');");
                    } else { // IE
                        oTR.onmouseover = function () { rowOver(this.getAttribute("idTr")); }
                        oTR.onmouseout = function () { rowOut(this.getAttribute("idTr")); }

                        oTD.onclick = function () { editRule(this.getAttribute("idTr")); }
                    }
                    break;
                case "INCIDENCE":
                    oTR.setAttribute("incidence", value);
                    break;
                case "ZONE":
                    oTR.setAttribute("zone", value);
                    break;
                case "TOTIME":
                    oTR.setAttribute("totime", value);
                    break;
                case "FROMTIME":
                    oTR.setAttribute("fromtime", value);
                    break;
                case "MAXTIME":
                    oTR.setAttribute("maxtime", value);
                    break;
                case "CAUSE":
                    oTR.setAttribute("cause", value);
                    break;
                case "CONDITIONVALUETYPE":
                    oTR.setAttribute("conditionvaluetype", value);
                    break;
                case "FROMVALUEUSERFIELD":
                    oTR.setAttribute("fromvalueuserfield", value);
                    break;
                case "TOVALUEUSERFIELD":
                    oTR.setAttribute("tovalueuserfield", value);
                    break;
                case "BETWEENVALUEUSERFIELD":
                    oTR.setAttribute("betweenvalueuserfield", value);
                    break;
                case "ACTIONVALUETYPE":
                    oTR.setAttribute("actionvaluetype", value);
                    break;
                case "MAXVALUEUSERFIELD":
                    oTR.setAttribute("maxvalueuserfield", value);
                    break;
            } //end switch
        } //end for

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != 'true') {
            //Creem la barra d'eines al TR
            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            //oTD.style.borderRight = "solid 1px silver";
            oTD.width = "50px";
            oTD.style.whiteSpace = "nowrap";
            //oTD.style.borderRight = "solid 1px silver";

            var aEdit = document.createElement("A");
            var aDelete = document.createElement("A");

            aEdit.href = "javascript: void(0);";
            aDelete.href = "javascript: void(0);";
            aEdit.title = document.getElementById("tagEditTitle").value;
            aDelete.title = document.getElementById("tagRemoveTitle").value;

            aEdit.setAttribute("row", "htRowC_" + oNewId);
            aDelete.setAttribute("row", "htRowC_" + oNewId);

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                aEdit.setAttribute("onclick", "javascript: editRule('" + oTR.id + "');");
                aDelete.setAttribute("onclick", "javascript: deleteRule('" + oTR.id + "');");
            } else { // IE
                aEdit.onclick = function () { editRule(this.getAttribute("row")); }
                aDelete.onclick = function () { deleteRule(this.getAttribute("row")); }
            }

            var imgEdit = document.createElement("IMG");
            var imgDelete = document.createElement("IMG");
            imgEdit.src = hBaseRef + "Images/Grid/edit.png";
            imgDelete.src = hBaseRef + "Images/Grid/remove.png";

            aEdit.appendChild(imgEdit);
            aDelete.appendChild(imgDelete);
            oTD.appendChild(aEdit);
            oTD.appendChild(aDelete);
            oTD.style.backgroundColor = "#E8EEF7";

            // ---------------------------------------------------- Up / Down
            //Creem la barra d'eines al TR
            var oTD2 = oTR.insertCell(-1); //Name
            oTD2.className = "GridStyle-cellheader";
            //oTD.style.borderRight = "solid 1px silver";
            oTD2.width = "50px";
            oTD2.style.whiteSpace = "nowrap";
            oTD2.style.borderRight = "solid 1px silver";

            var aMUP = document.createElement("A");
            var aMDown = document.createElement("A");

            aMUP.href = "javascript: void(0);";
            aMDown.href = "javascript: void(0);";
            //aMUP.title = document.getElementById("tagEditTitle").value;
            //aMDown.title = document.getElementById("tagRemoveTitle").value;

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                aMUP.setAttribute("onclick", "javascript: moveRow('tblGridRules',this.parentNode, -1);");
                aMDown.setAttribute("onclick", "javascript: moveRow('tblGridRules',this.parentNode, 1);");
            } else { // IE
                aMUP.onclick = function () { moveRow('tblGridRules', this.parentNode, -1); }
                aMDown.onclick = function () { moveRow('tblGridRules', this.parentNode, 1); }
            }

            var imgUP = document.createElement("IMG");
            var imgDown = document.createElement("IMG");
            imgUP.src = hBaseRef + "Images/Grid/uparrow.png";
            imgDown.src = hBaseRef + "Images/Grid/downarrow.png";

            aMUP.appendChild(imgUP);
            aMDown.appendChild(imgDown);
            oTD2.appendChild(aMUP);
            oTD2.appendChild(aMDown);
            oTD2.style.backgroundColor = "#E8EEF7";
        }

        return true;
    } catch (e) {
        showError("createGridLine", e);
        return false;
    } //end try
}

function AddDailyRule() {
    try {
        frmDailyRule_Show(null);
    } catch (e) { showError("AddDailyRule", e); }
}

function editDailyRule(idRow) {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        if (oDis == "false") {
            var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridDailyRules');

            //Si no hi ha taula
            if (hTable == null) { return; }

            var hRows;

            switch (BrowserDetect.browser) {
                case 'Firefox':
                case 'Safari':
                    hRows = document.getElementsByName("htRowsDailyRules");
                    break;
                case 'Explorer':
                    hRows = getElementsByName_iefix("TR", "htRowsDailyRules");
                    break;
                default:
                    hRows = document.getElementsByName("htRowsDailyRules");
                    if (hRows.length == 0) {
                        hRows = getElementsByName_iefix("TR", "htRowsDailyRules");
                    }
                    break;
            }

            var oDailyRule = null;

            //Bucle per les files del grid, per eliminar les files
            for (var n = 0; n < hRows.length; n++) {
                var hRow = hRows[n];
                if (hRow.id == idRow) {
                    oDailyRule = dailyRules[parseInt(hRow.getAttribute("priority"), 10) - 1];
                    break;
                }
            }

            if (oDailyRule != null) frmDailyRule_Show(oDailyRule);
        }
    } catch (e) { showError("editDailyRule", e); }
}

function deleteDailyRule(objId) {
    try {
        var IDShift = actualShift;

        var oRow = document.getElementById(objId);
        if (oRow == null) { return; }

        var IdRule = oRow.getAttribute("idRule");

        var url = "Shifts/srvMsgBoxShifts.aspx?action=Message&IDShift=" + IDShift + "&IDDailyRule=" + IdRule;
        url = url + "&TitleKey=deleteGrid.Denied.Title";
        url = url + "&DescriptionKey=deleteGrid.Denied.Description";
        url = url + "&Option1TextKey=deleteGrid.Denied.Option1Text";
        url = url + "&Option1DescriptionKey=deleteGrid.Denied.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteDailyRuleRow('" + objId + "'); return false;";
        url = url + "&Option2TextKey=deleteGrid.Denied.Option2Text";
        url = url + "&Option2DescriptionKey=deleteGrid.Denied.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteDailyRule", e); }
}

function deleteDailyRuleRow(objId) {
    setDailyRuleChanges(true);
    RemoveListDailyRule(objId);
    ReeStyleListDailyRule();
    hasChangesShifts(true);
}

function AddNewRules() {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
        loadGenericData("IDSHIFT", "hdnIDShift", "-1", "X_NUMBER", "", oDis, false);
        loadGenericData("IDRULE", "hdnIDRule", "-1", "X_NUMBER", "", oDis, false);

        showIncidenceData();
        cmbRuleZone1Client.SetValue("");
        cmbRuleConditionValueTypeClient.SetValue("");
        txtCriteria11Client.SetValue(new Date('1900/01/01 00:00:00'));
        txtCriteria12Client.SetValue(new Date('1900/01/01 23:59:00'));
        cmbRuleFromValueUserFieldsClient.SetValue("");
        cmbRuleToValueUserFieldsClient.SetValue("");
        cmbRuleBetweenValueUserFieldsClient.SetValue("");
        txtCriteria2ToClient.SetValue(new Date('1900/01/01 23:59:00'));
        cmbRuleCauses2Client.SetValue("");
        cmbRuleActionValueTypeClient.SetValue("");
        cmbRuleMaxValueUserFieldsClient.SetValue("");

        cmbRuleCriteria1Client.SetValue("");
        cmbRuleCriteria2Client.SetValue("");

        loadGenericData("IDGRID", "hdnNewRule", "-1", "X_TEXT", "", oDis, false);

        //Comprobacions de mostrar uns valors o uns altres per els combos
        showCriteria1(0);
        showConditionValueType(0);
        showCriteria2(false);
        showActionValueType(0);

        disableScreen(true);
        showWndRules(true);
    } catch (e) { showError("AddNewRules", e); }
}

function editRule(objId) {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        if (oDis == "false") {
            var oRow = document.getElementById(objId);
            if (oRow == null) { return; }

            showIncidenceData();

            loadGenericData("IDSHIFT", "hdnIDShift", oRow.getAttribute("idshift"), "X_NUMBER", "", oDis, false);
            loadGenericData("IDRULE", "hdnIDRule", oRow.getAttribute("idrule"), "X_NUMBER", "", oDis, false);

            cmbRuleConcept1Client.SetValue(oRow.getAttribute("incidence"));
            cmbRuleConcept2Client.SetValue(oRow.getAttribute("incidence"));
            cmbRuleZone1Client.SetValue(oRow.getAttribute("zone"));
            cmbRuleConditionValueTypeClient.SetValue(oRow.getAttribute("conditionvaluetype"));
            txtCriteria11Client.SetValue(new Date('1900/01/01 ' + oRow.getAttribute("fromtime") + ':00'));
            txtCriteria12Client.SetValue(new Date('1900/01/01 ' + oRow.getAttribute("totime") + ':00'));
            cmbRuleFromValueUserFieldsClient.SetValue(oRow.getAttribute("fromvalueuserfield"));
            cmbRuleToValueUserFieldsClient.SetValue(oRow.getAttribute("tovalueuserfield"));
            cmbRuleBetweenValueUserFieldsClient.SetValue(oRow.getAttribute("betweenvalueuserfield"));
            txtCriteria2ToClient.SetValue(new Date('1900/01/01 ' + oRow.getAttribute("maxtime") + ':00'));
            cmbRuleCauses2Client.SetValue(oRow.getAttribute("cause"));
            cmbRuleActionValueTypeClient.SetValue(oRow.getAttribute("actionvaluetype"));
            cmbRuleMaxValueUserFieldsClient.SetValue(oRow.getAttribute("maxvalueuserfield"));

            loadGenericData("IDGRID", "hdnNewRule", oRow.getAttribute("idtr"), "X_TEXT", "", oDis, false);

            var cmbRul1 = "";
            var cmbRul2 = "";
            var bolRul2 = false;

            //Criteria 1
            if (oRow.getAttribute("conditionvaluetype") == "0") { // valor directo
                if (oRow.getAttribute("fromtime") == "00:00" && oRow.getAttribute("totime") == "23:59") {
                    cmbRul1 = 0;
                } else if (oRow.getAttribute("totime") == "23:59") { //mayor que
                    cmbRul1 = 1;
                } else if (oRow.getAttribute("fromtime") == "00:00") { //menor que
                    cmbRul1 = 2;
                } else {
                    cmbRul1 = 3;
                }
            }
            else { // valor según campo de la ficha
                if (oRow.getAttribute("fromvalueuserfield") != "") {
                    cmbRul1 = 1;
                } else if (oRow.getAttribute("tovalueuserfield") != "") {
                    cmbRul1 = 2;
                } else if (oRow.getAttribute("betweenvalueuserfield") != "") {
                    cmbRul1 = 3;
                }
            }

            //Criteria 2
            if (oRow.getAttribute("actionvaluetype") == "0") { // valor directo
                if (oRow.getAttribute("maxtime") == "23:59") { //cualquier hora
                    cmbRul2 = 0;
                    bolRul2 = false;
                } else {
                    cmbRul2 = 1;
                    bolRul2 = true;
                }
            }
            else {  // valor según campo de la ficha
                cmbRul2 = 1;
                bolRul2 = true;
            }

            cmbRuleCriteria1Client.SetValue(cmbRul1);
            cmbRuleCriteria2Client.SetValue(cmbRul2);

            if (oDis == "true") {
                btnCancelClient.SetVisible(false);
                cmbRuleConcept1Client.SetEnabled(false);
                cmbRuleZone1Client.SetEnabled(false);
                cmbRuleConditionValueTypeClient.SetEnabled(false);
                txtCriteria11Client.SetEnabled(false);
                txtCriteria12Client.SetEnabled(false);
                cmbRuleFromValueUserFieldsClient.SetEnabled(false);
                cmbRuleToValueUserFieldsClient.SetEnabled(false);
                cmbRuleBetweenValueUserFieldsClient.SetEnabled(false);
                txtCriteria2ToClient.SetEnabled(false);
                cmbRuleCauses2Client.SetEnabled(false);
                cmbRuleActionValueTypeClient.SetEnabled(false);
                cmbRuleMaxValueUserFieldsClient.SetEnabled(false);
                cmbRuleCriteria1Client.SetEnabled(false);
                cmbRuleCriteria2Client.SetEnabled(false);
            }

            //Comprobacions de mostrar uns valors o uns altres per els combos
            showCriteria1(cmbRul1);
            showConditionValueType(oRow.getAttribute("conditionvaluetype"));
            showCriteria2(bolRul2);
            showActionValueType(oRow.getAttribute("actionvaluetype"));

            disableScreen(true);
            showWndRules(true);
        }
    } catch (e) { showError("editRule", e); }
}

function showIncidenceData() {
    if (curShiftType === "PerHours") {
        cmbRuleConcept2Client.SetValue("");
        cmbRuleConcept1Client.SetVisible(false);
        cmbRuleConcept2Client.SetVisible(true);
    }
    else {
        cmbRuleConcept1Client.SetValue("");
        cmbRuleConcept2Client.SetVisible(false);
        cmbRuleConcept1Client.SetVisible(true);
    }
}

function showCriteria1(typeCriteria) {
    try {
        var divCr11 = document.getElementById('divSelCriteria11');
        var divCr10 = document.getElementById('divSelCriteria10');
        var divCr12 = document.getElementById('divSelCriteria12');
        var divFromValueUserField = document.getElementById('divFromValueUserField');
        var divToValueUserField = document.getElementById('divToValueUserField');
        var divBetweenValueUserField = document.getElementById('divBetweenValueUserField');

        var oFromTime = txtCriteria11Client;
        var oToTime = txtCriteria12Client;
        var oFromValueUserField = cmbRuleFromValueUserFieldsClient;
        var oToValueUserField = cmbRuleToValueUserFieldsClient;
        var oBetweenValueUserField = cmbRuleBetweenValueUserFieldsClient;

        var divConditionValueType = document.getElementById('divConditionValueType');

        switch (parseInt(typeCriteria, 10)) {
            case 0: //none
                divCr11.style.display = 'none';
                divCr10.style.display = 'none';
                divCr12.style.display = 'none';
                divFromValueUserField.style.display = 'none';
                divToValueUserField.style.display = 'none';
                divBetweenValueUserField.style.display = 'none';
                oFromTime.SetValue(new Date('1900/01/01 00:00:00'));
                oToTime.SetValue(new Date('1900/01/01 23:59:00'));
                oFromValueUserField.SetValue('');
                oToValueUserField.SetValue('');
                oBetweenValueUserField.SetValue('');
                divConditionValueType.style.display = 'none';
                break;
            case 1: //first
                divCr11.style.display = '';
                divCr10.style.display = 'none';
                divCr12.style.display = 'none';
                divFromValueUserField.style.display = '';
                divToValueUserField.style.display = 'none';
                divBetweenValueUserField.style.display = 'none';
                oToTime.SetValue(new Date('1900/01/01 23:59:00'));
                oToValueUserField.SetValue('');
                oBetweenValueUserField.SetValue('');
                divConditionValueType.style.display = '';
                break;
            case 2: //second
                divCr11.style.display = 'none';
                divCr10.style.display = 'none';
                divCr12.style.display = '';
                divFromValueUserField.style.display = 'none';
                divToValueUserField.style.display = '';
                divBetweenValueUserField.style.display = 'none';
                oFromTime.SetValue(new Date('1900/01/01 00:00:00'));
                oFromValueUserField.SetValue('');
                oBetweenValueUserField.SetValue('');
                divConditionValueType.style.display = '';
                break;
            case 3: // all
                divCr11.style.display = '';
                divCr10.style.display = '';
                divCr12.style.display = '';
                divFromValueUserField.style.display = 'none';
                divToValueUserField.style.display = 'none';
                divBetweenValueUserField.style.display = '';
                oFromValueUserField.SetValue('');
                oToValueUserField.SetValue('');
                divConditionValueType.style.display = '';
                break;
        }
    } catch (e) { showError("showCriteria1", e); }
}

function showConditionValueType(typeValue) {
    try {
        var divConditionValueDirect = document.getElementById('divConditionValueDirect');
        var divConditionValueUserField = document.getElementById('divConditionValueUserField');

        switch (parseInt(typeValue, 10)) {
            case 0: // valor director
                divConditionValueDirect.style.display = '';
                divConditionValueUserField.style.display = 'none';
                break;
            case 1: // según campo de la ficha
                divConditionValueDirect.style.display = 'none';
                divConditionValueUserField.style.display = '';
                break;
        }
    } catch (e) { showError("showConditionValueType", e); }
}

function showCriteria2(bol) {
    try {
        var divCr = document.getElementById('divSelCriteria2');
        if (bol) {
            divCr.style.display = '';
        } else {
            divCr.style.display = 'none';
            txtCriteria2ToClient.SetValue(new Date('1900/01/01 23:59:00'));
            cmbRuleActionValueTypeClient.SetValue("");
        }
    } catch (e) { showError("showCriteria2", e); }
}

function showActionValueType(typeValue) {
    try {
        var divActionValueDirect = document.getElementById('divMaxValueDirect');
        var divActionValueUserField = document.getElementById('divMaxValueUserField');
        switch (parseInt(typeValue, 10)) {
            case 0: // valor director
                divActionValueDirect.style.display = '';
                divActionValueUserField.style.display = 'none';
                break;
            case 1: // según campo de la ficha
                divActionValueDirect.style.display = 'none';
                divActionValueUserField.style.display = '';
                break;
        }
    } catch (e) { showError("showActionValueType", e); }
}

function validateEditRule() {
    try {
        //Comprobem que estiguin omplerts els camps correctament
        var valRuleConcept1 = (curShiftType === "PerHours") ? cmbRuleConcept2Client.GetValue() : cmbRuleConcept1Client.GetValue();
        var valRuleZone1 = cmbRuleZone1Client.GetValue();
        var valRuleCriteria1 = cmbRuleCriteria1Client.GetValue();
        var txtCriteria11 = txtCriteria11Client.GetValue();
        var txtCriteria12 = txtCriteria12Client.GetValue();
        var valRuleCauses2 = cmbRuleCauses2Client.GetValue();
        var valRuleCriteria2 = cmbRuleCriteria2Client.GetValue();
        var txtCriteria2 = txtCriteria2ToClient.GetValue();

        if (valRuleConcept1 == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoConceptSelected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (valRuleZone1 == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoZonetSelected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (valRuleCriteria1 == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoCriteria1Selected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (document.getElementById('divSelCriteria11').style.display == "") {
            if (txtCriteria11 == "") {
                showErrorPopup("Error.Title", "error", "Error.Description.NoValidCriteria11", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        if (document.getElementById('divSelCriteria12').style.display == "") {
            if (txtCriteria12 == "") {
                showErrorPopup("Error.Title", "error", "Error.Description.NoValidCriteria12", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        if (valRuleCauses2 == null || valRuleCauses2 == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoCause2Selected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (valRuleCriteria2 == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoCriteria2Selected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        if (document.getElementById('divMaxValueDirect').style.display == "") {
            if (txtCriteria2 == "") {
                showErrorPopup("Error.Title", "error", "Error.Description.NoValidCriteria2", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        return true;
    } catch (e) { showError("validateEditRule", e); return false; }
}

function saveRule() {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        if (oDis == "true") {
            cancelRule();
            return;
        }

        if (validateEditRule()) {
            var oIDShift = document.getElementById('hdnIDShift').value;
            var oIDRule = document.getElementById('hdnIDRule').value;
            var oIncidence = (curShiftType === "PerHours") ? cmbRuleConcept2Client.GetValue() : cmbRuleConcept1Client.GetValue();
            var oZone = cmbRuleZone1Client.GetValue();
            var oFromTime = txtCriteria11Client.GetValue().format2Time();
            var oToTime = txtCriteria12Client.GetValue().format2Time();
            var oCause = cmbRuleCauses2Client.GetValue();
            var oMaxTime = txtCriteria2ToClient.GetValue().format2Time();
            var oConditionValueType = cmbRuleConditionValueTypeClient.GetValue();
            if (oConditionValueType == "" || oConditionValueType == null) oConditionValueType = "0";
            var oFromValueUserField = cmbRuleFromValueUserFieldsClient.GetValue();
            if (oFromValueUserField == null) oFromValueUserField = "";
            var oToValueUserField = cmbRuleToValueUserFieldsClient.GetValue();
            if (oToValueUserField == null) oToValueUserField = "";
            var oBetweenValueUserField = cmbRuleBetweenValueUserFieldsClient.GetValue();
            if (oBetweenValueUserField == null) oBetweenValueUserField = "";
            var oActionValueType = cmbRuleActionValueTypeClient.GetValue();
            if (oActionValueType == "" || oActionValueType == null) oActionValueType = "0";
            var oMaxValueUserField = cmbRuleMaxValueUserFieldsClient.GetValue();
            if (oMaxValueUserField == null) oMaxValueUserField = "";

            var oRule = {
                fields: [
                    { 'field': 'IDShift', 'value': oIDShift, 'control': '', 'type': '' },
                    { 'field': 'IDRule', 'value': oIDRule, 'control': '', 'type': '' },
                    { 'field': 'RuleDescription', 'value': ' ', 'control': '', 'type': '' },
                    { 'field': 'Incidence', 'value': oIncidence, 'control': '', 'type': '' },
                    { 'field': 'Zone', 'value': oZone, 'control': '', 'type': '' },
                    { 'field': 'FromTime', 'value': oFromTime, 'control': '', 'type': '' },
                    { 'field': 'ToTime', 'value': oToTime, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Cause', 'value': oCause, 'control': '', 'type': '' },
                    { 'field': 'MaxTime', 'value': oMaxTime, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'ConditionValueType', 'value': oConditionValueType, 'control': '', 'type': '' },
                    { 'field': 'FromValueUserField', 'value': oFromValueUserField, 'control': '', 'type': '' },
                    { 'field': 'ToValueUserField', 'value': oToValueUserField, 'control': '', 'type': '' },
                    { 'field': 'BetweenValueUserField', 'value': oBetweenValueUserField, 'control': '', 'type': '' },
                    { 'field': 'ActionValueType', 'value': oActionValueType, 'control': '', 'type': '' },
                    { 'field': 'MaxValueUserField', 'value': oMaxValueUserField, 'control': '', 'type': '' }
                ]
            };

            if (document.getElementById('hdnNewRule').value == "-1") { //nou registre
                createGridLine(oRule.fields);
            } else {
                editGridLine(oRule.fields, document.getElementById('hdnNewRule').value);
            }
            hasChanges(true, true);

            //Tanquem finestra
            disableScreen(false);
            showWndRules(false);

            setRuleChanges(true);
        }
    } catch (e) { showError("saveRule", e); }
}

function editGridLine(arrFields, rowID) {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridRules');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var oTR = document.getElementById(rowID);
        if (oTR == null) { return; }
        for (n = 0; n < arrFields.length; n++) {
            var fieldName = arrFields[n].field.toUpperCase();
            var controls = arrFields[n].control;
            var value = arrFields[n].value;
            var typeControl = arrFields[n].type.toUpperCase();
            var list = arrFields[n].list;

            switch (fieldName.toUpperCase()) {
                case "IDSHIFT":
                    oTR.setAttribute("idShift", value);
                    break;
                case "RULEDESCRIPTION":
                    var parmID = rowID.split("_");
                    var oNewID = parmID[parmID.length - 1];
                    break;
                case "IDRULE":
                    oTR.setAttribute("idrule", value);
                    break;
                case "INCIDENCE":
                    oTR.setAttribute("incidence", value);
                    break;
                case "ZONE":
                    oTR.setAttribute("zone", value);
                    break;
                case "TOTIME":
                    oTR.setAttribute("totime", value);
                    break;
                case "FROMTIME":
                    oTR.setAttribute("fromtime", value);
                    break;
                case "MAXTIME":
                    oTR.setAttribute("maxtime", value);
                    break;
                case "CAUSE":
                    oTR.setAttribute("cause", value);
                    break;
                case "CONDITIONVALUETYPE":
                    oTR.setAttribute("conditionvaluetype", value);
                    break;
                case "FROMVALUEUSERFIELD":
                    oTR.setAttribute("fromvalueuserfield", value);
                    break;
                case "TOVALUEUSERFIELD":
                    oTR.setAttribute("tovalueuserfield", value);
                    break;
                case "BETWEENVALUEUSERFIELD":
                    oTR.setAttribute("betweenvalueuserfield", value);
                    break;
                case "ACTIONVALUETYPE":
                    oTR.setAttribute("actionvaluetype", value);
                    break;
                case "MAXVALUEUSERFIELD":
                    oTR.setAttribute("maxvalueuserfield", value);
                    break;
            } //end switch
        } //end for

        retrieveRuleDescription(arrFields, "DescRuleGrid_" + oNewID); //Recuperem desde el server el nom
    } catch (e) { showError("EditGridLine", e); }
}

function cancelRule() {
    disableScreen(false);
    showWndRules(false)
}

function showWndRules(bol) {
    try {
        var divC = document.getElementById('divRule');
        if (divC != null) {
            if (bol == true) {
                divC.style.display = '';
                divC.style.marginLeft = ((divC.offsetWidth / 2) * -1) + "px";
                divC.style.marginTop = ((divC.offsetHeight / 2) * -1) + "px";   //- 160;
            } else {
                divC.style.display = 'none';
            }
        }
    } catch (e) { showError("showWndRules", e); }
}

function deleteRule(objId) {
    try {
        var IDShift = actualShift;

        var oRow = document.getElementById(objId);
        if (oRow == null) { return; }

        var IdRule = oRow.getAttribute("idRule");

        var url = "Shifts/srvMsgBoxShifts.aspx?action=Message&IDShift=" + IDShift + "&IDRule=" + IdRule;
        url = url + "&TitleKey=deleteGrid.Denied.Title";
        url = url + "&DescriptionKey=deleteGrid.Denied.Description";
        url = url + "&Option1TextKey=deleteGrid.Denied.Option1Text";
        url = url + "&Option1DescriptionKey=deleteGrid.Denied.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteRuleRow('" + objId + "'); return false;";
        url = url + "&Option2TextKey=deleteGrid.Denied.Option2Text";
        url = url + "&Option2DescriptionKey=deleteGrid.Denied.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteRule", e); }
}

function deleteRuleRow(objId) {
    setRuleChanges(true);
    RemoveListRule(objId);
    ReeStyleListRule();
    hasChangesShifts(true);
}

function setDailyRuleChanges(bol) {
    try {
        if (bol) {
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnDailyRuleChanges').value = "1";
        } else {
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnDailyRuleChanges').value = "0";
        }
    } catch (e) { showError("setRuleChanges", e); }
}

function setRuleChanges(bol) {
    try {
        if (bol) {
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRuleChanges').value = "1";
        } else {
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRuleChanges').value = "0";
        }
    } catch (e) { showError("setRuleChanges", e); }
}

function RemoveListRule(objId) {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridRules');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsRules");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsRules");
                break;
            default:
                hRows = document.getElementsByName("htRowsRules");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsRules");
                }
                break;
        }

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.id == objId) {
                //si troba el row, l'eliminem (n+1, per la capcelera)
                hTable.deleteRow(n + 1);
                return;
            }
        }

        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("RemoveListValue", e); }
}

function RemoveListDailyRule(objId) {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridDailyRules');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsDailyRules");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsDailyRules");
                break;
            default:
                hRows = document.getElementsByName("htRowsDailyRules");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsDailyRules");
                }
                break;
        }

        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];

            if (hRow.id == objId) {
                //si troba el row, l'eliminem (n+1, per la capcelera)
                var index = parseInt(hRow.getAttribute("priority"), 10) - 1;

                dailyRules.splice(index, 1);
                break;
            }
        }

        //Bucle per les files del grid, per eliminar les files
        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                while (hRows.length > 0) {
                    hTable.deleteRow(1);
                }
                break;
            case 'Explorer':
                for (var i = 0; i < hRows.length; i++) {
                    hTable.deleteRow(1);
                }
                break;
            default:
                try {
                    while (hRows.length > 0) {
                        hTable.deleteRow(1);
                    }
                } catch (e) { }
                break;
        }

        for (var i = 0; i < dailyRules.length; i++) {
            dailyRules[i].Priority = i + 1;
            createGridDailyRulesLine(dailyRules[i]);
        }
    } catch (e) { showError("RemoveListDailyValue", e); }
}

function ReeStyleListRule() {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridRules');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsRules");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsRules");
                break;
            default:
                hRows = document.getElementsByName("htRowsRules");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsRules");
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            if ((n % 2) != 0) {
                altRow = "2";
            } else {
                altRow = "1";
            }

            var hRow = hRows[n];
            hRow.cells[0].className = 'GridStyle-cell' + altRow;
            hRow.cells[1].className = 'GridStyle-cell' + altRow;
        }

        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("RemoveListValue", e); }
}

function ReeStyleListDailyRule() {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridDailyRules');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsDailyRules");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsDailyRules");
                break;
            default:
                hRows = document.getElementsByName("htRowsDailyRules");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsDailyRules");
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            if ((n % 2) != 0) {
                altRow = "2";
            } else {
                altRow = "1";
            }

            var hRow = hRows[n];
            hRow.cells[0].className = 'GridStyle-cell' + altRow;
            hRow.cells[1].className = 'GridStyle-cell' + altRow;
        }

        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("ReeStyleListDailyRule", e); }
}

function moveRow(tableID, cell, direction) {
    mTable = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + tableID);
    rowIndex = parseInt(cell.parentNode.rowIndex);

    if ((rowIndex == 1 && direction > 0) || (rowIndex == mTable.rows.length - 1 && direction < 0) || (rowIndex > 0 && rowIndex < mTable.rows.length - 1)) {
        if (mTable.rows.length > 2) {
            if ((rowIndex + direction) > 0) {
                var row = mTable.rows[rowIndex];

                var cell0 = row.cells[0].cloneNode(true);
                var cell1 = row.cells[1].cloneNode(true);
                var cell2 = row.cells[2].cloneNode(true);

                cell1.childNodes[0].onclick = mTable.rows[rowIndex].cells[1].childNodes[0].onclick
                cell1.childNodes[1].onclick = mTable.rows[rowIndex].cells[1].childNodes[1].onclick
                cell2.childNodes[0].onclick = mTable.rows[rowIndex].cells[2].childNodes[0].onclick
                cell2.childNodes[1].onclick = mTable.rows[rowIndex].cells[2].childNodes[1].onclick

                mTable.deleteRow(rowIndex);
                var oRow = mTable.insertRow(rowIndex + direction);

                jQuery.each(row.attributes, function () {
                    oRow.setAttribute(this.nodeName, this.nodeValue);
                });

                oRow.id = row.id;

                oRow.appendChild(cell0);
                oRow.appendChild(cell1);
                oRow.appendChild(cell2);

                ReeStyleListRule();

                if (tableID == 'tblGridDailyRules') {
                    var actualPosition = rowIndex - 1;

                    dailyRules[actualPosition].Priority = dailyRules[actualPosition].Priority + direction;

                    dailyRules[actualPosition + direction].Priority = dailyRules[actualPosition + direction].Priority + (direction * -1);
                    dailyRules = dailyRules.sort(function (a, b) { return (a.Priority > b.Priority) ? 1 : ((b.Priority > a.Priority) ? -1 : 0); });

                    mTable.rows[rowIndex + direction].setAttribute('priority', parseInt(mTable.rows[rowIndex + direction].getAttribute('priority'), 10) + direction);
                    mTable.rows[rowIndex].setAttribute('priority', parseInt(mTable.rows[rowIndex].getAttribute('priority'), 10) + (direction * -1));

                    ReeStyleListDailyRule();
                }
                hasChanges(true, true);
            }
        }
    }
}

function retrieveRuleDescription(arrFields, destID) {
    try {
        var oFields = '';

        for (n = 0; n < arrFields.length; n++) {
            var fieldName = arrFields[n].field.toUpperCase();
            var value = arrFields[n].value;

            //Validem els camps
            var strParam = fieldName + "=" + encodeURIComponent(value);
            oFields += strParam + "&";
        } //end for

        oFields = oFields.substring(0, oFields.length - 1);
        showLoadingGrid(true);
        AsyncCall("POST", "Handlers/srvShifts.ashx?action=retRuleDesc&" + oFields, "json", "arrStatus", "showLoadingGrid(false);checkShiftStatus(arrStatus); if(arrStatus[0].error == 'false'){ putDescriptionRule(arrStatus,'" + destID + "'); }");

        return true;
    } catch (e) {
        showError('retrieveRuleDescription', e);
        return false;
    }
}

function putDescriptionRule(arrStatus, destID) {
    try {
        var oDest = document.getElementById(destID);
        if (oDest != null) {
            oDest.innerHTML = arrStatus[0].msg;
        }
    } catch (e) { showError("putDescriptionRule", e); }
}

function checkShiftStatus(oStatus) {
    try {
        //Carreguem el array global per mantenir els valors
        var arrStatus = oStatus;
        var objError = arrStatus[0];

        //Si es un error, mostrem el missatge

        if (typeof (objError.error) != 'undefined') {
            if (objError.error == "true") {
                if (objError.typemsg == "1") { //Missatge estil pop-up
                    var url = "Shifts/srvMsgBoxShifts.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                        "DescriptionText=" + encodeURIComponent(objError.msg) + "&" +
                        "Option1TextKey=SaveName.Error.Option1Text&" +
                        "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                        "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                    parent.ShowMsgBoxForm(url, 400, 300, '');
                } else { //Missatge estil inline
                }
                hasChanges(true);
            } else {
                if (objError.Prefix == 'QUESTION') {
                    var url = "Shifts/srvMsgBoxShifts.aspx?action=Message&TitleText=" + encodeURIComponent(objError.Title) + "&" +
                        "DescriptionText=" + encodeURIComponent(objError.Description) + "&";

                    if (objError.ButtonVisible1 == true) {
                        url += "Option1TextText=" + encodeURIComponent(objError.ButtonText1) + "&" +
                            "Option1DescriptionText=" + encodeURIComponent(objError.ButtonDescription1) + "&" +
                            "Option1OnClickScript=" + objError.ButtonFunct1 + " return false;&";
                    }
                    if (objError.ButtonVisible2 == true) {
                        url += "Option2TextText=" + encodeURIComponent(objError.ButtonText2) + "&" +
                            "Option2DescriptionText=" + encodeURIComponent(objError.ButtonDescription2) + "&" +
                            "Option2OnClickScript=" + objError.ButtonFunct2 + " return false;&";
                    }
                    url += "IconUrl=~/Base/Images/MessageFrame/dialog-question.png";

                    parent.ShowMsgBoxForm(url, 400, 300, '');

                    hasChanges(true);
                }
            }
            showLoadingGrid(false);
        } else {
            if (objError.Error == true) {
                if (objError.TypeMsg == "PopupMsg") { //Missatge estil pop-up
                    var url = "Shifts/srvMsgBoxShifts.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                        "DescriptionText=" + encodeURIComponent(objError.ErrorText) + "&" +
                        "Option1TextKey=SaveName.Error.Option1Text&" +
                        "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                        "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                    parent.ShowMsgBoxForm(url, 400, 300, '');
                } else { //Missatge estil inline
                }
                hasChanges(true);
            } else {
                if (objError.Prefix == 'QUESTION') {
                    var url = "Shifts/srvMsgBoxShifts.aspx?action=Message&TitleText=" + encodeURIComponent(objError.Title) + "&" +
                        "DescriptionText=" + encodeURIComponent(objError.Description) + "&";

                    if (objError.ButtonVisible1 == true) {
                        url += "Option1TextText=" + encodeURIComponent(objError.ButtonText1) + "&" +
                            "Option1DescriptionText=" + encodeURIComponent(objError.ButtonDescription1) + "&" +
                            "Option1OnClickScript=" + objError.ButtonFunct1 + " return false;&";
                    }
                    if (objError.ButtonVisible2 == true) {
                        url += "Option2TextText=" + encodeURIComponent(objError.ButtonText2) + "&" +
                            "Option2DescriptionText=" + encodeURIComponent(objError.ButtonDescription2) + "&" +
                            "Option2OnClickScript=" + objError.ButtonFunct2 + " return false;&";
                    }
                    url += "IconUrl=~/Base/Images/MessageFrame/dialog-question.png";

                    parent.ShowMsgBoxForm(url, 400, 300, '');

                    hasChanges(true);
                }
            }
        }
    } catch (e) { alert('checkStatus: ' + e); }
}

function AddNewAssignments() {
    try {
        document.getElementById('hdnAddAssignmentIDRow').value = "";
        frmAddAssignment_ShowNew();
    } catch (e) { showError("AddNewAssignments", e); }
}

function updateAddAssignmentRow(rowID, arrValues) {
    try {
        if (jsGridAssignments == null) { createGridAssignments(arrValues); }
        if (rowID == "") {
            jsGridAssignments.createRow(arrValues, true);
        } else {
            jsGridAssignments.editRow(rowID, arrValues);
        }
    } catch (e) { showError("updateAddAssignmentRow", e); }
}

function deleteGridAssignments(idRow) {
    try {
        var oRow = document.getElementById(idRow);
        var BeginRO = oRow.getAttribute("jsgridatt_beginro");
        var EndRO = oRow.getAttribute("jsgridatt_endro");

        if (BeginRO == "true" || EndRO == "true") {
            showErrorPopup("Error.frmAssignment.NoDelAssignmentTitle", "ERROR", "Error.frmAssignment.NoDelAssignmentDesc", "Error.frmAssignment.OK", "Error.frmAssignment.OKDesc", "");
        } else {
            var url = "Shifts/srvMsgBoxShifts.aspx?action=Message";
            url = url + "&TitleKey=deleteAssignmentDef.Title";
            url = url + "&DescriptionKey=deleteAssignmentDef.Description";
            url = url + "&Option1TextKey=deleteAssignmentDef.Option1Text";
            url = url + "&Option1DescriptionKey=deleteAssignmentDef.Option1Description";
            url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelAssignment('" + idRow + "'); return false;";
            url = url + "&Option2TextKey=deletePeriodDef.Option2Text";
            url = url + "&Option2DescriptionKey=deletePeriodDef.Option2Description";
            url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
            parent.ShowMsgBoxForm(url, 400, 300, '');
        }
    } catch (e) { showError("deleteGridAssignments", e); }
}

function delSelAssignment(idRow) {
    try {
        jsGridAssignments.deleteRow(idRow);
        hasChanges(true, false);
    } catch (e) { showError("delSelAssignment", e); }
}

function editGridAssignments(idRow) {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true") { return; }
        var arrValues = new Array();
        arrValues = jsGridAssignments.retRowJSON(idRow);

        document.getElementById('hdnAddAssignmentIDRow').value = idRow;

        frmAddAssignment_Show(arrValues);
    } catch (e) { showError("editGridAssignments", e); }
}

function getIsFloating() {
    try {
        var bolIsFloating = false;
        bolIsFloating = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNormalFloating_rButton').checked;
        return bolIsFloating;
    } catch (e) {
        showError('getIsFloating', e);
        return false;
    }
}

function showTLLayer(objPrefix, ID, Type, arrNames, arrValues) {
    try {
        if (Type.toUpperCase() == "WORKING") {
            frmEditShiftFlexible_Show(arrNames, arrValues);
        } else if (Type.toUpperCase() == "MANDATORY") {
            frmEditShiftMandatory_Show(arrNames, arrValues);
        } else if (Type.toUpperCase() == "BREAK") {
            frmEditShiftBreak_Show(arrNames, arrValues);
        } else {
            frmAddZone_Show(arrNames, arrValues);
        }
    } catch (e) { showError("showTLLayer", e); }
}

function ShowDelZones() {
    try {
        if (oTMZones != null) {
            if (oTMZones.selectedLayer() != null) {
                //mostra missatge de confirmació abans...
                ConfirmDelZone();
            }
        }
    } catch (e) { showError("ShowDelZones", e); }
}

function ConfirmDelZone() {
    try {
        var url = "Shifts/srvMsgBoxShifts.aspx?action=Message";
        url = url + "&TitleKey=deleteShiftZone.Title";
        url = url + "&DescriptionKey=deleteShiftZone.Description";
        url = url + "&Option1TextKey=deleteShiftZone.Option1Text";
        url = url + "&Option1DescriptionKey=deleteShiftZone.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelZone(); return false;";
        url = url + "&Option2TextKey=deleteShiftZone.Option2Text";
        url = url + "&Option2DescriptionKey=deleteShiftZone.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showErrorPopup("ConfirmDelZone", e); }
}

function delSelZone() {
    try {
        if (oTMZones.deleteSelectedLayer()) {
            hasChanges(true);
        }
    } catch (e) { showError("delSelZone", e); }
}

function ShowDelDefinition() {
    try {
        if (oTMDefinitions != null) {
            if (oTMDefinitions.selectedLayer() != null) {
                //mostra missatge de confirmació abans...
                ConfirmDelDefinition();
            }
        }
    } catch (e) { showError("ShowDelDefinition", e); }
}

function ConfirmDelDefinition() {
    try {
        var url = "Shifts/srvMsgBoxShifts.aspx?action=Message";
        url = url + "&TitleKey=deleteShiftDef.Title";
        url = url + "&DescriptionKey=deleteShiftDef.Description";
        url = url + "&Option1TextKey=deleteShiftDef.Option1Text";
        url = url + "&Option1DescriptionKey=deleteShiftDef.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelDefinition(); return false;";
        url = url + "&Option2TextKey=deleteShiftDef.Option2Text";
        url = url + "&Option2DescriptionKey=deleteShiftDef.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showErrorPopup("ConfirmDelDefinition", e); }
}

function delSelDefinition() {
    try {
        if (oTMDefinitions.deleteSelectedLayer()) {
            hasChanges(true);
        }
    } catch (e) { showError("delSelDefinition", e); }
}

function recalcRules() {
    try {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate_rButton').checked == true) { //option bydate
            var dateFormatValue = document.getElementById('ctl00_contentMainBody_dateFormatValue').value;
            var date1 = dtRecDateClient.GetValue();
            var date2 = recalcConfigClient.Get("FirstDate");

            //Date en blanc, error
            if (dtRecDateClient.GetValue() == null) {
                showErrorPopup("Error.RecalcDateBlank.Title", "ERROR", "Error.RecalcDateBlank.Description", "", "Error.RecalcDateBlank.OK", "Error.RecalcDateBlank.OKDesc", "");
                return;
            }

            //Date mes gran que la de inici de recalcul
            if (date1 < date2) {
                showErrorPopup("Error.RecalcDateNotCorrect.Title", "ERROR", "Error.RecalcDateNotCorrect.Description", "", "Error.RecalcDateNotCorrect.OK", "Error.RecalcDateNotCorrect.OKDesc", "");
                return;
            }

            recalcConfigClient.Set("NeedRecalc", true);
            saveDefChanges();
        } else { //option all
            if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays_rButton').checked == true) { //option bydate
                dtRecDateClient.SetValue(null);
                recalcConfigClient.Set("NeedRecalc", true);
                saveDefChanges();
            } else {
                showErrorPopup("Error.TitleNoRecalcSelected", "error", "Error.Description.NoRecalcSelected", "", "Error.OK", "Error.OKDesc", "");
            }
        }
    } catch (e) { showError("recalcRules", e); }
}

function saveChangesShift() {
    try {
        if (ASPxClientEdit.ValidateGroup('shiftMainInfo', true)) {
            if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value == "1") {
                if (parseInt(actualShift, 10) >= 0) {
                    var chkObsolete = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_chkObsolete');
                    var chkObsoleteOld = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_chkObsoleteLastValue');
                    if (chkObsolete != null) {
                        if (chkObsolete.checked && chkObsolete.checked == chkObsoleteOld.checked) {
                            showErrorPopup("Error.TitleShiftObsolete", "error", "Error.Description.ShiftObsolete", "", "Error.OK", "Error.OKDesc", "");
                            return;
                        } else {
                            isUsed();
                        }
                    } else {
                        isUsed();
                    }
                } else {
                    saveDefChanges();
                }
            } else {
                saveDefChanges();
            }
        } else {
            showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        };
    } catch (e) { alert("saveChangesShift: " + e); }
}

function isUsed() {
    try {
        showLoadingGrid(true);
        AsyncCall("POST", "Handlers/srvShifts.ashx?action=shiftIsUsed&ID=" + actualShift, "json", "arrStatus", "showLoadingGrid(false); checkShiftStatus(arrStatus); if(arrStatus[0].error == 'false'){ retIsUsed(arrStatus); }")
    } catch (e) { showError("isUsed", e); }
}

function retIsUsed(arrStatus) {
    try {
        if (arrStatus[0].msg.toUpperCase() == "SHIFTUSEDNO") {
            saveDefChanges();
        } else {
            var oParms = arrStatus[0].msg.split(":");
            dtRecDateClient.SetValue(new Date(oParms[1] + " 00:00:00"));
            var optAll = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays_panOptionPanel');
            if (oParms[2] == "0") {
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate_rButton').checked == true;
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays_rButton').checked == false;
                //disable option all
                optAll.setAttribute('venabled', 'false');
            } else {
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate_rButton').checked == false;
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays_rButton').checked == true;
                //enable option all
                optAll.setAttribute('venabled', 'true');
            }

            venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays');

            disableScreen(true);
            showWndRecalcShift(true);
        }
    } catch (e) { showError("retIsUsed", e); }
}

function showWndRecalcShift(bol) {
    var divC = document.getElementById('divMsgChg');
    if (divC != null) {
        if (bol == true) {
            divC.style.display = '';
        } else {
            divC.style.display = 'none';
        }
    }
}

function closeWndRecalShift() {
    disableScreen(false);
    showWndRecalcShift(false);
}

function retrieveJSONRulesGrid() {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridRules');
        var newArr = new Array();

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsRules");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsRules");
                break;
            default:
                hRows = document.getElementsByName("htRowsRules");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsRules");
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];

            var oRule = {
                fields: [
                    { 'field': 'IDShift', 'value': hRow.getAttribute("idshift"), 'control': '', 'type': '' },
                    { 'field': 'IDRule', 'value': hRow.getAttribute("idrule"), 'control': '', 'type': '' },
                    { 'field': 'Incidence', 'value': hRow.getAttribute("incidence"), 'control': '', 'type': '' },
                    { 'field': 'Zone', 'value': hRow.getAttribute("zone"), 'control': '', 'type': '' },
                    { 'field': 'ToTime', 'value': hRow.getAttribute("totime"), 'control': '', 'type': '' },
                    { 'field': 'FromTime', 'value': hRow.getAttribute("fromtime"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'MaxTime', 'value': hRow.getAttribute("maxtime"), 'control': '', 'type': '' },
                    { 'field': 'Cause', 'value': hRow.getAttribute("cause"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'ConditionValueType', 'value': hRow.getAttribute("conditionvaluetype"), 'control': '', 'type': '' },
                    { 'field': 'FromValueUserField', 'value': hRow.getAttribute("fromvalueuserfield"), 'control': '', 'type': '' },
                    { 'field': 'ToValueUserField', 'value': hRow.getAttribute("tovalueuserfield"), 'control': '', 'type': '' },
                    { 'field': 'BetweenValueUserField', 'value': hRow.getAttribute("betweenvalueuserfield"), 'control': '', 'type': '' },
                    { 'field': 'ActionValueType', 'value': hRow.getAttribute("actionvaluetype"), 'control': '', 'type': '' },
                    { 'field': 'MaxValueUserField', 'value': hRow.getAttribute("maxvalueuserfield"), 'control': '', 'type': '' }
                ]
            };

            newArr.push(oRule);
        }

        return newArr;
    } catch (e) { showError("retrieveJSONRulesGrid", e); }
}

function saveDefChanges(CheckVacationsEmpty) {
    try {
        //Aqui afegim la nova carrega del grid desde javascript
        var oJSONLayers = oTMDefinitions.retrieveJSONLayers();
        var oJSONHZones = oTMZones.retrieveJSONLayers();
        var oJSONRules = retrieveJSONRulesGrid();

        var arrAssignments;

        //Carreguem el array
        if (jsGridAssignments != null) { arrAssignments = jsGridAssignments.toJSONStructure(); }

        if (CheckVacationsEmpty == null) {
            CheckVacationsEmpty = true;
        }
        if (saveFinallyShift(oJSONRules, oJSONLayers, oJSONHZones, CheckVacationsEmpty, arrAssignments) == true) {
            hasChanges(false, false);
            closeWndRecalShift();
        }
    } catch (e) { showError("saveDefChanges", e); }
}

function saveFinallyShift(arrRules, arrLayers, arrZones, CheckVacationsEmpty, arrAssignments) {
    try {
        showLoadingGrid(true);

        var oParameters = {};
        oParameters.aTab = actualShiftTab;
        oParameters.ID = actualShift;
        oParameters.oType = "S";
        oParameters.Name = document.getElementById("readOnlyNameShift").textContent.trim();
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SAVESHIFT";
        oParameters.DailyRules = dailyRules;

        if (curShiftType === "PerHours") {
            //oParameters.AllowComplementary = chkAllowComplementaryClient.GetChecked();
            oParameters.AllowComplementary = true;
        } else {
            oParameters.AllowComplementary = false;
        }

        var strRules = "";
        if (arrRules != null) {
            for (var x = 0; x < arrRules.length; x++) {
                for (var xF = 0; xF < arrRules[x].fields.length; xF++) {
                    strRules += arrRules[x].fields[xF].field.replace("rolt", "") + "=" + arrRules[x].fields[xF].value;
                    if (xF != arrRules[x].fields.length) strRules += "*|*";
                }
                if (x != arrRules.length) strRules += "*&*";
            }
        }
        oParameters.resultRules = strRules;

        var strLayers = "";
        var minBreak = "0:00";
        var allowFloatingData = false;
        var minBreakAction = "";
        var bolMinBreakAction = false;
        if (arrLayers != null) {
            for (var x = 0; x < arrLayers.length; x++) {
                for (var xF = 0; xF < arrLayers[x].layer.length; xF++) {
                    if (arrLayers[x].layer[xF].value != null && arrLayers[x].layer[xF].value != "") {
                        var layerField = arrLayers[x].layer[xF].field.replace("rolt_", "");
                        var layerValue = arrLayers[x].layer[xF].value;
                        strLayers += layerField + "=" + layerValue;
                        if (layerField === 'allowmodini' || layerField === 'allowmodduration') {
                            if (!allowFloatingData || allowFloatingData === 'false') {
                                if (layerValue === 'True' || layerValue === 'true')
                                    layerValue = true;
                                if (layerValue === 'False' || layerValue === 'false')
                                    layerValue = false;
                                allowFloatingData = layerValue;
                            }
                        }
                        if (curShiftType == "PerHours") {
                            if (layerField === 'minbreakaction' && layerValue === 'SubstractAttendanceTime') {
                                bolMinBreakAction = true;
                            }
                            if (layerField === 'minbreaktime' && bolMinBreakAction)
                                minBreak = minBreak + "||" + layerValue;
                        }
                    }
                    if (xF != arrLayers[x].layer.length) strLayers += "*|*";
                }
                if (x != arrLayers.length) strLayers += "*&*";
                bolMinBreakAction = false;
            }
        }
        oParameters.resultLayers = strLayers;
        if (curShiftType == "PerHours") {
            oParameters.AllowFloatingData = true;
        } else {
            oParameters.AllowFloatingData = allowFloatingData;
        }
        oParameters.BreakHours = minBreak;

        var strZones = "";
        if (arrZones != null) {
            for (var x = 0; x < arrZones.length; x++) {
                for (var xF = 0; xF < arrZones[x].layer.length; xF++) {
                    if (arrZones[x].layer[xF].value != null && arrZones[x].layer[xF].value != "") {
                        strZones += arrZones[x].layer[xF].field.replace("rolt_", "") + "=" + arrZones[x].layer[xF].value;
                    }
                    if (xF != arrZones[x].layer.length) strZones += "*|*";
                }
                if (x != arrZones.length) strZones += "*&*";
            }
        }
        oParameters.resultTimeZones = strZones;

        var strAssignments = "";
        if (arrAssignments != null) {
            for (var x = 0; x < arrAssignments.length; x++) {
                var arrAssignmentFields = arrAssignments[x];
                for (var xF = 0; xF < arrAssignmentFields.length; xF++) {
                    strAssignments += arrAssignmentFields[xF].field + "=" + arrAssignmentFields[xF].value;
                    if (xF != arrAssignmentFields.length) strAssignments += "*|*";
                }
                if (x != arrAssignments.length) strAssignments += "*&*";
            }
        }
        oParameters.resultAssignments = strAssignments;

        if (CheckVacationsEmpty != null && CheckVacationsEmpty == true) {
            oParameters.checkVacationsEmpty = true;
        } else {
            oParameters.checkVacationsEmpty = false;
        }

        if (dtRecDateClient.GetValue() != null) {
            var sDate = dtRecDateClient.GetValue();
            oParameters.RecalcDate = sDate.getFullYear() + "/" + (sDate.getMonth() + 1) + "/" + sDate.getDate() + " 00:00:00";
        } else {
            oParameters.RecalcDate = "2079/1/1 00:00:00";
        }

        oParameters.DRPatternPunches = Array.from(
            document.querySelectorAll("[id*='drPair'] input")
        )
            .filter(input => input.id.endsWith("_I"))
            .map(input => input.value.trim())
            .join(",");

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);
        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);

        return true;
    } catch (e) {
        showLoadingGrid(false);
        showError('saveShift', e);
        return false;
    }
}

function addNewPair() {
    const currentIndx = Array.from(document.querySelectorAll("[id*='drPair']")).filter(
        pair => window.getComputedStyle(pair).display !== "none"
    ).length;

    const nextIndx = currentIndx + 1;
    if (nextIndx <= 5) {
        document.querySelector("[id*='drPair" + nextIndx + "']").style.display = "" //mostramos nueva fila
        document.querySelector("[id*='hdnCurrentPair']").value = nextIndx //actualizamos el hdn
        document.querySelector("[id*='btnRemovePair']").style.display = "" //mostramos botón eliminado
    }
    if (nextIndx >= 5) document.querySelector("[id*='btnAddPair']").setAttribute('style', 'display:none !important'); //ocultamos botón
    resetDeletePairIndx();
}

function removePair() {
    const visiblePairs = Array.from(document.querySelectorAll("[id*='drPair']")).filter(
        pair => window.getComputedStyle(pair).display !== "none"
    );

    const currentIndx = visiblePairs.length; 

    if (currentIndx > 1) {
        const pairToHide = visiblePairs[currentIndx - 1];

        pairToHide.style.display = "none";
        pairToHide.querySelectorAll("input").forEach(input => input.value = "");
        document.querySelector("[id*='hdnCurrentPair']").value = currentIndx - 1 //actualizamos el hdn
        document.querySelector("[id*='btnAddPair']").style.display = ""; //mostramos botón añadir
        updatePattern();
        hasChangesShifts(true, false);
    } else {
        //Solo queda 1 par visible, lo ocultamos y limpiamos
        visiblePairs[0].querySelectorAll("input").forEach(input => input.value = "");
        updatePattern(false); //ocultamos botón de eliminar, ya que no tiene sentido
        hideDeletePairBtn(document.querySelector("[id*='drPair" + currentIndx + "'] .dx-link-delete"))
        hasChangesShifts(true, false);
    }
}

function updatePattern(force = true) {
    resetDeletePairIndx(force);
    checkDRPatternChangeDay();
}

function resetDeletePairIndx(force = true) {
    if (force) {
        document.querySelectorAll("[id*='drPair'] .dx-link-delete").forEach(e => hideDeletePairBtn(e))
        const currentIndx = 5 - document.querySelectorAll("[id*='drPair'][style*='display:none']").length;
        showDeletePairBtn(document.querySelector("[id*='drPair" + currentIndx + "'] .dx-link-delete"))
    }
}

function hideDeletePairBtn(e) {
    if (e) {
        e.style.opacity = "0";
        e.style.pointerEvents = "none";
    }
}

function showDeletePairBtn(e) {
    if (e) {
        e.style.opacity = "1";
        e.style.pointerEvents = "inherit";
    }
}

function checkDRPatternChangeDay() {
    document.querySelector(".dailyRecord_container .dayChanged").style.display = "none";
    let prevPunch = 0;
    let hasChangedDay = false;
    document.querySelectorAll("[id*='drPair'] input").forEach(e => {
        if (e.value.length > 0) {
            const valuePunch = parseInt(e.value.replace(":", ""))
            if (valuePunch < prevPunch) document.querySelector(".dailyRecord_container .dayChanged").style.display = "block";
            prevPunch = valuePunch;
        }
    })
}

function newShift() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=Shift";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newDocumentAbsence', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        var gridRules = document.getElementById('gridRules');
        var msgNoRule = document.getElementById('msgRuleNoSave');
        var strMsg = '';
        if (msgNoRule != null) {
            strMsg = msgNoRule.value;
        }

        if (gridRules != null) {
            gridRules.textContent = strMsg;
        }

        cargaShift(-1);
        newObjectName = ObjName;
    } catch (e) { showError('newConcept', e); }
}

function copyShift() {
    try {
        if (actualShift == -1) {
            showErrorPopup("Info.SelectOrSaveShiftTitle",
                "INFO",
                "Info.SelectOrSaveShiftTitleDescription",
                "",
                "Info.SelectOrSaveShift.Accept",
                "Info.SelectOrSaveShift.AcceptDesc");
            return;
        } else {
            showLoadingGrid(true);
            AsyncCall("POST", "Handlers/srvShifts.ashx?action=copyXShift&ID=" + actualShift, "json", "arrStatus", "showLoadingGrid(false); checkShiftStatus(arrStatus); if(arrStatus[0].error == 'false'){ refreshTree(); }")
        }
    } catch (e) { showError('copyShift', e); }
}

function ShowRemoveShift() {
    if (actualShift < 0) { return; }

    var url = "Shifts/srvMsgBoxShifts.aspx?action=Message";
    url = url + "&TitleKey=deleteShift.Title";
    url = url + "&DescriptionKey=deleteShift.Description";
    url = url + "&Option1TextKey=deleteShift.Option1Text";
    url = url + "&Option1DescriptionKey=deleteShift.Option1Description";
    url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteShift('" + actualShift + "'); return false;";
    url = url + "&Option2TextKey=deleteShift.Option2Text";
    url = url + "&Option2DescriptionKey=deleteShift.Option2Description";
    url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function deleteShift(oId, usedShift) {
    try {
        showLoadingGrid(true);

        if (usedShift == false) { //No se utiliza, se puede eliminar
            AsyncCall("POST", "Handlers/srvShifts.ashx?action=deleteXShift&ID=" + oId, "json", "arrStatus", "checkShiftStatus(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
        } else {
            AsyncCall("POST", "Handlers/srvShifts.ashx?action=shiftIsUsedForDel&ID=" + oId, "json", "arrStatus", "showLoadingGrid(false); checkShiftStatus(arrStatus); if(arrStatus[0].error == 'false'){ chkDelIsUsed(arrStatus); }")
        }
    } catch (e) { showError('deleteShift', e); }
}

function chkDelIsUsed(arrStatus) {
    try {
        if (arrStatus[0].msg.toUpperCase() == "SHIFTUSEDNO") {
            deleteShift(actualShift, false);
        } else {
            var url = "Shifts/srvMsgBoxShifts.aspx?action=Message";
            url = url + "&TitleKey=deleteShiftUsed.Title";
            url = url + "&DescriptionKey=deleteShiftUsed.Description";
            url = url + "&Option1TextKey=deleteShiftUsed.Option1Text";
            url = url + "&Option1DescriptionKey=deleteShiftUsed.Option1Description";
            url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteShift('" + actualShift + "',false); return false;";
            url = url + "&Option2TextKey=deleteShiftUsed.Option2Text";
            url = url + "&Option2DescriptionKey=deleteShiftUsed.Option2Description";
            url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

            parent.ShowMsgBoxForm(url, 400, 300, '');
        }
    } catch (e) { showError("chkDelIsUsed", e); }
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesShifts";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function changeRules(s, e) {
    try {
        if (s.GetChecked()) {
            cmbRuleConcept1Client.SetVisible(false);
            cmbRuleConcept2Client.SetVisible(true);
        }
        else {
            cmbRuleConcept1Client.SetVisible(true);
            cmbRuleConcept2Client.SetVisible(false);
        }
    } catch (ex) { showError("changeRules", ex); }
}

function changeHolidayTypeVisibility() {
    $('#divHolidaySelectHours').hide();
    $('#divHolidaySelectTime').hide();

    if (cmbHolidayShiftTypeHoursClient.GetSelectedItem().value != 0) {
        if (cmbCauseHolidayClient.GetSelectedItem() != null) {
            if (cmbCauseHolidayClient.GetSelectedItem().value.split("_")[1] == "0") {
                $('#divHolidaySelectHours').show();
            } else {
                $('#divHolidaySelectTime').show();
            }
        }
    };
}

function enableNotifyRelated(s, e) {
    if (s.GetValue()) {
        ckPunchAtTimeClient.SetEnabled(true);
        CkPunchAtTimeDescClient.SetEnabled(true);
        dtPunchAtTimeClient.SetEnabled(true);
    } else {
        ckPunchAtTimeClient.SetEnabled(false);
        CkPunchAtTimeDescClient.SetEnabled(false);
        dtPunchAtTimeClient.SetEnabled(false);
    }
}

function enableDisableCmbWorkdayStartBefore() {
    if (ckWorkdayStartBeforeClient.GetChecked())
        cmbWorkdayStartBefore.SetEnabled(true);
    else
        cmbWorkdayStartBefore.SetEnabled(false);
}

function enableDisableCmbWorkdayStartAfter() {
    if (ckWorkdayStartAfterClient.GetChecked())
        cmbWorkdayStartAfter.SetEnabled(true);
    else
        cmbWorkdayStartAfter.SetEnabled(false);
}

function enableDisableCmbFutureBalance(bAnualWork) {
    if (bAnualWork) {
        //Saldo seleccionado de tipo año laboral
        cmbFutureBalance.SetEnabled(false);
        cmbFutureBalance.SetSelectedItem(cmbFutureBalance.GetItem(0));
    } else {
        cmbFutureBalance.SetEnabled(true);
    }
}

//Función para saber si el saldo seleccionado es de tipo año laboral
function cmbConceptBalanceChanged() {
    const oId = cmbConceptBalance.GetSelectedItem()?.value;
    if (oId >= 0) {
        var postLoad = "if(arrStatus[0].msg.toUpperCase() == 'ISANUALWORK'){ enableDisableCmbFutureBalance(true); }else{ enableDisableCmbFutureBalance(false); }";
        AsyncCall("POST", "Handlers/srvShifts.ashx?action=selectedHolidayConceptIsAnualWork&ID=" + oId, "json", "arrStatus", postLoad)
    }
}
