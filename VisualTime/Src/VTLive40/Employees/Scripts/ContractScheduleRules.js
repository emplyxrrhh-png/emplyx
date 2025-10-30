var actualIDContract = '';
var actualIDLabAgree = '';
var telecommutingOptions;
var telecommutingMandatoryDays = [];
var telecommutingOptionalDays = [];
var presenceMandatoryDays = [];
var telecommutingInfoLoaded = false;

//function hasChanges(hasChanges, sender) {
//    if (typeof sender != 'undefined') {
//        sender.cpCustomized = true;
//        $('#' + sender.cpSwitch).dxSwitch({ value: true });
//    }

//}

function frmShowContractScheduleRules_Show(params) {
    try {
        //show te form
        if (params.length == 2 && !isNaN(params[1])) {
            actualIDContract = params[0];
            actualIDLabAgree = parseInt(params[1], 10);
            venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting');
            venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteScheduleRules');
            initScheduleRuleControl('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback', 'false');

            var oParameters = {};
            oParameters.aTab = 1;
            oParameters.ID = actualIDContract;
            oParameters.IDLabAgree = actualIDLabAgree;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "GETCONTRACTSCHEDULERULES";
            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);
            ContractScheduleRulesCallbackClient.PerformCallback(strParameters);
        }
    } catch (e) { showError("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmShowContractScheduleRules_Show", e); }
}

function frmShowContractScheduleRules_Close() {
    try {
        //show te form
        showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules', false);
    } catch (e) { showError("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmShowContractScheduleRules_Close", e); }
}

function frmShowContractScheduleRules_Save() {
    try {
        showLoadingGrid(true);

        var oParameters = {};
        oParameters.aTab = 1;
        oParameters.ID = actualIDContract;
        oParameters.IDLabAgree = actualIDLabAgree;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SAVECONTRACTSCHEDULERULES";

        oParameters.resultClientAction = (txtYearHoursClient.cpCustomized ? "1" : "0") + (txtYearHolidaysClient.cpCustomized ? "1" : "0") + (tbWorkingDaysClient.cpCustomized ? "1" : "0") + (chkCanWorkOnFeastDaysClient.cpCustomized ? "1" : "0") + (chkCanWorkOnNonWorkingDaysClient.cpCustomized ? "1" : "0")

        if (($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_divTelecommutingGeneral').css("display") != 'none') && (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_chkButton').checked == true)) {
            oParameters.Telecommuting = ckTelecommuteYes_client.GetValue();
            if (cmbDaysOrPercentClient.GetValue() == 0)
                oParameters.TelecommutingMaxDays = txtTelecommutingMaxOptional_Client.GetValue();
            else
                oParameters.TelecommutingPercentage = txtTelecommutingMaxOptional_Client.GetValue();
            oParameters.TelecommutingMandatoryDays = array2String(telecommutingMandatoryDays, ",");
            oParameters.PresenceMandatoryDays = array2String(presenceMandatoryDays, ",");
            oParameters.TelecommutingOptionalDays = array2String(telecommutingOptionalDays, ",");
            oParameters.TelecommutingPeriodType = cmbWeekOrMonthClient.GetValue();
            oParameters.TelecommutingAgreementStart = moment(txtCanTelecommuteFromClient.GetValue()).format('YYYY/MM/DD');
            oParameters.TelecommutingAgreementEnd = moment(txtCanTelecommuteToClient.GetValue()).format('YYYY/MM/DD');

            if (moment(txtCanTelecommuteFromClient.GetValue()).format('YYYY/MM/DD') > moment(txtCanTelecommuteToClient.GetValue()).format('YYYY/MM/DD')) {
                showErrorPopup("Error.Title", "error", "Error.ValidationTCDates", "Error.OK", "Error.OKDesc", "");
                showLoadingGrid(false);
            }
            else if ((oParameters.Telecommuting == true) && (!moment(txtCanTelecommuteToClient.GetValue()).isValid() || !moment(txtCanTelecommuteFromClient.GetValue()).isValid())) {
                showErrorPopup("Error.Title", "error", "Error.ValidationTCDates", "Error.OK", "Error.OKDesc", "");
                showLoadingGrid(false);
            }
            else if (cmbDaysOrPercentClient.GetValue() == '0' && cmbWeekOrMonthClient.GetValue() == '0' && parseInt(txtTelecommutingMaxOptional_Client.GetValue()) > 7) {
                showErrorPopup("Error.Title", "error", "Error.ValidationTCDaysWeek", "Error.OK", "Error.OKDesc", "");
                showLoadingGrid(false);
            }
            else {
                if (cmbDaysOrPercentClient.GetValue() == '0' && cmbWeekOrMonthClient.GetValue() == '1' && parseInt(txtTelecommutingMaxOptional_Client.GetValue()) > 31) {
                    showErrorPopup("Error.Title", "error", "Error.ValidationTCDaysMonth", "Error.OK", "Error.OKDesc", "");
                    showLoadingGrid(false);
                }
                else {
                    if (cmbDaysOrPercentClient.GetValue() == '0' && cmbWeekOrMonthClient.GetValue() == '2' && parseInt(txtTelecommutingMaxOptional_Client.GetValue()) > 93) {
                        showErrorPopup("Error.Title", "error", "Error.ValidationTCDaysQuarter", "Error.OK", "Error.OKDesc", "");
                        showLoadingGrid(false);
                    }
                    else {
                        var strParameters = JSON.stringify(oParameters);
                        strParameters = encodeURIComponent(strParameters);
                        ContractScheduleRulesCallbackClient.PerformCallback(strParameters);
                    }
                }
            }
        }
        else {
            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);
            ContractScheduleRulesCallbackClient.PerformCallback(strParameters);
        }
    } catch (e) { showError("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmShowContractScheduleRules_Save", e); }
}

