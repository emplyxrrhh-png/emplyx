
(function () {
    //Properties
    let viewHandler = null;
    let rulesGroupDS = null;
    let serverTagsDS = [];
    let ruleTypesDS = [];
    let shiftsDS = [];
    let shiftGroupDS = [];
    let lastServerResponse = null;

    $(document).ready(async function () {
        viewUtilsManager.initAccordions();
        viewUtilsManager.setupCardListFilterButton("RulesGroup");
        viewUtilsManager.print("RulesGroup Module loaded");

        //Initialize ViewHandler
        viewHandler = viewUtilsManager.createViewStateHandler();
        
        window.RuleMng_undo = () => RuleMng_undo();
        window.RuleMng_save = () => RuleMng_save();
        window.RulesManager = {
            AddRule : async (cRule) => AddNewRule(cRule, rulesGroupDS),
            UpdateRule: async (cRule) => UpdateRule(cRule, rulesGroupDS),
            DeleteRule: async (cRule) => DeleteRule(cRule, rulesGroupDS),
            ReloadRule: async (cRule) => ReloadRule(cRule, rulesGroupDS),
            RuleExists: async (cRule) => RuleExists(cRule, rulesGroupDS),
            GetRuleHistoryItem: async (cRule, historyId) => LoadRuleHistoryItem(cRule, rulesGroupDS, historyId),
            GetDS: () => rulesGroupDS,
            SetDS: (ds) => rulesGroupDS = ds,
            Server: {
                lastServerResponse: lastServerResponse,
                validateHistoryItem: async (ruleHistoryItem) => validateItem(ruleHistoryItem),
                save: async (scenarioChanges) => saveCurrentScenario(rulesGroupDS, scenarioChanges)
            },
            Events: {
                onLoad: async () => rulesGroup_PageLoad(),
                refresh: async () => refreshView(),
                edit: (rule) => editRule(rule),
                order: (shift) => orderShiftRules(shift)
            },
            Toolbar: {
                Search: () => searchRulesGroupDashboard(),
                New: () => addNewRule()
            },
            Data: {
                Tags: () => getAvailableTags(),
                RuleGroups: () => getAvailableRuleGroups(),
                Shifts: () => shiftsDS,
                RuleTypes: () => ruleTypesDS,
                ShiftGroups: () => shiftGroupDS
            }
        }

        // ==========================================
        // SECTION: Data source management
        // ==========================================
        //#region Data source management

        const addRuleTagsToSet = (rule, tagSet) => {
            if (Array.isArray(rule.Tags)) {
                rule.Tags.forEach(tag => tagSet.add(tag));
            }
        }

        const addGroupTagsToSet = (group, tagSet) => {
            if (Array.isArray(group.Rules)) {
                group.Rules.forEach(rule => addRuleTagsToSet(rule, tagSet));
            }
        }

        const getAvailableTags = () => {
            const uniqueTags = new Set();

            window.RulesManager.GetDS().forEach(group => addGroupTagsToSet(group, uniqueTags));

            serverTagsDS.forEach(tag => uniqueTags.add(tag.id));


            return Array.from(uniqueTags).map(tag => ({ id: tag })).sort((a, b) => {
                if (a.id < b.id) return -1;
                if (a.id > b.id) return 1;
            });
        }

        const getAvailableRuleGroups = () => {
            // Usar un Set para mantener grupos únicos (basados en ID)
            const uniqueGroupsMap = new Map();

            // Recorrer el array original y extraer los grupos
            window.RulesManager.GetDS().forEach(group => {
                uniqueGroupsMap.set(group.Id, {
                    Id: group.Id,
                    Name: group.Name
                });
            });

            // Convertir el Map a un array de objetos
            return Array.from(uniqueGroupsMap.values());
        }
        //#endregion

        // ==========================================
        // SECTION: View management
        // ==========================================
        //#region View management
        document.addEventListener("startStateEvent", (data) => viewHandlerEvent(data), false);
        viewHandler.transition(viewHandler.value, "read");
        viewUtilsManager.loadViewOptions("RulesGroup", "read", function () {
            window.RulesManager.Events.onLoad();
        }, () => { }, 'LiveRules');

        const viewHandlerEvent = (eventData) => { };

        const rulesGroup_PageLoad = async () => {
            setEntityChanges('RuleMng', false);
            setEntityChanges('Rule', false);

            sb_hasChanges('Rule', false);
            sb_hasChanges('RuleMng', false);

            setDeleteStatus('Rule', false);
            setDeleteStatus('RuleMng', false);

            await loadServerTags();
            await loadServerRuleTypes();
            await loadServerShifts();
            await loadServerShiftGroups();
            await loadRulesGroupDashboard();
            buildFilterRow();
        }

        const buildFilterRow = () => {
            $("#searchRulesGroupBar").html('');
            const filterContainer = $("<div>").attr("id", "filterToolbar").appendTo("#searchRulesGroupBar");
            $("<div>").attr("id", "newActionPopover").appendTo("#searchRulesGroupBar");

            // Crear el toolbar con DevExtreme
            filterContainer.dxToolbar({
                items: [
                    {
                        widget: "dxSelectBox",
                        location: "before",
                        options: {
                            elementAttr: { id: "dashboardViewType" },
                            placeholder: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_View", "RulesGroup"),
                            stylingMode: "outlined",
                            width: 200,
                            items: [
                                { id: 1, name: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_Rule", "RulesGroup") },
                                { id: 2, name: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_Shift", "RulesGroup") }
                            ],
                            valueExpr: "id",
                            displayExpr: "name",
                            value: 1,
                            onValueChanged: () => window.RulesManager.Events.refresh()
                        }
                    }, {
                        widget: "dxTextBox",
                        location: "before",
                        options: {
                            elementAttr: { id: "dashboardTextFilter" },
                            placeholder: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_WriteToSearch", "RulesGroup"),
                            stylingMode: "text",
                            width: 420,
                            value: null,
                            onValueChanged: () => window.RulesManager.Events.refresh()
                        }
                    },
                    {
                        widget: "dxDateBox",
                        location: "before",
                        options: {
                            elementAttr: { id: "dashboardDateFilter" },
                            placeholder: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_SelectDate", "RulesGroup"),
                            stylingMode: "outlined",
                            width: 180,
                            type: "date",
                            displayFormat: getDateLocalizationFormats().format,
                            value: new Date(),
                            showClearButton: true,
                            onValueChanged: (e) => window.RulesManager.Events.refresh()
                        }
                    },
                    {
                        widget: "dxTagBox",
                        location: "after",
                        options: {
                            elementAttr: { id: "dashboardShiftFilter" },
                            width: 200,
                            value: null,
                            multiline: false,
                            showMultiTagOnly: true,
                            maxDisplayedTags: 1,
                            hideSelectedItems: false,
                            placeholder: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_Shifts", "RulesGroup"),
                            valueExpr: "Id",
                            displayExpr: "Name",
                            searchEnabled: true,
                            searchExpr: "Name",
                            searchMode: "contains",
                            showClearButton: true,
                            items: window.RulesManager.Data.Shifts(),
                            onValueChanged: () => window.RulesManager.Events.refresh()
                        }
                    },
                    {
                        widget: "dxTagBox",
                        location: "after",
                        options: {
                            elementAttr: { id: "dashboardRuleTypeFilter" },
                            width: 200,
                            value: null,
                            multiline: false,
                            showMultiTagOnly: true,
                            maxDisplayedTags: 1,
                            hideSelectedItems: false,
                            placeholder: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_Types", "RulesGroup"),
                            valueExpr: "Id",
                            displayExpr: "Name",
                            searchEnabled: true,
                            searchExpr: "Name",
                            searchMode: "contains",
                            showClearButton: true,
                            items: window.RulesManager.Data.RuleTypes(),
                            onValueChanged: () => window.RulesManager.Events.refresh()
                        }
                    },
                    {
                        widget: "dxTagBox",
                        location: "after",
                        options: {
                            elementAttr: { id: "dashboardTagFilter" },
                            width: 200,
                            value: null,
                            multiline: false,
                            showMultiTagOnly: true,
                            maxDisplayedTags: 1,
                            hideSelectedItems: false,
                            placeholder: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_Tags", "RulesGroup"),
                            valueExpr: "id", 
                            displayExpr: "id",
                            searchEnabled: true,
                            searchExpr: "id", 
                            searchMode: "contains",
                            showClearButton: true,
                            items: window.RulesManager.Data.Tags(),
                            onValueChanged: () => window.RulesManager.Events.refresh()
                        }
                    },
                    {
                        widget: "dxButton",
                        location: "after",
                        options: {
                            visible: false,
                            icon: 'search',
                            onClick: () => window.RulesManager.Toolbar.Search()
                        }
                    },
                    {
                        widget: "dxButton",
                        location: "after",
                        options: {
                            elementAttr: { id: "btn_createNewRule" },
                            icon: 'add',
                            onClick: () => window.RulesManager.Toolbar.New()
                        }
                    }
                ]
            });
        }

        const loadServerRuleTypes = async () => {
            await $.ajax({
                url: `${BASE_URL}Tag/GetCustomTagBox?type=rulettypes`,
                data: {},
                type: "GET",
                dataType: "json",
                success: (data) => {
                    if (typeof data === "object" && data != null) {
                        ruleTypesDS = data;
                    } else {
                        DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_UnknownError", "RulesGroup"), 'error', 2000);
                    }

                },
                error: (error) => console.error(error),
            });
        }

        const loadServerShiftGroups = async () => {
            await $.ajax({
                url: `${BASE_URL}Tag/GetCustomTagBox?type=shiftgroups`,
                data: {},
                type: "GET",
                dataType: "json",
                success: (data) => {
                    if (typeof data === "object" && data != null) {
                        shiftGroupDS = data;
                    } else {
                        DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_UnknownError", "RulesGroup"), 'error', 2000);
                    }
                },
                error: (error) => console.error(error),
            });
        }

        const loadServerShifts = async () => {
            await $.ajax({
                url: `${BASE_URL}Tag/GetCustomTagBox?type=shifts`,
                data: {},
                type: "GET",
                dataType: "json",
                success: (data) => {
                    if (typeof data === "object" && data != null) {
                        shiftsDS = data;
                    } else {
                        DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_UnknownError", "RulesGroup"), 'error', 2000);
                    }
                },
                error: (error) => console.error(error),
            });
        }

        const loadServerTags = async () => {
            await $.ajax({
                url: `${BASE_URL}Tag/GetAvailableTags`,
                data: { },
                type: "GET",
                dataType: "json",
                success: (data) => {
                    if (typeof data === "object" && data != null) {
                        serverTagsDS = data;
                    } else {
                        DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_UnknownError", "RulesGroup"), 'error', 2000);
                    }
                },
                error: (error) => console.error(error),
            });
        }

        const loadRulesGroupDashboard = async () => {

            let responseCallback = (ruleGroupsData) => {
                window.RulesManager.SetDS(ruleGroupsData);
                window.RulesManager.Events.refresh();
            }

            await $.ajax({
                url: `${BASE_URL}RulesGroup/GetDashboard`,
                data: { filter: {} },
                type: "POST",
                dataType: "json",
                success: (data) => {
                    if (typeof data === "object" && data != null) {
                        if (data.Success) responseCallback(data.Data);
                        else DevExpress.ui.notify(data.ErrorText, 'error', 2000);
                    } else {
                        DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_UnknownError", "RulesGroup"), 'error', 2000);
                    }

                },
                error: (error) => console.error(error),
            });

        }
        //#endregion

        // ==========================================
        // SECTION: Current Rules DS management
        // ==========================================
        //#region Current Rules DS management
        const getNewGroupId = (rulesDS) => {
            if (!rulesDS || rulesDS.length === 0) return -1;

            // Usar reduce para encontrar el ID mínimo
            let minID = rulesDS.reduce((minId, group) => {
                return (group.Id < minId) ? group.Id : minId;
            }, rulesDS[0].Id);

            if (minID > 0) minID = 0;

            return minID - 1;
        }

        const getRulesFromGroup = (group) => {
            if (!group.Rules || !Array.isArray(group.Rules)) {
                return [];
            }
            return group.Rules;
        }

        const getNewRuleId = (rulesDS) => {
            if (!rulesDS || rulesDS.length == 0) return -1;

            let smallestId = Infinity;

            const allRules = [];
            rulesDS.forEach(group => {
                const groupRules = getRulesFromGroup(group);
                allRules.push(...groupRules);
            });

            if (allRules.length === 0) return -1;
            smallestId = Math.min(...allRules.map(rule => rule.Id));

            if (smallestId > 0) smallestId = 0;

            return smallestId - 1;
        }

        const AddNewRule = async (cRule, rulesDS) => {            
            if (!cRule || !rulesDS) return false;
            
            const group = rulesDS.find(g => g.Id === cRule.GroupId);

            if (group != null) {
                group.Rules.push(cRule);
            } else {
                const newGroup = {
                    Id: getNewGroupId(rulesDS),
                    Name: cRule.GroupName || getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_NewGroup", "RulesGroup"),
                    Rules: [cRule],
                    EditionStatus: 3
                };
                rulesDS.push(newGroup);
            }

            refreshTagFilterDS();
            window.RulesManager.Events.refresh();

            return true;
        };

        const RuleExists = async (cRule, rulesDS) => {
            if (!cRule || !rulesDS) return false;

            for (const group of rulesDS) {
                const ruleIndex = group.Rules.findIndex(rule => rule.Id === cRule.Id);
                if (ruleIndex !== -1) return true;
            }

            return false;
        };

        const UpdateRule = async (cRule, rulesDS) => {    
            if (!cRule || !rulesDS) return false;

            let ruleUpdated = false;

            for (const group of rulesDS) {                

                const ruleIndex = group.Rules.findIndex(rule => rule.Id === cRule.Id);
                
                if (ruleIndex !== -1) {
                    ruleUpdated = true;
                    if (cRule.EditionStatus !== 3) cRule.EditionStatus = 1;

                    if (group.Id === cRule.GroupId) {
                        group.Rules[ruleIndex] = { ...group.Rules[ruleIndex], ...cRule };
                    } else {
                        group.Rules.splice(ruleIndex, 1);
                        const newGroup = rulesDS.find(g => g.Id === cRule.GroupId);
                        if (newGroup) {
                            newGroup.Rules.push(cRule);
                        } else {
                            const newGroup = {
                                Id: getNewGroupId(rulesDS),
                                Name: cRule.GroupName || getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_NewGroup", "RulesGroup"),
                                Rules: [cRule],
                                EditionStatus: 3
                            };
                            rulesDS.push(newGroup);
                        }
                    }
                    break;
                } 
            }    


            refreshTagFilterDS();
            window.RulesManager.Events.refresh();               
            return ruleUpdated;
        };

        const DeleteRule = async (cRule, rulesDS) => {
            if (!cRule || !rulesDS) return false;

            let ruleDeleted = false;

            for (const group of rulesDS) {
                const ruleIndex = group.Rules.findIndex(rule => rule.Id === cRule.Id);

                if (ruleIndex !== -1) {
                    cRule.EditionStatus = 2;
                    group.Rules[ruleIndex] = { ...group.Rules[ruleIndex], ...cRule };

                    ruleDeleted = true;
                    break;
                }
            }

            refreshTagFilterDS();
            window.RulesManager.Events.refresh();

            return ruleDeleted;
        }

        const ReloadRule = async (cRule, rulesDS) => {
            let gID = cRule.Id;

            let tmpRule = findRuleById(rulesDS, gID);

            if (tmpRule != null) {                
                window.Rule.SetValue(tmpRule);                
            }
        }

        const LoadRuleHistoryItem = async (cRule, rulesGroupDS, historyId) => {
            let gID = cRule.Id;

            let tmpRule = findRuleById(rulesGroupDS, gID);

            if (tmpRule != null) {

                const index = tmpRule.RuleDefinitions.findIndex(entry => entry.Id === historyId);
                if (index !== -1) {
                    cRule.RuleDefinitions[index] = { ...tmpRule.RuleDefinitions[index] };
                    window.Rule.SetValue(cRule);
                }
                else {
                    cRule.RuleDefinitions.find(entry => entry.Id == historyId).Description = "";
                    cRule.RuleDefinitions.find(entry => entry.Id == historyId).EffectiveFrom = dateToJsonIsoString(new Date().getTime());
                    cRule.RuleDefinitions.find(entry => entry.Id == historyId).Shifts = [];
                    cRule.RuleDefinitions.find(entry => entry.Id == historyId).EmployeeContext = null;

                    window.Rule.SetValue(cRule);
                }
            }
                            
        }

        //#endregion

        // ==========================================
        // SECTION: Filter DS
        // ==========================================
        //#region Filter DS
        const filterRule = (rule, filter) => {
            // Verify rule is not deleted
            if (typeof rule.EditionStatus !== 'undefined' && rule.EditionStatus === 2) return false;

            // Verificar match por keyword
            if (!matchesKeyword(rule, filter.keyword)) return false;

            // Verificar match por tipo
            if (!matchesType(rule, filter.types)) return false;

            // Verificar match por tags
            if (!matchesTags(rule, filter.tags)) return false;

            // Filtrar las entradas históricas
            rule.RuleDefinitions = filterRuleDefinitions(rule.RuleDefinitions, filter);

            // La regla es válida solo si tiene al menos una entrada histórica que cumpla con los filtros
            return rule.RuleDefinitions.length > 0;
        }

        const matchesKeyword = (rule, keyword) =>{
            if (!keyword || keyword.trim() === '') return true;
            if (!rule) return false;

            const normalizedKeyword = keyword.toLowerCase().trim();
            const nameMatches = rule.Name ? rule.Name.toLowerCase().includes(normalizedKeyword) : false;
            const descriptionMatches = rule.Description ? rule.Description.toLowerCase().includes(normalizedKeyword) : false;

            return nameMatches || descriptionMatches;
        }

        const matchesType =(rule, types) => {
            return !types || types.length === 0 || types.includes(rule.Type);
        }

        const matchesTags = (rule, tags) => {
            return !tags || tags.length === 0 ||
                rule.Tags.some(cTag => tags.includes(cTag));
        }

        const filterRuleDefinitions = (entries, filter) => {
            return entries.filter(entry => {
                // Verificar rango de fechas
                if (!isDateInRange(entry, filter.date)) return false;

                // Verificar coincidencia de turnos
                return matchesShifts(entry, filter.shifts);
            });
        }

        const isDateInRange = (entry, date) => {
            const effectiveFrom = moment(entry.EffectiveFrom).toDate();
            const effectiveUntil = entry.EffectiveUntil ?
                moment(entry.EffectiveUntil).toDate() :
                new Date(9999, 11, 31);

            return date >= effectiveFrom && date <= effectiveUntil;
        }

        const matchesShifts = (entry, shifts) => {
            return !shifts || shifts.length === 0 ||
                entry.Shifts.some(shift => shifts.includes(shift.IdShift));
        }

        const filterRulesDS = (rulesDS, filter) => {
            if (!filter || !rulesDS) return rulesDS;

            for (let rulegroup of rulesDS) {
                rulegroup.Rules = rulegroup.Rules.filter(rule => filterRule(rule, filter));
            }

            return rulesDS.filter(group => group.Rules.length > 0);
        }
        //#endregion

        // ==========================================
        // SECTION: Dashboard management
        // ==========================================
        //#region Dashboard management
        const buildSearchFilter = () => {
            // Objeto para almacenar todos los filtros
            const filters = {};

            // Obtener valor del SelectBox (primer elemento)
            const selectBox = $("#dashboardViewType").dxSelectBox("instance");
            if (selectBox) {
                filters.selectedOption = selectBox.option("value");
            } else {
                filters.selectedOption = 1;
            }

            // Obtener valor del SelectBox (primer elemento)
            const dateBox = $("#dashboardDateFilter").dxDateBox("instance");
            if (dateBox) {
                filters.date = dateBox.option("value");
            } else {
                filters.date = new Date();
            }

            // Obtener valor del TextBox (segundo elemento)
            const textBox = $("#dashboardTextFilter").dxTextBox("instance");
            if (textBox) {
                filters.keyword = textBox.option("value");
            } else {
                filters.keyword = "";
            }

            // Shifts TagBox (primero)
            const shiftsTagBox = $("#dashboardShiftFilter").dxTagBox("instance");
            if (shiftsTagBox) {
                filters.shifts = shiftsTagBox.option("value");
            } else {
                filters.shifts = [];
            }

            // Types TagBox (segundo)
            const typesTagBox = $("#dashboardRuleTypeFilter").dxTagBox("instance");
            if (typesTagBox) {
                filters.types = typesTagBox.option("value");
            } else {
                filters.types = [];
            }

            // Tags TagBox (tercero)
            const tagsTagBox = $("#dashboardTagFilter").dxTagBox("instance");
            if (tagsTagBox) {
                filters.tags = tagsTagBox.option("value");
            } else {
                filters.tags = [];
            }

            return filters;
        }

        const refreshTagFilterDS = () => {
            const tagsTagBox = $("#dashboardTagFilter").dxTagBox("instance");
            tagsTagBox.option("items", window.RulesManager.Data.Tags());
            tagsTagBox.repaint();
        }

        const findRuleById = (dataset, ruleId) =>  {
            // Recorrer cada grupo
            for (const group of dataset) {
                // Buscar la regla por ID dentro de las reglas del grupo
                const foundRule = group.Rules.find(rule => rule.Id === ruleId);

                // Si se encuentra la regla, devolverla
                if (foundRule) {
                    return structuredClone(foundRule);
                }
            }

            // Si no se encuentra ninguna regla con ese ID, devolver null
            return null;
        }

        const buildRulesDS = () => {
            if (parseInt(buildSearchFilter().selectedOption, 10) == 1) return buildRuleGroupsDS();
            else return buildShiftGroupsDS();
        }

        const filterDS = () => {
            let filter = buildSearchFilter();
            let srcData = structuredClone(window.RulesManager.GetDS());

            return filterRulesDS(srcData,filter);
        }

        const createRuleItem = (rule, groupName) => {
            return {
                id: rule.Id,
                groupName: groupName,
                ruleName: rule.Name,
                type: rule.Type,
                tags: rule.Tags
            };
        }

        const createShiftRuleItem = (rule, shift) => {
            return {
                id: rule.Id,
                name: rule.Name,
                description: rule.Description,
                type: rule.Type,
                tags: rule.Tags,
                shift: shift.IdShift,
                groupName: shift.IdShiftGroup,
                order: shift.Order
            };
        }

        const processGroup = (group, resultArray) => {
            if (!group.Rules || !Array.isArray(group.Rules)) return;

            group.Rules.forEach(rule => {
                resultArray.push(createRuleItem(rule, group.Name));
            });
        }

        const processShifts = (rule, historyEntry, resultArray) => {
            if (!historyEntry.Shifts || !Array.isArray(historyEntry.Shifts)) return;

            historyEntry.Shifts.forEach(shift => {
                resultArray.push(createShiftRuleItem(rule, shift));
            });
        }

        const processRule = (rule, resultArray) => {
            if (!rule.RuleDefinitions || !Array.isArray(rule.RuleDefinitions)) return;

            // Tomar la primera entrada de historial (según la condición original que siempre devuelve true)
            const activeHistoryEntry = rule.RuleDefinitions[0];
            if (activeHistoryEntry) {
                processShifts(rule, activeHistoryEntry, resultArray);
            }
        }

        const processRuleGroup = (group, resultArray) => {
            if (!group.Rules || !Array.isArray(group.Rules)) return;

            group.Rules.forEach(rule => {
                processRule(rule, resultArray);
            });
        }

        const buildRuleGroupsDS = () => {
            const result = [];

            filterDS().forEach(group => processGroup(group, result));

            return result;
        }

        const buildShiftGroupsDS = () => {
            let newDS = filterDS();

            // Array para almacenar los resultados
            const transformedData = [];

            // Recorrer cada grupo
            newDS.forEach(group => {
                processRuleGroup(group, transformedData);
            });

            // Ordenar el array por ShiftGroupName, ShiftName y Order
            transformedData.sort((a, b) => {
                // Primero comparar por ShiftGroupName
                if (a.groupName < b.groupName) return -1;
                if (a.groupName > b.groupName) return 1;

                // Si ShiftGroupName es igual, comparar por ShiftName
                if (a.shift < b.shift) return -1;
                if (a.shift > b.shift) return 1;

                // Si ShiftName también es igual, comparar por Order
                return a.order - b.order;
            });

            return transformedData;
        }

        const createTagsContainer = (tags) => {
            if (!tags || !Array.isArray(tags) || tags.length === 0) {
                return $();
            }

            const tagsContainer = $("<div style='display:flex; flex-wrap:wrap; gap:5px;'>");

            tags.forEach(tag => {
                tagsContainer.append(createTagElement(tag));
            });

            return tagsContainer;
        }

        const createTagElement = (tag) => {
            return $("<div>")
                .text(tag)
                .css({
                    "padding": "2px 8px",
                    "background-color": "#f0f0f0",
                    "border-radius": "12px",
                    "font-size": "12px",
                    "border": "1px solid #e0e0e0",
                    "color": "#666",
                    "white-space": "nowrap"
                });
        }

        const createNameContainer = (cellItem) => {
            return $("<div style='line-height:20px'>").text(cellItem.value)
        }

        const createEditButton = (ruleData) => {
            let actionElement = $("<div style='line-height:20px;margin-left:auto'>");
            return actionElement.append($("<button style='margin-left:auto;margin-right:10px'>")
                .addClass("dx-button")
                .addClass("dx-button-mode-contained")
                .addClass("dx-button-success")
                .text(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_btnEdit", "RulesGroup"))
                .on("click", (e) => {
                    e.stopPropagation();
                    window.RulesManager.Events.edit(ruleData);
                }));
        }

        const createShiftGroupContent = (cellInfo) => {

            const shiftsDS = window.RulesManager.Data.Shifts().find(item => item.Id === cellInfo.data.key);

            if (!shiftsDS) return 'unknown';

            let groupContainer = $("<div style='display:flex; flex-direction:row' >");
            let groupText = $("<span>").text(shiftsDS.Name).css("margin-right", "10px");

            let actionButton = $("<button style='margin-left:auto;margin-right:10px'>")
                .addClass("dx-button")
                .addClass("dx-button-mode-contained")
                .text(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_btnOrder", "RulesGroup"))
                .on("click", function (e) {
                    e.stopPropagation();
                    const firstLevel = cellInfo.data?.items?.[0] ?? cellInfo.data?.collapsedItems?.[0];
                    const result = firstLevel?.items?.[0] ?? firstLevel?.collapsedItems?.[0];
                    window.RulesManager.Events.order(result);
                });

            return groupContainer.append(groupText).append(actionButton)
        }

        const createCellContent = (cellInfo) => {
            // Crear contenedor para el nombre de la regla y los tags
            let cellContent = $("<div style='display:flex; flex-direction:row; gap:10px'>");

            cellContent.append(createNameContainer(cellInfo));

            if (typeof cellInfo.data !== 'undefined' && typeof cellInfo.data.tags !== 'undefined' && cellInfo.data.tags.length > 0) {
                cellContent.append(createTagsContainer(cellInfo.data.tags));
            }

            cellContent.append(createEditButton(cellInfo.data));

            return cellContent;
        }

        const createTypeCellContent = (cellInfo) => {

            const ruleTypeDS = window.RulesManager.Data.RuleTypes().find(item => item.Id === cellInfo.data.key);

            if (ruleTypeDS != null) {
                let groupContainer = $("<div style='display:flex; flex-direction:row' >");
                let groupText = $("<span>").text(ruleTypeDS.Name).css("margin-right", "10px");

                return groupContainer.append(groupText)
            } else {
                return "unknown";
            }
        }

        const createShiftGroupCellContent = (cellInfo) => {
            const shiftGroupDS = window.RulesManager.Data.ShiftGroups().find(item => item.Id === cellInfo.data.key);

            if (!shiftGroupDS) return 'unknown';

            let groupContainer = $("<div style='display:flex; flex-direction:row' >");
            let groupText = $("<span>").text(shiftGroupDS.Name).css("margin-right", "10px");

            return groupContainer.append(groupText)
        }

        const createGroupCellContent = (cellInfo) => {

            let groupContainer = $("<div style='display:flex; flex-direction:row' >");
            let groupText = $("<span>").text(cellInfo.text).css("margin-right", "10px");

            return groupContainer.append(groupText)
        }

        const generateRuleGroupsGrid = (ds) => {
            $('#rulesGroupDashboard').dxDataGrid({
                dataSource: ds,
                keyExpr: 'id',
                showBorders: true,
                rowAlternationEnabled: true,
                grouping: { autoExpandAll: true },
                groupPanel: { visible: true },
                paging: { enabled: false },
                sorting: { enabled: false },
                searchPanel: {
                    visible: false
                },
                columns: [
                    {
                        dataField: 'groupName',
                        groupIndex: 0,
                        groupCellTemplate: function (cellElement, cellInfo) {
                            cellElement.html("").append(createGroupCellContent(cellInfo));
                        }
                    },
                    {
                        dataField: 'type', groupIndex: 1,
                        sortOrder: 'desc',
                        allowSorting: false,
                        groupCellTemplate: function (cellElement, cellInfo) {
                            cellElement.html("").append(createTypeCellContent(cellInfo));
                        }
                    },
                    {
                        dataField: 'ruleName',
                        cellTemplate: function (container, options) {
                            container.append(createCellContent(options));
                        }
                    }
                ],
                rowDragging: {
                    allowReordering: false
                },
                onRowDblClick: function (e, k) {
                    // Solo actuar en filas de grupo de primer nivel
                    if (e.rowType === 'data') {
                        e.event.preventDefault();
                        window.RulesManager.Events.edit(e.data);
                    }
                }
            });
        }

        const generateShiftGroupsGrid = (ds) => {
            $('#rulesGroupDashboard').dxDataGrid({
                dataSource: ds,
                keyExpr: 'id',
                showBorders: true,
                rowAlternationEnabled: true,
                grouping: { autoExpandAll: true },
                groupPanel: { visible: true },
                paging: { enabled: false },
                sorting: { enabled: false },
                searchPanel: {
                    visible: false
                },
                columns: [
                    {
                        dataField: 'groupName', groupIndex: 0,
                        groupCellTemplate: function (cellElement, cellInfo) {
                            cellElement.html("").append(createShiftGroupCellContent(cellInfo));
                        }
                    },
                    {
                        dataField: 'shift', groupIndex: 1,
                        groupCellTemplate: function (cellElement, cellInfo) {
                            cellElement.html("").append(createShiftGroupContent(cellInfo));
                        }
                    },
                    {
                        dataField: 'type', groupIndex: 2,
                        sortOrder: 'desc',
                        allowSorting: false,
                        groupCellTemplate: function (cellElement, cellInfo) {
                            cellElement.html("").append(createTypeCellContent(cellInfo));
                        }
                    },
                    {
                        dataField: 'name',
                        cellTemplate: function (container, options) {
                            container.append(createCellContent(options));
                        }
                    }
                ],
                rowDragging: {
                    allowReordering: false
                },
                onRowDblClick: function (e, k) {
                    // Solo actuar en filas de grupo de primer nivel
                    if (e.rowType === 'data') {
                        e.event.preventDefault();
                        window.RulesManager.Events.edit(e.data);
                    } else if (e.rowType === 'group' && e.groupIndex === 1) {
                        e.event.preventDefault();
                        const firstLevel = e.data?.items?.[0] ?? e.data?.collapsedItems?.[0];
                        const result = firstLevel?.items?.[0] ?? firstLevel?.collapsedItems?.[0];
                        window.RulesManager.Events.order(result);
                    }
                }
            });
        }

        const refreshView = async () => {
            const mappedRules = buildRulesDS();

            if (parseInt(buildSearchFilter().selectedOption, 10) == 1) generateRuleGroupsGrid(mappedRules);
            else generateShiftGroupsGrid(mappedRules);
        }

        const orderShiftRules = (shift) => {
            DevExpress.ui.notify('Ordenar horarios', 'success', 2000);
        }

        const editRule = (item) => {
            let gID = item.id;

            let tmpRule = findRuleById(window.RulesManager.GetDS(), gID);

            if (tmpRule == null) {
                DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_UnknownError", "RulesGroup"), 'error', 2000);
            } else {
                window.Rule.SetValue(structuredClone(tmpRule));
                $("#ruleFrm").dxPopup("instance").show();
            }
        }

        //#endregion

        // ==========================================
        // SECTION: Toolbar actions
        // ==========================================
        //#region Toolbar actions
        const searchRulesGroupDashboard = async () => {
            // button temporaly disables. Server search
        }

        const initNewRulePopover = (data, index) => {
            $("#listaOpciones").dxList({
                items: window.RulesManager.Data.RuleTypes(),
                height: "auto",
                itemTemplate: function (data, index) {
                    // Template personalizado para cada elemento de la lista
                    return $("<div>").addClass("list-item")
                        .append(
                            $("<span>").text(data.Name).css("margin-left", "10px")
                        )
                        .css({
                            "display": "flex",
                            "align-items": "center",
                            "padding": "8px 0"
                        });
                },
                onItemClick: function (e) {
                    // Llamar a nuestra función manejadora con el elemento seleccionado
                    handleNewRuleClick(e.itemData);                    
                }
            });
        }

        const handleNewRuleClick = (item) => {
            $("#newActionPopover").dxPopover("instance").hide();

            window.Rule.SetValue({
                Id: getNewRuleId(window.RulesManager.GetDS()),
                Name: '',
                Type: item.Id,
                RuleDefinitions: [{
                    Id: -1,
                    Description: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_NewHistoricRule", "RulesGroup"),
                    EffectiveFrom: dateToJsonIsoString(moment().startOf('day').toDate()),
                    EffectiveUntil: null,
                    EditionStatus: 3
                }],
                EditionStatus: 3
            });

            $("#ruleFrm").dxPopup("instance").show();
        }

        const addNewRule = async () => {
            $('#newActionPopover').html('');
            $('#newActionPopover').dxPopover({
                target: "#btn_createNewRule",
                position: "bottom",
                width: 250,
                showTitle: false,
                visible: false,
                contentTemplate: function (contentElement) {
                    $("<div id='listaOpciones'>").appendTo(contentElement);
                    initNewRulePopover();
                }
            });

            $("#newActionPopover").dxPopover("instance").show();
        }
        //#endregion

        // ==========================================
        // SECTION: Save changes bar
        // ==========================================
        //#region Save changes bar
        const RuleMng_undo = async () => {
            rulesGroup_PageLoad();
            return { Success: true, Data: null, ErrorText: '' };
        }

        const getChanges = (data) => {
            const changes = [];

            let editionStatusTexts = {
                0: getTextFromCatalog(undefined, `editionStatus_0`, ''),
                1: getTextFromCatalog(undefined, `editionStatus_1`, ''),
                2: getTextFromCatalog(undefined, `editionStatus_2`, ''),
                3: getTextFromCatalog(undefined, `editionStatus_3`, '')
            }

            let objectTypeTexts = {
                group: getTextFromCatalog(undefined, 'editionStatus_Grupo', ''),
                rule: getTextFromCatalog(undefined, 'editionStatus_Rule', ''),
                definition: getTextFromCatalog(undefined, 'editionStatus_Definition', ''),
                order: getTextFromCatalog(undefined, `editionStatus_Order`, ''),
                shift: getTextFromCatalog(undefined, `editionStatus_Shift`, ''),
                action: getTextFromCatalog(undefined, `editionStatus_Action`, ''),
                condition: getTextFromCatalog(undefined, `editionStatus_Condition`, '')
            }

            data.forEach(group => {
                const editStatusText = group.EditionStatus === 0 ? '' : `(${editionStatusTexts[group.EditionStatus]})`;

                analyzeItem(group, `${objectTypeTexts.group}: ${group.Name}${editStatusText}`);
            });

            return changes;

            function analyzeItem(item, path) {
                if (!shouldContinueAnalysis(item, path)) return;

                analyzeCollection(item.Rules, path, analyzeRule);
                analyzeCollection(item.RuleDefinitions, path, analyzeDefinition);
                analyzeCollection(item.Shifts, path, analyzeShift);
                analyzeCollection(item.Actions, path, analyzeAction);
                analyzeCollection(item.Conditions, path, analyzeCondition);
            }

            function shouldContinueAnalysis(item, path) {
                if (item.EditionStatus === undefined) {
                    return true; // Continuar si no tiene EditionStatus
                }

                // Registrar cambio si EditionStatus > 0
                if (item.EditionStatus > 0) {
                    changes.push({
                        Path: path,
                        Action: item.EditionStatus
                    });
                }

                // Continuar solo si no es NotEdited (0) ni Deleted (2)
                return item.EditionStatus !== 2;
            }

            function analyzeCollection(collection, parentPath, analyzerFn) {
                if (!Array.isArray(collection)) return;

                collection.forEach((item, index) => {
                    if (item && typeof item === 'object') {
                        analyzerFn(item, index, parentPath);
                    }
                });
            }

            function analyzeRule(rule, _, parentPath) {
                const editStatusText = rule.EditionStatus === 0 ? '' : `(${editionStatusTexts[rule.EditionStatus]})`;
                const rulePath = `${parentPath} > ${objectTypeTexts.rule}: ${rule.Name}${editStatusText}`;
                analyzeItem(rule, rulePath);
            }

            function analyzeDefinition(def, _, parentPath) {
                const editStatusText = def.EditionStatus === 0 ? '' : `(${editionStatusTexts[def.EditionStatus]})`;
                const defPath = `${parentPath} > ${objectTypeTexts.definition}: ${moment(def.EffectiveFrom).format(getDateLocalizationFormats().moment)} - ${def.Description}${editStatusText}`;
                analyzeItem(def, defPath);
            }

            function analyzeShift(shift, _, parentPath) {
                const editStatusText = shift.EditionStatus === 0 ? '' : `(${editionStatusTexts[shift.EditionStatus]})`;
                const shiftDesc = `${shift.ShiftName}: ${objectTypeTexts.order}: ${shift.Order}${editStatusText}`;
                const shiftPath = `${parentPath} > ${objectTypeTexts.shift}: ${shiftDesc}`;
                analyzeItem(shift, shiftPath);
            }

            function analyzeAction(action, index, parentPath) {
                const editStatusText = action.EditionStatus === 0 ? '' : `(${editionStatusTexts[action.EditionStatus]})`;
                const actionPath = `${parentPath} > ${objectTypeTexts.action}[${index}]${editStatusText}`;
                analyzeItem(action, actionPath);
            }

            function analyzeCondition(condition, index, parentPath) {
                const editStatusText = condition.EditionStatus === 0 ? '' : `(${editionStatusTexts[condition.EditionStatus]})`;
                const conditionPath = `${parentPath} > ${objectTypeTexts.condition}[${index}]${editStatusText}`;
                analyzeItem(condition, conditionPath);
            }
        };



        const RuleMng_save = async () => {
            let scenarioChanges = getChanges(window.RulesManager.GetDS());

            if (scenarioChanges.length > 0) {
                // Construir la lista de paths en HTML
                let changesList = scenarioChanges
                    .map(change => `<li>${change.Path}</li>`)
                    .join('');
                let message = `<div style="max-width:640px;">
                                  <div style="color:#b94a48;margin-bottom:8px;">
                                    <strong>${getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_Attention", "RulesGroup")}:</strong> ${getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_WarnAboutChanges", "RulesGroup") }
                                  </div>
                                  <div style="margin-bottom:8px;">${getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_ChangesDetected", "RulesGroup") }:</div>
                                  <ul style="max-height:220px;overflow-y:auto;padding-left:18px;margin-bottom:8px;border:1px solid #eee;background:#fafafa;">
                                      ${changesList}
                                  </ul>
                                  <div style="margin-top:12px;">${getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_ValidateSave", "RulesGroup") }</div>
                              </div>`;

                // Mostrar el popup de confirmación
                let result = await DevExpress.ui.dialog.confirm(message, `${getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_ConfirmSave", "RulesGroup") }`);

                if (!result) {
                    return { Success: false, Data: null, ErrorText: getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_CancelByUser", "RulesGroup") };
                }

                result = result && await window.RulesManager.Server.save(scenarioChanges);

                if (result) {
                    DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_SaveOK", "RulesGroup"), 'success', 2000);
                }

                return { Success: result, Data: null, ErrorText: '' };
            } else {
                return { Success: true, Data: null, ErrorText: '' };
            }
        }
        //#endregion


        // ==========================================
        // SECTION: Server actions
        // ==========================================
        //#region Server actions
        const processServerResult = (data) => {
            let result = false;

            window.RulesManager.lastServerResponse = data;

            if (typeof data === "object" && data != null) {
                if (data.Success) {
                    result = true;
                } else {
                    DevExpress.ui.notify(data.ErrorText, 'error', 2000);
                }
            } else {
                DevExpress.ui.notify(getTextFromCatalog(window.RuleGroups.i18n, "roRuleDashboard_UnknownError", "RulesGroup"), 'error', 2000);
            }

            return result;
        }


        const validateItem = async (item) => {
            let result = false;

            await $.ajax({
                url: `${BASE_URL}Rule/ValidateRuleHistory`,
                data: { historyEntry: item },
                type: "POST",
                dataType: "json",
                success: (data) => { result = processServerResult(data) },
                error: (error) => console.error(error),
            });

            return result;
        }

        const saveCurrentScenario = async (currentScenario, scenarioChanges) => {
            let result = false;

            await $.ajax({
                url: `${BASE_URL}RulesGroup/SaveDashboard`,
                data: { scenario: currentScenario, changes: scenarioChanges },
                type: "POST",
                dataType: "json",
                success: (data) => { result = processServerResult(data); },
                error: (error) => console.error(error),
            });

            return result;
        }
        
        //#endregion
    });
})();



