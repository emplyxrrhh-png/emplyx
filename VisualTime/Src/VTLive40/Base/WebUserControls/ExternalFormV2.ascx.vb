Imports Robotics.Web.Base

Partial Class WebUserControls_ExternalFormV2
    Inherits UserControlBase

#Region "Properties"

    Public ReadOnly Property PopupFrameExternalForm() As Robotics.WebControls.roPopupFrameV2
        Get
            Return Me.RoPopupFrameExternalForm
        End Get
    End Property

    Public Property DragEnabled() As Boolean
        Get
            If ViewState("DragEnabled") Is Nothing Then
                Return True
            Else
                Return ViewState("DragEnabled")
            End If

        End Get
        Set(ByVal value As Boolean)
            ViewState("DragEnabled") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.Request.IsAuthenticated AndAlso Me.ExternalFormFrame.Attributes("src") = "" Then
            Me.ExternalFormFrame.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
        End If

        Me.RoPopupFrameExternalForm.DragEnabled = Me.DragEnabled

    End Sub

#End Region

#Region "Methods"

    Public Sub ShowExternalForm(ByVal strUrl As String, ByVal intWidth As Integer, ByVal intHeight As Integer, ByVal strTitle As String, Optional ByVal strParameterName As String = "", Optional ByVal strParameterValue As String = "", Optional ByVal bolCloseButton As Boolean = True, Optional ByVal bolPopupButton As Boolean = False, Optional ByVal bolPopupResizable As Boolean = True)

        If strParameterName <> "" Then
            strUrl &= "?" & strParameterName & "=" & strParameterValue
        End If
        If strUrl.IndexOf("?") = -1 Then
            strUrl &= "?"
        Else
            strUrl &= "&"
        End If
        strUrl &= "StampParam=" & Now.TimeOfDay.Seconds.ToString
        Me.ExternalFormFrame.Attributes("src") = strUrl

        If strTitle IsNot String.Empty Then
            Me.lblTitleText.Text = strTitle
        End If

        Me.ControlBox_Close.Style("display") = IIf(bolCloseButton, "", "none")

        Me.ControlBox_Popup.Src = Me.Page.ResolveUrl("~/Base/Images/btnNewWindow.png")
        Me.ControlBox_Popup.Style("display") = IIf(bolPopupButton, "", "none")
        If bolPopupButton Then
            strUrl &= IIf(strUrl.IndexOf("?") > 0, "&", "?") & "IsPopup=true"
            Dim strResizable As String = IIf(bolPopupResizable, "true", "false")
            Me.ControlBox_Popup.Attributes.Add("onclick", "OpenPopup('" & strUrl & "'," & intWidth & "," & intHeight & ", " & strResizable & "); HideExternalForm();")
        End If

        Me.RoPopupFrameExternalForm.Show()

    End Sub

    Public Sub IniExternalForm()

        'Me.ExternalFormFrame.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
        Me.ExternalFormFrame.Attributes("src") = ""

    End Sub

#End Region

End Class