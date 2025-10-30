VTPortal.noPermissions = function (params) {
    var viewModel = {
        showPermissionsIcon: VTPortal.roApp.ShowPermissionsIcon(),
        noDataToShow: i18nextko.i18n.t('noDataToShow'),
    };

    return viewModel;
};