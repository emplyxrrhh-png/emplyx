VTPortal.forgotten = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var punchesDS = ko.observable([]);
    var selectedDate = ko.observable(new Date());
    var punchesDesc = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            if (VTPortal.roApp.userStatus().TaskTitle.length <= 50) {
                return i18nextko.i18n.t('inTask') + VTPortal.roApp.userStatus().TaskTitle;
            } else {
                return i18nextko.i18n.t('inTask') + VTPortal.roApp.userStatus().TaskTitle.substr(0, 48) + '...';
            }
        } else return '';
    });
    var noPunches = ko.computed(function () {
        return i18nextko.i18n.t('noPunches');
    });
    var requestId = ko.observable(null);
    var request = ko.observable();
    var employeeAccruals = ko.observable([]);
    var viewActions = ko.observable(false);
    var lblAvailable = ko.observable(0);
    var lblPending = ko.observable(0);
    var lblApproved = ko.observable(0);
    var infoDivVisible = ko.observable(false);
    var popupVisible = ko.observable(false);
    var remarks = ko.observable('');
    var remarksDivVisible = ko.observable(false);
    var currentPunchesDivVisible = ko.observable(false);
    var myApprovalsBlock = VTPortal.approvalHistory();
    var viewHistory = ko.observable(false);
    var requestMainDivVisible = ko.observable(false);
    var loadingDivVisible = ko.observable(false);

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: []
    }));

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    var lblPunchInfo = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().RequestInfo;
        }
        else {
            '';
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

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                loadingDivVisible(false);
                requestMainDivVisible(true);

                request(result);
                getPunches();
                viewActions(false);
                if (result.ReqStatus == 0 || result.ReqStatus == 1) {
                    viewActions(true);
                }
                if (result.Comments != "") {
                    remarksDivVisible(true);
                    remarks(result.Comments);
                }
                else {
                    remarksDivVisible(false);
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

    var getPunches = function (e) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                currentPunchesDivVisible(true);
                var punches = result.Punches;
                for (var i = 0; i < punches.length; i++) {
                    switch (punches[i].Type) {
                        case 1:
                        case 2:
                        case 3:
                        case 7:
                            if (punches[i].TypeData != null && punches[i].TypeData > 0) {
                                if (punches[i].ActualType == 1) {
                                    if (typeof punches[i].InTelecommute != 'undefined' && punches[i].InTelecommute.toLowerCase() == "true") {
                                        punches[i].cssClass = 'dx-icon-TATC_in';
                                        punches[i].Name = i18nextko.i18n.t('roEntry') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                    }
                                    else {
                                        punches[i].cssClass = 'dx-icon-TA_in_cause';
                                        punches[i].Name = i18nextko.i18n.t('roPunches_TA_in_causeHome') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                    }
                                } else {
                                    punches[i].cssClass = 'dx-icon-TA_out_cause';
                                    punches[i].Name = i18nextko.i18n.t('roPunches_TA_out_causeHome') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                }
                            } else {
                                if (punches[i].ActualType == 1) {
                                    if (typeof punches[i].InTelecommute != 'undefined' && punches[i].InTelecommute.toLowerCase() == "true") {
                                        punches[i].cssClass = 'dx-icon-TATC_in';
                                        punches[i].Name = i18nextko.i18n.t('roEntry') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                    }
                                    else {
                                        punches[i].cssClass = 'dx-icon-TA_in';
                                        punches[i].Name = i18nextko.i18n.t('roEntry') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                    }
                                } else {
                                    punches[i].cssClass = 'dx-icon-TA_out';
                                    punches[i].Name = i18nextko.i18n.t('roExit') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                }
                            }
                            punches[i].hasAction = true;
                            break;
                        case 4:
                            punches[i].cssClass = 'dx-icon-Task_change';
                            punches[i].hasAction = false;
                            punches[i].Name = i18nextko.i18n.t('roPunches_Task_changeHome') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                            break;
                        case 13:
                            punches[i].cssClass = 'dx-icon-CostCenter';
                            punches[i].hasAction = false;
                            punches[i].Name = i18nextko.i18n.t('roPunches_CostCenter') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                            break;
                    }

                    punches[i].Value = punches[i].RequestedTypeData != "" ? i18nextko.i18n.t('roRequestPendingApprove') + punches[i].RequestedTypeData : punches[i].RelatedInfo;
                }
                punchesDS(punches);
            } else {
                currentPunchesDivVisible(false);
                punchesDS([]);
            }
        }).getPunchesOnDate(request().IdEmployee, request().Date1);
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
        title: i18nextko.i18n.t('roRequestType_ForbiddenPunch'),
        subscribeBlock: globalStatus(),
        empImage: employeeImage,
        lblEmployee: lblEmployee,
        detailsVisible: function () {
            popupRequestDaysVisible(true);
        },
        lblRequestDates: lblRequestDates,
        lblOverlays: i18nextko.i18n.t('lblOverlays'),
        lblAccruals: i18nextko.i18n.t('roMyAccrualsHint'),
        lblNoOverlays: i18nextko.i18n.t('lblNoOverlays'),
        lblAvailable: lblAvailable,
        lblPending: lblPending,
        lblApproved: lblApproved,
        noPunches: noPunches,
        lblRemarks: i18nextko.i18n.t('lblRemarks'),
        lblApprovedText: i18nextko.i18n.t('lblApprovedText'),
        lblPendingText: i18nextko.i18n.t('lblPendingText'),
        lblAvailableText: i18nextko.i18n.t('lblAvailableText'),
        lblRequestedDays: i18nextko.i18n.t('lblRequestedDays'),
        lblCurrentPunches: i18nextko.i18n.t('lblCurrentPunches'),
        lblLoading: i18nextko.i18n.t('lblLoading'),
        lblGroupName: lblGroupName,
        remarks: remarks,
        lblPunchInfo: lblPunchInfo,
        myApprovalsBlock: myApprovalsBlock,
        popupVisible: popupVisible,
        infoDivVisible: infoDivVisible,
        remarksDivVisible: remarksDivVisible,
        currentPunchesDivVisible: currentPunchesDivVisible,
        requestMainDivVisible: requestMainDivVisible,
        loadingDivVisible: loadingDivVisible,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        punchesDS: punchesDS,
        noPunches: noPunches,
        listCurrentPunches: {
            dataSource: punchesDS,
            scrollingEnabled: true,
            grouped: false
        },
        scrollContent: {
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