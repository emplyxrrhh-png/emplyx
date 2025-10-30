VTPortal.summaryHome = function (params) {
    "use strict";

    var statusIcon = ko.computed(function () {
        if (!VTPortal.roApp.lastRequestFailed()) return "sandClock";
        else return "noConnection";
    })

    var isSupervisor = ko.computed(function () {
        return VTPortal.roApp.db.settings.onlySupervisor;
    })

    var presenceTextColor = ko.computed(function () {
        if (!VTPortal.roApp.lastRequestFailed()) {
            if (VTPortal.roApp.userStatus() != null) {
                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                    return "green";
                } else {
                    return "red";
                }
            } else return 'red';
        } else {
            return 'red';
        }
    });

    var presenceTime = ko.computed(function () {
        if (!VTPortal.roApp.lastRequestFailed()) {
            if (VTPortal.roApp.userStatus() != null) {
                var sDate = moment(VTPortal.roApp.userStatus().ServerDate);
                var lpDate = moment(VTPortal.roApp.userStatus().LastPunchDate);
                var difference = sDate.diff(lpDate);
                return moment.duration(difference).humanize();
            } else return '';
        }
        else return i18nextko.i18n.t('roWithoutConnection');
    });

    var employeeStatusTime = ko.computed(function () {
        moment(lpDate).format('D MM YYYY, h:mm:ss a');

        if (!VTPortal.roApp.lastRequestFailed()) {
            VTPortal.roApp.isOnline(true);
            if (VTPortal.roApp.userStatus() != null) {
                var sDate = moment();
                var lpDate = moment(VTPortal.roApp.userStatus().LastPunchDate);
                var difference = sDate.diff(lpDate);
                var totalDays = sDate.diff(lpDate, 'days');
                //var differentHumanize = moment.duration(difference).humanize();
                var differentHumanize = moment.utc(difference).format('HH:mm');
                if (VTPortal.roApp.userStatus().PresenceStatus == "Inside") {
                    if (totalDays > 0) {
                        return i18nextko.i18n.t('roFromDays') + i18nextko.i18n.t('roInFrom2') + ' (' + moment(lpDate).format('HH:mm') + 'h)';
                    }
                    else {
                        return differentHumanize + ' ' + i18nextko.i18n.t('roInFrom2') + ' (' + moment(lpDate).format('HH:mm') + 'h)';
                    }
                } else {
                    if (totalDays > 0) {
                        return i18nextko.i18n.t('roFromDays') + i18nextko.i18n.t('roOutFrom2') + ' (' + moment(lpDate).format('HH:mm') + 'h)';
                    }
                    else {
                        return differentHumanize + i18nextko.i18n.t('roOutFrom2') + ' (' + moment(lpDate).format('HH:mm') + 'h)';
                    }
                }
            } else return '';
        }
        else {
            VTPortal.roApp.isOnline(false);
            return i18nextko.i18n.t('roWithoutConnection');
        }
    })
    var serverTime = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            var sDate = moment(VTPortal.roApp.userStatus().ServerDate);
            return sDate.format('LT')
        } else return '';
    });

    var serverDate = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            var sDate = moment(VTPortal.roApp.userStatus().ServerDate);
            return sDate.format('LL')
        } else return '';
    });

    var username = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            return VTPortal.roApp.userStatus().EmployeeName;
        } else return '';
    });

    var productivTask = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            if (VTPortal.roApp.userStatus().TaskTitle.length != 0) {
                if (VTPortal.roApp.userStatus().TaskTitle.length <= 50) {
                    return i18nextko.i18n.t('inTask') + VTPortal.roApp.userStatus().TaskTitle;
                } else {
                    return i18nextko.i18n.t('inTask') + VTPortal.roApp.userStatus().TaskTitle.substr(0, 48) + '...';
                }
            }
            else { return '' };
        } else return '';
    });

    var productivTaskDesc = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            if (VTPortal.roApp.userStatus().TaskDescription != null && VTPortal.roApp.userStatus().TaskDescription != undefined && VTPortal.roApp.userStatus().TaskDescription.length != 0) {
                if (VTPortal.roApp.userStatus().TaskDescription.length <= 50) {
                    return VTPortal.roApp.userStatus().TaskDescription;
                } else {
                    return VTPortal.roApp.userStatus().TaskDescription.substr(0, 48) + '...';
                }
            }
            else { return '' };
        } else return '';
    });

    var statusTitle = ko.computed(function () {
        return i18nextko.i18n.t('statusTitle');
    });

    var costCenter = ko.computed(function () {
        if (VTPortal.roApp.userStatus() != null) {
            if (VTPortal.roApp.userStatus().CostCenterName.length != 0) {
                return i18nextko.i18n.t('inCostCenter') + VTPortal.roApp.userStatus().CostCenterName;
            }
            else { return ''; };
        } else return '';
    });

    var viewModel = {
        myTime: presenceTime,
        statusTitle: statusTitle,
        lblEmployeeStatusTime: employeeStatusTime,
        textColor: presenceTextColor,
        lblUsername: username,
        lblServerTime: serverTime,
        lblServerDate: serverDate,
        lblProductivTask: productivTask,
        lblProductivTaskDesc: productivTaskDesc,
        lblCostCenter: costCenter,
        statusIcon: statusIcon,
        isSupervisor: isSupervisor,
    };

    return viewModel;
};