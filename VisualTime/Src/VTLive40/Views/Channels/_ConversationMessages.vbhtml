@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim conversationState = String.Empty

    Dim buttonsStyle = ""
    Dim closedStyle = "display:none"
    Dim listHeight = ""
    Dim closedBy As String = String.Empty

    Select Case (ViewData("Conversation").Status)
        Case ConversationStatusEnum.Pending
            conversationState = labels("Messages#statePending")
        Case ConversationStatusEnum.Closed
            conversationState = labels("Messages#stateClosed")
            buttonsStyle = "display:none"
            listHeight = "height:calc(100% - 50px)"
            closedBy = labels("Messages#stateClosed") & " " & labels("Messages#by") & " " & ViewData("Conversation").LastStatusChangeByName & " " & labels("Messages#at") & " " & CDate(ViewData("Conversation").LastStatusChangeOn).ToString("dd-MM-yyyy HH:mm")
            closedStyle = ""
        Case ConversationStatusEnum.Dismissed
            conversationState = labels("Messages#stateDismissed")
            buttonsStyle = "display:none"
            listHeight = "height:calc(100% - 50px)"
            closedStyle = ""
            closedBy = labels("Messages#stateDismissed") & " " & labels("Messages#by") & " " & ViewData("Conversation").LastStatusChangeByName & " " & labels("Messages#at") & " " & CDate(ViewData("Conversation").LastStatusChangeOn).ToString("dd-MM-yyyy HH:mm")
        Case ConversationStatusEnum.Ongoing
            conversationState = labels("Messages#stateOngoing")
    End Select

    Dim highComplaintComplexity = "false"

    If ViewData("Conversation").Complexity = ConversationComplexity.High Then
        highComplaintComplexity = "true"
    End If
    Dim messages As Generic.List(Of roMessage) = ViewData("ConversationMessages")

End Code

<script>
    var jsLabels = JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels)) ');
    var selectedComplaint = @Html.Raw(ViewData("Conversation").Id);
    $(() => {

        if (@highComplaintComplexity) {
            $("#chkComplexity").addClass("high");
            $("#chkComplexity").removeClass("normal");            
        }
        else {
            $("#chkComplexity").addClass("normal");
            $("#chkComplexity").removeClass("high");            
        }

        $('#togBtn').prop('checked', @highComplaintComplexity);

        $('#togBtn').change(function () {
            if (this.checked) {
                $("#chkComplexity").addClass("high");
                $("#chkComplexity").removeClass("normal");
                changeConversationComplexity(viewUtilsManager.getSelectedCardId(), 1);
            }
            else {
                $("#chkComplexity").addClass("normal");
                $("#chkComplexity").removeClass("high");
                changeConversationComplexity(viewUtilsManager.getSelectedCardId(), 0);
            }
        });
    });
</script>

<div Class="channelConversationsActions">
    <div id="msgPending" style="display: inline-block;">
        <div>
            <Span Class="lblStatusDescription">@Html.Raw(labels("Messages#conversationStatus"))</Span>
            <Span id="spanStatusText" Class="conversationStatus"></Span>
        </div>
    </div>

    <div id="msgClosed" style="@buttonsStyle" Class="statusButton">
        @(Html.DevExtreme().Button() _
                                                                                                                                                    .ID("btnMsgClosed") _
                                                                                                                                                    .OnClick("changeMsgState") _
                                                                                                                                                    .Text(labels("Messages#closed")) _
                                                                                                                                                    .Type(ButtonType.Success))
    </div>
    <div id="msgRejected" style="@buttonsStyle" Class="statusButton">
        @(Html.DevExtreme().Button() _
                                                                                                                            .ID("btnMsgRejected") _
                                                                                                                            .OnClick("changeMsgState") _
                                                                                                                            .Text(labels("Messages#dismissed")) _
                                                                                                                            .Type(ButtonType.Danger))
    </div>
    <div id="printConv">
        @(Html.DevExtreme().Button() _
                                                                                                                                                    .ID("btnPrintConv") _
                                                                                                                                                    .OnClick("printConversation") _
                                                                                                                                                    .Text(labels("Messages#roPrintComplaint")))
    </div>
    @If ViewData("Conversation") IsNot Nothing AndAlso ViewData("Conversation").Channel.IsComplaintChannel AndAlso ViewData("Conversation").Status <> ConversationStatusEnum.Closed AndAlso ViewData("Conversation").Status <> ConversationStatusEnum.Dismissed Then
        @<div id="convComplexity" style="width:200px;" Class="statusButton">
            <div Class="dx-field-label" style="white-space:normal;width:143px;color:#333;font-size:1.3em;padding-top:5px;">
                @Html.Raw(labels("Messages#conversationComplexity"))
            </div>
            <div id="divChangeComplexity">
                <Label style="width:auto;" Class="switch">
                    <input type="checkbox" id="togBtn">
                    <div id="chkComplexity" Class="slider round">
                        <Span Class="on"></Span>
                        <Span Class="off"></Span>
                    </div>
                </Label>
            </div>
        </div>
    End If
</div>

<div Class="conversations">
    <div id = "divMessagesList" style="@listHeight" Class="messageList">
        @For Each oMessage As roMessage In messages
            @Html.Partial("_messageWrapper", oMessage)

        Next
        <div Class="statusClosedInfo" style="@closedStyle">
            @closedBy
        </div>
    </div>
    <div id = "sendBox" Class="sendBox" style="@buttonsStyle">
        <div style = "margin-left:30px;margin-right:30px" >
            @(Html.DevExtreme().TextArea() _
                                                                .ID("newMsgText") _
                                                                .Placeholder(labels("Messages#SendPlaceHolder")) _
                                                                .Height(90) _
                                                                .Width(New JS("calcMessageWidth")))

            <Button Class="sendButtonContent" onclick="createNewMessage()"></Button>
        </div>
    </div>
</div>

<Script>
    $('#divMessagesList').scrollTop($('#divMessagesList')[0].scrollHeight);

    $(`#conversationInfo${viewUtilsManager.getSelectedCardId()} .csBadgeSelector`).hide()
</Script>