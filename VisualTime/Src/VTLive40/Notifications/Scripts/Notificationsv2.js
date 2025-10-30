var actualTab = 0; // TAB per mostrar
var actualNotification; // Notification actual

var oNotification;   //Clase NotificationsData

var newObjectName = "";

var clientObjectsData = null;

function checkNotificationEmptyName(newName) {
    document.getElementById("readOnlyNameNotification").textContent = newName;
    hasChanges(true);
}

function btnOpenPopupSelectorEmployeesClient_Click() {
    PopupSelectorEmployeesClient.Show();

    let selectorValue = document.getElementById("EmployeeFilter").value;
    if (selectorValue != "" && selectorValue[0] == '{') window.frames['ifEmployeeSelector'].initUniversalSelector(JSON.parse(selectorValue), false, 'notifications');
    else window.frames['ifEmployeeSelector'].initUniversalSelector('', false, 'notifications');
}

function closeAndApplySelector(currentSelection) {
    let selector = {};
    selector.Userfields = currentSelection.UserFields;
    selector.ComposeFilter = currentSelection.ComposeFilter;
    selector.Filters = currentSelection.Filters;
    selector.Operation = currentSelection.Operation;
    document.getElementById("EmployeeFilter").value = JSON.stringify(currentSelection);
    txtEmployeesClient.SetText(currentSelection.Description);    
    txtEmployees72Client.SetText(currentSelection.Description); 
    hasChanges(true);
    PopupSelectorEmployeesClient.Hide();
}

function OnInitGroupSelector(s, e) {
    ASPxClientUtils.AttachEventToElement(window.document, "keydown", function (evt) {
        if (evt.keyCode == ASPxClientUtils.StringToShortcutCode("ESCAPE"))
            closeSelectorEmployeePopup();
    });
}

function closeSelectorEmployeePopup() {
    PopupSelectorEmployeesClient.Hide();
}

function cargaNodo(Nodo) {
    try {
        if (Nodo.id == "source") newNotification();
        else cargaNotification(Nodo.id);
    } catch (e) { showError("cargaNodo", e); }
}

//Carga Tabs y contenido Empleados
function cargaNotification(IDNotification) {
    try {
        actualNotification = IDNotification;
        //TAB Gris Superior
        showLoadingGrid(true);
        cargaNotificationTabSuperior(IDNotification);
    } catch (e) { showError("cargaNotification", e); }
}

// Carrega la part del TAB grisa superior
function cargaNotificationTabSuperior(IDNotification) {
    try {
        var Url = "";

        Url = "Handlers/srvNotifications.ashx?action=getNotificationTab&aTab=" + actualTab + "&ID=" + IDNotification;
        AsyncCall("POST", Url, "CONTAINER", "divNotification", "cargaNotificationBarButtons(" + IDNotification + ");");
    }
    catch (e) {
        showError("cargaNotificationTabSuperior", e);
    }
}

function loadDefaultButtons(IDNotification) {
    try {
        var Url = "";

        Url = "Handlers/srvNotifications.ashx?action=getNotificationTab&aTab=" + 0 + "&ID=" + -1;
        AsyncCall("POST", Url, "CONTAINER", "divNotification", "");
    }
    catch (e) {
        showError("cargaNotificationTabSuperior", e);
    }
}
var responseObj = null;
function cargaNotificationBarButtons(IDNotification) {
    try {
        var Url = "Handlers/srvNotifications.ashx?action=getBarButtons&ID=" + IDNotification;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDNotification + ")");
    }
    catch (e) {
        showError("cargaReportBarButtons", e);
    }
}

function parseResponseBarButtons(oResponse, IDNotification) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);
    changeTabs(0);
    cargaNotificationDivs(IDNotification);
}

