Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class srvMsgBoxScheduler
    Inherits PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Select Case Request("action")
                Case "DeleteProgrammedAbsence" 'Borra la linia de les ausencies
                    DeleteProgrammedAbsence()
                Case "DeleteAus" 'Borra la linia de les ausencies
                    DeleteAus()
                Case "DeleteEmp"
                    DeleteEmp()
                Case "DeleteGroup"
                    DeleteGroup()
                Case "Message"
                    Message()
                Case "keepAlive"
                    EmptyResponse()
            End Select
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try
    End Sub

    Private Sub EmptyResponse()

        Response.Clear()
        Response.Write("OK")
        Response.Flush()
    End Sub

    ''' <summary>
    ''' Missatge de la linia de Eliminar ausencies
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteProgrammedAbsence()
        Try
            'TODO: Faltara possar els textes, botons, etc.
            Me.MsgBoxContent.Title = Me.Language.Translate("ProgrammedAbsence.Remove.Title", Me.DefaultScope) '"Va a eliminar una ausencia<br />¿Esta seguro?"
            Me.MsgBoxContent.TitleDescription = "" '"Que desea hacer:"

            Me.MsgBoxContent.Option1Text = Me.Language.Translate("ProgrammedAbsence.Delete", Me.DefaultScope) ' "Borrar"
            Me.MsgBoxContent.Option1Visible = True

            Me.MsgBoxContent.Option2Text = Me.Language.Translate("ProgrammedAbsence.NoDelete", Me.DefaultScope) ' "No Borrar"
            Me.MsgBoxContent.Option1Visible = True

            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.IconUrl = "~/Base/Images/MessageFrame/trash64.png"

            Dim Navigator As String = roTypes.Any2String(Request("Navigator"))
            If Request.Browser.MajorVersion <= 11 AndAlso Request.Browser.Browser.ToUpper.Contains("EXPLORER") Then
                Me.MsgBoxContent.btnOption1_Click = "window.frames['3'].DeleteAbsenceFromMoves(); HideMsgBoxForm(); return false;"
            Else ' "Firefox" "Safari" "Chrome"
                Me.MsgBoxContent.btnOption1_Click = "window.frames['1'].DeleteAbsenceFromMoves(); HideMsgBoxForm(); return false;"
            End If

            Me.MsgBoxContent.btnOption2_Click = "HideMsgBoxForm(); return false;"
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Missatge de la linia de Eliminar ausencies
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteAus()
        Try
            'TODO: Faltara possar els textes, botons, etc.
            Me.MsgBoxContent.Title = Me.Language.Translate("ProgrammedAbsence.Remove.Title", Me.DefaultScope) '"Va a eliminar una ausencia<br />¿Esta seguro?"
            Me.MsgBoxContent.TitleDescription = "" '"Que desea hacer:"

            Me.MsgBoxContent.Option1Text = Me.Language.Translate("ProgrammedAbsence.Delete", Me.DefaultScope) ' "Borrar"
            Me.MsgBoxContent.Option1Visible = True

            Me.MsgBoxContent.Option2Text = Me.Language.Translate("ProgrammedAbsence.NoDelete", Me.DefaultScope) ' "No Borrar"
            Me.MsgBoxContent.Option1Visible = True

            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.IconUrl = "~/Base/Images/MessageFrame/trash64.png"

            Me.MsgBoxContent.btnOption1_Click = "window.frames['ifPrincipal'].removeGridAus('" & Request("IDCause") & "','" & Request("ID") & "','" & Request("BeginDate") & "'); HideMsgBoxForm(); return false;"
            Me.MsgBoxContent.btnOption2_Click = "HideMsgBoxForm(); return false;"
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Missatge de la linia de Eliminar Usuaris
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteEmp()
        Try
            'TODO: Faltara possar els textes, botons, etc.
            Me.MsgBoxContent.Option1Text = "Borrar"
            Me.MsgBoxContent.Option1Description = "Borrará el empleado seleccionado"
            Me.MsgBoxContent.Option1Visible = True
            Me.MsgBoxContent.Option2Text = "No Borrar"
            Me.MsgBoxContent.Option2Description = "Dejará sin borrar el empleado"
            Me.MsgBoxContent.Option1Visible = True
            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.Title = "Va a eliminar el empleado<br />¿Esta seguro?"
            Me.MsgBoxContent.TitleDescription = "Que desea hacer:"

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.IconUrl = "~/Base/Images/MessageFrame/trash64.png"

            Me.MsgBoxContent.btnOption1_Click = "window.frames['ifPrincipal'].removeEmployee('" & Request("ID") & "'); HideMsgBoxForm(); return false;"
            Me.MsgBoxContent.btnOption2_Click = "HideMsgBoxForm(); return false;"
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Missatge de la linia de Eliminar Grups
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteGroup()
        Try
            'TODO: Faltara possar els textes, botons, etc.
            Me.MsgBoxContent.Option1Text = "Borrar"
            Me.MsgBoxContent.Option1Description = "Borrará el grupo seleccionado"
            Me.MsgBoxContent.Option1Visible = True
            Me.MsgBoxContent.Option2Text = "No Borrar"
            Me.MsgBoxContent.Option2Description = "Dejará sin borrar el grupo"
            Me.MsgBoxContent.Option1Visible = True
            Me.MsgBoxContent.Option3Visible = False

            Me.MsgBoxContent.Title = "Va a eliminar el grupo<br />¿Esta seguro?"
            Me.MsgBoxContent.TitleDescription = "Que desea hacer:"

            Me.MsgBoxContent.AlertVisible = False

            Me.MsgBoxContent.IconUrl = "~/Base/Images/MessageFrame/trash64.png"

            Me.MsgBoxContent.btnOption1_Click = "window.frames['ifPrincipal'].removeGroup('" & Request("ID") & "'); HideMsgBoxForm(); return false;"
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
            Dim _Option4TextKey As String = ""
            Dim _Option4DescriptionKey As String = ""
            Dim _Option4OnClickScript As String = ""
            Dim _AlertKey As String = ""
            Dim _IconUrl As String = ""

            For Each cVar As String In Request.Params
                Select Case cVar.ToString
                    Case "TitleKey" : _TitleKey = Request(cVar.ToString)
                    Case "DescriptionKey" : _DescriptionKey = Request(cVar.ToString)
                    Case "DescriptionText"
                        _DescriptionText = Request(cVar.ToString).Replace("$br", "<br>")
                        _DescriptionText = _DescriptionText.Replace("$b", "<b>")
                        _DescriptionText = _DescriptionText.Replace("$/b", "</b>")
                    Case "Option1TextKey" : _Option1TextKey = Request(cVar.ToString)
                    Case "Option1DescriptionKey" : _Option1DescriptionKey = Request(cVar.ToString)
                    Case "Option1OnClickScript" : _Option1OnClickScript = Request(cVar.ToString)
                    Case "Option2TextKey" : _Option2TextKey = Request(cVar.ToString)
                    Case "Option2DescriptionKey" : _Option2DescriptionKey = Request(cVar.ToString)
                    Case "Option2OnClickScript" : _Option2OnClickScript = Request(cVar.ToString)
                    Case "Option3TextKey" : _Option3TextKey = Request(cVar.ToString)
                    Case "Option3DescriptionKey" : _Option3DescriptionKey = Request(cVar.ToString)
                    Case "Option3OnClickScript" : _Option3OnClickScript = Request(cVar.ToString)
                    Case "Option4TextKey" : _Option4TextKey = Request(cVar.ToString)
                    Case "Option4DescriptionKey" : _Option4DescriptionKey = Request(cVar.ToString)
                    Case "Option4OnClickScript" : _Option4OnClickScript = Request(cVar.ToString)
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
                If _Option1DescriptionKey <> "" Then
                    .Option1Description = Me.Language.Translate(_Option1DescriptionKey, Me.DefaultScope)
                Else
                    .Option1Description = ""
                End If
                If _Option1OnClickScript <> "" Then _
                    .btnOption1_Click = _Option1OnClickScript
                .Option1Visible = (_Option1TextKey <> "")

                If _Option2TextKey <> "" Then _
                    .Option2Text = Me.Language.Translate(_Option2TextKey, Me.DefaultScope)
                If _Option2DescriptionKey <> "" Then
                    .Option2Description = Me.Language.Translate(_Option2DescriptionKey, Me.DefaultScope)
                Else
                    .Option2Description = ""
                End If
                If _Option2OnClickScript <> "" Then _
                    .btnOption2_Click = _Option2OnClickScript
                .Option2Visible = (_Option2TextKey <> "")

                If _Option3TextKey <> "" Then _
                    .Option3Text = Me.Language.Translate(_Option3TextKey, Me.DefaultScope)
                If _Option3DescriptionKey <> "" Then
                    .Option3Description = Me.Language.Translate(_Option3DescriptionKey, Me.DefaultScope)
                Else
                    .Option3Description = ""
                End If
                If _Option3OnClickScript <> "" Then _
                    .btnOption3_Click = _Option3OnClickScript
                .Option3Visible = (_Option3TextKey <> "")

                If _Option4TextKey <> "" Then _
                   .Option4Text = Me.Language.Translate(_Option4TextKey, Me.DefaultScope)
                If _Option4DescriptionKey <> "" Then
                    .Option4Description = Me.Language.Translate(_Option4DescriptionKey, Me.DefaultScope)
                Else
                    .Option4Description = ""
                End If
                If _Option4OnClickScript <> "" Then _
                    .btnOption4_Click = _Option4OnClickScript
                .Option4Visible = (_Option4TextKey <> "")

                If _AlertKey <> "" Then _
                    .AlertText = Me.Language.Translate(_AlertKey, Me.DefaultScope)
                .AlertVisible = (_AlertKey <> "")

                .IconUrl = Me.Page.ResolveUrl(_IconUrl) ' "~/Base/Images/MessageFrame/trash64.png"

            End With
        Catch ex As Exception
        End Try
    End Sub

End Class