Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class DownloadLogs
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

    Protected Sub DownloadFile(ByVal arrBytes As Byte())
        Try
            If arrBytes IsNot Nothing AndAlso arrBytes.Length > 0 Then
                Me.Controls.Clear()
                Response.Clear()
                Response.ClearHeaders()
                Response.ClearContent()

                Response.AddHeader("Content-Disposition", "attachment; filename=""Logs.zip""")
                Response.AddHeader("Content-Type", "application/force-download")
                Response.AddHeader("Content-Transfer-Encoding", "binary")
                Response.AddHeader("Content-Length", arrBytes.Length)
                Response.ContentType = "application/octet-stream"
                Response.OutputStream.Write(arrBytes, 0, arrBytes.Length)
                Response.Flush()
                Response.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub VisualizeDocument(ByVal deliveredDocument As roDocumentFile)

        Try

            Response.Clear()
            Response.ClearHeaders()
            Response.ClearContent()

            Dim strNameFile = "_"

            Response.AddHeader("Content-Disposition", "attachment; filename=""" & deliveredDocument.DocumentName.Replace(" ", "_") & """")
            Response.AddHeader("Content-Type", "application/octet-stream")
            Response.AddHeader("Content-Transfer-Encoding", "binary")
            Response.AddHeader("Content-Length", deliveredDocument.DocumentContent.Length)
            Response.ContentType = "application/octet-stream"
            Response.OutputStream.Write(deliveredDocument.DocumentContent, 0, deliveredDocument.DocumentContent.Length)
            Response.Flush()
            Response.Close()
        Catch ex As Exception
            Response.Clear()
            Response.ClearHeaders()
            Response.ClearContent()

            Response.ContentType = "image/gif"
            Response.WriteFile(Me.Server.MapPath("..\Reports\Images\PRINTER_REMOVE_256.GIF"))
            Response.Flush()
        End Try

    End Sub

End Class