Partial Class Base_Popups_roMessageBoxV2
    Inherits System.Web.UI.UserControl

    Public Property IconUrl() As String
        Get
            Return Me.imgTop.Src
        End Get
        Set(ByVal value As String)
            Me.imgTop.Src = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return Me.lblTop.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblTop.InnerHtml = value
        End Set
    End Property

    Public Property TitleDescription() As String
        Get
            Return Me.lblDescription.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblDescription.InnerHtml = value
        End Set
    End Property

    Public Property Option1Visible() As Boolean
        Get
            Return Me.btnOption1.Visible
        End Get
        Set(ByVal value As Boolean)
            Me.btnOption1.Visible = value
        End Set
    End Property

    Public Property Option1Text() As String
        Get
            Return Me.lblOption1Text.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblOption1Text.InnerHtml = value
        End Set
    End Property

    Public Property Option1Description() As String
        Get
            Return Me.lblOption1Description.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblOption1Description.InnerHtml = value
        End Set
    End Property

    Public Property Option2Visible() As Boolean
        Get
            Return Me.btnOption2.Visible
        End Get
        Set(ByVal value As Boolean)
            Me.btnOption2.Visible = value
        End Set
    End Property

    Public Property Option2Text() As String
        Get
            Return Me.lblOption2Text.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblOption2Text.InnerHtml = value
        End Set
    End Property

    Public Property Option2Description() As String
        Get
            Return Me.lblOption2Description.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblOption2Description.InnerHtml = value
        End Set
    End Property

    Public Property Option3Visible() As Boolean
        Get
            Return Me.btnOption3.Visible
        End Get
        Set(ByVal value As Boolean)
            Me.btnOption3.Visible = value
        End Set
    End Property

    Public Property Option3Text() As String
        Get
            Return Me.lblOption3Text.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblOption3Text.InnerHtml = value
        End Set
    End Property

    Public Property Option3Description() As String
        Get
            Return Me.lblOption3Description.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblOption3Description.InnerHtml = value
        End Set
    End Property

    Public Property Option4Visible() As Boolean
        Get
            Return Me.btnOption4.Visible
        End Get
        Set(ByVal value As Boolean)
            Me.btnOption4.Visible = value
        End Set
    End Property

    Public Property Option4Text() As String
        Get
            Return Me.lblOption4Text.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblOption4Text.InnerHtml = value
        End Set
    End Property

    Public Property Option4Description() As String
        Get
            Return Me.lblOption4Description.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblOption4Description.InnerHtml = value
        End Set
    End Property

    Public Property AlertVisible() As Boolean
        Get
            Return (Me.divAviso.Style("display") <> "none")
        End Get
        Set(ByVal value As Boolean)
            Me.divAviso.Style("display") = IIf(value, "", "none")
        End Set
    End Property

    Public Property AlertText() As String
        Get
            Return Me.lblAviso.InnerHtml
        End Get
        Set(ByVal value As String)
            Me.lblAviso.InnerHtml = value
        End Set
    End Property

    Public Property btnOption1_Click() As String
        Get
            Return Me.btnOption1.Attributes("onclick")
        End Get
        Set(ByVal value As String)
            Me.btnOption1.Attributes("onclick") = value
        End Set
    End Property

    Public Property btnOption2_Click() As String
        Get
            Return Me.btnOption2.Attributes("onclick")
        End Get
        Set(ByVal value As String)
            Me.btnOption2.Attributes("onclick") = value
        End Set
    End Property

    Public Property btnOption3_Click() As String
        Get
            Return Me.btnOption3.Attributes("onclick")
        End Get
        Set(ByVal value As String)
            Me.btnOption3.Attributes("onclick") = value
        End Set
    End Property

    Public Property btnOption4_Click() As String
        Get
            Return Me.btnOption4.Attributes("onclick")
        End Get
        Set(ByVal value As String)
            Me.btnOption4.Attributes("onclick") = value
        End Set
    End Property

    Public Property PopupID() As String
        Get
            Return Me.ObjectPopup.ClientInstanceName
        End Get
        Set(ByVal value As String)
            Me.ObjectPopup.ClientInstanceName = value
        End Set
    End Property

    Public Property PopupWidth() As Unit
        Get
            Return Me.ObjectPopup.Width
        End Get
        Set(ByVal value As Unit)
            Me.ObjectPopup.Width = value
        End Set
    End Property

    Public Property PopupHeight() As Unit
        Get
            Return Me.ObjectPopup.Height
        End Get
        Set(ByVal value As Unit)
            Me.ObjectPopup.Height = value
        End Set
    End Property

#Region "Events"

    Public Event Option1Click(ByVal sender As Object)

    Public Event Option2Click(ByVal sender As Object)

    Public Event Option3Click(ByVal sender As Object)

    Public Event Option4Click(ByVal sender As Object)

    Protected Sub btnOption_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOption1.ServerClick, btnOption2.ServerClick, btnOption3.ServerClick, btnOption4.ServerClick
        If sender Is btnOption1 Then
            RaiseEvent Option1Click(sender)
        ElseIf sender Is btnOption2 Then
            RaiseEvent Option2Click(sender)
        ElseIf sender Is btnOption3 Then
            RaiseEvent Option3Click(sender)
        ElseIf sender Is btnOption4 Then
            RaiseEvent Option4Click(sender)
        End If
    End Sub

#End Region

End Class