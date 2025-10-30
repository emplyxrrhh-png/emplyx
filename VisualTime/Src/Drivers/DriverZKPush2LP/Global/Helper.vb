Public Class Helper

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
            Dim iYear As Integer = dDate.Year
            Dim iMonth As Integer = dDate.Month
            Dim iDay As Integer = dDate.Day
            Dim iHour As Integer = dDate.Hour
            Dim iMinute As Integer = dDate.Minute
            Dim iSecond As Integer = dDate.Second

            Return ((iYear - 2000) * 12 * 31 + (iMonth - 1) * 31 + (iDay - 1)) * (24 * 60 * 60) + iHour * 60 * 60 + iMinute * 60 + iSecond
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Shared Function GetAdjustmentDate(ByVal transitionTime As TimeZoneInfo.TransitionTime, ByVal year As Integer) As DateTime
        If transitionTime.IsFixedDateRule Then
            Return New DateTime(year, transitionTime.Month, transitionTime.Day)
        Else
            Dim cal As System.Globalization.Calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar
            Dim startOfWeek As Integer = transitionTime.Week * 7 - 6
            Dim firstDayOfWeek As Integer = CInt(cal.GetDayOfWeek(New DateTime(year, transitionTime.Month, 1)))
            Dim transitionDay As Integer
            Dim changeDayOfWeek As Integer = CInt(transitionTime.DayOfWeek)

            If firstDayOfWeek <= changeDayOfWeek Then
                transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek)
            Else
                transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek)
            End If

            If transitionDay > cal.GetDaysInMonth(year, transitionTime.Month) Then transitionDay -= 7
            Return New DateTime(year, transitionTime.Month, transitionDay, transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second)
        End If
    End Function

    Public Shared Function Decode(ByVal packet As Byte()) As Byte()
        Dim temp As Byte() = {}
        Try
            Dim i = packet.Length - 1

            While packet(i) = 0
                i = i - 1
            End While

            temp = New Byte(i) {}
            Array.Copy(packet, temp, i + 1)
        Catch ex As Exception
            Return packet
        End Try

        Return temp
    End Function

End Class