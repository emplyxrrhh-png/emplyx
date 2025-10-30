@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div class="editViewBtns">
    <div class="editBtn" style="display:none; margin-left:auto;"><span id="previousEdition">@ReportController.GetServerLanguage().Translate("roReportBtnPreviows", "ReportsDX")</span></div>
    <div class="editBtn" style="display:none"><span id="nextEdition">@ReportController.GetServerLanguage().Translate("roReportBtnNext", "ReportsDX")</span></div>
    <div class="editBtn"><span id="acceptEdition">@ReportController.GetServerLanguage().Translate("roReportBtnAccept", "ReportsDX")</span></div>
    <div class="editBtn"><span id="cancelEdition">@ReportController.GetServerLanguage().Translate("roReportBtnCancel", "ReportsDX")</span></div>
</div>