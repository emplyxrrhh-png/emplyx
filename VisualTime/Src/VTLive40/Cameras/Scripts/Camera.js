var actualTab = 0; // TAB per mostrar
var actualCamera; // Camera actual

var newObjectName = "";

function checkCameraEmptyName(newName) {
    document.getElementById("readOnlyNameCamera").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    showLoadingGrid(false);

    ConvertControls('divContent');
    ConvertControls('divCamera');

    checkResult(s);

    switch (s.cpAction) {
        case "GETCAMERA":
            if (s.cpIsNew == true) {
                refreshTree();
            } else {
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameCamera").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    document.getElementById("readOnlyNameCamera").textContent = newObjectName;
                    hasChanges(true);
                    txtName_Client.SetValue(newObjectName);
                }
            }
            break;

        case "SAVECAMERA":
            if (s.cpResult == 'OK') {
                hasChanges(false);
                refreshTree();
            }
            break;

        default:
            hasChanges(false);
    }
}

function saveChanges() {
    if (ASPxClientEdit.ValidateGroup(null, true)) {
        grabarCamera(actualCamera);
    } else {
        showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
    };
}

function undoChanges() {
    try {
        if (actualCamera == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesCameras";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaCamera(actualCamera);
        }
    } catch (e) { showError("undoChanges", e); }
}

function cargaNodo(Nodo) {
    if (Nodo.id == "source") newCamera();
    else cargaCamera(Nodo.id);
}

function cargaCamera(IdCamera) {
    actualCamera = IdCamera;
    showLoadingGrid(true);
    cargaCameraTabSuperior(IdCamera);
}

function newCamera() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=Camera";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newCamera', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaCamera(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

function cargaCameraTabSuperior(IdCamera) {
    try {
        var Url = "Handlers/srvCameras.ashx?action=getCameraTab&aTab=" + actualTab + "&ID=" + IdCamera;
        AsyncCall("POST", Url, "CONTAINER", "divCamera", "cargaCameraBarButtons(" + IdCamera + ");");
    }
    catch (e) {
        showError("cargaCameraTabSuperior", e);
    }
}
var responseObj = null;
function cargaCameraBarButtons(IdCamera) {
    try {
        var Url = "Handlers/srvCameras.ashx?action=getBarButtons&ID=" + IdCamera;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IdCamera + ")");
    }
    catch (e) {
        showError("cargaCameraBarButtons", e);
    }
}

function parseResponseBarButtons(oResponse, IdCamera) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaCameraDivs(IdCamera);
}

function grabarCamera(IdCamera) {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = actualCamera;
    oParameters.Name = document.getElementById("readOnlyNameCamera").textContent.trim();
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVECAMERA";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaCameraDivs(IdCamera) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdCamera;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETCAMERA";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function hasChanges(bolChanges) {
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
    }
    else {
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

function ShowRemoveCamera() {
    try {
        if (actualCamera == "-1" || actualCamera == "0") { return; }

        var url = "Cameras/srvMsgBoxCameras.aspx?action=Message";
        url = url + "&TitleKey=deleteCamera.Title";
        url = url + "&DescriptionKey=deleteCamera.Description";
        url = url + "&Option1TextKey=deleteCamera.Option1Text";
        url = url + "&Option1DescriptionKey=deleteCamera.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteCamera('" + actualCamera + "'); return false;";
        url = url + "&Option2TextKey=deleteCamera.Option2Text";
        url = url + "&Option2DescriptionKey=deleteCamera.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveCamera", e); }
}

function deleteCamera(oId) {
    try {
        AsyncCall("POST", "Handlers/srvCameras.ashx?action=deleteCamera&ID=" + oId, "json", "arrStatus", "checkStatus(arrStatus,true); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
    }
    catch (e) { showError('deleteCamera', e); }
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function viewCam() {
    try {
        var tUrl = txtURL_Client.GetValue();
        var sUrl = '';
        if (tUrl != null) {
            if (tUrl.value == "") {
                showErrorPopup("Error.Title", "error", "Error.Description.NoURLSelected", "Error.OK", "Error.OKDesc", "");
                return;
            }
            if (tUrl.indexOf("://") == -1) {
                sUrl = "http://" + tUrl;
            }
            window.open(sUrl);
        }
    } catch (e) { showError("viewCam", e); }
}

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesCameras";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesCameras";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
        } else if (DataType == "6") {
            refreshTree();
        }
    } catch (e) { showError("RefreshScreen", e); }
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Cameras/srvMsgBoxCameras.aspx?action=Message";
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
        if (oResult.cpAction == "SAVECAMERA") {
            hasChanges(true);
        }

        var url = "Cameras/srvMsgBoxCameras.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
            "DescriptionText=" + oResult.cpMessage + "&" +
            "Option1TextKey=SaveName.Error.Option1Text&" +
            "Option1DescriptionKey=SaveName.Error.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
}

function checkStatus(oStatus, noHasChanges) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Cameras/srvMsgBoxCameras.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else { //Missatge estil inline
            }
            if (noHasChanges == null) {
                hasChanges(true);
            }
        }
    } catch (e) { showError("checkStatus", e); }
}