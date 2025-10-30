@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
End Code

<div class="emptyMessagePosition">
    <div class="emptyConversation"></div>
    <div class="emptyMessage">
        <span>@Html.Raw(labels("Messages#emptylist"))</span>
    </div>
</div>