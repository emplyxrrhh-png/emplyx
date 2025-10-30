@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<div class="panHeader4">
    @Html.Raw(labels("Genius#shareView"))
</div>

<div class="list-shareContent">
    <br />
    <div style="width: 95%;margin: 0 auto;">
        <div id="shareDiv">
            <div>
                @Html.Raw(labels("Genius#whoShares"))
            </div>
            <br />
            <div>
                @(Html.DevExtreme().TagBox() _
                    .ID("ShareEmployeeText") _
                    .Placeholder(labels("Genius#selectUser")) _
                    .ValueExpr("ID") _
                    .DisplayExpr("Name") _
                    .SearchEnabled(True) _
                    .ShowSelectionControls(True) _
                    .ApplyValueMode(EditorApplyValueMode.Instantly) _
                    .DataSource(ViewBag.SharePassports))
            </div>
            <br />
            <div class="shareInfo">
                @Html.Raw(labels("Genius#shareDescription"))
            </div>
        </div>
    </div>
</div>
<br />
<br />

<div style="margin: 15px;position: absolute; bottom: 0;right: 0;">
    @(Html.DevExtreme().Button() _
                                        .ID("addUser") _
                                        .Icon("save") _
                                        .OnClick("onShareGeniusAccept") _
                                        .Type(ButtonType.Default))
</div>
</div>