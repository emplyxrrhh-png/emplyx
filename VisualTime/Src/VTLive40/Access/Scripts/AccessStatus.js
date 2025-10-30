var actualTab = 0; // TAB per mostrar
var actualAccessStatus; // AccessStatus actual

var oAccessStatInfo;   //Clase AccessStatusData
var oAccessStatus;   //Clase AccessStatusData

var bolLoadFirstAccessStatus = false;
var isRoot = true;

var jsGridExceptions; //Grid Exceptions
var jsGridPeriods; //Grid Periods

var idInterval;

var actualNodo = 0;
var timeOutAccess;

function cargaNodo(Nodo) {
    try {
        var oID = Nodo.id.substring(1);

        if (Nodo.id.substring(0, 1) == "A") {
            isRoot = true;
            if (document.getElementById('divEmpresa') == null) {
                cargaAccessStatInfo(oID);
                bolLoadFirstAccessStatus = true;
            } else {
                actualAccessStatus = oID;
                if (oAccessStatInfo == null) {
                    oAccessStatInfo = new DataAccessStatInfo(oID);
                } else {
                    oAccessStatInfo.getAccessStatInfoById(oID);
                }
            }
            actualNodo = 0;

            clearTimeout(timeOutAccess)
        }
        else {
            isRoot = false;
            actualNodo = oID;
            cargaAccessStatus(oID);
            bolLoadFirstAccessStatus = true;
        }
    }
    catch (e) {
        showError("cargaNodo", e);
    }
}

//Carga Tabs y contenido Empleados
function cargaAccessStatInfo(IDAccessStatus) {
    try {
        actualAccessStatus = IDAccessStatus;
        //TAB Gris Superior
        showLoadingGrid(true);
        cargaAccessStatInfoTabSuperior(IDAccessStatus);
        //Area General, etc.
        //cargaAccessStatInfoDivs(IDAccessStatus);
    } catch (e) { showError("cargaAccessStatInfo", e); }
}

//Carga Tabs y contenido Empleados
function cargaAccessStatus(IDAccessStatus) {
    try {
        actualAccessStatus = IDAccessStatus;
        //TAB Gris Superior
        showLoadingGrid(true);
        cargaAccessStatusTabSuperior(IDAccessStatus);
        //Area General, etc.
        //ppr-> cargaAccessStatusDivs(IDAccessStatus);

        //cargaAccessStatusDivs(1);
    }
    catch (e) {
        showError("cargaAccessStatus", e);
    }
}

var responseObj = null;
// Carrega la part del TAB grisa superior
function cargaAccessStatInfoTabSuperior(IDAccessStatus) {
    try {
        var Url = "";

        Url = "srvAccessStatInfo.aspx?action=getAccessStatInfoTab&aTab=" + actualTab + "&ID=" + IDAccessStatus;
        AsyncCall("POST", Url, "CONTAINER", "divAccessStatus", "");

        Url = "srvAccessStatInfo.aspx?action=getBarButtons&ID=" + IDAccessStatus;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtonsInfo(objContainerId," + IDAccessStatus + ")");
    }
    catch (e) {
        showError("cargaAccessStatInfoTabSuperior", e);
    }
}

function parseResponseBarButtonsInfo(oResponse, IDAccessStatus) {
    let container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaAccessStatInfoDivs(IDAccessStatus);
}
var responseObjStatus = null;
// Carrega la part del TAB grisa superior
function cargaAccessStatusTabSuperior(IDAccessStatus) {
    try {
        var Url = "";

        Url = "srvAccessStatus.aspx?action=getAccessStatusTab&aTab=" + actualTab + "&ID=" + IDAccessStatus;
        AsyncCall("POST", Url, "CONTAINER", "divAccessStatus", "");

        Url = "srvAccessStatus.aspx?action=getBarButtons&ID=" + IDAccessStatus;
        AsyncCall("POST", Url, "JSON3", responseObjStatus, "parseResponseBarButtonsStatus(objContainerId," + IDAccessStatus + ")");
    }
    catch (e) {
        showError("cargaAccessStatusTabSuperior", e);
    }
}

