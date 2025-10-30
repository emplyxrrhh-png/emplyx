var actualTab = 0;
var roBudget = null;
var roScheduleCalendar = null;
var PerformActionCallbackClient = null;
var orgChart = null;
var orgChartSelectedNode = null;

function getActualBudgetViewType(bMustRefresh) {
    var oldView = Robotics.Client.Constants.TypeView.Definition;
    var newView = Robotics.Client.Constants.TypeView.Definition;

    oldView = localStorage.getItem('BudgetView') || "1";

    switch (actualTab) {
        case 0:
            //roScheduleCalendar = null;
            newView = Robotics.Client.Constants.TypeView.Definition;
            break;
        case 1:
            //roScheduleCalendar = null;
            if (oldView >= 5 && oldView <= 6) newView = Robotics.Client.Constants.TypeView.Detail;
            else newView = Robotics.Client.Constants.TypeView.Planification;
            break;
        case 2:
            //roBudget = null;
            bMustRefresh = false;
            break;
    }

    if (bMustRefresh && roBudget != null) roBudget.setTypeView(newView);

    return newView;
}

function PageBase_Load() {
    ConvertControls();
    resizeFrames();

    actualTab = parseInt(localStorage.getItem('BudgetType') || "0", 10);
    setDateIntervalsVisible(actualTab, false);

    var Url = "../Scheduler/srvMsgBoxScheduler.aspx?action=keepAlive"
    AsyncCall("POST", Url, "CONTAINER", "emptyContainer", "");

    setTimeout(function () { initializeBudget(actualTab); }, 100);
}

function setDateIntervalsVisible(btab, bRefreshCalendar) {
    if (btab >= 1) {
        $('#ctl00_contentMainBody_divShiftsPalette').hide();

        if (btab == 1) {
            $('#divDayDetailCalendar').show();
            $('#divWeekDetailCalendar').show();
        } else {
            $('#divDayDetailCalendar').hide();
            $('#divWeekDetailCalendar').hide();
        }

        $('#divDayCalendar').hide();
        $('#divWeekCalendar').show();

        $('#divTwoCalendar').show();
        $('#divMonthCalendar').show();
        $('#divFreeCalendar').show();
    } else {
        $('#ctl00_contentMainBody_divShiftsPalette').show();

        $('#divDayDetailCalendar').hide();
        $('#divWeekDetailCalendar').hide();

        $('#divDayCalendar').hide();
        $('#divWeekCalendar').show();

        $('#divTwoCalendar').show();
        $('#divMonthCalendar').show();
        $('#divFreeCalendar').show();
    }

    var bNeedToReload = true;

    planView = localStorage.getItem('BudgetView') || "1";

    if (btab == 0 && (parseInt(planView, 10) == 5 || parseInt(planView, 10) == 6)) {
        localStorage.setItem('BudgetView', "2")

        var startDate = moment().startOf('week');
        var endDate = moment().endOf('week').add(1, 'week');
        convertDateOut(startDate, endDate);

        if (bRefreshCalendar) {
            manageText(startDate.format(localFormat) + localseparator + endDate.format(localFormat), 2, $('#divTwoCalendar'));
            bNeedToReload = false;
        }
    }

    return bNeedToReload;
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

    divHeight = divHeight - 5;

    $("#divTabData").height(divHeight);
    $("#divContenido").height(divHeight);

    var divContenidoHeight = $("#divContenido").height() - $("#barraSuperior").outerHeight(true) - 10;
    $("#divCalContenido").height(divContenidoHeight);
}

function initializeBudget(tab) {
    actualTab = tab;
    finallyChangeTab(tab);
    reloadBarButtons();
}

function reloadBarButtons() {
    if (roBudget != null) roBudget.loadingFunctionExtended(true);

    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "RELOADBARBUTTONS";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelBarButtonsClient.PerformCallback(strParameters);
}

