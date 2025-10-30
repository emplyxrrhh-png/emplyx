VTPortal.myteam = function (params) {
    var requestsCount = ko.observable(0);
    var dailyRecordsCount = ko.observable(0);
    var alertsCount = ko.observable(0);
    var curSelectedView = ko.observable(1);
    var curSelectedIndex = ko.observable(0);
    var loadingVisible = ko.observable(false);
    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var cView = null;

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


    var requestsDesc = ko.computed(function () {
        if (requestsCount() == 0) {
            return i18nextko.i18n.t('noRequests');
        }
        else {
            return i18nextko.i18n.t('newRequests') + requestsCount() + i18nextko.i18n.t('newRequests2');
        }
    });

    var workingStatementsDesc = ko.computed(function () {
        if (dailyRecordsCount() == 0) {
            return i18nextko.i18n.t('noWorkingStatements');
        }
        else {
            return i18nextko.i18n.t('newWorkingStatements') + dailyRecordsCount() + i18nextko.i18n.t('newWorkingStatements2');
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

        let loadAlertsCallback = function (myTeamView) {
            return function () {
                if (VTPortal.roApp.userAlerts() != null && typeof VTPortal.roApp.userAlerts() != 'undefined' && VTPortal.roApp.loggedIn == true) {
                    var alerts = VTPortal.roApp.userAlerts().SupervisorStatus;

                    if (typeof alerts.Alerts != 'undefined' && typeof myTeamView.alertsCount() != 'undefined') {
                        myTeamView.alertsCount(alerts.Alerts.length);
                    }
                    if (typeof alerts.DocumentAlerts != 'undefined' && typeof myTeamView.alertsCount() != 'undefined') {
                        myTeamView.alertsCount(myTeamView.alertsCount() + alerts.DocumentAlerts.AbsenteeismDocuments.length + alerts.DocumentAlerts.DocumentsValidation.length + alerts.DocumentAlerts.GpaAlerts.length + alerts.DocumentAlerts.MandatoryDocuments.length + alerts.DocumentAlerts.WorkForecastDocuments.length + alerts.DocumentAlerts.AccessAuthorizationDocuments.length)
                    }

                    var callbackFuntion = function (result) {
                        if (result.Status == window.VTPortalUtils.constants.OK.value) {
                            const requests = Object.assign({}, result);
                            var requestsInfo = result.Requests.filter(obj => {
                                return obj.IdRequestType !== 17;
                            });
                            if (myTeamView) myTeamView.requestsCount(requestsInfo.length);
                            if (result.Requests != null && typeof (result.Requests) != 'undefined') requests.Requests = requestsInfo;
                            VTPortal.roApp.requestsDS(requests);
                            var dailyRecordInfo = result.Requests.filter(obj => { return obj.IdRequestType === 17; });
                            const dailyRecords = Object.assign({}, result);
                            if (myTeamView) myTeamView.dailyRecordsCount(dailyRecordInfo.length);
                            if (result.Requests != null && typeof (result.Requests) != 'undefined') dailyRecords.Requests = dailyRecordInfo;
                            VTPortal.roApp.dailyRecordsDS(dailyRecords);
                            if (myTeamView) myTeamView.refreshAvailableOptions();
                        } else {
                            if (myTeamView) myTeamView.requestsCount(0);
                            if (myTeamView) myTeamView.dailyRecordsCount(0);
                            if (myTeamView) myTeamView.refreshAvailableOptions();
                        }
                    };

                    new WebServiceRobotics(function (result) { callbackFuntion(result); }).getSupervisedRequests(false, moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').startOf('year').add(1, 'year'), '0*1|1*2*3*4*5*6*7*8*9*10*11*12*13*14*15*16*17', 'RequestDate DESC');
                }
            }        
        }

        VTPortal.roApp.refreshEmployeeAlerts(loadAlertsCallback(cView));
    }

    var refreshModules = function () {
        VTPortal.notificationsHome().viewShown();

        VTPortal.roApp.refreshEmployeeStatus();
        VTPortal.roApp.refreshEmployeeAlerts();
    }

    function refreshAvailableOptions() {
        
        if (!VTPortal.roApp.loggedIn) VTPortalUtils.utils.loginIfNecessary();

        var refreshMenuItems = [];

        refreshMenuItems.push({ "ID": 1, "Name": i18nextko.t('roMyTeamEmployees') });
        refreshMenuItems.push({ "ID": 2, "Name": i18nextko.t('roMyTeamRequests'), "Count": requestsDesc });
        if (VTPortal.roApp.DailyRecordEnabled()) {
            refreshMenuItems.push({ "ID": 4, "Name": i18nextko.t('roMyTeamDailyRecords'), "Count": workingStatementsDesc });
        }

        if (VTPortal.roApp.userStatus().HasAlertPermission || typeof VTPortal.roApp.userStatus().HasAlertPermission == 'undefined') {
            refreshMenuItems.push({ "ID": 3, "Name": i18nextko.t('roMyTeamAlerts'), "Count": alertsDesc });
        }

        mainMenuItems(refreshMenuItems);

        setTimeout(function () { $('#scrollview').height($('#panelsContent').height()); }, 100);
    }

    function viewShown() {
        cView = this;

        globalStatus().viewShown();
        refreshData();
        refreshModules();
        
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
                    case 4:
                        VTPortal.app.navigate("myTeamDailyRecords");
                        break;
                    default:
                        VTPortalUtils.utils.notifyMesage("not implemented", 'error', 2000)
                        break;
                }
            }
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        alertsCount: alertsCount,
        dailyRecordsCount: dailyRecordsCount,
        refreshAvailableOptions: refreshAvailableOptions,
        requestsCount: requestsCount
    };

    return viewModel;
};