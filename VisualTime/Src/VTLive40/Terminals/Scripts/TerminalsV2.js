var actualTerminalImage = "";
var jsGridSirens = null;
var firstLoad = false;

function checkTerminalEmptyName(newName) {
    document.getElementById("readOnlyNameTerminal").textContent = newName;    
    hasChanges(true);
}

function cargaTerminals(id) {
    showLoadingGrid(true);
    actualType = "T";
    actualTerminal = id;
    cargaTerminalsTabSuperior(actualTerminal);
}

function cargaTerminalsTabSuperior(IDTerminals) {
    try {
        var Url = "Handlers/srvTerminals.ashx?action=getTerminalTab&aTab=" + actualTabTerminal + "&ID=" + IDTerminals;
        AsyncCall("POST", Url, "CONTAINER", "divTerminals", "cargaTerminalsBarButtons(" + IDTerminals + ");");
    }
    catch (e) {
        showError("cargaTerminalsTabSuperior", e);
    }
}
var responseObj = null;
function cargaTerminalsBarButtons(IDTerminals) {
    try {
        var Url = "Handlers/srvTerminals.ashx?action=getBarButtons&ID=" + IDTerminals;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDTerminals + ")");
    }
    catch (e) {
        showError("cargaTerminalsTabSuperior", e);
    }
}

function parseResponseBarButtons(oResponse, IDTerminals) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaTerminalsDivs(IDTerminals);
}

function cargaTerminalsDivs(IDTerminals) {
    try {
        var oParameters = {};
        oParameters.aTab = actualTabTerminal;
        oParameters.ID = actualTerminal;
        oParameters.oType = "T";
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "GETTERMINALS";
        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    }
    catch (e) {
        showError("cargaTerminalsDivs", e);
    }
}

function hasChangesTerminals(bolChanges, IdReader, FieldModified) {
    var divTop = document.getElementById('divMsgTop');
    var divBottom = document.getElementById('divMsgBottom');

    var tagHasChanges = document.getElementById('msgHasChanges');
    var msgChanges = '<changes>';
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setMessage(msgChanges);

    if (bolChanges) {
        if (IdReader != null) {
            if (validateReaders(IdReader, FieldModified)) {
                //oTerminal.validateTerminal();
            }
        } else {
            //oTerminal.validateTerminal();
        }

        divTop.style.display = '';
        divBottom.style.display = '';
        document.getElementById('divContentPanels').className = "divContentPanelsWithMessage";
    } else {
        divTop.style.display = 'none';
        divBottom.style.display = 'none';
        document.getElementById('divContentPanels').className = "divContentPanelsWithOutMessage";
    }
}

function validateReaders(IdReader, FieldModified) {
    try {
        showLoadingGrid(true);
        var oParameters = {};
        oParameters.aTab = actualTabTerminal;
        oParameters.ID = actualTerminal;
        oParameters.oType = "T";
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "VALIDATETERMINALREADER";
        oParameters.FieldModified = FieldModified
        oParameters.IdReader = IdReader

        var arrSirens = null;
        if (jsGridSirens != null) { arrSirens = jsGridSirens.toJSONStructureAdvanced(); }

        oParameters.sirens = arrSirens;

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
        ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
    }
    catch (e) {
        showError("cargaTerminalsDivs", e);
    }
}

function saveChangesTerminals() {
    if (document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_legacyModeEnabled").value.toUpperCase() == "TRUE") {
        showCaptchaRestrictionWarning();
    } else {
        saveDefChangesTerminals();
    }
}

function saveDefChangesTerminals() {
    try {
        if (ASPxClientEdit.ValidateGroup(null, true)) {
            showLoadingGrid(true);
            var oParameters = {};
            oParameters.aTab = actualTabTerminal;
            oParameters.ID = actualTerminal;
            oParameters.oType = "T";
            oParameters.gridTags = actualDxGridTags;
            oParameters.Name = document.getElementById('readOnlyNameTerminal').textContent;
            oParameters.StampParam = new Date().getMilliseconds();
            oParameters.action = "SAVETERMINAL";

            var arrSirens = null;
            if (jsGridSirens != null) { arrSirens = jsGridSirens.toJSONStructureAdvanced(); }

            oParameters.sirens = arrSirens;

            var strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
            ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);            
        } else {
            showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
        };
    } catch (e) { showError("saveChangesTerminals", e); }
}

