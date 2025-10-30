var actualProductiveUnit;
var actualPUnitModes = [];

var newObjectName = "";

var chartFont = {
    family: 'Robotics',
    size: 18
}

var legendFont = {
    family: 'Robotics',
    size: 10
}

function checkProductiveUnitEmptyName(newName) {
    document.getElementById("readOnlyNameProductiveUnit").textContent = newName;
    hasChanges(true);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    changeTabs(actualTab);

    ConvertControls('divContent');
    ConvertControls('divProductiveUnit');

    checkResult(s);

    switch (s.cpAction) {
        case "GETPRODUCTIVEUNIT":
            if (s.cpIsNew == true) {
                refreshTree();
            } else {
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameProductiveUnit").textContent = s.cpNameRO;
                    hasChanges(false);
                    ASPxClientEdit.ValidateGroup(null, true);
                } else {
                    document.getElementById("readOnlyNameProductiveUnit").textContent = newObjectName;
                    hasChanges(true);
                    txtName_Client.SetValue(newObjectName);
                }
            }

            LoadUnitModes(s);
            LoadSummary(cmbSummaryPeriodClient.GetSelectedItem().value);

            break;

        case "SAVEPRODUCTIVEUNIT":
            if (s.cpResult == 'OK') {
                hasChanges(false);
                refreshTree();
            } else {
                LoadUnitModes(s);
                hasChanges(true);
            }
            break;

        default:
            hasChanges(false);
    }
    showLoadingGrid(false);
}

function saveChanges() {
    if (ASPxClientEdit.ValidateGroup(null, true)) {
        grabarProductiveUnit(actualProductiveUnit);
    } else {
        showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
    };
}

function undoChanges() {
    try {
        if (actualProductiveUnit == -1) {
            var ctlPrefix = "ctl00_contentMainBody_roTreesProductiveUnit";
            eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
        } else {
            cargaProductiveUnit(actualProductiveUnit);
        }
    } catch (e) { showError("undoChanges", e); }
}

function cargaNodo(Nodo) {
    if (Nodo.id == "source") Nodo.id = "-1";
    cargaProductiveUnit(Nodo.id);
}

function cargaProductiveUnit(IdProductiveUnit) {
    actualProductiveUnit = IdProductiveUnit;
    showLoadingGrid(true);
    cargaProductiveUnitTabSuperior(IdProductiveUnit);
}

function newProductiveUnit() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=ProductiveUnit";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newProductiveUnit', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaProductiveUnit(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

function cargaProductiveUnitTabSuperior(IdProductiveUnit) {
    try {
        var Url = "Handlers/srvProductiveUnit.ashx?action=getProductiveUnitTab&aTab=" + actualTab + "&ID=" + IdProductiveUnit;
        AsyncCall("POST", Url, "CONTAINER", "divProductiveUnit", "cargaProductiveUnitBarButtons(" + IdProductiveUnit + ");");
    }
    catch (e) {
        showError("cargaProductiveUnitTabSuperior", e);
    }
}
var responseObj = null;
function cargaProductiveUnitBarButtons(IdProductiveUnit) {
    try {
        var Url = "Handlers/srvProductiveUnit.ashx?action=getBarButtons&ID=" + IdProductiveUnit;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IdProductiveUnit + ")");
    }
    catch (e) {
        showError("cargaProductiveUnitBarButtons", e);
    }
}

function parseResponseBarButtons(oResponse, IdProductiveUnit) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaProductiveUnitDivs(IdProductiveUnit);
}

