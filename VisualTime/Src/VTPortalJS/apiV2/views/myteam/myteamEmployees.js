VTPortal.myTeamEmployees = function (params) {
    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: VTPortal.roApp.supervisedEmployees,
        searchOperation: "contains",
        searchExpr: ["Name", "Group", "TaskTitle", "CostCenterName", "InTelecommuteDesc"]
    }));
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var loadingVisible = ko.observable(false);

    function viewShown() {
        globalStatus().viewShown();
        loadingVisible(true);
        if (licenseVisible) {
            bLicense(false);
            //licenseNotReaded(true);
        };
        var onSuccessLoad = function () {
            if (VTPortal.roApp.supervisedEmployees.length == 0) {
                var timeout = setTimeout(function () {
                    loadingVisible(false);
                }, 60000);

                new WebServiceRobotics(function (result) {
                    loadingVisible(false);
                    clearTimeout(timeout);
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        var employeesInfo = result.Employees;
                        for (var i = 0; i < employeesInfo.length; i++) {
                            if (employeesInfo[i].Image == "") employeesInfo[i].Image = result.DefaultImage;

                            var cssStatusClass = '';
                            switch (employeesInfo[i].PresenceStatus) {
                                case "Inside":
                                    cssStatusClass = 'employeeListPhoto';
                                    break;
                                case "Outside":
                                    cssStatusClass = 'employeeListPhotoOut';
                                    break;
                            }
                            employeesInfo[i].cssContainerClass = cssStatusClass;

                            if (employeesInfo[i].InTelecommute == "True") {
                                employeesInfo[i].ImageTC = 'dx-icon-taTelecommuting_HomeList';
                                employeesInfo[i].InTelecommuteDesc = "Teletrabajo casa teletrabajando tele teletreball teleworking telecommute telecommuting teleworking";
                            }
                            else {
                                employeesInfo[i].InTelecommuteDesc = "oficina office centro centre workcenter presencial on-site";
                            }

                            if (employeesInfo[i].TaskTitle != "" && employeesInfo[i].TaskTitle != null) {
                                employeesInfo[i].TaskTitle = i18nextko.i18n.t('inTask') + employeesInfo[i].TaskTitle;
                            }
                            if (employeesInfo[i].CostCenterName != "" && employeesInfo[i].CostCenterName != null) {
                                employeesInfo[i].CostCenterName = i18nextko.i18n.t('inCostCenter') + employeesInfo[i].CostCenterName;
                            }
                        }

                        VTPortal.roApp.supervisedEmployees = result.Employees;

                        dataSource(new DevExpress.data.DataSource({
                            store: employeesInfo,
                            searchOperation: "contains",
                            searchExpr: ["Name", "Group", "TaskTitle", "CostCenterName", "InTelecommuteDesc"]
                        }));
                    } else {
                        VTPortalUtils.utils.processErrorMessage(result);
                    }
                }).getMyEmployees();
            }
            else {
                loadingVisible(false);
            }
        };

        if (!VTPortal.roApp.loggedIn) {
            loadingVisible(false);
            VTPortalUtils.utils.loginIfNecessary('', onSuccessLoad);
        } else {
            onSuccessLoad();
            if (VTPortal.roApp.showLegalText() == "1" && localStorage.getItem("ShowLegalText.VTPortal") != "0") {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roENSShowLegalText'), 'info', 0, function () {
                    localStorage.setItem("ShowLegalText.VTPortal", "0");
                });
            }
        }
    }

    var listItemClick = function (info) {
        if (typeof info != 'undefined' && info != null) {
            if (VTPortal.roApp.isImpersonateEnabled()) {
                var idEmp = parseInt(info.itemData.EmployeeId, 10);
                VTPortal.roApp.impersonatedIDEmployee = idEmp;
                VTPortal.roApp.impersonatedNameEmployee = i18nextko.i18n.t('roProfileOf') + info.itemData.Name;
                VTPortal.roApp.loadInitialData(true, true, true, false, false, function () { VTPortal.roApp.redirectAtHome(); });
            } else {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roImpersonateDenied'), 'info', 0);
            }
        }
    }

    var licenseNotReaded = ko.observable(false);
    var bLicense = ko.observable(false);

    var licenseVisible = ko.computed(function () {
        return (!VTPortal.roApp.licenseAccepted() && (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.LicenseAgreement) && (VTPortal.roApp.impersonatedIDEmployee == -1));
    });

    var licenseConsent = ko.computed(function () {
        if (VTPortal.roApp.licenseConsent() == null) {
            return i18nextko.i18n.t('roLicenseText');
        } else {
            return VTPortal.roApp.licenseConsent().Message;
        }
    });

    var customConsent = ko.computed(function () {
        if (VTPortal.roApp.licenseConsent() == null) {
            return false;
        } else {
            return true;
        }
    });

    var btnAcceptLicense = function (params) {
        if (bLicense()) {
            if (VTPortal.roApp.licenseConsent() == null) {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        VTPortal.roApp.db.settings.licenseAccepted = true;
                        VTPortal.roApp.db.settings.save();
                        VTPortal.roApp.licenseAccepted(true);
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
                    }
                }).acceptMyLicense(bLicense());
            } else {
                new WebServiceRobotics(function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        VTPortal.roApp.db.settings.licenseAccepted = true;
                        VTPortal.roApp.db.settings.save();
                        VTPortal.roApp.licenseAccepted(true);
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t(window.VTPortalUtils.utils.languageTag(result.Status)), 'error', 0);
                    }
                }).acceptConsent(VTPortal.roApp.licenseConsent().Message);
            }
        }
    }

    var onBtnRefresh = function (e) {
        loadingVisible(true);
        new WebServiceRobotics(function (result) {
            loadingVisible(false);
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var employeesInfo = result.Employees;
                for (var i = 0; i < employeesInfo.length; i++) {
                    if (employeesInfo[i].Image == "") employeesInfo[i].Image = result.DefaultImage;

                    var cssStatusClass = '';
                    switch (employeesInfo[i].PresenceStatus) {
                        case "Inside":
                            cssStatusClass = 'employeeListPhoto';
                            break;
                        case "Outside":
                            cssStatusClass = 'employeeListPhotoOut';
                            break;
                    }
                    employeesInfo[i].cssContainerClass = cssStatusClass;

                    if (employeesInfo[i].InTelecommute == "True") {
                        employeesInfo[i].ImageTC = 'dx-icon-taTelecommuting_HomeList';
                    }

                    if (employeesInfo[i].TaskTitle != "" && employeesInfo[i].TaskTitle != null) {
                        employeesInfo[i].TaskTitle = i18nextko.i18n.t('inTask') + employeesInfo[i].TaskTitle;
                    }
                    if (employeesInfo[i].CostCenterName != "" && employeesInfo[i].CostCenterName != null) {
                        employeesInfo[i].CostCenterName = i18nextko.i18n.t('inCostCenter') + employeesInfo[i].CostCenterName;
                    }
                }

                VTPortal.roApp.supervisedEmployees = result.Employees;

                dataSource(new DevExpress.data.DataSource({
                    store: employeesInfo,
                    searchOperation: "contains",
                    searchExpr: ["Name", "Group", "TaskTitle", "CostCenterName"]
                }));
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        }).getMyEmployees();
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roMyTeamEmployees'),
        subscribeBlock: globalStatus(),
        listOptions: {
            dataSource: dataSource,
            grouped: false,
            indicateLoading: false,
            pageLoadMode: 'nextButton',
            height: "90%",
            itemTemplate: 'item',
            onItemClick: listItemClick
        },
        btnRefresh: {
            onClick: onBtnRefresh,
            text: '',
            icon: "Images/Common/refresh.png",
        },
        searchOptions: {
            valueChangeEvent: "keyup",
            mode: "search",
            height: "10%",
            onValueChanged: function (args) {
                dataSource().searchValue(args.value);
                dataSource().load();
            }
        },
        licenceViewContent: {
            bounceEnabled: true,
            showScrollbar: 'always',
            height: 200
            //onReachBottom: function(options){
            //    licenseNotReaded(false);
            //    options.component.release();
            //    options.component.toggleLoading();
            //}
        },
        lblTermsAndContidions: i18nextko.t('roTermsAndConditions'),
        customConsent: customConsent,
        lbllicenseText: licenseConsent,
        lbllicenseAdv1: i18nextko.t('roAgreement1'),
        lbllicenseAdv2: i18nextko.t('roAgreement2'),
        lbllicenseAdv3: i18nextko.t('roAgreement3'),
        licenseVisible: licenseVisible,
        ckLicenceAccepted: {
            text: i18nextko.t('roLicenseAccept'),
            value: bLicense,
            disabled: licenseNotReaded
        },
        btnAcceptLicense: {
            onClick: btnAcceptLicense,
            text: i18nextko.t('roSaveLicense')
        },
        loadingPanel: {
            showPane: false,
            height: 110,
            shading: true,
            shadingColor: 'rgba(255,255,255,0.5)',
            indicatorSrc: 'Images/DoubleRing.gif',
            visible: loadingVisible
        }
    };

    return viewModel;
};