function changeBudgetTabs(numTab) {
    var bolChange = true;

    if (actualTab == 2) {
        if (roScheduleCalendar != null) {
            bolChange = !roScheduleCalendar.hasChanges;

            var onAcceptFunc = 'finallyChangeTab(' + numTab + ')';
            if (!bolChange) roScheduleCalendar.showChangesWarning(onAcceptFunc);
        }
    } else {
        if (roBudget != null) {
            bolChange = !roBudget.hasChanges;

            var onAcceptFunc = 'finallyChangeTab(' + numTab + ')';
            if (!bolChange) roBudget.showChangesWarning(onAcceptFunc);
        }
    }

    if (bolChange) finallyChangeTab(numTab);
}

function finallyChangeTab(numTab) {
    arrButtons = new Array('ctl00_contentMainBody_TABBUTTON_Definition', 'ctl00_contentMainBody_TABBUTTON_Calendar', 'ctl00_contentMainBody_TABBUTTON_OrgCalendar');

    var maxVisiblTab = 0;

    for (n = 0; n < 3; n++) {
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

    if (parseInt(numTab, 10) == 0) {
        document.getElementById('hdnReportsType').value = 'Reads';
        document.getElementById('hdnReportsTitleControlID').value = hdnCalendarConfigClient.Get('hdnReportsTitleReads');
    } else {
        document.getElementById('hdnReportsType').value = 'Shifts';
        document.getElementById('hdnReportsTitleControlID').value = hdnCalendarConfigClient.Get('hdnReportsTitleShifts');
    }

    localStorage.setItem('BudgetType', numTab);
    actualTab = numTab;

    hasChanges(false);

    reloadBarButtons();

    if (actualTab == 2) {
        $('#ctl00_contentMainBody_oOrgCalendar_roCalendarRender').show();
        $('#ctl00_contentMainBody_oCalendar_roCalendarRender').hide();
        reloadCalendar();

        $('#ctl00_contentMainBody_divShiftsPalette').hide();
        $('#divDayDetailCalendar').hide();
        $('#divWeekDetailCalendar').hide();
        $('#divDayCalendar').hide();
        $('#divWeekCalendar').show();
        $('#divTwoCalendar').show();
        $('#divMonthCalendar').show();
        $('#divFreeCalendar').show();
    } else {
        $('#ctl00_contentMainBody_oOrgCalendar_roCalendarRender').hide();
        $('#ctl00_contentMainBody_oCalendar_roCalendarRender').show();
        var bNeedToReload = setDateIntervalsVisible(numTab, true);
        if (bNeedToReload) reloadBudget();
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
        if (actualTab == 2) {
            if (roScheduleCalendar != null) {
                roScheduleCalendar.saveChanges();
            }
        } else {
            if (roBudget != null) {
                roBudget.saveChanges();
            }
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChanges() {
    try {
        if (actualTab == 2) {
            if (roScheduleCalendar != null) {
                roScheduleCalendar.refresh();
            }
        } else {
            if (roBudget != null) {
                roBudget.refresh();
            }
        }
    } catch (e) { showError("undoChanges", e); }
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

function ShowReports(Title, ReportTitle, ReportsType, DefaultReportsVersion, RootURL) {
    var bolChange = true;

    if (roBudget != null) {
        bolChange = !roBudget.hasChanges;

        var onAcceptFunc = 'ShowReportsFinally("' + Title + '","' + ReportTitle + '","' + ReportsType + '"' + DefaultReportsVersion + ',"' + RootURL + '")';
        if (!bolChange) roBudget.showChangesWarning(onAcceptFunc);
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

function OnInitGroupSelector(s, e) {
    //ASPxClientUtils.AttachEventToElement(window.document, "keydown", function (evt) {
    //    if (evt.keyCode == ASPxClientUtils.StringToShortcutCode("ESCAPE"))
    //        closeSelectorEmployeePopup(false);
    //});
}

function closeSelectorEmployeePopup(bCommitChanges) {
    if (bCommitChanges) {
        var selNode = orgChart.getSelectedNode();

        eraseCookie("Budget_OrgChartFilter");
        createCookie("Budget_OrgChartFilter", selNode.Id);
        txtOrgChartSelectionClient.SetText(selNode.Name);

        refreshBudget();
    }
    orgChartPopup.hide();
}

function reloadBudget() {
    var actualView = getActualBudgetViewType(true);

    if (roBudget != null) {
        var pUnitsFilter = (localStorage.getItem('BudgetStateOptions') || "false@@0@false").split('@');

        var calDates = localStorage.getItem('BudgetIntervalDates') || "0";
        var startCalDate = null;
        var endCalDate = null;

        if (calDates != "0") {
            var arrayDates = calDates.split(',');
            arrayDates.forEach(function (item, index) {
                if (item.indexOf('#') > -1) {
                    if (startCalDate == null) startCalDate = moment(convertDateIn(item), localFormat).toDate();
                    else endCalDate = moment(convertDateIn(item), localFormat).toDate();
                }
            });

            if (arrayDates.length <= 1) {
                endCalDate = startCalDate;
            }
            convertDateOut(startCalDate, endCalDate, false);
        }
        roBudget.loadData(startCalDate, endCalDate, getOrgChartFilter(), actualView, pUnitsFilter[1], eval(pUnitsFilter[0]));

        roBudget.loadData(null, null, getOrgChartFilter(), actualView, pUnitsFilter[1], eval(pUnitsFilter[0]));
    } else {
        var actualView = getActualBudgetViewType(false);

        roBudget = new Robotics.Client.Controls.roCalendar('ctl00_contentMainBody_oCalendar', actualView, showLoadingGrid, showErrorPopup, 'BudgetStateOptions');
        PerformActionCallbackClient = eval('ctl00_contentMainBody_oCalendar_PerformActionCallbackClient');
        roBudget.clientMode.OnSelectedCell = roBudget_SelectedCell;
        roBudget.clientMode.OnDayClick = setUpAndOpenDetailDialog;
        roBudget.create();

        LoadDateSelector(hdnCalendarConfigClient.Get('Language'));
    }
}

function reloadCalendar() {
    var actualView = Robotics.Client.Constants.TypeView.Planification;

    if (roScheduleCalendar != null) {
        var calFilters = localStorage.getItem('CalendarLoadRecursive') || "false@@0@false@false";

        var budgetDates = localStorage.getItem('SchedulerIntervalDates') || "0";
        if (budgetDates != "0") {
            var arrayDates = budgetDates.split(',');
            arrayDates.forEach(function (item, index) {
                if (item.indexOf('#') > -1) {
                    if (startBudgetDate == null) startBudgetDate = moment(convertDateIn(item), localFormat).toDate();
                    else endBudgetDate = moment(convertDateIn(item), localFormat).toDate();
                }
            });

            if (arrayDates.length <= 1) {
                endBudgetDate = startBudgetDate;
            }
            convertDateOut(startBudgetDate, endBudgetDate, true);
        }
        roScheduleCalendar.loadData(startBudgetDate, endBudgetDate, "D" + getOrgChartFilter(), calFilters[0] == 'false' ? false : true, actualView, calFilters[1], typeof calFilters[3] == 'undefined' ? false : (calFilters[3] == 'false' ? false : true), typeof calFilters[4] == 'undefined' ? false : (calFilters[4] == 'false' ? false : true));

        //roScheduleCalendar.loadData(null, null, "D" + getOrgChartFilter(), calFilters[0] == 'false' ? false : true, actualView, calFilters[1], typeof calFilters[3] == 'undefined' ? false : (calFilters[3] == 'false' ? false : true), typeof calFilters[4] == 'undefined' ? false : (calFilters[4] == 'false' ? false : true));
    } else {
        roScheduleCalendar = new Robotics.Client.Controls.roCalendar('ctl00_contentMainBody_oOrgCalendar', actualView, showLoadingGrid, showErrorPopup, 'CalendarLoadRecursive');
        PerformActionCallbackClient = eval('ctl00_contentMainBody_oOrgCalendar_PerformActionCallbackClient');
        roScheduleCalendar.create();

        LoadDateSelector(hdnCalendarConfigClient.Get('Language'));
    }
}

var orgChartPopup = null;

function btnOpenOrgChartSelector(s, e) {
    orgChartPopup = $('#orgChartDiv').dxPopup({
        fullScreen: false,
        width: 1100,
        height: 500,
        showTitle: true,
        title: Globalize.formatMessage("roSelectorOrgChart"),
        visible: false,
        dragEnabled: true,
        hideOnOutsideClick: false
    }).dxPopup("instance");

    orgChartPopup.show();

    $('#btnAcceptOrgChart').dxButton({
        text: Globalize.formatMessage("Done"),
        onClick: function () {
            closeSelectorEmployeePopup(true);
        }
    });

    $('#btnCancelOrgChart').dxButton({
        text: Globalize.formatMessage("Cancel"),
        onClick: function () {
            orgChartPopup.hide();
        }
    });

    if (orgChart == null) {
        //orgChart = new Robotics.Client.Controls.roOrgChart('ctl00_contentMainBody_orgChart_');
        //orgChart.loadData(readCookie("Budget_OrgChartFilter", "0"));
    }
}

function getOrgChartFilter() {
    var orgChartFilter = readCookie("Budget_OrgChartFilter", "0")

    return orgChartFilter;
}

function loadProductiveUnits(s, e) {
    ASPxProductiveUnitSelectorClient.PerformCallback(cmbProductiveUnitsClient.GetSelectedItem().value);
}

function ASPxCallbackPanelBarButtonsClient_EndCallBack(s, e) {
    //showLoadingGrid(false);
    switch (s.cpAction) {
        case "RELOADBARBUTTONS":
            if (s.cpResult != "OK") {
                showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
            } else {
                loadProductiveUnits();
            }
            break;
    }
}

function CallbackCalendar_CallbackError(s, e) {
    if (roBudget != null) {
        clearTimeout(roBudget.refreshTimmer);
        roBudget.loadingFunctionExtended(false);
    }
}

function roBudget_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        var objResultParams = null;
        if (typeof (s.cpObjResultParams) != 'undefined') objResultParams = s.cpObjResultParams;

        roBudget.endCallback(s.cpAction, JSON.parse(s.cpObjResult, roDateReviver), JSON.parse(objResultParams, roDateReviver));
    } else {
        if (roBudget != null) {
            roBudget.loadingFunctionExtended(false);

            if (!roBudget.parseErrorMessage(s.cpAction, JSON.parse(s.cpObjResult, roDateReviver))) showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
        } else {
            showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
        }
    }
}

function roBudgetComplentary_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        switch (s.cpAction) {
            case "SHIFTLAYERDEFINITION":
                roBudget.complementaryManager.finallyPrepareDialogElements(JSON.parse(s.cpObjResult, roDateReviver));
                break;
        }
    }
}

function roBudgetAssignments_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        switch (s.cpAction) {
            case "SHIFTLAYERDEFINITION":
                roBudget.assignmentsManager.finallyPrepareDialogElements(JSON.parse(s.cpObjResult, roDateReviver));
                break;
        }
    }
}

function ASPxOrgChartClientEndCallBack(s, e) {
    switch (s.cpAction) {
        case "GETSECURITYCHART":
            if (checkResult(s)) {
                orgChart.parseResponse(s, e);
            }
            break;
        case "SAVESECURITYCHART":
            break;
        default:
            hasChanges(false);
    }
}

function roCalendar_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        var objResultParams = null;
        if (typeof (s.cpObjResultParams) != 'undefined') objResultParams = s.cpObjResultParams;

        roScheduleCalendar.endCallback(s.cpAction, JSON.parse(s.cpObjResult, roDateReviver), JSON.parse(objResultParams, roDateReviver));
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

        var aiPlannerResult = "";
        try {
            aiPlannerResult = JSON.parse(s.cpErrorMsg);
        } catch (e) {
            aiPlannerResult = s.cpErrorMsg;
        }

        if (typeof (aiPlannerResult.msg) != 'undefined' && aiPlannerResult.msg != '') {
            showErrorPopup("OK.CopyOK", "INFO", "", aiPlannerResult.msg, "Error.OK", "", "RefreshScreen('fullRefresh');");
        } else {
            if (typeof s.cpErrorMsg != undefined && s.cpErrorMsg != "") {
                showErrorPopup("Error.CopyError", "ERROR", "", s.cpErrorMsg, "Error.OK", "", "");
            } else {
                showErrorPopup("Error.CopyError", "ERROR", "Error." + s.cpActionMsg, "", "Error.OK", "", "");
            }
        }
    } else if (s.cpAction == "CHECKPROGRESS") {
        if (s.cpActionResult == "OK") {
            clearInterval(monitor);
            AspxLoadingPopup_Client.Hide();
            showErrorPopup("OK.CopyOK", "INFO", "OK." + s.cpActionMsg, "", "Error.OK", "", "RefreshScreen('fullRefresh');");
        } else if (s.cpActionResult == "OKAIPLANNER") {
            var aiPlannerResult = JSON.parse(s.cpFileMsg)

            clearInterval(monitor);
            AspxLoadingPopup_Client.Hide();
            if (typeof (aiPlannerResult.msg) != 'undefined' && aiPlannerResult.msg != '') {
                showErrorPopup("OK.CopyOK", "INFO", "", aiPlannerResult.msg, "Error.OK", "", "RefreshScreen('fullRefresh');");
            } else {
                showErrorPopup("OK.CopyOK", "INFO", "OK.AIPlannerFinished", "", "Error.OK", "", "RefreshScreen('fullRefresh');");
            }

            if (typeof aiPlannerResult.file != 'undefined' && aiPlannerResult.file != "") {
                window.open("../Alerts/downloadFile.aspx?filename=" + aiPlannerResult.file);
            }
        }
    }
}

