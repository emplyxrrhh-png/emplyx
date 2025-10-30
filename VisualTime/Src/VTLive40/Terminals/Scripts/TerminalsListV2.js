function cargaTerminalsList(id) {
    showLoadingGrid(true);
    actualTerminalList = id;
    actualType = "L";
    cargaTerminalsListTabSuperior(actualTerminalList);
}

function cargaTerminalsListTabSuperior(IDTerminalsList) {
    try {
        var Url = "Handlers/srvTerminalsList.ashx?action=getTerminalsListTab&aTab=" + actualTabTerminalList + "&ID=" + IDTerminalsList;
        AsyncCall("POST", Url, "CONTAINER", "divTerminals", "cargaTerminalsListBarButtons(" + IDTerminalsList + ");");
    }
    catch (e) {
        showError("cargaTerminalsListTabSuperior", e);
    }
}
var responseObjList = null;
function cargaTerminalsListBarButtons(IDTerminalsList) {
    try {
        var Url = "Handlers/srvTerminalsList.ashx?action=getBarButtons&ID=" + IDTerminalsList;
        AsyncCall("POST", Url, "JSON3", responseObjList, "parseResponseBarButtonsList(objContainerId," + IDTerminalsList + ")");
    }
    catch (e) {
        showError("cargaTerminalsListBarButtons", e);
    }
}

function parseResponseBarButtonsList(oResponse, IDTerminalsList) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaTerminalsListDivs(IDTerminalsList);
}

function cargaTerminalsListDivs(IDTerminalsList) {
    try {
        var oParameters = {};
        oParameters.aTab = actualTabTerminalList;
        oParameters.ID = actualTerminalList;
        oParameters.oType = "L";
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "GETTERMINALSLIST";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    }
    catch (e) {
        showError("cargaTerminalsListTabSuperior", e);
    }
}

function refreshTerminalStatus(IDTerminal) {
    try {
        showLoadingGrid(true);
        var Url = "Handlers/srvTerminalsList.ashx?action=getTerminalInfo&ID=" + IDTerminal;
        AsyncCall("POST", Url, "CONTAINER", "terminalStatusDiv" + IDTerminal, "showLoadingGrid(false);");
    }
    catch (e) {
        showError("cargaTerminalsListTabSuperior", e);
    }
}

function hasChangesTerminalsList(bolChanges) {
    var divTop = document.getElementById('divMsgTop');
    var divBottom = document.getElementById('divMsgBottom');

    var tagHasChanges = document.getElementById('msgHasChanges');
    var msgChanges = '<changes>';
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setMessage(msgChanges);

    if (bolChanges) {
        divTop.style.display = '';
        divBottom.style.display = '';
        document.getElementById('divContentPanels').className = "divContentPanelsWithMessage";
    } else {
        divTop.style.display = 'none';
        divBottom.style.display = 'none';
        document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
    }
}

function saveChangesTerminalsList() {
    try {
        try {
            showLoadingGrid(true);
            var oParameters = {};
            oParameters.aTab = actualTabTerminalList;
            oParameters.ID = actualTerminalList;
            oParameters.oType = "L";
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVETERMINALSLIST";
            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
        }
        catch (e) {
            showError("saveChangesTerminalsList", e);
        }
    } catch (e) { showError("saveChangesTerminalsList", e); }
}

function undoChangesTerminalsList() {
    try {
        showLoadingGrid(true);
        cargaTerminalsList(1);
    } catch (e) { showError("undoChangesTerminalsList", e); }
}

function changeTerminalsListTabs(numTab) {
    var arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    var arrDivs = new Array('divList01', 'divList02');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (n == numTab) {
            if (tab != null) tab.className = 'bTab-active';
            if (div != null) div.style.display = '';
        } else {
            if (tab != null) tab.className = 'bTab';
            if (div != null) div.style.display = 'none';
        }
    }
    actualTabTerminalList = numTab;
}

function changeTabsByNameTerminalsList(nameTab) {
    var arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    vararrDivs = new Array('divList01', 'divList02');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (div.id == nameTab) {
            if (tab != null) tab.className = 'bTab-active';
            if (div != null) div.style.display = '';
            actualTabTerminalList = n;
        } else {
            if (tab != null) tab.className = 'bTab';
            if (div != null) div.style.display = 'none';
        }
    }
}

function showUserFieldCombo(typeValue) {
    try {
        if (typeValue < 0) {
            typeValue = 0;
        }
        var divUserField = document.getElementById('divUserField');
        switch (typeValue) {
            case 0: // valor directo
                divUserField.style.display = 'none';
                break;
            //case 1: // valor directo
            //    divUserField.style.display = 'none';
            //    break;
            case 1: // según campo de la ficha
                divUserField.style.display = '';
                break;
        }
    } catch (e) { showError("showUserFieldCombo", e); }
}