// Carrega els apartats dels divs de l'usuari
function cargaNotificationDivs(IDNotification) {
    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = IDNotification;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETNOTIFICATION";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    //changeTabs(actualTab);
    showLoadingGrid(false);

    ConvertControls('divContent');

    checkResult(s);

    switch (s.cpAction) {
        case "GETNOTIFICATION":
            hasChanges(false);

            if (typeof s.cpClientControlsRO != 'undefined' && s.cpClientControlsRO != '') {
                clientObjectsData = JSON.parse(s.cpClientControlsRO)

                $("#lstShiftsAssignSchedule").dxTagBox({
                    dataSource: new DevExpress.data.ArrayStore({
                        data: clientObjectsData.shifts,
                        key: "ID"
                    }),                    
                    displayExpr: "Name",
                    valueExpr: "ID",
                    showSelectionControls: true,
                    applyValueMode: "useButtons",
                    readOnly: !clientObjectsData.clientEnabled,
                    value: clientObjectsData.selectedShifts,
                    onValueChanged: function (e) {
                        hasChanges(true);
                        clientObjectsData.selectedShifts = e.value;
                    }
                });
            }



            GetNOTIFICATION_AFTER(s);
            break;

        case "SAVENOTIFICATION":
            if (s.cpResult == 'OK') {
                hasChanges(false);
                refreshTree();
            } else {
                hasChanges(true);
                GetNOTIFICATION_AFTER(s)
            }
            break;

        case "GETNOTIFICATIONSCENARIOS":
            if (s.cpResult == 'OK') {
                hasChanges(false);
                loadHtmlEditors(s);
            }
            break;
        case "SAVELANGUAGETEXT":
            if (s.cpResult == 'OK') {
                hasChanges(false);
            } else {
                hasChanges(true);
            }
            break;
        default:
            hasChanges(false);
    }
}

