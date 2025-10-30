var iEditItemID = -1;
var actualTab = 'TABBUTTON_General'; // TAB per mostrar
var actualSupervisor; // Passport actual
var jsGridIPs = null;
var jsGridCategories;//Grid
var tmpGroupsSelected = '';
var oldParams = ''
var undo = false;
var oldSupervisor = '';
var isShowingLoadingMain = 0;

function showPopupLoader() {
    if (isShowingLoadingMain == 0) {
        if (typeof (window.parent.parent.frames["ifPrincipal"]) != "undefined") {
            window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
        } else {
            window.parent.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
        }
        isShowingLoadingMain += 1;
    }
}

function hidePopupLoader(bForceHide) {
    isShowingLoadingMain -= 1;
    if (isShowingLoadingMain <= 0 || bForceHide) {
        isShowingLoadingMain = 0;
        if (typeof (window.parent.parent.frames["ifPrincipal"]) != "undefined") {
            window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
        } else {
            window.parent.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
        }
    }
}

function checkSupervisorEmptyName(newName) {
    document.getElementById("ASPxCallbackPanelContenido_txtName_I").textContent = newName;
    hasChanges(true);
}

function refreshSupervisorPanels(s, e) {
    supervisorLoaded = true;
    hasChanges(false);
    supervisorLoaded = false;
    ConvertControls('divContent');
    LoadPassportCategories(s);

    venableOPC('ASPxCallbackPanelContenido_cnIdentifyMethods_chkUsername');
    var isAdminObj = document.getElementById('ASPxCallbackPanelContenido_cnIdentifyMethods_chkUsername_hdnMustActivateApplicationAccess');
    if (isAdminObj != null) {
        if (isAdminObj.value == "0" && document.getElementById('ASPxCallbackPanelContenido_cnIdentifyMethods_chkUsername_txtUserName').value != "") {
            var table = document.getElementById('ASPxCallbackPanelContenido_cnIdentifyMethods_chkUsername_panOptionPanel');
            var checkObj = null;
            if (table.getAttribute("vmode") == "0") {
                checkObj = document.getElementById("ASPxCallbackPanelContenido_cnIdentifyMethods_chkUsername_rButton");
            } else {
                checkObj = document.getElementById("ASPxCallbackPanelContenido_cnIdentifyMethods_chkUsername_chkButton");
            }
            checkObj.disabled = true;
        }
    }

    //venableOPC('cnIdentifyMethods_chkIntegrated');
    venableOPC('ASPxCallbackPanelContenido_cnIdentifyMethods_chkCard');
    venableOPC('ASPxCallbackPanelContenido_cnIdentifyMethods_chkNFC');
    venableOPC('ASPxCallbackPanelContenido_cnIdentifyMethods_chkBiometric');
    venableOPC('ASPxCallbackPanelContenido_cnIdentifyMethods_chkPin');
    venableOPC('ASPxCallbackPanelContenido_cnIdentifyMethods_chkPlate');

    venableOPC('ASPxCallbackPanelContenido_optBGListAll');
    venableOPC('ASPxCallbackPanelContenido_optBGListValue');
    linkOPCItems('ASPxCallbackPanelContenido_optBGListAll,ASPxCallbackPanelContenido_optBGListValue');

    SetFunctionality('ASPxCallbackPanelContenido_cnIdentifyMethods');

    checkMustActivateBlock('ASPxCallbackPanelContenido_cnIdentifyMethods');

    var pwd;
    pwd = document.getElementById('ASPxCallbackPanelContenido_cnIdentifyMethods_chkUsername_txtPassword');
    if (pwd != null) pwd.value = RoboticsJsUtils.decryptString(pwd.getAttribute("valuePwd"));
    pwd = document.getElementById('ASPxCallbackPanelContenido_cnIdentifyMethods_chkPin_txtPin');
    if (pwd != null) pwd.value = RoboticsJsUtils.decryptString(pwd.getAttribute("valuePwd"));

    //roCB_addComboHandler('ASPxCallbackPanelContenido_cmbLanguage_DropDown');
    roCB_addComboHandler('ASPxCallbackPanelContenido_cnIdentifyMethods_cmbFunctionality_DropDown');
    //roCB_addComboHandler('ASPxCallbackPanelContenido_cmbLevelOfAuthority_DropDown');

    document.onclick = roCB_hideAllCombos;

    var divLevelOfAuthority = document.getElementById('divSliderLevelOfAuthority');
    var hdnLevelOfAuthority = document.getElementById('hdnLevelOfAuthority');
    if (divLevelOfAuthority != null && hdnLevelOfAuthority != null) {
        hdnLevelOfAuthority.value = divLevelOfAuthority.getAttribute("CCvalue");
    }

    var hdGridIPs = [{ 'fieldname': 'value', 'description': '', 'size': '100%' }];
    hdGridIPs[0].description = document.getElementById('hdnValueGridName').value;

    var allowedIPs = document.getElementById("ASPxCallbackPanelContenido_groupAccessRestrictions_ChkRestrictedIP_txtAllowedIPs").value;
    var arrGridIPs = [];
    if (allowedIPs != null) {
        var ips = allowedIPs.split('#');
        for (var i = 0; i < ips.length; i++) {
            if (ips[i] != "") {
                arrGridIPs.push({ fields: [{ field: 'id', value: i }, { field: 'value', value: ips[i] }] });
            }
        }

        jsGridIPs = new jsGrid('ASPxCallbackPanelContenido_groupAccessRestrictions_ChkRestrictedIP_gridAllowedIPs', hdGridIPs, arrGridIPs, s.cpCanModifyAddress, s.cpCanModifyAddress, false, 'AllowedIPs');

        if (allowedIPs == "") {
            disableChildElements('ASPxCallbackPanelContenido_groupAccessRestrictions_ChkRestrictedIP_panContainer');
        }
    }

    ASPxClientEdit.ValidateGroup(null, true);

    supervisorLoaded = true;

    ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_roTrees.LoadTreeViews(true, false, false);
    var groups = document.getElementById('hdnTreeGroups');
    ASPxCallbackPanelContenido_objContainerTreeV3_newItemInGrid(groups.value);
    hasChanges(false);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    iEditItemID = -1
    var contenedor2;
    contenedor2 = document.getElementById('divContent');
    changeTabs(actualTab);
    showLoadingGrid(false);

    $("#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_tabTree02").hide();
    $("#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_showEmployeeFilter").hide();
    $("#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_btn5Filter").hide();
    $("#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_tree-div").css("width", "inherit");
    $("#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_tree-div").css("max-width", "284px");
    $("#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_tree-div").css("height", "285px");
    $("#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_tree-div3").css("max-height", "195px");
    $("#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_tree-div3").css("max-width", "275px");
    $("#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_tree-div-FF").css("height", "285px");

    if (s.cpActionRO == "GETPASSPORT") {
        refreshSupervisorPanels(s, e);
    } else if (s.cpActionRO == "SAVEPASSPORT") {
        if (s.cpResultRO.substring(0, 2) != 'OK') {
            var type = s.cpResultRO.substr(0, 7)
            var message = s.cpResultRO.substr(7, s.cpResultRO.length - 7)
            var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(message);
            parent.parent.ShowMsgBoxForm(url, 500, 300, '');

            if (type == "INFOMSG") {
                hasChanges(false);
                cargaPassport(actualSupervisor);
            } else {
                refreshSupervisorPanels(s, e);
                hasChanges(true);
            }
        } else {
            refreshSupervisorPanels(s, e);
            hasChanges(false);
            refreshParentCardTree(actualSupervisor);
        }
    }
}

