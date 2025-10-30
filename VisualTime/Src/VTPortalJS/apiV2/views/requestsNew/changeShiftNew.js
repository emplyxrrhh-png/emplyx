VTPortal.changeShiftNew = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var selectedDate = ko.observable(moment().add(-1, 'days').toDate());
    var requestId = ko.observable(null);
    var request = ko.observable();
    var myTeamPlan = ko.observable([]);
    var myTeamPlanDS = ko.observable([]);
    var viewActions = ko.observable(false);
    var hasMultipleDays = ko.observable(false);
    var lblAvailable = ko.observable(0);
    var IdEmployeeRequest = ko.observable(0);
    var lblPending = ko.observable(0);
    var lblSelectedDayMV = ko.observable("");
    var lblSelectedDayDetails = ko.observable("");
    var lblApproved = ko.observable(0);
    var infoDivVisible = ko.observable(false);
    var myTeamPlanDivVisible = ko.observable(true);
    var popupVisible = ko.observable(false);
    var remarks = ko.observable('');
    var myApprovalsBlock = VTPortal.approvalHistory();
    var viewHistory = ko.observable(false);
    var requestMainDivVisible = ko.observable(false);
    var loadingDivVisible = ko.observable(false);
    var requestDate1 = ko.observable(moment());
    var requestDate2 = ko.observable(moment());
    var oldShift = ko.observable(0);
    var newShift = ko.observable(0);
    var oldShiftName = ko.observable("");
    var newShiftName = ko.observable("");
    var oldShiftColor = ko.observable("");
    var newShiftColor = ko.observable("");

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

    var getMyTeamPlanInfo = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var dias = result.Days;
                if (dias.length > 1) {
                    hasMultipleDays(true);
                }

                var arrShifts = []
                if (result.EmployeePlanning.length > 0) {
                    for (i = 0; i < result.EmployeePlanning.length; i++) {
                        arrShifts.push(result.EmployeePlanning[i].IdShift);
                        for (y = 0; y < dias.length; y++) {
                            if (result.EmployeePlanning[i].DatePlanned == dias[y].DatePlanned) {
                                dias[y].OldEmployeeShift = result.EmployeePlanning[i].IdShift;
                                dias[y].OldEmployeeName = result.EmployeePlanning[i].ShiftName;
                                dias[y].OldEmployeeColor = result.EmployeePlanning[i].ShiftColor;
                            }
                        }
                    }

                    //dias.forEach(function (item, index) {
                    //    item.Employees = item.Employees.filter(item2 => (arrShifts.includes(item2.IdShift) || item2.IdShift == newShift()));
                    //});
                }
                else {
                    //dias.forEach(function (item, index) {
                    //    item.Employees = item.Employees.filter(item2 => (item2.IdShift == oldShift() || item2.IdShift == newShift()));
                    //});
                }

                for (i = 0; i < dias.length; i++) {
                    for (y = 0; y < dias[i].Employees.length; y++) {
                        if (dias[i].Employees[y].EmployeeImage == "") {
                            dias[i].Employees[y].EmployeeImage = result.DefaultImage.replace("no-repeat center center", "");
                        }
                    }
                }

                dataSource(new DevExpress.data.DataSource({
                    store: dias
                }));
            } else {
            }
        }).getMyTeamPlanEmployees(moment(requestDate1()).format("YYYY/MM/DD"), moment(requestDate2()).format("YYYY/MM/DD"), IdEmployeeRequest(), oldShift(), newShift());
    }

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                requestDate1(result.Date1);
                requestDate2(result.Date2);
                oldShift(result.Field4);
                newShift(result.IdShift);

                oldShiftName(result.OldShiftName);
                newShiftName(result.NewShiftName);
                oldShiftColor(result.OldShiftColor);
                newShiftColor(result.NewShiftColor);

                IdEmployeeRequest(result.IdEmployee);

                getMyTeamPlanInfo();

                loadingDivVisible(false);
                requestMainDivVisible(true);

                request(result);

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

    function goPrevious() {
        $("#myTeamPlanDiv").dxMultiView("option", "selectedIndex", $("#myTeamPlanDiv").dxMultiView("option", "selectedIndex") - 1);
    }

    function goNext() {
        $("#myTeamPlanDiv").dxMultiView("option", "selectedIndex", $("#myTeamPlanDiv").dxMultiView("option", "selectedIndex") + 1);
    }

    function refreshData() {
        loadingDivVisible(true);
        loadRequest();
    }

    function viewShown() {
        globalStatus().viewShown();
        refreshData();
    };

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roRequestType_ChangeShift'),
        subscribeBlock: globalStatus(),
        empImage: employeeImage,
        lblEmployee: lblEmployee,
        lblRequestDates: lblRequestDates,
        lblmyTeamPlan: i18nextko.i18n.t('lblMyTeamPlan'),
        lblAvailable: lblAvailable,
        lblPending: lblPending,
        lblApproved: lblApproved,
        lblRemarks: i18nextko.i18n.t('lblRemarks'),
        lblLoading: i18nextko.i18n.t('lblLoading'),
        lblSelectedDayMV: lblSelectedDayMV,
        lblSelectedDayDetails: lblSelectedDayDetails,
        lblGroupName: lblGroupName,
        remarks: remarks,
        myApprovalsBlock: myApprovalsBlock,
        popupVisible: popupVisible,
        infoDivVisible: infoDivVisible,
        requestMainDivVisible: requestMainDivVisible,
        loadingDivVisible: loadingDivVisible,
        myTeamPlanDivVisible: myTeamPlanDivVisible,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        scrollContent: {
        },
        btnNext: {
            onClick: goNext,
            icon: 'spinright',
            visible: hasMultipleDays
        },
        btnPrevious: {
            onClick: goPrevious,
            icon: 'spinleft',
            visible: hasMultipleDays
        },
        myTeamPlan: {
            dataSource: dataSource,
            selectedIndex: 0,
            loop: true,
            animationEnabled: true,
            itemTemplate(itemData, itemIndex, itemElement) {
                var global = $("<div>");
                global.dxTileView({
                    items: itemData.Employees,
                    height: 175,
                    direction: "horizontal",
                    baseItemHeight: 75,
                    baseItemWidth: 75,
                    itemMargin: 10,
                    itemTemplate(itemData, itemIndex, itemElement) {
                        if (itemData.IdShift == oldShift()) {
                            itemElement.append(`<div class="imageTileviewRequest" id="div_${itemData.IdEmployee}_${itemData.IdCounter}"><div class="textTileview">${itemData.EmployeeName
                                }</div><div class="imageTileviewRequest" style="border: 4px solid ${oldShiftColor()}; background: ${itemData.EmployeeImage}"></div></div>`);
                        }
                        else if (itemData.IdShift == newShift()) {
                            itemElement.append(`<div class="imageTileviewRequest" id="div_${itemData.IdEmployee}_${itemData.IdCounter}"><div class="textTileview">${itemData.EmployeeName
                                }</div><div class="imageTileviewRequest" style="border: 4px solid ${newShiftColor()}; background: ${itemData.EmployeeImage}"></div></div>`);
                        }
                        else {
                            itemElement.append(`<div style="width:0; height:0;overflow: hidden"></div>`);
                        }
                    },
                }).appendTo(itemElement);
            },
            onSelectionChanged(e) {
                if (typeof e.addedItems[0].OldEmployeeShift != 'undefined') {
                    oldShift(e.addedItems[0].OldEmployeeShift);
                    oldShiftColor(e.addedItems[0].OldEmployeeColor);
                    oldShiftName(e.addedItems[0].OldEmployeeName);
                }
                var origenCount = e.addedItems[0].Employees.filter(d => d.IdShift == oldShift()).length;
                var destinoCount = e.addedItems[0].Employees.filter(d => d.IdShift == newShift()).length;

                lblSelectedDayMV(moment(e.addedItems[0].DatePlanned).format("DD/MM/YYYY"));
                if (oldShiftName() == "???") {
                    lblSelectedDayDetails(`<span style="color: ${newShiftColor()}">&#9679</span>` + " " + newShiftName() + ": <b>" + destinoCount + "</b> ");
                }
                else {
                    lblSelectedDayDetails(`<span style="color: ${newShiftColor()}">&#9679</span>` + " " + newShiftName() + ": <b>" + destinoCount + "</b> " + `<span style="color: ${oldShiftColor()}">&#9679</span>` + " " + oldShiftName() + ": <b>" + origenCount + "</b>")
                }
                $("#myTeamPlanDiv").dxMultiView("instance").repaint();
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