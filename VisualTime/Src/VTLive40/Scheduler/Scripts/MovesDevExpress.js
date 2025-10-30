function showParentLoadingGrid(bIsShowingLoadingGrid) {
    if (typeof window.parent.frames['ifPrincipal'] != 'undefined' && typeof window.parent.frames['ifPrincipal'].showLoadingGrid == 'function')
        window.parent.frames['ifPrincipal'].showLoadingGrid(bIsShowingLoadingGrid);
}
function initialMovesNewLoad() {
    showParentLoadingGrid(true);
    ASPxMovesNewPanelClient.PerformCallback("INITIALLOAD")
}

function reLoadView() {
    if (!IsGridEditingAndWarn()) {
        showParentLoadingGrid(true);
        clearTimeout(_timeout);
        hasServerChanges("ASPxMovesNewPanelClient.PerformCallback('RELOADDATA')");
    }
}

function undoChanges(callbackFunction) {
    showParentLoadingGrid(true);
    clearTimeout(_timeout);

    if (typeof callbackFunction != 'undefined') {
        setTimeout(function () { eval(callbackFunction); }, 1000);
    } else {
        ASPxMovesNewPanelClient.PerformCallback('RELOADDATA');
    }
}

function hasServerChanges(onReturnCallback) {
    var jasonificado = jasonifica("ISCHANGEALLOWED", "", onReturnCallback, "TRUE");
    CallbackSessionClient.PerformCallback(jasonificado);
}

function navigateToPreviousDate() {
    if (!IsGridEditingAndWarn()) {
        showParentLoadingGrid(true);
        clearTimeout(_timeout);
        hasServerChanges("ASPxMovesNewPanelClient.PerformCallback('NAVIGATETOPREVIOUSDATE')");
    }
}

function navigateToNextDate() {
    if (!IsGridEditingAndWarn()) {
        showParentLoadingGrid(true);
        clearTimeout(_timeout);
        hasServerChanges("ASPxMovesNewPanelClient.PerformCallback('NAVIGATETONEXTDATE')");
    }
}

function navigateToPreviousEmployee() {
    if (!IsGridEditingAndWarn()) {
        showParentLoadingGrid(true);
        clearTimeout(_timeout);
        hasServerChanges("ASPxMovesNewPanelClient.PerformCallback('NAVIGATETOPREVIOUSEMPLOYEE')");
    }
}

function navigateToNextEmployee() {
    if (!IsGridEditingAndWarn()) {
        showParentLoadingGrid(true);
        clearTimeout(_timeout);
        hasServerChanges("ASPxMovesNewPanelClient.PerformCallback('NAVIGATETONEXTEMPLOYEE')");
    }
}

function navigateToPreviousSelector() {
    if (!IsGridEditingAndWarn()) {
        showParentLoadingGrid(true);
        clearTimeout(_timeout);
        hasServerChanges("ASPxMovesNewPanelClient.PerformCallback('NAVIGATETOPREVIOUSSELECTOR')");
    }
}

function navigateToNextSelector() {
    if (!IsGridEditingAndWarn()) {
        showParentLoadingGrid(true);
        clearTimeout(_timeout);
        hasServerChanges("ASPxMovesNewPanelClient.PerformCallback('NAVIGATETONEXTSELECTOR')");
    }
}

function navigateToSelectorChoice(s, e) {
    if (!IsGridEditingAndWarn()) {
        showParentLoadingGrid(true);
        clearTimeout(_timeout);
        hasServerChanges("ASPxMovesNewPanelClient.PerformCallback('NAVIGATETOSELECTORCHOICE')");
    }
}

function navigateToSelectedDate(s, e) {
    if (!IsGridEditingAndWarn()) {
        showParentLoadingGrid(true);
        clearTimeout(_timeout);
        hasServerChanges("ASPxMovesNewPanelClient.PerformCallback('CHANGEDATE')");
    }
}

function saveAndContinue(onReturnFunc) {
    hasMovesNewChanges = 1;
    SaveAll(onReturnFunc);
}
function discardAndContinue(onReturnFunc) {
    CanUndoAll(onReturnFunc);
}

