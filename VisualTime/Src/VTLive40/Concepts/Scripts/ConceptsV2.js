var automaticAccrualConf = {};
var cancelOPCEvents = false;
var actualConceptType = 0;

function cargaConceptNodo(Nodo) {
    try {
        if (actualType == "" || actualType == "C") {
            if (Nodo.id.toUpperCase() == "SOURCE") {
                newConcept();
                return;
            }
            actualConcept = Nodo.id;
            cargaConcept(Nodo.id);
        } else {
            if (Nodo.id.toUpperCase() == "SOURCE") actualConcept = -1
            else actualConcept = Nodo.id;
        }
    } catch (e) { showError("cargaNodo", e); }
}

//Carga Tabs y contenido Empleados
function cargaConcept(IDConcept) {
    actualConcept = IDConcept;
    actualType = "C";
    //TAB Gris Superior
    showLoadingGrid(true);
    cargaConceptTabSuperior(IDConcept);
}

// Carrega la part del TAB grisa superior
function cargaConceptTabSuperior(IDConcept) {
    try {
        var parms = "";

        parms = { "action": "getConceptTab", "aTab": actualConceptTab, "ID": IDConcept };
        AjaxCall("POST", "json", "Handlers/srvConcepts.ashx", parms, "CONTAINER", "divConcepts", "cargaConceptBarButtons(" + IDConcept + ")");
    }
    catch (e) {
        showError("cargaConceptTabSuperior", e);
    }
}
var responseObj = null;
function cargaConceptBarButtons(IDConcept) {
    try {
        var Url = "";

        Url = "Handlers/srvConcepts.ashx?action=getBarButtons&ID=" + IDConcept;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDConcept + ")");
    }
    catch (e) {
        showError("cargaConceptTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IDConcept) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaConceptDivs(IDConcept);
}

// Carrega els apartats dels divs de l'usuari
function cargaConceptDivs(IDConcept) {
    var oParameters = {};
    oParameters.aTab = actualConceptTab;
    oParameters.ID = actualConcept;
    oParameters.oType = "C";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETCONCEPT";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Cambia els Tabs i els divs
function changeTabsConcepts(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03', 'TABBUTTON_04', 'TABBUTTON_05', 'TABBUTTON_06', 'TABBUTTON_07', 'TABBUTTON_08');
    arrDivs = new Array('div00', 'div2', 'div3', 'div4', 'div5', 'div6', 'div7', 'div8', 'div9');

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
    actualConceptTab = numTab;
}

//Cambia els Tabs i els divs (per nom)
function changeTabsByNameConcepts(nameTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03', 'TABBUTTON_04', 'TABBUTTON_05', 'TABBUTTON_06', 'TABBUTTON_07', 'TABBUTTON_08');
    arrDivs = new Array('div00', 'div2', 'div3', 'div4', 'div5', 'div6', 'div7', 'div8', 'div9');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (tab != null && div != null) {
            if (div.id == nameTab) {
                tab.className = 'bTab-active';
                div.style.display = '';
                actualConceptTab = n;
            } else {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }
}

function hasChangesConcepts(bolChanges) {
    try {
        var divTop = document.getElementById('divMsgTop');
        var divBottom = document.getElementById('divMsgBottom');

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
        } else {
            divTop.style.display = 'none';
            divBottom.style.display = 'none';
            document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
        }
    } catch (e) { showError("hasChangesConcepts", e); }
}

function undoChangesConcepts() {
    try {
        if (actualConcept == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesConcepts";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaConceptDivs(actualConcept);
        }
    } catch (e) { showError("undoChangesConcepts", e); }
}

function refreshTreeConcept() {
    eval('ctl00_contentMainBody_roTreesConcepts_roTrees.LoadTreeViews(true, true, true);');
}

function checkCriteriaVisibility() {
    if (document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeesPermissionCriteria_rButton").checked == false) {
        document.getElementById("criteriaCell").style.display = "none";
    } else {
        document.getElementById("criteriaCell").style.display = "";
    }
}

function ConvertGroupsConcept() {
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom');

    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime,ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes,ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optChkRound');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optChkRound_optRoundUP,ctl00_contentMainBody_ASPxCallbackPanelContenido_optChkRound_optRoundDown,ctl00_contentMainBody_ASPxCallbackPanelContenido_optChkRound_optRoundAprox');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_chkDailyRecord');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeesPermissionAll');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeesPermissionNobody');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeesPermissionCriteria');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeesPermissionAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeesPermissionNobody,ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeesPermissionCriteria');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optPayPerHours');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optPayPerHours_optFixedByAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_optPayPerHours_optFixedByEmp,ctl00_contentMainBody_ASPxCallbackPanelContenido_optPayPerHours_optFixedByEmp');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate,ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays,ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');
}

function createGridLine(arrFields) {
    try {
        //Carreguem el array global per mantenir els valors
        var n;

        var oTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridCompositions');

        oTable.setAttribute("border", "0");
        oTable.setAttribute("cellpadding", "0");
        oTable.setAttribute("cellspacing", "0");

        var oNewId = oTable.rows.length;
        var altRow = 1;

        /*Afegim el tr */
        var oTR = oTable.insertRow(-1);
        oTR.id = "htRowC_" + oNewId;
        oTR.setAttribute("name", "htRowsComposition");

        if ((oNewId % 2) != 0) {
            altRow = "1";
        } else {
            altRow = "2";
        }

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
        } else { // IE
        }

        var factorField = "";
        for (n = 0; n < arrFields.length; n++) {
            var fieldName = arrFields[n].field.toUpperCase();
            var value = arrFields[n].value;

            switch (fieldName) {
                case "FACTORUFIELD":
                    factorField = value;
                    break;
            }
        }

        for (n = 0; n < arrFields.length; n++) {
            var fieldName = arrFields[n].field.toUpperCase();
            var controls = arrFields[n].control;
            var strControls = "";
            if (controls != null) {
                for (nC = 0; nC < arrFields[n].control.length; nC++) {
                    strControls += strControls + arrFields[n].control[nC] + ',';
                }
            }
            if (strControls != "") {
                controls = strControls.substring(0, strControls.length - 2);
            } else {
                controls = strControls;
            }

            var value = arrFields[n].value;
            var typeControl = arrFields[n].type.toUpperCase();
            var list = arrFields[n].list;

            switch (fieldName) {
                case "IDCONCEPT":
                    oTR.setAttribute("idconcept", value);
                    break;
                case "IDCAUSETYPE":
                    oTR.setAttribute("idcausetype", value);
                    break;
                case "IDCAUSE":
                    oTR.setAttribute("idcause", value);
                    break;
                case "NAME":
                    var oTD = oTR.insertCell(-1); //Name
                    oTD.className = "GridStyle-cell" + altRow;
                    oTD.textContent = value;
                    oTD.width = "30%";
                    oTD.setAttribute("idTr", "htRowC_" + oNewId);
                    oTR.setAttribute("idTr", "htRowC_" + oNewId);
                    if (window.addEventListener) { // Mozilla, Netscape, Firefox
                        oTD.setAttribute("onmouseover", "javascript: rowOver('htRowC_" + oNewId + "');");
                        oTD.setAttribute("onmouseout", "javascript: rowOut('htRowC_" + oNewId + "');");
                        oTD.setAttribute("onclick", "javascript: editComposition('htRowC_" + oNewId + "');");
                    } else { // IE
                        oTR.onmouseover = function () { rowOver(this.getAttribute("idTr")); }
                        oTR.onmouseout = function () { rowOut(this.getAttribute("idTr")); }

                        oTD.onclick = function () { editComposition(this.getAttribute("idTr")); }
                    }
                    break;
                case "DESCRIPTION":
                    var oTD = oTR.insertCell(-1); //Name
                    oTD.className = "GridStyle-cell" + altRow;
                    oTD.textContent = value;
                    oTD.width = "45%";
                    oTD.setAttribute("idTr", "htRowC_" + oNewId);
                    oTR.setAttribute("idTr", "htRowC_" + oNewId);
                    if (window.addEventListener) { // Mozilla, Netscape, Firefox
                        oTD.setAttribute("onmouseover", "javascript: rowOver('htRowC_" + oNewId + "');");
                        oTD.setAttribute("onmouseout", "javascript: rowOut('htRowC_" + oNewId + "');");
                        oTD.setAttribute("onclick", "javascript: editComposition('htRowC_" + oNewId + "');");
                    } else { // IE
                        /*oTR.onmouseover = function() { rowOver(this.getAttribute("idTr")); }
                        oTR.onmouseout = function() { rowOut(this.getAttribute("idTr")); }*/

                        oTD.onclick = function () { editComposition(this.getAttribute("idTr")); }
                    }
                    break;
                case "CONDITIONCHECK":
                    oTR.setAttribute("conditioncheck", value);
                    break;
                case "FACTORVALUE":
                    var oTD = oTR.insertCell(-1); //Name
                    oTD.className = "GridStyle-cell" + altRow;
                    if (factorField != "") {
                        oTD.innerHTML = factorField;
                    } else {
                        oTD.innerHTML = value;
                    }
                    oTD.width = "25%";
                    oTD.setAttribute("idTr", "htRowC_" + oNewId);
                    oTR.setAttribute("idTr", "htRowC_" + oNewId);
                    if (window.addEventListener) { // Mozilla, Netscape, Firefox
                        oTD.setAttribute("onmouseover", "javascript: rowOver('htRowC_" + oNewId + "');");
                        oTD.setAttribute("onmouseout", "javascript: rowOut('htRowC_" + oNewId + "');");
                        oTD.setAttribute("onclick", "javascript: editComposition('htRowC_" + oNewId + "');");
                    } else { // IE
                        /*oTR.onmouseover = function() { rowOver(this.getAttribute("idTr")); }
                        oTR.onmouseout = function() { rowOut(this.getAttribute("idTr")); }*/

                        oTD.onclick = function () { editComposition(this.getAttribute("idTr")); }
                    }
                    //break;
                    oTR.setAttribute("factorvalue", value);
                    break;
                case "COMPARE":
                    oTR.setAttribute("compare", value);
                    break;
                case "VALUE_TYPE":
                    oTR.setAttribute("value_type", value);
                    break;
                case "VALUE_IDCAUSE":
                    oTR.setAttribute("value_idcause", value);
                    break;
                case "VALUE_DIRECT":
                    oTR.setAttribute("value_direct", value);
                    break;
                case "VALUE_UFIELD":
                    oTR.setAttribute("value_ufield", value);
                    break;
                case "CAUSESCONDITIONS":
                    oTR.setAttribute("causesconditions", value);
                    break;

                case "IDCONCEPTCOMPOSITION":
                    oTR.setAttribute("IDConceptComposition", value);
                    break;

                case "IDSHIFT":
                    oTR.setAttribute("IdShift", value);
                    break;
                case "IDTYPE":
                    oTR.setAttribute("IDType", value);
                    break;
                case "TYPEDAYPLANNED":
                    oTR.setAttribute("TypeDayPlanned", value);
                    break;
                case "FACTORUFIELD":
                    oTR.setAttribute("FactorUField", value);
                    break;
            } //end switch
        } //end for

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true") {
            //Creem la barra d'eines al TR
            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            //oTD.style.borderRight = "solid 1px silver";
            oTD.width = "50px";
            oTD.style.whiteSpace = "nowrap";
            oTD.style.borderRight = "solid 1px silver";

            var aEdit = document.createElement("A");
            var aDelete = document.createElement("A");

            aEdit.href = "javascript: void(0);";
            aDelete.href = "javascript: void(0);";
            aEdit.title = document.getElementById("tagEditTitle").value;
            aDelete.title = document.getElementById("tagRemoveTitle").value;

            aEdit.setAttribute("row", "htRowC_" + oNewId);
            aDelete.setAttribute("row", "htRowC_" + oNewId);

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                aEdit.setAttribute("onclick", "javascript: editComposition('" + oTR.id + "');");
                aDelete.setAttribute("onclick", "javascript: deleteComposition('" + oTR.id + "');");
            } else { // IE
                aEdit.onclick = function () { editComposition(this.getAttribute("row")); }
                aDelete.onclick = function () { deleteComposition(this.getAttribute("row")); }
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
        } else {
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_imgAddListValue').onclick = "";
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_imgRemoveListValue').onclick = "";
        }
        return true;
    } catch (e) {
        showError("createGridLine", e);
        return false;
    } //end try
}

