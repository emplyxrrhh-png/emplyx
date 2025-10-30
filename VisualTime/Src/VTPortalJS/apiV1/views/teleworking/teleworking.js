VTPortal.teleworking = function (params) {
    var curSelectedView = ko.observable(1);
    var curSelectedIndex = ko.observable(0);

    if (typeof params.id != 'undefined') {
        curSelectedView(parseInt(params.id, 10));
        curSelectedIndex(parseInt(params.id, 10) - 1);
    }

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
    }
    var myWorkBlock = VTPortal.myWork(),
        myWorkDailyBlock = VTPortal.myWorkDaily(),
        myWorkRequestsBlock = VTPortal.myWorkRequests();

    var scheduleTitle = ko.computed(function () {
        switch (curSelectedView()) {
            case 1:
                return i18nextko.i18n.t('roMyWorkTitle');
                break;
            case 2:
                return i18nextko.i18n.t('roMyWorkDaily');
                break;
            case 3:
                return i18nextko.i18n.t('roMyWorkRequests');
                break;
        }
    }, this);

    //Añadimos cada tab en función de los permisos del empleado
    var availableTabPanels = ko.observable([]);

    var tmpTabs = []
    tmpTabs.push({ "ID": 1, "cssClass": "dx-icon-myWork" });
    tmpTabs.push({ "ID": 2, "cssClass": "dx-icon-myWorkDaily" });
    tmpTabs.push({ "ID": 3, "cssClass": "dx-icon-myWorkRequests" });

    availableTabPanels(tmpTabs);

    function setUpView() {
        var selDate = moment(params.param);

        myWorkBlock.modelIsReady(false);
        myWorkDailyBlock.modelIsReady(false);
        myWorkRequestsBlock.modelIsReady(false);

        switch (curSelectedView()) {
            case 1:
                myWorkBlock.initializeView(selDate);
                break;
            case 2:
                myWorkDailyBlock.initializeView(selDate);
                break;
            case 3:
                myWorkRequestsBlock.initializeView(selDate);
                break;
        }
    };

    var changeTab = function (item) {
        curSelectedView(item.ID);
        setUpView();
        curSelectedIndex(item.ID - 1);
    }

    var viewModel = {
        viewShown: viewShown,
        title: scheduleTitle,
        subscribeBlock: globalStatus(),
        myWorkBlock: myWorkBlock,
        myWorkDailyBlock: myWorkDailyBlock,
        myWorkRequestsBlock: myWorkRequestsBlock,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        selectedView: curSelectedView,
        selectedIndex: curSelectedIndex,
        tabPanelOptions: {
            dataSource: availableTabPanels,
            selectedIndex: curSelectedIndex,
            itemTemplate: "title",
            onSelectionChanged: function (e) {
                changeTab(e.addedItems[0]);
            }
        }
    };

    return viewModel;
};