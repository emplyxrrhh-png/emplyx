var EmployeeArrButtons = new Array('TABBUTTON_GeneralEmployees', 'TABBUTTON_UserField', 'TABBUTTON_Contract', 'TABBUTTON_Punches', 'TABBUTTON_Absence', 'TABBUTTON_Assignment', 'TABBUTTON_Supervisors', 'TABBUTTON_Consents');
var EmployeeArrDivs = new Array('panEmpGeneral', 'panEmpUserFields', 'panEmpContratos', 'panEmpFichajes', 'panEmpAusPrev', 'panEmpAssignments', 'panActiveNotifications', 'divConsents');
var EmployeeDivsLoaded = new Array(false, false, false, false, false, false, false, false);

function checkEmployeeEmptyName(newName) {
    hasChanges(true);
}

//Carga Tabs y contenido Empleados
function cargaEmpleado(IDEmpleado) {
    for (n = 0; n < EmployeeDivsLoaded.length; n++) {
        EmployeeDivsLoaded[n] = false;
    }
    actualEmployee = IDEmpleado;
    //TAB Gris Superior
    actualType = "E";
    showLoadingGrid(true);
    cargaEmpleadoTabSuperior(IDEmpleado);
}

function saveChangesEmployee() {
    try {
        if (actualTab == 0) {
            if (ASPxClientEdit.ValidateGroup('employeenameGroup', true)) {
                saveName(actualEmployee);
            } else {
                showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationFieldsFailed", "Error.OK", "Error.OKDesc", "");
            }
        }
        if (actualTab == 5) {
            if (!GridSuitabilityClient.IsEditing()) {
                saveAssignments(actualEmployee);
            } else {
                showErrorPopup("Error.ValidationTitle", "error", "Error.ValidationGridIsEditingError", "Error.OK", "Error.OKDesc", "");
            }
        }
        if (actualTab == 2) {
            saveEmployeeForgottenRight(actualEmployee);
        }
    } catch (e) { showError("saveChanges", e); }
}

function undoChangesEmployee() {
    try {
        showLoadingGrid(true);
        cargaEmpleado(actualEmployee);
    } catch (e) { showError("undoChanges", e); }
}

var responseObj = null;
// Carrega la part del TAB grisa superior
function cargaEmpleadoTabSuperior(IDEmpleado) {
    var Url = "";

    Url = "Handlers/srvEmployees.ashx?action=getBarButtons&ID=" + IDEmpleado;
    AsyncCall("POST", Url, "JSON3", responseObj, "parseResponseBarButtons(objContainerId," + IDEmpleado + ")");
}

function parseResponseBarButtons(oResponse, IDEmpleado) {
    var container = document.getElementById("divBarButtons");
    if (container != null) container.innerHTML = oResponse.BarButtons;

    window.parent.setUPReportsAndWizards(oResponse);

    cargaEmpleadoDivCentral(IDEmpleado);
}

function cargaEmpleadoDivCentral(IDEmpleado) {
    var Url = "";

    Url = "Handlers/srvEmployees.ashx?action=getEmployeeTab&aTab=" + actualTab + "&ID=" + IDEmpleado;
    AsyncCall2("POST", Url, "CONTAINER", "divEmpleados", "changeEmployeeTabs(" + actualTab + ")");
}

