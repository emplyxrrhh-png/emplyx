var actualTab = 0; // TAB per mostrar 0- pendientes, 1- histórico
var actualList = 0; // Lista seleccionada: Pendientes 0- Mis solicitudes, 1- Otras solicitudes, 2- Histórico solicitudes
var MyListLoaded = false;
var OtherListLoaded = false;
var HistoryListLoaded = false;
var arrStatus;

async function initialLoad() {
    let oParams = await getroRequestParams(true);

    await loadRequests(undefined, undefined, undefined, 'false');

    ConvertControls();

    resizeListHeight();

    if (oParams.getFilterEmployees(0) == "")
        ShowSelector(0, 1);
    else
        getElementFromList(0, "aFEmployees").textContent = getElementFromList(0, "lblAllEmp").textContent;

    if (oParams.getFilterEmployees(1) == "")
        ShowSelector(1, 1);
    else
        getElementFromList(1, "aFEmployees").textContent = getElementFromList(1, "lblAllEmp").textContent;

    if (oParams.getFilterEmployees(2) == "")
        ShowSelector(2, 1);
    else
        getElementFromList(2, "aFEmployees").textContent = getElementFromList(2, "lblAllEmp").textContent;

    if ($("#actualListValue").val() == '1') chTopTabList('OtherRequests');
}

//==  PROBADOS OK ===========================================================================================
function getElementFromList(ListType, prefix) {
    var obj = null;
    switch (ListType) {
        case 0:
            obj = document.getElementById("ctl00_contentMainBody_Pend_" + prefix);
            break;
        case 1:
            obj = document.getElementById("ctl00_contentMainBody_Other_" + prefix);
            break;
        case 2:
            obj = document.getElementById("ctl00_contentMainBody_Hist_" + prefix);
            break;
    }
    if (obj == null) {
        S
        alert("atencion! prefix " + prefix + " es nulo");
    }
    return obj;
}

function SortDirection(ListType, direction) {
    try {
        var oAscending = getElementFromList(ListType, 'icoAscending');
        var oDescending = getElementFromList(ListType, 'icoDescending');

        if (oAscending != null && oDescending != null) {
            var classAscending = new Array();
            var classDescending = new Array();
            classAscending = oAscending.className.split(' ');
            classDescending = oDescending.className.split(' ');
            if (direction == 'ASC') {
                oAscending.className = classAscending[0] + ' RequestListIcoPressed';
                oDescending.className = classDescending[0] + ' RequestListIcoUnPressed';
            }
            if (direction == 'DESC') {
                oAscending.className = classAscending[0] + ' RequestListIcoUnPressed';
                oDescending.className = classDescending[0] + ' RequestListIcoPressed';
            }

            // Guada configuración en cookie
            setOrderDirection(ListType, direction);
        }
    }
    catch (e) {
        showError("SortDirection", e);
    }
}

function AdvancedFilter(ListType) {
    var bolRet = false;
    try {
        var beginId = "txtRequestDateBegin&&_Client";
        var endId = "txtRequestDateEnd&&_Client";

        var beginRequestedId = "txtRequestedDateBegin&&_Client";
        var endRequestedId = "txtRequestedDateEnd&&_Client";

        switch (ListType) {
            case 0:
                beginId = beginId.replace("&&", 'Pend');
                endId = endId.replace("&&", 'Pend');
                beginRequestedId = beginRequestedId.replace("&&", 'Pend');
                endRequestedId = endRequestedId.replace("&&", 'Pend');
                break;
            case 1:
                beginId = beginId.replace("&&", 'Other');
                endId = endId.replace("&&", 'Other');
                beginRequestedId = beginRequestedId.replace("&&", 'Other');
                endRequestedId = endRequestedId.replace("&&", 'Other');
                break;
            case 2:
                beginId = beginId.replace("&&", 'Hist');
                endId = endId.replace("&&", 'Hist');
                beginRequestedId = beginRequestedId.replace("&&", 'Hist');
                endRequestedId = endRequestedId.replace("&&", 'Hist');
                break;
        }

        var BeginDate = null;
        var EndDate = null;
        var BeginRequestedDate = null;
        var EndRequestedDate = null;

        eval("BeginDate = " + beginId);
        eval("EndDate = " + endId);
        eval("BeginRequestedDate = " + beginRequestedId);
        eval("EndRequestedDate = " + endRequestedId);

        var otBeginDate = BeginDate.GetValue();
        var otEndDate = EndDate.GetValue();
        var otBeginRequestedDate = BeginRequestedDate.GetValue();
        var otEndRequestedDate = EndRequestedDate.GetValue();

        if (otBeginDate != null && otEndDate != null) {
            var intMonth = parseInt(otBeginDate.getMonth()) + 1;
            var strBeginDate = otBeginDate.getFullYear() + "/" + intMonth + "/" + otBeginDate.getDate();

            intMonth = parseInt(otEndDate.getMonth()) + 1;
            var strEndDate = otEndDate.getFullYear() + "/" + intMonth + "/" + otEndDate.getDate();

            setFilterRequestDate(ListType, strBeginDate, strEndDate);
        }
        else {
            setFilterRequestDate(ListType, "", "");
        }

        if (otBeginRequestedDate != null && otEndRequestedDate != null) {
            var intMonth = parseInt(otBeginRequestedDate.getMonth()) + 1;
            var strBeginRequestedDate = otBeginRequestedDate.getFullYear() + "/" + intMonth + "/" + otBeginRequestedDate.getDate();

            intMonth = parseInt(otEndRequestedDate.getMonth()) + 1;
            var strEndRequestedDate = otEndRequestedDate.getFullYear() + "/" + intMonth + "/" + otEndRequestedDate.getDate();

            setFilterRequestedDate(ListType, strBeginRequestedDate, strEndRequestedDate);
        }
        else {
            setFilterRequestedDate(ListType, "", "");
        }

        bolRet = true;
    }
    catch (e) {
        showError("AdvancedFilter", e);
    }
    return bolRet;
}