function refreshParentCardTree(supervisor) {
    window.parent.window.refreshCardTree(supervisor);
}

function showParentLoading(state) {
    parent.window.parent.window.showLoadingGrid(state);
}

function ASPxOrgChartClientEndCallBack(s, e) {
    switch (s.cpAction) {
        case "GETSECURITYCHART":
            if (checkResult(s)) {
                orgChart.parseResponse(s, e);
            }
            break;
        case "SAVESECURITYCHART":

        default:
    }
}

function checkResult(oResult) {
    if (oResult.cpResult == 'KO') {
        showErrorPopup("SaveName.Error.Text", "ERROR", "", oResult.cpMessage, "SaveName.Error.Option1Text", "SaveName.Error.Option1Description", "", "", "", "", "", "", "");
        return false;
    }
    return true;
}

function hasChanges(bolChanges, markRecalc) {
    if (typeof (supervisorLoaded) != 'undefined' && supervisorLoaded == true) {
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
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function saveChanges() {
    if (ASPxClientEdit.ValidateGroup(null, true)) {
        grabarPassport(actualSupervisor);
    } else {
        showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
    };
}

function undoChanges() {
    cargaPassport(actualSupervisor);
    hasChanges(false);
    undo = true;
}

function cargaNodo(Nodo) {
    if (Nodo.id != "source") {
        cargaPassport(Nodo.id);
    } else {
        cargaPassport(-1);
    }
}

//Carga Tabs y contenido Empleados
function cargaPassport(IDPassport) {
    supervisorLoaded = false;
    actualSupervisor = IDPassport;
    //TAB Gris Superior
    showLoadingGrid(true);
    cargaPassportTabSuperior(IDPassport);
}
var responseObj = null;
// Carrega la part del TAB grisa superior
function cargaPassportTabSuperior(IDPassport) {
    try {
        var Url = "";

        Url = "Handlers/srvSupervisorsV3.ashx?action=getBarButtons&ID=" + IDPassport;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDPassport + ")")
    }
    catch (e) {
        showError("cargaPassportTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IDPassport) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.parent.setUPReportsAndWizards(oResponse);

    cargaPassportDivs(IDPassport);
}

function EnableIpOpc() {
    //venableOPC('ASPxCallbackPanelContenido_groupAccessRestrictions_ChkRestrictedIP');
}

function grabarPassport(IDPassport) {
    hdnGroupsConfigClient.Set('GroupsSelected', tmpGroupsSelected);

    if (CheckConvertControls('') == true) {
        showLoadingGrid(true);
        if (jsGridIPs != null) {
            //Bucle per totes les composicions
            var ipRows = jsGridIPs.getRows();
            if (ipRows != null) {
                var arrAllowdIp = "";
                for (var x = 0; x < ipRows.length; x++) {
                    //Bucle per els camps
                    if (x != 0) arrAllowdIp = arrAllowdIp + "#";
                    arrAllowdIp = arrAllowdIp + jsGridIPs.retRowJSON(ipRows[x].id)[1].value
                }
                if (arrAllowdIp != "") {
                    document.getElementById("ASPxCallbackPanelContenido_groupAccessRestrictions_ChkRestrictedIP_txtAllowedIPs").value = arrAllowdIp;
                }
            }
        }

        var oParameters = {};
        oParameters.aTab = actualTab
        oParameters.ID = IDPassport;
        oParameters.gridCategories = actualDxGridCategories;
        oParameters.gridUsers = gridUsersDS;
        oParameters.groups = tmpGroupsSelected;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SAVEPASSPORT";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    }
}

// Carrega els apartats dels divs de l'usuari
function cargaPassportDivs(IDPassport) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IDPassport;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETPASSPORT";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

//Cambia els Tabs i els divs
function changeTabs(IDTab) {
    arrDivs = new Array('ASPxCallbackPanelContenido_divGeneral', 'ASPxCallbackPanelContenido_divRestricciones', 'ASPxCallbackPanelContenido_divIdentifyMethods', 'ASPxCallbackPanelContenido_divActiveNotifications', 'ASPxCallbackPanelContenido_divAllowedApplications', 'ASPxCallbackPanelContenido_divConsents');

    for (n = 0; n < arrDivs.length; n++) {
        var div = document.getElementById(arrDivs[n]);
        if (n == IDTab) {
            if (div != null) div.style.display = '';
        } else {
            if (div != null) div.style.display = 'none';
        }
    }

    actualTab = IDTab;
}

function editgridUsers(e) {
    try {
        document.getElementById('hdnAddRequestCategoryIDRow').value = e;
        frmNewUserException_Show(e);
        setTimeout(function () { dxgridUsers.cancelEditData(); }, 200);
    } catch (e) { showError("editgridUsers", e); }
}

function frmNewUserException_Show(e) {
    try {
        cmbPassportAvailableClient.SetEnabled(true);
        cmbPassportAvailableClient.SetValue(null);
        
        showWUF('ASPxCallbackPanelContenido_frmNewUserException1', true);
    } catch (e) { showError("frmNewUserException_Show", e); }
}

function saveUserException() {
    try {
        var item = cmbPassportAvailableClient.GetSelectedItem();

        if (gridUsersDS == null) gridUsersDS = [];

        gridUsersDS.push({
            IDPassport: -1,
            IDEmployee: item.value,
            Name: item.text
        });
        dxgridUsers.option('dataSource', { store: gridUsersDS });
        
        hasChanges(true);
        closeUserException();
    } catch (e) { showError("saveUserException", e); }
}

function closeUserException() {
    try {
        //show te form
        showWUF('ASPxCallbackPanelContenido_frmNewUserException1', false);
    } catch (e) { showError("frmNewUserException_Close", e); }
}

function editgridCategories(e) {
    try {
        document.getElementById('hdnAddRequestCategoryIDRow').value = e;

        frmNewRequestCategory_Show(e);

        setTimeout(function () { dxgridCategories.cancelEditData(); }, 200);
    } catch (e) { showError("editGridCategories", e); }
}

function frmNewRequestCategory_Validate() {
    try {
        if (cmbRequestCategoryClient.GetSelectedItem().value == -1) {
            showErrorPopup("Error.frmNewRequestCategory.cmbCategoryTitle", "ERROR", "Error.frmNewRequestCategory.cmbCategoryTitleDesc", "Error.frmNewRequestCategory.OK", "Error.frmNewRequestCategory.OKDesc", "");
            return false;
        }

        if (cmbLevelClient.GetSelectedItem().value == -1) {
            showErrorPopup("Error.frmNewRequestCategory.cmbLevelTitle", "ERROR", "Error.frmNewRequestCategory.cmbLevelTitleDesc", "Error.frmNewRequestCategory.OK", "Error.frmNewRequestCategory.OKDesc", "");
            return false;
        }

        if (cmbNextLevelClient.GetSelectedItem().value == -1) {
            showErrorPopup("Error.frmNewRequestCategory.cmbNextLevelTitle", "ERROR", "Error.frmNewRequestCategory.cmbNextLevelTitleDesc", "Error.frmNewRequestCategory.OK", "Error.frmNewRequestCategory.OKDesc", "");
            return false;
        }

        if (parseInt(cmbNextLevelClient.GetSelectedItem().value) < parseInt(cmbLevelClient.GetSelectedItem().value)) {
            showErrorPopup("Error.frmNewRequestCategory.cmbNextLevelTitle", "ERROR", "Error.frmNewRequestCategory.cmbNextLevelInferior", "Error.frmNewRequestCategory.OK", "Error.frmNewRequestCategory.OKDesc", "");

            return false;
        }

        return true;
    } catch (e) {
        showError("frmNewRequestCategory_Validate", e);
        return false;
    }
}

//Carrega de tots els camps en un objecte JSON
function AddNewRequestCategory_FieldsToJSON() {
    try {
        var IDCategory;
        var CategoryName;
        var IDLevel;
        var LevelName;
        var IDNextLevel;
        var NextLevelName;

        var item = cmbRequestCategoryClient.GetSelectedItem();
        IDCategory = item.value;
        CategoryName = item.text;
        item = cmbLevelClient.GetSelectedItem();
        IDLevel = item.value;
        LevelName = item.text;
        item = cmbNextLevelClient.GetSelectedItem();
        IDNextLevel = item.value;
        NextLevelName = item.text;

        var oAtts = [{ 'attname': 'idcategory', 'value': IDCategory },
        { 'attname': 'categoryname', 'value': CategoryName },
        { 'attname': 'idlevel', 'value': IDLevel },
        { 'attname': 'levelname', 'value': LevelName },
        { 'attname': 'idnextlevel', 'value': IDNextLevel },
        { 'attname': 'nextlevelname', 'value': NextLevelName }
        ];

        return oAtts;
    } catch (e) {
        showError("AddNewRequestCategory_FieldsToJSON", e);
        return null;
    }
}

function frmNewRequestCategory_Show(idRow) {
    try {
        iEditItemID = idRow.data.ID;
        cmbRequestCategoryClient.SetEnabled(false);
        cmbRequestCategoryClient.SetValue(-1);
        cmbLevelClient.SetValue(-1);
        cmbNextLevelClient.SetValue(-1);

        cmbRequestCategoryClient.SetValue(idRow.data.IDCategory);

        cmbLevelClient.SetValue(idRow.data.LevelOfAuthority);

        cmbNextLevelClient.SetValue(idRow.data.ShowFromLevel);

        showWUF('ASPxCallbackPanelContenido_frmNewRequestCategory1', true);
    } catch (e) { showError("frmNewRequestCategory_Show", e); }
}

function frmNewRequestCategory_Save() {
    try {
        if (frmNewRequestCategory_Validate()) {
            var item = cmbRequestCategoryClient.GetSelectedItem();
            var itemLevel = cmbLevelClient.GetSelectedItem();
            var itemNext = cmbNextLevelClient.GetSelectedItem();
            var bAlreadyExists = false;
            var category = {
                ID: -1,
                IDCategory: item.value,
                CategoryName: item.text,
                LevelOfAuthority: itemLevel.value,
                ShowFromLevel: itemNext.value
            }

            if (actualDxGridCategories.length == 0) {
                category.ID = 1
            }
            else {
                if (iEditItemID != -1) {
                    var tmpItem = actualDxGridCategories.find(function (x) {
                        return x.IDCategory == item.value;
                    });

                    category.ID = tmpItem.ID;
                } else {
                    var tmpItem = actualDxGridCategories.find(function (x) {
                        return x.IDCategory == item.value;
                    });

                    if (typeof tmpItem != 'undefined') {
                        bAlreadyExists = true;
                    } else {
                        var tmpItem = actualDxGridCategories.max(false, function (n) {
                            return n.ID;
                        });
                        category.ID = tmpItem.ID + 1;
                    }
                }
            }

            if (actualDxGridCategories == null) {
                actualDxGridCategories.push(category);
                dxgridCategories.option('dataSource', { store: actualDxGridCategories });
            }
            else {
                if (!bAlreadyExists) {
                    actualDxGridCategories = actualDxGridCategories.remove(function (x) { return x.ID == category.ID; });
                    actualDxGridCategories.push(category);
                    dxgridCategories.option('dataSource', { store: actualDxGridCategories });
                } else {
                    showErrorPopup("Error.frmNewRequestCategory.cmbNextLevelTitle", "ERROR", "Error.frmNewRequestCategory.alreadyExists", "Error.frmNewRequestCategory.OK", "Error.frmNewRequestCategory.OKDesc", "");
                }
            }

            hasChanges(true);
            frmNewRequestCategory_Close();
        }
    } catch (e) { showError("frmNewRequestCategory_Save", e); }
}

//Nou usuari wizard
function ShowNewPassportWizard(IDCurrentPassport) {
    var Title = '';
    top.ShowExternalForm2('Security/Wizards/NewPassportWizard.aspx?IDCurrentPassport=' + IDCurrentPassport, 500, 450, Title, '', false, false, false);
    //refreshParentCardTree(IDCurrentPassport); //todo: pasarle el nuevo id passport creado
}

//Mostra el MsgBoxForm per confirmar eliminacio de l'usuari
function ShowRemovePassport(ID) {
    var url = "Security/srvMsgBoxPassports.aspx?action=DeletePassport&ID=" + ID + "&SupervisorsFrame=1"; //modificar esta url??
    parent.parent.ShowMsgBoxForm(url, 400, 300, '');
}

function AddNewRequestCategory() {
    try {
        iEditItemID = -1;
        document.getElementById('hdnAddRequestCategoryIDRow').value = "";
        frmNewRequestCategory_ShowNew();
    } catch (e) { showError("NewRequestCategory", e); }
}

function frmNewRequestCategory_ShowNew() {
    try {
        cmbRequestCategoryClient.SetEnabled(true);
        cmbRequestCategoryClient.SetValue(0);
        cmbLevelClient.SetValue(1);
        cmbNextLevelClient.SetValue(11);
        showWUF('ASPxCallbackPanelContenido_frmNewRequestCategory1', true);
    } catch (e) { showError("frmNewRequestCategory_ShowNew", e); }
}

function save() {
    frmNewRequestCategory_Save();
    return false;
}
function close() {
    frmNewRequestCategory_Close();
    return false;
}

function frmNewRequestCategory_Close() {
    try {
        //show te form
        showWUF('ASPxCallbackPanelContenido_frmNewRequestCategory1', false);
    } catch (e) { showError("frmNewRequestCategory_Close", e); }
}

function removePassport(ID) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    showParentLoading(true);
    var ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvSupervisorsV3.ashx?action=deletePassport&ID=" + ID + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            if (ajax.responseText == 'OK') {
                showParentLoading(false);
                refreshParentCardTree(-2);
            } else {
                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                    showParentLoading(false);
                    parent.parent.ShowMsgBoxForm(url, 500, 300, '');
                }
            }
        }
    }

    ajax.send(null)
}

