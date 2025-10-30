//Funciones para realizar las acciones provinentes del webservice
function WebServiceRobotics(onSucc, onError, wsURL) {
    this.onSuccessCall = onSucc;
    this.onErrorCall = (typeof onError == 'function' ? onError : WebServiceRobotics.connectionTimedOut);

    this.varType = "GET";
    this.varUrl = window.VTPortal.roApp.getServerUrl(wsURL).toLowerCase();

    this.varData = null; //{"Country": "usa"};
    this.varContentType = "application/json; charset=utf-8";
    this.varDataType = "json";
    this.varProcessData = true;
    this.finalizeSessionErrorCodes = [VTPortalUtils.constants.NO_SESSION.value, VTPortalUtils.constants.GENERAL_ERROR_InvalidSecurityToken.value,
        VTPortalUtils.constants.SERVER_NOT_RUNNING.value, VTPortalUtils.constants.LOGIN_INVALID_APP.value,
        VTPortalUtils.constants.LOGIN_INVALID_VERSION_APP.value, VTPortalUtils.constants.LOGIN_INVALID_APP_ADFS.value,
        VTPortalUtils.constants.GENERAL_ERROR_MaxCurrentSessionsExceeded.value];

}

WebServiceRobotics.prototype.blobCreationFromURL = function (inputURI) {
    var binaryVal;

    // mime extension extraction
    var inputMIME = 'type: "image/jpeg"';

    // Extract remaining part of URL and convert it to binary value
    if (inputURI.split(',')[0].indexOf('base64') >= 0)
        binaryVal = atob(inputURI.split(',')[1]);

    // Decoding of base64 encoded string
    else
        binaryVal = unescape(inputURI.split(',')[1]);

    // Computation of new string in which hexadecimal
    // escape sequences are replaced by the character
    // it represents

    // Store the bytes of the string to a typed array
    var blobArray = [];
    for (var index = 0; index < binaryVal.length; index++) {
        blobArray.push(binaryVal.charCodeAt(index));
    }

    return new Blob([blobArray], {
        type: inputMIME
    });
}

WebServiceRobotics.prototype.callUrlTimeGate = function (url, formData, time) {
    VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() + 1);
    if (VTPortal.roApp.timegateLoggedIn) VTPortal.roApp.lastTimegateAction = moment();

    var timeout = 30000;
    if (typeof (time) != 'undefined') timeout = time;

    var extendContext = function (url, extraContent, nosession) {
        return function (data, textStatus) {
            if (VTPortal.roApp.timegateLoggedIn) VTPortal.roApp.lastTimegateAction = moment();

            try {
                if (VTPortal.roApp.lastRequestFailed()) {
                    VTPortal.roApp.lastRequestFailed(false);
                    VTPortal.roApp.offlineCounter(0);
                }

                extraContent(data.d);

                if (VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() - 1);
            }
            catch (e) {
                //VTPortalUtils.utils.notifyMesage(JSON.stringify(data) + " --- " + e.message, 'error', 0); //MODO DEBUG
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roUnkownErrorOnRequest") + url.split('/').pop(), 'error', 0);
                VTPortal.roApp.wsRequestCounter(0);
            }
        };
    };

    var extendContextError = function (url, extraError) {
        return function (xhr, ajaxOptions, thrownError) {
            if (VTPortal.roApp.timegateLoggedIn) VTPortal.roApp.lastTimegateAction = moment();

            try {
                if (typeof extraError != 'undefined') extraError(url, xhr, ajaxOptions, thrownError);
                VTPortal.roApp.wsRequestCounter(0);
            }
            catch (e) {
                //VTPortalUtils.utils.notifyMesage(e.message, 'error', 0); //MODO DEBUG
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roUnkownErrorOnRequest") + url.split('/').pop(), 'error', 0);
                VTPortal.roApp.wsRequestCounter(0);
            }
        };
    }

    $.ajax({
        type: 'POST',
        url: url, // Location of the service
        timeout: timeout,
        data: formData, //Data sent to server
        cache: false,
        contentType: false,
        processData: false,
        success: extendContext(url, this.onSuccessCall, this.finalizeSessionErrorCodes),
        error: extendContextError(url, this.onErrorCall), // When Service call fails
        headers: {
            'roAuth': VTPortal.roApp.guidToken,
            'roToken': VTPortal.roApp.securityToken,
            'roAlias': VTPortal.roApp.getImpToken(),
            'roSrc': VTPortal.roApp.isRemoteEnvironment(),
            'roCompanyID': VTPortal.roApp.companyID.split("-")[0].toLowerCase(),
            'roApp': (VTPortal.roApp.isTimeGate() ? 'TimeGate' : 'VTPortal')
        },
        xhrFields: {
            withCredentials: true
        }
    });
}

WebServiceRobotics.prototype.callUrlPost = function (url, formData, time) {
    this.callUploadFileDesktop(url, formData, time);
}

WebServiceRobotics.prototype.callUploadFileDesktop = function (url, formData, time) {
    VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() + 1);
    if (VTPortal.roApp.timegateLoggedIn) VTPortal.roApp.lastTimegateAction = moment();

    let timeout = 30000;
    if (typeof (time) != 'undefined') timeout = time;

    let extendContext = function (url, extraContent, nosession) {
        return function (data, textStatus) {
            if (VTPortal.roApp.timegateLoggedIn) VTPortal.roApp.lastTimegateAction = moment();

            try {
                if (VTPortal.roApp.lastRequestFailed()) {
                    VTPortal.roApp.lastRequestFailed(false);
                    VTPortal.roApp.offlineCounter(0);
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roSessionRecovered"), 'success', 2000);
                }

                if (nosession.includes(data.d.Status)) {
                    if (data.d.Status == VTPortalUtils.constants.GENERAL_ERROR_MaxCurrentSessionsExceeded.value) {
                        WebServiceRobotics.maxConcurrentUsersReached(data.d)
                    } else {
                        WebServiceRobotics.connectionLoggedOut(data.d);
                    }
                } else {
                    extraContent(data.d);
                }

                if (VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() - 1);
            }
            catch (e) {
                //VTPortalUtils.utils.notifyMesage(JSON.stringify(data) + " --- " + e.message, 'error', 0); //MODO DEBUG
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roUnkownErrorOnRequest") + url.split('/').pop(), 'error', 0);
                VTPortal.roApp.wsRequestCounter(0);
            }
        };
    };

    let extendContextError = function (url, extraError) {
        return function (xhr, ajaxOptions, thrownError) {
            if (VTPortal.roApp.timegateLoggedIn) VTPortal.roApp.lastTimegateAction = moment();

            try {
                if (typeof extraError != 'undefined') extraError(url, xhr, ajaxOptions, thrownError);
                else WebServiceRobotics.connectionTimedOut(url, xhr, ajaxOptions, thrownError);

                VTPortal.roApp.wsRequestCounter(0);
            }
            catch (e) {
                //VTPortalUtils.utils.notifyMesage(e.message, 'error', 0); //MODO DEBUG
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roUnkownErrorOnRequest") + url.split('/').pop(), 'error', 0);
                VTPortal.roApp.wsRequestCounter(0);
            }
        };
    }

    $.ajax({
        type: 'POST',
        url: url, // Location of the service
        timeout: timeout,
        data: formData, //Data sent to server
        cache: false,
        contentType: false,
        processData: false,
        success: extendContext(url, this.onSuccessCall, this.finalizeSessionErrorCodes),
        error: extendContextError(url, this.onErrorCall), // When Service call fails
        headers: {
            'roAuth': VTPortal.roApp.guidToken,
            'roToken': VTPortal.roApp.securityToken,
            'roAlias': VTPortal.roApp.getImpToken(),
            'roSrc': VTPortal.roApp.isRemoteEnvironment(),
            'roCompanyID': VTPortal.roApp.companyID.split("-")[0].toLowerCase(),
            'roApp': (VTPortal.roApp.isTimeGate() ? 'TimeGate' : 'VTPortal')
        },
        xhrFields: {
            withCredentials: true
        }
    });
};