//Cambia els Tabs i els divs
function changeEmployeeTabs(numTab, callbackObject) {
    for (n = 0; n < EmployeeArrButtons.length; n++) {
        var tab = document.getElementById(EmployeeArrButtons[n]);
        var div = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_" + EmployeeArrDivs[n]);
        if (tab != null && div != null) {
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
    if (numTab == 0) {
        var summaryEmployee = null;
        if (typeof callbackObject != 'undefined' && typeof callbackObject.cp_EmployeeSummary != 'undefined' && callbackObject.cp_EmployeeSummary != null) summaryEmployee = callbackObject.cp_EmployeeSummary;

        if (EmployeeDivsLoaded[numTab] == true && typeof summaryEmployee != 'undefined' && summaryEmployee != null) {
            LoadEmployeeSummary(cmbSummaryPeriodClient?.GetSelectedItem()?.value);
        }
    }

    if (EmployeeDivsLoaded[numTab] == false) {
        showLoadingGrid(true);
        cargaEmpleadoDivs(actualEmployee, actualTab);
    }
}

// Carrega els apartats dels divs de l'usuari
function cargaEmpleadoDivs(IDEmployee, actualTab) {
    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = IDEmployee;
    oParameters.oType = "E";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "GETEMPLOYEE";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function enableSaveHighlight(s, e) {
    try {
        var divCol = document.getElementById('tbSaveHighlight');
        if (divCol == null) { return; }
        divCol.style.display = '';
        s.GetInputElement().style.display = 'none';
    } catch (e) { showError("enableSaveHighlight", e); }
}

function saveHighlight() {
    try {
        var postParams = 'highlightColor=' + encodeURIComponent(dxColorPickerClient.color) + "&ID=" + actualEmployee;
        var stamp = '&StampParam=' + new Date().getMilliseconds();

        ajax = nuevoAjax();
        ajax.open("POST", "Handlers/srvEmployees.ashx?action=updateHighlightColor" + stamp, true);
        ajax.setRequestHeader("Content-type", "application/x-www-form-urlencoded");

        ajax.onreadystatechange = function () {
            if (ajax.readyState == 4) {
                if (ajax.responseText == 'OK') {
                    var divCol = document.getElementById('tbSaveHighlight');
                    if (divCol == null) { return; }
                    divCol.style.display = 'none';
                } else {
                    if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                        var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                    }
                }
            }
        }
        ajax.send(postParams);
    } catch (e) { showError("saveHighlight", e); }
}

function editGrid(IDEmpleado) {
    showLoadingGrid(true);

    var scrollPosition = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divGrid").scrollTop();
    localStorage.setItem("scrollUserFieldEmployeesPosition", scrollPosition);

    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = IDEmpleado;
    oParameters.oType = "E";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "EDITGRID";
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function saveGrid(IDEmpleado) {
    showLoadingGrid(true);

    var inputClientsIDs = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_hdnEmployeeFieldsIDs').value.split("#");
    var nTotalFields = inputClientsIDs.length;

    var processedInputs = new Array();

    var postvars = '';
    for (n = 0; n < nTotalFields; n++) {
        var devVar = undefined;
        var devVarDate = undefined;

        var clientID = inputClientsIDs[n];
        var clientDateID = ""

        try {
            clientDateID = "try{devVarDate = " + clientID.replace("_BeginPeriod", "").replace("_LinkShow", "").replace("_Client", "_Date_Client") + "; }catch(e){ devVarDate = undefined;}";
            clientID = "try{devVar = " + clientID + "; }catch(e){ devVar = undefined;}";

            eval(clientID);
            eval(clientDateID);
        } catch (e) {
            devVar = undefined;
            devVarDate = undefined;
        }

        var bolSend = false
        if (typeof (devVarDate) != 'undefined' && devVarDate.cpIsEddited == true) {
            bolSend = true;
        }

        if (typeof (devVar) != 'undefined' && devVar.cpIsEddited == true) {
            bolSend = true;
        }

        if (bolSend) {
            if ($.inArray(devVar.cpOriginalFieldName, processedInputs) == -1) {
                if (devVar.cpObjectType == 'date') {
                    if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue().ToStringDate("D/M/Y", "/"));
                    else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';
                } if (devVar.cpObjectType == 'text') {
                    if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue());
                    else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';
                } if (devVar.cpObjectType == 'combo') {
                    if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue());
                    else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';
                } if (devVar.cpObjectType == 'time') {
                    if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue().format2Time());
                    else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';
                } if (devVar.cpObjectType == 'timeP') {
                    if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue().format2Time());
                    else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';

                    var endPeriodVar = undefined;
                    try {
                        endPeriodVar = clientID.replace("_BeginPeriod", "_EndPeriod")
                        eval(endPeriodVar);

                        if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue().format2Time());
                        else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';
                    } catch (e) {
                        endPeriodVar = undefined;
                    }
                } if (devVar.cpObjectType == 'dateP') {
                    if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue().ToStringDate("D/M/Y", "/"));
                    else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';

                    var endPeriodVar = undefined;
                    try {
                        endPeriodVar = clientID.replace("_BeginPeriod", "_EndPeriod")
                        eval(endPeriodVar);

                        if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue().ToStringDate("D/M/Y", "/"));
                        else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';
                    } catch (e) {
                        endPeriodVar = undefined;
                    }
                } if (devVar.cpObjectType == 'link') {
                    if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue());
                    else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';

                    var endPeriodVar = undefined;
                    try {
                        endPeriodVar = clientID.replace("_LinkShow", "_LinkReal")
                        eval(endPeriodVar);

                        if (devVar.GetValue() != null) postvars += '&USR_' + devVar.cpOriginalFieldName + '=' + encodeURIComponent(devVar.GetValue())
                        else postvars += '&USR_' + devVar.cpOriginalFieldName + '=';
                    } catch (e) {
                        endPeriodVar = undefined;
                    }
                }

                if (typeof (devVarDate) != 'undefined') {
                    postvars += '&USR_' + devVarDate.cpOriginalFieldName + '=' + devVarDate.GetDate().ToStringDate("D/M/Y", "/");
                }

                processedInputs.push(devVar.cpOriginalFieldName);
            }
        }
    }

    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = IDEmpleado;
    oParameters.oType = "E";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEEMPLOYEEFIELDS";
    oParameters.resultClientAction = postvars;
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    var scrollPosition = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divGrid").scrollTop();
    localStorage.setItem("scrollUserFieldEmployeesPosition", scrollPosition);
    

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function UserFieldValueChange(FieldName, DayDate) {
    var inputDate = undefined;

    eval('try{inputDate = ' + FieldName + '_Date_Client;}catch(e){}');
    if (typeof (inputDate) != "undefined") {
        if (inputDate.GetVisible() == true) {
            inputDate.SetValue(new Date())
            inputDate.cpIsEddited = true;
        }
    }
}

function cancelEditGridE(IDEmpleado) {
    showLoadingGrid(true);
    actualTab = 1;
    cargaEmpleadoDivs(IDEmpleado, actualTab);
}

function showLoadingUserFields(loading) {
    var img = document.getElementById('imgUserFieldsLoading');
    if (img != null) {
        if (loading == true) {
            img.style.display = '';
        }
        else {
            img.style.display = 'none';
        }
    }
}

function btnOpenPopupAlertDocumentsClient_Click() {
    documentAlertsClient.Show();
}

function documentAlertsClient_PopUp(e) {
}

function ShowEmployeeTypes(idEmployee) {
    var Title = ''; //document.getElementById('spanIdentify').innerHTML;
    parent.ShowExternalForm2('Employees/EmployeeType.aspx?EmployeeID=' + idEmployee, 500, 520, Title, '', false, false);
}

function ShowLockDate(idEmployee, lockDate, lockDateType) {
    var Title = '';
    parent.ShowExternalForm2('Employees/LockDate.aspx?EmployeeID=' + idEmployee + '&lockDate=' + lockDate + '&lockDateType=' + lockDateType, 500, 540, Title, '', false, false);
}

function ShowContracts(idEmployee) {
    var Title = ''; //document.getElementById('spanContracts').innerHTML;
    parent.ShowExternalForm2('Employees/EmployeeContracts.aspx?EmployeeID=' + idEmployee, 1650, 800, Title, '', false, false);
}

