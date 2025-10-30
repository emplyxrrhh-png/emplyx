(function () {
    let gridComponent = {};
    //Properties
    $(document).ready(async function () {
        window.HistoricGrid = {
            init: async (container, ds, title, keys) => initHistoricGrid(container, ds, title, keys),
            selectRow: async (container, historyId) => selectRow(historyId, gridComponent[container]),
            updateDS: async (container, ds) => updateDS(ds, gridComponent[container])
        }
        // ==========================================
        // SECTION: View management
        // ==========================================
        //#region View management
        const initHistoricGrid = async (container, ds, title, keys) => {
            let selectedGrid = {};

            ds = [...ds].filter(dsLine => {
                if (typeof dsLine.EditionStatus !== 'undefined') return dsLine.EditionStatus !== 2;
                return true;
            });

            ds = [...ds].sort((a, b) => {
                const dateA = moment(a[keys.historyDate]).toDate();
                const dateB = moment(b[keys.historyDate]).toDate();
                return dateB - dateA;
            });


            if (typeof gridComponent[container] === 'undefined') {
                selectedGrid = {
                    id: container,
                    container: $(`#${container}`),
                    ds: ds,
                    title: title,
                    keys: keys,
                    dataGridHistoric: null,
                    lastIdSelected: -1
                };
                gridComponent[container] = selectedGrid;
            } else {
                gridComponent[container].container = $(`#${container}`);
                gridComponent[container].ds = ds;
                gridComponent[container].title = title;
                gridComponent[container].keys = keys;
                gridComponent[container].dataGridHistoric = null;

                selectedGrid = gridComponent[container];
            }
            createHistoricLayout(selectedGrid);
        }
        //#endregion

        // ==========================================
        // SECTION: Historic layout
        // ==========================================
        //#region Historic layout
        const createHistoricLayout = (gridConfig) => {
            gridConfig.container.html('');

            // Crear la estructura del contenedor principal
            const divTitleBar = $("<div>").attr("class", "headerSaveBar");

            const headerNewDiv = $("<div>").css({ "display": "flex", "flex-direction": "row" });
            const titleContainer = $('<div class="objectName" style="line-height: 20px;">').append($(`<span id="${gridConfig.id}_spanTitle">`).attr('class','historyGridTitle').text(gridConfig.title) );
            const buttonContainer = $("<div>").attr("id", `newHistoryItem_${gridConfig.id}`).css({ "margin-left": "auto"});
            divTitleBar.append(headerNewDiv.append(titleContainer, buttonContainer));

            const gridContainerDiv = $("<div>").css({ "display": "inline-block", "margin-left": "10px" });
            const gridContainer = $("<div>").attr("id", `gridHistoryItem_${gridConfig.id}`);
            gridContainerDiv.append(gridContainer);

            // Añadir el contenedor principal al elemento especificado
            gridConfig.container.attr('style', 'width: 100%;display: flex;flex-direction: column;gap: 10px;');
            gridConfig.container.append(divTitleBar, gridContainerDiv);

            // Inicializar el botón DevExtreme
            $(`#newHistoryItem_${gridConfig.id}`).dxButton({
                icon: "plus",
                text: "",
                elementAttr: { id: `newHistoryItem_${gridConfig.id}` },
                onClick: async function (e) {
                    let newFunction = `${gridConfig.id}_OnNewClick`;
                    if (typeof window[newFunction] === 'function') {
                        await window[newFunction](e);
                    } else {
                        console.error(`Function ${newFunction} not found.`);
                    }
                },
                type: "default"
            });

            gridConfig.dataGridHistoric = $(`#gridHistoryItem_${gridConfig.id}`).dxTreeList({
                dataSource: gridConfig.ds,
                keyExpr: gridConfig.keys.id,
                columns: [{
                    dataField: gridConfig.keys.description,
                    caption: getTextFromCatalog(undefined, "roHistoryGrid_Description"),
                    allowSorting: false
                }, {
                    dataField: gridConfig.keys.historyDate,
                    dataType: 'date',
                    caption: getTextFromCatalog(undefined, "roHistoryGrid_Date"),
                    width: 100,
                    calculateCellValue: function(rowData) {
                        const dateValue = rowData[gridConfig.keys.historyDate];
                        if (!dateValue) return null;
                        return moment(dateValue).format(getDateLocalizationFormats().moment);
                    },
                }],
                selection: {
                    mode: 'single',
                },
                showBorders: true,
                showRowLines: true,
                onSelectionChanged: async function (e) {
                    let onHistoryClick = `${gridConfig.id}_OnHistoryItemClick`;
                    if (typeof window[onHistoryClick] === 'function') {
                        await window[onHistoryClick](e);
                    } else {
                        console.error(`Function ${onHistoryClick} not found.`);
                    }
                }
            }).dxTreeList('instance');

            selectFirstRow(gridConfig);

        }
        //#endregion

        // ==========================================
        // SECTION: Events
        // ==========================================
        //#region Events
        const selectFirstRow = (historyConfig) => {

            if (!historyConfig || historyConfig.ds.length == 0) return;

            const firstRowId = historyConfig.ds[0][historyConfig.keys.id];
            historyConfig.dataGridHistoric.selectRows([firstRowId], false);
            historyConfig.dataGridHistoric.navigateToRow(firstRowId);
        }

        const selectRow = async (newId, historyConfig) => {
            if (!historyConfig) return;

            if (historyConfig.dataGridHistoric) {
                if (newId === null || newId === undefined) {
                    historyConfig.dataGridHistoric.clearSelection();
                } else {
                    historyConfig.dataGridHistoric.selectRows([newId], false);
                    historyConfig.dataGridHistoric.navigateToRow(newId);
                }
            }
        }

        const updateDS = async (ds, historyConfig) => {
            if (!historyConfig || !historyConfig.dataGridHistoric) return;

            try {
                const selectedRowKeys = historyConfig.dataGridHistoric.getSelectedRowKeys ?
                    historyConfig.dataGridHistoric.getSelectedRowKeys() : [];

                const currentSelectedId = selectedRowKeys.length > 0 ? selectedRowKeys[0] : null;

                ds = [...ds].filter(dsLine => {
                    if (typeof dsLine.EditionStatus !== 'undefined') return dsLine.EditionStatus !== 2;
                    return true;
                });

                ds = [...ds].sort((a, b) => {
                    const dateA = moment(a[historyConfig.keys.historyDate]).toDate();
                    const dateB = moment(b[historyConfig.keys.historyDate]).toDate();
                    return dateB - dateA;
                });

                historyConfig.ds = ds;
                historyConfig.dataGridHistoric.option("dataSource", ds);

                historyConfig.dataGridHistoric.refresh();

                if (currentSelectedId) {
                    const stillExists = ds.some(item =>
                        item[historyConfig.keys.id] === currentSelectedId);


                    if (stillExists) {
                        // Restaurar la selección si el ID existe
                        historyConfig.dataGridHistoric.selectRows([currentSelectedId], false);
                        historyConfig.dataGridHistoric.navigateToRow(currentSelectedId);
                    } else {
                        // Seleccionar el primer elemento si el ID ya no existe
                        selectFirstRow(historyConfig);
                    }
                } else {
                    // Si no había elemento seleccionado, seleccionar el primero
                    selectFirstRow(historyConfig);
                }
            } catch (error) {
                console.error("Error en updateDS:", error);
            }

            
        }
        //#endregion
    });
})();


