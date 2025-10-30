var actualTab = 0;
var actualAccessGroup;

var jsGridGroups; //Grid
var jsGridEmployees; //Grid

var newObjectName = "";

function checkAccessGroupEmptyName(newName) {
    document.getElementById("readOnlyNameAccessGroup").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    changeTabs(actualTab);
    showLoadingGrid(false);

    ConvertControls('divContent');
    ConvertControls('divAccessGroup');

    checkResult(s);

    switch (s.cpAction) {
        case "GETACCESSGROUP":
            hasChanges(false);
            if (s.cpIsNew == true) {
                refreshTree();
            } else {
                GetAccessGroup_AFTER(s);
            }
            break;

        case "SAVEACCESSGROUP":
            GetAccessGroup_AFTER(s);
            if (s.cpResult == 'OK') {
                hasChanges(false);
                refreshTree();
            } else {
                hasChanges(true);
            }
            break;

        default:
            hasChanges(false);
    }
}

function GetAccessGroup_AFTER(s) {
    try {
        if (s.cpNameRO != null && s.cpNameRO != "") {
            document.getElementById("readOnlyNameAccessGroup").textContent = s.cpNameRO;
            hasChanges(false);
            ASPxClientEdit.ValidateGroup(null, true);
        } else {
            document.getElementById("readOnlyNameAccessGroup").textContent = newObjectName;
            hasChanges(true);
            txtName_Client.SetValue(newObjectName);
        }

        //Esborrem els grids
        if (jsGridGroups != null) { jsGridGroups.destroyGrid(); }
        if (jsGridEmployees != null) { jsGridEmployees.destroyGrid(); }

        //Cargar Grids
        if (s.cpGridsJSON != null && s.cpGridsJSON.length != 0) {
            var objGrids = JSON.parse(s.cpGridsJSON);
            createGridGroups(objGrids[0].groupperms);

            createGridEmployees();
            jsGridEmployees.addRows(objGrids[2].groups, 'idobject');
            jsGridEmployees.addRows(objGrids[1].employees, 'idobject');

            //createGridEmployees(objGrids[1].employees);
        }
    }
    catch (e) {
        showError("GetAccessGroup_AFTER", e);
    }
}

function saveChanges() {
    if (ASPxClientEdit.ValidateGroup(null, true)) {
        grabarAccessGroup(actualAccessGroup);
    } else {
        showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
    };
}

function undoChanges() {
    try {
        if (actualAccessGroup == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesAccessGroups";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaAccessGroup(actualAccessGroup);
        }
    } catch (e) { showError("undoChanges", e); }
}

function cargaNodo(Nodo) {
    if (Nodo.id == "source") newAccessGroup();
    else cargaAccessGroup(Nodo.id);
}

function cargaAccessGroup(IdAccessGroup) {
    actualAccessGroup = IdAccessGroup;
    showLoadingGrid(true);
    cargaAccessGroupTabSuperior(IdAccessGroup);
}

function newAccessGroup() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=AccessGroup";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newAccessGroup', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaAccessGroup(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

var responseObjGroup = null;

function cargaAccessGroupTabSuperior(IDAccessGroup) {
    try {
        var Url = "";

        Url = "Handlers/srvAccessGroups.ashx?action=getAccessGroupTab&aTab=" + actualTab + "&ID=" + IDAccessGroup;
        AsyncCall("POST", Url, "CONTAINER", "divAccessGroup", "");

        Url = "Handlers/srvAccessGroups.ashx?action=getBarButtons&ID=" + IDAccessGroup;
        AsyncCall("POST", Url, "JSON3", responseObjGroup, "parseResponseBarButtonsGroup(objContainerId," + IDAccessGroup + ")");
    }
    catch (e) {
        showError("cargaAccessGroupTabSuperior", e);
    }
}

function parseResponseBarButtonsGroup(oResponse, IDAccessGroup) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaAccessGroupDivs(IDAccessGroup);
}

