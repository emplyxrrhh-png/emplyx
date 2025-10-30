@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<span class="schedulerTxt" style="text-transform:capitalize">@ReportController.GetServerLanguage().Translate("roReportSchedulerEvery", "ReportsDX") </span><span class="opt1" style="width: 80px"></span><span class="schedulerTxt"> @ReportController.GetServerLanguage().Translate("roReportSchedulerWeeks", "ReportsDX"), @ReportController.GetServerLanguage().Translate("roReportSchedulerOn", "ReportsDX") </span><span class="opt2" style="width: 180px"></span><span class="schedulerTxt">,@ReportController.GetServerLanguage().Translate("roReportSchedulerAt", "ReportsDX") </span><span class="opt3" style="width: 135px"></span>