function GetNOTIFICATION_AFTER(s) {
    try {
        if (s.cpIsNew) {
            refreshTree();
        } else {
            if (s.cpNameRO != null && s.cpNameRO != "") {
                document.getElementById("readOnlyNameNotification").textContent = s.cpNameRO;
                hasChanges(false);
                ASPxClientEdit.ValidateGroup(null, true);
            } else {
                document.getElementById("readOnlyNameNotification").textContent = newObjectName;
                hasChanges(true);
                txtName_Client.SetValue(newObjectName);
            }
        }

        if (typeof s.cpClientControlsRO != 'undefined' && s.cpClientControlsRO != '') {
            clientObjectsData = JSON.parse(s.cpClientControlsRO);            

            $("#lstShiftsToCheckFirstPunch").dxTagBox({
                dataSource: new DevExpress.data.ArrayStore({
                    data: clientObjectsData.shifts,
                    key: "ID"
                }),
                displayExpr: "Name",
                valueExpr: "ID",
                searchExpr: "Name",
                searchEnabled: true,                 
                showSelectionControls: true,
                applyValueMode: "useButtons",
                readOnly: !clientObjectsData.clientEnabled,
                value: clientObjectsData.selectedShifts,
                onValueChanged: function (e) {
                    hasChanges(true);
                    clientObjectsData.selectedShifts = e.value;
                }
            });

            $("#lstRigidShiftsToCheckFirstPunch").dxTagBox({
                dataSource: new DevExpress.data.ArrayStore({
                    data: clientObjectsData.rigidShifts,
                    key: "ID"
                }),
                displayExpr: "Name",
                valueExpr: "ID",
                searchExpr: "Name",
                searchEnabled: true,                     
                showSelectionControls: true,
                applyValueMode: "useButtons",
                readOnly: !clientObjectsData.clientEnabled,
                value: clientObjectsData.selectedShifts,
                onValueChanged: function (e) {
                    hasChanges(true);
                    clientObjectsData.selectedShifts = e.value;
                }
            });

            var gridDatasource = JSON.parse(JSON.stringify(clientObjectsData.shiftsTimeLimit));

            $("#grdShiftsNotifyAt").dxDataGrid({
    id: "grdShiftsNotifyAt",
                showColumnLines: false,                    
    showRowLines: false,
    rowAlternationEnabled: true,
    showBorders: false,
                headerFilter: { visible: true },
                searchPanel: {
                    visible: true,
                    highlightCaseSensitive: true,
                    alignment: "left"
                },
    allowColumnResizing: true,
    filterRow: { visible: false, applyFilter: "auto" },
    dataSource: {
        store: {
            type: 'array',
            key: 'ID',
            data: gridDatasource
        }
                },    
    editing: {
        mode: "row",
        allowUpdating: true,
        allowAdding: true,
        allowDeleting: true,
        texts: { deleteRow: 'Delete', editRow: 'Edit', confirmDeleteMessage: "" }
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
        if (e.rowType === "data") {
            if (e.isAltRow) {
                e.cellElement.css({ "backgroundColor": "rgb(245,245,245)" });
                e.cellElement.parent().css({ "border-left": "none" });
            }
            else {
                e.cellElement.css({ "backgroundColor": "rgb(250,250,250)" });                
            }            
        }
        else if (e.rowType === 'header') {
            e.cellElement.css({ "backgroundColor": "white" });
            e.cellElement.css({ "color": "black" });
            e.cellElement.css({ "fontWeight": "500" });
            e.cellElement.css({ "fontSize": "12px" });
            e.cellElement.css({ "border": "0px !important" });
        }
    },
    onRowPrepared: function (e) {
        e.rowElement.css({ height: 30 });
        e.rowElement.css({ "border": "0px !important" });
                },                
                onEditorPreparing: function (e) {
                    if (e.dataField === "ID" && e.parentType === "dataRow") {
                        if (e.row.isNewRow) {
                            e.editorOptions.readOnly = false;                            
                        } else {
                            e.editorOptions.readOnly = true;                            
                        }
                    }
                },   
                onRowInserted: function (e) {    
                    clientObjectsData.selectedShiftsTimeLimit = $("#grdShiftsNotifyAt").dxDataGrid("instance").getDataSource().items();
                    hasChanges(true);
        },
                onRowRemoved: function (e) {
                    clientObjectsData.selectedShiftsTimeLimit = $("#grdShiftsNotifyAt").dxDataGrid("instance").getDataSource().items();
            hasChanges(true);
        },
                onRowUpdated: function (e) {
                    clientObjectsData.selectedShiftsTimeLimit = $("#grdShiftsNotifyAt").dxDataGrid("instance").getDataSource().items();
            hasChanges(true);
                },                
    remoteOperations: {
        sorting: true,
        paging: true
    },
    paging: {
        enabled: true,
        pageSize: 50
    },
    pager: {
        showPageSizeSelector: true,
        allowedPageSizes: [10, 50, 100],
        showInfo: true
    },
    columns: [
        {
            caption: (Globalize.formatMessage('shift') != null && Globalize.formatMessage('shift').length > 0 ? Globalize.formatMessage('shift') : 'Horario'),
            dataField: "ID",            
            allowEditing: true,
            allowDeleting: true,
            alignment: "center",
            lookup: {
                dataSource: function (option) {
                    if (option.isNewRow) {
                        var gridInstance = $("#grdShiftsNotifyAt").dxDataGrid("instance");
                        var existingIds = gridInstance.getDataSource().items().map(function (item) {
                            return item.ID;
                        });
                        return clientObjectsData.shifts.filter(function (shift) {
                            return existingIds.indexOf(shift.ID) === -1;
                        });
                    }
                    else {
                        return clientObjectsData.shifts;
                    }
                },
                displayExpr: "Name",
                valueExpr: "ID"
            },
            alignment: 'left',
            validationRules: [{ type: 'required' }],            
            filterOperations: ["contains"]            
        },
        {
            caption: (Globalize.formatMessage('timeLimit') != null && Globalize.formatMessage('timeLimit').length > 0 ? Globalize.formatMessage('timeLimit') : 'Hora de corte'), 
            dataField: "TimeLimit",            
            allowEditing: true,
            searchEnabled: true,
            allowDeleting: true,
            alignment: "center",
            dataType: "datetime",
            format: "HH:mm",
            alignment: 'left',
            validationRules: [{ type: 'required' }],
            editorOptions: { 
                displayFormat: "HH:mm",
                useMaskBehavior: true,
                type: "time"
            },
            width: 150
        }
    ]
}).dxDataGrid("instance");



            
        }

        let selectorValue = document.getElementById("EmployeeFilter").value;
        if (selectorValue != "" && selectorValue[0] == '{') {
            txtEmployeesClient.SetText(buildSelectedEmployeesString(JSON.parse(selectorValue)));
            txtEmployees72Client.SetText(buildSelectedEmployeesString(JSON.parse(selectorValue)));
        }
        else {
            txtEmployeesClient.SetText(buildSelectedEmployeesString(''));
            txtEmployees72Client.SetText(buildSelectedEmployeesString(''));
        }

        if (typeof (cmbTypeClient) != 'undefined') {
            if (cmbTypeClient.GetSelectedItem() != null) {
                ShowSelectedDiv(cmbTypeClient.GetSelectedItem().value);
            }

            if (cmbTypeClient.GetSelectedItem() != null) {
                if (cmbTypeClient.GetSelectedItem().value == '18') {
                    if (cmbTypeValueClient.GetSelectedItem() != null) {
                        showTypeValue(cmbTypeValueClient.GetSelectedItem().value);
                    }
                }
            }
        } else {
            var selectedValue = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_cmbType_VI').value;
            ShowSelectedDiv(selectedValue);

            if (selectedValue == '18') {
                var selectedValueType = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_grNotification18_cmbTypeValue_VI').value;
                showTypeValue(selectedValueType);
            }
        }
    }
    catch (e) {
        showError("GetNOTIFICATION_AFTER", e);
    }
}

