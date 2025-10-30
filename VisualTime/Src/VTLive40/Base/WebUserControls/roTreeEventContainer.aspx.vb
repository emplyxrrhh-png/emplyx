Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roTreeEventContainer
    Inherits PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.Request("PrefixCookie") <> "" Then
            Me.oTreeEvent.PrefixCookie = Me.Request("PrefixCookie")
        End If

        If Me.Request("AfterSelectFuncion") <> "" Then
            Me.oTreeEvent.AfterSelectFuncion = Me.Request("AfterSelectFuncion")
        Else
            Me.oTreeEvent.AfterSelectFuncion = String.Empty
        End If

    End Sub

End Class