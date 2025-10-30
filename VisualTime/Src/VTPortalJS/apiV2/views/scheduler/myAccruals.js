VTPortal.myAccruals = function (params) {
    var myEventHandler = function (model, e) {
        var res = 'middle';
        var offset = e.targetOffset;
        if (offset < 0) swipeRight();
        else if (offset > 0) swipeLeft();
    }
    var noPermissions = VTPortal.noPermissions();
    var selectedDate = ko.observable(moment().add(-1, 'days').toDate());
    var modelIsReady = ko.observable(false);
    var productivDS = ko.observable([]);
    var scheduleDS = ko.observable([]);
    var holidaysDS = ko.observable([]);
    var detailGridDS = ko.observable([]);
    var summaryGridDS = ko.observable([]);
    var cmbSummaryShiftDS = ko.observable([]);
    var cmbSummaryShiftValue = ko.observable(null);

    var hasPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals || VTPortal.roApp.empPermissions().Schedule.ProductivAccruals))) {
            return true;
        } else {
            return false;
        }
    });

    var swipeLeft = function () {
        selectedDate(moment(selectedDate()).subtract(1, 'days'));
        refreshData();
    }

    var swipeRight = function () {
        selectedDate(moment(selectedDate()).add(1, 'days'));
        refreshData();
    }

    function viewShown(selDate) {
        selectedDate(moment().add(-1, 'days').toDate());
        refreshData();
        modelIsReady(true);
        loadLists('shifts.permissiontype');
    };

    var onNewRequest = function () {
        VTPortal.app.navigate('requestsList/6,11,13,14/null');
    }

    var refreshData = function () {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Holidays) {
            if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals || VTPortal.roApp.empPermissions().Schedule.ProductivAccruals || VTPortal.roApp.empPermissions().Schedule.QuerySchedule))) {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        //Solo si tenemos permisos para consultar saldos de presencia
                        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals)) {
                            var summaryDS = [];
                            var dailySummaryDS = [];

                            var accrualsSummary = result.ScheduleSummary;
                            if (accrualsSummary.WeekAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.WeekAccruals));
                            if (accrualsSummary.MonthAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.MonthAccruals));
                            if (accrualsSummary.YearAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.YearAccruals));
                            if (accrualsSummary.ContractAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.ContractAccruals));
                            if (accrualsSummary.YearWorkAccruals.length > 0) summaryDS = summaryDS.concat(VTPortalUtils.utils.generateAccrualsArray(accrualsSummary.YearWorkAccruals));

                            var ds = [];

                            if (summaryDS.length == 0 && dailySummaryDS.length == 0) scheduleDS([]);
                            else {
                                if (dailySummaryDS.length > 0) {
                                    ds.push({ key: 'Daily', items: dailySummaryDS.sortBy(function (n) { return n.Name }) });
                                }
                                if (summaryDS.length > 0) {
                                    ds.push({ key: 'Total', items: summaryDS.sortBy(function (n) { return n.Name }) });
                                }
                            }
                            scheduleDS(ds);
                        }

                        //Solo si tenemos permisos para consultar vacaciones
                        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.QuerySchedule)) {
                            var summaryDS = []
                            var accrualsSummary = result.HolidaysSummary;

                            if (accrualsSummary.HolidaysInfo.length > 0) {
                                for (var i = 0; i < accrualsSummary.HolidaysInfo.length; i++) {
                                    if (accrualsSummary.HolidaysInfo[i].AccrualDefaultQuery == null || accrualsSummary.HolidaysInfo[i].AccrualDefaultQuery != "L") {
                                        if (accrualsSummary.HolidaysInfo[i].IDShift > 0) accrualsSummary.HolidaysInfo[i].Type = 'O';
                                        else accrualsSummary.HolidaysInfo[i].Type = 'H';

                                        var seriesDS = []
                                        if (accrualsSummary.HolidaysInfo[i].Type == 'O') {
                                            seriesDS.push({ valueField: "Done", name: i18nextko.i18n.t('roHolidayResume_Done') });
                                        }
                                        else {
                                            if (accrualsSummary.HolidaysInfo[i].Done > 0) {
                                                seriesDS.push({ valueField: "Done", name: i18nextko.i18n.t('roHolidayResume_Done') });
                                            }
                                        }
                                        seriesDS.push({ valueField: "Pending", name: i18nextko.i18n.t('roHolidayResume_Pending') });
                                        seriesDS.push({ valueField: "Lasting", name: i18nextko.i18n.t('roHolidayResume_Lasting') });
                                        seriesDS.push({ valueField: "Available", name: i18nextko.i18n.t('roHolidayResume_Available') });
                                        seriesDS.push({ valueField: "Prevision", name: i18nextko.i18n.t('roHolidayResume_Prevision') });

                                        summaryDS.push({
                                            Name: accrualsSummary.HolidaysInfo[i].Name,
                                            Gauge: {
                                                dataSource: [{
                                                    task: accrualsSummary.HolidaysInfo[i].Name,
                                                    Done: accrualsSummary.HolidaysInfo[i].Done,
                                                    Pending: accrualsSummary.HolidaysInfo[i].Pending,
                                                    Lasting: accrualsSummary.HolidaysInfo[i].Lasting,
                                                    Available: accrualsSummary.HolidaysInfo[i].Available,
                                                    Prevision: accrualsSummary.HolidaysInfo[i].Prevision
                                                }],
                                                commonSeriesSettings: {
                                                    argumentField: "task",
                                                    type: "bar",
                                                    label: {
                                                        visible: true,
                                                        customizeText: VTPortalUtils.utils.convertHoursToTime(accrualsSummary.HolidaysInfo[i], false)
                                                    }
                                                },
                                                series: seriesDS,
                                                title: {
                                                    text: accrualsSummary.HolidaysInfo[i].Name,
                                                },
                                                legend: {
                                                    verticalAlignment: "bottom",
                                                    horizontalAlignment: "center"
                                                },
                                                argumentAxis: {
                                                    label: { visible: false }
                                                }

                                            }
                                        });
                                    }
                                }
                            }

                            if (summaryDS.length > 0) {
                                var ds = [{ key: 'Vacaciones', items: summaryDS.sortBy(function (n) { return n.Name }) }]
                                holidaysDS(ds);
                            } else {
                                holidaysDS([])
                            }
                        }

                        //Solo si tenemos permisos para consultar saldos de tareas
                        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.ProductivAccruals)) {
                            var accrualsSummary = result.ProductiVSummary;
                            var summaryDS = []

                            if (accrualsSummary.YearAccruals.length > 0) {
                                for (var i = 0; i < accrualsSummary.YearAccruals.length; i++) {
                                    accrualsSummary.YearAccruals[i].MaxValue = accrualsSummary.YearAccruals[i].TaskValue;
                                    accrualsSummary.YearAccruals[i].Type = 'H';

                                    var yearValue = accrualsSummary.YearAccruals[i].TaskValue;

                                    var monthValue = 0;
                                    $.grep(accrualsSummary.MonthAccruals, function (e) {
                                        if (e.IdTask == accrualsSummary.YearAccruals[i].IdTask) {
                                            monthValue = e.TaskValue;
                                        }
                                    });

                                    var weekValue = 0;
                                    $.grep(accrualsSummary.WeekAccruals, function (e) {
                                        if (e.IdTask == accrualsSummary.YearAccruals[i].IdTask) {
                                            weekValue = e.TaskValue;
                                        }
                                    });

                                    var dailyValue = 0;
                                    $.grep(accrualsSummary.DailyAccruals, function (e) {
                                        if (e.IdTask == accrualsSummary.YearAccruals[i].IdTask) {
                                            dailyValue = e.TaskValue;
                                        }
                                    });

                                    summaryDS.push({
                                        Name: accrualsSummary.YearAccruals[i].TaskName,
                                        Gauge: {
                                            dataSource: [{ task: accrualsSummary.YearAccruals[i].TaskName, year: yearValue, month: monthValue, week: weekValue, day: dailyValue }],
                                            commonSeriesSettings: {
                                                argumentField: "task",
                                                type: "bar",
                                                label: {
                                                    visible: true,
                                                    customizeText: VTPortalUtils.utils.convertHoursToTime(accrualsSummary.YearAccruals[i], false)
                                                }
                                            },
                                            series: [
                                                { valueField: "year", name: i18nextko.i18n.t('roPeriod_year') },
                                                { valueField: "month", name: i18nextko.i18n.t('roPeriod_month') },
                                                { valueField: "week", name: i18nextko.i18n.t('roPeriod_week') },
                                                { valueField: "day", name: i18nextko.i18n.t('roPeriod_daily') }
                                            ],
                                            title: {
                                                text: accrualsSummary.YearAccruals[i].TaskName,
                                                subtitle: {
                                                    text: "tiempo en horas"
                                                }
                                            },
                                            legend: {
                                                verticalAlignment: "bottom",
                                                horizontalAlignment: "center"
                                            },
                                            argumentAxis: {
                                                label: { visible: false }
                                            }
                                        }
                                    });
                                }

                                var ds = [{ key: 'ProductiV', items: summaryDS.sortBy(function (n) { return n.Name }) }]
                                productivDS(ds);
                            } else {
                                productivDS([]);
                            }
                        }

                        if (scheduleDS().length > 0 && (productivDS().length > 0 || holidaysDS().length > 0)) {
                            for (var i = 0; i < scheduleDS().length; i++) {
                                if (typeof $("#listSchedule").dxList("instance") !== "undefined") $("#listSchedule").dxList("instance").collapseGroup(i);
                            }
                        }
                        if (productivDS().length > 0 && (scheduleDS().length > 0 || holidaysDS().length > 0)) {
                            if (typeof $("#listProductiV").dxList("instance") !== "undefined") $("#listProductiV").dxList("instance").collapseGroup(0);
                        } 
                        if (holidaysDS().length > 0 && (scheduleDS().length > 0 || productivDS().length > 0)) {
                            if (typeof $("#listHolidays").dxList("instance") !== "undefined") $("#listHolidays").dxList("instance").collapseGroup(0);
                        } 
                        $('#scrollview').height($('#panelsContent').height());
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

    var loadLists = function (lstdataSource) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var initialShifts = [];

                initialShifts = initialShifts.add(result.SelectFields.clone());
                if (initialShifts[0]?.FieldValue) refreshDetailGridData(initialShifts[0].FieldValue);  //Si hay datos, actualizamos el grid con el primer horario seleccionado
                if (initialShifts[0]?.FieldValue) refreshSummaryGridData(initialShifts[0].FieldValue);  //Si hay datos, actualizamos el grid con el primer horario seleccionado

                cmbSummaryShiftDS(initialShifts);

                if (initialShifts.length > 0) cmbSummaryShiftValue(initialShifts[0].FieldValue);

            } else {
                var onContinue = function () {
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }, function (error) {
            var onContinue = function () {
            }
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinue);
        }).getGenericList(lstdataSource);
    }

    var cmbSummaryShiftChanged = function (e) {
        var selectedIdShift = e.value;
        refreshSummaryGridData(selectedIdShift);
        refreshDetailGridData(selectedIdShift);
    }

    function refreshDetailGridData(idShift) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {                   
                result.Value.map(c => c.DayType = i18nextko.i18n.t("conceptsSummary_" + c.DayType.toLowerCase()));
                detailGridDS(result.Value);
            } else {
                var onContinue = function () {
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }, function (error) {
            var onContinue = function () {
            }
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingSummaryHolidaysInfo"), 'error', 0, onContinue);
        }).getConceptsDetailByShift(idShift);
    }

    function refreshSummaryGridData(idShift) {        
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                result.Value.map(c => c.TransactionDate = moment(c.TransactionDate).toDate());
                summaryGridDS(result.Value);
            } else {
                var onContinue = function () {
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }, function (error) {
            var onContinue = function () {
            }
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingSummaryHolidaysInfo"), 'error', 0, onContinue);
        }).getConceptsSummaryByShift(idShift);
    }

    var viewModel = {
        modelIsReady: modelIsReady,
        initializeView: viewShown,
        hasPermission: hasPermission,
        noPermissions: noPermissions,
        headerDate: {
            value: selectedDate,
            pickerType: 'rollers',
            displayFormat: 'shortDate'
        },
        btnPrevious: {
            icon: 'chevronprev',
            onClick: swipeLeft
        },
        btnNext: {
            icon: 'chevronnext',
            onClick: swipeRight
        },
        //myEventHandler: myEventHandler,
        scheduleDS: scheduleDS,
        productivDS: productivDS,
        holidaysDS: holidaysDS,
        listSchedule: {
            dataSource: scheduleDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: true,
            itemTemplate: 'ScheduleItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roResumeAccruals') + "</div>");
            }
        },
        listHolidays: {
            dataSource: holidaysDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: true,
            itemTemplate: 'HolidaysItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roResumeHolidays') + "</div>");
            }
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
        newRequest: {
            onClick: onNewRequest,
            text: '',
            icon: "Images/Common/plus.png",
            visible: VTPortal.roApp.ReadMode() == false
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        summaryGrid: {
            dataSource: summaryGridDS,
            keyExpr: 'ID',
            columns: [
                {
                    caption: i18nextko.i18n.t('conceptsSummary_date'),
                    dataField: 'TransactionDate',
                    sortOrder: 'desc' 
                    ,minWidth: 100
                },
                {
                    caption: i18nextko.i18n.t('conceptsSummary_date'),
                    dataField: 'TransactionDateOrder',
                    sortOrder: 'desc', 
                    visible: false
                },
                { caption: i18nextko.i18n.t('conceptsSummary_detail'), dataField: 'Detail', minWidth: 105 },
                { caption: i18nextko.i18n.t('conceptsSummary_days'), dataField: 'Days' },
                { caption: i18nextko.i18n.t('conceptsSummary_total'), dataField: 'Total' }
            ],
            showBorders: true,
            sorting: { mode: 'none' },
        },
        detailGrid: {
            dataSource: detailGridDS,
            keyExpr: 'ID',
            columns: [                
                { caption: i18nextko.i18n.t('conceptsSummary_daytype'), dataField: 'DayType' }, 
                { caption: i18nextko.i18n.t('conceptsSummary_numberofdays'), dataField: 'NumberOfDays' }
            ],
            showBorders: true,            
            sorting: { mode: 'none' },                        
        },
        lblSummaryShift: i18nextko.t('roSelectSummaryShift'),
        cmbSummaryShiftDS: cmbSummaryShiftDS,
        cmbSummaryShift: {
            dataSource: cmbSummaryShiftDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: cmbSummaryShiftValue,
            onValueChanged: cmbSummaryShiftChanged,
            onload: cmbSummaryShiftChanged
        }
    }       

    return viewModel;
};