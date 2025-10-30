@imports Robotics.Web.Base.ReportSvc

@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
End Code

<div id="communiqueMessageContainer">
    <div id="messageCommunique" style="display:none">
        <div class="widgetForm"></div>
    </div>
</div>
<div id="communiqueConfigContainer">
    <div id="configCommunique">
        <div class="widgetForm"></div>
    </div>
</div>
<div id="communiqueStatisticsContainer">
    <div class="VTpageSeparator" style="display:none"><span>@CommuniqueController.GetServerLanguage().Translate("Statistics", "Communique")</span></div>
    <div id="statisticsCommunique" style="display:none">
        <div class="widgetStatistics"></div>
    </div>
</div>

<div id="divCommuniqueEmployeeSelector">
</div>