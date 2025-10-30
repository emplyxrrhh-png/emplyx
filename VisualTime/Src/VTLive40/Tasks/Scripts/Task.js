var actualTask; // Id de Task actual iniciada
var actualProgressTask; // Progress del Id de Task actual
var actualTaskName;
var actualTab = 0;
var oTask; // Clase TaskData
var tasksFieldsGrid = null;
var tasksAlertsGrid = null;
var tasksAssignmentsGrid = null;
var grdStats;

var isShowingTaskEmployees = false;

function showLoadingGrid(loading) {
    parent.showLoader(loading);
}

/* ***************************************************************************************************/
// CargaNodoTarea: Obtener campos de la fila seleccionada en el Grid y generar un Callback para obtener objeto Task
/* ***************************************************************************************************/
function CargaNodoTarea(arrFieldsGrid) {
    try {
        showLoadingGrid(true);

        if (arrFieldsGrid == null) {
            actualTask = -1;
            actualProgressTask = -1;
            $("#deleteTaskCell").hide();
        } else {
            if (isNaN(arrFieldsGrid[0])) {
                actualTask = -1;
                actualProgressTask = -1;
                $("#deleteTaskCell").hide();
            } else {
                $("#deleteTaskCell").show();
                actualTask = arrFieldsGrid[0];
                actualProgressTask = arrFieldsGrid[1];
            }
        }

        hasChanges(false);

        var oParameters = {};
        oParameters.idTask = actualTask;
        oParameters.timeWorked = actualProgressTask;

        var cmbView = cmbViewClient;
        var cmbGroupBy = cmbGroupByClient;

        oParameters.idView = cmbView.GetSelectedIndex();
        oParameters.idGroup = cmbGroupBy.GetSelectedIndex();

        var dateinf = txtGroupByDateInfClient.date;
        var datesup = txtGroupByDateSupClient.date;

        if (dateinf != "" && datesup == "") {
            datesup = dateinf;
        }
        if (datesup != "" && dateinf == "") {
            dateinf = datesup;
        }

        oParameters.beginDate = dateinf;
        oParameters.endDate = datesup;

        oParameters.action = "LOADTASK";

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
        ASPxCallbackpanelDetailsTaskClient.PerformCallback(strParameters);

        showLoadingGrid(false);
    } catch (e) {
        showLoadingGrid(false);
        showError("cargaNodoTarea", e);
    }
}

function updateGridTaskActions(arrFieldsGrid) {
    if (arrFieldsGrid == null) {
        actualTask = -1;
        actualProgressTask = -1;
        $("#deleteTaskCell").hide();
    } else {
        if (isNaN(arrFieldsGrid[0]) || arrFieldsGrid[0] === null) {
            actualTask = -1;
            actualProgressTask = -1;
            $("#deleteTaskCell").hide();
        } else {
            actualTask = arrFieldsGrid[0];
            actualProgressTask = arrFieldsGrid[1];
            actualTaskName = arrFieldsGrid[2];
            $("#deleteTaskCell").show();
        }
    }
}

function updateProjectName() {
    var strNameDesc = " :: ";

    if (txtProjectClient.GetValue() != null) {
        strNameDesc = txtProjectClient.GetValue() + strNameDesc;
    }
    if (txtNameClient.GetValue() != null) {
        strNameDesc = strNameDesc + txtNameClient.GetValue();
    }

    $("#readOnlyNameAssignments").text(strNameDesc);
}
async function ASPxCallbackpanelDetailsTaskClient_EndCallBack(s, e) {
    //Una vez finalizada la carga de la tarea se muestra en pantalla

    var oParameters = {};
    var strParameters;

    if (s.cpID != null) {
        actualTask = s.cpID;
    }

    checkResult(s);

    switch (s.cpAction) {
        case "LOADTASK":
            await getroTreeState('objContainerTreeV3_treeEmpDetailTaskGrid').then(roState => roState.reset());

            oParameters.idTask = actualTask;
            oParameters.timeWorked = actualProgressTask;

            var cmbView = cmbViewClient;
            var cmbGroupBy = cmbGroupByClient;

            oParameters.idView = cmbView.GetSelectedIndex();
            oParameters.idGroup = cmbGroupBy.GetSelectedIndex();

            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optClosingAllways"
            );
            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optClosingByDate"
            );
            linkOPCItems(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optClosingAllways,ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optClosingByDate"
            );

            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivAllways"
            );
            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByDate"
            );
            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByEndTask"
            );
            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByIniTask"
            );
            linkOPCItems(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivAllways,ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByDate,ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByEndTask,ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByIniTask"
            );

            //panel03
            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optColabOnlyOneEmp"
            );
            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optColabAllEmp"
            );
            linkOPCItems(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optColabOnlyOneEmp,ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optColabAllEmp"
            );

            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optColabOnlyOneEmp_optTypeCollabAny"
            );
            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optColabOnlyOneEmp_optTypeCollabFirst"
            );
            linkOPCItems(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optColabOnlyOneEmp_optTypeCollabAny,ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optColabOnlyOneEmp_optTypeCollabFirst"
            );

            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optAutEmpAll"
            );
            venableOPC(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optAutEmpSelect"
            );
            linkOPCItems(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optAutEmpAll,ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optAutEmpSelect"
            );

            var dateinf = txtGroupByDateInfClient.date;
            var datesup = txtGroupByDateSupClient.date;

            updateProjectName();

            if (dateinf != "" && datesup == "") {
                datesup = dateinf;
            }
            if (datesup != "" && dateinf == "") {
                dateinf = datesup;
            }

            oParameters.beginDate = dateinf;
            oParameters.endDate = datesup;

            strParameters = JSON.stringify(oParameters);
            strParameters = encodeURIComponent(strParameters);

            var divGridTasks = document.getElementById(
                "ctl00_contentMainBody_divGridTasks"
            );
            var divMenuTask = document.getElementById("taskDetails");
            divGridTasks.style.display = "none";
            divMenuTask.style.display = "";

            //Cargar Grids
            if (s.cpGridsJSON != null && s.cpGridsJSON.length != 0) {
                var obj2;
                eval("obj2 = [" + s.cpGridsJSON + "]");

                //Grid de estadisticas
                loadStatistics(obj2);
            }

            var Progress = 0;
            if (s.cpProgress != null && s.cpProgress.length != 0) {
                Progress = parseFloat(s.cpProgress.replace(",", "."));
            }

            var Exceeded = 0;
            if (s.cpExceeded != null && s.cpExceeded.length != 0) {
                Exceeded = s.cpExceeded;
            }

            var Employees = 0;

            if (s.cpSelectedElements != null && s.cpSelectedElements.length != 0) {
                Employees = s.cpSelectedElements;
            }

            $(
                "#ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optAutEmpSelect_lblEmpSelect"
            ).text(Employees + " " + $("#hdnSeleccionar").val().split(",")[1]);

            //Barra de Progreso
            var divProgreso = document.getElementById("divProgreso");
            var anchoTotal = parseFloat(divProgreso.style.width);
            var oWidth = (anchoTotal * Progress) / 100;

            var InitialTime = txtInitialTimeClient;
            InitialTime.enabled = true;
            //InitialTime.removeAttr("disabled");

            if (Progress > 0) {
                //InitialTime.attr("disabled", true);
                InitialTime.enabled = false;
            }

            if (oWidth < 0) oWidth = 0;
            if (oWidth > anchoTotal) oWidth = anchoTotal;
            $("#divProgresoIn").width(oWidth);

            if (Progress == 0) {
                $("#divProgresoIn").html("").hide();
                $("#divProgresoIn2")
                    .html(Math.round(Progress) + "%")
                    .css("text-align", "center")
                    .width("100%")
                    .show();
            } else {
                $("#divProgresoIn").show();
                if (Progress > 10) {
                    var $spanProg = $(
                        '<span id="spanProgresoIn" style="position:relative; top:12px;"></span>'
                    ).html(Math.round(Progress) + "%");
                    $("#divProgresoIn").empty().append($spanProg);
                    $("#divProgresoIn2").html("").hide();
                } else {
                    $("#divProgresoIn").empty();
                    $("#divProgresoIn2")
                        .html(Math.round(Progress) + "%")
                        .width("auto")
                        .show();
                }
            }

            if (Exceeded == "1") {
                $("#divProgresoIn").removeClass("contenedor");
                $("#divProgresoIn").addClass("contenedorRed");
            } else {
                $("#divProgresoIn").removeClass("contenedorRed");
                $("#divProgresoIn").addClass("contenedor");
            }

            ASPxCallbackpanelDetailsTaskClient.SetVisible(true);

            if (actualTask == -1) {
                changeTabs(1);
                hasChanges(true);
                txtNameClient.SetFocus();
            } else {
                hasChanges(false);
                changeTabs(actualTab);
            }

            //LLAMADA CALLBACK PARA OBTENER DETALLES DE LA TAREA
            //pnlTaskDetails.PerformCallback(strParameters);

            //LLAMADA CALLBACK PARA LLENAR GRAFICO
            pnlTaskGraf.PerformCallback(strParameters);

            //LLAMADA CALLBACK PARA LLENAR EL GRID DE CAMPOS DE TAREA
            pnlTaskFieldsTask.PerformCallback(strParameters);

            //LLAMADA CALLBACK PARA LLENAR EL GRID DE ALERTAS
            pnlTaskAlertsTask.PerformCallback(strParameters);

            //LLAMADA CALLBACK PARA LLENAR EL GRID DE ALERTAS
            pnlTaskAssignmentsTask.PerformCallback(strParameters);

            break;

        case "SAVETASK":
            if (s.cpResult == "OK") {
            } else {
                hasChanges(true);
                changeTabs(actualTab);
                showLoadingGrid(false);
            }
            break;

        default:
            hasChanges(false);
            showLoadingGrid(false);
    }
}

