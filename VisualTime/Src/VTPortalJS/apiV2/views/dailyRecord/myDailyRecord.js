VTPortal.myDailyRecord = function (params) {
    var globalStatus = ko.observable(VTPortal.bigUserInfo());
    var dsDailyRecord = ko.observable([]);
    var selectedDay = ko.observable(moment().startOf('month').toDate());

    if (typeof params.id != 'undefined' && parseInt(params.id, 10) != -1) requestId(parseInt(params.id, 10));

    var lblDailyRecordPending = ko.computed(function () {
        var registeredDays = dsDailyRecord().filter(element => (element.DateStatus == 3 || element.DateStatus == 2)).length;
        var ongoingDays = dsDailyRecord().filter(element => (element.DateStatus == 1)).length;

        var sText = i18nextko.i18n.t('roDailyRecordRecorded'); //{'registered':registeredDays,'ongoing':ongoingDays});
        sText = sText.replace('{{registered}}', (ongoingDays + registeredDays) + '');
        sText = sText.replace('{{ongoing}}', ongoingDays);

        return sText;
    });
    var lblDailyRecordMissing = ko.computed(function () {
        var pendingDays = dsDailyRecord().filter(element => (element.DateStatus == 0)).length;
        var sText = i18nextko.i18n.t('roDailyRecordMissing');
        sText = sText.replace('{{pending}}', pendingDays);

        return sText;
    });

    var lblDailyRecordResume = ko.computed(function () {
        return i18nextko.i18n.t('roDailyRecordResume');
    });

    function getCellTemplate(data) {
        var actualDayClass = '';
        var textColor = '#FFFFFF;text-shadow: 2px 2px 2px black;';

        var selDay = dsDailyRecord().find(element => moment.tz(element.Date, VTPortal.roApp.serverTimeZone).add(12, 'hours').format("YYYYMMDD") == moment(data.date).format("YYYYMMDD"));
        if (moment(data.date).format("YYYYMMDD") == moment().format("YYYYMMDD")) actualDayClass = 'actualDate';

        style = 'height:100%;border-radius: 10%;';
        if (selDay != null) {
            switch (selDay.DateStatus) {
                case 0: // Día pendiente de realizar la declaración de jornada
                    style += 'background: #D9534F';
                    break;
                case 1: // Declaración solicitada pendiente de aprobar
                    style += 'background: #F0AD4E;';
                    break;
                case 2, 3: // Declaración aceptada independientemente del saldo.
                    style += 'background:#62BA62;';
                    break;
                case 5: // Declaración no permitida
                    style += 'background: transparent;';
                    textColor = '#000000;font-weight: bold;';
                    break;
                default:
                    style += 'background:#e3e3e3;';
                    break;
            }
        }

        return "<div style='width:100%'><div class='CalendarCellDailyLayout'><div style='height:100%' class='innerCalCellTemplate'><div style='" + style + "'> <span class='CalendarCellDailyLayoutText " + actualDayClass + "' id='" + moment(data.date).format("YYYYMMDD") + "' style='color: " + textColor + "'>" + data.text + "</span></div></div></div>";
    }

    function onOptionChanged(e) {
        if (e.fullName == "currentDate") {
            if (moment(e.value).month() != moment(e.previousValue).month()) {
                selectedDay(moment(e.value).startOf('month').toDate());
                viewShown();
            }
        }
    }

    function onCellClicked(e) {
        var cellClickedDate = moment(e.value);
        var sDay = dsDailyRecord().find(element => moment.tz(element.Date, VTPortal.roApp.serverTimeZone).add(12, 'hours').format("YYYYMMDD") == moment(cellClickedDate).format("YYYYMMDD"));

        if (typeof (sDay != 'undefined') && sDay != null && sDay.DateStatus != 5) {
            if (moment().startOf('day').isBefore(cellClickedDate)) VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roDailyRecordFuture"), 'error', 3000);
            else {
                var idDailyRecord = sDay.IdRecord;
                if (idDailyRecord < 0 || sDay.DateStatus == 0) {
                    VTPortal.app.navigate("addDailyRecord/" + idDailyRecord + "/" + cellClickedDate.format("YYYY-MM-DD") + "/" + sDay.CanTelecommute + "/" + sDay.HasPunchesPattern);
                } else {
                    VTPortal.app.navigate("dailyRecordDetail/" + idDailyRecord + "/" + cellClickedDate.format("YYYY-MM-DD") + "/" + sDay.CanTelecommute + "/" + "FC");
                }
            }
        } else {
            VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roDailyRecordNotAllowed"), 'error', 3000);
        }
    }

    function viewShown() {
        VTPortal.roApp.db.settings.markForRefresh(['dailyrecord']);
        globalStatus().viewShown();

        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                dsDailyRecord(result.Value.DayData);

                setTimeout(function () { $("#calendarContainer").dxCalendar("instance").repaint(); $("#calendarContainer").height($('#scrollview').height()); }, 100);
            } else {
                dsDailyRecord([]);
                var onContinue = function () {
                    VTPortal.roApp.loadInitialData(false, false, true, false, false);
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
            // $('#scrollview').height($('#panelsContent').height() - 20);
        }, function () { }).getMyDailyRecordCalendar(selectedDay());
    };

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roMyDailyRecord'),
        subscribeBlock: globalStatus(),
        lblDailyRecordMissing: lblDailyRecordMissing,
        lblDailyRecordPending: lblDailyRecordPending,
        lblDailyRecordResume: lblDailyRecordResume,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        scrollContent: {
        },
        calendarOptions: {
            width: '104%',
            height: 465,
            useCellTemplate: true,
            cellTemplate: getCellTemplate,
            firstDayOfWeek: moment()._locale._week.dow,
            zoomLevel: 'month',
            maxZoomLevel: 'month',
            minZoomLevel: 'month',
            showTodayButton: false,
            onCellClick: onCellClicked,
            onOptionChanged: onOptionChanged
        }
    };

    return viewModel;
};

//,
//addDailyRecord: {
//    text: 'Añadir declaración',
//        visible: true,
//            onClick: function (e) {
//                VTPortal.app.navigate("addDailyRecord");
//            }

//}