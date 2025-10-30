Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roFilterTaskTemplateSelector
    Inherits PageBase

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("jquery", "~/Base/jquery/jquery-3.7.1.min.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not IsPostBack Then

            '--> FeatureAlias("A definir!!!!")
            'If Me.HasFeaturePermission(FeatureAlias, Permission.Read) Then
            '    WLHelperWeb.RedirectAccessDenied(False)
            '    Exit Sub
            'End If

            Me.hdnSelectedValue.Value = "-1"

        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        Dim bolCorrect As Boolean = False

        Dim i As String = Me.hdnSelectedValue.Value
        If i <> "-1" Then
            bolCorrect = True
        End If

        If bolCorrect Then
            Me.hdnCanClose.Value = "1"
        Else
            Me.hdnCanClose.Value = "0"
        End If

    End Sub

End Class