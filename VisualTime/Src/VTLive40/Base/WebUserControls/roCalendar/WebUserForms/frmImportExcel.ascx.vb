Imports Robotics.Web.Base

Partial Class WebUserForms_frmImportExcel
    Inherits UserControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        rbExcelType1.ClientInstanceName = Me.ClientID & "_rbExcelType1"
        rbExcelType2.ClientInstanceName = Me.ClientID & "_rbExcelType2"
        ckImportCopyHolidays.ClientInstanceName = Me.ClientID & "_ckImportCopyHolidaysClient"
        ckImportCopyMainShifts.ClientInstanceName = Me.ClientID & "_ckImportCopyMainShiftsClient"
        ckImportKeepBloquedDays.ClientInstanceName = Me.ClientID & "_ckImportKeepBloquedDaysClient"
        ckImportKeepHolidays.ClientInstanceName = Me.ClientID & "_ckImportKeepHolidaysClient"
    End Sub

End Class