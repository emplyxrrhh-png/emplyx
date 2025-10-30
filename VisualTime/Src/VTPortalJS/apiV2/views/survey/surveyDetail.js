VTPortal.surveyDetail = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var surveyModel = ko.observable();

    var getSurvey = function () {
        new WebServiceRobotics(function (result) {
            if (result.oState.Result == window.VTPortalUtils.constants.OK.value) {
                let surveyContent = JSON.parse(result.Survey.Content);

                surveyContent.pages.forEach(page => {
                    page.elements.forEach(element => {
                        if (element.type === "dropdown") {
                            element.renderAs = "select";
                        }
                    });
                });

                const newLocale = surveyContent.locale;
                surveyModel(result);
                let survey = new Survey.Model(surveyContent);
                Survey.StylesManager.applyTheme("orange");
                survey.locale = newLocale || 'es';
                survey.onComplete.add(sendDataToServer);
                $("#surveyContainer").Survey({ model: survey });
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
                    VTPortal.roApp.db.settings.markForRefresh(['surveys', "status"]);
                    VTPortal.app.navigate('status', { root: true });
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