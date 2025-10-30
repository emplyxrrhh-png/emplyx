VTPortal.statusHeader = function (params) {
    var changeUserPhotoVisible = ko.observable(false);
    var selectedFiles = ko.observableArray();
    var formImageUri = ko.observable('');
    var selectFileText = ko.observable(i18nextko.i18n.t('roFindFile'));
    var originalPhoto = ko.observable('');

    var employeeImage = ko.computed(function () {
        var backgroundImage = 'url(data:image/png;base64,' + VTPortal.roApp.userPhoto() + ') no-repeat center center';
        return backgroundImage;
    });

    var bBottom = ko.computed(function () {
        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                    return "3px solid green";
                } else {
                    return "3px solid red";
                }
            } else return '';
        } else {
            return "3px solid #6b96eb";
        }
    });

    var serverDate = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            var sDate = moment(VTPortal.roApp.userStatus().ServerDate);
            return sDate.format('LL') + " " + sDate.format('LT');
        } else return '';
    });

    var username = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            return VTPortal.roApp.userStatus().EmployeeName;
        } else return '';
    });

    var productivTask = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            if (VTPortal.roApp.userStatus().TaskTitle.length <= 35) {
                return VTPortal.roApp.userStatus().TaskTitle;
            } else {
                return VTPortal.roApp.userStatus().TaskTitle.substr(0, 35) + '...';
            }
        } else return '';
    });

    var costCenter = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            return VTPortal.roApp.userStatus().CostCenterName;
        } else return '';
    });

    var alerts = ko.computed(function () {
        return VTPortalUtils.utils.badgeCount();
    });

    var buttonVisible = ko.computed(function () {
        if (VTPortal.roApp.showLogoutHome() != null && VTPortal.roApp.db.settings.ApiVersion > VTPortalUtils.apiVersion.CauseNote && VTPortal.roApp.impersonatedIDEmployee == -1) {
            return VTPortal.roApp.showLogoutHome()
        } else return false;
    });

    var requests = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            return VTPortal.roApp.userStatus().ScheduleStatus.RequestAlerts.length;
        } else return '';
    });

    var forgottenPunches = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            return VTPortal.roApp.userStatus().ScheduleStatus.IncompletePunches.length;
        } else return '';
    });

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

    function onLogoutClicked() {
        if (VTPortal.roApp.loggedIn) {
            new WebServiceRobotics(function (result) {
                window.VTPortalUtils.utils.onLogoutSuccessFunc(result);
            }, function (error) {
                window.VTPortalUtils.utils.onLogoutErrorFunc(error);;
            }).logout();
        }
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
                        quality: 100,
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
                        quality: 100,
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
        bgImage: backgroundImage,
        empImage: employeeImage,
        borderBottom: bBottom,
        lblUsername: username,
        lblAlerts: alerts,
        lblRequests: requests,
        lblForgottenPunches: forgottenPunches,
        lblServerDate: serverDate,
        lblProductivTask: productivTask,
        lblCostCenter: costCenter,
        lblProfileImage: i18nextko.t('roProfileImage'),
        changeUserPhotoVisible: changeUserPhotoVisible,
        btnLogout: {
            onClick: onLogoutClicked,
            text: '',
            icon: "Images/Actions/exit.png",
            visible: false
        },
        changeUserPhoto: function () {
            changeUserPhotoVisible(true);
        },
        goToProfile: function () {
            VTPortal.app.navigate('profile');
        },
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
        appMode: VTPortal.roApp.isModeApp(),
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
        hintView: i18nextko.i18n.t('roOptions')
    };

    return viewModel;
};