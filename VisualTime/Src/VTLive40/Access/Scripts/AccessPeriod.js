var actualTab = 0;
var actualAccessPeriod;

var jsGridPeriods; //Grid

var newObjectName = "";

function checkAccessPeriodEmptyName(newName) {
    document.getElementById("readOnlyNameAccessPeriod").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    try {
        changeTabs(actualTab);
        showLoadingGrid(false);

        ConvertControls('divContent');
        ConvertControls('divAccessPeriod');

        checkResult(s);

        switch (s.cpAction) {
            case "GETACCESSPERIOD":
                hasChanges(false);
                if (s.cpIsNew == true) {
                    refreshTree();
                } else {
                    GetAccessPeriod_AFTER(s);
                }
                break;

            case "SAVEACCESSPERIOD":
                if (s.cpResult == 'OK') {
                    hasChanges(false);
                    refreshTree();
                }
                break;

            default:
                hasChanges(false);
        }
    } catch (e) {
        showLoadingGrid(false);
    }
}

function GetAccessPeriod_AFTER(s) {
    try {
        if (s.cpNameRO != null && s.cpNameRO != "") {
            document.getElementById("readOnlyNameAccessPeriod").textContent = s.cpNameRO;
            hasChanges(false);
            ASPxClientEdit.ValidateGroup(null, true);
        } else {
            document.getElementById("readOnlyNameAccessPeriod").textContent = newObjectName;
            hasChanges(true);
            txtName_Client.SetValue(newObjectName);
        }

        if (jsGridPeriods != null) { jsGridPeriods.destroyGrid(); }

        //Cargar Grids
        if (s.cpGridsJSON != null && s.cpGridsJSON.length != 0) {
            var objGrids = JSON.parse(s.cpGridsJSON);
            createGridPeriods(objGrids[0].grid);
        }
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true")
            document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_btnAddAccessPeriods").parentNode.parentNode.style.display = "none";        
        else
            document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_btnAddAccessPeriods").parentNode.parentNode.style.display = "block";
    }
    catch (e) {
        showError("GetAccessPeriod_AFTER", e);
    }
}

function saveChanges() {
    if (ASPxClientEdit.ValidateGroup(null, true)) {
        grabarAccessPeriod(actualAccessPeriod);
    } else {
        showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
    };
}

function undoChanges() {
    try {
        if (actualAccessPeriod == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesAccessPeriods";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaAccessPeriod(actualAccessPeriod);
        }
    } catch (e) { showError("undoChanges", e); }
}

function cargaNodo(Nodo) {
    if (Nodo.id == "source") newAccessPeriod();
    else cargaAccessPeriod(Nodo.id);
}

function cargaAccessPeriod(IdAccessPeriod) {
    actualAccessPeriod = IdAccessPeriod;
    showLoadingGrid(true);
    cargaAccessPeriodTabSuperior(IdAccessPeriod);
}

function newAccessPeriod() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=AccessPeriod";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newAccessGroup', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaAccessPeriod(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

function cargaAccessPeriodTabSuperior(IDAccessPeriod) {
    try {
        var Url = "Handlers/srvAccessPeriods.ashx?action=getAccessPeriodTab&aTab=" + actualTab + "&ID=" + IDAccessPeriod;
        AsyncCall("POST", Url, "CONTAINER", "divAccessPeriod", "cargaAccessPeriodBarButtons(" + IDAccessPeriod + ");");
    }
    catch (e) {
        showError("cargaAccessPeriodTabSuperior", e);
    }
}
var responseObjPeriod = null;
function cargaAccessPeriodBarButtons(IDAccessPeriod) {
    try {
        var Url = "Handlers/srvAccessPeriods.ashx?action=getBarButtons&ID=" + IDAccessPeriod;
        AsyncCall("POST", Url, "JSON3", responseObjPeriod, "parseResponseBarButtonsPeriod(objContainerId," + IDAccessPeriod + ")");
    } catch (e) {
        showError("cargaAccessPeriodTabSuperior", e);
    }
}

