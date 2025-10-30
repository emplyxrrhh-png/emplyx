VTPortal.myPunches = function (params) {
    var myEventHandler = function (model, e) {
        var res = 'middle';
        var offset = e.targetOffset;
        if (offset < 0) swipeRight();
        else if (offset > 0) swipeLeft();
    }

    var selectedDate = ko.observable(new Date());

    var modelIsReady = ko.observable(false);
    var punchesDS = ko.observable([]);

    var swipeLeft = function () {
        selectedDate(moment(selectedDate()).subtract(1, 'days'));
    }

    var swipeRight = function () {
        selectedDate(moment(selectedDate()).add(1, 'days'));
    }

    function viewShown(sDate) {
        selectedDate(sDate);
        modelIsReady(true);
    };

    var onNewRequest = function () {
        VTPortal.app.navigate('forgottenPunch/-1/' + moment(selectedDate()).format("YYYY-MM-DD"));
    }

    var hasPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Punch.ScheduleQuery || VTPortal.roApp.empPermissions().Punch.ProductiVQuery))) {
            return true;
        } else {
            return false;
        }
    });

    var loadDiaryAccruals = function () {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals || VTPortal.roApp.empPermissions().Schedule.ProductivAccruals))) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var listDS = punchesDS()

                    //Solo si tenemos permisos para consultar saldos de tareas
                    if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.ScheduleAccruals)) {
                        var schedule = result.ScheduleAccruals;
                        for (var i = 0; i < schedule.length; i++) {
                            var cssClass = '';

                            switch (schedule[i].key) {
                                case 'daily':
                                    schedule[i].key = 'presenceDaily';
                                    cssClass = 'dx-icon-scheduleDaily';
                                    break;
                            }

                            for (var z = 0; z < schedule[i].items.length; z++) {
                                schedule[i].items[z].cssClass = cssClass;
                                schedule[i].items[z].hasAction = false;
                            }
                        }
                        if (schedule.length > 0) listDS.push(schedule[0]);
                    }

                    //Solo si tenemos permisos para consultar saldos de presencia
                    if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Schedule.ProductivAccruals)) {
                        var productiV = result.TaskAccruals;
                        for (var i = 0; i < productiV.length; i++) {
                            var cssClass = '';
                            switch (productiV[i].key) {
                                case 'daily':
                                    productiV[i].key = 'productiVDaily';
                                    cssClass = 'dx-icon-productivDaily';
                                    break;
                            }

                            for (var z = 0; z < productiV[i].items.length; z++) {
                                productiV[i].items[z].cssClass = cssClass;
                                productiV[i].items[z].hasAction = false;
                            }
                        }

                        if (productiV.length > 0) listDS.push(productiV[0]);
                    }

                    punchesDS(listDS)
                    $('#scrollview').height($('#panelsContent').height() - 70);
                } else {
                    productivDS([]);
                    scheduleDS([]);
                    var onContinue = function () {
                        VTPortal.roApp.loadInitialData(false, false, true, false, false);
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            }).getMyDailyAccruals(selectedDate());
        }
    }

    var refreshData = function (e) {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Punch.ScheduleQuery || VTPortal.roApp.empPermissions().Punch.ProductiVQuery))) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var punches = result.Punches;
                    for (var i = 0; i < punches.length; i++) {
                        switch (punches[i].Type) {
                            case 1:
                            case 2:
                            case 3:
                            case 7:
                                if (punches[i].TypeData != null && punches[i].TypeData > 0) {
                                    if (punches[i].ActualType == 1) {
                                        if (typeof punches[i].InTelecommute != 'undefined' && punches[i].InTelecommute.toLowerCase() == "true") {
                                            punches[i].cssClass = 'dx-icon-TATC_in';
                                        }
                                        else {
                                            punches[i].cssClass = 'dx-icon-TA_in_cause';
                                        }
                                    } else {
                                        punches[i].cssClass = 'dx-icon-TA_out_cause';
                                    }
                                } else {
                                    if (punches[i].ActualType == 1) {
                                        if (typeof punches[i].InTelecommute != 'undefined' && punches[i].InTelecommute.toLowerCase() == "true") {
                                            punches[i].cssClass = 'dx-icon-TATC_in';
                                        }
                                        else {
                                            punches[i].cssClass = 'dx-icon-TA_in';
                                        }
                                    } else {
                                        punches[i].cssClass = 'dx-icon-TA_out';
                                    }
                                }
                                punches[i].hasAction = true;
                                break;
                            case 4:
                                punches[i].cssClass = 'dx-icon-Task_change';
                                punches[i].hasAction = false;
                                break;
                            case 13:
                                punches[i].cssClass = 'dx-icon-CostCenter';
                                punches[i].hasAction = false;
                                break;
                        }
                        punches[i].Name = moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('LLL');
                        punches[i].Value = punches[i].RequestedTypeData != "" ? i18nextko.i18n.t('roRequestPendingApprove') + punches[i].RequestedTypeData : punches[i].RelatedInfo;
                    }

                    punchesDS(punches);

                    //var categoryPunches = [{ key: 'punches', items: punches }];
                    //punchesDS(categoryPunches);
                    //loadDiaryAccruals();

                    $('#scrollview').height($('#panelsContent').height() - 70);
                } else {
                    punchesDS([]);
                    var onContinue = function () {
                        VTPortal.roApp.loadInitialData(false, false, true, false, false);
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            }).getMyPunches(e.value);
        }
    }

    var viewModel = {
        title: i18nextko.i18n.t('roMyPunchesTitle'),
        modelIsReady: modelIsReady,
        initializeView: viewShown,
        punchesDS: punchesDS,
        hasPermission: hasPermission,
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
        listOptions: {
            dataSource: punchesDS,
            //scrollingEnabled: false,
            //grouped: true,
            //collapsibleGroups: true,
            //groupTemplate: function (data) {
            //    return $("<div>" + i18nextko.i18n.t('roDaily_' + data.key) + "</div>")
            //},
            scrollingEnabled: false,
            grouped: false,
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    VTPortal.app.navigate('justifyPunch/-1/' + moment(info.itemData.DateTime).format("YYYYMMDDHHmmss"));
                }
            }
        },
        newRequest: {
            onClick: onNewRequest,
            text: '',
            icon: "Images/Common/plus.png",
            visible: VTPortal.roApp.ReadMode() == false
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};