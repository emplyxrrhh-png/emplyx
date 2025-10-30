function showLoadingGrid(loading) {
    try {
        parent.showLoader(loading);
    } catch (e) { showError("showLoadingGrid", e); }
}

function openTestEmail() {
    CheckEmailPopupClient.Show();
}

function closeTestEmail() {
    CheckEmailPopupClient.Hide();
}

function checkEmail() {
    testMailPanelClient.PerformCallback('CHECK_MAIL');
}

function btnDescargarLogs_Click() {
    var url = 'DownloadLogs.aspx';
    var Title = 'Logs';
    window.open(url);
}
function diagnosticsPanel_performCallback(key, values) {
    var strParameters = JSON.stringify({ Method: key, Parameters: values });
    strParameters = encodeURIComponent(strParameters);
    diagnosticsPanelClient.PerformCallback(strParameters)
}

function CallbackSessionClient_performCallback(key, values) {
    var strParameters = JSON.stringify({ Method: key, Parameters: values });
    strParameters = encodeURIComponent(strParameters);
    CallbackSessionClient.PerformCallback(strParameters)
}

function resetClientCache(s, e) {
    CallbackSessionClient_performCallback('resetClientCache');
}

function importVTCFile(s, e) {
    CallbackSessionClient_performCallback('importVTCFile', JSON.stringify({ content: vtcContentClient.GetText() }));
}

function saveMx9Parameter(s, e) {
    CallbackSessionClient_performCallback('saveMx9Parameter', JSON.stringify({ idTerminal: mx9IdTerminalClient.GetText(), name: txtMx9ParameterNameClient.GetText(), value: txtMx9ParameterValueClient.GetText() }));
}

function registerTerminalMT(s, e) {
    CallbackSessionClient_performCallback('registerTerminalMT', JSON.stringify({ terminalId: txtSerialNumberIdClient.GetText(), terminalType: cmbTerminalTypeClient.GetSelectedItem().value }));
}

function diagnosticsPanel_endCallback(s, e) {
    switch (s.cpActionRO) {
        case "LOGOUT":
            document.location = s.cpURLredirect;
            break;
        case "loadQuerySelector":
            loadQuerySelector_End(s);
            break;
        case "loadData":
            loadData_End(s);
            break;
        case "loadStateServer":
            loadStateServer_End(s);
            break;
        case "btnDisableHTTPS_Click":
            btnDisableHTTPS_Click_End(s);
            break;
        case "loadDisableHTTPS":
            loadDisableHTTPS_End(s);
            break;
        case "btnResetTerminal_Click":
            btnResetTerminal_Click_End(s);
            break;
        case "btnRebootTerminal_Click":
            btnRebootTerminal_Click_End(s);
            break;
        case "registerTerminalMT":
            btnRegisterTerminalMT_End(s);
            break;
        case "importVTCFile":
            importVTCFile_End(s);
            break;
        case "resetClientCache":
            resetClientCache_End(s);
            break;
        case "saveMx9Parameter":
            saveMx9Parameter_End(s);
            break;
    }
}

function testMailPanel_endCallback(s, e) {
    setTimeout(function () { showLoadingGrid(false); }, 100);
}

