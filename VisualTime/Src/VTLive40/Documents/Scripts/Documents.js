var actualTabActivity = 0; // TAB per mostrar els grups
var actualActivity; // Grup actual

var actualTabDocument = 0; // TAB per mostrar els grups
var actualDocument; // Grup actual

var actualType = "";
var newObjectName = "";
var createType = "D";
var isShowingLoadingMain = 0;

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    ConvertControls('divContenido');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optExpireOld');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optMandatoryDocument');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoApprove');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optApproveRequiered');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optNoApprove,ctl00_contentMainBody_ASPxCallbackPanelContenido_optApproveRequiered');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAllwaysValid');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optValidUntil');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAllwaysValid,ctl00_contentMainBody_ASPxCallbackPanelContenido_optValidUntil');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptPanelExpireOnServer');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptPanelnoExpire');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptPanelExpireOnDate');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptPanelExpireOnServer,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptPanelnoExpire,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptPanelExpireOnDate');

    if (actualType == "D") {
        if (s.cpActionRO == "GETDOCUMENT" || s.cpActionRO == "SAVEDOCUMENT") {
            //txtProjectName_Client.SetValue("___");

            if (s.cpResultRO == "OK") {
                if (typeof s.cpIsNewRO != 'undefined' && s.cpIsNewRO == true && typeof s.cpNewObjId != 'undefined' && parseInt(s.cpNewObjId, 10) >= 0) { actualDocument = parseInt(s.cpNewObjId, 10); }

                if (s.cpNameRO != null && s.cpNameRO != "") {
                    var arrF = null;
                    if (s.cpFieldsGridRO != "") eval("arrF = [" + s.cpFieldsGridRO + "]");
                    else arrF = new Array();

                    document.getElementById("readOnlyNameDocumentTemplate").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup("Document", true);
                } else {
                    document.getElementById("readOnlyNameDocumentTemplate").textContent = newObjectName;
                    hasChanges(true);
                    txtDocName_Client.SetValue(newObjectName);

                    rblScope_SelectedIndexChanged(rblScope_Client);
                }
            } else {
                var result = null;
                eval("result  = [" + s.cpMessageRO + "]");
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameDocumentTemplate").textContent = s.cpNameRO;
                    ASPxClientEdit.ValidateGroup("Document", true);
                } else {
                    document.getElementById("readOnlyNameDocumentTemplate").textContent = newObjectName;
                    txtDocName_Client.SetValue(newObjectName);
                }
                var arrF = null;
                hasChanges(true);

                checkStatusDocuments(result);
            }
        }
    }
    showLoadingGrid(false);
}

function cargaNodo(Nodo) {
    if (Nodo.id.toUpperCase() == "SOURCE") {
        newProject();
        return;
    }

    var id = Nodo.id.substring(1);
    if (Nodo.id.substring(0, 1) == "D") {
        cargaDocumentTemplate(id);
    }
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function hasChanges(bolChanges) {
    if (actualType == "A") {
        hasChangesActivity(bolChanges);
    } else if (actualType == "D") {
        hasChangesDocument(bolChanges);
    }
}

function saveChanges() {
    try {
        if (actualType == "D") {
            saveChangesDocument();
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChanges() {
    try {
        if (actualType == "D") {
            undoChangesDocument();
        }
    } catch (e) { showError("undoChanges", e); }
}

function changeTabs(numTab) {
    if (actualType == "D") {
        changeDocumentTabs(numTab);
    }
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        msgTop.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Documents/srvMsgBoxDocument.aspx?action=Message";
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

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        if (createType == "D") {
            cargaDocumentTemplate(-1);
        }
        newObjectName = ObjName;
    } catch (e) { showError('newTemplate', e); }
}

function showPopupLoader() {
    if (isShowingLoadingMain == 0) {
        if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
            window.parent.frames["ifPrincipal"].showLoadingGrid(true);
        } else {
            window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
        }
        isShowingLoadingMain += 1;
    }
}

function hidePopupLoader(bForceHide) {
    isShowingLoadingMain -= 1;
    if (isShowingLoadingMain <= 0 || bForceHide) {
        isShowingLoadingMain = 0;
        if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
            window.parent.frames["ifPrincipal"].showLoadingGrid(false);
        } else {
            window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
        }
    }
}