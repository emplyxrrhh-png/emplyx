VTPortal.status = function (params) {
    //Per accedir al punchInfo tcType, optionsVisible

    var selectedDate = ko.observable(new Date());

    var summaryHome = VTPortal.summaryHome(),
        notificationsHome = VTPortal.notificationsHome(),
        accrualsHome = VTPortal.accrualsHome(),
        emptyHome = VTPortal.emptyHome(),
        surveysHome = VTPortal.surveysHome(),
        dailyRecordHome = VTPortal.dailyRecordHome(),
        communiquesHome = VTPortal.communiquesHome(),
        channelsHome = VTPortal.channelsHome();
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var hasMandatoryCommuniques = ko.observable(false);
    var hasMandatorySurveys = ko.observable(false);
    var hasMandatoryAnswer = ko.observable(false);
    var showPopupAnswer = ko.observable(false);
    var showPopupCommuniques = ko.observable(false);
    var showPopupSurveys = ko.observable(false);
    var later = ko.observable(false);
    var ApiVersion = ko.computed(function () {
        return VTPortal.roApp.db.settings.ApiVersion;
    });

    var HasSurveys = ko.computed(function () {
        if (VTPortal.roApp.userSurveys() != null && typeof VTPortal.roApp.userSurveys() != 'undefined') {
            var surveys = VTPortal.roApp.userSurveys();
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

    var DailyRecordHomeEnabled = ko.computed(function () {
        if (VTPortal.roApp.DailyRecordEnabled()) {
            return true;
        }
        else {
            return false;
        }
    });

    var showMandatoryPopups = function () {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
            if (VTPortal.roApp.userCommuniques() != null) {
                for (var i = 0; i < VTPortal.roApp.userCommuniques().length; ++i) {
                    if (VTPortal.roApp.userCommuniques()[i].EmployeeCommuniqueStatus[0].AnswerRequired == true && VTPortal.roApp.userCommuniques()[i].EmployeeCommuniqueStatus[0].Answer == "" && VTPortal.roApp.userCommuniques()[i].Communique.MandatoryRead == true && VTPortal.roApp.userCommuniques()[i].Communique.Archived == false) {
                        if (later() == false) {
                            hasMandatoryAnswer(true);
                        }
                    }
                }

                for (var i = 0; i < VTPortal.roApp.userCommuniques().length; ++i) {
                    if (VTPortal.roApp.userCommuniques()[i].EmployeeCommuniqueStatus[0].Read == false && VTPortal.roApp.userCommuniques()[i].Communique.MandatoryRead == true && VTPortal.roApp.userCommuniques()[i].Communique.Archived == false && VTPortal.roApp.userCommuniques()[i].EmployeeCommuniqueStatus[0].AnswerRequired == false) {
                        if (later() == false) {
                            hasMandatoryCommuniques(true);
                        }
                    }
                }
            }
        }

        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Surveys) {
            if (VTPortal.roApp.userSurveys() != null && typeof VTPortal.roApp.userSurveys() != 'undefined') {
                if (typeof VTPortal.roApp.userSurveys() != 'undefined') {
                    for (var i = 0; i < VTPortal.roApp.userSurveys().length; ++i) {
                        if (VTPortal.roApp.userSurveys()[i].IsMandatory == true) {
                            if (later() == false && VTPortal.roApp.stopShowingSurveysPopup() == false) {
                                VTPortal.roApp.stopShowingSurveysPopup(true);
                                hasMandatorySurveys(true);
                            }
                        }
                    }
                }
            }
        }
    }

    var showPopups = ko.computed(function () {
        //TODO: Revisar si existirán comunicados obligatorios en channels, para añadir este popup
        // Revisar si el cliente ha utilizado comunicados y tiene modulo activo(pbi: 1190266)
        if (hasMandatoryAnswer() == true && hasMandatoryCommuniques() == true && VTPortal.roApp.isCegidHub() == false && VTPortal.roApp.isCommuniquesEnabled() == true) {
            showPopupAnswer(true);
            showPopupCommuniques(false);
        }
        else if (hasMandatoryAnswer() == false && hasMandatoryCommuniques() == true && VTPortal.roApp.isCegidHub() == false && VTPortal.roApp.isCommuniquesEnabled() == true) {
            showPopupAnswer(false);
            showPopupCommuniques(true);
        }
        else if (hasMandatoryAnswer() == true && hasMandatoryCommuniques() == false && VTPortal.roApp.isCegidHub() == false && VTPortal.roApp.isCommuniquesEnabled() == true) {
            showPopupAnswer(true);
            showPopupCommuniques(false);
        }
        else {
            showPopupAnswer(false);
            showPopupCommuniques(false);
        }

        if (hasMandatorySurveys() == true && VTPortal.roApp.isCegidHub() == false) {
            showPopupSurveys(true);
        }
        else {
            showPopupSurveys(false);
        }
       
    });

    var title = ko.computed(function () {
        return i18nextko.i18n.t('roHomeTitle');
    });

    var refreshModules = function () {
        if (VTPortal.roApp.loggedIn) {
            emptyHome.viewShown();
            accrualsHome.viewShown();
            notificationsHome.viewShown();

            if (VTPortal.roApp.DailyRecordEnabled()) dailyRecordHome.viewShown();
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) communiquesHome.viewShown();
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) channelsHome.viewShown();
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Surveys) surveysHome.viewShown();

            VTPortal.roApp.refreshEmployeeStatus();
            VTPortal.roApp.refreshEmployeeAlerts();
        }
    }


    var refresh = function () {
        VTPortal.roApp.db.settings.resetAlertsStatusObject();

        later(false);
        VTPortal.roApp.punchInProgress(false);

        refreshModules();
    }

    var viewShown = function () {
        window.VTPortalUtils.utils.setActiveTab('status');
        setTimeout(function () {
            later(false);
            globalStatus().viewShown();
            refreshModules();
        }, 1000);

    }

    var goCommuniques = function () {
        hasMandatoryCommuniques(false);
        hasMandatoryAnswer(false);

        window.VTPortalUtils.utils.setActiveTab('communiques');
        VTPortal.app.navigate('communiques', { root: true });
    }

    var goSurveys = function () {
        hasMandatorySurveys(false);
        later(true);
        VTPortal.app.navigate('surveys', { root: true });
    }

    var viewModel = {
        viewShown: viewShown,
        title: title,
        subscribeBlock: globalStatus(),
        summaryHome: summaryHome,
        notificationsHome: notificationsHome,
        accrualsHome: accrualsHome,
        emptyHome: emptyHome,
        communiquesHome: communiquesHome,
        channelsHome: channelsHome,
        surveysHome: surveysHome,
        dailyRecordHome: dailyRecordHome,
        dxholdHandler: function (viewModel, jQueryEvent) {
            $.when(punchAllowedCalc()).then(quickPunch());
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        homeContent: {
            bounceEnabled: true,
            onPullDown: function (options) {
                refresh();
                options.component.release();
            }
        },
        lblmandatoryCommuniques: i18nextko.t('roMandatoryCommuniques'),
        lblmandatorySurveys: i18nextko.t('roMandatorySurveys'),
        lblmandatoryAnswer: i18nextko.t('roMandatoryAnswer'),
        ApiVersion: ApiVersion,
        HasSurveys: HasSurveys,
        DailyRecordHomeEnabled: DailyRecordHomeEnabled,
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
        refresh: refresh,
        showPopupSurveys: showPopupSurveys,
        showPopupCommuniques: showPopupCommuniques,
        showPopupAnswer: showPopupAnswer,
        hasMandatoryCommuniques: hasMandatoryCommuniques,
        hasMandatorySurveys: hasMandatorySurveys,
        hasMandatoryAnswer: hasMandatoryAnswer,
        showMandatoryPopups: showMandatoryPopups,
        refreshModules: refreshModules
    };
    VTPortal.roApp.statusView = viewModel;
    return viewModel;
};