function ShowMoveCurrentEmployee(idEmployee) {
    var Title = ''; //$get('<%= lblMoveEmployeeFormTitle.ClientID %>').innerHTML;
    parent.ShowExternalForm2('Employees/MoveEmployee.aspx?EmployeeID=' + idEmployee, 410, 490, Title, '', false, false);
}
function ShowMobility(idEmployee) {
    var Title = ''; //document.getElementById('spanMobility').innerHTML;
    parent.ShowExternalForm2('Employees/EmployeeEditMobility.aspx?EmployeeID=' + idEmployee, 920, 500, Title, '', false, false);
}

function EditAssignments(idEmployee) {
    var Title = ''; //document.getElementById('spanMobility').innerHTML;
    parent.ShowExternalForm2('Employees/EmployeeEditAssignments.aspx?EmployeeID=' + idEmployee, 420, 440, Title, '', false, false);
}

function ShowIdentifyMethods(idEmployee) {
    var Title = ''; //document.getElementById('spanIdentify').innerHTML;
    parent.ShowExternalForm2('Employees/EmployeeIdentifyMethods.aspx?EmployeeID=' + idEmployee, 1055, 645, Title, '', false, false);
}

function ShowEmployeePermissions(idEmployee) {
    var Title = ''; //document.getElementById('spanEmployeePermissions').innerHTML;
    parent.ShowExternalForm2('Employees/EmployeePermissions.aspx?EmployeeID=' + idEmployee + '&PermissionsType=E', 640, 530, Title, '', false, false);
}

function ShowApplicationsEmployee(idEmployee) {
    var Title = ''; //document.getElementById('spanIdentify').innerHTML;
    parent.ShowExternalForm2('Employees/EmployeeAllowedApplications.aspx?EmployeeID=' + idEmployee, 650, 580, Title, '', false, false);
}

function ShowAccessAuthorizations(idEmployee, idGroup) {
    var Title = '';
    parent.ShowExternalForm2('Employees/EmployeeEditAuthorizations.aspx?EmployeeID=' + idEmployee + '&GroupID=' + idGroup, 480, 440, Title, '', false, false);
}

function ShowEmployeeMessages(idEmployee) {
    var Title = '';
    parent.ShowExternalForm2('Employees/EmployeeEditMessages.aspx?EmployeeID=' + idEmployee, 480, 440, Title, '', false, false);
}

function ShowRemoveEmployeeData(idEmployee) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvEmployees.ashx?action=employeeHasData&ID=" + idEmployee + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            var strResponse = ajax.responseText;

            if (strResponse.substr(0, 6) == 'NODATA') {
                ShowCaptchaRemoveEmployee(idEmployee);
            } else {
                var Title = '';
                parent.ShowExternalForm2('Employees/EmployeeRemoveData.aspx?EmployeeID=' + idEmployee, 480, 470, Title, '', false, false);
            }
        }
    }

    ajax.send(null);
}

function ShowCenters(idEmployee) {
    var Title = '';
    parent.ShowExternalForm2('Employees/EmployeeCenters.aspx?EmployeeID=' + idEmployee, 700, 440, Title, '', false, false);
}

function ASPxCallbackSessionPClient_EndCallBack(s, e) {
    switch (s.cpActionRO) {
        case "GETLABAGREESTARTUP":
            LoadAntValues(s);
            break;
    }

    showLoadingGrid(false);
}

function holidaysSummaryEndCallback(s, e) {
    setTimeout(function () {
        $("#divHolidaysSummaryClick").click(function () {
            //getting the next element
            $content = $("#collapsableHolidaysSummary");
            //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
            $content.slideToggle(500, function () {
            });
        });
    }, 500);    
}

//==========================================================================
//Guarda los empleados seleccionados en el TreeV3
function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
    if (oParm1 == "") {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_GroupDocumentManagmentCompany_CallbackSessionP_hdnEmployees').value = "";
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_GroupDocumentManagmentCompany_CallbackSessionP_hdnFilter').value = "";
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_GroupDocumentManagmentCompany_CallbackSessionP_hdnFilterUser').value = "";
    }
    else {
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_GroupDocumentManagmentCompany_CallbackSessionP_hdnEmployees').value = oParm1;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_GroupDocumentManagmentCompany_CallbackSessionP_hdnFilter').value = oParm2;
        document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_GroupDocumentManagmentCompany_CallbackSessionP_hdnFilterUser').value = oParm3;
    }
}
function PrepareAdministration() {
    $("#divContractsClick").click(function () {
        //getting the next element
        $content = $("#collapsibleContracts");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });
}

function PrepareProfile() {
    $("#divProfileClick").click(function () {
        //getting the next element
        $content = $("#collapsableProfile");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });

    $("#divPunchClick").click(function () {
        //getting the next element
        $content = $("#collapsablePunch");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });

    $("#divPlanClick").click(function () {
        //getting the next element
        $content = $("#collapsablePlan");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });
    
    $("#divHolidaysSummaryClick").click(function () {        
        //getting the next element
        $content = $("#collapsableHolidaysSummary");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });

    $("#divSummaryClick").click(function () {
        //getting the next element
        $content = $("#collapsableSummary");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });
}
function PrepareHRProfile(s) {
    if (typeof s.cpOnBoardingResult != 'undefined') {
        var tasks = JSON.parse(s.cpOnBoardingResult)
        LoadOnboardingList(tasks);
    }
    $("#divUFClick").click(function () {
        //getting the next element
        $content = $("#collapsableUserFields");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });

    $("#divDocClick").click(function () {
        //getting the next element
        $content = $("#collapsableDocuments");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });

    $("#divOnboardingClick").click(function () {
        //getting the next element
        $content = $("#collapsableOnboarding");
        //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
        $content.slideToggle(500, function () {
        });
    });

    if (localStorage.scrollUserFieldEmployeesPosition) {
        $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divGrid").scrollTop(localStorage.getItem("scrollUserFieldEmployeesPosition"));
    }
}

