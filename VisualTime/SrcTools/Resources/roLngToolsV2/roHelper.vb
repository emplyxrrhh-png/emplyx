Imports System.IO
Imports System.Text

Public Class roHelper
    Public Shared Function GetFileEncoding(filePath As String) As Encoding
        Using sr As New StreamReader(filePath, True)
            sr.Read()
            Return sr.CurrentEncoding
        End Using
    End Function
End Class
