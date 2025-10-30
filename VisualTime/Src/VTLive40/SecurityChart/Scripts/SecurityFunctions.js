var actualTab = 0;
var actualSecurityFunction;
var clientObjectsData = null;
var newObjectName = "";

function checkDinningRoomEmptyName(newName) {
    document.getElementById("readOnlyNameSecurityFunction").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    changeTabs(actualTab);

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optBGListAll');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optBGListValue');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optBGListAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_optBGListValue');

    showLoadingGrid(false);

    ConvertControls('divContent');

    checkResult(s);

    switch (s.cpActionRO) {
        case "GETSECURITYFUNCTION":
            if (typeof s.cpClientControlsRO != 'undefined' && s.cpClientControlsRO != '') {
                clientObjectsData = JSON.parse(s.cpClientControlsRO)

                $("#lstBusinessGroups").dxTagBox({
                    dataSource: new DevExpress.data.ArrayStore({
                        data: clientObjectsData.businessGroups,
                        key: "ID"
                    }),
                    displayExpr: "Name",
                    valueExpr: "ID",
                    showSelectionControls: true,
                    applyValueMode: "useButtons",
                    readOnly: !clientObjectsData.clientEnabled,
                    value: clientObjectsData.selectedBusinessGroups,
                    onValueChanged: function (e) {
                        hasChanges(true);
                        chgOPCItems('1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optBGListAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_optBGListValue', 'undefined');
                        clientObjectsData.selectedBusinessGroups = e.value;
                    }
                });
            }

            if (s.cpIsNew) {
                refreshTree();
            } else if (s.cpNameRO != null && s.cpNameRO != "") {
                document.getElementById("readOnlyNameSecurityFunction").textContent = s.cpNameRO;
                hasChanges(false);
                ASPxClientEdit.ValidateGroup(null, true);
            } else {
                document.getElementById("readOnlyNameSecurityFunction").textContent = newObjectName;
                hasChanges(true);
                txtName_Client.SetValue(newObjectName);
                onRoleNameChange();
            }

            if (s.cpExistingExternalIds) {
                var hiddenField = document.getElementsByName("ctl00$contentMainBody$hdnExistingExternalIds")[0];
                if (hiddenField) {
                    hiddenField.value = s.cpExistingExternalIds;
                }
            }

            drawPermissions(clientObjectsData.updatedPermissions);
            break;

        case "SAVESECURITYFUNCTION":
            if (s.cpResultRO == 'OK') {
                hasChanges(false);
                refreshTree();
            }
            break;
        default:
            hasChanges(false);
    }
}

function PermissionCallback_CallbackComplete(s, e) {
    showLoadingGrid(false);

    checkResult(s);

    switch (s.cpActionRO) {
        case "UPDATESECURITYFUNCTIONPERMISSION":
            drawPermissions(JSON.parse(s.cpClientControlsRO).updatedPermissions);
            break;
    }
}

function drawPermissions(permissions) {

    for (var i = 0; i < permissions.length; i++) {
        var cPerm = "";
        switch (permissions[i].IDPermission) {
            case 0:
                cPerm = "None";
                break;
            case 3:
                cPerm = "Read";
                break;
            case 6:
                cPerm = "Write";
                break;
            case 9:
                cPerm = "Admin";
                break;
            default:
                break;
        }

        var css = "Permission" + cPerm;
        if (permissions[i].Checked) {
            css += " PermissionPressed";
        } else {
            css += " PermissionUnPressed";
        }

        var strOnClick = "#"
        if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeaturePermission' + cPerm + "_" + permissions[i].IDFeature).css('cursor') != 'not-allowed') {
            if (parseInt(actualSecurityFunction, 10) > 0 || (parseInt(actualSecurityFunction, 10) == 0 && permissions[i].IDPermission > 1)) {
                strOnClick = "UpdFeaturePermission('" + permissions[i].IDFeature + "', '" + cPerm + "')";
            }

            if (strOnClick == "#") {
                $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeaturePermission' + cPerm + "_" + permissions[i].IDFeature).css('cursor', 'not-allowed');
            } else {
                $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeaturePermission' + cPerm + "_" + permissions[i].IDFeature).css('class', 'pointer');
            }

        }
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeaturePermission' + cPerm + "_" + permissions[i].IDFeature).attr('class', css);
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeaturePermission' + cPerm + "_" + permissions[i].IDFeature).attr('onclick', strOnClick)
    }
}

function saveChanges() {
    if (ASPxClientEdit.ValidateGroup(null, true)) {
        grabarSecurityFunction(actualSecurityFunction);
    } else {
        showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
    };
}

