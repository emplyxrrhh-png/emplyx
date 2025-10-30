VTPortal.leaves = function (params) {
    var curSelectedView = ko.observable(1);
    var curSelectedIndex = ko.observable(0);

    if (typeof params.id != 'undefined') {
        curSelectedView(parseInt(params.id, 10));
        curSelectedIndex(parseInt(params.id, 10) - 1);
    }

    var myLeavesBlock = VTPortal.myLeaves(),
        myLeavesAlertsBlock = VTPortal.myLeavesAlerts();

    var scheduleTitle = ko.computed(function () {
        switch (curSelectedView()) {
            case 1:
                return i18nextko.i18n.t('roMyLeavesTitle');
                break;
            case 2:
                return i18nextko.i18n.t('roMyLeavesAletsTitle');
                break;
        }
    }, this);

    //Añadimos cada tab en función de los permisos del empleado
    var availableTabPanels = ko.observable([]);

    var alertcount = ko.computed(function () {
        if (typeof VTPortal.roApp.userStatus().ScheduleStatus.TrackingDocuments != 'undefined') {
            var alerts = VTPortal.roApp.userStatus().ScheduleStatus;

            var alertCount = 0;
            for (var i = 0; i < alerts.TrackingDocuments.length; i++) {
                if (alerts.TrackingDocuments[i].IdDaysAbsence > 0 && alerts.TrackingDocuments[i].Scope == 3) {
                    alertCount++;
                }
            }
            return alertCount;
        } else {
            return 0;
        }
    });

    var tmpTabs = []
    tmpTabs.push({ "ID": 1, "cssClass": "dx-icon-myLeaves" });
    tmpTabs.push({ "ID": 2, "cssClass": "dx-icon-myLeavesAlerts", badge: alertcount() });

    availableTabPanels(tmpTabs);
    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function setUpView() {
        var selDate = moment(params.param);

        myLeavesBlock.modelIsReady(false);
        myLeavesAlertsBlock.modelIsReady(false);

        switch (curSelectedView()) {
            case 1:
                myLeavesBlock.viewShown(selDate);
                break;
            case 2:
                myLeavesAlertsBlock.viewShown(selDate);
                break;
        }
    };

    function viewShown() {
        globalStatus().viewShown();
        setUpView();
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
        myLeavesBlock: myLeavesBlock,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        myLeavesAlertsBlock: myLeavesAlertsBlock,
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