function checkResult(oResult) {
    if (oResult.cpResult == "NOK") {
        if (oResult.cpAction == "SAVETASK") {
            hasChanges(true);
        }

        var url =
            "Tasks/srvMsgBoxTask.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
            "DescriptionText=" +
            oResult.cpMessage +
            "&" +
            "Option1TextKey=SaveName.Error.Option1Text&" +
            "Option1DescriptionKey=SaveName.Error.Option1Description&" +
            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
            "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
        parent.ShowMsgBoxForm(url, 400, 300, "");
    }
}

/* ***************************************************************************************************/
// pnlTaskDetails_EndCallBack: Retorno de la llamada Callback. Carga en pantalla el objeto Task recibido
/* ***************************************************************************************************/
function pnlTaskDetails_EndCallBack() {
    var fields = document.getElementById(
        "ctl00_contentMainBody_ASPxCallbackPanel2_hdnFieldsASP"
    );
    var fieldsStatistics = document.getElementById(
        "ctl00_contentMainBody_ASPxCallbackPanel2_hdnFieldsStatistics"
    );

    if (fields == null) return;

    if (oTask == null) oTask = new DataTask();

    if (fields != null) {
        var obj1;
        eval("obj1 = [" + fields.value + "]");

        var obj2;
        eval("obj2 = [" + fieldsStatistics.value + "]");

        oTask.loadTask(obj1, obj2);
        oTask.TaskId = actualTask;

        fields.value = "";
        fieldsStatistics.value = "";

        //Una vez finalizada la carga de la tarea se muestra en pantalla
        var divGridTasks = document.getElementById(
            "ctl00_contentMainBody_divGridTasks"
        );
        var divMenuTask = document.getElementById("taskDetails");
        divGridTasks.style.display = "none";
        divMenuTask.style.display = "";
        ASPxCallbackpanelDetailsTaskClient.SetVisible(true);
    }
}

/* ***************************************************************************************************/
// UpdateStatisticsGroupBy: Obtener parametros de estadisticas y generar un Callback para obtener JSON de estadisticas
/* ***************************************************************************************************/
function UpdateStatisticsGroupBy(idGroup) {
    var cmbView = cmbViewClient;

    var idView = cmbView.GetSelectedIndex();

    RefreshCombos(idView, idGroup);

    UpdateStatistics(idView, idGroup);
}

/* ***************************************************************************************************/
// UpdateStatisticsView: Obtener parametros de estadisticas y generar un Callback para obtener JSON de estadisticas
/* ***************************************************************************************************/
function UpdateStatisticsView(idView) {
    var cmbGroupBy = cmbGroupByClient;

    var idGroup = cmbGroupBy.GetSelectedIndex();

    RefreshCombos(idView, idGroup);

    UpdateStatistics(idView, idGroup);
}

function RefreshCombos(idView, idGroup) {
    if (idView == 0) {
        $("#tdlabelGroupBy").show();
        $("#tdcmbGroupBy").show();

        if (idGroup == 1) $("#tbGroupbyDates").show();
        else $("#tbGroupbyDates").hide();
    } else {
        $("#tdlabelGroupBy").hide();
        $("#tdcmbGroupBy").hide();
        $("#tbGroupbyDates").hide();
    }
}

function UpdateStatistics(idView, idGroup) {
    var oParameters = {};
    oParameters.idTask = actualTask;
    oParameters.idView = idView;
    oParameters.idGroup = idGroup;

    var dateinf = txtGroupByDateInfClient.date;
    var datesup = txtGroupByDateSupClient.date;

    if (dateinf != "" && datesup == "") {
        datesup = dateinf;
    }
    if (datesup != "" && dateinf == "") {
        dateinf = datesup;
    }

    oParameters.beginDate = dateinf;
    oParameters.endDate = datesup;

    var strParameters = JSON.stringify(oParameters);
    strParameters = encodeURIComponent(strParameters);

    //LLAMADA CALLBACK PARA LLENAR GRID DE ESTADISTICAS
    pnlTaskStatistics.PerformCallback(strParameters);

    //LLAMADA CALLBACK PARA LLENAR GRAFICO
    pnlTaskGraf.PerformCallback(strParameters);
}

/* ***********************************************************************************************/
// pnlTaskStatistics_EndCallBack: Retorno de la llamada Callback. Carga en el Grid el objeto JSON
/* ***********************************************************************************************/
function pnlTaskStatistics_EndCallBack() {
    try {
        showLoadingGrid(true);
        var fieldsStatistics = document.getElementById(
            "ctl00_contentMainBody_ASPxCallbackPanel3_hdnFieldsStatisticsSmall"
        );

        var obj2;
        eval("obj2 = [" + fieldsStatistics.value + "]");

        //Grid de estadisticas
        loadStatistics(obj2);
        showLoadingGrid(false);
    } catch (e) {
        showLoadingGrid(false);
        showError("pnlTaskStatistics_EndCallBack", e);
    }
}

