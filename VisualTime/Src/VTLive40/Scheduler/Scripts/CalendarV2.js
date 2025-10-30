var actualTab = 0;
var roScheduleCalendar = null;
var PerformActionCallbackClient = null;

function PageBase_Load() {
    ConvertControls();
    resizeFrames();

    eraseCookie('TreeState_ctl00_contentMainBody_PopupSelectorEmployees_ASPxPanel3_objContainerTreeV3_roTrees1EmpCalendar');

    let textodates = FormateaFecha(moment().startOf('week').toDate()) + "," + FormateaFecha(moment().add(1, 'week').endOf('week').toDate());
    localStorage.setItem('PlanView', "2");

    let owDates = localStorage.getItem('OverwriteCalendarDates');

    if (typeof owDates != 'undefined' && owDates != null && JSON.parse(owDates)) {
        localStorage.setItem('OverwriteCalendarDates', false);
    }
    else {
        localStorage.setItem('SchedulerIntervalDates', textodates);
    }

    actualTab = parseInt(localStorage.getItem('CalendarType') || "0", 10);

    if (actualTab == 0) {
        $('#ctl00_contentMainBody_divShiftsPalette').hide();
    } else {
        $('#ctl00_contentMainBody_divShiftsPalette').show();
    }

    setTimeout(function () { initializeCalendar(actualTab); }, 100);
}

function FormateaFecha(d) {
    var strFecha = "";
    strFecha = d.getFullYear() + "#" + (d.getMonth() + 1) + "#" + d.getDate();
    return strFecha;
}

window.onresize = function () {
    resizeFrames();
    $(".ui-button-text").addClass("btnFlat btnFlatBlack");
}

function resizeFrames() {
    var divMainBodyHeight = $("#divMainBody").outerHeight(true);
    var divHeight = 0;
    if (divMainBodyHeight < 525) {
        divHeight = 525 - $("#divTabInfo").outerHeight(true);
    }
    else {
        divHeight = divMainBodyHeight - $("#divTabInfo").outerHeight();
    }

    divHeight = divHeight - 35;

    $("#divTabData").height(divHeight);
    $("#divContenido").height(divHeight);

    var divContenidoHeight = $("#divContenido").height() - $("#barraSuperior").outerHeight(true) - 10;
    $("#divCalContenido").height(divContenidoHeight);
}

function initializeCalendar(tab) {
    actualTab = tab;
    finallyChangeTab(tab);
    reloadBarButtons();
}

function reloadBarButtons() {
    if (roScheduleCalendar != null) roScheduleCalendar.loadingFunctionExtended(true);

    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "RELOADBARBUTTONS";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelBarButtonsClient.PerformCallback(strParameters);

    cargaCalendarV2Wizards(actualTab);
}
var responseObj = null;
function cargaCalendarV2Wizards(actualTab) {
    var Url = "";

    Url = "Handlers/srvScheduler.ashx?action=getBarButtons&ID=" + actualTab;
    AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId)");
}

function parseResponseBarButtons(oResponse) {
    window.parent.setUPReportsAndWizards(oResponse);
}

function changeCalendarTabs(numTab) {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'finallyChangeTab(' + numTab + ')';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) finallyChangeTab(numTab);
}

async function finallyChangeTab(numTab) {
    arrButtons = new Array('ctl00_contentMainBody_TABBUTTON_InputOutputs', 'ctl00_contentMainBody_TABBUTTON_Calendar');

    var maxVisiblTab = 0;

    for (n = 0; n < 2; n++) {
        var tab = document.getElementById(arrButtons[n]);
        if (tab != null) {
            maxVisiblTab = n;
            if (n == numTab) {
                tab.className = 'bTab-active';
            }
            else {
                tab.className = 'bTab';
            }
        }
    }
    if (maxVisiblTab < numTab) numTab = maxVisiblTab;

    if (numTab == 0) {
        $('#ctl00_contentMainBody_divShiftsPalette').hide();
    } else {
        $('#ctl00_contentMainBody_divShiftsPalette').show();
    }

    if (parseInt(numTab, 10) == 0) {
        document.getElementById('hdnReportsType').value = 'Reads';
        document.getElementById('hdnReportsTitleControlID').value = hdnCalendarConfigClient.Get('hdnReportsTitleReads');
    } else {
        document.getElementById('hdnReportsType').value = 'Shifts';
        document.getElementById('hdnReportsTitleControlID').value = hdnCalendarConfigClient.Get('hdnReportsTitleShifts');
    }

    localStorage.setItem('CalendarType', numTab);
    actualTab = numTab;

    hasChanges(false);

    await reloadSelectionFromServer();
}