function ConfirmLaunchBroadcaster() {
    try {
        var url = "Terminals/srvMsgBoxTerminals.aspx?action=LaunchBroadcaster";
        parent.ShowMsgBoxForm(url, 500, 300, '');
    }
    catch (e) {
        showError("ConfirmLaunchBroadcaster", e);
    }
}

var actualDxGridTags = null;
var zones = null;
//var modes = null;

function LoadNFCGrid(s) {
    if (typeof s.cpNFCReaders == "undefined" || s.cpNFCReaders == "") {
        actualDxGridTags = [];
    }
    else {
        actualDxGridTags = JSON.parse(s.cpNFCReaders, roDateReviver);
    }

    if (typeof s.cpZones == "undefined" || s.cpZones == "") {
        zones = [];
    }
    else {
        zones = JSON.parse(s.cpZones, roDateReviver);
    }

    //if (typeof s.cpModes == "undefined" || s.cpModes == "") {
    //    modes = [];
    //}
    //else {
    //    modes = JSON.parse(s.cpModes, roDateReviver);
    //}

    dxGridTags = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divNFCTags").dxDataGrid({
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,
        dataSource: {
            store: actualDxGridTags
        },
        editing: {
            mode: "popup",
            popup: {
                title: Globalize.formatMessage('roTagNFC'),
                showTitle: true,
                width: 500,
                height: 300
            },
            form: {
                colCount: 1,
                items: ["description", "nfc", "idzone"]
            },
            allowUpdating: true,
            //allowDeleting: true,
            allowAdding: true,
            texts: { deleteRow: 'Delete', editRow: 'Edit' }
        },
        onRowUpdated: function (e) {
            hasChanges(true);
        },
        onRowInserted: function (e) {
            hasChanges(true);
        },
        onToolbarPreparing: function (e) {
            e.toolbarOptions.items[0].showText = "always";
            e.toolbarOptions.items[0].options.text = Globalize.formatMessage('roAddTagNFC');
            e.toolbarOptions.items[0].options.icon = "add";
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                $links.text("");

                if (isEditing) {
                    $links.filter(".dx-link-save").addClass("dx-icon-save");
                    $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                } else {
                    $links.filter(".dx-link-edit").addClass("dx-icon-edit");
                    $links.filter(".dx-link-delete").addClass("dx-icon-trash");
                }
            }
        },

        onEditingStart: function (e) {
            
        },
        onRowRemoved: function (e) {
            hasChanges(true);
        },
        remoteOperations: {
            paging: true
        },
        paging: {
            pageSize: 12
        },
        pager: {
            showPageSizeSelector: false
        },
        columns: [
            { caption: "ID Reader", dataField: "id", visible: false, },
            { caption: Globalize.formatMessage('roDescription'), dataField: "description", sortIndex: 0, sortOrder: "asc", validationRules: [{ type: "required" }] },
            { caption: Globalize.formatMessage('roInternalCode'), dataField: "nfc", sortIndex: 0, sortOrder: "asc", validationRules: [{ type: "required" }] },
            {
                caption: Globalize.formatMessage('roZone'), dataField: "idzone", validationRules: [{ type: "required" }],
                lookup: {
                    dataSource: zones,
                    displayExpr: "name",
                    valueExpr: "id"
                }
            },
            {
                caption: Globalize.formatMessage('roBehaviour'), allowEditing: false,
                calculateCellValue(rowData) {
                    return rowData.idzone
                },
                lookup: {
                    dataSource: zones,
                    displayExpr: "zonemode",
                    valueExpr: "id"
                },
            },
            //{
            //    caption: Globalize.formatMessage('roBehaviour'),
            //    dataField: "idmode",
            //    allowEditing: false,
            //    lookup: {
            //        dataSource: function (options) {
            //            return {
            //                store: modes,
            //                //filter: options.data ? ["id", "=", options.data.zonemode] : null
            //            };
            //        },
            //        valueExpr: "id",
            //        displayExpr: "name"
            //    }
            //}
        ]
    }).dxDataGrid("instance");
}

function LaunchBroadcaster() {
    try {
        var stamp = '&StampParam=' + new Date().getMilliseconds();

        ajax = nuevoAjax();

        ajax.open("GET", "Handlers/srvTerminalsList.ashx?action=launchBroadcaster" + stamp, true);
        ajax.onreadystatechange = function () {
            if (ajax.readyState == 4) {
                var strResponse = ajax.responseText;
                if (strResponse.substr(0, 3) == 'NOK') {
                    var url = "Terminals/srvMsgBoxTerminals.aspx?action=EndLaunchBroadcaster&exec=NOK";
                    parent.ShowMsgBoxForm(url, 500, 300, '');
                }
                else {
                    if (strResponse == 'OK') {
                        var url = "Terminals/srvMsgBoxTerminals.aspx?action=EndLaunchBroadcaster&exec=OK";
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                    }
                }
            }
        }
        ajax.send(null)
    }
    catch (e) {
        showError("LaunchBroadcaster: ", e);
    }
}