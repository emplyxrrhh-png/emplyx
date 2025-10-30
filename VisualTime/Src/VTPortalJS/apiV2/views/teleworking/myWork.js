VTPortal.myWork = function (params) {
    var modelIsReady = ko.observable(false);
    var teleworkingDS = ko.observable([]);
    var popoverVisible = ko.observable(false);
    var isRequestingData = true;
    var calCell = ko.observable('#calendarContainer')
    var noPermissions = VTPortal.noPermissions();
    var yearsLoaded = ko.observable([]);

    function viewShown(selDate) {
        yearsLoaded([]);
        refreshData(selDate);
        modelIsReady(true);
    };

    var refreshData = function (selDate) {
        var yearRequest = moment(selDate).year();
        if (yearsLoaded().indexOf(yearRequest) == -1 && hasPermission()) {
            yearsLoaded().push(yearRequest);

            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    teleworkingDS(result.Requests);
                    setTimeout(function () { $("#calendarContainer").dxCalendar("instance").repaint(); }, 100);
                } else {
                    teleworkingDS([]);
                    var onContinue = function () {
                        VTPortal.roApp.loadInitialData(false, false, true, false, false);
                        VTPortal.roApp.redirectAtHome();
                    }
                    VTPortalUtils.utils.processErrorMessage(result, onContinue);
                }
                isRequestingData = false;
                // $('#scrollview').height($('#panelsContent').height() - 20);
            }, function () { yearsLoaded(yearsLoaded().remove(yearRequest)); isRequestingData = false; }).getTeleworkingDetail(moment().startOf('day').startOf('year'), moment().startOf('day').endOf('year'));
        } else {
            isRequestingData = false;
        }
    }

    var hasPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().Punch.ScheduleQuery)) {
            return true;
        } else {
            return false;
        }
    });

    var onNewRequest = function () {
        VTPortal.app.navigate('requestsList/4,15');
    }

    function getCellTemplate(data) {
        var absenceStyle = '', style = '', color = "#dcdcdc", absenceColor = "#dcdcdc", textColor = '#000000';
        var iGradient = 90, eGradient = 95;

        var actualDayClass = '';
        if (moment(data.date).format("YYYYMMDD") == moment().format("YYYYMMDD")) actualDayClass = 'actualDate';

        var teleworkingRequests = teleworkingDS();

        var selDay = null; // Debemos buscar si hay una baja en este dia dentro de  leavesDS();

        for (var i = 0; i < teleworkingRequests.length; i++) {
            var req = teleworkingRequests[i];
            if (req.RequestDays != null && req.RequestDays.length > 0) {
                for (var x = 0; x < req.RequestDays.length; x++) {
                    if (moment.tz(req.RequestDays[x].RequestDate, VTPortal.roApp.serverTimeZone).startOf('day').isSame(moment(data.date))) {
                        selDay = true;
                    } else {
                        selDay = false;
                    }
                    if (selDay) break;
                }
            } else {
                if (moment.tz(req.Date1, VTPortal.roApp.serverTimeZone).isSame(moment(data.date))) {
                    selDay = true;
                } else {
                    selDay = false;
                }
            }
            if (selDay) break;
        }

        if (selDay != null) {
            if (selDay) color = '#0046FE';
        } else {
            if (!isRequestingData) {
                isRequestingData = true;
                refreshData(data.date);
            }
        }

        absenceStyle = 'width:100%;height:100%;color: ' + absenceColor + ';';
        absenceStyle += 'background: ' + absenceColor + ';';
        absenceStyle += 'background: -webkit-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
        absenceStyle += 'background: -moz-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
        absenceStyle += 'background: -ms-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
        absenceStyle += 'background: -o-radial-gradient(circle closest-side, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
        absenceStyle += 'background: radial-gradient(circle closest-side at center, ' + absenceColor + ' 90%,rgba(255,255,255,0) 95%);';
        absenceStyle += 'background-repeat: no-repeat;';
        absenceStyle += 'background-position: center center;display:table;';//font-weight:bold;';

        style = 'width:100%;height:100%;min-height:50px;color: ' + textColor + ';';
        style += 'background: ' + color + ';';
        style += 'background: -webkit-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background: -moz-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background: -ms-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background: -o-radial-gradient(circle closest-side, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background: radial-gradient(circle closest-side at center, ' + color + ' ' + iGradient + '%,rgba(255,255,255,0) ' + eGradient + '%);';
        style += 'background-repeat: no-repeat;';
        style += 'background-position: center center;display:table;';

        //return "<div style='width:100%;height:100%'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div style='" + style + "'> <span id='" + moment(data.date).format("YYYYMMDD") + "' style='display:table-cell;vertical-align:middle;color:#fff;mix-blend-mode: difference;'>" + data.text + "</span></div></div></div></div>";
        //return "<div style='width:100%;height:100%' onClick='VTPortal.myCalendar().onCellClicked(\"" + moment(data.date).format("YYYYMMDD") + "\")'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div style='" + style + "'> <span id='" + moment(data.date).format("YYYYMMDD") + "' style='display:table-cell;vertical-align:middle;'>" + data.text + "</span></div></div></div></div>";
        return "<div style='width:100%;height:100%;min-height:50px;'><div class='innerCalCellTemplate' style='" + absenceStyle + "'><div style='display:table-cell;vertical-align:middle'><div id='" + moment(data.date).format("YYYYMMDD") + "' style='" + style + "'> <span class='" + actualDayClass + "' style='display:table-cell;vertical-align:middle;font-weight:bold'>" + data.text + "</span></div></div></div></div>";
    }

    var onCellClicked = function (e) {
        selectedDay = e.value;
        calCell('#' + moment(e.value).format("YYYYMMDD"));
        popoverVisible(true);
    };

    var dayToolbarActions = [];

    if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().CanCreateRequests)) {
        dayToolbarActions.push({
            location: 'center',
            widget: 'dxButton',
            options: {
                hint: i18nextko.i18n.t('roNewRequest_Detail'),
                icon: "taNewRequest_Detail",
                onClick: function () {
                    popoverVisible(false);
                    VTPortal.app.navigate('requestsList/4,15/' + moment(selectedDay).format("YYYY-MM-DD"));
                }
            }
        });
    }

    dayToolbarActions.push({
        location: 'center',
        widget: 'dxButton',
        options: {
            hint: i18nextko.i18n.t('roInformation'),
            icon: "taInformation",
            onClick: function () {
                popoverVisible(false);
                VTPortal.app.navigate("teleworking/2/" + moment(selectedDay).format("YYYY-MM-DD"), { target: 'current' });
            }
        }
    });

    var viewModel = {
        modelIsReady: modelIsReady,
        initializeView: viewShown,
        hasPermission: hasPermission,
        noPermissions: noPermissions,
        calendarOptions: {
            width: '100%',
            height: 450,
            useCellTemplate: true,
            cellTemplate: getCellTemplate,
            firstDayOfWeek: moment()._locale._week.dow,
            zoomLevel: 'month',
            maxZoomLevel: 'month',
            minZoomLevel: 'month',
            //onValueChanged: onCellClicked,
            onCellClick: onCellClicked,
            showTodayButton: false
        },
        calendarCellPopover: {
            target: calCell,
            position: "top",
            width: 300,
            shading: true,
            shadingColor: "rgba(0, 0, 0, 0.5)",
            visible: popoverVisible
        },
        dayToolbarActions: { items: dayToolbarActions },
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