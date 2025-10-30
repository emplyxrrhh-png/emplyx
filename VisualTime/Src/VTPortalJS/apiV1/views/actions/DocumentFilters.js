VTPortal.DocumentFilters = function (params) {
    var iDate = ko.observable(moment().startOf('year').toDate())
    var eDate = ko.observable(moment().endOf('year').toDate())
    var selectedIndex = ko.observable(0);
    var selectedStatusVal = ko.observable([0, 1, 2]);
    var selectedOrderDirection = ko.observable('DESC');
    var selectedOrderBy = ko.observable('DeliveryDate');

    var availableTabPanels = [];
    availableTabPanels.push({ "ID": 1, "cssClass": "dx-icon-filterParams" });
    availableTabPanels.push({ "ID": 2, "cssClass": "dx-icon-orderParams" });

    var statusDS = [{
        "ID": 0,
        "Name": i18nextko.t('roDocTypeUserField'),
        "ImageSrc": "Images/Documents/doc-EmployeeField.png"
    }, {
        "ID": 1,
        "Name": i18nextko.t('roDocTypeContract'),
        "ImageSrc": "Images/Documents/doc-EmployeeContract.png"
    }, {
        "ID": 2,
        "Name": i18nextko.t('roDocTypeCompany'),
        "ImageSrc": "Images/Documents/doc-Company.png"
    },
    {
        "ID": 3,
        "Name": i18nextko.t('roDocTypeReason'),
        "ImageSrc": "Images/Documents/doc-Reason.png"
    },
    {
        "ID": 4,
        "Name": i18nextko.t('roDocTypeLeaveOrPermission'),
        "ImageSrc": "Images/Documents/doc-LeaveOrPermission.png"
    },
    {
        "ID": 5,
        "Name": i18nextko.t('roDocumentValidation'),
        "ImageSrc": "Images/Documents/doc-LeaveOrPermission.png"
    },
    {
        "ID": 6,
        "Name": i18nextko.t('roDocTypeVisit'),
        "ImageSrc": "Images/Documents/doc-Visit.png"
    },
    {
        "ID": 7,
        "Name": i18nextko.t('rocommuniquesTitle'),
        "ImageSrc": "Images/Documents/doc-Visit.png"
    }
    ];

    var orderDirectionDS = [{
        id: 'ASC',
        name: i18nextko.t('roOrderDirectionASC')
    }, {
        id: 'DESC',
        name: i18nextko.t('roOrderDirectionDESC')
    }];

    var orderByDS = [{
        id: 'DeliveryDate',
        name: i18nextko.t('roDocOrderBy_field1')
    }];

    var globalStatus = ko.observable(VTPortal.bigUserInfo());

    function viewShown() {
        globalStatus().viewShown();
        if (VTPortal.roApp.documentsFilters() != null) {
            var uFilter = VTPortal.roApp.documentsFilters();

            selectedOrderDirection(uFilter.order.direction);
            selectedOrderBy(uFilter.order.by);
            iDate(uFilter.filter.iDate);
            eDate(uFilter.filter.eDate);
            selectedStatusVal(uFilter.filter.status);
        } else {
            iDate(moment().startOf('year').toDate());
            eDate(moment().endOf('year').toDate());
            selectedStatusVal([0, 1, 2, 3, 4, 5, 6, 7]);
            selectedOrderDirection('DESC');
            selectedOrderBy('DeliveryDate');
        }
    }

    function goPage() {
        var objFilter = {
            order: {
                direction: selectedOrderDirection(),
                by: selectedOrderBy()
            },
            filter: {
                iDate: iDate(),
                eDate: eDate(),
                status: selectedStatusVal()
            }
        }

        VTPortal.roApp.documentsFilters(objFilter);
        VTPortal.roApp.db.settings.documentsFilters = JSON.stringify(objFilter).encodeBase64();
        VTPortal.roApp.db.settings.saveParameter('documentsFilters');

        VTPortal.app.back();
    }

    function removeFilter() {
        VTPortal.roApp.documentsFilters(null);
        VTPortal.roApp.db.settings.documentsFilters = '';
        VTPortal.roApp.db.settings.saveParameter('documentsFilters');

        VTPortal.app.back();
    }

    var viewModel = {
        viewShown: viewShown,
        title: i18nextko.t('roDocumentFilterTitle'),
        subscribeBlock: globalStatus(),
        lblInitialDate: i18nextko.t('roRequestInitialDateLbl'),
        lblEndDate: i18nextko.t('roRequestEndDateLbl'),
        lblStatus: i18nextko.t('roDocumentTypeLbl'),
        lblOrderBy: i18nextko.t('roRequestOrderBy'),
        lblOrderDirection: i18nextko.t('roRequestOrderDirection'),
        beginDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: iDate,
            valueChangeEvent: 'focusout'
        },
        endDate: {
            type: "date",
            pickerType: VTPortalUtils.utils.datetimeTypeSelect(),
            value: eDate,
            valueChangeEvent: 'focusout'
        },
        statusTagBox: {
            items: statusDS,
            showSelectionControls: true,
            applyValueMode: "useButtons",
            displayExpr: "Name",
            valueExpr: "ID",
            itemTemplate: "statusItem",
            value: selectedStatusVal
        },
        btnAccept: {
            onClick: goPage,
            text: i18nextko.t('closeConfig'),
        },
        btnRemoveFilter: {
            onClick: removeFilter,
            text: i18nextko.t('roRemoveFilter'),
        },
        selectedTab: selectedIndex,
        tabPanelOptions: {
            dataSource: availableTabPanels,
            selectedIndex: selectedIndex,
            itemTemplate: "title"
        },
        orderDirection: {
            items: orderDirectionDS,
            displayExpr: "name",
            valueExpr: "id",
            value: selectedOrderDirection
        },
        orderFields: {
            items: orderByDS,
            displayExpr: "name",
            valueExpr: "id",
            value: selectedOrderBy
        }
    };

    return viewModel;
};