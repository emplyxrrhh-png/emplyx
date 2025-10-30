VTPortal.myTeamAlerts = function (params) {
    var mtalertDS = ko.observable([]);

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var generateAlerts = function () {
        globalStatus().viewShown();
        var alerts = VTPortal.roApp.userStatus().SupervisorStatus;

        var lstAlerts = [];

        if (alerts != null) {
            var cssClass = '';

            if (typeof alerts.Alerts != 'undefined') {
                for (var i = 0; i < alerts.Alerts.length; i++) {
                    switch (alerts.Alerts[i].AlertType) {
                        case 40:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-request_pending',
                                name: i18nextko.i18n.t('rorequest_pending'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });

                            break;
                        case 41:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-employee_present_with_expired_documents',
                                name: i18nextko.i18n.t('roemployee_present_with_expired_documents'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 19:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Day_With_Unmatched_Time_Record',
                                name: i18nextko.i18n.t('Day_With_Unmatched_Time_Record'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 21:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Non_Justified_Incident',
                                name: i18nextko.i18n.t('roNon_Justified_Incident'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 20:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Day_with_Unreliable_Time_Record',
                                name: i18nextko.i18n.t('roDay_with_Unreliable_Time_Record'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 15:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Employee_Not_Arrived_or_Late',
                                name: i18nextko.i18n.t('roEmployee_Not_Arrived_or_Late'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 43:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-LabAgree_Max_Exceeded',
                                name: i18nextko.i18n.t('roLabAgree_Max_Exceeded'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 44:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-LabAgree_Min_Reached',
                                name: i18nextko.i18n.t('roLabAgree_Min_Reached'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 24:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Task_Close_to_Start',
                                name: i18nextko.i18n.t('roTask_Close_to_Start'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 23:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Task_Close_to_Finish',
                                name: i18nextko.i18n.t('roTask_Close_to_Finish'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 25:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Task_Exceeding_Planned_Time',
                                name: i18nextko.i18n.t('roTask_Exceeding_Planned_Time'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 29:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Task_exceeding_Started_Date',
                                name: i18nextko.i18n.t('roTask_exceeding_Started_Date'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 26:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Task_Exceeding_Finished_Date',
                                name: i18nextko.i18n.t('roTask_Exceeding_Finished_Date'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 30:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Task_With_ALerts',
                                name: i18nextko.i18n.t('roTask_With_ALerts'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 45:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Tasks_Request_complete',
                                name: i18nextko.i18n.t('roTasks_Request_complete'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 54:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Productive_Unit_Under_Coverage',
                                name: i18nextko.i18n.t('roProductive_Unit_Under_Coverage'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 57:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Productive_Unit_Under_Coverage',
                                name: i18nextko.i18n.t('roConsent_PortalAlert'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 58:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Productive_Unit_Under_Coverage',
                                name: i18nextko.i18n.t('roConsent_DesktopAlert'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 59:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Productive_Unit_Under_Coverage',
                                name: i18nextko.i18n.t('roConsent_TerminalAlert'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                        case 60:
                            lstAlerts.push({
                                AlertType: alerts.Alerts[i].AlertType,
                                cssClass: 'dx-icon-Productive_Unit_Under_Coverage',
                                name: i18nextko.i18n.t('roConsent_VisitsAlert'),
                                hasAction: true,
                                type: 'request',
                                description: alerts.Alerts[i].Description
                            });
                            break;
                    }
                }
            }
        }

        if (typeof alerts.DocumentAlerts != 'undefined') {
            cssClass = 'dx-icon-trackingDocuments';

            if (alerts.DocumentAlerts.AbsenteeismDocuments.length > 0) {
                lstAlerts.push({
                    AlertType: -1,
                    cssClass: 'dx-icon-AbsenteeismDocuments',
                    name: i18nextko.i18n.t('roAbsenteeismDocument'),
                    hasAction: true,
                    type: 'request',
                    description: i18nextko.i18n.t('roAlertExists') + alerts.DocumentAlerts.AbsenteeismDocuments.length + i18nextko.i18n.t('roAlertAbsenteeism')
                });
            }
            if (alerts.DocumentAlerts.DocumentsValidation.length > 0) {
                lstAlerts.push({
                    AlertType: -2,
                    cssClass: 'dx-icon-DocumentsValidation',
                    name: i18nextko.i18n.t('roDocumentValidation'),
                    hasAction: true,
                    type: 'request',
                    description: i18nextko.i18n.t('roAlertExists') + alerts.DocumentAlerts.DocumentsValidation.length + i18nextko.i18n.t('roAlertDocumentsValidation')
                });
            }
            if (alerts.DocumentAlerts.GpaAlerts.length > 0) {
                lstAlerts.push({
                    AlertType: -3,
                    cssClass: 'dx-icon-GpaAlerts',
                    name: i18nextko.i18n.t('roGpaDocument'),
                    hasAction: true,
                    type: 'request',
                    description: i18nextko.i18n.t('roAlertExists') + alerts.DocumentAlerts.GpaAlerts.length + i18nextko.i18n.t('roAlertGpaAlerts')
                });
            }
            if (alerts.DocumentAlerts.MandatoryDocuments.length > 0) {
                lstAlerts.push({
                    AlertType: -4,
                    cssClass: 'dx-icon-MandatoryDocuments',
                    name: i18nextko.i18n.t('roMandatoryDocument'),
                    hasAction: true,
                    type: 'request',
                    description: i18nextko.i18n.t('roAlertExists') + alerts.DocumentAlerts.MandatoryDocuments.length + i18nextko.i18n.t('roAlertMandatoryDocuments')
                });
            }
            if (alerts.DocumentAlerts.WorkForecastDocuments.length > 0) {
                lstAlerts.push({
                    AlertType: -5,
                    cssClass: 'dx-icon-WorkForecastDocuments',
                    name: i18nextko.i18n.t('roAccessForecastDocuments'),
                    hasAction: true,
                    type: 'request',
                    description: i18nextko.i18n.t('roAlertExists') + alerts.DocumentAlerts.WorkForecastDocuments.length + i18nextko.i18n.t('roAlertForecastDocuments')
                });
            }
            if (alerts.DocumentAlerts.AccessAuthorizationDocuments.length > 0) {
                lstAlerts.push({
                    AlertType: -6,
                    cssClass: 'dx-icon-AccessAuthorizationDocuments',
                    name: i18nextko.i18n.t('roAccessAuthorizationDocument'),
                    hasAction: true,
                    type: 'request',
                    description: i18nextko.i18n.t('roAlertExists') + alerts.DocumentAlerts.AccessAuthorizationDocuments.length + i18nextko.i18n.t('roAlertAccessAuthorization')
                });
            }
        }

        mtalertDS(lstAlerts);
    }

    var onBtnRefresh = function (e) {
        VTPortal.roApp.refreshEmployeeStatus(false, true);
    }

    var viewModel = {
        viewShown: generateAlerts,
        title: i18nextko.t('roMyTeamAlerts'),
        subscribeBlock: globalStatus(),
        mtalertDS: mtalertDS,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        btnRefresh: {
            onClick: onBtnRefresh,
            text: '',
            icon: "Images/Common/refresh.png",
        },
        listAlerts: {
            dataSource: mtalertDS,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'AlertItem',
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    switch (info.itemData.AlertType) {
                        case 40:
                            VTPortal.app.navigate('myTeamRequests/', { root: true });
                            break;
                        default:
                            VTPortal.app.navigate('myTeamAlertsDetail/' + info.itemData.AlertType);
                            break;
                    }
                }
            }
        }
    };

    return viewModel;
};