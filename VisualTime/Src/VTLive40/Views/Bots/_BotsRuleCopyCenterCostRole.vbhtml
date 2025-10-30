@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<div id="divCopyCenterCostRule">
    <div id="divCenterCosts" style="display: flex;justify-content: center;">
        <div class="dx-field-label">
            @Html.Raw(labels("Bots#ruleCopyCenterCostRule"))
        </div>
        <div class="dx-field-value">
            @(Html.DevExtreme().TagBox() _
                                            .ID("BotsIdTemplateList") _
                                            .Placeholder(labels("Bots#placeholdercostcenterroles")) _
                                            .ValueExpr("Id") _
                                            .DisplayExpr("Name") _
                                            .SearchEnabled(True) _
                                            .ShowSelectionControls(True) _
                                            .ApplyValueMode(EditorApplyValueMode.UseButtons) _
                                            .DataSource(ViewBag.CostCenterRoles) _
            )
        </div>
    </div>
</div>