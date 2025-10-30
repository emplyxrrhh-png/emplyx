@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div id="reportPlannerContainer">
    <div class="VTpageSeparator"><span>@ReportController.GetServerLanguage().Translate("roReportPlanifications", "ReportsDX")</span></div>
    <div id="reportPlannedList">
    </div>
    <div class="VTpageSeparator"><span>@ReportController.GetServerLanguage().Translate("roReportConfiguration", "ReportsDX")</span></div>
    <div id="reportPlanOptions" style="display:none">
        <div id="planificationInfo"></div>
        <div id="plannerScheduler">
            <div class="stepOne"></div>
            <div class="stepTwo"></div>
        </div>
        <div id="plannerLanguage"></div>
        <div id="plannerFormat"></div>
        <div id="plannerDestination">
            <span id="planificationDestinationTextBox"></span>
            <span id="planificationDestination" class="reportActionBtn">@ReportController.GetServerLanguage().Translate("roReportDestination", "ReportsDX")</span>
        </div>
    </div>
</div>