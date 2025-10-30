function checkConceptGroupEmptyName(newName) {
    document.getElementById("readOnlyNameConceptGroup").textContent = newName;
    hasChanges(true);
}

function cargaConceptGroupNodo(Nodo) {
    try {
        if (actualType == "G") {
            if (Nodo.id.toUpperCase() == "SOURCE") {
                newConceptGroup();
                return;
            }
            actualConceptGroup = Nodo.id;
            cargaConceptGroup(Nodo.id);
        } else {
            if (Nodo.id.toUpperCase() == "SOURCE") actualConceptGroup = -1
            else actualConceptGroup = Nodo.id;
        }
    } catch (e) { showError("cargaGroupNodo", e); }
}

//Carga Tabs y contenido Empleados
function cargaConceptGroup(IDConceptGroup) {
    try {
        actualConceptGroup = IDConceptGroup;
        actualType = "G";
        //TAB Gris Superior
        showLoadingGrid(true);
        cargaConceptGroupTabSuperior(IDConceptGroup);
    } catch (e) { showError("cargaConceptGroup", e); }
}

// Carrega la part del TAB grisa superior
function cargaConceptGroupTabSuperior(IDConceptGroup) {
    try {
        var parms = "";

        parms = { "action": "getConceptGroupTab", "ID": IDConceptGroup };
        AjaxCall("POST", "json", "Handlers/srvConceptGroups.ashx", parms, "CONTAINER", "divConcepts", "cargaConceptGroupBarButtons(" + IDConceptGroup + ")");
    }
    catch (e) {
        showError("cargaConceptTabSuperior", e);
    }
}
var responseObjGroups = null;
function cargaConceptGroupBarButtons(IDConceptGroup) {
    try {
        var Url = "";

        Url = "Handlers/srvConceptGroups.ashx?action=getBarButtons&ID=" + IDConceptGroup;
        AsyncCall("POST", Url, "JSON3", responseObjGroups, "parseResponseBarButtonsGroups(objContainerId," + IDConceptGroup + ")");
    }
    catch (e) {
        showError("cargaConceptTabSuperior", e);
    }
}

function parseResponseBarButtonsGroups(oResponse, IDConceptGroup) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaConceptGroupDivs(IDConceptGroup);
}

// Carrega els apartats dels divs de l'usuari
function cargaConceptGroupDivs(IDConceptGroup) {
    var oParameters = {};
    oParameters.aTab = actualConceptGroupTab;
    oParameters.ID = actualConceptGroup;
    oParameters.oType = "G";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETCONCEPTGROUP";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Cambia els Tabs i els divs
function changeTabsGroups(numTab) {
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
    actualConceptTab = numTab;
}

//Cambia els Tabs i els divs (per nom)
function changeTabsByNameGroups(nameTab) {
    arrButtons = new Array('TABBUTTON_00');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div20');

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

function hasChangesConceptGroups(bolChanges) {
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

function undoChangesConceptGroups() {
    try {
        if (actualConceptGroup == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesConceptGroups";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaConceptGroupDivs(actualConceptGroup);
        }
    } catch (e) { showError("undoChanges", e); }
}

function AddNewConceptGroup() {
    try {
        var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;

        disableScreen(true);
        showWndConceptGroupsItem(true);
    } catch (e) { showError("AddNewConceptGroupItem", e); }
}

function cancelConceptGroupsItem() {
    disableScreen(false);
    showWndConceptGroupsItem(false)
}

function showWndConceptGroupsItem(bol) {
    try {
        var divC = document.getElementById('divConceptGroupsItems');
        if (divC != null) {
            if (bol == true) {
                divC.style.display = '';
                divC.style.marginLeft = ((divC.offsetWidth / 2) * -1) + "px";
                divC.style.marginTop = ((divC.offsetHeight / 2) * -1) + "px";   //- 160;
            } else {
                divC.style.display = 'none';
            }
        }
    } catch (e) { showError("showWndConceptGroupsItem", e); }
}

function saveConceptGroupsItem() {
    try {
        if (validateEditConceptGroupsItem()) {
            isNew = true;

            var oIDConcept = cmbConceptsClient.GetValue();
            var oNameConcept = cmbConceptsClient.GetText();

            var oConceptItem = {
                fields: [
                    { 'field': 'IDConcept', 'value': oIDConcept, 'control': '', 'type': '' },
                    { 'field': 'NameConcept', 'value': oNameConcept, 'control': '', 'type': '' }
                ]
            };

            if (isNew) {
                createGridLineConceptGroupItems(oConceptItem.fields);
            }

            hasChanges(true);

            //Tanquem finestra
            disableScreen(false);
            showWndConceptGroupsItem(false);

            setConceptGroupsItemChanges(true);
        }
    } catch (e) { showError("saveConceptGroupsItem", e); }
}

function RemoveListConceptGroupsItems(objId) {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridConceptGroups');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsConceptGroupsItem");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsConceptGroupsItem");
                break;
            default:
                hRows = document.getElementsByName("htRowsConceptGroupsItem");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsConceptGroupsItem");
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
    } catch (e) { showError("RemoveListConceptGroupsItems", e); }
}

function ReeStyleListConceptGroupsItem() {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridConceptGroups');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsConceptGroupsItem");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsConceptGroupsItem");
                break;
            default:
                hRows = document.getElementsByName("htRowsConceptGroupsItem");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsConceptGroupsItem");
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
            /*hRow.id = "htRowC_" + nCID;
            hRow.setAttribute("idtr","htRowC_" + nCID);
            nCID += 1;*/

            hRow.cells[0].className = 'GridStyle-cell' + altRow;
            hRow.cells[1].className = 'GridStyle-cell' + altRow;
        }

        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("ReeStyleListConceptGroupsItem", e); }
}

