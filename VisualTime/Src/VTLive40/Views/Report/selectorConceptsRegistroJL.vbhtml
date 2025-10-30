@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code
<div style="height:400px;" id="editView">
    <form id="selectorConceptsRegistroJL" onsubmit="preventDefaultOnSubmit(event)" style="">

        <div>
            <p>@ReportController.GetServerLanguage().Translate("roReportDesignerTitleOpcionalConcepts", "ReportsDX")</p>
            <div id="showConcepts"></div>
        </div>
        <div class="selectores">
            <span class="comboHelperText">
                @ReportController.GetServerLanguage().Translate("roReportDesignerSaldos", "ReportsDX")
            </span>
            <div id="selectConcepts"></div>
        </div>
    </form>
</div>