function parseResponseBarButtonsPeriod(oResponse, IDAccessPeriod) {
    let container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaAccessPeriodDivs(IDAccessPeriod);
}

function grabarAccessPeriod(IdAccessPeriod) {
    showLoadingGrid(true);

    if (jsGridPeriods != null) { arrPeriods = jsGridPeriods.toJSONStructure(); }

    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAccessPeriod;
    oParameters.Name = document.getElementById("readOnlyNameAccessPeriod").innerHTML.trim();
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEACCESSPERIOD";

    var oFields = "";
    if (arrPeriods != null) {
        for (var x = 0; x < arrPeriods.length; x++) {
            for (var y = 0; y < arrPeriods[x].length; y++) {
                oFields += encodeURIComponent(arrPeriods[x][y].value) + "#";
            }
            oFields = oFields.substring(0, oFields.length - 1) + ";";
        }
        oFields = oFields.substring(0, oFields.length - 1);
    }
    oParameters.gridPeriods = oFields;

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaAccessPeriodDivs(IdAccessPeriod) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAccessPeriod;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETACCESSPERIOD";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (n == numTab) {
            tab.className = 'bTab-active';
            div.style.display = '';
        } else {
            tab.className = 'bTab';
            div.style.display = 'none';
        }
    }
    actualTab = numTab;
}

function changeTabsByName(nameTab) {
    arrButtons = new Array('TABBUTTON_00');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (div.id == nameTab) {
            tab.className = 'bTab-active';
            div.style.display = '';
            actualTab = n;
        } else {
            tab.className = 'bTab';
            div.style.display = 'none';
        }
    }
}

function hasChanges(bolChanges, markRecalc) {
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
    } catch (e) { showError("hasChanges", e); }
}

