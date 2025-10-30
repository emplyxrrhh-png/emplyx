VTPortal.addDailyRecord = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var selectedDate = ko.observable(new Date());
    var punchesDS = ko.observable([]);
    var punchesPatternDS = ko.observable([]);
    var Punches = ko.observable([]);
    var blocksShown = ko.observable(1);
    var forecastsDS = ko.observable([{ key: 'forecasts', items: [] }]);
    var scheduleDS = ko.observable([{ key: 'forecasts', items: [] }]);
    var dayInfo = ko.observable(null);
    var tcInfo = ko.observable(null);
    var requestValue2DS = ko.observable([]);
    var reqValue1 = ko.observable('E'), tcTypeValue1 = ko.observable(null), reqValue2 = ko.observable(null), zoneValue1 = ko.observable(null);
    var tcTypeValue2 = ko.observable(null), zoneValue2 = ko.observable(null);
    var tcTypeValue3 = ko.observable(null), zoneValue3 = ko.observable(null);
    var tcTypeValue4 = ko.observable(null), zoneValue4 = ko.observable(null);
    var tcTypeValue5 = ko.observable(null), zoneValue5 = ko.observable(null);
    var tcTypesDS = ko.observable([]);
    var shiftLayersDS = ko.observable([]);
    var zoneSelected1 = ko.observable(null), tcTypeSelected1 = ko.observable(null);
    var zoneSelected2 = ko.observable(null), tcTypeSelected2 = ko.observable(null);
    var zoneSelected3 = ko.observable(null), tcTypeSelected3 = ko.observable(null);
    var zoneSelected4 = ko.observable(null), tcTypeSelected4 = ko.observable(null);
    var zoneSelected5 = ko.observable(null), tcTypeSelected5 = ko.observable(null);
    var block1Shown = ko.observable(false);
    var block2Shown = ko.observable(false);
    var block3Shown = ko.observable(false);
    var block4Shown = ko.observable(false);
    var block5Shown = ko.observable(false);
    var hasPunchesPattern = ko.observable(false);

    var delete2Shown = ko.computed(function () {
        return !block3Shown();
    });

    var delete3Shown = ko.computed(function () {
        return !block4Shown();
    });

    var delete4Shown = ko.computed(function () {
        return !block5Shown();
    });

    var blocksAreValid = ko.observable(false);

    //var iDate1 = ko.observable(moment().startOf('day').toDate()), iDate2 = ko.observable(moment().startOf('day').toDate()), iDate3 = ko.observable(moment().startOf('day').toDate()), iDate4 = ko.observable(moment().startOf('day').toDate()), iDate5 = ko.observable(moment().startOf('day').toDate());
    //var eDate1 = ko.observable(moment().startOf('day').toDate()), eDate2 = ko.observable(moment().startOf('day').toDate()), eDate3 = ko.observable(moment().startOf('day').toDate()), eDate4 = ko.observable(moment().startOf('day').toDate()), eDate5 = ko.observable(moment().startOf('day').toDate());
    var iDate1 = ko.observable(null), iDate2 = ko.observable(null), iDate3 = ko.observable(null), iDate4 = ko.observable(null), iDate5 = ko.observable(null);
    var eDate1 = ko.observable(null), eDate2 = ko.observable(null), eDate3 = ko.observable(null), eDate4 = ko.observable(null), eDate5 = ko.observable(null);

    var blocksShown = ko.computed(function () {
        var i = 1;
        if (block2Shown()) {
            i += 1;
        }
        if (block3Shown()) {
            i += 1;
        }
        if (block4Shown()) {
            i += 1;
        }
        if (block5Shown()) {
            i += 1;
        }
        return i;
    });

    var validateBlocks = function () {
        if (block1Invalid() || block2Invalid() || block3Invalid() || block4Invalid() || block5Invalid()) {
            blocksAreValid(false);
        }
        else {
            blocksAreValid(true);
        }
    }

    function diffInBlock(i) {
        var eDateBegin = eval("eDate" + i);
        var iDateEnd = eval("iDate" + i);

        var difInBlock = 0;

        if (eDateBegin() != null && iDateEnd() != null) {
            if (eDateBegin() >= iDateEnd()) {
                difInBlock = moment.duration(moment(eDateBegin(), "HH:mm").diff(moment(iDateEnd(), "HH:mm"))).asMinutes();
            }
            else {
                difInBlock = moment.duration(moment(eDateBegin(), "HH:mm").add(1, 'days').diff(moment(iDateEnd(), "HH:mm"))).asMinutes();
            }
        }
        return Math.floor(difInBlock);
    }

    var lblRemaining = ko.computed(function () {
        var total = diffInBlock(1) + diffInBlock(2) + diffInBlock(3) + diffInBlock(4) + diffInBlock(5);

        return i18nextko.i18n.t("roDeclaringDR") + " " + convertMinsToHrsMins(total) + " " + i18nextko.i18n.t("roDeclaringDR2");
    });

    var block1HasNightTime = ko.computed(function () {
        if ((moment(eDate1(), "HH:mm").format("HH:mm") < moment(iDate1(), "HH:mm").format("HH:mm"))) {
            return true;
        }
        else {
            return false;
        }
    });

    var block2HasNightTime = ko.computed(function () {
        if (block2Shown()) {
            if (((moment(eDate2(), "HH:mm").format("HH:mm") < moment(iDate2(), "HH:mm").format("HH:mm")) || (moment(iDate2(), "HH:mm").format("HH:mm") < moment(eDate1(), "HH:mm").format("HH:mm")))) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var block3HasNightTime = ko.computed(function () {
        if (block3Shown()) {
            if ((moment(eDate3(), "HH:mm").format("HH:mm") < moment(iDate3(), "HH:mm").format("HH:mm")) || (moment(iDate3(), "HH:mm").format("HH:mm") < moment(eDate2(), "HH:mm").format("HH:mm"))) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var block4HasNightTime = ko.computed(function () {
        if (block4Shown()) {
            if ((moment(eDate4(), "HH:mm").format("HH:mm") < moment(iDate4(), "HH:mm").format("HH:mm")) || (moment(iDate4(), "HH:mm").format("HH:mm") < moment(eDate3(), "HH:mm").format("HH:mm"))) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var block5HasNightTime = ko.computed(function () {
        if (block5Shown()) {
            if ((moment(eDate5(), "HH:mm").format("HH:mm") < moment(iDate5(), "HH:mm").format("HH:mm")) || (moment(iDate5(), "HH:mm").format("HH:mm") < moment(eDate4(), "HH:mm").format("HH:mm"))) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });
    var block1Invalid = ko.computed(function () {
        if (moment(iDate1(), "HH:mm").isValid() && moment(eDate1(), "HH:mm").isValid()) {
            if (moment(eDate1(), "HH:mm").format("HH:mm") != moment(iDate1(), "HH:mm").format("HH:mm")) {
                return false;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var block2Invalid = ko.computed(function () {
        if (block2Shown() && (moment(iDate2(), "HH:mm").isValid() && moment(eDate2(), "HH:mm").isValid())) {
            if ((moment(eDate2(), "HH:mm").format("HH:mm") != moment(iDate2(), "HH:mm").format("HH:mm"))) {
                if (block2HasNightTime()) {
                    return false;
                }
                else {
                    if ((moment(eDate1(), "HH:mm").format("HH:mm") < moment(iDate2(), "HH:mm").format("HH:mm"))) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var block3Invalid = ko.computed(function () {
        if (block3Shown() && (moment(iDate3(), "HH:mm").isValid() && moment(eDate3(), "HH:mm").isValid())) {
            if ((moment(eDate3(), "HH:mm").format("HH:mm") != moment(iDate3(), "HH:mm").format("HH:mm"))) {
                if (block3HasNightTime()) {
                    return false;
                }
                else {
                    if ((moment(eDate2(), "HH:mm").format("HH:mm") < moment(iDate3(), "HH:mm").format("HH:mm"))) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var block4Invalid = ko.computed(function () {
        if (block4Shown() && (moment(iDate4(), "HH:mm").isValid() && moment(eDate4(), "HH:mm").isValid())) {
            if ((moment(eDate4(), "HH:mm").format("HH:mm") != moment(iDate4(), "HH:mm").format("HH:mm"))) {
                if (block4HasNightTime()) {
                    return false;
                }
                else {
                    if ((moment(eDate3(), "HH:mm").format("HH:mm") < moment(iDate4(), "HH:mm").format("HH:mm"))) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var block5Invalid = ko.computed(function () {
        if (block5Shown() && (moment(iDate5(), "HH:mm").isValid() && moment(eDate5(), "HH:mm").isValid())) {
            if ((moment(eDate5(), "HH:mm").format("HH:mm") != moment(iDate5(), "HH:mm").format("HH:mm"))) {
                if (block5HasNightTime()) {
                    return false;
                }
                else {
                    if ((moment(eDate4(), "HH:mm").format("HH:mm") < moment(iDate5(), "HH:mm").format("HH:mm"))) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var thereIsNighttime = ko.computed(function () {
        if (block1HasNightTime() || block2HasNightTime() || block3HasNightTime() || block4HasNightTime() || block5HasNightTime()) {
            return true;
        }
        else {
            return false;
        }
    });

    var noPunches = ko.computed(function () {
        return i18nextko.i18n.t('noPunches');
    });
    var requestId = ko.observable(null);
    var popupDayInfoVisible = ko.observable(false);
    var popupNewPunchVisible = ko.observable(false);

    var saveVisible = ko.computed(function () {
        if (moment(iDate1(), "HH:mm").isValid() && moment(eDate1(), "HH:mm").isValid()) {
            return true;
        }
        else {
            return false;
        }
    });

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

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    if (typeof params.objId != 'undefined') hasPunchesPattern(params.objId);

    var lblDailyRecordDay = ko.computed(function () {
        return moment(params.param, "YYYY-MM-DD").format('LL');
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

    var setClockValues = function (punchesQuantity) {
        var pairs = punchesQuantity / 2;
        for (var i = 0; i < pairs; i++) {
            var e = document.getElementById('block' + (i + 1));
            e.style.display = 'block';
            eval("block" + (i + 1) + "Shown(true)");

            var iDateTemp;
            var eDateTemp;

            //var inicio = moment.tz(moment(punchesPatternDS()[(i * 2)].DateTime), VTPortal.roApp.serverTimeZone).toDate();
            //var final = moment.tz(moment(punchesPatternDS()[(i * 2) + 1].DateTime), VTPortal.roApp.serverTimeZone).toDate();

            var inicio = moment(moment.tz(moment(punchesPatternDS()[(i * 2)].DateTime), VTPortal.roApp.serverTimeZone).format("YYYY-MM-DD HH:mm"), "YYYY-MM-DD HH:mm").toDate();
            var final = moment(moment.tz(moment(punchesPatternDS()[(i * 2) + 1].DateTime), VTPortal.roApp.serverTimeZone).format("YYYY-MM-DD HH:mm"), "YYYY-MM-DD HH:mm").toDate();

            eval("iDateTemp=iDate" + (i + 1));
            eval("eDateTemp=eDate" + (i + 1));

            iDateTemp(inicio);
            eDateTemp(final);
        }
    }

    var shiftName = ko.computed(function () {
        if (dayInfo() != null && dayInfo().MainShift != null) {
            return dayInfo().MainShift.Name;
        } else {
            return "";
        }
    });

    var loadLists = function (lstdataSource) {
        var callbackFuntionDR = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                punchesPatternDS(result.Value.Punches);
                setClockValues(punchesPatternDS().length);
            } else {
            }
        };

        if (VTPortal.roApp.DailyRecordPatternEnabled() == true && hasPunchesPattern() == 'true') {
            new WebServiceRobotics(function (result) { callbackFuntionDR(result); }).getDailyRecordPattern(params.param);
        }

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
    }

    var telecommutingImage = ko.computed(function () {
        backgroundImage = 'url(Images/logovtl.ico) no-repeat center center';
        return backgroundImage;
    });

    var zoneImage = ko.computed(function () {
        backgroundImage = 'url(Images/logovtl.ico) no-repeat center center';
        return backgroundImage;
    });

    function saveRequest() {
        var wsRobotics = null;
        var continuar = true;

        validateBlocks();

        if (blocksAreValid()) {
            continuar = true;
        }
        else {
            continuar = false;
        }

        if (continuar) {
            var blockSwitch = 0;

            if (block1HasNightTime()) {
                blockSwitch = 1;
            }
            else {
                if (block2HasNightTime()) {
                    blockSwitch = 2;
                }
                else {
                    if (block3HasNightTime()) {
                        blockSwitch = 3;
                    }
                    else {
                        if (block4HasNightTime()) {
                            blockSwitch = 4;
                        }
                        else {
                            if (block5HasNightTime()) {
                                blockSwitch = 5;
                            }
                            else { blockSwitch = 0; }
                        }
                    }
                }
            }

            var newPunches = Punches();

            for (let i = 1; i <= blocksShown(); i++) {
                var newPunchBegin = new Object();
                var newPunchEnd = new Object();

                //Entrada
                newPunchBegin.isNew = true;
                newPunchBegin.IDEmployee = VTPortal.roApp.employeeId;
                newPunchBegin.Direction = 'E';
                newPunchBegin.Type = 1;

                if (eval("zoneSelected" + i + "()") != null) {
                    newPunchBegin.IDZone = eval("zoneSelected" + i + "()").Id;
                }
                if (eval("tcTypeSelected" + i + "()") != null) {
                    newPunchBegin.InTelecommute = (eval("tcTypeSelected" + i + "()").FieldValue == "1") ? true : false;
                }

                //Salida
                newPunchEnd.isNew = true;
                newPunchEnd.IDEmployee = VTPortal.roApp.employeeId;
                newPunchEnd.Direction = 'S';
                newPunchEnd.Type = 2;

                if (eval("zoneSelected" + i + "()") != null) {
                    newPunchBegin.IDZone = eval("zoneSelected" + i + "()").Id;
                }

                //Horas de los fichajes

                if (blockSwitch == 0) {
                    //newPunchBegin.DateTime = moment(params.param + ' ' + eval("iDate" + i + "()"), "YYYY-MM-DD HH:mm:ss").format("YYYY-MM-DD HH:mm:ss");
                    newPunchBegin.DateTime = moment(params.param + ' ' + moment(eval("iDate" + i + "()")).format("HH:mm")).format("YYYY-MM-DD HH:mm:ss");
                    newPunchBegin.ShiftDate = moment(params.param, "YYYY-MM-DD").format(VTPortal.roApp.localFormat);

                    newPunchEnd.DateTime = moment(params.param + ' ' + moment(eval("eDate" + i + "()")).format("HH:mm")).format("YYYY-MM-DD HH:mm:ss");
                    newPunchEnd.ShiftDate = moment(params.param, "YYYY-MM-DD").format(VTPortal.roApp.localFormat);
                }
                else if (blockSwitch == i) {
                    if (moment(eval("iDate" + i + "()")).format("HH:mm") <= moment(eval("eDate" + i + "()")).format("HH:mm")) {
                        newPunchBegin.DateTime = moment(params.param + ' ' + moment(eval("iDate" + i + "()")).format("HH:mm")).add(1, 'days').format("YYYY-MM-DD HH:mm:ss");
                        newPunchBegin.ShiftDate = moment(params.param, "YYYY-MM-DD").format(VTPortal.roApp.localFormat);

                        newPunchEnd.DateTime = moment(params.param + ' ' + moment(eval("eDate" + i + "()")).format("HH:mm")).add(1, 'days').format("YYYY-MM-DD HH:mm:ss");
                        newPunchEnd.ShiftDate = moment(params.param, "YYYY-MM-DD").add(1, 'days').format(VTPortal.roApp.localFormat);
                    }
                    else {
                        newPunchBegin.DateTime = moment(params.param + ' ' + moment(eval("iDate" + i + "()")).format("HH:mm")).format("YYYY-MM-DD HH:mm:ss");
                        newPunchBegin.ShiftDate = moment(params.param, "YYYY-MM-DD").format(VTPortal.roApp.localFormat);

                        newPunchEnd.DateTime = moment(params.param + ' ' + moment(eval("eDate" + i + "()")).format("HH:mm")).add(1, 'days').format("YYYY-MM-DD HH:mm:ss");
                        newPunchEnd.ShiftDate = moment(params.param, "YYYY-MM-DD").add(1, 'days').format(VTPortal.roApp.localFormat);
                    }
                }
                else if (blockSwitch < i) {
                    newPunchBegin.DateTime = moment(params.param + ' ' + moment(eval("iDate" + i + "()")).format("HH:mm")).add(1, 'days').format("YYYY-MM-DD HH:mm:ss");
                    newPunchBegin.ShiftDate = moment(params.param, "YYYY-MM-DD").add(1, 'days').format(VTPortal.roApp.localFormat);

                    newPunchEnd.DateTime = moment(params.param + ' ' + moment(eval("eDate" + i + "()")).format("HH:mm")).add(1, 'days').format("YYYY-MM-DD HH:mm:ss");
                    newPunchEnd.ShiftDate = moment(params.param, "YYYY-MM-DD").add(1, 'days').format(VTPortal.roApp.localFormat);
                }
                else {
                    newPunchBegin.DateTime = moment(params.param + ' ' + moment(eval("iDate" + i + "()")).format("HH:mm")).format("YYYY-MM-DD HH:mm:ss");
                    newPunchBegin.ShiftDate = moment(params.param, "YYYY-MM-DD").format(VTPortal.roApp.localFormat);

                    newPunchEnd.DateTime = moment(params.param + ' ' + moment(eval("eDate" + i + "()")).format("HH:mm")).format("YYYY-MM-DD HH:mm:ss");
                    newPunchEnd.ShiftDate = moment(params.param, "YYYY-MM-DD").format(VTPortal.roApp.localFormat);
                }

                if (moment(newPunchBegin.DateTime).isValid() && moment(newPunchEnd.DateTime).isValid()) {
                    newPunches.push(newPunchBegin);
                    newPunches.push(newPunchEnd);
                }
            }
            punchesDS(newPunches);

            var DailyRecord = {
                "IdEmployee": VTPortal.roApp.employeeId,
                "DailyRecordStatus": 0,
                "Punches": punchesDS(),
                "RecordDate": moment(params.param, "YYYY-MM-DD").format(VTPortal.roApp.localFormat)
            }
            var dailyRecordJSON = JSON.stringify(DailyRecord);
            var onWSError = function (error) {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
            }
            var onWSResult = function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.db.settings.markForRefresh(['status','dailyrecord','notifications']);
                    punchesDS([]);
                    Punches([]);
                    VTPortal.app.navigate("myDailyRecord", { root: true });
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
                } else {
                    punchesDS([]);
                    Punches([]);
                    var onContinue = function () {
                        VTPortal.app.navigate("myDailyRecord", { root: true });
                    }
                    VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
                }
            };
            if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);
            wsRobotics.SaveRequestDailyRecord(dailyRecordJSON);
        }
        else {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("invalidPeriods"), 'warning', 2000);
        }
    }

    function viewShown() {
        globalStatus().viewShown();
        loadLists();
        tcTypesDS([{ FieldName: i18nextko.i18n.t("roOffice"), FieldValue: '0' }, { FieldName: i18nextko.i18n.t("roHome"), FieldValue: '1' }]);
        tcInfo(params.iDate == 'true' ? true : false);
        requestValue2DS(new DevExpress.data.DataSource({
            store: []
        }));
    };

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roAddDailyRecord'),
        subscribeBlock: globalStatus(),
        lblDailyRecordForDay: i18nextko.i18n.t('roDailyRecord'),
        lblDailyRecordDay: lblDailyRecordDay,
        noPunches: noPunches,
        forecastsDS: forecastsDS,
        scheduleDS: scheduleDS,
        lblLoading: i18nextko.i18n.t('lblLoading'),
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        punchesDS: punchesDS,
        lblShift: i18nextko.t('roShiftLbl'),
        dateBeginText: i18nextko.t('roEntry'),
        dateEndText: i18nextko.t('roExit'),
        txtDayShift: {
            placeholder: i18nextko.t('roShiftLbl'),
            value: shiftName,
            readOnly: true
        },
        tcText: i18nextko.t('roTCF'),
        zText: i18nextko.t('roZoneDR'),
        tcInfo: tcInfo,
        reqValue1: reqValue1,
        txtPlannedHours: {
            placeholder: i18nextko.t('roPlannedLbl'),
            value: plannedHours,
            readOnly: true
        },
        dateBegin: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: iDate1,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: 'HH:mm',
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    iDate1(moment("1970-01-01 " + moment().format("HH:mm"), "YYYY-MM-DD HH:mm").toDate());
                }
            }
        },
        dateEnd: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: eDate1,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: 'HH:mm',
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    eDate1(moment(iDate1()).clone().toDate());
                }
            }
        },
        cmbTCType: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue1,
            onValueChanged: function (data) {
                tcTypeSelected1(data.component.option('selectedItem'));
            }
        },
        cmbZones: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue1,
            onValueChanged: function (data) {
                zoneSelected1(data.component.option('selectedItem'));
            }
        },
        tcImage: telecommutingImage,
        zImage: zoneImage,
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
        popupDayInfoVisible: popupDayInfoVisible,
        lblInitialDate: i18nextko.t('roRequestDateHourLbl'),
        lblTC: i18nextko.t('roTCF'),
        detailsVisible: function () {
            var selDate = moment(params.param, "YYYY-MM-DD");
            selectedDate(selDate.toDate());
            refreshData();
            popupDayInfoVisible(true);
        },
        hasSchedulePermission: function () {
            return VTPortal.roApp.empPermissions().Schedule.QuerySchedule;
        },
        lblDayInfo: i18nextko.i18n.t('roInformationDay'),
        noPunches: noPunches,
        scrollContent: {
        },
        tcSwitch: {
            width: 50,
        },
        scrollPopup: {},
        addPunch: {
            icon: 'plus',
            visible: true,
            onClick: function (e) {
                if (!block2Shown()) {
                    var e = document.getElementById('block2');
                    e.style.display = 'block';
                    block2Shown(true);
                }
                else {
                    if (!block3Shown()) {
                        var e = document.getElementById('block3');
                        e.style.display = 'block';
                        block3Shown(true);
                    }
                    else {
                        if (!block4Shown()) {
                            var e = document.getElementById('block4');
                            e.style.display = 'block';
                            block4Shown(true);
                        }
                        else {
                            if (!block5Shown()) {
                                var e = document.getElementById('block5');
                                e.style.display = 'block';
                                block5Shown(true);
                            }
                        }
                    }
                }
            }
        },
        btnSave: {
            onClick: saveRequest,
            text: '',
            hint: i18nextko.i18n.t('roSaveRequest'),
            icon: "Images/Common/save.png",
            visible: saveVisible
        },
        closeBlock2: {
            onClick: function (e) {
                var e = document.getElementById('block2');
                e.style.display = 'none';
                block2Shown(false);
                eDate2(null);
                iDate2(null);
            },
            text: '',
            icon: "remove",
            visible: delete2Shown
        },
        closeBlock3: {
            onClick: function (e) {
                var e = document.getElementById('block3');
                e.style.display = 'none';
                block3Shown(false);
                eDate3(null);
                iDate3(null);
            },
            text: '',
            icon: "remove",
            visible: delete3Shown
        },
        closeBlock4: {
            onClick: function (e) {
                var e = document.getElementById('block4');
                e.style.display = 'none';
                block4Shown(false);
                eDate4(null);
                iDate4(null);
            },
            text: '',
            icon: "remove",
            visible: delete4Shown
        },
        closeBlock5: {
            onClick: function (e) {
                var e = document.getElementById('block5');
                e.style.display = 'none';
                block5Shown(false);
                eDate5(null);
                iDate5(null);
            },
            text: '',
            icon: "remove",
            visible: true
        },
        savePunch: {
            text: i18nextko.i18n.t('roSaveLicense'),
            onClick: function (e) {
                var dateBox = $("#beginDate").dxDateBox("instance");
                var dateBoxValue = dateBox.option('value');

                var newPunch = new Object();

                newPunch.isNew = true;
                newPunch.IDEmployee = VTPortal.roApp.employeeId;
                newPunch.DateTime = moment(dateBoxValue).format(VTPortal.roApp.localFormat);
                newPunch.ShiftDate = moment(params.param, "YYYY-MM-DD").format(VTPortal.roApp.localFormat);

                if (reqValue1() == 'E') {
                    newPunch.cssClass = 'dx-icon-NewDailyRecordPunch';
                    newPunch.Date = moment.tz(dateBoxValue, VTPortal.roApp.serverTimeZone);
                    newPunch.Name = i18nextko.i18n.t('roEntry') + ': ' + moment.tz(dateBoxValue, VTPortal.roApp.serverTimeZone).format('HH:mm');
                    newPunch.Direction = 'E';
                    newPunch.Type = 1;
                }
                else {
                    newPunch.cssClass = 'dx-icon-NewDailyRecordPunch';
                    newPunch.Date = moment.tz(dateBoxValue, VTPortal.roApp.serverTimeZone);
                    newPunch.Name = i18nextko.i18n.t('roExit') + ': ' + moment.tz(dateBoxValue, VTPortal.roApp.serverTimeZone).format('HH:mm');
                    newPunch.Direction = 'S';
                    newPunch.Type = 2;
                }

                if (zoneSelected1() != null) {
                    newPunch.IDZone = zoneSelected1().Id;
                }

                newPunch.InTelecommute = false;

                if (tcTypeSelected1() != null) {
                    if (tcTypeSelected1().FieldValue == '1') {
                        newPunch.InTelecommute = true;
                    }
                }

                var newPunches = Punches();
                newPunches.push(newPunch);

                var currentPunches = punchesDS();
                currentPunches.push(newPunch);
                currentPunches.sortBy(function (o) { return new Date(o.Date) });
                punchesDS(currentPunches);
                saveVisible(true);
                popupNewPunchVisible(false);
            }
        },
        dateBegin2: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: iDate2,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: 'HH:mm',
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    iDate2(moment(eDate1()).clone().toDate());
                }
            }
        },
        dateEnd2: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: eDate2,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: "HH:mm",
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    eDate2(moment(iDate2()).clone().toDate());
                }
            }
        },
        cmbTCType2: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue2,
            onValueChanged: function (data) {
                tcTypeSelected2(data.component.option('selectedItem'));
            }
        },
        cmbZones2: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue2,
            onValueChanged: function (data) {
                zoneSelected2(data.component.option('selectedItem'));
            }
        },
        dateBegin3: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: iDate3,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: "HH:mm",
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    iDate3(moment(eDate2()).clone().toDate());
                }
            }
        },
        dateEnd3: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: eDate3,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: "HH:mm",
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    eDate3(moment(iDate3()).clone().toDate());
                }
            }
        },
        cmbTCType3: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue3,
            onValueChanged: function (data) {
                tcTypeSelected3(data.component.option('selectedItem'));
            }
        },
        cmbZones3: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue3,
            onValueChanged: function (data) {
                zoneSelected3(data.component.option('selectedItem'));
            }
        },
        dateBegin4: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: iDate4,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: "HH:mm",
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    iDate4(moment(eDate3()).clone().toDate());
                }
            }
        },
        dateEnd4: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: eDate4,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: "HH:mm",
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    eDate4(moment(iDate4()).clone().toDate());
                }
            }
        },
        cmbTCType4: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue4,
            onValueChanged: function (data) {
                tcTypeSelected4(data.component.option('selectedItem'));
            }
        },
        cmbZones4: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue4,
            onValueChanged: function (data) {
                zoneSelected4(data.component.option('selectedItem'));
            }
        },
        dateBegin5: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: iDate5,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: "HH:mm",
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    iDate5(moment(eDate4()).clone().toDate());
                }
            }
        },
        dateEnd5: {
            type: 'time',
            pickerType: VTPortalUtils.utils.dailyRecordDatetimeTypeSelect(),
            value: eDate5,
            width: (VTPortal.roApp.isModeApp() ? 75 : 95),
            valueChangeEvent: 'focusout',
            displayFormat: "HH:mm",
            onFocusIn: function (e) {
                if (!VTPortal.roApp.isModeApp() && e.element.find("input").val() == "") {
                    eDate5(moment(iDate5()).clone().toDate());
                }
            }
        },
        cmbTCType5: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue5,
            onValueChanged: function (data) {
                tcTypeSelected5(data.component.option('selectedItem'));
            }
        },
        cmbZones5: {
            dataSource: VTPortal.roApp.zonesDataSource,
            displayExpr: "Name",
            valueExpr: "Id",
            value: zoneValue5,
            onValueChanged: function (data) {
                zoneSelected5(data.component.option('selectedItem'));
            }
        },
        block1Invalid: block1Invalid,
        block2Invalid: block2Invalid,
        block3Invalid: block3Invalid,
        block4Invalid: block4Invalid,
        block5Invalid: block5Invalid,
        thereIsNighttime: thereIsNighttime,
        blockInvalidText: i18nextko.i18n.t('invalidPeriod'),
        blockNTText: i18nextko.i18n.t('blockNTText'),
        lblRemaining: lblRemaining
    };

    return viewModel;
};