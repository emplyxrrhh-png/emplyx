@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div id="reportExecContainer" style="display:none">
    <div class="VTpageSeparator"><span>@ReportController.GetServerLanguage().Translate("roReportLastExecutions", "ReportsDX")</span></div>
    <div id="reportsHistoricContainer">
        <ul class=reportsHistoric></ul>
    </div>
</div>