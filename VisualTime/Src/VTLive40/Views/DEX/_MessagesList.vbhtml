@Imports Robotics.Base.DTOs

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim messages = CType(Model, Generic.List(Of roMessage))
End Code

@For Each oMessage As roMessage In messages
    @Html.Partial("_messageWrapper", oMessage)
Next