function loadStatistics(arrFieldsStatistics) {
    if (arrFieldsStatistics == null) return;
    try {
        var headerGrid = [
            { fieldname: "id", description: "", size: "5%" },
            { fieldname: "concept", description: "", size: "80%" },
            { fieldname: "total", description: "", size: "15%" },
        ];

        var caption = document.getElementById("hdnCaptionGridStatsCol1").value;

        var cmbView = cmbViewClient;
        var cmbGroupBy = cmbGroupByClient;

        var cmbView_Value = cmbView.GetSelectedIndex();
        var cmbGroupBy_Value = cmbGroupBy.GetSelectedIndex();

        RefreshCombos(cmbView_Value, cmbGroupBy_Value);

        var edtRow = false;

        if (cmbView_Value == "1") {
            caption = caption.split(",")[3];
        } else {
            if (cmbGroupBy_Value == "0") {
                caption = caption.split(",")[0];
                edtRow = true;
            } else {
                if (cmbGroupBy_Value == "1") {
                    caption = caption.split(",")[1];
                } else {
                    caption = caption.split(",")[2];
                }
            }
        }

        headerGrid[0].description = "Id";
        headerGrid[1].description = caption;
        headerGrid[2].description = document.getElementById(
            "hdnCaptionGridStatsCol2"
        ).value;

        var delRow = false;

        grdStats = new jsGrid(
            "grdEstadisticas",
            headerGrid,
            arrFieldsStatistics,
            edtRow,
            delRow,
            false,
            "FilterList"
        );

        //estilo de las filas creadas
        var tbGrid = $("#tblGridFilterList tr:nth-child(1)");
        if (tbGrid != null) {
            tbGrid[0].cells[0].style.textAlign = "center";
            tbGrid[0].cells[1].style.textAlign = "center";
            tbGrid[0].cells[2].style.textAlign = "center";
            var $Filas = $("[name='htRowsFilterList']");
            $Filas.each(function (indexFila) {
                this.childNodes[0].style.textAlign = "center";

                this.childNodes[2].style.textAlign = "right";
                this.childNodes[2].style.paddingRight = "10px";
                if (indexFila == $Filas.length - 1) {
                    this.childNodes[2].style.fontWeight = "bold";
                    //this.childNodes[3].style.fontWeight = "bold";
                    $(this.childNodes[0]).html("");
                }
            });
        }
    } catch (e) {
        showError("<%= Me.ClientID %>_createGrid", e);
    }
}

/* ***********************************************************************************************/
// pnlTaskStatistics_EndCallBack: Retorno de la llamada Callback. El grafico debe estar cargado
/* ***********************************************************************************************/
function pnlTaskGraf_EndCallBack() { }

/* ***********************************************************************************************/
// pnlTaskFieldsTask_EndCallBack: Retorno de la llamada Callback. El grafico debe estar cargado
/* ***********************************************************************************************/
function pnlTaskFieldsTask_EndCallBack() {
    try {
        showLoadingGrid(true);
        var fieldsGrid = document.getElementById(
            "ctl00_contentMainBody_ASPxCallbackPanel4_hdnTaskFieldsTask"
        );

        var obj2;
        eval("obj2 = " + fieldsGrid.value + "");

        //Grid de campos de tarea
        loadTasksFieldsGrid(obj2.taskFields);
        showLoadingGrid(false);
    } catch (e) {
        showLoadingGrid(false);
        showError("pnlTaskStatistics_EndCallBack", e);
    }
}

function pnlTaskAssignmentsTask_EndCallBack() {
    try {
        showLoadingGrid(true);
        var assignmentsGrid = document.getElementById(
            "ctl00_contentMainBody_ASPxCallbackPanel6_hdnTaskAssignmentsTask"
        );

        var obj2;
        eval("obj2 = " + assignmentsGrid.value + "");

        //Grid de alertas de tareas
        loadTasksAssignmentsGrid(obj2.taskAssignments);
        showLoadingGrid(false);
    } catch (e) {
        showLoadingGrid(false);
        showError("pnlTaskAssignmentsTask_EndCallBack", e);
    }
}

function pnlTaskAlertsTask_EndCallBack() {
    try {
        showLoadingGrid(true);
        var alertsGrid = document.getElementById(
            "ctl00_contentMainBody_ASPxCallbackPanel5_hdnTaskAlertsTask"
        );

        var obj2;
        eval("obj2 = " + alertsGrid.value + "");

        //Grid de alertas de tareas
        loadTasksAlertsGrid(obj2.taskAlerts);
        showLoadingGrid(false);
    } catch (e) {
        showLoadingGrid(false);
        showError("pnlTaskStatistics_EndCallBack", e);
    }
}

function saveChanges() {
    try {
        grabarTask(actualTask);

        //} //end if
    } catch (e) {
        showError("saveChanges", e);
    }
}

function grabarTask(IDTask) {
    try {
        showLoadingGrid(true);

        var arrTaskFields = null;
        if (tasksFieldsGrid != null) {
            arrTaskFields = tasksFieldsGrid.toJSONStructureAdvanced();
        }

        var arrTaskAlerts = null;
        if (tasksAlertsGrid != null) {
            arrTaskAlerts = tasksAlertsGrid.toJSONStructureAdvanced();
        }

        var arrTaskAssigments = null;
        if (tasksAssignmentsGrid != null) {
            arrTaskAssigments = tasksAssignmentsGrid.toJSONStructureAdvanced();
        }

        var oParameters = {};
        oParameters.aTab = actualTab;
        oParameters.idTask = IDTask;
        oParameters.StampParam = new Date().getMilliseconds();
        oParameters.action = "SAVETASK";
        oParameters.timeWorked = actualProgressTask;

        oParameters.gridFields = arrTaskFields;
        oParameters.gridAlerts = arrTaskAlerts;
        oParameters.gridAssigments = arrTaskAssigments;

        var strParameters = JSON.stringify(oParameters);
        strParameters = encodeURIComponent(strParameters);

        //LLAMADA CALLBACK PARA GUARDAR LOS DATOS DE LA TAREA
        ASPxCallbackpanelDetailsTaskClient.PerformCallback(strParameters);
    } catch (e) {
        showError("grabarReportScheduler", e);
    }
}

//Mostra el ToolTip a la barra d'eines
function showTbTip(tip) {
    document.getElementById(tip).style.display = "";
}

//Amaga el ToolTip a la barra d'eines
function hideTbTip(tip) {
    document.getElementById(tip).style.display = "none";
}

function hasChanges(bolChanges, markRecalc) {
    var divTop = document.getElementById("divMsgTop");

    var tagHasChanges = document.getElementById("msgHasChanges");

    var AddTaskAssigment = document.getElementById("tblAddTask");

    var msgChanges = "<changes>";
    if (tagHasChanges != null) {
        msgChanges = tagHasChanges.value;
    }

    setStyleMessage("divMsg2");
    setMessage(msgChanges);

    if (bolChanges) {
        //espera oTasks.validateTask();
        divTop.style.display = "";
        AddTaskAssigment.style.display = "none";
    } else {
        divTop.style.display = "none";
        AddTaskAssigment.style.display = "";
    }
}

function setMessage(msg) {
    try {
        var msgTop = document.getElementById("msgTop");
        msgTop.textContent = msg;
    } catch (e) {
        showError("setMessage", e);
    }
}

function setStyleMessage(classMsg) {
    try {
        //divContainers styles
        var divTop = document.getElementById("divMsgTop");
        divTop.className = classMsg;
    } catch (e) {
        showError("setStyleMessage", e);
    }
}