function TypeFilter(ListType, obj) {
    try {
        var Filter = '';
        for (n = 1; n <= 17; n++) {
            var oIcoTypeFilter = null;
            switch (n) {
                case 1: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeUserFieldsChange'); break;
                case 2: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeForbiddenPunch'); break;
                case 3: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeJustifyPunch'); break;
                case 4: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeExternalWorkResumePart'); break;
                case 5: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeChangeShift'); break;
                case 6: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeVacationsOrPermissions'); break;
                case 7: oIcoTypeFilter = getElementFromList(ListType, 'icoTypePlannedAbsences'); break;
                case 8: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeExchangeShiftBetweenEmployees'); break;
                case 9: oIcoTypeFilter = getElementFromList(ListType, 'icoTypePlannedCauses'); break;
                case 10: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeForbiddenTaskPunch'); break;
                case 11: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeCancelHolidays'); break;
                case 12: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeForgottenCostCenterPunch'); break;
                case 13: oIcoTypeFilter = getElementFromList(ListType, 'icoTypePlannedHolidays'); break;
                case 14: oIcoTypeFilter = getElementFromList(ListType, 'icoTypePlannedOvertimes'); break;
                case 15: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeExternalWorkWeekResume'); break;
                case 16: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeTelecommute'); break;
                case 17: oIcoTypeFilter = getElementFromList(ListType, 'icoTypeDailyRecord'); break;
            }
            if (oIcoTypeFilter != null) {
                if (oIcoTypeFilter.id == obj.id) {
                    //Es el seleccionat, modifiquem el valor
                    var claseObj = new Array();
                    claseObj = obj.className.split(' ');
                    if (claseObj[1] == 'RequestListIcoUnPressed') {
                        oIcoTypeFilter.className = claseObj[0] + ' RequestListIcoPressed';
                    } else {
                        oIcoTypeFilter.className = claseObj[0] + ' RequestListIcoUnPressed';
                    }
                }
                //Recupera les clases (normalment 2 icoFilterx icoEstat)
                var clase = new Array();
                clase = oIcoTypeFilter.className.split(' ');
                if (clase[1] == 'RequestListIcoPressed') {
                    Filter = Filter + ',' + n;
                }
            }
        }
        if (Filter != '') {
            Filter = Filter.substr(1, Filter.length - 1);
        }
        // Guardar configuración
        setFilterRequestType(ListType, Filter);
    }
    catch (e) {
        showError("TypeFilter", e);
    }
}

function filterVisible(ListType) {
    var objDiv = getElementFromList(ListType, 'divFiltreAvan');
    if (objDiv != null) {
        if (objDiv.style.display == '') {
            objDiv.style.display = 'none';
        } else {
            objDiv.style.display = '';
        }
    }
}

async function ShowSelector(ListType, oSel) {
    try {
        var oParams = await getroRequestParams();

        if (oSel == 1) { //todos los empleados
            oParams.setFilterEmployees(ListType, '');
            oParams.setFilterTree(ListType, '');
            oParams.setFilterTreeUser(ListType, '');
            getElementFromList(ListType, "aFEmployees").textContent = getElementFromList(ListType, "lblAllEmp").textContent;
        }
        else {
            var navigatorIndex = "";
            var prefixNameFunction = "";
            switch (ListType) {
                case 0:
                    prefixNameFunction = 'Pend';
                    break;
                case 1:
                    prefixNameFunction = 'Other';
                    break;
                case 2:
                    prefixNameFunction = 'Hist';
                    break;
            }

            if (window.addEventListener) //Firefox
                navigatorIndex = "0";
            else //IE
                navigatorIndex = "1";

            navigatorIndex = "0";

            var prefixCookie = "objContainerTreeV3_RequestsFilterTreeEmp" + prefixNameFunction + "Grid";
            var prefixTree = "RequestsFilterTreeEmp" + prefixNameFunction;
            var returnFunc = "GetSelectedTreeV3" + prefixNameFunction;
            var prefix = prefixNameFunction;

            var strBase = "Base/Popups/EmployeeSelectorPopup.aspx?PrefixTree=" + prefixTree + "&PrefixCookie=" + prefixCookie + "&AfterSelectFuncion=" + returnFunc + "&Prefix=" + prefix + "&NavigatorIndex=" + navigatorIndex + "&FeatureAlias=" + "Employees";
            getElementFromList(ListType, "aFEmployees").textContent = getElementFromList(ListType, "lblEmpSelect").textContent;
            parent.ShowExternalForm2(strBase, 800, 400, "", "", false, false);
        }
    }
    catch (e) {
        showError("ShowSelector", e);
    }
}

