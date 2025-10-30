@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<div id="divCreateSupervisorRule">
    <div id="divSupervisors" style="display: flex;justify-content: center;">
        <div class="dx-field-label">
            @Html.Raw(labels("Bots#ruleSupervisors"))
        </div>
        <div class="dx-field-value">
            @(Html.DevExtreme().SelectBox() _
                        .ID("BotsIdTemplate") _
                        .Placeholder(labels("Bots#placeholderSupervisors")) _
                        .ValueExpr("ID") _
                        .DisplayExpr("Name") _
                        .SearchEnabled(True) _
                        .DataSource(ViewBag.Supervisors) _
    )
        </div>
    </div>
</div>