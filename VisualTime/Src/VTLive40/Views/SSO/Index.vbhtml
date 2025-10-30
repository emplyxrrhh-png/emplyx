@imports Robotics.Web.Base.API

@Code
    Layout = "~/Views/Shared/_layout.vbhtml"

    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
    Dim baseURL = Url.Content("~")
    Dim priorities As String() = {"CEGIDID", "AAD", "Adfs", "OKTA", "SAML"}
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim remoteURL = ViewBag.checkURL
End Code

<script>
    var BASE_URL = "@baseURL";
    var chekPage_URL = "@Html.Raw(ViewBag.checkURL)";
    var jsLabels = JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels)) ');
</script>
<link href="@Url.Content("~/Content/bootstrap.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/liveMVC.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/Live/sso/roSSO.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Scripts/jszip.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/Live/sso/roSSOScript.min.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

@Html.Partial("~/Views/Base/BarButtons/_ButtonList.vbhtml")

<div id="divMainBody">
    <div id="divTabData" Class="divDataCells">

        <div id="divContenido" Class="divAllContent twoWindowsFlexLayout">
            <div style="margin:20px">
                <div class="panHeader2 panBottomMargin">
                    <span class="panelTitleSpan">
                        <span id="">@Html.Raw(labels("SSO#headerTitle"))</span>
                    </span>
                </div>
                <div style="min-height:20px"></div>
                <div style="margin-left:20px">
                    @(Html.DevExtreme().CheckBox().ID("ckSSOStatus").Text(labels("SSO#enableSSO") + "(" + remoteURL + ")").OnValueChanged("checkSSOStatus"))
                    <br />
                    <div style="margin-top:20px;margin-left:20px">
                        @(Html.DevExtreme().List() _
                                        .ID("rgSSOptions") _
                                        .Items(priorities) _
                                        .ItemTemplate(New JS("ssoItemTemplate")) _
                                        .Width("100%") _
                                        .ShowSelectionControls(True) _
                                        .AllowItemDeleting(False) _
                                        .SelectionMode(ListSelectionMode.Single) _
                                        .ScrollingEnabled(True) _
                                        .PageLoadMode(ListPageLoadMode.NextButton) _
                                        .OnSelectionChanged("ssoValueChanged") _
                                        .Disabled(True)
                            )
                    </div>

                    <div style="margin-top:20px;">
                        <div class="dx-field" style="float:left">
                            @(Html.DevExtreme().CheckBox().ID("ckVTLiveMixAuthEnabled").ReadOnly(True).Text(labels("SSO#enableVTLiveMixAuth")).Disabled(True))
                        </div>
                        <div class="dx-field" style="margin-left:30px; float:left">
                            @(Html.DevExtreme().CheckBox().ID("ckVPortalMixAuthEnabled").Text(labels("SSO#enableVTPortalMixAuth")).Disabled(True))
                        </div>
                    </div>

                    <div style="clear:both">

                        <div>
                            <div class="dx-field" style="margin-left:40px;float:left">
                                @(Html.DevExtreme().Button().ID("btSubmitNewConfiguration").Text(labels("SSO#saveValidation")).OnClick("saveSSOData").Disabled(True))
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    @code
        Html.DevExtreme().Popup().ID("adfsHelp").Width(1200).Height(750).ShowTitle(False).DragEnabled(False).HideOnOutsideClick(True).ContentTemplate(Sub()
        @<text>
            @Html.Partial("_adfsHelp")
        </text>
                                                                                                                                                          End Sub).Render()
    End code

    @code
        Html.DevExtreme().Popup().ID("aadHelp").Width(1200).Height(750).ShowTitle(False).DragEnabled(False).HideOnOutsideClick(True).ContentTemplate(Sub()
        @<text>
            @Html.Partial("_aadHelp")
        </text>
                                                                                                                                                         End Sub).Render()
    End code

    @code
        Html.DevExtreme().Popup().ID("oktaHelp").Width(1200).Height(750).ShowTitle(False).DragEnabled(False).HideOnOutsideClick(True).ContentTemplate(Sub()
        @<text>
            @Html.Partial("_oktaHelp")
        </text>
                                                                                                                                                          End Sub).Render()
    End code
</div>