function grabarAccessGroup(IdAccessGroup) {
    showLoadingGrid(true);
    var ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvAccessGroups.ashx?action=canSaveAccessGroups&ID=" + IdAccessGroup, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            var sResponse = null;
            eval("sResponse = [" + ajax.responseText + "]");
            objError = sResponse[0];

            //Si es un error, mostrem el missatge
            if (objError.error == "true") {
                showLoadingGrid(false);
                if (objError.typemsg == "1") { //Missatge estil pop-up
                    var url = "Access/srvMsgBoxAccess.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                        "DescriptionText=" + objError.msg + "&" +
                        "Option1TextKey=SaveName.Error.Option1Text&" +
                        "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                        "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                    parent.ShowMsgBoxForm(url, 400, 300, '');
                }

                hasChanges(true);
            } else {
                finallyGrabarAccessGroup(IdAccessGroup);
            }
        }
    }

    ajax.send(null)
}

function finallyGrabarAccessGroup(IdAccessGroup) {
    showLoadingGrid(true);

    //Carreguem els arrays
    if (jsGridGroups != null) { arrGroups = jsGridGroups.toJSONStructure(); }
    if (jsGridEmployees != null) { arrEmployees = jsGridEmployees.toJSONStructure(); }

    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdAccessGroup;
    oParameters.Name = document.getElementById("readOnlyNameAccessGroup").innerHTML.trim();
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEACCESSGROUP";

    var idZone = "Error";
    var idAccessPeriod = "Error";

    var oFields = "";
    if (arrGroups != null) {
        for (var x = 0; x < arrGroups.length; x++) {
            for (var groupIndex = 0; groupIndex < arrGroups[x].length; groupIndex++) {
                if (arrGroups[x][groupIndex].field == "IDZONE") {
                    idZone = arrGroups[x][groupIndex].value;
                } else if (arrGroups[x][groupIndex].field == "IDACCESSPERIOD") {
                    idAccessPeriod = arrGroups[x][groupIndex].value;
                }
            }

            oFields += IdAccessGroup + "#" + idZone + "#" + idAccessPeriod + ";";
        }
        oFields = oFields.substring(0, oFields.length - 1);
    }
    oParameters.gridGroups = oFields;
    oParameters.authorized = arrEmployees;

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaAccessGroupDivs(IdAccessGroup) {
    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = IdAccessGroup;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETACCESSGROUP";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div01', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div02');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (n == numTab) {
            if (tab != null) tab.className = 'bTab-active';
            if (div != null) div.style.display = '';
        } else {
            if (tab != null) tab.className = 'bTab';
            if (div != null) div.style.display = 'none';
        }
    }
    actualTab = numTab;
}

function changeTabsByName(nameTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div01', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div02');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (div.id == nameTab) {
            if (tab != null) tab.className = 'bTab-active';
            if (div != null) div.style.display = '';
            actualTab = n;
        } else {
            if (tab != null) tab.className = 'bTab';
            if (div != null) div.style.display = 'none';
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

function ShowRemoveAccessGroup() {
    try {
        if (actualAccessGroup == "-1" || actualAccessGroup == "0") { return; }

        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deleteAccessGroup.Title";
        url = url + "&DescriptionKey=deleteAccessGroup.Description";
        url = url + "&Option1TextKey=deleteAccessGroup.Option1Text";
        url = url + "&Option1DescriptionKey=deleteAccessGroup.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteAccessGroup('" + actualAccessGroup + "'); return false;";
        url = url + "&Option2TextKey=deleteAccessGroup.Option2Text";
        url = url + "&Option2DescriptionKey=deleteAccessGroup.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveAccessGroup", e); }
}

