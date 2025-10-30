@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<section id="BotsNew">
    <div id="mainPanelDisplay" class="generalDiv">
        @Html.Partial("_BotsConfigView")
        <div id="divBotsWhen">
        </div>
       @Html.Partial("_BotsRulesView")
    </div>
</section>