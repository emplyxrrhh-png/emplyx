var currentBotView = {};
var hasChanges = false;
var bAddNewBot = false;
var addedRow = {};
var BotRules = [];
var selectedRule = null;
var onTypeChangeEvent = false;
(function () {
    //Properties
    var viewHandler = null;
    $(document).ready(async function () {
        //console.log("Carga script supervisors");
        viewUtilsManager.initAccordions();
        viewUtilsManager.setupCardListFilterButton("Bots");
        viewUtilsManager.print("Bots Module loaded");
        //Initialize ViewHandler
        viewHandler = viewUtilsManager.createViewStateHandler();

        // -----------------------------
        // GET DATA VIEW (ON READY) ----
        //------------------------------
        window.addNewBot = addNewBot;
        window.deleteCurrentBot = validationDeleteBotView;
        window.deleteConfirmed = deleteCurrentBot;
        window.clearForm = clearForm;
        //Set public functions
        window.loadRequest = loadBot;
        window.saveData = saveData;
        window.refreshCardTree = viewUtilsManager.refreshCardTree;
        window.unselectCards = viewUtilsManager.unselectCards;

        //Register events
        document.addEventListener("startStateEvent", (data) => viewHandlerEvent(data), false);
        viewHandler.transition(viewHandler.value, "read");

        //Wait for carview load and
        viewUtilsManager.loadViewOptions("Bots", "read", function () {
            //load default bot maybe?
        }, () => { }, 'LiveBots');
    });

    const loadBot = () => {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'Bots/GetBot',
            dataType: "json",
            data: { idBot: viewUtilsManager.getSelectedCardId() },
            success: function (data) {
                if (typeof data == 'string') {
                    $("#divBotsMainView").html('');
                    DevExpress.ui.notify(data, "error", 5000);
                } else {
                    currentBotView = data
                    $("#divBotsMainView").load('/Bots/GetBotView?idView=' + data.Id + '&idTypeBot=' + data.Type, function () {
                        //CAllback view loaded
                        //Setear campos
                        BotRules = data.BotRules;
                        $("#divBotsWhen").load('/Bots/GetBotTypeDescription?idTypeBot=' + data.Type, function () {
                        })
                        $("#gridRulesBots").dxDataGrid("instance").option("dataSource", BotRules);

                        $("#BotName").dxTextBox("instance").option("value", currentBotView.Name);
                        $("#BotType").dxSelectBox("instance").option("value", currentBotView.Type);
                        $("#BotType").dxSelectBox("instance").option("disabled", true);

                        botHasChanges(false);
                    })
                }
            },
            error: function () {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate(currentBotView), "error", 2000);
                currentBotView = {};
                BotRules = [];
            }
        });
    }

    const addNewBot = () => {
        $("#newBotPopup").dxPopup("instance").show();
    }

    const clearForm = () => {
        $("#BotName").dxTextBox("instance").option("value", null);
        $("#BotType").dxSelectBox("instance").option("value", null);
    }

    const deleteCurrentBot = () => {
        window.loadingRequest = true;
        if (isEmpty(currentBotView)) {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roBotNoViewSelected'), 'error', 2000);
        } else if (currentBotView.Id == 0) {
            //Actualizamos sin hacer DELETE
            currentBotView = {};
            BotRules = [];
            refreshCardTree(-1);
            botHasChanges(false);
            unselectCards();
            $("#divBotsMainView").html('');
            //DevExpress.ui.notify(viewUtilsManager.DXTranslate('roBotNotSaved'), "error", 2000);
            window.loadingRequest = false;
        }
        else {
            try {
                $.ajax({
                    type: "DELETE",
                    url: BASE_URL + 'Bots/DeleteBot',
                    dataType: "json",
                    data: { idBot: viewUtilsManager.getSelectedCardId() },
                    success: function (data) {
                        if (typeof data == 'string') {
                            DevExpress.ui.notify(data, "error", 5000);
                        } else {
                            currentBotView = {};
                            BotRules = [];
                        }
                    },
                    error: function () {
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roBotSaved'), "error", 2000);
                    }
                });
            }
            catch (error) {
                console.error(error);
            } finally {
                //clearForm();
                botHasChanges(false);

                $("#divBotsMainView").html('');
                refreshCardTree(-1);
                window.loadingRequest = false;
            }
        }
    }

    function viewHandlerEvent(eventData) {
        var eventDetails = eventData.detail;

        viewUtilsManager.print(eventDetails.currentState + " state");
        switch (eventDetails.currentState) {
            case "read":
                //Código para limpiar los controles
                //console.log('READDDDDDD');
                break;

            case "create":
                //console.log('CREATEEEEEE');

                break;

            case "update":
                //console.log('UPDATEEEEEEEEEE');
                break;

            case "delete":
                //console.log('DELETEEEEEEEEEEEE');
                break;
        }
    };

    var validationDeleteBotView = function () {
        //must validate that Bot is OK first etc etc
        var buttonsDialog = [];
        buttonsDialog.push(viewUtilsManager.buildButtonDialog('Yes', 'Yes', deleteCurrentBot));
        buttonsDialog.push(viewUtilsManager.buildButtonDialog('No', 'No', function () { }));

        viewUtilsManager.showModalDialog(this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsWarning,
            '',
            'roJsWarning', 'roBotConfirmDelete',
            buttonsDialog);
    };

    const saveData = () => { //new bot for the very first time
        currentBotView.Name = $("#BotName").dxTextBox("instance").option("value");
        currentBotView.Type = $("#BotType").dxSelectBox("instance").option("value");
        currentBotView.BotRules = BotRules;
        if (validateModel(currentBotView)) {
            try {
                $.ajax({
                    type: "POST",
                    url: BASE_URL + 'Bots/CreateOrUpdateBot',
                    dataType: "json",
                    data: { oBotParam: currentBotView },
                    success: function (data) {
                        if (typeof data == 'string') {
                            DevExpress.ui.notify(data, "error", 5000);
                        } else {
                            currentBotView = data;
                            refreshCardTree(data.Id);
                            botHasChanges(false);
                        }
                    },
                    error: function (e) {
                        DevExpress.ui.notify(viewUtilsManager.DXTranslate(currentBotView), "error", 2000);
                        //currentBotView = {};
                        //BotRules = [];
                    }
                });
            } catch (e) {
                console.error(e);
            }
        }
    }
})();

