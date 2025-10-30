VTPortal.dailyRecordHome = function (params) {
    "use strict";

    var selectedDay = ko.observable(moment().startOf('month').toDate());

    var dsDailyRecord = ko.observable(null);
    var statusIcon = ko.computed(function () {
        if (!VTPortal.roApp.lastRequestFailed()) return "sandClock";
        else return "noConnection";
    })

    var dailyRecordPending = ko.computed(function () {
        var registeredDays = 0;
        var ongoingDays = 0;
        if (dsDailyRecord() != null) {
            registeredDays = dsDailyRecord().filter(element => (element.DateStatus == 3 || element.DateStatus == 2)).length;
            ongoingDays = dsDailyRecord().filter(element => (element.DateStatus == 1)).length;
        }

        var sText = i18nextko.i18n.t('roDailyRecordRecorded'); //{'registered':registeredDays,'ongoing':ongoingDays});
        sText = sText.replace('{{registered}}', (ongoingDays + registeredDays) + '');
        sText = sText.replace('{{ongoing}}', ongoingDays);

        return sText;
    });
    var dailyRecordMissing = ko.computed(function () {
        var pendingDays = 0;
        if (dsDailyRecord() != null)  pendingDays = dsDailyRecord().filter(element => (element.DateStatus == 0)).length;
        var sText = i18nextko.i18n.t('roDailyRecordMissing');
        sText = sText.replace('{{pending}}', pendingDays);

        return sText;
    });

    var statusTitle = ko.computed(function () {
        return i18nextko.i18n.t('roDailyRecord');
    });

    function goToDR() {
        window.VTPortalUtils.utils.setActiveTab('dailyRecord');
        VTPortal.app.navigate('myDailyRecord', { root: true });
    }

    var getDailyRecord = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {

                VTPortal.roApp.db.settings.updateCacheDS('dailyrecord', result.Value.DayData);
                dsDailyRecord(result.Value.DayData);
            } else {
                dsDailyRecord([]);
                var onContinue = function () {
                    VTPortal.roApp.loadInitialData(false, false, true, false, false);
                    VTPortal.roApp.redirectAtHome();
                }
                VTPortalUtils.utils.processErrorMessage(result, onContinue);
            }
        }, function () { }).getMyDailyRecordCalendar(selectedDay());
    }

    function dailyRecordViewShown() {
        dsDailyRecord(VTPortal.roApp.db.settings.getCacheDS('dailyrecord'));
        if (dsDailyRecord() == null || window.VTPortalUtils.needToRefresh('dailyrecord')) getDailyRecord();
    };

    var viewModel = {
        viewShown: dailyRecordViewShown,
        statusTitle: statusTitle,
        lblDailyRecordMissing: dailyRecordMissing,
        lblDailyRecordPending: dailyRecordPending,
        statusIcon: statusIcon,
        goToDR: goToDR,
        btnGoDR: {
            onClick: goToDR,
            icon: 'spinright'
        },
        iconDR: {
            onClick: goToDR,
            icon: 'event'
        },
    };

    return viewModel;
};