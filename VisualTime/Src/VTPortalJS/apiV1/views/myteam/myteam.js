VTPortal.myteam = function (params) {
    var requestsCount = ko.observable(0);
    var alertsCount = ko.observable(0);
    var curSelectedView = ko.observable(1);
    var curSelectedIndex = ko.observable(0);
    var loadingVisible = ko.observable(false);

    if (typeof params.id != 'undefined') {
        curSelectedView(parseInt(params.id, 10));
        curSelectedIndex(parseInt(params.id, 10) - 1);
    }

    var apiVersion = ko.computed(function () {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SupervisorPortal) return true;
        else return false;
    });

    var mainMenuItems = ko.observable(new DevExpress.data.DataSource({
        store: []
    }));;

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var requestsDesc = ko.computed(function () {
        if (requestsCount() == 0) {
            return i18nextko.i18n.t('noRequests');
        }
        else {
            return i18nextko.i18n.t('newRequests') + requestsCount() + i18nextko.i18n.t('newRequests2');
        }
    });

    var alertsDesc = ko.computed(function () {
        if (alertsCount() == 0) {
            return i18nextko.i18n.t('noAlerts');
        }
        else {
            return i18nextko.i18n.t('newAlerts') + alertsCount() + i18nextko.i18n.t('newAlerts2');
        }
    });

    function refreshData() {
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined' && VTPortal.roApp.loggedIn == true) {
            var alerts = VTPortal.roApp.userStatus().SupervisorStatus;

            if (typeof alerts.Alerts != 'undefined') {
                alertsCount(alerts.Alerts.length);
            }
            if (typeof alerts.DocumentAlerts != 'undefined') {
                alertsCount(alertsCount() + alerts.DocumentAlerts.AbsenteeismDocuments.length + alerts.DocumentAlerts.DocumentsValidation.length + alerts.DocumentAlerts.GpaAlerts.length + alerts.DocumentAlerts.MandatoryDocuments.length + alerts.DocumentAlerts.WorkForecastDocuments.length + alerts.DocumentAlerts.AccessAuthorizationDocuments.length)
            }

            var callbackFuntion = function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.lastOkRequests(new Date());
                    VTPortal.roApp.requestsDS(result);
                    var requestsInfo = result.Requests;
                    requestsCount(requestsInfo.length);
                    addItems();
                } else {
                    requestsCount(0);
                    addItems();
                }
            };

            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getSupervisedRequests(false, moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').startOf('year').add(1, 'year'), '0*1|1*2*3*4*5*6*7*8*9*10*11*12*13*14*15*16', 'RequestDate DESC');
        }
    }

    function addItems() {
        if (!VTPortal.roApp.loggedIn) VTPortalUtils.utils.loginIfNecessary();

        var refreshMenuItems = [];

        refreshMenuItems.push({ "ID": 1, "Name": i18nextko.t('roMyTeamEmployees') });
        refreshMenuItems.push({ "ID": 2, "Name": i18nextko.t('roMyTeamRequests'), "Count": requestsDesc });

        if (VTPortal.roApp.userStatus().HasAlertPermission == true || typeof VTPortal.roApp.userStatus().HasAlertPermission == 'undefined') {
            refreshMenuItems.push({ "ID": 3, "Name": i18nextko.t('roMyTeamAlerts'), "Count": alertsDesc });
        }

        mainMenuItems(refreshMenuItems);

        setTimeout(function () { $('#scrollview').height($('#panelsContent').height()); }, 100);
    }

    function viewShown() {
        globalStatus().viewShown();
        refreshData();
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roMySupervisorTitle'),
        subscribeBlock: globalStatus(),
        futureVersions: i18nextko.t('roMyTeamFutureVersions'),
        isSupervisorPortalActive: apiVersion,
        listOptions: {
            height: "100%",
            scrollingEnabled: false,
            grouped: false,
            dataSource: mainMenuItems,
            indicateLoading: false,
            onItemClick: function (info) {
                switch (info.itemData.ID) {
                    case 1:
                        VTPortal.app.navigate("myTeamEmployees");
                        break;
                    case 2:
                        VTPortal.app.navigate("myTeamRequests");
                        break;
                    case 3:
                        VTPortal.app.navigate("myTeamAlerts");
                        break;
                    default:
                        VTPortalUtils.utils.notifyMesage("not implemented", 'error', 2000)
                        break;
                }
            }
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
    };

    return viewModel;
};