var actualTab = 0; // TAB per mostrar
var actualEventScheduler; // Evento actual
var newObjectName = "";
var jsGridAuthorizations;

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    showLoadingGrid(false);

    switch (s.cpActionRO) {
        case "GETEventScheduler":

            if (s.cpResultRO == "OK") {
                //Cargar Grids
                if (s.cpGridsJSON != null && s.cpGridsJSON.length != 0) {
                    var objGrids = JSON.parse(s.cpGridsJSON);
                    createGridAuthorizations(objGrids[0].authorizations);
                }

                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameEventScheduler").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    document.getElementById("readOnlyNameEventScheduler").textContent = newObjectName;
                    hasChanges(true);
                    txtName_Client.SetValue(newObjectName);
                }

                if (s.cpIsNewRO == true) {
                    refreshTree();
                }
            }

            break;
        case "SAVEEventScheduler":

            if (s.cpResultRO == "KO") {
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameEventScheduler").textContent = s.cpNameRO;
                }
                if (s.cpGridsJSON != null && s.cpGridsJSON.length != 0) {
                    var objGrids = JSON.parse(s.cpGridsJSON);
                    createGridAuthorizations(objGrids[0].authorizations);
                }
                hasChanges(true);
                showErrorPopup("Error.Title", "error", "", s.cpErrorRO.ErrorText, "Error.OK", "Error.OKDesc", "");
            }

            break;
    }
}

function cargaNodo(Nodo) {
    try {
        if (Nodo.id.toUpperCase() == "SOURCE") newEventScheduler();
        else cargaEventScheduler(Nodo.id);
    } catch (e) { showError("cargaNodo", e); }
}

function cargaEventScheduler(IDEventScheduler) {
    showLoadingGrid(true);
    actualEventScheduler = IDEventScheduler;
    cargaEventSchedulerTabSuperior(IDEventScheduler);
}

function cargaEventSchedulerTabSuperior(IDEventScheduler) {
    try {
        var parms = { "action": "getEventSchedulerTab", "aTab": actualTab, "ID": IDEventScheduler };
        AjaxCall("POST", "json", "Handlers/srvEventsScheduler.ashx", parms, "CONTAINER", "divEventsScheduler", "cargaEventSchedulerBarButtons(" + IDEventScheduler + ")");
    }
    catch (e) {
        showError("cargaEventSchedulerTabSuperior", e);
    }
}
var responseObj = null;
function cargaEventSchedulerBarButtons(IDEventScheduler) {
    try {
        var Url = "";

        Url = "Handlers/srvEventsScheduler.ashx?action=getBarButtons&ID=" + IDEventScheduler;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDEventScheduler + ")");
    }
    catch (e) {
        showError("cargaEventSchedulerTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IDEventScheduler) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaEventSchedulerDivs(IDEventScheduler);
}

function cargaEventSchedulerDivs(IDEventScheduler) {
    try {
        var oParameters = {};
        oParameters.aTab = actualTab
        oParameters.ID = actualEventScheduler;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "GETEventScheduler";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    } catch (e) { showError("cargaDocumentAbsenceDivs", e); }
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

function btnPopupSelectorDateAcceptClient_Click() {
    try {
        if (actualEventScheduler == -1) {
            showErrorPopup("Info.SelectOrSaveShiftTitle",
                "INFO",
                "Info.SelectOrSaveShiftTitleDescription",
                "",
                "Info.SelectOrSaveShift.Accept",
                "Info.SelectOrSaveShift.AcceptDesc");
            return;
        } else {
            var newDate = moment(txtNewDateClient.GetValue()).format("DD/MM/YYYY");
            showLoadingGrid(true);
            CopyEventPopup_Client.Hide();
            AsyncCall("GET", "Handlers/srvEventsScheduler.ashx?action=copyXEvent&ID=" + actualEventScheduler + "&NewDate=" + newDate, "json", "arrStatus", "showLoadingGrid(false); checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ refreshTree(); }")
        }
    } catch (e) { showError('copyShift', e); }
}

function AddNewAuthorization() {
    try {
        document.getElementById('hdnAddAuthorizationIDRow').value = "";
        var beginDate = moment(txtDateClient.GetValue()).format("DD/MM/YYYY"); // document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_txtDate_I').value;
        var endDate = moment(txtEndDateClient.GetValue()).format("DD/MM/YYYY"); //document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_txtEndDate_I').value;
        frmAddAuthorization_ShowNew(beginDate, endDate);
    } catch (e) { showError("AddNewAuthorization", e); }
}

function updateAddAuthorizationRow(rowID, arrValues) {
    try {
        if (jsGridAuthorizations == null) { createGridAuthorizations(); }
        if (rowID == "") {
            jsGridAuthorizations.createRow(arrValues, true);
        } else {
            jsGridAuthorizations.editRow(rowID, arrValues);
        }
    } catch (e) { showError("updateAddAuthorizationRow", e); }
}

function editGridAuthorizations(idRow) {
    try {
        var arrValues = new Array();
        arrValues = jsGridAuthorizations.retRowJSON(idRow);

        document.getElementById('hdnAddAuthorizationIDRow').value = "";
        var beginDate = moment(txtDateClient.GetValue()).format("DD/MM/YYYY"); // document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_txtDate_I').value;
        var endDate = moment(txtEndDateClient.GetValue()).format("DD/MM/YYYY"); //document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_txtEndDate_I').value;

        document.getElementById('hdnAddAuthorizationIDRow').value = idRow;

        frmAddAuthorization_Show(arrValues, beginDate, endDate);
    } catch (e) { showError("editGridAuthorizations", e); }
}

function deleteGridAuthorizations(idRow) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deleteAuthorizationsDef.Title";
        url = url + "&DescriptionKey=deleteAuthorizationsDef.Description";
        url = url + "&Option1TextKey=deleteAuthorizationsDef.Option1Text";
        url = url + "&Option1DescriptionKey=deleteAuthorizationsDef.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelAuthorizations('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deleteAuthorizationsDef.Option2Text";
        url = url + "&Option2DescriptionKey=deleteAuthorizationsDef.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteGridAuthorizations", e); }
}

