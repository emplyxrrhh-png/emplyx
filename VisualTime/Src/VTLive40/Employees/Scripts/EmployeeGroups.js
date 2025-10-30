var actualTab = 0; // TAB per mostrar
var actualEmployee; // Usuari actual

var actualTabGrupo = 0; // TAB per mostrar els grups
var actualGrupo; // Grup actual

var actualType = "E";
var isCompany = false;
var orgChart = null;

function showLoadingGrid(loading) {
    try {
        parent.showLoader(loading);
    } catch (e) { showError("showLoadingGrid", e); }
}

 function cargaNodo(Nodo) {
    actualGrupo = -1;
    actualEmployee = -1;
    if (Nodo.id.substring(0, 1) == 'A') { //Es un grupo
        if (Nodo.parentNode.text == 'rootnode') {
            isCompany = true;
        }
        else { isCompany = false; }
        cargaGrupo(Nodo.id.substring(1));       
        var element = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnSelectedGroup");
        if (element.length) {
            element.val(Nodo.getPath());
        }
        element = $("#ctl00_contentMainBody_hdnSelectedGroup");
        if (element.length) {
            element.val(Nodo.getPath());
        }        
    } else {
        if (Nodo.id.substring(0, 1) == 'B') { //Es un empleado
            if (Nodo.parentNode != null) {
                if (Nodo.parentNode.id.substring(0, 1) == 'A')
                    actualGrupo = Nodo.parentNode.id.substring(1);
            }
            cargaEmpleado(Nodo.id.substring(1));
        } else {
            //Si no es ni grup ni usuari... no fa res...
        }
    }
}

function changeTabs(tab) {
    if (actualType == "E") {
        changeEmployeeTabs(tab);
    } else {
        changeCompanyTabs(tab);
    }
}

function ASPxCallbackPanelContenidoClient_EndCallBack(s, e) {
    var contenedor2 = document.getElementById('divContent');
    if (actualType == "E") {
        if (typeof txtNameCompany_Client != 'undefined') txtNameCompany_Client.SetValue("___");
        EmployeeDivsLoaded[actualTab] = true;

        changeEmployeeTabs(actualTab, s);

        showEmployeeSection();

        EmployeeDivsLoaded[actualTab] = false;
        if (actualTab == 0) {
            localStorage.setItem("scrollUserFieldEmployeesPosition", "0");
            PrepareProfile();
        }

        if (s.cpActionRO == "GETEMPLOYEE") {
            hasChanges(false);
        } else if (s.cpActionRO == "EDITGRID") {
            showLoadingGrid(false);
        }

        if (actualTab == 1) {
            PrepareHRProfile(s);
        }

        if (actualTab == 2) {
            PrepareAdministration();
        }

        if (s.cpActionRO == "SAVEEMPLOYEEASSIGNMENTS") {
            if (s.cpResultRO == "KO") {
                Robotics.Client.JSErrors.showJSerrorPopup(Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                    { text: '', key: 'roJsError' }, { text: s.cpMessage, key: '' },
                    { text: '', textkey: 'roErrorClose', desc: '', desckey: '', script: '' },
                    Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton(), Robotics.Client.JSErrors.createEmptyButton())
            }
        }
    } else {
        if (typeof txtEmpName_Client != 'undefined') txtEmpName_Client.SetValue("____");
        groupDivsLoaded[actualTabGrupo] = true;
        changeCompanyTabs(actualTabGrupo);
        showGroupSection();
        groupDivsLoaded[actualTabGrupo] = false;
        EnablePanelsFunctionallity();
        if (s.cpActionRO == "GETGROUP") {
            hasChanges(false);
            if (typeof (s.cpReloadRO) != 'undefined' && s.cpReloadRO == true) {
                refreshEmployeeTree(true);
            }
        } else if (s.cpActionRO == "SAVEGROUP") {
            var message = s.cpResultRO.substr(7, s.cpResultRO.length - 7)
            var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);
            parent.ShowMsgBoxForm(url, 500, 300, '');
            hasChanges(true);
        }
    }
    showLoadingGrid(false);
}

