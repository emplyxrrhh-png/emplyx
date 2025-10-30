VTPortal.telecommuting = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var selectedDate = ko.observable(moment().add(-1, 'days').toDate());
    var requestId = ko.observable(null);
    var request = ko.observable();
    var employeeAccruals = ko.observable([]);
    var viewActions = ko.observable(false);
    var lblAvailable = ko.observable(0);
    var lblPending = ko.observable(0);
    var lblApproved = ko.observable(0);
    var infoDivVisible = ko.observable(false);
    var accrualsDivVisible = ko.observable(false);
    var popupVisible = ko.observable(false);
    var popupRequestDaysVisible = ko.observable(false);
    var details = ko.observable('');
    var myApprovalsBlock = VTPortal.approvalHistory();
    var viewHistory = ko.observable(false);
    var requestMainDivVisible = ko.observable(false);
    var loadingDivVisible = ko.observable(false);

    var tcInfo = ko.observable(
        null
    );

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: []
    }));

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    var tcTelecommutePlannedHours = ko.computed(function () {
        if (tcInfo() != null) {
            return '(<b>' + tcInfo().TelecommutePlannedHours + ' </b>' + i18nextko.i18n.t('tcTelecommutePlannedHours1') + ' ' + i18nextko.i18n.t('tcTelecommutePlannedHours2') + '<b> ' + tcInfo().TotalWorkingPlannedHours + '</b>)';
        }
        else {
            return '';
        }
    });

    var tcPeriodStart = ko.computed(function () {
        if (tcInfo() != null) {
            if (tcInfo().ByPercentage == true) {
                switch (tcInfo().Period) {
                    case 0:
                        return i18nextko.i18n.t('tcPeriodStart') + '<b> ' + moment(tcInfo().PeriodStart).format("DD/MM/YYYY") + ' ' + i18nextko.i18n.t('tcPeriodAnd') + ' ' + moment(tcInfo().PeriodEnd).format("DD/MM/YYYY") + '</b> ' + i18nextko.i18n.t('tcMaxInPeriod') + ' <b>' + tcInfo().MaxPercentage + '%/' + i18nextko.i18n.t('semana') + ' </b> ' + i18nextko.i18n.t('tcMaxInPeriod2');
                        break;
                    case 1:
                        return i18nextko.i18n.t('tcPeriodStart') + '<b> ' + moment(tcInfo().PeriodStart).format("DD/MM/YYYY") + ' ' + i18nextko.i18n.t('tcPeriodAnd') + ' ' + moment(tcInfo().PeriodEnd).format("DD/MM/YYYY") + '</b> ' + i18nextko.i18n.t('tcMaxInPeriod') + ' <b>' + tcInfo().MaxPercentage + '%/' + i18nextko.i18n.t('mes') + ' </b> ' + i18nextko.i18n.t('tcMaxInPeriod2');
                        break;
                    case 2:
                        return i18nextko.i18n.t('tcPeriodStart') + '<b> ' + moment(tcInfo().PeriodStart).format("DD/MM/YYYY") + ' ' + i18nextko.i18n.t('tcPeriodAnd') + ' ' + moment(tcInfo().PeriodEnd).format("DD/MM/YYYY") + '</b> ' + i18nextko.i18n.t('tcMaxInPeriod') + ' <b>' + tcInfo().MaxPercentage + '%/' + i18nextko.i18n.t('trimestre') + ' </b> ' + i18nextko.i18n.t('tcMaxInPeriod2');
                        break;
                }
            }
            else {
                switch (tcInfo().Period) {
                    case 0:
                        return i18nextko.i18n.t('tcPeriodStart') + '<b> ' + moment(tcInfo().PeriodStart).format("DD/MM/YYYY") + ' ' + i18nextko.i18n.t('tcPeriodAnd') + ' ' + moment(tcInfo().PeriodEnd).format("DD/MM/YYYY") + '</b> ' + i18nextko.i18n.t('tcMaxInPeriod') + ' <b>' + tcInfo().MaxDays + 'd/' + i18nextko.i18n.t('semana') + ' </b> ' + i18nextko.i18n.t('tcMaxInPeriod2');
                        break;
                    case 1:
                        return i18nextko.i18n.t('tcPeriodStart') + '<b> ' + moment(tcInfo().PeriodStart).format("DD/MM/YYYY") + ' ' + i18nextko.i18n.t('tcPeriodAnd') + ' ' + moment(tcInfo().PeriodEnd).format("DD/MM/YYYY") + '</b> ' + i18nextko.i18n.t('tcMaxInPeriod') + ' <b>' + tcInfo().MaxDays + 'd/' + i18nextko.i18n.t('mes') + ' </b> ' + i18nextko.i18n.t('tcMaxInPeriod2');
                        break;
                    case 2:
                        return i18nextko.i18n.t('tcPeriodStart') + '<b> ' + moment(tcInfo().PeriodStart).format("DD/MM/YYYY") + ' ' + i18nextko.i18n.t('tcPeriodAnd') + ' ' + moment(tcInfo().PeriodEnd).format("DD/MM/YYYY") + '</b> ' + i18nextko.i18n.t('tcMaxInPeriod') + ' <b>' + tcInfo().MaxDays + 'd/' + i18nextko.i18n.t('trimestre') + ' </b> ' + i18nextko.i18n.t('tcMaxInPeriod2');
                        break;
                }
            };
        }
        else {
            return '';
        }
    });

    var tcCurrentPercentage = ko.computed(function () {
        if (tcInfo() != null) {
            if (tcInfo().ByPercentage == true) {
                return i18nextko.i18n.t('tcPercentageUsedRequest') + '<b> ' + tcInfo().CurrentAgreementPercentageUsed + '</b>' + i18nextko.i18n.t('tcPercentageUsedRequest2') + ' (<b>' + tcInfo().TelecommutePlannedHours + ' </b>' + i18nextko.i18n.t('tcTelecommutePlannedHours1') + ' ' + i18nextko.i18n.t('tcTelecommutePlannedHours2') + '<b> ' + tcInfo().TotalWorkingPlannedHours + '</b>)';
            }
            else {
                return i18nextko.i18n.t('tcDaysUsedRequest') + '<b> ' + tcInfo().CurrentAgreementDaysUsed + '</b> ' + i18nextko.i18n.t('tcDaysUsedRequest2') + ' (<b>' + tcInfo().TelecommutePlannedHours + ' </b>' + i18nextko.i18n.t('tcTelecommutePlannedHours1') + ' ' + i18nextko.i18n.t('tcTelecommutePlannedHours2') + '<b> ' + tcInfo().TotalWorkingPlannedHours + '</b>)';
            };
        }
        else {
            return '';
        }
    });

    var lblEmployee = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().EmployeeName;
        }
        else {
            '';
        }
    });

    var lblRequestDates = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().RequestInfo;
        }
        else {
            '';
        }
    });

    var lblGroupName = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().EmployeeGroup;
        }
        else {
            '';
        }
    });

    var getTCInfo = function (date, idEmployee) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                tcInfo(result);
            } else {
                DevExpress.ui.notify(i18nextko.t('roTCError'), 'warning', 3000);
            }
        }).getTelecommutingInfo(moment(date), idEmployee);
    }

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                getTCInfo(result.Date1, result.IdEmployee);

                loadingDivVisible(false);
                requestMainDivVisible(true);

                request(result);
                infoDivVisible(true);

                viewActions(false);
                if (result.ReqStatus == 0 || result.ReqStatus == 1) {
                    viewActions(true);
                }

                myApprovalsBlock.refreshApprovals(result.RequestsHistoryEntries);
            } else {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }, function (error) {
            var onContinue = function () {
                VTPortal.roApp.redirectAtHome();
            }
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequest"), 'error', 0, onContinue);
        }).getRequestForSupervisor(requestId());
    }

    var employeeImage = ko.computed(function () {
        if (typeof request() != 'undefined') {
            var backgroundImage = '';
            if (request().EmployeeImage != '') {
                backgroundImage = 'url(data:image/png;base64,' + request().EmployeeImage + ')';
            }
            else {
                backgroundImage = 'url(Images/logovtl.ico)';
            }

            return backgroundImage;
        }
        else {
            return '';
        }
    });

    function refreshData() {
        loadingDivVisible(true);
        loadRequest();
    }

    function viewShown() {
        globalStatus().viewShown();
        refreshData();
    };

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roRequestType_PlannedAbsences'),
        subscribeBlock: globalStatus(),
        empImage: employeeImage,
        lblEmployee: lblEmployee,
        lblRequestDates: lblRequestDates,
        tcCurrentPercentage: tcCurrentPercentage,
        tcPeriodStart: tcPeriodStart,
        tcTelecommutePlannedHours: tcTelecommutePlannedHours,
        lblAccruals: i18nextko.i18n.t('roMyAccrualsHint'),
        lblAvailable: lblAvailable,
        lblPending: lblPending,
        lblApproved: lblApproved,
        lblDetails: i18nextko.i18n.t('lblDetails'),
        lblApprovedText: i18nextko.i18n.t('lblApprovedText'),
        lblPendingText: i18nextko.i18n.t('lblPendingText'),
        lblAvailableText: i18nextko.i18n.t('lblAvailableText'),
        lblRequestedDays: i18nextko.i18n.t('lblRequestedDays'),
        lblLoading: i18nextko.i18n.t('lblLoading'),
        lblGroupName: lblGroupName,
        myApprovalsBlock: myApprovalsBlock,
        popupVisible: popupVisible,
        popupRequestDaysVisible: popupRequestDaysVisible,
        accrualsDivVisible: accrualsDivVisible,
        infoDivVisible: infoDivVisible,
        requestMainDivVisible: requestMainDivVisible,
        loadingDivVisible: loadingDivVisible,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        scrollContent: {
        },
        listRequestDays: {
            dataSource: dataSource,
            scrollingEnabled: true,
            grouped: false,
            height: 300,
            itemTemplate: 'RequestItem'
        },
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequestNew(requestId(), request().RequestType, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequestNew(requestId(), request().RequestType, false); },
            text: '',
            hint: i18nextko.i18n.t('roRefuse'),
            icon: "Images/Common/refuse.png",
            visible: viewActions
        },
        btnHistory: {
            onClick: function () { popupVisible(true); },
            text: '',
            hint: i18nextko.i18n.t('roLblRequestApprovals'),
            icon: "Images/Common/historic.png",
            visible: viewHistory
        },
    };

    return viewModel;
};