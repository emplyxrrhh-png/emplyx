VTPortal.changeShift = function (params) {
    var requestId = ko.observable(null), remarks = ko.observable('');
    var canSave = ko.observable(false), canDelete = ko.observable(false), viewHistory = ko.observable(false), popupVisible = ko.observable(false), viewActions = ko.observable(false);

    var calendarStatus = ko.observable({}), isRequestingData = true, selectedIndex = ko.observable(0);
    var minDate = ko.observable(moment().startOf('day').startOf('year').add(-1, 'year').toDate()), maxDate = ko.observable(moment().startOf('day').endOf('year').add(1, 'year').toDate());
    var availableTabPanels = [{ "ID": 1, "cssClass": "dx-icon-configRequest" }, { "ID": 2, "cssClass": "dx-icon-selectDate" }];
    var calSelectedDays = {}, iHour = ko.observable(moment().startOf('day').toDate()), eHour = ko.observable(moment().endOf('day').toDate());

    var minSelectedDate = ko.observable(null), maxSelectedDate = ko.observable(null);

    var isFloating = ko.observable(false);
    var iHour = ko.observable(moment().startOf('day').toDate());

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));
    if (typeof params.param != 'undefined' && parseInt(params.param, 10) != -1) {
        minSelectedDate(null);
        maxSelectedDate(null);
        // calSelectedDays[moment(params.param, 'YYYY-MM-DD').format("YYYYMMDD")] = false;
    }

    var formReadOnly = ko.observable(false);

    var reqValue1 = ko.observable(null), reqValue2 = ko.observable(null), reqValue3 = ko.observable(null);

    var requestValue1DS = ko.observable([]);
    var requestValue2DS = ko.observable([]);
    var requestValue3DS = ko.observable([]);

    var myApprovalsBlock = VTPortal.approvalHistory();

    var cmbRequestValue1Changed = function (e) {
        if (!(requestId() != null && requestId() > 0)) {
            var selectedIdShift = e.value;
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    if (result.IsFloating) {
                        iHour(moment.tz(result.StartFloating, VTPortal.roApp.serverTimeZone).toDate());
                        isFloating(true);
                    } else {
                        iHour(moment().startOf('day').toDate());
                        isFloating(false);
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
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingShiftInfo"), 'error', 0, onContinue);
            }).checkIsFloatingShift(selectedIdShift);
        }
    }

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                remarks(result.Comments);

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

                var iDate = moment(result.strDate1, "YYYY-MM-DD HH:mm:ss").startOf('day');
                var eDate = moment(result.strDate2, "YYYY-MM-DD HH:mm:ss").startOf('day');
                while (iDate <= eDate) {
                    calSelectedDays[iDate.clone().add(12, 'hours').format("YYYYMMDD")] = true;
                    iDate = iDate.add(1, 'day');
                }

                if (result.strStartShift != '') {
                    iHour(moment(result.strStartShift, "YYYY-MM-DD HH:mm:ss"));

                    switch (moment(result.strStartShift, "YYYY-MM-DD HH:mm:ss").date()) {
                        case 29:
                            reqValue3('0');
                            break;
                        case 30:
                            reqValue3('1');
                            break;
                        case 31:
                            reqValue3('2');
                            break;
                    }

                    isFloating(true);
                } else {
                    isFloating(false);
                }

                reqValue2(result.IdShift + '');
                result.Field4 == 0 ? reqValue1('-1') : reqValue1(result.Field4 + '');

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
                var initialShifts = [{ FieldName: i18nextko.i18n.t('roAllShifts'), FieldValue: '-1' }];

                initialShifts = initialShifts.add(result.SelectFields.clone());

                requestValue1DS(initialShifts);
                requestValue2DS(result.SelectFields.clone());

                if (requestId() != null && requestId() > 0) {
                    loadRequest(requestId());
                } else {
                    if (initialShifts.length > 0) reqValue1(initialShifts[0].FieldValue);
                    if (result.SelectFields.length > 0) reqValue2(result.SelectFields[0].FieldValue);

                    loadWorkingCalendar(new Date());
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
            requestValue2DS([]);
            requestValue3DS([{ FieldName: i18nextko.i18n.t('roShift_DayBefore'), FieldValue: '0' }, { FieldName: i18nextko.i18n.t('roShift_ActualDay'), FieldValue: '1' }, { FieldName: i18nextko.i18n.t('roShift_DayAfter'), FieldValue: '2' }]);
            reqValue3('1');

            loadLists('shifts.workingtype');

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

    var wsRobotics = null;
    function saveRequest() {
        if (minSelectedDate() == null || maxSelectedDate == null) {
            selectedIndex(1);
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roSelectDayInfo'), 'info', 2000);
        } else {
            var startShiftHour = "";
            var idShift1 = reqValue1() != null ? reqValue1() : '-1';
            var idShift2 = reqValue2() != null ? reqValue2() : '-1';

            if (isFloating()) {
                var tmpMoment = moment('1899-12-30' + ' ' + moment(iHour()).format('HH:mm') + ':00', 'YYYY-MM-DD HH:mm:ss');
                switch (reqValue3()) {
                    case '0':
                        tmpMoment.add(-1, 'days')
                        break;
                    case '2':
                        tmpMoment.add(1, 'days')
                        break;
                }
                startShiftHour = tmpMoment.format('YYYY-MM-DD HH:mm');
            }

            var onWSError = function (error) {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
            }

            var onWSResult = function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.redirectAtRequestList(5);
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
                } else {
                    var onContinue = function () {
                        wsRobotics.saveChangeShift(minSelectedDate() == null ? VTPortalUtils.nullDate : minSelectedDate(), maxSelectedDate() == null ? VTPortalUtils.nullDate : maxSelectedDate(), idShift2, idShift1, startShiftHour, remarks(), true);
                    }

                    VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
                }
            };
            if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

            wsRobotics.saveChangeShift(minSelectedDate() == null ? VTPortalUtils.nullDate : minSelectedDate(), maxSelectedDate() == null ? VTPortalUtils.nullDate : maxSelectedDate(), idShift2, idShift1, startShiftHour, remarks(), false);
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
                actuallyHolidays = selDay.IsHoliday;
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
                return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span class='" + actualDayClass + "' style='display:table-cell;vertical-align:middle;font-weight:bold'>" + data.text + "</span></div></div></div></div>";
            } else if (data.view == 'year') {
                if (moment(data.date).isAfter(moment(minDate()).startOf('month').add(-1, 'day')) && moment(data.date).isBefore(moment(maxDate()))) {
                    var checkInitial = moment(data.date).startOf('month');
                    var checkEnd = moment(data.date).endOf('month');
                    var bCounter = 0;

                    while (checkInitial < checkEnd) {
                        if (calSelectedDays[checkInitial.format("YYYYMMDD")] == true) {
                            bCounter += 1;
                        }

                        checkInitial = checkInitial.add(1, 'day');
                    }

                    if (bCounter > 0) return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span style='display:table-cell;vertical-align:middle'>" + data.text.capitalize(true, true) + " (" + bCounter + ")</span></div></div></div></div>";
                    else return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span style='display:table-cell;vertical-align:middle'>" + data.text.capitalize(true, true) + "</span></div></div></div></div>";
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
            //Descomentar para activar la validación de días festivos
            //var selDay = calendarStatus()[moment(e.value).format("YYYYMMDD")];
            //var isActuallyHoliday = selDay.IsHoliday;

            //if (selDay.MainShift != null && selDay.MainShift.PlannedHours > 0 && isActuallyHoliday == false) {
            if (minSelectedDate() == null && maxSelectedDate() == null) {
                minSelectedDate(moment(e.value));
                maxSelectedDate(moment(e.value));
            } else {
                if (moment(e.value).isSame(minSelectedDate())) minSelectedDate(maxSelectedDate());
                else if (moment(e.value).isSame(maxSelectedDate())) maxSelectedDate(minSelectedDate());
                else if (moment(e.value) > minSelectedDate()) maxSelectedDate(moment(e.value));
                else if (moment(e.value) < minSelectedDate()) minSelectedDate(moment(e.value));
            }
            //}

            var checkInitial = moment(minDate());
            var checkEnd = moment(maxDate());

            while (checkInitial < checkEnd) {
                if (checkInitial >= minSelectedDate() && checkInitial <= maxSelectedDate()) {
                    calSelectedDays[checkInitial.format("YYYYMMDD")] = true;
                } else {
                    if (typeof calSelectedDays[checkInitial.format("YYYYMMDD")] != 'undefined') {
                        calSelectedDays[checkInitial.format("YYYYMMDD")] = false;
                    }
                }
                checkInitial = checkInitial.add(1, 'day');
            }

            $("#calendarSelectorContainer").dxCalendar("instance").repaint();
        }
    };

    var changeTab = function (item) {
        if (item.ID == 2) {
            setTimeout(function () { $("#calendarSelectorContainer").dxCalendar("instance").repaint(); }, 200);
        }
    }

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_ChangeShift'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: i18nextko.t('roSelectShift'),
        lblRequestValue2: i18nextko.t('roReeplaceShift'),
        lblInitialHour: i18nextko.t('roRequestShiftInitialHourLbl'),
        lblSelectDay: i18nextko.t('roRequestShiftSelectDayLbl'),
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
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 5, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 5, false); },
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
        beginHour: {
            type: "time",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: iHour,
            readOnly: formReadOnly,
            valueChangeEvent: 'focusout'
        },
        cmbRequestValue1: {
            dataSource: requestValue1DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue1,
            readOnly: formReadOnly
        },
        cmbRequestValue2: {
            dataSource: requestValue2DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue2,
            readOnly: formReadOnly,
            onValueChanged: cmbRequestValue1Changed
        },
        cmbRequestValue3: {
            dataSource: requestValue3DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue3,
            readOnly: formReadOnly
        },
        isFloating: isFloating,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};