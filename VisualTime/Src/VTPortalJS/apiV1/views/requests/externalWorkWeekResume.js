VTPortal.externalWorkWeekResume = function (params) {
    var requestId = ko.observable(null), remarks = ko.observable('');;
    var canSave = ko.observable(false), canDelete = ko.observable(false), viewHistory = ko.observable(false), popupVisible = ko.observable(false), viewActions = ko.observable(false);
    var formReadOnly = ko.observable(false), myApprovalsBlock = VTPortal.approvalHistory();
    var requestDays = ko.observable([]);

    var editGrid = ko.computed(function () { return !formReadOnly() });

    var directionsDS = [{ Name: i18nextko.i18n.t("roEntry"), Direction: 'E' }, { Name: i18nextko.i18n.t("roExit"), Direction: 'S' }];

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

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
                    if (result.ReqStatus == 0 || result.ReqStatus == 1) {
                        viewActions(true);
                        canDelete(false);
                    }
                }

                var tmpInfo = [];
                if (result.RequestDays != null && result.RequestDays.length > 0) {
                    for (var j = 0; j < result.RequestDays.length; j++) {
                        var cDay = result.RequestDays[j];

                        tmpInfo.push({ Description: '', DateTime: moment.tz(cDay.RequestDate, VTPortal.roApp.serverTimeZone).toDate(), Direction: cDay.ActualType == 1 ? 'E' : 'S', IdCause: cDay.IdCause, Remark: cDay.Remark })
                    }
                }
                requestDays(tmpInfo);
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

    function saveRequest() {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtRequestList(15);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
            } else {
                var onContinue = function () {
                    wsRobotics.saveExternalWorkWeekResume(remarks(), requestDays(), true);
                }

                VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        wsRobotics.saveExternalWorkWeekResume(remarks(), requestDays().sortBy(function (n) { return n.DateTime; }), false);
    }

    function deleteRequest() {
        if (requestId() != null && requestId() > 0) VTPortalUtils.utils.deleteRequest(requestId());
    }

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_ExternalWorkWeekResume'),
        lblPunchesInfo: i18nextko.t('roExternalWorkWeekResumeDays'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        remarks: {
            value: remarks,
            readOnly: formReadOnly
        },
        daysInfo: {
            dataSource: requestDays,
            editing: {
                mode: "form",
                allowUpdating: editGrid,
                allowDeleting: editGrid,
                allowAdding: editGrid,
                texts: {
                    deleteRow: 'Delete',
                    editRow: 'Edit',
                    //saveRowChanges: 'Save',
                    //cancelRowChanges: 'Cancel'
                }
            },
            onInitNewRow: function (e) {
                e.data = { Description: '', DateTime: moment().startOf('day').toDate(), Direction: 'E', IdCause: -1, Remark: '' }
            },
            onCellPrepared: function (e) {
                if (e.column && e.column.command == 'edit') {
                    e.cellElement.find(".dx-link:contains('Edit')").html($('<span class=\'dx-icon-edit icon\' />').height(16).width(16));
                    e.cellElement.find(".dx-link:contains('Delete')").html($('<span class=\'dx-icon-remove icon\' />').height(16).width(16));
                    //e.cellElement.find(".dx-link:contains('Save')").html($('<span class=\'dx-icon-save icon-big\' />').height(16).width(16));
                    //e.cellElement.find(".dx-link:contains('Cancel')").html($('<span class=\'dx-icon-clear icon-big\' />').height(16).width(16));
                }
            },
            columns: [
                {
                    allowEditing: false,
                    visible: true,
                    dataField: "Description",
                    caption: i18nextko.i18n.t('roGrid_Description'),
                    dataType: "string",
                    allowSorting: false,
                    formItem: {
                        visible: false
                    },
                    calculateCellValue: function (data) {
                        var causeName = ''
                        if (typeof data.IdCause != 'undefined') {
                            var tmpCauses = VTPortalUtils.causes;
                            for (var i = 0; i < tmpCauses.length; i++) {
                                if (tmpCauses[i].ID == data.IdCause) {
                                    causeName = tmpCauses[i].Name;
                                }
                            }
                        }

                        return data.Direction + ": " + moment.tz(data.DateTime, VTPortal.roApp.serverTimeZone).format('DD/MM/YYYY HH:mm') + (causeName != '' ? ' - ' + causeName : '') + (data.Remark != '' ? '(' + data.Remark + ')' : '');
                    }
                }, {
                    sortIndex: 1,
                    sortOrder: 'asc',
                    allowEditing: formReadOnly,
                    visible: false,
                    dataField: "DateTime",
                    caption: i18nextko.i18n.t('roGrid_DateTime'),
                    dataType: "date",
                    format: 'dd/MM/yyyy HH:mm',
                    editorOptions: {
                        type: "datetime",
                        pickerType: VTPortalUtils.utils.datetimeTypeSelect()
                    }
                }, {
                    allowEditing: formReadOnly,
                    visible: false,
                    dataField: "Direction",
                    caption: i18nextko.i18n.t('roGrid_Direction'),
                    lookup: {
                        dataSource: directionsDS,
                        displayExpr: "Name",
                        valueExpr: "Direction"
                    }
                }, {
                    allowEditing: formReadOnly,
                    visible: false,
                    dataField: "IdCause",
                    caption: i18nextko.i18n.t('roGrid_Cause'),
                    lookup: {
                        dataSource: VTPortalUtils.causes,
                        displayExpr: "Name",
                        valueExpr: "ID"
                    }
                }, {
                    allowEditing: formReadOnly,
                    visible: false,
                    dataField: "Remark",
                    caption: i18nextko.i18n.t('roGrid_Remark'),
                }],
            readOnly: formReadOnly
        },
        subscribeBlock: globalStatus(),
        scrollContent: {
            height: computedScrollHeight
        },
        popupVisible: popupVisible,
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 15, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 15, false); },
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
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};