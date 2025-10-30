var actualTab = 0; // TAB per mostrar
var actualCause; // Cause actual
var ctrlGridDocumentTrace = null;
var newObjectName = "";
var cancelOPCEvents = false;

var actualTypeconfigure = -1;

function checkCauseEmptyName(newName) {
    document.getElementById("readOnlyNameCause").textContent = newName;
    hasChanges(true, false);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    var contenedor2 = document.getElementById('divContenido');

    if (s.cpActionRO == "GETCAUSE" || s.cpActionRO == "SAVECAUSE") {
        cancelOPCEvents = true;

        EnablePanelsFunctionallity();
        ConvertControls('divContenido');
        causeTypeVisible(cmbCauseTypeClient.GetSelectedItem());

        cancelOPCEvents = false;

        if (s.cpResultRO == "OK") {
            if (s.cpIsNew == true) {
                refreshTree();
                return;
            }

            if (s.cpNameRO != null && s.cpNameRO != "") {
                var arrTZ = null;
                if (s.cpDocumentsGridRO != "") eval("arrTZ = " + s.cpDocumentsGridRO);
                if (arrTZ == null) arrTZ = new Array()
                createGridDocumentTrace(arrTZ);

                document.getElementById("readOnlyNameCause").textContent = s.cpNameRO;
                hasChanges(false, false);
                ASPxClientEdit.ValidateGroup(null, true);
            } else {
                document.getElementById("readOnlyNameCause").textContent = newObjectName;
                hasChanges(true, false);
                txtName_Client.SetValue(newObjectName);
            }

            if (s.cpActionRO == "SAVECAUSE") {
                actualCause = parseInt(s.cpNewIdRO, 10);
                refreshTree();
            }
        } else {
            hasChanges(true, false);

            var arrTZ = null;
            if (s.cpDocumentsGridRO != "") eval("arrTZ = " + s.cpDocumentsGridRO);
            if (arrTZ == null) arrTZ = new Array()
            createGridDocumentTrace(arrTZ);

            var result = new Array();
            result.push(s.cpMessageRO);
            checkStatus(result);
        }

        showLoadingGrid(false);
    }

    if (document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnReadOnlyMode").value == "1") changeStateVisibilityTab(false, true, true, true);
}

function checkStatus(oStatus) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (typeof (objError.Error) != 'undefined') {
            if (objError.Error == true) {
                if (objError.TypeMsg == "PopupMsg") { //Missatge estil pop-up
                    var url = "Causes/srvMsgBoxCauses.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                        "DescriptionText=" + encodeURIComponent(objError.ErrorText) + "&" +
                        "Option1TextKey=SaveName.Error.Option1Text&" +
                        "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                        "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                    parent.ShowMsgBoxForm(url, 400, 300, '');
                } else { //Missatge estil inline
                }
            }
        } else {
            if (objError.error == "true") {
                if (objError.typemsg == "1") { //Missatge estil pop-up
                    var url = "Causes/srvMsgBoxCauses.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                        "DescriptionText=" + objError.msg + "&" +
                        "Option1TextKey=SaveName.Error.Option1Text&" +
                        "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                        "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                        "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                    parent.ShowMsgBoxForm(url, 400, 300, '');
                } else { //Missatge estil inline
                }
            }
        }
    } catch (e) { showError("checkStatus", e); }
}

function cargaNodo(Nodo) {
    try {
        var oID = Nodo.id;
        if (isNaN(oID)) { oID = -1; }
        actualCause = oID;
        cargaCause(actualCause);
    } catch (e) { showError("cargaNodo", e); }
}

//Carga Tabs y contenido Empleados
function cargaCause(IDCause) {
    try {
        resetUserCriteriaControl('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria_punchesCriteria');
        resetUserCriteriaControl('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria_visibilityCriteria');
        actualCause = IDCause;
        showLoadingGrid(true);
        cargaCauseTabSuperior(IDCause);
    }
    catch (e) {
        showError("cargaCause", e);
    }
}

//Carrega la part del TAB grisa superior
function cargaCauseTabSuperior(IDCause) {
    try {
        var Url = "Handlers/srvCauses.ashx?action=getCauseTab&aTab=" + actualTab + "&ID=" + IDCause;
        AsyncCall("POST", Url, "CONTAINER", "divCause", "cargaCauseBarButtons(" + IDCause + ");");
    }
    catch (e) {
        showError("cargaCauseTabSuperior", e);
    }
}
var responseObj = null;
//Carrega la part del TAB grisa superior
function cargaCauseBarButtons(IDCause) {
    try {
        var Url = "Handlers/srvCauses.ashx?action=getBarButtons&ID=" + IDCause;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDCause + ")");
    }
    catch (e) {
        showError("cargaCauseBarButtons", e);
    }
}

function parseResponseBarButtons(oResponse, IDCause) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaCauseDivs(IDCause);
}

//Carrega els apartats dels divs de l'usuari
function cargaCauseDivs(IDCause) {
    try {
        var oParameters = {};
        oParameters.aTab = actualTab;
        oParameters.ID = actualCause;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "GETCAUSE";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    } catch (e) { showError("cargaCauseDivs", e); }
}

//Cambia els Tabs i els divs
function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03', 'TABBUTTON_04', 'TABBUTTON_05');
    arrDivs = new Array('div00', 'div01', 'div02', 'div03', 'div04', 'div05');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (n == numTab) {
            tab.className = 'bTab-active';
            div.style.display = '';
        } else {
            if (tab != null) {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }
    actualTab = numTab;
}

