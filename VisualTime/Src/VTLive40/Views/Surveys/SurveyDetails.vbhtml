@Imports Robotics.Base.DTOs
@Imports Robotics.Web.Base.roLanguageWeb

@ModelType roToDoList

@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim SurveysController As VTLive40.SurveysController = New VTLive40.SurveysController()
    Dim baseURL = Url.Content("~")
End Code
<script>var BASE_URL ="@baseURL"; </script>
<link href="@Url.Content("~/Base/Styles/roStart.min.css")" rel="stylesheet" type="text/css" />
<link href="@Url.Content("~/Base/Styles/roLiveStyles.min.css")" rel="stylesheet" type="text/css" />
<script src="@Url.Content("~/Base/Scripts/Ajax.js")" type="text/javascript"></script>
<script src="@Url.Content("~/Base/Scripts/roSurvey.js")" type="text/javascript"></script>

<div id="news" style="float:left"></div>
<div id="newsOk" style="float:right; margin-top:60px;"></div>

<div style="display:flex;">
    <div style="border-left: 1vw solid white;border-top: 1vw solid white; width:100%;border-right: 1vw solid white;">
        <div class="panHeader5" style="display: flex;align-items: center;">
            <div style="margin-right:0.5vw"><img src="~/Base/Images/StartMenuIcos/Surveys96.png" width="48" height="48" /></div>
            <div>
                <div style="font-size:20px;width:100%">@SurveysController.GetServerLanguage().Translate("roSurveysTitle", "Surveys")</div>
                <div><span id="readOnlyDescritionCompany" style="font-size: 11px;font-weight: 100;">@SurveysController.GetServerLanguage().Translate("roSurveysInfo", "Surveys")</span></div>
            </div>
        </div>
    </div>
</div>
<br />

<div id="divMainBody" style="min-height:unset !important; height:unset !important">
    <!-- TAB SUPERIOR -->
    <div id="divTabData">

        <div id="divContenido" Class="divAllContentSurveys twoWindowsFlexLayout">

            <div id="divContenido" class="twoWindowsMainPanel maxHeight divRightContent">

                <div class="form ro-tab-section container" style="display: flex;">

                    <div class="sectionSurveys" style="flex-grow: 1;" id="section2">
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<Script>
    var RootUrl = "@Html.Raw(ViewBag.RootUrl)";
    var Permission = "@Html.Raw(ViewBag.PermissionOverEmployees)";
</Script>