//Graba el nom de l'usuari
function saveName(ID) {
    if (CheckConvertControls('divSupervisors') == true) {
        var txtName = txtName_Client.GetText();

        var stamp = '&StampParam=' + new Date().getMilliseconds();

        var ajax = nuevoAjax();
        ajax.open("GET", "Handlers/srvSupervisorsV3.ashx?action=chgName&ID=" + ID + "&NewName=" + encodeURIComponent(txtName) + stamp, true);

        ajax.onreadystatechange = function () {
            if (ajax.readyState == 4) {
                if (ajax.responseText == 'OK') {
                    document.getElementById('ASPxCallbackPanelContenido_txtName_I').textContent = txtName;
                    // Modificamos el nombre en los árboles d'empleado
                    var ctlPrefix = "roTreesSupervisors";
                    eval(ctlPrefix + "_roTrees.RenameSelectedNode(txtName);");

                    //RenameSelectedNode(txtName);
                } else {
                    if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                        var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                        parent.parent.ShowMsgBoxForm(url, 500, 300, '');
                    }
                }
            }
        }

        ajax.send(null)
    }
}

//Eliminación de empleado
function deleteGridEmployees(idRow) {
    try {
        var url = "Access/srvMsgBoxAccess.aspx?action=Message";
        url = url + "&TitleKey=deleteEmployeeExc.Title";
        url = url + "&DescriptionKey=deleteEmployeeExc.Description";
        url = url + "&Option1TextKey=deleteEmployeeExc.Option1Text";
        url = url + "&Option1DescriptionKey=deleteEmployeeExc.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelEmployee('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deleteEmployeeExc.Option2Text";
        url = url + "&Option2DescriptionKey=deleteEmployeeExc.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteGridEmployees", e); }
}

