VTPortal.punchesHome = function (params) {
    var punchesDS = ko.observable([]);
    var selectedDate = ko.observable(new Date());
    var punchesDesc = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            if (VTPortal.roApp.userStatus().TaskTitle.length <= 50) {
                return i18nextko.i18n.t('inTask') + VTPortal.roApp.userStatus().TaskTitle;
            } else {
                return i18nextko.i18n.t('inTask') + VTPortal.roApp.userStatus().TaskTitle.substr(0, 48) + '...';
            }
        } else return '';
    });

    var punchesTitle = ko.computed(function () {
        return i18nextko.i18n.t('punchesTitle');
    });
    var noPunches = ko.computed(function () {
        return i18nextko.i18n.t('noPunches');
    });

    var newPunchDone = ko.computed(function () {
        if (VTPortal.roApp.newPunchDone() == true) {
            getPunches();
        }
    });

    var getPunches = function (e) {
        VTPortal.roApp.newPunchDone(false);
        new WebServiceRobotics(function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.punchInProgress(false);
                var punches = result.Punches;
                for (var i = 0; i < punches.length; i++) {
                    switch (punches[i].Type) {
                        case 1:
                        case 2:
                        case 3:
                        case 7:
                            if (punches[i].TypeData != null && punches[i].TypeData > 0) {
                                if (punches[i].ActualType == 1) {
                                    if (typeof punches[i].InTelecommute != 'undefined' && punches[i].InTelecommute.toLowerCase() == "true") {
                                        punches[i].cssClass = 'dx-icon-TATC_in';
                                        punches[i].Name = i18nextko.i18n.t('roEntry') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                    }
                                    else {
                                        punches[i].cssClass = 'dx-icon-TA_in_cause';
                                        punches[i].Name = i18nextko.i18n.t('roPunches_TA_in_causeHome') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                    }
                                } else {
                                    punches[i].cssClass = 'dx-icon-TA_out_cause';
                                    punches[i].Name = i18nextko.i18n.t('roPunches_TA_out_causeHome') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                }
                            } else {
                                if (punches[i].ActualType == 1) {
                                    if (typeof punches[i].InTelecommute != 'undefined' && punches[i].InTelecommute.toLowerCase() == "true") {
                                        punches[i].cssClass = 'dx-icon-TATC_in';
                                        punches[i].Name = i18nextko.i18n.t('roEntry') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                    }
                                    else {
                                        punches[i].cssClass = 'dx-icon-TA_in';
                                        punches[i].Name = i18nextko.i18n.t('roEntry') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                    }
                                } else {
                                    punches[i].cssClass = 'dx-icon-TA_out';
                                    punches[i].Name = i18nextko.i18n.t('roExit') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                                }
                            }
                            punches[i].hasAction = true;
                            break;
                        case 4:
                            punches[i].cssClass = 'dx-icon-Task_change';
                            punches[i].hasAction = false;
                            punches[i].Name = i18nextko.i18n.t('roPunches_Task_changeHome') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                            break;
                        case 13:
                            punches[i].cssClass = 'dx-icon-CostCenter';
                            punches[i].hasAction = false;
                            punches[i].Name = i18nextko.i18n.t('roPunches_CostCenter') + ': ' + moment.tz(punches[i].DateTime, VTPortal.roApp.serverTimeZone).format('HH:mm');
                            break;
                    }

                    punches[i].Value = punches[i].RequestedTypeData != "" ? i18nextko.i18n.t('roRequestPendingApprove') + punches[i].RequestedTypeData : punches[i].RelatedInfo;
                }
                punchesDS(punches);
                VTPortal.roApp.punchesDS(punches);

                //$('#scrollview').height($('#panelsContent').height() - 70);
            } else {
                punchesDS([]);
                VTPortal.roApp.punchesDS([]);
            }
        }).getMyPunches(selectedDate.Value);
    }
    function goToPunches() {
        if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
            VTPortal.roApp.selectedTab(4);
            VTPortal.app.navigate("punchManagement/1/" + moment().format("YYYY-MM-DD"), { root: true });
        }
        else {
            VTPortal.roApp.selectedTab(3);
            VTPortal.app.navigate("punchManagement/1/" + moment().format("YYYY-MM-DD"), { root: true });
        }
    }

    var viewModel = {
        goToPunches: goToPunches,
        viewShown: getPunches,
        punchesTitle: punchesTitle,
        lblpunchesDesc: punchesDesc,
        punchesDS: punchesDS,
        newPunchDone: newPunchDone,
        noPunches: noPunches,
        listOptions: {
            dataSource: VTPortal.roApp.punchesDS,
            //scrollingEnabled: false,
            //grouped: true,
            //collapsibleGroups: true,
            //groupTemplate: function (data) {
            //    return $("<div>" + i18nextko.i18n.t('roDaily_' + data.key) + "</div>")
            //},
            scrollingEnabled: false,
            grouped: false,
            onItemClick: function (info) {
                goToPunches()
            }
        },
        btnGoPunches: {
            onClick: goToPunches,
            icon: 'spinright'
        },
        removeButton: {
            icon: 'clock',
            onClick: goToPunches
        }
    };

    return viewModel;
};