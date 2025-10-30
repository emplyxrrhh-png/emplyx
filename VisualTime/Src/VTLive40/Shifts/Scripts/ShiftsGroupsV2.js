function checkShiftGroupEmptyName(newName) {
    document.getElementById("readOnlyNameShiftGroup").textContent = newName;
    hasChanges(true);
}

//Carga Tabs y contenido Empleados
function cargaShiftGroup(IDShiftGroup) {
    try {
        actualShiftGroup = IDShiftGroup;
        actualType = "G";
        //TAB Gris Superior
        showLoadingGrid(true);
        cargaShiftGroupTabSuperior(IDShiftGroup);
    } catch (e) { showError("cargaConceptGroup", e); }
}

// Carrega la part del TAB grisa superior
function cargaShiftGroupTabSuperior(IDShiftGroup) {
    try {
        var parms = "";

        parms = { "action": "getShiftGroupsTab", "ID": IDShiftGroup, "aTab": actualShiftGroupTab };
        AjaxCall("POST", "json", "Handlers/srvShiftsGroups.ashx", parms, "CONTAINER", "divShifts", "cargaShiftGroupBarButtons(" + IDShiftGroup + ")");
    }
    catch (e) {
        showError("cargaConceptTabSuperior", e);
    }
}
var responseObjGroups = null;
function cargaShiftGroupBarButtons(IDShiftGroup) {
    try {
        var Url = "";

        Url = "Handlers/srvShiftsGroups.ashx?action=getBarButtons&ID=" + IDShiftGroup;
        AsyncCall("POST", Url, "JSON3", responseObjGroups, "parseResponseBarButtonsGroups(objContainerId," + IDShiftGroup + ")");
    }
    catch (e) {
        showError("cargaConceptTabSuperior", e);
    }
}
function parseResponseBarButtonsGroups(oResponse, IDShiftGroup) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargasShiftGroupDivs(IDShiftGroup);
}

// Carrega els apartats dels divs de l'usuari
function cargasShiftGroupDivs(IDShiftGroup) {
    var oParameters = {};
    oParameters.aTab = actualShiftGroupTab;
    oParameters.ID = IDShiftGroup;
    oParameters.oType = "G";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETSHIFTGROUP";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Cambia els Tabs i els divs
function changeTabsShiftGroups(numTab) {
    arrButtons = new Array('TABBUTTON_00');
    arrDivs = new Array('div20');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
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
    actualShiftTab = numTab;
}

//Cambia els Tabs i els divs (per nom)
function changeTabsByNameShiftGroups(nameTab) {
    arrButtons = new Array('TABBUTTON_00');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div20');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (tab != null && div != null) {
            if (div.id == nameTab) {
                tab.className = 'bTab-active';
                div.style.display = '';
                actualShiftTab = n;
            } else {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }
}

function hasChangesShiftGroups(bolChanges) {
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
}

function undoChangesShiftGroups() {
    try {
        if (actualShiftGroup == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesShifts";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargasShiftGroupDivs(actualShiftGroup);
        }
    } catch (e) { showError("undoChangesConcepts", e); }
}

//Mostra el MsgBoxForm per confirmar eliminacio de l'usuari
function ShowRemoveShiftGroup(ID) {
    if (ID == "-1") { return; }

    var url = "Shifts/srvMsgBoxShifts.aspx?action=Message";
    url = url + "&TitleKey=deleteShiftGroup.Title";
    url = url + "&DescriptionKey=deleteShiftGroup.Description";
    url = url + "&Option1TextKey=deleteShiftGroup.Option1Text";
    url = url + "&Option1DescriptionKey=deleteShiftGroup.Option1Description";
    url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].removeShiftGroup('" + ID + "'); return false;";
    url = url + "&Option2TextKey=deleteShiftGroup.Option2Text";
    url = url + "&Option2DescriptionKey=deleteShiftGroup.Option2Description";
    url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function removeShiftGroup(ID) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();
    showLoadingGrid(true);
    ajax = nuevoAjax();
    ajax.open("POST", "Handlers/srvShiftsGroups.ashx?action=deleteShiftGroup&ID=" + ID + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            var strResponse = ajax.responseText;
            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var url = "Shifts/srvMsgBoxShifts.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.substr(7, strResponse.length - 7));
                parent.ShowMsgBoxForm(url, 500, 300, '');
            } else {
                var ctlPrefix = "ctl00_contentMainBody_roTreesShifts";
                eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
            }
            showLoadingGrid(false);
        }
    }

    ajax.send(null);
}

function saveChangesShiftGroups() {
    if (ASPxClientEdit.ValidateGroup(null, false)) {
        showLoadingGrid(true);

        var oParameters = {};
        oParameters.aTab = actualShiftGroupTab;
        oParameters.ID = actualShiftGroup;
        oParameters.oType = "G";
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SAVESHIFTGROUP";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    } else {
        showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
    };
}

//Graba el nom de l'usuari
function saveNameShiftGroup(ID) {
    var txtName = document.getElementById('txtNameShiftGroup').value;

    var stamp = '&StampParam=' + new Date().getMilliseconds();

    ajax = nuevoAjax();
    ajax.open("POST", "Handlers/srvShiftsGroups.ashx?action=chgNameShiftGroup&ID=" + ID + "&NewName=" + encodeURIComponent(txtName) + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            if (ajax.responseText == 'OK') {
                document.getElementById('readOnlyNameShiftGroup').textContent = txtName;
                EditNameShiftGroup('false');

                var ctlPrefix = "ctl00_contentMainBody_roTreesShifts";
                eval(ctlPrefix + "_roTrees.RenameSelectedNode(txtName);");
            } else {
                var url = "Shifts/srvMsgBoxShifts.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.substr(7, strResponse.length - 7));
                parent.ShowMsgBoxForm(url, 500, 300, '');
            }
        }
    }
    ajax.send(null);
}

//Carga empleado y Tabs desde la pantalla de grupos (reposiciona el arbol)
function cargaHorario2(IDShift) {
    //Reposiciona l'arbre principal
    var ctlPrefix = "ctl00_contentMainBody_roTreesShifts";
    eval(ctlPrefix + "_roTrees.GetSelectedPath('1', 'B" + IDShift + "', false,'" + ctlPrefix + "',true);");
}