function grabarProductiveUnit(IdProductiveUnit) {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdProductiveUnit;
    oParameters.Name = document.getElementById("readOnlyNameProductiveUnit").innerHTML.trim();
    oParameters.Modes = actualPUnitModes;//JSON.stringify(actualPUnitModes);
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEPRODUCTIVEUNIT";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function cargaProductiveUnitDivs(IdProductiveUnit) {
    var oParameters = {};
    oParameters.aTab = actualTab
    oParameters.ID = IdProductiveUnit;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETPRODUCTIVEUNIT";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function ShowRemoveProductiveUnit() {
    try {
        if (actualProductiveUnit == "-1" || actualProductiveUnit == "0") { return; }

        var url = "AIScheduler/srvMsgBoxAIScheduler.aspx?action=Message";
        url = url + "&TitleKey=deleteProductiveUnit.Title";
        url = url + "&DescriptionKey=deleteProductiveUnit.Description";
        url = url + "&Option1TextKey=deleteProductiveUnit.Option1Text";
        url = url + "&Option1DescriptionKey=deleteProductiveUnit.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteProductiveUnit('" + actualProductiveUnit + "'); return false;";
        url = url + "&Option2TextKey=deleteProductiveUnit.Option2Text";
        url = url + "&Option2DescriptionKey=deleteProductiveUnit.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveProductiveUnit", e); }
}

function deleteProductiveUnit(Id) {
    try {
        if (Id == "-1" || Id == "0") {
        }
        else {
            try {
                AsyncCall("POST", "Handlers/srvProductiveUnit.ashx?action=deleteProductiveUnit&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus,true); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
            }
            catch (e) {
                showError('deleteProductiveUnit', e);
            }
        }
    }
    catch (e) {
        showError('deleteProductiveUnit', e);
    }
}

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesProductiveUnit";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, false, false);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesProductiveUnit";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
        } else if (DataType == "6") {
            refreshTree();
        }
    } catch (e) { showError("RefreshScreen", e); }
}

function checkResult(oResult) {
    if (oResult.cpResult == 'NOK') {
        if (oResult.cpAction == "SAVEPRODUCTIVEUNIT") {
            hasChanges(true);
        }

        var url = "AIScheduler/srvMsgBoxAIScheduler.aspx?action=Message&TitleKey=Error.Title.Text&" +
            "DescriptionText=" + oResult.cpMessage + "&" +
            "Option1TextKey=SaveName.Error.Option1Text&" +
            "Option1DescriptionKey=SaveName.Error.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
}

function checkStatus(oStatus, noHasChanges) {
    try {
        arrStatus = oStatus;
        objError = arrStatus[0];

        if (objError.error == "true") {
            if (objError.typemsg == "1") {
                var url = "AIScheduler/srvMsgBoxAIScheduler.aspx?action=Message&TitleKey=Error.Title&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else {
            }
            if (noHasChanges == null) {
                hasChanges(true);
            }
        }
    } catch (e) { showError("checkStatus", e); }
}

var gridUnitModes = null;
var roPUnitCalendar = null;
var unitModePopup = null;
var editPUnitMode = null;

function LoadUnitModes(s) {
    actualPUnitModes = JSON.parse(s.cpModes, roDateReviver);

    gridUnitModes = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divModesGrid").dxDataGrid({
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,
        dataSource: {
            store: actualPUnitModes
        },
        editing: {
            mode: "row",
            allowUpdating: true,
            allowDeleting: true,
            texts: { deleteRow: 'Delete', editRow: 'Edit' }
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
        onRowRemoved: function (e) {
            hasChanges(true);
        },
        onEditingStart: function (e) {
            editProductiveUnit(e);
        },
        remoteOperations: {
            sorting: true,
            paging: true
        },
        paging: {
            pageSize: 12
        },
        pager: {
            showPageSizeSelector: false
        },
        columns: [
            { caption: "Abrv.", dataField: "ShortName", allowEditing: false, width: 50 },
            { caption: "Nombre", dataField: "Name", allowEditing: false },
        ]
    }).dxDataGrid("instance");
}

function AddNewProductiveUnit(s, e) {
    var newId = 0;

    for (var i = 0; i < actualPUnitModes.length; i++) {
        if (newId > actualPUnitModes[i].ID) newId = actualPUnitModes[i].ID;
    }

    editPUnitMode = { ID: (newId - 1), IDProductiveUnit: -1, ShortName: '', Name: '', Description: '', HtmlColor: "#000000", CostValue: 0.0, UnitModePositions: [] };
    setUpAndOpenModeDialog();
}

function editProductiveUnit(e) {
    editPUnitMode = Object.clone(e.data, true);
    setUpAndOpenModeDialog();
    setTimeout(function () { gridUnitModes.cancelEditData(); }, 200);
}