WebServiceRobotics.prototype.callUrl = function (url, params, time) {
    try {
        if (typeof navigator.splashscreen != 'undefined') {
            navigator.splashscreen.hide();
        }
    } catch (e) { }


    if (url.indexOf("/GetUserNotificationsStatus") == -1) VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() + 1);

    params.timestamp = new Date().getMilliseconds();

    var timeout = 60000;
    if (typeof (time) != 'undefined') timeout = time;

    var extendContext = function (url, extraContent, nosession) {
        return function (data, textStatus) {
            try {
                if (VTPortal.roApp.lastRequestFailed()) {
                    VTPortal.roApp.lastRequestFailed(false);
                    VTPortal.roApp.offlineCounter(0);
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roSessionRecovered"), 'success', 2000);
                }

                /* if (VTPortal.roApp.adfsLoginInprogress == false) {*/
                if (nosession.includes(data.d.Status)) { //REVISAR
                    if (data.d.Status == VTPortalUtils.constants.GENERAL_ERROR_MaxCurrentSessionsExceeded.value) {
                        WebServiceRobotics.maxConcurrentUsersReached(data.d)
                    } else {
                        WebServiceRobotics.connectionLoggedOut(data.d);
                    }
                } else {
                    if (url.indexOf('GetGenericList') != -1 && data.d.Status == window.VTPortalUtils.constants.OK.value) {
                        data.d.SelectFields = data.d.SelectFields.sortBy(function (n) {
                            return n.FieldName;
                        });
                    }

                    extraContent(data.d);
                }
                //}
                //else {
                //}

                if (url.indexOf("/GetUserNotificationsStatus") == -1 && VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() - 1);
            }
            catch (e) {
                if (url.indexOf('GetEmployeeAlerts') == -1) {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roUnkownErrorOnRequest") + url.split('/').pop(), 'error', 0);
                    //VTPortalUtils.utils.notifyMesage(JSON.stringify(data.d)  + " --- "+e.message, 'error', 0); //MODO DEBUG
                }

                VTPortal.roApp.wsRequestCounter(0);
            }
        };
    };

    var extendContextError = function (url, extraError) {
        return function (xhr, ajaxOptions, thrownError) {
            try {
                if (url.indexOf('GetEmployeeAlerts') == -1) {
                    if (typeof extraError != 'undefined') extraError(url, xhr, ajaxOptions, thrownError);
                    else WebServiceRobotics.connectionTimedOut(url, xhr, ajaxOptions, thrownError);
                }
                VTPortal.roApp.wsRequestCounter(0);
            }
            catch (e) {
                if (url.indexOf('GetEmployeeAlerts') == -1) {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roUnkownErrorOnRequest") + url.split('/').pop(), 'error', 0);
                    //VTPortalUtils.utils.notifyMesage(e.message, 'error', 0); //MODO DEBUG
                }
                    VTPortal.roApp.wsRequestCounter(0);
            }
        };
    }

    $.ajax({
        type: this.varType, //GET or POST or PUT or DELETE verb
        url: url, // Location of the service
        timeout: timeout,
        data: decodeURIComponent($.param(params)), //Data sent to server
        contentType: this.varContentType, // content type sent to server
        dataType: this.varDataType, //Expected data format from server
        processdata: this.varProcessData, //True or False
        success: extendContext(url, this.onSuccessCall, this.finalizeSessionErrorCodes),
        error: extendContextError(url, this.onErrorCall), // When Service call fails
        headers: {
            'roAuth': VTPortal.roApp.guidToken,
            'roToken': VTPortal.roApp.securityToken,
            'roAlias': VTPortal.roApp.getImpToken(),
            'roSrc': VTPortal.roApp.isRemoteEnvironment(),
            'roCompanyID': (VTPortal.roApp.companyID != "") ? VTPortal.roApp.companyID.split("-")[0].toLowerCase() : VTPortal.roApp.db.settings.companyID.split("-")[0].toLowerCase(),
            'roApp': (VTPortal.roApp.isTimeGate() ? 'TimeGate' : 'VTPortal')
        },
        xhrFields: {
            withCredentials: true
        }
    });
};

WebServiceRobotics.prototype.callFileTranferUrl = function (newUrl, formData, imageURI) {
    var wsSend = this;

    if (cordova.platformId === 'ios') {
        window.resolveLocalFileSystemURL(imageURI, function (fileEntry) {
            fileEntry.file(function (file) {
                var reader = new FileReader();

                reader.onloadend = function (evt) {
                    //VTPortalUtils.utils.notifyMesage('Fichero configuración abierto', 'info', 2000);
                    var filecontent = evt.target.result;
                    var blob = new Blob([new Uint8Array(filecontent)], { type: "image/png" });

                    formData.append("userfile", blob, "file.png");
                    wsSend.callUploadFileDesktop(newUrl, formData);
                };

                reader.readAsArrayBuffer(file);
            }, function (e) {
                console.log(e);
            })
        }, function (e) {
            console.log(e);
        });
    }
    else {
        window.resolveLocalFileSystemURI(imageURI, function (fileEntry) {
            fileEntry.file(function (file) {
                var reader = new FileReader();

                reader.onloadend = function (evt) {
                    //VTPortalUtils.utils.notifyMesage('Fichero configuración abierto', 'info', 2000);
                    var filecontent = evt.target.result;
                    var blob = new Blob([new Uint8Array(filecontent)], { type: "image/png" });

                    formData.append("userfile", blob, "file.png");
                    wsSend.callUploadFileDesktop(newUrl, formData);
                };

                reader.readAsArrayBuffer(file);
            }, function (e) {
                console.log(e);
            })
        }, function (e) {
            console.log(e);
        });
    }

    return;
}

WebServiceRobotics.maxConcurrentUsersReached = function (result) {
    VTPortal.roApp.wsRequestCounter(0);
    var onContinue = function () {
        try {
            VTPortal.roApp.impersonatedIDEmployee = -1;
            VTPortal.roApp.impersonateOnlyRequest = false;
            VTPortal.roApp.redirectAtHome(false, true);
        } catch (e) {
        }
    }
    VTPortalUtils.utils.processErrorMessage(result, onContinue);
};

WebServiceRobotics.connectionLoggedOut = function (result) {
    VTPortal.roApp.wsRequestCounter(0);
    var onContinue = function () {
        try {
            VTPortal.roApp.loggedIn = false;
            VTPortal.roApp.securityToken = '';


            if (VTPortal.roApp.adfsEnabled()) {
                VTPortal.roApp.userName('');
            }

            if (!VTPortal.roApp.isTimeGate() && VTPortal.roApp.adfsEnabled()) {
                VTPortal.roApp.companyID = '';
                VTPortal.roApp.db.settings.companyID = '';
            }
            
            VTPortal.roApp.db.settings.adfsCredential = '';
            VTPortal.roApp.db.settings.password = '';

            VTPortal.roApp.db.settings.rememberLogin = false;
            VTPortal.roApp.db.settings.rememberLoginApp = '';

            VTPortal.roApp.db.settings.save();
            VTPortal.roApp.impersonatedIDEmployee = -1;
            VTPortal.roApp.supervisedEmployees = [];
            VTPortal.roApp.LastLogin(null);
            VTPortal.roApp.automaticLogin = true;

            VTPortal.roApp.adfsLoginInprogress = false;
            localStorage.setItem('adfsLoginInprogress', 'false');

            if (VTPortal.roApp.refreshId != -1) {
                clearTimeout(VTPortal.roApp.refreshId);
                VTPortal.roApp.refreshId = -1;
            }
        } catch (e) {
        }


        if (!VTPortal.roApp.isTimeGate()) {
            VTPortal.app.navigate("login", { root: true });
        } else {
            VTPortal.app.navigate("TimeGate", { root: true });
        }


    }

    if ((result.Status == VTPortalUtils.constants.GENERAL_ERROR_InvalidSecurityToken && !VTPortal.roApp.db.settings.isUsingAdfs) || result.Status != VTPortalUtils.constants.GENERAL_ERROR_InvalidSecurityToken) {
        VTPortalUtils.utils.processErrorMessage(result, onContinue);
    }
};