//Cambia els Tabs i els divs (per nom)
function changeTabsByName(nameTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02', 'TABBUTTON_03', 'TABBUTTON_04', 'TABBUTTON_05');
    arrDivs = new Array('div00', 'div01', 'div02', 'div03', 'div04', 'div05');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (div.id == nameTab) {
            tab.className = 'bTab-active';
            div.style.display = '';
            actualTab = n;
        } else {
            if (tab != null) {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }
}

//Mostra el ToolTip a la barra d'eines
function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

//Amaga el ToolTip a la barra d'eines
function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function hasChanges(bolChanges, markRecalc) {
    var needToRecalc = false;
    if (typeof (markRecalc) != "undefined") needToRecalc = markRecalc;
    else needToRecalc = true;

    var divTop = document.getElementById('divMsgTop');
    var divBottom = document.getElementById('divMsgBottom');

    if (!cancelOPCEvents) {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value == "0") {
            if (bolChanges && needToRecalc == true) {
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value = "1";
            } else {
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value = "0";
            }
        }
    }

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

function refreshBeforeSave(arrStatus) {
    refreshTree();
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

function undoChanges() {
    try {
        if (actualCause == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesCauses";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaCause(actualCause);
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
    var ctlPrefix = "ctl00_contentMainBody_roTreesCauses";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, false, false);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesCauses";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Causes/srvMsgBoxCauses.aspx?action=Message";
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

function EnablePanelsFunctionallity() {
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysCause');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysCause');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optDaysCause_optDaysFromCause');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCustomized');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCustomized');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optProductiveHoursOnCC');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optExternalProductiveHours');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optProductiveHoursOnCC,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optExternalProductiveHours');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_optAbsenceMandatoryDays');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_optCauseAbsences');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_optCauseCloseAbsences');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_optCauseNotRequieresPermission');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptValueUp');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptValueDown');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptValueAprox');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptValueUp,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptValueDown,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptValueAprox');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptIndivRound');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptOneInDay');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptIndivRound,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptOneInDay');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityAll');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityNobody');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityNobody,ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesAll');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesNobody');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesNobody,ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyNothing');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyPeriod');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyNothing,ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyPeriod');

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptAllRequestTypes');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptLeaveRequest');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptCustomRequestList');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptNoneRequestList');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptAllRequestTypes,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptLeaveRequest,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptCustomRequestList,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptNoneRequestList');

    var opNoConnector = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyNothing_rButton').checked;

    if (opNoConnector == true) {
        chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyNothing,ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyPeriod', 'undefined');
    } else {
        chgOPCItems('1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyNothing,ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyPeriod', 'undefined');
    }

    ActivateMaxDays();
    ActivateTimePeriodAllowed();
    ActivateTimeBetween();
    ActivateJustifyPanel();
}

function causeTypeVisible(s) {
    $('#rowCauseHours').hide();
    $('#rowCauseDays').hide();
    $('#rowCauseCustom').hide();

    switch (s.value) {
        case 0:
            $('#rowCauseHours').show();
            changeStateVisibilityTab(true, true, true, true);
            break;
        case 1:
            $('#rowCauseDays').show();
            changeStateVisibilityTab(false, true, true, true);
            break;
        case 2:
            $('#rowCauseCustom').show();
            changeStateVisibilityTab(false, true, true, true);
            break;
    }
}

function showMsg(oMsg) {
    alert(oMsg);
}

function checkCriteriaVisibility() {
    if (document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria_rButton").checked == false) {
        document.getElementById("visibilityCell").style.display = "none";
    } else {
        document.getElementById("visibilityCell").style.display = "";
    }
}

function checkPunchesVisibility() {
    if (document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria_rButton").checked == false) {
        document.getElementById("punchesCell").style.display = "none";
    } else {
        document.getElementById("punchesCell").style.display = "";
    }
}

function ActivateMaxDays() {
    var ctrl = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_GroupBox2_chkMaxDays");
    if (ctrl.checked) {
        txtDaysDurationClient.SetEnabled(true);
    } else {
        txtDaysDurationClient.SetEnabled(false);
    }
}

function ActivateTimePeriodAllowed() {
    var ctrl = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_GroupBox2_chkTimePeriodAllowed");
    if (ctrl.checked) {
        txtFromPeriodClient.SetEnabled(true);
        txtToPeriodClient.SetEnabled(true);
    } else {
        txtFromPeriodClient.SetEnabled(false);
        txtToPeriodClient.SetEnabled(false);
    }
}

function ActivateTimeBetween() {
    var ctrl = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_GroupBox2_chkTimeBetween");
    if (ctrl.checked) {
        txtMinDurationClient.SetEnabled(true);
        txtMaxDurationClient.SetEnabled(true);
    } else {
        txtMinDurationClient.SetEnabled(false);
        txtMaxDurationClient.SetEnabled(false);
    }
}

function ActivateJustifyPanel() {
    var ctrl = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_chkButton");
    if (ctrl.checked) {
        mskJustifyPeriodsStartClient.SetEnabled(true);
        mskJustifyPeriodsEndClient.SetEnabled(true);
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyNothing_panOptionPanel').setAttribute('venabled', 'True');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyPeriod_panOptionPanel').setAttribute('venabled', 'True');
    }
    else {
        mskJustifyPeriodsStartClient.SetEnabled(false);
        mskJustifyPeriodsEndClient.SetEnabled(false);
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyNothing_panOptionPanel').setAttribute('venabled', 'False');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opCheckIncidence_opJustifyPeriod_panOptionPanel').setAttribute('venabled', 'False');
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

