@Imports Robotics.Base.DTOs
@Code
    Dim oMessage = CType(Model, roMessage)

    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim isEmployeeMessage As String = If(oMessage.IsResponse, "responseMessage", String.Empty)

    Dim imageUrl As String = If(oMessage.IsResponse, "/Employee/GetSupervisorPhoto/" & oMessage.CreatedBy, "/Employee/GetEmployeePhoto/" & oMessage.CreatedBy)
End Code

<div Class="messageWrapper @isEmployeeMessage">
    <div Class="messageSeparationLineTop"></div>
    <div Class="messageHeader">
        <div class="messageIconWrap">
        </div>
        <div class="messagePhoto" style="background:url(@imageUrl)"></div>
        <div class="messageInfo">
            <div class="messageAuthor"> <span>@oMessage.CreatedByName</span></div>
            <div class="messageDate"> <span>@labels("Messages#sendedAt") @oMessage.CreatedOn.ToString("dd-MM-yyyy HH:mm")</span></div>
        </div>
    </div>
    <div Class="messageContent">@oMessage.Body</div>
</div>