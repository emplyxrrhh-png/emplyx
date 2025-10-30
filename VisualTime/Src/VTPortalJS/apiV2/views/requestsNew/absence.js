VTPortal.absence = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var selectedDate = ko.observable(moment().add(-1, 'days').toDate());
    var requestId = ko.observable(null);
    var request = ko.observable();
    var overlays = ko.observable([]);
    var employeeAccruals = ko.observable([]);
    var viewActions = ko.observable(false);
    var lblAvailable = ko.observable(0);
    var lblPending = ko.observable(0);
    var lblApproved = ko.observable(0);
    var lblNotAvailable = ko.observable(0);
    var lblExpired = ko.observable(0);
    var lblFinal = ko.observable(0);
    var isAnnualWork = ko.observable(false);
    var infoDivVisible = ko.observable(false);
    var accrualsDivVisible = ko.observable(false);
    var overlaysDivVisible = ko.observable(false);
    var daysDetailDiv = ko.observable(false);
    var noOverlaysDivVisible = ko.observable(false);
    var popupVisible = ko.observable(false);
    var popupRequestDaysVisible = ko.observable(false);
    var remarks = ko.observable('');
    var myApprovalsBlock = VTPortal.approvalHistory();
    var viewHistory = ko.observable(false);
    var requestMainDivVisible = ko.observable(false);
    var loadingDivVisible = ko.observable(false);

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: []
    }));

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    var lblEmployee = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().EmployeeName;
        }
        else {
            '';
        }
    });
    var lblRequestDates = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().RequestInfo;
        }
        else {
            '';
        }
    });
    var lblGroupName = ko.computed(function () {
        if (typeof request() != 'undefined') {
            return request().EmployeeGroup;
        }
        else {
            '';
        }
    });

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                loadingDivVisible(false);
                requestMainDivVisible(true);

                request(result);

                if (result.RequestDays != null) {
                    if (result.RequestDays.length > 1) {
                        var ds = []
                        result.RequestDays.forEach(function (item, index) {
                            ds.push({ RequestDate: moment(item.RequestDate).format('dddd DD MMMM') });
                        });
                        dataSource(new DevExpress.data.DataSource({
                            store: ds
                        }));
                        daysDetailDiv(true);
                    }
                    else {
                        daysDetailDiv(false);
                    }
                }
                else {
                    daysDetailDiv(false);
                }

                if (result.RequestType == 6) {
                    accrualsDivVisible(true);
                    employeeAccruals(result.EmployeeAccruals);
                    lblApproved(result.EmployeeAccruals.Lasting)
                    lblAvailable(result.EmployeeAccruals.Available)
                    lblPending(result.EmployeeAccruals.Pending)
                    isAnnualWork(result.IsAnnualWork);
                    if (result.IsAnnualWork) {
                        lblNotAvailable(result.EmployeeAccruals.WithoutEnjoyment)
                        lblExpired(result.EmployeeAccruals.Expired)
                        const final = result.EmployeeAccruals.Available - result.EmployeeAccruals.Pending - result.EmployeeAccruals.Lasting - result.EmployeeAccruals.Expired - result.EmployeeAccruals.WithoutEnjoyment;
                        lblFinal(final)
                    }
                }
                else {
                    accrualsDivVisible(false);
                }
                if (result.Comments != "") {
                    infoDivVisible(true);
                    remarks(result.Comments);
                }
                else {
                    infoDivVisible(false);
                }

                viewActions(false);
                if (result.ReqStatus == 0 || result.ReqStatus == 1) {
                    viewActions(true);
                }

                myApprovalsBlock.refreshApprovals(result.RequestsHistoryEntries);
            } else {
                var onContinue = function () {
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }, function (error) {
            var onContinue = function () {
                VTPortal.roApp.redirectAtHome();
            }
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequest"), 'error', 0, onContinue);
        }).getRequestForSupervisor(requestId());
    }

    var employeeImage = ko.computed(function () {
        if (typeof request() != 'undefined') {
            var backgroundImage = '';
            if (request().EmployeeImage != '') {
                backgroundImage = 'url(data:image/png;base64,' + request().EmployeeImage + ')';
            }
            else {
                backgroundImage = 'url(Images/logovtl.ico)';
            }

            return backgroundImage;
        }
        else {
            return '';
        }
    });

    var getOverlappingInfo = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                if (result.Overlays.length > 0) {
                    overlaysDivVisible(true);
                    noOverlaysDivVisible(false);
                    for (i = 0; i < result.Overlays.length; i++) {
                        if (result.Overlays[i].EmployeeImage == "") {
                            result.Overlays[i].EmployeeImage = result.DefaultImage.replace("no-repeat center center", "");
                        }
                        else {
                            result.Overlays[i].EmployeeImage = result.Overlays[i].EmployeeImage.replace("no-repeat center center", "");
                        }
                    }

                    overlays(result.Overlays);
                }
                else {
                    overlaysDivVisible(false);
                    noOverlaysDivVisible(true);
                }
            } else {
            }
        }).getOverlappingEmployees(requestId());
    }

    function refreshData() {
        loadingDivVisible(true);
        loadRequest();
        getOverlappingInfo();
    }

    function viewShown() {
        globalStatus().viewShown();
        refreshData();
    };

    var reqApprovedText = ko.computed(function () {
        if (typeof request() != 'undefined' && request().RequestType == 6) {
            return i18nextko.i18n.t("lblApprovedHolidaysText");
        }
        else {
            return i18nextko.i18n.t("lblApprovedText");
        }
    });

    var reqPendingText = ko.computed(function () {
        if (typeof request() != 'undefined' && request().RequestType == 6) {
            return i18nextko.i18n.t("lblPendingHolidaysText");
        }
        else {
            return i18nextko.i18n.t("lblPendingText");
        }
    });

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roRequestType_PlannedAbsences'),
        subscribeBlock: globalStatus(),
        empImage: employeeImage,
        lblEmployee: lblEmployee,
        detailsVisible: function () {
            popupRequestDaysVisible(true);
        },
        lblRequestDates: lblRequestDates,
        lblOverlays: i18nextko.i18n.t('lblOverlays'),
        lblAccruals: i18nextko.i18n.t('roMyAccrualsHint'),
        lblNoOverlays: i18nextko.i18n.t('lblNoOverlays'),
        lblAvailable: lblAvailable,
        lblPending: lblPending,
        lblApproved: lblApproved,
        lblNotAvailable: lblNotAvailable,
        lblExpired: lblExpired,
        lblFinal: lblFinal,
        lblRemarks: i18nextko.i18n.t('lblRemarks'),
        lblApprovedText: reqApprovedText,
        lblPendingText: reqPendingText,
        lblAvailableText: i18nextko.i18n.t('lblAvailableText'),
        lblNotAvailableText: i18nextko.i18n.t('lblNotAvailableText'),
        lblExpiredText: i18nextko.i18n.t('lblExpiredText'),
        lblFinalText: i18nextko.i18n.t('lblFinalText'),
        lblRequestedDays: i18nextko.i18n.t('lblRequestedDays'),
        lblLoading: i18nextko.i18n.t('lblLoading'),
        lblGroupName: lblGroupName,
        isAnnualWork: isAnnualWork,
        remarks: remarks,
        myApprovalsBlock: myApprovalsBlock,
        popupVisible: popupVisible,
        popupRequestDaysVisible: popupRequestDaysVisible,
        accrualsDivVisible: accrualsDivVisible,
        infoDivVisible: infoDivVisible,
        requestMainDivVisible: requestMainDivVisible,
        daysDetailDiv: daysDetailDiv,
        loadingDivVisible: loadingDivVisible,
        overlaysDivVisible: overlaysDivVisible,
        noOverlaysDivVisible: noOverlaysDivVisible,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        scrollContent: {
        },
        listRequestDays: {
            dataSource: dataSource,
            scrollingEnabled: true,
            grouped: false,
            height: 300,
            itemTemplate: 'RequestItem'
        },
        overlays: {
            items: overlays,
            height: 175,
            direction: "horizontal",
            baseItemHeight: 75,
            baseItemWidth: 75,
            itemMargin: 10,
            itemTemplate(itemData, itemIndex, itemElement) {
                if (itemData.IsRequest == true) {
                    itemElement.append(`<div class="imageTileviewRequest" id="div_${itemData.IdEmployee}_${itemData.IdCounter}"><div class="textTileview">${itemData.EmployeeName
                        }</div><div class="imageTileviewRequest" style="border: 3px solid #0046FE; background-image: ${itemData.EmployeeImage}"></div></div>`);
                    var global = $("<div>")
                    var a = $("<div style='margin:10px; font-size: 10px;'>").html(itemData.AbsenceResume);
                    global.append(a);
                    global.dxPopover({
                        target: `#div_${itemData.IdEmployee}_${itemData.IdCounter}`,
                        showEvent: 'dxclick',
                        position: 'top',
                        width: 300
                    }).appendTo(itemElement);
                }
                else {
                    itemElement.append(`<div class="imageTileview" id="div_${itemData.IdEmployee}_${itemData.IdCounter}"><div class="textTileview">${itemData.EmployeeName
                        }</div><div class="imageTileview" style="background-image: ${itemData.EmployeeImage}"></div></div>`);
                    var global = $("<div>")
                    var a = $("<div style='margin:10px; font-size: 10px;'>").html(itemData.AbsenceResume);
                    global.append(a);
                    global.dxPopover({
                        target: `#div_${itemData.IdEmployee}_${itemData.IdCounter}`,
                        showEvent: 'dxclick',
                        position: 'top',
                        width: 300
                    }).appendTo(itemElement);
                }
            },
        },
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequestNew(requestId(), request().RequestType, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequestNew(requestId(), request().RequestType, false); },
            text: '',
            hint: i18nextko.i18n.t('roRefuse'),
            icon: "Images/Common/refuse.png",
            visible: viewActions
        },
        btnHistory: {
            onClick: function () { popupVisible(true); },
            text: '',
            hint: i18nextko.i18n.t('roLblRequestApprovals'),
            icon: "Images/Common/historic.png",
            visible: viewHistory
        },
    };

    return viewModel;
};