function delSelEmployee(idRow) {
    try {
        jsGridEmployees.deleteRow(idRow);
        hasChanges(true);
    } catch (e) { showError("delSelEmployee", e); }
}

function savePassport(IDPassport) {
    if (CheckConvertControls('divGeneral') == true) {
        var contenedor = document.getElementById('divGeneral');

        var opt = document.getElementById('optBGListAll_rButton');
        if (opt != null) {
            if (opt.checked == true) {
                document.getElementById('optBGListValue_txtBusinessGroup').value = '';
            }
        }

        //'cnIdentifyMethods_chkIntegrated_chkButton', 'cnIdentifyMethods_chkIntegrated_txtLogin',
        arrControls = new Array('txtDescription', 'txtStartDate', 'txtExpirationDate', 'chkState', 'cmbLanguage_Value',
            'cnIdentifyMethods_chkUsername_chkButton', 'cnIdentifyMethods_chkUsername_txtUserName', 'cnIdentifyMethods_chkUsername_txtPassword',
            'cnIdentifyMethods_chkCard_chkButton', 'cnIdentifyMethods_chkCard_txtCardMX',
            'cnIdentifyMethods_chkNFC_chkButton', 'cnIdentifyMethods_chkNFC_txtNFC',
            'cnIdentifyMethods_chkBiometric_chkButton',
            'cnIdentifyMethods_chkPin_chkButton', 'cnIdentifyMethods_chkPin_txtPin',
            'cnIdentifyMethods_chkPlate_chkButton', 'cnIdentifyMethods_chkPlate_txtPlate1', 'cnIdentifyMethods_chkPlate_txtPlate2', 'cnIdentifyMethods_chkPlate_txtPlate3', 'cnIdentifyMethods_chkPlate_txtPlate4',
            'cnIdentifyMethods_cmbFunctionality_Value', 'cmbLevelOfAuthority_Value', 'optBGListValue_txtBusinessGroup');

        //'checked', 'value',
        arrValueProperties = new Array('value', 'value', 'value', 'checked', 'value',
            'checked', 'value', 'value',
            'checked', 'value',
            'checked', 'value',
            'checked',
            'checked', 'value',
            'checked', 'value', 'value', 'value', 'value',
            'value', 'value', 'value');

        var postvars = '';
        var strValue = '';
        var control;
        for (n = 0; n < arrControls.length; n++) {
            control = document.getElementById(arrControls[n]);
            if (control != null) {
                eval("strValue=control." + arrValueProperties[n] + ";");
                postvars += '&' + encodeURIComponent(arrControls[n]) + '=' + encodeURIComponent(strValue);
            }
        }

        var stamp = '&StampParam=' + new Date().getMilliseconds();

        var ajax = nuevoAjax();
        ajax.open("GET", "Handlers/srvSupervisorsV3.ashx?action=savePassport&ID=" + IDPassport + postvars + stamp, true);
        ajax.onreadystatechange = function () {
            if (ajax.readyState == 4) {
                if (ajax.responseText == 'OK') {
                    cargaPassport(IDPassport);
                }
                else {
                    if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                        var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                        parent.parent.ShowMsgBoxForm(url, 500, 300, '');
                    }
                }
            }
        }
        ajax.send(postvars);
    }
}

