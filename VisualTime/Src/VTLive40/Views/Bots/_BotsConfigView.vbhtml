@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<section id="BotsNew">
    <div class="panHeader2 panBottomMargin">
        <span class="panelTitleSpan">
            <span id="">@Html.Raw(labels("Bots#Title"))</span>
        </span>
    </div>

    <div style="min-height:15px"></div>

    <div style="width: 95%;margin: 0 auto;">
        <div id="divBotName" class="d-flex justify-content-start">
            <div class="dx-field-label botsName">
                @Html.Raw(labels("Bots#botName"))
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().TextBox() _
                .ID("BotName") _
                .OnValueChanged("onBotNameChange") _
                )
            </div>
        </div>
    </div>

    <div style="min-height:15px"></div>

    <div style="width: 95%;margin: 0 auto;">
        <div id="divBotType" class="d-flex justify-content-start">
            <div class="dx-field-label botstype">
                @Html.Raw(labels("Bots#type"))
            </div>
            <div class="dx-field-value">
                @(Html.DevExtreme().SelectBox() _
                    .ID("BotType") _
                    .OnValueChanged("onTypeChange") _
                    .DataSource(ViewBag.AvailableBotTypes) _
                    .ValueExpr("Id") _
                    .DisplayExpr("Name") _
                    )
            </div>
        </div>
    </div>
    <div style="min-height:15px"></div>

</section>