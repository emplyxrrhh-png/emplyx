@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div style="margin-top: -200px;" id="editView">
    <form id="selectorDate"
          onchange="collectParamValues()"
          onsubmit="preventDefaultOnSubmit(event)"
          style="">

        <div id="datetimeSelects">
            <div>
                <input id="libre0" type="radio" value="0" name="period" checked />
                <label for="libre0">@ReportController.GetServerLanguage().Translate("roReportSelectorInsertData", "ReportsDX") <b>@ReportController.GetServerLanguage().Translate("date", "saldosmesames")</b></label>
                <div id="parameterDate" style="display: inline-block; margin-left: 20px;"></div>
            </div>
            <div>
                <input id="hoy2" type="radio" value="2" name="period" />
                <label for="hoy2" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorToday", "ReportsDX")</label>
            </div>
            <div>
                <input id="ayer3" type="radio" value="3" name="period" />
                <label for="ayer3" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorYesterday", "ReportsDX")</label>
            </div>
        </div>
        <p id="hintDataSelection" style="color: #ff5c35;padding-left: 6px;max-width: 370px;opacity: 0;">@ReportController.GetServerLanguage().Translate("roReportSelectorDateHintMessage", "ReportsDX")</p>
    </form>
</div>