function editGridLine(arrFields) {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridCompositions');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsComposition");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsComposition");
                break;
            default:
                hRows = document.getElementsByName("htRowsComposition");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsComposition");
                }
                break;
        }

        var oIDConceptComposition;
        var factorField = "";
        for (n = 0; n < arrFields.length; n++) {
            var fieldName = arrFields[n].field.toUpperCase();
            var controls = arrFields[n].control;
            var value = arrFields[n].value;
            var typeControl = arrFields[n].type.toUpperCase();
            var list = arrFields[n].list;

            switch (fieldName) {
                case "IDCONCEPTCOMPOSITION":
                    oIDConceptComposition = value;
                    break;
                case "FACTORUFIELD":
                    factorField = value;
                    break;
            }
        }

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var oTR = hRows[n];
            if (oTR.getAttribute("IDConceptComposition") == oIDConceptComposition) {
                for (n = 0; n < arrFields.length; n++) {
                    var fieldName = arrFields[n].field.toUpperCase();
                    var controls = arrFields[n].control;
                    var value = arrFields[n].value;
                    var typeControl = arrFields[n].type.toUpperCase();
                    var list = arrFields[n].list;

                    switch (fieldName) {
                        case "IDCONCEPT":
                            oTR.setAttribute("idconcept", value);
                            break;
                        case "IDCAUSE":
                            oTR.setAttribute("idcause", value);
                            break;
                        case "IDCAUSETYPE":
                            oTR.setAttribute("idcausetype", value);
                            break;
                        case "NAME":
                            oTR.cells[0].textContent = value;
                            break;
                        case "DESCRIPTION":
                            oTR.cells[1].textContent = value;
                            break;
                        case "CONDITIONCHECK":
                            oTR.setAttribute("conditioncheck", value);
                            break;
                        case "FACTORVALUE":
                            oTR.setAttribute("factorvalue", value);
                            if (value != "" && factorField == "") {
                                oTR.cells[2].innerHTML = value;
                            }
                            break;
                        case "COMPARE":
                            oTR.setAttribute("compare", value);
                            break;
                        case "VALUE_TYPE":
                            oTR.setAttribute("value_type", value);
                            break;
                        case "VALUE_IDCAUSE":
                            oTR.setAttribute("value_idcause", value);
                            break;
                        case "VALUE_UFIELD":
                            oTR.setAttribute("value_ufield", value);
                            break;
                        case "VALUE_DIRECT":
                            oTR.setAttribute("value_direct", value);
                            break;
                        case "CAUSESCONDITIONS":
                            oTR.setAttribute("causesconditions", value);
                            break;

                        case "IDCONCEPTCOMPOSITION":
                            oTR.setAttribute("IDConceptComposition", value);
                            break;

                        case "IDSHIFT":
                            oTR.setAttribute("IdShift", value);
                            break;
                        case "IDTYPE":
                            oTR.setAttribute("IDType", value);
                            break;
                        case "TYPEDAYPLANNED":
                            oTR.setAttribute("TypeDayPlanned", value);
                            break;
                        case "FACTORUFIELD":
                            oTR.setAttribute("FactorUField", value);
                            if (value != "") {
                                oTR.cells[2].innerHTML = value;
                            }
                            break;
                    } //end switch
                } //end for
                return true;
            }
        }
    } catch (e) { showError("EditGridLine", e); }
}

function checkIfExistIDCauseInGrid(IDConceptComposition) {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridCompositions');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsComposition");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsComposition");
                break;
            default:
                hRows = document.getElementsByName("htRowsComposition");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsComposition");
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("IDConceptComposition") == IDConceptComposition) {
                return true;
            }
        }

        return false;
        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("checkIfExistIDCauseInGrid", e); }
}

function getNextConceptCompositionID() {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridCompositions');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsComposition");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsComposition");
                break;
            default:
                hRows = document.getElementsByName("htRowsComposition");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsComposition");
                }
                break;
        }

        var CurrentMaxID = -1;
        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            var tmpID = parseInt(hRow.getAttribute("IDConceptComposition"), 10);
            if (tmpID > CurrentMaxID) {
                CurrentMaxID = tmpID;
            }
        }

        return CurrentMaxID + 1;
    } catch (e) { showError("getNextConceptCompositionID", e); }
}

var alertShown = false;
function conceptTypeHasChanges(callerID) {
    var canChange = true;
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridCompositions');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsComposition");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsComposition");
                break;
            default:
                hRows = document.getElementsByName("htRowsComposition");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsComposition");
                }
                break;
        }

        if (hRows.length > 0) canChange = false;

        //Bucle per les files del grid, per cambiar els estils
        //for (var n = 0; n < hRows.length; n++) {
        //    var hRow = hRows[n];
        //    var tmpID = parseInt(hRow.getAttribute("IDType"), 10);
        //    if (tmpID > 0) {
        //        canChange = false;
        //        break;
        //    }
        //}

        if (canChange) {
            hasChanges(true);

            if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime_rButton').checked == true) {
                //Saldo de horas
                actualConceptType = 0;

                //Visibilidad de consultas
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays_panOptionPanel').setAttribute("venabled", "True");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours_panOptionPanel').setAttribute("venabled", "True");

                chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours,ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays', 'undefined');

                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays_panOptionPanel').setAttribute("venabled", "False");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours_panOptionPanel').setAttribute("venabled", "False");

                //Visibilidad de devengos
                if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_rButton').checked == true) {
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");

                    chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals', 'undefined');

                    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals');
                    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals');
                    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');

                    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');
                }

                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_panOptionPanel').setAttribute("venabled", "False");
            } else if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes_rButton').checked == true) {
                //saldo de veces
                actualConceptType = 1;

                //Visibilidad de consultas
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays_panOptionPanel').setAttribute("venabled", "True");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours_panOptionPanel').setAttribute("venabled", "True");

                chgOPCItems('1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours,ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays', 'undefined');

                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays_panOptionPanel').setAttribute("venabled", "False");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours_panOptionPanel').setAttribute("venabled", "False");

                //Visibilidad de devengos
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
            } else if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom_rButton').checked == true) {
                //Saldo personalizado
                actualConceptType = 2;

                //Visibilidad de consultas
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays_panOptionPanel').setAttribute("venabled", "True");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours_panOptionPanel').setAttribute("venabled", "True");

                //Visibilidad de devengos
                if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_rButton').checked == true ||
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals_rButton').checked == true) {
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");

                    chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals', 'undefined');

                    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals');
                    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals');
                    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');

                    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');
                }

                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals_panOptionPanel').setAttribute("venabled", "False");
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_panOptionPanel').setAttribute("venabled", "False");
            }

            venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewDays');
            venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optViewHours');

            venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals');
            venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals');
            venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');
            linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');
        } else {
            if (alertShown == false) {
                alertShown = true;
                showErrorPopup("CauseTypeHasComposition.Error.Text", "error", "CauseTypeHasComposition.Error.Description", "", "CauseTypeHasComposition.Error.Option1Text", "CauseTypeHasComposition.Error.Option1Description", "window.frames['ifPrincipal'].chgOPCItems('" + actualConceptType + "', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime,ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes,ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom', 'undefined');window.frames['ifPrincipal'].alertShown = false;")
            }
        }

        return canChange;
    } catch (e) { showError("getNextConceptCompositionID", e); }
}