function undoChangesTerminals() {
    try {
        showLoadingGrid(true);
        cargaTerminals(actualTerminal);
    } catch (e) { showError("undoChangesTerminals", e); }
}

function changeTerminalsTabs(numTab) {
    var arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02');
    var arrDivs = new Array('div01', 'div02', 'div03');

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
    actualTabTerminal = numTab;

    //Funcio per recarrega del posicionament del Flash (si es mostra)
    if (actualTabTerminal == 1) { loadTabZones(); }
}

function changeTabsByNameTerminals(nameTab) {
    var arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02');
    vararrDivs = new Array('div01', 'div02', 'div03');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + arrDivs[n]);
        if (div.id == nameTab) {
            if (tab != null) tab.className = 'bTab-active';
            if (div != null) div.style.display = '';
            actualTabTerminal = n;
        } else {
            if (tab != null) tab.className = 'bTab';
            if (div != null) div.style.display = 'none';
        }
    }

    if (actualTabTerminal == 1) { loadTabZones(); }
}

async function ShowSelector(objPrefix, oSel, bolhasChanges) {
    try {
        if (oSel == 1) {
            document.getElementById(objPrefix + '_gBoxWhoPunches_aFEmployees').textContent = document.getElementById('lblAllEmp').value;
            document.getElementById(objPrefix + '_gBoxWhoPunches_hdnEmployees').value = "ALL";
            document.getElementById(objPrefix + '_gBoxWhoPunches_hdnFilter').value = "";
            document.getElementById(objPrefix + '_gBoxWhoPunches_hdnFilterUser').value = "";
            if (bolhasChanges != false) { hasChanges(true); }
        }
        else {
            await getroTreeState("objContainerTreeV3_" + objPrefix + "_AccFilterTreeTermLimitEmpGrid").then(roState => roState.reset());
            document.getElementById('ctlTreeSelectorPrefix').value = objPrefix;

            if (window.addEventListener) //Firefox
                navigatorIndex = "0";
            else //IE
                navigatorIndex = "1";

            var prefixCookie = "objContainerTreeV3_" + objPrefix + "_AccFilterTreeTermLimitEmpGrid";
            var prefixTree = objPrefix + "_AccFilterTreeTermLimitEmp";
            var returnFunc = "GetSelectedTreeV3";
            var prefix = "";

            var strBase = "Base/Popups/EmployeeSelectorPopup.aspx?PrefixTree=" + prefixTree + "&PrefixCookie=" + prefixCookie + "&AfterSelectFuncion=" + returnFunc + "&Prefix=" + prefix + "&NavigatorIndex=" + navigatorIndex;
            parent.ShowExternalForm2(strBase, 500, 400, "", "", false, false);
        }
    }
    catch (e) {
        showError("ShowSelector", e);
    }
}

function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
    var objPrefix = document.getElementById('ctlTreeSelectorPrefix').value;
    if (oParm1 == "") {
        document.getElementById(objPrefix + '_gBoxWhoPunches_hdnEmployees').value = "ALL";
        document.getElementById(objPrefix + '_gBoxWhoPunches_hdnFilter').value = "";
        document.getElementById(objPrefix + '_gBoxWhoPunches_hdnFilterUser').value = "";
    }
    else {
        document.getElementById(objPrefix + '_gBoxWhoPunches_hdnEmployees').value = oParm1;
        document.getElementById(objPrefix + '_gBoxWhoPunches_hdnFilter').value = oParm2;
        document.getElementById(objPrefix + '_gBoxWhoPunches_hdnFilterUser').value = oParm3;
    }

    if (document.getElementById(objPrefix + '_gBoxWhoPunches_hdnEmployees').value == "ALL")
        document.getElementById(objPrefix + '_gBoxWhoPunches_aFEmployees').textContent = document.getElementById('lblAllEmp').value; //todos
    else
        document.getElementById(objPrefix + '_gBoxWhoPunches_aFEmployees').textContent = document.getElementById('lblEmpSelect').value; //Seleccionar
    
    hasChanges(true);
}

