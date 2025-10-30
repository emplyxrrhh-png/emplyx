@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base.roLanguageWeb

@Code
    Layout = "~/Views/Shared/_layoutBS.vbhtml"
    Dim baseURL = Url.Content("~")
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim companyName As String = Robotics.VTBase.roTypes.Any2String(ViewData("CompanyName"))
End Code

<script type="text/javascript">
    var RootUrl = "@Html.Raw(ViewBag.RootUrl)";
    var BASE_URL = "@baseURL";
    var jsLabels = JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels)) ');
</script>

<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.main.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/DEX/roDEX.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Base/Scripts/Ajax.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/DEX/roDEX.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<div class="bg-color-eb h-fill">
    <div class="MainToolbarPad">
        <div id="tbToolbar" class="Toolbar">
            <div class="tbd_logo" onclick="location.href = 'https://visualtime.net';" style="cursor: pointer;">
            </div>
            <div id="tdLogoBar" class="tbd_bar">
                <div id="vtLogoTextDiv" class="tbd_bar_text" style="cursor: pointer; position: relative;" onclick="location.href = 'https://visualtime.net';">
                    <div id="vtLogoVersionDiv" style="margin-top: 13px;" class="tbd_bar_textVersion">
                        <span class="notranslate">cegid Visualtime</span>
                    </div>
                </div>
                <div id="tdMenuToolbar" runat="server" align="center" class="tdMenuToolbar">
                </div>
            </div>
            <div id="menu_icons">
            </div>
        </div>
    </div>
    <div class="d-flex justify-content-around min-vh-94 align-items-center">
        <div class="d-flex p-4 gap-4 flex-column-reverse flex-md-row max-width-1400 min-vh-72 w-100">
            <div style="display: flex; flex-direction: column; gap: 2rem; flex: 40vw;">
                <div class="border border-3 border-dark rounded-5 flex-grow-1 p-4 p-md-5 bg-color-grey">
                    <div id="divConversationContent" class="list-DexMessages">
                        @Html.Partial("_ComplaintMessages")
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<script>
    initDEX();
</script>