function parseResponseBarButtonsStatus(oResponse, IDAccessStatus) {
    let container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaAccessStatusDivs(IDAccessStatus);
}
// Carrega els apartats dels divs de l'usuari
function cargaAccessStatInfoDivs(IDAccessStatus) {
    try {
        var Url = "srvAccessStatInfo.aspx?action=getAccessStatInfo&aTab=" + actualTab + "&ID=" + IDAccessStatus;
        AsyncCall("POST", Url, "CONTAINER", "divContent", "oAccessStatInfo = new DataAccessStatInfo(" + IDAccessStatus + "); showLoadingGrid(false);");
    } catch (e) { showError("cargaAccessStatInfoDivs", e); }
}

function cargaAccessStatusDivs(activeTime) {
    try {
        clearTimeout(timeOutAccess);

        if (actualNodo > 0) {
            var go = true;
            var loadingData = document.getElementById('hdnLoadingData');
            if (loadingData != null)
                if (loadingData.value == "1")
                    go = false;

            if (go == true) {
                //ACTIVAR SEMAFORO
                if (loadingData != null) loadingData.value = "1";

                var Url = "srvAccessStatus.aspx?action=getAccessStatus&aTab=" + actualTab + "&ID=" + actualNodo;
                AsyncCall("POST", Url, "CONTAINER", "divContent", "cargaAccessStatusDivsReturn();");

                var objDiv = document.getElementById('divContenido');
                if (objDiv != null) {
                    objDiv.style.display = '';
                }
            }

            //if (activeTime > 0) {
            //    activeTime = 6000;  //1 minuto
            //    timeOutAccess = setTimeout("cargaAccessStatusDivs(true)", activeTime); //Periodic call to server.
            //}
        }
    }
    catch (e) {
        showError("cargaAccessStatusDivs", e);
    }
}

// funcion que se ejecuta al volver de cargaAccessStatusDivs
function cargaAccessStatusDivsReturn() {
    try {
        oAccessStatus = new DataAccessStatus(" + actualNodo + ");

        //DESACTIVAR SEMAFORO
        var loadingData = document.getElementById('hdnLoadingData');
        if (loadingData != null)
            loadingData.value = "0";

        showLoadingGrid(false);
    }
    catch (e) {
        showError("cargaAccessStatusDivsReturn", e);
    }
}

//Cambia els Tabs i els divs
function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('div00', 'div01');

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

//Cambia els Tabs i els divs (per nom)
function changeTabsByName(nameTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('div00', 'div01');

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
    } catch (e) { showError("RefreshScreen", e); }
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
    } catch (e) { alert('setMessage: ' + e); }
}