function loadQuerySelector_Start() {
    if ($('#querySelectorPlaceholder select').length === 0)
        CallbackSessionClient_performCallback('loadQuerySelector');
    else {
        activateTabEnd('Queries');
    }
    activateTabStart('Queries');
    $('#descQuery').dxTextBox({
        height: 20,
        width: "100%",
        readOnly: true
    });
}
function loadQuerySelector_End(s) {
    if (!s.cpStatus.Correct)
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);
    else {
        var selectParams = {};
        $('#querySelector').dxSelectBox({
            height: 20,
            dataSource: s.cpData.map(function (o) { return { name: o.Name, value: { id: o.Id, parameters: o.Parameters, description: o.Description } }; }),
            displayExpr: "name",
            valueExpr: "value",
            dropDownButtonTemplate: "dropDownButton",
            onValueChanged: function (o) {
                selectParams = {};
                $('#descQuery').dxTextBox({
                    height: 20,
                    width: "100%",
                    value: o.value.description,
                    readOnly: true
                });
                $('#dxGrid').remove();
                $('<div id="dxGrid"/>').appendTo($('#grid'));
                selectParams["id"] = o.value.id;
                $('#querySelectorPlaceholder div#parameters div').remove();
                $(o.value.parameters).each(function () {
                    var oCurrentParam = this;

                    var mainDiv = $('<div/>');
                    var divRow = $('<div class="divRow">').appendTo(mainDiv);
                    var divRowDescription = $('<div class="divRowDescription" id="descQuery">').appendTo(divRow);
                    var spanDescription = $('<span class="labelForm" style="margin-top: 6px;"/>').appendTo(divRow);
                    spanDescription.text(oCurrentParam.Description + ': ');
                    var componentDiv = $('<div class="componentForm">').appendTo(divRow);
                    var div = $('<div class="dx-field">').appendTo(componentDiv);
                    mainDiv.appendTo($('#querySelectorPlaceholder div#parameters'));
                    var type = "";



                    switch (oCurrentParam.Type) {
                        case "Decimal":
                            selectParams[oCurrentParam.Name] = 0;
                            div.dxNumberBox({
                                height: 20,
                                width: "100%",
                                'name': oCurrentParam.Name,
                                valueChangeEvent: "keyup",
                                onKeyPress: function (e) {
                                    var event = e.jQueryEvent,
                                        str = String.fromCharCode(event.keyCode);
                                    if (!/[0-9.,]/.test(str))
                                        event.preventDefault();
                                },
                                onValueChanged: function (data) {
                                    selectParams[oCurrentParam.Name] = data.value;
                                }
                            });
                            break;
                        case "Integer":
                            selectParams[oCurrentParam.Name] = 0;
                            div.dxNumberBox({
                                height: 20,
                                width: "100%",
                                'name': oCurrentParam.Name,
                                valueChangeEvent: "keyup",
                                onKeyPress: function (e) {
                                    var event = e.jQueryEvent,
                                        str = String.fromCharCode(event.keyCode);
                                    if (!/[0-9]/.test(str))
                                        event.preventDefault();
                                },
                                onValueChanged: function (data) {
                                    selectParams[oCurrentParam.Name] = data.value;
                                }
                            });
                            break;
                        case "Date":
                            var currentDate = new Date();
                            selectParams[oCurrentParam.Name] = moment(currentDate).format("DD/MM/YYYY");
                            div.dxDateBox({
                                height: 20,
                                width: "100%",
                                'name': oCurrentParam.Name,
                                "pickerType": "native",
                                "type": "date",
                                'value': currentDate,
                                onValueChanged: function (data) {
                                    selectParams[oCurrentParam.Name] = moment(data.value).format("DD/MM/YYYY");
                                }
                            });
                            break;
                        case "Time":
                            var currentDate = new Date();
                            selectParams[oCurrentParam.Name] = moment(currentDate).format("HH:mm");
                            div.dxDateBox({
                                height: 20,
                                width: "100%",
                                'name': oCurrentParam.Name,
                                "pickerType": "native",
                                "type": "time",
                                'value': currentDate,
                                onValueChanged: function (data) {
                                    selectParams[oCurrentParam.Name] = moment(data.value).format("HH:mm");
                                }
                            });
                            break;
                        case "Text":
                            selectParams[oCurrentParam.Name] = "";
                            div.dxTextBox({
                                height: 20,
                                width: "100%",
                                'name': oCurrentParam.Name,
                                onValueChanged: function (data) {
                                    selectParams[oCurrentParam.Name] = data.value;
                                }
                            });
                            break;
                        default:
                            break;
                    }
                });
            }
        });
        $('#sendQuery').dxButton({
            height: 20,
            text: "Enviar",
            onClick: function () {
                CallbackSessionClient_performCallback('loadData', JSON.stringify(selectParams));
            }
        });
    }

    activateTabEnd('Queries');
}

function loadData_End(s) {
    if (!s.cpStatus.Correct)
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);

    $("#dxGrid").dxDataGrid({
        dataSource: s.cpData.Data,
        allowColumnResizing: true,
        columnResizingMode: "widget",
        columnAutoWidth: true,
        scrolling: {
            mode: "virtual"
        },
        filterRow: {
            visible: true
        },
        filterPanel: {
            visible: true,
        },
        height: $('#divContenido').height() - $('#querySelectorPlaceholder').outerHeight(true) - 50,
        columns: s.cpData.Columns
    });
}

