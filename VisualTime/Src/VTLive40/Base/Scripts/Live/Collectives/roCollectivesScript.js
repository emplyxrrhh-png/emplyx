// ==========================================
// SECTION: Properties and Initialization
// ==========================================
//#region Properties
let currentCollectiveView = {};
let originalCollectiveView = {};
let userFields = [];
let removedUserFields = [];
let filterBuilderInstance = null;
let dataGridHistoric = null;
let currentCollectiveDefinition = {};
let isProcessingHistoryItemClick = false;
let isNewlyCreatedDefinition = false;
let employeesPopupInstance = null;
let locale = 'es';
let shortFormat = getDateLocalizationFormats().moment;

let initializationPromise = null; // Para controlar la inicialización crítica
let dataGridsInitialized = false; // Para controlar la inicialización de DataGrids

const filter = [];
const TAB_SIZE = 4;
//#endregion

// Función para asegurar que la inicialización crítica se complete una sola vez
function ensureInitialized() {
    if (!initializationPromise) {
        initializationPromise = (async () => {
            try {
                //console.log("ensureInitialized: Iniciando carga de userFields, filterBuilder y grids...");
                // loadUserFields se encarga de obtener userFields, inicializar filterBuilderInstance,
                // y llamar a initializeDataGrids (que inicializará el grid histórico la primera vez).
                await loadUserFields(); 
                //console.log("ensureInitialized: Carga de userFields, filterBuilder y grids completada.");
            } catch (error) {
                //console.error("ensureInitialized: Error durante la inicialización crítica:", error);
                DevExpress.ui.notify("Error crítico de inicialización. Por favor, recargue la página.", "error", 0);
                throw error; // Propagar el error para que los llamadores puedan manejarlo
                }
            })();
        }
        return initializationPromise;
    }

(function () {
    
    var viewHandler = null;
    $(document).ready(async function () {
        // ==========================================
        // SECTION: Initialization
        // ==========================================
        //#region Initialization
        viewUtilsManager.initAccordions();
        viewUtilsManager.setupCardListFilterButton("Collectives");
        viewUtilsManager.print("Collectives Module loaded");
        //Initialize ViewHandler
        viewHandler = viewUtilsManager.createViewStateHandler();

        locale = JSON.parse(localStorage.getItem("roLanguage")).key;
        DevExpress.localization.locale(locale);

        //Set public functions
        window.loadRequest = loadCollective;
        window.loadUserFields = loadUserFields;
        window.saveData = saveData;
        window.deleteCurrentCollective = deleteCurrentCollective;
        window.refreshCardTree = viewUtilsManager.refreshCardTree;
        window.unselectCards = viewUtilsManager.unselectCards;
        window.filterBuilderInstance = filterBuilderInstance;
        window.dataGridHistoric = dataGridHistoric;
        window.Collectives_save = saveData;
        window.Collectives_close = null;
        window.Collectives_delete = deleteCurrentCollective;
        window.Collectives_undo = initializeCollectiveData;
        window.CollectivesDefinition_save = saveDataDefinition;
        window.CollectivesDefinition_close = null;
        window.CollectivesDefinition_delete = deleteCurrentCollectiveDefinition;
        window.CollectivesDefinition_undo = undoCollectiveDefinition;
        window.CollectivesDefinition_visualize = clickVisualizeEmployees;
        window.collectiveHistoricGrid_OnHistoryItemClick = clickHictoricEntry;
        window.collectiveHistoricGrid_OnNewClick = clickNewHistoricEntry;
        

        //Register events
        document.addEventListener("startStateEvent", (data) => viewHandlerEvent(data), false);
        viewHandler.transition(viewHandler.value, "read");

        //Añadimos el botón flotante de Nuevo colectivo en el árbol
        $("#divNewButton").prependTo("#divTree");

        setDeleteStatus("Collectives", false);


        //Wait for carview load and
        viewUtilsManager.loadViewOptions("Collectives", "read", async function () {
            try {
                await ensureInitialized(); // Asegura que userFields, filterBuilder, y grids base estén listos

                // Inicializar el popup de empleados aquí
                initializeEmployeesPopup();

                setupNavigationProtection();

                if (!viewUtilsManager.getSelectedCardId()) {
                    checkDisabledButtons(true);
                    checkDisabledDefinitionButtons(true);
                    setDeleteStatus("Collectives", false);
                    setDeleteStatus("CollectivesDefinition", false);
                    //Seleccionamos el primer elemento al cargar la página
                    if ($("#CardView_DXMainTable .dxcvCard").length > 0) {
                        //console.log("viewUtilsManager.loadViewOptions: Hay tarjetas, simulando clic en la primera.");
                        $("#CardView_DXMainTable .dxcvCard")[0].click();
                    } else {
                        //console.log("viewUtilsManager.loadViewOptions: No hay tarjetas, creando nuevo colectivo.");
                        await newCollective(); // newCollective también debe esperar a la inicialización
                    }
                } else {
                    // Si ya hay un ID seleccionado
                    //console.log("viewUtilsManager.loadViewOptions: Hay un ID de tarjeta seleccionado, cargando colectivo.");
                    await loadCollective(); 
                }
            } catch (initError) {
                     //console.error("viewUtilsManager.loadViewOptions: Falló la inicialización crítica o la carga del colectivo inicial.", initError);
                }
            }, () => { }, 'LiveCollectives');
        //#endregion
    });
})();

// ==========================================
// SECTION: Navigation Protection
// ==========================================
//#region Navigation Protection
function setupNavigationProtectionEverything() {
    window.addEventListener('beforeunload', function (event) {
        if (window.sectionHasChanges("Collectives") || window.sectionHasChanges("CollectivesDefinition")) {
            const message = "No has guardado cambios en el colectivo. ¿Seguro que desea salir?"; // Mensaje estándar (los navegadores modernos pueden usar su propio mensaje por seguridad)

            setTimeout(function () {
                hideLoading(); //Se ejecuta cuando el usuario clica en Cancelar (y mantenerse en la misma página de colectivos)
            }, 100);

            event.returnValue = message;
            return message;
        }
    });
}

function exportEmployeesGrid(e) {
    var workbook = new ExcelJS.Workbook();
    var worksheet = workbook.addWorksheet('Main sheet');
        var filenameToUse;
    filenameToUse = 'collectiveEmployees_' + (currentCollectiveView.Name || 'UnnamedCollective') + '_' + moment(currentCollectiveDefinition.BeginDate).format(shortFormat) + '_' + moment().format("YYYYMMDD");

    DevExpress.excelExporter.exportDataGrid({
        worksheet: worksheet,
        component: e.component,
        customizeCell: function (options) {
            options.excelCell.font = { name: 'Arial', size: 11 };
            options.excelCell.alignment = { horizontal: 'left' };
        }
    }).then(function () {
        workbook.xlsx.writeBuffer().then(function (buffer) {
            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), filenameToUse + '.xlsx');
        });
    });
}

function setupNavigationProtection() {
    // Capturar la tecla F5 para mostrar advertencia personalizada
    document.addEventListener('keydown', function (e) {
        if (e.key === 'F5' || e.keyCode === 116) {
            if (window.sectionHasChanges("Collectives") || window.sectionHasChanges("CollectivesDefinition")) {
                // Mostrar mensaje y prevenir la recarga
                e.preventDefault();
                if (confirm("No has guardado cambios en el colectivo. " + String.fromCharCode(191) + "Seguro que desea salir?")) {
                    // Si el usuario confirma, permitir la recarga
                    window.location.reload();
                } else {
                    // Si cancela, permanecer en la página
                    setTimeout(function () {
                        hideLoading();
                    }, 100);
                }
            }
            // Si no hay cambios, permitir F5 normal
        }
    });
    // Para el cierre de pestaña o navegador (no para navegación entre páginas)
    window.addEventListener('beforeunload', function (event) {
        // Solo mostrar mensaje si se está cerrando la ventana/pestaña, no al navegar
        if ((event.clientY < 0) || event.altKey || event.ctrlKey || event.metaKey ||
            (event.clientX > window.innerWidth)) {
            if (window.sectionHasChanges("Collectives") || window.sectionHasChanges("CollectivesDefinition")) {
                const message = "No has guardado cambios en el colectivo. " + String.fromCharCode(191) + "Seguro que desea salir?";
                setTimeout(function () {
                    hideLoading(); // Se ejecuta cuando el usuario clica en Cancelar
                }, 100);
                event.returnValue = message;
                return message;
            }
        }
    });
}


