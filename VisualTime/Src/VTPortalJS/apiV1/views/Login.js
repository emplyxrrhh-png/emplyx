VTPortal.login = function (params) {
    var inAppBrowserRef = null, timer = -1;
    var username = VTPortal.roApp.userName,
        passwordForId = ko.observable(""),
        clientServer = VTPortal.roApp.mtServer,
        serverLocation = VTPortal.roApp.configWS,
        isSSL = VTPortal.roApp.configSSL,
        currentLanguage = ko.observable(window.VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language)),
        toastVisible = ko.observable(false),
        toastMessage = ko.observable(''),
        toastType = ko.observable('error');
    var tmpServer = '';
    var tmpIsSSL = null;
    var modePwd = ko.observable('password');
    var userLogin = ko.observable(i18nextko.i18n.t('user'));
    var configuration = ko.observable(i18nextko.i18n.t('configuration'));
    var client = ko.observable(i18nextko.i18n.t('client'));
    var usernameLogin = ko.observable(i18nextko.i18n.t('username'));
    var pwdLogin = ko.observable(i18nextko.i18n.t('pwd'));
    var passwordLogin = ko.observable(i18nextko.i18n.t('password'));
    var lngLogin = ko.observable(i18nextko.i18n.t('lng'));
    var sendMsgLogin = ko.observable(i18nextko.i18n.t('sendMsg'));
    var sendSSOLogin = ko.observable(i18nextko.i18n.t('sendSSOLogin'));
    var recoverPwdLogin = ko.observable(i18nextko.i18n.t('recoverPwd'));
    var recoverTitleLogin = ko.observable(i18nextko.i18n.t('roRecoverTitle'));
    var recoverUserNameLogin = ko.observable(i18nextko.i18n.t('roRecoverUserName'));
    var recoverEmailLogin = ko.observable(i18nextko.i18n.t('roRecoverEmail'));
    var recoverFullInfoLogin = ko.observable(i18nextko.i18n.t('roRecoverFullInfo'));
    var sendRecoverRequestLogin = ko.observable(i18nextko.i18n.t('roNextStep'));
    var firstAttempt = ko.observable(true);
    var firstAttemptPwd = ko.observable(true);
    var multiTennantChecked = ko.observable(false);
    var sendUsingSSO = ko.observable(true);
    var enableSSOLoginButton = ko.observable(true);
    var disableSTDLoginButton = ko.observable(true);

    var adfsLoginAttempts = 0;

    var recoverUserName = ko.observable(''),
        recoverEmail = ko.observable('');

    var popupVisible = ko.observable(false),
        setPwdPopupVisible = ko.observable(false),
        popupMTVisible = ko.observable(false),
        recoverPwdPopupVisible = ko.observable(false);

    var requestKey = ko.observable(''),
        newPassword = ko.observable('');

    var isMT = ko.computed(function () {
        if (Cookies.get('IsMT') == 'True') {
            return 'True';
        } else {
            return 'False';
        }
    });

    var isModeApp = ko.computed(function () {
        if (VTPortal.roApp.isModeApp()) {
            return true;
        } else {
            var vtLiveAPIUrl = Cookies.get('VTPortalApi');

            if (vtLiveAPIUrl != null && vtLiveAPIUrl != '') {
                vtLiveAPIUrl = decodeURIComponent(vtLiveAPIUrl);
                var serverUrl = vtLiveAPIUrl.replace('https://', '').replace('http://', '');
                serverUrl = serverUrl.substr(0, serverUrl.indexOf('/'));
                VTPortal.roApp.configWS(serverUrl);

                if (vtLiveAPIUrl.toLowerCase().indexOf('https://') == -1) VTPortal.roApp.configSSL(false);
                else VTPortal.roApp.configSSL(true);
            } else {
                var serverUrl = window.location.href.toLowerCase().replace('https://', '').replace('http://', '');
                serverUrl = serverUrl.substr(0, serverUrl.indexOf('/'));
                VTPortal.roApp.db.settings.wsURL = serverLocation(serverUrl);

                if (window.location.href.toLowerCase().indexOf('https://') == -1) isSSL(false);
                else isSSL(true);
            }

            return false;
        }
    });

    var goPage = function (params) {
        VTPortal.roApp.automaticLogin = true;
        checkVisualTimeServer(true);
    };

    var goToSSOLogin = function (params) {
        prepareSSOLogin();
    }

    var grantPermission = function () {
        FirebasePlugin.grantPermission(function (gotPermission) {
            if (gotPermission) {
                // getToken();
            }
            console.log("Permission was " + (gotPermission ? "granted" : "denied"));
        });
    };

    var commitLogin = function (bForceLogin) {
        if (typeof VTPortal.roApp.isMT() != 'undefined' && VTPortal.roApp.isMT().toLowerCase() == "true") {
            VTPortal.roApp.companyID = clientServer();
            VTPortal.roApp.db.companyID = VTPortal.roApp.companyID;
        }

        var result = DevExpress.validationEngine.getGroupConfig('validationCode').validate();

        if (VTPortal.roApp.db.settings.isUsingAdfs && VTPortal.roApp.db.settings.adfsCredential == '') {
            VTPortal.roApp.wsRequestCounter(0);
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roLoginFromWeb'), 'info', 0);
        } else {
            if (result.isValid || (VTPortal.roApp.db.settings.isUsingAdfs && VTPortal.roApp.db.settings.adfsCredential != '')) {
                VTPortal.roApp.db.settings.wsURL = tmpServer;
                VTPortal.roApp.db.settings.isSSL = tmpIsSSL;
                VTPortal.roApp.db.settings.login = username().trim();
                VTPortal.roApp.db.settings.language = currentLanguage().ID;
                VTPortal.roApp.db.settings.companyID = clientServer().trim();
                VTPortal.roApp.db.settings.MT = localStorage.getItem('IsMT') == 'true' ? true : false;

                if (!VTPortal.roApp.db.settings.isUsingAdfs && VTPortal.roApp.db.settings.adfsCredential == '') {
                    new WebServiceRobotics(function (result) {
                        if (result.Status == window.VTPortalUtils.constants.OK.value && result.Token != null) {
                            VTPortal.roApp.db.settings.password = passwordForId().trim();
                            VTPortal.roApp.setLanguage(currentLanguage().tag);

                            localStorage.setItem("ApiVersion", result.ApiVersion);

                            if (VTPortal.roApp.isModeApp()) {
                                if (result.ApiVersion >= VTPortalUtils.apiVersion.Firebase) {
                                    try {
                                        FirebasePlugin.hasPermission(function (hasPermission) {
                                            if (!hasPermission) {
                                                grantPermission();
                                            }
                                        });
                                    }
                                    catch (e) {
                                    }
                                }
                            }

                            if (typeof result.ServerTimezone != 'undefined') {
                                VTPortal.roApp.serverTimeZone = result.ServerTimezone;
                                if (VTPortal.roApp.serverTimeZone != VTPortal.roApp.db.settings.serverTimezone) {
                                    VTPortal.roApp.db.settings.serverTimezone = VTPortal.roApp.serverTimeZone;
                                }
                            }

                            if (VTPortal.roApp.db.settings.ApiVersion != result.ApiVersion) {
                                VTPortal.roApp.db.emptyViewsCatalogs();
                                VTPortal.roApp.db.settings.ApiVersion = result.ApiVersion;
                            }

                            if (typeof result.Consent != 'undefined') {
                                VTPortal.roApp.licenseConsent(result.Consent);
                            }

                            if (VTPortal.roApp.licenseAccepted() != result.LicenseAccepted) {
                                VTPortal.roApp.db.settings.licenseAccepted = result.LicenseAccepted;
                                VTPortal.roApp.licenseAccepted(result.LicenseAccepted);
                            }
                            if (typeof result.IsSaas != 'undefined') {
                                VTPortal.roApp.isSaas(result.IsSaas);
                            }
                            if (moment(result.LastLogin).year() != 2079) {
                                VTPortal.roApp.LastLogin(result.LastLogin);
                            }
                            VTPortal.roApp.automaticLogin = true;
                            VTPortal.roApp.securityToken = result.Token;
                            VTPortal.roApp.loggedIn = true;
                            VTPortal.roApp.userId = result.UserId;
                            VTPortal.roApp.AnywhereLicense = result.AnywhereLicense;
                            VTPortal.roApp.RequiereFineLocation = (typeof result.RequiereFineLocation != 'undefined' ? result.RequiereFineLocation : true);
                            VTPortal.roApp.ShowForbiddenSections = (typeof result.ShowForbiddenSections != 'undefined' ? result.ShowForbiddenSections : true);
                            VTPortal.roApp.showLogoutHome(typeof result.ShowLogoutHome != 'undefined' ? result.ShowLogoutHome : true);
                            VTPortal.roApp.isAD = result.IsAD;
                            VTPortal.roApp.db.settings.showForbiddenSections = VTPortal.roApp.ShowForbiddenSections;
                            VTPortal.roApp.db.settings.showLogoutHome = result.ShowLogoutHome;
                            VTPortal.roApp.db.settings.isAD = result.IsAD;
                            VTPortal.roApp.db.settings.AnywhereLicense = result.AnywhereLicense;
                            VTPortal.roApp.db.settings.RequiereFineLocation = (typeof result.RequiereFineLocation != 'undefined' ? result.RequiereFineLocation : true);

                            if (typeof result.EmployeeId != 'undefined') VTPortal.roApp.employeeId = result.EmployeeId;
                            else VTPortal.roApp.employeeId = -1;
                            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SupervisorPortal && VTPortal.roApp.employeeId == -1) VTPortal.roApp.db.settings.onlySupervisor = true
                            else VTPortal.roApp.db.settings.onlySupervisor = false;

                            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SupervisorPortal) {
                                VTPortal.roApp.db.settings.supervisorPortalEnabled = result.SupervisorPortalEnabled;
                            }

                            VTPortal.roApp.db.settings.save();

                            setTimeout(function () {
                                VTPortal.roApp.loadInitialData(true, true, true, true, true);
                                VTPortal.roApp.redirectAtHome();
                                passwordForId('');
                            }, 200);
                        } else {
                            VTPortal.roApp.automaticLogin = false;
                            VTPortalUtils.utils.processSecurityMessage(result);
                            VTPortal.roApp.db.settings.password = passwordForId().trim();
                        }
                    }).login(username().trim(), passwordForId().trim(), currentLanguage().ID, VTPortal.roApp.infoVersion, '', true);
                } else {
                    VTPortal.roApp.automaticLogin = false;
                    if (VTPortal.roApp.db.settings.isUsingAdfs && VTPortal.roApp.db.settings.adfsCredential != '') {
                        var sTokens = VTPortal.roApp.db.settings.adfsCredential.decodeBase64().split('#');
                        VTPortal.roApp.guidToken = sTokens[0];
                        VTPortal.roApp.securityToken = sTokens[1];
                        if (sTokens.length > 2) {
                            VTPortal.roApp.companyID = sTokens[2];
                        }

                        VTPortal.roApp.setLanguage(currentLanguage().tag);
                        VTPortal.roApp.db.settings.save();

                        new WebServiceRobotics(function (result) {
                            var onFinish = function () {
                                VTPortal.roApp.automaticLogin = true;
                                VTPortal.roApp.db.settings.password = passwordForId().trim();
                                VTPortal.roApp.setLanguage(currentLanguage().tag);
                                VTPortal.roApp.redirectAtHome();
                            }

                            window.VTPortalUtils.utils.onCheckUserSession(result, onFinish, currentLanguage().ID);
                        }, function (error) {
                            window.VTPortalUtils.utils.onLoginErrorFunc(error);
                        }).getLoggedInUserInfo();
                    }
                }
            }
        }
    }

    var recoverPwd = function (params) {
        var result = DevExpress.validationEngine.getGroupConfig('requestRecover').validate();
        if (result.isValid) {
            if (firstAttemptPwd() == true) {
                firstAttemptPwd(false);
                tmpServer = serverLocation().trim();
            }
            tmpIsSSL = isSSL();

            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    recoverPwdPopupVisible(false);
                    firstAttemptPwd(true);
                    setPwdPopupVisible(true);
                    $('#txtRequestKey :input').focus();
                } else {
                    VTPortalUtils.utils.processErrorMessage(result, function () { });
                }
            }, function (error) {
                if (isModeApp()) {
                    var serverUrlElements = serverLocation().split('.');
                    if (serverUrlElements.length == 3 && serverLocation().trim().indexOf('.visualtime.net') < 0) {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                        });
                    }

                    else {
                        if (multiTennantChecked() == false) {
                            if (tmpServer.trim().indexOf('.visualtime.net') > 0 && tmpServer.trim().indexOf('-vtportal.visualtime.net') < 0) {
                                tmpServer = tmpServer.replace(".visualtime.net", "-vtportal.visualtime.net");
                                multiTennantChecked(true);
                                recoverPwd();
                            }
                            else if (tmpServer.trim().indexOf('.visualtime.net') > 0 && tmpServer.trim().indexOf('-vtportal.visualtime.net') > 0) {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                                    tmpServer = tmpServer.replace("-vtportal.visualtime.net", ".visualtime.net");
                                    tmpServer = tmpServer.replace(".visualtime.net", "");
                                });
                            }
                            else if (serverUrlElements.length == 1 && tmpServer != '') {
                                tmpServer = serverLocation().concat('.visualtime.net');
                                recoverPwd();
                            }
                            else if (serverUrlElements.length == 3 && tmpServer != '' && tmpServer.trim().indexOf('.visualtime.net') < 0 && tmpServer.trim().indexOf('-vtportal.visualtime.net') < 0) {
                                tmpServer = tmpServer.concat('.visualtime.net');
                                recoverPwd();
                            }
                            else {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                                    tmpServer = tmpServer.replace("-vtportal.visualtime.net", ".visualtime.net");
                                    tmpServer = tmpServer.replace(".visualtime.net", "");
                                });
                            }
                        }
                        else {
                            multiTennantChecked(false);
                            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                                tmpServer = tmpServer.replace("-vtportal.visualtime.net", ".visualtime.net");
                                tmpServer = tmpServer.replace(".visualtime.net", "");
                            });
                        }
                    }
                }
                else {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                    });
                }
            }, tmpServer, tmpIsSSL).recoverPassword(recoverUserName().trim(), recoverEmail().trim());
        }
    };

    var setNewPassword = function (params) {
        var result = DevExpress.validationEngine.getGroupConfig('setRecoveredPwd').validate();
        if (result.isValid) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    recoverPwdPopupVisible(false);
                    setPwdPopupVisible(false);
                    username(recoverUserName());
                    passwordForId(newPassword());
                    requestKey('');
                    newPassword('');
                    recoverUserName('');
                    recoverEmail('');
                    goPage();
                } else {
                    VTPortalUtils.utils.processErrorMessage(result, function () { });
                }
            }, function (e) { }, tmpServer, tmpIsSSL).resetPasswordToNew(recoverUserName().trim(), requestKey().trim(), newPassword().trim());
        }
    };

    var showPopup = function () {
        popupVisible(true);
        multiTennantChecked(false);
        firstAttempt(true);
        $('#txtServerLocation :input').focus();
    }
    var showPopupMT = function () {
        popupMTVisible(true);
        $('#txtClientLocation :input').focus();
    }

    var closeOptions = function () {
        firstAttempt(true);
        multiTennantChecked(false);
        checkVisualTimeServer(false);
    }

    var checkMT = function () {
        VTPortal.roApp.companyID = clientServer();
        checkVisualTimeServer(false);
    }

    var prepareSSOLogin = function () {
        localStorage.setItem('adfsLoginInprogress', 'true');
        VTPortal.roApp.adfsLoginInprogress = true;

        VTPortal.roApp.db.settings.isUsingAdfs = true;
        VTPortal.roApp.adfsEnabled(true);
        popupVisible(false);
        popupMTVisible(false);
        VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() + 1);

        if (typeof VTPortal.roApp.isMT() != 'undefined' && VTPortal.roApp.isMT().toLowerCase() == "true") {
            if (tmpServer.toLowerCase().indexOf("vtportaldev") != -1) {
                tmpServer = tmpServer.toLowerCase().replace("vtportaldev", "vtlivedev");
                openVisualTimePortal("http" + (isSSL() == true ? "s" : "") + "://vtlivedev-idi01.azurewebsites.net/VTLogin/VTPortalLogin.aspx?isApp=" + (isModeApp() ? '1' : '0') + "&referer=" + VTPortal.roApp.companyID + "&_=" + new Date().getMilliseconds());
            } else {
                openVisualTimePortal("http" + (isSSL() == true ? "s" : "") + "://vtlive.visualtime.net/VTLogin/VTPortalLogin.aspx?isApp=" + (isModeApp() ? '1' : '0') + "&referer=" + VTPortal.roApp.companyID + "&_=" + new Date().getMilliseconds());
            }

            //openVisualTimePortal("http" + (isSSL() == true ? "s" : "") + "://localhost/VTLive/VTLogin/VTPortalLogin.aspx?isApp=" + (isModeApp() ? '1' : '0') + "&referer=" + VTPortal.roApp.companyID + "&_=" + new Date().getMilliseconds());
        } else {
            openVisualTimePortal("http" + (isSSL() == true ? "s" : "") + "://" + tmpServer + "/VTLive/VTLogin/VTPortalLogin.aspx?isApp=" + (isModeApp() ? '1' : '0') + "&referer=" + VTPortal.roApp.companyID + "&_=" + new Date().getMilliseconds());
        }
    }

    var checkVisualTimeServer = function (bForceLogin) {
        if (firstAttempt() == true) {
            firstAttempt(false);
            tmpServer = serverLocation().trim();
        }
        tmpIsSSL = isSSL();

        var serverUrlElements = tmpServer.split('.');
        var hasPorts = tmpServer.split(':');

        //if (serverUrlElements.length == 1 && tmpServer != '' && hasPorts.length == 1 && tmpServer != 'localhost') {
        //    tmpServer = tmpServer.concat('.visualtime.net');
        //}
        if (typeof VTPortal.roApp.isMT() != 'undefined' && VTPortal.roApp.isMT().toLowerCase() == "true") {
            VTPortal.roApp.companyID = clientServer();
            VTPortal.roApp.companyID = VTPortal.roApp.companyID.toLowerCase();
            VTPortal.roApp.db.settings.companyID = VTPortal.roApp.companyID;
        }

        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                if (typeof result.ApiVersion != 'undefined') {
                    localStorage.setItem("ApiVersion", result.ApiVersion);
                    VTPortal.roApp.db.settings.ApiVersion = result.ApiVersion;
                } else {
                    VTPortal.roApp.db.settings.ApiVersion = 0;
                }

                if (tmpServer.trim().indexOf('vtportal.visualtime.net') >= 0) {
                    VTPortal.roApp.db.settings.MT = true;
                    VTPortal.roApp.db.settings.wsURL = tmpServer;
                    VTPortal.roApp.db.settings.isSSL = tmpIsSSL;
                    localStorage.setItem('IsMT', VTPortal.roApp.db.settings.MT);

                    VTPortal.roApp.db.settings.saveParameter('wsURL', function () {
                        //Estamos en versión legacy y indicamos en MT, redirijimos a la versión 2.
                        var path = window.location.href.substr(0, window.location.href.indexOf('index.html'));
                        path = path.replace('1/', 'index.html');
                        document.location.href = path;
                    });
                } else {
                    VTPortal.roApp.db.settings.MT = false;
                    localStorage.setItem('IsMT', VTPortal.roApp.db.settings.MT);
                }

                //if ((typeof VTPortal.roApp.isMT() != 'undefined' && VTPortal.roApp.isMT().toLowerCase() == "true") || (tmpServer.trim().indexOf('-vtportal.visualtime.net') > 0)) {
                //    VTPortal.roApp.connectedToMT(true);
                //} else {
                //    VTPortal.roApp.connectedToMT(false);
                //}

                VTPortal.roApp.connectedToMT(false);

                enableSSOLoginButton(!result.Result);

                if (result.Result == true && (result.SSOmixedModeEnabled == undefined || result.SSOmixedModeEnabled == false)) {
                    if (result.SSOmixedModeEnabled != undefined) disableSTDLoginButton(!result.SSOmixedModeEnabled);
                    else disableSTDLoginButton(false);

                    if (!VTPortal.roApp.adfsLoginInprogress) {
                        if (VTPortal.roApp.automaticLogin == true) {
                            prepareSSOLogin();
                        }
                    } else {
                        if (VTPortal.roApp.db.settings.adfsCredential == '') {
                            localStorage.setItem('adfsLoginInprogress', 'false');
                            VTPortal.roApp.adfsLoginInprogress = false;

                            VTPortal.roApp.db.settings.isUsingAdfs = false;
                            VTPortal.roApp.db.settings.adfsCredential = '';

                            VTPortal.roApp.db.settings.save();

                            var adfsUserName = Cookies.get('VTPortalAdfsUserName');
                            var errorText = "Usuario no reconocido"
                            if (adfsUserName != null && adfsUserName != '') {
                                errorText = adfsUserName + ' no existe en VisualTime';
                                Cookies.remove('VTPortalAdfsUserName');
                            }

                            VTPortalUtils.utils.notifyMesage(errorText, 'error', 0, function () { });
                        }
                    }
                } else {
                    disableSTDLoginButton(false);
                    if (typeof result.SSOServerEnabled != 'undefined' && result.SSOServerEnabled == true && result.SSOUserLoggedIn == true) {
                        VTPortal.roApp.ssoEnabled(true);
                        VTPortal.roApp.ssoUserName(result.SSOUserName);
                        if (!bForceLogin) {
                            username(VTPortal.roApp.ssoUserName());
                            passwordForId(VTPortal.roApp.ssoUserName().toLowerCase());
                        }
                    } else {
                        VTPortal.roApp.ssoEnabled(false);
                        VTPortal.roApp.ssoUserName('');
                    }

                    VTPortal.roApp.db.settings.isUsingAdfs = false;
                    VTPortal.roApp.db.settings.adfsCredential = '';
                    VTPortal.roApp.adfsEnabled(false);
                    popupVisible(false);
                    popupMTVisible(false);

                    if (VTPortal.roApp.automaticLogin && bForceLogin) commitLogin(bForceLogin);
                }
            } else {
                VTPortal.roApp.db.settings.wsURL = VTPortal.roApp.db.settings.wsURL.toString().replace("-vtportal.visualtime.net", ".visualtime.net");
                VTPortal.roApp.db.settings.wsURL = VTPortal.roApp.db.settings.wsURL.toString().replace(".visualtime.net", "");
                VTPortal.roApp.db.settings.save();
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () { });
            }
        }, function (error) {
            VTPortal.roApp.companyID = "";
            VTPortal.roApp.db.settings.adfsCredential = '';
            VTPortal.roApp.db.settings.isUsingAdfs = false;
            VTPortal.roApp.adfsEnabled(false);

            if (isModeApp()) {
                var serverUrlElements = serverLocation().split('.');
                if (serverUrlElements.length == 3 && serverLocation().trim().indexOf('.visualtime.net') < 0) {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                        popupVisible(false);
                        if (bForceLogin) commitLogin(bForceLogin);
                    });
                }

                else {
                    if (multiTennantChecked() == false) {
                        if (tmpServer.trim().indexOf('.visualtime.net') > 0 && tmpServer.trim().indexOf('-vtportal.visualtime.net') < 0) {
                            tmpServer = tmpServer.replace(".visualtime.net", "-vtportal.visualtime.net");
                            multiTennantChecked(true);
                            checkVisualTimeServer(bForceLogin);
                        }
                        else if (tmpServer.trim().indexOf('.visualtime.net') > 0 && tmpServer.trim().indexOf('-vtportal.visualtime.net') > 0) {
                            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                                tmpServer = tmpServer.replace("-vtportal.visualtime.net", ".visualtime.net");
                                tmpServer = tmpServer.replace(".visualtime.net", "");
                                popupVisible(false);
                                popupMTVisible(false);
                                if (bForceLogin) commitLogin(bForceLogin);
                            });
                        }
                        else if (serverUrlElements.length == 1 && tmpServer != '') {
                            tmpServer = serverLocation().concat('.visualtime.net');
                            checkVisualTimeServer(bForceLogin);
                        }
                        else if (serverUrlElements.length == 3 && tmpServer != '' && tmpServer.trim().indexOf('.visualtime.net') < 0 && tmpServer.trim().indexOf('-vtportal.visualtime.net') < 0) {
                            tmpServer = tmpServer.concat('.visualtime.net');
                            checkVisualTimeServer(bForceLogin);
                        }
                        else {
                            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                                tmpServer = tmpServer.replace("-vtportal.visualtime.net", ".visualtime.net");
                                tmpServer = tmpServer.replace(".visualtime.net", "");
                                popupVisible(false);
                                popupMTVisible(false);
                                if (bForceLogin) commitLogin(bForceLogin);
                            });
                        }
                    }
                    else {
                        multiTennantChecked(false);
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                            tmpServer = tmpServer.replace("-vtportal.visualtime.net", ".visualtime.net");
                            tmpServer = tmpServer.replace(".visualtime.net", "");
                            popupVisible(false);
                            popupMTVisible(false);
                            if (bForceLogin) commitLogin(bForceLogin);
                        });
                    }
                }
            }
            else {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () {
                    popupVisible(false);
                    popupMTVisible(false);
                    if (bForceLogin) commitLogin(bForceLogin);
                });
            }
        }, tmpServer, tmpIsSSL).isAdfsActive();
    }

    function showExternal(url) {
        setTimeout(function () {
            window.open(url, "_self");
        }, 5000);
    }

    function openVisualTimePortal(url) {
        if (typeof cordova != 'undefined' && typeof cordova.InAppBrowser != 'undefined') {
            var target = "_blank";
            var options = "usewkwebview=yes,fullscreen=yes,hidespinner=yes,hidden=yes,location=no,hideurlbar=yes,hidenavigationbuttons=yes,toolbar=no,clearcache=no,clearsessioncache=no";

            adfsLoginAttempts = 0;
            inAppBrowserRef = cordova.InAppBrowser.open(url, target, options);
            inAppBrowserRef.addEventListener('loadstop', function () { showAdfsLoginPort(true) });
        } else {
            showExternal(url);
            //if (typeof VTPortal.roApp.isMT() != 'undefined' && VTPortal.roApp.isMT().toLowerCase() == "true") {
            //    commitLogin(true);
            //}
            //else {
            //    showExternal(url);
            //}
        }
    }

    var adfsCallback = function (values) {
        var strCloseClass = values[0];
        var bNeedToClose = false;
        try {
            if (strCloseClass != null && strCloseClass.toUpperCase() != "NULL") {
                var tmpObj = JSON.parse(strCloseClass);
                if (typeof tmpObj != 'undefined' && tmpObj != null) {
                    bNeedToClose = tmpObj.close;

                    if (adfsLoginAttempts > 15) {
                        username('');
                        passwordForId('');
                        VTPortal.roApp.db.settings.adfsCredential = '';
                        inAppBrowserRef.close();
                        inAppBrowserRef = null;
                        VTPortalUtils.utils.notifyMesage('No se han podido obtener las credenciales del servidor ADFS', 'error', 0, function () { });
                    } else {
                        if (bNeedToClose && tmpObj.username != '' && tmpObj.token != '') {
                            username(tmpObj.username);
                            passwordForId("\\" + tmpObj.username);
                            VTPortal.roApp.db.settings.adfsCredential = tmpObj.token;

                            if (timer != -1) clearInterval(timer);

                            inAppBrowserRef.close();
                            inAppBrowserRef = null;
                            commitLogin(true);
                        } else {
                            username('');
                            passwordForId('');
                            VTPortal.roApp.db.settings.adfsCredential = '';
                            adfsLoginAttempts = adfsLoginAttempts + 1;
                        }
                    }
                } else {
                    bNeedToClose = false;
                }
            } else {
                bNeedToClose = false;
            }
        } catch (e) {
            bNeedToClose = false;
        }

        timer = setTimeout(function () {
            if (inAppBrowserRef != null) {
                inAppBrowserRef.executeScript({ code: "getLoggedInUserInfo()" }, adfsCallback);
            }
        }, 3000);
    }

    var showAdfsLoginPort = function (raiseBrowser) {
        if (VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(0);
        if (raiseBrowser && inAppBrowserRef != null) inAppBrowserRef.show();

        timer = setTimeout(function () {
            if (inAppBrowserRef != null) {
                inAppBrowserRef.executeScript({ code: "getLoggedInUserInfo()" }, adfsCallback);
            }
        }, 3000);
    }

    //var canUseAdfsLoginVisible = ko.computed(function () {
    //    if (VTPortal.roApp.adfsEnabled() || (VTPortal.roApp.ssoEnabled() && sendUsingSSO())) {
    //        return true;
    //    } else {
    //        return false;
    //    }
    //});

    //var emptyServerLocation = ko.computed(function () {
    //    return ((canUseAdfsLoginVisible() || serverLocation().trim == '') ? true : false);
    //});

    var serverLocationCalculated = ko.computed(function () {
        if (serverLocation().trim == '') {
            i18nextko.i18n.t('roRequieredServer');
        } else {
            return serverLocation().trim().replace('-vtportal.visualtime.net', '.visualtime.net');
        }
    });

    var onKeyUpPopup = function (e) {
        if (e.jQueryEvent.keyCode == 13) checkVisualTimeServer(true);
    }

    var onKeyUp = function (e) {
        if (e.jQueryEvent.keyCode == 13) goPage(e);
    }

    var onKeyUpNext = function (e) {
        if (e.jQueryEvent.keyCode == 13) $('#accessPwd :input').focus();
    }

    var onKeyUpNextEmail = function (e) {
        if (e.jQueryEvent.keyCode == 13) $('#txtRecoverEmail :input').focus();
    }
    var onKeyUpConfirmRecover = function (e) {
        if (e.jQueryEvent.keyCode == 13) recoverPwd(e);
    }

    var onBtnRecoverClick = function () {
        firstAttemptPwd(true);
        multiTennantChecked(false);
        recoverPwdPopupVisible(true);
        $('#txtRecoverUserName :input').focus();
    }

    var onKeyUpNextNewPwd = function (e) {
        if (e.jQueryEvent.keyCode == 13) $('#txtNewPassword :input').focus();
    }

    var onKeyUpConfirmSavePwd = function (e) {
        if (e.jQueryEvent.keyCode == 13) setNewPassword(e);
    }

    var onViewShown = function () {
        //if ((typeof VTPortal.roApp.isMT() != 'undefined' && VTPortal.roApp.isMT().toLowerCase() == "true")  || (serverLocation().trim().indexOf('-vtportal.visualtime.net') > 0)) {
        //    VTPortal.roApp.connectedToMT(true);
        //} else {
        //    VTPortal.roApp.connectedToMT(false);
        //}

        VTPortal.roApp.connectedToMT(false);

        $(".toggle-password").off("click");
        $(".toggle-password").click(function () {
            if (modePwd() == "password") {
                modePwd('text');
            } else {
                modePwd('password');
            }
        });
        serverLocation(serverLocation().replace("-vtportal.visualtime.net", ".visualtime.net"));

        try { navigator.splashscreen.hide(); } catch (e) { }

        VTPortal.roApp.wsRequestCounter(0);

        if (serverLocation().trim() != '') checkVisualTimeServer(false);

        if (VTPortal.roApp.automaticLogin == false) {
            passwordForId('');
            if (VTPortal.roApp.ssoEnabled() == true && VTPortal.roApp.ssoUserName() != '') {
                username(VTPortal.roApp.ssoUserName());
                passwordForId(VTPortal.roApp.ssoUserName().toLowerCase());
            }
        }
    }

    var onBeforeViewSetup = function () {
        VTPortal.roApp.setLanguage(currentLanguage().tag);
    }

    var viewModel = {
        viewShown: onViewShown,
        beforeViewSetup: onBeforeViewSetup,
        lblConfiguration: configuration,
        lblClient: client,
        lblLanguage: lngLogin,
        lblServerLocation: i18nextko.t('serverLocation'),
        lblClientLocation: i18nextko.t('clientLocation'),
        lblSecureConnection: i18nextko.t('isSSL'),
        lblUsername: userLogin,
        lblPassword: pwdLogin,
        isModeApp: isModeApp,
        isMT: isMT,
        txtUsername: {
            placeholder: usernameLogin,
            value: username,
            onKeyUp: onKeyUpNext,
            disabled: disableSTDLoginButton
        },
        txtClient: {
            readOnly: true,
            onFocusIn: showPopupMT,
            value: clientServer,
        },
        txtPwd: {
            placeholder: passwordLogin,
            value: passwordForId,
            mode: modePwd,
            onKeyUp: onKeyUp,
            disabled: disableSTDLoginButton
        },
        txtServer: {
            readOnly: true,
            onFocusIn: showPopup,
            placeholder: serverLocationCalculated,
            value: serverLocation
        },
        currentLanguage: currentLanguage,
        dataSourceLanguages: {
            dataSource: VTPortal.roApp.availableLanguages,
            displayExpr: "Name",
            placeholder: i18nextko.t('language'),
            value: currentLanguage,
            itemTemplate: 'langItemTemplate',
            onItemClick: function (info) {
                switch (info.itemData.ID) {
                    case 'ESP':
                        configuration('configuración')
                        client('servidor')
                        userLogin('usuario')
                        pwdLogin('contraseña')
                        lngLogin('idioma')
                        sendMsgLogin('Enviar')
                        sendSSOLogin('Iniciar sesión con SSO')
                        recoverPwdLogin('He olvidado la contraseña')
                        usernameLogin('Introduzca login')
                        passwordLogin('Introduzca contraseña')
                        recoverTitleLogin('Recuperar contraseña')
                        recoverUserNameLogin('Nombre de usuario')
                        recoverEmailLogin('Dirección de correo')
                        recoverFullInfoLogin('Indique su nombre de usuario y correo electrónico. Al pulsar sobre el botón \"Siguiente\" se le enviará un mensaje con la clave de recuperación que deberá introducir para poder restablecer la contraseña.')
                        sendRecoverRequestLogin('Siguiente')
                        break;
                    case 'CAT':
                        configuration('configuració')
                        client('servidor')
                        userLogin('usuari')
                        pwdLogin('contrasenya')
                        lngLogin('idioma')
                        sendMsgLogin('Enviar')
                        sendSSOLogin('Iniciar sessió amb SSO')
                        recoverPwdLogin('He oblidat la contrasenya')
                        usernameLogin('Introdueixi el login')
                        passwordLogin('Introdueixi la contrasenya')
                        recoverTitleLogin('Recuperar contraseña')
                        recoverUserNameLogin('Nom usuari')
                        recoverEmailLogin('Direcció de correu')
                        recoverFullInfoLogin('Indiqui el seu usuari i correu electrónic.Al premer sobre el botó \"Següent\" el sistema enviarà un correu electrónic amb la clau de de recuperació que haurá de introduïr al següent formulari conjuntament amb la nova contrasenya.')
                        sendRecoverRequestLogin('Següent')
                        break;
                    case 'ENG':
                        configuration('configuration')
                        client('server')
                        userLogin('user')
                        pwdLogin('password')
                        lngLogin('language')
                        sendMsgLogin('Send')
                        sendSSOLogin('Start session with SSO')
                        recoverPwdLogin('I have forgotten my password')
                        usernameLogin('Enter login')
                        passwordLogin('Enter password')
                        recoverTitleLogin('Recover password')
                        recoverUserNameLogin('User name')
                        recoverEmailLogin('Email')
                        recoverFullInfoLogin('Enter your username and email address. By clicking \"Next\" a message will be sent to you with the recovery key that you will be required to enter in order to recover your password.')
                        sendRecoverRequestLogin('Next')
                        break;
                    case 'FRA':
                        configuration('configuration')
                        client('client')
                        userLogin('utilisateur')
                        pwdLogin('mot de passe')
                        lngLogin('langue')
                        sendMsgLogin('Envoyer')
                        sendSSOLogin('Connexion avec SSO')
                        recoverPwdLogin('J`ai oublié mon mot de passe')
                        usernameLogin('Entrez le login')
                        passwordLogin('Entrez le mot de passe')
                        recoverTitleLogin('Récupérer le mot de passe')
                        recoverUserNameLogin('Nom d`utilisateur')
                        recoverEmailLogin('Adresse e-mail')
                        recoverFullInfoLogin('Indiquez votre nom d`utilisateur et votre e-mail.En cliquant sur le bouton \"Suivant\", vous enverrez un message avec la clé de récupération que vous devez saisir pour réinitialiser le mot de passe.')
                        sendRecoverRequestLogin('Suivant')
                        break;
                    case 'ITA':
                        configuration('configurazione')
                        client('cliente')
                        userLogin('utente')
                        pwdLogin('password')
                        lngLogin('lingua')
                        sendMsgLogin('Invia')
                        sendSSOLogin('Inizia la sessione con SSO')
                        recoverPwdLogin('Ho dimenticato la password')
                        usernameLogin('Nome utente')
                        passwordLogin('Immetti la password')
                        recoverTitleLogin('Recupera password.')
                        recoverUserNameLogin('Nome utente')
                        recoverEmailLogin('Indirizzo di posta')
                        recoverFullInfoLogin('Indica il tuo nome utente e il tuo indirizzo email. Cliccando il tasto \"Avanti\" riceverai un messaggio con il codice di recupero che dovrai inserire per poter ripristinare la password.')
                        sendRecoverRequestLogin('Avanti')
                        break;
                    case 'GAL':
                        userLogin('usuario')
                        client('servidor')
                        pwdLogin('contrasinal')
                        lngLogin('linguaxe')
                        sendMsgLogin('Enviar')
                        sendSSOLogin('Iniciar sesión con SSO')
                        recoverPwdLogin('Esqueime o meu contrasinal')
                        usernameLogin('Introduce o usuario')
                        passwordLogin('Introduce o contrasinal')
                        recoverTitleLogin('Recuperar contraseña')
                        recoverUserNameLogin('Nombre de usuario')
                        recoverEmailLogin('Dirección de correo')
                        recoverFullInfoLogin('Indique su nombre de usuario y correo electrónico. Al pulsar sobre el botón \"Siguiente\" se le enviará un mensaje con la clave de recuperación que deberá introducir para poder restablecer la contraseña.')
                        sendRecoverRequestLogin('Enviar')
                        break;
                    case 'POR':
                        userLogin('usuário')
                        client('servidor')
                        pwdLogin('palavra-passe')
                        lngLogin('língua')
                        sendMsgLogin('Mandar')
                        sendSSOLogin('Entrar com SSO')
                        recoverPwdLogin('Recuperar palavra-passe')
                        usernameLogin('Digite o usuário')
                        passwordLogin('Recuperar palavra-passe')
                        recoverTitleLogin('Nova palavra-passe')
                        recoverUserNameLogin('Nome de utilizador')
                        recoverEmailLogin('Endereço de correio')
                        recoverFullInfoLogin('Indique o seu nome e endereço de correio eletrónico. Ao premir o botão “Seguinte” receberá uma mensagem com a senha de recuperação, que deverá introduzir para poder redefinir a palavra-passe.')
                        sendRecoverRequestLogin('Seguinte')
                        break;
                }
                //VTPortal.app.navigate("login", { root: true });
            }
        },
        btnLogin: {
            onClick: goPage,
            text: sendMsgLogin,
            disabled: disableSTDLoginButton
        },
        btnRedirectToSSO: {
            onClick: goToSSOLogin,
            text: sendSSOLogin,
            disabled: enableSSOLoginButton,
            visible: ko.computed(function () { return !VTPortal.roApp.connectedToMT(); })
        },
        btnAcceptConf: {
            onClick: closeOptions,
            text: i18nextko.t('closeConfig'),
        },
        btnAcceptMTConf: {
            onClick: checkMT,
            text: i18nextko.t('closeConfig'),
        },
        btnRecover: {
            onClick: onBtnRecoverClick,
            text: recoverPwdLogin,
            disabled: disableSTDLoginButton
        },
        popupVisible: popupVisible,
        popupMTVisible: popupMTVisible,
        txtServerLocation: {
            value: serverLocation,
            placeholder: i18nextko.t('serverLocation'),
            onKeyUp: onKeyUpPopup
        },
        txtClientLocation: {
            value: clientServer,
            placeholder: i18nextko.t('serverLocation'),
            onKeyUp: onKeyUpPopup
        },
        ckServerSecurity: {
            text: i18nextko.t('isSSL'),
            value: isSSL
        },
        toast: {
            message: toastMessage,
            type: toastType,
            displayTime: 3000,
            visible: toastVisible
        },
        requiredField: {
            validationGroup: 'validationCode',
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        recoverPwdPopupVisible: recoverPwdPopupVisible,
        lblRecoverTitle: recoverTitleLogin,
        lblRecoverUserName: recoverUserNameLogin,
        txtRecoverUserName: {
            placeholder: i18nextko.t('roRecoverUserNamePH'),
            value: recoverUserName,
            onKeyUp: onKeyUpNextEmail
        },
        lblRecoverEmail: recoverEmailLogin,
        txtRecoverEmail: {
            placeholder: i18nextko.t('roRecoverEmailPH'),
            value: recoverEmail,
            onKeyUp: onKeyUpConfirmRecover
        },
        requieredRecoverEmail: {
            validationGroup: 'requestRecover',
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        requieredRecoverUserName: {
            validationGroup: 'requestRecover',
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        btnSendRecoverRequest: {
            validationGroup: 'requestRecover',
            onClick: recoverPwd,
            text: sendRecoverRequestLogin,
        },
        setPwdPopupVisible: setPwdPopupVisible,
        lblSetPassword: i18nextko.t('roSetRecoveredPassword'),
        lblRequestKey: i18nextko.t('roRequestKey'),
        txtRequestKey: {
            placeholder: i18nextko.t('roRequestKeyPH'),
            value: requestKey,
            onKeyUp: onKeyUpNextNewPwd
        },
        commScroll: {
            height: "80%",
            bounceEnabled: false
        },
        lblNewPassword: i18nextko.t('roNewPassword'),
        txtNewPassword: {
            placeholder: i18nextko.t('roNewPasswordPH'),
            mode: 'password',
            value: newPassword,
            onKeyUp: onKeyUpConfirmSavePwd
        },
        requieredRequestKey: {
            validationGroup: 'setRecoveredPwd',
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        requieredNewPassword: {
            validationGroup: 'setRecoveredPwd',
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        btnSetNewPassword: {
            validationGroup: 'setRecoveredPwd',
            onClick: setNewPassword,
            text: i18nextko.t('sendMsg'),
        },
        lblRecoverFullInfo: recoverFullInfoLogin,
        //canUseAdfsLoginVisible: canUseAdfsLoginVisible,
        ssoEnabled: VTPortal.roApp.ssoEnabled,
        ckUseSSOLogin: {
            value: sendUsingSSO,
            text: i18nextko.t('sendUsingSSOIdentity'),
            onValueChanged: function (data) {
                if (VTPortal.roApp.ssoEnabled() && sendUsingSSO()) {
                    username(VTPortal.roApp.ssoUserName());
                    passwordForId(VTPortal.roApp.ssoUserName().toLowerCase());
                } else {
                    username('');
                    passwordForId('')
                }
            }
        }
    };

    return viewModel;
};