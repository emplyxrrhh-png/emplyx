Imports Robotics.Web.Base

Partial Class WebUserForms_frmCopyOptions
    Inherits UserControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ckSPKeepHolidays.ClientInstanceName = Me.ClientID & "_ckSPKeepHolidaysClient"
        ckSPKeepBloquedDays.ClientInstanceName = Me.ClientID & "_ckSPKeepBloquedDaysClient"
    End Sub

End Class