function ASPxProductiveUnitSelector_EndCallback(s, e) {
    $(".pUnitSelector").draggable({ revert: "invalid", containment: '#divMainBody', cursor: "move", helper: 'clone', zIndex: 100000 });
}

function roBudget_SelectedCell(oPUnit, oDay, oContainer) {
    if (roBudget != null) {
        if (roBudget.clientMode instanceof Robotics.Client.Controls.roBudgetCalendarDefinition) {
            if (oPUnit != null) {
                var actualPunit = oPUnit.ProductiveUnitData.ProductiveUnit.ID;
                var lastPUnit = parseInt(cmbProductiveUnitsClient.GetSelectedItem().value, 10);

                if (actualPunit != lastPUnit) {
                    cmbProductiveUnitsClient.SetSelectedItem(cmbProductiveUnitsClient.FindItemByValue(oPUnit.ProductiveUnitData.ProductiveUnit.ID));
                    lblProductiveUnitNameClient.SetText(cmbProductiveUnitsClient.FindItemByValue(oPUnit.ProductiveUnitData.ProductiveUnit.ID).text);
                    loadProductiveUnits();
                }
            } else {
                //cmbProductiveUnitsClient.SetSelectedIndex(0);
                //loadProductiveUnits();
            }
        }
    }
}

