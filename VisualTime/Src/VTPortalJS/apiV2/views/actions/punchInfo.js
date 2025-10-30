VTPortal.punchInfo = function (params) {
    var optionsVisible = ko.observable(false);
    var tcType = ko.observable('');
    var idCause = ko.observable(-1);
    var pProgressLocVisible = null;
    var pQuickPunch = null;
    var punchType = ko.observable('NoCause');
    var punchDirection = ko.observable('E');
    var telecommutingOptions = ko.computed(function () {
        var ds = []

        ds.push({ text: i18nextko.i18n.t('roOffice'), type: 'success', cssClassText: 'textPunchOption', ImageSrc: "Images/icons8-office-48.png" });
        ds.push({ text: i18nextko.i18n.t('roHome'), type: 'success', cssClassText: 'textPunchOption', ImageSrc: "Images/icons8-home-48.png" });

        return ds;
    });
    var requieresCause = ko.observable(false);

    var initView = function (quickPunch, progressLocVisible) {
        pQuickPunch = quickPunch;
        pProgressLocVisible = progressLocVisible;

        idCause(-1);
        punchType('NoCause');
        punchDirection('E');
        requieresCause(false);
        VTPortal.roApp.SelectedZone(-1);
        VTPortal.roApp.ReliableZone(true);
    }

    var onZoneClick = function (e) {
        VTPortal.roApp.popupZoneVisible(false);
        VTPortal.roApp.ZoneSelected(true);
        VTPortal.roApp.SelectedZone(e.itemData.Id);
        VTPortal.roApp.ReliableZone(false);
        askForCauseIfNeeded();
    }

    var onCauseClick = function (e) {
        requieresCause(false);
        VTPortal.roApp.popupCauseVisible(false);
        idCause(e.itemData.ID);
        askForCauseIfNeeded();
    }

    var reloadMap = function (onEndCallback) {
        if (VTPortal.roApp.ZoneSelected()) {
            if (typeof onEndCallback != 'undefined')
                onEndCallback();
        }
        else {
            if (VTPortal.roApp.empPermissions().MustUseGPS) {
                progressLocVisible(true);

                var onReloadCallback = function (progressLocVisible, onEndCallback) {
                    return function () {
                        progressLocVisible(false);
                        if (typeof onEndCallback != 'undefined' && onEndCallback != null) onEndCallback();
                    }
                }

                VTPortalUtils.utils.reloadLocation(onReloadCallback(progressLocVisible, onEndCallback));
            } else {
                progressLocVisible(false);
                if (typeof onEndCallback != 'undefined') onEndCallback();
            }
        }
    }

    var commitNFCPunch = function (params, onEndPunchCallback) {
        window.VTPortalUtils.utils.doPunch('ta', params, onEndPunchCallback);
    }

    var commitManualPunch = function (punchTypeId) {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.NewRequests) {
            switch (punchTypeId) {
                case 0:
                    if (!VTPortal.roApp.lastRequestFailed()) {
                        punchDirection('E');
                        punchType("NoCause");
                        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userTelecommute() != null && VTPortal.roApp.userTelecommute().Telecommuting) {
                            showTelecommuteOptionsIfNeeded();
                        } else {
                            askForZoneIfNeeded();
                        }
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                    }

                    break;
                case 1:
                    if (!VTPortal.roApp.lastRequestFailed()) {
                        punchDirection('S');
                        finallyCommitPunchToServer();
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                    }
                    break;
                case 2:
                    if (!VTPortal.roApp.lastRequestFailed()) {
                        punchDirection('E');
                        punchType("Cause");
                        requieresCause(true);

                        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userTelecommute() != null && VTPortal.roApp.userTelecommute().Telecommuting) {
                            showTelecommuteOptionsIfNeeded();
                        } else {
                            askForZoneIfNeeded();
                        }
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                    }
                    break;
                case 3:
                    if (!VTPortal.roApp.lastRequestFailed()) {
                        requieresCause(true);
                        punchDirection('S');
                        askForCauseIfNeeded();
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                    }

                    break;
            }
        }
        else {
            switch (punchTypeId) {
                case 0:
                    if (!VTPortal.roApp.lastRequestFailed()) {
                        punchDirection('E');
                        punchType("NoCause");
                        requieresCause(false);

                        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userTelecommute() != null && VTPortal.roApp.userTelecommute().Telecommuting) {
                            showTelecommuteOptionsIfNeeded();
                        } else {
                            askForCauseIfNeeded();
                        }
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                    }

                    break;
                case 1:
                    if (!VTPortal.roApp.lastRequestFailed()) {
                        punchDirection('S');
                        punchType("NoCause");
                        requieresCause(false);
                        askForCauseIfNeeded();
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                    }
                    break;
                case 2:
                    if (!VTPortal.roApp.lastRequestFailed()) {
                        punchDirection('E');
                        punchType("Cause");
                        requieresCause(true);

                        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userTelecommute() != null && VTPortal.roApp.userTelecommute().Telecommuting) {
                            showTelecommuteOptionsIfNeeded();
                        } else {
                            askForCauseIfNeeded();
                        }
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                    }
                    break;
                case 3:
                    if (!VTPortal.roApp.lastRequestFailed()) {
                        punchDirection('S');
                        punchType("Cause");
                        requieresCause(true);
                        askForCauseIfNeeded();
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                    }

                    break;
            }
        }
    };

    var commitQuickPunch = function () {
        idCause(-1);
        punchType('NoCause');
        requieresCause(false);
        VTPortal.roApp.SelectedZone(-1);
        VTPortal.roApp.ReliableZone(true);

        if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
            punchDirection("S");
        } else {
            punchDirection("E");
        }

        var onEnQuickPunchCallback = function () {
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting) {
                if (VTPortal.roApp.userTelecommute() != null && VTPortal.roApp.userTelecommute().Telecommuting) {
                    if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                        finallyCommitPunchToServer();
                    } else {
                        showTelecommuteOptionsIfNeeded();
                    }
                } else {
                    if (VTPortal.roApp.db.settings.ApiVersion < VTPortalUtils.apiVersion.NewRequests)
                        finallyCommitPunchToServer();
                    else {
                        if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                            finallyCommitPunchToServer();
                        }
                        else
                            askForZoneIfNeeded();
                    }
                }
            } else {
                finallyCommitPunchToServer();
            }
        }

        /* Aquí comprobar si el se tiene que mostrar telecomute*/
        if (typeof pQuickPunch != 'undefined') pQuickPunch(reloadMap(onEnQuickPunchCallback));
        else {
            onEnQuickPunchCallback();
        }
    }

    var showTelecommuteOptionsIfNeeded = function () {
        let currentZone = VTPortal.roApp.currentLocationInfo().zone;
        let currentZoneTelecommutingType = null;

        if (currentZone != null)
            currentZoneTelecommutingType = VTPortal.roApp.ServerZones().filter(zone => zone.Id == currentZone)[0].TelecommutingZoneType;

        if (VTPortal.roApp.isTimeGate() && VTPortal.roApp.timeGateConfig  && VTPortal.roApp.timeGateConfig.InZone > 0) {
            currentZoneTelecommutingType = VTPortal.roApp.ServerZones().filter(zone => zone.Id == VTPortal.roApp.timeGateConfig.InZone)[0].TelecommutingZoneType;
            currentZone = -1;
        }

        if (punchDirection() == 'E' && (currentZone == null || currentZoneTelecommutingType == 2))
            optionsVisible(true);
        else {
            if (currentZoneTelecommutingType != null) tcType(currentZoneTelecommutingType);
            askForZoneIfNeeded();
        }


        
    }

    var onTelecommuteOptionSelected = function (s, e) {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.NewRequests) {
            if (e.itemIndex == 0) {
                tcType(0);
                askForZoneIfNeeded();
            } else {
                tcType(1);
                askForCauseIfNeeded();
            }
        } else {
            e.itemIndex == 0 ? tcType(0) : tcType(1);
            askForCauseIfNeeded();
        }
    }

    var askForZoneIfNeeded = function () {
        var location = Object.clone(VTPortal.roApp.currentLocationInfo(), true);

        if (location.zone == null) {
            if ((!location.reliable && VTPortal.roApp.empPermissions().MustSelectZone && VTPortal.roApp.empPermissions().MustUseGPS) || ((VTPortal.roApp.empPermissions().MustSelectZone && !VTPortal.roApp.empPermissions().MustUseGPS))) {
                if (!VTPortal.roApp.empPermissions().MustUseGPS) {
                    VTPortal.roApp.loadZones();
                    VTPortal.roApp.popupZoneVisible(true);
                }
                else {
                    VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('errorGettingGPSNew'), 'info', 0, function () {
                        VTPortal.roApp.loadZones();
                        VTPortal.roApp.popupZoneVisible(true);
                    }, function () {
                        VTPortal.roApp.redirectAtHome();
                    }, i18nextko.i18n.t('roSelectZone'), i18nextko.i18n.t('roCancel'));
                }
            } else askForCauseIfNeeded();
        }
        else {
            VTPortal.roApp.SelectedZone(location.zone);
            VTPortal.roApp.ReliableZone(true);
            askForCauseIfNeeded();
        }
    }

    var askForCauseIfNeeded = function () {
        if (requieresCause()) {
            VTPortal.roApp.popupCauseVisible(true);
        } else {
            finallyCommitPunchToServer();
        }
    }

    var finallyCommitPunchToServer = function () {
        if (punchDirection() == "E") {
            if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                VTPortalUtils.utils.isShowingPopup(false);
                VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                    window.VTPortalUtils.utils.doPunch('ta', { idCause: idCause(), direction: punchDirection(), tcType: tcType() });
                }, function () {
                }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
            } else {
                window.VTPortalUtils.utils.doPunch('ta', { idCause: idCause(), direction: punchDirection(), tcType: tcType() });
            }
        } else if (punchDirection() == "S") {
            if (VTPortal.roApp.userStatus().PresenceStatus == "Outside") {
                VTPortalUtils.utils.isShowingPopup(false);
                VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyOut'), 'warning', 0, function () {
                    window.VTPortalUtils.utils.doPunch('ta', { idCause: idCause(), direction: punchDirection(), tcType: tcType() });
                }, function () {
                }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
            } else {
                window.VTPortalUtils.utils.doPunch('ta', { idCause: idCause(), direction: punchDirection(), tcType: tcType() });
            }
        }
    }

    var progressLocVisible = function (bVisible) {
        if (pProgressLocVisible != null) pProgressLocVisible(bVisible);
    }

    var viewModel = {
        OptionsVisible: optionsVisible,
        TcType: tcType,
        CommitQuickPunch: commitQuickPunch,
        CommitManualPunch: commitManualPunch,
        CommitNFCPunch: commitNFCPunch,
        ReloadMap: reloadMap,
        InitView: initView,
        ListZonesVisible: VTPortal.roApp.popupZoneVisible,
        ListCausesVisible: VTPortal.roApp.popupCauseVisible,
        lblWhereAreYou: i18nextko.t('lblWhereAreYou'),
        lblCauseInput: i18nextko.t('lblCauseInput'),
        searchOptions: {
            valueChangeEvent: "keyup",
            mode: "search",
            height: "10%",
            onValueChanged: function (args) {
                VTPortal.roApp.zonesDataSource().searchValue(args.value);
                VTPortal.roApp.zonesDataSource().load();
            }
        },
        causeSearchOptions: {
            valueChangeEvent: "keyup",
            mode: "search",
            height: "10%",
            onValueChanged: function (args) {
                VTPortal.roApp.causesDataSource().searchValue(args.value);
                VTPortal.roApp.causesDataSource().load();
            }
        },
        listCauses: {
            dataSource: VTPortal.roApp.causesDataSource,
            scrollingEnabled: true,
            height: 300,
            grouped: false,
            itemTemplate: 'RequestItem',
            /*onItemClick: function (e) {
                alert("doem")
            }*/
            onItemClick: onCauseClick
        },
        listZones: {
            dataSource: VTPortal.roApp.zonesDataSource,
            scrollingEnabled: true,
            height: 300,
            grouped: false,
            itemTemplate: 'RequestItem',
            /*onItemClick: function (e) {
                alert("doem")
            }*/
            onItemClick: onZoneClick
        },
        punchOptions: {
            dataSource: telecommutingOptions,
            visible: optionsVisible,
            showTitle: false,
            title: '¿Desde dónde trabajarás hoy?',
            showCancelButton: ko.observable(false),
            itemTemplate: function (data, index, element) {
                if (index == 0) {
                    var listItem = $('<div>').attr('class', 'listMenuItemContentPunchOptions');
                    listItem.append($('<img>').attr('src', data.ImageSrc).attr('class', 'listMenuItemIcon EmployeeImagePhoto'));
                    listItem.append($('<div>').attr('class', data.cssClassText).text(data.text));

                    element.append(listItem);
                }
                else {
                    var listItem = $('<div>').attr('class', 'listMenuItemContentPunchOptions');
                    listItem.append($('<img>').attr('src', data.ImageSrc).attr('class', 'listMenuItemIcon EmployeeImagePhoto'));
                    listItem.append($('<div>').attr('class', data.cssClassText).text(data.text));

                    element.append(listItem);
                }
            },
            onItemClick: function (e) {
                onTelecommuteOptionSelected(null, e);
            }
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};