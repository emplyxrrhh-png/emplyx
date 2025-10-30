Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class roNotificationTask_Employee_LateArrival_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Employee_Not_Arrived_or_Late, sGUID)
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
        Return Me.GetDefaultDestinationConfig()
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric))
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()

            If _oNotificationTask.Parameters = String.Empty OrElse _oNotificationTask.Parameters = "LateArrival" Then
                oItem.Subject = roNotificationHelper.Message("Notification.EmployeeLater.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.EmployeeLater.Body", Params, , oConf.Language)
            ElseIf _oNotificationTask.Parameters = "EarlyExit" Then
                oItem.Subject = roNotificationHelper.Message("Notification.EmployeeEarlyExit.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.EmployeeEarlyExit.Body", Params, , oConf.Language)
            End If

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEmployeeLateArrival::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Empleados que se han retrasado
        '
        Dim SQL As String
        Dim BeginMandatory As New roTime
        Dim LastPunchTime As New roTime
        Dim FilterTime As String
        Dim ExistAfterPunch As Boolean
        Dim LateArrivalTime As String
        Dim ShouldNotifyExitBeforeTime As Boolean
        Dim sFiredDate As String
        Dim bRet As Boolean = True
        Dim oParams As New roParameters("OPTIONS")

        Try
            ' Obtenemos el tiempo de cortesia para los retrasos
            LateArrivalTime = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VisualTime.Procees.Notifier.LateArrivalTime"))

            If Not IsDate(LateArrivalTime) OrElse Len(LateArrivalTime) <> 5 Then
                ' Por defecto 0 minutos
                LateArrivalTime = "00:00"
            End If

            ShouldNotifyExitBeforeTime = (roTypes.Any2Double(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "ShouldNotifyExitBeforeTime")) = 1)

            ' Verificamos si existen empleados ausentes que deberian estar presentes
            ' y aun no tiene alerta creada
            ' y no estan en ausencia prolongada
            ' y no fichan mediante la declaración de jornada (idfeature 20002)

            Dim oLicSupport As New roServerLicense()
            Dim dailyRecordInstalled As Boolean = oLicSupport.FeatureIsInstalled("Feature\DailyRecord")

            If dailyRecordInstalled Then
                SQL = "@SELECT#  EmployeeStatus.IDEmployee, BeginMandatory, ShiftDate, LastPunch, Type,  ISNULL(POF.Permission,0) "
                SQL = SQL & "FROM EmployeeStatus  "
                SQL = SQL & "INNER JOIN sysroPassports ON sysroPassports.IDEmployee = EmployeeStatus.IDEmployee "
                SQL = SQL & "LEFT JOIN DailySchedule ON DailySchedule.IDEmployee = EmployeeStatus.IDEmployee AND CONVERT(DATE,BeginMandatory) = DailySchedule.Date "
                SQL = SQL & "LEFT JOIN sysrovwSecurity_PermissionOverFeatures POF ON POF.IDPassport = sysroPassports.ID AND POF.IDFeature = 20002 "
                SQL = SQL & "WHERE IsPresent=0 AND BeginMandatory<" & roTypes.Any2Time(Now).SQLSmallDateTime & " AND BeginMandatory >= " & roTypes.Any2Time(roTypes.Any2Time(Now).DateOnly).SQLSmallDateTime
                SQL = SQL & " AND DailySchedule.Status > 50 "
                SQL = SQL & " AND ISNULL(POF.Permission,0) = 0"
            Else
                SQL = "@SELECT#  EmployeeStatus.IDEmployee, BeginMandatory, ShiftDate, LastPunch, Type "
                SQL = SQL & "FROM EmployeeStatus  "
                SQL = SQL & "LEFT JOIN DailySchedule ON DailySchedule.IDEmployee = EmployeeStatus.IDEmployee AND CONVERT(DATE,BeginMandatory) = DailySchedule.Date "
                SQL = SQL & "WHERE IsPresent=0 AND BeginMandatory<" & roTypes.Any2Time(Now).SQLSmallDateTime & " AND BeginMandatory >= " & roTypes.Any2Time(roTypes.Any2Time(Now).DateOnly).SQLSmallDateTime
                SQL = SQL & " AND DailySchedule.Status > 50 "
            End If

            'Solo añadimos la comprobación del último fichaje si no se tiene que notificar salidas anticipadas
            If Not ShouldNotifyExitBeforeTime Then
                SQL = SQL & " AND (LastPunch IS NULL OR LastPunch < BeginMandatory OR UPPER(Type) <> 'S') "
            End If

            SQL = SQL & " AND NOT EXISTS "
            SQL = SQL & "(@SELECT# * "
            SQL = SQL & " From sysroNotificationTasks "
            SQL = SQL & " Where sysroNotificationTasks.Key1Numeric = EmployeeStatus.IDEmployee "
            SQL = SQL & " AND EmployeeStatus.BeginMandatory  =  sysroNotificationTasks.Key3Datetime "
            SQL = SQL & " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "
            SQL = SQL & " ORDER BY IDEmployee "

            ' Obtenemos todos los empleados que no estan presentes
            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado, creamos una nueva notificacion
                ' en caso necesario
                For Each dr As DataRow In dt.Rows
                    Dim xNow As DateTime = roTypes.UnspecifiedNow()
                    Dim tZoneInfo As TimeZoneInfo = Nothing
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
                    Dim IsProgrammedAbsence As Double
                    IsProgrammedAbsence = roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# DISTINCT IDEmployee FROM ProgrammedAbsences WHERE " &
                                                            " IDEmployee=" & dr("IDEmployee") &
                                                            " AND BeginDate<=" & roTypes.Any2Time(roTypes.Any2Time(dr("BeginMandatory")).DateOnly).SQLSmallDateTime &
                                                            " AND (ISNULL(FinishDate,dateadd(day, MaxLastingDays-1,BeginDate)) >=" & roTypes.Any2Time(roTypes.Any2Time(dr("BeginMandatory")).DateOnly).SQLSmallDateTime() &
                                                            ")"))


                    Dim IsProgrammedAbsenceByHours As Double
                    IsProgrammedAbsenceByHours = roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# DISTINCT IDEmployee FROM sysrovwHoursAbsences WHERE " &
                                                            " IDEmployee= " & dr("IDEmployee") &
                                                            " And cast(Date as date) = cast(" & nowOnClientTimezone.SQLSmallDateTime() & " as date)" &
                                                            " And cast(" & nowOnClientTimezone.SQLSmallDateTime() & " as time) between cast(BeginTime as time) And cast(EndTime as time)"))

                    Dim IsHolidays As Double
                    IsHolidays = roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# IsHolidays FROM DailySchedule WHERE IDEmployee=" & dr("IDEmployee") &
                                                            " AND Date=" & roTypes.Any2Time(roTypes.Any2Time(dr("BeginMandatory")).DateOnly).SQLSmallDateTime))

                    Dim IsProgrammedHolidays As Double
                    IsProgrammedHolidays = roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# distinct idemployee from ProgrammedHolidays where IDEmployee = " & dr("IDEmployee") &
                                                        " and " & roTypes.Any2Time(nowOnClientTimezone.ValueDateTime.Date).SQLSmallDateTime & "= date "))



                    If IsProgrammedAbsence = 0 AndAlso IsHolidays = 0 AndAlso IsProgrammedHolidays = 0 AndAlso IsProgrammedAbsenceByHours = 0 Then
                        ' Hora teórica de obligación (viene de EmployeeStatus)
                        BeginMandatory = roTypes.Any2Time(dr("BeginMandatory"))

                        ' Obtenemos el margen de cortesia de la franja mas próxima, si tratamos con horarios con franjas rígidas ...
                        FilterTime = GetFilterTime(roTypes.Any2Double(dr("IDEmployee")), roTypes.Any2Time(dr("BeginMandatory")))
                        'Se puede ajustar la hora teórica desde la que se espera al empleado.
                        If FilterTime <> "NOSHIFT" AndAlso IsDate(FilterTime) Then
                            BeginMandatory = roTypes.Any2Time(roTypes.Any2DateTime(dr("BeginMandatory")).Add(TimeSpan.Parse(FilterTime)))
                        End If
                        'Si mi horario es totalmente flexible para sumar horas trabajadas
                        Dim IdShift = roTypes.Any2Integer(AccessHelper.ExecuteScalar("@SELECT# IDShiftUsed FROM DailySchedule WHERE IDEmployee= " & dr("IDEmployee") & " AND Date=" & roTypes.Any2Time(roTypes.Any2Time(Now).DateOnly).SQLSmallDateTime))
                        Dim isFlexible = roTypes.Any2Boolean(AccessHelper.ExecuteScalar("@SELECT# CASE " &
                                                                                        " WHEN " &
                                                                                        " SUM(CASE WHEN IDType = 1000 THEN 1 ELSE 0 END) = 1 " &
                                                                                        " AND SUM(CASE WHEN IDType IN (1100, 1200, 1300, 1400) THEN 1 ELSE 0 END) = 0 " &
                                                                                        " THEN 1 " &
                                                                                        " ELSE 0 " &
                                                                                        " END AS Flexible " &
                                                                                        " FROM " &
                                                                                        " sysroShiftsLayers " &
                                                                                        " WHERE " &
                                                                                        " IDShift = " & IdShift))
                        If isFlexible Then
                            Dim beginMandatoryFlexible As String = GetFlexibleMandatoryTime(roTypes.Any2Double(dr("IDEmployee")), IdShift)?.SQLSmallDateTime
                            If Not String.IsNullOrEmpty(beginMandatoryFlexible) Then
                                If beginMandatoryFlexible <> BeginMandatory.SQLSmallDateTime Then
                                    Dim strSQL = "@UPDATE# EmployeeStatus SET BeginMandatory=" & beginMandatoryFlexible & " WHERE IDEmployee=" & dr("IDEmployee")
                                    AccessHelper.ExecuteSqlWithoutTimeOut(strSQL)
                                End If

                                If roTypes.Any2Boolean(ads("ShowOnDesktop")) Then
                                    sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                                Else
                                    sFiredDate = "NULL"
                                End If

                                SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, FiredDate, Parameters) VALUES (" &
                                             roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & beginMandatoryFlexible & ", " & sFiredDate & ",'LateArrival')"

                                bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                            End If
                        Else

                            BeginMandatory = roTypes.Any2Time(Date.FromOADate(CDate(BeginMandatory.Value).ToOADate() + CDate(LateArrivalTime).ToOADate()))

                            ' Si el empleado ha fichado una salida el mismo dia posterior al inicio obligado,
                            ' es una salida anticipada, no un retraso
                            ExistAfterPunch = False
                            If IsDate(dr("LastPunch")) AndAlso roTypes.Any2String(dr("Type")) = "S" AndAlso ShouldNotifyExitBeforeTime Then
                                'Aplicamos el margen de cortesia si existe al �ltimo fichaje
                                LastPunchTime = roTypes.Any2Time(roTypes.DateTime2Double(roTypes.Any2Time(CDate(dr("LastPunch"))).Value) + roTypes.DateTime2Double(roTypes.Any2Time(CDate(LateArrivalTime)).Value))

                                If (roTypes.Any2Time(dr("LastPunch")).VBNumericValue > roTypes.Any2Time(dr("BeginMandatory")).VBNumericValue) Then
                                    ExistAfterPunch = True
                                End If
                            End If

                            If Not ExistAfterPunch Then
                                ' Verificamos si en este momento esta dentro de una prevision de horas de ausencia.
                                ' en ese caso no generamos la notiifcacion
                                If BeginMandatory.Value < roTypes.Any2Time(Now).Value AndAlso Not roNotificationHelper.EmployeeOnProgrammedCauseOnTime(dr("BeginMandatory"), roTypes.Any2Double(dr("IDEmployee")), nowOnClientTimezone.ValueDateTime) Then
                                    'Notificamos que el empleado llega tarde
                                    If roTypes.Any2Boolean(ads("ShowOnDesktop")) Then
                                        sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                                    Else
                                        sFiredDate = "NULL"
                                    End If
                                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, FiredDate, Parameters) VALUES (" &
                                             roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("BeginMandatory")).SQLDateTime & ", " & sFiredDate & ",'LateArrival')"
                                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                                End If
                            Else
                                If (LastPunchTime.VBNumericValue < roTypes.Any2Time(Now).VBNumericValue) And (roTypes.Any2Time(dr("BeginMandatory")).Value <> roTypes.Any2Time(roNotificationHelper.roNullDate).Value) Then
                                    'Debemos notificar la salida antes del horario
                                    If roTypes.Any2Boolean(ads("ShowOnDesktop")) Then
                                        sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                                    Else
                                        sFiredDate = "NULL"
                                    End If

                                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, FiredDate, Parameters) VALUES (" &
                                         roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("BeginMandatory")).SQLDateTime & ", " & sFiredDate & ",'EarlyExit')"
                                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                                End If
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEmployeeLateArrival::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

    Private Function GetFilterTime(ByVal IDEmployee As Double, ByVal BeginMandatory As roTime) As String
        Dim FilterTime As String = ""
        Dim SQL As String
        Dim IDShift As Double

        Try
            Dim BeginPeriod As roTime = roTypes.Any2Time("00:00")

            ' Obtenemos el horario actual
            IDShift = roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# IDShiftUsed FROM DailySchedule WHERE IDEmployee= " & IDEmployee & " AND Date=" & roTypes.Any2Time(roTypes.Any2Time(Now).DateOnly).SQLSmallDateTime))
            If IDShift = 0 Then
                Return "NOSHIFT"
            End If

            ' Verificamos si el horario tiene franjas rigidas
            If roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# COUNT(*) FROM sysroShiftsLayers WHERE IDShift = " & IDShift & " AND IDType = 1100")) = 0 Then
                Return "NOSHIFT"
            End If

            ' Obtenemos los fitros de cortesia de las franja rigidas
            SQL = "@SELECT# * FROM sysroShiftsLayers WHERE IDShift = " & IDShift & " AND IDType = 1400 Order by ID"

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            Dim mDefinition As roCollection
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    mDefinition = New roCollection
                    mDefinition.LoadXMLString(dr("Definition"))
                    If mDefinition.Exists("Value") AndAlso roTypes.Any2Time(mDefinition("Begin")).VBNumericValue > BeginPeriod.VBNumericValue AndAlso
                        BeginMandatory.VBNumericValue >= roTypes.Any2Time(roTypes.DateTime2Double(BeginMandatory.DateOnly) + roTypes.DateTime2Double(roTypes.Any2Time(mDefinition("Begin")).Value)).VBNumericValue Then

                        BeginPeriod = roTypes.Any2Time(mDefinition("Begin"))
                        FilterTime = mDefinition("Value")
                    End If
                Next
            End If

            If FilterTime = "" Then
                ' Obtenemos el filtro del horario general
                SQL = "@SELECT# * FROM sysroShiftsLayers WHERE IDShift = " & IDShift & " AND IDType = 1500"
                dt = AccessHelper.CreateDataTable(SQL)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    mDefinition = New roCollection
                    mDefinition.LoadXMLString(dt.Rows(0)("Definition"))
                    If mDefinition.Exists("Value") Then
                        FilterTime = mDefinition("Value")
                    End If
                End If
            End If
        Catch ex As Exception
            FilterTime = ""
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEmployeeLateArrival::GetFilterTime: Unexpected error: ", ex)
        End Try

        Return FilterTime
    End Function

    Private Function GetFlexibleMandatoryTime(ByVal IDEmployee As Double, ByVal IDShift As Integer) As roTime
        Dim BeginMandatory As roTime = Nothing
        Try
            Dim SQL = "@SELECT# * FROM sysroShiftsLayers WHERE IDShift = " & IDShift & " AND IDType = 1000 Order by ID"
            Dim SqlLayer = "@SELECT# * FROM sysroShiftsLayers WHERE IDShift = " & IDShift & "  AND IDType = 1600"
            Dim hoursWorkedFraction As Double = Convert.ToDouble(AccessHelper.ExecuteScalar("@SELECT# SUM(Value) FROM DailyIncidences WHERE IDEmployee= " & IDEmployee & " And Date =" & roTypes.Any2Time(roTypes.Any2Time(Now).DateOnly).SQLSmallDateTime & " AND IDType = 1001 GROUP BY IDType"))

            Dim workingHours = TimeSpan.FromHours(hoursWorkedFraction)

            Dim xNow As DateTime = roTypes.UnspecifiedNow()

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            Dim mDefinition As roCollection
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim dr = dt.Rows(0)
                mDefinition = New roCollection
                mDefinition.LoadXMLString(dr("Definition"))
                Dim endTimeLayer = roTypes.DateTimeAdd(xNow.Date, mDefinition("Finish"))

                Dim dtLayer As DataTable = AccessHelper.CreateDataTable(SqlLayer)

                If dtLayer IsNot Nothing AndAlso dtLayer.Rows.Count > 0 Then
                    Dim drLayer = dtLayer.Rows(0)
                    mDefinition = New roCollection
                    mDefinition.LoadXMLString(drLayer("Definition"))

                    Dim minTimeLayer = TimeSpan.FromHours(roConversions.ConvertTimeToHours(mDefinition("MinTime")))
                    Dim difference = minTimeLayer - workingHours
                    Dim tZoneInfo As TimeZoneInfo = Nothing
                    Dim nowTimeWithTimezone = roNotificationHelper.GetTimeOnTimezone(IDEmployee, xNow, tZoneInfo)

                    Dim tempBeginMandatory = roTypes.Any2Time(nowTimeWithTimezone.Add(difference))

                    If tempBeginMandatory.ValueDateTime > endTimeLayer Then
                        BeginMandatory = roTypes.Any2Time(roNotificationHelper.GetServerTime(IDEmployee, (endTimeLayer - difference), tZoneInfo))
                    End If
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEmployeeLateArrival::GetFlexibleMandatoryTime: Unexpected error: ", ex)
        End Try

        Return BeginMandatory
    End Function

End Class