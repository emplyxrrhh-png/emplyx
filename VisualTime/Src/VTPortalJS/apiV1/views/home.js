VTPortal.home = function (params) {
    var selectedDate = ko.observable(new Date());

    var mainMenuItems = ko.observable([]);

    var summaryHome = VTPortal.summaryHome(),
        notificationsHome = VTPortal.notificationsHome(),
        punchesHome = VTPortal.punchesHome(),
        accrualsHome = VTPortal.accrualsHome(),
        emptyHome = VTPortal.emptyHome(),
        surveysHome = VTPortal.surveysHome(),
        telecommutingHome = VTPortal.telecommutingHome(),
        communiquesHome = VTPortal.communiquesHome();
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var progressVisible = ko.observable(false);
    var optionsVisible = ko.observable(false);
    var progressLocVisible = ko.observable(false);
    var licenseNotReaded = ko.observable(false);
    var popupChangePwdVisible = ko.observable(false), popupValidationCodeVisible = ko.observable(false);
    var quickPunchDone = ko.observable(false);
    var quickPunchFailed = ko.observable(false);
    var punchNotAllowedVisible = ko.observable(false);
    var lblPunchDone = ko.observable(false);
    var punchAllowedBool = ko.observable(false);
    var hasMandatoryCommuniques = ko.observable(false);
    var tcType = ko.observable('');
    var hasMandatorySurveys = ko.observable(false);
    var hasMandatoryAnswer = ko.observable(false);
    var showPopupAnswer = ko.observable(false);
    var showPopupCommuniques = ko.observable(false);
    var showPopupSurveys = ko.observable(false);
    var later = ko.observable(false);
    var punchAllowedBoolInterface = ko.observable(false);
    var txtOldPwd = ko.observable(''), txtNewPwd = ko.observable(''), txtNewPwdRepeat = ko.observable(''), txtValidationCode = ko.observable('');
    var bLicense = ko.observable(false);
    var ApiVersion = ko.computed(function () {
        return VTPortal.roApp.db.settings.ApiVersion;
    });
    var HasSurveys = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            var surveys = VTPortal.roApp.userStatus().Surveys;
            if (surveys != null && typeof surveys != 'undefined' && surveys.length > 0) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var TelecommutingEnabled = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            var tcEnabled = VTPortal.roApp.userStatus().Telecommuting;
            if (tcEnabled != null && typeof tcEnabled != 'undefined' && tcEnabled) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    });

    var calculateMandatory = ko.computed(function () {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
            if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
                for (var i = 0; i < VTPortal.roApp.userStatus().Communiques.length; ++i) {
                    if (VTPortal.roApp.userStatus().Communiques[i].EmployeeCommuniqueStatus[0].AnswerRequired == true && VTPortal.roApp.userStatus().Communiques[i].EmployeeCommuniqueStatus[0].Answer == "" && VTPortal.roApp.userStatus().Communiques[i].Communique.MandatoryRead == true && VTPortal.roApp.userStatus().Communiques[i].Communique.Archived == false) {
                        if (later() == false) {
                            hasMandatoryAnswer(true);
                        }
                    }
                }

                for (var i = 0; i < VTPortal.roApp.userStatus().Communiques.length; ++i) {
                    if (VTPortal.roApp.userStatus().Communiques[i].EmployeeCommuniqueStatus[0].Read == false && VTPortal.roApp.userStatus().Communiques[i].Communique.MandatoryRead == true && VTPortal.roApp.userStatus().Communiques[i].Communique.Archived == false) {
                        hasMandatoryCommuniques(true);
                    }
                }
            }
        }

        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Surveys) {
            if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
                if (typeof VTPortal.roApp.userStatus().Surveys != 'undefined') {
                    for (var i = 0; i < VTPortal.roApp.userStatus().Surveys.length; ++i) {
                        if (VTPortal.roApp.userStatus().Surveys[i].IsMandatory == true) {
                            if (later() == false && VTPortal.roApp.stopShowingSurveysPopup() == false) {
                                VTPortal.roApp.stopShowingSurveysPopup(true);
                                hasMandatorySurveys(true);
                            }
                        }
                    }
                }
            }
        }
    });

    var showPopups = ko.computed(function () {
        if (popupChangePwdVisible() == false && popupValidationCodeVisible() == false) {
            if (hasMandatoryAnswer() == true && hasMandatoryCommuniques() == true) {
                showPopupAnswer(true);
                showPopupCommuniques(true);
            }
            else if (hasMandatoryAnswer() == false && hasMandatoryCommuniques() == true) {
                showPopupAnswer(false);
                showPopupCommuniques(true);
            }
            else if (hasMandatoryAnswer() == true && hasMandatoryCommuniques() == false) {
                showPopupAnswer(true);
                showPopupCommuniques(false);
            }
            else {
                showPopupAnswer(false);
                showPopupCommuniques(false);
            }

            if (hasMandatorySurveys() == true) {
                showPopupSurveys(true);
            }
            else {
                showPopupSurveys(false);
            }
        }
        else {
            showPopupAnswer(false);
            showPopupCommuniques(false);
        }
    });

    var licenseVisible = ko.computed(function () {
        return (!VTPortal.roApp.licenseAccepted() && (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.LicenseAgreement) && (VTPortal.roApp.impersonatedIDEmployee == -1));
    });

    var lbllicenseAdv2Visibile = ko.computed(function () {
        return (VTPortal.roApp.AnywhereLicense == true ? "display:''" : "display:'none'");
    });

    var lbllicenseAdv3Visibile = ko.computed(function () {
        return (VTPortal.roApp.AnywhereLicense == true ? "display:''" : "display:'none'");
    });

    var licenseConsent = ko.computed(function () {
        if (VTPortal.roApp.licenseConsent() == null) {
            return i18nextko.i18n.t('roLicenseText');
        } else {
            return VTPortal.roApp.licenseConsent().Message;
        }
    });

    var txtQuickPunch = ko.computed(function () {
        if (!VTPortal.roApp.lastRequestFailed()) {
            if (VTPortal.roApp.userStatus() != null) {
                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                    return i18nextko.i18n.t('roQuickPunchOut');
                } else {
                    return i18nextko.i18n.t('roQuickPunchIn');
                }
            } else return i18nextko.i18n.t('roQuickPunchIn');
        } else {
            return i18nextko.i18n.t('roQuickPunchIn');
        }
    });

    var txtpunchNotAllowed = ko.computed(function () {
        return i18nextko.i18n.t('ropunchNotAllowed');
    });

    var customConsent = ko.computed(function () {
        if (VTPortal.roApp.licenseConsent() == null) {
            return false;
        } else {
            return true;
        }
    });

    var btnChangePwd = function (params) {
        var result = params.validationGroup.validate();
        if (result.isValid) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.loggedIn = false;
                    VTPortal.roApp.db.settings.password = txtNewPwd();
                    VTPortal.roApp.db.settings.save();
                    txtOldPwd('');
                    txtNewPwd('');
                    txtNewPwdRepeat('');

                    $('#scrollview').find('.pass').each(function (index) {
                        $(this).dxValidator('instance').reset();
                    });

                    popupChangePwdVisible(false);
                    window.VTPortalUtils.utils.loginIfNecessary('', function () {
                        if (VTPortal.roApp.db.settings.onlySupervisor) {
                            VTPortal.roApp.redirectAtHome(false);
                        } else {
                            VTPortal.roApp.redirectAtHome(true);
                        }
                    });
                } else {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
                }
            }).changePassword(txtOldPwd(), txtNewPwd());
        }
    }

    var btnAcceptLicense = function (params) {
        if (bLicense()) {
            if (VTPortal.roApp.licenseConsent() == null) {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        VTPortal.roApp.db.settings.licenseAccepted = true;
                        VTPortal.roApp.db.settings.save();
                        VTPortal.roApp.licenseAccepted(true);
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
                    }
                }).acceptMyLicense(bLicense());
            } else {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        VTPortal.roApp.db.settings.licenseAccepted = true;
                        VTPortal.roApp.db.settings.save();
                        VTPortal.roApp.licenseAccepted(true);
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
                    }
                }).acceptConsent(VTPortal.roApp.licenseConsent().Message);
            }
        }
    }

    var punchAllowedCalc = function () {
        if (VTPortal.roApp.empPermissions() == null || VTPortal.roApp.userStatus() == null) {
            punchAllowedBool(false);
            punchAllowedBoolInterface(false);
        }
        else {
            if ((VTPortal.roApp.empPermissions().Punch.SchedulePunch && VTPortal.roApp.userStatus().PunchesEnabled)) {
                punchAllowedBool(true);
            } else {
                punchAllowedBool(false);
            }

            if ((VTPortal.roApp.empPermissions().Punch.SchedulePunch && VTPortal.roApp.userStatus().PunchesEnabled) ||
                (VTPortal.roApp.empPermissions().Punch.ProductiVPunch && VTPortal.roApp.userStatus().ProductiVEnabled) ||
                (VTPortal.roApp.empPermissions().Punch.CostCenterPunch && VTPortal.roApp.userStatus().PunchesEnabled && VTPortal.roApp.userStatus().CostCenterEnabled)) {
                punchAllowedBoolInterface(true);
            } else {
                punchAllowedBoolInterface(false);
            }
        }
    }

    var btnSendValidation = function (params) {
        var result = params.validationGroup.validate();
        if (result.isValid) {
            popupValidationCodeVisible(false);
            window.VTPortalUtils.utils.loginIfNecessary(txtValidationCode(), function () {
                if (VTPortal.roApp.db.settings.onlySupervisor) {
                    VTPortal.roApp.redirectAtHome(false);
                } else {
                    VTPortal.roApp.redirectAtHome(true);
                }
            });
        }
    }

    var title = ko.computed(function () {
        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            return i18nextko.i18n.t('roHomeTitle');
        } else {
            return i18nextko.i18n.t('roHomeTitle');
            //return VTPortal.roApp.impersonatedNameEmployee;
            //return i18nextko.i18n.t('roHomeTitle');//"3px solid #6b96eb";
        }
    });

    var doPunch = function () {
        if (punchAllowedBoolInterface() == true) {
            VTPortal.app.navigate("punches");
        }
        else {
            punchNotAllowedVisible(true);
        }
    }

    var newRequest = function () {
        VTPortal.app.navigate('requestsList/1,2,3,4,5,6,7,8,9,10,11,12,13,14,15/' + moment(selectedDate.Value).format("YYYY-MM-DD"));
    }

    var goBack = function () {
        VTPortal.roApp.impersonatedIDEmployee = -1;
        if (VTPortal.roApp.db.settings.onlySupervisor) {
            VTPortal.roApp.redirectAtHome(false);
        } else {
            VTPortal.roApp.selectedTab(2);
            VTPortal.app.navigate("myteam", { root: true });
        }

        VTPortal.roApp.loadInitialData(true, true, true, false, false);
    }

    var refreshSupervisedRequests = function () {
        //if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
        //    if (VTPortal.roApp.isOnline()) {
        //        if (VTPortal.roApp.userStatus() != null && VTPortal.roApp.loggedIn == true) {
        //            var callbackFuntion = function (result) {
        //                if (result.Status == window.VTPortalUtils.constants.OK.value) {
        //                    VTPortal.roApp.lastOkRequests(new Date());
        //                    var requestsInfo = result.Requests;
        //                    VTPortal.roApp.supervisedRequests(requestsInfo.length);

        //                } else {
        //                    VTPortal.roApp.supervisedRequests(0);
        //                }
        //            };

        //            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getSupervisedRequests(false, moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').startOf('year').add(1, 'year'), '0*1|1*2*3*4*5*6*7*8*9*10*11*12*13*14*15', 'RequestDate DESC');

        //        }
        //    }
        //}
    }

    var resume = function () {
        later(false);
        VTPortal.roApp.punchInProgress(false);
        punchesHome.viewShown();
        accrualsHome.viewShown();
        notificationsHome.viewShown();
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
            communiquesHome.viewShown();
        }
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userStatus().Telecommuting == true) {
            telecommutingHome.viewShown();
        }
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Surveys) {
            surveysHome.viewShown();
        }
        var now = new Date();
        var diffMs = (now - VTPortal.roApp.lastOkRefresh());
        var diffMins = Math.round(((diffMs % 86400000) % 3600000) / 60000);

        if (diffMins > 10) {
            VTPortal.roApp.refreshEmployeeStatus(true, true);
        }

        if (VTPortal.roApp.lastOkRequests() == null) {
            refreshSupervisedRequests();
        }

        else {
            var diffMsRequests = (now - VTPortal.roApp.lastOkRequests());
            var diffMinsRequests = Math.round(((diffMsRequests % 86400000) % 3600000) / 60000);

            if (diffMinsRequests > 10) {
                refreshSupervisedRequests();
            }
        }

        VTPortal.roApp.clientTimeZone = moment.tz.guess(true);

        new WebServiceRobotics(function (result) {
        }, function (error) {
        }).updateServerTimeZone(VTPortal.roApp.clientTimeZone);
    }

    var refresh = function () {
        later(false);
        VTPortal.roApp.punchInProgress(false);
        punchesHome.viewShown();
        accrualsHome.refreshAcc();
        notificationsHome.viewShown();
        VTPortal.roApp.refreshEmployeeStatus(true, true);
        refreshSupervisedRequests();
        punchAllowedCalc();
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
            communiquesHome.viewShown();
        }
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userStatus().Telecommuting == true) {
            telecommutingHome.viewShown();
        }
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Surveys) {
            surveysHome.viewShown();
        }
        VTPortal.roApp.loadInitialData(false, true, false, false, false, false);
    }

    var proceedQuickPunch = function () {
        tcType(0);
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting) {
            if (VTPortal.roApp.userStatus().Telecommuting == true) {
                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                    $.when(punchAllowedCalc()).then(quickPunch());
                }
                else {
                    $.when(punchAllowedCalc()).then(optionsVisible(true));
                }
            }
            else {
                $.when(punchAllowedCalc()).then(quickPunch());
            }
        }
        else {
            $.when(punchAllowedCalc()).then(quickPunch());
        }
    }

    var registerToken = function (token) {
        new WebServiceRobotics(function (result) {
            if (result.oState.Result == window.VTPortalUtils.constants.OK.value) {
            } else {
            }
        }).registerFirebaseToken(token, VTPortal.roApp.db.settings.UUID);
    }

    var getToken = function () {
        FirebasePlugin.getToken(function (token) {
            VTPortal.roApp.FirebaseToken = token;
            registerToken(token);
        })
    };

    var onZoneClick = function (e) {
        VTPortal.roApp.popupZoneVisible(false);
        VTPortal.roApp.ZoneSelected(true);
        VTPortal.roApp.SelectedZone(e.itemData.Id);
        quickPunch();
    }

    var viewShown = function () {
        VTPortal.roApp.nfcEnabled(false);
        VTPortal.roApp.nfcPunchInProgress(false);

        setTimeout(function () {
            var now = new Date();
            var diffMs = (now - VTPortal.roApp.lastOkRefresh());
            var diffMins = Math.round(((diffMs % 86400000) % 3600000) / 60000);

            if (diffMins > 10 || VTPortal.roApp.documentSent() == true) {
                VTPortal.roApp.documentSent(false);
                VTPortal.roApp.refreshEmployeeStatus(true, true);
            }

            if (VTPortal.roApp.lastOkRequests() == null) {
                refreshSupervisedRequests();
            }

            else {
                var diffMsRequests = (now - VTPortal.roApp.lastOkRequests());
                var diffMinsRequests = Math.round(((diffMsRequests % 86400000) % 3600000) / 60000);

                if (diffMinsRequests > 10) {
                    refreshSupervisedRequests();
                }
            }

            var timeoutId = 0;

            $('#target').on('mousedown touchstart', function () {
                $("#btnActionMenuPopover2").addClass("pressing");
                progressVisible(true);
                timeoutId = setTimeout(proceedQuickPunch, 1500);
            }).on('mouseup mouseleave mouseout touchend', function () {
                $("#btnActionMenuPopover2").removeClass("pressing");
                progressVisible(false);
                clearTimeout(timeoutId);
            });

            later(false);
            globalStatus().viewShown();
            punchesHome.viewShown();
            accrualsHome.viewShown();
            emptyHome.viewShown();
            notificationsHome.viewShown();
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
                communiquesHome.viewShown();
            }
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined' && VTPortal.roApp.userStatus().Telecommuting == true) {
                telecommutingHome.viewShown();
            }

            punchAllowedCalc();
            if (licenseVisible) {
                bLicense(false);
            }
        }, 1000);

        if (VTPortal.roApp.isModeApp()) {
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Firebase) {
                try {
                    getToken();
                }
                catch (e) {
                }
            }
        }
    }

    if (typeof params.id == 'undefined') {
        window.VTPortalUtils.utils.loginIfNecessary('', viewShown);
    } else {
        switch (parseInt(params.id, 10)) {
            case 1:
                popupChangePwdVisible(true);
                break;
            case 2:
                popupValidationCodeVisible(true);
                break;
            case 3:
                VTPortal.roApp.impersonatedIDEmployee = -1;
                if (VTPortal.roApp.db.settings.onlySupervisor) {
                    VTPortal.roApp.redirectAtHome(false);
                } else {
                    VTPortal.roApp.redirectAtHome(true);
                }

                VTPortal.roApp.loadInitialData(true, true, true, false, false);
                break;
        }
    }

    var reloadMap = function (onEndCallback) {
        if (VTPortal.roApp.ZoneSelected()) {
            if (typeof onEndCallback != 'undefined')
                onEndCallback();
        }
        else {
            if (VTPortal.roApp.empPermissions().MustUseGPS) {
                progressLocVisible(true);
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
                                progressLocVisible(false);
                                onEndCallback();
                            });
                        }
                        else {
                            progressLocVisible(false);
                            onEndCallback();
                        }
                    },
                    function onError(error) {
                        progressLocVisible(false);
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
                        if (typeof onEndCallback != 'undefined')
                            onEndCallback();
                    }, { timeout: 10000 })
                    ;
            } else {
                progressLocVisible(false);
                if (typeof onEndCallback != 'undefined')
                    onEndCallback();
            }
        }
    }

    var commitPunch = function () {
        if (!VTPortal.roApp.lastRequestFailed()) {
            VTPortal.roApp.punchInProgress(true);
            if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'S', fast: true, tcType: tcType() });
            } else {
                window.VTPortalUtils.utils.doPunch('ta', { idCause: -1, direction: 'E', fast: true, tcType: tcType(), selectedZone: VTPortal.roApp.SelectedZone() });
            }
        } else {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roPunchOffline"), 'warning', 0);
        }
    }

    var commitPunchResponse = function (direction, bCommit) {
        if (direction == "S") {
            if (bCommit == true) {
                lblPunchDone(i18nextko.i18n.t("roQuickPunchOutDone"));
                quickPunchDone(true);
                punchesHome.viewShown();
            }
        } else {
            if (bCommit == true) {
                lblPunchDone(i18nextko.i18n.t("roQuickPunchInDone"));
                quickPunchDone(true);
                punchesHome.viewShown();
            }
        }
    }

    var quickPunch = function () {
        $("#btnActionMenuPopover2").removeClass("pressing");
        progressVisible(false);

        if (punchAllowedBool() == true) {
            if (VTPortal.roApp.empPermissions().MustUseGPS) {
                reloadMap(commitPunch);
            } else {
                reloadMap(commitPunch);
            }
        }
        else {
            punchNotAllowedVisible(true);
        }
    };

    var telecommutingOptions = ko.computed(function () {
        var ds = []

        ds.push({ text: i18nextko.i18n.t('roOffice'), type: 'success', cssClassText: 'textPunchOption', ImageSrc: "Images/icons8-office-48.png" });
        ds.push({ text: i18nextko.i18n.t('roHome'), type: 'success', cssClassText: 'textPunchOption', ImageSrc: "Images/icons8-home-48.png" });

        return ds;
    });

    var acceptQuick = function () {
        //punchesHome.viewShown();
        quickPunchDone(false);
    };
    var acceptQuickFailed = function () {
        $("#btnActionMenuPopover2").removeClass("pressing");
        progressVisible(false);
        quickPunchFailed(false);
    };
    var acceptPunch = function () {
        punchNotAllowedVisible(false);
    };

    var goCommuniques = function () {
        hasMandatoryCommuniques(false);
        hasMandatoryAnswer(false);
        if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
            VTPortal.roApp.selectedTab(5);
            VTPortal.app.navigate('communiques', { root: true });
        }
        else {
            VTPortal.roApp.selectedTab(4);
            VTPortal.app.navigate('communiques', { root: true });
        }
    }

    var goSurveys = function () {
        hasMandatorySurveys(false);
        later(true);
        VTPortal.app.navigate('surveys', { root: true });
    }

    var viewModel = {
        commitPunchResponse: commitPunchResponse,
        title: title,
        subscribeBlock: globalStatus(),
        summaryHome: summaryHome,
        notificationsHome: notificationsHome,
        punchesHome: punchesHome,
        accrualsHome: accrualsHome,
        emptyHome: emptyHome,
        communiquesHome: communiquesHome,
        surveysHome: surveysHome,
        telecommutingHome: telecommutingHome,
        popupChangePwdVisible: popupChangePwdVisible,
        lblOldPwd: i18nextko.t('lblOldPwd'),
        lblNewPwd: i18nextko.t('lblNewPwd'),
        lblPunchDone: lblPunchDone,
        lblPunchNotAllowed: i18nextko.t('ropunchNotAllowed'),
        lblNewPwdRepeat: i18nextko.t('lblNewPwdRepeat'),
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
                                    {
                                        VTPortal.roApp.loadZones();
                                        VTPortal.roApp.popupZoneVisible(true);
                                    }
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
                    quickPunch();
                }
            }
        },
        listZones: {
            dataSource: VTPortal.roApp.zonesDataSource,
            scrollingEnabled: true,
            height: 300,
            grouped: false,
            itemTemplate: 'RequestItem',
            /*onItemClick: function (e) {
                alert("doem")
            }*/
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
        doPunch: {
            onClick: function () { $.when(punchAllowedCalc()).then(doPunch()) },
            visible: ko.computed(function () { return VTPortal.roApp.impersonatedIDEmployee == -1 }),
            text: i18nextko.t('roPunchButton'),
        },
        dxholdHandler: function (viewModel, jQueryEvent) {
            $.when(punchAllowedCalc()).then(quickPunch());
        },
        quickPunchFailed: {
            onClick: function () {
                lblPunchDone(i18nextko.i18n.t("roHoldToPunch"));
                quickPunchFailed(true);
            },
            text: txtQuickPunch,
            visible: ko.computed(function () { return VTPortal.roApp.impersonatedIDEmployee == -1 }),
        },
        quickPunch: {
            onClick: function () {
                $("#btnActionMenuPopover2").removeClass("pressing");
                progressVisible(false);
                if (optionsVisible() == false) {
                    lblPunchDone(i18nextko.i18n.t("roHoldToPunch"));
                    quickPunchFailed(true);
                }
            },
            text: txtQuickPunch,
            visible: ko.computed(function () { return VTPortal.roApp.impersonatedIDEmployee == -1 }),
        },
        quickPunchCss: ko.computed(function () {
            if (!VTPortal.roApp.lastRequestFailed()) {
                if (VTPortal.roApp.userStatus() != null) {
                    if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                        return 'quickPunchCssOut'
                    } else {
                        return 'quickPunchCssIn'
                    }
                } else return 'quickPunchCssIn'
            } else {
                return 'quickPunchCssIn'
            }
        }),
        goBack: {
            onClick: goBack,
            text: i18nextko.t('lblGoBack'),
            visible: ko.computed(function () { return VTPortal.roApp.impersonatedIDEmployee != -1 })
        },
        punchNotAllowed: {
            text: txtpunchNotAllowed,
        },
        request: {
            onClick: newRequest,
            visible: ko.computed(function () { return VTPortal.roApp.impersonatedIDEmployee == -1 }),
            text: i18nextko.t('roRequestButton'),
        },
        txtOldPwd: {
            placeholder: i18nextko.t('roPH_oldPwd'),
            value: txtOldPwd,
            mode: 'password'
        },
        txtNewPwd: {
            placeholder: i18nextko.t('roPH_newPwd'),
            value: txtNewPwd,
            mode: 'password'
        },
        txtNewPwdRepeat: {
            placeholder: i18nextko.t('roPH_newPwdRepeat'),
            value: txtNewPwdRepeat,
            mode: 'password'
        },
        btnChangePwd: {
            validationGroup: 'changePwd',
            onClick: btnChangePwd,
            text: i18nextko.t('roChangePwd')
        },
        requieredOldField: {
            validationGroup: 'changePwd',
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        requieredNewField: {
            validationGroup: 'changePwd',
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        compareField: {
            validationGroup: 'changePwd',
            validationRules: [{
                type: 'compare', comparisonType: '==', comparisonTarget: txtNewPwd, message: i18nextko.i18n.t('roPasswordsNotMatch')
            }]
        },

        popupValidationCodeVisible: popupValidationCodeVisible,
        quickPunchDone: quickPunchDone,
        quickPunchFailed: quickPunchFailed,
        punchNotAllowedVisible: punchNotAllowedVisible,
        lblValidationCode: i18nextko.t('lblValidationCode'),
        txtValidationCode: {
            placeholder: i18nextko.t('roPH_ValidationCode'),
            value: txtValidationCode,
            mode: 'password'
        },
        requieredValidationField: {
            validationGroup: 'validationCode',
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        btnAcceptValidationCode: {
            validationGroup: 'validationCode',
            onClick: btnSendValidation,
            text: i18nextko.t('sendMsg')
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        progressPanel: {
            message: i18nextko.t('roKeepPushing'),
            showPane: true,
            height: 150,
            shading: false,
            delay: 200,
            shadingColor: 'rgba(0,0,0,0)',
            indicatorSrc: 'Images/Ripple1.gif',
            visible: progressVisible
        },
        progressPanelLoc: {
            message: i18nextko.t('roLocalizating'),
            showPane: true,
            shading: false,
            height: 150,
            shadingColor: 'rgba(0,0,0,0)',
            indicatorSrc: 'Images/Ripple1.gif',
            visible: progressLocVisible
        },
        licenceViewContent: {
            bounceEnabled: true,
            showScrollbar: 'always',
            height: 200
        },
        homeContent: {
            bounceEnabled: true,
            onPullDown: function (options) {
                refresh();
                options.component.release();
            }
        },
        lblTermsAndContidions: i18nextko.t('roTermsAndConditions'),
        lblWhereAreYou: i18nextko.t('lblWhereAreYou'),
        customConsent: customConsent,
        lbllicenseText: licenseConsent,
        lbllicenseAdv1: i18nextko.t('roAgreement1'),
        lbllicenseAdv2: i18nextko.t('roAgreement2'),
        lbllicenseAdv2Visibile: lbllicenseAdv2Visibile,
        lbllicenseAdv3: i18nextko.t('roAgreement3'),
        lbllicenseAdv3Visibile: lbllicenseAdv3Visibile,
        licenseVisible: licenseVisible,
        ListZonesVisible: VTPortal.roApp.popupZoneVisible,
        lblmandatoryCommuniques: i18nextko.t('roMandatoryCommuniques'),
        lblmandatorySurveys: i18nextko.t('roMandatorySurveys'),
        lblmandatoryAnswer: i18nextko.t('roMandatoryAnswer'),
        ApiVersion: ApiVersion,
        HasSurveys: HasSurveys,
        TelecommutingEnabled: TelecommutingEnabled,
        ckLicenceAccepted: {
            text: i18nextko.t('roLicenseAccept'),
            value: bLicense,
            disabled: licenseNotReaded
        },
        btnAcceptLicense: {
            onClick: btnAcceptLicense,
            text: i18nextko.t('roSaveLicense')
        },
        btnAccept: {
            onClick: acceptQuick,
            text: i18nextko.t('closeConfig'),
        },
        btnAcceptFailed: {
            onClick: acceptQuickFailed,
            text: i18nextko.t('closeConfig'),
        },
        btnAcceptPunch: {
            onClick: acceptPunch,
            text: i18nextko.t('closeConfig'),
        },
        btnMandatorySurvey: {
            onClick: goSurveys,
            text: i18nextko.t('roGoNow'),
        },
        btnMandatoryCommunique: {
            onClick: goCommuniques,
            text: i18nextko.t('roGoNow'),
        },
        btnAnswerLaterSurvey: {
            onClick: function () {
                later(true);
                hasMandatorySurveys(false);
            },
            text: i18nextko.t('roGoLater'),
        },
        btnAnswerLater: {
            onClick: function () {
                later(true);
                hasMandatoryAnswer(false);
            },
            text: i18nextko.t('roGoLater'),
        },
        actionMenuPopover: {
            target: ('#btnActionMenuPopover2'),
            shading: false,
            width: 300,
            shadingColor: "rgba(0, 0, 0, 0.1)",
            visible: VTPortal.roApp.showPopover
        },

        quickPunchHelp: i18nextko.t('roquickPunchHelp'),
        refresh: refresh,
        punchAllowedCalc: punchAllowedCalc,
        resume: resume,
        showPopupSurveys: showPopupSurveys,
        showPopupCommuniques: showPopupCommuniques,
        showPopupAnswer: showPopupAnswer,
        hasMandatoryCommuniques: hasMandatoryCommuniques,
        hasMandatorySurveys: hasMandatorySurveys,
        hasMandatoryAnswer: hasMandatoryAnswer,
        startProgressImage: function () {
            $("#btnActionMenuPopover2").addClass("pressing");
            progressVisible(true);
        },
        endProgressImage: function () {
            $("#btnActionMenuPopover2").removeClass("pressing");
            progressVisible(false);
        }
    };
    VTPortal.roApp.homeView = viewModel;
    return viewModel;
};