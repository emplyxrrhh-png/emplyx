VTPortal.myCalendar = function (params) {
    var noPermissions = VTPortal.noPermissions();
    var startDateViewPeriod = ko.observable(moment().startOf('month').toDate());
    var calendarStatus = ko.observable({});
    var selectedDayInfo = ko.observable(null);
    var selectedDay = moment().toDate();
    var popoverVisible = ko.observable(false);
    var modelIsReady = ko.observable(false);
    var dayToolbarActionsCalculated = ko.observable([]);
    var capacityInfo = ko.observable('');
    var CurrentSeating = ko.observable(0);
    var MaxSeating = ko.observable(0);
    var ZoneVisible = ko.observable(false);
    var selectedDayDetail = ko.observable(null);
    var tcInfo = ko.observable(null);
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

    var lblCurrentUsage = ko.observable('');

    var tcCurrentPercentage = ko.computed(function () {
        if (tcInfo() != null) {
            if (tcInfo().ByPercentage == true) {
                switch (tcInfo().Period) {
                    case 0:
                        return i18nextko.i18n.t('tcPercentageUsedDayInfo') + ' ' + tcInfo().CurrentAgreementPercentageUsed + i18nextko.i18n.t('tcPercentageUsedDayInfo2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxPercentage + '%/' + i18nextko.i18n.t('semana') + ')';
                        break;
                    case 1:
                        return i18nextko.i18n.t('tcPercentageUsedDayInfo') + ' ' + tcInfo().CurrentAgreementPercentageUsed + i18nextko.i18n.t('tcPercentageUsedDayInfo2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxPercentage + '%/' + i18nextko.i18n.t('mes') + ')';
                        break;
                    case 2:
                        return i18nextko.i18n.t('tcPercentageUsedDayInfo') + ' ' + tcInfo().CurrentAgreementPercentageUsed + i18nextko.i18n.t('tcPercentageUsedDayInfo2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxPercentage + '%/' + i18nextko.i18n.t('trimestre') + ')';
                        break;
                }
            }
            else {
                switch (tcInfo().Period) {
                    case 0:
                        return i18nextko.i18n.t('tcDaysUsedDayInfo') + ' ' + tcInfo().CurrentAgreementDaysUsed + ' ' + i18nextko.i18n.t('tcDaysUsedDayInfo2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxDays + 'd/' + i18nextko.i18n.t('semana') + ')';
                        break;
                    case 1:
                        return i18nextko.i18n.t('tcDaysUsedDayInfo') + ' ' + tcInfo().CurrentAgreementDaysUsed + ' ' + i18nextko.i18n.t('tcDaysUsedDayInfo2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxDays + 'd/' + i18nextko.i18n.t('mes') + ')';
                        break;
                    case 2:
                        return i18nextko.i18n.t('tcDaysUsedDayInfo') + ' ' + tcInfo().CurrentAgreementDaysUsed + ' ' + i18nextko.i18n.t('tcDaysUsedDayInfo2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxDays + 'd/' + i18nextko.i18n.t('trimestre') + ')';
                        break;
                }
            };
        }
        else {
            return '';
        }
    })

    function viewShown(sDate) {
        var queryDate = startDateViewPeriod();

        if (typeof (sDate) != 'undefined' && sDate != null) queryDate = sDate;

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
                // $('#scrollview').height($('#panelsContent').height() - 20);
            }, function () { }).getMyCalendar(queryDate);
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
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && typeof (selDay.CanTelecommute) != 'undefined' && selDay.CanTelecommute) {
                //if ( (selDay.TelecommuteForced || ( && selDay.TelecommutingExpected == true)))

                if (typeof selDay.TelecommuteForced != 'undefined' && selDay.TelecommuteForced) {
                    if ((typeof (selDay.MainShift) != 'undefined' && selDay.MainShift != null) && selDay.MainShift.PlannedHours > 0 && !selDay.Alerts.OnAbsenceDays && !selDay.Alerts.OnHolidays) {
                        if (selDay.TelecommutingExpected == true) {
                            return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <div class='dx-icon-Cal_TC_Home'></div><span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
                        } else if (selDay.TelecommutingOptional == true) {
                            return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <div class='dx-icon-Cal_TC_Optional'></div><span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
                        } else {
                            return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <div class='dx-icon-Cal_TC_Presence'></div><span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
                        }
                    } else {
                        //Aquest cas no es pot donar mai, pero el deixem buit per si de cas
                        return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
                    }
                } else {
                    if ((typeof (selDay.MainShift) != 'undefined' && selDay.MainShift != null) && selDay.MainShift.PlannedHours > 0 && !selDay.Alerts.OnAbsenceDays && !selDay.Alerts.OnHolidays) {
                        if (selDay.TelecommutingOptionalDays != null && selDay.TelecommutingOptionalDays.indexOf(moment(data.date).day().toString()) >= 0) {
                            return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <div class='dx-icon-Cal_TC_Optional'></div><span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
                        } else if (selDay.PresenceMandatoryDays != null && selDay.PresenceMandatoryDays.indexOf(moment(data.date).day().toString()) >= 0) {
                            return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <div class='dx-icon-Cal_TC_Presence'></div><span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
                        } else if (selDay.TelecommutingMandatoryDays != null && selDay.TelecommutingMandatoryDays.indexOf(moment(data.date).day().toString()) >= 0) {
                            return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <div class='dx-icon-Cal_TC_Home'></div><span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
                        } else {
                            return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'><span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
                        }
                    } else {
                        return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
                    }
                }
            } else {
                return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
            }
        }
        else {
            return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
        }
    }

    function checkTelecommutingChange(selectedDay, type) {
        var typeRequested = type;
        new WebServiceRobotics(function (result, type) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                if (result.NeedRequest == false) {
                    viewShown(selectedDay);
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roTCChanged'), 'success', 2000);
                }
                else {
                    changeTelecommutingByRequest(selectedDay, typeRequested);
                }
            } else {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
            }
        }).checkTelecommutingChange(moment(selectedDay).format("YYYY/MM/DD"), type, VTPortal.roApp.impersonatedIDEmployee != -1);
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

    var getTCInfo = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                tcInfo(result);
            } else {
                DevExpress.ui.notify(i18nextko.t('roTCError'), 'warning', 3000);
            }
        }).getTelecommutingInfo(moment(selectedDay));
    }

    function generateCalendarSelectorAll() {
        var lstCalendarSelector = [];

        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userTelecommute() != null && VTPortal.roApp.userTelecommute().Telecommuting && calendarStatus()[moment(selectedDay).format("YYYYMMDD")].CanTelecommute == true) {
            if (moment(selectedDay) >= moment().startOf('day')) {
                if (calendarStatus()[moment(selectedDay).format("YYYYMMDD")].TelecommutingExpected == true) {
                    if ((CurrentSeating() >= MaxSeating()) && (MaxSeating() > 0)) {
                        lstCalendarSelector.push({
                            ID: 0,
                            cssClass: 'dx-icon-taTelecommuting_OfficeFullCal',
                            cssClassText: 'textActionCalendar',
                            Content: i18nextko.i18n.t('roRequestTelecommutingOffice'),
                            Desc: lblCapacityInfo(),
                            CurrentUsage: ''
                        });
                    }
                    else {
                        lstCalendarSelector.push({
                            ID: 1,
                            cssClass: 'dx-icon-taTelecommuting_OfficeCal',
                            cssClassText: 'textActionCalendar',
                            Content: i18nextko.i18n.t('roRequestTelecommutingOffice'),
                            Desc: lblCapacityInfo(),
                            CurrentUsage: ''
                        });
                    }
                }

                else {
                    lstCalendarSelector.push({
                        ID: 2,
                        cssClass: 'dx-icon-taTelecommuting_HomeCal',
                        cssClassText: 'textActionCalendar',
                        Content: i18nextko.i18n.t('roRequestTelecommutingHome'),
                        Desc: lblCapacityInfo(),
                        CurrentUsage: tcCurrentPercentage()
                    });
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

    var onOptionChanged = function (e) {
        if (e.fullName == "currentDate") {
            if (moment(e.value).month() != moment(e.previousValue).month()) {
                startDateViewPeriod(moment(e.value).startOf('month').toDate());

                var bDayLoaded = calendarStatus()[moment.tz(startDateViewPeriod(), VTPortal.roApp.serverTimeZone).add(12, 'hours').format("YYYYMMDD")];
                if (typeof (bDayLoaded) == "undefined" || bDayLoaded == null) {
                    viewShown();
                }
            }
        }
    }

    var onCellClicked = function (e) {
        selectedDay = e.value;
        calCell('#' + moment(e.value).format("YYYYMMDD"));

        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userTelecommute() != null && VTPortal.roApp.userTelecommute().Telecommuting) {
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

                    if (calendarStatus()[moment(selectedDay).format("YYYYMMDD")].TelecommutingExpected == true) {
                        $.when(generateCalendarSelectorAll()).then(popoverVisible(true));
                    }
                    else {
                        new WebServiceRobotics(function (result) {
                            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                                tcInfo(result);
                                $.when(generateCalendarSelectorAll()).then(popoverVisible(true));
                            } else {
                                $.when(generateCalendarSelectorAll()).then(popoverVisible(true));
                            }
                        }).getTelecommutingInfo(moment(selectedDay));
                    }
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
        noPermissions: noPermissions,
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
            onOptionChanged: onOptionChanged,
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
                        checkTelecommutingChange(selectedDay, 'home');
                        break;
                    case 2:
                        popoverVisible(false);
                        checkTelecommutingChange(selectedDay, 'office');
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