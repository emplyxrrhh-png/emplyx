VTPortal.communiques = function (params) {
    var communiqueDS = ko.observable(false);
    var activeTab = ko.observable(1);

    var dataSource = ko.observable(new DevExpress.data.DataSource({
        searchOperation: "contains",
        searchExpr: ["Subject", "Message", "SentOn", "CreatedBy"]
    }));

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function generatecommuniquesAll() {
        activeTab(1);
        $("#btnNotRead").removeClass("pressing");
        $("#btnAllCom").addClass("pressing");
        $("#btnArchived").removeClass("pressing");

        var lstcommuniques = [];
        var comunicados = VTPortal.roApp.userCommuniques();
        if (comunicados != null && comunicados.length > 0) {
            var cssClass = '';
            var cssClassText = '';

            for (var i = 0; i < comunicados.length; i++) {
                if (comunicados[i].Communique.Archived == false) {
                    if (comunicados[i].Communique.MandatoryRead == true) {
                        switch (comunicados[i].EmployeeCommuniqueStatus[0].Read) {
                            case false:
                                cssClass = 'dx-icon-communiqueImportant';
                                cssClassText = 'textCommuniqueNotRead';
                                break;
                            case true:
                                cssClass = 'dx-icon-communiqueImportantRead';
                                cssClassText = '';
                                break;
                        }
                    }
                    else {
                        switch (comunicados[i].EmployeeCommuniqueStatus[0].Read) {
                            case false:
                                cssClass = 'dx-icon-communiqueNotRead';
                                cssClassText = 'textCommuniqueNotRead';
                                break;
                            case true:
                                cssClass = 'dx-icon-communiqueRead';
                                cssClassText = '';
                                break;
                        }
                    }

                    name = comunicados[i].Subject;

                    lstcommuniques.push({
                        ID: comunicados[i].Communique.Id,
                        cssClass: cssClass,
                        cssClassText: cssClassText,
                        Message: compressMessage(comunicados[i].Communique.Message.replaceAll("<br>", "")),
                        SentOn: moment(comunicados[i].Communique.SentOn).format("DD/MM/YYYY"),
                        CreatedBy: comunicados[i].Communique.CreatedBy.PassportName,
                        Subject: comunicados[i].Communique.Subject,
                    });
                }
            }
        }
        if (lstcommuniques.length > 0) {
            communiqueDS(true);
        }
        else {
            communiqueDS(false);
        }
        dataSource(new DevExpress.data.DataSource({
            store: lstcommuniques,
            searchOperation: "contains",
            searchExpr: ["Subject", "Message", "SentOn", "CreatedBy"]
        }));
        return lstcommuniques;
    };
    function generatecommuniquesArchived() {
        activeTab(2);
        $("#btnNotRead").removeClass("pressing");
        $("#btnAllCom").removeClass("pressing");
        $("#btnArchived").addClass("pressing");

        var lstcommuniques = [];
        var comunicados = VTPortal.roApp.userCommuniques();
        if (comunicados != null && comunicados.length > 0) {
            var cssClass = '';
            var cssClassText = '';

            for (var i = 0; i < comunicados.length; i++) {
                if (comunicados[i].Communique.Archived == true && comunicados[i].EmployeeCommuniqueStatus[0].Read == true) {
                    if (comunicados[i].Communique.MandatoryRead == true) {
                        switch (comunicados[i].EmployeeCommuniqueStatus[0].Read) {
                            case false:
                                cssClass = 'dx-icon-communiqueImportant';
                                cssClassText = 'textCommuniqueNotRead';
                                break;
                            case true:
                                cssClass = 'dx-icon-communiqueImportantRead';
                                cssClassText = '';
                                break;
                        }
                    }
                    else {
                        switch (comunicados[i].EmployeeCommuniqueStatus[0].Read) {
                            case false:
                                cssClass = 'dx-icon-communiqueNotRead';
                                cssClassText = 'textCommuniqueNotRead';
                                break;
                            case true:
                                cssClass = 'dx-icon-communiqueRead';
                                cssClassText = '';
                                break;
                        }
                    }

                    name = comunicados[i].Subject;

                    lstcommuniques.push({
                        ID: comunicados[i].Communique.Id,
                        cssClass: cssClass,
                        cssClassText: cssClassText,
                        Message: compressMessage(comunicados[i].Communique.Message),
                        SentOn: moment(comunicados[i].Communique.SentOn).format("DD/MM/YYYY"),
                        CreatedBy: comunicados[i].Communique.CreatedBy.PassportName,
                        Subject: comunicados[i].Communique.Subject,
                    });
                }
            }
        }
        if (lstcommuniques.length > 0) {
            communiqueDS(true);
        }
        else {
            communiqueDS(false);
        }
        dataSource(new DevExpress.data.DataSource({
            store: lstcommuniques,
            searchOperation: "contains",
            searchExpr: ["Subject", "Message", "SentOn", "CreatedBy"]
        }));
        return lstcommuniques;
    };
    function generatecommuniquesNotRead() {
        activeTab(0);
        $("#btnNotRead").addClass("pressing");
        $("#btnAllCom").removeClass("pressing");
        $("#btnArchived").removeClass("pressing");

        var lstcommuniques = [];
        var comunicados = VTPortal.roApp.userCommuniques();
        if (comunicados != null && comunicados.length > 0) {
            var cssClass = '';
            var cssClassText = '';

            for (var i = 0; i < comunicados.length; i++) {
                if (comunicados[i].EmployeeCommuniqueStatus[0].Read == false) {
                    if (comunicados[i].Communique.MandatoryRead == true) {
                        switch (comunicados[i].EmployeeCommuniqueStatus[0].Read) {
                            case false:
                                cssClass = 'dx-icon-communiqueImportant';
                                cssClassText = 'textCommuniqueNotRead';
                                break;
                            case true:
                                cssClass = 'dx-icon-communiqueImportantRead';
                                cssClassText = '';
                                break;
                        }
                    }
                    else {
                        switch (comunicados[i].EmployeeCommuniqueStatus[0].Read) {
                            case false:
                                cssClass = 'dx-icon-communiqueNotRead';
                                cssClassText = 'textCommuniqueNotRead';
                                break;
                            case true:
                                cssClass = 'dx-icon-communiqueRead';
                                cssClassText = '';
                                break;
                        }
                    }

                    name = comunicados[i].Subject;

                    lstcommuniques.push({
                        ID: comunicados[i].Communique.Id,
                        cssClass: cssClass,
                        cssClassText: cssClassText,
                        Message: compressMessage(comunicados[i].Communique.Message),
                        SentOn: moment(comunicados[i].Communique.SentOn).format("DD/MM/YYYY"),
                        CreatedBy: comunicados[i].Communique.CreatedBy.PassportName,
                        Subject: comunicados[i].Communique.Subject,
                    });
                }
            }
        }
        if (lstcommuniques.length > 0) {
            communiqueDS(true);
        }
        else {
            communiqueDS(false);
        }
        dataSource(new DevExpress.data.DataSource({
            store: lstcommuniques,
            searchOperation: "contains",
            searchExpr: ["Subject", "Message", "SentOn", "CreatedBy"]
        }));
        return lstcommuniques;
    };

    function compressMessage(message) {
        return message.replace(/<img[^>]*>/g, "");
    }

    function resizeImageListCommuniquees() {
        var i = 0;
        var imgs = $("div.smallTextDescriptionComm img");

        for (i = 0; i < imgs.length; i++) {
            if (imgs[i].width > 50) {
                imgs[i].classList.add("communiqueeImageList");
            }
        }
    };

    function viewShown() {
        VTPortal.roApp.db.settings.markForRefresh(['communiques', "status"]);
        globalStatus().viewShown();
        activeTab(1);

        let onLoadCallback = function (result) {
            if (result.Status == window.VTPortalUtils.constants.OK.value) {

                VTPortal.roApp.db.settings.updateCacheDS('communiques', result.Communiques);
                VTPortal.roApp.userCommuniques(result.Communiques);
                generatecommuniquesAll();
                resizeImageListCommuniquees();

            } else {
                VTPortalUtils.utils.processErrorMessage(result);
            }
        }

        VTPortal.roApp.userCommuniques(VTPortal.roApp.db.settings.getCacheDS('communiques'));
        if (VTPortal.roApp.userCommuniques() == null || window.VTPortalUtils.needToRefresh('communiques')) {
            new WebServiceRobotics(onLoadCallback, function () { }).getEmployeeCommuniques();
        } else {
            onLoadCallback();
        }

    };

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.t('rocommuniquesTitle'),
        subscribeBlock: globalStatus(),
        communiqueDS: communiqueDS,
        loadingPanel: VTPortalUtils.utils.loadingPanelConf(),
        commScroll: {
            height: '70%',
            onPullDown: function (options) {
                VTPortal.roApp.db.settings.markForRefresh(['communiques', "status"]);
                $.when(VTPortal.roApp.refreshEmployeeStatus(false)).then(options.component.release());
                switch (activeTab()) {
                    case 0:
                        generatecommuniquesNotRead();
                        break;
                    case 1:
                        generatecommuniquesAll();
                        break;
                    case 2:
                        generatecommuniquesArchived();
                        break;
                }
            }
        },
        listcommuniques: {
            dataSource: dataSource,
            scrollingEnabled: false,
            grouped: false,
            itemTemplate: 'communiqueItem',
            onItemClick: function (info) {
                VTPortal.app.navigate('communiqueDetail/' + info.itemData.ID);
            },
            onItemRendered: function (e) { setTimeout(resizeImageListCommuniquees, 100); }
        },
        searchOptions: {
            valueChangeEvent: "keyup",
            mode: "search",
            height: "10%",
            onValueChanged: function (args) {
                dataSource().searchValue(args.value);
                dataSource().load();
            }
        },
        notRead: {
            onClick: generatecommuniquesNotRead,
            text: i18nextko.t('roNotRead')
        },

        allComm: {
            onClick: generatecommuniquesAll,
            text: i18nextko.t('roAllComm')
        },

        archived: {
            onClick: generatecommuniquesArchived,
            text: i18nextko.t('roArchived')
        },
    };

    return viewModel;
};