function ContractScheduleRulesCallback_EndCallBack(s, e) {
    showLoadingGrid(false);

    switch (s.cpActionRO) {
        case "GETCONTRACTSCHEDULERULES":
            if (typeof s.cpTelecommutingOptions != 'undefined') {

                telecommutingOptions = JSON.parse(s.cpTelecommutingOptions);
                telecommutingMandatoryDays = [];
                telecommutingOptionalDays = [];
                presenceMandatoryDays = [];
                for (i = 0; i < tbWorkingDaysClient.values.length; i++)
                    if (telecommutingMandatoryDays.indexOf(tbWorkingDaysClient.values[i]) == -1 && telecommutingOptionalDays.indexOf(tbWorkingDaysClient.values[i]) == -1)
                        presenceMandatoryDays.push(tbWorkingDaysClient.values[i]);
            }
            if (typeof s.cpTelecommutingPatternResult != 'undefined') {
                var daysInfo = JSON.parse(s.cpTelecommutingPatternResult)
                if (s.cpTelecommutingMandatoryDays != null && s.cpTelecommutingMandatoryDays != "") {
                    telecommutingMandatoryDays = s.cpTelecommutingMandatoryDays.split(",");
                }
                else {
                    telecommutingMandatoryDays = [];
                }
                if (s.cpTelecommutingOptionalDays != null && s.cpTelecommutingOptionalDays != "") {
                    telecommutingOptionalDays = s.cpTelecommutingOptionalDays.split(",");
                }
                else {
                    telecommutingOptionalDays = [];
                }
                if (s.cpPresenceMandatoryDays != null && s.cpPresenceMandatoryDays != "") {
                    presenceMandatoryDays = s.cpPresenceMandatoryDays.split(",");
                }
                else {
                    presenceMandatoryDays = [];
                    for (i = 0; i < tbWorkingDaysClient.values.length; i++)
                        if (telecommutingMandatoryDays.indexOf(tbWorkingDaysClient.values[i]) == -1 && telecommutingOptionalDays.indexOf(tbWorkingDaysClient.values[i]) == -1)
                            presenceMandatoryDays.push(tbWorkingDaysClient.values[i]);
                }

                LoadTelecommutingPattern(daysInfo, telecommutingOptions);
            }
            else {
                telecommutingOptionalDays = [];
                telecommutingMandatoryDays = [];
                LoadTelecommutingPattern(null);
            }
            showWUF('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules', true);
            $(".divRowDescription").each(function (i, val) {
                $(this).attr('style', 'display:flex;text-align: left;');
            });
            $(".componentForm").each(function (i, val) {
                $(this).attr('style', 'display:flex;text-align: left;');
            });

            $("#switchYearHours").dxSwitch({ width: "60px", value: txtYearHoursClient.cpCustomized, onValueChanged: function (e) { txtYearHoursClient.SetEnabled(e.value); txtForkClient.SetEnabled(e.value); if (e.value) { txtYearHoursClient.Focus(); }; txtYearHoursClient.cpCustomized = e.value; } });
            $("#switchYearHolidays").dxSwitch({ width: "60px", value: txtYearHolidaysClient.cpCustomized, onValueChanged: function (e) { txtYearHolidaysClient.SetEnabled(e.value); if (e.value) { txtYearHolidaysClient.Focus(); }; txtYearHolidaysClient.cpCustomized = e.value; } });
            $("#switchCanWorkOnNonWorkingDays").dxSwitch({ width: "60px", value: chkCanWorkOnNonWorkingDaysClient.cpCustomized, onValueChanged: function (e) { chkCanWorkOnNonWorkingDaysClient.SetEnabled(e.value); $("#switchWorkingDays").dxSwitch("instance").option("value", e.value);  if (e.value) { chkCanWorkOnNonWorkingDaysClient.Focus(); }; chkCanWorkOnNonWorkingDaysClient.cpCustomized = e.value; } });
            $("#switchWorkingDays").dxSwitch({ width: "60px", value: tbWorkingDaysClient.cpCustomized, onValueChanged: function (e) { tbWorkingDaysClient.SetEnabled(e.value); $("#switchCanWorkOnNonWorkingDays").dxSwitch("instance").option("value", e.value); if (e.value) { tbWorkingDaysClient.Focus(); } tbWorkingDaysClient.cpCustomized = e.value; } });
            $("#switchCanWorkOnFeastDays").dxSwitch({ width: "60px", value: chkCanWorkOnFeastDaysClient.cpCustomized, onValueChanged: function (e) { chkCanWorkOnFeastDaysClient.SetEnabled(e.value); if (e.value) { chkCanWorkOnFeastDaysClient.Focus(); }; chkCanWorkOnFeastDaysClient.cpCustomized = e.value; } });

            txtYearHoursClient.SetEnabled(txtYearHoursClient.cpCustomized);
            txtForkClient.SetEnabled(txtYearHoursClient.cpCustomized);
            txtYearHolidaysClient.SetEnabled(txtYearHolidaysClient.cpCustomized);
            tbWorkingDaysClient.SetEnabled(tbWorkingDaysClient.cpCustomized);
            chkCanWorkOnFeastDaysClient.SetEnabled(chkCanWorkOnFeastDaysClient.cpCustomized);
            chkCanWorkOnNonWorkingDaysClient.SetEnabled(chkCanWorkOnNonWorkingDaysClient.cpCustomized);

            if (document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_chkButton').checked == false) {
                ckTelecommuteYes_client.SetEnabled(false);
                ckTelecommuteNo_client.SetEnabled(false);
                txtTelecommutingMaxOptional_Client.SetEnabled(false);
                cmbWeekOrMonthClient.SetEnabled(false);
                cmbDaysOrPercentClient.SetEnabled(false);
                disableTelecommutingPattern();
                txtCanTelecommuteFromClient.SetEnabled(false);
                txtCanTelecommuteToClient.SetEnabled(false);
            }
            else {
                ckTelecommuteYes_client.SetEnabled(true);
                ckTelecommuteNo_client.SetEnabled(true);
                enableTC();
            }

            break;
        case "SAVECONTRACTSCHEDULERULES":
            if (s.cpResultRO == "KO") {
                showErrorPopup("Error.Title", "error", "", s.cpErrorRO.ErrorText, "Error.OK", "Error.OKDesc", "");
            } else {
                frmShowContractScheduleRules_Close();
            }
            break;
    }
}

