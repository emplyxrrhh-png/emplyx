Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.WebControls

Partial Class Base_WebUserControls_roTrees_roTreesSelector
    Inherits UserControlBase

#Region "Declarations"

    Private m_cookiePrefix As String = String.Empty 'Prefix per possar davant les cookies

    Dim Criteris(,) As String = {
                                    {"Criteria.Equal", "="},
                                    {"Criteria.Major", ">"},
                                    {"Criteria.MajororEqual", ">="},
                                    {"Criteria.Minor", "<"},
                                    {"Criteria.MinororEqual", "<="},
                                    {"Criteria.Different", "<>"},
                                    {"Criteria.StartsWith", "*"},
                                    {"Criteria.Contains", "**"}
                                    }

    Protected mJSFilterShow As String = "filterFloatVisible('" & Me.ClientID & "');"
    Protected AdvancedFilterDisplay As String = String.Empty

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

    Public Property FilterFloat() As Boolean
        Get
            If ViewState("FilterFloat") Is Nothing Then
                Return True
            Else
                Return ViewState("FilterFloat")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("FilterFloat") = value
            If value = True Then
                mJSFilterShow = "filterFloatVisible('" & Me.ClientID & "');"
            Else
                mJSFilterShow = "filterEmbeddedVisible('" & Me.ClientID & "');"
            End If
        End Set
    End Property

    Public Property PrefixTree() As String
        Get
            If ViewState("PrefixTree") Is Nothing Then
                Return String.Empty
            Else
                Return ViewState("PrefixTree")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("PrefixTree") = value
        End Set
    End Property

    Public Property FiltersVisible() As String
        Get
            If ViewState("FiltersVisible") Is Nothing Then
                Return "1111"
            Else
                Return ViewState("FiltersVisible")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("FiltersVisible") = value
        End Set
    End Property

    Public Property AdvancedFilterVisible() As Boolean
        Get
            If ViewState("AdvancedFilterVisible") Is Nothing Then
                Return True
            Else
                Return ViewState("AdvancedFilterVisible")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("AdvancedFilterVisible") = value
            Me.AdvancedFilterDisplay = IIf(value, String.Empty, "none")
        End Set
    End Property

    Public Property AfterSelectFilterFuncion() As String
        Get
            If ViewState("AfterSelectFilterFuncion") Is Nothing Then
                hdnAfterSelectFilterFuncion.Value = String.Empty
                Return String.Empty
            Else
                Return ViewState("AfterSelectFilterFuncion")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("AfterSelectFilterFuncion") = value
            hdnAfterSelectFilterFuncion.Value = value
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
                Return False
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
                Return False
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
                Return "../" & ViewState("Tree1ImagePath")
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
                Return "../" & ViewState("Tree2ImagePath")
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
                Return "../" & ViewState("Tree3ImagePath")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Tree3ImagePath") = value
        End Set
    End Property

    Public Property Filter1Class() As String
        Get
            If ViewState("Filter1Class") Is Nothing Then
                Return "icoFilter1_24"
            Else
                Return ViewState("Filter1Class")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter1Class") = value
        End Set
    End Property

    Public Property Filter2Class() As String
        Get
            If ViewState("Filter2Class") Is Nothing Then
                Return "icoFilter2_24"
            Else
                Return ViewState("Filter2Class")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter2Class") = value
        End Set
    End Property

    Public Property Filter3Class() As String
        Get
            If ViewState("Filter3Class") Is Nothing Then
                Return "icoFilter3_24"
            Else
                Return ViewState("Filter3Class")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter3Class") = value
        End Set
    End Property

    Public Property Filter4Class() As String
        Get
            If ViewState("Filter4Class") Is Nothing Then
                Return "icoFilter4_24"
            Else
                Return ViewState("Filter4Class")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter4Class") = value
        End Set
    End Property

    Public Property Filter1LanguageKey() As String
        Get
            If ViewState("Filter1LanguageKey") Is Nothing Then
                Return "ttFilter1"
            Else
                Return ViewState("Filter1LanguageKey")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter1LanguageKey") = value
        End Set
    End Property

    Public Property Filter2LanguageKey() As String
        Get
            If ViewState("Filter2LanguageKey") Is Nothing Then
                Return "ttFilter2"
            Else
                Return ViewState("Filter2LanguageKey")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter2LanguageKey") = value
        End Set
    End Property

    Public Property Filter3LanguageKey() As String
        Get
            If ViewState("Filter3LanguageKey") Is Nothing Then
                Return "ttFilter3"
            Else
                Return ViewState("Filter3LanguageKey")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter3LanguageKey") = value
        End Set
    End Property

    Public Property Filter4LanguageKey() As String
        Get
            If ViewState("Filter4LanguageKey") Is Nothing Then
                Return "ttFilter4"
            Else
                Return ViewState("Filter4LanguageKey")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter4LanguageKey") = value
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
                    Return String.Empty
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
                Return String.Empty
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
                Return String.Empty
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
                Return String.Empty
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

    ''' <summary>
    ''' xxx
    ''' </summary>
    Public Property ShowEmployeeFilters() As Boolean
        Get
            If ViewState("ShowEmployeeFilters") Is Nothing Then
                Return True
            Else
                Return ViewState("ShowEmployeeFilters")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowEmployeeFilters") = value
        End Set
    End Property

    Public Property ShowTreeCaption() As Boolean
        Get
            Return dvCaption.Visible
        End Get
        Set(ByVal value As Boolean)
            dvCaption.Visible = value

            If dvCab1.Attributes("class") Is Nothing Then
                dvCab1.Attributes.Add("Class", String.Empty)
            End If

            If value = True Then
                'quitar RoundCorner_5_Sup
                dvCab1.Attributes("class") = dvCab1.Attributes("class").Replace("RoundCorner_5_Sup", String.Empty).Trim()
            Else
                'poner RoundCorner_5_Sup
                dvCab1.Attributes("class") = dvCab1.Attributes("class") & " RoundCorner_5_Sup"
            End If
        End Set
    End Property

    Public Property TreeCaption() As String
        Get
            Return spanCaption.InnerText
        End Get
        Set(ByVal value As String)
            spanCaption.InnerText = value
        End Set
    End Property

    Public Property TreeIsDouble() As Boolean
        Get
            If ViewState("TreeIsDouble") Is Nothing Then
                Return False
            Else
                Return ViewState("TreeIsDouble")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("TreeIsDouble") = value
        End Set
    End Property

    Public Property TreeDoubleCaption() As String
        Get
            Return spanCaptionDouble.InnerText
        End Get
        Set(ByVal value As String)
            spanCaptionDouble.InnerText = value
        End Set
    End Property

    Public Property TreeDoubleClickFuncion() As String
        Get
            If ViewState("TreeDoubleClickFuncion") Is Nothing Then
                hdnTreeDoubleClickFuncion.Value = String.Empty
                Return String.Empty
            Else
                Return ViewState("TreeDoubleClickFuncion")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("TreeDoubleClickFuncion") = value
            hdnTreeDoubleClickFuncion.Value = value
        End Set
    End Property

    Public Property ShowRefreshTree() As Boolean
        Get
            If ViewState("ShowRefreshTree") Is Nothing Then
                Return True
            Else
                Return ViewState("ShowRefreshTree")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowRefreshTree") = value
        End Set
    End Property

    Public Property ShowAlterAdvancedMode() As Boolean
        Get
            If ViewState("ShowAlterAdvancedMode") Is Nothing Then
                Return False
            Else
                Return ViewState("ShowAlterAdvancedMode")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("ShowAlterAdvancedMode") = value
        End Set
    End Property

#End Region

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            IsScriptManagerInParent()
            PageLoadSelector()
            PageLoadTree()
        End If
    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
        cacheManager.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roTree", "~/Base/Scripts/rocontrols/roTrees/roTree.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roTrees", "~/Base/Scripts/rocontrols/roTrees/roTrees.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js", Me.Parent.Page)

        cacheManager.InsertCssIncludes(Me.Parent.Page)

        Return True
    End Function

    Private Sub PageLoadSelector()

        If Me.ShowEmployeeFilters Then

            Me.dvCab1.Visible = True

            'Boton de filtro 1
            If Me.FiltersVisible.Substring(0, 1) = "1" Then
                Me.icoFilt1.Style.Add("display", String.Empty)
                Me.icoFilt1.Title = Me.Language.Translate(Me.Filter1LanguageKey, "roSelector")
                If Me.AfterSelectFilterFuncion = String.Empty Then
                    Me.icoFilt1.Attributes("onclick") = "UpdTreeFilter24(this,'" & Me.ClientID & "');"
                Else
                    Me.icoFilt1.Attributes("onclick") &= Me.AfterSelectFilterFuncion
                End If
            Else
                Me.icoFilt1.Style.Add("display", "none")
                Me.icoFilt1.Title = String.Empty
                Me.icoFilt1.Attributes("onclick") = String.Empty
            End If

            'Boton de filtro 2
            If Me.FiltersVisible.Substring(1, 1) = "1" Then
                Me.icoFilt2.Style.Add("display", String.Empty)
                Me.icoFilt2.Title = Me.Language.Translate(Me.Filter2LanguageKey, "roSelector")
                If Me.AfterSelectFilterFuncion = String.Empty Then
                    Me.icoFilt2.Attributes("onclick") = "UpdTreeFilter24(this,'" & Me.ClientID & "');"
                Else
                    Me.icoFilt2.Attributes("onclick") &= Me.AfterSelectFilterFuncion
                End If
            Else
                Me.icoFilt2.Style.Add("display", "none")
                Me.icoFilt2.Title = String.Empty
                Me.icoFilt2.Attributes("onclick") = String.Empty
            End If

            'Boton de filtro 3
            If Me.FiltersVisible.Substring(2, 1) = "1" Then
                Me.icoFilt3.Style.Add("display", String.Empty)
                Me.icoFilt3.Title = Me.Language.Translate(Me.Filter3LanguageKey, "roSelector")
                If Me.AfterSelectFilterFuncion = String.Empty Then
                    Me.icoFilt3.Attributes("onclick") = "UpdTreeFilter24(this,'" & Me.ClientID & "');"
                Else
                    Me.icoFilt3.Attributes("onclick") &= Me.AfterSelectFilterFuncion
                End If
            Else
                Me.icoFilt3.Style.Add("display", "none")
                Me.icoFilt3.Title = String.Empty
                Me.icoFilt3.Attributes("onclick") = String.Empty
            End If

            'Boton de filtro 4
            If Me.FiltersVisible.Substring(3, 1) = "1" Then
                Me.icoFilt4.Style.Add("display", String.Empty)
                Me.icoFilt4.Title = Me.Language.Translate(Me.Filter4LanguageKey, "roSelector")
                If Me.AfterSelectFilterFuncion = String.Empty Then
                    Me.icoFilt4.Attributes("onclick") = "UpdTreeFilter24(this,'" & Me.ClientID & "');"
                Else
                    Me.icoFilt4.Attributes("onclick") &= Me.AfterSelectFilterFuncion
                End If
            Else
                Me.icoFilt4.Style.Add("display", "none")
                Me.icoFilt4.Title = String.Empty
                Me.icoFilt4.Attributes("onclick") = String.Empty
            End If

            'Boton de filtro avanzado
            If Me.AdvancedFilterVisible Then
                Me.icoFilt5.Style.Add("display", String.Empty)
                Me.icoFilt5.Title = Me.Language.Translate("ttFilter5", "roSelector")
                If Me.AfterSelectFilterFuncion = String.Empty Then
                    Me.icoFilt5.Attributes("onclick") = "UpdTreeFilter24(this,'" & Me.ClientID & "');"
                Else
                    Me.icoFilt5.Attributes("onclick") &= Me.AfterSelectFilterFuncion
                End If
            Else
                Me.icoFilt5.Style.Add("display", "none")
                Me.icoFilt5.Title = String.Empty
                Me.icoFilt5.Attributes("onclick") = String.Empty
            End If

            Dim oTreeState As roTreeState = HelperWeb.roSelector_GetTreeState(Me.ClientID)

            If Me.AdvancedFilterVisible Then
                If FilterFloat Then
                    mJSFilterShow = "filterFloatVisible('" & Me.ClientID & "');"
                    CreateFilterFloat()
                Else
                    mJSFilterShow = "filterEmbeddedVisible('" & Me.ClientID & "','" & PrefixTree & "');"
                    CreateFilterEmbedd()
                End If

                Me.icoFiltAdv.Style.Add("display", Me.AdvancedFilterDisplay)
                Me.icoFiltAdv.Title = Me.Language.Translate("AdvancedFilter", "roSelector")
                Me.icoFiltAdv.Attributes("onclick") = mJSFilterShow

                'Carrega els filtres actuals (recuperats per cookies)
                RetrieveActualFilters(oTreeState)
            Else
                mJSFilterShow = String.Empty
            End If

            Me.icoFilt2.Title = Me.Language.Translate(Me.Filter2LanguageKey, "roSelector")
            Me.icoFilt3.Title = Me.Language.Translate(Me.Filter3LanguageKey, "roSelector")
            Me.icoFilt4.Title = Me.Language.Translate(Me.Filter4LanguageKey, "roSelector")
            Me.icoFilt5.Title = Me.Language.Translate("ttFilter5", "roSelector")
            Me.icoFiltAdv.Title = Me.Language.Translate("AdvancedFilter", "roSelector")

            If oTreeState.Filter <> String.Empty AndAlso oTreeState.Filter.Length >= 5 Then
                Me.icoFilt1.Attributes("class") = Me.Filter1Class & " roCanHide " & IIf(oTreeState.Filter.Substring(0, 1) = "1", "icoPressed_24", "icoUnPressed_24")
                Me.icoFilt2.Attributes("class") = Me.Filter2Class & " roCanHide " & IIf(oTreeState.Filter.Substring(1, 1) = "1", "icoPressed_24", "icoUnPressed_24")
                Me.icoFilt3.Attributes("class") = Me.Filter3Class & " roCanHide " & IIf(oTreeState.Filter.Substring(2, 1) = "1", "icoPressed_24", "icoUnPressed_24")
                Me.icoFilt4.Attributes("class") = Me.Filter4Class & " roCanHide " & IIf(oTreeState.Filter.Substring(3, 1) = "1", "icoPressed_24", "icoUnPressed_24")
                Me.icoFilt5.Attributes("class") = Me.icoFilt5.Attributes("class").Split(" ")(0) & " roCanHide " & IIf(oTreeState.Filter.Substring(4, 1) = "1", "icoPressed_24", "icoUnPressed_24")
                Me.icoFiltAdv.Attributes("class") = Me.icoFiltAdv.Attributes("class").Split(" ")(0) & " roCanHide " & IIf(oTreeState.UserFieldFilter <> String.Empty, "icoPressed_24", "icoUnPressed_24")
            End If
        Else
            Me.dvCab1.Visible = False
        End If

        If ShowRefreshTree Then
            If Not Me.ShowEmployeeFilters Then
                dvRefreshSimple.Visible = True
            Else
                dvRefreshSimple.Visible = False
            End If
        Else
            dvRefreshSimple.Visible = False
        End If

        Me.btnTreeDouble.Visible = False
        Me.dvTreeDoubleSelector.Visible = False
        If Me.TreeIsDouble AndAlso Me.TreeDoubleCaption <> String.Empty Then
            Me.btnTreeDouble.Visible = True
            Me.spanCaptionDouble.Attributes("onclick") &= Me.TreeDoubleClickFuncion
            Me.dvTreeDoubleSelector.Visible = True
        End If

    End Sub

    Private Sub PageLoadTree()
        Try

            If Me.ShowEmployeeFilters Then

                Dim oTreeState As roTreeState = Nothing

                If Me.Tree2Visible Or Me.Tree3Visible Then
                    oTreeState = HelperWeb.roSelector_GetTreeState(Me.ClientID)
                End If

                'Arbol Agrupacion
                If Me.Tree2Visible Then
                    Me.cmbAgrupacio.Text = String.Empty

                    Dim oUserFieldsPermission As Permission
                    Dim oUserFieldsAccessPermission() As Permission = {Permission.None, Permission.None, Permission.None}
                    oUserFieldsPermission = Me.GetFeaturePermission("Employees.UserFields.Information")
                    oUserFieldsAccessPermission(0) = Me.GetFeaturePermission("Employees.UserFields.Information.Low")
                    oUserFieldsAccessPermission(1) = Me.GetFeaturePermission("Employees.UserFields.Information.Medium")
                    oUserFieldsAccessPermission(2) = Me.GetFeaturePermission("Employees.UserFields.Information.High")

                    If oUserFieldsPermission <> Permission.None Then

                        Dim strSecurityFilter As String = String.Empty

                        If oUserFieldsAccessPermission(0) > Permission.None Then
                            strSecurityFilter = "(AccessLevel = 0) "
                        End If

                        If oUserFieldsAccessPermission(1) > Permission.None Then
                            If strSecurityFilter <> String.Empty Then strSecurityFilter &= " OR "
                            strSecurityFilter &= "(AccessLevel = 1) "
                        End If

                        If oUserFieldsAccessPermission(2) > Permission.None Then
                            If strSecurityFilter <> String.Empty Then strSecurityFilter &= " OR "
                            strSecurityFilter &= "(AccessLevel = 2) "
                        End If

                        Dim dTbl As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, strSecurityFilter, False)
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
                    Me.DesplegableCaja.Visible = False
                Else
                    If Me.Tree1Visible = False Then Me.tabTree01.Style("display") = "none"
                    If Me.Tree2Visible = False Then Me.tabTree02.Style("display") = "none"
                    If Me.Tree3Visible = False Then Me.tabTree03.Style("display") = "none"
                End If

            End If
        Catch
        End Try

    End Sub

#Region "Selector functions"

    ''' <summary>
    ''' Crea filtre en el area del Tree
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateFilterEmbedd()
        'Try
        Dim hTable As HtmlTable = New HtmlTable
        Dim hRow As HtmlTableRow
        Dim hCell As HtmlTableCell

        hTable.CellPadding = 0
        hTable.CellSpacing = 0
        hTable.Border = 0
        hTable.Width = "auto"
        hTable.Attributes("style") = "padding: 2px;"

        hRow = New HtmlTableRow
        hCell = New HtmlTableCell
        hCell.ColSpan = 3
        hCell.VAlign = "top"
        hCell.Attributes("style") = "width: auto;"

        Dim hTable2 As New HtmlTable
        Dim hRow2 As New HtmlTableRow
        Dim hCell2 As New HtmlTableCell

        hTable2.Border = 0
        hTable2.Width = "100%"
        hTable2.Height = "100%"

        'hCell2.Height = "160px"
        Dim hDivFilterContainer As New HtmlGenericControl("div")
        hDivFilterContainer.ID = "dvContainer"
        hDivFilterContainer.Attributes("style") = "border: solid 1px silver; width: auto; height: 200px; overflow: auto;"
        Me.createEmbeddFilterFields(hDivFilterContainer)
        hCell2.Controls.Add(hDivFilterContainer)
        hRow2.Cells.Add(hCell2)
        hTable2.Rows.Add(hRow2)

        hRow2 = New HtmlTableRow
        hCell2 = New HtmlTableCell

        hCell2.Attributes("style") = "padding: 5px; height:10px;"
        hCell2.Align = "right"

        Dim hTable3 As New HtmlTable
        Dim hRow3 As New HtmlTableRow
        Dim hCell3 As New HtmlTableCell

        hTable3.Width = "100%"
        hTable3.Border = 0
        hTable3.Height = "auto"
        hCell3.Align = "left"

        Dim hAnchor As New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = "ClearUserFieldFilter('" & Me.ClientID & "');"
        hAnchor.InnerText = Me.Language.Translate("Button.Clear", "roSelector")
        hCell3.Controls.Add(hAnchor)
        hRow3.Cells.Add(hCell3)

        hCell3 = New HtmlTableCell
        hCell3.Align = "right"
        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = "SaveUserFieldFilter(""" & Me.ClientID & """, """ & Me.PrefixTree & """,""" & mJSFilterShow & """);"
        hAnchor.InnerText = Me.Language.Keyword("Button.Accept")
        hCell3.Controls.Add(hAnchor)

        Dim hSpan As New HtmlGenericControl("span")
        hSpan.InnerText = " | "
        hCell3.Controls.Add(hSpan)

        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = mJSFilterShow
        hAnchor.InnerText = Me.Language.Keyword("Button.Cancel")
        hCell3.Controls.Add(hAnchor)

        hRow3.Cells.Add(hCell3)
        hTable3.Rows.Add(hRow3)

        hCell2.Controls.Add(hTable3)
        hRow2.Cells.Add(hCell2)
        hTable2.Rows.Add(hRow2)

        hCell.Controls.Add(hTable2)
        hRow.Cells.Add(hCell)
        hTable.Rows.Add(hRow)

        Me.divFiltreAvan.Controls.Add(hTable)
        'Catch ex As Exception
        '    Response.Write(ex.Message.ToString)
        'End Try
    End Sub

    ''' <summary>
    ''' Crea els camps del filtre (estructura flotant)
    ''' </summary>
    ''' <param name="hDiv">Div on anira inclos el contingut generat</param>
    ''' <remarks></remarks>
    Private Sub CreateFloatFilterFields(ByRef hDiv As HtmlGenericControl)
        Try
            Dim hTable As New HtmlTable
            Dim hRow As HtmlTableRow
            Dim hCell As HtmlTableCell

            'Carreguem el combo principal (Noms de Camp)
            Dim hSelect As roComboBox

            Dim hText As HtmlInputText

            Dim dTbl As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "", False)
            Dim dRows() As DataRow = dTbl.Select(Nothing, "FieldName")

            'Capcelera
            hRow = New HtmlTableRow
            hCell = New HtmlTableCell
            hCell.InnerText = Me.Language.Translate("Criteria.Field", "roSelector") '"Campo"
            hRow.Cells.Add(hCell)
            hCell = New HtmlTableCell
            hCell.InnerText = Me.Language.Translate("Criteria.Criteria", "roSelector") '"Criterio"
            hRow.Cells.Add(hCell)
            hCell = New HtmlTableCell
            hCell.InnerText = Me.Language.Translate("Criteria.Value", "roSelector") '"Valor"
            hRow.Cells.Add(hCell)
            hTable.Rows.Add(hRow)

            'Creem les files de criteris (5 per omisió)
            For n As Integer = 1 To 5
                hRow = New HtmlTableRow
                hCell = New HtmlTableCell

                hSelect = New roComboBox
                hSelect.AutoResizeChildsWidth = True
                hSelect.DocTypeEnabled = False
                hSelect.ChildsWidth = "200px"
                hSelect.ID = "UserFieldFilter" & n.ToString
                For Each dRow As DataRow In dRows
                    If dRow("Used") = False Then Continue For
                    hSelect.AddItem(dRow("FieldName"), "Usr_" & dRow("FieldName") & "|" & dRow("FieldType"), "")
                Next

                'Afegeix el combo de camps d'usuari
                hCell.Controls.Add(hSelect)
                hRow.Cells.Add(hCell)

                'Select de criteris
                hCell = New HtmlTableCell
                hSelect = New roComboBox
                hSelect.ID = "CriteriaFilter" & n.ToString

                For nCrit As Integer = Criteris.GetLowerBound(0) To Criteris.GetUpperBound(0)
                    hSelect.AddItem(Me.Language.Translate(Criteris(nCrit, 0), "roSelector"), Criteris(nCrit, 1), "")
                Next

                'Afegeix el combo de criteris
                hCell.Controls.Add(hSelect)
                hRow.Cells.Add(hCell)

                'Afegeix el camp de busqueda
                hCell = New HtmlTableCell
                hText = New HtmlInputText
                hText.ID = "ValueFilter" & n.ToString
                hText.Attributes("class") = "textClass"
                hText.Style("width") = "170px"
                hText.Value = String.Empty

                'Afegeix els inputs de busqueda
                hCell.Controls.Add(hText)
                hRow.Cells.Add(hCell)

                hTable.Rows.Add(hRow)

                'Afegim la fila de Y o O (AND OR)
                If n <> 5 Then
                    hRow = New HtmlTableRow
                    hCell = New HtmlTableCell
                    hCell.ColSpan = 3
                    Dim hTableYO As New HtmlTable
                    Dim hRowYO As New HtmlTableRow
                    Dim hCellYO As New HtmlTableCell
                    Dim hOption As New HtmlInputRadioButton
                    hOption.ID = "OptionAND" & n
                    hOption.Name = "AndAndOr" & n
                    hOption.Value = "AND"
                    hOption.Checked = True
                    hOption.Attributes("class") = "inputRadioClass"
                    hCellYO.Controls.Add(hOption)
                    Dim lblAndOr As Label = New Label 'Etiqueta
                    lblAndOr.Text = "Y"
                    lblAndOr.CssClass = "inputRadioClass"
                    hCellYO.Controls.Add(lblAndOr)
                    hRowYO.Cells.Add(hCellYO)

                    hCellYO = New HtmlTableCell
                    hOption = New HtmlInputRadioButton
                    hOption.ID = "OptionOR" & n
                    hOption.Name = "AndAndOr" & n
                    hOption.Value = "OR"
                    hOption.Attributes("class") = "inputRadioClass"
                    hCellYO.Controls.Add(hOption)
                    lblAndOr = New Label ' Etiqueta
                    lblAndOr.Text = "O"
                    lblAndOr.CssClass = "inputRadioClass"
                    hCellYO.Controls.Add(lblAndOr)
                    hRowYO.Cells.Add(hCellYO)

                    hTableYO.Rows.Add(hRowYO)
                    hCell.Controls.Add(hTableYO)
                    hRow.Cells.Add(hCell)
                    hTable.Rows.Add(hRow)
                End If

            Next

            hDiv.Controls.Add(hTable)
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try

    End Sub

    ''' <summary>
    ''' Crea els camps del filtre (estructura embebida)
    ''' </summary>
    ''' <param name="hDiv">DIV que contindra el contingut</param>
    ''' <remarks></remarks>
    Private Sub createEmbeddFilterFields(ByRef hDiv As HtmlGenericControl)
        'Try
        Dim colAlternate As String = "#E0E5EF"

        Dim hTable As New HtmlTable
        hTable.CellPadding = 0
        hTable.CellSpacing = 0
        Dim hRow As HtmlTableRow
        Dim hCell As HtmlTableCell

        'Carreguem el combo principal (Noms de Camp)
        Dim hCombo As Robotics.WebControls.roComboBox
        'Dim hListItem As ListItem

        Dim hText As HtmlInputText

        Dim dTbl As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "", False)
        Dim dRows() As DataRow = dTbl.Select(Nothing, "FieldName")

        'Capcelera
        hRow = New HtmlTableRow
        hCell = New HtmlTableCell
        hCell.Style("padding") = "2px"
        hCell.BgColor = colAlternate
        hCell.InnerText = Me.Language.Translate("Criteria.Field", "roSelector") '"Campo"
        hRow.Cells.Add(hCell)
        hCell = New HtmlTableCell
        hCell.Style("padding") = "2px"
        hCell.BgColor = colAlternate
        hCell.InnerText = Me.Language.Translate("Criteria.Criteria", "roSelector") '"Criterio"
        hRow.Cells.Add(hCell)
        hTable.Rows.Add(hRow)

        'Creem les files de criteris (5 per omisió)
        For n As Integer = 1 To 5
            hRow = New HtmlTableRow
            hCell = New HtmlTableCell
            hCell.BgColor = colAlternate
            hCell.Style("padding") = "2px"

            hCombo = New Robotics.WebControls.roComboBox
            hCombo.ID = "UserFieldFilter" & n.ToString
            hCombo.ParentWidth = "150px"
            hCombo.ChildsVisible = 4
            hCombo.ClearItems()
            hCombo.AddItem("", "")
            For Each dRow As DataRow In dRows
                If dRow("Used") = False Then Continue For
                hCombo.AddItem(dRow("FieldName"), "Usr_" & dRow("FieldName") & "|" & dRow("FieldType"), "")
            Next

            'Afegeix el combo de camps d'usuari
            hCell.Controls.Add(hCombo)
            hRow.Cells.Add(hCell)

            'Select de criteris
            hCell = New HtmlTableCell
            hCell.BgColor = colAlternate
            hCell.Style("padding") = "2px"

            hCombo = New Robotics.WebControls.roComboBox
            hCombo.ID = "CriteriaFilter" & n.ToString
            hCombo.AutoResizeChildsWidth = "true"
            hCombo.ParentWidth = "100px"
            hCombo.ChildsVisible = 6
            hCombo.ClearItems()
            For nCrit As Integer = Criteris.GetLowerBound(0) To Criteris.GetUpperBound(0)
                hCombo.AddItem(Me.Language.Translate(Criteris(nCrit, 0), "roSelector"), Criteris(nCrit, 1), "")
            Next

            'Afegeix el combo de criteris
            hCell.Controls.Add(hCombo)
            hRow.Cells.Add(hCell)

            'Afegeix el camp de busqueda
            hCell = New HtmlTableCell
            hCell.Style("padding") = "2px"
            hCell.BgColor = colAlternate
            hText = New HtmlInputText
            hText.ID = "ValueFilter" & n.ToString
            hText.Attributes("class") = "textClass"
            hText.Style("width") = "170px"
            hText.Value = String.Empty

            hTable.Rows.Add(hRow)
            hRow = New HtmlTableRow

            hCell = New HtmlTableCell
            hCell.Style("padding") = "2px"
            hCell.BgColor = colAlternate
            hCell.InnerText = Me.Language.Translate("Criteria.Value", "roSelector") & " " '"Valor"
            hCell.Width = "40px"
            hRow.Cells.Add(hCell)

            'Afegeix els inputs de busqueda
            hCell.Controls.Add(hText)
            hCell.ColSpan = 2
            hRow.Cells.Add(hCell)

            hTable.Rows.Add(hRow)

            'Afegim la fila de Y o O (AND OR)
            If n <> 5 Then
                hRow = New HtmlTableRow
                hCell = New HtmlTableCell
                hCell.Style("padding") = "2px"
                hCell.BgColor = colAlternate
                hCell.ColSpan = 3
                Dim hTableYO As New HtmlTable
                Dim hRowYO As New HtmlTableRow
                Dim hCellYO As New HtmlTableCell
                Dim hOption As New HtmlInputRadioButton
                hOption.ID = "OptionAND" & n
                hOption.Name = "AndAndOr" & n
                hOption.Value = "AND"
                hOption.Checked = True
                hOption.Attributes("class") = "inputRadioClass"
                hCellYO.Controls.Add(hOption)
                Dim lblAndOr As Label = New Label 'Etiqueta
                lblAndOr.Text = "Y"
                lblAndOr.CssClass = "inputRadioClass"
                hCellYO.Controls.Add(lblAndOr)
                hRowYO.Cells.Add(hCellYO)

                hCellYO = New HtmlTableCell
                hCell.BgColor = colAlternate
                hOption = New HtmlInputRadioButton
                hOption.ID = "OptionOR" & n
                hOption.Name = "AndAndOr" & n
                hOption.Value = "OR"
                hOption.Attributes("class") = "inputRadioClass"
                hCellYO.Controls.Add(hOption)
                lblAndOr = New Label ' Etiqueta
                lblAndOr.Text = "O"
                lblAndOr.CssClass = "inputRadioClass"
                hCellYO.Controls.Add(lblAndOr)
                hRowYO.Cells.Add(hCellYO)

                hTableYO.Rows.Add(hRowYO)
                hCell.Controls.Add(hTableYO)
                hRow.Cells.Add(hCell)
                hTable.Rows.Add(hRow)
            End If
            If colAlternate = "#FFFFFF" Then
                colAlternate = "#E0E5EF"
            Else
                colAlternate = "#FFFFFF"
            End If
        Next

        hDiv.Controls.Add(hTable)

    End Sub

    ''' <summary>
    ''' Crea filtre flotant
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateFilterFloat()
        'Try

        Dim hDivTb As New HtmlGenericControl("div")
        hDivTb.Attributes("class") = "RoundCornerFrame roundCorner bodyPopup"
        hDivTb.Attributes("style") = "width: auto; *width: 470px;"

        'TAULA 2 (Contenidora dels camps)
        Dim hTable2 As New HtmlTable
        Dim hRow2 As New HtmlTableRow
        Dim hCell2 As New HtmlTableCell
        hTable2.Border = 0

        Dim hDiv As New HtmlGenericControl("div")
        CreateFloatFilterFields(hDiv)

        hCell2.Controls.Add(hDiv)
        hRow2.Cells.Add(hCell2) '<TD> amb DIV Controls
        hTable2.Rows.Add(hRow2) '<TR> Div Controls

        hRow2 = New HtmlTableRow
        hCell2 = New HtmlTableCell

        'TAULA 3 (Botonera FOOTER)
        Dim hTable3 As New HtmlTable

        Me.CreateButtonFooter(hTable3)
        hCell2.Controls.Add(hTable3)
        hRow2.Cells.Add(hCell2)
        hTable2.Rows.Add(hRow2)

        hDivTb.Controls.Add(hTable2)

        Me.divFiltreAvan_Float.Controls.Add(hDivTb)

        'Catch ex As Exception
        '    Response.Write(ex.Message.ToString)
        'End Try
    End Sub

    ''' <summary>
    ''' Crea els botons del peu (borrar, Aceptar, Cancelar)
    ''' </summary>
    ''' <param name="hTable3">HtmlTable a omplir</param>
    ''' <remarks></remarks>
    Private Sub CreateButtonFooter(ByRef hTable3 As HtmlTable)
        Dim hRow3 As New HtmlTableRow
        Dim hCell3 As New HtmlTableCell
        Dim hAnchor As New HtmlAnchor

        hTable3.Width = "100%"
        hTable3.Border = 0
        hTable3.BorderColor = "blue"
        hTable3.Height = "10px"

        hCell3.Align = "left"
        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = "ClearUserFieldFilter('" & Me.ClientID & "');"
        hAnchor.InnerText = Me.Language.Translate("Button.Clear", "roSelector")
        hCell3.Controls.Add(hAnchor)
        hRow3.Cells.Add(hCell3)

        hCell3 = New HtmlTableCell
        hCell3.Align = "right"
        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = "SaveUserFieldFilter(""" & Me.ClientID & """,""" & Me.PrefixTree & """,""" & mJSFilterShow & """);"
        hAnchor.InnerText = Me.Language.Keyword("Button.Accept")
        hCell3.Controls.Add(hAnchor)

        Dim hSpan As New HtmlGenericControl("span")
        hSpan.InnerText = " | "
        hCell3.Controls.Add(hSpan)

        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = mJSFilterShow
        hAnchor.InnerText = Me.Language.Keyword("Button.Cancel")
        hCell3.Controls.Add(hAnchor)

        hRow3.Cells.Add(hCell3)
        hTable3.Rows.Add(hRow3)

    End Sub

    ''' <summary>
    ''' Recupera els filtres actuals i els carrega al formulari de criteris
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RetrieveActualFilters(ByVal oTreeState As roTreeState)
        Dim hSelect As roComboBox
        Dim hText As HtmlInputText

        '' Obtenemos los filtros actuales
        Dim strUFFilter As String = oTreeState.UserFieldFilter
        If strUFFilter <> String.Empty Then

            Try
                strUFFilter = HttpUtility.UrlDecode(strUFFilter)
                ' Mostramos filtro actual
                Dim Filters() As String = strUFFilter.Split(Chr(127))
                Dim Parts() As String
                Dim strFieldAux As String = String.Empty

                For i As Integer = 0 To Filters.Length - 1

                    If Filters(i) <> String.Empty Then

                        Parts = Filters(i).Split("~")

                        'Desproteger valor (problemas al codificar/decodificar) le quitamos parentesis
                        strFieldAux = Parts(0).Split("|")(1)
                        If strFieldAux.StartsWith("(") And strFieldAux.EndsWith(")") Then
                            Parts(0) = Parts(0).Split("|")(0) & "|" & strFieldAux.Substring(1, strFieldAux.Length - 2)
                        End If

                        ' Campo
                        hSelect = CType(Me.FindControl("UserFieldFilter" & i + 1), roComboBox)
                        For j As Integer = 0 To hSelect.ItemsValue.Count - 1
                            If hSelect.ItemsValue(j).ToString.ToLower = Parts(0).ToLower Then
                                hSelect.SelectedValue = hSelect.ItemsValue(j)
                                hSelect.Value = hSelect.ItemsValue(j)
                                Exit For
                            End If
                        Next

                        ' Condición
                        hSelect = CType(Me.FindControl("CriteriaFilter" & i + 1), roComboBox)
                        For j As Integer = 0 To hSelect.ItemsValue.Count - 1
                            If hSelect.ItemsValue(j) = Parts(1) Then
                                hSelect.SelectedValue = hSelect.ItemsValue(j)
                                hSelect.Value = hSelect.ItemsValue(j)
                                Exit For
                            End If
                        Next

                        ' Valor
                        hText = Me.FindControl("ValueFilter" & i + 1)
                        'Desproteger valor (problemas al codificar/decodificar) le quitamos parentesis
                        If Parts(2).StartsWith("(") And Parts(2).EndsWith(")") Then
                            hText.Value = Parts(2).Substring(1, Parts(2).Length - 2)
                        Else
                            hText.Value = Parts(2)
                        End If

                        If Parts(3) = "AND" Then
                            CType(Me.FindControl("OptionAND" & i + 1), HtmlInputRadioButton).Checked = True
                        Else
                            CType(Me.FindControl("OptionOR" & i + 1), HtmlInputRadioButton).Checked = True
                        End If

                    End If

                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "Error loading advanced filters", ex)
            End Try
        End If

    End Sub

#End Region

End Class