function LoadOnboardingList(s) {
    acumValues = [];

    if (s != null) {
        for (var i = 0; i < s.length; i++) {
            acumValues.push({
                Id: s[i].Id,
                Title: s[i].TaskName,
                LastModifiedBy: s[i].SupervisorName,
                LastModified: s[i].LastChangeDate,
                Status: s[i].Done
            });
        }
    }

    gridValues = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_divOnboardingGrid").dxDataGrid({
        id: "gridOnboardingEmp",
        showColumnLines: false,
        showRowLines: false,
        rowAlternationEnabled: false,
        showBorders: false,
        headerFilter: { visible: false },
        allowColumnResizing: true,
        filterRow: { visible: false },
        dataSource: {
            store: {
                type: 'array',
                key: 'Id',
                data: acumValues
            }
        },
        editing: {
            mode: "row",
            allowUpdating: false,
            allowAdding: false,
            allowDeleting: false,
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
                e.cellElement.css({ "backgroundColor": "#fff" });
            }
            else if (e.rowType === 'header') {
                e.cellElement.css({ "backgroundColor": "#8a9da3" });
                e.cellElement.css({ "color": "#fff" });
                e.cellElement.css({ "fontWeight": "600" });
                e.cellElement.css({ "fontSize": "12px" });
                e.cellElement.css({ "border": "0px !important" });
            }
        },
        onRowPrepared: function (e) {
            e.rowElement.css({ height: 30 });
            e.rowElement.css({ "border": "0px !important" });
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
            { caption: (Globalize.formatMessage('Id') != null && Globalize.formatMessage('Id').length > 0 ? Globalize.formatMessage('Id') : 'Id'), dataField: "Id", allowEditing: false, allowDeleting: false, visible: false, alignment: "center" },
            { caption: (Globalize.formatMessage('Status') != null && Globalize.formatMessage('Status').length > 0 ? Globalize.formatMessage('Status') : 'Estado'), dataField: "Status", allowEditing: false, allowDeleting: false, alignment: "center" },
            { caption: (Globalize.formatMessage('Task') != null && Globalize.formatMessage('Task').length > 0 ? Globalize.formatMessage('Task') : 'Tarea'), dataField: "Title", allowEditing: false, allowDeleting: false, alignment: "center" },
            { caption: (Globalize.formatMessage('UpdatedOnDay') != null && Globalize.formatMessage('UpdatedOnDay').length > 0 ? Globalize.formatMessage('UpdatedOnDay') : 'Actualizado el día'), dataField: "LastModified", allowEditing: false, allowDeleting: false, dataType: "date", format: "dd/MM/yyyy", alignment: "center" },
            { caption: (Globalize.formatMessage('By') != null && Globalize.formatMessage('By').length > 0 ? Globalize.formatMessage('By') : 'Por'), dataField: "LastModifiedBy", allowEditing: false, allowDeleting: false, alignment: "center" },

        ]
    }).dxDataGrid("instance");
}

function saveName(ID) {
    var txtName = txtEmpName_Client.GetValue();
    var txtLanguage = cmbLanguage_Client.GetSelectedItem().value;
    var isProductiv = ckProductiveYes_client.GetValue();

    var stamp = '&StampParam=' + new Date().getMilliseconds();

    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvEmployees.ashx?action=chgName&ID=" + ID + "&NewName=" + encodeURIComponent(txtName) + "&IsProductiv=" + encodeURIComponent(isProductiv) + "&NewLang=" + txtLanguage + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            var strResponse = ajax.responseText;

            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.substr(7, strResponse.length - 7));
                parent.ShowMsgBoxForm(url, 500, 300, '');
            }
            else {
                if (strResponse == 'OK') {
                    document.getElementById('readOnlyNameEmp').textContent = txtName;
                    // Modificamos el nombre en los árboles d'empleado
                    refreshEmployeeTree(true);
                }
            }
        }
    }
    ajax.send(null)
}

function saveEmployeeForgottenRight(ID) {
    var hasForgottenRight = ckForgottenRightYes_client.GetValue();

    var stamp = '&StampParam=' + new Date().getMilliseconds();

    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvEmployees.ashx?action=chgForgottenRight&ID=" + ID + "&HasForgottenRight=" + encodeURIComponent(hasForgottenRight) + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            var strResponse = ajax.responseText;

            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.substr(7, strResponse.length - 7));
                parent.ShowMsgBoxForm(url, 500, 300, '');
            }
            else {
                if (strResponse == 'OK') {
                    hasChanges(false);
                }
            }
        }
    }

    ajax.send(null)
}

function AddNewProgrammedAbsence(idEmployee) {
    var url = 'Employees/EditProgrammedAbsence.aspx?EmployeeID=' + idEmployee + '&NewRecord=1';
    var Title = '';
    parent.ShowExternalForm2(url, 830, 485, Title, '', false, false, false);
}

function AddNewProgrammedIncidence(idEmployee) {
    var url = 'Employees/EditProgrammedIncidence.aspx?EmployeeID=' + idEmployee + '&NewRecord=1';
    var Title = '';
    parent.ShowExternalForm2(url, 830, 485, Title, '', false, false, false);
}

function editGridAus(IDCause, ID, BeginDate) {
    var url = 'Employees/EditProgrammedAbsence.aspx?EmployeeID=' + ID + '&BeginDate=' + encodeURIComponent(BeginDate);
    var Title = ''
    parent.ShowExternalForm2(url, 830, 485, Title, '', false, false, false);
}