//Cambia els Tabs i els divs
function changeTabs(numTab) {
    actualTab = numTab;
    //BOTONES
    arrButtons = new Array(
        "TABBUTTON_00",
        "TABBUTTON_01",
        "TABBUTTON_02",
        "TABBUTTON_03",
        "TABBUTTON_04",
        "TABBUTTON_05",
        "TABBUTTON_06"
    );
    for (n = 0; n < arrButtons.length; n++) {
        if (n == 6) {
            var tab = document.getElementById("ctl00_contentMainBody_TABBUTTON_06");
            if (tab != null) {
                if (n == numTab) {
                    tab.className = "bTab-active";
                } else {
                    tab.className = "bTab";
                }
            }
        } else {
            var tab = document.getElementById(arrButtons[n]);
            if (tab != null) {
                if (n == numTab) {
                    tab.className = "bTab-active";
                } else {
                    tab.className = "bTab";
                }
            }
        }
    }

    //PANELES
    panel00.SetVisible(false);
    panel01.SetVisible(false);
    panel02.SetVisible(false);
    panel03.SetVisible(false);
    panel04.SetVisible(false);
    panel05.SetVisible(false);
    panel06.SetVisible(false);
    switch (numTab) {
        case 0:
            panel00.SetVisible(true);
            break;
        case 1:
            panel01.SetVisible(true);
            break;
        case 2:
            panel02.SetVisible(true);
            break;
        case 3:
            panel03.SetVisible(true);
            break;
        case 4:
            panel04.SetVisible(true);
            break;
        case 5:
            panel05.SetVisible(true);
            break;
        case 6:
            panel06.SetVisible(true);
            break;
    }
}

//Oculta los detalles de una tarea y muestra el grid
function BackToGrid() {
    var divGridTasks = document.getElementById(
        "ctl00_contentMainBody_divGridTasks"
    );
    var divMenuTask = document.getElementById("taskDetails");

    divGridTasks.style.display = "";
    divMenuTask.style.display = "none";

    ASPxCallbackpanelDetailsTaskClient.SetVisible(false);

    $("#readOnlyNameAssignments").text($("#hdnCaptionGrid").val());

    document.getElementById("ctl00_contentMainBody_IDLoadTask").value = "0";

    refreshGrid();
}

function showDatePicker(bol) {
    try {
        var divDP = document.getElementById("dtPicker");
        if (divDP == null) {
            return;
        }
        if (bol == null) {
            if (divDP.style.display == "") {
                divDP.style.display = "none";
            } else {
                divDP.style.display = "";
            }
        } else {
            if (bol) {
                divDP.style.display = "";
            } else {
                divDP.style.display = "none";
            }
        }
    } catch (e) {
        showError("showDatePicker", e);
    }
}

function ChangeColor(color) {
    try {
        var divCol = document.getElementById("colorShift");
        if (divCol == null) {
            return;
        }
        divCol.style.backgroundColor = color;
    } catch (e) {
        showError("Changecolor", e);
    }
}

function refreshGrid() {
    try {
        $("#deleteTaskCell").hide();
        GridTareas.Refresh();
    } catch (e) {
        showError("refreshGrid", e);
    }
    modifyHeaders();
}

function CreateNewTask() {
    try {
        CargaNodoTarea(null);
        if (tasksFieldsGrid != null) tasksFieldsGrid.destroyGrid();
        if (tasksAlertsGrid != null) tasksAlertsGrid.destroyGrid();
        //hasChanges(true);
        //changeTabs(1);
        //txtNameClient.SetFocus();
    } catch (e) {
        showError("CreateNewTask", e);
    }
}

function ShowCreateTaskWizard() {
    //var url = 'Tasks/Wizards/CreateTaskWizard.aspx';
    //var Title = '';
    //parent.ShowExternalForm2(url, 530, 450, Title, '', false, false, false);
    CreateNewTask();
}

/* ***************************************************************************************************/
// ShowRemoveTask: Obtener fila y campos de la fila del grid seleccionada
/* ***************************************************************************************************/
function ShowRemoveTask() {
    try {
        var NameTask =
            $("#readOnlyNameAssignments").text().indexOf("::") > -1
                ? txtNameClient.GetValue()
                : actualTaskName;
        //obtener tarea seleccionada
        if (actualTask > -1) {
            var url = "Tasks/srvMsgBoxTask.aspx?action=Message";
            url = url + "&TitleKey=deleteTask.Title";
            url = url + "&DescriptionKey=deleteTask.Description";
            url = url + "&Option1TextKey=deleteTask.Option1Text";
            url = url + "&Option1DescriptionKey=deleteTask.Option1Description";
            url =
                url +
                "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteTask(" +
                actualTask +
                "); return false;";
            url = url + "&Option2TextKey=deleteTask.Option2Text";
            url = url + "&Option2DescriptionKey=deleteTask.Option2Description";
            url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
            url = url + "&NameTask=" + encodeURIComponent(NameTask);
            parent.ShowMsgBoxForm(url, 400, 300, "");
            //var nIndex = parseInt(GridTareas.focusedRowIndex);
            //if (nIndex > -1)
            //GridTareas.GetRowValues(nIndex, 'ID;Name', ShowRemoveTaskAfter);
        }
    } catch (e) {
        showError("ShowRemoveTask", e);
    }
}

/* ***************************************************************************************************/
// ShowRemoveTaskAfter: Missatge confirmació abans eliminació de la tarea
/* ***************************************************************************************************/
function ShowRemoveTaskAfter(oTaskFields) {
    try {
        if (isRowSelected(oTaskFields[0]) == true) {
            var url = "Tasks/srvMsgBoxTask.aspx?action=Message";
            url = url + "&TitleKey=deleteTask.Title";
            url = url + "&DescriptionKey=deleteTask.Description";
            url = url + "&Option1TextKey=deleteTask.Option1Text";
            url = url + "&Option1DescriptionKey=deleteTask.Option1Description";
            url =
                url +
                "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].deleteTask(" +
                oTaskFields[0] +
                "); return false;";
            url = url + "&Option2TextKey=deleteTask.Option2Text";
            url = url + "&Option2DescriptionKey=deleteTask.Option2Description";
            url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
            url = url + "&NameTask=" + encodeURIComponent(oTaskFields[1]);
            parent.ShowMsgBoxForm(url, 400, 300, "");
        } else {
            return;
        }
    } catch (e) {
        showError("ShowRemoveTaskAfter", e);
    }
}

function isRowSelected(id) {
    try {
        if (id != null && parseInt(id) > 0) {
            return true;
        } else {
            var url =
                "Tasks/srvMsgBoxTask.aspx?action=Message&TitleKey=rowNotSelected.Error.Text&" +
                "Option1TextKey=rowNotSelected.Error.Option1Text&" +
                "Option1DescriptionKey=rowNotSelected.Error.Option1Description&" +
                "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                "IconUrl=~/Base/Images/MessageFrame/dialog-information.png";

            parent.ShowMsgBoxForm(url, 400, 300, "");
            return false;
        }
    } catch (e) {
        showError("isRowSelected", e);
    }
}

/* ***************************************************************************************************/
// deleteTask: Eliminar Tarea. Mensaje confirmado
/* ***************************************************************************************************/
function deleteTask(Id) {
    try {
        //COMPROBAR QUE VISTA HAY ACTIVA Y VOLVER AL GRID
        if ((document.getElementById("taskDetails").style.display = "")) {
            BackToGrid();
        }

        if (Id > 0) {
            deleteTaskEx(Id);
        }
    } catch (e) {
        showError("deleteTask", e);
    }
}

