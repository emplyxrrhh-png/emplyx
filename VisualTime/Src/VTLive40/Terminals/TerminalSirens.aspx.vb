Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Web.Base
Imports Robotics.WebControls

Partial Class Terminals_TerminalSirens
    Inherits PageBase

    Private oCurrentTerminal As roTerminal = Nothing

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Robotics
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)

        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("TerminalSirens", "~/Terminals/Scripts/TerminalSirens.js")

        'Scripts dels json2
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        'Scripts per control de webuserforms
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        'WebUserForms Clients (JS)
        Me.InsertExtraJavascript("frmAddSiren", "~/Terminals/Scripts/frmAddSiren.js")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.InsertCssIncludes(Me.Page)

        oCurrentTerminal = API.TerminalServiceMethods.GetTerminal(Me, Request("IDTerminal"), True)
        If oCurrentTerminal Is Nothing Then Exit Sub

        Me.lblSirensTitle.Text = Me.lblSirensTitle.Text.Replace("{1}", oCurrentTerminal.Description)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        Dim ocmbSirenWD As roComboBox = Me.frmAddSiren1.FindControl("cmbSirenWeekDay")

        ocmbSirenWD.AddItem(Me.Language.Keyword("weekday.1"), "1", "")
        ocmbSirenWD.AddItem(Me.Language.Keyword("weekday.2"), "2", "")
        ocmbSirenWD.AddItem(Me.Language.Keyword("weekday.3"), "3", "")
        ocmbSirenWD.AddItem(Me.Language.Keyword("weekday.4"), "4", "")
        ocmbSirenWD.AddItem(Me.Language.Keyword("weekday.5"), "5", "")
        ocmbSirenWD.AddItem(Me.Language.Keyword("weekday.6"), "6", "")
        ocmbSirenWD.AddItem(Me.Language.Keyword("weekday.7"), "7", "")

        Dim arrRelays() As String = oCurrentTerminal.SupportedSirens.Split(",")

        For Each strRelay As String In arrRelays
            cmbRelay.AddItem(strRelay, strRelay, "")
        Next

        cmbRelay.SelectedValue = oCurrentTerminal.SirensOutput

        Dim oPermission As Permission = Me.GetFeaturePermission("Terminals.Definition")
        If oPermission >= Permission.Write Then
            hdnModeEdit.Value = "false"
        Else
            hdnModeEdit.Value = "true"
        End If

    End Sub

End Class