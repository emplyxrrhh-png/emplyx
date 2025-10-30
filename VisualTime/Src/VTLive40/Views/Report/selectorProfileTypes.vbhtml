@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code
<div style="height:400px;" id="editView">
    <form id="selectorProfileTypes" onsubmit="preventDefaultOnSubmit(event)" style="">

        <div>
            <p>@ReportController.GetServerLanguage().Translate("roReportProfileTypesDescription1", "ReportsDX")</p>
            <div id="profileTypes"></div>
        </div>
        <div class="selectores">
            <p>@ReportController.GetServerLanguage().Translate("roReportProfileTypesDescription2", "ReportsDX")</p>
            <div id="selectConcepts"></div>
            <span class="comboHelperText">
                @ReportController.GetServerLanguage().Translate("roReportDesignerIncidences", "ReportsDX")
            </span>
            <div id="selectIncidences"></div>
            <span class="comboHelperText">
                @ReportController.GetServerLanguage().Translate("roReportDesignerJustifications", "ReportsDX")
            </span>
            <div id="selectCauses"></div>
        </div>
        <p>@ReportController.GetServerLanguage().Translate("roReportProfileTypesDescription3", "ReportsDX")</p>
        <div class="criterios">
            <span>@ReportController.GetServerLanguage().Translate("roReportProfileTypesThat", "ReportsDX")</span>
            <div id="criterio"></div>
            <span style="display:none;">@ReportController.GetServerLanguage().Translate("roReportProfileTypesTheValue", "ReportsDX")</span>
            <div id="tipoValorCriterio"></div>
            <div id="valorCriterio"></div>
            <div id="valorFichaCriterio"></div>
            <div id="rangoCriterio"></div>
        </div>
    </form>
</div>