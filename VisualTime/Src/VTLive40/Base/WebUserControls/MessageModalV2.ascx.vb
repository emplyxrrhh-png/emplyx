Partial Class Base_WebUserControls_MessageModalV2
    Inherits System.Web.UI.UserControl
    'Inherits UserControlBase

    Public Event OptionOnClick(ByVal btn As String)

    Dim strTitle As String
    Dim strTitleDescription As String

    Dim strOption1Text As String
    Dim strOption1Key As String
    Dim strOption1Description As String

    Dim strOption2Text As String
    Dim strOption2Key As String
    Dim strOption2Description As String

    Dim strOption3Text As String
    Dim strOption3Key As String
    Dim strOption3Description As String

    Dim strOption4Text As String
    Dim strOption4Key As String
    Dim strOption4Description As String

    Dim strAlertText As String

#Region "Properties"

    Public Property Width() As Integer
        Get
            If ViewState("Width") Is Nothing Then
                Return 100
            Else
                Return ViewState("Width")
            End If

        End Get
        Set(ByVal value As Integer)
            ViewState("Width") = value
        End Set
    End Property

    Public Property IconUrl() As String
        Get
            Return Me.MsgBoxContent.IconUrl
        End Get
        Set(ByVal value As String)
            Me.MsgBoxContent.IconUrl = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return strTitle
        End Get
        Set(ByVal value As String)
            strTitle = value
        End Set
    End Property

    Public Property TitleDescription() As String
        Get
            Return strTitleDescription
        End Get
        Set(ByVal value As String)
            strTitleDescription = value
        End Set
    End Property

    Public Property AcceptButtonKey() As String
        Get
            Dim strKey As String
            If Me.hdnAcceptButtonKey.Value = "" Then
                strKey = "MessageFrame.Accept.Button"
            Else
                strKey = Me.hdnAcceptButtonKey.Value
            End If
            Return strKey
        End Get
        Set(ByVal value As String)
            Me.hdnAcceptButtonKey.Value = value
        End Set
    End Property

    Public Property Option1Text() As String
        Get
            Return strOption1Text
        End Get
        Set(ByVal value As String)
            strOption1Text = value
        End Set
    End Property

    Public Property Option1Key() As String
        Get
            Return hdnOption1Key.Value
        End Get
        Set(ByVal value As String)
            strOption1Key = value
            hdnOption1Key.Value = strOption1Key
        End Set
    End Property

    Public Property Option1Description() As String
        Get
            Return strOption1Description
        End Get
        Set(ByVal value As String)
            strOption1Description = value
        End Set
    End Property

    Public Property Option1Click() As String
        Get
            Return Me.MsgBoxContent.btnOption1_Click
        End Get
        Set(ByVal value As String)
            Me.MsgBoxContent.btnOption1_Click = value
        End Set
    End Property

    Public Property Option1CloseClient() As Boolean
        Get
            Return (Me.hdnOption1CloseClient.Value = "1")
        End Get
        Set(ByVal value As Boolean)
            Me.hdnOption1CloseClient.Value = IIf(value, "1", "0")
        End Set
    End Property

    Public Property Option1EventServer() As Boolean
        Get
            Return (Me.hdnOption1EventServer.Value = "1")
        End Get
        Set(ByVal value As Boolean)
            Me.hdnOption1EventServer.Value = IIf(value, "1", "0")
        End Set
    End Property

    Public Property Option2Text() As String
        Get
            Return strOption2Text
        End Get
        Set(ByVal value As String)
            strOption2Text = value
        End Set
    End Property

    Public Property Option2Key() As String
        Get
            Return hdnOption2Key.Value
        End Get
        Set(ByVal value As String)
            strOption2Key = value
            hdnOption2Key.Value = strOption2Key
        End Set
    End Property

    Public Property Option2Description() As String
        Get
            Return strOption2Description
        End Get
        Set(ByVal value As String)
            strOption2Description = value
        End Set
    End Property

    Public Property Option2Click() As String
        Get
            Return Me.MsgBoxContent.btnOption2_Click
        End Get
        Set(ByVal value As String)
            Me.MsgBoxContent.btnOption2_Click = value
        End Set
    End Property

    Public Property Option2CloseClient() As Boolean
        Get
            Return (Me.hdnOption2CloseClient.Value = "1")
        End Get
        Set(ByVal value As Boolean)
            Me.hdnOption2CloseClient.Value = IIf(value, "1", "0")
        End Set
    End Property

    Public Property Option2EventServer() As Boolean
        Get
            Return (Me.hdnOption2EventServer.Value = "1")
        End Get
        Set(ByVal value As Boolean)
            Me.hdnOption2EventServer.Value = IIf(value, "1", "0")
        End Set
    End Property

    Public Property Option3Text() As String
        Get
            Return strOption3Text
        End Get
        Set(ByVal value As String)
            strOption3Text = value
        End Set
    End Property

    Public Property Option3Key() As String
        Get
            Return hdnOption3Key.Value
        End Get
        Set(ByVal value As String)
            strOption3Key = value
            hdnOption3Key.Value = strOption3Key
        End Set
    End Property

    Public Property Option3Description() As String
        Get
            Return strOption3Description
        End Get
        Set(ByVal value As String)
            strOption3Description = value
        End Set
    End Property

    Public Property Option3Click() As String
        Get
            Return Me.MsgBoxContent.btnOption3_Click
        End Get
        Set(ByVal value As String)
            Me.MsgBoxContent.btnOption3_Click = value
        End Set
    End Property

    Public Property Option3CloseClient() As Boolean
        Get
            Return (Me.hdnOption3CloseClient.Value = "1")
        End Get
        Set(ByVal value As Boolean)
            Me.hdnOption3CloseClient.Value = IIf(value, "1", "0")
        End Set
    End Property

    Public Property Option3EventServer() As Boolean
        Get
            Return (Me.hdnOption3EventServer.Value = "1")
        End Get
        Set(ByVal value As Boolean)
            Me.hdnOption3EventServer.Value = IIf(value, "1", "0")
        End Set
    End Property

    Public Property Option4Text() As String
        Get
            Return strOption4Text
        End Get
        Set(ByVal value As String)
            strOption4Text = value
        End Set
    End Property

    Public Property Option4Key() As String
        Get
            Return hdnOption4Key.Value
        End Get
        Set(ByVal value As String)
            strOption4Key = value
            hdnOption4Key.Value = strOption4Key
        End Set
    End Property

    Public Property Option4Description() As String
        Get
            Return strOption4Description
        End Get
        Set(ByVal value As String)
            strOption4Description = value
        End Set
    End Property

    Public Property Option4Click() As String
        Get
            Return Me.MsgBoxContent.btnOption4_Click
        End Get
        Set(ByVal value As String)
            Me.MsgBoxContent.btnOption4_Click = value
        End Set
    End Property

    Public Property Option4CloseClient() As Boolean
        Get
            Return (Me.hdnOption4CloseClient.Value = "1")
        End Get
        Set(ByVal value As Boolean)
            Me.hdnOption4CloseClient.Value = IIf(value, "1", "0")
        End Set
    End Property

    Public Property Option4EventServer() As Boolean
        Get
            Return (Me.hdnOption4EventServer.Value = "1")
        End Get
        Set(ByVal value As Boolean)
            Me.hdnOption4EventServer.Value = IIf(value, "1", "0")
        End Set
    End Property

    Public Property AlertText() As String
        Get
            Return strAlertText
        End Get
        Set(ByVal value As String)
            Me.strAlertText = value
        End Set
    End Property

    'Public ReadOnly Property PopupFrame() As AjaxControlToolkit.ModalPopupExtender
    '    Get
    '        Return Me.MyModalPopupExtender
    '    End Get
    'End Property

    'Public ReadOnly Property UpdatePanel() As UpdatePanel
    '    Get
    '        Return Me.updMessageFrame
    '    End Get
    'End Property

    'Public ReadOnly Property AcceptText() As String
    '    Get
    '        If Me.Language IsNot Nothing Then
    '            Return Me.Language.Keyword("Button.Accept")
    '        Else
    '            Return ""
    '        End If
    '    End Get
    'End Property

    Public ReadOnly Property AcceptDescription() As String
        Get
            Return ""
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub MsgBoxContent_Option1Click(ByVal sender As Object) Handles MsgBoxContent.Option1Click
        RaiseEvent OptionOnClick(Option1Key)
    End Sub

    Protected Sub MsgBoxContent_Option2Click(ByVal sender As Object) Handles MsgBoxContent.Option2Click
        RaiseEvent OptionOnClick(Option2Key)
    End Sub

    Protected Sub MsgBoxContent_Option3Click(ByVal sender As Object) Handles MsgBoxContent.Option3Click
        RaiseEvent OptionOnClick(Option3Key)
    End Sub

    Protected Sub MsgBoxContent_Option4Click(ByVal sender As Object) Handles MsgBoxContent.Option4Click
        RaiseEvent OptionOnClick(Option4Key)
    End Sub

