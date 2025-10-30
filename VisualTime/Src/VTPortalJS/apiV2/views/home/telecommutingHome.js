VTPortal.telecommutingHome = function (params) {

    var tcInfo = ko.observable(
        null
    );

    var tcCurrentPercentage = ko.computed(function () {
        if (tcInfo() != null) {
            if (tcInfo().ByPercentage == true) {
                switch (tcInfo().Period) {
                    case 0:
                        return i18nextko.i18n.t('tcPercentageUsed') + ' ' + tcInfo().CurrentAgreementPercentageUsed + i18nextko.i18n.t('tcPercentageUsed2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxPercentage + '%/' + i18nextko.i18n.t('semana') + ')';
                        break;
                    case 1:
                        return i18nextko.i18n.t('tcPercentageUsed') + ' ' + tcInfo().CurrentAgreementPercentageUsed + i18nextko.i18n.t('tcPercentageUsed2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxPercentage + '%/' + i18nextko.i18n.t('mes') + ')';
                        break;
                    case 2:
                        return i18nextko.i18n.t('tcPercentageUsed') + ' ' + tcInfo().CurrentAgreementPercentageUsed + i18nextko.i18n.t('tcPercentageUsed2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxPercentage + '%/' + i18nextko.i18n.t('trimestre') + ')';
                        break;
                }
            }
            else {
                switch (tcInfo().Period) {
                    case 0:
                        return i18nextko.i18n.t('tcDaysUsed') + ' ' + tcInfo().CurrentAgreementDaysUsed + ' ' + i18nextko.i18n.t('tcDaysUsed2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxDays + 'd/' + i18nextko.i18n.t('semana') + ')';
                        break;
                    case 1:
                        return i18nextko.i18n.t('tcDaysUsed') + ' ' + tcInfo().CurrentAgreementDaysUsed + ' ' + i18nextko.i18n.t('tcDaysUsed2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxDays + 'd/' + i18nextko.i18n.t('mes') + ')';
                        break;
                    case 2:
                        return i18nextko.i18n.t('tcDaysUsed') + ' ' + tcInfo().CurrentAgreementDaysUsed + ' ' + i18nextko.i18n.t('tcDaysUsed2') + ' (' + i18nextko.i18n.t('tcPercentageUsed3') + tcInfo().MaxDays + 'd/' + i18nextko.i18n.t('trimestre') + ')';
                        break;
                }
            };
        }
        else {
            return '';
        }
    })

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

    var telecommutingTitle = ko.computed(function () {
        return i18nextko.i18n.t('roTeleworkingTitle');
    });

    var getTCInfo = function () {
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.db.settings.updateCacheDS('tcinfo', result);
                tcInfo(result);
            } else {
                DevExpress.ui.notify(i18nextko.t('roTCError'), 'warning', 3000);
            }
        }).getTelecommutingInfo(moment());
    }

    var viewShown = function (e) {
        tcInfo(VTPortal.roApp.db.settings.getCacheDS('tcinfo'));
        if (tcInfo() == null || window.VTPortalUtils.needToRefresh('tcinfo')) getTCInfo();
    }

    var viewModel = {
        viewShown: viewShown,
        telecommutingTitle: telecommutingTitle,
        lblTCToday: tcToday,
        lblTCTomorrow: tcTomorrow,
        lblCurrentPercentage: tcCurrentPercentage,
        iconTC: {
            icon: 'map'
        },
    };

    return viewModel;
};