@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<span class="reportActionBtn" id="triggerExecutionBtn" title="@ReportController.GetServerLanguage().Translate("roReportBtnExecuteHelpText", "ReportsDX")">
    @ReportController.GetServerLanguage().Translate("roReportBtnExecute", "ReportsDX")
</span>
<!--span class="reportActionBtn" id="planExecutionBtn" title="@ReportController.GetServerLanguage().Translate("roReportBtnPlanHelpText", "ReportsDX")">
    @ReportController.GetServerLanguage().Translate("roReportBtnPlan", "ReportsDX")
</span-->