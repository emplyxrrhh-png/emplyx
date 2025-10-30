Imports Robotics.Web.Base

Partial Class Base_Master
    Inherits System.Web.UI.MasterPage

    Public Shared MasterVersion As String = NoCachePageBase.MasterVersion

    Public NoCacheBase As New NoCachePageBase

    'TODO:MessageFrame1
    Public Sub ShowOptionMessage(ByVal Title As String, ByVal Description As String, ByVal Option1text As String, ByVal Option1Key As String, ByVal Option1Description As String, ByVal Option2Text As String, ByVal Option2Key As String, ByVal Option2Description As String, Optional ByVal Option3Text As String = "", Optional ByVal Option3Key As String = "", Optional ByVal Option3Description As String = "")
        Me.MessageFrame1.Title = Title
        Me.MessageFrame1.TitleDescription = Description
        Me.MessageFrame1.Option1Text = Option1text
        Me.MessageFrame1.Option1Key = Option1Key
        Me.MessageFrame1.Option1Description = Option1Description
        Me.MessageFrame1.Option2Text = Option2Text
        Me.MessageFrame1.Option2Key = Option2Key
        Me.MessageFrame1.Option2Description = Option2Description
        Me.MessageFrame1.Option3Text = Option3Text
        Me.MessageFrame1.Option3Key = Option3Key
        Me.MessageFrame1.Option3Description = Option3Description
        Me.MessageFrame1.Show()
    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        NoCacheBase.InsertJavascriptIncludes(Me.Page)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        NoCacheBase.InsertCssIncludes(Me.Page)

        If Not Me.IsPostBack Then
            WLHelperWeb.IniContext()
        End If

        'TODO: MessageFrame1
        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf MessageClick

    End Sub

    Public Sub UpdateContext(Optional ByVal intContextDataType As Integer = 0, Optional ByVal _IDPassport As Integer = -1)
    End Sub

    Public Event OnMessageClick(ByVal btn As String)

    Protected Sub MessageClick(ByVal btn As String)
        RaiseEvent OnMessageClick(btn)
    End Sub

End Class