VTPortal.punchManagement = function (params) {
    var selectedIndex = ko.observable(0);
    var curSelectedView = ko.observable(1);

    var selectedDate = ko.observable(new Date());

    if (typeof params.id != 'undefined') {
        selectedIndex(parseInt(params.id, 10) - 1);
        curSelectedView(parseInt(params.id, 10));
    }

    if (typeof params.param != 'undefined') {
        selectedDate(moment(params.param, "YYYY-MM-DD").toDate());
    }

    var myPunches = VTPortal.myPunches(),
        myPunchRequests = VTPortal.myPunchRequests();

    var profileTitle = ko.computed(function () {
        switch (curSelectedView()) {
            case 1:
                return i18nextko.i18n.t('roPunchesTitle');
                break;
            case 2:
                return i18nextko.i18n.t('roMyPunchRequests');
                break;
        }
    }, this);

    var availableTabPanels = [];
    availableTabPanels.push({ "ID": 1, "cssClass": "dx-icon-myPunches" });
    availableTabPanels.push({ "ID": 2, "cssClass": "dx-icon-myPuncheRequests" });

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function setUpView() {
        myPunches.modelIsReady(false);
        myPunchRequests.modelIsReady(false);

        switch (curSelectedView()) {
            case 1:
                myPunches.initializeView(selectedDate());
                break;
            case 2:
                myPunchRequests.initializeView();
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
    }

    var logoutSession = function () {
        VTPortal.roApp.db.settings.password = '';

        VTPortal.roApp.db.settings.save();
        VTPortal.roApp.impersonatedIDEmployee = -1;
        VTPortal.roApp.supervisedEmployees = [];
        VTPortal.roApp.loggedIn = false;
        VTPortal.app.navigate("login", { root: true });
    }

    var viewModel = {
        viewShown: viewShown,
        title: profileTitle,
        subscribeBlock: globalStatus(),
        myPunchesBlock: myPunches,
        myPunchRequestsBlock: myPunchRequests,
        selectedTab: selectedIndex,
        selectedView: curSelectedView,
        tabPanelOptions: {
            dataSource: availableTabPanels,
            selectedIndex: selectedIndex,
            itemTemplate: "title",
            onSelectionChanged: function (e) {
                changeTab(e.addedItems[0]);
            }
        }
    };

    return viewModel;
};