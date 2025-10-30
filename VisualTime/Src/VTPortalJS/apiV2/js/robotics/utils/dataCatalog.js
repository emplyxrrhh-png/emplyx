(function () {
    var namespace = function (name) {
        var namespaces = name.split('.'),
            namespace = window,
            index;
        for (index = 0; index < namespaces.length; index += 1) {
            namespace = namespace[namespaces[index]] = namespace[namespaces[index]] || {};
        }
        return namespace;
    };

    namespace("Robotics.Client.Data");
}());

Robotics.Client.Data.roCatalog = function () {
    this.channelsLoaded = false;
    this.channelsDS = ko.observable([]);
    this.conversationsLoaded = {};
    this.conversationsDS = [];
    this.messagesLoaded = {};
    this.messagesDS = [];

    this.workingObservable = null;
}

Robotics.Client.Data.roCatalog.prototype.getChannelsDS = function (bReload) {
    if (VTPortal.roApp.loggedIn) {
        if (this.channelsLoaded && (typeof (bReload) == 'undefined' || (typeof (bReload) != 'undefined' && bReload == false))) return this.channelsDS;

        var catalog = this;
        this.channelsLoaded = true;
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

                //TODO: Sigue pasando el error que aparece cuando refrescas canales en el momento que se han cargado de BD y hay un cambio de estado en BD de uno existente a oculto.

                catalog.channelsDS(channels);
            } else {
                catalog.channelsDS([]);
                VTPortalUtils.utils.processErrorMessage(result, function () { });
            }
        }, function (error) {
            catalog.channelsDS([]);
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingChannels"), 'error', 0, function () { });
        }).getMyChannels();
    }

    return this.channelsDS;
};

Robotics.Client.Data.roCatalog.prototype.loadConversationsDS = function (idChannel, sObservable, bReload) {
    this.workingObservable = sObservable;
    /*if (typeof this.conversationsLoaded[`${idChannel}`] == 'undefined') this.conversationsLoaded[`${idChannel}`] = false;
    if (this.conversationsLoaded[`${idChannel}`] == false) {
        var tmpDS = {
            id: idChannel,
            data: []
        };

        this.conversationsDS.push(tmpDS);
    }*/

    if (VTPortal.roApp.loggedIn) {
        var catalog = this;

        /*if (this.conversationsLoaded[`${idChannel}`] == false || (typeof (bReload) != 'undefined' && bReload == true)) {*/

        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var conversations = result.Value;
                for (var i = 0; i < conversations.length; i++) {
                    conversations[i]['hasAction'] = true;

                    if (conversations[i].NewMessages == 0) conversations[i]['MessagesInfo'] = i18nextko.i18n.t('nomessages');
                    else conversations[i]['MessagesInfo'] = i18nextko.i18n.t('newmessages') + conversations[i].NewMessages + ' ' + i18nextko.i18n.t('newmessages2');

                    conversations[i]['cssClass'] = 'dx-channelMessage';
                }
                //catalog.conversationsLoaded[`${idChannel}`] = true;

                catalog.workingObservable(conversations);

                /*for (var i = 0; i < catalog.conversationsDS.length; i++) {
                    if (catalog.conversationsDS[i].id == idChannel) {
                        catalog.conversationsDS[i].data = conversations;
                        catalog.workingObservable(Object.clone(catalog.conversationsDS[i].data, true));
                    }
                }*/
            } else {
                /*for (var i = 0; i < catalog.conversationsDS.length; i++) {
                    if (catalog.conversationsDS[i].id == idChannel) catalog.conversationsDS[i].data = [];
                }*/
                catalog.workingObservable([]);
                VTPortalUtils.utils.processErrorMessage(result, function () { });
            }
        }, function (error) {
            for (var i = 0; i < catalog.conversationsDS.length; i++) {
                if (catalog.conversationsDS[i].id == idChannel) catalog.conversationsDS[i].data = [];
            }
            catalog.workingObservable([]);

            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingConversations"), 'error', 0, function () { });
        }).getMyConversationsByIDChannel(idChannel);

        /*} else {
            for (var i = 0; i < catalog.conversationsDS.length; i++) {
                if (this.conversationsDS[i].id == idChannel) this.workingObservable(Object.clone(this.conversationsDS[i].data, true));
            }
        }        */
    }
};

Robotics.Client.Data.roCatalog.prototype.loadMessagesDS = function (idConversation, sObservable, bReload) {
    this.workingObservable = sObservable;

    /*if (typeof this.messagesLoaded[`${idConversation}`] == 'undefined') this.messagesLoaded[`${idConversation}`] = false;
    if (this.messagesLoaded[`${idConversation}`] == false) {
        var tmpDS = {
            id: idConversation,
            data: []
        };

        this.messagesDS.push(tmpDS);
    }*/

    if (VTPortal.roApp.loggedIn) {
        var catalog = this;

        /*if (this.messagesLoaded[`${idConversation}`] == false || (typeof (bReload) != 'undefined' && bReload == true)) {*/

        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var messages = result.Value;
                for (var i = 0; i < messages.length; i++) {
                    messages[i]['hasAction'] = true;

                    if (messages[i].NewMessages == 0) messages[i]['MessagesInfo'] = i18nextko.i18n.t('nomessages');
                    else messages[i]['MessagesInfo'] = i18nextko.i18n.t('newmessages') + messages[i].NewMessages + ' ' + i18nextko.i18n.t('newmessages2');

                    messages[i]['cssClass'] = 'dx-channelMessage';
                }
                //catalog.messagesLoaded[`${idConversation}`] = true;
                catalog.workingObservable(messages);

                /*for (var i = 0; i < catalog.messagesDS.length; i++) {
                    if (catalog.messagesDS[i].id == idConversation) {
                        catalog.messagesDS[i].data = messages;
                        catalog.workingObservable(Object.clone(catalog.messagesDS[i].data, true));
                    }
                }*/
            } else {
                /*for (var i = 0; i < catalog.messagesDS.length; i++) {
                    if (catalog.messagesDS[i].id == idConversation) catalog.messagesDS[i].data = [];
                }*/
                catalog.workingObservable([]);
                VTPortalUtils.utils.processErrorMessage(result, function () { });
            }
        }, function (error) {
            for (var i = 0; i < catalog.messagesDS.length; i++) {
                if (catalog.messagesDS[i].id == idConversation) catalog.messagesDS[i].data = [];
            }
            catalog.workingObservable([]);

            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingConversations"), 'error', 0, function () { });
        }).getMyMessagesByIDConversation(idConversation);

        /*} else {
            for (var i = 0; i < catalog.messagesDS.length; i++) {
                if (this.messagesDS[i].id == idConversation) this.workingObservable(Object.clone(this.messagesDS[i].data, true));
            }
        }*/
    }
};

Robotics.Client.Data.roCatalog.prototype.destroyDS = function () {
    this.channelsLoaded = false;
    this.channelsDS([]);
    this.conversationsLoaded = {};
    this.conversationsDS = [];
    this.messagesLoaded = {};
    this.messagesDS = [];
};