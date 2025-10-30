@imports Robotics.Base.DTOs

@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div id="customSearchPanel" class="dxcvSearchPanel">
    <table cellspacing="0" cellpadding="0" border="0" style="border-collapse:collapse;">
        <tbody>
            <tr>
                <td style="width:100%;">
                    <table class="dxeButtonEditSys dxeButtonEdit dxeNullText dxh0" cellspacing="1" cellpadding="0" id="CardView_DXSE" border="0" style="width:100%;" savedspellcheck="[object Object]" spellcheck="false">
                        <tbody>
                            <tr>
                                <td id="reportCategoriesInnerScope" colspan="3" style="display:none">
                                    <span id="reportCategoriesBtn">
                                        <nobr>
                                            <span>@ReportController.GetServerLanguage().Translate("roAllCategories", "ReportsDX")</span>
                                            <i class="fa fa-caret-down"></i>
                                        </nobr>
                                    </span>
                                </td>
                                <td Class="dxic" style="width:100%;">
                                    <input Class="dxeEditArea dxeEditAreaSys dxh0" id="customSearchBar" type="text" placeholder="@ReportController.GetServerLanguage().Translate("roPlaceholderSearchReportsBy", "ReportsDX")">
                                    <!--Html.DevExpress().Hint(Sub(settings)
                                        settings.Name = "Hint"
                                        settings.TargetSelector = "#customSearchBar"
                                        settings.AppearAfter = 300
                                        settings.DisappearAfter = 1000
                                        settings.Content = "Puedes utilizar la búsqueda avanzada con => clave:" & Chr(34) & "valor" & Chr(34)
                                        settings.Title = "Búsqueda avanzada"
                                        settings.ControlStyle.BorderWidth = 1
                                        settings.ShowCallout = True
                                        settings.Position = HintPosition.Bottom
                                    End Sub).GetHtml()-->
                                </td>
                                <td id="hitSearchInnerScope" colspan="3"><span id="hitSearchBtn" title="Buscar entre los informes"><i class="fa fa-search"></i></span></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
    <div class="menuCategories"></div>
</div>