var monitor = -1;

function showCaptcha(bDeleteing) {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=COPYSPECIAL";
    if (bDeleteing) contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=DELETEBUDGET";

    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
}

function captchaCallback(action) {
    switch (action) {
        case "COPYSPECIAL":
            AspxLoadingPopup_Client.Show();
            roBudget.closeAdvancedCopyDialog();
            PerformAction();
            break;
        case "RUNAIPLANNER":
            AspxLoadingPopup_Client.Show();
            RunAIPlannerFinally(true);
            break;
        case "CLEARAIPLANNER":
            AspxLoadingPopup_Client.Show();
            RunAIPlannerFinally(false);
            break;
        case "DELETEBUDGET":
            roBudget.clientMode.finallyRemoveUnit();
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
    oParameters.idSecurityNode = roBudget.employeeFilter;
    oParameters.idProductiveUnit = roBudget.selectedEmployee.ProductiveUnitData.ProductiveUnit.ID;
    oParameters.initialBeginDate = moment(roBudget.oCalendar.BudgetData[0].PeriodData.DayData[roBudget.selectedMinColumn].PlanDate).clone().toDate();
    oParameters.initialEndDate = moment(roBudget.oCalendar.BudgetData[0].PeriodData.DayData[roBudget.selectedMaxColumn].PlanDate).clone().toDate();
    oParameters.pasteStartDate = moment(roBudget.selectedDay.PlanDate).clone().toDate();
    oParameters.filters = roBudget.advCopyManager.getFiltersValue();
    oParameters.copyEmployees = (roBudget.clientMode instanceof Robotics.Client.Controls.roBudgetCalendarSchedule ? true : false);

    oParameters.action = 'PERFORM_ACTION';

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    PerformActionCallbackClient.PerformCallback(strParameters);
}

function showDaySelected(selectedDay) {
    var selDate = moment(selectedDay, "DD/MM/YYYY");
    $('#divDayCalendar').data('dateRangePicker').setDateRange(selDate.format(localFormat), selDate.format(localFormat));
}

function RefreshScreen(DataType) {
    switch (DataType) {
        case '1':
        case 'fullRefresh':
            refreshBudget();
            break;
    }
    return;
}

function checkResult(oResult) {
    if (oResult.cpResult == 'KO') {
        showErrorPopup("SaveName.Error.Text", "ERROR", "", oResult.cpMessage, "SaveName.Error.Option1Text", "SaveName.Error.Option1Description", "", "", "", "", "", "", "");
        return false;
    }
    return true;
}

var oDetailCalendar = null;
var detailPopup = null;

function setUpAndOpenDetailDialog(budgetDetail, idOrgChartNode, sDate, pUnitMode, loadIndictments) {
    var formID = '';
    var buttons = [];

    formID = "#dialogCellDetail";

    detailPopup = $(formID).dialog({
        autoOpen: false,
        height: 'auto',
        width: '1100px',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: Globalize.formatMessage("Done"),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                detailPopup.dialog("close");
                refreshBudget();
            }
        }],
        close: function () {
        }
    });

    detailPopup.dialog("open");

    var budgetView = getActualBudgetViewType(false);
    var detailView = Robotics.Client.Constants.TypeView.DayDetail;

    switch (budgetView) {
        case Robotics.Client.Constants.TypeView.Definition:
            detailView = Robotics.Client.Constants.TypeView.DayDetail;
            break;
        case Robotics.Client.Constants.TypeView.Detail:
        case Robotics.Client.Constants.TypeView.Planification:
            detailView = Robotics.Client.Constants.TypeView.DaySchedule;
            break;
    }

    oDetailCalendar = new Robotics.Client.Controls.roCalendar('ctl00_contentMainBody_oDetailCalendar', detailView, showLoadingGrid, showErrorPopup);
    oDetailCalendar.create();
    oDetailCalendar.loadData(budgetDetail.BudgetData[0].PeriodData.DayData[0].ProductiveUnitMode.UnitModePositions, idOrgChartNode, sDate, budgetDetail.BudgetData[0].ProductiveUnitData.ProductiveUnit, pUnitMode, loadIndictments);

    setupPopupDescription(budgetDetail.BudgetData[0].ProductiveUnitData.ProductiveUnit, budgetDetail.BudgetData[0].PeriodData.DayData[0].ProductiveUnitMode, sDate);
}

