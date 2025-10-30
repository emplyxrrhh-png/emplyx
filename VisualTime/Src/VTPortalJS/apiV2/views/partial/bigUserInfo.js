VTPortal.bigUserInfo = function (params) {

    var requestsSupervisor = ko.observable(0);
    var alertsSupervisor = ko.observable(0);
    var lblUsername = ko.observable('');
    var lblLastLogin = ko.observable('');

    var isSupervisor = ko.computed(function () {
        return VTPortal.roApp.impersonatedIDEmployee != -1;
    })

    function setCustomImage(image, position, opacity) {
        // var elF = document.getElementById('customPhoto');
        var tOpacity = opacity / 10;

        if (image == "") {
            //image = "../2/Images/path2994-7-7-6.png";
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
                colorLeft = "#0046FE";
            }
            if (colorRight == null || colorRight == "") {
                colorRight = "#286EFF";
            }
        }
        else {
            colorLeft = "#FF4B4F"; //color impersonar
            colorRight = "#FF4B4F";
        }

        //var el = document.getElementById('customBackground');
        return 'background: linear-gradient(to right, ' + colorLeft + ', ' + colorRight + ');background-size: contain !important; height: 100%;';
    }

    var viewShown = function () {
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

    var customCompanyLogo = ko.computed(function () {
        if (VTPortal.roApp.customBackground() != null) {
            return 'url(' + VTPortal.roApp.customBackground().Image + ') no-repeat center center';
        } else {
            VTPortal.roApp.HasCustomHeader(false);
            return "";
        }
    });

    var companyLogoVisible = ko.computed(function () {
        return VTPortal.roApp.customBackground() != null && VTPortal.roApp.customBackground().Image ? true : false;
    });
   
    function onLogoutClicked() {
        if (VTPortal.roApp.loggedIn) {
            new WebServiceRobotics(function (result) {
                window.VTPortalUtils.utils.onLogoutSuccessFunc(result);
            }, function (error) {
                window.VTPortalUtils.utils.onLogoutErrorFunc(error);;
            }).logout(VTPortal.roApp.db.settings.UUID);
        }
    }

    var viewModel = {
        viewShown: viewShown,
        empImage: employeeImage,
        customCompanyLogo: customCompanyLogo,
        companyLogoVisible: companyLogoVisible,
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
        btnLogout: {
            onClick: onLogoutClicked,
            text: '',
            icon: "Images/Actions/exit.png",
            visible: false
        }
    };

    return viewModel;
};