function checkIfExistIDConceptInGrid(IDConcept) {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridConceptGroups');

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsConceptGroupsItem");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsConceptGroupsItem");
                break;
            default:
                hRows = document.getElementsByName("htRowsConceptGroupsItem");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsConceptGroupsItem");
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];
            if (hRow.getAttribute("idconcept") == IDConcept) {
                return true;
            }
        }

        return false;
        //oComposition.deleteCauseCondition(IDCause);
    } catch (e) { showError("checkIfExistIDConceptInGrid", e); }
}

function setConceptGroupsItemChanges(bol) {
    try {
        if (bol) {
            document.getElementById('hdnConceptGroupsChanges').value = "1";
        } else {
            document.getElementById('hdnConceptGroupsChanges').value = "0";
        }
    } catch (e) { showError("setConceptGroupsItemChanges", e); }
}

function getChangesInConceptGroupsItem() {
    try {
        if (document.getElementById('hdnCompositionChanges').value == "1") {
            return true;
        } else {
            return false;
        }
    } catch (e) { showError("getChangesInConceptGroupsItem", e); }
}

function moveRow(tableID, cell, direction) {
    mTable = document.getElementById(tableID);
    rowIndex = parseInt(cell.parentNode.rowIndex);

    if ((rowIndex == 1 && direction > 0) || (rowIndex == mTable.rows.length - 1 && direction < 0) || (rowIndex > 0 && rowIndex < mTable.rows.length - 1)) {
        if (mTable.rows.length > 2) {
            if ((rowIndex + direction) > 0) {
                var row = mTable.rows[rowIndex];

                var cell0 = row.cells[0].cloneNode(true);
                var cell1 = row.cells[1].cloneNode(true);
                var cell2 = row.cells[2].cloneNode(true);

                cell1.childNodes[0].onclick = mTable.rows[rowIndex].cells[1].childNodes[0].onclick
                cell2.childNodes[0].onclick = mTable.rows[rowIndex].cells[2].childNodes[0].onclick
                cell2.childNodes[1].onclick = mTable.rows[rowIndex].cells[2].childNodes[1].onclick

                mTable.deleteRow(rowIndex);
                var oRow = mTable.insertRow(rowIndex + direction);

                for (var x = 0; x < row.attributes.length; x++) {
                    oRow.setAttribute(row.attributes[x].nodeName, row.attributes[x].nodeValue);
                }

                oRow.id = row.id;

                oRow.appendChild(cell0);
                oRow.appendChild(cell1);
                oRow.appendChild(cell2);

                ReeStyleListConceptGroupsItem();
                hasChanges(true);
            }
        }
    }
}