WebServiceRobotics.connectionTimedOut = function (url, xhr, ajaxOptions, thrownError) {
    VTPortal.roApp.wsRequestCounter(0);

    var actionRequested = url.split('/').pop();
    var excludeActionsRepeat = ["GetUserScheduleStatus", "GetEmployeeStatus", "GetWsEmployeePhoto", "GetWsBackgroundImage", "GetMyPermissions"];

    VTPortal.roApp.offlineCounter(VTPortal.roApp.offlineCounter() + 1);
    VTPortal.roApp.lastRequestFailed(true);
};

WebServiceRobotics.prototype.login = function (user, password, language, appVersion, validationCode, buttonLogin) {
    let newUrl = this.varUrl + "/Authenticate";
    let formData = new FormData();
    formData.append("user", user);
    formData.append("password", VTPortalUtils.utils.encryptString(password));
    formData.append("language", language);
    formData.append("accessFromApp", VTPortal.roApp.isModeApp());
    formData.append("appVersion", appVersion);
    formData.append("validationCode", validationCode);
    formData.append("timeZone", VTPortal.roApp.clientTimeZone);
    formData.append("buttonLogin", buttonLogin);
    this.callUrlPost(newUrl, formData);
};

WebServiceRobotics.prototype.validateSession = function (code) {
    let newUrl = this.varUrl + "/ValidateSession";
    let formData = new FormData();

    formData.append("code", code);

    this.callUrlPost(newUrl, formData);
};

WebServiceRobotics.prototype.authenticateSession = function () {
    let newUrl = this.varUrl + "/AuthenticateSession";
    let formData = new FormData();

    formData.append("isModeApp", VTPortal.roApp.isModeApp());

    this.callUrlPost(newUrl, formData);
};

WebServiceRobotics.prototype.recoverPassword = function (userName, email) {
    let newUrl = this.varUrl + "/RecoverMyPassword";
    let formData = new FormData();
    formData.append("userName", userName);
    formData.append("email", email);
    this.callUrlPost(newUrl, formData);
};

WebServiceRobotics.prototype.updateServerLanguage = function (language) {
    let newUrl = this.varUrl + "/UpdateServerLanguage";
    let formData = new FormData();

    formData.append("language", language);

    this.callUrlPost(newUrl, formData);
};

WebServiceRobotics.prototype.resetPasswordToNew = function (userName, requestKey, newPassword) {
    let newUrl = this.varUrl + "/ResetPassword";
    let formData = new FormData();
    formData.append("userName", userName);
    formData.append("requestKey", requestKey);
    formData.append("newPassword", newPassword);
    this.callUrlPost(newUrl, formData);
};

WebServiceRobotics.prototype.logout = function (uuid) {
    let newUrl = this.varUrl + "/Logout";
    this.varData = { "uuid": uuid };
    this.callUrl(newUrl, this.varData);
};

