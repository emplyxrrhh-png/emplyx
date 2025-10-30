Imports Robotics.Web.Base

Partial Class LoginRedirect
    Inherits NoCachePageBase

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("QueryString", "~/Base/Scripts/QueryStrings.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
    End Sub

End Class