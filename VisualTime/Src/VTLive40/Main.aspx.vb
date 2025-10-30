Imports Robotics.Web.Base

Partial Class _Main
    Inherits PageBase

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("dhtmlHistory", "~/Base/Scripts/dhtmlHistory.js", , True)
        Me.InsertExtraJavascript("UserHistory", "~/Base/Scripts/roUserHistory.js", , True)
        Me.InsertExtraJavascript("UserTasks", "~/Scripts/UserTasks.js", , True)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If WLHelperWeb.CurrentPassport IsNot Nothing Then
            ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "startTimers", "showLoader(true);setTimeout(function(){ RefreshUserTasks(true); },300);", True)
        Else
            WLHelperWeb.RedirectNotAuthenticated()
        End If

        If Not IsPostBack Then
            Me.LoadingPanelMain.Text = Me.Language.Translate("main.loadingText", DefaultScope)
            'If Me.ifPrincipal.Src = "about:blank" Then
            '    Me.ifPrincipal.Src = "Start"
            'End If
        End If

        Session.Timeout = 86400

    End Sub

    Protected Sub btSignOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSignOut.Click

        WLHelperWeb.SignOut(Me.Page, WLHelperWeb.CurrentPassport)
        WLHelperWeb.RedirectNotAuthenticated()
    End Sub

End Class