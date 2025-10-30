VTPortal.scheduler = function (params) {
    var curSelectedView = ko.observable(1);
    var curSelectedIndex = ko.observable(0);

    if (typeof params.id != 'undefined') {
        curSelectedView(parseInt(params.id, 10));
        curSelectedIndex(parseInt(params.id, 10) - 1);
    }

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var viewShown = function () {
        globalStatus().viewShown();
    }

    var myCalendarBlock = VTPortal.myCalendar(),
        myAccrualsBlock = VTPortal.myAccruals(),
        myAuthorizationsBlock = VTPortal.myAuthorizations(),
        myRequestsBlock = VTPortal.myRequests();

    var scheduleTitle = ko.computed(function () {
        switch (curSelectedView()) {
            case 1:
                return i18nextko.i18n.t('roMyCalendarTitle');
                break;
            case 2:
                return i18nextko.i18n.t('roMyAccrualsTitle');
                break;
            case 3:
                return i18nextko.i18n.t('roMyAuthorizationsTitle');
                break;
            case 4:
                return i18nextko.i18n.t('roProfileRequests');
                break;
        }
    }, this);

    //Añadimos cada tab en función de los permisos del empleado
    var availableTabPanels = ko.observable([]);

    var tmpTabs = []
    tmpTabs.push({ "ID": 1, "cssClass": "dx-icon-myCalendar", "hint": i18nextko.i18n.t('roMyCalendarHint') });
    tmpTabs.push({ "ID": 2, "cssClass": "dx-icon-myAccruals", "hint": i18nextko.i18n.t('roMyAccrualsHint') });
    tmpTabs.push({ "ID": 3, "cssClass": "dx-icon-myAuthorizations", "hint": i18nextko.i18n.t('roMyAuthorizationsHint') });
    tmpTabs.push({ "ID": 4, "cssClass": "dx-icon-profileRequests", "hint": i18nextko.i18n.t('roMyRequestsHint') });

    availableTabPanels(tmpTabs);

    function setUpView() {
        var selDate = moment(params.param);

        myCalendarBlock.modelIsReady(false);
        myAccrualsBlock.modelIsReady(false);
        myAuthorizationsBlock.modelIsReady(false);
        myRequestsBlock.modelIsReady(false);

        switch (curSelectedView()) {
            case 1:
                myCalendarBlock.initializeView(selDate);
                break;
            case 2:
                myAccrualsBlock.initializeView(selDate);
                break;
            case 3:
                myAuthorizationsBlock.initializeView(selDate);
                break;
            case 4:
                myRequestsBlock.initializeView();
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
        myCalendarBlock: myCalendarBlock,
        myAccrualsBlock: myAccrualsBlock,
        myAuthorizationsBlock: myAuthorizationsBlock,
        myRequestsBlock: myRequestsBlock,
        selectedView: curSelectedView,
        selectedIndex: curSelectedIndex,
        tabPanelOptions: {
            dataSource: availableTabPanels,
            selectedIndex: curSelectedIndex,
            itemTemplate: "title",
            onSelectionChanged: function (e) {
                changeTab(e.addedItems[0]);
            }
        },
    };

    return viewModel;
};