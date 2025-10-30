VTPortal.myTeamAlertsDetail = function (params) {
    var mtalertDetailDS = ko.observable(new DevExpress.data.DataSource({
        store: [],
        searchOperation: "contains",
        searchExpr: "name"
    }));

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var generateAlerts = function () {
        globalStatus().viewShown();

        if (VTPortal.roApp.userAlerts() != null && typeof VTPortal.roApp.userAlerts() != 'undefined' && VTPortal.roApp.loggedIn == true) {
            var alerts = VTPortal.roApp.userAlerts().SupervisorStatus;

            var lstAlerts = [];

            if (alerts != null) {
                var cssClass = '';
                var hasAction = false;
                switch (parseInt(params.id, 10)) {
                    case -1:

                        for (var i = 0; i < alerts.DocumentAlerts.AbsenteeismDocuments.length; i++) {
                            lstAlerts.push({
                                ID: -1,
                                IdDocumentTemplate: alerts.DocumentAlerts.AbsenteeismDocuments[i].IDDocumentTemplate,
                                IdCause: alerts.DocumentAlerts.AbsenteeismDocuments[i].IdCause,
                                NotificationDate: moment.tz(alerts.DocumentAlerts.AbsenteeismDocuments[i].DateTime, VTPortal.roApp.serverTimeZone),
                                cssClass: 'dx-icon-AbsenteeismDocuments',
                                name: i18nextko.i18n.t('roAbsenteeismDocument'),
                                hasAction: hasAction,
                                type: 'tracking',
                                description: alerts.DocumentAlerts.AbsenteeismDocuments[i].Description
                            });
                        }

                        break;
                    case -2:

                        for (var i = 0; i < alerts.DocumentAlerts.DocumentsValidation.length; i++) {
                            lstAlerts.push({
                                ID: -1,
                                IdDocumentTemplate: alerts.DocumentAlerts.DocumentsValidation[i].IDDocumentTemplate,
                                IdCause: alerts.DocumentAlerts.DocumentsValidation[i].IdCause,
                                NotificationDate: moment.tz(alerts.DocumentAlerts.DocumentsValidation[i].DateTime, VTPortal.roApp.serverTimeZone),
                                cssClass: 'dx-icon-DocumentsValidation',
                                name: i18nextko.i18n.t('roDocumentValidation'),
                                hasAction: hasAction,
                                type: 'tracking',
                                description: alerts.DocumentAlerts.DocumentsValidation[i].Description
                            });
                        }
                        break;
                    case -3:

                        for (var i = 0; i < alerts.DocumentAlerts.GpaAlerts.length; i++) {
                            lstAlerts.push({
                                ID: -1,
                                IdDocumentTemplate: alerts.DocumentAlerts.GpaAlerts[i].IDDocumentTemplate,
                                IdCause: alerts.DocumentAlerts.GpaAlerts[i].IdCause,
                                NotificationDate: moment.tz(alerts.DocumentAlerts.GpaAlerts[i].DateTime, VTPortal.roApp.serverTimeZone),
                                cssClass: 'dx-icon-GpaAlerts',
                                name: i18nextko.i18n.t('roGpaDocument'),
                                hasAction: hasAction,
                                type: 'tracking',
                                description: alerts.DocumentAlerts.GpaAlerts[i].Description
                            });
                        }
                        break;
                    case -4:

                        for (var i = 0; i < alerts.DocumentAlerts.MandatoryDocuments.length; i++) {
                            lstAlerts.push({
                                ID: -1,
                                IdDocumentTemplate: alerts.DocumentAlerts.MandatoryDocuments[i].IDDocumentTemplate,
                                IdCause: alerts.DocumentAlerts.MandatoryDocuments[i].IdCause,
                                NotificationDate: moment.tz(alerts.DocumentAlerts.MandatoryDocuments[i].DateTime, VTPortal.roApp.serverTimeZone),
                                cssClass: 'dx-icon-MandatoryDocuments',
                                name: i18nextko.i18n.t('roMandatoryDocument'),
                                hasAction: hasAction,
                                type: 'tracking',
                                description: alerts.DocumentAlerts.MandatoryDocuments[i].Description
                            });
                        }
                        break;
                    case -5:
                        for (var i = 0; i < alerts.DocumentAlerts.WorkForecastDocuments.length; i++) {
                            lstAlerts.push({
                                ID: -1,
                                IdDocumentTemplate: alerts.DocumentAlerts.WorkForecastDocuments[i].IDDocumentTemplate,
                                IdCause: alerts.DocumentAlerts.WorkForecastDocuments[i].IdCause,
                                NotificationDate: moment.tz(alerts.DocumentAlerts.WorkForecastDocuments[i].DateTime, VTPortal.roApp.serverTimeZone),
                                cssClass: 'dx-icon-WorkForecastDocuments',
                                name: i18nextko.i18n.t('roAccessForecastDocuments'),
                                hasAction: hasAction,
                                type: 'tracking',
                                description: alerts.DocumentAlerts.WorkForecastDocuments[i].Description
                            });
                        }

                        break;
                    case -6:
                        for (var i = 0; i < alerts.DocumentAlerts.AccessAuthorizationDocuments.length; i++) {
                            lstAlerts.push({
                                ID: -1,
                                IdDocumentTemplate: alerts.DocumentAlerts.AccessAuthorizationDocuments[i].IDDocumentTemplate,
                                IdCause: alerts.DocumentAlerts.AccessAuthorizationDocuments[i].IdCause,
                                NotificationDate: moment.tz(alerts.DocumentAlerts.AccessAuthorizationDocuments[i].DateTime, VTPortal.roApp.serverTimeZone),
                                cssClass: 'dx-icon-AccessAuthorizationDocuments',
                                name: i18nextko.i18n.t('roAccessAuthorizationDocument'),
                                hasAction: hasAction,
                                type: 'tracking',
                                description: alerts.DocumentAlerts.AccessAuthorizationDocuments[i].Description
                            });
                        }
                        break;
                    default:

                        new WebServiceRobotics(function (result) {
                            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                                for (var i = 0; i < result.Alerts.length; i++) {
                                    var cssClass = "";
                                    switch (parseInt(params.id, 10)) {
                                        case 40:
                                            cssClass = 'dx-icon-request_pending';
                                            break;
                                        case 41:
                                            cssClass = 'dx-icon-employee_present_with_expired_documents';
                                            break;
                                        case 19:
                                            cssClass = 'dx-icon-Day_with_Unreliable_Time_Record';
                                            break;
                                        case 21:
                                            cssClass = 'dx-icon-Non_Justified_Incident';
                                            break;
                                        case 20:
                                            cssClass = 'dx-icon-Day_with_Unreliable_Time_Record';
                                            break;
                                        case 15:
                                            cssClass = 'dx-icon-Employee_Not_Arrived_or_Late';
                                            break;
                                        case 43:
                                            cssClass = 'dx-icon-LabAgree_Max_Exceeded';
                                            break;
                                        case 44:
                                            cssClass = 'dx-icon-LabAgree_Min_Reached';
                                            break;
                                        case 24:
                                            cssClass = 'dx-icon-Task_Close_to_Start';
                                            break;
                                        case 23:
                                            cssClass = 'dx-icon-Task_Close_to_Finish';
                                            break;
                                        case 25:
                                            cssClass = 'dx-icon-Task_Exceeding_Planned_Time';
                                            break;
                                        case 29:
                                            cssClass = 'dx-icon-Task_exceeding_Started_Date';
                                            break;
                                        case 26:
                                            cssClass = 'dx-icon-Task_Exceeding_Finished_Date';
                                            break;
                                        case 30:
                                            cssClass = 'dx-icon-Task_With_ALerts';
                                            break;
                                        case 45:
                                            cssClass = 'dx-icon-Tasks_Request_complete';
                                            break;
                                        case 54:
                                            cssClass = 'dx-icon-Productive_Unit_Under_Coverage';
                                            break;
                                        case 57:
                                            cssClass = 'dx-icon-Productive_Unit_Under_Coverage';
                                            break;
                                        case 58:
                                            cssClass = 'dx-icon-Productive_Unit_Under_Coverage';
                                            break;
                                        case 59:
                                            cssClass = 'dx-icon-Productive_Unit_Under_Coverage';
                                            break;
                                        case 60:
                                            cssClass = 'dx-icon-Productive_Unit_Under_Coverage';
                                            break;
                                    }
                                    lstAlerts.push({
                                        ID: result.Alerts[i].Id,
                                        name: result.Alerts[i].Name,
                                        cssClass: cssClass,
                                        description: result.Alerts[i].Description
                                    });
                                }
                                mtalertDetailDS(new DevExpress.data.DataSource({
                                    store: lstAlerts,
                                    searchOperation: "contains",
                                    searchExpr: "name"
                                }));
                            } else {
                                VTPortalUtils.utils.processErrorMessage(result);
                            }
                        }).getSupervisorAlertsDetail(parseInt(params.id, 10));

                        break;
                };

                mtalertDetailDS(new DevExpress.data.DataSource({
                    store: lstAlerts,
                    searchOperation: "contains",
                    searchExpr: "name"
                }));

                setTimeout(function () {
                    $('#scrollview').height($('#panelsContent').height() - 50);
                }, 100);
            }
        }
    }

    var viewModel = {
        viewShown: generateAlerts,
        title: i18nextko.t('roMyTeamAlerts'),
        subscribeBlock: globalStatus(),
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        listAlerts: {
            dataSource: mtalertDetailDS,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'AlertItem',
        },
        searchAlerts: {
            valueChangeEvent: "keyup",
            mode: "search",
            height: "10%",
            onValueChanged: function (args) {
                mtalertDetailDS().searchValue(args.value);
                mtalertDetailDS().load();
            }
        },
    };

    return viewModel;
};