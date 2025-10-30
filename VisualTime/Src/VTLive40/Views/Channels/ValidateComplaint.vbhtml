@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<div>
    <div class=" panHeader2 panBottomMargin complaintHeader">
        <span class="panelTitleSpan">@Html.Raw(labels("Channels#validateComplaintTitle"))</span>
    </div>
    

    <div class="complaintText">
        <div id="dexConditions">
        </div>
    </div>
    <div Class="complaintButtons">
        <div style="display:flex;justify-content:center;">
            <div style="margin:10px">
                    @(Html.DevExtreme().Button() _
                            .ID("btnConfirmPublish") _
                            .OnClick("confirmPublishChannel") _
                            .Text(labels("Channels#roChannelDEXAccept")) _
                            .Type(ButtonType.Success) _
                )
            </div>

            <div style="margin:10px">
                @(Html.DevExtreme().Button() _
                    .ID("btnCancelPublish") _
                    .OnClick("cancelPublishChannel") _
                    .Text(labels("Channels#roChannelCancelPublish").ToString()) _
                    .Type(ButtonType.Normal) _
                    )
            </div>
        </div>
    </div>
</div>