VTPortal.profileConfig = function (params) {
    var inAppBrowserRef = null;
    var configActionsDS = ko.observable([]);
    var modelIsReady = ko.observable(false);
    var popupChangePwdVisible = ko.observable(false);
    var popupChangeLngVisible = ko.observable(false);
    var lngLogin = ko.observable(i18nextko.i18n.t('lngProfile'));
    var txtOldPwd = ko.observable(''), txtNewPwd = ko.observable(''), txtNewPwdRepeat = ko.observable('');
    var currentLanguage = ko.observable(window.VTPortal.roApp.getLanguageByID(VTPortal.roApp.db.settings.language));
    var changeUserPhotoVisible = ko.observable(false);
    var selectedFiles = ko.observableArray();
    var selectFileText = ko.observable(i18nextko.i18n.t('roFindFile'));
    var originalPhoto = ko.observable('');
    var formImageUri = ko.observable('');


    function viewShown(selDate) {
        var refreshMenuItems = [];

        let addPasswordChange = true;

        if (VTPortal.roApp.db.settings.login.indexOf('\\') != -1) addPasswordChange = false;

        if (VTPortal.roApp.db.settings.adfsCredential != '') addPasswordChange = false;

        if (VTPortal.roApp.db.settings.isAD) addPasswordChange = false;

        if (VTPortal.roApp.db.settings.login.indexOf('.\\') != -1) addPasswordChange = true;

        if (VTPortal.roApp.isTimeGate()) addPasswordChange = false;


        refreshMenuItems.push({ "ID": 0, "Name": i18nextko.t('roChangePhotoBt'), "CssClass": 'ro-icon-ChangePhoto' });

        if (addPasswordChange == true) {
            refreshMenuItems.push({ "ID": 1, "Name": i18nextko.t('roChangePwdBt'), "CssClass": 'ro-icon-ChangePwd' });
        }

        refreshMenuItems.push({ "ID": 2, "Name": i18nextko.t('lng'), "CssClass": 'ro-icon-language' });

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

        if (!VTPortal.roApp.isTimeGate()) {
            VTPortal.roApp.db.settings.language = currentLanguage().ID;
            VTPortal.roApp.db.settings.save();
        }

        if (VTPortal.roApp.db.settings.adfsCredential != '') {
            var app = window.VTPortal.roApp.db.settings;

            new WebServiceRobotics(function (result) {
                VTPortalUtils.utils.buildHamburgerMenu();
                setTimeout(function () { VTPortal.app.navigate("loading", { root: true }); }, 200)

            }, function (error) {
                setTimeout(function () { VTPortal.app.navigate("loading", { root: true }); }, 200)
            }).updateServerLanguage(window.VTPortal.roApp.getLanguageByID(app.language).ID);
        } else {
            var app = window.VTPortal.roApp.db.settings;

            new WebServiceRobotics(function (result) {
                VTPortalUtils.utils.buildHamburgerMenu();

                VTPortal.roApp.redirectAtHome();
            }, function (error) {
            }, this.wsURL, this.isSSL).updateServerLanguage(window.VTPortal.roApp.getLanguageByID(app.language).ID);
        }
    }

    var btnChangePwd = function (fParam) {
        var result = fParam.validationGroup.validate();
        if (result.isValid) {
            popupChangePwdVisible(false);
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    if (VTPortal.roApp.isModeApp()) VTPortal.roApp.db.settings.rememberLoginApp = txtNewPwd();
                    else VTPortal.roApp.db.settings.password = '';

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


    var wsRobotics = null;
    var uploadUserPhoto = function () {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingPhoto"), 'error', 0);
            changeUserPhotoVisible(false);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.loadInitialData(true, false, false, false, false);
                VTPortal.roApp.redirectAtHome();
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roProfilePhotoSaved"), 'success', 2000);
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }

            changeUserPhotoVisible(false);
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        var formData = new FormData();
        if (VTPortal.roApp.isModeApp() == false) formData.append("userfile", selectedFiles()[0]);
        else formData.append("userfile", originalPhoto());

        if (VTPortal.roApp.isModeApp()) wsRobotics.saveProfileImageMobile(formData);
        else wsRobotics.saveProfileImageDesktop(formData);
    }

    function selectPhotoEvent() {
        VTPortalUtils.utils.isShowingPopup(false);

        if (Object.has(navigator, 'camera')) {
            var selectFromLibrary = function () {
                navigator.camera.getPicture(
                    function (imageURI) {
                        originalPhoto(imageURI);
                        if (typeof (device) != 'undefined') {
                            if (cordova.platformId === 'ios') {
                                formImageUri(window.WkWebView.convertFilePath(imageURI));
                            }
                            else {
                                formImageUri(imageURI);
                            }
                        }
                        else {
                            formImageUri(imageURI);
                        }
                    },
                    function (message) {
                        if (!message.includes('cancelled')) { VTPortalUtils.utils.notifyMesage(message, 'error', 2000); }
                    },
                    {
                        quality: 50,
                        sourceType: Camera.PictureSourceType.PHOTOLIBRARY,
                        destinationType: Camera.DestinationType.FILE_URI,
                        allowEdit: false,
                        encodingType: Camera.EncodingType.JPEG,
                        targetWidth: 200,
                        targetHeight: 200,
                        saveToPhotoAlbum: false,
                        correctOrientation: true
                    }
                );
            };
            var takeFromCamera = function () {
                navigator.camera.getPicture(
                    function (imageURI) {
                        originalPhoto(imageURI);
                        if (typeof (device) != 'undefined') {
                            if (cordova.platformId === 'ios') {
                                formImageUri(window.WkWebView.convertFilePath(imageURI));
                            }
                            else {
                                formImageUri(imageURI);
                            }
                        }
                        else {
                            formImageUri(imageURI);
                        }
                    },
                    function (message) {
                        if (!message.includes('cancelled')) { VTPortalUtils.utils.notifyMesage(message, 'error', 2000); }
                    },
                    {
                        quality: 50,
                        sourceType: Camera.PictureSourceType.CAMERA,
                        destinationType: Camera.DestinationType.FILE_URI,
                        allowEdit: false,
                        encodingType: Camera.EncodingType.JPEG,
                        targetWidth: 200,
                        targetHeight: 200,
                        saveToPhotoAlbum: false,
                        correctOrientation: true
                    }
                );
            };

            VTPortalUtils.utils.questionMessage(i18nextko.i18n.t('roSelectOrigin'), 'info', 0, selectFromLibrary, takeFromCamera, i18nextko.i18n.t('roTakeFromLibrary'), i18nextko.i18n.t('roTakeFromCamera'));
        } else {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoCameraAvailable'), 'error', 0);
        }
    }

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
                    case 0:
                        changeUserPhotoVisible(true);
                        break;
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
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),

        changeUserPhotoVisible: changeUserPhotoVisible,
        btnCancelUpload: {
            onClick: function () { changeUserPhotoVisible(false); },
            text: i18nextko.t('roCancel'),
        },
        btnAcceptUpload: {
            onClick: function () { uploadUserPhoto(); },
            text: i18nextko.t('roSaveRequest'),
        },
        selectPhotoEvent: selectPhotoEvent,
        imageSrc: formImageUri,
        uploadOptions: {
            multiple: false,
            accept: "*",
            value: selectedFiles,
            uploadMode: "useForm",
            showFileList: false,
            selectButtonText: selectFileText,
            onValueChanged: function (e) {
                selectFileText(e.value[0].name)
            },
            onHiding: function (options) {
                VTPortalUtils.utils.isShowingPopup(false);
            }
        },
        changeUserPhoto: function () {
            changeUserPhotoVisible(true);
        },
        appMode: VTPortal.roApp.isModeApp(),
        lblProfileImage: i18nextko.t('roProfileImage'),
        photoPopover: {
            target: ('#employeePhotoDiv'),
            shading: true,
            width: 200,
            shadingColor: "rgba(0, 0, 0, 0.5)",
            visible: VTPortal.roApp.showPopover
        },
        photoHelp: i18nextko.t('roPhotoHelp')
    };

    return viewModel;
};