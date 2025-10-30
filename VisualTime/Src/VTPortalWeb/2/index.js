window.VTPortal = window.VTPortal || {};

$(function () {
    Sugar.extend();
    DevExpress.devices.current({ platform: "android" });

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
                    if (VTPortal.roApp.punchInProgress()) {
                        if (VTPortal.roApp.homeView != null && VTPortalUtils.utils.isOnView('home')) VTPortal.roApp.homeView.refreshModules();
                        if (VTPortal.roApp.statusView != null && VTPortalUtils.utils.isOnView('status')) VTPortal.roApp.statusView.refreshModules();
                    } else {
                        if (VTPortal.roApp.isTimeGate()) {
                            if (!VTPortal.roApp.nfcTimegateInProgress()) {
                                if (!VTPortalUtils.utils.isOnView('TimeGate')) {
                                    VTPortal.app.navigate("TimeGate", { root: true });
                                } else {
                                    VTPortalUtils.utils.logoutTimegate();
                                }
                            }
                        } else {
                            VTPortal.roApp.loggedIn = false;
                            VTPortalUtils.utils.loginIfNecessary('', function () {
                                if (VTPortal.roApp.homeView != null && VTPortalUtils.utils.isOnView('home')) VTPortal.roApp.homeView.refreshModules();
                                if (VTPortal.roApp.statusView != null && VTPortalUtils.utils.isOnView('status')) VTPortal.roApp.statusView.refreshModules();
                            });
                        }
                    }
                }
            }, 1000);
    });

    var currentlyOfline = false;

    $(document).on("online", function () {
        if (currentlyOfline) {
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
        VTPortal.roApp.offlineCounter(VTPortal.roApp.offlineCounter() + 1);
    });

    function onNavigatingBack(e) {
        if (typeof VTPortal.roApp != 'undefined') {
            
            if (VTPortal.roApp.impersonateOnlyRequest) {
                VTPortal.roApp.impersonatedIDEmployee = -1;
                VTPortal.roApp.impersonateOnlyRequest = false;
                VTPortal.roApp.loadInitialData(true, true, true, false, false);
            }
        }
        if (VTPortalUtils.utils.isShowingPopup()) VTPortalUtils.utils.isShowingPopup(false);

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

    var emptyLayoutViews = ["login", "loading", "TimeGate"];

    VTPortal.app.on("resolveLayoutController", function (args) {
        var viewName = args.viewInfo.viewName;

        if (emptyLayoutViews.includes(viewName)) {
            args.layoutController = args.availableLayoutControllers[1].controller;
        } else {
            args.layoutController = args.availableLayoutControllers[0].controller;
        }
    });

    if (localStorage.getItem('adfsLoginInprogress') == null) localStorage.setItem('adfsLoginInprogress', 'false');


    const frameworkNavigate = VTPortal.app.navigate;
    VTPortal.app.navigate = function () {

        if (!VTPortalUtils.utils.isOnView('TimeGate') && arguments[0] == 'TimeGate') {
            if (VTPortal.roApp.timegateLanguage() != VTPortal.roApp.currentLanguageId()) {
                VTPortal.roApp.currentLanguageId(VTPortal.roApp.timegateLanguage());
                VTPortal.roApp.setLanguage(VTPortal.roApp.getLanguageByID(VTPortal.roApp.timegateLanguage()).tag);

                let nThis = this;
                let nArguments = arguments;
                setTimeout(function () {
                    frameworkNavigate.apply(nThis, nArguments);
                }, 500);
            } else {
                frameworkNavigate.apply(this, arguments);
            }
        } else {
            if (typeof VTPortal.roApp != 'undefined' && typeof VTPortal.roApp.lastTimegateAction != 'undefined') {

                VTPortal.roApp.lastTimegateAction = moment();
            }

            frameworkNavigate.apply(this, arguments);
        }

        
    }



    document.addEventListener('click', function (event) {
        if (VTPortal.roApp.timegateLoggedIn) {
            VTPortal.roApp.lastTimegateAction = moment();
        }
    });

    VTPortal.roApp = new roApp();
    VTPortal.roApp.initializeCulture();
    VTPortal.roApp.db.loadSettings();
    localStorage.setItem('VTPortalOnlineClose', 'false');
    if (VTPortal.roApp.isModeApp()) {
        document.body.classList.add('mobile-mode');
    } else {
        document.body.classList.add('desktop-mode');
    }
});