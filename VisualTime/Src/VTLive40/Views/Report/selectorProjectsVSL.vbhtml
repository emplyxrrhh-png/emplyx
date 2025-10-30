@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code
<div style="height:400px;" id="editView">
    <form id="selectorProjectsVSL" onsubmit="preventDefaultOnSubmit(event)" style="">
        <div style="display: grid; grid-template-columns: auto auto; align-items: center; column-gap: 1rem;">
            <span>@ReportController.GetServerLanguage().Translate("roReportProjectsStart", "ReportsDX"): </span><div id="startProject"></div>
            <span>@ReportController.GetServerLanguage().Translate("roReportProjectsEnd", "ReportsDX"): </span><div id="endProject" style="margin-top: 1rem;"></div>
        </div>
    </form>
</div>