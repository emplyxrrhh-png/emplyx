//Funciones para realizar las acciones provinentes del webservice
function WebServiceRobotics(onSucc, onError, wsURL, isSSL) {
    this.onSuccessCall = onSucc;
    this.onErrorCall = (typeof onError == 'function' ? onError : WebServiceRobotics.connectionTimedOut);

    this.varType = "GET";
    this.varUrl = window.VTPortal.roApp.getServerUrl(wsURL, isSSL);

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

WebServiceRobotics.prototype.callUrlPost = function (url, formData, time) {
    this.callUploadFileDesktop(url, formData, time);
}

WebServiceRobotics.prototype.callUploadFileDesktop = function (url, formData, time) {
    VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() + 1);

    var timeout = 30000;
    if (typeof (time) != 'undefined') timeout = time;

    var extendContext = function (url, extraContent, nosession) {
        return function (data, textStatus) {
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
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roUnkownErrorOnRequest") + url.split('/').pop(), 'error', 0);
                VTPortal.roApp.wsRequestCounter(0);
            }
        };
    };

    var extendContextError = function (url, extraError) {
        return function (xhr, ajaxOptions, thrownError) {
            try {
                if (typeof extraError != 'undefined') extraError(url, xhr, ajaxOptions, thrownError);
                else WebServiceRobotics.connectionTimedOut(url, xhr, ajaxOptions, thrownError);

                VTPortal.roApp.wsRequestCounter(0);
            }
            catch (e) {
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
            'roSrc': VTPortal.roApp.isModeApp(),
            'roCompanyID': VTPortal.roApp.companyID,
            'roApp': 'VTPortal'
        }
    });
};

WebServiceRobotics.prototype.callUrl = function (url, params, time) {
    try {
        if (typeof navigator.splashscreen != 'undefined') {
            navigator.splashscreen.hide();
        }
    } catch (e) { }

    VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() + 1);

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
                if (nosession.includes(data.d.Status)) {
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

                if (VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(VTPortal.roApp.wsRequestCounter() - 1);
            }
            catch (e) {
                if (url.indexOf('GetEmployeeAlerts') == -1) VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roUnkownErrorOnRequest") + url.split('/').pop(), 'error', 0);

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
                if (url.indexOf('GetEmployeeAlerts') == -1) VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roUnkownErrorOnRequest") + url.split('/').pop(), 'error', 0);
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
            'roSrc': VTPortal.roApp.isModeApp(),
            'roCompanyID': (VTPortal.roApp.companyID != "") ? VTPortal.roApp.companyID : VTPortal.roApp.db.settings.companyID,
            'roApp': 'VTPortal'
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
            VTPortal.roApp.securityToken = '';
            if (VTPortal.roApp.adfsEnabled()) {
                VTPortal.roApp.userName('');
            }

            VTPortal.roApp.loggedIn = false;
            VTPortal.roApp.companyID = '';
            VTPortal.roApp.db.settings.companyID = '';
            VTPortal.roApp.db.settings.adfsCredential = '';
            VTPortal.roApp.db.settings.password = '';
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
        VTPortal.app.navigate("login", { root: true });
    }

    if ((result.Status == VTPortalUtils.constants.GENERAL_ERROR_InvalidSecurityToken && !VTPortal.roApp.db.settings.isUsingAdfs) || result.Status != VTPortalUtils.constants.GENERAL_ERROR_InvalidSecurityToken) {
        VTPortalUtils.utils.processErrorMessage(result, onContinue);
    }
};

WebServiceRobotics.connectionTimedOut = function (url, xhr, ajaxOptions, thrownError) {
    VTPortal.roApp.wsRequestCounter(0);

    var actionRequested = url.split('/').pop();
    var excludeActionsRepeat = ["GetEmployeeStatus", "GetWsEmployeePhoto", "GetWsBackgroundImage", "GetMyPermissions"];

    //if (VTPortal.roApp.offlineCounter() > 0) {
    //    if (!excludeActionsRepeat.includes(actionRequested)) VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNoConnectionAvailable"), 'warning', 0);
    //}else{
    //    //VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roServerTimeout"), 'error', 0);
    //}

    VTPortal.roApp.offlineCounter(VTPortal.roApp.offlineCounter() + 1);
    VTPortal.roApp.lastRequestFailed(true);
};

WebServiceRobotics.prototype.login = function (user, password, language, appVersion, validationCode, buttonLogin) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/Authenticate";
        var formData = new FormData();
        formData.append("user", user);
        formData.append("password", VTPortalUtils.utils.encryptString(password));
        formData.append("language", language);
        formData.append("accessFromApp", VTPortal.roApp.isModeApp());
        formData.append("appVersion", appVersion);
        formData.append("validationCode", validationCode);
        formData.append("timeZone", VTPortal.roApp.clientTimeZone);
        formData.append("buttonLogin", buttonLogin);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/Login";
        this.varData = { "usr": encodeURIComponent(user), "pwd": encodeURIComponent(password), "language": language, "accessFromApp": VTPortal.roApp.isModeApp(), "appVersion": appVersion, "validationCode": validationCode, "timeZone": VTPortal.roApp.clientTimeZone };
        this.callUrl(newUrl, this.varData);
    }
};

