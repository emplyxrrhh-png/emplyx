Imports System.Drawing
Imports System.IO
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Web.Base

Partial Class LoadPlaneImg
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Request("ID") = "" Or Not IsNumeric(Request("ID")) Then Exit Sub

        Dim bImage() As Byte = Nothing
        Dim oZonePlane As roZonePlane = API.ZoneServiceMethods.GetZonePlaneByID(Me.Page, Me.Request("ID"), False)

        If oZonePlane IsNot Nothing Then
            If oZonePlane.PlaneImage IsNot Nothing AndAlso oZonePlane.PlaneImage.Length > 0 Then
                bImage = oZonePlane.PlaneImage
            End If

            If bImage IsNot Nothing AndAlso bImage.Length > 0 Then
                Dim ms As MemoryStream = New MemoryStream()

                ms.Write(bImage, 0, bImage.Length - 1)
                Dim oImg As Image = Image.FromStream(ms)
                Dim gr As Graphics = Graphics.FromImage(oImg)

                Dim bytImage(-1) As Byte

                If oImg.Width > 250 Or oImg.Height > 250 Then
                    Dim oResImage As System.Drawing.Image = HelperWeb.ResizeImage(oImg, 400, 300)

                    Dim msResult As MemoryStream = New MemoryStream()
                    oResImage.Save(msResult, Imaging.ImageFormat.Jpeg)
                    ReDim bytImage(msResult.Length)
                    bytImage = msResult.ToArray()
                    msResult.Close()
                Else
                    Dim msResult As MemoryStream = New MemoryStream()
                    oImg.Save(msResult, Imaging.ImageFormat.Jpeg)
                    ReDim bytImage(msResult.Length)
                    bytImage = msResult.ToArray()
                    msResult.Close()
                End If

                Response.Clear()
                Response.ContentType = "image/png"
                Response.BinaryWrite(bytImage)
            Else
                Response.Write("Nothing to do")
            End If
        End If

    End Sub

End Class