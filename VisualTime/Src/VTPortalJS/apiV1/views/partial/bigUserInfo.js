VTPortal.bigUserInfo = function (params) {
    var mainMenuItems = ko.observable([]);
    var changeUserPhotoVisible = ko.observable(false);
    var selectedFiles = ko.observableArray();
    var requestsSupervisor = ko.observable(0);
    var alertsSupervisor = ko.observable(0);
    var formImageUri = ko.observable('');
    var originalPhoto = ko.observable('');
    var selectFileText = ko.observable(i18nextko.i18n.t('roFindFile'));

    var refreshMenuItems = ko.observable([]);

    var lblUsername = ko.observable('');
    var lblLastLogin = ko.observable('');

    var isSupervisor = ko.computed(function () {
        return VTPortal.roApp.impersonatedIDEmployee != -1;
    })

    var supervisorBadge = ko.computed(function () {
        //if ((typeof requestsSupervisor() != 'undefined' && requestsSupervisor() != 0) || (typeof alertsSupervisor() != 'undefined' && alertsSupervisor() != 0)) {
        if (VTPortalUtils.utils.badgeCount(true) > 0) {
            return "!";
        }
        else {
            return "";
        }
    })

    function setCustomImage(image, position, opacity) {
        // var elF = document.getElementById('customPhoto');
        var tOpacity = opacity / 10;

        if (image == "") {
            image = "../2/Images/path2994-7-7-6.png";
        }
        var tFileContent = image;

        if (position == null) {
            return 'background: url(' + tFileContent + ') no-repeat center; background-size: contain !important;height:100%;opacity:' + tOpacity + '';
        }
        else {
            switch (position) {
                case 4:
                    return 'background: url(' + tFileContent + ') no-repeat center; background-size: cover !important;height:100%;opacity:' + tOpacity + '';
                    break;
                case 3:
                    return 'background: url(' + tFileContent + ') no-repeat right; background-size: contain !important;height:100%;opacity:' + tOpacity + '';
                    break;
                case 2:
                    return 'background: url(' + tFileContent + ') no-repeat center; background-size: contain !important;height:100%;opacity:' + tOpacity + '';
                    break;
                case 1:
                    return 'background: url(' + tFileContent + ') no-repeat left; background-size: contain !important;height:100%;opacity:' + tOpacity + '';
                    break;
                default:
                    return 'background: url(' + tFileContent + ') no-repeat center; background-size: contain !important;height:100%;opacity:' + tOpacity + '';
                    break;
            }
        }
        return '';
    }

    function setCustomColor(colorLeft, colorRight) {
        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (colorLeft == null || colorLeft == "") {
                colorLeft = "#FF5C35";
            }
            if (colorRight == null || colorRight == "") {
                colorRight = "#f8aa32";
            }
        }
        else {
            colorLeft = "#5b757d";
            colorRight = "#9ac5d2";
        }

        //var el = document.getElementById('customBackground');
        return 'background: linear-gradient(to right, ' + colorLeft + ', ' + colorRight + ');background-size: contain !important;';
    }

    var viewShown = function () {
        //if (VTPortal.roApp.HasCustomHeader()) {
        //    var customHeader = JSON.parse(VTPortal.roApp.db.settings.wsBackground);

        //    setCustomColor(customHeader.LeftColor, customHeader.RightColor);
        //    setCustomImage(customHeader.Image, customHeader.Position, customHeader.Opacity);
        //}

        var itemIndex = 1;
        refreshMenuItems([]);
        //Supervisores UNICOS
        if (VTPortal.roApp.db.settings.onlySupervisor) {
            if (VTPortal.roApp.impersonatedIDEmployee == -1) {
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 15, "Name": i18nextko.t('roHomeMyTeamEmployees') });
                itemIndex = itemIndex + 1;
                if (VTPortal.roApp.userStatus() == null) {
                    refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 16, "Name": i18nextko.t('roHomeMyTeamAlerts') });
                    itemIndex = itemIndex + 1;
                } else {
                    if (typeof VTPortal.roApp.userStatus().HasAlertPermission == 'undefined' || VTPortal.roApp.userStatus().HasAlertPermission == true) {
                        refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 16, "Name": i18nextko.t('roHomeMyTeamAlerts') });
                        itemIndex = itemIndex + 1;
                    }
                }
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 17, "Name": i18nextko.t('roHomeMyTeamRequests') });
                itemIndex = itemIndex + 1;
                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.OnBoardings) {
                    if (VTPortal.roApp.isSaas() == true) {
                        refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 20, "Name": "onboarding" });
                        itemIndex = itemIndex + 1;
                    }
                }
            }
            else {
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 1, "Name": i18nextko.t('roHomeTitle') });
                itemIndex = itemIndex + 1;
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 2, "Name": i18nextko.t('roGHMenuTitle') });
                itemIndex = itemIndex + 1;
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 3, "Name": i18nextko.t('roPunchesTitle') });
                itemIndex = itemIndex + 1;
                try {
                    if ((VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Requests[3].Permission == true || VTPortal.roApp.empPermissions().Requests[14].Permission == true))) {
                        refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 4, "Name": i18nextko.t('roHomeWorkingTitle') });
                        itemIndex = itemIndex + 1;
                    }
                }
                catch (e) { }
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 6, "Name": i18nextko.t('roHomeLeavesTitle') });
                itemIndex = itemIndex + 1;
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 7, "Name": i18nextko.t('roHomeProfile') });
                itemIndex = itemIndex + 1;
            }
        }
        //Resto Supervisores
        else if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
            if (VTPortal.roApp.impersonatedIDEmployee == -1) {
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 1, "Name": i18nextko.t('roHomeTitle') });
                itemIndex = itemIndex + 1;
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 18, "Name": i18nextko.t('roHomeTeam'), "badge": supervisorBadge });
                itemIndex = itemIndex + 1;
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 2, "Name": i18nextko.t('roGHMenuTitle') });
                itemIndex = itemIndex + 1;
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 3, "Name": i18nextko.t('roPunchesTitle') });
                itemIndex = itemIndex + 1;
                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.OnBoardings) {
                    if (VTPortal.roApp.isSaas() == true) {
                        refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 20, "Name": "onboarding" });
                        itemIndex = itemIndex + 1;
                    }
                }
                //if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.OnBoardings) {
                //    refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 21, "Name": i18nextko.t('roHomeSurvey') });
                //    itemIndex = itemIndex + 1;
                //}
                if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
                    refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 19, "Name": i18nextko.t('roHomeCommunique') });
                    itemIndex = itemIndex + 1;
                }
                try {
                    if ((VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Requests[3].Permission == true || VTPortal.roApp.empPermissions().Requests[14].Permission == true))) {
                        refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 4, "Name": i18nextko.t('roHomeWorkingTitle') });
                        itemIndex = itemIndex + 1;
                    }
                }
                catch (e) { }

                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 5, "Name": i18nextko.t('roHomeDocumentsTitle') });
                itemIndex = itemIndex + 1;

                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 6, "Name": i18nextko.t('roHomeLeavesTitle') });
                itemIndex = itemIndex + 1;

                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 7, "Name": i18nextko.t('roHomeProfile') });
                itemIndex = itemIndex + 1;
            }
            else {
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 1, "Name": i18nextko.t('roHomeTitle') });
                itemIndex = itemIndex + 1;
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 2, "Name": i18nextko.t('roGHMenuTitle') });
                itemIndex = itemIndex + 1;
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 3, "Name": i18nextko.t('roPunchesTitle') });
                itemIndex = itemIndex + 1;
                try {
                    if ((VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Requests[3].Permission == true || VTPortal.roApp.empPermissions().Requests[14].Permission == true))) {
                        refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 4, "Name": i18nextko.t('roHomeWorkingTitle') });
                        itemIndex = itemIndex + 1;
                    }
                }
                catch (e) { }
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 6, "Name": i18nextko.t('roHomeLeavesTitle') });
                itemIndex = itemIndex + 1;
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 7, "Name": i18nextko.t('roHomeProfile') });
                itemIndex = itemIndex + 1;
            }
        }
        //EMPLEADOS
        else {
            refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 1, "Name": i18nextko.t('roHomeTitle') });
            itemIndex = itemIndex + 1;

            refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 2, "Name": i18nextko.t('roGHMenuTitle') });
            itemIndex = itemIndex + 1;
            refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 3, "Name": i18nextko.t('roPunchesTitle') });
            itemIndex = itemIndex + 1;

            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
                refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 19, "Name": i18nextko.t('roHomeCommunique') });
                itemIndex = itemIndex + 1;
            }
            try {
                if ((VTPortal.roApp.empPermissions() != null && (VTPortal.roApp.empPermissions().Requests[3].Permission == true || VTPortal.roApp.empPermissions().Requests[14].Permission == true))) {
                    refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 4, "Name": i18nextko.t('roHomeWorkingTitle') });
                    itemIndex = itemIndex + 1;
                }
            }
            catch (e) { }
            refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 5, "Name": i18nextko.t('roHomeDocumentsTitle') });
            itemIndex = itemIndex + 1;

            refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 6, "Name": i18nextko.t('roHomeLeavesTitle') });
            itemIndex = itemIndex + 1;

            refreshMenuItems().push({ "ID": itemIndex, "ActionIndex": 7, "Name": i18nextko.t('roHomeProfile') });
            itemIndex = itemIndex + 1;
        }

        mainMenuItems(refreshMenuItems());

        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (VTPortal.roApp.userStatus() != null) {
                lblUsername(VTPortal.roApp.userStatus().EmployeeName);
            }
            if (VTPortal.roApp.LastLogin() != null) {
                lblLastLogin(i18nextko.i18n.t('roLastLogin') + ": " + moment(VTPortal.roApp.LastLogin()).format('DD MMMM YYYY, HH:mm:ss'));
            }
        }
        else {
            lblUsername(VTPortal.roApp.impersonatedNameEmployee);
        }
    }

    var employeeImage = ko.computed(function () {
        var backgroundImage = 'url(data:image/png;base64,' + VTPortal.roApp.userPhoto() + ') no-repeat center center';
        return backgroundImage;
    });

    var statusIcon = ko.computed(function () {
        if (!VTPortal.roApp.lastRequestFailed()) return "sandClock";
        else return "noConnection";
    })

    var photoVisible = ko.computed(function () {
        if (VTPortal.roApp.db.settings.onlySupervisor && VTPortal.roApp.impersonatedIDEmployee == -1) return false;
        else return true;
    })

    var bBottom = ko.computed(function () {
        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                    return ""; //"3px solid green";
                } else {
                    return "";//"3px solid red";
                }
            } else return '';
        } else {
            return ""; //"3px solid #6b96eb";
        }
    });

    var presenceTextColor = ko.computed(function () {
        if (!VTPortal.roApp.lastRequestFailed()) {
            if (VTPortal.roApp.userStatus() != null) {
                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                    return "green";
                } else {
                    return "red";
                }
            } else return 'red';
        } else {
            return 'red';
        }
    });

    var presenceTime = ko.computed(function () {
        if (!VTPortal.roApp.lastRequestFailed()) {
            if (VTPortal.roApp.userStatus() != null) {
                var sDate = moment(VTPortal.roApp.userStatus().ServerDate);
                var lpDate = moment(VTPortal.roApp.userStatus().LastPunchDate);
                var difference = sDate.diff(lpDate);
                return moment.duration(difference).humanize();
            } else return '';
        }
        else return i18nextko.i18n.t('roWithoutConnection');
    });

    var employeeStatusTime = ko.computed(function () {
        moment(lpDate).format('D MM YYYY, h:mm:ss a');

        if (!VTPortal.roApp.lastRequestFailed()) {
            if (VTPortal.roApp.userStatus() != null) {
                var sDate = moment(VTPortal.roApp.userStatus().ServerDate);
                var lpDate = moment(VTPortal.roApp.userStatus().LastPunchDate);
                var difference = sDate.diff(lpDate);
                //var differentHumanize = moment.duration(difference).humanize();
                var differentHumanize = moment.utc(difference).format('HH:mm [horas]');
                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                    return i18nextko.i18n.t('roInFrom') + moment(lpDate).format('HH:mm') + ' (' /*+ i18nextko.i18n.t('roFrom')*/ + differentHumanize + 'h)'; //"3px solid green";
                } else {
                    return i18nextko.i18n.t('roOutFrom') + moment(lpDate).format('HH:mm') + ' (' /*+ i18nextko.i18n.t('roFrom')*/ + differentHumanize + 'h)';//"3px solid red";
                }
            } else return '';
        }
        else return i18nextko.i18n.t('roWithoutConnection');
    })
    var serverTime = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            var sDate = moment(VTPortal.roApp.userStatus().ServerDate);
            return sDate.format('LT')
        } else return '';
    });

    var buttonVisible = ko.computed(function () {
        if (VTPortal.roApp.showLogoutHome() != null && VTPortal.roApp.db.settings.ApiVersion > VTPortalUtils.apiVersion.CauseNote && VTPortal.roApp.impersonatedIDEmployee == -1) {
            return VTPortal.roApp.showLogoutHome()
        } else return false;
    });

    var serverDate = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            var sDate = moment(VTPortal.roApp.userStatus().ServerDate);
            return sDate.format('dddd DD MMM');
        } else return '';
    });

    var welcome = ko.computed(function () {
        if (moment().hour() < 13 && moment().hour() > 5) { return i18nextko.i18n.t('roWelcomeMorning'); }
        else if (moment().hour() > 12 && moment().hour() < 21) { return i18nextko.i18n.t('roWelcomeAfter'); }
        else { return i18nextko.i18n.t('roWelcomeNight'); }
    });

    var productivTask = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            if (VTPortal.roApp.userStatus().TaskTitle.length <= 50) {
                return VTPortal.roApp.userStatus().TaskTitle;
            } else {
                return VTPortal.roApp.userStatus().TaskTitle.substr(0, 48) + '...';
            }
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

    var costCenter = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            return VTPortal.roApp.userStatus().CostCenterName;
        } else return '';
    });

    var customCssBack = ko.computed(function () {
        if (VTPortal.roApp.customBackground() != null) {
            VTPortal.roApp.HasCustomHeader(true);
            return setCustomColor(VTPortal.roApp.customBackground().LeftColor, VTPortal.roApp.customBackground().RightColor);
        }
        else {
            VTPortal.roApp.HasCustomHeader(false);
            return "";
        }
    });
    var customPhotoBack = ko.computed(function () {
        if (VTPortal.roApp.customBackground() != null) {
            VTPortal.roApp.HasCustomHeader(true);
            return setCustomImage(VTPortal.roApp.customBackground().Image, VTPortal.roApp.customBackground().Position, VTPortal.roApp.customBackground().Opacity);
        }
        else {
            VTPortal.roApp.HasCustomHeader(false);
            return "";
        }
    });
    function onLogoutClicked() {
        if (VTPortal.roApp.loggedIn) {
            new WebServiceRobotics(function (result) {
                window.VTPortalUtils.utils.onLogoutSuccessFunc(result);
            }, function (error) {
                window.VTPortalUtils.utils.onLogoutErrorFunc(error);;
            }).logout();
        }
    }

    var viewModel = {
        viewShown: viewShown,
        changeUserPhotoVisible: changeUserPhotoVisible,
        empImage: employeeImage,
        borderBottom: bBottom,
        myTime: presenceTime,
        lblEmployeeStatusTime: employeeStatusTime,
        textColor: presenceTextColor,
        lblWelcome: welcome,
        photoVisible: photoVisible,
        lblUsername: lblUsername,
        lblLastLogin: lblLastLogin,
        lblServerTime: serverTime,
        lblServerDate: serverDate,
        lblProfile: i18nextko.t('roProfileOf'),
        isSupervisor: isSupervisor,
        lblProductivTask: productivTask,
        lblProfileImage: i18nextko.t('roProfileImage'),
        selectPhotoEvent: selectPhotoEvent,
        imageSrc: formImageUri,
        appMode: VTPortal.roApp.isModeApp(),
        lblCostCenter: costCenter,
        statusIcon: statusIcon,
        cssBackground: ko.computed(function () {
            if (VTPortal.roApp.impersonatedIDEmployee != -1) {
                return 'backgroundOpacityImp';
            }
            else {
                return 'backgroundOpacity';
            }
        }),
        customCssBackground: customCssBack,
        customPhotoBackground: customPhotoBack,
        btnCancelUpload: {
            onClick: function () { changeUserPhotoVisible(false); },
            text: i18nextko.t('roCancel'),
        },
        btnAcceptUpload: {
            onClick: function () { uploadUserPhoto(); },
            text: i18nextko.t('roSaveRequest'),
        },
        btnLogout: {
            onClick: onLogoutClicked,
            text: '',
            icon: "Images/Actions/exit.png",
            visible: false
        },
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
        photoPopover: {
            target: ('#employeePhotoDiv'),
            shading: true,
            width: 200,
            shadingColor: "rgba(0, 0, 0, 0.5)",
            visible: VTPortal.roApp.showPopover
        },
        photoHelp: i18nextko.t('roPhotoHelp'),
        tabPanelOptionsMain: {
            dataSource: mainMenuItems,
            selectedIndex: VTPortal.roApp.selectedTab() - 1,
            itemTemplate: "title",
            selectOnFocus: false,
            //onSelectionChanged: function (e) {
            //    switch (e.addedItems[0].ActionIndex) {
            //        case 19:
            //            VTPortal.app.navigate("communiques", { root: true });
            //            break;

            //        default:
            //            //VTPortalUtils.utils.notifyMesage("not implemented", 'error', 2000)
            //            break;

            //    }

            //},
            onItemClick: function (e) {
                VTPortal.roApp.selectedTab(e.itemData.ID);

                switch (e.itemData.ActionIndex) {
                    case 1:
                        VTPortal.app.navigate("home", { root: true });
                        break;
                    case 2:
                        VTPortal.app.navigate("scheduler/1/" + moment().format("YYYY-MM-DD"), { root: true });
                        break;
                    case 3:
                        VTPortal.app.navigate("punchManagement/1/" + moment().format("YYYY-MM-DD"), { root: true });
                        break;
                    case 4:
                        VTPortal.app.navigate("teleworking/1/" + moment().format("YYYY-MM-DD"), { root: true });
                        break;
                    case 5:
                        VTPortal.app.navigate("documents", { root: true });
                        break;
                    case 6:
                        VTPortal.app.navigate("leaves", { root: true });
                        break;
                    case 7:
                        VTPortal.app.navigate("profile", { root: true });
                        break;
                    case 15:
                        VTPortal.app.navigate("myTeamEmployees", { root: true });
                        break;
                    case 16:
                        VTPortal.app.navigate("myTeamAlerts", { root: true });
                        break;
                    case 17:
                        VTPortal.app.navigate("myTeamRequests", { root: true });
                        break;
                    case 18:
                        VTPortal.app.navigate("myteam", { root: true });
                        break;
                    case 19:
                        VTPortal.app.navigate("communiques", { root: true });
                        break;
                    case 20:
                        VTPortal.app.navigate("onboardings", { root: true });
                        break;
                    case 21:
                        VTPortal.app.navigate("surveyDetail", { root: true });
                        break;
                    default:
                        //VTPortalUtils.utils.notifyMesage("not implemented", 'error', 2000)
                        break;
                }
            }
        }
    };

    return viewModel;
};