function checkResult(oResult) {
    if (oResult.cpResult == 'NOK') {
        if (oResult.cpAction == "SAVENOTIFICATION") {
            hasChanges(true);
        }

        var url = "Notifications/srvMsgBoxNotifications.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
            "DescriptionText=" + oResult.cpMessage + "&" +
            "Option1TextKey=SaveName.Error.Option1Text&" +
            "Option1DescriptionKey=SaveName.Error.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
}

//Cambia els Tabs i els divs
function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div01');
    var selectedValue = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_cmbType_VI').value;
    ShowSelectedDiv(selectedValue);

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (n == numTab) {
            tab.className = 'bTab-active';
            div.style.display = '';
        } else {
            tab.className = 'bTab';
            div.style.display = 'none';
        }
    }
    actualTab = numTab;

    //if (numTab == 1 && oAvailableScenarios == null) {
    if (numTab == 1) {
        initialLoad_LanguageDefinition();
    }
}

//Cambia els Tabs i els divs (per nom)
function changeTabsByName(nameTab) {
    arrButtons = new Array('TABBUTTON_00');
    arrDivs = new Array('div00');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (div.id == nameTab) {
            tab.className = 'bTab-active';
            div.style.display = '';
            actualTab = n;
        } else {
            tab.className = 'bTab';
            div.style.display = 'none';
        }
    }
}

//Mostra el ToolTip a la barra d'eines
function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

//Amaga el ToolTip a la barra d'eines
function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function hasChanges(bolChanges, markRecalc) {
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

function saveChanges() {
    switch (actualTab) {
        case 0:
            if (ASPxClientEdit.ValidateGroup("CreateNotifications", true)) {
                grabarNotification(actualNotification);
            } else {
                showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
            }
            break;

        case 1:
            grabarNotification(actualNotification);
            break;
    }
}

function grabarNotification(IDNotification) {
    try {
        showLoadingGrid(true);

        if (actualTab == 0) {
            var oParameters = {};
            oParameters.aTab = actualTab;
            oParameters.ID = IDNotification;
            oParameters.Name = document.getElementById("readOnlyNameNotification").textContent.trim();
            oParameters.clientControlsData = clientObjectsData;

            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVENOTIFICATION";

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
        } else {
            saveScenarioAndContinue();
        }
    } catch (e) { showError("grabarNotification", e); }
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

//Refresh de les pantalles (RETORN)
function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
        } else if (DataType == "6") {
            refreshTree();
        }
    } catch (e) { showError("RefreshScreen", e); }
}

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function undoChanges() {
    try {
        if (actualTab == 0) {
            if (actualNotification == -1) {
                var ctlPrefix = "ctl00_contentMainBody_roTreesNotifications";
                eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
            } else {
                cargaNotification(actualNotification);
            }
        } else {
            loadSelectedScenarioData();
        }
    } catch (e) { showError("undoChanges", e); }
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesNotifications";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesNotifications";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Notifications/srvMsgBoxNotifications.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        url = url + "&DescriptionKey=" + Description;
        url = url + "&Option1TextKey=" + Opt1Text;
        url = url + "&Option1DescriptionKey=" + Opt1Desc;
        url = url + "&Option1OnClickScript=HideMsgBoxForm();" + strScript1 + "; return false;";
        if (Opt2Text != null) {
            url = url + "&Option2TextKey=" + Opt2Text;
            url = url + "&Option2DescriptionKey=" + Opt2Desc;
            url = url + "&Option2OnClickScript=HideMsgBoxForm();" + strScript2 + "; return false;";
        }
        if (Opt3Text != null) {
            url = url + "&Option3TextKey=" + Opt3Text;
            url = url + "&Option3DescriptionKey=" + Opt3Desc;
            url = url + "&Option3OnClickScript=HideMsgBoxForm();" + strScript3 + "; return false;";
        }
        if (typeIcon.toUpperCase() == "TRASH") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
        } else if (typeIcon.toUpperCase() == "ERROR") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
        } else if (typeIcon.toUpperCase() == "INFO") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
        }

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("showErrorPopup", e); }
}