function timezoneChange(s, e) {
    var timezone = s.GetSelectedItem().value.split("_")[1]

    if (timezone == "1") {
        EnableElement(document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_chkTimeZone_chkAutoDaylight"))
    } else {
        DisableElement(document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_chkTimeZone_chkAutoDaylight"))
    }
}

function loadTabZones() {
    try {
        //Recorrem el TabContainerClient
        for (var n = 0; n < 8; n++) {
            var tab = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_tab0' + n);
            if (tab != null) {
                if (tab.style.display == "") { //Es el tab visible, executem loadFlashZones
                    if (tab.className == "tabHeader-Active") {
                        loadFlashZones(n);
                        break;
                    }
                } else { //Si no es visible, es que ja no hi han mes
                    break;
                }
            }
        }
    } catch (e) { showError("loadTabZones", e); }
}

function ctlTabPositions(objPrefix, tabpos) {
    try {
        actualZoneReader = (tabpos + 1);
        actualZoneDirection = 1;
        //Fet perque no es posiciona al cambiar amb els tabs (flash readers)
        loadFlashZones((tabpos + 1));
    } catch (e) { showError("ctlTabPositions", e); }
}

function ctlTabZonePositions(objPrefix, tabpos) {
    try {
        actualZoneDirection = (tabpos + 1);
        //Fet perque no es posiciona al cambiar amb els tabs (flash readers)
        loadFlashZones(actualZoneReader, (tabpos + 1));
    } catch (e) { showError("ctlTabPositions", e); }
}

function changeVertTabs(numTab, objPrefix) {
    try {
        var arrButtons = new Array();
        var arrDivs = new Array();
        arrButtons = retElementsByName('VertBtn_' + objPrefix, 'A');
        arrDivs = retElementsByName('VertDiv_' + objPrefix, 'DIV');

        for (n = 0; n < arrButtons.length; n++) {
            if (n == numTab) {
                arrButtons[n].className = 'tabVertActive';
                arrDivs[n].style.display = '';
                if (n.toString() == "2") {
                    actualZoneDirection = 1;
                    ctlTabZonePositions(objPrefix + '_tabZones', 0);
                    loadFlashZones(objPrefix.replace("ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR", ""));
                }
            } else {
                arrButtons[n].className = 'tabVertInactive';
                arrDivs[n].style.display = 'none';
            }
        }
    } catch (e) { showError("changeVertTabs", e); }
}

function loadFlashZones(IDReader, IDTabZone) {
    try {
        if (typeof (IDTabZone) == 'undefined' || (typeof (IDTabZone) != 'undefined' && IDTabZone == 1)) {
            var argsStr = "";
            var oPositions = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlPositionIn').value;
            var oReaders = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlReadersIn').value;
            var oIDPlane = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlZoneImgIn').value;
            //var oActualReader = document.getElementById('tabCtl01_frmTR' + IDReader + '_FlActualReader').value;
            var oActualReader = IDReader;

            argsStr = oPositions + "*|*" + oReaders + "*|*" + oActualReader;

            var action = "REPOSITION";
            var args = argsStr;

            //if (oIDPlane != "") {
            action = "RELOADBGANDCHANGE";
            args = oIDPlane + "_" + args;
            //}

            setTimeout("ReaderMap_DoFSCommand('ReaderMap" + IDReader + "In','" + action + "', '" + args + "');", 800);
        } else {
            var argsStr = "";
            var oPositions = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlPositionOut').value;
            var oReaders = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlReadersOut').value;
            var oIDPlane = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader + '_FlZoneImgOut').value;
            //var oActualReader = document.getElementById('tabCtl01_frmTR' + IDReader + '_FlActualReader').value;
            var oActualReader = IDReader;

            argsStr = oPositions + "*|*" + oReaders + "*|*" + oActualReader;

            var action = "REPOSITION";
            var args = argsStr;

            //if (oIDPlane != "") {
            action = "RELOADBGANDCHANGE";
            args = oIDPlane + "_" + args;
            //}

            setTimeout("ReaderMap_DoFSCommand('ReaderMap" + IDReader + "Out','" + action + "', '" + args + "');", 800);
        }
    } catch (e) { showError("loadFlashZones", e); }
}

function loadReaders(activeReaders) {
    try {
        for (var index = 0; index < activeReaders; index++) {
            var activeReader = (index + 1);
            
            InitializeReaders('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + activeReader);
            ConvertControls('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + activeReader + '_optChkCustomButtons_panContainer');
            EnableReaderControls(activeReader);
        }
    } catch (e) { showError("TerminalData:loadFlashReaders", e); }
}

function EnableReaderControls(IDReader, tBehaviour) {
    var prefix = 'ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + IDReader;
    var terminalBehaviour = "";
    if (typeof (tBehaviour) == 'undefined') {
        terminalBehaviour = document.getElementById(prefix + '_gBoxWhatPunches_hdnTerminalModeSelected').value;
    } else {
        document.getElementById(prefix + '_gBoxWhatPunches_hdnTerminalModeSelected').value = tBehaviour;
        terminalBehaviour = tBehaviour;
    }

    if (terminalBehaviour != "") {
        var mode = terminalBehaviour.split("_")[0];

        if (mode[6] == "0") {
            eval("cmbCostCentersClient" + IDReader + ".SetValue(null);");
            eval("cmbCostCentersClient" + IDReader + ".SetEnabled(false);");
        } else {
            eval("cmbCostCentersClient" + IDReader + ".SetEnabled(true);");
        }

        var activityEnabled = terminalBehaviour.split("_")[3];
       
        var zonesEnabled = terminalBehaviour.split("_")[5];

        if (zonesEnabled == "2") {
            document.getElementById(prefix + '_tabZones_tdTitle2').style.display = '';
        } else {
            ctlTabZonePositions(prefix + '_tabZones', 0);
            actualZoneDirection = 1;
            document.getElementById(prefix + '_tabZones_tdTitle2').style.display = 'none';
        }

        var relesEnabled = terminalBehaviour.split("_")[6];

        if (relesEnabled == "0") {
            document.getElementById(prefix + '_optChkOutPut_chkButton').checked = false;
            document.getElementById(prefix + '_optChkInvalidOutPut_chkButton').checked = false;
            disableChildElements(prefix + '_optChkOutPut_panOptionPanel');
            disableChildElements(prefix + '_optChkInvalidOutPut_panOptionPanel');
        } else {
            firstLoad = true;
            venableOPC(prefix + '_optChkOutPut');
            venableOPC(prefix + '_optChkInvalidOutPut');
            linkOPCItems(prefix + '_optChkOutPut,' + prefix + '_optChkInvalidOutPut');
            firstLoad = false;
        }

        if (terminalBehaviour.split("_")[7] == "1" && document.getElementById(prefix + '_tabZones_hdnCmbPosZoneInSelection').value.split("_")[4] == "False") {
            eval("cmbPosZoneInClient" + IDReader + ".SetValue(null);");
        }

        if (terminalBehaviour.split("_")[8] == "0" && document.getElementById(prefix + '_tabZones_hdnCmbPosZoneOutSelection').value.split("_")[4] == "True") {
            eval("cmbPosZoneOutClient" + IDReader + ".SetValue(null);");
        }
    }
   }


function posZoneInIndexChanged(s, e) {
    var value = s.GetSelectedItem().value.split("_");

    var prefix = 'ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + actualZoneReader;
    var zone1Direction = document.getElementById(prefix + '_gBoxWhatPunches_hdnTerminalModeSelected').value.split("_")[7];
    document.getElementById(prefix + '_tabZones_hdnCmbPosZoneInSelection').value = value;

    if (zone1Direction == "1" && value[4] == "False") {
        showErrorPopup("Error.WrongZoneTitle", "error", "Error.NonWorkingZoneOnInput", "", "Error.OK", "Error.OKDesc", "");
        s.SetValue(null);
    } else {
        eval("changePosZoneIn('" + value[1] + "','" + value[2] + "','" + value[3] + "');");
    }
}

function repositionReadersIn(oParms) {
    //changePosReader
    changePosReaderIn(actualZoneReader, oParms);
    loadFlashZones(actualZoneReader, actualZoneDirection);
}

function ShowZoomZoneOut(IDReader, FixPoint) {
    try {
        if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value == "true" && FixPoint == "true") { return; }
        var Title = '';
        var oID = IDReader.replace("ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR", "");
        var ImageID = "";

        var strParams = "";

        var Parm0 = document.getElementById(IDReader + '_FlPositionOut').value;
        var Parm1 = document.getElementById(IDReader + '_FlReadersOut').value;
        var Parm2 = document.getElementById(IDReader + '_FlActualReaderOut').value;
        ImageID = document.getElementById(IDReader + '_FlZoneImgOut').value;

        strParams = Parm0 + "*|*" + Parm1 + "*|*" + Parm2;

        var w = document.body.clientWidth - 50;
        var h = document.body.clientHeight;

        top.ShowExternalForm2('Terminals/ZoomZone.aspx?IDReader=' + oID + "&FixPoint=" + FixPoint + "&strParams=" + encodeURIComponent(strParams) + "&ImageID=" + ImageID, w, h, Title, '', false, false);
    } catch (e) { showError("ShowZoomZoneOut", e); }
}

function posZoneOutIndexChanged(s, e) {
    var value = s.GetSelectedItem().value.split("_");
    var prefix = 'ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + actualZoneReader;

    var zone2Direction = document.getElementById(prefix + '_gBoxWhatPunches_hdnTerminalModeSelected').value.split("_")[8];
    document.getElementById(prefix + '_tabZones_hdnCmbPosZoneOutSelection').value = value;

    if (zone2Direction == "0" && value[4] == "True") {
        showErrorPopup("Error.WrongZoneTitle", "error", "Error.WorkingZoneOnOutput", "", "Error.OK", "Error.OKDesc", "");
        s.SetValue(null);
    } else {
        eval("changePosZoneOut('" + value[1] + "','" + value[2] + "','" + value[3] + "');");
    }
}

function repositionReadersOut(oParms) {
    changePosReaderOut(actualZoneReader, oParms);
    loadFlashZones(actualZoneReader, actualZoneDirection);
}

function InitializeReaders(objPrefix) {
    try {
        firstLoad = true;
        venableOPC(objPrefix + '_gBoxHowPunches_optHPFast');
        venableOPC(objPrefix + '_gBoxHowPunches_optHPInteractive');
        linkOPCItems(objPrefix + '_gBoxHowPunches_optHPFast,' + objPrefix + '_gBoxHowPunches_optHPInteractive');

        venableOPC(objPrefix + '_optServerLocal');
        venableOPC(objPrefix + '_optLocalServer');
        venableOPC(objPrefix + '_optLocal');
        venableOPC(objPrefix + '_optServer');
        linkOPCItems(objPrefix + '_optServerLocal,' + objPrefix + '_optLocalServer,' + objPrefix + '_optLocal,' + objPrefix + '_optServer');

        venableOPC(objPrefix + '_optChkCustomButtons');
        venableOPC(objPrefix + '_optChkPrintTicket');
        firstLoad = false;
    } catch (e) { showError("InitializeReaders", e); }
}

//function reloadflBg(oID) {
//    StatusMap_DoFSCommand('RELOADBG', oID);
//    if (actualTerminal < 1) { return; }
//    actualTerminalImage = oID;
//}

function createSirensGrids(arrCC) {
    try {
        try {
            var hdGridSiren = [{ 'fieldname': 'WeekDayName', 'description': '', 'size': 'auto' },
            { 'fieldname': 'Hour', 'description': '', 'size': '70px' },
            { 'fieldname': 'Duration', 'description': '', 'size': '70px' }];

            hdGridSiren[0].description = document.getElementById('hdnLngWeekdayName').value;
            hdGridSiren[1].description = document.getElementById('hdnLngHour').value;
            hdGridSiren[2].description = document.getElementById('hdnLngDuration').value;

            var edtRow = false;
            var delRow = false;

            if (document.getElementById('ctl00_contentMainBody_hdnModeEdit').value != "true") {
                edtRow = true;
                delRow = true;
            }

            jsGridSirens = new jsGrid('grdSirens', hdGridSiren, arrCC[0].sirens, edtRow, delRow, false, 'Sirens');
        } catch (e) { showError("TerminalData:createGridSiren", e); }
    } catch (e) { showError('TerminalData:createSirensGrids', e); }
}

function AddNewSiren() {
    try {
        document.getElementById('hdnAddSirenIDRow').value = ""; 
        frmAddSiren_ShowNew();
    } catch (e) { showError("AddNewSiren", e); }
}

function editGridSirens(idRow) {
    try {
        var arrValues = new Array();
        arrValues = jsGridSirens.retRowJSON(idRow);

        document.getElementById('hdnAddSirenIDRow').value = idRow;

        frmAddSiren_Show(arrValues);
    } catch (e) { showError("editGridSirens", e); }
}

function updateAddSirenRow(rowID, arrValues) {
    try {
        if (jsGridSirens == null) { createGridSirens(); }
        if (rowID == "") {
            jsGridSirens.createRow(arrValues, true);
        } else {
            jsGridSirens.editRow(rowID, arrValues);
        }
    } catch (e) { showError("updateAddSirenRow", e); }
}

function deleteGridSirens(idRow) {
    try {
        var url = "Terminals/srvMsgBoxTerminals.aspx?action=Message";
        url = url + "&TitleKey=deleteSirensDef.Title";
        url = url + "&DescriptionKey=deleteSirensDef.Description";
        url = url + "&Option1TextKey=deleteSirensDef.Option1Text";
        url = url + "&Option1DescriptionKey=deleteSirensDef.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelSiren('" + idRow + "'); return false;";
        url = url + "&Option2TextKey=deleteSirensDef.Option2Text";
        url = url + "&Option2DescriptionKey=deleteSirensDef.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("deleteGridSirens", e); }
}

function delSelSiren(idRow) {
    try {
        jsGridSirens.deleteRow(idRow);        
        hasChanges(true);
    } catch (e) { showError("delSelSiren", e); }
}

function showWarning() {
    var enabled = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_chkEnabled_chkButton').checked;
    if (!enabled) {
    Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsInfo, '',
        { text: '', key: 'roJsWarning' }, { text: '', key: 'msgWarningTerminalDisable' },
        { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
        Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
    }
}

function delSelSiren(idRow) {
    try {
        jsGridSirens.deleteRow(idRow);        
        hasChanges(true);
    } catch (e) { showError("delSelSiren", e); }
}

function checkStatusTerminal(oStatus) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "Terminals/srvMsgBoxTerminals.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else { //Missatge estil inline
            }
        }
    } catch (e) { showError('TerminalData:checkStatus', e); }
}

