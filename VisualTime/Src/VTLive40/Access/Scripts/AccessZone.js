var actualTab = 0;
var actualAccessZone;
var actualAccessZoneMain;

var isRoot = true;

var jsGridExceptions; //Grid Exceptions
var jsGridPeriods; //Grid Periods

var newObjectName = "";

function checkAccessZoneEmptyName(newName) {
    document.getElementById("readOnlyNameAccessZone").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    showLoadingGrid(false);

    ConvertControls('divContent');
    ConvertControls('divAccessZone');

    checkResult(s);

    switch (s.cpAction) {
        case "GETACCESSZONEMAIN":
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divEmpresa').style.display = '';
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divZone').style.display = 'none';
            hasChanges(false);
            SetPosition();
            break;

        case "GETACCESSZONE":
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divEmpresa').style.display = 'none';
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divZone').style.display = '';
            changeTabs(actualTab);
            hasChanges(false);
            GetAccessZone_AFTER(s);

            if (s.cpIsNew == true) {
                refreshTree();
            }
            break;

        case "SAVEACCESSZONE":
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divEmpresa').style.display = 'none';
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divZone').style.display = '';
            changeTabs(actualTab);
            if (s.cpResult == 'OK') {
                hasChanges(false);
                refreshTree();
            }
            break;

        default:
            hasChanges(false);
    }
}

function GetAccessZone_AFTER(s) {
    try {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divEmpresa').style.display = 'none';
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divZone').style.display = '';

        if (s.cpNameRO != null && s.cpNameRO != "") {
            document.getElementById("readOnlyNameAccessZone").textContent = s.cpNameRO;
            hasChanges(false);
            ASPxClientEdit.ValidateGroup(null, true);
        } else {
            document.getElementById("readOnlyNameAccessZone").textContent = newObjectName;
            hasChanges(true);
            txtName_Client.SetValue(newObjectName);
        }

        //Esborrem els grids
        if (jsGridExceptions != null) { jsGridExceptions.destroyGrid(); }
        if (jsGridPeriods != null) { jsGridPeriods.destroyGrid(); }

        //Cargar Grids
        if (s.cpGridsJSON != null && s.cpGridsJSON.length != 0) {
            var objGrids = JSON.parse(s.cpGridsJSON);
            createGridExceptions(objGrids[0].exceptions);
            createGridPeriods(objGrids[1].periods);
        }

        if (getAccessZonesPositions() != "") {
            var oParmArray = new Array();
            oParmArray = getAccessZonesPositions().split(",");
            var strColor = oParmArray[5];
            if (!strColor.startsWith("#")) {
                var color = new RGBColor(strColor);
                var colorHTML;
                if (color.ok) { // 'ok' is true when the parsing was a success
                    colorHTML = color.toHex();
                } else {
                    colorHTML = oParmArray[5];
                }
                var strAux = oParmArray[0] + "," + oParmArray[1] + "," + oParmArray[2] + "," + oParmArray[3] + "," + oParmArray[4] + "," + colorHTML.substring(1);
                hdnManagePlaneClient.Set('Position', strAux);
            }
            if (document.getElementById('divLocationMap').innerHTML != "") {
                loadLocationMapFlash(getAccessZonesImageID(), getAccessZonesPositions());
            } else {
                loadLocationMapFlash(getAccessZonesImageID(), getAccessZonesPositions());
            }
        } else {
            if (document.getElementById('divLocationMap').innerHTML != "") {
                loadLocationMapFlash(getAccessZonesImageID(), '');
            } else {
                loadLocationMapFlash(getAccessZonesImageID(), '');
            }
        }

        top.focus();
    }
    catch (e) {
        showError("GetAccessZone_AFTER", e);
    }
}

