let flGrid;
let monitor = -1;

function showLoadingGrid(loading) { parent.showLoader(loading); }

function btnRefreshClient_Click() {
    if (txtBeginDateClient.GetDate() > txtEndDateClient.GetDate()) {
        Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
            { text: '', key: 'roJsError' }, { text: '', key: 'roInvalidDatePeriod' },
            { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
            Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
    } else {
        PerformAction();
    }
}

function PerformAction() {
    PerformActionCallbackClient.PerformCallback("PERFORM_ACTION");
}

function PerformActionCallback_CallbackComplete(s, e) {
    if (s.cpAction == "PERFORM_ACTION") {
        AspxLoadingPopup_Client.Show();
        monitor = setInterval(function () {
            PerformActionCallbackClient.PerformCallback("CHECKPROGRESS");
        }, 5000);
    } else if (s.cpAction == "ERROR") {
        clearInterval(monitor);
        AspxLoadingPopup_Client.Hide();
    } else if (s.cpAction == "CHECKPROGRESS") {
        if (s.cpActionResult == "OK") {
            clearInterval(monitor);
            AspxLoadingPopup_Client.Hide();

            let _LCODE_ = s.cpLCode;
            let FLEX_BASE_URL = s.cpFlexBaseUrl;

            let reportConfig = {
                dataSource: {
                    dataSourceType: "json",
                    filename: s.cpAuditResult,
                    mapping: {
                        "2": {
                            filters: false
                        },
                        "4": {
                            type: "date string"
                        },
                        "5": {
                            type: "datetime"
                        }
                    }
                },
                options: {
                    grid: {
                        type: "flat",
                        showTotals: "off",
                        showGrandTotals: "off",
                        showHeaders: false
                    },
                    configuratorButton: false,
                    dateTimePattern: "HH:mm"
                },
                slice: {
                    measures: [
                        { uniqueName: "2", caption: " " },
                        { uniqueName: "3", caption: Globalize.formatMessage('rogenius_caption_action') },
                        { uniqueName: "4", caption: Globalize.formatMessage('rogenius_caption_date') },
                        { uniqueName: "5", caption: Globalize.formatMessage('rogenius_caption_time') },
                        { uniqueName: "9", caption: Globalize.formatMessage('rogenius_caption_user') },
                        { uniqueName: "7", caption: Globalize.formatMessage('rogenius_caption_objectype') },
                        { uniqueName: "8", caption: Globalize.formatMessage('rogenius_caption_objecname') },
                        { uniqueName: "10", caption: Globalize.formatMessage('rogenius_caption_description') },
                        { uniqueName: "11", caption: Globalize.formatMessage('rogenius_caption_client') }
                    ],
                    flatSort: [
                        { uniqueName: "5", sort: "asc", }
                    ]
                },
                formats: [
                    { name: "", thousandsSeparator: "", decimalSeparator: ",", decimalPlaces: 2 }
                ],
                tableSizes: {
                    columns: [
                        { idx: 0, width: 100 },
                        { idx: 1, width: 100 },
                        { idx: 2, width: 100 },
                        { idx: 3, width: 100 },
                        { idx: 4, width: 150 },
                        { idx: 5, width: 200 },
                        { idx: 6, width: 200 },
                        { idx: 7, width: 500 },
                        { idx: 8, width: 200 }
                    ]
                }
            };

            function customCellText(cell, data) {
                if (data.measure != null && data.measure.uniqueName == "2") {
                    switch (cell.text) {
                        case "1":
                            cell.text = "<img src='Images/aConnect.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "2":
                            cell.text = "<img src='Images/aDisconnect.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "3":
                            cell.text = "<img src='Images/aSelect.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "4":
                            cell.text = "<img src='Images/aMultiSelect.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "5":
                            cell.text = "<img src='Images/aUpdate.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "6":
                            cell.text = "<img src='Images/aInsert.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "7":
                            cell.text = "<img src='Images/aDelete.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "8":
                            cell.text = "<img src='Images/aExecuted.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "9":
                            cell.text = "<img src='Images/aConnectFail.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "10":
                            cell.text = "<img src='Images/aBlock.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                        case "11":
                            cell.text = "<img src='Images/aUnblock.gif' alt='' align='middle' style='height: 16px; width: 16px;'>";
                            break;
                    }
                }
            }

            flGrid = new Flexmonster({
                container: "flxAudit",
                componentFolder: FLEX_BASE_URL,
                height: "calc(100vh - 250px)",
                global: {
                    localization: {
                        grid: {
                            blankMember: ""
                        }
                    }
                },
                report: reportConfig,
                customizeCell: customCellText,
                licenseKey: _LCODE_
            });
            $("#divExport").show();
        }
    }
}

function exportToExcel() {
    const params = {
        filename: "audit",
        excelSheetName: "audit",
        useCustomizeCellForData: false
    };

    flGrid.exportTo("excel", params);
}