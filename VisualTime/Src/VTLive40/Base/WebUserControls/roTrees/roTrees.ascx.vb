Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roTrees_roTrees
    Inherits UserControlBase

#Region "Declarations"

    Private m_cookiePrefix As String = "" 'Prefix per possar davant les cookies
    Private bolFirstClick As Boolean = True

#End Region

#Region "Properties"

    Public Property cookiePrefix() As String
        Get
            Return Me.m_cookiePrefix
        End Get
        Set(ByVal value As String)
            Me.m_cookiePrefix = value
        End Set
    End Property

    ''' <summary>
    ''' 1er Tree Visible (Arbre Normal)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree1Visible() As Boolean
        Get
            If ViewState("Tree1Visible") Is Nothing Then
                Return True
            Else
                Return ViewState("Tree1Visible")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree1Visible") = value
        End Set
    End Property

    ''' <summary>
    ''' 2n Tree Visible (Arbre Agrupacions)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree2Visible() As Boolean
        Get
            If ViewState("Tree2Visible") Is Nothing Then
                Return True
            Else
                Return ViewState("Tree2Visible")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree2Visible") = value
        End Set
    End Property

    ''' <summary>
    ''' 3er Tree Visible (Arbre busqueda)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree3Visible() As Boolean
        Get
            If ViewState("Tree3Visible") Is Nothing Then
                Return True
            Else
                Return ViewState("Tree3Visible")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree3Visible") = value
        End Set
    End Property

    ''' <summary>
    ''' 1er Tree MultiSelect (Arbre Normal)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree1MultiSel() As Boolean
        Get
            If ViewState("Tree1MultiSel") Is Nothing Then
                Return False
            Else
                Return ViewState("Tree1MultiSel")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree1MultiSel") = value
        End Set
    End Property

    ''' <summary>
    ''' 2n Tree MultiSelect (Arbre Agrupacions)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree2MultiSel() As Boolean
        Get
            If ViewState("Tree2MultiSel") Is Nothing Then
                Return False
            Else
                Return ViewState("Tree2MultiSel")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree2MultiSel") = value
        End Set
    End Property

    ''' <summary>
    ''' 3er Tree MultiSelect (Arbre busqueda)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree3MultiSel() As Boolean
        Get
            If ViewState("Tree3MultiSel") Is Nothing Then
                Return False
            Else
                Return ViewState("Tree3MultiSel")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree3MultiSel") = value
        End Set
    End Property

    ''' <summary>
    ''' 1er Tree Sols Grups (Arbre Normal)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree1ShowOnlyGroups() As Boolean
        Get
            If ViewState("Tree1ShowOnlyGroups") Is Nothing Then
                Return False
            Else
                Return ViewState("Tree1ShowOnlyGroups")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree1ShowOnlyGroups") = value
        End Set
    End Property

    ''' <summary>
    ''' 2n Tree Sols Grups (Arbre Agrupacions)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree2ShowOnlyGroups() As Boolean
        Get
            If ViewState("Tree2ShowOnlyGroups") Is Nothing Then
                Return False
            Else
                Return ViewState("Tree2ShowOnlyGroups")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree2ShowOnlyGroups") = value
        End Set
    End Property

    ''' <summary>
    ''' 3er Tree Sols Grups (Arbre busqueda)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree3ShowOnlyGroups() As Boolean
        Get
            If ViewState("Tree3ShowOnlyGroups") Is Nothing Then
                Return False
            Else
                Return ViewState("Tree3ShowOnlyGroups")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree3ShowOnlyGroups") = value
        End Set
    End Property

    ''' <summary>
    ''' 1er Arbre Funció Javascript (sense parentesis)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree1Function() As String
        Get
            If ViewState("Tree1Function") Is Nothing Then
                Return "cargaNodo"
            Else
                Return ViewState("Tree1Function")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree1Function") = value
        End Set
    End Property

    ''' <summary>
    ''' 2n Arbre Funció Javascript (sense parentesis)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree2Function() As String
        Get
            If ViewState("Tree2Function") Is Nothing Then
                Return "cargaNodo"
            Else
                Return ViewState("Tree2Function")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree2Function") = value
        End Set
    End Property

    ''' <summary>
    ''' 3er Arbre Funció Javascript (sense parentesis)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree3Function() As String
        Get
            If ViewState("Tree3Function") Is Nothing Then
                Return "cargaNodo"
            Else
                Return ViewState("Tree3Function")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree3Function") = value
        End Set
    End Property

    ''' <summary>
    ''' 1er Arbre Plana web de carrega
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree1SelectorPage() As String
        Get
            If ViewState("Tree1SelectorPage") Is Nothing Then
                Return "EmployeeSelectorData.aspx"
            Else
                Return ViewState("Tree1SelectorPage")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree1SelectorPage") = value
        End Set
    End Property

    ''' <summary>
    ''' 2on Arbre Plana web de carrega
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree2SelectorPage() As String
        Get
            If ViewState("Tree2SelectorPage") Is Nothing Then
                Return "EmployeeSelectorData.aspx"
            Else
                Return ViewState("Tree2SelectorPage")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree2SelectorPage") = value
        End Set
    End Property

    ''' <summary>
    ''' 3er Arbre Plana web de carrega
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree3SelectorPage() As String
        Get
            If ViewState("Tree3SelectorPage") Is Nothing Then
                Return "EmployeeSelectorData.aspx"
            Else
                Return ViewState("Tree3SelectorPage")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree3SelectorPage") = value
        End Set
    End Property

    ''' <summary>
    ''' 1er Arbre URL imatges
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree1ImagePath() As String
        Get
            If ViewState("Tree1ImagePath") Is Nothing Then
                Return "../images/EmployeeSelector"
            Else
                Return ViewState("Tree1ImagePath")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree1ImagePath") = value
        End Set
    End Property

    ''' <summary>
    ''' 2on Arbre URL imatges
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree2ImagePath() As String
        Get
            If ViewState("Tree2ImagePath") Is Nothing Then
                Return "../images/EmployeeSelector"
            Else
                Return ViewState("Tree2ImagePath")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree2ImagePath") = value
        End Set
    End Property

    ''' <summary>
    ''' 3er Arbre URL imatges
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Tree3ImagePath() As String
        Get
            If ViewState("Tree3ImagePath") Is Nothing Then
                Return "../images/EmployeeSelector"
            Else
                Return ViewState("Tree3ImagePath")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree3ImagePath") = value
        End Set
    End Property

    Public Property FirstClick() As Boolean
        Get
            If ViewState("FirstClick") Is Nothing Then
                Return bolFirstClick
            Else
                Return ViewState("FirstClick")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("FirstClick") = value
            bolFirstClick = value
        End Set
    End Property

    ''' <summary>
    ''' Path per resoldre la ruta de les crides en Ajax
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ResolvePath() As String
        Get
            If ViewState("ResolvePath") Is Nothing Then
                Dim rePath As String = Me.Page.ResolveUrl("~/Base/WebUserControls/EmployeeSelectorData.aspx")
                If rePath.Length > 0 Then
                    rePath = Mid(rePath, 1, InStrRev(rePath, "/"))
                    Return rePath
                Else
                    Return ""
                End If
            Else
                Return ViewState("ResolvePath")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("ResolvePath") = value
        End Set
    End Property

    ''' <summary>
    ''' roMainSelector Id (si el conte)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property roMainSelectorID() As String
        Get
            If ViewState("roMainSelectorID") Is Nothing Then
                Return ""
            Else
                Return ViewState("roMainSelectorID")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("roMainSelectorID") = value
        End Set
    End Property

    ''' <summary>
    ''' Permitir DragDrop para el árbol 1
    ''' </summary>
    Public Property Tree1EnableDD() As Boolean
        Get
            If ViewState("Tree1EnableDD") Is Nothing Then
                Return False
            Else
                Return ViewState("Tree1EnableDD")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree1EnableDD") = value
        End Set
    End Property

    ''' <summary>
    ''' Permitir DragDrop para el árbol 2
    ''' </summary>
    Public Property Tree2EnableDD() As Boolean
        Get
            If ViewState("Tree2EnableDD") Is Nothing Then
                Return False
            Else
                Return ViewState("Tree2EnableDD")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Tree2EnableDD") = value
        End Set
    End Property

    ''' <summary>
    ''' Función cliente que se dispara cuando un node se ha movido con DragDrop para el árbol 1
    ''' </summary>
    Public Property Tree1FunctDD() As String
        Get
            If ViewState("Tree1FunctDD") Is Nothing Then
                Return ""
            Else
                Return ViewState("Tree1FunctDD")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree1FunctDD") = value
        End Set
    End Property

    ''' <summary>
    ''' Función cliente que se dispara cuando un node se ha movido con DragDrop para el árbol 2
    ''' </summary>
    Public Property Tree2FunctDD() As String
        Get
            If ViewState("Tree2FunctDD") Is Nothing Then
                Return ""
            Else
                Return ViewState("Tree2FunctDD")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree2FunctDD") = value
        End Set
    End Property

    Public Property FeatureAlias() As String
        Get
            If ViewState("FeatureAlias") Is Nothing Then
                Return ""
            Else
                Return ViewState("FeatureAlias")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("FeatureAlias") = value
        End Set
    End Property

    Public Property FeatureType() As String
        Get
            If ViewState("FeatureType") Is Nothing Then
                Return "U"
            Else
                Return ViewState("FeatureType")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("FeatureType") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            IsScriptManagerInParent()

            Dim oTreeState As roTreeState = Nothing

            If Me.Tree2Visible Or Me.Tree3Visible Then
                oTreeState = HelperWeb.roSelector_GetTreeState(Me.ClientID)
            End If

            'Arbol Agrupacion
            If Me.Tree2Visible Then
                Me.cmbAgrupacio.Text = ""
                Dim dTbl As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "AccessLevel <> 2", False)
                Dim dRows() As DataRow = dTbl.Select(Nothing, "FieldName")
                cmbAgrupacio.ClearItems()
                For Each dRow As DataRow In dRows
                    If dRow("Used") = False Then Continue For
                    cmbAgrupacio.AddItem(dRow("FieldName"), "setUserField('" & dRow("FieldName") & "','" & Me.ClientID & "'); eval('" & Me.ClientID & "_roTrees.LoadTree(""2"");'); return false;")
                Next

                ' Obtener campo ficha a utilizar en el segundo árbol
                Dim strSelectedUserField As String = oTreeState.UserField
                ' Verificamos que el campo de la ficha exista en la lista actual
                If dTbl.Select("FieldName = '" & strSelectedUserField.Replace("'", "''") & "'", "").Length > 0 Then
                    Me.cmbAgrupacio.Text = oTreeState.UserField
                End If

                Me.tabTree02.Title = Me.Language.Translate("TreePersonalizedView", "roSelector")
                Me.tabTree02.Attributes("onclick") = "chTree('2','" & Me.ClientID & "','" & Me.ClientID & "');"
            End If
            'Fin Arbol Agrupacion

            'Arbol BuscarPor
            If Me.Tree3Visible Then
                Me.cmbFieldFind.ClearItems()
                Me.cmbFieldFind.AddItem(Me.Language.Translate("FieldSearch.EmployeeName", "roSelector"), "EmployeeName", "setFieldFind('EmployeeName', '','" & Me.ClientID & "'); document.getElementById('" & Me.ClientID & "_FieldFindValue').value=''; eval('" & Me.ClientID & "_roTrees.LoadTree(""3"");'); return false;")
                Me.cmbFieldFind.AddItem(Me.Language.Translate("FieldSearch.idContract", "roSelector"), "idContract", "setFieldFind('idContract','','" & Me.ClientID & "'); document.getElementById('" & Me.ClientID & "_FieldFindValue').value=''; eval('" & Me.ClientID & "_roTrees.LoadTree(""3"");'); return false;")
                Me.cmbFieldFind.AddItem(Me.Language.Translate("FieldSearch.idCard", "roSelector"), "idCard", "setFieldFind('idCard','','" & Me.ClientID & "'); document.getElementById('" & Me.ClientID & "_FieldFindValue').value=''; eval('" & Me.ClientID & "_roTrees.LoadTree(""3"");'); return false;")
                Me.cmbFieldFind.AddItem(Me.Language.Translate("FieldSearch.idEmployee", "roSelector"), "idEmployee", "setFieldFind('idEmployee','','" & Me.ClientID & "'); document.getElementById('" & Me.ClientID & "_FieldFindValue').value=''; eval('" & Me.ClientID & "_roTrees.LoadTree(""3"");'); return false;")
                Me.cmbFieldFind.AddItem(Me.Language.Translate("FieldSearch.advFilter", "roSelector"), "advFilter", "setFieldFind('advFilter','','" & Me.ClientID & "'); document.getElementById('" & Me.ClientID & "_FieldFindValue').value=''; eval('" & Me.ClientID & "_roTrees.LoadTree(""3"");'); return false;")

                If HelperSession.GetFeatureIsInstalledFromApplication("Feature\TerminalConnector") Then
                    Me.cmbFieldFind.AddItem(Me.Language.Translate("FieldSearch.Plate", "roSelector"), "Plate", "setFieldFind('Plate','','" & Me.ClientID & "'); document.getElementById('" & Me.ClientID & "_FieldFindValue').value=''; eval('" & Me.ClientID & "_roTrees.LoadTree(""3"");'); return false;")
                End If

                ' Obtener campo ficha a utilizar en el segundo árbol
                Me.cmbFieldFind.Value = oTreeState.FieldFindColumn
                Me.cmbFieldFind.SelectedValue = oTreeState.FieldFindColumn

                Me.tabTree03.Title = Me.Language.Translate("TreeListView", "roSelector")
                Me.tabTree03.Attributes("onclick") = "chTree('3','" & Me.ClientID & "','" & Me.ClientID & "');"
            End If
            'Fin Arbol BuscarPor

            Me.tabTree01.Title = Me.Language.Translate("TreeNormalView", "roSelector")
            Me.tabTree01.Attributes("onclick") = "chTree('1','" & Me.ClientID & "','" & Me.ClientID & "');"

            If Me.Tree1Visible = True And Me.Tree2Visible = False And Me.Tree3Visible = False Then
                Me.Tabs.Visible = False
            Else
                If Me.Tree1Visible = False Then Me.tabTree01.Style("display") = "none"
                If Me.Tree2Visible = False Then Me.tabTree02.Style("display") = "none"
                If Me.Tree3Visible = False Then Me.tabTree03.Style("display") = "none"
            End If
        Catch
        End Try

    End Sub

#End Region

#Region "Methods"

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
        cacheManager.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roTreeCheckNode", "~/Base/Scripts/rocontrols/roTrees/TreeCheckNode.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roTree", "~/Base/Scripts/rocontrols/roTrees/roTree.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roTrees", "~/Base/Scripts/rocontrols/roTrees/roTrees.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js", Me.Parent.Page)

        Return True
    End Function

#End Region

End Class