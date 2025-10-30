Imports Robotics.Web.Base

Partial Class frmAddAssignment
    Inherits UserControlBase

    Public Sub loadFormAssignment()
        Try
            cmbAssignment.Items.Clear()
            Dim dTblAssignment As DataTable = API.AssignmentServiceMethods.GetAssignmentsDataTable(Me.Page, "Name", False)
            For Each dRowAR As DataRow In dTblAssignment.Rows
                cmbAssignment.Items.Add(New DevExpress.Web.ListEditItem(dRowAR("Name"), dRowAR("Id")))
            Next
        Catch ex As Exception
        End Try
    End Sub

End Class