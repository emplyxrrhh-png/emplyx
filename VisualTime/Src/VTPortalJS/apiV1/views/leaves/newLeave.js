VTPortal.newLeave = function (params) {
    var remarks = ko.observable('');
    var canSave = ko.observable(false);

    var documentsDS = ko.observable([]);
    var documentsBlock = new VTPortal.documentInput();
    var causeHasDocuments = ko.computed(function () {
        return (documentsDS().length > 0);
    });

    var minSelectedDate = ko.observable(moment().toDate());

    if (typeof params.iDate != 'undefined' && parseInt(params.iDate, 10) != -1) {
        minSelectedDate(moment(params.iDate, 'YYYY-MM-DD'));
    }

    var viewTitle = ko.computed(function () {
        return i18nextko.i18n.t('roRequestType_Leave')
    });

    var absenceTitle = ko.computed(function () {
        return i18nextko.i18n.t('roSelectLeave')
    });

    var reqValue1 = ko.observable(null), requestValue1DS = ko.observable([]);

    var computedScrollHeight = ko.computed(function () {
        return '80%'
    });

    var loadLists = function (lstdataSource) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                requestValue1DS(result.SelectFields);
                if (result.SelectFields.length > 0) reqValue1(result.SelectFields[0].FieldValue);
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
        requestValue1DS([]);

        loadLists('causes.availableleave');

        canSave(true);
    }

    var wsRobotics = null;
    function saveRequest() {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtHome();
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roLaveSaved"), 'success', 2000);
                VTPortal.roApp.loadInitialData(false, false, false, false, false);
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        var document = documentsBlock.getDocument();
        var wsMinDate = minSelectedDate() == null ? VTPortalUtils.nullDate : moment(minSelectedDate());

        if (VTPortal.roApp.isModeApp()) wsRobotics.saveLeaveMobile(wsMinDate, wsMinDate, reqValue1(), remarks(), document);
        else wsRobotics.saveLeaveDesktop(wsMinDate, wsMinDate, reqValue1(), remarks(), document);
    }

    var cmbRequestValue1Changed = function (e) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                documentsDS(result.SelectFields);
                documentsBlock.initializeData(result.SelectFields, false, -1);
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
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingCauseInfo"), 'error', 0, onContinue);
        }).getRequieredLeaveDocuments(parseInt(e.value, 10), true);
    }

    var viewModel = {
        viewShown: viewShown,
        title: viewTitle,
        lblDocDate: i18nextko.t('roRequestDocRelatedDate'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: absenceTitle,
        subscribeBlock: globalStatus(),
        scrollContent: {
            height: computedScrollHeight
        },
        btnSave: {
            onClick: saveRequest,
            text: '',
            icon: "Images/Common/save.png",
            visible: canSave
        },
        remarks: {
            value: remarks,
        },
        cmbRequestValue1: {
            dataSource: requestValue1DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue1,
            onValueChanged: cmbRequestValue1Changed,
        },
        docDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: minSelectedDate,
            valueChangeEvent: 'focusout'
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        myDocumentsBlock: documentsBlock,
        hasDocuments: causeHasDocuments
    };

    return viewModel;
};