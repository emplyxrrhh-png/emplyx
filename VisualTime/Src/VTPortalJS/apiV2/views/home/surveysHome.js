VTPortal.surveysHome = function (params) {
    var count = ko.observable(0);
    var countTotal = ko.computed(function () {
        return count()
    });

    var surveysDesc = ko.computed(function () {
        count(0);
        if (VTPortal.roApp.userSurveys() != null && typeof VTPortal.roApp.userSurveys() != 'undefined' && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Surveys) {
            if (typeof VTPortal.roApp.userSurveys != 'undefined') {
                count(VTPortal.roApp.userSurveys().length);
            }
            if (count() == 0) {
                VTPortal.roApp.hasSurveys(false);
                return i18nextko.i18n.t('nosurveys');
            }
            else {
                VTPortal.roApp.hasSurveys(true);
                return i18nextko.i18n.t('newsurveys') + count() + ' ' + i18nextko.i18n.t('newsurveys2');
            }
        } else return '';
    });

    var surveysTitle = ko.computed(function () {
        return i18nextko.i18n.t('rosurveysTitle');
    });

    function goToSurveys() {
        window.VTPortalUtils.utils.setActiveTab('surveys');
        VTPortal.app.navigate('surveys', { root: true });
    }
    var surveysViewShown = function (e) {
        let surveyView = this;
        let onViewShownCallback = function () {
            surveyView.lblSurveysDesc();

            if (VTPortal.roApp.userSurveys() != null && VTPortal.roApp.userSurveys().length > 0) {
                if (VTPortal.roApp.statusView!= null) VTPortal.roApp.statusView.showMandatoryPopups();
            }
        }

        let onLoadCallback = function (callback) {
            return function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.db.settings.updateCacheDS('surveys', result.Surveys);
                    VTPortal.roApp.userSurveys(result.Surveys);
                } else {
                    VTPortalUtils.utils.processErrorMessage(result);
                }

                if (typeof (callback == 'function')) callback();
            }
        }

        VTPortal.roApp.userSurveys(VTPortal.roApp.db.settings.getCacheDS('surveys'));
        if (VTPortal.roApp.userSurveys() == null || window.VTPortalUtils.needToRefresh('surveys')) {
            new WebServiceRobotics(onLoadCallback(onViewShownCallback), function () { }).getEmployeeSurveys();
        } else {
            onViewShownCallback();
        }
    }
    var viewModel = {
        viewShown: surveysViewShown,
        goToSurveys: goToSurveys,
        surveysTitle: surveysTitle,
        lblSurveysDesc: surveysDesc,
        btnGo: {
            onClick: goToSurveys,
            text: countTotal,
            visible: countTotal,
        },

        iconSurveys: {
            onClick: goToSurveys,
            icon: 'tips'
        },
        hasSurveys: ko.computed(function () {
            if (VTPortal.roApp.hasSurveys() == true) {
                return 'hasSurveys';
            }
            else {
                return 'hasSurveysNot';
            }
        }),

        hasSurveysTitle: ko.computed(function () {
            if (VTPortal.roApp.hasSurveys() == true) {
                return 'mainTitle';
            }
            else {
                return 'mainTitle';
            }
        }),
    };

    return viewModel;
};