var actualTab = 0; // TAB per mostrar
var actualReportScheduler; // ReportScheduler actual
var jsGridDestinations; //Grid Destinations

var newObjectName = "";
var newReportType = 0;

function checkReportSchedulerEmptyName(newName) {
    document.getElementById("readOnlyNameReportScheduler").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    changeTabs(actualTab);
    showLoadingGrid(false);

    ConvertControls('divContent');

    checkResult(s);

    switch (s.cpAction) {
        case "GETREPORTSCHEDULER":
            hasChanges(false);
            GetREPORTSCHEDULER_AFTER(s);
            break;

        case "SAVEREPORTSCHEDULER":
            if (s.cpResult == 'OK') {
                hasChanges(false);
                refreshTree();
            }
            else {
                GetREPORTSCHEDULER_AFTER(s);
                hasChanges(true);
            }

            break;

        default:
            hasChanges(false);
    }
}

function checkResult(oResult) {
    if (oResult.cpResult == 'NOK') {
        if (oResult.cpAction == "SAVEREPORTSCHEDULER") {
            hasChanges(true);
        }

        var url = "ReportScheduler/srvMsgBoxReportScheduler.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
            "DescriptionText=" + oResult.cpMessage + "&" +
            "Option1TextKey=SaveName.Error.Option1Text&" +
            "Option1DescriptionKey=SaveName.Error.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
}

function GetREPORTSCHEDULER_AFTER(s) {
    try {
        if (parseInt(reportTypeClient.Get("ReportType"), 10) == 0) {
            $('#generalReportType').css('display', '');
            $('#emergencyRow').css('display', '');
            $('#reportConfigSection').css('display', '');
            $('#analitycsConfigSection').css('display', 'none');
            $('#employeeReportType').css('display', 'none');
            $('#analyticReportType').css('display', 'none');
        } else if (parseInt(reportTypeClient.Get("ReportType"), 10) == 1) {
            $('#generalReportType').css('display', 'none');
            $('#emergencyRow').css('display', 'none');
            $('#reportConfigSection').css('display', '');
            $('#analitycsConfigSection').css('display', 'none');
            $('#employeeReportType').css('display', '');
            $('#analyticReportType').css('display', 'none');
        } else if (parseInt(reportTypeClient.Get("ReportType"), 10) == 2) {
            $('#generalReportType').css('display', 'none');
            $('#emergencyRow').css('display', 'none');
            $('#reportConfigSection').css('display', 'none');
            $('#analitycsConfigSection').css('display', '');
            $('#employeeReportType').css('display', 'none');
            $('#analyticReportType').css('display', '');
        }

        if (s.cpIsNew == true) {
            refreshTree();
        } else {
            if (s.cpNameRO != null && s.cpNameRO != "") {
                document.getElementById("readOnlyNameReportScheduler").textContent = s.cpNameRO;
                hasChanges(false);
                ASPxClientEdit.ValidateGroup(null, true);
            } else {
                document.getElementById("readOnlyNameReportScheduler").textContent = newObjectName;
                hasChanges(true);
                txtName_Client.SetValue(newObjectName);
            }
        }

        //Cargar Grids
        if (s.cpGridsJSON != null && s.cpGridsJSON.length != 0) {
            var objGrids = JSON.parse(s.cpGridsJSON);
            createGridDestinations(objGrids[0].destinations);
        }

        if (cmbAnalyticType.GetSelectedItem() != null) {
            if (cmbViewClient.GetSelectedItem() != null) {
                setAnalyticsConfVisibility(cmbAnalyticType.GetSelectedItem().value, cmbViewClient.GetSelectedItem().value);
            } else {
                setAnalyticsConfVisibility(cmbAnalyticType.GetSelectedItem().value, -1);
            }
        }
    }
    catch (e) {
        showError("GetREPORTSCHEDULER_AFTER", e);
    }
}

function checkEmergencyStatus(s) {
    if (s.GetSelectedIndex() >= 0) {
        if (s.GetSelectedItem().value.split("#")[2] == "1") {
            document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_chkEmergency").checked = true;
        }
        else {
            document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_chkEmergency").checked = false;
        }

        txtProfile_Client.SetValue("");
        ASPxClientEdit.ValidateGroup(null, true);

        txtProfileData_Client.Set("cpReportProfileID", "");
    }
}

createGridDestinations = function (arrGridDestinations) {
    try {
        var hdGridDestinations = [{ 'fieldname': 'Display', 'description': '', 'size': 'auto' }];
        hdGridDestinations[0].description = document.getElementById('hdnLngDestination').value;

        var edtRow = false;
        var delRow = false;

        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnModeEdit').value != "true") {
            edtRow = true;
            delRow = true;
        }

        jsGridDestinations = new jsGrid('grdDestinations', hdGridDestinations, arrGridDestinations, edtRow, delRow, false, 'Destinations');
    } catch (e) { showError("createGridDestinations", e); }
}