//Mostra el ToolTip a la barra d'eines
function showTbTip(tip) {
    var oTip = document.getElementById(tip);
    if (oTip != null) oTip.style.display = '';
}

//Amaga el ToolTip a la barra d'eines
function hideTbTip(tip) {
    var oTip = document.getElementById(tip);
    if (oTip != null) oTip.style.display = 'none';
}

function showLoadingGrid(loading) {
    parent.parent.showLoader(loading);
}

function RefreshScreen(DataType) {
    
    refreshParentCardTree(parseInt(DataType,10));
}

function checkPermission() {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    showPopupLoader();
    var ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvSupervisorsV3.ashx?action=checkPermission" + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            hidePopupLoader(true);
            if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                parent.parent.ShowMsgBoxForm(url, 500, 300, '');
            }
        }
    }

    ajax.send(null);
}

function KeyPressFunction(e) {
    tecla = (document.all) ? e.keyCode : e.which;
    if (tecla == 13) {
        return false;
    }
}

function ShowBusinessGroup() {
    if (actualSupervisor > 0) {
        var txtSeleccion = document.getElementById('optBGListValue_txtBusinessGroup');

        var parameters;
        if (txtSeleccion == null)
            parameters = '?idPassport=' + actualSupervisor + "&seleccion=''";
        else
            parameters = '?idPassport=' + actualSupervisor + "&seleccion=" + txtSeleccion.value;

        parent.ShowExternalForm2('Security/PassportsBusinesGroups.aspx' + parameters, 400, 400, '', '', false, false);
    }
}

