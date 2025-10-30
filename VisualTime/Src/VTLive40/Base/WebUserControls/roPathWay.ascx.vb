Imports Robotics.Web.Base

Partial Class WebUserControls_roPathWay
    Inherits UserControlBase

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.buttonIcoHome.HRef = Request.ApplicationPath
    End Sub

End Class