VTPortal.conversationMessages = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var conversationId = ko.observable(-1);
    var txtNewMessage = ko.observable('');
    var popupAddNewMessageVisible = ko.observable(false);
    var conversationStatus = ko.observable(0);

    const showAddButton = ko.computed(function () {
        return (conversationStatus() == 0 || conversationStatus() == 1);
    });

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) conversationId(parseInt(params.id, 10));
    if (typeof params.param != 'undefined' && parseInt(params.param, 10) != -1) conversationStatus(parseInt(params.param, 10));
    else conversationId(-1);

    var messagesDS = ko.observable([]);

    var saveMessage = function (fParam) {
        var result = fParam.validationGroup.validate();
        if (result.isValid) {
            popupAddNewMessageVisible(false);
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.db.settings.markForRefresh(['channels', "status"]);
                    txtNewMessage('');

                    $('#scrollview').find('.pass').each(function (index) {
                        $(this).dxValidator('instance').reset();
                    });
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNCOKNewMessage'), 'success', 2500);
                    VTPortal.roApp.redirectAtHome();
                } else {
                    var onContinue = function () {
                        //openPopup(true);
                    }
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0, onContinue);
                }
            }).addNewMessage(conversationId(), encodeURIComponent(txtNewMessage()));
        }
    }

    function viewShown() {
        globalStatus().viewShown();
        loadMessages();
    };

    function refresh() {
        globalStatus().viewShown();
        loadMessages();
    };

    function loadMessages() {
        //VTPortal.roApp.dataCatalogs.loadMessagesDS(conversationId(), messagesDS, true);
        if (VTPortal.roApp.loggedIn) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var messages = result.Value;
                    for (var i = 0; i < messages.length; i++) {
                        messages[i]['hasAction'] = true;

                        if (messages[i].NewMessages == 0) messages[i]['MessagesInfo'] = i18nextko.i18n.t('nomessages');
                        else messages[i]['MessagesInfo'] = i18nextko.i18n.t('newmessages') + messages[i].NewMessages + ' ' + i18nextko.i18n.t('newmessages2');

                        messages[i]['cssClass'] = messages[i].IsResponse ? '' : 'employee';
                    }
                    messagesDS(messages);
                    setTimeout(function () {
                        var i_scroller = $("#scrollview.conversationMessages").dxScrollView("instance");
                        var i_height = i_scroller.scrollHeight();
                        i_scroller.scrollTo(i_height);
                    }, 100);
                } else {
                    messagesDS([]);
                    VTPortalUtils.utils.processErrorMessage(result, function () { });
                }
            }, function (error) {
                messagesDS([]);

                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingConversations"), 'error', 0, function () { });
            }).getMyMessagesByIDConversation(conversationId());
        }
    }

    var openPopup = function () {
        popupAddNewMessageVisible(true);
    }

    var closePopup = function () {
        popupAddNewMessageVisible(false);
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roMyMessages'),
        subscribeBlock: globalStatus(),
        btnAdd: {
            onClick: openPopup,
            text: '',
            hint: '',
            icon: "Images/Common/plus.png",
            visible: showAddButton
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        messagesDS: messagesDS,
        messagesContent: {
            height: '76%',
            bounceEnabled: true,
            onPullDown: function (options) {
                var resultFunc = function (callback) {
                    return function () { callback(); }
                }
                //setTimeout(resultFunc(refresh), 100);
                options.component.release();
            }
        },
        listMessages: {
            dataSource: messagesDS,
            scrollingEnabled: false,
            grouped: false,
            selectionMode: 'none',
            focusStateEnabled: false
        },
        newMessage: {
            value: txtNewMessage,
            placeholder: i18nextko.t('roNCtxtMessage')
        },
        lblMessage: i18nextko.t('roNClblMessage'),
        popupAddNewMessageVisible: popupAddNewMessageVisible,
        btnAddNewMessage: {
            onClick: saveMessage,
            text: i18nextko.t('roNCAddNewMessage')
        },
        btnCancel: {
            onClick: closePopup,
            text: i18nextko.t('roCancel')
        },
        requieredMessage: {
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
    };

    return viewModel;
};