function createGridDocumentTrace(arrFieldsGrid) {
    try {
        var headerGrid = [{ 'fieldname': 'idDocumentTrace', 'description': '', 'size': '-1' },
        { 'fieldname': 'idCause', 'description': '', 'size': '-1' },
        { 'fieldname': 'idLabAgree', 'description': '', 'size': '-1' },
        { 'fieldname': 'idDocument', 'description': '', 'size': '-1' },
        { 'fieldname': 'typeRequest', 'description': '', 'size': '-1' },
        { 'fieldname': 'numItems', 'description': '', 'size': '-1' },
        { 'fieldname': 'numItems2', 'description': '', 'size': '-1' },
        { 'fieldname': 'flexibleWhen', 'description': '', 'size': '-1' },
        { 'fieldname': 'flexibleWhen2', 'description': '', 'size': '-1' },
        { 'fieldname': 'nameDocument', 'description': '', 'size': '30%' },
        { 'fieldname': 'nameInterval', 'description': '', 'size': '50' },
        { 'fieldname': 'nameLabAgree', 'description': '', 'size': '20%' }]

        headerGrid[0].description = "idDoc";
        headerGrid[1].description = "idCause";
        headerGrid[2].description = "idLab"
        headerGrid[3].description = "idDoc";
        headerGrid[4].description = "type";
        headerGrid[5].description = "items"
        headerGrid[6].description = "items2"
        headerGrid[7].description = "Flex"
        headerGrid[8].description = "Flex2"
        headerGrid[9].description = document.getElementById('hdnDocumentTitle').value;
        headerGrid[10].description = document.getElementById('hdnIntervalTitle').value;
        headerGrid[11].description = document.getElementById('hdnLabAgreeTitle').value;

        var edtRow = true;
        var delRow = true;

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true") {
            edtRow = false;
            delRow = false;
        }

        ctrlGridDocumentTrace = new jsGrid("ctl00_contentMainBody_ASPxCallbackPanelContenido_gridDocumentTrace", headerGrid, arrFieldsGrid, edtRow, delRow, false, "DocumentTraceFieldsList");
    }
    catch (e) {
        showError("createGridDocumentTrace", e);
    }
}

