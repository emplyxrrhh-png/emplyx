//Clase principal de la aplicación a partir de la qual se genera todo
function roApp() {
    if (typeof (device) != 'undefined' || window.document.URL.indexOf("localhost:8036") >= 0) {
        $.support.cors = true;
    }

    let iCounter = ko.observable(0);
    let nfcprogress = ko.observable(false);
    let nfcTimegateProgress = ko.observable(false);
    let signInProgress = ko.observable(false);

    //Version info
    this.infoVersion = "3.40.1";
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
    this.DailyRecordEnabled = ko.observable(false);
    this.ShowPermissionsIcon = ko.observable(true);
    this.DailyRecordPatternEnabled = ko.observable(false);
    this.ShowForbiddenSections = true;
    this.selectedTab = ko.observable(1);
    this.automaticLogin = true;
    this.refreshId = -1;
    this.adfsLoginInprogress = false;
    this.adfsEnabled = ko.observable(false);
    this.adfsMixedAuth = ko.observable(false);
    this.punchInProgress = ko.observable(false);
    this.ssoVersion = ko.observable(0);
    this.ssoEnabled = ko.observable(false);
    this.ssoUnkownError = ko.observable(false);
    this.apiURL = "";
    this.serverURL = { valid: false, isMT: false, url: 'notValid', companyId: '', defURL: '', defSSL: '' };
    this.HeaderMD5 = "";
    this.BackgroundMD5 = "";
    this.timeGateBackground = "";
    this.HeaderChanged = ko.observable(false);
    this.localFormat = 'YYYY-MM-DD[T]HH:mm:ss';
    this.ssoUserName = ko.observable('');
    this.currentLanguageId = ko.observable('');
    this.hasNotifications = ko.observable(true);
    this.hasCommuniques = ko.observable(true);
    this.hasMessages = ko.observable(true);
    this.hasSurveys = ko.observable(true);
    this.hasInquiries = ko.observable(false);
    this.lastRequestFailed = ko.observable(false);
    this.isMTApp = ko.observable(false);
    this.supervisedRequests = ko.observable(0);
    this.productivDS = ko.observable(null);
    this.requestsDS = ko.observable(null);
    this.dailyRecordsDS = ko.observable(null);
    this.zonesDS = ko.observable(null);
    this.scheduleDS = ko.observable(null);
    this.holidaysDS = ko.observable(null);
    this.documentSent = ko.observable(false);
    this.isSaas = ko.observable(false);
    this.isCegidHub = ko.observable(false);
    this.isCommuniquesEnabled = ko.observable(false);
    this.isChannelsEnabled = ko.observable(false);
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
    this.isImpersonateEnabled = ko.observable(true);
    this.isLatamMex = ko.observable(false);
    this.supervisedEmployees = [];
    this.capacityEmployees = [];
    this.popupZoneVisible = ko.observable(false);
    this.popupCauseVisible = ko.observable(false);
    this.ZoneSelected = ko.observable(false);
    this.SelectedCause = ko.observable(-1);
    this.SelectedZone = ko.observable(-1);
    this.ReliableZone = ko.observable(true);

    this.zonesDataSource = ko.observable(new DevExpress.data.DataSource({
        store: [],
        searchOperation: "contains",
        searchExpr: "Name"
    }));

    this.causesDataSource = ko.observable(new DevExpress.data.DataSource({
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
    this.isOnline = ko.observable(true);
    this.homeView = null;
    this.statusView = null;
    this.connectedToMT = ko.observable(false);
    this.nfcPunchInProgress = nfcprogress;
    this.nfcTimegateInProgress = nfcTimegateProgress;
    this.signInProgressLoad = signInProgress;
    this.HasCustomHeader = ko.observable(false);
    this.ReadMode = ko.observable(false);
    this.ServerZones = ko.observable(null);
    this.customBackground = ko.observable(null);
    this.showLegalText = ko.observable("");
    this.dataCatalogs = new Robotics.Client.Data.roCatalog();
    this.gpsEmployeePermission = ko.observable(true);
    this.userName = ko.observable('');
    this.mtServer = ko.observable('');
    this.configWS = ko.observable('');
    this.configSSL = ko.observable(false);
    this.syncing = ko.observable(false);
    this.nfcEnabled = ko.observable(false);
    this.userPhoto = ko.observable('');
    this.currentTCInfo = ko.observable('');
    this.userStatus = ko.observable(null);
    this.userCommuniques = ko.observable(null);
    this.userSurveys = ko.observable(null);
    this.userTelecommute = ko.observable(null);
    this.userAlerts = ko.observable(null);
    this.stopShowingSurveysPopup = ko.observable(false);
    this.requestFilters = ko.observable(null);
    this.teleWorkingFilters = ko.observable(null);
    this.punchesFilters = ko.observable(null);
    this.authorizationFilters = ko.observable(null);
    this.documentsFilters = ko.observable(null);
    this.showLogoutHome = ko.observable(false);
    this.isAD = ko.observable('');
    this.supervisorFilters = ko.observable(null);
    this.supervisorFiltersDR = ko.observable(null);
    this.empPermissions = ko.observable(null);
    this.clientTimeZone = moment.tz.guess();
    this.serverTimeZone = 'Europe/Berlin';
    this.licenseAccepted = ko.observable(true);
    this.licenseConsent = ko.observable(null);
    this.currentLocationInfo = ko.observable({
        coords: [0, 0],
        location: '',
        fullAddress: '',
        reliable: true,
        zone: null
    });
    this.taskPunchRequest = ko.observable({
        currentAction: -1,
        newAction: -1,
        currentTaskUserFieldsDefinition: null,
        currentTaskUserFieldsValue: ['', '', '', -1, -1, -1],
        newTaskUserFieldsDefinition: null,
        newTaskUserFieldsValue: ['', '', '', -1, -1, -1]
    });

    this.isTimeGate = ko.observable(false);
    this.screenTimeout = ko.observable(10);
    this.logoutTimmer = -1;

    this.timeGateTimmerId = -1;
    this.timeGateTimmerResponse = -1;
    this.lastTimegateAction = moment();
    this.timeGateConfig = null;
    this.timegateLoggedIn = false;
    this.timegateLanguage = ko.observable("ESP");
    this.timeGateMode = ko.observable(null);
    this.timeGateModeEIP = 123;
    this.timeGateModeTA = 95;
    this.timeGateModeCO = 126;

    this.checkAutomaticLogout = function () {
        if(VTPortal.roApp.logoutTimmer > 0) clearTimeout(VTPortal.roApp.logoutTimmer);

        if (VTPortal.roApp.wsRequestCounter() > 0) {
            //si hay alguna petición en curso del portal, nos esperamos a que termine para revisar el timer
            VTPortal.roApp.logoutTimmer = setTimeout(VTPortal.roApp.checkAutomaticLogout, 1000);
        } else {
            let serverDisconnect = VTPortal.roApp.screenTimeout();
            if (typeof serverDisconnect == 'undefined') serverDisconnect = 10;
            if (moment().diff(VTPortal.roApp.lastTimegateAction, 'seconds') > serverDisconnect) {
                if (VTPortal.roApp.isTimeGate() && VTPortal.roApp.timegateLoggedIn) {
                    VTPortalUtils.utils.logoutTimegate();
                    VTPortal.app.navigate("TimeGate", { root: true });
                }
            } else {
                VTPortal.roApp.logoutTimmer = setTimeout(VTPortal.roApp.checkAutomaticLogout, 1000);
            }
        }
        
    }

    this.getNumericVersion = function (customVersion) {
        let vSplit = this.infoVersion.split(".");
        if (typeof customVersion != 'undefined' && customVersion != '') {
            vSplit = customVersion.split(".");
        }

        let intNumericVersion = 0;

        for (let i = 0; i < vSplit.length; i++) {
            let padDigits = (vSplit.length - i - 1) * 3;

            intNumericVersion = intNumericVersion + parseInt(vSplit[i].padEnd(vSplit[i].length + padDigits, "0"), 10);
        }

        return intNumericVersion;
    }

    this.redirectAtRequestList = function (requestType, bIsSupervisor) {
        let bRedirectToSupervisor = false;

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
        } else {
            VTPortal.roApp.loadInitialData(true, false, false, false, false);
            if (VTPortal.roApp.db.settings.onlySupervisor) {
                VTPortal.app.navigate("myTeamRequests", { root: 'true' });
            } else {
                VTPortal.app.navigate("myTeamRequests", { target: 'back' });
            }
        }
    }

    this.redirectAtHome = function (onlyBar, refreshStatus) {
        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (VTPortal.roApp.db.settings.onlySupervisor) {
                VTPortal.app.createNavigation(VTPortal.config.supervisorNavigation);
                VTPortal.app.renderNavigation();
            } else if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
                VTPortal.app.createNavigation(VTPortal.config.navigation);
                VTPortal.app.renderNavigation();
            } else {
                VTPortal.app.createNavigation(VTPortal.config.navigationEmployee);//#6996b5
                VTPortal.app.renderNavigation();
            }

            this.selectedTab(1);
            if (VTPortal.roApp.db.settings.onlySupervisor) VTPortal.app.navigate("myTeamEmployees", { root: true });
            else VTPortal.app.navigate("home", { root: true });

        } else {
            this.selectedTab(1);
            VTPortal.app.createNavigation(VTPortal.config.impersonatedEmployee);
            VTPortal.app.renderNavigation();
            if (typeof onlyBar == 'undefined' || (typeof onlyBar != 'undefined' && onlyBar )) {
                VTPortal.app.navigate("home", { root: true });
            }
        }

        if (typeof refreshStatus != 'undefined' && refreshStatus) {
            VTPortal.roApp.loadInitialData(true, true, true, true, false);
        }
    }

    this.reloadCausesList = function () {
        let onContinue = function () {
            VTPortal.roApp.redirectAtHome();
        }

        let tmpCausesDS = VTPortal.roApp.db.settings.getCacheDS('causes');

        if (tmpCausesDS != null) { 
            VTPortalUtils.causes = tmpCausesDS.visibility;
            VTPortal.roApp.causesDataSource(new DevExpress.data.DataSource({
                store: tmpCausesDS.readerinput,
                searchOperation: "contains",
                searchExpr: "Name"
            }));
        }

        if (tmpCausesDS == null || window.VTPortalUtils.needToRefresh('causes')) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    let causesVDS = [];

                    for (const element of result.SelectFields) {
                        causesVDS.push({ ID: parseInt(element.FieldValue, 10), Name: element.FieldName })
                    }

                    VTPortalUtils.causes = causesVDS;

                    let serverDS = {
                        visibility: causesVDS,
                        readerinput: null
                    };


                    new WebServiceRobotics(function (result) {
                        if (result.Status == window.VTPortalUtils.constants.OK.value) {
                            let causesRDS = [];

                            for (const element of result.SelectFields) {
                                if (parseInt(element.FieldValue, 10) > 0) {
                                    causesRDS.push({ ID: parseInt(element.FieldValue, 10), Name: element.FieldName })
                                }
                            }

                            serverDS.readerinput = causesRDS;
                            VTPortal.roApp.db.settings.updateCacheDS('causes', serverDS);


                            VTPortal.roApp.causesDataSource(new DevExpress.data.DataSource({
                                store: causesRDS,
                                searchOperation: "contains",
                                searchExpr: "Name"
                            }));
                        } else {
                            VTPortalUtils.utils.processErrorMessage(result, onContinue);
                        }
                    }).getGenericList('causes.readerinputcode');


                } else {
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            }, function (error) {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinue);
            }).getGenericList('causes.visibilitypermissions');


        }
    }

    this.reloadZonesList = function () {
        VTPortal.roApp.ServerZones(VTPortal.roApp.db.settings.getCacheDS('zones'));

        if (VTPortal.roApp.ServerZones() == null || window.VTPortalUtils.needToRefresh('zones')) {
            let onContinueCallback = function () {
                VTPortal.roApp.redirectAtHome();
            };
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.db.settings.updateCacheDS('zones', result.ListZones);
                    VTPortal.roApp.ServerZones(result.ListZones);
                } else {
                    VTPortalUtils.utils.processErrorMessage(result, onContinueCallback);
                }
            }, function (error) {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinueCallback);
            }).getServerZones();
        }

    }

    this.getZoneOfCurrentLocation = function() {
        if ( VTPortal.roApp.ServerZones() != null ) {
            let listZones = VTPortal.roApp.ServerZones().filter(function (a) {
                return a.Area != null && parseFloat(a.Area.toString().replace(",", ".")) > 0;
            });
            listZones = listZones.sort(function (a, b) {
                return a.Area - b.Area;
            });
            let location = VTPortal.roApp.currentLocationInfo();

            if (listZones.length > 0) {
                for (const element of listZones) {
                    if (element.MapInfo != null && element.MapInfo.Coordinates != null && element.MapInfo.Coordinates.length == 2) {
                        if (parseFloat(location.coords[0]) >= parseFloat(element.MapInfo.Coordinates[1].Latitud)
                            && parseFloat(location.coords[0]) <= parseFloat(element.MapInfo.Coordinates[0].Latitud)
                            && parseFloat(location.coords[1]) >= parseFloat(element.MapInfo.Coordinates[1].Longitud)
                            && parseFloat(location.coords[1]) <= parseFloat(element.MapInfo.Coordinates[0].Longitud)) {
                            return element.Id;
                        }
                    }
                }
            }
            return null;
        } else {
            return null;
        }
    };

    this.loadZones = function () {
        let callbackFuntion = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                let zonesInfo = result.ListZones;
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

        new WebServiceRobotics(function (result) { callbackFuntion(result); }).getZones();
    };

    this.loadInitialData = function(bReloadUserPhoto, bReloadBackground, bLoadPermissions, bReloadPosition, enableTrigger, onLoadCallback) {
        let portalApp = this;

        if (VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(0);

        if (VTPortal.roApp.loggedIn ) {
            

            if (VTPortalUtils.causes == null) VTPortal.roApp.reloadCausesList();
            if (VTPortal.roApp.ServerZones() == null) VTPortal.roApp.reloadZonesList();

            if (bReloadUserPhoto) portalApp.reloadUserPhoto();

            if (bReloadBackground) portalApp.reloadBackground();

            if (bLoadPermissions) portalApp.reloadUserPermissions(bReloadPosition, onLoadCallback);
            else VTPortal.roApp.refreshEmployeeStatus(bReloadPosition, onLoadCallback);

            portalApp.redirectToDocumentIfRequiered();

        }

        if (enableTrigger) portalApp.restartRefreshTrigger();
    }

    this.reloadUserPhoto = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.db.settings.userPhoto = result.Base64StringContent;
                VTPortal.roApp.userPhoto(result.Base64StringContent);
                VTPortal.roApp.db.settings.saveParameter('userPhoto');
            }
        }).GetWsEmployeePhoto(-1);
    }

    this.reloadBackground = function () {
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
        else if (VTPortal.roApp.db.settings.wsBackground != null && VTPortal.roApp.db.settings.wsBackground != '') {
            VTPortal.roApp.HasCustomHeader(true);
        }
        else {
            VTPortal.roApp.HasCustomHeader(false);
        }
    }

    this.reloadUserPermissions = function (bReloadPosition, onLoadCallback) {
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

                VTPortal.roApp.refreshEmployeeStatus(bReloadPosition, onLoadCallback);
            }
        }).getMyPermissions(); 
    }

    this.restartRefreshTrigger = function () {

        if (VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(0);
        if (!VTPortal.roApp.loggedIn) return;

        if (VTPortal.roApp.refreshId != -1) clearTimeout(VTPortal.roApp.refreshId);

        VTPortal.roApp.refreshId = setTimeout(function () {

            VTPortal.roApp.refreshId = -1;
            VTPortal.roApp.db.exportToJsonString(function () { });

            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) VTPortal.roApp.loadInitialData(false, false, false, false, true);
                else {
                    window.VTPortalUtils.utils.onLogoutSuccessFunc({ Status: 0 });
                }
            }, function (error) {
                window.VTPortalUtils.utils.onLoginErrorFunc(error);
            }, VTPortal.roApp.serverURL.url).authenticateSession();

        }, 600000);

        if (VTPortal.roApp.refreshClockId != -1) {
            clearInterval(VTPortal.roApp.refreshClockId);
        }

        VTPortal.roApp.refreshClockId = setInterval(function () {
            VTPortalUtils.utils.updateClientTime(10);
        }, 10000);
    }

    this.redirectToDocumentIfRequiered = function () {
        let documentSigned = Cookies.get('VTPortalSign');
        Cookies.remove('VTPortalSign');
        if (documentSigned != null) VTPortal.app.navigate("documents", { root: true });
    }

    this.refreshEmployeeStatus = function (bReloadPosition, endCallback) {

        let onRefreshStatusResponse = function (endCallback) {
            return function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value || (result.Status < -8 && result.Status > -59)) {
                    VTPortal.roApp.db.settings.updateCacheDS('status', result);
                    VTPortal.roApp.userStatus(result);

                    if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
                        if (VTPortal.roApp.userStatus().ReadMode) VTPortal.roApp.ReadMode(true);
                        else VTPortal.roApp.ReadMode(false);
                    }

                    if (VTPortal.roApp.homeView != null) VTPortal.roApp.homeView.punchAllowedCalc();
                    if (typeof endCallback == 'function') endCallback();
                    setTimeout(function () {
                        VTPortalUtils.utils.buildHamburgerMenu();
                    }, 1000); 
                } else {
                    VTPortalUtils.utils.processErrorMessage(result);
                }
            }

            VTPortal.roApp.checkIfRedirectToRequestIsEnabled();

        }



        let onRefreshNotificationStatus = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.db.settings.updateCacheDS('alertstatus', result);
                VTPortal.roApp.userNotificationStatus(result);


                setTimeout(function () {

                    if (VTPortal.roApp.userNotificationStatus() != null && VTPortal.roApp.userNotificationStatus().HasMandatoryCommuniques &&
                        !(VTPortalUtils.utils.isOnView('communiques') || VTPortalUtils.utils.isOnView('communiqueDetail') || VTPortalUtils.utils.isOnView('status'))) {

                        VTPortal.roApp.db.settings.markForRefresh(['communiques']);
                        VTPortal.app.navigate("status", { root: true });
                    }

                }, 1000);
            }
        }
        
        let onEmployeeTelecommuteResponse = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.db.settings.updateCacheDS('telecommute',result.Telecommute);
                VTPortal.roApp.userTelecommute(result.Telecommute);
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        }

        if (bReloadPosition && VTPortal.roApp.gpsEmployeePermission()) {
            if (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().MustUseGPS != null && VTPortal.roApp.empPermissions().MustUseGPS) {
                VTPortalUtils.utils.reloadLocation(null);
            }
        }

        let callbackAlreadyExecuted = false;
        let needToRefreshStatus = window.VTPortalUtils.needToRefresh('status');
        VTPortal.roApp.userStatus(VTPortal.roApp.db.settings.getCacheDS('status'));
        if (VTPortal.roApp.userStatus() == null || needToRefreshStatus) {
            callbackAlreadyExecuted = true;
            new WebServiceRobotics(onRefreshStatusResponse(endCallback), function () { }).getUserScheduleStatus();
            
        }

        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            VTPortal.roApp.userNotificationStatus(VTPortal.roApp.db.settings.getCacheDS('alertstatus'));
            if (needToRefreshStatus) {
                new WebServiceRobotics(onRefreshNotificationStatus, function () { }).getUserNotificationsStatus();

            }
        } else {
            VTPortal.roApp.userNotificationStatus(null);
        }


        VTPortal.roApp.userTelecommute(VTPortal.roApp.db.settings.getCacheDS('telecommute'));
        if (VTPortal.roApp.userTelecommute() == null || window.VTPortalUtils.needToRefresh('telecommute')) {
            new WebServiceRobotics(onEmployeeTelecommuteResponse, function () { }).getEmployeeTelecommutingInfo();
        }



        if (!callbackAlreadyExecuted && typeof endCallback == 'function') {
            endCallback();
            setTimeout(function () {
                VTPortalUtils.utils.buildHamburgerMenu();
            }, 1000); 
        }
    }

    this.refreshEmployeeAlerts = function (endcallback) {

        let onEmployeeNotificationsResponse = function(eCallback) {
            return function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value || (result.Status < -8 && result.Status > -59)) {
                    if (typeof result.ScheduleStatus.TrackingDocuments == 'undefined' || result.ScheduleStatus.TrackingDocuments == null) result.ScheduleStatus.TrackingDocuments = [];
                    if (typeof result.ScheduleStatus.ForecastDocuments == 'undefined' || result.ScheduleStatus.ForecastDocuments == null) result.ScheduleStatus.ForecastDocuments = [];
                    if (typeof result.ScheduleStatus.SignDocuments == 'undefined' || result.ScheduleStatus.SignDocuments == null) result.ScheduleStatus.SignDocuments = [];

                    let curStatus = VTPortal.roApp.userAlerts();
                    if (curStatus != null) {
                        curStatus.ScheduleStatus = result.ScheduleStatus;
                        curStatus.SupervisorStatus = result.SupervisorStatus;
                    } else {
                        curStatus = {
                            ScheduleStatus: result.ScheduleStatus,
                            SupervisorStatus: result.SupervisorStatus
                        }
                    }

                    VTPortal.roApp.db.settings.updateCacheDS('notifications', curStatus);
                    VTPortal.roApp.userAlerts(curStatus);

                    if (typeof eCallback == 'function') eCallback();

                } else {
                    VTPortalUtils.utils.processErrorMessage(result);
                }
            }
        }

        let loading = false;
        VTPortal.roApp.userAlerts(VTPortal.roApp.db.settings.getCacheDS('notifications'));
        if (VTPortal.roApp.userAlerts() == null || window.VTPortalUtils.needToRefresh('notifications')) {
            loading = true;
            new WebServiceRobotics(onEmployeeNotificationsResponse(endcallback), function () { }).getUserNotifications();
        } 

        if (!loading && typeof endcallback == 'function') endcallback();
    }



    this.checkIfRedirectToRequestIsEnabled = function () {
        let onContinue = function () {
            VTPortal.roApp.impersonatedIDEmployee = -1;
            VTPortal.roApp.impersonateOnlyRequest = false;
            VTPortal.roApp.redirectAtHome();
        }

        if (!VTPortal.roApp.adfsLoginInprogress) {
            let newRequestToken = Cookies.get('VTPortalRequest');
            Cookies.remove('VTPortalRequest');
            if (newRequestToken != null) {
                let values = newRequestToken.split("_");
                VTPortal.roApp.impersonatedIDEmployee = values[1];
                VTPortal.roApp.impersonateOnlyRequest = true;

                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        VTPortal.roApp.loadInitialData(true, false, false, false, false, function () { VTPortalUtils.utils.navigateToRequest({ IdRequestType: result.RequestType, ID: result.Id }); });
                    } else {
                        VTPortalUtils.utils.processErrorMessage(result, onContinue);
                    }
                }, function (error) {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequest"), 'error', 0, onContinue);
                }).getRequest(values[0]);
            }
        }
    }

    this.getLanguageByID = function (id) {
        for (const element of this.availableLanguages) {
            if (element.ID == id) {
                return element;
            }
        }

        return this.availableLanguages[0];
    }

    this.initializeCulture = function () {
        i18nextko.init("es", ko);
        i18nextko.setLanguage('es');
        moment.locale('es');

        let path = window.location.href.substr(0, window.location.href.indexOf('index.html'));
        $.ajaxSetup({ cache: false });
        $.getJSON(path + "../js/cldr/locales/likelySubtags.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-ca.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-fr.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-it.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-en.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-es.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-gl.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-pt.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
        $.getJSON(path + "../js/cldr/locales/environment-eu.json?_=" + new Date().getMilliseconds(), function (data) { Globalize.load(data) });
    }

    this.disableImpersonateActionsOnRequest = function () {
        //Ocultamos el menú que tiene acciones de impersonación
        if (document.querySelector('#tabMain')) document.querySelector('#tabMain').style.display = 'none';
    }

    this.isModeApp = function () {
        return typeof (device) != 'undefined';
    };

    this.isRemoteEnvironment = function () {
        if (typeof (device) != 'undefined') {
            return true;
        } else {
            return window.document.URL.indexOf("localhost:8036") >= 0;
        }
    };

    this.onIosDevice = function () {
        if (typeof (device) != 'undefined') {
            return cordova.platformId === 'ios';
        } else {
            return false;
        }
    };

    this.onIos18Device = function () {
        if (typeof (device) != 'undefined' && cordova.platformId === 'ios') {
            return (parseFloat(device.version) >= 18);
        } else {
            return false;
        }
    };

    this.openURLonExternalBrowser = function (url) {
        if (VTPortal.roApp.isModeApp()) {
            cordova.InAppBrowser.open(url, '_system', 'location=yes');
        } else {
            window.open(url, '_system', 'location=yes');
        }
    }

    this.getServerUrl = function (wsURL) {
        if (!VTPortal.roApp.isModeApp()) {
            if (window.document.URL.indexOf("localhost:8036") >= 0) {
                if (typeof wsURL != 'undefined' && wsURL != '') {
                    this.apiURL = wsURL;
                }
            } else {
                this.apiURL = "/api/Portalsvcx.svc";
            }
        } else if (typeof wsURL != 'undefined' && wsURL != '') {
            this.apiURL = wsURL;
        }

        if (this.apiURL == "") {
            this.apiURL = VTPortal.roApp.db.settings.wsURL;
        }

        return this.apiURL;
    }

    this.getImpToken = function () {
        let strToken = ("roImpersonateUser##" + this.impersonatedIDEmployee + "##").encodeBase64();
        return strToken.replace(/=/g, '*');
    };

    this.setLanguage = function (locale) {
        i18nextko.init(locale, ko);
        i18nextko.setLanguage(locale);
        moment.locale(locale);

        let path = window.location.href.substr(0, window.location.href.indexOf('index.html'));

        $.getJSON(path + "../2/js/localization/dx.all." + locale + ".json", function (data3) {
            Globalize.loadMessages(data3);
            Globalize.locale(locale);
        });
    }

    this.db = new DatabaseUtils('spVtPortal', 'VTPortal settings', this.isModeApp());

    this.loadPanelVisible = ko.computed(function () {
        if (nfcprogress()) return false;

        if (signInProgress()) return true;

        if (iCounter() <= 0) {
            if (iCounter() < 0) iCounter(0);
            return false;
        } else {
            return true;
        }
    });

    this.showNfcPanel = ko.computed(function () {
        if (!nfcprogress()) return false;

        return !(VTPortalUtils.utils.isShowingPopup());
    });

    this.availableLanguages = [{
        "ID": "ESP",
        "Name": "Español",
        "tag": "es",
    }, {
        "ID": "CAT",
        "Name": "Catalán",
        "tag": "ca",
    }, {
        "ID": "EKR",
        "Name": "Euskera",
        "tag": "eu",
    }, {
        "ID": "GAL",
        "Name": "Gallego",
        "tag": "gl",
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
        "ID": "POR",
        "Name": "Portugués",
        "tag": "pt",
    }, {
        "ID": "SLK",
        "Name": "Eslovaco",
        "tag": "sk",
    } ];

    if (this.empPermissions() != null && this.empPermissions().MustUseGPS) {
        this.currentLocationInfo = ko.observable({
            coords: [41.5551618, 2.0960154],
            location: 'Sabadell',
            fullAddress: 'Avenida Francesc Macià 60, Sabadell, Spain',
            reliable: false,
            zone: null
        });
    }

    this.mainMenuItems = ko.observable([]);
    this.hamburgerMenu = {
        dataSource: this.mainMenuItems,
        adaptivityEnabled: true,
        itemTemplate: "title",
        selectOnFocus: false,
        onItemClick: function (e) {
            VTPortal.roApp.selectedTab(e.itemData.ID);

            switch (e.itemData.ActionIndex) {
                case 1:
                    VTPortal.app.navigate("home", { root: true });
                    break;
                case 2:
                    VTPortal.app.navigate("scheduler/1/" + moment().format("YYYY-MM-DD"), { root: true });
                    break;
                case 3:
                    VTPortal.app.navigate("punchManagement/1/" + moment().format("YYYY-MM-DD"), { root: true });
                    break;
                case 4:
                    VTPortal.app.navigate("teleworking/1/" + moment().format("YYYY-MM-DD"), { root: true });
                    break;
                case 5:
                    VTPortal.app.navigate("documents", { root: true });
                    break;
                case 6:
                    VTPortal.app.navigate("leaves", { root: true });
                    break;
                case 7:
                    VTPortal.app.navigate("profile", { root: true });
                    break;
                case 15:
                    VTPortal.app.navigate("myTeamEmployees", { root: true });
                    break;
                case 16:
                    VTPortal.app.navigate("myTeamAlerts", { root: true });
                    break;
                case 17:
                    VTPortal.app.navigate("myTeamRequests", { root: true });
                    break;
                case 18:
                    VTPortal.app.navigate("myteam", { root: true });
                    break;
                case 19:
                    VTPortal.app.navigate("communiques", { root: true });
                    break;
                case 20:
                    VTPortal.app.navigate("onboardings", { root: true });
                    break;
                case 21:
                    VTPortal.app.navigate("surveyDetail", { root: true });
                    break;
                case 22:
                    VTPortal.app.navigate("myDailyRecord", { root: true });
                    break;
                case 23:
                    VTPortal.app.navigate("myChannels", { root: true });
                    break;
                case 24:
                    VTPortal.app.navigate("status", { root: true });
                    break;
                case 25: //Back to my team
                    VTPortal.roApp.impersonatedIDEmployee = -1;
                    if (VTPortal.roApp.db.settings.onlySupervisor) {
                        VTPortal.roApp.redirectAtHome(false);
                    } else {
                        VTPortal.roApp.selectedTab(2);
                        VTPortal.app.navigate('myteam', { root: true });
                    }
                    VTPortal.roApp.loadInitialData(true, true, true, false, false, function () {
                        VTPortalUtils.utils.buildHamburgerMenu();
                    });
                    break;
                case 26: //Logout
                    new WebServiceRobotics(function (result) {
                        window.VTPortalUtils.utils.onLogoutSuccessFunc(result);
                    }, function (error) {
                        window.VTPortalUtils.utils.onLogoutErrorFunc(error);
                    }).logout(VTPortal.roApp.db.settings.UUID);
                    break;
                case 27: //About
                    DevExpress.ui.notify('VisualTime Portal ' + VTPortal.roApp.infoVersion, 'info', 3000);
                    break;
                case 28: 
                    VTPortal.app.navigate("configuration", { root: true });
                    break;
                case 29: //weblinks
                    VTPortal.app.navigate("myWebLinks", { root: true });
                    break;
                case 30: //just one link
                    var url = e.itemData.URL;
                    if (url) {
                        window.open(url, '_blank');
                    }
                    break;
                default:
                    //VTPortalUtils.utils.notifyMesage("not implemented", 'error', 2000)
                    break;
            }
        }
    };

    this.userNotificationStatus = ko.observable(null);
    let privateRoApp = ko.observable(this);

    this.userStatusBadge = ko.computed(function () {
        if (VTPortalUtils.utils.badgeCount(false, false, false, privateRoApp()) > 0) {
            return "!";
        }
        else {
            return "";
        }
    });
    this.supervisorBadge = ko.computed(function () {
        if (VTPortalUtils.utils.badgeCount(true, false, false, privateRoApp()) > 0) {
            return "!";
        }
        else {
            return "";
        }
    });
    this.showHamburgerMenu = ko.observable(false);    
}