function SetPosition() {
    try {
        if (getAccessZonesPositionsMain() != "") {
            var oParmArray = new Array();
            oParmArray = getAccessZonesPositionsMain().split(",");
            var strPositions = "";

            if (oParmArray.length > 9) {
                var oCnt = Math.round(oParmArray.length / 9);
                for (var xy = 0; xy < oCnt; xy++) {
                    var oIdx = xy * 9;
                    var strColor = oParmArray[oIdx + 6];
                    if (!strColor.startsWith("#")) {
                        var color = new RGBColor(strColor);
                        var colorHTML;

                        if (color.ok) { // 'ok' is true when the parsing was a success
                            colorHTML = color.toHex();
                        } else {
                            colorHTML = oParmArray[oIdx + 6];
                        } //end if

                        strPositions += oParmArray[oIdx] + "," + oParmArray[oIdx + 1] + "," + oParmArray[oIdx + 2] + "," + oParmArray[oIdx + 3] + "," + oParmArray[oIdx + 4] + "," + oParmArray[oIdx + 5] + "," + colorHTML.substring(1) + "," + oParmArray[oIdx + 7] + "," + oParmArray[oIdx + 8] + ",";
                    } //end if
                } //end for

                strPositions = strPositions.substring(0, strPositions.length - 1);
                hdnManagePlaneClient.Set('PositionMain', strPositions);
            }
            else {
                var strColor = oParmArray[8];
                if (!strColor.startsWith("#")) {
                    var color = new RGBColor(strColor);
                    var colorHTML;
                    if (color.ok) { // 'ok' is true when the parsing was a success
                        colorHTML = color.toHex();
                    } else {
                        colorHTML = oParmArray[6];
                    } //end if
                    var strAux = oParmArray[0] + "," + oParmArray[1] + "," + oParmArray[2] + "," + oParmArray[3] + "," + oParmArray[4] + "," + oParmArray[5] + "," + colorHTML.substring(1) + "," + oParmArray[7] + "," + oParmArray[8];
                    hdnManagePlaneClient.Set('PositionMain', strAux);
                }
            }

            if (document.getElementById('divLocationMap').innerHTML != "") {
                setTimeout("LocationMap_DoFSCommand('REPOSITION', '" + getAccessZonesPositionsMain() + "');", 1500);
            }
            else {
                loadLocationGlobalMapFlash(getAccessZonesImageIDMain(), getAccessZonesPositionsMain());
            }
        }
        else {
            if (document.getElementById('divLocationMap').innerHTML != "") {
                setTimeout("LocationMap_DoFSCommand('RESET', '');", 1500);
            }
            else {
                loadLocationGlobalMapFlash(getAccessZonesImageIDMain(), '');
            }
        }

        top.focus();
    }
    catch (e) {
        showError("SetPosition", e);
    }
}

function saveChanges() {
    if (ASPxClientEdit.ValidateGroup(null, true)) {
        grabarAccessZone(actualAccessZone);
    } else {
        showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
    };
}

function undoChanges() {
    try {
        if (actualAccessZone == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesAccessZones";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaAccessZone(actualAccessZone);
        }
    } catch (e) { showError("undoChanges", e); }
}

function cargaNodo(Nodo) {
    try {
        if (Nodo.id == "source") {
            newAccessZone();
        } else {
            var oID = Nodo.id.substring(1);
            if (Nodo.id.substring(0, 1) == "A") {
                isRoot = true;
                cargaAccessZoneMain(oID);
            }
            else {
                isRoot = false;
                cargaAccessZone(oID);
            }
        }
    } catch (e) { showError("cargaNodo", e); }
}

function cargaAccessZone(IdAccessZone) {
    actualAccessZone = IdAccessZone;
    showLoadingGrid(true);
    cargaAccessZoneTabSuperior(IdAccessZone);
}

function cargaAccessZoneMain(IdAccessZoneMain) {
    actualAccessZoneMain = IdAccessZoneMain;
    showLoadingGrid(true);
    cargaAccessZoneTabSuperiorMain(IdAccessZoneMain);
}

function newAccessZone() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=AccessZone";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newAccessGroup', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaAccessZone(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