function editGridInc(IDCause, ID, BeginDate, IDAbsence) {
    var url = 'Employees/EditProgrammedIncidence.aspx?EmployeeID=' + ID + '&BeginDate=' + encodeURIComponent(BeginDate) + '&AbsenceID=' + IDAbsence;
    var Title = '';
    parent.ShowExternalForm2(url, 830, 485, Title, '', false, false, false);
}

function ShowRemoveGridAus(IDCause, ID, BeginDate) {
    var url = 'Employees/DeleteAbsenceCaptcha.aspx?AbsenceType=0&IdProgrammedHoliday=-1&IdProgrammedOvertime=-1&IdEmployee=' + ID + '&BeginDate=' + encodeURIComponent(BeginDate) + '&IdAbsence=' + 1;
    parent.ShowExternalForm2(url, 450, 250, '', '', false, false);
}

function ShowRemoveGridInc(IDCause, ID, BeginDate, IDAbsence) {
    var url = 'Employees/DeleteAbsenceCaptcha.aspx?AbsenceType=1&IdProgrammedHoliday=-1&IdProgrammedOvertime=-1&IdEmployee=' + ID + '&BeginDate=' + encodeURIComponent(BeginDate) + '&IdAbsence=' + IDAbsence;
    parent.ShowExternalForm2(url, 450, 250, '', '', false, false);
}

function editGridHolidays(idProgrammedHolidaysm, idEmployee) {
    var url = 'Employees/EditProgrammedHolidays.aspx?ProgrammedHolidaysID=' + idProgrammedHolidaysm + '&EmployeeID=' + idEmployee
    var Title = '';
    parent.ShowExternalForm2(url, 830, 500, Title, '', false, false, false);
}

function ShowRemoveHolidaysInc(idProgrammedHolidays) {
    var url = 'Employees/DeleteAbsenceCaptcha.aspx?AbsenceType=2&IdProgrammedHoliday=' + idProgrammedHolidays + '&IdProgrammedOvertime=-1&IdEmployee=-1&BeginDate=1970/01/01&IdAbsence=-1';
    parent.ShowExternalForm2(url, 450, 250, '', '', false, false);
}

function editGridOvertime(idProgrammedOvertimes, idEmployee) {
    var url = 'Employees/EditProgrammedOvertimes.aspx?ProgrammedHolidaysID=' + idProgrammedOvertimes + '&EmployeeID=' + idEmployee
    var Title = '';
    parent.ShowExternalForm2(url, 830, 485, '', '', false, false);
}

function ShowRemoveOvertimeInc(idProgrammedOvertime) {
    var url = 'Employees/DeleteAbsenceCaptcha.aspx?AbsenceType=3&IdProgrammedHoliday=-1&IdProgrammedOvertime=' + idProgrammedOvertime + '&IdEmployee=-1&BeginDate=1970/01/01&IdAbsence=-1';
    parent.ShowExternalForm2(url, 450, 250, '', '', false, false);
}

function removeGridAus(IDCause, ID, BeginDate) {
    var contenedor;
    contenedor = document.getElementById('gridAusenciasPrevistas');

    var stamp = '&StampParam=' + new Date().getMilliseconds();

    showLoadingGrid(true);
    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvEmployees.ashx?action=deleteProgAus&IDCause=" + IDCause + "&ID=" + ID + "&BeginDate=" + encodeURIComponent(BeginDate) + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            showLoadingGrid(false);
            var strResponse = ajax.responseText;
            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var oKey = strResponse.substr(7, strResponse.length - 7);
                var message = "TitleKey=DeleteAus.InFreezeDate.Text&" +
                    "DescriptionKey=DeleteAus.InFreezeDate.Description&" +
                    "Option1TextKey=DeleteAus.InFreezeDate.Option1Text&" +
                    "Option1DescriptionKey=DeleteAus.InFreezeDate.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png"
                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);

                parent.ShowMsgBoxForm(url, 500, 300, '');
            } else {
                cargaEmpleadoDivs(actualEmployee, actualTab);
            }
        }
    }

    ajax.send(null)
}

function removeGridInc(IDCause, ID, BeginDate, IDAbsence) {
    var contenedor;
    contenedor = document.getElementById('gridIncidenciasPrevistas');

    showLoadingGrid(true);
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvEmployees.ashx?action=deleteProgInc&IDCause=" + IDCause + "&ID=" + ID + "&BeginDate=" + encodeURIComponent(BeginDate) + "&IDAbsence=" + IDAbsence + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            showLoadingGrid(false);

            var strResponse = ajax.responseText;
            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var oKey = strResponse.substr(7, strResponse.length - 7);
                var message = "TitleKey=DeleteAus.InFreezeDate.Text&" +
                    "DescriptionKey=DeleteAus.InFreezeDate.Description&" +
                    "Option1TextKey=DeleteAus.InFreezeDate.Option1Text&" +
                    "Option1DescriptionKey=DeleteAus.InFreezeDate.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png"
                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);

                parent.ShowMsgBoxForm(url, 500, 300, '');
            } else {
                cargaEmpleadoDivs(actualEmployee, actualTab);
            }
        }
    }

    ajax.send(null)
}

function removeGridHolidays(IdProgrammedHoliday) {
    var contenedor;
    contenedor = document.getElementById('gridAusenciasPrevistas');

    var stamp = '&StampParam=' + new Date().getMilliseconds();

    showLoadingGrid(true);
    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvEmployees.ashx?action=deleteProgHolidays&IDProgHoliday=" + IdProgrammedHoliday + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            showLoadingGrid(false);
            var strResponse = ajax.responseText;
            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var oKey = strResponse.substr(7, strResponse.length - 7);
                var message = "TitleKey=DeleteAus.InFreezeDate.Text&" +
                    "DescriptionKey=DeleteAus.InFreezeDate.Description&" +
                    "Option1TextKey=DeleteAus.InFreezeDate.Option1Text&" +
                    "Option1DescriptionKey=DeleteAus.InFreezeDate.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png"
                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);

                parent.ShowMsgBoxForm(url, 500, 300, '');
            } else {
                cargaEmpleadoDivs(actualEmployee, actualTab);
            }
        }
    }

    ajax.send(null)
}

