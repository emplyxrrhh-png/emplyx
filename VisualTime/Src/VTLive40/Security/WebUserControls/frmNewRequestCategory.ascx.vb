Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class frmNewRequestCategory
    Inherits UserControlBase

    Protected Sub frmNewRequestCategory_Load(sender As Object, e As EventArgs) Handles Me.Load

        loadFormNewRequestCategory()

        If Not IsPostBack Then

        End If
    End Sub

    Private Sub loadFormNewRequestCategory()
        Try
            Try
                cmbRequestCategory.Items.Clear()
                cmbLevel.Items.Clear()
                cmbNextLevel.Items.Clear()
                cmbRequestCategory.ValueType = GetType(Integer)

                Dim dTbl As List(Of roSecurityCategory) = SecurityV3ServiceMethods.GetRequestCategories(Me.Page)

                If dTbl IsNot Nothing Then
                    For Each dRow As roSecurityCategory In dTbl
                        cmbRequestCategory.Items.Add(New DevExpress.Web.ListEditItem(dRow.Description, dRow.ID))
                    Next
                End If

                For index As Integer = 1 To 11
                    cmbLevel.Items.Add(index.ToString, index)
                    cmbNextLevel.Items.Add(index.ToString, index)
                Next
            Catch ex As Exception
            End Try
        Catch ex As Exception
        End Try
    End Sub

End Class