function ShowRemoveTerminal() {
    try {
        if (actualTerminal < 1) { return; }

        var url = "Terminals/srvMsgBoxTerminals.aspx?action=Message";
        url = url + "&TitleKey=deleteTerminal.Title";
        url = url + "&DescriptionKey=deleteTerminal.Description";
        url = url + "&Option1TextKey=deleteTerminal.Option1Text";
        url = url + "&Option1DescriptionKey=deleteTerminal.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteTerminal('" + actualTerminal + "'); return false;";
        url = url + "&Option2TextKey=deleteTerminal.Option2Text";
        url = url + "&Option2DescriptionKey=deleteTerminal.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveTerminal", e); }
}

function deleteTerminal(ID) {
    try {
        var postLoad = "checkStatusTerminal(arrStatus);";
        //if (delTreeNode != false) {
        postLoad += "if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }";
        //} else {
        //    postLoad += "if(arrStatus[0].error == 'false'){ refreshTree(); }";
        //}
        AsyncCall("POST", "Handlers/srvTerminals.ashx?action=deleteXTerminal&ID=" + ID, "json", "arrStatus", postLoad)
    } catch (e) { showError('TerminalData:deleteTerminal', e); }
}

function ConfirmLaunchBroadcasterForTerminal() {
    try {
        var url = "Terminals/srvMsgBoxTerminals.aspx?action=LaunchBroadcasterForTerminal";
        parent.ShowMsgBoxForm(url, 500, 300, '');
    }
    catch (e) {
        showError("ConfirmLaunchBroadcasterForTerminal", e);
    }
}

function LaunchBroadcasterForTerminal() {
    try {
        var stamp = '&IDTerminal=' + actualTerminal + '&StampParam=' + new Date().getMilliseconds();

        ajax = nuevoAjax();

        ajax.open("GET", "Handlers/srvTerminals.ashx?action=launchBroadcaster" + stamp, true);
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
        showError("LaunchBroadcasterForTerminal: ", e);
    }
}