function hideLoading() {
    //TODO: Ocultar loading y revisar url
}

//#endregion

// ==========================================
// SECTION: View Management
// ==========================================
//#region View Management
const initializeDataGrids = () => {
    // Esta función es llamada por loadUserFields (a través de ensureInitialized)
    if (dataGridsInitialized) {
        //console.log("initializeDataGrids: Los grids ya fueron inicializados. Omitiendo reinicialización del grid histórico.");
        return Promise.resolve();
    }
    try {
        // Inicializar el grid con Promise para garantizar la carga
        return new Promise((resolve) => {
            //console.log("initializeDataGrids: Primera inicialización de collectiveHistoricGrid (con datos vacíos).");
            window.HistoricGrid.init('collectiveHistoricGrid', [],
                getTextFromCatalog(window.Collectives.i18n, "rocollectives_historictitle", "Collectives"),
                { id: "Id", description: "Description", historyDate: "BeginDate" });

            dataGridsInitialized = true; // Marcar como inicializado DESPUÉS del primer init exitoso

            setTimeout(() => {
                updateNewHistoryButtonState(); 
                // initializeEmployeesPopup(); // Ya se llama en loadViewOptions
                resolve();
            }, 300);
        });
    } catch (error) {
        //console.error("Error en la inicialización de grids (initializeDataGrids):", error);
        return Promise.resolve(); 
    }
}

function initializeEmployeesPopup() {
    if (!employeesPopupInstance) {
        employeesPopupInstance = $("#employeesPopup").dxPopup({
            title: "",
            visible: false,
            hideOnOutsideClick: true,
            showCloseButton: true,
            width: 800,
            height: 775,
            contentTemplate: function (contentElement) {
                // Limpia el contenido anterior
                contentElement.empty();
                // Crea un nuevo div para el grid
                $("<div>")
                    .attr("id", "employeesDatagridPopup")
                    .appendTo(contentElement)
                    .dxDataGrid({
                        columns: [
                            {
                                dataField: 'EmployeeName',
                                caption: getTextFromCatalog(window.Collectives.i18n, "roCollectives_EmployeeName", "Collectives")
                            },
                            {
                                dataField: 'Group',
                                caption: getTextFromCatalog(window.Collectives.i18n, "roCollectives_Group", "Collectives"),
                                groupCellTemplate: function (groupCell, info) {
                                    groupCell.text(info.value);
                                }
                            }
                        ],
                        filterRow: {
                            visible: true,
                            applyFilter: 'auto',
                        },
                        headerFilter: {
                            visible: true // Permite filtrar por cabecera de columna
                        },
                        allowColumnReordering: true,
                        showBorders: true,
                        height: 675,
                        groupPanel: {
                            visible: true // Permite al usuario arrastrar columnas aquí para agrupar
                        },
                        sorting: {
                            mode: "multiple" // Permite ordenar por múltiples columnas
                        },
                        paging: {
                            pageSize: 15 // Define el tamaño de página
                        },
                        selection: {
                            mode: "multiple", // Permite seleccionar una o varias filas
                            selectAllMode: "allPages", // Comportamiento del checkbox "Seleccionar todo" (puede ser 'page' o 'allPages')
                            showCheckBoxesMode: "none" // Muestra checkboxes al hacer clic en la fila (otras opciones: 'onClick', 'always', 'none', 'onLongTap')
                        },
                        export: {
                            enabled: true, // Habilita la exportación de datos
                            fileName: "EmpleadosColectivo_" + moment().format("YYYYMMDD"),
                            allowExportSelectedData: false // Permite exportar solo los datos seleccionados
                        },
                        summary: { // Permite añadir resúmenes
                            groupItems: [{
                                column: "EmployeeName",
                                summaryType: "count", // Cuenta el número de empleados en cada grupo
                                displayFormat: getTextFromCatalog(window.Collectives.i18n, "roCollectives_EmployeesDataGrid_GroupCount", "Collectives") + ": {0}",
                                showInGroupFooter: true, 
                                alignByColumn: true, 
                                alignment: "right" 
                            }],
                            totalItems: [ // Añadir esta sección para el total general
                                {
                                    column: "EmployeeName", // Columna bajo la cual aparecerá el total. Puede ser cualquier columna.
                                    summaryType: "count",
                                    displayFormat: getTextFromCatalog(window.Collectives.i18n, "roCollectives_EmployeesDataGrid_TotalCount", "Collectives") + ": {0}",
                                    alignment: "left" // Alinea el texto del resumen total a la derecha
                                }
                            ]
                        },
                        dataSource: [], 
                        onExporting: exportEmployeesGrid,
                    });
            }
        }).dxPopup("instance");
    }
}

const loadUserFields = async () => {
    // Esta función ahora es llamada principalmente por ensureInitialized
    try {
        const data = await $.ajax({
            type: "POST",
            url: BASE_URL + 'Collectives/GetEmployeeUserfields',
            dataType: "json",
            data: {},
        });

        if (typeof data === 'string') {
            $("#divCollectivesMainView").html('');
            DevExpress.ui.notify(data, "error", 5000);
            throw new Error("Error al cargar campos de usuario: " + data); // Lanzar error para que ensureInitialized lo capture
        } else {
            // Mapeamos los datos recibidos al formato de dxFilterBuilder
            userFields = data.map(item => {
                let additionalProps = {};
                switch (item.FieldType) {
                    case 0:
                        additionalProps = {
                            dataType: 'string',
                            filterOperations: ["=", "<>", "startswith", "endswith", "contains", "notcontains"],
                        }
                        break;
                    case 1:
                    case 3:
                        additionalProps = {
                            dataType: 'number',
                            filterOperations: ["=", "<>", ">", ">=", "<", "<=", "between"],
                        }
                        break;
                    case 2:
                        const { formatMessage } = DevExpress.localization;
                        additionalProps = {
                            dataType: 'date',
                            caption: formatMessage(item.FieldName),
                            format: 'shortDate',
                            filterOperations: ["=", "<>", ">", ">=", "<", "<=", "timeAgoMoreThan", "timeAgoMoreOrEqualThan", "timeAgoLessThan", "timeAgoLessOrEqualThan", "anniversary"],
                            calculateFilterExpression: function (filterValue, selectedFilterOperation) {
                                return [item.FieldName, selectedFilterOperation || '=', filterValue];
                            }
                        }
                        break;
                    case 5:
                        additionalProps = {
                            filterOperations: ["=", "<>", "anyof"],
                            lookup: {
                                dataSource: item.ListValues,
                            },
                        }
                        break;
                    default:
                        additionalProps = {
                            dataType: 'string',
                        }
                }
                return {
                    dataField: item.FieldName,
                    ...additionalProps
                };
            });
            //console.log("loadUserFields: userFields cargados:", userFields.length);
            await initializeFilterBuilder(userFields); // filterBuilderInstance se establece aquí
            //console.log("loadUserFields: initializeFilterBuilder completado.");
            await initializeDataGrids(); // collectiveHistoricGrid se inicializa aquí (primera vez)
            //console.log("loadUserFields: initializeDataGrids completado.");
        }
    } catch (error) {
        //console.error('Error en loadUserFields:', error);
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roCollectivesUserFieldsNotLoaded'), "error", 2000);
            currentCollectiveView = {}; // Resetear vista si falla la carga de campos
            throw error; // Propagar el error para ensureInitialized
        }
    }

