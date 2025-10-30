Imports Robotics.Web.Base

Partial Class _Default
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

        If WLHelperWeb.CurrentPassport Is Nothing Then
            WLHelperWeb.RedirectNotAuthenticated()
        Else
            Dim url = "Main.aspx"
            If HttpContext.Current.Session IsNot Nothing Then
                Dim redirecUrl = HttpContext.Current.Session("URL2Redirect")
                If redirecUrl IsNot Nothing AndAlso redirecUrl <> String.Empty Then
                    url = "Main.aspx#" & redirecUrl
                End If
                HttpContext.Current.Session("URL2Redirect") = String.Empty
            End If

            WLHelperWeb.RedirectToUrl(Me.Page.ResolveUrl(url))
        End If

    End Sub

End Class