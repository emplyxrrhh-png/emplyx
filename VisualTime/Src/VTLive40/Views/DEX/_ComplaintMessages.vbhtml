@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim conversationState = String.Empty
    Dim conversationStateClass = String.Empty

    Dim buttonsStyle = ""
    Dim closedStyle = "display:none"
    Dim listHeight = ""
    Dim closedBy As String = String.Empty
    Dim companyName As String = Robotics.VTBase.roTypes.Any2String(ViewData("CompanyName"))

    If ViewData("Conversation") IsNot Nothing Then
        Select Case (ViewData("Conversation").Status)
            Case ConversationStatusEnum.Pending
                conversationState = labels("Messages#statePending")
                conversationStateClass = "conversationState0"
            Case ConversationStatusEnum.Closed
                conversationState = labels("Messages#stateClosed")
                buttonsStyle = "display:none"
                listHeight = "height:calc(100% - 50px)"
                closedBy = labels("Messages#stateClosed") & " " & labels("Messages#by") & " " & ViewData("Conversation").LastStatusChangeByName & " " & labels("Messages#at") & " " & CDate(ViewData("Conversation").LastStatusChangeOn).ToString("dd-MM-yyyy HH:mm")
                closedStyle = ""
                conversationStateClass = "conversationState3"
            Case ConversationStatusEnum.Dismissed
                conversationState = labels("Messages#stateDismissed")
                buttonsStyle = "display:none"
                listHeight = "height:calc(100% - 50px)"
                closedStyle = ""
                closedBy = labels("Messages#stateDismissed") & " " & labels("Messages#by") & " " & ViewData("Conversation").LastStatusChangeByName & " " & labels("Messages#at") & " " & CDate(ViewData("Conversation").LastStatusChangeOn).ToString("dd-MM-yyyy HH:mm")
                conversationStateClass = "conversationState2"
            Case ConversationStatusEnum.Ongoing
                conversationState = labels("Messages#stateOngoing")
                conversationStateClass = "conversationState1"
        End Select
    End If
    Dim messages As Generic.List(Of roMessage) = ViewData("ConversationMessages")

End Code

<script>
    var jsLabels = JSON.parse('@Html.Raw(Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(labels)) ');
    var selectedComplaint = null;
    var selectedReference = null;
    @If ViewData("Conversation") IsNot Nothing Then
        @<text>
                selectedComplaint = @Html.Raw(ViewData("Conversation").Id);
                selectedReference = '@Html.Raw(ViewData("Conversation").ReferenceNumber)';
        </text>
    End If
</script>
<div class="conversations">
    @If ViewData("Conversation") IsNot Nothing Then
        @<div Class="channelConversationsActions">
            <h2 id="complaintTitle">@ViewData("Conversation").Title</h2>
            <div id="msgPending" style="display: inline-block;position:relative;">
                <div>
                    <div class="complaintSubject">
                        <Span id="spanStatusText" Class="conversationState @conversationStateClass">@conversationState</Span>
                    </div>
                    <h5>Empresa: @Html.Raw(companyName)</h5>
                    <h5 id="complaintRef">Referencia: @ViewData("Conversation").ReferenceNumber</h5>
                </div>
                <div class="printComplaintMessages">
                    @(Html.DevExtreme().Button() _
                            .ID("printComplaintMessages") _
                            .Icon("print") _
                            .OnClick("printComplaintMessages") _
                            .Text(labels("Messages#roPrintComplaint")) _
                            .Type(ButtonType.Default)
                        )
                    @(Html.DevExtreme().Button() _
                                .ID("btnGoToHome") _
                                .Icon("close") _
                                .OnClick("goToHome") _
                                .Type(ButtonType.Danger) _
                        )
                </div>
            </div>
        </div>
    End If
    @If messages IsNot Nothing AndAlso messages.Count > 0 Then
        @<div id="divMessagesList" style="@listHeight" Class="messageList">
            @Html.Partial("_MessagesList", messages)
        </div>
        @<div id="sendBox" Class="sendBox" style="@buttonsStyle">
            @(Html.DevExtreme().TextArea() _
                                        .ID("newMsgText") _
                                        .Placeholder(labels("Messages#SendPlaceHolder")) _
                                        .Height(90) _
                                        .Width(New JS("calcMessageWidth")))

            <Button Class="sendButtonContent" onclick="createNewMessage()"></Button>
        </div>
    Else
        @<div id="divConversationContent" Class="conversationMessages">
            @Html.Partial("_EmptyMessages")
        </div>
    End If
</div>

<Script>
    @If messages IsNot Nothing AndAlso messages.Count > 0 Then
    @<text>
        $('#divMessagesList').scrollTop($('#divMessagesList')[0].scrollHeight);
    </text>
    End If
</Script>