function cargaNodo(Nodo) {
    try {
        $('#reportInfo').css('display', 'none');
        $('#selectReport').css('display', 'none');
        hasChanges(false);

        if (Nodo.id == "source") {
            $('#reportInfo').css('display', '');
            newReportScheduler();
        } else if (Nodo.id == "A" || Nodo.id == "B" || Nodo.id == "C") {
            actualReportScheduler = -2;

            var imageType = "0";
            switch (Nodo.id) {
                case "A":
                    imageType = "0";
                    break;
                case "B":
                    imageType = "1";
                    break;
                case "C":
                    imageType = "2";
                    break;
            }

            var Url = "Handlers/srvReportScheduler.ashx?action=getReportSchedulerTab&aTab=" + actualTab + "&ID=-2&ImageType=" + imageType;
            AsyncCall("POST", Url, "CONTAINER", "divReportScheduler", "cargaReportBarButtonsMin(-1);");
            $('#selectReport').css('display', '');
        } else {
            $('#reportInfo').css('display', '');
            cargaReportScheduler(Nodo.id);
        }
    } catch (e) { showError("cargaNodo", e); }
}

function cargaReportScheduler(IDReportScheduler) {
    try {
        actualReportScheduler = IDReportScheduler;
        //TAB Gris Superior
        showLoadingGrid(true);
        cargaReportSchedulerTabSuperior(IDReportScheduler);
    } catch (e) { showError("cargaReportScheduler", e); }
}

function cargaReportSchedulerTabSuperior(IDReportScheduler) {
    try {
        var Url = "Handlers/srvReportScheduler.ashx?action=getReportSchedulerTab&aTab=" + actualTab + "&ID=" + IDReportScheduler + "&ImageType=" + newReportType;
        AsyncCall("POST", Url, "CONTAINER", "divReportScheduler", "cargaReportBarButtons(" + IDReportScheduler + ");");
    }
    catch (e) {
        showError("cargaReportSchedulerTabSuperior", e);
    }
}

function cargaReportBarButtonsMin(IDReportScheduler) {
    try {
        var Url = "Handlers/srvReportScheduler.ashx?action=getBarButtons&ID=" + IDReportScheduler;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtonsMin(objContainerId," + IDReportScheduler + ")");
    }
    catch (e) {
        showError("cargaReportSchedulerTabSuperior", e);
    }
}
var responseObj = null;
function cargaReportBarButtons(IDReportScheduler) {
    try {
        var Url = "Handlers/srvReportScheduler.ashx?action=getBarButtons&ID=" + IDReportScheduler;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDReportScheduler + ")");
    }
    catch (e) {
        showError("cargaReportSchedulerTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IDReportScheduler) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaReportSchedulerDivs(IDReportScheduler);
}
function parseResponseBarButtonsMin(oResponse, IDReportScheduler) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);
}

function ExecuteNow() {
    try {
        if (actualReportScheduler == "-1" || actualReportScheduler == "0") { return; }

        var url = "ReportScheduler/srvMsgBoxReportScheduler.aspx?action=Message";
        url = url + "&TitleKey=ExecuteReportScheduler.Title";
        url = url + "&DescriptionKey=ExecuteReportScheduler.Description";
        url = url + "&Option1TextKey=ExecuteReportScheduler.Option1Text";
        url = url + "&Option1DescriptionKey=ExecuteReportScheduler.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].executeReportScheduler('" + actualReportScheduler + "'); return false;";
        url = url + "&Option2TextKey=ExecuteReportScheduler.Option2Text";
        url = url + "&Option2DescriptionKey=ExecuteReportScheduler.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/execute_now.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ExecuteNow", e); }
}

