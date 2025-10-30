VTPortal.myLeavesAlerts = function (params) {
    var modelIsReady = ko.observable(false);
    var alertDS = ko.observable([]);

    function viewShown(selDate) {
        refreshData();
        modelIsReady(true);
    };

    var hasPermission = ko.computed(function () {
        if (VTPortal.roApp.empPermissions() == null || (VTPortal.roApp.empPermissions() != null && VTPortal.roApp.empPermissions().LeavesEnabled)) {
            return true;
        } else {
            return false;
        }
    });

    function refreshData() {
        var alerts = VTPortal.roApp.userStatus().ScheduleStatus;

        var lstAlerts = [];

        if (alerts != null) {
            var cssClass = '';

            if (typeof alerts.TrackingDocuments != 'undefined') {
                cssClass = 'dx-icon-trackingDocuments';

                for (var i = 0; i < alerts.TrackingDocuments.length; i++) {
                    var forecastType = 'days';
                    var relatedId = 0;

                    if (alerts.TrackingDocuments[i].IdHoursAbsence > 0) {
                        forecastType = 'hours';
                        relatedId = alerts.TrackingDocuments[i].IdHoursAbsence;
                    } else if (alerts.TrackingDocuments[i].IdOvertimeForecast > 0) {
                        forecastType = 'overtime';
                        relatedId = alerts.TrackingDocuments[i].IdOvertimeForecast;
                    } else if (alerts.TrackingDocuments[i].IdDaysAbsence > 0) {
                        forecastType = 'days';
                        relatedId = alerts.TrackingDocuments[i].IdDaysAbsence;
                    }

                    if (alerts.TrackingDocuments[i].Scope == 3 && forecastType == 'days') {
                        lstAlerts.push({
                            ID: -1,
                            IdDocumentTemplate: alerts.TrackingDocuments[i].IDDocumentTemplate,
                            IdRelatedObject: relatedId,
                            IdCause: alerts.TrackingDocuments[i].IdCause,
                            ForecatstType: forecastType,
                            NotificationDate: moment.tz(alerts.TrackingDocuments[i].DateTime, VTPortal.roApp.serverTimeZone),
                            cssClass: cssClass,
                            name: i18nextko.i18n.t('roTrackingDocument'),
                            hasAction: true,
                            type: 'tracking',
                            description: alerts.TrackingDocuments[i].Description,
                            iDate: null
                        });
                    }
                }
            }
        }

        alertDS(lstAlerts);
    }

    var onNewRequest = function () {
        VTPortal.app.navigate('newLeave/-1');
    }

    var viewModel = {
        modelIsReady: modelIsReady,
        viewShown: viewShown,
        hasPermission: hasPermission,
        newRequest: {
            onClick: onNewRequest,
            text: '',
            icon: "Images/Common/plus.png",
            visible: VTPortal.roApp.ReadMode() == false
        },
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        alertDS: alertDS,
        listAlerts: {
            dataSource: alertDS,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'AlertItem',
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    switch (info.itemData.type) {
                        case 'tracking':
                            VTPortal.app.navigate("sendLeaveDocument/" + info.itemData.IdDocumentTemplate + "/" + info.itemData.IdRelatedObject + "/" + info.itemData.IdCause + "/" + info.itemData.ForecatstType);
                            break;
                    }
                }
            }
        }
    };

    return viewModel;
};