function GridContractScheduleRules_BeginCallback(e, c) {
}

function GridContractScheduleRules_EndCallback(s, e) {
    //if (s.IsEditing()) {
    //    hasChanges(true);
    //} else {
    //    if (s.cpAction == "ROWINSERTING" || s.cpAction == "ROWUPDATING" || s.cpAction == "ROWDELETE") {
    //        hasChanges(true);
    //    }
    //}
}

function GridContractScheduleRules_OnRowDblClick(s, e) {
    GridContractScheduleRulesClient.GetRowValues(e.visibleIndex, 'Id', loadScheduleRules);
}

function GridContractScheduleRules_FocusedRowChanged(s, e) {
}

function GridContractScheduleRules_CustomButtonClick(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        GridContractScheduleRulesClient.GetRowValues(e.visibleIndex, 'Id', loadScheduleRules);
    }
}

function AddNewContractScheduleRule(s, e) {
    loadScheduleRules(-1);
}

function loadScheduleRules(id) {
    actualScheduleRuleId = id;
    var oParameters = {};
    oParameters.aTab = 1;
    oParameters.ID = actualScheduleRuleId;
    oParameters.IDLabAgree = actualIDContract;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETLABAGREERESCHEDULERULE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    ASPxScheduleRulesCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function dropDownBoxEditorTemplateDay(cellElement, cellInfo) {
    var dayPropertyName;
    var dayIndex;
    if (cellInfo.column.index == 7) {
        dayPropertyName = "Day0";
        dayIndex = 0;
    }
    else {
        dayPropertyName = "Day" + (cellInfo.column.index);
        dayIndex = cellInfo.column.index;
    }
    var dayProperty = cellInfo.data[dayPropertyName];
    return $('<div>').dxSelectBox({
        dataSource: telecommutingOptions,
        displayExpr: 'Name',
        valueExpr: 'ID',
        value: dayProperty,
        onSelectionChanged(selectionChangedArgs) {
            cellInfo.setValue(selectionChangedArgs.selectedItem.ID);
            if (selectionChangedArgs.selectedItem.ID == 1) {
                telecommutingMandatoryDays.push(dayIndex.toString());
                deleteElementFromArray(telecommutingOptionalDays, dayIndex.toString());
                deleteElementFromArray(presenceMandatoryDays, dayIndex.toString());
            }
            else {
                if (selectionChangedArgs.selectedItem.ID == 2) {
                    telecommutingOptionalDays.push(dayIndex.toString())
                    deleteElementFromArray(telecommutingMandatoryDays, dayIndex.toString());
                    deleteElementFromArray(presenceMandatoryDays, dayIndex.toString());
                }
                else {
                    if (typeof presenceMandatoryDays != 'undefined' && presenceMandatoryDays.length > 0 && presenceMandatoryDays.indexOf(dayIndex.toString()) == -1)
                        presenceMandatoryDays.push(dayIndex.toString())
                    deleteElementFromArray(telecommutingMandatoryDays, dayIndex.toString());
                    deleteElementFromArray(telecommutingOptionalDays, dayIndex.toString());
                }
            }
        },
        fieldTemplate(data, container) {
            const result = $(`<div class='custom-item'><img style='width:24px;'src='${data ? data.ImageSrc : ''
                }' /><div class='telOption'></div></div>`);
            result
                .find('.telOption')
                .dxTextBox({
                    value: data && data.Name,
                    readOnly: true
                });
            container.append(result);
        },
        itemTemplate(data) {
            return `<div class='custom-item'><img style="width:16px;margin-right:10px;float:left;" src='${data.ImageSrc}' /><div class='product-name'>${data.Name}</div></div>`;
        },
    });
}

function telecommutingInfoTemplate(element, info) {
    if (telecommutingOptions != null && telecommutingOptions.length > 0) {
        var telecommutingOptionInfo = telecommutingOptions.filter(function (item) { return item.ID === info.text; });
        if (typeof telecommutingOptionInfo != undefined && telecommutingOptionInfo.length > 0) {
            element.append("<div><img style='width:24px;' src='" + telecommutingOptionInfo[0].ImageSrc + "'/></div>").append("<div>" + telecommutingOptionInfo[0].Name + "</div>")
        }
        else
            element.append("<div>" + telecommutingOptionInfo[0].Name + "</div>")
    }
}

function LoadTelecommutingPattern(s) {
    acumValues = [];
    telecommutingInfoLoaded = true;
    if (s != null && s.length > 0) {
        for (var i = 0; i < s.length; i++) {
            acumValues.push({
                Id: s[i].Week,
                Day0: s[i].Day0,
                Day1: s[i].Day1,
                Day2: s[i].Day2,
                Day3: s[i].Day3,
                Day4: s[i].Day4,
                Day5: s[i].Day5,
                Day6: s[i].Day6
            });
        }
    }
    else {
        acumValues.push({
            Id: 1,
            Day0: 0,
            Day1: 0,
            Day2: 0,
            Day3: 0,
            Day4: 0,
            Day5: 0,
            Day6: 0
        });
    }
    gridValues = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_divTelecommutingPatternGrid").dxDataGrid({
        id: "gridContractTelecommtingPattern",
        showColumnLines: false,
        showRowLines: false,
        rowAlternationEnabled: false,
        showBorders: false,
        height: 100,
        headerFilter: { visible: false },
        allowColumnResizing: true,
        filterRow: { visible: false },
        toolbar: { visible: false },
        dataSource: {
            store: {
                type: 'array',
                key: 'Id',
                data: acumValues
            }
        },
        editing: {
            mode: 'batch',
            allowUpdating: true,
            allowAdding: false,
            allowDeleting: false,
            selectTextOnEditStart: true,
            startEditAction: 'click',
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                $links.text("");

                if (isEditing) {
                    $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                } else {
                    $links.filter(".dx-link-edit").addClass("dx-icon-edit");
                    $links.filter(".dx-link-delete").addClass("dx-icon-trash");
                }
            }
            if (e.rowType === "data") {
                e.cellElement.css({ "backgroundColor": "#fff" });
            }
        },
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.css({ height: 30 });
                e.rowElement.css({ "border": "0px !important" });
            }
        },
        customizeColumns: function (columns) {
            for (var i = 1; i < 8; i++) {
                var col = i.toString();
                if (i == 7)
                    col = "0";
                if (tbWorkingDaysClient.values.indexOf(col) == -1)
                    columns[i].visible = false;
            }
        },
        remoteOperations: {
            sorting: false,
            paging: false
        },
        paging: {
            enabled: false
        },
        onToolbarPreparing: function (e) {
            e.toolbarOptions.visible = false;
            var toolbarItems = e.toolbarOptions.items;
            $.each(toolbarItems, function (_, item) {
                if (item.name == "saveButton" || item.name == "revertButton") {
                    item.visible = false;
                }
            });
        },
        columns: [
            { caption: "Id", dataField: "Id", allowEditing: true, allowDeleting: false, visible: false, alignment: "center" },
            { caption: window.parent.Globalize.formatMessage("monday"), dataField: "Day1", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("tuesday"), dataField: "Day2", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("wednesday"), dataField: "Day3", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("thursday"), dataField: "Day4", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("friday"), dataField: "Day5", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("saturday"), dataField: "Day6", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("sunday"), dataField: "Day0", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 }
        ]
    }).dxDataGrid("instance");
}