WebServiceRobotics.prototype.changePassword = function (oldPassword, newPassword) {
    let newUrl = this.varUrl + "/ChangeMyPassword";
    let formData = new FormData();
    formData.append("oldPassword", oldPassword);
    formData.append("newPassword", newPassword);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.checkTelecommutingChange = function (selectedDay, type, impersonating) {
    let newUrl = this.varUrl + "/CheckTelecommutingChange";
    this.varData = { "selectedDay": selectedDay, 'type': type, 'impersonating': impersonating };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyTeamPlanEmployees = function (requestDate1, requestDate2, idEmployee, oldShift, newShift) {
    let newUrl = this.varUrl + "/GetMyTeamPlanEmployees";
    this.varData = { "startDay": requestDate1, 'endDay': requestDate2, 'idEmployee': idEmployee, 'oldShift': oldShift, 'newShift': newShift };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.changeTelecommuting = function (selectedDay, type, impersonating) {
    let newUrl = this.varUrl + "/ChangeTelecommuting";
    this.varData = { "selectedDay": selectedDay, 'type': type, 'impersonating': impersonating };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.changeTelecommutingByRequest = function (selectedDay, type, impersonating) {
    let newUrl = this.varUrl + "/ChangeTelecommutingByRequest";
    this.varData = { "selectedDay": selectedDay, 'type': type, 'impersonating': impersonating };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.acceptConsent = function (consentText) {
    let newUrl = this.varUrl + "/AcceptConsent";

    let formData = new FormData();
    formData.append("consentText", consentText);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.acceptMyLicense = function (bAcceptLicense) {
    let newUrl = this.varUrl + "/AcceptMyLicense";
    this.varData = { "bAcceptLicense": bAcceptLicense, };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getEmployeeStatus = function () {
    let newUrl = this.varUrl + "/GetEmployeeStatus";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getUserScheduleStatus = function () {
    let newUrl = this.varUrl + "/GetUserScheduleStatus";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getUserNotifications = function () {
    let newUrl = this.varUrl + "/GetUserNotifications";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getUserNotificationsStatus = function () {
    let newUrl = this.varUrl + "/GetUserNotificationsStatus";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}


WebServiceRobotics.prototype.getServerZones = function () {
    let newUrl = this.varUrl + "/GetServerZones";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getEmployeeCommuniques = function () {
    let newUrl = this.varUrl + "/GetEmployeeCommuniques";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getEmployeeSurveys = function () {
    let newUrl = this.varUrl + "/GetEmployeeSurveys";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getEmployeeTelecommutingInfo = function () {
    let newUrl = this.varUrl + "/GetEmployeeTelecommutingInfo";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getEmployeeAlerts = function () {
    let newUrl = this.varUrl + "/GetEmployeeAlerts";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.GetWsEmployeePhoto = function (employeeId) {
    let newUrl = this.varUrl + "/GetWsEmployeePhoto";
    this.varData = { "employeeId": employeeId };
    this.callUrl(newUrl, this.varData);

    return newUrl;
}

WebServiceRobotics.prototype.getView = function (viewName) {
    let newUrl = this.varUrl + "/GetView";
    this.varData = { 'viewName': viewName };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.GetWsBackgroundImage = function (employeeId, imageId) {
    let newUrl = this.varUrl + "/GetWsBackgroundImage";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
    return newUrl;
}

WebServiceRobotics.prototype.getGenericList = function (requestType) {
    let newUrl = this.varUrl + "/GetGenericList";
    this.varData = { "requestType": requestType };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getTasksByName = function (taskName) {
    let newUrl = this.varUrl + "/GetTasksByName";
    this.varData = { "taskName": taskName };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setStatus = function (causeId, direction, latitude, longitude, identifier, locationZone, fullAddress, reliable, nfcTag, tcType, reliableZone = true, selectedZone = -1) {
    let newUrl = this.varUrl + "/SetStatus";
    this.varData = { "causeId": causeId, 'direction': direction, "latitude": latitude, "longitude": longitude, "identifier": identifier, "locationZone": locationZone, "fullAddress": fullAddress, "reliable": reliable, "nfcTag": nfcTag, "tcType": tcType, "reliableZone": reliableZone, "selectedZone": selectedZone };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setStatusWithPhoto = function (causeId, direction, latitude, longitude, identifier, locationZone, fullAddress, reliable, imageURI, nfcTag, tcType, reliableZone = true, selectedZone = -1) {
    let newUrl = this.varUrl + "/SetStatusWithPhoto";

    let formData = new FormData();
    formData.append("causeId", causeId);
    formData.append("direction", direction);
    formData.append("latitude", latitude);
    formData.append("longitude", longitude);
    formData.append("identifier", identifier);
    formData.append("locationZone", locationZone);
    formData.append("fullAddress", fullAddress);
    formData.append("reliable", reliable);
    formData.append("nfcTag", ((typeof nfcTag == 'undefined') ? '' : nfcTag));
    formData.append("tcType", ((typeof tcType == 'undefined') ? '' : tcType));
    formData.append("reliableZone", reliableZone);
    formData.append("selectedZone", selectedZone);

    this.callFileTranferUrl(newUrl, formData, imageURI);
}

WebServiceRobotics.prototype.setCostCenterStatus = function (costCenterId, latitude, longitude, identifier, locationZone, fullAddress, reliable, tcType) {
    let newUrl = this.varUrl + "/SetCostCenterStatus";
    this.varData = {
        "costCenterId": costCenterId, "latitude": latitude, "longitude": longitude,
        "identifier": identifier, "locationZone": locationZone, "fullAddress": fullAddress, "reliable": reliable, "tcType": tcType
    };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setCostCenterPunchWithPhoto = function (costCenterId, latitude, longitude, identifier, locationZone, fullAddress, reliable, imageURI, tcType) {
    let newUrl = this.varUrl + "/SetCostCenterPunchWithPhoto";

    let formData = new FormData();
    formData.append("costCenterId", costCenterId);
    formData.append("latitude", latitude);
    formData.append("longitude", longitude);
    formData.append("identifier", identifier);
    formData.append("locationZone", locationZone);
    formData.append("fullAddress", fullAddress);
    formData.append("reliable", reliable);
    formData.append("tcType", ((typeof tcType == 'undefined') ? '' : tcType));
    this.callFileTranferUrl(newUrl, formData, imageURI);
}

WebServiceRobotics.prototype.SetTaskPunch = function (taskId, newTaskId, completeTask, latitude, longitude, identifier, locationZone, fullAddress, oldFieldValues, newFieldValues, reliable, tcType) {
    let newUrl = this.varUrl + "/SetTaskPunch";
    this.varData = {
        "taskId": taskId, "newTaskId": newTaskId, "latitude": latitude, "longitude": longitude,
        "identifier": identifier, "locationZone": locationZone, "fullAddress": fullAddress, "oldValue0": oldFieldValues[0], "oldValue1": oldFieldValues[1],
        "oldValue2": oldFieldValues[2], "oldValue3": oldFieldValues[3], "oldValue4": oldFieldValues[4], "oldValue5": oldFieldValues[5],
        "newValue0": newFieldValues[0], "newValue1": newFieldValues[1], "newValue2": newFieldValues[2], "newValue3": newFieldValues[3],
        "newValue4": newFieldValues[4], "newValue5": newFieldValues[5], 'completeTask': completeTask, "reliable": reliable, "tcType": tcType
    };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setTaskPunchWithPhoto = function (taskId, newTaskId, completeTask, latitude, longitude, identifier, locationZone, fullAddress, oldTaskValues, newTaskValues, reliable, imageURI, tcType) {
    let newUrl = this.varUrl + "/SetTaskPunchWithPhoto";

    let formData = new FormData();
    formData.append("taskId", taskId);
    formData.append("newTaskId", newTaskId);
    formData.append("completeTask", completeTask);
    formData.append("latitude", latitude);
    formData.append("longitude", longitude);
    formData.append("identifier", identifier);
    formData.append("locationZone", locationZone);
    formData.append("fullAddress", fullAddress);
    formData.append("reliable", reliable);
    formData.append("tcType", ((typeof tcType == 'undefined') ? '' : tcType));
    formData.append("oldValue0", oldTaskValues[0]);
    formData.append("oldValue1", oldTaskValues[1]);
    formData.append("oldValue2", oldTaskValues[2]);
    formData.append("oldValue3", oldTaskValues[3]);
    formData.append("oldValue4", oldTaskValues[4]);
    formData.append("oldValue5", oldTaskValues[5]);

    formData.append("newValue0", newTaskValues[0]);
    formData.append("newValue1", newTaskValues[1]);
    formData.append("newValue2", newTaskValues[2]);
    formData.append("newValue3", newTaskValues[3]);
    formData.append("newValue4", newTaskValues[4]);
    formData.append("newValue5", newTaskValues[5]);

    this.callFileTranferUrl(newUrl, formData, imageURI);
}

WebServiceRobotics.prototype.getMyCalendar = function (selectedDate) {
    let newUrl = this.varUrl + "/GetMyCalendar";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyDailyRecordCalendar = function (selectedDate) {
    let newUrl = this.varUrl + "/GetMyDailyRecordCalendar";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getLoadSeatingCapacity = function (selectedDate) {
    let newUrl = this.varUrl + "/GetLoadSeatingCapacity";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getEmployeeDayInfo = function (selectedDate, idEmployee) {
    let newUrl = this.varUrl + "/GetEmployeeDayInfo";
    this.varData = { "strDayDate": moment(selectedDate).format("DD/MM/YYYY"), "idEmployee": idEmployee };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getAccrualsSummary = function (selectedDate) {
    let newUrl = this.varUrl + "/GetAccrualsSummary";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyAccruals = function (selectedDate) {
    let newUrl = this.varUrl + "/GetMyAccruals";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyDailyAccruals = function (selectedDate) {
    let newUrl = this.varUrl + "/GetMyDailyAccruals";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyPunches = function (selectedDate) {
    let newUrl = this.varUrl + "/GetMyPunches";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getCommuniqueById = function (idCommunique) {
    let newUrl = this.varUrl + "/GetCommuniqueById";
    this.varData = { "idCommunique": idCommunique };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.sendSurveyResponse = function (idSurvey, idEmployee, Response) {
    let newUrl = this.varUrl + "/SendSurveyResponse";
    let formData = new FormData();
    formData.append("idSurvey", idSurvey);
    formData.append("idEmployee", idEmployee);
    formData.append("Response", JSON.stringify(Response));
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.getSurveyById = function (idSurvey) {
    let newUrl = this.varUrl + "/GetSurveyById";
    this.varData = { "idSurvey": idSurvey };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.answerCommunique = function (idCommunique, answer) {
    let newUrl = this.varUrl + "/AnswerCommunique";
    this.varData = { "idCommunique": idCommunique, "answer": answer };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.registerFirebaseToken = function (token, uuid) {
    let newUrl = this.varUrl + "/RegisterFirebaseToken";
    this.varData = { "token": token, "uuid": uuid };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getHolidaysInfo = function () {
    let newUrl = this.varUrl + "/GetHolidaysInfo";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getUserFields = function () {
    let newUrl = this.varUrl + "/GetUserFields";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getTelecommutingInfo = function (selectedDate, idEmployee) {
    let newUrl = this.varUrl + "/GetTelecommutingInfo";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY"), "idEmployee": idEmployee };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getTeleworkingDetail = function (sDate, eDate) {
    let newUrl = this.varUrl + "/GetTeleworkingDetail";
    this.varData = { "iDate": moment(sDate).format("YYYYMMDD"), "eDate": moment(eDate).format("YYYYMMDD") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyDailyTeleWorking = function (sDate) {
    let newUrl = this.varUrl + "/GetMyDailyTeleWorking";
    this.varData = { "selectedDate": moment(sDate).format("YYYYMMDD") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyRequests = function (showAll, sDate, eDate, filter, order, sRequestedDate, eRequestedDate) {
    let newUrl = this.varUrl + "/GetMyRequests";
    this.varData = { "showAll": showAll, "dateStart": sDate != null ? moment(sDate).format("YYYY-MM-DD HH:mm") : "", "dateEnd": eDate != null ? moment(eDate).format("YYYY-MM-DD HH:mm") : "", "filter": filter, "orderBy": order, "dateRequestedStart": sRequestedDate != null ? moment(sRequestedDate).format("YYYY-MM-DD HH:mm") : "", "dateRequestedEnd": eRequestedDate != null ? moment(eRequestedDate).format("YYYY-MM-DD HH:mm") : "" };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getSupervisedRequests = function (showAll, sDate, eDate, filter, order, sRequestedDate, eRequestedDate) {
    let newUrl = this.varUrl + "/GetSupervisedRequests";
    this.varData = { "showAll": showAll, "dateStart": sDate != null ? moment(sDate).format("YYYY-MM-DD HH:mm") : "", "dateEnd": eDate != null ? moment(eDate).format("YYYY-MM-DD HH:mm") : "", "filter": filter, "orderBy": order, "dateRequestedStart": sRequestedDate != null ? moment(sRequestedDate).format("YYYY-MM-DD HH:mm") : "", "dateRequestedEnd": eRequestedDate != null ? moment(eRequestedDate).format("YYYY-MM-DD HH:mm") : "" };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getOverlappingEmployees = function (idRequest) {
    let newUrl = this.varUrl + "/GetOverlappingEmployees";
    this.varData = { "idRequest": idRequest };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getZones = function () {
    let newUrl = this.varUrl + "/GetZones";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyLeaves = function (showAll, sDate, eDate) {
    let newUrl = this.varUrl + "/GetMyLeaves";
    this.varData = { "showAll": showAll, "dateStart": sDate != null ? moment(sDate).format("YYYY-MM-DD HH:mm") : "", "dateEnd": eDate != null ? moment(eDate).format("YYYY-MM-DD HH:mm") : "" };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyPermissions = function () {
    let newUrl = this.varUrl + "/GetMyPermissions";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setRequestReaded = function (requestId) {
    let newUrl = this.varUrl + "/SetRequestReaded";
    this.varData = { "requestId": requestId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setNotificationReaded = function (notificationtId) {
    let newUrl = this.varUrl + "/SetNotificationReaded";
    this.varData = { "notificationtId": notificationtId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRequest = function (requestId) {
    let newUrl = this.varUrl + "/GetRequest";
    this.varData = { "requestId": requestId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getDailyRecord = function (requestId) {
    let newUrl = this.varUrl + "/GetDailyRecord";
    this.varData = { "idDailyRecord": requestId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getDailyRecordRequestForSupervisor = function (requestId) {
    let newUrl = this.varUrl + "/GetDailyRecordRequestForSupervisor";
    this.varData = { "requestId": requestId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getDailyRecordPattern = function (selectedDate) {
    let newUrl = this.varUrl + "/GetEmployeeDailyRecordPunchesPattern";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getPunchesOnDate = function (idEmployee, selectedDate) {
    let newUrl = this.varUrl + "/GetPunchesOnDate";
    this.varData = { "idEmployee": idEmployee, "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRequestForSupervisor = function (requestId) {
    let newUrl = this.varUrl + "/GetRequestForSupervisor";
    this.varData = { "requestId": requestId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.saveUserFieldRequest = function (fieldName, fieldValue, comments, hasHistory, historyDate, acceptWarning) {
    let newUrl = this.varUrl + "/SaveRequestUserField";
    let formData = new FormData();
    formData.append("fieldName", fieldName);
    formData.append("fieldValue", fieldValue);
    formData.append("comments", comments);
    formData.append("hasHistory", hasHistory);
    formData.append("historyDate", historyDate.format("YYYY-MM-DD HH:mm"));
    formData.append("acceptWarning", acceptWarning);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.saveForbbidenPunch = function (punchDateTime, causeId, comments, direction, acceptWarning, tcType) {
    let newUrl = this.varUrl + "/SaveRequestForbiddenPunch";
    let formData = new FormData();
    formData.append("punchDate", punchDateTime.format("YYYY-MM-DD HH:mm"));
    formData.append("idCause", causeId == "" ? "-1" : causeId);
    formData.append("comments", comments);
    formData.append("direction", direction);
    formData.append("acceptWarning", acceptWarning);
    formData.append("tcType", ((typeof tcType == 'undefined') ? '' : tcType));
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.SaveRequestDailyRecord = function (dailyRecordJSON) {
    let newUrl = this.varUrl + "/SaveRequestDailyRecord";
    let formData = new FormData();
    formData.append("dailyRecord", dailyRecordJSON);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.savePunchCause = function (idCause, punchDate, comments, acceptWarning) {
    let newUrl = this.varUrl + "/SaveRequestJustifyPunch";
    let formData = new FormData();
    formData.append("punchDate", punchDate.format("YYYY-MM-DD HH:mm:ss"));
    formData.append("idCause", idCause);
    formData.append("comments", comments);
    formData.append("acceptWarning", acceptWarning);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.sendPunch = function (punch) {
    var instance = this;

    return new Promise(function (resolve, reject) {
        let newUrl = instance.varUrl + "/SavePunch";

        instance.promiseResolve = resolve;
        instance.promiseReject = reject;

        let formData = new FormData();
        formData.append("oPunch", JSON.stringify(punch));

        instance.callUrlPost(newUrl, formData);
    });
};

WebServiceRobotics.prototype.saveExternalWork = function (externalWorkDate, duration, comments, idCause, acceptWarning) {
    let newUrl = this.varUrl + "/SaveRequestExternalWork";
    let formData = new FormData();
    formData.append("externalWorkDate", externalWorkDate.format("YYYY-MM-DD HH:mm:ss"));
    formData.append("idCause", idCause);
    formData.append("duration", duration);
    formData.append("comments", comments);
    formData.append("acceptWarning", acceptWarning);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.saveChangeShift = function (from, to, idRequestedShift, idReplaceShift, strStartShiftHour, comments, acceptWarning) {
    let newUrl = this.varUrl + "/SaveRequestChangeShift";
    let formData = new FormData();
    formData.append("fromDate", from.format("YYYY-MM-DD HH:mm"));
    formData.append("toDate", to.format("YYYY-MM-DD HH:mm"));
    formData.append("idRequestedShift", idRequestedShift);
    formData.append("idReplaceShift", idReplaceShift);
    formData.append("strStartShiftHour", strStartShiftHour);
    formData.append("comments", comments);
    formData.append("acceptWarning", acceptWarning);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.saveShiftExchange = function (sDate, sCompensateDate, idEmployee, idShift, idSourceShift, comments, acceptWarning) {
    let newUrl = this.varUrl + "/SaveRequestShiftExchange";
    let formData = new FormData();
    formData.append("sDate", moment(sDate).format("YYYY-MM-DD HH:mm"));
    formData.append("sCompensateDate", sCompensateDate);
    formData.append("idEmployee", idEmployee);
    formData.append("idShift", idShift);
    formData.append("idSourceShift", idSourceShift);
    formData.append("comments", comments);
    formData.append("acceptWarning", acceptWarning);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.savePermissions = function (strDates, idHolidays, comments, acceptWarning) {
    let newUrl = this.varUrl + "/SaveRequestHolidays";
    let formData = new FormData();
    formData.append("strDates", strDates);
    formData.append("idHolidays", idHolidays);
    formData.append("comments", comments);
    formData.append("acceptWarning", acceptWarning);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.saveAbsences = function (from, to, idCause, comments, acceptWarning, documentData) {
    let ua = window.navigator.userAgent;
    let msie = ua.indexOf("MSIE ");
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        let newUrl = this.varUrl + "/SaveRequestPlannedAbsences";
        let formData = new FormData();

        formData.append("fromDate", from.format("YYYY-MM-DD HH:mm"));
        formData.append("toDate", to.format("YYYY-MM-DD HH:mm"));
        formData.append("idCause", idCause);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        if (documentData.get('userfile') != '') {
            let newUrl = this.varUrl + "/SaveRequestPlannedAbsences";

            documentData.append("fromDate", from.format("YYYY-MM-DD HH:mm"));
            documentData.append("toDate", to.format("YYYY-MM-DD HH:mm"));
            documentData.append("idCause", idCause);
            documentData.append("comments", comments);
            documentData.append("acceptWarning", acceptWarning);

            if (VTPortal.roApp.isModeApp()) {
                this.callFileTranferUrl(newUrl, documentData, documentData.get('userfile'));
            }
            else {
                this.callUploadFileDesktop(newUrl, documentData);
            }
        } else {
            let newUrl = this.varUrl + "/SaveRequestPlannedAbsences";
            let formData = new FormData();

            formData.append("fromDate", from.format("YYYY-MM-DD HH:mm"));
            formData.append("toDate", to.format("YYYY-MM-DD HH:mm"));
            formData.append("idCause", idCause);
            formData.append("comments", comments);
            formData.append("acceptWarning", acceptWarning);
            this.callUrlPost(newUrl, formData);
        }
    }
}

WebServiceRobotics.prototype.saveIncidence = function (sDate, eDate, from, to, duration, idCause, comments, acceptWarning, documentData) {
    let ua = window.navigator.userAgent;
    let msie = ua.indexOf("MSIE ");
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        let newUrl = this.varUrl + "/SaveRequestPlannedCauses";
        let formData = new FormData();
        formData.append("fromDate", sDate.format("YYYY-MM-DD HH:mm"));
        formData.append("toDate", eDate.format("YYYY-MM-DD HH:mm"));
        formData.append("fromHour", from.format("YYYY-MM-DD HH:mm"));
        formData.append("toHour", to.format("YYYY-MM-DD HH:mm"));
        formData.append("duration", duration);
        formData.append("idCause", idCause);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);

        this.callUrlPost(newUrl, formData);
    }
    else {
        if (documentData.get('userfile') != '') {
            let newUrl = this.varUrl + "/SaveRequestPlannedCauses";

            documentData.append("fromDate", sDate.format("YYYY-MM-DD HH:mm"));
            documentData.append("toDate", eDate.format("YYYY-MM-DD HH:mm"));
            documentData.append("fromHour", from.format("YYYY-MM-DD HH:mm"));
            documentData.append("toHour", to.format("YYYY-MM-DD HH:mm"));
            documentData.append("duration", duration);
            documentData.append("idCause", idCause);
            documentData.append("comments", comments);
            documentData.append("acceptWarning", acceptWarning);

            if (VTPortal.roApp.isModeApp()) {
                this.callFileTranferUrl(newUrl, documentData, documentData.get('userfile'));
            }
            else {
                this.callUploadFileDesktop(newUrl, documentData);
            }
        }
        else {
            let newUrl = this.varUrl + "/SaveRequestPlannedCauses";
            let formData = new FormData();
            formData.append("fromDate", sDate.format("YYYY-MM-DD HH:mm"));
            formData.append("toDate", eDate.format("YYYY-MM-DD HH:mm"));
            formData.append("fromHour", from.format("YYYY-MM-DD HH:mm"));
            formData.append("toHour", to.format("YYYY-MM-DD HH:mm"));
            formData.append("duration", duration);
            formData.append("idCause", idCause);
            formData.append("comments", comments);
            formData.append("acceptWarning", acceptWarning);

            this.callUrlPost(newUrl, formData);
        }
    }
}

WebServiceRobotics.prototype.saveOvertime = function (sDate, eDate, from, to, duration, idCause, comments, acceptWarning, documentData) {
    let ua = window.navigator.userAgent;
    let msie = ua.indexOf("MSIE ");
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        let newUrl = this.varUrl + "/SaveRequestPlannedOvertime";
        let formData = new FormData();
        formData.append("fromDate", sDate.format("YYYY-MM-DD HH:mm"));
        formData.append("toDate", eDate.format("YYYY-MM-DD HH:mm"));
        formData.append("fromHour", from.format("YYYY-MM-DD HH:mm"));
        formData.append("toHour", to.format("YYYY-MM-DD HH:mm"));
        formData.append("duration", duration);
        formData.append("idCause", idCause);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);

        this.callUrlPost(newUrl, formData);
    }
    else {
        if (documentData.get('userfile') != '') {
            let newUrl = this.varUrl + "/SaveRequestPlannedOvertime";

            documentData.append("fromDate", sDate.format("YYYY-MM-DD HH:mm"));
            documentData.append("toDate", eDate.format("YYYY-MM-DD HH:mm"));
            documentData.append("fromHour", from.format("YYYY-MM-DD HH:mm"));
            documentData.append("toHour", to.format("YYYY-MM-DD HH:mm"));
            documentData.append("duration", duration);
            documentData.append("idCause", idCause);
            documentData.append("comments", comments);
            documentData.append("acceptWarning", acceptWarning);

            if (VTPortal.roApp.isModeApp()) {
                this.callFileTranferUrl(newUrl, documentData, documentData.get('userfile'));
            }
            else {
                this.callUploadFileDesktop(newUrl, documentData);
            }
        }
        else {
            let newUrl = this.varUrl + "/SaveRequestPlannedOvertime";
            let formData = new FormData();
            formData.append("fromDate", sDate.format("YYYY-MM-DD HH:mm"));
            formData.append("toDate", eDate.format("YYYY-MM-DD HH:mm"));
            formData.append("fromHour", from.format("YYYY-MM-DD HH:mm"));
            formData.append("toHour", to.format("YYYY-MM-DD HH:mm"));
            formData.append("duration", duration);
            formData.append("idCause", idCause);
            formData.append("comments", comments);
            formData.append("acceptWarning", acceptWarning);

            this.callUrlPost(newUrl, formData);
        }
    }
}

WebServiceRobotics.prototype.savePlannedHoliday = function (sDate, allDay, from, to, idCause, comments, acceptWarning) {
    let newUrl = this.varUrl + "/SaveRequestPlannedHoliday";
    let formData = new FormData();
    formData.append("datesStr", sDate);
    formData.append("allDay", allDay);
    formData.append("fromHour", from.format("YYYY-MM-DD HH:mm"));
    formData.append("toHour", to.format("YYYY-MM-DD HH:mm"));
    formData.append("idCause", idCause);
    formData.append("comments", comments);
    formData.append("acceptWarning", acceptWarning);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.SaveRequest_ForbiddenTaskPunch = function (sDate, taskId, eDate, cTaskId, completeTask, comments, value1, value2, value3, value4, value5, value6, acceptWarning, tcType) {
    let newUrl = this.varUrl + "/SaveRequestForbiddenTaskPunch";
    let formData = new FormData();
    formData.append("punchDate", sDate);
    formData.append("continueOnPunchDate", eDate);
    formData.append("idTask", taskId);
    formData.append("idContinueOnTask", cTaskId);
    formData.append("completeTask", completeTask);
    formData.append("comments", comments);
    formData.append("value1", value1);
    formData.append("value2", value2);
    formData.append("value3", value3);
    formData.append("value4", value4);
    formData.append("value5", value5);
    formData.append("value6", value6);
    formData.append("acceptWarning", acceptWarning);
    formData.append("tcType", ((typeof tcType == 'undefined') ? '' : tcType));
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.saveCancelHolidays = function (strDates, idCause, idShift, comments, acceptWarning) {
    let newUrl = this.varUrl + "/SaveRequestCancelHolidays";
    let formData = new FormData();
    formData.append("strDates", strDates);
    formData.append("idShift", idShift);
    formData.append("idCause", idCause);
    formData.append("comments", comments);
    formData.append("acceptWarning", acceptWarning);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.saveCostCenterPunch = function (punchDate, idCostCenter, comments, acceptWarning, tcType) {
    let newUrl = this.varUrl + "/SaveRequestForbiddenCostCenterPunch";
    let formData = new FormData();
    formData.append("punchDate", punchDate.format("YYYY-MM-DD HH:mm"));
    formData.append("idCostCenter", idCostCenter == "" ? -1 : idCostCenter);
    formData.append("comments", comments);
    formData.append("acceptWarning", acceptWarning);
    formData.append("tcType", ((typeof tcType == 'undefined') ? '' : tcType));
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.saveExternalWorkWeekResume = function (remarks, rInfo, acceptWarning) {
    let newUrl = this.varUrl + "/SaveRequestExternalWorkWeekResume";
    let formData = new FormData();
    formData.append("resumeInfo", JSON.stringify(rInfo));
    formData.append("comments", remarks);
    formData.append("acceptWarning", acceptWarning);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.saveProfileImageDesktop = function (documentData) {
    let newUrl = this.varUrl + "/SaveProfileImage";

    this.callUploadFileDesktop(newUrl, documentData);
}

WebServiceRobotics.prototype.saveProfileImageMobile = function (documentData) {
    let newUrl = this.varUrl + "/SaveProfileImage";
    let formData = new FormData();
    this.callFileTranferUrl(newUrl, formData, documentData.get('userfile'));
}

WebServiceRobotics.prototype.saveLeaveDesktop = function (from, to, idCause, comments, documentData) {
    let newUrl = this.varUrl + "/SaveLeave";

    documentData.append("from", from.format("YYYY-MM-DD"));
    documentData.append("to", to.format("YYYY-MM-DD"));
    documentData.append("idCause", idCause);
    documentData.append("remarks", comments);

    this.callUploadFileDesktop(newUrl, documentData);
}

WebServiceRobotics.prototype.saveLeaveMobile = function (from, to, idCause, comments, documentData) {
    let newUrl = this.varUrl + "/SaveLeave";

    let ua = window.navigator.userAgent;
    let msie = ua.indexOf("MSIE ");
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        documentData.append("from", from.format("YYYY-MM-DD"));
        documentData.append("to", to.format("YYYY-MM-DD"));
        documentData.append("idCause", idCause);
        documentData.append("remarks", comments);

        this.callUploadFileDesktop(newUrl, documentData);
    }
    else {
        if (documentData.get('userfile') != '') {

            let formData = new FormData();
            formData.append("from", from.format("YYYY-MM-DD"));
            formData.append("to", to.format("YYYY-MM-DD"));
            formData.append("idCause", idCause);
            formData.append("remarks", comments);
            formData.append("idTemplateDocument", documentData.get('idTemplateDocument'));

            this.callFileTranferUrl(newUrl, formData, documentData.get('userfile'));
        } else {
            documentData.append("from", from.format("YYYY-MM-DD"));
            documentData.append("to", to.format("YYYY-MM-DD"));
            documentData.append("idCause", idCause);
            documentData.append("remarks", comments);

            this.callUploadFileDesktop(newUrl, documentData);
        }
    }
}

WebServiceRobotics.prototype.setLeaveEndDate = function (leaveId, endDate, documentTemplate) {
    let newUrl = this.varUrl + "/SetLeaveEndDate";
    if (endDate != null)
        this.varData = { "leaveId": leaveId, "documentTemplate": documentTemplate, "endDate": endDate };
    else
    this.varData = { "leaveId": leaveId, "documentTemplate": documentTemplate };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.checkIsWorkingdayShift = function (shiftId) {
    let newUrl = this.varUrl + "/CheckIsWorkingdayShift";
    this.varData = { "shiftId": shiftId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.checkIsFloatingShift = function (shiftId) {
    let newUrl = this.varUrl + "/CheckIsFloatingShift";
    this.varData = { "shiftId": shiftId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getUserFieldAvailableValues = function (fieldName) {
    let newUrl = this.varUrl + "/GetUserFieldAvailableValues";
    this.varData = { "fieldName": fieldName };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.deleteRequest = function (requestId) {
    let newUrl = this.varUrl + "/DeleteRequest";
    this.varData = { "requestId": requestId };
    this.callUrl(newUrl, this.varData);
}
WebServiceRobotics.prototype.deleteAbsence = function (absenceId) {
    let newUrl = this.varUrl + "/DeleteAbsence";
    this.varData = { "absenceId": absenceId };
    this.callUrl(newUrl, this.varData);
}
WebServiceRobotics.prototype.deleteProgrammedAbsence = function (forecastId, forecastType) {
    let newUrl = this.varUrl + "/DeleteProgrammedAbsence";
    this.varData = { "forecastId": forecastId, "forecastType": forecastType };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.uploadDocumentDesktop = function (formData) {
    let newUrl = this.varUrl + "/UploadDocument";
    this.callUploadFileDesktop(newUrl, formData);
}

WebServiceRobotics.prototype.uploadDocumentMobile = function (documentData, forecastType, remarks, docRelatedInfo) {
    let newUrl = this.varUrl + "/UploadDocument";

    let formData = new FormData();
    formData.append("forecastType", forecastType);
    formData.append("remarks", remarks);
    formData.append("docRelatedInfo", docRelatedInfo);
    formData.append("idRelatedObject", documentData.get('idRelatedObject'));
    formData.append("idTemplateDocument", documentData.get('idTemplateDocument'));

    this.callFileTranferUrl(newUrl, formData, documentData.get('userfile'));
}

WebServiceRobotics.prototype.getMyDocuments = function (sDate, eDate, filter, order) {
    let newUrl = this.varUrl + "/GetMyDocuments";
    this.varData = { "dateStart": sDate != null ? moment(sDate).format("YYYY-MM-DD HH:mm") : "", "dateEnd": eDate != null ? moment(eDate).format("YYYY-MM-DD HH:mm") : "", "filter": filter, "orderBy": order };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getDocumentBytes = function (documentId) {
    let newUrl = this.varUrl + "/GetDocumentBytes";
    this.varData = { "documentId": documentId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.uploadDocumenttoSign = function (documentId) {
    let newUrl = this.varUrl + "/UploadDocumenttoSign";
    this.varData = { "documentId": documentId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getAvailablePermitsCalendar = function (sDate, idCause) {
    let newUrl = this.varUrl + "/GetAvailablePermitsCalendar";
    this.varData = { "selectedDate": moment(sDate).format("DD/MM/YYYY"), "ProgrammedHoliday_IDCause": idCause };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getTaskUserFieldsAction = function (action, taskId) {
    let newUrl = this.varUrl + "/GetTaskUserFieldsAction";
    this.varData = { "action": action, "taskId": taskId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.saveNewTaskAlert = function (taskId, taskTextAlert) {
    let newUrl = this.varUrl + "/SaveNewTaskAlert";
    this.varData = { "taskId": taskId, "taskTextAlert": encodeURIComponent(taskTextAlert) };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getLoggedInUserInfo = function () {
    let newUrl = this.varUrl + "/GetLoggedInUserInfo";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRequieredLeaveDocuments = function (idCause, isStarting) {
    let newUrl = this.varUrl + "/GetRequieredLeaveDocuments";
    this.varData = { 'idCause': idCause, 'isStarting': isStarting };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getAvailableDocumentTemplateType = function (idEmployee) {
    let newUrl = this.varUrl + "/GetAvailableDocumentTemplateType";
    this.varData = { 'idEmployee': idEmployee };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRequieredCauseDocuments = function (idCause) {
    let newUrl = this.varUrl + "/GetRequieredCauseDocuments";
    this.varData = { 'idCause': idCause };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getUserFile = function (token, userFieldName) {
    let newUrl = this.varUrl + "/GetUserFile?token=" + token + "&userFieldName=" + encodeURIComponent(userFieldName);
    return newUrl;
}

WebServiceRobotics.prototype.getCurrentTaskInfo = function (token, taskId) {
    let newUrl = this.varUrl + "/GetCurrentTaskInfo";
    this.varData = { "token": token, "taskId": taskId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getTaskInfo = function (token, taskId) {
    let newUrl = this.varUrl + "/GetTaskInfo";
    this.varData = { "token": token, "taskId": taskId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyEmployees = function () {
    let newUrl = this.varUrl + "/GetMyEmployees";
    this.varData = {};
    this.callUrl(newUrl, this.varData, 60000);
}
WebServiceRobotics.prototype.getCapacityDetail = function (selectedDate) {
    let newUrl = this.varUrl + "/GetCapacityDetail";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyOnBoardings = function () {
    let newUrl = this.varUrl + "/GetMyOnBoardings";
    this.varData = {};
    this.callUrl(newUrl, this.varData, 60000);
}

WebServiceRobotics.prototype.getOnBoardingtasksById = function (idOnboarding) {
    let newUrl = this.varUrl + "/GetOnBoardingTasksById";
    this.varData = { "idOnboarding": idOnboarding };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.updateTaskStatus = function (status, idTask, idList) {
    let newUrl = this.varUrl + "/UpdateTaskStatus";
    this.varData = { "status": status, "idTask": idTask, "idList": idList };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyEmployeesWithStatus = function () {
    let newUrl = this.varUrl + "/GetMyEmployeesWithStatus";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getSupervisorAlertsDetail = function (idAlertType) {
    let newUrl = this.varUrl + "/GetSupervisorAlertsDetail";
    this.varData = { "idAlertType": idAlertType };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.approveRefuseRequest = function (idRequest, bApprove, bForceApprove) {
    let newUrl = this.varUrl + "/ApproveRefuseRequest";
    this.varData = { "idRequest": idRequest, "bApprove": bApprove, "bForceApprove": bForceApprove };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.approveRefuseRequestNew = function (idRequest, bApprove, bForceApprove) {
    let newUrl = this.varUrl + "/ApproveRefuseRequestNew";
    this.varData = { "idRequest": idRequest, "bApprove": bApprove, "bForceApprove": bForceApprove };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.approveRefuseMultipleDR = function (idRequests, action) {
    let newUrl = this.varUrl + "/ApproveRefuseMultipleDR";
    this.varData = { "idRequests": idRequests, "action": action };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.approveRefuseRequestByEmployee = function (idRequest, bApprove, bForceApprove) {
    let newUrl = this.varUrl + "/ApproveRefuseRequestByEmployee";
    this.varData = { "idRequest": idRequest, "bApprove": bApprove, "bForceApprove": bForceApprove };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getAvailableEmployeesForDate = function (sDate, eDate, iDestinationShift, iSourceShift) {
    let newUrl = this.varUrl + "/GetAvailableEmployeesForDate";
    this.varData = { "sDate": moment(sDate).format("DD/MM/YYYY"), "eDate": moment(eDate).format("DD/MM/YYYY"), "iSourceShift": iSourceShift, "iDestinationShift": iDestinationShift };
    this.callUrl(newUrl, this.varData, 60000);
}

WebServiceRobotics.prototype.getDaysToCompensate = function (sDate, eDate, iExchangeEmployee) {
    let newUrl = this.varUrl + "/GetDaysToCompensate";
    this.varData = { "sDate": moment(sDate).format("DD/MM/YYYY"), "eDate": moment(eDate).format("DD/MM/YYYY"), "iExchangeEmployee": iExchangeEmployee };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRankingForDay = function (sDate, idRequestType, idReason) {
    let newUrl = this.varUrl + "/GetRankingForDay";
    this.varData = { "sDate": moment(sDate).format("DD/MM/YYYY"), "idRequestType": idRequestType, "idReason": idReason };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.isAdfsActive = function () {
    let newUrl = this.varUrl + "/isAdfsActive";
    this.varData = {};
    this.callUrl(newUrl, this.varData, 5000);
}

WebServiceRobotics.prototype.getSsoConfiguration = function () {
    let newUrl = this.varUrl + "/GetSsoConfiguration";
    this.varData = {};
    this.callUrl(newUrl, this.varData, 5000);
}

WebServiceRobotics.prototype.updateServerLanguage = function (lang) {
    let newUrl = this.varUrl + "/updateServerLanguage";
    this.varData = { "lang": lang };
    this.callUrl(newUrl, this.varData, 2000);
}

WebServiceRobotics.prototype.updateServerTimeZone = function (timeZone) {
    let newUrl = this.varUrl + "/updateServerTimeZone";
    this.varData = { "timeZone": timeZone };
    this.callUrl(newUrl, this.varData, 2000);
}

WebServiceRobotics.prototype.getMyChannels = function () {
    let newUrl = this.varUrl + "/GetMyChannels";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getComplaintChannelUrl = function () {
    let newUrl = this.varUrl + "/GetDEXUrl";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyConversationsByIDChannel = function (idChannel) {
    let newUrl = this.varUrl + "/GetMyConversationsByChannel";
    this.varData = { idChannel };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.addNewConversation = function (idChannel, title, message, isAnonymous) {
    let newUrl = this.varUrl + "/AddNewConversation";
    let formData = new FormData();
    formData.append("idChannel", idChannel);
    formData.append("title", title);
    formData.append("message", message);
    formData.append("isAnonymous", isAnonymous);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.getMyMessagesByIDConversation = function (idConversation) {
    let newUrl = this.varUrl + "/GetMyMessagesByConversation";
    this.varData = { idConversation };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.addNewMessage = function (idConversation, message) {
    let newUrl = this.varUrl + "/AddNewMessage";
    let formData = new FormData();
    formData.append("idConversation", idConversation);
    formData.append("message", message);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.getConceptsSummaryByShift = function (idShift) {
    let newUrl = this.varUrl + "/GetConceptsSummaryByShift";
    this.varData = { idShift };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getConceptsDetailByShift = function (idShift) {
    let newUrl = this.varUrl + "/GetConceptsDetailByShift";
    this.varData = { idShift };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.enableTimegate = function (serialNumber, name, apkVersion) {
    let newUrl = this.varUrl + "/EnableTimegate";
    let formData = new FormData();
    formData.append("serialNumber", serialNumber);
    formData.append("name", name);
    formData.append("apkVersion", apkVersion);    
    this.callUrlPost(newUrl, formData);  
}

WebServiceRobotics.prototype.disableTimegate = function (serialNumber) {
    let newUrl = this.varUrl + "/DisableTimegate";
    let formData = new FormData();
    formData.append("serialNumber", serialNumber);    
    this.callUrlPost(newUrl, formData);
}
WebServiceRobotics.prototype.getTimegateConfiguration = function (serialNumber) {
    let newUrl = this.varUrl + "/GetTimegateConfiguration";
    this.varData = { serialNumber };
    this.callUrl(newUrl, this.varData);
}
WebServiceRobotics.prototype.initializeTimeGate = function () {    
    let newUrl = this.varUrl.replace('portalsvcx', 'timegate') + "/Initialize";     
    let formData = new FormData();    
    formData.append("apkVersion", VTPortal.roApp.infoVersion);   
    this.callUrlTimeGate(newUrl, formData);
}

WebServiceRobotics.prototype.identify = function (id,pin,nfc) {
    let newUrl = this.varUrl.replace('portalsvcx', 'timegate') + "/Identify";

    let formData = new FormData();
    formData.append("id", id);
    formData.append("pin", pin);
    formData.append("nfc", nfc);

    formData.append("accessFromApp", (VTPortal.roApp.isModeApp() ? '1':'0'));
    formData.append("appVersion", VTPortal.roApp.infoVersion);
    formData.append("timeZone", VTPortal.roApp.clientTimeZone);

    this.callUrlTimeGate(newUrl, formData);
}

WebServiceRobotics.prototype.getTimeGateBackground = function () {
    let newUrl = this.varUrl.replace('portalsvcx', 'timegate') + "/GetBackground";
    let formData = new FormData();
    formData.append("apkVersion", VTPortal.roApp.infoVersion);
    this.callUrlTimeGate(newUrl, formData);
}

WebServiceRobotics.prototype.GetAllPortalWebLinks = function () {
    let newUrl = this.varUrl + "/GetAllPortalWebLinks";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}