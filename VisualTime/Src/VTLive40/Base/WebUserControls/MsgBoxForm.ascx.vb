Imports Robotics.Web.Base

Partial Class WebUserControls_MsgBoxForm
    Inherits UserControlBase

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.Request.IsAuthenticated AndAlso Me.MsgBoxFormFrame.Attributes("src") = "" Then
            'Me.MsgBoxFormFrame.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
        End If

    End Sub

#End Region

#Region "Methods"

    Public Sub ShowMsgBoxForm(ByVal strUrl As String, ByVal intWidth As Integer, ByVal intHeight As Integer, ByVal strTitle As String, Optional ByVal strParameterName As String = "", Optional ByVal strParameterValue As String = "", Optional ByVal bolCloseButton As Boolean = True, Optional ByVal bolPopupButton As Boolean = False, Optional ByVal bolPopupResizable As Boolean = True)

        'Me.Title_Label.Text = strTitle

        Me.MsgBoxFormFrame.Attributes("width") = intWidth & "px"
        Me.MsgBoxFormFrame.Attributes("height") = intHeight & "px"
        If strParameterName <> "" Then
            strUrl &= "?" & strParameterName & "=" & strParameterValue
        End If
        If strUrl.IndexOf("?") = -1 Then
            strUrl &= "?"
        Else
            strUrl &= "&"
        End If
        strUrl &= "StampParam=" & Now.TimeOfDay.Seconds.ToString
        Me.MsgBoxFormFrame.Attributes("src") = strUrl

        Me.RoPopupFrameMsgBoxForm.Width = (intWidth + 10)
        Me.RoPopupFrameMsgBoxForm.Height = intHeight

        Me.RoPopupFrameMsgBoxForm.Attributes("width") = (intWidth + 10) & "px"
        Me.RoPopupFrameMsgBoxForm.Attributes("height") = intHeight & "px"
        Me.RoPopupFrameMsgBoxForm.Style("width") = (intWidth + 10) & "px"
        Me.RoPopupFrameMsgBoxForm.Style("height") = intHeight & "px"

        Me.Title_Label.Text = strTitle
        Me.ControlBox_Close.Style("display") = IIf(bolCloseButton, "", "none")

        Me.ControlBox_Popup.Style("display") = IIf(bolPopupButton, "", "none")
        If bolPopupButton Then
            strUrl &= IIf(strUrl.IndexOf("?") > 0, "&", "?") & "IsPopup=true"
            Dim strResizable As String = IIf(bolPopupResizable, "true", "false")
            Me.ControlBox_Popup.Attributes.Add("onclick", "OpenPopup('" & strUrl & "'," & intWidth & "," & intHeight & ", " & strResizable & "); HideMsgBoxForm();")
        End If

        Me.RoPopupFrameMsgBoxForm.Show()

        'Me.PanelPopupForm.Attributes("width") = (intWidth + 20) & "px"
        'Me.PanelPopupForm.Attributes("height") = intHeight & "px"

        'Me.ModalPopupForm.Show()
        'showPopup("ModalPopupFormBehavior");

    End Sub

    Public Sub IniMsgBoxForm()

        'Me.MsgBoxFormFrame.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
        'Me.MsgBoxFormFrame.Attributes("src") = ""

    End Sub

#End Region

End Class