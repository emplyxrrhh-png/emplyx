VTPortal.SelectListObject = function (params) {
    var taskFieldsVisible = ko.observable(false);
    var taskFieldsBlock = VTPortal.taskFieldsInput();
    var curTaskAction = '';
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var isTaskL = ko.observable(params.id);
    var onshow = function () {
        globalStatus().viewShown();
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                dataSource(new DevExpress.data.DataSource({
                    store: result.SelectFields,
                    searchOperation: "contains",
                    searchExpr: ["FieldName", "RelatedInfo"]
                }));
            } else {
            }
        }).getGenericList(params.id);
    }

    var lngTitle = params.id.replaceAll('.', '');
    var tcType = ko.observable('');
    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) tcType(params.iDate);

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: [],
        searchOperation: "contains",
        searchExpr: ["FieldName", "RelatedInfo"]
    }));

    var listItemClick = function (info) {
        VTPortal.roApp.SelectedCause(info.itemData.FieldValue.toNumber());
        switch (params.param) {
            case "TCH":
            case "TCO":
                taskActionClick(params.param, info)
                break;
            case "C":
                window.VTPortalUtils.utils.doPunch('cc', { idCenter: info.itemData.FieldValue.toNumber(), tcType: tcType() });
                break;
        }
    }

    var taskActionClick = function (action, info) {
        curTaskInfo = { 'action': action, 'info': info };

        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.taskPunchRequest().newAction = action;
                VTPortal.roApp.taskPunchRequest().newTaskUserFieldsDefinition = result.Fields;

                var bAskInfo = false;
                for (var i = 0; i < result.Fields.length; i++) {
                    if (result.Fields[i].Used) {
                        bAskInfo = true;
                    }
                }

                if (!bAskInfo) {
                    var taskName = info.itemData.FieldName;
                    if (info.itemData.FieldValue.toNumber() == 0) taskName = taskName.replaceAll('(', '').replaceAll(')', '');
                    if (taskName.indexOf(':') == 0) taskName = taskName.substring(1);

                    var iTaskName = VTPortal.roApp.userStatus().TaskTitle;
                    if (iTaskName.indexOf(':') == 0) iTaskName = iTaskName.substring(1);

                    var msg = i18nextko.i18n.t('roTask_ChangeConfirm');
                    msg = msg.replace('{{initialTask}}', iTaskName);
                    msg = msg.replace('{{taskName}}', taskName);

                    VTPortalUtils.utils.questionMessage(msg, 'info', 0, function () {
                        switch (action) {
                            case "TCH":
                                window.VTPortalUtils.utils.doPunch('task', { idTask: VTPortal.roApp.userStatus().TaskId, idNewTask: info.itemData.FieldValue.toNumber(), oldValues: VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue, newValues: VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue, complete: false, tcType: tcType() });
                                break;
                            case "TCO":
                                window.VTPortalUtils.utils.doPunch('task', { idTask: VTPortal.roApp.userStatus().TaskId, idNewTask: info.itemData.FieldValue.toNumber(), oldValues: VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue, newValues: VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue, complete: true, tcType: tcType() });
                                break;
                        }
                    }, function () { });
                } else {
                    taskFieldsBlock.initTaskProperties(false);
                    taskFieldsVisible(true);
                }
            } else {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingTaskInfo"), 'error', 0, onContinue);
            }
        }).getTaskUserFieldsAction(VTPortalUtils.taskAction.begin, info.itemData.FieldValue.toNumber());
    }

    var continuePunch = function () {
        taskFieldsVisible(false);
        taskFieldsBlock.savePunchInfo();

        var taskName = curTaskInfo.info.itemName;
        if (curTaskInfo.info.itemData.toNumber() == 0) taskName = taskName.replaceAll('(', '').replaceAll(')', '');
        if (taskName.indexOf(':') == 0) taskName = taskName.substring(1);

        var iTaskName = VTPortal.roApp.userStatus().TaskTitle;
        if (iTaskName.indexOf(':') == 0) iTaskName = iTaskName.substring(1);

        var msg = i18nextko.i18n.t('roTask_ChangeConfirm');
        msg = msg.replace('{{initialTask}}', iTaskName);
        msg = msg.replace('{{taskName}}', taskName);

        VTPortalUtils.utils.questionMessage(msg, 'info', 0, function () {
            switch (curTaskInfo.action) {
                case "TCH":
                    window.VTPortalUtils.utils.doPunch('task', { idTask: VTPortal.roApp.userStatus().TaskId, idNewTask: curTaskInfo.info.itemData.FieldValue.toNumber(), oldValues: VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue, newValues: VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue, complete: false, tcType: tcType() });
                    break;
                case "TCO":
                    window.VTPortalUtils.utils.doPunch('task', { idTask: VTPortal.roApp.userStatus().TaskId, idNewTask: curTaskInfo.info.itemData.FieldValue.toNumber(), oldValues: VTPortal.roApp.taskPunchRequest().currentTaskUserFieldsValue, newValues: VTPortal.roApp.taskPunchRequest().newTaskUserFieldsValue, complete: true, tcType: tcType() });
                    break;
            }
        }, function () { });
    }

    var viewModel = {
        viewShown: onshow,
        title: i18nextko.t(lngTitle),
        subscribeBlock: globalStatus(),
        isTaskL: isTaskL,
        listOptions: {
            dataSource: dataSource,
            height: "70%",
            itemTemplate: 'item',
            onItemClick: listItemClick
        },
        searchOptions: {
            valueChangeEvent: "keyup",
            mode: "search",
            height: "10%",
            onValueChanged: function (args) {
                dataSource().searchValue(args.value);
                dataSource().load();
            }
        },
        taskFieldsVisible: taskFieldsVisible,
        taskFieldsBlock: taskFieldsBlock,
        btnCancelTaskFields: {
            onClick: function () { taskFieldsVisible(false) },
            text: i18nextko.t('roCancelCaptcha'),
        },
        btnAcceptUserFields: {
            onClick: function () { continuePunch() },
            text: i18nextko.t('roContinue'),
        },
    };

    return viewModel;
};