var actualConceptTab = 0; // TAB per mostrar
var actualConcept; // Concept actual

var actualConceptGroupTab = 0; // TAB per mostrar
var actualConceptGroup; // Concept actual

var actualType = ""

var newObjectName = "";

function ShowTreesConceptGroups() {
    var dvI = $("#dvTreeConcepts");
    var dvE = $("#dvTreeConceptGroups");
    dvE.show();
    dvI.hide();
    cargaConceptGroup(actualConceptGroup);
}

function ShowTreesConcepts() {
    var dvI = $("#dvTreeConcepts");
    var dvE = $("#dvTreeConceptGroups");
    dvI.show();
    dvE.hide();
    cargaConcept(actualConcept);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    var contenedor2 = document.getElementById('divContent');
    if (actualType == "C") {
        automaticAccrualConf = { causes: [], shifts: [], clientEnabled: false, selectedHourCauses: [], selectedDayCauses: [], selectedDayShifts: [] };

        if (s.cpActionRO == "GETCONCEPT" || s.cpActionRO == "SAVECONCEPT") {
            ConvertControls('divContent');
            cancelOPCEvents = true;
            ConvertGroupsConcept();
            try { 
            accrualTypeChanged(cmbShowValueClient.GetSelectedItem().value);
            }
            catch (e) {  }
            cancelOPCEvents = false;
            if (s.cpResultRO == "OK") {
                txtConceptGroupName_Client.SetValue("_____");

                if (s.cpIsNewRO == true) {
                    refreshTreeConcept();
                    return;
                }
                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameConcept").textContent = s.cpNameRO;
                    if (s.cpCompositionsGridRO != "") {
                        var grid = null;
                        eval("grid = [" + s.cpCompositionsGridRO + "]");
                        createGridCompositions(grid);
                    }
                    if (s.cpCombosIndexRO != "") {
                        setSelectedIndexes("ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeesPermissionCriteria_employeeCriteria", s.cpCombosIndexRO);
                    }

                    var clientData = { causes: [], shifts: [], clientEnabled: false, selectedHourCauses: [], selectedDayCauses: [], selectedDayShifts: [] };

                    if (typeof s.cpClientControlsRO != 'undefined' && s.cpClientControlsRO != "") {
                        clientData = JSON.parse(s.cpClientControlsRO);
                    }

                    initializeAutomaticAccrualsData(clientData);

                    ASPxClientEdit.ValidateGroup(null, true);
                    hasChanges(false);
                } else {
                    document.getElementById("readOnlyNameConcept").textContent = newObjectName;
                    txtDescription_Client.SetFocus();
                    txtDescription_Client.SetValue(newObjectName);

                    if (typeof s.cpClientControlsRO != 'undefined' && s.cpClientControlsRO != "") {
                        automaticAccrualConf = JSON.parse(s.cpClientControlsRO);
                    }

                    initializeAutomaticAccrualsData(automaticAccrualConf);

                    hasChanges(true);
                }

                txtConceptName_Client.SetValue(document.getElementById("readOnlyNameConcept").textContent);

                if (s.cpActionRO == "SAVECONCEPT") {
                    actualConcept = parseInt(s.cpNewIdRO, 10);
                    refreshTreeConcept();
                    hasChanges(false);
                }
            } else {
                if (typeof s.cpClientControlsRO != 'undefined' && s.cpClientControlsRO != "") {
                    clientData = JSON.parse(s.cpClientControlsRO);
                }

                initializeAutomaticAccrualsData(clientData);

                //Mostramos el error
                if (s.cpCompositionsGridRO != "") {
                    var grid = null;
                    eval("grid = [" + s.cpCompositionsGridRO + "]");
                    createGridCompositions(grid);
                }
                hasChanges(true);
                showErrorPopup("SaveName.Error.Text", "error", "", s.cpMessageRO, "SaveName.Error.Option1Text", "SaveName.Error.Option1Description", "")
            }
        }
    } else {
        if (s.cpActionRO == "GETCONCEPTGROUP" || s.cpActionRO == "SAVECONCEPTGROUP") {
            if (s.cpResultRO == "OK") {
                ConvertControls('divContent');

                txtConceptName_Client.SetValue("_____");
                txtShortName_Client.SetValue("__");

                if (s.cpIsNewRO == true) {
                    refreshTreeConceptGroups();
                    return;
                }

                if (s.cpNameRO != null && s.cpNameRO != "") {
                    document.getElementById("readOnlyNameConceptGroup").textContent = s.cpNameRO;
                    if (s.cpCompositionsGridRO != "") {
                        var grid = null;
                        eval("grid = [" + s.cpCompositionsGridRO + "]");
                        createGridConceptGroupsItems(grid);
                    }
                    ASPxClientEdit.ValidateGroup(null, true);
                    hasChanges(false);
                } else {
                    document.getElementById("readOnlyNameConceptGroup").textContent = newObjectName;
                    hasChanges(true);
                    txtConceptGroupName_Client.SetFocus();
                    txtConceptGroupName_Client.SetValue(newObjectName);
                }

                if (s.cpActionRO == "SAVECONCEPTGROUP") {
                    actualConceptGroup = parseInt(s.cpNewIdRO, 10);
                    refreshTreeConceptGroups();
                    hasChanges(false);
                }
            } else {
                //Mostramos el error
                if (s.cpCompositionsGridRO != "") {
                    var grid = null;
                    eval("grid = [" + s.cpCompositionsGridRO + "]");
                    createGridConceptGroupsItems(grid);
                }
                hasChanges(true);
                showErrorPopup("SaveName.Error.Text", "error", "", s.cpMessageRO, "SaveName.Error.Option1Text", "SaveName.Error.Option1Description", "")
            }
        }
    }
    showLoadingGrid(false);
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

function hasChanges(bolChanges) {
    if (actualType == "C") {
        hasChangesConcepts(bolChanges);
    } else if (actualType == "G") {
        hasChangesConceptGroups(bolChanges);
    }
}

function saveChanges() {
    try {
        if (actualType == "C") {
            saveChangesConcept();
        } else if (actualType == "G") {
            saveChangesConceptGroups();
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChanges() {
    try {
        showLoadingGrid(true);
        if (actualType == "C") {
            undoChangesConcepts();
        } else if (actualType == "G") {
            undoChangesConceptGroups();
        }
    } catch (e) { showError("undoChanges", e); }
}

function changeTabs(numTab) {
    if (actualType == "C") {
        changeTabsConcepts(numTab);
    } else if (actualType == "G") {
        changeTabsGroups(numTab);
    }
}

//Cambia els Tabs i els divs (per nom)
function changeTabsByName(nameTab) {
    if (actualType == "C") {
        changeTabsByNameConcepts(nameTab);
    } else if (actualType == "G") {
        changeTabsByNameGroups(nameTab);
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

function setMessage(msg) {
    try {
        var msgTop = document.getElementById('msgTop');
        var msgBottom = document.getElementById('msgBottom');
        msgTop.textContent = msg;
        msgBottom.textContent = msg;
    } catch (e) { showError('setMessage', e); }
}

function showErrorPopup(Title, typeIcon, Description, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Concepts/srvMsgBoxConcepts.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        if (Description != "") url = url + "&DescriptionKey=" + Description;
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

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        if (actualType == "C") cargaConcept(-1);
        else cargaConceptGroup(-1);
        newObjectName = ObjName;
    } catch (e) { showError('newConcept', e); }
}