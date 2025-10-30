VTPortal.justifyPunch = function (params) {
    var requestId = ko.observable(null), remarks = ko.observable(''), paramField = ko.observable('');
    var canSave = ko.observable(false), canDelete = ko.observable(false), viewHistory = ko.observable(false), popupVisible = ko.observable(false), viewActions = ko.observable(false);

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));
    if (typeof params.param != 'undefined') paramField(params.param);

    var formReadOnly = ko.observable(false);

    var reqValue1 = ko.observable(null);

    var requestValue1DS = ko.observable([]);

    var myApprovalsBlock = VTPortal.approvalHistory();

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                remarks(result.Comments);
                if (result.ReqStatus == 0) canDelete(true);
                else canDelete(false);

                viewActions(false);
                if (VTPortal.roApp.impersonatedIDEmployee != -1) {

                    if (VTPortal.roApp.isImpersonateEnabled() == false) VTPortal.roApp.disableImpersonateActionsOnRequest();

                    if (result.ReqStatus == 0 || result.ReqStatus == 1) {
                        viewActions(true);
                        canDelete(false);
                    }
                }

                myApprovalsBlock.refreshApprovals(result.RequestsHistoryEntries);

                reqValue1(result.IdCause + '');
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

    var loadLists = function (lstdataSource) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                requestValue1DS(result.SelectFields);

                if (requestId() != null && requestId() > 0) {
                    loadRequest(requestId());
                } else {
                    if (result.SelectFields.length > 0) reqValue1(result.SelectFields[0].FieldValue);
                }
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
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinue);
        }).getGenericList(lstdataSource);
    }

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        loadLists('causes.readerinputcode');

        if (requestId() != null && requestId() > 0) {
            formReadOnly(true);
            VTPortalUtils.utils.markRequestAsRead(requestId());
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
    function saveRequest() {
        var punchDateTime = moment(paramField(), 'YYYYMMDDHHmmss');
        var causeId = reqValue1() != null ? reqValue1() : '-1';

        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtRequestList(3);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
            } else {
                var onContinue = function () {
                    wsRobotics.savePunchCause(causeId, punchDateTime, remarks(), true);
                }

                VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        wsRobotics.savePunchCause(causeId, punchDateTime, remarks(), false);
    }

    function deleteRequest() {
        if (requestId() != null && requestId() > 0) VTPortalUtils.utils.deleteRequest(requestId());
    }

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_JustifyPunch'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: i18nextko.t('roRequestCauseLbl'),
        subscribeBlock: globalStatus(),
        scrollContent: {
            height: computedScrollHeight
        },
        popupVisible: popupVisible,
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 3, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 3, false); },
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
        btnSave: {
            onClick: saveRequest,
            text: '',
            hint: i18nextko.i18n.t('roSaveRequest'),
            icon: "Images/Common/save.png",
            visible: canSave
        },
        btnDelete: {
            onClick: deleteRequest,
            text: '',
            hint: i18nextko.i18n.t('roDeleteRequest'),
            icon: "Images/Common/delete.png",
            visible: canDelete
        },
        remarks: {
            value: remarks,
            readOnly: formReadOnly
        },
        cmbRequestValue1: {
            dataSource: requestValue1DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue1,
            readOnly: formReadOnly
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};