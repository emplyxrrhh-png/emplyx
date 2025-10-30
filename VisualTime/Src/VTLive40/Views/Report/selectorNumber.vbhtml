@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div style="margin-top: -200px;" id="editView">
    <form id="selectorNumber"
          onsubmit="preventDefaultOnSubmit(event)"
          style="">
        <label for="parameterNumber">@ReportController.GetServerLanguage().Translate("roReportSelectorIntroduce", "ReportsDX") <b>@ReportController.GetServerLanguage().Translate("roReportSelectorNumero", "ReportsDX")</b></label><input onchange="collectParamValues()" id="parameterNumber" type="number" value="" />
    </form>
</div>