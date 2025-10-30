VTPortal.myRequests = function (params) {
    var modelIsReady = ko.observable(false);
    var requestsDS = ko.observable([]);
    var popoverVisible = ko.observable(false);
    var showOptionsImage = ko.observable('Images/Common/more.png');
    var noPermissions = VTPortal.noPermissions();
    var dataFiltered = ko.observable(false);

    var filterDescription = ko.computed(function () {
        if (VTPortal.roApp.requestFilters() != null) {
            return i18nextko.i18n.t('roFilteringData');
        } else {
            return '';
        }
    })

    function viewShown(selDate) {
        refreshData();
        modelIsReady(true);
        if (!dataFiltered()) {
            $('#scrollview').height($('#panelsContent').height() - 70);
        } else {
            $('#scrollview').height($('#panelsContent').height() - 94);
        }
    };

    var hasPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Requests != null) {
            var tmpPermission = false;
            for (var i = 0; i < VTPortal.roApp.empPermissions().Requests.length; i++) {
                if (VTPortal.roApp.empPermissions().Requests[i].Permission) {
                    tmpPermission = true;
                    break;
                }
            }

            return tmpPermission;
        } else {
            return false;
        }
    })

    function refreshData() {
        dataFiltered(false);
        var uFilter = VTPortal.roApp.requestFilters();

        var callbackFuntion = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var requestsInfo = result.Requests;
                for (var i = 0; i < requestsInfo.length; i++) {
                    requestsInfo[i].Name = i18nextko.i18n.t('roRequestType_' + requestsInfo[i].RequestType);

                    if (result.CanCreateRequest) {
                        requestsInfo[i].hasAction = true;
                    } else {
                        requestsInfo[i].hasAction = false;
                    }

                    var cssStatusClass = '';
                    switch (requestsInfo[i].Status) {
                        case 0:
                            cssStatusClass = 'ro_Status_pending';
                            requestsInfo[i].StatusDesc = i18nextko.i18n.t('roRequestStatus_pending');;
                            break;
                        case 1:
                            cssStatusClass = 'ro_Status_ongoing';
                            requestsInfo[i].StatusDesc = i18nextko.i18n.t('roRequestStatus_running');
                            break;
                        case 2:
                            cssStatusClass = 'ro_Status_accepted';
                            requestsInfo[i].StatusDesc = i18nextko.i18n.t('roRequestStatus_approved');
                            break;
                        case 3:
                            cssStatusClass = 'ro_Status_denied';
                            requestsInfo[i].StatusDesc = i18nextko.i18n.t('roRequestStatus_denied');
                            break;
                        case 4:
                            cssStatusClass = 'ro_Status_canceled';
                            requestsInfo[i].StatusDesc = i18nextko.i18n.t('roRequestStatus_canceled');
                            requestsInfo[i].hasAction = false;
                            break;
                    }

                    var cssClass = '';
                    switch (requestsInfo[i].IdRequestType) {
                        case 1:
                            cssClass = 'dx-icon-userFieldChange';
                            break;
                        case 4:
                            cssClass = 'dx-icon-externalWorkResume';
                            break;
                        case 5:
                            cssClass = 'dx-icon-changeShift';
                            break;
                        case 6:
                            //cssClass = 'dx-icon-holidays';
                            cssClass = 'dx-icon-plannedHoliday';
                            break;
                        case 8:
                            cssClass = 'dx-icon-shiftExchange';
                            break;
                        case 7:
                            cssClass = 'dx-icon-plannedAbsences';
                            break;
                        case 11:
                            cssClass = 'dx-icon-holidaysCancel';
                            break;
                        case 13:
                            cssClass = 'dx-icon-plannedHoliday';
                            break;
                        case 15:
                            cssClass = 'dx-icon-externalWorkWeekResume';
                            break;
                    }
                    requestsInfo[i].cssContainerClass = cssStatusClass;
                    requestsInfo[i].cssClass = cssClass;

                    $('#scrollview').height($('#panelsContent').height());
                }
                requestsDS(requestsInfo);
            } else {
                requestsDS([]);
                var onContinue = function () {
                    VTPortal.roApp.loadInitialData(false, false, true, false, false);
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        };

        if (uFilter == null) {
            dataFiltered(false);
            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getMyRequests(false, moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').add(1, 'day'), '0*1*2*3*4|1*5*6*7*8*11*13', 'RequestDate DESC', moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').add(1, 'year'));
        } else {
            dataFiltered(true);
            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getMyRequests(false, uFilter.filter.iDate, uFilter.filter.eDate, uFilter.filter.status.join('*') + '|' + uFilter.filter.types.join('*'), uFilter.order.by + ' ' + uFilter.order.direction, uFilter.filter.iRequestedDate, uFilter.filter.eRequestedDate);
        }
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
        VTPortal.app.navigate("RequestFilters/0");
    }

    var onBtnNewRequests = function (e) {
        popoverVisible(false);
        VTPortal.app.navigate("requestsList/1,5,6,7,8,11,13/null");
    }

    var viewModel = {
        modelIsReady: modelIsReady,
        initializeView: viewShown,
        hasPermission: hasPermission,
        noPermissions: noPermissions,
        requestsDS: requestsDS,
        dataFiltered: dataFiltered,
        filterDescription: filterDescription,
        listRequests: {
            dataSource: requestsDS,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'RequestItem',
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    switch (info.itemData.IdRequestType) {
                        case 1:
                            VTPortal.app.navigate('userFields/' + info.itemData.Id);
                            break;
                        case 5:
                            VTPortal.app.navigate('changeShift/' + info.itemData.Id);
                            break;
                        case 6:
                            VTPortal.app.navigate('plannedHoliday/' + info.itemData.Id);
                            break;
                        case 7:
                            VTPortal.app.navigate('plannedAbsence/' + info.itemData.Id);
                            break;
                        case 8:
                            VTPortal.app.navigate('shiftExchange/' + info.itemData.Id);
                            break;
                        case 11:
                            VTPortal.app.navigate('cancelHolidays/' + info.itemData.Id);
                            break;
                        case 13:
                            VTPortal.app.navigate('plannedHoliday/' + info.itemData.Id);
                            break;
                        case 15:
                            VTPortal.app.navigate('externalWorkWeekResume/' + info.itemData.Id);
                            break;
                    }
                }
            }
        },
        actionMenuPopover: {
            target: ('#btnActionMenuPopover'),
            position: "top",
            shading: true,
            shadingColor: "rgba(0, 0, 0, 0.5)",
            visible: popoverVisible,
            onHidden: onHidePopover
        },
        showOptions: {
            onClick: onCellClicked,
            text: '',
            icon: showOptionsImage
        },
        btnFilterRequests: {
            onClick: onBtnFilterRequests,
            text: '',
            hint: i18nextko.i18n.t('roFilter'),
            icon: "Images/Common/filter.png",
        },
        btnNewRequests: {
            onClick: onBtnNewRequests,
            text: '',
            hint: i18nextko.i18n.t('roNew'),
            icon: "Images/Common/plus.png",
            visible: VTPortal.roApp.ReadMode() == false
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};