var actualTabActivity = 0; // TAB per mostrar els grups
var actualActivity; // Grup actual

var actualTabDocument = 0; // TAB per mostrar els grups
var actualDocument; // Grup actual

var actualType = "";
var newObjectName = "";
var createType = "D";
var isShowingLoadingMain = 0;

//function ASPxCallbackPanelContenido_Callback(s, e) { }

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    //venableOPC('ASPxCallbackPanelContenido_optExpireOld');
    //venableOPC('ASPxCallbackPanelContenido_optMandatoryDocument');

    //venableOPC('ASPxCallbackPanelContenido_optNoApprove');
    //venableOPC('ASPxCallbackPanelContenido_optApproveRequiered');
    //linkOPCItems('ASPxCallbackPanelContenido_optNoApprove,ASPxCallbackPanelContenido_optApproveRequiered');

    //venableOPC('ASPxCallbackPanelContenido_optAllwaysValid');
    //venableOPC('ASPxCallbackPanelContenido_optValidUntil');
    //linkOPCItems('ASPxCallbackPanelContenido_optAllwaysValid,ASPxCallbackPanelContenido_optValidUntil');

    //venableOPC('ASPxCallbackPanelContenido_roOptPanelExpireOnServer');
    //venableOPC('ASPxCallbackPanelContenido_roOptPanelnoExpire');
    //venableOPC('ASPxCallbackPanelContenido_roOptPanelExpireOnDate');
    //linkOPCItems('ASPxCallbackPanelContenido_roOptPanelExpireOnServer,ASPxCallbackPanelContenido_roOptPanelnoExpire,ASPxCallbackPanelContenido_roOptPanelExpireOnDate');

    if (s.cpActionRO == "GETDOCUMENT" || s.cpActionRO == "SAVEDOCUMENT") {
        if (s.cpResultRO == "OK") {
            if (s.cpActionRO == "SAVEDOCUMENT") {
                parent.RefreshScreen('save', parseInt(s.cpObjectId, 10));
                Close();
            } else {
                if (typeof s.cpIsNewRO != 'undefined' && s.cpIsNewRO == true && typeof s.cpNewObjId != 'undefined' && parseInt(s.cpNewObjId, 10) >= 0) { actualDocument = parseInt(s.cpNewObjId, 10); }

                if (s.cpNameRO != null && s.cpNameRO != "") {
                    var arrF = null;
                    if (s.cpFieldsGridRO != "") eval("arrF = [" + s.cpFieldsGridRO + "]");
                    else arrF = new Array();

                    ASPxClientEdit.ValidateGroup("Document", true);
                }
            }
        } else {
            var result = null;
            eval("result  = [" + s.cpMessageRO + "]");
            if (s.cpNameRO != null && s.cpNameRO != "") {
                ASPxClientEdit.ValidateGroup("Document", true);
            }

            checkStatusDocuments(result);
        }
        reloadUI();
    }
}

function isNew() {
    var hiddenId = hiddenDocumentTemplateID_Client.Get("ID");
    return hiddenId && hiddenId != null && hiddenId != '' && hiddenId == -1;
}

function reloadUI() {
    rblAccessAuthorization_Checked(rblCompanyAccessAuthorization_Client);

    //ckNotification700_Client.SetEnabled(ckCanAddDocumentEmployeeClient.GetChecked());
    ckValidPeriod_Checked(ckValidPeriod_Client);
    rblAmbit_SelectedIndexChanged(rblAmbit_Client);
    rblMandatory_SelectedIndexChanged(rblMandatory_Client);
    ckApproveRequiered_Checked(ckApproveRequiered_Client);
    rbValidUntil_Checked(rbValidUntil_Client);
    //LeaveOrPermission_Checked();

    if (!isNew()) {
        try {
            rblAmbit_Client.SetEnabled(false);
            rblCompany_Client.SetEnabled(false);
            rblCompanyAccessAuthorization_Client.SetEnabled(false);
            rblEmployeeContract_Client.SetEnabled(false);
            rblEmployeeAccessAuthorization_Client.SetEnabled(false);
            rbnNonCriticality_Client.SetEnabled(false);
            rbnAdviceCriticality_Client.SetEnabled(false);
            rbnDeniedCriticality_Client.SetEnabled(false);
            rblLeaveOrPermission_Client.SetEnabled(false);
            rblCauseNote_Client.SetEnabled(false);
        } catch { }
    }
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

function saveChanges() {
    try {
        if (actualType == "D") {
            saveChangesDocument();
        }
    } catch (e) { showError("saveChanges", e); }
}

function changeTabs(numTab) {
    if (actualType == "D") {
        changeDocumentTabs(numTab);
    }
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