//==========================================================================
//Guarda los empleados seleccionados en el TreeV3 PEND
//==========================================================================
async function GetSelectedTreeV3Pend(oParm1, oParm2, oParm3) {
    var oParams = await getroRequestParams();
    if (oParm1 == "") {
        oParams.setFilterEmployees(0, '');
        oParams.setFilterTree(0, '');
        oParams.setFilterTreeUser(0, '');
        document.getElementById('ctl00_contentMainBody_Pend_aFEmployees').textContent = document.getElementById('ctl00_contentMainBody_Pend_aEmpAll').textContent;
    }
    else {
        oParams.setFilterEmployees(0, oParm1);
        oParams.setFilterTree(0, oParm2);
        oParams.setFilterTreeUser(0, oParm3);
        document.getElementById('ctl00_contentMainBody_Pend_aFEmployees').textContent = document.getElementById('ctl00_contentMainBody_Pend_aEmpSelect').textContent;
    }
}

//==========================================================================
//Guarda los empleados seleccionados en el TreeV3 OTHER
//==========================================================================
async function GetSelectedTreeV3Other(oParm1, oParm2, oParm3) {
    var oParams = await getroRequestParams();
    if (oParm1 == "") {
        oParams.setFilterEmployees(1, '');
        oParams.setFilterTree(1, '');
        oParams.setFilterTreeUser(1, '');
        document.getElementById('ctl00_contentMainBody_Other_aFEmployees').textContent = document.getElementById('ctl00_contentMainBody_Other_aEmpAll').textContent;
    }
    else {
        oParams.setFilterEmployees(1, oParm1);
        oParams.setFilterTree(1, oParm2);
        oParams.setFilterTreeUser(1, oParm3);
        document.getElementById('ctl00_contentMainBody_Other_aFEmployees').textContent = document.getElementById('ctl00_contentMainBody_Other_aEmpSelect').textContent;
    }
}

//==========================================================================
//Guarda los empleados seleccionados en el TreeV3 HISTORY
//==========================================================================
async function GetSelectedTreeV3Hist(oParm1, oParm2, oParm3) {
    var oParams = await getroRequestParams();
    if (oParm1 == "") {
        oParams.setFilterEmployees(2, '');
        oParams.setFilterTree(2, '');
        oParams.setFilterTreeUser(2, '');
        document.getElementById('ctl00_contentMainBody_Hist_aFEmployees').textContent = document.getElementById('ctl00_contentMainBody_Hist_aEmpAll').textContent;
    }
    else {
        oParams.setFilterEmployees(2, oParm1);
        oParams.setFilterTree(2, oParm2);
        oParams.setFilterTreeUser(2, oParm3);
        document.getElementById('ctl00_contentMainBody_Hist_aFEmployees').textContent = document.getElementById('ctl00_contentMainBody_Hist_aEmpSelect').textContent;
    }
}

async function setOrderField(ListType, strOrderField) {
    var oParams = await getroRequestParams();
    oParams.setOrder(ListType, strOrderField);
}
async function setOrderDirection(ListType, strOrderDirection) {
    var oParams = await getroRequestParams();
    oParams.setOrderDirection(ListType, strOrderDirection);
}
async function setLevels(ListType, strLevels) {
    var oParams = null;
    if (ListType == 1) {
        oParams = await getroRequestParams();
    }
    if (oParams != null) {
        oParams.setFilterLevels(strLevels);
    }
}
async function setDaysFrom(ListType, objDaysFromId) {
    var oParams = null;
    if (ListType == 1) {
        oParams = await getroRequestParams();
    }
    var obj = document.getElementById(objDaysFromId);
    if (oParams != null && obj != null) {
        oParams.setFilterDaysFrom(obj.value);
    }
}
async function setFilterRequestDate(ListType, strBeginDate, strEndDate) {
    var oParams = await getroRequestParams();
    oParams.setFilterRequestDate(ListType, strBeginDate, strEndDate);
}

async function setFilterRequestedDate(ListType, strBeginDate, strEndDate) {
    var oParams = await getroRequestParams();
    oParams.setFilterRequestedDate(ListType, strBeginDate, strEndDate);
}

async function setFilterRequestType(ListType, strFilter) {
    var oParams = await getroRequestParams();
    oParams.setFilterRequestType(ListType, strFilter);
}

async function setFilterIdCause(ListType, strIdCause) {
    var oParams = await getroRequestParams();
    oParams.setFilterIdCause(ListType, strIdCause);
}

async function setFilterIdSupervisor(ListType, strIdSupervisor) {
    var oParams = await getroRequestParams();
    oParams.setFilterIdSupervisor(ListType, strIdSupervisor);
}
//==============================================================================================

