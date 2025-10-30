@Imports Robotics.Base.DTOs
@ModelType roWebLink
<div class="webLinksPanel" style="min-width: 260px; max-width: 280px;">
    <div class="panHeaderRobotics">@Model.Title</div>
    <br />
    <div class="NewsSupportPosition">
        <div style="color:#333333" class="">@Model.Description</div>
        <br />
        <div class="panHeaderContact">
            @Code
                Dim URL As String = If(Model.URL.StartsWith("http"), Model.URL, "https://" & Model.URL)
            End Code
            <a id="rbClientsLink" runat="server" href="@URL" target="_blank" style="font-size: 14px; font-weight: bold;">@Model.LinkCaption</a>
        </div>
        <br />
        <br />
    </div>
</div>
