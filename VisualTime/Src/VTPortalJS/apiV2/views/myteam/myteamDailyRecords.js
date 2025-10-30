VTPortal.myTeamDailyRecords = function (params) {
    var itemCount = ko.observable(0);
    var dataSource = ko.observable(new DevExpress.data.DataSource({
        store: [],
        searchOperation: "contains",
        searchExpr: ["Name", "GroupName", "Adjusted", "Description", "Status", "AdjustedDetails"]
    }));

    var popoverVisible = ko.observable(false);
    var dataFiltered = ko.observable(false);
    var selectedWorkingStatements = ko.observable([]);
    var loadingVisible = ko.observable(false);
    var repainted = ko.observable(false);
    var filterDescription = ko.computed(function () {
        if (VTPortal.roApp.supervisorFiltersDR() != null) {
            return i18nextko.i18n.t('roFilteringData');
        } else {
            return '';
        }
    })

    var requestsSelected = ko.computed(function () {
        if (selectedWorkingStatements().length > 0) {
            return true;
        } else {
            return false;
        }
    })

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function convertMinsToHrsMins(minutes) {
        var h = Math.floor(minutes / 60);
        var m = minutes % 60;
        h = h < 10 ? '0' + h : h;
        m = m < 10 ? '0' + m : m;
        return h + ':' + m;
    }

    function viewShown() {
        globalStatus().viewShown();
        refreshData();
    };

    function refreshData() {
        dataFiltered(false);
        loadingVisible(true);
        var uFilter = VTPortal.roApp.supervisorFiltersDR();

        //getRequestsDR
        var callbackFuntion = function (result) {
            loadingVisible(false);
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var requestsInfo = result.Requests;
                for (var i = 0; i < requestsInfo.length; i++) {
                    requestsInfo[i].Name = requestsInfo[i].Name;
                    requestsInfo[i].GroupName = requestsInfo[i].DailyRecord.EmployeeGroup;
                    requestsInfo[i].hasAction = true;

                    if (requestsInfo[i].DailyRecord.EmployeeImage == "") requestsInfo[i].DailyRecord.EmployeeImage = result.DefaultImage;
                    requestsInfo[i].EmployeeImage = requestsInfo[i].DailyRecord.EmployeeImage;

                    var fecha = moment(requestsInfo[i].DailyRecord.RecordDate).format("DD-MM-YYYY");
                    requestsInfo[i].Description = i18nextko.i18n.t('roRequestDRInfo') + '<b>' + fecha + '</b>' + i18nextko.i18n.t('roRequestDRInfo2') + requestsInfo[i].DailyRecord.Punches.length + i18nextko.i18n.t('roRequestDRInfo3');
                    if (requestsInfo[i].DailyRecord.Adjusted) {
                        var texto = i18nextko.i18n.t('roAdjusted');
                        requestsInfo[i].Adjusted = texto;
                        requestsInfo[i].cssClassAdjusted = 'roAdjusted'
                    }
                    else if (requestsInfo[i].DailyRecord.TimeAccrued == 0) {
                        var texto = i18nextko.i18n.t('roNoAccrued');
                        requestsInfo[i].Adjusted = texto;
                        requestsInfo[i].cssClassAdjusted = 'roNoAccrued'
                    }
                    else {
                        var texto = i18nextko.i18n.t('roNoAdjusted');
                        requestsInfo[i].Adjusted = texto;
                        requestsInfo[i].cssClassAdjusted = 'roNoAdjusted'

                        var difference = requestsInfo[i].DailyRecord.TimeExpected - requestsInfo[i].DailyRecord.TimeAccrued;

                        if (difference <= 0) {
                            if (Math.abs(difference) < 120) {
                                requestsInfo[i].AdjustedDetails = i18nextko.i18n.t("roAdjustedExceeded") + " " + convertMinsToHrsMins(Math.abs(difference)) + " " + i18nextko.i18n.t("roExceededHoursDROne");
                            }
                            else {
                                requestsInfo[i].AdjustedDetails = i18nextko.i18n.t("roAdjustedExceeded") + " " + convertMinsToHrsMins(Math.abs(difference)) + " " + i18nextko.i18n.t("roExceededHoursDR");
                            }
                        }
                        else {
                            if (Math.abs(difference) < 120) {
                                requestsInfo[i].AdjustedDetails = i18nextko.i18n.t("roAdjustedMissingOne") + " " + convertMinsToHrsMins(Math.abs(difference)) + " " + i18nextko.i18n.t("roExceededHoursDROne");
                            }
                            else {
                                requestsInfo[i].AdjustedDetails = i18nextko.i18n.t("roAdjustedMissing") + " " + convertMinsToHrsMins(Math.abs(difference)) + " " + i18nextko.i18n.t("roExceededHoursDR");
                            }
                        }
                    }
                    requestsInfo[i].IdStatus = requestsInfo[i].Status;
                    switch (requestsInfo[i].Status) {
                        case 0:
                            var texto = i18nextko.i18n.t('roRequestStatus_pending')
                            requestsInfo[i].Status = texto;
                            break;
                        case 1:
                            var texto = i18nextko.i18n.t('roRequestStatus_running')
                            requestsInfo[i].Status = texto;

                            break;
                        case 2:
                            var texto = i18nextko.i18n.t('roRequestStatus_approved')
                            requestsInfo[i].Status = texto;
                            break;
                        case 3:
                            var texto = i18nextko.i18n.t('roRequestStatus_denied')
                            requestsInfo[i].Status = texto;
                            break;
                        case 4:
                            var texto = i18nextko.i18n.t('roRequestStatus_canceled')
                            requestsInfo[i].Status = texto;
                            break;
                    }
                    var cssClass = '';

                    requestsInfo[i].cssClass = cssClass;

                    //$('#scrollview').height($('#panelsContent').height());
                }
                itemCount(requestsInfo.length);
                dataSource(new DevExpress.data.DataSource({
                    store: requestsInfo,
                    searchOperation: "contains",
                    searchExpr: ["Name", "GroupName", "Adjusted", "Description", "Status", "AdjustedDetails"]
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
                    searchExpr: ["Name", "GroupName", "Adjusted", "Description", "AdjustedDetails"]
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
            var dailyRecords = VTPortal.roApp.dailyRecordsDS();
            if (dailyRecords != null && typeof dailyRecords.Requests != 'undefined') {
                VTPortal.roApp.dailyRecordsDS(null);
                callbackFuntion(dailyRecords);
            } else {
                new WebServiceRobotics(function (result) { callbackFuntion(result); }).getSupervisedRequests(false, moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').startOf('year').add(1, 'year'), '0*1|17', 'RequestDate DESC', moment().startOf('day').startOf('year').add(-6, 'month'), moment().startOf('day').add(1, 'year'));
            }
        } else {
            dataFiltered(true);
            new WebServiceRobotics(function (result) { callbackFuntion(result); }).getSupervisedRequests(false, uFilter.filter.iDate, uFilter.filter.eDate, uFilter.filter.status.join('*') + '|' + uFilter.filter.types.join('*'), uFilter.order.by + ' ' + uFilter.order.direction, uFilter.filter.iRequestedDate, uFilter.filter.eRequestedDate);
        }
    }

    var approveRefuse = function (action) {
        idRequests = JSON.stringify(selectedWorkingStatements());

        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("GENERAL_ERROR"), 'error', 0);
        }

        var onWSResult = function (actionType) {
            return function (result) {
                selectedWorkingStatements([]);
                refreshData();
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    if (actionType == 'approve') {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestsApproved"), 'success', 5000);
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestsDenied"), 'success', 5000);
                    }
                } else {
                    if (actionType == 'approve') {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestsNoApproved"), 'error', 5000);
                    } else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestsNoDenied"), 'error', 5000);
                    }
                }
            }
        };
        wsRobotics = new WebServiceRobotics(onWSResult(action), onWSError);

        wsRobotics.approveRefuseMultipleDR(idRequests, action, false);
    }

    var onBtnFilterRequests = function (e) {
        popoverVisible(false);
        VTPortal.app.navigate("RequestFilters/5");
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roMyTeamRequests'),
        subscribeBlock: globalStatus(),
        dailyRecordsDS: itemCount,
        dataFiltered: dataFiltered,
        filterDescription: filterDescription,
        listRequests: {
            dataSource: dataSource,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'RequestItem',
            showSelectionControls: true,
            onSelectionChanged: function (info) {
                // selectedWorkingStatements(info.component.option('selectedItemKeys'));

                var items = [];
                info.component.option('selectedItemKeys').forEach((item) => {
                    if (item.IdStatus == 0 || item.IdStatus == 1) {
                        items.push(item.Id);
                    }
                });
                selectedWorkingStatements(items);
            },
            //onContentReady: function (info) {
            //    if (info.component.option("items").length > 0) {
            //        info.component.option("items")[0].disabled = true

            //        if (!repainted()) {
            //            repainted(true);
            //            $("#listRequests").dxList("instance").repaint();

            //        }
            //    }
            //},
            selectionMode: 'all',
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    VTPortal.app.navigate("dailyRecordDetail/" + info.itemData.Id + "/" + moment(info.itemData.DailyRecord.RecordDate).format("YYYY-MM-DD") + "/" + false + "/" + "FR");
                }
            },
            selectionByClick: false,
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
        btnApprove: {
            onClick: function () {
                approveRefuse('approve');
            },
            text: i18nextko.i18n.t('roApprove'),
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: requestsSelected
        },
        btnRefuse: {
            onClick: function () {
                approveRefuse('refuse');
            },
            text: i18nextko.i18n.t('roRefuse'),
            hint: i18nextko.i18n.t('roRefuse'),
            icon: "Images/Common/refuse.png",
            visible: requestsSelected
        },
    };

    return viewModel;
};