function StateFilter(ListType, obj) {
    var Filter = '';
    for (n = 1; n < 6; n++) {
        var oIcoStateFilter = null;

        switch (n) {
            case 1:
                oIcoStateFilter = getElementFromList(ListType, 'icoStatePending');
                break;
            case 2:
                oIcoStateFilter = getElementFromList(ListType, 'icoStateOnGoing');
                break;
            case 3:
                oIcoStateFilter = getElementFromList(ListType, 'icoStateAccepted');
                break;
            case 4:
                oIcoStateFilter = getElementFromList(ListType, 'icoStateDenied');
                break;
            case 5:
                oIcoStateFilter = getElementFromList(ListType, 'icoStateCanceled');
                break;
        }

        if (oIcoStateFilter != null) {
            if (oIcoStateFilter.id == obj.id) {
                //Es el seleccionat, modifiquem el valor
                var claseObj = new Array();
                claseObj = obj.className.split(' ');
                if (claseObj[1] == 'RequestListIcoUnPressed') {
                    oIcoStateFilter.className = claseObj[0] + ' RequestListIcoPressed';
                } else {
                    oIcoStateFilter.className = claseObj[0] + ' RequestListIcoUnPressed';
                }
            }
            //Recupera les clases (normalment 2 icoFilterx icoEstat)
            var clase = new Array();
            clase = oIcoStateFilter.className.split(' ');
            if (clase[1] == 'RequestListIcoUnPressed') {
                Filter = Filter + '0';
            } else {
                Filter = Filter + '1';
            }
        }
    }
    // Guardar configuración
    setFilterRequestState(Filter);
}

// PENDIENTES DE PROBAR ==========================================================================
function loadRequestsFromFilter(ListType, loadTab) {
    var obj = getElementFromList(ListType, "chkSaveFilter");
    loadRequests(ListType, loadTab, obj.checked, undefined, 'false');
}

async function loadRequests(ListType, loadTab, CheckSaveFilter, overWriteFilter) {
    try {
        showLoadingGrid(true);

        if (CheckSaveFilter == undefined || CheckSaveFilter == null) CheckSaveFilter = false;

        if (ListType == null) ListType = actualList;

        actualList = ListType;

        var oParams = await getroRequestParams();

        if (ListType == 0) { // Lista mis solicitudes
            actualTab = 0;
            MyListLoaded = true;

            if (loadTab == null || loadTab == true) loadRequestsTab('');

            var cmbNum = eval("cmbNumRequestsPend")

            var NumReg = cmbNum.GetSelectedItem().value
            loadRequestsList(ListType, 'gridMyRequestsListContainer', NumReg, oParams.getOrder(0), oParams.getFilter(0), oParams.getLevelsBelow(0), oParams.getFilterEmployees(0),
                oParams.getFilterTree(0), oParams.getFilterTreeUser(0), oParams.getFilterIdCause(0), oParams.getFilterIdSupervisor(0), CheckSaveFilter, overWriteFilter);
        }
        if (ListType == 1) { // Lista otras solicitudes
            actualTab = 0;
            OtherListLoaded = true;

            if (loadTab == null || loadTab == true) loadRequestsTab('');

            var cmbNum = eval("cmbNumRequestsOther")
            var NumReg = cmbNum.GetSelectedItem().value
            loadRequestsList(ListType, 'gridOtherRequestsListContainer', NumReg, oParams.getOrder(1), oParams.getFilter(1), oParams.getLevelsBelow(1), oParams.getFilterEmployees(1),
                oParams.getFilterTree(1), oParams.getFilterTreeUser(1), oParams.getFilterIdCause(1), oParams.getFilterIdSupervisor(1), CheckSaveFilter, overWriteFilter);
        }
        if (ListType == 2) { // Lista histórico solicitudes
            actualTab = 1;
            HistoryListLoaded = true;

            if (loadTab == null || loadTab == true) loadRequestsTab('');
            var cmbNum = eval("cmbNumRequestsHist")
            var NumReg = cmbNum.GetSelectedItem().value
            loadRequestsList(ListType, 'gridHistoryRequestsListContainer', NumReg, oParams.getOrder(2), oParams.getFilter(2), oParams.getLevelsBelow(2), oParams.getFilterEmployees(2),
                oParams.getFilterTree(2), oParams.getFilterTreeUser(2), oParams.getFilterIdCause(2), oParams.getFilterIdSupervisor(2), CheckSaveFilter, overWriteFilter);
        }
    }
    catch (e) {
        showError("loadRequests", e);
    }
}

var responseObj = null;
// Carrega la part del TAB grisa superior
function loadRequestsTab(strFilter, strFilterOptions) {
    try {
        showLoadingGrid(true);
        var Url = "";

        Url = "../Requests/srvRequests.aspx?action=getRequestsTab&aTab=" + actualTab + "&Filter=" + encodeURIComponent(strFilter);
        if (strFilterOptions != undefined && strFilterOptions != null) {
            Url = Url + "&FilterOptions=" + strFilterOptions;
        }
        AsyncCall("POST", Url, "CONTAINER", "divTab", "");

        Url = "../Requests/Handlers/srvRequests.ashx?action=getBarButtons";
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId)");
    }
    catch (e) {
        showError("loadRequestsTab", e);
    }
}