function ShowBusinessCenter() {
    if (actualSupervisor > 0) {
        parent.ShowExternalForm2('Security/PassportsBusinessCenters.aspx?idPassport=' + actualSupervisor, 400, 400, '', '', false, false);
    }
}

function RestorePwd() {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvSupervisorsV3.ashx?action=resetPassport&ID=" + actualSupervisor + "&PassportType=U" + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {            
            if (ajax.responseText == 'OK') {
                cargaPassport(actualSupervisor);
            } else {
                // Volvemos a cargar el árbol
                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                    parent.parent.ShowMsgBoxForm(url, 500, 300, '');
                    
                } else if (ajax.responseText.substr(0, 7) == 'INFOMSG') {
                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                    parent.parent.ShowMsgBoxForm(url, 500, 300, '');
                }
                parent.parent.showLoader(false);
            }
        }
    }

    ajax.send(null)
}


function RestoreCegidID() {
    parent.showLoader(true);
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvSupervisorsV3.ashx?action=restoreCegidID&ID=" + actualSupervisor + "&PassportType=U" + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            parent.showLoader(false);
            if (ajax.responseText == 'OK') {
            } else {
                // Volvemos a cargar el árbol
                cargaPassport(actualSupervisor);

                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                    parent.ShowMsgBoxForm(url, 500, 300, '');
                } else if (ajax.responseText.substr(0, 7) == 'INFOMSG') {
                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                    parent.ShowMsgBoxForm(url, 500, 300, '');
                }
            }
        }
    }

    ajax.send(null)
}