function ASPxMovesNewPanel_EndCallBack(s, e) {
    showParentLoadingGrid(false);
    GetDailyScheduleStatus();

    if (s.cpActionRO == "RELOADDATA") {
        if (mainCallbackuFunction != '') {
            eval(mainCallbackuFunction);
        } else {
            if ((cmbViewClient.GetSelectedItem().value + '') != "2") {
                GridMovesClient.PerformCallback("ShowTerminal");
            } else {
                GridMovesClient.PerformCallback("ShowLocation");
            }
        }
    }
}

//===== BOTONES DE LA PANTALLA ==================================================
function GridCancelEditing() {
    try {
        if (GridIncidencesClient.IsEditing()) {
            GridIncidencesClient.CancelEdit();
        }
    }
    catch (e) {
        showError("IsGridEditing", e);
    }
}

//also called by MovesNew.aspx onclick
function IsGridEditingAndWarn() {
    try {
        if (typeof (GridIncidencesClient) != "undefined") {
            if (GridIncidencesClient.IsEditing()) {
                showPopupMoves2("DataEditing.Message", "INFO", "", "DataEditing.OK", "", "", "", "", "");
                return true;
            }
        }

        return false;
    }
    catch (e) {
        showError("IsGridEditing", e);
    }
}

function AcceptGridChangesLocally() {
    if (GridIncidencesClient.IsEditing()) {
        //Boto Aplicar quan s'està editant el grid.
        GridIncidencesClient.UpdateEdit();
    }
}

var bWaitingForSave = false;

function SaveAll(callbackFunction) {
    if (GridIncidencesClient.IsEditing()) {
        //Boto Aplicar quan s'està editant el grid.
        bWaitingForSave = true;
        GridIncidencesClient.UpdateEdit();
        waitForSave(callbackFunction);
        return;
    }

    //Boto Cerrar + Guardar
    //quan posem cerrar i diem de guardar, passa per aqui enlloc de ferho a traver de timer (si hi hagues IsGridEditing ja fa un popup i evita passar per aqui).
    //aqui no hi ha haver timer pq el popup despres fa un close i destrueix el timer.
    waitForSave(callbackFunction);

    //RefreshPageComplete(false);
}

var _timeout2 = -1;

function waitForSave(callbackFunction) {
    clearTimeout(_timeout2);
    if (!GridMovesClient.InCallback() && !GridIncidencesClient.InCallback()) {
        RealSave(callbackFunction);
    } else {
        _timeout2 = setTimeout(function () { waitForSave(callbackFunction); }, 1000);
    }
}

function RealSave(callbackFunction) {
    var grid;
    grid = ASPxClientGridView.Cast(GridIncidencesClient);
    grid.PerformCallback("SAVE_RELOAD");

    grid = ASPxClientGridView.Cast(GridMovesClient);
    grid.PerformCallback("SAVE_RELOAD");

    CallbackPanelRemarksClient.PerformCallback("SAVE_RELOAD");

    document.getElementById('ASPxMovesNewPanel_hdnClientStatus').value = "";
    hasMovesNewChanges = 1;

    if (typeof callbackFunction != 'undefined') {
        setTimeout(function () { eval(callbackFunction); }, 1000);
    }
}

function CanUndoAll(callbackFunction) {
    try {
        grid = ASPxClientGridView.Cast(GridIncidencesClient);
        grid.CancelEdit();
        undoChanges(callbackFunction);
    }
    catch (e) {
        showError("CanUndoAll", e);
    }
}

function UndoAll() {
    RefreshPageComplete(true);
}

var closeTimmer = -1;
var closeDialog = false;
function StatusCallback_CallbackComplete(s, e) {
    if (s.cpAction == "CANCLOSE") {
        if (s.cpResult) {
            var hdnMustRefresh_PageBase = document.getElementById("ASPxMovesNewPanel_hdnMustRefresh_PageBase");
            hdnMustRefresh_PageBase.value = "movesNew";
            document.getElementById("ASPxMovesNewPanel_hdnParams_PageBase").value = hasMovesNewChanges;
            Close();
        }
        else {
            //showPopupMoves2("SaveConfirmation.Title", "INFO", "",
            //    "SaveConfirmation.Answer.Yes.Title", "SaveConfirmation.Answer.Yes.Description", "window.frames[1].SaveAll(); window.frames[1].Close();",
            //    "SaveConfirmation.Answer.No.Title", "SaveConfirmation.Answer.No.Description", "window.frames[1].Close();",
            //    "SaveConfirmation.Answer.Cancel.Title", "SaveConfirmation.Answer.Cancel.Description", "");
            closeDialog = true;
        }
    }
}