async function reloadSelectionFromServer() {

    await loadSelectorViewFromServer('/Cookie', 'calendarController', 'empMVCSelector');

    const cat = localStorage.getItem('empMVCSelector');
    if (cat != null) {
        let tmpOptions = JSON.parse(cat);

        hdnCalendarConfigClient.Set('EmpSelected', tmpOptions.view.ComposeFilter);
        hdnCalendarConfigClient.Set('EmpFilters', tmpOptions.view.Filter);
        hdnCalendarConfigClient.Set('EmpUserFields', tmpOptions.view.UserFields);
    }

    reloadBarButtons();
    reloadCalendar();
}

function ASPxCallbackPanelBarButtonsClient_EndCallBack(s, e) {
    //showLoadingGrid(false);
    switch (s.cpAction) {
        case "RELOADBARBUTTONS":
            if (s.cpResult != "OK") {
                showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
            } else {
                loadShifts();
            }
            break;
    }
}

function CallbackCalendar_CallbackError(s, e) {
    if (roScheduleCalendar != null) {
        clearTimeout(roScheduleCalendar.refreshTimmer);
        roScheduleCalendar.loadingFunctionExtended(false);
    }
}

function showLoadingGrid(loading) {
    try {
        parent.showLoader(loading);
    } catch (e) { showError("showLoadingGrid", e); }
}

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showErrorPopup(Title, typeIcon, Description, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Scheduler/srvMsgBoxScheduler.aspx?action=Message";
        url = url + "&TitleKey=" + Title;

        if (Description != "") { url = url + "&DescriptionKey=" + Description; }
        else { url = url + "&DescriptionText=" + DescriptionText; }

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

function btnOpenPopupSelectorEmployeesClient_Click() {
    PopupSelectorEmployeesClient.Show();

    let initValue = undefined;
    window.frames['ifEmployeeSelector'].initUniversalSelector(initValue,true,'calendarController');
}

function closeAndApplySelector(currentSelection) {

    hdnCalendarConfigClient.Set('EmpSelected', currentSelection.ComposeFilter);
    hdnCalendarConfigClient.Set('EmpFilters', currentSelection.Filter);
    hdnCalendarConfigClient.Set('EmpUserFields', currentSelection.UserFields);

    reloadCalendar();

    PopupSelectorEmployeesClient.Hide();
}

function OnInitGroupSelector(s, e) {
    ASPxClientUtils.AttachEventToElement(window.document, "keydown", function (evt) {
        if (evt.keyCode == ASPxClientUtils.StringToShortcutCode("ESCAPE"))
            closeSelectorEmployeePopup();
    });
}

function closeSelectorEmployeePopup() {
    PopupSelectorEmployeesClient.Hide();
}

function reloadCalendar() {
    var actualView = Robotics.Client.Constants.TypeView.Review;
    if (parseInt(actualTab, 10) == 0) {
        actualView = Robotics.Client.Constants.TypeView.Review;
    } else if (parseInt(actualTab, 10) == 1) {
        actualView = Robotics.Client.Constants.TypeView.Planification;
    }

    if (roScheduleCalendar != null) {
        var calFilters = (localStorage.getItem('CalendarLoadRecursive') || "false@@0@false@false").split('@');

        roScheduleCalendar.loadData(null, null, getEmployeeFilter(), calFilters[0] == 'false' ? false : true, actualView, calFilters[1], typeof calFilters[3] == 'undefined' ? false : (calFilters[3] == 'false' ? false : true), typeof calFilters[4] == 'undefined' ? false : (calFilters[4] == 'false' ? false : true));
    } else {
        roScheduleCalendar = new Robotics.Client.Controls.roCalendar('ctl00_contentMainBody_oCalendar', actualView, showLoadingGrid, showErrorPopup, 'CalendarLoadRecursive');
        PerformActionCallbackClient = eval('ctl00_contentMainBody_oCalendar_PerformActionCallbackClient');
        roScheduleCalendar.create();

        LoadDateSelector(hdnCalendarConfigClient.Get('Language'));
    }
}

function loadShifts(s, e) {
    ASPxSelectorShiftsCallbackPanelContenidoClient.PerformCallback(cmbShiftGroupsClient.GetSelectedItem().value);
}

function getEmployeeFilter() {
    return hdnCalendarConfigClient.Get('EmpSelected') + '¬' + hdnCalendarConfigClient.Get('EmpFilters') + '¬' + hdnCalendarConfigClient.Get('EmpUserFields');
}

function CallbackCalendar_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        var objResultParams = null;
        if (typeof (s.cpObjResultParams) != 'undefined') objResultParams = s.cpObjResultParams;
        
        if (typeof (s.cpObjResultConfigShiftType) != 'undefined') $("#ctl00_contentMainBody_oCalendar_hdnHolidayShiftPeriodicity").val(s.cpObjResultConfigShiftType);

        roScheduleCalendar.endCallback(s.cpAction, JSON.parse(s.cpObjResult, roDateReviver), JSON.parse(objResultParams, roDateReviver));

        txtEmployeesClient.SetText(roScheduleCalendar.getEmployeeCountResume());
    } else {
        if (roScheduleCalendar != null) {
            roScheduleCalendar.loadingFunctionExtended(false);

            if (!roScheduleCalendar.parseErrorMessage(s.cpAction, JSON.parse(s.cpObjResult, roDateReviver))) showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
        } else {
            showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
        }
    }
}