function onTypeChange(e) {
    if (e.value == null) e.value = -1;
    if (!onTypeChangeEvent) {
        var gridValues = $("#gridRulesBots").dxDataGrid("instance");
        if (gridValues) {
            if (gridValues.totalCount() > 0) {
                DevExpress.ui.notify("Debes eliminar todas las reglas para cambiar de tipo de bot.", 'error', 6000);
                onTypeChangeEvent = true;
                e.component.option("value", e.previousValue);
                return false;
            }
        }
        botHasChanges(true);
        $("#divBotsWhen").load('/Bots/GetBotTypeDescription?idTypeBot=' + e.value, function () {
            //CAllback view loaded
            currentBotView.Type = e.value;
        })
    }
    else {
        onTypeChangeEvent = false;
    }

    //Remove Rules that are not from this type
    //deleteRulesNotFromType(e.value);
}

function beforeSend(operation, ajaxSettings) {
}

function refreshRules() {
    $("#gridRulesBots").dxDataGrid("instance").refresh();
}

function RulesRemoved() {
    botHasChanges(true);
}

function onCellPrepared(e) {
    if (e.rowType == "header") {
        e.cellElement.css("text-align", "left");
    }
}

function context_menu(e) {
}

function toolbar_preparing(e) {
}

function onRulePopupShown() {
    try {
        $.ajax({
            type: "POST",
            url: BASE_URL + 'Bots/GetAvailableRulesForView',
            dataType: "json",
            data: { iBotType: currentBotView.Type },
            success: function (data) {
                if (typeof data == 'string') {
                    DevExpress.ui.notify(data, "error", 5000);
                } else {
                    $("#BotCreationRule").dxSelectBox("instance").option("dataSource", data);
                    if (selectedRule) {
                        $("#BotCreationRule").dxSelectBox("instance").option("value", selectedRule?.row?.data?.Type);
                    } else {
                        $("#BotCreationRule").dxSelectBox("instance").option("value", data[0]?.Id);
                    }
                    let selectorInstance = $("#BotsIdTemplate").data("dxSelectBox") || $("#BotsIdTemplateList").data("dxTagBox");

                    if (selectorInstance) {
                        if (selectorInstance.NAME === "dxSelectBox") {
                            selectorInstance.option("value", selectedRule.row.data.IDTemplate);
                        } else if (selectorInstance.NAME === "dxTagBox") {
                            selectorInstance.option("value", selectedRule.row.data.Definition.SourceIds);
                        }
                    } else {
                        $("#ckruleActivated").dxCheckBox("instance").option("value", true);
                    }
                }
            },
            error: function () {
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roBotSaved'), "error", 2000);
            }
        });
    }
    catch (error) {
        console.error(error);
    }
}