function cargaAccessZoneTabSuperior(IDAccessZone) {
    try {
        var Url = "Handlers/srvAccessZones.ashx?action=getAccessZoneTab&aTab=" + actualTab + "&ID=" + IDAccessZone;
        AsyncCall("POST", Url, "CONTAINER", "divAccessZone", "cargaAccessZoneBarButtons(" + IDAccessZone + ");");
    }
    catch (e) {
        showError("cargaAccessZoneTabSuperior", e);
    }
}
var responseObj = null;
function cargaAccessZoneBarButtons(IDAccessZone) {
    try {
        var Url = "Handlers/srvAccessZones.ashx?action=getBarButtons&ID=" + IDAccessZone;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDAccessZone + ")");
    }
    catch (e) {
        showError("cargaAccessZoneTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IDAccessZone) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaAccessZoneDivs(IDAccessZone);
}

function cargaAccessZoneTabSuperiorMain(IDAccessZoneMain) {
    try {
        var Url = "Handlers/srvAccessZones.ashx?action=getAccessZoneMainTab&ID=" + IDAccessZoneMain;
        AsyncCall("POST", Url, "CONTAINER", "divAccessZone", "cargaAccessZoneBarButtonsMain(" + IDAccessZoneMain + ");");
    }
    catch (e) {
        showError("cargaAccessZoneTabSuperior", e);
    }
}
var responseObjZone = null;
function cargaAccessZoneBarButtonsMain(IDAccessZoneMain) {
    try {
        var Url = "Handlers/srvAccessZones.ashx?action=getBarButtonsMain&ID=" + IDAccessZoneMain;
        AsyncCall("POST", Url, "JSON3", responseObjZone, "parseResponseBarButtonsZone(objContainerId," + IDAccessZoneMain + ")");
    }
    catch (e) {
        showError("cargaAccessZoneTabSuperior", e);
    }
}

function parseResponseBarButtonsZone(oResponse, IDAccessZoneMain) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaAccessZoneDivsMain(IDAccessZoneMain);
}

function grabarAccessZone(IdAccessZone) {
    showLoadingGrid(true);

    //Carreguem els arrays
    if (jsGridExceptions != null) { arrExceptions = jsGridExceptions.toJSONStructure(); }
    if (jsGridPeriods != null) { arrPeriods = jsGridPeriods.toJSONStructure(); }

    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAccessZone;
    oParameters.Name = document.getElementById("readOnlyNameAccessZone").innerHTML.trim();
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEACCESSZONE";

    var oFields = "";
    if (arrExceptions != null) {
        for (var x = 0; x < arrExceptions.length; x++) {
            oFields += encodeURIComponent(arrExceptions[x][0].value) + "#" + encodeURIComponent(arrExceptions[x][1].value) + ";";
        }
        oFields = oFields.substring(0, oFields.length - 1);
    }
    oParameters.gridExceptions = oFields;

    oFields = "";
    if (arrPeriods != null) {
        for (var x = 0; x < arrPeriods.length; x++) {
            oFields += encodeURIComponent(arrPeriods[x][0].value) + "#" + encodeURIComponent(arrPeriods[x][1].value) + "#" +
                encodeURIComponent(arrPeriods[x][2].value) + "#" + encodeURIComponent(arrPeriods[x][3].value) + "#" +
                encodeURIComponent(arrPeriods[x][4].value) + ";";
        }
        oFields = oFields.substring(0, oFields.length - 1);
    }
    oParameters.gridPeriods = oFields;

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaAccessZoneDivs(IdAccessZone) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAccessZone;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETACCESSZONE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaAccessZoneDivsMain(IdAccessZoneMain) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAccessZoneMain;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETACCESSZONEMAIN";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div01');

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
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div01');

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

function hasChanges(bolChanges) {
    if (isRoot && actualAccessZone != -1) return;

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

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesAccessZones";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesAccessZones";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function ShowRemoveAccessZone() {
    try {
        if (actualAccessZone == "-1") { return; }

        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deleteAccessZone.Title";
        url = url + "&DescriptionKey=deleteAccessZone.Description";
        url = url + "&Option1TextKey=deleteAccessZone.Option1Text";
        url = url + "&Option1DescriptionKey=deleteAccessZone.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteAccessZone('" + actualAccessZone + "'); return false;";
        url = url + "&Option2TextKey=deleteAccessZone.Option2Text";
        url = url + "&Option2DescriptionKey=deleteAccessZone.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveAccessZone", e); }
}

function deleteAccessZone(oId) {
    try {
        AsyncCall("POST", "Handlers/srvAccessZones.ashx?action=deleteAccessZone&ID=" + oId, "json", "arrStatus", "checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
    }
    catch (e) { showError('deleteAccessZone', e); }
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

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
            if (isRoot) {
                //Si es passa zona, redireccionem
                if (oParms != "") {
                    ShowZone(oParms);
                }
            }
            else {
                setAccessZonesPositions(oParms);
                LocationMap_DoFSCommand("REPOSITION", oParms);
                hasChanges(true);
            }
        }
        else if (DataType == "2") {
            refreshCombos();
        }
        else if (DataType == "6") {
            refreshTree();
        }
    } catch (e) { showError("RefreshScreen", e); }
}

