@Imports Robotics.Base.DTOs

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim baseURL = Url.Content("~")
End Code

<script type="text/javascript">
    var BASE_URL = "@baseURL";
    var jsLabels = JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels)) ');
</script>

<meta name="viewport" content="width=device-width,initial-scale=1">
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.main.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/dx.robotics.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Base/Scripts/Live/DEX/roDEX.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<form id="frmLogin" method="post" runat="server">
    <div class="loginFull">
        <div class="rbBackgroundImg errorBackgroundImg" id="rbBackground" runat="server" style="background-image: url(@Html.Raw(Url.Content("~/Base/Images/LoginBackground/img-form_C.png")))"></div>
        <div class="rbLoginColumn errorLoginColumn">
            <div class="rbLogoInformation LogoLogin errorLogoInformation">
                <div id="vtLogoVersionDiv" class="tbd_bar_textVersion " translate="no">
                    <div ID="titleVT" class="errorLogo" />
                </div>
            </div>
            <div class="rbLoginData">
                <p style="font-weight: bold; padding: 0 3vw;">@Html.Raw(labels("DEX#AlreadyLoggedIn"))</p>
                <div>
                    @(Html.DevExtreme().Button() _
.ID("invalidateCurrentSession") _
.OnClick("invalidateCurrentSession") _
.Text(labels("DEX#closeSessionDEX")) _
.Type(ButtonType.Default))
                </div>
            </div>
            <div class="rbTextClass">
                <div style="height: 50px">
                    <a href="https://www.cegid.com/ib/es/productos/cegid-visualtime/">©2025 Visualtime</a>&nbsp;|&nbsp; <a href="https://www.cegid.com/ib/es/asistencia-al-cliente/cegid-visualtime/">Soporte técnico</a>
                </div>
            </div>
        </div>
    </div>
</form>