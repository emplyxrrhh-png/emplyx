@imports Robotics.Web.Base.API

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"

    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim baseURL = Url.Content("~")
End Code

<script>
    var BASE_URL = "@baseURL";
</script>
<!-- Crear los scripts correspondientes para supervisors -->
<link href="@Url.Content("~/Content/bootstrap.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/liveMVC.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/Supervisors/roSupervisors.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Scripts/jszip.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/Supervisors/roSupervisorsScript.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

@Html.Partial("~/Views/Base/BarButtons/_ButtonList.vbhtml")

<div id="divMainBody">
    <div id="divTabData" Class="divDataCells">

        <div id="divContenido" Class="divAllContent twoWindowsFlexLayout">

            @Html.Partial("~/Views/Base/CardTree/_CardTree.vbhtml")

            <div id="divButtons" class="divMVCMiddleButtons">
                @Html.Partial("~/Views/Base/TabBar/_DataTabBarButtons.vbhtml")
            </div>

            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent">
                <iframe id="ifSupervisorsContent" runat="server" style="background-color: Transparent" height="100%" width="100%"
                        scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="~/Security/SupervisorsContent.aspx" onload="continueLoading()" />
            </div>

            <div id="popupContainer"></div>
        </div>
    </div>
</div>