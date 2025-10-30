Imports Robotics.Web.Base

Partial Class srvMsgBoxTerminals
    Inherits PageBase

#Region "Events"

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            Select Case Request("action")

                Case "Message"
                    Message()

                Case "LaunchBroadcaster"
                    MessageLaunchBroadcaster()
                Case "LaunchBroadcasterForTerminal"
                    MessageLaunchBroadcasterForTerminal()

                Case "EndLaunchBroadcaster"
                    MessageEndLaunchBroadcaster()

            End Select
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try

    End Sub

#End Region

#Region "Methods"

    Private Sub MessageEndLaunchBroadcaster()
        Try

            If Request("exec") = "OK" Then
                Me.MsgBoxContent.Option1Text = Me.Language.Translate("LaunchBroadcaster.OKProcess", Me.DefaultScope)
                Me.MsgBoxContent.IconUrl = Me.Page.ResolveUrl("~/Base/Images/MessageFrame/dialog-information.png")
            Else
                Me.MsgBoxContent.Option1Text = Me.Language.Translate("LaunchBroadcaster.NOKProcess", Me.DefaultScope)
                Me.MsgBoxContent.IconUrl = Me.Page.ResolveUrl("~/Base/Images/MessageFrame/dialog-warning.png")

            End If

            Me.MsgBoxContent.Title = Me.Language.Translate("LaunchBroadcaster.Title.End", Me.DefaultScope)
            Me.MsgBoxContent.TitleDescription = ""

            Me.MsgBoxContent.Option1Description = ""
            Me.MsgBoxContent.Option1Visible = True

            Me.MsgBoxContent.Option2Visible = False
            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.btnOption1_Click = "HideMsgBoxForm(); return false;"
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MessageLaunchBroadcaster()
        Try

            Me.MsgBoxContent.Option1Text = Me.Language.Translate("Launch", Me.DefaultScope) ' "Lanzar
            Me.MsgBoxContent.Option1Description = ""
            Me.MsgBoxContent.Option1Visible = True

            Me.MsgBoxContent.Option2Text = Me.Language.Translate("NoLaunch", Me.DefaultScope) ' "No Lanzar"
            Me.MsgBoxContent.Option2Description = ""
            Me.MsgBoxContent.Option2Visible = True

            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.Title = Me.Language.Translate("LaunchBroadcaster.Title", Me.DefaultScope)
            Me.MsgBoxContent.TitleDescription = Me.Language.Translate("LaunchBroadcaster.TitleDescription", Me.DefaultScope)

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.IconUrl = Me.Page.ResolveUrl("~/Base/Images/MessageFrame/appointment-missed.png")

            Me.MsgBoxContent.btnOption1_Click = "window.frames['ifPrincipal'].LaunchBroadcaster(); HideMsgBoxForm(); return false;"
            Me.MsgBoxContent.btnOption2_Click = "HideMsgBoxForm(); return false;"
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MessageLaunchBroadcasterForTerminal()
        Try

            Me.MsgBoxContent.Option1Text = Me.Language.Translate("Launch", Me.DefaultScope) ' "Lanzar
            Me.MsgBoxContent.Option1Description = ""
            Me.MsgBoxContent.Option1Visible = True

            Me.MsgBoxContent.Option2Text = Me.Language.Translate("NoLaunch", Me.DefaultScope) ' "No Lanzar"
            Me.MsgBoxContent.Option2Description = ""
            Me.MsgBoxContent.Option2Visible = True

            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.Title = Me.Language.Translate("LaunchBroadcaster.Title", Me.DefaultScope)
            Me.MsgBoxContent.TitleDescription = Me.Language.Translate("LaunchBroadcaster.TitleDescription", Me.DefaultScope)

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.IconUrl = Me.Page.ResolveUrl("~/Base/Images/MessageFrame/appointment-missed.png")

            Me.MsgBoxContent.btnOption1_Click = "window.frames['ifPrincipal'].LaunchBroadcasterForTerminal(); HideMsgBoxForm(); return false;"
            Me.MsgBoxContent.btnOption2_Click = "HideMsgBoxForm(); return false;"
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

            For Each cVar As String In Request.Params
                Select Case "" & cVar.ToString
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