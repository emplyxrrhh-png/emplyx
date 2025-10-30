VTPortal.taskSelector = function (params) {
    var taskFieldsVisible = ko.observable(false);
    var searchCriteria = ko.observable("");
    var taskFieldsBlock = VTPortal.taskFieldsInput();
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var onshow = function () {
        globalStatus().viewShown();
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
                        dataSource(new DevExpress.data.DataSource({
                            store: result.SelectFields,
                            searchOperation: "contains",
                            searchExpr: ["FieldName", "RelatedInfo"]
                        }));
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('lblTaskNotFound'), 'warning', 5000); break;
                    case -20:
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('lblTaskTooMany'), 'warning', 5000); break;
                }
            }
        }).getTasksByName(searchCriteria());
    }

    var tcType = ko.observable('');

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: [],
        searchOperation: "contains",
        searchExpr: ["FieldName", "RelatedInfo"]
    }));

    var listItemClick = function (info) {
        taskActionClick(params.id, info)
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

        var taskName = curTaskInfo.info.itemData.FieldName;
        if (curTaskInfo.info.itemData.FieldValue.toNumber() == 0) taskName = taskName.replaceAll('(', '').replaceAll(')', '');
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
        title: i18nextko.t('roSearchTask'),
        subscribeBlock: globalStatus(),
        listOptions: {
            dataSource: dataSource,
            height: "70%",
            itemTemplate: 'item',
            noDataText: "",
            onItemClick: listItemClick
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
            onClick: function () { searchTasks() },
            text: i18nextko.t('roSearchTask'),
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