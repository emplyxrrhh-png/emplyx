VTPortal.channelConversations = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var channelId = ko.observable(-1);
    var popupAddNewConversationVisible = ko.observable(false);
    var txtTitle = ko.observable(''), txtMessage = ko.observable(''), sbAnonymous = ko.observable(0);

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) channelId(parseInt(params.id, 10));
    else channelId(-1);

    var conversationsDS = ko.observable([]);
    var channel = ko.observable(null);

    function viewShown() {
        VTPortal.roApp.db.settings.markForRefresh(['channels', "status"]);
        globalStatus().viewShown();
        loadChannel();
        loadConversations();
    };

    function refresh() {
        globalStatus().viewShown();
        loadConversations();
    };

    function loadChannel() {
        //TODO: QUITAR ESTO Y OBTENERLO DEL DATACATALOG, NO HACER LLAMADA PARA OBTENER TODOS LOS CANALES, RECOGERLO DE LOS CARGADOS.
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
                    channel(channels.find(c => c.Id == channelId()));
                    sbAnonymous(allowAnonymous() ? 1 : 0);
                } else {
                    channel(null);
                    VTPortalUtils.utils.processErrorMessage(result, function () { });
                }
            }, function (error) {
                channel(null);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingChannels"), 'error', 0, function () { });
            }).getMyChannels();
        }
    }

    var allowAnonymous = ko.computed(function () {
        if (channel() != null) {
            if (!(typeof channel().AllowAnonymous === 'undefined')) {
                return channel().AllowAnonymous
            }
        }
        return false;
    });

    function loadConversations() {
        //VTPortal.roApp.dataCatalogs.loadConversationsDS(channelId(), conversationsDS, true);
        if (VTPortal.roApp.loggedIn) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var conversations = result.Value;
                    for (var i = 0; i < conversations.length; i++) {
                        conversations[i]['hasAction'] = true;

                        if (conversations[i].NewMessages == 0) conversations[i]['MessagesInfo'] = i18nextko.i18n.t('nomessages');
                        else conversations[i]['MessagesInfo'] = i18nextko.i18n.t('newmessages') + conversations[i].NewMessages + ' ' + i18nextko.i18n.t('newmessages2');

                        conversations[i]['cssClass'] = 'dx-channelMessage';
                        conversations[i]['ConversationDetails'] = i18nextko.i18n.t('chStatusConversation'); //Estructura final -> Estado: XXX | Ref.: XXX
                        switch (conversations[i].Status) {
                            case 0:
                                conversations[i]['cssClass'] += ' ro_Status_pending';
                                conversations[i]['ConversationDetails'] += i18nextko.i18n.t('chStatusPending');
                                break;
                            case 1:
                                conversations[i]['cssClass'] += ' ro_Status_ongoing';
                                conversations[i]['ConversationDetails'] += i18nextko.i18n.t('chStatusOnGoing');
                                break;
                            case 2:
                                conversations[i]['cssClass'] += ' ro_Status_denied';
                                conversations[i]['ConversationDetails'] += i18nextko.i18n.t('chStatusDenied');
                                break;
                            case 3:
                                conversations[i]['cssClass'] += ' ro_Status_accepted';
                                conversations[i]['ConversationDetails'] += i18nextko.i18n.t('chStatusClosed');
                                break;
                        }
                        conversations[i]['ConversationDetails'] += ' | Ref.: ' + conversations[i].ReferenceNumber;
                    }
                    //Ordenamos por estado y fecha de último mensaje
                    conversations.sort(function (a, b) {
                        if (a.Status < b.Status) {
                            return -1;
                        } else if (a.Status > b.Status) {
                            return 1;
                        }
                        if (a.LastMessageTimestamp > b.LastMessageTimestamp) {
                            return -1;
                        } else if (a.LastMessageTimestamp < b.LastMessageTimestamp) {
                            return 1;
                        }
                        return 0;
                    });

                    conversationsDS(conversations);
                } else {
                    conversationsDS([]);
                    VTPortalUtils.utils.processErrorMessage(result, function () { });
                }
            }, function (error) {
                conversationsDS([]);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingConversations"), 'error', 0, function () { });
            }).getMyConversationsByIDChannel(channelId());
        }
        sbAnonymous(allowAnonymous() ? 1 : 0);
    }

    var openPopup = function () {
        popupAddNewConversationVisible(true);
    }

    var closePopup = function () {
        popupAddNewConversationVisible(false);
    }

    var btnAddNewConversation = function (fParam) {
        var result = fParam.validationGroup.validate();
        if (result.isValid) {
            popupAddNewConversationVisible(false);
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    txtTitle('');
                    txtMessage('');
                    sbAnonymous(allowAnonymous() ? 1 : 0);

                    $('#scrollview').find('.pass').each(function (index) {
                        $(this).dxValidator('instance').reset();
                    });
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNCOKNewConversation'), 'success', 2500);
                    navigateToConversationMessages(result.Value.Id, result.Value.Status);
                } else {
                    var onContinue = function () {
                        openPopup(true);
                    }
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0, onContinue);
                }
            }).addNewConversation(channelId(), txtTitle(), encodeURIComponent(txtMessage()), sbAnonymous() ? 1 : 0);
        }
    }

    function navigateToConversationMessages(conversationId, conversationStatus) {
        VTPortal.app.navigate('conversationMessages/' + conversationId + '/' + conversationStatus);
    }

    const privacityOptions = [{
        title: i18nextko.t('roNCsbAnonymous'),
        name: 'anonymous',
        value: 1
    }, {
        title: i18nextko.t('roNCsbNotAnonymous'),
        name: 'open',
        value: 0
    }];

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roMyConversations'),
        subscribeBlock: globalStatus(),
        btnAdd: {
            onClick: openPopup,
            text: '',
            hint: '',
            icon: "Images/Common/plus.png"
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        conversationsDS: conversationsDS,
        conversationsContent: {
            height: '76%',
            bounceEnabled: false,
            onPullDown: function (options) {
                var resultFunc = function (callback) {
                    return function () { callback(); }
                }
                //setTimeout(resultFunc(refresh), 100);
                options.component.release();
            }
        },
        listConversations: {
            dataSource: conversationsDS,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'AlertItem',
            onItemClick: function (info) {
                navigateToConversationMessages(info.itemData.Id, info.itemData.Status);
            }
        }, txtTitle: {
            placeholder: i18nextko.t('roNCtxtTitle'),
            value: txtTitle
        },
        txtMessage: {
            placeholder: i18nextko.t('roNCtxtMessage'),
            value: txtMessage
        },
        sbAnonymous: {
            text: i18nextko.t('roNCsbAnonymous'),
            value: sbAnonymous
        },
        showAnonymous: allowAnonymous,
        btnCancel: {
            onClick: closePopup,
            text: i18nextko.t('roCancel')
        },
        lblTitle: i18nextko.t('roNClblTitle'),
        lblMessage: i18nextko.t('roNClblMessage'),
        lblAnonymous: i18nextko.t('roNClblAnonymous'),
        popupAddNewConversationVisible: popupAddNewConversationVisible,
        requieredTitle: {
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        requieredMessage: {
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        requieredAnonymous: {
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        btnAddNewConversation: {
            onClick: btnAddNewConversation,
            text: i18nextko.t('roNCAddNewConversation')
        },
    };

    return viewModel;
};