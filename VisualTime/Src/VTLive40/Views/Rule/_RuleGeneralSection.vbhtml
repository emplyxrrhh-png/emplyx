@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<section id="ruleGeneralSection">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("RulesGroup#GeneralHeader"))</span>
        </span>
    </div>
    <div style="min-height:15px"></div>
    <div style="width: 95%;margin: 0 auto;" class="d-flex gap-5">
        <div class="d-flex flex-column w-50">
            <div class="d-flex mb-2">
                <div style="width: 100px;" class="mt-2">
                    @Html.Raw(labels("RulesGroup#lblName"))
                </div>
                <div class="w-100">
                    @(Html.DevExtreme().TextBox() _
.ID("txtName") _
.MaxLength(90) _
                                    )
                </div>
            </div>
            <div class="d-flex mb-2">
                <div style="width: 100px;" class="mt-5">
                    @Html.Raw(labels("RulesGroup#lblDesc"))
                </div>
                <div class="w-100">
                    @(Html.DevExtreme().TextArea() _
        .ID("txtDescription") _
        .Height(90) _
        .MaxLength(4000) _
                                            )
                </div>
            </div>
            <div class="d-flex mb-2">
                <div style="width: 100px;">
                    @Html.Raw(labels("RulesGroup#lblType"))
                </div>
                <div class="w-100">
                    <div id="lblTypeDescription" class="label ms-2"></div>
                </div>
            </div>
            <div class="d-flex mb-2">
                <div style="width: 100px;" class="mt-2">
                    @Html.Raw(labels("RulesGroup#lblGroup"))
                </div>
                <div class="w-100">
                    <div id="cboGroup"></div>                    
                </div>
            </div>
        </div>
        <div class="d-flex flex-column w-50">
            <div class="d-flex mb-2">
                <div style="width: 100px;" class="mt-2">
                    @Html.Raw(labels("RulesGroup#lblTag"))
                </div>
                <div class="w-100">
                    <div id="tbTags"></div>                    
                </div>
            </div>
        </div>
        </div>
</section>