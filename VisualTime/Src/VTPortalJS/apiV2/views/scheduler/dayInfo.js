VTPortal.dayInfo = function (params) {
    var scheduleTitle = i18nextko.i18n.t('roDayDetail');

    var selectedDate = ko.observable(new Date());
    var dayInfo = ko.observable(null);
    var forecastsDS = ko.observable([{ key: 'forecasts', items: [] }]);
    var scheduleDS = ko.observable([{ key: 'forecasts', items: [] }]);
    var productivDS = ko.observable([{ key: 'forecasts', items: [] }]);
    var shiftLayersDS = ko.observable([]);
    var selectedForecast = null;
    var buttonEditVisible = ko.observable(false);
    var buttonDeleteVisible = ko.observable(false);

    var swipeLeft = function () {
        selectedDate(moment(selectedDate()).subtract(1, 'days'));
    }

    var swipeRight = function () {
        selectedDate(moment(selectedDate()).add(1, 'days'));
    }

    var myEventHandler = function (model, e) {
        var res = 'middle';
        var offset = e.targetOffset;
        if (offset < 0) swipeRight();
        else if (offset > 0) swipeLeft();
    }

    var refreshData = function (e) {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.CauseNote) {
            var callbackFuntion = function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        if (result.DayInfo != null && result.DayInfo.DayData.length > 0) dayInfo(result.DayInfo.DayData[0]);

                        var forecasts = result.Forecasts;
                        for (var i = 0; i < forecasts.length; i++) {
                            switch (forecasts[i].ForecastType) {
                                case 'days':
                                    forecasts[i].cssClass = 'dx-icon-plannedAbsences';
                                    break;
                                case 'hours':
                                    forecasts[i].cssClass = 'dx-icon-plannedCauses';
                                    break;
                                case 'holidayhours':
                                    forecasts[i].cssClass = 'dx-icon-plannedHoliday';
                                    break;
                                case 'overtime':
                                    forecasts[i].cssClass = 'dx-icon-plannedOvertime';

                                    break;
                            }

                            forecasts[i].hasAction = forecasts[i].HasDocuments;

                            if (typeof forecasts[i].ForecastDetail != 'undefined' && forecasts[i].ForecastDetail != "" && forecasts[i].ForecastDetail != null) {
                                forecasts[i].Description = i18nextko.i18n.t('roLeavesFrom') + ' ' + moment(forecasts[i].BeginDate).format('L') + ' ' + i18nextko.i18n.t('roLeavesCause') + ' ' + forecasts[i].Cause + ' (' + forecasts[i].ForecastDetail + ')';
                            }
                            else {
                                forecasts[i].Description = i18nextko.i18n.t('roLeavesFrom') + ' ' + moment(forecasts[i].BeginDate).format('L') + ' ' + i18nextko.i18n.t('roLeavesCause') + ' ' + forecasts[i].Cause;
                            }

                            if (forecasts[i].DocAlerts != null && forecasts[i].DocAlerts.length > 0) {
                                forecasts[i].AlertDescription = i18nextko.i18n.t('roLeavesRequiereDocuments');
                            } else {
                                forecasts[i].AlertDescription = i18nextko.i18n.t('roLeavesNoAlerts');
                            }
                        }

                        var ds = [{ key: 'forecasts', items: forecasts }]

                        forecastsDS(ds);

                        refreshShiftLayers(result);

                        refreshAccruals();
                } else {
                    if (VTPortalUtils.utils.isOnView("dayInfo")) {
                        dayInfo(null);
                        forecastsDS([{ key: 'forecasts', items: [] }]);
                        var onContinue = function () {
                            VTPortal.roApp.loadInitialData(false, false, true, false, false);
                            VTPortal.roApp.redirectAtHome();
                        }
                        if (VTPortalUtils.utils.isOnView("dayInfo")) VTPortalUtils.utils.processErrorMessage(result, onContinue);
                    }
                }
            };

            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getEmployeeDayInfo(selectedDate(), -1);
        } else {
            var onContinue = function () {
                VTPortal.roApp.loadInitialData(false, false, true, false, false);
                VTPortal.roApp.redirectAtHome();
            }

            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roIncorrectApiVersion'), 'error', 0, onContinue);
        }
    }

    var refreshShiftLayers = function (result) {
        var layerItems = [];

        var mmt = moment();

        // Your moment at midnight
        var mmtMidnight = mmt.clone().startOf('day');

        if (typeof result.DayInfo != 'undefined' && result.DayInfo != null && result.DayInfo.DayData.length > 0 && result.DayInfo.DayData[0].MainShift != null) {
            var selShift = result.DayInfo.DayData[0].MainShift;

            if (selShift.ShiftLayers == 0) {
                var startHour = moment.tz(selShift.StartHour, VTPortal.roApp.serverTimeZone);
                var endHour = moment.tz(selShift.EndHour, VTPortal.roApp.serverTimeZone);

                mmtMidnight = moment(startHour).endOf('day');
                var iHourValue = moment(startHour).diff(mmtMidnight, 'minutes') - 1;

                var shiftDuration;
                if (moment(endHour) >= moment(startHour)) {
                    shiftDuration = moment(endHour).diff(moment(startHour), 'minutes')
                } else {
                    shiftDuration = moment(endHour).add(1, 'day').diff(moment(startHour), 'minutes')
                }

                layerItems.push({
                    Name: '',
                    Gauge: {
                        scale: {
                            startValue: iHourValue,
                            endValue: iHourValue + shiftDuration,
                            tickInterval: 60,
                            label: { indentFromTick: -3, true: false, customizeText: VTPortalUtils.utils.convertShiftHourToTime(iHourValue + shiftDuration) },
                            tick: { color: "#536878" }
                        },
                        value: 0,
                        subvalues: [iHourValue, (iHourValue + shiftDuration)],
                        subvalueIndicator: {
                            type: "textCloud",
                            color: "#0046FE",
                            text: {
                                font: { size: 12 },
                                customizeText: VTPortalUtils.utils.convertShiftHourToTime(iHourValue + shiftDuration)
                            }
                        },
                        rangeContainer: {
                            offset: 10,
                            ranges: [
                                { startValue: iHourValue, endValue: (iHourValue + shiftDuration), color: "#77DD77" }
                            ]
                        }
                    }
                });
            } else {
                for (var i = 0; i < selShift.ShiftLayers; i++) {
                    var tmpLayerDef = selShift.ShiftLayersDefinition[i];

                    // tmpLayerDef.LayerStartTime = moment.tz(tmpLayerDef.LayerStartTime, VTPortal.roApp.serverTimeZone).toDate();
                    //moment(moment().format())._tzm

                    var specificMoment = moment.tz(moment(tmpLayerDef.LayerStartTime), VTPortal.roApp.serverTimeZone);

                    var strDate = specificMoment.format("YYYYMMDDHHmmss");

                    mmtMidnight = moment(strDate, "YYYYMMDDHHmmss").endOf('day');

                    var iHourValue = moment(strDate, "YYYYMMDDHHmmss").diff(mmtMidnight, 'minutes') - 1;

                    layerItems.push({
                        Name: i18nextko.i18n.t('roLayer') + " " + tmpLayerDef.LayerID,
                        Gauge: {
                            scale: {
                                startValue: iHourValue,
                                endValue: iHourValue + tmpLayerDef.LayerDuration,
                                tickInterval: 60,
                                label: { indentFromTick: -3, visible: false, customizeText: VTPortalUtils.utils.convertShiftHourToTime(iHourValue + tmpLayerDef.LayerDuration) },
                                tick: { color: "#536878" }
                            },
                            value: 0,
                            subvalues: [iHourValue, (iHourValue + tmpLayerDef.LayerOrdinaryHours), (iHourValue + tmpLayerDef.LayerDuration)],
                            subvalueIndicator: {
                                type: "textCloud",
                                color: "#0046FE",
                                text: {
                                    font: { size: 12 },
                                    customizeText: VTPortalUtils.utils.convertShiftHourToTime(iHourValue + selShift.PlannedHours)
                                }
                            },
                            rangeContainer: {
                                offset: 10,
                                ranges: [
                                    { startValue: iHourValue, endValue: (iHourValue + tmpLayerDef.LayerOrdinaryHours), color: "#77DD77" },
                                    { startValue: (iHourValue + tmpLayerDef.LayerOrdinaryHours), endValue: (iHourValue + tmpLayerDef.LayerDuration), color: "#ff0000" }
                                ]
                            }
                        }
                    });
                }
            }

            shiftLayersDS([{ key: 'Franjas', items: layerItems }]);
        } else {
            shiftLayersDS([]);
        }
    }

    var refreshAccruals = function () {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Holidays) {
            if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals || VTPortal.roApp.empPermissions().Schedule.ProductivAccruals))) {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                            //Solo si tenemos permisos para consultar saldos de presencia
                            if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals)) {
                                var summaryDS = []
                                var dailySummaryDS = []
                                var psummaryDS = []

                                var productivSummary = result.ProductiVSummary;
                                if (productivSummary.DailyAccruals.length > 0) psummaryDS = psummaryDS.concat(VTPortalUtils.utils.generateAccrualsArrayProductiv(productivSummary.DailyAccruals));

                                var accrualsSummary = result.ScheduleSummary;
                                if (accrualsSummary.DailyAccruals.length > 0) dailySummaryDS = dailySummaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.DailyAccruals));
                                if (accrualsSummary.WeekAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.WeekAccruals));
                                if (accrualsSummary.MonthAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.MonthAccruals));
                                if (accrualsSummary.YearAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.YearAccruals));
                                if (accrualsSummary.ContractAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.ContractAccruals));
                                if (accrualsSummary.YearWorkAccruals && accrualsSummary.YearWorkAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.YearWorkAccruals));

                                for (var i = 0; i < summaryDS.length; i++) {
                                    summaryDS[i].Name = summaryDS[i].Name + ' ' + (summaryDS[i].YearWorkPeriod || '');
                                }

                                var dsP = [];

                                var ds = [];

                                if (psummaryDS.length == 0) dsP.push({ key: 'forecasts', items: [] });
                                else {
                                    if (psummaryDS.length > 0) {
                                        dsP.push({ key: 'Daily', items: psummaryDS.sortBy(function (n) { return n.Name }) });
                                    }
                                }

                                if (summaryDS.length == 0 && dailySummaryDS.length == 0) ds.push({ key: 'forecasts', items: [] });
                                else {
                                    if (dailySummaryDS.length > 0) {
                                        ds.push({ key: 'Daily', items: dailySummaryDS.sortBy(function (n) { return n.Name }) });
                                    }
                                    if (summaryDS.length > 0) {
                                        ds.push({ key: 'Total', items: summaryDS.sortBy(function (n) { return n.Name }) });
                                    }
                                }
                                productivDS(dsP);
                                scheduleDS(ds);
                            }

                            if (scheduleDS()[0].items.length > 0) {
                                for (var i = 0; i < scheduleDS().length; i++) {
                                    if (typeof $("#listSchedule").dxList("instance") !== "undefined" ) $("#listSchedule").dxList("instance").collapseGroup(i);
                                }
                            }

                            if (productivDS()[0].items.length > 0) {
                                for (var i = 0; i < productivDS().length; i++) {
                                    if (typeof $("#listProductiv").dxList("instance") !== "undefined") $("#listProductiv").dxList("instance").collapseGroup(i);
                                }
                            }

                            $('#scrollview').height($('#panelsContent').height() - 70);

                    } else {
                        if (VTPortalUtils.utils.isOnView("dayInfo")) {
                            scheduleDS(ko.observable([{ key: 'forecasts', items: [] }]));
                            productivDS(ko.observable([{ key: 'forecasts', items: [] }]));

                            var onContinue = function () {
                                VTPortal.roApp.loadInitialData(false, false, true, false, false);
                                VTPortal.roApp.redirectAtHome();
                            }
                            VTPortalUtils.utils.processErrorMessage(result, onContinue);
                        }
                    }
                }).getAccrualsSummary(selectedDate());
            }
        } else {
            var onContinue = function () {
            }

            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roIncorrectApiVersion'), 'error', 0, onContinue);
        }
    }
    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function setUpView() {
        globalStatus().viewShown();
        var selDate = moment(params.id, "YYYY-MM-DD");
        selectedDate(selDate.toDate());
    };

    var shiftName = ko.computed(function () {
        if (dayInfo() != null && dayInfo().MainShift != null) {
            return dayInfo().MainShift.Name;
        } else {
            return "";
        }
    });

    function convertMinsToHrsMins(minutes) {
        var h = Math.floor(minutes / 60);
        var m = minutes % 60;
        h = h < 10 ? '0' + h : h;
        m = m < 10 ? '0' + m : m;
        return h + ':' + m;
    }

    var plannedHours = ko.computed(function () {
        if (dayInfo() != null && dayInfo().MainShift != null) {
            return i18nextko.i18n.t("roPlannedLbl") + ': ' + convertMinsToHrsMins(dayInfo().MainShift.PlannedHours);
        } else {
            return "";
        }
    });

    var dblRemarkValue = ko.computed(function () {
        if (dayInfo() != null && dayInfo().IncidenceData != null && typeof dayInfo().IncidenceData.NormalWork != 'undefined' && dayInfo().IncidenceData.NormalWork != 0) {
            return dayInfo().IncidenceData.NormalWork;
        } else {
            return 0;
        }
    });

    var onPlannedAbsence = ko.computed(function () {
        if (dayInfo() != null && dayInfo().Alerts != null) {
            return dayInfo().Alerts.OnAbsenceDays;
        } else {
            return false;
        }
    });

    var onPlannedCause = ko.computed(function () {
        if (dayInfo() != null && dayInfo().Alerts != null) {
            return dayInfo().Alerts.OnAbsenceHours;
        } else {
            return false;
        }
    });

    var onPlannedHoliday = ko.computed(function () {
        if (dayInfo() != null && dayInfo().Alerts != null) {
            return dayInfo().Alerts.OnHolidaysHours;
        } else {
            return false;
        }
    });

    var onPlannedOvertime = ko.computed(function () {
        if (dayInfo() != null && dayInfo().Alerts != null) {
            return dayInfo().Alerts.OnOvertimesHours;
        } else {
            return false;
        }
    });

    var onHolidays = ko.computed(function () {
        if (dayInfo() != null && dayInfo().Alerts != null) {
            return dayInfo().Alerts.OnHolidays;
        } else {
            return false;
        }
    });
    var onBtnEditRequest = function (e) {
        VTPortal.app.navigate('sendLeaveDocument/-1/' + selectedForecast.itemData.ForecastID + "/" + selectedForecast.itemData.IdCause + "/" + selectedForecast.itemData.ForecastType);
    }

    var onBtnDeleteRequest = function (e) {
        deleteProgrammedAbsence(selectedForecast.itemData.ForecastID, selectedForecast.itemData.ForecastType);
    }
    var wsRobotics = null;
    function deleteProgrammedAbsence(forecastID, forecastType) {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorDeleteProgrammedAbsence"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtHome();
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roDeleteProgrammedAbsenceSaved"), 'success', 2000);
            } else {
                var onContinue = function () {
                    // wsRobotics.deleteProgrammedAbsence(forecastID, forecastType);
                }

                VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        wsRobotics.deleteProgrammedAbsence(forecastID, forecastType);
    }

    var viewModel = {
        viewShown: setUpView,
        title: scheduleTitle,
        subscribeBlock: globalStatus(),
        forecastsDS: forecastsDS,
        listForecasts: {
            dataSource: forecastsDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: false,
            itemTemplate: 'ForecastItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roDailyForecasts') + "</div>");
            },
            onItemClick: function (info) {
                selectedForecast = info;
                if (info.itemData.hasAction) {
                    buttonEditVisible(true);
                }
                else {
                    buttonEditVisible(false);
                }

                if (VTPortal.roApp.empPermissions().Forecast == true) {
                    buttonDeleteVisible(true);
                }
                else {
                    buttonDeleteVisible(false);
                }

                //
                //if (info.itemData.hasAction) {
                //    VTPortal.app.navigate('sendLeaveDocument/-1/' + info.itemData.ForecastID + "/" + info.itemData.IdCause + "/" + info.itemData.ForecastType);
                //}
            }
        },
        headerDate: {
            value: selectedDate,
            pickerType: 'rollers',
            displayFormat: 'shortDate',
            onValueChanged: refreshData
        },
        btnPrevious: {
            icon: 'chevronprev',
            onClick: swipeLeft
        },
        btnNext: {
            icon: 'chevronnext',
            onClick: swipeRight
        },
        myEventHandler: myEventHandler,
        lblShift: i18nextko.t('roShiftLbl'),
        lblAccrual: i18nextko.t('roAccrualLbl'),
        txtDayShift: {
            placeholder: i18nextko.t('roShiftLbl'),
            value: shiftName,
            readOnly: true
        },
        txtPlannedHours: {
            placeholder: i18nextko.t('roPlannedLbl'),
            value: plannedHours,
            readOnly: true
        },
        remarkValue: dblRemarkValue,
        txtRemarkValue: {
            placeholder: i18nextko.t('roAccrualLbl'),
            value: dblRemarkValue,
            readOnly: true
        },
        onHolidays: onHolidays,
        onPlannedAbsence: onPlannedAbsence,
        onPlannedCause: onPlannedCause,
        onPlannedHoliday: onPlannedHoliday,
        onPlannedOvertime: onPlannedOvertime,
        scheduleDS: scheduleDS,
        productivDS: productivDS,
        listSchedule: {
            dataSource: scheduleDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: true,
            itemTemplate: 'ScheduleItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roResumeAccruals_' + data.key) + "</div>");
            }
        },
        listProductiv: {
            dataSource: productivDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: true,
            itemTemplate: 'ScheduleItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roPResumeAccruals_' + data.key) + "</div>");
            }
        },
        listShiftLayer: {
            dataSource: shiftLayersDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: true,
            itemTemplate: 'ScheduleItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roResumeAccruals_' + data.key) + "</div>");
            }
        },

        btnDeleteRequest: {
            visible: buttonDeleteVisible,
            onClick: onBtnDeleteRequest,
            text: '',
            icon: "Images/Common/delete.png",
        },
        btnEditRequest: {
            visible: buttonEditVisible,
            onClick: onBtnEditRequest,
            text: '',
            icon: "Images/Common/plus.png",
        },
    };

    return viewModel;
};