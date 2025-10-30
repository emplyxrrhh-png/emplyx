var actualTab = 0;
var actualAccessStatus;
var actualAccessStatusMain;
var actualNodo = 0;
var isRoot = true;

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    showLoadingGrid(false);

    ConvertControls('divContent');
    ConvertControls('divAccessStatus');

    checkResult(s);

    switch (s.cpAction) {
        case "GETACCESSSTATUS":
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divEmpresa').style.display = 'none';
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divStatus').style.display = '';
            top.focus();
            break;

        case "GETACCESSSTATUSMAIN":
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divEmpresa').style.display = '';
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divStatus').style.display = 'none';
            SetPosition();
            top.focus();
            break;

        default:
            hasChanges(false);
    }
}

function cargaNodo(Nodo) {
    try {
        var oID = Nodo.id.substring(1);
        if (Nodo.id.substring(0, 1) == "A") {
            isRoot = true;
            cargaAccessStatusMain(oID);
            actualNodo = 0;
        }
        else {
            actualNodo = oID;
            isRoot = false;
            cargaAccessStatus(oID);
        }
    } catch (e) { showError("cargaNodo", e); }
}

function cargaAccessStatus(IdAccessStatus) {
    actualAccessStatus = IdAccessStatus;
    showLoadingGrid(true);
    cargaAccessStatusTabSuperior(IdAccessStatus);
}

function cargaAccessStatusMain(IdAccessStatusMain) {
    actualAccessStatusMain = IdAccessStatusMain;
    actualAccessStatus = 0;
    showLoadingGrid(true);
    cargaAccessStatusTabSuperiorMain(IdAccessStatusMain);
}
var responseObj = null;
function cargaAccessStatusTabSuperior(IDAccessStatus) {
    try {
        var Url = "";

        Url = "Handlers/srvAccessStatus.ashx?action=getAccessStatusTab&aTab=" + actualTab + "&ID=" + IDAccessStatus;
        AsyncCall("POST", Url, "CONTAINER", "divAccessStatus", "");

        Url = "Handlers/srvAccessStatus.ashx?action=getBarButtons&ID=" + IDAccessStatus;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDAccessStatus + ")");
    }
    catch (e) {
        showError("cargaAccessStatusTabSuperior", e);
    }
}
var responseObjStatu = null;
function cargaAccessStatusTabSuperiorMain(IDAccessStatusMain) {
    try {
        var Url = "";

        Url = "Handlers/srvAccessStatus.ashx?action=getAccessStatusMainTab&ID=" + IDAccessStatusMain;
        AsyncCall("POST", Url, "CONTAINER", "divAccessStatus", "");

        Url = "Handlers/srvAccessStatus.ashx?action=getBarButtonsMain&ID=" + IDAccessStatusMain;
        AsyncCall("POST", Url, "JSON3", responseObjStatu, "parseResponseBarButtonsMainStatu(objContainerId," + IDAccessStatusMain + ")");
    }
    catch (e) {
        showError("cargaAccessStatusTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IdAccessStatus) {
    let container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaAccessStatusDivs(IdAccessStatus);
}

function parseResponseBarButtonsMainStatu(oResponse, IdAccessStatusMain) {
    let container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaAccessStatusDivsMain(IdAccessStatusMain);
}

function cargaAccessStatusDivs(IdAccessStatus) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAccessStatus;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETACCESSSTATUS";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaAccessStatusDivsMain(IdAccessStatusMain) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAccessStatusMain;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETACCESSSTATUSMAIN";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Mostra el ToolTip a la barra d'eines
function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

//Amaga el ToolTip a la barra d'eines
function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

//Refresh de les pantalles (RETORN)
function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
            //Si es passa zona, redireccionem
            if (oParms != "") {
                ShowZone(oParms);
            }
        }
    }
    catch (e) { showError("RefreshScreen", e); }
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function setMessage(msg) {
    try {
        let msgTop = document.getElementById('msgTop');
        let msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    }
    catch (e) { alert('setMessage: ' + e); }
}

function setStyleMessage(classMsg) {
    try {
        //divContainers styles
        var divTop = document.getElementById('divMsgTop');
        var divBottom = document.getElementById('divMsgBottom');

        divTop.className = classMsg;
        divBottom.className = classMsg;
    }
    catch (e) { alert('setStyleMessage: ' + e); }
}

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesAccessStatus";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
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

function showLoadingGrid(loading) { parent.showLoader(loading); }

function ShowNewAccessStatusWizard() {
    try {
        var Title = '';
        top.ShowExternalForm2('Access/Wizards/NewAccessZoneWizard.aspx', 500, 450, Title, '', false, false, false);
    }
    catch (e) { showErrorPopup("ShowNewAccessStatusWizard", e); }
}