function parseResponseBarButtons(oResponse) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);
}
async function loadRequestsList(ListType, ListContainerID, strNumRequestToLoad, strOrder, strFilter, strLevelsBelow, strFilterEmployees, strFilterTree, strFilterTreeUser, strIdCause, strIdSupervisor, CheckSaveFilter, overwriteFilter) {
    try {
        showLoadingGrid(true);
        var Url = "../Requests/srvRequests.aspx?action=getRequestsList&aTab=" + actualTab + "&Order=" + strOrder + "&Filter=" + encodeURIComponent(strFilter) + "&ListType=" + ListType;

        if (strNumRequestToLoad != undefined && strNumRequestToLoad != null && strNumRequestToLoad > 0) {
            Url = Url + "&NumRequestToLoad=" + strNumRequestToLoad;
        }

        if (strLevelsBelow != undefined && strLevelsBelow != null) {
            Url = Url + "&LevelsBelow=" + strLevelsBelow;
        }

        if (strFilterEmployees != undefined && strFilterEmployees != null) {
            Url = Url + "&FilterEmployees=" + encodeURIComponent(strFilterEmployees);
        }

        if (strFilterTree != undefined && strFilterTree != null) {
            Url = Url + "&FilterTree=" + encodeURIComponent(strFilterTree);
        }

        if (strFilterTreeUser != undefined && strFilterTreeUser != null) {
            let strTemp = StringEncodeControlChars(strFilterTreeUser);
            Url = Url + "&FilterTreeUser=" + encodeURIComponent(strTemp);
        }

        if (strIdCause != undefined && strIdCause != null) {
            if (parseInt(strIdCause) > 0) {
                Url = Url + "&IdCause=" + encodeURIComponent(strIdCause);
            }
            else {
                Url = Url + "&IdCause=0";
            }
        }

        if (strIdSupervisor != undefined && strIdSupervisor != null) {
            if (parseInt(strIdSupervisor) > 0) {
                var strTemp = StringEncodeControlChars(strIdSupervisor);
                Url = Url + "&IdSupervisor=" + encodeURIComponent(strIdSupervisor);
            }
            else {
                Url = Url + "&IdSupervisor=0";
            }
        }

        AsyncCall("POST", Url, "CONTAINER", ListContainerID, "showLoadingGrid(false);SelectRow(null);");

        //Guardar filtro definido por el usuario en las tres listas
        if (CheckSaveFilter) {
            let oParams = await getroRequestParams();
            let strFilters = JSON.stringify(oParams.buildRequestObject())
            strFilters = encodeURIComponent(strFilters);

            Url = "../Requests/srvRequests.aspx?action=saveFilter&Filter=" + strFilters;
            AsyncCall("POST", Url, "json")
        }
        else {
            if (typeof overwriteFilter != 'undefined') {
            }
            else {
                Url = "../Requests/srvRequests.aspx?action=saveFilter&Filter=" + "";
                AsyncCall("POST", Url, "json")
            }
        }
    }
    catch (e) {
        showError("loadRequestsList", e);
    }
}

function loadRequest(IDRequest, IdTableRow) {
    try {
        showLoadingGrid(true);
        var Url = "../Requests/srvRequests.aspx?action=getRequest&IDRequest=" + IDRequest + "&IdTableRow=" + IdTableRow;
        AsyncCall("POST", Url, "CONTAINER", "divRequestContent", "ConvertControls('divRequestContent');  showLoadingGrid(false); showMainForm();");
    }
    catch (e) {
        showError("loadRequest", e);
    }
}

var orgChart = null;
var orgChartPopup = null;

function showMainForm() {
    var doc = document.getElementById("divRequestContent");

    if (typeof (doc) != 'undefined') {
        if (doc != null) {
            doc.style.display = '';
        }
    }

    var butCancel = document.getElementById("divCancel");
    if (typeof (butCancel) != 'undefined') {
        if (butCancel != null) butCancel.style.display = '';
    }
}

function loadRequestOrgChart(IDRequest) {
}

function ASPxOrgChartClientEndCallBack(s, e) {
    showLoadingGrid(false);

    switch (s.cpAction) {
        case "GETSECURITYCHART":
            if (checkResult(s)) {
                orgChart.parseResponse(s, e);
            }
            break;
    }
}

function checkResult(oResult) {
    if (oResult.cpResult == 'KO') {
        DevExpress.ui.dialog.alert(oResult.cpMessage, Globalize.formatMessage("roError"));
        return false;
    }
    return true;
}

function showComments(IDRequest, IdTableRow, AproveRefuse) {
    try {
        document.getElementById('hdnCommentsRequestID').value = IDRequest;
        document.getElementById('hdnCommentsRequestIDTableRow').value = IdTableRow
        document.getElementById('hdnCommentsApproveRefuse').value = AproveRefuse;

        showWUF('frmComments1', true);
    } catch (e) { showError("showComments", e); }
}
function closeComments() {
    try {
        showWUF('frmComments1', false);
    } catch (e) { showError("closeComments", e); }
}

function showPendingSupervisors(IDRequest) {
    showLoadingGrid(true);
    var ajax = nuevoAjax();
    ajax.open("GET", "../Requests/Handlers/srvRequests.ashx?action=getSupervisorsPending&IDRequest=" + IDRequest, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            if (ajax.responseText != "") {
                document.getElementById("divShowSupervisorsPending").style.display = 'none';
                document.getElementById("divLblSupervisorsPending").style.display = '';
                document.getElementById("lblNextLevelPassports").textContent = ajax.responseText;
            } else {
                document.getElementById("divShowSupervisorsPending").style.display = '';
                document.getElementById("divLblSupervisorsPending").style.display = 'none';
                document.getElementById("lblNextLevelPassports").textContent = '';
            }
            showLoadingGrid(false);
        }
    }

    ajax.send(null)
}