const updateEmployeesDataGrid = (filterExpression) => {

    let refdate = getEffectiveUIDate();

    // Nos aseguramos de que el popup esté inicializado
    if (!employeesPopupInstance) {
        initializeEmployeesPopup();
    }

    // Mostrar indicador de carga antes de la llamada AJAX
    const grid = $("#employeesDatagridPopup").dxDataGrid("instance");
    if (grid) {
        grid.option("loadPanel", { enabled: true, showIndicator: true });

        // Iniciar animación de carga
        grid.beginCustomLoading();
    }

    $.ajax({
        type: "POST",
        url: BASE_URL + 'Collectives/GetCollectiveEmployees',
        dataType: "json",
        data: { filterExpression: JSON.stringify(filterExpression.getFilterExpression()), refdate: refdate.apiDate },
        success: function (data) {
            // Asegurar que el popup está visible antes de actualizar el grid
            updateEmployeesDataGridDS(data);
        },
        error: function (ex) {
            //console.error("Error al obtener empleados del colectivo:", ex);
            DevExpress.ui.notify("Error al cargar los empleados", "error", 3000);

            // Detener la animación de carga en caso de error
            if (grid) {
                grid.endCustomLoading();
            }
        }
    });
}

const updateEmployeesDataGridDS = (data) => {
    setTimeout(function () {
        // Actualiza el grid dentro del popup
        const grid = $("#employeesDatagridPopup").dxDataGrid("instance");
        if (grid) {
            grid.option("dataSource", data);
            grid.refresh();

            // Detener la animación de carga después de actualizar los datos
            grid.endCustomLoading();
        }
    }, 100); // Pequeño retraso para asegurar que el DOM esté listo
}
//#endregion

// ==========================================
// SECTION: filterBuilder
// ==========================================
//#region filterBuilder

async function initializeFilterBuilder(fields) {
    filterBuilderInstance = $('#filterBuilder').dxFilterBuilder({
        fields,
        value: filter,
        onValueChanged: changeCollectiveDefinitionFilter,
        onInitialized: updateTexts,
        onContentReady: contentReadyFilter,
        customOperations: [
            createTimeAgoOperation('timeAgoMoreThan', 'roTimeAgoMoreThanOperation', '<'),
            createTimeAgoOperation('timeAgoLessThan', 'roTimeAgoLessThanOperation', '>'),
            createTimeAgoOperation('timeAgoMoreOrEqualThan', 'roTimeAgoMoreOrEqualThanOperation', '<='),
            createTimeAgoOperation('timeAgoLessOrEqualThan', 'roTimeAgoLessOrEqualThanOperation', '>='),
            {
                name: 'anyof',
                caption: viewUtilsManager.DXTranslate('roAnyOfOperation'),
                icon: 'check',
                editorTemplate(data) {
                    return $('<div>').dxTagBox({
                        value: data.value,
                        items: data.field.lookup.dataSource || [],
                        inputAttr: { 'aria-label': viewUtilsManager.DXTranslate('roAnyOfCaptionOperation') },
                        onValueChanged(e) {
                            data.setValue(e.value && e.value.length ? e.value : null);
                        },
                        width: '350px',
                    });
                },
                calculateFilterExpression(filterValue, field) {
                    return filterValue && filterValue.length
                        && Array.prototype.concat.apply([], filterValue.map((value) => [[field.dataField, '=', value], 'or'])).slice(0, -1);
                },
            },
            {
                name: 'anniversary',
                caption: viewUtilsManager.DXTranslate('roAnniversaryOperation'),
                icon: 'check',
                hasValue: false,
                calculateFilterExpression(filterValue, field) {
                    return [[field.dataField, "=", 'anniversary']];
                },
            }
        ],
    }).dxFilterBuilder('instance');
}
function createTimeAgoOperation(operationName, translationKey, comparisonOperator) {
    return {
        name: operationName,
        caption: viewUtilsManager.DXTranslate(translationKey),
        icon: 'event',
        dataTypes: ['date'],
        editorTemplate(data) {
            const numberBox = $('<div>').dxNumberBox({
                value: data.value?.quantity || null,
                min: 1,
                max: 1200,
                showSpinButtons: true,
                onValueChanged(e) {
                    if (!data.value) {
                        data.value = {};
                    }

                    if (e.value) {
                        data.value.quantity = e.value;
                        data.setValue(data.value);
                        comboBox.dxSelectBox("instance").option("disabled", false); // Habilita comboBox
                    } else {
                        delete data.value.quantity;
                        data.setValue(data.value);
                        comboBox.dxSelectBox("instance").option("disabled", true); // Deshabilita comboBox
                    }
                },
                width: '100px'
            });

            const comboBox = $('<div>').dxSelectBox({
                items: [
                    { id: 'days', text: viewUtilsManager.DXTranslate('roTimeAgoDayUnitDay') },
                    { id: 'weeks', text: viewUtilsManager.DXTranslate('roTimeAgoDayUnitWeek') },
                    { id: 'months', text: viewUtilsManager.DXTranslate('roTimeAgoDayUnitMonth') },
                    { id: 'years', text: viewUtilsManager.DXTranslate('roTimeAgoDayUnitYear') }
                ],
                value: data.value?.unit || null,
                displayExpr: 'text',
                valueExpr: 'id',
                inputAttr: { 'aria-label': viewUtilsManager.DXTranslate('roTimeAgoCaptionOperation') },
                disabled: !data.value?.quantity,
                onValueChanged(e) {
                    if (!data.value) {
                        data.value = {};
                    }
                    data.value.unit = e.value;
                    data.value.unittext = getTimeAgoUnit(e.value);
                    data.setValue(data.value);
                }
            });

            return $('<div>').append(numberBox).append(comboBox);
        },
        customizeText(fieldInfo) {
            if (!fieldInfo || !fieldInfo.value.quantity || !fieldInfo.value.unit) return null;
            return `${fieldInfo.value.quantity} ${fieldInfo.value.unittext}`;
        },
        calculateFilterExpression(filterValue, field) {
            if (!filterValue || !filterValue.quantity || !filterValue.unit) return null;
            return [[field.dataField, comparisonOperator, filterValue.quantity + '*' + filterValue.unit + '*' + 'timeAgo']];
        }
    };
}
function getTimeAgoUnit(unit) {
    switch (unit) {
        case 'days':
            return viewUtilsManager.DXTranslate('roTimeAgoDayUnitDay');
        case 'weeks':
            return viewUtilsManager.DXTranslate('roTimeAgoDayUnitWeek');
        case 'months':
            return viewUtilsManager.DXTranslate('roTimeAgoDayUnitMonth');
        case 'years':
            return viewUtilsManager.DXTranslate('roTimeAgoDayUnitYear');
        default:
            return null;
    }
}
function contentReadyFilter(e) {
    let el = document.querySelectorAll(".dx-filterbuilder-text.dx-filterbuilder-item-value > .dx-filterbuilder-item-value-text");

    el.forEach(x => {
        let text = x.textContent;
        if (text.includes("00:00.000Z"))
            x.textContent = moment(x.textContent).format(shortFormat);
    });
}
function updateTexts(e) {
    const filterText = humanizeFilterExpression(e.component.option('value')).replace(/\r\n/g, '<br>');
    // Controlar la visibilidad del filtro según si hay contenido o no
    if (filterText.trim() === "") {
        // Si no hay texto, ocultar el contenedor
        $('#filterText').html('').css('opacity', 0);
    } else {
        // Si hay texto, mostrar el contenedor con el contenido
        $('#filterText').html(filterText).css('opacity', 1);
    }

    $('#filterText').html(filterText);
    $('#dataSourceText').text(formatValue(e.component.getFilterExpression()));
}
function formatValue(value, spaces = TAB_SIZE) {
    if (value && Array.isArray(value[0])) {
        return `[${getLineBreak(spaces)}${value.map((item) => (Array.isArray(item[0]) ? formatValue(item, spaces + TAB_SIZE) : JSON.stringify(item))).join(`,${getLineBreak(spaces)}`)}${getLineBreak(spaces - TAB_SIZE)}]`;
    }
    return JSON.stringify(value);
}
function getLineBreak(spaces) {
    return `\r\n${new Array(spaces + 1).join(' ')}`;
}