function complementaryDefinitionCallback_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        switch (s.cpAction) {
            case "SHIFTLAYERDEFINITION":
                roScheduleCalendar.complementaryManager.finallyPrepareDialogElements(JSON.parse(s.cpObjResult, roDateReviver));
                break;
        }
    }
}

function assignmentDefinitionCallback_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        switch (s.cpAction) {
            case "SHIFTLAYERDEFINITION":
                roScheduleCalendar.assignmentsManager.finallyPrepareDialogElements(JSON.parse(s.cpObjResult, roDateReviver));
                break;
        }
    }
}

function PerformActionCallback_CallbackComplete(s, e) {
    if (s.cpAction == "VALIDATE") {
        if (s.cpResult == true) {
            showCaptcha();
        } else {
            showErrorPopup("Error.DatesPeriod", "ERROR", "Error.DatesPeriodDesc", "", "Error.OK", "", "");
        }
    } else if (s.cpAction == "PERFORM_ACTION") {
        monitor = setInterval(function () {
            var oParameters = {};
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = 'CHECKPROGRESS';

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            PerformActionCallbackClient.PerformCallback(strParameters);
        }, 5000);
    } else if (s.cpAction == "ERROR") {
        clearInterval(monitor);
        AspxLoadingPopup_Client.Hide();
        showErrorPopup("Error.CopyError", "ERROR", "Error." + s.cpActionMsg, "", "Error.OK", "", "");
    } else if (s.cpAction == "CHECKPROGRESS") {
        if (s.cpActionResult == "OK") {
            clearInterval(monitor);
            AspxLoadingPopup_Client.Hide();
            showErrorPopup("OK.CopyOK", "INFO", "OK." + s.cpActionMsg, "", "Error.OK", "", "RefreshScreen('fullRefresh');");
        }
    }
}

function ASPxSelectorShiftsCallbackPanelContenido_EndCallback(s, e) {
    //$(".shiftSelector").draggable({ revert: "invalid", containment: '#divMainBody', cursor: "move", helper: 'clone', zIndex: 100000 });

    function handleDragStart(e) {
        this.style.opacity = '0.4';
    }

    function handleDragEnd(e) {
        this.style.opacity = '1';
        roScheduleCalendar.onDrop(e);
        e.stopPropagation(); // stops the browser from redirecting.
        return false;
    }

    let items = document.querySelectorAll("#ctl00_contentMainBody_ASPxSelectorShiftsCallbackPanelContenido_divShiftsServer .shiftSelector");
    items.forEach(function (item) {
        item.addEventListener('dragstart', handleDragStart, false);
        item.addEventListener('dragend', handleDragEnd, false);
    });
}

