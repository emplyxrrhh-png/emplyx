Imports Robotics.Web.Base

Partial Class WebUserForms_frmErrorDetail
    Inherits UserControlBase

    Private Sub WebUserForms_frmErrorDetail_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.txtErrorMemo.ClientInstanceName = Me.ClientID & "_txtErrorMemoClient"
        End If
    End Sub

End Class