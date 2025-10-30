VTPortal.RequestFilters = function (params) {
    var iDate = ko.observable(moment().startOf('year').toDate())
    var eDate = ko.observable(moment().endOf('year').toDate())
    var iRequestedDate = ko.observable(moment().startOf('year').toDate())
    var eRequestedDate = ko.observable(moment().endOf('year').toDate())
    var selectedIndex = ko.observable(0);
    var selectedTypesVal = ko.observable([1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16]);
    var selectedStatusVal = ko.observable([0, 1, 2, 3, 4]);
    var selectedOrderDirection = ko.observable('DESC');
    var selectedOrderBy = ko.observable('RequestDate');
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var workingMode = parseInt(params.id, 10);

    if (workingMode == 0) { //Generic requests
        selectedTypesVal([1, 5, 6, 7, 11, 12, 13]);
    } else if (workingMode == 1) { //Teleworking
        selectedTypesVal([4, 15]);
    } else if (workingMode == 2) { //Fichajes
        selectedTypesVal([2, 3, 10, 12]);
    } else if (workingMode == 3) { //Autorizaciones
        selectedTypesVal([9, 14]);
    } else if (workingMode == 4) { //Autorizaciones
        selectedTypesVal([1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16]);
    }

    var availableTabPanels = [];
    availableTabPanels.push({ "ID": 1, "cssClass": "dx-icon-filterParams" });
    availableTabPanels.push({ "ID": 2, "cssClass": "dx-icon-orderParams" });

    var statusDS = [{
        "ID": 0,
        "Name": i18nextko.t('roRequestPendingStatus'),
        "ImageSrc": "Images/Common/request_pending.png"
    }, {
        "ID": 1,
        "Name": i18nextko.t('roRequestOnGoingStatus'),
        "ImageSrc": "Images/Common/request_ongoing.png"
    }, {
        "ID": 2,
        "Name": i18nextko.t('roRequestApprovedStatus'),
        "ImageSrc": "Images/Common/request_approve.png"
    }, {
        "ID": 3,
        "Name": i18nextko.t('roRequestDeniedStatus'),
        "ImageSrc": "Images/Common/request_denied.png"
    }, {
        "ID": 4,
        "Name": i18nextko.t('roRequestCanceledStatus'),
        "ImageSrc": "Images/Common/request_denied.png"
    }];

    if (workingMode == 4) { //Autorizaciones
        statusDS = [{
            "ID": 0,
            "Name": i18nextko.t('roRequestPendingStatus'),
            "ImageSrc": "Images/Common/request_pending.png"
        }, {
            "ID": 1,
            "Name": i18nextko.t('roRequestOnGoingStatus'),
            "ImageSrc": "Images/Common/request_ongoing.png"
        }]
    }

    var typesDS = [];

    var perm = VTPortal.roApp.empPermissions().Requests;

    if (workingMode != 4) {
        for (var i = 0; i < perm.length; i++) {
            if (perm[i].Permission) {
                switch (perm[i].RequestType) {
                    case 1:
                        if (workingMode == 0) typesDS.push({ "ID": 1, "Name": i18nextko.i18n.t('roRequestType_UserFieldsChange'), "ImageSrc": "Images/Requests/ico_UserFieldChange.png" });
                        break;
                    case 2:
                        if (workingMode == 2) typesDS.push({ "ID": 2, "Name": i18nextko.i18n.t('roRequestType_ForbiddenPunch'), "ImageSrc": "Images/Requests/ico_ForbiddenPunch.png" });
                        break;
                    case 3:
                        if (workingMode == 2) typesDS.push({ "ID": 3, "Name": i18nextko.i18n.t('roRequestType_JustifyPunch'), "ImageSrc": "Images/Requests/ico_JustifyPunch.png" });
                        break;
                    case 4:
                        if (workingMode == 1) typesDS.push({ "ID": 4, "Name": i18nextko.i18n.t('roRequestType_ExternalWorkResumePart'), "ImageSrc": "Images/Requests/ico_ExternalWorkResume.png" });
                        break;
                    case 5:
                        if (workingMode == 0) typesDS.push({ "ID": 5, "Name": i18nextko.i18n.t('roRequestType_ChangeShift'), "ImageSrc": "Images/Requests/ico_ChangeShift.png" });
                        break;
                    case 6:
                        if (workingMode == 0) typesDS.push({ "ID": 6, "Name": i18nextko.i18n.t('roRequestType_VacationsOrPermissions'), "ImageSrc": "Images/Requests/ico_Holidays.png" });
                        break;
                    case 7:
                        if (workingMode == 0) typesDS.push({ "ID": 7, "Name": i18nextko.i18n.t('roRequestType_PlannedAbsences'), "ImageSrc": "Images/Requests/ico_PlannedAbsences.png" });
                        break;
                    case 8:
                        if (workingMode == 0) typesDS.push({ "ID": 8, "Name": i18nextko.i18n.t('roRequestType_ShiftExchange'), "ImageSrc": "Images/Requests/ico_ShiftExchange.png" });
                        break;
                    case 9:
                        if (workingMode == 3) typesDS.push({ "ID": 9, "Name": i18nextko.i18n.t('roRequestType_PlannedCauses'), "ImageSrc": "Images/Requests/ico_PlannedCauses.png" });
                        break;
                    case 10:
                        if (workingMode == 2) typesDS.push({ "ID": 10, "Name": i18nextko.i18n.t('roRequestType_ForbiddenTaskPunch'), "ImageSrc": "Images/Requests/ico_ForbiddenTaskPunch.png" });
                        break;
                    case 11:
                        if (workingMode == 0) typesDS.push({ "ID": 11, "Name": i18nextko.i18n.t('roRequestType_CancelHolidays'), "ImageSrc": "Images/Requests/ico_HolidaysCancel.png" });
                        break;
                    case 12:
                        if (workingMode == 2) typesDS.push({ "ID": 12, "Name": i18nextko.i18n.t('roRequestType_ForgottenCostCenterPunch'), "ImageSrc": "Images/Requests/ico_ForbiddenCostPunch.png" });
                        break;
                    case 13:
                        if (workingMode == 0) typesDS.push({ "ID": 13, "Name": i18nextko.i18n.t('roRequestType_PlannedHoliday'), "ImageSrc": "Images/Requests/ico_PlannedHoliday.png" });
                        break;
                    case 14:
                        if (workingMode == 3) typesDS.push({ "ID": 14, "Name": i18nextko.i18n.t('roRequestType_PlannedOvertime'), "ImageSrc": "Images/Requests/ico_PlannedOvertime.png" });
                        break;
                    case 15:
                        if (workingMode == 1) typesDS.push({ "ID": 15, "Name": i18nextko.i18n.t('roRequestType_ExternalWorkWeekResume'), "ImageSrc": "Images/Requests/ico_ExternalWorkWeekResume.png" });
                        break;
                    case 16:
                        if (workingMode == 1) typesDS.push({ "ID": 16, "Name": i18nextko.i18n.t('roRequestType_Telecommute'), "ImageSrc": "Images/Requests/ico_telecommute.png" });
                        break;
                }
            }
        }
    } else {
        typesDS.push({ "ID": 1, "Name": i18nextko.i18n.t('roRequestType_UserFieldsChange'), "ImageSrc": "Images/Requests/ico_UserFieldChange.png" });
        typesDS.push({ "ID": 2, "Name": i18nextko.i18n.t('roRequestType_ForbiddenPunch'), "ImageSrc": "Images/Requests/ico_ForbiddenPunch.png" });
        typesDS.push({ "ID": 3, "Name": i18nextko.i18n.t('roRequestType_JustifyPunch'), "ImageSrc": "Images/Requests/ico_JustifyPunch.png" });
        typesDS.push({ "ID": 4, "Name": i18nextko.i18n.t('roRequestType_ExternalWorkResumePart'), "ImageSrc": "Images/Requests/ico_ExternalWorkResume.png" });
        typesDS.push({ "ID": 5, "Name": i18nextko.i18n.t('roRequestType_ChangeShift'), "ImageSrc": "Images/Requests/ico_ChangeShift.png" });
        typesDS.push({ "ID": 6, "Name": i18nextko.i18n.t('roRequestType_VacationsOrPermissions'), "ImageSrc": "Images/Requests/ico_Holidays.png" });
        typesDS.push({ "ID": 7, "Name": i18nextko.i18n.t('roRequestType_PlannedAbsences'), "ImageSrc": "Images/Requests/ico_PlannedAbsences.png" });
        typesDS.push({ "ID": 8, "Name": i18nextko.i18n.t('roRequestType_ShiftExchange'), "ImageSrc": "Images/Requests/ico_ShiftExchange.png" });
        typesDS.push({ "ID": 9, "Name": i18nextko.i18n.t('roRequestType_PlannedCauses'), "ImageSrc": "Images/Requests/ico_PlannedCauses.png" });
        typesDS.push({ "ID": 10, "Name": i18nextko.i18n.t('roRequestType_ForbiddenTaskPunch'), "ImageSrc": "Images/Requests/ico_ForbiddenTaskPunch.png" });
        typesDS.push({ "ID": 11, "Name": i18nextko.i18n.t('roRequestType_CancelHolidays'), "ImageSrc": "Images/Requests/ico_HolidaysCancel.png" });
        typesDS.push({ "ID": 12, "Name": i18nextko.i18n.t('roRequestType_ForgottenCostCenterPunch'), "ImageSrc": "Images/Requests/ico_ForbiddenCostPunch.png" });
        typesDS.push({ "ID": 13, "Name": i18nextko.i18n.t('roRequestType_PlannedHoliday'), "ImageSrc": "Images/Requests/ico_PlannedHoliday.png" });
        typesDS.push({ "ID": 14, "Name": i18nextko.i18n.t('roRequestType_PlannedOvertime'), "ImageSrc": "Images/Requests/ico_PlannedOvertime.png" });
        typesDS.push({ "ID": 15, "Name": i18nextko.i18n.t('roRequestType_ExternalWorkWeekResume'), "ImageSrc": "Images/Requests/ico_ExternalWorkWeekResume.png" });
        typesDS.push({ "ID": 16, "Name": i18nextko.i18n.t('roRequestType_Telecommute'), "ImageSrc": "Images/Requests/ico_telecommute.png" });
    }

    typesDS = typesDS.sortBy(function (n) { return n.Name; });

    var orderDirectionDS = [{
        id: 'ASC',
        name: i18nextko.t('roOrderDirectionASC')
    }, {
        id: 'DESC',
        name: i18nextko.t('roOrderDirectionDESC')
    }];

    var orderByDS = [{
        id: 'RequestType',
        name: i18nextko.t('roOrderBy_field1')
    }, {
        id: 'RequestDate',
        name: i18nextko.t('roOrderBy_field2')
    }, {
        id: 'Status',
        name: i18nextko.t('roOrderBy_field3')
    }, {
        id: 'Date1',
        name: i18nextko.t('roOrderBy_field4')
    }];

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        if (workingMode == 0) {
            if (VTPortal.roApp.requestFilters() != null) {
                var uFilter = VTPortal.roApp.requestFilters();

                selectedOrderDirection(uFilter.order.direction);
                selectedOrderBy(uFilter.order.by);
                iDate(uFilter.filter.iDate);
                eDate(uFilter.filter.eDate);
                iRequestedDate(uFilter.filter.iRequestedDate);
                eRequestedDate(uFilter.filter.eRequestedDate);
                selectedTypesVal(uFilter.filter.types);
                selectedStatusVal(uFilter.filter.status);
            } else {
                selectedTypesVal([1, 5, 6, 7, 11, 13]);
                selectedStatusVal([0, 1, 2, 3, 4]);
                selectedOrderDirection('DESC');
                selectedOrderBy('RequestDate');
                iDate(moment().startOf('year').toDate())
                eDate(moment().endOf('year').toDate())
                iRequestedDate(moment().startOf('year').toDate())
                eRequestedDate(moment().endOf('year').toDate())
            }
        } else if (workingMode == 1) {
            if (VTPortal.roApp.teleWorkingFilters() != null) {
                var uFilter = VTPortal.roApp.teleWorkingFilters();

                selectedOrderDirection(uFilter.order.direction);
                selectedOrderBy(uFilter.order.by);
                iDate(uFilter.filter.iDate);
                eDate(uFilter.filter.eDate);
                iRequestedDate(uFilter.filter.iRequestedDate);
                eRequestedDate(uFilter.filter.eRequestedDate);
                selectedTypesVal(uFilter.filter.types);
                selectedStatusVal(uFilter.filter.status);
            } else {
                selectedTypesVal([4, 16]);
                selectedStatusVal([0, 1, 2, 3, 4]);
                selectedOrderDirection('DESC');
                selectedOrderBy('RequestDate');
                iDate(moment().startOf('year').toDate())
                eDate(moment().endOf('year').toDate())
                iRequestedDate(moment().startOf('year').toDate())
                eRequestedDate(moment().endOf('year').toDate())
            }
        } else if (workingMode == 2) {
            if (VTPortal.roApp.punchesFilters() != null) {
                var uFilter = VTPortal.roApp.punchesFilters();

                selectedOrderDirection(uFilter.order.direction);
                selectedOrderBy(uFilter.order.by);
                iDate(uFilter.filter.iDate);
                eDate(uFilter.filter.eDate);
                iRequestedDate(uFilter.filter.iRequestedDate);
                eRequestedDate(uFilter.filter.eRequestedDate);
                selectedTypesVal(uFilter.filter.types);
                selectedStatusVal(uFilter.filter.status);
            } else {
                selectedTypesVal([2, 3, 10, 12]);
                selectedStatusVal([0, 1, 2, 3, 4]);
                selectedOrderDirection('DESC');
                selectedOrderBy('RequestDate');
                iDate(moment().startOf('year').toDate())
                eDate(moment().endOf('year').toDate())
                iRequestedDate(moment().startOf('year').toDate())
                eRequestedDate(moment().endOf('year').toDate())
            }
        } else if (workingMode == 3) {
            if (VTPortal.roApp.authorizationFilters() != null) {
                var uFilter = VTPortal.roApp.authorizationFilters();

                selectedOrderDirection(uFilter.order.direction);
                selectedOrderBy(uFilter.order.by);
                iDate(uFilter.filter.iDate);
                eDate(uFilter.filter.eDate);
                iRequestedDate(uFilter.filter.iRequestedDate);
                eRequestedDate(uFilter.filter.eRequestedDate);
                selectedTypesVal(uFilter.filter.types);
                selectedStatusVal(uFilter.filter.status);
            } else {
                selectedTypesVal([9, 14]);
                selectedStatusVal([0, 1, 2, 3, 4]);
                selectedOrderDirection('DESC');
                selectedOrderBy('RequestDate');
                iDate(moment().startOf('year').toDate())
                eDate(moment().endOf('year').toDate())
                iRequestedDate(moment().startOf('year').toDate())
                eRequestedDate(moment().endOf('year').toDate())
            }
        } else if (workingMode == 4) {
            if (VTPortal.roApp.supervisorFilters() != null) {
                var uFilter = VTPortal.roApp.supervisorFilters();

                selectedOrderDirection(uFilter.order.direction);
                selectedOrderBy(uFilter.order.by);
                iDate(uFilter.filter.iDate);
                eDate(uFilter.filter.eDate);
                iRequestedDate(uFilter.filter.iRequestedDate);
                eRequestedDate(uFilter.filter.eRequestedDate);
                selectedTypesVal(uFilter.filter.types);
                selectedStatusVal(uFilter.filter.status);
            } else {
                selectedTypesVal([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);
                selectedStatusVal([0, 1]);
                selectedOrderDirection('DESC');
                selectedOrderBy('RequestDate');
                iDate(moment().startOf('year').toDate())
                eDate(moment().endOf('year').toDate())
                iRequestedDate(moment().startOf('year').toDate())
                eRequestedDate(moment().endOf('year').toDate())
            }
        }
    }
    function goPage() {
        var objFilter = {
            order: {
                direction: selectedOrderDirection(),
                by: selectedOrderBy()
            },
            filter: {
                iDate: iDate(),
                eDate: eDate(),
                iRequestedDate: iRequestedDate(),
                eRequestedDate: eRequestedDate(),
                types: selectedTypesVal(),
                status: selectedStatusVal()
            }
        }

        if (workingMode == 0) { //Generic requests
            VTPortal.roApp.requestFilters(objFilter);
            VTPortal.roApp.db.settings.requestFilters = JSON.stringify(objFilter).encodeBase64();
            VTPortal.roApp.db.settings.saveParameter('requestFilters');
        } else if (workingMode == 1) { //Teleworking
            VTPortal.roApp.teleWorkingFilters(objFilter);
            VTPortal.roApp.db.settings.teleWorkingFilters = JSON.stringify(objFilter).encodeBase64();
            VTPortal.roApp.db.settings.saveParameter('teleWorkingFilters');
        } else if (workingMode == 2) { //Fichajes
            VTPortal.roApp.punchesFilters(objFilter);
            VTPortal.roApp.db.settings.punchesFilters = JSON.stringify(objFilter).encodeBase64();
            VTPortal.roApp.db.settings.saveParameter('punchesFilters');
        } else if (workingMode == 3) { //Autorizaciones
            VTPortal.roApp.authorizationFilters(objFilter);
            VTPortal.roApp.db.settings.authorizationFilters = JSON.stringify(objFilter).encodeBase64();
            VTPortal.roApp.db.settings.saveParameter('authorizationFilters');
        } else if (workingMode == 4) { //supervisor
            VTPortal.roApp.supervisorFilters(objFilter);
            VTPortal.roApp.db.settings.supervisorFilters = JSON.stringify(objFilter).encodeBase64();
            VTPortal.roApp.db.settings.saveParameter('supervisorFilters');
        }

        if (workingMode == 0) { //Generic requests
            VTPortal.app.navigate("scheduler/4/" + moment().format("YYYY-MM-DD"), { target: 'back' });
        } else if (workingMode == 1) { //Teleworking
            VTPortal.app.navigate("teleworking/3/" + moment().format("YYYY-MM-DD"), { target: 'back' });
        } else if (workingMode == 2) { //Fichajes
            VTPortal.app.navigate("punchManagement/2/" + moment().format("YYYY-MM-DD"), { target: 'back' });
        } else if (workingMode == 3) { //Autorizaciones
            VTPortal.app.navigate("scheduler/3/" + moment().format("YYYY-MM-DD"), { target: 'back' });
        } else if (workingMode == 4) { //supervisor
            VTPortal.app.navigate("myTeamRequests", { target: 'back' });
        }
    }

    function removeFilter() {
        if (workingMode == 0) { //Generic requests
            VTPortal.roApp.requestFilters(null);
            VTPortal.roApp.db.settings.requestFilters = '';
            VTPortal.roApp.db.settings.saveParameter('requestFilters');
        } else if (workingMode == 1) { //Teleworking
            VTPortal.roApp.teleWorkingFilters(null);
            VTPortal.roApp.db.settings.teleWorkingFilters = '';
            VTPortal.roApp.db.settings.saveParameter('teleWorkingFilters');
        } else if (workingMode == 2) { //Fichajes
            VTPortal.roApp.punchesFilters(null);
            VTPortal.roApp.db.settings.punchesFilters = '';
            VTPortal.roApp.db.settings.saveParameter('punchesFilters');
        } else if (workingMode == 3) { //Autorizaciones
            VTPortal.roApp.authorizationFilters(null);
            VTPortal.roApp.db.settings.authorizationFilters = '';
            VTPortal.roApp.db.settings.saveParameter('authorizationFilters');
        } else if (workingMode == 4) { //Autorizaciones
            VTPortal.roApp.supervisorFilters(null);
            VTPortal.roApp.db.settings.supervisorFilters = '';
            VTPortal.roApp.db.settings.saveParameter('supervisorFilters');
        }

        if (workingMode == 0) { //Generic requests
            VTPortal.app.navigate("scheduler/4/" + moment().format("YYYY-MM-DD"), { target: 'back' });
        } else if (workingMode == 1) { //Teleworking
            VTPortal.app.navigate("teleworking/3/" + moment().format("YYYY-MM-DD"), { target: 'back' });
        } else if (workingMode == 2) { //Fichajes
            VTPortal.app.navigate("punchManagement/2/" + moment().format("YYYY-MM-DD"), { target: 'back' });
        } else if (workingMode == 3) { //Autorizaciones
            VTPortal.app.navigate("scheduler/3/" + moment().format("YYYY-MM-DD"), { target: 'back' });
        } else if (workingMode == 4) { //supervisor
            VTPortal.app.navigate("myTeamRequests", { target: 'back' });
        }
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.t('roRequestFilterTitle'),
        subscribeBlock: globalStatus(),
        lblInitialDate: i18nextko.t('roRequestInitialDateLbl'),
        lblEndDate: i18nextko.t('roRequestEndDateLbl'),
        lblInitialRequestedDate: i18nextko.t('roRequestInitialRequestedDateLbl'),
        lblEndRequestedDate: i18nextko.t('roRequestEndRequestedDateLbl'),
        lblStatus: i18nextko.t('roRequestStatusLbl'),
        lblType: i18nextko.t('roRequestTypeLbl'),
        lblOrderBy: i18nextko.t('roRequestOrderBy'),
        lblOrderDirection: i18nextko.t('roRequestOrderDirection'),
        subscribeBlock: globalStatus(),
        workingMode: workingMode,
        beginDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: iDate,
            showClearButton: true,
            valueChangeEvent: 'focusout'
        },
        endDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: eDate,
            showClearButton: true,
            valueChangeEvent: 'focusout'
        },
        statusTagBox: {
            items: statusDS,
            showSelectionControls: true,
            applyValueMode: "instantly",//VTPortal.roApp.isModeApp() ? "useButtons" : "instantly",
            displayExpr: "Name",
            valueExpr: "ID",
            itemTemplate: "statusItem",
            value: selectedStatusVal
        },
        typeTagBox: {
            items: typesDS,
            showSelectionControls: true,
            applyValueMode: "instantly",//VTPortal.roApp.isModeApp() ? "useButtons" : "instantly",
            displayExpr: "Name",
            valueExpr: "ID",
            itemTemplate: "typeItem",
            value: selectedTypesVal
        },
        beginRequestedDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: iRequestedDate,
            showClearButton: true,
            valueChangeEvent: 'focusout'
        },
        endRequestedDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: eRequestedDate,
            showClearButton: true,
            valueChangeEvent: 'focusout'
        },
        btnAccept: {
            onClick: goPage,
            text: i18nextko.t('closeConfig'),
        },
        btnRemoveFilter: {
            onClick: removeFilter,
            text: i18nextko.t('roRemoveFilter'),
        },
        selectedTab: selectedIndex,
        tabPanelOptions: {
            dataSource: availableTabPanels,
            selectedIndex: selectedIndex,
            itemTemplate: "title"
        },
        orderDirection: {
            items: orderDirectionDS,
            displayExpr: "name",
            valueExpr: "id",
            value: selectedOrderDirection
        },
        orderFields: {
            items: orderByDS,
            displayExpr: "name",
            valueExpr: "id",
            value: selectedOrderBy
        }
    };

    return viewModel;
};