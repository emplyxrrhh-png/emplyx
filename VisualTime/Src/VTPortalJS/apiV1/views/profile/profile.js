VTPortal.profile = function (params) {
    var selectedIndex = ko.observable(0);
    var curSelectedView = ko.observable(1);

    if (typeof params.id != 'undefined') {
        selectedIndex(parseInt(params.id, 10) - 1);
        curSelectedView(parseInt(params.id, 10));
    }

    var profileUserFields = VTPortal.profileUserFields(),
        profileConfig = VTPortal.profileConfig();

    var profileTitle = ko.computed(function () {
        switch (curSelectedView()) {
            case 1:
                return i18nextko.i18n.t('roProfileUserFields');
                break;
            case 2:
                return i18nextko.i18n.t('roProfileConfig');
                break;
        }
    }, this);

    var mainMenuItems = ko.observable([]);

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        var availableTabPanels = [];
        if (!VTPortal.roApp.db.settings.onlySupervisor) availableTabPanels.push({ "ID": 1, "cssClass": "dx-icon-profileUserFields" });
        availableTabPanels.push({ "ID": 2, "cssClass": "dx-icon-profileConfig" });

        if (!VTPortal.roApp.db.settings.onlySupervisor) profileUserFields.modelIsReady(false);
        profileConfig.modelIsReady(false);

        mainMenuItems(availableTabPanels);
    };

    function refreshTabs() {
        switch (curSelectedView()) {
            case 1:
                profileUserFields.viewShown();
                break;
            case 2:
                profileConfig.viewShown();
                break;
        }
    };

    var changeTab = function (item) {
        if (typeof item != 'undefined') curSelectedView(item.ID);
        refreshTabs();
    }

    //var logoutSession = function () {
    //    VTPortal.roApp.db.settings.password = '';
    //    VTPortal.roApp.db.settings.save();
    //    VTPortal.roApp.impersonatedIDEmployee = -1;
    //    VTPortal.roApp.supervisedEmployees = [];
    //    VTPortal.roApp.loggedIn = false;
    //    VTPortal.app.navigate("login", { root: true });
    //}

    var viewModel = {
        viewShown: viewShown,
        title: profileTitle,
        subscribeBlock: globalStatus(),
        profileUserFieldsBlock: profileUserFields,
        profileConfigBlock: profileConfig,
        selectedTab: selectedIndex,
        selectedView: curSelectedView,
        tabPanelOptions: {
            dataSource: mainMenuItems,
            selectedIndex: selectedIndex,
            itemTemplate: "title",
            onSelectionChanged: function (e) {
                changeTab(e.addedItems[0]);
            }
        }
    };

    return viewModel;
};