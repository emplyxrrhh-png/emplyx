VTPortal.communiquesHome = function (params) {
    var count = ko.observable(0);
    var countInquiries = ko.observable(0);
    var countTotal = ko.computed(function () {
        return count() + countInquiries()
    });

    var communiquesDesc = ko.computed(function () {
        count(0);
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined' && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
            for (var i = 0; i < VTPortal.roApp.userStatus().Communiques.length; ++i) {
                if (VTPortal.roApp.userStatus().Communiques[i].EmployeeCommuniqueStatus[0].Read == false && VTPortal.roApp.userStatus().Communiques[i].Communique.Archived == false) {
                    count(count() + 1);
                }
            }

            if (count() == 0) {
                VTPortal.roApp.hasCommuniques(false);
                return i18nextko.i18n.t('nocommuniques');
            }
            else {
                VTPortal.roApp.hasCommuniques(true);
                return i18nextko.i18n.t('newcommuniques') + count() + ' ' + i18nextko.i18n.t('newcommuniques2');
            }
        } else return '';
    });

    var communiquesDescInquiries = ko.computed(function () {
        //var count = 0;
        countInquiries(0);
        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined' && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
            for (var i = 0; i < VTPortal.roApp.userStatus().Communiques.length; ++i) {
                if (VTPortal.roApp.userStatus().Communiques[i].EmployeeCommuniqueStatus[0].Answered == false && VTPortal.roApp.userStatus().Communiques[i].Communique.AllowedResponses.length > 0 && VTPortal.roApp.userStatus().Communiques[i].Communique.Archived == false) {
                    countInquiries(countInquiries() + 1)
                }
            }

            if (countInquiries() == 0) {
                VTPortal.roApp.hasInquiries(false);
                return '';
            }
            else {
                VTPortal.roApp.hasInquiries(true);
                return i18nextko.i18n.t('newcommuniques') + countInquiries() + ' ' + i18nextko.i18n.t('newInquiries2');
            }
        } else return '';
    });

    var communiquesTitle = ko.computed(function () {
        return i18nextko.i18n.t('rocommuniquesTitle');
    });

    function goToCommuniques() {
        if (VTPortal.roApp.db.settings.supervisorPortalEnabled) {
            if (VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.OnBoardings && VTPortal.roApp.isSaas() == true) {
                VTPortal.roApp.selectedTab(6);
            }
            else {
                VTPortal.roApp.selectedTab(5);
            }

            VTPortal.app.navigate('communiques', { root: true });
        }
        else {
            VTPortal.roApp.selectedTab(4);
            VTPortal.app.navigate('communiques', { root: true });
        }
    }
    var viewShown = function (e) {
        //VTPortal.roApp.refreshEmployeeStatus(false);
        communiquesDesc();
        communiquesDescInquiries();
    }
    var viewModel = {
        viewShown: viewShown,
        goToCommuniques: goToCommuniques,
        communiquesTitle: communiquesTitle,
        lblCommuniquesDesc: communiquesDesc,
        lblCommuniquesDescInquiries: communiquesDescInquiries,
        btnGo: {
            onClick: goToCommuniques,
            text: countTotal,
            visible: countTotal,
        },

        iconCommuniques: {
            onClick: goToCommuniques,
            icon: 'comment'
        },
        hasCommuniques: ko.computed(function () {
            if (VTPortal.roApp.hasCommuniques() == true) {
                return 'hasCommuniques';
            }
            else {
                return 'hasCommuniquesNot';
            }
        }),

        hasCommuniquesTitle: ko.computed(function () {
            if (VTPortal.roApp.hasCommuniques() == true) {
                return 'mainTitle';
            }
            else {
                return 'mainTitle';
            }
        }),
    };

    return viewModel;
};