function setMessage(msg) {
    try {
        let msgTop = document.getElementById('msgTop');
        let msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function ShowRemoveAccessPeriod() {
    try {
        if (actualAccessPeriod == "-1") { return; }

        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deleteAccessPeriod.Title";
        url = url + "&DescriptionKey=deleteAccessPeriod.Description";
        url = url + "&Option1TextKey=deleteAccessPeriod.Option1Text";
        url = url + "&Option1DescriptionKey=deleteAccessPeriod.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteAccessPeriod('" + actualAccessPeriod + "'); return false;";
        url = url + "&Option2TextKey=deleteAccessPeriod.Option2Text";
        url = url + "&Option2DescriptionKey=deleteAccessPeriod.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveAccessPeriod", e); }
}

function deleteAccessPeriod(oId) {
    try {
        AsyncCall("POST", "Handlers/srvAccessPeriods.ashx?action=deleteAccessPeriod&ID=" + oId, "json", "arrStatus", "checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
    } catch (e) { showError('deleteAccessPeriod', e); }
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesAccessPeriods";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesAccessPeriods";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function RefreshScreen(DataType, oParms) {
    try {
        /*        if (DataType == "1") {
                    if (oAccessPeriods != null) {
                        oAccessPeriods.setAccessPeriodsPositions(oParms);
                    }
                    hasChanges(true);
                }*/
    } catch (e) { showError("RefreshScreen", e); }
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        url = url + "&DescriptionKey=" + Description;
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

function checkResult(oResult) {
    if (oResult.cpResult == 'NOK') {
        if (oResult.cpAction == "SAVEACCESSPERIOD") {
            hasChanges(true);
        }

        var url = "Access/srvMsgBoxAccess.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
            "DescriptionText=" + oResult.cpMessage + "&" +
            "Option1TextKey=SaveName.Error.Option1Text&" +
            "Option1DescriptionKey=SaveName.Error.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
}

function checkStatus(oStatus) {
    try {
        arrStatus = oStatus;
        objError = arrStatus[0];

        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Access/srvMsgBoxAccess.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
                showLoadingGrid(false);
            } else {
            }
            hasChanges(true);
        }
    } catch (e) { showError("checkStatus", e); }
}

function ShowNewAccessPeriodsWizard() {
    try {
        var Title = '';
        top.ShowExternalForm2('Access/Wizards/NewAccessPeriodWizard.aspx', 500, 450, Title, '', false, false, false);
    } catch (e) { showErrorPopup("ShowNewAccessPeriodsWizard", e); }
}

//== GRIDS =========================

createGridPeriods = function (arrGridPeriods) {
    try {
        var hdGridPeriod = [{ 'fieldname': 'Description', 'description': '', 'size': '100%' }];

        hdGridPeriod[0].description = document.getElementById('hdnStrDescription').value;

        var edtRow = false;
        var delRow = false;

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "true") {
            edtRow = true;
            delRow = true;
        }

        jsGridPeriods = new jsGrid('ctl00_contentMainBody_ASPxCallbackPanelContenido_grdPeriods', hdGridPeriod, arrGridPeriods, edtRow, delRow, false, 'Periods');
        showLoadingGrid(false);
    } catch (e) { showError("createGridPeriods", e); }
}

function AddNewGridPeriod() {
    try {
        document.getElementById('hdnAddPeriodIDRow').value = "";
        frmNewPeriod_ShowNew();
    } catch (e) { showError("AddNewGridPeriod", e); }
}

//updateAddPeriodRow : Actualitza el registre al grid (afegeix, modifica)
function updateAddPeriodRow(rowID, arrValues) {
    try {
        if (jsGridPeriods == null) { createGridPeriods(); }
        if (rowID == "") {
            rowID = jsGridPeriods.createRow(arrValues, true);
        } else {
            jsGridPeriods.editRow(rowID, arrValues);
        }
        getAccessPeriodDescription(rowID, arrValues);
    } catch (e) { showError("updateAddPeriodRow", e); }
}

function getAccessPeriodDescription(rowID, objArr) {
    try {
        var strParams = "";
        for (n = 0; n < objArr.length; n++) {
            fieldName = objArr[n].attname.toUpperCase();
            value = objArr[n].value;
            strParams += fieldName + "=" + encodeURIComponent(value) + "&";
        } //end for
        strParams = strParams.substring(0, strParams.length - 1);

        AsyncCall("POST", "Handlers/srvAccessPeriods.ashx?action=getAccessPeriodDescription&" + strParams, "json", "arrDesc", "setAccessPeriodDescription('" + rowID + "', arrDesc);");
    } catch (e) { showError('getAccessPeriodDescription', e); }
}

function setAccessPeriodDescription(rowID, objArr) {
    try {
        var oAtts = [{ 'attname': 'description', 'value': objArr[0].value }];

        if (jsGridPeriods == null) { createGridPeriods(); }
        if (rowID == "") {
            rowID = jsGridPeriods.createRow(oAtts, true);
        } else {
            jsGridPeriods.editRow(rowID, oAtts);
        }
    } catch (e) { showError("setAccessPeriodDescription", e); }
}

//editGridPeriods : Edicio dels periodes
function editGridPeriods(idRow) {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true") { return; }
        var arrValues = new Array();
        arrValues = jsGridPeriods.retRowJSON(idRow);

        document.getElementById('hdnAddPeriodIDRow').value = idRow;

        frmNewPeriod_Show(arrValues);
    } catch (e) { showError("editGridPeriods", e); }
}

function deleteGridPeriods(idRow) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deletePeriodsDef.Title";
        url = url + "&DescriptionKey=deleteAccPeriodsDef.Description";
        url = url + "&Option1TextKey=deleteAccPeriodsDef.Option1Text";
        url = url + "&Option1DescriptionKey=deleteAccPeriodsDef.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelPeriod('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deleteAccPeriodDef.Option2Text";
        url = url + "&Option2DescriptionKey=deleteAccPeriodDef.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteGridPeriods", e); }
}

function delSelPeriod(idRow) {
    try {
        jsGridPeriods.deleteRow(idRow);
        hasChanges(true);
    } catch (e) { showError("delSelPeriod", e); }
}