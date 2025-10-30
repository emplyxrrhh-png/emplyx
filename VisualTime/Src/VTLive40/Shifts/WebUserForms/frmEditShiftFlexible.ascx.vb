Imports Robotics.Web.Base

Partial Class WebUserForms_frmEditShiftFlexible
    Inherits UserControlBase

    Public Sub loadFormEditShiftFlexible()
        Try
            cmbPresFromTime.Items.Clear()
            cmbPresFromTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbPresFromTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbPresFromTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

            cmbPresToTime.Items.Clear()
            cmbPresToTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbPresToTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbPresToTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

            cmbMaxThen.Items.Clear()
            cmbMaxThen.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Flexible.GenerateExtraHours", DefaultScope), 0))
            cmbMaxThen.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Flexible.GenerateIncidence", DefaultScope), 1))
        Catch ex As Exception
            Response.Write(ex.StackTrace)
        End Try
    End Sub

End Class