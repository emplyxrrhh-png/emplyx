VTPortal.communiqueDetail = function (params) {
    var communique = ko.observable();
    var optionsVisible = ko.observable(false);
    var documentsVisible = ko.observable(false);
    var voteText = ko.observable(i18nextko.i18n.t('roVote'));
    var myResponse = ko.observable('');
    var allowExtraResponse = ko.computed(function () {
        myResponse('');
        if (typeof communique() != 'undefined') {
            if (communique().Status.EmployeeCommuniqueStatus[0].Answered == true) {
                if (communique().Status.Communique.AllowChangeResponse == true) {
                    voteText((communique().Status.EmployeeCommuniqueStatus[0].Answer) + ' (' + i18nextko.i18n.t('roChange') + ')');
                    return true;
                }
                else {
                    myResponse(communique().Status.EmployeeCommuniqueStatus[0].Answer);
                    return false;
                }
            }
            else {
                return true;
            }
        }
        else {
            return false;
        }
    });

    var hasDocuments = ko.observable(false);
    var documents = ko.computed(function () {
        if (typeof communique() != 'undefined') {
            if (communique().Status.Communique.Documents.length > 0) {
                hasDocuments(true);
                var ds = []
                communique().Status.Communique.Documents.forEach(function (item, index) {
                    ds.push({ text: item.Title, id: item.Id, type: 'success' });
                });
                return ds;
            }
            else {
                hasDocuments(false);
            }
        }
        else {
            hasDocuments(false);
        }
    });

    var hasResponses = ko.observable(false);
    var responses = ko.computed(function () {
        if (typeof communique() != 'undefined') {
            if (communique().Status.Communique.AllowedResponses.length > 0) {
                hasResponses(true);
                var ds = []
                communique().Status.Communique.AllowedResponses.forEach(function (item, index) {
                    ds.push({ text: item, type: 'success' });
                });
                return ds;
            }
            else {
                hasResponses(false);
            }
        }
        else {
            hasResponses(false);
        }
    });
    var lblSubject = ko.computed(function () {
        if (typeof communique() != 'undefined') {
            return communique().Status.Communique.Subject;
        }
        else {
            '';
        }
    });
    var lblCommuniqueDate = ko.computed(function () {
        if (typeof communique() != 'undefined') {
            return moment(communique().Status.Communique.SentOn).format("dddd DD MMMM YYYY");;
        }
        else {
            '';
        }
    });
    var lblMessage = ko.computed(function () {
        if (typeof communique() != 'undefined') {
            return communique().Status.Communique.Message;
        }
        else {
            '';
        }
    });
    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    var employeeImage = ko.computed(function () {
        if (typeof communique() != 'undefined') {
            var backgroundImage = '';
            if (communique().Status.Communique.CreatedBy.EmployeePhoto != '') {
                backgroundImage = 'url(data:image/png;base64,' + communique().Status.Communique.CreatedBy.EmployeePhoto + ') no-repeat center center';
            }
            else {
                backgroundImage = 'url(Images/logovtl.png) no-repeat center center';
            }

            return backgroundImage;
        }
        else {
            return '';
        }
    });

    var getCommunique = function () {
        new WebServiceRobotics(function (result) {
            if (result.oState.Result == window.VTPortalUtils.constants.OK.value) {
                communique(result);
            } else {
                DevExpress.ui.notify(i18nextko.t('roCommError'), 'warning', 3000);
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
                    VTPortal.roApp.selectedTab(5);
                    VTPortal.app.navigate('communiques', { root: true });
                }
                else {
                    VTPortal.roApp.selectedTab(4);
                    VTPortal.app.navigate('communiques', { root: true });
                }
            }
        }).getCommuniqueById(params.id);
    }

    function viewShown() {
        globalStatus().viewShown();
        getCommunique();
    };

    var viewModel = {
        viewShown: viewShown,
        lblSubject: lblSubject,
        empImage: employeeImage,
        lblCommuniqueDate: lblCommuniqueDate,
        allowExtraResponse: allowExtraResponse,
        hasDocuments: hasDocuments,
        lblMessage: lblMessage,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        title: i18nextko.t('rocommuniquesTitle'),
        subscribeBlock: globalStatus(),
        communique: communique,
        voteOptions: {
            dataSource: responses,
            visible: optionsVisible,
            showTitle: false,
            showCancelButton: ko.observable(true),
            onItemClick: function (e) {
                // var expDate = moment(communique().Status.Communique.ExpirationDate).format('DD/MM/YYYY HH:mm:ss');
                if (moment(communique().Status.Communique.ExpirationDate).isAfter(moment())) {
                    new WebServiceRobotics(function (result) {
                        if (result.oState.Result == window.VTPortalUtils.constants.OK.value) {
                            DevExpress.ui.notify(i18nextko.t('roAnswerCorrect'), 'success', 3000);
                            if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
                                VTPortal.roApp.selectedTab(5);
                                VTPortal.app.navigate('communiques', { root: true });
                            }
                            else {
                                VTPortal.roApp.selectedTab(4);
                                VTPortal.app.navigate('communiques', { root: true });
                            }
                        } else {
                            DevExpress.ui.notify(i18nextko.t('roAnswerIncorrect'), 'warning', 3000);
                            if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
                                VTPortal.roApp.selectedTab(5);
                                VTPortal.app.navigate('communiques', { root: true });
                            }
                            else {
                                VTPortal.roApp.selectedTab(4);
                                VTPortal.app.navigate('communiques', { root: true });
                            }
                        }
                    }).answerCommunique(communique().Status.Communique.Id, e.itemData.text);
                }
                else {
                    DevExpress.ui.notify(i18nextko.t('roExpired'), 'warning', 3000);
                }
            }
        },
        checkDocuments: {
            dataSource: documents,
            visible: documentsVisible,
            showTitle: false,
            showCancelButton: ko.observable(true),
            onItemClick: function (e) {
                new WebServiceRobotics(function (result) {
                    window.VTPortalUtils.utils.downloadBytes(result.Value)
                }).getDocumentBytes(e.itemData.id);
            }
        },
        buttonOptions: {
            text: voteText,
            visible: hasResponses,
            onClick: function (e) {
                optionsVisible(true);
            }
        },
        buttonMyResponse: {
            text: myResponse,
            visible: hasResponses
        },
        buttonDocuments: {
            text: i18nextko.t('roCheckDocuments'),
            visible: hasDocuments,
            onClick: function (e) {
                documentsVisible(true);
            }
        },
        cssBackground: ko.computed(function () {
            return 'backgroundOpacityImp';
        }),
    };

    return viewModel;
};