function onRulePopupHiding() {
    $("#BotRuleName").dxTextBox("instance").option("value", null)
    $("#BotCreationRule").dxSelectBox("instance").option("value", null)
    $("#newRulePopup").dxPopup("instance").hide();
    $("#ckruleActivated").dxCheckBox("instance").option("value", false);
    if (selectedRule) {
        selectedRule = null;
    }
    //delete all fields that are from custom rule
    let selectorInstance = $("#BotsIdTemplate").data("dxSelectBox") || $("#BotsIdTemplateList").data("dxTagBox");
    emptyField(selectorInstance);
}

function onNewBotPopupShown() {
}

function onNewBotPopUpHidden() {
}

function saveBotPopUp() {
    currentBotView.Id = 0;
    var name = $("#BotNameConfig").dxTextBox("instance").option("value");
    if (!name) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate("roBotWithoutName"), 'error', 2000);
    } else {
        var type = AvailableBotTypes[0].Id;
        //                    $("#divBotsMainView").load('/Bots/GetBotView?idView=' + data.Id + '&idTypeBot=' + data.Type, function () {
        $("#divBotsMainView").load('/Bots/GetBotView?idView=-1&idTypeBot=' + type, function () {
            $("#divBotsWhen").load('/Bots/GetBotTypeDescription?idTypeBot=' + AvailableBotTypes[0].Id, function () {
            })
            BotRules = [];
            $("#gridRulesBots").dxDataGrid("instance").option("dataSource", BotRules);
            botHasChanges(true);
            $("#BotName").dxTextBox("instance").option("value", name);
            $("#BotType").dxSelectBox("instance").option("value", type);
            $("#BotNameConfig").dxTextBox("instance").option("value", null);
            $("#newBotPopup").dxPopup("instance").hide();
        })
    }
}

function cancelBotPopUp() {
    $("#BotNameConfig").dxTextBox("instance").option("value", null);
    $("#newBotPopup").dxPopup("instance").hide();
}

function onBotNameChange() {
    botHasChanges(true);
}

function botHasChanges(bolChanges) {
    try {
        var tagHasChanges = document.querySelector("#saveBar > div > div");
        hasChanges = bolChanges;
        if (bolChanges) {
            tagHasChanges.style.display = '';
        } else {
            tagHasChanges.style.display = 'none';
        }
    } catch (e) { showError("hasChangesEventsScheduler", e); }
}

function isEmpty(obj) {
    for (const prop in obj) {
        if (Object.hasOwn(obj, prop)) {
            return false;
        }
    }

    return true;
}

function validateModel(model) {
    if (model.Id == null) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate("roBotWithoutId"), 'error', 2000);
        return false;
    }
    if (model.Name == null || !model.Name) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate("roBotWithoutName"), 'error', 2000);
        return false;
    }

    if (model.Type == null) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate("roBotWithoutType"), 'error', 2000);
        return false;
    }

    return true;
}

function validateRuleModel(ruleName, ruleType, idTemplate, sourceIds) {
    if (!ruleName) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate("roRuleWithoutName"), 'error', 2000);
        return false;
    }

    if (ruleType == null) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate("roRuleWithoutType"), 'error', 2000);
        return false;
    }

    if (idTemplate == null && sourceIds == null ) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate("roRuleWithoutIdTemplate"), 'error', 2000);
        return false;
    }
    return true;

    var ruleName = $("#BotRuleName").dxTextBox("instance").option("value");
    var ruleType = $("#BotCreationRule").dxSelectBox("instance").option("value");
    var ruleIsActive = $("#ckruleActivated").dxCheckBox("instance").option("value");
    var idTemplate = ($("#BotsIdTemplate").data("dxSelectBox") || $("#BotsIdTemplateList").data("dxTagBox")).option("value");
}

function undoChanges() {
    if (currentBotView.Id == 0) {
        //DevExpress.ui.notify(viewUtilsManager.DXTranslate('roBotNotSaved'), "error", 2000);
        currentBotView = {};
        BotRules = [];
        unselectCards();
        $("#divBotsMainView").html('');
    } else {
        loadRequest(currentBotView.Id);
    }
    //clearForm();
    botHasChanges(false);
}