function executeReportScheduler(ID) {
    try {
        if (ID == "-1" || ID == "0") { return; }
        var Url = "Handlers/srvReportScheduler.ashx?action=executeReportScheduler&aTab=" + actualTab + "&ID=" + ID;
        AsyncCall("POST", Url, "json", "arrstatus", "checkexecuted(arrstatus);");
    } catch (e) { showError("ExecuteNow", e); }
}

function checkexecuted(oStatus) {
    try {
        arrStatus = oStatus;
        objError = arrStatus[0];

        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "ReportScheduler/srvMsgBoxReportScheduler.aspx?action=Message&TitleKey=ExecuteName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=ExecuteName.Error.Option1Text&" +
                    "Option1DescriptionKey=ExecuteName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            }
        }
    } catch (e) { showError("checkexecuted", e); }
}

function cargaReportSchedulerDivs(IDReportScheduler) {
    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = IDReportScheduler;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETREPORTSCHEDULER";
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

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function hasChanges(bolChanges, markRecalc) {
    var EditMode = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnModeEdit').value

    if (EditMode == "true") { return; }

    var divTop = document.getElementById('divMsgTop');
    var divBottom = document.getElementById('divMsgBottom');

    var tagHasChanges = document.getElementById('msgHasChanges');
    var msgChanges = '<changes>';
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setMessage(msgChanges);

    if (bolChanges) {
        validateReportScheduler(false);

        divTop.style.display = '';
        divBottom.style.display = '';
        document.getElementById('divContentPanels').className = "divContentPanelsWithMessage";
    } else {
        divTop.style.display = 'none';
        divBottom.style.display = 'none';
        document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
    }
}

function validateReportScheduler(bolValidate) {
    try {
        if (bolValidate) {
            var cmbReport = cmbReportClient;
            var cmbProfile = cmbProfileClient;
            var cmbFormat = document.getElementById('cmbFormat_Value');

            if (cmbReport.GetSelectedIndex() < 0) { showErrMsgBox('cmbReportEmpty'); return false; }
            if (cmbProfile.GetSelectedIndex() < 0) { showErrMsgBox('cmbProfileEmpty'); return false; }

            var strError = optSchedule2_validateField('gBox1_optSchedule1');
            if (strError != "") { showErrMsgBox(strError); return false; }

            var arrDestinations = new Array();

            if (jsGridDestinations != null) {
                arrDestinations = jsGridDestinations.toJSONStructure();
                if (arrDestinations == null) {
                    showErrMsgBox('GridEmpty');
                    return false;
                } else {
                    if (arrDestinations.length == 0) { showErrMsgBox('GridEmpty'); return false; }
                }
            } else {
                showErrMsgBox('GridEmpty'); return false;
            }
        }
        return true;
    } catch (e) { showError('validateReportScheduler', e); }
}

function saveChanges() {
    if (parseInt(reportTypeClient.Get("ReportType"), 10) == 2) {
        grabarReportScheduler(actualReportScheduler);
    } else {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            grabarReportScheduler(actualReportScheduler);
        } else {
            showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
        };
    }
}

function grabarReportScheduler(IDReportScheduler) {
    try {
        showLoadingGrid(true);

        //Carreguem els arrays

        var oParameters = {};
        oParameters.aTab = actualTab;
        oParameters.ID = IDReportScheduler;
        oParameters.Name = document.getElementById("readOnlyNameReportScheduler").textContent.trim();
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SAVEREPORTSCHEDULER";

        var oFields = "";
        if (jsGridDestinations != null && parseInt(reportTypeClient.Get("ReportType"), 10) == 0) {
            arrGroups = jsGridDestinations.toJSONStructure();

            if (arrGroups != null) {
                for (var x = 0; x < arrGroups.length; x++) {
                    oFields += arrGroups[x][0].value + "#" + arrGroups[x][1].value + "#" + arrGroups[x][2].value + ";";
                }
                oFields = oFields.substring(0, oFields.length - 1);
            }
        }

        oParameters.gridDestinations = oFields;

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    } catch (e) { showError("grabarReportScheduler", e); }
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
            hasChanges(true);
        } else if (DataType == "6") {
            refreshTree();
        }
    } catch (e) { showError("RefreshScreen", e); }
}

