VTPortal.notificationsHome = function (params) {
    var notificationsCount = ko.observable(0);
    var notificationsDesc = ko.computed(function () {
        notificationsCount(0);
        if (VTPortal.roApp.impersonatedIDEmployee == -1) {
            if (VTPortal.roApp.db.settings.onlySupervisor) {
                notificationsCount(VTPortalUtils.utils.badgeCount(true, false, false));
            } else {
                notificationsCount(VTPortalUtils.utils.badgeCount(false));
                if (VTPortal.roApp.db.settings.supervisorPortalEnabled && VTPortal.roApp.impersonatedIDEmployee == -1) {
                    notificationsCount(VTPortalUtils.utils.badgeCount(false));
                }
            }
        } else {
            notificationsCount(VTPortalUtils.utils.badgeCount(false));
        }

        if (VTPortal.roApp.userStatus() != null && typeof VTPortal.roApp.userStatus() != 'undefined') {
            if (notificationsCount() == 0) {
                VTPortal.roApp.hasNotifications(false);
                return i18nextko.i18n.t('noNotifications');
            }
            else {
                VTPortal.roApp.hasNotifications(true);
                return i18nextko.i18n.t('newNotifications') + notificationsCount() + i18nextko.i18n.t('newNotifications2');
            }
        } else return '';
    });

    var notificationsTitle = ko.computed(function () {
        return i18nextko.i18n.t('notificationsTitle');
    });

    function goTo() {
        window.VTPortalUtils.utils.setActiveTab('alerts');
        VTPortal.app.navigate("alerts");
    }

    function notificationsViewShown() {
    };

    var viewModel = {
        goTo: goTo,
        notificationsTitle: notificationsTitle,
        viewShown: notificationsViewShown,
        lblnotificationsDesc: notificationsDesc,
        btnGo: {
            onClick: goTo,
            text: notificationsCount,
            visible: VTPortal.roApp.hasNotifications,
        },
        iconNotifications: {
            onClick: goTo,
            icon: 'message'
        },
        hasNotifications: ko.computed(function () {
            //if (VTPortal.roApp.hasNotifications() == true) {
            //    return 'hasNotifications';
            //}
            //else {
            return 'hasNotificationsNot';
            //}
        }),

        hasNotificationsTitle: ko.computed(function () {
            //if (VTPortal.roApp.hasNotifications() == true) {
            //    return 'mainTitleNoti';
            //}
            //else {
            return 'mainTitle';
            //}
        }),
    };

    return viewModel;
};