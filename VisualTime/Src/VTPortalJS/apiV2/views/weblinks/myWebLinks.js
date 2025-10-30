VTPortal.myWebLinks = function (params) {
    var webLinksDS = ko.observable([]);

    function viewShown() {
        loadWebLinks();
    }

    function loadWebLinks() {
        if (VTPortal.roApp.loggedIn) {
            new WebServiceRobotics(function (result) {
                if (result.Status == window.VTPortalUtils.constants.OK.value) {
                    VTPortal.roApp.db.settings.updateCacheDS('weblinks', result.Value);

                    let links = result.Value;
                    links = links.filter(function (link) {
                        return link.ShowOnPortal === true
                    });

                    links.sort(function (a, b) {
                        return a.Position - b.Position;
                    });

                    for (var i = 0; i < links.length; i++) {
                        links[i]['hasAction'] = true;
                        links[i]['cssClass'] = 'dx-weblink';
                        links[i]['Name'] = links[i].Title;
                        links[i]['Value'] = links[i].Description;
                        links[i]['URL'] = links[i].URL;
                    }
                    webLinksDS(links);
                } else {
                    webLinksDS([]);
                    VTPortalUtils.utils.processErrorMessage(result, function () { });
                }
            }, function (error) {
                webLinksDS([]);
                VTPortalUtils.utils.notifyMesage(i18nextko.i18n.t("roErrorLoadingWebLinks"), 'error', 0, function () { });
            }).GetAllPortalWebLinks();
        }
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.i18n.t('roWebLinks'),
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        webLinksDS: webLinksDS,
        listOptions: {
            dataSource: webLinksDS,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'item',
            onItemClick: function (info) {
                if (info.itemData.URL) {
                    VTPortal.roApp.openURLonExternalBrowser(info.itemData.URL); 
                }
            }
        }
    };

    return viewModel;
};