function refreshCombos() {
    try {
        if (isRoot == true) {
            var strParameters = "SELECTEDVALUE=-1";
            if (cmbZonePlaneMainClient.GetSelectedIndex() != -1) {
                strParameters = "SELECTEDVALUE=" + cmbZonePlaneMainClient.GetSelectedItem().value;
            }
            cmbZonePlaneMainClient.PerformCallback(strParameters);
        }
        else {
            var strParameters = "SELECTEDVALUE=-1";
            if (cmbZonePlaneClient.GetSelectedIndex() != -1) {
                strParameters = "SELECTEDVALUE=" + cmbZonePlaneClient.GetSelectedItem().value;
            }
            cmbZonePlaneClient.PerformCallback(strParameters);
        }
    }
    catch (e) { showErrorPopup("refreshCombos", e); }
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
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
        if (oResult.cpAction == "SAVEACCESSZONE") {
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
            } else {
            }
            hasChanges(true);
        }
    } catch (e) { showError("checkStatus", e); }
}

function ShowNewAccessZonesWizard() {
    try {
        var Title = '';
        top.ShowExternalForm2('Access/Wizards/NewAccessZoneWizard.aspx', 500, 450, Title, '', false, false, false);
    } catch (e) { showErrorPopup("ShowNewAccessZonesWizard", e); }
}

function ShowChangeZoneImage() {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true") { return; }
        var Title = '';
        var oID;

        if (isRoot == true) {
            oID = actualAccessZoneMain;
        }
        else {
            oID = actualAccessZone;
        }
        top.ShowExternalForm2('Access/ChangeZoneImage.aspx?ID=' + oID, 350, 150, Title, '', false, false);
    }
    catch (e) { showError("ShowChangeZoneImage", e); }
}

function showMsg(oMsg) {
    alert(oMsg);
}

function ShowZoomZone(FixPoints) {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true" && FixPoints == "true") { return; }
        var Title = '';
        var ImageID = '';
        var Global = '0';
        var oID;

        if (isRoot == true) {
            oID = actualAccessZoneMain;
            strParams = getAccessZonesPositionsMain();
            Global = '1';
            ImageID = getAccessZonesImageIDMain();
        }
        else {
            oID = actualAccessZone;
            ImageID = getAccessZonesImageID();
            strParams = LocationMap_DoFSCommand("GETPARMS", "true");
            Global = '0';
        }

        var w = 900;
        var h = 800;

        if (window.addEventListener) { // Mozilla, etc.
            w = document.body.clientWidth - 50;
            h = document.body.clientHeight;
        }
        else { //IE
            //w = document.body.scrollWidth - 50;
            //h = document.body.scrollHeight;
        }

        top.ShowExternalForm2('Access/AccessZoomZone.aspx?ID=' + oID + "&Global=" + Global + "&strParams=" + encodeURIComponent(strParams) + "&ImageID=" + ImageID + "&FixPoints=" + FixPoints, w, h, Title, '', false, false);
    } catch (e) { showError("ShowZoomZone", e); }
}

