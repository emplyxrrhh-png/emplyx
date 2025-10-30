VTPortal.myPunchRequests = function (params) {
    var modelIsReady = ko.observable(false);
    var requestsDS = ko.observable([]);
    var popoverVisible = ko.observable(false);
    var showOptionsImage = ko.observable('Images/Common/more.png');
    var noPermissions = VTPortal.noPermissions();
    var dataFiltered = ko.observable(false);

    var filterDescription = ko.computed(function () {
        if (VTPortal.roApp.punchesFilters() != null) {
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
        var uFilter = VTPortal.roApp.punchesFilters();

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
                            break;
                        case 1:
                            cssStatusClass = 'ro_Status_ongoing';
                            break;
                        case 2:
                            cssStatusClass = 'ro_Status_accepted';
                            break;
                        case 3:
                            cssStatusClass = 'ro_Status_denied';
                            break;
                    }

                    var cssClass = '';
                    switch (requestsInfo[i].IdRequestType) {
                        case 2:
                            cssClass = 'dx-icon-forbiddenPunch';
                            break;
                        case 3:
                            cssClass = 'dx-icon-justifyPunch';
                            break;
                        case 10:
                            cssClass = 'dx-icon-forbiddenPunch';
                            //cssClass = 'dx-icon-forbiddenTaskPunch';
                            break;
                        case 12:
                            cssClass = 'dx-icon-forbiddenPunch';
                            //cssClass = 'dx-icon-forbiddenCostPunch';
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
            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getMyRequests(false, moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').add(1, 'day'), '0*1*2*3|2*3*10*12', 'RequestDate DESC', moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').add(1, 'year'));
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
        VTPortal.app.navigate("RequestFilters/2");
    }

    var onBtnNewRequests = function (e) {
        popoverVisible(false);
        VTPortal.app.navigate('requestsList/2,3,10,12/null');
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
                        case 2:
                            VTPortal.app.navigate('forgottenPunch/' + info.itemData.Id);
                            break;
                        case 3:
                            VTPortal.app.navigate('justifyPunch/' + info.itemData.Id);
                            break;
                        case 10:
                            VTPortal.app.navigate('forgottenPunch/' + info.itemData.Id);
                            //VTPortal.app.navigate('forgottenTPunch/' + info.itemData.Id);
                            break;
                        case 12:
                            VTPortal.app.navigate('forgottenPunch/' + info.itemData.Id);
                            //VTPortal.app.navigate('forgottenCCPunch/' + info.itemData.Id);
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