Imports Robotics.Web.Base

Partial Class Base_Master2
    Inherits System.Web.UI.MasterPage

    Public Shared MasterVersion As String = NoCachePageBase.MasterVersion
    Public NoCacheBase As New NoCachePageBase

    'TODO:MessageFrame1
    Public Sub ShowOptionMessage(ByVal Title As String, ByVal Description As String, ByVal Option1text As String, ByVal Option1Key As String, ByVal Option1Description As String, ByVal Option2Text As String, ByVal Option2Key As String, ByVal Option2Description As String, Optional ByVal Option3Text As String = "", Optional ByVal Option3Key As String = "", Optional ByVal Option3Description As String = "")
    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Me.IsPostBack Then
            NoCacheBase.InsertJavascriptIncludes(Me.Page)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            NoCacheBase.InsertCssIncludes(Me.Page)
        End If
    End Sub

    Public Sub UpdateContext(Optional ByVal intContextDataType As Integer = 0, Optional ByVal _IDPassport As Integer = -1)
    End Sub

    Public Event OnMessageClick(ByVal btn As String)

    Protected Sub MessageClick(ByVal btn As String)
        RaiseEvent OnMessageClick(btn)
    End Sub

End Class