function ShowZoomStatus() {
    try {
        var Title = '';
        var ImageID = getAccessZonesImageIDMain();
        var oID = actualAccessStatusMain;
        strParams = getAccessZonesPositionsMain();

        var w = document.body.clientWidth - 50;
        var h = document.body.clientHeight;

        top.ShowExternalForm2('Access/ZoomStatus.aspx?ID=' + oID + "&strParams=" + encodeURIComponent(strParams) + "&ImageID=" + ImageID, w, h, Title, '', false, false);
    }
    catch (e) { showError("ShowZoomStatus", e); }
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

//Redireccionem a la zona seleccionada (flash)
function ShowZone(IdSubZone) {
    try {
        //Reposiciona l'arbre principal
        var ctlPrefix = "ctl00_contentMainBody_roTreesAccessStatus";
        eval(ctlPrefix + "_roTrees.GetSelectedPath('1', 'B' + IdSubZone, false,'" + ctlPrefix + "',true);");
    }
    catch (e) { showError("ShowZone", e); }
}

function ReturnToParent() {
    try {
        //Reposiciona l'arbre principal
        var ctlPrefix = "ctl00_contentMainBody_roTreesAccessStatus";
        eval(ctlPrefix + "_roTrees.GetSelectedPath('1', 'A1', false,'" + ctlPrefix + "',true);");
    }
    catch (e) { showError("ReturnToParent", e); }
}

//Filtre d'accesos (desde usuari)
function showAccessFilter(IDEmployee, IDZone) {
    try {
        var Title = '';
        //var IDZone = oAccessStatus.getAccessStatusID();
        top.ShowExternalForm2('Access/AccessFilterPunches.aspx?IDEmployee=' + IDEmployee + '&IDZone=' + IDZone, 750, 495, Title, '', false, false);
    }
    catch (e) { showError("ShowAccessFilter", e); }
}

//Filtre d'accesos (desde usuari)
function ShowAccFilter(IDZone, IDEmployee) {
    try {
        var strIDZone = "ALL";
        var strIDEmp = "ALL";
        var Title = '';
        if (IDZone != null) { strIDZone = IDZone; }
        if (IDEmployee != null) { strIDEmp = IDEmployee; }
        top.ShowExternalForm2('Access/AccessFilterPunches.aspx?IDEmployee=' + strIDEmp + '&IDZone=' + strIDZone, 750, 495, Title, '', false, false);
    }
    catch (e) { showError("ShowAccFilter", e); }
}

function ShowAccPlatesFilter() {
    try {
        var Title = '';
        top.ShowExternalForm2('Access/AccessFilterPlates.aspx', 1150, 725, Title, '', false, false);
    } catch (e) { showError("ShowAccPlatesFilter", e); }
}

function reloadflBg(oID) {
    StatusMap_DoFSCommand('RELOADBG', oID);
    actualAccessStatusMain = oID;
    setAccessZonesImageIDMain(oID);

    var strParameters = "SELECTEDPLANE=" + oID;
    CallbackHelperClient.PerformCallback(strParameters);
}

function CallbackHelperClient_CallbackComplete(s, e) {
    if (e.result != "") {
        var param = e.result.split("@");
        setAccessZonesPositionsMain(param[0]);
        setAccessZonesImageIDMain(param[1]);
        SetPosition();
    }
}

function SetPosition() {
    try {
        if (getAccessZonesPositionsMain() != "") {
            var oParmArray = new Array();
            oParmArray = getAccessZonesPositionsMain().split(",");
            var strPositions = "";

            if (oParmArray.length > 10) {
                var oCnt = Math.round(oParmArray.length / 10);
                for (var xy = 0; xy < oCnt; xy++) {
                    var oIdx = xy * 10;
                    var strColor = oParmArray[oIdx + 6];
                    if (!strColor.startsWith("#")) {
                        var color = new RGBColor(strColor);
                        var colorHTML;

                        if (color.ok) { // 'ok' is true when the parsing was a success
                            colorHTML = color.toHex();
                        } else {
                            colorHTML = oParmArray[oIdx + 6];
                        } //end if

                        strPositions += oParmArray[oIdx]
                            + "," + oParmArray[oIdx + 1]
                            + "," + oParmArray[oIdx + 2]
                            + "," + oParmArray[oIdx + 3]
                            + "," + oParmArray[oIdx + 4]
                            + "," + oParmArray[oIdx + 5]
                            + "," + colorHTML.substring(1)
                            + "," + oParmArray[oIdx + 7]
                            + "," + +oParmArray[oIdx + 8]
                            + "," + +oParmArray[oIdx + 9]
                            + ",";
                    }
                }

                strPositions = strPositions.substring(0, strPositions.length - 1);
                setAccessZonesPositionsMain(strPositions);
            }
            else {
                var strColor = oParmArray[6];
                if (!strColor.startsWith("#")) {
                    var color = new RGBColor(strColor);
                    var colorHTML;
                    if (color.ok) { // 'ok' is true when the parsing was a success
                        colorHTML = color.toHex();
                    } else {
                        colorHTML = oParmArray[6];
                    } //end if
                    var PositionsAux = oParmArray[0]
                        + "," + oParmArray[1]
                        + "," + oParmArray[2]
                        + "," + oParmArray[3]
                        + "," + oParmArray[4]
                        + "," + oParmArray[5]
                        + "," + colorHTML.substring(1)
                        + "," + oParmArray[7]
                        + "," + oParmArray[8]
                        + "," + oParmArray[9];
                    setAccessZonesPositionsMain(PositionsAux);
                }
            }

            if (document.getElementById('divStatusMap').innerHTML != "") {
                setTimeout("StatusMap_DoFSCommand('REPOSITION', '" + getAccessZonesPositionsMain() + "');", 1500);
            }            
        }
        else {
            if (document.getElementById('divStatusMap').innerHTML != "") {
                setTimeout("StatusMap_DoFSCommand('RESET', '');", 1500);
            }            
        }
    }
    catch (e) {
        showError("SetPosition", e);
    }
}

function viewCam(oID) {
    window.open("viewCam.aspx?ID=" + oID, "_blank");
}

function showEmployee(oN, oID, strRoolUrl) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var _ajax = nuevoAjax();
    _ajax.open("GET", "../Base/WebUserControls/EmployeeSelectorData.aspx?action=getSelectionPath&node=B" + oID + "&TreeType=1" + stamp, true);

    _ajax.onreadystatechange = async function () {
        if (_ajax.readyState == 4) {
            var objPrefix = "ctl00_contentMainBody_roTrees1";
            var val = _ajax.responseText;
            if (val == null || val == "null") { val = ""; }

            var oTreeState = await getroTreeState(objPrefix);
            oTreeState.setSelectedPath(val, '1');
            oTreeState.setSelected("B" + oID, '1');

            window.open("../#/" + strRoolUrl + "/Employees/Employees", "_blank");
        }
    }

    _ajax.send(null)
}

