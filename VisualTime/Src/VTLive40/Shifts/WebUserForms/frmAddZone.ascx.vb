Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserForms_frmAddZone
    Inherits UserControlBase

    Public Sub loadFormAddZone()
        Try
            cmbShiftFromTime.Items.Clear()
            cmbShiftFromTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbShiftFromTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbShiftFromTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

            cmbShiftToTime.Items.Clear()
            cmbShiftToTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbShiftToTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbShiftToTime.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

            cmbType.Items.Clear()
            Dim dTblZ As DataTable = ShiftServiceMethods.GetTimeZones(Me.Page)
            For Each dRowZone As DataRow In dTblZ.Rows
                cmbType.Items.Add(New DevExpress.Web.ListEditItem(dRowZone("Name"), dRowZone("ID")))
            Next
        Catch ex As Exception

        End Try
    End Sub

End Class