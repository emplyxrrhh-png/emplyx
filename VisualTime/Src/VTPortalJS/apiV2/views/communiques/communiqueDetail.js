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
        if (typeof communique() != 'undefined' && !VTPortal.roApp.isTimeGate()) {
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
                backgroundImage = 'url("data:image/png;base64,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") center center no-repeat';
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
                var comunicados = VTPortal.roApp.userCommuniques();
                if (comunicados != null && comunicados.length > 0) {
                    const item = comunicados.find(item => item.Communique.Id === result.Status.Communique.Id);
                    if (item) {
                        item.EmployeeCommuniqueStatus[0].Read = true;
                    }
                }
                communique(result);
            } else {
                DevExpress.ui.notify(i18nextko.t('roCommError'), 'warning', 3000);
                window.VTPortalUtils.utils.setActiveTab('communiques');
                VTPortal.app.navigate('communiques', { root: true });
            }
        }).getCommuniqueById(params.id);
    }

    function viewShown() {
        VTPortal.roApp.db.settings.markForRefresh(['communiques', "status"]);
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
                        } else {
                            DevExpress.ui.notify(i18nextko.t('roAnswerIncorrect'), 'warning', 3000);
                        }
                        window.VTPortalUtils.utils.setActiveTab('communiques');
                        VTPortal.app.navigate('communiques', { root: true });
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