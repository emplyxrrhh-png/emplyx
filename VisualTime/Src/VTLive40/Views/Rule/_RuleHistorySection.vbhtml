@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<section id="ruleHistorySection">    
    <div class="container mt-3 gap-3" style="margin:0;max-width:100%">
        <div class="row">
            <div class="col-3">
                <div id="ruleHistoryGrid" style="width: 100%;"></div>
            </div>            
            <div class="col-9">
                <div class="row">
                    @Html.Partial("_stateBarIcons", New With {.Section = "RuleHistory", .Title = labels("RulesGroup#DefinitionHeader"), .IsHeader = True, .ShowClose = False, .ShowUndo = True, .ShowDelete = True, .ShowSave = True, .ShowVisualize = False})
                    <div style="min-height:15px"></div>
                </div>
                <div class="row flexm mb-3 mt-3">
                    <div class="col-6">
                        <div class="d-flex mb-2">
                            <div style="width: 100px;" class="mt-2">
                                @Html.Raw(labels("RulesGroup#lblDesc"))
                            </div>
                            <div class="w-100">
                                @(Html.DevExtreme().TextBox() _
.ID("txtDefinitionDescription") _
.MaxLength(90)
)
                            </div>
                        </div>
                    </div>
                    <div class="col-6">
                        <div class="d-flex mb-2">
                            <div style="width: 100px;" class="mt-2">
                                @Html.Raw(labels("RulesGroup#lblAvailableFrom"))
                            </div>
                            <div class="w-130">
                                <div id="dpAvailableFrom"></div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row d-flex">
                    <div class="panHeader2 panBottomMargin" style="background-color: #85a6ff">
                        <div class="row">
                            <span class="panelTitleSpan">
                                <span id="">@Html.Raw(labels("RulesGroup#lblContext"))</span>
                            </span>
                        </div>
                    </div>
                    <div style="min-height:15px"></div>
                </div>
                <div class="row d-flex">
                    <div class="d-flex mb-2">
                        <div style="width: 100px;" class="mt-2">
                            @Html.Raw(labels("RulesGroup#lblShifts"))
                        </div>
                        <div class="w-100">
                            <div id="tbShifts"></div>
                        </div>
                    </div>
                </div>
                <div class="row d-flex mb-3">
                    <div class="d-flex mb-2">
                        <div style="width: 100px;" class="mt-2">
                            @Html.Raw(labels("RulesGroup#lblWhoApplies"))
                        </div>
                        <div class="w-100">
                            <input type="hidden" id="EmployeeFilter" value="" runat="server" clientidmode="Static" />
                            @(Html.DevExtreme().TextBox() _
.ID("txtWhoRuleApply") _
.Placeholder(labels("RulesGroup#selectWhoApply")) _
)
                        </div>
                    </div>
                </div>
                <div class="row d-flex">
                    <div class="panHeader2 panBottomMargin" style="background-color: #85a6ff">
                        <div class="row">
                            <span class="panelTitleSpan">
                                <span id="">@Html.Raw(labels("RulesGroup#lblConditions"))</span>
                            </span>
                        </div>
                    </div>
                    <div style="min-height:15px"></div>
                </div>
                <div class="row d-flex">
                    <div class="panHeader2 panBottomMargin" style="background-color: #85a6ff">
                        <div class="row">
                            <span class="panelTitleSpan">
                                <span id="">@Html.Raw(labels("RulesGroup#lblActions"))</span>
                            </span>
                        </div>
                    </div>
                    <div style="min-height:15px"></div>
                </div>
            </div>
        </div>
    </div>
</section>
