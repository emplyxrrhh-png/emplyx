Imports System.IO
Imports System.IO.Compression
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Alerts_downloadFile
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim action As String = roTypes.Any2String(Request("Action"))
        Dim idTask As String = roTypes.Any2Integer(Request("TaskID"))
        Dim pFileName As String = roTypes.Any2String(Request("FileName"))
        Dim downloadFileName As String = String.Empty

        Dim fileName As String = String.Empty
        Dim arrBytes As Byte() = {}
        If pFileName <> String.Empty Then
            fileName = pFileName

            If action.ToLower = "template" Then
                arrBytes = API.LiveTasksServiceMethods.GetCommonTemplateFile(Me.Page, fileName)
            End If

            downloadFileName = IO.Path.GetFileName(fileName)
            If downloadFileName.Contains("_") Then
                downloadFileName = downloadFileName.Split("_")(0)
                downloadFileName = downloadFileName & IO.Path.GetExtension(fileName)
            End If
        Else
            Dim strError As String = String.Empty
            Dim iStatus As Integer = 0
            Select Case action.ToUpper()
                Case "EXPORT"
                    Dim oLiveTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Nothing, idTask)
                    If oLiveTask IsNot Nothing AndAlso oLiveTask.IDPassport = WLHelperWeb.CurrentPassport.ID Then
                        fileName = oLiveTask.ErrorCode
                    End If

                    arrBytes = API.LiveTasksServiceMethods.GetExportedFileZipped(Me.Page, fileName)

                    If arrBytes IsNot Nothing AndAlso arrBytes.Length > 0 Then
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
                    End If

                    downloadFileName = IO.Path.GetFileName(fileName)
                    If downloadFileName.Contains("_") Then
                        downloadFileName = downloadFileName.Split("_")(0)
                        downloadFileName = downloadFileName & IO.Path.GetExtension(fileName)
                    End If
                Case "REPORTTASKDX"
                    Dim oTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Me.Page, idTask)
                    Dim oCollection As New roCollection(oTask.XmlParameters)
                    Dim executionGuid As Guid = Guid.Parse(oCollection("Guid"))

                    Dim response As (Byte(), String) = ReportServiceMethods.GetExecutionAssocietedExportFile(executionGuid, Nothing)
                    arrBytes = response.Item1
                    downloadFileName = "report." & response.Item2
            End Select
        End If

        Me.Controls.Clear()
        Response.Clear()
        Response.ClearHeaders()
        Response.ClearContent()
        If downloadFileName.Length > 0 AndAlso arrBytes IsNot Nothing AndAlso arrBytes.Length > 0 Then

            Response.AddHeader("Content-Disposition", "attachment; filename=""" & downloadFileName.Replace(" ", "_") & """")
            Response.AddHeader("Content-Type", "application/force-download")
            Response.AddHeader("Content-Transfer-Encoding", "binary")
            Response.AddHeader("Content-Length", arrBytes.Length)
            Response.ContentType = "application/octet-stream"
            Response.OutputStream.Write(arrBytes, 0, arrBytes.Length)

            Response.Flush()
            Response.Close()
        End If
    End Sub

End Class