function createGridConceptGroupsItems(arrCC) {
    try {
        for (var n = 0; n < arrCC.length; n++) {
            createGridLineConceptGroupItems(arrCC[n].concepts);
        }
    }
    catch (e) {
        showError('createGridConceptGroupsItems', e);
    }
    finally {
        showLoadingGrid(false);
    }
}

function createGridLineConceptGroupItems(arrFields) {
    try {
        //Carreguem el array global per mantenir els valors
        var n;

        var oTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridConceptGroups');

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
        oTR.setAttribute("name", "htRowsConceptGroupsItem");

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

            switch (fieldName) {
                case "IDCONCEPT":
                    oTR.setAttribute("idconcept", value);
                    break;
                case "NAMECONCEPT":
                    var oTD = oTR.insertCell(-1); //Name
                    oTD.className = "GridStyle-cell" + altRow;
                    oTD.textContent = value;
                    oTD.width = "100%";
                    oTD.setAttribute("idTr", "htRowC_" + oNewId);
                    oTR.setAttribute("idTr", "htRowC_" + oNewId);
                    if (window.addEventListener) { // Mozilla, Netscape, Firefox
                        oTD.setAttribute("onmouseover", "javascript: rowOver('htRowC_" + oNewId + "');");
                        oTD.setAttribute("onmouseout", "javascript: rowOut('htRowC_" + oNewId + "');");
                        //oTD.setAttribute("onclick", "javascript: editComposition('htRowC_" + oNewId + "');");
                    } else { // IE
                        oTR.onmouseover = function () { rowOver(this.getAttribute("idTr")); }
                        oTR.onmouseout = function () { rowOut(this.getAttribute("idTr")); }

                        //oTD.onclick = function() { editComposition(this.getAttribute("idTr")); }
                    }
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
            //oTD.style.borderRight = "solid 1px silver";

            //var aEdit = document.createElement("A");
            var aDelete = document.createElement("A");

            //aEdit.href = "javascript: void(0);";
            aDelete.href = "javascript: void(0);";
            //aEdit.title = document.getElementById("tagEditTitle").value;
            aDelete.title = document.getElementById("tagRemoveTitle").value;

            //aEdit.setAttribute("row", "htRowC_" + oNewId);
            aDelete.setAttribute("row", "htRowC_" + oNewId);

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                //aEdit.setAttribute("onclick", "javascript: editComposition('" + oTR.id + "');");
                aDelete.setAttribute("onclick", "javascript: deleteConceptGroupItem('" + oTR.id + "');");
            } else { // IE
                //aEdit.onclick = function() { editComposition(this.getAttribute("row")); }
                aDelete.onclick = function () { deleteConceptGroupItem(this.getAttribute("row")); }
            }

            var imgEdit = document.createElement("IMG");
            var imgDelete = document.createElement("IMG");
            //imgEdit.src = "Base/Images/Grid/edit.png";
            imgDelete.src = hBaseRef + "Images/Grid/remove.png";

            //aEdit.appendChild(imgEdit);
            aDelete.appendChild(imgDelete);
            //oTD.appendChild(aEdit);
            oTD.appendChild(aDelete);
            oTD.style.backgroundColor = "#E8EEF7";

            //*-------------------------------------------------------

            //Creem la barra d'eines al TR
            var oTD2 = oTR.insertCell(-1); //Name
            oTD2.className = "GridStyle-cellheader";
            //oTD.style.borderRight = "solid 1px silver";
            oTD2.width = "50px";
            oTD2.style.whiteSpace = "nowrap";
            oTD2.style.borderRight = "solid 1px silver";

            //var aEdit = document.createElement("A");
            var aMUp = document.createElement("A");
            var aMDown = document.createElement("A");

            //aEdit.href = "javascript: void(0);";
            aMUp.href = "javascript: void(0);";
            aMDown.href = "javascript: void(0);";

            if (window.addEventListener) { // Mozilla, Netscape, Firefox
                //aEdit.setAttribute("onclick", "javascript: editComposition('" + oTR.id + "');");
                aMUp.setAttribute("onclick", "javascript: moveRow('" + oTable.id + "',this.parentNode,-1);");
                aMDown.setAttribute("onclick", "javascript: moveRow('" + oTable.id + "',this.parentNode,1);");
            } else { // IE
                //aEdit.onclick = function() { editComposition(this.getAttribute("row")); }
                aMUp.onclick = function () { moveRow(oTable.id, this.parentNode, -1); }
                aMDown.onclick = function () { moveRow(oTable.id, this.parentNode, 1); }
            }

            var imgUP = document.createElement("IMG");
            var imgDown = document.createElement("IMG");
            imgUP.src = hBaseRef + "Images/Grid/uparrow.png";
            imgDown.src = hBaseRef + "Images/Grid/downarrow.png";

            aMUp.appendChild(imgUP);
            aMDown.appendChild(imgDown);

            oTD2.appendChild(aMUp);
            oTD2.appendChild(aMDown);
            oTD2.style.backgroundColor = "#E8EEF7";
        } else {
            var imgAdd = document.getElementById('frmCompositions1_optChkCondition_imgAddListValue');
            if (imgAdd != null) { imgAdd.onclick = ""; }
            var imgRemove = document.getElementById('frmCompositions1_optChkCondition_imgRemoveListValue');
            if (imgRemove != null) { imgRemove.onclick = ""; }
        }
        return true;
    } catch (e) {
        showError("createGridLineConceptGroupItems", e);
        return false;
    } //end try
}