function setupPopupDescription(pUnit, pUnitMode, sDate) {
    var lblDayInformation = $('#ctl00_contentMainBody_lblDayInformation');

    lblDayInformation.empty();

    var nodeName = $('<div></div>').attr('class', 'alignLeft fontBold').html('<span class="lineDescriptionHeight" style="padding-left:10px">' + pUnit.Name + '</span>');

    var nodeDate = $('<div></div>').attr('class', 'alignLeft').html('<span class="lineDescriptionHeight">' + Globalize.formatDate(sDate, { date: "full" }) + ':</span>');

    var modeTitle = $('<div></div>').attr('class', 'alignLeft paddingLeft').html('<span  class="lineDescriptionHeight">' + Globalize.formatMessage('roPUnitMode') + ':</span>');
    var modeDescription = $('<div></div>').attr('class', 'alignLeft').attr('style', 'padding:6px;margin-left:5px;background-color:' + pUnitMode.HtmlColor + ';color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(pUnitMode.HtmlColor)).html(pUnitMode.Name);

    //var positionTitle = $('<div></div>').attr('class', 'alignLeft paddingLeft').html('<span  class="lineDescriptionHeight">' + Globalize.formatMessage('roPosition') + ':</span>');
    //var positionDescription = $('<div></div>').attr('class', 'alignLeft').attr('style', 'padding:6px;margin-left:5px;background-color:' + pUnitModePosition.AssignmentData.Color + ';color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(pUnitModePosition.AssignmentData.Color)).html(pUnitModePosition.AssignmentData.Name);

    //var shiftTitle = $('<div></div>').attr('class', 'alignLeft paddingLeft').html('<span  class="lineDescriptionHeight">' + Globalize.formatMessage('roShift') + ':</span>');
    //var shiftDescription = $('<div></div>').attr('class', 'alignLeft').attr('style', 'padding:6px;margin-left:5px;background-color:' + pUnitModePosition.ShiftData.Color + ';color:' + new Robotics.Client.Common.roHtmlColor().invertCssColor(pUnitModePosition.ShiftData.Color)).html(pUnitModePosition.ShiftData.Name);

    //var actual = 0;
    //var total = 0;

    //for (var i = 0; i < pUnitMode.UnitModePositions.length; i++) {
    //    total = total + pUnitMode.UnitModePositions[i].Quantity;
    //    for (var j = 0; j < pUnitMode.UnitModePositions[i].EmployeesData.length; j++) {
    //        if (pUnitMode.UnitModePositions[i].EmployeesData[j].IDEmployee > 0) { actual++; }
    //    }
    //}

    //var employeesTitle = $('<div></div>').attr('class', 'alignLeft paddingLeft').html('<span  class="lineDescriptionHeight">' + Globalize.formatMessage('roEmployeesAssigned') + ':</span>');
    //var employeesDescription = $('<div></div>').attr('class', 'alignLeft fontBold').html('<span  class="lineDescriptionHeight" style="padding-left:6px">' + Globalize.formatNumber(actual) + '/' + Globalize.formatNumber(total) + '</span>');

    var costTitle = $('<div></div>').attr('class', 'alignLeft paddingLeft').html('<span  class="lineDescriptionHeight">' + Globalize.formatMessage('ropUnitCost') + ':</span>');
    var costDescription = $('<div></div>').attr('class', 'alignLeft fontBold').html('<span  class="lineDescriptionHeight" style="padding-left:6px">' + Globalize.formatNumber(pUnitMode.CostValue) + '</span>');

    lblDayInformation.append(nodeDate, nodeName, modeTitle, modeDescription, costTitle, costDescription);//, modeTitle, modeDescription, positionTitle, positionDescription, shiftTitle, shiftDescription);
}

function oDetailCalendar_calendar_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        var objResultParams = null;
        if (typeof (s.cpObjResultParams) != 'undefined') objResultParams = s.cpObjResultParams;

        oDetailCalendar.endCallback(s.cpAction, JSON.parse(s.cpObjResult, roDateReviver), JSON.parse(objResultParams, roDateReviver));
    } else {
        if (oDetailCalendar != null) {
            oDetailCalendar.loadingFunctionExtended(false);

            if (!oDetailCalendar.parseErrorMessage(s.cpAction, JSON.parse(s.cpObjResult, roDateReviver))) showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
        } else {
            showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
        }
    }
}

