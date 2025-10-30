VTPortal.home = function (params) {
    //Per accedir al punchInfo tcType, optionsVisible

    var punchInfoBlock = VTPortal.punchInfo();

    var selectedDate = ko.observable(new Date());
    var summaryHome = VTPortal.summaryHome(),
        punchesHome = VTPortal.punchesHome(),
        emptyHome = VTPortal.emptyHome(),
        telecommutingHome = VTPortal.telecommutingHome(),
        webLinksHome = VTPortal.webLinksHome();

    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var progressVisible = ko.observable(false);
    var progressLocVisible = ko.observable(false);
    var licenseNotReaded = ko.observable(false);
    var popupChangePwdVisible = ko.observable(false), popupValidationCodeVisible = ko.observable(false);
    var quickPunchDone = ko.observable(false);
    var quickPunchFailed = ko.observable(false);
    var punchNotAllowedVisible = ko.observable(false);
    var lblPunchDone = ko.observable(false);
    var punchAllowedBool = ko.observable(false);
    var punchAllowedBoolInterface = ko.observable(false);
    var txtOldPwd = ko.observable(''), txtNewPwd = ko.observable(''), txtNewPwdRepeat = ko.observable(''), txtValidationCode = ko.observable('');
    var bLicense = ko.observable(false);
    var ApiVersion = ko.computed(function () {
        return VTPortal.roApp.db.settings.ApiVersion;
    });


    var validationDesc = ko.observable("");
    var validationCodeComputed = ko.computed(function () {
        return validationDesc();// '. ' + i18nextko.i18n.t('lblValidationCode');
    });

    var TelecommutingEnabled = ko.computed(function () {
        if (VTPortal.roApp.userTelecommute() != null && typeof VTPortal.roApp.userTelecommute() != 'undefined') {
            var tcEnabled = VTPortal.roApp.userTelecommute().Telecommuting;
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
                    VTPortal.roApp.loggedIn = true;
                    VTPortal.roApp.db.settings.rememberLogin = true;
                    if (VTPortal.roApp.isModeApp()) VTPortal.roApp.db.settings.rememberLoginApp = txtNewPwd().trim();
                    else VTPortal.roApp.db.settings.password = '';

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
                            VTPortal.roApp.loadInitialData(true, true, true, true, true, function () { VTPortal.roApp.redirectAtHome(false); });
                        } else {
                            VTPortal.roApp.loadInitialData(true, true, true, true, true, function () { VTPortal.roApp.redirectAtHome(true); });
                        }
                    });
                } else {
                    VTPortal.roApp.loggedIn = false;
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

            //i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status))

            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    if (VTPortal.roApp.db.settings.onlySupervisor) {
                            VTPortal.roApp.redirectAtHome(false);
                        } else {
                            VTPortal.roApp.redirectAtHome(true);
                        }
                } else {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(-65)), 'error', 0, function () { VTPortalUtils.utils.onLogoutErrorFunc(result); });
                    
                }
            }).validateSession(txtValidationCode());
        }
    }

    var title = ko.computed(function () {
        return i18nextko.i18n.t('roHomeTitle');
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

    var goDailyRecord = function () {
        VTPortal.app.navigate('myDailyRecord');
    }

    var goBack = function () {
        let onBack = function () {
            if (VTPortal.roApp.db.settings.onlySupervisor) {
                VTPortal.roApp.redirectAtHome(false);
            } else {
                window.VTPortalUtils.utils.setActiveTab('myTeam');
                VTPortal.app.navigate("myteam", { root: true });
            }
            setTimeout(function () {
                VTPortalUtils.utils.buildHamburgerMenu();
            }, 1000);
        }

        VTPortal.roApp.impersonatedIDEmployee = -1;
        VTPortal.roApp.loadInitialData(true, true, true, false, false,onBack);


        

    }

    var refreshModules = function () {
        if (VTPortal.roApp.loggedIn) {
            emptyHome.viewShown();
            punchesHome.viewShown();
            webLinksHome.viewShown();
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userTelecommute() != null && VTPortal.roApp.userTelecommute().Telecommuting) telecommutingHome.viewShown();
            VTPortal.roApp.refreshEmployeeStatus(true);
        }
    }


    var resume = function () {
        VTPortal.roApp.clientTimeZone = moment.tz.guess(true);
        VTPortal.roApp.punchInProgress(false);
        refreshModules();

        new WebServiceRobotics(function (result) { }, function (error) { }).updateServerTimeZone(VTPortal.roApp.clientTimeZone);
    }

    var refresh = function () {
        VTPortal.roApp.db.settings.resetFastPunchStatusObject();
        VTPortal.roApp.punchInProgress(false);
        refreshModules();
        punchAllowedCalc();
    }

    var proceedQuickPunch = function () {
        punchInfoBlock.TcType(0);
        $.when(punchAllowedCalc()).then(punchInfoBlock.CommitQuickPunch());
    }

    var registerToken = function (token) {
        if (VTPortal.roApp.isTimeGate()) return;

        new WebServiceRobotics(function (result) {}).registerFirebaseToken(token, VTPortal.roApp.db.settings.UUID);
    }

    var getToken = function () {
        if (VTPortal.roApp.isTimeGate()) return;

        FirebasePlugin.getToken(function (token) {
            VTPortal.roApp.FirebaseToken = token;
            registerToken(token);
        })
    };

    var quickPunchPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() == null || VTPortal.roApp.userStatus() == null) {
            return false;
        }
        else {
            return ((VTPortal.roApp.empPermissions().Punch.SchedulePunch && VTPortal.roApp.userStatus().PunchesEnabled));
        }
    });

    var dailyRecordPermission = ko.computed(function () {

        if (VTPortal.roApp.empPermissions() == null || VTPortal.roApp.userStatus() == null) {
            return false;
        }
        else {
            return ((!quickPunchPermission()) && VTPortal.roApp.DailyRecordEnabled());
        }
    });

    var viewShown = function () {
        var quickPunch = function (onContinue) {
            $("#btnActionMenuPopover2").removeClass("pressing");
            progressVisible(false);

            if (punchAllowedBool() == true) {
                VTPortal.roApp.punchInProgress(true);
                if (typeof onContinue != 'undefined') onContinue();
            }
            else punchNotAllowedVisible(true);
        };

        punchInfoBlock.InitView(quickPunch, progressLocVisible);
        VTPortal.roApp.nfcEnabled(false);
        VTPortal.roApp.nfcPunchInProgress(false);

        setTimeout(function () {

            if (VTPortal.roApp.documentSent() == true) {
                VTPortal.roApp.documentSent(false);
                VTPortal.roApp.refreshEmployeeStatus(true);
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

            globalStatus().viewShown();
            refreshModules();
            punchAllowedCalc();
            if (licenseVisible) bLicense(false);
            window.VTPortalUtils.utils.setActiveTab('home');
        }, 1000);

        if (VTPortal.roApp.isModeApp() && !VTPortal.roApp.isTimeGate()) {
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Firebase && VTPortal.roApp.impersonatedIDEmployee == -1) {
                try {
                    getToken();
                }
                catch (e) {
                }
            }
        }

        if (VTPortal.roApp.showLegalText() == "1" && localStorage.getItem("ShowLegalText.VTPortal") != "0") {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roENSShowLegalText'), 'info', 0, function () {
                localStorage.setItem("ShowLegalText.VTPortal", "0");
            });
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
                validationDesc(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(-63)));
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

    var commitPunchResponse = function (direction, bCommit) {
        if (direction == "S") {
            if (bCommit) {
                lblPunchDone(i18nextko.i18n.t("roQuickPunchOutDone"));
                quickPunchDone(true);
                punchesHome.viewShown();
            }
        } else {
            if (bCommit) {
                lblPunchDone(i18nextko.i18n.t("roQuickPunchInDone"));
                quickPunchDone(true);
                punchesHome.viewShown();
            }
        }
    }

    var acceptQuick = function () {
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

    var viewModel = {
        myPunchInfoBlock: punchInfoBlock,
        commitPunchResponse: commitPunchResponse,
        title: title,
        subscribeBlock: globalStatus(),
        summaryHome: summaryHome,
        punchesHome: punchesHome,
        emptyHome: emptyHome,
        telecommutingHome: telecommutingHome,
        webLinksHome: webLinksHome,
        popupChangePwdVisible: popupChangePwdVisible,
        lblOldPwd: i18nextko.t('lblOldPwd'),
        lblNewPwd: i18nextko.t('lblNewPwd'),
        lblPunchDone: lblPunchDone,
        lblPunchNotAllowed: i18nextko.t('ropunchNotAllowed'),
        lblNewPwdRepeat: i18nextko.t('lblNewPwdRepeat'),
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
                quickPunchFailed(false);
            },
            text: txtQuickPunch,
            visible: ko.computed(function () { return VTPortal.roApp.impersonatedIDEmployee == -1 }),
        },
        quickPunch: {
            onClick: function () {
                $("#btnActionMenuPopover2").removeClass("pressing");
                progressVisible(false);
                if (punchInfoBlock.OptionsVisible() == false) {
                    lblPunchDone(i18nextko.i18n.t("roHoldToPunch"));
                    quickPunchFailed(false);
                }
            },
            text: txtQuickPunch,
            visible: ko.computed(function () { return quickPunchPermission() && VTPortal.roApp.impersonatedIDEmployee == -1 }),
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
        dailyRecordPermission: dailyRecordPermission,
        dailyRecordBtn: {
            onClick: goDailyRecord,
            text: i18nextko.t('lblDailyRecordButton'),
        },
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
        lblValidationCode: validationCodeComputed,
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
        customConsent: customConsent,
        lbllicenseText: licenseConsent,
        lbllicenseAdv1: i18nextko.t('roAgreement1'),
        lbllicenseAdv2: i18nextko.t('roAgreement2'),
        lbllicenseAdv2Visibile: lbllicenseAdv2Visibile,
        lbllicenseAdv3: i18nextko.t('roAgreement3'),
        lbllicenseAdv3Visibile: lbllicenseAdv3Visibile,
        licenseVisible: licenseVisible,
        lblmandatoryCommuniques: i18nextko.t('roMandatoryCommuniques'),
        lblmandatorySurveys: i18nextko.t('roMandatorySurveys'),
        lblmandatoryAnswer: i18nextko.t('roMandatoryAnswer'),
        ApiVersion: ApiVersion,
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
        startProgressImage: function () {
            $("#btnActionMenuPopover2").addClass("pressing");
            progressVisible(true);
        },
        endProgressImage: function () {
            $("#btnActionMenuPopover2").removeClass("pressing");
            progressVisible(false);
        },
        refreshModules: refreshModules
    };
    VTPortal.roApp.homeView = viewModel;

    return viewModel;
};