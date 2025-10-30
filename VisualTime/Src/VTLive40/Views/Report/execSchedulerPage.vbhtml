@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div id="execSchedulerPage" style="display:none">
    <div class="VTpageSeparator"><span>@ReportController.GetServerLanguage().Translate("roReportBtnExecute", "ReportsDX")</span></div>
    @Html.Partial("listTabReportActions")
</div>