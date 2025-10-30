@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div id="editView">
    <div id="editDiv">
        <textarea id="descriptionEdition" placeholder="@ReportController.GetServerLanguage().Translate("roReportDescriptionTextareaPlaceholder", "ReportsDX")" style="margin: 0px; width: 600px; height: 150px;"></textarea>
    </div>
    @Html.Partial("editViewBtns")
</div>