Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class roNotificationTask_Employee_Without_Exit_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Employee_Without_Exit, sGUID)
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
        Return GetDefaultDestinationConfig(True, True, True, True)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Employees WHERE ID = " & _oNotificationTask.Key1Numeric)),
                Format$(_oNotificationTask.key3Datetime, "HH:mm")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.Employee_Without_Exit.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.Employee_Without_Exit.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEmployeeWithoutExit::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Aviso al empleado que sigue trabajando sin autorizaci�n
        '
        Dim SQL As String = ""
        Dim mCondition As New roCollection
        Dim bRet As Boolean = True

        Try
            Dim isJune6610 As Boolean = VTBase.roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "June6610"))
            If Not isJune6610 OrElse roTypes.Any2String(ads("CreatorID")).ToUpper = "SYSTEM" Then

                Dim xDate As DateTime

                Dim oLicSupport As New roServerLicense()
                Dim dailyRecordInstalled As Boolean = oLicSupport.FeatureIsInstalled("Feature\DailyRecord")

                If dailyRecordInstalled Then
                    SQL = "@SELECT# DailySchedule.IDEmployee, IDSHift1, Date, isnull(NotifyEmployeeExitAt, 0) as  NotifyEmployeeExitAt " &
                 "FROM DailySchedule with (nolock) " &
                 "INNER JOIN Shifts with (nolock) On DailySchedule.IDShift1 = Shifts.ID " &
                 "INNER JOIN sysroPassports On sysroPassports.IDEmployee = DailySchedule.IDEmployee And sysroPassports.GroupType  ='E' " &
                 "LEFT JOIN sysrovwSecurity_PermissionOverFeatures POF ON POF.IDPassport = sysroPassports.ID " &
                                                   "AND POF.IDFeature = 20002  " &
                 "WHERE Date = " & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime & " AND isnull(EnableNotifyExit,0) = 1  " &
                 "AND ISNULL(POF.Permission,0) = 0"
                Else
                    SQL = "@SELECT# IDEmployee, IDSHift1, Date, isnull(NotifyEmployeeExitAt, 0) as  NotifyEmployeeExitAt FROM DailySchedule with (nolock) INNER JOIN Shifts with (nolock) On DailySchedule.IDShift1 = Shifts.ID WHERE Date = " & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime & " and isnull(EnableNotifyExit,0) = 1  "
                End If

                ' Obtenemos todos los empleados que HOY tienen asignado un horario que debe generar notificacion a una hora determinada

                SQL = SQL & " AND NOT EXISTS " &
                  "(@SELECT# 1 " &
                      " From sysroNotificationTasks with (nolock) " &
                      " Where sysroNotificationTasks.Key1Numeric = DailySchedule.IDEmployee " &
                      " AND sysroNotificationTasks.Key4DateTime =  DailySchedule.Date " &
                      " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

                Dim tZoneInfo As TimeZoneInfo = Nothing
                Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    ' Para cada empleado
                    For Each dr As DataRow In dt.Rows
                        tZoneInfo = Nothing
                        ' Obtenemos la hora a la que se debe notificar el empleado
                        xDate = roTypes.Any2DateTime(dr("Date")).AddMinutes(roTypes.Any2Integer(dr("NotifyEmployeeExitAt")))
                        xDate = roNotificationHelper.GetServerTime(roTypes.Any2Integer(dr("IDEmployee")), xDate, tZoneInfo)

                        Dim notificationDate As DateTime = roTypes.Any2DateTime(dr("Date")).AddMinutes(roTypes.Any2Integer(dr("NotifyEmployeeExitAt")))

                        ' Obtenemos el ultimo fichaje del dia
                        SQL = "@SELECT# TOP 1 isnull(ActualType,0) as ActualType, isnull(DateTime, getdate()) as DateTime FROM Punches with (nolock) WHERE IDEmployee= " & dr("IDEmployee") & " AND ShiftDate = " & roTypes.Any2Time(dr("Date")).SQLSmallDateTime & " AND ActualType IN(1,2)  Order by Datetime desc"
                        Dim dtPunches As DataTable = AccessHelper.CreateDataTable(SQL)
                        If dtPunches IsNot Nothing AndAlso dtPunches.Rows.Count > 0 Then
                            ' Para cada empleado
                            Dim oPunch As DataRow = dtPunches.Rows(0)
                            ' En el caso que sea una entrada,
                            ' si la hora del fichaje es anterior a la hora autorizada
                            ' y la hora actual es posterior, generamos la notificacion
                            If roTypes.Any2Integer(oPunch("ActualType")) = 1 AndAlso roTypes.Any2DateTime(oPunch("DateTime")).Ticks <= xDate.Ticks AndAlso xDate.Ticks <= DateTime.Now.Ticks Then
                                ' Si el ultimo fichaje es una entrada y la hora actual es => a la fecha de la notificacion , la generamos en caso que no lo hagamos hecho ya
                                SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key4DateTime, Key3DateTime) VALUES (" &
                                                    roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("Date")).SQLDateTime & "," & roTypes.Any2Time(notificationDate).SQLDateTime & ")"
                                bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                            End If
                        End If
                    Next
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
                If WhoToNotify >= 0 Then
                    ' Limitamos los empleados que han fichado a los seleccionados en la notificaión
                    Dim selector As New roUniversalSelector
                    Dim selectorManager As New Base.VTSelectorManager.roSelectorManager
                    Dim selectorFilter As New roSelectorFilter
                    selectorFilter = roJSONHelper.Deserialize(Of roSelectorFilter)(notification.Condition.EmployeeFilter)
                    selector = selectorManager.GetEmployeesFromSelectorFilter(selectorFilter, False, False, True)

                    Dim applyFilter As Boolean = False
                    applyFilter = selectorFilter.ComposeMode.ToUpper <> "ALL" AndAlso selectorFilter.ComposeFilter.Length > 0
                    'Obtenemos todos aquellos employees que tengan fichajes de entrada de hoy con un shift asignado
                    Dim joinClause As String = If(applyFilter, $"INNER JOIN {selector.TemporalTableName} TMP ON TMP.IDEmployee = DailySchedule.IDEmployee", "")
                    Dim xDate As DateTime

                    Dim oLicSupport As New roServerLicense()
                    Dim dailyRecordInstalled As Boolean = oLicSupport.FeatureIsInstalled("Feature\DailyRecord")

                    If dailyRecordInstalled Then
                        SQL = $"@SELECT# DailySchedule.IDEmployee, IDSHift1, Date, isnull(NotifyEmployeeExitAt, 0) as  NotifyEmployeeExitAt 
                 FROM DailySchedule with (nolock) {joinClause}
                 INNER JOIN Shifts with (nolock) On DailySchedule.IDShift1 = Shifts.ID 
                 INNER JOIN sysroPassports On sysroPassports.IDEmployee = DailySchedule.IDEmployee And sysroPassports.GroupType  ='E' 
                 LEFT JOIN sysrovwSecurity_PermissionOverFeatures POF ON POF.IDPassport = sysroPassports.ID 
                                                   AND POF.IDFeature = 20002  
                 WHERE Date = " & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime & " AND isnull(EnableNotifyExit,0) = 1  
                 AND ISNULL(POF.Permission,0) = 0"
                    Else
                        SQL = $"@SELECT# IDEmployee, IDSHift1, Date, isnull(NotifyEmployeeExitAt, 0) as  NotifyEmployeeExitAt FROM DailySchedule with (nolock)
                            {joinClause} 
                            INNER JOIN Shifts with (nolock) On DailySchedule.IDShift1 = Shifts.ID WHERE Date = " & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime & " and isnull(EnableNotifyExit,0) = 1  "
                    End If

                    ' Obtenemos todos los empleados que HOY tienen asignado un horario que debe generar notificacion a una hora determinada

                    SQL = SQL & " AND NOT EXISTS " &
                  "(@SELECT# 1 " &
                      " From sysroNotificationTasks with (nolock) " &
                      " Where sysroNotificationTasks.Key1Numeric = DailySchedule.IDEmployee " &
                      " AND sysroNotificationTasks.Key4DateTime =  DailySchedule.Date " &
                      " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

                    Dim tZoneInfo As TimeZoneInfo = Nothing
                    Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)

                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                        ' Para cada empleado
                        For Each dr As DataRow In dt.Rows
                            tZoneInfo = Nothing
                            ' Obtenemos la hora a la que se debe notificar el empleado
                            xDate = roTypes.Any2DateTime(dr("Date")).AddMinutes(roTypes.Any2Integer(dr("NotifyEmployeeExitAt")))
                            xDate = roNotificationHelper.GetServerTime(roTypes.Any2Integer(dr("IDEmployee")), xDate, tZoneInfo)

                            Dim notificationDate As DateTime = roTypes.Any2DateTime(dr("Date")).AddMinutes(roTypes.Any2Integer(dr("NotifyEmployeeExitAt")))

                            ' Obtenemos el ultimo fichaje del dia
                            SQL = "@SELECT# TOP 1 isnull(ActualType,0) as ActualType, isnull(DateTime, getdate()) as DateTime FROM Punches with (nolock) WHERE IDEmployee= " & dr("IDEmployee") & " AND ShiftDate = " & roTypes.Any2Time(dr("Date")).SQLSmallDateTime & " AND ActualType IN(1,2)  Order by Datetime desc"
                            Dim dtPunches As DataTable = AccessHelper.CreateDataTable(SQL)
                            If dtPunches IsNot Nothing AndAlso dtPunches.Rows.Count > 0 Then
                                ' Para cada empleado
                                Dim oPunch As DataRow = dtPunches.Rows(0)
                                ' En el caso que sea una entrada,
                                ' si la hora del fichaje es anterior a la hora autorizada
                                ' y la hora actual es posterior, generamos la notificacion
                                If roTypes.Any2Integer(oPunch("ActualType")) = 1 AndAlso roTypes.Any2DateTime(oPunch("DateTime")).Ticks <= xDate.Ticks AndAlso xDate.Ticks <= DateTime.Now.Ticks Then
                                    ' Si el ultimo fichaje es una entrada y la hora actual es => a la fecha de la notificacion , la generamos en caso que no lo hagamos hecho ya
                                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric, Key4DateTime, Key3DateTime) VALUES (@idNotification, @idEmployee, @whoToNotify, @date, @notificationDate)"
                                    SQL = Strings.Replace(SQL, "@idNotification", roTypes.Any2String(ads("ID")))
                                    SQL = Strings.Replace(SQL, "@idEmployee", roTypes.Any2String(dr("IDEmployee")))
                                    SQL = Strings.Replace(SQL, "@date", roTypes.Any2Time(dr("Date")).SQLDateTime)
                                    SQL = Strings.Replace(SQL, "@notificationDate", roTypes.Any2Time(notificationDate).SQLDateTime)
                                    Select Case WhoToNotify
                                        Case 0, 1 'A supervisor o a usuario
                                            'Crear la notificación
                                            SQL = Strings.Replace(SQL, "@whoToNotify", roTypes.Any2String(WhoToNotify))
                                            bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                                    End Select
                                End If
                            End If
                        Next
                    End If
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEmployeeWithoutExit::GenrateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class