function SendUsername() {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvSupervisorsV3.ashx?action=SendUsername&ID=" + actualSupervisor + "&PassportType=U" + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            if (ajax.responseText == 'OK') {
            } else {
                // Volvemos a cargar el árbol
                cargaPassport(actualSupervisor);

                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                    parent.parent.ShowMsgBoxForm(url, 500, 300, '');
                } else if (ajax.responseText.substr(0, 7) == 'INFOMSG') {
                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                    parent.parent.ShowMsgBoxForm(url, 500, 300, '');
                }
            }
        }
    }

    ajax.send(null)
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters";
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

        parent.parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("showErrorPopup", e); }
}

function copySupervisors() {
    var Title = '';
    var url = 'SecurityChart/Wizards/SupervisorCopy.aspx?IDOrigin=' + actualSupervisor;
    top.ShowExternalForm2(url, 500, 450, Title, '', false, false, false);
}

function captchaCallback(action) {
    switch (action) {
        case "ERROR":
            window.parent.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
            break;
    }
}

function ShowCurrentLoggedUsers() {
    var Title = '';
    top.ShowExternalForm2('Security/Wizards/CurrentLoggedUsers.aspx', 800, 390, Title, '', false, false, false);
}

var dxgridCategories = null;
var actualDxGridCategories = null;
var unitModePopup = null;
var editPUnitMode = null;
var dxgridUsers = null;
var gridUsersDS = null;


