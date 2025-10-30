@Imports VTLive40.Helpers
@Code
    Dim viewTitle = ViewData(Constants.DefaultViewTitle)
    Dim viewCaption As String = ViewData(Constants.DefaultViewCaption)
    Dim buttonTabsData = ViewData(Constants.DefaultTabBarButtonData)
End Code

<div id="divBarButtons" class="maxHeight">
    <div class="middleBarButtonsMain">
        <div style="height:100%;vertical-align:top;">

            <a id="unhideTreeBtnWrapper" style="display:none;" Class='mainActionBtn' href="#" title="@viewTitle">
                @(Html.DevExtreme().Button() _
                    .ID("unhideTreeBtn") _
                    .Icon("chevronright") _
                    .OnClick("function() { hideTree(); }"))
            </a>

            @For Each guiAction In buttonTabsData

                Dim tmpIdPath As String = guiAction.IDPath

                If tmpIdPath.Contains("TextBtn_") Then
                    tmpIdPath = "TextBtn"
                End If

            @Html.Partial("~/Views/Base/TabBar/_DBButton.vbhtml", guiAction)
            Next guiAction
        </div>
    </div>
</div>