function hasChanges(bolChanges) {
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
    } else {
        divTop.style.display = 'none';
    }
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        msgTop.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function saveChanges() {
    try {
        if (roScheduleCalendar != null) {
            roScheduleCalendar.saveChanges();
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChanges() {
    try {
        if (roScheduleCalendar != null) {
            roScheduleCalendar.refresh();
        }
    } catch (e) { showError("undoChanges", e); }
}

function CheckCalendar() {
    var bolChange = false;

    if (roScheduleCalendar != null) {
        bolChange = roScheduleCalendar.hasChanges && roScheduleCalendar.loadIndictments;
    }

    if (bolChange) {
        roScheduleCalendar.clientMode.checkCalendarIndictments();
    } else {
        DevExpress.ui.dialog.alert(Globalize.formatMessage('roNotChangesToValidate'), Globalize.formatMessage('roAlert'));
    }
}

function ShowIncompletedDays(_IDGroup, _IDEmployee) {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'popupActionFunc("Scheduler/MovesNew.aspx?action=incompletedDays",1400, 620, true, true)';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) popupActionFunc("Scheduler/MovesNew.aspx?action=incompletedDays", 1400, 620, true, true);
}

function ShowNotJustifiedDays(_IDGroup, _IDEmployee) {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'popupActionFunc("Scheduler/MovesNew.aspx?action=notjustifiedDays",1400, 620, true, true)';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) popupActionFunc("Scheduler/MovesNew.aspx?action=notjustifiedDays", 1400, 620, true, true);
}

function ShowNotReliabledDays(_IDGroup, _IDEmployee) {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'popupActionFunc("Scheduler/MovesNew.aspx?action=notreliabledDays",1400, 620, true, true)';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) popupActionFunc("Scheduler/MovesNew.aspx?action=notreliabledDays", 1400, 620, true, true);
}

function ShowAssignCausesWizard() {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'popupActionFunc("Scheduler/Wizards/AssignCausesWizard.aspx",500,485,false, true)';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) popupActionFunc("Scheduler/Wizards/AssignCausesWizard.aspx", 500, 485, false, true);
}

function ShowInsertMassPunchWizard() {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'popupActionFunc("Scheduler/Wizards/EmployeeMassPunchWizard.aspx",500,485, false, true)';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) popupActionFunc("Scheduler/Wizards/EmployeeMassPunchWizard.aspx", 500, 485, false, true);
}

function ShowIncidencesWizard() {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'popupActionFunc("Scheduler/Wizards/IncidencesWizard.aspx",1100, 555, false, true)';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) popupActionFunc("Scheduler/Wizards/IncidencesWizard.aspx", 1100, 555, false, true);
}

function ShowAssignCentersWizard() {
    var bolChange = true;
    var DateSelectedInf = moment(roScheduleCalendar.firstDate).format('YYYY-MM-DD');
    var DateSelectedSup = moment(roScheduleCalendar.endDate).format('YYYY-MM-DD');

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'popupActionFunc("Scheduler/Wizards/AssignCentersWizard.aspx?DateStart="' + encodeURIComponent(DateSelectedInf) + '"&DateEnd="' + encodeURIComponent(DateSelectedSup) + '",1100, 515,false, true)';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) popupActionFunc("Scheduler/Wizards/AssignCentersWizard.aspx?DateStart=" + encodeURIComponent(DateSelectedInf) + "&DateEnd=" + encodeURIComponent(DateSelectedSup), 1100, 515, false, true);
}

function ShowCopySchedulerWizard() {
    if (roScheduleCalendar.selectedEmployee != null) {
        var bolChange = true;

        if (roScheduleCalendar != null) {
            bolChange = !roScheduleCalendar.hasChanges;

            var onAcceptFunc = 'popupActionFunc("Scheduler/Wizards/CopyScheduleWizard.aspx?EmployeeID=' + roScheduleCalendar.selectedEmployee.EmployeeData.IDEmployee + '",600, 500, false, true)';
            if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
        }

        if (bolChange) popupActionFunc("Scheduler/Wizards/CopyScheduleWizard.aspx?EmployeeID=" + roScheduleCalendar.selectedEmployee.EmployeeData.IDEmployee, 600, 500, false, true);
    } else {
        showErrorPopup("Error.CheckUserDayTitle", "ERROR", "Error.CheckUserDayDesc", "", "Error.OK", "Error.OKDesc", "");
    }
}

function ShowEmployeeMultiAbsences() {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'popupActionFunc("Employees/Wizards/MultiAbsencesWizard.aspx",800, 610, false, true)';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) popupActionFunc("Employees/Wizards/MultiAbsencesWizard.aspx", 800, 610, false, false);
}