function deleteConceptGroupItem(objId) {
    try {
        //var IDConcept = oConceptGroup.getConceptGroupID();

        var oRow = document.getElementById(objId);
        if (oRow == null) { return; }

        var IDConcept = oRow.getAttribute("idconcept");

        var url = "Concepts/srvMsgBoxConcepts.aspx?action=Message&IDConcept=" + IDConcept;
        url = url + "&TitleKey=deleteConceptGroupGrid.Denied.Title";
        url = url + "&DescriptionKey=deleteConceptGroupGrid.DeleteRow.Description";
        url = url + "&Option1TextKey=deleteConceptGroupGrid.DeleteRow.Option1Text";
        url = url + "&Option1DescriptionKey=deleteConceptGroupGrid.DeleteRow.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteConceptGroupItemRow('" + objId + "'); return false;";
        url = url + "&Option2TextKey=deleteConceptGroupGrid.DeleteRow.Option2Text";
        url = url + "&Option2DescriptionKey=deleteConceptGroupGrid.DeleteRow.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteConceptGroupItem", e); }
}

function deleteConceptGroupItemRow(objId) {
    setConceptGroupsItemChanges(true);
    RemoveListConceptGroupsItems(objId);
    ReeStyleListConceptGroupsItem();
    hasChanges(true);
}

