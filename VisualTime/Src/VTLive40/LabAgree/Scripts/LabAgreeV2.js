var actualTab = 0; // TAB per mostrar
var actualLabAgrees; // LabAgrees actual
var telecommutingOptions;
var telecommutingMandatoryDays = [];
var telecommutingOptionalDays = [];
var telecommutingInfoLoaded = false;

var oLabAgrees;   //Clase LabAgreesData

var bolLoadFirstLabAgrees = false;
var newObjectName = "";
var responseObj = null;
var jsGridStartupValues; //Grid StartupValues
var jsGridLabAgreeRules; //Grid LabAgreeRules

function cargaNodo(Nodo) {
    try {
        if (Nodo.id.toUpperCase() == "SOURCE") newLabAgree();
        else cargaLabAgrees(Nodo.id);
    } catch (e) { showError("cargaNodo", e); }
}

function checkLabAgreeEmptyName(newName) {
    document.getElementById("readOnlyNameLabAgrees").textContent = newName;
    hasChanges(true);
}

function newLabAgree() {
    try {
        var contentUrl = "../Base/Popups/CreateObjectPopup.aspx?ObjectType=LabAgree";
        NewObjectPopup_Client.SetContentUrl(contentUrl);
        NewObjectPopup_Client.Show();
    } catch (e) { showError('newDocumentAbsence', e); }
}

function NewObjectCallback(ObjName) {
    try {
        showLoadingGrid(true);
        cargaLabAgrees(-1);
        newObjectName = ObjName;
    } catch (e) { showError('newConcept', e); }
}

function cargaLabAgrees(IDLabAgrees) {
    try {
        actualLabAgrees = IDLabAgrees;
        //TAB Gris Superior
        showLoadingGrid(true);
        cargaLabAgreesTabSuperior(IDLabAgrees);
    } catch (e) { showError("cargaLabAgrees", e); }
}

function cargaLabAgreesTabSuperior(IDLabAgrees) {
    try {
        var Url = "Handlers/srvLabAgree.ashx?action=getLabAgreesTab&aTab=" + actualTab + "&ID=" + IDLabAgrees;
        AsyncCall("POST", Url, "CONTAINER", "divLabAgrees", "cargaLabAgreesBarButtons(" + IDLabAgrees + ");");
    }
    catch (e) {
        showError("cargaLabAgreesTabSuperior", e);
    }
}
var responseObj = null;
function cargaLabAgreesBarButtons(IDLabAgrees) {
    try {
        var Url = "Handlers/srvLabAgree.ashx?action=getBarButtons&ID=" + IDLabAgrees;
        AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDLabAgrees + ")");
    }
    catch (e) {
        showError("cargaLabAgreesTabSuperior", e);
    }
}
function parseResponseBarButtons(oResponse, IDLabAgrees) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaLabAgreesDivs(IDLabAgrees);
}

function ckFixedValue_client_CheckedChanged(s, e) {
    var txt = document.getElementById('divtxtYearHours');
    var cmb = document.getElementById('divcmbUserField');

    if (s.GetValue() == true) {
        cmbUserFieldClient.SetEnabled(false);
        txtYearHoursClient.SetEnabled(true);
        txt.style.display = '';
        cmb.style.display = '';
    } else {
        cmbUserFieldClient.SetEnabled(true);
        txtYearHoursClient.SetEnabled(false);
        txt.style.display = '';
        cmb.style.display = '';
    }
}

function ckUserField_client_CheckedChanged(s, e) {
    var txt = document.getElementById('divtxtYearHours');
    var cmb = document.getElementById('divcmbUserField');

    if (s.GetValue() == true) {
        cmbUserFieldClient.SetEnabled(true);
        txtYearHoursClient.SetEnabled(false);
        txt.style.display = '';
        cmb.style.display = '';
    } else {
        cmbUserFieldClient.SetEnabled(false);
        txtYearHoursClient.SetEnabled(true);
        txt.style.display = '';
        cmb.style.display = '';
    }
}

