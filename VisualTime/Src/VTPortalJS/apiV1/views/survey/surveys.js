VTPortal.surveys = function (params) {
    var surveyDS = ko.observable(false);

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        searchOperation: "contains",
        searchExpr: ["Subject", "Message", "SentOn", "CreatedBy"]
    }));

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function generatesurveysAll() {
        var lstsurveys = [];
        var IsAnonymous = "";
        var ExpiratesOn = "";

        var surveys = VTPortal.roApp.userStatus().Surveys;
        if (surveys != null && surveys.length > 0) {
            var cssClass = '';
            var cssClassText = '';

            for (var i = 0; i < surveys.length; i++) {
                var sDate = moment();
                var lpDate = moment(surveys[i].ExpirationDate);
                var totalDays = lpDate.diff(sDate, 'days');

                if (surveys[i].IsMandatory == true) {
                    cssClass = 'dx-icon-surveyImportant';
                    cssClassText = 'textCommuniqueNotRead';
                }
                else {
                    cssClass = 'dx-icon-surveyNotRead';
                    cssClassText = 'textCommuniqueNotRead';
                }
                if (surveys[i].Anonymous == true) {
                    IsAnonymous = i18nextko.i18n.t("roIsAnonymous");
                }
                else {
                    IsAnonymous = "";
                }
                name = surveys[i].Title;

                lstsurveys.push({
                    ID: surveys[i].Id,
                    cssClass: cssClass,
                    cssClassText: cssClassText,
                    Content: surveys[i].Content,
                    SentOn: moment(surveys[i].SentOn).format("DD/MM/YYYY"),
                    Subject: surveys[i].Title,
                    CreatedBy: surveys[i].CreatedByName,
                    IsAnonymous: IsAnonymous,
                    ExpiresOn: i18nextko.i18n.t("roExpiresOn") + totalDays + i18nextko.i18n.t("roExpiresOnDays")
                });
            }
        }
        if (lstsurveys.length > 0) {
            surveyDS(true);
        }
        else {
            surveyDS(false);
        }
        dataSource(new DevExpress.data.DataSource({
            store: lstsurveys,
            searchOperation: "contains",
            searchExpr: ["Subject", "Message", "SentOn", "CreatedBy"]
        }));
        return lstsurveys;
    };

    function viewShown() {
        globalStatus().viewShown();
        generatesurveysAll();
    };

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.t('rosurveysTitle'),
        subscribeBlock: globalStatus(),
        surveyDS: surveyDS,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        commScroll: {
            height: '70%',
            onPullDown: function (options) {
                generatesurveysAll();
            }
        },
        listsurveys: {
            dataSource: dataSource,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'surveyItem',
            onItemClick: function (info) {
                VTPortal.app.navigate('surveyDetail/' + info.itemData.ID);
            }
        },
        searchOptions: {
            valueChangeEvent: "keyup",
            mode: "search",
            height: "10%",
            onValueChanged: function (args) {
                dataSource().searchValue(args.value);
                dataSource().load();
            }
        },

        allComm: {
            onClick: generatesurveysAll,
            text: i18nextko.t('roAllComm')
        },
    };

    return viewModel;
};