function saveChanges() {
    try {
        if (actualType == "E") {
            saveChangesEmployee();
        } else {
            saveChangesGroup();
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChanges() {
    try {
        if (actualType == "E") {
            undoChangesEmployee();
        } else {
            undoChangesGroup();
        }
    } catch (e) { showError("undoChanges", e); }
}

function showEmployeeSection() {
    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_employeeRow').show();
    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_companyRow').hide();
}

function showGroupSection() {
    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_employeeRow').hide();
    $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_companyRow').show();

    if (isCompany == true && actualTabGrupo == 1) {
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_panGroupDocs').hide();
        $('#ctl00_contentMainBody_ASPxCallbackPanelContenido_panGroupDocsCompany').show();
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

//Mostra el ToolTip a la barra d'eines
function showTbTip(tip) {
    document.getElementById(tip).style.display = '';
}

//Amaga el ToolTip a la barra d'eines
function hideTbTip(tip) {
    document.getElementById(tip).style.display = 'none';
}

//Refresh de les pantalles (RETORN)
function RefreshScreen(MustRefresh, Params) {
    var bReload = false;

    if (typeof (Params) == "undefined" || Params == null) Params = "";
    if (MustRefresh == '5') {
        if (Params.indexOf("DELETE_EMPLOYEE#") != -1) {
            var arParams = Params.split("#");
            if (arParams.length == 2) removeEmployee(arParams[1]);
        } else if (Params.indexOf("DELETE_PROGRAMMED_ABSENCE#") != -1) {
            var arParams = Params.split("#");
            if (arParams.length == 2) eval("removeGridAus(" + arParams[1] + ");"); //alert('delete programmed absence');
        } else if (Params.indexOf("DELETE_PROGRAMMED_CAUSE#") != -1) {
            var arParams = Params.split("#");
            if (arParams.length == 2) eval("removeGridInc(" + arParams[1] + ");"); //alert('delete programmed cause');
        }
    } else if (MustRefresh == '10') {
        var message = "TitleKey=PassportDeleted.Title&" +
            "DescriptionKey=PassportDeleted.Description&" +
            "Option1TextKey=PassportDeleted.Option1Text&" +
            "Option1DescriptionKey=PassportDeleted.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png"
        var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);
        parent.ShowMsgBoxForm(url, 500, 300, '');
    } else if (MustRefresh.substring(0, 1) == '9') {
        var message = "TitleKey=InfoPwd.Title&" +
            "DescriptionText=" + MustRefresh.substring(2) + "&" +
            "Option1TextKey=InfoPwd.Option1Text&" +
            "Option1DescriptionKey=InfoPwd.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/dialog-information.png"
        var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);
        parent.ShowMsgBoxForm(url, 500, 300, '');
    }

    if (MustRefresh == '4' || MustRefresh == '8' || MustRefresh == '1') {
        //Movilidades dentro de los grupos, creacion de grupos y usuarios provocamos fullreload
        bReload = true;
    }

    refreshEmployeeTree(bReload);
}

function ShowReports(Title, ReportsTitle, ReportsType, DefaultReportsVersion, RootURL) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != '') Title = Title + ' - ' + ReportsTitle;
        parent.ShowExternalForm('Reports/Reports.aspx', 900, 570, Title, 'ReportsType', ReportsType);
    } else {
        parent.reenviaFrame('/' + RootURL + '//Report', '', 'Reports', 'Portal\Reports\AdvReport');
    }
}

function showErrorPopup(Title, typeIcon, Description, Opt1Text, Opt1Desc, strScript1, Opt2Text, Opt2Desc, strScript2, Opt3Text, Opt3Desc, strScript3) {
    try {
        var url = "Employees/srvMsgBoxEmployees.aspx?action=MessageEx";
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

function ASPxOrgChartClientEndCallBack(s, e) {
    switch (s.cpAction) {
        case "GETSECURITYCHART":
            if (checkResult(s)) {
                orgChart.parseResponse(s, e);
            }
            break;
        case "SAVESECURITYCHART":
            break;
        default:
            hasChanges(false);
    }
}

function checkResult(oResult) {
    if (oResult.cpResult == 'KO') {
        showErrorPopup("SaveName.Error.Text", "ERROR", "", oResult.cpMessage, "SaveName.Error.Option1Text", "SaveName.Error.Option1Description", "", "", "", "", "", "", "");
        return false;
    }
    return true;
}