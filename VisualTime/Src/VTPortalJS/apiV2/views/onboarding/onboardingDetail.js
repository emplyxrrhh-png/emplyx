VTPortal.onboardingDetail = function (params) {
    var taskDS = ko.observable();

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        searchOperation: "contains",
        searchExpr: ["TaskName"]
    }));

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var getonboarding = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var taskInfo = result.Tasks;
                taskDS(true);

                dataSource(new DevExpress.data.DataSource({
                    store: taskInfo,
                    sort: "Done",
                    searchOperation: "contains",
                    searchExpr: ["TaskName"]
                }));
            } else {
                taskDS(false);
                VTPortalUtils.utils.processErrorMessage(result);
            }
        }).getOnBoardingtasksById(params.id);
    }

    function changeStatus(done, idTask, idList) {
        var status = false;
        if (done == false) { status = true; }
        else { status = false; }
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var taskInfo = result.Tasks;
                taskDS(true);

                dataSource(new DevExpress.data.DataSource({
                    store: taskInfo,
                    sort: "Done",
                    searchOperation: "contains",
                    searchExpr: ["TaskName"]
                }));
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        }).updateTaskStatus(status, idTask, idList);
    }
    function taskItem(data, index, element) {
        var listItem = $('<div>').attr('class', 'listMenuItemContentOnb');

        if (data.Done == true) {
            listItem.append(
                $("<div />").attr('style', 'float:left;margin-top: 10px;').dxCheckBox({
                    value: data.Done
                }));
            listItem.append($('<div>').attr('style', 'margin-left: 50px; margin-top: -5px;font-size: 13px;').text(data.TaskName));
            listItem.append($('<div>').attr('class', 'smallTextDescriptionOnb').attr('style', 'float:right;').text(data.SupervisorName));
            listItem.append($('<div>').attr('class', 'smallTextDescriptionOnb').attr('style', 'margin-left:50px;').text(data.LastChangeDate));
        }
        else {
            listItem.append(
                $("<div />").attr('style', 'float:left;').dxCheckBox({
                    value: data.Done
                }));
            listItem.append($('<div>').attr('style', 'margin-left: 50px; margin-top: -5px;font-size: 13px;').text(data.TaskName));
        }
        element.append(listItem);
    }

    function viewShown() {
        globalStatus().viewShown();
        getonboarding();
    };

    var viewModel = {
        viewShown: viewShown,
        title: "OnBoarding",
        subscribeBlock: globalStatus(),
        taskDS: taskDS,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        commScroll: {
            height: '70%',
            onPullDown: function (options) {
                getonboarding();
            }
        },
        listtasks: {
            dataSource: dataSource,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: taskItem,
            noDataText: "Todavía no se han creado tareas",
            onItemClick: function (info) {
                changeStatus(info.itemData.Done, info.itemData.Id, info.itemData.IdList)
            }
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
    };

    return viewModel;
};