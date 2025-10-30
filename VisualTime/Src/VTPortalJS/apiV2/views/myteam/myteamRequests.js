VTPortal.myTeamRequests = function (params) {
    var itemCount = ko.observable(0);
    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: [],
        searchOperation: "contains",
        searchExpr: "Name"
    }));

    var popoverVisible = ko.observable(false);
    var dataFiltered = ko.observable(false);
    var loadingVisible = ko.observable(false);
    var filterDescription = ko.computed(function () {
        if (VTPortal.roApp.supervisorFilters() != null) {
            return i18nextko.i18n.t('roFilteringData');
        } else {
            return '';
        }
    })

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        refreshData();
    };

    function refreshData() {
        dataFiltered(false);
        loadingVisible(true);
        var uFilter = VTPortal.roApp.supervisorFilters();

        var callbackFuntion = function (result) {
            loadingVisible(false);
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var requestsInfo = result.Requests;
                for (var i = 0; i < requestsInfo.length; i++) {
                    requestsInfo[i].Name = requestsInfo[i].Name;

                    requestsInfo[i].hasAction = true;

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
                        case 1:
                            cssClass = 'dx-icon-userFieldChange';
                            break;
                        case 2:
                            cssClass = 'dx-icon-forbiddenPunch';
                            break;
                        case 3:
                            cssClass = 'dx-icon-justifyPunch';
                            break;
                        case 4:
                            cssClass = 'dx-icon-externalWorkResume';
                            break;
                        case 5:
                            cssClass = 'dx-icon-changeShift';
                            break;
                        case 6:
                            cssClass = 'dx-icon-plannedHoliday';
                            break;
                        case 7:
                            cssClass = 'dx-icon-plannedAbsences';
                            break;
                        case 8:
                            cssClass = 'dx-icon-shiftExchange';
                            break;
                        case 9:
                            cssClass = 'dx-icon-plannedCauses';
                            break;
                        case 10:
                            cssClass = 'dx-icon-forbiddenPunch';
                            break;
                        case 11:
                            cssClass = 'dx-icon-holidaysCancel';
                            break;
                        case 12:
                            cssClass = 'dx-icon-forbiddenPunch';
                            break;
                        case 13:
                            cssClass = 'dx-icon-plannedHoliday';
                            break;
                        case 14:
                            cssClass = 'dx-icon-plannedOvertime';
                            break;
                        case 15:
                            cssClass = 'dx-icon-externalWorkWeekResume';
                            break;
                        case 16:
                            cssClass = 'dx-icon-telecommute';
                            break;
                    }
                    requestsInfo[i].cssContainerClass = cssStatusClass;
                    requestsInfo[i].cssClass = cssClass;

                    //$('#scrollview').height($('#panelsContent').height());
                }
                itemCount(requestsInfo.length);
                dataSource(new DevExpress.data.DataSource({
                    store: requestsInfo,
                    searchOperation: "contains",
                    searchExpr: "Name"
                }));

                setTimeout(function () {
                    if (dataFiltered()) {
                        $('#panelsContent').height($('#panelsContent').height() - 80);
                    } else {
                        $('#scrollview').height($('#panelsContent').height() - 45);
                    }
                }, 100);
            } else {
                itemCount(0);
                dataSource(new DevExpress.data.DataSource({
                    store: [],
                    searchOperation: "contains",
                    searchExpr: "Name"
                }));

                var onContinue = function () {
                    VTPortal.roApp.loadInitialData(false, false, true, false, false);
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        };

        if (uFilter == null) {
            dataFiltered(false);
            var requests = VTPortal.roApp.requestsDS();
            if (requests != null && typeof requests.Requests != 'undefined') {
                VTPortal.roApp.requestsDS(null);
                callbackFuntion(requests);
            } else {
                new WebServiceRobotics(function (result) { callbackFuntion(result); }).getSupervisedRequests(false, moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').startOf('year').add(1, 'year'), '0*1|1*2*3*4*5*6*7*8*9*10*11*12*13*14*15*16', 'RequestDate DESC', moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').add(1, 'year'));
            }
        } else {
            dataFiltered(true);
            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getSupervisedRequests(false, uFilter.filter.iDate, uFilter.filter.eDate, uFilter.filter.status.join('*') + '|' + uFilter.filter.types.join('*'), uFilter.order.by + ' ' + uFilter.order.direction, uFilter.filter.iRequestedDate, uFilter.filter.eRequestedDate);
        }
    }

    var onBtnFilterRequests = function (e) {
        popoverVisible(false);
        VTPortal.app.navigate("RequestFilters/4");
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roMyTeamRequests'),
        subscribeBlock: globalStatus(),
        requestsDS: itemCount,
        dataFiltered: dataFiltered,
        filterDescription: filterDescription,
        listRequests: {
            dataSource: dataSource,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'RequestItem',
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.NewRequests) {
                        const notImpersonateRequests = [2, 5, 6, 7, 9, 10, 11, 12, 13, 16];
                        if (!notImpersonateRequests.includes(info.itemData.IdRequestType)) {
                            VTPortal.roApp.impersonateOnlyRequest = true;
                            VTPortal.roApp.impersonatedIDEmployee = info.itemData.IdEmployee;
                            VTPortal.roApp.impersonatedNameEmployee = i18nextko.i18n.t('roProfileOf') + info.itemData.Name;
                            VTPortal.roApp.loadInitialData(true, false, false, false, false);
                        }

                        switch (info.itemData.IdRequestType) {
                            case 1:
                                VTPortal.app.navigate('userFields/' + info.itemData.Id);
                                break;
                            case 2:
                            case 10:
                            case 12:
                                VTPortal.app.navigate('forgotten/' + info.itemData.Id);
                                break;
                            case 3:
                                VTPortal.app.navigate('justifyPunch/' + info.itemData.Id);
                                break;
                            case 4:
                                VTPortal.app.navigate('externalWork/' + info.itemData.Id);
                                break;
                            case 5:
                                VTPortal.app.navigate('changeShiftNew/' + info.itemData.Id);
                                break;
                            case 6:
                            case 7:
                            case 9:
                            case 13:
                                VTPortal.app.navigate('absence/' + info.itemData.Id);
                                break;
                            case 8:
                                VTPortal.app.navigate('shiftExchange/' + info.itemData.Id);
                                break;
                            case 11:
                                VTPortal.app.navigate('cancelHolidaysNew/' + info.itemData.Id);
                                break;
                            case 14:
                                VTPortal.app.navigate('plannedOvertime/' + info.itemData.Id);
                                break;
                            case 15:
                                VTPortal.app.navigate('externalWorkWeekResume/' + info.itemData.Id);
                                break;
                            case 16:
                                VTPortal.app.navigate('telecommuting/' + info.itemData.Id);
                                break;
                        }
                    }
                    else {
                        VTPortal.roApp.impersonateOnlyRequest = true;
                        VTPortal.roApp.impersonatedIDEmployee = info.itemData.IdEmployee;
                        VTPortal.roApp.impersonatedNameEmployee = i18nextko.i18n.t('roProfileOf') + info.itemData.Name;
                        VTPortal.roApp.loadInitialData(true, false, false, false, false);

                        switch (info.itemData.IdRequestType) {
                            case 1:
                                VTPortal.app.navigate('userFields/' + info.itemData.Id);
                                break;
                            case 2:
                                VTPortal.app.navigate('forgottenPunch/' + info.itemData.Id);
                                break;
                            case 3:
                                VTPortal.app.navigate('justifyPunch/' + info.itemData.Id);
                                break;
                            case 4:
                                VTPortal.app.navigate('externalWork/' + info.itemData.Id);
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
                            case 9:
                                VTPortal.app.navigate('plannedCause/' + info.itemData.Id);
                                break;
                            case 10:
                                VTPortal.app.navigate('forgottenPunch/' + info.itemData.Id);
                                break;
                            case 11:
                                VTPortal.app.navigate('cancelHolidays/' + info.itemData.Id);
                                break;
                            case 12:
                                VTPortal.app.navigate('forgottenPunch/' + info.itemData.Id);
                                break;
                            case 13:
                                VTPortal.app.navigate('plannedHoliday/' + info.itemData.Id);
                                break;
                            case 14:
                                VTPortal.app.navigate('plannedOvertime/' + info.itemData.Id);
                                break;
                            case 15:
                                VTPortal.app.navigate('externalWorkWeekResume/' + info.itemData.Id);
                                break;
                        }
                    }
                }
            }
        },
        btnFilterRequests: {
            onClick: onBtnFilterRequests,
            text: '',
            icon: "Images/Common/filter.png",
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
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
    };

    return viewModel;
};