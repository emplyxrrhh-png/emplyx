Imports System.Drawing
Imports System.IO
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Scheduler_LoadImageCapture
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim bImage() As Byte = Nothing
        Dim strEmployeeName As String = String.Empty

        Dim IdPunch As Integer = roTypes.Any2Integer(Request.QueryString("IdPunch"))
        If IdPunch > 0 Then

            Try
                Dim tmpPunch As roPunch = PunchServiceMethods.GetPunch(Me.Page, IdPunch, True)

                If tmpPunch IsNot Nothing AndAlso tmpPunch.GetCaptureBytes().Length > 0 Then
                    bImage = tmpPunch.GetCaptureBytes()
                End If

                strEmployeeName = API.EmployeeServiceMethods.GetEmployeeName(Me.Page, tmpPunch.IDEmployee)
            Catch
            End Try

            If bImage Is Nothing OrElse bImage.Length = 0 Then
                Dim fs As FileStream = New FileStream(Server.MapPath("~/Scheduler/Images/NoUser.jpg"), FileMode.Open, FileAccess.Read)
                Dim br As BinaryReader = New BinaryReader(fs)
                ReDim bImage(fs.Length)
                br.Read(bImage, 0, fs.Length)
                br.Close()
                fs.Close()
            End If
        Else

            'No hay captura
            Dim fs As FileStream = New FileStream(Server.MapPath("~/Scheduler/Images/NoUser.jpg"), FileMode.Open, FileAccess.Read)
            Dim br As BinaryReader = New BinaryReader(fs)
            ReDim bImage(fs.Length)
            br.Read(bImage, 0, fs.Length)
            br.Close()
            fs.Close()

        End If

        If IdPunch > 0 Then
            Dim lstAuditParameterNames As New List(Of String)
            Dim lstAuditParameterValues As New List(Of String)

            lstAuditParameterNames.Add("{EmployeeName}")
            lstAuditParameterValues.Add(strEmployeeName)

            lstAuditParameterNames.Add("{IdPunch}")
            lstAuditParameterValues.Add(IdPunch)

            API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aSelect, Robotics.VTBase.Audit.ObjectType.tPunchImage, "", lstAuditParameterNames, lstAuditParameterValues, Nothing)
        End If

        Dim ms As MemoryStream = New MemoryStream()

        ms.Write(bImage, 0, bImage.Length - 1)
        Dim oImg As System.Drawing.Image = System.Drawing.Image.FromStream(ms)
        Dim gr As Graphics = Graphics.FromImage(oImg)

        Dim msResult As MemoryStream = New MemoryStream()
        oImg.Save(msResult, Imaging.ImageFormat.Jpeg)
        Dim bytImage(msResult.Length) As Byte
        bytImage = msResult.ToArray()
        msResult.Close()

        Response.Clear()
        Response.ContentType = "image/jpeg"
        Response.BinaryWrite(bytImage)
    End Sub

End Class