function ShowTemplateAssignWizard() {
    var bolChange = true;

    var idgroup = "";
    if (roScheduleCalendar.selectedEmployee != null) idgroup = "Scheduler/Wizards/TemplateAssignWizardV2.aspx?GroupID=" + roScheduleCalendar.selectedEmployee.EmployeeData.IDGroup;
    else idgroup = "Scheduler/Wizards/TemplateAssignWizardV2.aspx"

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'popupActionFunc("' + idgroup + '",665, 620, false, true)';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) popupActionFunc(idgroup, 665, 600, false, true);
}

function ShowDailyCoveragePlanned(idGroup, coverageDate) {
    try {
        var bolChange = true;
        var actionURL = "Scheduler/DailyCoveragePlanned.aspx?IDGroup=" + idGroup + "&CoverageDate=" + coverageDate;
        if (roScheduleCalendar != null) {
            bolChange = !roScheduleCalendar.hasChanges;

            var onAcceptFunc = 'popupActionFunc("' + actionURL + '", 1000, 480, false,false)';
            if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
        }

        if (bolChange) popupActionFunc(actionURL, 1000, 480, false, false);
    } catch (e) { showError("SchedulerCoverages::ShowDailyCoveragePlanned", e); }
}

function ShowDailyCoverage(idGroup, coverageDate) {
    try {
        var bolChange = true;
        var actionURL = "Scheduler/DailyCoverage.aspx?IDGroup=" + idGroup + "&CoverageDate=" + coverageDate;
        if (roScheduleCalendar != null) {
            bolChange = !roScheduleCalendar.hasChanges;

            var onAcceptFunc = 'popupActionFunc("' + actionURL + '",500, 580, false, false)';
            if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
        }

        if (bolChange) popupActionFunc(actionURL, 500, 580, false, false);
    } catch (e) { showError("SchedulerCoverages::ShowDailyCoverage", e); }
}

function popupActionFunc(action, width, height, addUserParams, requiereEmployee) {
    if (roScheduleCalendar.getSelectedIDEmployees().length > 0 || !requiereEmployee) {
        var url = action;
        if (addUserParams) {
            url += getDateParamsV2();
            url += setEmployeeFilters();
        }
        var Title = '';

        if (url.length <= 2048) parent.ShowExternalForm2(url, width, height, Title, '', false, false, false);
        else showErrorPopup("Error.LengthExceed", "ERROR", "Error.QueryLenghtExceed", "", "Error.OK", "Error.OKDesc", "");
    } else {
        showErrorPopup("Error.OptionNotAvailable", "ERROR", "Error.OptionNotAvailableDesc", "", "Error.OK", "Error.OKDesc", "");
    }
}

function ShowReports(Title, ReportTitle, ReportsType, DefaultReportsVersion, RootURL) {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'ShowReportsFinally("' + Title + '","' + ReportTitle + '","' + ReportsType + '"' + DefaultReportsVersion + ',"' + RootURL + '")';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) ShowReportsFinally(Title, ReportTitle, ReportsType, DefaultReportsVersion, RootURL);
}

function ShowReportsFinally(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        var TitleControlID = document.getElementById('hdnReportsTitleControlID').value;
        Title = Title + ' - ' + document.getElementById(TitleControlID).value;
        ReportsType = ReportsType + document.getElementById('hdnReportsType').value;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function ExportPlanToExcel() {
    if (roScheduleCalendar != null) {
        roScheduleCalendar.exportToExcel();
    }
}

function ImportPlanFromExcel() {
    var bolChange = true;

    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;

        var onAcceptFunc = 'ShowImportPlanFromExcel()';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }

    if (bolChange) ShowImportPlanFromExcel();
}

function ShowImportPlanFromExcel() {
    if (roScheduleCalendar != null) {
        roScheduleCalendar.importFromExcel();
    }
}

function ShowRemarksConfig() {
    var Title = ''; //$get('<%= lblWeekScheduleWizardFormTitle.ClientID %>').innerHTML;
    parent.ShowExternalForm2('Scheduler/RemarksConfig.aspx', 500, 500, Title, '', false, false, false);
}

function ShowCalendarRemarksConfig() {
    var bolChange = true;
    if (roScheduleCalendar != null) {
        bolChange = !roScheduleCalendar.hasChanges;
        var onAcceptFunc = 'ShowCalenarRemarksConfigFinally()';
        if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
    }
    if (bolChange) ShowCalenarRemarksConfigFinally();
}

function ShowCalenarRemarksConfigFinally() {
    var Title = ''; //$get('<%= lblWeekScheduleWizardFormTitle.ClientID %>').innerHTML;
    parent.ShowExternalForm2('Scheduler/CalendarRemarksConfig.aspx', 700, 650, Title, '', false, false, false);
}

