@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<span class="reportActionBtn" id="goBackBtn">
    <a href="#">&lt; @ReportController.GetServerLanguage().Translate("roReportDesignerBack", "ReportsDX")</a>
</span>
<span class="reportActionBtn" id="newPlanificationBtn">
    <a href="#">@ReportController.GetServerLanguage().Translate("roReportBtnNewPlanification", "ReportsDX")</a>
</span>