function saveOrUpdateMode() {
    if (txtUnitModeNameClient.GetValue() == '') {
        showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
        return false;
    }
    if (txtUnitModeShortNameClient.GetValue() == '') {
        showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
        return false;
    }

    editPUnitMode.IDProductiveUnit = parseInt(actualProductiveUnit, 10);
    editPUnitMode.Name = txtUnitModeNameClient.GetValue();
    editPUnitMode.ShortName = txtUnitModeShortNameClient.GetValue();
    editPUnitMode.CostValue = parseFloat(txtUnitModeShortCostClient.GetValue().replace(',', '.'));
    editPUnitMode.HtmlColor = txtUnitModeColorClient.GetColor();
    editPUnitMode.Description = txtUnitModeDescClient.GetValue();
    editPUnitMode.UnitModePositions = roPUnitCalendar.oCalendar;

    var bExists = false;

    for (var i = 0; i < editPUnitMode.UnitModePositions.length; i++) {
        editPUnitMode.UnitModePositions[i].IDProductiveUnitMode = editPUnitMode.ID;

        if (typeof editPUnitMode.UnitModePositions[i].EmployeesData == 'undefined') editPUnitMode.UnitModePositions[i].EmployeesData = null;
        if (typeof editPUnitMode.UnitModePositions[i].Coverage == 'undefined') editPUnitMode.UnitModePositions[i].Coverage = 0;
    }

    for (var i = 0; i < actualPUnitModes.length; i++) {
        if (actualPUnitModes[i].ID == editPUnitMode.ID) bExists = true;
    }

    if (editPUnitMode.ID == -1 && !bExists) {
        actualPUnitModes.push(editPUnitMode);
    } else {
        actualPUnitModes = actualPUnitModes.remove(function (n) {
            return n.ID == editPUnitMode.ID;
        });
        actualPUnitModes.push(editPUnitMode);

        actualPUnitModes = actualPUnitModes.sortBy(function (n) {
            return n.Name;
        });
    }
    gridUnitModes.option('dataSource', { store: actualPUnitModes });

    return true;
}

function setUpAndOpenModeDialog() {
    var formID = '';
    var buttons = [];

    formID = "#dialogUnitNode";

    unitModePopup = $(formID).dialog({
        autoOpen: false,
        height: 'auto',
        width: '1100px',
        modal: true,
        resizable: false,
        draggable: false,
        buttons: [{
            text: Globalize.formatMessage("Done"),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                saveOrUpdateMode();

                hasChanges(true);

                unitModePopup.dialog("close");
            }
        }, {
            text: Globalize.formatMessage("Cancel"),
            "class": 'btnFlat btnFlatBlack',
            click: function () {
                unitModePopup.dialog("close");
            }
        }],
        close: function () {
        }
    });

    unitModePopup.dialog("open");

    txtUnitModeNameClient.SetValue(editPUnitMode.Name);
    txtUnitModeShortNameClient.SetValue(editPUnitMode.ShortName);
    txtUnitModeShortCostClient.SetValue(Globalize.formatNumber(editPUnitMode.CostValue));
    txtUnitModeColorClient.SetColor(editPUnitMode.HtmlColor);
    txtUnitModeDescClient.SetValue(editPUnitMode.Description);

    roPUnitCalendar = new Robotics.Client.Controls.roCalendar('ctl00_contentMainBody_oCalendar', Robotics.Client.Constants.TypeView.Planification, showLoadingGrid, showErrorPopup, 'CalendarLoadRecursive');
    roPUnitCalendar.create();
    roPUnitCalendar.loadData(editPUnitMode.UnitModePositions);
}

function AddNewPUnitPosition() {
    roPUnitCalendar.prepareShfitsDialog();
}

function CallbackCalendar_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        var objResultParams = null;
        if (typeof (s.cpObjResultParams) != 'undefined') objResultParams = s.cpObjResultParams;

        roPUnitCalendar.endCallback(s.cpAction, JSON.parse(s.cpObjResult, roDateReviver), JSON.parse(objResultParams, roDateReviver));
    } else {
        if (roPUnitCalendar != null) {
            roPUnitCalendar.loadingFunctionExtended(false);

            if (!roPUnitCalendar.parseErrorMessage(s.cpAction, JSON.parse(s.cpObjResult, roDateReviver))) showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
        } else {
            showErrorPopup("Error.Title", "error", "", s.cpMessage, "Error.OK", "Error.OKDesc", "");
        }
    }
}

