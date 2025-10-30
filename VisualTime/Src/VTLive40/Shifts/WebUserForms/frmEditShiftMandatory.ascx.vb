Imports Robotics.Web.Base

Partial Class WebUserForms_frmEditShiftMandatory
    Inherits UserControlBase

    Public Sub loadFormEditShiftMandatory()
        Try

            cmbStartSel.Items.Clear()
            cmbStartSel.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.FixedHour", DefaultScope), 0))
            cmbStartSel.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.BetweenHours", DefaultScope), 1))

            cmbEndSel.Items.Clear()
            cmbEndSel.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.FixedHour", DefaultScope), 0))
            cmbEndSel.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.ByEntrance", DefaultScope), 1))

            cmbStartAt1.Items.Clear()
            cmbStartAt1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbStartAt1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbStartAt1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

            cmbStartAt2ATo.Items.Clear()
            cmbStartAt2ATo.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbStartAt2ATo.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbStartAt2ATo.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

            cmbStartAt2AFrom.Items.Clear()
            cmbStartAt2AFrom.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbStartAt2AFrom.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbStartAt2AFrom.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

            cmbEndAt1.Items.Clear()
            cmbEndAt1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
            cmbEndAt1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
            cmbEndAt1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

            cmbRetInf.Items.Clear()
            cmbRetInf.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.TimeWorked", DefaultScope), 0))
            cmbRetInf.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.IgnoreIncidence", DefaultScope), 1))

            cmbIntInf.Items.Clear()
            cmbIntInf.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.TimeWorked", DefaultScope), 0))
            cmbIntInf.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.IgnoreIncidence", DefaultScope), 1))

            cmbSalAnt.Items.Clear()
            cmbSalAnt.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.TimeWorked", DefaultScope), 0))
            cmbSalAnt.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.IgnoreIncidence", DefaultScope), 1))
        Catch ex As Exception
        End Try
    End Sub

End Class