function delSelAuthorizations(idRow) {
    try {
        jsGridAuthorizations.deleteRow(idRow);
        hasChanges(true);
    } catch (e) { showError("delSelAuthorizations", e); }
}

createGridAuthorizations = function (arrGridAuthorizations) {
    try {
        var hdGridAuthorizations = [{ 'fieldname': 'Display', 'description': '', 'size': 'auto' }];
        hdGridAuthorizations[0].description = document.getElementById('hdnLngAuthorization').value;

        var edtRow = true;
        var delRow = true;

        jsGridAuthorizations = new jsGrid('grdAuthorizations', hdGridAuthorizations, arrGridAuthorizations, edtRow, delRow, false, 'Authorizations');
    } catch (e) { showError("createGridAuthorizations", e); }
}

function setEventFilter(values) {
    document.getElementById('ctl00_contentMainBody_hdnDates').value = values;
}

function frmAddAuthorization_ShowNew() {
    try {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmAddAuthorization1', true);
    } catch (e) { showError("frmAddAuthorization_ShowNew", e); }
}

function checkEventSchedulerEmptyName(newName) {
    document.getElementById("readOnlyNameEventScheduler").textContent = newName;
    hasChanges(true);
}

function newEventScheduler() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=EventScheduler";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newDocumentAbsence', e); }
}

function copyEventScheduler() {
    try {
        CopyEventPopup_Client.Show();
    } catch (e) { showError('newDocumentAbsence', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaEventScheduler(-1);
        newObjectName = ObjName;
    } catch (e) { showError('newConcept', e); }
}

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function hasChanges(bolChanges) {
    try {
        var divTop = document.getElementById('divMsgTop');
        var divBottom = document.getElementById('divMsgBottom');

        var tagHasChanges = document.getElementById('msgHasChanges');
        var msgChanges = '<changes>';
        if (tagHasChanges != null) {
            msgChanges = tagHasChanges.value;
        }

        setMessage(msgChanges); //'Se han realizado cambios en el acumulado.');

        if (bolChanges) {
            divTop.style.display = '';
            divBottom.style.display = '';
            document.getElementById('divContentPanels').className = "divContentPanelsWithMessage";
        } else {
            divTop.style.display = 'none';
            divBottom.style.display = 'none';
            document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
        }
    } catch (e) { showError("hasChangesEventsScheduler", e); }
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { showError('setMessage', e); }
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function undoChanges() {
    try {
        if (actualEventScheduler == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesEvents";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaEventScheduler(actualEventScheduler);
        }
    } catch (e) { showError("undoChanges", e); }
}

function ShowRemoveEventScheduler() {
    if (actualEventScheduler < 1) { return; }

    var url = "Access/srvMsgBoxAccess.aspx?action=Message";
    url = url + "&TitleKey=deleteEventScheduler.Title";
    url = url + "&DescriptionKey=deleteEventScheduler.Description";
    url = url + "&Option1TextKey=deleteEventScheduler.Option1Text";
    url = url + "&Option1DescriptionKey=deleteEventScheduler.Option1Description";
    url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteEventScheduler('" + actualEventScheduler + "'); return false;";
    url = url + "&Option2TextKey=deleteEventScheduler.Option2Text";
    url = url + "&Option2DescriptionKey=deleteEventScheduler.Option2Description";
    url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function deleteEventScheduler(Id) {
    try {
        AsyncCall("GET", "Handlers/srvEventsScheduler.ashx?action=deleteXEventScheduler&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
    } catch (e) { alert('deleteEventScheduler', e); }
}

function checkStatus(oStatus) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Access/srvMsgBoxAccess.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else { //Missatge estil inline
            }
            hasChanges(true);
        }
    } catch (e) { alert('checkStatus: ' + e); }
}

function RefreshScreen(DataType) {
    if (actualEventScheduler != null && actualEventScheduler > 0) {
        cargaEventScheduler(actualEventScheduler);
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

function refreshTree() {
    eval('ctl00_contentMainBody_roTreesEvents_roTrees.LoadTreeViews(true, true, true);');
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesEvents";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
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

function saveChanges() {
    try {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            var oParameters = {};
            oParameters.aTab = actualTab
            oParameters.ID = actualEventScheduler;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVEEventScheduler";

            var oFields = "";
            if (jsGridAuthorizations != null) {
                arrGroups = jsGridAuthorizations.toJSONStructure();

                if (arrGroups != null) {
                    for (var x = 0; x < arrGroups.length; x++) {
                        oFields += arrGroups[x][0].value + ";" + arrGroups[x][2].value + ".";
                    }
                    oFields = oFields.substring(0, oFields.length - 1);
                }
            }

            oParameters.gridAuthorizations = oFields;

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        };
    } catch (e) { showError("saveChanges", e); }
}