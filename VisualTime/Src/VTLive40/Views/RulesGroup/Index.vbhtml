@imports Robotics.Web.Base.API

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"

    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<script>
    var BASE_URL = "@baseURL";
    var jsLabels = null;
    window.RuleGroups = { i18n: JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels).Replace("'", "\'")) ') };
</script>
<style>
    .dx-toast-warning {
        background-color: #e9e19c !important;
    }
</style>
<!-- Crear los scripts correspondientes para supervisors -->
<link href="@Url.Content("~/Content/bootstrap.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/liveMVC.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/Rule/roRulesGroup.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Base/Scripts/Live/RulesGroup/roRulesGroupScript.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Rule/roRuleScript.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

@Html.Partial("~/Views/Base/BarButtons/_ButtonList.vbhtml")

<div id="divMainBody">
    <div id="divTabData" Class="divDataCells">

        <div id="divContenido" Class="divAllContent">
            <div style="display:flex;flex-direction:column;gap:10px;padding:10px">
                @Html.Partial("_stateBarIcons", New With {.Section = "RuleMng", .ShowClose = False, .ShowUndo = True, .ShowDelete = False, .ShowSave = True, .ShowVisualize = False})

                <div id="searchRulesGroupBar" class="groupRuleSearchBar">
                </div>
                <div id="rulesGroupDashboard" class="groupRuleDashboard">
                </div>
            </div>
        </div>
    </div>

    @Code

        Html.DevExtreme().Popup() _
.ID("ruleFrm") _
.FullScreen(True) _
.ShowTitle(False) _
.ShowCloseButton("False") _
.DragEnabled(False) _
.HideOnOutsideClick(False) _
.OnShown("function(e) { window.Rule.Events.onLoad(); }") _
.ContentTemplate(Sub()
@<text>
    @Html.Partial("~/Views/Rule/Rule.vbhtml")
</text>
End Sub).Render()
    End Code

</div>
<div id="divRuleContextSelector">
</div>
