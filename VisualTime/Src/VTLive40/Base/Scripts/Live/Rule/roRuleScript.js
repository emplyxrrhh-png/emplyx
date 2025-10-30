(function () {
    let cRule = {}    
    let cRuleHistoryId = 101
    let isLoadingRule = false;  
    let isProcessingHistoryItemClick = false;    
    let isNewRule = false;
    let newRuleId = 0;

    //Properties
    $(document).ready(async function () {        
        window.Rule_save = async () => Rule_save(cRule);
        window.Rule_close = async () => Rule_close();
        window.Rule_undo = async () => Rule_undo(cRule);
        window.Rule_delete = async () => Rule_delete(cRule);
        window.RuleHistory_save = async () => RuleHistory_save(cRule);
        window.RuleHistory_undo = async () => RuleHistory_undo(cRule);
        window.RuleHistory_delete = async () => RuleHistory_delete(cRule);
        window.parentCloseAndApplySelector = async (currentSelection) => parentCloseAndApplySelector(currentSelection);
        window.ruleHistoryGrid_OnHistoryItemClick = async (e) => ruleHistoryGrid_OnHistoryItemClick(e);
        window.ruleHistoryGrid_OnNewClick = async (e) => ruleHistoryGrid_OnNewClick(e);        
        window.Rule = {
            SetValue: (rule) => { cRule = rule },
            GetValue: () => { return cRule },
            Events: {
                onLoad: async () => renderRuleView()
            }
        }
        window.RuleEditor = {
            loaded: false,
            Events: {
                onLoad: async (s, e) => showRuleEditor(s, e),
                confirmRuleDefinition: (ruleDefinition) => saveRuleDefinition(ruleDefinition),
                cancelRuleDefinition: () => cancelRuleDefinition(),
                getCurrentRuleDefinition: () => getCurrentRuleDefinition()
            }
        }
        // ==========================================
        // SECTION: View management
        // ==========================================
        //#region View management        

        const renderRuleView = async () => {
            isLoadingRule = true;
            if (window.Rule.GetValue() == null) {
                DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roConvertedError", "RulesGroup"));
                return;
            }            
            setEntityChanges('Rule', false);
            sb_hasChanges('Rule', false);            
            setChangesBarTitle('Rule', window.Rule.GetValue().Name);
            setEntityChanges('RuleHistory', false);
            sb_hasChanges('RuleHistory', false);
            setDeleteStatus('Rule', true);
            cRuleHistoryId = window.Rule.GetValue().RuleDefinitions[0].Id; 
            await initializeRuleControls();        
            window.HistoricGrid.init('ruleHistoryGrid', window.Rule.GetValue().RuleDefinitions, getTextFromCatalog(window.RuleGroups.i18n, "roRuleDetail_HistoryTitle", "RulesGroup"), { id: "Id", description: "Description", historyDate: "EffectiveFrom" });            
            isLoadingRule = false;
        }

        const initializeRuleControls = async () => {
            $("#txtName").dxTextBox("instance").option("value", window.Rule.GetValue().Name);
            $("#txtName").dxTextBox("instance").option("onValueChanged", function () {
                if (!isLoadingRule) {
                    const rule = window.Rule.GetValue();
                    rule.Name = $("#txtName").dxTextBox("instance").option("value");
                    window.Rule.SetValue(rule);
                    checkDisabledButtons();
                }
            });

            $("#txtDescription").dxTextArea("instance").option("value", window.Rule.GetValue().Description);
            $("#txtDescription").dxTextArea("instance").option("onValueChanged", function () {
                if (!isLoadingRule) {
                    const rule = window.Rule.GetValue();
                    rule.Description = $("#txtDescription").dxTextArea("instance").option("value");
                    window.Rule.SetValue(rule);
                    checkDisabledButtons();
                }
            });
            $("#lblTypeValue").text(window.Rule.GetValue().TypeDescription);
            const ruleType = window.RulesManager.Data.RuleTypes().find(type => type.Id == window.Rule.GetValue().Type);
            $("#lblTypeDescription").text(ruleType.Name + ". " + ruleType.Description);
            createCmbRulesGroup();
            createTagBoxRulesTags();            
        }

        const initializeRuleHistory = async () => {            

            $("#dpAvailableFrom").dxDateBox({
                type: 'date',
                value: moment(window.Rule.GetValue().RuleDefinitions.find(history => history.Id == cRuleHistoryId).EffectiveFrom).toDate(),
                width: 130,
                displayFormat: getDateLocalizationFormats().format,
                inputAttr: { 'aria-label': 'Date' },
                onValueChanged: function (e) {
                    if (!isLoadingRule) {
                        const dateValue = e.value;
                        const rule = window.Rule.GetValue();
                        rule.RuleDefinitions.find(history => history.Id == cRuleHistoryId).EffectiveFrom = dateToJsonIsoString(dateValue);
                        window.Rule.SetValue(rule);
                        checkDisabledHistoryButtons();
                    }
                },
            });
            $("#txtDefinitionDescription").dxTextBox("instance").option("value", window.Rule.GetValue().RuleDefinitions.find(history => history.Id == cRuleHistoryId).Description);
            $("#txtDefinitionDescription").dxTextBox("instance").option("onValueChanged", function () {
                if (!isLoadingRule) {
                    const rule = window.Rule.GetValue();
                    rule.RuleDefinitions.find(history => history.Id == cRuleHistoryId).Description = $("#txtDefinitionDescription").dxTextBox("instance").option("value");                    
                    window.Rule.SetValue(rule);
                    checkDisabledHistoryButtons();
                }
            });
            createTagBoxRulesShifts();            

            intializeContextSelector();
            await window.HistoricGrid.selectRow("ruleHistoryGrid", cRuleHistoryId);
        }

        const intializeContextSelector = async () => {
            const ruleDefinition = window.Rule.GetValue().RuleDefinitions.find(entry => entry.Id == cRuleHistoryId);
            if (ruleDefinition != null && ruleDefinition.EmployeeContext != null) {
                let currentSelection = {};
                if (ruleDefinition.EmployeeContext != null) {
                    currentSelection.UserFields = ruleDefinition.EmployeeContext.UserFields
                    currentSelection.ComposeFilter = ruleDefinition.EmployeeContext.ComposeFilter;
                    currentSelection.ComposeMode = ruleDefinition.EmployeeContext.ComposeMode;
                    currentSelection.Filter = ruleDefinition.EmployeeContext.Filters;
                    currentSelection.Operation = ruleDefinition.EmployeeContext.Operation;
                }

                $("#txtWhoRuleApply").dxTextBox("instance").option("value", buildSelectedEmployeesString(currentSelection));
            }
            else
                $("#txtWhoRuleApply").dxTextBox("instance").option("value", buildSelectedEmployeesString(''));

            $('#txtWhoRuleApply').off("click");
            $('#txtWhoRuleApply').on("click", function (e) {                                
                parent.showLoader(true);

                let employeeContext = window.Rule.GetValue().RuleDefinitions.find(entry => entry.Id == cRuleHistoryId).EmployeeContext;
                let currentSelectorView = { Employees: '', Groups: '', Filter: '', UserFields: '', ComposeMode: '', ComposeFilter: '', Advanced: false, Operation: '' };                
                if (employeeContext != null)
                    currentSelectorView = { Employees: [], Groups: [], Filter: employeeContext.Filter, UserFields: employeeContext.UserFields, ComposeMode: employeeContext.ComposeMode, ComposeFilter: employeeContext.ComposeFilter, Advanced: false, Operation: employeeContext.Operation };                


                $("#divRuleContextSelector").load('/Employee/EmployeeSelectorPartial?feature=Employees&pageName=rules&config=111&unionType=or&advancedMode=0&advancedFilter=0&allowAll=0&allowNone=0', function () {
                    loadPartialInfo();
                    initUniversalSelector(currentSelectorView, false, 'rulesSelector');
                    parent.showLoader(false);
                });
            });
        }

        const createCmbRulesGroup = async () => {
            $("#cboGroup").dxSelectBox({
                dataSource: window.RulesManager.Data.RuleGroups(),
                valueExpr: "Id",
                displayExpr: "Name",
                acceptCustomValue: true,
                value: window.Rule.GetValue().GroupId, 
                placeholder: getTextFromCatalog(window.RuleGroups.i18n, "lblAddOrSelectGroups", "RulesGroup"),
                onCustomItemCreating: function (e) {
                    const newValue = e.text;
                    const newItem = { Id: newValue, Name: newValue };
                    const dataSource = e.component.getDataSource();
                    dataSource.store().insert(newItem);
                    dataSource.reload();
                    e.customItem = newItem;
                },
                onValueChanged: function (e) {
                    if (!isLoadingRule) {
                        const rule = window.Rule.GetValue();
                        rule.GroupId = e.value;
                        rule.GroupName = e.component.option("displayValue");
                        window.Rule.SetValue(rule);
                        checkDisabledButtons();
                    }
                }
            });


        }

        const createTagBoxRulesTags = async () => {
            $("#tbTags").dxTagBox({
                dataSource: window.RulesManager.Data.Tags(),
                valueExpr: "id",
                displayExpr: "id",
                acceptCustomValue: true,
                searchEnabled: true,
                showSelectionControls: true,
                placeholder: getTextFromCatalog(window.RuleGroups.i18n, "lblAddOrSelectTags", "RulesGroup"),
                value: window.Rule.GetValue().Tags,
                onCustomItemCreating: function (e) {
                    const newValue = e.text;
                    const newItem = { id: newValue };
                    const dataSource = e.component.getDataSource();
                    dataSource.store().insert(newItem);
                    dataSource.reload();
                    e.customItem = newItem;
                },
                onValueChanged: function (e) {
                    if (!isLoadingRule) {
                        if (JSON.stringify(e.previousValue) === JSON.stringify(e.value)) return;
                        const rule = window.Rule.GetValue();
                        rule.Tags = e.value;
                        window.Rule.SetValue(rule);
                        checkDisabledButtons();
                    }
                }
            });
        }

        const createTagBoxRulesShifts = async () => {
                $("#tbShifts").dxTagBox({
                    dataSource: window.RulesManager.Data.Shifts().map(shift => ({
                        Id: shift.Id,
                        Name: shift.Name
                    })),
                    valueExpr: "Id",
                    displayExpr: "Name",                    
                    searchEnabled: true,
                    showSelectionControls: true,
                    placeholder: getTextFromCatalog(window.RuleGroups.i18n, "lblSelectShift", "RulesGroup"), 
                    onValueChanged: function (e) {
                        if (!isLoadingRule) {
                            const rule = window.Rule.GetValue();
                            const shifts = rule.RuleDefinitions.find(entry => entry.Id == cRuleHistoryId).Shifts;
                            const selectedShifts = [];
                            let order = 1;
                            if (shifts != null)
                                order = shifts.length > 0 ? Math.max(...shifts.map(shift => shift.Order)) + 1 : 0;

                            for (let i = 0; i < e.value.length; i++) {
                                const shiftId = e.value[i];
                                const shiftOnDS = window.RulesManager.Data.Shifts().find(shift => shift.Id == shiftId);
                                const shiftGroupOnDS = window.RulesManager.Data.ShiftGroups().find(shiftGroup => shiftGroup.Id == shiftOnDS.GroupId);
                                if (rule.RuleDefinitions.find(entry => entry.Id == cRuleHistoryId).Shifts != null && rule.RuleDefinitions.find(entry => entry.Id == cRuleHistoryId).Shifts.find(shift => shift.IdShift == shiftId) != null) {
                                    selectedShifts.push(rule.RuleDefinitions.find(entry => entry.Id == cRuleHistoryId).Shifts.find(shift => shift.IdShift == shiftId));
                                }
                                else {
                                    const shift = window.RulesManager.Data.Shifts().find(shift => shift.Id == shiftId);
                                    selectedShifts.push({ IdShift: parseInt(shiftId), ShiftName: shiftOnDS.Name, IdShiftGroup: shiftOnDS.GroupId, ShiftGroupName: shiftGroupOnDS.Name, IdRule: rule.Id, Order: order });
                                    order = order + 1;
                                }


                            }
                            rule.RuleDefinitions.find(entry => entry.Id == cRuleHistoryId).Shifts = selectedShifts;                            
                            window.Rule.SetValue(rule);
                            checkDisabledHistoryButtons();
                        }
                    }

                });

            if (typeof window.Rule.GetValue() != 'undefined' && window.Rule.GetValue().RuleDefinitions.find(history => history.Id == cRuleHistoryId).Shifts != null) {
                $("#tbShifts").dxTagBox("instance").option("value", window.Rule.GetValue().RuleDefinitions
                    .find(history => history.Id == cRuleHistoryId)
                    .Shifts.map(shift => shift.IdShift));                
            }
            else
                $("#tbShifts").dxTagBox("instance").option("value", []);
        }

        const parentCloseAndApplySelector = function (currentSelection) {
            const ruleDefinition = window.Rule.GetValue().RuleDefinitions.find(entry => entry.Id == cRuleHistoryId);
            if (ruleDefinition.EmployeeContext != null) {
                ruleDefinition.EmployeeContext.UserFields = currentSelection.UserFields;
                ruleDefinition.EmployeeContext.ComposeFilter = currentSelection.ComposeFilter;
                ruleDefinition.EmployeeContext.ComposeMode = currentSelection.ComposeMode;
                ruleDefinition.EmployeeContext.Filters = currentSelection.Filter;
                ruleDefinition.EmployeeContext.Operation = currentSelection.Operation;
            }
            else {
                ruleDefinition.EmployeeContext = {
                    UserFields: currentSelection.UserFields,
                    ComposeFilter: currentSelection.ComposeFilter,
                    ComposeMode: currentSelection.ComposeMode,
                    Filters: currentSelection.Filter,
                    Operation: currentSelection.Operation
                };
            }

            $("#txtWhoRuleApply").dxTextBox("instance").option("value", buildSelectedEmployeesString(currentSelection));
            checkDisabledHistoryButtons();
        };

        const ruleHistoryGrid_OnHistoryItemClick = async (e) => { 

            if (isProcessingHistoryItemClick) return;

            try {
                isProcessingHistoryItemClick = true;
                if (!sectionHasChanges("RuleHistory") && (!isNewRule || e.selectedRowKeys == newRuleId)) {
                    cRuleHistoryId = e.selectedRowKeys[0];
                    isLoadingRule = true;
                    await initializeRuleHistory();
                    isLoadingRule = false;
                }
                else {
                    if (e.selectedRowKeys[0] != e.currentDeselectedRowKeys[0]) {                                                
                        initializeRuleHistory();
                        DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roPendingChanges", "RulesGroup"), 'error', 2000);                        
                    }
                }                            
            } finally {
                isProcessingHistoryItemClick = false;
            }
                        
        }        

        const ruleHistoryGrid_OnNewClick = async (e) => {
            if (!isNewRule && !sectionHasChanges("RuleHistory")) {
                let newRuleHistoryId = -1;
                const ruleDefinitions = window.Rule.GetValue().RuleDefinitions;

                if (ruleDefinitions != null && ruleDefinitions.length > 0) {

                    const negativeIds = ruleDefinitions
                        .map(definition => definition.Id)
                        .filter(id => id < 0);

                    if (negativeIds.length > 0) {
                        newRuleHistoryId = Math.min(...negativeIds) - 1;
                    }
                }
                cRuleHistoryId = newRuleHistoryId;

                const newRuleDefinition = {
                    Id: cRuleHistoryId,
                    Description: "",
                    EffectiveFrom: `/Date(${new Date().getTime()})/`,
                    EditionStatus: 3,
                    Shifts: [],
                    EmployeeContext: null
                };

                window.Rule.GetValue().RuleDefinitions.push(newRuleDefinition);

                isLoadingRule = true;
                isNewRule = true;
                newRuleId = cRuleHistoryId;
                await refreshHistoricDataGrid();
                await initializeRuleHistory();
                disableNewHistoryBtn();
                isLoadingRule = false;
            }
        }

        const enableNewHistoryBtn = () => {
            try {
                const newHistoryBtn = $(`#newHistoryItem_ruleHistoryGrid`).dxButton("instance");
                if (newHistoryBtn) {
                    newHistoryBtn.option("disabled", false);
                    $(`#newHistoryItem_ruleHistoryGrid`).removeClass("disabled");
                }
            } catch (e) {
                console.error("Error deshabilitando botón:", e);
            }
        }

        const disableNewHistoryBtn = () => {
            try {
                const newHistoryBtn = $(`#newHistoryItem_ruleHistoryGrid`).dxButton("instance");
                if (newHistoryBtn) {
                    newHistoryBtn.option("disabled", true);
                    $(`#newHistoryItem_ruleHistoryGrid`).addClass("disabled");
                }
            } catch (e) {
                console.error("Error deshabilitando botón:", e);
            }
        }

        const refreshHistoricDataGrid = async () => {
            const updatedDataSource = window.Rule.GetValue().RuleDefinitions.filter(el => el.EditionStatus != 2);

            window.HistoricGrid.updateDS("ruleHistoryGrid", updatedDataSource);

            if (updatedDataSource.length <= 1) {
                setDeleteStatus("RuleHistory", false);
            } else {
                setDeleteStatus("RuleHistory", true);
            }
        }
        
        //#endregion


        // ==========================================
        // SECTION: Save changes bar
        // ==========================================
        //#region Save changes bar
        const Rule_save = async (cRule) => {
            if (!sectionHasChanges("RuleHistory") && !isNewRule) {
                let saved = false;
                $("#ruleFrm").dxPopup("instance").hide();

                if (!await window.RulesManager.RuleExists(cRule)) {
                    saved = await window.RulesManager.AddRule(cRule);
                } else {
                    saved = await window.RulesManager.UpdateRule(cRule);
                }

                if (sectionHasChanges("Rule")) sb_hasChanges('RuleMng', true);
                if (saved)
                    DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, 'roSavedRuleOK', "RulesGroup"), 'warning', 2000);
                else
                    DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, 'roErrorSavingRule', "RulesGroup"), 'error', 2000);
                return { Success: true, Data: null, ErrorText: '' };
            }
            else {
                DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, 'roPendingChangesOnHistory', "RulesGroup"), 'error', 2000);
                return { Success: false, Data: null, ErrorText: '' };
            }
            
        }

        const Rule_undo = async (cRule) => {
            sb_hasChanges('Rule', false);
            sb_hasChanges('RuleHistory', false);
            isNewRule = false;
            newRuleId = 0;
            window.RulesManager.ReloadRule(cRule);
            await initializeRuleControls();
            refreshHistoricDataGrid();
            return { Success: true, Data: null, ErrorText: '' };
        }

        const Rule_delete = async (cRule) => {
            const deleted = window.RulesManager.DeleteRule(cRule, window.RulesManager.GetDS());
            if (deleted) { 
                DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, 'roDeletedRuleOK', "RulesGroup"), 'success', 2000);
                window.Rule.SetValue(null);
                sb_hasChanges('Rule', false);
                sb_hasChanges('RuleMng', true);

                $("#ruleFrm").dxPopup("instance").hide();
            }                
            else
                DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, 'roErrorDeletingRule', "RulesGroup"), 'error', 2000);            
            return { Success: true, Data: null, ErrorText: '' };
        }

        const Rule_close = async () => {
            window.Rule.SetValue(null);
            sb_hasChanges('Rule', false);
            sb_hasChanges('RuleHistory', false);
            isNewRule = false;
            newRuleId = 0;

            $("#ruleFrm").dxPopup("instance").hide();
            return { Success: true, Data: null, ErrorText: '' };
        }
        //#endregion

        // ==========================================
        // SECTION: Save changes history bar
        // ==========================================
        //#region Save changes bar
        const RuleHistory_save = async (cRule) => {
            return await $.ajax({
                url: `${BASE_URL}Rule/ValidateRuleHistory`,
                data: window.Rule.GetValue().RuleDefinitions.find(entry => entry.Id == cRuleHistoryId),
                type: "POST",
                dataType: "json",
                success: (data) => {
                    if (data.Success) {
                        setEntityChanges('RuleHistory', false);
                        sb_hasChanges('RuleHistory', false);
                        setEntityChanges('Rule', true);
                        sb_hasChanges('Rule', true);                        
                        window.Rule.GetValue().RuleDefinitions.find(entry => entry.Id == cRuleHistoryId).EditionStatus = 1;
                        refreshHistoricDataGrid();
                        isNewRule = false;
                        newRuleId = 0;
                        enableNewHistoryBtn();
                        return { Success: true, Data: null, ErrorText: '' }; 
                    } else {
                        DevExpress.ui.notify(data.ErrorText);
                    }
                },
                error: (error) => {
                    console.error(error);
                },
            });
        };


        const RuleHistory_undo = async (cRule) => {
            sb_hasChanges('RuleHistory', false);
            window.RulesManager.GetRuleHistoryItem(cRule, cRuleHistoryId);
            isLoadingRule = true;
            await initializeRuleHistory();
            isLoadingRule = false;            
            return { Success: true, Data: null, ErrorText: '' };
        }

        const RuleHistory_delete = async (cRule) => {
            setEntityChanges('RuleHistory', false);
            sb_hasChanges('RuleHistory', false);
            setEntityChanges('Rule', true);
            sb_hasChanges('Rule', true);
            enableNewHistoryBtn();
            isNewRule = false;
            newRuleId = 0;
            window.Rule.GetValue().RuleDefinitions.find(entry => entry.Id == cRuleHistoryId).EditionStatus = 2;
            await refreshHistoricDataGrid();            
            return { Success: true, Data: null, ErrorText: '' }; 
        }
        //#endregion


        // ==========================================
        // SECTION: ASPX rule editor popup
        // ==========================================
        //#region ASPX rule editor popup

        const getCurrentRuleDefinition = () => {
            //let currentRuleDefinition = window.Rule.GetValue().RuleDefinitions.find(entry => entry.Id == cRuleHistoryId);

        }

        const saveRuleDefinition = async (ruleDefinition) => {
        }

        const cancelRuleDefinition = async () => {
            $('#editDailyRuleFrm').dxPopup('instance').hide();
        }

        const showRuleEditor = (s,e) => {
            let iframe = s.component._$popupContent[0].querySelector('#ifDailyRuleEditor');
            let showloadFunc = parent.showLoader;
            

            if (iframe) {
                if (!window.RuleEditor.loaded) {
                    iframe.addEventListener('load', function () {
                        let ruleEditorDocument = iframe;
                        showloadFunc(false);
                        ruleEditorDocument.contentWindow.setGetCurrentRuleDefinition(window.RuleEditor.Events.getCurrentRuleDefinition);
                        ruleEditorDocument.contentWindow.setOnSaveRuleEdition(window.RuleEditor.Events.confirmRuleDefinition);
                        ruleEditorDocument.contentWindow.setOnCancelEdition(window.RuleEditor.Events.cancelRuleDefinition);
                        ruleEditorDocument.contentWindow.onAdvancedRulePopupShown();
                    });
                    window.RuleEditor.loaded = true;
                }
                
            } else {
                console.warn('No se encontró el iframe con nombre ifFrameRuleEditor');
            }
        }

        //#endregion
    });
})();

const checkDisabledButtons = (forceUnmodified = false) => {    
    const unmodified = $("#txtName").dxTextBox("instance").option("value").length <= 0;    

    sb_hasChanges("Rule", !forceUnmodified && !unmodified);
    setChangesBarTitle('Rule', $("#txtName").dxTextBox("instance").option("value"));
}

const checkDisabledHistoryButtons = (forceUnmodified = false) => {
    const unmodified = $("#txtDefinitionDescription").dxTextBox("instance").option("value").length <= 0;

    sb_hasChanges("RuleHistory", !forceUnmodified && !unmodified);    
}




const openDailyRule = () => {
    parent.showLoader(true);
    $('#editDailyRuleFrm').dxPopup('instance').show();
}