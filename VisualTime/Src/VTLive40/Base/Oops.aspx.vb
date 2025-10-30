Imports Robotics.Web.Base

Public Class Oops
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.lblResult.Text = "Los datos del servicio SSO no se han podido validar correctamente"
    End Sub

End Class