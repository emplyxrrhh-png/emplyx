Partial Class base_WebUserControls_roOptPanelClientGroup
    Inherits System.Web.UI.UserControl

    Private m_Value As String

    Public Property [Value]() As String
        Get
            If ViewState("Value") IsNot Nothing Then
                Return ViewState("Value")
            Else
                Return m_Value
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Value") = value
            m_Value = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            IsScriptManagerInParent()
        Catch ex As Exception
        End Try
    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
        cacheManager.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js", Me.Parent.Page)

        Return True
    End Function

End Class