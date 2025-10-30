VTPortal.loading = function (params) {
    //var loadingTitle = ko.computed(function () {
    //    return '';
    //});

    function viewShown() {
        if (typeof params.id != 'undefined' && params.id == 'fromLogin') {
            setTimeout(function () { VTPortal.app.navigate("login", { root: true }); }, 500)
        }
        else {
            setTimeout(function () {
                VTPortal.roApp.loadInitialData(true, true, true, true, true, function () {
                    if (VTPortal.roApp.db.settings.onlySupervisor) {
                        VTPortal.roApp.redirectAtHome(false);
                    } else {
                        VTPortal.roApp.redirectAtHome(true);
                    }
                })
            }, 500)
        }
    };

    var viewModel = {
        viewShown: viewShown,
        loadingPanel: {
            message: i18nextko.i18n.t("roApplyingLanguage"),
            showPane: true,
            shading: true,
            height: 150,
            shadingColor: 'rgba(0,0,0,0)',
            indicatorSrc: 'Images/loader_v4.gif',
            visible: true
        }
    };

    return viewModel;
};