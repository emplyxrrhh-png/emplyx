Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roTreeV3
    Inherits UserControlBase

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim script As ClientScriptManager = Me.Parent.Page.ClientScript

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
        cacheManager.InsertExtraJavascript("jquery", "~/Base/jquery/jquery-3.7.1.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js", Me.Parent.Page)

        Me.objTreePrev.PrefixTree = Me.PrefixTree

        Me.objTreePrev.TreesBehaviorID = Me.PrefixTree

        Me.objTreePrev.AfterSelectFilterFuncion = Me.ClientID & "_GetFilters();"

        Me.objTreePrev.FeatureAlias = Me.FeatureAlias

        Me.objTreePrev.ID = Me.PrefixTree

        Me.objTreePrev.FilterFixed = Me.FilterFixed

        Me.objTreePrev.OnlyGroups = Me.OnlyGroups

        Me.objTreePrev.EnableCustomFilters = Me.EnableCustomFilters

    End Sub

    Protected ReadOnly Property CookieNameTreeV25() As String
        Get
            'Return "objContainerTreeV3_roChildSelector_" & Me.PrefixTree
            If Embedded Then
                Return Me.PrefixCookie
            Else
                Return "objContainerTreeV3_" & Me.PrefixTree
            End If

        End Get
    End Property

    Public Property Embedded() As Boolean
        Get
            If ViewState("Embedded") Is Nothing Then
                Return False
            Else
                Return ViewState("Embedded")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Embedded") = value
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

    Public Property PrefixTree() As String
        Get
            If ViewState("PrefixTree") Is Nothing Then
                Return ""
            Else
                Return ViewState("PrefixTree")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("PrefixTree") = value
        End Set
    End Property

    Public Property PrefixCookie() As String
        Get
            If ViewState("PrefixCookie") Is Nothing Then
                Return ""
            Else
                Return ViewState("PrefixCookie")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("PrefixCookie") = value
        End Set
    End Property

    Public Property AfterSelectFuncion() As String
        Get
            If ViewState("AfterSelectFuncion") Is Nothing Then
                Return ""
            Else
                Return ViewState("AfterSelectFuncion")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("AfterSelectFuncion") = value
        End Set
    End Property

    Public Property FilterFixed() As String
        Get
            If ViewState("FilterFixed") Is Nothing Then
                Return ""
            Else
                Return ViewState("FilterFixed")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("FilterFixed") = value
        End Set
    End Property

    Public Property OnlyGroups() As Boolean
        Get
            If ViewState("OnlyGroups") Is Nothing Then
                Return False
            Else
                Return ViewState("OnlyGroups")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("OnlyGroups") = value
        End Set
    End Property

    Public Property EnableCustomFilters() As Boolean
        Get
            If ViewState("EnableCustomFilters") Is Nothing Then
                Return False
            Else
                Return ViewState("EnableCustomFilters")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("EnableCustomFilters") = value
        End Set
    End Property

End Class