//*****************************************************************************************/
// deleteTask: Eliminació de la Tarea per ID
//********************************************************************************************/
function deleteTaskEx(oId) {
    try {
        AsyncCall(
            "POST",
            "srvTasks.aspx?action=deleteXTask&ID=" + oId,
            "json",
            "arrStatus",
            "checkStatus(arrStatus,true); if(arrStatus[0].error == 'false'){ refreshGrid();BackToGrid(); }"
        );
    } catch (e) {
        showError("deleteTaskEx", e);
    }
}

/* ***************************************************************************************************/
// copyTask: Copiar Tareas
/* ***************************************************************************************************/
function ShowCopyTask() {
    try {
        //obtener tarea seleccionada
        var nIndex = parseInt(GridTareas.focusedRowIndex);
        if (nIndex > -1)
            GridTareas.GetRowValues(nIndex, "ID;Name", ShowCopyTaskAfter);
    } catch (e) {
        showError("ShowCopyTask", e);
    }
}

function ShowCopyTaskAfter(oTaskFields) {
    try {
        if (oTaskFields[0] > -1) {
            var url = "Tasks/srvMsgBoxTask.aspx?action=Message";
            url = url + "&TitleKey=copyTask.Title";
            url = url + "&DescriptionKey=copyTask.Description";
            url = url + "&Option1TextKey=copyTask.Option1Text";
            url = url + "&Option1DescriptionKey=copyTask.Option1Description";
            url =
                url +
                "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].copyTask(" +
                oTaskFields[0] +
                "); return false;";
            url = url + "&Option2TextKey=copyTask.Option2Text";
            url = url + "&Option2DescriptionKey=copyTask.Option2Description";
            url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
            url = url + "&IconUrl=~/Base/Images/MessageFrame/task-due.png";
            url = url + "&NameTask=" + oTaskFields[1];

            parent.ShowMsgBoxForm(url, 400, 300, "");
        } else {
            return;
        }
    } catch (e) {
        showError("ShowCopyTaskAfter", e);
    }
}

/* ***************************************************************************************************/
// copyTask: Copia la Tarea
/* ***************************************************************************************************/
function copyTask(Id) {
    try {
        if (Id > 0) {
            if (oTask == null) oTask = new DataTask();
            oTask.copyTask(Id);
        }
    } catch (e) {
        showError("copyTask", e);
    }
}

//==========================================================================
//Muestra ventana modal con el selector y botones de aceptar/cancelar
//==========================================================================
function ShowGroupSelector() {
    $find("RoPopupFrame1Behavior").show();
    document.getElementById("ctl00_contentMainBody_RoPopupFrame1").style.display =
        "";
}

//==========================================================================
//Oculta ventana modal con el selector y botones de aceptar/cancelar
//==========================================================================
async function HideGroupSelector() {
    try {
        $find("RoPopupFrame1Behavior").hide();
        document.getElementById(
            "ctl00_contentMainBody_RoPopupFrame1"
        ).style.display = "none";

        //comprobar si se ha seleccionado algo
        await getroTreeState('objContainerTreeV3_treeEmpDetailTaskGrid').then(roState => roState.reload());
        var oTreeState = await getroTreeState('objContainerTreeV3_treeEmpDetailTaskGrid'); 
        var nodes = oTreeState.getSelected("1");

        hasChanges(true);

        if (nodes == "") {
            $(
                "#ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optAutEmpSelect_lblEmpSelect"
            ).text($("#hdnSeleccionar").val().split(",")[0]); //Seleccionar...
        } else {
            //Obtener cantidad total de empleados asignados a la tarea
            showLoadingGrid(true);
            AsyncCall(
                "POST",
                "srvTasks.aspx?action=getEmployeesSelected&NodesSelected=" + nodes,
                "json",
                "arrStatus",
                "AfterSelectEmployees(arrStatus);"
            );
        }
    } catch (e) {
        showLoadingGrid(false);
        showError("HideGroupSelector", e);
    }
}

function AfterSelectEmployees(arrStatus) {
    try {
        showLoadingGrid(false);
        checkStatus(arrStatus);
        if (arrStatus[0].error == "false") {
            arrStatus[0].msg.substring(3);
            $(
                "#ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel03_optAutEmpSelect_lblEmpSelect"
            ).text(
                arrStatus[0].msg.substring(3) +
                " " +
                $("#hdnSeleccionar").val().split(",")[1]
            );
        }
    } catch (e) {
        showLoadingGrid(false);
        showError("AfterSaveTask", e);
    }
}

function checkStatus(oStatus, noHasChanges) {
    try {
        //Carreguem el array global per mantenir els valors
        arrStatus = oStatus;
        objError = arrStatus[0];

        //Si es un error, mostrem el missatge
        if (objError.error == "true") {
            if (objError.typemsg == "1") {
                //Missatge estil pop-up
                var url =
                    "Tasks/srvMsgBoxTask.aspx?action=Message&TitleKey=SaveName.Error.Text&" +
                    "DescriptionText=" +
                    objError.msg +
                    "&" +
                    "Option1TextKey=SaveName.Error.Option1Text&" +
                    "Option1DescriptionKey=SaveName.Error.Option1Description&" +
                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";
                parent.ShowMsgBoxForm(url, 400, 300, "");
            } else {
                //Missatge estil inline
            }
            if (noHasChanges == null) {
                hasChanges(true);
            }
        }
    } catch (e) {
        showError("checkStatus", e);
    }
} //end checkStatus function

function TaskChange(IDTask, NameTask) {
    if ($("#hdnControl").val() == "1") {
        txtIniTaskClient.SetValue("");
        txtEndTaskClient.SetValue(NameTask);
        //document.getElementById("ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByIniTask_txtIniTask").value = "";
        //document.getElementById("ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByEndTask_txtEndTask").value = NameTask;
        document.getElementById(
            "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_hdnActivationTask"
        ).value = IDTask;
        document.getElementById("hdnControl").value = "0";
    } else {
        if ($("#hdnControl").val() == "2") {
            //document.getElementById("ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByIniTask_txtIniTask").value = NameTask;
            //document.getElementById("ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_optActivByEndTask_txtEndTask").value = "";
            txtIniTaskClient.SetValue(NameTask);
            txtEndTaskClient.SetValue("");
            document.getElementById(
                "ctl00_contentMainBody_ASPxCallbackpanelDetailsTask_panel02_hdnActivationTask"
            ).value = IDTask;
            document.getElementById("hdnControl").value = "0";
        }
    }
}

function undoChanges() {
    try {
        if (actualTask == -1) {
            BackToGrid();
        } else {
            var oParameters = [];
            oParameters[0] = actualTask;
            oParameters[1] = actualProgressTask;
            CargaNodoTarea(oParameters);
        }
        hasChanges(false);
    } catch (e) {
        showError("undoChanges", e);
    }
}

//===================================================================
//===================================================================
//===================================================================

function retrieveError(objError) {
    try {
        setStyleMessage("divMsg-Error");

        var tagHasErrors = document.getElementById("msgHasErrors");
        var msgErrors = "<errors>";
        if (tagHasErrors != null) {
            msgErrors = tagHasErrors.value;
        }

        setMessage(msgErrors); //'Se han encontrado errores en la validación de campos.');

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
    } catch (e) {
        showError("retrieveError", e);
    }
}

