@imports System.Web.Mvc
@imports Robotics.Web.VTLiveMvC.Controllers

@Code
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
End Code

<!DOCTYPE html>
<html lang="en" class="notranslate" translate="no" style="height:100%; overflow:hidden;">
<head>
    <meta charset="utf-8" />
    <title>@ViewData("Title")</title>
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />
    <link href="@Url.Content("~/Base/Styles/FontAwesone/css/font-awesome.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
    <script src="@Url.Content("~/Base/jquery/jquery-3.7.1.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
    <script src="@Url.Content("~/Scripts/jszip.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
    <script type="text/javascript" src="@Url.Content("~/Base/Scripts/Live/Communique/dx-quill.min.js")@Html.Raw(scriptVersion)"></script>


    @Html.DevExpress().GetStyleSheets(
                            New StyleSheet With {.ExtensionSuite = ExtensionSuite.Editors},
                            New StyleSheet With {.ExtensionSuite = ExtensionSuite.TreeList},
                            New StyleSheet With {.ExtensionSuite = ExtensionSuite.CardView},
                            New StyleSheet With {.ExtensionSuite = ExtensionSuite.Report})

    @Html.DevExpress().GetScripts(
                                        New Script With {.ExtensionSuite = ExtensionSuite.Editors},
                                        New Script With {.ExtensionSuite = ExtensionSuite.TreeList},
                                        New Script With {.ExtensionSuite = ExtensionSuite.CardView},
                                        New Script With {.ExtensionSuite = ExtensionSuite.Report})

    <script type="text/javascript" src="@Url.Content("~/Base/globalize/sugar.lite.min.js")@Html.Raw(scriptVersion)"></script>
    <script type="text/javascript" src="@Url.Content("~/Base/Scripts/Cookies.js")@Html.Raw(scriptVersion)"></script>
    <script type="text/javascript" src="@Url.Content("~/Base/Scripts/moment.min.js")@Html.Raw(scriptVersion)"></script>
    <script type="text/javascript" src="@Url.Content("~/Base/Scripts/moment-tz.min.js")@Html.Raw(scriptVersion)"></script>
    <script type="text/javascript" src="@Url.Content("~/Base/Scripts/Live/roDateManager.min.js")@Html.Raw(scriptVersion)"></script>
    <script type="text/javascript" src="@Url.Content("~/Base/globalize/dx.Initialize.js")@Html.Raw(scriptVersion)"></script>
    <script type="text/javascript" src="@Url.Content("~/Base/globalize/dx.InitializeCulture.js")@Html.Raw(scriptVersion)"></script>

    <script type="text/javascript" src="@Url.Content("~/Scripts/aspnet/dx.aspnet.mvc.js")@Html.Raw(scriptVersion)"></script>
    <script type="text/javascript" src="@Url.Content("~/Scripts/aspnet/dx.aspnet.data.js")@Html.Raw(scriptVersion)"></script>

    <script src="@Url.Content("~/Base/Scripts/Live/commons.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
    <script src="@Url.Content("~/Base/Scripts/Live/utils.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
    <script src="@Url.Content("~/Base/Scripts/Live/Employees/roEmployeeSelector.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
    <script>

        var prefix = '@Url.Content("~/Base/globalize/cldr/locales/likelySubtags.json")';
        loadJSLanguages(prefix.replace("/globalize/cldr/locales/likelySubtags.json", ""), '@Html.Raw(scriptVersion)', function () { DevExpress.localization.locale(JSON.parse(localStorage.roLanguage).key); });
    </script>
</head>
<body style="height:100%">
    <section id="main" style="height: 100%;overflow-y: auto;">
        @RenderBody()
    </section>
</body>
</html>