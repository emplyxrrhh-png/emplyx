Imports Robotics.Web.Base

Partial Class srvMsgBoxUserTasks
    Inherits NoCachePageBase

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            Select Case Request("action")
                Case "ResolveOverMaxEmployeesSoon"
                    ResolveOverMaxEmployeesSoon("OverMaxEmployeesSoon")
                Case "ResolveOverMaxJobEmployeesSoon"
                    ResolveOverMaxEmployeesSoon("OverMaxJobEmployeesSoon")
                Case "Message"
                    Me.Message()
                Case "MessageEx"
                    Me.Message2()

            End Select
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try

    End Sub

#End Region

#Region "Methods"

    Private Sub ResolveOverMaxEmployeesSoon(ByVal strLngKey As String)
        Try

            Me.MsgBoxContent.Option1Text = Me.Language.Keyword("Button.Accept")
            Me.MsgBoxContent.Option1Description = ""
            Me.MsgBoxContent.Option1Visible = True
            Me.MsgBoxContent.Option2Visible = False
            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.Title = Me.Language.Translate(strLngKey & ".Title", Me.DefaultScope)

            Dim oParams As New Generic.List(Of String)
            oParams.Add("")
            oParams.Add("0")
            oParams.Add("0")
            If Request("Date") IsNot Nothing Then oParams(0) = Request("Date")
            If Request("ActiveEmployees") IsNot Nothing Then oParams(1) = Request("ActiveEmployees")
            If Request("MaxEmployees") IsNot Nothing Then oParams(2) = Request("MaxEmployees")
            Me.MsgBoxContent.TitleDescription = Me.Language.Translate(strLngKey & ".Message", Me.DefaultScope, oParams)

            Me.MsgBoxContent.AlertVisible = True
            Me.MsgBoxContent.AlertText = Me.Language.Translate(strLngKey & ".Alert", Me.DefaultScope)

            Me.MsgBoxContent.IconUrl = HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.InformationIcon)

            'Me.MsgBoxContent.btnOption1_Click = "window.frames['ifPrincipal'].removeGridAus('" & Request("IDCause") & "','" & Request("ID") & "','" & Request("BeginDate") & "'); HideMsgBoxForm(); return false;"
            Me.MsgBoxContent.btnOption1_Click = "HideMsgBoxForm(); return false;"
        Catch ex As Exception
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
            'For Each cVar As String In Request.Params
            '    Select Case cVar.ToString
            '        Case "TitleKey" : _TitleKey = Request(cVar.ToString)
            '        Case "DescriptionKey" : _DescriptionKey = Request(cVar.ToString)
            '        Case "DescriptionText" : _DescriptionText = Request(cVar.ToString)
            '        Case "Option1TextKey" : _Option1TextKey = Request(cVar.ToString)
            '        Case "Option1DescriptionKey" : _Option1DescriptionKey = Request(cVar.ToString)
            '        Case "Option1OnClickScript" : _Option1OnClickScript = Request(cVar.ToString)
            '        Case "Option2TextKey" : _Option2TextKey = Request(cVar.ToString)
            '        Case "Option2DescriptionKey" : _Option2DescriptionKey = Request(cVar.ToString)
            '        Case "Option2OnClickScript" : _Option2OnClickScript = Request(cVar.ToString)
            '        Case "Option3TextKey" : _Option3TextKey = Request(cVar.ToString)
            '        Case "Option3DescriptionKey" : _Option3DescriptionKey = Request(cVar.ToString)
            '        Case "Option3OnClickScript" : _Option3OnClickScript = Request(cVar.ToString)
            '        Case "AlertKey" : _AlertKey = Request(cVar.ToString)
            '        Case "IconUrl" : _IconUrl = Request(cVar.ToString)
            '    End Select
            'Next

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

    ''' <summary>
    ''' Muestra un mensaje genérico
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Message2()
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

            For Each cVar As String In Request.Params
                Select Case cVar.ToString
                    Case "TitleKey" : _TitleKey = Request(cVar.ToString)
                    Case "DescriptionKey" : _DescriptionKey = Request(cVar.ToString)
                    Case "DescriptionText" : _DescriptionText = Request(cVar.ToString)
                    Case "Option1TextKey" : _Option1TextKey = Request(cVar.ToString)
                    Case "Option1DescriptionKey" : _Option1DescriptionKey = Request(cVar.ToString)
                    Case "Option1OnClickScript" : _Option1OnClickScript = Request(cVar.ToString)
                    Case "Option2TextKey" : _Option2TextKey = Request(cVar.ToString)
                    Case "Option2DescriptionKey" : _Option2DescriptionKey = Request(cVar.ToString)
                    Case "Option2OnClickScript" : _Option2OnClickScript = Request(cVar.ToString)
                    Case "Option3TextKey" : _Option3TextKey = Request(cVar.ToString)
                    Case "Option3DescriptionKey" : _Option3DescriptionKey = Request(cVar.ToString)
                    Case "Option3OnClickScript" : _Option3OnClickScript = Request(cVar.ToString)
                    Case "AlertKey" : _AlertKey = Request(cVar.ToString)
                    Case "IconUrl" : _IconUrl = Request(cVar.ToString)
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

#End Region

End Class