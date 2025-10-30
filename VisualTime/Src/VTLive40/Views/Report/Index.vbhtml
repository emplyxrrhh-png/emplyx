@imports Robotics.Web.Base.API

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"
    ViewData("Title") = "Reports"
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
    Dim CategoriesList As String = ReportController.GetReportCategoriesAsJson()
    Dim scriptVersion As String = ReportController.ScriptsVersion
    Dim redirectPath As String = ReportController.GetRedirectPath().Replace("@@mvcPath@@", "controllerFunction+reportId")
    Dim reportManagerPermissionsByUser = SecurityServiceMethods.GetPermissionOverFeature(Nothing, "Reports", "U")
    Dim baseURL = Url.Content("~")
    Dim lang = ReportController.GetServerLanguage().GetLanguageKey()
End Code

<script>var BASE_URL ="@baseURL"; </script>
<script>var lang ="@lang"; </script>

<link href="@Url.Content("~/Base/Styles/Live/Report/reportsCards.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<script src="@Url.Content("~/Base/Scripts/Live/Report/roReportLayoutsScript.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/rocontrols/roTrees/roTreeState.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<script>
    var categoriesTree = @Html.Raw(CategoriesList);
    function openDesignerLink(controllerFunction = "/ReportDesigner/", reportId = "") { @Html.Raw(redirectPath); }
</script>
@Html.Partial("dataTabInfo")
<div id="divMainBody">
    <!-- TAB SUPERIOR -->
    <div id="reportCategoryTree" hidden>
        @Html.Partial("categoriesMenu")
    </div>
    <div id="divTabData" Class="divDataCells">
        <div id="divContenido" Class="divAllContent twoWindowsFlexLayout">
            <div Class="twoWindowsSidePanel maxHeight treeSize" id="divTree">
                <div class="treeCaption"><span>@ReportController.GetServerLanguage().Translate("reports", "ReportsDX")</span></div>
                @Html.Partial("searchPanelReports")
                @Html.Partial("cardView", Model)
            </div>
            <div id="divButtons" class="divMiddleButtons">
                @Html.Partial("dataTabBarButtons")
            </div>
            <div Class="twoWindowsMainPanel maxHeight divRightContent">
                @Html.Partial("saveOrDiscartChanges")
                <div id="mainPanelDisplay">
                    @Html.Partial("configPage")
                    @Html.Partial("listTab")
                    <div id="fieldsEditor" style="display:none"></div>
                </div>
            </div>
        </div>
    </div>
</div>