var selRequest = -1;
var selIdTableRow = -1;
var selComments = "";

function approveRequest(IDRequest, IdTableRow, Comments, forceApprove) {
    try {
        selRequest = IDRequest;
        selIdTableRow = IdTableRow;
        selComments = Comments;

        showLoadingGrid(true);
        //var postLoad = "checkStatus(arrStatus, 'approveRequest'); if(arrStatus[0].error == 'false'){ loadRequests(null, true); }";
        var postLoad = "";
        if (IdTableRow != "-1") {
            postLoad = "checkStatus(arrStatus, 'approveRequest'); if(arrStatus[0].error == 'false'){ LockRequest('" + IDRequest + "','" + IdTableRow + "'); loadRequest('" + IDRequest + "','" + IdTableRow + "'); }";
        } else {
            postLoad = "checkStatus(arrStatus, 'approveRequest'); if(arrStatus[0].error == 'false'){ loadRequest('" + IDRequest + "','" + IdTableRow + "'); }";
        }
        var url = "../Requests/srvRequests.aspx?action=approveRequest&IDRequest=" + IDRequest;
        if (Comments != null) {
            url = url + "&Comments=" + encodeURIComponent(Comments)
        }
        if (forceApprove != null) {
            url = url + "&forceApprove=" + forceApprove;
        } else {
            url = url + "&forceApprove=false";
        }
        url = url + "&CheckLockedDays=true";

        AsyncCall("POST", url, "json", "arrStatus", postLoad)
    }
    catch (e) {
        showError("approveRequest", e);
    }
}

function refuseRequest(IDRequest, IdTableRow, Comments) {
    try {
        showLoadingGrid(true);
        //var postLoad = "checkStatus(arrStatus, 'refuseRequest'); if(arrStatus[0].error == 'false'){ loadRequests(null, true); }";
        var postLoad = "";
        if (IdTableRow != "-1") {
            postLoad = "checkStatus(arrStatus, 'refuseRequest'); if(arrStatus[0].error == 'false'){ LockRequest('" + IDRequest + "','" + IdTableRow + "'); loadRequest('" + IDRequest + "','" + IdTableRow + "'); }";
        } else {
            postLoad = "checkStatus(arrStatus, 'refuseRequest'); if(arrStatus[0].error == 'false'){ loadRequest('" + IDRequest + "','" + IdTableRow + "'); }";
        }
        var url = "../Requests/srvRequests.aspx?action=refuseRequest&IDRequest=" + IDRequest;
        if (Comments != null) {
            url = url + "&Comments=" + encodeURIComponent(Comments);
        }
        AsyncCall("POST", url, "json", "arrStatus", postLoad)
    } catch (e) { showError("refuseRequest", e); }
}

function checkStatus(oStatus, action) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "";
                if (objError.id == "NotEnoughConceptBalance" || objError.id == "NeedConfirmation") {
                    url = "Requests/srvMsgBoxRequests.aspx?action=Message&TitleKey=" + action + ".Error.Text&" +
                        "DescriptionText=" + objError.msg + "&" +
                        "Option1TextKey=" + action + ".Balance.Error.Option1Text&" +
                        "Option1DescriptionKey=" + action + ".Balance.Error.Option1Description&" +
                        "Option1OnClickScript=window.frames['ifPrincipal'].approveRequest(" + selRequest + ",'" + selIdTableRow + "','" + selComments + "',true);HideMsgBoxForm(); return false;&" +
                        "Option2TextKey=" + action + ".Balance.Error.Option2Text&" +
                        "Option2DescriptionKey=" + action + ".Balance.Error.Option2Description&" +
                        "Option2OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/dialog-question.png";
                } else {
                    url = "Requests/srvMsgBoxRequests.aspx?action=Message&TitleKey=" + action + ".Error.Text&" +
                        "DescriptionText=" + objError.msg + "&" +
                        "Option1TextKey=" + action + ".Error.Option1Text&" +
                        "Option1DescriptionKey=" + action + ".Error.Option1Description&" +
                        "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                }
                parent.ShowMsgBoxForm(url, 400, 300, '');
                showLoadingGrid(false);
            } else { //Missatge estil inline
            }
        }
    } catch (e) { showError('checkStatus', e); }
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

//Mostra el ToolTip a la barra d'eines
function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

//Amaga el ToolTip a la barra d'eines
function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

async function setFilterRequestState(strFilter) {
    var oParams = await getroRequestParams();
    oParams.setFilterRequestState(strFilter);
}
async function setShowAllStatusLevel(bolShow) {
    var oParams = await getroRequestParams();
    oParams.setShowAllStatusLevel(bolShow);
}
async function setIncludeMajorStatusLevel(bolInclude) {
    var oParams = await getroRequestParams();
    oParams.setShowIncludeMajorStatusLevel(bolInclude);
}
async function setFilterRequestTypePend(strFilter) {
    var oParams = await getroRequestParams();
    oParams.setFilterRequestTypePend(strFilter);
}
async function setFilterPendingDays(bolEnabled, strDays) {
    var oParams = await getroRequestParams();
    oParams.setFilterPendingDays(bolEnabled, strDays);
}
async function setFilterOnGoingDays(bolEnabled, strDays) {
    var oParams = await getroRequestParams();
    oParams.setFilterOnGoingDays(bolEnabled, strDays);
}

