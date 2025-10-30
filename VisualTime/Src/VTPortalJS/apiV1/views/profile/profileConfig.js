VTPortal.profileConfig = function (params) {
    var inAppBrowserRef = null;
    var configActionsDS = ko.observable([]);
    var modelIsReady = ko.observable(false);
    var popupChangePwdVisible = ko.observable(false);
    var popupChangeLngVisible = ko.observable(false);
    var lngLogin = ko.observable(i18nextko.i18n.t('lngProfile'));
    var txtOldPwd = ko.observable(''), txtNewPwd = ko.observable(''), txtNewPwdRepeat = ko.observable('');
    var currentLanguage = ko.observable(window.VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language));

    function viewShown(selDate) {
        var refreshMenuItems = [];

        var addPasswordChange = true;

        if (VTPortal.roApp.db.settings.login.indexOf('\\') != -1)
            addPasswordChange = false;

        if (VTPortal.roApp.db.settings.adfsCredential != '')
            addPasswordChange = false;

        if (VTPortal.roApp.db.settings.isAD == true)
            addPasswordChange = false

        if (VTPortal.roApp.db.settings.login.indexOf('.\\') != -1)
            addPasswordChange = true

        if (addPasswordChange == true) {
            refreshMenuItems.push({ "ID": 1, "Name": i18nextko.t('roChangePwdBt'), "CssClass": 'ro-icon-ChangePwd' });
        }

        refreshMenuItems.push({ "ID": 2, "Name": i18nextko.t('lng'), "CssClass": 'ro-icon-info' });

        // refreshMenuItems.push({ "ID": 2, "Name": i18nextko.t('roLogout'), "CssClass": 'ro-icon-Logout' });

        configActionsDS(refreshMenuItems);

        modelIsReady(true);
        $('#scrollview').height($('#panelsContent').height() - 70);
    };

    var logoutSession = function () {
        if (VTPortal.roApp.loggedIn) {
            new WebServiceRobotics(function (result) {
                window.VTPortalUtils.utils.onLogoutSuccessFunc(result);
            }, function (error) {
                window.VTPortalUtils.utils.onLogoutErrorFunc(error);;
            }).logout(VTPortal.roApp.db.settings.UUID);
        }
    }

    var openPopupChgPwd = function () {
        popupChangePwdVisible(true);
    }

    var closePopupChgPwd = function () {
        popupChangePwdVisible(false);
    }

    var openPopupChgLng = function () {
        popupChangeLngVisible(true);
    }

    var closePopupChgLng = function () {
        popupChangeLngVisible(false);
    }

    var btnChangeLng = function (fParam) {
        VTPortal.roApp.setLanguage(currentLanguage().tag);
        VTPortal.roApp.db.settings.language = currentLanguage().ID;
        VTPortal.roApp.db.settings.save();

        if (VTPortal.roApp.db.settings.adfsCredential != '') {
            var app = window.VTPortal.roApp.db.settings;

            new WebServiceRobotics(function (result) {
                setTimeout(function () { VTPortal.app.navigate("loading", { root: true }); }, 200)
            }, function (error) {
                setTimeout(function () { VTPortal.app.navigate("loading", { root: true }); }, 200)
            }).updateServerLanguage(window.VTPortal.roApp.getLanguageByID(app.language).ID);
        } else {
            var app = window.VTPortal.roApp.db.settings;

            new WebServiceRobotics(function (result) {
                VTPortal.roApp.redirectAtHome();
            }, function (error) {
                window.VTPortalUtils.utils.onLoginErrorFunc(error);
            }, this.wsURL, this.isSSL).login(app.login, app.password, window.VTPortal.roApp.getLanguageByID(app.language).ID, VTPortal.roApp.infoVersion, '', false);
        }
    }

    var btnChangePwd = function (fParam) {
        var result = fParam.validationGroup.validate();
        if (result.isValid) {
            popupChangePwdVisible(false);
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.db.settings.password = txtNewPwd();
                    VTPortal.roApp.db.settings.save();
                    txtOldPwd('');
                    txtNewPwd('');
                    txtNewPwdRepeat('');

                    $('#scrollview').find('.pass').each(function (index) {
                        $(this).dxValidator('instance').reset();
                    });

                    VTPortal.roApp.redirectAtHome();
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roOkChangingPwd'), 'success', 2500);
                } else {
                    var onContinue = function () {
                        openPopupChgPwd(true);
                    }
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0, onContinue);
                }
            }).changePassword(txtOldPwd(), txtNewPwd());
        }
    }
    //const capitalize = (s) => {
    //    if (typeof s !== 'string') return ''
    //    return s.charAt(0).toUpperCase() + s.slice(1)
    //}

    var viewModel = {
        modelIsReady: modelIsReady,
        viewShown: viewShown,
        lblLanguage: lngLogin,
        lblOldPwd: i18nextko.t('lblOldPwd'),
        lblNewPwd: i18nextko.t('lblNewPwd'),
        lblNewPwdRepeat: i18nextko.t('lblNewPwdRepeat'),
        popupChangePwdVisible: popupChangePwdVisible,
        popupChangeLngVisible: popupChangeLngVisible,
        listOptions: {
            dataSource: configActionsDS,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'configAction',
            onItemClick: function (info) {
                switch (info.itemData.ID) {
                    case 1:
                        openPopupChgPwd();
                        break;
                    case 2:
                        openPopupChgLng();
                        break;
                }
            }
        }, txtOldPwd: {
            placeholder: i18nextko.t('roPH_oldPwd'),
            value: txtOldPwd,
            mode: 'password'
        },
        txtNewPwd: {
            placeholder: i18nextko.t('roPH_newPwd'),
            value: txtNewPwd,
            mode: 'password'
        },
        txtNewPwdRepeat: {
            placeholder: i18nextko.t('roPH_newPwdRepeat'),
            value: txtNewPwdRepeat,
            mode: 'password'
        },
        btnCancel: {
            onClick: closePopupChgPwd,
            text: i18nextko.t('roCancel')
        },
        btnCancelLng: {
            onClick: closePopupChgLng,
            text: i18nextko.t('roCancel')
        },
        currentLanguage: currentLanguage,
        dataSourceLanguages: {
            dataSource: VTPortal.roApp.availableLanguages,
            displayExpr: "Name",
            placeholder: i18nextko.t('language'),
            value: currentLanguage,
            itemTemplate: 'langItemTemplate',
            onItemClick: function (info) {
                switch (info.itemData.ID) {
                    case 'ESP':

                        break;
                    case 'CAT':

                        break;
                    case 'ENG':

                        break;
                }
                //VTPortal.app.navigate("login", { root: true });
            }
        },

        btnChangePwd: {
            onClick: btnChangePwd,
            text: i18nextko.t('roChangePwd')
        },
        btnChangeLng: {
            onClick: btnChangeLng,
            text: i18nextko.t('roChangePwd')
        },
        requieredOldField: {
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        requieredNewField: {
            validationRules: [{ type: 'required', message: i18nextko.i18n.t('roRequieredField') }]
        },
        compareField: {
            validationRules: [{
                type: 'compare', comparisonType: '==', comparisonTarget: txtNewPwd, message: i18nextko.i18n.t('roPasswordsNotMatch')
            }]
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};