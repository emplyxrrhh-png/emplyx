Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roTreeTaskContainer
    Inherits PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.Request("PrefixCookie") <> "" Then
            Me.oTreeTask.PrefixCookie = Me.Request("PrefixCookie")
        End If

        If Me.Request("AfterSelectFuncion") <> "" Then
            Me.oTreeTask.AfterSelectFuncion = Me.Request("AfterSelectFuncion")
        Else
            Me.oTreeTask.AfterSelectFuncion = String.Empty
        End If

    End Sub

End Class