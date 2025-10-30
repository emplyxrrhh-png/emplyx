Imports System.IO

Public Class MyLog

    Public Shared Sub WriteLog(ByVal sender As Object, ByVal msg As String)
        Dim sw As StreamWriter = New StreamWriter(getFileName(), True)
        If msg.Contains("End") Then msg += vbNewLine
        Dim p As Integer = sender.ToString().LastIndexOf("_")
        p = sender.ToString().Substring(0, p - 1).LastIndexOf("_")
        sw.WriteLine(String.Format("{0}: {1} | {2}", DateTime.Now.TimeOfDay, sender.ToString().Substring(p + 1), msg))
        sw.Close()
    End Sub

    Public Shared Sub WriteLog(ByVal msg As String)
        Dim sw As StreamWriter = New StreamWriter(getFileName(), True)
        If msg.Contains("End") Then msg += vbNewLine
        sw.WriteLine(String.Format("{0}: {1}", DateTime.Now.TimeOfDay, msg))
        sw.Close()
    End Sub

    Private Shared Function getFileName() As String
        Return String.Format("c:\vtLive_{0}.log", DateTime.Now.ToString("yyyyMMdd"))
    End Function

    Public Shared Sub ClearLog()
        File.Delete(getFileName())
    End Sub

End Class