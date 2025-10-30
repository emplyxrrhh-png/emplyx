var actualTab = 0;
var actualAssignment;

var newObjectName = "";

function checkAssignmentEmptyName(newName) {
    document.getElementById("readOnlyNameAssignments").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    showLoadingGrid(false);

    ConvertControls('divContent');
    ConvertControls('divAssignments');

    checkResult(s);

    switch (s.cpAction) {
        case "GETASSIGNMENT":
            if (s.cpIsNew == true) {
                refreshTree();
            } else {
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameAssignments").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    document.getElementById("readOnlyNameAssignments").textContent = newObjectName;
                    hasChanges(true);
                    txtName_Client.SetValue(newObjectName);
                }
            }
            break;

        case "SAVEASSIGNMENT":
            if (s.cpResult == 'OK') {
                hasChanges(false);
                refreshTree();
            }
            break;

        default:
            hasChanges(false);
    }
}

function cargaAssignment(IdAssignment) {
    actualAssignment = IdAssignment;
    showLoadingGrid(true);
    cargaAssignmentTabSuperior(IdAssignment);
}

function saveChanges() {
    if (ASPxClientEdit.ValidateGroup(null, true)) {
        grabarAssignment(actualAssignment);
    } else {
        showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
    };
}

function undoChanges() {
    try {
        if (actualAssignment == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesAssignments";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaAssignment(actualAssignment);
        }
    } catch (e) { showError("undoChanges", e); }
}

function cargaNodo(Nodo) {
    if (Nodo.id == "source") newAssignment();
    else cargaAssignment(Nodo.id);
}

function newAssignment() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=Assignment";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newCamera', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaAssignment(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}
var responseObj = null;
function cargaAssignmentTabSuperior(IdAssignment) {
    try {
        var Url = "";

        Url = "Handlers/srvAssignments.ashx?action=getAssignmentsTab&aTab=" + actualTab + "&ID=" + IdAssignment;
        AsyncCall("POST", Url, "CONTAINER", "divAssignments", "");

        Url = "Handlers/srvAssignments.ashx?action=getBarButtons&ID=" + IdAssignment;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IdAssignment + ")");
    }
    catch (e) {
        showError("cargaAssignmentTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IdAssignment) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaAssignmentDivs(IdAssignment);
}

function grabarAssignment(IdAssignment) {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAssignment;
    oParameters.Name = document.getElementById("readOnlyNameAssignments").innerHTML.trim();
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEASSIGNMENT";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaAssignmentDivs(IdAssignment) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAssignment;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETASSIGNMENT";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
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

function ShowRemoveAssignment() {
    try {
        if (actualAssignment == "-1" || actualAssignment == "0") { return; }

        var url = "Assignments/srvMsgBoxAssignment.aspx?action=Message";
        url = url + "&TitleKey=deleteAssignment.Title";
        url = url + "&DescriptionKey=deleteAssignment.Description";
        url = url + "&Option1TextKey=deleteAssignment.Option1Text";
        url = url + "&Option1DescriptionKey=deleteAssignment.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteAssignment('" + actualAssignment + "'); return false;";
        url = url + "&Option2TextKey=deleteAssignment.Option2Text";
        url = url + "&Option2DescriptionKey=deleteAssignment.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveAssignment", e); }
}

function deleteAssignment(Id) {
    try {
        if (Id == "-1" || Id == "0") {
        }
        else {
            try {
                AsyncCall("POST", "Handlers/srvAssignments.ashx?action=deleteAssignment&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus,true); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
            }
            catch (e) {
                showError('deleteAssignment', e);
            }
        }
    }
    catch (e) {
        showError('deleteAssignment', e);
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
    var ctlPrefix = "ctl00_contentMainBody_roTreesAssignments";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, false, false);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesAssignments";
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
        var url = "Assignments/srvMsgBoxAssignment.aspx?action=Message";
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
        if (oResult.cpAction == "SAVEASSIGNMENT") {
            hasChanges(true);
        }

        var url = "Assignments/srvMsgBoxAssignment.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
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
                var url = "Assignments/srvMsgBoxAssignment.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
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