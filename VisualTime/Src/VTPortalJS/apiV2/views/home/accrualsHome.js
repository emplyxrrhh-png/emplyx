VTPortal.accrualsHome = function (params) {
    var selectedDate = ko.observable(moment().add(-1, 'days').toDate());
    var modelIsReady = ko.observable(false);
    var scheduleDS = ko.observable(null);
    var holidaysDS = ko.observable(null);

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

                        //Solo si tenemos permisos para consultar saldos de presencia
                        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals)) {
                            var summaryDS = [];

                            var accrualsSummary = result.ScheduleSummary;
                            if (accrualsSummary.WeekAccruals.length > 0) summaryDS = summaryDS.concat(accrualsSummary.WeekAccruals);
                            if (accrualsSummary.MonthAccruals.length > 0) summaryDS = summaryDS.concat(accrualsSummary.MonthAccruals);
                            if (accrualsSummary.YearAccruals.length > 0) summaryDS = summaryDS.concat(accrualsSummary.YearAccruals);
                            if (accrualsSummary.ContractAccruals.length > 0) summaryDS = summaryDS.concat(accrualsSummary.ContractAccruals);
                            if (accrualsSummary.YearWorkAccruals && accrualsSummary.YearWorkAccruals.length > 0) summaryDS = summaryDS.concat(accrualsSummary.YearWorkAccruals);

                            for (var i = 0; i < summaryDS.length; i++) {
                                summaryDS[i].Name = summaryDS[i].Name + ': ' + summaryDS[i].TotalFormat + ' ' + (summaryDS[i].YearWorkPeriod || '');
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
                                    if (summaryDS[i].IDShift == -1) { //Vacaciones por horas
                                        summaryDS[i].Pending = i18nextko.i18n.t('roHolidayResume_Pending') + ': ' + (summaryDS[i].ValueFormat === "H" ? VTPortalUtils.utils.hoursToDuration(summaryDS[i].Pending) : summaryDS[i].Pending);
                                        summaryDS[i].Lasting = i18nextko.i18n.t('roHolidayResume_Lasting') + ': ' + (summaryDS[i].ValueFormat === "H" ? VTPortalUtils.utils.hoursToDuration(summaryDS[i].Lasting) : summaryDS[i].Lasting); 
                                        summaryDS[i].Available = i18nextko.i18n.t('roHolidayResume_Available') + ': ' + (summaryDS[i].ValueFormat === "H" ? VTPortalUtils.utils.hoursToDuration(summaryDS[i].Available) : summaryDS[i].Available);
                                        summaryDS[i].Prevision = i18nextko.i18n.t('roHolidayResume_Prevision') + ': ' + (summaryDS[i].ValueFormat === "H" ? VTPortalUtils.utils.hoursToDuration(summaryDS[i].Prevision) : summaryDS[i].Prevision); 
                                        if (summaryDS[i].Done > 0) {
                                            summaryDS[i].Done = i18nextko.i18n.t('roHolidayResume_Done') + ': ' + (summaryDS[i].ValueFormat === "H" ? VTPortalUtils.utils.hoursToDuration(summaryDS[i].Done) : summaryDS[i].Done); 
                                        }
                                        else {
                                            summaryDS[i].Done = "";
                                        }
                                    }
                                    else { //Vacaciones por días
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
                                VTPortal.roApp.holidaysDS([]);
                            }
                        }

                        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
                            VTPortal.roApp.db.settings.updateCacheDS('accruals', {
                                idemployee: VTPortal.roApp.userId,
                                schedule: scheduleDS(),
                                holidays: holidaysDS()
                            });
                        }


                    } else {
                        scheduleDS([]);
                        holidaysDS([]);
                        var onContinue = function () {
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

    var accrualsViewShown = function () {

        let tmpDS = VTPortal.roApp.db.settings.getCacheDS('accruals');
        if (tmpDS != null && tmpDS.idemployee == VTPortal.roApp.userId) {
            scheduleDS(tmpDS.schedule);
            holidaysDS(tmpDS.holidays);
        } else {
            scheduleDS(null);
            holidaysDS(null);
        }

        if (window.VTPortalUtils.needToRefresh('accruals') || scheduleDS() == null || holidaysDS() == null || VTPortal.roApp.impersonatedIDEmployee != -1) refreshAccruals();
    }


    var accrualsTitle = ko.computed(function () {
        return i18nextko.i18n.t('accrualsTitle');
    });
    var noAccrualsDesc = ko.computed(function () {
        return i18nextko.i18n.t('noAccrualsDesc');
    });

    function goToAccruals() {
            window.VTPortalUtils.utils.setActiveTab('accruals');
    }

    var viewModel = {
        goToAccruals: goToAccruals,
        viewShown: accrualsViewShown,
        scheduleDS: scheduleDS,
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