function DDown_Out(ListType) {
    try {
        var aFEmployees = getElementFromList(ListType, "aFEmployees");
        if (aFEmployees.getAttribute("vdisable") == "true")
            return;
        var divFloatMenuE = getElementFromList(ListType, "divFloatMenuE");
        divFloatMenuE.style.display = 'none';
    }
    catch (e) {
        showError("DDown_OutPend", e);
    }
}

function DDown_Over(ListType) {
    try {
        var aFEmployees = getElementFromList(ListType, "aFEmployees");
        if (aFEmployees.getAttribute("vdisable") == "true")
            return;
        var divFloatMenuE = getElementFromList(ListType, "divFloatMenuE");
        divFloatMenuE.style.display = '';
    }
    catch (e) {
        showError("DDown_OverPend", e);
    }
}

function showEmployee(oN, oID, strRoolUrl) {
    $.ajax({
        url: `/Employee/GetEmployeeTreeSelectionPath/${oID}`,
        data: {},
        type: "GET",
        dataType: "json",
        success: async (data) => {
            if (typeof data != 'string') {

                let treeState = await getroTreeState("ctl00_contentMainBody_roTrees1", true);
                await treeState.setRedirectData(data.EmployeePath, data.GroupSelectionPath);

                window.open("../#/" + strRoolUrl + "/Employees/Employees", "_blank");

            } else {
                DevExpress.ui.notify(data, 'error', 2000);
            }
        },
        error: (error) => console.error(error),
    });
}

function showEmployeeGroup(oN, oID, strRoolUrl) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();
    var _ajax = nuevoAjax();
    _ajax.open("GET", "../Base/WebUserControls/EmployeeSelectorData.aspx?action=getSelectionPath&node=A" + oID + "&TreeType=1" + stamp, true);

    _ajax.onreadystatechange = async function () {
        if (_ajax.readyState == 4) {
            var objPrefix = "ctl00_contentMainBody_roTrees1";
            var val = _ajax.responseText;
            if (val == null || val == "null") { val = ""; }

            let treeState = await getroTreeState("ctl00_contentMainBody_roTrees1", true);
            await treeState.setRedirectData("A" + oID, val);

            window.open("../#/" + strRoolUrl + "/Employees/Employees", "_blank");
        }
    }
    _ajax.send(null)
}

function ShowUserFieldHistory(IDEmployee, FieldName) {
    top.ShowExternalForm2('Employees/UserFieldHistory.aspx?Type=0&IDSource=' + IDEmployee + '&FieldName=' + FieldName, 500, 450, '', '', false, false, false);
}

function ShowUserFieldValue(aAnchor, CountField) {
    aAnchor.style.display = 'none';
    document.getElementById('aHideValue_' + CountField).style.display = '';
    document.getElementById('divShowValue_' + CountField).style.display = '';
    ConvertControls('divShowValue_' + CountField);
}

function HideUserFieldValue(aAnchor, CountField) {
    aAnchor.style.display = 'none';
    document.getElementById('aShowValue_' + CountField).style.display = '';
    document.getElementById('divShowValue_' + CountField).style.display = 'none';
}

function UserFieldValueChange(FieldName, DayDate) {
    var inputDate = document.getElementById(FieldName + '_@@Date@@');
    if (inputDate != null) {
        if (inputDate.getAttribute('CCvisible') == 'true') {
            if (inputDate.value != DayDate) {
                inputDate.value = DayDate;
                inputDate.style.color = 'red';
            }
        }
    }
}

//Recuperem el tipus de format de la data (d=0,m=1,y=2)
function retDateFormat() {
    var dateFormat = document.getElementById('ctl00_contentMainBody_dtFormat').value;
    return dateFormat;
}

function retDateFormatText() {
    var dateFormatText = document.getElementById('ctl00_contentMainBody_dtFormatText').value;
    return dateFormatText;
}

/* Ver Calendario anual */
/*************************************************************************************************************/
function showEmpAnnualDetail(IDEmp) {
    var url = 'Scheduler/AnnualView.aspx?EmployeeID=' + IDEmp + "&fromPage=request";
    var Title = ''; //$get('<%= lblMovesFormTitle.ClientID %>').innerHTML;
    parent.ShowExternalForm2(url, 950, 535, Title, '', true, false, false);
}

function showPlannedHolidaysResume(IDRequest) {
    try {
        document.getElementById('hdnDayDetailsRequestID').value = IDRequest;
        showWUF('frmDayDetails1', true);
    } catch (e) { showError("showComments", e); }
}

function closeDayDetails() {
    try {
        showWUF('frmDayDetails1', false);
    } catch (e) { showError("closeComments", e); }
}