function removeGridOvertimes(IdProgrammedOvertime) {
    var contenedor;
    contenedor = document.getElementById('gridAusenciasPrevistas');

    var stamp = '&StampParam=' + new Date().getMilliseconds();

    showLoadingGrid(true);
    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvEmployees.ashx?action=deleteProgOvertime&IDProgOvertime=" + IdProgrammedOvertime + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            showLoadingGrid(false);
            var strResponse = ajax.responseText;
            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var oKey = strResponse.substr(7, strResponse.length - 7);
                var message = "TitleKey=DeleteAus.InFreezeDate.Text&" +
                    "DescriptionKey=DeleteAus.InFreezeDate.Description&" +
                    "Option1TextKey=DeleteAus.InFreezeDate.Option1Text&" +
                    "Option1DescriptionKey=DeleteAus.InFreezeDate.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png"
                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);

                parent.ShowMsgBoxForm(url, 500, 300, '');
            } else {
                cargaEmpleadoDivs(actualEmployee, actualTab);
            }
        }
    }

    ajax.send(null)
}

function ShowUserFieldHistory(IDEmployee, FieldName) {
    top.ShowExternalForm2('Employees/UserFieldHistory.aspx?Type=0&IDSource=' + IDEmployee + '&FieldName=' + encodeURIComponent(FieldName), 500, 300, '', '', false, false, false);
}

function ShowUserFieldValue(aAnchor, CountField, IDEmployee, FieldName) {
    if (FieldName == null) FieldName = '';

    var stamp = '&StampParam=' + new Date().getMilliseconds();

    showLoadingGrid(true);

    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvEmployees.ashx?action=AuditUserFieldQuery&ID=" + IDEmployee + "&FieldName=" + FieldName + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            showLoadingGrid(false);

            var strResponse = ajax.responseText;

            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.substr(7, strResponse.length - 7));
                parent.ShowMsgBoxForm(url, 500, 300, '');
            }
            else {
                if (strResponse == 'OK') {
                    aAnchor.style.display = 'none';
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aHideValue_' + CountField).style.display = '';
                    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divShowValue_' + CountField).style.display = '';
                }
            }
        }
    }

    ajax.send(null)
}

function HideUserFieldValue(aAnchor, CountField) {
    aAnchor.style.display = 'none';
    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aShowValue_' + CountField).style.display = '';
    document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_divShowValue_' + CountField).style.display = 'none';
}

function ShowMultiMobilityEmployee() {
    var Title = '';
    var url = 'Employees/Wizards/MultiEmployeeMobilityWizard.aspx';
    top.ShowExternalForm2(url, 800, 500, Title, '', false, false, false);
}

function ShowEmployeeMultiAbsences() {
    var Title = '';
    var url = 'Employees/Wizards/MultiAbsencesWizard.aspx';
    top.ShowExternalForm2(url, 800, 610, Title, '', false, false, false);
}

function ShowEmployeeMassMarkConsents() {
}

function ShowLockDateWizard() {
    var Title = '';
    var url = 'Employees/Wizards/LockDateWizard.aspx';
    top.ShowExternalForm2(url, 800, 620, Title, '', false, false, false);
}

function ShowSecurityActionsWizard(type) {
    if (type == 'e') {
        var Title = '';
        var url = 'Security/Wizards/PassportSecurityActions.aspx?typeWizard=e';
        top.ShowExternalForm2(url, 500, 600, Title, '', false, false, false);
    }
}

function ShowWizardEmployeeMessage() {
    var Title = '';
    var url = 'Employees/Wizards/MessagesWizard.aspx';
    top.ShowExternalForm2(url, 800, 510, Title, '', false, false, false);
}

function ShowPrintAbsences(idEmployee) {
    var Title = '';
    var url = 'Employees/Wizards/PrintAbsences.aspx';
    top.ShowExternalForm2(url, 1000, 380, Title, 'EmployeeID=' + idEmployee, false, false, false);
}

function ShowNewEmployeeWizard() {
    if (actualGrupo > 0) {
        var Title = '';
        var url = 'Employees/Wizards/NewMultiEmployeeWizard.aspx?GroupID=' + actualGrupo;
        top.ShowExternalForm2(url, 800, 520, Title, '', false, false, false);
    } else {
        showErrorPopup("Error.SelectionTitle", "error", "Error.MustSelectAGroup", "Error.OK", "Error.OKDesc", "");
    }
}

function ShowDocumentsDownload() {
    var Title = '';
    var url = 'Employees/DocumentAllEmployees.aspx';
    top.ShowExternalForm2(url, 800, 450, Title, '', false, false, false);
}

function ShowEmployeeCopyWizard(IDEmployee) {
    var Title = ''
    top.ShowExternalForm2('Employees/Wizards/EmployeeCopyWizard.aspx', 800, 545, Title, 'IDEmployeeSource=' + IDEmployee, false, false, false);
}

function ShowChangeEmployeeImage(ID) {
    var Title = '';
    top.ShowExternalForm2('Employees/ChangeEmployeeImage.aspx?ID=' + ID, 350, 150, Title, '', '', false, false);
}

function ShowRemoveEmployee(ID) {
    var url = "Employees/srvMsgBoxEmployees.aspx?action=DeleteEmp&ID=" + ID;
    parent.ShowMsgBoxForm(url, 400, 300, '');
}