function updatePatternColumns() {
    for (var i = 1; i <= 7; i++) {
        var col = i.toString();
        if (i == 7)
            col = "0"
        if (tbWorkingDaysClient.values.indexOf(col) == -1) {
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_divTelecommutingPatternGrid").dxDataGrid("columnOption", i, "visible", false);
            if (telecommutingMandatoryDays.indexOf(col) >= 0)
                deleteElementFromArray(telecommutingMandatoryDays, col.toString());
            if (telecommutingOptionalDays.indexOf(col) >= 0)
                deleteElementFromArray(telecommutingOptionalDays, col.toString());
        }
        else
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_divTelecommutingPatternGrid").dxDataGrid("columnOption", i, "visible", true);
    }
}

function unsetTelecommutingPattern() {
    if (telecommutingInfoLoaded) {
        source = [];

        source.push({
            Id: 1,
            Day0: "0",
            Day1: "0",
            Day2: "0",
            Day3: "0",
            Day4: "0",
            Day5: "0",
            Day6: "0"
        });

        var store = {
            store: {
                type: 'array',
                key: 'Id',
                data: source
            }
        };
        var dataGrid = $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_divTelecommutingPatternGrid').dxDataGrid('instance');
        dataGrid.option("disabled", true);
        dataGrid.option("dataSource", store);
        dataGrid.refresh();

        telecommutingMandatoryDays = [];
        telecommutingOptionalDays = [];
    }
}

function setTelecommutingPattern() {
    if (telecommutingInfoLoaded) {
        var dataGrid = $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_divTelecommutingPatternGrid').dxDataGrid('instance');
        dataGrid.option("disabled", false);
        dataGrid.refresh();
    }
}

function disableTelecommutingPattern() {
    if (telecommutingInfoLoaded) {
        var dataGrid = $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_divTelecommutingPatternGrid').dxDataGrid('instance');
        dataGrid.option("disabled", true);
        dataGrid.refresh();
    }
}

function enableTelecommutingPattern() {
    var dataGrid = $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_frmContractScheduleRules_ContractScheduleRulesCallback_optOverwriteTelecommuting_divTelecommutingPatternGrid').dxDataGrid('instance');
    dataGrid.option("disabled", false);
    dataGrid.refresh();
}