function CanCloseMe(s, e) {
    try {
        closeDialog = false;
        closeTimmer = -1;
        // when is hdnMustRefresh_PageBase used? right now only seems to be setted.
        if (IsGridEditingAndWarn()) {
            showPopupMoves2("DataEditing.Message", "INFO", "", "DataEditing.OK", "", "", "", "", "");
        } else {
            hasServerChanges("ReallyClose()");
        }
    }
    catch (e) {
        showError("CanCloseMe", e);
    }
}

function ReallyClose() {
    try {
        showParentLoadingGrid(false);

        var hdnMustRefresh_PageBase = document.getElementById("ASPxMovesNewPanel_hdnMustRefresh_PageBase");
        hdnMustRefresh_PageBase.value = "movesNew";
        document.getElementById("ASPxMovesNewPanel_hdnParams_PageBase").value = hasMovesNewChanges;

        var strParameters = JSON.stringify({
            StampParam: new Date().getMilliseconds(),
            action: "CANCLOSE"
        });

        strParameters = encodeURIComponent(strParameters);
        if (!StatusCallbackClient.InCallback()) StatusCallbackClient.PerformCallback(strParameters);

        clearTimeout(closeTimmer);
        setTimeout(closeMovesNew, 1000);
    }
    catch (e) {
        showError("CanCloseMe", e);
    }
}

function closeMovesNew() {
    if (closeDialog == true) close();
    else {
        clearTimeout(closeTimmer);
        setTimeout(closeMovesNew, 1000);
    }
}

//==================================================================

function GetDailyScheduleStatus(bolSetTimeout) {
    if (typeof bolSetTimeout == 'undefined') bolSetTimeout = true;
    clearTimeout(_timeout);
    if (_timeout) _timeout = null;

    var IDEmployee = document.getElementById("ASPxMovesNewPanel_hdnIDEmployeePage").value;
    if (parseInt(IDEmployee, 10) < 0) return;

    var DateMove = document.getElementById("ASPxMovesNewPanel_hdnDatePage").value;
    if (DateMove == "") return;

    var view = document.getElementById("ASPxMovesNewPanel_hdnViewPage").value;
    if (parseInt(view, 10) < 0) return;

    var stamp = '&StampParam=' + new Date().getMilliseconds();

    var _ajax = nuevoAjax();
    _ajax.open("GET", "Handlers/srvMoves.ashx?action=getDailyScheduleStatus&IDEmployee=" + IDEmployee + "&View=" + view + "&DateMove=" + DateMove + stamp, true);
    _ajax.onreadystatechange = function () {
        if (_ajax.readyState == 4) {
            var Response = _ajax.responseText;
            var statusNumber = parseInt(Response, 10);

            if (isNaN(statusNumber)) {
                statusNumber = 0;
            }

            if (statusNumber == -1) {
                window.top.reenviaFrame('#Start', '', '', '');
                return;
            }

            if (statusNumber < 60) {
                //Bloqueamos incidencias y saldos
                var view = document.getElementById("ASPxMovesNewPanel_hdnViewPage").value;
                if (view != "2") {
                    GridCancelEditing();
                    ShowMyProgress();
                } else {
                    HideMyProgress();
                }

                document.getElementById('ASPxMovesNewPanel_hdnClientStatus').value = Response;
            } else if (statusNumber >= 60 && statusNumber < 70) {
                HideMyProgressAccruals();

                //si hdnClientStatus es -1 acaba de hacer el load y no es necesario recargar
                var hdnClientStatus = document.getElementById('ASPxMovesNewPanel_hdnClientStatus');
                if (hdnClientStatus.value == "-1") { hdnClientStatus.value = Response; }

                var bolRefresh = (hdnClientStatus.value != Response);
                if (bolRefresh) {
                    document.getElementById("ASPxMovesNewPanel_hdnClientStatus").value = Response;
                    //Refrescar Grid incidencias
                    grid = ASPxClientGridView.Cast(GridIncidencesClient);
                    if (!grid.InCallback()) grid.PerformCallback("RELOAD");
                }
            } else if (statusNumber >= 70) {
                HideMyProgress();
                //si hdnClientStatus es -1 acaba de hacer el load y no es necesario recargar
                var hdnClientStatus = document.getElementById('ASPxMovesNewPanel_hdnClientStatus');
                if (hdnClientStatus.value == "-1") { hdnClientStatus.value = Response; }

                var bolRefresh = (hdnClientStatus.value != Response);
                if (bolRefresh) {
                    document.getElementById("ASPxMovesNewPanel_hdnClientStatus").value = Response;

                    //Refrescar Grid acumulados
                    var grid = ASPxClientGridView.Cast(GridAcumClient);
                    if (!grid.InCallback()) grid.PerformCallback("RELOAD");

                    grid = ASPxClientGridView.Cast(GridIncidencesClient);
                    if (!grid.InCallback()) grid.PerformCallback("RELOAD");

                    //Refrescar Localizacion
                    if (!CallbackPanelLocalizationClient.InCallback()) CallbackPanelLocalizationClient.PerformCallback();
                }
            }

            showParentLoadingGrid(false);

            if (!bolSetTimeout || bolSetTimeout == null || bolSetTimeout == true) {
                _timeout = setTimeout(function () { refreshEditorTimer(); }, 3000); //Periodic call to server.
            }
        }
    }
    _ajax.send(null);
}