function ShowCaptchaRemoveEmployee(ID) {
    var Title = '';
    parent.ShowExternalForm2('Employees/DeleteEmployeeCaptcha.aspx', 450, 250, Title, 'IdEmployee=' + ID, false, false);
}

function removeEmployee(ID) {
    var stamp = '&StampParam=' + new Date().getMilliseconds();

    showLoadingGrid(true);
    ajax = nuevoAjax();
    ajax.open("GET", "Handlers/srvEmployees.ashx?action=deleteEmp&ID=" + ID + stamp, true);

    ajax.onreadystatechange = function () {
        if (ajax.readyState == 4) {
            showLoadingGrid(false);
            var strResponse = ajax.responseText;

            if (strResponse.substr(0, 7) == 'MESSAGE') {
                var url = "Employees/srvMsgBoxEmployees.aspx?action=MessageEx&" + strResponse.substr(7, strResponse.length - 7);
                parent.ShowMsgBoxForm(url, 500, 300, '');
            }
            else {
                if (strResponse == 'OK') {
                    //ACTUALIZAR A OTRO EMPLEADO, etc.
                    var ctlPrefix = "ctl00_contentMainBody_roTrees1";
                    eval(ctlPrefix + "_roTrees.SelectFirstNode('1');");
                    refreshEmployeeTree(true);
                }
            }
        }
    }

    ajax.send(null)
}

function DeleteBiometricDataByEmployee(ID) {
    try {
        var stamp = '&StampParam=' + new Date().getMilliseconds();

        ajax = nuevoAjax();
        ajax.open("GET", "Handlers/srvEmployees.ashx?action=DeleteBiometricDataByEmployee&ID=" + ID + stamp, true);
        ajax.onreadystatechange = function () {
            if (ajax.readyState == 4) {
                var strResponse = ajax.responseText;

                if (strResponse.substr(0, 7) == 'MESSAGE') {
                    var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.substr(7, strResponse.length - 7));
                    parent.ShowMsgBoxForm(url, 500, 300, '');
                }
                else {
                    if (strResponse == 'OK') {
                        //TODO HA IDO BIEN
                        var message = "TitleKey=DeleteBiometricsEmployee.OKProcess.Text&" +
                            "DescriptionKey=DeleteBiometricsEmployee.OKProcess.Description&" +
                            "Option1TextKey=DeleteBiometricsEmployee.OKProcess.Option1Text&" +
                            "Option1DescriptionKey=DeleteBiometricsEmployee.OKProcess.Option1Description&" +
                            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                            "IconUrl=~/Base/Images/MessageFrame/dialog-information.png"
                        var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                    }
                }
            }
        }
        ajax.send(null)
    }
    catch (e) {
        showError("DeleteBiometricDataByEmployee: ", e);
    }
}

function ShowHideSupervisorsChilds(idFeature) {
    var tb = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_rowFeatureChilds' + idFeature);
    if (tb != null) {
        if (tb.style.display == '') {
            tb.style.display = 'none';
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeatureOpenImg' + idFeature).src = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeatureOpenImg' + idFeature).src.replace('minus', 'plus');
        } else {
            tb.style.display = '';
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeatureOpenImg' + idFeature).src = document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeatureOpenImg' + idFeature).src.replace('plus', 'minus');
        }
    }
}

////////////////////////////////////////////////  EMPLOYEE_ASSIGNMENTS (SUITABLITY) GRID /////////////////////////////////////////////////////

function gridSuitability_FocusedRowChanged(s, e) {
    if (s.IsEditing()) {
        s.UpdateEdit();
    }
}

function gridSuitability_BeginCallback(s, e) {
    if (e.command == 'UPDATEEDIT') {
        hasChanges(true);
    }
    if (e.command == 'DELETEROW') {
        hasChanges(true);
    }
}

//Agregar nueva fila en el grid de incidencias
function AddNewSuitability(s, e) {
    var grid = ASPxClientGridView.Cast("GridSuitabilityClient");
    grid.AddNewRow();
}

//Eliminar una incidencia en el datatable del servidor
function DeleteSuitability(IdRow) {
    grid = ASPxClientGridView.Cast("GridSuitabilityClient");
    grid.DeleteRow(IdRow);
}

function saveAssignments(IDEmpleado) {
    showLoadingGrid(true);

    var oParameters = {};
    oParameters.aTab = actualTab;
    oParameters.ID = IDEmpleado;
    oParameters.oType = "E";
    oParameters.StampParam = new Date().getMilliseconds();
    oParameters.action = "SAVEEMPLOYEEASSIGNMENTS";
    //oParameters.resultClientAction = postvars;
    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    ASPxCallbackPanelContenidoClient.PerformCallback(strParameters);
}

function showAlertDetailPopUp(contentUrl) {
    try {
        showLoadingGrid(true);
        AlertDetailsPopup_Client.SetContentUrl('');
        AlertDetailsPopup_Client.Show();
        AlertDetailsPopup_Client.SetContentUrl(contentUrl);
    } catch (e) { showError('showAlertDetailPopUp', e); }
}

//============ SELECTOR DE EMPLEADOS ===============================
function btnOpenPopupSelectorEmployeesClient_Click() {
    PopupSelectorEmployeesClient.Show();
}

function btnPopupSelectorEmployeesAcceptClient_Click() {
    showLoadingGrid(true);
    CallbackSessionClient.PerformCallback("GETINFOSELECTED")

    PopupSelectorEmployeesClient.Hide();
}

function btnPopupSelectorEmployeesCancelClient_Click() {
    PopupSelectorEmployeesClient.Hide();
}

