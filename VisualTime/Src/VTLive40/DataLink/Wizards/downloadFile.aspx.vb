Imports System.IO
Imports System.IO.Compression
Imports Robotics.Web.Base

Partial Class Wizards_downloadFile
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Sin webservice por problemas de tamaño AGM
        'Response.ClearContent()
        'Response.Clear()
        'Response.ContentType = "text/plain"
        'Response.AddHeader("Content-Disposition",
        '                   "attachment; filename=" + fileName)
        'Response.TransmitFile(fileName)
        'Response.Flush()
        'Response.End()

        Dim fileName As String = HttpContext.Current.Session("ExportFileName")
        Dim arrBytes As Byte() = {}
        Dim exportIsTMPFile As Boolean = False

        If HttpContext.Current.Session("ExportIsTMPFile") IsNot Nothing Then

            exportIsTMPFile = HttpContext.Current.Session("ExportIsTMPFile")

            If exportIsTMPFile Then
                Dim fInfo As New FileInfo(fileName)
                Dim fs As New FileStream(fileName, FileMode.Open, FileAccess.Read)
                Dim br As New BinaryReader(fs)
                arrBytes = br.ReadBytes(CInt(fInfo.Length))
                br.Close()
                fs.Close()
            End If

            HttpContext.Current.Session("ExportIsTMPFile") = Nothing
        Else
            arrBytes = API.LiveTasksServiceMethods.GetExportedFileZipped(Me.Page, fileName)
        End If

        Me.Controls.Clear()
        Response.Clear()
        Response.ClearHeaders()
        Response.ClearContent()
        'If fileName.Length > 0 AndAlso arrBytes IsNot Nothing AndAlso arrBytes.Length > 0 Then

        Dim downloadFileName As String = String.Empty

        If Not exportIsTMPFile Then

            'descompresión de archivo
            Dim stream = New MemoryStream()
            Dim zipStream = New DeflateStream(New MemoryStream(arrBytes), CompressionMode.Decompress, True)
            Dim buffer = New Byte(4095) {}
            While True
                Dim size = zipStream.Read(buffer, 0, buffer.Length)
                If size > 0 Then
                    stream.Write(buffer, 0, size)
                Else
                    Exit While
                End If
            End While
            zipStream.Close()

            arrBytes = stream.ToArray()
            downloadFileName = IO.Path.GetFileName(fileName)
        Else
            fileName = HttpContext.Current.Session("ExportRealFileName")
            downloadFileName = fileName
        End If

        'generacion de archivo
        If downloadFileName.Contains("_") Then
            downloadFileName = downloadFileName.Split("_")(0)
            downloadFileName = downloadFileName & IO.Path.GetExtension(fileName)
        End If

        Response.AddHeader("Content-Disposition", "attachment; filename=""" & downloadFileName.Replace(" ", "_") & """")
        Response.AddHeader("Content-Type", "application/force-download")
        Response.AddHeader("Content-Transfer-Encoding", "binary")
        Response.AddHeader("Content-Length", arrBytes.Length)
        Response.ContentType = "application/octet-stream"
        Response.OutputStream.Write(arrBytes, 0, arrBytes.Length)

        Response.Flush()
        Response.Close()
        'End If
    End Sub

End Class