function humanizeFilterExpression(filterExpression) {
    if (!filterExpression || !Array.isArray(filterExpression) || filterExpression.length === 0) {
        return "";
    }

    // Mapeo de operadores a sus claves de traducción personalizadas
    const operatorTranslationKeys = {
        // Lógicos
        and: 'roLogicAnd',
        or: 'roLogicOr',
        notAnd: 'roLogicNotAnd',
        notOr: 'roLogicNotOr',

        // Comparación
        '!': 'roComparisonNot',
        '=': 'roComparisonEqual',
        '<>': 'roComparisonNotEqual',
        '>': 'roComparisonGreaterThan',
        '>=': 'roComparisonGreaterOrEqual',
        '<': 'roComparisonLessThan',
        '<=': 'roComparisonLessOrEqual',
        startswith: 'roComparisonStartsWith',
        endswith: 'roComparisonEndsWith',
        contains: 'roComparisonContains',
        notcontains: 'roComparisonNotContains',
        anyof: 'roComparisonAnyOf',
        between: 'roComparisonBetween',

        // Tiempo
        anniversary: 'roTimeAnniversaryYear',
        timeAgoMoreThan: 'roTimeAgoMoreThan',
        timeAgoLessThan: 'roTimeAgoLessThan',
        timeAgoMoreOrEqualThan: 'roTimeAgoMoreOrEqual',
        timeAgoLessOrEqualThan: 'roTimeAgoLessOrEqual'
    };

    // Función que obtiene la traducción del operador
    function getOperatorText(operator) {
        const translationKey = operatorTranslationKeys[operator];
        const logicalOperators = new Set(["and", "or", "notAnd", "notOr"]);
        const ret = logicalOperators.has(operator) ? "\r\n" : ""; //Añadimos salto de líneas para operadores lógicos
        if (translationKey) {
            const translation = viewUtilsManager.DXTranslate(translationKey);

            // Si la traducción existe y es válida, la usamos
            if (translation && translation !== translationKey) {
                return ret + translation;
            }
        }

        // Si no hay traducción, devolvemos el operador tal cual (el sistema usará su traducción predeterminada)
        return ret + operator;
    }

    // Función para formatear valores
    function formatValue(value, operator) {
        if (value === null || value === undefined) {
            return "";
        } else if (typeof value === 'string' && (value.includes("00:00.000Z") || value.includes("00:00:00 GMT"))) {
            return moment(value).format(shortFormat);
        } else if (typeof value === 'object' && (value.toString().includes("00:00.000Z") || value.toString().includes("00:00:00 GMT"))) {
            return moment(value).format(shortFormat);
        } else if (typeof value === 'object' && value.quantity && value.unit) {
            return `${value.quantity} ${value.unittext || getTimeAgoUnit(value.unit)}`;
        } else if (Array.isArray(value)) {
            let ret = "";
            if (operator == "anyof") {
                ret = "(" + value.join(" " + viewUtilsManager.DXTranslate('roLogicOr') + " ") + ")";
            } else {
                ret = value.join(" " + viewUtilsManager.DXTranslate('roLogicAnd') + " "); //cuando es el caso de: X entre Y,Z  -> Mostramos: X está entre Y y Z
            }
            return ret;
        }

        return value;
    }

    // Función recursiva para procesar condiciones
    function processCondition(condition) {
        // Caso 1: Es un operador lógico simple (string)
        if (!Array.isArray(condition)) {
            return getOperatorText(condition);
        }

        // Caso 2: Es un grupo de condiciones
        if (Array.isArray(condition[0]) || Array.isArray(condition[1])) {
            const result = condition.map(item => {
                if (Array.isArray(item)) {
                    return processCondition(item);
                } else {
                    return getOperatorText(item);
                }
            }).join(" ");

            return `(${result})`;
        }

        // Caso 3: Es una condición simple [campo, operador, valor]
        const field = condition[0];
        const operator = condition[1];
        const value = formatValue(condition[2], operator);

        // Caso especial para aniversario
        if (operator === 'anniversary') {
            return `${getOperatorText(operator)} ${field}`;
        }

        // Para todos los demás operadores
        return `${field} ${getOperatorText(operator)} ${value}`;
    }

    // Procesar la expresión principal
    let result = processCondition(filterExpression);

    // Aplicar mejoras de formato

    // 1. Eliminar paréntesis redundantes exteriores
    if (result.startsWith('(') && result.endsWith(')')) {
        let level = 0;
        let canRemove = true;

        for (let i = 0; i < result.length; i++) {
            if (result[i] === '(') level++;
            else if (result[i] === ')') level--;

            if (level === 0 && i < result.length - 1) {
                canRemove = false;
                break;
            }
        }

        if (canRemove) {
            result = result.substring(1, result.length - 1);
        }
    }

    if (result.length > 0) {
        result = result.charAt(0).toUpperCase() + result.substring(1);
    }

    return result;
}
//#endregion



// ==========================================
// SECTION: Changes Bar for Collectives
// ==========================================
//#region Changes Bar for Collectives
const loadCollective = async () => {

    if (viewUtilsManager.getSelectedCardId() == currentCollectiveView.Id) return;

    try {
        await ensureInitialized();
    } catch (error) {
        //console.error("loadCollective: Falló la inicialización crítica. No se puede cargar el colectivo.", error);
        return { Success: false, Message: "Error de inicialización." };
    }

    if (window.sectionHasChanges("Collectives") || window.sectionHasChanges("CollectivesDefinition")) {
        if (viewUtilsManager.getSelectedCardId() != currentCollectiveView.Id) viewUtilsManager.setSelectedCardId(currentCollectiveView.Id);
        const message = getTextFromCatalog(window.Collectives.i18n, "rocollectives_pendingchangesoncollective", "Collectives");
        DevExpress.ui.notify(message, "warning", 3000);
        updateNewHistoryButtonState();
        return false; // O { Success: false, Message: message }
    }

    if (viewUtilsManager.getSelectedCardId() > 0) {
        try {
            const data = await $.ajax({
                type: "POST",
                url: BASE_URL + 'Collectives/GetCollective',
                dataType: "json",
                data: { idCollective: viewUtilsManager.getSelectedCardId() },
            });

            if (typeof data === 'string') {
                $("#divCollectivesMainView").html('');
                DevExpress.ui.notify(data, "error", 5000);
                return { Success: false, Message: data };
            } else {
                originalCollectiveView = data;
                currentCollectiveView = structuredClone(originalCollectiveView);

                isNewlyCreatedDefinition = false;
                await initializeCollectiveData(); // initializeCollectiveData es async

                forceDisableChanges('Collectives');
                setDeleteStatus("Collectives", true);

                forceDisableChanges('CollectivesDefinition');
                setDeleteStatus("CollectivesDefinition", true);

                return { Success: true, Data: currentCollectiveView }; 
            }
            
        } catch (error) {
            //console.error('Error en loadCollective:', error);
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roCollectivesNotLoaded'), "error", 2000);
            currentCollectiveView = {};
            return { Success: false, Message: 'Error al cargar colectivo: ' + error.message }; 
        }
    } else {
        await clearData(); 
        return { Success: true, Message: "Borramos datos del nuevo colectivo." }; 
        }
    };

const forceDisableChanges = (section) => {
    setEntityChanges(section, false); //Solo llamar esta función cuando carga el objeto!!
    sb_hasChanges(section, false);
}

