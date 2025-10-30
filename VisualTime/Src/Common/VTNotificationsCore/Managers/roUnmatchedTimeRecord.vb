Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class roNotificationTask_Day_With_Unmatched_Time_Record_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Day_With_Unmatched_Time_Record, sGUID)
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
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric)),
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.Daywithunmatchedtimerecord.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.Daywithunmatchedtimerecord.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roUnmatchedTimeRecord::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Dim bolRet As Boolean = True

        Dim StartLimit As String = ""
        Dim EndLimit As String = ""

        Try
            If CheckAutomaticEndPunch() Then
                ' Obtenemos todas las notificaciones a verificar
                Dim strSql As String = "@SELECT# * FROM Notifications with (nolock) " &
                                " WHERE Activated=1" &
                                " AND IDType IN(19, 35) " &
                                " ORDER BY IDType"

                Dim tb As DataTable = AccessHelper.CreateDataTable(strSql)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    'Miramos licencia para mirar campos New One
                    Dim oLicSupport As New Extensions.roLicenseSupport()
                    Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()
                    Dim sVTEdition As String = String.Empty
                    If oLicInfo.Edition <> roServerLicense.roVisualTimeEdition.NotSet Then
                        sVTEdition = oLicInfo.Edition.ToString
                    End If
                    For Each oDaywithunmatchedtimeRow As DataRow In tb.Rows
                        ' Verificamos si existen empleados con fichajes impares
                        ' y aun no tiene alerta creada
                        strSql = "@SELECT# IDEmployee, Date FROM sysrovwIncompletedDays with (nolock) " &
                                "WHERE " &
                                "Date >=" & roTypes.Any2Time(Now.Date).Add(-30, "d").SQLSmallDateTime & " AND " &
                                "Date <" & roTypes.Any2Time(Now.Date).SQLSmallDateTime &
                                " AND NOT EXISTS " &
                                "(@SELECT# * " &
                                " From sysroNotificationTasks with (nolock) " &
                                " Where sysroNotificationTasks.Key1Numeric = sysrovwIncompletedDays.IDEmployee " &
                                " AND sysrovwIncompletedDays.Date =  sysroNotificationTasks.Key3Datetime " &
                                " AND IDNotification=" & roTypes.Any2Double(oDaywithunmatchedtimeRow("ID")) & ") "

                        Dim logSQL As String = strSql
                        Dim tbEmployees As DataTable = AccessHelper.CreateDataTable(strSql)
                        If tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0 Then
                            Dim bolInsert As Boolean = False
                            For Each oEmployeeRow As DataRow In tbEmployees.Rows
                                bolInsert = True
                                'Si la fecha es ayer, revisamos que no tenga horario nocturno
                                If roTypes.Any2Time(oEmployeeRow("Date")).Value = roTypes.Any2Time(Now.Date).Add(-1, "d").Value Then
                                    Dim tbShift As DataTable = AccessHelper.CreateDataTable("@SELECT# isnull(DailySchedule.IDShiftUsed, 0) as ShiftUsed, Shifts.StartLimit, Shifts.EndLimit, isnull(DailySchedule.ExpectedWorkingHours,Shifts.ExpectedWorkingHours) as ExpectedWorkingHours, isnull(IsFloating,0) as  IsFloating , DailySchedule.StartShiftUsed, DailySchedule.StartFlexible1 as StartFlexible, DailySchedule.EndFlexible1 as EndFlexible FROM DailySchedule with (nolock), Shifts   with (nolock) WHERE DailySchedule.IDShiftUsed = Shifts.id AND  IDEmployee=" &
                                                                                            roTypes.Any2Double(oEmployeeRow("IDEmployee")) & " AND Date=" & roTypes.Any2Time(oEmployeeRow("Date")).SQLSmallDateTime)
                                    If tbShift IsNot Nothing AndAlso tbShift.Rows.Count > 0 Then
                                        Dim IDShift As Double = roTypes.Any2Double(tbShift.Rows(0)("ShiftUsed"))
                                        If IDShift <> 0 Then
                                            If sVTEdition = "Starter" Then
                                                StartLimit = roTypes.Any2String(tbShift.Rows(0)("StartFlexible"))
                                                EndLimit = roTypes.Any2String(tbShift.Rows(0)("EndFlexible"))
                                            Else
                                                StartLimit = roTypes.Any2String(tbShift.Rows(0)("StartLimit"))
                                                EndLimit = roTypes.Any2String(tbShift.Rows(0)("EndLimit"))
                                            End If

                                            If roTypes.Any2Time(StartLimit).DateOnly <> roTypes.Any2Time(EndLimit).DateOnly Then
                                                bolInsert = False
                                            End If

                                            If bolInsert Then
                                                Try
                                                    ' Verificamos si es un flotante que pueda finalizar al dia siguiente
                                                    Dim IsFloating As Boolean = roTypes.Any2Boolean(tbShift.Rows(0)("IsFloating"))
                                                    If IsFloating AndAlso Not IsDBNull(tbShift.Rows(0)("StartShiftUsed")) Then
                                                        Dim StartTime As Date = roTypes.Any2Time(oEmployeeRow("Date")).Add(roTypes.Any2Time(tbShift.Rows(0)("StartShiftUsed"), True)).Value
                                                        Dim EndTime As Date = roTypes.Any2Time(StartTime).Add(roTypes.Any2Time(tbShift.Rows(0)("ExpectedWorkingHours"), True)).Value
                                                        If roTypes.Any2Time(StartTime).DateOnly <> roTypes.Any2Time(EndTime).DateOnly Then
                                                            bolInsert = False
                                                        End If
                                                    End If
                                                Catch ex As Exception
                                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roUnmatchedTimeRecord::GenerateNotificationTasks::Verificamos si es un flotante que pueda finalizar al dia siguiente::", ex)
                                                End Try
                                            End If
                                        End If
                                    End If
                                End If

                                If bolInsert Then
                                    Dim sFiredDate As String = ""
                                    If roTypes.Any2Boolean(oDaywithunmatchedtimeRow("ShowOnDesktop")) = True Then
                                        sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                                    Else
                                        sFiredDate = "NULL"
                                    End If
                                    strSql = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, FiredDate) VALUES (" &
                                                roTypes.Any2Double(oDaywithunmatchedtimeRow("ID")) & "," & oEmployeeRow("IDEmployee") & "," & roTypes.Any2Time(oEmployeeRow("Date")).SQLDateTime & ", " & sFiredDate & ")"

                                    AccessHelper.ExecuteSql(strSql)
                                End If
                            Next
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roUnmatchedTimeRecord::GenerateNotificationTasks::", ex)
        End Try

        Return bolRet
    End Function

    Private Function CheckAutomaticEndPunch() As Boolean
        '
        ' Creamos los fichajes automaticos del dia anterior en caso necesario
        '

        Dim bolRet As Boolean = True
        Dim bolSavePunch As Boolean = False

        Try
            ' Obtenemos las notificaciones generadas del dia anterior referentes al empleado,
            ' que necesita autorización para seguir trabajando,
            ' que aun no hayan sido procesadas

            ' Tipo de notificacion 72
            Dim strSQL As String = "@SELECT#  * From sysroNotificationTasks with (nolock) " &
                                          " Where IDNotification IN( @SELECT# ID FROM Notifications with (nolock) where IDType=72 and Activated=1 ) " &
                                          " AND sysroNotificationTasks.Key4DateTime = " & roTypes.Any2Time(Now.Date).Add(-1, "d").SQLDateTime &
                                          " AND isnull(sysroNotificationTasks.Key2Numeric, 0) = 0 and Executed=1 "
            Dim tb As DataTable = AccessHelper.CreateDataTable(strSQL)
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                ' Obtenemos el passport de sistema para crear el fichaje
                Dim idSystem As Integer = roTypes.Any2Integer(roConstants.GetSystemUserId())

                For Each oDaywithunmatchedtimeRow As DataRow In tb.Rows
                    ' Para cada empleado que se le notifico , verficamos si aun se debe generar el fichero de salida automatico
                    Dim IDEmployee As Double = roTypes.Any2Double(oDaywithunmatchedtimeRow("Key1Numeric"))
                    Dim xDate As Date = oDaywithunmatchedtimeRow("Key4DateTime")

                    ' Obtenemos el horario utilizado ese día y verificamos si es necesario crear el fichaje automatico
                    strSQL = "@SELECT#  CompleteExitAt, EnableCompleteExit FROM Shifts with (nolock) WHERE isnull(EnableCompleteExit,0) = 1  AND ID IN (@SELECT# IDShift1 FROM  DailySchedule with (nolock) WHERE IDEmployee=" & IDEmployee.ToString & " AND Date=" & roTypes.Any2Time(xDate.Date).SQLSmallDateTime & ")"
                    Dim tbShift As DataTable = AccessHelper.CreateDataTable(strSQL)
                    If tbShift IsNot Nothing AndAlso tbShift.Rows.Count > 0 Then
                        ' Obtenemos la hora a la que tenemos que crear el fichaje
                        Dim CompleteExitAt As Integer = roTypes.Any2Integer(tbShift.Rows(0)("CompleteExitAt"))

                        ' Obtenemos el ultimo fichaje del dia para ese Empleado/Fecha
                        strSQL = "@SELECT#  TOP 1 isnull(ActualType,0) As ActualType, isnull(DateTime, getdate()) As DateTime FROM Punches With (nolock) WHERE IDEmployee= " & IDEmployee.ToString & " And ShiftDate = " & roTypes.Any2Time(xDate.Date).SQLSmallDateTime & " And ActualType In(1,2)  Order by Datetime desc"
                        Dim tbEmployee As DataTable = AccessHelper.CreateDataTable(strSQL)
                        If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then
                            Dim ActualType As Double = roTypes.Any2Double(tbEmployee.Rows(0)("ActualType"))
                            Dim xDateTime As Date = tbEmployee.Rows(0)("DateTime")
                            If ActualType = 1 Then
                                ' En el caso que sea una entrada, y dicha entrada sea anterior
                                ' al fichaje que tenemos que generar, creamos la salida con la hora indicada en el horario
                                If xDateTime < xDate.Date.AddMinutes(CompleteExitAt) Then
                                    Dim oPunch As New VTBusiness.Punch.roPunch
                                    oPunch.IDEmployee = IDEmployee
                                    oPunch.DateTime = xDate.Date.AddMinutes(CompleteExitAt)
                                    oPunch.ShiftDate = xDate.Date
                                    oPunch.Type = PunchTypeEnum._OUT
                                    oPunch.ActualType = PunchTypeEnum._OUT
                                    oPunch.Passport = idSystem
                                    oPunch.Source = PunchSource.System
                                    oPunch.Save(False)
                                    bolSavePunch = True
                                    roTrace.GetInstance().AddTraceEvent("Forgotten exit punch at " & oPunch.DateTime?.ToString("yyyy-MM-dd HH:mm") & " created for employee with id " & IDEmployee)
                                End If
                            End If
                        End If
                    End If

                    ' Actualizamos la notificacion como verificada (Key2Numeric = 1)
                    strSQL = "@UPDATE# sysroNotificationtasks Set Key2Numeric= 1 WHERE ID= " & oDaywithunmatchedtimeRow("ID")
                    AccessHelper.ExecuteSql(strSQL)

                Next
            End If

            ' En el caso que se hayan guardado fichajes creamos la tarea de recalculo
            If bolSavePunch Then roConnector.InitTask(TasksType.MOVES)
        Catch ex As Exception
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roNotificationCreateManager::ExecuteNotificationsDaywithUnmatchedTimerecord::CheckAutomaticEndPunch :", ex)
        End Try

        Return bolRet
    End Function

End Class