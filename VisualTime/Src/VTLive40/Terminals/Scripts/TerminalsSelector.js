var actualTabTerminal = 0; // TAB per mostrar
var actualTerminal; // TerminalsLists actual

var actualZoneReader = 1;
var actualZoneDirection = 1;

var actualTabTerminalList = 0;
var actualTerminalList;

var actualType = "";

var nodeLoaded = false;

function cargaNodo(Nodo) {
    try {
        hasChanges(false);
        nodeLoaded = false;
        var oID = Nodo.id.substring(1);
        if (Nodo.id.substring(0, 1) == "A") {
            cargaTerminalsList(oID);
        } else {
            cargaTerminals(oID);
        }
    } catch (e) { showError("cargaNodo", e); }
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    ConvertControls('divContenido');
    if (actualType == "T") {
        if (s.cpActionRO == "GETTERMINAL" || s.cpActionRO == "SAVETERMINAL") {
            ASPxClientEdit.ValidateGroup(null, true);
            activeTabContainer('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01', 0);
            loadReaders(s.cpActiveReadersRO);
            actualZoneReader = 1;
            actualZoneDirection = 1;
            activeTabContainer('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR1_tabZones', 0);

            LoadNFCGrid(s);

            if (s.cpResultRO != "OK") {
                actualZoneReader = 1;
                actualZoneDirection = 1;
                var result = null;
                eval("result  = [" + s.cpMessageRO + "]");
                checkStatusTerminal(result);
                nodeLoaded = true;                
                hasChanges(true);
            } else {
                if (s.cpActionRO == "SAVETERMINAL") {
                    renameSelectedNode(s.cpNameTreeRO);
                }
            }
        } else if (s.cpActionRO == "VALIDATETERMINALREADER") {
            activeTabContainer('ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01', s.cpReaderValidationIdRO - 1);
            if (s.cpControlValidationRO == 'optHPFast' || s.cpControlValidationRO == 'optHPInteractive' || s.cpControlValidationRO == 'optHBlind') {
                changeVertTabs(1, 'ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + (s.cpReaderValidationIdRO));
            } else if (s.cpControlValidationRO == 'optChkOutPut' || s.cpControlValidationRO == 'optChkInvalidOutPut' || s.cpControlValidationRO == 'optChkCustomButtons') {
                changeVertTabs(3, 'ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + (s.cpReaderValidationIdRO));
            } else {
                changeVertTabs(0, 'ctl00_contentMainBody_ASPxCallbackPanelContenido_tabCtl01_frmTR' + (s.cpReaderValidationIdRO));
            }

            loadReaders(s.cpActiveReadersRO);            
            hasChanges(true);
        }
        var arrSirens = null;
        if (s.cpSirensRO == "") s.cpSirensRO = '{ "sirens": []}';
        eval("arrSirens = [" + s.cpSirensRO + "]");

        createSirensGrids(arrSirens);
    } else {
        resizeFrames();
        if (s.cpActionRO == "GETTERMINALSLIST" || s.cpActionRO == "SAVETERMINALSLIST") {
            if (s.cpResultRO != "OK") {
                var result = null;
                eval("result  = [" + s.cpMessageRO + "]");
                checkStatusTerminal(result);
                nodeLoaded = true;
                setPasswordPlaceholders();                
                hasChanges(true);
            } else {
                hasChanges(false);
            }
        }
    }

    setTimeout(function () { nodeLoaded = true; }, 500);
    showLoadingGrid(false);
}

function hasChanges(bolChanges, IdReader, FieldModified) {
    if (nodeLoaded == true && !firstLoad) {
        if (actualType == "T") {
            hasChangesTerminals(bolChanges, IdReader, FieldModified);
        } else if (actualType == "L") {
            hasChangesTerminalsList(bolChanges);
        }
    }    
}

function saveChanges() {
    try {
        nodeLoaded = false;
        if (actualType == "T") {
            saveChangesTerminals();
        } else if (actualType == "L") {
            saveChangesTerminalsList();
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChanges() {
    try {
        nodeLoaded = false;
        if (actualType == "T") {
            undoChangesTerminals();
        } else if (actualType == "L") {
            undoChangesTerminalsList();
        }
    } catch (e) { showError("undoChanges", e); }
}

function changeTabs(numTab) {
    if (actualType == "T") {
        changeTerminalsTabs(numTab);
    } else if (actualType == "L") {
        changeTerminalsListTabs(numTab);
    }
}

function changeTabsByName(nameTab) {
    if (actualType == "T") {
        changeTabsByNameTerminals(nameTab);
    } else if (actualType == "L") {
        changeTabsByNameTerminalsList(nameTab);
    }
}

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { alert('setMessage: ' + e); }
}

function RefreshScreen(DataType, oParms) {
    try {
        if (DataType == "1") {
            if (actualTerminal > 0) {
                if (actualZoneDirection == 1) {
                    repositionReadersIn(oParms);
                } else {
                    repositionReadersOut(oParms);
                }                
                hasChanges(true);
            }
        } else if (DataType == "9") {
            refreshTree();
        } else if (DataType == "6") {
            refreshTree();
        }
    } catch (e) { showError("RefreshScreen", e); }
}

function refreshTree() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesTerminals";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
}

function renameSelectedNode(txt) {
    // Modificamos el nombre en los árboles d'empleado
    var ctlPrefix = "ctl00_contentMainBody_roTreesTerminals";
    eval(ctlPrefix + "_roTrees.RenameSelectedNode('" + txt + "');");
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Terminals/srvMsgBoxTerminals.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        if (DescriptionKey != "") url = url + "&DescriptionKey=" + DescriptionKey;
        if (DescriptionText != "") url = url + "&DescriptionText=" + DescriptionText;
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

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function DDown_Out(objPrefix) {
    try {
        if (document.getElementById(objPrefix + '_gBoxWhoPunches_aFEmployees').getAttribute("vdisable") == "true") { return; }
        document.getElementById(objPrefix + '_gBoxWhoPunches_divFloatMenuE').style.display = 'none';
    } catch (e) { showError("DDown_Out", e); }
}

function DDown_Over(objPrefix) {
    try {
        if (document.getElementById(objPrefix + '_gBoxWhoPunches_aFEmployees').getAttribute("vdisable") == "true") { return; }
        document.getElementById(objPrefix + '_gBoxWhoPunches_divFloatMenuE').style.display = '';
    } catch (e) { showError("DDown_Over", e); }
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesTerminals";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function ConfigureTerminal(IDTerminal) {
    try {
        //Reposiciona l'arbre principal
        var ctlPrefix = "ctl00_contentMainBody_roTreesTerminals";
        eval(ctlPrefix + "_roTrees.GetSelectedPath('1', 'B" + IDTerminal + "', false,'" + ctlPrefix + "',true);");
    } catch (e) { showError("ConfigureTerminal", e); }
}

function AddNewTerminal() {
    try {
        parent.ShowExternalForm2('Terminals/Wizards/NewTerminalWizard.aspx', 500, 450, '', '', false, false);
    } catch (e) { showError('AddNewTerminal', e); }
}

function ChangeCommsState() {
    try {
        parent.ShowExternalForm2('Terminals/Wizards/ChangeCommsStateWizard.aspx', 500, 450, '','',false,false);
    } catch (e) { showError('ChangeCommsState', e); }
}

function ChangeTerminalPwd() {
    try {
        parent.ShowExternalForm2('Terminals/Wizards/ChangeCommsPassword.aspx', 500, 450, '', '', false, false);
    } catch (e) { showError('ChangeCommsState', e); }
}

function setPasswordPlaceholders() {
    var hasSupremaPassword = document.querySelector("[id$='hasSupremaPassword']").value;
    var passwordValue = txtPasswordSuprema_Client.GetInputElement().value;
    if (hasSupremaPassword === 'true' && passwordValue.trim() === '') {
        txtPasswordSuprema_Client.GetInputElement().value = "          ";
    }
}

function resetPasswordPlaceholders() {
    var passwordValue = txtPasswordSuprema_Client.GetInputElement().value;
    if (passwordValue.trim() === '') {
        txtPasswordSuprema_Client.GetInputElement().value = "";
    }
}