function undoChanges() {
    try {
        if (actualSecurityFunction == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesSecurityFunctions";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaSecurityFunction(actualSecurityFunction);
        }
    } catch (e) { showError("undoChanges", e); }
}

function cargaNodo(Nodo) {
    if (Nodo.id == "source") Nodo.id = "-1";
    cargaSecurityFunction(Nodo.id);
}

function cargaSecurityFunction(IdSecurityFunction) {
    actualSecurityFunction = IdSecurityFunction;
    showLoadingGrid(true);
    cargaSecurityFunctionTabSuperior(IdSecurityFunction);
}

function newSecurityFunction() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=SecurityFunction";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newCamera', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaSecurityFunction(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

function copySecurityFunction() {
    try {
        showLoadingGrid(true);
        var Url = "Handlers/srvSecurityFunctions.ashx?action=copySecurityFunction&aTab=" + actualTab + "&ID=" + actualSecurityFunction;
        AsyncCall("POST", Url, "json", "arrstatus", "showLoadingGrid(false);refreshTree();");
    }
    catch (e) {
        showError("copySecurityFunction", e);
    }
}

function cargaSecurityFunctionTabSuperior(IdSecurityFunction) {
    try {
        var Url = "Handlers/srvSecurityFunctions.ashx?action=getSecurityFunctionTab&aTab=" + actualTab + "&ID=" + IdSecurityFunction;
        AsyncCall("POST", Url, "CONTAINER", "divSecurityFunction", "cargaSecurityFunctionBarButtons(" + IdSecurityFunction + ");");
    }
    catch (e) {
        showError("cargaSecurityFunctionTabSuperior", e);
    }
}
var responseObjSec = null;
function cargaSecurityFunctionBarButtons(IdSecurityFunction) {
    try {
        var Url = "Handlers/srvSecurityFunctions.ashx?action=getBarButtons&ID=" + IdSecurityFunction;
        AsyncCall("POST", Url, "JSON3", responseObjSec, "parseResponseBarButtonsSec(objContainerId," + IdSecurityFunction + ")");
    }
    catch (e) {
        showError("cargaSecurityFunctionBarButtons", e);
    }
}

function parseResponseBarButtonsSec(oResponse, IdSecurityFunction) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaSecurityFunctionDivs(IdSecurityFunction);
}

function grabarSecurityFunction(IdSecurityFunction) {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdSecurityFunction;
    oParameters.Name = document.getElementById("readOnlyNameSecurityFunction").textContent.trim();
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.clientControlsData = clientObjectsData;

    oParameters.action = "SAVESECURITYFUNCTION";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaSecurityFunctionDivs(IdSecurityFunction) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdSecurityFunction;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETSECURITYFUNCTION";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_divGeneral', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_divPermissions');

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
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_divGeneral', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_divPermissions');

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

function ShowRemoveSecurityFunction() {
    try {
        if (actualSecurityFunction == "-1" || actualSecurityFunction == "0") { return; }

        var url = "SecurityChart/srvMsgBoxSecurityChart.aspx?action=Message";
        url = url + "&TitleKey=deleteSecurityFunction.Title";
        url = url + "&DescriptionKey=deleteSecurityFunction.Description";
        url = url + "&Option1TextKey=deleteSecurityFunction.Option1Text";
        url = url + "&Option1DescriptionKey=deleteSecurityFunction.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteSecurityFunction('" + actualSecurityFunction + "'); return false;";
        url = url + "&Option2TextKey=deleteSecurityFunction.Option2Text";
        url = url + "&Option2DescriptionKey=deleteSecurityFunction.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveSecurityFunction", e); }
}

