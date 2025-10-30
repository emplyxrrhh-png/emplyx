VTPortal.webLinksHome = function (params) {
    var linksDS = ko.observable([]);

    function GetAllWebLinks(callback) {
        new WebServiceRobotics(function (result) {
            if (result.Status === window.VTPortalUtils.constants.OK.value) {
                VTPortal.roApp.db.settings.updateCacheDS('weblinks', result.Value);
                paintLinks(result.Value)
            } else {
                linksDS([]);
                DevExpress.ui.notify(i18nextko.t('RES_webLinkError'), 'warning', 3000);
            }
        }).GetAllPortalWebLinks();
    }

    function loadLinks() {
        GetAllWebLinks(function (links) {
            linksDS(links);
        });
    }

    function openLink(url) {
        if (url) VTPortal.roApp.openURLonExternalBrowser(url); 
    }

    function paintLinks(links) {
        const portalDashboardLinks = [];

        if (links != null && (links.length == 0 || (links.length > 0 && typeof links[0].ShowOnPortalDashboard != 'undefined'))) {
            let portalLinks = links.filter(function (link) {
                return link.ShowOnPortalDashboard === true;
            });

            for (let i = 0; i < portalLinks.length; i++) {
                portalDashboardLinks.push({
                    title: portalLinks[i].Title,
                    text: portalLinks[i].Description,
                    url: portalLinks[i].URL
                });
            }
        } else {
            VTPortal.roApp.db.settings.markForRefresh(['weblinks']);
        }

        linksDS(portalDashboardLinks);
    }

    var viewShown = function () {

        let tmpDS = VTPortal.roApp.db.settings.getCacheDS('weblinks');
        if (tmpDS == null || window.VTPortalUtils.needToRefresh('weblinks')) {
            loadLinks();
        } else {
            paintLinks(tmpDS);
        }
    };

    var viewModel = {
        linksDS: linksDS,
        openLink: openLink,
        viewShown: viewShown,
    };

    return viewModel;
};