function OnInitCauses(s, e) {
}

function updateExtraCauses(s, e) {
    const selectedExtras = tbExtrasClient.GetValue().split(",");
    const selectedDouble = cmbCausesDblClient.GetSelectedItem()?.value;
    const selectedTriple = cmbCausesTplClient.GetSelectedItem()?.value;

    //Obtenemos todas las justificaciones
    var allCausesItems = [];
    for (var i = 0; i < tbCausesClient.GetItemCount(); i++) {
        allCausesItems.push(tbCausesClient.GetItem(i));
    }

    //Vaciamos los combos
    tbExtrasClient.ClearItems();
    cmbCausesDblClient.ClearItems();
    cmbCausesTplClient.ClearItems();

    for (var i = 0; i < allCausesItems.length; i++) {
        //Actualizar valores tokenBox de extras
        if (allCausesItems[i].value != selectedTriple && allCausesItems[i].value != selectedDouble) {
            tbExtrasClient.AddItem(allCausesItems[i].text, allCausesItems[i].value)
        }
        //Actualizar valores comboBox dobles
        if (allCausesItems[i].value != selectedTriple && !selectedExtras.includes(allCausesItems[i].value)) {
            cmbCausesDblClient.AddItem(allCausesItems[i].text, allCausesItems[i].value)
        }
        //Actualizar valores comboBox triples
        if (allCausesItems[i].value != selectedDouble && !selectedExtras.includes(allCausesItems[i].value)) {
            cmbCausesTplClient.AddItem(allCausesItems[i].text, allCausesItems[i].value)
        }
    }

    if (selectedExtras) allCausesItems.map((cause) => { if (selectedExtras.includes(cause.value)) tbExtrasClient.AddToken(cause.text, cause.value) });
    if (selectedDouble) allCausesItems.map((cause) => { if (cause.value == selectedDouble) cmbCausesDblClient.SetSelectedItem(cmbCausesDblClient.FindItemByValue(selectedDouble)) });
    if (selectedTriple) allCausesItems.map((cause) => { if (cause.value == selectedTriple) cmbCausesTplClient.SetSelectedItem(cmbCausesTplClient.FindItemByValue(selectedTriple)) });

}