function setStyleMessage(classMsg) {
    try {
        //divContainers styles
        var divTop = document.getElementById('divMsgTop');
        var divBottom = document.getElementById('divMsgBottom');

        divTop.className = classMsg;
        divBottom.className = classMsg;
    } catch (e) { alert('setStyleMessage: ' + e); }
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

//Mostra el "Cargando..." Sobre el Grid
/*************************************************************************************************************/
function showLoadingGrid(loading) { parent.showLoader(loading); }

//Asistente exportacion
function ShowNewAccessStatusWizard() {
    try {
        var Title = '';
        top.ShowExternalForm2('Access/Wizards/NewAccessZoneWizard.aspx', 500, 450, Title, '', false, false, false);
    } catch (e) { showErrorPopup("ShowNewAccessStatusWizard", e); }
}

function showMsg(oMsg) {
    alert(oMsg);
}

//Cambiar la imatge de l'usuari
function ShowZoomStatus() {
    try {
        var Title = '';
        var ImageID = oAccessStatInfo.getAccessStatInfoImageID();
        var oID;
        oID = oAccessStatInfo.getAccessStatInfoID();
        strParams = oAccessStatInfo.getAccessStatInfoPositions();
        //ImageID = oID;

        var w = document.body.clientWidth - 50;
        var h = document.body.clientHeight;

        top.ShowExternalForm2('Access/ZoomStatus.aspx?ID=' + oID + "&strParams=" + encodeURIComponent(strParams) + "&ImageID=" + ImageID, w, h, Title, '', false, false);
    } catch (e) { showError("ShowZoomStatus", e); }
}

function setPosition() {
    try {
        var oPos = "";
        if (isRoot) {
            if (oAccessStatInfo != null) {
                oPos = oAccessStatInfo.getAccessStatInfoPositions();
            } //end if
        } else {
            if (oAccessStatus != null) {
                oPos = oAccessStatus.getAccessStatusPositions();
            } //end if
        } //end if
        if (oPos != "") { setTimeout("StatusMap_DoFSCommand('REPOSITION', '" + oPos + "');", 800); }
    } catch (e) { showError("setPosition", e); }
} //end function

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function refreshStatusList() {
    try {
        if (document.getElementById('divEmpresa') == null) {
            //No esta carregat, deshabilitem el timer
            disableTimer();
        } else {
            if (oAccessStatInfo == null) {
                disableTimer();
            } else {
                oAccessStatInfo.getAccessStatInfoById(oAccessStatInfo.getAccessStatInfoID());
            }
        }
    } catch (e) { showError("refreshStatusList", e); }
}

function enableTimer() {
    try {
        if (idInterval == null) {
            idInterval = setInterval("refreshStatusList()", 30000);
        }
    } catch (e) { showError("enableTimer", e); }
}

function disableTimer() {
    try {
        if (idInterval != null) {
            clearInterval(idInterval);
            idInterval = null;
        }
    } catch (e) { showError("disableTimer", e); }
}

//Carga empleado y Tabs desde la pantalla de grupos (reposiciona el arbol)
function cargaTreeZone(IDSubZone) {
}

//Redireccionem a la zona seleccionada (flash)
function ShowZone(IdSubZone) {
    try {
        //Reposiciona l'arbre principal
        var ctlPrefix = "ctl00_contentMainBody_roTreesAccessStatus";
        eval(ctlPrefix + "_roTrees.GetSelectedPath('1', 'B' + IdSubZone, false,'" + ctlPrefix + "',true);");
    } catch (e) { showError("ShowZone", e); }
}

function ReturnToParent() {
    try {
        //Reposiciona l'arbre principal
        var ctlPrefix = "ctl00_contentMainBody_roTreesAccessStatus";
        eval(ctlPrefix + "_roTrees.GetSelectedPath('1', 'A1', false,'" + ctlPrefix + "',true);");
    } catch (e) { showError("ReturnToParent", e); }
}

//Filtre d'accesos (desde usuari)
function showAccessFilter(IDEmployee, IDZone) {
    try {
        var Title = '';
        //var IDZone = oAccessStatus.getAccessStatusID();
        top.ShowExternalForm2('Access/AccessFilterPunches.aspx?IDEmployee=' + IDEmployee + '&IDZone=' + IDZone, 750, 495, Title, '', false, false);
    } catch (e) { showError("ShowAccessFilter", e); }
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
    } catch (e) { showError("ShowAccFilter", e); }
}

function ShowAccPlatesFilter() {
    try {
        var Title = '';
        top.ShowExternalForm2('Access/AccessFilterPlates.aspx', 1150, 790, Title, '', false, false);
    } catch (e) { showError("ShowAccPlatesFilter", e); }
}

function reloadflBg(oID) {
    StatusMap_DoFSCommand('RELOADBG', oID);
    if (oAccessStatInfo == null) { return; }
    oAccessStatInfo.setAccessStatInfoID(oID);
    oAccessStatInfo.setAccessStatInfoImageID(oID);
    oAccessStatInfo.getAccessStatInfoById(oID);
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
        if (actualNodo > 0) {
            createCookie('showAccessStatusMonitor', actualNodo, 1);
            window.open("../../" + strRoolUrl + "/Access/AccessStatusMonitor.aspx", "_blank");
        }
    } catch (e) { showError("ShowAccessFilter", e); }
}