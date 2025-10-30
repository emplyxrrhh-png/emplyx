Imports Robotics.Web.Base

Partial Class Base_ErrorPage
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            Dim Ex As Exception = Session("VTException")
            If Ex IsNot Nothing Then
                Me.lblMessage.Text = Me.Language.Translate("CodeException.Text", Me.DefaultScope)
                Me.lblDescription.Text = Me.Language.Translate("CodeException.Description", Me.DefaultScope)
            End If
        Catch ex As Exception
            Me.lblTitle.Text = Me.Language.Translate("CodeException.Title", Me.DefaultScope)
        End Try

    End Sub

End Class