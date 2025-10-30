Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace Notifications

    Public Class roNotificationHelper

        Public Shared roNullDate = "1/1/2079"

        Public Shared CO_SQLNOPUNCH = "@SELECT# * from(" &
            " @SELECT# DailyIncidences.IDEmployee, count(WorkingTime) AS NoWorking" &
            " FROM DailyIncidences JOIN sysroDailyIncidencesTypes On sysroDailyIncidencesTypes.ID = DailyIncidences.IDType And sysroDailyIncidencesTypes.WorkingTime = 0" &
            " JOIN DailySchedule On DailyIncidences.IDEmployee = DailySchedule.IDEmployee  And DailyIncidences.Date = DailySchedule.Date AND DailySchedule.Status > 40 " &
            " WHERE isnull(DailySchedule.IDShiftUsed, IDShift1) IN (@SELECT# ID FROM SHIFTS WHERE ExpectedWorkingHours > 0 )" &
            " And DailyIncidences.Date BETWEEN @iDate AND @fDate" &
            " AND DailyIncidences.IDEmployee in ( " &
            "     @SELECT# IDEmployee from EmployeeStatus where IsPresent = 0 and LastPunch < dateadd(""d"", " & '-@days, GETDATE()) AND BeginMandatory < GETDATE()" &
            "     -1*(@SELECT# @days + dbo.sysrofnPastConsecutiveNonWorkingDaysFromDate(EmployeeStatus.IDEmployee, @iDate, @fDate)), GETDATE()) AND BeginMandatory < GETDATE() " &
            " And IDEmployee not in( @SELECT# DISTINCT IDEmployee FROM ProgrammedAbsences WHERE BeginDate <= GETDATE() AND isnull(FinishDate, dateadd(""d"",MaxLastingDays,Begindate)) >= GETDATE() ))" &
            " GROUP BY DailyIncidences.IDEmployee) tmp where tmp.NoWorking >= @days"

        Public Shared CO_SQLNOPUNCHEMP = "@SELECT# top (@days)" &
            " ods.Date, ods.Status, IDEmployee," &
            " isnull((" &
            " @SELECT# count(WorkingTime) As Working" &
            " FROM DailyIncidences JOIN sysroDailyIncidencesTypes" &
            " On sysroDailyIncidencesTypes.ID = DailyIncidences.IDType And sysroDailyIncidencesTypes.WorkingTime=1" &
            " JOIN DailySchedule On DailyIncidences.IDEmployee = DailySchedule.IDEmployee AND DailySchedule.Status > 40 " &
            " And DailyIncidences.Date = DailySchedule.Date" &
            " WHERE isnull(DailySchedule.IDShiftUsed, IDShift1) In(@SELECT# ID FROM SHIFTS WHERE ExpectedWorkingHours > 0)" &
            " And DailyIncidences.Date = ods.Date" &
            " And DailyIncidences.IDEmployee Not In( @SELECT# DISTINCT IDEmployee FROM ProgrammedAbsences WHERE BeginDate <= ods.Date" &
            " And isnull(FinishDate, dateadd(""d"",MaxLastingDays,Begindate)) >= ods.Date )" &
            " And DailyIncidences.IDEmployee Not In( @SELECT# DISTINCT IDEmployee FROM DailySchedule WHERE Date = ods.Date And isnull(IsHolidays,0) = 1)" &
            " And DailyIncidences.IDEmployee Not In( @SELECT# DISTINCT IDEmployee FROM ProgrammedHolidays  WHERE Date = ods.Date  )" &
            " And DailyIncidences.IDEmployee = ods.IDEmployee" &
            " ),0) As Presente" &
            " FROM DailySchedule ods WHERE isnull(IDShiftUsed, IDShift1) In(@SELECT# ID FROM SHIFTS WHERE ExpectedWorkingHours > 0 )" &
            " And Date BETWEEN @iDate And @fDate" &
            " And IDEmployee = @idEmployee" &
            " ORDER BY Date desc"

        Public Shared Function CheckTerminalConnectedNotifications(ByVal oNotificationTask As DataRow)
            Dim bRet As Boolean = False

            Try

                ' Alerta de terminal desconectado. Independientemente de que se tenga que enviar email ...
                Dim notificationType As eNotificationType = roTypes.Any2Integer(oNotificationTask("IDType"))
                If notificationType = eNotificationType.Terminal_Disconnected Then
                    Dim strTerminal As String = ""
                    Dim dt As DataTable = CreateDataTable("@SELECT# * FROM Terminals WHERE ID = " & Any2String(oNotificationTask("Key1Numeric")))

                    If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then

                        strTerminal = Any2String(dt.Rows(0)("Description"))

                        Dim Params As New ArrayList
                        Params.Add(strTerminal)
                        Params.Add(Format$(oNotificationTask("Key3DateTime"), "dd/MM/yyyy HH:mm"))

                        If oNotificationTask("Parameters") = "NotConnected" Then
                            Dim oUserTask As New VTBusiness.UserTask.roUserTask()
                            oUserTask.ID = VTBusiness.UserTask.roUserTask.roUserTaskObject & ":\\TERMINALDISCONNECTED" & dt.Rows(0)("ID")
                            oUserTask.DateCreated = Now
                            oUserTask.TaskType = VTBusiness.UserTask.TaskType.UserTaskRepair
                            oUserTask.ResolverURL = "FN:\\Resolver_TerminalDisconnected"
                            oUserTask.Message = Message("Notification.TerminalDisconnected.Body", Params, , "ESP")
                            oUserTask.ResolverValue1 = Left(strTerminal, 50)
                            oUserTask.ResolverValue2 = Format$(oNotificationTask("Key3DateTime"), "dd/MM/yyyy HH:mm")
                            oUserTask.Save()
                        Else
                            Dim oUserTask As New VTBusiness.UserTask.roUserTask()
                            oUserTask.ID = VTBusiness.UserTask.roUserTask.roUserTaskObject & ":\\TERMINALDISCONNECTED" & dt.Rows(0)("ID")
                            oUserTask.Delete(False)
                        End If
                    End If
                End If
            Catch ex As Exception
                bRet = False
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roNotificationHelper::CheckTerminalConnectedNotifications::", ex)
            End Try

            Return bRet
        End Function

        Public Shared Function HaveToExcludeNotification(ByVal oNotificationTask As DataRow, ByVal IdEmployee As Integer) As Boolean
            Dim bExcludeNotificacion As Boolean = False

            Dim strDisabledNotificationsList As New List(Of String)(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTLive.Notification.DisabledNotificationsList").Split(","))
            Dim strDisableNotificationUserField As String = roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTLive.Notification.DisableUserField"))

            Try

                Dim notificationType As eNotificationType = roTypes.Any2Integer(oNotificationTask("IDType"))
                If strDisabledNotificationsList.Contains(roTypes.Any2String(notificationType)) Then
                    Dim strSQL As String = "@DECLARE# @Date smalldatetime " &
                                            "SET @Date = " & Any2Time(Now.Date).SQLSmallDateTime & " " &
                                            "@SELECT# * FROM GetEmployeeUserFieldValue(" & Any2String(IdEmployee) & ",'" & strDisableNotificationUserField & "', @Date)"
                    Dim tbs As DataTable = CreateDataTable(strSQL)
                    If tbs IsNot Nothing AndAlso tbs.Rows.Count > 0 Then
                        If roTypes.Any2String(tbs.Rows(0).Item("Value").ToString) = "1" Then bExcludeNotificacion = True
                    End If
                End If

                ' Sólo envío notificaciones referentes a empleados con contrato
                If Not bExcludeNotificacion AndAlso IdEmployee > 0 Then
                    Dim oEmployee As New VTEmployees.Employee.roEmployee
                    oEmployee = Robotics.Base.VTEmployees.Employee.roEmployee.GetEmployee(IdEmployee, New VTEmployees.Employee.roEmployeeState(-1))
                    If oEmployee IsNot Nothing Then bExcludeNotificacion = (oEmployee.EmployeeStatus = DTOs.EmployeeStatusEnum.Old)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CNotificationServer::Work:Error cheking if notifications should be excluded:" & oNotificationTask("ID"), ex)
            End Try

            Return bExcludeNotificacion
        End Function

        Public Shared Function Message(ByVal strKey As String, Optional ByVal oParamList As ArrayList = Nothing, Optional ByVal strFileReference As String = "ProcessNotificationServer", Optional ByVal strLanguageKey As String = "ESP", Optional bInvalidateCache As Boolean = False) As String
            Dim strMessage As String = String.Empty
            Dim bCustomLanguageContent As Dictionary(Of String, Byte()) = roCacheManager.GetInstance().GetCustomLanguage(RoAzureSupport.GetCompanyName())

            If bCustomLanguageContent Is Nothing Then
                bCustomLanguageContent = New Dictionary(Of String, Byte()) From {
                            {"ESP", Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer.ESP.CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)},
                            {"ENG", Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer.ENG.CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)},
                            {"CAT", Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer.CAT.CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)},
                            {"POR", Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer.POR.CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)},
                            {"GAL", Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer.GAL.CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)},
                            {"ITA", Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer.ITA.CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)},
                            {"EKR", Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer.EKR.CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)},
                            {"FRA", Robotics.Azure.RoAzureSupport.DownloadFile("ProcessNotificationServer.FRA.CUST.LNG", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName() & "/customFiles", False)}
                        }

                roCacheManager.GetInstance().UpdateCustomLanguageCache(RoAzureSupport.GetCompanyName(), bCustomLanguageContent)
            End If

            Dim oLanguageCustom = New roLanguageLocalCustom
            oLanguageCustom.SetLanguageReference(strFileReference, strLanguageKey)

            Try
                If bCustomLanguageContent(strLanguageKey.ToUpper) IsNot Nothing AndAlso bCustomLanguageContent(strLanguageKey.ToUpper).Length > 0 Then
                    oLanguageCustom.LoadFromByteArray(bCustomLanguageContent(strLanguageKey.ToUpper))
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CNotificationServer::Message::Could not load from azure::", ex)
            End Try

            oLanguageCustom.ClearUserTokens()
            If oParamList IsNot Nothing Then
                For i As Integer = 0 To oParamList.Count - 1
                    oLanguageCustom.AddUserToken(oParamList(i))
                Next
            End If

            strMessage = oLanguageCustom.Translate(strKey, "")

            Dim oLanguage = New roLanguage
            oLanguage.SetLanguageReference(strFileReference, strLanguageKey)
            If strMessage = String.Empty OrElse strMessage = "NotFound" OrElse strMessage = "(NoEntry)" Then
                oLanguage.ClearUserTokens()
                If oParamList IsNot Nothing Then
                    For i As Integer = 0 To oParamList.Count - 1
                        oLanguage.AddUserToken(oParamList(i))
                    Next
                End If

                strMessage = oLanguage.Translate(strKey, "")
            Else
                strMessage = oLanguage.TranslateDicTokens(strMessage)
            End If

            Return strMessage
        End Function

        Public Shared Function CreateNewDestination(email As String, language As String, idEmployee As Integer, idPassport As Integer, bAddEmployeeWarning As Boolean, isRoboticsUser As Boolean) As roNotificationDestinationConfig
            Try
                If email <> String.Empty Then
                    Return New roNotificationDestinationConfig() With {
                            .Email = email,
                            .Language = language,
                            .AddEmployeeWarning = bAddEmployeeWarning,
                            .IdUser = New roNotificationUserConfig() With {
                                    .IdEmployee = idEmployee,
                                    .IdPassport = idPassport,
                                    .IsRoboticsUser = isRoboticsUser
                                }
                            }
                Else
                    Return Nothing
                End If
            Catch ex As Exception

                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationHelper::CreateNewDestination::", ex)
                Return Nothing
            End Try

        End Function

        Public Shared Function CreateNewPushDestination(language As String, idEmployee As Integer, idPassport As Integer) As roNotificationDestinationConfig
            Try

                Return New roNotificationDestinationConfig() With {
                            .Email = "",
                            .Language = language,
                            .AddEmployeeWarning = False,
                            .IdUser = New roNotificationUserConfig() With {
                                    .IdEmployee = idEmployee,
                                    .IdPassport = idPassport,
                                    .IsRoboticsUser = False
                                }
                            }
            Catch ex As Exception

                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationHelper::CreateNewPushDestination::", ex)
                Return Nothing
            End Try

        End Function

        Public Shared Function GetInitialDate() As Date
            Dim xDate As Date
            Dim oParameters As New roParameters("OPTIONS", True)

            If oParameters.Parameter(Parameters.InitialNotifierDate) Is Nothing Then
                oParameters.Parameter(Parameters.InitialNotifierDate) = Date.Now.Date
                oParameters.Save()
            End If

            Try
                If IsDate(oParameters.Parameter(Parameters.InitialNotifierDate)) Then
                    xDate = roTypes.Any2Time(oParameters.Parameter(Parameters.InitialNotifierDate)).Value
                Else
                    xDate = roTypes.CreateDateTime(1900, 1, 1)
                End If
            Catch ex As Exception
                VTBase.roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCreateManager::GetInitialDate::Error recovering initial date", ex)
                xDate = Date.Now.Date
            End Try

            Return xDate
        End Function

        Public Shared Function EmployeeOnProgrammedCauseOnTime(ByVal oDateTime As Date, ByVal IDEmployee As Double, ByVal xNow As DateTime) As Boolean
            Dim sSQL As String
            Dim BeginPeriod As New roTime
            Dim EndPeriod As New roTime

            Try

                ' Verificamos si existe alguna prevision de ausencia por horas para el dia a revisar
                sSQL = "@SELECT# BeginTime , EndTime, convert(numeric(8,6), isnull(Duration,0)) as duration  " &
                        "FROM ProgrammedCauses " &
                                "LEFT JOIN Causes On Causes.ID = ProgrammedCauses.IDCause " &
                        "WHERE idEmployee = " & IDEmployee & " AND " &
                            "((Date BETWEEN " & roTypes.Any2Time(roTypes.Any2Time(oDateTime).DateOnly).SQLSmallDateTime & " AND " & roTypes.Any2Time(roTypes.Any2Time(oDateTime).DateOnly).SQLSmallDateTime & ") OR " &
                            "(FinishDate BETWEEN " & roTypes.Any2Time(roTypes.Any2Time(oDateTime).DateOnly).SQLSmallDateTime & " AND " & roTypes.Any2Time(roTypes.Any2Time(oDateTime).DateOnly).SQLSmallDateTime & ") OR " &
                            "(Date < " & roTypes.Any2Time(roTypes.Any2Time(oDateTime).DateOnly).SQLSmallDateTime & " AND FinishDate > " & roTypes.Any2Time(roTypes.Any2Time(oDateTime).DateOnly).SQLSmallDateTime & ")) Order by BeginTime desc"
                Dim dt As DataTable = AccessHelper.CreateDataTable(sSQL)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each dr As DataRow In dt.Rows
                        ' Obtenemos el periodo de tiempo de la prevision de horas
                        BeginPeriod = New roTime
                        BeginPeriod = roTypes.Any2Time(roTypes.DateTimeAdd(roTypes.Any2Time(oDateTime).DateOnly, roTypes.Any2Time(dr("BeginTime")).Value)) ' Inicio del periodo
                        EndPeriod = New roTime
                        EndPeriod = roTypes.Any2Time(roTypes.DateTimeAdd(roTypes.Any2Time(oDateTime).DateOnly, roTypes.Any2Time(dr("EndTime")).Value)) ' Fin del periodo
                        If roTypes.Any2Time(xNow).Value >= BeginPeriod.Value And roTypes.Any2Time(xNow).Value <= EndPeriod.Value Then
                            ' Si actualmente estamos dentro de una prevision de horas,
                            ' revisamos si se ha superado la hora limite
                            BeginPeriod = roTypes.Any2Time(roTypes.DateTimeAdd(BeginPeriod.Value, roTypes.Any2Time(dr("duration")).Value)) ' Hora limite
                            If roTypes.Any2Time(xNow).Value <= BeginPeriod.Value Then
                                ' Si no se ha superado la hora m�xima hasta la que puede ausentarse el empleado,
                                ' esta dentro de los margenes correctos
                                Return True
                            Else
                                ' Se ha superado la hora maxima y incumplimos la prevision
                            End If
                        End If
                    Next
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCreateManager::EmployeeOnProgrammedCauseOnTime: Unexpected error: ", ex)
            End Try

            Return False
        End Function

        Public Shared Function IsEmployeeValidConcept(ByVal IDEmployee As Long, ByVal sDate As String, ByVal mCondition As roCollection) As Boolean
            Dim bRet As Boolean = False
            Dim IDConcept As Double
            Dim CompareConceptType As Double
            Dim TargetTypeConcept As String '"DirectValue" � "UserField"
            Dim TargetConcept As String
            Dim ValueCompare As Double
            Dim TotalAccrual As Double

            Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

            Try

                IDConcept = Any2Double(mCondition("IDConcept"))

                CompareConceptType = Any2Double(mCondition("CompareConceptType"))
                TargetTypeConcept = Any2String(mCondition("TargetTypeConcept"))
                TargetConcept = Any2String(mCondition("TargetConcept"))

                If IDConcept = 0 Then
                    Return True
                End If

                TotalAccrual = Any2Double(AccessHelper.ExecuteScalar("@SELECT# ISNULL(SUM(VALUE), 0) AS TOTAL FROM DailyAccruals WHERE IDEmployee=" & IDEmployee & " AND Date =" & Any2Time(sDate).SQLSmallDateTime & " AND isnull(CarryOver,0) = 0 AND isnull(StartupValue,0) = 0 AND IDConcept =" & IDConcept))

                Select Case TargetTypeConcept
                    Case "DirectValue"
                        ' Valor directo
                        If InStr(TargetConcept, ":") > 0 Then
                            ValueCompare = DateTime2Double(CDate(TargetConcept))
                        Else
                            ValueCompare = Any2Double(Strings.Replace(TargetConcept, ".", oInfo.CurrencyDecimalSeparator))
                        End If

                    Case "UserField"
                        ' El valor de un campo de la ficha
                        Dim userFieldRawValue As String = String.Empty
                        Dim oUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, TargetConcept, Any2Time(sDate).Value, New VTUserFields.UserFields.roUserFieldState(-1), False)
                        userFieldRawValue = oUserField.FieldRawValue

                        If InStr(userFieldRawValue, ":") > 0 Then
                            ValueCompare = DateTime2Double(CDate(userFieldRawValue))
                        Else
                            ValueCompare = Any2Double(Strings.Replace(userFieldRawValue, ".", oInfo.CurrencyDecimalSeparator))
                        End If

                    Case Else
                        Return True
                End Select

                Select Case CompareConceptType
                    Case UserFieldsTypes.CompareType.Equal
                        If TotalAccrual = ValueCompare Then bRet = True
                    Case UserFieldsTypes.CompareType.Minor
                        If TotalAccrual < ValueCompare Then bRet = True
                    Case UserFieldsTypes.CompareType.MinorEqual
                        If TotalAccrual <= ValueCompare Then bRet = True
                    Case UserFieldsTypes.CompareType.Major
                        If TotalAccrual > ValueCompare Then bRet = True
                    Case UserFieldsTypes.CompareType.MajorEqual
                        If TotalAccrual >= ValueCompare Then bRet = True
                    Case UserFieldsTypes.CompareType.Distinct
                        If TotalAccrual <> ValueCompare Then bRet = True
                    Case Else
                        bRet = True
                End Select
            Catch ex As Exception
                bRet = True
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCreateManager::IsEmployeeValidConcept: Unexpected error: ", ex)
            End Try

            Return bRet
        End Function

        Public Shared Function CreateBreakNotification(ByVal dtPunches As DataTable, ByVal courtesyTime As TimeSpan, ByVal beginControlPeriod As Date, ByVal finishControlPeriod As Date, ByVal notificationType As eNotificationType, idEmployee As Integer, idNotification As Integer, ByVal breakStartTime As Date, ByVal beginWorkingDate As Date) As Boolean

            Dim SQL As String
            Dim bRet As Boolean = True
            Dim breakRuleInfringed As Boolean = False
            Dim breakStarted As Boolean = False
            Dim breakCompleted As Boolean = False
            Dim workingDayStarted As Boolean = False
            Dim i As Integer = 0
            Dim continueChecking As Boolean = True

            Try

                finishControlPeriod = finishControlPeriod.Add(courtesyTime)
                'Comprobar sus punches de entrada y salida
                If dtPunches IsNot Nothing AndAlso dtPunches.Rows.Count > 0 Then
                    ' Considero todos los fichajes anteriores al final del periodo de control
                    ' El periodo de control es:
                    '   - Aviso de descanso no realizado (BreakNotTaken): La franja de descanso.
                    '   - Aviso de descanso no inicialdo (BreakStart): Desde el inicio de la franja de descanso hasta X minutos después, con X el margen de cortesia
                    '   - Aviso de descanso no finalizado (BreakFinish): Desde el inicio de la franja de descanso hasta X minutos después del final de la franja de descanso, con X el margen de cortesia
                    ' Si tiene una salida dentro del periodo de control, es que ha empezado el descanso
                    ' Si tras haber empezado el descanso tiene una entrada, es que ha completado el descanso
                    breakRuleInfringed = False
                    breakStarted = False
                    breakCompleted = False
                    While i < dtPunches.Rows.Count AndAlso continueChecking
                        Dim drPunch As DataRow = dtPunches.Rows(i)
                        If Any2DateTime(drPunch("DateTime")) >= beginControlPeriod Then
                            If Any2DateTime(drPunch("DateTime")) <= finishControlPeriod Then
                                'Por si empieza la jornada una vez ha empezado el descanso ...
                                If Not workingDayStarted AndAlso Any2DateTime(drPunch("DateTime")) >= beginWorkingDate.AddHours(-4) AndAlso Any2Integer(drPunch("ActualType")) = 1 Then
                                    workingDayStarted = True
                                Else
                                    Select Case Any2Integer(drPunch("ActualType"))
                                        Case 1 'Entrada
                                            If breakStarted Then
                                                breakCompleted = True
                                                continueChecking = False
                                            End If
                                        Case 2 'Salida
                                            If workingDayStarted Then
                                                breakStarted = True
                                            End If
                                    End Select
                                End If
                            Else
                                ' Los fichajes posteriores al descanso no son relevantes
                                continueChecking = False
                            End If
                        Else
                            ' Los fichajes previos al descanso sólo son relevantes para decidir si el empleado empezó a trabajar hoy
                            ' Si un empleado NO viene a trabajar, no hay que notificarle que no ha hecho el descanso al supervisor, ni que no lo ha empezado a el.
                            If Not workingDayStarted AndAlso Any2DateTime(drPunch("DateTime")) >= beginWorkingDate.AddHours(-4) AndAlso Any2Integer(drPunch("ActualType")) = 1 Then
                                workingDayStarted = True
                            End If
                        End If
                        i += 1
                    End While

                    Select Case notificationType
                        Case eNotificationType.BreakNotTaken
                            breakRuleInfringed = workingDayStarted AndAlso Not breakStarted
                        Case eNotificationType.BreakStart
                            breakRuleInfringed = workingDayStarted AndAlso Not breakStarted
                        Case eNotificationType.BreakFinish
                            breakRuleInfringed = breakStarted AndAlso Not breakCompleted
                    End Select

                    If breakRuleInfringed Then
                        'Crear la notificación
                        SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime) " &
                              "VALUES (@idNotification, @idEmployee, @beginBreakDate)"

                        SQL = Strings.Replace(SQL, "@idNotification", Any2String(idNotification))
                        SQL = Strings.Replace(SQL, "@idEmployee", Any2String(idEmployee))
                        SQL = Strings.Replace(SQL, "@beginBreakDate", Any2Time(breakStartTime).SQLDateTime)

                        bRet = AccessHelper.ExecuteSql(SQL)
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCreateManager::CreateBreakNotification: Unexpected error: ", ex)
                bRet = False
            End Try

            Return bRet
        End Function

        Public Shared Function GetServerTime(ByVal idemployee As Integer, ByVal xDate As DateTime, ByRef tZoneInfo As TimeZoneInfo) As DateTime

            Dim bMultitimezone As Boolean = roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "MultiTimeZoneEnabled"))

            If bMultitimezone Then
                If tZoneInfo Is Nothing Then
                    Try
                        tZoneInfo = GetUserTimezone(idemployee, tZoneInfo)
                    Catch ex As Exception
                        tZoneInfo = TimeZoneInfo.Local
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationHelper::GetLocalizedDatetime::" & ex.Message, ex)
                    End Try
                End If

                xDate = roTypes.CreateDateTime(xDate.Year, xDate.Month, xDate.Day, xDate.Hour, xDate.Minute, xDate.Second, DateTimeKind.Unspecified)
                Return TimeZoneInfo.ConvertTime(xDate, tZoneInfo, TimeZoneInfo.Local)
            Else
                Return xDate
            End If

        End Function

        Public Shared Function GetTimeOnTimezone(ByVal idemployee As Integer, ByVal xDate As DateTime, ByRef tZoneInfo As TimeZoneInfo) As DateTime

            Dim bMultitimezone As Boolean = roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "MultiTimeZoneEnabled"))

            If bMultitimezone Then
                If tZoneInfo Is Nothing Then
                    Try
                        tZoneInfo = GetUserTimezone(idemployee, tZoneInfo)
                    Catch ex As Exception
                        tZoneInfo = TimeZoneInfo.Local
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationHelper::GetLocalizedDatetime::" & ex.Message, ex)
                    End Try
                End If

                xDate = roTypes.CreateDateTime(xDate.Year, xDate.Month, xDate.Day, xDate.Hour, xDate.Minute, xDate.Second, DateTimeKind.Unspecified)
                Return TimeZoneInfo.ConvertTime(xDate, TimeZoneInfo.Local, tZoneInfo)
            Else
                Return xDate
            End If

        End Function

        Private Shared Function GetUserTimezone(idemployee As Integer, tZoneInfo As TimeZoneInfo) As TimeZoneInfo
            tZoneInfo = TimeZoneInfo.Local

            Dim oEmployeeState As New Employee.roEmployeeState
            Dim oMobility As Employee.roMobility = Employee.roMobility.GetCurrentMobility(idemployee, oEmployeeState)
            If oEmployeeState.Result = EmployeeResultEnum.NoError Then

                Dim oGroupState As New Group.roGroupState
                Dim idWorkingZone As Integer = -1
                Dim idNonWorkingzone As Integer = -1
                Group.roGroup.GetGroupZones(oMobility.IdGroup, idWorkingZone, idNonWorkingzone, oGroupState)

                If oGroupState.Result = GroupResultEnum.NoError AndAlso idWorkingZone > -1 Then
                    Dim oZoneState As New Zone.roZoneState
                    Dim zone As New Zone.roZone(idWorkingZone, oZoneState)
                    If oZoneState.Result = ZoneResultEnum.NoError AndAlso (Not String.IsNullOrEmpty(zone.DefaultTimezone)) Then
                        tZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(zone.DefaultTimezone)
                    End If
                End If
            End If

            Return tZoneInfo
        End Function
    End Class

End Namespace