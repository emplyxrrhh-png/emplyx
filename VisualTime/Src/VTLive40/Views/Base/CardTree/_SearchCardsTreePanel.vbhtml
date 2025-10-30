@Imports VTLive40.Helpers
@Imports Robotics.VTBase
@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
    Dim viewTitle = ViewData(Constants.DefaultViewTitle)
    Dim viewCaption As String = ViewData(Constants.DefaultViewCaption)
    Dim viewCategories As Boolean = ViewData(Constants.DefaultViewCategories)
    Dim searchText As String = ViewData(Constants.SearchText)
    Dim filterText As String = ViewData(Constants.FilterText)
End Code

<div id="customSearchPanel" class="dxcvSearchPanel">
    <table cellspacing="0" cellpadding="0" border="0" style="border-collapse:collapse; margin: auto;">
        <tbody>
            <tr>
                <td style="width:100%;">
                    <table class="dxeButtonEditSys dxeButtonEdit dxeNullText dxh0" cellspacing="1" cellpadding="0" id="CardView_DXSE" border="0" style="width:100%;" savedspellcheck="[object Object]" spellcheck="false">
                        <tbody>
                            @If viewCategories Then
                            @<tr>
                                <td id="filterInnerScope" colspan="2">
                                    <div class="cardViewFilterButton" id="filterListBtn">
                                        <nobr>
                                            <span>
                                                @If filterText.Equals("") Then
                                                    ReportController.GetServerLanguage().Translate("roAllCategories", "ReportsDX")
                                                Else
                                                @filterText
                                                End If
                                            </span>
                                            <i id="searchButtonDirection" class="fa fa-caret-right"></i>
                                        </nobr>
                                    </div>
                                </td>
                            </tr>
                            End If
                            <tr>
                                <td class="dxic" style="width:100%;">
                                    <input class="dxeEditArea dxeEditAreaSys dxh0" id="customSearchBar" type="text" placeholder="@searchText">
                                </td>
                                <td id="hitSearchInnerScope" colspan="3"><span id="hitSearchBtn" title="@searchText"><i class="fa fa-search"></i></span></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
    <div id="cardTreeSearchFilter" Class="menuCategories"></div>
</div>