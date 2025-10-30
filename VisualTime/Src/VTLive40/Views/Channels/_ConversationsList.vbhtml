@Imports Robotics.Base.DTOs
@Imports VTLive40.Helpers

@Code
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
    Dim baseURL = Url.Content("~")
    Dim scriptVersion As String = ViewData(VTLive40.Helpers.Constants.ScriptVersion)

    Dim BaseIcon As String = ViewData("CardBaseIcon")
    Dim cardviewData = ViewData("ChannelConversation")
    Dim cardDataType As String = "Conversations"
    Dim SelectedCardId As String = If(ViewData("SelectedCardId") IsNot Nothing, ViewData("SelectedCardId"), "").ToString

    Dim channelName As String = iif(ViewData("ChannelName") = String.Empty, labels("Conversations#treeCaption"), ViewData("ChannelName"))
End Code

<link href="@Url.Content("~/Base/Styles/Live/roCardsTreeStyles.min.css")@Html.Raw(scriptVersion)" rel="stylesheet" type="text/css" />

<script src="@Url.Content("~/Base/Scripts/Live/roCardsTreeScript.js")@Html.Raw(scriptVersion)" type="text/javascript"></script>

<div class="conversationLeftPanel maxHeight conversationsTreeSize" id="divTree">
    <div class="conversationsTreeCaption">
        <span>@channelName</span>
    </div>

    <div id="customSearchPanel" class="dxcvSearchPanel">
        <table cellspacing="0" cellpadding="0" border="0" style="border-collapse:collapse; margin: auto;">
            <tbody>
                <tr>
                    <td style="width:100%;">
                        <table class="dxeButtonEditSys dxeButtonEdit dxeNullText dxh0" cellspacing="1" cellpadding="0" id="CardView_DXSE" border="0" style="width:100%;" savedspellcheck="[object Object]" spellcheck="false">
                            <tbody>
                                <tr>
                                    <td class="dxic" style="width:100%;">
                                        <input class="dxeEditArea dxeEditAreaSys dxh0" id="customSearchBar" type="text" placeholder="@labels("Conversations#searchText")">
                                    </td>
                                    <td id="hitSearchInnerScope" colspan="3"><span id="hitSearchBtn" title="@labels("Conversations#searchText")"><i class="fa fa-search"></i></span></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
        <div id="cardTreeSearchFilter" Class="menuCategories"></div>
    </div>

    @Html.Partial("_ConversationsCardView")
</div>