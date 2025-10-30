var actualTab = 0; // TAB per mostrar
var actualBusinessCenters; // BusinessCenters actual

var newObjectName = "";

function checkBusinessCenterEmptyName(newName) {
    document.getElementById("readOnlyNameBusinessCenters").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    ConvertControls('divContenido');
    EnablePanelsFunctionallity();

    showLoadingGrid(false);

    switch (s.cpActionRO) {
        case "GETBUSINESSCENTER":

            if (s.cpResultRO == "OK") {
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameBusinessCenters").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    document.getElementById("readOnlyNameBusinessCenters").textContent = newObjectName;
                    hasChanges(true);
                    txtName_Client.SetValue(newObjectName);
                }

                if (s.cpIsNew == true) {
                    refreshTree();
                }
            }

            break;
        case "SAVEBUSINESSCENTER":
            if (s.cpResultRO == "KO") {
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameBusinessCenters").textContent = s.cpNameRO;
                }
                hasChanges(true);
                showErrorPopup("Error.Title", "error", "", s.cpErrorRO.ErrorText, "Error.OK", "Error.OKDesc", "");
            }

            break;
    }
}

function cargaNodo(Nodo) {
    try {
        //if (selectFistNode) {
        //    selectFistNode = false;
        //    SelectFirstNode();
        //    actualBusinessCenters = Nodo.id;
        //}else{
        if (Nodo.id.toUpperCase() == "SOURCE") newBusinessCenter();
        else {
            cargaBusinessCenters(Nodo.id);
        }
        //}
    } catch (e) { showError("cargaNodo", e); }
}

function cargaBusinessCenters(IDBusinessCenters) {
    try {
        showLoadingGrid(true);
        actualBusinessCenters = IDBusinessCenters;
        cargaBusinessCentersTabSuperior(IDBusinessCenters);
    } catch (e) { showError("cargaBusinessCenters", e); }
}

function cargaBusinessCentersTabSuperior(IDBusinessCenters) {
    try {
        var Url = "Handlers/srvBusinessCenters.ashx?action=getBusinessCentersTab&aTab=" + actualTab + "&ID=" + IDBusinessCenters;
        AsyncCall("POST", Url, "CONTAINER", "divBusinessCenters", "cargaBusinessCentersBarButtons(" + IDBusinessCenters + ")");
    }
    catch (e) {
        showError("cargaBusinessCentersTabSuperior", e);
    }
}
var responseObj = null;
function cargaBusinessCentersBarButtons(IDBusinessCenters) {
    try {
        var Url = "Handlers/srvBusinessCenters.ashx?action=getBarButtons&ID=" + IDBusinessCenters;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDBusinessCenters + ")");
    }
    catch (e) {
        showError("cargaBusinessCentersBarButtons", e);
    }
}

function parseResponseBarButtons(oResponse, IDBusinessCenters) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaBusinessCentersDivs(IDBusinessCenters);
}

function cargaBusinessCentersDivs(IDBusinessCenters) {
    try {
        var oParameters = {};
        oParameters.aTab = actualTab
        oParameters.ID = actualBusinessCenters;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "GETBUSINESSCENTER";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    } catch (e) { showError("cargaIndicatorsDivs", e); }
}

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function hasChanges(bolChanges, markRecalc) {
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

function saveChanges() {
    try {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            showLoadingGrid(true);
            var oParameters = {};
            oParameters.aTab = actualTab
            oParameters.ID = actualBusinessCenters;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVEBUSINESSCENTER";

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        };
    } catch (e) { showError("saveChanges", e); }
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
        } else if (DataType == "6") {
            refreshTree();
        }
    } catch (e) { showError("RefreshScreen", e); }
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function SelectFirstNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesBusinessCenters";
    eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
}

function undoChanges() {
    try {
        if (actualBusinessCenters == -1) {
            SelectFirstNode();
        } else {
            cargaBusinessCenters(actualBusinessCenters);
        }
    } catch (e) { showError("undoChanges", e); }
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
    var ctlPrefix = "ctl00_contentMainBody_roTreesBusinessCenters";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true,true);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesBusinessCenters";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Tasks/srvMsgBoxBusinessCenter.aspx?action=Message";
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

function showMsg(oMsg) {
    alert(oMsg);
}

function newBusinessCenter() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=BusinessCenter";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newDocumentAbsence', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaBusinessCenters(-1);
        newObjectName = ObjName;
    } catch (e) { showError('newConcept', e); }
}

function deleteBusinessCenter(Id) {
    try {
        AsyncCall("POST", "Handlers/srvBusinessCenters.ashx?action=deleteXBusinessCenter&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus,true); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
    } catch (e) { showError('deleteBusinessCenter', e); }
}

function checkStatus(oStatus, noHasChanges) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Tasks/srvMsgBoxBusinessCenter.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + encodeURIComponent(objError.msg) + "&" +
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

function ShowRemoveBusinessCenter() {
    try {
        if (actualBusinessCenters < 1) { return; }

        var url = "Tasks/srvMsgBoxBusinessCenter.aspx?action=Message";
        url = url + "&TitleKey=deleteBusinessCenter.Title";
        url = url + "&DescriptionKey=deleteBusinessCenter.Description";
        url = url + "&Option1TextKey=deleteBusinessCenter.Option1Text";
        url = url + "&Option1DescriptionKey=deleteBusinessCenter.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteBusinessCenter('" + actualBusinessCenters + "'); return false;";
        url = url + "&Option2TextKey=deleteBusinessCenter.Option2Text";
        url = url + "&Option2DescriptionKey=deleteBusinessCenter.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveBusinessCenter", e); }
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function editBusinessCenterFieldsGrid(IDGroup) {
    showLoadingUserFields(true);

    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = actualBusinessCenters;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "EDITGRID";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function saveBusinessCenterFieldsGrid(IDGroup) {
    showLoadingGrid(true);

    var contenedor;
    contenedor = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divBusinessCenterFields');

    arrInputs = new Array();
    arrInputs = contenedor.getElementsByTagName('input');
    var nTotalFields = arrInputs.length;

    var postvars = '';
    for (n = 0; n < nTotalFields; n++) {
        postvars += '&USR_' + arrInputs[n].id.replace('ctl00_contentMainBody_ASPxCallbackPanelContenido_', '') + '=' + arrInputs[n].value;
    }

    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = actualBusinessCenters;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEBUSINESSCENTERFIELDS";
    oParameters.resultClientAction = postvars;
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cancelBusinessCenterFieldsGrid() {
    showLoadingGrid(true);
    cargaBusinessCenters(actualBusinessCenters);
}

function showLoadingUserFields(loading) {
    var img = document.getElementById('imgUserFieldsLoading');
    if (img != null) {
        if (loading == true) {
            img.style.display = '';
        }
        else {
            img.style.display = 'none';
        }
    }
}

function EnablePanelsFunctionallity() {
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opActive');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opNoActive');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opActive,ctl00_contentMainBody_ASPxCallbackPanelContenido_opNoActive');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opAllZones');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opSelectedZones');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opAllZones,ctl00_contentMainBody_ASPxCallbackPanelContenido_opSelectedZones');
}

//Cambia els Tabs i els divs
function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('div00', 'div01');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
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