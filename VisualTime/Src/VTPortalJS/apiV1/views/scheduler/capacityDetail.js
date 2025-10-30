VTPortal.capacityDetail = function (params) {
    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: VTPortal.roApp.capacityEmployees,
        searchOperation: "contains",
        searchExpr: ["Name", "Group"]
    }));
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var loadingVisible = ko.observable(false);
    var selectedDate = ko.observable(new Date());
    var titleCapacity = ko.observable('');
    var onSuccessLoad = function () {
        new WebServiceRobotics(function (result) {
            loadingVisible(false);
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var employeesInfo = result.Employees;
                for (var i = 0; i < employeesInfo.length; i++) {
                    if (employeesInfo[i].Image == "") employeesInfo[i].Image = result.DefaultImage;
                }

                VTPortal.roApp.capacityEmployees = result.Employees;

                dataSource(new DevExpress.data.DataSource({
                    store: employeesInfo,
                    searchOperation: "contains",
                    searchExpr: ["Name", "Group"]
                }));
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        }).getCapacityDetail(selectedDate());
    };
    function viewShown() {
        globalStatus().viewShown();
        var selDate = moment(params.id, "YYYY-MM-DD");
        titleCapacity(i18nextko.i18n.t('rocapacityDetail') + ' ' + params.id)
        selectedDate(selDate.toDate());
        loadingVisible(true);

        if (!VTPortal.roApp.loggedIn) {
            loadingVisible(false);
            VTPortalUtils.utils.loginIfNecessary('', onSuccessLoad);
        } else {
            onSuccessLoad();
        }
    }

    var viewModel = {
        viewShown: viewShown,
        title: titleCapacity,
        subscribeBlock: globalStatus(),
        listOptions: {
            dataSource: dataSource,
            grouped: false,
            indicateLoading: false,
            pageLoadMode: 'nextButton',
            height: "90%",
            itemTemplate: 'item'
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
        loadingPanel: {
            showPane: false,
            height: 110,
            shading: true,
            shadingColor: 'rgba(255,255,255,0.5)',
            indicatorSrc: 'Images/DoubleRing.gif',
            visible: loadingVisible
        }
    };

    return viewModel;
};