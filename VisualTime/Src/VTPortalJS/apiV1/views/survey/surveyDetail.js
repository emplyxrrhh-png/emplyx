VTPortal.surveyDetail = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var surveyModel = ko.observable();

    var getSurvey = function () {
        new WebServiceRobotics(function (result) {
            if (result.oState.Result == window.VTPortalUtils.constants.OK.value) {
                surveyModel(result);
                Survey.StylesManager.applyTheme("orange");
                var survey = new Survey.Model(result.Survey.Content, "surveyContainer");
                survey.locale = "es";
                survey.onComplete.add(sendDataToServer);
            } else {
                DevExpress.ui.notify(i18nextko.t('roSurveyError'), 'warning', 3000);
                VTPortal.app.navigate('surveys', { root: true });
            }
        }).getSurveyById(params.id);
    }

    function sendDataToServer(e) {
        new WebServiceRobotics(function (result) {
            if (result.oState.Result == window.VTPortalUtils.constants.OK.value) {
                setTimeout(function () {
                    VTPortal.roApp.refreshEmployeeStatus(false, false);
                    VTPortal.app.navigate('home', { root: true });
                }, 2000);
            } else {
                setTimeout(function () {
                    DevExpress.ui.notify(i18nextko.t('roSurveySaveError'), 'warning', 3000);
                    VTPortal.app.navigate('surveys', { root: true });
                }, 2000);
            }
        }).sendSurveyResponse(params.id, VTPortal.roApp.employeeId, e.data);
    }

    function viewShown() {
        globalStatus().viewShown();
        getSurvey();
    };

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roHomeSurvey'),
        subscribeBlock: globalStatus(),
        surveyModel: surveyModel,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        commScroll: {
            height: '75%',
            onPullDown: function (options) {
            }
        },
    };

    return viewModel;
};