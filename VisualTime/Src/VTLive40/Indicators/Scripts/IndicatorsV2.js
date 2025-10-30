var actualTab = 0; // TAB per mostrar
var actualIndicator; // Indicador actual

var newObjectName = "";

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    showLoadingGrid(false);

    switch (s.cpActionRO) {
        case "GETINDICATOR":

            if (s.cpResultRO == "OK") {
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameIndicators").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    document.getElementById("readOnlyNameIndicators").textContent = newObjectName;
                    hasChanges(true);
                    txtName_Client.SetValue(newObjectName);
                }

                if (s.cpIsNewRO == true) {
                    refreshTree();
                }
            }

            break;
        case "SAVEINDICATOR":
            if (s.cpResultRO == "KO") {
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameIndicators").textContent = s.cpNameRO;
                }
                hasChanges(true);
                showErrorPopup("Error.Title", "error", "", s.cpErrorRO.ErrorText, "Error.OK", "Error.OKDesc", "");
            }

            break;
    }
}

function cargaNodo(Nodo) {
    try {
        if (Nodo.id.toUpperCase() == "SOURCE") newIndicator();
        else cargaIndicators(Nodo.id);
    } catch (e) { showError("cargaNodo", e); }
}

function cargaIndicators(IDIndicator) {
    try {
        showLoadingGrid(true);
        actualIndicator = IDIndicator;
        cargaIndicatorsTabSuperior(IDIndicator);
    } catch (e) { showError("cargaIndicators", e); }
}

function cargaIndicatorsTabSuperior(IDIndicator) {
    try {
        var Url = "Handlers/srvIndicators.ashx?action=getIndicatorsTab&aTab=" + actualTab + "&ID=" + IDIndicator;
        AsyncCall("POST", Url, "CONTAINER", "divIndicators", "cargaIndicatorsBarButtons(" + IDIndicator + ")");
    }
    catch (e) {
        showError("cargaIndicatorsTabSuperior", e);
    }
}
var responseObj = null;
function cargaIndicatorsBarButtons(IDIndicator) {
    try {
        var Url = "Handlers/srvIndicators.ashx?action=getBarButtons&ID=" + IDIndicator;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDIndicator + ")");
    }
    catch (e) {
        showError("cargaIndicatorsTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IDIndicator) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaIndicatorsDivs(IDIndicator);
}

function cargaIndicatorsDivs(IDIndicator) {
    try {
        var oParameters = {};
        oParameters.aTab = actualTab
        oParameters.ID = actualIndicator;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "GETINDICATOR";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    } catch (e) { showError("cargaIndicatorsDivs", e); }
}

function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00');
    arrDivs = new Array('div00');

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
    arrDivs = new Array('div00');

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

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesIndicators";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesIndicators";
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

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Indicators/srvMsgBoxIndicator.aspx?action=Message";
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

function undoChanges() {
    try {
        if (actualIndicator == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesIndicators";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaIndicators(actualIndicator);
        }
    } catch (e) { showError("undoChanges", e); }
}

function saveChanges() {
    try {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            var oParameters = {};
            oParameters.aTab = actualTab
            oParameters.ID = actualIndicator;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVEINDICATOR";

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        };
    } catch (e) { showError("saveChanges", e); }
}

function checkIndicatorEmptyName(newName) {
    document.getElementById("readOnlyNameIndicators").textContent = newName;
    hasChanges(true);
}

function newIndicator() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=Indicator";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newDocumentAbsence', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaIndicators(-1);
        newObjectName = ObjName;
    } catch (e) { showError('newConcept', e); }
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function deleteIndicator(Id) {
    try {
        AsyncCall("POST", "Handlers/srvIndicators.ashx?action=deleteXIndicator&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus,true); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
    } catch (e) { showError('deleteIndicator', e); }
}

function ShowRemoveIndicator() {
    try {
        if (actualIndicator < 1) { return; }

        var url = "Indicators/srvMsgBoxIndicator.aspx?action=Message";
        url = url + "&TitleKey=deleteIndicator.Title";
        url = url + "&DescriptionKey=deleteIndicator.Description";
        url = url + "&Option1TextKey=deleteIndicator.Option1Text";
        url = url + "&Option1DescriptionKey=deleteIndicator.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteIndicator('" + actualIndicator + "'); return false;";
        url = url + "&Option2TextKey=deleteIndicator.Option2Text";
        url = url + "&Option2DescriptionKey=deleteIndicator.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveIndicator", e); }
}

function checkStatus(oStatus, noHasChanges) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Indicators/srvMsgBoxIndicator.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
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