function deleteAccessGroup(Id) {
    try {
        if (Id != "-1") {
            AsyncCall("POST", "Handlers/srvAccessGroups.ashx?action=deleteAccessGroup&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
        }
    } catch (e) { showError('deleteAccessGroup', e); }
}

function copyAccessGroup() {
    try {
        if (actualAccessGroup == -1) {
            showErrorPopup("Info.SelectOrSaveShiftTitle",
                "INFO",
                "Info.SelectOrSaveShiftTitleDescription",
                "",
                "Info.SelectOrSaveShift.Accept",
                "Info.SelectOrSaveShift.AcceptDesc");
            return;
        } else {
            showLoadingGrid(true);
            AsyncCall("GET", "Handlers/srvAccessGroups.ashx?action=copyXAccessGroup&ID=" + actualAccessGroup, "json", "arrStatus", "showLoadingGrid(false); checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ refreshTree(); }")
        }
    } catch (e) { showError('copyShift', e); }
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
    var ctlPrefix = "ctl00_contentMainBody_roTreesAccessGroups";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesAccessGroups";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
            var oEmps;
            if (oParms != "") {
                oEmps = eval(oParms);

                if (jsGridEmployees == null) { createGridEmployees(); }

                for (var i = 0; i < oEmps[0].employees.length; i++) {
                    oEmps[0].employees[i].fields[0].value = '<img src="' + document.getElementById("hdnEmployeeUrl").value + '" />';
                }

                for (var i = 0; i < oEmps[0].groups.length; i++) {
                    oEmps[0].groups[i].fields[0].value = '<img src="' + document.getElementById("hdnGroupUrl").value + '" />';
                }

                jsGridEmployees.addRows(oEmps[0].employees, 'idobject', true);
                jsGridEmployees.addRows(oEmps[0].groups, 'idobject', true);

                hasChanges(true);
            }
        }
    } catch (e) { showError("RefreshScreen", e); }
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
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
        if (oResult.cpAction == "SAVEACCESSGROUP") {
            hasChanges(true);
        }

        var url = "Access/srvMsgBoxAccess.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
            "DescriptionText=" + oResult.cpMessage + "&" +
            "Option1TextKey=SaveName.Error.Option1Text&" +
            "Option1DescriptionKey=SaveName.Error.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
}

function checkStatus(oStatus) {
    try {
        arrStatus = oStatus;
        objError = arrStatus[0];

        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Access/srvMsgBoxAccess.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
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

function ShowNewAccessGroupsWizard() {
    try {
        var Title = '';
        var ID = actualAccessGroup;
        top.ShowExternalForm2('Access/Wizards/AccGroupEmployeeWizard.aspx?IDAccessGroup=' + ID, 500, 450, Title, '', false, false, false);
    } catch (e) { showErrorPopup("ShowNewAccessGroupsWizard", e); }
}

//== GRIDS =========================

//*****************************************************************************************/
// createGridGroups
// Carrega del Grids (Groups)
//********************************************************************************************/
function createGridGroups(arrGridGroups) {
    try {
        //Grid Grups Access
        var hdGridGroup = [{ 'fieldname': 'ZoneName', 'description': '', 'size': '50%' },
        { 'fieldname': 'PeriodName', 'description': '', 'size': '50%' }];

        hdGridGroup[0].description = document.getElementById('hdnStrZoneName').value;
        hdGridGroup[1].description = document.getElementById('hdnStrPeriodName').value;

        var edtRow = false;
        var delRow = false;

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "true") {
            edtRow = true;
            delRow = true;
        }

        jsGridGroups = new jsGrid('ctl00_contentMainBody_ASPxCallbackPanelContenido_grdGroups', hdGridGroup, arrGridGroups, edtRow, delRow, false, 'Groups');
    } catch (e) { showError("createGridGroups", e); }
}

//*****************************************************************************************/
// createGridGroups
// Carrega del Grids (Groups)
//********************************************************************************************/
function createGridEmployees(arrGridEmployees) {
    try {
        //Grid Usuaris
        var hdGridEmp = [{ 'fieldname': 'Icon', 'description': '', 'size': '20px', html:true },
        { 'fieldname': 'Name', 'description': '', 'size': '100%' }];

        hdGridEmp[0].description = '';
        hdGridEmp[1].description = document.getElementById('hdnStrEmpName').value;

        var edtRow = false;
        var delRow = false;

        if (document.getElementById('ctl00_contentMainBody_hdnModeEditEmployees').value != "true") {
            edtRow = true;
            delRow = true;
        }

        jsGridEmployees = new jsGrid('ctl00_contentMainBody_ASPxCallbackPanelContenido_grdEmployees', hdGridEmp, arrGridEmployees, false, delRow, false, 'Employees');
    } catch (e) { showError("createGridGroups", e); }
}

//Creacion de grupo
function AddNewGridGroup() {
    try {
        document.getElementById('hdnAddAccPermissionIDRow').value = "";
        frmNewAccPermission_ShowNew();
    } catch (e) { showError("AddNewGridGroup", e); }
}

//Edicion de grupo
function editGridGroups(idRow) {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true") { return; }
        var arrValues = new Array();
        arrValues = jsGridGroups.retRowJSON(idRow);

        document.getElementById('hdnAddAccPermissionIDRow').value = idRow;

        frmNewAccPermission_Show(arrValues);
    } catch (e) { showError("editGridGroups", e); }
}