const initializeCollectiveData = async () => {

    // Ocultar el tooltip de recordatorio si existe
    if (window.saveCollectiveTooltip) {
        window.saveCollectiveTooltip.hide();
    }

    if (isNewCollective()) {
        currentCollectiveDefinition = {};
        newCollective();
        forceDisableChanges('Collectives');
        forceDisableChanges('CollectivesDefinition');
        return { Success: true, Data: "OK" };
    }

    currentCollectiveView = structuredClone(originalCollectiveView);

    $("#txtName").dxTextBox("instance").option("value", currentCollectiveView.Name);
    $("#txtName").dxTextBox("instance").option("onValueChanged", function () {
        checkDisabledButtons();
    });

    $("#txtDescription").dxTextArea("instance").option("value", currentCollectiveView.Description);
    $("#txtDescription").dxTextArea("instance").option("onValueChanged", function () {
        checkDisabledButtons();
    });

    await refreshHistoricDataGrid();


    if (currentCollectiveView.HistoryEntries.length > 0) {
        //ordenamos registros por BeginDate desc
        currentCollectiveView.HistoryEntries.sort((a, b) => moment(b.BeginDate).toDate() - moment(a.BeginDate).toDate());
        await loadHistoricEntry(currentCollectiveView.HistoryEntries[0].Id);
    } else {
        //Gestión de botones?
        //Creo que no debería darse este caso
    }

    setChangesBarTitle('Collectives', currentCollectiveView.Name);
    updateNewHistoryButtonState();

    return { Success: true, Data: "OK" };
}

const refreshHistoricDataGrid = async () => {
    // Verificar que exista window.HistoricGrid y el método updateDS
    if (!window.HistoricGrid || typeof window.HistoricGrid.updateDS !== 'function') {
        //console.warn("HistoricGrid no está inicializado correctamente");
        return;
    }

    // Verificar que currentCollectiveView exista y que HistoryEntries sea un array
    if (!currentCollectiveView) {
        currentCollectiveView = { HistoryEntries: [] };
    }

    if (!Array.isArray(currentCollectiveView.HistoryEntries)) {
        currentCollectiveView.HistoryEntries = [];
    }

    try {
        const updatedDataSource = currentCollectiveView.HistoryEntries.filter(el => el.EditionStatus != 2); // Filtramos los registros eliminados

        await window.HistoricGrid.updateDS("collectiveHistoricGrid", updatedDataSource);

        //Si solo hay un registro o ninguno, deshabilitamos el botón de eliminar
        if (updatedDataSource.length <= 1) {
            setDeleteStatus("CollectivesDefinition", false);
        } else {
            setDeleteStatus("CollectivesDefinition", true);
        }
    } catch (error) {
        //console.error("Error al actualizar el grid histórico:", error);
        // Intentar una actualización con array vacío como fallback
        try {
            await window.HistoricGrid.updateDS("collectiveHistoricGrid", []);
        } catch (fallbackError) {
            //console.error("Error crítico en el grid histórico:", fallbackError);
        }
    }
}

const saveData = async () => {
    if (!isNewCollective()) updateCollectiveStatus(1);  //Si no es un colectivo nuevo, lo marcamos como editado

    if (validateModel(currentCollectiveView)) {
        try {
            const response = await $.ajax({
                type: "POST",
                url: BASE_URL + 'Collectives/CreateOrUpdateCollective',
                dataType: "json",
                data: { oCollectiveParam: currentCollectiveView },
            });

            if (typeof response === 'string') {
                DevExpress.ui.notify(response, "error", 5000);
                return { Success: false, Message: response };
            } else {
                // Ocultar el tooltip de recordatorio si existe
                if (window.saveCollectiveTooltip) {
                    window.saveCollectiveTooltip.hide();
                }

                originalCollectiveView = response;
                currentCollectiveView = structuredClone(response);
                isNewlyCreatedDefinition = false;
                await initializeCollectiveData(); // initializeCollectiveData es async
                checkDisabledButtons();
                updateNewHistoryButtonState();
                refreshCardTree(response.Id);
                DevExpress.ui.notify(viewUtilsManager.DXTranslate('roCollectivesSaved'), "success", 2000);
                return { Success: true, Data: response };
            }
        } catch (error) {
            DevExpress.ui.notify(viewUtilsManager.DXTranslate('roCollectivesNotSaved'), "error", 2000);
            return { Success: false, Message: error };
        }
    } else {
        return { Success: false, Message: "Modelo inválido" };
    }
}

const deleteCurrentCollective = async () => {
    window.loadingRequest = true;

    if (isEmpty(currentCollectiveView)) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roCollectiveNoViewSelected'), 'error', 2000);
        window.loadingRequest = false;
        return;
    }

    if (currentCollectiveView.Id == 0) {
        currentCollectiveView = {};
        refreshCardTree(-1);
        unselectCards();
        $("#divCollectivesMainView").html('');
        window.loadingRequest = false;
        return;
    }

    try {
        const response = await $.ajax({
            type: "DELETE",
            url: BASE_URL + 'Collectives/DeleteCollective',
            dataType: "json",
            data: { idCollective: viewUtilsManager.getSelectedCardId() },
        });

        if (typeof response === 'string') {
            DevExpress.ui.notify(response, "error", 5000);
            return { Success: false, Data: response };

        } else {
            currentCollectiveView = {};
            return { Success: true, Data: currentCollectiveView };

        }
    } catch (error) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roCollectiveNotDeleted'), "error", 2000);
        return { Success: false, Data: error };
    } finally {
        // Ocultar el tooltip de recordatorio si existe
        if (window.saveCollectiveTooltip) {
            window.saveCollectiveTooltip.hide();
        }
        clearData();
        await refreshCardTree(0);
        setTimeout(function () {
            if ($("#CardView_DXMainTable .dxcvCard").length == 0) newCollective();
        }, 350);
        
        window.loadingRequest = false;
    }
}

const newCollective = async () => {
    try {
        await ensureInitialized(); 
    } catch (error) {
        //console.error("newCollective: Falló la inicialización crítica. No se puede crear nuevo colectivo.", error);
        return;
    }
    
    let hasChanges = false;
    try {
        hasChanges = window.sectionHasChanges("Collectives") || window.sectionHasChanges("CollectivesDefinition");
    } catch (error) {
        //console.error("Error verificando cambios en newCollective:", error);
        hasChanges = true;
    }

    if (hasChanges) {
        const message = getTextFromCatalog(window.Collectives.i18n, "rocollectives_savebeforenewcollective", "Collectives");
        DevExpress.ui.notify(message, "warning", 3000);
        return;
    }

    refreshCardTree(-1);
    await clearData(); 

    //let xBeginDate = dateToJsonIsoString(moment().utc().startOf("day").toDate());
    let xBeginDate = new DateManager(new Date(),'UTC').startOf('day').toISOString();
    currentCollectiveDefinition = {
        Id: getNewHistoricId(),
        BeginDate: xBeginDate,
        EditionStatus: 3, 
        Description: "",
        Definition: "[]",
        FilterExpression: "[]"
    };
    
    if (!currentCollectiveView || !Array.isArray(currentCollectiveView.HistoryEntries)) {
        currentCollectiveView = { HistoryEntries: [] };
    }
    currentCollectiveView.HistoryEntries.push(currentCollectiveDefinition);

    $("#txtHistoricDescription").dxTextBox("instance").option("value", "");
    
    if (filterBuilderInstance) {
        filterBuilderInstance.option("value", []);
    } else {
        //console.error("newCollective: filterBuilderInstance no está definido al intentar establecer valor vacío.");
    }

    $("#datepicker").dxDateBox({
        type: 'date',
        value: moment().utc().startOf("day").toDate(),
            width: 120,
            inputAttr: { 'aria-label': 'Date' },
            displayFormat: getDateLocalizationFormats().format,
            onValueChanged: changeCollectiveDefinitionBeginDate,
        });

        updateCollectiveStatus(3); 
        await refreshHistoricDataGrid();
        updateNewHistoryButtonState();

        forceDisableChanges('Collectives');
        forceDisableChanges('CollectivesDefinition');

        await loadHistoricEntry(currentCollectiveDefinition.Id);
    }




const changeCollectiveName = (e) => {
    currentCollectiveView.Name = e.component.option("value");
    checkDisabledButtons(); // Validar si los botones deben estar habilitados
}

const changeCollectiveDesc = (e) => {
    currentCollectiveView.Description = e.component.option("value");
    checkDisabledButtons(); // Validar si los botones deben estar habilitados
}

const updateCollectiveStatus = (id) => {
    /*
    NotEdited = 0
    Edited = 1
    Deleted = 2
    [New] = 3
    */
    currentCollectiveView.EditionStatus = id;
}

