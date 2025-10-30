Imports Robotics.Web.Base

Partial Class EmployeePermissions
    Inherits PageBase

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("EmployeePermissions", "~/Security/Scripts/EmployeePermissions.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Dim script As ClientScriptManager = Me.Page.ClientScript

        If Not IsPostBack Then

            Me.hdnID.Value = Request("ID")
            Me.hdnIDFeature.Value = Request("IDFeature")

            Dim oParam As New Generic.List(Of String)
            oParam.Add(Request("FeatureName"))
            Me.lblDescription.Text = Me.Language.Translate("EmployeePermissions.Description", Me.DefaultScope, oParam)

            HelperWeb.roSelector_Initialize("roChildSelectorW_treeEmployeesPermissions")
            Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/WebUserControls/roWizardSelectorContainer.aspx?TreesEnabled=111&TreesMultiSelect=000&TreesOnlyGroups=000&TreeFunction=parent.EmployeeSelected&FilterFloat=false&PrefixTree=treeEmployeesPermissions")

        End If

    End Sub

End Class