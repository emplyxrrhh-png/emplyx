@Code

    Dim viewIcon = ViewData(VTLive40.Helpers.Constants.DefaultViewIcon)
    Dim viewTitle = ViewData(VTLive40.Helpers.Constants.DefaultViewTitle)
    Dim callbackFunc = "viewUtilsManager.changeTab"
    Dim barButtonData = ViewData(VTLive40.Helpers.Constants.DefaultBarButtonData)

    Dim buttonTabsData = ViewData(VTLive40.Helpers.Constants.DefaultTabBarButtonData)
End Code

<div id="divScreenSubTabsGeneric" class="w-100">
    <div class="dx-field">

        <div class="switchMainViewTabsGeneric SubTab w-100 pr-2" style="text-align:right">

            @For Each tab In buttonTabsData

                Dim tmpIdPath As String = tab.IDPath

                If tmpIdPath.Contains("TextBtn_") Then
                    tmpIdPath = "TextBtn"
                End If

            @<a Class='mainActionBtnGeneric viewTab @(If(buttonTabsData.IndexOf(tab) = 0, "activeTab", ""))'
                data-tabIntex="@buttonTabsData.IndexOf(tab)" data-toggle="pill"
                onclick="@Html.Raw(tab.AfterFunction)" style="text-decoration: none !important;"> @Html.Raw(tmpIdPath) </a>
            Next
        </div>
    </div>
</div>

<script>

    //document.onload = function () {
    //    //ChangeTab(0);
    //};
</script>