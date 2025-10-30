<%@ WebHandler Language="VB" Class="srvMoves" %>

Imports System
Imports System.IO
Imports System.Data
Imports System.Drawing
Imports Robotics.Web.Base
Imports Robotics.VTBase
Imports Robotics.Base.VTBusiness.Absence

Public Class srvMoves
    Inherits handlerBase



    Public Overrides Sub SetActionsNotRequieredUpdateSession()
        FeaturesNotRequiereUpdateSession = {"getDailyScheduleStatus"}
    End Sub

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.scope = "Scheduler"

        Select Case Request("action")
            Case "getDailyScheduleStatus" ' Retorna el calendari principal
                GetDailyScheduleStatus()
            Case "deleteProgAus" 'Eliminar una Ausencia prevista
                DeleteProgAus()
            Case "getCapture"
                Me.GetCapture()
        End Select
    End Sub

    Private Sub GetDailyScheduleStatus()

        Dim strStatus As String = ""

        Dim intIDEmployee As Integer = Request("IDEmployee")
        Dim strDateMove As String = Request("DateMove")
        Dim xDateMove As Date = CDate(strDateMove)
        Dim intView As Integer = Request("View")
        'Dim xDateMove As New Date(strDateMove.Substring(6, 4), strDateMove.Substring(3, 2), strDateMove.Substring(0, 2))

        Try
            'Aquí se pueden dar excepciones ya que se esta función se va llamando periodicamente y puede caducar la sesión de por medio
            Dim tb As DataTable = API.EmployeeServiceMethods.GetScheduleStatus(Nothing, intIDEmployee, xDateMove)
            If Not tb Is Nothing Then
                If tb.Rows.Count > 0 Then
                    strStatus = tb.Rows(0).Item("Status")
                End If

                If intView = 3 And strStatus = "70" Then
                    tb = API.EmployeeServiceMethods.GetTaskPlan(Nothing, intIDEmployee, xDateMove)
                    If tb.Rows.Count > 0 Then
                        strStatus = tb.Rows(0).Item("Status")
                        If strStatus = "80" Then
                            strStatus = "70"
                        End If
                    End If
                End If
            Else
                strStatus = "-1"
            End If
        Catch ex As Exception

        End Try


        If strStatus = "" Then
            strStatus = "70"
        End If
        Response.Write(strStatus)

    End Sub

    Private Sub DeleteProgAus()

        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""

        Try

            Dim oProgrammedAbsence As New roProgrammedAbsence
            ' Obtenemos los campos de la ausencia
            oProgrammedAbsence.IDCause = Request("IDCause")
            oProgrammedAbsence.IDEmployee = Request("ID")
            Dim strDate As String = Request("BeginDate")
            oProgrammedAbsence.BeginDate = CDate(strDate).Date

            If Not API.ProgrammedAbsencesServiceMethods.DeleteProgrammedAbsence(Nothing, oProgrammedAbsence, True) Then
                strErrorInfo = API.ProgrammedAbsencesServiceMethods.LastErrorText
            End If


        Catch ex As Exception
            strErrorInfo = ex.ToString
        End Try
        If strErrorInfo <> "" Then
            strResponse = "MESSAGE" &
                          "TitleKey=RemoveProgrammedAbsence.Error.Title&" +
                          "DescriptionText=" + strErrorInfo + "&" +
                          "Option1TextKey=RemoveProgrammedAbsence.Error.Option1Text&" +
                          "Option1DescriptionKey=RemoveProgrammedAbsence.Error.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
        End If
        Response.Write(strResponse)

    End Sub

    Private Sub GetCapture()

        If Request("ID") = "" Or Not IsNumeric(Request("ID")) Then Exit Sub

        'Dim oMove As MoveService.roMove = MoveService.MoveServiceMethods.GetMove(Nothing, Request("ID"))

        'If oMove IsNot Nothing Then

        '    Dim bImage() As Byte = Nothing
        '    Dim strDateTime As String = ""
        '    If Request("Type") = "IN" Then
        '        If oMove.CaptureINBytes IsNot Nothing AndAlso oMove.CaptureINBytes.Length > 0 Then
        '            bImage = oMove.CaptureINBytes
        '        End If
        '        If oMove.DateTimeIN.HasValue Then strDateTime = oMove.DateTimeIN.Value.ToString
        '    Else
        '        If oMove.CaptureOUTBytes IsNot Nothing AndAlso oMove.CaptureOUTBytes.Length > 0 Then
        '            bImage = oMove.CaptureOUTBytes
        '        End If
        '        If oMove.DateTimeOUT.HasValue Then strDateTime = oMove.DateTimeOUT.Value.ToString
        '    End If

        '    If bImage IsNot Nothing AndAlso bImage.Length > 0 Then
        '        Dim ms As MemoryStream = New MemoryStream()

        '        ms.Write(bImage, 0, bImage.Length - 1)
        '        Dim oImg As Image = Image.FromStream(ms)
        '        Dim gr As Graphics = Graphics.FromImage(oImg)

        '        Dim pgRect As Rectangle = New Rectangle(0, 0, oImg.Width, oImg.Height)
        '        Dim solidBlack As SolidBrush = New SolidBrush(Color.Yellow)

        '        Dim fn As Font = New Font("Arial", 16)
        '        gr.DrawString(strDateTime, fn, solidBlack, 10, 10)

        '        Dim msResult As MemoryStream = New MemoryStream()
        '        oImg.Save(msResult, Imaging.ImageFormat.Jpeg)
        '        Dim bytImage(msResult.Length) As Byte
        '        bytImage = msResult.ToArray()
        '        msResult.Close()

        '        Response.Clear()
        '        Response.ContentType = "image/jpeg"
        '        Response.BinaryWrite(bytImage)

        '    Else
        '        Response.Write("No capture")
        '    End If
        'End If


    End Sub

End Class