function loadStateServer_Start() {
    activateTabStart('Status');
    showLoadingGrid(true);
    diagnosticsPanel_performCallback('loadStateServer');
}
function loadStateServer_End(s) {
    showLoadingGrid(false);
    activateTabEnd('Status');
}

function loadActions_Start() {
    activateTab('Actions');
}

function activateTab(tabCode) {
    activateTabStart(tabCode);
    activateTabEnd(tabCode);
}

function activateTabStart(tabCode) {
}

function activateTabEnd(tabCode) {
    //$('#diagnosticsPanel_div01').hide();
    //$('#div2').hide();
    //$('#divActions').hide();

    //switch (tabCode) {
    //    case 'Status':
    //        $('#diagnosticsPanel_div01').show();
    //        break;
    //    case 'Actions':
    //        $('#divActions').show();
    //        break;
    //    case 'Queries':
    //        $('#div2').show();
    //        break;
    //}
}

function btnDisableHTTPS_Click() {
    CallbackSessionClient_performCallback('btnDisableHTTPS_Click');
}
function btnDisableHTTPS_Click_End(s) {
    if (!s.cpStatus.Correct)
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);

    $("#lblDisableHTTPS").text(s.cpData.LabelText);
    $("#btnDisableHTTPS").val(s.cpData.ButtonText);
}

function btnResetTerminal_Click() {
    var terminalId = $("#ctl00_contentMainBody_txtTerminal_I").val();
    if (terminalId === "") return diagAlert("Debe seleccionar un identificador de terminal", false);
    if (!confirm("Esta acción puede inutilizar el terminal durante un largo periodo de tiempo. ¿Está seguro que desea continuar?")) return false;

    CallbackSessionClient_performCallback("btnResetTerminal_Click", JSON.stringify({ terminalId: terminalId }));
}
function btnResetTerminal_Click_End(s) {
    if (!s.cpStatus.Correct)
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);
    else
        diagAlert("Volcado completo de terminal en proceso.", true);

    $("#ctl00_contentMainBody_txtTerminal_I").val("");
}

function btnRebootTerminal_Click() {
    var terminalId = $("#ctl00_contentMainBody_txtTerminalReboot_I").val();
    if (terminalId === "") return diagAlert("Debe seleccionar un identificador de terminal", false);
    if (!confirm("El terminal se reiniciará. ¿Está seguro que desea continuar?")) return false;

    CallbackSessionClient_performCallback("btnRebootTerminal_Click", JSON.stringify({ terminalId: terminalId }));
}
function btnRebootTerminal_Click_End(s) {
    if (!s.cpStatus.Correct)
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);
    else
        diagAlert("Reinicio del terminal en proceso", true);

    $("#ctl00_contentMainBody_txtTerminalReboot_I").val("");
}

function saveMx9Parameter_End(s) {
    if (!s.cpStatus.Correct) {
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);
    } else {
        txtMx9ParameterNameClient.SetText('');
        txtMx9ParameterValueClient.SetText('');
        diagAlert("Se ha importado correctamente el fichero VTC", true);
    }
}

function importVTCFile_End(s) {
    if (!s.cpStatus.Correct) {
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);
    } else {
        vtcContentClient.SetText('');
        diagAlert("Se ha importado correctamente el fichero VTC", true);
    }
}

function resetClientCache_End(s) {
    if (!s.cpStatus.Correct)
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);
    else
        diagAlert("Se ha reseteado la cache correctamente", true);
}

function btnRegisterTerminalMT_End(s) {
    if (!s.cpStatus.Correct)
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);
    else
        alert(s.cpStatus.Message, true);
}

function loadDisableHTTPS() {
    CallbackSessionClient_performCallback('loadDisableHTTPS');
}
function loadDisableHTTPS_End(s) {
    if (!s.cpStatus.Correct)
        diagAlert("Se ha producido un error. \r\n" + s.cpStatus.Message, false);

    $("#lblDisableHTTPS").text(s.cpData.LabelText);
    $("#btnDisableHTTPS").val(s.cpData.ButtonText);
}

function diagAlert(text, bCorrect) {
    if (!bCorrect) {
        Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
            { text: '', key: 'roJsError' }, { text: text, key: '' },
            { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
            Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
    } else {
        Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsSuccess, '',
            { text: '', key: 'roJsSuccess' }, { text: text, key: '' },
            { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
            Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
    }
}