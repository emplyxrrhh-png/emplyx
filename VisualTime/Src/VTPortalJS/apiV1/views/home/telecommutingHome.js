VTPortal.telecommutingHome = function (params) {
    "use strict";

    var tcInfo = ko.observable(
        null
    );

    var tcToday = ko.computed(function () {
        if (tcInfo() != null) {
            if (tcInfo().TCToday.toLowerCase() == 'true') {
                return i18nextko.i18n.t('tcTodayHome');
            }
            else {
                if (tcInfo().NoWorkToday.toLowerCase() == "true") {
                    return i18nextko.i18n.t('tcNoWorkToday')
                }
                else {
                    if (tcInfo().WorkcenterNameToday == "") {
                        return i18nextko.i18n.t('tcTodayOfficeNWC')
                    }
                    else {
                        return i18nextko.i18n.t('tcTodayOffice') + tcInfo().WorkcenterNameToday;
                    }
                }
            };
        }
        else {
            return i18nextko.i18n.t('tcLoading');
        }
    });

    var tcTomorrow = ko.computed(function () {
        if (tcInfo() != null) {
            if (tcInfo().TCTomorrow.toLowerCase() == 'true') {
                return i18nextko.i18n.t('tcTomorrowHome');
            }
            else {
                if (tcInfo().NoWorkTomorrow.toLowerCase() == "true") {
                    return i18nextko.i18n.t('tcNoWorkTomorrow')
                }
                else {
                    if (tcInfo().WorkcenterNameTomorrow == "") {
                        return i18nextko.i18n.t('tcTomorrowOfficeNWC')
                    }
                    else {
                        return i18nextko.i18n.t('tcTomorrowOffice') + tcInfo().WorkcenterNameTomorrow;
                    }
                }
            };
        }
        else {
            return '';
        }
    });

    //var workcenterStatus = ko.computed(function () {
    //    if (tcInfo().WorkcenterStatus.length != 0) {
    //        return i18nextko.i18n.t('tcWorkcenterStatus') + tcInfo().WorkcenterStatus;
    //    }
    //    else { return '' };
    //});

    //var workcenterStatusTomorrow = ko.computed(function () {
    //    if (tcInfo().WorkcenterStatus.length != 0) {
    //        return i18nextko.i18n.t('tcWorkcenterStatusTomorrow') + tcInfo().WorkcenterStatusTomorrow;
    //    }
    //    else { return '' };
    //});

    var telecommutingTitle = ko.computed(function () {
        return i18nextko.i18n.t('roTeleworkingTitle');
    });

    var getTCInfo = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                tcInfo(result);
            } else {
                DevExpress.ui.notify(i18nextko.t('roTCError'), 'warning', 3000);
            }
        }).getTelecommutingInfo();
    }

    var viewShown = function (e) {
        getTCInfo();
    }

    var viewModel = {
        viewShown: viewShown,
        telecommutingTitle: telecommutingTitle,
        lblTCToday: tcToday,
        lblTCTomorrow: tcTomorrow,
        iconTC: {
            icon: 'map'
        },
    };

    return viewModel;
};