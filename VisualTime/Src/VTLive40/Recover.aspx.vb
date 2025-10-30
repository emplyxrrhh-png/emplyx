Imports Robotics.Web.Base

Partial Class Recover
    Inherits PageBase

#Region "Events"

    Private Sub Recover_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.InsertJavascriptIncludes()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)

        Me.InsertCssIncludes(Me.Page)

        If WLHelperWeb.CurrentPassport() IsNot Nothing Then
            WLHelperWeb.RedirectDefault()
            Exit Sub
        End If

        If Not Me.IsPostBack Then
        End If

    End Sub

#End Region

End Class