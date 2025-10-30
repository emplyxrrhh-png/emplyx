VTPortal.myChannels = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var channelsDS = ko.observable([]);

    const defaultCCUrl = VTPortal.roApp.serverURL.ssoURL;
    var complaintChannelUrl = ko.observable(defaultCCUrl);

    function viewShown() {
        VTPortal.roApp.db.settings.markForRefresh(['channels', "status"]);
        globalStatus().viewShown();
        loadChannels();
    };

    function loadChannels() {
        //channelsDS = VTPortal.roApp.dataCatalogs.getChannelsDS();
        if (VTPortal.roApp.loggedIn) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var channels = result.Value;
                    for (var i = 0; i < channels.length; i++) {
                        channels[i]['hasAction'] = true;
                        if (channels[i].IsComplaintChannel) {
                            channels[i]['cssClass'] = 'dx-channelComplaint';
                            channels[i]['MessagesInfo'] = i18nextko.i18n.t('compliantChannelDesc');
                        } else {
                            if (channels[i].NewMessages == 0) channels[i]['MessagesInfo'] = i18nextko.i18n.t('nomessages');
                            else channels[i]['MessagesInfo'] = i18nextko.i18n.t('newmessages') + channels[i].NewMessages + ' ' + i18nextko.i18n.t('newmessages2');
                            channels[i]['cssClass'] = 'dx-channelCommunications';
                        }
                    }
                    channelsDS(channels);
                } else {
                    channelsDS([]);
                    VTPortalUtils.utils.processErrorMessage(result, function () { });
                }
            }, function (error) {
                channelsDS([]);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingChannels"), 'error', 0, function () { });
            }).getMyChannels();

            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    if (result.Value != null && result.Value.length > 0) complaintChannelUrl(defaultCCUrl + result.Value);
                } else {
                    complaintChannelUrl(defaultCCUrl);
                    VTPortalUtils.utils.processErrorMessage(result, function () { });
                }
            }, function (error) {
                complaintChannelUrl(defaultCCUrl);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingComplaintChannelUrl"), 'error', 0, function () { });
            }).getComplaintChannelUrl();
        }
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roMyDailyRecord'),
        subscribeBlock: globalStatus(),
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        channelsDS: channelsDS,
        listChannels: {
            dataSource: channelsDS,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'AlertItem',
            onItemClick: function (info) {
                if (info.itemData.IsComplaintChannel) {
                    if (complaintChannelUrl() != defaultCCUrl && complaintChannelUrl().length > 0) {
                        VTPortal.roApp.openURLonExternalBrowser(complaintChannelUrl()); 
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingComplaintChannelUrl"), 'error', 0, function () { });
                    }
                } else {
                    VTPortal.app.navigate('channelConversations/' + info.itemData.Id);
                }
            }
        }
    };

    return viewModel;
};