WebServiceRobotics.prototype.recoverPassword = function (userName, email) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/RecoverMyPassword";
        var formData = new FormData();
        formData.append("userName", userName);
        formData.append("email", email);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/RecoverPassword";
        this.varData = { "userName": encodeURIComponent(userName), "email": encodeURIComponent(email) };
        this.callUrl(newUrl, this.varData);
    }
};

WebServiceRobotics.prototype.resetPasswordToNew = function (userName, requestKey, newPassword) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/ResetPassword";
        var formData = new FormData();
        formData.append("userName", userName);
        formData.append("requestKey", requestKey);
        formData.append("newPassword", newPassword);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/ResetPasswordToNew";
        this.varData = { "userName": encodeURIComponent(userName), "requestKey": encodeURIComponent(requestKey), "newPassword": encodeURIComponent(newPassword) };
        this.callUrl(newUrl, this.varData);
    }
};

WebServiceRobotics.prototype.logout = function (uuid) {
    var newUrl = this.varUrl + "/Logout";
    this.varData = { "uuid": uuid };
    this.callUrl(newUrl, this.varData);
};

WebServiceRobotics.prototype.changePassword = function (oldPassword, newPassword) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/ChangeMyPassword";
        var formData = new FormData();
        formData.append("oldPassword", oldPassword);
        formData.append("newPassword", newPassword);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/ChangePassword";
        this.varData = { "oldPassword": encodeURIComponent(oldPassword), "newPassword": encodeURIComponent(newPassword) };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.changeTelecommuting = function (selectedDay, type, impersonating) {
    var newUrl = this.varUrl + "/ChangeTelecommuting";
    this.varData = { "selectedDay": selectedDay, 'type': type, 'impersonating': impersonating };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.changeTelecommutingByRequest = function (selectedDay, type, impersonating) {
    var newUrl = this.varUrl + "/ChangeTelecommutingByRequest";
    this.varData = { "selectedDay": selectedDay, 'type': type, 'impersonating': impersonating };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.acceptConsent = function (consentText) {
    var newUrl = this.varUrl + "/AcceptConsent";

    var formData = new FormData();
    formData.append("consentText", consentText);
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.acceptMyLicense = function (bAcceptLicense) {
    var newUrl = this.varUrl + "/AcceptMyLicense";
    this.varData = { "bAcceptLicense": bAcceptLicense, };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getEmployeeStatus = function () {
    var newUrl = this.varUrl + "/GetEmployeeStatus";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getEmployeeAlerts = function () {
    var newUrl = this.varUrl + "/GetEmployeeAlerts";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.GetWsEmployeePhoto = function (employeeId) {
    var newUrl = this.varUrl + "/GetWsEmployeePhoto";
    this.varData = { "employeeId": employeeId };
    this.callUrl(newUrl, this.varData);

    return newUrl;
}

WebServiceRobotics.prototype.getView = function (viewName) {
    var newUrl = this.varUrl + "/GetView";
    this.varData = { 'viewName': viewName };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.GetWsBackgroundImage = function (employeeId, imageId) {
    var newUrl = this.varUrl + "/GetWsBackgroundImage";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
    return newUrl;
}

WebServiceRobotics.prototype.getGenericList = function (requestType) {
    var newUrl = this.varUrl + "/GetGenericList";
    this.varData = { "requestType": requestType };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setStatus = function (causeId, direction, latitude, longitude, identifier, locationZone, fullAddress, reliable, nfcTag, tcType, reliableZone = true, selectedZone = -1) {
    var newUrl = this.varUrl + "/SetStatus";
    this.varData = { "causeId": causeId, 'direction': direction, "latitude": latitude, "longitude": longitude, "identifier": identifier, "locationZone": locationZone, "fullAddress": fullAddress, "reliable": reliable, "nfcTag": nfcTag, "tcType": tcType, "reliableZone": reliableZone, "selectedZone": selectedZone };
    this.callUrl(newUrl, this.varData);
}

//WebServiceRobotics.prototype.setStatus = function (causeId, direction, latitude, longitude, identifier, locationZone, fullAddress, reliable) {
//    var newUrl = this.varUrl + "/SetStatus";
//    this.varData = { "causeId": causeId, 'direction': direction, "latitude": latitude, "longitude": longitude, "identifier": identifier, "locationZone": locationZone, "fullAddress": fullAddress, "reliable": reliable};
//    this.callUrl(newUrl, this.varData);
//}

WebServiceRobotics.prototype.setStatusWithPhoto = function (causeId, direction, latitude, longitude, identifier, locationZone, fullAddress, reliable, imageURI, nfcTag, tcType, reliableZone = true, selectedZone = -1) {
    var newUrl = this.varUrl + "/SetStatusWithPhoto";

    //var params = new Object();
    //params.causeId = causeId;
    //params.direction = direction;
    //params.latitude = latitude;
    //params.longitude = longitude;
    //params.identifier = identifier;
    //params.locationZone = locationZone;
    //params.fullAddress = fullAddress;
    //params.reliable = reliable;
    //params.nfcTag = nfcTag;
    //this.callFileTranferUrl(newUrl, params, imageURI);

    var formData = new FormData();
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
    var newUrl = this.varUrl + "/SetCostCenterStatus";
    this.varData = {
        "costCenterId": costCenterId, "latitude": latitude, "longitude": longitude,
        "identifier": identifier, "locationZone": locationZone, "fullAddress": fullAddress, "reliable": reliable, "tcType": tcType
    };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setCostCenterPunchWithPhoto = function (costCenterId, latitude, longitude, identifier, locationZone, fullAddress, reliable, imageURI, tcType) {
    var newUrl = this.varUrl + "/SetCostCenterPunchWithPhoto";

    //var params = new Object();
    //params.costCenterId = costCenterId;
    //params.latitude = latitude;
    //params.longitude = longitude;
    //params.identifier = identifier;
    //params.locationZone = locationZone;
    //params.fullAddress = fullAddress;
    //params.reliable = reliable;

    //this.callFileTranferUrl(newUrl, params, imageURI);

    var formData = new FormData();
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
    var newUrl = this.varUrl + "/SetTaskPunch";
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
    var newUrl = this.varUrl + "/SetTaskPunchWithPhoto";

    //var params = new Object();
    //params.taskId = taskId;
    //params.newTaskId = newTaskId;
    //params.completeTask = completeTask;
    //params.latitude = latitude;
    //params.longitude = longitude;
    //params.identifier = identifier;
    //params.locationZone = locationZone;
    //params.fullAddress = fullAddress;
    //params.reliable = reliable;

    //params.oldValue0 = oldTaskValues[0];
    //params.oldValue1 = oldTaskValues[1];
    //params.oldValue2 = oldTaskValues[2];
    //params.oldValue3 = oldTaskValues[3];
    //params.oldValue4 = oldTaskValues[4];
    //params.oldValue5 = oldTaskValues[5];

    //params.newValue0 = newTaskValues[0];
    //params.newValue1 = newTaskValues[1];
    //params.newValue2 = newTaskValues[2];
    //params.newValue3 = newTaskValues[3];
    //params.newValue4 = newTaskValues[4];
    //params.newValue5 = newTaskValues[5];

    //this.callFileTranferUrl(newUrl, params, imageURI);

    var formData = new FormData();
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
    var newUrl = this.varUrl + "/GetMyCalendar";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getLoadSeatingCapacity = function (selectedDate) {
    var newUrl = this.varUrl + "/GetLoadSeatingCapacity";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getEmployeeDayInfo = function (selectedDate) {
    var newUrl = this.varUrl + "/GetEmployeeDayInfo";
    this.varData = { "strDayDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getAccrualsSummary = function (selectedDate) {
    var newUrl = this.varUrl + "/GetAccrualsSummary";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyAccruals = function (selectedDate) {
    var newUrl = this.varUrl + "/GetMyAccruals";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyDailyAccruals = function (selectedDate) {
    var newUrl = this.varUrl + "/GetMyDailyAccruals";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyPunches = function (selectedDate) {
    var newUrl = this.varUrl + "/GetMyPunches";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getCommuniqueById = function (idCommunique) {
    var newUrl = this.varUrl + "/GetCommuniqueById";
    this.varData = { "idCommunique": idCommunique };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.sendSurveyResponse = function (idSurvey, idEmployee, Response) {
    var newUrl = this.varUrl + "/SendSurveyResponse";
    var formData = new FormData();
    formData.append("idSurvey", idSurvey);
    formData.append("idEmployee", idEmployee);
    formData.append("Response", JSON.stringify(Response));
    this.callUrlPost(newUrl, formData);
}

WebServiceRobotics.prototype.getSurveyById = function (idSurvey) {
    var newUrl = this.varUrl + "/GetSurveyById";
    this.varData = { "idSurvey": idSurvey };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.answerCommunique = function (idCommunique, answer) {
    var newUrl = this.varUrl + "/AnswerCommunique";
    this.varData = { "idCommunique": idCommunique, "answer": answer };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.registerFirebaseToken = function (token, uuid) {
    var newUrl = this.varUrl + "/RegisterFirebaseToken";
    this.varData = { "token": token, "uuid": uuid };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getHolidaysInfo = function () {
    var newUrl = this.varUrl + "/GetHolidaysInfo";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getUserFields = function () {
    var newUrl = this.varUrl + "/GetUserFields";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getTelecommutingInfo = function () {
    var newUrl = this.varUrl + "/GetTelecommutingInfo";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getTeleworkingDetail = function (sDate, eDate) {
    var newUrl = this.varUrl + "/GetTeleworkingDetail";
    this.varData = { "iDate": moment(sDate).format("YYYYMMDD"), "eDate": moment(eDate).format("YYYYMMDD") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyDailyTeleWorking = function (sDate) {
    var newUrl = this.varUrl + "/GetMyDailyTeleWorking";
    this.varData = { "selectedDate": moment(sDate).format("YYYYMMDD") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyRequests = function (showAll, sDate, eDate, filter, order, sRequestedDate, eRequestedDate) {
    var newUrl = this.varUrl + "/GetMyRequests";
    this.varData = { "showAll": showAll, "dateStart": sDate != null ? moment(sDate).format("YYYY-MM-DD HH:mm") : "", "dateEnd": eDate != null ? moment(eDate).format("YYYY-MM-DD HH:mm") : "", "filter": filter, "orderBy": order, "dateRequestedStart": sRequestedDate != null ? moment(sRequestedDate).format("YYYY-MM-DD HH:mm") : "", "dateRequestedEnd": eRequestedDate != null ? moment(eRequestedDate).format("YYYY-MM-DD HH:mm") : "" };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getSupervisedRequests = function (showAll, sDate, eDate, filter, order) {
    var newUrl = this.varUrl + "/GetSupervisedRequests";
    this.varData = { "showAll": showAll, "dateStart": sDate != null ? moment(sDate).format("YYYY-MM-DD HH:mm") : "", "dateEnd": eDate != null ? moment(eDate).format("YYYY-MM-DD HH:mm") : "", "filter": filter, "orderBy": order };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getOverlappingEmployees = function (idRequest) {
    var newUrl = this.varUrl + "/GetOverlappingEmployees";
    this.varData = { "idRequest": idRequest };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getZones = function () {
    var newUrl = this.varUrl + "/GetZones";
    varData = {};
    this.callUrl(newUrl, varData);
}

WebServiceRobotics.prototype.getMyLeaves = function (showAll, sDate, eDate) {
    var newUrl = this.varUrl + "/GetMyLeaves";
    this.varData = { "showAll": showAll, "dateStart": sDate != null ? moment(sDate).format("YYYY-MM-DD HH:mm") : "", "dateEnd": eDate != null ? moment(eDate).format("YYYY-MM-DD HH:mm") : "" };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyPermissions = function () {
    var newUrl = this.varUrl + "/GetMyPermissions";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setRequestReaded = function (requestId) {
    var newUrl = this.varUrl + "/SetRequestReaded";
    this.varData = { "requestId": requestId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.setNotificationReaded = function (notificationtId) {
    var newUrl = this.varUrl + "/SetNotificationReaded";
    this.varData = { "notificationtId": notificationtId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRequest = function (requestId) {
    var newUrl = this.varUrl + "/GetRequest";
    this.varData = { "requestId": requestId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getPunchesOnDate = function (idEmployee, selectedDate) {
    var newUrl = this.varUrl + "/GetPunchesOnDate";
    this.varData = { "idEmployee": idEmployee, "selectedDate": moment(selectedDate).format("YYYY-MM-DD") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRequestForSupervisor = function (requestId) {
    var newUrl = this.varUrl + "/GetRequestForSupervisor";
    this.varData = { "requestId": requestId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.saveUserFieldRequest = function (fieldName, fieldValue, comments, hasHistory, historyDate, acceptWarning) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestUserField";
        var formData = new FormData();
        formData.append("fieldName", fieldName);
        formData.append("fieldValue", fieldValue);
        formData.append("comments", comments);
        formData.append("hasHistory", hasHistory);
        formData.append("historyDate", historyDate.format("YYYY-MM-DD HH:mm"));
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_UserField";
        this.varData = { "fieldName": encodeURIComponent(fieldName), "fieldValue": encodeURIComponent(fieldValue), "comments": encodeURIComponent(comments), "hasHistory": hasHistory, "historyDate": historyDate.format("YYYY-MM-DD HH:mm") };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveForbbidenPunch = function (punchDateTime, causeId, comments, direction, acceptWarning, tcType) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestForbiddenPunch";
        var formData = new FormData();
        formData.append("punchDate", punchDateTime.format("YYYY-MM-DD HH:mm"));
        formData.append("idCause", causeId == "" ? "-1" : causeId);
        formData.append("comments", comments);
        formData.append("direction", direction);
        formData.append("acceptWarning", acceptWarning);
        formData.append("tcType", ((typeof tcType == 'undefined') ? '' : tcType));
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_ForbiddenPunch";
        this.varData = { "punchDate": punchDateTime.format("YYYY-MM-DD HH:mm"), "idCause": causeId == "" ? "-1" : causeId, "direction": direction, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.savePunchCause = function (idCause, punchDate, comments, acceptWarning) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestJustifyPunch";
        var formData = new FormData();
        formData.append("punchDate", punchDate.format("YYYY-MM-DD HH:mm:ss"));
        formData.append("idCause", idCause);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_JustifyPunch";
        this.varData = { "idCause": idCause, "punchDate": punchDate.format("YYYY-MM-DD HH:mm:ss"), "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.sendPunch = function (punch) {
    var instance = this;

    return new Promise(function (resolve, reject) {
        var newUrl = instance.varUrl + "/SavePunch";

        instance.promiseResolve = resolve;
        instance.promiseReject = reject;

        var formData = new FormData();
        formData.append("oPunch", JSON.stringify(punch));

        instance.callUrlPost(newUrl, formData);
    });
};

WebServiceRobotics.prototype.saveExternalWork = function (externalWorkDate, duration, comments, idCause, acceptWarning) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestExternalWork";
        var formData = new FormData();
        formData.append("externalWorkDate", externalWorkDate.format("YYYY-MM-DD HH:mm:ss"));
        formData.append("idCause", idCause);
        formData.append("duration", duration);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_ExternalWork";
        this.varData = { "externalWorkDate": externalWorkDate.format("YYYY-MM-DD"), "idCause": idCause, "duration": duration, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveChangeShift = function (from, to, idRequestedShift, idReplaceShift, strStartShiftHour, comments, acceptWarning) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestChangeShift";
        var formData = new FormData();
        formData.append("fromDate", from.format("YYYY-MM-DD HH:mm"));
        formData.append("toDate", to.format("YYYY-MM-DD HH:mm"));
        formData.append("idRequestedShift", idRequestedShift);
        formData.append("idReplaceShift", idReplaceShift);
        formData.append("strStartShiftHour", strStartShiftHour);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_ChangeShift";
        this.varData = {
            "fromDate": from.format("YYYY-MM-DD HH:mm"), "toDate": to.format("YYYY-MM-DD HH:mm"), "idRequestedShift": idRequestedShift, "idReplaceShift": idReplaceShift,
            "strStartShiftHour": strStartShiftHour, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning
        };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveShiftExchange = function (sDate, sCompensateDate, idEmployee, idShift, idSourceShift, comments, acceptWarning) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestShiftExchange";
        var formData = new FormData();
        formData.append("sDate", moment(sDate).format("YYYY-MM-DD HH:mm"));
        formData.append("sCompensateDate", sCompensateDate);
        formData.append("idEmployee", idEmployee);
        formData.append("idShift", idShift);
        formData.append("idSourceShift", idSourceShift);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_ShiftExchange";
        this.varData = { "sDate": moment(sDate).format("YYYY-MM-DD HH:mm"), "sCompensateDate": sCompensateDate, "idEmployee": idEmployee, 'idShift': idShift, 'idSourceShift': idSourceShift, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.savePermissions = function (strDates, idHolidays, comments, acceptWarning) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestHolidays";
        var formData = new FormData();
        formData.append("strDates", strDates);
        formData.append("idHolidays", idHolidays);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_Holidays";
        this.varData = { "strDates": strDates, "idHolidays": idHolidays, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveAbsences = function (from, to, idCause, comments, acceptWarning, documentData) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var ua = window.navigator.userAgent;
        var msie = ua.indexOf("MSIE ");
        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
            var newUrl = this.varUrl + "/SaveRequestPlannedAbsences";
            var formData = new FormData();

            formData.append("fromDate", from.format("YYYY-MM-DD HH:mm"));
            formData.append("toDate", to.format("YYYY-MM-DD HH:mm"));
            formData.append("idCause", idCause);
            formData.append("comments", comments);
            formData.append("acceptWarning", acceptWarning);
            this.callUrlPost(newUrl, formData);
        }
        else {
            if (documentData.get('userfile') != '') {
                var newUrl = this.varUrl + "/SaveRequestPlannedAbsences";

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
            }
            else {
                var newUrl = this.varUrl + "/SaveRequestPlannedAbsences";
                var formData = new FormData();

                formData.append("fromDate", from.format("YYYY-MM-DD HH:mm"));
                formData.append("toDate", to.format("YYYY-MM-DD HH:mm"));
                formData.append("idCause", idCause);
                formData.append("comments", comments);
                formData.append("acceptWarning", acceptWarning);
                this.callUrlPost(newUrl, formData);
            }
        }
    } else {
        var newUrl = this.varUrl + "/SaveRequest_PlannedAbsences";
        this.varData = { "fromDate": from.format("YYYY-MM-DD HH:mm"), "toDate": to.format("YYYY-MM-DD HH:mm"), "idCause": idCause, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveIncidence = function (sDate, eDate, from, to, duration, idCause, comments, acceptWarning, documentData) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var ua = window.navigator.userAgent;
        var msie = ua.indexOf("MSIE ");
        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
            var newUrl = this.varUrl + "/SaveRequestPlannedCauses";
            var formData = new FormData();
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
                var newUrl = this.varUrl + "/SaveRequestPlannedCauses";

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
                var newUrl = this.varUrl + "/SaveRequestPlannedCauses";
                var formData = new FormData();
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
    } else {
        var newUrl = this.varUrl + "/SaveRequest_PlannedCauses";
        this.varData = {
            "fromDate": sDate.format("YYYY-MM-DD HH:mm"), "toDate": eDate.format("YYYY-MM-DD HH:mm"), "fromHour": from.format("YYYY-MM-DD HH:mm"), "toHour": to.format("YYYY-MM-DD HH:mm"),
            "duration": duration, "idCause": idCause, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning
        };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveOvertime = function (sDate, eDate, from, to, duration, idCause, comments, acceptWarning, documentData) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var ua = window.navigator.userAgent;
        var msie = ua.indexOf("MSIE ");
        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
            var newUrl = this.varUrl + "/SaveRequestPlannedOvertime";
            var formData = new FormData();
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
                var newUrl = this.varUrl + "/SaveRequestPlannedOvertime";

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
                var newUrl = this.varUrl + "/SaveRequestPlannedOvertime";
                var formData = new FormData();
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
    } else {
        var newUrl = this.varUrl + "/SaveRequest_PlannedOvertime";
        this.varData = {
            "fromDate": sDate.format("YYYY-MM-DD HH:mm"), "toDate": eDate.format("YYYY-MM-DD HH:mm"), "fromHour": from.format("YYYY-MM-DD HH:mm"), "toHour": to.format("YYYY-MM-DD HH:mm"),
            "duration": duration, "idCause": idCause, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning
        };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.savePlannedHoliday = function (sDate, allDay, from, to, idCause, comments, acceptWarning) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestPlannedHoliday";
        var formData = new FormData();
        formData.append("datesStr", sDate);
        formData.append("allDay", allDay);
        formData.append("fromHour", from.format("YYYY-MM-DD HH:mm"));
        formData.append("toHour", to.format("YYYY-MM-DD HH:mm"));
        formData.append("idCause", idCause);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_PlannedHoliday";
        this.varData = {
            "datesStr": sDate, "allDay": allDay, "fromHour": from.format("YYYY-MM-DD HH:mm"), "toHour": to.format("YYYY-MM-DD HH:mm"), "idCause": idCause, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning
        };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.SaveRequest_ForbiddenTaskPunch = function (sDate, taskId, eDate, cTaskId, completeTask, comments, value1, value2, value3, value4, value5, value6, acceptWarning, tcType) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestForbiddenTaskPunch";
        var formData = new FormData();
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
    } else {
        var newUrl = this.varUrl + "/SaveRequest_ForbiddenTaskPunch";
        this.varData = {
            "punchDate": sDate, "continueOnPunchDate": eDate, "idTask": taskId, "idContinueOnTask": cTaskId, "completeTask": completeTask,
            "comments": encodeURIComponent(comments), "value1": value1, "value2": value2, "value3": value3, "value4": value4, "value5": value5, "value6": value6, "acceptWarning": acceptWarning
        };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveCancelHolidays = function (strDates, idCause, idShift, comments, acceptWarning) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestCancelHolidays";
        var formData = new FormData();
        formData.append("strDates", strDates);
        formData.append("idShift", idShift);
        formData.append("idCause", idCause);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_CancelHolidays";
        this.varData = { "strDates": strDates, "idCause": idCause, "idShift": idShift, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveCostCenterPunch = function (punchDate, idCostCenter, comments, acceptWarning, tcType) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestForbiddenCostCenterPunch";
        var formData = new FormData();
        formData.append("punchDate", punchDate.format("YYYY-MM-DD HH:mm"));
        formData.append("idCostCenter", idCostCenter == "" ? -1 : idCostCenter);
        formData.append("comments", comments);
        formData.append("acceptWarning", acceptWarning);
        formData.append("tcType", ((typeof tcType == 'undefined') ? '' : tcType));
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_ForbiddenCostCenterPunch";
        this.varData = { "punchDate": punchDate.format("YYYY-MM-DD HH:mm"), "idCostCenter": idCostCenter == "" ? -1 : idCostCenter, "comments": encodeURIComponent(comments), "acceptWarning": acceptWarning };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveExternalWorkWeekResume = function (remarks, rInfo, acceptWarning) {
    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PasswordEncrypt) {
        var newUrl = this.varUrl + "/SaveRequestExternalWorkWeekResume";
        var formData = new FormData();
        formData.append("resumeInfo", JSON.stringify(rInfo));
        formData.append("comments", remarks);
        formData.append("acceptWarning", acceptWarning);
        this.callUrlPost(newUrl, formData);
    } else {
        var newUrl = this.varUrl + "/SaveRequest_ExternalWorkWeekResume";
        this.varData = { 'comments': remarks, 'resumeInfo': JSON.stringify(rInfo), 'acceptWarning': acceptWarning };
        this.callUrl(newUrl, this.varData);
    }
}

WebServiceRobotics.prototype.saveProfileImageDesktop = function (documentData) {
    var newUrl = this.varUrl + "/SaveProfileImage";

    this.callUploadFileDesktop(newUrl, documentData);
}

WebServiceRobotics.prototype.saveProfileImageMobile = function (documentData) {
    var newUrl = this.varUrl + "/SaveProfileImage";

    var formData = new FormData();
    //formData.append("userfile", documentData.get('userfile'));
    //this.callUploadFileDesktop(newUrl, formData);

    this.callFileTranferUrl(newUrl, formData, documentData.get('userfile'));
}

WebServiceRobotics.prototype.saveLeaveDesktop = function (from, to, idCause, comments, documentData) {
    var newUrl = this.varUrl + "/SaveLeave";

    documentData.append("from", from.format("YYYY-MM-DD"));
    documentData.append("to", to.format("YYYY-MM-DD"));
    documentData.append("idCause", idCause);
    documentData.append("remarks", comments);

    this.callUploadFileDesktop(newUrl, documentData);
}

WebServiceRobotics.prototype.saveLeaveMobile = function (from, to, idCause, comments, documentData) {
    var newUrl = this.varUrl + "/SaveLeave";

    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        documentData.append("from", from.format("YYYY-MM-DD"));
        documentData.append("to", to.format("YYYY-MM-DD"));
        documentData.append("idCause", idCause);
        documentData.append("remarks", comments);

        this.callUploadFileDesktop(newUrl, documentData);
    }
    else {
        if (documentData.get('userfile') != '') {
            //var params = new Object();
            //params.from = from.format("YYYY-MM-DD");
            //params.to = to.format("YYYY-MM-DD");
            //params.idCause = idCause;
            //params.remarks = comments;
            //params.idTemplateDocument = documentData.get('idTemplateDocument');

            //this.callFileTranferUrl(newUrl, params, documentData.get('userfile'));

            var formData = new FormData();
            formData.append("from", from.format("YYYY-MM-DD"));
            formData.append("to", to.format("YYYY-MM-DD"));
            formData.append("idCause", idCause);
            formData.append("remarks", comments);
            formData.append("idTemplateDocument", documentData.get('idTemplateDocument'));

            this.callFileTranferUrl(newUrl, formData, documentData.get('userfile'));

            //formData.append("userfile", documentData.get('userfile'));
            //this.callUploadFileDesktop(newUrl, formData);
        } else {
            documentData.append("from", from.format("YYYY-MM-DD"));
            documentData.append("to", to.format("YYYY-MM-DD"));
            documentData.append("idCause", idCause);
            documentData.append("remarks", comments);

            this.callUploadFileDesktop(newUrl, documentData);
        }
    }
}

WebServiceRobotics.prototype.checkIsWorkingdayShift = function (shiftId) {
    var newUrl = this.varUrl + "/CheckIsWorkingdayShift";
    this.varData = { "shiftId": shiftId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.checkIsFloatingShift = function (shiftId) {
    var newUrl = this.varUrl + "/CheckIsFloatingShift";
    this.varData = { "shiftId": shiftId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getUserFieldAvailableValues = function (fieldName) {
    var newUrl = this.varUrl + "/GetUserFieldAvailableValues";
    this.varData = { "fieldName": fieldName };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.deleteRequest = function (requestId) {
    var newUrl = this.varUrl + "/DeleteRequest";
    this.varData = { "requestId": requestId };
    this.callUrl(newUrl, this.varData);
}
WebServiceRobotics.prototype.deleteAbsence = function (absenceId) {
    var newUrl = this.varUrl + "/DeleteAbsence";
    this.varData = { "absenceId": absenceId };
    this.callUrl(newUrl, this.varData);
}
WebServiceRobotics.prototype.deleteProgrammedAbsence = function (forecastId, forecastType) {
    var newUrl = this.varUrl + "/DeleteProgrammedAbsence";
    this.varData = { "forecastId": forecastId, "forecastType": forecastType };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.uploadDocumentDesktop = function (formData) {
    var newUrl = this.varUrl + "/UploadDocument";
    this.callUploadFileDesktop(newUrl, formData);
}

WebServiceRobotics.prototype.uploadDocumentMobile = function (documentData, forecastType, remarks, docRelatedInfo) {
    var newUrl = this.varUrl + "/UploadDocument";

    //var params = new Object();
    //params.forecastType = forecastType;
    //params.remarks = remarks;
    //params.docRelatedInfo = docRelatedInfo;
    //params.idRelatedObject = documentData.get('idRelatedObject');
    //params.idTemplateDocument = documentData.get('idTemplateDocument');

    //this.callFileTranferUrl(newUrl, params, documentData.get('userfile'));

    var formData = new FormData();
    formData.append("forecastType", forecastType);
    formData.append("remarks", remarks);
    formData.append("docRelatedInfo", docRelatedInfo);
    formData.append("idRelatedObject", documentData.get('idRelatedObject'));
    formData.append("idTemplateDocument", documentData.get('idTemplateDocument'));

    this.callFileTranferUrl(newUrl, formData, documentData.get('userfile'));
    //formData.append("userfile", documentData.get('userfile'));
    //this.callUploadFileDesktop(newUrl, formData);
}

WebServiceRobotics.prototype.getMyDocuments = function (sDate, eDate, filter, order) {
    var newUrl = this.varUrl + "/GetMyDocuments";
    this.varData = { "dateStart": sDate != null ? moment(sDate).format("YYYY-MM-DD HH:mm") : "", "dateEnd": eDate != null ? moment(eDate).format("YYYY-MM-DD HH:mm") : "", "filter": filter, "orderBy": order };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getDocumentBytes = function (documentId) {
    var newUrl = this.varUrl + "/GetDocumentBytes";
    this.varData = { "documentId": documentId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.uploadDocumenttoSign = function (documentId) {
    var newUrl = this.varUrl + "/UploadDocumenttoSign";
    this.varData = { "documentId": documentId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getAvailablePermitsCalendar = function (sDate, idCause) {
    var newUrl = this.varUrl + "/GetAvailablePermitsCalendar";
    this.varData = { "selectedDate": moment(sDate).format("DD/MM/YYYY"), "ProgrammedHoliday_IDCause": idCause };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getTaskUserFieldsAction = function (action, taskId) {
    var newUrl = this.varUrl + "/GetTaskUserFieldsAction";
    this.varData = { "action": action, "taskId": taskId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.saveNewTaskAlert = function (taskId, taskTextAlert) {
    var newUrl = this.varUrl + "/SaveNewTaskAlert";
    this.varData = { "taskId": taskId, "taskTextAlert": encodeURIComponent(taskTextAlert) };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getLoggedInUserInfo = function () {
    var newUrl = this.varUrl + "/GetLoggedInUserInfo";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRequieredLeaveDocuments = function (idCause, isStarting) {
    var newUrl = this.varUrl + "/GetRequieredLeaveDocuments";
    this.varData = { 'idCause': idCause, 'isStarting': isStarting };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getAvailableDocumentTemplateType = function (idEmployee) {
    var newUrl = this.varUrl + "/GetAvailableDocumentTemplateType";
    this.varData = { 'idEmployee': idEmployee };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRequieredCauseDocuments = function (idCause) {
    var newUrl = this.varUrl + "/GetRequieredCauseDocuments";
    this.varData = { 'idCause': idCause };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getUserFile = function (token, userFieldName) {
    var newUrl = this.varUrl + "/GetUserFile?token=" + token + "&userFieldName=" + encodeURIComponent(userFieldName);
    return newUrl;
}

WebServiceRobotics.prototype.getCurrentTaskInfo = function (token, taskId) {
    var newUrl = this.varUrl + "/GetCurrentTaskInfo";
    this.varData = { "token": token, "taskId": taskId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getTaskInfo = function (token, taskId) {
    var newUrl = this.varUrl + "/GetTaskInfo";
    this.varData = { "token": token, "taskId": taskId };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyEmployees = function () {
    var newUrl = this.varUrl + "/GetMyEmployees";
    this.varData = {};
    this.callUrl(newUrl, this.varData, 60000);
}
WebServiceRobotics.prototype.getCapacityDetail = function (selectedDate) {
    var newUrl = this.varUrl + "/GetCapacityDetail";
    this.varData = { "selectedDate": moment(selectedDate).format("DD/MM/YYYY") };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyOnBoardings = function () {
    var newUrl = this.varUrl + "/GetMyOnBoardings";
    this.varData = {};
    this.callUrl(newUrl, this.varData, 60000);
}

WebServiceRobotics.prototype.getOnBoardingtasksById = function (idOnboarding) {
    var newUrl = this.varUrl + "/GetOnBoardingTasksById";
    this.varData = { "idOnboarding": idOnboarding };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.updateTaskStatus = function (status, idTask, idList) {
    var newUrl = this.varUrl + "/UpdateTaskStatus";
    this.varData = { "status": status, "idTask": idTask, "idList": idList };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getMyEmployeesWithStatus = function () {
    var newUrl = this.varUrl + "/GetMyEmployeesWithStatus";
    this.varData = {};
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getSupervisorAlertsDetail = function (idAlertType) {
    var newUrl = this.varUrl + "/GetSupervisorAlertsDetail";
    this.varData = { "idAlertType": idAlertType };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.approveRefuseRequest = function (idRequest, bApprove, bForceApprove) {
    var newUrl = this.varUrl + "/ApproveRefuseRequest";
    this.varData = { "idRequest": idRequest, "bApprove": bApprove, "bForceApprove": bForceApprove };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.approveRefuseRequestNew = function (idRequest, bApprove, bForceApprove) {
    var newUrl = this.varUrl + "/ApproveRefuseRequestNew";
    this.varData = { "idRequest": idRequest, "bApprove": bApprove, "bForceApprove": bForceApprove };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.approveRefuseRequestByEmployee = function (idRequest, bApprove, bForceApprove) {
    var newUrl = this.varUrl + "/ApproveRefuseRequestByEmployee";
    this.varData = { "idRequest": idRequest, "bApprove": bApprove, "bForceApprove": bForceApprove };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getAvailableEmployeesForDate = function (sDate, eDate, iDestinationShift, iSourceShift) {
    var newUrl = this.varUrl + "/GetAvailableEmployeesForDate";
    this.varData = { "sDate": moment(sDate).format("DD/MM/YYYY"), "eDate": moment(eDate).format("DD/MM/YYYY"), "iSourceShift": iSourceShift, "iDestinationShift": iDestinationShift };
    this.callUrl(newUrl, this.varData, 60000);
}

WebServiceRobotics.prototype.getDaysToCompensate = function (sDate, eDate, iExchangeEmployee) {
    var newUrl = this.varUrl + "/GetDaysToCompensate";
    this.varData = { "sDate": moment(sDate).format("DD/MM/YYYY"), "eDate": moment(eDate).format("DD/MM/YYYY"), "iExchangeEmployee": iExchangeEmployee };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.getRankingForDay = function (sDate, idRequestType, idReason) {
    var newUrl = this.varUrl + "/GetRankingForDay";
    this.varData = { "sDate": moment(sDate).format("DD/MM/YYYY"), "idRequestType": idRequestType, "idReason": idReason };
    this.callUrl(newUrl, this.varData);
}

WebServiceRobotics.prototype.isAdfsActive = function () {
    var newUrl = this.varUrl + "/isAdfsActive";
    this.varData = {};
    this.callUrl(newUrl, this.varData, 5000);
}

WebServiceRobotics.prototype.updateServerLanguage = function (lang) {
    var newUrl = this.varUrl + "/updateServerLanguage";
    this.varData = { "lang": lang };
    this.callUrl(newUrl, this.varData, 2000);
}

WebServiceRobotics.prototype.updateServerTimeZone = function (timeZone) {
    var newUrl = this.varUrl + "/updateServerTimeZone";
    this.varData = { "timeZone": timeZone };
    this.callUrl(newUrl, this.varData, 2000);
}