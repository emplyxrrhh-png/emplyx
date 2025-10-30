@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div id="editView">
    <div id="editDiv">
        <h4 style="color:#FF5C35;">@ReportController.GetServerLanguage().Translate("roReportAddRemovePermissionsToUser", "ReportsDX")</h4>
        <ul id="viewersList">
            <li id="addViewer">
                <label id="adminuser">@ReportController.GetServerLanguage().Translate("roReportUsers", "ReportsDX")</label>
                <input id="addViewerInput" list="adminusers" name="adminuser">

                <datalist id="adminusers">
                    <option value="Internet Explorer">
                </datalist>
                <i id="addViewerBtn" class="fa fa-plus-circle"></i>
            </li>
        </ul>
    </div>
    @Html.Partial("editViewBtns")
</div>