function refreshEditorTimer() {
    clearTimeout(_timeout);
    if (!GridMovesClient.InCallback() && !GridIncidencesClient.InCallback()) {
        GetDailyScheduleStatus();
    } else {
        _timeout = setTimeout(function () { refreshEditorTimer(); }, 3000);
    }
}

//=== MOVES =========================================================================

//Agregar nuevo fichaje desde el boton de Añadir
function AddNewMove() {
    LoadDetailsPage(true);
}

//Editar el fichaje al hacer doble click sobre la linea
function GridMoves_RowDblClick(s, e) {
    if (!IsGridEditingAndWarn()) {
        s.GetRowValues(e.visibleIndex, "LinkDetails", OpenMoveDetails);
    }
}
function GridMoves_CustomButtonClick(s, e) {
    if (e.buttonID == "ValidateMove") {
        e.processOnServer = true;
    }
}

function OpenMoveDetails(url) {
    document.getElementById("ASPxMovesNewPanel_hdnGridChanges").value = "0";
    LoadDetailsPage(false, url);
}

//Retorno de la pagina MoveDetail.aspx
function DetailsChange(option, type, aType, sDate, mDate, time, terminal, cause, action, idpassport, row, city, zone, position, task, idtask, daysToAdd, field1, field2, field3, field4, field5, field6, invalidType, typeDetails, center, reliable, telecommuting) {
    try {
        if (row == "") row = -1;

        document.getElementById("ASPxMovesNewPanel_hdnGridChanges").value = option;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsType").value = type;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsActualType").value = aType;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsShiftDate").value = sDate;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsMoveDate").value = mDate;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsMoveTime").value = time;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsIdTerminal").value = terminal;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsIdCause").value = cause;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsIdCenter").value = center;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsIdAction").value = action;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsIdPassport").value = idpassport;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsNumRow").value = row;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsCity").value = city;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsIdZone").value = zone;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsPosition").value = position;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsTask").value = task;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsIDTask").value = idtask;
        document.getElementById("ASPxMovesNewPanel_hdnDaysToAdd").value = daysToAdd;
        document.getElementById("ASPxMovesNewPanel_hdnSwitchReliable").value = reliable;
        document.getElementById("ASPxMovesNewPanel_hdnSwitchTelecommuting").value = telecommuting;

        document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField1Pop").value = field1;
        document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField2Pop").value = field2;
        document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField3Pop").value = field3;
        document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField4Pop").value = field4;
        document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField5Pop").value = field5;
        document.getElementById("PopDetails_CallbackPanelDetails_hdnDetailsField6Pop").value = field6;

        document.getElementById("ASPxMovesNewPanel_hdnDetailsInvalidtype").value = invalidType;
        document.getElementById("ASPxMovesNewPanel_hdnDetailsTypeDetails").value = typeDetails;

        if (type != "6") {
            //enviar datos del fichaje al servidor
            var miCallback = ASPxClientCallback.Cast("CallbackSessionClient");
            var jasonificado = jasonifica("SAVEMOVE", row);
            if (!miCallback.InCallback()) miCallback.PerformCallback(jasonificado);
        }
    }
    catch (e) {
        showError("DetailsChange", e);
    }
}

