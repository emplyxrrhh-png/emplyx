Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

#Region "roReportSchedulerSchedule"

<DataContract()>
Public Class roReportSchedulerScheduleManager

    Public Shared Function retScheduleString(ByVal oConf As roReportSchedulerSchedule) As String
        Dim strRet As String = ""

        Select Case oConf.ScheduleType
            Case eRSScheduleType.Hours ' Horas
                strRet = "4@" & oConf.Hour & "@"
                strRet &= oConf.Days.ToString
            Case eRSScheduleType.Daily ' Diario
                strRet = "0@" & oConf.Hour & "@"
                strRet &= oConf.Days.ToString
            Case eRSScheduleType.Weekly ' Semanal
                strRet = "1@" & oConf.Hour & "@"
                strRet &= oConf.Weeks.ToString & "@"
                strRet &= oConf.WeekDays
            Case eRSScheduleType.Monthly ' Mensual
                strRet = "2@" & oConf.Hour & "@"
                If oConf.MonthlyType = eRSMonthlyType.DayOfMonth Then ' Dia
                    strRet &= "0@" & oConf.Day.ToString
                ElseIf oConf.MonthlyType = eRSMonthlyType.StartAndDayMonth Then  'Inici, Dia i Mes
                    strRet &= "1@" & oConf.Start.ToString & "@" & CInt(oConf.WeekDay).ToString
                End If
            Case eRSScheduleType.OneTime  'Una vez
                strRet = "3@" & oConf.Hour & "@"
                strRet &= oConf.DateSchedule.Year & "/" & oConf.DateSchedule.Month & "/" & oConf.DateSchedule.Day & " 00:00:00"
        End Select
        Return strRet
    End Function

    Public Shared Function Load(ByVal strSchedule As String) As roReportSchedulerSchedule
        If strSchedule = "" Then Return Nothing
        Dim arrStr As String()
        arrStr = strSchedule.Split("@")

        Dim oRet As New roReportSchedulerSchedule

        If arrStr.Length > 0 Then
            oRet.Hour = arrStr(1).ToString
            Select Case arrStr(0)
                Case "0" ' Diari (Dies)
                    oRet.ScheduleType = eRSScheduleType.Daily
                    oRet.Days = CInt(arrStr(2))
                Case "1" ' Semanal (Dia Semana)
                    oRet.ScheduleType = eRSScheduleType.Weekly
                    oRet.Weeks = CInt(arrStr(2))
                    oRet.WeekDays = arrStr(3)
                Case "2" ' Mensual
                    oRet.ScheduleType = eRSScheduleType.Monthly
                    Select Case arrStr(2).ToString
                        Case "0" 'El día X de cada mes
                            oRet.MonthlyType = eRSMonthlyType.DayOfMonth
                            oRet.Day = CInt(arrStr(3))
                        Case "1" 'El {primer,...} {lunes,...} de cada mes
                            oRet.MonthlyType = eRSMonthlyType.StartAndDayMonth
                            oRet.Start = CInt(arrStr(3))
                            oRet.WeekDay = CType(arrStr(4), eRSWeekDay)
                    End Select
                Case "3" ' Una sola vez
                    oRet.ScheduleType = eRSScheduleType.OneTime
                    Dim arrDate As String() = arrStr(2).Split(" ")(0).Replace("-", "/").Split("/")
                    oRet.DateSchedule = roTypes.CreateDateTime(arrDate(0), arrDate(1), arrDate(2).Split("+")(0), 0, 0, 0)
                Case "4" ' horas
                    oRet.ScheduleType = eRSScheduleType.Hours
            End Select
        End If

        Return oRet
    End Function


    Public Function IsEqual(ByVal bSourceItem As roReportSchedulerSchedule, ByVal oCompareItem As roReportSchedulerSchedule) As Boolean

        Dim bolRet As Boolean = False

        If oCompareItem IsNot Nothing Then
            bolRet = (bSourceItem.ToString() = oCompareItem.ToString())
        End If

        Return bolRet

    End Function

#End Region

End Class