VTPortal.myLeaves = function (params) {
    var modelIsReady = ko.observable(false);
    var leavesDS = ko.observable([]);

    function viewShown(selDate) {
        refreshData();
        modelIsReady(true);
    };

    var hasPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().LeavesEnabled)) {
            return true;
        } else {
            return false;
        }
    });

    function refreshData() {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().LeavesEnabled)) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var leaves = result.Leaves;
                    for (var i = 0; i < leaves.length; i++) {
                        leaves[i].cssClass = 'dx-icon-Leave';

                        leaves[i].Description = i18nextko.i18n.t('roLeavesFrom') + ' ' + moment.tz(leaves[i].BeginDate, VTPortal.roApp.serverTimeZone).format('L') + ' ' + i18nextko.i18n.t('roLeavesCause') + ' ' + leaves[i].Cause;

                        leaves[i].hasAction = true;

                        if (leaves[i].DocAlerts != null && leaves[i].DocAlerts.length > 0) {
                            leaves[i].AlertDescription = leaves[i].DocAlerts[0].Description;
                        } else {
                            leaves[i].AlertDescription = i18nextko.i18n.t('roLeavesNoAlerts');
                        }
                    }

                    leavesDS(leaves);

                    $('#scrollview').height($('#panelsContent').height() - 70);
                } else {
                    leavesDS([]);
                    var onContinue = function () {
                        VTPortal.roApp.loadInitialData(false, false, true, false, false);
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            }).getMyLeaves(true, null, null);
        }
    }

    var onNewRequest = function () {
        VTPortal.app.navigate('newLeave/-1');
    }

    var viewModel = {
        modelIsReady: modelIsReady,
        viewShown: viewShown,
        hasPermission: hasPermission,
        leavesDS: leavesDS,
        listLeaves: {
            dataSource: leavesDS,
            grouped: false,
            scrollingEnabled: false,
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    VTPortal.app.navigate('sendLeaveDocument/-1/' + info.itemData.AbsenceID + "/" + info.itemData.IdCause + "/leave");
                }
            }
        },
        newRequest: {
            onClick: onNewRequest,
            text: '',
            icon: "Images/Common/plus.png",
            visible: VTPortal.roApp.ReadMode() == false
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};