function frmCompositionChanges() {
    var oConditionCheck = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_chkButton').checked;
    if (oConditionCheck == true) {
        cmbCompareClient.SetEnabled(true);
        cmbTypeValueClient.SetEnabled(true);
        cmbCauseTypeClient.SetEnabled(true);
        cmbCauseUFieldClient.SetEnabled(true);
    } else {
        cmbCompareClient.SetEnabled(false);
        cmbTypeValueClient.SetEnabled(false);
        cmbCauseTypeClient.SetEnabled(false);
        cmbCauseUFieldClient.SetEnabled(false);
    }
}

function AddNewComposition() {
    try {
        clearCausesConditionsGrid();
        var oDis = "false";

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "false") {
            oDis = "true";
        }

        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime_rButton').checked == true) {
            ckCause_client.SetEnabled(true);
            ckIncidence_client.SetEnabled(false);
            ckShift_client.SetEnabled(false);
            cmbDayCauseClient.SetEnabled(false);
            ckDayContainsCause_client.SetEnabled(false);
            ckValue_client.SetEnabled(false);
        } else {
            ckCause_client.SetEnabled(false);
            ckIncidence_client.SetEnabled(true);
            ckShift_client.SetEnabled(true);
            cmbDayCauseClient.SetEnabled(true);
            ckDayContainsCause_client.SetEnabled(true);
            ckValue_client.SetEnabled(true);
        }

        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes_rButton').checked == true) {
            cmbValueDailyCauseClient.SetVisible(true);
            cmbValueCustomCauseClient.SetVisible(false);
        } else {
            cmbValueDailyCauseClient.SetVisible(false);
            cmbValueCustomCauseClient.SetVisible(true);
        }              

        ckFixedValue_client.SetValue(true);
        ckFactorUserField_client.SetValue(false);
        txtFactorCompositionClient.SetValue("1");
        cmbFactorUserFieldClient.SetValue("");
        cmbFactorUserFieldClient.SetEnabled(false);

        if (ckCause_client.GetEnabled()) {
            ckCause_client.SetValue(true);
            cmbCauseClient.SetValue("");
            cmbCauseClient.SetEnabled(true);
        } else {
            ckCause_client.SetValue(false);
            cmbCauseClient.SetValue("");
            cmbCauseClient.SetEnabled(false);
        }

        if (ckCause_client.GetEnabled()) {
            ckValue_client.SetValue(false);
            cmbValueDailyCauseClient.SetValue("");
            cmbValueDailyCauseClient.SetEnabled(false);
            cmbValueCustomCauseClient.SetValue("");
            cmbValueCustomCauseClient.SetEnabled(false);
        } else {
            ckValue_client.SetValue(true);
            cmbValueDailyCauseClient.SetValue("");
            cmbValueDailyCauseClient.SetEnabled(true);
            cmbValueCustomCauseClient.SetValue("");
            cmbValueCustomCauseClient.SetEnabled(true);
        }

        if (cmbShowValueClient.GetValue() == 'L') {
            ckCause_client.SetEnabled(false);
            ckIncidence_client.SetEnabled(false);
            ckShift_client.SetEnabled(true);
            ckShift_client.SetValue(true);
            cmbDayCauseClient.SetEnabled(false);
            ckDayContainsCause_client.SetEnabled(false);
            cmbValueDailyCauseClient.SetEnabled(false);
            ckValue_client.SetEnabled(false);
            ckFactorUserField_client.SetEnabled(false);
            txtFactorCompositionClient.SetValue(-1);            
            txtFactorCompositionClient.SetEnabled(false);
           
        } 

        ckDayContainsCause_client.SetValue(false);
        cmbDayContainsCauseClient.SetValue("");
        cmbDayContainsCauseClient.SetEnabled(false);

        conceptCompositionDataClient.Set("IsNew", true);
        conceptCompositionDataClient.Set("IDConceptComposition", getNextConceptCompositionID());
        conceptCompositionDataClient.Set("IDType", "0");

        ckShift_client.SetValue(false);
        cmbShiftClient.SetValue("");
        cmbShiftClient.SetEnabled(false);

        ckIncidence_client.SetValue(false);
        cmbDayCauseClient.SetValue("");
        cmbDayCauseClient.SetEnabled(false);

        cmbDaysTypeClient.SetEnabled(false);
        cmbDaysTypeClient.SetValue("");
        cmbDaysTypeCauseClient.SetEnabled(false);
        cmbDaysTypeCauseClient.SetValue("");

        cmbCompareClient.SetValue("0");
        cmbTypeValueClient.SetValue("0");
        cmbCauseTypeClient.SetValue("0");
        cmbCauseUFieldClient.SetValue("");        

        if (cmbShowValueClient.GetValue() == 'L') {
            ckCause_client.SetEnabled(false);
            ckIncidence_client.SetEnabled(false);
            ckShift_client.SetEnabled(true);
            ckShift_client.SetValue(true);
            cmbDayCauseClient.SetEnabled(false);
            ckDayContainsCause_client.SetEnabled(false);
            cmbValueDailyCauseClient.SetEnabled(false);
            ckValue_client.SetEnabled(false);
            cmbShiftClient.SetEnabled(true);
            cmbDaysTypeClient.SetEnabled(true);
        } 

        loadGenericData("VALUE_DIRECT", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_txtValueType", "00:00", "X_TEXT", "", oDis, false);
        loadGenericData("ConditionCheck", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition", "false", "X_OPTIONCHECK", "", oDis, false);

        cmbCauseTypeClient.SetVisible(false);
        cmbCauseUFieldClient.SetVisible(false);
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_txtValueType').style.display = '';

        disableScreen(true);
        showWndCompositions(true);
    } catch (e) { showError("AddNewComposition", e); }
}