function getDateParamsV2() {
    var rangeDatesV2 = localStorage.getItem('SchedulerIntervalDates') || "0";

    var arrayDatesV2 = rangeDatesV2.split(',');
    var dateIni = null;
    var dateEnd = null;
    if (rangeDatesV2 != "0") {
        arrayDatesV2 = rangeDatesV2.split(',');
        arrayDatesV2.forEach(function (item, index) {
            if (item.indexOf('#') > -1) {
                if (dateIni == null)
                    dateIni = moment(convertDateIn(item), localFormat);
                else
                    dateEnd = moment(convertDateIn(item), localFormat);
            }
        });

        if (arrayDatesV2.length <= 1) {
            dateEnd = dateIni;
        }
    } else {
        dateIni = moment();
        dateEnd = moment();
    }

    var urlDateParamV2 = '&DateStart=' + encodeURIComponent(dateIni.format('DD/MM/YYYY')) + '&DateEnd=' + encodeURIComponent(dateEnd.format('DD/MM/YYYY'));
    return urlDateParamV2;
}

function setEmployeeFilters() {
    var filterSelected = roScheduleCalendar.getSelectedIDEmployees();
    return '&EmpFilter=' + encodeURIComponent(filterSelected) + '&CalendarV2=1';
}

var monitor = -1;

function showCaptcha() {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=COPYSPECIAL";
    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
}

function captchaCallback(action) {
    switch (action) {
        case "COPYSPECIAL":
            AspxLoadingPopup_Client.Show();
            roScheduleCalendar.closeAdvancedCopyDialog();
            PerformAction();
            break;
        case "ERROR":
            showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "", "Error.OK", "", "");
            break;
    }
}

function PerformValidation() {
    var oParameters = {};
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = 'VALIDATE';

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    PerformActionCallbackClient.PerformCallback(strParameters);
}

function PerformAction() {
    var oParameters = {};
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.idEmployee = roScheduleCalendar.getFirstObjectSelected().IDEmployee;
    oParameters.initialBeginDate = moment(roScheduleCalendar.getFirstSelectedDay().PlanDate).clone().toDate();
    oParameters.initialEndDate = moment(roScheduleCalendar.getFirstSelectedDay().PlanDate).clone().add((roScheduleCalendar.selectedMaxColumn - roScheduleCalendar.selectedMinColumn), 'days').toDate();
    oParameters.pasteStartDate = moment(roScheduleCalendar.selectedDay.PlanDate).clone().toDate();
    oParameters.destEmployee = roScheduleCalendar.selectedEmployee.EmployeeData.IDEmployee;
    oParameters.copyWorkingShifts = roScheduleCalendar.copyWorkingShifts ? '1' : '0';
    oParameters.copyHolidaysShifts = roScheduleCalendar.copyHolidaysShifts ? '1' : '0';
    oParameters.copyAssignmentsShifts = roScheduleCalendar.copyAssignmentsShifts ? '1' : '0';
    oParameters.filters = roScheduleCalendar.advCopyManager.getFiltersValue();

    oParameters.action = 'PERFORM_ACTION';

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    PerformActionCallbackClient.PerformCallback(strParameters);
}

function showDaySelected(selectedDay) {
    var selDate = moment(selectedDay, "DD/MM/YYYY");
    $('#divDayCalendar').data('dateRangePicker').setDateRange(selDate.format(localFormat), selDate.format(localFormat));
}

function RefreshScreen(DataType, hasChanges) {
    switch (DataType) {
        case 'movesNew':
            if (hasChanges == "1") refreshCalendar();
            //if (typeof roScheduleCalendar != 'undefined' && roScheduleCalendar != null) {
            //    var refreshDate = roScheduleCalendar.selectedDay.PlanDate;

            //    if (typeof selectedDate != 'undefined') {
            //        refreshDate = moment(selectedDay, "YYYYMMDD").toDate();
            //    }

            //    roScheduleCalendar.refreshDayWithParams(roScheduleCalendar.selectedEmployee.EmployeeData.IDEmployee, refreshDate);
            //}
            break;
        case '1':
        case 'DailyCoveragePlanned':
        case 'fullRefresh':
            refreshCalendar();
            break;
        case 'DailyCoverageTeoric':
            if (roScheduleCalendar != null) roScheduleCalendar.setUpSchedulingTimmer(1);
            break;
    }
    return;
}