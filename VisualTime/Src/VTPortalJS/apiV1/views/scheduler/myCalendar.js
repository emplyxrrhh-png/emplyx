VTPortal.myCalendar = function (params) {
    var calendarStatus = ko.observable({});
    var selectedDayInfo = ko.observable(null);
    var isRequestingData = true;
    var selectedDay = moment().toDate();
    var popoverVisible = ko.observable(false);
    var modelIsReady = ko.observable(false);
    var dayToolbarActionsCalculated = ko.observable([]);
    var capacityInfo = ko.observable('');
    var CurrentSeating = ko.observable(0);
    var MaxSeating = ko.observable(0);
    var ZoneVisible = ko.observable(false);
    var selectedDayDetail = ko.observable(null);
    var hasPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.QuerySchedule)) {
            return true;
        } else {
            return false;
        }
    });
    var lblCapacityInfo = ko.computed(function () {
        if (capacityInfo() != '' && ZoneVisible() == true) {
            return i18nextko.i18n.t('roCurrentCapacity') + ': ' + capacityInfo();
        }
        else {
            return '';
        }
    });

    function viewShown(selDate) {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.QuerySchedule)) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var cData = calendarStatus();
                    for (var i = 0; i < result.oCalendar.DayData.length; i++) {
                        cData[moment.tz(result.oCalendar.DayData[i].PlanDate, VTPortal.roApp.serverTimeZone).add(12, 'hours').format("YYYYMMDD")] = result.oCalendar.DayData[i];
                    }
                    calendarStatus(cData);

                    setTimeout(function () { $("#calendarContainer").dxCalendar("instance").repaint(); $("#calendarContainer").height($('#scrollview').height()); }, 100);
                } else {
                    calendarStatus([]);
                    var onContinue = function () {
                        VTPortal.roApp.loadInitialData(false, false, true, false, false);
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
                isRequestingData = false;
                // $('#scrollview').height($('#panelsContent').height() - 20);
            }, function () { isRequestingData = false; }).getMyCalendar(selDate);
        }

        modelIsReady(true);
    };

    function pickTextColorBasedOnBgColorAdvanced(bgColor, lightColor, darkColor) {
        var color = (bgColor.charAt(0) === '#') ? bgColor.substring(1, 7) : bgColor;
        var r = parseInt(color.substring(0, 2), 16); // hexToR
        var g = parseInt(color.substring(2, 4), 16); // hexToG
        var b = parseInt(color.substring(4, 6), 16); // hexToB
        var uicolors = [r / 255, g / 255, b / 255];
        var c = uicolors.map(function (col) {
            if (col <= 0.03928) {
                return col / 12.92;
            }
            return Math.pow((col + 0.055) / 1.055, 2.4);
        });
        var L = (0.2126 * c[0]) + (0.7152 * c[1]) + (0.0722 * c[2]);
        return (L > 0.700) ? darkColor : lightColor;
    }

    function getCellTemplate(data) {
        var style = '';
        var color = "#ffffff", absenceColor = "";

        var selDay = calendarStatus()[moment(data.date).format("YYYYMMDD")];

        var actualDayClass = '';

        if (moment(data.date).format("YYYYMMDD") == moment().format("YYYYMMDD")) actualDayClass = 'actualDate';

        var dayRemarkValue = null;

        if (selDay != null) {
            if (selDay.MainShift != null) color = selDay.MainShift.Color;
            if (selDay.IncidenceData != null && typeof selDay.IncidenceData.NormalWork != 'undefined' && selDay.IncidenceData.NormalWork != 0) {
                dayRemarkValue = selDay.IncidenceData.NormalWork;
                if (selDay.IncidenceData.NormalWork > 0) absenceColor = "#05580c"
                else if (selDay.IncidenceData.NormalWork < 0) absenceColor = "#e00032"
            } else {
                if (selDay.MainShift != null) {
                    absenceColor = color;
                    if (selDay.Alerts.OnAbsenceHours || selDay.Alerts.OnAbsenceDays || selDay.Alerts.OnHolidays || selDay.Alerts.OnOvertimesHours || selDay.Alerts.OnHolidaysHours) {
                        if (selDay.Alerts.OnAbsenceHours) { absenceColor = '#00f12b'; }
                        else if (selDay.Alerts.OnOvertimesHours) { absenceColor = '#0019cc'; }
                        else if (selDay.Alerts.OnHolidaysHours) { absenceColor = '#ffa500'; }
                        else if (selDay.Alerts.OnAbsenceDays) { absenceColor = '#a400cc'; }
                        else if (selDay.Alerts.OnHolidays) { absenceColor = '#dc7214'; }
                    }
                }
            }
        } else {
            if (!isRequestingData) {
                isRequestingData = true;
                viewShown(data.date);
            }
        }

        var gradientColor = VTPortalUtils.utils.generateGradientFromColor(selDay);

        // var textColor = new Robotics.Client.Common.roHtmlColor().invertCssColor(color);

        var textColor = pickTextColorBasedOnBgColorAdvanced(color, '#FFFFFF', '#000000')

        //if (color == "#fff" || color == "#ffffff") {
        //    textColor = '#000'
        //}
        //else {
        //    textColor = '#fff'
        //}

        style = 'height:100%;border-radius: 10%;' + gradientColor;
        if (absenceColor != "") style = style + ";border:2px solid " + absenceColor

        if (selDay != null && typeof selDay != 'undefined') {
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && selDay.TelecommutingExpected == true) {
                return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <div class='dx-icon-Cal_TC_Home'></div><span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
            }
            else {
                return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
            }
        }
        else {
            return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
        }
    }

    function changeTelecommutingByRequest(selectedDay, type) {
        var message = "";
        if (type == "office") {
            message = i18nextko.i18n.t('roDoRequestFromHome');
        }
        else {
            message = i18nextko.i18n.t('roDoRequestFromOffice');
        }

        VTPortalUtils.utils.questionMessage(message, 'info', 0, function () {
            new WebServiceRobotics(function (result, type) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    viewShown(selectedDay);

                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roRequestSaved'), 'success', 2000);
                } else {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
                }
            }).changeTelecommutingByRequest(moment(selectedDay).format("YYYY-MM-DD"), type, VTPortal.roApp.impersonatedIDEmployee != -1);
        }, function () {
        }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
    }

    function changeTelecommuting(selectedDay, type) {
        new WebServiceRobotics(function (result, type) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                viewShown(selectedDay);

                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roTCChanged'), 'success', 2000);
            } else {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
            }
        }).changeTelecommuting(moment(selectedDay).format("YYYY/MM/DD"), type, VTPortal.roApp.impersonatedIDEmployee != -1);
    }

    var dataSource = ko.observable(new DevExpress.data.DataSource({
    }));

    function getCurrentOptionalsUsedInWeek(selectedDay, optionals) {
        var iMax = 0;
        var bDate = moment(selectedDay).startOf("week");
        var eDate = moment(selectedDay).endOf("week");

        while (bDate <= eDate) {
            if (optionals.includes(moment(bDate).day().toString())) {
                if (calendarStatus()[moment(bDate).format("YYYYMMDD")].TelecommutingExpected == true) {
                    iMax = iMax + 1;
                }
            }
            bDate = bDate.add(1, 'days')
        }

        return iMax;
    }

    function getCurrentOptionalsUsedInMonth(selectedDay, optionals) {
        var iMax = 0;
        var bDate = moment(selectedDay).startOf("month");
        var eDate = moment(selectedDay).endOf("month");

        while (bDate <= eDate) {
            if (optionals.includes(moment(bDate).day().toString())) {
                if (calendarStatus()[moment(bDate).format("YYYYMMDD")].TelecommutingExpected == true) {
                    iMax = iMax + 1;
                }
            }
            bDate = bDate.add(1, 'days')
        }

        return iMax;
    }

    function generateCalendarSelectorAll() {
        var lstCalendarSelector = [];

        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userStatus().Telecommuting == true && calendarStatus()[moment(selectedDay).format("YYYYMMDD")].CanTelecommute == true) {
            if (moment(selectedDay) >= moment().startOf('day')) {
                var optionals = [];
                var mandatories = [];

                if (calendarStatus()[moment(selectedDay).format("YYYYMMDD")].TelecommutingOptionalDays != "") {
                    optionals = calendarStatus()[moment(selectedDay).format("YYYYMMDD")].TelecommutingOptionalDays.split(',');
                }

                if (calendarStatus()[moment(selectedDay).format("YYYYMMDD")].TelecommutingMandatoryDays != "") {
                    mandatories = calendarStatus()[moment(selectedDay).format("YYYYMMDD")].TelecommutingMandatoryDays.split(',');
                }

                var maxoptionals = calendarStatus()[moment(selectedDay).format("YYYYMMDD")].TelecommutingMaxOptionalDays;
                var periodType = calendarStatus()[moment(selectedDay).format("YYYYMMDD")].TelecommutingPeriodType;

                if (calendarStatus()[moment(selectedDay).format("YYYYMMDD")].TelecommutingExpected == true) {
                    if (mandatories.includes(moment(selectedDay).day().toString())) {
                        if ((CurrentSeating() >= MaxSeating()) && (MaxSeating() > 0)) {
                            lstCalendarSelector.push({
                                ID: 0,
                                cssClass: 'dx-icon-taTelecommuting_OfficeFullCal',
                                cssClassText: 'textActionCalendar',
                                Content: i18nextko.i18n.t('roRequestTelecommutingOffice'),
                                Desc: lblCapacityInfo()
                            });
                        }
                        else {
                            lstCalendarSelector.push({
                                ID: 1,
                                cssClass: 'dx-icon-taTelecommuting_OfficeCal',
                                cssClassText: 'textActionCalendar',
                                Content: i18nextko.i18n.t('roRequestTelecommutingOffice'),
                                Desc: lblCapacityInfo()
                            });
                        }
                    }
                    else {
                        if ((CurrentSeating() >= MaxSeating()) && (MaxSeating() > 0)) {
                            lstCalendarSelector.push({
                                ID: 0,
                                cssClass: 'dx-icon-taTelecommuting_OfficeFullCal',
                                cssClassText: 'textActionCalendar',
                                Content: i18nextko.i18n.t('roRequestTelecommutingOffice'),
                                Desc: lblCapacityInfo()
                            });
                        }
                        else {
                            lstCalendarSelector.push({
                                ID: 8,
                                cssClass: 'dx-icon-taTelecommuting_OfficeCal',
                                cssClassText: 'textActionCalendar',
                                Content: i18nextko.i18n.t('roRequestTelecommutingOffice'),
                                Desc: lblCapacityInfo()
                            });
                        }
                    }
                }

                else {
                    if (optionals.length > 0 && optionals.includes(moment(selectedDay).day().toString())) {
                        var currentOptionalsUsed = 0;

                        if (typeof (periodType) == 'undefined' || periodType == 0)
                            currentOptionalsUsed = getCurrentOptionalsUsedInWeek(selectedDay, optionals);
                        else
                            currentOptionalsUsed = getCurrentOptionalsUsedInMonth(selectedDay, optionals);
                        if (currentOptionalsUsed < maxoptionals) {
                            lstCalendarSelector.push({
                                ID: 2,
                                cssClass: 'dx-icon-taTelecommuting_HomeCal',
                                cssClassText: 'textActionCalendar',
                                Content: i18nextko.i18n.t('roRequestTelecommutingHome'),
                                Desc: lblCapacityInfo()
                            });
                        }
                        else {
                            lstCalendarSelector.push({
                                ID: 7,
                                cssClass: 'dx-icon-taTelecommuting_HomeCal',
                                cssClassText: 'textActionCalendar',
                                Content: i18nextko.i18n.t('roRequestTelecommutingHome'),
                                Desc: lblCapacityInfo()
                            });
                        }
                    }
                    else if (mandatories.length > 0 && mandatories.includes(moment(selectedDay).day().toString())) {
                        lstCalendarSelector.push({
                            ID: 2,
                            cssClass: 'dx-icon-taTelecommuting_HomeCal',
                            cssClassText: 'textActionCalendar',
                            Content: i18nextko.i18n.t('roRequestTelecommutingHome'),
                            Desc: lblCapacityInfo()
                        });
                    }
                    else if (!mandatories.includes(moment(selectedDay).day().toString()) && !optionals.includes(moment(selectedDay).day().toString())) {
                        lstCalendarSelector.push({
                            ID: 7,
                            cssClass: 'dx-icon-taTelecommuting_HomeCal',
                            cssClassText: 'textActionCalendar',
                            Content: i18nextko.i18n.t('roRequestTelecommutingHome'),
                            Desc: lblCapacityInfo()
                        });
                    }
                    else {
                        lstCalendarSelector.push({
                            ID: 7,
                            cssClass: 'dx-icon-taTelecommuting_HomeCal',
                            cssClassText: 'textActionCalendar',
                            Content: i18nextko.i18n.t('roRequestTelecommutingHome'),
                            Desc: lblCapacityInfo()
                        });
                    }
                }
            }
            else {
            }

            if (ZoneVisible() == true) {
                lstCalendarSelector.push({
                    ID: 6,
                    cssClass: 'dx-icon-taTelecommuting_Capacity',
                    cssClassText: 'textActionCalendar',
                    Content: i18nextko.i18n.t('roRequestTelecommutingOfficeWho')
                });
            }
        }
        else {
        }

        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Punch.ScheduleQuery || VTPortal.roApp.empPermissions().Punch.ProductiVQuery))) {
            lstCalendarSelector.push({
                ID: 3,
                cssClass: 'dx-icon-taPunches_DetailCal',
                cssClassText: 'textActionCalendar',
                Content: i18nextko.i18n.t('roPunches_Detail')
            });
        }

        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().CanCreateRequests)) {
            if (VTPortal.roApp.ReadMode() == false) {
                lstCalendarSelector.push({
                    ID: 4,
                    cssClass: 'dx-icon-taNewRequest_DetailCal',
                    cssClassText: 'textActionCalendar',
                    Content: i18nextko.i18n.t('roNewRequest_Detail')
                });
            }
        }

        lstCalendarSelector.push({
            ID: 5,
            cssClass: 'dx-icon-taInformationCal',
            cssClassText: 'textActionCalendar',
            Content: i18nextko.i18n.t('roInformationDay')
        });

        dataSource(new DevExpress.data.DataSource({
            store: lstCalendarSelector
        }));
        return lstCalendarSelector;
    };

    var shiftName = ko.computed(function () {
        if (selectedDayInfo() != null && selectedDayInfo().MainShift != null) {
            return selectedDayInfo().MainShift.Name;
        } else {
            return "";
        }
    });

    var dblRemarkValue = ko.computed(function () {
        if (selectedDayInfo() != null && selectedDayInfo().IncidenceData != null && typeof selectedDayInfo().IncidenceData.NormalWork != 'undefined' && selectedDayInfo().IncidenceData.NormalWork != 0) {
            return selectedDayInfo().IncidenceData.NormalWork;
        } else {
            return 0;
        }
    });
    var onAbsence = ko.computed(function () {
        if (selectedDayInfo() != null && selectedDayInfo().Alerts != null) {
            return selectedDayInfo().Alerts.OnAbsenceDays;
        } else {
            return false;
        }
    });

    var onCause = ko.computed(function () {
        if (selectedDayInfo() != null && selectedDayInfo().Alerts != null) {
            return selectedDayInfo().Alerts.OnAbsenceHours;
        } else {
            return false;
        }
    });

    var onHolidays = ko.computed(function () {
        if (selectedDayInfo() != null && selectedDayInfo().Alerts != null) {
            return selectedDayInfo().Alerts.OnHolidays;
        } else {
            return false;
        }
    });

    var calCell = ko.observable('#calendarContainer')

    //var onCellClicked = function (selDate) {
    //    selectedDay = moment(selDate,"YYYYMMDD").toDate();
    //    calCell('#' + selDate);
    //    popoverVisible(true);
    //};

    var onCellClicked = function (e) {
        selectedDay = e.value;
        calCell('#' + moment(e.value).format("YYYYMMDD"));

        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userStatus().Telecommuting == true) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    CurrentSeating(parseInt(result.capacities.CurrentSeating));
                    MaxSeating(parseInt(result.capacities.MaxSeatingCapacity));

                    if (typeof result.capacities.ZoneCapacityVisible == 'undefined') {
                        ZoneVisible(true);
                    }
                    else {
                        ZoneVisible(result.capacities.ZoneCapacityVisible);
                    }

                    if (result.capacities.MaxSeatingCapacity > 0) {
                        capacityInfo(result.capacities.CurrentSeating + '/' + result.capacities.MaxSeatingCapacity)
                    }
                    else
                        capacityInfo('');

                    selectedDayDetail(result.oCalendar);
                    $.when(generateCalendarSelectorAll()).then(popoverVisible(true));
                } else {
                    CurrentSeating(0);
                    MaxSeating(0);
                    selectedDayDetail([]);
                    var onContinue = function () {
                        VTPortal.roApp.loadInitialData(false, false, true, false, false);
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            }, function () { }).getLoadSeatingCapacity(selectedDay);
        }
        else {
            $.when(generateCalendarSelectorAll()).then(popoverVisible(true));
        }
    };

    var viewModel = {
        onCellClicked: onCellClicked,
        modelIsReady: modelIsReady,
        initializeView: viewShown,
        hasPermission: hasPermission,
        calendarOptions: {
            width: '100%',
            useCellTemplate: true,
            height: 450,
            cellTemplate: getCellTemplate,
            firstDayOfWeek: moment()._locale._week.dow,
            zoomLevel: 'month',
            maxZoomLevel: 'month',
            minZoomLevel: 'month',
            //onValueChanged: onCellClicked,
            onCellClick: onCellClicked,
            showTodayButton: false
        },
        calendarCellPopover: {
            target: calCell,
            position: "top",
            width: 300,
            shading: true,
            shadingColor: "rgba(0, 0, 0, 0.5)",
            visible: popoverVisible
        },

        dayToolbarActions: {
            dataSource: dataSource,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'actionItem',
            onItemClick: function (info) {
                switch (info.itemData.ID) {
                    case 0:
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roTCFull'), 'error', 2000);
                        popoverVisible(false);
                        break;
                    case 1:
                        popoverVisible(false);
                        changeTelecommutingByRequest(selectedDay, 'home');
                        break;
                    case 2:
                        popoverVisible(false);
                        changeTelecommuting(selectedDay, 'office');
                        break;
                    case 7:
                        popoverVisible(false);
                        changeTelecommutingByRequest(selectedDay, 'office');
                        break;
                    case 8:
                        popoverVisible(false);
                        changeTelecommuting(selectedDay, 'home');
                        break;
                    case 3:
                        popoverVisible(false);
                        VTPortal.app.navigate("punchManagement/1/" + moment(selectedDay).format("YYYY-MM-DD"));
                        break;
                    case 4:
                        popoverVisible(false);
                        VTPortal.app.navigate('requestsList/5,6,7,8,9,11,13,14/' + moment(selectedDay).format("YYYY-MM-DD"));
                        break;
                    case 5:
                        popoverVisible(false);
                        VTPortal.app.navigate('dayInfo/' + moment(selectedDay).format("YYYY-MM-DD"));
                        break;
                    case 6:
                        popoverVisible(false);
                        VTPortal.app.navigate('capacityDetail/' + moment(selectedDay).format("YYYY-MM-DD"));
                        break;
                }
            }
        },
        capacityInfo: capacityInfo,
        lblCapacityInfo: lblCapacityInfo,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    }

    return viewModel;
};