function validateEditConceptGroupsItem() {
    try {
        //Comprobem si esta el idcause com read-only (edicio) o es combobox
        if (cmbConceptsClient.GetValue() == null || cmbConceptsClient.GetValue() == "-1" || cmbConceptsClient.GetValue() == "") {
            showErrorPopup("Error.Title", "error", "Error.Description.NoConceptSelected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        isNew = true;

        if (isNew) { //Comprobem que no existeix al grid actual
            var oIDConcept = cmbConceptsClient.GetValue();
            if (checkIfExistIDConceptInGrid(oIDConcept) == true) {
                showErrorPopup("Error.Title", "error", "Error.Description.InNewExistConcept", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        return true;
    } catch (e) { showError("validateEditConceptGroupsItem", e); return false; }
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

function newConceptGroup() {
    var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=ConceptGroup";
    NewObjectPopup_Client.SetContentUrl(contentUrl);
    NewObjectPopup_Client.Show();
}

function ShowRemoveConceptGroup() {
    if (actualConceptGroup == -1) { return; }

    var url = "Concepts/srvMsgBoxConcepts.aspx?action=Message";
    url = url + "&TitleKey=deleteConceptGroup.Title";
    url = url + "&DescriptionKey=deleteConceptGroup.Description";
    url = url + "&Option1TextKey=deleteConceptGroup.Option1Text";
    url = url + "&Option1DescriptionKey=deleteConceptGroup.Option1Description";
    url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteConceptGroup(" + actualConceptGroup + "); return false;";
    url = url + "&Option2TextKey=deleteConceptGroup.Option2Text";
    url = url + "&Option2DescriptionKey=deleteConceptGroup.Option2Description";
    url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function deleteConceptGroup(Id) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvConceptGroups.ashx?action=deleteXConceptGroup&ID=" + Id + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            if (ajax.responseText == 'OK') {
                deleteSelectedNodeConceptGroups();
            } else {
                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                    var url = "Concepts/srvMsgBoxConcepts.aspx?action=Message&" + ajax.responseText.substr(7, ajax.responseText.length - 7);
                    parent.ShowMsgBoxForm(url, 500, 300, '');
                }
            }
        }
    }

    ajax.send(null);
}

function refreshTreeConceptGroups() {
    eval('ctl00_contentMainBody_roTreesConceptGroups_roTrees.LoadTreeViews(true, true, true);');
}

function deleteSelectedNodeConceptGroups() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesConceptGroups";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function saveChangesConceptGroups() {
    try {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            showLoadingGrid(true);
            var oJSONComp = retrieveJSONConceptGroupsItemGrid();

            var oParameters = {};
            oParameters.aTab = actualConceptGroupTab;
            oParameters.ID = actualConceptGroup;
            oParameters.oType = "G";
            oParameters.Name = document.getElementById("readOnlyNameConceptGroup").textContent.trim();
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVECONCEPTGROUP";

            var oFields = "";
            if (oJSONComp != null && oJSONComp.length > 0) {
                //Bucle per totes les composicions
                for (var x = 0; x < oJSONComp.length; x++) {
                    //Bucle per els camps
                    for (var xF = 0; xF < oJSONComp[x].fields.length; xF++) {
                        var strParam = "CONCEPT_" + x + "_" + oJSONComp[x].fields[xF].field + "=" + encodeURIComponent(oJSONComp[x].fields[xF].value);
                        oFields += strParam + "&";
                    }
                }
                oFields = oFields.substring(0, oFields.length - 1);
            }
            oParameters.resultClientAction = oFields;

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        };
    } catch (e) { showError("saveChangesConceptGroup", e); }
}

function retrieveJSONConceptGroupsItemGrid() {
    try {
        var hTable = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tblGridConceptGroups');
        var newArr = new Array();

        //Si no hi ha taula
        if (hTable == null) { return; }

        var hRows;

        switch (BrowserDetect.browser) {
            case 'Firefox':
            case 'Safari':
                hRows = document.getElementsByName("htRowsConceptGroupsItem");
                break;
            case 'Explorer':
                hRows = getElementsByName_iefix("TR", "htRowsConceptGroupsItem");
                break;
            default:
                hRows = document.getElementsByName("htRowsConceptGroupsItem");
                if (hRows.length == 0) {
                    hRows = getElementsByName_iefix("TR", "htRowsConceptGroupsItem");
                }
                break;
        }

        //Bucle per les files del grid, per cambiar els estils
        for (var n = 0; n < hRows.length; n++) {
            var hRow = hRows[n];

            var oComp = {
                fields: [
                    { 'field': 'IDConcept', 'value': hRow.getAttribute("idconcept"), 'control': '', 'type': '' },
                    { 'field': 'NameConcept', 'value': hRow.cells[0].textContent, 'control': '', 'type': '' }
                ]
            };

            newArr.push(oComp);
        }

        return newArr;
    } catch (e) { showError("retrieveJSONConceptGroupsItemGrid", e); }
}