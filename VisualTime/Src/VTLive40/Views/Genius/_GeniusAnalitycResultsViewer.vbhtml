@Imports Robotics.Base.DTOs

@ModelType roGeniusView
@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<section id="geniusAnalyticLauncher">
    <form id="mainForm">
        <div class="panHeader2 panBottomMargin">
            <span class="panelTitleSpan">
                <span id="">@Html.Raw(labels("Genius#OldGenius"))</span>
            </span>
        </div>
        <div class="runsDescription">
            <span id="">@Html.Raw(labels("Genius#LastRunsInfo"))</span>
        </div>
        <div>
            <div id="geniusExecutions" style="margin-top:10px;"></div>
            <div id="geniusExecutionsBI" style="margin-top:10px;"></div>
            <div id="toastBILinkCopied"></div>
            <div id="toastBIDeleted"></div>
        </div>
    </form>
</section>