function complementaryDefinitionCallback_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        switch (s.cpAction) {
            case "SHIFTLAYERDEFINITION":
                roPUnitCalendar.complementaryManager.finallyPrepareDialogElements(JSON.parse(s.cpObjResult, roDateReviver));
                break;
        }
    }
}

function assignmentDefinitionCallback_CallbackComplete(s, e) {
    if (s.cpResult == "OK") {
        switch (s.cpAction) {
            case "SHIFTLAYERDEFINITION":
                roPUnitCalendar.assignmentsManager.finallyPrepareDialogElements(JSON.parse(s.cpObjResult, roDateReviver));
                break;
        }
    }
}

function PerformActionCallback_CallbackComplete(s, e) {
}

function LoadSummary(selectedPeriod) {
    showLoadingGrid(true);

    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUSummary').hide();
    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUDraw').empty();
    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUCanvas').empty();

    LoadPUModeSummary(selectedPeriod);
}

function LoadPUModeSummary(selectedPeriod) {
    var Url = "";
    Url = "Handlers/srvProductiveUnit.ashx?action=GetSummary&ID=" + actualProductiveUnit + "&Range=" + selectedPeriod;
    AsyncCall("POST", Url, "CONTAINER", "ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUDraw", "LoadPUModeChart('" + selectedPeriod + "')");
}

function LoadPUModeChart(selectedPeriod) {
    var Url = "";
    Url = "Handlers/srvProductiveUnit.ashx?action=DrawSummary&ID=" + actualProductiveUnit + "&Range=" + selectedPeriod;
    AsyncCall("POST", Url, "json2", "summaryInfo", "DrawCharts(summaryInfo);ShowHideEmptySummaryInfo();showLoadingGrid(false);");
}

function DrawCharts(summaryInfo) {
    var summaryData = JSON.parse(summaryInfo, roDateReviver);

    var dataSourceCenter = [];
    if (typeof (summaryData.modeValues) != "undefined") {
        for (var i = 0; i < summaryData.modeValues.length; i++) {
            dataSourceCenter.push({
                centerName: summaryData.modeNames[i],
                centerValue: summaryData.modeValues[i]
            });
        }

        var maxWidth = $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUCanvas').width();

        if (dataSourceCenter.length > 0) {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUCanvas').dxPieChart({
                size: {
                    width: 700,
                    height: 500
                },
                dataSource: dataSourceCenter,
                series: [
                    {
                        argumentField: "centerName",
                        valueField: "centerValue",
                        label: {
                            font: legendFont,
                            visible: true,
                            connector: {
                                visible: true,
                                width: 2
                            }
                        }
                    }
                ],
                tooltip: {
                    font: legendFont,
                    enabled: true
                },
                legend: {
                    font: legendFont
                },
                title: {
                    text: summaryData.modeSummaryName,
                    font: chartFont
                },
                "export": {
                    enabled: true
                },
                onPointClick: function (e) {
                    //var point = e.target;
                    //toggleVisibility(point);
                },
                onLegendClick: function (e) {
                    //var arg = e.target;
                    //toggleVisibility(this.getAllSeries()[0].getPointsByArg(arg)[0]);
                }
            });
        } else {
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUCanvas').empty();
            $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUCanvas').removeData();
        }
    } else {
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUCanvas').empty();
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUCanvas').removeData();
    }

    var options = {
        scale: {
            startValue: 0, endValue: 100,
            tickInterval: 50,
            label: {
                customizeText: function (arg) {
                    return arg.valueText + ' %';
                }
            }
        }
    };
}

function ShowHideEmptySummaryInfo() {
    var bNoData = true;

    if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUDraw').html().trim() == '') {
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUSummary').hide();
    } else {
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divPUSummary').show();
        bNoData = false;
    }

    if (actualProductiveUnit < 0 || bNoData) {
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_noDataRow').show();
    } else {
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_noDataRow').hide();
    }
}