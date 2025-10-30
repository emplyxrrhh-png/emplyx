VTPortal.shiftExchange = function (params) {
    var requestId = ko.observable(null), remarks = ko.observable('');
    var canSave = ko.observable(false), canDelete = ko.observable(false), viewHistory = ko.observable(false), popupVisible = ko.observable(false), viewActions = ko.observable(false), requestStatus = ko.observable(null);

    var formReadOnly = ko.observable(false);

    var initialShift = ko.observable(null), shiftSourceNameValue = ko.observable('');
    var iDate = ko.observable(moment().startOf('day').toDate());
    var eDate = ko.observable(moment().startOf('day').toDate());

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));
    if (typeof params.param != 'undefined' && parseInt(params.param, 10) != -1) {
        iDate(moment(params.param, 'YYYY-MM-DD').toDate());
        eDate(moment(params.param, 'YYYY-MM-DD').toDate());
    }

    var minAllowed = ko.observable(moment("01/01/" + moment(iDate()).year(), "DD/MM/YYYY"));
    var maxAllowed = ko.observable(moment("31/12/" + moment(iDate()).year(), "DD/MM/YYYY"));

    var reqValue1 = ko.observable(null), reqValue2 = ko.observable(null);
    var requestValue1DS = ko.observable([]), requestValue2DS = ko.observable([]);

    var bNeedToCompensate = ko.observable(false), iNeedToCompensateDays = ko.observable(0), iCompensateDate = ko.observable(null), needShift = ko.observable(true);

    var myApprovalsBlock = VTPortal.approvalHistory();

    var shiftSourceName = ko.computed(function () {
        if (!(requestId() != null && requestId() > 0)) {
            return (initialShift() != null ? initialShift().MainShift.Name : '');
        } else {
            return shiftSourceNameValue();
        }
    })

    var cmbEmployeeChanged = function (e) {
        if (!(requestId() != null && requestId() > 0) && needShift() == false) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    if (result.Days > 0) {
                        iNeedToCompensateDays(result.Days);
                        bNeedToCompensate(true);
                    } else {
                        iNeedToCompensateDays(0);
                        bNeedToCompensate(false);
                    }
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
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinue);
            }).getDaysToCompensate(iDate(), eDate(), e.value);
        }
    }

    var cmbRequestValue1Changed = function (e) {
        if (!(requestId() != null && requestId() > 0)) {
            var selectedIdShift = e.value;
            if (parseInt(selectedIdShift, 10) > 0) loadAvailableEmployees(iDate(), eDate(), initialShift().MainShift.ID, selectedIdShift);

            var item = requestValue1DS().find(function (x) { return parseInt(x.FieldValue, 10) == parseInt(e.value, 10); });
            if (item != null) {
                var selectedWorkingHours = parseFloat(item.RelatedInfo)

                if (selectedWorkingHours == -1) {
                    bNeedToCompensate(false);
                } else {
                    if ((selectedWorkingHours > 0 && initialShift().MainShift.PlannedHours == 0) || (selectedWorkingHours == 0 && initialShift().MainShift.PlannedHours > 0)) {
                        bNeedToCompensate(true);
                    } else {
                        bNeedToCompensate(false);
                    }
                }
            } else {
                bNeedToCompensate(false);
            }
            if (bNeedToCompensate()) iCompensateDate(null);
        }
    }

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                requestStatus(result.ReqStatus);
                if (VTPortal.roApp.impersonatedIDEmployee == -1 && VTPortal.roApp.employeeId == result.IdEmployee) VTPortalUtils.utils.markRequestAsRead(requestId());

                if (VTPortal.roApp.impersonatedIDEmployee != -1) {
                    if (VTPortal.roApp.isImpersonateEnabled() == false) VTPortal.roApp.disableImpersonateActionsOnRequest();
                }

                remarks(result.Comments);

                if (result.ReqStatus == 0 || result.ReqStatus == 1) canDelete(true);
                else canDelete(false);

                viewActions(false);

                if ((VTPortal.roApp.impersonatedIDEmployee == result.IdEmployee && result.ReqStatus == 1) ||
                    (VTPortal.roApp.impersonatedIDEmployee == result.IdEmployeeExchange && result.ReqStatus == 0)) {
                    viewActions(true);
                    canDelete(false);
                } else {
                    if (VTPortal.roApp.employeeId != result.IdEmployee) {
                        if (result.ReqStatus == 0) viewActions(true);
                        canDelete(false);
                    }
                    if (VTPortal.roApp.db.settings.supervisorPortalEnabled == true && VTPortal.roApp.employeeId != result.IdEmployeeExchange) {
                        if (result.ReqStatus == 0) {
                            viewActions(false);
                        }
                        else {
                            viewActions(true);
                        }
                    }
                }

                myApprovalsBlock.refreshApprovals(result.RequestsHistoryEntries);
                iDate(moment(result.strDate1, "YYYY-MM-DD HH:mm:ss").startOf('day').toDate());

                if (result.strDate2 != '') {
                    iCompensateDate(moment(result.strDate2, "YYYY-MM-DD HH:mm:ss").startOf('day').toDate());
                    bNeedToCompensate(true);
                }

                if (VTPortal.roApp.employeeId == result.IdEmployee) {
                    reqValue2("" + result.IdEmployeeExchange);
                    reqValue1("" + result.IdShift);

                    var item = requestValue1DS().find(function (shift) {
                        return (parseInt(shift.FieldValue, 10) == parseInt(result.Field4, 10));
                    })

                    shiftSourceNameValue(item.FieldName);
                } else {
                    reqValue2("" + result.IdEmployee);
                    reqValue1("" + parseInt(result.Field4, 10));

                    var item = requestValue1DS().find(function (shift) {
                        return (parseInt(shift.FieldValue, 10) == parseInt(result.IdShift, 10));
                    })

                    shiftSourceNameValue(item.FieldName);
                    lblSourceEmployee(i18nextko.i18n.t('roEmployeeSource'));
                }
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
        }).getRequest(requestId());
    }

    var loadLists = function (lstdataSource) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var availableShifts = Object.clone(result.SelectFields, true);
                availableShifts = availableShifts.add({ FieldName: '---', FieldValue: -1, RelatedInfo: -1 }, 0);

                requestValue1DS(availableShifts);

                if (requestId() != null && requestId() > 0) {
                    loadListEmp('employees.fullList');
                } else {
                    if (availableShifts.length > 0) {
                        reqValue1(availableShifts[0].FieldValue);
                        if (needShift() && availableShifts[0].FieldValue > 0) loadAvailableEmployees(iDate(), eDate(), initialShift().MainShift.ID, result.SelectFields[0].FieldValue);
                        //else loadAvailableEmployees(iDate(), eDate(), -1, -1);
                    }
                }
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
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinue);
        }).getGenericList(lstdataSource);
    }

    var loadListEmp = function (lstdataSource) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                requestValue2DS(result.SelectFields);

                if (requestId() != null && requestId() > 0) {
                    loadRequest(requestId());
                }
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
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinue);
        }).getGenericList(lstdataSource);
    }

    var loadAvailableEmployees = function (sDate, eDate, iDestinationShift, iSourceShift) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var availableEmployees = result.SelectFields.clone();
                requestValue2DS(availableEmployees);

                if (!(requestId() != null && requestId() > 0)) {
                    if (availableEmployees.length > 0 && reqValue2() == null) reqValue2(availableEmployees[0].FieldValue);
                }
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
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingRequestInfo"), 'error', 0, onContinue);
        }).getAvailableEmployeesForDate(sDate, eDate, iDestinationShift, iSourceShift);
    }

    var getDayInfo = function (sDate) {
        var callbackFuntion = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                if (result.DayInfo.DayData[0].MainShift != null) {
                    if (result.DayInfo.DayData[0].Locked == false) {
                        if (result.DayInfo.DayData[0].EmployeeStatusOnDay != 1) {
                            initialShift(result.DayInfo.DayData[0]);
                            loadLists('shifts.exchangeshifts');
                        }
                        else {
                            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorNoContract"), 'error', 0, onContinue);
                        }
                    }
                    else {
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLockedDay"), 'error', 0, onContinue);
                    }
                }
                else {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorNoDailySchedule"), 'error', 0, onContinue);
                }
            } else {
                var onContinue = function () {
                    VTPortal.roApp.loadInitialData(false, false, true, false, false);
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        };

        new WebServiceRobotics(function (result) { callbackFuntion(result); }).getEmployeeDayInfo(sDate, -1);
    }

    var calendarValueChanged = function () {
        if (!(requestId() != null && requestId() > 0)) {
            //if (moment(iDate()).isSame(moment(eDate()))) {
            //    getDayInfo(iDate());
            //}
            minAllowed(moment("01/01/" + moment(iDate()).year(), "DD/MM/YYYY"));
            maxAllowed(moment("31/12/" + moment(iDate()).year(), "DD/MM/YYYY"));
            eDate(iDate());
            getDayInfo(iDate());
        }
    }

    var calendarEndDateValueChanged = function () {
        if (!(requestId() != null && requestId() > 0)) {
            if (moment(iDate()).isSame(moment(eDate()))) needShift(true);
            else {
                needShift(false);
                //loadAvailableEmployees(iDate(), eDate(), -1, -1);
            }
        }
    }

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        requestValue1DS([]);
        reqValue1(null);
        requestValue2DS([]);
        reqValue2(null);

        if (requestId() != null && requestId() > 0) {
            loadLists('shifts.fulllist.withexcluded');
            formReadOnly(true);
        } else {
            if (moment(iDate()).isSame(moment(eDate()))) getDayInfo(iDate());
        }

        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (requestId() != null) canDelete(true);
        }// else {
        //    if (requestId() != null) viewActions(false);
        //}

        if (requestId() == null) canSave(true);
        if (requestId() != null) viewHistory(true);
    }

    var wsRobotics = null;
    function saveRequest() {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtRequestList(8);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
            } else {
                var onContinue = function () {
                    var strCompensateDate = (bNeedToCompensate() == false ? '' : (iCompensateDate() == null ? VTPortalUtils.nullDate : moment(iCompensateDate()).format("YYYY-MM-DD HH:mm")));

                    wsRobotics.saveShiftExchange(iDate() == null ? VTPortalUtils.nullDate : iDate(), strCompensateDate, reqValue2(), reqValue1(), initialShift().MainShift.ID, remarks(), true);
                }

                VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        var item = requestValue2DS().find(function (elem) {
            return elem.FieldValue == reqValue2();
        });

        if (item != null && item.RelatedInfo == '') {
            if (!bNeedToCompensate() || (bNeedToCompensate() && iCompensateDate() != null)) {
                var strCompensateDate = (bNeedToCompensate() == false ? '' : (iCompensateDate() == null ? VTPortalUtils.nullDate : moment(iCompensateDate()).format("YYYY-MM-DD HH:mm")));
                wsRobotics.saveShiftExchange(iDate() == null ? VTPortalUtils.nullDate : iDate(), strCompensateDate, reqValue2(), reqValue1(), initialShift().MainShift.ID, remarks(), false);
            } else {
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roCompensateDateNeeded"), 'error', 0);
            }
        }
        else VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotAvailableEmployeeSelected"), 'error', 0);
    }

    function deleteRequest() {
        if (requestId() != null && requestId() > 0) VTPortalUtils.utils.deleteRequest(requestId());
    }

    var lblSourceEmployee = ko.observable(i18nextko.i18n.t('roEmployeesAvailable'));

    //    ; (function () {
    //    if (requestId() != null && requestId() > 0) {
    //        return i18nextko.i18n.t('roEmployeeSource');
    //    } else {
    //        return ;
    //    }
    //})

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: i18nextko.t('roRequestType_ShiftExchange'),
        lblInitialDate: i18nextko.t('roRequestExchangeDate'),
        lblEndDate: i18nextko.t('roRequestEndDateLbl'),
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: i18nextko.t('roChangeShiftTo'),
        lblRequestValue2: lblSourceEmployee,
        lblInitialShift: i18nextko.t('roInitialShift'),
        subscribeBlock: globalStatus(),
        scrollContent: {
            height: computedScrollHeight
        },
        popupVisible: popupVisible,
        btnApprove: {
            onClick: function () {
                if (requestStatus() == 1) { VTPortalUtils.utils.approveRefuseRequest(requestId(), 8, true); }
                else { VTPortalUtils.utils.approveRefuseRequestByEmp(requestId(), 8, true); }
            },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () {
                if (requestStatus() == 1) { VTPortalUtils.utils.approveRefuseRequest(requestId(), 8, false); }
                else { VTPortalUtils.utils.approveRefuseRequestByEmp(requestId(), 8, false); }
            },
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
        btnSave: {
            onClick: saveRequest,
            text: '',
            hint: i18nextko.i18n.t('roSaveRequest'),
            icon: "Images/Common/save.png",
            visible: canSave
        },
        btnDelete: {
            onClick: deleteRequest,
            text: '',
            hint: i18nextko.i18n.t('roDeleteRequest'),
            icon: "Images/Common/delete.png",
            visible: canDelete
        },
        beginDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: iDate,
            readOnly: formReadOnly,
            valueChangeEvent: 'focusout',
            onValueChanged: calendarValueChanged
        },
        endDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: eDate,
            readOnly: formReadOnly,
            valueChangeEvent: 'focusout',
            onValueChanged: calendarEndDateValueChanged
        },
        remarks: {
            value: remarks,
            readOnly: formReadOnly
        },
        cmbRequestValue1: {
            dataSource: requestValue1DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue1,
            readOnly: formReadOnly,
            onValueChanged: cmbRequestValue1Changed
        },
        cmbRequestValue2: {
            dataSource: requestValue2DS,
            displayExpr: "FieldName",
            valueExpr: "FieldValue",
            value: reqValue2,
            readOnly: formReadOnly,
            fieldTemplate: function (data, container) {
                var result = "";
                if (data != null) {
                    result = $("<div class='custom-item'><div style='margin-top:5px;' class='" + (data.RelatedInfo == '' ? 'availableEmployee' : 'notAvailableEmployee') + "' /><div class='employee-name " + (data.RelatedInfo == '' ? '' : 'notAvailableDesc') + "'></div></div>");

                    //var txtInfoValue = data.FieldName + (data.RelatedInfo == '' ? '' : (' (Err:' + data.RelatedInfo + ')'))
                    result.find(".employee-name").dxTextBox({
                        value: data.FieldName,
                        readOnly: true
                    });
                } else {
                    result = $("<div class='custom-item'><div style='margin-top:5px;' class='emptyEmployee' /><div class='employee-name'></div></div>");
                    result.find(".employee-name").dxTextBox({
                        value: i18nextko.i18n.t('roNoAvailableEmp'),
                        readOnly: true
                    });
                }
                container.append(result);
            },
            itemTemplate: function (data) {
                return "<div class='custom-item'><div class='" + (data.RelatedInfo == '' ? '' : 'notAvailableDesc') + "'>" + data.FieldName + " " + (data.RelatedInfo == '' ? '' : " (" + i18nextko.i18n.t('roNoAvailable') + ")") + "</div></div>";
            },
            onValueChanged: cmbEmployeeChanged
        },
        txtInitialShift: {
            value: shiftSourceName,
            readOnly: true,
        },
        needToCompensate: bNeedToCompensate,
        needShift: needShift,
        lblCompensateDate: i18nextko.t('roCompensateWithDay'),
        compensateDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: iCompensateDate,
            readOnly: formReadOnly,
            max: maxAllowed,
            min: minAllowed,
            valueChangeEvent: 'focusout'
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf()
    };

    return viewModel;
};