function editComposition(objId) {
    try {
        var oRow = document.getElementById(objId);
        if (oRow == null) { return; }
        var oDis = "false";

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "false") {
            oDis = "true";
        }

        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime_rButton').checked == true) {
            ckCause_client.SetEnabled(true);
            ckIncidence_client.SetEnabled(false);
            ckShift_client.SetEnabled(false);
            cmbDayCauseClient.SetEnabled(false);
            ckDayContainsCause_client.SetEnabled(false);
            ckValue_client.SetEnabled(false);
        } else {
            ckCause_client.SetEnabled(false);
            ckIncidence_client.SetEnabled(true);
            ckShift_client.SetEnabled(true);
            cmbDayCauseClient.SetEnabled(true);
            ckDayContainsCause_client.SetEnabled(true);
            ckValue_client.SetEnabled(true);
        }

        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes_rButton').checked == true) {
            cmbValueDailyCauseClient.SetVisible(true);
            cmbValueCustomCauseClient.SetVisible(false);
        } else {
            cmbValueDailyCauseClient.SetVisible(false);
            cmbValueCustomCauseClient.SetVisible(true);
        }

        conceptCompositionDataClient.Set("IsNew", false);
        conceptCompositionDataClient.Set("IDConceptComposition", oRow.getAttribute("IDConceptComposition"));
        conceptCompositionDataClient.Set("IDType", oRow.getAttribute("IDType"));

        txtFactorCompositionClient.SetValue(oRow.getAttribute("factorvalue"));

        cmbCompareClient.SetValue(oRow.getAttribute("compare"));
        cmbTypeValueClient.SetValue(oRow.getAttribute("value_type"));
        cmbCauseTypeClient.SetValue(oRow.getAttribute("value_idcause"));
        cmbCauseUFieldClient.SetValue(oRow.getAttribute("value_ufield"));

        var idshiftTmp = oRow.getAttribute("IdShift") + "_";
        var isHolidays = 0;
        for (var i = 0; i < cmbShiftClient.GetItemCount(); i++) {
            if (cmbShiftClient.GetItem(i).value.indexOf(idshiftTmp) == 0) {
                isHolidays = parseInt(cmbShiftClient.GetItem(i).value.split("_")[1], 10);
                cmbShiftClient.SetValue(cmbShiftClient.GetItem(i).value);
                break;
            }
        }

        cmbDayCauseClient.SetValue(oRow.getAttribute("idcause"));

        switch (oRow.getAttribute("IDType")) {
            case "0":
                break;
            case "1":
                cmbDaysTypeClient.SetValue(oRow.getAttribute("TypeDayPlanned"));
                cmbDaysTypeCauseClient.SetValue('');
                break;
            case "2":
                cmbDaysTypeClient.SetValue('');
                cmbDaysTypeCauseClient.SetValue(oRow.getAttribute("TypeDayPlanned"));
                break;
        }

        if (oRow.getAttribute("FactorUField") != "") {
            txtFactorCompositionClient.SetValue("0");
            cmbFactorUserFieldClient.SetValue(oRow.getAttribute("FactorUField"));
        }

        loadGenericData("VALUE_DIRECT", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_txtValueType", oRow.getAttribute("value_direct").toString(), "X_TEXT", "", oDis, false);
        loadGenericData("ConditionCheck", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition", oRow.getAttribute("conditioncheck").toString(), "X_OPTIONCHECK", "", oDis, false);

        if (oRow.getAttribute("conditioncheck").toString() == "false") {
            cmbCauseClient.SetEnabled(false);
            cmbCompareClient.SetEnabled(false);
            cmbTypeValueClient.SetEnabled(false);
        }

        if (oDis == "true") {
            btnCancelClient.SetVisible(false);
        }

        clearCausesConditionsGrid();
        loadCausesConditionsGrid(oRow.getAttribute("causesconditions"));

        var txtValueType = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_txtValueType')

        cmbCauseTypeClient.SetVisible(false);
        cmbCauseUFieldClient.SetVisible(false);
        txtValueType.style.display = 'none';

        switch (oRow.getAttribute("value_type")) {
            case "0": txtValueType.style.display = ''; break;
            case "1": cmbCauseUFieldClient.SetVisible(true); break;
            case "2": cmbCauseTypeClient.SetVisible(true); break;
        }

        if (cmbFactorUserFieldClient.GetValue() != null && cmbFactorUserFieldClient.GetValue() != "") {
            ckFixedValue_client.SetValue(false);
            txtFactorCompositionClient.SetValue("0");
            txtFactorCompositionClient.SetEnabled(false);
            ckFactorUserField_client.SetValue(true);
            cmbFactorUserFieldClient.SetEnabled(true);
        } else {
            ckFixedValue_client.SetValue(true);
            ckFactorUserField_client.SetValue(false);
            cmbFactorUserFieldClient.SetEnabled(false);
            cmbFactorUserFieldClient.SetValue("");
        }

        var idcauseType = parseInt(oRow.getAttribute("idcausetype"), 10);

        switch (oRow.getAttribute("IDType")) {
            case "0":
                if (ckCause_client.GetEnabled()) {
                    cmbCauseClient.SetValue(oRow.getAttribute("idcause"));
                    cmbCauseClient.SetEnabled(true);
                    ckCause_client.SetValue(true);

                    ckValue_client.SetValue(false);
                    cmbValueDailyCauseClient.SetValue("");
                    cmbValueDailyCauseClient.SetEnabled(false);

                    cmbValueCustomCauseClient.SetValue("");
                    cmbValueCustomCauseClient.SetEnabled(false);

                    ckDayContainsCause_client.SetValue(false);
                    cmbDayContainsCauseClient.SetValue("");
                    cmbDayContainsCauseClient.SetEnabled(false);
                } else {
                    cmbCauseClient.SetValue("");
                    cmbCauseClient.SetEnabled(false);
                    ckCause_client.SetValue(false);

                    ckValue_client.SetValue(false);
                    cmbValueDailyCauseClient.SetValue("");
                    cmbValueDailyCauseClient.SetEnabled(false);

                    cmbValueCustomCauseClient.SetValue("");
                    cmbValueCustomCauseClient.SetEnabled(false);

                    ckDayContainsCause_client.SetValue(false);
                    cmbDayContainsCauseClient.SetValue("");
                    cmbDayContainsCauseClient.SetEnabled(false);
                    if (idcauseType == 0) {
                        ckValue_client.SetValue(true);

                        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes_rButton').checked == true) {
                            cmbValueDailyCauseClient.SetValue(oRow.getAttribute("idcause"));
                            cmbValueDailyCauseClient.SetEnabled(true);
                        } else {
                            cmbValueCustomCauseClient.SetValue(oRow.getAttribute("idcause"));
                            cmbValueCustomCauseClient.SetEnabled(true);
                        }
                    } else {
                        ckDayContainsCause_client.SetValue(true);
                        cmbDayContainsCauseClient.SetValue(oRow.getAttribute("idcause"));
                        cmbDayContainsCauseClient.SetEnabled(true);
                    }
                }
                ckIncidence_client.SetValue(false);
                ckShift_client.SetValue(false);
                cmbShiftClient.SetEnabled(false);
                cmbShiftClient.SetValue("");
                cmbDayCauseClient.SetEnabled(false);
                cmbDayCauseClient.SetValue("");
                cmbDaysTypeClient.SetEnabled(false);
                cmbDaysTypeClient.SetValue("");
                cmbDaysTypeCauseClient.SetEnabled(false);
                cmbDaysTypeCauseClient.SetValue("");
                break;
            case "1":
                ckCause_client.SetValue(false);
                ckIncidence_client.SetValue(false);
                ckShift_client.SetValue(true);
                cmbShiftClient.SetEnabled(true);
                cmbDaysTypeClient.SetEnabled(true);
                cmbCauseClient.SetValue("");
                cmbCauseClient.SetEnabled(false);
                cmbDayCauseClient.SetValue("");
                cmbDayCauseClient.SetEnabled(false);

                if (isHolidays == 2) {
                    cmbDaysTypeClient.SetEnabled(true);
                } else {
                    cmbDaysTypeClient.SetEnabled(false);
                    cmbDaysTypeClient.SetValue('0');
                }
                cmbDaysTypeCauseClient.SetEnabled(false);
                cmbDaysTypeCauseClient.SetValue("");

                ckValue_client.SetValue(false);
                cmbValueDailyCauseClient.SetValue("");
                cmbValueDailyCauseClient.SetEnabled(false);
                cmbValueCustomCauseClient.SetValue("");
                cmbValueCustomCauseClient.SetEnabled(false);

                ckDayContainsCause_client.SetValue(false);
                cmbDayContainsCauseClient.SetValue("");
                cmbDayContainsCauseClient.SetEnabled(false);

                break;
            case "2":
                cmbDayCauseClient.SetValue(oRow.getAttribute("idcause"));
                cmbDayCauseClient.SetEnabled(true);
                ckCause_client.SetValue(false);
                ckIncidence_client.SetValue(true);
                cmbDaysTypeClient.SetEnabled(false);
                cmbDaysTypeClient.SetValue('');
                cmbDaysTypeCauseClient.SetEnabled(true);
                cmbShiftClient.SetValue("");
                cmbShiftClient.SetEnabled(false);
                cmbCauseClient.SetValue("");
                cmbCauseClient.SetEnabled(false);

                ckValue_client.SetValue(false);
                cmbValueDailyCauseClient.SetValue("");
                cmbValueDailyCauseClient.SetEnabled(false);
                cmbValueCustomCauseClient.SetValue("");
                cmbValueCustomCauseClient.SetEnabled(false);

                ckDayContainsCause_client.SetValue(false);
                cmbDayContainsCauseClient.SetValue("");
                cmbDayContainsCauseClient.SetEnabled(false);
                break;
        }

        disableScreen(true);
        showWndCompositions(true);
    } catch (e) { showError("editComposition", e); }
}

function showTypeValue(type) {
    var txt = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_txtValueType')

    cmbCauseTypeClient.SetVisible(false);
    cmbCauseUFieldClient.SetVisible(false);
    txt.style.display = 'none';
    switch (type) {
        case 0:
            txt.style.display = "";
            break;
        case 1:
            cmbCauseUFieldClient.SetVisible(true);
            break;
        case 2:
            cmbCauseTypeClient.SetVisible(true);
            break;
    }
}

function clearCausesConditionsGrid() {
    try {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_divConditionsCauses').innerHTML = '';
    } catch (e) { showError("clearCausesConditions", e); }
}

function loadCausesConditionsGrid(strCausesConditions) {
    try {
        if (strCausesConditions == "" || strCausesConditions == "null") { return; }
        var arrCausesConditions = strCausesConditions.split("|");

        if (arrCausesConditions.length > 0) {
            for (var x = 0; x < arrCausesConditions.length; x++) {
                var arrCCFields = arrCausesConditions[x].split(",");
                AddListValue(arrCCFields[0], arrCCFields[1], arrCCFields[2]);
            }
        }
    } catch (e) { showError("loadCausesConditions", e); }
}

function deleteComposition(objId) {
    try {
        var IDConcept = actualConcept;

        var oRow = document.getElementById(objId);
        if (oRow == null) { return; }

        var IdComposition = oRow.getAttribute("IDConceptComposition");

        var url = "Concepts/srvMsgBoxConcepts.aspx?action=Message&IDConcept=" + IDConcept + "&IDComposition=" + IdComposition;
        url = url + "&TitleKey=deleteGrid.Denied.Title";
        url = url + "&DescriptionKey=deleteGrid.Denied.Description";
        url = url + "&Option1TextKey=deleteGrid.Denied.Option1Text";
        url = url + "&Option1DescriptionKey=deleteGrid.Denied.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteCompositionRow('" + objId + "'); return false;";
        url = url + "&Option2TextKey=deleteGrid.Denied.Option2Text";
        url = url + "&Option2DescriptionKey=deleteGrid.Denied.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteComposition", e); }
}

function deleteCompositionRow(objId) {
    setCompositionChanges(true);
    RemoveListComposition(objId);
    ReeStyleListComposition();
    hasChanges(true);
}

function setCompositionChanges(bol) {
    try {
        if (bol) {
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnCompositionChanges').value = "1";
        } else {
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnCompositionChanges').value = "0";
        }
    } catch (e) { showError("setCompositionChanges", e); }
}

function RemoveListComposition(objId) {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridCompositions');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsComposition");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsComposition");
                break;
            default:
                hRows = document.getElementsByName("htRowsComposition");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsComposition");
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

function ReeStyleListComposition() {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridCompositions');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsComposition");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsComposition");
                break;
            default:
                hRows = document.getElementsByName("htRowsComposition");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsComposition");
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

function createGridCompositions(arrCC) {
    try {
        if (arrCC == null) { return; }
        if (arrCC.length == 0) { return; }

        if (arrCC[0].error == "true") {
            CheckConceptStatus(arrCC);
            return;
        }

        var n;
        for (n = 0; n < arrCC.length; n++) {
            createGridLine(arrCC[n].composition);
        } //end for
    }
    catch (e) {
        showLoadingGrid(false);
        showError('createGridCompositions', e);
    }
}

function CheckConceptStatus(oStatus) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Concepts/srvMsgBoxConcepts.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else { //Missatge estil inline
            }
            hasChanges(true);
        }
    }
    catch (e) {
        showLoadingGrid(false);
        alert('checkStatus: ' + e);
    }
} //end checkStatus function

