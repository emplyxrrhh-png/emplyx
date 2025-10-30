window.VTPortal = window.VTPortal || {};

$(function () {
    Sugar.extend();
    DevExpress.devices.current({ platform: "android" });

    //var getToken = function () {
    //    FirebasePlugin.getToken(function (token) {
    //        VTPortal.roApp.FirebaseToken = token;
    //        //registerToken();
    //    })

    //};
    //var grantPermission = function () {
    //    FirebasePlugin.grantPermission(function (hasPermission) {
    //        if (hasPermission) {
    //            getToken();
    //        }
    //        console.log("Permission was " + (hasPermission ? "granted" : "denied"));
    //    });
    //};

    $(document).on("deviceready", function () {
        //setTimeout(function () { navigator.splashscreen.hide(); }, 3000);
        if (window.devextremeaddon) {
            window.devextremeaddon.setup();
        }
        $(document).on("backbutton", function () {
            DevExpress.processHardwareBackButton();
        });

        //try {
        //    grantPermission();
        //}
        //catch {
        //}
    });

    $(document).on("pause", function () {
        if (VTPortal.roApp.refreshId != -1) {
            clearTimeout(VTPortal.roApp.refreshId);
            VTPortal.roApp.refreshId = -1;
        }
    });

    $(document).on("resume", function () {
        setTimeout(function () {
            if (VTPortal.roApp.loggedIn) {
                if (VTPortal.roApp.punchInProgress() == true) {
                    if (VTPortal.roApp.homeView != null) VTPortal.roApp.homeView.resume();
                }
                else {
                    VTPortal.roApp.loggedIn = false;
                    VTPortalUtils.utils.loginIfNecessary('', function () {
                        if (VTPortal.roApp.homeView != null) VTPortal.roApp.homeView.resume();
                    });
                }
            }
        }, 1000);
    });

    var currentlyOfline = false;

    $(document).on("online", function () {
        if (currentlyOfline) {
            // VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roConnectionRecovered'), 'success', 3000);
            VTPortal.roApp.lastRequestFailed(false);
            VTPortal.roApp.isOnline(true);
            VTPortal.roApp.offlineCounter(0);
            currentlyOfline = false;
        }
    });

    $(document).on("offline", function () {
        currentlyOfline = true;
        VTPortal.roApp.lastRequestFailed(true);
        VTPortal.roApp.isOnline(false);
        //VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roConnectionLost'), 'error', 0);
        VTPortal.roApp.offlineCounter(VTPortal.roApp.offlineCounter() + 1);
    });

    function onNavigatingBack(e) {
        if (typeof VTPortal.roApp != 'undefined') {
            //var currentViewName = VTPortal.app.navigationManager.currentItem().uri.split('/');
            //var oldViewName = '';

            //var currentIndex = VTPortal.app.navigationManager.currentIndex() - 1;
            //var canExit = false;

            //while (currentIndex > 0 && !canExit) {
            //    oldViewName = VTPortal.app.navigationManager.getItemByIndex(currentIndex).uri.split('/');

            //    if (currentViewName[0] == oldViewName[0]) {
            //    } else {
            //        canExit = true;
            //    }
            //}

            ////if (typeof VTPortal.app.navigationManager.getItemByIndex(VTPortal.app.navigationManager.currentIndex() - 1) != 'undefined') {
            ////    oldViewName = VTPortal.app.navigationManager.getItemByIndex(VTPortal.app.navigationManager.currentIndex() - 1).uri.split('/');
            ////}

            if (VTPortal.roApp.impersonateOnlyRequest) {
                VTPortal.roApp.impersonatedIDEmployee = -1;
                VTPortal.roApp.impersonateOnlyRequest = false;
                VTPortal.roApp.loadInitialData(true, true, true, false, false);
            }
        }
        if (VTPortalUtils.utils.isShowingPopup() == true) VTPortalUtils.utils.isShowingPopup(false);

        if (e.isHardwareButton && !VTPortal.app.canBack()) {
            e.cancel = true;
            exitApp();
        }
    }

    function exitApp() {
        switch (DevExpress.devices.real().platform) {
            case "android":
                exitAndroidApp();
                break;
            case "win":
                MSApp.terminateApp('');
                break;
            default:
                break;
        }
    }

    function exitAndroidApp() {
        //VTPortalUtils.utils.isShowingPopup(false);
        //VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roExitAppDetail'), 'warning', 0, function () {
        if (VTPortal.roApp.isModeApp()) {
            navigator.Backbutton.goHome(function () {
                console.log('success')
            }, function () {
                console.log('fail')
            });
        }
        else {
            localStorage.setItem('VTPortalOnlineClose', 'true');
        }

        //if (VTPortal.roApp.isModeApp()) navigator.app.exitApp();
        //else

        //}, function () {
        //    if (VTPortal.roApp.loggedIn) {
        //        VTPortal.roApp.redirectAtHome();
        //    } else {
        //        VTPortal.app.navigate("login", { root: true });
        //    }
        //}, i18nextko.i18n.t('roExit'), i18nextko.i18n.t('roGotoHome'));
    }

    var layoutSet = DevExpress.framework.html.layoutSets["navbar"],
        emptyLayoutController = new DevExpress.framework.html.EmptyLayoutController();

    layoutSet.push({ controller: emptyLayoutController });

    VTPortal.app = new DevExpress.framework.html.HtmlApplication({
        namespace: VTPortal,
        layoutSet: layoutSet,
        navigation: VTPortal.config.navigation,
        commandMapping: VTPortal.config.commandMapping
    });

    VTPortal.app.router.register(":view/:id/:param/:iDate/:objId", { view: "login", id: undefined, param: undefined, iDate: undefined, objId: undefined });
    VTPortal.app.on("navigatingBack", onNavigatingBack);

    var emptyLayoutViews = ["login", "loading"];//, "requestsList", "RequestFilters", "SelectListObject", "userFields", "forgottenPunch", "justifyPunch", "externalWork", "changeShift", "newLeave", "shiftExchange",
    //"plannedAbsence", "plannedCause", "cancelHolidays", "sendLeaveDocument", "DocumentFilters", "plannedHoliday", "plannedOvertime", "externalWorkWeekResume", "profile", "dayInfo"];

    VTPortal.app.on("resolveLayoutController", function (args) {
        var viewName = args.viewInfo.viewName;

        if (emptyLayoutViews.includes(viewName)) {
            args.layoutController = args.availableLayoutControllers[1].controller;
        } else {
            args.layoutController = args.availableLayoutControllers[0].controller;
        }
    });

    if (localStorage.getItem('adfsLoginInprogress') == null) localStorage.setItem('adfsLoginInprogress', 'false');

    VTPortal.roApp = new roApp();
    VTPortal.roApp.initializeCulture();
    VTPortal.roApp.db.loadSettings();
    localStorage.setItem('VTPortalOnlineClose', 'false');
});