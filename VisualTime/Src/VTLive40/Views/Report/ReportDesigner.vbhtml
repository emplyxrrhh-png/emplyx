@imports DevExpress.DataAccess.ConnectionParameters
@imports DevExpress.DataAccess.Sql

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"
    ViewData("Title") = "ReportDesigner"

    Dim oConnString As MsSqlConnectionParameters = New VTLive40.ReportController().GetReportDesignerConnectionParameters()
    Dim redirectPath As String = New VTLive40.ReportController().GetRedirectPath().Replace(",@@mvcPath@@", "")
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
    Dim baseURL = Url.Content("~")
End Code

<script>var BASE_URL ="@baseURL"; </script>

<script src="@Url.Content("~/Base/Scripts/Live/Report/roReportDesignerScript.min.js")" type="text/javascript"></script>

<link rel="stylesheet" type="text/css" href="@Url.Content("~/Base/Styles/Live/Report/reportDesigner.min.css")">

@*Exit SVG Icon for the report designer view*@
<script type="text/html" id="exit">
    <svg data-bind="svgAttrs" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
        <path style="fill:#ec3d3d" d="m23.558982,10.662391c-0.049341,-0.117931 -0.119952,-0.225216 -0.209928,-0.315103l-2.901484,-2.898631c-0.378269,-0.376946 -0.989731,-0.376946 -1.368,0c-0.378269,0.377897 -0.378269,0.989709 0,1.366655l1.25095,1.24972l-5.404383,0c-0.535001,0 -0.967464,0.432989 -0.967464,0.966512s0.432463,0.966512 0.967464,0.966512l5.404337,0l-1.25095,1.24972c-0.378269,0.377897 -0.378269,0.989709 0,1.366655c0.188658,0.189424 0.436318,0.283208 0.684023,0.283208s0.495364,-0.093738 0.684023,-0.283208l2.901484,-2.898631c0.089976,-0.088936 0.160586,-0.19622 0.209928,-0.315103c0.097685,-0.235727 0.097685,-0.502489 0,-0.738306z" />
        <path style="fill:#ec3d3d" d="m16.86111,13.931399c-0.535001,0 -0.967464,0.432989 -0.967464,0.966512l0,4.832653l-3.8699,0l0,-15.464472c0,-0.426239 -0.280584,-0.803184 -0.689828,-0.925918l-3.360069,-1.007107l7.919797,0l0,4.832653c0,0.533523 0.432463,0.966512 0.967464,0.966512s0.967464,-0.432989 0.967464,-0.966512l0,-5.799165c0,-0.533569 -0.432463,-0.966558 -0.967464,-0.966558l-15.479647,0c-0.034829,0 -0.065804,0.014498 -0.099635,0.018349c-0.045487,0.004848 -0.087073,0.01255 -0.13061,0.023197c-0.101585,0.026096 -0.193511,0.065739 -0.279586,0.119834c-0.021269,0.013546 -0.047391,0.014498 -0.067708,0.029947c-0.0078,0.005844 -0.010703,0.016491 -0.018458,0.022291c-0.10544,0.083091 -0.193466,0.184577 -0.257319,0.305408c-0.01356,0.026096 -0.016462,0.054141 -0.027074,0.081188c-0.030974,0.073441 -0.064806,0.144979 -0.076416,0.226167c-0.004853,0.028996 0.003855,0.056043 0.002902,0.084088c-0.000952,0.019346 -0.01356,0.036743 -0.01356,0.056043l0,19.330567c0,0.461034 0.326025,0.857325 0.777853,0.947212l9.674774,1.93307c0.062901,0.013546 0.126755,0.019346 0.189611,0.019346c0.221538,0 0.43922,-0.076341 0.613367,-0.219417c0.223488,-0.183625 0.354097,-0.457183 0.354097,-0.747141l0,-0.966512l4.83741,0c0.535001,0 0.967464,-0.432989 0.967464,-0.966512l0,-5.799211c0,-0.533523 -0.432463,-0.966512 -0.967464,-0.966512z" />
    </svg>
