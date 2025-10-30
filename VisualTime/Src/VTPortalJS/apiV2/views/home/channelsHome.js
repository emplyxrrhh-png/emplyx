VTPortal.channelsHome = function (params) {
    var count = ko.observable(0);
    var channelsDS = ko.observable([]);
    var channelsDesc = ko.computed(function () {
        count(0);

        if (channelsDS() != null && channelsDS().length > 0) {
            for (var i = 0; i < channelsDS().length; ++i) {
                count(count() + channelsDS()[i].NewMessages);
            }
        }

        if (count() == 0) {
            VTPortal.roApp.hasMessages(false);
            return i18nextko.i18n.t('nomessages');
        } else {
            VTPortal.roApp.hasMessages(true);
            return i18nextko.i18n.t('newmessages') + count() + ' ' + i18nextko.i18n.t('newmessages2');
        }
    });

    var channelsTitle = ko.computed(function () {
        return i18nextko.i18n.t('rochannelsTitle');
    });

    function goToChannels() {
        window.VTPortalUtils.utils.setActiveTab('channels');

        VTPortal.app.navigate('myChannels', { root: true });
    }

    function loadChannels() {
        if (VTPortal.roApp.loggedIn) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var channels = result.Value;
                    for (var i = 0; i < channels.length; i++) {
                        channels[i]['hasAction'] = true;
                        if (channels[i].NewMessages == 0) channels[i]['MessagesInfo'] = i18nextko.i18n.t('nomessages');
                        else channels[i]['MessagesInfo'] = i18nextko.i18n.t('newmessages') + channels[i].NewMessages + ' ' + i18nextko.i18n.t('newmessages2');

                        if (channels[i].IsComplaintChannel) channels[i]['cssClass'] = 'dx-channelComplaint';
                        else channels[i]['cssClass'] = 'dx-channelCommunications';
                    }

                    VTPortal.roApp.db.settings.updateCacheDS('channels', channels);
                    channelsDS(channels);
                } else {
                    channelsDS([]);
                    VTPortalUtils.utils.processErrorMessage(result, function () { });
                }
            }, function (error) {
                channelsDS([]);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingChannels"), 'error', 0, function () { });
            }).getMyChannels();
        }
    }

    var viewShown = function (e) {        
        channelsDS(VTPortal.roApp.db.settings.getCacheDS('channels'));
        if (channelsDS() == null || window.VTPortalUtils.needToRefresh('channels')) loadChannels();
    }

    var viewModel = {
        viewShown: viewShown,
        refreshChannels: loadChannels,
        goToChannels: goToChannels,
        channelsTitle: channelsTitle,
        lblChannelsDesc: channelsDesc,
        btnGo: {
            onClick: goToChannels,
            text: count,
            visible: count,
        },

        iconChannels: {
            onClick: goToChannels,
            icon: 'channels'
        },
        hasMessages: ko.computed(function () {
            if (VTPortal.roApp.hasMessages() == true) {
                return 'hasMessages';
            }
            else {
                return 'hasMessagesNot';
            }
        }),

        hasMessagesTitle: ko.computed(function () {
            if (VTPortal.roApp.hasMessages() == true) {
                return 'mainTitle';
            }
            else {
                return 'mainTitle';
            }
        }),
    };

    return viewModel;
};