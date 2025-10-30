@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
    Dim baseURL = Url.Content("~")
    Dim ReportsIcon = baseURL & "ReportScheduler/Images/ReportScheduler80.png"
End Code

<div id="divTabInfo" class="divDataCells">
    <div id="divTabCommuniques" class="blackRibbonTitle">
        <div class="blackRibbonIcon">
            <img src="@ReportsIcon" alt="" height="100" width="100">
        </div>
        <div class="blackRibbonDescription">
            <h1 id="ctl00_contentMainBody_lblHeader" class="NameText" style="margin:10px 0;font-size:20px;">@ReportController.GetServerLanguage().Translate("reports", "ReportsDX")</h1>
        </div>
        <div Class="switchMainViewTabs">
            @Html.Partial("switchMainViewTabs")
        </div>
    </div>
</div>