Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roTreeEvent
    Inherits UserControlBase

    Public Property ResolvePathEvent() As String
        Get
            If ViewState("ResolvePathEvent") Is Nothing Then
                Dim rePath As String = Me.Page.ResolveUrl("~/Base/WebUserControls/EventSelectorData.aspx")
                If rePath.Length > 0 Then
                    rePath = Mid(rePath, 1, InStrRev(rePath, "/"))
                    Return rePath
                Else
                    Return String.Empty 'si pasa esto, malo!
                End If
            Else
                Return ViewState("ResolvePathEvent")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("ResolvePathEvent") = value
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Me.IsPostBack Then

            IsScriptManagerInParent()

            Me.cmbFieldFind.ClearItems()
            Me.cmbFieldFind.AddItem(Me.Language.Translate("FieldSearch.Year", "roTreeEvent"), "year", "setFieldFindALL('year','','" & Me.ClientID & "');")
            Me.cmbFieldFind.AddItem(Me.Language.Translate("FieldSearch.Event", "roTreeEvent"), "event", "setFieldFindALL('event','','" & Me.ClientID & "');")
            Me.cmbFieldFind.SelectedIndex = 0
        End If

    End Sub

    Private Sub IsScriptManagerInParent()

        If Me.Parent.Page.ClientScript IsNot Nothing Then

            Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
            cacheManager.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js", Me.Parent.Page)
            cacheManager.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js", Me.Parent.Page)
            cacheManager.InsertExtraJavascript("roTreeEvent", "~/Base/Scripts/roTreeEvent.js", Me.Parent.Page)
            cacheManager.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js", Me.Parent.Page)
            cacheManager.InsertExtraJavascript("jquery", "~/Base/jquery/jquery-3.7.1.min.js", Me.Parent.Page)
            cacheManager.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js", Me.Parent.Page)

        End If

    End Sub

End Class