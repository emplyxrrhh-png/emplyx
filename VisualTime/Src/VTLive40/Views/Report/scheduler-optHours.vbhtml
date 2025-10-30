@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<span class="schedulerTxt" style="text-transform:capitalize">@ReportController.GetServerLanguage().Translate("roReportSchedulerEvery", "ReportsDX") </span><span class="opt1" style="width: 135px"></span><span class="schedulerTxt"> @ReportController.GetServerLanguage().Translate("roReportSchedulerHours", "ReportsDX").</span>