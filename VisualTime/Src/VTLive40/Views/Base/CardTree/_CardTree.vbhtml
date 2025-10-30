@Imports VTLive40.Helpers
@Code
    Dim viewCaption As String = ViewData(Constants.DefaultViewCaption)
End Code

<link href="@Url.Content("~/Base/Styles/Live/roCardsTreeStyles.min.css")" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Base/Scripts/Live/roCardsTreeScript.js")" type="text/javascript"></script>

<div class="twoWindowsSidePanelGeneric maxHeight treeSize" id="divTree">
    <div class="treeCaption">
        <span>@viewCaption</span>

        @*<a href="javascript: void(0)" class="treeCaption-refreshBtn" title="@cardController.GetServerLanguage().Translate("refreshCardTree", "CardsTree")" onclick="refreshCardTree()"></a>*@
        @*<a href="javascript: void(0)" class="treeCaption-hideBtn" title="@cardController.GetServerLanguage().Translate("refreshCardTree", "CardsTree")" onclick="hideTree()"></a>*@
    </div>

    @Html.Partial("~/Views/Base/CardTree/_SearchCardsTreePanel.vbhtml")

    @Html.Partial("~/Views/Base/CardTree/_CardView.vbhtml")
</div>