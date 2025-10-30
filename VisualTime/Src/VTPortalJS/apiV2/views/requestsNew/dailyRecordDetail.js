VTPortal.dailyRecordDetail = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var punchesDS = ko.observable([]);
    var popupDayInfoVisible = ko.observable(false);
    var selectedDate = ko.observable(new Date());
    var canDelete = ko.observable(false);
    var forecastsDS = ko.observable([{ key: 'forecasts', items: [] }]);
    var requestId = ko.observable(null);
    var dayInfo = ko.observable(null);
    var request = ko.observable();
    var viewActions = ko.observable(false);
    var tcInfo1 = ko.observable(false), tcInfo2 = ko.observable(false), tcInfo3 = ko.observable(false), tcInfo4 = ko.observable(false), tcInfo5 = ko.observable(false);
    var zInfo1 = ko.observable(false), zInfo2 = ko.observable(false), zInfo3 = ko.observable(false), zInfo4 = ko.observable(false), zInfo5 = ko.observable(false);
    var iDate1 = ko.observable(''), iDate2 = ko.observable(''), iDate3 = ko.observable(''), iDate4 = ko.observable(''), iDate5 = ko.observable('');
    var eDate1 = ko.observable(''), eDate2 = ko.observable(''), eDate3 = ko.observable(''), eDate4 = ko.observable(''), eDate5 = ko.observable('');
    var tcTypeValue1 = ko.observable(null), zoneValue1 = ko.observable(null);
    var tcTypeValue2 = ko.observable(null), zoneValue2 = ko.observable(null);
    var tcTypeValue3 = ko.observable(null), zoneValue3 = ko.observable(null);
    var tcTypeValue4 = ko.observable(null), zoneValue4 = ko.observable(null);
    var tcTypeValue5 = ko.observable(null), zoneValue5 = ko.observable(null);
    var block1Visible = ko.observable(false);
    var block2Visible = ko.observable(false);
    var block3Visible = ko.observable(false);
    var block4Visible = ko.observable(false);
    var block5Visible = ko.observable(false);
    var blocksVisible = ko.observable(false);
    var punchesVisible = ko.observable(false);
    var hasDesajustes = ko.observable(false);
    var notAdjustedVisible = ko.observable(false);
    var dailyRecordModified = ko.observable(false);
    var tcTypesDS = ko.observable([]);
    var lblAvailable = ko.observable(0);
    var shiftLayersDS = ko.observable([]);
    var scheduleDS = ko.observable([{ key: 'forecasts', items: [] }]);
    var lblPending = ko.observable(0);
    var lblApproved = ko.observable(0);
    var lblNotAdjusted = ko.observable("");
    var infoDivVisible = ko.observable(false);
    var popupVisible = ko.observable(false);
    var myApprovalsBlock = VTPortal.approvalHistory();
    var viewHistory = ko.observable(false);
    var requestMainDivVisible = ko.observable(false);
    var loadingDivVisible = ko.observable(false);
    var fromNavigation = ko.observable("");

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));
    if (typeof params.objId != 'undefined') fromNavigation(params.objId);

    var lblPunchInfo = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().RequestInfo;
        }
        else {
            '';
        }
    });

    var lblEmployee = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().EmployeeName;
        }
        else {
            '';
        }
    });

    var lblRequestDates = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().RequestInfo;
        }
        else {
            '';
        }
    });

    var lblGroupName = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().EmployeeGroup;
        }
        else {
            '';
        }
    });

    var plannedHours = ko.computed(function () {
        if (dayInfo() != null && dayInfo().MainShift != null) {
            return i18nextko.i18n.t("roPlannedLbl") + ': ' + convertMinsToHrsMins(dayInfo().MainShift.PlannedHours);
        } else {
            return "";
        }
    });

    var setClockValues = function (punchesQuantity) {
        var pairs = punchesQuantity / 2;
        for (var i = 0; i < pairs; i++) {
            eval("block" + (i + 1) + "Visible(true)");

            var inicio = moment.tz(moment(punchesDS()[(i * 2)].DateTime), VTPortal.roApp.serverTimeZone);
            var final = moment.tz(moment(punchesDS()[(i * 2) + 1].DateTime), VTPortal.roApp.serverTimeZone);

            $("#timeBeginDailyRecord" + (i + 1)).dxTextBox({
                value: inicio.format("HH:mm"),
                displayFormat: "HH:mm"
            }).dxTextBox("instance").option("value", inicio.format("HH:mm"));
            $("#timeEndDailyRecord" + (i + 1)).dxTextBox({
                value: final.format("HH:mm"),
                displayFormat: "hh:mm"
            }).dxTextBox("instance").option("value", final.format("HH:mm"));

            if (punchesDS()[(i * 2)].InTelecommute) {
                eval("tcInfo" + (i + 1) + "(true)");
                $("#tcType" + (i + 1)).dxSelectBox("instance").option("value", "1");
            }

            if (punchesDS()[(i * 2)].IDZone != 0 && punchesDS()[(i * 2)].IDZone != 255) {
                eval("zInfo" + (i + 1) + "(true)");
                $("#tcZone" + (i + 1)).dxSelectBox("instance").option("value", punchesDS()[(i * 2)].IDZone);
            }
            //tcTypeValue1
            //zoneValue1
        }
    }

    var loadRequest = function () {
        if (VTPortal.roApp.db.settings.supervisorPortalEnabled && fromNavigation() == "FR") {
            new WebServiceRobotics(function (result) {
                if (result.Item2.Status == window.VTPortalUtils.constants.OK.value) {
                    loadingDivVisible(false);
                    requestMainDivVisible(true);

                    request(result.Item2);
                    viewActions(false);
                    if (result.Item2.ReqStatus == 0 || result.Item2.ReqStatus == 1) {
                        viewActions(true);
                    }

                    punchesDS(result.Item1.Punches);

                    if (result.Item1.Modified || !result.Item1.Adjusted) {
                        hasDesajustes(true);
                    }
                    else {
                        hasDesajustes(false);
                    }

                    if (!result.Item1.Modified) {
                        blocksVisible(true);
                        punchesVisible(false);
                        setClockValues(punchesDS().length);
                    }
                    else {
                        dailyRecordModified(true);
                        blocksVisible(false);
                        punchesVisible(true);
                        getPunches(result.Item1.Punches);
                    }

                    if (!result.Item1.Adjusted) {
                        notAdjustedVisible(true);
                        var difference = result.Item1.TimeExpected - result.Item1.TimeAccrued;

                        if (difference <= 0) {
                            if (Math.abs(difference) < 120) {
                                lblNotAdjusted(i18nextko.i18n.t("roAdjustedExceeded") + " " + convertMinsToHrsMins(Math.abs(difference)) + " " + i18nextko.i18n.t("roExceededHoursDROne"));
                            }
                            else {
                                lblNotAdjusted(i18nextko.i18n.t("roAdjustedExceeded") + " " + convertMinsToHrsMins(Math.abs(difference)) + " " + i18nextko.i18n.t("roExceededHoursDR"));
                            }
                        }
                        else {
                            if (Math.abs(difference) < 120) {
                                lblNotAdjusted(i18nextko.i18n.t("roAdjustedMissingOne") + " " + convertMinsToHrsMins(Math.abs(difference)) + " " + i18nextko.i18n.t("roExceededHoursDROne"));
                            }
                            else {
                                lblNotAdjusted(i18nextko.i18n.t("roAdjustedMissing") + " " + convertMinsToHrsMins(Math.abs(difference)) + " " + i18nextko.i18n.t("roExceededHoursDR"));
                            }
                        }
                    }
                    else {
                        notAdjustedVisible(false);
                    }

                    myApprovalsBlock.refreshApprovals(result.Item2.RequestsHistoryEntries);
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
            }).getDailyRecordRequestForSupervisor(requestId());
        }
        else {
            new WebServiceRobotics(function (result) {
                if (result.Item2 == window.VTPortalUtils.constants.OK.value) {
                    loadingDivVisible(false);
                    requestMainDivVisible(true);

                    request(result.Item1);
                    punchesDS(result.Item1.Punches);

                    if (result.Item3.ReqStatus == 0) canDelete(true);
                    else canDelete(false);

                    if (!result.Item1.Modified) {
                        hasDesajustes(false);
                        blocksVisible(true);
                        punchesVisible(false);
                        setClockValues(punchesDS().length);
                    }
                    else {
                        hasDesajustes(false);
                        dailyRecordModified(true);
                        blocksVisible(false);
                        punchesVisible(true);
                        getPunches(result.Item1.Punches);
                    }

                    viewActions(false);
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
            }).getDailyRecord(requestId());
        }
    }

    var getPunches = function (punches) {
        for (var i = 0; i < punches.length; i++) {
            if (punches[i].CauseName != "NO JUSTIFICADO" && punches[i].CauseName != "") {
                punches[i].Value = i18nextko.i18n.t('roCause') + ': ' + punches[i].CauseName
            }

            if (punches[i].ZoneName != "" && punches[i].IDZone != 255) {
                punches[i].Zone = i18nextko.i18n.t('roZone') + ': ' + punches[i].ZoneName
            }

            if (punches[i].InTelecommute == true) {
                punches[i].TC = i18nextko.i18n.t('roRequestTypeLbl') + ': ' + i18nextko.i18n.t('roHome')
            }

            switch (punches[i].Type) {
                case 1:

                    punches[i].cssClass = 'dx-icon-TA_in';
                    punches[i].Name = i18nextko.i18n.t('roEntry') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                    break;

                case 2:

                    punches[i].cssClass = 'dx-icon-TA_out';
                    punches[i].Name = i18nextko.i18n.t('roExit') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                    punches[i].TC = ""
                    break;
            }
        }
        punchesDS(punches);
    }

    var employeeImage = ko.computed(function () {
        if (typeof request() != 'undefined') {
            var backgroundImage = '';
            if (request().EmployeeImage != '') {
                backgroundImage = 'url(data:image/png;base64,' + request().EmployeeImage + ')';
            }
            else {
                backgroundImage = 'url(Images/logovtl.ico)';
            }

            return backgroundImage;
        }
        else {
            return '';
        }
    });

    var refreshDayData = function (e) {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.CauseNote) { //alw
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

                        if (typeof forecasts[i].ForecastDetail != 'undefined' && forecasts[i].ForecastDetail != "" && forecasts[i].ForecastDetail != null) {
                            forecasts[i].Description = i18nextko.i18n.t('roLeavesFrom') + ' ' + moment(forecasts[i].BeginDate).format('L') + ' ' + i18nextko.i18n.t('roLeavesCause') + ' ' + forecasts[i].Cause + ' (' + forecasts[i].ForecastDetail + ')';
                        }
                        else {
                            forecasts[i].Description = i18nextko.i18n.t('roLeavesFrom') + ' ' + moment(forecasts[i].BeginDate).format('L') + ' ' + i18nextko.i18n.t('roLeavesCause') + ' ' + forecasts[i].Cause;
                        }
                    }

                    var ds = [{ key: 'forecasts', items: forecasts }]

                    forecastsDS(ds);

                    refreshShiftLayers(result);

                    //refreshAccruals();
                } else {
                    dayInfo(null);
                    forecastsDS([{ key: 'forecasts', items: [] }]);
                    var onContinue = function () {
                        VTPortal.roApp.loadInitialData(false, false, true, false, false);
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            };

            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getEmployeeDayInfo(selectedDate(), request().IdEmployee);
        } else {
            var onContinue = function () {
                VTPortal.roApp.loadInitialData(false, false, true, false, false);
                VTPortal.roApp.redirectAtHome();
            }

            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roIncorrectApiVersion'), 'error', 0, onContinue);
        }
    }

    function convertMinsToHrsMins(minutes) {
        var h = Math.floor(minutes / 60);
        var m = minutes % 60;
        h = h < 10 ? '0' + h : h;
        m = m < 10 ? '0' + m : m;
        return h + ':' + m;
    }

    function deleteRequest() {
        if (requestId() != null && requestId() > 0) VTPortalUtils.utils.deleteRequest(requestId(), 17);
    }

    var shiftName = ko.computed(function () {
        if (dayInfo() != null && dayInfo().MainShift != null) {
            return dayInfo().MainShift.Name;
        } else {
            return "";
        }
    });

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

    function refreshData() {
        loadingDivVisible(true);
        loadRequest();
    }

    function viewShown() {
        globalStatus().viewShown();

        var callbackFuntion = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var zonesInfo = result.ListZones;
                VTPortal.roApp.zonesDS(zonesInfo);
                VTPortal.roApp.zonesDataSource(new DevExpress.data.DataSource({
                    store: VTPortal.roApp.zonesDS(),
                    searchOperation: "contains",
                    searchExpr: "Name"
                }));
            } else {
            }
        };

        if (VTPortal.roApp.zonesDS() == null) {
            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getZones();
        }
        tcTypesDS([{ FieldName: i18nextko.i18n.t("roOffice"), FieldValue: '0' }, { FieldName: i18nextko.i18n.t("roHome"), FieldValue: '1' }]);

        if (fromNavigation() == "FR") {
            if (requestId() != null && requestId() > 0) {
                VTPortalUtils.utils.markRequestAsRead(requestId());
            }
        }

        refreshData();
    };

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('lblCurrentDR'),
        subscribeBlock: globalStatus(),
        empImage: employeeImage,
        lblEmployee: lblEmployee,
        detailsVisible: function () {
            popupDayInfoVisible(true);
            var selDate = moment(params.param, "YYYY-MM-DD");
            selectedDate(selDate.toDate());
            refreshDayData();
        },
        listCurrentPunches: {
            dataSource: punchesDS,
            scrollingEnabled: true,
            grouped: false
        },
        lblRequestDates: lblRequestDates,
        lblOverlays: i18nextko.i18n.t('lblOverlays'),
        lblAccruals: i18nextko.i18n.t('roMyAccrualsHint'),
        lblNoOverlays: i18nextko.i18n.t('lblNoOverlays'),
        lblAvailable: lblAvailable,
        lblPending: lblPending,
        lblApproved: lblApproved,
        notAdjustedVisible: notAdjustedVisible,
        lblRemarks: i18nextko.i18n.t('lblRemarks'),
        lblHasRequestedDR: i18nextko.i18n.t('lblMyRequestedDR') + ' ' + moment(params.param, "YYYY-MM-DD").format('LL'),
        lblApprovedText: i18nextko.i18n.t('lblApprovedText'),
        lblPendingText: i18nextko.i18n.t('lblPendingText'),
        lblAvailableText: i18nextko.i18n.t('lblAvailableText'),
        lblRequestedDays: i18nextko.i18n.t('lblRequestedDays'),
        lblCurrentPunches: i18nextko.i18n.t('lblCurrentDR'),
        lblCurrentPunches2: i18nextko.i18n.t('lblCurrentPunches'),
        lblHasBeenModified: i18nextko.i18n.t('lblHasBeenModified'),
        lblLoading: i18nextko.i18n.t('lblLoading'),
        lblGroupName: lblGroupName,
        lblNotAdjusted: lblNotAdjusted,
        lblDayInfo: i18nextko.i18n.t('roInformationDay'),
        lblPunchInfo: lblPunchInfo,
        lblShift: i18nextko.t('roShiftLbl'),
        txtDayShift: {
            placeholder: i18nextko.t('roShiftLbl'),
            value: shiftName,
            readOnly: true
        },
        scrollPopup: {
        },
        txtPlannedHours: {
            placeholder: i18nextko.t('roPlannedLbl'),
            value: plannedHours,
            readOnly: true
        },
        listShiftLayer: {
            dataSource: shiftLayersDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: false,
            itemTemplate: 'ScheduleItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roResumeAccruals_' + data.key) + "</div>");
            }
        },
        listForecasts: {
            dataSource: forecastsDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: false,
            itemTemplate: 'ForecastItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roDailyForecasts') + "</div>");
            },
        },
        listSchedule: {
            dataSource: scheduleDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: false,
            itemTemplate: 'ScheduleItem',
            groupTemplate: function (data) {
                return $("<div>" + i18nextko.i18n.t('roResumeAccruals_' + data.key) + "</div>");
            }
        },
        myApprovalsBlock: myApprovalsBlock,
        scheduleDS: scheduleDS,
        forecastsDS: forecastsDS,
        popupVisible: popupVisible,
        infoDivVisible: infoDivVisible,
        popupDayInfoVisible: popupDayInfoVisible,
        requestMainDivVisible: requestMainDivVisible,
        loadingDivVisible: loadingDivVisible,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        punchesDS: punchesDS,
        lblNotes: i18nextko.i18n.t('roNotes'),
        hasDesajustes: hasDesajustes,
        scrollContent: {
        },
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequestNew(requestId(), request().RequestType, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequestNew(requestId(), request().RequestType, false); },
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
        btnDelete: {
            onClick: deleteRequest,
            text: '',
            hint: i18nextko.i18n.t('roDeleteRequest'),
            icon: "Images/Common/delete.png",
            visible: canDelete
        },
        tcInfo1: tcInfo1,
        tcInfo2: tcInfo2,
        tcInfo3: tcInfo3,
        tcInfo4: tcInfo4,
        tcInfo5: tcInfo5,
        zInfo1: zInfo1,
        zInfo2: zInfo2,
        zInfo3: zInfo3,
        zInfo4: zInfo4,
        zInfo5: zInfo5,
        block1Visible: block1Visible,
        block2Visible: block2Visible,
        block3Visible: block3Visible,
        block4Visible: block4Visible,
        block5Visible: block5Visible,
        blocksVisible: blocksVisible,
        punchesVisible: punchesVisible,
        dateBegin: {
            width: 67,
            value: iDate1,
            disabled: true
        },
        dateEnd: {
            width: 67,
            value: eDate1,
            disabled: true
        },
        cmbTCType: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue1,
            disabled: true
        },
        cmbZones: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue1,
            disabled: true
        },
        dateBegin2: {
            disabled: true,
            width: 67,
            value: iDate2,
        },
        dateEnd2: {
            disabled: true,
            width: 67,
            value: eDate2,
        },
        cmbTCType2: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue2,
            disabled: true
        },
        cmbZones2: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue2,
            disabled: true
        },
        dateBegin3: {
            disabled: true,
            width: 67,
            value: iDate3,
        },
        dateEnd3: {
            disabled: true,
            width: 67,
            value: eDate3,
        },
        cmbTCType3: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue3,
            disabled: true
        },
        cmbZones3: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue3,
            disabled: true
        },
        dateBegin4: {
            disabled: true,
            width: 67,
            value: iDate4,
        },
        dateEnd4: {
            disabled: true,
            width: 67,
            value: eDate4,
        },
        cmbTCType4: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue4,
            disabled: true
        },
        cmbZones4: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue4,
            disabled: true
        },
        dateBegin5: {
            disabled: true,
            width: 67,
            value: iDate5,
        },
        dateEnd5: {
            disabled: true,
            width: 67,
            value: eDate5,
        },
        cmbTCType5: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue5,
            disabled: true
        },
        cmbZones5: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue5,
            disabled: true
        },
    };

    return viewModel;
};