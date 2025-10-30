@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<section id="BotsWhen">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("Bots#whenDescripcion"))</span>
        </span>
    </div>

    <div style="min-height:15px"></div>

    <div style="width: 95%;margin: 0 auto;">
        <div class="d-flex justify-content-start">
            <div class="dx-field-label geniusLabel">
                @Html.Raw(labels("Bots#Title"))
            </div>
        </div>
    </div>

</section>