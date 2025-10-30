VTPortal.accrualsHome = function (params) {
    var selectedDate = ko.observable(moment().add(-1, 'days').toDate());
    var modelIsReady = ko.observable(false);
    var productivDS = ko.observable([]);
    var scheduleDS = ko.observable([]);
    var holidaysDS = ko.observable([]);

    var hasPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals || VTPortal.roApp.empPermissions().Schedule.ProductivAccruals))) {
            return true;
        } else {
            return false;
        }
    });

    var refreshAccruals = function () {
        selectedDate(moment().add(-1, 'days').toDate());

        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Holidays) {
            if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals || VTPortal.roApp.empPermissions().Schedule.ProductivAccruals || VTPortal.roApp.empPermissions().Schedule.QuerySchedule))) {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        VTPortal.roApp.lastOkAccruals(new Date());

                        if (VTPortal.roApp.impersonatedIDEmployee != -1) {
                            VTPortal.roApp.lastOkAccrualsEmployeeId(VTPortal.roApp.impersonatedIDEmployee);
                        }
                        else {
                            VTPortal.roApp.lastOkAccrualsEmployeeId(VTPortal.roApp.employeeId);
                        }

                        //Solo si tenemos permisos para consultar saldos de presencia
                        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals)) {
                            var summaryDS = [];

                            var accrualsSummary = result.ScheduleSummary;
                            //if (accrualsSummary.DailyAccruals.length > 0) dailySummaryDS = dailySummaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.DailyAccruals));
                            if (accrualsSummary.WeekAccruals.length > 0) summaryDS = summaryDS.concat(accrualsSummary.WeekAccruals);
                            if (accrualsSummary.MonthAccruals.length > 0) summaryDS = summaryDS.concat(accrualsSummary.MonthAccruals);
                            if (accrualsSummary.YearAccruals.length > 0) summaryDS = summaryDS.concat(accrualsSummary.YearAccruals);
                            if (accrualsSummary.ContractAccruals.length > 0) summaryDS = summaryDS.concat(accrualsSummary.ContractAccruals);

                            for (var i = 0; i < summaryDS.length; i++) {
                                summaryDS[i].Name = summaryDS[i].Name + ': ' + summaryDS[i].TotalFormat;
                            }
                            summaryDS = summaryDS.sortBy(function (n) { return n.Name });
                            scheduleDS(summaryDS);
                            VTPortal.roApp.scheduleDS(summaryDS);
                        }

                        //Solo si tenemos permisos para consultar vacaciones
                        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.QuerySchedule)) {
                            var summaryDS = []
                            var accrualsSummary = result.HolidaysSummary;

                            if (accrualsSummary.HolidaysInfo.length > 0) {
                                summaryDS = summaryDS.concat(accrualsSummary.HolidaysInfo);

                                for (var i = 0; i < summaryDS.length; i++) {
                                    if (summaryDS[i].IDShift == -1) {
                                        summaryDS[i].Pending = i18nextko.i18n.t('roHolidayResume_Pending') + ': ' + summaryDS[i].Pending;
                                        summaryDS[i].Lasting = i18nextko.i18n.t('roHolidayResume_Lasting') + ': ' + summaryDS[i].Lasting;
                                        summaryDS[i].Available = i18nextko.i18n.t('roHolidayResume_Available') + ': ' + summaryDS[i].Available;
                                        summaryDS[i].Prevision = i18nextko.i18n.t('roHolidayResume_Prevision') + ': ' + summaryDS[i].Prevision;
                                        if (summaryDS[i].Done > 0) {
                                            summaryDS[i].Done = i18nextko.i18n.t('roHolidayResume_Done') + ': ' + summaryDS[i].Done;
                                        }
                                        else {
                                            summaryDS[i].Done = "";
                                        }
                                    }
                                    else {
                                        summaryDS[i].Pending = i18nextko.i18n.t('roHolidayResume_Pending') + ': ' + summaryDS[i].Pending;
                                        summaryDS[i].Lasting = i18nextko.i18n.t('roHolidayResume_Lasting') + ': ' + summaryDS[i].Lasting;
                                        summaryDS[i].Available = i18nextko.i18n.t('roHolidayResume_Available') + ': ' + summaryDS[i].Available;
                                        summaryDS[i].Prevision = i18nextko.i18n.t('roHolidayResume_Prevision') + ': ' + summaryDS[i].Prevision;
                                        summaryDS[i].Done = i18nextko.i18n.t('roHolidayResume_Done') + ': ' + summaryDS[i].Done;
                                    }
                                }
                            }

                            if (summaryDS.length > 0) {
                                holidaysDS(accrualsSummary.HolidaysInfo);
                                VTPortal.roApp.holidaysDS(accrualsSummary.HolidaysInfo);
                            } else {
                                holidaysDS([])
                            }
                        }
                    } else {
                        productivDS([]);
                        scheduleDS([]);
                        holidaysDS([]);
                        var onContinue = function () {
                            VTPortal.roApp.loadInitialData(false, false, true, false, false);
                            VTPortal.roApp.redirectAtHome();
                        }
                        VTPortalUtils.utils.processErrorMessage(result, onContinue);
                    }
                }).getAccrualsSummary(selectedDate());
            }
        } else {
            var onContinue = function () {
            }

            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roIncorrectApiVersion'), 'error', 0, onContinue);
        }
    }

    var getAccruals = function () {
        if (VTPortal.roApp.lastOkAccruals() == null || (VTPortal.roApp.lastOkAccrualsEmployeeId() != VTPortal.roApp.employeeId) || VTPortal.roApp.impersonatedIDEmployee != -1) {
            refreshAccruals();
        }
        else {
            var now = new Date();
            var lastDay = VTPortal.roApp.lastOkAccruals().getDate();
            //var diffMs = (now - VTPortal.roApp.lastOkAccruals());
            //var diffMins = Math.round(((diffMs % 86400000) % 3600000) / 60000);

            if (now.getDate() != lastDay) {
                refreshAccruals();
            }
            else {
                if (VTPortal.roApp.scheduleDS() != null) {
                    scheduleDS(VTPortal.roApp.scheduleDS());
                }
                else {
                    scheduleDS([]);
                }
                if (VTPortal.roApp.holidaysDS() != null) {
                    holidaysDS(VTPortal.roApp.holidaysDS());
                }
                else {
                    holidaysDS([]);
                }
            }
        }
    }

    var accrualsTitle = ko.computed(function () {
        return i18nextko.i18n.t('accrualsTitle');
    });
    var noAccrualsDesc = ko.computed(function () {
        return i18nextko.i18n.t('noAccrualsDesc');
    });

    function goToAccruals() {
        //VTPortal.app.navigate("scheduler");
        if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
            VTPortal.roApp.selectedTab(3);
            VTPortal.app.navigate('scheduler/2', { root: true });
        }
        else {
            VTPortal.roApp.selectedTab(2);
            VTPortal.app.navigate('scheduler/2', { root: true });
        }
    }

    var viewModel = {
        goToAccruals: goToAccruals,
        viewShown: getAccruals,
        refreshAcc: refreshAccruals,
        scheduleDS: scheduleDS,
        productivDS: productivDS,
        hasPermission: hasPermission,
        holidaysDS: holidaysDS,
        accrualsTitle: accrualsTitle,
        lblaccrualsDesc: accrualsTitle,
        accrualsTitleList: i18nextko.i18n.t('accrualsTitleList'),
        holidaysTitleList: i18nextko.i18n.t('holidaysTitleList'),

        listSchedule: {
            dataSource: scheduleDS,
            activeStateEnabled: false,
            scrollingEnabled: false,
        },
        listHolidays: {
            dataSource: holidaysDS,
            activeStateEnabled: false,
            scrollingEnabled: false,
        },
        listProductiV: {
            dataSource: productivDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: true,
            itemTemplate: 'ProductiVItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roResumeProductiV') + "</div>");
            }
        },
        lblnoAccrualsDesc: noAccrualsDesc,
        btnGoAccruals: {
            onClick: goToAccruals,
            icon: 'spinright'
        },
        iconAccruals: {
            onClick: goToAccruals,
            icon: 'columnchooser'
        }
    };

    return viewModel;
};