function rowClick(rowID, ID, dTable) {
    document.getElementById('selectedIdx').value = ID;
    var tParent = document.getElementById(dTable);
    var tCells = tParent.getElementsByTagName("td");
    for (var i = 0; i < tCells.length; i++) {
        removeCssClass(tCells[i], "gridRowOver");
        removeCssClass(tCells[i], "gridRowSelected");
    }

    var table = document.getElementById(rowID);
    var cells = table.getElementsByTagName("td");
    for (var i = 0; i < cells.length; i++) {
        removeCssClass(cells[i], "gridRowOver");
        addCssClass(cells[i], "gridRowSelected");
    }
}

function addCssClass(obj, clsTxt) {
    obj.className = obj.className + ' ' + clsTxt;
}

function removeCssClass(obj, clsTxt) {
    var parmCss = new Array();
    parmCss = obj.className.split(" ");

    obj.className = ''; //Reset dels CSS
    //Carreguem tots els anteriors atributs
    for (nCss = 0; nCss < parmCss.length; nCss++) {
        if (parmCss[nCss] != clsTxt) {
            obj.className = obj.className + ' ' + parmCss[nCss];
        }
    }
}

function ShowWindow(objId, bol) {
    try {
        var objWnd = document.getElementById(objId);
        var oDivBg;
        if (objWnd != null) {
            if (bol == true) {
                oDivBg = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_BgS');
                oDivBg.style.display = "";
                oDivBg.style.top = "0";
                oDivBg.style.left = "0";
                oDivBg.style.height = 2000;  //document.body.offsetHeight;
                oDivBg.style.width = 3000;  //document.body.offsetWidth;
                oDivBg.style.backgroundColor = "transparent";

                objWnd.style.display = "";
            } else {
                oDivBg = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_BgS');
                oDivBg.style.display = "none";
                objWnd.style.display = "none";
            }
        }
    } catch (e) { showError("ShowWindow", e); }
}

function AddCauseOK() {
    try {
        var cmb = cmbCauseAddClient;

        var opOperation = document.getElementById("opPlus");
        var opStr = '+';

        if (cmb == null) { return; }

        if (cmb.GetSelectedItem().value == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoCauseSelected", "", "Error.OK", "Error.OKDesc", "");
            return;
        }

        if (opOperation.checked == false) {
            opStr = '-';
        }

        var bolRet = AddListValue(cmb.GetSelectedItem().value, cmb.GetSelectedItem().text, opStr);

        //Cerramos la ventana si todo ok
        if (bolRet == true) {
            ShowWindow('divNewCauseConditions', false);
            //hasChanges(true);
        }
    } catch (e) { showError("AddCauseOK", e); }
}

function AddListValue(IDCause, Name, Operation) {
    try {
        //Si existe en la lista, devolvemos un error
        if (checkIfExistValueinList(IDCause)) {
            //Mostrar error
            return false;
        }

        var oTable = document.getElementById('htTableCondition');

        if (oTable == null) {
            oTable = document.createElement("TABLE");
            oTable.id = "htTableCondition";
            oTable.className = "GridStyle GridEmpleados";
            oTable.setAttribute("border", "0");
            oTable.setAttribute("cellPadding", "0");
            oTable.setAttribute("cellSpacing", "0");

            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_divConditionsCauses').appendChild(oTable);
            oTable.setAttribute("style", "width: 100%;");
            oTable.style.width = "100%";

            var oTR = oTable.insertRow(-1);
            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.width = 200;
            oTD.setAttribute("style", "border-right: 0pt none;");
            oTD.textContent = document.getElementById("header1").value;

            var oTD = oTR.insertCell(-1); //Name
            oTD.className = "GridStyle-cellheader";
            oTD.textContent = document.getElementById("header2").value;
        }

        var oNewId = oTable.rows.length;
        var altRow = 1;

        /*Afegim el tr */
        var oTR = oTable.insertRow(-1);
        oTR.id = "htRow_" + oNewId;
        oTR.setAttribute("name", "htRows");
        oTR.setAttribute("IDCause", IDCause);

        oTR.setAttribute("Operation", Operation);

        if (window.addEventListener) { // Mozilla, Netscape, Firefox
            oTR.setAttribute("onmouseover", "javascript: rowOver('" + oTR.id + "');");
            oTR.setAttribute("onmouseout", "javascript: rowOut('" + oTR.id + "');");
            oTR.setAttribute("onclick", "javascript: rowClick('" + oTR.id + "','" + IDCause + "','htTableCondition');");
        } else { // IE
            oTR.onmouseover = function () { rowOver(this.id); }
            oTR.onmouseout = function () { rowOut(this.id); }
            oTR.onclick = function () { rowClick(this.id, this.getAttribute("IDCause"), 'htTableCondition'); }
        }

        if ((oNewId % 2) != 0) {
            altRow = "1";
        } else {
            altRow = "2";
        }

        var oTD = oTR.insertCell(-1); //Name
        oTD.className = "GridStyle-cell" + altRow;
        oTD.textContent = Name;

        var oTD = oTR.insertCell(-1); //Name
        oTD.className = "GridStyle-cell" + altRow;

        if (Operation.trim() == "+") {
            oTD.textContent = document.getElementById('OperationPlus').value;
        } else {
            oTD.textContent = document.getElementById('OperationMinus').value;
        }

        return true;
    } catch (e) {
        showError("AddListValue", e);
        return false;
    }
} //end function AddListValue

function getCountCausesConditionsGrid() {
    try {
        var arr = new Array();
        var hTable = document.getElementById('htTableCondition');

        //Si no hi ha taula
        if (hTable == null) { return 0; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRows");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRows");
                break;
            default:
                hRows = document.getElementsByName("htRows");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRows");
                }
                break;
        } //end switch

        return hRows.length;
    } catch (e) {
        showError("retrieveCausesConditionsGrid", e);
        return null;
    }
}

function checkIfExistValueinList(IDCause) {
    try {
        var hTable = document.getElementById('htTableCondition');

        //Si no hi ha taula
        if (hTable == null) { return; }
        if (IDCause == "") { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRows");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRows");
                break;
            default:
                hRows = document.getElementsByName("htRows");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRows");
                }
                break;
        }

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("IDCause") == IDCause) {
                //si troba el row, l'eliminem (n+1, per la capcelera)
                return true;
            }
        }

        return false;

        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("checkIfExistValueinList", e); return false; }
}

function RemoveListValue() {
    try {
        var IDCause = document.getElementById('selectedIdx').value;
        var hTable = document.getElementById('htTableCondition');

        //Si no hi ha taula
        if (hTable == null) { return; }
        if (IDCause == "") { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRows");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRows");
                break;
            default:
                hRows = document.getElementsByName("htRows");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRows");
                }
                break;
        }

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("IDCause") == IDCause) {
                //si troba el row, l'eliminem (n+1, per la capcelera)
                hTable.deleteRow(n + 1);
                hasChanges(true);
                return;
            }
        }

        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("RemoveListValue", e); }
}