function GridMovesClient_beginCallback(s, c) {
    clearTimeout(_timeout);
}
//=== FIN MOVES ================================================================

//=== CALLBACKS END ============================================================
function GridIncidencesClient_EndCallback(s, e) {
    if (s.cpReturnValue != "") {
        refreshEditorTimer();

        var hdnMustRefresh_PageBase = document.getElementById("ASPxMovesNewPanel_hdnMustRefresh_PageBase");
        hdnMustRefresh_PageBase.value = "movesNew";
        document.getElementById("ASPxMovesNewPanel_hdnParams_PageBase").value = hasMovesNewChanges;
    }
}

function GridMovesClient_EndCallback(s, e) {
    if (s.cpReturnValue != "") {
        refreshEditorTimer();

        var hdnMustRefresh_PageBase = document.getElementById("ASPxMovesNewPanel_hdnMustRefresh_PageBase");
        hdnMustRefresh_PageBase.value = "movesNew";
        document.getElementById("ASPxMovesNewPanel_hdnParams_PageBase").value = hasMovesNewChanges;
    }

    showParentLoadingGrid(false);
}

function CallbackPanelRemarksClient_EndCallback(s, e) {
    if (s.cpReturnValue != "") {
        var hdnMustRefresh_PageBase = document.getElementById("ASPxMovesNewPanel_hdnMustRefresh_PageBase");
        hdnMustRefresh_PageBase.value = "movesNew";
        document.getElementById("ASPxMovesNewPanel_hdnParams_PageBase").value = hasMovesNewChanges;
    }
}
//=== FIN CALLBACKS END =========================================================

//==== INCIDENCIAS ==============================================================
//Agregar nueva fila en el grid de incidencias
function AddNewIncidence(s, e) {
    var grid = ASPxClientGridView.Cast("GridIncidencesClient");
    grid.AddNewRow();
}

//Eliminar una incidencia en el datatable del servidor
function DeleteIncidence(IdRow) {
    grid = ASPxClientGridView.Cast("GridIncidencesClient");
    grid.DeleteRow(IdRow);
}

function gridIncidences_BeginCallback(e, c) {
    clearTimeout(_timeout);
}

//==== FIN INCIDENCIAS ==========================================================

//======== FUNCIONES HELPER  ====================================================
//Retorno del cambio de variables de session en el servidor
function CallbackSession_CallbackComplete(s, e) {
    var strParameter = decodeURIComponent(e.parameter);
    var oParameter = JSON.parse(strParameter);

    switch (oParameter.accion) {
        case "SHOWTOTALACUM":
        case "SHOWDAILYACUM":
            var grid = ASPxClientGridView.Cast(GridAcumClient);
            grid.Refresh();
            break;

        case "VISTA":
            RefreshPageComplete(false);
            if (oParameter.scriptClient != "") {
                eval(oParameter.scriptClient);
            }
            break;

        case "ISCHANGEALLOWED":
            if (e.result == "TRUE") {
                if (oParameter.scriptClient != "") {
                    eval(oParameter.scriptClient);
                }
            } else {
                showParentLoadingGrid(false);
                Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
                    { text: '', key: 'roScheduleChangesTitle' }, { text: '', key: 'roScheduleChangesDesc' },
                    { text: '', textkey: 'roScheduleYes', desc: '', desckey: 'roScheduleYesDesc', script: 'saveAndContinue("' + oParameter.scriptClient + '")' },
                    { text: '', textkey: 'roScheduleNo', desc: '', desckey: 'roScheduleNoDesc', script: 'discardAndContinue("' + oParameter.scriptClient + '")' },
                    { text: '', textkey: 'roScheduleCancelar', desc: '', desckey: 'roScheduleCancelarDesc', script: '' },
                    Robotics.Client.JSErrors.createEmptyButton())
            }
            break;

        case "SAVEMOVE":
            if (e.result == "TRUE") {
                GridMovesClient.Refresh();
            }
            break;

        default:
        //nada
    }
}

