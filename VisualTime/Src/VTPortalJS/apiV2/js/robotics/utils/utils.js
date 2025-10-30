(function () {
    var isShowingPopup = ko.observable(false);

    var insertYoutubeVideo = function (video) {
        var flashPlayerVersion = swfobject.getFlashPlayerVersion();

        if (!flashPlayerVersion.major && !flashPlayerVersion.minor && !flashPlayerVersion.release) {
            $("#" + video.htmlElementId).html("<div class='missing-flash'>Flash player isn't installed.</div>");
            var linkToAdobeFlash = $("<a />").text("Install Adobe Flash Player")
                .attr("href", "http://get.adobe.com/flashplayer/")
                .attr("target", "_blank");
            $(".missing-flash").append(linkToAdobeFlash);
            return;
        }

        var params = {
            allowScriptAccess: "always"
        };
        var atts = {
            id: video.htmlElementId
        };
        swfobject.embedSWF("http://www.youtube.com/v/" + video.id + "?enablejsapi=1&playerapiid=ytplayer&version=3",
            video.htmlElementId, video.width, video.height, video.flashPlayerVersion, null, null, params, atts);
    };

    var formatTimeSpan = function (startDate) {
        var startDateTimeStamp = startDate.getTime(),
            nowDateTimeStamp = new Date().getTime(),
            one_day = 1000 * 60 * 60 * 24,
            one_hour = 1000 * 60 * 60,
            one_minute = 1000 * 60;

        var diffInDay = Math.floor((nowDateTimeStamp - startDateTimeStamp) / one_day),
            diffInHours = Math.floor((nowDateTimeStamp - startDateTimeStamp) / one_hour),
            diffInMinutes = Math.ceil((nowDateTimeStamp - startDateTimeStamp) / one_minute);

        if (diffInDay === 0) {
            if (diffInHours > 0) {
                return diffInHours + " hours ago"
            }
            else {
                if (diffInMinutes > 0) {
                    return diffInMinutes + " minutes ago";
                }
                else {
                    return "Now";
                }
            }
        }
        if (diffInDay < 7) {
            return diffInDay + " days ago";
        }
        if (diffInDay < 31) {
            return Math.floor(diffInDay / 7) + " weeks ago";
        }

        if (diffInDay < 365) {
            return Math.floor(diffInDay / 31) + " months ago";
        }

        if (diffInDay >= 365) {
            return diffInDay + " days ago";
        }

        return diffInDay;
    };

    var notImplemented = function () {
        alert("Not implemented for the demo");
    };

    var guuid = function () {
        var UUID = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) { var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8); return v.toString(16); });
        return UUID;
    };

    var lngTag = function (errorCode) {
        for (i in window.VTPortalUtils.constants) {
            if (window.VTPortalUtils.constants[i].value == errorCode) {
                return window.VTPortalUtils.constants[i].tag;
            }
        }
        return '';
    };

    var onLogoutResetParameters = function () {
        if (VTPortal.roApp.adfsEnabled()) {
            VTPortal.roApp.userName('');
        }

        VTPortal.roApp.loggedIn = false;
        VTPortal.roApp.db.settings.adfsCredential = '';
        VTPortal.roApp.db.settings.password = '';
        VTPortal.roApp.db.settings.rememberLoginApp = '';
        VTPortal.roApp.db.settings.rememberLogin = false;
        VTPortal.roApp.db.settings.save();
        VTPortal.roApp.impersonatedIDEmployee = -1;
        VTPortal.roApp.supervisedEmployees = [];
        VTPortal.roApp.LastLogin(null);
        VTPortal.roApp.zonesDS(null);
        VTPortal.roApp.automaticLogin = false;

        VTPortal.roApp.adfsLoginInprogress = false;
        localStorage.setItem('adfsLoginInprogress', 'false');

        VTPortal.roApp.zonesDS(null);

        VTPortal.roApp.zonesDataSource = ko.observable(new DevExpress.data.DataSource({
            store: [],
            searchOperation: "contains",
            searchExpr: "Name"
        }));

        VTPortalUtils.causes = null;

        VTPortal.roApp.causesDataSource(new DevExpress.data.DataSource({
            store: [],
            searchOperation: "contains",
            searchExpr: "Name"
        }));

        if (VTPortal.roApp.refreshId != -1) {
            clearTimeout(VTPortal.roApp.refreshId);
            VTPortal.roApp.refreshId = -1;
        }
    }

    var onLogoutSuccessFunc = function (result) {
        if (result.Status == window.VTPortalUtils.constants.OK.value) {
            VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() + 1);

            VTPortalUtils.utils.onLogoutResetParameters();

            if (typeof cordova != 'undefined' && typeof cordova.InAppBrowser != 'undefined') {
                var target = "_blank";
                var options = "usewkwebview=yes,hidespinner=yes,hidden=yes,location=no,hideurlbar=yes,hidenavigationbuttons=yes,toolbar=no,clearcache=yes,clearsessioncache=yes";
                inAppBrowserRef = cordova.InAppBrowser.open(window.location.href, target, options);
                inAppBrowserRef.addEventListener('loadstop', function () {
                    inAppBrowserRef.close();
                    inAppBrowserRef = null;
                    //REVISAR SI ES CORRECTO O DEJAMOS SOLO login
                    if (VTPortal.roApp.db.settings.isTimeGate == true) {
                        VTPortal.app.navigate("TimeGate", { root: true });
                    } else {
                        VTPortal.app.navigate("login", { root: true });
                    }
                });
                //REVISAR SI ES CORRECTO O DEJAMOS SOLO login
                if (VTPortal.roApp.db.settings.isTimeGate == true) {
                    VTPortal.app.navigate("TimeGate", { root: true });
                } else {
                    VTPortal.app.navigate("login", { root: true });
                }
            } else {
                //REVISAR SI ES CORRECTO O DEJAMOS SOLO login
                if (VTPortal.roApp.db.settings.isTimeGate == true) {
                    VTPortal.app.navigate("TimeGate", { root: true });
                } else {
                    VTPortal.app.navigate("login", { root: true });
                }
            }

            if (VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() - 1);
        } else {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roErrorClosingSession'), 'error', 0)
        }
    }

    var onLogoutErrorFunc = function (error) {
        VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() + 1);
        if (VTPortal.roApp.adfsEnabled()) {
            VTPortal.roApp.userName('');
        }

        VTPortal.roApp.loggedIn = false;
        VTPortal.roApp.db.settings.adfsCredential = '';
        VTPortal.roApp.db.settings.password = '';
        VTPortal.roApp.db.settings.save();
        VTPortal.roApp.impersonatedIDEmployee = -1;
        VTPortal.roApp.supervisedEmployees = [];
        VTPortal.roApp.automaticLogin = false;

        if (VTPortal.roApp.refreshId != -1) {
            clearTimeout(VTPortal.roApp.refreshId);
            VTPortal.roApp.refreshId = -1;
        }

        if (typeof cordova != 'undefined' && typeof cordova.InAppBrowser != 'undefined') {
            var target = "_blank";
            var options = "usewkwebview=yes,hidespinner=yes,hidden=yes,location=no,hideurlbar=yes,hidenavigationbuttons=yes,toolbar=no,clearcache=yes,clearsessioncache=yes";
            inAppBrowserRef = cordova.InAppBrowser.open(window.location.href, target, options);
            inAppBrowserRef.addEventListener('loadstop', function () {
                inAppBrowserRef.close();
                inAppBrowserRef = null;
                VTPortal.app.navigate("login", { root: true });
            });
            VTPortal.app.navigate("login", { root: true });
        } else {
            VTPortal.app.navigate("login", { root: true });
        }
    }

    var onLoginErrorFunc = function (error) {
        VTPortal.roApp.securityToken = '';
        VTPortal.roApp.loggedIn = false;
        VTPortal.roApp.userId = -1;

        VTPortal.roApp.db.settings.deleteRefreshTimestamps();


        VTPortal.roApp.AnywhereLicense = false;
        VTPortal.roApp.RequiereFineLocation = true;
        VTPortal.roApp.DailyRecordEnabled(false);
        VTPortal.roApp.DailyRecordPatternEnabled(false);
        VTPortal.roApp.failedLogins(VTPortal.roApp.failedLogins() + 1);

        VTPortal.roApp.lastRequestFailed(true);
        if (VTPortal.roApp.failedLogins() > 3) {
            VTPortal.roApp.db.settings.adfsCredential = '';
            VTPortal.roApp.db.settings.password = '';
            VTPortal.roApp.db.settings.save();
            VTPortal.roApp.failedLogins(0);
            VTPortal.app.navigate("login", { root: true });
        }
        else {
            setTimeout(function () {
                if (!VTPortal.roApp.loggedIn) VTPortalUtils.utils.loginIfNecessary();
            }, 30000);
        }
    }

    var loadingPanelConf = function () {
        return {
            // message: i18nextko.i18n.t("loadingTitle"),
            showPane: false,
            height: 110,
            shading: true,
            shadingColor: 'rgba(255,255,255,0.5)',
            indicatorSrc: 'Images/DoubleRing.gif',
            visible: VTPortal.roApp.loadPanelVisible
        }
    }

    var deleteRequest = function (requestId, requestType) {
        var rType = requestType;
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                if (!result.Result) {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestNotExists"), 'error', 0);
                } else {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestDeleted"), 'success', 3000);

                    if (rType == 17) {
                        VTPortal.app.navigate("myDailyRecord", { root: true });
                    }
                    else {
                        VTPortal.roApp.redirectAtHome();
                    }
                }
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        }, function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequest"), 'error', 0);
        }).deleteRequest(requestId);
    }

    var markRequestAsRead = function (requestId) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                if (!result.Result) {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestNotExists"), 'error', 0);
                } else {
                    VTPortal.roApp.loadInitialData(false, false, true, false, false);
                }
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        }, function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequest"), 'error', 0);
        }).setRequestReaded(requestId);
    }

    var questionMessage = function (message, Type, notifyTime, onContinue, onCancel, onContinueText, onCancelText) {
        var btnContinueTxt = i18nextko.i18n.t('roContinue');
        var btnCancelTxt = i18nextko.i18n.t('roCancel');

        if (typeof onContinueText != 'undefined' && onContinueText != null && onContinueText != '') btnContinueTxt = onContinueText;
        if (typeof onCancelText != 'undefined' && onCancelText != null && onCancelText != '') btnCancelTxt = onCancelText;

        if (notifyTime == 0) {
            if (VTPortalUtils.utils.isShowingPopup() == false) {
                VTPortalUtils.utils.isShowingPopup(true);
                var title = "";
                var iconType = Type;

                switch (Type) {
                    case "success":
                        title = i18nextko.i18n.t('roSuccessTitle');
                        break;
                    case "error":
                        title = i18nextko.i18n.t('roErrorTitle');
                        break;
                    case "warning":
                        title = i18nextko.i18n.t('roWarningTitle');
                        break;
                    case "info":
                        title = i18nextko.i18n.t('roInfoTitle');
                        break;
                    default:
                        iconType = Type.split('_')[0];
                        title = i18nextko.i18n.t(Type.split('_')[1]);
                        break;
                }

                var customMessageDiv = $('<div>').attr('class', 'roPopupMsg');

                customMessageDiv.append($('<div>').attr('class', 'roPopupIcon ro-icon-' + iconType));
                customMessageDiv.append($('<div>').attr('class', 'roPopupText').text(message));
                var heightDiv = $('<div>').attr('style', 'clear:both');

                var customDialog = DevExpress.ui.dialog.custom({
                    title: title,
                    message: $('<div>').append(customMessageDiv, heightDiv).html(),
                    css: 'roCustomDialog',
                    buttons: [
                        { text: btnContinueTxt, onClick: function () { return 'continue' } },
                        { text: btnCancelTxt, onClick: function () { return 'exit' } }
                    ]
                });

                customDialog.show().done(function (dialogResult) {
                    switch (dialogResult) {
                        case 'continue':
                            if (typeof onContinue != 'undefined') onContinue();
                            break;
                        case 'exit':
                            if (typeof onCancel != 'undefined') onCancel();
                            break;
                    }
                    customDialog.hide();
                    VTPortalUtils.utils.isShowingPopup(false);
                });
            }
        } else {
            DevExpress.ui.notify(message, Type, notifyTime);
        }
    }

    var notifyMessage = function (message, Type, notifyTime, onContinue) {
        if (notifyTime == 0) {
            if (VTPortalUtils.utils.isShowingPopup() == false) {
                VTPortalUtils.utils.isShowingPopup(true);
                var title = "";
                var iconType = Type;

                switch (Type) {
                    case "success":
                        title = i18nextko.i18n.t('roSuccessTitle');
                        break;
                    case "error":
                        title = i18nextko.i18n.t('roErrorTitle');
                        break;
                    case "warning":
                        title = i18nextko.i18n.t('roWarningTitle');
                        break;
                    case "info":
                        title = i18nextko.i18n.t('roInfoTitle');
                        break;
                    default:
                        iconType = Type.split('_')[0];
                        title = i18nextko.i18n.t(Type.split('_')[1]);
                        break;
                }

                var customMessageDiv = $('<div>').attr('class', 'roPopupMsg');
                customMessageDiv.append($('<div>').attr('class', 'roPopupIcon ro-icon-' + iconType));
                customMessageDiv.append($('<div>').attr('class', 'roPopupText').text(message));

                var heightDiv = $('<div>').attr('style', 'clear:both');

                var customDialog = DevExpress.ui.dialog.custom({
                    title: title,
                    message: $('<div>').append(customMessageDiv, heightDiv).html(),
                    css: 'roCustomDialog',
                    buttons: [
                        { text: i18nextko.i18n.t('roClose'), onClick: function () { return 'exit' } }
                    ]
                });

                customDialog.show().done(function (dialogResult) {
                    switch (dialogResult) {
                        case 'exit':
                            if (typeof onContinue != 'undefined') onContinue();
                            break;
                    }
                    customDialog.hide();
                    VTPortalUtils.utils.isShowingPopup(false);
                });
            }
        } else {
            if (typeof onContinue != 'undefined') onContinue();
            DevExpress.ui.notify(message, Type, notifyTime);
        }
    }

    var processErrorMessage = function (result, onContinue) {
        if (typeof result.CustomErrorText == 'undefined') {
            result.CustomErrorText = '';
        }

        if (result.CustomErrorText != '') VTPortalUtils.utils.notifyMesage(result.CustomErrorText, 'error', 0, onContinue);
        else VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0, onContinue);
    }

    var processSupervisorRequestErrorMessage = function (result, onContinue, onCancel) {
        switch (result.Status) {
            case window.VTPortalUtils.constants.REQUEST_ERROR_CustomError.value:
                VTPortalUtils.utils.notifyMesage(result.StatusErrorMsg, 'error', 0, onCancel);
                break;
            case window.VTPortalUtils.constants.REQUEST_PENDING_VALIDATION.value:
            case window.VTPortalUtils.constants.REQUEST_WARNING_NeedConfirmation.value:
                VTPortalUtils.utils.questionMessage(result.StatusErrorMsg, 'warning', 0, onContinue, onCancel);
                break;
            case window.VTPortalUtils.constants.REQUEST_DENIED_DUE_REQUESTVALIDATION.value:
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)) + " " + result.StatusErrorMsg, 'warning', 0, function () { VTPortal.roApp.redirectAtHome(); });
                break;
            case window.VTPortalUtils.constants.REQUEST_DENIED_DUE_SERVERDATA.value:
                VTPortalUtils.utils.notifyMesage(result.StatusErrorMsg, 'error', 0, function () { VTPortal.roApp.redirectAtHome(); });
                break;
            default:
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0, onCancel);
                break;
        }
    }

    var processRequestErrorMessage = function (result, onContinue, onCancel) {
        switch (result.Status) {
            case window.VTPortalUtils.constants.REQUEST_ERROR_CustomError.value:
                VTPortalUtils.utils.notifyMesage(result.StatusErrorMsg, 'error', 0, onCancel);
                break;
            case window.VTPortalUtils.constants.REQUEST_WARNING_NeedConfirmation.value:
                VTPortalUtils.utils.questionMessage(result.StatusErrorMsg, 'warning', 0, onContinue, onCancel);
                break;
            case window.VTPortalUtils.constants.ERROR_CREATING_NOTIFICATION.value:
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'warning', 1500, function () { VTPortal.roApp.redirectAtHome(); });
                break;
            case window.VTPortalUtils.constants.REQUEST_PENDING_VALIDATION.value:
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)) + " " + result.StatusErrorMsg, 'warning', 0, function () { VTPortal.roApp.redirectAtHome(); });
                break;
            case window.VTPortalUtils.constants.REQUEST_DENIED_DUE_REQUESTVALIDATION.value:
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)) + " " + result.StatusErrorMsg, 'warning', 0, function () { VTPortal.roApp.redirectAtHome(); });
                break;
            case window.VTPortalUtils.constants.REQUEST_DENIED_DUE_SERVERDATA.value:
                VTPortalUtils.utils.notifyMesage(result.StatusErrorMsg, 'error', 0, function () { VTPortal.roApp.redirectAtHome(); });
                break;
            default:
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0, onCancel);
                break;
        }
    }

    var processSecurityMessage = function (result) {
        switch (result.Status) {
            case window.VTPortalUtils.constants.LOGIN_RESULT_LOW_STRENGHT_ERROR.value:
            case window.VTPortalUtils.constants.LOGIN_RESULT_MEDIUM_STRENGHT_ERROR.value:
            case window.VTPortalUtils.constants.LOGIN_RESULT_HIGH_STRENGHT_ERROR.value:
            case window.VTPortalUtils.constants.LOGIN_PASSWORD_EXPIRED.value:
                async function redirectHome() {
                    VTPortal.app.navigate("home/1", { root: true });
                }
                VTPortal.roApp.db.settings.save(async function () {
                    await redirectHome();
                    //Mostramos mensaje de alerta
                    setTimeout(function () { VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'warning', 0); }, 200);
                });
                break;
            case window.VTPortalUtils.constants.LOGIN_NEED_TEMPORANY_KEY.value:
            case window.VTPortalUtils.constants.LOGIN_TEMPORANY_KEY_EXPIRED.value:
            case window.VTPortalUtils.constants.LOGIN_INVALID_VALIDATION_CODE.value:
                VTPortal.roApp.db.settings.save(function () {
                    VTPortal.app.navigate("home/2", { root: true });
                });

                break;
            default:
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
                VTPortal.roApp.securityToken = '';
                VTPortal.roApp.loggedIn = false;
                VTPortal.roApp.userId = -1;
                VTPortal.roApp.AnywhereLicense = false;
                VTPortal.roApp.DailyRecordEnabled(false);
                VTPortal.roApp.DailyRecordPatternEnabled(false);
                VTPortal.roApp.RequiereFineLocation = true;
                VTPortal.roApp.db.settings.save(function () { VTPortal.app.navigate("login", { root: true }); });

                break;
        }
    }

    var loginIFNecessary = async function (validationCode, onEndCallback) {
        var app = window.VTPortal.roApp.db.settings;

        var isSSO = false;

        if (VTPortal.roApp.db.settings.adfsCredential != '') {
            var sTokens = VTPortal.roApp.db.settings.adfsCredential.decodeBase64().split('#');
            VTPortal.roApp.guidToken = sTokens[0];
            VTPortal.roApp.securityToken = sTokens[1];

            if (sTokens.length > 2) {
                if (VTPortal.roApp.db.settings.companyID.indexOf(sTokens[2]) == 0) VTPortal.roApp.companyID = VTPortal.roApp.db.settings.companyID;
                else VTPortal.roApp.companyID = sTokens[2];
            }
            isSSO = true;
        }

        if (!VTPortal.roApp.serverURL.valid) {
            let urlValidator = new Robotics.Client.ValidateURL();
            VTPortal.roApp.serverURL = await urlValidator.getURLforClient(VTPortal.roApp.db.settings.companyID, VTPortal.roApp.db.settings.wsURL, VTPortal.roApp.isModeApp());
        }

        if (!window.VTPortal.roApp.loggedIn) {
            if (VTPortal.roApp.serverURL.valid) {
                if (VTPortal.roApp.serverURL.isMT) {

                    if (VTPortal.roApp.isTimeGate()) {
                        new WebServiceRobotics(function (result) {
                            window.VTPortalUtils.utils.clearTimegateData();
                            VTPortal.app.navigate("TimeGate", { root: true });
                        }, function (error) {
                            window.VTPortalUtils.utils.clearTimegateData();
                            VTPortal.app.navigate("TimeGate", { root: true });
                        }, VTPortal.roApp.serverURL.url).logout(VTPortal.roApp.db.settings.UUID);

                    } else {
                        if (!VTPortal.roApp.isModeApp() && VTPortal.roApp.getNumericVersion() >= VTPortal.roApp.getNumericVersion("3.30.0")) {
                            if (!isSSO) {
                                new WebServiceRobotics(function (result) {
                                    this.VTPortal.roApp.failedLogins(0);
                                    VTPortal.roApp.adfsLoginInprogress = false;

                                    window.VTPortalUtils.utils.onCheckUserSession(result, onEndCallback);
                                    window.VTPortalUtils.utils.setActiveTab('home');
                                }, function (error) {
                                    window.VTPortalUtils.utils.onLoginErrorFunc(error);
                                }, VTPortal.roApp.serverURL.url).authenticateSession();
                            } else {
                                new WebServiceRobotics(function (result) {
                                    VTPortal.roApp.adfsLoginInprogress = false;
                                    localStorage.setItem('adfsLoginInprogress', 'false');

                                    this.VTPortal.roApp.failedLogins(0);

                                    window.VTPortalUtils.utils.onCheckUserSession(result, onEndCallback);
                                    window.VTPortalUtils.utils.setActiveTab('home');
                                }, function (error) {
                                    window.VTPortalUtils.utils.onLoginErrorFunc(error);
                                }, VTPortal.roApp.serverURL.url).getLoggedInUserInfo();
                            }
                        } else {
                            if (!isSSO) {
                                var login = app.password;
                                if (login == '') login = app.rememberLoginApp;

                                new WebServiceRobotics(function (result) {
                                    VTPortalUtils.utils.onLoginCommitFunc(result, false, onEndCallback);
                                }, function (error) {
                                    window.VTPortalUtils.utils.onLoginErrorFunc(error);
                                }, VTPortal.roApp.serverURL.url).login(app.login, login, '', VTPortal.roApp.infoVersion, validationCode, false);
                            } else {
                                new WebServiceRobotics(function (result) {
                                    VTPortal.roApp.adfsLoginInprogress = false;
                                    localStorage.setItem('adfsLoginInprogress', 'false');

                                    this.VTPortal.roApp.failedLogins(0);

                                    window.VTPortalUtils.utils.onCheckUserSession(result, onEndCallback);
                                    window.VTPortalUtils.utils.setActiveTab('home');
                                }, function (error) {
                                    window.VTPortalUtils.utils.onLoginErrorFunc(error);
                                }, VTPortal.roApp.serverURL.url).getLoggedInUserInfo();
                            }
                        }
                    }
                } else {
                    VTPortal.roApp.db.settings.MT = false;
                    localStorage.setItem('IsMT', VTPortal.roApp.db.settings.MT);
                    VTPortal.roApp.db.settings.saveParameter('wsURL', function () {
                        VTPortal.roApp.db.settings.saveParameter('ssl', function () {
                            var path = window.location.href.substr(0, window.location.href.indexOf('index.html'));
                            path = path.replace('2/', 'index.html');
                            document.location.href = path;
                        });
                    });
                }
            } else {
                window.VTPortalUtils.utils.onLogoutSuccessFunc({ Status: 0 });
            }
        } else {
            if (typeof onEndCallback == 'function') {
                onEndCallback();
            }
        }
    }

    var encryptString = function (oStr) {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
            var encryptedStr = oStr; //CryptoJS.enc.Hex.parse(oStr);
            var key = CryptoJS.enc.Hex.parse('152a3243b4157617c81f2a6b1c2d3e4f');
            var ivs = CryptoJS.enc.Hex.parse('101a12641415161713391a1c1c1d9e1f');
            var encryptor = CryptoJS.AES.encrypt(encryptedStr, key, { iv: ivs });
            return encryptor.toLocaleString();
        } else {
            return oStr;
        }
    }

    var onCheckUserSession = function (result, onEndCallback, forcedLanguageId) {
        if (result.Status == window.VTPortalUtils.constants.OK.value) {
            VTPortal.roApp.automaticLogin = true;
            VTPortal.roApp.securityToken = result.Token;
            VTPortal.roApp.loggedIn = true;

            VTPortal.roApp.userId = result.UserId;
            var bSaveSettings = VTPortal.roApp.db.settings.deleteRefreshTimestamps();

            VTPortal.roApp.AnywhereLicense = result.AnywhereLicense;
            VTPortal.roApp.DailyRecordEnabled(typeof result.DailyRecordEnabled != 'undefined' ? result.DailyRecordEnabled : false);
            VTPortal.roApp.DailyRecordPatternEnabled(typeof result.DailyRecordPatternEnabled != 'undefined' ? result.DailyRecordPatternEnabled : false);
            VTPortal.roApp.ShowPermissionsIcon(typeof result.ShowPermissionsIcon != 'undefined' ? result.ShowPermissionsIcon : true);
            VTPortal.roApp.isImpersonateEnabled(typeof result.IsImpersonateEnabled != 'undefined' ? result.IsImpersonateEnabled : true);
            VTPortal.roApp.isLatamMex(typeof result.IsLatamMex != 'undefined' ? result.IsLatamMex : false);

            var languageHasChanged = false;

            if (typeof forcedLanguageId != 'undefined' && forcedLanguageId != '') {
                new WebServiceRobotics(function (result) {
                }, function (error) {
                }).updateServerLanguage(forcedLanguageId);
            }
            else {
                if (typeof result.Language != 'undefined' && VTPortal.roApp.db.settings.language != result.Language) {
                    bSaveSettings = true;
                    languageHasChanged = true;
                    VTPortal.roApp.db.settings.language = result.Language;
                }
            }

            VTPortal.roApp.setLanguage(window.VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language).tag);

            VTPortal.roApp.clientTimeZone = moment.tz.guess(true);

            VTPortal.roApp.isSaas(result.IsSaas);
            if (moment(result.LastLogin).year() != 2079) {
                VTPortal.roApp.LastLogin(result.LastLogin);
            }

            if (typeof result.IsCegidHub != 'undefined') {
                VTPortal.roApp.isCegidHub(result.IsCegidHub);
            }

            if (typeof result.IsCommuniquesEnabled != 'undefined') {
                VTPortal.roApp.isCommuniquesEnabled(result.IsCommuniquesEnabled);
            }

            if (typeof result.IsChannelsEnabled != 'undefined') {
                VTPortal.roApp.isChannelsEnabled(result.IsChannelsEnabled);
            }

            new WebServiceRobotics(function (result) {
            }, function (error) {
            }).updateServerTimeZone(VTPortal.roApp.clientTimeZone);

            if (VTPortal.roApp.db.settings.ApiVersion != result.ApiVersion) {
                VTPortal.roApp.db.emptyViewsCatalogs();
                VTPortal.roApp.db.settings.ApiVersion = result.ApiVersion;
                bSaveSettings = true;
            }

            if (typeof result.Consent != 'undefined') {
                VTPortal.roApp.licenseConsent(result.Consent);
            }

            if (VTPortal.roApp.db.settings.licenseAccepted != result.LicenseAccepted) {
                VTPortal.roApp.db.settings.licenseAccepted = result.LicenseAccepted;
                VTPortal.roApp.licenseAccepted(result.LicenseAccepted);
                bSaveSettings = true;
            }

            if (typeof result.EmployeeId != 'undefined') VTPortal.roApp.employeeId = result.EmployeeId;
            else VTPortal.roApp.employeeId = -1;

            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SupervisorPortal && VTPortal.roApp.employeeId == -1) {
                if (VTPortal.roApp.db.settings.onlySupervisor != true) bSaveSettings = true;
                VTPortal.roApp.db.settings.onlySupervisor = true
            } else {
                if (VTPortal.roApp.db.settings.onlySupervisor != false) bSaveSettings = true;
                VTPortal.roApp.db.settings.onlySupervisor = false;
            }

            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SupervisorPortal) {
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled != result.SupervisorPortalEnabled) bSaveSettings = true;

                VTPortal.roApp.db.settings.supervisorPortalEnabled = result.SupervisorPortalEnabled;
            }

            if (typeof result.ShowForbiddenSections != 'undefined') {
                VTPortal.roApp.ShowForbiddenSections = (typeof result.ShowForbiddenSections != 'undefined' ? result.ShowForbiddenSections : true);
                if (VTPortal.roApp.ShowForbiddenSections != VTPortal.roApp.db.settings.showForbiddenSections) {
                    VTPortal.roApp.db.settings.showForbiddenSections = VTPortal.roApp.ShowForbiddenSections;
                    bSaveSettings = true;
                }
            }

            if (typeof result.ShowLogoutHome != 'undefined') {
                VTPortal.roApp.showLogoutHome(typeof result.ShowLogoutHome != 'undefined' ? result.ShowLogoutHome : true);
                if (VTPortal.roApp.showLogoutHome() != VTPortal.roApp.db.settings.showLogoutHome) {
                    VTPortal.roApp.db.settings.showLogoutHome = VTPortal.roApp.showLogoutHome();
                    bSaveSettings = true;
                }
            }

            if (typeof result.ServerTimezone != 'undefined') {
                VTPortal.roApp.serverTimeZone = result.ServerTimezone;
                if (VTPortal.roApp.serverTimeZone != VTPortal.roApp.db.settings.serverTimezone) {
                    VTPortal.roApp.db.settings.serverTimezone = VTPortal.roApp.serverTimeZone;
                    bSaveSettings = true;
                }
            }

            if (typeof result.HeaderMD5 != 'undefined') {
                VTPortal.roApp.HeaderMD5 = result.HeaderMD5;
                if (VTPortal.roApp.HeaderMD5 != VTPortal.roApp.db.settings.HeaderMD5) {
                    VTPortal.roApp.db.settings.HeaderMD5 = VTPortal.roApp.HeaderMD5;
                    bSaveSettings = true;
                    VTPortal.roApp.HeaderChanged(true);
                }
                else {
                    VTPortal.roApp.HeaderChanged(false);
                }
            }

            if (VTPortal.roApp.db.settings.rememberLogin == false ||
                (VTPortal.roApp.isModeApp() && VTPortal.roApp.db.settings.rememberLoginApp == '') ||
                (!VTPortal.roApp.isModeApp() && VTPortal.roApp.db.settings.password != '')) {
                bSaveSettings = true;
            }

            VTPortal.roApp.db.settings.rememberLogin = true;
            if (VTPortal.roApp.isModeApp() && VTPortal.roApp.db.settings.rememberLoginApp == '') {
                VTPortal.roApp.db.settings.rememberLoginApp = VTPortal.roApp.db.settings.password;
            }

            if (!VTPortal.roApp.isModeApp()) VTPortal.roApp.db.settings.password = '';

            if (bSaveSettings) VTPortal.roApp.db.settings.save();

            if (typeof result.SSOServerEnabled != 'undefined' && result.SSOServerEnabled == true && result.SSOUserLoggedIn == true) {
                VTPortal.roApp.ssoEnabled(true);
                VTPortal.roApp.ssoUserName(result.SSOUserName);
            } else {
                VTPortal.roApp.ssoEnabled(false);
                VTPortal.roApp.ssoUserName('');
            }

            var path = '';

            var enabledVersion = localStorage.getItem('VersionEnabled');
            if (typeof result.DefaultVersion != 'undefined') {
                if (result.DefaultVersion == "V1") {
                    if (typeof enabledVersion != 'undefined' && parseInt(enabledVersion, 10) == 2) {
                        if (VTPortal.roApp.isModeApp()) {
                            setTimeout(function () {
                                path = window.location.href.substr(0, window.location.href.indexOf('index.html'));
                                localStorage.setItem('VersionEnabled', '1');
                                document.location.href = path + '../1/index.html';
                            }, 1000)
                            return;
                        }
                        else {
                            setTimeout(function () {
                                path = window.location.href.substr(0, window.location.href.indexOf('index.aspx'));
                                localStorage.setItem('VersionEnabled', '1');
                                document.location.href = path + '../1/indexv1.aspx';
                            }, 1000)
                            return;
                        }
                    }
                }
                else {
                    if (typeof enabledVersion != 'undefined' && parseInt(enabledVersion, 10) == 1) {
                        if (VTPortal.roApp.isModeApp()) {
                            setTimeout(function () {
                                path = window.location.href.substr(0, window.location.href.indexOf('index.html'));
                                localStorage.setItem('VersionEnabled', '2');
                                document.location.href = path + '../2/index.html';
                            }, 1000)
                            return;
                        }
                        else {
                            setTimeout(function () {
                                path = window.location.href.substr(0, window.location.href.indexOf('index.aspx'));
                                localStorage.setItem('VersionEnabled', '2');
                                document.location.href = path + '../2/indexv2.aspx';
                            }, 1000)
                            return;
                        }
                    }
                }
            }

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

            if (languageHasChanged == true) {
                setTimeout(function () { VTPortal.app.navigate("loading", { root: true }); }, 200)
            } else {
                VTPortal.roApp.loadInitialData(true, true, true, true, true, onEndCallback);
            }

        } else {
            VTPortal.roApp.loggedIn = false;
            VTPortal.roApp.db.settings.adfsCredential = '';
            VTPortal.roApp.db.settings.save();
            VTPortal.app.navigate("login", { root: true });
        }
    }

    var clearTimegateData = function () {
        VTPortal.roApp.db.settings.lastStatusRefresh.userId = -999;
        VTPortal.roApp.db.settings.deleteVTPortalUserData();
        VTPortal.roApp.licenseAccepted(true);
        VTPortal.roApp.db.settings.deleteRefreshTimestamps();
        VTPortal.roApp.impersonatedIDEmployee = -1;
        VTPortal.roApp.supervisedEmployees = [];
        VTPortal.roApp.userId = -1;

        VTPortal.roApp.userStatus(null);
        VTPortal.roApp.userCommuniques(null);
        VTPortal.roApp.userSurveys(null);
        VTPortal.roApp.userTelecommute(null);
        VTPortal.roApp.userAlerts(null);
        VTPortal.roApp.timeGateMode(null);
    }


    var onLoginCommitFunc = function (result, redirectAtHome, onEndCallback) {
        let isTimeGate = VTPortal.roApp.isTimeGate();

        let okReturnCodes = [0, -59, -60, -61, -62, -63, -64];
        if (isTimeGate) okReturnCodes = [0];

        if (okReturnCodes.includes(result.Status) && result.Token != null) {
            let languageHasChanged = false;

            if (!isTimeGate) {
                VTPortal.roApp.db.settings.rememberLogin = true;
                VTPortal.roApp.automaticLogin = true;

                if (!redirectAtHome) {
                    if (typeof result.DefaultLanguage != 'undefined' && VTPortal.roApp.db.settings.language != result.DefaultLanguage) {
                        languageHasChanged = true;
                        VTPortal.roApp.db.settings.language = result.DefaultLanguage;
                    }
                }
            } else {
                VTPortal.roApp.db.settings.lastStatusRefresh.userId = -999;
                VTPortal.roApp.db.settings.deleteVTPortalUserData();
                VTPortal.roApp.licenseAccepted(true);
            }

            VTPortal.roApp.securityToken = result.Token;
            VTPortal.roApp.loggedIn = true;

            VTPortal.roApp.userId = result.UserId;
            VTPortal.roApp.db.settings.deleteRefreshTimestamps();

            localStorage.setItem("ApiVersion", result.ApiVersion);

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
            if (typeof result.IsCegidHub != 'undefined') {
                VTPortal.roApp.isCegidHub(result.IsCegidHub);
            }
            if (typeof result.IsCommuniquesEnabled != 'undefined') {
                VTPortal.roApp.isCommuniquesEnabled(result.IsCommuniquesEnabled);
            }
            if (typeof result.IsChannelsEnabled != 'undefined') {
                VTPortal.roApp.isChannelsEnabled(result.IsChannelsEnabled);
            }
            if (typeof result.ShowLegalText != 'undefined' && result.ShowLegalText) {
                VTPortal.roApp.showLegalText(result.ShowLegalText);
                localStorage.setItem("ShowLegalText.VTPortal", "1");
            } else {
                VTPortal.roApp.showLegalText("0");
                localStorage.setItem("ShowLegalText.VTPortal", "0");
            }

            VTPortal.roApp.DailyRecordPatternEnabled(typeof result.DailyRecordPatternEnabled != 'undefined' ? result.DailyRecordPatternEnabled : false);
            VTPortal.roApp.AnywhereLicense = result.AnywhereLicense;
            VTPortal.roApp.RequiereFineLocation = (typeof result.RequiereFineLocation != 'undefined' ? result.RequiereFineLocation : true);
            VTPortal.roApp.DailyRecordEnabled(typeof result.DailyRecordEnabled != 'undefined' ? result.DailyRecordEnabled : false);
            VTPortal.roApp.ShowPermissionsIcon(typeof result.ShowPermissionsIcon != 'undefined' ? result.ShowPermissionsIcon : true);
            VTPortal.roApp.ShowForbiddenSections = (typeof result.ShowForbiddenSections != 'undefined' ? result.ShowForbiddenSections : true);
            VTPortal.roApp.showLogoutHome(typeof result.ShowLogoutHome != 'undefined' ? result.ShowLogoutHome : true);
            VTPortal.roApp.isAD = result.IsAD;
            VTPortal.roApp.db.settings.showForbiddenSections = VTPortal.roApp.ShowForbiddenSections;
            VTPortal.roApp.db.settings.showLogoutHome = result.ShowLogoutHome;
            VTPortal.roApp.db.settings.isAD = result.IsAD;
            VTPortal.roApp.db.settings.AnywhereLicense = result.AnywhereLicense;
            VTPortal.roApp.db.settings.RequiereFineLocation = (typeof result.RequiereFineLocation != 'undefined' ? result.RequiereFineLocation : true);
            VTPortal.roApp.isImpersonateEnabled(typeof result.IsImpersonateEnabled != 'undefined' ? result.IsImpersonateEnabled : true);
            VTPortal.roApp.isLatamMex(typeof result.IsLatamMex != 'undefined' ? result.IsLatamMex : false);

            if (typeof result.EmployeeId != 'undefined') VTPortal.roApp.employeeId = result.EmployeeId;
            else VTPortal.roApp.employeeId = -1;
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SupervisorPortal && VTPortal.roApp.employeeId == -1) VTPortal.roApp.db.settings.onlySupervisor = true
            else VTPortal.roApp.db.settings.onlySupervisor = false;

            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SupervisorPortal) {
                VTPortal.roApp.db.settings.supervisorPortalEnabled = result.SupervisorPortalEnabled;
            }

            VTPortal.roApp.db.settings.save(function () {
                if (!isTimeGate) {
                    if ([-59, -60, -61, -62, -63, -64].includes(result.Status) && result.Token != "") {
                        VTPortalUtils.utils.processSecurityMessage(result);
                    } else {
                        if (redirectAtHome) {
                            VTPortal.roApp.userStatus(null);
                            VTPortal.roApp.userCommuniques(null);
                            VTPortal.roApp.userSurveys(null);
                            VTPortal.roApp.userTelecommute(null);
                            VTPortal.roApp.userAlerts(null);

                            VTPortal.roApp.loadInitialData(true, true, true, true, true, function () { VTPortal.roApp.redirectAtHome(); });
                        } else {
                            if (languageHasChanged) {
                                setTimeout(function () { VTPortal.app.navigate("loading", { root: true }); }, 200)
                            } else {
                                VTPortal.roApp.loadInitialData(true, true, true, true, true, onEndCallback);
                            }
                        }
                        
                    }
                } else {
                    if (redirectAtHome) {
                        VTPortal.roApp.userStatus(null);
                        VTPortal.roApp.userCommuniques(null);
                        VTPortal.roApp.userSurveys(null);
                        VTPortal.roApp.userTelecommute(null);
                        VTPortal.roApp.userAlerts(null);
                        VTPortal.roApp.loadInitialData(true, true, true, true, true, function () {VTPortal.roApp.redirectAtHome(); });
                    } else {
                        if (languageHasChanged) {
                            setTimeout(function () { VTPortal.app.navigate("loading", { root: true }); }, 200)
                        } else {
                            VTPortal.roApp.loadInitialData(true, true, true, true, true, onEndCallback);
                        }
                    }
                }
            });


        } else {
            if (!isTimeGate) VTPortalUtils.utils.processSecurityMessage(result);
        }
    }

    var onLoginSuccessFunc = function (result, callback) {
        VTPortal.roApp.lastRequestFailed(false);
        VTPortal.roApp.offlineCounter(0);
        if (result.Status == window.VTPortalUtils.constants.OK.value) {
            VTPortal.roApp.automaticLogin = true;
            VTPortal.roApp.securityToken = result.Token;
            VTPortal.roApp.loggedIn = true;

            VTPortal.roApp.userId = result.UserId;
            var bSaveSettings = VTPortal.roApp.db.settings.deleteRefreshTimestamps();
            VTPortal.roApp.userStatus(null);
            VTPortal.roApp.userCommuniques(null);
            VTPortal.roApp.userSurveys(null);
            VTPortal.roApp.userTelecommute(null);
            VTPortal.roApp.userAlerts(null);

            VTPortal.roApp.AnywhereLicense = result.AnywhereLicense;
            VTPortal.roApp.RequiereFineLocation = (typeof result.RequiereFineLocation != 'undefined' ? result.RequiereFineLocation : true);
            VTPortal.roApp.DailyRecordEnabled(typeof result.DailyRecordEnabled != 'undefined' ? result.DailyRecordEnabled : false);
            VTPortal.roApp.DailyRecordPatternEnabled(typeof result.DailyRecordPatternEnabled != 'undefined' ? result.DailyRecordPatternEnabled : false);
            VTPortal.roApp.ShowPermissionsIcon(typeof result.ShowPermissionsIcon != 'undefined' ? result.ShowPermissionsIcon : true);
            VTPortal.roApp.isImpersonateEnabled(typeof result.IsImpersonateEnabled != 'undefined' ? result.IsImpersonateEnabled : true);
            VTPortal.roApp.isLatamMex(typeof result.IsLatamMex != 'undefined' ? result.IsLatamMex : false);

            var languageHasChanged = false;
            if (typeof result.DefaultLanguage != 'undefined' && VTPortal.roApp.db.settings.language != result.DefaultLanguage) {
                bSaveSettings = true;
                languageHasChanged = true;
                VTPortal.roApp.db.settings.language = result.DefaultLanguage;
            }

            VTPortal.roApp.setLanguage(window.VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language).tag);

            if (VTPortal.roApp.db.settings.ApiVersion != result.ApiVersion) {
                VTPortal.roApp.db.emptyViewsCatalogs();
                VTPortal.roApp.db.settings.ApiVersion = result.ApiVersion;
                bSaveSettings = true;
            }

            if (typeof result.EmployeeId != 'undefined') VTPortal.roApp.employeeId = result.EmployeeId;
            else VTPortal.roApp.employeeId = -1;

            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SupervisorPortal && VTPortal.roApp.employeeId == -1) {
                if (VTPortal.roApp.db.settings.onlySupervisor != true) bSaveSettings = true;
                VTPortal.roApp.db.settings.onlySupervisor = true
            } else {
                if (VTPortal.roApp.db.settings.onlySupervisor != false) bSaveSettings = true;
                VTPortal.roApp.db.settings.onlySupervisor = false;
            }

            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SupervisorPortal) {
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled != result.SupervisorPortalEnabled) bSaveSettings = true;

                VTPortal.roApp.db.settings.supervisorPortalEnabled = result.SupervisorPortalEnabled;
            }

            if (typeof result.ShowForbiddenSections != 'undefined') {
                VTPortal.roApp.ShowForbiddenSections = (typeof result.ShowForbiddenSections != 'undefined' ? result.ShowForbiddenSections : true);
                if (VTPortal.roApp.ShowForbiddenSections != VTPortal.roApp.db.settings.showForbiddenSections) {
                    VTPortal.roApp.db.settings.showForbiddenSections = VTPortal.roApp.ShowForbiddenSections;
                    bSaveSettings = true;
                }
            }

            if (typeof result.ShowLogoutHome != 'undefined') {
                VTPortal.roApp.showLogoutHome(typeof result.ShowLogoutHome != 'undefined' ? result.ShowLogoutHome : true);
                if (VTPortal.roApp.showLogoutHome() != VTPortal.roApp.db.settings.showLogoutHome) {
                    VTPortal.roApp.db.settings.showLogoutHome = VTPortal.roApp.showLogoutHome();
                    bSaveSettings = true;
                }
            }

            if (typeof result.ServerTimezone != 'undefined') {
                VTPortal.roApp.serverTimeZone = result.ServerTimezone;
                if (VTPortal.roApp.serverTimeZone != VTPortal.roApp.db.settings.serverTimezone) {
                    VTPortal.roApp.db.settings.serverTimezone = VTPortal.roApp.serverTimeZone;
                    bSaveSettings = true;
                }
            }

            if (typeof result.Consent != 'undefined') {
                VTPortal.roApp.licenseConsent(result.Consent);
            }

            if (VTPortal.roApp.licenseAccepted() != result.LicenseAccepted) {
                VTPortal.roApp.db.settings.licenseAccepted = result.LicenseAccepted;
                VTPortal.roApp.licenseAccepted(result.LicenseAccepted);
                bSaveSettings = true;
            }

            if (typeof result.HeaderMD5 != 'undefined') {
                VTPortal.roApp.HeaderMD5 = result.HeaderMD5;
                if (VTPortal.roApp.HeaderMD5 != VTPortal.roApp.db.settings.HeaderMD5) {
                    VTPortal.roApp.db.settings.HeaderMD5 = VTPortal.roApp.HeaderMD5;
                    bSaveSettings = true;
                    VTPortal.roApp.HeaderChanged(true);
                }
                else {
                    VTPortal.roApp.HeaderChanged(false);
                }
            }

            if (VTPortal.roApp.db.settings.rememberLogin == false ||
                (VTPortal.roApp.isModeApp() && VTPortal.roApp.db.settings.rememberLoginApp == '') ||
                (!VTPortal.roApp.isModeApp() && VTPortal.roApp.db.settings.password != '')) {
                bSaveSettings = true;
            }

            VTPortal.roApp.db.settings.rememberLogin = true;
            if (VTPortal.roApp.isModeApp() && VTPortal.roApp.db.settings.rememberLoginApp == '') {
                VTPortal.roApp.db.settings.rememberLoginApp = VTPortal.roApp.db.settings.password;
            }

            if (!VTPortal.roApp.isModeApp()) VTPortal.roApp.db.settings.password = '';

            if (bSaveSettings) VTPortal.roApp.db.settings.save();

            if (typeof result.IsSaas != 'undefined') {
                VTPortal.roApp.isSaas(result.IsSaas);
            }
            if (moment(result.LastLogin).year() != 2079) {
                VTPortal.roApp.LastLogin(result.LastLogin);
            }

            if (typeof result.IsCegidHub != 'undefined') {
                VTPortal.roApp.isCegidHub(result.IsCegidHub);
            }

            if (typeof result.IsCommuniquesEnabled != 'undefined') {
                VTPortal.roApp.isCommuniquesEnabled(result.IsCommuniquesEnabled);
            }

            if (typeof result.IsChannelsEnabled != 'undefined') {
                VTPortal.roApp.isChannelsEnabled(result.IsChannelsEnabled);
            }

            if (typeof result.SSOServerEnabled != 'undefined' && result.SSOServerEnabled == true && result.SSOUserLoggedIn == true) {
                VTPortal.roApp.ssoEnabled(true);
                VTPortal.roApp.ssoUserName(result.SSOUserName);
            } else {
                VTPortal.roApp.ssoEnabled(false);
                VTPortal.roApp.ssoUserName('');
            }

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

            if (languageHasChanged == true) {
                setTimeout(function () { VTPortal.app.navigate("loading", { root: true }); }, 200)
            }
            else {
                VTPortal.roApp.loadInitialData(true, true, true, true, true, callback);
            }
        } else {
            VTPortalUtils.utils.processSecurityMessage(result);
        }
    };

    var doFinalPunchWithPhoto = function (location, type, params, imageURI, reliable, onEndPunch, zoneReliable = true, selectedZone = -1) {
        var endPunchCallback = function () {
            if (typeof onEndPunch != 'undefined') {
                onEndPunch();
            }
        }

        var ws = new WebServiceRobotics(window.VTPortalUtils.utils.parsePunchResult(type, params, endPunchCallback));

        var lat = reliable == true ? location.coords[0] : -1;
        var lng = reliable == true ? location.coords[1] : -1;
        var loc = reliable == true ? location.location : '';
        var addr = reliable == true ? location.fullAddress : '';

        switch (type) {
            case "ta":
                ws.setStatusWithPhoto(params.idCause, params.direction, lat, lng, '', loc, addr, reliable, imageURI, params.nfcTag, params.tcType, zoneReliable, selectedZone);
                break;
            case "task":
                ws.setTaskPunchWithPhoto(params.idTask, params.idNewTask, params.complete, lat, lng, '', loc, addr, params.oldValues, params.newValues, reliable, imageURI, params.tcType);
                break;
            case "cc":
                ws.setCostCenterPunchWithPhoto(params.idCenter, lat, lng, '', loc, addr, reliable, imageURI, params.tcType);
                break;
        }
    }

    var doFinalPunchWithoutPhoto = function (location, type, params, gpsReliable, photoReliable, onEndPunch, zoneReliable = true, selectedZone = -1) {
        var reliable = (gpsReliable && photoReliable);

        var endPunchCallback = function () {
            if (typeof onEndPunch != 'undefined') {
                onEndPunch();
            }
        }

        var ws = new WebServiceRobotics(window.VTPortalUtils.utils.parsePunchResult(type, params, endPunchCallback));

        var lat = -1;
        var lng = -1;
        var loc = '';
        var addr = '';

        if (VTPortal.roApp.empPermissions().MustUseGPS && gpsReliable == true) lat = location.coords[0];
        if (VTPortal.roApp.empPermissions().MustUseGPS && gpsReliable == true) lng = location.coords[1];
        if (VTPortal.roApp.empPermissions().MustUseGPS && gpsReliable == true) loc = location.location;
        if (VTPortal.roApp.empPermissions().MustUseGPS && gpsReliable == true) addr = location.fullAddress;

        switch (type) {
            case "ta":
                //ws.setStatus(params.idCause, params.direction, lat, lng, '', loc, addr, reliable);
                ws.setStatus(params.idCause, params.direction, lat, lng, '', loc, addr, reliable, params.nfcTag, params.tcType, zoneReliable, selectedZone);
                //if(typeof params.fast != 'undefined' && params.fast == true) VTPortal.roApp.homeView.commitPunchResponse(params.direction, true);
                break;
            case "task":
                ws.SetTaskPunch(params.idTask, params.idNewTask, params.complete, lat, lng, '', loc, addr, params.oldValues, params.newValues, reliable, params.tcType);
                break;
            case "cc":
                ws.setCostCenterStatus(params.idCenter, lat, lng, '', loc, addr, reliable, params.tcType);
                break;
        }
    }

    var doFinalPunch = function (location, type, params, reliable, onEndPunch, reliableZone, selectedZone) {
        if (VTPortal.roApp.empPermissions().MustUsePhoto && Object.has(navigator, 'camera')) {
            navigator.camera.getPicture(
                function (imageURI) {
                    doFinalPunchWithPhoto(location, type, params, imageURI, reliable, onEndPunch, reliableZone, selectedZone);
                },
                function (message) {
                    VTPortalUtils.utils.isShowingPopup(false);
                    VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('errorGettingPhoto'), 'info', 0, function () {
                        doFinalPunchWithoutPhoto(location, type, params, reliable, false, onEndPunch, reliableZone, selectedZone);
                    }, function () {
                        VTPortal.roApp.redirectAtHome();
                    }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                },
                {
                    quality: 20,
                    sourceType: Camera.PictureSourceType.CAMERA,
                    destinationType: Camera.DestinationType.FILE_URI,
                    allowEdit: false,
                    encodingType: Camera.EncodingType.JPEG,
                    targetWidth: 200,
                    targetHeight: 200,
                    saveToPhotoAlbum: false,
                    correctOrientation: false,
                    cameraDirection: 1
                }
            );
        } else {
            doFinalPunchWithoutPhoto(location, type, params, reliable, true, onEndPunch, reliableZone, selectedZone);
        }
    }

    var doPunch = function (type, params, onEndPunch) {
        VTPortal.roApp.punchInProgress(true);
        if (VTPortal.roApp.AnywhereLicense) {
            var location = Object.clone(VTPortal.roApp.currentLocationInfo(), true);

            if (VTPortal.roApp.empPermissions().MustUseGPS && !location.reliable) {
                VTPortalUtils.utils.isShowingPopup(false);
                if (VTPortal.roApp.SelectedZone() == -1) {
                    VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('errorGettingGPS'), 'info', 0, function () {
                        doFinalPunch(location, type, params, false, onEndPunch, VTPortal.roApp.ReliableZone(), VTPortal.roApp.SelectedZone());
                    }, function () {
                        VTPortal.roApp.redirectAtHome();
                    }, i18nextko.i18n.t('roContinue'), i18nextko.i18n.t('roCancel'));
                }
                else
                    doFinalPunch(location, type, params, false, onEndPunch, VTPortal.roApp.ReliableZone(), VTPortal.roApp.SelectedZone());
            } else {
                doFinalPunch(location, type, params, true, onEndPunch, VTPortal.roApp.ReliableZone(), VTPortal.roApp.SelectedZone());
            }
        } else {
            doFinalPunchWithoutPhoto({
                coords: [0, 0],
                location: '',
                fullAddress: '',
                reliable: true
            }, type, params, true, true, onEndPunch);
        }
    }

    var reloadLocation = function (onLocationLoaded) {
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
                            location.zone = VTPortal.roApp.getZoneOfCurrentLocation();
                            VTPortal.roApp.currentLocationInfo(location);
                        } else {
                            var location = VTPortal.roApp.currentLocationInfo();
                            location.coords[0] = 0; location.coords[1] = 0;
                            location.location = "API Error parsing city";
                            location.fullAddress = "API Error parsing address";
                            location.reliable = false;
                            location.zone = null;
                            VTPortal.roApp.currentLocationInfo(location);
                        }

                        if (typeof onLocationLoaded != 'undefined' && onLocationLoaded != null) onLocationLoaded();
                    });
                }
                else {
                    if (typeof onLocationLoaded != 'undefined' && onLocationLoaded != null) onLocationLoaded();
                }
            },
            function onError(error) {
                if (error.code == 1) { //PositionError.PERMISSION_DENIED
                    var location = VTPortal.roApp.currentLocationInfo();
                    location.coords[0] = 0; location.coords[1] = 0;
                    location.location = "API Error parsing city";
                    location.fullAddress = "API Error parsing address";
                    location.reliable = false;
                    location.zone = null;
                    VTPortal.roApp.gpsEmployeePermission(false);
                } else {
                    if (error.message.indexOf("Only secure origins are allowed") == 0) {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('chromeGeolocationAccessForbidden'), 'error', 0);
                    } else {
                        var location = VTPortal.roApp.currentLocationInfo();
                        location.coords[0] = 0; location.coords[1] = 0;
                        location.location = "API Error parsing city";
                        location.fullAddress = "API Error parsing address";
                        location.zone = null;
                        location.reliable = false;
                        VTPortal.roApp.currentLocationInfo(location);
                        VTPortal.roApp.gpsEmployeePermission(true);
                    }
                }
                if (typeof onLocationLoaded != 'undefined' && onLocationLoaded != null) onLocationLoaded();
            }, { maximumAge: VTPortal.roApp.RequiereFineLocation ? 600000 : 6000000, timeout: 10000, enableHighAccuracy: VTPortal.roApp.RequiereFineLocation });
    }

    var badgeCount = function (bSupervisorAlerts, bOnlyRequests, bIsGrouped, privateRoApp) {
        if (privateRoApp == null) privateRoApp = VTPortal.roApp;

        if (!bSupervisorAlerts) {
            if (privateRoApp.userNotificationStatus() != null && privateRoApp.userNotificationStatus().HasStatusAlerts) return 1;

            if (privateRoApp.userAlerts() != null && typeof privateRoApp.userAlerts() != 'undefined') {
                let alertNumber = 0
                if (typeof privateRoApp.userAlerts().ScheduleStatus.RequestAlerts != 'undefined') alertNumber = alertNumber + privateRoApp.userAlerts().ScheduleStatus.RequestAlerts.length;
                if (typeof privateRoApp.userAlerts().ScheduleStatus.IncompletePunches != 'undefined') alertNumber = alertNumber + privateRoApp.userAlerts().ScheduleStatus.IncompletePunches.length;
                if (typeof privateRoApp.userAlerts().ScheduleStatus.ForecastDocuments != 'undefined') alertNumber = alertNumber + privateRoApp.userAlerts().ScheduleStatus.ForecastDocuments.length;
                if (privateRoApp.userAlerts().ScheduleStatus.SignDocuments != null && typeof privateRoApp.userAlerts().ScheduleStatus.SignDocuments != 'undefined') alertNumber = alertNumber + privateRoApp.userAlerts().ScheduleStatus.SignDocuments.length;
                if (typeof privateRoApp.userAlerts().ScheduleStatus.Notifications != 'undefined') alertNumber = alertNumber + privateRoApp.userAlerts().ScheduleStatus.Notifications.length;

                if (privateRoApp.userAlerts().ScheduleStatus.ExpiredDocAlert != null) alertNumber = alertNumber + 1;

                return alertNumber;
            } else return '';
        } else {
            if (privateRoApp.userNotificationStatus() != null && privateRoApp.userNotificationStatus().HasSupervisorAlerts) return 1;

            if (privateRoApp.userAlerts() != null && typeof privateRoApp.userAlerts() != 'undefined') {
                if (bOnlyRequests) {
                    if (bIsGrouped) {
                        let alertNumber = privateRoApp.userAlerts().SupervisorStatus.Alerts.length;

                        alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.DocumentsValidation.length > 0) ? 1 : 0;
                        alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.MandatoryDocuments.length > 0) ? 1 : 0;
                        alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.AbsenteeismDocuments.length > 0) ? 1 : 0;
                        alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.GpaAlerts.length > 0) ? 1 : 0;
                        alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.WorkForecastDocuments.length > 0) ? 1 : 0;
                        alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.AccessAuthorizationDocuments.length > 0) ? 1 : 0;

                        return alertNumber;
                    } else {
                        let alertNumber = privateRoApp.userAlerts().SupervisorStatus.Alerts.find(function (alert) { return alert.AlertType == 40 });
                        if (typeof alertNumber != 'undefined' && alertNumber != null) return alertNumber.DetailCount;
                        else return '';
                    }
                } else {
                    if (bIsGrouped) {
                        let alertNumber = (privateRoApp.userAlerts().SupervisorStatus.Alerts.length > 0 ? 1 : 0);

                        if (alertNumber == 0) alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.DocumentsValidation.length > 0) ? 1 : 0;
                        if (alertNumber == 0) alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.MandatoryDocuments.length > 0) ? 1 : 0;
                        if (alertNumber == 0) alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.AbsenteeismDocuments.length > 0) ? 1 : 0;
                        if (alertNumber == 0) alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.GpaAlerts.length > 0) ? 1 : 0;
                        if (alertNumber == 0) alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.WorkForecastDocuments.length > 0) ? 1 : 0;
                        if (alertNumber == 0) alertNumber += (privateRoApp.userAlerts().SupervisorStatus.DocumentAlerts.AccessAuthorizationDocuments.length > 0) ? 1 : 0;

                        var requestNumber = privateRoApp.userAlerts().SupervisorStatus.Alerts.find(function (alert) { return alert.AlertType == 40 });
                        if (typeof requestNumber != 'undefined' && requestNumber != null) {
                            if (requestNumber.DetailCount > 0) alertNumber += 1;
                        }

                        return alertNumber;
                    } else {
                        let alertNumber = privateRoApp.userAlerts().SupervisorStatus.Alerts.length;

                        if (!bIsGrouped) {
                            var requestNumber = privateRoApp.userAlerts().SupervisorStatus.Alerts.find(function (alert) { return alert.AlertType == 40 });
                            if (typeof requestNumber != 'undefined' && requestNumber != null) alertNumber += requestNumber.DetailCount;
                        }

                        return alertNumber;
                    }
                }
            } else return '';
        }
    }

    var parsePunchResult = function (actualType, actualParams, onEndCallback) {
        return function (result) {
            if (typeof nfc != 'undefined') {
                if (cordova.platformId === 'ios') {
                    nfc.invalidateSession();
                }
                VTPortal.roApp.nfcEnabled(false);
                VTPortal.roApp.nfcPunchInProgress(false);
            }
            // VTPortal.roApp.punchInProgress(false);
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.db.settings.markForRefresh(['status', 'punches']);
                
                if (actualType == "ta") {
                    if (typeof result.StatusInfoMsg != 'undefined' && result.StatusInfoMsg != null && result.StatusInfoMsg != "") {
                        VTPortalUtils.utils.notifyMesage(result.StatusInfoMsg, 'success', 2000);
                    }
                    else {
                        if (actualParams.direction == "S") {
                            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roQuickPunchOutDone'), 'success', 2000);
                        }
                        else if (actualParams.direction == "X") {
                            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNFCPunchDone'), 'success', 2000);
                        }
                        else {
                            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roQuickPunchInDone'), 'success', 2000);
                        }
                    }


                    VTPortal.roApp.redirectAtHome();
                }
                else {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('lblPunchDone'), 'success', 2000);
                    VTPortal.roApp.redirectAtHome();
                }

                let extendCallBackFunction = function (iCallback) {
                    return function () {
                        if (typeof iCallback == 'function') iCallback();
                        if (VTPortal.roApp.homeView != null && VTPortalUtils.utils.isOnView('home')) VTPortal.roApp.homeView.refreshModules();
                    }
                }

                VTPortal.roApp.loadInitialData(false, false, false, false, false, extendCallBackFunction(onEndCallback));

            } else if (result.Status == window.VTPortalUtils.constants.PUNCH_ERROR_REPEAT_IN.value || result.Status == window.VTPortalUtils.constants.PUNCH_ERROR_REPEAT_OUT.value) {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'info', 0);
            } else if (result.Status == window.VTPortalUtils.constants.PUNCH_NFC_TAG_NOT_FOUND.value) {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'warning', 3000);
            }
            else {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
            }
            VTPortal.roApp.ZoneSelected(false);
            VTPortal.roApp.SelectedZone(-1);
            VTPortal.roApp.ReliableZone(true);
        }
    }

    var getCellColor = function (calSelectedDays, cDate, period) {
        var color = "#dcdcdc";
        if (period == 'month') {
            if (typeof calSelectedDays[moment(cDate).format("YYYYMMDD")] != 'undefined' && calSelectedDays[moment(cDate).format("YYYYMMDD")] == true) {
                color = '#0046FE';
            }
        } else if (period == 'year') {
            var checkInitial = moment(cDate).startOf('month');
            var checkEnd = moment(cDate).endOf('month');

            while (checkInitial < checkEnd) {
                calSelectedDays[checkInitial.format("YYYYMMDD")]

                if (typeof calSelectedDays[checkInitial.format("YYYYMMDD")] != 'undefined' && calSelectedDays[checkInitial.format("YYYYMMDD")] == true) {
                    color = '#0046FE';
                    break;
                }
                checkInitial = checkInitial.add(1, 'day');
            }
        }

        return color;
    }

    var roDateReviver = function (key, value) {
        var a;
        if (typeof value === 'string') {
            //a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)Z$/.exec(value);
            //if (a) {
            //    return new Date(Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4],
            //                    +a[5], +a[6]));
            //}
            var dateISO = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:[.,]\d+)?Z/i;
            var dateISOUTC = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/i;
            var dateTimezone = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}([+-])\d{2}:\d{2}/i;

            if (dateISO.test(value)) {
                var tmp = new Date(value);

                var offset = tmp.getTimezoneOffset() / 60;
                var hours = tmp.getHours();
                tmp.setHours(hours + offset);

                return tmp;
            }

            if (dateTimezone.test(value)) {
                var a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})[+-](\d{2}):(\d{2})$/.exec(value);
                var tmp = new Date(+a[1], +a[2] - 1, +a[3], +a[4], a[5], +a[6]);
                return tmp;
            }

            if (dateISOUTC.test(value)) {
                var a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}?)$/.exec(value);
                var tmp = new Date(+a[1], +a[2] - 1, +a[3], +a[4], a[5], +a[6]);
                return tmp;
            }
        }
        return value;
    };

    var convertShiftHourToTime = function (minutesc) {
        return function (arg) {
            var mmtMidnight = moment().startOf('day').add(parseInt(arg.value, 10), 'minutes');

            return mmtMidnight.format("HH:mm");
        }
    };

     const hoursToDuration = function (hours) {
        const isNegative = hours < 0;
        const absoluteHours = Math.abs(hours);
        const totalMinutes = absoluteHours * 60;
        const hoursInteger = Math.floor(totalMinutes / 60);
        const minutes = Math.floor(totalMinutes % 60);

        const hoursStr = hoursInteger.toString().padStart(2, '0');
        const minutesStr = minutes.toString().padStart(2, '0');

        return `${isNegative ? '-' : ''}${hoursStr}:${minutesStr}`;
    };


    var convertHoursToTime = function (accrual, noDec) {
        return function (arg) {
            if (accrual.Type == 'O') return (noDec == false ? arg.valueText : parseInt(arg.valueText, 10));
            else {
                var strResult = "0:00";
                if (arg.value == 0) return strResult;

                var sign = "";
                if (arg.value < 0) sign = "-";

                if (typeof accrual.TotalFormat == 'undefined') {
                    var remainder = Math.abs(arg.value) % 1;
                    var remainderTime = new Date(remainder * 3600 * 1000);
                    return sign + Math.floor(Math.abs(arg.value)) + ':' + ('0' + remainderTime.getMinutes()).slice(-2);
                } else {
                    return accrual.TotalFormat;
                }
            }
        }
    };

    var hoursToHHMM = function (hours) {
        return function (arg) {
            var h = String(Math.trunc(hours)).padStart(2, '0');
            var m = String(Math.abs(Math.round((hours - h) * 60))).padStart(2, '0');
            return h + ':' + m;
        }
    }

    var generateAccrualsArrayProductiv = function (accrualsSet) {
        var accruals = [];

        for (var i = 0; i < accrualsSet.length; i++) {
            accrualsSet[i].MaxValue = accrualsSet[i].TaskValue;

            var startValue = 0;
            var endValue = 0;
            var actualValue = 0;

            if (accrualsSet[i].TaskValue <= 0) {
                startValue = accrualsSet[i].TaskValue;
                endValue = 1;
                actualValue = startValue;
            } else {
                startValue = 0;
                endValue = accrualsSet[i].MaxValue;
                actualValue = accrualsSet[i].TaskValue;
            }

            accruals.push({
                Name: accrualsSet[i].TaskName,
                Gauge: {
                    scale: {
                        allowDecimals: true,
                        startValue: startValue,
                        endValue: endValue,
                        ttickInterval: endValue
                        // label: { visible: false, customizeText: VTPortalUtils.utils.convertHoursToTime(accrualsSet[i], false) }
                    },
                    value: actualValue,
                    subvalues: actualValue,
                    subvalueIndicator: {
                        type: "textCloud",
                        color: accrualsSet[i].TaskValue > accrualsSet[i].MaxValue ? "#ff0000" : "#0046FE",
                        text: { customizeText: VTPortalUtils.utils.hoursToHHMM(accrualsSet[i].TaskValue) }
                    }
                }
            });
        }

        return accruals;
    };

    var generateAccrualsArray = function (accrualsSet) {
        var accruals = [];

        for (var i = 0; i < accrualsSet.length; i++) {
            if (accrualsSet[i].MaxValue == 0) accrualsSet[i].MaxValue = accrualsSet[i].Total;

            var startValue = 0;
            var endValue = 0;
            var actualValue = 0;

            if (accrualsSet[i].Total <= 0) {
                startValue = accrualsSet[i].Total;
                endValue = 1;
                actualValue = startValue;
            } else {
                startValue = 0;
                endValue = accrualsSet[i].MaxValue;
                actualValue = accrualsSet[i].Total;
            }

            accruals.push({
                Name: accrualsSet[i].Name,
                Gauge: {
                    scale: {
                        allowDecimals: true,
                        startValue: startValue,
                        endValue: endValue,
                        ttickInterval: endValue
                        // label: { visible: false, customizeText: VTPortalUtils.utils.convertHoursToTime(accrualsSet[i], false) }
                    },
                    value: actualValue,
                    subvalues: actualValue,
                    subvalueIndicator: {
                        type: "textCloud",
                        color: accrualsSet[i].Total > accrualsSet[i].MaxValue ? "#ff0000" : "#0046FE",
                        text: { customizeText: VTPortalUtils.utils.convertHoursToTime(accrualsSet[i], false) }
                    }
                },
                YearWorkPeriod: accrualsSet[i].YearWorkPeriod
            });
        }

        return accruals;
    };

    var wsRobotics = null;
    var approveRefuseRequest = function (idRequest, requestType, bApprove) {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorOnRequestAction"), 'error', 0);
        }

        var onWSResult = function (bApproveMsg) {
            return function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    if (VTPortal.roApp.impersonateOnlyRequest) {
                        VTPortal.roApp.impersonatedIDEmployee = -1;

                        VTPortal.roApp.db.settings.markForRefresh(['status','notifications']);
                        VTPortal.roApp.impersonateOnlyRequest = false;

                        VTPortal.roApp.redirectAtRequestList(requestType, true);
                    } else {
                        VTPortal.roApp.redirectAtRequestList(requestType);
                    }

                    if (bApproveMsg) {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestApproved"), 'success', 2000);
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestDenied"), 'success', 2000);
                    }
                } else {
                    var onContinue = function () {
                        wsRobotics.approveRefuseRequest(idRequest, bApproveMsg, true);
                    }

                    VTPortalUtils.utils.processSupervisorRequestErrorMessage(result, onContinue, function () { });
                }
            }
        };
        wsRobotics = new WebServiceRobotics(onWSResult(bApprove), onWSError);

        wsRobotics.approveRefuseRequest(idRequest, bApprove, false);
    };

    var approveRefuseRequestNew = function (idRequest, requestType, bApprove) {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorOnRequestAction"), 'error', 0);
        }

        var onWSResult = function (bApproveMsg) {
            return function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    if (requestType == 17) {
                        VTPortal.app.navigate("myTeamDailyRecords");
                    }
                    else {
                        VTPortal.app.navigate("myTeamRequests");
                    }

                    if (bApproveMsg) {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestApproved"), 'success', 2000);
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestDenied"), 'success', 2000);
                    }
                } else {
                    var onContinue = function () {
                        wsRobotics.approveRefuseRequestNew(idRequest, bApproveMsg, true);
                    }

                    VTPortalUtils.utils.processSupervisorRequestErrorMessage(result, onContinue, function () { });
                }
            }
        };
        wsRobotics = new WebServiceRobotics(onWSResult(bApprove), onWSError);

        wsRobotics.approveRefuseRequestNew(idRequest, bApprove, false);
    };

    var approveRefuseRequestByEmp = function (idRequest, requestType, bApprove) {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorOnRequestAction"), 'error', 0);
        }

        var onWSResult = function (bApproveMsg) {
            return function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    if (VTPortal.roApp.impersonateOnlyRequest) {
                        VTPortal.roApp.impersonatedIDEmployee = -1;
                        VTPortal.roApp.impersonateOnlyRequest = false;
                        VTPortal.roApp.redirectAtRequestList(requestType, true);
                    } else {
                        VTPortal.roApp.redirectAtRequestList(requestType);
                    }

                    if (bApproveMsg) {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestAccepted"), 'success', 2000);
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestRejected"), 'success', 2000);
                    }
                } else {
                    var onContinue = function () {
                        wsRobotics.approveRefuseRequestByEmployee(idRequest, bApproveMsg, true);
                    }

                    VTPortalUtils.utils.processSupervisorRequestErrorMessage(result, onContinue, function () { });
                }
            }
        };
        wsRobotics = new WebServiceRobotics(onWSResult(bApprove), onWSError);

        wsRobotics.approveRefuseRequestByEmployee(idRequest, bApprove, false);
    };

    var checkIfRequestHasRanquings = function (requestType, idReason) {
        if ($.grep(VTPortal.roApp.empPermissions().Requests, function (eGrep) {
            if (eGrep.RequestType == requestType && eGrep.Permission == true && typeof eGrep.HasRankings != 'undefined' && eGrep.HasRankings == true) {
                var fItem = eGrep.AutomaticReasons.find(function (item) {
                    return item == idReason;
                })
                if (fItem != null) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }).length == 1) {
            return true;
        } else {
            return false;
        }
    }

    var generateGradientFromColor = function (cellInfo) {
        var startColor = '#ffffff';
        var endColor = '#ffffff';

        if (cellInfo != null && cellInfo.Alerts != null) {
            if (cellInfo.Alerts.OnAbsenceDays) { startChange = '21%'; changeToWhite = '50%'; }
            else if (cellInfo.Alerts.OnAbsenceHours || cellInfo.Alerts.OnHolidaysHours) { startChange = '50%'; changeToWhite = '85%'; }
        }

        if (cellInfo != null && cellInfo.MainShift != null) {
            startColor = cellInfo.MainShift.Color;
        }

        if (cellInfo != null && typeof cellInfo.RequestedShift != 'undefined' && cellInfo.RequestedShift != '') {
            endColor = cellInfo.RequestedShift;
        } else {
            endColor = startColor;
        }

        var percentage = "75";

        var gradientStyle = "";
        gradientStyle = 'background: ' + startColor + ';';
        gradientStyle += 'background: -moz-linear-gradient(top, ' + startColor + ' 0%, ' + startColor + ' ' + percentage + '%, ' + endColor + ' ' + percentage + '%, ' + endColor + ' 100%);';
        gradientStyle += 'background: -webkit-gradient(left top, left bottom, color-stop(0%, ' + startColor + '), color-stop(' + percentage + '%, ' + startColor + '), color-stop(' + percentage + '%, ' + endColor + '), color-stop(100%, ' + endColor + '));';
        gradientStyle += 'background: -webkit-linear-gradient(top, ' + startColor + ' 0%, ' + startColor + ' ' + percentage + '%, ' + endColor + ' ' + percentage + '%, ' + endColor + ' 100%);';
        gradientStyle += 'background: -o-linear-gradient(top, ' + startColor + ' 0%, ' + startColor + ' ' + percentage + '%, ' + endColor + ' ' + percentage + '%, ' + endColor + ' 100%);';
        gradientStyle += 'background: -ms-linear-gradient(top, ' + startColor + ' 0%, ' + startColor + ' ' + percentage + '%, ' + endColor + ' ' + percentage + '%, ' + endColor + ' 100%);';
        gradientStyle += 'background: linear-gradient(to bottom, ' + startColor + ' 0%, ' + startColor + ' ' + percentage + '%, ' + endColor + ' ' + percentage + '%, ' + endColor + ' 100%);';
        gradientStyle += 'filter: progid: DXImageTransform.Microsoft.gradient(startColorstr = "' + startColor + '", endColorstr = "' + endColor + '", GradientType = 0);';

        return gradientStyle;
    };

    var causesDS = null;

    var needToRefresh = function (key) {

        if (VTPortal.roApp.impersonatedIDEmployee != -1) return true;

        if (typeof VTPortal.roApp.db.settings.refreshConfiguration[key] == 'undefined') return true;

        var config = VTPortal.roApp.db.settings.refreshConfiguration[key];

        var bNeedToRefresh = true;

        if (config.refresh == 1) {
            let iPeriod = moment(VTPortal.roApp.db.settings.lastStatusRefresh[key].timestamp);
            let now = moment();

            if (config.off != '') {
                let intervals = config.off.split("@");
                for (let i = 0; i < intervals.length; i++) {
                    let xTimes = intervals[i].split("-");

                    let sPeriod = moment(now.format("YYYY-MM-DD") + " " + xTimes[0], "YYYY-MM-DD hh:mm");
                    let ePeriod = moment(now.format("YYYY-MM-DD") + " " + xTimes[1], "YYYY-MM-DD hh:mm");

                    if (now.isBetween(sPeriod, ePeriod)) {
                        bNeedToRefresh = false;
                        break;
                    }
                }
            }

            if (bNeedToRefresh) {
                let diffInterval = config.interval;
                if (config.interval == -1) {
                    iPeriod = iPeriod.startOf('day');
                    now = now.startOf('day').add(1, 'hour');
                    diffInterval = 1440;
                } 

                let duration = moment.duration(now.diff(iPeriod));
                if (duration.asMinutes() <= diffInterval) bNeedToRefresh = false;
            }
        }

        return bNeedToRefresh;
    }

    var isOnView = function (viewName) {

        try {
            let cView = VTPortal.app.navigationManager.currentStack.items[VTPortal.app.navigationManager.currentStack.currentIndex];
            if (cView.uri == viewName) return true;
        } catch (e) {
            return false;
        }
        
        return false;
    }
    var navigateToRequest = function (request) {
        switch (request.IdRequestType) {
            case 1:
                VTPortal.app.navigate('userFields/' + request.ID)
                break;
            case 2:
                VTPortal.app.navigate('forgottenPunch/' + request.ID);
                break;
            case 3:
                VTPortal.app.navigate('justifyPunch/' + request.ID);
                break;
            case 4:
                VTPortal.app.navigate('externalWork/' + request.ID);
                break;
            case 5:
                VTPortal.app.navigate('changeShift/' + request.ID);
                break;
            case 6:
                VTPortal.app.navigate('plannedHoliday/' + request.ID);
                break;
            case 7:
                VTPortal.app.navigate('plannedAbsence/' + request.ID);
                break;
            case 8:
                VTPortal.app.navigate('shiftExchange/' + request.ID);
                break;
            case 9:
                VTPortal.app.navigate('plannedCause/' + request.ID);
                break;
            case 10:
                VTPortal.app.navigate('forgottenPunch/' + request.ID);
                break;
            case 11:
                VTPortal.app.navigate('cancelHolidays/' + request.ID);
                break;
            case 12:
                VTPortal.app.navigate('forgottenPunch/' + request.ID);
                break;
            case 13:
                VTPortal.app.navigate('plannedHoliday/' + request.ID);
                break;
            case 14:
                VTPortal.app.navigate('plannedOvertime/' + request.ID);
                break;
            case 15:
                VTPortal.app.navigate('externalWorkWeekResume/' + request.ID);
                break;
        }
    }

    var updateClientTime = function (seconds) {
        if (!VTPortal.roApp.loggedIn) {
            clearInterval(VTPortal.roApp.refreshClockId);
            return;
        }

        var status = VTPortal.roApp.userStatus();
        if (status != null) {
            status.ServerDate = moment(status.ServerDate).add(seconds, 'seconds').toDate();
            VTPortal.roApp.userStatus(status);
        }
    }

    var logoutTimegate = function () {

        if (VTPortal.roApp.refreshId != -1) clearTimeout(VTPortal.roApp.refreshId);
        VTPortal.roApp.timegateLoggedIn = false;

        if (VTPortal.roApp.logoutTimmer > 0) {
            clearTimeout(VTPortal.roApp.logoutTimmer);
            VTPortal.roApp.logoutTimmer = -1;
        }

        new WebServiceRobotics(function (result) {
            window.VTPortalUtils.utils.clearTimegateData();
        }, function (error) {
            window.VTPortalUtils.utils.clearTimegateData();
        }).logout(VTPortal.roApp.db.settings.UUID);
    }
    var refreshData = function () {
        if (!VTPortal.roApp.loggedIn) VTPortalUtils.utils.loginIfNecessary();
        else {
            if (VTPortal.roApp.db.settings.onlySupervisor && VTPortal.roApp.impersonatedIDEmployee == -1) VTPortal.app.navigate("myteam", { root: true });
            else VTPortal.roApp.loadInitialData(false, false, false, false, false);
        }
    }
    var deleteProgrammedAbsence = function (forecastID, forecastType) {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorDeleteProgrammedAbsence"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtHome();
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roDeleteProgrammedAbsenceSaved"), 'success', 2000);
            } else {
                var onContinue = function () {
                    // wsRobotics.deleteProgrammedAbsence(forecastID, forecastType);
                }

                VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        wsRobotics.deleteProgrammedAbsence(forecastID, forecastType);
    }

    var downloadBytesMobile = function (fileName, data) {
        try {
            fileName = fileName.split("?")[0];
            var re = /(?:\.([^.]+))?$/;
            var fileExtension = re.exec(fileName)[1];

            if (cordova.platformId === 'ios') {
                window.resolveLocalFileSystemURL(cordova.file.documentsDirectory, function (directoryEntry) {
                    directoryEntry.getFile(fileName, { create: true }, function (fileEntry) {
                        fileEntry.createWriter(function (fileWriter) {
                            fileWriter.onwriteend = function (e) {
                                cordova.plugins.fileOpener2.showOpenWithDialog(cordova.file.documentsDirectory + fileName, checkMimeTypes(fileExtension),
                                    {
                                        error: function (e) {
                                            console.log('Error status: ' + e.status + ' - Error message: ' + e.message);
                                        },
                                        success: function () {
                                            console.log('file opened successfully');
                                        }
                                    }
                                );
                            };

                            fileWriter.onerror = function (e) {
                                alert(e.message);
                            };

                            var blob = new Blob([data], { type: checkMimeTypes(fileExtension) });

                            fileWriter.write(blob);
                        }, function onerror(e) {
                            alert(e.message);
                        });
                    }, function onerror(e) {
                        alert(e.message);
                    });
                }, function onerror(e) {
                    alert(e.message);
                });
            }
            else {
                window.resolveLocalFileSystemURL(cordova.file.dataDirectory, function (directoryEntry) {
                    directoryEntry.getFile(fileName, { create: true }, function (fileEntry) {
                        fileEntry.createWriter(function (fileWriter) {
                            fileWriter.onwriteend = function (e) {
                                cordova.plugins.fileOpener2.showOpenWithDialog(cordova.file.dataDirectory + fileName, checkMimeTypes(fileExtension),
                                    {
                                        error: function (e) {
                                            console.log('Error status: ' + e.status + ' - Error message: ' + e.message);
                                        },
                                        success: function () {
                                            console.log('file opened successfully');
                                        }
                                    }
                                );
                            };

                            fileWriter.onerror = function (e) {
                                alert(e.message);
                            };

                            var blob = new Blob([data], { type: checkMimeTypes(fileExtension) });

                            fileWriter.write(blob);
                        }, function onerror(e) {
                            alert(e.message);
                        });
                    }, function onerror(e) {
                        alert(e.message);
                    });
                }, function onerror(e) {
                    alert(e.message);
                });
            }
        } catch (e) {
            alert(e.message);
        }
    }

    var downloadBytes = function (file) {
        var bytes = new Uint8Array(file.DocumentContent);

        if (VTPortal.roApp.isModeApp()) {
            downloadBytesMobile(file.DocumentName, bytes);
        } else {
            var fileName = file.DocumentName.split("?")[0];
            var re = /(?:\.([^.]+))?$/;
            var fileExtension = re.exec(fileName)[1];
            var blob = new Blob([bytes], { type: checkMimeTypes(fileExtension) });

            if (navigator.appVersion.toString().indexOf('.NET') > 0) {
                window.navigator.msSaveBlob(blob, fileName);
            }
            else {
                //var bytes = file.DocumentContent;

                // var blob = new Blob([bytes], { type: "application/octet-stream" });
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                var fileName = fileName;
                link.download = fileName;
                link.click();
            }
        }
    }

    var setActiveTab = function (tab) {
        var tabIndex = -1;
        switch (tab) {
            case 'home':
                tabIndex = 1;
                break;
            case 'status':
            case 'alerts':
                tabIndex = 2;
                break;
            case 'myTeam':
                tabIndex = 3;
                break;
            case 'accruals':
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled) tabIndex = 4;
                else tabIndex = 3;
                break;
            case 'dailyRecord':
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled) tabIndex = 5;
                else tabIndex = 4;
                break;
            case 'punches':
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled) tabIndex = 6;
                else tabIndex = 5; //Posición inicial para empleado

                if (!VTPortal.roApp.DailyRecordEnabled()) tabIndex -= 1; //Si no hay declaración 1 atras
                break;
            case 'surveys':
                tabIndex = -1;
                break;
            case 'communiques':
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
                    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.OnBoardings && VTPortal.roApp.isSaas() == true) tabIndex = 8;
                    else tabIndex = 7;
                }
                else tabIndex = 6; //Posición inicial para empleado

                if (!VTPortal.roApp.DailyRecordEnabled()) tabIndex -= 1; //Si no hay declaración 1 atras
                break;
            case 'channels':
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
                    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.OnBoardings && VTPortal.roApp.isSaas() == true) tabIndex = 9;
                    else tabIndex = 8;
                } else tabIndex = 7; //Posición inicial para empleado

                if (!VTPortal.roApp.DailyRecordEnabled()) tabIndex -= 1; //Si no hay declaración 1 atras
                if (!(VTPortal.roApp.isCegidHub() == false && VTPortal.roApp.isCommuniquesEnabled() == true)) tabIndex -= 1; //Si no hay comunicados 1 atras

                break;
            default:
                break;
        }

        if (tabIndex >= 0) VTPortal.roApp.selectedTab(tabIndex);
    }



    function buildHamburgerMenu() {
        var refreshMenuItems = [];
        var itemIndex = 1;

        //aprovechamos la carga del menú para añadir la clase impersonate al body, para poder tratarlo por css en otras páginas
        if (VTPortal.roApp.impersonatedIDEmployee != -1) {
            $("body").addClass("impersonate");
        } else {
            $("body").removeClass("impersonate");
        }

        if (VTPortal.roApp.impersonatedIDEmployee != -1) {
            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 25, "Name": VTPortal.roApp.impersonatedNameEmployee, "Class": "icon iconImpersonate" });
            itemIndex = itemIndex + 1;
        }

        //Supervisores UNICOS
        if (VTPortal.roApp.db.settings.onlySupervisor) {
            if (VTPortal.roApp.impersonatedIDEmployee == -1) {
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 15, "Name": i18nextko.t('roHomeMyTeamEmployees'), "Class": "icon iconTeam" });
                itemIndex = itemIndex + 1;
                if (VTPortal.roApp.userStatus() == null) {
                    refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 16, "Name": i18nextko.t('roHomeMyTeamAlerts'), "Class": "icon iconAlerts" });
                    itemIndex = itemIndex + 1;
                } else {
                    if (typeof VTPortal.roApp.userStatus().HasAlertPermission == 'undefined' || VTPortal.roApp.userStatus().HasAlertPermission) {
                        refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 16, "Name": i18nextko.t('roHomeMyTeamAlerts'), "Class": "icon iconAlerts", "badge": VTPortal.roApp.supervisorBadge });
                        itemIndex = itemIndex + 1;
                    }
                }
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 17, "Name": i18nextko.t('roHomeMyTeamRequests'), "Class": "icon iconRequests" });
                itemIndex = itemIndex + 1;
                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.OnBoardings) {
                    if (VTPortal.roApp.isSaas() && !VTPortal.roApp.isCegidHub()) {
                        refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 20, "Name": "onboarding", "Class": "icon iconOB" });
                        itemIndex = itemIndex + 1;
                    }
                }
                if (VTPortal.roApp.empPermissions() != null
                    && VTPortal.roApp.empPermissions().Terminals
                    && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SharedPortal) {
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 28, "Name": i18nextko.t('roConfiguration'), "Class": "icon iconConfiguration" });
                    itemIndex = itemIndex + 1;
                }
            }
            else {
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 1, "Name": i18nextko.t('roHomeTitle'), "Class": "icon iconHome" });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 24, "Name": i18nextko.t('roDocumentsStatus'), "Class": "icon iconStatus", "badge": VTPortal.roApp.userStatusBadge });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 2, "Name": i18nextko.t('roGHMenuTitle'), "Class": "icon iconGH" });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 3, "Name": i18nextko.t('roPunchesTitle'), "Class": "icon iconPunches" });
                itemIndex = itemIndex + 1;
                try {
                    if ((VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Requests[3].Permission || VTPortal.roApp.empPermissions().Requests[14].Permission))) {
                        refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 4, "Name": i18nextko.t('roHomeWorkingTitle'), "Class": "icon iconHW" });
                        itemIndex = itemIndex + 1;
                    }
                }
                catch (e) { }
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 6, "Name": i18nextko.t('roHomeLeavesTitle'), "Class": "icon iconLeaves" });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 7, "Name": i18nextko.t('roHomeProfile'), "Class": "icon iconProfile" });
            }
        }
        //Resto Supervisores
        else if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
            if (VTPortal.roApp.impersonatedIDEmployee == -1) {
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 1, "Name": i18nextko.t('roHomeTitle'), "Class": "icon iconHome" });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 24, "Name": i18nextko.t('roDocumentsStatus'), "Class": "icon iconStatus", "badge": VTPortal.roApp.userStatusBadge });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 18, "Name": i18nextko.t('roHomeTeam'), "Class": "icon iconTeam", "badge": VTPortal.roApp.supervisorBadge });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 2, "Name": i18nextko.t('roGHMenuTitle'), "Class": "icon iconGH" });
                itemIndex = itemIndex + 1;
                if (VTPortal.roApp.DailyRecordEnabled()) {
                    refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 22, "Name": i18nextko.t('roDailyRecord'), "Class": "icon iconDailyRecord" });
                    itemIndex = itemIndex + 1;
                }
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 3, "Name": i18nextko.t('roPunchesTitle'), "Class": "icon iconPunches" });
                itemIndex = itemIndex + 1;
                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.OnBoardings) {
                    if (VTPortal.roApp.isSaas() && VTPortal.roApp.isCegidHub() == false) {
                        refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 20, "Name": "onboarding", "Class": "icon iconOB" });
                        itemIndex = itemIndex + 1;
                    }
                }
                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
                    // Revisar si el cliente ha utilizado comunicados y tiene modulo activo (pbi: 1190266)
                    if (VTPortal.roApp.isCegidHub() == false && VTPortal.roApp.isCommuniquesEnabled()) {
                        refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 19, "Name": i18nextko.t('roHomeCommunique'), "Class": "icon iconCommuniquees" });
                        itemIndex = itemIndex + 1;
                    }
                }

                if (VTPortal.roApp.isChannelsEnabled()) { // VTPortal.roApp.isCegidHub() == false) {
                    refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 23, "Name": i18nextko.t('roHomeChannels'), "Class": "icon iconChannels" });
                    itemIndex = itemIndex + 1;
                }

                try {
                    if ((VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Requests[3].Permission || VTPortal.roApp.empPermissions().Requests[14].Permission))) {
                        refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 4, "Name": i18nextko.t('roHomeWorkingTitle'), "Class": "icon iconHW" });
                        itemIndex = itemIndex + 1;
                    }
                }
                catch (e) { }

                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 5, "Name": i18nextko.t('roHomeDocumentsTitle'), "Class": "icon iconDocuments" });
                itemIndex = itemIndex + 1;

                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 6, "Name": i18nextko.t('roHomeLeavesTitle'), "Class": "icon iconLeaves" });
                itemIndex = itemIndex + 1;

                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 7, "Name": i18nextko.t('roHomeProfile'), "Class": "icon iconProfile" });
                if (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Terminals
                    && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SharedPortal) {

                    refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 28, "Name": i18nextko.t('roConfiguration'), "Class": "icon iconConfiguration" });
                    itemIndex = itemIndex + 1;
                }
            }
            else {
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 1, "Name": i18nextko.t('roHomeTitle'), "Class": "icon iconHome" });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 24, "Name": i18nextko.t('roDocumentsStatus'), "badge": VTPortal.roApp.userStatusBadge, "Class": "icon iconStatus" });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 2, "Name": i18nextko.t('roGHMenuTitle'), "Class": "icon iconGH" });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 3, "Name": i18nextko.t('roPunchesTitle'), "Class": "icon iconPunches" });
                itemIndex = itemIndex + 1;
                try {
                    if ((VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Requests[3].Permission || VTPortal.roApp.empPermissions().Requests[14].Permission))) {
                        refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 4, "Name": i18nextko.t('roHomeWorkingTitle'), "Class": "icon iconHW" });
                        itemIndex = itemIndex + 1;
                    }
                }
                catch (e) { }
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 6, "Name": i18nextko.t('roHomeLeavesTitle'), "Class": "icon iconLeaves" });
                itemIndex = itemIndex + 1;
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 7, "Name": i18nextko.t('roHomeProfile'), "Class": "icon iconProfile" });
            }
        }
        //EMPLEADOS
        else {
            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 1, "Name": i18nextko.t('roHomeTitle'), "Class": "icon iconHome" });
            itemIndex = itemIndex + 1;
            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 24, "Name": i18nextko.t('roDocumentsStatus'), "badge": VTPortal.roApp.userStatusBadge, "Class": "icon iconStatus" });
            itemIndex = itemIndex + 1;
            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 2, "Name": i18nextko.t('roGHMenuTitle'), "Class": "icon iconGH" });
            itemIndex = itemIndex + 1;

            if (VTPortal.roApp.DailyRecordEnabled()) {
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 22, "Name": i18nextko.t('roDailyRecord'), "Class": "icon iconDailyRecord" });
                itemIndex = itemIndex + 1;
            }

            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 3, "Name": i18nextko.t('roPunchesTitle'), "Class": "icon iconPunches" });
            itemIndex = itemIndex + 1;

            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
                // Revisar si el cliente ha utilizado comunicados y tiene modulo activo (pbi: 1190266)
                if (VTPortal.roApp.isCegidHub() == false && VTPortal.roApp.isCommuniquesEnabled()) {
                    refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 19, "Name": i18nextko.t('roHomeCommunique'), "Class": "icon iconCommuniquees" });
                    itemIndex = itemIndex + 1;
                }
            }

            if (VTPortal.roApp.isChannelsEnabled()) { // VTPortal.roApp.isCegidHub() == false) {
                refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 23, "Name": i18nextko.t('roHomeChannels'), "Class": "icon iconChannels" });
                itemIndex = itemIndex + 1;
            }

            try {
                if ((VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Requests[3].Permission || VTPortal.roApp.empPermissions().Requests[14].Permission))) {
                    refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 4, "Name": i18nextko.t('roHomeWorkingTitle'), "Class": "icon iconHW" });
                    itemIndex = itemIndex + 1;
                }
            }
            catch (e) { }
            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 5, "Name": i18nextko.t('roHomeDocumentsTitle'), "Class": "icon iconDocuments" });
            itemIndex = itemIndex + 1;

            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 6, "Name": i18nextko.t('roHomeLeavesTitle'), "Class": "icon iconLeaves" });
            itemIndex = itemIndex + 1;

            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 7, "Name": i18nextko.t('roHomeProfile'), "Class": "icon iconProfile" });
            itemIndex = itemIndex + 1;
        }

        if (!VTPortal.roApp.isTimeGate()) {

            let tmpLinks = VTPortal.roApp.db.settings.getCacheDS('weblinks');
            if (tmpLinks == null || window.VTPortalUtils.needToRefresh('weblinks')) {

                new WebServiceRobotics(
                    function (result) {
                        if (result.Status == window.VTPortalUtils.constants.OK.value) {
                            VTPortal.roApp.db.settings.updateCacheDS('weblinks', result.Value);

                            let tmpLinks = result.Value.filter(function (link) {
                                return link.ShowOnPortal === true;
                            });

                            if (tmpLinks.length === 1) {
                                refreshMenuItems.push({
                                    "ID": itemIndex,
                                    "ActionIndex": 30,
                                    "Name": tmpLinks[0].Title,
                                    "Class": "icon iconWeblink",
                                    "URL": tmpLinks[0].URL
                                });

                                itemIndex = itemIndex + 1;
                            } else if (tmpLinks.length > 1) {
                                refreshMenuItems.push({
                                    "ID": itemIndex,
                                    "ActionIndex": 29,
                                    "Name": i18nextko.t('roWebLinksTitle'),
                                    "Class": "icon iconWeblink"
                                });

                                itemIndex = itemIndex + 1;
                            }
                        }

                        addFinalItemsAndSetMenu();
                    },
                    //en caso de error, timeout o cualquier problema, añadimos el logout y VTP sin los weblinks
                    function (error) {
                        addFinalItemsAndSetMenu();
                    }
                ).GetAllPortalWebLinks();


            } else {
                if (tmpLinks != null && (tmpLinks.length == 0 || (tmpLinks.length > 0 && typeof tmpLinks[0].ShowOnPortalDashboard != 'undefined'))) {
                    let links = tmpLinks.filter(function (link) {
                        return link.ShowOnPortal === true;
                    });

                    if (links.length === 1) {
                        refreshMenuItems.push({
                            "ID": itemIndex,
                            "ActionIndex": 30,
                            "Name": links[0].Title,
                            "Class": "icon iconWeblink",
                            "URL": links[0].URL
                        });

                        itemIndex = itemIndex + 1;
                    } else if (links.length > 1) {
                        refreshMenuItems.push({
                            "ID": itemIndex,
                            "ActionIndex": 29,
                            "Name": i18nextko.t('roWebLinksTitle'),
                            "Class": "icon iconWeblink"
                        });

                        itemIndex = itemIndex + 1;
                    }

                    addFinalItemsAndSetMenu();
                } else {
                    VTPortal.roApp.db.settings.markForRefresh(['weblinks']);
                    addFinalItemsAndSetMenu();
                }
            }
        } else {
            //Si no es TimeGate, añadimos el logout y VTP sin los weblinks
            addFinalItemsAndSetMenu();
        }

        function addFinalItemsAndSetMenu() {
            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 26, "Name": i18nextko.t('roLogout'), "Class": "icon iconLogout" });
            itemIndex = itemIndex + 1;

            refreshMenuItems.push({ "ID": itemIndex, "ActionIndex": 0, "Name": "VTP " + VTPortal.roApp.infoVersion, "Class": "" });

            VTPortal.roApp.mainMenuItems(refreshMenuItems);
        }
    }


    window.VTPortalUtils = {
        dateReviver: roDateReviver,
        needToRefresh: needToRefresh,
        causes: causesDS,
        nullDate: moment(new Date(1970, 0, 1, 0, 0, 0, 0)),
        apiVersion: {
            InitialVersion: 1,
            Holidays: 2,
            LicenseAgreement: 3,
            CauseNote: 4,
            LogoutHome: 5,
            SupervisorPortal: 6,
            Adfs: 7,
            CausesPermissions: 8,
            PasswordEncrypt: 9,
            PortalNext: 10,
            Communiques: 11,
            SendDocuments: 12,
            OnBoardings: 13,
            DocumentSign: 14,
            Surveys: 15,
            Telecommuting: 16,
            CustomHeader: 17,
            Firebase: 18,
            NewRequests: 19,
            RecoverSession: 20,
            SharedPortal: 21
        },
        taskAction: {
            begin: 1,
            change: 2,
            complete: 3
        },
        calendar: {
            getCellColor: getCellColor
        },
        utils: {
            downloadBytes: downloadBytes,
            deleteProgrammedAbsence: deleteProgrammedAbsence,
            refreshData: refreshData,
            logoutTimegate: logoutTimegate,
            isOnView: isOnView,
            navigateToRequest: navigateToRequest,
            generateGradientFromColor: generateGradientFromColor,
            checkIfRequestHasRanquings: checkIfRequestHasRanquings,
            approveRefuseRequest: approveRefuseRequest,
            approveRefuseRequestNew: approveRefuseRequestNew,
            approveRefuseRequestByEmp: approveRefuseRequestByEmp,
            convertShiftHourToTime: convertShiftHourToTime,
            hoursToDuration: hoursToDuration,
            hoursToHHMM: hoursToHHMM,
            convertHoursToTime: convertHoursToTime,
            generateAccrualsArray: generateAccrualsArray,
            generateAccrualsArrayProductiv: generateAccrualsArrayProductiv,
            badgeCount: badgeCount,
            insertYoutubeVideo: insertYoutubeVideo,
            formatTimeSpan: formatTimeSpan,
            notImplemented: notImplemented,
            generateGuid: guuid,
            languageTag: lngTag,
            reloadLocation: reloadLocation,
            doPunch: doPunch,
            parsePunchResult: parsePunchResult,
            clearTimegateData: clearTimegateData,
            loginIfNecessary: loginIFNecessary,
            onLoginCommitFunc: onLoginCommitFunc,
            onLoginSuccessFunc: onLoginSuccessFunc,
            onLogoutSuccessFunc: onLogoutSuccessFunc,
            onLogoutResetParameters: onLogoutResetParameters,
            onLogoutErrorFunc: onLogoutErrorFunc,
            onCheckUserSession: onCheckUserSession,
            onLoginErrorFunc: onLoginErrorFunc,
            processSecurityMessage: processSecurityMessage,
            processErrorMessage: processErrorMessage,
            processRequestErrorMessage: processRequestErrorMessage,
            processSupervisorRequestErrorMessage: processSupervisorRequestErrorMessage,
            notifyMesage: notifyMessage,
            questionMessage: questionMessage,
            loadingPanelConf: loadingPanelConf,
            markRequestAsRead: markRequestAsRead,
            deleteRequest: deleteRequest,
            setActiveTab: setActiveTab,
            datetimeTypeSelect: function () {
                return VTPortal.roApp.isModeApp() ? 'rollers' : 'rollers';
            },
            dailyRecordDatetimeTypeSelect: function () {
                return VTPortal.roApp.isModeApp() ? 'rollers' : 'native';
            },
            updateClientTime: updateClientTime,
            encryptString: encryptString,
            isShowingPopup: isShowingPopup,
            buildHamburgerMenu: buildHamburgerMenu

        },
        constants: {
            OK: { 'value': 0, 'tag': 'OK' },
            NO_SESSION: { 'value': -1, 'tag': 'NO_SESSION' },
            BAD_CREDENTIALS: { 'value': -2, 'tag': 'BAD_CREDENTIALS' },
            NOT_FOUND: { 'value': -3, 'tag': 'NOT_FOUND' },
            GENERAL_ERROR: { 'value': -4, 'tag': 'GENERAL_ERROR' },
            WRONG_MEDIA_TYPE: { 'value': -5, 'tag': 'WRONG_MEDIA_TYPE' },
            NOT_LICENSED: { 'value': -6, 'tag': 'NOT_LICENSED' },
            SERVER_NOT_RUNNING: { 'value': -7, 'tag': 'SERVER_NOT_RUNNING' },
            NO_LIVE_PORTAL: { 'value': -8, 'tag': 'NO_LIVE_PORTAL' },
            PUNCH_ERROR_DO_SEQUENCE: { 'value': -9, 'tag': 'PUNCH_ERROR_DO_SEQUENCE' },
            PUNCH_ERROR_SEQ_STATUS_OK: { 'value': -10, 'tag': 'PUNCH_ERROR_SEQ_STATUS_OK' },
            PUNCH_ERROR_REPEAT_IN: { 'value': -11, 'tag': 'PUNCH_ERROR_REPEAT_IN' },
            PUNCH_ERROR_REPEAT_OUT: { 'value': -12, 'tag': 'PUNCH_ERROR_REPEAT_OUT' },
            PUNCH_ERROR_MAX_SEQ_OK: { 'value': -13, 'tag': 'PUNCH_ERROR_MAX_SEQ_OK' },
            PUNCH_ERROR_MAX_SEQ_ERR: { 'value': -14, 'tag': 'PUNCH_ERROR_MAX_SEQ_ERR' },
            PUNCH_ERROR_NO_SEQUENCE: { 'value': -15, 'tag': 'PUNCH_ERROR_NO_SEQUENCE' },
            PUNCH_ERROR_DELETING: { 'value': -16, 'tag': 'PUNCH_ERROR_DELETING' },
            PUNCH_ERROR_SAVING: { 'value': -17, 'tag': 'PUNCH_ERROR_SAVING' },
            PUNCH_ERROR_FORBIDDEN: { 'value': -18, 'tag': 'PUNCH_ERROR_FORBIDDEN' },
            REQUEST_PUNCH_ERROR: { 'value': -19, 'tag': 'REQUEST_PUNCH_ERROR' },
            FORBID_INCORRECT_DATE_BEFORE_PUNCH: { 'value': -20, 'tag': 'FORBID_INCORRECT_DATE_BEFORE_PUNCH' },
            FORBID_INCORRECT_DATE_FUTURE: { 'value': -21, 'tag': 'FORBID_INCORRECT_DATE_FUTURE' },
            FORBID_ERROR_SAVE_PUNCH: { 'value': -22, 'tag': 'FORBID_ERROR_SAVE_PUNCH' },
            FORBID_ERROR_DO_SEQUENCE: { 'value': -23, 'tag': 'FORBID_ERROR_DO_SEQUENCE' },
            USER_FIELDS_ACCESS_DENIED: { 'value': -24, 'tag': 'USER_FIELDS_ACCESS_DENIED' },
            REQUEST_ERROR_ConnectionError: { 'value': -25, 'tag': 'REQUEST_ERROR_ConnectionError' },
            REQUEST_ERROR_SqlError: { 'value': -26, 'tag': 'REQUEST_ERROR_SqlError' },
            REQUEST_ERROR_NoDeleteBecauseNotPending: { 'value': -27, 'tag': 'REQUEST_ERROR_NoDeleteBecauseNotPending' },
            REQUEST_ERROR_IncorrectDates: { 'value': -28, 'tag': 'REQUEST_ERROR_IncorrectDates' },
            REQUEST_ERROR_NoApprovePermissions: { 'value': -29, 'tag': 'REQUEST_ERROR_NoApprovePermissions' },
            REQUEST_ERROR_UserFieldNoRequestVisible: { 'value': -30, 'tag': 'REQUEST_ERROR_UserFieldNoRequestVisible' },
            REQUEST_ERROR_NoApproveRefuseLevelOfAuthorityRequired: { 'value': -31, 'tag': 'REQUEST_ERROR_NoApproveRefuseLevelOfAuthorityRequired' },
            REQUEST_ERROR_UserFieldValueSaveError: { 'value': -32, 'tag': 'REQUEST_ERROR_UserFieldValueSaveError' },
            REQUEST_ERROR_InvalidPassport: { 'value': -33, 'tag': 'REQUEST_ERROR_InvalidPassport' },
            REQUEST_ERROR_ChangeShiftError: { 'value': -34, 'tag': 'REQUEST_ERROR_ChangeShiftError' },
            REQUEST_ERROR_VacationsOrPermissionsError: { 'value': -35, 'tag': 'REQUEST_ERROR_VacationsOrPermissionsError' },
            REQUEST_ERROR_ExistsLockedDaysInPeriod: { 'value': -36, 'tag': 'REQUEST_ERROR_ExistsLockedDaysInPeriod' },
            REQUEST_ERROR_ForbiddenPunchError: { 'value': -37, 'tag': 'REQUEST_ERROR_ForbiddenPunchError' },
            REQUEST_ERROR_JustifyPunchError: { 'value': -38, 'tag': 'REQUEST_ERROR_JustifyPunchError' },
            REQUEST_ERROR_RequestMoveNotExist: { 'value': -39, 'tag': 'REQUEST_ERROR_RequestMoveNotExist' },
            REQUEST_ERROR_RequestMoveTooMany: { 'value': -40, 'tag': 'REQUEST_ERROR_RequestMoveTooMany' },
            REQUEST_ERROR_PlannedAbsencesError: { 'value': -41, 'tag': 'REQUEST_ERROR_PlannedAbsencesError' },
            REQUEST_ERROR_PlannedCausesError: { 'value': -42, 'tag': 'REQUEST_ERROR_PlannedCausesError' },
            REQUEST_ERROR_ExternalWorkResumePartError: { 'value': -43, 'tag': 'REQUEST_ERROR_ExternalWorkResumePartError' },
            REQUEST_ERROR_UserFieldRequired: { 'value': -44, 'tag': 'REQUEST_ERROR_UserFieldRequired' },
            REQUEST_ERROR_PunchDateTimeRequired: { 'value': -45, 'tag': 'REQUEST_ERROR_PunchDateTimeRequired' },
            REQUEST_ERROR_CauseRequired: { 'value': -46, 'tag': 'REQUEST_ERROR_CauseRequired' },
            REQUEST_ERROR_DateRequired: { 'value': -47, 'tag': 'REQUEST_ERROR_DateRequired' },
            REQUEST_ERROR_HoursRequired: { 'value': -48, 'tag': 'REQUEST_ERROR_HoursRequired' },
            REQUEST_ERROR_ShiftRequired: { 'value': -49, 'tag': 'REQUEST_ERROR_ShiftRequired' },
            REQUEST_ERROR_RequestRepited: { 'value': -50, 'tag': 'REQUEST_ERROR_RequestRepited' },
            REQUEST_ERROR_PunchExist: { 'value': -51, 'tag': 'REQUEST_ERROR_PunchExist' },
            REQUEST_ERROR_StartShiftRequired: { 'value': -52, 'tag': 'REQUEST_ERROR_StartShiftRequired' },
            REQUEST_ERROR_PlannedCausesOverlapped: { 'value': -53, 'tag': 'REQUEST_ERROR_PlannedCausesOverlapped' },
            REQUEST_ERROR_PlannedAbsencesOverlapped: { 'value': -54, 'tag': 'REQUEST_ERROR_PlannedAbsencesOverlapped' },
            REQUEST_ERROR_TaskRequiered: { 'value': -55, 'tag': 'REQUEST_ERROR_TaskRequiered' },
            REQUEST_ERROR_TIME_BETWEEN_PUNCHES: { 'value': -56, 'tag': 'REQUEST_ERROR_TIME_BETWEEN_PUNCHES' },
            REQUEST_ERROR_TIME_BETWEEN_PUNCHES_OVER: { 'value': -57, 'tag': 'REQUEST_ERROR_TIME_BETWEEN_PUNCHES_OVER' },
            TASK_ALERT_SAVE_ERROR: { 'value': -58, 'tag': 'TASK_ALERT_SAVE_ERROR' },
            LOGIN_RESULT_LOW_STRENGHT_ERROR: { 'value': -59, 'tag': 'LOGIN_RESULT_LOW_STRENGHT_ERROR' },
            LOGIN_RESULT_MEDIUM_STRENGHT_ERROR: { 'value': -60, 'tag': 'LOGIN_RESULT_MEDIUM_STRENGHT_ERROR' },
            LOGIN_RESULT_HIGH_STRENGHT_ERROR: { 'value': -61, 'tag': 'LOGIN_RESULT_HIGH_STRENGHT_ERROR' },
            LOGIN_PASSWORD_EXPIRED: { 'value': -62, 'tag': 'LOGIN_PASSWORD_EXPIRED' },
            LOGIN_NEED_TEMPORANY_KEY: { 'value': -63, 'tag': 'LOGIN_NEED_TEMPORANY_KEY' },
            LOGIN_TEMPORANY_KEY_EXPIRED: { 'value': -64, 'tag': 'LOGIN_TEMPORANY_KEY_EXPIRED' },
            LOGIN_INVALID_VALIDATION_CODE: { 'value': -65, 'tag': 'LOGIN_INVALID_VALIDATION_CODE' },
            LOGIN_BLOCKED_ACCESS_APP: { 'value': -66, 'tag': 'LOGIN_BLOCKED_ACCESS_APP' },
            LOGIN_TEMPORANY_BLOQUED: { 'value': -67, 'tag': 'LOGIN_TEMPORANY_BLOQUED' },
            LOGIN_GENERAL_BLOCK_ACCESS: { 'value': -68, 'tag': 'LOGIN_GENERAL_BLOCK_ACCESS' },
            LOGIN_INVALID_CLIENT_LOCATION: { 'value': -69, 'tag': 'LOGIN_INVALID_CLIENT_LOCATION' },
            LOGIN_INVALID_VERSION_APP: { 'value': -70, 'tag': 'LOGIN_INVALID_VERSION_APP' },
            LOGIN_INVALID_APP: { 'value': -71, 'tag': 'LOGIN_INVALID_APP' },
            REQUEST_ERROR_CostCenterRequiered: { 'value': -72, 'tag': 'REQUEST_ERROR_CostCenterRequiered' },
            GENERAL_ERROR_NoPermissions: { 'value': -73, 'tag': 'GENERAL_ERROR_NoPermissions' },
            GENERAL_ERROR_InvalidSecurityToken: { 'value': -74, 'tag': 'GENERAL_ERROR_InvalidSecurityToken' },
            REQUEST_ERROR_PlannedHolidaysError: { 'value': -75, 'tag': 'REQUEST_ERROR_PlannedHolidaysError' },
            REQUEST_ERROR_PlannedHolidaysOverlapped: { 'value': -76, 'tag': 'REQUEST_ERROR_PlannedHolidaysOverlapped' },
            REQUEST_ERROR_AnotherHolidayExistInDate: { 'value': -77, 'tag': 'REQUEST_ERROR_AnotherHolidayExistInDate' },
            REQUEST_ERROR_AnotherAbsenceExistInDate: { 'value': -78, 'tag': 'REQUEST_ERROR_AnotherAbsenceExistInDate' },
            REQUEST_ERROR_InHolidayPlanification: { 'value': -79, 'tag': 'REQUEST_ERROR_InHolidayPlanification' },
            REQUEST_ERROR_VacationsOrPermissionsOverlapped: { 'value': -80, 'tag': 'REQUEST_ERROR_VacationsOrPermissionsOverlapped' },
            REQUEST_ERROR_CustomError: { 'value': -81, 'tag': 'REQUEST_ERROR_CustomError' },
            REQUEST_WARNING_NeedConfirmation: { 'value': -82, 'tag': 'REQUEST_WARNING_NeedConfirmation' },
            LOGIN_INVALID_APP_ADFS: { 'value': -83, 'tag': 'LOGIN_INVALID_APP_ADFS' },
            NO_DOCUMENT_TEMPLATE_SELECTED: { 'value': -84, 'tag': 'NO_DOCUMENT_TEMPLATE_SELECTED' },
            NO_DOCUMENT_ATTACHED: { 'value': -85, 'tag': 'NO_DOCUMENT_ATTACHED' },
            USER_OR_EMAIL_NOTFOUND: { 'value': -86, 'tag': 'USER_OR_EMAIL_NOTFOUND' },
            RECOVERKEY_NOTFOUND: { 'value': -87, 'tag': 'RECOVERKEY_NOTFOUND' },
            ERROR_CREATING_NOTIFICATION: { 'value': -88, 'tag': 'ERROR_CREATING_NOTIFICATION' },
            REQUEST_PENDING_VALIDATION: { 'value': -89, 'tag': 'REQUEST_PENDING_VALIDATION' },
            REQUEST_DENIED_DUE_REQUESTVALIDATION: { 'value': -90, 'tag': 'REQUEST_DENIED_DUE_REQUESTVALIDATION' },
            REQUEST_DENIED_DUE_SERVERDATA: { 'value': -91, 'tag': 'REQUEST_DENIED_DUE_SERVERDATA' },
            GENERAL_ERROR_MaxCurrentSessionsExceeded: { 'value': -92, 'tag': 'GENERAL_ERROR_MaxCurrentSessionsExceeded' },
            PUNCH_NFC_TAG_NOT_FOUND: { 'value': -94, 'tag': 'PUNCH_NFC_TAG_NOT_FOUND' },
            DAILYRECORD_INCORRECT_DATE_FUTURE: { 'value': -101, 'tag': 'DAILYRECORD_INCORRECT_DATE_FUTURE' },
            DAILYRECORD_INCORRECT_DATE_FROZEN: { 'value': -102, 'tag': 'DAILYRECORD_INCORRECT_DATE_FROZEN' },
            DAILYRECORD_INCORRECT_DATE_ALREADY_EXISTS: { 'value': -103, 'tag': 'DAILYRECORD_INCORRECT_DATE_ALREADY_EXISTS' },
            DAILYRECORD_WRONG_PUNCHES_SEQUENCE: { 'value': -104, 'tag': 'DAILYRECORD_WRONG_PUNCHES_SEQUENCE' },
            DAILYRECORD_ERROR_SAVING_REQUEST: { 'value': -106, 'tag': 'DAILYRECORD_ERROR_SAVING_REQUEST' },
            DAILYRECORD_ERROR_LOADING: { 'value': -107, 'tag': 'DAILYRECORD_ERROR_LOADING' },
            DAILYRECORD_ERROR_LOADING_CALENDAR: { 'value': -108, 'tag': 'DAILYRECORD_ERROR_LOADING_CALENDAR' },
            DAILYRECORD_ERROR_APPROVING_WHEN_MADE_BY_SUPERVISOR: { 'value': -109, 'tag': 'DAILYRECORD_ERROR_APPROVING_WHEN_MADE_BY_SUPERVISOR' },
            DAILYRECORD_ERROR_DOES_NOT_EXISTS: { 'value': -110, 'tag': 'DAILYRECORD_ERROR_DOES_NOT_EXISTS' },
            DAILYRECORD_ERROR_NO_PUNCHES: { 'value': -111, 'tag': 'DAILYRECORD_ERROR_NO_PUNCHES' },
            DAILYRECORD_PUNCHES_OVERLAPED: { 'value': -112, 'tag': 'DAILYRECORD_PUNCHES_OVERLAPED' },
            DAILYRECORD_HAS_REPEATED_PUNCHES: { 'value': -113, 'tag': 'DAILYRECORD_HAS_REPEATED_PUNCHES' },
            ERROR_UPLOADING_DOCUMENT_TO_SIGN: { 'value': -114, 'tag': 'ERROR_UPLOADING_DOCUMENT_TO_SIGN' },
            REQUEST_ERROR_OutOfContract: { 'value': -115, 'tag': 'REQUEST_ERROR_OutOfContract' },
        }
    };
})();