function LoadPassportCategories(s) {
    if (s.cpGridsJSON == "null") {
        actualDxGridCategories = [];
    }
    else {
        actualDxGridCategories = JSON.parse(s.cpGridsJSON, roDateReviver);
    }

    if (s.cpUsersJSON == "null") {
        gridUsersDS = [];
    }
    else {
        gridUsersDS = JSON.parse(s.cpUsersJSON, roDateReviver);
    }

    dxgridUsers = $("#ASPxCallbackPanelContenido_divExceptionsSelector").dxDataGrid({
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,
        dataSource: {
            store: gridUsersDS
        },
        editing: {
            mode: "row",
            allowUpdating: false,
            allowDeleting: true,
            texts: { deleteRow: 'Delete', editRow: 'Edit' }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                $links.text("");

                if (isEditing) {
                    $links.filter(".dx-link-save").addClass("dx-icon-save");
                    $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                } else {
                    $links.filter(".dx-link-edit").addClass("dx-icon-edit");
                    $links.filter(".dx-link-delete").addClass("dx-icon-trash");
                }
            }
        },
        onRowRemoved: function (e) {
            hasChanges(true);
        },
        onEditingStart: function (e) {
            editgridUsers(e);
        },
        remoteOperations: {
            paging: true
        },
        paging: {
            pageSize: 12
        },
        pager: {
            showPageSizeSelector: false
        },
        columns: [
            { caption: "ID Categoría", dataField: "ID", visible: false, allowEditing: false },
            { caption: Globalize.formatMessage('roEmployeeName'), dataField: "Name", allowEditing: false, sortIndex: 0, sortOrder: "asc" }
        ]
    }).dxDataGrid("instance");
    
    actualDxGridCategories.forEach(function (item, index) {
        item.ID = index;
        item.CategoryName = Globalize.formatMessage("CategoryName_" + item.IDCategory);
    });

    dxgridCategories = $("#ASPxCallbackPanelContenido_divRequestCategory").dxDataGrid({
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,
        dataSource: {
            store: actualDxGridCategories
        },
        editing: {
            mode: "row",
            allowUpdating: true,
            allowDeleting: true,
            texts: { deleteRow: 'Delete', editRow: 'Edit' }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                $links.text("");

                if (isEditing) {
                    $links.filter(".dx-link-save").addClass("dx-icon-save");
                    $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                } else {
                    $links.filter(".dx-link-edit").addClass("dx-icon-edit");
                    $links.filter(".dx-link-delete").addClass("dx-icon-trash");
                }
            }

            if (e.rowType === "data" && e.column.dataField === "CategoryName") {
                switch (e.data.IDCategory) {
                    case 0:
                        break;
                }
            }
        },
        onRowRemoved: function (e) {
            hasChanges(true);
        },
        onEditingStart: function (e) {
            editgridCategories(e);
        },
        remoteOperations: {
            paging: true
        },
        paging: {
            pageSize: 12
        },
        pager: {
            showPageSizeSelector: false
        },
        columns: [
            { caption: "ID Categoría", dataField: "IDCategory", visible: false, allowEditing: false },
            { caption: Globalize.formatMessage('roCategoryName'), dataField: "CategoryName", allowEditing: false, sortIndex: 0, sortOrder: "asc" },
            { caption: Globalize.formatMessage('roLevelName'), dataField: "LevelOfAuthority", allowEditing: false },
            { caption: Globalize.formatMessage('roNextLevelName'), dataField: "ShowFromLevel", allowEditing: false },
        ]
    }).dxDataGrid("instance");
}

function GetSelectedSupervisorGroupTreeV3(oParm1) {
    if (oParm1 == "") {
        tmpGroupsSelected = '';
    } else {
        tmpGroupsSelected = oParm1;
    }
    if (oldParams != oParm1 && oldSupervisor == actualSupervisor && oldSupervisor != "" && undo == false) {
        //hasChanges(true);
    }
    oldParams = oParm1;
    oldSupervisor = actualSupervisor;
    undo = false;
}