function showErrorPopup(
    Title,
    typeIcon,
    Description,
    Opt1Text,
    Opt1Desc,
    strScript1,
    Opt2Text,
    Opt2Desc,
    strScript2,
    Opt3Text,
    Opt3Desc,
    strScript3
) {
    try {
        var url = "Assignments/srvMsgBoxAssignment.aspx?action=Message";
        url = url + "&TitleKey=" + Title;
        url = url + "&DescriptionKey=" + Description;
        url = url + "&Option1TextKey=" + Opt1Text;
        url = url + "&Option1DescriptionKey=" + Opt1Desc;
        url =
            url +
            "&Option1OnClickScript=HideMsgBoxForm();" +
            strScript1 +
            "; return false;";
        if (Opt2Text != null) {
            url = url + "&Option2TextKey=" + Opt2Text;
            url = url + "&Option2DescriptionKey=" + Opt2Desc;
            url =
                url +
                "&Option2OnClickScript=HideMsgBoxForm();" +
                strScript2 +
                "; return false;";
        }
        if (Opt3Text != null) {
            url = url + "&Option3TextKey=" + Opt3Text;
            url = url + "&Option3DescriptionKey=" + Opt3Desc;
            url =
                url +
                "&Option3OnClickScript=HideMsgBoxForm();" +
                strScript3 +
                "; return false;";
        }
        if (typeIcon.toUpperCase() == "TRASH") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";
        } else if (typeIcon.toUpperCase() == "ERROR") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/alert32.png";
        } else if (typeIcon.toUpperCase() == "INFO") {
            url = url + "&IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
        }

        parent.ShowMsgBoxForm(url, 400, 300, "");
    } catch (e) {
        showError("showErrorPopup", e);
    }
}

function ShowReports(
    Title,
    ReportsTitle,
    ReportsType,
    DefaultReportsVersion,
    RootURL
) {
    if (DefaultReportsVersion == 1) {
        if (ReportsTitle != "") Title = Title + " - " + ReportsTitle;
        parent.ShowExternalForm(
            "Reports/Reports.aspx",
            900,
            550,
            Title,
            "ReportsType",
            ReportsType
        );
    } else {
        parent.reenviaFrame(
            "/" + RootURL + "//Report",
            "",
            "Reports",
            "PortalGeneralManagement"
        );
    }
}

function SetTrackbarValue(s, e) {
    TrackBarPriority.SetValue(s.GetPosition());
}

function loadTasksFieldsGrid(arrFields) {
    if (arrFields == null) return;
    try {
        if (tasksFieldsGrid != null) {
            tasksFieldsGrid.destroyGrid();
        }
        var headerGrid = [
            /*{ 'fieldname': 'IDField', 'description': '', 'size': '5%' },*/
            { fieldname: "FieldName", description: "", size: "30%" },
            { fieldname: "Value", description: "", size: "35%" },
            { fieldname: "Action", description: "", size: "30%" },
        ];

        /*headerGrid[0].description = "Id";*/
        headerGrid[0].description = document.getElementById(
            "hdnCaptionTaskFieldsName"
        ).value;
        headerGrid[1].description = document.getElementById(
            "hdnCaptionTaskFieldsValue"
        ).value;
        headerGrid[2].description = document.getElementById(
            "hdnCaptionTaskFieldsAction"
        ).value;

        var edtRow = false;
        var delRow = false;

        if (
            document.getElementById("ctl00_contentMainBody_hdnModeEdit").value ==
            "true"
        ) {
            edtRow = true;
            delRow = true;
        }
        tasksFieldsGrid = new jsGrid(
            "grdUserFieldsTask",
            headerGrid,
            arrFields,
            edtRow,
            delRow,
            false,
            "TaskFieldsList"
        );
    } catch (e) {
        showError("<%= Me.ClientID %>_createGrid", e);
    }
}

function loadTasksAlertsGrid(arrFields) {
    if (arrFields == null) return;
    try {
        if (tasksAlertsGrid != null) {
            tasksAlertsGrid.destroyGrid();
        }
        var headerGrid = [
            /*{ 'fieldname': 'IDField', 'description': '', 'size': '5%' },*/
            { fieldname: "EmployeeName", description: "", size: "25%" },
            { fieldname: "DateTime", description: "", size: "10%" },
            { fieldname: "Comment", description: "", size: "45%" },
            { fieldname: "IsReaded", description: "", size: "10%" },
        ];

        /*headerGrid[0].description = "Id";*/
        headerGrid[0].description = document.getElementById(
            "hdnCaptionTaskAlertsName"
        ).value;
        headerGrid[1].description = document.getElementById(
            "hdnCaptionTaskAlertsDate"
        ).value;
        headerGrid[2].description = document.getElementById(
            "hdnCaptionTaskAlertsComment"
        ).value;
        headerGrid[3].description = document.getElementById(
            "hdnCaptionTaskAlertsReaded"
        ).value;

        var edtRow = false;
        var delRow = false;

        if (
            document.getElementById("ctl00_contentMainBody_hdnModeEdit").value ==
            "true"
        ) {
            edtRow = true;
            delRow = true;
        }
        tasksAlertsGrid = new jsGrid(
            "grdAlertsTask",
            headerGrid,
            arrFields,
            edtRow,
            delRow,
            false,
            "TaskAlertsList"
        );
    } catch (e) {
        showError("<%= Me.ClientID %>_createGrid", e);
    }
}

function loadTasksAssignmentsGrid(arrFields) {
    if (arrFields == null) return;
    try {
        if (tasksAssignmentsGrid != null) {
            tasksAssignmentsGrid.destroyGrid();
        }
        var headerGrid = [
            { fieldname: "IDAssignment", description: "", size: "-1" },
            { fieldname: "AssignmentName", description: "", size: "95%" },
        ];

        headerGrid[0].description = "Id";
        headerGrid[1].description = document.getElementById(
            "hdnCaptionTaskAssignmentName"
        ).value;

        var edtRow = false;

        var delRow = false;

        if (
            document.getElementById("ctl00_contentMainBody_hdnModeEdit").value ==
            "true"
        ) {
            delRow = true;
        }

        tasksAssignmentsGrid = new jsGrid(
            "grdAssignmentsTask",
            headerGrid,
            arrFields,
            edtRow,
            delRow,
            false,
            "TaskAssignmentsList"
        );
    } catch (e) {
        showError("<%= Me.ClientID %>_createGrid", e);
    }
}

function editGridTaskAlertsList(idRow) {
    var rows = null;

    var selectedId = -1;
    var selectedIdEmployee = "";
    var selectedEmployeeName = "";
    var selectedDatetime = "";
    var selectedComment = "";
    var selectedIsReaded = "";
    var selectedReaded = "";

    if (tasksAlertsGrid != null) rows = tasksAlertsGrid.getRows();

    var selectedNodes = "";
    if (rows != null) {
        for (var i = 0; i < rows.length; i++) {
            var curElem = rows[i];

            if (curElem.id == idRow) {
                selectedId = curElem.getAttribute("jsgridatt_id");
                selectedIdEmployee = curElem.getAttribute("jsgridatt_IDEmployee");
                selectedEmployeeName = curElem.getAttribute("jsgridatt_EmployeeName");
                selectedDatetime = curElem.getAttribute("jsgridatt_Datetime");
                selectedComment = curElem.getAttribute("jsgridatt_Comment");
                selectedIsReaded = curElem.getAttribute("jsgridatt_IsReaded");
                selectedReaded = curElem.getAttribute("jsgridatt_Readed");
            }

            if (i > 0) selectedNodes = selectedNodes + ",";
            selectedNodes = selectedNodes + curElem.getAttribute("jsgridatt_id");
        }
    }

    parent.ShowExternalForm2(
        "Tasks/TaskAlert.aspx?alertid=" +
        selectedId +
        "&idemployee=" +
        selectedIdEmployee +
        "&employeename=" +
        encodeURIComponent(selectedEmployeeName) +
        "&datetime=" +
        selectedDatetime +
        "&comment=" +
        encodeURIComponent(selectedComment) +
        "&isreaded=" +
        selectedIsReaded +
        "&readed=" +
        selectedReaded +
        "&SelectedNodes=" +
        selectedNodes,
        420,
        440,
        "",
        "",
        false,
        false,
        false
    );
}

