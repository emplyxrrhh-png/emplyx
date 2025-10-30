Imports System.Drawing
Imports System.IO
Imports Robotics.Base.VTBusiness.Capture
Imports Robotics.Web.Base

Partial Class CapturePhoto
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim bImage() As Byte '= WLHelperWeb.CurrentPassportPhoto(Me)
        'Dim oStrm As New System.IO.FileStream(Me.Server.MapPath("..\Images\User_128.gif"), IO.FileMode.Open, IO.FileAccess.Read)
        'Dim oReader As New System.IO.BinaryReader(oStrm)
        'ReDim bImage(oReader.BaseStream.Length - 1)
        'oStrm.Read(bImage, 0, bImage.Length)
        'oReader.Read(bImage, 0, oReader.BaseStream.Length)
        'Response.Clear()
        'Response.ContentType = "image/gif"
        'Response.BinaryWrite(bImage)
        'oReader.Close()
        'oStrm.Close()

        If Request("ID") = "" Or Not IsNumeric(Request("ID")) Then Exit Sub

        Dim bImage() As Byte = Nothing
        Dim oCapture As roCapture = API.CaptureServiceMethods.GetCaptureByID(Me.Page, Me.Request("ID"))

        If oCapture IsNot Nothing Then
            If oCapture.Capture IsNot Nothing AndAlso oCapture.Capture.Length > 0 Then
                bImage = oCapture.Capture
            End If

            If bImage IsNot Nothing AndAlso bImage.Length > 0 Then
                Dim ms As MemoryStream = New MemoryStream()

                ms.Write(bImage, 0, bImage.Length - 1)
                Dim oImg As Image = Image.FromStream(ms)
                Dim gr As Graphics = Graphics.FromImage(oImg)

                Dim pgRect As Rectangle = New Rectangle(0, 0, oImg.Width, oImg.Height)
                Dim solidBlack As SolidBrush = New SolidBrush(Color.Yellow)

                Dim fn As Font = New Font("Arial", 16)
                gr.DrawString(oCapture.DateTime.ToString, fn, solidBlack, 10, 10)

                Dim msResult As MemoryStream = New MemoryStream()
                oImg.Save(msResult, Imaging.ImageFormat.Jpeg)
                Dim bytImage(msResult.Length) As Byte
                bytImage = msResult.ToArray()
                msResult.Close()

                Response.Clear()
                Response.ContentType = "image/jpeg"
                Response.BinaryWrite(bytImage)
            Else
                Response.Write("No capture")
                'Dim oStrm As New System.IO.FileStream(Me.Server.MapPath("..\Images\userStart.png"), IO.FileMode.Open, IO.FileAccess.Read)
                'Dim oReader As New System.IO.BinaryReader(oStrm)
                'ReDim bImage(oReader.BaseStream.Length - 1)
                'oStrm.Read(bImage, 0, bImage.Length)
                'oReader.Read(bImage, 0, oReader.BaseStream.Length)
                'Response.Clear()
                'Response.ContentType = "image/png"
                'Response.BinaryWrite(bImage)
                'oReader.Close()
                'oStrm.Close()
            End If
        End If

    End Sub

End Class