Imports Robotics.Web.Base

Partial Class Base_EmployeeSelectorPopup
    Inherits PageBase

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load
        Me.InsertCssIncludes(Me.Page)

        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")

        If Not IsPostBack Then
            Me.endEvalFunc.Value = "parent.parent.frames['ifPrincipal']." & Request.Params("AfterSelectFuncion")
            Me.endEvalCookie.Value = Request.Params("PrefixCookie")
            Dim FeatureAlias As String
            If Request.Params("FeatureAlias") IsNot Nothing Then
                FeatureAlias = Request.Params("FeatureAlias")
            Else
                FeatureAlias = ""
            End If

            Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                          "PrefixTree=" & Request.Params("PrefixTree") & "&" &
                          "FeatureAlias=" & FeatureAlias & "&" &
                          "PrefixCookie=" & Request.Params("PrefixCookie") & "&" &
                          "AfterSelectFuncion=parent.parent.frames['ifPrincipal']." & Request.Params("AfterSelectFuncion") & "&" &
                          "Prefix=" & Request.Params("Prefix"))
        End If
    End Sub

End Class