function editGridFilterList(idRow) {
    var rows = null;
    var rowId = -1;
    var employeeId = -1;
    if (grdStats != null) rows = grdStats.getRows();

    var cmbGroupBy = cmbGroupByClient;
    var cmbView = cmbViewClient;

    var cmbView_Value = cmbView.GetSelectedIndex();
    var cmbGroupBy_Value = cmbGroupBy.GetSelectedIndex();

    if (cmbView_Value == "0" && cmbGroupBy_Value == "0" && rows != null) {
        for (var i = 0; i < rows.length - 1; i++) {
            var curElem = rows[i];

            if (curElem.id == idRow) {
                rowId = curElem.getAttribute("jsgridatt_id");
                employeeId = curElem.getAttribute("jsgridatt_employeeId");
                break;
            }
        }

        if (actualTask != -1 && rowId != -1) {
            // nueva pantalla de edicion de fichajes
            var url = "Scheduler/MovesNew.aspx?GroupID=-1&TaskFilterID=" + actualTask;
            url = url + "&EmployeeID=" + employeeId;
            var Title = "";
            parent.ShowExternalForm2(url, 1400, 620, Title, "", false, false, false);
        }
    }
}

function editGridTaskFieldsList(idRow) {
    var rows = null;

    var selectedId = -1;
    var selectedValue = "";
    var selectedAction = "";

    if (tasksFieldsGrid != null) rows = tasksFieldsGrid.getRows();

    var selectedNodes = "";
    if (rows != null) {
        for (var i = 0; i < rows.length; i++) {
            var curElem = rows[i];

            if (curElem.id == idRow) {
                selectedId = curElem.getAttribute("jsgridatt_idfield");
                selectedValue = curElem.getAttribute("jsgridatt_value");
                selectedAction = curElem.getAttribute("jsgridatt_action");
            }

            if (i > 0) selectedNodes = selectedNodes + ",";
            selectedNodes = selectedNodes + curElem.getAttribute("jsgridatt_idfield");
        }
    }

    parent.ShowExternalForm2(
        "Tasks/TaskField.aspx?TaskFieldID=" +
        selectedId +
        "&action=" +
        selectedAction +
        "&value=" +
        selectedValue +
        "&SelectedNodes=" +
        selectedNodes,
        420,
        440,
        "",
        "",
        false,
        false,
        false
    );
}

function editGridTaskAssigmentsList() {
    var Title = "";
    parent.ShowExternalForm2(
        "Tasks/TaskEditAssignments.aspx",
        420,
        470,
        "",
        "TaskID=" + actualTask,
        false,
        false,
        false
    );
}

function editGridTaskAssigmentsListEXXXXXXX(idRow) {
    var rows = null;

    var selectedId = -1;
    var selectedValue = "";
    var selectedAction = "";

    if (tasksAssignmentsGrid != null) rows = tasksAssignmentsGrid.getRows();

    var selectedNodes = "";
    if (rows != null) {
        for (var i = 0; i < rows.length; i++) {
            var curElem = rows[i];

            if (curElem.id == idRow) {
                selectedId = curElem.getAttribute("jsgridatt_idfield");
                selectedValue = curElem.getAttribute("jsgridatt_value");
                selectedAction = curElem.getAttribute("jsgridatt_action");
            }

            if (i > 0) selectedNodes = selectedNodes + ",";
            selectedNodes = selectedNodes + curElem.getAttribute("jsgridatt_idfield");
        }
    }

    parent.ShowExternalForm2(
        "Tasks/TaskField.aspx?TaskFieldID=" +
        selectedId +
        "&action=" +
        selectedAction +
        "&value=" +
        selectedValue +
        "&SelectedNodes=" +
        selectedNodes,
        420,
        440,
        "",
        "",
        false,
        false,
        false
    );
}

function deleteGridTaskAssignmentsList(idRow) {
    try {
        var url = "Tasks/srvMsgBoxTask.aspx?action=Message";
        url = url + "&TitleKey=deleteTaskAssigment.Title";
        url = url + "&DescriptionKey=deleteTaskAssigment.Description";
        url = url + "&Option1TextKey=deleteTaskAssigment.Option1Text";
        url = url + "&Option1DescriptionKey=deleteTaskAssigment.Option1Description";
        url =
            url +
            "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelTaskAssigments('" +
            idRow +
            "'); return false;";
        url = url + "&Option2TextKey=deleteTaskAssigment.Option2Text";
        url = url + "&Option2DescriptionKey=deleteTaskAssigment.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, "");
    } catch (e) {
        showError("deleteGridTaskAssignmentsList", e);
    }
}

function delSelTaskAssigments(idRow) {
    try {
        try {
            tasksAssignmentsGrid.deleteRow(idRow);
            hasChanges(true);
        } catch (e) { }
    } catch (e) {
        showError("delSelTaskAssigments", e);
    }
}

function deleteGridTaskAlertsList(idRow) {
    try {
        var url = "Tasks/srvMsgBoxTask.aspx?action=Message";
        url = url + "&TitleKey=deleteTaskAlert.Title";
        url = url + "&DescriptionKey=deleteTaskAlert.Description";
        url = url + "&Option1TextKey=deleteTaskAlert.Option1Text";
        url = url + "&Option1DescriptionKey=deleteTaskAlert.Option1Description";
        url =
            url +
            "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelTaskAlerts('" +
            idRow +
            "'); return false;";
        url = url + "&Option2TextKey=deleteTaskAlert.Option2Text";
        url = url + "&Option2DescriptionKey=deleteTaskAlert.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, "");
    } catch (e) {
        showError("deleteGridTaskAlertsList", e);
    }
}

function delSelTaskAlerts(idRow) {
    try {
        try {
            tasksAlertsGrid.deleteRow(idRow);
            hasChanges(true);
        } catch (e) { }
    } catch (e) {
        showError("delSelTaskAlerts", e);
    }
}

function deleteGridTaskFieldsList(idRow) {
    try {
        var url = "Tasks/srvMsgBoxTask.aspx?action=Message";
        url = url + "&TitleKey=deleteTaskField.Title";
        url = url + "&DescriptionKey=deleteTaskField.Description";
        url = url + "&Option1TextKey=deleteTaskField.Option1Text";
        url = url + "&Option1DescriptionKey=deleteTaskField.Option1Description";
        url =
            url +
            "&Option1OnClickScript=HideMsgBoxForm(); window.frames['ifPrincipal'].delSelTaskFields('" +
            idRow +
            "'); return false;";
        url = url + "&Option2TextKey=deleteTaskField.Option2Text";
        url = url + "&Option2DescriptionKey=deleteTaskField.Option2Description";
        url = url + "&Option2OnClickScript=HideMsgBoxForm(); return false;";
        url = url + "&IconUrl=~/Base/Images/MessageFrame/trash64.png";

        parent.ShowMsgBoxForm(url, 400, 300, "");
    } catch (e) {
        showError("deleteGridTaskFieldsList", e);
    }
}