function FormateaFecha(d) {
    var strFecha = "";
    strFecha = d.getFullYear() + "#" + (d.getMonth() + 1) + "#" + d.getDate();
    return strFecha;
}

 function showPlanificationGroup(IDGroup, strStartDate, strEndDate, numTabSelected, strRoolUrl) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var _ajax = nuevoAjax();
    _ajax.open("GET", "../Base/WebUserControls/EmployeeSelectorData.aspx?action=getSelectionPath&node=A" + IDGroup + "&TreeType=1" + stamp, true);

     _ajax.onreadystatechange = async function () {
        if (_ajax.readyState == 4) {
            var val = _ajax.responseText;
            if (val == null || val == "null") { val = ""; }
            var objPrefix = "";
            var calendarURL = "";

            convertDateOut(moment(strStartDate).format(localFormat), moment(strEndDate).format(localFormat));
            objPrefix = 'ctl00_contentMainBody_PopupSelectorEmployees_ASPxPanel3_objContainerTreeV3_roTrees1EmpCalendar';
            calendarURL = "../#/" + strRoolUrl + "/Scheduler/Calendar";

            let oTreeState = await getroTreeState(objPrefix);
            oTreeState.load(objPrefix);
            oTreeState.setSelectedPath(val, '1');
            oTreeState.setSelected("A" + IDGroup, '1');
            oTreeState.setActiveTreeType('1');
            //

            const cat = localStorage.getItem('empMVCSelector');
            let currentView = { Employees: [], Groups: [], Collectives: [], LabAgrees: [], Filter: "11110", Operation: "or", UserFields: "", ComposeMode: 'Custom', ComposeFilter: "", Advanced: false };
            if (cat != null) {
                let tmpOptions = JSON.parse(cat);
                currentView = tmpOptions.view;
            }

            try {
                currentView.ByPassCache = true;
                currentView.Groups = ["A" + IDGroup];
                currentView.ComposeFilter = "A" + IDGroup;
                currentView.Employees = [];

                let response = await fetch("/Cookie/SetUniversalSelector", {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ sSelectorName: 'calendarController', sSelectorValue: JSON.stringify(currentView) })
                });

                if (!response.ok) {
                    throw new Error('Error en la solicitud POST');
                }

                // Convertir la respuesta a JSON
                await response.json();

                let textodates = FormateaFecha(moment(strStartDate).toDate()) + "," + FormateaFecha(moment(strEndDate).toDate());
                localStorage.setItem('SchedulerIntervalDates', textodates);
                localStorage.setItem('OverwriteCalendarDates', true);

            } catch (e) {
            }

            window.open(calendarURL, "_blank");
        }
    }

    _ajax.send(null)
}

//Cambia els tabs superiors del selector
function chTopTabList(tabSelected) {
    var tab01Filter = document.getElementById('tabMyRequestsListFilter');
    var tab01Content = document.getElementById('tabMyRequestsListContent');
    var tab01Bottom = document.getElementById('tabMyRequestsListBottom');
    var tab02Filter1 = document.getElementById('tabOtherRequestsListFilter1');
    var tab02Content = document.getElementById('tabOtherRequestsListContent');
    var tab02Bottom = document.getElementById('tabOtherRequestsListBottom');
    var tabT01 = document.getElementById('tabMyRequestsListTitle');
    var tabT02 = document.getElementById('tabOtherRequestsListTitle');

    if (tabSelected == 'MyRequests') {
        if (tab01Filter != null) { tab01Filter.style.display = ''; }
        if (tab01Content != null) { tab01Content.style.display = ''; }
        if (tab01Bottom != null) { tab01Bottom.style.display = ''; }
        if (tab02Filter1 != null) { tab02Filter1.style.display = 'none'; }
        if (tab02Content != null) { tab02Content.style.display = 'none'; }
        if (tab02Bottom != null) { tab02Bottom.style.display = 'none'; }
        if (tabT01 != null) { tabT01.className = 'tabt_l_active'; }
        if (tabT02 != null) { tabT02.className = 'tabt_r'; }
        actualList = 0;
        if (MyListLoaded == false) loadRequests(0, false, undefined, 'false');
    }
    if (tabSelected == 'OtherRequests') {
        if (tab01Filter != null) { tab01Filter.style.display = 'none'; }
        if (tab01Content != null) { tab01Content.style.display = 'none'; }
        if (tab01Bottom != null) { tab01Bottom.style.display = 'none'; }
        if (tab02Filter1 != null) { tab02Filter1.style.display = ''; }
        if (tab02Content != null) { tab02Content.style.display = ''; }
        if (tab02Bottom != null) { tab02Bottom.style.display = ''; }
        if (tabT01 != null) { tabT01.className = 'tabt_l'; }
        if (tabT02 != null) { tabT02.className = 'tabt_r_active'; }
        actualList = 1;
        if (OtherListLoaded == false) loadRequests(1, false, undefined, 'false');
    }

    TabListClick(tabSelected);
}

function TabListClick(tabSelected) {
    try {
    } catch (e) { showError("TabListClick", e); }
}

function downloadRequestDoc(idDocument) {
    var url = '../Employees/DocumentVisualize.aspx?DeliveredDocument=' + idDocument;
    var Title = 'Documentos';
    window.open(url);
}

//Cambia els Tabs i els divs
function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('divRequestLists', 'divRequestLists2');

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

    if (actualTab == 1) {
        if (HistoryListLoaded == false) loadRequests(2, false, undefined, 'false');
    }
}