const isNewCollective = () => {
    return currentCollectiveView.EditionStatus == 3;
}

const checkDisabledButtons = (forceUnmodified = false) => {
    //TODO: REVISAR ESTO, SI SIGUE ESTANDO AQUÍ EL FILTERBUILDERINSTANCE
    const unmodified = ($("#txtName").dxTextBox("instance").option("value").length <= 0
        || filterBuilderInstance?.option("value") == null
        || filterBuilderInstance?.option("value").length == 0);

    sb_hasChanges("Collectives", !forceUnmodified && !unmodified);
    setChangesBarTitle("Collectives", $("#txtName").dxTextBox("instance").option("value"));
    //checkDisabledDefinitionButtons();
}

const clearData = async () => { 
    $("#txtName").dxTextBox("instance").option("value", "");
    $("#txtDescription").dxTextArea("instance").option("value", "");
    if (filterBuilderInstance) { 
        filterBuilderInstance.option("value", []);
    }
    currentCollectiveView = { HistoryEntries : []};
    currentCollectiveDefinition = {};
    await refreshHistoricDataGrid(); 
    checkDisabledButtons(true);
        setDeleteStatus("Collectives", false);
    }
//#endregion



// ==========================================
// SECTION: Changes Bar for CollectivesDefinition
// ==========================================
//#region Changes Bar for CollectivesDefinition
const undoCollectiveDefinition = async () => {
    // Si es un registro nuevo (Id negativo)
    if (isNewDefinitionCollective() && currentCollectiveDefinition.Id < 0) {
        // Solo limpiamos los datos de la pantalla, pero mantenemos el registro
        clearDefinitionData(false); // Pasar false para no borrar selección en grid

        // Volver a cargar el registro para restablecer valores predeterminados
        await loadHistoricEntry(currentCollectiveDefinition.Id);
        if(!isNewCollective) setDeleteStatus("CollectivesDefinition", true);
    } else {
        // Para registros existentes, restaurar desde el original
        const index = currentCollectiveView.HistoryEntries.findIndex(el => el.Id == currentCollectiveDefinition.Id);
        if (index !== -1) {
            const originalEntry = structuredClone(originalCollectiveView).HistoryEntries.find(el => el.Id == currentCollectiveDefinition.Id);
            if (originalEntry) {
                currentCollectiveView.HistoryEntries[index] = originalEntry; // Quitar la 'c' al final
            }
        }
        await loadHistoricEntry(currentCollectiveDefinition.Id);
        isNewlyCreatedDefinition = false;
    }

    updateNewHistoryButtonState();
    return { Success: true, Message: "OK" };
}


const saveDataDefinition = async () => {
    const dataValidated = await validateDefinition();

    if (dataValidated.Data) {
        if (!isNewDefinitionCollective()) {
            updateCollectiveDefinitionStatus(1);
        }

        const index = currentCollectiveView.HistoryEntries.findIndex(el => el.Id == currentCollectiveDefinition.Id);
        if (index !== -1) {
            currentCollectiveView.HistoryEntries[index] = currentCollectiveDefinition;
        } else {
            currentCollectiveView.HistoryEntries.push(currentCollectiveDefinition);
        }

        await refreshHistoricDataGrid();
        await loadHistoricEntry(currentCollectiveDefinition.Id);

        // Importante: Actualizar correctamente el estado después de guardar
        setEntityChanges('CollectivesDefinition', false);
        sb_hasChanges("CollectivesDefinition", false);

        // Actualizar también el estado del colectivo principal
        if (!isNewCollective()) updateCollectiveStatus(1); // Marcar como editado
        checkDisabledButtons();

        // Actualizar los botones del histórico
        isNewlyCreatedDefinition = false;
        setTimeout(() => {
            updateNewHistoryButtonState();
        }, 100);

        // Mostrar tooltip para recordar guardar el colectivo
        setupSaveCollectiveTooltip();
    }

    return { Success: true, Data: "OK" };
}

const validateDefinition = async () => {
    // Validar el modelo antes de guardar
    try {
        currentCollectiveDefinition.CollectiveId = currentCollectiveView.Id;
        const response = await $.ajax({
            type: "POST",
            url: BASE_URL + 'Collectives/ValidateCollectiveDefinition',
            dataType: "json",
            data: { oCollectiveDefinitionParam: currentCollectiveDefinition },
        });

        if (typeof response === 'string') {
            DevExpress.ui.notify(response, "error", 5000);
            return { Success: true, Message: response };
        } else {
            return { Success: true, Data: response };
        }
    } catch (error) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate('roCollectivesNotSaved'), "error", 2000);
        return { Success: false, Message: error };
    }
}


const deleteCurrentCollectiveDefinition = async () => {
    try {
        // 1. Eliminar o marcar el registro
        const wasNewEntry = currentCollectiveDefinition.Id < 0 || isNewlyCreatedDefinition;

        if (wasNewEntry) {
            // Para registros nuevos, eliminar completamente
            currentCollectiveView.HistoryEntries = currentCollectiveView.HistoryEntries.filter(
                el => el.Id !== currentCollectiveDefinition.Id
            );
        } else {
            // Para registros existentes, marcar como borrado
            updateCollectiveDefinitionStatus(2);
        }

        // 2. Desactivar banderas de cambios ANTES de cualquier otra operación
        isNewlyCreatedDefinition = false;
        forceDisableChanges('CollectivesDefinition');

        // 3. Limpiar la interfaz
        clearDefinitionData(true); // Limpiar selección del grid

        // 4. Actualizar el grid para mostrar la lista actualizada
        await refreshHistoricDataGrid();

        // 5. Importante: Buscar otro registro para seleccionar DESPUÉS de actualizar el grid
        const remainingEntries = currentCollectiveView.HistoryEntries.filter(el => el.EditionStatus !== 2);

        // 6. Si hay registros restantes, seleccionar uno
        if (remainingEntries && remainingEntries.length > 0) {
            // Ordenar por fecha más reciente
            remainingEntries.sort((a, b) => moment(b.BeginDate).toDate() - moment(a.BeginDate).toDate());

            // Esperar un momento para que el grid termine su actualización
            setTimeout(async () => {
                try {
                    await loadHistoricEntry(remainingEntries[0].Id);

                    // Forzar que el grid seleccione visualmente esta fila (puede que ya esté seleccionada en memoria)
                    window.HistoricGrid.selectRow("collectiveHistoricGrid", remainingEntries[0].Id);
                } catch (innerError) {
                    //console.error("Error seleccionando registro histórico después de borrar:", innerError);
                }
            }, 250); // Más tiempo para asegurar que el grid se ha actualizado
        }

        // 7. Actualizar botones
        checkDisabledButtons();
        updateNewHistoryButtonState();

        return { Success: true, Message: "OK" };
    } catch (error) {
        //console.error("Error al eliminar definición:", error);
        forceDisableChanges('CollectivesDefinition');
        DevExpress.ui.notify("Error al eliminar la definición", "error", 2000);
        return { Success: false, Message: error.message };
    }
}

const changeCollectiveDefinitionDesc = (e) => {
    currentCollectiveDefinition.Description = e.value;
    checkDisabledDefinitionButtons(); // Validar si los botones deben estar habilitados
}

const changeCollectiveDefinitionBeginDate = (e) => {
    const dateValue = new DateManager(e.value, 'UTC').toISOString();
    currentCollectiveDefinition.BeginDate = dateValue;
    updateTexts({ component: filterBuilderInstance });
    checkDisabledDefinitionButtons(); // Validar si los botones deben estar habilitados
}

const changeCollectiveDefinitionFilter = (e) => {
    updateTexts(e);
    currentCollectiveDefinition.Definition = JSON.stringify(filterBuilderInstance.option("value"));
    currentCollectiveDefinition.FilterExpression = JSON.stringify(filterBuilderInstance.getFilterExpression());
    checkDisabledDefinitionButtons();
}