function delSelTaskFields(idRow) {
    try {
        try {
            tasksFieldsGrid.deleteRow(idRow);
            hasChanges(true);
        } catch (e) { }
    } catch (e) {
        showError("delSelTaskFields", e);
    }
}

//Refresh de les pantalles (RETORN)
function RefreshScreen(DataType, oParms) {
    try {
        if (isShowingTaskEmployees == false) {
            if (DataType == "1") {
                var oTaskField;
                if (oParms != "") {
                    oTaskField = eval(oParms);
                    tasksFieldsGrid.addRows(oTaskField, "idfield", true);
                    hasChanges(true);
                }
            } else if (DataType == "2") {
                var oTaskAlert;
                if (oParms != "") {
                    oTaskAlert = eval(oParms);
                    tasksAlertsGrid.addRows(oTaskAlert, "id", true);
                    hasChanges(true);
                }
            } else if (DataType == "True") {
                showLoadingGrid(true);
                var oParameters = [];
                oParameters[0] = actualTask;
                oParameters[1] = -1;
                CargaNodoTarea(oParameters);
            } else if (DataType == "TaskAssignments") {
                showLoadingGrid(true);
                var oParameters = [];
                oParameters[0] = actualTask;
                oParameters[1] = -1;
                CargaNodoTarea(oParameters);
            } else {
                refreshGrid();
            }
        } else {
            PopupPopupTaskEmployees_Client.Show();
        }
    } catch (e) {
        showError("RefreshScreen", e);
    }
}

function refreshTotalDesviations(CentesimalFormatProductiV, DecimalFormat) {
    //if (oTask != null) oTask.calculateNewDesviation();
    calculateNewDesviation(CentesimalFormatProductiV, DecimalFormat);
}

function calculateNewDesviation(CentesimalFormatProductiV, DecimalFormat) {
    if (CentesimalFormatProductiV == 1) {
        var d1 = parseFloat(
            txtTimeChangedRequirementsClient.GetValue().replaceAll(",", ".")
        );
        var d2 = parseFloat(txtEmployeeTimeClient.GetValue().replaceAll(",", "."));
        var d3 = parseFloat(txtMaterialTimeClient.GetValue().replaceAll(",", "."));
        var d4 = parseFloat(
            txtForecastErrorTimeClient.GetValue().replaceAll(",", ".")
        );
        var d5 = parseFloat(txtTeamTimeClient.GetValue().replaceAll(",", "."));
        var d6 = parseFloat(txtOtherTimeClient.GetValue().replaceAll(",", "."));
        var d7 = parseFloat(
            txtNonProductiveTimeIncidenceClient.GetValue().replaceAll(",", ".")
        );

        var dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7;
        txtTotalDesviationHoursClient.SetValue(
            dTotal.toString().replace(".", DecimalFormat)
        );
    } else {
        var d1 = convertHoursToMinutes(txtTimeChangedRequirementsClient.GetValue());
        var d2 = convertHoursToMinutes(txtEmployeeTimeClient.GetValue());
        var d3 = convertHoursToMinutes(txtMaterialTimeClient.GetValue());
        var d4 = convertHoursToMinutes(txtForecastErrorTimeClient.GetValue());
        var d5 = convertHoursToMinutes(txtTeamTimeClient.GetValue());
        var d6 = convertHoursToMinutes(txtOtherTimeClient.GetValue());
        var d7 = convertHoursToMinutes(
            txtNonProductiveTimeIncidenceClient.GetValue()
        );

        var dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7;
        txtTotalDesviationHoursClient.SetValue(convertMinutesToHour(dTotal));
    }
}

function convertMinutesToHour(Minutes) {
    try {
        var isNegative = false;
        if (Minutes < 0) {
            isNegative = true;
            Minutes = Minutes * -1;
        }
        var Hours = Math.floor(parseInt(Minutes) / 60);
        var MinutesRest = "00";
        if ((parseInt(Minutes) ^ 60) > 0) {
            //Si no son horas justas, sacar los minutos
            MinutesRest = parseInt(Minutes) - Hours * 60;
        }

        //if (Hours.toString().length == 1) { Hours = "0" + Hours; }
        if (MinutesRest.toString().length == 1) {
            MinutesRest = "0" + MinutesRest;
        }

        if (isNegative) {
            return "-" + Hours + ":" + MinutesRest;
        } else {
            return Hours + ":" + MinutesRest;
        }
    } catch (e) {
        showError("convertMinutesToHour", e);
    }
}

function convertHoursToMinutes(Hours) {
    try {
        var isNegative = false;
        var HourStr = Hours.split(":")[0];
        if (HourStr.substr(0, 1) == "-") {
            isNegative = true;
            HourStr = HourStr.substr(1, HourStr.length - 1);
        }
        var Hour = parseFloat(HourStr);
        var MinutesStr = Hours.split(":")[1];
        var Minutes = parseFloat(MinutesStr);
        if (isNegative) {
            return (Hour * 60 + Minutes) * -1;
        } else {
            return Hour * 60 + Minutes;
        }
    } catch (e) {
        showError("convertHoursToMinutes", e);
        return 0;
    }
}

function LaunchTaskTemplateWizard() {
    var Title = "";
    top.ShowExternalForm2(
        "TaskTemplates/Wizards/LaunchTaskTemplateWizard.aspx?IDEmployeeSource=" + 4,
        850,
        515,
        Title,
        "",
        false,
        false,
        false
    );
}

function LaunchTaskCompleteWizard() {
    var Title = "";
    top.ShowExternalForm2(
        "Tasks/Wizards/CompleteTasksWizard.aspx",
        500,
        450,
        Title,
        "",
        false,
        false,
        false
    );
}

function selectedTaks(ID) {
    window.parent.showLoader(true);
    var contentUrl = "../Tasks/TaskEmployeeStatus.aspx?TaskId=" + ID;
    PopupPopupTaskEmployees_Client.SetContentUrl(contentUrl);
    PopupPopupTaskEmployees_Client.Show();
    isShowingTaskEmployees = true;
}

function GridTareasClientCustomButton_Click(s, e) {
    if (e.buttonID == "ShowTaskEmployeeStatus") {
        GridTareas.GetRowValues(e.visibleIndex, "ID", selectedTaks);
    }
}

function modifyHeaders() {
    document.getElementById(
        "ctl00_contentMainBody_GridTareas_col14"
    ).style.display = "none";
    document
        .getElementById("ctl00_contentMainBody_GridTareas_col13")
        .setAttribute("colspan", 2);
    var filtersRow = document.getElementById(
        "ctl00_contentMainBody_GridTareas_DXHeadersRow0"
    ).nextSibling.childNodes;
    filtersRow[14].setAttribute("colspan", 2);
    filtersRow[15].style.display = "none";

    //refreshGrid();

    if (
        document.getElementById("ctl00_contentMainBody_IDLoadTask").value != "0"
    ) {
        //CargaNodoTarea(document.getElementById("ctl00_contentMainBody_IDLoadTask").value);
        CargaNodoTarea([
            parseInt(
                document.getElementById("ctl00_contentMainBody_IDLoadTask").value,
                10
            ),
            0,
        ]);
    }
}