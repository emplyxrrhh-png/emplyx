@Code
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
End Code

<div id="customSearchPanel" class="dxcvSearchPanel">
    <table cellspacing="0" cellpadding="0" border="0" style="border-collapse:collapse;">
        <tbody>
            <tr>
                <td style="width:100%;">
                    <table class="dxeButtonEditSys dxeButtonEdit dxeNullText dxh0" cellspacing="1" cellpadding="0" id="CardView_DXSE" border="0" style="width:100%;" savedspellcheck="[object Object]" spellcheck="false">
                        <tbody>
                            <tr>

                                <td class="dxic" style="width:100%;">
                                    <input class="dxeEditArea dxeEditAreaSys dxh0" id="customSearchBar" type="text" placeholder="@CommuniqueController.GetServerLanguage().Translate("roPlaceholderSearchReportsBy", "Communique")">
                                    <!--Html.DevExpress().Hint(Sub(settings)
                                        settings.Name = "Hint"
                                        settings.TargetSelector = "#customSearchBar"
                                        settings.AppearAfter = 300
                                        settings.Content = "Puedes utilizar la búsqueda avanzada con => clave:" & Chr(34) & "valor" & Chr(34)
                                        settings.Title = "Búsqueda avanzada"
                                        settings.ControlStyle.BorderWidth = 1
                                        settings.ShowCallout = True
                                        settings.Position = HintPosition.Bottom
                                    End Sub).GetHtml()-->
                                </td>
                                <td id="hitSearchInnerScope" colspan="3"><span id="hitSearchBtn" title="@CommuniqueController.GetServerLanguage().Translate("roSearchCommuniques", "Communique")"><i class="fa fa-search"></i></span></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
    <div class="menuCategories"></div>
</div>