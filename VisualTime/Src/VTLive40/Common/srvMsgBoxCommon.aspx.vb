Imports Robotics.Web.Base

Partial Class srvMsgBoxCommon
    Inherits PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Select Case Request("action")
                Case "Message"
                    Me.Message()
            End Select
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Muestra un mensaje genérico
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Message()
        Try

            Dim _TitleKey As String = ""
            Dim _DescriptionKey As String = ""
            Dim _DescriptionText As String = ""
            Dim _Option1TextKey As String = ""
            Dim _Option1DescriptionKey As String = ""
            Dim _Option1OnClickScript As String = ""
            Dim _Option2TextKey As String = ""
            Dim _Option2DescriptionKey As String = ""
            Dim _Option2OnClickScript As String = ""
            Dim _Option3TextKey As String = ""
            Dim _Option3DescriptionKey As String = ""
            Dim _Option3OnClickScript As String = ""
            Dim _AlertKey As String = ""
            Dim _IconUrl As String = ""

            Dim strParameters As String = Me.Request("Parameters")
            Dim strKey As String
            Dim strValue As String
            For Each cVar As String In strParameters.Split("&")
                strKey = cVar.Split("=")(0)
                strValue = cVar.Split("=")(1)
                Select Case strKey
                    Case "TitleKey" : _TitleKey = strValue
                    Case "DescriptionKey" : _DescriptionKey = strValue
                    Case "DescriptionText" : _DescriptionText = strValue
                    Case "Option1TextKey" : _Option1TextKey = strValue
                    Case "Option1DescriptionKey" : _Option1DescriptionKey = strValue
                    Case "Option1OnClickScript" : _Option1OnClickScript = strValue
                    Case "Option2TextKey" : _Option2TextKey = strValue
                    Case "Option2DescriptionKey" : _Option2DescriptionKey = strValue
                    Case "Option2OnClickScript" : _Option2OnClickScript = strValue
                    Case "Option3TextKey" : _Option3TextKey = strValue
                    Case "Option3DescriptionKey" : _Option3DescriptionKey = strValue
                    Case "Option3OnClickScript" : _Option3OnClickScript = strValue
                    Case "AlertKey" : _AlertKey = strValue
                    Case "IconUrl" : _IconUrl = strValue
                End Select
            Next
            With Me.MsgBoxContent

                If _TitleKey <> "" Then _
                    .Title = Me.Language.Translate(_TitleKey, Me.DefaultScope)
                If _DescriptionKey <> "" Then
                    .TitleDescription = Me.Language.Translate(_DescriptionKey, Me.DefaultScope)
                Else
                    .TitleDescription = _DescriptionText
                End If

                If _Option1TextKey <> "" Then _
                    .Option1Text = Me.Language.Translate(_Option1TextKey, Me.DefaultScope)
                If _Option1DescriptionKey <> "" Then _
                    .Option1Description = Me.Language.Translate(_Option1DescriptionKey, Me.DefaultScope)
                If _Option1OnClickScript <> "" Then _
                    .btnOption1_Click = _Option1OnClickScript
                .Option1Visible = (_Option1TextKey <> "")

                If _Option2TextKey <> "" Then _
                    .Option2Text = Me.Language.Translate(_Option2TextKey, Me.DefaultScope)
                If _Option2DescriptionKey <> "" Then _
                .Option2Description = Me.Language.Translate(_Option2DescriptionKey, Me.DefaultScope)
                If _Option2OnClickScript <> "" Then _
                    .btnOption2_Click = _Option2OnClickScript
                .Option2Visible = (_Option2TextKey <> "")

                If _Option3TextKey <> "" Then _
                    .Option3Text = Me.Language.Translate(_Option3TextKey, Me.DefaultScope)
                If _Option3DescriptionKey <> "" Then _
                .Option3Description = Me.Language.Translate(_Option3DescriptionKey, Me.DefaultScope)
                If _Option3OnClickScript <> "" Then _
                    .btnOption3_Click = _Option3OnClickScript
                .Option3Visible = (_Option3TextKey <> "")

                If _AlertKey <> "" Then _
                    .AlertText = Me.Language.Translate(_AlertKey, Me.DefaultScope)
                .AlertVisible = (_AlertKey <> "")

                .IconUrl = Me.Page.ResolveUrl(_IconUrl) ' "~/Base/Images/MessageFrame/trash64.png"

            End With
        Catch ex As Exception
        End Try
    End Sub

End Class