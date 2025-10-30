VTPortal.telecommute = function (params) {
    var requestId = ko.observable(null), remarks = ko.observable('');;
    var canSave = ko.observable(false), canDelete = ko.observable(false), viewHistory = ko.observable(false), popupVisible = ko.observable(false), viewActions = ko.observable(false);
    var formReadOnly = ko.observable(false), myApprovalsBlock = VTPortal.approvalHistory();
    var requestDays = ko.observable([]);

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                if (result.Field1 == "0") {
                    remarks(i18nextko.i18n.t("roRequestTelecommuteOffice") + moment(result.Date1).format("DD/MM/YYYY"));
                }
                else {
                    remarks(i18nextko.i18n.t("roRequestTelecommuteHome") + moment(result.Date1).format("DD/MM/YYYY"));
                }

                if (result.ReqStatus == 0) canDelete(true);
                else canDelete(false);

                viewActions(false);
                if (VTPortal.roApp.impersonatedIDEmployee != -1) {
                    if (result.ReqStatus == 0 || result.ReqStatus == 1) {
                        viewActions(true);
                        canDelete(false);
                    }
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
        }).getRequest(requestId());
    }

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        if (requestId() != null && requestId() > 0) {
            formReadOnly(true);
            VTPortalUtils.utils.markRequestAsRead(requestId());
            loadRequest();
        }

        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (requestId() != null) canDelete(true);
        } else {
            if (requestId() != null) viewActions(false);
        }

        if (requestId() == null) canSave(true);
        if (requestId() != null) viewHistory(true);
    }

    var wsRobotics = null;

    function deleteRequest() {
        if (requestId() != null && requestId() > 0) VTPortalUtils.utils.deleteRequest(requestId());
    }

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_telecommute'),
        lblPunchesInfo: i18nextko.t('rotelecommuteDays'),
        lblRemarks: i18nextko.t('rotelecommuteDaysDetail'),
        remarks: {
            value: remarks,
            readOnly: formReadOnly
        },
        subscribeBlock: globalStatus(),
        scrollContent: {
            height: computedScrollHeight
        },
        popupVisible: popupVisible,
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 16, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 16, false); },
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
        btnDelete: {
            onClick: deleteRequest,
            text: '',
            hint: i18nextko.i18n.t('roDeleteRequest'),
            icon: "Images/Common/delete.png",
            visible: canDelete
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};