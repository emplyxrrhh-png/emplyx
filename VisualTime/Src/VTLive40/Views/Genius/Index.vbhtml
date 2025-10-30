@imports Robotics.Web.Base.API

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"

    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim baseURL = Url.Content("~")
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim viewIcon = ViewData(VTLive40.Helpers.Constants.DefaultViewIcon)
    Dim viewTitle = ViewData(VTLive40.Helpers.Constants.DefaultViewTitle)
    Dim viewDescription = ViewData(VTLive40.Helpers.Constants.DefaultViewDescription)
    Dim callbackFunc = "viewUtilsManager.changeTab"
    Dim barButtonData = ViewData(VTLive40.Helpers.Constants.DefaultBarButtonData)
    Dim customChangeTab = ViewData(VTLive40.Helpers.Constants.CustomChangeTab)
    If customChangeTab Is Nothing OrElse customChangeTab.Equals("") Then
        customChangeTab = "undefined"
    End If

End Code

<script>
    var BASE_URL = "@baseURL";
    var SelectedTask = "@Html.Raw(ViewBag.SelectedTask)";
    var _LCODE_ = "@Html.Raw(ViewBag.LCode)";
    var isConsultor = "@Html.Raw(ViewBag.IsConsultor)";
    var isGeniusAdvanced = "@Html.Raw(ViewBag.AdvancedGenius)";
    var concepts = @Html.Raw(Json.Encode(ViewBag.lstConcepts));
    var jsLabels = null;
    var BIIntegration = "@Html.Raw(ViewBag.BIIntegration)";
</script>
<link href="@Url.Content("~/Content/bootstrap.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/liveMVC.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/Genius/roGenius.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/flexmonster/flexmonster.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Base/flexmonster/flexmonster.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Scripts/jszip.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Genius/roGeniusScript.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<div id="divScreenTabsGeneric">
    <div class="dx-field">

        <div class="ScreenIconTitleGeneric">
            <img src="@viewIcon" alt="Screen Icon" height="80" width="80">
            <div>
                <div>
                    <h1 id="ctl00_contentMainBody_lblHeader" class="NameText" style="margin: 10px 0 0 0;font-size: 20px;margin-left: 10px;font-weight: 600;"> @Html.Raw(viewTitle)</h1>
                </div>
                @If viewDescription.length > 0 Then
                    @<div style="margin-left: 10px"><span id="readOnlyDescritionCompany"> @Html.Raw(viewDescription)</span></div>
                End If
            </div>
        </div>

        <div class="blackRibbonButtons" style="width:170px;">
            <div id="geniusGeneral" class="bTabGeniusMenu bTabGeniusMenu-active" style="cursor: pointer;">
                @Html.Raw(labels("Genius#generalGenius"))
            </div>
            <div id="geniusPlanification" class="bTabGeniusMenu" style="cursor: pointer; display:none;">
                @Html.Raw(labels("Genius#geniusPlanification"))
            </div>
        </div>
    </div>
</div>

