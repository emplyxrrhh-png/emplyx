VTPortal.telecommutingSelector = function (params) {
    var telecommutingSelectorDS = ko.observable(false);
    var tcTitle = ko.observable('');

    var dataSource = ko.observable(new DevExpress.data.DataSource({
    }));

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function generatetelecommutingSelectorAll() {
        var lsttelecommutingSelector = [];

        lsttelecommutingSelector.push({
            ID: 0,
            cssClass: 'dx-icon-taTelecommuting_Office',
            cssClassText: 'textActionPunch',
            Content: i18nextko.i18n.t('roOffice')
        });

        lsttelecommutingSelector.push({
            ID: 1,
            cssClass: 'dx-icon-taTelecommuting_Home',
            cssClassText: 'textActionPunch',
            Content: i18nextko.i18n.t('roHome')
        });

        if (lsttelecommutingSelector.length > 0) {
            telecommutingSelectorDS(true);
        }
        else {
            telecommutingSelectorDS(false);
        }
        dataSource(new DevExpress.data.DataSource({
            store: lsttelecommutingSelector
        }));
        return lsttelecommutingSelector;
    };

    function viewShown() {
        globalStatus().viewShown();
        generatetelecommutingSelectorAll();

        if (params.param == "0") {
            tcTitle(i18nextko.i18n.t('roTCWhere'));
        }
        else {
            tcTitle(i18nextko.i18n.t('roTCWhereF'));
        }
    };

    var viewModel = {
        viewShown: viewShown,
        title: tcTitle,
        subscribeBlock: globalStatus(),
        telecommutingSelectorDS: telecommutingSelectorDS,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        commScroll: {
            height: '70%',
            onPullDown: function (options) {
                generatetelecommutingSelectorAll();
            }
        },
        listtelecommutingSelector: {
            dataSource: dataSource,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'actionItem',
            onItemClick: function (info) {
                if (params.param == "0") {
                    VTPortal.app.navigate('punches/' + info.itemData.ID);
                }
                else {
                    VTPortal.app.navigate('forgottenPunch/-1/' + moment(params.iDate).format("YYYY-MM-DD") + '/' + info.itemData.ID);
                }
            }
        },
    };

    return viewModel;
};