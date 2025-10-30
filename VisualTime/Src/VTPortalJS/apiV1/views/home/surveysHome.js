VTPortal.surveysHome = function (params) {
    var count = ko.observable(0);
    var countTotal = ko.computed(function () {
        return count()
    });

    var surveysDesc = ko.computed(function () {
        count(0);
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined' && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Surveys) {
            if (typeof VTPortal.roApp.userStatus().Surveys != 'undefined') {
                for (var i = 0; i < VTPortal.roApp.userStatus().Surveys.length; ++i) {
                    count(count() + 1);
                }
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
        VTPortal.app.navigate('surveys', { root: true });
    }
    var viewShown = function (e) {
        //VTPortal.roApp.refreshEmployeeStatus(false);
        surveysDesc();
    }
    var viewModel = {
        viewShown: viewShown,
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