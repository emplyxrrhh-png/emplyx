@imports Robotics.Web.Base.API

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"

    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim baseURL = Url.Content("~")
End Code

<script>
    var BASE_URL = "@baseURL";
    var AvailableBotTypes =  @Html.Raw(Json.Encode(ViewBag.AvailableBotTypes));
    var AllRules = @Html.Raw(Json.Encode(ViewBag.AllRules));
</script>
<!-- Crear los scripts correspondientes para supervisors -->
<link href="@Url.Content("~/Content/bootstrap.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/liveMVC.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/Bots/roBots.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Scripts/jszip.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Bots/roBotsScript.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

@Html.Partial("~/Views/Base/BarButtons/_ButtonList.vbhtml")

<div id="divMainBody">
    <div id="divTabData" Class="divDataCells">

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
                    <div id="divBotsMainView">
                    </div>
                </div>
            </div>
        </div>
    </div>

    @code
        Html.DevExtreme().Popup() _
    .ID("newBotPopup") _
    .Width(1000) _
    .Height("auto") _
    .Position(PositionAlignment.Top) _
    .ShowTitle(False) _
    .DragEnabled(False) _
    .HideOnOutsideClick(True) _
    .OnShown("onNewBotPopupShown") _
    .OnHidden("onNewBotPopUpHidden") _
    .ContentTemplate(Sub()
    @<text>
        @Html.Partial("_BotsNewBotConfiguration")
    </text>
End Sub).Render()
    End code
</div>