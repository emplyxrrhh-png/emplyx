VTPortal.userFields = function (params) {
    var requestId = ko.observable(null), remarks = ko.observable(''), paramField = ko.observable(''), viewActions = ko.observable(false);
    var canSave = ko.observable(false), canDelete = ko.observable(false), popupVisible = ko.observable(false), viewHistory = ko.observable(false), actualFieldDefinition = ko.observable('');

    var documentsDS = ko.observable([]);
    var hasDocuments = ko.observable(false);
    var documentsBlock = new VTPortal.documentInput();

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));
    if (typeof params.param != 'undefined') paramField(params.param);

    var formReadOnly = ko.observable(false);

    var reqValue1 = ko.observable(null), reqValue2 = ko.observable(null);
    var txtReqValue = ko.observable(''), txtReqValue2 = ko.observable('');;
    var dateReqValue2 = ko.observable(moment().startOf('day').toDate()), timeReqValue2 = ko.observable(moment().startOf('day').toDate());

    var requestValue1DS = ko.observable([]);
    var requestValue2DS = ko.observable([]);

    var isReqValueCmb2 = ko.observable(false), isReqValueDate2 = ko.observable(false), isReqValueTime2 = ko.observable(false), isReqValueText2 = ko.observable(false);

    var myApprovalsBlock = VTPortal.approvalHistory();

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    var setReq2VisibleFields = function (isCmb, isDate, isTime, isText) {
        isCmb ? isReqValueCmb2(true) : isReqValueCmb2(false);
        isDate ? isReqValueDate2(true) : isReqValueDate2(false);
        isTime ? isReqValueTime2(true) : isReqValueTime2(false);
        isText ? isReqValueText2(true) : isReqValueText2(false);
    }

    var cmbRequestValue1Changed = function (e) {
        var fieldDefinition = e.value.split('__');
        actualFieldDefinition(e.value);
        hasDocuments(false);

        switch (parseInt(fieldDefinition[1], 10)) {
            case 2:
                setReq2VisibleFields(false, true, false, false);
                break;
            case 4:
                setReq2VisibleFields(false, false, true, false);
                break;
            case 5:
                setReq2VisibleFields(true, false, false, false);

                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        var availableValues = [];
                        for (var i = 0; i < result.Values.length; i++) {
                            availableValues.push({
                                FieldName: result.Values[i],
                                FieldValue: result.Values[i]
                            });
                        }

                        requestValue2DS(availableValues);
                        if (availableValues.length > 0) reqValue2(availableValues[0].FieldValue);
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
                }).getUserFieldAvailableValues(fieldDefinition[0]);
                break;
            case 9:
                initializeDocumentSection(fieldDefinition[3]);
                break;
            default:
                setReq2VisibleFields(false, false, false, true);
                break;
        }
    }

    var initializeDocumentSection = function (idDocumentTemplate) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                documentsDS(result.SelectFields);
                documentsBlock.initializeData(result.SelectFields, false, parseInt(idDocumentTemplate, 10));
                hasDocuments(true);
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
        }).getGenericList('documents');
    }

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                remarks(result.Comments);

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

                var fields = requestValue1DS();
                for (var i = 0; i < fields.length; i++) {
                    if (fields[i].FieldValue == result.FieldName) {
                        actualFieldDefinition(fields[i].FieldValue);
                        reqValue1(fields[i].FieldValue);
                    }
                }

                var fieldDefinition = actualFieldDefinition().split('__');

                switch (parseInt(fieldDefinition[1], 10)) {
                    case 2:
                        dateReqValue2(moment(result.FieldValue, "YYYY/MM/DD").toDate());
                        setReq2VisibleFields(false, true, false, false);
                        break;
                    case 4:
                        timeReqValue2(moment(result.FieldValue, "HH:mm").toDate());
                        setReq2VisibleFields(false, false, true, false);
                        break;
                    case 5:
                        setReq2VisibleFields(true, false, false, false);

                        var request = result;

                        new WebServiceRobotics(function (result) {
                            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                                var availableValues = [];
                                for (var i = 0; i < result.Values.length; i++) {
                                    availableValues.push({
                                        FieldName: result.Values[i],
                                        FieldValue: result.Values[i]
                                    });
                                }

                                requestValue2DS(availableValues);

                                setTimeout(function () { reqValue2(request.FieldValue); }, 50);
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
                        }).getUserFieldAvailableValues(fieldDefinition[0]);

                        break;
                    default:
                        setReq2VisibleFields(false, false, false, true);
                        txtReqValue2(result.FieldValue);
                        break;
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
                    if (paramField() == '') {
                        if (result.SelectFields.length > 0) reqValue1(result.SelectFields[0].FieldValue);
                    } else {
                        for (var i = 0; i < result.SelectFields.length; i++) {
                            if (result.SelectFields[i].FieldValue.startsWith(paramField() + '__')) reqValue1(result.SelectFields[i].FieldValue);
                        }
                    }
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
        actualFieldDefinition('');
        requestValue2DS([]);
        setReq2VisibleFields(true, false, false, false);

        loadLists('userfields');

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
        var fieldDefinition = actualFieldDefinition().split('__');
        var fieldType = parseInt(fieldDefinition[1], 10)

        if (fieldType == 9) {
            //debemos subir un fichero
            var onWSError = function (error) {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
            }

            var onWSResult = function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.redirectAtHome();

                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roDocumentUploaded"), 'success', 2000);
                } else {
                    VTPortalUtils.utils.processErrorMessage(result);
                }
            };
            if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

            var document = documentsBlock.getDocument();

            if (VTPortal.roApp.isModeApp()) {
                wsRobotics.uploadDocumentMobile(document, -1, remarks(), "");
            } else {
                document.append("idRelatedObject", 1);
                document.append("remarks", remarks());
                document.append("docRelatedInfo", "");

                wsRobotics.uploadDocumentDesktop(document);
            }
        } else {
            var newValue = '';
            switch (fieldType) {
                case 2:
                    newValue = moment(dateReqValue2()).format('YYYY/MM/DD');
                    break;
                case 4:
                    newValue = moment(timeReqValue2()).format('HH:mm');
                    break;
                case 5:
                    newValue = reqValue2();
                    break;
                default:
                    newValue = txtReqValue2();
                    break;
            }

            var onWSError = function (error) {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
            }

            var onWSResult = function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.redirectAtRequestList(1);
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
                } else {
                    var onContinue = function () {
                        wsRobotics.saveUserFieldRequest(fieldDefinition[0], newValue, remarks(), fieldDefinition[2].toLowerCase() == 'false' ? false : true, moment().startOf('day'), true);
                    }

                    VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
                }
            };

            if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

            wsRobotics.saveUserFieldRequest(fieldDefinition[0], newValue, remarks(), fieldDefinition[2].toLowerCase() == 'false' ? false : true, moment().startOf('day'), false);
        }
    }

    function deleteRequest() {
        if (requestId() != null && requestId() > 0) VTPortalUtils.utils.deleteRequest(requestId());
    }

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_UserFieldsChange'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: i18nextko.t('roRequestUserFieldLbl'),
        lblRequestValue2: i18nextko.t('roRequestUserFieldValueLbl'),
        subscribeBlock: globalStatus(),
        scrollContent: {
            height: computedScrollHeight
        },
        popupVisible: popupVisible,
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 1, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 1, false); },
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
            onValueChanged: cmbRequestValue1Changed,
            readOnly: formReadOnly
        },
        cmbRequestValue2: {
            dataSource: requestValue2DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue2,
            visible: isReqValueCmb2,
            readOnly: formReadOnly
        },
        dateRequestValue2: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: dateReqValue2,
            visible: isReqValueDate2,
            readOnly: formReadOnly,
            valueChangeEvent: 'focusout'
        },
        timeRequestValue2: {
            type: "time",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: timeReqValue2,
            visible: isReqValueTime2,
            readOnly: formReadOnly,
            valueChangeEvent: 'focusout'
        },
        txtRequestValue2: {
            value: txtReqValue2,
            visible: isReqValueText2,
            readOnly: formReadOnly
        },
        hasDocuments: hasDocuments,
        myDocumentsBlock: documentsBlock,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};