//Refresca todas los componentes de la pagina al mismo tiempo
function RefreshPageComplete(bolReload) {
    //var Reload=false;

    try {
        reLoadView();
        //if (typeof (bolReload) != "undefined" && bolReload != null) Reload = bolReload;

        //if (Reload) {
        //    if (!GridMovesClient.InCallback()) GridMovesClient.PerformCallback("RELOAD");
        //} else {
        //    if (!GridMovesClient.InCallback()) GridMovesClient.Refresh();
        //}

        //if (!CallbackPanelShiftClient.InCallback()) CallbackPanelShiftClient.PerformCallback();

        //if (!CallbackPanelAbsenceClient.InCallback()) CallbackPanelAbsenceClient.PerformCallback();

        //if (Reload) {
        //    if (!GridIncidencesClient.InCallback()) GridIncidencesClient.PerformCallback("RELOAD");
        //} else {
        //    if (!GridIncidencesClient.InCallback()) GridIncidencesClient.Refresh();
        //}

        //if (Reload) {
        //    if (!GridAcumClient.InCallback()) GridAcumClient.PerformCallback("RELOAD");
        //} else {
        //    if (!GridAcumClient.InCallback()) GridAcumClient.Refresh();
        //}

        //if (!CallbackPanelRemarksClient.InCallback())CallbackPanelRemarksClient.PerformCallback();

        //if (!CallbackPanelLocalizationClient.InCallback())CallbackPanelLocalizationClient.PerformCallback();
    }
    catch (e) {
        showError("DetailsChange", e);
    }
}

//Retorno de pantallas modales (Refrescos de pantalla)
function RefreshScreen(MustRefresh, Params) {
    if (MustRefresh == '5') {
        switch (Params) {
            case "SAVE_PROGRAMMED_ABSENCE":
                var hdnMustRefresh_PageBase = document.getElementById("ASPxMovesNewPanel_hdnMustRefresh_PageBase");
                hdnMustRefresh_PageBase.value = "movesNew";
                document.getElementById("ASPxMovesNewPanel_hdnParams_PageBase").value = hasMovesNewChanges;
                RefreshPageComplete(false);
                break;

            default:
            //nada
        }
    }
}

//Comprobacion del retorno de una llamada Ajax
function checkStatus(oStatus) {
    try {
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") {
                var url = "Scheduler/srvMsgBoxScheduler.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            }
        }
    }
    catch (e) {
        showError("checkStatus", e);
    }
}

function jasonifica(Accion, Valor, scriptClient, showMessage) {
    var tmpAccion = "", tmpValor = "", tmpscriptClient = "", tmpshowMessage = "FALSE";

    if (typeof (Accion) != "undefined") tmpAccion = Accion;
    if (typeof (Valor) != "undefined") tmpValor = Valor;
    if (typeof (scriptClient) != "undefined") tmpscriptClient = scriptClient;
    if (typeof (showMessage) != "undefined") tmpshowMessage = showMessage;

    var oParameters = {};
    oParameters.accion = tmpAccion;
    oParameters.valor = tmpValor;
    oParameters.scriptClient = tmpscriptClient;
    oParameters.showMessage = tmpshowMessage;
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);
    return strParameters;
}

function MuestraSelector() {
    $("#ASPxMovesNewPanel_divSelector").toggle();
}

// Mostra popup Errors
function showPopupMoves2(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Scheduler/srvMsgBoxScheduler.aspx?action=Message";
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
    } catch (e) { showError("showPopupMoves2", e); }
}