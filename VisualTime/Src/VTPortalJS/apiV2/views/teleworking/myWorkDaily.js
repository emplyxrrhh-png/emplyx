VTPortal.myWorkDaily = function (params) {
    var myEventHandler = function (model, e) {
        var res = 'middle';
        var offset = e.targetOffset;
        if (offset < 0) swipeRight();
        else if (offset > 0) swipeLeft();
    }
    var noPermissions = VTPortal.noPermissions();
    var selectedDate = ko.observable(new Date());
    var modelIsReady = ko.observable(false);
    var requestsDS = ko.observable([]);

    var swipeLeft = function () {
        selectedDate(moment(selectedDate()).subtract(1, 'days'));
    }

    var swipeRight = function () {
        selectedDate(moment(selectedDate()).add(1, 'days'));
    }

    function viewShown(selDate) {
        selectedDate(selDate);
        modelIsReady(true);
    };

    var onNewRequest = function () {
        VTPortal.app.navigate('requestsList/4,15/' + moment(selectedDate()).format("YYYY-MM-DD"));
    }

    var hasPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Punch.ScheduleQuery)) {
            return true;
        } else {
            return false;
        }
    });

    var refreshData = function (e) {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Punch.ScheduleQuery)) {
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
                            case 4:
                                cssClass = 'dx-icon-externalWorkResume';
                                break;
                            case 15:
                                cssClass = 'dx-icon-externalWorkWeekResume';
                                break;
                        }
                        requestsInfo[i].cssContainerClass = cssStatusClass;
                        requestsInfo[i].cssClass = cssClass;
                    }
                    requestsDS(requestsInfo);

                    $('#scrollview').height($('#panelsContent').height() - 70);
                } else {
                    requestsDS([]);
                    var onContinue = function () {
                        VTPortal.roApp.loadInitialData(false, false, true, false, false);
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
            };

            new WebServiceRobotics(function (result) {
                callbackFuntion(result);
            }).getMyDailyTeleWorking(moment(e.value).startOf('day'));
        }
    }

    var viewModel = {
        modelIsReady: modelIsReady,
        initializeView: viewShown,
        requestsDS: requestsDS,
        hasPermission: hasPermission,
        noPermissions: noPermissions,
        headerDate: {
            value: selectedDate,
            pickerType: 'rollers',
            displayFormat: 'shortDate',
            onValueChanged: refreshData
        },
        btnPrevious: {
            icon: 'chevronprev',
            onClick: swipeLeft
        },
        btnNext: {
            icon: 'chevronnext',
            onClick: swipeRight
        },
        myEventHandler: myEventHandler,
        listOptions: {
            dataSource: requestsDS,
            grouped: false,
            scrollingEnabled: false,
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    switch (info.itemData.IdRequestType) {
                        case 4:
                            VTPortal.app.navigate('externalWork/' + info.itemData.Id);
                            break;
                        case 15:
                            VTPortal.app.navigate('externalWorkWeekResume/' + info.itemData.Id);
                            break;
                    }
                }
            }
        },
        newRequest: {
            onClick: onNewRequest,
            text: '',
            icon: "Images/Common/plus.png",
            visible: VTPortal.roApp.ReadMode() == false
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};