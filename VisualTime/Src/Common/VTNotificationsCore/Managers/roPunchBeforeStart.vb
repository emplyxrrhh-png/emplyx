Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class roNotificationTask_PunchBeforeStart_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.PunchBeforeStart, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return _oNotificationTask.Key1Numeric
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return True
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Dim oDest As roNotificationDestinationConfig() = {}
        Select Case _oNotificationTask.key2Numeric
            Case 0 'Usuario
                oDest = Me.GetDefaultDestinationConfig(False, True, False, True)
            Case 1 'Supervisor
                oDest = Me.GetDefaultDestinationConfig(False, False, True, False)
        End Select

        Return oDest
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem

        Dim oItem As New roNotificationItem
        Try
            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Employees WHERE ID = " & _oNotificationTask.Key1Numeric))
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()

            Select Case _oNotificationTask.key2Numeric
                Case 0 'Usuario
                    oItem.Subject = roNotificationHelper.Message("Notification.PunchBeforeStart.Subject", Nothing, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.PunchBeforeStart.Body", Params, , oConf.Language)
                Case 1 'Supervisor
                    oItem.Subject = roNotificationHelper.Message("Notification.PunchBeforeStartSupervisor.Subject", Params, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.PunchBeforeStartSupervisor.Body", Params, , oConf.Language)
            End Select

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roPunchBeforeStart::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Dim SQL As String
        Dim bRet As Boolean = True

        Try
            Dim strEmployeesIds = String.Empty
            Dim tZoneInfo As TimeZoneInfo = Nothing

            Dim isJune6610 As Boolean = VTBase.roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "June6610"))

            If Not isJune6610 OrElse roTypes.Any2String(ads("CreatorID")).ToUpper = "SYSTEM" Then
                ' Notificación de sistema
                ' Visualtime previo a v6.6.10

                'Obtenemos todos aquellos employees que tengan fichajes de entrada de hoy con un shift asignado
                SQL = "@SELECT# distinct(p.IDEmployee) " &
                      "FROM Punches P " &
                      "INNER JOIN DailySchedule ds on p.IDEmployee = ds.IDEmployee " &
                      "WHERE p.ActualType = 1 AND p.ShiftDate=@Date AND ds.Date=@Date"
                SQL = Strings.Replace(SQL, "@Date", roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime)
                Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    ' Para cada empleado
                    For Each dr As DataRow In dt.Rows
                        strEmployeesIds += String.Format("B{0},", dr("IDEmployee"))
                    Next

                    If strEmployeesIds.Length > 0 Then
                        Dim oCalendarState As VTCalendar.roCalendarState = New VTCalendar.roCalendarState(-1)
                        Dim calendarManager As VTCalendar.roCalendarManager = New VTCalendar.roCalendarManager(oCalendarState)
                        Dim calendar As roCalendar = calendarManager.Load(DateTime.Now.Date, DateTime.Now.Date, strEmployeesIds.Substring(0, strEmployeesIds.Length - 1), DTOs.CalendarView.Planification, CalendarDetailLevel.Daily, False,,,,, True)
                        If calendar IsNot Nothing AndAlso calendar.CalendarData IsNot Nothing Then
                            For Each calendarRow As roCalendarRow In calendar.CalendarData
                                For Each dayData As roCalendarRowDayData In calendarRow.PeriodData.DayData
                                    tZoneInfo = Nothing
                                    If dayData IsNot Nothing AndAlso dayData.PunchData IsNot Nothing AndAlso dayData.PunchData.Length > 0 AndAlso dayData.ShiftUsed IsNot Nothing AndAlso dayData.ShiftUsed.EnableNotifyBefore Then

                                        ' Obtenemos las previsiones de horas de exceso de hoy
                                        Dim lstProgrammedOvertimes As New Generic.List(Of roProgrammedOvertime)
                                        Dim bState = New roProgrammedOvertimeState(-1)
                                        Dim oProgrammedOvertimeManager As New roProgrammedOvertimeManager(bState)

                                        lstProgrammedOvertimes = oProgrammedOvertimeManager.GetProgrammedOvertimes(calendarRow.EmployeeData.IDEmployee, bState, "BeginDate =" & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime)

                                        Dim horaInicioJornada = dayData.ShiftUsed.StartHour.AddMinutes(dayData.ShiftUsed.NotifyEmployeeBeforeAt * -1)
                                        Dim inicioJornada As Date = New Date(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, horaInicioJornada.Hour, horaInicioJornada.Minute, horaInicioJornada.Second)
                                        inicioJornada = roNotificationHelper.GetServerTime(calendarRow.EmployeeData.IDEmployee, inicioJornada, tZoneInfo)

                                        Dim esHorasExceso As Boolean = False
                                        For Each horaExceso As roProgrammedOvertime In lstProgrammedOvertimes
                                            Dim horaExcesoBegin As Date = New Date(horaExceso.ProgrammedBeginDate.Year, horaExceso.ProgrammedBeginDate.Month, horaExceso.ProgrammedBeginDate.Day, horaExceso.BeginTime.Hour, horaExceso.BeginTime.Minute, horaExceso.BeginTime.Second)
                                            Dim horaExcesoFinish As Date = New Date(horaExceso.ProgrammedEndDate.Year, horaExceso.ProgrammedEndDate.Month, horaExceso.ProgrammedEndDate.Day, horaExceso.EndTime.Hour, horaExceso.EndTime.Minute, horaExceso.EndTime.Second)
                                            If horaExcesoBegin <= inicioJornada AndAlso horaExcesoFinish >= inicioJornada Then
                                                esHorasExceso = True
                                                Exit For
                                            End If
                                        Next

                                        'Obtenemos el punch antes del inicio de jornada
                                        Dim punches As List(Of roCalendarRowPunchData) = dayData.PunchData.Where(Function(x) x.DateTimePunch < inicioJornada).OrderBy(Function(x) x.DateTimePunch).ToList()
                                        Dim entryPunch As roCalendarRowPunchData = Nothing
                                        If punches IsNot Nothing AndAlso punches.Count > 0 Then
                                            entryPunch = punches.Last()
                                        End If

                                        'Si no hay horas de exceso y existe un punch de entrada antes del inicio de la jornada con el tiempo aplicado
                                        If Not esHorasExceso AndAlso entryPunch IsNot Nothing AndAlso entryPunch.ActualType = CalendarPunchTypeEnum._IN Then
                                            'Comprobar que no existe la notificación
                                            SQL = "@SELECT# count(id) FROM sysroNotificationTasks WHERE IDNotification=@idNotification AND Key1Numeric=@idEmployee AND Key3DateTime=@punchDate"
                                            SQL = Strings.Replace(SQL, "@idNotification", roTypes.Any2String(ads("ID")))
                                            SQL = Strings.Replace(SQL, "@idEmployee", roTypes.Any2String(calendarRow.EmployeeData.IDEmployee))
                                            SQL = Strings.Replace(SQL, "@punchDate", roTypes.Any2Time(entryPunch.DateTimePunch).SQLDateTime)

                                            Dim notificationNoExist As Boolean = AccessHelper.ExecuteScalar(SQL) = 0

                                            If notificationNoExist Then
                                                Dim WhoToNotify = roTypes.Any2Double(dayData.ShiftUsed.WhoToNotifyBefore)
                                                SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime) VALUES (@idNotification, @idEmployee, @whoToNotify, @punchDate)"
                                                SQL = Strings.Replace(SQL, "@idNotification", roTypes.Any2String(ads("ID")))
                                                SQL = Strings.Replace(SQL, "@idEmployee", roTypes.Any2String(calendarRow.EmployeeData.IDEmployee))
                                                SQL = Strings.Replace(SQL, "@punchDate", roTypes.Any2Time(entryPunch.DateTimePunch).SQLDateTime)
                                                Select Case WhoToNotify
                                                    Case 0, 1 'A supervisor o a usuario
                                                        'Crear la notificación
                                                        SQL = Strings.Replace(SQL, "@whoToNotify", roTypes.Any2String(WhoToNotify))
                                                        bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                                                    Case 2 'se envia a ambos
                                                        'Crear la notificación
                                                        Dim SQL0 = SQL
                                                        SQL0 = Strings.Replace(SQL0, "@whoToNotify", "0")
                                                        bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL0)

                                                        'Crear la notificación
                                                        SQL = Strings.Replace(SQL, "@whoToNotify", "1")
                                                        bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                                                End Select
                                            End If
                                            Exit For
                                        End If
                                    End If
                                Next
                            Next
                        End If
                    End If
                End If

            Else
                ' Notificación desde pantalla de notificaciones
                ' Visualtime v6.6.10

                Dim notification As New VTNotifications.Notifications.roNotification(ads("ID"), New VTNotifications.Notifications.roNotificationState(-1))
                Dim WhoToNotify As Integer = -1

                Dim notifySupervisor As Boolean = notification.AllowMail
                Dim notifyEmployee As Boolean = notification.Condition.MailListUserfield.Trim <> String.Empty

                If notifyEmployee AndAlso notifySupervisor Then
                    WhoToNotify = 2
                ElseIf notifyEmployee Then
                    WhoToNotify = 0
                ElseIf notifySupervisor Then
                    WhoToNotify = 1
                End If

                If notification.Condition.IDShifts.ToList.Any AndAlso WhoToNotify >= 0 Then
                    ' Limitamos los empleados que han fichado a los seleccionados en la notificaión
                    Dim selector As New roUniversalSelector
                    Dim selectorManager As New Base.VTSelectorManager.roSelectorManager
                    Dim selectorFilter As New roSelectorFilter
                    selectorFilter = roJSONHelper.Deserialize(Of roSelectorFilter)(notification.Condition.EmployeeFilter)
                    selector = selectorManager.GetEmployeesFromSelectorFilter(selectorFilter, False, False, True)

                    ' Y a los horarios de la notificación
                    Dim selectedShifts As String = String.Join(",", notification.Condition.IDShifts)

                    Dim applyFilter As Boolean = False
                    applyFilter = selectorFilter.ComposeMode.ToUpper <> "ALL" AndAlso selectorFilter.ComposeFilter.Length > 0
                    'Obtenemos todos aquellos employees que tengan fichajes de entrada de hoy con un shift asignado
                    Dim joinClause As String = If(applyFilter, $"INNER JOIN {selector.TemporalTableName} TMP ON TMP.IDEmployee = p.IDEmployee", "")
                    Dim sqlcommand As String = $"@SELECT# distinct(p.IDEmployee) 
                                                 FROM Punches P 
                                                 INNER JOIN DailySchedule ds on p.IDEmployee = ds.IDEmployee AND ds.IDShift1 IN ({selectedShifts})
                                                 {joinClause}
                                                 WHERE p.ActualType = 1 
                                                 AND p.ShiftDate= {roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime} 
                                                 AND ds.Date= {roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime}"

                    Dim tabletemp As DataTable = AccessHelper.CreateDataTable(sqlcommand)

                    If tabletemp IsNot Nothing AndAlso tabletemp.Rows.Count > 0 Then
                        ' Para cada empleado
                        strEmployeesIds = String.Join(",", tabletemp.AsEnumerable().Select(Function(dr) $"B{dr("IDEmployee")}"))

                        If strEmployeesIds.Length > 0 Then
                            Dim oCalendarState As VTCalendar.roCalendarState = New VTCalendar.roCalendarState(-1)
                            Dim calendarManager As VTCalendar.roCalendarManager = New VTCalendar.roCalendarManager(oCalendarState)
                            Dim calendar As roCalendar = calendarManager.Load(DateTime.Now.Date, DateTime.Now.Date, strEmployeesIds, DTOs.CalendarView.Planification, CalendarDetailLevel.Daily, False,,,,, True)
                            If calendar IsNot Nothing AndAlso calendar.CalendarData IsNot Nothing Then
                                For Each calendarRow As roCalendarRow In calendar.CalendarData
                                    For Each dayData As roCalendarRowDayData In calendarRow.PeriodData.DayData
                                        tZoneInfo = Nothing
                                        If dayData IsNot Nothing AndAlso dayData.PunchData IsNot Nothing AndAlso dayData.PunchData.Length > 0 AndAlso dayData.ShiftUsed IsNot Nothing Then

                                            ' Obtenemos las previsiones de horas de exceso de hoy
                                            Dim lstProgrammedOvertimes As New Generic.List(Of roProgrammedOvertime)
                                            Dim bState = New roProgrammedOvertimeState(-1)
                                            Dim oProgrammedOvertimeManager As New roProgrammedOvertimeManager(bState)

                                            lstProgrammedOvertimes = oProgrammedOvertimeManager.GetProgrammedOvertimes(calendarRow.EmployeeData.IDEmployee, bState, "BeginDate =" & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime)

                                            Dim minutesBefore As Integer = notification.Condition.PunchBeforeStartTimeLimit.Hour * 60 + notification.Condition.PunchBeforeStartTimeLimit.Minute
                                            Dim horaInicioJornada As DateTime = dayData.ShiftUsed.StartHour.AddMinutes(minutesBefore * -1)
                                            Dim inicioJornada As Date = Now.Date.Add(horaInicioJornada.TimeOfDay)
                                            inicioJornada = roNotificationHelper.GetServerTime(calendarRow.EmployeeData.IDEmployee, inicioJornada, tZoneInfo)

                                            Dim esHorasExceso As Boolean = False
                                            For Each horaExceso As roProgrammedOvertime In lstProgrammedOvertimes
                                                Dim horaExcesoBegin As Date = horaExceso.ProgrammedBeginDate.Date.Add(horaExceso.BeginTime.TimeOfDay)
                                                Dim horaExcesoFinish As Date = horaExceso.ProgrammedEndDate.Date.Add(horaExceso.EndTime.TimeOfDay)
                                                If horaExcesoBegin <= inicioJornada AndAlso horaExcesoFinish >= inicioJornada Then
                                                    esHorasExceso = True
                                                    Exit For
                                                End If
                                            Next

                                            'Obtenemos el punch antes del inicio de jornada
                                            Dim punches As List(Of roCalendarRowPunchData) = dayData.PunchData.Where(Function(x) x.DateTimePunch < inicioJornada).OrderBy(Function(x) x.DateTimePunch).ToList()
                                            Dim entryPunch As roCalendarRowPunchData = Nothing
                                            If punches IsNot Nothing AndAlso punches.Count > 0 Then
                                                entryPunch = punches(punches.Count - 1)
                                            End If

                                            'Si no hay horas de exceso y existe un punch de entrada antes del inicio de la jornada con el tiempo aplicado
                                            If Not esHorasExceso AndAlso entryPunch IsNot Nothing AndAlso entryPunch.ActualType = CalendarPunchTypeEnum._IN Then
                                                'Comprobar que no existe la notificación
                                                SQL = "@SELECT# count(id) FROM sysroNotificationTasks WHERE IDNotification=@idNotification AND Key1Numeric=@idEmployee AND Key3DateTime=@punchDate"
                                                SQL = Strings.Replace(SQL, "@idNotification", roTypes.Any2String(ads("ID")))
                                                SQL = Strings.Replace(SQL, "@idEmployee", roTypes.Any2String(calendarRow.EmployeeData.IDEmployee))
                                                SQL = Strings.Replace(SQL, "@punchDate", roTypes.Any2Time(entryPunch.DateTimePunch).SQLDateTime)

                                                Dim notificationNoExist As Boolean = AccessHelper.ExecuteScalar(SQL) = 0

                                                If notificationNoExist Then
                                                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime) VALUES (@idNotification, @idEmployee, @whoToNotify, @punchDate)"
                                                    SQL = Strings.Replace(SQL, "@idNotification", roTypes.Any2String(ads("ID")))
                                                    SQL = Strings.Replace(SQL, "@idEmployee", roTypes.Any2String(calendarRow.EmployeeData.IDEmployee))
                                                    SQL = Strings.Replace(SQL, "@punchDate", roTypes.Any2Time(entryPunch.DateTimePunch).SQLDateTime)
                                                    Select Case WhoToNotify
                                                        Case 0, 1 'A supervisor o a usuario
                                                            'Crear la notificación
                                                            SQL = Strings.Replace(SQL, "@whoToNotify", roTypes.Any2String(WhoToNotify))
                                                            bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                                                        Case 2 'se envia a ambos
                                                            'Crear la notificación
                                                            Dim SQL0 = SQL
                                                            SQL0 = Strings.Replace(SQL0, "@whoToNotify", "0")
                                                            bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL0)

                                                            'Crear la notificación
                                                            SQL = Strings.Replace(SQL, "@whoToNotify", "1")
                                                            bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                                                    End Select
                                                End If
                                                Exit For
                                            End If
                                        End If
                                    Next
                                Next
                            End If
                        End If
                    End If

                End If
            End If

        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roPunchBeforeStart::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class