const clickHictoricEntry = async (e) => {
    if (isProcessingHistoryItemClick) return;

    // Si hay cambios pendientes, impedir la navegación
    if (window.sectionHasChanges("CollectivesDefinition")) {
        // Restaurar la selección anterior
        setTimeout(() => {
            window.HistoricGrid.selectRow("collectiveHistoricGrid", currentCollectiveDefinition?.Id);
        }, 100);

        const message = getTextFromCatalog(window.Collectives.i18n, "roCollectives_PendingChangesOnDefinition", "Collectives");
        DevExpress.ui.notify(message, "warning", 3000);
        return;
    }

   try {
        isProcessingHistoryItemClick = true;
        const selectedId = e?.selectedRowKeys[0];
        if (selectedId) await loadHistoricEntry(selectedId);
    } finally {
        isProcessingHistoryItemClick = false;
    }
}


const clickNewHistoricEntry = async (e) => {
    if (canAddNewHistoricEntry()) {
        try {
            // Marcar que estamos creando una nueva definición
            isNewlyCreatedDefinition = true;

            // Deshabilitamos el botón inmediatamente
            disableNewHistoryBtn();

            // Creamos un nuevo registro
            await newHistoryEntry();

            // Establecemos el flag que evitará que cualquier evento de cambio reactive el botón
            //setEntityChanges('CollectivesDefinition', true);
            //sb_hasChanges('CollectivesDefinition', true);

            // Aseguramos que el botón permanece deshabilitado
            disableNewHistoryBtn();

        } catch (error) {
            //console.error("Error al crear nueva definición:", error);
            isNewlyCreatedDefinition = false;
        }
    } else {
        let message = "";
        if (isNewCollective() && currentCollectiveView.Id <= 0) {
            message = getTextFromCatalog(window.Collectives.i18n, "roCollectives_saveBeforeNewDefinition", "Collectives"); 
        } else {
            message = getTextFromCatalog(window.Collectives.i18n, "roCollectives_pendingChangesOnDefinition", "Collectives");
        }
        DevExpress.ui.notify(message, "warning", 3000);
    }
}
// Modificar updateNewHistoryButtonState para aceptar un parámetro que fuerce la deshabilitación
const updateNewHistoryButtonState = (forceDisabled = false) => {
    try {
        const newHistoryBtn = $(`#newHistoryItem_collectiveHistoricGrid`).dxButton("instance");

        if (newHistoryBtn) {
            // Si se pide explícitamente deshabilitar, ignoramos canAddNewHistoricEntry
            const disabled = forceDisabled ? true : !canAddNewHistoricEntry();
            newHistoryBtn.option("disabled", disabled);

            if (disabled) {
                $(`#newHistoryItem_collectiveHistoricGrid`).addClass("disabled");
            } else {
                $(`#newHistoryItem_collectiveHistoricGrid`).removeClass("disabled");
            }
        }
    } catch (e) {
        //console.error("Error en updateNewHistoryButtonState:", e);
    }
}


const disableNewHistoryBtn = () => {
    try {
        const newHistoryBtn = $(`#newHistoryItem_collectiveHistoricGrid`).dxButton("instance");
        if (newHistoryBtn) {
            newHistoryBtn.option("disabled", true);
            $(`#newHistoryItem_collectiveHistoricGrid`).addClass("disabled");
        }
    } catch (e) {
        //console.error("Error deshabilitando botón:", e);
    }
}


const canAddNewHistoricEntry = () => {
    try {
        // Si es un registro recién creado, NO permitir crear otro
        if (isNewlyCreatedDefinition) {
            return false;
        }

        // El resto igual que antes
        if (!currentCollectiveView || isEmpty(currentCollectiveView) ||
            !currentCollectiveView.HistoryEntries || isNewCollective() || currentCollectiveView.Id <= 0) {
            return false;
        }

        return !window.sectionHasChanges("CollectivesDefinition");
    } catch (e) {
        //console.error("Error en canAddNewHistoricEntry:", e);
        return false;
    }
}

const clickVisualizeEmployees = () => {
    //Actualizamos la cabecera del popup
    let refdate = getEffectiveUIDate();
    $("#employeesPopup").dxPopup("instance").option("title", (currentCollectiveView.Name || "") + " " + getTextFromCatalog(window.Collectives.i18n, "roCollectives_atDate", "Collectives") + " " + moment(refdate.apiDate).format("DD/MM/YYYY"));
    //Mostramos el popup
    $("#employeesPopup").dxPopup("instance").show();

    // Actualizamos los datos del grid dentro del popup
    updateEmployeesDataGrid($('#filterBuilder').dxFilterBuilder("instance"), );

    return { Success: true, Message: "OK" };
}



const canNavigateAway = () => {
    // Verificar cambios tanto en el colectivo como en la definición
    if (window.sectionHasChanges("Collectives") || window.sectionHasChanges("CollectivesDefinition")) {
        const message = viewUtilsManager.DXTranslate('roCollectiveSaveCurrentDefinition') ||
            "Debe guardar o descartar los cambios en la definición actual";
        DevExpress.ui.notify(message, "warning", 3000);
        return false;
    }
    return true;
}

const newHistoryEntry = async () => {
    //let xBeginDate = dateToJsonIsoString(moment().utc().startOf("day").toDate());
    let xBeginDate = new DateManager(new Date(), 'UTC').startOf('day').toISOString();
    currentCollectiveDefinition = {
        Id: getNewHistoricId(),
        BeginDate: xBeginDate,
        EditionStatus: 3,
        Description: "",
        Definition: "[]",   // Asegurarse de que esté inicializado correctamente
        FilterExpression: "[]"  // Asegurarse de que esté inicializado correctamente
    }

    currentCollectiveView.HistoryEntries.push(currentCollectiveDefinition);

    await refreshHistoricDataGrid();
    await loadHistoricEntry(currentCollectiveDefinition.Id);
}


const getNewHistoricId = () => {
    const ds = currentCollectiveView.HistoryEntries;
    if (!ds || ds.length === 0) return -1;

    // Usar reduce para encontrar el ID mínimo
    let minID = ds.reduce((minId, group) => {
        return (group.Id < minId) ? group.Id : minId;
    }, ds[0].Id);

    if (minID > 0) minID = 0;

    return minID - 1;
}

// Función recursiva para identificar campos de usuario faltantes en el filtro
function findMissingUserFields(filterItem) {
    // Casos base: no es un array o es un array vacío
    if (!filterItem || !Array.isArray(filterItem) || filterItem.length === 0) {
        return [];
    }

    const missingFields = [];

    // Si es una condición simple [campo, operador, valor]
    if (filterItem.length === 3 && typeof filterItem[0] === 'string' && typeof filterItem[1] === 'string') {
        // El primer elemento es el campo de usuario
        const fieldName = filterItem[0];
        if (!userFields.find(e => e.dataField === fieldName)) {
            missingFields.push(fieldName);
        }
        return missingFields;
    }

    // Si es un grupo de condiciones con operadores lógicos (and, or)
    // Procesamos cada elemento que pueda ser una condición
    filterItem.forEach(item => {
        if (Array.isArray(item)) {
            // Llamada recursiva para procesar este subgrupo
            const subMissing = findMissingUserFields(item);
            missingFields.push(...subMissing);
        }
        // Si es una cadena, es un operador lógico (and, or) y lo ignoramos
    });

    return missingFields;
}

