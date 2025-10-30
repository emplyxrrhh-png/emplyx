VTPortal.documents = function (params) {
    var documentsDS = ko.observable([]);
    var popoverVisible = ko.observable(false);
    var showOptionsImage = ko.observable('Images/Common/more.png');
    var document = ko.observable();
    var showPopupSign = ko.observable(false);
    var noPermissions = VTPortal.noPermissions();
    var showPopupSignApp = ko.observable(false);
    var optionsVisible = ko.observable(false);
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var options = [
        { text: i18nextko.i18n.t('roSignFile') },
        { text: i18nextko.i18n.t('roDownloadFile') }
    ];
    var requieresSign = ko.computed(function () {
        if (VTPortal.roApp.isTimeGate() || VTPortal.roApp.db.settings.ApiVersion < VTPortalUtils.apiVersion.DocumentSign) {
            return false;
        }
        else {
            if (typeof document() != 'undefined') {
                if (document().DocumentTemplate.RequiresSigning == true) {
                    return true;
                }
            }
            else {
                return false;
            }
        }
    });

    function viewShown() {
        globalStatus().viewShown();
        refreshData();
    };

    var dataFiltered = ko.observable(false);

    var filterDescription = ko.computed(function () {
        if (VTPortal.roApp.documentsFilters() != null) {
            return i18nextko.i18n.t('roFilteringData');
        } else {
            return '';
        }
    })

    var computedScrollHeight = ko.computed(function () {
        if (!dataFiltered()) return '100%'
        else return '95%';
    });

    var hasPermission = ko.computed(function () {
        //if (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Requests != null && VTPortal.roApp.empPermissions().Requests.length > 0
        //    && VTPortal.roApp.empPermissions().Requests[3].Permission == true) {
        //    return true;
        //} else {
        //    return false;
        //}
        return true;
    })

    function refreshData() {
        dataFiltered(false);
        if (hasPermission()) {
            var uFilter = VTPortal.roApp.documentsFilters();

            var callbackFuntion = function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    var documentInfo = result.Documents;
                    for (var i = 0; i < documentInfo.length; i++) {
                        if(!VTPortal.roApp.isTimeGate()) documentInfo[i].hasAction = true;
                        documentInfo[i].Name = documentInfo[i].Title;
                        documentInfo[i].TypeName = documentInfo[i].DocumentTemplate.Name;
                        documentInfo[i].Description = i18nextko.i18n.t('roDocumentDelivered') + " " + moment.tz(documentInfo[i].DeliveredDate, VTPortal.roApp.serverTimeZone).format('LL') + " " + moment.tz(documentInfo[i].DeliveredDate, VTPortal.roApp.serverTimeZone).format('LT');
                        
                        var status = documentInfo[i].SignStatus;
                        documentInfo[i].SignDesc = "";
                        var cssClass = '';
                        switch (documentInfo[i].DocumentTemplate.Scope) {
                            case 0:
                                cssClass = 'dx-icon-Document_EmployeeField';
                                break;
                            case 1:
                                cssClass = 'dx-icon-Document_EmployeeContract';
                                break;
                            case 2:
                                cssClass = 'dx-icon-Document_Punch';
                                break;
                            case 3:
                                cssClass = 'dx-icon-Document_Reason';
                                break;
                            case 4:
                                cssClass = 'dx-icon-Document_LeaveOrPermission';
                                break;
                            case 5:
                                cssClass = 'dx-icon-Document_Company';
                                break;
                            case 6:
                                cssClass = 'dx-icon-Document_Visit';
                                break;
                        }

                        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.DocumentSign) {
                            if (documentInfo[i].DocumentTemplate.RequiresSigning == true) {
                                cssClass = 'dx-icon-Document_Sign';
                                switch (status) {
                                    case 1:
                                        documentInfo[i].SignDesc = i18nextko.i18n.t('roDocumentsStatus') + ": " + i18nextko.i18n.t('roDocPending')
                                        break;
                                    case 2:
                                        documentInfo[i].SignDesc = i18nextko.i18n.t('roDocumentsStatus') + ": " + i18nextko.i18n.t('roDocInProgress')
                                        break;
                                    case 3:
                                        documentInfo[i].SignDesc = i18nextko.i18n.t('roDocumentsStatus') + ": " + i18nextko.i18n.t('roDocSigned');
                                        break;
                                    case 4:
                                        documentInfo[i].SignDesc = i18nextko.i18n.t('roDocumentsStatus') + ": " + i18nextko.i18n.t('roDocRejected');
                                        break;
                                }
                            }
                        }

                        documentInfo[i].cssContainerClass = '';
                        documentInfo[i].cssClass = cssClass;
                    }
                    documentsDS(documentInfo);
                }
            };

            if (uFilter == null) {
                dataFiltered(false);
                new WebServiceRobotics(function (result) { callbackFuntion(result); }).getMyDocuments(null, null, '0*1*2*3*4*5*6', 'DeliveryDate DESC');
            } else {
                dataFiltered(true);
                new WebServiceRobotics(function (result) { callbackFuntion(result); }).getMyDocuments(uFilter.filter.iDate, uFilter.filter.eDate, uFilter.filter.status.join('*'), uFilter.order.by + ' ' + uFilter.order.direction);
            }
        }
    }

    function openSigningWebsite(value, idDocument) {
        if (VTPortal.roApp.isModeApp()) {
            var prefix = VTPortal.roApp.serverURL.url.replace("/api/Portalsvcx.svc", "");

            var returnUrl = btoa(prefix + "/SigningPage.aspx?GUID=" + value.GUID + "&MTClient=" + VTPortal.roApp.companyID + "&App=1&IdDocument=" + idDocument);

            var url = value.SignatureViDRemoteURL + "&url_return=" + returnUrl;

            var target = "_blank";
            var options = "usewkwebview=yes,fullscreen=yes,hidespinner=yes,hidden=yes,location=yes,hideurlbar=yes,hidenavigationbuttons=yes,toolbar=no,clearcache=no,clearsessioncache=no";

            adfsLoginAttempts = 0;
            showPopupSignApp(true);
            showExternalApp(url, target, options);
        } else {
            //var returnURLBrowser = btoa("http://" + VTPortal.roApp.db.settings.wsURL + "/VTPortal/2/indexv2.aspx?IdDocument=" + idDocument);
            showPopupSign(true);

            var prefix = window.location.origin;
            var returnURLBrowser = btoa(prefix + "/SigningPage.aspx?GUID=" + value.GUID + "&MTClient=" + VTPortal.roApp.companyID + "&App=0&IdDocument=" + idDocument)

            var url = value.SignatureViDRemoteURL + "&url_return=" + returnURLBrowser;
            showExternal(url);
        }
    }

    function showExternalApp(url, target, options) {
        setTimeout(function () {
            inAppBrowserRef = cordova.InAppBrowser.open(url, target, options);
            inAppBrowserRef.addEventListener('loadstop', function () { getSignInfo(); });
            inAppBrowserRef.addEventListener('exit', function () { exitBrowser(); });
        }, 3000);
    }

    function showExternal(url) {
        setTimeout(function () {
            window.open(url, "_self");
        }, 5000);
    }

    function exitBrowser() {
        showPopupSignApp(false);
        refreshData();
    }
    function getSignInfo() {
        if (VTPortal.roApp.wsRequestCounter() > 0) VTPortal.roApp.wsRequestCounter(0);
        if (inAppBrowserRef != null) inAppBrowserRef.show();

        timer = setTimeout(function () {
            if (inAppBrowserRef != null) {
                inAppBrowserRef.executeScript({ code: "returnToMobile()" }, signDataObtained);
            }
        }, 3000);
    }

    var signDataObtained = function (values) {
        showPopupSignApp(false);
        if (values == "1") {
            inAppBrowserRef.close();
            inAppBrowserRef = null;
            refreshData();
        }
        else {
            timer = setTimeout(function () {
                if (inAppBrowserRef != null) {
                    inAppBrowserRef.executeScript({ code: "returnToMobile()" }, signDataObtained);
                }
            }, 3000);
        }
    }

    var onBtnNewDocument = function (e) {
        popoverVisible(false);
        VTPortal.app.navigate('sendDocument/-1');
    }

    var onCellClicked = function (e) {
        popoverVisible(true);
        showOptionsImage('Images/Common/moreV.png');
    };

    var onHidePopover = function () {
        showOptionsImage('Images/Common/more.png');
    }

    var onBtnFilterRequests = function (e) {
        popoverVisible(false);
        showOptionsImage('Images/Common/more.png');
        VTPortal.app.navigate("DocumentFilters/1");
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.t('roDocumentsTitle'),
        subscribeBlock: globalStatus(),
        hasPermission: hasPermission,
        noPermissions: noPermissions,
        documentsDS: documentsDS,
        requieresSign: requieresSign,
        dataFiltered: dataFiltered,
        lblSignInfo: i18nextko.t('roSignInfo'),
        lblSignInfoApp: i18nextko.t('roSignInfoApp'),
        showPopupSign: showPopupSign,
        showPopupSignApp: showPopupSignApp,
        filterDescription: filterDescription,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        signOptions: {
            dataSource: options,
            visible: optionsVisible,
            showTitle: false,
            showCancelButton: ko.observable(true),
            onItemClick: function (e) {
                if (e.itemIndex == 0) {
                    new WebServiceRobotics(function (result) {
                        switch (result.Status) {
                            case 0:
                                openSigningWebsite(result.Value, document().Id);
                                break;
                            case -95:
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roNoDocumentUploaded'), 'info', 2000);
                                break;
                            case -96:
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roInvalidPhoneNumber'), 'info', 2000);
                                break;
                            case -97:
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roEmptyFile'), 'info', 2000);
                                break;
                            case -98:
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roDocExceeded'), 'info', 2000);
                                break;
                            case -114:
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roDocumentVidSignerKo'), 'info', 2000);
                                break;
                        }
                    }).uploadDocumenttoSign(document().Id);
                }
                else {
                    new WebServiceRobotics(function (result) {
                        window.VTPortalUtils.utils.downloadBytes(result.Value)
                    }).getDocumentBytes(document().Id);
                }
            }
        },
        listRequests: {
            dataSource: documentsDS,
            scrollingEnabled: true,
            grouped: false,
            itemTemplate: 'RequestItem',
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.DocumentSign) {
                        if (info.itemData.DocumentTemplate.RequiresSigning == true && info.itemData.SignStatus == 1) {
                            document(info.itemData);
                            optionsVisible(true);
                        }
                        else {
                            new WebServiceRobotics(function (result) {
                                window.VTPortalUtils.utils.downloadBytes(result.Value)
                            }).getDocumentBytes(info.itemData.Id);
                        }
                    }
                    else {
                        new WebServiceRobotics(function (result) {
                            window.VTPortalUtils.utils.downloadBytes(result.Value)
                        }).getDocumentBytes(info.itemData.Id);
                    }
                }
            }
        },
        scrollContent: {
            height: computedScrollHeight
        },
        showOptions: {
            onClick: onCellClicked,
            text: '',
            icon: showOptionsImage
        },
        actionMenuPopover: {
            target: ('#btnActionMenuPopover'),
            position: "top",
            shading: true,
            shadingColor: "rgba(0, 0, 0, 0.5)",
            visible: popoverVisible,
            onHidden: onHidePopover
        },
        btnFilterRequests: {
            onClick: onBtnFilterRequests,
            text: '',
            hint: i18nextko.i18n.t('roFilter'),
            icon: "Images/Common/filter.png",
        },
        btnNewDocument: {
            onClick: onBtnNewDocument,
            visible: (!VTPortal.roApp.isTimeGate() && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.SendDocuments && !VTPortal.roApp.ReadMode()),
            text: '',
            hint: i18nextko.i18n.t('roNew'),
            icon: "Images/Common/plus.png"
        }
    };

    return viewModel;
};