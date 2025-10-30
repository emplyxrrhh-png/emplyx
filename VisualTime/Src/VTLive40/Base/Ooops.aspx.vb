Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class Ooops
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim customErrorMessage As String = String.Empty
        If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
            customErrorMessage = roTypes.Any2String(HttpContext.Current.Session("CustomErrorMessage"))
        End If

        If Not String.IsNullOrEmpty(customErrorMessage) Then
            Me.lblResult.Text = customErrorMessage
            HttpContext.Current.Session("CustomErrorMessage") = String.Empty
        End If


    End Sub

End Class