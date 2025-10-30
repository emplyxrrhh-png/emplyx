@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div style="margin-top: -200px;" id="editView">
    <form id="selectorString"
          onchange="collectParamValues()"
          onsubmit="preventDefaultOnSubmit(event)"
          action="#"
          style="">
        <label for="parameterString">@ReportController.GetServerLanguage().Translate("roReportSelectorIntroduce", "ReportsDX") <b>@ReportController.GetServerLanguage().Translate("roReportSelectorText", "ReportsDX")</b></label><input onchange="collectParamValues()" id="parameterString" type="text" value="" />
    </form>
</div>