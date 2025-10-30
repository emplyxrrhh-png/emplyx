Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roWizardSelectorContainerMultiSelectV3
    Inherits PageBase


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.Request("PrefixTree") <> "" Then
            Me.objContainerTreeV3.PrefixTree = Me.Request("PrefixTree")
        End If

        If Me.Request("FeatureAlias") <> "" Then
            Me.objContainerTreeV3.FeatureAlias = Me.Request("FeatureAlias")
        End If

        If Me.Request("PrefixCookie") <> "" Then
            Me.objContainerTreeV3.PrefixCookie = Me.Request("PrefixCookie")
        End If

        If Me.Request("AfterSelectFuncion") <> "" Then
            Me.objContainerTreeV3.AfterSelectFuncion = Me.Request("AfterSelectFuncion")
        Else
            Me.objContainerTreeV3.AfterSelectFuncion = String.Empty
        End If

        If Me.Request("FilterFixed") <> "" Then
            Me.objContainerTreeV3.FilterFixed = Me.Request("FilterFixed")
        Else
            Me.objContainerTreeV3.FilterFixed = String.Empty
        End If

        If Me.Request("EnableCustomFilters") <> "" Then
            Me.objContainerTreeV3.EnableCustomFilters = roTypes.Any2Boolean(Me.Request("EnableCustomFilters"))
        Else
            Me.objContainerTreeV3.EnableCustomFilters = True
        End If

    End Sub

End Class