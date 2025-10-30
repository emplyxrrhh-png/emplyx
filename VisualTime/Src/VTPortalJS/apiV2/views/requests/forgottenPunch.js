VTPortal.forgottenPunch = function (params) {
    var requestId = ko.observable(null), remarks = ko.observable('');
    var canSave = ko.observable(false), canDelete = ko.observable(false), viewHistory = ko.observable(false), popupVisible = ko.observable(false), viewActions = ko.observable(false);

    var bCustomizeFinish = ko.observable(false), bCompleteTask = ko.observable(false);
    var taskFieldsBlock = VTPortal.taskFieldsInput();

    var taskComboVisible = ko.observable(true);
    var ListTasksVisible = ko.observable(false);
    var taskSearchVisible = ko.observable(false);
    var taskSearchVisible2 = ko.observable(false);
    var searchCriteria = ko.observable("");
    var tcEnabled = ko.observable(null);
    var forgottenPunchType = ko.observable(null);
    var forgottenPunchTypeDS = ko.observable([]);
    var forgottenCauseLbl = ko.computed(function () {
        if (forgottenPunchType() != null) {
            switch (forgottenPunchType()) {
                case 1:
                    return i18nextko.i18n.t('roRequestCauseLbl');
                    break;
                case 2:
                    return i18nextko.i18n.t('roRequestTaskLbl');
                    break;
                case 3:
                    return i18nextko.i18n.t('roRequestCostCenersLbl');
                    break;
            }
        }
    });
    var tmpAvailablePunches = [];
    if (!VTPortal.roApp.impersonateOnlyRequest) {
        if ($.grep(VTPortal.roApp.empPermissions().Requests, function (e) { return (e.RequestType == 2 && e.Permission == true); }).length == 1) tmpAvailablePunches.push({ "ID": 1, "Name": i18nextko.i18n.t('roForgottenType_Presence') });
        if ($.grep(VTPortal.roApp.empPermissions().Requests, function (e) { return (e.RequestType == 10 && e.Permission == true); }).length == 1) tmpAvailablePunches.push({ "ID": 2, "Name": i18nextko.i18n.t('roForgottenType_Task') });
        if ($.grep(VTPortal.roApp.empPermissions().Requests, function (e) { return (e.RequestType == 12 && e.Permission == true); }).length == 1) tmpAvailablePunches.push({ "ID": 3, "Name": i18nextko.i18n.t('roForgottenType_CostCenter') });
    } else {
        tmpAvailablePunches.push({ "ID": 1, "Name": i18nextko.i18n.t('roForgottenType_Presence') });
        tmpAvailablePunches.push({ "ID": 2, "Name": i18nextko.i18n.t('roForgottenType_Task') });
        tmpAvailablePunches.push({ "ID": 3, "Name": i18nextko.i18n.t('roForgottenType_CostCenter') });
    }

    forgottenPunchTypeDS(tmpAvailablePunches);
    var tcType = ko.observable('');
    var iDate = ko.observable(moment().startOf('day').toDate());
    var eTaskHour = ko.observable(moment().startOf('day').toDate());

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));
    if (typeof params.param != 'undefined' && parseInt(params.param, 10) != -1) iDate(moment(params.param, 'YYYY-MM-DD'));
    if (typeof params.iDate != 'undefined' && parseInt(params.iDate, 10) != -1) tcType(params.iDate);

    var formReadOnly = ko.observable(false);

    var reqValue1 = ko.observable(null), reqValue2 = ko.observable(null), tcTypeValue = ko.observable(null), continueWith = ko.observable(null);

    var taskSearched1 = ko.observable(null);
    var taskSearched2 = ko.observable(null);
    var buttonText1 = ko.observable(i18nextko.i18n.t('roSelectTask'));
    var buttonText2 = ko.observable(i18nextko.i18n.t('roSelectTask'));
    var searchFrom = ko.observable(0);
    var tcTypesDS = ko.observable([]);
    var requestValue1DS = ko.observable([]);
    var requestValue2DS = ko.observable([]), cmbContinueWithDS = ko.observable([]);

    var myApprovalsBlock = VTPortal.approvalHistory();

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    var curRequestInfo = null;

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: [],
        searchOperation: "contains",
        searchExpr: ["FieldName", "RelatedInfo"]
    }));

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                curRequestInfo = result;

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
                iDate(moment(result.strDate1,"YYYY-MM-DD HH:mm:ss").toDate());

                forgottenPunchType();

                switch (result.RequestType) {
                    case 2:
                        forgottenPunchType(1);
                        reloadRequestAvailableInfo(1);
                        break;
                    case 10:
                        forgottenPunchType(2);
                        reloadRequestAvailableInfo(2);
                        break;
                    case 12:
                        forgottenPunchType(3);
                        reloadRequestAvailableInfo(3);
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

    var searchTasks = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                dataSource(new DevExpress.data.DataSource({
                    store: result.SelectFields,
                    searchOperation: "contains",
                    searchExpr: ["FieldName", "RelatedInfo"]
                }));
            } else {
                switch (result.Status) {
                    case -10:
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('lblTaskNotFound'), 'warning', 5000); break;
                    case -20:
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('lblTaskTooMany'), 'warning', 5000); break;
                }
            }
        }).getTasksByName(searchCriteria());
    }

    var loadLists = function (lstdataSource) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                requestValue2DS(new DevExpress.data.DataSource({
                    store: result.SelectFields,
                    paginate: true,
                    pageSize: 50,
                    pageLoadMode: "scrollBottom"
                }));

                cmbContinueWithDS(new DevExpress.data.DataSource({
                    store: result.SelectFields.clone(),
                    paginate: true,
                    pageSize: 50,
                    pageLoadMode: "scrollBottom"
                }));

                if (requestId() != null && requestId() > 0 && curRequestInfo != null) {
                    switch (forgottenPunchType()) {
                        case 1:
                            reqValue1(curRequestInfo.Field1 + '');
                            reqValue2(curRequestInfo.IdCause + '');
                            break;
                        case 2:
                            reqValue2(curRequestInfo.IdTask + '');
                            bCompleteTask(curRequestInfo.CompletedTask);
                            bCustomizeFinish(curRequestInfo.IdTask2 >= 0);
                            eTaskHour(moment(curRequestInfo.strDate2, "YYYY-MM-DD HH:mm:ss").toDate());
                            continueWith(curRequestInfo.IdTask2 + '');
                            //Si hay una tarea indicando que se personaliza el cambio debemos pedir los campos
                            if (curRequestInfo.IdTask2 >= 0) reloadCustomizeFinishData();

                            break;
                        case 3:
                            reqValue2(curRequestInfo.IdCenter + '');
                            break;
                    }
                } else {
                    if (result.SelectFields.length > 0 && formReadOnly() == false) reqValue2(result.SelectFields[0].FieldValue);
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
        forgottenPunchType(forgottenPunchTypeDS()[0].ID);

        requestValue2DS(new DevExpress.data.DataSource({
            store: []
        }));

        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Telecommuting && VTPortal.roApp.userTelecommute() != null && VTPortal.roApp.userTelecommute().Telecommuting) {
            tcEnabled(1);
        }
        else {
            tcEnabled(0);
        }

        tcTypesDS([{ FieldName: i18nextko.i18n.t("roOffice"), FieldValue: '0' }, { FieldName: i18nextko.i18n.t("roHome"), FieldValue: '1' }]);
        requestValue1DS([{ FieldName: i18nextko.i18n.t("roEntry"), FieldValue: 'E' }, { FieldName: i18nextko.i18n.t("roExit"), FieldValue: 'S' }]);
        reqValue1('E');
        tcTypeValue('0');

        if (requestId() != null && requestId() > 0) {
            formReadOnly(true);
            loadRequest(requestId());
            VTPortalUtils.utils.markRequestAsRead(requestId());
        } else {
            reloadRequestAvailableInfo(forgottenPunchType());
        }

        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (requestId() != null) canDelete(true);
        } else {
            if (requestId() != null) viewActions(false);
        }

        if (requestId() == null) canSave(true);
        if (requestId() != null) viewHistory(true);
    }

    function reloadRequestAvailableInfo(reqType) {
        switch (reqType) {
            case 1:
                loadLists('causes.readerinputcode');
                break;
            case 2:
                if (typeof VTPortal.roApp.userStatus().AvailableTasks != 'undefined' && VTPortal.roApp.userStatus().AvailableTasks < 200) {
                    taskComboVisible(true);
                    taskSearchVisible(false);
                    taskSearchVisible2(false);
                    loadLists('tasks.availabletasks');
                }
                else {
                    taskComboVisible(false);
                    taskSearchVisible(true);
                    taskSearchVisible2(true);
                    requestValue2DS([]);
                }
                break;
            case 3:
                loadLists('costcenters');
                break;
        }
    }

    var wsRobotics = null;

    function saveRequest() {
        var punchDateTime = moment(moment(iDate()).format("YYYY-MM-DD HH:mm:ss"));
        if (punchDateTime <= moment()) {
            switch (forgottenPunchType()) {
                case 1:
                    savePresenceRequest();
                    break;
                case 2:
                    saveTaskRequest();
                    break;
                case 3:
                    saveCostCenterRequest();
                    break;
            }

            //reseteamos las alertas por si hemos guardado un fichaje olvidado que teniamos como alerta.
            VTPortal.roApp.db.settings.markForRefresh(['notifications','punches','status']);
        }
        else {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingFuturePunch"), 'error', 0);
        }
    }

    function savePresenceRequest() {
        if (reqValue1() == 'S') {
            tcTypeValue('0');
        }
        var punchDateTime = moment(moment(iDate()).format("YYYY-MM-DD HH:mm:ss"));
        var causeId = reqValue2() != null ? reqValue2() : '-1';

        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                if (result.PunchWithoutRequest == true) {
                    VTPortal.roApp.redirectAtHome();
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("lblPunchDone"), 'success', 2000);
                }
                else {
                VTPortal.roApp.redirectAtRequestList(2);
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
                }
            } else {
                var onContinue = function () {
                    wsRobotics.saveForbbidenPunch(punchDateTime, causeId, remarks(), reqValue1(), true, tcTypeValue());
                }

                VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        wsRobotics.saveForbbidenPunch(punchDateTime, causeId, remarks(), reqValue1(), false, tcTypeValue());
    }

    function saveTaskRequest() {
        var punchDateTime = moment(iDate()).format("YYYY-MM-DD HH:mm");
        var idNewTask = reqValue2() == null ? -1 : reqValue2();
        var idContinueOnTask = continueWith() == null ? -1 : continueWith();

        var onChangeHour = moment(iDate()).format("YYYY-MM-DD") + " " + moment(eTaskHour()).format("HH:mm");
        var finalizeTask = bCompleteTask();

        taskFieldsBlock.savePunchInfo();
        var taskValue = VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue;

        if (bCustomizeFinish()) {
            momentEndHour = moment(iDate()).format("YYYY-MM-DD") + " " + moment(eTaskHour()).format("HH:mm");
        }

        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtRequestList(10);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
            } else {
                var onContinue = function () {
                    wsRobotics.SaveRequest_ForbiddenTaskPunch(punchDateTime, idNewTask, onChangeHour, idContinueOnTask, bCompleteTask(), remarks(), taskValue[0], taskValue[1], taskValue[2], taskValue[3], taskValue[4], taskValue[5], true, tcTypeValue());
                }
                VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        wsRobotics.SaveRequest_ForbiddenTaskPunch(punchDateTime, idNewTask, onChangeHour, idContinueOnTask, bCompleteTask(), remarks(), taskValue[0], taskValue[1], taskValue[2], taskValue[3], taskValue[4], taskValue[5], false, tcTypeValue());
    }

    function saveCostCenterRequest() {
        var punchDateTime = moment(moment(iDate()).format("YYYY-MM-DD HH:mm:ss"));
        var idCostCenter = reqValue2() == null ? -1 : reqValue2();

        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtRequestList(12);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
            } else {
                var onContinue = function () {
                    wsRobotics.saveCostCenterPunch(punchDateTime, idCostCenter, remarks(), true, tcTypeValue());
                }

                VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        wsRobotics.saveCostCenterPunch(punchDateTime, idCostCenter, remarks(), false, tcTypeValue());
    }

    function deleteRequest() {
        if (requestId() != null && requestId() > 0) VTPortalUtils.utils.deleteRequest(requestId());
    }

    var listItemClick = function (info) {
        if (forgottenPunchType() == 2 && !formReadOnly()) {
            if (searchFrom() == 0) {
                reqValue2(info.itemData.FieldValue.toNumber());
                taskSearched1(info.itemData);
                reloadCustomizeFinishData();
                buttonText1(info.itemData.FieldName);
            }
            else {
                continueWith(info.itemData.FieldValue.toNumber());
                taskSearched2(info.itemData);
                buttonText2(info.itemData.FieldName);
            }

            ListTasksVisible(false);
        }
    }

    function reloadCustomizeFinishData() {
        VTPortal.roApp.taskPunchRequest({
            currentAction: -2,
            newAction: -1,
            currentTaskUserFieldsDefinition: null,
            currentTaskUserFieldsValue: ['', '', '', -1, -1, -1],
            newTaskUserFieldsDefinition: null,
            newTaskUserFieldsValue: ['', '', '', -1, -1, -1]
        });

        if (formReadOnly() && curRequestInfo != null) {
            VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[0] = curRequestInfo.Field1;
            VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[1] = curRequestInfo.Field2;
            VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[2] = curRequestInfo.Field3;
            VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[3] = curRequestInfo.Field4;
            VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[4] = curRequestInfo.Field5;
            VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue[5] = curRequestInfo.Field6;
        }

        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsDefinition = result.Fields;
                var bAskInfo = false;
                for (var i = 0; i < result.Fields.length; i++) {
                    if (result.Fields[i].Used) {
                        bAskInfo = true;
                    }
                }

                if (bAskInfo) taskFieldsBlock.initTaskProperties(formReadOnly());
            } else {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingTaskInfo"), 'error', 0, onContinue);
            }
        }).getTaskUserFieldsAction(VTPortalUtils.taskAction.change, reqValue2());
    }

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_ForbiddenPunch'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblForgottenPunchType: i18nextko.t('roForgottenPunchType'),
        lblRequestValue1: i18nextko.t('roRequestDirectionLbl'),
        lblRequestValue2: forgottenCauseLbl,
        lblInitialDate: i18nextko.t('roRequestDateHourLbl'),
        subscribeBlock: globalStatus(),
        scrollContent:{
            height: computedScrollHeight
        },
        popupVisible: popupVisible,
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 2, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 2, false); },
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
        beginDate: {
            type: "datetime",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: iDate,
            readOnly: formReadOnly
        },
        tcEnabled: tcEnabled ,
        forgottenPunchType: forgottenPunchType,
        taskComboVisible: taskComboVisible,
        cmbforgottenPunchType: {
            dataSource: forgottenPunchTypeDS,
            displayExpr: "Name",
            valueExpr: "ID",
            value: forgottenPunchType,
            readOnly: formReadOnly,
            onValueChanged: function (data) {
                if (!formReadOnly()) reloadRequestAvailableInfo(data.value);
            }
        },
        reqValue1: reqValue1,
        cmbRequestValue1: {
            dataSource: requestValue1DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue1,
            readOnly: formReadOnly
        },
        cmbTCType: {
            dataSource: tcTypesDS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: tcTypeValue,
            readOnly: formReadOnly
        },
        lblTaskInput: i18nextko.t('roSearchTask'),
        cmbRequestValue2: {
            dataSource: requestValue2DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue2,
            readOnly: formReadOnly,
            visible: taskComboVisible,
            onValueChanged: function (data) {
                if (forgottenPunchType() == 2 && !formReadOnly()) {
                    reloadCustomizeFinishData();
                }
            }
        },
        listOptions: {
            dataSource: dataSource,
            itemTemplate: 'item',
            noDataText: "",
            scrollingEnabled: true,
            height: 300,
            onItemClick: function (data) {
                 listItemClick(data);
            }
        },
        searchOptions: {
            valueChangeEvent: "keyup",
            mode: "search",
            height: "10%",
            onValueChanged: function (args) {
                searchCriteria(args.value);
            }
        },
        btnAcceptText: {
            ID: "btnAcceptTextId",
            visible: taskSearchVisible,
            onClick: function () {
                searchFrom(0);
                ListTasksVisible(true);
            },
            text: buttonText1,
        },
        btnAcceptText2: {
            iD: "btnAcceptTextId2",
            visible: taskSearchVisible2,
            onClick: function () {
                searchFrom(1);
                ListTasksVisible(true);
            },
            text: buttonText2,
        },
        btnAcceptText3: {
            onClick: function () { searchTasks() },
            text: i18nextko.t('roSearchTask'),
        },
        lblTC: i18nextko.t('roTCWhereF'),
        lblContinueWith: i18nextko.t('rolblContinueWith'),
        finishVisible: bCustomizeFinish,
        cmbContinueWith:{
            dataSource: cmbContinueWithDS,
            visible: taskComboVisible,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: continueWith,
            readOnly: formReadOnly
        },
        lblEndTaskHour: i18nextko.t('rolblEndTaskHour'),
        endTaskHour:{
            type: "time",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: eTaskHour,
            readOnly: formReadOnly,
            valueChangeEvent: 'focusout'
        },
        taskFieldsBlock: taskFieldsBlock,
        ckCompleteTask:{
            text: i18nextko.t('roCompleteTaks'),
            value: bCompleteTask,
            readOnly: formReadOnly
        },
        ListTasksVisible: ListTasksVisible,
        ckCustomizeFinish: {
            text: i18nextko.t('roCustomizeFinishLbl'),
            value: bCustomizeFinish,
            readOnly: formReadOnly
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};