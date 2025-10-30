Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class WebUserForms_frmEditShiftBreak
    Inherits UserControlBase

    Public Sub loadFormEditShiftBreak()
        Try

            Me.divNotifications.Style("Display") = ""

            cmbCanAbsFrom.Items.Clear()
            cmbCanAbsFrom.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbCanAbsFrom.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbCanAbsFrom.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))
            cmbCanAbsFrom.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftHours", DefaultScope), 3))
            cmbCanAbsTo.Items.Clear()
            cmbCanAbsTo.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbCanAbsTo.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbCanAbsTo.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))
            cmbCanAbsTo.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftHours", DefaultScope), 3))
        Catch ex As Exception
            'do nothing
        End Try
    End Sub

End Class