function editProfile() {
    var filename = cmbReportClient.GetSelectedItem().value.split("#")[1];
    var reportType = cmbReportClient.GetSelectedItem().value.split("#")[3];
    var Profile = txtProfileData_Client.Get("cpReportProfileID");
    parent.ShowExternalForm2('Reports/Wizards/ReportProfile.aspx?FileName=' + filename + '&Profile=' + Profile + '&ReportType=' + reportType + '&OnlyGenerateProfile=true', 800, 495, '', '', false, false, false);
}

function refreshProfileData(reportName, reportProfileID) {
    txtProfile_Client.SetValue(reportName);
    if (txtProfileData_Client.Contains("cpReportProfileID")) {
        txtProfileData_Client.Set("cpReportProfileID", reportProfileID);
    } else {
        txtProfileData_Client.Add("cpReportProfileID", reportProfileID);
    }

    ASPxClientEdit.ValidateGroup(null, true);
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function undoChanges() {
    try {
        if (actualReportScheduler == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesReportScheduler";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaReportScheduler(actualReportScheduler);
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
    var ctlPrefix = "ctl00_contentMainBody_roTreesReportScheduler";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesReportScheduler";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "ReportScheduler/srvMsgBoxReportScheduler.aspx?action=Message";
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

var jsGridDestinations; //Grid Destinations

function editGridDestinations(idRow) {
    try {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnModeEdit').value == "true") { return; }
        var arrValues = new Array();
        arrValues = jsGridDestinations.retRowJSON(idRow);

        document.getElementById('hdnAddDestinationIDRow').value = idRow;

        frmAddDestination_Show(arrValues);
    } catch (e) { showError("editGridDestinations", e); }
}

function updateAddDestinationRow(rowID, arrValues) {
    try {
        if (jsGridDestinations == null) { createGridDestinations(); }
        if (rowID == "") {
            jsGridDestinations.createRow(arrValues, true);
        } else {
            jsGridDestinations.editRow(rowID, arrValues);
        }
    } catch (e) { showError("updateAddDestinationRow", e); }
}

function deleteGridDestinations(idRow) {
    try {
        var url = "ReportScheduler/srvMsgBoxReportScheduler.aspx?action=Message";
        url = url + "&TitleKey=deleteDestinationsDef.Title";
        url = url + "&DescriptionKey=deleteDestinationsDef.Description";
        url = url + "&Option1TextKey=deleteDestinationsDef.Option1Text";
        url = url + "&Option1DescriptionKey=deleteDestinationsDef.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelDestinations('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deleteDestinationsDef.Option2Text";
        url = url + "&Option2DescriptionKey=deleteDestinationsDef.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteGridDestinations", e); }
}

function delSelDestinations(idRow) {
    try {
        jsGridDestinations.deleteRow(idRow);
        hasChanges(true);
    } catch (e) { showError("delSelDestinations", e); }
}

function AddNewDestination() {
    try {
        document.getElementById('hdnAddDestinationIDRow').value = "";
        frmAddDestination_ShowNew();
    } catch (e) { showError("AddNewDestination", e); }
}

function newReportScheduler() {
    try {
        newReportType = 0;
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=ReportScheduler";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newNotification', e); }
}

function newReportEmployeeScheduler() {
    try {
        newReportType = 1;
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=EmployeeReportScheduler";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newNotification', e); }
}

function newReportAnalitycsScheduler() {
    try {
        newReportType = 2;
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=EmployeeAnalyticsScheduler";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newNotification', e); }
}

function NewObjectCallback(ObjName) {
    try {
        reportTypeClient.Set("ReportType", newReportType);
        showLoadingGrid(true);
        cargaReportScheduler(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

function deleteReportScheduler(Id) {
    try {
        if (Id != "-1" && Id != "0") {
            AsyncCall("POST", "Handlers/srvReportScheduler.ashx?action=deleteReportScheduler&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
        }
    } catch (e) { showError('deleteReportScheduler', e); }
}

function checkStatus(oStatus) {
    try {
        arrStatus = oStatus;
        objError = arrStatus[0];

        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "ReportScheduler/srvMsgBoxReportScheduler.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else {
            }
            hasChanges(true);
        }
    } catch (e) { showError("checkStatus", e); }
}

function ShowRemoveReportScheduler() {
    try {
        if (actualReportScheduler == "-1" || actualReportScheduler == "0") { return; }

        var url = "ReportScheduler/srvMsgBoxReportScheduler.aspx?action=Message";
        url = url + "&TitleKey=deleteReportScheduler.Title";
        url = url + "&DescriptionKey=deleteReportScheduler.Description";
        url = url + "&Option1TextKey=deleteReportScheduler.Option1Text";
        url = url + "&Option1DescriptionKey=deleteReportScheduler.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteReportScheduler('" + actualReportScheduler + "'); return false;";
        url = url + "&Option2TextKey=deleteReportScheduler.Option2Text";
        url = url + "&Option2DescriptionKey=deleteReportScheduler.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveReportScheduler", e); }
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function showErrMsgBox(idMsg) {
    try {
        showErrorPopup("Error.frmReportScheduler." + idMsg + "Title", "ERROR", "Error.frmReportScheduler." + idMsg + "Desc", "Error.frmReportScheduler.OK", "Error.frmReportScheduler.OKDesc", "");
    } catch (e) { showError("showErrMsgBox", e); }
}

function showMsg(oMsg) {
    alert(oMsg);
}

function CallbackSession_CallbackComplete(s, e) {
    if (e.parameter == "GETINFOSELECTED") {
        var Limit = e.result.indexOf(";");
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnEmployeesSelected').value = e.result.substring(0, Limit);
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_txtEmployees').value = e.result.substring(Limit + 1);
    } else if (e.parameter == "GETANALYTICVIEWS") {
        cmbViewClient.SetSelectedIndex(0);
    } else if (e.parameter == "GETVIEWLAYOUTS") {
        cmbLayoutClient.SetSelectedIndex(0);
    } else if (e.parameter.substring(0, 18) == "GETCONCEPTSBYGROUP") {
        if (e.result != "") {
            var arrConcepts = e.result.split("#")[0].split(",");
            var operation = e.result.split("#")[1];

            if (operation == "1") {
                ListConcepts_Client.SelectValues(arrConcepts);
            }
            else {
                ListConcepts_Client.UnselectValues(arrConcepts);
            }
        }
    } else if (e.parameter.substring(0, 19) == "GETDSFUNCTIONLAYOUT") {
        cmbLayoutClient.cpDSFunction = e.result;
        setAnalyticsConfVisibility();
    }

    showLoadingGrid(false);
}

function btnOpenPopupSelectorEmployeesClient_Click() {
    PopupSelectorEmployeesClient.Show();
}

function btnPopupSelectorEmployeesAcceptClient_Click() {
    showLoadingGrid(true);
    CallbackSessionClient.PerformCallback("GETINFOSELECTED");
    PopupSelectorEmployeesClient.Hide();
    hasChanges(true, false);
}

function setAnalyticsConfVisibility(idConf, idView) {
    $('#analyticEmployeeFilter').hide();
    $('#analyticUserFieldFilter').hide();
    $('#analyticAccrualFilter').hide();
    $('#analyticCostCenterFilter').hide();
    $('#analyticRequestFilter').hide();

    if (cmbLayoutClient.cpDSFunction != '') {
        if (cmbLayoutClient.cpDSFunction.indexOf('employeeFilter') > 0) $('#analyticEmployeeFilter').show();
        if (cmbLayoutClient.cpDSFunction.indexOf('userFieldsFilter') > 0) $('#analyticUserFieldFilter').show();
        if (cmbLayoutClient.cpDSFunction.indexOf('conceptsFilter') > 0) $('#analyticAccrualFilter').show();
        if (cmbLayoutClient.cpDSFunction.indexOf('costCenterFilter') > 0) $('#analyticCostCenterFilter').show();
        if (cmbLayoutClient.cpDSFunction.indexOf('requestTypesFilter') > 0) $('#analyticRequestFilter').show();
    } else {
        if (idConf == undefined) {
            try {
                idConf = cmbAnalyticType.GetSelectedItem().value;
            } catch (e) { }
        }
        if (idView == undefined) {
            try {
                idView = cmbViewClient.GetSelectedItem().value;
            } catch (e) { }
        }

        switch (idConf) {
            case 0: //scheduler
                switch (idView) {
                    case 1:
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        $('#analyticAccrualFilter').show();
                        break;
                    case 2:
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        break;
                    case 3:
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        break;
                    case 4:
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        break;
                    case 5:
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        $('#analyticRequestFilter').show();
                        break;
                    case 6:
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        break;
                    default:
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        $('#analyticAccrualFilter').show();
                        break;
                }
                break;
            case 1: //costcenter
                switch (idView) {
                    case 1:
                        $('#analyticCostCenterFilter').show();
                        break;
                    case 2:
                        $('#analyticCostCenterFilter').show();
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        break;
                    case 3:
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        break;
                    case 4:
                        $('#analyticCostCenterFilter').show();
                        $('#analyticEmployeeFilter').show();
                        $('#analyticUserFieldFilter').show();
                        break;
                    default:
                        $('#analyticEmployeeFilter').show();
                        $('#analyticCostCenterFilter').show();
                        $('#analyticUserFieldFilter').show();
                        break;
                }
                break;
            case 2: //access
                $('#analyticEmployeeFilter').show();
                $('#analyticUserFieldFilter').show();
                break;
            case 3: //productiv
                $('#analyticEmployeeFilter').show();
                $('#analyticUserFieldFilter').show();
                break;
            case 4: //equality_plan
                $('#analyticEmployeeFilter').show();
                $('#analyticUserFieldFilter').show();
                break;
        }
    }
}

//============ COMBO DE LA VISTA SELECCIONADA =======================
function cmbAnalyticType_ValueChanged(s, e) {
    if (s.GetSelectedItem() != null) {
        cmbViewClient.PerformCallback("GETANALYTICVIEWS");
        hasChanges(true, false);
    }
}

function cmbViewClient_ValueChanged(s, e) {
    if (s.GetSelectedItem() != null) {
        setAnalyticsConfVisibility(cmbAnalyticType.GetSelectedItem().value, s.GetSelectedItem().value);
        cmbLayoutClient.PerformCallback("GETVIEWLAYOUTS");
        hasChanges(true, false);
    } else {
        setAnalyticsConfVisibility(cmbAnalyticType.GetSelectedItem().value, -1);
    }
}

function cmbViewClient_EndCallback(s, e) {
    cmbViewClient.SetSelectedIndex(0);
    cmbLayoutClient.PerformCallback("GETVIEWLAYOUTS");
}

function cmbLayoutClient_ValueChanged(s, e) {
    hasChanges(true, false);
    if (s.GetSelectedItem() != null) CallbackSessionClient.PerformCallback("GETDSFUNCTIONLAYOUT#" + s.GetSelectedItem().value);
}

function cmbLayout_EndCallback(s, e) {
    cmbLayoutClient.SetSelectedIndex(0);
    if (s.GetSelectedItem() != null) CallbackSessionClient.PerformCallback("GETDSFUNCTIONLAYOUT#" + s.GetSelectedItem().value);
}
//============ POPUP lISTA de la ficha ========================
function btnOpenPopupUserFields_Click(s, e) {
    PopupUserFieldsClient.Show();
}

function btnPopupUserFieldsAcceptClient_Click(s, e) {
    showLoadingGrid(true);

    var arrUserFieldsValues = ckUserFieldsListClient.GetSelectedValues();
    var hdnUserFieldSelected = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnUserFieldsSelected')

    if (arrUserFieldsValues.length > 0) {
        hdnUserFieldSelected.value = arrUserFieldsValues;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_txtUserFields').value = arrUserFieldsValues.length +
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnUserFieldsSelectedText').value;
    } else {
        hdnUserFieldSelected.value = "";
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_txtUserFields').value = "";
    }
    PopupUserFieldsClient.Hide();
    hasChanges(true, false);
    showLoadingGrid(false);
}

function ckUserFieldsList_SelectedIndexChanged(s, e) {
    //Controlar que no seleccionen más de 10
    var arrUserFieldsValues = ckUserFieldsListClient.GetSelectedValues();
    if (arrUserFieldsValues.length > 10) {
        var arrayindex = new Array();
        arrayindex.push(e.index);
        s.UnselectIndices(arrayindex);
        document.getElementById("spanErrorSelected").style.display = "";
    } else {
        document.getElementById("spanErrorSelected").style.display = "none";
    }
}

function PopupUserFieldsClient_PopUp(s, e) {
}

//============ POPUP lISTA DE SALDOS ====================================
function btnOpenPopupConceptsClient_Click() {
    PopupConceptsClient.Show();
}

function PopupConceptsClient_PopUp(e) {
}

function btnPopupConceptsAcceptClient_Click() {
    showLoadingGrid(true);

    var arrGroupConceptsValues = ListConceptGroups_Client.GetSelectedValues();
    var arrConceptsValues = ListConcepts_Client.GetSelectedValues();

    var hdnGroupConceptsSelected = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnGroupConceptsSelected')
    var hdnConceptsSelected = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnConceptsSelected')

    if (arrConceptsValues.length > 0) {
        hdnGroupConceptsSelected.value = arrGroupConceptsValues;
        hdnConceptsSelected.value = arrConceptsValues;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_txtConcepts').value = arrConceptsValues.length +
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnConceptsSelectedText').value;
    }
    else {
        hdnGroupConceptsSelected.value = "";
        hdnConceptsSelected.value = "";
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_txtConcepts').value = "";
    }
    PopupConceptsClient.Hide();
    hasChanges(true, false);
    showLoadingGrid(false);
}

function ListConceptGroups_SelectedIndexChanged(s, e) {
    if (e.isSelected == true)
        var strParameters = "GETCONCEPTSBYGROUP#" + s.items[e.index].value + "#1";
    else
        var strParameters = "GETCONCEPTSBYGROUP#" + s.items[e.index].value + "#0";

    CallbackSessionClient.PerformCallback(strParameters);
}

//======================SELECTOR tipo de solicitudes ==============
function btnOpenPopupRequestTypes_Click(s, e) {
    PopupRequestTypesClient.Show();
}

function PopupRequestTypesClient_PopUp(s, e) {
}

function btnPopupRequestTypesAcceptClient_Click(s, e) {
    showLoadingGrid(true);

    var arrRequestTypesValues = ckRequestTypesListClient.GetSelectedValues();
    var hdnRequestTypeselected = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnRequestTypesSelected')

    if (arrRequestTypesValues.length > 0) {
        hdnRequestTypeselected.value = arrRequestTypesValues;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_txtRequestTypes').value = arrRequestTypesValues.length +
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnRequestTypesSelectedText').value;
    } else {
        hdnRequestTypeselected.value = "";
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_txtRequestTypes').value = "";
    }
    PopupRequestTypesClient.Hide();
    showLoadingGrid(false);
}

//======================SELECTOR CENTROS DE COSTE ==============

function btnOpenPopupBusinessCenters_Click() {
    frmBusinessCenterSelector_Show();
}

function ASPxBusinessCentersSelectorCallbackPanelContenidoClient_EndCallBack(s, e) {
    if (s.cp_RefreshScreen == "CANCEL") {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_frmBusinessCenterSelector', false);
    } else if (s.cp_RefreshScreen == "INIT") {
        showLoadingGrid(false);
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_frmBusinessCenterSelector', true);
    } else if (s.cp_RefreshScreen == "CLOSEWITHSAVE") {
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_frmBusinessCenterSelector', false);
        refreshTree();
    } else if (s.cp_RefreshScreen == "CLOSECALLPARENT") {
        var bcValues = s.cp_BcValues;
        btnPopupBusinessCentersAcceptClient_Click(bcValues);
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_frmBusinessCenterSelector', false);
    }
    showLoadingGrid(false);
    lgGridClient.Hide();
    lpSelectorClient.Hide();
}

function btnPopupBusinessCentersAcceptClient_Click(values) {
    showLoadingGrid(true);

    var arrBusinessCenterValues = values;
    var hdnBussinesCenterSelected = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnBusinessCentersSelected')

    if (arrBusinessCenterValues.length > 0 && arrBusinessCenterValues[0] !== "") {
        hdnBussinesCenterSelected.value = arrBusinessCenterValues;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_txtBusinessCenters').value = arrBusinessCenterValues.length +
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_hdnBusinessCentersSelectedText').value;
    } else {
        hdnBussinesCenterSelected.value = "";
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_RoGroupBox1_txtBusinessCenters').value = "";
    }
    hasChanges(true, false);
    showLoadingGrid(false);
}