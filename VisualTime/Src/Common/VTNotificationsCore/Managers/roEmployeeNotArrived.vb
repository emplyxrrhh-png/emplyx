Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class roNotificationTask_EmployeeNotArrivedBeforeStartLimit_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.EmployeeNotArrivedBeforeStartLimit, sGUID)
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

        ' Notificacion de sistema
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
                    oItem.Subject = roNotificationHelper.Message("Notification.EmployeeNotArrivedBeforeStartLimit.Subject", Nothing, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.EmployeeNotArrivedBeforeStartLimit.Body", Params, , oConf.Language)
                Case 1 'Supervisor
                    oItem.Subject = roNotificationHelper.Message("Notification.EmployeeNotArrivedBeforeStartLimitForSupervisor.Subject", Params, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.EmployeeNotArrivedBeforeStartLimitForSupervisor.Body", Params, , oConf.Language)
            End Select

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEmployeeNotArrived::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Empleados no han fichado antes de la hora límite de entrada
        '
        Dim SQL As String
        Dim StartLimit As New roTime
        Dim LastPunchTime As New roTime
        Dim NotifyEmployeeAfterAt As String
        Dim WhoToNotify As Integer
        Dim ExistAfterPunch As Boolean
        Dim ShouldNotifyExitBeforeTime As Boolean
        Dim sFiredDate As String
        Dim bRet As Boolean = True
        Dim oParams As New roParameters("OPTIONS")

        Try

            Dim notification As New VTNotifications.Notifications.roNotification(ads("ID"), New VTNotifications.Notifications.roNotificationState(-1))

            Dim oLicSupport As New roServerLicense()
            Dim dailyRecordInstalled As Boolean = oLicSupport.FeatureIsInstalled("Feature\DailyRecord")

            Dim SQLFilter As String = ""
            Dim isJune6610 As Boolean = VTBase.roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "June6610"))
            If isJune6610 AndAlso roTypes.Any2String(ads("CreatorID")).ToUpper <> "SYSTEM" Then
                ' Visualtime v6.6.10
                ' Limitamos los empleados a los seleccionados en la notificaión
                Dim selector As New roUniversalSelector
                Dim selectorManager As New Base.VTSelectorManager.roSelectorManager
                Dim selectorFilter As New roSelectorFilter
                selectorFilter = roJSONHelper.Deserialize(Of roSelectorFilter)(notification.Condition.EmployeeFilter)
                selector = selectorManager.GetEmployeesFromSelectorFilter(selectorFilter, False, False, True)
                Dim applyFilter As Boolean = False
                applyFilter = selectorFilter.ComposeMode.ToUpper <> "ALL" AndAlso selectorFilter.ComposeFilter.Length > 0
                'Obtenemos todos aquellos employees que tengan fichajes de entrada de hoy con un shift asignado
                If applyFilter Then
                    SQLFilter = "INNER JOIN " & selector.TemporalTableName & " TMP ON TMP.IDEmployee = EmployeeStatus.IDEmployee "
                End If
            End If


            If dailyRecordInstalled Then
                SQL = "@SELECT#  EmployeeStatus.IDEmployee, BeginMandatory, StartLimit, ShiftDate, LastPunch, Type, DailySchedule.IDShiftUsed, DailySchedule.IsHolidays "
                SQL = SQL & "FROM EmployeeStatus  "
                SQL = SQL & SQLFilter
                SQL = SQL & "INNER JOIN sysroPassports ON sysroPassports.IDEmployee = EmployeeStatus.IDEmployee "
                SQL = SQL & "LEFT JOIN sysrovwSecurity_PermissionOverFeatures POF ON POF.IDPassport = sysroPassports.ID AND POF.IDFeature = 20002 "
                SQL = SQL & "LEFT JOIN DailySchedule ON DailySchedule.IDEmployee = EmployeeStatus.IDEmployee AND CONVERT(DATE,StartLimit) = DailySchedule.Date "
                SQL = SQL & "WHERE IsPresent=0 AND StartLimit<" & roTypes.Any2Time(Now).SQLSmallDateTime & " AND StartLimit >= " & roTypes.Any2Time(roTypes.Any2Time(Now).DateOnly).SQLSmallDateTime
                SQL = SQL & " AND DailySchedule.Status > 40 "
                SQL = SQL & " AND ISNULL(POF.Permission,0) = 0"
            Else
                SQL = "@SELECT#  EmployeeStatus.IDEmployee, BeginMandatory, StartLimit, ShiftDate, LastPunch, Type, DailySchedule.IDShiftUsed, DailySchedule.IsHolidays "
                SQL = SQL & "FROM EmployeeStatus  "
                SQL = SQL & SQLFilter
                SQL = SQL & "LEFT JOIN DailySchedule ON DailySchedule.IDEmployee = EmployeeStatus.IDEmployee AND CONVERT(DATE,StartLimit) = DailySchedule.Date "
                SQL = SQL & "WHERE IsPresent=0 AND StartLimit<" & roTypes.Any2Time(Now).SQLSmallDateTime & " AND StartLimit >= " & roTypes.Any2Time(roTypes.Any2Time(Now).DateOnly).SQLSmallDateTime
                SQL = SQL & " AND DailySchedule.Status > 40 "
            End If

            ' Verificamos si existen empleados ausentes que deberian estar presentes
            ' y aun no tiene alerta creada
            ' y no estan en ausencia prolongada

            'Solo a�adimos la comprovaci�n del �ltimo fichaje si no se tiene que notificar salidas anticipadas
            If Not ShouldNotifyExitBeforeTime Then
                SQL = SQL & " AND (LastPunch IS NULL OR LastPunch < StartLimit OR UPPER(Type) <> 'S') "
            End If

            SQL = SQL & " AND NOT EXISTS "
            SQL = SQL & "(@SELECT# * "
            SQL = SQL & " From sysroNotificationTasks "
            SQL = SQL & " Where sysroNotificationTasks.Key1Numeric = EmployeeStatus.IDEmployee "
            SQL = SQL & " AND EmployeeStatus.StartLimit  =  sysroNotificationTasks.Key3Datetime "
            SQL = SQL & " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "
            SQL = SQL & " ORDER BY IDEmployee "

            ' Obtenemos todos los horarios
            Dim dtShifts As DataTable = AccessHelper.CreateDataTable("@SELECT# ID, EnableNotifyAfter, NotifyEmployeeAfterAt, WhoToNotifyAfter FROM Shifts")

            ' Obtenemos todos los empleados que no estan presentes
            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)

            Dim tZoneInfo As TimeZoneInfo = Nothing

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado, creamos una nueva notificacion
                ' en caso necesario
                For Each dr As DataRow In dt.Rows
                    tZoneInfo = Nothing
                    Dim xNow As DateTime = roTypes.UnspecifiedNow()
                    Dim nowOnClientTimezone As roTime = roTypes.Any2Time(roNotificationHelper.GetTimeOnTimezone(dr("IDEmployee"), xNow, tZoneInfo))

                    'Cargamos la planificación del día para saber la hora de cierre del mismo para no generar la alerta si ya ha terminado el día.
                    Dim daydata As roCalendarRowPeriodData = VTCalendar.roCalendarRowPeriodDataManager.LoadCellsByCalendar(nowOnClientTimezone.ValueDateTime.Date, nowOnClientTimezone.ValueDateTime.Date, dr("IDEmployee"), -1, 3, oParams,
                                                                                  CalendarView.Planification, CalendarDetailLevel.Daily, Nothing, Nothing, Nothing,
                                                                                  New VTCalendar.roCalendarRowPeriodDataState(-1), False, bLoadAlerts:=False)

                    If daydata IsNot Nothing AndAlso daydata.DayData.Length > 0 AndAlso daydata.DayData(0).MainShift IsNot Nothing Then
                        Dim dateNow As DateTime = nowOnClientTimezone.ValueDateTime

                        Dim endHour As DateTime = roTypes.CreateDateTime(dateNow.Year, dateNow.Month, dateNow.Day, daydata.DayData(0).MainShift.EndHour.Hour, daydata.DayData(0).MainShift.EndHour.Minute, 0)

                        If daydata.DayData(0).MainShift.EndHour.Day = 29 Then endHour = endHour.AddDays(-1)
                        If daydata.DayData(0).MainShift.EndHour.Day = 31 Then endHour = endHour.AddDays(1)

                        If endHour < dateNow Then
                            Continue For
                        End If
                    End If


                    ' Si el empleado esta en ausencia prolongada no creamos notificacion
                    Dim IsProgrammedAbsence As Double = roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# DISTINCT IDEmployee FROM ProgrammedAbsences WHERE " &
                                                            " IDEmployee=" & dr("IDEmployee") &
                                                            " AND BeginDate<=" & roTypes.Any2Time(roTypes.Any2Time(dr("StartLimit")).DateOnly).SQLSmallDateTime &
                                                            " AND (ISNULL(FinishDate,dateadd(day, MaxLastingDays-1,BeginDate)) >=" & roTypes.Any2Time(roTypes.Any2Time(dr("StartLimit")).DateOnly).SQLSmallDateTime &
                                                            ")"))

                    Dim IsProgrammedHolidays As Double = roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# distinct idemployee from ProgrammedHolidays where IDEmployee = " & dr("IDEmployee") &
                                                        " and " & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime & "= date "))

                    If IsProgrammedAbsence = 0 AndAlso roTypes.Any2Double(dr("IsHolidays")) = 0 AndAlso IsProgrammedHolidays = 0 Then
                        ' Obtenemos el margen de cortesia de la franja mas proxima

                        Dim idShiftUsed = roTypes.Any2Integer(dr("IDShiftUsed"))

                        'Obtenemos los datos del horario
                        NotifyEmployeeAfterAt = GetNotifyEmployeeAfterAt(WhoToNotify, idShiftUsed, dtShifts, ads, notification)

                        If NotifyEmployeeAfterAt <> "NOSHIFT" Then

                            'Dim timeNotifyEmployeeAfterAt As DateTime = New DateTime(1, 1, 1, 0, rotypes.Any2Integer(NotifyEmployeeAfterAt), 0)

                            StartLimit = roTypes.Any2Time(dr("StartLimit"))
                            If Not IsNothing(NotifyEmployeeAfterAt) Then
                                StartLimit = StartLimit.Add(roTypes.Any2Integer(NotifyEmployeeAfterAt), "n")
                            End If

                            Dim xStartLimit As DateTime = StartLimit.Value
                            'No hace falta localizar la hora inicio de la tabla employeestatus ya que el eog la pone en hora con la tarea multitimezone.
                            'xStartLimit = roNotificationHelper.GetServerTime(roTypes.Any2Integer(dr("IDEmployee")), xStartLimit, tZoneInfo)

                            ' Si el empleado ha fichado una salida el mismo dia posterior al inicio obligado,
                            ' es una salida anticipada, no un retraso
                            ExistAfterPunch = False
                            If IsDate(dr("LastPunch")) And roTypes.Any2String(dr("Type")) = "S" And ShouldNotifyExitBeforeTime Then
                                'Aplicamos el margen de cortesia si existe al �ltimo fichaje
                                LastPunchTime = roTypes.Any2Time(roTypes.DateTime2Double(roTypes.Any2Time(CDate(dr("LastPunch"))).Value))
                                LastPunchTime = LastPunchTime.Add(roTypes.Any2Integer(NotifyEmployeeAfterAt), "n")

                                If (roTypes.Any2Time(dr("LastPunch")).VBNumericValue > roTypes.Any2Time(dr("StartLimit")).VBNumericValue) Then
                                    ExistAfterPunch = True
                                End If
                            End If
                            If Not ExistAfterPunch Then
                                If xStartLimit < DateTime.Now Then
                                    ' Verificamos si en este momento esta dentro de una prevision de horas
                                    ' de ausencia.
                                    ' en ese caso no generamos la notiifcacion
                                    If Not roNotificationHelper.EmployeeOnProgrammedCauseOnTime(dr("StartLimit"), roTypes.Any2Double(dr("IDEmployee")), nowOnClientTimezone.ValueDateTime) Then
                                        'Notificamos que el empleado llega tarde
                                        If roTypes.Any2Boolean(ads("ShowOnDesktop")) Then
                                            sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                                        Else
                                            sFiredDate = "NULL"
                                        End If
                                        Select Case WhoToNotify
                                            Case 0, 1 'A supervisor o a usuario
                                                SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime, FiredDate, Parameters) VALUES (" &
                                                            roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Integer(WhoToNotify) & "," & roTypes.Any2Time(dr("StartLimit")).SQLDateTime & ", " & sFiredDate & ",'EmployeeNotArrivedBeforeStartLimit')"
                                                bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)

                                            Case 2 'se envia a ambos
                                                SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime, FiredDate, Parameters) VALUES (" &
                                                         roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & ", 0," & roTypes.Any2Time(dr("StartLimit")).SQLDateTime & ", " & sFiredDate & ",'EmployeeNotArrivedBeforeStartLimit')"
                                                bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)

                                                SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime, FiredDate, Parameters) VALUES (" &
                                                          roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & ",1," & roTypes.Any2Time(dr("StartLimit")).SQLDateTime & ", " & sFiredDate & ",'EmployeeNotArrivedBeforeStartLimit')"
                                                bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                                        End Select
                                    End If

                                End If
                            Else
                                If (LastPunchTime.VBNumericValue < roTypes.Any2Time(Now).VBNumericValue) And (roTypes.Any2Time(dr("StartLimit")).Value <> roTypes.Any2Time(roNotificationHelper.roNullDate).Value) Then
                                    'Debemos notificar la salida antes del horario
                                    If roTypes.Any2Boolean(ads("ShowOnDesktop")) Then
                                        sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                                    Else
                                        sFiredDate = "NULL"
                                    End If

                                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, FiredDate, Parameters) VALUES (" &
                                         roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("StartLimit")).SQLDateTime & ", " & sFiredDate & ",'EarlyExit')"
                                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)

                                End If
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEmployeeNotArrived::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet

    End Function

    Private Function GetNotifyEmployeeAfterAt(ByRef whoToNotify As Integer, ByVal IdShiftUsed As Integer, ByVal dtShifts As DataTable, ByVal ads As DataRow, ByVal notification As VTNotifications.Notifications.roNotification) As String
        Dim NotifyEmployeeAfterAt As String = "NOSHIFT"

        Try
            Dim CreatorID As String = IIf(IsDBNull(ads("CreatorID")), "", roTypes.Any2String(ads("CreatorID")).ToUpper)
            Dim isJune6610 As Boolean = VTBase.roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "June6610"))

            If Not isJune6610 OrElse CreatorID = "SYSTEM" Then
                ' En el caso que la notificacion sea de sistema, los parametros de la notificación están definidos en el horario 
                Dim shiftUsed = dtShifts.Select("ID = " + roTypes.Any2String(IdShiftUsed))

                If shiftUsed.Count > 0 AndAlso IdShiftUsed <> 0 AndAlso roTypes.Any2Boolean(shiftUsed(0).Item("EnableNotifyAfter")) Then
                    ' Obtenemos cuando hay que notificar al usuario
                    NotifyEmployeeAfterAt = roTypes.Any2Double(shiftUsed(0).Item("NotifyEmployeeAfterAt"))
                    whoToNotify = roTypes.Any2Double(shiftUsed(0).Item("WhoToNotifyAfter"))
                End If
            Else
                ' Visualtime v6.6.10
                ' de lo contrario, los parámetros de la notificación están definidos en la propia notificación
                whoToNotify = -1

                Dim notifySupervisor As Boolean = notification.AllowMail
                Dim notifyEmployee As Boolean = notification.Condition.MailListUserfield.Trim <> String.Empty

                If notifyEmployee AndAlso notifySupervisor Then
                    whoToNotify = 2
                ElseIf notifyEmployee Then
                    whoToNotify = 0
                ElseIf notifySupervisor Then
                    whoToNotify = 1
                End If

                If whoToNotify = -1 Then
                    Return NotifyEmployeeAfterAt
                End If

                If notification.Condition.IDShifts.ToList.Any AndAlso whoToNotify >= 0 Then
                    ' Si el horario no esta dentro de la definicion de la notificación, no aplica
                    If Not notification.Condition.IDShifts.Contains(IdShiftUsed) Then
                        Return NotifyEmployeeAfterAt
                    End If
                End If

                ' Tolerancia
                NotifyEmployeeAfterAt = (notification.Condition.PunchToleranceTime.Hour * 60) + notification.Condition.PunchToleranceTime.Minute


            End If
        Catch ex As Exception
            NotifyEmployeeAfterAt = ""
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEmployeeNotArrived::GetNotifyEmployeeAfterAt: Unexpected error: ", ex)
        End Try

        Return NotifyEmployeeAfterAt

    End Function

End Class