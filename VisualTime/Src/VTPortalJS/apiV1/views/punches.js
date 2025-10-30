VTPortal.punches = function (params) {
    var curPunchAction = ko.observable('presence');
    var selectedIndex = ko.observable(0);
    var actionDS = ko.observable(false);
    var punchesPanels = ko.observable([]);
    var positionreliable = ko.computed(function () { return VTPortal.roApp.currentLocationInfo().reliable; });
    var mustUseGps = ko.computed(function () { return VTPortal.roApp.empPermissions().MustUseGPS; });
    var refreshTimmer = -1;
    var taskFieldsVisible = ko.observable(false);
    var taskFieldsBlock = VTPortal.taskFieldsInput();
    var curTaskUrl = '';
    var lstActions = ko.observable([]);
    var optionsVisible = ko.observable(false);

    var taskAlertVisible = ko.observable(false);
    var lblSaveTaskAlertTitle = ko.observable('');
    var valueTaskAlert = ko.observable('');
    var punchType = ko.observable('');

    var captchaVisible = ko.observable(false);
    var actionCaptcha = ko.observable('');
    var captchaValue = ko.observable(Math.floor(Math.random() * 100001));
    var tcType = ko.observable('');

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: [],
        searchOperation: "contains",
        searchExpr: "Name"
    }));

    // if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) tcType(params.id);

    var quickPunch = function () {
        if (punchType() == "NoCause") {
            if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                VTPortalUtils.utils.isShowingPopup(false);
                VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                    window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'S', tcType: tcType(), selectedZone: VTPortal.roApp.SelectedZone() });
                }, function () {
                }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
            } else {
                window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E', tcType: tcType(), selectedZone: VTPortal.roApp.SelectedZone() });
            }
        }
        else {
            if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                VTPortalUtils.utils.isShowingPopup(false);
                VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                    VTPortal.app.navigate("SelectListObject/causes.readerinputcode/S" + '/' + tcType());
                }, function () {
                }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
            } else {
                VTPortal.app.navigate("SelectListObject/causes.readerinputcode/E" + '/' + tcType());
            }
        }
    };

    var telecommutingOptions = ko.computed(function () {
        var ds = []

        ds.push({ text: i18nextko.i18n.t('roOffice'), type: 'success', cssClassText: 'textPunchOption', ImageSrc: "Images/icons8-office-48.png" });
        ds.push({ text: i18nextko.i18n.t('roHome'), type: 'success', cssClassText: 'textPunchOption', ImageSrc: "Images/icons8-home-48.png" });

        return ds;
    });

    var punchTitle = ko.computed(function () {
        switch (curPunchAction()) {
            case 'presence':
                return i18nextko.i18n.t('roATPuncheTitle');
                break;
            case 'productiv':
                return i18nextko.i18n.t('roTPunchesTitle');
                break;
            case 'costcenter':
                return i18nextko.i18n.t('roCCPunchesTitle');
                break;
        }
    }, this);

    function generateactions(tab) {
        var lstactions = [];
        switch (tab) {
            case 0:
                break;

            case 1:
                if (VTPortal.roApp.empPermissions().Punch.SchedulePunch && VTPortal.roApp.userStatus().PunchesEnabled) {
                    lstactions.push({
                        ID: 0,
                        cssClass: 'dx-icon-TA_in',
                        cssClassText: 'textActionPunch',
                        Action: i18nextko.i18n.t('roPunches_TA_in'),
                    });

                    lstactions.push({
                        ID: 1,
                        cssClass: 'dx-icon-TA_out',
                        cssClassText: 'textActionPunch',
                        Action: i18nextko.i18n.t('roPunches_TA_out'),
                    });

                    lstactions.push({
                        ID: 2,
                        cssClass: 'dx-icon-TA_in_cause',
                        cssClassText: 'textActionPunch',
                        Action: i18nextko.i18n.t('roPunches_TA_in_cause'),
                    });

                    lstactions.push({
                        ID: 3,
                        cssClass: 'dx-icon-TA_out_cause',
                        cssClassText: 'textActionPunch',
                        Action: i18nextko.i18n.t('roPunches_TA_out_cause'),
                    });
                }
                break;
            case 2:
                if (VTPortal.roApp.empPermissions().Punch.ProductiVPunch && VTPortal.roApp.userStatus().ProductiVEnabled) {
                    lstactions.push({
                        ID: 4,
                        cssClass: 'dx-icon-Task_change',
                        cssClassText: 'textActionPunch',
                        Action: i18nextko.i18n.t('roPunches_Task_change'),
                    });
                    if (VTPortal.roApp.userStatus().HasCompletePermission && VTPortal.roApp.userStatus().TaskId > 0) {
                        lstactions.push({
                            ID: 5,
                            cssClass: 'dx-icon-Task_complete',
                            cssClassText: 'textActionPunch',
                            Action: i18nextko.i18n.t('roPunches_Task_complete'),
                        });
                    }
                    if (VTPortal.roApp.userStatus().TaskId > 0) {
                        lstactions.push({
                            ID: 6,
                            cssClass: 'dx-icon-Task_alert',
                            cssClassText: 'textActionPunch',
                            Action: i18nextko.i18n.t('roPunches_Task_alert'),
                        });
                    }
                }
                break;
            case 3:
                if (VTPortal.roApp.empPermissions().Punch.CostCenterPunch && VTPortal.roApp.userStatus().PunchesEnabled && VTPortal.roApp.userStatus().CostCenterEnabled) {
                    lstactions.push({
                        ID: 7,
                        cssClass: 'dx-icon-CostCenter',
                        cssClassText: 'textActionPunch',
                        Action: i18nextko.i18n.t('roPunches_CostCenter'),
                    });
                }
                break;
        }

        if (lstactions.length > 0) {
            actionDS(true);
            lstActions(lstactions);
        }
        else {
            actionDS(false);
        }

        return lstactions;
    };

    if ((VTPortal.roApp.empPermissions().Punch.SchedulePunch && VTPortal.roApp.userStatus().PunchesEnabled) ||
        (VTPortal.roApp.empPermissions().Punch.ProductiVPunch && VTPortal.roApp.userStatus().ProductiVEnabled) ||
        (VTPortal.roApp.empPermissions().Punch.CostCenterPunch && VTPortal.roApp.userStatus().PunchesEnabled && VTPortal.roApp.userStatus().CostCenterEnabled)) {
        var tmpPanels = []

        //Añadimos cada tab y botón en función de los permisos del empleado para fichar
        if (VTPortal.roApp.empPermissions().Punch.SchedulePunch && VTPortal.roApp.userStatus().PunchesEnabled) {
            tmpPanels.push({ "ID": 1, "cssClass": "dx-icon-presence" });
        }

        if (VTPortal.roApp.empPermissions().Punch.ProductiVPunch && VTPortal.roApp.userStatus().ProductiVEnabled) {
            tmpPanels.push({ "ID": 2, "cssClass": "dx-icon-productiV" });
        }

        if (VTPortal.roApp.empPermissions().Punch.CostCenterPunch && VTPortal.roApp.userStatus().PunchesEnabled && VTPortal.roApp.userStatus().CostCenterEnabled) {
            tmpPanels.push({ "ID": 3, "cssClass": "dx-icon-costCenter" });
        }

        punchesPanels(tmpPanels);
    } else {
        punchesPanels([]);
    }

    var changeTab = function (item) {
        if (item.ID == 1) curPunchAction('presence');
        else if (item.ID == 2) curPunchAction('productiv');
        else if (item.ID == 3) curPunchAction('costcenter');
        reloadMap();
        setTimeout(function () { $('.scrollview').each(function () { $(this).height($('#panelsContent').height() - 75); }); }, 200);
    }

    var reloadMap = function () {
        if (VTPortal.roApp.gpsEmployeePermission()) {
            if (VTPortal.roApp.empPermissions().MustUseGPS) {
                navigator.geolocation.getCurrentPosition(
                    function (position) {
                        var location = VTPortal.roApp.currentLocationInfo();

                        //Solo hacemos la petición a la api de google si se ha modificado la posición.
                        if (location.coords[0] != position.coords.latitude || location.coords[1] != position.coords.longitude || location.reliable == false || location.location == "-") {
                            geocoder = new google.maps.Geocoder();
                            geocoder.geocode({ 'address': position.coords.latitude + ',' + position.coords.longitude, 'language': 'es' }, function (results, status) {
                                var location = VTPortal.roApp.currentLocationInfo();
                                if (status == google.maps.GeocoderStatus.OK) {
                                    for (i = 0; i < results[0].address_components.length; i++) {
                                        if (results[0].address_components[i].types[0] == 'locality') {
                                            try { location.location = results[0].address_components[i].long_name; } catch (e) { location.location = "API Error parsing city"; }
                                        }
                                    }
                                    try { location.fullAddress = results[0].formatted_address; } catch (e) { location.fullAddress = "API Error parsing address"; }

                                    location.coords = [position.coords.latitude, position.coords.longitude];
                                    location.reliable = true;
                                    VTPortal.roApp.currentLocationInfo(location);
                                } else {
                                    var location = VTPortal.roApp.currentLocationInfo();
                                    location.coords[0] = 0; location.coords[1] = 0;
                                    location.location = "API Error parsing city";
                                    location.fullAddress = "API Error parsing address";
                                    location.reliable = false;

                                    VTPortal.roApp.currentLocationInfo(location);
                                }
                            });
                        }
                    },
                    function onError(error) {
                        if (error.code == 1) { //PositionError.PERMISSION_DENIED
                            var location = VTPortal.roApp.currentLocationInfo();
                            location.coords[0] = 0; location.coords[1] = 0;
                            location.location = "API Error parsing city";
                            location.fullAddress = "API Error parsing address";
                            location.reliable = false;
                            VTPortal.roApp.gpsEmployeePermission(false);
                        } else {
                            if (error.message.indexOf("Only secure origins are allowed") == 0) {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('chromeGeolocationAccessForbidden'), 'error', 0);
                            } else {
                                var location = VTPortal.roApp.currentLocationInfo();
                                location.coords[0] = 0; location.coords[1] = 0;
                                location.location = "API Error parsing city";
                                location.fullAddress = "API Error parsing address";
                                location.reliable = false;

                                VTPortal.roApp.currentLocationInfo(location);
                                VTPortal.roApp.gpsEmployeePermission(true);
                            }
                        }
                    }, { timeout: 10000 });
            }
        }
    }

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function tagDetected(nfcEvent) {
        //VTPortalUtils.utils.notifyMesage('¡NFC DETECTADO! ' + ndef.textHelper.decodePayload(nfcEvent.tag.ndefMessage[0].payload), 'success', 2000);
        if (VTPortal.app.navigationManager._currentItem.uri == 'punches') {
            if (typeof nfcEvent.tag.ndefMessage[0] != 'undefined') {
                if (VTPortal.roApp.nfcPunchInProgress() == false) {
                    VTPortal.roApp.nfcPunchInProgress(true);
                    //VTPortalUtils.utils.notifyMesage('¡NFC DETECTADO! ' + ndef.textHelper.decodePayload(nfcEvent.tag.ndefMessage[0].payload), 'success', 2000);

                    var onEndPunchCallback = function () {
                        if (cordova.platformId === 'ios') {
                            nfc.invalidateSession();
                        }
                        VTPortal.roApp.nfcEnabled(false);
                        VTPortal.roApp.nfcPunchInProgress(false);
                    }

                    if (!VTPortal.roApp.lastRequestFailed()) {
                        window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'X', fast: false, nfcTag: ndef.textHelper.decodePayload(nfcEvent.tag.ndefMessage[0].payload), tcType: tcType() }, onEndPunchCallback);
                    }
                }
            }
            else {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roInvalidNFC'), 'warning', 2000);
            }
        }
    };

    function startNFCListener() {
        nfc.addNdefListener(
            tagDetected,
            function () {
                VTPortal.roApp.nfcEnabled(true);
                //VTPortalUtils.utils.notifyMesage('¡Se ha activado el NFC!', 'info', 2000);
            },
            function () {
                VTPortal.roApp.nfcEnabled(false);

                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roErrorNFC'), 'warning', 2000);
            }
        );
    };

    function errorIos(error) {
        VTPortal.roApp.nfcEnabled(false);
        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roErrorNFC'), 'warning', 2000);
    };

    function nfcIosEnabled() {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
            if (typeof nfc != 'undefined') {
                if (cordova.platformId === 'ios') {
                    //nfc.enabled(onSuccess, onFailure);
                    //nfc.beginSession();
                    //startNFCListener();
                    nfc.beginSession(startNFCListener, errorIos);
                }
            }
        }
    }
    //function onSuccess() {
    //    VTPortalUtils.utils.notifyMesage('el nfc funciona en ios', 'info', 10000);
    //}

    //function onFailure(error) {
    //    VTPortalUtils.utils.notifyMesage(error.toString(), 'warning', 10000);
    //}

    var onZoneClick = function (e) {
        VTPortal.roApp.popupZoneVisible(false);
        VTPortal.roApp.ZoneSelected(true);
        VTPortal.roApp.SelectedZone(e.itemData.Id);
        quickPunch();
    }
    function viewShown() {
        //generateactions(1);

        VTPortal.roApp.nfcPunchInProgress(false);
        if (VTPortal.roApp.isModeApp()) {
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
                if (typeof nfc != 'undefined') {
                    if (cordova.platformId === 'ios') {
                        //nfc.enabled(onSuccess, onFailure);
                        //nfc.beginSession();
                        //startNFCListener();
                        //nfc.beginSession(startNFCListener, errorIos);
                    }
                    else {
                        startNFCListener();
                    }
                }
            }
        }
        globalStatus().viewShown();
        reloadMap();
        setTimeout(function () { $('.scrollview').each(function () { $(this).height($('#panelsContent').height() - 75); }); }, 200);
        if (refreshTimmer == -1) refreshTimmer = setInterval(function () { reloadMap(); }, 10000);
    }

    var viewHidden = function () {
        clearInterval(refreshTimmer);
        refreshTimmer = -1;
    }

    var validateCaptcha = function (params) {
        var result = params.validationGroup.validate();
        if (result.isValid) {
            captchaVisible(false);
            doTaskChecksBeforePunch("SelectListObject/tasks.availabletasks/TCO", VTPortalUtils.taskAction.complete);
        }
    }

    var doTaskChecksBeforePunch = function (url, action) {
        curTaskUrl = url;

        VTPortal.roApp.taskPunchRequest({
            currentAction: -1,
            newAction: -1,
            currentTaskUserFieldsDefinition: null,
            currentTaskUserFieldsValue: ['', '', '', -1, -1, -1],
            newTaskUserFieldsDefinition: null,
            newTaskUserFieldsValue: ['', '', '', -1, -1, -1]
        });

        if (VTPortal.roApp.userStatus().TaskId > 0) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.taskPunchRequest().currentAction = action;
                    VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition = result.Fields;

                    var bAskInfo = false;
                    for (var i = 0; i < result.Fields.length; i++) {
                        if (result.Fields[i].Used) {
                            bAskInfo = true;
                        }
                    }

                    if (!bAskInfo) {
                        VTPortal.app.navigate(url);
                    } else {
                        taskFieldsBlock.initTaskProperties(false);
                        taskFieldsVisible(true);
                    }
                } else {
                    var onContinue = function () {
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingTaskInfo"), 'error', 0, onContinue);
                }
            }).getTaskUserFieldsAction(action, VTPortal.roApp.userStatus().TaskId);
        } else {
            VTPortal.app.navigate(url);
        }
    }
    var continuePunch = function () {
        taskFieldsVisible(false);
        taskFieldsBlock.savePunchInfo();

        VTPortal.app.navigate(curTaskUrl);
    }

    var saveTaskAlert = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtHome();
            } else {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }).saveNewTaskAlert(VTPortal.roApp.userStatus().TaskId, valueTaskAlert());
    }

    var viewModel = {
        title: punchTitle,
        viewShown: viewShown,
        viewHidden: viewHidden,
        lblCaptcha: i18nextko.t('roCaptchaTitle'),
        lblCaptchaDescription: i18nextko.t('roCaptchaDescription'),
        lblCaptchaValue: captchaValue,
        subscribeBlock: globalStatus(),
        positionreliable: positionreliable,
        mustUseGps: mustUseGps,
        tabPanelOptions: {
            height: "85%",
            dataSource: punchesPanels,
            selectedIndex: selectedIndex,
            animationEnabled: false,
            swipeEnabled: false,
            itemTitleTemplate: "title",
            onSelectionChanged: function (e) {
                changeTab(e.addedItems[0]);
                switch (e.addedItems[0].ID) {
                    case 1:
                        generateactions(1);
                        break;
                    case 2:
                        generateactions(2);
                        break;
                    case 3:
                        generateactions(3);
                        break;
                }
            }
        },
        commScroll: {
            height: '70%'
        },

        listactions: {
            dataSource: lstActions,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'actionItem',
            onItemClick: function (info) {
                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.NewRequests) {
                    switch (info.itemData.ID) {
                        case 0:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userStatus().Telecommuting == true) {
                                    punchType("NoCause");
                                    optionsVisible(true);
                                }
                                else {
                                    punchType("NoCause");
                                    if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                        VTPortalUtils.utils.isShowingPopup(false);
                                        VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                                            window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E' });
                                        }, function () {
                                        }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                                    } else {
                                        window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E' });
                                    }
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }

                            break;
                        case 1:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Outside") {
                                    VTPortalUtils.utils.isShowingPopup(false);
                                    VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyOut'), 'warning', 0, function () {
                                        window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'S' });
                                    }, function () {
                                    }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                                } else {
                                    window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'S' });
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }
                            break;
                        case 2:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userStatus().Telecommuting == true) {
                                    punchType("Cause");
                                    optionsVisible(true);
                                }
                                else {
                                    if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                        VTPortalUtils.utils.isShowingPopup(false);
                                        VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                                            VTPortal.app.navigate("SelectListObject/causes.readerinputcode/E");
                                        }, function () {
                                        }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                                    } else {
                                        VTPortal.app.navigate("SelectListObject/causes.readerinputcode/E");
                                    }
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }
                            break;
                        case 3:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Outside") {
                                    VTPortalUtils.utils.isShowingPopup(false);
                                    VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyOut'), 'warning', 0, function () {
                                        VTPortal.app.navigate("SelectListObject/causes.readerinputcode/S");
                                    }, function () {
                                    }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                                } else {
                                    VTPortal.app.navigate("SelectListObject/causes.readerinputcode/S");
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }

                            break;
                        case 4:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                    if (VTPortal.roApp.userStatus().TaskId > 0) {
                                        doTaskChecksBeforePunch("SelectListObject/tasks.availabletasks/TCH", VTPortalUtils.taskAction.change);
                                    } else {
                                        doTaskChecksBeforePunch("SelectListObject/tasks.availabletasks/TCH", VTPortalUtils.taskAction.begin);
                                    }
                                } else {
                                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoPresence'), 'info', 0);
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }
                            break;
                        case 5:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                    if (VTPortal.roApp.userStatus().CanCompleteTask) {
                                        captchaValue(Math.floor(Math.random() * 100001));
                                        captchaVisible(true);
                                    } else {
                                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoPresence'), 'info', 0);
                                    }
                                } else {
                                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoPresence'), 'info', 0);
                            }
                            break;
                        case 6:
                            lblSaveTaskAlertTitle(i18nextko.i18n.t('roCurrentTaskAlert') + VTPortal.roApp.userStatus().TaskTitle);
                            taskAlertVisible(true);
                            break;
                        case 7:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                    VTPortal.app.navigate("SelectListObject/costcenters/C");
                                } else {
                                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoPresence'), 'info', 0);
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }
                            break;
                        case 8:
                            //if (!VTPortal.roApp.lastRequestFailed()) {
                            //    if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                            //        VTPortalUtils.utils.isShowingPopup(false);
                            //        VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                            //            window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E', tcType: tcType() });
                            //        }, function () {
                            //        }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                            //    } else {
                            //        window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E', tcType: tcType() });
                            //    }

                            //} else {
                            //    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            //}
                            break;
                    }
                }
                else {
                    switch (info.itemData.ID) {
                        case 0:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userStatus().Telecommuting == true) {
                                    punchType("NoCause");
                                    optionsVisible(true);
                                }
                                else {
                                    if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                        VTPortalUtils.utils.isShowingPopup(false);
                                        VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                                            window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E' });
                                        }, function () {
                                        }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                                    } else {
                                        window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E' });
                                    }
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }

                            break;
                        case 1:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Outside") {
                                    VTPortalUtils.utils.isShowingPopup(false);
                                    VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyOut'), 'warning', 0, function () {
                                        window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'S' });
                                    }, function () {
                                    }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                                } else {
                                    window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'S' });
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }
                            break;
                        case 2:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userStatus().Telecommuting == true) {
                                    punchType("Cause");
                                    optionsVisible(true);
                                }
                                else {
                                    if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                        VTPortalUtils.utils.isShowingPopup(false);
                                        VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                                            VTPortal.app.navigate("SelectListObject/causes.readerinputcode/E");
                                        }, function () {
                                        }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                                    } else {
                                        VTPortal.app.navigate("SelectListObject/causes.readerinputcode/E");
                                    }
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }
                            break;
                        case 3:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Outside") {
                                    VTPortalUtils.utils.isShowingPopup(false);
                                    VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyOut'), 'warning', 0, function () {
                                        VTPortal.app.navigate("SelectListObject/causes.readerinputcode/S");
                                    }, function () {
                                    }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                                } else {
                                    VTPortal.app.navigate("SelectListObject/causes.readerinputcode/S");
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }

                            break;
                        case 4:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                    if (VTPortal.roApp.userStatus().TaskId > 0) {
                                        doTaskChecksBeforePunch("SelectListObject/tasks.availabletasks/TCH", VTPortalUtils.taskAction.change);
                                    } else {
                                        doTaskChecksBeforePunch("SelectListObject/tasks.availabletasks/TCH", VTPortalUtils.taskAction.begin);
                                    }
                                } else {
                                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoPresence'), 'info', 0);
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }
                            break;
                        case 5:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                    if (VTPortal.roApp.userStatus().CanCompleteTask) {
                                        captchaValue(Math.floor(Math.random() * 100001));
                                        captchaVisible(true);
                                    } else {
                                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoPresence'), 'info', 0);
                                    }
                                } else {
                                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoPresence'), 'info', 0);
                            }
                            break;
                        case 6:
                            lblSaveTaskAlertTitle(i18nextko.i18n.t('roCurrentTaskAlert') + VTPortal.roApp.userStatus().TaskTitle);
                            taskAlertVisible(true);
                            break;
                        case 7:
                            if (!VTPortal.roApp.lastRequestFailed()) {
                                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                                    VTPortal.app.navigate("SelectListObject/costcenters/C");
                                } else {
                                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoPresence'), 'info', 0);
                                }
                            } else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            }
                            break;
                        case 8:
                            //if (!VTPortal.roApp.lastRequestFailed()) {
                            //    if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                            //        VTPortalUtils.utils.isShowingPopup(false);
                            //        VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                            //            window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E', tcType: tcType() });
                            //        }, function () {
                            //        }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                            //    } else {
                            //        window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E', tcType: tcType() });
                            //    }

                            //} else {
                            //    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
                            //}
                            break;
                    }
                }
            }
        },
        progressPanel: {
            message: i18nextko.t('roNFCPunch'),
            showPane: true,
            shading: false,
            height: 150,
            delay: 200,
            shadingColor: 'rgba(0,0,0,0)',
            indicatorSrc: 'Images/loader_v4.gif',
            visible: VTPortal.roApp.showNfcPanel
        },
        captchaVisible: captchaVisible,
        txtCaptcha: {
            placeholder: i18nextko.t('roCaptchaTitlePH'),
            value: actionCaptcha
        },
        actionDS: actionDS,
        btnAcceptCptcha: {
            validationGroup: 'validateCaptcha',
            text: i18nextko.t('roAcceptCaptcha'),
            onClick: validateCaptcha
        },
        btnCancelCaptcha: {
            onClick: function () { captchaVisible(false) },
            text: i18nextko.t('roCancelCaptcha'),
        },
        compareField: {
            validationGroup: 'validateCaptcha',
            validationRules: [{
                type: 'compare', comparisonType: '==', comparisonTarget: captchaValue, message: i18nextko.i18n.t('roCaptchaNotMatch')
            }]
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
                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.NewRequests) {
                    if (e.itemIndex == 0) {
                        tcType(0);
                        var location = Object.clone(VTPortal.roApp.currentLocationInfo(), true);
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
                        }
                        else
                            quickPunch();
                    }
                    else {
                        tcType(1);
                        quickPunch();
                    }
                }
                else {
                    if (e.itemIndex == 0) {
                        tcType(0);
                    }
                    else {
                        tcType(1);
                    }

                    if (punchType() == "NoCause") {
                        if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                            VTPortalUtils.utils.isShowingPopup(false);
                            VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                                window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E', tcType: tcType() });
                            }, function () {
                            }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                        } else {
                            window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E', tcType: tcType() });
                        }
                    }
                    else {
                        if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                            VTPortalUtils.utils.isShowingPopup(false);
                            VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roAlreadyIn'), 'warning', 0, function () {
                                VTPortal.app.navigate("SelectListObject/causes.readerinputcode/E" + '/' + tcType());
                            }, function () {
                            }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                        } else {
                            VTPortal.app.navigate("SelectListObject/causes.readerinputcode/E" + '/' + tcType());
                        }
                    }
                }
            }
        },
        ListZonesVisible: VTPortal.roApp.popupZoneVisible,
        listZones: {
            dataSource: VTPortal.roApp.zonesDataSource,
            scrollingEnabled: true,
            height: 300,
            grouped: false,
            itemTemplate: 'RequestItem',
            onItemClick: onZoneClick
        },
        searchOptions: {
            valueChangeEvent: "keyup",
            mode: "search",
            height: "10%",
            onValueChanged: function (args) {
                VTPortal.roApp.zonesDataSource().searchValue(args.value);
                VTPortal.roApp.zonesDataSource().load();
            }
        },

        taskFieldsVisible: taskFieldsVisible,
        taskFieldsBlock: taskFieldsBlock,
        btnCancelTaskFields: {
            onClick: function () { taskFieldsVisible(false) },
            text: i18nextko.t('roCancelCaptcha'),
        },
        btnAcceptUserFields: {
            onClick: function () { continuePunch() },
            text: i18nextko.t('roContinue'),
        },
        lblTaskAlert: i18nextko.t('roTaskAlert'),
        lblWhereAreYou: i18nextko.t('lblWhereAreYou'),
        taskAlertVisible: taskAlertVisible,
        lblSaveTaskAlertTitle: lblSaveTaskAlertTitle,
        btnCancelTaskAlert: {
            onClick: function () { taskAlertVisible(false) },
            text: i18nextko.t('roCancel'),
        },
        btnAcceptTaskAlert: {
            onClick: function () { saveTaskAlert() },
            text: i18nextko.t('roContinue'),
        },
        txtTaskAlert: {
            value: valueTaskAlert,
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        btnNFCIos: {
            onClick: nfcIosEnabled,
            text: i18nextko.t('roPunchNFC'),
            visible: (VTPortal.roApp.onIosDevice() && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques)
        },
    };

    return viewModel;
};