function validateEditComposition() {
    try {
        //Comprobem si esta el idcause com read-only (edicio) o es combobox
        var cmbValue = "";

        if (ckCause_client.GetValue() == true) {
            if (cmbCauseClient.GetSelectedItem() != null) {
                cmbValue = cmbCauseClient.GetSelectedItem().value;
            }
        } else if (ckValue_client.GetValue() == true) {
            if (cmbValueDailyCauseClient.GetSelectedItem() != null) {
                cmbValue = cmbValueDailyCauseClient.GetSelectedItem().value;
            } else if (cmbValueCustomCauseClient.GetSelectedItem() != null) {
                cmbValue = cmbValueCustomCauseClient.GetSelectedItem().value;
            }
        } else if (ckDayContainsCause_client.GetValue() == true) {
            if (cmbDayContainsCauseClient.GetSelectedItem() != null) {
                cmbValue = cmbDayContainsCauseClient.GetSelectedItem().value;
            }
        } else if (ckShift_client.GetValue() == true) {
            if (cmbShiftClient.GetSelectedItem() != null) {
                cmbValue = cmbShiftClient.GetSelectedItem().value;
            }
            if (cmbValue != "") {
                if (cmbDaysTypeClient.GetSelectedItem() != null) {
                    cmbValue = cmbDaysTypeClient.GetSelectedItem().value;
                } else {
                    cmbValue = "";
                }
            }
        } else if (ckIncidence_client.GetValue() == true) {
            if (cmbDayCauseClient.GetSelectedItem() != null) {
                cmbValue = cmbDayCauseClient.GetSelectedItem().value;
            }
            if (cmbValue != "") {
                if (cmbDaysTypeCauseClient.GetSelectedItem() != null) {
                    cmbValue = cmbDaysTypeCauseClient.GetSelectedItem().value;
                } else {
                    cmbValue = "";
                }
            }
        }

        if (cmbValue == "") {
            if (ckShift_client.GetValue() == false)
                showErrorPopup("Error.Title", "error", "Error.Description.NoCauseSelected", "", "Error.OK", "Error.OKDesc", "");
            else
                showErrorPopup("Error.Title", "error", "Error.Description.NoShiftSelected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        var oFactorValue = "";
        if (ckFixedValue_client.GetValue()) {
            oFactorValue = txtFactorCompositionClient.GetValue();
        } else if (ckFactorUserField_client.GetValue()) {
            if (cmbFactorUserFieldClient.GetSelectedItem() != null) {
                oFactorValue = cmbFactorUserFieldClient.GetSelectedItem().text;
            }
        }

        if (oFactorValue == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoFactorValue", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }        

        var oConditionCheck = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_chkButton').checked;
        if (oConditionCheck == true) {
            if (getCountCausesConditionsGrid() == 0) {
                showErrorPopup("Error.Title", "error", "Error.Description.NoConditionCauses", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        if (conceptCompositionDataClient.Get("IsNew")) { //Comprobem que no existeix al grid actual
            if (checkIfExistIDCauseInGrid(conceptCompositionDataClient.Get("IDConceptComposition")) == true) {
                showErrorPopup("Error.Title", "error", "Error.Description.InNewExistCause", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        return true;
    } catch (e) { showError("validateEditComposition", e); return false; }
}

function saveComposition() {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        if (oDis == "false") {
            cancelComposition();
            return;
        }

        if (validateEditComposition()) {
            var isNew = conceptCompositionDataClient.Get("IsNew");
            var oIDConcept = actualConcept;
            var oIDCauseType = -1;

            var oIDCause = "";
            if (ckCause_client.GetValue() == true && cmbCauseClient.GetSelectedItem() != null) {
                oIDCause = cmbCauseClient.GetSelectedItem().value;
            } else if (ckValue_client.GetValue() == true && (cmbValueDailyCauseClient.GetSelectedItem() != null || cmbValueCustomCauseClient.GetSelectedItem() != null)) {
                if (cmbValueDailyCauseClient.GetSelectedItem() != null) oIDCause = cmbValueDailyCauseClient.GetSelectedItem().value;
                else if (cmbValueCustomCauseClient.GetSelectedItem() != null) oIDCause = cmbValueCustomCauseClient.GetSelectedItem().value;
            } else if (ckDayContainsCause_client.GetValue() == true && cmbDayContainsCauseClient.GetSelectedItem() != null) {
                oIDCause = cmbDayContainsCauseClient.GetSelectedItem().value;
            }

            var oIDConceptComposition = conceptCompositionDataClient.Get("IDConceptComposition");

            var oIDShift = "";
            if (cmbShiftClient.GetSelectedItem() != null) {
                oIDShift = cmbShiftClient.GetSelectedItem().value.split("_")[0];
            }

            var oDaysType = "";

            var oIDDayCause = "";
            if (cmbDayCauseClient.GetSelectedItem() != null) {
                oIDDayCause = cmbDayCauseClient.GetSelectedItem().value;
            }
            var oIDType = 0;
            var oName = "";
            var oDescription = "";
            if (ckCause_client.GetValue() == true) {
                oIDType = "0";
                oIDCauseType = "-1";
                oName = cmbCauseClient.GetSelectedItem().text;
            } else if (ckValue_client.GetValue() == true) {
                oIDType = "0";
                oIDCauseType = "0";
                if (cmbValueDailyCauseClient.GetSelectedItem() != null) oName = cmbValueDailyCauseClient.GetSelectedItem().text;
                else if (cmbValueCustomCauseClient.GetSelectedItem() != null) oName = cmbValueCustomCauseClient.GetSelectedItem().text;
            } else if (ckDayContainsCause_client.GetValue() == true) {
                oIDType = "0";
                oIDCauseType = "1";
                oName = cmbDayContainsCauseClient.GetSelectedItem().text;
            } else if (ckShift_client.GetValue() == true) {
                oIDType = "1";
                oName = cmbShiftClient.GetSelectedItem().text;
                oIDCause = "0";
                oDescription = cmbDaysTypeClient.GetSelectedItem().text;
                if (cmbDaysTypeClient.GetSelectedItem() != null) {
                    oDaysType = cmbDaysTypeClient.GetSelectedItem().value;
                }
            } else if (ckIncidence_client.GetValue() == true) {
                oIDType = "2";
                oName = cmbDayCauseClient.GetSelectedItem().text;
                oIDCause = oIDDayCause;
                oDescription = cmbDaysTypeCauseClient.GetSelectedItem().text;
                if (cmbDaysTypeCauseClient.GetSelectedItem() != null) {
                    oDaysType = cmbDaysTypeCauseClient.GetSelectedItem().value;
                }
            }

            var oConditionCheck = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_chkButton').checked;
            var oFactorValue = txtFactorCompositionClient.GetValue();
            var oCompare = cmbCompareClient.GetSelectedItem().value;
            var oValue_Type = cmbTypeValueClient.GetSelectedItem().value;
            var oValue_IDCause = cmbCauseTypeClient.GetSelectedItem().value;
            var oValue_Direct = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_txtValueType').value;

            var oValue_UField = "";
            if (cmbCauseUFieldClient.GetSelectedItem() != null) {
                oValue_UField = cmbCauseUFieldClient.GetSelectedItem().value;
            }

            var oFactorUField = "";
            if (cmbFactorUserFieldClient.GetSelectedItem() != null) {
                oFactorUField = cmbFactorUserFieldClient.GetSelectedItem().value;
            }

            var oCausesConditions = toStringCausesConditions();

            var oComp = {
                fields: [
                    { 'field': 'IDConcept', 'value': oIDConcept, 'control': '', 'type': '' },
                    { 'field': 'IDCause', 'value': oIDCause, 'control': '', 'type': '' },
                    { 'field': 'IDCauseType', 'value': oIDCauseType, 'control': '', 'type': '' },
                    { 'field': 'IDConceptComposition', 'value': oIDConceptComposition, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'IDShift', 'value': oIDShift, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'IDType', 'value': oIDType, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'TypeDayPlanned', 'value': oDaysType, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'FactorUField', 'value': oFactorUField, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Name', 'value': oName, 'control': '', 'type': '' },
                    { 'field': 'Description', 'value': oDescription, 'control': '', 'type': '' },
                    { 'field': 'ConditionCheck', 'value': oConditionCheck, 'control': '', 'type': '' },
                    { 'field': 'FactorValue', 'value': oFactorValue, 'control': '', 'type': '' },
                    { 'field': 'Compare', 'value': oCompare, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Value_Type', 'value': oValue_Type, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Value_IDCause', 'value': oValue_IDCause, 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Value_Direct', 'value': oValue_Direct, 'control': '', 'type': '' },
                    { 'field': 'Value_UField', 'value': oValue_UField, 'control': '', 'type': '' },
                    { 'field': 'CausesConditions', 'value': oCausesConditions, 'control': '', 'type': '' }
                ]
            };

            if (isNew) {
                createGridLine(oComp.fields);
            } else {
                editGridLine(oComp.fields);
            }

            hasChanges(true);

            //Tanquem finestra
            disableScreen(false);
            showWndCompositions(false);

            setCompositionChanges(true);
        }
    } catch (e) { showError("saveComposition", e); }
}

function toStringCausesConditions() {
    try {
        var hTable = document.getElementById('htTableCondition');
        var arrStr = '';

        //Si no hi ha taula
        if (hTable == null) { return null; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRows");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRows");
                break;
            default:
                hRows = document.getElementsByName("htRows");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRows");
                }
                break;
        } //end switch

        //Bucle per les files del grid, per eliminar les files
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            arrStr = arrStr + hRow.getAttribute("IDCause") + ',' + hRow.cells[0].innerHTML + ',' + hRow.getAttribute("Operation") + '|';
        } //end for

        if (arrStr.length > 0) {
            arrStr = arrStr.substring(0, arrStr.length - 1);
        }

        return arrStr;
    } catch (e) { showError("toStringCausesConditions", e); return ''; }
}

function cancelComposition() {
    disableScreen(false);
    showWndCompositions(false)
}

function showWndCompositions(bol) {
    try {
        var divC = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_frm');
        if (divC != null) {
            if (bol == true) {
                divC.style.display = '';
                divC.style.marginLeft = ((divC.offsetWidth / 2) * -1) + "px";
                divC.style.marginTop = ((divC.offsetHeight / 2) * -1) + "px";
            } else {
                divC.style.display = 'none';
            }
        }
    } catch (e) { showError("showWndCompositions", e); }
}

function saveChangesConcept() {
    try {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            if (actualConcept > 0) {
                if (checkTypeChange()) { //si s'ha cambiat el tipus, popup missatge
                } else {
                    if (getChangesInComposition()) { //Si s'ha cambiat la composicio, missatge
                        disableScreen(true);
                        showWndRecalcConcept(true);
                    } else { //Sino, graba el acumulat
                        saveDefChanges();
                    }
                } //end if
            } else {
                saveDefChanges();
            } //end if
        } else {
            showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        };
    } catch (e) { showError("saveChangesConcept", e); }
}

function checkTypeChange() {
    try {
        //comprobem si s'ha cambiat el tipus, etc.
        var chkOpt;

        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime_rButton').checked == true) {
            chkOpt = "0";
        } else {
            if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes_rButton').checked == true) {
                chkOpt = "1";
            } else {
                chkOpt = "2";
            }
        }

        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_oldConceptType').value != chkOpt) {
            showErrorPopup("Question.RestartValues",
                "INFO",
                "Question.RestartValues.Description",
                "",
                "Question.RestartValues.OptionDelete",
                "Question.RestartValues.OptionDeleteDesc",
                "window.frames['ifPrincipal'].saveChanges1();",
                "Question.RestartValues.OptionCancel",
                "Question.RestartValues.OptionCancelDesc",
                "");
            return true;
        }

        return false;
    } catch (e) { showError("checkImportantConceptChanges", e); return "3"; }
}

function saveChanges1() {
    try {
        showWndRecalcConcept(true);
    } catch (e) { showError("saveChanges1", e); }
}

function showWndRecalcConcept(bol) {
    if (cmbShowValueClient.GetValue() == 'L') {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays_rButton').checked = true;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate_rButton').checked = false;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate_panOptionPanel').setAttribute("venabled", "False");
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate');        
    }
    else {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate_panOptionPanel').setAttribute("venabled", "True");
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate_panOptionPanel').setAttribute("venabled", "True");
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate');
    }
    var divC = document.getElementById('divMsgChg');
    if (divC != null) {
        if (bol == true) {
            divC.style.display = '';
            if (dtRecDateClient.GetDate() != null) {
            }
            //divC.style.position = "fixed";
        } else {
            divC.style.display = 'none';
        }
    }
}

function closeWndRecalConcept() {
    disableScreen(false);
    showWndRecalcConcept(false);
}

function recalcComposition() {
    try {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcByDate_rButton').checked == true) { //option bydate
            //Date en blanc, error
            if (dtRecDateClient.GetDate() == null) {
                showErrorPopup("Error.RecalcDateBlank.Title", "ERROR", "Error.RecalcDateBlank.Description", "", "Error.RecalcDateBlank.OK", "Error.RecalcDateBlank.OKDesc", "");
                return;
            }

            var date1 = dtRecDateClient.GetDate();
            var date2 = recalcConfigClient.Get("RecalcDate")

            //Date mes gran que la de inici de recalcul
            if (date1 < date2) {
                showErrorPopup("Error.RecalcDateNotCorrect.Title", "ERROR", "Error.RecalcDateNotCorrect.Description", "", "Error.RecalcDateNotCorrect.OK", "Error.RecalcDateNotCorrect.OKDesc", "");
                return;
            }
            saveDefChanges();
        } else { //option all
            dtRecDateClient.SetDate(null);
            if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optRecalcAllDays_rButton').checked == true) { //option bydate
                saveDefChanges();
            } else {
                showErrorPopup("Error.RecalcNotSelected.Title", "ERROR", "Error.RecalcNotSelected.Description", "", "Error.RecalcNotSelected.OK", "Error.RecalcNotSelected.OKDesc", "");
            }
        }
    } catch (e) { showError("recalcComposition", e); }
}

