@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<div class="aParent" style="float:right; margin-top:1px;">
    <div id="onlyEditButtons" style="display: inline-block;">

        <div id="botSave" style="display: inline-block;">
            @(Html.DevExtreme().Button() _
                                                    .ID("btnSaveBot") _
                                                    .OnClick("saveBotPopUp") _
                                                    .Icon("todo") _
                                                    .Type(ButtonType.Default) _
                )
        </div>
        <div id="botCancel" style="display: inline-block;margin-left:10px;">
            @(Html.DevExtreme().Button() _
                                                    .ID("btnCancelBot") _
                                                    .OnClick("cancelBotPopUp") _
                                                    .Icon("close") _
                                                    .Type(ButtonType.Danger) _
                )
        </div>
    </div>
</div>

<br />
<br />
<br />

<div id="divBotConfig">
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
                   .ID("BotNameConfig") _
                )
            </div>
        </div>
    </div>
</div>