function cargaLabAgreesDivs(IDLabAgrees) {
    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = actualLabAgrees;
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETLABAGREE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function enableTC() {
    if (ckTelecommuteYes_client.GetValue() == false) {
        txtCanTelecommuteFromClient.SetDate(null);
        txtCanTelecommuteToClient.SetDate(null);
        txtTelecommutingMaxOptional_Client.SetValue("");
        txtCanTelecommuteFromClient.SetEnabled(false);
        txtCanTelecommuteToClient.SetEnabled(false);
        cmbWeekOrMonthClient.SetEnabled(false);
        cmbDaysOrPercentClient.SetEnabled(false);
        txtTelecommutingMaxOptional_Client.SetEnabled(false);
        unsetTelecommutingPattern();
    }
    else {
        tbTelecommutingOptional_Client.SetEnabled(true);
        txtCanTelecommuteFromClient.SetEnabled(true);
        cmbWeekOrMonthClient.SetEnabled(true);
        cmbDaysOrPercentClient.SetEnabled(true);
        txtCanTelecommuteToClient.SetEnabled(true);
        txtTelecommutingMaxOptional_Client.SetEnabled(true);
        setTelecommutingPattern();
    }
}

function grabarLabAgree(IDLabAgrees) {
    showLoadingGrid(true);
    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = actualLabAgrees;
    oParameters.Name = document.getElementById("readOnlyNameLabAgrees").textContent.trim();
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVELABAGREE";

    if ($('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTelecommutingGeneral').css("display") != 'none') {
        oParameters.Telecommuting = ckTelecommuteYes_client.GetValue();
        if (cmbDaysOrPercentClient.GetValue() == 0)
            oParameters.TelecommutingMaxOptionalDays = txtTelecommutingMaxOptional_Client.GetValue();
        else
            oParameters.TelecommutingPercentage = txtTelecommutingMaxOptional_Client.GetValue();
        oParameters.TelecommutingPeriodType = cmbWeekOrMonthClient.GetValue();
        oParameters.TelecommutingAgreementStart = moment(txtCanTelecommuteFromClient.GetValue()).format('YYYY/MM/DD');
        oParameters.TelecommutingAgreementEnd = moment(txtCanTelecommuteToClient.GetValue()).format('YYYY/MM/DD');
        oParameters.TelecommutingMandatoryDays = array2String(telecommutingMandatoryDays, ",");
        oParameters.PresenceMandatoryDays = array2String(presenceMandatoryDays, ",");
        oParameters.TelecommutingOptionalDays = array2String(telecommutingOptionalDays, ",");
    }

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    var txt = document.getElementById('divtxtYearHours');
    var cmb = document.getElementById('divcmbUserField');

    var hasYearHours = txtYearHoursClient.GetValue();

    if (s.cpNameRO == null) {
        txt.style.display = '';
        cmb.style.display = '';
    }
    else {
        if (cmbUserFieldClient.GetSelectedItem() == null) {
            txt.style.display = '';
            cmb.style.display = '';
        }

        else {
            txt.style.display = '';
            cmb.style.display = '';
        }
    }

    showLoadingGrid(false);
    if (s.cpActionRO == "GETLABAGREE") {
        if (typeof s.cpTelecommutingOptions != 'undefined') {

            telecommutingOptions = JSON.parse(s.cpTelecommutingOptions);
            telecommutingMandatoryDays = [];
            telecommutingOptionalDays = [];
            presenceMandatoryDays = [];
            for (i = 0; i < tbWorkingDaysClient.values.length; i++)
                if (telecommutingMandatoryDays.indexOf(tbWorkingDaysClient.values[i]) == -1 && telecommutingOptionalDays.indexOf(tbWorkingDaysClient.values[i]) == -1)
                    presenceMandatoryDays.push(tbWorkingDaysClient.values[i]);
        }

        if (typeof s.cpTelecommutingPatternResult != 'undefined') {
            var daysInfo = JSON.parse(s.cpTelecommutingPatternResult)
            telecommutingOptions = JSON.parse(s.cpTelecommutingOptions)
            if (s.cpTelecommutingMandatoryDays != null && s.cpTelecommutingMandatoryDays != "") {
                telecommutingMandatoryDays = s.cpTelecommutingMandatoryDays.split(",");
            }
            else {
                telecommutingMandatoryDays = [];
            }
            if (s.cpTelecommutingOptionalDays != null && s.cpTelecommutingOptionalDays != "") {
                telecommuntingOptionalDays = s.cpTelecommutingOptionalDays.split(",");
            }
            else {
                telecommuntingOptionalDays = [];
            }
            if (s.cpPresenceMandatoryDays != null && s.cpPresenceMandatoryDays != "") {
                presenceMandatoryDays = s.cpPresenceMandatoryDays.split(",");
            }
            else {
                presenceMandatoryDays = [];
                for (i = 0; i < tbWorkingDaysClient.values.length; i++)
                    if (telecommutingMandatoryDays.indexOf(tbWorkingDaysClient.values[i]) == -1 && telecommuntingOptionalDays.indexOf(tbWorkingDaysClient.values[i]) == -1)
                        presenceMandatoryDays.push(tbWorkingDaysClient.values[i]);
            }

            LoadTelecommutingPattern(daysInfo, telecommutingOptions);
        }
        else {
            telecommutingOptionalDays = [];
            telecommutingMandatoryDays = [];
        }
        enableTC();
        if (s.cpNameRO != null && s.cpNameRO != "") {
            document.getElementById("readOnlyNameLabAgrees").textContent = s.cpNameRO;
            hasChanges(false);
            ASPxClientEdit.ValidateGroup(null, true);
        } else {
            document.getElementById("readOnlyNameLabAgrees").textContent = newObjectName;
            hasChanges(true);
            txtLabAgreeName_Client.SetValue(newObjectName);
        }

        if (s.cpIsNew == true) {
            refreshTree();
        }
    } else if (s.cpActionRO == "SAVELABAGREE") {
        var message = s.cpResultRO.substr(7, s.cpResultRO.length - 7)
        if (s.cpResultRO.indexOf("MESSAGE") != -1) {
            var url = "LabAgree/srvMsgBoxLabAgree.aspx?action=Message&" + message;
            parent.ShowMsgBoxForm(url, 500, 300, '');
            hasChanges(true);
        } else {
            refreshTree();
        }
    }

    updateExtraCauses();
}

function changeTabs(numTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div01', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div02');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (div != null && tab != null) {
            if (n == numTab) {
                tab.className = 'bTab-active';
                div.style.display = '';
            } else {
                tab.className = 'bTab';
                div.style.display = 'none';
            }
        }
    }
    actualTab = numTab;
}

