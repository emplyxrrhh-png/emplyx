VTPortal.login = function (params) {
    var inAppBrowserRef = null, timer = -1;
    var username = VTPortal.roApp.userName,
        passwordForId = ko.observable(""),
        clientServer = VTPortal.roApp.mtServer,
        serverLocation = ko.observable(''),
        isSSL = VTPortal.roApp.configSSL,
        currentLanguage = ko.observable(window.VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language)),
        toastVisible = ko.observable(false),
        toastMessage = ko.observable(''),
        toastType = ko.observable('error');
    var modePwd = ko.observable('password');
    var userLogin = ko.observable(i18nextko.i18n.t('user'));
    var configuration = ko.observable(i18nextko.i18n.t('configuration'));
    var client = ko.observable(i18nextko.i18n.t('clientLocation'));
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

    var isWebEnvironment = ko.computed(function () {
        if (!VTPortal.roApp.isModeApp()) {
            return 'True';
        } else {
            return 'False';
        }
    });

    var isModeApp = ko.computed(function () {
        if (VTPortal.roApp.isModeApp()) {
            return true;
        } else {
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
        if (VTPortal.roApp.isTimeGate()) return;

        FirebasePlugin.grantPermission(function (gotPermission) {
            if (gotPermission) {
                // getToken();
            }
            console.log("Permission was " + (gotPermission ? "granted" : "denied"));
        });
    };

    var commitLogin = function (bForceLogin) {
        var dxResult = DevExpress.validationEngine.getGroupConfig('validationCode').validate();

        if (VTPortal.roApp.db.settings.isUsingAdfs && VTPortal.roApp.db.settings.adfsCredential == '') {
            VTPortal.roApp.wsRequestCounter(0);
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roLoginFromWeb'), 'info', 0);
        } else {
            if (dxResult.isValid || (VTPortal.roApp.db.settings.isUsingAdfs && VTPortal.roApp.db.settings.adfsCredential != '')) {
                VTPortal.roApp.db.settings.wsURL = VTPortal.roApp.serverURL.defURL;
                VTPortal.roApp.db.settings.isSSL = VTPortal.roApp.serverURL.defSSL;
                VTPortal.roApp.db.settings.login = username().trim();
                VTPortal.roApp.db.settings.language = currentLanguage().ID;
                VTPortal.roApp.db.settings.companyID = VTPortal.roApp.serverURL.companyId;
                
                VTPortal.roApp.mtServer(VTPortal.roApp.serverURL.companyId);
                VTPortal.roApp.db.settings.MT = localStorage.getItem('IsMT') == 'true' ? true : false;

                if (!VTPortal.roApp.db.settings.isUsingAdfs && VTPortal.roApp.db.settings.adfsCredential == '') {
                    new WebServiceRobotics(function (result) {
                        VTPortal.roApp.setLanguage(currentLanguage().tag);
                        if (VTPortal.roApp.isModeApp()) VTPortal.roApp.db.settings.rememberLoginApp = passwordForId().trim();
                        else VTPortal.roApp.db.settings.password = '';

                        passwordForId('');

                        if (VTPortal.roApp.isModeApp() && !VTPortal.roApp.isTimeGate()) {
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

                        VTPortalUtils.utils.onLoginCommitFunc(result, true);
                    }, function (error) { }, VTPortal.roApp.serverURL.url).login(username().trim(), passwordForId().trim(), currentLanguage().ID, VTPortal.roApp.infoVersion, '', true);
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

                        var endSaveCallback = function () {
                            new WebServiceRobotics(function (result) {
                                var onFinish = function () {
                                    VTPortal.roApp.automaticLogin = true;
                                    VTPortal.roApp.db.settings.rememberLogin = true;
                                    if (VTPortal.roApp.isModeApp()) VTPortal.roApp.db.settings.rememberLoginApp = passwordForId().trim();
                                    else VTPortal.roApp.db.settings.password = '';

                                    VTPortal.roApp.setLanguage(currentLanguage().tag);
                                    VTPortal.roApp.redirectAtHome();
                                }

                                window.VTPortalUtils.utils.onCheckUserSession(result, onFinish, currentLanguage().ID);
                            }, function (error) {
                                window.VTPortalUtils.utils.onLoginErrorFunc(error);
                            }, VTPortal.roApp.serverURL.url).getLoggedInUserInfo();
                        }

                        VTPortal.roApp.db.settings.save(endSaveCallback);
                    }
                }
            }
        }
    }

    var recoverPwd = function (params) {
        var result = DevExpress.validationEngine.getGroupConfig('requestRecover').validate();
        if (result.isValid) {
            if (VTPortal.roApp.serverURL.valid) {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        recoverPwdPopupVisible(false);
                        setPwdPopupVisible(true);
                        $('#txtRequestKey :input').focus();
                    } else {
                        VTPortalUtils.utils.processErrorMessage(result, function () { });
                    }
                }, function (error) { }, VTPortal.roApp.serverURL.url).recoverPassword(recoverUserName().trim(), recoverEmail().trim());
            }
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
            }, function (e) { }, VTPortal.roApp.serverURL.url).resetPasswordToNew(recoverUserName().trim(), requestKey().trim(), newPassword().trim());
        }
    };

    var showPopup = function () {
        popupVisible(true);
        $('#txtServerLocation :input').focus();
    }
    var showPopupMT = function () {
        popupMTVisible(true);
        $('#txtClientLocation :input').focus();
    }

    var closeOptions = function () {
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

        if (VTPortal.roApp.db.settings.MT == true) {
            if (VTPortal.roApp.ssoVersion() == 2) {
                openVisualTimePortal(VTPortal.roApp.serverURL.ssoURL + "/Auth/" + VTPortal.roApp.serverURL.companyId.split('-')[0] + "?source=VTPORTAL&isApp=" + (isModeApp() ? '1' : '0') + "&referer=" + VTPortal.roApp.serverURL.companyId.split('-')[0] + "&_=" + new Date().getMilliseconds());
            } else {
                openVisualTimePortal(VTPortal.roApp.serverURL.ssoURL + "/VTLogin/VTPortalLogin.aspx?isApp=" + (isModeApp() ? '1' : '0') + "&referer=" + VTPortal.roApp.serverURL.companyId.split('-')[0] + "&_=" + new Date().getMilliseconds());
            }
        } else {
            openVisualTimePortal(VTPortal.roApp.serverURL.ssoURL + "/VTLogin/VTPortalLogin.aspx?isApp=" + (isModeApp() ? '1' : '0') + "&referer=" + VTPortal.roApp.serverURL.companyId.split('-')[0] + "&_=" + new Date().getMilliseconds());
        }
    }


    var redirectToST = function (serverURL, serverSSL) {
        VTPortal.roApp.db.settings.MT = false;
        VTPortal.roApp.db.settings.wsURL = serverURL;
        VTPortal.roApp.db.settings.isSSL = serverSSL;
        localStorage.setItem('IsMT', VTPortal.roApp.db.settings.MT);
        VTPortal.roApp.db.settings.saveParameter('wsURL', function () {
            VTPortal.roApp.db.settings.saveParameter('ssl', function () {
                var path = window.location.href.substr(0, window.location.href.indexOf('index.html'));
                path = path.replace('2/', 'index.html');
                document.location.href = path;
            });
        });
    }

    var checkVisualTimeServer = async function (bForceLogin) {
        var tmpServer = serverLocation().trim();
        var tmpIsSSL = isSSL();

        var serverToValidate = "";
        var companyToValidate = "";

        if (!isModeApp()) {
            VTPortal.roApp.companyID = clientServer();
            VTPortal.roApp.companyID = VTPortal.roApp.companyID.toLowerCase();
            VTPortal.roApp.db.settings.companyID = VTPortal.roApp.companyID;
            companyToValidate = VTPortal.roApp.db.settings.companyID;
        } else {
            //App o versión legacy
            tmpServer = serverLocation().trim();
            tmpServer = tmpServer.replace(".visualtime.net", "").toLowerCase();
            serverToValidate = tmpServer;
            companyToValidate = tmpServer.split(":")[0].split(".")[0];

            VTPortal.roApp.companyID = tmpServer.split(":")[0].split(".")[0];
            VTPortal.roApp.companyID = VTPortal.roApp.companyID.split("-")[0];
            VTPortal.roApp.companyID = VTPortal.roApp.companyID.toLowerCase();
            VTPortal.roApp.db.settings.companyID = VTPortal.roApp.companyID;

            if (companyToValidate == serverToValidate) serverToValidate = "";
        }

        if (!VTPortal.roApp.serverURL.valid || VTPortal.roApp.serverURL.companyId != companyToValidate) {
            
            VTPortal.roApp.wsRequestCounter(1);
            let urlValidator = new Robotics.Client.ValidateURL();
            VTPortal.roApp.serverURL = await urlValidator.getURLforClient(companyToValidate, serverToValidate, isModeApp());

            if (!VTPortal.roApp.serverURL.isMT && VTPortal.roApp.serverURL.valid) {
                redirectToST(companyToValidate, tmpIsSSL);
                return;
            }


            VTPortal.roApp.wsRequestCounter(0);

            VTPortal.roApp.db.settings.wsURL = '';
            VTPortal.roApp.db.settings.login = '';
            VTPortal.roApp.db.settings.password = '';
            VTPortal.roApp.db.settings.MT = '';
            VTPortal.roApp.db.settings.isSSL = false;
            VTPortal.roApp.db.settings.saveLogin = false;
            VTPortal.roApp.db.settings.rememberLogin = false;
            VTPortal.roApp.db.settings.rememberLoginApp = '';

            VTPortal.roApp.db.settings.save(prepareServerConfig(bForceLogin, tmpServer, tmpIsSSL));
        } else {
            commitServerConfiguration(bForceLogin, tmpServer, tmpIsSSL);
        }
    }

    function prepareServerConfig(bForceLogin, tmpServer, tmpIsSSL) {
        return function () {
            if ((VTPortal.roApp.isModeApp() && VTPortal.roApp.serverURL.valid) || (!VTPortal.roApp.isModeApp() && VTPortal.roApp.serverURL.valid && VTPortal.roApp.serverURL.isMT)) {
                commitServerConfiguration(bForceLogin, tmpServer, tmpIsSSL);
            } else {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () { });
            }
        };
    }

    function commitServerConfiguration(bForceLogin, serverURL, serverSSL) {
        if (VTPortal.roApp.serverURL.valid || ignoreValid) {

            if (typeof VTPortal.roApp.db.settings.serverSSOConfig[VTPortal.roApp.serverURL.companyId] == 'undefined' || VTPortal.roApp.db.settings.serverSSOConfig[VTPortal.roApp.serverURL.companyId] == null) {

                if (VTPortal.roApp.serverURL.isMT) {
                    new WebServiceRobotics(function (result) {
                        if (result.Status == window.VTPortalUtils.constants.OK.value) {
                            VTPortal.roApp.db.settings.serverSSOConfig[VTPortal.roApp.serverURL.companyId] = result;

                            parseGetSSOConfiguration(VTPortal.roApp.db.settings.serverSSOConfig[VTPortal.roApp.serverURL.companyId], bForceLogin, serverURL, serverSSL);
                        } else {
                            VTPortal.roApp.db.settings.serverSSOConfig[VTPortal.roApp.serverURL.companyId] = null;
                            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () { });
                        }
                    }, function (error) { }, VTPortal.roApp.serverURL.url).getSsoConfiguration();
                } else {
                    if (VTPortal.roApp.serverURL.valid) {
                        redirectToST(serverURL, serverSSL);
                        return;
                    }
                }
            } else {
                parseGetSSOConfiguration(VTPortal.roApp.db.settings.serverSSOConfig[VTPortal.roApp.serverURL.companyId], bForceLogin, serverURL, serverSSL);
            }

        } else {
            VTPortal.roApp.db.settings.serverSSOConfig[VTPortal.roApp.serverURL.companyId] = null;
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotValidVisualTimeServer"), 'error', 0, function () { });
        }
    }


    function parseGetSSOConfiguration(result, bForceLogin, tmpServer, tmpIsSSL) {
        if (typeof result.ApiVersion != 'undefined') {
            localStorage.setItem("ApiVersion", result.ApiVersion);
            VTPortal.roApp.db.settings.ApiVersion = result.ApiVersion;
        } else {
            VTPortal.roApp.db.settings.ApiVersion = 0;
        }

        if (typeof result.RefreshConfiguration != 'undefined' && result.RefreshConfiguration != "") {
            VTPortal.roApp.db.settings.refreshConfiguration = JSON.parse(result.RefreshConfiguration.decodeBase64());
            VTPortal.roApp.db.settings.saveParameter('refreshConfig');
        }


        if (VTPortal.roApp.serverURL.isMT) {
            VTPortal.roApp.db.settings.MT = true;
            localStorage.setItem('IsMT', VTPortal.roApp.db.settings.MT);
        } else {
            if (VTPortal.roApp.serverURL.valid) {
                redirectToST(tmpServer, tmpIsSSL);
                return;
            }
        }

        VTPortal.roApp.connectedToMT(false);
        VTPortal.roApp.ssoVersion(result.SSOVersion);
        enableSSOLoginButton(!result.Result);

        if (result.Result && (result.SSOmixedModeEnabled == undefined || !result.SSOmixedModeEnabled)) {
            if (result.SSOmixedModeEnabled != undefined) disableSTDLoginButton(!result.SSOmixedModeEnabled);
            else disableSTDLoginButton(false);

            if (!VTPortal.roApp.adfsLoginInprogress) {
                if (VTPortal.roApp.automaticLogin) {
                    prepareSSOLogin();
                } else {
                    VTPortal.roApp.automaticLogin = true;
                    VTPortal.roApp.db.settings.isUsingAdfs = false;
                    VTPortal.roApp.db.settings.adfsCredential = '';
                    VTPortal.roApp.adfsEnabled(false);
                    popupVisible(false);
                    popupMTVisible(false);
                }
            } else {
                if (VTPortal.roApp.db.settings.adfsCredential == '') {
                    localStorage.setItem('adfsLoginInprogress', 'false');
                    VTPortal.roApp.adfsLoginInprogress = false;

                    VTPortal.roApp.db.settings.isUsingAdfs = false;
                    VTPortal.roApp.db.settings.adfsCredential = '';

                    VTPortal.roApp.db.settings.save();

                    let adfsUserName = Cookies.get('VTPortalAdfsUserName');
                    let errorText = "Usuario no reconocido"
                    if (adfsUserName != null && adfsUserName != '') {
                        errorText = adfsUserName + ' no existe en VisualTime';
                        Cookies.remove('VTPortalAdfsUserName');
                    }

                    VTPortalUtils.utils.notifyMesage(errorText, 'error', 0, function () { });
                }
            }
        } else {
            disableSTDLoginButton(false);
            if (typeof result.SSOServerEnabled != 'undefined' && result.SSOServerEnabled && result.SSOUserLoggedIn) {
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
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roUnkownADFSUser'), 'error', 0, function () { });
                    } else {
                        if (bNeedToClose && tmpObj.username != '' && tmpObj.token != '') {
                            localStorage.setItem('adfsLoginInprogress', 'false');
                            var sTokens = tmpObj.token.decodeBase64().split('#');

                            if (sTokens[1] == 'unknown') {
                                username('');
                                passwordForId('');
                                VTPortal.roApp.db.settings.adfsCredential = '';
                                inAppBrowserRef.close();
                                inAppBrowserRef = null;
                                checkVisualTimeServer(false);
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roUnkownADFSUser'), 'error', 0, function () { });
                            } else {
                                username(tmpObj.username);
                                passwordForId("\\" + tmpObj.username);
                                VTPortal.roApp.db.settings.adfsCredential = tmpObj.token;

                                if (timer != -1) clearInterval(timer);

                                inAppBrowserRef.close();
                                inAppBrowserRef = null;
                                commitLogin(true);
                            }
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
        }, 1500);
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
        VTPortal.roApp.dataCatalogs.destroyDS();

        if (VTPortal.roApp.currentLanguageId() == '') {
            if (typeof VTPortal.roApp.db.settings.language != 'undefined' && VTPortal.roApp.db.settings.language != '') {
                VTPortal.roApp.currentLanguageId(VTPortal.roApp.db.settings.language);
            } else {
                VTPortal.roApp.currentLanguageId('ESP');
            }
        }

        currentLanguage(window.VTPortal.roApp.getLanguageByID(VTPortal.roApp.currentLanguageId()));

        VTPortal.roApp.connectedToMT(false);

        $(".toggle-password").off("click");
        $(".toggle-password").click(function () {
            if (modePwd() == "password") {
                modePwd('text');
            } else {
                modePwd('password');
            }
        });

        serverLocation(VTPortal.roApp.configWS() == '' ? VTPortal.roApp.mtServer() : VTPortal.roApp.configWS());
        serverLocation(serverLocation().replace("-vtportal.visualtime.net", ".visualtime.net"));
        serverLocation(serverLocation().replace(".visualtime.net", ""));

        try { navigator.splashscreen.hide(); } catch (e) { }

        VTPortal.roApp.wsRequestCounter(0);

        if (VTPortal.roApp.isModeApp()) {
            if (serverLocation().trim() != '') checkVisualTimeServer(false);
        } else {
            if (clientServer().trim() != '') checkVisualTimeServer(false);
        }

        if (window.document.URL.indexOf("localhost:8036") >= 0) {
            checkVisualTimeServer(false);
        }

        if (VTPortal.roApp.ssoUnkownError()) {
            VTPortal.roApp.ssoUnkownError(false);
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roUnkownADFSUser'), 'error', 0, function () { });
        } else {
            if (VTPortal.roApp.automaticLogin == false) {
                passwordForId('');
                if (VTPortal.roApp.ssoEnabled() == true && VTPortal.roApp.ssoUserName() != '') {
                    username(VTPortal.roApp.ssoUserName());
                    passwordForId(VTPortal.roApp.ssoUserName().toLowerCase());
                }
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
        lblServerLocation: client,
        lblClientLocation: client,
        lblSecureConnection: i18nextko.t('isSSL'),
        lblUsername: userLogin,
        lblPassword: pwdLogin,
        isModeApp: isModeApp,
        isWebEnvironment: isWebEnvironment,
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
            valueExpr: "ID",
            placeholder: i18nextko.t('language'),
            value: VTPortal.roApp.currentLanguageId,
            itemTemplate: 'langItemTemplate',
            onItemClick: function (info) {
                VTPortal.roApp.setLanguage(info.itemData.tag);
                VTPortal.roApp.currentLanguageId(info.itemData.ID);
                setTimeout(function () { VTPortal.app.navigate("loading/fromLogin", { root: true }); }, 500)
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