Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class AccessZoomZone
    Inherits PageBase

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/OpenWindow.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Access.Zones", Permission.Read) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

    End Sub

End Class