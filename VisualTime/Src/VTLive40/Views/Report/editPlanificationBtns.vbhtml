@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div class="editPlanificationBtns">
    <div class="editBtn"><span id="startEdition">@ReportController.GetServerLanguage().Translate("roReportBtnEdit", "ReportsDX")</span></div>
    <div class="editBtn"><span id="removePlanification">@ReportController.GetServerLanguage().Translate("roReportBtnRemove", "ReportsDX")</span></div>
    <div class="editBtn" style="display:none"><span id="acceptEdition">@ReportController.GetServerLanguage().Translate("roReportBtnAccept", "ReportsDX")</span></div>
    <div class="editBtn" style="display:none"><span id="cancelEdition">@ReportController.GetServerLanguage().Translate("roReportBtnCancel", "ReportsDX")</span></div>
</div>