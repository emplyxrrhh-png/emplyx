@Code

    Dim viewIcon = ViewData(VTLive40.Helpers.Constants.DefaultViewIcon)
    Dim viewTitle = ViewData(VTLive40.Helpers.Constants.DefaultViewTitle)
    Dim viewDescription = ViewData(VTLive40.Helpers.Constants.DefaultViewDescription)
    Dim callbackFunc = "viewUtilsManager.changeTab"
    Dim barButtonData = ViewData(VTLive40.Helpers.Constants.DefaultBarButtonData)
    Dim customChangeTab = ViewData(VTLive40.Helpers.Constants.CustomChangeTab)
    If customChangeTab Is Nothing OrElse customChangeTab.Equals("") Then
        customChangeTab = "undefined"
    End If
End Code

<div id="divScreenTabsGeneric">
    <div class="dx-field">

        <div class="ScreenIconTitleGeneric">
            <img src="@viewIcon" alt="Screen Icon" height="80" width="80">
            <div>
                <div>
                    <h1 id="ctl00_contentMainBody_lblHeader" class="NameText" style="margin: 10px 0 0 0;font-size: 20px;margin-left: 10px;font-weight: 600;"> @Html.Raw(viewTitle)</h1>
                </div>
                @If viewDescription.length > 0 Then
                @<div style="margin-left: 10px"><span id="readOnlyDescritionCompany"> @Html.Raw(viewDescription)</span></div>
                End If
            </div>
        </div>

        <div Class="switchMainViewTabsGeneric" style="text-align:center">

            @For Each tab In barButtonData
            @<a Class='mainActionBtnGeneric viewTab @(If(barButtonData.IndexOf(tab) = 0, "activeTab", ""))' data-tabIntex="@barButtonData.IndexOf(tab)" data-toggle="pill" onclick="@Html.Raw(callbackFunc & "(" & barButtonData.IndexOf(tab) & ")")" style="text-decoration: none !important;"> @Html.Raw(tab) </a>
                    Next
        </div>
    </div>
</div>

<script>

            //document.onload = function () {
            //    //ChangeTab(0);
            //};
</script>