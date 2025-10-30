VTPortal.alerts = function (params) {
    var alertDS = ko.observable([]);

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var notiAction = function (info) {
        switch (info.itemData.IdNotificationType) {
            case 51:
                VTPortal.app.navigate("scheduler/1/" + info.itemData.NotificationDate.format("YYYY-MM-DD"), { target: 'current' });
                break;
            case 52:
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roRead'), 'success', 1000);
                generateAlerts();
                break;
            case 75:
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t('roRead'), 'success', 1000);
                generateAlerts();
                break;
        }
    }

    var generateAlerts = function () {
        globalStatus().viewShown();
        var alerts = VTPortal.roApp.userStatus().ScheduleStatus;

        var lstAlerts = [];

        if (alerts != null) {
            var cssClass = '';
            var hasAction = true;
            if (typeof alerts.RequestAlerts != 'undefined') {
                for (var i = 0; i < alerts.RequestAlerts.length; i++) {
                    switch (alerts.RequestAlerts[i].IdRequestType) {
                        case 1:
                            cssClass = 'dx-icon-userFieldChange';
                            break;
                        case 2:
                            cssClass = 'dx-icon-forbiddenPunch';
                            break;
                        case 3:
                            cssClass = 'dx-icon-justifyPunch';
                            break;
                        case 4:
                            cssClass = 'dx-icon-externalWorkResume';
                            break;
                        case 5:
                            cssClass = 'dx-icon-changeShift';
                            break;
                        case 6:
                            //cssClass = 'dx-icon-holidays';
                            cssClass = 'dx-icon-plannedHoliday';
                            break;
                        case 7:
                            cssClass = 'dx-icon-plannedAbsences';
                            break;
                        case 8:
                            cssClass = 'dx-icon-shiftExchange';
                            break;
                        case 9:
                            cssClass = 'dx-icon-plannedCauses';
                            break;
                        case 10:
                            cssClass = 'dx-icon-forbiddenPunch';
                            //cssClass = 'dx-icon-forbiddenTaskPunch';
                            break;
                        case 11:
                            cssClass = 'dx-icon-holidaysCancel';
                            break;
                        case 12:
                            //cssClass = 'dx-icon-forbiddenCostPunch';
                            cssClass = 'dx-icon-forbiddenPunch';
                            break;
                        case 13:
                            //cssClass = 'dx-icon-plannedHoliday';
                            cssClass = 'dx-icon-plannedHoliday';
                            break;
                        case 14:
                            cssClass = 'dx-icon-plannedOvertime';
                            break;
                        case 15:
                            cssClass = 'dx-icon-externalWorkWeekResume';
                            break;
                        case 16:
                            cssClass = 'dx-icon-telecommute';
                            break;
                    }

                    switch (alerts.RequestAlerts[i].Status) {
                        case 0:
                            name = i18nextko.i18n.t('roRequest_pending');
                            break;
                        case 1:
                            name = i18nextko.i18n.t('roRequest_running');
                            break;
                        case 2:
                            name = i18nextko.i18n.t('roRequest_approved');
                            break;
                        case 3:
                            name = i18nextko.i18n.t('roRequest_denied');
                            break;
                        case 4:
                            name = i18nextko.i18n.t('roRequest_canceled');
                            break;
                    }

                    lstAlerts.push({
                        ID: alerts.RequestAlerts[i].IdRequest,
                        IdRequestType: alerts.RequestAlerts[i].IdRequestType,
                        IdRelatedObject: -1,
                        IdNotificationType: -1,
                        NotificationDate: null,
                        cssClass: cssClass,
                        name: (typeof alerts.RequestAlerts[i].AlertSubject != 'undefined' ? alerts.RequestAlerts[i].AlertSubject : name),
                        hasAction: true,
                        type: 'request',
                        description: alerts.RequestAlerts[i].Description,
                        iDate: null
                    });
                }
            }

            if (typeof alerts.IncompletePunches != 'undefined') {
                cssClass = 'dx-icon-incompletePunch';
                for (var i = 0; i < alerts.IncompletePunches.length; i++) {
                    lstAlerts.push({
                        ID: -1,
                        IdRequestType: -1,
                        IdRelatedObject: -1,
                        IdNotificationType: -1,
                        NotificationDate: null,
                        cssClass: cssClass,
                        name: (typeof alerts.IncompletePunches[i].AlertSubject != 'undefined' ? alerts.IncompletePunches[i].AlertSubject : i18nextko.i18n.t('roIncompletePunch')),//i18nextko.i18n.t('roIncompletePunch'),
                        hasAction: true,
                        type: 'incomplete',
                        description: (typeof alerts.IncompletePunches[i].AlertDescription != 'undefined' ? alerts.IncompletePunches[i].AlertDescription : (i18nextko.i18n.t('roIncompletePunchAt') + ' ' + moment.tz(alerts.IncompletePunches[i].DateTime, VTPortal.roApp.serverTimeZone).format("DD/MM/YYYY"))),//i18nextko.i18n.t('roIncompletePunchAt') + ' ' + moment.tz(alerts.IncompletePunches[i].DateTime, VTPortal.roApp.serverTimeZone).format("DD/MM/YYYY"),
                        iDate: moment.tz(alerts.IncompletePunches[i].DateTime, VTPortal.roApp.serverTimeZone)
                    });
                }
            }

            if (typeof alerts.ForecastDocuments != 'undefined') {
                cssClass = 'dx-icon-trackingDocuments';
                for (var i = 0; i < alerts.ForecastDocuments.length; i++) {
                    var forecastType = 'days';
                    var relatedId = 0;

                    if (alerts.ForecastDocuments[i].IdHoursAbsence > 0) {
                        forecastType = 'hours';
                        relatedId = alerts.ForecastDocuments[i].IdHoursAbsence;
                    } else if (alerts.ForecastDocuments[i].IdOvertimeForecast > 0) {
                        forecastType = 'overtime';
                        relatedId = alerts.ForecastDocuments[i].IdOvertimeForecast;
                    } else if (alerts.ForecastDocuments[i].IdDaysAbsence > 0) {
                        forecastType = 'days';
                        relatedId = alerts.ForecastDocuments[i].IdDaysAbsence;
                    } else if (alerts.ForecastDocuments[i].IdRequest > 0) {
                        forecastType = 'request';
                        relatedId = alerts.ForecastDocuments[i].IdRequest;
                    }

                    hasAction = false;
                    if (alerts.ForecastDocuments[i].IDDocument <= 0) hasAction = true;

                    lstAlerts.push({
                        ID: -1,
                        IdDocumentTemplate: alerts.ForecastDocuments[i].IDDocumentTemplate,
                        IdRelatedObject: relatedId,
                        IdCause: alerts.ForecastDocuments[i].IdCause,
                        ForecatstType: forecastType,
                        NotificationDate: moment.tz(alerts.ForecastDocuments[i].DateTime, VTPortal.roApp.serverTimeZone),
                        cssClass: cssClass,
                        name: i18nextko.i18n.t('roTrackingDocument'),
                        hasAction: hasAction,
                        type: 'tracking',
                        description: alerts.ForecastDocuments[i].Description,
                        iDate: null
                    });
                }
            }

            if (typeof alerts.SignDocuments != 'undefined') {
                cssClass = 'dx-icon-Document_Sign';
                for (var i = 0; i < alerts.SignDocuments.length; i++) {
                    hasAction = false;
                    if (alerts.SignDocuments[i].IDDocument <= 0) hasAction = true;

                    lstAlerts.push({
                        ID: -1,
                        IdDocumentTemplate: alerts.SignDocuments[i].IDDocumentTemplate,
                        IdRelatedObject: relatedId,
                        NotificationDate: moment.tz(alerts.SignDocuments[i].DateTime, VTPortal.roApp.serverTimeZone),
                        cssClass: cssClass,
                        name: alerts.SignDocuments[i].DocumentTemplateName,
                        hasAction: hasAction,
                        type: 'sign',
                        description: alerts.SignDocuments[i].Description,
                        iDate: null
                    });
                }
            }

            if (typeof alerts.Notifications != 'undefined') {
                for (var i = 0; i < alerts.Notifications.length; i++) {
                    cssClass = 'dx-icon-trackingDocuments';

                    switch (alerts.Notifications[i].IdNotificationType) {
                        case 51:
                            cssClass = 'dx-icon-assignedShift';
                            break;
                        case 75:
                            cssClass = 'dx-icon-taTelecommuting_Home';
                            break;
                    }

                    //if (alerts.Notifications[i].IdNotificationType == 75 || alerts.Notifications[i].IdNotificationType == 52) {
                    //    hasAction = false;
                    //}

                    lstAlerts.push({
                        ID: -1,
                        IdRequestType: -1,
                        IdRelatedObject: alerts.Notifications[i].IdNotification,
                        IdNotificationType: alerts.Notifications[i].IdNotificationType,
                        NotificationDate: moment.tz(alerts.Notifications[i].DateTime, VTPortal.roApp.serverTimeZone),
                        cssClass: cssClass,
                        name: alerts.Notifications[i].Name,
                        hasAction: hasAction,
                        type: 'notification',
                        description: alerts.Notifications[i].Description,
                        iDate: null
                    });
                }
            }

            if (typeof alerts.ExpiredDocAlert != 'undefined') {
                if (alerts.ExpiredDocAlert != null) {
                    cssClass = 'dx-icon-expiredDocAlert';
                    lstAlerts.push({
                        ID: -1,
                        IdRequestType: -1,
                        IdRelatedObject: -1,
                        IdNotificationType: -1,
                        NotificationDate: null,
                        cssClass: cssClass,
                        name: (typeof alerts.ExpiredDocAlert[i].AlertSubject != 'undefined' ? alerts.ExpiredDocAlert[i].AlertSubject : i18nextko.i18n.t('roExpiredDocument')),//i18nextko.i18n.t('roExpiredDocument'),
                        hasAction: false,
                        type: 'expired',
                        description: (typeof alerts.ExpiredDocAlert[i].AlertDescription != 'undefined' ? alerts.ExpiredDocAlert[i].AlertDescription : (i18nextko.i18n.t('roExpiredDocuments') + ' ' + moment.tz(alerts.ExpiredDocAlert[i].DateTime, VTPortal.roApp.serverTimeZone).format("DD/MM/YYYY"))),//i18nextko.i18n.t('roExpiredDocuments') + ' ' + moment.tz(alerts.ExpiredDocAlert.DateTime, VTPortal.roApp.serverTimeZone).format("DD/MM/YYYY"),
                        iDate: null
                    });
                }
            }
        }

        alertDS(lstAlerts);
    }

    var viewModel = {
        viewShown: generateAlerts,
        title: i18nextko.t('roAlertsTitle'),
        subscribeBlock: globalStatus(),
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
                        case 'sign':
                            VTPortal.app.navigate("documents");
                            break;
                        case 'notification':
                            new WebServiceRobotics(function (result) {
                                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                                    if (!result.Result) {
                                        VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotificationNotExists"), 'error', 1000);
                                    } else {
                                        $.when(VTPortal.roApp.loadInitialData(false, false, true, false, false)).then(notiAction(info))
                                    }
                                } else {
                                    VTPortalUtils.utils.processErrorMessage(result);
                                }
                            }, function (error) {
                                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roNotificationLoadError"), 'error', 1000);
                            }).setNotificationReaded(info.itemData.IdRelatedObject);
                            break;
                        case 'request':

                            if (info.itemData.IdRequestType != 16) {
                                VTPortalUtils.utils.markRequestAsRead(info.itemData.ID);
                                VTPortalUtils.utils.navigateToRequest(info.itemData);
                            }
                            else {
                                VTPortalUtils.utils.markRequestAsRead(info.itemData.ID);
                                VTPortal.app.navigate("scheduler/1/" + moment().format("YYYY-MM-DD"));
                            }

                            break;
                        case 'incomplete':
                            VTPortal.app.navigate('forgottenPunch/-1/' + info.itemData.iDate.format("YYYY-MM-DD"));
                            break;
                    }
                }
            }
        }
    };

    return viewModel;
};