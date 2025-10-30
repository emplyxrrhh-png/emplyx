var actualTab = 0;
var actualDiningRoom;

var newObjectName = "";

function checkDinningRoomEmptyName(newName) {
    document.getElementById("readOnlyNameDiningRoom").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    changeTabs(actualTab);
    showLoadingGrid(false);

    ConvertControls('divContent');
    ConvertControls('divDiningRoom');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAll');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optSelection');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_optSelection');

    checkResult(s);

    switch (s.cpAction) {
        case "GETDININGROOM":
            if (s.cpIsNew == true) {
                refreshTree();
            } else {
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameDiningRoom").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    document.getElementById("readOnlyNameDiningRoom").textContent = newObjectName;
                    hasChanges(true);
                    txtName_Client.SetValue(newObjectName);
                }
            }
            break;

        case "SAVEDININGROOM":
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
        grabarDiningRoom(actualDiningRoom);
    } else {
        showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
    };
}

function undoChanges() {
    try {
        if (actualDiningRoom == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesDiningRoom";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaDiningRoom(actualDiningRoom);
        }
    } catch (e) { showError("undoChanges", e); }
}

function cargaNodo(Nodo) {
    if (Nodo.id == "source") Nodo.id = "-1";
    cargaDiningRoom(Nodo.id);
}

function cargaDiningRoom(IdDiningRoom) {
    actualDiningRoom = IdDiningRoom;
    showLoadingGrid(true);
    cargaDiningRoomTabSuperior(IdDiningRoom);
}

function newDiningRoom() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=DiningRoom";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newCamera', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaDiningRoom(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

function cargaDiningRoomTabSuperior(IdDiningRoom) {
    try {
        var Url = "Handlers/srvDiningRoom.ashx?action=getDiningRoomTab&aTab=" + actualTab + "&ID=" + IdDiningRoom;
        AsyncCall("POST", Url, "CONTAINER", "divDiningRoom", "cargaDiningRoomBarButtons(" + IdDiningRoom + ");");
    }
    catch (e) {
        showError("cargaDiningRoomTabSuperior", e);
    }
}
var responseObj = null;
function cargaDiningRoomBarButtons(IdDiningRoom) {
    try {
        var Url = "Handlers/srvDiningRoom.ashx?action=getBarButtons&ID=" + IdDiningRoom;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IdDiningRoom + ")");
    }
    catch (e) {
        showError("cargaDiningRoomBarButtons", e);
    }
}

function parseResponseBarButtons(oResponse, IdDiningRoom) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaDiningRoomDivs(IdDiningRoom);
}

function grabarDiningRoom(IdDiningRoom) {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdDiningRoom;
    oParameters.Name = document.getElementById("readOnlyNameDiningRoom").textContent.trim();
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEDININGROOM";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaDiningRoomDivs(IdDiningRoom) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdDiningRoom;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETDININGROOM";
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

function ShowRemoveDiningRoom() {
    try {
        if (actualDiningRoom == "-1" || actualDiningRoom == "0") { return; }

        var url = "DiningRoom/srvMsgBoxDiningRoom.aspx?action=Message";
        url = url + "&TitleKey=deleteDiningRoom.Title";
        url = url + "&DescriptionKey=deleteDiningRoom.Description";
        url = url + "&Option1TextKey=deleteDiningRoom.Option1Text";
        url = url + "&Option1DescriptionKey=deleteDiningRoom.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteDiningRoom('" + actualDiningRoom + "'); return false;";
        url = url + "&Option2TextKey=deleteDiningRoom.Option2Text";
        url = url + "&Option2DescriptionKey=deleteDiningRoom.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveDiningRoom", e); }
}

function deleteDiningRoom(Id) {
    try {
        if (Id == "-1" || Id == "0") {
        }
        else {
            try {
                AsyncCall("POST", "Handlers/srvDiningRoom.ashx?action=deleteDiningRoom&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus,true); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
            }
            catch (e) {
                showError('deleteDiningRoom', e);
            }
        }
    }
    catch (e) {
        showError('deleteDiningRoom', e);
    }
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
    var ctlPrefix = "ctl00_contentMainBody_roTreesDiningRoom";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, false, false);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesDiningRoom";
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
        var url = "DiningRoom/srvMsgBoxDiningRoom.aspx?action=Message";
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
        if (oResult.cpAction == "SAVEDININGROOM") {
            hasChanges(true);
        }

        var url = "DiningRoom/srvMsgBoxDiningRoom.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
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
        arrStatus = oStatus;
        objError = arrStatus[0];

        if (objError.error == "true") {
            if (objError.typemsg == "1") {
                var url = "DiningRoom/srvMsgBoxDiningRoom.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else {
            }
            if (noHasChanges == null) {
                hasChanges(true);
            }
        }
    } catch (e) { showError("checkStatus", e); }
}