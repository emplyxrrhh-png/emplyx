VTPortal.profileUserFields = function (params) {
    var modelIsReady = ko.observable(false);
    var userFieldsDS = ko.observable([]);
    var noPermissions = VTPortal.noPermissions();
    function viewShown(selDate) {
        refreshData();
        modelIsReady(true);
        $('#scrollview').height($('#panelsContent').height() - 70);
    };

    function refreshData() {
        if ((VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().UserFieldQuery))) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var categories = result.Categories;
                    for (var i = 0; i < categories.length; i++) {
                        for (var z = 0; z < categories[i].items.length; z++) {
                            if (result.CanCreateRequest) {
                                if (typeof categories[i].items[z].CanDeliverDocument != 'undefined' && categories[i].items[z].CanDeliverDocument) {
                                    categories[i].items[z].hasAction = true;
                                } else {
                                    categories[i].items[z].hasAction = false;
                                }
                            } else {
                                categories[i].items[z].hasAction = false;
                            }

                            categories[i].items[z].cssClass = '';
                        }
                    }
                    userFieldsDS(categories);

                    for (var i = 0; i < categories.length; i++) {
                        if (i > 0) { $("#profileUserFields").dxList("instance").collapseGroup(i); }
                    }
                } else {
                    userFieldsDS([]);
                    var onContinue = function () {
                        VTPortal.roApp.loadInitialData(false, false, true, false, false);
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            }).getUserFields();
        }
    }

    var hasPermission = ko.computed(function () {
        if ((VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().UserFieldQuery))) {
            return true;
        } else {
            return false;
        }
    });

    var viewModel = {
        modelIsReady: modelIsReady,
        viewShown: viewShown,
        hasPermission: hasPermission,
        noPermissions: noPermissions,
        userFieldsDS: userFieldsDS,
        profileUserFields: {
            dataSource: userFieldsDS,
            scrollingEnabled: false,
            grouped: true,
            collapsibleGroups: true,
            groupTemplate: function (data) {
                return $("<div>" + data.key + "</div>")
            },
            itemTemplate: 'userField',
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    VTPortal.app.navigate('userFields/-1/' + info.itemData.Name);
                }
            }
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};