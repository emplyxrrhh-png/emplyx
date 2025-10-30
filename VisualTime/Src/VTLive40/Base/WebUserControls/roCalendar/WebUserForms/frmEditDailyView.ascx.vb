Imports Robotics.Web.Base

Partial Class WebUserForms_frmEditDailyView
    Inherits UserControlBase

    Private Sub WebUserForms_frmErrorDetail_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.rbViewModes.ClientInstanceName = Me.ClientID & "_rbViewModesClient"

            Me.rbViewModes.Items.Clear()
            Me.rbViewModes.Items.Add(Language.Translate("DailyPeriod.15", DefaultScope), 15)
            Me.rbViewModes.Items.Add(Language.Translate("DailyPeriod.30", DefaultScope), 30)
            Me.rbViewModes.Items.Add(Language.Translate("DailyPeriod.60", DefaultScope), 60)
        End If

    End Sub

End Class