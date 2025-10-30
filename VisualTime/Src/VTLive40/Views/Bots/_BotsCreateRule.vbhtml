@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<script>
    var BASE_URL = "@baseURL";
        var AvailableBotRules = @Html.Raw(Json.Encode(ViewBag.AvailableRules));
    var AllRules = @Html.Raw(Json.Encode(ViewBag.AllRules));
</script>

<div class="aParent" style="float:right; margin-top:1px;">
    <div id="onlyEditButtons" style="display: inline-block;">
        <div id="ruleSave" style="display: inline-block;">
            @(Html.DevExtreme().Button() _
                                                                                                                                                                            .ID("btnSaveBot") _
                                                                                                                                                                            .OnClick("saveRulePopUp") _
                                                                                                                                                                            .Icon("todo") _
                                                                                                                                                                            .Type(ButtonType.Default) _
                )
        </div>
        <div id="ruleCancel" style="display: inline-block;margin-left:10px;">
            @(Html.DevExtreme().Button() _
                                                                                                                                                                                    .ID("btnCancelBot") _
                                                                                                                                                                                    .OnClick("cancelRulePopUp") _
                                                                                                                                                                                    .Icon("close") _
                                                                                                                                                                                    .Type(ButtonType.Danger) _
                )
        </div>
    </div>
</div>

<br />
<br />
<br />

<div id="divRuleConfig">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            @Html.Raw(labels("Bots#createRulePanel"))
        </span>
    </div>

    <div style="min-height:15px"></div>

    <div style="width: 95%;margin: 0 auto;">
        <div id="divRuleName" class="d-flex justify-content-start">
            <div class="dx-field-label ruleName">
                @Html.Raw(labels("Bots#ruleName"))
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().TextBox() _
.ID("BotRuleName") _
                )
            </div>
        </div>
    </div>

    <div style="min-height:15px"></div>

    <div style="width: 95%;margin: 0 auto;">
        <div id="divRuleType" class="d-flex justify-content-start">
            <div class="dx-field-label ruletype">
                @Html.Raw(labels("Bots#ruleCreationName"))
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().SelectBox() _
.ID("BotCreationRule") _
.OnValueChanged("onRuleCreationChange") _
.ValueExpr("Id") _
.DisplayExpr("Name") _
                    )
            </div>
        </div>
    </div>

    <div style="min-height:15px"></div>

    <div style="width: 95%;margin: 0 auto;">
        <div id="divCreateRuleMainView">
        </div>
    </div>

    <div style="min-height:15px"></div>

    <div style="width: 95%;margin: 0 auto;">
        <div id="divActivateRule" class="d-flex justify-content-start">
            <div class="dx-field-value noFixedWidth" style="">
                @(Html.DevExtreme.CheckBox() _
         .ID("ckruleActivated").Text(labels("Bots#enabled"))
                    )
            </div>
        </div>
    </div>
</div>