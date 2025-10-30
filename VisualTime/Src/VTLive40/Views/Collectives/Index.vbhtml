@imports Robotics.Web.Base.API

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"

    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim baseURL = Url.Content("~")
End Code

<script>
    var BASE_URL = "@baseURL";
</script>
<!-- Crear los scripts correspondientes para colectivos -->
<link href="@Url.Content("~/Content/bootstrap.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/liveMVC.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/Collectives/roCollectives.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Scripts/jszip.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Collectives/roCollectivesScript.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<script src="@Url.Content("~/Scripts/exceljs.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Scripts/FileSaver.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

@Html.Partial("~/Views/Base/BarButtons/_ButtonList.vbhtml")

<div id="divMainBody">
    <div id="divTabData" Class="divDataCells">

        <div id="divContenido" Class="divAllContent twoWindowsFlexLayout" style="position: relative">
            <div id="divNewButton" class="align-self-end" style="justify-self: end;position: absolute;z-index: 1;right: 10px;">
                <div id="CollectiveNew" style="display: inline-block;margin-left:10px;">
                    @(Html.DevExtreme().Button() _
.ID("newCollective") _
.Icon("plus") _
.OnClick("newCollective") _
.Type(ButtonType.Default) _
                )
                </div>
            </div>
            @Html.Partial("~/Views/Base/CardTree/_CardTree.vbhtml")

            <div id="divContenidoRight" class="twoWindowsMainPanel maxHeight divRightContent d-flex flex-column gap-3" style="padding: 10px;">

                @Html.Partial("_stateBarIcons", New With {.Section = "Collectives", .ShowClose = False, .ShowUndo = True, .ShowDelete = True, .ShowSave = True, .ShowVisualize = False})

                <div id="mainPanelDisplay" class="generalDiv" style="height: auto; overflow-y: auto; max-height: 76vh;">
                    @Html.Partial("~/Views/Collectives/_CollectivesDefinition.vbhtml")
                    @Html.Partial("~/Views/Collectives/_CollectivesFilter.vbhtml")
                </div>
            </div>
        </div>
    </div>
</div>