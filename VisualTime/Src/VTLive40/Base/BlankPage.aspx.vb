Imports Robotics.Web.Base

Partial Class BlankPage
    Inherits NoCachePageBase

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("jquery", "~/Base/jquery/jquery-3.7.1.min.js", , True)
        Me.InsertExtraJavascript("jqueryAdapter", "~/Base/ext-3.4.0/ext-jquery-adapter.js", , True)
        Me.InsertExtraJavascript("extAll", "~/Base/ext-3.4.0/ext-all.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not IsPostBack Then
            Me.LoadingBlankPanel.Text = Me.Language.Translate("main.loadingText", DefaultScope)
        End If

    End Sub

End Class