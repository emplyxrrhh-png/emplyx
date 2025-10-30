var jsGridLocations = null;
var needToSaveLocations = false;

function showLoadingGrid(loading) { parent.showLoader(loading); }

function hasChanges(bolChanges, markRecalc) {
    var divTop = document.getElementById('divMsgTop');
    var divBottom = document.getElementById('divMsgBottom');

    var tagHasChanges = document.getElementById('msgHasChanges');
    var msgChanges = '<changes>';
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setStyleMessage('divMsg2');
    setMessage(msgChanges);

    if (bolChanges) {
        if (CheckConvertControls('') == false) {
            retrieveError();
        }

        divTop.style.display = '';
        divBottom.style.display = '';
    } else {
        divTop.style.display = 'none';
        divBottom.style.display = 'none';
    }
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function setStyleMessage(classMsg) {
    try {
        var divTop = document.getElementById('divMsgTop');
        var divBottom = document.getElementById('divMsgBottom');

        divTop.className = classMsg;
        divBottom.className = classMsg;
    } catch (e) { alert('setStyleMessage: ' + e); }
}

function CheckSave() {
    if (CheckConvertControls('') == false) {
        var message = "TitleKey=SaveConfigurationOptions.Check.Invalid.Text&" +
            "DescriptionKey=SaveConfigurationOptions.Check.Invalid.Description&" +
            "Option1TextKey=SaveConfigurationOptions.Check.Invalid.Option1Text&" +
            "Option1DescriptionKey=SaveConfigurationOptions.Check.Invalid.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            'IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png'
        var url = "Options/srvMsgBoxOptions.aspx?action=Message&Parameters=" + encodeURIComponent(message);
        parent.ShowMsgBoxForm(url, 500, 300, '');
        return false;
    }
    else {
        return true;
    }
}

function retrieveError(objError) {
    try {
        setStyleMessage('divMsg-Error');
        var tagHasErrors = document.getElementById('msgHasErrors');
        var msgErrors = '<errors>';
        if (tagHasErrors != null) {
            msgErrors = tagHasErrors.value;
        }
        setMessage(msgErrors);

        if (objError != null) {
            if (objError.tabContainer != undefined) {
                positionTabContainer(objError.tabContainer);
            }

            if (objError.tab != undefined) {
                positionTab(objError.tab);
            }

            if (objError.id != undefined) {
                try {
                    document.getElementById(objError.id).focus();
                } catch (ex) { }
            }
        }
    } catch (e) { showError('retrieveError', e); }
}

function AddNewFilesDestination() {
    try {
        var newID = jsGridLocations.getRows().length + 1;
        frmAddRoute_ShowNew(newID);
    } catch (e) { showError("AddNewFilesDestination", e); }
}

function editGridlocations(idRow) {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "0") { return; }
        var arrValues = new Array();
        arrValues = jsGridLocations.retRowJSON(idRow);

        frmAddRoute_Show(arrValues);
    } catch (e) { showError("EditLocation", e); }
}

