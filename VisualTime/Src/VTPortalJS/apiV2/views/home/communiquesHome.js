VTPortal.communiquesHome = function (params) {
    var count = ko.observable(0);
    var countInquiries = ko.observable(0);
    var countTotal = ko.computed(function () {
        return count() + countInquiries()
    });

    var communiquesDesc = ko.computed(function () {
        count(0);
        let hasInquiries = false;
        if (VTPortal.roApp.userCommuniques() != null  && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
            for (const element of VTPortal.roApp.userCommuniques()) {
                if (!element.EmployeeCommuniqueStatus[0].Read && !element.Communique.Archived && element.Communique.AllowedResponses.length == 0) {
                    count(count() + 1);
                } else if (!element.EmployeeCommuniqueStatus[0].Answered && element.Communique.AllowedResponses.length > 0 && !element.Communique.Archived ) {
                    hasInquiries = true;
                }
            }

            if (count() == 0) {
                VTPortal.roApp.hasCommuniques(false);
                return hasInquiries ? "" : i18nextko.i18n.t('nocommuniques');
            }
            else {
                VTPortal.roApp.hasCommuniques(true);
                return i18nextko.i18n.t('newcommuniques') + count() + ' ' + i18nextko.i18n.t('newcommuniques2');
            }
        } else return '';
    });

    var communiquesDescInquiries = ko.computed(function () {
        countInquiries(0);
        if (VTPortal.roApp.userCommuniques() != null && VTPortal.roApp.db.settings.ApiVersion >= VTPortalUtils.apiVersion.Communiques) {
            for (const element of VTPortal.roApp.userCommuniques()) {
                if (!element.EmployeeCommuniqueStatus[0].Answered && element.Communique.AllowedResponses.length > 0 && !element.Communique.Archived) {
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
        window.VTPortalUtils.utils.setActiveTab('communiques');
        VTPortal.app.navigate('communiques', { root: true });
    }
    var communiquesViewShown = function (e) {
        let communiqueView = this;
        let onViewShownCallback = function () {
            communiqueView.lblCommuniquesDesc();
            communiqueView.lblCommuniquesDescInquiries();


            if (VTPortal.roApp.userCommuniques() != null && VTPortal.roApp.userCommuniques().length > 0) {
                if (VTPortal.roApp.statusView != null) VTPortal.roApp.statusView.showMandatoryPopups();
            }

        }

        let onLoadCallback = function (callback) {
            return function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {

                    VTPortal.roApp.db.settings.updateCacheDS('communiques', result.Communiques);
                    VTPortal.roApp.userCommuniques(result.Communiques);

                } else {
                    VTPortalUtils.utils.processErrorMessage(result);
                }

                if (typeof (callback == 'function')) callback();
            }
        }

        VTPortal.roApp.userCommuniques(VTPortal.roApp.db.settings.getCacheDS('communiques'));
        if (VTPortal.roApp.userCommuniques() == null || window.VTPortalUtils.needToRefresh('communiques')) {
            new WebServiceRobotics(onLoadCallback(onViewShownCallback), function () { }).getEmployeeCommuniques();
        } else {
            onViewShownCallback();
        }
    }


    var viewModel = {
        viewShown: communiquesViewShown,
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