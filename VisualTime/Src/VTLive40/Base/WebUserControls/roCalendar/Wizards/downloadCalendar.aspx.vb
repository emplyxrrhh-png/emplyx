Imports Robotics.Web.Base

Partial Class Wizards_downloadCalendar
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim fileName As String = "CalendarExport.xlsx"
        Dim arrBytes As Byte() = HttpContext.Current.Session("CALENDAR_EXPORT")

        Me.Controls.Clear()
        Response.Clear()
        Response.ClearHeaders()
        Response.ClearContent()
        If fileName.Length > 0 AndAlso arrBytes IsNot Nothing AndAlso arrBytes.Length > 0 Then

            'Dim downloadFileName As String = IO.Path.GetFileName(fileName)
            'If downloadFileName.Contains("_") Then
            '    downloadFileName = downloadFileName.Split("_")(0)
            '    downloadFileName = downloadFileName & IO.Path.GetExtension(fileName)
            'End If

            Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileName.Replace(" ", "_") & """")
            Response.AddHeader("Content-Type", "application/force-download")
            Response.AddHeader("Content-Transfer-Encoding", "binary")
            Response.AddHeader("Content-Length", arrBytes.Length)
            Response.ContentType = "application/octet-stream"
            Response.OutputStream.Write(arrBytes, 0, arrBytes.Length)

            Response.Flush()
            Response.Close()

            HttpContext.Current.Session("CALENDAR_EXPORT") = Nothing
        End If
    End Sub

End Class