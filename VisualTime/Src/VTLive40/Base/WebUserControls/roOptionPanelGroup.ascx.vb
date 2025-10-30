Partial Class roOptionPanelGroup
    Inherits System.Web.UI.UserControl

    Public OPanels As OrderedDictionary = New OrderedDictionary

    ''' <summary>
    ''' Afegeix un OPanelContainer a gestionar els events CheckedChanged
    ''' </summary>
    ''' <param name="oPanel">OPanelContainer</param>
    ''' <remarks></remarks>
    Public Sub addOPanel(ByVal oPanel As roOptionPanelContainer)
        OPanels.Add(oPanel.UniqueID, oPanel)
        AddHandler oPanel.CheckedChanged, AddressOf OPanelContainer1_CheckedChanged
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Protected Sub OPanelContainer1_CheckedChanged(ByVal sender As Object)
        Dim oPparent As roOptionPanelContainer = CType(sender, roOptionPanelContainer)

        If oPparent.Checked = False And oPparent.TypeOPanel <> roOptionPanelContainer.TypusOption.ImageOption Then Exit Sub
        Dim oPan As roOptionPanelContainer
        For Each oPanelEntry As DictionaryEntry In OPanels
            oPan = CType(oPanelEntry.Value, roOptionPanelContainer)
            If oPan.UniqueID = oPparent.UniqueID Then
                If oPparent.TypeOPanel = roOptionPanelContainer.TypusOption.ImageOption Then
                    oPparent.Checked = True
                End If
                Continue For
            End If

            oPan.Checked = False
        Next
    End Sub

End Class