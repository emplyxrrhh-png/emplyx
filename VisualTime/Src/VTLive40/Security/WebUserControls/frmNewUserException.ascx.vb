Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class frmNewUserException
    Inherits UserControlBase

    Protected Sub frmNewUserException_Load(sender As Object, e As EventArgs) Handles Me.Load

        loadFormNewRequestCategory()

    End Sub

    Private Sub loadFormNewRequestCategory()

        Try
            cmbPassportAvailable.Items.Clear()

            Dim dTbl As DataTable = API.EmployeeServiceMethods.GetAllEmployees(Nothing, "", "")

            Dim listItems As New Generic.List(Of DevExpress.Web.ListEditItem)
            If dTbl IsNot Nothing Then
                For Each dRow As DataRow In dTbl.Rows
                    listItems.Add(New DevExpress.Web.ListEditItem($"{dRow("EmployeeName")} ({dRow("FullGroupName")})", dRow("IDEmployee")))
                Next
            End If

            cmbPassportAvailable.Items.AddRange(listItems.OrderBy(Function(x) x.Text).ToArray())

        Catch ex As Exception
        End Try
    End Sub

End Class