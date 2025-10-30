Imports Robotics.Web.Base

Partial Class Base_AccessDenied
    Inherits PageBase

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.btClose.Visible = (Me.Request("CloseButton") = "true")

    End Sub

End Class