function showAccessStatusMonitor(strRoolUrl) {
    try {
        if (actualAccessStatus > 0) {
            createCookie('showAccessStatusMonitor', actualAccessStatus, 1);
            window.open("../../" + strRoolUrl + "/Access/AccessStatusMonitor.aspx", "_blank");
        }
    } catch (e) { showError("showAccessStatusMonitor", e); }
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

//*****************************************************************************************/
// getAccessZonesParams
// Recupera Params del AccessZones
//********************************************************************************************/
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

//*****************************************************************************************/
// setAccessZonesParams
// Recupera Params del AccessZones
//********************************************************************************************/
function setAccessZonesPositions(oValue) {
    try {
        return hdnManagePlaneClient.Set('Position', oValue);
    }
    catch (e) {
        showError('setAccessZonesPositions', e);
    }
}

//*****************************************************************************************/
// setAccessZonesParams
// Recupera Params del AccessZones
//********************************************************************************************/
function setAccessZonesImageID(oValue) {
    try {
        hdnManagePlaneClient.Set('ImageID', oValue);
    }
    catch (e) {
        showError('setAccessZonesImageID', e);
    }
}

//=================
///===== MAIN IMAGENES ZONA ===============
//=================

//*****************************************************************************************/
// getAccessZonesImageID
// Recupera ImageID del AccessZone
//********************************************************************************************/
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

//*****************************************************************************************/
// getAccessZonesParams
// Recupera Params del AccessZones
//********************************************************************************************/
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

//*****************************************************************************************/
// setAccessZonesParams
// Recupera Params del AccessZones
//********************************************************************************************/
function setAccessZonesPositionsMain(oValue) {
    try {
        hdnManagePlaneClient.Set('PositionMain', oValue);
    }
    catch (e) {
        showError('setAccessZonesPositionsMain', e);
    }
}

//*****************************************************************************************/
// setAccessZonesImageID
// Recupera Params del AccessZones
//********************************************************************************************/
function setAccessZonesImageIDMain(oValue) {
    try {
        hdnManagePlaneClient.Set('ImageIDMain', oValue);
    }
    catch (e) {
        showError('setAccessZonesImageIDMain', e);
    }
}

function showAccessStatusMonitor(strRoolUrl) {
    try {
        if (actualNodo > 0) {
            createCookie('showAccessStatusMonitor', actualNodo, 1);
            window.open("../../" + strRoolUrl + "/Access/AccessStatusMonitor.aspx", "_blank");
        }
    } catch (e) { showError("ShowAccessFilter", e); }
}