function getChangesInComposition() {
    try {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnCompositionChanges').value == "1") {
            return true;
        } else {
            return false;
        }
    } catch (e) { showError("getChangesInComposition", e); }
}

function saveDefChanges() {
    try {
        //Aqui afegim la nova carrega del grid desde javascript
        var oJSONComp = retrieveJSONCompositionGrid();
        if (finallySaveConcept(oJSONComp) == true) {
            hasChanges(false);
            closeWndRecalConcept();
        }
    } catch (e) { showError("saveDefChanges", e); }
}

function retrieveJSONCompositionGrid() {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridCompositions');
        var newArr = new Array();

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
            case 'Chrome':
                hRows = document.getElementsByName("htRowsComposition");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsComposition");
                break;
            default:
                hRows = document.getElementsByName("htRowsComposition");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsComposition");
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];

            var oComp = {
                fields: [
                    { 'field': 'IDConcept', 'value': hRow.getAttribute("idconcept"), 'control': '', 'type': '' },
                    { 'field': 'IDCause', 'value': hRow.getAttribute("idcause"), 'control': '', 'type': '' },
                    { 'field': 'IDConceptComposition', 'value': hRow.getAttribute("IDConceptComposition"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'IDShift', 'value': hRow.getAttribute("IdShift"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'IDType', 'value': hRow.getAttribute("IDType"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'TypeDayPlanned', 'value': hRow.getAttribute("TypeDayPlanned"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'FactorUField', 'value': hRow.getAttribute("FactorUField"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Name', 'value': hRow.cells[0].innerHTML, 'control': '', 'type': '' },
                    { 'field': 'Description', 'value': hRow.cells[1].innerHTML, 'control': '', 'type': '' },
                    { 'field': 'ConditionCheck', 'value': hRow.getAttribute("conditioncheck"), 'control': '', 'type': '' },
                    { 'field': 'FactorValue', 'value': hRow.getAttribute("factorvalue"), 'control': '', 'type': '' },
                    { 'field': 'Compare', 'value': hRow.getAttribute("compare"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Value_Type', 'value': hRow.getAttribute("value_type"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Value_IDCause', 'value': hRow.getAttribute("value_idcause"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Value_UField', 'value': hRow.getAttribute("value_ufield"), 'control': '', 'type': '', 'list': '' },
                    { 'field': 'Value_Direct', 'value': hRow.getAttribute("value_direct"), 'control': '', 'type': '' },
                    { 'field': 'CausesConditions', 'value': hRow.getAttribute("causesconditions"), 'control': '', 'type': '' }
                ]
            };

            newArr.push(oComp);
        }

        return newArr;
    } catch (e) { showError("retrieveJSONCompositionGrid", e); }
}

function finallySaveConcept(jsonFields) {
    try {
        var oFields = '';

        //Si s'han passat compositions, carrega
        if (jsonFields != null) {
            //Bucle per totes les composicions
            for (var x = 0; x < jsonFields.length; x++) {
                //Bucle per els camps
                for (var xF = 0; xF < jsonFields[x].fields.length; xF++) {
                    oFields += jsonFields[x].fields[xF].value
                    if (xF != jsonFields[x].fields.length) {
                        oFields += "*|*";
                    }
                }
                if (x != jsonFields.length) {
                    oFields += "*&*";
                }
            }
        }

        showLoadingGrid(true);

        var oParameters = {};
        oParameters.aTab = actualConceptTab;
        oParameters.ID = actualConcept;
        oParameters.oType = "C";
        oParameters.Name = document.getElementById("readOnlyNameConcept").textContent.trim();
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.resultClientAction = oFields;

        oParameters.AutomaticAccrualConf = JSON.stringify(automaticAccrualConf);

        oParameters.action = "SAVECONCEPT";

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);
        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);

        return true;
    }
    catch (e) {
        showLoadingGrid(false);
        showError('saveConcept', e);
        return false;
    }
}

function ShowRemoveConcept() {
    if (actualConcept < 1) { return; }

    var url = "Concepts/srvMsgBoxConcepts.aspx?action=Message";
    url = url + "&TitleKey=deleteConcept.Title";
    url = url + "&DescriptionKey=deleteConcept.Description";
    url = url + "&Option1TextKey=deleteConcept.Option1Text";
    url = url + "&Option1DescriptionKey=deleteConcept.Option1Description";
    url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteConcept('" + actualConcept + "'); return false;";
    url = url + "&Option2TextKey=deleteConcept.Option2Text";
    url = url + "&Option2DescriptionKey=deleteConcept.Option2Description";
    url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function deleteConcept(Id) {
    try {
        var stamp = '&StampParam=' + new Date().getMilliseconds();

        var ajax = nuevoAjax();
        ajax.open("GET", "Handlers/srvConcepts.ashx?action=deleteXConcept&ID=" + Id + stamp, true);

        ajax.onreadystatechange = function () {
            if (ajax.readyState == 4) {
                if (ajax.responseText == 'OK') {
                    deleteConceptSelectedNode();
                } else {
                    if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                        var url = "Concepts/srvMsgBoxConcepts.aspx?action=Message&" + ajax.responseText.substr(7, ajax.responseText.length - 7);
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                    }
                }
            }
        }

        ajax.send(null);
    } catch (e) { showError('deleteConcept', e); }
}

function deleteConceptSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesConcepts";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function newConcept() {
    var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=Concept";
    NewObjectPopup_Client.SetContentUrl(contentUrl);
    NewObjectPopup_Client.Show();
}

function checkConceptEmptyName(newName) {
    document.getElementById("readOnlyNameConcept").textContent = newName;
    hasChanges(true);
}

function ckValue_client_CheckedChanged(s, e) {
    if (s.GetValue() == true) {
        cmbCauseClient.SetEnabled(false);
        cmbValueDailyCauseClient.SetEnabled(true);
        cmbValueCustomCauseClient.SetEnabled(true);
        cmbDayContainsCauseClient.SetEnabled(false);
        cmbDaysTypeClient.SetEnabled(false);
        cmbDaysTypeCauseClient.SetEnabled(false);
    } else {
        cmbCauseClient.SetEnabled(false);
    }
}

function ckDayContainsCause_client_CheckedChanged(s, e) {
    if (s.GetValue() == true) {
        cmbCauseClient.SetEnabled(false);
        cmbValueDailyCauseClient.SetEnabled(false);
        cmbValueCustomCauseClient.SetEnabled(false);
        cmbDayContainsCauseClient.SetEnabled(true);
        cmbDaysTypeClient.SetEnabled(false);
        cmbDaysTypeCauseClient.SetEnabled(false);
    } else {
        cmbCauseClient.SetEnabled(false);
    }
}

function ckCause_client_CheckedChanged(s, e) {
    if (s.GetValue() == true) {
        cmbCauseClient.SetEnabled(true);
        cmbValueDailyCauseClient.SetEnabled(false);
        cmbValueCustomCauseClient.SetEnabled(false);
        cmbDayContainsCauseClient.SetEnabled(false);
        cmbDaysTypeClient.SetEnabled(false);
        cmbDaysTypeCauseClient.SetEnabled(false);
    } else {
        cmbCauseClient.SetEnabled(false);
    }
}

function ckShift_client_CheckedChanged(s, e) {
    if (s.GetValue() == true) {
        cmbShiftClient.SetEnabled(true);
        cmbValueDailyCauseClient.SetEnabled(false);
        cmbValueCustomCauseClient.SetEnabled(false);
        cmbDayContainsCauseClient.SetEnabled(false);
        cmbDaysTypeClient.SetEnabled(true);
        cmbDaysTypeCauseClient.SetEnabled(false);
    } else {
        cmbShiftClient.SetEnabled(false);
    }
    if (cmbShiftClient.GetSelectedItem() != null) {
        if (cmbShiftClient.GetSelectedItem().value.split('_')[1] != '2') {
            cmbDaysTypeClient.SetEnabled(false); cmbDaysTypeClient.SetValue('0');
        } else {
            cmbDaysTypeClient.SetEnabled(true);
        }
    }
}

function cmbShiftClient_SelectedIndexChanged(s, e) {
    if (s.GetSelectedItem().value.split('_')[1] != '2') {
        cmbDaysTypeClient.SetEnabled(false); cmbDaysTypeClient.SetValue('0');
    } else {
        cmbDaysTypeClient.SetEnabled(true);
    }
}

function ckIncidence_client_CheckedChanged(s, e) {
    if (s.GetValue() == true) {
        cmbDayCauseClient.SetEnabled(true);
        cmbValueDailyCauseClient.SetEnabled(false);
        cmbValueCustomCauseClient.SetEnabled(false);
        cmbDayContainsCauseClient.SetEnabled(false);
        cmbDaysTypeClient.SetEnabled(false);
        cmbDaysTypeCauseClient.SetEnabled(true);
    } else {
        cmbDayCauseClient.SetEnabled(false);
    }
}

function ckFixedValue_client_CheckedChanged(s, e) {
    if (s.GetValue() == true) {
        txtFactorCompositionClient.SetEnabled(true);
        cmbFactorUserFieldClient.SetValue('');
    } else {
        txtFactorCompositionClient.SetEnabled(false);
    }
}

function ckFactorUserField_client_CheckedChanged(s, e) {
    if (s.GetValue() == true) {
        cmbFactorUserFieldClient.SetEnabled(true);
    } else {
        cmbFactorUserFieldClient.SetEnabled(false);
    }
}

function AutomaticAccrualHoursHasChanges(bRecalculate, changeOPC, bForceChange) {
    if (!cancelOPCEvents && (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals_rButton').checked == true || bForceChange)) {
        if (changeOPC) {
            chgOPCItems('1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals', 'undefined');
        }

        if (cmbHoursAutomaticAccrual_Client.GetSelectedItem().value == 1) {
            $('#divHoursAutomaticAccrual_Userfield').show();
            $('#divHoursAutomaticAccrual_Fixed').hide();
        } else {
            $('#divHoursAutomaticAccrual_Userfield').hide();
            $('#divHoursAutomaticAccrual_Fixed').show();
        };

        if (bRecalculate) {
            automaticAccrualConf.accrualautomaticType = 2;
            hasChanges(true);
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnCompositionChanges').value = "1";
        }
    }
}

function AutomaticAccrualDayHasChanges(bRecalculate, changeOPC, bForceChange) {
    if (!cancelOPCEvents && (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_rButton').checked == true || bForceChange)) {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes_rButton').checked == true || document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom_rButton').checked == true) {
            if (changeOPC) {
                chgOPCItems('2', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals', 'undefined');
            }

            if (cmbDaysAutomaticAccrual_Client.GetSelectedItem().value == 1) {
                $('#divDaysAutomaticAccrual_Userfield').show();
                $('#divDaysAutomaticAccrual_Fixed').hide();
            } else {
                $('#divDaysAutomaticAccrual_Userfield').hide();
                $('#divDaysAutomaticAccrual_Fixed').show();
            };

            if (bRecalculate) {
                automaticAccrualConf.accrualautomaticType = 1;
                hasChanges(true);
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnCompositionChanges').value = "1";
            }
        }
        //else {
        //    chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals', 'undefined');
        //    showErrorPopup("Error.AutomaticAccrualType.Title", "ERROR", "Error.AutomaticAccrualType.Description", "", "Error.AutomaticAccrualType.OK", "Error.AutomaticAccrualType.OKDesc", "");
        //}
    }
}

function NoAutomaticAccrualHasChanges(bRecalculate, bForceChange) {
    if (!cancelOPCEvents && (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals_rButton').checked == true || bForceChange)) {
        if (bRecalculate) {
            automaticAccrualConf.accrualautomaticType = 0;
            hasChanges(true);
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnCompositionChanges').value = "1";
        }
    }
}

function initializeAutomaticAccrualsData(clientData) {
    automaticAccrualConf = clientData;

    cancelOPCEvents = true;
    //venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals');
    //venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals');
    //venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');
    //linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');
    cancelOPCEvents = false;

    switch (clientData.accrualautomaticType) {
        case 0:
            //chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals,ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals', 'undefined');
            NoAutomaticAccrualHasChanges(false, false, false);
            break;
        case 1:
            AutomaticAccrualDayHasChanges(false, false, false);
            break;
        case 2:
            AutomaticAccrualHoursHasChanges(false, false, false);
            break;
    }

    $("#lstCausesHoursAutomaticAccrual").dxTagBox({
        dataSource: new DevExpress.data.ArrayStore({
            data: clientData.causes,
            key: "ID"
        }),
        displayExpr: "Name",
        valueExpr: "ID",
        showSelectionControls: true,
        applyValueMode: "useButtons",
        disabled: cmbShowValueClient.GetValue() == 'L',
        readOnly: !clientData.clientEnabled,
        value: clientData.selectedHourCauses,
        onValueChanged: function (e) {
            automaticAccrualConf.selectedHourCauses = e.value;
            AutomaticAccrualHoursHasChanges(true, true, true);
            hasChanges(true);
        }
    });

    $("#lstCausesDaysAutomaticAccrual").dxTagBox({
        dataSource: new DevExpress.data.ArrayStore({
            data: clientData.causes,
            key: "ID"
        }),
        displayExpr: "Name",
        valueExpr: "ID",
        disabled: cmbShowValueClient.GetValue() == 'L',
        showSelectionControls: true,
        applyValueMode: "useButtons",
        readOnly: !clientData.clientEnabled,
        value: clientData.selectedDayCauses,
        onValueChanged: function (e) {
            automaticAccrualConf.selectedDayCauses = e.value;
            AutomaticAccrualDayHasChanges(true, true, true);
            hasChanges(true);
        }
    });

    $("#lstShiftsDaysAutomaticAccrual").dxTagBox({
        dataSource: new DevExpress.data.ArrayStore({
            data: clientData.shifts,
            key: "ID"
        }),
        displayExpr: "Name",
        valueExpr: "ID",
        disabled: cmbShowValueClient.GetValue() == 'L',
        showSelectionControls: true,
        applyValueMode: "useButtons",
        readOnly: !clientData.clientEnabled,
        value: clientData.selectedDayShifts,
        onValueChanged: function (e) {
            automaticAccrualConf.selectedDayShifts = e.value;
            AutomaticAccrualDayHasChanges(true, true, true);
            hasChanges(true);
        }
    });
}

function accrualTypeChanged(newValue) {    
    var conceptDefaultQuery = $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnConceptPeriodType').val();
    if (newValue == 'C') {
        $('#divAccrualExpiration').show();
    } else {
        $('#divAccrualExpiration').hide();
    }
    accrualExpirationHasChanges();
    if (newValue == 'L') {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime_rButton').checked = false;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom_rButton').checked = false;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes_rButton').checked = true;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime_panOptionPanel').setAttribute("venabled", "False");
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom_panOptionPanel').setAttribute("venabled", "False");
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes_panOptionPanel').setAttribute("venabled", "True");
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals_rButton').checked = true;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals_rButton').checked = false;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_rButton').checked = false;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_ckDaysAutomaticAccrualAllDays_S').checked = false;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_ckDaysAutomaticAccrualOnlyDays_S').checked = false;        
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals_panOptionPanel').setAttribute("venabled", "True");
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals_panOptionPanel').setAttribute("venabled", "False");
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals_panOptionPanel').setAttribute("venabled", "False");        
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoAutomaticAccruals');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursAutomaticAccruals');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysAutomaticAccruals');
        cmbHoursAutomaticAccrual_Client.enabled = false;
        cmbDaysAutomaticAccrual_Client.enabled = false;
        cmbAutomaticAccrualCause_Client.enabled = false;
        ckDaysAutomaticAccrualAllDays_client.enabled = false;
        if (typeof ckDaysAutomaticAccrualAllDays_client != 'undefined')
            ckDaysAutomaticAccrualAllDays_client.checked = false;
        ckDaysAutomaticAccrualOnlyDays_client.enabled = false;
        if (typeof ckDaysAutomaticAccrualOnlyDays_client != 'undefined')
            ckDaysAutomaticAccrualOnlyDays_client.checked = false;
        try {
            $("#lstCausesHoursAutomaticAccrual").dxTagBox("instance").option("disabled", true)
            $("#lstCausesDaysAutomaticAccrual").dxTagBox("instance").option("disabled", true)
            $("#lstShiftsDaysAutomaticAccrual").dxTagBox("instance").option("disabled", true)
        }
        catch { }
        if (conceptDefaultQuery != 'L') {            
            cmbShowValueClient.enabled = true;
        }
        else
            cmbShowValueClient.enabled = false;
    }
    else {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime_panOptionPanel').setAttribute("venabled", "True");
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom_panOptionPanel').setAttribute("venabled", "True");
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes_panOptionPanel').setAttribute("venabled", "True");        
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTime');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptCustom');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opConceptTimes');                
    }
}

function accrualExpirationHasChanges() {
    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAccrualExpiration_chkButton').checked == true) {
        $('#divAccrualExpirationContent').show();
    } else {
        $('#divAccrualExpirationContent').hide();
    }

    if (!cancelOPCEvents) document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnCompositionChanges').value = "1";
}

function PeriodHasChanges() {
    if (!cancelOPCEvents) document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnCompositionChanges').value = "1";
}