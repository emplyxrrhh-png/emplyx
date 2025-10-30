VTPortal.cancelHolidaysNew = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var requestId = ko.observable(null);
    var request = ko.observable();
    var viewActions = ko.observable(false);
    var lblAvailable = ko.observable(0);
    var IdEmployeeRequest = ko.observable(0);
    var lblPending = ko.observable(0);
    var lblSelectedDayMV = ko.observable("");
    var lblSelectedDayDetails = ko.observable("");
    var lblApproved = ko.observable(0);
    var infoDivVisible = ko.observable(false);
    var popupVisible = ko.observable(false);
    var remarks = ko.observable('');
    var myApprovalsBlock = VTPortal.approvalHistory();
    var viewHistory = ko.observable(false);
    var requestMainDivVisible = ko.observable(false);
    var loadingDivVisible = ko.observable(false);
    var daysDetailDiv = ko.observable(false);

    var popupRequestDaysVisible = ko.observable(false);

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: []
    }));

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

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

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    var formReadOnly = ko.observable(false);

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                remarks(result.Comments);

                IdEmployeeRequest(result.IdEmployee);

                loadingDivVisible(false);
                requestMainDivVisible(true);

                request(result);

                if (result.RequestDays != null) {
                    if (result.RequestDays.length > 1) {
                        var ds = []
                        result.RequestDays.forEach(function (item, index) {
                            ds.push({ RequestDate: moment(item.RequestDate).format('dddd DD MMMM') });
                        });
                        dataSource(new DevExpress.data.DataSource({
                            store: ds
                        }));
                        daysDetailDiv(true);
                    }
                    else {
                        daysDetailDiv(false);
                    }
                }
                else {
                    daysDetailDiv(false);
                }

                if (result.Comments != "") {
                    infoDivVisible(true);
                    remarks(result.Comments);
                }
                else {
                    infoDivVisible(false);
                }

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

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        refreshData();
    };

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_CancelHolidays'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: i18nextko.t('roSelectPlannedHoliday'),
        lblSelectDaysInfo: i18nextko.t('roSelectDayInfo'),
        lblRequestedDays: i18nextko.i18n.t('lblRequestedDays'),
        empImage: employeeImage,
        lblEmployee: lblEmployee,
        detailsVisible: function () {
            popupRequestDaysVisible(true);
        },
        daysDetailDiv: daysDetailDiv,
        lblRequestDates: lblRequestDates,
        lblAvailable: lblAvailable,
        lblPending: lblPending,
        lblApproved: lblApproved,
        lblLoading: i18nextko.i18n.t('lblLoading'),
        lblSelectedDayMV: lblSelectedDayMV,
        lblSelectedDayDetails: lblSelectedDayDetails,
        lblGroupName: lblGroupName,
        subscribeBlock: globalStatus(),
        scrollContent: {
            height: computedScrollHeight
        },
        popupVisible: popupVisible,
        popupRequestDaysVisible: popupRequestDaysVisible,
        infoDivVisible: infoDivVisible,
        requestMainDivVisible: requestMainDivVisible,
        loadingDivVisible: loadingDivVisible,
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
        remarks: {
            value: remarks,
            readOnly: formReadOnly
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        listRequestDays: {
            dataSource: dataSource,
            scrollingEnabled: true,
            grouped: false,
            height: 300,
            itemTemplate: 'RequestItem'
        }
    };

    return viewModel;
};