@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div id="parametersTitle" class="VTpageSeparator">
    <span>@ReportController.GetServerLanguage().Translate("roReportSelectorStep", "ReportsDX") <span id="parametersCounter"></span>/<span id="parametersCounterTotal"></span> @ReportController.GetServerLanguage().Translate("roReportSelectorParamsIntroductionToExecuteReport", "ReportsDX")</span>
</div>
<span id="parametersName">@ReportController.GetServerLanguage().Translate("roReportSelectorParamName", "ReportsDX"): <b></b></span>