function addNewRule() {
    $("#newRulePopup").dxPopup("instance").show();
    /*$("#divCreateRuleMainView").load('/Bots/GeRuleView?idView=-1', function () {
    }*/
}

function saveRulePopUp() {
    var ruleName = $("#BotRuleName").dxTextBox("instance").option("value");
    var ruleType = $("#BotCreationRule").dxSelectBox("instance")?.option("value");
    var ruleIsActive = $("#ckruleActivated").dxCheckBox("instance").option("value");
    var idTemplate = $("#BotsIdTemplate").data("dxSelectBox")?.option("value");
    var sourceIds = $("#BotsIdTemplateList").data("dxTagBox")?.option("value");

    if (validateRuleModel(ruleName, ruleType, idTemplate, sourceIds)) {
        if (selectedRule) {
            $("#gridRulesBots").dxDataGrid("instance").getDataSource().store().update(selectedRule.row.key, { Name: ruleName, Type: ruleType, IsActive: ruleIsActive, IDTemplate: idTemplate, Definition: { SourceIds: sourceIds } })
            selectedRule = null;
        } else {
            var newRule = {
                Id: 0, IdBot: currentBotView.Id, Type: ruleType, Name: ruleName, IsActive: ruleIsActive, IDTemplate: idTemplate, Definition: { SourceIds: sourceIds } };
            $("#gridRulesBots").dxDataGrid("instance").getDataSource().store().insert(newRule)
        }
        $("#gridRulesBots").dxDataGrid("instance").refresh();
        botHasChanges(true);
        $("#BotRuleName").dxTextBox("instance").option("value", null)
        $("#BotCreationRule").dxSelectBox("instance").option("value", null)
        //delete custom fields
        $("#BotsIdTemplate").data("dxSelectBox")?.option("value", null)
        $("#BotsIdTemplateList").data("dxTagBox")?.option("value", null)
        $("#newRulePopup").dxPopup("instance").hide();
    }
}

function cancelRulePopUp() {
    $("#newRulePopup").dxPopup("instance").hide();
}

function getWidth() {
    return document.documentElement.clientWidth - 40;
}

function getHeight() {
    return document.documentElement.clientHeight - 40;
}

function onRuleCreationChange(e) {
    if (e.value != null) {
        $("#divCreateRuleMainView").load('/Bots/GetRuleView?idRule=' + e.value, function () {
            let selectorInstance = $("#BotsIdTemplate").data("dxSelectBox") || $("#BotsIdTemplateList").data("dxTagBox")
            let selectorDataSource = selectorInstance.getDataSource();
            selectorDataSource.reload();

            let iArray = selectorDataSource.store()._array;

            selectorInstance.option("dataSource", new DevExpress.data.DataSource({
                store: {
                    type: "array",
                    data: iArray,
                    key: "ID"
                },
                paginate: true,
                pageSize: 50,
                pageLoadMode: "scrollBottom"
            }));

        })
    }
}

function deleteRulesNotFromType(type) {
    var availableRules = ViewBag.AvailableRules;
    var gridValues = $("#gridRulesBots").dxDataGrid("instance");

    var filteredGridValues = gridValues.filter(function (gridItem) {
        return availableRules.some(function (availableItem) {
            return availableItem.id === gridItem.id;
        });
    });

    gridValues.splice(0, gridValues.length, ...filteredGridValues);
}

function GetCurrentBotView() {
    if (currentBotView != null && currentBotView.Id != null)
        return currentBotView.Id;
    else
        return null;
}

function onInitNewRow(args) {
    args.data = addedRow;
    addedRow = null;
}

function onRowRemoving(e) {
    console.log(e);
}

function RuleSelected(selectedItems) {
    if (selectedItems.row != null) {
        $("#newRulePopup").dxPopup("instance").show();
        $("#BotRuleName").dxTextBox("instance").option("value", selectedItems.row.data.Name)
        $("#BotCreationRule").dxSelectBox("instance").option("value", selectedItems.row.data.Type)
        $("#ckruleActivated").dxCheckBox("instance").option("value", selectedItems.row.data.IsActive);
        selectedRule = selectedItems;

    }
}

function AllowModify() {
    return true;
}

function emptyField(fieldSelector) {
    if (fieldSelector) {
        fieldSelector.option("value", null);
    }
}