function btnDownloadClient_Click() {
    var dataGrid = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_GroupDocumentManagmentCompany_divValuesGrid").dxDataGrid("instance");
    var selectedKeys = dataGrid.getSelectedRowKeys();
    if (selectedKeys.toString() != "") {
        var url = 'DocumentVisualize.aspx?DeliveredDocument=' + selectedKeys.toString();
        var Title = 'Documentos';
        window.open(url);
    }
}

function LoadAntValues(s) {
    acumValues = [];

    if (s.cpValues != null) {
        for (var i = 0; i < s.cpValues.length; i++) {
            acumValues.push({
                Id: s.cpValues[i].Id,
                Title: s.cpValues[i].Title,
                EmployeeName: s.cpValues[i].EmployeeName,
                DocumentTemplateName: s.cpValues[i].DocumentTemplate.Name,
                DocumentTemplateScope: s.cpValues[i].DocumentTemplate.Scope,
                DocumentTemplateArea: s.cpValues[i].DocumentTemplate.Area,
                DeliveredDate: s.cpValues[i].DeliveredDate,
                Remarks: s.cpValues[i].Remarks,
                Status: s.cpValues[i].Status,
                BeginDate: s.cpValues[i].DocumentTemplate.BeginValidity,
                EndDate: s.cpValues[i].DocumentTemplate.EndValidity,
                Validity: s.cpValues[i].Validity,
                ReceivedDate: s.cpValues[i].DocumentTemplate.ReceivedDate
            });
        }
    }
    gridValues = $("#ctl00_contentMainBody_ASPxCallbackPanelContenido_GroupDocumentManagmentCompany_divValuesGrid").dxDataGrid({
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,
        headerFilter: { visible: true },
        allowColumnResizing: true,
        filterRow: { visible: true },
        dataSource: {
            store: {
                type: 'array',
                key: 'Id',
                data: acumValues
            }
        },
        selection: {
            mode: "none" // or "multiple" | "none"
        },
        editing: {
            mode: "row",
            allowUpdating: false,
            allowAdding: false,
            allowDeleting: false,
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
            { caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_Id"), dataField: "Id", allowEditing: false, allowDeleting: false, visible: false, },
            { caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_Name"), dataField: "Title", allowEditing: false, allowDeleting: false, },
            { caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_Employee"), dataField: "EmployeeName", allowEditing: false, allowDeleting: false, },
            { caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_Title"), dataField: "DocumentTemplateName", allowEditing: false, allowDeleting: false, },
            {
                caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_TemplateScope"), dataField: "DocumentTemplateScope",
                allowEditing: false, allowDeleting: false,
                calculateCellValue: function (rowData) {
                    switch (rowData.DocumentTemplateScope) {
                        case "EmployeeField":
                            return "Campo de la ficha";
                            break;
                        case "EmployeeContract":
                            return "Contrato";
                            break;
                        case "Company":
                            return "Empresa";
                            break;
                        case "LeaveOrPermission":
                            return "Baja o permiso";
                            break;
                        case "CauseNote":
                            return "Justificante";
                            break;
                        case "EmployeeAccessAuthorization":
                            return "Autorización de acceso";
                            break;
                        case "CompanyAccessAuthorization":
                            return "Autorizacón de empresa";
                            break;
                        case "Communique":
                            return "Comunicado";
                            break;
                        default:
                            return "Desconocido";
                            break;
                    }
                },
            },
            {
                caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_TemplateArea"), dataField: "DocumentTemplateArea",
                allowEditing: false, allowDeleting: false,
                calculateCellValue: function (rowData) {
                    switch (rowData.DocumentTemplateArea) {
                        case "Prevention":
                            return "Prevención";
                            break;
                        case "Labor":
                            return "Laboral";
                            break;
                        case "Legal":
                            return "Legal";
                            break;
                        case "Security":
                            return "Seguridad";
                            break;
                        case "Quality":
                            return "Calidad";
                            break;
                        default:
                            return "Desconocido";
                            break;
                    }
                },
            },
            { caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_Remarks"), dataField: "Remarks", allowEditing: false, allowDeleting: false, },
            { caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_DeliveredDate"), dataField: "DeliveredDate", allowEditing: false, allowDeleting: false, },
            {
                caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_Status"), dataField: "Status", allowEditing: false, allowDeleting: false,
                calculateCellValue: function (rowData) {
                    switch (rowData.Status) {
                        case "Pending":
                            return "Pendiente";
                            break;
                        case "Validated":
                            return "Validado";
                            break;
                        case "Expired":
                            return "Caducado";
                            break;
                        case "Rejected":
                            return "Rechazado";
                            break;
                        case "Invalidated":
                            return "Invalidado";
                            break;
                        default:
                            return "Desconocido";
                            break;
                    }
                },
            },
            { caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_StartValidity"), dataField: "BeginDate", allowEditing: false, allowDeleting: false, },
            { caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_EndValidity"), dataField: "EndDate", allowEditing: false, allowDeleting: false, },
            {
                caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_Validity"), dataField: "Validity", allowEditing: false, allowDeleting: false,
                calculateCellValue: function (rowData) {
                    switch (rowData.Validity) {
                        case "CheckPending":
                            return "Pendiente aprobación";
                            break;
                        case "CurrentlyValid":
                            return "Validado";
                            break;
                        case "ValidOnFuture":
                            return "Válido próximamente";
                            break;
                        case "NotEnoughAuthorityLevel":
                            return "Sin nivel de aprobación";
                            break;
                        case "Invalid":
                            return "Inválido";
                            break;
                        default:
                            return "Desconocido";
                            break;
                    }
                },
            },
            { caption: window.parent.Globalize.formatMessage("groupDocumentsTableHeader_ReceivedDate"), dataField: "ReceivedDate", allowEditing: false, allowDeleting: false, },

        ]
    }).dxDataGrid("instance");
}

function chkForgottenRightValueChanged() {
    hasChanges(true);
}