function EditDocumentTrace(isNew, objDocument) {
    try {
        if (cmbCauseTypeClient.GetSelectedItem().value == 0) {
            var oDis = document.getElementById('ctl00_contentMainBody_hdnModeEdit').value;
            var nameControls = "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1,ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2"
            if (isNew) {
                cmbDocumentClient.SetValue("");
                cmbLabAgreeClient.SetValue("0");
                cmbDocumentFirstTimeClient.SetValue("0");
                loadGenericData("TXTDOCUMENTFIRSTTIMEDURATION", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_txtDocumentFirstTimeDuration", "0", "X_NUMBER", "", oDis, false);
                loadGenericData("TXTDOCUMENTEVERYTIMEDURATION", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration", "0", "X_NUMBER", "", oDis, false);
                loadGenericData("TXTDOCUMENTEVERYTIMEDURATION2", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration2", "0", "X_NUMBER", "", oDis, false);
                loadGenericData("OPTIONS", nameControls, 0, "X_OPTIONGROUP", "", oDis, false);
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_IdDocumentTrace').value = "-1";
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_IdRow').value = "-1";
                chgOPCItems(0, nameControls, 'undefined');
                ChangeDocumentFirstTime(0);
            } else {
                if (cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_0") != null) {
                    cmbDocumentClient.SetSelectedItem(cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_0"));
                } else if (cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_1") != null) {
                    cmbDocumentClient.SetSelectedItem(cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_1"));
                } else if (cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_2") != null) {
                    cmbDocumentClient.SetSelectedItem(cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_2"));
                } else if (cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_3") != null) {
                    cmbDocumentClient.SetSelectedItem(cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_3"));
                } else if (cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_4") != null) {
                    cmbDocumentClient.SetSelectedItem(cmbDocumentClient.FindItemByValue(objDocument.IdDocument + "_4"));
                }

                cmbLabAgreeClient.SetValue(objDocument.idLabAgree);
                cmbDocumentFirstTimeClient.SetValue(objDocument.cmbDocumentFirstTime);
                loadGenericData("TXTDOCUMENTFIRSTTIMEDURATION", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_txtDocumentFirstTimeDuration", objDocument.txtDocumentFirstTimeDuration, "X_NUMBER", "", oDis, false);
                loadGenericData("TXTDOCUMENTEVERYTIMEDURATION", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration", objDocument.txtDocumentEveryTimeDuration, "X_NUMBER", "", oDis, false);
                loadGenericData("TXTDOCUMENTEVERYTIMEDURATION2", "ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration2", objDocument.txtDocumentEveryTimeDuration2, "X_NUMBER", "", oDis, false);
                if (objDocument.typeRequest == "0" || objDocument.typeRequest == "1" || objDocument.typeRequest == "5") {
                    loadGenericData("OPTIONS", nameControls, 0, "X_OPTIONGROUP", "", oDis, false);
                    chgOPCItems(0, nameControls, 'undefined');
                } else {
                    loadGenericData("OPTIONS", nameControls, 1, "X_OPTIONGROUP", "", oDis, false);
                    chgOPCItems(1, nameControls, 'undefined');
                }
                if (objDocument.cmbDocumentFirstTime == "0" || objDocument.cmbDocumentFirstTime == "1")
                    ChangeDocumentFirstTime(0);
                else
                    ChangeDocumentFirstTime(1);
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_IdDocumentTrace').value = objDocument.IdDocumentTrace;
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_IdRow').value = objDocument.IdRow;
            }
            disableScreen(true);
            showWndCompositions(true);
        } else {
            showErrorPopup("Error.Title", "error", "Error.Documents.OnlyInAbsence", "", "Error.OK", "Error.OKDesc", "");
        }
    }
    catch (e) {
        showError("EditDocumentTrace", e);
    }
}

function validateDocumentTrace() {
    try {
        //DOCUMENTO
        if (cmbDocumentClient.GetSelectedItem() == null || (cmbDocumentClient.GetSelectedItem() != null && parseInt(cmbDocumentClient.GetSelectedItem().value.split("_")[0], 10) < 1)) {
            showErrorPopup("Error.Title", "error", "Error.Description.NoDocumentSelected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        //CONVENIO
        if (cmbLabAgreeClient.GetSelectedItem() == null || (cmbLabAgreeClient.GetSelectedItem() != null && parseInt(cmbLabAgreeClient.GetSelectedItem().value, 10) < 0)) {
            showErrorPopup("Error.Title", "error", "Error.Description.NoLabAgreeSelected", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        //selector de cuando en una unica vez
        var docType = parseInt(cmbDocumentClient.GetSelectedItem().value.split("_")[1], 10);
        if (docType == 0 || docType == 1) {
            var when = parseInt(cmbDocumentFirstTimeClient.GetSelectedItem().value, 10);

            if (docType == 0 && when != 0 && when != 2) {
                showErrorPopup("Error.Title", "error", "Error.Leave.NoStartSelected", "", "Error.OK", "Error.OKDesc", "");
                return false;
            } else if (docType == 1 && when != 1 && when != 3) {
                showErrorPopup("Error.Title", "error", "Error.Leave.NoEndSelected", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
        }

        var cmbDocumentFirstTime = "0";
        var cmbDayWeekMonth = "0";
        var cmbBeginEndNext = "0";
        var typeRequest = 0;
        var numItems = -1;
        var numItems2 = 0;

        //PRIMER GRUPO DE OPCIONES
        var optDocumentOptions1 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_rButton").checked
        if (optDocumentOptions1 == true) {
            cmbDocumentFirstTime = cmbDocumentFirstTimeClient.GetValue();
            if (cmbDocumentFirstTime == "2" || cmbDocumentFirstTime == "3") {
                numItems = parseInt(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_txtDocumentFirstTimeDuration').value);
                if (numItems <= 0) {
                    showErrorPopup("Error.Title", "error", "Error.Description.NoQuantitySelected", "", "Error.OK", "Error.OKDesc", "");
                    return false;
                }
            }
        } else { //SEGUNDO GRUPO DE OPCIONES
            numItems = parseInt(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration').value);
            if (numItems <= 0) {
                showErrorPopup("Error.Title", "error", "Error.Description.NoQuantitySelected", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }

            cmbDayWeekMonth = 0;//cmbDayWeekMonthClient.GetValue();
            numItems2 = parseInt(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration2').value);
            if (numItems2 < 0) {
                showErrorPopup("Error.Title", "error", "Error.Description.NoQuantitySelected", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
            cmbBeginEndNext = 0;//cmbBeginEndNextClient.GetValue();
        }

        if (optDocumentOptions1 == true) {
            if (cmbDocumentFirstTime == "0") {
                typeRequest = 0;
            } else {
                if (cmbDocumentFirstTime == "1") {
                    typeRequest = 1;
                } else {
                    if (cmbDocumentFirstTime == "2" || cmbDocumentFirstTime == "3") {
                        typeRequest = 5;
                        if (cmbDocumentFirstTime == "2") cmbBeginEndNext = "0";
                        else cmbBeginEndNext = "1";
                    }
                }
            }
        } else {
            if (numItems2 > 0) {
                typeRequest = 6;
            } else {
                if (cmbDayWeekMonth == "0") {
                    typeRequest = 2;
                } else {
                    if (cmbDayWeekMonth == "1") {
                        typeRequest = 3;
                        cmbDayWeekMonth = "0";
                    } else {
                        typeRequest = 4;
                        cmbDayWeekMonth = "0";
                    }
                }
            }
        }

        if (checkIfExistDocumentInGrid(parseInt(cmbLabAgreeClient.GetSelectedItem().value, 10), cmbDocumentClient.GetSelectedItem().value) == true) {
            showErrorPopup("Error.Title", "error", "Error.Description.DuplicatedDocument", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }

        return true;
    }
    catch (e) {
        showError("validateDocumentTrace", e); return false;
    }
}

function saveDocumentTrace() {
    try {
        if (validateDocumentTrace()) {
            var grid = ctrlGridDocumentTrace;
            var idDocumentTrace = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_IdDocumentTrace').value;
            var idRow = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_IdRow').value;
            var idCause = actualCause;

            var idLabAgree = cmbLabAgreeClient.GetSelectedItem().value;
            var idDocument = cmbDocumentClient.GetSelectedItem().value.split("_")[0];
            var nameLabAgree = cmbLabAgreeClient.GetSelectedItem().text;
            var nameDocument = cmbDocumentClient.GetSelectedItem().text;

            var typeRequest = 0;
            var numItems = -1;
            var numItems2 = 0;
            var flexibleWhen = "0";
            var flexibleWhen2 = "0";
            var strDef = "";

            var optDocumentOptions1 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_rButton").checked
            if (optDocumentOptions1 == true) {
                var cmbDocumentFirstTime = cmbDocumentFirstTimeClient.GetValue();

                if (cmbDocumentFirstTime == "0") {
                    typeRequest = 0;
                    strDef = document.getElementById('hdnAbsencesAtBegin').value;
                }
                else {
                    if (cmbDocumentFirstTime == "1") {
                        typeRequest = 1;
                        strDef = document.getElementById('hdnAbsencesAtEnd').value;
                    }
                    else {
                        if (cmbDocumentFirstTime == "2" || cmbDocumentFirstTime == "3") {
                            typeRequest = 5;
                            numItems = parseInt(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_txtDocumentFirstTimeDuration').value);
                            strDef = document.getElementById('hdnAbsencesEveryFlexible1').value;
                            strDef = strDef.replace("XXX", numItems.toString());
                            if (cmbDocumentFirstTime == "2") {
                                flexibleWhen = "0";
                                strDef = strDef.replace("YYY", "Inicio");
                            }
                            else {
                                flexibleWhen = "1";
                                strDef = strDef.replace("YYY", "Final");
                            }
                        }
                    }
                }
            }
            else { //optDocumentOptions2 == true
                var cmbBeginEndNext = 0; //cmbBeginEndNextClient.GetSelectedItem().value;

                var cmbDayWeekMonth = 0; //cmbDayWeekMonthClient.GetSelectedItem().value;

                numItems = parseInt(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration').value, 10);
                numItems2 = parseInt(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration2').value, 10);

                flexibleWhen = cmbBeginEndNext;

                if (numItems2 > 0) {
                    typeRequest = 6;
                    flexibleWhen2 = cmbDayWeekMonth;
                    strDef = document.getElementById('hdnAbsencesEveryFlexible2').value;
                    strDef = strDef.replace("XXX", numItems.toString());
                    strDef = strDef.replace("ZZZ", numItems2.toString());
                }
                else {
                    flexibleWhen2 = "0";
                    if (cmbDayWeekMonth == "0") {
                        typeRequest = 2;
                        strDef = document.getElementById('hdnAbsencesEveryDays').value;
                        strDef = strDef.replace("XXX", numItems.toString());
                    }
                    else {
                        if (cmbDayWeekMonth == "1") {
                            typeRequest = 3;
                            strDef = document.getElementById('hdnAbsencesEveryWeeks').value;
                            strDef = strDef.replace("XXX", numItems.toString());
                        }
                        else {
                            typeRequest = 4;
                            strDef = document.getElementById('hdnAbsencesEveryMonths').value;
                            strDef = strDef.replace("XXX", numItems.toString());
                        }
                    }
                }
            }

            var arrValues = [{ field: 'idDocumentTrace', value: idDocumentTrace },
            { field: 'idCause', value: idCause },
            { field: 'idLabAgree', value: idLabAgree },
            { field: 'idDocument', value: idDocument },
            { field: 'typeRequest', value: typeRequest },
            { field: 'numItems', value: numItems.toString() },
            { field: 'numItems2', value: numItems2.toString() },
            { field: 'flexibleWhen', value: flexibleWhen },
            { field: 'flexibleWhen2', value: flexibleWhen2 },
            { field: 'nameDocument', value: nameDocument },
            { field: 'nameInterval', value: strDef },
            { field: 'nameLabAgree', value: nameLabAgree }];

            if (idRow == "-1") {
                grid.createRow(arrValues, null);
            }
            else {
                grid.deleteRow(idRow);
                grid.createRow(arrValues, null);
            }

            hasChanges(true, false);

            //Tanquem finestra
            disableScreen(false);
            showWndCompositions(false);
        }
    }
    catch (e) {
        showError("saveDocumentTrace", e);
    }
}

function checkIfExistDocumentInGrid(idLabAgree, idDocument) {
    try {
        var grid = ctrlGridDocumentTrace;
        if (grid != null) {
            var idRow = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_IdRow').value

            var arrRows = grid.getRows();
            for (var n = 0; n < arrRows.length; n++) {
                var hRow = arrRows[n];

                var fDocument = parseInt(idDocument.split(":")[0], 10);
                var fTypeDocument = parseInt(idDocument.split(":")[1], 10);

                var cLabAgree = parseInt(hRow.cells[3].innerHTML, 10);
                var cDocument = parseInt(hRow.cells[4].innerHTML, 10);

                if (idRow == "-1") {
                    if (((cLabAgree == idLabAgree) || (cLabAgree == 0 && idLabAgree != 0) || (idLabAgree == 0 && cLabAgree != 0)) && cDocument == fDocument) {
                        return true;
                    }
                }
                else {
                    if (hRow.id != idRow && ((cLabAgree == idLabAgree) || (cLabAgree == 0 && idLabAgree != 0) || (idLabAgree == 0 && cLabAgree != 0)) && cDocument == fDocument) {
                        return true;
                    }
                }
            }
        }
        else {
            return false;
        }
    }
    catch (e) {
        showError("checkIfExistDocumentInGrid", e);
    }
}

function ChangeDocumentFirstTime(index) {
    if (index == 0) {
        document.getElementById("tdDocumentFirstTimeDaysDuration").style.display = 'none';
        document.getElementById("tdDocumentFirstTimeDaysDuration2").style.display = '';
    }
    else {
        document.getElementById("tdDocumentFirstTimeDaysDuration").style.display = '';
        document.getElementById("tdDocumentFirstTimeDaysDuration2").style.display = 'none';
    }
}

function cancelDocumentTrace() {
    disableScreen(false);
    showWndCompositions(false)
}

function showWndCompositions(bol) {
    try {
        var divC = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_frm');
        if (divC != null) {
            if (bol == true) {
                divC.style.display = '';
                divC.style.marginLeft = ((divC.offsetWidth / 2) * -1) + "px";
                divC.style.marginTop = ((divC.offsetHeight / 2) * -1) + "px";
            }
            else {
                divC.style.display = 'none';
            }
        }
    }
    catch (e) {
        showError("showWndCompositions", e);
    }
}

function editGridDocumentTraceFieldsList(sId) {
    try {
        var grid = ctrlGridDocumentTrace;
        if (grid != null) {
            //obtener la fila del grid
            var tmpRow = grid.retRowJSON(sId);

            var objDocument = new Object();
            objDocument.IdRow = sId;
            objDocument.IdDocumentTrace = tmpRow[0].value;
            objDocument.idLabAgree = tmpRow[2].value;
            objDocument.IdDocument = tmpRow[3].value;
            objDocument.typeRequest = tmpRow[4].value;;

            objDocument.cmbDocumentFirstTime = "0";
            objDocument.txtDocumentFirstTimeDuration = 0;

            objDocument.txtDocumentEveryTimeDuration = 0
            objDocument.cmbDayWeekMonth = "0";
            objDocument.txtDocumentEveryTimeDuration2 = 0;
            objDocument.cmbBeginEndNext = "0";

            var numberItems = parseInt(tmpRow[5].value);
            var numberItems2 = parseInt(tmpRow[6].value);

            if (objDocument.typeRequest == "0") {
                objDocument.cmbDocumentFirstTime = "0";
            }
            else {
                if (objDocument.typeRequest == "1") {
                    objDocument.cmbDocumentFirstTime = "1";
                }
                else {
                    if (objDocument.typeRequest == "5") {
                        var cmbBeginEndNext = tmpRow[7].value;
                        if (cmbBeginEndNext == "0") {
                            objDocument.cmbDocumentFirstTime = "2";
                        }
                        else {
                            objDocument.cmbDocumentFirstTime = "3";
                        }
                        objDocument.txtDocumentFirstTimeDuration = numberItems;
                    }
                    else {
                        if (objDocument.typeRequest == "2") {
                            objDocument.cmbDayWeekMonth = "0";
                            objDocument.txtDocumentEveryTimeDuration = numberItems;
                        }
                        else {
                            if (objDocument.typeRequest == "3") {
                                objDocument.cmbDayWeekMonth = "1";
                                objDocument.txtDocumentEveryTimeDuration = numberItems;
                            }
                            else {
                                if (objDocument.typeRequest == "4") {
                                    objDocument.cmbDayWeekMonth = "2";
                                    objDocument.txtDocumentEveryTimeDuration = numberItems;
                                }
                                else { //6
                                    objDocument.txtDocumentEveryTimeDuration = numberItems;
                                    if (numberItems2 < 0) numberItems2 = 0;
                                    objDocument.txtDocumentEveryTimeDuration2 = numberItems2;
                                    objDocument.cmbDayWeekMonth = tmpRow[8].value;
                                    objDocument.cmbBeginEndNext = tmpRow[7].value;
                                }
                            }
                        }
                    }
                }
            }

            EditDocumentTrace(false, objDocument);
        }
    }
    catch (e) {
        showError("editGridDocumentTraceFieldsList", e);
    }
}

function deleteGridDocumentTraceFieldsList(sId) {
    try {
        var grid = ctrlGridDocumentTrace;
        if (grid != null) {
            //obtener la fila del grid
            var tmpRow = grid.retRowJSON(sId);

            //borrar la fila del grid
            grid.deleteRow(sId);

            hasChanges(true, false);
        }
    }
    catch (e) {
        showError("deleteGridDocumentTraceFieldsList", e);
    }
}

function saveChanges() {
    try {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnRecalcChanges').value == "1") {
                if (parseInt(actualCause, 10) >= 0) {
                    isUsed();
                } else {
                    saveDefChanges();
                }
            } else {
                saveDefChanges();
            }
        } else {
            showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        };
    }
    catch (e) {
        showError("saveChanges", e);
    }
}

function isUsed() {
    try {
        showLoadingGrid(true);
        AsyncCall("POST", "Handlers/srvCauses.ashx?action=causeIsUsed&ID=" + actualCause, "json", "arrStatus", "showLoadingGrid(false); checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ retIsUsed(arrStatus); }")
    } catch (e) { showError("isUsed", e); }
}

function retIsUsed(arrStatus) {
    try {
        if (arrStatus[0].msg.toUpperCase() == "CAUSEUSEDNO") {
            saveDefChanges();
        } else {
            var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=SAVECAUSE&ShowFreezingDate=1";
            CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
            CaptchaObjectPopup_Client.Show();
        }
    } catch (e) { showError("retIsUsed", e); }
}

function captchaCallback(action) {
    switch (action) {
        case "SAVECAUSE":
            saveDefChanges();
            break;
        case "ERROR":
            showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "", "");
            break;
    }
}

function saveDefChanges() {
    try {
        showLoadingGrid(true);

        var arrDocumentRows = null
        if (ctrlGridDocumentTrace != null) {
            arrDocumentRows = ctrlGridDocumentTrace.toJSONStructureAdvanced();
        }

        var oParameters = {};
        oParameters.aTab = actualTab;
        oParameters.ID = actualCause;
        oParameters.Name = document.getElementById("readOnlyNameCause").textContent.trim();
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SAVECAUSE";
        if (arrDocumentRows != "undefined" && arrDocumentRows != null) oParameters.gridDocuments = arrDocumentRows;

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);
        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    } catch (e) {
        showError("saveDefChanges", e);
    }
}

function newCause() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=Cause";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newCamera', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaCause(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

function ShowRemoveCause() {
    try {
        if (actualCause == -1 || actualCause == 0) { return; }

        var url = "Causes/srvMsgBoxCauses.aspx?action=Message";
        url = url + "&TitleKey=deleteCause.Title";
        url = url + "&DescriptionKey=deleteCause.Description";
        url = url + "&Option1TextKey=deleteCause.Option1Text";
        url = url + "&Option1DescriptionKey=deleteCause.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteCause('" + actualCause + "'); return false;";
        url = url + "&Option2TextKey=deleteCause.Option2Text";
        url = url + "&Option2DescriptionKey=deleteCause.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveCause", e); }
}

function deleteCause(Id) {
    try {
        if (Id == "-1" || Id == "0") {
        } else {
            showLoadingGrid(true);
            AsyncCall("POST", "Handlers/srvCauses.ashx?action=deleteXCause&ID=" + Id, "json", "arrStatus", "showLoadingGrid(false); checkStatus(arrStatus,true); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
        }
    } catch (e) { showError('deleteCause', e); }
}

function causeTypeChange() {
    cmbConceptBalanceClient.SetEnabled(false);
    cmbAbsenceConceptBalanceClient.SetEnabled(false);
    cmbAbsenceConceptBalanceDaysClient.SetEnabled(false);
    cmbConceptBalanceProductiveClient.SetEnabled(false);
    cmbConceptBalanceProductiveDaysClient.SetEnabled(false);

    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_rButton').checked == true) {
        cmbConceptBalanceProductiveClient.SetEnabled(true);
        cmbConceptBalanceProductiveDaysClient.SetEnabled(true);
        chgOPCItems('-1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause', 'undefined', false);
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optProductiveHoursOnCC_rButton').checked == false &&
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optExternalProductiveHours_rButton').checked == false) {
            chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optProductiveHoursOnCC,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optExternalProductiveHours', 'undefined', false);
        }
    } else if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_rButton').checked == true) {
        chgOPCItems('-1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optProductiveHoursOnCC,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optExternalProductiveHours', 'undefined', false);
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday_rButton').checked == false &&
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_rButton').checked == false) {
            chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause', 'undefined', false);
        }
    }

    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday_rButton').checked == true) cmbConceptBalanceClient.SetEnabled(true);
    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_rButton').checked == true) {
        cmbAbsenceConceptBalanceClient.SetEnabled(true);
        cmbAbsenceConceptBalanceDaysClient.SetEnabled(true);
    }

    if (!cancelOPCEvents) hasChanges(true, true);

    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_rButton').checked == true) {
        $('#divAvailableAbsenceRequests').hide();
        $('#divAvailableOverWorkRequests').show();
        $('#divAvailableHolidayRequests').hide();
    } else {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday_rButton').checked == true) {
            $('#divAvailableAbsenceRequests').hide();
            $('#divAvailableOverWorkRequests').hide();
            $('#divAvailableHolidayRequests').show();
        } else {
            $('#divAvailableAbsenceRequests').show();
            $('#divAvailableOverWorkRequests').hide();
            $('#divAvailableHolidayRequests').hide();
        }
    }
}

function causeProductive() {
    cmbConceptBalanceClient.SetEnabled(false);

    cmbAbsenceConceptBalanceClient.SetEnabled(false);
    cmbAbsenceConceptBalanceDaysClient.SetEnabled(false);

    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_rButton').checked == true) {
        cmbConceptBalanceProductiveClient.SetEnabled(true);
        cmbConceptBalanceProductiveDaysClient.SetEnabled(true);
        chgOPCItems('-1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause', 'undefined', false);
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optProductiveHoursOnCC_rButton').checked == false &&
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optExternalProductiveHours_rButton').checked == false) {
            chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optProductiveHoursOnCC,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optExternalProductiveHours', 'undefined', false);

            $('#divAvailableAbsenceRequests').show();
            $('#divAvailableOverWorkRequests').hide();
            $('#divAvailableHolidayRequests').hide();
        }
    } else {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optProductiveHoursOnCC_rButton').checked == true ||
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optExternalProductiveHours_rButton').checked == true) {
            chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive', 'undefined', false);

            $('#divAvailableAbsenceRequests').hide();
            $('#divAvailableOverWorkRequests').hide();
            $('#divAvailableHolidayRequests').show();
        }
    }

    if (!cancelOPCEvents) hasChanges(true, true);
}

function causeNonProductive() {
    cmbConceptBalanceClient.SetEnabled(false);
    cmbAbsenceConceptBalanceClient.SetEnabled(false);
    cmbAbsenceConceptBalanceDaysClient.SetEnabled(false);

    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_rButton').checked == true) {
        chgOPCItems('-1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optProductiveHoursOnCC,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_optExternalProductiveHours', 'undefined', false);
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday_rButton').checked == false &&
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_rButton').checked == false) {
            chgOPCItems('0', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause', 'undefined', false);
            $('#divAvailableAbsenceRequests').show();
            $('#divAvailableOverWorkRequests').hide();
            $('#divAvailableHolidayRequests').hide();
        }
    } else {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday_rButton').checked == true ||
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_rButton').checked == true) {
            chgOPCItems('1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive,ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive', 'undefined', false);
            $('#divAvailableAbsenceRequests').hide();
            $('#divAvailableOverWorkRequests').hide();
            $('#divAvailableHolidayRequests').show();
        }
    }

    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveCause_rButton').checked == true) {
        cmbAbsenceConceptBalanceClient.SetEnabled(true);
        cmbAbsenceConceptBalanceDaysClient.SetEnabled(true);
        cmbConceptBalanceProductiveClient.SetEnabled(false);
        cmbConceptBalanceProductiveDaysClient.SetEnabled(false);
    }

    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday_rButton').checked == true) {
        cmbConceptBalanceClient.SetEnabled(true);
        cmbConceptBalanceProductiveClient.SetEnabled(false);
        cmbConceptBalanceProductiveDaysClient.SetEnabled(false);
        changeStateVisibilityTab(false, false, true, false);
    } else {
        changeStateVisibilityTab(true, false, true, false);
    }

    if (!cancelOPCEvents) hasChanges(true, true);
}

function changeStateVisibilityTab(bEnable, checkVisibility, checkPunches, checkTabVisibility) {
    if (checkVisibility) {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityAll_panOptionPanel').setAttribute('venabled', 'True');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityNobody_panOptionPanel').setAttribute('venabled', 'True');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria_panOptionPanel').setAttribute('venabled', 'True');

        if (!bEnable) {
            chgOPCItems('1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityNobody,ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria', 'undefined', false);

            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityAll_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityNobody_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
        }

        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityAll');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityNobody');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria');
        linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityNobody,ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria');
    }

    if (checkPunches) {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesAll_panOptionPanel').setAttribute('venabled', 'True');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesNobody_panOptionPanel').setAttribute('venabled', 'True');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria_panOptionPanel').setAttribute('venabled', 'True');

        if (!bEnable) {
            chgOPCItems('1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesNobody,ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria', 'undefined', false);

            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesAll_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesNobody_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
        }

        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesAll');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesNobody');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria');
        linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesAll,ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesNobody,ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria');

        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAbsenceMandatoryDays_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCauseAbsences_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCauseCloseAbsences_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCauseAutomaticValidation_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');

        if (!bEnable) {
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAbsenceMandatoryDays_chkButton').checked = false;
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCauseAbsences_chkButton').checked = false;
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCauseCloseAbsences_chkButton').checked = false;
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCauseAutomaticValidation_chkButton').checked = false;
        }

        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optAbsenceMandatoryDays');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCauseAbsences');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCauseCloseAbsences');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_optCauseAutomaticValidation');

        txtInputCodeClient.SetEnabled(bEnable);
        txtMaxsDaysAbsenceClient.SetEnabled(bEnable);
        txtCauseAutomaticValidationClient.SetEnabled(bEnable);
    }

    if (checkTabVisibility) {
        if (bEnable) {
            document.getElementById('TABBUTTON_04').style.display = "";
        } else {
            document.getElementById('TABBUTTON_04').style.display = "none";
        }
    }

    if (checkVisibility) {
        if (!bEnable) {
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptAllRequestTypes_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptLeaveRequest_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptCustomRequestList_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptNoneRequestList_panOptionPanel').setAttribute('venabled', bEnable == true ? 'True' : 'False');
        } else {
            if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdHasLeaveDocuments').value == '1') { //has leave documents
                chgOPCItems('1', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptAllRequestTypes,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptLeaveRequest,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptCustomRequestList', 'undefined', false);

                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptAllRequestTypes_panOptionPanel').setAttribute('venabled', 'False');
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptLeaveRequest_panOptionPanel').setAttribute('venabled', 'True');
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptCustomRequestList_panOptionPanel').setAttribute('venabled', 'False');
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptNoneRequestList_panOptionPanel').setAttribute('venabled', 'False');
            } else {
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptAllRequestTypes_panOptionPanel').setAttribute('venabled', 'True');
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptLeaveRequest_panOptionPanel').setAttribute('venabled', 'False');
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptCustomRequestList_panOptionPanel').setAttribute('venabled', 'True');
                document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptNoneRequestList_panOptionPanel').setAttribute('venabled', 'True');
            }
        }
    }

    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptAllRequestTypes');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptLeaveRequest');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptCustomRequestList');
    venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptNoneRequestList');
    linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptAllRequestTypes,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptLeaveRequest,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptCustomRequestList,ctl00_contentMainBody_ASPxCallbackPanelContenido_roOptNoneRequestList');

    if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursProductive_rButton').checked == true) {
        $('#divAvailableAbsenceRequests').hide();
        $('#divAvailableOverWorkRequests').show();
        $('#divAvailableHolidayRequests').hide();
    } else {
        if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optHoursNonProductive_optHoursNonProductiveHoliday_rButton').checked == true) {
            $('#divAvailableAbsenceRequests').hide();
            $('#divAvailableOverWorkRequests').hide();
            $('#divAvailableHolidayRequests').show();
        } else {
            $('#divAvailableAbsenceRequests').show();
            $('#divAvailableOverWorkRequests').hide();
            $('#divAvailableHolidayRequests').hide();
        }
    }
}