function editGridPeriods(idRow) {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true") { return; }
        var arrValues = new Array();
        arrValues = jsGridExceptions.retRowJSON(idRow);

        document.getElementById('hdnAddPeriodIDRow').value = idRow;

        frmAddPeriod_Show(arrValues);
    } catch (e) { showError("editGridPeriods", e); }
}

function updateAddPeriodRow(rowID, arrValues) {
    try {
        if (jsGridPeriods == null) { createGridPeriods(); }
        if (rowID == "") {
            jsGridPeriods.createRow(arrValues, true);
        } else {
            jsGridPeriods.editRow(rowID, arrValues);
        }
    } catch (e) { showError("updateAddPeriodRow", e); }
}

function updateAddExceptionRow(rowID, arrValues) {
    try {
        if (jsGridExceptions == null) { createGridExceptions(); }
        if (rowID == "") {
            jsGridExceptions.createRow(arrValues, true);
        } else {
            jsGridExceptions.editRow(rowID, arrValues);
        }
    } catch (e) { showError("updateAddExceptionRow", e); }
}

function deleteGridPeriods(idRow) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deletePeriodsDef.Title";
        url = url + "&DescriptionKey=deletePeriodsDef.Description";
        url = url + "&Option1TextKey=deletePeriodsDef.Option1Text";
        url = url + "&Option1DescriptionKey=deletePeriodsDef.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelPeriod('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deletePeriodDef.Option2Text";
        url = url + "&Option2DescriptionKey=deletePeriodDef.Option2Description";
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

function editGridExceptions(idRow) {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true") { return; }
        var arrValues = new Array();
        arrValues = jsGridPeriods.retRowJSON(idRow);

        document.getElementById('hdnAddExceptionIDRow').value = idRow;

        frmAddException_Show(arrValues);
    } catch (e) { showError("editGridExceptions", e); }
}

function deleteGridExceptions(idRow) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deleteExceptionsDef.Title";
        url = url + "&DescriptionKey=deleteExceptionsDef.Description";
        url = url + "&Option1TextKey=deleteExceptionsDef.Option1Text";
        url = url + "&Option1DescriptionKey=deleteExceptionsDef.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelException('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deleteExceptionDef.Option2Text";
        url = url + "&Option2DescriptionKey=deleteExceptionDef.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteGridExceptions", e); }
}

function delSelException(idRow) {
    try {
        jsGridExceptions.deleteRow(idRow);
        hasChanges(true);
    } catch (e) { showError("delSelException", e); }
}

function AddNewZonesInactivity() {
    try {
        document.getElementById('hdnAddPeriodIDRow').value = "";
        frmAddPeriod_ShowNew();
    } catch (e) { showError("AddNewZonesInactivity", e); }
}

function AddNewZonesExceptions() {
    try {
        document.getElementById('hdnAddExceptionIDRow').value = "";
        frmAddException_ShowNew();
    } catch (e) { showError("AddNewZonesExceptions", e); }
}

function createGridExceptions(arrGridExceptions) {
    try {
        var hdGridException = [{ 'fieldname': 'ExceptionDate', 'description': '', 'size': 'auto' }];

        hdGridException[0].description = document.getElementById('hdnLngDateException').value;

        var edtRow = false;
        var delRow = false;

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "true") {
            edtRow = true;
            delRow = true;
        }

        jsGridExceptions = new jsGrid('ctl00_contentMainBody_ASPxCallbackPanelContenido_grdExceptions', hdGridException, arrGridExceptions, edtRow, delRow, false, 'Exceptions');
    } catch (e) { showError("createGridExceptions", e); }
}