function showMsg(oMsg) {
    alert(oMsg);
}

/* ***************************************************************************************************/
// newNotification: Nova Notification
/* ***************************************************************************************************/
function newNotification() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=Notification";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
        loadDefaultButtons();
    } catch (e) { showError('newNotification', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaNotification(-1);
        newObjectName = ObjName;
    } catch (e) { showError('NewObjectCallback', e); }
}

/* ***************************************************************************************************/
// deleteNotification: Eliminar Notification actual
/* ***************************************************************************************************/
function deleteNotification(Id) {
    try {
        if (Id != "-1" && Id != "0") {
            AsyncCall("POST", "Handlers/srvNotifications.ashx?action=deleteXNotification&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
        }
    } catch (e) { showError('deleteNotification', e); }
}

//*****************************************************************************************/
// checkStatus
// Retorn d'objecte Status de la crida Ajax correcte
//********************************************************************************************/
function checkStatus(oStatus, noHasChanges) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Notifications/srvMsgBoxNotifications.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else { //Missatge estil inline
            }
            if (noHasChanges == null) {
                hasChanges(true);
            }
        }
    } catch (e) { showError("checkStatus", e); }
}                //end checkStatus function

/* ***************************************************************************************************/
// ShowRemoveNotification: Missatge confirmació eliminació Notification
/* ***************************************************************************************************/
function ShowRemoveNotification() {
    try {
        if (actualNotification == "-1" || actualNotification == "0") { return; }

        var url = "Notifications/srvMsgBoxNotifications.aspx?action=Message";
        url = url + "&TitleKey=deleteNotification.Title";
        url = url + "&DescriptionKey=deleteNotification.Description";
        url = url + "&Option1TextKey=deleteNotification.Option1Text";
        url = url + "&Option1DescriptionKey=deleteNotification.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteNotification('" + actualNotification + "'); return false;";
        url = url + "&Option2TextKey=deleteNotification.Option2Text";
        url = url + "&Option2DescriptionKey=deleteNotification.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveNotification", e); }
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function showNotification84() {
    $("#lstShiftsToCheckFirstPunch").hide();
    $("#lstRigidShiftsToCheckFirstPunch").show();
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblPunchBeforeStart").hide();
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblPunchNotDone").show();
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblTimeLimit").hide();
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblMinutesBefore").hide();    
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblTimeTolerance").show();
}

function showNotification83() {
    $("#lstShiftsToCheckFirstPunch").show();
    $("#lstRigidShiftsToCheckFirstPunch").hide();
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblPunchBeforeStart").show();
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblPunchNotDone").hide();
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblTimeLimit").show();
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblMinutesBefore").show();    
    $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_lblTimeTolerance").hide();
}

