//Clase principal de la aplicación a partir de la qual se genera todo
function roApp() {
    $.support.cors = true;

    var iCounter = ko.observable(0);
    var nfcprogress = ko.observable(false);
    var signInProgress = ko.observable(false);

    //Version info
    this.infoVersion = "3.19.1";
    //User info
    this.companyID = "";
    this.loggedIn = false;
    this.securityToken = null;
    this.guidToken = window.VTPortalUtils.utils.generateGuid();
    this.user = "";
    this.userId = -1;
    this.employeeId = -1;
    this.AnywhereLicense = false;
    this.syncTaskId = -1;
    this.FirebaseToken = "";
    this.showPopover = ko.observable(false);
    this.RequiereFineLocation = true;
    this.ShowForbiddenSections = true;
    this.selectedTab = ko.observable(1);
    this.automaticLogin = true;
    this.refreshId = -1;
    this.adfsLoginInprogress = false;
    this.adfsEnabled = ko.observable(false);
    this.adfsMixedAuth = ko.observable(false);
    this.punchInProgress = ko.observable(false);
    this.ssoEnabled = ko.observable(false);

    this.HeaderMD5 = "";
    this.HeaderChanged = ko.observable(false);
    this.ssoUserName = ko.observable('');
    this.hasNotifications = ko.observable(true);
    this.hasCommuniques = ko.observable(true);
    this.hasSurveys = ko.observable(true);
    this.hasInquiries = ko.observable(false);
    this.lastRequestFailed = ko.observable(false);
    this.newPunchDone = ko.observable(false);
    this.lastOkRefresh = ko.observable(new Date());
    this.lastOkRequests = ko.observable(null);
    this.isMTApp = ko.observable(false);
    this.lastOkAccruals = ko.observable(null);
    this.lastOkAccrualsEmployeeId = ko.observable(null);
    this.supervisedRequests = ko.observable(0);
    this.productivDS = ko.observable(null);
    this.requestsDS = ko.observable(null);
    this.zonesDS = ko.observable(null);
    this.scheduleDS = ko.observable(null);
    this.holidaysDS = ko.observable(null);
    this.documentSent = ko.observable(false);
    this.isSaas = ko.observable(false);
    this.LastLogin = ko.observable(null);
    this.CustomHeader = ko.observable(null);
    this.punchesDS = ko.observable([]);
    this.menuItems = ko.observable([]);
    this.offlineCounter = ko.observable(0);
    this.failedLogins = ko.observable(0);
    this.wsRequestCounter = iCounter;
    this.impersonateOnlyRequest = false;
    this.impersonatedIDEmployee = -1;
    this.impersonatedNameEmployee = "";
    this.supervisedEmployees = [];
    this.capacityEmployees = [];
    this.popupZoneVisible = ko.observable(false);
    this.ZoneSelected = ko.observable(false);
    this.SelectedCause = ko.observable(-1);
    this.SelectedZone = ko.observable(-1);
    this.zonesDataSource = ko.observable(new DevExpress.data.DataSource({
        store: [],
        searchOperation: "contains",
        searchExpr: "Name"
    }));
    this.offlinePunch = ko.observable({
        punch: {
            idEmployee: '',
            credential: -1,
            method: 1,
            action: 'E',
            punchDateTime: moment().format("YYYY-MM-DD HH:mm:ss"),
            PunchData: {
                AttendanceData: { IdCause: 0 }
            },
            online: 0,
            status: 0,
            command: 0,
            punchImage: ''
        },
        command: 0,
        status: null,
        display: null,
        employeeStatus: null,
        saveOnClose: true
    }
    );
    this.refreshId = -1;
    this.refreshClockId = -1;
    this.getImpToken = function () {
        var strToken = ("roImpersonateUser##" + this.impersonatedIDEmployee + "##").encodeBase64();
        return strToken.replace(/=/g, '*');
    };
    this.isOnline = ko.observable(true);
    this.homeView = null;
    this.connectedToMT = ko.observable(false);

    this.nfcPunchInProgress = nfcprogress;
    this.signInProgressLoad = signInProgress;
    this.HasCustomHeader = ko.observable(false);
    this.ReadMode = ko.observable(false);

    this.customBackground = ko.observable(null);

    //this.CalculateCustomHeader = ko.computed(function () {
    //    if (typeof VTPortal.roApp != 'undefined') {
    //        if (VTPortal.roApp.db.settings.ApiVersion < VTPortalUtils.apiVersion.CustomHeader) {
    //            HasCustomHeader(false);
    //        }
    //        else {
    //            if (VTPortal.roApp.db.settings.wsBackground == null) {
    //                HasCustomHeader(false);
    //            }
    //            else {
    //                HasCustomHeader(true);
    //            }

    //        }
    //    }
    //});

    this.loadPanelVisible = ko.computed(function () {
        if (nfcprogress() == true) {
            return false;
        }
        else if (signInProgress() == true) {
            return true;
        }
        else {
            if (iCounter() <= 0) {
                if (iCounter() < 0) iCounter(0);
                return false;
            } else {
                return true;
            }
        }
    });

    this.showNfcPanel = ko.computed(function () {
        if (nfcprogress() == false) {
            return false;
        }
        else {
            if (VTPortalUtils.utils.isShowingPopup() == true) {
                return false;
            }
            else {
                return true;
            }
        }
    });

    this.redirectAtRequestList = function (requestType, bIsSupervisor) {
        var bRedirectToSupervisor = false;

        if (typeof bIsSupervisor != 'undefined') bRedirectToSupervisor = bIsSupervisor;

        if (!bRedirectToSupervisor) {
            switch (requestType) {
                case 1:
                case 5:
                case 6:
                case 7:
                case 8:
                case 11:
                case 13:
                    VTPortal.app.navigate("scheduler/4/" + moment().format("YYYY-MM-DD"), { target: 'back' });
                    break;
                case 9:
                case 14:
                    VTPortal.app.navigate("scheduler/3/" + moment().format("YYYY-MM-DD"), { target: 'back' });
                    break;
                case 4:
                case 15:
                    VTPortal.app.navigate("teleworking/3/" + moment().format("YYYY-MM-DD"), { target: 'back' });
                    break;
                case 2:
                case 3:
                case 10:
                case 12:
                    VTPortal.app.navigate("punchManagement/2/" + moment().format("YYYY-MM-DD"), { target: 'back' });
                    break;
            }

            this.refreshEmployeeStatus(false);
        } else {
            VTPortal.roApp.loadInitialData(true, false, false, false, false);
            if (VTPortal.roApp.db.settings.onlySupervisor) {
                VTPortal.app.navigate("myTeamRequests", { root: 'true' });
            } else {
                VTPortal.app.navigate("myTeamRequests", { target: 'back' });
            }
        }

        //if (workingMode == 0) { //Generic requests
        //} else if (workingMode == 1) { //Teleworking
        //} else if (workingMode == 2) { //Fichajes
        //} else if (workingMode == 3) { //Autorizaciones
        //} else if (workingMode == 4) { //supervisor
        //    VTPortal.app.navigate("myteam/2", { target: 'current' });
        //}
    }

    this.redirectAtHome = function (onlyBar, refreshStatus) {
        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (VTPortal.roApp.db.settings.onlySupervisor) {
                VTPortal.app.createNavigation(VTPortal.config.supervisorNavigation);
                VTPortal.app.renderNavigation();
            } else {
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
                    VTPortal.app.createNavigation(VTPortal.config.navigation);
                    VTPortal.app.renderNavigation();
                } else {
                    VTPortal.app.createNavigation(VTPortal.config.navigationEmployee);//#6996b5
                    VTPortal.app.renderNavigation();
                }
            }

            if (typeof onlyBar == 'undefined' || (typeof onlyBar != 'undefined' && onlyBar == false)) {
                if (VTPortal.roApp.db.settings.onlySupervisor) {
                    this.selectedTab(1);
                    VTPortal.app.navigate("myTeamEmployees", { root: true });
                } else {
                    this.selectedTab(1);
                    VTPortal.app.navigate("home", { root: true });
                }
            }
            else {
                if (VTPortal.roApp.db.settings.onlySupervisor) {
                    this.selectedTab(1);
                    VTPortal.app.navigate("myTeamEmployees", { root: true });
                } else {
                    this.selectedTab(1);
                    VTPortal.app.navigate("home", { root: true });
                }
            }
        } else {
            this.selectedTab(1);
            VTPortal.app.createNavigation(VTPortal.config.impersonatedEmployee);
            VTPortal.app.renderNavigation();
            if (typeof onlyBar == 'undefined' || (typeof onlyBar != 'undefined' && onlyBar == false)) {
                VTPortal.app.navigate("home", { root: true });
            }
        }

        if (typeof refreshStatus != 'undefined' && refreshStatus == true) {
            VTPortal.roApp.loadInitialData(true, true, true, true, false);
        }
    }

    this.reloadCausesList = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var causesDS = [];

                for (var i = 0; i < result.SelectFields.length; i++) {
                    causesDS.push({ ID: parseInt(result.SelectFields[i].FieldValue, 10), Name: result.SelectFields[i].FieldName })
                }

                VTPortalUtils.causes = causesDS;
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
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinue);
        }).getGenericList('causes.visibilitypermissions');
    }

    this.getServerUrl = function (wsURL, isSSL) {
        var newURL = "";
        var MTURL = Cookies.get('MTLiveApiUrl');
        var MTURLUncoded = decodeURIComponent(MTURL);
        if (typeof VTPortal.roApp.isMT() != 'undefined' && VTPortal.roApp.isMT().toLowerCase() == "true") {
            //newURL = 'https://dev-vtliveapi.visualtime.net/Portal/PortalSvcx.svc';
            newURL = MTURLUncoded;
        }
        else {
            var originalCompanyIDConGuion = "";
            var completeHost = "";
            if (VTPortal.roApp.companyID == "") {
                if (typeof wsURL != 'undefined' && wsURL != null) {
                    completeHost = wsURL;
                }
                else {
                    completeHost = VTPortal.roApp.db.settings.wsURL;
                }
                originalCompanyIDConGuion = completeHost.split(".")[0];
                VTPortal.roApp.companyID = completeHost.split(".")[0];
                VTPortal.roApp.companyID = VTPortal.roApp.companyID.split("-")[0];
                VTPortal.roApp.companyID = VTPortal.roApp.companyID.toLowerCase();
            }
            else {
                if (typeof wsURL != 'undefined' && wsURL != null) {
                    completeHost = wsURL;
                }
                else {
                    completeHost = VTPortal.roApp.db.settings.wsURL;
                }
                originalCompanyIDConGuion = completeHost.split(".")[0];
            }

            if (typeof wsURL != 'undefined' && wsURL != null) {
                if (wsURL.includes('vtportal.visualtime.net')) {
                    if (originalCompanyIDConGuion.toLowerCase().includes("-qa")) {
                        newURL = "https://qa-vtliveapi.visualtime.net/Portal/Portalsvcx.svc"
                    }
                    else if (originalCompanyIDConGuion.toLowerCase().includes("-dev")) {
                        newURL = "https://dev-devvtliveapi.visualtime.net/Portal/Portalsvcx.svc"
                    }
                    else {
                        newURL = "https://vtliveapi.visualtime.net/Portal/Portalsvcx.svc"
                    }
                    VTPortal.roApp.isMTApp(true);
                }
                else {
                    newURL = "http" + (isSSL ? "s://" : "://") + wsURL + "/VTLiveApi/Portal/PortalSvcx.svc";
                }
            } else {
                if (VTPortal.roApp.db.settings.wsURL.includes('vtportal.visualtime.net')) {
                    if (originalCompanyIDConGuion.toLowerCase().includes("-qa")) {
                        newURL = "https://qa-vtliveapi.visualtime.net/Portal/Portalsvcx.svc"
                    }
                    else if (originalCompanyIDConGuion.toLowerCase().includes("-dev")) {
                        newURL = "https://dev-devvtliveapi.visualtime.net/Portal/Portalsvcx.svc"
                    }
                    else {
                        newURL = "https://vtliveapi.visualtime.net/Portal/Portalsvcx.svc"
                    }

                    VTPortal.roApp.isMTApp(true);
                }

                else {
                    newURL = "http" + (VTPortal.roApp.db.settings.isSSL ? "s://" : "://") + VTPortal.roApp.db.settings.wsURL + "/VTLiveApi/Portal/PortalSvcx.svc";
                }
            }
        }

        //Para MT debug
        //newURL = 'https://vtliveapi.visualtime.net/Portal/PortalSvcx.svc'
        //VTPortal.roApp.companyID = 'robotics'

        //newURL = 'http://localhost:8031/Portal/PortalSvcx.svc'; //Descomentar linea en development
        //newURL = 'http://localhost/VTLiveApi/Portal/PortalSvcx.svc'; //Descomentar linea en development
        //newURL = 'https://demotest.visualtime.net/VTLiveApi/Portal/PortalSvcx.svc'; //Descomentar linea en development
        //newURL = 'https://rolive5n31.visualtime.net/VTLiveApi/Portal/PortalSvcx.svc'; //Descomentar linea en development
        //newURL = 'https://premium.visualtime.net/VTLiveApi/Portal/PortalSvcx.svc'; //Descomentar linea en development
        //newURL = 'http://192.168.1.152/VTLiveApiDev/Portal/PortalSvcx.svc'; //Descomentar linea en development

        return newURL;
    }

    this.loadZones = function () {
        var callbackFuntion = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var zonesInfo = result.ListZones;
                VTPortal.roApp.zonesDS(zonesInfo);
                VTPortal.roApp.zonesDataSource(new DevExpress.data.DataSource({
                    store: VTPortal.roApp.zonesDS(),
                    searchOperation: "contains",
                    searchExpr: "Name"
                }));

                setTimeout(function () {
                    $('#scrollview').height($('#panelsContent').height() - 45);
                }, 100);
            } else {
                itemCount(0);
                dataSource(new DevExpress.data.DataSource({
                    store: [],
                    searchOperation: "contains",
                    searchExpr: "Name"
                }));
            }
        };

        if (VTPortal.roApp.zonesDS() == null) {
            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getZones();
        }
    };

    this.isModeApp = function () {
        if (typeof (device) != 'undefined') {
            return true;
        } else {
            return false;
        }
    };

    this.onIosDevice = function () {
        if (typeof (device) != 'undefined') {
            if (cordova.platformId === 'ios') {
                return true;
            }
            else {
                return false;
            }
        } else {
            return false;
        }
    };

    this.gpsEmployeePermission = ko.observable(true);
    this.userName = ko.observable('');
    this.mtServer = ko.observable('');
    this.configWS = ko.observable('');
    this.configSSL = ko.observable(false);
    this.syncing = ko.observable(false);
    this.nfcEnabled = ko.observable(false);
    this.userPhoto = ko.observable('');
    this.userStatus = ko.observable(null);
    this.stopShowingSurveysPopup = ko.observable(false);
    this.requestFilters = ko.observable(null);
    this.teleWorkingFilters = ko.observable(null);
    this.punchesFilters = ko.observable(null);
    this.authorizationFilters = ko.observable(null);
    this.documentsFilters = ko.observable(null);
    this.showLogoutHome = ko.observable(false);
    this.isAD = ko.observable('');
    this.isMT = ko.observable('');
    this.supervisorFilters = ko.observable(null);
    this.empPermissions = ko.observable(null);
    this.clientTimeZone = moment.tz.guess();
    this.serverTimeZone = 'Europe/Berlin';

    this.licenseAccepted = ko.observable(true);
    this.licenseConsent = ko.observable(null);

    //this.roDB = new PortalDB();

    this.setLanguage = function (locale) {
        i18nextko.init(locale, ko);
        i18nextko.setLanguage(locale);
        moment.locale(locale);

        var path = window.location.href.substr(0, window.location.href.indexOf('index.html'));

        $.getJSON(path + "../2/js/localization/dx.all." + locale + ".json", function (data3) {
            Globalize.loadMessages(data3);
            Globalize.locale(locale);
        });
    }

    if (this.empPermissions() != null && this.empPermissions().MustUseGPS) {
        this.currentLocationInfo = ko.observable({
            coords: [41.5551618, 2.0960154],
            location: 'Sabadell',
            fullAddress: 'Avenida Francesc Macià 60, Sabadell, Spain',
            reliable: false
        });
    } else {
        this.currentLocationInfo = ko.observable({
            coords: [0, 0],
            location: '',
            fullAddress: '',
            reliable: true
        });
    }

    this.taskPunchRequest = ko.observable({
        currentAction: -1,
        newAction: -1,
        currentTaskUserFieldsDefinition: null,
        currentTaskUserFieldsValue: ['', '', '', -1, -1, -1],
        newTaskUserFieldsDefinition: null,
        newTaskUserFieldsValue: ['', '', '', -1, -1, -1]
    });

    this.loadInitialData = function (bReloadUser, bReloadBackground, bLoadPermissions, bReloadPosition, enableTrigger, onLoadCallback) {
        if (VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(0);

        if (VTPortal.roApp.loggedIn) {
            if (VTPortalUtils.causes == null) VTPortal.roApp.reloadCausesList();

            if (bReloadUser) {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        VTPortal.roApp.db.settings.userPhoto = result.Base64StringContent;
                        VTPortal.roApp.userPhoto(result.Base64StringContent);
                        VTPortal.roApp.db.settings.saveParameter('userPhoto');
                    } else {
                    }
                }).GetWsEmployeePhoto(-1);

                //new WebServiceRobotics(function (result) {
                //    if (result.Status == window.VTPortalUtils.constants.OK.value || (result.Status < -8 && result.Status > -59)) {
                //        if (typeof result.ScheduleStatus.TrackingDocuments == 'undefined' || result.ScheduleStatus.TrackingDocuments == null) result.ScheduleStatus.TrackingDocuments = [];
                //        if (typeof result.ScheduleStatus.ForecastDocuments == 'undefined' || result.ScheduleStatus.ForecastDocuments == null) result.ScheduleStatus.ForecastDocuments = [];

                //        var curStatus = VTPortal.roApp.userStatus();
                //        curStatus.ScheduleStatus = Object.clone(result.ScheduleStatus, true);
                //        curStatus.SupervisorStatus = Object.clone(result.SupervisorStatus, true);
                //        VTPortal.roApp.userStatus(curStatus);

                //        VTPortal.roApp.setBadgeCount(result);
                //    }
                //}).getEmployeeAlerts();
            }

            if (bReloadBackground) {
                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.CustomHeader && VTPortal.roApp.HeaderChanged()) {
                    new WebServiceRobotics(function (result) {
                        if (result.Status == window.VTPortalUtils.constants.OK.value) {
                            VTPortal.roApp.db.settings.wsBackground = result.PortalConfiguration;
                            VTPortal.roApp.CustomHeader(result.PortalConfiguration);
                            VTPortal.roApp.db.settings.saveParameter('wsBackground');
                            if (result.PortalConfiguration != null && result.PortalConfiguration != "") {
                                VTPortal.roApp.customBackground(JSON.parse(result.PortalConfiguration));
                                VTPortal.roApp.HasCustomHeader(true);
                            }
                            else {
                                VTPortal.roApp.HasCustomHeader(false);
                            }
                        } else {
                            VTPortal.roApp.HasCustomHeader(false);
                        }
                    }).GetWsBackgroundImage();
                }
                else {
                    try {
                        var ob = JSON.parse(VTPortal.roApp.db.settings.wsBackground);
                        if (VTPortal.roApp.db.settings.wsBackground != null && VTPortal.roApp.db.settings.wsBackground != '') {
                            VTPortal.roApp.HasCustomHeader(true);
                        }
                        else {
                            VTPortal.roApp.HasCustomHeader(false);
                        }
                    } catch (e) {
                        VTPortal.roApp.HasCustomHeader(false);
                    }
                }
            }

            if (bLoadPermissions) {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        //Se hace por compatibilidad con los permisos antiguos
                        if (typeof result.Punch.ScheduleQuery == 'undefined') result.Punch.ScheduleQuery = result.Punch.Query;
                        if (typeof result.Punch.ProductiVQuery == 'undefined') result.Punch.ProductiVQuery = result.Punch.Query;
                        if (typeof result.MustUseGPS == 'undefined' || result.MustUseGPS == null) result.MustUseGPS = true;
                        if (typeof result.MustUsePhoto == 'undefined' || result.MustUsePhoto == null) result.MustUsePhoto = true;

                        VTPortal.roApp.db.settings.empPermissions = JSON.stringify(result).encodeBase64();
                        VTPortal.roApp.empPermissions(result);
                        VTPortal.roApp.db.settings.saveParameter('empPermissions');

                        VTPortal.roApp.refreshEmployeeStatus(bReloadPosition, true, onLoadCallback);
                        //if (typeof onLoadCallback == 'function') onLoadCallback();
                    }
                }).getMyPermissions();
            } else {
                VTPortal.roApp.refreshEmployeeStatus(bReloadPosition, false, onLoadCallback);
                // if (typeof onLoadCallback == 'function') onLoadCallback();
            }

            var documentSigned = Cookies.get('VTPortalSign');
            Cookies.remove('VTPortalSign');
            if (documentSigned != null) {
                VTPortal.app.navigate("documents", { root: true });
            }

            if (enableTrigger) {
                if (VTPortal.roApp.refreshId != -1) clearTimeout(VTPortal.roApp.refreshId);

                VTPortal.roApp.refreshId = setTimeout(function () {
                    VTPortal.roApp.refreshId = -1;
                    VTPortal.roApp.db.exportToJsonString(function () { });
                    VTPortal.roApp.loadInitialData(false, false, false, false, true);
                }, 600000);

                if (VTPortal.roApp.refreshClockId != -1) {
                    clearInterval(VTPortal.roApp.refreshClockId);
                }

                VTPortal.roApp.refreshClockId = setInterval(function () {
                    VTPortalUtils.utils.updateClientTime(10);
                }, 10000);
            }

            //if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.NewRequests && bReloadZones && VTPortal.roApp.empPermissions().MustSelectZone) {
            //    loadZones();
            //}
            //if (VTPortal.roApp.isModeApp()) {
            //    VTPortal.roApp.executeTask(); //sync fichajes offline
            //}
        }
    }

    this.refreshEmployeeStatus = function (bReloadPosition, bReloadAlerts, endCallback) {
        var tmpScheduleStatus = null;
        var tmpSupervisorStatus = null;

        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            var oldStatus = VTPortal.roApp.userStatus();

            tmpScheduleStatus = Object.clone(oldStatus.ScheduleStatus, true);
            tmpSupervisorStatus = Object.clone(oldStatus.SupervisorStatus, true);
        }

        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value || (result.Status < -8 && result.Status > -59)) {
                VTPortal.roApp.lastOkRefresh(new Date());
                if (typeof result.ScheduleStatus.TrackingDocuments == 'undefined' || result.ScheduleStatus.TrackingDocuments == null) result.ScheduleStatus.TrackingDocuments = [];
                if (typeof result.ScheduleStatus.ForecastDocuments == 'undefined' || result.ScheduleStatus.ForecastDocuments == null) result.ScheduleStatus.ForecastDocuments = [];
                if (typeof result.ScheduleStatus.SignDocuments == 'undefined' || result.ScheduleStatus.SignDocuments == null) result.ScheduleStatus.SignDocuments = [];

                if (tmpScheduleStatus != null && typeof tmpScheduleStatus != 'undefined' && tmpSupervisorStatus != null && typeof tmpSupervisorStatus != 'undefined') {
                    result.ScheduleStatus = tmpScheduleStatus;
                    result.SupervisorStatus = tmpSupervisorStatus;
                }
                VTPortal.roApp.userStatus(result);
                VTPortal.roApp.db.settings.lastStatus = JSON.stringify(result).encodeBase64();
                VTPortal.roApp.db.settings.saveParameter('lastStatus');

                if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
                    if (VTPortal.roApp.userStatus().ReadMode == true) {
                        VTPortal.roApp.ReadMode(true);
                    }
                    else {
                        VTPortal.roApp.ReadMode(false);
                    }
                }

                if (bReloadPosition && VTPortal.roApp.gpsEmployeePermission()) {
                    if (VTPortal.roApp.empPermissions().MustUseGPS != null && VTPortal.roApp.empPermissions().MustUseGPS) {
                        navigator.geolocation.getCurrentPosition(
                            function (position) {
                                var location = VTPortal.roApp.currentLocationInfo();

                                //location.location = "-";
                                //location.fullAddress = "-";
                                //location.coords = [position.coords.latitude, position.coords.longitude];
                                //location.reliable = true;
                                //VTPortal.roApp.currentLocationInfo(location);

                                //Solo hacemos la petición a la api de google si se ha modificado la posición.
                                if (location.coords[0] != position.coords.latitude || location.coords[1] != position.coords.longitude || location.reliable == false) {
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
                                    VTPortal.roApp.currentLocationInfo(location);
                                    VTPortal.roApp.gpsEmployeePermission(false);
                                } else {
                                    var location = VTPortal.roApp.currentLocationInfo();
                                    location.coords[0] = 0; location.coords[1] = 0;
                                    location.location = "API Error parsing city";
                                    location.fullAddress = "API Error parsing address";
                                    location.reliable = false;
                                    VTPortal.roApp.currentLocationInfo(location);
                                    VTPortal.roApp.gpsEmployeePermission(true);
                                }
                            }, { maximumAge: VTPortal.roApp.RequiereFineLocation ? 600000 : 6000000, timeout: 10000, enableHighAccuracy: VTPortal.roApp.RequiereFineLocation });
                    }
                }

                if (bReloadAlerts) {
                    new WebServiceRobotics(function (result) {
                        if (result.Status == window.VTPortalUtils.constants.OK.value || (result.Status < -8 && result.Status > -59)) {
                            if (typeof result.ScheduleStatus.TrackingDocuments == 'undefined' || result.ScheduleStatus.TrackingDocuments == null) result.ScheduleStatus.TrackingDocuments = [];
                            if (typeof result.ScheduleStatus.ForecastDocuments == 'undefined' || result.ScheduleStatus.ForecastDocuments == null) result.ScheduleStatus.ForecastDocuments = [];
                            if (typeof result.ScheduleStatus.SignDocuments == 'undefined' || result.ScheduleStatus.SignDocuments == null) result.ScheduleStatus.SignDocuments = [];

                            var curStatus = VTPortal.roApp.userStatus();
                            curStatus.ScheduleStatus = Object.clone(result.ScheduleStatus, true);
                            curStatus.SupervisorStatus = Object.clone(result.SupervisorStatus, true);
                            VTPortal.roApp.userStatus(curStatus);

                            VTPortal.roApp.setBadgeCount(result);
                        }
                    }).getEmployeeAlerts();
                }
                if (VTPortal.roApp.homeView != null) VTPortal.roApp.homeView.punchAllowedCalc();
                if (typeof endCallback == 'function') endCallback();
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }

            if (!VTPortal.roApp.adfsLoginInprogress) {
                var newRequestToken = Cookies.get('VTPortalRequest');
                Cookies.remove('VTPortalRequest');
                if (newRequestToken != null) {
                    var values = newRequestToken.split("_");
                    VTPortal.roApp.impersonatedIDEmployee = values[1];
                    VTPortal.roApp.impersonateOnlyRequest = true;

                    new WebServiceRobotics(function (result) {
                        if (result.Status == window.VTPortalUtils.constants.OK.value) {
                            VTPortal.roApp.loadInitialData(true, false, false, false, false, function () { VTPortalUtils.utils.navigateToRequest({ IdRequestType: result.RequestType, ID: result.Id }); });
                        } else {
                            var onContinue = function () {
                                VTPortal.roApp.impersonatedIDEmployee = -1;
                                VTPortal.roApp.impersonateOnlyRequest = false;
                                VTPortal.roApp.redirectAtHome();
                            }
                            VTPortalUtils.utils.processErrorMessage(result, onContinue);
                        }
                    }, function (error) {
                        var onContinue = function () {
                            VTPortal.roApp.impersonatedIDEmployee = -1;
                            VTPortal.roApp.impersonateOnlyRequest = false;
                            VTPortal.roApp.redirectAtHome();
                        }
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequest"), 'error', 0, onContinue);
                    }).getRequest(values[0]);
                }
            }
        }).getEmployeeStatus();
    }

    this.setBadgeCount = function (result) {
        //if (VTPortal.roApp.impersonatedIDEmployee == -1) {
        //    if (VTPortal.roApp.db.settings.onlySupervisor) {
        //        VTPortal.app.navigation[1].option('badge', VTPortalUtils.utils.badgeCount(true, true, true));
        //        VTPortal.app.navigation[2].option('badge', VTPortalUtils.utils.badgeCount(true, true, false));
        //    } else {
        //        VTPortal.app.navigation[1].option('badge', VTPortalUtils.utils.badgeCount(false));
        //        if (VTPortal.roApp.db.settings.supervisorPortalEnabled && VTPortal.roApp.impersonatedIDEmployee == -1) {
        //            VTPortal.app.navigation[3].option('badge', VTPortalUtils.utils.badgeCount(true, false, true));
        //        }
        //    }
        //} else {
        //    VTPortal.app.navigation[1].option('badge', VTPortalUtils.utils.badgeCount(false));
        //}
    }

    this.availableLanguages = [{
        "ID": "ESP",
        "Name": "Español",
        "tag": "es",
    }, {
        "ID": "CAT",
        "Name": "Catalán",
        "tag": "ca",
    }, {
        "ID": "ENG",
        "Name": "Inglés",
        "tag": "en",
    }, {
        "ID": "FRA",
        "Name": "Francés",
        "tag": "fr",
    }, {
        "ID": "ITA",
        "Name": "Italiano",
        "tag": "it",
    }, {
        "ID": "GAL",
        "Name": "Gallego",
        "tag": "gl",
    },{
        "ID": "POR",
        "Name": "Portugués",
        "tag": "pt",
    }];

    this.getLanguageByID = function (id) {
        for (var i = 0; i < this.availableLanguages.length; i++) {
            if (this.availableLanguages[i].ID == id) {
                return this.availableLanguages[i];
            }
        }

        return this.availableLanguages[0];
    }

    this.initializeCulture = function () {
        i18nextko.init("es", ko);
        i18nextko.setLanguage('es');
        moment.locale('es');

        var path = window.location.href.substr(0, window.location.href.indexOf('index.html'));
        $.ajaxSetup({ cache: false });
        $.getJSON(path + "../js/cldr/locales/likelySubtags.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-ca.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-fr.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-it.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-en.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-es.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-gl.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-pt.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
    }

    //this.executeTask = async function () {
    //    try {
    //        clearTimeout(VTPortal.roApp.syncTaskId);
    //        if (!VTPortal.roApp.lastRequestFailed()) {
    //            await window.VTPortalUtils.utils.fullSync();
    //        }
    //    } catch (e) {
    //    }

    //    VTPortal.roApp.syncTaskId = setTimeout(VTPortal.roApp.executeTask, 20000);
    //};

    this.db = new DatabaseUtils('spVtPortal', 'VTPortal settings', this.isModeApp());
}