function createGridPeriods(arrGridPeriods) {
    try {
        var hdGridPeriod = [{ 'fieldname': 'WeekDayName', 'description': '', 'size': 'auto' },
        { 'fieldname': 'Begin', 'description': '', 'size': '50px' },
        { 'fieldname': 'End', 'description': '', 'size': '50px' }];

        hdGridPeriod[0].description = document.getElementById('hdnLngWeekdayName').value;
        hdGridPeriod[1].description = document.getElementById('hdnLngDateBegin').value;
        hdGridPeriod[2].description = document.getElementById('hdnLngDateEnd').value;

        var edtRow = false;
        var delRow = false;

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "true") {
            edtRow = true;
            delRow = true;
        }

        jsGridPeriods = new jsGrid('ctl00_contentMainBody_ASPxCallbackPanelContenido_grdPeriods', hdGridPeriod, arrGridPeriods, edtRow, delRow, false, 'Periods');
    } catch (e) { showError("createGridPeriods", e); }
}

function reloadflBg(oID) {
    LocationMap_DoFSCommand('RELOADBG', oID);
    if (isRoot == true) {
        setAccessZonesImageIDMain(oID);
        var strParameters = "SELECTEDPLANE=" + oID;
        CallbackHelperClient.PerformCallback(strParameters);
    }
    else {
        setAccessZonesImageID(oID);
    }
}

function CallbackHelperClient_CallbackComplete(s, e) {
    if (e.result != "") {
        var param = e.result.split("@");
        setAccessZonesPositionsMain(param[0]);
        setAccessZonesImageIDMain(param[1]);
        SetPosition();
    }
}

function ShowZonePlanes() {
    parent.ShowExternalForm2('Access/Wizards/PlanesWizard.aspx', 900, 600, '', '', false, false);
}

function getAccessZonesImageID() {
    try {
        if (typeof (hdnManagePlaneClient.Get('ImageID')) == 'undefined')
            return "";
        else
            return hdnManagePlaneClient.Get('ImageID');
    }
    catch (e) {
        showError('getAccessZonesImageID', e);
        return "";
    }
}

function getAccessZonesPositions() {
    try {
        if (typeof (hdnManagePlaneClient.Get('Position')) == 'undefined')
            return "";
        else
            return hdnManagePlaneClient.Get('Position');
    }
    catch (e) {
        showError('getAccessZonesPositions', e);
        return "";
    }
}

function setAccessZonesPositions(oValue) {
    try {
        return hdnManagePlaneClient.Set('Position', oValue);
    }
    catch (e) {
        showError('setAccessZonesPositions', e);
    }
}

function setAccessZonesImageID(oValue) {
    try {
        hdnManagePlaneClient.Set('ImageID', oValue);
    }
    catch (e) {
        showError('setAccessZonesImageID', e);
    }
}

function getAccessZonesImageIDMain() {
    try {
        if (typeof (hdnManagePlaneClient.Get('ImageIDMain')) == 'undefined')
            return "";
        else
            return hdnManagePlaneClient.Get('ImageIDMain');
    }
    catch (e) {
        showError('getAccessZonesImageIDMain', e);
        return "";
    }
}

function getAccessZonesPositionsMain() {
    try {
        if (typeof (hdnManagePlaneClient.Get('PositionMain')) == 'undefined')
            return "";
        else
            return hdnManagePlaneClient.Get('PositionMain');
    }
    catch (e) {
        showError('getAccessZonesPositionsMain', e);
        return "";
    }
}

function setAccessZonesPositionsMain(oValue) {
    try {
        hdnManagePlaneClient.Set('PositionMain', oValue);
    }
    catch (e) {
        showError('setAccessZonesPositionsMain', e);
    }
}

function setAccessZonesImageIDMain(oValue) {
    try {
        hdnManagePlaneClient.Set('ImageIDMain', oValue);
    }
    catch (e) {
        showError('setAccessZonesImageIDMain', e);
    }
}