//Eliminación de grupo
function deleteGridGroups(idRow) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deleteGroupsDef.Title";
        url = url + "&DescriptionKey=deleteAccGroupsDef.Description";
        url = url + "&Option1TextKey=deleteAccGroupsDef.Option1Text";
        url = url + "&Option1DescriptionKey=deleteAccGroupsDef.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelGroup('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deleteAccGroupDef.Option2Text";
        url = url + "&Option2DescriptionKey=deleteAccGroupDef.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteGridGroups", e); }
}

function delSelGroup(idRow) {
    try {
        jsGridGroups.deleteRow(idRow);
        hasChanges(true);
    } catch (e) { showError("delSelGroup", e); }
}

//Eliminación de empleado
function deleteGridEmployees(idRow) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deleteEmployeeDef.Title";
        url = url + "&DescriptionKey=deleteEmployeeDef.Description";
        url = url + "&Option1TextKey=deleteEmployeeDef.Option1Text";
        url = url + "&Option1DescriptionKey=deleteEmployeeDef.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelEmployee('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deleteEmployeeDef.Option2Text";
        url = url + "&Option2DescriptionKey=deleteEmployeeDef.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteGridEmployees", e); }
}

function delSelEmployee(idRow) {
    try {
        jsGridEmployees.deleteRow(idRow);
        hasChanges(true);
    } catch (e) { showError("delSelEmployee", e); }
}

//updateAddGroupRow : Actualitza el registre al grid (afegeix, modifica)
function updateAddAccPermissionRow(rowID, arrValues) {
    try {
        if (jsGridGroups == null) { createGridGroups(); }
        if (rowID == "") {
            rowID = jsGridGroups.createRow(arrValues, true);
        } else {
            jsGridGroups.editRow(rowID, arrValues);
        }
    } catch (e) { showError("updateAddAccPermissionRow", e); }
}

function EmptyAccessGroup() {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=EMPTYACCESSGROUP";
    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
}

function captchaCallback(action) {
    switch (action) {
        case "EMPTYACCESSGROUP":
            try {
                if (actualAccessGroup > 0) {
                    AsyncCall("POST", "Handlers/srvAccessGroups.ashx?action=emptyAccessGroupEmp&ID=" + actualAccessGroup, "json", "arrStatus", "checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ undoChanges(); }")
                }
            } catch (e) { showError('emptyAccessGroupEmp', e); }
            break;
        case "ERROR":
            window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
            break;
    }
}

function AddDocumentsToAuthorization() {
    gridDocumentsAuthorizedClient.AddNewRow();
}

function gridDocumentsAuthorized_EndCallback(s, e) {
    if (!s.IsEditing()) {
        hasChanges(true);
    }
}