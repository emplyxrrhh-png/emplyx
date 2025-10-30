Imports System.Drawing
Imports System.IO
Imports Robotics.Base.DTOs.UserFieldsTypes

Public Class roImagesHelper

    Public Shared Function ResizeImageIfNeeded(ByVal bytes As Byte(), ByVal Width As Integer, ByVal Height As Integer) As Byte()
        Try
            Dim imagenStream As New MemoryStream(bytes)
            Dim oImg As Image = Image.FromStream(imagenStream)

            'Si le pasamos Height = 0, se calcula la altura proporcionalmente
            If Height = 0 Then
                Height = CInt((oImg.Height / oImg.Width) * Width)
            End If
            If oImg.Width > Width Or oImg.Height > Height Then
                Dim ms As IO.MemoryStream = New IO.MemoryStream
                Dim thumb As New Drawing.Bitmap(Width, Height)
                Dim g As Drawing.Graphics = Drawing.Graphics.FromImage(thumb)
                g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                g.CompositingMode = Drawing2D.CompositingMode.SourceOver
                g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

                g.DrawImage(oImg, 0, 0, Width, Height)
                g.Dispose()
                thumb.Save(ms, oImg.RawFormat)
                oImg.Dispose()
                thumb.Dispose()
                bytes = ms.ToArray
            End If

            Return bytes
        Catch ex As Exception
            Return Array.CreateInstance(GetType(Byte), 0)
        End Try
    End Function

End Class