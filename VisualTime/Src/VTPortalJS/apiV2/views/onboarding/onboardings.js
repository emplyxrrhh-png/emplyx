VTPortal.onboardings = function (params) {
    var onboardingDS = ko.observable(false);

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        searchOperation: "contains",
        searchExpr: ["EmployeeName", "Comments", "Status"]
    }));

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function generateonboardingsAll() {
        var onboardings = [];

        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var onboardingsInfo = result.OnBoardings;

                for (var i = 0; i < onboardingsInfo.length; i++) {
                    onboardingsInfo[i].StartDate = moment(onboardingsInfo[i].StartDate).format("DD-MM-YYYY");
                }

                dataSource(new DevExpress.data.DataSource({
                    store: onboardingsInfo,
                    sort: "EmployeeName",
                    searchOperation: "contains",
                    searchExpr: ["EmployeeName", "Comments", "Status"]
                }));
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        }).getMyOnBoardings();

        if (onboardings.length > 0) {
            onboardingDS(true);
        }
        else {
            onboardingDS(true);
        }

        return onboardings;
    };

    function viewShown() {
        globalStatus().viewShown();
        generateonboardingsAll();
    };

    function progressBar(data, index, element) {
        if (data.Status != null) {
            var divisiones = data.Status.split("/");

            var a = $("<div style='background:" + data.Image + "' class='listMenuItemIcon EmployeeImagePhoto' />");

            var listItem = $('<div>').attr('class', 'listMenuItemContent');
            listItem.append($('<div>').attr('class', data.cssClassText).text(data.EmployeeName));
            listItem.append($('<span>').attr('style', 'float:right;margin-top: 6px;font-size: 11px;').text(data.StartDate));
            listItem.append(
                $("<div />").dxProgressBar({
                    min: 0,
                    max: parseInt(divisiones[1], 10),
                    value: parseInt(divisiones[0], 10),
                    width: 150,
                    statusFormat: function (ratio, value) { return 'Estado: ' + data.Status + ' (' + Math.round(ratio * 100) + '%' + ')' }
                }));
            listItem.append($('<div>').attr('class', 'smallTextDescriptionOnb').text(data.Comments));
            element.append(a, listItem);
        }
    }

    var viewModel = {
        viewShown: viewShown,
        title: "OnBoarding",
        subscribeBlock: globalStatus(),
        onboardingDS: onboardingDS,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        commScroll: {
            height: '70%',
            onPullDown: function (options) {
                generateonboardingsAll();
            }
        },
        listProgress: {
            min: 0,
            max: 100,
            value: 63
        },
        listonboardings: {
            dataSource: dataSource,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: progressBar,
            onItemClick: function (info) {
                VTPortal.app.navigate('onboardingDetail/' + info.itemData.IdList);
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