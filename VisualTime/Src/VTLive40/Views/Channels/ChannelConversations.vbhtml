@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)
End Code

<link href="@Url.Content("~/Base/Styles/Live/Channels/roChannels.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<div class="channelConversations" style="">
    <div class="channelConversationsList">
        @Html.Partial("_ConversationsList")
    </div>

    <div id="divConversationContent" class="conversationMessages">
        @Html.Partial("_EmptyMessages")
    </div>
</div>