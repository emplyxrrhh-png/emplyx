var bClearFilter = true;

function showErrorPopup(Title, typeIcon, DescriptionKey, DescriptionText, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Documents/srvMsgBoxDocument.aspx?action=Message";
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

//=========CAPTCHA====================================================
function showCaptcha() {
    var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=SAVEBUSINESSCENTERS";
    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
    CaptchaObjectPopup_Client.Show();
}

function captchaCallback(action) {
    switch (action) {
        case "SAVEBUSINESSCENTERS":
            showLoadingGrid(true);
            CallbackSessionClient.PerformCallback("SAVEDATA");
            break;
        case "ERROR":
            window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
            break;
    }
}
//============ POPUP SAVE VIEWS ====================================
function formatDateYMD(initialDate) {
    if (initialDate == "" || initialDate == null) {
        return "";
    }
    else {
        var dia = initialDate.getDate();
        if (dia.toString().length == 1) dia = "0" + dia.toString();
        var mes = initialDate.getMonth() + 1; //Months are zero based
        if (mes.toString().length == 1) mes = "0" + mes.toString();
        var ejer = initialDate.getFullYear();
        var finalDate = ejer + "#" + mes + "#" + dia;
        return finalDate;
    }
}

//==================================================================

//============ SELECTOR DE EMPLEADOS ===============================
function btnOpenPopupSelectorEmployeesClient_Click() {
    PopupSelectorEmployeesClient.Show();
}

function btnPopupSelectorEmployeesAcceptClient_Click() {
    CallbackSessionClient.PerformCallback("GETINFOSELECTED")
    PopupSelectorEmployeesClient.Hide();
}

function btCloseRequestClient_Click() {
    PopupEditRequestClient.Hide();
    ThrowRefreshClient(false);
}
//==================================================================

//============ SELECTOR DE INCIDENCIAS =============================
function btnOpenPopupSelectorCausesClient_Click() {
    PopupSelectorCausesClient.Show();
}

function btnPopupSelectorCausesAcceptClient_Click() {
    var arrCausesValues = ListCauses_Client.GetSelectedValues();

    var hdnCausesSelected = document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnCausesSelected');

    if (arrCausesValues.length > 0) {
        hdnCausesSelected.value = arrCausesValues;
        if (arrCausesValues.length == 1) {
            CallbackSessionClient.PerformCallback("UPDATECAUSETEXT")
        } else {
            document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_txtCauses').value = arrCausesValues.length + " " + document.getElementById('ctl00_contentMainBody_hdnCauseSelectedText').value;
        }
    }
    else {
        hdnCausesSelected.value = "";
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_txtCauses').value = document.getElementById('ctl00_contentMainBody_hdnAllCauses').value;
    }

    PopupSelectorCausesClient.Hide();
}

function SelectAllCauses() {
    if (chkCausesClient.GetChecked()) {
        ListCauses_Client.SelectAll();
    } else {
        ListCauses_Client.UnselectAll();
    }
}

//==================================================================

//============ RETORNO DE LLAMADAS DE CALLBACKS SIMPLES ============
var monitor = -1;
function CallbackSession_CallbackComplete(s, e) {
    if (e.parameter == "GETINFOSELECTED") {
        var Limit = e.result.indexOf(";");
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnEmployeesSelected').value = e.result.substring(0, Limit);;
        var txtEmployees = document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_txtEmployees')
        txtEmployees.value = e.result.substring(Limit + 1);
        if (txtEmployees.value == " ")
            txtEmployees.value = document.getElementById('ctl00_contentMainBody_hdnAllEmployees').value;
    } else if (e.parameter == "CLIENTSHOWDETAILS") {
        var typePos = e.result.indexOf(";");
        var type = e.result.substring(0, typePos);
        var url = e.result.substring(typePos + 1);
        ShowDetailGrid(url, type);
    } else if (e.parameter == "UPDATECAUSETEXT") {
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_txtCauses').value = e.result;
    } else if (e.parameter == "SAVEDATA") {
        if (s.cpActionResult == "OK") {
            monitor = setInterval(function () { CallbackSessionClient.PerformCallback("CHECKPROGRESS"); }, 5000);
        } else {
            parent.showLoader(false);
            window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", s.cpActionMessageResult, "Error.OK", "Error.OKDesc", "");
            clearInterval(monitor);
        }
    } else if (s.cpAction == "CHECKPROGRESS") {
        if (s.cpActionResult == "OK") {
            parent.showLoader(false);
            clearInterval(monitor);
            btnRefreshClient_Click();
        } else if (s.cpActionResult == "KO") {
            parent.showLoader(false);
            window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", s.cpActionMessageResult, "Error.OK", "Error.OKDesc", "");
            clearInterval(monitor);
        }
    } else if (e.parameter == "CLIENTEMPLOYEEDETAILS") {
        var typePos = e.result.indexOf("@");
        var idEmployee = e.result.substring(0, typePos);
        var rootUrl = e.result.substring(typePos + 1);

        var stamp = '&StampParam=' + new Date().getMilliseconds();
        var _ajax = nuevoAjax();
        _ajax.open("GET", "../Base/WebUserControls/EmployeeSelectorData.aspx?action=getSelectionPath&node=B" + idEmployee + "&TreeType=1" + stamp, true);
        _ajax.onreadystatechange = async function () {
            if (_ajax.readyState == 4) {
                var objPrefix = "ctl00_contentMainBody_roTrees1";
                var val = _ajax.responseText;
                if (val == null || val == "null") { val = ""; }

                var oTreeState = await getroTreeState(objPrefix);
                oTreeState.setSelectedPath(val, '1');
                oTreeState.setSelected("B" + idEmployee, '1');
                oTreeState.setActiveTreeType('1');

                window.open("../#/" + rootUrl + "/Employees/Employees", "_blank");
            }
        }
        _ajax.send(null)
    }
}
//==================================================================

//============ OBTENER DATOS =================================
function btnRefreshClient_Click() {
    ThrowRefreshClient(true);
}

function PopupWarningYes() {
    PopupWarningClient.Hide();
    ThrowRefreshClient(true);
}

function ThrowRefreshClient(clearFilter) {
    if (typeof (clearFilter) != 'undefined') {
        bClearFilter = clearFilter;
    }
    GridAbsencesStatusClient.PerformCallback("REFRESHGRID");
    parent.showLoader(false);
}

function GridAbsencesClient_EndCallback(s, e) {
}

//============ Eventos combo cliente =================================
function cmbCausesFilter_ValueChanged() {
}

function cmbStatusClient_ValueChanged() {
    if (cmbStatusClient.GetValue() == "2" || cmbStatusClient.GetValue() == "3") {
        $('#dateFilters').css('display', '');
    } else {
        $('#dateFilters').css('display', 'none');
    }
}

function GridAbsencesClientCustomButton_Click(s, e) {
    if (e.buttonID == "ShowDetailButton") {
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnSelectedRowVisibleIndex').value = e.visibleIndex;
        CallbackSessionClient.PerformCallback("CLIENTSHOWDETAILS");
    } else if (e.buttonID == "ShowEmployeeButton") {
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnSelectedRowVisibleIndex').value = e.visibleIndex;
        CallbackSessionClient.PerformCallback("CLIENTEMPLOYEEDETAILS");
    }
}

function ShowDetailGrid(url, type) {
    var Title = '';
    if (type == 1) { //ProgrammedAbsence
        parent.ShowExternalForm2(url, 830, 440, Title, '', false, false, false);
    } else if (type == 2) { //ProgramedCause
        parent.ShowExternalForm2(url, 830, 440, Title, '', false, false, false);
    } else if (type == 3) {
        PopupEditRequestClient.Show();
        loadRequest(url, -1);
    }
}

function RefreshScreen(MustRefresh, Params) {
    ThrowRefreshClient(false);
}

///=====================JAVASCRIPT DE PANTALLA===========================
function GridIncidences_beginCallback(e, c) {
}

function GridIncidences_EndCallback(s, e) {
    if (s.IsEditing()) {
    }
    showLoadingGrid(false);
}

function GridIncidences_FocusedRowChanged(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
}

function GridIncidences_OnRowDblClick(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
    s.StartEditRow(e.visibleIndex);
}

function GridIncidences_SelectionChanged(s, e) {
    s.GetSelectedFieldValues("ValueHoraEditable", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    var totalTime = 0;
    for (var i = 0; i < values.length; i++) {
        totalTime += parseFloat(values[i].filterTimeFormat());
    }

    var timeStr = totalTime.HoursToHHMMSS(false);
    while (timeStr.length < 9) timeStr = "0" + timeStr;
    txtTimeSelectedClient.SetValue(timeStr);
}

function GridCancelEditing() {
    try {
        if (IsGridEditing(false)) {
            GridIncidencesClient.CancelEdit();
        }
    }
    catch (e) {
        showError("IsGridEditing", e);
    }
}

function IsGridEditing(showWarning) {
    try {
        if (typeof (GridIncidencesClient) != "undefined") {
            if (typeof (showWarning) == "undefined" || showWarning == null) showWarning = true;

            var bResult = false
            if (GridIncidencesClient.IsEditing()) {
                bResult = true;
                if (showWarning == true) {
                    showPopupMoves2("DataEditing.Message", "INFO", "", "DataEditing.OK", "", "", "", "", "");
                }
            }
            return bResult;
        }
        else {
            return true;
        }
    }
    catch (e) {
        showError("IsGridEditing", e);
    }
}

//============ OBTENER DATOS =================================
function btnRefreshClient_Click() {
    if (DateDiff('d', formatDateYMD(txtDateInfClient.GetDate()), formatDateYMD(txtDateSupClient.GetDate())) > 31) {
        var url = "Tasks/srvMsgBoxTask.aspx?action=Message";
        url = url + "&TitleKey=invalidperod.Title";
        url = url + "&DescriptionKey=invalidperod.Description";
        url = url + "&Option1TextKey=invalidperod.Option1Text";
        url = url + "&Option1DescriptionKey=invalidperod.Option1Description";
        url = url + "&Option1OnClickScript=HideMsgBoxForm();return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/Alert32.png";
        parent.ShowMsgBoxForm(url, 400, 300, '');
    }
    else {
        var hdnBussinesCenterSelected = document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnBusinessCentersSelected')

        if (hdnBussinesCenterSelected.value == "") {
            document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnBusinessCentersSelected').value = "0"
        }
        refreshCostPivotGrid();
    }
}

function refreshCostPivotGrid() {
    showLoadingGrid(true);
    setTimeout(function () { GridIncidencesClient.PerformCallback("RELOAD"); }, 1000);
}

function btnRunClient_Click() {
    showCaptcha();
}

//============ SELECTOR EMPLEADOS =================================
function PopupSelectorEmployeesClient_PopUp(s, e) {
    try {
        s.SetHeaderText("");
        var iFrm = document.getElementById("ctl00_contentMainBody_PopupSelectorEmployees_ASPxPanel3_GroupSelectorFrame");
        var strBase = "../Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" +
            "PrefixTree=treeEmpReviewBusinessCenters&FeatureAlias=Calendar.JustifyIncidences&PrefixCookie=objContainerTreeV3_treeEmpReviewBusinessCentersGrid&" +
            "AfterSelectFuncion=parent.GetSelectedTreeV3";
        iFrm.src = strBase;
    }
    catch (e) {
        showError("PopupSelectorEmployeesClient_PopUp", e);
    }
}
//==================================================================
//Guarda los empleados seleccionados en el TreeV3
function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
    if (oParm1 == "") {
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnEmployees').value = "";
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilter').value = "";
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilterUser').value = "";
    }
    else {
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnEmployees').value = oParm1;
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilter').value = oParm2;
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnFilterUser').value = oParm3;
    }
}

function showLoadingGrid(loading) { parent.showLoader(loading); }

//======================SELECTOR CENTROS DE COSTE ==============

function btnOpenPopupSelectorCenterClient_Click() {
    frmBusinessCenterSelector_Show();
}

function ASPxBusinessCentersSelectorCallbackPanelContenidoClient_EndCallBack(s, e) {
    if (s.cp_RefreshScreen == "CANCEL") {
        showWUF('ctl00_contentMainBody_CallbackPanelPivot_frmBusinessCenterSelector', false);
    } else if (s.cp_RefreshScreen == "INIT") {
        showLoadingGrid(false);
        showWUF('ctl00_contentMainBody_CallbackPanelPivot_frmBusinessCenterSelector', true);
    } else if (s.cp_RefreshScreen == "CLOSEWITHSAVE") {
        showWUF('ctl00_contentMainBody_CallbackPanelPivot_frmBusinessCenterSelector', false);
        refreshTree();
    } else if (s.cp_RefreshScreen == "CLOSECALLPARENT") {
        var bcValues = s.cp_BcValues;
        btnPopupBusinessCentersAcceptClient_Click(bcValues);
        showWUF('ctl00_contentMainBody_CallbackPanelPivot_frmBusinessCenterSelector', false);
    }
    showLoadingGrid(false);
    lgGridClient.Hide();
    lpSelectorClient.Hide();
    s.cp_RefreshScreen = ""
}

function btnPopupBusinessCentersAcceptClient_Click(values) {
    showLoadingGrid(true);
    var arrBusinessCenterValues = values;
    var hdnBussinesCenterSelected = document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnBusinessCentersSelected')

    if (arrBusinessCenterValues.length > 0 && arrBusinessCenterValues[0] !== "") {
        hdnBussinesCenterSelected.value = arrBusinessCenterValues;
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_txtBusinessCenters').value = arrBusinessCenterValues.length +
            document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_hdnBusinessCentersSelectedText').value;
    } else {
        hdnBussinesCenterSelected.value = "";
        document.getElementById('ctl00_contentMainBody_CallbackPanelPivot_txtBusinessCenters').value = "";
    }
    showLoadingGrid(false);
    CallbackSessionClient.PerformCallback("RELOADBC");
}