#End Region

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        Me.MyPopupFrame_DIV.Style("width") = Me.Width & "px"

        Me.MsgBoxContent.Title = strTitle
        Me.MsgBoxContent.TitleDescription = strTitleDescription

        Me.MsgBoxContent.Option1Text = strOption1Text
        Me.MsgBoxContent.Option1Description = strOption1Description
        Me.MsgBoxContent.Option1Visible = (strOption1Text <> "")

        Me.MsgBoxContent.Option2Text = strOption2Text
        Me.MsgBoxContent.Option2Description = strOption2Description
        Me.MsgBoxContent.Option2Visible = (strOption2Text <> "")

        Me.MsgBoxContent.Option3Text = strOption3Text
        Me.MsgBoxContent.Option3Description = strOption3Description
        Me.MsgBoxContent.Option3Visible = (strOption3Text <> "")

        Me.MsgBoxContent.Option4Text = strOption4Text
        Me.MsgBoxContent.Option4Description = strOption4Description
        Me.MsgBoxContent.Option4Visible = (strOption4Text <> "")

        Me.MsgBoxContent.AlertText = strAlertText
        Me.MsgBoxContent.AlertVisible = (strAlertText <> "")

    End Sub

    Public Sub Show()
        Me.MyModalPopupExtender.Show()
    End Sub

    Public Sub Hide()
        Me.MyModalPopupExtender.Hide()
    End Sub

    Public Sub Update()
        Me.updMessageFrame.Update()
    End Sub

End Class