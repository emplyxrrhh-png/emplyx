Public Class Helper

    Public Shared Function GetSessionID() As String
        Dim sessionID As String = Nothing
        Dim index As Integer = 0
        Dim random As Random = Nothing

        Dim strList As New List(Of String)()

        For i As Integer = 0 To 9
            strList.Add(i.ToString())
        Next

        For i As Integer = 97 To 122
            Dim aa As Char = Chr(i) 'CChar(CChar(CStr(i)))
            strList.Add(aa.ToString())
        Next

        random = New Random()
        For i As Integer = 0 To 9
            index = random.[Next](strList.Count)
            sessionID = sessionID + strList(index)
        Next

        'TODO reponet el valor correcto
        Return sessionID
    End Function

    Public Shared Function GetRegistryCode() As String
        Dim registryCode As String = Nothing
        Dim index As Integer = 0
        Dim random As Random = Nothing
        'int [] index = new int[10];

        Dim strList As New List(Of String)()

        For i As Integer = 65 To 90
            Dim aa As Char = CChar(ChrW(i))
            strList.Add(aa.ToString())
        Next

        Dim number As Char() = New Char(9) {}
        For i As Integer = 48 To 57
            Dim aa As Char = CChar(ChrW(i))
            strList.Add(aa.ToString())
            number(i - 48) = aa
        Next

        For i As Integer = 97 To 122
            Dim aa As Char = CChar(ChrW(i))
            strList.Add(aa.ToString())
        Next

        random = New Random()
        For i As Integer = 0 To 9
            index = random.[Next](strList.Count)
            'strList.RemoveAt(index);
            registryCode = registryCode + strList(index)
        Next

        Return registryCode
    End Function

    Public Shared Function Normal2ZKDatetime(dDate As DateTime) As Integer
        Try
            Dim iYear As Integer = 0
            Dim iMonth As Integer = 0
            Dim iDay As Integer = 0
            Dim iHour As Integer = 0
            Dim iMinute As Integer = 0
            Dim iSecond As Integer = 0

            Return ((iYear - 2000) * 12 * 31 + (iMonth - 1) * 31 + (iDay - 1)) * (24 * 60 * 60) + iHour * 60 * 60 + iMinute * 60 + iSecond
        Catch ex As Exception
            Return 0
        End Try
    End Function

End Class