function ShowSelectedDiv(divNum) {
    try {          
        for (var n = 1; n <= 84; n++) {            
            var oDiv = document.getElementById('divNotification' + n);
            if (oDiv != null) {
                if (n == divNum) {
                    oDiv.style.display = '';
                }
                else {
                    oDiv.style.display = 'none';
                }
            }
        }
        if (divNum == 83 || divNum == 84) {
            var oDiv = document.getElementById('divNotification8384');
            oDiv.style.display = '';
            if (divNum == 83)
                showNotification83();
            else
                showNotification84();
        }
        else {
            var oDiv = document.getElementById('divNotification8384');
            oDiv.style.display = 'none';            
        }
        var oDiv = document.getElementById('divSupervisorByPortal');

        var defaultSupervisorIDs = new Array('1', '2', '6', '9', '10', '11', '12', '13', '14', '15', '17', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', '31', '43', '44', '51', '56', '61', '65', '66', '67', '69', '70', '72', '73', '76', '77', '78', '83', '84');

        oDiv.style.display = (defaultSupervisorIDs.indexOf(divNum) != -1 ? 'none' : '');

        var ochk = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optSupervisorByPortal_chkButton');

        if (oDiv.style.display == 'none') {
            ochk.checked = false;
        }

        oDiv = document.getElementById('divEmployeeByPortal')
        var defaultEmployeeIDs = new Array('51');

        oDiv.style.display = (defaultEmployeeIDs.indexOf(divNum) != -1 ? '' : 'none');

        var ochk = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeeByPortal_chkButton');

        if (oDiv.style.display == 'none') {
            ochk.checked = false;
        }

        oDiv = document.getElementById('divSupervisorByMail');
        oDiv.style.display = (divNum == 10 || divNum == 12 || divNum == 17 || divNum == 22 || divNum == 51 || divNum == 61 || divNum == 67 || divNum == 72 ? 'none' : '');

        ochk = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optSupervisorByMail_chkButton');

        if (oDiv.style.display == 'none') {
            ochk.checked = false;
        }

        oDiv = document.getElementById('divEmployeeField');
        oDiv.style.display = (divNum == 10 || divNum == 11 || divNum == 12 || (divNum >= 22 && divNum <= 26) || divNum == 29 || divNum == 56 || divNum == 65 || divNum == 66 || divNum == 69 || divNum == 73 || divNum == 61 ? 'none' : '');

        ochk = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeeField_chkButton');

        if (oDiv.style.display == 'none') {
            ochk.checked = false;
        }

        oDiv = document.getElementById('divMailList');
        oDiv.style.display = ((divNum >= 23 && divNum <= 26) || divNum == 29 || divNum == 72 || divNum == 83 || divNum == 84 ? 'none' : '');

        ochk = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_optMailList_chkButton');

        if (oDiv.style.display == 'none') {
            ochk.checked = false;
        }

        oDiv = document.getElementById('divBreach');
        oDiv.style.display = (divNum == 77 || divNum == 78 ? '' : 'none');

        oDiv = document.getElementById('divConditionRole');
        oDiv.style.display = (divNum == 65 || divNum == 56 ? '' : 'none');
    }
    catch (e) {
        showError("ShowSelectedDiv", e);
    }
}

function showTypeValue(type) {
    try {
        var div1 = document.getElementById('divValueType');
        var div2 = document.getElementById('divConceptUserField');
        switch (type.toUpperCase()) {
            case "DIRECTVALUE":
                div1.style.display = "";
                div2.style.display = "none";
                break;
            case "USERFIELD":
                div1.style.display = "none";
                div2.style.display = "";
                break;
        }
    }
    catch (e) {
        showError("showTypeValue", e);
    }
}

var isEditorInHtmlMode = true;

function onBtnAlterPreview(s, e) {
    isEditorInHtmlMode = !isEditorInHtmlMode;

    dxNotificationLanguageEditorClient.SetVisible(isEditorInHtmlMode);
    dxNotificationLanguagePreviewClient.SetVisible(!isEditorInHtmlMode);
}

function initialLoad_LanguageDefinition() {
    try {
        showLoadingGrid(true);

        var oParameters = {};
        oParameters.aTab = actualTab;
        oParameters.ID = -1;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "GETNOTIFICATIONSCENARIOS";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    } catch (e) { showError("cargaNotification", e); }
}

var oAvailableScenarios = null;
var oLastNotificationSelected = null;
var oLastScenarioSelected = null;
var oLastLanguageSelected = null;
var bReloadNewData = false;
function loadHtmlEditors(response) {
    oAvailableScenarios = JSON.parse(response.cpObject);
    hasChangesHtmlEditors(false);

    oLastNotificationSelected = cmbNotificationTypeClient.GetValue();
    oLastScenarioSelected = cmbAvailableScenariosClient.GetValue();
    oLastLanguageSelected = cmbAvailableLanguagesClient.GetValue();
}

function hasChangesHtmlEditors(bHasChanges, bOverwrite) {
    if (typeof bHasChanges != 'undefined' && typeof bOverwrite != 'undefined') {
        if (bOverwrite) {
            oAvailableScenarios.HasChanges = bHasChanges;
            var curScenario = oAvailableScenarios.Scenarios.find(function (scenario) { return scenario.IDScenario === oLastScenarioSelected; });
            curScenario.Subject = dxNotificationHeaderEditorClient.GetHtml();
            curScenario.Body = dxNotificationLanguageEditorClient.GetHtml();

            hasChanges(oAvailableScenarios.HasChanges);
        } else {
            if (oAvailableScenarios.HasChanges) {
                AskForSaveData();
            } else {
                loadSelectedScenarioData();
            }
        }
    }
}

function AskForSaveData() {
    var customMessageDiv = $('<div>').attr('class', 'roPopupMsg');
    customMessageDiv.append($('<div>').attr('class', 'roPopupIcon ro-icon-errorType-3'));
    customMessageDiv.append($('<div>').attr('class', 'roPopupText').text(Globalize.formatMessage("roContinueAndLoseChanges")));

    var heightDiv = $('<div>').attr('style', 'clear:both');

    var computedButtons = [];

    computedButtons.push(
        { text: Globalize.formatMessage("roSaveAndcontinue"), onClick: function () { return 'roButton1' } }
    );

    computedButtons.push(
        { text: Globalize.formatMessage("roIgnoreChanges"), onClick: function () { return 'roButton2' } }
    );

    computedButtons.push(
        { text: Globalize.formatMessage("roCancel"), onClick: function () { return 'roButton3' } }
    );

    var customDialog = DevExpress.ui.dialog.custom({
        title: Globalize.formatMessage("roQuestionTitle"),
        message: $('<div>').append(customMessageDiv, heightDiv).html(),
        css: 'roCustomDialog',
        buttons: computedButtons
    });

    customDialog.show().done(function (dialogResult) {
        switch (dialogResult) {
            case 'roButton1':
                bReloadNewData = true;
                saveScenarioAndContinue();
                break;
            case 'roButton2':
                loadSelectedScenarioData();
                break;
            case 'roButton3':
                cmbNotificationTypeClient.SetValue(oLastNotificationSelected);
                cmbAvailableScenariosClient.SetValue(oLastScenarioSelected);
                cmbAvailableLanguagesClient.SetValue(oLastLanguageSelected);
                oAvailableScenarios.HasChanges = true;
                hasChanges(oAvailableScenarios.HasChanges);
                break;
        }
        Robotics.Client.JSErrors.isShowingPopup = false;
        customDialog.hide();
    });
}

function saveScenarioAndContinue() {
    var oParameters = {};
    oParameters.Scenarios = oAvailableScenarios;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVELANGUAGETEXT";

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    SaveLanguageCallbackClient.PerformCallback(strParameters);
}

function loadSelectedScenarioData() {
    showLoadingGrid(true);

    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = cmbAvailableScenariosClient.GetValue();
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETNOTIFICATIONSCENARIOS";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function SaveLanguageCallbackClientEndCallBack(s, e) {
    showLoadingGrid(false);

    switch (s.cpAction) {
        case "SAVELANGUAGETEXT":
            if (s.cpResult == 'OK') {
                if (typeof oAvailableScenarios != 'undefined') oAvailableScenarios.HasChanges = false;
                if (bReloadNewData) loadSelectedScenarioData();

                hasChanges(false);
            } else {
                hasChanges(true);
            }
            break;
        default:
            hasChanges(false);
    }
}

function cleanFields(notificationTypeIndex) {
    switch (notificationTypeIndex) {
        case '83':
            cleanNotification83Fields();
            break;
        case '84':
            cleanNotification84Fields();
            break;
        case '72':
            cleanNotification72Fields();
            break;
    }
}
function cleanNotification84Fields() {
    $("#lstRigidShiftsToCheckFirstPunch").dxTagBox("instance").option("value", []);    
    clientObjectsData.selectedShifts = [];    
    if (txtEmployeesClient != null) txtEmployeesClient.SetValue(null);    
    
}
function cleanNotification83Fields() {        
    $("#lstShiftsToCheckFirstPunch").dxTagBox("instance").option("value", []);
    clientObjectsData.selectedShifts = [];
    if (timeLimitNotification83 != null) timeLimitNotification83.SetValue(null);
    if (txtEmployeesClient != null) txtEmployeesClient.SetValue(null);    
}

function cleanNotification72Fields() {        
    if (txtEmployees72Client != null) txtEmployees72Client.SetValue(null);    
}