const loadHistoricEntry = async (id) => {
    if (!filterBuilderInstance || !userFields) { // userFields.length === 0 podría ser válido si no hay campos
         //console.error("loadHistoricEntry: filterBuilderInstance o userFields no están definidos/listos. Flujo de inicialización incorrecto.");
         DevExpress.ui.notify("Error interno: Componentes de filtro no listos para cargar entrada histórica.", "error", 5000);
         return; 
    }

    const entry = currentCollectiveView.HistoryEntries.find(e => e.Id == id);
    if (!entry) { 
        //console.error(`loadHistoricEntry: No se encontró la definición histórica con Id = ${ id }.currentCollectiveView.HistoryEntries: `, currentCollectiveView.HistoryEntries);
        DevExpress.ui.notify(`Error: No se encontró la entrada histórica(Id: ${ id }).`, "error", 3000);
        await clearDefinitionData(true); 
        return;
    }
    currentCollectiveDefinition = entry;

    await window.HistoricGrid.selectRow("collectiveHistoricGrid", id);

    $("#txtHistoricDescription").dxTextBox("instance").option("value", currentCollectiveDefinition.Description);

    let filterValue = [];
    try {
        if (currentCollectiveDefinition.Definition && currentCollectiveDefinition.Definition.trim() !== "") {
            filterValue = JSON.parse(currentCollectiveDefinition.Definition);
        }
    } catch (error) {
        //console.error("Error parsing Definition JSON en loadHistoricEntry:", error, "Definición:", currentCollectiveDefinition.Definition);
        DevExpress.ui.notify("Error al procesar la definición del filtro guardada. Se usará un filtro vacío.", "warning", 5000);
        filterValue = []; 
    }

    const missingUserFields = findMissingUserFields(filterValue);

    if (missingUserFields.length > 0) {
        const message = getTextFromCatalog(
            window.Collectives.i18n,
            "rocollectives_removeduserfields",
            "Collectives"
        ) + ": " + [...new Set(missingUserFields)].join(", ");

        DevExpress.ui.notify(message, "warning", 5000);
        filterValue = [];
    }

    try {
        filterBuilderInstance.option("value", filterValue);
    } catch (error) {
        //console.error("Error setting filterBuilder value en loadHistoricEntry:", error, "Valor del filtro:", filterValue);
        DevExpress.ui.notify("Error al aplicar el filtro guardado. Se mostrará un filtro vacío.", "error", 5000);
        if (filterBuilderInstance) { 
            try {
                filterBuilderInstance.option("value", []);
            } catch (resetError) {
                //console.error("Error al intentar resetear filterBuilder a vacío:", resetError);
            }
        }
    }

    updateEmployeesDataGridDS([]);
        
    $("#datepicker").dxDateBox({
        type: 'date',
        value: moment(currentCollectiveDefinition.BeginDate).toDate(),
        width: 120,
            inputAttr: { 'aria-label': 'Date' },
            displayFormat: getDateLocalizationFormats().format,
            onValueChanged: changeCollectiveDefinitionBeginDate,
        });

    $("#lastChange_name").html(currentCollectiveDefinition.ModifiedBy);

    if (typeof currentCollectiveDefinition.ModifiedDate != 'undefined')
        $("#lastChange_date").html(new DateManager(currentCollectiveDefinition.ModifiedDate, 'UTC').format(shortFormat));
    else
        $("#lastChange_date").html(moment().format(shortFormat));


    if (typeof currentCollectiveDefinition.ModifiedDate != 'undefined')
        $("#lastChange_time").html(new DateManager(currentCollectiveDefinition.ModifiedDate, 'UTC').format("HH:mm"));
    else
        $("#lastChange_time").html(moment().format("HH:mm"));

    

    setEntityChanges('CollectivesDefinition', false);
    sb_hasChanges("CollectivesDefinition", false);
    updateNewHistoryButtonState();
}


const updateCollectiveDefinitionStatus = (id) => {
    /*
    NotEdited = 0
    Edited = 1
    Deleted = 2
    [New] = 3
    */
    currentCollectiveDefinition.EditionStatus = id;
}

const isNewDefinitionCollective = () => {
    return currentCollectiveDefinition.EditionStatus == 3;
}

const checkDisabledDefinitionButtons = (forceUnmodified = false) => {
    // Agregar verificaciones de nulos para evitar errores
    const txtHistoricDescription = $("#txtHistoricDescription").dxTextBox("instance");
    const filterBuilderValue = filterBuilderInstance?.option("value");

    // Si algún componente necesario no está disponible, no hacer cambios
    if (!txtHistoricDescription || !filterBuilderInstance) {
        return;
    }

    const descriptionEmpty = !txtHistoricDescription.option("value") ||
        txtHistoricDescription.option("value").length <= 0;
    const filterEmpty = !filterBuilderValue ||
        (Array.isArray(filterBuilderValue) && filterBuilderValue.length === 0);

    const unmodified = descriptionEmpty || filterEmpty;

    sb_hasChanges("CollectivesDefinition", !unmodified);
    updateNewHistoryButtonState();
}


const clearDefinitionData = (clearSelection = true) => {
    // Limpiar campos de entrada
    $("#txtHistoricDescription").dxTextBox("instance").option("value", "");
    $("#datepicker").dxDateBox("instance").option("value", new Date());
    //$("#historicLastChange").css("opacity", 0);
    filterBuilderInstance.option("value", []);

    // Solo quitar la selección si se solicita explícitamente
    if (clearSelection) {
        window.HistoricGrid.selectRow("collectiveHistoricGrid", null);
    }

    sb_hasChanges("CollectivesDefinition", false);
    setDeleteStatus("CollectivesDefinition", false);
}

//#endregion


// ==========================================
// SECTION: Utility Functions
// ==========================================
//#region Utility Functions
function validateModel(model) {
    if (model.Name == null || !model.Name) {
        DevExpress.ui.notify(viewUtilsManager.DXTranslate("roCollectiveWithoutName"), 'error', 2000);
        return false;
    }

    return true;
}
function isEmpty(obj) {
    for (const prop in obj) {
        if (Object.hasOwn(obj, prop)) {
            return false;
        }
    }

    return true;
}

function setupSaveCollectiveTooltip() {
    // Encontrar el botón de guardar colectivo
    const $saveButton = $("#Collectives_btnSaveEntity");

    if (!$saveButton || $saveButton.length === 0) {
        //console.warn("No se pudo encontrar el botón de guardar colectivo");
        return;
    }

    // Crear un contenedor separado para el tooltip si no existe
    const tooltipId = "saveCollectiveTooltip";
    if ($("#" + tooltipId).length === 0) {
        $("body").append(`<div id="${tooltipId}"></div>`);
    }

    // Mensaje del tooltip
    const tooltipMessage = "Recuerda guardar el colectivo para no perder los cambios";

    // Configurar el tooltip con un estilo más integrado con el diseño
    const tooltip = $("#" + tooltipId).dxTooltip({
        target: $saveButton,
        position: "bottom",
        arrowPosition: "top", // Asegurar que la flecha apunte hacia arriba
        contentTemplate: function () {
            return $("<div>")
                .css({
                    textAlign: "center",
                    color: "#495057",
                    fontSize: "13px",
                    display: "flex"
                })
                .html(`
                    <i class="dx-icon dx-icon-warning" style="margin-right:10px; font-size:18px; color:#e65c00;"></i>
                    <span>${tooltipMessage}</span>
                `);
        },
        animation: {
            show: { type: 'fade', duration: 200, from: 0, to: 1 },
            hide: { type: 'fade', duration: 200, from: 1, to: 0 }
        },
        closeOnOutsideClick: false,
        visible: false,
        shading: false, 
        hideOnParentScroll: false 
    }).dxTooltip("instance");

    // Guardar referencia global al tooltip para poder acceder desde otras funciones
    window.saveCollectiveTooltip = tooltip;

    // Mostrar el tooltip
    if (tooltip) {
        tooltip.show();
    }
}

function getEffectiveUIDate() {
    let endDate = currentCollectiveDefinition.EndDate;
    let beginDate = currentCollectiveDefinition.BeginDate;
    //let isotoday = dateToJsonIsoString(moment().utc().startOf('day').toDate());
    let isotoday = new DateManager(new Date(),'UTC').startOf('day').toISOString();

    let effectiveDate;

    if (beginDate !== undefined) {
        if (moment(beginDate) >= moment(isotoday)) {
            effectiveDate = beginDate;
        } else {
            if (endDate !== undefined) {
                if (moment(endDate) > moment(isotoday)) {
                    effectiveDate = isotoday;
                } else {
                    effectiveDate = endDate;
                }
            } else {
                effectiveDate = beginDate;
            }
        }
    } else {
        effectiveDate = isotoday;
    }

    return {
        apiDate: effectiveDate
    };
}
//#endregion
