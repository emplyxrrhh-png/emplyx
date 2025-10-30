(function () {
    // LANGUAGE  ______________________________________________________________________
    const TODAS_CATEGORIAS = window.parent.Globalize.formatMessage("TODAS_CATEGORIAS");
    const SIN_EJECUCIONES = window.parent.Globalize.formatMessage("SIN_EJECUCIONES");
    const ELIMINA = window.parent.Globalize.formatMessage("ELIMINA");
    const CREAR = window.parent.Globalize.formatMessage("CREAR");
    const CANCELAR = window.parent.Globalize.formatMessage("CANCELAR");
    const EDITAR = window.parent.Globalize.formatMessage("EDITAR");
    const CONTINUAR = window.parent.Globalize.formatMessage("CONTINUAR");
    const ACEPTAR = window.parent.Globalize.formatMessage("ACEPTAR");
    const GUARDANDO_DATOS = window.parent.Globalize.formatMessage("GUARDANDO_DATOS");
    const TITLE_VALIDATION = "Atención";//window.parent.Globalize.formatMessage("TITLE_VALIDATION");
    const SIN_PLANIFICACIONES = "No hay planificaciones para el informe seleccionado.";// window.parent.Globalize.formatMessage("SIN_PLANIFICACIONES");
    const CREAR_PLANIFICACION = window.parent.Globalize.formatMessage("CREAR_PLANIFICACION");
    const CLONANDO_INFORME = window.parent.Globalize.formatMessage("CLONANDO_INFORME");
    const DESCARGANDO_EJECUCION = window.parent.Globalize.formatMessage("DESCARGANDO_EJECUCION");
    const GESTIONANDO_PERMISOS = window.parent.Globalize.formatMessage("GESTIONANDO_PERMISOS");
    const LANZANDO_INFORME = window.parent.Globalize.formatMessage("LANZANDO_INFORME");
    const CONFIRMAR_BORRAR_PLANIFICACION = window.parent.Globalize.formatMessage("CONFIRMAR_BORRAR_PLANIFICACION");
    const CONFIRMAR_BORRAR_INFORME = window.parent.Globalize.formatMessage("CONFIRMAR_BORRAR_INFORME");
    const LANGUAGES_SELECT_PLACEHOLDER = window.parent.Globalize.formatMessage("LANGUAGES_SELECT_PLACEHOLDER");
    const FORMAT_SELECT_PLACEHOLDER = window.parent.Globalize.formatMessage("FORMAT_SELECT_PLACEHOLDER");
    const CLONE_NAME_PLACEHOLDER = window.parent.Globalize.formatMessage("CLONE_NAME_PLACEHOLDER");
    const SI = window.parent.Globalize.formatMessage("SI");
    const NO = window.parent.Globalize.formatMessage("NO");
    const ELIMINAR_INFORME = window.parent.Globalize.formatMessage("ELIMINAR_INFORME");
    const NO_PERMISOS_PARA_BORRAR_INFORME = window.parent.Globalize.formatMessage("NO_PERMISOS_PARA_BORRAR_INFORME");
    const FALTA_COMPLETAR_ALGUNOS_CAMPOS = window.parent.Globalize.formatMessage("FALTA_COMPLETAR_ALGUNOS_CAMPOS");
    const ALGO_MAL_INTENTAR_GUARDAR_DATOS = window.parent.Globalize.formatMessage("ALGO_MAL_INTENTAR_GUARDAR_DATOS");
    const ALGO_MAL_INTENTAR_CLONAR_INFORME = window.parent.Globalize.formatMessage("ALGO_MAL_INTENTAR_CLONAR_INFORME");
    const NO_SE_PUEDE_RECUPERAR_EJECUCION = window.parent.Globalize.formatMessage("NO_SE_PUEDE_RECUPERAR_EJECUCION");
    const PRIMERO_SELECIONA_USUARIO_DE_LISTA = window.parent.Globalize.formatMessage("PRIMERO_SELECIONA_USUARIO_DE_LISTA");
    const USUARIO_YA_TIENE_PERMISOS = window.parent.Globalize.formatMessage("USUARIO_YA_TIENE_PERMISOS");
    const NO_SE_PUDO_LANZAR_INFORME = window.parent.Globalize.formatMessage("NO_SE_PUDO_LANZAR_INFORME");
    const INTENTELO_DE_NUEVO = window.parent.Globalize.formatMessage("INTENTELO_DE_NUEVO");
    const DESTINATION_PLACEHOLDER = window.parent.Globalize.formatMessage("DESTINATION_PLACEHOLDER");
    const REPORT_PARAMETER_SHIFT_DATE = window.parent.Globalize.formatMessage("REPORT_PARAMETER_SHIFT_DATE");
    const REPORT_PARAMETER_HOLIDAY_DATE = window.parent.Globalize.formatMessage("REPORT_PARAMETER_HOLIDAY_DATE");
    const REPORT_PARAMETER_CONCEPT_GROUP = window.parent.Globalize.formatMessage("REPORT_PARAMETER_CONCEPT_GROUP");
    const REPORT_PARAMETER_SELECTOR_EMPLEADOS = window.parent.Globalize.formatMessage("REPORT_PARAMETER_SELECTOR_EMPLEADOS");
    const REPORT_PARAMETER_DATES = window.parent.Globalize.formatMessage("REPORT_PARAMETER_DATES");
    const REPORT_PARAMETER_CAUSES = window.parent.Globalize.formatMessage("REPORT_PARAMETER_CAUSES");
    const REPORT_PARAMETER_INCIDENCES = window.parent.Globalize.formatMessage("REPORT_PARAMETER_INCIDENCES");
    const REPORT_PARAMETER_ZONES = window.parent.Globalize.formatMessage("REPORT_PARAMETER_ZONES");
    const REPORT_PARAMETER_TERMINALS = window.parent.Globalize.formatMessage("REPORT_PARAMETER_TERMINALS");
    const REPORT_PARAMETER_TASKS = 'Tasks';//window.parent.Globalize.formatMessage("REPORT_PARAMETER_TASKS");
    const REPORT_PARAMETER_USER_FIELDS = window.parent.Globalize.formatMessage("REPORT_PARAMETER_USER_FIELDS");
    const REPORT_PARAMETER_CONCEPT_DESCRIPTION = window.parent.Globalize.formatMessage("REPORT_PARAMETER_CONCEPT_DESCRIPTION"); //saldos
    const REPORT_PARAMETER_CONCEPTS = window.parent.Globalize.formatMessage("REPORT_PARAMETER_CONCEPTS");
    const REPORT_PARAMETER_VIEW = "Visualización";//window.parent.Globalize.formatMessage("REPORT_PARAMETER_VIEW");    
    const REPORT_PARAMETER_ACCESS_TYPE = "Tipo de acceso";
    const REPORT_PARAMETER_FILTER_PROFILE = window.parent.Globalize.formatMessage("REPORT_PARAMETER_FILTER_PROFILE");
    const REPORT_PARAMETER_CAUSES_RJL = window.parent.Globalize.formatMessage("REPORT_PARAMETER_CAUSES_RJL");
    const REPORT_PARAMETER_CONCEPTS_RJL = window.parent.Globalize.formatMessage("REPORT_PARAMETER_CONCEPTS_RJL");
    const REPORT_PARAMETER_YEAR_MONTH = window.parent.Globalize.formatMessage("REPORT_PARAMETER_YEAR_MONTH");
    const REPORT_PARAMETER_BETWEEN_YEAR_MONTH = window.parent.Globalize.formatMessage("REPORT_PARAMETER_YEAR_MONTH"); //Por ahora aprovechamos el de REPORT_PARAMETER_BETWEEN_YEAR_MONTH. Tendrán el mismo valor
    const REPORT_PARAMETER_FORMAT = window.parent.Globalize.formatMessage("FORMAT");
    const REPORT_PARAMETER_FILTERVALUES = window.parent.Globalize.formatMessage("REPORT_PARAMETER_FILTERVALUES");
    const SELECT_ALL = window.parent.Globalize.formatMessage("SELECT_ALL");
    const VIEWMORE = window.parent.Globalize.formatMessage("VIEWMORE");
    const SELECT = window.parent.Globalize.formatMessage("SELECT")
    const DISPLAY = window.parent.Globalize.formatMessage("DISPLAY")
    const TOTALS = window.parent.Globalize.formatMessage("TOTALS");
    const REPORT_PARAMETER_DETAIL = window.parent.Globalize.formatMessage("REPORT_PARAMETER_DETAIL");
    const REPORT_PARAMETER_SHOWCHART = window.parent.Globalize.formatMessage("REPORT_PARAMETER_CHART");
    const REPORT_PARAMETER_PROJECT_VSL = window.parent.Globalize.formatMessage("REPORT_PARAMETER_PROJECT_VSL");

    const ACCESSTYPE = window.parent.Globalize.formatMessage("ACCESSTYPE");
    const REPORT_PARAMETER_VALID_ACCESS = window.parent.Globalize.formatMessage("REPORT_PARAMETER_VALID_ACCESS");
    const REPORT_PARAMETER_INVALID_ACCESS = window.parent.Globalize.formatMessage("REPORT_PARAMETER_INVALID_ACCESS");

    const REPORT_PARAMETER_FORMAT_MSG = window.parent.Globalize.formatMessage("REPORT_PARAMETER_FORMAT_MSG");
    const REPORT_FILTER_VALUES = window.parent.Globalize.formatMessage("REPORT_FILTER_VALUES");
    const REPORT_VALUESRANGE_FILTER = window.parent.Globalize.formatMessage("REPORT_VALUESRANGE_FILTER");
    const AND = window.parent.Globalize.formatMessage("AND");
    const VALUE = window.parent.Globalize.formatMessage("VALUE");
    const FIELDCAMP = window.parent.Globalize.formatMessage("FIELDCAMP");

    const PERIOD = window.parent.Globalize.formatMessage("PERIOD");
    const NEXT_REPORT = window.parent.Globalize.formatMessage("NEXT_REPORT");
    const DESTINATION = window.parent.Globalize.formatMessage("DESTINATION");
    const LANGUAGE = window.parent.Globalize.formatMessage("LANGUAGE");
    const WHO = window.parent.Globalize.formatMessage("WHO");
    const DESCRIPTION = window.parent.Globalize.formatMessage("DESCRIPTION");
    const OVERRIDE_PLANIFICATION_PARAMS = window.parent.Globalize.formatMessage("OVERRIDE_PLANIFICATION_PARAMS");
    const KEEPPARAMS = window.parent.Globalize.formatMessage("KEEPPARAMS");
    const CHANGEPARAMS = window.parent.Globalize.formatMessage("CHANGEPARAMS");
    const REPORT_PARAMATERS = window.parent.Globalize.formatMessage("REPORT_PARAMATERS");
    const MSG_VALIDATION_DISPLAY = window.parent.Globalize.formatMessage("MSG_VALIDATION_DISPLAY");
    const MSG_VALIDATION_CHART = window.parent.Globalize.formatMessage("MSG_VALIDATION_DISPLAY");
    const MSG_VALIDATION_CONCEPTGROUPS = window.parent.Globalize.formatMessage("MSG_VALIDATION_CONCEPTGROUPS");
    const MSG_VALIDATION_CRITERIA_DISPLAY = window.parent.Globalize.formatMessage("MSG_VALIDATION_DISPLAY");
    const MSG_VALIDATION_EMPLOYEE = window.parent.Globalize.formatMessage("MSG_VALIDATION_EMPLOYEE");
    const MSG_VALIDATION_CONCEPTS = window.parent.Globalize.formatMessage("MSG_VALIDATION_CONCEPTS");
    const MSG_VALIDATION_MAXNUMBER = window.parent.Globalize.formatMessage("MSG_VALIDATION_MAXNUMBER");
    const MSG_VALIDATION_PROJECTS_MINMAX = window.parent.Globalize.formatMessage("MSG_VALIDATION_PROJECTS_MINMAX");

    const TYPE_Userfield = window.parent.Globalize.formatMessage("USERFIELDS");
    const TYPE_Shifts = window.parent.Globalize.formatMessage("SHIFTS");
    const TYPE_HolidayShifts = window.parent.Globalize.formatMessage("HOLIDAYSSHIFTS");
    const TYPE_ConceptGroup = window.parent.Globalize.formatMessage("CONCEPTGROUP");

    const SALDOS = window.parent.Globalize.formatMessage("SALDOS");
    const JUSTIFICACIONES = window.parent.Globalize.formatMessage("JUSTIFICACIONES");
    const INCIDENCIAS = window.parent.Globalize.formatMessage("INCIDENCIAS");
    const INCIDENCIASYJUSTIFICACIONES = window.parent.Globalize.formatMessage("INCIDENCIASYJUSTIFICACIONES");
    const SUPERA = window.parent.Globalize.formatMessage("SUPERA");
    const NOSUPERA = window.parent.Globalize.formatMessage("NOSUPERA");
    const ELVALOR = window.parent.Globalize.formatMessage("ELVALOR");
    const ELVALORDELCAMPO = window.parent.Globalize.formatMessage("ELVALORDELCAMPO");
    const DIARIAMENTE = "diariamente";
    const DIARIAMENTE_TEXT = window.parent.Globalize.formatMessage("DIARIAMENTE");
    const ENPERIODOSELECCIONADO = "en el periodo seleccionado";
    const ENPERIODOSELECCIONADO_TEXT = window.parent.Globalize.formatMessage("ENPERIODOSELECCIONADO");
    const SELECCIONASALDOS = window.parent.Globalize.formatMessage("SELECCIONASALDOS");
    const SELECCIONACONCEPTS = window.parent.Globalize.formatMessage("SELECCIONACONCEPTS");
    const SELECCIONAINCIDENCIAS = window.parent.Globalize.formatMessage("SELECCIONAINCIDENCIAS");
    const MAXINCIDENCIASJUSTIFICACIONES = window.parent.Globalize.formatMessage("MAXINCIDENCIASJUSTIFICACIONES");
    const MAXINCIDENCIAS = window.parent.Globalize.formatMessage("MAXINCIDENCIAS");
    const MAXJUSTIFICACIONES = window.parent.Globalize.formatMessage("MAXJUSTIFICACIONES");
    const MAXSALDOS = window.parent.Globalize.formatMessage("MAXSALDOS");

    const ENERO = window.parent.Globalize.formatMessage("MONTH_01");
    const FEBRERO = window.parent.Globalize.formatMessage("MONTH_02");
    const MARZO = window.parent.Globalize.formatMessage("MONTH_03");
    const ABRIL = window.parent.Globalize.formatMessage("MONTH_04");
    const MAYO = window.parent.Globalize.formatMessage("MONTH_05");
    const JUNIO = window.parent.Globalize.formatMessage("MONTH_06");
    const JULIO = window.parent.Globalize.formatMessage("MONTH_07");
    const AGOSTO = window.parent.Globalize.formatMessage("MONTH_08");
    const SEPTIEMBRE = window.parent.Globalize.formatMessage("MONTH_09");
    const OCTUBRE = window.parent.Globalize.formatMessage("MONTH_10");
    const NOVIEMBRE = window.parent.Globalize.formatMessage("MONTH_11");
    const DICIEMBRE = window.parent.Globalize.formatMessage("MONTH_12");
    const CURRENTMONTH = window.parent.Globalize.formatMessage("MONTH_CURRENT");
    const PREVMONTH = window.parent.Globalize.formatMessage("MONTH_PREV");
    const NEXTMONTH = window.parent.Globalize.formatMessage("MONTH_NEXT");

    var originalMaxValueText;
    var localFormat = 'DD-MM-YYYY';

    // VARIABLES ______________________________________________________________________
    const radioInputsSchedulerStep1 = [
        { title: window.parent.Globalize.formatMessage("DAILY"), range: "optDiary", value: 0 },
        { title: window.parent.Globalize.formatMessage("WEEKLY"), range: "optWeekly", value: 1 },
        { title: window.parent.Globalize.formatMessage("MONTHLY"), range: "optMonthly", value: 2 },
        { title: window.parent.Globalize.formatMessage("ONCE"), range: "optOneTime", value: 3 },
        { title: window.parent.Globalize.formatMessage("INTERVAL"), range: "optHours", value: 4 }
    ];
    const exportFormats = [
        { title: "PDF", value: 0 },
        { title: "Excel", value: 1 },
        { title: "CSV", value: 3 },
        //{ title: "Word", value: 4 },
        { title: "Text", value: 5 },
        //{ title: "Mail", value: 6 },
        //{ title: "Image", value: 7 },
        //{ title: "MHT", value: 8 },
        //{ title: "RTF", value: 9 },
    ];
    const levelsCategory = [
        { Description: "1", value: 1 },
        { Description: "2", value: 2 },
        { Description: "3", value: 3 },
        { Description: "4", value: 4 },
        { Description: "5", value: 5 },
        { Description: "6", value: 6 },
        { Description: "7", value: 7 },
        { Description: "8", value: 8 },
        { Description: "9", value: 9 },
        { Description: "10", value: 10 },
        { Description: "11", value: 11 },
    ];
    const widgetInstancesToDestroy = [];
    const CURRENTMONTHVALUE = 13;
    const PREVMONTHVALUE = 14;
    const NEXTMONTHVALUE = 15;

    const currentDate = new Date();
    const monthData = [
        { title: CURRENTMONTH, value: CURRENTMONTHVALUE },
        { title: PREVMONTH, value: PREVMONTHVALUE },
        { title: NEXTMONTH, value: NEXTMONTHVALUE },
        { title: ENERO, value: 1 },
        { title: FEBRERO, value: 2 },
        { title: MARZO, value: 3 },
        { title: ABRIL, value: 4 },
        { title: MAYO, value: 5 },
        { title: JUNIO, value: 6 },
        { title: JULIO, value: 7 },
        { title: AGOSTO, value: 8 },
        { title: SEPTIEMBRE, value: 9 },
        { title: OCTUBRE, value: 10 },
        { title: NOVIEMBRE, value: 11 },
        { title: DICIEMBRE, value: 12 }
    ]
    const yearData = [
        2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030
    ]

    let paramsSignatures;
    let indexParamsSignatures = 0;
    let isMenuCategoriesDisplayed = false;
    let $reportCards;
    let mainPanelTabs;
    let $dxcvCSD;
    let allCategories;
    let currentReportData;
    let editedReportData;
    let cardsPanelObserver;
    let cardsPanelScrollLayer;
    let originSearch;
    let customSearchBar;
    let reportCategoriesBtn;
    let menuCategories;
    let removeReportBtn;
    let mainPanelViews;
    let searchCategory;
    let categories;
    let reportsPageConfig;
    let scheduleOptions = {};
    let scheduleFrequency;
    let populateDestinationSelector;
    let removeExecBtn;
    let destinationParameters;
    let userPassportId = 1;
    let validated = true;
    let originSelectedConcepts = "";
    let initializedGroupConcepts = false;
    let initializedConcepts = false;
    let initializedCauses = false;
    let valorCriterio = null;
    let valorCriterioStr = null;
    let valorFichaCriterioData = [];
    // ON READY ______________________________________________________________________
    $(document).ready(async function () {
        //set global vars
        $reportCards = [...document.querySelectorAll(".reportCard")];
        mainPanelTabs = document.querySelectorAll(".switchMainViewTabs span");
        mainPanelViews = document.querySelectorAll("#mainPanelDisplay > div#reportConfigContainer, #mainPanelDisplay > div#reportExecContainer, #mainPanelDisplay > div#execSchedulerPage");
        $dxcvCSD = document.querySelector(".dxcvCSD");
        originSearch = document.querySelector("#CardView_DXSE_I");
        customSearchBar = document.querySelector("#customSearchBar");
        reportCategoriesBtn = document.querySelector("#reportCategoriesBtn");
        menuCategories = document.querySelector("#customSearchPanel .menuCategories");
        removeExecBtn = document.querySelector("#mainPanelDisplay .removeExecBtn");
        removeReportBtn = document.querySelector(".switchMainViewTabs #reportRemoveBtn");
        searchCategory = document.querySelector("#reportCategoriesBtn > nobr > span");

        //first executions
        checkCardsLastExecution();

        cardsPanelScrollLayer = populator.scrollTransparencyLayer($dxcvCSD);
        createCardsPanelObserver();
        $reportCards.length > 0 && cardsPanelObserver.observe($reportCards[$reportCards.length - 1]);

        window.reloadCardsPanel = reloadCardsPanel;

        resizePanel();

        /*---------------------------LISTENERS LISTENERS LISTENERS LISTENERS ------------------------*/
        $(".reportCard").off("click");
        $(reportCategoriesBtn).off("click");
        $(customSearchBar).off("keyup");
        $(".reportCard").on("click", handleCardClick);
        $(reportCategoriesBtn).on("click", handleCategoriesBtnClick);
        $(customSearchBar).on("keyup", handleSearchReports);
        //$(removeReportBtn).on("click", handleRemoveReportClick);
        /*---------------------------LISTENERS LISTENERS LISTENERS LISTENERS ------------------------*/

        reportsPageConfig = await getReportsPageConfig();
        userPassportId = await getUserPassportId();
    });

    // set local format
    switch (lang) {
        case "es-ES":
        case "ca-ES":
        case "pt-PT":
        case "gl-ES":
        case "it-IT":
        case "fr-FR":
        case "eu-ES":
            localFormat = 'dd/M/yyyy';
            break;
        case "en-US":
        default:
            localFormat = 'M/dd/yyyy';
            break;
    }

    // WINDOW FUNCTIONS _______________________________________________________________
    window.onresize = () => { resizePanel(); };

    window.collectParamValues = async (param1, param2, param3) => {
        let finalParam = "";
        const whatEdition = document
            .querySelector("#editView > form")
            .getAttribute("id");

        const form = document.querySelector("#fieldsEditor #" + whatEdition);
        const paramToEdit = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);
        let myArray = {};

        switch (whatEdition) {
            case "selectorEmployees":
                const usersParam = `${param1 || -1}@${param2 || -1}@${param3 || ""}`;
                finalParam = usersParam;
                break;

            case "selectorPeriodTime":
                const momentFormat = "YYYY-MM-DD HH:mm:ss";
                const checked = form.querySelector("input[type=radio]:checked").value;
                const initDateInput = $("#desdeDateTime").dxDateBox("instance");
                const finalDateInput = $("#hastaDateTime").dxDateBox("instance");
                const { initDate, finalDate } = editor.selectorDatetime(parseInt(checked, 10), initDateInput, finalDateInput);
                const periodParam =
                    checked !== "0"
                        ? `${checked},${initDate.hour(0).minute(0).second(0).format(momentFormat)},${finalDate.hour(23).minute(59).second(59).format(momentFormat)}`
                        : () => {
                            return `${checked},${window.parent.moment(initDate()).hour(0).minute(0).second(0).format(momentFormat)},${window.parent.moment(finalDate()).hour(23).minute(59).second(59).format(momentFormat)}`;
                        };
                finalParam = periodParam;
                break;

            case "selectorString":
                const stringParam = form.querySelector("#parameterString").value;
                finalParam = stringParam;
                break;

            case "selectorNumber":
                const numberParam = form.querySelector("#parameterNumber").value;
                finalParam = numberParam;
                break;

            case "selectorDate":
                var momentFormat2 = "YYYY-MM-DD";
                var checked2 = form.querySelector("input[type=radio]:checked").value;
                var dateParam = "";
                const dateInput = $("#parameterDate").dxDateBox("instance");
                var showHint = false;

                switch (checked2) {
                    case "0":
                        dateInput.option('disabled', false);
                        const dateVal = dateInput.option('value');
                        dateParam = dateVal.format(momentFormat2);
                        dateInput.option('value', dateParam);
                        showHint = editedReportData.Name == "Control de vacaciones por días"; //Nombre del informe en BD, siempre va a ser este valor en cualquier idioma
                        break;
                    case "2":
                        dateInput.option('disabled', true);
                        dateParam = moment().format(momentFormat2);
                        showHint = false;
                        break;
                    case "3":
                        dateInput.option('disabled', true);
                        dateParam = moment().add('days', -1).format(momentFormat2);
                        showHint = false;
                        break;
                }

                //Mostramos mensaje de ayuda en el informe Control de días de vacaciones, PBI 1230654.
                if (showHint) {
                    $("#hintDataSelection").css('opacity', '1');
                } else {
                    $("#hintDataSelection").css('opacity', '0');
                }

                const periodParam2 = `${checked2},${dateParam}`;
                finalParam = periodParam2;

                break;

            case "selectorDestination":
                editor.addDestination(param1);
                return;

            case "selectorUniversal":
                if (paramToEdit.Type == 'Robotics.Base.userFieldsSelector' || paramToEdit.Type == 'Robotics.Base.userFieldsSelectorRadioBtn') {
                    finalParam = "";

                    if (param1 === null) // inicialización pantalla
                    {
                        var actualSelection = (currentReportData.ParametersList).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name).Value;

                        if ((actualSelection !== undefined) && (actualSelection !== "")) // Existe una seleccion actual
                        {
                            finalParam = actualSelection;
                        }
                    }
                    // guardamos valores en variables de saldo, si estan declaradas y si finalParam tiene un array de valores seleccionados
                    if ((finalParam !== "") || (!(param1 === undefined || param1 === null))) {
                        if (!(param1 === undefined || param1 === null)) finalParam = param1;

                        let param = 0;
                        let i = 0;
                        for (paramArray in currentReportData.ParametersList) {
                            param = 0;
                            if ((currentReportData.ParametersList[paramArray].Type === "Robotics.Base.userFieldIdentifier") && (i < 11)) {
                                var userField = "userfield" + (i + 1).toString(); // userfield1,userfield2... userfield11
                                if (currentReportData.ParametersList[paramArray].Name === userField) {
                                    if (!(finalParam.split(",")[i] === undefined || finalParam.split(",")[i] === null)) {
                                        param = finalParam.split(",")[i];
                                    }

                                    currentReportData.ParametersList[paramArray].Value = param;
                                }
                                i++;
                            }
                        }

                        var universalInstance = $("#selectorUniversal").dxDataGrid("instance"); //.option("selectedItemKeys", finalParam.split(","));
                        universalInstance.selectRows(finalParam.split(","), true);
                    }
                }
                else {
                    finalParam = "";

                    if (param1 === null) // inicialización pantalla
                    {
                        var actualSelection = (currentReportData.ParametersList).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name).Value;

                        if ((actualSelection !== undefined) && (actualSelection !== "")) // Existe una seleccion actual
                        {
                            finalParam = actualSelection;
                        }

                        if ((finalParam !== "") || (!(param1 === undefined || param1 === null))) {
                            var universalInstance = $("#selectorUniversal").dxDataGrid("instance"); //.option("selectedItemKeys", finalParam.split(","));
                            universalInstance.selectRows(finalParam.split(","), true);
                        }
                    }
                }

                break;

            case "selectorConcepts":
                finalParam = "";

                if (param1 === null) // inicialización pantalla
                {
                    var actualSelection = (currentReportData.ParametersList).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name).Value;

                    if ((actualSelection !== undefined) && (actualSelection !== "")) // Existe una seleccion actual
                    {
                        finalParam = actualSelection;
                    }
                }
                // guardamos valores en variables de saldo, si estan declaradas y si finalParam tiene un array de valores seleccionados
                if ((finalParam !== "") || (!(param1 === undefined || param1 === null))) {
                    if (!(param1 === undefined || param1 === null)) finalParam = param1;
                    let param = 0;
                    let i = 0;
                    for (paramArray in currentReportData.ParametersList) {
                        param = 0;
                        if ((currentReportData.ParametersList[paramArray].Type === "Robotics.Base.conceptIdentifier")
                            && (i < 16)) {
                            var nameConcept = "concept" + (i + 1).toString(); // concept1,concept2... concept11, concept12
                            if (currentReportData.ParametersList[paramArray].Name === nameConcept) {
                                if (!(finalParam.split(",")[i] === undefined || finalParam.split(",")[i] === null)) {
                                    param = finalParam.split(",")[i];
                                }

                                currentReportData.ParametersList[paramArray].Value = param;
                            }
                            i++;
                        }
                    }
                    if (typeof finalParam == "object") finalParam = finalParam.Value;
                    document.getElementById("selecttionItemsConcepts").innerText = finalParam;

                    if (initializedConcepts && finalParam) {
                        $("#listConcepts").dxList("instance").option("selectedItemKeys", finalParam.split(","));
                    }
                    if (initializedGroupConcepts) {
                        document.getElementById("selecttionItemsGroups").innerText = $("#listGroupsConcepts").dxList("instance").option("selectedItemKeys").join(",");
                    }
                }

                break;

            case "selectorCauses":
                finalParam = "";

                if (param1 === null) // inicialización pantalla
                {
                    var actualSelection = (currentReportData.ParametersList).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name).Value;

                    if ((actualSelection !== undefined) && (actualSelection !== "")) // Existe una seleccion actual
                    {
                        finalParam = actualSelection;
                    }
                }
                // guardamos valores en variables de saldo, si estan declaradas y si finalParam tiene un array de valores seleccionados
                if ((finalParam !== "") || (!(param1 === undefined || param1 === null))) {
                    if (!(param1 === undefined || param1 === null)) finalParam = param1;
                    let param = 0;
                    let i = 0;
                    for (paramArray in currentReportData.ParametersList) {
                        param = 0;
                        if ((currentReportData.ParametersList[paramArray].Type === "Robotics.Base.causeIdentifier")
                            && (i < 11)) {
                            var nameCause = "cause" + (i + 1).toString(); // cause1,cause2... cause11
                            if (currentReportData.ParametersList[paramArray].Name === nameCause) {
                                if (!(finalParam.split(",")[i] === undefined || finalParam.split(",")[i] === null)) {
                                    param = finalParam.split(",")[i];
                                }

                                currentReportData.ParametersList[paramArray].Value = param;
                            }
                            i++;
                        }
                    }
                    document.getElementById("selecttionItemsCauses").innerText = finalParam;

                    if (initializedCauses) {
                        try {
                            $("#listCauses").dxList("instance").option("selectedItemKeys", finalParam.split(","));
                        }
                        catch { ; }
                    }
                }

                break;

            case "selectorFormat":

                myArray['format'] = $("#typeDocReport").dxSelectBox("instance").option("value");

                switch (param1) {
                    case "format":
                        myArray['format'] = param2;
                        if (planificationsTab.classList.contains("activeTab")) {
                            $("#typeDocReport").dxSelectBox("instance").option("value", param2);
                        }
                        break;
                }
                finalParam = myArray;
                break;

            case "selectorViewFormat":

                myArray['ckDetail'] = $("#ckAllDetails").dxCheckBox("instance").option("value")
                myArray['ckTotal'] = $("#ckTotals").dxCheckBox("instance").option("value");
                myArray['ckShowChart'] = $("#ckShowChart").dxCheckBox("instance").option("value");

                switch (param1) {
                    case "detail":
                        myArray['ckDetail'] = param2;
                        break;
                    case "total":
                        myArray['ckTotal'] = param2;
                        break;
                    case "chart":
                        myArray['ckShowChart'] = param2;
                        break;
                }
                finalParam = myArray;
                break;

            case "selectorAccessType":

                myArray['ckValids'] = $("#ckValids").dxCheckBox("instance").option("value");
                myArray['ckInvalids'] = $("#ckInvalids").dxCheckBox("instance").option("value");

                switch (param1) {
                    case "valids":
                        myArray['ckValids'] = param2;
                        break;
                    case "invalids":
                        myArray['ckInvalids'] = param2;
                        break;
                }
                finalParam = myArray;
                break;

            case "selectorProfileTypes":

                myArray['profileTypes'] = (param1 == "profileTypes") ? param2 : $("#profileTypes").dxRadioGroup("instance").option("value");
                myArray['criterio'] = (param1 == "criterio") ? param2 : $("#criterio").dxSelectBox("instance").option("value");
                myArray['tipoValorCriterio'] = (param1 == "tipoValorCriterio") ? param2 : $("#tipoValorCriterio").dxSelectBox("instance").option("value");
                myArray['valorCriterio'] = (param1 == "valorCriterio") ? param2 : valorCriterio;
                myArray['valorFichaCriterio'] = (param1 == "valorFichaCriterio") ? param2 : $("#valorFichaCriterio").dxSelectBox("instance").option("value");
                myArray['rangoCriterio'] = (param1 == "rangoCriterio") ? param2 : $("#rangoCriterio").dxSelectBox("instance").option("value");

                //actualizamos los valores del selector
                const profileTypes = ["selectConcepts", "selectCauses", "selectIncidences"];
                profileTypes.map(pt => myArray[pt] = ($("#" + pt).dxTagBox("instance")) ? $("#" + pt).dxTagBox("instance").option("value") : "");

                finalParam = myArray;

                //Para el resto de parametros ocultos, le damos el valor que tengan en esta pantalla
                let newValue;
                let i = 0;
                let x = 0;
                let y = 0;
                for (paramArray in currentReportData.ParametersList) {
                    newValue = null;

                    switch (currentReportData.ParametersList[paramArray].Name) {
                        case "PTTipoCriterio":
                            newValue = myArray['profileTypes']
                            break;
                        case "PTRangoCriterio":
                            newValue = myArray['rangoCriterio']; //"rango fechas"; //diariamente
                            break;
                        case "PTTipoValorCriterio":
                            newValue = myArray['tipoValorCriterio']; //selector para saber si se quiere comparar con un valor introducido manualmente, o de un campo de la ficha
                            break;
                        case "PTValorCriterio": //Valor que aparece para cuando el selector tipoCriterio es de tipo valor numérico, sin tirar de campo de la ficha
                            newValue = (myArray['valorCriterio'] !== undefined && myArray['valorCriterio'].length == 5) ? myArray['valorCriterio'].slice(0, 3) + ':' + myArray['valorCriterio'].slice(3) : myArray['valorCriterio'];
                            break;
                        case "PTValorFichaCriterio":
                            newValue = myArray['valorFichaCriterio']; //para el selector campo de la ficha, cuando tipoCriterio está seleccionado como valor del campo.
                            break;
                        case "PTCriterio":
                            newValue = myArray['criterio']; //"mayor"/"menor"
                            break;
                        case "PTConcepts":
                            if (myArray['selectConcepts'] !== undefined) {
                                if (typeof myArray['selectConcepts'][0] === 'object' && myArray['selectConcepts'][0].value !== undefined) {
                                    newValue = myArray['selectConcepts'].map((e) => e.value).join(",");
                                } else {
                                    newValue = myArray['selectConcepts'].join(",");
                                }
                            }
                            break;
                        case "PTCauses":
                            if (myArray['selectCauses'] !== undefined) {
                                if (typeof myArray['selectCauses'][0] === 'object' && myArray['selectCauses'][0].value !== undefined) {
                                    newValue = myArray['selectCauses'].map((e) => e.value).join(",");
                                } else {
                                    newValue = myArray['selectCauses'].join(",");
                                }
                            }
                            break;
                        case "PTIncidences":
                            if (myArray['selectIncidences'] !== undefined) {
                                if (typeof myArray['selectIncidences'][0] === 'object' && myArray['selectIncidences'][0].value !== undefined) {
                                    newValue = myArray['selectIncidences'].map((e) => e.value).join(",");
                                } else {
                                    newValue = myArray['selectIncidences'].join(",");
                                }
                            }
                            break;
                        default:
                            newValue = null;
                            break;
                    }

                    if ((currentReportData.ParametersList[paramArray].Type === "Robotics.Base.conceptIdentifier")
                        && (i < 11)) {
                        let nameConcept = "concept" + (i + 1).toString(); // concept1,concept2... concept11

                        let concept = null;
                        if (myArray['selectConcepts'] !== undefined) {
                            if (typeof myArray['selectConcepts'][0] === 'object' && myArray['selectConcepts'][0].value !== undefined) {
                                concept = myArray['selectConcepts'][i]?.value;
                            } else {
                                concept = myArray['selectConcepts'][i];
                            }
                        }

                        if (currentReportData.ParametersList[paramArray].Name === nameConcept) {
                            if (!(concept === undefined || concept === null)) {
                                newValue = concept;
                            } else {
                                newValue = '-1';
                            }
                        }
                        i++;
                    } else if ((currentReportData.ParametersList[paramArray].Type === "Robotics.Base.causeIdentifier")
                        && (x < 11)) {
                        let nameCause = "cause" + (x + 1).toString(); // cause1,cause2... cause11

                        let cause = null;
                        if (myArray['selectCauses'] !== undefined) {
                            if (typeof myArray['selectCauses'][0] === 'object' && myArray['selectCauses'][0].value !== undefined) {
                                cause = myArray['selectCauses'][x]?.value;
                            } else {
                                cause = myArray['selectCauses'][x];
                            }
                        }
                        if (currentReportData.ParametersList[paramArray].Name === nameCause) {
                            if (!(cause === undefined || cause === null)) {
                                newValue = cause;
                            } else {
                                newValue = '-1';
                            }
                        }
                        x++;
                    } else if ((currentReportData.ParametersList[paramArray].Type === "Robotics.Base.incidenceIdentifier")
                        && (y < 11)) {
                        let nameIncidence = "incidence" + (y + 1).toString(); // incidence1,incidence2... incidence11

                        let incidence = null;
                        if (myArray['selectIncidences'] !== undefined) {
                            if (typeof myArray['selectIncidences'][0] === 'object' && myArray['selectIncidences'][0].value !== undefined) {
                                incidence = myArray['selectIncidences'][y]?.value;
                            } else {
                                incidence = myArray['selectIncidences'][y];
                            }
                        }
                        if (currentReportData.ParametersList[paramArray].Name === nameIncidence) {
                            if (!(incidence === undefined || incidence === null)) {
                                newValue = incidence;
                            } else {
                                newValue = '-1';
                            }
                        }
                        y++;
                    }

                    if (newValue !== null) currentReportData.ParametersList[paramArray].Value = newValue;
                }
                break;

            case "selectorCausesRegistroJL":

                myArray['showCauses'] = (param1 == "showCauses") ? param2 : $("#showCauses").dxRadioGroup("instance").option("value");

                //actualizamos los valores del selector
                const showCauses = ["selectCauses"];
                /*if (myArray['showCauses'] == 0) {
                    showCauses = [];
                }*/
                showCauses.map(pt => myArray[pt] = ($("#" + pt).dxTagBox("instance")) ? $("#" + pt).dxTagBox("instance").option("value") : "");

                finalParam = myArray;

                //Para el resto de parametros ocultos, le damos el valor que tengan en esta pantalla
                let newVal;
                let indx = 0;
                for (paramArray in currentReportData.ParametersList) {
                    switch (currentReportData.ParametersList[paramArray].Name) {
                        case "PTShowCauses":
                            newVal = myArray['showCauses']
                            break;
                        case "PTCauses":
                            if (myArray['selectCauses'] !== undefined) {
                                if (Array.isArray(myArray['selectCauses'])) {
                                    if (myArray['selectCauses'].length > 0 && typeof myArray['selectCauses'][0] === 'object' && myArray['selectCauses'][0].value !== undefined) {
                                        newVal = myArray['selectCauses'].map((e) => e.value).join(",");
                                    } else {
                                        newVal = myArray['selectCauses'].join(",");
                                    }
                                } else {
                                    newVal = myArray['selectCauses'].toString();
                                }
                            }
                            break;
                        default:
                            newVal = null;
                            break;
                    }
                    if ((currentReportData.ParametersList[paramArray].Type === "Robotics.Base.causeIdentifier")
                        && (indx < 11)) {
                        let nameCause = "cause" + (indx + 1).toString(); // cause1,cause2... cause11
                        let cause = (myArray['selectCauses'] !== undefined) ? myArray['selectCauses'].map((e) => e.value)[indx] : null;
                        if (currentReportData.ParametersList[paramArray].Name === nameCause) {
                            if (!(cause === undefined || cause === null)) {
                                newVal = cause;
                            } else {
                                newVal = '-1';
                            }
                        }
                        indx++;
                    }

                    if (newVal !== null) currentReportData.ParametersList[paramArray].Value = newVal;
                }
                break;

            case "selectorConceptsRegistroJL":

                myArray['showConcepts'] = (param1 == "showConcepts") ? param2 : $("#showConcepts").dxRadioGroup("instance").option("value");

                //actualizamos los valores del selector
                const showConcepts = ["selectConcepts"];
                showConcepts.map(pt => myArray[pt] = ($("#" + pt).dxTagBox("instance")) ? $("#" + pt).dxTagBox("instance").option("value") : "");

                finalParam = myArray;

                //Para el resto de parametros ocultos, le damos el valor que tengan en esta pantalla
                let newValConcept;
                let indexConcept = 0;
                for (paramArray in currentReportData.ParametersList) {
                    switch (currentReportData.ParametersList[paramArray].Name) {
                        case "PTShowConcepts":
                            newValConcept = myArray['showConcepts']
                            break;
                        case "PTConcepts":
                            if (myArray['selectConcepts'] !== undefined) {
                                if (Array.isArray(myArray['selectConcepts']) && myArray['selectConcepts'].length > 0) {
                                    if (typeof myArray['selectConcepts'][0] === 'object' && myArray['selectConcepts'][0].value !== undefined) {
                                        // Si los elementos son objetos con una propiedad 'value'
                                        newValConcept = myArray['selectConcepts'].map(e => e.value).join(",");
                                    } else {
                                        // Si los elementos son valores simples
                                        newValConcept = myArray['selectConcepts'].join(",");
                                    }
                                } else {
                                    newValConcept = "";
                                }
                            }
                            break;
                        default:
                            newValConcept = null;
                            break;
                    }
                    if ((currentReportData.ParametersList[paramArray].Type === "Robotics.Base.causeIdentifier")
                        && (indexConcept < 11)) {
                        let nameConcept = "concept" + (indexConcept + 1).toString(); // concept1,concept2... concept11
                        let concept = (myArray['selectConcepts'] !== undefined) ? myArray['selectConcepts'].map((e) => e.value)[indexConcept] : null;
                        if (currentReportData.ParametersList[paramArray].Name === nameConcept) {
                            if (!(concept === undefined || concept === null)) {
                                newValConcept = concept;
                            } else {
                                newValConcept = '-1';
                            }
                        }
                        indexConcept++;
                    }

                    if (newValConcept !== null) currentReportData.ParametersList[paramArray].Value = newValConcept;
                }
                break;

            case "selectorYearAndMonth":
                //IMPORTANTE: Necesitamos crear 2 parametros extras en el informe con nombre: "Year" y "Month" para darles valor automáticamente con el valor seleccionado en éste selector

                //llegara como 13 o SIGUIENTE_MES
                myArray['month'] = (param1 == "month") ? param2 : $("#month").dxSelectBox("instance").option("value");
                myArray['year'] = (param1 == "year") ? param2 : $("#year").dxSelectBox("instance").option("value");

                //Para el resto de parametros ocultos, le damos el valor que tengan en esta pantalla
                for (paramArray in currentReportData.ParametersList) {
                    var paramName = currentReportData.ParametersList[paramArray].Name;
                    if (paramName == "month") {
                        //Añadimos logica de mostrar currentData si el valor seleccionado es dinámico
                        currentReportData.ParametersList[paramArray].Value = getDynamicMonth(myArray[paramName]);
                    } else if (paramName == "year") {
                        currentReportData.ParametersList[paramArray].Value = getDynamicYear(myArray[paramName], myArray['month']);
                    }
                }
                finalParam = myArray;
                break;

            case "selectorBetweenYearAndMonth":
                //IMPORTANTE: Necesitamos crear 4 parametros extras en el informe con nombre: "YearStart", "MonthStart", "YearEnd" y "MonthEnd" para darles valor automáticamente con el valor seleccionado en éste selector
                myArray['monthStart'] = (param1 == "monthStart") ? param2 : $("#monthStart").dxSelectBox("instance").option("value");
                myArray['yearStart'] = (param1 == "yearStart") ? param2 : $("#yearStart").dxSelectBox("instance").option("value");
                myArray['monthEnd'] = (param1 == "monthEnd") ? param2 : $("#monthEnd").dxSelectBox("instance").option("value");
                myArray['yearEnd'] = (param1 == "yearEnd") ? param2 : $("#yearEnd").dxSelectBox("instance").option("value");

                finalParam = myArray;

                //Para el resto de parametros ocultos, le damos el valor que tengan en esta pantalla
                for (paramArray in currentReportData.ParametersList) {
                    var paramName = currentReportData.ParametersList[paramArray].Name;
                    //Añadimos logica de mostrar currentData si el valor seleccionado es dinámico -> 'd'
                    if (paramName == "monthStart" || paramName == "monthEnd") {
                        currentReportData.ParametersList[paramArray].Value = getDynamicMonth(myArray[paramName]);
                    } else if (paramName == "yearStart") {
                        currentReportData.ParametersList[paramArray].Value = getDynamicYear(myArray[paramName], myArray['monthStart']);
                    } else if (paramName == "yearEnd") {
                        currentReportData.ParametersList[paramArray].Value = getDynamicYear(myArray[paramName], myArray['monthEnd']);
                    }
                }
                break;

            case "selectorFilterValues":
                var param1 = param1;
                var param2 = param2;
                var tmpValue = "";
                var tmpValueDec = "";

                if (param1 === null) {
                    myArray['optionFilterValues'] = document.getElementById("optionFilter").value;
                }
                else {
                    myArray['optionFilterValues'] = $("#titleFilterValues").dxCheckBox("instance").option('value');
                }

                myArray['typeFilterSince'] = $("#typeFilterMin").dxSelectBox("instance").option("value");
                myArray['typeFilterTo'] = $("#typeFilterMax").dxSelectBox("instance").option("value");

                if (myArray['typeFilterSince'] === "text") {
                    tmpValue = $("#valueTextMin").dxNumberBox("instance").option('value');
                    tmpValue = (tmpValue === undefined) ? "0" : tmpValue;
                    tmpValueDec = $("#valueTextMinDec").dxNumberBox("instance").option('value');
                    tmpValueDec = (tmpValueDec === undefined) ? "00" : tmpValueDec;
                    myArray['valueSince'] = tmpValue + ":" + tmpValueDec;
                }
                else {
                    myArray['valueSince'] = $("#valueFieldsMin").dxSelectBox("instance").option("value");
                }

                if (myArray['typeFilterTo'] === "text") {
                    tmpValue = $("#valueTextMax").dxNumberBox("instance").option('value');
                    tmpValue = (tmpValue === undefined) ? "0" : tmpValue;
                    tmpValueDec = $("#valueTextMaxDec").dxNumberBox("instance").option('value');
                    tmpValueDec = (tmpValueDec === undefined) ? "00" : tmpValueDec;
                    myArray['valueTo'] = tmpValue + ":" + tmpValueDec;
                }
                else {
                    myArray['valueTo'] = $("#valueFieldsMax").dxSelectBox("instance").option("value");
                }

                switch (param1) {
                    case "titleFilterValues":
                        myArray['optionFilterValues'] = param2;
                        break;
                    //-------------------------------------
                    case "typeFilterMin":
                        myArray['typeFilterSince'] = param2;
                        if (param2 === "text") {
                            myArray['valueSince'] = $("#valueTextMin").dxNumberBox("instance").option('value');
                        }
                        else if (param2 === "field") {
                            myArray['valueSince'] = $("#valueFieldsMin").dxSelectBox("instance").option("value");
                        }
                        break;

                    case "valueTextMin":
                        tmpValue = $("#valueTextMinDec").dxNumberBox("instance").option('value');
                        myArray['valueSince'] = param2 + ":" + tmpValue;
                        break;

                    case "valueTextMinDec":
                        tmpValue = $("#valueTextMin").dxNumberBox("instance").option('value');
                        myArray['valueSince'] = tmpValue + ":" + param2;
                        break;

                    case "valueFieldsMin": // user fields
                        myArray['valueSince'] = param2;
                        break;
                    //-------------------------------------
                    case "typeFilterMax":
                        myArray['typeFilterTo'] = param2;
                        if (param2 === "text") { myArray['valueTo'] = $("#valueTextMax").dxNumberBox("instance").option('value'); }
                        else if (param2 === "field") { myArray['valueTo'] = $("#valueFieldsMax").dxSelectBox("instance").option("value"); }
                        break;

                    case "valueTextMax":
                        tmpValueDec = $("#valueTextMaxDec").dxNumberBox("instance").option('value');
                        myArray['valueTo'] = param2 + ":" + tmpValueDec;
                        break;

                    case "valueTextMaxDec":
                        tmpValue = $("#valueTextMax").dxNumberBox("instance").option('value');
                        myArray['valueTo'] = tmpValue + ":" + param2;
                        break;

                    case "valueFieldsMax":
                        myArray['valueTo'] = param2;
                        break;
                }
                document.getElementById("optionFilter").value = myArray['optionFilterValues'];
                finalParam = myArray;
                break;
            case "selectorTasks":
                var hdnTasksSelection = document.getElementById('hdnTasksSelected');
                if (param1 != "") {
                    finalParam = (param2 != "") ? param1 + ',' + param2 : param1;
                } else {
                    finalParam = param2;
                }
                hdnTasksSelection.value = finalParam;
                break;
            case "selectorProjectsVSL":
                //IMPORTANTE: Necesitamos crear 2 parametros extras en el informe con nombre: "StartProject" y "EndProject" para darles valor automáticamente con el valor seleccionado en este selector

                myArray['startProject'] = (param1 == "startProject") ? param2 : $("#startProject").dxNumberBox("instance").option("value");
                myArray['endProject'] = (param1 == "endProject") ? param2 : $("#endProject").dxNumberBox("instance").option("value");

                //Para el resto de parametros ocultos, le damos el valor que tengan en esta pantalla
                for (paramArray in currentReportData.ParametersList) {
                    var paramName = currentReportData.ParametersList[paramArray].Name;
                    if (paramName == "startProject") {
                        currentReportData.ParametersList[paramArray].Value = $("#startProject").dxNumberBox("instance").option("value");
                    } else if (paramName == "endProject") {
                        currentReportData.ParametersList[paramArray].Value = $("#endProject").dxNumberBox("instance").option("value");
                    }
                }

                finalParam = myArray;
                break;

            default:
                //selectorUniversal, dxDataGrid->onSelectionChanged
                break;
        }

        paramToEdit.Value = finalParam;
    };

    window.preventDefaultOnSubmit = (event) => event.preventDefault();

    window.pushPreviowsParamsDestination = () => {
        const indexPlanificationEdited = editedReportData.PlannedExecutionsList.findIndex(
            (plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey")
        );
        frmAddDestinationV2_Show(destinationParameters);
    };

    window.GetSelectedTreeTask = (param1, param2) => {
        collectParamValues(param1, param2);
    }
    // FUNCTIONS ______________________________________________________________________

    const getDynamicMonth = (param) => {
        let month = 0;
        const currentMonth = currentDate.getMonth();
        switch (param) {
            case CURRENTMONTHVALUE:
            case CURRENTMONTH:
                month = (currentMonth + 1);
                break;
            case PREVMONTHVALUE:
            case PREVMONTH:
                month = (currentMonth == 0) ? 12 : currentMonth; //devolvemos diciembre (del año anterior) si estamos en enero -> getMonth() == 0 == Enero
                break;
            case NEXTMONTHVALUE:
            case NEXTMONTH:
                month = (currentMonth == 11) ? 1 : (currentMonth + 2); //devolvemos enero (del año siguiente) si estamos en diciembre -> getMonth() == 11 == Diciembre
                break;
            default:
                month = param;
                break;
        }
        return month;
    }

    const getDynamicYear = (param, month) => {
        let year = 0;
        const currentYear = currentDate.getFullYear();
        switch (month) {
            case CURRENTMONTH:
            case CURRENTMONTHVALUE:
                year = currentYear;
                break;
            case PREVMONTH:
            case PREVMONTHVALUE:
                year = (currentDate.getMonth() == 0) ? (currentYear - 1) : currentYear; //Si estamos en enero y pedimos mes anterior, restamos 1 al año
                break;
            case NEXTMONTH:
            case NEXTMONTHVALUE:
                year = (currentDate.getMonth() == 11) ? (currentYear + 1) : currentYear; //Si estamos en diciembre y pedimos siguiente mes, sumamos 1 al año
                break;
            default:
                year = param;
                break;
        }
        return year;
    }

    const handlePlanExecutionClick = async () => {
        const planificationsTab = document.querySelector("#planificationsTab");
        const mainPanelDisplay = document.querySelector("#mainPanelDisplay");
        const planner = document.createElement("div");

        if (document.querySelector("#plannerPage")) { document.querySelector("#plannerPage").remove(); }

        //si tenemos activo el botón planificaciones mantenemos oculto el report
        if (planificationsTab.classList.contains("activeTab")) {
            document.querySelector("#reportExecContainer").style = "display:none";
            document.querySelector("#reportConfigContainer").style = "display:none";
        }

        // [ PLANNER ___________________________________________________
        planner.setAttribute("id", "plannerPage");
        planner.innerHTML = await getViewTemplate("plannerPage");
        [
            ...mainPanelDisplay.querySelectorAll("#reportConfigContainer, #reportExecContainer"),
        ].map((div) => (div.style.display = "none"));

        //if there is a previous planner by error, remove it to regenerate
        const $previousPlanner = mainPanelDisplay.querySelector("#plannerPage");
        if ($previousPlanner) $previousPlanner.remove();

        mainPanelDisplay.insertBefore(planner, mainPanelDisplay.firstChild);

        const plannedListContainer = mainPanelDisplay.querySelector("#reportPlannedList");

        populator.planifications(plannedListContainer);		//print list

        planner.style.display = "block";
        //  PLANNER ] ___________________________________________________

        //update active tab
        const tabs = [...document.querySelectorAll(".switchMainViewTabs .viewTab")];
        tabs.map((tab) => tab.classList.remove("activeTab"));
        tabs.find((tab) => tab.getAttribute("id") === "planificationsTab").classList.add("activeTab");

        // botón icono si hay  planificaciones ---------------------
        var idPLanImgReport = "imagePlanReport" + currentReportData.Id;
        if (document.getElementById(idPLanImgReport) !== null) {
            while (document.getElementById(idPLanImgReport).childNodes[0] !== undefined) {
                document.getElementById(idPLanImgReport).removeChild(document.getElementById(idPLanImgReport).childNodes[0]);
            }

            if (currentReportData.PlannedExecutionsList.length > 0) {
                var newElement = document.createElement("img");
                newElement.setAttribute("src", BASE_URL + "Base/Images/StartMenuIcos/Events.png");
                newElement.setAttribute("readonly", "true");
                newElement.setAttribute("width", "16px");
                document.getElementById(idPLanImgReport).appendChild(newElement);
            }
        }
    };

    const exitPlanner = async (tabsContainer) => {
        const planner = document.querySelector("#mainPanelDisplay #plannerPage");
        const editReportsBtnsTemplate = await getViewTemplate("switchMainViewTabs");

        !!planner && planner.remove();
        tabsContainer.innerHTML = editReportsBtnsTemplate;
        await updateReportActions(currentReportData.Id);

        const currentCard = document.querySelector(".reportCard.reportCardClicked");
        currentCard.click();
    };

    async function handleRemoveReportClick() {
        if (!currentReportData.Permissions.Remove) {
            await DevExpress.ui.dialog.alert(NO_PERMISOS_PARA_BORRAR_INFORME, ELIMINAR_INFORME);
            return;
        }

        const confirms = await utils.dialogDx(CONFIRMAR_BORRAR_INFORME, ELIMINAR_INFORME);

        if (confirms) {
            const isremoved = await removeReport(currentReportData.Id);

            if (isremoved) {
                $reportCards
                    .find((card) => card.classList.contains("reportCardClicked"))
                    .remove();
                reloadCardsPanel();
            }
            else { DevExpress.ui.dialog.alert(NO_PERMISOS_PARA_BORRAR_INFORME, ELIMINAR_INFORME); }
        }
    }

    function handleSearchReports(event, motherCat = null) {
        let category;
        if (searchCategory) {
            let mothers = "";
            if (motherCat) {
                const motherCategories = [];
                while (motherCat) {
                    const newMotherCat = categories.find((cat) => cat.Id === motherCat);
                    motherCategories.push(newMotherCat);
                    motherCat = newMotherCat.MotherCategoryId;
                    motherCategories.map((cat) => (mothers += ` cat:${cat.Name};`));
                }
            }

            //category = searchCategory.innerText === TODAS_CATEGORIAS
            //			? ""
            //	: `cat:${searchCategory.innerText};${mothers && " " + mothers}`;

            category = searchCategory.innerText === TODAS_CATEGORIAS
                ? ""
                : `cat:${searchCategory.innerText}`;
        }
        else { category = ""; }

        originSearch.value = `${category} ${document.querySelector("#customSearchBar").value}`;
        originSearch.onchange();
    }

    function reloadCardsPanel() {
        originSearch = document.querySelector("#CardView_DXSE_I");
        $reportCards = [...document.querySelectorAll(".reportCard")];
        $dxcvCSD = document.querySelector(".dxcvCSD");

        checkCardsLastExecution();

        if ($reportCards.length > 0) {
            cardsPanelScrollLayer = populator.scrollTransparencyLayer($dxcvCSD);
            createCardsPanelObserver();
            cardsPanelObserver.observe($reportCards[$reportCards.length - 1]);
        }

        resizePanel();
        populator.blurAllTabs();
        populator.blurAnyReportCard($reportCards);
        $(".reportCard").off("click");
        $(".reportCard").on("click", handleCardClick);
    }

    function createCardsPanelObserver() {
        cardsPanelObserver = new IntersectionObserver(
            (entries) => {
                if (entries[0].isIntersecting && entries[0].intersectionRatio >= 0.5) { cardsPanelScrollLayer.style.opacity = 0; }
                else { cardsPanelScrollLayer.style.opacity = 1; }
            },
            { root: $dxcvCSD, threshold: [0, 0.5, 1] }
        );
    }

    const checkCardsLastExecution = () => {
        const dates = [...document.querySelectorAll(".reportCard li.lastExecution"),];
        populator.lastExecutions(dates);
    };

    async function handleCategoriesBtnClick() {
        await $.ajax({
            url: `${BASE_URL}Report/GetReportCategoriesAsJson`,
            type: "POST",
            dataType: "json",
            success: (data) => (categories = data),
            error: (error) => console.error(error),
        });

        if (categories) {
            populator.categoryTreeMenu(menuCategories, [
                { ID: 7, Description: TODAS_CATEGORIAS },
                ...categories,
            ]);
        }

        handleBtnCategoriesStyles();   //styles of Btn;
    }

    const handleBtnCategoriesStyles = (catClick = false) => {
        const $reportCategoriesBtn = $("#reportCategoriesBtn");
        const $reportCategoriesInnerScope = $("#reportCategoriesInnerScope");

        if (!!$("#reportCategoriesInnerScope").length) {
            isMenuCategoriesDisplayed = catClick ? true : isMenuCategoriesDisplayed;

            if (!isMenuCategoriesDisplayed) {
                $reportCategoriesBtn.addClass("reportTagsBtnActive");
                $reportCategoriesBtn
                    .find(".fa.fa-caret-down")
                    .addClass("fa-caret-downDroped");
                $reportCategoriesInnerScope.addClass("active");
                menuCategories.classList.add("menuCategoriesDisplayed");

                const surrounder = document.createElement("div");
                surrounder.setAttribute("id", "surrounder");
                menuCategories.parentNode.appendChild(surrounder);

                surrounder.addEventListener("click", handleCategoriesBtnClick, false);
            }
            else {
                const surrounder = document.querySelector("#surrounder");
                if (surrounder !== null) {
                    surrounder.remove();
                }
                $reportCategoriesBtn.removeClass("reportTagsBtnActive");
                $reportCategoriesInnerScope.removeClass("active");
                $reportCategoriesBtn
                    .find(".fa.fa-caret-down.fa-caret-downDroped")
                    .removeClass("fa-caret-downDroped");
                menuCategories.classList.remove("menuCategoriesDisplayed");
            }
        }
        //:: display <toggle> hide :: div with tags cloud
        isMenuCategoriesDisplayed = !isMenuCategoriesDisplayed;
    };

    async function handleExecutionTrigger(params = "") {
        let indexPlannedList = -1;
        if (!!document.querySelector("#plannerPage")) {
            utils.removeIdNewPlanifications();

            let paramsListSerielized = JSON.stringify(editedReportData.ParametersList);

            if (params != "") paramsListSerielized = JSON.stringify(params);

            const dataGridInstance = $("#reportPlannedList").dxDataGrid("instance");
            indexPlannedList =
                dataGridInstance.option("focusedRowKey") === "newPlanification"
                    ? editedReportData.PlannedExecutionsList.length - 1
                    : editedReportData.PlannedExecutionsList.findIndex(
                        (plan) => plan.Id === dataGridInstance.option("focusedRowKey")
                    );

            if (editedReportData.PlannedExecutionsList[indexPlannedList] === undefined) {
                editedReportData.PlannedExecutionsList[indexPlannedList] = {};
                editedReportData.PlannedExecutionsList[indexPlannedList].ParametersJson = {};
                editedReportData.PlannedExecutionsList[indexPlannedList].ParametersJson = paramsListSerielized;
            }
            else { editedReportData.PlannedExecutionsList[indexPlannedList].ParametersJson = paramsListSerielized; }

            const isSaved = await editor.acceptEdition(false, "planifications");
            isSaved && handlePlanExecutionClick();
            return;
        }

        const reportId = currentReportData.Id;
        if (!reportId) throw new Error("no se Identifica ningún Report seleccionado");

        const idTask = (indexPlannedList != -1) ? await triggerExecution(reportId, JSON.stringify(params), JSON.stringify(editedReportData.PlannedExecutionsList[indexPlannedList].ViewFields)) : await triggerExecution(reportId, JSON.stringify(params));

        !idTask && DevExpress.ui.dialog.alert(`${NO_SE_PUDO_LANZAR_INFORME} ${INTENTELO_DE_NUEVO}`, LANZANDO_INFORME);

        //console.log(params);
        //console.log(editedReportData.ParametersList);

        currentReportData.ExecutionsList.unshift({
            BlobLink: "",
            ExecutionDate: new Date(),
            FileLink: "#",
            Guid: "",
            LayoutID: currentReportData.Id,
            Status: 1,
        });
        //make new execution appear in progress status
        displayExecutions(currentReportData);

        setTimeout(checkExecutionsInProgress, 3000);
    }

    function validation() {
        var Parameters = editedReportData.ParametersList.filter((prm) => prm.TemplateName !== "hidden");
        validated = true;

        if (Parameters.length > indexParamsSignatures && Parameters[indexParamsSignatures]) {
            switch (Parameters[indexParamsSignatures].TemplateName) {
                case "selectorEmployees":// employees , required IDEmployee
                    if (Parameters[indexParamsSignatures].Value === "-1@11110@") {
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                            this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                            { text: TITLE_VALIDATION, key: '' },
                            { text: MSG_VALIDATION_EMPLOYEE, key: '' },
                            { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton()
                        )

                        validated = false;
                    }
                    break;

                case "selectorPeriodTime":
                    if (Parameters[indexParamsSignatures].Value === "") {
                        const momentFormat = "YYYY-MM-DD HH:mm:ss";
                        const checked = document.querySelector("input[type=radio]:checked").value;
                        const initDateInput = $("#desdeDateTime").dxDateBox("instance");
                        const finalDateInput = $("#hastaDateTime").dxDateBox("instance");
                        const { initDate, finalDate } = editor.selectorDatetime(parseInt(checked, 10), initDateInput, finalDateInput);
                        const periodParam =
                            checked !== "0"
                                ? `${checked},${initDate.format()},${finalDate.format(
                                    momentFormat
                                )}`
                                : () => {
                                    return `${checked},${window.parent
                                        .moment(initDate())
                                        .format(momentFormat)},${window.parent
                                            .moment(finalDate())
                                            .format(momentFormat)}`;
                                };
                        Parameters[indexParamsSignatures].Value = periodParam;
                    }
                    break;

                case "selectorViewFormat":
                    if (Parameters[indexParamsSignatures].Value === "") Parameters[indexParamsSignatures].Value = { ckDetail: false, ckTotal: false, ckShowChart: false };

                    if (
                        (Parameters[indexParamsSignatures].Value.ckDetail === false)
                        &&
                        (Parameters[indexParamsSignatures].Value.ckTotal === false)

                    ) {
                        collectParamValues("chart", false, null);
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                            this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                            { text: TITLE_VALIDATION, key: '' },
                            { text: MSG_VALIDATION_DISPLAY, key: '' },
                            { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton()
                        )

                        validated = false;
                    }
                    if (
                        (Parameters[indexParamsSignatures].Value.ckShowChart === true)
                        &&
                        (currentReportData.Name == "Saldos con detalle")
                        &&
                        (currentReportData.ParametersList != null && currentReportData.ParametersList.length > 0 && currentReportData.ParametersList[3] != null)
                        && (currentReportData.ParametersList[3].Value.split(",").length > 10)

                    ) {
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                            this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                            { text: TITLE_VALIDATION, key: '' },
                            { text: MSG_VALIDATION_CHART, key: '' },
                            { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton()
                        )

                        validated = false;
                    }
                    break;
                case "selectorAccessType":
                    if (Parameters[indexParamsSignatures].Value === "") Parameters[indexParamsSignatures].Value = { ckValids: false, ckInvalids: false };

                    if (
                        (Parameters[indexParamsSignatures].Value.ckValids === false)
                        &&
                        (Parameters[indexParamsSignatures].Value.ckInvalids === false)
                    ) {
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                            this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                            { text: TITLE_VALIDATION, key: '' },
                            { text: MSG_VALIDATION_DISPLAY, key: '' },
                            { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton()
                        )

                        validated = false;
                    }
                    break;
                case "selectorProfileTypes":
                    let parameters = Parameters[indexParamsSignatures].Value;
                    if (Parameters[indexParamsSignatures].Value === "") Parameters[indexParamsSignatures].Value = { profileTypes: 0 };
                    const validateProfileTypes = [1, 2, 3, 4].includes(parameters.profileTypes);
                    let validateSelectors = false;
                    var validator = $("#valorCriterio").dxValidator("instance");
                    var criterioFormat = validator.validate();
                    const validateCriterioParams = (parameters.criterio !== undefined && parameters.rangoCriterio !== undefined && parameters.valorCriterio !== null && parameters.valorCriterio !== undefined && criterioFormat.isValid);
                    if (validateProfileTypes) {
                        switch (parameters.profileTypes) {
                            case 1:
                                //validateSelectors = (parameters.selectConcepts !== undefined && parameters.selectConcepts !== "" && parameters.selectConcepts.length != 0);
                                validateSelectors = (
                                    parameters.selectConcepts !== undefined &&
                                    parameters.selectConcepts !== "" &&
                                    parameters.selectConcepts.length !== 0 &&
                                    parameters.selectConcepts.every(e => parseInt(e.value, 10) !== 0) &&
                                    !parameters.selectConcepts.every(e => parseInt(e.value, 10) === -1) &&
                                    typeof (parameters.rangoCriterio) === 'string'
                                );
                                break;
                            case 2:
                                validateSelectors = (parameters.selectCauses !== undefined && parameters.selectCauses !== "" && parameters.selectCauses.length != 0);
                                break;
                            case 3:
                                validateSelectors = (parameters.selectIncidences !== undefined && parameters.selectIncidences !== "" && parameters.selectIncidences.length != 0);
                                break;
                            case 4:
                                validateSelectors = (parameters.selectCauses !== undefined && parameters.selectCauses !== "" && parameters.selectCauses.length != 0) && (parameters.selectIncidences !== undefined && parameters.selectIncidences !== "" && parameters.selectIncidences.length != 0);
                                break;
                        }
                    }
                    const validateSelectedUserField = parameters?.tipoValorCriterio != "campo" || (parameters?.tipoValorCriterio == "campo" && valorFichaCriterioData.find(el => el.value == parameters?.valorFichaCriterio));
                    if (
                        !(validateProfileTypes && validateSelectors && validateCriterioParams && validateSelectedUserField)
                    ) {
                        if (criterioFormat == null || criterioFormat.isValid) {
                            this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                                this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                                { text: TITLE_VALIDATION, key: '' },
                                { text: MSG_VALIDATION_DISPLAY, key: '' },
                                { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton()
                            )
                        }
                        else
                            this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                                this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                                { text: TITLE_VALIDATION, key: '' },
                                { text: MSG_VALIDATION_CRITERIA_DISPLAY, key: '' },
                                { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton()
                            )

                        validated = false;
                    }
                    break;
                case "selectorCausesRegistroJL":

                    //Revisarlo, pero tenemos que dar error si ha seleccionado mostrar justif. y que el selector no haya seleccionado nada.
                    let params = Parameters[indexParamsSignatures].Value;
                    let validateCauses = false;
                    if (Parameters[indexParamsSignatures].Value === "") Parameters[indexParamsSignatures].Value = { showCauses: 0 };

                    if (params.showCauses == true) {
                        validateCauses = (params.selectCauses !== undefined && params.selectCauses !== "" && params.selectCauses.length != 0);
                        if (!validateCauses) {
                            this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                                this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                                { text: TITLE_VALIDATION, key: '' },
                                { text: MSG_VALIDATION_DISPLAY, key: '' },
                                { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton()
                            )
                            validated = false;
                        }
                    }
                    break;
                case "selectorConceptsRegistroJL":

                    //Revisarlo, pero tenemos que dar error si ha seleccionado mostrar justif. y que el selector no haya seleccionado nada.
                    let paramsConcepts = Parameters[indexParamsSignatures].Value;
                    let validateConcepts = false;
                    if (Parameters[indexParamsSignatures].Value === "") Parameters[indexParamsSignatures].Value = { showConcepts: 0 };

                    if (paramsConcepts.showConcepts == true) {
                        validateConcepts = (paramsConcepts.selectConcepts !== undefined && paramsConcepts.selectConcepts !== "" && paramsConcepts.selectConcepts.length != 0);
                        if (!validateConcepts) {
                            this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                                this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                                { text: TITLE_VALIDATION, key: '' },
                                { text: MSG_VALIDATION_DISPLAY, key: '' },
                                { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton()
                            )
                            validated = false;
                        }
                    }
                    break;
                case "selectorYearAndMonth":
                    break;
                case "selectorBetweenYearAndMonth":
                    break;
                case "selectorConcepts":
                    if (Parameters[indexParamsSignatures].Value === "") {
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                            this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                            { text: TITLE_VALIDATION, key: '' },
                            { text: MSG_VALIDATION_CONCEPTS, key: '' },
                            { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton()
                        )
                        validated = false;
                    }
                    break;
                case "selectorCauses":
                    if (Parameters[indexParamsSignatures].Value === "") {
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                            this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                            { text: TITLE_VALIDATION, key: '' },
                            { text: MSG_VALIDATION_CONCEPTS, key: '' },
                            { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton()
                        )
                        validated = false;
                    }
                    break;

                case "selectorDate":
                    if (Parameters[indexParamsSignatures].Value === "") {
                        const momentFormat = "YYYY-MM-DD HH:mm:ss";
                        const checked = document.querySelector("input[type=radio]:checked").value;
                        const initDateInput = $("#parameterDate").dxDateBox("instance");
                        const { initDate } = editor.selectorDatetime(parseInt(checked, 10), initDateInput);
                        const periodParam =
                            checked !== "0"
                                ? `${checked},${initDate.format()}`
                                : () => {
                                    return `${checked},${initDate()}`;
                                };
                        Parameters[indexParamsSignatures].Value = periodParam;
                    }
                    break;
                case "selectorProjectsVSL":
                    let paramsP = Parameters[indexParamsSignatures].Value;
                    const validateProjectParams = (paramsP.startProject !== undefined && paramsP.endProject !== undefined && paramsP.startProject > paramsP.endProject);

                    if (validateProjectParams) {
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                            this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                            { text: TITLE_VALIDATION, key: '' },
                            { text: MSG_VALIDATION_PROJECTS_MINMAX, key: '' },
                            { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton()
                        )
                        validated = false;
                    }
                    break;
                case "selectorFormat":
                    break;
                case "selectorUniversal":
                    if (!isNaN(Parameters[indexParamsSignatures].Description) && Parameters[indexParamsSignatures].Type == 'Robotics.Base.userFieldsSelector') {

                        let shiftsMaxLength = parseInt(Parameters[indexParamsSignatures].Description, 10);


                        if (Parameters[indexParamsSignatures].Value.split(",").length > shiftsMaxLength) {
                            this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                                this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                                { text: TITLE_VALIDATION, key: '' },
                                { text: MSG_VALIDATION_MAXNUMBER.replace('(1)', shiftsMaxLength), key: '' },
                                { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                                this.parent.Robotics.Client.JSErrors.createEmptyButton()
                            )


                            validated = false;
                        }


                    }
                    if ((Parameters[indexParamsSignatures].Type == 'Robotics.Base.conceptGroupsSelector')
                        &&
                        (currentReportData.Name == "Saldos día a día")
                        &&
                        (currentReportData.ParametersList != null && currentReportData.ParametersList.length > 0 && currentReportData.ParametersList[5] != null)
                        && (currentReportData.ParametersList[5].Value.split(",").length > 1)

                    ) {
                        this.parent.Robotics.Client.JSErrors.showJSerrorPopup(
                            this.parent.Robotics.Client.JSErrors.JSErrorTypes.roJsError, '',
                            { text: TITLE_VALIDATION, key: '' },
                            { text: MSG_VALIDATION_CONCEPTGROUPS, key: '' },
                            { text: '', textkey: 'SI', desc: '', desckey: '', script: '' },
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton(),
                            this.parent.Robotics.Client.JSErrors.createEmptyButton()
                        )

                        validated = false;
                    }


                    break;
                default:
                    break;
            }
        }
    }

    const sleep = (duration) => {
        return new Promise(resolve => setTimeout(resolve, duration));
    }

    const askReportParams = async (origin) => {

        //if call from btn Lanzar set parameters from bbdd
        if ((origin != undefined)) {
            var currentData = currentReportData.ParametersList;
            var element;

            //PLANIFICATION
            if (planificationsTab.classList.contains("activeTab")) {
                var Data;
                var bbddLastPlannification;
                var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                {
                    bbddLastPlannification = currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson;
                }
                if (bbddLastPlannification != undefined) {
                    for (cd in currentData) {
                        element = JSON.parse(bbddLastPlannification).find((param) => param.TemplateName === currentData[cd].TemplateName && param.Name === currentData[cd].Name)
                        if (element != undefined) currentData[cd].Value = element.Value;
                    }
                }
            }
            //REPORT
            else {
                var bbddLastReport = currentReportData.LastParameters;

                if (bbddLastReport != undefined) {
                    for (cd in currentData) {
                        element = JSON.parse(bbddLastReport).find((param) => param.TemplateName === currentData[cd].TemplateName && param.Name === currentData[cd].Name)
                        if (element != undefined) {
                            currentData[cd].Value = element.Value;
                        }
                    }
                }
            }
        }

        paramsSignatures = currentReportData.ParametersList.filter((prm) => prm.TemplateName !== "hidden");
        const hiddenSignatures = currentReportData.ParametersList.filter((prm) => prm.TemplateName === "hidden");

        if (!paramsSignatures || paramsSignatures.length < 1) {
            handleExecutionTrigger(!!hiddenSignatures && hiddenSignatures);
            return;
        }

        for (let i = 0; i < paramsSignatures.length; i++) {
            if (paramsSignatures[i].TemplateName == "selectorEmployees") {
                sessionStorage.ReportEmployees_hdnNodes = paramsSignatures[i].Value;

                if (typeof origin != 'undefined') {
                    await getroTreeState('objContainerTreeV3_treeEmployeesReportExecution').then(roState => roState.reset());
                    await getroTreeState('objContainerTreeV3_treeEmployeesReportExecutionGrid').then(roState => roState.reset());

                    let recportCurrentFilter = paramsSignatures[i].Value.split("@");

                    let selectedUsers = recportCurrentFilter[0];
                    let selectedFilter = '11110';
                    let selectedUserFields = '';

                    if (recportCurrentFilter.length >= 2) {
                        selectedFilter = recportCurrentFilter[1];
                        selectedUserFields = recportCurrentFilter[2];
                    }

                    let state = await getroTreeState('objContainerTreeV3_treeEmployeesReportExecutionGrid', true);
                    await state.setLocalData(selectedUsers, '', '', '', '');

                    state = await getroTreeState('objContainerTreeV3_treeEmployeesReportExecution', true)
                    await state.setLocalData('', '', '', selectedFilter, selectedUserFields);
                }
            } else if (paramsSignatures[i].TemplateName == "selectorTasks") {
                if (typeof origin != 'undefined') {
                    await getroTreeState('objContainerTreeV3_treeTaskReportProfile').then(roState => roState.reset());
                    let recportCurrentFilter = paramsSignatures[i].Value.split("@");
                    let tasksAndProjects = recportCurrentFilter[0].split(',');

                    let tasksIds = [];
                    let projectIds = [];
                    for (let taskCount = 0; taskCount < tasksAndProjects.length; taskCount++) {
                        if (isNaN(parseInt(tasksAndProjects[taskCount], 10))) projectIds.push(tasksAndProjects[taskCount]);
                        else tasksIds.push(tasksAndProjects[taskCount]);
                    }

                    let state = await getroTreeState('objContainerTreeV3_treeTaskReportProfile', true);
                    await state.setLocalData(tasksIds.join(','), projectIds.join(','), '', '', '');
                }
            }
        }

        const fieldsEditor = document.querySelector("#fieldsEditor");
        fieldsEditor.style.display = "";
        if (currentReportData.Name == "cumplen criterio") {
            fieldsEditor.style.width = "40vw";
            fieldsEditor.style.left = "30vw";
            fieldsEditor.style.minWidth = "800px";
        }


        //prompt for each param
        const isFirstParam = indexParamsSignatures === 0;
        const isLastParam = indexParamsSignatures === paramsSignatures.length - 1;
        const template = await getViewTemplate(paramsSignatures[indexParamsSignatures].TemplateName);

        if (isFirstParam) {
            const paramSelectorHeader = await getViewTemplate("selectorTitle");
            fieldsEditor.innerHTML = paramSelectorHeader;
        }

        const paramCounter = fieldsEditor.querySelector("#parametersTitle #parametersCounter");
        paramCounter.innerText = `${indexParamsSignatures + 1}`;

        const paramCounterTotal = fieldsEditor.querySelector("#parametersTitle #parametersCounterTotal");
        paramCounterTotal.innerText = `${paramsSignatures.length}`;

        const paramNameSlot = fieldsEditor.querySelector("span#parametersName > b");
        const paramName = utils.translateParamName(paramsSignatures[indexParamsSignatures]);
        paramNameSlot.innerText = paramName;

        fieldsEditor.innerHTML += template;

        if (isFirstParam) {
            const buttonsStr = await getViewTemplate("editViewBtns");
            fieldsEditor.innerHTML += buttonsStr;
        }

        const buttons = fieldsEditor.querySelector(".editViewBtns");
        const acceptParamsInput = buttons.querySelector("#acceptEdition");
        const cancelBtn = buttons.querySelector("#cancelEdition");
        const nextBtn = buttons.querySelector("#nextEdition");
        const previousBtn = buttons.querySelector("#previousEdition");

        if (!isLastParam) {
            acceptParamsInput.parentElement.style.display = "none";
            nextBtn.parentElement.style.display = "";
            nextBtn.parentElement.addEventListener("click", editor.nextEdition, false);
        }
        else {
            acceptParamsInput.parentElement.style.display = "";
            nextBtn.parentElement.style.display = "none";
        }

        if (indexParamsSignatures !== 0) {
            previousBtn.parentElement.style.display = "";
            previousBtn.parentElement.addEventListener("click", editor.previousEdition, false);
        }

        acceptParamsInput.parentElement.addEventListener("click", () => {
            editor.acceptExecutionParams();
            if (validated) {
                indexParamsSignatures = 0;
            }
        }, false
        );

        cancelBtn.parentElement.addEventListener("click", () => {
            editor.cancelEdition();
            indexParamsSignatures = 0;
        }, false
        );

        let paramTemplate = paramsSignatures[indexParamsSignatures].TemplateName;
        var selectorContainer;
        switch (paramTemplate) {
            case "selectorViewFormat":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorViewFormat(selectorContainer);
                break;
            case "selectorAccessType":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorAccessType(selectorContainer);
                break;
            case "selectorProfileTypes":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorProfileTypes(selectorContainer);
                break;
            case "selectorConceptsRegistroJL":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorConceptsRegistroJL(selectorContainer);
                break;
            case "selectorCausesRegistroJL":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorCausesRegistroJL(selectorContainer);
                break;
            case "selectorYearAndMonth":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorYearAndMonth(selectorContainer);
                break;
            case "selectorBetweenYearAndMonth":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorBetweenYearAndMonth(selectorContainer);
                break;
            case "selectorFormat":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorFormat(selectorContainer);
                break;
            case "selectorFilterValues":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorFilterValues(selectorContainer);
                break;
            case "selectorDate":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorDate(selectorContainer);
                break;
            case "selectorProjectsVSL":
                selectorContainer = mainPanelDisplay.querySelector("#" + paramTemplate);
                populator.selectorProjectsVSL(selectorContainer);
                break;

            default:
                var component = document.querySelector("#editView>Form").getAttribute("id");
                if (populator[component] !== undefined) populator[component]();
                break;
        }

        setFormValuesFromReportData(paramTemplate);
    };
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------

    function handleTabClick() {
        const planificationsTab = document.querySelector("#planificationsTab");

        if (!planificationsTab.classList.contains("activeTab")) {
            //ensure show all parts of div
            [...mainPanelViews].map((div) => (div.style.display = ""));

            displayPdfViewerData(currentReportData.LayoutPreviewXMLBinary);
            displayExecutions(currentReportData);
            displayConfigPage(currentReportData);
        } else {
            updateReportActions(currentReportData.Id);
        }
    }

    async function handleCardClick() {
        const newReportId = $(this).find(".reportCardInfo").attr("data-report-id");

        var prueba = $(".treeCaption");
        //$(".treeCaption").load("cardView", Model);
        //$('.treeCaption').reloadCardsPanel;
        //take source to preview, set it to current report
        await loadCurrentReport(newReportId);

        const planificationsTab = document.querySelector("#planificationsTab");
        const generalTab = document.querySelector("#generalTab");

        if (planificationsTab.classList.contains("activeTab")) { document.querySelector("#planificationsTab").click(); }

        //reset tabs to preview
        handleTabClick();

        //change state of this
        handleCardClickStyles($(this));
    }

    const handleCardClickStyles = (thisCard) => {
        //reset styles
        $reportCards.map((card) => {
            card.classList.remove("reportCardClicked");
            const dwnBtn = card.querySelector(".reportExecution.executiuonIsFine > i");
            dwnBtn && dwnBtn.classList.remove("active");
        });

        //apply new styles
        thisCard.addClass("reportCardClicked");
        thisCard.find(".reportExecution.executiuonIsFine > i").addClass("active");
    };

    const handleCloneReportClick = async () => {
        //popup intro name dialog
        const nameClonedReport = await getViewTemplate("nameClonedReport");
        const fieldsEditor = document.querySelector("#fieldsEditor");

        fieldsEditor.style.height = "150px";
        fieldsEditor.style.display = "flex";
        fieldsEditor.style.justifyContent = "center";
        fieldsEditor.style.flexFlow = "column nowrap";
        fieldsEditor.innerHTML = nameClonedReport;

        const inputInstance = $("#clonedReportInput")
            .dxTextBox({ placeholder: CLONE_NAME_PLACEHOLDER, width: 350, })
            .dxValidator({
                validationRules: [{
                    type: 'required',
                    message: 'Debes informar el nombre del informe',
                }],
            })

        //handle accept
        fieldsEditor.querySelector("#acceptEdition").parentNode.addEventListener(
            "click",
            async () => {
                const validationResult = inputInstance.dxValidator("instance").validate();
                if (validationResult.isValid) {
                    const reportName = inputInstance.dxTextBox("instance").option("value");
                    const clonedOK = await copyReport(currentReportData.Id, reportName);
                    if (clonedOK) {
                        editor.hideEditor();
                        reloadCardsPanel();
                        utils.searchInCardsPanel(reportName, true);
                    }
                    else { DevExpress.ui.dialog.alert(ALGO_MAL_INTENTAR_CLONAR_INFORME, CLONANDO_INFORME); }
                }
            },
            false
        );

        fieldsEditor.querySelector("#cancelEdition").parentNode.addEventListener("click", () => { editor.hideEditor(); }, false);
    };

    const handleShrinkExpandCommTree = (shrinkBtn, expandBtn, isShrink) => {
        const divTree = document.querySelector("#divTree");
        if (isShrink) {
            divTree.style.display = "none";
            shrinkBtn.style.display = "none";
            expandBtn.style.display = "";
        }
        else {
            divTree.style.display = "";
            expandBtn.style.display = "none";
            shrinkBtn.style.display = "";
        }
    };

    const updateReportActions = async (reportid) => {
        const reportActions = document.querySelector("#reportConfigContainer .executionActions");
        await populator.reportActions(reportActions, "listTabReportActions");

        const triggerExecBtn = reportActions.querySelector("#triggerExecutionBtn");

        const shrinkBtnOld = document.querySelector("#treeShrinkBtn");
        const expandBtnOld = document.querySelector("#treeExpandBtn");
        const openDesignerLinkOld = document.querySelector("#reportEditBtn");
        const reportCreateBtnOld = document.querySelector("#reportCreateBtn");
        const removeReportBtnOld = document.querySelector("#reportRemoveBtn");
        const reportCloneBtnOld = document.querySelector("#reportCloneBtn");
        const generalTabOld = document.querySelector("#generalTab");
        const planificationsTabOld = document.querySelector("#planificationsTab");
        const addPlanificationBtnOld = document.querySelector("#reportAddPlanificationBtn");

        const shrinkBtn = shrinkBtnOld.cloneNode(true);
        const expandBtn = expandBtnOld.cloneNode(true);
        const openDesignerLink = openDesignerLinkOld && openDesignerLinkOld.cloneNode(true);
        const reportCreateBtn = reportCreateBtnOld && reportCreateBtnOld.cloneNode(true);
        const removeReportBtn = removeReportBtnOld && removeReportBtnOld.cloneNode(true);
        const reportCloneBtn = reportCloneBtnOld && reportCloneBtnOld.cloneNode(true);
        const generalTab = generalTabOld.cloneNode(true);
        const planificationsTab = planificationsTabOld.cloneNode(true);
        const addPlanificationBtn = addPlanificationBtnOld && addPlanificationBtnOld.cloneNode(true);

        shrinkBtnOld.parentNode.replaceChild(shrinkBtn, shrinkBtnOld);
        expandBtnOld.parentNode.replaceChild(expandBtn, expandBtnOld);
        openDesignerLinkOld && openDesignerLinkOld.parentNode.replaceChild(openDesignerLink, openDesignerLinkOld);
        reportCreateBtnOld && reportCreateBtnOld.parentNode.replaceChild(reportCreateBtn, reportCreateBtnOld);
        removeReportBtnOld && removeReportBtnOld.parentNode.replaceChild(removeReportBtn, removeReportBtnOld);
        reportCloneBtnOld && reportCloneBtnOld.parentNode.replaceChild(reportCloneBtn, reportCloneBtnOld);
        generalTabOld.parentNode.replaceChild(generalTab, generalTabOld);
        planificationsTabOld.parentNode.replaceChild(planificationsTab, planificationsTabOld);
        addPlanificationBtnOld && addPlanificationBtnOld.parentNode.replaceChild(addPlanificationBtn, addPlanificationBtnOld);

        triggerExecBtn.addEventListener("click", () => { editedReportData = { ...currentReportData }; askReportParams(); }, false);
        removeReportBtn && removeReportBtn.addEventListener("click", handleRemoveReportClick, false);
        reportCloneBtn && reportCloneBtn.addEventListener("click", handleCloneReportClick, false);
        generalTab.addEventListener("click", () => { exitPlanner(document.querySelector(".switchMainViewTabs")); }, false);
        planificationsTab.addEventListener("click", handlePlanExecutionClick, false);

        addPlanificationBtn && addPlanificationBtn.addEventListener
            (
                "click",
                async () => {
                    await handlePlanExecutionClick();
                    editor.addPlanification(document.querySelector("#reportPlannedList"));
                },
                false
            );
        shrinkBtn.addEventListener("click", () => { handleShrinkExpandCommTree(shrinkBtn, expandBtn, true); }, false);
        expandBtn.addEventListener("click", () => { handleShrinkExpandCommTree(shrinkBtn, expandBtn, false); }, false);

        reportCreateBtn && (reportCreateBtn.style.display = "");

        if (!currentReportData.Permissions.Remove || !utils.permissionsOverUser(9)) { removeReportBtn && (removeReportBtn.style.display = "none"); }
        else { removeReportBtn && (removeReportBtn.style.display = ""); }

        if (currentReportData.Permissions.Edit) {
            openDesignerLink && (openDesignerLink.style.display = "");
            openDesignerLink && openDesignerLink.setAttribute(
                "href",
                `javascript:openDesignerLink(/OpenReportDesigner/,${reportid})`
            );
        }
        else { openDesignerLink && (openDesignerLink.style.display = "none"); }

        addPlanificationBtn && (addPlanificationBtn.style.display = "");

        if (currentReportData.IsEmergencyReport) {
            reportCloneBtn && (reportCloneBtn.style.display = "none");
            //if (currentReportData.PlannedExecutionsList.length >= 1) { addPlanificationBtn && (addPlanificationBtn.style.display = "none"); } //Ocultamos el botón de planificación para informes de emergencia
        } else {
            reportCloneBtn && (reportCloneBtn.style.display = "");
        }

        //   // botón icono si hay  planificaciones ---------------------
        //var idPLanImgReport = "imagePlanReport" + reportid ;
        //if (document.getElementById(idPLanImgReport) !== null)
        //{
        //	while (document.getElementById(idPLanImgReport).childNodes[0] !== undefined) {
        //	 document.getElementById(idPLanImgReport).removeChild(document.getElementById(idPLanImgReport).childNodes[0]);
        //	}

        //	if (currentReportData.PlannedExecutionsList.length >0)
        //	{
        //		var newElement = document.createElement("img");
        //		newElement.setAttribute("src", BASE_URL + "Base/Images/StartMenuIcos/Events.png");
        //		newElement.setAttribute("readonly", "true");
        //		newElement.setAttribute("width", "16px");
        //		document.getElementById(idPLanImgReport).appendChild(newElement);
        //	}
        //      }
    };

    const fill = (number, len) => "0".repeat(len - number.toString().length) + number.toString();

    function resizePanel() { $dxcvCSD.parentNode.style.height = `${window.innerHeight - 250}px`; }

    function displayPdfViewerData(blobImagex64 = "") {
        const preview = document.querySelector("#reportPdfViewer > .imgPreview");
        preview.style.backgroundImage = `url("data:image/png;base64,${blobImagex64}")`;
    }

    function displayConfigPage(reportData) {
        const configPage = [...mainPanelViews].find((pan) => "reportConfigContainer" === pan.getAttribute("id"));
        const reportInfoDiv = configPage.querySelector("#reportInfo");

        populator.reportLayoutInfo(reportData, reportInfoDiv);
    }

    function displayExecutions(reportData) {
        const executionsPage = document.querySelector("#reportExecContainer");
        const historicDiv = executionsPage.querySelector("#reportsHistoricContainer");
        const uList = executionsPage.querySelector("ul.reportsHistoric");

        updateReportActions(currentReportData.Id, (currentReportData.LastExecution && currentReportData.LastExecution.Guid) || null);
        populator.executionsList(uList, reportData.ExecutionsList.sort((a, b) => (a.ExecutionDate > b.ExecutionDate) ? -1 : 1), historicDiv);
    }

    function handleMenuCategoryClick(thisCategory) {
        const motherCat = null;
        searchCategory.innerText = thisCategory.Description;
        handleSearchReports(null, motherCat);
        stylesCategoryClick(thisCategory.Description);
    }

    function setFormValuesFromReportData(paramTemplate) {
        switch (paramTemplate) {
            case "selectorDate":
                var DtCurrent = currentReportData.ParametersList.find((param) => param.TemplateName === "selectorDate" && param.Name === paramsSignatures[indexParamsSignatures].Name);

                const initDateInput = $("#parameterDate").dxDateBox("instance");
                initDateInput.option('value', moment(initDateInput.option('value')).format("YYYY-MM-DD"));

                var selectedOption = 0;
                var sDate = null;
                var selectable = null;
                if (DtCurrent.Value.indexOf(',') > 0) {
                    selectedOption = DtCurrent.Value.split(',')[0];
                    sDate = DtCurrent.Value.split(',')[1];

                    selectable = document.querySelector(`#selectorDate input[value='${selectedOption}']`);
                } else {
                    sDate = DtCurrent.Value;

                    var dd = new Date(sDate).getDate();
                    var mm = new Date(sDate).getMonth() + 1;
                    var yyyy = new Date(sDate).getFullYear();

                    if ((isNaN(dd) || isNaN(mm) || isNaN(yyyy))) {
                        sDate = new Date();
                        dd = new Date(sDate).getDate();
                        mm = new Date(sDate).getMonth() + 1;
                        yyyy = new Date(sDate).getFullYear();
                    }
                    selectable = document.querySelector(`#selectorDate input[value='0']`);
                }

                if (selectable) selectable.setAttribute("checked", "");

                if (selectedOption !== 0 && selectedOption !== "0") {
                    initDateInput.option('disabled', true);
                }

                //parameterDate.value = selectedOption + ',' + moment(sDate).format('YYYY-MM-DD');
                break;

            default:
                break;
        }
    }

    function stylesCategoryClick(thisCategoryName) {
        allCategories = document.querySelectorAll(".categoryText tr .dx-treelist-text-content");

        // this must be by ID not string of text, to avoid repeats (idea: getAttribute("data-id"))
        const targetCategories = [...allCategories].filter((cat) => cat.innerText === thisCategoryName);

        //highlight Categories this must be by ID not string of text, to avoid repeats
        targetCategories.map((cat) => { cat.parentNode.classList.toggle("targetCategory"); });
    }

    async function handleExecutionDownload(event) {
        // when an execution is selected, the download button link is updated
        //console.log(event);
        //console.log(event.path);
        const thisExecution = event.path ? event.path[0] : event.currentTarget; //ie & edge
        const executionId = thisExecution.getAttribute("data-guid")
            || (currentReportData.LastExecution && currentReportData.LastExecution.Guid)
            || 0;

        const blob = await getExportDataAndExtension(executionId);

        if (blob.size > 0) {
            var docExt = currentReportData.ExecutionsList.find((prm) => prm.Guid === executionId).Extension;

            const aElement = document.createElement("a");
            aElement.setAttribute("download", `${currentReportData.DisplayName == 'NotFound' ? currentReportData.Name : currentReportData.DisplayName}.${docExt}`);
            aElement.setAttribute("href", window.URL.createObjectURL(blob));
            aElement.click();
            aElement.remove();
        }
        else { DevExpress.ui.dialog.alert(NO_SE_PUEDE_RECUPERAR_EJECUCION, DESCARGANDO_EJECUCION); }
    }

    const checkExecutionsInProgress = async () => {
        const executions = [
            ...currentReportData.ExecutionsList.filter((exec) => exec.Status === 1),
        ];
        //console.log("executions in progress", executions);

        await executions.map(async (exec) => {
            //const status = await getExecutionStatus(exec.Guid);
            if (status !== 1) {
                await loadCurrentReport(currentReportData.Id);
                displayExecutions(currentReportData);
                populator.singleCard(
                    document.querySelector(".reportCard.reportCardClicked"),
                    currentReportData
                );
            }
        });

        if (executions.length > 0) {
            setTimeout(checkExecutionsInProgress, 3000);
        }
    };

    const loadCurrentReport = async (reportId) => {
        //update global var 'currentReportData'
        let importedReportObj;

        await $.ajax({
            url: `${BASE_URL}Report/GetReportByIdAsJson`,
            data: { reportId },
            type: "POST",
            dataType: "json",
            success: (data) => (importedReportObj = data),
            error: (error) => console.error(error),
        });
        currentReportData = utils.viewFieldsToParsed(importedReportObj);
        //console.log(currentReportData);
        if (!editedReportData || editedReportData?.Id != currentReportData?.Id) editedReportData = { ...currentReportData };
        window.currentReportData = currentReportData;
    };

    const getExportDataAndExtension = async (executionId) => {
        let file;
        await $.ajax({
            url: `${BASE_URL}Report/GetExportDataAndExtension`,
            data: { executionId },
            type: "POST",
            xhrFields: { responseType: "blob", },
            success: (data) => { file = data; },
            error: (error) => { }//console.log("GET PDF:", error),
        });

        return file;
    };

    const triggerExecution = async (reportId, reportParameters, viewFields) => {
        let idTask;
        await $.ajax({
            url: `${BASE_URL}Report/TriggerExecution`,
            data: { reportId, reportParameters, viewFields },
            type: "POST",
            dataType: "json",
            success: (data) => (idTask = data),
            error: (error) => console.error(error),
        });

        return idTask;
    };

    const getExecutionStatus = async (executionId) => {
        let taskStatus;
        await $.ajax({
            url: `${BASE_URL}Report/GetExecutionStatus`,
            data: { executionId },
            type: "POST",
            dataType: "json",
            success: (data) => (taskStatus = data),
            error: (error) => console.error(error),
        });

        return taskStatus;
    };

    const getAllLanguages = async () => {
        let langs;

        await $.ajax({
            url: `${BASE_URL}Report/GetAllLanguages`,
            data: {},
            type: "POST",
            dataType: "json",
            success: (data) => (langs = data),
            error: (error) => { },
        });

        return langs;
    };

    const copyReport = async (reportId, newReportName) => {
        let res;

        await $.ajax({
            url: `${BASE_URL}Report/CopyReport`,
            data: { reportId, newReportName },
            type: "POST",
            dataType: "text",
            success: (data) => (res = data),
            error: (e) => { },
        });

        return res === "True";
    };

    const updateReportCategory = async (reportId, categoryId, categoryLevel) => {
        let res;

        await $.ajax({
            url: `${BASE_URL}Report/UpdateReportCategories`,
            data: { reportId, categoryId, categoryLevel },
            type: "POST",
            dataType: "text",
            success: (data) => (res = data),
            error: (e) => { },
        });

        return res === "True";
    };

    const getReportsPageConfig = async () => {
        let configObj;

        await $.ajax({
            url: `${BASE_URL}Report/GetReportsPageConfiguration`,
            data: {},
            type: "POST",
            dataType: "json",
            success: (data) => (configObj = data),
            error: (error) => console.error(error),
        });

        return configObj;
    };

    const getUserPassportId = async () => {
        let passportId;
        await $.ajax({
            url: `${BASE_URL}Report/GetUserPassportID`,
            data: {},
            type: "POST",
            dataType: "json",
            success: (data) => (passportId = data),
            error: (error) => console.error(error),
        });

        return passportId;
    };

    const getUsers = async () => {
        let users;
        await $.ajax({
            url: `${BASE_URL}Report/GetUsers`,
            data: {},
            type: "POST",
            dataType: "json",
            success: (data) => (users = data),
            error: (error) => console.error(error),
        });

        return users;
    };

    const getSelectorUniversalOptions = async (paramType) => {
        let paramOptions;

        await $.ajax({
            url: `${BASE_URL}Report/GetSelectorUniversalParamOptions`,
            data: { paramType, isEmergencyReport: currentReportData.IsEmergencyReport },
            type: "POST",
            dataType: "json",
            success: (data) => (paramOptions = data),
            error: (error) => console.error(error),
        });

        return paramOptions;
    };

    const saveReportInfo = async (reportObj, flag = "report") => {
        if (flag != 'report') {
            reportObj.LayoutXMLBinary = '';
            reportObj.LayoutPreviewXMLBinary = '';
        }

        const reportData = JSON.stringify(utils.viewFieldsToString(reportObj));
        let saved;

        await $.ajax({
            url: `${BASE_URL}Report/SaveReportInfo`,
            data: { reportData, flag },
            type: "POST",
            dataType: "text",
            success: (data) => (saved = data),
            error: (error) => console.error(error),
        });

        return saved === "True";
    };

    const removeReport = async (reportId) => {
        let response;

        await $.ajax({
            url: `${BASE_URL}Report/DeleteReportLayout`,
            data: { reportId },
            type: "POST",
            dataType: "text",
            success: (data) => (response = data),
            error: (error) => console.error(error),
        });

        response = response === "False" ? false : true;

        return response;
    };

    const getViewTemplate = async (templateName = "editReportViewers", idreport = currentReportData.Id) => {
        let template;
        await $.ajax({
            url: `${BASE_URL}Report/GetViewTemplate`,
            data: { templateName, idreport },
            type: "POST",
            dataType: "html",
            success: (data) => (template = data),
            error: (error) => console.error(error),
        });

        return template;
    };

    const populator = {
        singleCard: (card, reportData) => {
            const elExecution = card.querySelector("li.lastExecution span");
            let elDescription = card.querySelector("span.descriptionText");
            let elCategories = card.querySelector("span.categoryText");
            const dwnldBtn = card.querySelector(".fa.fa-download");

            if (!elDescription && reportData.Description) {
                const div = document.createElement("div");
                elDescription = document.createElement("span");

                div.classList.add("reportDescription");
                elDescription.classList.add("descriptionText");
                div.appendChild(elDescription);
                card.querySelector(".reportCardInfo").appendChild(div);
            }

            if (!elCategories && reportData.CategoriesString) {
                const div = document.createElement("div");
                elCategories = document.createElement("span");

                div.classList.add("reportCategories");
                elCategories.classList.add("categoryText");
                div.appendChild(elCategories);
                card.querySelector(".reportCardInfo").appendChild(div);
            }

            if (reportData.LastExecution) {
                elExecution.parentNode.classList.add("reportExecution", "executiuonIsFine");
                elExecution.innerText = utils.getStringDate(new Date(reportData.LastExecution.ExecutionDate), true);

                if (!dwnldBtn) {
                    const dwnldBtn = utils.iElementFontAwesome("fa fa-download");
                    dwnldBtn.classList.add("active");
                    card
                        .querySelector("li.lastExecution")
                        .insertBefore(dwnldBtn, elExecution);
                    dwnldBtn.addEventListener("click", handleExecutionDownload, false);
                }
            }
            else {
                elExecution.innerText = SIN_EJECUCIONES;
                dwnldBtn && dwnldBtn.remove();
            }

            if (elDescription) { elDescription.innerText = reportData.Description.replace(/\n/g, " "); }
            if (elCategories) { elCategories.innerText = reportData.CategoriesString.replace("ǁ", "") || "" }; //pending return empty string from server to avoid front check
        },
        reportActions: async (div, template) => {
            const buttons = await getViewTemplate(template);
            div.innerHTML = buttons;
        },
        editReportViewers: async (div) => {
            const adminUsers = await getUsers().then((users) => users.map((user) => ({ PassportId: user.ID, Name: user.Name })));

            const ul = div.querySelector("ul#viewersList");
            const select = ul.querySelector("datalist#adminusers");

            [...ul.querySelectorAll("li[data-passportid]")].map((li) => li.remove());

            editedReportData.Viewers.map((viewer) => {
                const li = document.createElement("li");

                li.setAttribute("data-passportid", viewer.PassportId);
                li.innerText = viewer.Name;
                li.append(utils.iElementFontAwesome("fa fa-minus-circle"));
                ul.append(li);
                li.addEventListener("click", () => editor.removeViewer(viewer.PassportId), false);
            });

            select.innerHTML = "";
            adminUsers.map((user) => {
                const option = document.createElement("option");

                option.value = user.Name;
                option.setAttribute("data-userid", user.ID);

                select.append(option);
            });

            ul.append(select);
            ul.querySelector("i#addViewerBtn").addEventListener("click", editor.addViewer, false);
        },
        editReportDescription: (div) => {
            const textarea = div.querySelector("textarea#descriptionEdition");
            textarea.value = editedReportData.Description;
            textarea.addEventListener("change", () => editor.description(textarea.value), false);
        },
        reportLayoutInfo: (reportData, containerDiv) => {
            const title = containerDiv.querySelector("p span#reportTitle");
            const description = containerDiv.querySelector("p span#reportDescription");
            const creationDate = containerDiv.querySelector("p span#reportCreationDate");
            const creationAuthor = containerDiv.querySelector("p span#reportCreationAuthor");
            const categoriesEditable = containerDiv.querySelector(".reportCategories .categoryTextEditable");
            const categories = containerDiv.querySelector(".reportCategories .categoryText");
            const level = containerDiv.querySelector(".reportCategories .levelText");

            const dateCreation = new Date(reportData.CreationDate);

            title.innerText = reportData.DisplayName || "";
            description.innerText = reportData.Description;

            creationDate.innerText = `${dateCreation.getDate()
                }-${dateCreation.getMonth() + 1
                }-${dateCreation.getFullYear()} ${dateCreation.getHours()
                }:${(dateCreation.getMinutes() < 10 ? '0' : '')}${dateCreation.getMinutes()
                }`;
            creationAuthor.innerHTML = reportData.CreatorName || "<b style='color:#FF5C35'>Robotics</b>";
            creationAuthor.setAttribute("data-author-id", reportData.CreatorPassportId);

            var idCatReport = reportData.CategoriesList && reportData.CategoriesList.map((cat) => cat.ID).join(", ");
            var CatReport = reportData.CategoriesList && reportData.CategoriesList.map((cat) => cat.Description).join(", ");
            var idCatLevel = reportData.CategoriesList && reportData.CategoriesList.map((cat) => cat.CategoryLevel).join(", ");
            var reportManagerPermissionsByUser = (window.currentReportData.CreatorPassportId === userPassportId);
            var editCategory = reportManagerPermissionsByUser;

            if (categories) {
                if (editCategory) {
                    document.getElementById("listCategoryEditable").style.display = '';
                    document.getElementById("levelCategoryEditable").style.display = '';

                    document.getElementById("listCategory").style.display = 'none';
                    document.getElementById("levelCategory").style.display = 'none';

                    //si tiene varias categorias sólo tenemos en cuenta la primera para el selector:
                    idCatReport = parseInt(idCatReport.split(","));
                    idCatLevel = parseInt(idCatLevel.split(","));

                    populator.listCategories(idCatReport, idCatLevel);
                }
                else {
                    document.getElementById("listCategory").style.display = '';
                    document.getElementById("levelCategory").style.display = '';
                    document.getElementById("listCategoryEditable").style.display = 'none';
                    document.getElementById("levelCategoryEditable").style.display = 'none';

                    categories.innerText = CatReport;
                    level.innerText = idCatLevel;
                }
            }

            /** -----------------listeners listeners listeners listeners------------------- */
        },
        executionsList: (uList, executionsList, containerDiv) => {
            [...uList.querySelectorAll("li")].map((li) => li.remove());

            while (executionsList.length > 3) { executionsList.pop(); }

            if (!!currentReportData) {
                executionsList.map((exec) => {
                    const listItem = document.createElement("li");
                    const span = document.createElement("span");
                    const icon = document.createElement("div");
                    const dateObj = new Date(exec.ExecutionDate);                    

                    const { statusClass, iconClass } = utils.getExecutionStatusClasses(exec.Status);
                    const dwnldBtn = utils.iElementFontAwesome(iconClass);
                    const removeExecBtn = utils.iElementFontAwesome("fa fa-trash");

                    listItem.classList.add("reportExecution", statusClass);
                    span.innerText = utils.getStringDate(dateObj);

                    icon.classList.add("executionIcon");
                    icon.style.backgroundImage = `url("Base/Images/file.png")`;
                    dwnldBtn.setAttribute("data-guid", exec.Guid);

                    removeExecBtn.setAttribute("id", "removeExecBtn");
                    dwnldBtn.setAttribute("id", "downloadExecBtn");

                    listItem.appendChild(removeExecBtn);
                    icon.appendChild(dwnldBtn);
                    listItem.appendChild(icon);
                    listItem.appendChild(span);
                    uList.appendChild(listItem);

                    dwnldBtn.addEventListener("click", handleExecutionDownload, false);
                    removeExecBtn.addEventListener("click", () => { editor.removeExecution(listItem); }, false);
                });
            }

            // add info to executions
            const inProgress = [...containerDiv.querySelectorAll(".reportExecution.executionIsInProgress"),];
            const futures = [...containerDiv.querySelectorAll(".reportExecution.executionIsFuture"),];
            const errors = [...containerDiv.querySelectorAll(".reportExecution.executionIsError"),];
            const rights = [...containerDiv.querySelectorAll(".reportExecution.executiuonIsFine"),];

            inProgress.map((exec) => {
                !exec.querySelector(".executionIcon") && exec
                    .querySelector(".executionIcon")
                    .appendChild(utils.iElementFontAwesome("fa fa-spinner"));
                const btn = exec.querySelector(".fa.fa-spinner");
                btn.style.opacity = ".8";
                btn.style.color = "#FF5C35";
            });
            futures.map((exec) => {
                !exec.querySelector("i") && exec
                    .querySelector("span")
                    .before(utils.iElementFontAwesome("fa fa-hourglass-o"));
            });
            errors.map((exec) => {
                !exec.querySelector(".executionIcon") && exec
                    .querySelector(".executionIcon")
                    .appendChild(utils.iElementFontAwesome("fa fa-times"));
                const btn = exec.querySelector(".fa.fa-times");
                btn.style.opacity = ".8";
                btn.style.color = "tomato";
            });
            rights.map((exec) => {
                !exec.querySelector(".executionIcon") && exec
                    .querySelector(".executionIcon")
                    .appendChild(utils.iElementFontAwesome("fa fa-download"));
                const btn = exec.querySelector(".fa.fa-download");
                btn.setAttribute("id", "downloadExecBtn");
                btn.addEventListener("click", handleExecutionDownload, false);
            });
        },
        categoryTreeReport: (div, categories) => {
            if (DevExpress.ui.dxTreeList.getInstance($(div)) != undefined) {
                //	$(div).dxTreeList("dispose");
            }

            $(div).dxTreeList({
                dataSource: categories.map((cat) => ({ ...cat, ID: cat.ID + 1 })),
                keyExpr: "ID",
                columns: [{ dataField: "Description", caption: "Categorías", },],
                onCellClick: (e) => {
                    //handleMenuCategoryClick(e);
                    //handleBtnCategoriesStyles(true);
                },
                scrolling: {
                    showScrollbar: "Never",
                },
                expandedRowKeys: [1],
                showRowLines: true,
                showBorders: true,
                columnAutoWidth: true,
            });
        },
        categoryTreeMenu: (div, categories) => {
            if (DevExpress.ui.dxTreeList.getInstance($(div)) != undefined) { $(div).dxTreeList("dispose"); }

            $(div).dxTreeList({
                dataSource: categories.map((cat) => ({ ...cat, ID: cat.ID + 1 })),
                keyExpr: "ID",
                columns: [{ dataField: "Description", },],
                onCellClick: function (e) {
                    handleMenuCategoryClick(e.data);
                    handleBtnCategoriesStyles(true);
                },
                scrolling: { showScrollbar: "Never", },
                expandedRowKeys: [1],
                showRowLines: true,
                showBorders: true,
                columnAutoWidth: true,
            });
        },
        listCategories: (catReport, levelReport) => {
            var arrayCategories = JSON.parse(document.querySelector("#reportCategoryTree>p").innerText);
            //[{"ID":0,"Description":"Prevención"},{"ID":1,"Description":"Laboral"},{"ID":2,"Description":"Legal"}]. . .
            $("#listCategoryEditable").dxSelectBox({
                dataSource: new DevExpress.data.ArrayStore({ data: arrayCategories, key: "Description" }),
                width: "200px",
                displayExpr: "Description",//"title",
                valueExpr: "ID",
                value: catReport,//options[0].value,
                readOnly: false,
                visible: true,
                placeholder: "...",
                onValueChanged: function (data) {
                    var reportID = currentReportData.Id;
                    var selectedLevel = $("#levelCategoryEditable").dxSelectBox("instance").option("value");
                    if (selectedLevel === "") {
                        selectedLevel = "1";
                        $("#levelCategoryEditable").dxSelectBox("instance").option("value", 1);
                    }

                    updateReportCategory(currentReportData.Id, data.value, selectedLevel);
                },
            });
            $("#levelCategoryEditable").dxSelectBox({
                dataSource: new DevExpress.data.ArrayStore({ data: levelsCategory, key: "Description" }),
                width: "200px",
                displayExpr: "Description",
                valueExpr: "value",
                value: levelReport,
                readOnly: false,
                visible: true,
                placeholder: "...",
                onValueChanged: function (data) {
                    var reportID = currentReportData.Id;
                    var selectedCategory = $("#listCategoryEditable").dxSelectBox("instance").option("value");
                    if (selectedCategory !== "") {
                        updateReportCategory(currentReportData.Id, selectedCategory, data.value);
                    }
                },
            });
        },
        lastExecutions: (dates) => {
            dates.map((date) => {
                const status = parseInt(date.getAttribute("data-status"), 10);
                const prevDownload = date.querySelector("i");
                const { statusClass, iconClass } = utils.getExecutionStatusClasses(status);
                if (!!iconClass && !!statusClass) {
                    const dwnldBtn = utils.iElementFontAwesome(iconClass);

                    dwnldBtn.addEventListener("click", handleExecutionDownload, false);
                    date.classList.add("reportExecution", statusClass);

                    !!prevDownload && prevDownload.remove();
                    date.insertBefore(dwnldBtn, date.querySelector("span"));
                }
            });
        },
        scrollTransparencyLayer: (div) => {
            const transparentLayer = document.createElement("div");
            const parent = div.parentNode;

            transparentLayer.classList.add("scrollPanelLayer");
            parent.appendChild(transparentLayer);
            parent.style.verticalAlign = "top";
            parent.style.position = "relative";

            return transparentLayer;
        },
        blurAllTabs: () => { [...mainPanelViews].map((pan) => (pan.style.display = "none")); },
        blurAnyReportCard: (cards) => { cards.map((card) => card.classList.remove("reportCardClicked")); },
        selectorEmployees: () => {
            //take params from editedReportData
            const param = editedReportData.ParametersList.find((prm) => prm.TemplateName === "selectorEmployees").Value;
        },
        selectorDate: () => {
            const whatEdition = document
                .querySelector("#editView > form")
                .getAttribute("id");
            const form = document.querySelector("#fieldsEditor #" + whatEdition);
            $("#parameterDate").dxDateBox({
                value: new Date(),
                onValueChanged: function (e) {
                    let checked = form.querySelector("input[type=radio]:checked").value;
                    if (checked == 0 || checked == '0') collectParamValues();
                }
            }).dxDateBox("instance");
        },
        selectorPeriodTime: () => {
            const whatEdition = document
                .querySelector("#editView > form")
                .getAttribute("id");
            const form = document.querySelector("#fieldsEditor #" + whatEdition);
            const desdeDateTime = $("#desdeDateTime").dxDateBox({
                value: new Date(),
                displayFormat: localFormat,
                onValueChanged: function (e) {
                    hastaDateTime.option("min", e.value);
                    let checked = form.querySelector("input[type=radio]:checked").value;
                    if (checked == 0 || checked == '0') collectParamValues();
                }
            }).dxDateBox("instance");
            const hastaDateTime = $("#hastaDateTime").dxDateBox({
                value: new Date(),
                displayFormat: localFormat,
                onValueChanged: function (e) {
                    let checked = form.querySelector("input[type=radio]:checked").value;
                    if (checked == 0 || checked == '0') collectParamValues();
                }
            }).dxDateBox("instance");
            const param = editedReportData.ParametersList.find((prm) => prm.TemplateName === "selectorPeriodTime").Value;

            const pSplit = param.split(",");
            const initDateInput = $("#desdeDateTime").dxDateBox("instance");
            const finalDateInput = $("#hastaDateTime").dxDateBox("instance");

            if (pSplit.length != 3) {
                const selectId = 2;
                const selectable = selectId ? document.querySelector(`#selectorPeriodTime input[value='${selectId}']`) : null;
                const { initDate, finalDate } = editor.selectorDatetime(parseInt(selectId, 10), initDateInput, finalDateInput);

                selectable && selectable.setAttribute("checked", "");
            } else {
                const selectId = pSplit[0];
                const selectable = selectId ? document.querySelector(`#selectorPeriodTime input[value='${selectId}']`) : null;

                initDateInput.value = window.parent.moment(pSplit[1]).format("YYYY-MM-DD");
                finalDateInput.value = window.parent.moment(pSplit[2]).format("YYYY-MM-DD");

                const { initDate, finalDate } = editor.selectorDatetime(parseInt(selectId, 10), initDateInput, finalDateInput);

                selectable && selectable.setAttribute("checked", "");
            }
        },
        selectorString: () => { document.querySelector("input#parameterString").value = editedReportData.ParametersList.find((prm) => prm.TemplateName === "selectorString").Value; },
        selectorNumber: () => { document.querySelector("input#parameterNumber").value = editedReportData.ParametersList.find((prm) => prm.TemplateName === "selectorNumber").Value; },
        selectorFormat: async (container) => {
            $("#viewFormat").text(REPORT_PARAMETER_FORMAT);

            var selectOldValues = function () // set values components from values html-input-hidden
            {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if ((planificationsTab.classList.contains("activeTab")))//PLANIFICATION
                {
                    if ((typeof paramSelected.Value === undefined)// plan parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === "")))) {
                        var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                        var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                        if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                        {
                            //control en el caso que los parametros seleccionados en el report hayan cambiado y no esten en la ultima planificación lanzada
                            paramsLastExecution = JSON.parse(currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                            if (paramsLastExecution !== undefined) {
                                paramSelected = paramsLastExecution.Value;
                            }
                        }
                    }
                }
                else//REPORT
                {
                    if (((typeof paramSelected.Value === undefined)// last report parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === ""))))
                        && (currentReportData.LastParameters !== undefined)) {
                        paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                        if (paramsLastExecution !== undefined) {
                            paramSelected = paramsLastExecution.Value;
                        }
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }

                var format = isNaN(paramSelected.format) ? 0 : parseInt(paramSelected.format, 10);

                if ((planificationsTab.classList.contains("activeTab"))) {
                    format = $("#plannerFormat").is(":empty")
                        ? 0
                        : $("#plannerFormat").dxSelectBox("instance");
                }

                $("#typeDocReport").dxSelectBox("instance").option("value", format);
                collectParamValues("format", format, null);
            }

            $("#typeDocReport").dxSelectBox({
                width: "30%",
                dataSource: new DevExpress.data.DataSource({ store: exportFormats, type: 'array', key: "value" }),
                valueExpr: "value",
                displayExpr: "title",
                readOnly: false,
                placeholder: SELECT,
                onValueChanged: function (data) {
                    collectParamValues("format", data.value, null);
                },
                onInitialized: function (data) {
                    selectOldValues();
                },
            });//-------------------------------
        },
        selectorViewFormat: async (container) => {
            $("#viewCheckSelection").text(DISPLAY);
            //$("#viewFormat").text(FORMAT);

            var selectOldValues = function () // set values components from values html-input-hidden
            {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if ((planificationsTab.classList.contains("activeTab")))//PLANIFICATION
                {
                    if ((typeof paramSelected.Value === undefined)// plan parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === "")))) {
                        var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                        var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                        if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                        {
                            //control en el caso que los parametros seleccionados en el report hayan cambiado y no esten en la ultima planificación lanzada
                            paramsLastExecution = JSON.parse(currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                            if (paramsLastExecution !== undefined) {
                                paramSelected = paramsLastExecution.Value;
                            }
                        }
                    }
                }
                else//REPORT
                {
                    if (((typeof paramSelected.Value === undefined)// last report parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === ""))))
                        && (currentReportData.LastParameters !== undefined)) {
                        paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                        if (paramsLastExecution !== undefined) {
                            paramSelected = paramsLastExecution.Value;
                        }
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }

                var detail = paramSelected.ckDetail;
                var total = paramSelected.ckTotal;
                var chart = paramSelected.ckShowChart;

                if (detail !== true) { detail = false; }
                $("#ckAllDetails").dxCheckBox("instance").option("value", detail);

                if (total !== true) { total = false; }
                $("#ckTotals").dxCheckBox("instance").option("value", total);

                if (chart !== true) { chart = false; }
                $("#ckShowChart").dxCheckBox("instance").option("value", chart);

            }
            $("#ckAllDetails").dxCheckBox({
                width: 400,
                text: REPORT_PARAMETER_DETAIL,
                onValueChanged: function (data) {
                    collectParamValues("detail", data.value, null);
                }
            });//--------------------------------
            $("#ckTotals").dxCheckBox({
                width: 400,
                text: TOTALS,
                onValueChanged: function (data) {
                    collectParamValues("total", data.value, null);
                }
            });//-------------------------------------

            $("#ckShowChart").dxCheckBox({
                width: 400,
                text: REPORT_PARAMETER_SHOWCHART,
                visible: currentReportData.Name === "Saldos con detalle",
                onValueChanged: function (data) {
                    collectParamValues("chart", data.value, null);
                },
                onInitialized: function (data) {
                    setTimeout(function () {
                        selectOldValues();
                    }, 100);

                }
            });//-------------------------------------
        },
        selectorAccessType: async (container) => {
            console.log("entra selectorAccessType");
            $("#viewCheckSelection").text(ACCESSTYPE);
            //$("#viewFormat").text(FORMAT);

            var selectOldValues = function () // set values components from values html-input-hidden
            {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if ((planificationsTab.classList.contains("activeTab")))//PLANIFICATION
                {
                    if ((typeof paramSelected.Value === undefined)// plan parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === "")))) {
                        var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                        var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                        if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                        {
                            //control en el caso que los parametros seleccionados en el report hayan cambiado y no esten en la ultima planificación lanzada
                            paramsLastExecution = JSON.parse(currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                            if (paramsLastExecution !== undefined) {
                                paramSelected = paramsLastExecution.Value;
                            }
                        }
                    }
                }
                else//REPORT
                {
                    if (((typeof paramSelected.Value === undefined)// last report parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === ""))))
                        && (currentReportData.LastParameters !== undefined)) {
                        paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                        if (paramsLastExecution !== undefined) {
                            paramSelected = paramsLastExecution.Value;
                        }
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }

                var accessValids = paramSelected.ckValids;
                var accessInvalids = paramSelected.ckInvalids;

                if (accessValids !== true) { accessValids = false; }
                $("#ckValids").dxCheckBox("instance").option("value", accessValids);

                if (accessInvalids !== true) { accessInvalids = false; }
                $("#ckInvalids").dxCheckBox("instance").option("value", accessInvalids);
            }
            $("#ckValids").dxCheckBox({
                width: 300,
                text: REPORT_PARAMETER_VALID_ACCESS,
                onValueChanged: function (data) {
                    collectParamValues("valids", data.value, null);
                }
                //todo: onInitialized: valor por defecto -> true

            });//--------------------------------
            $("#ckInvalids").dxCheckBox({
                width: 300,
                text: REPORT_PARAMETER_INVALID_ACCESS,
                onValueChanged: function (data) {
                    collectParamValues("invalids", data.value, null);
                },
                onInitialized: function (data) {
                    selectOldValues();
                }
            });//-------------------------------------
        },
        selectorProfileTypes: async (container) => {
            //$("#viewCheckSelection").text(DISPLAY);
            //$("#viewFormat").text(FORMAT);
            const hideShowSelectorsByProfileType = (selectedProfile) => {
                $("#selectorProfileTypes .selectores > div").hide();
                $("#selectorProfileTypes .comboHelperText").hide();
                if (selectedProfile == 1) $("#selectConcepts").show();
                if (selectedProfile == 2 || selectedProfile == 4) $("#selectCauses").show();
                if (selectedProfile == 3 || selectedProfile == 4) $("#selectIncidences").show();
            }

            var selectOldValues = function (profileType = null) // set values components from values html-input-hidden
            {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if ((planificationsTab.classList.contains("activeTab")))//PLANIFICATION
                {
                    if ((typeof paramSelected.Value === undefined)// plan parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === "")))) {
                        var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                        var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                        if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                        {
                            //control en el caso que los parametros seleccionados en el report hayan cambiado y no esten en la ultima planificación lanzada
                            paramsLastExecution = JSON.parse(currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                            if (paramsLastExecution !== undefined) {
                                paramSelected = paramsLastExecution.Value;
                            }
                        }
                    }
                }
                else//REPORT
                {
                    if (((typeof paramSelected.Value === undefined)// last report parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === ""))))
                        && (currentReportData.LastParameters !== undefined)) {
                        paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                        if (paramsLastExecution !== undefined) {
                            paramSelected = paramsLastExecution.Value;
                        }
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }

                if (profileType) {  //es un selector de concepts, causes o incidences
                    switch (profileType) {
                        case "selectConcepts":
                            if (typeof (paramSelected.selectConcepts) != "undefined") {
                                if ($("#selectConcepts") && $("#selectConcepts").dxTagBox("instance")) {
                                    let selectedValues;

                                    if (Array.isArray(paramSelected.selectConcepts) &&
                                        paramSelected.selectConcepts.length > 0 &&
                                        typeof paramSelected.selectConcepts[0] === 'object') {
                                        selectedValues = paramSelected.selectConcepts.map(item => item.value);
                                    } else {
                                        selectedValues = paramSelected.selectConcepts;
                                    }
                                    $("#selectConcepts").dxTagBox("instance").option("value", selectedValues);
                                }
                            }
                            break;
                        case "selectCauses":
                            if (typeof (paramSelected.selectCauses) != "undefined") {
                                if ($("#selectCauses") && $("#selectCauses").dxTagBox("instance")) {
                                    let selectedValues;

                                    if (Array.isArray(paramSelected.selectCauses) &&
                                        paramSelected.selectCauses.length > 0 &&
                                        typeof paramSelected.selectCauses[0] === 'object') {
                                        selectedValues = paramSelected.selectCauses.map(item => item.value);
                                    } else {
                                        selectedValues = paramSelected.selectCauses;
                                    }
                                    $("#selectCauses").dxTagBox("instance").option("value", selectedValues);
                                }
                            }
                            break;
                        case "selectIncidences":
                            if (typeof (paramSelected.selectIncidences) != "undefined") {
                                if ($("#selectIncidences") && $("#selectIncidences").dxTagBox("instance")) {
                                    let selectedValues;

                                    if (Array.isArray(paramSelected.selectIncidences) &&
                                        paramSelected.selectIncidences.length > 0 &&
                                        typeof paramSelected.selectIncidences[0] === 'object') {
                                        selectedValues = paramSelected.selectIncidences.map(item => item.value);
                                    } else {
                                        selectedValues = paramSelected.selectIncidences;
                                    }
                                    $("#selectIncidences").dxTagBox("instance").option("value", selectedValues);
                                }
                            }
                            break;
                        case "criterio":
                            if (typeof (paramSelected.criterio) != "undefined") {
                                $("#criterio").dxSelectBox("instance").option("value", paramSelected.criterio);
                            } else {
                                $("#criterio").dxSelectBox("instance").option("value", criterioData[0].value);
                            }
                            break;
                        case "tipoValorCriterio":
                            if (typeof (paramSelected.tipoValorCriterio) != "undefined") {
                                $("#tipoValorCriterio").dxSelectBox("instance").option("value", paramSelected.tipoValorCriterio);
                            } else {
                                $("#tipoValorCriterio").dxSelectBox("instance").option("value", tipoValorCriterioData[0].value);
                            }
                            break;
                        case "valorCriterio":
                            if (typeof (paramSelected.valorCriterio) != 'undefined') {
                                if (paramSelected.valorCriterio.length == 4) {
                                    $("#valorCriterio").dxTextBox("instance").option("value", paramSelected.valorCriterio.substring(0, 2) + ":" + paramSelected.valorCriterio.substring(2, 4));
                                    valorCriterioStr = paramSelected.valorCriterio.substring(0, 2) + ":" + paramSelected.valorCriterio.substring(2, 4);
                                }
                                else {
                                    $("#valorCriterio").dxTextBox("instance").option("value", paramSelected.valorCriterio.substring(0, 3) + ":" + paramSelected.valorCriterio.substring(3, 5));
                                    valorCriterioStr = paramSelected.valorCriterio.substring(0, 3) + ":" + paramSelected.valorCriterio.substring(3, 5);
                                }
                                valorCriterio = paramSelected.valorCriterio
                            }
                            else {
                                valorCriterio = "00000";
                                valorCriterioStr = "000:00";
                                $("#valorCriterio").dxTextBox("instance").option("value", valorCriterioStr);
                            }
                            break;
                        case "valorFichaCriterio":
                            if (typeof (paramSelected.valorFichaCriterio) != "undefined") {
                                $("#valorFichaCriterio").dxSelectBox("instance").option("value", paramSelected.valorFichaCriterio);
                            } else {
                                $("#valorFichaCriterio").dxSelectBox("instance").option("value", valorFichaCriterioData[0]?.value);
                            }
                            break;
                        case "rangoCriterio":
                            if (typeof (paramSelected.rangoCriterio) != "undefined") {
                                if (typeof paramSelected.rangoCriterio === 'object') {
                                    $("#rangoCriterio").dxSelectBox("instance").option("value", paramSelected.rangoCriterio?.value);
                                } else {
                                    $("#rangoCriterio").dxSelectBox("instance").option("value", paramSelected.rangoCriterio);
                                }
                            } else {
                                if (typeof (rangoData[0]) === 'object') {
                                    $("#rangoCriterio").dxSelectBox("instance").option("value", rangoData[0]?.value);
                                } else {
                                    $("#rangoCriterio").dxSelectBox("instance").option("value", rangoData[0]);
                                }

                            }
                            //collectParamValues(null, null, null);
                            break;
                        default:
                            break;
                    }
                } else { //actualizamos el filtro de perfil
                    let selectedProfile = paramSelected.profileTypes;
                    if (selectedProfile !== 1 && selectedProfile !== 2 && selectedProfile !== 3 && selectedProfile !== 4) { selectedProfile = 1; }
                    $("#profileTypes").dxRadioGroup("instance").option("value", selectedProfile);
                    updateMaxLabels(selectedProfile);
                    profileTypeSelected = selectedProfile;

                    //mostramos el selector seleccionado y ocultamos el resto
                    hideShowSelectorsByProfileType(selectedProfile);
                }
            }

            const conceptsMaxItems = 6;
            let conceptsSuspendValueChagned;
            const CAIMAXITEMS = 6;
            let causesAndIncidencesMaxItems = CAIMAXITEMS;
            let causesAndIncidencesSuspendValueChagned;
            let profileTypeSelected = 0;

            const changeSelectorValues = function (type, data) {
                const selectedIDs = data.map((e) => e.value).join(",");
                collectParamValues(type, selectedIDs);
            }

            const updateSelectors = (selectedProfile) => {
                //mostramos el selector seleccionado y ocultamos el resto
                hideShowSelectorsByProfileType(selectedProfile);
                updateMaxLabels(selectedProfile);
                profileTypeSelected = selectedProfile;

                //Si selecciona Incidencias&Justificaciones, mostramos label del selector
                if (selectedProfile == 4) {
                    $("#selectorProfileTypes .comboHelperText").show();
                    //Limitamos a 1 las opciones del selector                    
                    auxVal = ($("#selectIncidences")) ? $("#selectIncidences").dxTagBox("instance").option("value") : "";
                    if (auxVal !== undefined ) {
                        $("#selectIncidences").dxTagBox("instance").option("value", auxVal);
                    }
                }
            }
            const profileTypesData = [
                { title: SALDOS, value: 1 },
                { title: INCIDENCIAS, value: 3 },
                { title: JUSTIFICACIONES, value: 2 },
                { title: INCIDENCIASYJUSTIFICACIONES, value: 4 }
            ]
            const criterioData = [
                { title: SUPERA, value: "mayor" },
                { title: NOSUPERA, value: "menor" }
            ]
            const tipoValorCriterioData = [
                { title: ELVALOR, value: "valor" }, //Todo translate
                { title: ELVALORDELCAMPO, value: "campo" }
            ]
            valorFichaCriterioData = await getSelectorUniversalOptions("Robotics.Base.userFieldsNumberSelector");
            const rangoData = [
                { text: DIARIAMENTE_TEXT, value: "diariamente" },
                { text: ENPERIODOSELECCIONADO_TEXT, value: "en el periodo seleccionado" },
            ];
            const conceptsData = await getSelectorUniversalOptions("Robotics.Base.conceptsSelector");
            const causesData = await getSelectorUniversalOptions("Robotics.Base.causesSelector");
            const incidencesData = await getSelectorUniversalOptions("Robotics.Base.incidencesSelector");
            $("#profileTypes").dxRadioGroup({
                dataSource: profileTypesData,
                valueExpr: (data) => data.value,
                displayExpr: "title",
                activeStateEnabled: true,
                focusStateEnabled: true,
                hoverStateEnabled: true,
                text: REPORT_PARAMETER_DETAIL,
                onValueChanged: function (data) {
                    collectParamValues("profileTypes", data.value, null);
                    //mostramos el selector seleccionado y ocultamos el resto
                    updateSelectors(data.value);
                },
                onInitialized: function (data) {
                    selectOldValues();
                }
            });

            function updateMaxLabels(selectedProfile) {
                if (originalMaxValueText === undefined) {
                    originalMaxValueText = $("#selectorProfileTypes > div.selectores > p").html();
                }
                $("#selectorProfileTypes > div.selectores > p").text(originalMaxValueText);
                switch (selectedProfile) {
                    case 1:
                        //Saldo
                        $("#selectorProfileTypes > div.selectores > p").append("<strong> " + MAXSALDOS + ".</strong>");
                        break;
                    case 2:
                        //Justificaciones
                        $("#selectorProfileTypes > div.selectores > p");
                        break;
                    case 3:
                        //Incidencias
                        $("#selectorProfileTypes > div.selectores > p");
                        break;
                    case 4:
                        //Incidencias&Justificaciones
                        $("#selectorProfileTypes > div.selectores > p");
                        break;
                    default:
                        break;
                }
            }

            $("#selectConcepts").dxTagBox({
                items: conceptsData,
                displayExpr: "title",
                valueExpr: "value",
                placeholder: SELECCIONASALDOS,
                showSelectionControls: true,
                applyValueMode: 'useButtons',
                searchEnabled: true,
                searchExpr: "title",
                searchMode: "contains",
                searchTimeout: 200,
                minSearchLength: 0,
                onValueChanged: function (e) {

                    if (e.value.length > conceptsMaxItems) {
                        conceptsSuspendValueChagned = true;

                        const limitValues = e.value.slice(0, conceptsMaxItems);
                        e.component.option('value', limitValues);
                        changeSelectorValues("selectConcepts", limitValues);
                    } else {
                        changeSelectorValues("selectConcepts", e.value);
                    }
                },
                onInitialized: function () {
                    selectOldValues('selectConcepts');
                }
            });

            $("#selectCauses").dxTagBox({
                items: causesData,
                displayExpr: "title",
                valueExpr: "value",
                placeholder: SELECCIONACONCEPTS,
                showSelectionControls: true,
                applyValueMode: 'useButtons',
                searchEnabled: true,
                searchExpr: "title",
                searchMode: "contains",
                searchTimeout: 200,
                minSearchLength: 0,
                onValueChanged: function (e) {
                    if (causesAndIncidencesSuspendValueChagned) {
                        causesAndIncidencesSuspendValueChagned = false;
                        return;
                    }

                    changeSelectorValues("selectCauses", e.value);
                },
                onInitialized: function () {
                    selectOldValues('selectCauses');
                }
            });
            $("#selectIncidences").dxTagBox({
                items: incidencesData,
                displayExpr: "title",
                valueExpr: "value",
                placeholder: SELECCIONAINCIDENCIAS,
                showSelectionControls: true,
                applyValueMode: 'useButtons',
                searchEnabled: true,
                searchExpr: "title",
                searchMode: "contains",
                searchTimeout: 200,
                minSearchLength: 0,
                onValueChanged: function (e) {
                    if (causesAndIncidencesSuspendValueChagned) {
                        causesAndIncidencesSuspendValueChagned = false;
                        return;
                    }


                    changeSelectorValues("selectIncidences", e.value);

                },
                onInitialized: function () {
                    selectOldValues('selectIncidences');
                    updateSelectors(profileTypeSelected);
                }
            });
            $("#criterio").dxSelectBox({
                items: criterioData,
                displayExpr: 'title',
                valueExpr: 'value',
                width: '150px',
                dropDownOptions: {
                    minWidth: 150,
                },
                onValueChanged: function (data) {
                    collectParamValues("criterio", data.value, null);
                },
                onInitialized: function () {
                    selectOldValues('criterio');
                }
            });
            $('#valorCriterio').dxTextBox({
                value: '00000',
                onValueChanged: function (data) {
                    valorCriterioStr = data.value;
                    data.value = data.value.replace(":", "");
                    if (data.value.length == 4)
                        data.value = "0" + data.value;
                    valorCriterio = data.value;
                    collectParamValues("valorCriterio", data.value, null);
                },
                onInitialized: function () {
                    selectOldValues('valorCriterio');
                }
            }).dxValidator({
                validationRules: [{
                    type: "pattern",
                    pattern: /^(\d{2,3}):(\d{2})$/,
                    message: MSG_VALIDATION_CRITERIA_DISPLAY
                }]
            });
            $("#valorFichaCriterio").dxSelectBox({
                items: valorFichaCriterioData,
                displayExpr: 'title',
                valueExpr: 'value',
                width: '270px',
                dropDownOptions: {
                    minWidth: 270,
                },
                onValueChanged: function (data) {
                    collectParamValues("valorFichaCriterio", data.value, null);
                },
                onInitialized: function () {
                    selectOldValues('valorFichaCriterio');
                }
            });
            $("#tipoValorCriterio").dxSelectBox({
                items: tipoValorCriterioData,
                displayExpr: 'title',
                valueExpr: 'value',
                onValueChanged: function (data) {
                    collectParamValues("tipoValorCriterio", data.value, null);
                    //Si es valor, mostramos el selector de valor, si es campo, mostramos el selector de campos
                    if (data.value == "campo") {
                        $("#valorCriterio").dxTextBox("instance").option("visible", false);
                        $("#valorFichaCriterio").dxSelectBox("instance").option("visible", true);
                    }
                    else {
                        $("#valorCriterio").dxTextBox("instance").option("visible", true);
                        $("#valorFichaCriterio").dxSelectBox("instance").option("visible", false);
                    }
                },
                onInitialized: function (data) {
                    selectOldValues('tipoValorCriterio');
                    //Si es valor, mostramos el selector de valor, si es campo, mostramos el selector de campos
                    if ($("#tipoValorCriterio").dxSelectBox("instance").option("value") == "campo") {
                        $("#valorCriterio").dxTextBox("instance").option("visible", false);
                        $("#valorFichaCriterio").dxSelectBox("instance").option("visible", true);
                    }
                    else {
                        $("#valorCriterio").dxTextBox("instance").option("visible", true);
                        $("#valorFichaCriterio").dxSelectBox("instance").option("visible", false);
                    }
                }
            });
            $("#rangoCriterio").dxSelectBox({
                dataSource: rangoData,
                displayExpr: "text",
                valueExpr: "value",
                onValueChanged: function (data) {
                    collectParamValues("rangoCriterio", data.value, null);
                },
                onInitialized: function () {
                    selectOldValues('rangoCriterio');
                }
            });
            setTimeout(function () { collectParamValues(null, null, null); }, 100);

        },
        selectorCausesRegistroJL: async (container) => {
            //mostrar/ocultar selector dependiendo de si quiere ver justif. o no
            const hideShowSelectorCauses = (showCauses) => {
                $("#selectorCausesRegistroJL .selectores > div").hide();
                $("#selectorCausesRegistroJL .comboHelperText").hide();
                if (showCauses == 1) $("#selectCauses").show();
            }

            var selectOldValues = function (showCauses = null) // set values components from values html-input-hidden
            {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if ((planificationsTab.classList.contains("activeTab")))//PLANIFICATION
                {
                    if ((typeof paramSelected.Value === undefined)// plan parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === "")))) {
                        var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                        var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                        if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                        {
                            //control en el caso que los parametros seleccionados en el report hayan cambiado y no esten en la ultima planificación lanzada
                            paramsLastExecution = JSON.parse(currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                            if (paramsLastExecution !== undefined) {
                                paramSelected = paramsLastExecution.Value;
                            }
                        }
                    }
                }
                else//REPORT
                {
                    if (((typeof paramSelected.Value === undefined)// last report parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === ""))))
                        && (currentReportData.LastParameters !== undefined)) {
                        paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                        if (paramsLastExecution !== undefined) {
                            paramSelected = paramsLastExecution.Value;
                        }
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }

                if (showCauses) {  //es un selector de causes
                    switch (showCauses) {
                        case "selectCauses":
                            if (typeof (paramSelected.selectCauses) != undefined && paramSelected.showCauses == 1) {
                                if ($("#selectCauses") && $("#selectCauses").dxTagBox("instance")) {
                                    $("#selectCauses").dxTagBox("instance").option("value", paramSelected.selectCauses);
                                    if (typeof paramSelected.selectCauses != 'undefined')
                                        changeSelectorValues("selectCauses", paramSelected.selectCauses);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                } else { //actualizamos el valor del selector
                    let showCauses = paramSelected.showCauses;
                    if (showCauses !== 1 && showCauses !== 0) { showCauses = 0; }
                    $("#showCauses").dxRadioGroup("instance").option("value", showCauses);
                    //mostramos el selector seleccionado y ocultamos el resto
                    hideShowSelectorCauses(showCauses);
                }
            }

            const changeSelectorValues = function (type, data) {
                const selectedIDs = data.map((e) => e.value).join(",");
                collectParamValues(type, selectedIDs);
            }

            const updateSelectors = (showCauses) => {
                //mostramos el selector seleccionado y ocultamos el resto
                hideShowSelectorCauses(showCauses);
            }
            const showCausesData = [
                { title: NO, value: 0 },
                { title: SI, value: 1 },
            ]

            const causesData = await getSelectorUniversalOptions("Robotics.Base.causesSelector");
            $("#showCauses").dxRadioGroup({
                dataSource: showCausesData,
                valueExpr: (data) => data.value,
                displayExpr: "title",
                activeStateEnabled: true,
                focusStateEnabled: true,
                hoverStateEnabled: true,
                text: REPORT_PARAMETER_DETAIL,
                onValueChanged: function (data) {
                    collectParamValues("showCauses", data.value, null);
                    //mostramos el selector seleccionado y ocultamos el resto
                    updateSelectors(data.value);
                },
                onInitialized: function (data) {
                    selectOldValues();
                }
            });

            $("#selectCauses").dxTagBox({
                items: causesData,
                displayExpr: "title",
                valueExpr: "value",
                placeholder: SELECCIONACONCEPTS,
                showSelectionControls: true,
                applyValueMode: 'useButtons',
                searchEnabled: true,
                searchExpr: "title",
                searchMode: "contains",
                searchTimeout: 200,
                minSearchLength: 0,
                onValueChanged: function (e) {
                    changeSelectorValues("selectCauses", e.value);
                },
                onInitialized: function () {
                    selectOldValues('selectCauses');
                }
            });
        },
        selectorConceptsRegistroJL: async (container) => {
            //mostrar/ocultar selector dependiendo de si quiere ver justif. o no
            const hideShowSelectorConcepts = (showConcepts) => {
                $("#selectorConceptsRegistroJL .selectores > div").hide();
                $("#selectorConceptsRegistroJL .comboHelperText").hide();
                if (showConcepts == 1) $("#selectConcepts").show();
            }

            var selectOldValues = function (showConcepts = null) // set values components from values html-input-hidden
            {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if ((planificationsTab.classList.contains("activeTab")))//PLANIFICATION
                {
                    if ((typeof paramSelected.Value === undefined)// plan parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === "")))) {
                        var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                        var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                        if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                        {
                            //control en el caso que los parametros seleccionados en el report hayan cambiado y no esten en la ultima planificación lanzada
                            paramsLastExecution = JSON.parse(currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                            if (paramsLastExecution !== undefined) {
                                paramSelected = paramsLastExecution.Value;
                            }
                        }
                    }
                }
                else//REPORT
                {
                    if (((typeof paramSelected.Value === undefined)// last report parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === ""))))
                        && (currentReportData.LastParameters !== undefined)) {
                        paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                        if (paramsLastExecution !== undefined) {
                            paramSelected = paramsLastExecution.Value;
                        }
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }

                if (showConcepts) {  //es un selector de concepts
                    switch (showConcepts) {
                        case "selectConcepts":
                            if (typeof (paramSelected.selectConcepts) != undefined && paramSelected.showConcepts == 1) {
                                if ($("#selectConcepts") && $("#selectConcepts").dxTagBox("instance")) {
                                    let selectedValues;

                                    if (Array.isArray(paramSelected.selectConcepts) &&
                                        paramSelected.selectConcepts.length > 0 &&
                                        typeof paramSelected.selectConcepts[0] === 'object') {
                                        selectedValues = paramSelected.selectConcepts.map(item => item.value);
                                    } else {
                                        selectedValues = paramSelected.selectConcepts;
                                    }

                                    $("#selectConcepts").dxTagBox("instance").option("value", selectedValues);
                                    if (typeof paramSelected.selectConcepts != 'undefined')
                                        changeSelectorValues("selectConcepts", selectedValues);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                } else { //actualizamos el valor del selector
                    let showConcepts = paramSelected.showConcepts;
                    if (showConcepts !== 1 && showConcepts !== 0) { showConcepts = 0; }
                    $("#showConcepts").dxRadioGroup("instance").option("value", showConcepts);

                    //mostramos el selector seleccionado y ocultamos el resto
                    hideShowSelectorConcepts(showConcepts);
                }
            }

            const changeSelectorValues = function (type, data) {
                const selectedIDs = data.map((e) => e.value).join(",");
                collectParamValues(type, selectedIDs);
            }

            const updateSelectors = (showConcepts) => {
                //mostramos el selector seleccionado y ocultamos el resto
                hideShowSelectorConcepts(showConcepts);
            }
            const showConceptsData = [
                { title: NO, value: 0 },
                { title: SI, value: 1 },
            ]

            const conceptsData = await getSelectorUniversalOptions("Robotics.Base.conceptsSelector");  //Revisar esto
            $("#showConcepts").dxRadioGroup({
                dataSource: showConceptsData,
                valueExpr: (data) => data.value,
                displayExpr: "title",
                activeStateEnabled: true,
                focusStateEnabled: true,
                hoverStateEnabled: true,
                text: REPORT_PARAMETER_DETAIL,
                onValueChanged: function (data) {
                    collectParamValues("showConcepts", data.value, null);
                    //mostramos el selector seleccionado y ocultamos el resto
                    updateSelectors(data.value);
                },
                onInitialized: function (data) {
                    selectOldValues();
                }
            });

            $("#selectConcepts").dxTagBox({
                items: conceptsData,
                displayExpr: "title",
                valueExpr: "value",
                placeholder: SELECCIONASALDOS,
                showSelectionControls: true,
                applyValueMode: 'useButtons',
                searchEnabled: true,
                searchExpr: "title",
                searchMode: "contains",
                searchTimeout: 200,
                minSearchLength: 0,
                onValueChanged: function (e) {
                    changeSelectorValues("selectConcepts", e.value);
                },
                onInitialized: function () {
                    selectOldValues('selectConcepts');
                }
            });
        },
        selectorYearAndMonth: (container) => {
            let selectorInitialized = false;
            var selectOldValues = function (tipoSelector = null) // set values components from values html-input-hidden
            {
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === 'selectorYearAndMonth' && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if (((typeof paramSelected.Value === undefined)// last report parameters
                    || ((typeof paramSelected.Value === "string")
                        && ((paramSelected.value === undefined)
                            || (paramSelected.value === ""))))
                    && (currentReportData.LastParameters !== undefined)) {
                    paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                    if (paramsLastExecution !== undefined) {
                        paramSelected = paramsLastExecution.Value;
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }

                if (tipoSelector) {  //es un selector de concepts, causes o incidences
                    let sValue = "";
                    switch (tipoSelector) {
                        case "month":
                            sValue = (typeof (paramSelected.month) != "undefined") ? paramSelected.month : monthData[0].value;
                            break;
                        case "year":
                            sValue = (typeof (paramSelected.year) != "undefined") ? paramSelected.year : yearData[0];
                            break;
                    }
                    $("#" + tipoSelector).dxSelectBox("instance").option("value", sValue);
                }
            }

            const checkDynamicData = (data) => {
                const selectedItem = (data != null) ? data.component.option('selectedItem').title : $("#month").dxSelectBox("instance").option("text");
                const isDynamicSelected = (selectedItem == monthData[0].title || selectedItem == CURRENTMONTH || selectedItem == PREVMONTH || selectedItem == NEXTMONTH
                    || selectedItem == CURRENTMONTHVALUE || selectedItem == PREVMONTHVALUE || selectedItem == NEXTMONTHVALUE);

                if (isDynamicSelected && $("#year").dxSelectBox("instance")) {
                    $("#year").dxSelectBox("instance").option("value", getDynamicYear(currentDate.getFullYear(), selectedItem));
                    $("#year").dxSelectBox("instance").option("disabled", true);
                } else {
                    if ($("#year").dxSelectBox("instance")) $("#year").dxSelectBox("instance").option("disabled", false);
                }
            }

            $("#year").dxSelectBox({
                items: yearData,
                onValueChanged: function (data) {
                    if (selectorInitialized) collectParamValues("year", data.value, null);
                }
            });

            $("#month").dxSelectBox({
                items: monthData,
                displayExpr: 'title',
                valueExpr: 'value',
                onValueChanged: function (data) {
                    if (selectorInitialized) {
                        collectParamValues("month", data.value, null);
                        checkDynamicData(data);
                    }
                }
            });


            setTimeout(function () {
                selectorInitialized = false;
                selectOldValues('year');
                selectOldValues('month');
                checkDynamicData(null);
                selectorInitialized = true;
                collectParamValues(null, null, null);
            }, 100);
        },
        selectorBetweenYearAndMonth: (container) => {
            let selectorInitialized = false;
            var selectOldValues = function (tipoSelector = null) // set values components from values html-input-hidden
            {
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === 'selectorBetweenYearAndMonth' && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if (((typeof paramSelected.Value === undefined)// last report parameters
                    || ((typeof paramSelected.Value === "string")
                        && ((paramSelected.value === undefined)
                            || (paramSelected.value === ""))))
                    && (currentReportData.LastParameters !== undefined)) {
                    paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                    if (paramsLastExecution !== undefined) {
                        paramSelected = paramsLastExecution.Value;
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }

                if (tipoSelector) {  //es un selector de concepts, causes o incidences
                    let sValue = "";
                    switch (tipoSelector) {
                        case "monthStart":
                            sValue = (typeof (paramSelected.monthStart) != "undefined") ? paramSelected.monthStart : monthData[0].value;
                            break;
                        case "yearStart":
                            sValue = (typeof (paramSelected.yearStart) != "undefined") ? paramSelected.yearStart : yearData[0];
                            break;
                        case "monthEnd":
                            sValue = (typeof (paramSelected.monthEnd) != "undefined") ? paramSelected.monthEnd : monthData[1].value;
                            break;
                        case "yearEnd":
                            sValue = (typeof (paramSelected.yearEnd) != "undefined") ? paramSelected.yearEnd : yearData[0];
                            break;
                    }
                    $("#" + tipoSelector).dxSelectBox("instance").option("value", sValue);
                }
            }

            const checkDynamicData = (data, selector) => {
                const selectedItem = (data != null) ? data.component.option('selectedItem').title : $("#month" + selector).dxSelectBox("instance").option("text");
                const isCurrentSelected = (selectedItem == monthData[0].title || selectedItem == CURRENTMONTH || selectedItem == PREVMONTH || selectedItem == NEXTMONTH);

                if (isCurrentSelected && $("#year" + selector).dxSelectBox("instance")) {
                    $("#year" + selector).dxSelectBox("instance").option("value", getDynamicYear(currentDate.getFullYear(), selectedItem));
                    $("#year" + selector).dxSelectBox("instance").option("disabled", true);
                } else {
                    if ($("#year" + selector).dxSelectBox("instance")) $("#year" + selector).dxSelectBox("instance").option("disabled", false);
                }
            }

            $("#yearStart").dxSelectBox({
                items: yearData,
                onValueChanged: function (data) {
                    if (selectorInitialized) collectParamValues("yearStart", data.value, null);
                }
            });
            $("#monthStart").dxSelectBox({
                items: monthData,
                displayExpr: 'title',
                valueExpr: 'value',
                onValueChanged: function (data) {
                    if (selectorInitialized) {
                        collectParamValues("monthStart", data.value, null);
                        checkDynamicData(data, 'Start');
                    }
                }
            });

            $("#yearEnd").dxSelectBox({
                items: yearData,
                onValueChanged: function (data) {
                    if (selectorInitialized) collectParamValues("yearEnd", data.value, null);
                }
            });
            $("#monthEnd").dxSelectBox({
                items: monthData,
                displayExpr: 'title',
                valueExpr: 'value',
                onValueChanged: function (data) {
                    if (selectorInitialized) {
                        collectParamValues("monthEnd", data.value, null);
                        checkDynamicData(data, 'End');
                    }
                }
            });

            setTimeout(function () {
                selectorInitialized = false;
                selectOldValues('yearStart');
                selectOldValues('monthStart');
                checkDynamicData(null, 'Start');
                selectOldValues('yearEnd');
                selectOldValues('monthEnd');
                checkDynamicData(null, 'End');
                selectorInitialized = true;
                collectParamValues(null, null, null);
            }, 100);

        },
        selectorFilterValues: async (container) => {
            const fieldUser = await getSelectorUniversalOptions("Robotics.Base.filterValuesSelector");
            const options = [{ title: VALUE, value: "text" }, { title: FIELDCAMP, value: "field" }];
            //___.___.___.___.___functions___.___.___.___.___
            var valuesHidden = function () // set values html-input-hidden
            {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramsFilter = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);
                var planActive = (planificationsTab.classList.contains("activeTab"));

                if (planActive && typeof currentReportData.PlannedExecutionsList !== 'undefined') {
                    if ((typeof paramsFilter.Value === undefined)// plan parameters
                        || ((typeof paramsFilter.Value === "string")
                            && ((paramsFilter.value === undefined)
                                || (paramsFilter.value === "")))) {
                        var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                        var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                        if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                        {
                            var paramsLastExecution = JSON.parse(currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                            if (paramsLastExecution !== undefined) {
                                paramsFilter = paramsLastExecution.Value;
                            }
                        }
                    }
                }

                if ((typeof paramsFilter.Value) === "object") { paramsFilter = paramsFilter.Value; }

                var typeFilterMin = paramsFilter.typeFilterSince;
                var typeFilterTo = paramsFilter.typeFilterTo;
                var valueSince = paramsFilter.valueSince;
                var valueTo = paramsFilter.valueTo;
                var ckOptionFilter = paramsFilter.optionFilterValues;

                if (ckOptionFilter !== true) { ckOptionFilter = false; }

                //case execute report (lanzar) --> lastParameters
                if (((typeFilterMin === undefined)
                    && (typeFilterTo === undefined)
                    && ((valueSince === "") || (valueSince === undefined))
                    && ((valueTo === "") || (valueTo === undefined))
                    && (ckOptionFilter === false)) && (currentReportData.LastParameters !== undefined)) {
                    paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                    if (paramsLastExecution !== undefined) {
                        paramsFilter = paramsLastExecution.Value;
                    }
                }
                if (paramsFilter.optionFilterValues === undefined) {
                    paramsFilter.optionFilterValues = false;
                    paramsFilter.typeFilterSince = "";
                    paramsFilter.typeFilterTo = "";
                    paramsFilter.valueSince = "";
                    paramsFilter.valueTo = "";
                }
                //________________________
                document.getElementById("optionFilter").value = paramsFilter.optionFilterValues;
                document.getElementById("typeMin").innerText = paramsFilter.typeFilterSince;
                document.getElementById("typeMax").innerText = paramsFilter.typeFilterTo;
                document.getElementById("valueMin").innerText = paramsFilter.valueSince;
                document.getElementById("valueMax").innerText = paramsFilter.valueTo;
            }
            var selectOldValues = function () // set values components from values html-input-hidden
            {
                var ckOptionFilter = document.getElementById("optionFilter").value;
                var typeFilterMin = document.getElementById("typeMin").innerText;
                var typeFilterTo = document.getElementById("typeMax").innerText;
                var valueSince = document.getElementById("valueMin").innerText;
                var valueTo = document.getElementById("valueMax").innerText;

                if (ckOptionFilter === true) {
                    $("#titleFilterValues").dxCheckBox("instance").option("value", false);
                    $("#titleFilterValues").dxCheckBox("instance").option("value", true);
                }
                else {
                    $("#titleFilterValues").dxCheckBox("instance").option("value", true);
                    $("#titleFilterValues").dxCheckBox("instance").option("value", false);
                }

                $("#typeFilterMin").dxSelectBox("instance").option("value", typeFilterMin);
                $("#typeFilterMax").dxSelectBox("instance").option("value", typeFilterTo);

                switch (typeFilterMin) {
                    case "text":
                        tmpValue = valueSince.split(":")[0];
                        tmpValueDec = valueSince.split(":")[1];
                        tmpValue = ((tmpValue === undefined) || (tmpValue === "undefined")) ? "0" : tmpValue;
                        tmpValueDec = ((tmpValueDec === undefined) || (tmpValueDec === "undefined")) ? "00" : tmpValueDec;

                        $("#valueTextMin").dxNumberBox("instance").option("value", tmpValue);
                        $("#valueTextMinDec").dxNumberBox("instance").option("value", tmpValueDec);
                        break;

                    case "field":
                        $("#valueFieldsMin").dxSelectBox("instance").option("value", valueSince);
                        break;
                }

                switch (typeFilterTo) {
                    case "text":
                        tmpValue = valueTo.split(":")[0];
                        tmpValueDec = valueTo.split(":")[1];
                        tmpValue = ((tmpValue === undefined) || (tmpValue === "undefined")) ? "0" : tmpValue;
                        tmpValueDec = ((tmpValueDec === undefined) || (tmpValueDec === "undefined")) ? "00" : tmpValueDec;

                        $("#valueTextMax").dxNumberBox("instance").option("value", tmpValue);
                        $("#valueTextMaxDec").dxNumberBox("instance").option("value", tmpValueDec);
                        break;

                    case "field":
                        $("#valueFieldsMax").dxSelectBox("instance").option("value", valueTo);
                        break;
                }
                collectParamValues(null, null, null);
            }
            var visibility = function () {
                var visibility = document.getElementById("optionFilter").value;
                $("#titleFilterValues").dxCheckBox("instance").option("value", visibility);

                var optionMin = $("#typeFilterMin").dxSelectBox("instance").option('value');
                var optionMax = $("#typeFilterMax").dxSelectBox("instance").option('value');

                $("#valueFieldsMin").dxSelectBox("instance").option('visible', false);
                $("#valueFieldsMax").dxSelectBox("instance").option('visible', false);
                $("#lbMinDecimal").text("");
                $("#lbMaxDecimal").text("");
                $("#valueTextMin").dxNumberBox("instance").option('visible', false);
                $("#valueTextMinDec").dxNumberBox("instance").option('visible', false);
                $("#valueTextMax").dxNumberBox("instance").option('visible', false);
                $("#valueTextMaxDec").dxNumberBox("instance").option('visible', false);

                if (visibility) {
                    $("#label0").text(REPORT_VALUESRANGE_FILTER);
                    $("#label1").text(AND);
                    $("#typeFilterMin").dxSelectBox("instance").option('visible', true);
                    $("#typeFilterMax").dxSelectBox("instance").option('visible', true);

                    if (optionMin === "text") {
                        $("#lbMinDecimal").text(":");
                        $("#valueTextMin").dxNumberBox("instance").option('visible', true);
                        $("#valueTextMinDec").dxNumberBox("instance").option('visible', true);
                    }
                    else if (optionMin === "field") {
                        $("#valueFieldsMin").dxSelectBox("instance").option('visible', true);
                    }

                    if (optionMax === "text") {
                        $("#lbMaxDecimal").text(":");
                        $("#valueTextMax").dxNumberBox("instance").option('visible', true);
                        $("#valueTextMaxDec").dxNumberBox("instance").option('visible', true);
                    }
                    else if (optionMax === "field") {
                        $("#valueFieldsMax").dxSelectBox("instance").option('visible', true);
                    }
                }
                else {
                    $("#label0").text("");
                    $("#label1").text("");
                    $("#typeFilterMax").dxSelectBox("instance").option('visible', false);
                    $("#typeFilterMin").dxSelectBox("instance").option('visible', false);
                }
            }
            //___.___.___.___._______________.___.___.___.___
            //_._._._._._._._._._._._._._._._._._._._._._._._\\
            $("#valueTextMin").dxNumberBox({	//[TEXT Min]  ___:__
                format: "####0",
                max: 99999,
                visible: false,
                width: "65px",
                onChange: function (data) {
                    var value = data.event.currentTarget.value;
                    collectParamValues("valueTextMin", value, null);
                    visibility();
                }
            });
            $("#lbMinDecimal").text(":");
            $("#valueTextMinDec").dxNumberBox({	//[TEXT Min]  ___:__
                format: "00",
                max: 59,
                visible: false,
                width: "45px",
                onChange: function (data) {
                    var value = data.event.currentTarget.value;
                    collectParamValues("valueTextMinDec", value, null);
                    visibility();
                }
            });
            //_._._._._._._._._._._._._._._._._._._._
            $("#valueTextMax").dxNumberBox({	//[TEXT Max]  ___:__
                format: "####0",
                max: 99999,
                visible: false,
                width: "65px",
                //showClearButton: true,
                onChange: function (data) {
                    var value = data.event.currentTarget.value;
                    collectParamValues("valueTextMax", value, null);
                    visibility();
                }
            });
            $("#lbMaxDecimal").text(":");
            $("#valueTextMaxDec").dxNumberBox({	//[TEXT Max]  ___:__
                format: "00",
                max: 59,
                visible: false,
                width: "45px",
                onChange: function (data) {
                    var value = data.event.currentTarget.value;
                    collectParamValues("valueTextMaxDec", value, null);
                    visibility();
                },
            });
            //_._._._._._._._._._._._._._._._._._._._._._._._//
            $("#valueFieldsMin").dxSelectBox({
                dataSource: new DevExpress.data.ArrayStore({ data: fieldUser, key: "value" }),
                width: "200px",
                displayExpr: "title",
                valueExpr: "value",
                placeholder: SELECT,
                visible: false,
                onValueChanged: function (data) {
                    collectParamValues("valueFieldsMin", data.value, null);
                    visibility();
                }
            });
            $("#valueFieldsMax").dxSelectBox({
                dataSource: new DevExpress.data.ArrayStore({ data: fieldUser, key: "value" }),
                width: "200px",
                displayExpr: "title",
                valueExpr: "value",
                value: options[0].value,
                readOnly: false,
                placeholder: SELECT,
                visible: false,
                onValueChanged: function (data) {
                    collectParamValues("valueFieldsMax", data.value, null);
                    visibility();
                }
            });
            //_._._._._._._._._._._._._._._._._._._._._._._._//
            $("#typeFilterMin").dxSelectBox({
                width: "200px",
                dataSource: new DevExpress.data.ArrayStore({ data: options, key: "value" }),
                displayExpr: "title",
                valueExpr: "value",
                readOnly: false,
                visible: false,
                placeholder: SELECT,
                onValueChanged: function (data) {
                    collectParamValues("typeFilterMin", data.value, null);
                    visibility();
                }
            });
            $("#typeFilterMax").dxSelectBox({
                dataSource: new DevExpress.data.ArrayStore({ data: options, key: "value" }),
                width: "200px",
                displayExpr: "title",
                valueExpr: "value",
                readOnly: false,
                visible: false,
                placeholder: SELECT,
                onValueChanged: function (data) {
                    collectParamValues("typeFilterMax", data.value, null);
                    visibility();
                }
            });
            //_._._._._._._._._._._._._._._._._._._._._._._._//
            $("#titleFilterValues").dxCheckBox({
                text: REPORT_FILTER_VALUES,
                hint: "Detalle",
                height: 25,
                onContentReady: function (data) {
                    valuesHidden();
                    selectOldValues();
                    visibility();
                },
                onValueChanged: function (data) {
                    collectParamValues("titleFilterValues", data.value, null);
                    visibility();
                }
            });
            //_._._._._._._._._._._._._._._._._._._._._._._._//
        },
        selectorProjectsVSL: (container) => {
            $("#startProject").dxNumberBox({
                min: 0,
                width: "65px",
                format: "#0",
                onChange: function (data) {
                    var value = data.event.currentTarget.value;
                    collectParamValues("startProject", value, null);
                }
            });
            $("#endProject").dxNumberBox({
                min: 0,
                width: "65px",
                format: "#0",
                onChange: function (data) {
                    var value = data.event.currentTarget.value;
                    collectParamValues("endProject", value, null);
                }
            });
        },
        selectorUniversal: async () => {
            const selectorOptions = await getSelectorUniversalOptions(paramsSignatures[indexParamsSignatures].Type);

            var changeUniversalSelector = function (data) {
                if (paramsSignatures[indexParamsSignatures].Type == 'Robotics.Base.userFieldsSelector' || paramsSignatures[indexParamsSignatures].Type == 'Robotics.Base.userFieldsSelectorRadioBtn') {
                    var selected = data.selectedRowKeys.join(",");
                    collectParamValues(selected)
                } else {
                    paramsSignatures[indexParamsSignatures].Value = data.selectedRowKeys.join(",");
                    editedReportData.ParametersList = [
                        ...new Set([
                            ...paramsSignatures,
                            ...editedReportData.ParametersList,
                        ]),
                    ];
                }
            }

            let selectionOptions = {
                allowSelectAll: true,
                mode: "multiple",
                showCheckBoxesMode: "always",
            }
            let caption = paramsSignatures[indexParamsSignatures].Name;

            switch (paramsSignatures[indexParamsSignatures].Type) {
                case ("Robotics.Base.userFieldsSelectorRadioBtn"): caption = paramsSignatures[indexParamsSignatures].Description; break;
                case ("Robotics.Base.userFieldsSelector"): caption = TYPE_Userfield; break;
                case ("Robotics.Base.shiftsSelector"): caption = TYPE_Shifts; break;
                case ("Robotics.Base.holidaysSelector"): caption = TYPE_HolidayShifts; break;
                case ("Robotics.Base.conceptGroupsSelector"): caption = TYPE_ConceptGroup; break;

            }



            if (paramsSignatures[indexParamsSignatures].Type == 'Robotics.Base.userFieldsSelectorRadioBtn') {
                selectionOptions = {
                    allowSelectAll: false,
                    mode: "single",
                    showCheckBoxesMode: "onClick",
                }
            }

            $("#selectorUniversal").dxDataGrid({
                dataSource: selectorOptions,
                keyExpr: "value",
                width: 300,
                height: 460,
                scrolling: { mode: "virtual", preloadEnabled: true, },
                showColumnLines: false,
                columns: [
                    {
                        caption: caption,
                        dataField: "title",
                    },
                ],
                selection: selectionOptions,
                paging: { enabled: false },
                onSelectionChanged: (e) => {
                    changeUniversalSelector(e);
                },
            });

            collectParamValues(null, null, null);
        },
        selectorConcepts: async () => {
            // [ LANGUAGE__________________________________________
            $("#conceptDescription").text(REPORT_PARAMETER_CONCEPT_DESCRIPTION);
            $("#titleGroupConcepts").text(REPORT_PARAMETER_CONCEPT_GROUP);
            $("#titleConcepts").text(REPORT_PARAMETER_CONCEPTS);
            DevExpress.localization.loadMessages({ "en": { "dxList-nextButtonText": VIEWMORE } });
            // ] LANGUAGE__________________________________________
            // [ VARIABLE _________________________________________
            //actual selection (dxlist)
            var selectedItemConceptsByGroup = [];
            var selectedItemConcepts = [];

            //old selection (div hidden)
            var arrayGroupConcepts = [];
            var arrayConcepts = [];

            // if exist Changes
            var ifChangesGroups = false;
            var ifChangesConcepts = false;

            // origin call change
            var originCallChange = "";
            initializedGroupConcepts = false;
            initializedConcepts = false;
            //] VARIABLE _________________________________________
            //[ FUNCTIONS__________________________________________

            var selectOldValues = function () // set values components from values html-input-hidden
            {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if ((planificationsTab.classList.contains("activeTab")))//PLANIFICATION
                {
                    if ((typeof paramSelected.Value === undefined)// plan parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === "")))) {
                        var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                        var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                        if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                        {
                            //control en el caso que los parametros seleccionados en el report hayan cambiado y no esten en la ultima planificación lanzada
                            paramsLastExecution = JSON.parse(currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                            if (paramsLastExecution !== undefined) {
                                paramSelected = paramsLastExecution.Value;
                            }
                        }
                    }
                }
                else//REPORT
                {
                    if (((typeof paramSelected.Value === undefined)// last report parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === ""))))
                        && (currentReportData.LastParameters !== undefined)) {
                        paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                        if (paramsLastExecution !== undefined) {
                            paramSelected = paramsLastExecution.Value;
                        }
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }


                collectParamValues(paramSelected, null, null);

            }

            var selectOrigin = function (data) {
                if (originCallChange === "") {
                    switch (data) {
                        case ("change_concept"): originCallChange = "listConcepts"; break;
                        case ("change_group"): originCallChange = "listGroupsConcepts"; break;
                    }
                }

                arrayGroupConcepts = document.getElementById("selecttionItemsGroups").innerText.split(",");
                arrayConcepts = document.getElementById("selecttionItemsConcepts").innerText.split(",");

                selectedItemConceptsByGroup = $("#listGroupsConcepts").dxList("instance").option("selectedItemKeys"); //ej:["73,71,70","31,32,30,49","39,22,42,58","33,56"]
                selectedItemConcepts = $("#listConcepts").dxList("instance").option("selectedItemKeys");

                ifChangesGroups = JSON.stringify(selectedItemConceptsByGroup.join(",").split(",")) !== JSON.stringify(arrayGroupConcepts);
                ifChangesConcepts = JSON.stringify(selectedItemConcepts) !== JSON.stringify(arrayConcepts);
            }
            var selectList = function (Tag) {
                selectOrigin(originCallChange);

                // origin change: concepts
                // data_concept diferent : true
                // result : asign new values  dxList to hidden
                if ((originCallChange === "listConcepts") && (ifChangesConcepts)) {
                    document.getElementById("selecttionItemsConcepts").innerText = selectedItemConcepts.toString();
                }

                // origin change: ""
                // result : asign hidden values to dxList
                if ((originCallChange === "") && (ifChangesConcepts)) {
                    document.getElementById("selecttionItemsConcepts").innerText = selectedItemConcepts.join(",").toString();
                    $("#listConcepts").dxList("instance").option("selectedItemKeys", selectedItemConcepts);
                }

                if ((Tag === "Groups")) {
                    selectGroupsByConceptSelected(itemsListGroups, selectedItemConcepts);
                }
            }
            var selectGroupsByConceptSelected = function (itemsListGroups, itemsSelectedConcepts) {
                var ifselectGroup = true;
                var arrayGroupConcepts = [];
                //recorremos lista de grupo
                for (a in itemsListGroups) {
                    var Group = itemsListGroups[a].value;
                    var itemsList = Group.split(",");

                    ifselectGroup = true;
                    //miramos si todos los items de itemList estan seleccionados ( contenidos en itemsSelectedConcepts )
                    for (b in itemsList) {
                        var existe = itemsSelectedConcepts.includes(itemsList[b]);
                        if (!(existe)) {
                            ifselectGroup = false;
                        }
                    }
                    if (ifselectGroup) {
                        arrayGroupConcepts.push(Group);
                    }
                }

                $("#listGroupsConcepts").dxList("instance").option("selectedItemKeys", arrayGroupConcepts);
                document.getElementById("selecttionItemsGroups").innerText = arrayGroupConcepts;
            }
            var initSelectionConcepts = function (data) {
                selectList("Concepts");
                selectList("Groups");
                selectOldValues();
            }
            var changeConcepts = function (data) {
                var selected = $("#listConcepts").dxList("instance").option("selectedItemKeys").join(",");
                selectList("Groups");
                collectParamValues(selected, null);
            }
            var readyConcepts = function (data) {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);
                var stringConcepts = null;

                if (paramSelected.Value !== undefined) {
                    stringConcepts = paramSelected.Value.split(",");
                }
                if (stringConcepts !== null) {
                    $("#listConcepts").dxList("instance").option('selectedItemKeys', stringConcepts);
                }
                selectList("Groups");
            }
            var removeItemFromArray = function (arr, item) {
                var i = arr.indexOf(item);

                if (i !== -1) { arr.splice(i, 1); }
            }
            //compara 2 arrays y devuelve los valores que no coinciden
            var unequalConcepts = function (a1, a2, key) {
                var arrayMax = [];
                var arrayMin = [];

                if (a1.length > a2.length) {
                    arrayMax = a1;
                    arrayMin = a2;
                }
                else {
                    arrayMax = a2;
                    arrayMin = a1;
                }

                if (key === "value") {
                    for (a in arrayMin) {
                        var index = arrayMax.findIndex(p => p.value === arrayMin[a].value)
                        if ((index + 1) > 0) arrayMax.splice(index, 1);
                    }
                }
                else {
                    for (a in arrayMin) {
                        var exist = arrayMax.includes(arrayMin[a]);

                        if (exist) {
                            var index = arrayMax.indexOf(arrayMin[a]);
                            arrayMax.splice(index, 1);
                        }
                    }
                }

                return (arrayMax);
            }

            var reorderArray = function (items, movingIndex, destinationIndex) {
                var movedItemData = items[movingIndex];

                items.splice(movingIndex, 1);
                items.splice(destinationIndex, 0, movedItemData);

                return items;
            }
            var reorderConcepts = function (e) {
                if (!e.component.isItemSelected(e.toIndex)) return;

                var selectedItemKeys = e.component.option("selectedItemKeys"),
                    oldKeyPosition = e.component.option("selectedItemKeys").indexOf(e.itemData.value),
                    newKeyPosition = e.toIndex,
                    newSelectedItemKeys = reorderArray(selectedItemKeys, oldKeyPosition, newKeyPosition);

                e.component.option("selectedItemKeys", newSelectedItemKeys);
                changeConcepts(e);
            }
            //seleccionar items en lista de saldos,solo si se ha ordenado desde el grupo
            var selectConceptsByGroup = function () {
                if (originCallChange !== "listConcepts") {
                    arrayGroupConcepts = document.getElementById("selecttionItemsGroups").innerText.split(",");
                    arrayConcepts = document.getElementById("selecttionItemsConcepts").innerText.split(",");

                    selectedItemConceptsByGroup = $("#listGroupsConcepts").dxList("instance").option("selectedItemKeys"); //ej:["73,71,70","31,32,30,49","39,22,42,58","33,56"]
                    selectedItemConcepts = $("#listConcepts").dxList("instance").option("selectedItemKeys");

                    // [ CASE 1 ] NO-SELECT group  => DELETE selection group to delection concepts
                    if ((arrayGroupConcepts.length > selectedItemConceptsByGroup.join(",").split(",").length)) {
                        var noSelectConcepts = unequalConcepts(arrayGroupConcepts, selectedItemConceptsByGroup.join(",").split(","));
                        for (e in noSelectConcepts) {
                            removeItemFromArray(selectedItemConcepts, noSelectConcepts[e]);
                        }
                    }

                    // [ CASE 2 ] SELECT  group => add selection group to selection concepts
                    if ((arrayGroupConcepts.length < selectedItemConceptsByGroup.join(",").split(",").length)
                        && (arrayConcepts.join(",").toString() == selectedItemConcepts.join(",").toString())
                    ) {
                        var SelectConcepts = unequalConcepts(arrayGroupConcepts, selectedItemConceptsByGroup.join(",").split(","));
                        for (e in SelectConcepts) {
                            var i = selectedItemConcepts.indexOf(SelectConcepts[e]);
                            if (i < 0) {
                                selectedItemConcepts.push(SelectConcepts[e]); //añadir si no esta en el array de concepts
                            }
                        }
                    }

                    $("#selecttionItemsGroups").text(selectedItemConceptsByGroup.join(","));
                    $("#listConcepts").dxList("instance").option("selectedItemKeys", selectedItemConcepts);
                    $("#selecttionItemsConcepts").text(selectedItemConcepts.join(","));
                }
            }
            //] FUNCTIONS__________________________________________
            // [ DATASOURCE _________________________________________
            // data --> from BBDD
            const itemsListGroups = await getSelectorUniversalOptions("Robotics.Base.groupConceptsSelector");
            const listConceptsBBDD = await getSelectorUniversalOptions(paramsSignatures[indexParamsSignatures].Type);

            // data to div hidden -->  switch case (new,plan, report, wizard )
            selectOldValues();

            //datasource dxlist concepts -->   switch case (new,plan, report, wizard )
            var setItemsConcepts = function () {
                var selectedConcepts = document.getElementById("selecttionItemsConcepts").innerText.split(",");
                var value = "";
                var index;
                var title = "";
                var arrayItemsConcepts = [];
                var arrayResult = [];

                for (c in selectedConcepts) {
                    value = selectedConcepts[c];
                    index = listConceptsBBDD.findIndex(p => p.value == value);
                    if (index + 1 > 0) {
                        title = listConceptsBBDD[index]["title"];
                        arrayItemsConcepts.push({ "value": value, "title": title });
                        arrayResult.push({ "value": value, "title": title });
                    }
                }

                var addConceptsNotSelected = unequalConcepts(listConceptsBBDD, arrayItemsConcepts, "value");
                for (c2 in addConceptsNotSelected) {
                    value = addConceptsNotSelected[c2].value;
                    index = listConceptsBBDD.findIndex(p => p.value == value);
                    if (index + 1 > 0) {
                        title = listConceptsBBDD[index]["title"];
                        arrayResult.push({ "value": value, "title": title });
                    }
                }

                return arrayResult;
            }
            //var listConcepts = listConceptsBBDD;
            var listConcepts = setItemsConcepts();
            // ] DATASOURCE _________________________________________
            //[ COMPONENTS_________________________________________
            $("#listGroupsConcepts").dxList({
                dataSource: new DevExpress.data.DataSource({ store: new DevExpress.data.ArrayStore({ key: "value", data: itemsListGroups }) }),
                displayExpr: "title",
                height: 400,
                showSelectionControls: true,
                selectionMode: "all",
                selectAllMode: "allPages",
                selectAllText: SELECT_ALL,
                onInitialized: function (data) { initializedGroupConcepts = true; },
                onSelectionChanged: function (data) {
                    selectOrigin('change_group');
                    selectConceptsByGroup();
                    originCallChange = "";
                },
                onContentReady: function (data) { }
            }).dxList("instance");

            $("#listConcepts").dxList({
                dataSource: new DevExpress.data.DataSource({ store: new DevExpress.data.ArrayStore({ key: "value", data: listConcepts }) }),
                itemDragging: {
                    allowReordering: true,
                },
                displayExpr: "title",
                height: 400,
                showSelectionControls: true,
                selectionMode: "all",
                selectAllMode: "allPages",
                selectAllText: SELECT_ALL,
                onInitialized: function (data) {
                    initializedConcepts = true;
                    initSelectionConcepts(data);
                },
                onSelectionChanged: function (data) {
                    selectOrigin('change_concept');
                    changeConcepts(data);
                    originCallChange = "";
                },
                onItemReordered: reorderConcepts,
                onContentReady: function (data) { readyConcepts(data); },
            }).dxList("instance");
            //] COMPONENTS_________________________________________
        },
        selectorCauses: async () => {
            // [ LANGUAGE__________________________________________
            $("#causeDescription").text(REPORT_PARAMETER_CONCEPT_DESCRIPTION);
            $("#titleCauses").text(REPORT_PARAMETER_CAUSES);
            DevExpress.localization.loadMessages({ "en": { "dxList-nextButtonText": VIEWMORE } });
            // ] LANGUAGE__________________________________________
            // [ VARIABLE _________________________________________
            var selectedItemCauses = [];

            var arrayCauses = [];

            // if exist Changes
            var ifChangesCauses = false;

            // origin call change
            var originCallChange = "";
            initializedCauses = false;
            //] VARIABLE _________________________________________
            //[ FUNCTIONS__________________________________________
            var selectOrigin = function (data) {
                if (originCallChange === "") {
                    switch (data) {
                        case ("change_cause"): originCallChange = "listCauses"; break;
                    }
                }

                arrayCauses = document.getElementById("selecttionItemsCauses").innerText.split(",");

                selectedItemCauses = $("#listCauses").dxList("instance").option("selectedItemKeys");

                ifChangesCauses = JSON.stringify(selectedItemCauses) !== JSON.stringify(arrayCauses);
            }
            var selectList = function (Tag) {
                selectOrigin(originCallChange);

                // origin change: causes
                // data_cause diferent : true
                // result : asign new values  dxList to hidden
                if ((originCallChange === "listCauses") && (ifChangesCauses)) {
                    document.getElementById("selecttionItemsCauses").innerText = selectedItemCauses.toString();
                }

                // origin change: ""
                // result : asign hidden values to dxList
                if ((originCallChange === "") && (ifChangesCauses)) {
                    document.getElementById("selecttionItemsCauses").innerText = selectedItemCauses.join(",").toString();
                    $("#listCauses").dxList("instance").option("selectedItemKeys", selectedItemCauses);
                }
            }

            var selectOldValues = function () // set values components from values html-input-hidden
            {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);//actual parametars
                var paramsLastExecution = "";

                if ((planificationsTab.classList.contains("activeTab")))//PLANIFICATION
                {
                    if ((typeof paramSelected.Value === undefined)// plan parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === "")))) {
                        var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey"));
                        var isNewPlanification = (currentReportData.PlannedExecutionsList[indexLastValues].Id === "newPlanification");

                        if ((indexLastValues >= 0) && (!isNewPlanification)) // Datos ultima ejecución
                        {
                            //control en el caso que los parametros seleccionados en el report hayan cambiado y no esten en la ultima planificación lanzada
                            paramsLastExecution = JSON.parse(currentReportData.PlannedExecutionsList[indexLastValues].ParametersJson).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                            if (paramsLastExecution !== undefined) {
                                paramSelected = paramsLastExecution.Value;
                            }
                        }
                    }
                }
                else//REPORT
                {
                    if (((typeof paramSelected.Value === undefined)// last report parameters
                        || ((typeof paramSelected.Value === "string")
                            && ((paramSelected.value === undefined)
                                || (paramSelected.value === ""))))
                        && (currentReportData.LastParameters !== undefined)) {
                        paramsLastExecution = JSON.parse(currentReportData.LastParameters).find((param) => param.Name === paramsSignatures[indexParamsSignatures].Name);
                        if (paramsLastExecution !== undefined) {
                            paramSelected = paramsLastExecution.Value;
                        }
                    }
                }

                if ((typeof paramSelected.Value) === "object") { paramSelected = paramSelected.Value; }


                collectParamValues(paramSelected, null, null);

            }

            var initSelectionCauses = function (data) {
                selectList("Causes");
                selectOldValues();
            }
            var changeCauses = function (data) {
                var selected = $("#listCauses").dxList("instance").option("selectedItemKeys").join(",");
                collectParamValues(selected, null);
            }
            var readyCauses = function (data) {
                var whatEdition = document.querySelector("#editView > form").getAttribute("id");
                var paramSelected = editedReportData.ParametersList.find((param) => param.TemplateName === whatEdition && param.Name === paramsSignatures[indexParamsSignatures].Name);
                var stringCauses = null;

                if (paramSelected.Value !== undefined) {
                    try {
                        stringCauses = paramSelected.Value.split(",");
                    }
                    catch { ; }
                }
                if (stringCauses !== null) {
                    $("#listCauses").dxList("instance").option('selectedItemKeys', stringCauses);
                }
            }
            var removeItemFromArray = function (arr, item) {
                var i = arr.indexOf(item);

                if (i !== -1) { arr.splice(i, 1); }
            }
            //compara 2 arrays y devuelve los valores que no coinciden
            var unequalCauses = function (a1, a2, key) {
                var arrayMax = [];
                var arrayMin = [];

                if (a1.length > a2.length) {
                    arrayMax = a1;
                    arrayMin = a2;
                }
                else {
                    arrayMax = a2;
                    arrayMin = a1;
                }

                if (key === "value") {
                    for (a in arrayMin) {
                        var index = arrayMax.findIndex(p => p.value === arrayMin[a].value)
                        if ((index + 1) > 0) arrayMax.splice(index, 1);
                    }
                }
                else {
                    for (a in arrayMin) {
                        var exist = arrayMax.includes(arrayMin[a]);

                        if (exist) {
                            var index = arrayMax.indexOf(arrayMin[a]);
                            arrayMax.splice(index, 1);
                        }
                    }
                }

                return (arrayMax);
            }

            var reorderArray = function (items, movingIndex, destinationIndex) {
                var movedItemData = items[movingIndex];

                items.splice(movingIndex, 1);
                items.splice(destinationIndex, 0, movedItemData);

                return items;
            }
            var reorderCauses = function (e) {
                if (!e.component.isItemSelected(e.toIndex)) return;

                var orderedItems = e.component.option("items"),
                    previousSelectedKey = null;

                for (var i = e.toIndex - 1; i > 0; i--) {
                    var item = orderedItems[i];
                    if (e.component.isItemSelected(i)) {
                        previousSelectedKey = item.value;
                        break;
                    }
                }

                var selectedItemKeys = e.component.option("selectedItemKeys"),
                    oldKeyPosition = selectedItemKeys.indexOf(e.itemData.value),
                    newKeyPosition = selectedItemKeys.indexOf(previousSelectedKey) + 1,
                    newSelectedItemKeys = reorderArray(selectedItemKeys, oldKeyPosition, newKeyPosition);

                e.component.option("selectedItemKeys", newSelectedItemKeys);
                changeCauses(e);
            }

            //] FUNCTIONS__________________________________________
            // [ DATASOURCE _________________________________________
            // data --> from BBDD
            const listCausesBBDD = await getSelectorUniversalOptions(paramsSignatures[indexParamsSignatures].Type);

            // data to div hidden -->  switch case (new,plan, report, wizard )
            selectOldValues();

            //datasource dxlist causes -->   switch case (new,plan, report, wizard )
            var setItemsCauses = function () {
                var selectedCauses = document.getElementById("selecttionItemsCauses").innerText.split(",");
                var value = "";
                var index;
                var title = "";
                var arrayItemsCauses = [];
                var arrayResult = [];

                for (c in selectedCauses) {
                    value = selectedCauses[c];
                    index = listCausesBBDD.findIndex(p => p.value == value);
                    if (index + 1 > 0) {
                        title = listCausesBBDD[index]["title"];
                        arrayItemsCauses.push({ "value": value, "title": title });
                        arrayResult.push({ "value": value, "title": title });
                    }
                }

                var addCausesNotSelected = unequalCauses(listCausesBBDD, arrayItemsCauses, "value");
                for (c2 in addCausesNotSelected) {
                    value = addCausesNotSelected[c2].value;
                    index = listCausesBBDD.findIndex(p => p.value == value);
                    if (index + 1 > 0) {
                        title = listCausesBBDD[index]["title"];
                        arrayResult.push({ "value": value, "title": title });
                    }
                }

                return arrayResult;
            }
            var listCauses = setItemsCauses();
            // ] DATASOURCE _________________________________________
            //[ COMPONENTS_________________________________________

            $("#listCauses").dxList({
                dataSource: new DevExpress.data.DataSource({ store: new DevExpress.data.ArrayStore({ key: "value", data: listCauses }) }),
                itemDragging: {
                    allowReordering: true,
                },
                displayExpr: "title",
                height: 400,
                showSelectionControls: true,
                selectionMode: "all",
                selectAllMode: "allPages",
                selectAllText: SELECT_ALL,
                onInitialized: function (data) {
                    initializedCauses = true;
                    initSelectionCauses(data);
                },
                onSelectionChanged: function (data) {
                    selectOrigin('change_cause');
                    changeCauses(data);
                    originCallChange = "";
                },
                onItemReordered: reorderCauses,
                onContentReady: function (data) { readyCauses(data); },
            }).dxList("instance");
            //] COMPONENTS_________________________________________
        },
        planifications: (container) => {
            const plannificationsDataSource = currentReportData.PlannedExecutionsList.map((exec) => Object.assign({}, exec, exec.ViewFields));

            $(container).dxDataGrid({
                dataSource: plannificationsDataSource,
                noDataText: SIN_PLANIFICACIONES,
                keyExpr: "Id",
                rowType: "data",
                columns: [
                    {
                        caption: DESCRIPTION,
                        dataField: "description",
                        dataType: "string",
                        width: 250,
                        allowEditing: true,
                        editorOptions: {
                            onFocusOut: (e) => {
                                const textInputValue = e.component.option("value");
                                const focusedRowKey = $(container)
                                    .dxDataGrid("instance")
                                    .option("focusedRowKey");
                                const focusedPlanificationIndex = editedReportData
                                    .PlannedExecutionsList.findIndex((exec) => exec.Id === focusedRowKey);

                                editedReportData.PlannedExecutionsList[focusedPlanificationIndex].ViewFields.description = textInputValue;
                            },
                        },
                    },
                    {
                        caption: PERIOD,
                        dataField: "scheduledSentence",
                        width: 250,
                        allowEditing: false,
                    },
                    {
                        caption: NEXT_REPORT,
                        dataField: "NextExecutionDate",
                        dataType: "string",
                        alignment: "center",
                        width: 140,
                        allowEditing: false,
                        calculateCellValue: (data) => (typeof data.NextExecutionDate != 'undefined') ? utils.getStringDate(new Date(data.NextExecutionDate)) : "",
                    },
                    {
                        caption: DESTINATION,
                        dataField: "Destination",
                        width: 130,
                        alignment: "center",
                        allowEditing: false,
                        calculateCellValue: (data) => {
                            const destinationObj = JSON.parse(data.Destination);
                            return destinationObj.display || destinationObj.type;
                        },
                    },
                    {
                        caption: LANGUAGE,
                        dataField: "Language",
                        width: 70,
                        alignment: "center",
                        allowEditing: false,
                        visible: !currentReportData.CreatorPassportId,
                    },
                    {
                        caption: REPORT_PARAMETER_FORMAT,
                        dataField: "Format",
                        calculateCellValue: (e) => {
                            if (e.ParametersJson != undefined) {
                                var cParams = JSON.parse(e.ParametersJson);
                                var cFormat = cParams.find(element => element.Type == 'Robotics.Base.formatSelector');
                                var indexFormat = 0;

                                if (cFormat != undefined) {
                                    if (cFormat.Value != null && cFormat.Value != "")
                                        indexFormat = exportFormats.findIndex((format) => format.value === cFormat.Value.format);
                                    else {
                                        var onlyExcel = cParams.find(element => element.Name == 'forceExcel');
                                        if (onlyExcel != null && onlyExcel.Value == true)
                                            indexFormat = 1;
                                    }
                                } else {
                                    indexFormat = exportFormats.findIndex((format) => format.value === e.Format);
                                }

                                return exportFormats[indexFormat]
                                    ? exportFormats[indexFormat].title
                                    : indexFormat;
                            } else {
                                return exportFormats[0].title;
                            }
                        },
                        width: 80,
                        alignment: "center",
                        allowEditing: false,
                    },
                    {
                        caption: WHO,
                        dataField: "CreatorName",
                        alignment: "center",
                        allowEditing: false,
                    },
                    {
                        width: 250,
                        type: "buttons",
                        alignment: "center",
                        allowEditing: false,
                        buttons: [
                            {
                                name: "edit",
                                text: EDITAR,
                                cssClass: "hiddenButton",
                            },
                            {
                                text: ACEPTAR,
                                name: "save",
                                onClick: async (e) => {
                                    const languageInstanceValue = $("#plannerLanguage").is(":empty")
                                        ? null
                                        : $("#plannerLanguage").dxSelectBox("instance");

                                    const formatInstanceValue = $("#plannerFormat").is(":empty")
                                        ? null
                                        : $("#plannerFormat").dxSelectBox("instance");

                                    const isConfigSet = utils.isThisWidgetSet([
                                        formatInstanceValue,
                                        languageInstanceValue,
                                        $("#plannerScheduler > .stepOne")
                                            .dxRadioGroup("instance")
                                            .option("value"),
                                    ]);

                                    if (isConfigSet) {
                                        await editor.schedule(scheduleFrequency, scheduleOptions, e);
                                        await updateReportActions(currentReportData.Id);
                                    }
                                    else {
                                        utils.removeIdNewPlanifications();

                                        const isSaved = await editor.acceptEdition(true, "planifications");
                                        isSaved && (await handlePlanExecutionClick());
                                    }
                                },
                            },
                            {
                                text: ELIMINA,
                                name: "delete",
                                cssClass: "hiddenButton",
                                onClick: async (e) => {
                                    const confirms = await utils.dialogDx(
                                        CONFIRMAR_BORRAR_PLANIFICACION,
                                        ELIMINAR_INFORME
                                    );

                                    if (confirms) {
                                        e.component.deleteRow(e.row.rowIndex);

                                        editedReportData.PlannedExecutionsList = editedReportData.PlannedExecutionsList.filter(
                                            (plan) => plan.Id !== e.component.option("focusedRowKey")
                                        );

                                        const isSaved = await editor.acceptEdition(true, "planifications");
                                        isSaved && handlePlanExecutionClick();
                                    }
                                    await updateReportActions(currentReportData.Id);
                                },
                            },
                            {
                                text: CANCELAR,
                                name: "cancel",
                                onClick: (e) => {
                                    if (e.row.key === "newPlanification") {
                                        e.component.cancelEditData();
                                        e.component.deselectRows(["newPlanification"]);
                                        e.component.deleteRow(e.row.rowIndex);
                                        return;
                                    }
                                    //trigger schedule
                                    editor.cancelEdition(false);
                                    //reset planner page
                                    handlePlanExecutionClick();
                                },
                            },
                        ],
                    },
                    {
                        visible: false,
                        caption: "update",
                        dataField: "updating",
                    },
                ],
                onEditingStart: async (e) => {
                    e.data.updating = Date.now();
                    const descriptionCellElement = e.component.getCellElement(e.component.getRowIndexByKey(e.key), 0);
                    e.component.focus(descriptionCellElement);

                    //print options
                    const reportPlanOptions = document.querySelector("#reportPlanOptions");
                    const schedulerStep1Div = document.querySelector("#plannerScheduler > .stepOne");
                    const plannerLanguageDiv = document.querySelector("#plannerLanguage");
                    const plannerFormatDiv = document.querySelector("#plannerFormat");
                    const plannerDestinationDiv = document.querySelector("#plannerDestination");

                    reportPlanOptions.style.display = "";
                    reportPlanOptions.style.opacity = 1;
                    reportPlanOptions.style.pointerEvents = "auto";

                    populator.schedulerStepOne(schedulerStep1Div, { selection: "optOneTime", }, e.data);
                    await populator.plannerLanguage(plannerLanguageDiv, e.data);
                    populator.plannerFormat(plannerFormatDiv, e.data);
                    populator.plannerDestination(plannerDestinationDiv, e.data);
                },
                onFocusedRowChanged: async (e) => {
                    var datarow = e.row;
                    var dt = datarow.data;

                    datarow.cells[datarow.cells.length - 1].cellElement
                        .find("a")
                        .css("display", "inline-block");

                    datarow.data.updating = Date.now();
                    e.component.focus(datarow);

                    //print options
                    const reportPlanOptions = document.querySelector("#reportPlanOptions");
                    reportPlanOptions.style.display = "";
                    reportPlanOptions.style.opacity = 0.8;
                    reportPlanOptions.style.pointerEvents = "none";

                    const schedulerStep1Div = document.querySelector("#plannerScheduler > .stepOne");
                    const plannerLanguageDiv = document.querySelector("#plannerLanguage");
                    const plannerFormatDiv = document.querySelector("#plannerFormat");
                    const plannerDestinationDiv = document.querySelector("#plannerDestination");

                    populator.schedulerStepOne(schedulerStep1Div, { selection: "optOneTime", }, dt);
                    await populator.plannerLanguage(plannerLanguageDiv, dt);
                    populator.plannerFormat(plannerFormatDiv, dt);
                    //populator.plannerDestination(plannerDestinationDiv, dt);
                    const texBoxInstance = $("#planificationDestinationTextBox")
                        .dxTextBox({
                            //text: destination.display || destination.type,
                            readOnly: true,
                            width: "400px",
                            placeholder: DESTINATION_PLACEHOLDER,
                            value: dt && dt.Destination ? JSON.parse(dt.Destination).display : null,
                        })
                        .dxTextBox("instance");

                },
                showColumnLines: false,
                showBorders: false,
                editing:
                {
                    mode: "row",
                    allowUpdating: true,
                    allowDeleting: true,
                    texts: { confirmDeleteMessage: "", },
                },
                paging: { enabled: false },
                //scrolling: { mode: "virtual", },
                hoverStateEnabled: true,
                focusedRowEnabled: true,
            });
        },
        schedulerStepOne: (container, options, { Scheduler }) => {
            const stepOneInstance = $(container)
                .dxRadioGroup({
                    dataSource: radioInputsSchedulerStep1,
                    valueExpr: (data) => data,
                    displayExpr: "title",
                    activeStateEnabled: false,
                    focusStateEnabled: false,
                    hoverStateEnabled: false,
                })
                .dxRadioGroup("instance");

            stepOneInstance.off("valueChanged");
            stepOneInstance.on("valueChanged", function (event) {
                const stepTwoContainer = document.querySelector("#plannerScheduler > .stepTwo");
                //encontrar instancias, i hacer un dispose + bum
                widgetInstancesToDestroy.map((instance) => {
                    instance.dispose();
                    instance.element().remove();
                });
                populator.schedulerStepTwo(
                    stepTwoContainer,
                    {
                        range: event.value && event.value.range,
                        title: event.value && event.value.title,
                    },
                    Scheduler &&
                        event.value &&
                        Scheduler.substring(0, 1) ==
                        radioInputsSchedulerStep1.find(
                            (opt) => opt.range == event.value.range
                        ).value
                        ? Scheduler
                        : ""
                );
            });

            stepOneInstance.option(
                "value",
                radioInputsSchedulerStep1.find((opt) => opt.value === parseInt(Scheduler.substring(0, 1), 10)) || null
            );
        },
        schedulerStepTwo: async (container, options, Scheduler) => {
            container.innerHTML = !!options.range && (await getViewTemplate(`scheduler-${options.range}`));

            const divOne = container.querySelector(".opt1");
            const divTwo = container.querySelector(".opt2");
            const divThree = container.querySelector(".opt3");
            const divFour = container.querySelector(".opt4");
            const provisionalState = { ...options };
            const widgetInstances = {};
            const arrScheduler = Scheduler && Scheduler.split("@");

            switch (options.range) {
                case "optHours":
                    scheduleFrequency = 4;
                    widgetInstances.Hour = $(divOne)
                        .dxDateBox({
                            type: "time",
                            value: new Date(`1/1/2019 ${arrScheduler ? arrScheduler[1] : "12:00:00"}`),
                            pickerType: "native",
                        })
                        .dxDateBox("instance");
                    widgetInstancesToDestroy.push(widgetInstances.Hour);
                    break;

                case "optDiary":
                    scheduleFrequency = 0;
                    widgetInstances.Days = $(divOne)
                        .dxNumberBox({
                            value: arrScheduler ? arrScheduler[2] : 1,
                            showSpinButtons: true,
                            max: 366,
                            min: 1,
                        })
                        .dxNumberBox("instance");

                    widgetInstances.Hour = $(divTwo)
                        .dxDateBox({
                            type: "time",
                            value: new Date(`1/1/2019 ${arrScheduler ? arrScheduler[1] : "12:00:00"}`),
                            displayFormat: "HH:mm",
                        })
                        .dxDateBox("instance");
                    widgetInstancesToDestroy.push(widgetInstances.Days, widgetInstances.Hour);
                    break;

                case "optWeekly":
                    scheduleFrequency = 1;
                    const weekData = [{ title: "Lunes" }, { title: "Martes" }, { title: "Miercoles" },
                    { title: "Jueves" }, { title: "Viernes" }, { title: "Sábado" },
                    { title: "Domingo" },];

                    widgetInstances.Weeks = $(divOne)
                        .dxNumberBox({
                            value: arrScheduler ? arrScheduler[2] : 1,
                            showSpinButtons: true,
                            max: 100,
                            min: 1,
                        })
                        .dxNumberBox("instance");

                    const valueArr = (arr) => {
                        return arr
                            .split("")
                            .reduce(
                                (acc, d, i) => (d == 1 && acc.push(weekData[i].title), acc),
                                []
                            ).join(", ");
                    }

                    widgetInstances.WeekDay = $(divTwo).dxDropDownBox({
                        keyExpr: "title",
                        dataSource: weekData,
                        value: arrScheduler ? valueArr(arrScheduler[3]) : "",
                        onInitialized: (e) => {
                            if (scheduleOptions) {
                                scheduleOptions.WeekDaysSentence = arrScheduler
                                    ? valueArr(arrScheduler[3])
                                    : "";
                                scheduleOptions.WeekDays = arrScheduler
                                    ? arrScheduler[3]
                                    : "0000000";
                            }
                        },
                        contentTemplate: (e) => {
                            return $("<div>").dxDataGrid({
                                dataSource: e.component.option("dataSource"),
                                columns: [{ dataField: "title" }],
                                keyExpr: "title",
                                value: arrScheduler ? valueArr(arrScheduler[3]) : "",
                                selectedRowKeys: arrScheduler
                                    ? arrScheduler[3]
                                        .split("")
                                        .reduce(
                                            (acc, d, i) => (
                                                d == 1 && acc.push(weekData[i].title), acc
                                            ),
                                            []
                                        )
                                    : [],
                                selection: {
                                    mode: "multiple",
                                    showCheckBoxesMode: "always",
                                },
                                filterBuilderPopup: { fullScreen: true },
                                showColumnHeaders: false,
                                showColumnLines: false,
                                showBorders: true,
                                onContentReady: (event) =>
                                    event.element.parent().css("padding", "0"),
                                onSelectionChanged: (event) => {
                                    scheduleOptions.WeekDaysSentence = event.selectedRowKeys.join(
                                        ", "
                                    );
                                    scheduleOptions.WeekDays = weekData
                                        .map((day) => !!event.selectedRowKeys.find((selected) => selected === day.title) ? 1 : 0)
                                        .join("");
                                    e.component.field().val(event.selectedRowKeys.join(", "));
                                },
                            });
                        },
                    }).dxDropDownBox("instance");
                    widgetInstances.Hour = $(divThree)
                        .dxDateBox({
                            type: "time",
                            value: new Date(`1/1/2019 ${arrScheduler ? arrScheduler[1] : "12:00:00"}`),
                            displayFormat: "HH:mm",
                        })
                        .dxDateBox("instance");
                    widgetInstancesToDestroy.push(widgetInstances.Weeks, widgetInstances.WeekDay, widgetInstances.Hour);
                    break;

                case "optMonthly":
                    scheduleFrequency = 2;
                    const monthlyOrderData = [
                        { title: "1er.", value: 1 },
                        { title: "2o.", value: 2 },
                        { title: "3o.", value: 3 },
                        { title: "4o.", value: 4 },
                        { title: "Último", value: 5 },
                    ];

                    const monthlyWeekData = [
                        { title: "Lunes", value: 1 },
                        { title: "Martes", value: 2 },
                        { title: "Miercoles", value: 3 },
                        { title: "Jueves", value: 4 },
                        { title: "Viernes", value: 5 },
                        { title: "Sábado", value: 6 },
                        { title: "Domingo", value: 7 },
                    ];

                    widgetInstances.Day = $(divOne)
                        .dxNumberBox({
                            value: arrScheduler
                                ? arrScheduler[2] == 0
                                    ? arrScheduler[3]
                                    : 1
                                : 1,
                            showSpinButtons: true,
                            max: 31,
                            min: 1,
                            onInitialized: (e) => {
                                if (arrScheduler[2] == 0) {
                                    $(".monthlyOpt1").fadeTo(1, 1);
                                    $(".monthlyOpt2").fadeTo(1, 0.6);
                                }
                                scheduleOptions.MonthlyType = 0;
                            },
                            onFocusIn: (e) => {
                                $(".monthlyOpt1").fadeTo(1, 1);
                                $(".monthlyOpt2").fadeTo(1, 0.6);
                                scheduleOptions.MonthlyType = 0;
                            },
                        })
                        .dxNumberBox("instance");

                    widgetInstances.Start = $(divTwo)
                        .dxSelectBox({
                            text: "value",
                            keyExpr: "title",
                            displayExpr: "title",
                            dataSource: monthlyOrderData,
                            value:
                                arrScheduler && arrScheduler[2] == 1
                                    ? monthlyOrderData.find((opt) => opt.value == arrScheduler[3])
                                    : null,
                            onInitialized: (e) => {
                                if (arrScheduler[2] == 1) {
                                    $(".monthlyOpt2").fadeTo(1, 1);
                                    $(".monthlyOpt1").fadeTo(1, 0.6);
                                    scheduleOptions.MonthlyType = 1;
                                }
                            },
                            onFocusIn: (e) => {
                                scheduleOptions.MonthlyType = 1;
                                $(".monthlyOpt2").fadeTo(1, 1);
                                $(".monthlyOpt1").fadeTo(1, 0.6);
                            },
                        })
                        .dxSelectBox("instance");

                    widgetInstances.WeekDay = $(divThree)
                        .dxSelectBox({
                            text: "value",
                            keyExpr: "title",
                            displayExpr: "title",
                            dataSource: monthlyWeekData,
                            value:
                                arrScheduler && arrScheduler[2] == 1
                                    ? monthlyWeekData.find((opt) => opt.value == arrScheduler[4])
                                    : null,
                            onInitialized: (e) => {
                                if (arrScheduler[2] == 1) {
                                    $(".monthlyOpt2").fadeTo(1, 1);
                                    $(".monthlyOpt1").fadeTo(1, 0.6);
                                }
                            },
                            onFocusIn: (e) => {
                                scheduleOptions.MonthlyType = 1;
                                $(".monthlyOpt2").fadeTo(1, 1);
                                $(".monthlyOpt1").fadeTo(1, 0.6);
                            },
                        })
                        .dxSelectBox("instance");

                    widgetInstances.Hour = $(divFour)
                        .dxDateBox({
                            type: "time",
                            value: new Date(`1/1/2019 ${arrScheduler ? arrScheduler[1] : "12:00:00"}`),
                            displayFormat: "HH:mm",
                        })
                        .dxDateBox("instance");
                    widgetInstancesToDestroy.push(
                        widgetInstances.Day,
                        widgetInstances.Start,
                        widgetInstances.WeekDay,
                        widgetInstances.Hour
                    );
                    break;
                case "optOneTime":
                    scheduleFrequency = 3;

                    const tomorrow = new Date().setDate(new Date().getDate() + 1);
                    const midnight = new Date(tomorrow).setHours(0, 0, 0, 0);

                    widgetInstances.DateTime = $(divOne)
                        .dxDateBox({
                            type: "datetime",
                            displayFormat: "dd-MM-yyyy, HH:mm",
                            invalidDateMessage: "",
                            value: arrScheduler
                                ? new Date(
                                    `${arrScheduler[2].split(" ")[0]} ${arrScheduler[1]}`
                                )
                                : midnight,
                        })
                        .dxDateBox("instance");
                    widgetInstancesToDestroy.push(widgetInstances.DateTime);
                    widgetInstances.Hour = widgetInstances.DateTime;
                    widgetInstances.Day = widgetInstances.DateTime;
                    break;
                default:
                    break;
            }

            scheduleOptions = {
                ...scheduleOptions,
                ...widgetInstances,
            };
        },
        plannerLanguage: async (container, data) => {
            const isDisabled = typeof currentReportData.CreatorPassportId !== "undefined";

            if (!isDisabled) {
                const exportLanguages = await getAllLanguages();

                $(container).dxSelectBox({
                    dataSource: exportLanguages,
                    placeholder: LANGUAGES_SELECT_PLACEHOLDER,
                    displayExpr: "Key",
                    valueExpr: "Key",
                    value: data.Language,
                });
            }
        },
        plannerFormat: (container, data) => {
            const isDisabled = true;
            if (!isDisabled) {
                $(container).dxSelectBox({
                    dataSource: exportFormats,
                    placeholder: FORMAT_SELECT_PLACEHOLDER,
                    valueExpr: "value",
                    displayExpr: "title",
                    value: data.Format,
                });
            }
        },
        plannerDestination: (container, data) => {
            $('#planificationDestination').off('click');
            $('#planificationDestination').on('click', async () => {
                const templateDestinationIframe = await getViewTemplate("selectorDestination");
                const buttonsTemplate = await getViewTemplate("editViewBtns");
                const fieldsEditor = document.querySelector("#fieldsEditor");
                const indexPlanificationEdited = editedReportData.PlannedExecutionsList
                    .findIndex((plan) => plan.Id === $("#reportPlannedList")
                        .dxDataGrid("instance")
                        .option("focusedRowKey")
                    );
                fieldsEditor.style.top = "50%";
                fieldsEditor.style.left = "50%";
                fieldsEditor.style.transform = "translate(-50%, -50%)";
                fieldsEditor.style.width = "850px";
                fieldsEditor.style.height = "90vh";
                fieldsEditor.style.maxHeight = "590px";
                fieldsEditor.style.display = "block";
                fieldsEditor.innerHTML = templateDestinationIframe + buttonsTemplate;

                editor.fillUpDestinationSelector(JSON.parse(data.Destination));

                fieldsEditor
                    .querySelector("#acceptEdition")
                    .parentNode.addEventListener("click", () => { editor.hideEditor(); }, false);

                fieldsEditor
                    .querySelector("#cancelEdition")
                    .parentNode.addEventListener("click",
                        () => {
                            editedReportData.PlannedExecutionsList[indexPlanificationEdited].Destination = currentReportData.PlannedExecutionsList[indexPlanificationEdited].Destination;
                            editor.hideEditor();
                        },
                        false
                    );
            });
        },
    };

    const editor = {
        addPlanification: (listContainer) => {
            const newPlanificationData = {
                Id: "newPlanification",
                Destination: "{}",
                Format: 0,
                Language: "ESP",
                PassportId: userPassportId,
                ReportId: currentReportData.Id,
                Scheduler: ("3@00:00:00@" + window.parent.moment().add(1, 'days').startOf('day').format("YYYY-MM-DD HH:mm:ss")),
                ViewFields:
                {
                    scheduledSentence: window.parent.Globalize.formatMessage("SCHEDULE_SENTENCE"),
                    description: window.parent.Globalize.formatMessage("NEW_SCHEDULE"),
                    actions: "",
                    updating: "",
                },
            };
            const planificationsInstance = $(listContainer).dxDataGrid("instance");
            editedReportData.PlannedExecutionsList.push(newPlanificationData);

            const plannificationsDataSource = currentReportData.PlannedExecutionsList.map((exec) => Object.assign({}, exec, exec.ViewFields));
            planificationsInstance.option("onContentReady", (e) => {
                const rowIndex = e.component.getRowIndexByKey("newPlanification");
                e.component.editRow(rowIndex);
                planificationsInstance.option("onContentReady", null);
            });
            planificationsInstance.option("dataSource", plannificationsDataSource);
        },
        addDestination: (param) => {
            const indexPlanificationEdited = editedReportData.PlannedExecutionsList.findIndex(
                (plan) => plan.Id === $("#reportPlannedList").dxDataGrid("instance").option("focusedRowKey")
            );

            editedReportData.PlannedExecutionsList[indexPlanificationEdited].Destination = JSON.stringify(param);

            // fill textBox with selected value
            $("#planificationDestinationTextBox")
                .dxTextBox("instance")
                .option("value", param.display || param.type);
        },
        fillUpDestinationSelector: (data) => { destinationParameters = data; },
        plannerSetValuesOfSelection: (planification) => {
            // scheduler
            const schedulerInstanceStep1 = $("#plannerScheduler .stepOne").dxRadioGroup("instance");

            const radioOptions = schedulerInstanceStep1
                .element()
                .find(".dx-widget > .dx-item.dx-radiobutton");

            const indexOfRadioInput = radioInputsSchedulerStep1.findIndex((el) => {
                const comparator = (planification.data.ViewFields.radioGroupIntervalOpt &&
                    planification.data.ViewFields.radioGroupIntervalOpt.range) ||
                    "optHours";
                return el.range === comparator;
            });

            radioOptions[indexOfRadioInput].click();
        },
        schedule: async (frequency, settings, dxEvent) => {
            if (Object.keys(settings).length < 1 || typeof frequency === "undefined" || frequency === null) {
                DevExpress.ui.dialog.alert(FALTA_COMPLETAR_ALGUNOS_CAMPOS, GUARDANDO_DATOS);
                return;
            }
            //build string
            const hour = window.parent.moment(settings.Hour.option("value"));
            let scheduleStr = `${frequency}@${hour.format("HH:mm:ss")}@`;
            let sentence = "";

            switch (frequency) {
                case 0:
                    scheduleStr += `${settings.Days.option("value")}`;
                    sentence = `Cada ${settings.Days.option(
                        "value"
                    )} días, a las ${hour.format("HH:mm")}.`;
                    break;
                case 4:
                    scheduleStr += `${1}`;
                    sentence = `Cada ${hour.format("HH:mm")} horas.`;
                    break;
                case 1:
                    scheduleStr += `${settings.Weeks.option("value")}@${settings.WeekDays
                        }`;
                    sentence = `Cada ${settings.Weeks.option("value")} semanas, los ${settings.WeekDaysSentence
                        }, a las ${hour.format("HH:mm")}.`;
                    break;
                case 2:
                    scheduleStr += `${settings.MonthlyType}@${settings.MonthlyType === 0
                        ? settings.Day.option("value")
                        : settings.Start.option("value").value +
                        "@" +
                        settings.WeekDay.option("value").value
                        }`;

                    sentence =
                        settings.MonthlyType === 0
                            ? `El día ${settings.Day.option(
                                "value"
                            )} de cada més, a las ${hour.format("HH:mm")}.`
                            : `El ${settings.Start.option("value").title} ${settings.WeekDay.option("value").title
                            } de cada més, a las ${hour.format("HH:mm")}.`;
                    break;
                case 3:
                    settings.DateTime = window.parent.moment(hour);
                    scheduleStr += `${settings.DateTime.format("YYYY/MM/DD")} 00:00:00`;
                    sentence = `El ${utils.getStringDate(
                        settings.DateTime,
                        true
                    )}, a las ${hour.format("HH:mm")}.`;
                    break;
                default:
                    break;
            }

            //take format & language
            const exportLanguage = $("#plannerLanguage").is(":empty")
                ? "---"
                : $("#plannerLanguage").dxSelectBox("instance").option("value");
            const exportFormat = $("#plannerFormat").is(":empty")
                ? 0
                : $("#plannerFormat").dxSelectBox("instance").option("value");

            const isUndefined = scheduleStr.indexOf("undefined", 0);
            if (isUndefined > -1 || !exportLanguage || typeof exportFormat !== "number") {
                DevExpress.ui.dialog.alert(FALTA_COMPLETAR_ALGUNOS_CAMPOS, GUARDANDO_DATOS);
                return;
            }

            const viewFields =
            {
                radioGroupIntervalOpt: { range: settings.range, title: settings.title },
                shceduledFrequency: frequency,
                scheduledSentence: sentence,
                scheduledParam: scheduleStr,
                exportFormat,
                exportLanguage,
            };

            const planificationsInstance = $("#reportPlannedList").dxDataGrid("instance");
            const focusedPanificationKey = planificationsInstance.option("focusedRowKey");

            if (!!focusedPanificationKey) {
                const focusedPlanificationIndex = editedReportData.PlannedExecutionsList.findIndex((exec) => exec.Id === focusedPanificationKey);

                //save properties
                if (focusedPlanificationIndex != -1) {
                    Object.assign(editedReportData.PlannedExecutionsList[focusedPlanificationIndex].ViewFields, viewFields);

                    editedReportData.PlannedExecutionsList[focusedPlanificationIndex].Scheduler = scheduleStr;
                    editedReportData.PlannedExecutionsList[focusedPlanificationIndex].Language = exportLanguage;
                    editedReportData.PlannedExecutionsList[focusedPlanificationIndex].Format = exportFormat;
                }

                //En caso que esta planificación no sea nueva y tenga parametros visibles ya configurados, preguntamos si quiere sobreescribirlos.
                if (
                    dxEvent.row.data.ParametersJson &&
                    JSON.parse(dxEvent.row.data.ParametersJson).filter(
                        (prm) => prm.TemplateName !== "hidden"
                    ).length > 0
                ) {
                    const confirms = await utils.dialogDx(
                        OVERRIDE_PLANIFICATION_PARAMS,
                        REPORT_PARAMATERS,
                        { SI: CHANGEPARAMS, NO: KEEPPARAMS, CANCELAR }
                    );

                    if (typeof confirms != 'undefined') {
                        if (confirms) {
                            askReportParams("editPlan");
                            return;
                        } else {
                            editor.acceptExecutionParams(
                                JSON.parse(
                                    currentReportData.PlannedExecutionsList[dxEvent.row.dataIndex]
                                        .ParametersJson
                                )
                            );
                        }
                    }
                }

                if (editedReportData.PlannedExecutionsList.findIndex((plan) => plan.Id === "newPlanification") > -1) {
                    askReportParams("newPlanification");
                    return;
                }

                return;
            }
        },
        selectorDatetime: (choice, initDateInput, finalDateInput) => {
            let initDate;
            let finalDate;
            const today = new Date();

            initDateInput.option("disabled", true);
            if (finalDateInput != null) finalDateInput.option("disabled", true);
            switch (choice) {
                case 0:
                    initDateInput.option("disabled", false);
                    if (finalDateInput != null) finalDateInput.option("disabled", false);
                    initDate = () => initDateInput.option("value");
                    if (finalDateInput != null) finalDate = () => finalDateInput.option("value");
                    break;
                case 1:
                    initDate = window.parent.moment(today.setDate(today.getDate() + 1));
                    if (finalDateInput != null) finalDate = initDate;
                    break;
                case 2:
                    initDate = window.parent.moment(today);
                    if (finalDateInput != null) finalDate = initDate;
                    break;
                case 3:
                    initDate = window.parent.moment(today.setDate(today.getDate() - 1));
                    if (finalDateInput != null) finalDate = initDate;
                    break;
                case 4:
                    initDate = window.parent.moment(today).startOf("week");
                    if (finalDateInput != null) finalDate = window.parent.moment(finalDate).endOf("week");
                    break;
                case 5:
                    initDate = window.parent.moment(today).subtract(1, "weeks").startOf("week");
                    if (finalDateInput != null) finalDate = window.parent.moment(today).subtract(1, "weeks").endOf("week");
                    break;
                case 6:
                    initDate = window.parent.moment(today).startOf("month");
                    if (finalDateInput != null) finalDate = window.parent.moment(finalDate).endOf("month");
                    break;
                case 7:
                    initDate = window.parent.moment(today).subtract(1, "months").startOf("month");
                    if (finalDateInput != null) finalDate = window.parent.moment(today).subtract(1, "months").endOf("month");
                    break;
                case 8:
                    initDate = window.parent.moment(today).startOf("year");
                    if (finalDateInput != null) finalDate = window.parent.moment(today);
                    break;
                case 9:
                    initDate = window.parent.moment(today).add(1, "weeks").startOf("week");
                    if (finalDateInput != null) finalDate = window.parent.moment(today).add(1, "weeks").endOf("week");
                    break;
                case 10:
                    initDate = window.parent.moment(today).add(1, "months").startOf("month");
                    if (finalDateInput != null) finalDate = window.parent.moment(today).add(1, "months").endOf("month");
                    break;
                default:
                    break;
            }

            if (choice !== 0) {
                initDateInput.option('value', initDate.format("YYYY-MM-DD"));
                if (finalDateInput != null) finalDateInput.option('value', finalDate.format("YYYY-MM-DD"));
            }
            return { initDate, finalDate };
        },
        removeExecution: (execution) => {
            const guid = execution
                .querySelector(".executionIcon > i ")
                .getAttribute("data-guid");

            editedReportData.ExecutionsList = editedReportData.ExecutionsList.filter((exec) => exec.Guid !== guid);

            editor.acceptEdition(true, "executions");
        },
        description: (description) => { editedReportData.Description = description; },
        removeViewer: (passportId) => {
            editedReportData.Viewers = editedReportData.Viewers.filter((viewer) => viewer.PassportId !== passportId);

            populator.editReportViewers(document.querySelector("#editView"), true);
        },
        addViewer: () => {
            const datalist = [
                ...document.querySelectorAll("datalist#adminusers option"),
            ];
            const selectedViewer = document.querySelector("input#addViewerInput");
            const elToAdd = datalist.find(
                (opt) => opt.value === selectedViewer.value
            );
            if (!elToAdd) {
                DevExpress.ui.dialog.alert(
                    PRIMERO_SELECIONA_USUARIO_DE_LISTA,
                    GESTIONANDO_PERMISOS
                );
                return;
            }
            const PassportId = parseInt(elToAdd.getAttribute("data-userid"), 10);
            const Name = elToAdd.value;
            const isAlreadyViewer = editedReportData.Viewers.find((viewer) => viewer.PassportId === PassportId);

            if (!isAlreadyViewer) {
                editedReportData.Viewers.push({ Name, PassportId, });
            }
            else {
                DevExpress.ui.dialog.alert(USUARIO_YA_TIENE_PERMISOS, GESTIONANDO_PERMISOS);
                return;
            }

            populator.editReportViewers(document.querySelector("#editView"), true);
            selectedViewer.value = "";
        },
        nextEdition: () => {
            editedReportData.ParametersList = utils.resolveFunctionValues(editedReportData.ParametersList);

            validation();

            if (validated) {
                editor.hideEditor();
                indexParamsSignatures++;
                askReportParams();
            }
        },
        previousEdition: () => {
            editedReportData.ParametersList = utils.resolveFunctionValues(editedReportData.ParametersList);
            editor.hideEditor();
            indexParamsSignatures--;
            askReportParams();
        },
        acceptExecutionParams: async (params = false) => {
            editedReportData.ParametersList = utils.resolveFunctionValues(params || editedReportData.ParametersList);
            validation();

            if (validated) {
                await handleExecutionTrigger(editedReportData.ParametersList);
                editor.hideEditor();
            }
        },
        acceptEdition: async (resetPanel = true, saveReportflag = "report") => {
            validation();
            if (validated) {
                const isSavedOK = await saveReportInfo(editedReportData, saveReportflag);

                if (isSavedOK) {
                    var repId = currentReportData.Id;
                    currentReportData = null;
                    editedReportData = null;
                    await loadCurrentReport(repId);
                    //updateReportActions(currentReportData.Id);
                    if (resetPanel) {
                        handleTabClick();
                        populator.singleCard(document.querySelector(".reportCard.reportCardClicked"), currentReportData);
                        await updateReportActions(currentReportData.Id);
                    }

                    return true;
                }
                else {
                    editor.cancelEdition(resetPanel);
                    DevExpress.ui.dialog.alert(ALGO_MAL_INTENTAR_GUARDAR_DATOS, GUARDANDO_DATOS);
                    return false;
                }
            }
        },
        cancelEdition: async (resetPanel = true) => {
            var indexLastValues = (currentReportData.PlannedExecutionsList).findIndex((plan) => plan.Id === $("#reportPlannedList")?.dxDataGrid("instance")?.option("focusedRowKey"));
            var isNewPlanification = currentReportData.PlannedExecutionsList[indexLastValues]?.Id === "newPlanification";

            if (currentReportData != null && !isNewPlanification) {
                var repId = currentReportData.Id;
                //currentReportData = null;
                editedReportData = null;
                await loadCurrentReport(repId);
            }

            if (resetPanel) {
                !document.querySelector("#plannerPage") && handleTabClick();
                editor.hideEditor();
            }
        },
        hideEditor: () => {
            const fieldsEditor = document.querySelector("#fieldsEditor");
            fieldsEditor.style.display = "none";
            fieldsEditor.style.width = "";
            fieldsEditor.style.height = "";
            const editView = document.querySelector("#fieldsEditor > #editView");
            if (editView) editView.remove();
        },
    };

    const utils =
    {
        translateParamName: (param) => {
            switch (param.Type) {
                case "Robotics.Base.shiftsSelector":		// HORARIOS
                    return REPORT_PARAMETER_SHIFT_DATE;
                case "Robotics.Base.holidaysSelector":		// HORARIOS
                    return REPORT_PARAMETER_HOLIDAY_DATE;
                case "Robotics.Base.conceptGroupsSelector": //GRUPO DE SALDOS
                    return REPORT_PARAMETER_CONCEPT_GROUP;
                case "Robotics.Base.conceptsSelector":		//SALDOS
                    return REPORT_PARAMETER_CONCEPTS;
                case "Robotics.Base.employeesSelector":		// EMPLEADOS
                    return REPORT_PARAMETER_SELECTOR_EMPLEADOS;
                case "Robotics.Base.zonesSelector":			//ZONAS
                    return REPORT_PARAMETER_ZONES;
                case "Robotics.Base.causesSelector":		//JUSTIFICACIONES
                    return REPORT_PARAMETER_CAUSES;
                case "Robotics.Base.incidencesSelector":		//INCIDENCIAS
                    return REPORT_PARAMETER_INCIDENCES;
                case "Robotics.Base.viewAndFormatSelector":	//VISUALIZACIÓN
                    return REPORT_PARAMETER_VIEW;
                case "Robotics.Base.accessTypeSelector":	//Tipo acceso
                    return REPORT_PARAMETER_ACCESS_TYPE;
                case "Robotics.Base.filterProfileTypesSelector":	//PERFIL DE FILTRO
                    return REPORT_PARAMETER_FILTER_PROFILE;
                case "Robotics.Base.filterSelectorCausesRegistroJL":	//CAUSES REGISTRO JORNADA LABORAL
                    return REPORT_PARAMETER_CAUSES_RJL;
                case "Robotics.Base.filterSelectorConceptsRegistroJL":	//CONCEPTS REGISTRO JORNADA LABORAL
                    return REPORT_PARAMETER_CONCEPTS_RJL;
                case "Robotics.Base.yearAndMonthSelector":	//MES Y AÑO
                    return REPORT_PARAMETER_YEAR_MONTH;
                case "Robotics.Base.betweenYearAndMonthSelector":	//RANGO MES Y AÑO
                    return REPORT_PARAMETER_BETWEEN_YEAR_MONTH;
                case "Robotics.Base.formatSelector":	//FORMATO
                    return REPORT_PARAMETER_FORMAT;
                case "Robotics.Base.filterValuesSelector":	//FILTRO POR VALORES
                    return REPORT_PARAMETER_FILTERVALUES;
                case "Robotics.Base.terminalsSelector":		// TERMINALES
                    return REPORT_PARAMETER_TERMINALS;
                case "Robotics.Base.tasksSelector":		// TASKS
                    return REPORT_PARAMETER_TASKS;
                case "Robotics.Base.userFieldsSelectorRadioBtn":
                case "Robotics.Base.userFieldsSelector":	//CAMPOS DE LA FICHA
                    return REPORT_PARAMETER_USER_FIELDS;
                case "System.DateTime":
                    return REPORT_PARAMETER_DATES;
                case "Robotics.Base.projectsVSL":	//Project
                    return REPORT_PARAMETER_PROJECT_VSL;
                default:
                    return param.Name;
            }
        },
        dialogDx: async (msg, title = "", answers = { SI, NO }) => {
            let answer;
            const buttons = [
                { text: answers.SI, onClick: (e) => (answer = true) },
                { text: answers.NO, onClick: (e) => (answer = false) },
            ];
            !!answers.CANCELAR &&
                buttons.push({ text: answers.CANCELAR, onClick: (e) => { } });

            const dialog = DevExpress.ui.dialog.custom({
                showTitle: !!title,
                title: title,
                messageHtml: msg,
                dragEnabled: true,
                buttons,
            });

            await dialog.show();

            return answer;
        },
        resolveFunctionValues: (params) =>
            params.map((prm) => {
                if (typeof prm.Value === "function") { prm.Value = prm.Value(); }
                return prm;
            }),
        viewFieldsToString: (obj) => {
            obj.PlannedExecutionsList.map((plan, index) => {
                obj.PlannedExecutionsList[index].ViewFields = JSON.stringify(plan.ViewFields);
            });

            return obj;
        },
        viewFieldsToParsed: (obj) => {
            obj.PlannedExecutionsList.map((plan, index) => {
                while (typeof obj.PlannedExecutionsList[index].ViewFields === "string") {
                    obj.PlannedExecutionsList[index].ViewFields = JSON.parse(plan.ViewFields || "{}");
                }
            });

            return obj;
        },
        getStringDate: (dateObj, justDate = false) => {
            var format = "DD-MM-YYYY"
            switch (lang) {
                case "es-ES":
                case "ca-ES":
                case "pt-PT":
                case "gl-ES":
                case "it-IT":
                case "fr-FR":
                case "eu-ES":
                    format = 'DD-MM-YYYY';
                    break;
                case "en-US":
                default:
                    format = 'MM-DD-YYYY';
                    break;
            }
            const dateFormat = justDate ? format : format + ", HH: mm";

            return window.parent.moment(dateObj).format(dateFormat);
        },
        iElementFontAwesome: (fontAwesomeClass) => {
            const iElement = document.createElement("i");
            iElement.setAttribute("class", fontAwesomeClass);

            return iElement;
        },
        getExecutionStatusClasses: (status) => {
            let statusClass;
            let iconClass;

            switch (status) {
                case 1:
                    statusClass = "executionIsInProgress";
                    iconClass = "fa fa-spinner";
                    break;
                case 4:
                    statusClass = "executionIsFuture";
                    iconClass = "fa fa-hourglass-o";
                    break;
                case 3:
                    statusClass = "executionIsError";
                    iconClass = "fa fa-times";
                    break;
                case 2:
                    iconClass = "fa fa-download";
                    statusClass = "executiuonIsFine";
                    break;
            }

            return { statusClass, iconClass };
        },
        isThisWidgetSet: (values) => {
            const setOne = values.find((val) => val !== null);
            return setOne !== null && typeof setOne !== "undefined";
        },
        removeIdNewPlanifications: () => {
            let indexNewPlanification = editedReportData.PlannedExecutionsList.findIndex(
                (plan) => plan.Id === "newPlanification"
            );
            while (indexNewPlanification > -1) {
                delete editedReportData.PlannedExecutionsList[indexNewPlanification].Id;
                indexNewPlanification = editedReportData.PlannedExecutionsList.findIndex(
                    (plan) => plan.Id === "newPlanification"
                );
            }
        },
        searchInCardsPanel: (text, feelLucky = false) => {
            const searchBox = document.querySelector("#customSearchBar");
            searchBox.value = text;
            $(searchBox).keyup();

            if (feelLucky) {
                setTimeout(() => document.querySelector(".reportCard").click(), 2000);
            }

            $(searchBox).focus();
        },
        permissionsOverUser: (permissionNeeded) =>
            parseInt(reportsPageConfig.reportManagerPermissionByUser, 10) >=
            permissionNeeded,
    };
})();