function changeTabsByName(nameTab) {
    arrButtons = new Array('TABBUTTON_00', 'TABBUTTON_01', 'TABBUTTON_02');
    arrDivs = new Array('ctl00_contentMainBody_ASPxCallbackPanelContenido_div00', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div01', 'ctl00_contentMainBody_ASPxCallbackPanelContenido_div02');

    for (n = 0; n < arrButtons.length; n++) {
        var tab = document.getElementById(arrButtons[n]);
        var div = document.getElementById(arrDivs[n]);
        if (div != null && tab != null) {
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
}

function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

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

function refreshBeforeSave(arrStatus) {
    refreshTree();
}

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
    telecommutingInfoLoaded = false;
    if (actualLabAgrees == -1) {
        var ctlPrefix = "ctl00_contentMainBody_roTreesLabAgrees";
        eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
    } else {
        cargaLabAgrees(actualLabAgrees);
    }
}

function validateTC() {
    if (ckTelecommuteYes_client.GetValue() == true) {
        if (moment(txtCanTelecommuteFromClient.GetValue()).format('YYYY/MM/DD') > moment(txtCanTelecommuteToClient.GetValue()).format('YYYY/MM/DD')) {
            showErrorPopup("Error.Title", "error", "Error.ValidationTCDates", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }
        else if (!moment(txtCanTelecommuteToClient.GetValue()).isValid() || !moment(txtCanTelecommuteFromClient.GetValue()).isValid()) {
            showErrorPopup("Error.Title", "error", "Error.ValidationTCDates", "", "Error.OK", "Error.OKDesc", "");
            return false;
        }
        //else if (parseInt(txtTelecommutingMaxOptional_Client.GetValue()) > tbTelecommutingOptional_Client.tokens.length) {
        //    showErrorPopup("Error.Title", "error", "Error.ValidationTCOptional", "", "Error.OK", "Error.OKDesc", "");
        //    return false;
        //}
        else {
            if (cmbDaysOrPercentClient.GetValue() == '0' && cmbWeekOrMonthClient.GetValue() == '0' && parseInt(txtTelecommutingMaxOptional_Client.GetValue()) > 7) {
                showErrorPopup("Error.Title", "error", "Error.ValidationTCDaysWeek", "", "Error.OK", "Error.OKDesc", "");
                return false;
            }
            else {
                if (cmbDaysOrPercentClient.GetValue() == '0' && cmbWeekOrMonthClient.GetValue() == '1' && parseInt(txtTelecommutingMaxOptional_Client.GetValue()) > 31) {
                    showErrorPopup("Error.Title", "error", "Error.ValidationTCDaysMonth", "", "Error.OK", "Error.OKDesc", "");
                    return false;
                }
                else {
                    if (cmbDaysOrPercentClient.GetValue() == '0' && cmbWeekOrMonthClient.GetValue() == '2' && parseInt(txtTelecommutingMaxOptional_Client.GetValue()) > 93) {
                        showErrorPopup("Error.Title", "error", "Error.ValidationTCDaysQuarter", "", "Error.OK", "Error.OKDesc", "");
                        return false;
                    }
                    else
                        return true;
                }
            }
        }
    }
    else {
        return true;
    }
}

function showCaptcha() {
    if (ASPxClientEdit.ValidateGroup("nameDesc", true)) {
        if (validateTC()) {
            var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=SAVELABAGREE&ShowFreezingDate=1";
            CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
            CaptchaObjectPopup_Client.Show();
        }
    } else {
        showErrorPopup("Error.Title", "error", "Error.ValidationFieldsFailed", "", "Error.OK", "Error.OKDesc", "");
    }
}

function captchaCallback(action) {
    switch (action) {
        case "SAVELABAGREE":
            try {
                grabarLabAgree(actualLabAgrees);
            } catch (e) { showError("saveChanges", e); }

            break;
        case "ERROR":
            window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
            break;
    }
}

function saveChanges() {
    showCaptcha();
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
    var ctlPrefix = "ctl00_contentMainBody_roTreesLabAgrees";
    eval(ctlPrefix + "_roTrees.LoadTreeViews(true, true, true);");
}

function deleteSelectedNode() {
    var ctlPrefix = "ctl00_contentMainBody_roTreesLabAgrees";
    eval(ctlPrefix + "_roTrees.DeleteSelectedNode();");
}

function ShowRemoveLabAgree() {
    try {
        if (actualLabAgrees == -1 || actualLabAgrees == 0) { return; }

        var url = "LabAgree/srvMsgBoxLabAgree.aspx?action=Message";
        url = url + "&TitleKey=deleteLabAgree.Title";
        url = url + "&DescriptionKey=deleteLabAgree.Description";
        url = url + "&Option1TextKey=deleteLabAgree.Option1Text";
        url = url + "&Option1DescriptionKey=deleteLabAgree.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteLabAgree('" + actualLabAgrees + "'); return false;";
        url = url + "&Option2TextKey=deleteLabAgree.Option2Text";
        url = url + "&Option2DescriptionKey=deleteLabAgree.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, '');
    } catch (e) { showError("ShowRemoveLabAgree", e); }
}

var arrStatus;

function deleteLabAgree(Id) {
    try {
        if (Id != "-1" && Id != "0") {
            AsyncCall("POST", "Handlers/srvLabAgree.ashx?action=deleteXLabAgree&ID=" + Id, "json", "arrStatus", "checkStatus(arrStatus); if(arrStatus[0].error == 'false'){ deleteSelectedNode(); }")
        }
    } catch (e) { showError('deleteLabAgree', e); }
}

function checkStatus(oStatus) {
    try {
        arrStatus = oStatus;
        objError = arrStatus[0];

        if (objError.error == "true") {
            if (objError.typemsg == "1") { //Missatge estil pop-up
                var url = "LabAgree/srvMsgBoxLabAgree.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" + objError.msg + "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else {
            }
            hasChanges(true);
        }
    } catch (e) { showError("checkStatus", e); }
}

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "LabAgree/srvMsgBoxLabAgree.aspx?action=Message";
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

function showMsg(oMsg) {
    alert(oMsg);
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function dropDownBoxEditorTemplateDay(cellElement, cellInfo) {
    var dayPropertyName;
    var dayIndex;
    if (cellInfo.column.index == 7) {
        dayPropertyName = "Day0";
        dayIndex = 0;
    }
    else {
        dayPropertyName = "Day" + (cellInfo.column.index);
        dayIndex = cellInfo.column.index;
    }
    var dayProperty = cellInfo.data[dayPropertyName];
    return $('<div>').dxSelectBox({
        dataSource: telecommutingOptions,
        displayExpr: 'Name',
        valueExpr: 'ID',
        value: dayProperty,
        onSelectionChanged(selectionChangedArgs) {
            if (cellInfo.value != selectionChangedArgs.selectedItem.ID) {
                hasChanges(true);
            }
            cellInfo.setValue(selectionChangedArgs.selectedItem.ID);
            if (selectionChangedArgs.selectedItem.ID == 1) {
                telecommutingMandatoryDays.push(dayIndex.toString());
                deleteElementFromArray(telecommutingOptionalDays, dayIndex.toString());
                deleteElementFromArray(presenceMandatoryDays, dayIndex.toString());
            }
            else {
                if (selectionChangedArgs.selectedItem.ID == 2) {
                    telecommutingOptionalDays.push(dayIndex.toString())
                    deleteElementFromArray(telecommutingMandatoryDays, dayIndex.toString());
                    deleteElementFromArray(presenceMandatoryDays, dayIndex.toString());
                }
                else {
                    if (presenceMandatoryDays.indexOf(dayIndex.toString()) == -1)
                        presenceMandatoryDays.push(dayIndex.toString())
                    deleteElementFromArray(telecommutingMandatoryDays, dayIndex.toString());
                    deleteElementFromArray(telecommutingOptionalDays, dayIndex.toString());
                }
            }
        },
        fieldTemplate(data, container) {
            const result = $(`<div class='custom-item'><img style='width:24px;'src='${data ? data.ImageSrc : ''
                }' /><div class='telOption'></div></div>`);
            result
                .find('.telOption')
                .dxTextBox({
                    value: data && data.Name,
                    readOnly: true
                });
            container.append(result);
        },
        itemTemplate(data) {
            return `<div class='custom-item'><img style="width:16px;margin-right:10px;float:left;" src='${data.ImageSrc}' /><div class='product-name'>${data.Name}</div></div>`;
        },
    });
}

function telecommutingInfoTemplate(element, info) {
    var telecommutingOptionInfo = telecommutingOptions.filter(function (item) { return item.ID === info.text; });
    if (typeof telecommutingOptionInfo != undefined && telecommutingOptionInfo.length > 0) {
        element.append("<div><img style='width:24px;' src='" + telecommutingOptionInfo[0].ImageSrc + "'/></div>").append("<div>" + telecommutingOptionInfo[0].Name + "</div>")
    }
    else
        element.append("<div>" + telecommutingOptionInfo[0].Name + "</div>")
}

function LoadTelecommutingPattern(s) {
    acumValues = [];
    telecommutingInfoLoaded = true;
    if (s != null && s.length > 0) {
        for (var i = 0; i < s.length; i++) {
            acumValues.push({
                Id: s[i].Week,
                Day0: s[i].Day0,
                Day1: s[i].Day1,
                Day2: s[i].Day2,
                Day3: s[i].Day3,
                Day4: s[i].Day4,
                Day5: s[i].Day5,
                Day6: s[i].Day6
            });
        }
    }
    else {
        acumValues.push({
            Id: 1,
            Day0: 0,
            Day1: 0,
            Day2: 0,
            Day3: 0,
            Day4: 0,
            Day5: 0,
            Day6: 0
        });
    }
    gridValues = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTelecommutingPatternGrid").dxDataGrid({
        id: "gridLabAgreeTelecommtingPattern",
        showColumnLines: false,
        showRowLines: false,
        height: 120,
        rowAlternationEnabled: false,
        showBorders: false,
        headerFilter: { visible: false },
        allowColumnResizing: true,
        filterRow: { visible: false },
        toolbar: { visible: false },
        dataSource: {
            store: {
                type: 'array',
                key: 'Id',
                data: acumValues
            }
        },
        editing: {
            mode: 'batch',
            allowUpdating: true,
            allowAdding: false,
            allowDeleting: false,
            selectTextOnEditStart: true,
            startEditAction: 'click',
        },
        onToolbarPreparing: function (e) {
            e.toolbarOptions.visible = false;
            var toolbarItems = e.toolbarOptions.items;
            $.each(toolbarItems, function (_, item) {
                if (item.name == "saveButton" || item.name == "revertButton") {
                    item.visible = false;
                }
            });
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var isEditing = e.row.isEditing, $links = e.cellElement.find(".dx-link");
                $links.text("");

                if (isEditing) {
                    $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
                } else {
                    $links.filter(".dx-link-edit").addClass("dx-icon-edit");
                    $links.filter(".dx-link-delete").addClass("dx-icon-trash");
                }
            }
            if (e.rowType === "data") {
                e.cellElement.css({ "backgroundColor": "#fff" });
            }
        },
        onRowPrepared: function (e) {
            if (e.rowType === "data") {
                e.rowElement.css({ height: 30 });
                e.rowElement.css({ "border": "0px !important" });
            }
        },
        customizeColumns: function (columns) {
            for (var i = 1; i < 8; i++) {
                var col = i.toString();
                if (i == 7)
                    col = "0";
                if (tbWorkingDaysClient.values.indexOf(col) == -1)
                    columns[i].visible = false;
            }
        },
        remoteOperations: {
            sorting: false,
            paging: false
        },
        paging: {
            enabled: false
        },
        columns: [

            { caption: "Id", dataField: "Id", allowEditing: true, allowDeleting: false, visible: false, alignment: "center" },
            { caption: window.parent.Globalize.formatMessage("monday"), dataField: "Day1", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("tuesday"), dataField: "Day2", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("wednesday"), dataField: "Day3", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("thursday"), dataField: "Day4", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("friday"), dataField: "Day5", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("saturday"), dataField: "Day6", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 },
            { caption: window.parent.Globalize.formatMessage("sunday"), dataField: "Day0", allowEditing: true, allowDeleting: false, alignment: "center", editCellTemplate: dropDownBoxEditorTemplateDay, cellTemplate: telecommutingInfoTemplate, width: 120 }
        ]
    }).dxDataGrid("instance");
}

function updatePatternColumns() {
    for (var i = 1; i <= 7; i++) {
        var col = i.toString();
        if (i == 7)
            col = "0"
        if (tbWorkingDaysClient.values.indexOf(col) == -1) {
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTelecommutingPatternGrid").dxDataGrid("columnOption", i, "visible", false);
            if (telecommutingMandatoryDays.indexOf(col) >= 0)
                deleteElementFromArray(telecommutingMandatoryDays, col.toString());
            if (telecommuntingOptionalDays.indexOf(col) >= 0)
                deleteElementFromArray(telecommuntingOptionalDays, col.toString());
            if (presenceMandatoryDays.indexOf(col) >= 0)
                deleteElementFromArray(presenceMandatoryDays, col.toString());
        }
        else {
            $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTelecommutingPatternGrid").dxDataGrid("columnOption", i, "visible", true);
            if (telecommutingMandatoryDays.indexOf(col) == -1 && telecommuntingOptionalDays.indexOf(col) == -1 && presenceMandatoryDays.indexOf(col) == -1)
                presenceMandatoryDays.push(col.toString());
        }
    }
}

function unsetTelecommutingPattern() {
    if (telecommutingInfoLoaded) {
        source = [];

        source.push({
            Id: 1,
            Day0: "0",
            Day1: "0",
            Day2: "0",
            Day3: "0",
            Day4: "0",
            Day5: "0",
            Day6: "0"
        });

        var store = {
            store: {
                type: 'array',
                key: 'Id',
                data: source
            }
        };
        var dataGrid = $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTelecommutingPatternGrid').dxDataGrid('instance');
        dataGrid.option("disabled", true);
        dataGrid.option("dataSource", store);
        dataGrid.refresh();

        telecommutingMandatoryDays = [];
        telecommutingOptionalDays = [];
    }
}

function setTelecommutingPattern() {
    if (telecommutingInfoLoaded) {
        var dataGrid = $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_divTelecommutingPatternGrid').dxDataGrid('instance');
        dataGrid.option("disabled", false);
        dataGrid.refresh();
    }
}