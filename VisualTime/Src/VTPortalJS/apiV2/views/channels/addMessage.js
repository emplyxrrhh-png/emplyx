VTPortal.addMessage = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function saveMessage() {
    }

    function viewShown() {
        VTPortal.roApp.db.settings.markForRefresh(['channels', "status"]);
        globalStatus().viewShown();
    };

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roMyDailyRecord'),
        subscribeBlock: globalStatus(),
        btnSave: {
            onClick: saveMessage,
            text: '',
            hint: i18nextko.i18n.t('roSaveRequest'),
            icon: "Images/Common/save.png"
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};