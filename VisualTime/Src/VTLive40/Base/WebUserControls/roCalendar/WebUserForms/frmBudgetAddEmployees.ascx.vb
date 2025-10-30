Imports Robotics.Web.Base

Partial Class frmBudgetAddEmployees
    Inherits UserControlBase

    Private Sub frmBudgetAddEmployees_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblInHolidays.ClientInstanceName = Me.ClientID & "_lblInHolidays"
        lblInRest.ClientInstanceName = Me.ClientID & "_lblInRest"
        lblOnAbsence.ClientInstanceName = Me.ClientID & "_lblOnAbsence"
        lblWithoutAssignment.ClientInstanceName = Me.ClientID & "_lblWithoutAssignment"

        If Not IsPostBack Then

        End If
    End Sub

End Class