</script>
<script type="text/html" id="descriptionEdit">
    <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Capa_1" x="0px" y="0px" width="44.688px" height="44.688px" viewBox="0 0 44.688 44.688" style="enable-background:new 0 0 44.688 44.688;" xml:space="preserve">
    <path style="fill:#606060" d="M25.013,39.119c-0.336,0.475-0.828,0.82-1.389,0.975l-2.79,0.762c-0.219,0.062-0.445,0.094-0.673,0.094    c-0.514,0-1.011-0.157-1.43-0.452c-0.615-0.428-1.001-1.103-1.062-1.834l-0.245-2.881c-0.058-0.591,0.101-1.183,0.437-1.659    l0.103-0.148H8.012c-0.803,0-1.454-0.662-1.454-1.463c0-0.804,0.651-1.466,1.454-1.466h12.046l2.692-3.845H8.012    c-0.803,0-1.454-0.662-1.454-1.465s0.651-1.465,1.454-1.465l16.811-0.043l6.304-9.039V8.497c0-1.1-0.851-1.988-1.948-1.988h-4.826    v3.819c0,1.803-1.474,3.229-3.274,3.229h-9.706c-1.804,0-3.227-1.427-3.227-3.229V6.509H3.268c-1.099,0-1.988,0.889-1.988,1.988    V42.65c0,1.1,0.89,2.037,1.988,2.037h25.909c1.1,0,1.949-0.938,1.949-2.037V30.438L25.013,39.119z M8.012,17.496h16.424    c0.801,0,1.453,0.661,1.453,1.464c0,0.803-0.652,1.465-1.453,1.465H8.012c-0.803,0-1.454-0.662-1.454-1.465    C6.558,18.157,7.209,17.496,8.012,17.496z" />

    <path style="fill:#606060" d="M11.4,11.636h9.697c0.734,0,1.331-0.596,1.331-1.332V4.727c0-0.736-0.597-1.332-1.331-1.332h-1.461    C19.626,1.52,18.102,0,16.223,0c-1.88,0-3.402,1.519-3.413,3.395H11.4c-0.736,0-1.331,0.596-1.331,1.332v5.576    C10.069,11.039,10.664,11.636,11.4,11.636z M16.224,1.891c0.835,0,1.512,0.672,1.521,1.505H14.7    C14.71,2.563,15.388,1.891,16.224,1.891z" />

    <path style="fill:#606060" d="M43.394,8.978c-0.045-0.248-0.186-0.465-0.392-0.609l-2.428-1.692c-0.164-0.115-0.353-0.17-0.539-0.17    c-0.296,0-0.591,0.14-0.772,0.403L22.064,31.573l3.973,2.771L43.238,9.682C43.38,9.477,43.437,9.224,43.394,8.978z" />

    <path style="fill:#606060" d="M19.355,35.6l0.249,2.896c0.012,0.167,0.101,0.316,0.236,0.412c0.096,0.066,0.209,0.104,0.321,0.104    c0.049,0,0.099-0.007,0.147-0.021l2.805-0.768c0.127-0.035,0.237-0.113,0.313-0.22l1.053-1.51l-3.976-2.772l-1.053,1.51    C19.376,35.338,19.341,35.469,19.355,35.6z" />
    </svg>
</script>

<script type="text/html" id="custom-parameter-editor">
    <div data-bind="dxTextBox: { value: value }, dxValidator: { validationRules: []}"></div>
</script>

