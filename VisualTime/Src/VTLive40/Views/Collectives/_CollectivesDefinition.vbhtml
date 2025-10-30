@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<section id="collectiveDefinition">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("Collectives#GeneralHeader"))</span>
        </span>
    </div>
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;" class="d-flex gap-5">
        <div class="d-flex flex-column w-50">
            <div class="d-flex mb-2">
                <div style="width: 100px;">
                    @Html.Raw(labels("Collectives#lblName"))
                </div>
                <div class="w-100">
                    @(Html.DevExtreme().TextBox() _
.OnChange("changeCollectiveName") _
.ID("txtName") _
.MaxLength(90) _
                                    )
                </div>
            </div>
            <div class="d-flex">
                <div style="width: 100px;">
                    @Html.Raw(labels("Collectives#lblDesc"))
                </div>
                <div class="w-100">
                    @(Html.DevExtreme().TextArea() _
.ID("txtDescription") _
.OnChange("changeCollectiveDesc") _
.Height(90) _
.MaxLength(4000) _
                                            )
                </div>
            </div>
        </div>
    </div>
</section>