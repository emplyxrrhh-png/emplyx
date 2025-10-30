VTPortal.plannedHoliday = function (params) {
    var requestId = ko.observable(null), remarks = ko.observable('');
    var canSave = ko.observable(false), canDelete = ko.observable(false), viewHistory = ko.observable(false), popupVisible = ko.observable(false), viewActions = ko.observable(false);
    var allDayPermit = ko.observable(true), calendarStatus = ko.observable({}), isRequestingData = true, selectedIndex = ko.observable(0);

    var holidayShiftIsWorkingDays = false;
    var requestTypeSelected = ko.observable("");

    var minDate = ko.observable(moment().startOf('day').startOf('year').add(-1, 'year').toDate());
    var maxDate = ko.observable(moment().startOf('day').endOf('year').add(1, 'year').toDate());

    var availableTabPanels = [{ "ID": 1, "cssClass": "dx-icon-configRequest" }, { "ID": 2, "cssClass": "dx-icon-selectDate" }];
    if ($.grep(VTPortal.roApp.empPermissions().Requests, function (e) { return (e.RequestType == 9 && e.Permission == true && typeof e.HasRankings != 'undefined' && e.HasRankings == true); }).length == 1) {
        var item = availableTabPanels.find(function (item) { return item.ID == "3"; });
        if (item == null) {
            availableTabPanels.push({ "ID": 3, "cssClass": "dx-icon-rankings", "badge": "" });
        }
    }

    var calSelectedDays = {}, iHour = ko.observable(moment().startOf('day').toDate()), eHour = ko.observable(moment().endOf('day').toDate());

    var noRankingDateSelected = ko.observable(true), hasRankingData = ko.observable(false), hasCoverageData = ko.observable(false);
    var rankingDS = ko.observable([]), txtMinimumCoverageRequiered = ko.observable(0), txtDifferentialCoverage = ko.observable(0);
    var requestStatus = ko.observable(0), idRequestTypeSelected = ko.observable(0), requestReasonSelected = ko.observable(0);

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    if ($.grep(VTPortal.roApp.empPermissions().Requests, function (e) {
        return ((e.RequestType == 6 && e.Permission == true && typeof e.HasRankings != 'undefined' && e.HasRankings == true) ||
            (e.RequestType == 13 && e.Permission == true && typeof e.HasRankings != 'undefined' && e.HasRankings == true));
    }).length == 1) {
        var item = availableTabPanels.find(function (item) { return item.ID == "3"; });
        if (item == null) {
            availableTabPanels.push({ "ID": 3, "cssClass": "dx-icon-rankings", "badge": "" });
        }
    }

    var cmbRequestValue1Changed = function (data) {
        if (!formReadOnly()) calSelectedDays = {};

        var selectedItem = $.grep(requestValue1DS(), function (e) { return e.FieldValue == data.value; });
        idRequestTypeSelected(6);
        requestReasonSelected(parseInt(data.value, 10))

        if (selectedItem.length > 0) {
            if (selectedItem[0].RelatedInfo == "S") {
                idRequestTypeSelected(6);
                loadShiftDetail(parseInt(data.value, 10));
                allDayPermit(true);
            } else {
                idRequestTypeSelected(13);
                holidayShiftIsWorkingDays = true;
            }
            requestTypeSelected(selectedItem[0].RelatedInfo);
        }
    }

    var onChangeCalendarSelection = function () {
        var item = availableTabPanels.find(function (item) { return item.ID == 3; });
        if (item == null) {
            item = { "ID": 3, "cssClass": "dx-icon-rankings", "badge": "" };
        }

        if (requestStatus() == 0 || requestStatus() == 1) {
            if (VTPortalUtils.utils.checkIfRequestHasRanquings(idRequestTypeSelected(), requestReasonSelected())) {
                var apiRobotics = null;
                var onWSError = function (error) {
                    noRankingDateSelected(true);
                    hasRankingData(false);
                    hasCoverageData(false);
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorRequestingRankings"), 'error', 0);
                }

                var onWSResult = function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        setTimeout(function () { $("#configRequestTab").dxTabs('instance').repaint(); }, 150);

                        if (result.Rankings != null) {
                            for (var i = 0; i < result.Rankings.RequestEmployeeRankingPositions.length; i++) {
                                if (result.Rankings.RequestEmployeeRankingPositions[i].IDEmployee == VTPortal.roApp.employeeId) {
                                    result.Rankings.RequestEmployeeRankingPositions[i].cssClass = 'rankingMyPosition'
                                } else {
                                    result.Rankings.RequestEmployeeRankingPositions[i].cssClass = 'listMenuItemPosition';
                                }
                            }
                            rankingDS(result.Rankings.RequestEmployeeRankingPositions);
                        }
                        else rankingDS([]);

                        if (result.RankingEnabled) {
                            item.badge = result.ActualPosition;
                        } else {
                            item.badge = "";
                        }
                        if (result.CoverageEnabled) {
                            txtMinimumCoverageRequiered(result.Coverages.MinCoverage);
                            txtDifferentialCoverage(result.Coverages.DifferentialQuantity);
                            item.badge = item.badge + "/" + result.Coverages.DifferentialQuantity;
                        }

                        availableTabPanels = availableTabPanels.remove(function (item) { return item.ID == 3; })
                        availableTabPanels.push(item);

                        noRankingDateSelected(false);
                        hasRankingData(result.RankingEnabled);
                        hasCoverageData(result.CoverageEnabled);
                    } else {
                        noRankingDateSelected(true);
                        hasRankingData(false);
                        hasCoverageData(false);
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorRequestingRankings"), 'error', 0);
                    }
                };
                if (apiRobotics == null) apiRobotics = new WebServiceRobotics(onWSResult, onWSError);

                var strDatesSelected = "";

                var checkInitial = moment().startOf('year');
                var checkEnd = moment().endOf('year').add(1, 'year');

                while (checkInitial < checkEnd) {
                    calSelectedDays[checkInitial.format("YYYYMMDD")]

                    if (typeof calSelectedDays[checkInitial.format("YYYYMMDD")] != 'undefined' && calSelectedDays[checkInitial.format("YYYYMMDD")] == true) {
                        if (strDatesSelected != '') strDatesSelected += '/'
                        strDatesSelected += checkInitial.format("YYYYMMDD");
                    }
                    checkInitial = checkInitial.add(1, 'day');
                }

                var firstDate = strDatesSelected.split('/');
                var xDate = null;
                if (firstDate.length > 0 && firstDate[0] != '') {
                    xDate = moment(firstDate[0], 'YYYYMMDD');
                }
                if (xDate != null) apiRobotics.getRankingForDay(xDate, idRequestTypeSelected(), requestReasonSelected());
                else {
                    noRankingDateSelected(true);
                    hasRankingData(false);
                    hasCoverageData(false);
                }
            } else {
                item.badge = "";
                availableTabPanels = availableTabPanels.remove(function (item) { return item.ID == 3; })
                availableTabPanels.push(item);
                noRankingDateSelected(true);
                hasRankingData(false);
                hasCoverageData(false);

                setTimeout(function () { $("#configRequestTab").dxTabs('instance').repaint(); }, 150);
            }
        } else {
            item.badge = "";
            availableTabPanels = availableTabPanels.remove(function (item) { return item.ID == 3; })
            availableTabPanels.push(item);
            noRankingDateSelected(true);
            hasRankingData(false);
            hasCoverageData(false);

            setTimeout(function () { $("#configRequestTab").dxTabs('instance').repaint(); }, 150);
        }
    }

    var formReadOnly = ko.observable(false), formReadOnlyCheck = ko.computed(function () { return requestTypeSelected() == "C" });
    var reqValue1 = ko.observable(null), requestValue1DS = ko.observable([]);
    var myApprovalsBlock = VTPortal.approvalHistory();

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                remarks(result.Comments);

                requestStatus(result.ReqStatus);

                if (result.ReqStatus == 0) canDelete(true);
                else canDelete(false);

                viewActions(false);
                if (VTPortal.roApp.impersonatedIDEmployee != -1) {
                    if (result.ReqStatus == 0 || result.ReqStatus == 1) {
                        viewActions(true);
                        canDelete(false);
                    }
                }

                myApprovalsBlock.refreshApprovals(result.RequestsHistoryEntries);

                if (moment(result.strDate1, "YYYY-MM-DD HH:mm:ss") < moment(minDate())) minDate(moment(result.strDate1, "YYYY-MM-DD HH:mm:ss").startOf('month').toDate());
                if (moment(result.strDate2, "YYYY-MM-DD HH:mm:ss") > moment(maxDate())) maxDate(moment(result.strDate2, "YYYY-MM-DD HH:mm:ss").endOf('year'));

                if (result.RequestDays != null && result.RequestDays.length > 0) {
                    for (var i = 0; i < result.RequestDays.length; i++) {
                        allDayPermit(result.RequestDays[i].AllDay);
                        calSelectedDays[moment.tz(result.RequestDays[i].RequestDate, VTPortal.roApp.serverTimeZone).add(12, 'hours').format("YYYYMMDD")] = true;
                        iHour(moment.tz(result.RequestDays[i].FromTime, VTPortal.roApp.serverTimeZone).toDate());
                        eHour(moment.tz(result.RequestDays[i].ToTime, VTPortal.roApp.serverTimeZone).toDate());
                    }
                } else {
                    allDayPermit(true);
                    var iDate = moment(result.strDate1, "YYYY-MM-DD HH:mm:ss").startOf('day');
                    var eDate = moment(result.strDate2, "YYYY-MM-DD HH:mm:ss").startOf('day');
                    while (iDate <= eDate) {
                        calSelectedDays[iDate.clone().add(12, 'hours').format("YYYYMMDD")] = true;
                        iDate = iDate.add(1, 'day');
                    }
                }

                if (result.RequestType == 6) {
                    requestTypeSelected("S");
                    reqValue1(result.IdShift + '');
                } else if (result.RequestType == 13) {
                    requestTypeSelected("C");
                    reqValue1(result.IdCause + '');
                }

                loadWorkingCalendar(new Date());
            } else {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }, function (error) {
            var onContinue = function () {
                VTPortal.roApp.redirectAtHome();
            }
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequest"), 'error', 0, onContinue);
        }).getRequest(requestId());
    }

    var loadLists = function (lstdataSource) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                result.SelectFields.forEach(function (item, index) {
                    if (item.RelatedInfo == "C") {
                        item.FieldValue = item.FieldValue * 100;
                    }
                });

                requestValue1DS(result.SelectFields);

                if (requestId() != null && requestId() > 0) {
                    loadRequest(requestId());
                } else {
                    if (result.SelectFields.length > 0) {
                        //if (result.SelectFields[0].RelatedInfo == "C") {
                        //    reqValue1(result.SelectFields[0].FieldValue*100);
                        //}
                        //else {
                        reqValue1(result.SelectFields[0].FieldValue);
                        //}

                        requestTypeSelected(result.SelectFields[0].RelatedInfo);

                        if (result.SelectFields[0].RelatedInfo == "S") loadShiftDetail(result.SelectFields[0].FieldValue);

                        loadWorkingCalendar(new Date());
                    }
                }
            } else {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }, function (error) {
            var onContinue = function () {
                VTPortal.roApp.redirectAtHome();
            }
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinue);
        }).getGenericList(lstdataSource);
    }

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Holidays) {
            requestValue1DS([]);

            loadLists('plannedholiday');

            if (requestId() != null && requestId() > 0) {
                formReadOnly(true);
                VTPortalUtils.utils.markRequestAsRead(requestId());
            }

            if (VTPortal.roApp.impersonatedIDEmployee == -1) {
                if (requestId() != null) canDelete(true);
            } else {
                if (requestId() != null) viewActions(false);
            }

            if (requestId() == null) canSave(true);
            if (requestId() != null) viewHistory(true);
        } else {
            var onContinue = function () {
                VTPortal.roApp.loadInitialData(false, false, true, false, false);
                VTPortal.roApp.redirectAtHome();
            }

            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roIncorrectApiVersion'), 'error', 0, onContinue);
        }
    }

    function loadShiftDetail(idShift) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                holidayShiftIsWorkingDays = result.Result;
                if (selectedIndex() == 1) setTimeout(function () { $("#calendarSelectorContainer").dxCalendar("instance").repaint(); }, 100);
            } else {
                var onContinue = function () {
                    VTPortal.roApp.loadInitialData(false, false, true, false, false);
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }).checkIsWorkingdayShift(idShift);
    }

    function loadWorkingCalendar(selDate) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var cData = calendarStatus();
                for (var i = 0; i < result.oCalendar.DayData.length; i++) {
                    cData[moment.tz(result.oCalendar.DayData[i].PlanDate, VTPortal.roApp.serverTimeZone).add(12, 'hours').format("YYYYMMDD")] = result.oCalendar.DayData[i];
                }
                calendarStatus(cData);
                if (selectedIndex() == 1) setTimeout(function () { $("#calendarSelectorContainer").dxCalendar("instance").repaint(); }, 100);
            } else {
                calendarStatus([]);
                var onContinue = function () {
                    VTPortal.roApp.loadInitialData(false, false, true, false, false);
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
            isRequestingData = false;
        }, function () { isRequestingData = false; }).getAvailablePermitsCalendar(selDate, -1);
    }

    function saveRequest() {
        if (requestTypeSelected() == "S") {
            saveHolidayRequest();
        } else {
            savePlannedHolidayRequest();
        }
    }

    var wsRobotics = null;

    function savePlannedHolidayRequest() {
        var startHour = moment(iHour()).format('HH:mm');
        var endHour = moment(eHour()).format('HH:mm');

        var strToShift = '1899-12-';
        var strFromShift = '1899-12-30 ';

        if (startHour > endHour) strToShift += '31 ';
        else strToShift += '30 ';

        var strFromTime = moment(strFromShift + startHour.format('HH:mm'), 'YYYY-MM-DD HH:mm');
        var strToTime = moment(strToShift + endHour.format('HH:mm'), 'YYYY-MM-DD HH:mm');

        var strDatesSelected = "";

        var checkInitial = moment().startOf('year').add(-1, 'year');
        var checkEnd = moment().endOf('year').add(1, 'year');

        while (checkInitial < checkEnd) {
            calSelectedDays[checkInitial.format("YYYYMMDD")]

            if (typeof calSelectedDays[checkInitial.format("YYYYMMDD")] != 'undefined' && calSelectedDays[checkInitial.format("YYYYMMDD")] == true) {
                if (strDatesSelected != '') strDatesSelected += '/'
                strDatesSelected += checkInitial.format("YYYYMMDD");
            }
            checkInitial = checkInitial.add(1, 'day');
        }

        if (strDatesSelected == "") {
            selectedIndex(1);
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roSelectDayInfo'), 'info', 2000);
        } else {
            var onWSError = function (error) {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
            }

            var onWSResult = function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.redirectAtHome();
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
                } else {
                    var onContinue = function () {
                        wsRobotics.savePlannedHoliday(strDatesSelected, allDayPermit(), strFromTime, strToTime, reqValue1() == null ? -1 : (reqValue1() / 100), remarks(), true);
                    }

                    VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
                }
            };

            wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

            wsRobotics.savePlannedHoliday(strDatesSelected, allDayPermit(), strFromTime, strToTime, reqValue1() == null ? -1 : (reqValue1() / 100), remarks(), false);
        }
    }

    function saveHolidayRequest() {
        var strDatesSelected = "";

        var checkInitial = moment().startOf('year').add(-1, 'year');
        var checkEnd = moment().endOf('year').add(1, 'year');

        while (checkInitial < checkEnd) {
            calSelectedDays[checkInitial.format("YYYYMMDD")]

            if (typeof calSelectedDays[checkInitial.format("YYYYMMDD")] != 'undefined' && calSelectedDays[checkInitial.format("YYYYMMDD")] == true) {
                if (strDatesSelected != '') strDatesSelected += '/'
                strDatesSelected += checkInitial.format("YYYYMMDD");
            }
            checkInitial = checkInitial.add(1, 'day');
        }

        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        if (strDatesSelected == "") {
            selectedIndex(1);
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roSelectDayInfo'), 'info', 2000);
        } else {
            var onWSResult = function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.redirectAtRequestList(13);
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
                } else {
                    var onContinue = function () {
                        wsRobotics.savePermissions(strDatesSelected, reqValue1() == null ? -1 : reqValue1(), remarks(), true);
                    }

                    VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
                }
            };

            wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

            wsRobotics.savePermissions(strDatesSelected, reqValue1() == null ? -1 : reqValue1(), remarks(), false);
        }
    }

    function deleteRequest() {
        if (requestId() != null && requestId() > 0) VTPortalUtils.utils.deleteRequest(requestId());
    }

    function getCellTemplate(data) {
        var style = '', color = "#dcdcdc", absenceColor = "#dcdcdc", textColor = '#000000';
        var iGradient = 90, eGradient = 95;
        var selDay = calendarStatus()[moment(data.date).format("YYYYMMDD")];
        var workingHous = 0;
        var actuallyHolidays = false;

        var actualDayClass = '';
        if (moment(data.date).format("YYYYMMDD") == moment().format("YYYYMMDD")) actualDayClass = 'actualDate';

        if (selDay != null) {
            if (selDay.MainShift != null) {
                workingHous = selDay.MainShift.PlannedHours;
                if (requestTypeSelected() == "C") actuallyHolidays = selDay.IsHoliday;
                else actuallyHolidays = (selDay.IsHoliday || selDay.Alerts.OnHolidaysHours);
            }

            color = VTPortalUtils.calendar.getCellColor(calSelectedDays, data.date, data.view);

            absenceStyle = 'width:100%;height:100%;color: ' + absenceColor + ';';
            absenceStyle += 'background: ' + absenceColor + ';';
            absenceStyle += 'background: -webkit-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background: -moz-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background: -ms-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background: -o-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background: radial-gradient(circle closest-side at center, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background-repeat: no-repeat;';
            absenceStyle += 'background-position: center center;display:table;';//font-weight:bold;';

            style = 'width:100%;height:100%;color: ' + textColor + ';';
            style += 'background: ' + color + ';';
            style += 'background: -webkit-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background: -moz-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background: -ms-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background: -o-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background: radial-gradient(circle closest-side at center, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background-repeat: no-repeat;';
            style += 'background-position: center center;display:table;';

            if (data.view == 'month') {
                if (moment(data.date).isAfter(moment(minDate()).startOf('month').add(-1, 'day')) && moment(data.date).isBefore(moment(maxDate()))) {
                    if (moment(data.date).startOf('day') >= moment(minDate()).startOf('day')) {
                        if ((((workingHous > 0 && holidayShiftIsWorkingDays == true) || holidayShiftIsWorkingDays == false) && actuallyHolidays == false) || (requestId() > 0)) {
                            return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span class='" + actualDayClass + "' style='display:table-cell;vertical-align:middle;font-weight:bold'>" + data.text + "</span></div></div></div></div>";
                        } else {
                            return "<div class='no-working-day'> <span class='" + actualDayClass + "' >" + data.text + "</span></div>";
                        }
                    } else {
                        return "<span class='" + actualDayClass + "' >" + data.text + "</span>";
                    }
                } else {
                    return "";
                }
            } else if (data.view == 'year') {
                if (moment(data.date).isAfter(moment(minDate()).startOf('month').add(-1, 'day')) && moment(data.date).isBefore(moment(maxDate()))) {
                    var checkInitial = moment(data.date).startOf('month');
                    var checkEnd = moment(data.date).endOf('month');

                    //if (checkInitial < moment().startOf('day')) checkInitial = moment().startOf('day');
                    var bCanRequest = false;
                    var bCounter = 0;

                    while (checkInitial < checkEnd) {
                        if (typeof calendarStatus()[checkInitial.format("YYYYMMDD")] != 'undefined' && calendarStatus()[checkInitial.format("YYYYMMDD")].MainShift != null && calendarStatus()[checkInitial.format("YYYYMMDD")].MainShift.PlannedHours > 0 && calendarStatus()[checkInitial.format("YYYYMMDD")].IsHoliday == false) {
                            bCanRequest = true;
                            break;
                        }
                        checkInitial = checkInitial.add(1, 'day');
                    }

                    checkInitial = moment(data.date).startOf('month');
                    if (checkInitial < moment().startOf('day')) checkInitial = moment().startOf('day');

                    while (checkInitial < checkEnd) {
                        if (calSelectedDays[checkInitial.format("YYYYMMDD")] == true) {
                            bCounter += 1;
                        }

                        checkInitial = checkInitial.add(1, 'day');
                    }

                    if (bCanRequest || requestId() > 0) {
                        if (bCounter > 0) return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span style='display:table-cell;vertical-align:middle'>" + data.text.capitalize(true, true) + " (" + bCounter + ")</span></div></div></div></div>";
                        else return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span style='display:table-cell;vertical-align:middle'>" + data.text.capitalize(true, true) + "</span></div></div></div></div>";
                    } else {
                        return "<span>" + data.text + "</span>";
                    }
                } else {
                    return "";
                }
            }
        } else {
            if (moment(data.date).isAfter(moment(minDate()).startOf('month').add(-1, 'day')) && moment(data.date).isBefore(moment(maxDate()))) {
                if (!isRequestingData) {
                    isRequestingData = true;
                    loadWorkingCalendar(data.date);
                }

                return "<span class='" + actualDayClass + "' >" + data.text + "</span>";
            } else {
                return "";
            }
        }
    }

    var onCellClicked = function (e) {
        if (requestId() == null && typeof calendarStatus()[moment(e.value).format("YYYYMMDD")] != 'undefined') {
            var isActuallyHoliday = false;
            var selDay = calendarStatus()[moment(e.value).format("YYYYMMDD")];

            if (requestTypeSelected() == "C") isActuallyHoliday = selDay.IsHoliday;
            else isActuallyHoliday = (selDay.IsHoliday || selDay.Alerts.OnHolidaysHours);

            if (((selDay.MainShift != null && selDay.MainShift.PlannedHours > 0 > 0 && holidayShiftIsWorkingDays == true) || holidayShiftIsWorkingDays == false) && isActuallyHoliday == false) {
                if (typeof calSelectedDays[moment(e.value).format("YYYYMMDD")] != 'undefined' && calSelectedDays[moment(e.value).format("YYYYMMDD")] == true) {
                    calSelectedDays[moment(e.value).format("YYYYMMDD")] = false;
                    $('#' + moment(e.value).format("YYYYMMDD")).css('background', 'transparent');
                } else {
                    calSelectedDays[moment(e.value).format("YYYYMMDD")] = true;

                    var color = '#ff5c35', textColor = '#000000', iGradient = 90, eGradient = 95;
                    var style = 'width:100%;height:100%;color: ' + textColor + ';';
                    style += 'background: ' + color + ';';
                    style += 'background: -webkit-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background: -moz-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background: -ms-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background: -o-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background: radial-gradient(circle closest-side at center, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
                    style += 'background-repeat: no-repeat;';
                    style += 'background-position: center center;display:table;';

                    $('#' + moment(e.value).format("YYYYMMDD")).attr('style', style);
                }
            }
        }
    };

    var changeTab = function (item) {
        if (item.ID == 2) {
            setTimeout(function () { $("#calendarSelectorContainer").dxCalendar("instance").repaint(); }, 200);
        } else if (item.ID == 3) {
            onChangeCalendarSelection();
        }
    }

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_PlannedHoliday'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: i18nextko.t('roSelectPlannedHoliday'),
        lblInitialHour: i18nextko.t('roRequestInitialHourLbl'),
        lblEndHour: i18nextko.t('roRequestEndHourLbl'),
        lblAllDay: i18nextko.t('roRequestAllDayLbl'),
        lblSelectDaysInfo: i18nextko.t('roSelectDayInfo'),
        subscribeBlock: globalStatus(),
        selectedTab: selectedIndex,
        scrollContent: {
            height: computedScrollHeight
        },
        tabPanelOptions: {
            dataSource: availableTabPanels,
            selectedIndex: selectedIndex,
            itemTemplate: "title",
            onSelectionChanged: function (e) {
                changeTab(e.addedItems[0]);
            }
        },
        popupVisible: popupVisible,
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 13, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 13, false); },
            text: '',
            hint: i18nextko.i18n.t('roRefuse'),
            icon: "Images/Common/refuse.png",
            visible: viewActions
        },
        btnHistory: {
            onClick: function () { popupVisible(true); },
            text: '',
            hint: i18nextko.i18n.t('roLblRequestApprovals'),
            icon: "Images/Common/historic.png",
            visible: viewHistory
        },
        btnSave: {
            onClick: saveRequest,
            text: '',
            hint: i18nextko.i18n.t('roSaveRequest'),
            icon: "Images/Common/save.png",
            visible: canSave
        },
        btnDelete: {
            onClick: deleteRequest,
            text: '',
            hint: i18nextko.i18n.t('roDeleteRequest'),
            icon: "Images/Common/delete.png",
            visible: canDelete
        },
        calendarOptions: {
            width: '100%',
            height: 450,
            useCellTemplate: true,
            cellTemplate: getCellTemplate,
            firstDayOfWeek: moment()._locale._week.dow,
            maxZoomLevel: 'month',
            zoomLevel: 'year',
            minZoomLevel: 'year',
            onCellClick: onCellClicked,
            showTodayButton: false,
            min: minDate,
            max: maxDate
        },
        remarks: {
            value: remarks,
            readOnly: formReadOnly
        },
        ckAllDay: {
            text: i18nextko.t('roRequestAllDayLbl'),
            value: allDayPermit,
            readOnly: formReadOnly,
            visible: formReadOnlyCheck
        },
        beginHour: {
            type: "time",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: iHour,
            readOnly: formReadOnly,
            valueChangeEvent: 'focusout'
        },
        endHour: {
            type: "time",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: eHour,
            readOnly: formReadOnly,
            valueChangeEvent: 'focusout'
        },
        cmbRequestValue1: {
            dataSource: requestValue1DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue1,
            readOnly: formReadOnly,
            onValueChanged: cmbRequestValue1Changed
        },
        allDayPermit: allDayPermit,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        noRankingDateSelected: noRankingDateSelected,
        hasRankingData: hasRankingData,
        hasCoverageData: hasCoverageData,
        rankingDS: rankingDS,
        listRanking: {
            dataSource: rankingDS,
            scrollingEnabled: false,
            collapsibleGroups: false,
            itemTemplate: 'RankingItem',
        },
        lblRankingHeader: i18nextko.t('roRankingTitle'),
        lblCoverageHeader: i18nextko.t('roCoverageTitle'),
        lblNoDataDescription: i18nextko.t('roNoRankingDescription'),
        lblMinimumCoverageRequiered: i18nextko.t('rolblMinimumCoverageRequiered'),
        txtMinimumCoverageRequiered: {
            value: txtMinimumCoverageRequiered,
            readOnly: true
        },
        lblDiferentialCoverage: i18nextko.t('rolblDiferentialCoverage'),
        txtDifferentialCoverage: {
            value: txtDifferentialCoverage,
            readOnly: true
        }
    };

    return viewModel;
};