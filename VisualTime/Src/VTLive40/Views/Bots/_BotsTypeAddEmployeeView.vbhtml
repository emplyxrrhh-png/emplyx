@Imports Robotics.Base.DTOs
@Imports VTLive40.Helpers

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

    <div style="width: 95%;margin: 0 auto;">
        <div class="d-flex justify-content-start">
            <div class="dx-field-label definition">
                @Html.Raw(labels("Bots#addNewEmployeeText"))
            </div>
        </div>
    </div>
</section>