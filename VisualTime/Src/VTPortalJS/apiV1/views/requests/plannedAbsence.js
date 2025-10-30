VTPortal.plannedAbsence = function (params) {
    var requestId = ko.observable(null), absenceId = ko.observable(null), remarks = ko.observable('');
    var canSave = ko.observable(false), canDelete = ko.observable(false), viewHistory = ko.observable(false), viewDocuments = ko.observable(false), popupDocumentsVisible = ko.observable(false), popupVisible = ko.observable(false), viewActions = ko.observable(false);

    var calendarStatus = ko.observable({}), isRequestingData = true, selectedIndex = ko.observable(0);
    var minDate = ko.observable(moment().startOf('day').startOf('year').add(-1, 'year').toDate()), maxDate = ko.observable(moment().startOf('day').endOf('year').add(1, 'year').toDate());
    var availableTabPanels = [{ "ID": 1, "cssClass": "dx-icon-configRequest", "badge": 0 }, { "ID": 2, "cssClass": "dx-icon-selectDate", "badge": 0 }];

    if ($.grep(VTPortal.roApp.empPermissions().Requests, function (e) { return (e.RequestType == 7 && e.Permission == true && typeof e.HasRankings != 'undefined' && e.HasRankings == true); }).length == 1) {
        var item = availableTabPanels.find(function (item) { return item.ID == "3"; });
        if (item == null) {
            availableTabPanels.push({ "ID": 3, "cssClass": "dx-icon-rankings", "badge": "" });
        }
    }

    var noRankingDateSelected = ko.observable(true), hasRankingData = ko.observable(false), hasCoverageData = ko.observable(false);
    var rankingDS = ko.observable([]), txtMinimumCoverageRequiered = ko.observable(0), txtDifferentialCoverage = ko.observable(0);
    var requestStatus = ko.observable(0), idRequestTypeSelected = ko.observable(0), requestReasonSelected = ko.observable(0);
    var selectFileText = ko.observable(i18nextko.i18n.t('roFindFile'));
    var fileAdded = ko.observable(i18nextko.i18n.t('roFileAdded'));
    var documentAttached = ko.observable(false);
    var calendarSelectMode = ko.observable(0);
    var documentsDS = ko.observable([]);
    var documentsBlock = new VTPortal.documentInput();
    var causeHasDocuments = ko.computed(function () {
        return (documentsDS().length > 0);
    });

    var cmbRequestValue1Changed = function (e) {
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.PortalNext) {
            documentsDS([]);

            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    documentsDS(result.SelectFields);
                    documentsBlock.initializeData(result.SelectFields, false, -1);

                    idRequestTypeSelected(7);
                    requestReasonSelected(parseInt(e.value, 10));

                    var item = requestValue1DS().find(function (x) { return parseInt(x.FieldValue, 10) == parseInt(e.value, 10); });
                    if (item != null) {
                        if (parseInt(item.RelatedInfo, 10) > 0) {
                            calendarSelectMode(parseInt(item.RelatedInfo, 10));
                        } else {
                            calendarSelectMode(0);
                        }
                    } else {
                        calendarSelectMode(0);
                    }

                    minSelectedDate(null);
                    maxSelectedDate(null);

                    if (!formReadOnly()) calSelectedDays = {};
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
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingCauseInfo"), 'error', 0, onContinue);
            }).getRequieredCauseDocuments(parseInt(e.value, 10));
        }
        else {
            documentsDS([]);
            idRequestTypeSelected(7);
            requestReasonSelected(parseInt(e.value, 10));

            var item = requestValue1DS().find(function (x) { return parseInt(x.FieldValue, 10) == parseInt(e.value, 10); });
            if (item != null) {
                if (parseInt(item.RelatedInfo, 10) > 0) {
                    calendarSelectMode(parseInt(item.RelatedInfo, 10));
                } else {
                    calendarSelectMode(0);
                }
            } else {
                calendarSelectMode(0);
            }

            minSelectedDate(null);
            maxSelectedDate(null);

            if (!formReadOnly()) calSelectedDays = {};
        }
    }

    var onChangeCalendarSelection = function () {
        var item = availableTabPanels.find(function (item) { return item.ID == 3; });
        if (item == null) {
            item = { "ID": 3, "cssClass": "dx-icon-rankings", "badge": "" };
        }

        if (requestStatus() == 0 || requestStatus() == 1) {
            if (VTPortalUtils.utils.checkIfRequestHasRanquings(idRequestTypeSelected(), requestReasonSelected())) {
                var apiRobotics = null;
                var onWSError = function (error) {
                    VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorRequestingRankings"), 'error', 0);
                }

                var onWSResult = function (result) {
                    if (result.Status == window.VTPortalUtils.constants.OK.value) {
                        setTimeout(function () { $("#configRequestTab").dxTabs('instance').repaint(); }, 150);

                        if (result.Rankings != null) {
                            for (var i = 0; i < result.Rankings.RequestEmployeeRankingPositions.length; i++) {
                                if (result.Rankings.RequestEmployeeRankingPositions[i].IDEmployee == VTPortal.roApp.employeeId) {
                                    result.Rankings.RequestEmployeeRankingPositions[i].cssClass = 'rankingMyPosition'
                                } else {
                                    result.Rankings.RequestEmployeeRankingPositions[i].cssClass = 'listMenuItemPosition';
                                }
                            }

                            rankingDS(result.Rankings.RequestEmployeeRankingPositions);
                        }
                        else rankingDS([]);

                        if (result.RankingEnabled) {
                            item.badge = result.ActualPosition;
                        } else {
                            item.badge = "";
                        }

                        if (result.CoverageEnabled) {
                            txtMinimumCoverageRequiered(result.Coverages.MinCoverage);
                            txtDifferentialCoverage(result.Coverages.DifferentialQuantity);
                            item.badge = item.badge + "/" + result.Coverages.DifferentialQuantity;
                        }

                        availableTabPanels = availableTabPanels.remove(function (item) { return item.ID == 3; })
                        availableTabPanels.push(item);

                        noRankingDateSelected(false);
                        hasRankingData(result.RankingEnabled);
                        hasCoverageData(result.CoverageEnabled);
                    } else {
                        noRankingDateSelected(true);
                        hasRankingData(false);
                        hasCoverageData(false);
                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorRequestingRankings"), 'error', 0);
                    }
                };
                if (apiRobotics == null) apiRobotics = new WebServiceRobotics(onWSResult, onWSError);

                if (minSelectedDate() != null) apiRobotics.getRankingForDay(minSelectedDate(), idRequestTypeSelected(), requestReasonSelected());
                else {
                    noRankingDateSelected(true);
                    hasRankingData(false);
                    hasCoverageData(false);
                }
            } else {
                noRankingDateSelected(true);
                hasRankingData(false);
                hasCoverageData(false);
                item.badge = "";
                availableTabPanels = availableTabPanels.remove(function (item) { return item.ID == 3; })
                availableTabPanels.push(item);

                setTimeout(function () { $("#configRequestTab").dxTabs('instance').repaint(); }, 150);
            }
        } else {
            noRankingDateSelected(true);
            hasRankingData(false);
            hasCoverageData(false);
            item.badge = "";
            availableTabPanels = availableTabPanels.remove(function (item) { return item.ID == 3; })
            availableTabPanels.push(item);

            setTimeout(function () { $("#configRequestTab").dxTabs('instance').repaint(); }, 150);
        }
    }

    var calSelectedDays = {}, iHour = ko.observable(moment().startOf('day').toDate()), eHour = ko.observable(moment().endOf('day').toDate());

    var minSelectedDate = ko.observable(null), maxSelectedDate = ko.observable(null);

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    if (typeof params.iDate != 'undefined' && parseInt(params.iDate, 10) != -1) {
        minSelectedDate(moment(params.iDate, 'YYYY-MM-DD'));
        maxSelectedDate(moment(params.iDate, 'YYYY-MM-DD'));
        calSelectedDays[moment(params.iDate, 'YYYY-MM-DD').format("YYYYMMDD")] = true;
    }

    var viewTitle = ko.computed(function () {
        return i18nextko.i18n.t('roRequestType_PlannedAbsences')
    });

    var absenceTitle = ko.computed(function () {
        return i18nextko.i18n.t('roSelectAbsence')
    });

    var formReadOnly = ko.observable(false);

    var reqValue1 = ko.observable(null), requestValue1DS = ko.observable([]);

    var myApprovalsBlock = VTPortal.approvalHistory();

    var computedScrollHeight = ko.computed(function () {
        return '76%'
    });

    var loadRequest = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                remarks(result.Comments);

                absenceId(result.AbsenceId);
                requestStatus(result.ReqStatus);

                if (result.ReqStatus == 0) canDelete(true);
                else canDelete(false);

                viewActions(false);
                if (VTPortal.roApp.impersonatedIDEmployee != -1) {
                    if (result.ReqStatus == 0 || result.ReqStatus == 1) {
                        viewActions(true);
                        canDelete(false);
                    }
                }

                var iDate = moment(result.strDate1, "YYYY-MM-DD HH:mm:ss").startOf('day');
                minSelectedDate(iDate.clone());

                var eDate = moment(result.strDate2, "YYYY-MM-DD HH:mm:ss").startOf('day');
                maxSelectedDate(eDate)
                while (iDate <= eDate) {
                    calSelectedDays[iDate.clone().add(12, 'hours').format("YYYYMMDD")] = true;
                    iDate = iDate.add(1, 'day');
                }

                if (typeof absenceId() != 'undefined' && absenceId() != null && absenceId() > 0) {
                    if (moment(result.strDate1, "YYYY-MM-DD HH:mm:ss") > moment()) {
                        canDelete(true);
                    }
                    else {
                        canDelete(false);
                    }
                }

                myApprovalsBlock.refreshApprovals(result.RequestsHistoryEntries);

                reqValue1(result.IdCause + '');

                loadWorkingCalendar(new Date());
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
                requestValue1DS(result.SelectFields);

                if (requestId() != null && requestId() > 0) {
                    loadRequest(requestId());
                } else {
                    if (result.SelectFields.length > 0) reqValue1(result.SelectFields[0].FieldValue);
                    loadWorkingCalendar(new Date());
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

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var wsRobotics = null;

    var uploadUserPhoto = function () {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtHome();

                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roDocumentUploaded"), 'success', 2000);
            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        };

        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);
        var document = documentsBlock.getDocument();

        if (VTPortal.roApp.isModeApp() == false) {
            document.append("idRelatedObject", requestId());
            document.append("forecastType", "request");

            wsRobotics.uploadDocumentDesktop(document);
        }
        else {
            document.append("idRelatedObject", requestId());
            wsRobotics.uploadDocumentMobile(document, "request", "", "");
        }
    }

    function viewShown() {
        globalStatus().viewShown();
        if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Holidays) {
            requestValue1DS([]);

            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.CausesPermissions) loadLists('causes.programmedAbsence');
            else loadLists('causes.visibilitypermissions');

            if (requestId() != null && requestId() > 0) {
                formReadOnly(true);
                VTPortalUtils.utils.markRequestAsRead(requestId());
            }

            if (VTPortal.roApp.impersonatedIDEmployee == -1) {
                if (requestId() != null) canDelete(true);
            } else {
                if (requestId() != null) viewActions(false);
            }
            if (requestId() == null) canSave(true);
            if (requestId() != null) viewHistory(true);
        } else {
            var onContinue = function () {
                VTPortal.roApp.loadInitialData(false, false, true, false, false);
                VTPortal.roApp.redirectAtHome();
            }

            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roIncorrectApiVersion'), 'error', 0, onContinue);
        }
    }

    function loadWorkingCalendar(selDate) {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                var cData = calendarStatus();
                for (var i = 0; i < result.oCalendar.DayData.length; i++) {
                    cData[moment.tz(result.oCalendar.DayData[i].PlanDate, VTPortal.roApp.serverTimeZone).add(12, 'hours').format("YYYYMMDD")] = result.oCalendar.DayData[i];
                }
                calendarStatus(cData);
                if (selectedIndex() == 1) setTimeout(function () { $("#calendarSelectorContainer").dxCalendar("instance").repaint(); }, 100);
            } else {
                calendarStatus([]);
                var onContinue = function () {
                    VTPortal.roApp.loadInitialData(false, false, true, false, false);
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
            isRequestingData = false;
        }, function () { isRequestingData = false; }).getAvailablePermitsCalendar(selDate, -1);
    }

    var wsRobotics = null;
    function saveRequest() {
        if (minSelectedDate() == null || maxSelectedDate == null) {
            selectedIndex(1);
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roSelectDayInfo'), 'info', 2000);
        } else {
            savePlannedAbsence();
        }
    }

    function savePlannedAbsence() {
        var onWSError = function (error) {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorSavingRequest"), 'error', 0);
        }

        var document = documentsBlock.getDocument();

        var onWSResult = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.redirectAtRequestList(7);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roRequestSaved"), 'success', 2000);
            } else {
                var onContinue = function () {
                    wsRobotics.saveAbsences(minSelectedDate() == null ? VTPortalUtils.nullDate : minSelectedDate(), maxSelectedDate() == null ? VTPortalUtils.nullDate : maxSelectedDate(), reqValue1(), remarks(), true, document);
                }

                VTPortalUtils.utils.processRequestErrorMessage(result, onContinue, function () { });
            }
        };
        if (wsRobotics == null) wsRobotics = new WebServiceRobotics(onWSResult, onWSError);

        wsRobotics.saveAbsences(minSelectedDate() == null ? VTPortalUtils.nullDate : minSelectedDate(), maxSelectedDate() == null ? VTPortalUtils.nullDate : maxSelectedDate(), reqValue1(), remarks(), false, document);
    }

    function deleteRequest() {
        //APV

        if (typeof absenceId() != 'undefined' && absenceId() != null && absenceId() > 0) {
            VTPortalUtils.utils.deleteProgrammedAbsence(absenceId(), "days");
        }
        else {
            if (requestId() != null && requestId() > 0) VTPortalUtils.utils.deleteRequest(requestId());
        }
    }

    function getCellTemplate(data) {
        var style = '', color = "#dcdcdc", absenceColor = "#dcdcdc", textColor = '#000000';
        var iGradient = 90, eGradient = 95;
        var selDay = calendarStatus()[moment(data.date).format("YYYYMMDD")];
        var workingHous = 0;
        var actuallyHolidays = false;

        var actualDayClass = '';
        if (moment(data.date).format("YYYYMMDD") == moment().format("YYYYMMDD")) actualDayClass = 'actualDate';

        if (selDay != null) {
            if (selDay.MainShift != null) {
                workingHous = selDay.MainShift.PlannedHours;
                actuallyHolidays = selDay.IsHoliday;
            }

            color = VTPortalUtils.calendar.getCellColor(calSelectedDays, data.date, data.view);

            absenceStyle = 'width:100%;height:100%;color: ' + absenceColor + ';';
            absenceStyle += 'background: ' + absenceColor + ';';
            absenceStyle += 'background: -webkit-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background: -moz-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background: -ms-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background: -o-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background: radial-gradient(circle closest-side at center, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
            absenceStyle += 'background-repeat: no-repeat;';
            absenceStyle += 'background-position: center center;display:table;';//font-weight:bold;';

            style = 'width:100%;height:100%;color: ' + textColor + ';';
            style += 'background: ' + color + ';';
            style += 'background: -webkit-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background: -moz-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background: -ms-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background: -o-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background: radial-gradient(circle closest-side at center, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
            style += 'background-repeat: no-repeat;';
            style += 'background-position: center center;display:table;';

            if (data.view == 'month') {
                return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span class='" + actualDayClass + "' style='display:table-cell;vertical-align:middle;font-weight:bold'>" + data.text + "</span></div></div></div></div>";
            } else if (data.view == 'year') {
                if (moment(data.date).isAfter(moment(minDate()).startOf('month').add(-1, 'day')) && moment(data.date).isBefore(moment(maxDate()))) {
                    var checkInitial = moment(data.date).startOf('month');
                    var checkEnd = moment(data.date).endOf('month');
                    var bCounter = 0;

                    while (checkInitial < checkEnd) {
                        if (calSelectedDays[checkInitial.format("YYYYMMDD")] == true) {
                            bCounter += 1;
                        }

                        checkInitial = checkInitial.add(1, 'day');
                    }

                    if (bCounter > 0) return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span style='display:table-cell;vertical-align:middle'>" + data.text.capitalize(true, true) + " (" + bCounter + ")</span></div></div></div></div>";
                    else return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span style='display:table-cell;vertical-align:middle'>" + data.text.capitalize(true, true) + "</span></div></div></div></div>";
                } else {
                    return "";
                }
            }
        } else {
            if (moment(data.date).isAfter(moment(minDate()).startOf('month').add(-1, 'day')) && moment(data.date).isBefore(moment(maxDate()))) {
                if (!isRequestingData) {
                    isRequestingData = true;
                    loadWorkingCalendar(data.date);
                }

                return "<span class='" + actualDayClass + "' >" + data.text + "</span>";
            } else {
                return "";
            }
        }
    }

    var onCellClicked = function (e) {
        if (requestId() == null && typeof calendarStatus()[moment(e.value).format("YYYYMMDD")] != 'undefined') {
            if (calendarSelectMode() == 0) {
                //Descomentar para activar la validación de días festivos
                //var selDay = calendarStatus()[moment(e.value).format("YYYYMMDD")];
                //var isActuallyHoliday = selDay.IsHoliday;

                //if (selDay.MainShift != null && selDay.MainShift.PlannedHours > 0 && isActuallyHoliday == false) {
                if (minSelectedDate() == null && maxSelectedDate() == null) {
                    minSelectedDate(moment(e.value));
                    maxSelectedDate(moment(e.value));
                } else {
                    if (moment(e.value).isSame(minSelectedDate())) minSelectedDate(maxSelectedDate());
                    else if (moment(e.value).isSame(maxSelectedDate())) maxSelectedDate(minSelectedDate());
                    else if (moment(e.value) > minSelectedDate()) maxSelectedDate(moment(e.value));
                    else if (moment(e.value) < minSelectedDate()) minSelectedDate(moment(e.value));
                }
                //}

                var checkInitial = moment(minDate());
                var checkEnd = moment(maxDate());

                while (checkInitial < checkEnd) {
                    if (checkInitial >= minSelectedDate() && checkInitial <= maxSelectedDate()) {
                        calSelectedDays[checkInitial.format("YYYYMMDD")] = true;
                    } else {
                        if (typeof calSelectedDays[checkInitial.format("YYYYMMDD")] != 'undefined') {
                            calSelectedDays[checkInitial.format("YYYYMMDD")] = false;
                        }
                    }
                    checkInitial = checkInitial.add(1, 'day');
                }
            } else {
                var checkInitial = null;
                var checkEnd = null;

                if (minSelectedDate() != null) checkInitial = minSelectedDate().clone();
                if (maxSelectedDate() != null) checkEnd = maxSelectedDate().clone();

                if (checkInitial != null) {
                    while (checkInitial <= checkEnd) {
                        calSelectedDays[checkInitial.format("YYYYMMDD")] = false;
                        checkInitial = checkInitial.add(1, 'day');
                    }
                }

                minSelectedDate(moment(e.value));
                maxSelectedDate(minSelectedDate().clone().add(calendarSelectMode() - 1, 'day'));

                checkInitial = minSelectedDate().clone();
                checkEnd = maxSelectedDate().clone();

                while (checkInitial <= checkEnd) {
                    calSelectedDays[checkInitial.format("YYYYMMDD")] = true;
                    checkInitial = checkInitial.add(1, 'day');
                }
            }

            $("#calendarSelectorContainer").dxCalendar("instance").repaint();
        }
    };

    var changeTab = function (item) {
        if (item.ID == 2) {
            setTimeout(function () { $("#calendarSelectorContainer").dxCalendar("instance").repaint(); }, 200);
        } else if (item.ID == 3) {
            onChangeCalendarSelection();
        }
    }

    var viewModel = {
        requestId: requestId,
        myApprovalsBlock: myApprovalsBlock,
        viewShown: viewShown,
        title: viewTitle,
        lblDocument: i18nextko.t('roLblDocument'),
        btnCancelUpload: {
            onClick: function () { popupDocumentsVisible(false); },
            text: i18nextko.t('roCancel'),
        },
        btnAcceptUpload: {
            onClick: function () {
                if (requestId() != null && requestId() > 0) {
                    uploadUserPhoto();
                }
                else {
                    documentAttached(true);
                    popupDocumentsVisible(false);
                }
            },
            text: i18nextko.t('roSaveRequest'),
        },
        lblRemarks: i18nextko.t('roRequestRemarksLbl'),
        lblRequestValue1: absenceTitle,
        lblSelectDaysInfo: i18nextko.t('roSelectDayInfo'),
        subscribeBlock: globalStatus(),
        selectedTab: selectedIndex,

        scrollContent: {
            height: computedScrollHeight
        },
        tabPanelOptions: {
            dataSource: availableTabPanels,
            selectedIndex: selectedIndex,
            itemTemplate: "title",
            onSelectionChanged: function (e) {
                changeTab(e.addedItems[0]);
            }
        },
        popupVisible: popupVisible,
        popupDocumentsVisible: popupDocumentsVisible,
        btnApprove: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 7, true); },
            text: '',
            hint: i18nextko.i18n.t('roApprove'),
            icon: "Images/Common/approve.png",
            visible: viewActions
        },
        btnRefuse: {
            onClick: function () { VTPortalUtils.utils.approveRefuseRequest(requestId(), 7, false); },
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
        btnDocuments: {
            onClick: function () { popupDocumentsVisible(true); },
            text: '',
            hint: i18nextko.i18n.t('roLblDocuments'),
            icon: "Images/Common/Document.png"
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
        calendarOptions: {
            width: '100%',
            useCellTemplate: true,
            cellTemplate: getCellTemplate,
            firstDayOfWeek: moment()._locale._week.dow,
            maxZoomLevel: 'month',
            zoomLevel: 'year',
            minZoomLevel: 'year',
            onCellClick: onCellClicked,
            showTodayButton: false,
            min: minDate,
            max: maxDate
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
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        noRankingDateSelected: noRankingDateSelected,
        hasRankingData: hasRankingData,
        hasCoverageData: hasCoverageData,
        rankingDS: rankingDS,
        listRanking: {
            dataSource: rankingDS,
            scrollingEnabled: false,
            collapsibleGroups: false,
            itemTemplate: 'RankingItem',
        },
        lblRankingHeader: i18nextko.t('roRankingTitle'),
        lblCoverageHeader: i18nextko.t('roCoverageTitle'),
        lblNoDataDescription: i18nextko.t('roNoRankingDescription'),
        lblMinimumCoverageRequiered: i18nextko.t('rolblMinimumCoverageRequiered'),
        txtMinimumCoverageRequiered: {
            value: txtMinimumCoverageRequiered,
            readOnly: true
        },
        lblDiferentialCoverage: i18nextko.t('rolblDiferentialCoverage'),
        txtDifferentialCoverage: {
            value: txtDifferentialCoverage,
            readOnly: true
        },
        myDocumentsBlock: documentsBlock,
        hasDocuments: causeHasDocuments,
        fileAdded: fileAdded,
        documentAttached: documentAttached
    };

    return viewModel;
};