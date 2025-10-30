@Imports Robotics.Base.DTOs

@ModelType roGeniusView
@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim flexComponentFolder = Url.Content("~/Base/Flexmonster/")
    Dim flexReportSrc = Url.Content("~/Genius/GetGeniusResult/")
End Code

<script>var FLEX_BASE_URL ="@flexComponentFolder"; var FLEX_REPORT_URL ="@flexReportSrc"; </script>
<script>var shortFormat = "@Html.Raw(ViewBag.Format)"</script>

<section id="geniusAnalyticViewer" style="display:none">
    @*<div class="right">
            @(Html.DevExtreme().Button() _
                                .Icon("back") _
                                .OnClick("function() { backToLauncher(); }"))
        </div>*@

    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("Genius#Results"))</span>
        </span>
    </div>
    <div style="min-height:15px"></div>

    <div id="flxAnalytic">
    </div>
</section>