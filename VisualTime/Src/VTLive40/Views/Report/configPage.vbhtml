@imports Robotics.Base.DTOs

@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div id="reportConfigContainer" style="display:none">
    <div class="VTpageSeparator"><span>@ReportController.GetServerLanguage().Translate("roReportDetails", "ReportsDX")</span></div>
    <div id="reportInfo">
        <!--<h2>Nombre del Informe</h2>-->
        <h4>@ReportController.GetServerLanguage().Translate("roReportTitleName", "ReportsDX"):</h4>
        <p style="display:inline-block">
            <span id="reportTitle"></span>
        </p>
        <br />
        <h4>@ReportController.GetServerLanguage().Translate("roReportDescription", "ReportsDX"):</h4>
        <p style="display:inline-block">
            <span id="reportDescription"></span>
        </p>
        <p style="display: none;">
            <b>@ReportController.GetServerLanguage().Translate("roReportCreated", "ReportsDX"):</b>
            <span id="reportCreationDate">15/02/2018</span>
            @ReportController.GetServerLanguage().Translate("roReportCreatedBy", "ReportsDX")
            <span id="reportCreationAuthor">usuario</span>
        </p>
        <!--<p>
        <b>Usuarios con permiso:</b>
        <span id="reportViewers"></span>
        <span id="editReportViewers" class="editFieldBtn"><i class="fa fa-plus-square"></i></span>
        </p-->
        <div Class="reportCategories" id="">
            <span id="categoriesLabel">@ReportController.GetServerLanguage().Translate("roCategories", "ReportsDX")</span>
            <div id="catReport" hidden>
                @Join(ReportController.GetAvailableCategories().Select(Of String)(Function(Cat) Cat.Description).ToArray, ", ")
            </div>
            <div id="listCategory" class="categoryText"></div>
            <div id="levelCategory" class="levelText"></div>
            <div id="listCategoryEditable" class="categoryText"></div>
            <div id="levelCategoryEditable" class="levelText"></div>
        </div>
    </div>
    @Html.Partial("pdfViewer")
    <div Class='btnReportDetail executionActions'>
        @Html.Partial("listTabReportActions")
    </div>
</div>