function deleteGridlocations(idRow) {
    var oRow = document.getElementById(idRow);

    var url = "Options/srvMsgBoxOptions.aspx?action=Message";
    url = url + "&TitleKey=deleteLocation.Title";
    url = url + "&DescriptionKey=deleteLocation.Description";
    url = url + "&Option1TextKey=deleteLocation.Option1Text";
    url = url + "&Option1DescriptionKey=deleteLocation.Option1Description";
    url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delLocationSafe('" + idRow + "'); return false;";
    url = url + "&Option2TextKey=deleteLocation.Option2Text";
    url = url + "&Option2DescriptionKey=deleteLocation.Option2Description";
    url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
    url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function delLocationSafe(idRow) {
    try {
        jsGridLocations.deleteRow(idRow);
        hasChanges(true);
    } catch (e) { showError("delSelAssignment", e); }
}

function createGridLocation() {
    if (jsGridLocations == null) {
        var hdGridLocations = [{ 'fieldname': 'ID', 'description': '', 'size': '-1' },
        { 'fieldname': 'Nombre', 'description': '', 'size': '20%' },
        { 'fieldname': 'Type', 'description': '', 'size': '10%' },
        { 'fieldname': 'Location', 'description': '', 'size': '70%' },
        { 'fieldname': 'Configuration', 'description': '', 'size': '-1' }];

        hdGridLocations[1].description = document.getElementById('ctl00_contentMainBody_tcLocations_tpLocations_hdnLngNombre').value;
        hdGridLocations[2].description = document.getElementById('ctl00_contentMainBody_tcLocations_tpLocations_hdnLngType').value;
        hdGridLocations[3].description = document.getElementById('ctl00_contentMainBody_tcLocations_tpLocations_hdnLngLocation').value;

        var arrA = document.getElementById('ctl00_contentMainBody_tcLocations_tpLocations_hdnSerializedGrid').value;
        var edtRow = false;
        var delRow = false;

        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "0") {
            edtRow = true;
            delRow = true;
        }

        var arrGridLocations = new Array();
        if (arrA != "") eval("arrGridLocations = [" + arrA + "]");
        else eval('arrGridLocations = [ { "locations": []} ]');

        jsGridLocations = new jsGrid('ctl00_contentMainBody_tcLocations_tpLocations_gridLocations', hdGridLocations, arrGridLocations[0].locations, edtRow, delRow, false, 'locations');
    }
}

function prepareLocations() {
    if (jsGridLocations != null) {
        var arrLocations = jsGridLocations.toJSONStructureAdvanced();
        var jsonStr = JSON.stringify(arrLocations);
        jsonStr = "{\"locations\":" + jsonStr + "}";
        document.getElementById('ctl00_contentMainBody_tcLocations_tpLocations_hdnSerializedEndGrid').value = jsonStr;
    }
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Options/srvMsgBoxOptions.aspx?action=Message";
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

function loadMovesRelatedInfo() {
    $.ajax({
        url: `/DocumentaryManagement/GetAvailableBioCertificates`,
        data: { },
        type: "GET",
        dataType: "json",
        success: (data) => {
            if (typeof data != 'string') {
                loadCertificatesGrid(data);
            } else {
                DevExpress.ui.notify(data, 'error', 2000);
            }
        },
        error: (error) => console.error(error),
    });
}


function loadCertificatesGrid(navData) {
    $('#availableCertificates').dxDataGrid({
        dataSource: navData,
        columns: [{
            dataField: 'Title',
            caption: document.getElementById("ctl00_contentMainBody_txtCertificateHeader").value,
            width: "30%",
            allowSorting: false,
            allowSearch: false,
            allowFiltering: false
        },{
            dataField: 'DeliveredBy',
            caption: document.getElementById("ctl00_contentMainBody_txtCreatedByHeader").value,
            width: "30%",
            allowSorting: false,
            allowSearch: false,
            allowFiltering: false
        }, {
            dataField: 'DeliveredDate',
            caption: document.getElementById("ctl00_contentMainBody_txtCreatedAtHeader").value,
            dataType: "date",
            width: "30%",
            allowSorting: false,
            allowSearch: false,
            allowFiltering: false,
            calculateCellValue: (data) =>
                data.ReadTimeStamp === "1970-01-01T00:00:00"
                    ? ""
                    : window.parent.moment(data.DeliveredDate).format("DD-MM-YYYY HH:mm")
        }, {
            caption: document.getElementById("ctl00_contentMainBody_txtLinkHeader").value,
            width: "10%",
            allowSorting: false,
            allowSearch: false,
            allowFiltering: false,
            cellTemplate: function (container, options) {
                let img = $("<div>");
                img.html("<div class='photoStart'><img style='cursor:pointer' src='/Base/Images/Grid/download.png' height='16' /></div>")
                img.appendTo(container);
            }
        } ],
        showBorders: false,
        rowAlternationEnabled: true,
        filterRow: {
            visible: false,
            applyFilter: 'auto',
        },
        headerFilter: {
            visible: true,
        },
        hoverStateEnabled: true,
        selection: {
            mode: 'single',
        },
        onCellClick(e) {
            if (e.columnIndex == 3 && typeof e.data != 'undefined') {
                window.open(e.data.Remarks, "_blank");
            }
            
        },
        editing: {
            allowDeleting: false
        },
        width:"100%"
    });
}


function loadWebLinks() {
    $.ajax({
        type: "POST",
        url: "/WebLinks/GetWebLinks",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response) {
                loadWebLinksGrid(response);
            } else {
                console.error("Error loading WebLinks.");
            }
        },
        error: function (error) {
            console.error("Error loading WebLinks: " + error);
        }
    });
}