function deleteSecurityFunction(Id) {
    try {
        if (Id == "-1" || Id == "0") {
        }
        else {
            try {
                AsyncCall("POST", "Handlers/srvSecurityFunctions.ashx?action=deleteSecurityFunction&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus,true); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
            }
            catch (e) {
                showError('deleteSecurityFunction', e);
            }
        }
    }
    catch (e) {
        showError('deleteSecurityFunction', e);
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
    if (document.getElementById(tip) != null) {
        document.getElementById(tip).style.display = '';
    } else {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_' + tip).style.display = '';
    }
}

function hideTbTip(tip) {
    if (document.getElementById(tip) != null) {
        document.getElementById(tip).style.display = 'none';
    } else {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_' + tip).style.display = 'none';
    }
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesSecurityFunctions";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, false, false);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesSecurityFunctions";
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
        var url = "SecurityChart/srvMsgBoxSecurityChart.aspx?action=Message";
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
    if (oResult.cpResultRO == 'KO') {
        if (oResult.cpActionRO == "SAVESECURITYFUNCTION") {
            hasChanges(true);
        }

        var url = "SecurityChart/srvMsgBoxSecurityChart.aspx?action=Message&TitleKey=SaveName.Error.Text" +
            "&DescriptionText=" + oResult.cpMessage + "" +
            "&Option1TextKey=SaveName.Error.Option1Text" +
            "&Option1DescriptionKey=SaveName.Error.Option1Description" +
            "&Option1OnClickScript=HideMsgBoxForm(); return false;" +
            "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
}

function checkStatus(oStatus, noHasChanges) {
    try {
        arrStatus = oStatus;
        objError = arrStatus[0];

        if (objError.error == "true") {
            if (objError.typemsg == "1") {
                var url = "SecurityChart/srvMsgBoxSecurityChart.aspx?action=Message&TitleKey=SaveName.Error.Text" +
                    "&DescriptionText=" + objError.msg + "" +
                    "&Option1TextKey=SaveName.Error.Option1Text" +
                    "&Option1DescriptionKey=SaveName.Error.Option1Description" +
                    "&Option1OnClickScript=HideMsgBoxForm(); return false;" +
                    "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else {
            }
            if (noHasChanges == null) {
                hasChanges(true);
            }
        }
    } catch (e) { showError("checkStatus", e); }
}

function ShowHideFeatureChilds(idFeature) {
    var tb = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_rowFeatureChilds' + idFeature);
    if (tb != null) {
        if (tb.style.display == '') {
            tb.style.display = 'none';
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeatureOpenImg' + idFeature).src = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeatureOpenImg' + idFeature).src.replace('minus', 'plus');
        } else {
            tb.style.display = '';
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeatureOpenImg' + idFeature).src = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeatureOpenImg' + idFeature).src.replace('plus', 'minus');
        }
    }
}

function ShowHideFeatureInfo(aFeature, idFeature) {
    var _className = 'FeatureInfoPressed';
    var claseObj = new Array();
    claseObj = aFeature.className.split(' ');
    if (claseObj.length > 1) {
        if (claseObj[1] == 'FeatureInfoPressed') {
            _className = 'FeatureInfoUnPressed';
        }
    }
    aFeature.className = claseObj[0] + ' ' + _className;

    var tb = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_rowFeatureInfo' + idFeature);
    if (tb != null) {
        if (_className == 'FeatureInfoPressed')
            tb.style.display = '';
        else
            tb.style.display = 'none';
    }
}

function UpdFeaturePermission(idFeature, Permission) {
    showLoadingGrid(true);

    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = actualSecurityFunction;
    oParameters.params = idFeature + "," + Permission;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "UPDATESECURITYFUNCTIONPERMISSION";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    PermissionCallbackClient.PerformCallback(strParameters);
}

//======================SELECTOR CENTROS DE COSTE ==============

function ShowBusinessCenter() {
    try {
        frmBusinessCenterSelector_Show();
    }
    catch (e) { showError("ShowBusinessCenter", e); }
}

function ASPxBusinessCentersSelectorCallbackPanelContenidoClient_EndCallBack(s, e) {
    if (s.cp_RefreshScreen == "CANCEL") {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmBusinessCenterSelector', false);
    } else if (s.cp_RefreshScreen == "INIT") {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmBusinessCenterSelector', true);
    } else if (s.cp_RefreshScreen == "CLOSEWITHSAVE") {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmBusinessCenterSelector', false);
        cargaSecurityFunction(actualSecurityFunction);
    } else if (s.cp_RefreshScreen == "CLOSECALLPARENT") {
        var bcValues = s.cp_BcValues;
        btnPopupBusinessCentersAcceptClient_Click(bcValues)
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmBusinessCenterSelector', false);
    }

    showLoadingGrid(false);

    lgGridClient.Hide();
    lpSelectorClient.Hide();
    s.cp_RefreshScreen = ""
}

function btnPopupBusinessCentersAcceptClient_Click(values) {
}

function onRoleNameChange() {
    if (txtExport_Client.GetValue() == null) {
        var fullName = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_txtName_I").value;

        var existingShortNames = (document.getElementsByName("ctl00$contentMainBody$hdnExistingExternalIds")[0]?.value || "");

        var shortName = generateShortName(fullName, existingShortNames, 5);

        if (txtExport_Client.GetValue() == null)
            txtExport_Client.SetValue(shortName);
    }
}