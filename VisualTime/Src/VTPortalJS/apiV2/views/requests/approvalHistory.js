VTPortal.approvalHistory = function (params) {
    var requestsDS = ko.observable([]);

    var viewShown = function (approvalsHistory) {
        var items = [];

        for (var i = 0; i < approvalsHistory.length; i++) {
            var cssClass = '';

            switch (approvalsHistory[i].Action) {
                case 0:
                    cssClass = 'dx-icon-requestPending';
                    break;
                case 1:
                    cssClass = 'dx-icon-requestOnGoing';
                    break;
                case 2:
                    cssClass = 'dx-icon-requestApprove';
                    break;
                case 3:
                    cssClass = 'dx-icon-requestDenied';
                    break;
            }

            items.push({
                cssClass: cssClass,
                Name: moment.tz(approvalsHistory[i].ActionDate, VTPortal.roApp.serverTimeZone).format('LLL'),
                Description: approvalsHistory[i].User + ":" + approvalsHistory[i].Comment
            });
        }

        requestsDS(items);
    }

    var viewModel = {
        refreshApprovals: viewShown,
        lblRequestApprovals: i18nextko.t('roLblRequestApprovals'),
        listApprovals: {
            dataSource: requestsDS,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'ApprovalItem',
            onItemClick: function (info) {
                if (info.itemData.hasAction) {
                    VTPortal.app.navigate('userFields/' + info.itemData.Id);
                }
            }
        }
    };

    return viewModel;
};