@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code
<div style="height:400px;" id="editView">
    <form id="selectorBetweenYearAndMonth" onsubmit="preventDefaultOnSubmit(event)" style="">
        <div style="margin-bottom: 2rem;">
            <label>@ReportController.GetServerLanguage().Translate("roReportBetweenYearAndMonthStart", "ReportsDX")</label>
            <div id="monthStart"></div>
            <div id="yearStart" style="margin-top: 1rem;"></div>
        </div>
        <div>
            <label>@ReportController.GetServerLanguage().Translate("roReportBetweenYearAndMonthEnd", "ReportsDX")</label>
            <div id="monthEnd"></div>
            <div id="yearEnd" style="margin-top: 1rem;"></div>
        </div>
    </form>
</div>