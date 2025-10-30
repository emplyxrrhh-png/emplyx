VTPortal.cancelHolidays = function (params) {
    var requestId = ko.observable(null), remarks = ko.observable('');
    var canSave = ko.observable(false), canDelete = ko.observable(false), viewHistory = ko.observable(false), popupVisible = ko.observable(false), viewActions = ko.observable(false);

    var minDate = ko.observable(moment().startOf('day').toDate());
    var maxDate = ko.observable(moment().startOf('day').endOf('year').add(1, 'year').toDate());

    var calendarStatus = ko.observable({});
    var isRequestingData = true;
    var selectedIndex = ko.observable(0);

    var requestTypeSelected = ko.observable("");
    var reqValue1 = ko.observable(null), requestValue1DS = ko.observable([]);

    var availableTabPanels = [];
    availableTabPanels.push({ "ID": 1, "cssClass": "dx-icon-configRequest" });
    availableTabPanels.push({ "ID": 2, "cssClass": "dx-icon-selectDate" });
    var calSelectedDays = {};

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    var formReadOnly = ko.observable(false);

    var myApprovalsBlock = VTPortal.approvalHistory();

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

                if (moment(result.strDate1, "YYYY-MM-DD HH:mm:ss") < moment(minDate())) minDate(moment(result.strDate1, "YYYY-MM-DD HH:mm:ss").startOf('month').toDate());
                if (moment(result.strDate2, "YYYY-MM-DD HH:mm:ss") > moment(maxDate())) maxDate(moment(result.strDate2, "YYYY-MM-DD HH:mm:ss").endOf('year'));

                if (result.RequestDays != null && result.RequestDays.length > 0) {
                    for (var i = 0; i < result.RequestDays.length; i++) {
                        calSelectedDays[moment.tz(result.RequestDays[i].RequestDate, VTPortal.roApp.serverTimeZone).add(12, 'hours').format("YYYYMMDD")] = true;
                    }
                } else {
                    var iDate = moment(result.strDate1, "YYYY-MM-DD HH:mm:ss").startOf('day');
                    var eDate = moment(result.strDate2, "YYYY-MM-DD HH:mm:ss").startOf('day');

                    while (iDate <= eDate) {
                        calSelectedDays[iDate.clone().add(12, 'hours').format("YYYYMMDD")] = true;
                        iDate = iDate.add(1, 'day');
                    }
                }

                if (parseInt(result.IdShift, 10) > 0) {
                    reqValue1(result.IdShift + '');
                    requestTypeSelected("S");
                } else if (parseInt(result.IdCause, 10) > 0) {
                    reqValue1(result.IdCause + '');
                    requestTypeSelected("C");
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

    function loadWorkingCalendar(selDate) {
        if (requestTypeSelected() == "C") {
            var idCause = reqValue1() / 100;
        }

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
        }, function () { isRequestingData = false; }).getAvailablePermitsCalendar(selDate, (requestTypeSelected() == "C" ? idCause : -1));
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
                        reqValue1(result.SelectFields[0].FieldValue);
                        requestTypeSelected(result.SelectFields[0].RelatedInfo);

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

    var wsRobotics = null;

    function saveRequest() {
        var strDatesSelected = "";

        var checkInitial = moment().startOf('month')
        var checkEnd = moment().endOf('year').add(1, 'year');

        var reqVal = reqValue1() != null ? reqValue1() : '-1';

        if (reqVal != '-1' && requestTypeSelected() == "C") {
            reqVal = reqVal / 100;
        }

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
                    VTPortal.roApp.redirectAtRequestList(11);
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
                } else {
                    var onContinue = function () {
                        wsRobotics.saveCancelHolidays(strDatesSelected, (requestTypeSelected() == "C" ? reqVal : -1), (requestTypeSelected() == "S" ? reqVal : -1), remarks(), true);
                    }

                    VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
                }
            };
            if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

            wsRobotics.saveCancelHolidays(strDatesSelected, (requestTypeSelected() == "C" ? reqVal : -1), (requestTypeSelected() == "S" ? reqVal : -1), remarks(), false);
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
                actuallyHolidays = false;

                if (requestTypeSelected() == "S" && selDay.IsHoliday == true && selDay.MainShift.ID == parseInt(reqValue1(), 10)) actuallyHolidays = true;
                else if (requestTypeSelected() == "C" && selDay.Alerts.OnHolidaysHours) actuallyHolidays = true;
            }

            color = VTPortalUtils.calendar.getCellColor(calSelectedDays, data.date, data.view);

            absenceStyle = 'width:100%;height:100%;color: ' + absenceColor + ';';
            absenceStyle += 'background: ' + absenceColor + ';';
            absenceStyle += 'background: -webkit-radial-gradient(circle closest-side, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
            absenceStyle += 'background: -moz-radial-gradient(circle closest-side, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
            absenceStyle += 'background: -ms-radial-gradient(circle closest-side, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
            absenceStyle += 'background: -o-radial-gradient(circle closest-side, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
            absenceStyle += 'background: radial-gradient(circle closest-side at center, ' + absenceColor + ' 95%,rgba(255,255,255,0) 100%);';
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
                        if (actuallyHolidays || requestId() > 0) {
                            return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span class='" + actualDayClass + "' style='display:table-cell;vertical-align:middle;font-weight:bold'>" + data.text + "</span></div></div></div></div>";
                        } else {
                            return "<div class='no-working-day'> <span class='" + actualDayClass + "' >" + data.text + "</span></div>";
                        }
                    } else {
                        return "<span>" + data.text + "</span>";
                    }
                } else {
                    return "";
                }
            } else if (data.view == 'year') {
                if (moment(data.date).isAfter(moment(minDate()).startOf('month').add(-1, 'day')) && moment(data.date).isBefore(moment(maxDate()))) {
                    var checkInitial = moment(data.date).startOf('month');
                    var checkEnd = moment(data.date).endOf('month');

                    if (checkInitial < moment().startOf('day')) checkInitial = moment().startOf('day');

                    var bCanRequest = false;
                    var bCounter = 0;

                    while (checkInitial < checkEnd) {
                        var dayOnHolidays = false;

                        if (requestTypeSelected() == "S" && typeof calendarStatus()[checkInitial] != 'undefined' && calendarStatus()[checkInitial.format("YYYYMMDD")].IsHoliday == true && calendarStatus()[checkInitial.format("YYYYMMDD")].MainShift.ID == parseInt(reqValue1(), 10)) dayOnHolidays = true;
                        else if (requestTypeSelected() == "C" && typeof calendarStatus()[checkInitial] != 'undefined' && calendarStatus()[checkInitial.format("YYYYMMDD")].Alerts.OnHolidaysHours) dayOnHolidays = true;

                        if (dayOnHolidays) {
                            bCanRequest = true;
                            break;
                        }
                        checkInitial = checkInitial.add(1, 'day');
                    }

                    checkInitial = moment(data.date).startOf('month');
                    if (checkInitial < moment().startOf('day')) checkInitial = moment().startOf('day');

                    while (checkInitial < checkEnd) {
                        if (calSelectedDays[checkInitial.format("YYYYMMDD")] == true) bCounter += 1;

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
        if (requestId() == null) {
            if (typeof calendarStatus()[moment(e.value).format("YYYYMMDD")] != 'undefined' && (calendarStatus()[moment(e.value).format("YYYYMMDD")].IsHoliday || calendarStatus()[moment(e.value).format("YYYYMMDD")].Alerts.OnHolidaysHours)) {
                var color = '#ff5c35', textColor = '#000000', iGradient = 90, eGradient = 95;

                if (typeof calSelectedDays[moment(e.value).format("YYYYMMDD")] != 'undefined' && calSelectedDays[moment(e.value).format("YYYYMMDD")] == true) {
                    calSelectedDays[moment(e.value).format("YYYYMMDD")] = false;

                    color = "#dcdcdc";
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
                } else {
                    color = '#ff5c35';
                    calSelectedDays[moment(e.value).format("YYYYMMDD")] = true;

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
        }
    }

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_CancelHolidays'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: i18nextko.t('roSelectPlannedHoliday'),
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
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 11, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 11, false); },
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
        remarks: {
            value: remarks,
            readOnly: formReadOnly
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
        cmbRequestValue1: {
            dataSource: requestValue1DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue1,
            readOnly: formReadOnly,
            onValueChanged: function (data) {
                if (!formReadOnly()) calSelectedDays = {};

                var selectedItem = $.grep(requestValue1DS(), function (e) { return e.FieldValue == data.value; });

                if (selectedItem.length > 0) {
                    requestTypeSelected(selectedItem[0].RelatedInfo);
                    calendarStatus({});
                    loadWorkingCalendar(new Date());
                }
            }
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};