<div id="divMainBody">
    <div id="divTabData" Class="divDataCells">
        <input type='hidden' id='hdnEmployees' runat='server' value='' />
        <input type="hidden" id="hdnFilter" runat="server" value='' />
        <input type="hidden" id="hdnFilterUser" runat="server" value='' />
        <input type="hidden" id="hdnUserFieldsSelected" runat="server" value='' />
        <input type="hidden" id="hdnCostCentersSelected" runat="server" value='' />
        <input type="hidden" id="hdnCausesSelected" runat="server" value='' />
        <input type="hidden" id="hdnConceptsSelected" runat="server" value='' />
        <input type="hidden" id="hdnSelectedDataTypes" runat="server" value='' />
        <input type="hidden" id="hdnRequestsSelected" runat="server" value='' />

        <div id="divContenido" Class="divAllContent twoWindowsFlexLayout">

            @Html.Partial("~/Views/Base/CardTree/_CardTree.vbhtml")

            <div id="divButtons" class="divMVCMiddleButtons">
                @Html.Partial("~/Views/Base/TabBar/_DataTabBarButtons.vbhtml")
            </div>

            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent">
                <div id="saveBar" class="">
                    @Html.Partial("~/Views/Base/BarButtons/_SaveBarButtons.vbhtml")
                </div>
                <div id="mainPanelDisplay" class="generalDiv">
                    @Html.Partial("_GeniusAnalitycNewReport")
                    @Html.Partial("_GeniusAnalitycResultsViewer")
                    @Html.Partial("_GeniusAnalitycView")
                </div>
                <div id="planificationDiv" style="display:none">
                    @Html.Partial("_GeniusAnalitycScheduleReport")
                    @Html.Partial("_GeniusAnalitycPlanningScheduler")
                </div>
            </div>

            <div id="popupContainer"></div>
        </div>
    </div>

    @code
        Html.DevExtreme().Popup() _
        .ID("sharePopup") _
        .Width(600) _
        .Height(300) _
        .Position(PositionAlignment.Top) _
        .ShowTitle(False) _
        .DragEnabled(False) _
        .HideOnOutsideClick(True) _
        .ContentTemplate(Sub()
        @<text>
            @Html.Partial("_ShareGenius")
        </text> End Sub).Render()
    End code

    @code
        Html.DevExtreme().Popup() _
    .ID("reportConfigurationPopup") _
    .Width(1000) _
    .Height("auto") _
    .Position(PositionAlignment.Top) _
    .ShowTitle(False) _
    .DragEnabled(False) _
    .HideOnOutsideClick(True) _
    .OnShown("onReportConfigurationPopupShown") _
    .OnHidden("onReportConfigurationPopUpHidden") _
    .ContentTemplate(Sub()
    @<text>
        @Html.Partial("_GeniusAnalitycLauncher")
    </text>
End Sub).Render()
    End code

    @code
        Html.DevExtreme().Popup() _
    .ID("CostsCenterPopUp") _
    .Width(500) _
    .Height("auto") _
    .MaxHeight(500) _
    .Position(PositionAlignment.Top) _
    .ShowTitle(False) _
    .DragEnabled(False) _
    .HideOnOutsideClick(True) _
    .ContentTemplate(Sub()
    @<text>
        @Html.Partial("_BusinessCentersConfig")
    </text>
End Sub).Render()
    End code
    @code
        Html.DevExtreme().Popup() _
    .ID("CauesPopUp") _
    .Width(500) _
    .Height("auto") _
    .MaxHeight(500) _
    .Position(PositionAlignment.Top) _
    .ShowTitle(False) _
    .DragEnabled(False) _
    .HideOnOutsideClick(True) _
    .OnHidden("onCauesPopUpHidden") _
    .ContentTemplate(Sub()
    @<text>
        @Html.Partial("_CausesConfig")
    </text>
End Sub).Render()
    End code

    @code
        Html.DevExtreme().Popup() _
    .ID("ConceptsPopUp") _
    .Width(500) _
    .Height("auto") _
    .MaxHeight(500) _
    .Position(PositionAlignment.Top) _
    .ShowTitle(False) _
    .DragEnabled(False) _
    .HideOnOutsideClick(True) _
    .ContentTemplate(Sub()
    @<text>
        @Html.Partial("_ConceptsConfig")
    </text> End Sub).Render()
    End code

    @code
        Html.DevExtreme().Popup() _
        .ID("ReuqestsPopUp") _
        .Width(500) _
        .Height("auto") _
        .MaxHeight(500) _
        .Position(PositionAlignment.Top) _
        .ShowTitle(False) _
    .DragEnabled(False) _
    .HideOnOutsideClick(True) _
    .ContentTemplate(Sub()
    @<text>
        @Html.Partial("_RequestsConfig")
    </text> End Sub).Render()
    End code
</div>
<div id="divGeniusEmployeeSelector">
</div>