@*Function to add the exit button and the "before-closing" behaviour*@
<script type="text/javascript">
    function onReportSaved(s, e)
    {
        if (!!window.closeAfterSaving) @Html.Raw(redirectPath);
    }

    function onServerError(s, e)
    {
        window.closeAfterSaving = false ;
    }

    function customizeDesigner(s, e)
    {
        window.reportDesignerObject = ASPxClientReportDesigner.Cast(s);

        var action = e.GetById("dxrd-newreport");
        if (action) action.visible = false;

        action = e.GetById("dxrd-exit");
        if (action) action.visible = false;

        action = e.GetById("dxrd-localization-editor");
        if (action) action.visible = false;

        action = e.GetById("dxrd-run-wizard");
        if (action) action.visible = false;

        action = e.GetById("dxrd-newreport-via-wizard");
        if (action) action.visible = false;

        action = e.GetById("dxrd-open-report");
        if (action) action.visible = false;

        action = e.GetById("dxrd-add-sql-datasource");
        if (action) action.visible = false;

        action = e.GetById("dxrd-add-multi-query-sql-datasource");
        if (action) action.visible = false;

        //ADD CUSTOM BUTTONS ______________________________________________________________________________________
        e.Actions.push(
            {
                text: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDescription", "ReportsDX"))",
                imageTemplateName: "descriptionEdit",
                visible: true,
                disabled: false,
                hasSeparator: false,
                clickAction: function () { editReportDescription(); }
            },
            {
                text: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerExit", "ReportsDX"))",
                imageTemplateName: "exit",
                visible: true,
                disabled: false,
                hasSeparator: false,
                clickAction: function ()
                {
                    let hasAnyDirtyTabs = reportDesignerObject.designerModel.getTabs()
                                                                            .map(tab => tab.isDirty())
                                                                            .reduce((acc = false, curr) => acc || curr);

                    if (reportDesignerObject.GetTabs().length > 1)
                    {
                        if (hasAnyDirtyTabs)
                        {
                            var dialog = DevExpress.ui.dialog.custom({
                                title:       "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerAttention", "ReportsDX"))",
                                messageHtml: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerPendingChangesToSave", "ReportsDX"))",
                                buttons: [
                                    { text: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerExitWithoutSave", "ReportsDX"))", onClick: () => { @Html.Raw(redirectPath) } },
                                    { text: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerBack", "ReportsDX"))" }
                                ]
                            });
                            dialog.show();
                        }
                        else
                        { @Html.Raw(redirectPath)}
                    }
                    else
                    {
                        if (hasAnyDirtyTabs)
                        {
                            var dialog = DevExpress.ui.dialog.custom({
                                title:       "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerDoYouWantSaveChanges", "ReportsDX"))",
                                messageHtml: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerPendingChangesToSave", "ReportsDX"))",
                                buttons: [
                                    { text: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportBtnSave", "ReportsDX"))",
                                      onClick: () => {
                                                        e.Actions.filter(function (x) { return x.text === 'Save' })[0].clickAction();
                                                        window.closeAfterSaving = true;
                                                       }
                                    },
                                    { text: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportBtnNoSave", "ReportsDX"))", onClick: () => { @Html.Raw(redirectPath) } },
                                    { text: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerBack", "ReportsDX"))" }
                                ]
                            });
                            dialog.show();
                        }
                        else
                        { @Html.Raw(redirectPath) }
                    }
                }
            }
        );

        //ADD CUSTOM PARAMETER TYPES ______________________________________________________________________________________
        s.AddParameterType(// Empleados  _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.employeesSelector",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerEmployees", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( //Campos de la ficha _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.userFieldsSelector",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerFileFields", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( //Campos de la ficha _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.userFieldsSelectorRadioBtn",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerFileFieldsRadioBtn", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Grupos de saldos _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.conceptGroupsSelector",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerBalancesGroups", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Saldos   _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.conceptsSelector",
                displayValue: "Saldos",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(// Justificaciones _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.causesSelector",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerJustifications", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(// Incidencias _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.incidencesSelector",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerIncidences", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Visualización  _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.viewAndFormatSelector",
                displayValue: "Configuración de visualización",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Tipo de acceso  _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.accessTypeSelector",
                displayValue: "Tipo de acceso",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Perfiles de filtro  _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.filterProfileTypesSelector",
                displayValue: "Perfiles de filtro",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Causes Registro Jornada Laboral  _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.filterSelectorCausesRegistroJL",
                displayValue: "Causes R. Jornada Laboral",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Concepts Registro Jornada Laboral  _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.filterSelectorConceptsRegistroJL",
                displayValue: "Concepts R. Jornada Laboral",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Mes y Año  _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.yearAndMonthSelector",
                displayValue: "YearMonth",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Rango Fechas Mes y Año  _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.betweenYearAndMonthSelector",
                displayValue: "BetweenYearMonth",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Formato _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.formatSelector",
                displayValue: "Formato",// "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerFormat", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Filtro por valores _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.filterValuesSelector",
                displayValue:"@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerFilterValues", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(// Horarios _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.shiftsSelector",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerSchedules", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(// Horarios _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.holidaysSelector",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerHolidays", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(// Terminales _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.terminalsSelector",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerTerminals", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(// Tasks _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.tasksSelector",
                displayValue: 'Tasks',
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(//Zonas _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.zonesSelector",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerZones", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(//Identificador de usuario _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.passportIdentifier",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerUserId", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(//Identificador de saldo   _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.conceptIdentifier",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportConceptId", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(//Identificador de justificación   _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.causeIdentifier",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportCauseId", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(//Identificador de incidencia   _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.incidenceIdentifier",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportIncidenceId", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(//Identificador de campo de la ficha   _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.userFieldIdentifier",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportUserFieldId", "ReportsDX"))",
                specifics: "integer",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( // Projects VSL  _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.projectsVSLSelector",
                displayValue: "ProjectsVSL",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType(//(system) TaskId _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.taskIdentifier",
                displayValue: "(system) TaskId",
                specifics: "integer",
                valueConverter: function(valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
        s.AddParameterType( //(system) TmpTable _ _ _ _ _ _ _ _ _ _ _ _ _
            {
                value: "Robotics.Base.functionCall",
                displayValue: "@Html.Raw(ReportController.GetServerLanguage().Translate("roReportDesignerFunctionCall", "ReportsDX"))",
                specifics: "string",
                valueConverter: function (valueObj) { return valueObj; }
            },
            { header: "custom-parameter-editor" }
        );
    }
</script>

<div id="divReportDesigner" runat="server" style="text-align: left; vertical-align: top; padding: 0px; height: 100%; display: block;">
    <div style="width: 100%; height: 100%; padding: 0px;">
        <div class="RoundCornerFrame roundCorner" style="height:100%">
            <div style="height:100%">
                @Html.DevExpress().ReportDesigner(Sub(settings)
                                                      settings.Name = "ReportDesigner"

                                                      settings.Style.Add("height", "100%")

                                                      Dim ds As New SqlDataSource(oConnString)
                                                      ds.RebuildResultSchema()
                                                      settings.DataSources.Add("VisualTime", ds)

                                                      settings.ClientSideEvents.CustomizeMenuActions = "customizeDesigner"
                                                      settings.ClientSideEvents.ReportSaved = "onReportSaved"
                                                      settings.ClientSideEvents.OnServerError = "onServerError"
                                                  End Sub).Bind(Model).GetHtml()
            </div>
        </div>
    </div>
</div>