Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roWizardSelectorContainer
    Inherits NoCachePageBase

    Private roTreesW As Base_WebUserControls_roTrees_roTrees

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        ' --------------- JQuery Librerias ------------------------------
        Me.InsertExtraJavascript("jquery", "~/Base/jquery/jquery-3.7.1.min.js")
        Me.InsertExtraJavascript("jquery-ui", "~/Base/jquery/jquery-ui.js")

        ' --------------  ExtJS Librerias -------------------------------
        Me.InsertExtraJavascript("ext-jquery-adapter", "~/Base/ext-3.4.0/ext-jquery-adapter.js")
        Me.InsertExtraJavascript("ext-all", "~/Base/ext-3.4.0/ext-all.js")

        ' -------------- Robotics -------------------------------
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js")
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js")
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js")
    End Sub

    ' ASP.base_webusercontrols_rotrees_ascx
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        'Parametres posibles a pasar a la plana
        'FilterFloat (true|false): Mostra el filtre flotant o integrat a la finestra
        'TreesEnabled (0:ocult, 1:visible) 3 arbres (ex: 100)
        'TreesMultiSelect (0:Normal, 1:MultiSelect) 3 arbres
        'TreesOnlyGroups (0:Normal, 1:Sols grups) 3 arbres
        'TreeFunction (nom de la funcio (sense parametres ni tancament, s'afegeix sol el parametre NODO)
        'PrefixTree
        'FiltersVisible
        'AdvancedFilterVisible
        'Filter1Class, Filter2Class, Filter3Class
        'Filter1LanguageKey, Filter2LanguageKey, Filter3LanguageKey
        'TreeImagePath
        'TreeSelectorPage
        'SelectedValues (Valors marcats)

        Dim FilterFloat As String = Me.Request("FilterFloat")
        If FilterFloat = "" Then FilterFloat = "False"

        Dim TreesEnabled As String = Me.Request("TreesEnabled")
        If TreesEnabled = "" Then TreesEnabled = "111"

        Dim TreesMultiSelect As String = Me.Request("TreesMultiSelect")
        If TreesMultiSelect = "" Then TreesMultiSelect = "000"

        Dim TreesOnlyGroups As String = Me.Request("TreesOnlyGroups")
        If TreesOnlyGroups = "" Then TreesOnlyGroups = "000"

        Dim TreeFunction As String = Me.Request("TreeFunction")
        If TreeFunction = "" Then TreeFunction = "parent.cargaNodo"

        'Filtre Flotant
        If FilterFloat.ToLower = "false" Then
            Me.roChildSelectorW.FilterFloat = False
        Else
            Me.roChildSelectorW.FilterFloat = True
        End If

        If Me.Request("PrefixTree") <> "" Then
            Me.roChildSelectorW.PrefixTree = Me.Request("PrefixTree")
            Me.roChildSelectorW.TreesBehaviorID = Me.Request("PrefixTree")
            'Me.roTreesW.ID = Me.Request("PrefixTree")
        End If
        Me.roTreesW = CType(LoadControl("~/Base/WebUserControls/roTrees/roTrees.ascx"), Base_WebUserControls_roTrees_roTrees) ' ASP.base_webusercontrols_rotrees_ascx

        roTreesW.ID = Me.PrefixTree
        Me.roChildSelectorW.TreeContainer.Controls.Add(Me.roTreesW)

        'Inicialitzacio d'usuaris
        Dim SelectedValues As String = Me.Request("SelectedValues")
        If SelectedValues <> "" Then
            HelperWeb.roSelector_Initialize("roChildSelectorW_" & Me.PrefixTree)
            If SelectedValues <> "ALL" Then
                Dim strEmp() As String = SelectedValues.Split(",")
                Dim oEmpLim As New Generic.List(Of Integer)
                For Each sEmp As String In strEmp
                    oEmpLim.Add(CInt(sEmp))
                Next
                HelperWeb.roSelector_SetSelection(oEmpLim, New Generic.List(Of Integer), "roChildSelectorW_" & Me.PrefixTree)
            End If
        End If

        'Trees Visibles
        If TreesEnabled.Substring(0, 1) = "1" Then Me.roTreesW.Tree1Visible = True Else Me.roTreesW.Tree1Visible = False
        If TreesEnabled.Substring(1, 1) = "1" Then Me.roTreesW.Tree2Visible = True Else Me.roTreesW.Tree2Visible = False
        If TreesEnabled.Substring(2, 1) = "1" Then Me.roTreesW.Tree3Visible = True Else Me.roTreesW.Tree3Visible = False

        'Trees multiselect
        If TreesMultiSelect.Substring(0, 1) = "1" Then Me.roTreesW.Tree1MultiSel = True Else Me.roTreesW.Tree1MultiSel = False
        If TreesMultiSelect.Substring(1, 1) = "1" Then Me.roTreesW.Tree2MultiSel = True Else Me.roTreesW.Tree2MultiSel = False
        If TreesMultiSelect.Substring(2, 1) = "1" Then Me.roTreesW.Tree3MultiSel = True Else Me.roTreesW.Tree3MultiSel = False

        'Trees sols grups
        If TreesOnlyGroups.Substring(0, 1) = "1" Then Me.roTreesW.Tree1ShowOnlyGroups = True Else Me.roTreesW.Tree1ShowOnlyGroups = False
        If TreesOnlyGroups.Substring(1, 1) = "1" Then Me.roTreesW.Tree2ShowOnlyGroups = True Else Me.roTreesW.Tree2ShowOnlyGroups = False
        If TreesOnlyGroups.Substring(2, 1) = "1" Then Me.roTreesW.Tree3ShowOnlyGroups = True Else Me.roTreesW.Tree3ShowOnlyGroups = False

        Me.roTreesW.Tree1Function = TreeFunction
        Me.roTreesW.Tree2Function = TreeFunction
        Me.roTreesW.Tree3Function = TreeFunction

        ' Permisos
        If Me.Request("FeatureAlias") <> "" Then
            Me.roTreesW.FeatureAlias = Me.Request("FeatureAlias")
        End If
        If Me.Request("FeatureType") <> "" Then
            Me.roTreesW.FeatureType = Me.Request("FeatureType")
        End If

        If Me.Request("FiltersVisible") <> "" Then
            Me.roChildSelectorW.FiltersVisible = Me.Request("FiltersVisible")
        End If

        If Me.Request("AdvancedFilterVisible") <> "" Then Me.roChildSelectorW.AdvancedFilterVisible = (Me.Request("AdvancedFilterVisible").ToLower = "true")

        If Me.Request("Filter1Class") <> "" Then Me.roChildSelectorW.Filter1Class = Me.Request("Filter1Class")
        If Me.Request("Filter1LanguageKey") <> "" Then Me.roChildSelectorW.Filter1LanguageKey = Me.Request("Filter1LanguageKey")
        If Me.Request("Filter2Class") <> "" Then Me.roChildSelectorW.Filter2Class = Me.Request("Filter2Class")
        If Me.Request("Filter2LanguageKey") <> "" Then Me.roChildSelectorW.Filter2LanguageKey = Me.Request("Filter2LanguageKey")
        If Me.Request("Filter3Class") <> "" Then Me.roChildSelectorW.Filter3Class = Me.Request("Filter3Class")
        If Me.Request("Filter3LanguageKey") <> "" Then Me.roChildSelectorW.Filter3LanguageKey = Me.Request("Filter3LanguageKey")

        If Me.Request("TreeImagePath") <> "" Then
            Me.roTreesW.Tree1ImagePath = Me.Request("TreeImagePath")
            Me.roTreesW.Tree2ImagePath = Me.Request("TreeImagePath")
            Me.roTreesW.Tree3ImagePath = Me.Request("TreeImagePath")
        End If

        If Me.Request("TreeSelectorPage") <> "" Then
            Me.roTreesW.Tree1SelectorPage = Me.Request("TreeSelectorPage")
            Me.roTreesW.Tree2SelectorPage = Me.Request("TreeSelectorPage")
            Me.roTreesW.Tree3SelectorPage = Me.Request("TreeSelectorPage")
        End If

        If Me.Request("EnableDD") <> "" Then
            Me.roTreesW.Tree1EnableDD = (Me.Request("EnableDD").Substring(0, 1) = "1")
            Me.roTreesW.Tree2EnableDD = (Me.Request("EnableDD").Substring(1, 1) = "1")
        End If

        If Me.Request("Tree1FunctDD") <> "" Then
            Me.roTreesW.Tree1FunctDD = Me.Request("Tree1FunctDD")
        End If
        If Me.Request("Tree2FunctDD") <> "" Then
            Me.roTreesW.Tree2FunctDD = Me.Request("Tree2FunctDD")
        End If

    End Sub

    Public ReadOnly Property PrefixTree() As String
        Get
            If Me.Request("PrefixTree") <> "" Then
                Return Me.Request("PrefixTree")
            Else
                Return "roTreesW"
            End If

        End Get
    End Property

    Public ReadOnly Property roTressWClientID() As String
        Get
            Return Me.roTreesW.ClientID
        End Get
    End Property

End Class