function loadWebLinksGrid(oWebLinks) {
    $('#gridWebLinks').dxDataGrid({
        dataSource: JSON.parse(oWebLinks),
        keyExpr: 'ID',
        allowColumnResizing: true,
        showBorders: true,
        columnResizingMode: 'nextColumn',
        columnMinWidth: 50,
        columnAutoWidth: false,
        editing: {
            mode: 'row',
            allowAdding: true,
            newRowPosition: 'last',
        },
        onEditorPreparing: function (e) {
            if (e.dataType === "boolean" && e.value === undefined) {
                e.editorOptions.value = false;
            }
        },
        onRowInserting: function (e) {
            e.data.Position = e.component.totalCount() + 1
            const dataSource = e.component.getDataSource().items().slice(); //Creamos una copia del dataSource
            return $.ajax({
                url: "/WebLinks/SaveWebLink",
                data: JSON.stringify({ webLink: e.data }),
                method: 'POST',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    e.data.ID = result; //Asignamos el nuevo ID al registro
                    dataSource.push(e.data);
                    e.component.option('dataSource', dataSource);
                    e.component.refresh();
                }
            });
        },
        onRowUpdated: function (e) {
            saveWebLink(e.data);
        },
        onRowRemoved: function (e) {
            deleteWebLink(e.data);
        },
        rowDragging: {
            allowReordering: true,
            onReorder: function (e) {
                var visibleRows = e.component.getVisibleRows();
                var toIndex = visibleRows[e.toIndex].rowIndex;
                var fromIndex = visibleRows[e.fromIndex].rowIndex;

                var dataSource = e.component.getDataSource().items().slice(); //Creamos una copia del dataSource
                var item = dataSource.splice(fromIndex, 1)[0];
                dataSource.splice(toIndex, 0, item);

                //Actualizar el dataSource del dxDataGrid
                e.component.option('dataSource', dataSource);

                //Actualizar la posición de cada elemento
                var newOrder = dataSource.map(function (row, index) {
                    row.Position = index + 1;
                    return row;
                });
                saveNewOrder(newOrder);
                e.component.refresh();

            }
        },
        paging: {
            enabled: false,
        },
        remoteOperations: true,
        sorting: {
            mode: "none"
        },
        editing: {
            mode: 'popup',
            allowUpdating: true,
            allowAdding: true,
            allowDeleting: true,
            confirmDelete: false,
            useIcons: true,
            popup: {
                title: Globalize.formatMessage("optionsWLPopupTitle"),
                showTitle: true,
                width: 700,
                height: 670,
            },
            form: {
                labelLocation: "top",
                items: [{
                    itemType: 'group',
                    colCount: 1,
                    colSpan: 2,
                    items: ['Title', {
                        dataField: 'Description',
                        editorType: 'dxTextArea',
                        colSpan: 2,
                        editorOptions: {
                            height: 100,
                        }
                    }],
                }, {
                    itemType: 'group',
                    colCount: 2,
                    colSpan: 2,
                    items: ['LinkCaption', 'URL'],
                }, {
                    itemType: 'group',
                    colCount: 1,
                    colSpan: 2,
                    items: [{
                        dataField: 'ShowOnLiveDashboard',
                        caption: Globalize.formatMessage("optionsWLCaptionShowOnLiveDashboard"),
                        colSpan: 2,
                        label: {
                            location: "left",
                        },
                        dataType: "boolean",
                    }, {
                        dataField: 'ShowOnPortalDashboard',
                        caption: Globalize.formatMessage("optionsWLCaptionShowOnPortalDashboard"),
                        colSpan: 2,
                        dataType: "boolean",
                        label: {
                            location: "left",
                        }
                    }, {
                        dataField: 'ShowOnPortal',
                        caption: Globalize.formatMessage("optionsWLCaptionShowOnPortal"),
                        dataType: "boolean",
                        colSpan: 2,
                        label: {
                            location: "left",
                        }
                    }],
                }],
            },
        },
        columns: [
            {
                dataField: 'Title',
                caption: Globalize.formatMessage("optionsWLCaptionTitle"),
                validationRules: [{ type: 'required' }, {
                    type: 'stringLength',
                    max: 25,
                    //message: 'Tamaño máximo de 25 carácteres',
                }],
                width: 200,
            }, {
                dataField: 'Description',
                caption: Globalize.formatMessage("optionsWLCaptionDescription"),
                validationRules: [{ type: 'required' }, {
                    type: 'stringLength',
                    max: 250,
                }],
            }, {
                dataField: 'LinkCaption',
                caption: Globalize.formatMessage("optionsWLCaptionLink"),
                validationRules: [{ type: 'required' }, {
                    type: 'stringLength',
                    max: 15,
                }],
                width: 125,
            }, {
                dataField: 'URL',
                caption: Globalize.formatMessage("optionsWLCaptionURL"),
                validationRules: [{ type: 'required' }],
                width: 350,
            }, {
                dataField: 'ShowOnLiveDashboard',
                caption: Globalize.formatMessage("optionsWLCaptionShowOnLiveDashboard"),
                dataType: "boolean",
                width: 200,
            }, {
                dataField: 'ShowOnPortalDashboard',
                caption: Globalize.formatMessage("optionsWLCaptionShowOnPortalDashboard"),
                dataType: "boolean",
                width: 200,
            }, {
                dataField: 'ShowOnPortal',
                caption: Globalize.formatMessage("optionsWLCaptionShowOnPortal"),
                dataType: "boolean",
                width: 200,
            }, {
                type: 'buttons',
                buttons: ['edit', 'delete']
            }],
        toolbar: {
            items: [{
                name: 'addRowButton',
                /*options: {
                    text: 'Añadir nuevo enlace',
                },
                showText: 'always',*/
            }],
        },
        onRowInserted(e) {
            e.component.navigateToRow(e.key);
        },
    });
}

function saveWebLink(data) {
    $.ajax({
        type: "POST",
        url: "/WebLinks/SaveWebLink",
        data: JSON.stringify({ webLink: data }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (!response) {
                console.error("Error saving link.");
            }
        },
        error: function (error) {
            console.error("Error: " + error);
        }
    });
}

function deleteWebLink(data) {
    $.ajax({
        type: "POST",
        url: "/WebLinks/DeleteWebLink",
        data: JSON.stringify({ webLink: data }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (!response) {
                console.error("Error deleting link.");
            }
        },
        error: function (error) {
            console.error("Error: " + error);
        }
    });
}

function saveNewOrder(newOrder) {
    $.ajax({
        type: "POST",
        url: "/WebLinks/SaveNewOrder",
        data: JSON.stringify({ newOrder: newOrder }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (!response) {
                console.log("Error saving links order.");
            }
        },
        error: function (error) {
            console.error("Error: " + error);
        }
    });
}