function oDetailCalendar_Complementary_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        switch (s.cpAction) {
            case "SHIFTLAYERDEFINITION":
                oDetailCalendar.complementaryManager.finallyPrepareDialogElements(JSON.parse(s.cpObjResult, roDateReviver));
                break;
        }
    }
}

function oDetailCalendar_assignment_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        switch (s.cpAction) {
            case "SHIFTLAYERDEFINITION":
                oDetailCalendar.assignmentsManager.finallyPrepareDialogElements(JSON.parse(s.cpObjResult, roDateReviver));
                break;
        }
    }
}

function RunAIPlanner(bSave) {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=RUNAIPLANNER";
    if (!bSave) contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=CLEARAIPLANNER";

    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
}

function RunAIPlannerFinally(bSave) {
    try {
        if (roBudget != null && typeof roBudget.clientMode != 'undefined' && roBudget.clientMode != null) {
            var oParameters = {};
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.initialBeginDate = moment(roBudget.firstDate).clone().toDate();
            oParameters.initialEndDate = moment(roBudget.endDate).clone().toDate();
            oParameters.idOrgChartNode = getOrgChartFilter();
            oParameters.pUnitFilter = roBudget.assignmentsFilter;

            if (bSave) oParameters.action = 'RUNAIPLANNER';
            else oParameters.action = 'CLEARAIPLANNER';

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);
            PerformActionCallbackClient.PerformCallback(strParameters);
        }
    } catch (e) { showError("saveChanges", e); }
}