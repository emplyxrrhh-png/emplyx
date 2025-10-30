@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim conversationState = String.Empty
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)

    Dim buttonsStyle = ""
    Dim closedStyle = "display:none"
    Dim listHeight = ""
    Dim closedBy As String = String.Empty

    buttonsStyle = "display:none"
    listHeight = "height:calc(100% - 50px)"
    closedStyle = ""

    Dim messages As Generic.List(Of roMessage) = ViewData("ConversationMessages")

End Code
<link href="@Url.Content("~/Base/Styles/Live/Channels/roLogBook.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />
<script>
    var jsLabels = JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels)) ');
    @If ViewData("ConversationMessages") IsNot Nothing AndAlso ViewData("ConversationMessages").Count > 0 Then
        @<text>
    hasMessages = true;
        </text>
    Else
            @<text>
    hasMessages = false;
        </text>
    End If
    $(() => {
        {            
            if (hasMessages)
                $("#printLogBook").dxButton("instance").option("visible", true);
            else
                $("#printLogBook").dxButton("instance").option("visible", false); } });
</script>

<div Class="conversations">
    @If messages IsNot Nothing AndAlso messages.Count > 0 Then
        @<div id="divMessagesList" style="@listHeight" Class="messageList">
            @For Each oMessage As roMessage In messages
                @Html.Partial("_messageWrapper", oMessage)
            Next
        </div>
        Else
        @<div id="divConversationContent" Class="conversationMessages">
            @Html.Partial("_EmptyMessages")
        </div>
    End If
</div>

<Script>
    $('#divMessagesList').scrollTop($('#divMessagesList')[0].scrollHeight);

    $(`#conversationInfo${viewUtilsManager.getSelectedCardId()} .csBadgeSelector`).hide()
</script>