Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Runtime.Serialization
Imports Robotics.Base.AIPlanner
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBudgets
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Absence
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.Incidence
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTChannels
Imports Robotics.Base.VTDocuments
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.Base.VTRequests
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes

Namespace VTEOGManager

    <DataContract>
    Public Class roEOGManager

#Region "Methods"

        Public Shared Function ExecuteTask(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolret As New BaseTaskResult With {.Result = True, .Description = String.Empty}
            Try
                Select Case UCase(oTask.Action)
                    Case roLiveTaskTypes.AssignTemplate.ToString().ToUpper
                        bolret = roEOGManager.AssignTemplate(oTask) ' Plantilla de festivos
                    Case roLiveTaskTypes.AssignTemplatev2.ToString().ToUpper
                        bolret = roEOGManager.AssignTemplatev2(oTask) ' Plantilla de festivos v2
                    Case roLiveTaskTypes.AssignWeekPlan.ToString().ToUpper
                        bolret = roEOGManager.AssignWeekPlan(oTask) ' Planificación semanal
                    Case roLiveTaskTypes.CopyAdvancedPlan.ToString().ToUpper
                        bolret = roEOGManager.CopyAdvancedPlan(oTask) ' Copiar planificación avanzada de un empleado a otro
                    Case roLiveTaskTypes.ValidateSignStatusDocument.ToString().ToUpper
                        bolret = roEOGManager.ValidateSignStatusDocument(oTask) ' Validamos el documento firmado por el usuario
                    Case roLiveTaskTypes.CopyAdvancedPlanv2.ToString().ToUpper
                        bolret = roEOGManager.CopyAdvancedPlanv2(oTask) ' Copiar planificación avanzada de un empleado a otro v2
                    Case roLiveTaskTypes.CopyPlan.ToString().ToUpper
                        bolret = roEOGManager.CopyPlan(oTask) ' Copiar planificación de un empleado a otro
                    Case roLiveTaskTypes.MassCopy.ToString().ToUpper
                        bolret = roEOGManager.MassCopy(oTask) ' Copia masiva de datos
                    Case roLiveTaskTypes.MassCause.ToString().ToUpper
                        bolret = roEOGManager.MassCauses(oTask) ' Introducción masiva de Justificaciones
                    Case roLiveTaskTypes.JustifiedIncidences.ToString().ToUpper
                        bolret = roEOGManager.MassIncidences(oTask) ' Justificac masiva de Incidencias
                    Case roLiveTaskTypes.AssignCenters.ToString().ToUpper
                        bolret = roEOGManager.MassAssignCenters(oTask) ' Asignacion masiva de centros de coste
                    Case roLiveTaskTypes.MassProgrammedAbsence.ToString().ToUpper
                        bolret = roEOGManager.MassProgrammedAbsence(oTask) 'Asignación masiva de ausencias previstas
                    Case roLiveTaskTypes.EmployeeMessage.ToString().ToUpper
                        bolret = roEOGManager.MessageEmployees(oTask) ' Mensajes massivos a empleados
                    Case roLiveTaskTypes.MassPunch.ToString().ToUpper
                        bolret = roEOGManager.MassPunchInput(oTask) ' Mensajes massivos a empleados
                    Case roLiveTaskTypes.DeleteOldPhotos.ToString.ToUpper
                        bolret = roEOGManager.DeleteOldPhotos(oTask) ' Borra las fotos antiguas de VisualTime para la auditoria
                    Case roLiveTaskTypes.DeleteOldPunches.ToString.ToUpper
                        bolret = roEOGManager.DeleteOldPunches(oTask) ' Borra los fichajes con más de 4 años de antigüedad de usuarios dados de baja que tengan habilitado el derecho al olvido, y el historial de conexiones de mas de 3 meses de antigüedad
                    Case roLiveTaskTypes.DeleteOldBiometricData.ToString.ToUpper
                        bolret = roEOGManager.DeleteOldBiometricData(oTask) ' Borra los datos biometricos de VisualTime para la auditoria
                    Case roLiveTaskTypes.CheckCloseDate.ToString.ToUpper
                        bolret = roEOGManager.CheckCloseDateAlert(oTask) ' Borra las fotos antiguas de VisualTime para la auditoria
                    Case roLiveTaskTypes.ManageVisits.ToString.ToUpper()
                        bolret = roEOGManager.ManageVisits(oTask) ' Borra las fotos antiguas de VisualTime para la auditoria
                    Case roLiveTaskTypes.PurgeNotifications.ToString.ToUpper()
                        bolret = roEOGManager.PurgeNotifications(oTask) ' Borra todas las notificaciones de empleados sin contrato
                    Case roLiveTaskTypes.CompleteTasksAndProjects.ToString.ToUpper()
                        bolret = roEOGManager.CompleteTasksAndProjects(oTask) ' Borra todas las notificaciones de empleados sin contrato
                    Case roLiveTaskTypes.DeleteOldDocuments.ToString.ToUpper
                        bolret = roEOGManager.DeleteOldDocuments(oTask) ' Borra los documentos antiguos de VisualTime para la auditoria
                    Case roLiveTaskTypes.ValidityDocuments.ToString.ToUpper
                        bolret = roEOGManager.CheckDocuments(oTask) ' Valida las fechas de validez de los documento para marcar los caducados
                    Case roLiveTaskTypes.DocumentTracking.ToString.ToUpper
                        bolret = roEOGManager.DocumentTracking(oTask) ' Alertas de obligatoriedad de presentar documenación
                    Case roLiveTaskTypes.CopyAdvancedBudgetPlan.ToString().ToUpper
                        bolret = roEOGManager.CopyAdvancedBudgetPlan(oTask) ' Copiar planificación avanzada de presupuestos
                    Case roLiveTaskTypes.DeleteAccessMovesHistory.ToString().ToUpper
                        bolret = roEOGManager.DeleteOldAccessMovesHistory(oTask) ' Borra el histórico de fichajes de accesos
                    Case roLiveTaskTypes.AIPlannerTask.ToString().ToUpper
                        bolret = roEOGManager.ExecuteAIPlannerTask(oTask) ' Borra el histórico de fichajes de accesos
                    Case roLiveTaskTypes.SecurityPermissions.ToString.ToUpper()
                        bolret = roEOGManager.GenerateSecurityPermissionsV3(oTask) ' Proceso de mapeo del organigrama de seguridad a estructura de permisos
                    Case roLiveTaskTypes.GenerateRoboticsPermissions.ToString().ToUpper
                        bolret = roEOGManager.GenerateRoboticsPermissionsV3(oTask)
                    Case roLiveTaskTypes.ConsolidateData.ToString().ToUpper
                        bolret = roEOGManager.ConsolidateVisualTimeData(oTask)
                    Case roLiveTaskTypes.RecalculatePunchDirection.ToString().ToUpper
                        bolret = roEOGManager.RecalculatePunchDirection(oTask) ' Recalcula dirección de fichajes de tipo AUTO entre fechas para X empleados
                    Case roLiveTaskTypes.MigrateDocsToAzure.ToString().ToUpper
                        bolret = roEOGManager.MigrateDocsToAzure(oTask) ' Recalcula dirección de fichajes de tipo AUTO entre fechas para X empleados
                    Case roLiveTaskTypes.AddReportToDocManager.ToString().ToUpper
                        bolret = roEOGManager.AddReportToDocManager(oTask) ' Recalcula dirección de fichajes de tipo AUTO entre fechas para X empleados
                    Case roLiveTaskTypes.CheckScheduleRulesFaults.ToString().ToUpper
                        bolret = roEOGManager.CheckScheduleRulesFaults(oTask) 'Calcula una vez al día si hay alertas de planificación pendientes de notificar
                    Case roLiveTaskTypes.MassLockDate.ToString().ToUpper
                        bolret = roEOGManager.MassLockDate(oTask) 'Asignación masiva de fecha de cierre
                    Case roLiveTaskTypes.CheckInvalidEntries.ToString().ToUpper
                        bolret = roEOGManager.CheckInvalidEntries(oTask) 'Fichajes incorrectos y borrado de notificaciones
                    Case roLiveTaskTypes.SynchronizeTerminals.ToString().ToUpper
                        bolret = roEOGManager.SynchronizeTerminals(oTask) 'Sincronizar terminales
                    Case roLiveTaskTypes.RecalculateRequestStatus.ToString().ToUpper
                        bolret = roEOGManager.RecalculateRequestStatus(oTask) 'Recalculo del estado de solicitudes pendientes o en curso
                    Case roLiveTaskTypes.ChangeRequestPermissions.ToString().ToUpper
                        bolret = roEOGManager.ChangeRequestPermissions(oTask) 'Recalculo permisos sobre solicitudes
                    Case roLiveTaskTypes.RemoveExpiredTasks.ToString.ToUpper()
                        bolret = roEOGManager.RemoveExpiredTasks(oTask)
                    Case roLiveTaskTypes.CheckAutomaticRequests.ToString.ToUpper()
                        bolret = roEOGManager.CheckAutomaticRequests(oTask)
                    Case roLiveTaskTypes.DeleteOldAudit.ToString.ToUpper()
                        bolret = roEOGManager.DeleteOldAudit(oTask)
                    Case roLiveTaskTypes.BlockInactivePassports.ToString.ToUpper
                        bolret = roEOGManager.BlockInactivePassports(oTask)
                    Case roLiveTaskTypes.EmployeeSecurtiyActions.ToString.ToUpper
                        bolret = roEOGManager.EmployeeSecurtiyActions(oTask)
                    Case roLiveTaskTypes.CheckConcurrenceData.ToString.ToUpper
                        bolret = roEOGManager.CheckConcurrenceData(oTask)
                    Case roLiveTaskTypes.DeleteOldComplaints.ToString.ToUpper
                        bolret = roEOGManager.DeleteOldComplaints(oTask)
                    Case roLiveTaskTypes.DataMonitoring.ToString.ToUpper
                        bolret = roEOGManager.DataMonitoring(oTask)
                End Select

            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::ExecuteTask :", Ex)
                bolret.Result = False
            End Try

            Return bolret
        End Function

#End Region

        Public Shared Function CheckAutomaticRequests(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try

                Dim intIDPassport As Integer = roConstants.GetSystemUserId()

                If intIDPassport > 0 Then
                    ' 01. RECHAZO AUTOMATICO DE SOLICITUDES
                    ' Obtenemos todas las solicitudes que hayan llegado a la fecha de rechazo y que esten pendientes o en curso
                    ' y que no sean automaticas
                    Dim strSQL As String = "@SELECT# ID FROM Requests WHERE Status in(" & eRequestStatus.Pending & "," & eRequestStatus.OnGoing & ") AND RejectedDate is not null and RejectedDate <= " & Any2Time(Now.Date).SQLSmallDateTime & " AND isnull(AutomaticValidation,0) = 0 ORDER BY RequestDate "
                    Dim tb As DataTable = CreateDataTableWithoutTimeouts(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        Dim oRequest As Requests.roRequest = Nothing
                        For Each oRequestRow As DataRow In tb.Rows
                            bolRet = False

                            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
                            Try

                                Dim oSupervisorState As New Requests.roRequestState(intIDPassport)
                                oRequest = New Requests.roRequest(oRequestRow("ID"), oSupervisorState)
                                If oRequest IsNot Nothing Then
                                    ' Se tiene que denegar
                                    ' En caso que no se pueda, se deniega por defecto
                                    bolRet = oRequest.ApproveRefuse(intIDPassport, False, oTask.State.Language.Translate("Requests.RejectedDate", "") & "." & oTask.State.Language.Translate("Requests.RefusedBy", "") & " SYSTEM",,, False, True)
                                    If Not bolRet Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckRequests_RejectedDate: Refused:: Request ID: " & oRequest.ID & " Employee ID: " & oRequest.IDEmployee & " : Error:: " & oRequest.State.ErrorText)
                                    End If

                                    If bolRet Then
                                        ' Generamos alerta al empleado en caso necesario
                                        Dim oNotificationState As New Robotics.Base.VTNotifications.Notifications.roNotificationState(oSupervisorState.IDPassport)
                                        Robotics.Base.VTNotifications.Notifications.roNotification.GenerateNotificationsForRequest(oRequest.ID, False, oNotificationState, True, False)
                                    End If
                                End If
                            Catch ex As Exception
                                bolRet = False
                            Finally
                                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                            End Try
                        Next
                    End If

                    ' 02. VALIDACIÓN AUTOMÁTICA DE SOLICITUDES
                    ' Obtenemos todas las solicitudes pendientes de validar de forma automatica
                    ' que hayan sobrepasado la fecha de validacion asignada,
                    ' se incluyen las solicitudes de intercambio de horario (en este caso la solicitud esta en estado OnGoing)
                    bolRet = True
                    strSQL = "@SELECT# ID FROM Requests WHERE AutomaticValidation = 1 AND ((Status=" & eRequestStatus.Pending & " AND RequestType <> " & eRequestType.ExchangeShiftBetweenEmployees & ") OR (Status=" & eRequestStatus.OnGoing & " AND RequestType = " & eRequestType.ExchangeShiftBetweenEmployees & "))" & " AND ValidationDate <= " & Any2Time(Now.Date).SQLSmallDateTime & " ORDER BY isnull(ValidationDate, RequestDate) , RequestDate "
                    tb = CreateDataTableWithoutTimeouts(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        Dim oRequest As Requests.roRequest = Nothing
                        For Each oRequestRow As DataRow In tb.Rows
                            bolRet = False

                            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
                            Try

                                Dim oSupervisorState As New Requests.roRequestState(intIDPassport)
                                oRequest = New Requests.roRequest(oRequestRow("ID"), oSupervisorState)
                                If oRequest IsNot Nothing Then
                                    ' Se intenta aprobar la solicitud
                                    bolRet = oRequest.ApproveRefuse(intIDPassport, True, oTask.State.Language.Translate("Requests.ApprovedBy", "") & " SYSTEM",,, False, True)
                                    If Not bolRet Then
                                        Select Case oRequest.State.Result
                                            Case RequestResultEnum.NoApprovePermissions, RequestResultEnum.NoApproveRefuseLevelOfAuthorityRequired
                                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckRequests_AutomaticValidation: Approved:: Request ID: " & oRequest.ID & " Employee ID: " & oRequest.IDEmployee & " : Error:: " & oRequest.State.ErrorText)
                                            Case Else
                                                ' En caso que no se pueda, se deniega por defecto
                                                bolRet = oRequest.ApproveRefuse(intIDPassport, False, oRequest.State.ErrorText & "." & oTask.State.Language.Translate("Requests.RefusedBy", "") & " SYSTEM",,, False, True)
                                                If Not bolRet Then
                                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckRequests_AutomaticValidation: Refused:: Request ID: " & oRequest.ID & " Employee ID: " & oRequest.IDEmployee & " : Error:: " & oRequest.State.ErrorText)
                                                End If
                                        End Select
                                    End If

                                    If bolRet Then
                                        ' Generamos alerta al empleado en caso necesario
                                        Dim oNotificationState As New Robotics.Base.VTNotifications.Notifications.roNotificationState(oSupervisorState.IDPassport)
                                        Robotics.Base.VTNotifications.Notifications.roNotification.GenerateNotificationsForRequest(oRequest.ID, False, oNotificationState, True, False)
                                    End If
                                End If
                            Catch ex As Exception
                                bolRet = False
                            Finally
                                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                            End Try
                        Next
                    End If
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckRequests: Invalid SYSTEM Passport")
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckRequests:Error")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CheckAutomaticRequests :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function RemoveExpiredTasks(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim oLiveTaskState = New roLiveTaskState(-1)
                Dim oParam As New AdvancedParameter.roAdvancedParameter(Robotics.Base.DTOs.AdvancedParameterType.AnalyticsPersistOnSystem.ToString(), New AdvancedParameter.roAdvancedParameterState(), Nothing)
                Dim strSQL As String = "@SELECT# * FROM sysroLiveTasks where Action = '" & roLiveTaskTypes.AnalyticsTask.ToString & "' AND DATEDIFF(DAY,TimeStamp, GETDATE()) > " & oParam.Value
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oAnalyticTaskRow As DataRow In tb.Rows
                        Dim tmpTask As New roLiveTask(oAnalyticTaskRow("ID"), oLiveTaskState)
                        'Eliminamos los que no sean BI, ya que van con otro parámetro de días
                        If tmpTask.Parameters.Item("DownloadBI") Is Nothing OrElse tmpTask.Parameters.Item("DownloadBI").ToString = "0" Then
                            tmpTask.Delete()
                        End If
                    Next
                End If

                oParam = New AdvancedParameter.roAdvancedParameter(DTOs.AdvancedParameterType.AnalyticsBIPersistOnSystem.ToString(), New AdvancedParameter.roAdvancedParameterState(), Nothing)
                strSQL = "@SELECT# * FROM sysroLiveTasks where Action = '" & roLiveTaskTypes.AnalyticsTask.ToString & "' AND Parameters LIKE '%DownloadBI%' AND DATEDIFF(DAY,TimeStamp, GETDATE()) > " & oParam.Value
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oAnalyticTaskRow As DataRow In tb.Rows
                        Dim tmpTask As New roLiveTask(oAnalyticTaskRow("ID"), oLiveTaskState)
                        'Eliminamos los que son de tipo BI
                        If tmpTask.Parameters.Item("DownloadBI").ToString = "1" Then
                            tmpTask.Delete()
                        End If
                    Next
                End If

                oParam = New AdvancedParameter.roAdvancedParameter(DTOs.AdvancedParameterType.ReportsPersistOnSystem.ToString(), New AdvancedParameter.roAdvancedParameterState(), Nothing)
                Dim iReportsPeristOnSystem As Integer = 2
                Dim iBroadcasterHangedForSure As Integer = 24

                If Not Integer.TryParse(oParam.Value, iReportsPeristOnSystem) Then
                    iReportsPeristOnSystem = 2
                End If

                strSQL = "@SELECT# * FROM sysroLiveTasks WHERE Action IN('" & roLiveTaskTypes.Import.ToString().ToUpper & "','" & roLiveTaskTypes.Export.ToString().ToUpper & "') AND DATEDIFF(DAY,TimeStamp, GETDATE()) > " & iReportsPeristOnSystem.ToString()
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oImportRow As DataRow In tb.Rows
                        Dim tmpTask As New roLiveTask(oImportRow("ID"), oLiveTaskState)
                        tmpTask.Delete()
                    Next
                End If

                strSQL = "@SELECT# * FROM sysroLiveTasks WHERE Action IN('" & roLiveTaskTypes.ReportTaskDX.ToString().ToUpper & "') AND DATEDIFF(DAY,TimeStamp, GETDATE()) > " & iReportsPeristOnSystem.ToString()
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oImportRow As DataRow In tb.Rows
                        Dim tmpTask As New roLiveTask(oImportRow("ID"), oLiveTaskState)
                        tmpTask.Delete()
                        CleanTmpTables(oImportRow("ID"))
                    Next
                End If

                strSQL = "@SELECT# * FROM sysroLiveTasks WHERE Action IN('" & roLiveTaskTypes.BroadcasterTask.ToString().ToUpper & "') AND DATEDIFF(HOUR,TimeStamp, GETDATE()) > " & iBroadcasterHangedForSure.ToString()
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oImportRow As DataRow In tb.Rows
                        Dim tmpTask As New roLiveTask(oImportRow("ID"), oLiveTaskState)
                        tmpTask.Delete()
                    Next
                End If
                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::RemoveExpiredTasks:Error")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::RemoveExpiredTasks :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Private Shared Sub CleanTmpTables(ByVal IDTask As Integer)

            ' Eliminamos todos los registros de las tablas temporales utilziadas en el informe ejecutado
            Try
                Dim strSQL As String

                strSQL = "@SELECT# Object_name(object_id) AS Nombre from sys.objects where type = 'U' and Object_name(object_id) like 'TMP%'"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        Try
                            strSQL = "@DELETE# FROM " & oRow("Nombre") & " WHERE IDReportTask=" & IDTask.ToString
                            ExecuteSql(strSQL)
                        Catch ex As Exception
                            'do nothing
                        End Try
                    Next
                End If
            Catch Ex As Exception
                'Stop
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CReportingServerNet::CleanTmpTables :", Ex)
            Finally
                '
            End Try

        End Sub

        Public Shared Function AssignTemplate(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim descriptionText As String = String.Empty
            Try
                Dim oConditions As Generic.List(Of roUserFieldCondition) = Nothing

                If roTypes.Any2Boolean(oTask.Parameters("UserFieldConditionExist")) Then
                    Dim oCondition As New roUserFieldCondition
                    oCondition.Compare = roTypes.Any2Integer(oTask.Parameters("UserFieldConditionCompare"))
                    oCondition.ValueType = roTypes.Any2Integer(oTask.Parameters("UserFieldConditionType"))
                    oCondition.Value = roTypes.Any2String(oTask.Parameters("UserFieldConditionValue"))
                    oCondition.UserField = New roUserField()
                    oCondition.UserField.FieldName = roTypes.Any2String(oTask.Parameters("UserFieldConditionUFieldName"))
                    oCondition.UserField.FieldType = roTypes.Any2Integer(oTask.Parameters("UserFieldConditionUFieldType"))
                    oConditions = New List(Of roUserFieldCondition)
                    oConditions.Add(oCondition)
                End If

                Dim _LockedDayAction As LockedDayAction = LockedDayAction.ReplaceAll
                If roTypes.Any2Boolean(oTask.Parameters("KeepBloquedDays")) Then
                    _LockedDayAction = LockedDayAction.NoReplaceAll
                End If

                Dim schedulerState As New VTBusiness.Scheduler.roSchedulerState(oTask.IDPassport)

                VTBusiness.Scheduler.roScheduleTemplate.AssignTemplate(oTask.Parameters("IDTemplate"), oTask.Parameters("IDGroup"), oConditions, oTask.Parameters("Year"), oTask.Parameters("IDShift") _
                                , DateTime.Parse(oTask.Parameters("StartShift")), oTask.Parameters("LockDays"), oTask.Parameters("IncludeSubGroups"), _LockedDayAction, LockedDayAction.NoReplaceAll, Nothing, -1, schedulerState, oTask.Parameters("KeepHolidaysDays"), False, oTask)

                If Not schedulerState.Result = SchedulerResultEnum.NoError Then
                    bolRet = False
                    descriptionText = schedulerState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                descriptionText = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::AssignTemplate :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = descriptionText}
        End Function

        Public Shared Function AssignTemplatev2(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim descriptionText As String = String.Empty

            Try
                Dim _LockedDayAction As LockedDayAction = LockedDayAction.ReplaceAll
                If roTypes.Any2Boolean(oTask.Parameters("KeepBloquedDays")) Then
                    _LockedDayAction = LockedDayAction.NoReplaceAll
                End If

                Dim strDestinEmployeeID As String() = roTypes.Any2String(oTask.Parameters("lstDestEmployees")).Split(",")
                Dim iDestinEmployeeID As New Generic.List(Of Integer)
                For Each sID As String In strDestinEmployeeID
                    ' Para
                    iDestinEmployeeID.Add(roTypes.Any2Integer(sID))
                Next

                Dim strEmployeesFilter As String = oTask.Parameters("lstEmployeesFilter")

                Dim schedulerState As New VTBusiness.Scheduler.roSchedulerState(oTask.IDPassport)

                Dim strUserFieldToMark As String = oTask.Parameters("UserFieldName")
                Dim strUserFieldValueToSet As String = oTask.Parameters("UserFieldValue")

                VTBusiness.Scheduler.roScheduleTemplate.AssignTemplatev2(oTask.Parameters("IDTemplate"), iDestinEmployeeID, oTask.Parameters("IDShift") _
                                                                           , DateTime.Parse(oTask.Parameters("StartShift")), oTask.Parameters("LockDays"), _LockedDayAction, LockedDayAction.NoReplaceAll, Nothing, -1, schedulerState, oTask.Parameters("FeastDays"), oTask.Parameters("KeepHolidaysDays"), False, oTask, strEmployeesFilter, strUserFieldToMark, strUserFieldValueToSet)

                If schedulerState.Result = SchedulerResultEnum.NoError Then
                    descriptionText = schedulerState.ResultDetail 'Sí. Por alguna razón, en el error de la tarea también se guardan detalles cuando no ha habido error ...
                Else
                    bolRet = False
                    descriptionText = schedulerState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                descriptionText = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::AssignTemplatev2 :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = descriptionText}
        End Function

        Public Shared Function AssignWeekPlan(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim descriptionText As String = String.Empty

            Try
                Dim employeeState As New Employee.roEmployeeState(oTask.IDPassport)

                Dim _LockedDayAction As LockedDayAction = LockedDayAction.ReplaceAll
                If roTypes.Any2Boolean(oTask.Parameters("KeepBloquedDays")) Then
                    _LockedDayAction = LockedDayAction.NoReplaceAll
                End If

                Dim strShifts As String = oTask.Parameters("lstWeekShifts")
                Dim strDates As String() = roTypes.Any2String(oTask.Parameters("lstWeekStartShifts")).Split(",")
                Dim strAssignments As String() = roTypes.Any2String(oTask.Parameters("lstWeekAssignments")).Split(",")

                Dim assignmentIDs As New Generic.List(Of Integer)
                For Each sID As String In strAssignments
                    assignmentIDs.Add(roTypes.Any2Integer(sID))
                Next
                Dim genericDates As New Generic.List(Of DateTime)
                For Each ostrDate As String In strDates
                    genericDates.Add(DateTime.Parse(ostrDate))
                Next


                Dim conf As String() = roTypes.Any2String(oTask.Parameters("lstEmployees")).Split("@")

                Dim strSelectorEmployees As String = conf(0)
                Dim strFeature As String = conf(1)
                Dim strFilter As String = conf(2)
                Dim strFilterUser As String = conf(3)


                Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oTask.IDPassport, strFeature, "U", Permission.Read,
                                                                                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{iTask}")
                lstAuditParameterValues.Add(oTask.ID)

                lstAuditParameterNames.Add("{EmployeeNames}")
                Dim sEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, employeeState)
                lstAuditParameterValues.Add(sEmployeeNamesForAudit)

                lstAuditParameterNames.Add("{DateIni}")
                lstAuditParameterValues.Add(DateTime.Parse(oTask.Parameters("xBeginDate")).ToShortDateString())

                lstAuditParameterNames.Add("{DateEnd}")
                lstAuditParameterValues.Add(DateTime.Parse(oTask.Parameters("xEndDate")).ToShortDateString())

                lstAuditParameterNames.Add("{Shifts}")
                Dim sShiftNamesForAudit As String = ""

                Dim strShiftIDs As String() = strShifts.Split(",")
                For Each idShift As String In strShiftIDs
                    Dim oShift As New Shift.roShift(roTypes.Any2Integer(idShift), New Shift.roShiftState(oTask.IDPassport))
                    If Not oShift Is Nothing Then
                        sShiftNamesForAudit += oShift.Name & ","
                    End If
                Next
                lstAuditParameterValues.Add(sShiftNamesForAudit)

                lstAuditParameterNames.Add("{KeepBloquedDays}")
                lstAuditParameterValues.Add(oTask.Parameters("KeepBloquedDays").ToString())

                lstAuditParameterNames.Add("{KeepHolidays}")
                lstAuditParameterValues.Add(oTask.Parameters("KeepHolidaysDays").ToString())

                Dim oAuditState As New AuditState.wscAuditState(oTask.IDPassport)
                Support.roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tSchedulerWeekly, "", lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                Dim aEmployees As New ArrayList(lstEmployees)

                roScheduler.AssignWeekShifts(aEmployees, New ArrayList(strShifts.Split(",")), genericDates, assignmentIDs, DateTime.Parse(oTask.Parameters("xBeginDate")), DateTime.Parse(oTask.Parameters("xEndDate")),
                                                                _LockedDayAction, LockedDayAction.NoReplaceAll, ShiftPermissionAction.ContinueAll, -1, Nothing, employeeState, oTask.Parameters("KeepHolidaysDays"), False, oTask)

                If Not employeeState.Result = SchedulerResultEnum.NoError Then
                    bolRet = False
                    descriptionText = employeeState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                descriptionText = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::AssignWeekPlan :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = descriptionText}
        End Function

        Public Shared Function CopyAdvancedPlanv2(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim descriptionText As String = String.Empty

            Try
                Dim oCalendarState As New roCalendarState(oTask.IDPassport)
                Dim oCalendarManager As New roCalendarManager(oCalendarState)

                oCalendarManager.CopyPlanv2(oTask.Parameters, oCalendarState)

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{ParametersValues}")
                Dim strParams As String = ""

                strParams += "lstOriginEmployees--> " & roTypes.Any2String(oTask.Parameters("lstOriginEmployees"))
                strParams += " lstDestEmployees--> " & roTypes.Any2String(oTask.Parameters("lstDestEmployees"))
                strParams += " BeginDateSource--> " & roTypes.Any2String(oTask.Parameters("BeginDateSource"))
                strParams += " EndDateSource--> " & roTypes.Any2String(oTask.Parameters("EndDateSource"))
                strParams += " FromDateDestination--> " & roTypes.Any2String(oTask.Parameters("FromDateDestination"))
                strParams += " RepeatMode--> " & roTypes.Any2String(oTask.Parameters("RepeatMode"))
                strParams += " RepeatModeValue--> " & roTypes.Any2String(oTask.Parameters("RepeatModeValue"))
                strParams += " RepeatStartMode--> " & roTypes.Any2String(oTask.Parameters("RepeatStartMode"))
                strParams += " RepeatStartModeValue--> " & roTypes.Any2String(oTask.Parameters("RepeatStartModeValue"))
                strParams += " RepeatSkipMode--> " & roTypes.Any2String(oTask.Parameters("RepeatSkipMode"))
                strParams += " RepeatSkipTimes--> " & roTypes.Any2String(oTask.Parameters("RepeatSkipTimes"))
                strParams += " RepeatSkipModeValue--> " & roTypes.Any2String(oTask.Parameters("RepeatSkipModeValue"))
                lstAuditParameterValues.Add(strParams)

                Dim oAuditState As New AuditState.wscAuditState(oTask.IDPassport)
                Support.roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tCopyPlanificationAdvanced, "", lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                If Not oCalendarState.Result = SchedulerResultEnum.NoError Then
                    bolRet = False
                    descriptionText = oCalendarState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                descriptionText = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CopyAdvancedPlanv2 :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = descriptionText}
        End Function

        Public Shared Function CopyAdvancedPlan(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim descriptionText As String = String.Empty

            Try
                Dim employeeState As New Employee.roEmployeeState(oTask.IDPassport)

                Dim strOriginEmployeeID As String() = roTypes.Any2String(oTask.Parameters("lstOriginEmployees")).Split(",")
                Dim strDestinEmployeeID As String() = roTypes.Any2String(oTask.Parameters("lstDestEmployees")).Split(",")

                Dim iOriginEmployeeID As New Generic.List(Of Integer)
                Dim iDestinEmployeeID As New Generic.List(Of Integer)

                For Each sID As String In strOriginEmployeeID
                    iOriginEmployeeID.Add(roTypes.Any2Integer(sID))
                Next

                For Each sID As String In strDestinEmployeeID
                    iDestinEmployeeID.Add(roTypes.Any2Integer(sID))
                Next

                Dim bolCopyHolidays As Boolean = False
                Dim intShiftType As ActionShiftType = ActionShiftType.AllShift
                If oTask.Parameters("CopyMainShifts") = True Or oTask.Parameters("CopyAlternativeShifts") = True Then
                    If oTask.Parameters("CopyMainShifts") AndAlso oTask.Parameters("CopyAlternativeShifts") = False Then intShiftType = ActionShiftType.PrimaryShift
                    If oTask.Parameters("CopyMainShifts") = False AndAlso oTask.Parameters("CopyAlternativeShifts") Then intShiftType = ActionShiftType.AlterShift
                    bolCopyHolidays = oTask.Parameters("CopyHolidays")
                Else
                    If oTask.Parameters("CopyHolidays") Then
                        intShiftType = ActionShiftType.HolidayShift
                    End If
                End If

                Dim bolKeepHolidayDays As Boolean = oTask.Parameters("KeepHolidaysDays")
                Dim _LockedDayAction As LockedDayAction = LockedDayAction.ReplaceAll
                If roTypes.Any2Boolean(oTask.Parameters("KeepBloquedDays")) Then
                    _LockedDayAction = LockedDayAction.NoReplaceAll
                End If

                Dim startDatePeriod As DateTime = DateTime.Parse(oTask.Parameters("xBeginDate"))
                Dim endDatePeriod As DateTime = DateTime.Parse(oTask.Parameters("xEndDate"))
                Dim strSelectedShifts() As String = roTypes.Any2String(oTask.Parameters("strSelectedShifts")).Split("_")
                Dim oListShifts As New Generic.List(Of String)
                For Each strList As String In strSelectedShifts
                    oListShifts.Add(strList)
                Next

                Dim bRet As Boolean = True

                Dim initialDays As Integer = endDatePeriod.Subtract(startDatePeriod).TotalDays + 1
                Dim totalActions As Integer = endDatePeriod.Subtract(startDatePeriod).TotalDays
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Double = 100 / totalActions

                If oTask.Parameters("CopyMainShifts") = True Or oTask.Parameters("CopyAlternativeShifts") = True Or oTask.Parameters("CopyHolidays") = True Then
                    roScheduler.CopyPlan(oListShifts, iDestinEmployeeID, startDatePeriod, endDatePeriod, intShiftType, _LockedDayAction, LockedDayAction.ReplaceAll, ShiftPermissionAction.ContinueAll,
                                        Nothing, -1, False, employeeState, False, bolKeepHolidayDays)

                    If bolCopyHolidays Then
                        roScheduler.CopyPlan(oListShifts, iDestinEmployeeID, startDatePeriod, endDatePeriod, intShiftType, _LockedDayAction, LockedDayAction.ReplaceAll, ShiftPermissionAction.ContinueAll,
                                               Nothing, -1, True, employeeState, False, )
                    End If
                End If

                If Not (bolRet AndAlso employeeState.Result = SchedulerResultEnum.NoError) Then
                    bolRet = False
                    descriptionText = employeeState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                descriptionText = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CopyAdvancedPlan :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = descriptionText}
        End Function

        Public Shared Function ValidateSignStatusDocument(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim descriptionText As String = String.Empty

            Try
                Dim GUIDDoc As String = roTypes.Any2String(oTask.Parameters("GUIDDoc"))
                Dim documentid As Integer = roTypes.Any2Integer(oTask.Parameters("DocumentID"))

                Dim docState As New roDocumentState(oTask.IDPassport)

                Dim oDocumentManager = New roDocumentManager(docState)
                bolRet = oDocumentManager.ValidateSignStatusDocument(GUIDDoc, documentid, True)

                If Not (bolRet AndAlso docState.Result = DocumentResultEnum.NoError) Then
                    bolRet = False
                    descriptionText = docState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                descriptionText = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::ValidateSignStatusDocument :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = descriptionText}
        End Function

        Public Shared Function CopyPlan(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim descriptionText As String = String.Empty

            Try
                Dim employeeState As New Employee.roEmployeeState(oTask.IDPassport)

                Dim _LockedDayAction As LockedDayAction = LockedDayAction.ReplaceAll
                If roTypes.Any2Boolean(oTask.Parameters("KeepBloquedDays")) Then
                    _LockedDayAction = LockedDayAction.NoReplaceAll
                End If

                Dim iOriginEmployeeID As Integer = roTypes.Any2Integer(oTask.Parameters("intSourceIDEmployee"))

                Dim bolCopyHolidays As Boolean = False
                Dim intShiftType As ActionShiftType = ActionShiftType.AllShift
                If oTask.Parameters("CopyMainShifts") = True Or oTask.Parameters("CopyAlternativeShifts") = True Then
                    If oTask.Parameters("CopyMainShifts") AndAlso oTask.Parameters("CopyAlternativeShifts") = False Then intShiftType = ActionShiftType.PrimaryShift
                    If oTask.Parameters("CopyMainShifts") = False AndAlso oTask.Parameters("CopyAlternativeShifts") Then intShiftType = ActionShiftType.AlterShift
                    bolCopyHolidays = oTask.Parameters("CopyHolidays")
                Else
                    If oTask.Parameters("CopyHolidays") Then
                        intShiftType = ActionShiftType.HolidayShift
                    End If
                End If

                Dim bolKeepHolidayDays As Boolean = oTask.Parameters("KeepHolidaysDays")

                Dim bRet As Boolean = True

                Dim conf As String() = roTypes.Any2String(oTask.Parameters("lstEmployees")).Split("@")
                Dim strSelectorEmployees As String = conf(0)
                Dim strFeature As String = conf(1)
                Dim strFilter As String = conf(2)
                Dim strFilterUser As String = conf(3)


                Dim lstEmployees As List(Of Integer) = roSelector.GetEmployeeList(Any2Integer(oTask.IDPassport), strFeature, "U", Nothing,
                                                                                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{iTask}")
                lstAuditParameterValues.Add(oTask.ID)

                lstAuditParameterNames.Add("{SourceEmployeeName}")
                lstAuditParameterValues.Add(roBusinessSupport.GetEmployeeName(iOriginEmployeeID, employeeState))

                lstAuditParameterNames.Add("{DestinationEmployeeName}")
                Dim sEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, employeeState)
                lstAuditParameterValues.Add(sEmployeeNamesForAudit)

                lstAuditParameterNames.Add("{DateIni}")
                lstAuditParameterValues.Add(DateTime.Parse(oTask.Parameters("xBeginDate")).ToShortDateString())

                lstAuditParameterNames.Add("{DateEnd}")
                lstAuditParameterValues.Add(DateTime.Parse(oTask.Parameters("xEndDate")).ToShortDateString())

                lstAuditParameterNames.Add("{CopyMainShifts}")
                lstAuditParameterValues.Add(oTask.Parameters("CopyMainShifts").ToString())

                lstAuditParameterNames.Add("{AlternativeShifts}")
                lstAuditParameterValues.Add(oTask.Parameters("CopyAlternativeShifts").ToString())

                lstAuditParameterNames.Add("{CopyHolidays}")
                lstAuditParameterValues.Add(oTask.Parameters("CopyHolidays").ToString())

                lstAuditParameterNames.Add("{KeepBloquedDays}")
                lstAuditParameterValues.Add(oTask.Parameters("KeepBloquedDays").ToString())

                lstAuditParameterNames.Add("{KeepHolidays}")
                lstAuditParameterValues.Add(oTask.Parameters("KeepHolidaysDays").ToString())

                Dim oAuditState As New AuditState.wscAuditState(oTask.IDPassport)
                Support.roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tCopyPlanification, "", lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                Dim totalActions As Integer = lstEmployees.Count * DateTime.Parse(oTask.Parameters("xEndDate")).Subtract(DateTime.Parse(oTask.Parameters("xBeginDate"))).TotalDays
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Integer = 100 / totalActions

                For Each iEmployeeID As Integer In lstEmployees

                    If oTask.Parameters("CopyMainShifts") = True Or oTask.Parameters("CopyAlternativeShifts") = True Or oTask.Parameters("CopyHolidays") = True Then
                        bRet = roScheduler.CopyPlan(iOriginEmployeeID, iEmployeeID, DateTime.Parse(oTask.Parameters("xBeginDate")), DateTime.Parse(oTask.Parameters("xEndDate")),
                                                      intShiftType, _LockedDayAction, LockedDayAction.NoReplaceAll, ShiftPermissionAction.ContinueAll,
                                                        Nothing, employeeState, False, False, bolKeepHolidayDays, False)

                        If bolCopyHolidays Then
                            bRet = roScheduler.CopyPlan(iOriginEmployeeID, iEmployeeID, DateTime.Parse(oTask.Parameters("xBeginDate")), DateTime.Parse(oTask.Parameters("xEndDate")),
                                                      ActionShiftType.HolidayShift, _LockedDayAction, LockedDayAction.NoReplaceAll, ShiftPermissionAction.ContinueAll,
                                                        Nothing, employeeState, False, False,, False)
                        End If
                    End If
                    oTask.Progress = oTask.Progress + stepProgress
                    oTask.Save()
                Next

                roConnector.InitTask(TasksType.DAILYSCHEDULE)

                If Not (bolRet AndAlso employeeState.Result = SchedulerResultEnum.NoError) Then
                    bolRet = False
                    descriptionText = employeeState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                descriptionText = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CopyPlan :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = descriptionText}
        End Function

        Public Shared Function DataMonitoring(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim oDataWatcher As New EOGDataWatcher
                Dim runEvery As EOGDataWatcher.RunEvery = roTypes.Any2Integer(oTask.Parameters("RunEvery"))
                bolRet = oDataWatcher.ExecuteMonitoring(runEvery)
                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DataMonitoring:Error handling data monitoring task")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::DataMonitoring :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function MassCopy(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim descriptionText As String = String.Empty

            Try
                Dim employeeState As New Employee.roEmployeeState(oTask.IDPassport)
                Dim userfieldState As New roUserFieldState(oTask.IDPassport)

                Dim CenterState As New BusinessCenter.roBusinessCenterState(oTask.IDPassport)

                Dim oContractState As New Contract.roContractState(oTask.IDPassport)

                Dim iSourceEmployee As Integer = oTask.Parameters("intSourceIDEmployee")

                Dim strUserFieldsHistory As String() = roTypes.Any2String(oTask.Parameters("userFieldHistory")).Split(",")
                Dim strUserFieldsNoHistory As String() = roTypes.Any2String(oTask.Parameters("userFieldNoHistory")).Split(",")
                Dim strAssignments As String() = roTypes.Any2String(oTask.Parameters("oAssignments")).Split(",")
                Dim bolCopyAssignments As Boolean = oTask.Parameters("ckCopyAssignments")

                Dim strCenters As String() = roTypes.Any2String(oTask.Parameters("oCenters")).Split("@")
                Dim bolCopyCenters As Boolean = oTask.Parameters("ckCopyCenters")

                Dim bolCopyUserFields As Boolean = oTask.Parameters("ckCopyUserFields")
                Dim bolCopyLabAgree As Boolean = oTask.Parameters("ckLabAgree")
                Dim bolSchedule As Boolean = oTask.Parameters("copySchedule")

                Dim xStart As DateTime = DateTime.Parse(oTask.Parameters("xStart"))
                Dim xEnd As DateTime = DateTime.Parse(oTask.Parameters("xEnd"))

                Dim bolCopyHolidays As Boolean = False
                Dim intShiftType As ActionShiftType = ActionShiftType.AllShift
                If oTask.Parameters("CopyMainShifts") = True Or oTask.Parameters("CopyAlternativeShifts") = True Then
                    If oTask.Parameters("CopyMainShifts") AndAlso oTask.Parameters("CopyAlternativeShifts") = False Then intShiftType = ActionShiftType.PrimaryShift
                    If oTask.Parameters("CopyMainShifts") = False AndAlso oTask.Parameters("CopyAlternativeShifts") Then intShiftType = ActionShiftType.AlterShift
                    bolCopyHolidays = oTask.Parameters("CopyHolidays")
                Else
                    If oTask.Parameters("CopyHolidays") Then
                        intShiftType = ActionShiftType.HolidayShift
                    End If
                End If

                Dim bolKeepHolidayDays As Boolean = oTask.Parameters("KeepHolidaysDays")
                Dim _LockedDayAction As LockedDayAction = LockedDayAction.ReplaceAll
                If roTypes.Any2Boolean(oTask.Parameters("KeepBloquedDays")) Then
                    _LockedDayAction = LockedDayAction.NoReplaceAll
                End If

                Dim lstUserFieldsNoHistory As New Generic.List(Of String)
                Dim lstUserFieldsHistory As New Generic.List(Of String)
                Dim oLabAgree As LabAgree.roLabAgree = Nothing
                Dim oAssignments As DataTable = Nothing
                Dim oCenters As DataTable = Nothing

                ' Obtenemos la definición de la table de histórico sin datos
                Dim tbUserFieldsHistory As DataTable = roEmployeeUserField.GetUserFieldHistoryDataTable(iSourceEmployee, "", userfieldState) ' EmployeeService.EmployeeServiceMethods.GetUserFieldHistoryDatatable(Me, Me.hdnIDEmployeeSource.Value, "")
                Dim tbHistory As DataTable = Nothing

                For Each strFieldName As String In strUserFieldsNoHistory
                    If strFieldName <> "" Then
                        lstUserFieldsNoHistory.Add(strFieldName)
                        tbHistory = roEmployeeUserField.GetUserFieldHistoryDataTable(iSourceEmployee, strFieldName, userfieldState)
                        If tbHistory IsNot Nothing Then
                            For Each oHistoryRow As DataRow In tbHistory.Rows
                                tbUserFieldsHistory.ImportRow(oHistoryRow)
                            Next
                        End If
                    End If
                Next

                Dim oRows As DataRow()
                For Each strValue As String In strUserFieldsHistory
                    If strValue <> "" Then
                        lstUserFieldsHistory.Add(strValue)
                        tbHistory = roEmployeeUserField.GetUserFieldHistoryDataTable(iSourceEmployee, strValue.Split("=")(0), userfieldState)
                        If tbHistory IsNot Nothing Then

                            Select Case strValue.Split("=")(1)
                                Case "1" ' Copiar valores vigente y futuros
                                    oRows = tbHistory.Select("Date <= '" & Format(Now.Date, "yyyy/MM/dd") & "'", "Date DESC")
                                    If oRows.Length > 0 Then
                                        tbUserFieldsHistory.ImportRow(oRows(0))
                                    End If
                                    oRows = tbHistory.Select("Date > '" & Format(Now.Date, "yyyy/MM/dd") & "'")
                                    For Each oHistoryRow As DataRow In oRows
                                        tbUserFieldsHistory.ImportRow(oHistoryRow)
                                    Next

                                Case "2" ' Solo copiar valores futuros
                                    oRows = tbHistory.Select("Date > '" & Format(Now.Date, "yyyy/MM/dd") & "'", "Date DESC")
                                    For Each oHistoryRow As DataRow In oRows
                                        tbUserFieldsHistory.ImportRow(oHistoryRow)
                                    Next

                            End Select
                        End If
                    End If
                Next

                If bolCopyLabAgree Then
                    Dim oContract As Contract.roContract = Contract.roContract.GetActiveContract(iSourceEmployee, oContractState, False)

                    If oContractState.Result = ContractsResultEnum.NoError Then
                        oLabAgree = oContract.LabAgree
                    End If
                End If

                If strAssignments.Length > 0 Then
                    oAssignments = Employee.roEmployeeAssignment.GetEmployeeAssignmentsDataTable(iSourceEmployee, employeeState, False)
                    For Each oAssignmentRow As DataRow In oAssignments.Rows

                        Dim bolFind As Boolean = False
                        For Each sAssignment As String In strAssignments
                            If oAssignmentRow("Name") = sAssignment Then
                                bolFind = True
                            End If
                        Next
                        If Not bolFind Then
                            oAssignmentRow.Delete()
                        End If
                    Next
                    oAssignments.AcceptChanges()
                End If

                If strCenters.Length > 0 Then
                    oCenters = BusinessCenter.roBusinessCenter.GetEmployeeBusinessCentersDataTable(CenterState, iSourceEmployee, False)
                    For Each oCenterRow As DataRow In oCenters.Rows

                        Dim bolFind As Boolean = False
                        For Each sCenter As String In strCenters
                            If oCenterRow("Name") = sCenter Then
                                bolFind = True
                            End If
                        Next
                        If Not bolFind Then
                            oCenterRow.Delete()
                        End If
                    Next
                    oCenters.AcceptChanges()
                End If



                Dim conf As String() = roTypes.Any2String(oTask.Parameters("lstDestinationIDEmployees")).Split("@")
                Dim strSelectorEmployees As String = conf(0)
                Dim strFeature As String = conf(1)
                Dim strFilter As String = conf(2)
                Dim strFilterUser As String = conf(3)


                Dim lstEmployees As List(Of Integer) = roSelector.GetEmployeeList(Any2Integer(oTask.IDPassport), strFeature, "U", Nothing,
                                                                                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{iTask}")
                lstAuditParameterValues.Add(oTask.ID)

                lstAuditParameterNames.Add("{SourceEmployeeName}")
                lstAuditParameterValues.Add(roBusinessSupport.GetEmployeeName(iSourceEmployee, employeeState))

                lstAuditParameterNames.Add("{DestinationEmployeeName}")
                Dim sEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, employeeState)
                lstAuditParameterValues.Add(sEmployeeNamesForAudit)

                lstAuditParameterNames.Add("{CopyUserFields}")
                lstAuditParameterValues.Add(bolCopyUserFields.ToString)

                lstAuditParameterNames.Add("{UserFieldsSelected}")
                lstAuditParameterValues.Add(oTask.Parameters("userFieldHistory") & "," & oTask.Parameters("userFieldNoHistory"))

                lstAuditParameterNames.Add("{CopyLabAgree}")
                lstAuditParameterValues.Add(bolCopyLabAgree.ToString)

                lstAuditParameterNames.Add("{CopyAssignments}")
                lstAuditParameterValues.Add(bolCopyAssignments.ToString)

                lstAuditParameterNames.Add("{Assignments}")
                lstAuditParameterValues.Add(oTask.Parameters("oAssignments"))

                lstAuditParameterNames.Add("{BusinessCenters}")
                lstAuditParameterValues.Add(oTask.Parameters("oCenters"))

                lstAuditParameterNames.Add("{CopySchedule}")
                lstAuditParameterValues.Add(bolSchedule.ToString)

                lstAuditParameterNames.Add("{DateIni}")
                lstAuditParameterValues.Add(xStart.ToShortDateString())

                lstAuditParameterNames.Add("{DateEnd}")
                lstAuditParameterValues.Add(xEnd.ToShortDateString())

                lstAuditParameterNames.Add("{PrimaryShifts}")
                lstAuditParameterValues.Add(oTask.Parameters("CopyMainShifts").ToString())

                lstAuditParameterNames.Add("{AlternativeShifts}")
                lstAuditParameterValues.Add(oTask.Parameters("CopyAlternativeShifts").ToString())

                lstAuditParameterNames.Add("{CopyHolidays}")
                lstAuditParameterValues.Add(oTask.Parameters("CopyHolidays").ToString())

                lstAuditParameterNames.Add("{KeepBloquedDays}")
                lstAuditParameterValues.Add(oTask.Parameters("KeepHolidaysDays").ToString())

                lstAuditParameterNames.Add("{KeepHolidays}")
                lstAuditParameterValues.Add(oTask.Parameters("KeepBloquedDays").ToString())

                Dim oAuditState As New AuditState.wscAuditState(oTask.IDPassport)
                Support.roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tEmployee, "", lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                Dim totalActions As Integer = lstEmployees.Count
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Double = (100 / totalActions)

                Dim bolCallBroadcaster As Boolean = False

                For Each intIDEmployee As Integer In lstEmployees

                    If iSourceEmployee <> intIDEmployee Then
                        Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(intIDEmployee, employeeState, False)

                        If oEmployee IsNot Nothing Then
                            ' Copiar campos de la ficha
                            If bolCopyUserFields AndAlso tbUserFieldsHistory IsNot Nothing AndAlso tbUserFieldsHistory.Rows.Count > 0 Then
                                ' Borramos los valores a partir de la fecha a copiar en los campos con histórico
                                For Each strValue As String In lstUserFieldsHistory
                                    Dim oFieldsHistoryRows As DataRow() = tbUserFieldsHistory.Select("FieldName = '" & strValue.Split("=")(0).Replace("'", "''") & "'", "Date ASC")
                                    If oFieldsHistoryRows.Length > 0 Then
                                        roEmployeeUserField.DeleteUserFieldHistory(intIDEmployee, strValue.Split("=")(0), oFieldsHistoryRows(0).Item("Date"), True, userfieldState)
                                    End If
                                Next
                                ' Borramos todos los valores de los campos sin histórico
                                For Each strFieldName As String In lstUserFieldsNoHistory
                                    roEmployeeUserField.DeleteUserFieldHistory(intIDEmployee, strFieldName, New Date(1900, 1, 1), True, userfieldState)
                                Next
                                ' Guardamos los valores del historico de todos los campos a copiar
                                For Each oFieldsHistoryRows As DataRow In tbUserFieldsHistory.Rows
                                    If oFieldsHistoryRows.RowState = DataRowState.Unchanged Then
                                        oFieldsHistoryRows.SetAdded() ' Marcamos la fila como nueva para que se guarde el cambio
                                    End If
                                Next
                                If Not roEmployeeUserField.SaveUserFieldHistory(intIDEmployee, tbUserFieldsHistory, userfieldState) Then

                                    'lstErrors.Add(Me.Language.Translate("CopyStep.Error.UserFields", Me.DefaultScope, Params))
                                    'If employeeState.ErrorText <> "" Then
                                    '    lstErrors.Add(employeeState.ErrorText)
                                    'End If

                                End If

                            End If

                            ' Planificación
                            If bolSchedule Then
                                If oTask.Parameters("CopyMainShifts") = True Or oTask.Parameters("CopyAlternativeShifts") = True Or oTask.Parameters("CopyHolidays") = True Then
                                    roScheduler.CopyPlan(iSourceEmployee, intIDEmployee, xStart, xEnd,
                                                          intShiftType, _LockedDayAction, LockedDayAction.NoReplaceAll, ShiftPermissionAction.ContinueAll,
                                                            Nothing, employeeState, False, False, bolKeepHolidayDays)

                                    If bolCopyHolidays Then
                                        roScheduler.CopyPlan(iSourceEmployee, intIDEmployee, xStart, xEnd,
                                                                      ActionShiftType.HolidayShift, _LockedDayAction, LockedDayAction.NoReplaceAll, ShiftPermissionAction.ContinueAll,
                                                                        Nothing, employeeState, False, False)
                                    End If
                                End If
                            End If

                            If bolCopyLabAgree Then
                                oContractState.Result = ContractsResultEnum.NoError
                                Dim oContract As Contract.roContract = Contract.roContract.GetActiveContract(intIDEmployee, oContractState, False)
                                If oContractState.Result = ContractsResultEnum.NoError Then
                                    oContract.LabAgree = oLabAgree
                                    oContract.Save()
                                End If
                            End If

                            If bolCopyAssignments AndAlso oAssignments IsNot Nothing AndAlso oAssignments.Rows.Count > 0 Then
                                Dim oActualAssignments = Employee.roEmployeeAssignment.GetEmployeeAssignmentsDataTable(intIDEmployee, employeeState, False) 'EmployeeService.EmployeeServiceMethods.GetAssignments(Me, intIDEmployee, False)
                                Dim oAssignmentRows As DataRow()
                                For Each oAssignmentRow As DataRow In oAssignments.Rows
                                    oAssignmentRows = oActualAssignments.Select("IDAssignment = " & oAssignmentRow("IDAssignment"), "")
                                    If oAssignmentRows.Length = 0 Then
                                        oActualAssignments.ImportRow(oAssignmentRow)
                                    ElseIf oAssignmentRows.Length > 0 Then
                                        oAssignmentRows(0)("Suitability") = oAssignmentRow("Suitability")
                                    End If
                                Next

                                Dim olstAssignments As New Generic.List(Of Employee.roEmployeeAssignment)
                                For Each oNewAssignmentRow As DataRow In oActualAssignments.Rows
                                    Dim oEmployeeAssignment As New Employee.roEmployeeAssignment
                                    If oNewAssignmentRow.RowState <> DataRowState.Deleted AndAlso Not IsDBNull(oNewAssignmentRow("IDAssignment")) Then
                                        oEmployeeAssignment.IDEmployee = intIDEmployee
                                        oEmployeeAssignment.IDAssignment = oNewAssignmentRow("IDAssignment")
                                        oEmployeeAssignment.Suitability = roTypes.Any2Integer(oNewAssignmentRow("Suitability"))
                                        olstAssignments.Add(oEmployeeAssignment)
                                    End If
                                Next

                                Employee.roEmployeeAssignment.SaveEmployeeAssignments(intIDEmployee, olstAssignments, employeeState, False)
                            End If

                            If bolCopyCenters AndAlso oCenters IsNot Nothing AndAlso oCenters.Rows.Count > 0 Then
                                Dim oActualCenters = BusinessCenter.roBusinessCenter.GetEmployeeBusinessCentersDataTable(CenterState, intIDEmployee, False)
                                Dim oCenterRows As DataRow()
                                For Each oCenterRow As DataRow In oCenters.Rows
                                    oCenterRows = oActualCenters.Select("IDCenter = " & oCenterRow("IDCenter"), "")
                                    If oCenterRows.Length = 0 Then
                                        oActualCenters.ImportRow(oCenterRow)
                                    ElseIf oCenterRows.Length > 0 Then
                                        oCenterRows(0)("BeginDate") = oCenterRow("BeginDate")
                                        oCenterRows(0)("EndDate") = oCenterRow("EndDate")
                                    End If
                                Next
                                Dim oCentersSet As New DataSet
                                oCentersSet.Tables.Add(oActualCenters)
                                If BusinessCenter.roBusinessCenter.SaveEmployeeCenters(intIDEmployee, oCentersSet, CenterState, False, False) Then
                                    bolCallBroadcaster = True
                                End If
                            End If
                        End If
                    End If

                    oTask.Progress = oTask.Progress + stepProgress
                    oTask.Save()
                Next

                If bolRet AndAlso employeeState.Result = SchedulerResultEnum.NoError Then
                    If bolCallBroadcaster Then roConnector.InitTask(TasksType.BROADCASTER)
                Else
                    bolRet = False
                    descriptionText = employeeState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                descriptionText = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MassCopy :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = descriptionText}
        End Function

        Public Shared Function MassIncidences(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strError As String = ""

            Try
                Dim strRows As String() = roTypes.Any2String(oTask.Parameters("lstRows")).Split("#")

                Dim totalActions As Integer = strRows.Length
                If totalActions = 0 Then totalActions = 1

                Dim oDailycause As New Cause.roDailyCauseList
                Dim oDailyCauseState As New Cause.roCauseState(oTask.IDPassport)

                bolRet = oDailycause.SaveMassCauses(strRows, oDailyCauseState, oTask, strError, True)

                If Not (bolRet AndAlso strError.Length = 0) Then
                    bolRet = False
                End If
            Catch ex As Exception
                bolRet = False
                strError = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MassIncidences :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}
        End Function

        Public Shared Function MassCauses(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strError As String = ""

            Try
                Dim employeeState As New Employee.roEmployeeState(oTask.IDPassport)

                Dim bolCompletedDay As Boolean = oTask.Parameters("CompletedDay")
                Dim xStart As DateTime = DateTime.Parse(oTask.Parameters("xBeginDate"))
                Dim xEnd As DateTime = DateTime.Parse(oTask.Parameters("xEndDate"))

                Dim intIDCause As Integer = oTask.Parameters("IDCause")





                Dim conf As String() = roTypes.Any2String(oTask.Parameters("lstEmployees")).Split("@")
                Dim strSelectorEmployees As String = conf(0)
                Dim strFeature As String = conf(1)
                Dim strFilter As String = conf(2)
                Dim strFilterUser As String = conf(3)

                Dim lstEmployees As List(Of Integer) = roSelector.GetEmployeeList(Any2Integer(oTask.IDPassport), strFeature, "U", Nothing,
                                                                                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)

                Dim totalActions As Integer = lstEmployees.Count
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Double = 100 / totalActions

                ' Obtenemos la fecha de congelacion
                Dim xFreezingDate As Date = roParameters.GetFirstDate()

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{iTask}")
                lstAuditParameterValues.Add(oTask.ID)

                lstAuditParameterNames.Add("{DestinationEmployeeName}")
                Dim sEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, employeeState)
                lstAuditParameterValues.Add(sEmployeeNamesForAudit)

                lstAuditParameterNames.Add("{DateIni}")
                lstAuditParameterValues.Add(xStart.ToShortDateString())

                lstAuditParameterNames.Add("{DateEnd}")
                lstAuditParameterValues.Add(xEnd.ToShortDateString())

                lstAuditParameterNames.Add("{CompletedDays}")
                lstAuditParameterValues.Add(bolCompletedDay.ToString)

                Dim oCurrentCause As New Cause.roCause(intIDCause, New Cause.roCauseState(oTask.IDPassport))
                lstAuditParameterNames.Add("{Cause}")
                lstAuditParameterValues.Add(oCurrentCause.Name)

                Dim oAuditState As New AuditState.wscAuditState(oTask.IDPassport)
                Support.roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tAssignCause, "", lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                For Each intIDEmployee As Integer In lstEmployees

                    ' Asignar justificacion
                    If Not roScheduler.AssignCause(intIDEmployee, xStart, xEnd, intIDCause,
                                                  bolCompletedDay, xFreezingDate, employeeState, False) Then
                        strError = strError & vbNewLine & employeeState.ErrorText
                        bolRet = False
                    Else
                        If employeeState.Result <> EmployeeResultEnum.NoError Then
                            strError = strError & vbNewLine & roBusinessSupport.GetEmployeeName(intIDEmployee, employeeState) & ": " & employeeState.ErrorText
                        End If
                    End If

                    oTask.Progress = oTask.Progress + stepProgress
                    oTask.Save()
                Next

            Catch ex As Exception
                bolRet = False
                strError = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MassCauses :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}

        End Function

        Public Shared Function CompleteTasksAndProjects(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strError As String = ""

            Try
                Dim strIntTasks As String = roTypes.Any2String(roTypes.Any2String(oTask.Parameters("Tasks")))
                Dim strProjects As String = roTypes.Any2String(roTypes.Any2String(oTask.Parameters("Projects")))

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{iTask}")
                lstAuditParameterValues.Add(oTask.ID)

                lstAuditParameterNames.Add("{strProjects}")
                If strProjects = String.Empty Then
                    lstAuditParameterValues.Add("--")
                Else
                    lstAuditParameterValues.Add(strProjects.Replace("'", ""))
                End If

                Dim oTaskState As New Task.roTaskState(oTask.IDPassport)
                Dim sTasksNamesForAudit As String = ""
                If strIntTasks <> String.Empty Then
                    For Each idTask As Integer In strIntTasks.Split(",")
                        sTasksNamesForAudit &= Task.roTask.GetTaskNameById(idTask, oTaskState) & ","
                    Next

                    If sTasksNamesForAudit <> String.Empty Then sTasksNamesForAudit = sTasksNamesForAudit.Substring(0, sTasksNamesForAudit.Length - 1)
                Else
                    sTasksNamesForAudit = "--"
                End If

                lstAuditParameterNames.Add("{strTaskNames}")
                lstAuditParameterValues.Add(sTasksNamesForAudit)

                Dim oAuditState As New AuditState.wscAuditState(oTask.IDPassport)
                Support.roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tCompleteTasks, "", lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                Dim totalActions As Integer = strIntTasks.Split(",").Length + strProjects.Split(",").Length
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Double = 100 / totalActions

                Dim tmpError As String = String.Empty
                If strIntTasks <> String.Empty Then
                    For Each iTaskID As Integer In strIntTasks.Split(",")
                        bolRet = True

                        If iTaskID <> 0 Then
                            If Not Task.roTask.CompleteTask(iTaskID, tmpError, oTaskState) Then strError = strError & tmpError & ","
                        End If

                        If bolRet Then oTask.Progress = oTask.Progress + stepProgress
                        oTask.Save()
                    Next
                End If

                If strProjects <> String.Empty Then
                    For Each strPrjectName As String In strProjects.Split(",")
                        If Not Task.roTask.CompleteTaskFromProject(strPrjectName.Replace("'", ""), tmpError, oTaskState) Then strError = strError & tmpError & ","
                        If bolRet Then oTask.Progress = oTask.Progress + stepProgress
                        oTask.Save()
                    Next
                End If

                If strError.Length > 0 Then
                    strError = strError.Substring(0, strError.Length - 1)
                End If

            Catch ex As Exception
                bolRet = False
                strError = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MassIncidences :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}
        End Function

        Public Shared Function MessageEmployees(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strError As String = ""

            Try
                Dim employeeState As New Employee.roEmployeeState(oTask.IDPassport)

                Dim conf As String() = roTypes.Any2String(oTask.Parameters("lstEmployees")).Split("@")
                Dim strSelectorEmployees As String = conf(0)
                Dim strFeature As String = conf(1)
                Dim strFilter As String = conf(2)
                Dim strFilterUser As String = conf(3)


                Dim lstEmployees As List(Of Integer) = roSelector.GetEmployeeList(Any2Integer(oTask.IDPassport), strFeature, "U", Nothing,
                                                                                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)

                Dim message As String = roTypes.Any2String(oTask.Parameters("message"))
                Dim oScheduleStr As String = roTypes.Any2String(oTask.Parameters("schedule"))

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{iTask}")
                lstAuditParameterValues.Add(oTask.ID)

                lstAuditParameterNames.Add("{message}")
                lstAuditParameterValues.Add(message)

                lstAuditParameterNames.Add("{schedule}")
                lstAuditParameterValues.Add(oScheduleStr)

                lstAuditParameterNames.Add("{DestinationEmployeeName}")
                Dim sEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, employeeState)
                lstAuditParameterValues.Add(sEmployeeNamesForAudit)

                Dim oAuditState As New AuditState.wscAuditState(oTask.IDPassport)
                Support.roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tMessageEmployees, "", lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                Dim totalActions As Integer = lstEmployees.Count
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Double = 100 / totalActions

                Dim oEmpState As New Employee.roEmployeeState

                Dim bolNotify As Boolean = False

                For Each iEmployee As String In lstEmployees

                    Dim oTMessages As New roTerminalMessage
                    oTMessages.ForAllEmployees = 0
                    oTMessages.Schedule = oScheduleStr
                    oTMessages.Message = message

                    bolRet = roTerminalMessage.SaveTerminalMessage(iEmployee, oTMessages, oEmpState)
                    If bolRet Then
                        bolNotify = True
                        oTask.Progress = oTask.Progress + stepProgress
                    End If

                    oTask.Save()
                Next

                If bolNotify Then
                    Extensions.roConnector.InitTask(TasksType.BROADCASTER)
                End If

                If Not (bolRet AndAlso strError.Length = 0) Then
                    bolRet = False
                End If
            Catch ex As Exception
                bolRet = False
                strError = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MassIncidences :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}
        End Function

        Public Shared Function MassAssignCenters(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strError As String = ""
            Try
                Dim strRows As String() = roTypes.Any2String(oTask.Parameters("lstRows")).Split("#")

                Dim totalActions As Integer = strRows.Length
                If totalActions = 0 Then totalActions = 1

                Dim oDailycause As New Cause.roDailyCauseList
                Dim oDailyCauseState As New Cause.roCauseState(oTask.IDPassport)
                bolRet = oDailycause.SaveMassAssignCenters(strRows, oDailyCauseState, oTask, strError, True)

                If Not (bolRet AndAlso strError.Length = 0) Then
                    bolRet = False
                End If
            Catch ex As Exception
                bolRet = False
                strError = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MassAssignCenters :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}
        End Function

        Public Shared Function MassLockDate(ByVal oTask As roLiveTask) As BaseTaskResult
            '
            ' Asignamos una fecha de cierre personalizada o bien aplicamos la fecha de cierre global
            '
            Dim bolRet As Boolean = True
            Dim strError As String = ""

            Try
                Dim EmployeeState As New Employee.roEmployeeState(oTask.IDPassport)
                Dim bolApplyLockDateGlobal As Boolean = oTask.Parameters("ckApplyLockDateGlobal")
                Dim xLockDate As DateTime = DateTime.Parse(oTask.Parameters("xLockDate"))

                Dim conf As String() = roTypes.Any2String(oTask.Parameters("lstDestinationIDEmployees")).Split("@")
                Dim strSelectorEmployees As String = conf(0)
                Dim strFeature As String = conf(1)
                Dim strFilter As String = conf(2)
                Dim strFilterUser As String = conf(3)

                Dim lstEmployees As List(Of Integer) = roSelector.GetEmployeeList(Any2Integer(oTask.IDPassport), strFeature, "U", Nothing,
                                                                                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{iTask}")
                lstAuditParameterValues.Add(oTask.ID)

                lstAuditParameterNames.Add("{DestinationEmployeeName}")
                Dim sEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, EmployeeState)
                lstAuditParameterValues.Add(sEmployeeNamesForAudit)

                lstAuditParameterNames.Add("{ApplyLockDateGlobal}")
                lstAuditParameterValues.Add(bolApplyLockDateGlobal.ToString)

                lstAuditParameterNames.Add("{LockDate}")
                lstAuditParameterValues.Add(xLockDate.ToShortDateString())

                Dim oAuditState As New AuditState.wscAuditState(oTask.IDPassport)
                Support.roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tLockDate, "", lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                Dim totalActions As Integer = lstEmployees.Count
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Double = (100 / totalActions)

                For Each intIDEmployee As Integer In lstEmployees
                    Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(intIDEmployee, EmployeeState, False)
                    If oEmployee IsNot Nothing Then
                        roBusinessSupport.SaveEmployeeLockDate(oEmployee.ID, xLockDate, IIf(bolApplyLockDateGlobal, False, True), EmployeeState)
                    End If

                    oTask.Progress = oTask.Progress + stepProgress
                    oTask.Save()
                Next

                Extensions.roConnector.InitTask(TasksType.MOVES)

                If Not (bolRet AndAlso EmployeeState.Result = SchedulerResultEnum.NoError) Then
                    bolRet = False
                    strError = EmployeeState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                strError = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MassLockDate :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}
        End Function

        Public Shared Function ExecuteAIPlannerTask(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strError As String = ""

            Try

                Dim oAIPlannerState As New roAIPlannerState(oTask.IDPassport)
                Dim oAIPlannerManager As New roAIPlannerManager(oAIPlannerState)

                Dim oBudget As roBudget = Nothing

                Dim oBudgetState As New roBudgetState(oTask.IDPassport)
                Dim oBudgetManager As New roBudgetManager(oBudgetState)

                Dim beginPeriod As Date = DateTime.Parse(oTask.Parameters("ScheduleBeginDate"))
                Dim endPeriod As Date = DateTime.Parse(oTask.Parameters("ScheduleEndDate"))
                Dim iIdOrgChartFilter As Integer = roTypes.Any2Integer(oTask.Parameters("IdOrgChartNode"))
                Dim pUnitFilter As String = roTypes.Any2String(oTask.Parameters("UnitFilter"))

                oBudget = oBudgetManager.Load(beginPeriod, endPeriod, iIdOrgChartFilter, BudgetView.Planification, BudgetDetailLevel.Daily, True,, pUnitFilter)
                Dim oAISolution As roAIProblemSolution = New roAIProblemSolution
                bolRet = oAIPlannerManager.SolveProblem(oBudget, oAISolution)

                ' Miro si debo añadir fichero de definición de problema
                Dim oParam As New Common.AdvancedParameter.roAdvancedParameter(DTOs.AdvancedParameterType.CustomizationLogEnabled.ToString, New AdvancedParameter.roAdvancedParameterState(), Nothing)
                If Not (roTypes.Any2Boolean(oParam.Value)) Then oAISolution.file = ""
                strError = Robotics.VTBase.roJSONHelper.SerializeNewtonSoft(oAISolution)
                If Not (bolRet AndAlso (oAIPlannerManager.State.Result = roAIPlannerState.ResultEnum.NoError OrElse oAIPlannerManager.State.Result = roAIPlannerState.ResultEnum.SomeExtraEmployeeRequired)) Then
                    bolRet = False
                End If
            Catch ex As Exception
                bolRet = False
                strError = ex.Message
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CopyPlan :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}
        End Function

        Public Shared Function PurgeNotifications(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Try
                ' Reseteamos el estado del empleado para que lo muestre como fuera de la oficina
                Dim strSQL As String = "@UPDATE# EmployeeStatus SET BeginMandatory = NULL, StartLimit = NULL WHERE IDEmployee IN (@SELECT# IDEmployee FROM sysrovwAllEmployeeGroups WHERE CurrentEmployee = 0) AND BeginMandatory IS NOT NULL"
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                ' Borramos sus notificaciones de empleado ausente
                strSQL = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification IN (@SELECT# ID FROM Notifications WHERE idtype IN (15, 13)) AND Key1Numeric IN (@SELECT# IDEmployee FROM sysrovwAllEmployeeGroups WHERE CurrentEmployee = 0)"
                bolRet = (bolRet AndAlso ExecuteSqlWithoutTimeOut(strSQL))

                'Borramos alertas de fichajes impares para dias en que no se tiene contrato
                strSQL = "@DELETE# sysroNotificationTasks " &
                                " WHERE IDNotification In (@SELECT# ID FROM Notifications WHERE idtype IN (19, 35)) " &
                                " AND id Not IN (" &
                                "    @SELECT# id from sysroNotificationTasks nt" &
                                "    INNER JOIN EmployeeContracts ON IDEmployee=nt.Key1Numeric " &
                                "    WHERE Key3DateTime between BeginDate AND EndDate )"
                ExecuteSqlWithoutTimeOut(strSQL)

                ' Tenemos que borrar todos los datos de los contratos finalizados para evitar que se vuelvan a generar notificaciones.
                If roBusinessSupport.GetCustomizationCode().ToUpper = "TAIF" Then
                    Dim strContractsSQL As String = "@SELECT# e1.IDEmployee, e1.EndDate As StartPeriod, e2.BeginDate as endPeriod FROM EmployeeContracts e1 INNER JOIN EmployeeContracts e2 " &
                                                    " ON e1.IDEmployee = e2.IDEmployee AND e2.BeginDate > e1.EndDate " &
                                                    " AND e2.BeginDate = (@SELECT# TOP 1 BeginDate FROM EmployeeContracts e3 WHERE e3.IDEmployee = e1.IDEmployee AND e3.BeginDate > e1.EndDate ORDER BY BeginDate ASC)" &
                                                    " WHERE e1.EndDate > DATEADD(yyyy,-1,getdate())"

                    Dim dtcontracts As DataTable = CreateDataTable(strContractsSQL, )

                    If dtcontracts IsNot Nothing AndAlso dtcontracts.Rows.Count > 0 Then
                        Dim TablesDelete As String() = {"DailyAccruals", "DailyCauses", "DailyIncidences", "DailySchedule"}

                        For Each oContract As DataRow In dtcontracts.Rows

                            strSQL = "@DELETE# FROM sysroRemarks WHERE ID IN (@SELECT# Remarks FROM DailySchedule WHERE Remarks IS NOT NULL AND IDEmployee =" & oContract("IDEmployee") &
                                        " AND Date>" & Any2Time(oContract("StartPeriod")).SQLSmallDateTime &
                                        " AND Date<" & Any2Time(oContract("endPeriod")).SQLSmallDateTime & ")"
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                            For Each strTable As String In TablesDelete
                                strSQL = "@DELETE# FROM " & strTable & " WHERE IDEmployee=" & roTypes.Any2String(oContract("IDEmployee")) &
                                         " AND Date>" & Any2Time(oContract("StartPeriod")).SQLSmallDateTime &
                                         " AND Date<" & Any2Time(oContract("endPeriod")).SQLSmallDateTime
                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                            Next
                        Next
                    End If
                End If

                ' Marcamos para reenvío las notificaciones con repetición (IDType = 50 Documento Pendiente y IDType = 70 Día sin justificar)
                Try
                    strSQL = "@UPDATE# sysroNotificationTasks SET Executed = 0 where Executed =  1 AND Repetition IS NOT NULL AND NextRepetition IS NOT NULL AND  DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) >= NextRepetition"
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                Catch ex As Exception
                End Try
                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::PurgueNotifications:Error")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::PurgeNotifications :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function DeleteOldPhotos(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Try
                Dim oParameters As New roParameters("OPTIONS", True)
                Dim oParam As Object = oParameters.Parameter(Parameters.PhotosKeepPeriod)

                Dim oKeepDays As Integer = 15
                Try
                    oKeepDays = roTypes.Any2Integer(oParam)
                Catch ex As Exception
                    oKeepDays = 15
                End Try

                Dim deleteLimit As Date = DateTime.Now.Date.AddDays(oKeepDays * -1)

                ' Fotos en PunchesCaptures (legacy)
                Dim strDeleteSQL As String = "@DELETE# FROM PunchesCaptures where IDPunch in ( " &
                                                 "@SELECT# top 100 IDPunch from PunchesCaptures inner join Punches on PunchesCaptures.IDPunch = Punches.ID and Punches.ShiftDate < " & roTypes.Any2Time(deleteLimit).SQLSmallDateTime & ")"

                Dim strCountSQL As String = "@SELECT# count(*) from PunchesCaptures inner join Punches on PunchesCaptures.IDPunch = Punches.ID and Punches.ShiftDate < " & roTypes.Any2Time(deleteLimit).SQLSmallDateTime

                While (roTypes.Any2Integer(ExecuteScalar(strCountSQL)) > 0 AndAlso bolRet)
                    bolRet = ExecuteSqlWithTimeOut(strDeleteSQL, 300)
                End While

                ' Fotos en Azure
                Dim strSQL As String = String.Empty
                strSQL = "@SELECT# ID FROM Punches WHERE PhotoOnAzure = 1 AND ShiftDate < " & roTypes.Any2Time(deleteLimit).SQLSmallDateTime
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oPunchRow As DataRow In tb.Rows
                        Azure.RoAzureSupport.DeletePunchPhotoFile(roTypes.Any2Integer(oPunchRow("ID")))
                    Next
                    strSQL = "@UPDATE# Punches SET HasPhoto = 0, PhotoOnAzure = 0 WHERE PhotoOnAzure = 1 AND ShiftDate < " & roTypes.Any2Time(deleteLimit).SQLSmallDateTime
                    ExecuteSql(strSQL)
                End If
                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldPhotos:SQL error deleting photos on DB")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::DeleteOldPhotos :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function DeleteOldPunches(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try

                'Borramos el historial de conexiones de mas de 3 meses de antigüedad
                Try
                    Dim oKeepMonths As Integer = 3
                    Dim deleteHistoryLimit As Date = DateTime.Now.Date.AddMonths(oKeepMonths * -1)

                    Dim strDeleteHistorySQL = "@DELETE# FROM sysroConnectionHistory where EventDateTime < " & roTypes.Any2Time(deleteHistoryLimit).SQLSmallDateTime

                    ExecuteSqlWithTimeOut(strDeleteHistorySQL, 300)
                Catch Ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roEOGManager:::DeleteOldPunches::Delete sysroConnectionHistory", Ex)
                End Try


                Dim intIDPassport As Integer = roConstants.GetSystemUserId()

                If intIDPassport > 0 Then
                    Dim oSupervisorState As New Requests.roRequestState(intIDPassport)

                    Dim requestOutsideContractSQL = "@SELECT# Requests.ID From Requests with (nolock) " &
                           " LEFT JOIN sysrovwCurrentOrFutureEmployeePeriod with (nolock) on Requests.IDEmployee = sysrovwCurrentOrFutureEmployeePeriod.IDEmployee " &
                           " WHERE sysrovwCurrentOrFutureEmployeePeriod.IDEmployee Is NULL AND Requests.Status = " & eRequestStatus.Pending &
                           " AND NOT EXISTS (@SELECT# 1 From EmployeeContracts with (nolock) WHERE EmployeeContracts.IDEmployee = Requests.IDEmployee AND Requests.Date1 BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate)"

                    Dim tbEmployeesWithoutContractAndRequestsOutsideIt = CreateDataTable(requestOutsideContractSQL)
                    If tbEmployeesWithoutContractAndRequestsOutsideIt IsNot Nothing AndAlso tbEmployeesWithoutContractAndRequestsOutsideIt.Rows.Count > 0 Then
                        For Each oRequestRow As DataRow In tbEmployeesWithoutContractAndRequestsOutsideIt.Rows
                            Dim oRequest = New Requests.roRequest(oRequestRow("ID"), oSupervisorState)
                            If oRequest IsNot Nothing Then
                                bolRet = oRequest.ApproveRefuse(intIDPassport, False, oTask.State.Language.Translate("Requests.ClosedOutOfContract", "") & "." & oTask.State.Language.Translate("Requests.RefusedBy", "") & " SYSTEM",,, False, True)
                                If Not bolRet Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldPunches::Refused:: Request ID: " & oRequest.ID & " Employee ID: " & oRequest.IDEmployee & " : Error:: " & oRequest.State.ErrorText)
                                End If
                            End If
                        Next
                    End If
                End If

                Dim oKeepYears As Integer = 4
                Dim deleteLimit As Date = DateTime.Now.Date.AddYears(oKeepYears * -1)

                Dim strDeleteSQL = "@DELETE# FROM Punches where ID in ( " &
                                       "@SELECT# top 100 Punches.ID from Punches " &
                                            " INNER JOIN Employees on Punches.IDEmployee = Employees.ID and Employees.HasForgottenRight = 1 and Punches.ShiftDate < " & roTypes.Any2Time(deleteLimit).SQLSmallDateTime &
                                            " LEFT join [sysrovwCurrentOrFutureEmployeePeriod] on Employees.ID = [sysrovwCurrentOrFutureEmployeePeriod].IDEmployee " &
                                            " WHERE [sysrovwCurrentOrFutureEmployeePeriod].IDEmployee IS NULL)"

                Dim strCountSQL As String = "@SELECT# count(*) from Punches " &
                                            " INNER JOIN Employees on Punches.IDEmployee = Employees.ID and Employees.HasForgottenRight = 1 and Punches.ShiftDate < " & roTypes.Any2Time(deleteLimit).SQLSmallDateTime &
                                            " LEFT join [sysrovwCurrentOrFutureEmployeePeriod] on Employees.ID = [sysrovwCurrentOrFutureEmployeePeriod].IDEmployee " &
                                            " WHERE [sysrovwCurrentOrFutureEmployeePeriod].IDEmployee IS NULL"

                Dim strUpdatedEmployeesSQL As String = "@SELECT# distinct Employees.ID, LockDate from Punches " &
                                            " INNER JOIN Employees on Punches.IDEmployee = Employees.ID and Employees.HasForgottenRight = 1 and Punches.ShiftDate < " & roTypes.Any2Time(deleteLimit).SQLSmallDateTime &
                                            " LEFT join [sysrovwCurrentOrFutureEmployeePeriod] on Employees.ID = [sysrovwCurrentOrFutureEmployeePeriod].IDEmployee " &
                                            " WHERE [sysrovwCurrentOrFutureEmployeePeriod].IDEmployee IS NULL"

                Dim tbUpdatedEmployees = CreateDataTable(strUpdatedEmployeesSQL)

                Dim removedPunchesInIteration As Integer = roTypes.Any2Integer(ExecuteScalar(strCountSQL))
                Dim removedPunches As Integer = 0

                While (removedPunchesInIteration > 0 AndAlso bolRet)
                    removedPunches = removedPunches + removedPunchesInIteration
                    bolRet = ExecuteSqlWithTimeOut(strDeleteSQL, 300)
                    removedPunchesInIteration = roTypes.Any2Integer(ExecuteScalar(strCountSQL))
                End While

                If bolRet Then
                    If tbUpdatedEmployees IsNot Nothing AndAlso tbUpdatedEmployees.Rows.Count > 0 Then
                        For Each oEmployeeRow As DataRow In tbUpdatedEmployees.Rows
                            If Any2DateTime(oEmployeeRow("LockDate")) < deleteLimit Then
                                Dim oEmployeeState As New VTEmployees.Employee.roEmployeeState(oEmployeeRow("ID"))
                                Dim oEmployee As VTEmployees.Employee.roEmployee = VTEmployees.Employee.roEmployee.GetEmployee(oEmployeeRow("ID"), oEmployeeState, False)
                                If oEmployee IsNot Nothing Then
                                    roBusinessSupport.SaveEmployeeLockDate(oEmployee.ID, deleteLimit, True, oEmployeeState, True)
                                End If
                            End If
                        Next
                    End If

                    ' Borramos tambien el backup de los punches anteriores a 31 dias ya que pueden llegar bastantes GB
                    strCountSQL = "@SELECT# COUNT(*) FROM sysroPunchesTransactions With (NOLOCK)"
                    Dim removedBackupPunches As Integer = roTypes.Any2Integer(ExecuteScalar(strCountSQL))

                    strDeleteSQL = "@DELETE# FROM sysroPunchesTransactions " &
                                       "WHERE DATEDIFF(day, TimeStamp, CAST(GETDATE() AS DATE)) > 31"
                    ExecuteSql(strDeleteSQL)
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldPunches : DeleteOldPunchesBackup : Deleted " & removedBackupPunches & "rows")

                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldPunches : Deleted " & removedPunches & " punches prior to " & roTypes.Any2String(deleteLimit))
                End If
                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldPunches:SQL error deleting old punches on DB")
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::DeleteOldPunches :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function DeleteOldBiometricData(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim oParameters As New roParameters("OPTIONS", True)
                Dim oParam As Object = oParameters.Parameter(Parameters.BiometricDataKeepPeriod)

                Dim oKeepDays As Integer = 15
                Try
                    oKeepDays = roTypes.Any2Integer(oParam)
                Catch ex As Exception
                    oKeepDays = 15
                End Try

                Dim deleteLimit As Date = DateTime.Now.Date.AddDays(oKeepDays * -1)
                Dim strDeleteSQL As String = "@DELETE# am FROM sysroPassports_AuthenticationMethods am" &
                                                " JOIN sysroPassports p ON p.id = am.IDPassport" &
                                                " JOIN EmployeeContracts c ON c.IDEmployee = p.IDEmployee" &
                                                " WHERE am.method = 4 AND am.BiometricData IS NOT NULL" &
                                                " AND c.EndDate < " & roTypes.Any2Time(deleteLimit).SQLSmallDateTime &
                                                " AND IDContract = (@SELECT# TOP (1) ec.IDContract from EmployeeContracts ec where ec.IDEmployee = c.IDEmployee order by ec.EndDate DESC)"

                bolRet = ExecuteSqlWithTimeOut(strDeleteSQL, 300)

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldBiometricData:SQL error deleting old biometric data on DB")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::DeleteOldBiometricData :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function DeleteOldAccessMovesHistory(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim oParameters As New roParameters("OPTIONS", True)
                Dim mNumMonthsAccess As Integer = roTypes.Any2Integer(oParameters.Parameter(Parameters.NumMonthsAccess))

                If mNumMonthsAccess <> 0 Then
                    Dim initialDate As DateTime = DateTime.Now.Date.AddMonths(mNumMonthsAccess * -1)

                    ' Borramos todos los accesos que sean anteriores a la primera fecha
                    ' que hay que gaurdar accesos historicos
                    Dim strDeleteSQL As String = "@DELETE# FROM PunchesCaptures where IDPunch in ( " &
                                       "@SELECT# top 100 IDPunch from PunchesCaptures inner join Punches on PunchesCaptures.IDPunch = Punches.ID and Punches.Type in (5,6) and Punches.ShiftDate < " & roTypes.Any2Time(initialDate).SQLSmallDateTime & ")"

                    Dim strCountSQL As String = "@SELECT# count(*) from PunchesCaptures inner join Punches on PunchesCaptures.IDPunch = Punches.ID and Punches.Type in (5,6) and Punches.ShiftDate < " & roTypes.Any2Time(initialDate).SQLSmallDateTime

                    While (roTypes.Any2Integer(ExecuteScalar(strCountSQL)) > 0 AndAlso bolRet)
                        bolRet = ExecuteSqlWithTimeOut(strDeleteSQL, 300)
                    End While

                    ' Fotos en Azure
                    Dim strSQL As String = String.Empty
                    strSQL = "@SELECT# ID FROM Punches WHERE PhotoOnAzure = 1 AND Type in (5,6) AND ShiftDate < " & roTypes.Any2Time(initialDate).SQLSmallDateTime
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        For Each oPunchRow As DataRow In tb.Rows
                            Azure.RoAzureSupport.DeletePunchPhotoFile(roTypes.Any2Integer(oPunchRow("ID")))
                        Next
                    End If

                    If bolRet Then
                        strDeleteSQL = "@DELETE# FROM Punches where Type in (5,6) AND ShiftDate < " & roTypes.Any2Time(initialDate).SQLSmallDateTime
                        bolRet = ExecuteSql(strDeleteSQL)
                    End If
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldAccessMovesHistory:SQL Error deleting historic access moves On DB")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::DeleteOldAccessMovesHistory :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function DeleteOldDocuments(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim oServerLicense As New roServerLicense
                Dim doclicense As Boolean = (oServerLicense.FeatureIsInstalled("Feature\Documents") OrElse oServerLicense.FeatureIsInstalled("Feature\Absences"))

                If doclicense Then
                    Dim oParameters As New roParameters("OPTIONS", True)
                    Dim oParam As Object = oParameters.Parameter(Parameters.DocumentsKeepPeriod)

                    Dim oKeepDays As Integer = 0
                    Try
                        oKeepDays = Any2Integer(oParam)
                    Catch ex As Exception
                        oKeepDays = 0
                    End Try

                    If oKeepDays > 0 Then

                        Dim strDeleteSQL As String = String.Empty
                        Dim strDocToDelete As String = "@SELECT# doc.Id, doc.BlobFileName, doc.SignStatus " &
                                " From documents doc " &
                                " INNER Join DocumentTemplates dt " &
                                "   On dt.id=doc.IdDocumentTemplate " &
                                " WHERE DateDiff(d, doc.DeliveryDate, GETDATE()) >= Case dt.[DaysBeforeDelete] When -1 Then " & oKeepDays & " Else dt.[DaysBeforeDelete] End " &
                                "   And dt.DaysBeforeDelete<>0  "

                        Dim dtDeleteDocuments As DataTable = CreateDataTable(strDocToDelete)

                        If dtDeleteDocuments IsNot Nothing AndAlso dtDeleteDocuments.Rows.Count > 0 Then

                            For Each oDocRow As DataRow In dtDeleteDocuments.Rows

                                Dim blobFileName As String = roTypes.Any2String(oDocRow("BlobFileName"))

                                If blobFileName <> String.Empty Then
                                    Azure.RoAzureSupport.DeleteFileFromBlob(blobFileName, roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName)

                                    If roTypes.Any2Integer(oDocRow("SignStatus")) = 3 Then
                                        Azure.RoAzureSupport.DeleteFileFromBlob(blobFileName & "_signreport", roLiveQueueTypes.documents, Azure.RoAzureSupport.GetCompanyName)
                                    End If
                                End If

                                strDeleteSQL = "@update# documents set document='', BlobFileName='', Title = Title + ' - Eliminado por RGPD' WHERE id=" & oDocRow("Id")
                                bolRet = ExecuteSql(strDeleteSQL)
                            Next
                        End If
                    End If
                Else
                    bolRet = True
                End If
                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldDocuments:SQL Error deleting documents On DB")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::DeleteOldDocuments :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function ManageVisits(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim manageChangeHour As Boolean = roTypes.Any2Boolean(oTask.Parameters("xChangeHour"))
                Dim manageChangeDay As Boolean = roTypes.Any2Boolean(oTask.Parameters("xChangeDay"))

                Dim strSQL As String = String.Empty

                If manageChangeHour Then

                    Dim fMinutes As Double = 720
                    Try
                        Dim oSystemParam As New roParameters("OPTIONS", True)
                        Dim strTime As String = oSystemParam.Parameter(Parameters.MovMaxHours)

                        If strTime <> "" Then
                            strTime = Format(CDate(strTime), "HH:mm")
                        Else
                            strTime = "12:00"
                        End If

                        fMinutes = roConversions.ConvertTimeToMinutes(strTime)
                    Catch ex As Exception
                        fMinutes = 720
                    End Try

                    'Poner la visitas en No presentado
                    strSQL = "@UPDATE# visit SET status = 3 WHERE status = 0 AND dateadd(mi," & fMinutes & ",begindate) < getdate()"
                    bolRet = ExecuteSql(strSQL)

                    If bolRet Then
                        'Poner en Cierre Automatica las visitas abiertas que hayan pasado las horas indicadas
                        strSQL = "@UPDATE# visit SET status = 4 WHERE status = 1  AND dateadd(mi," & fMinutes & ",begindate) < getdate()"
                        bolRet = ExecuteSql(strSQL)
                    End If
                    If bolRet Then
                        ' Reviso si hay visitas con periodicidad de las que no se ha creado la visita de hoy (si corresponde). Esto es necesario porque las
                        ' repeticiones se crean al finalizar una visita. Si un día no se produce esa visita, se corta la serie ...
                        strSQL = "@SELECT# * FROM (" &
                                        "@SELECT# *, ROW_NUMBER() OVER (PARTITION BY IDParentVisit ORDER BY IDVisit) AS RowNumber FROM ( " &
                                        "@SELECT# * from visit " &
                                        "inner join " &
                                        "(@SELECT# IDParentVisit idparent, MAX(Begindate) mbegindate, CloneEvery cevery from visit where len(CloneEvery)>0 and BeginDate < DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0) and IDParentVisit is not null and IDParentVisit not in " &
                                        "(@SELECT# IDParentVisit from visit where BeginDate>=  DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0) and IDParentVisit is not null) " &
                                        "Group by IDParentVisit, CloneEvery) tmp on tmp.idparent = Visit.idparentvisit and tmp.cevery = Visit.CloneEvery and Visit.BeginDate = tmp.mbegindate where Visit.Deleted = 0 and Visit.EndDate >= DateAdd(Day, DateDiff(Day, 0, GETDATE()), 0) " &
                                        ") aux2 ) aux3 " &
                                        "WHERE aux3.RowNumber = 1"

                        Dim tempTable As New DataTable
                        Dim sCloneEvery As String
                        Dim sOldIDVisit As String
                        Dim sIDVisit As String
                        Dim sName As String
                        Dim dBeginDate As Date
                        Dim dEndDate As Date
                        Dim bRepeat As Boolean
                        Dim iIDEmployee As Integer
                        Dim iCreatedBy As Integer
                        Dim iStatus As Integer
                        Dim dModified As Date
                        Dim sIDParentVisit As String
                        Dim visitType As Integer

                        tempTable = CreateDataTable(strSQL)

                        For Each oTmpRow As DataRow In tempTable.Rows
                            sCloneEvery = roTypes.Any2String(oTmpRow("CloneEvery"))
                            sIDVisit = roTypes.Any2String(oTmpRow("IDVisit"))
                            sOldIDVisit = sIDVisit
                            sName = roTypes.Any2String(oTmpRow("Name"))
                            dBeginDate = roTypes.Any2DateTime(oTmpRow("BeginDate"))
                            dEndDate = roTypes.Any2DateTime(oTmpRow("EndDate"))
                            bRepeat = roTypes.Any2Boolean(oTmpRow("Repeat"))
                            iIDEmployee = roTypes.Any2Integer(oTmpRow("IDEmployee"))
                            iCreatedBy = roTypes.Any2Integer(oTmpRow("CreatedBy"))
                            iStatus = roTypes.Any2Integer(oTmpRow("Status"))
                            dModified = roTypes.Any2DateTime(oTmpRow("Modified"))
                            sIDParentVisit = roTypes.Any2String(oTmpRow("IDParentVisit"))
                            visitType = roTypes.Any2String(oTmpRow("VisitType"))

                            If sCloneEvery.Replace("-", "").Length > 0 Then
                                sIDVisit = Guid.NewGuid().ToString()
                                iStatus = 0
                                While dBeginDate.Date < Now.Date
                                    Select Case sCloneEvery.Split(";")(0)
                                        Case "daily"
                                            If sCloneEvery.Split(";")(1) = "a" Then
                                                dBeginDate = dBeginDate.AddDays(1)
                                            Else
                                                dBeginDate = dBeginDate.AddDays(Any2Integer(sCloneEvery.Split(";")(2)))
                                            End If
                                        Case "weekly"
                                            Dim bexist As Boolean = False
                                            For i As Integer = 0 To sCloneEvery.Split(";")(2).Split(",").Count - 1
                                                If Any2Integer(sCloneEvery.Split(";")(2).Split(",")(i)) > dBeginDate.DayOfWeek Then
                                                    dBeginDate = dBeginDate.AddDays(Any2Integer(sCloneEvery.Split(";")(2).Split(",")(i)) - dBeginDate.DayOfWeek)
                                                    bexist = True
                                                    Exit For
                                                End If
                                                If Any2Integer(sCloneEvery.Split(";")(2).Split(",")(i)) = 0 Then
                                                    dBeginDate = dBeginDate.AddDays(7 - dBeginDate.DayOfWeek)
                                                    bexist = True
                                                    Exit For
                                                End If
                                            Next
                                            If Not bexist Then
                                                dBeginDate = dBeginDate.AddDays(7 * Any2Integer(sCloneEvery.Split(";")(1)))
                                                dBeginDate = dBeginDate.AddDays(-1 * (dBeginDate.DayOfWeek - Any2Integer(sCloneEvery.Split(";")(2).Split(",")(0))))
                                            End If
                                        Case "monthly"
                                            dBeginDate = New DateTime(dBeginDate.Year, dBeginDate.AddMonths(Any2Integer(sCloneEvery.Split(";")(2))).Month, Any2Integer(sCloneEvery.Split(";")(1)), dBeginDate.Hour, dBeginDate.Minute, 0)
                                        Case "yearly"
                                            dBeginDate = New DateTime(dBeginDate.AddYears(1).Year, Any2Integer(sCloneEvery.Split(";")(2)), Any2Integer(sCloneEvery.Split(";")(1)), dBeginDate.Hour, dBeginDate.Minute, 0)
                                    End Select
                                End While

                                ' Para visitas en las que se puede entrar más de una vez al día, aseguro que la fecha fin no acaba siendo menor que la de inicio
                                If dBeginDate < dEndDate Then
                                    strSQL = "@INSERT# INTO Visit ([IDVisit],[Name],[BeginDate],[EndDate],[Repeat],[IDEmployee],[CreatedBy],[Status],[Modified],[CloneEvery], [IDParentVisit], [VisitType])"
                                    strSQL += " values ('" + sIDVisit.ToString + "', '" + sName + "', " + Any2Time(dBeginDate).SQLDateTime + ", " + Any2Time(dEndDate).SQLDateTime + ", "
                                    strSQL += If(bRepeat, "1", "0") + ", " + iIDEmployee.ToString + ", " + iCreatedBy.ToString + ", 0, getdate(), '" + sCloneEvery.ToString + "','" & sIDParentVisit & "', " & visitType & ")"
                                    bolRet = ExecuteSql(strSQL)

                                    If bolRet Then
                                        ' Inserto los visitantes de la nueva visita
                                        strSQL = "@INSERT# INTO Visit_Visitor (IDVisit, IDVisitor) " &
                                    "@SELECT# '" & sIDVisit & "', IDVisitor from Visit_Visitor where IDVisit = '" & sOldIDVisit & "' "
                                        bolRet = ExecuteSql(strSQL)

                                        If bolRet Then
                                            ' Inserto los campos de la ficha de la visita
                                            strSQL = "@INSERT# INTO Visit_Fields_Value (IDField, IDVisit, Value) " &
                                        "@SELECT# idfield, '" & sIDVisit & "', Value from Visit_Fields_Value where IDVisit = '" & sOldIDVisit & "' "
                                            bolRet = ExecuteSql(strSQL)
                                        End If
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If

                If manageChangeDay AndAlso bolRet Then
                    ' Borramos visitas pasados X días según LOPD
                    strSQL = "@DELETE# visit WHERE repeat = 0 AND dateadd(d, " &
                                "cast( " &
                                    "(@SELECT# isnull((@SELECT# value from [sysroLiveAdvancedParameters] where ParameterName='vst_DeleteDataVisitors'),90)" &
                                ") as int)," &
                             "begindate) < getdate()"
                    bolRet = ExecuteSql(strSQL)

                    If bolRet Then
                        'Borrar visitantes huérfanos (Se borran los fichajes en cascada)
                        strSQL = "@DELETE# visitor WHERE IDVisitor IN (@SELECT# idvisitor " &
                                          "FROM (@SELECT# vr.IDVisitor, count(vv.IDVisitor) can " &
                                                       "FROM visitor vr LEFT OUTER JOIN [Visit_Visitor] vv ON vv.IDVisitor = vr.IDVisitor GROUP BY vr.IDVisitor) tmp " &
                                          "WHERE can=0)"
                        bolRet = ExecuteSql(strSQL)
                    End If
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::ManageVisits:SQL error deleting visits on DB")
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::ManageVisits :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function CheckCloseDateAlert(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim oParameters As New roParameters("OPTIONS", True)
                Dim oChecked As Integer = 0
                Dim oPeriod As Integer = Nothing

                Try
                    oChecked = roTypes.Any2Integer(oParameters.Parameter(Parameters.CloseDateAlert))
                    oPeriod = roTypes.Any2Integer(oParameters.Parameter(Parameters.CloseDateAlertPeriod))
                Catch ex As Exception
                    oChecked = 0
                    oPeriod = 30
                End Try

                Dim oDate As Object = Nothing
                Try
                    oDate = oParameters.Parameter(Parameters.FirstDate)
                Catch ex As Exception
                    oDate = Nothing
                End Try

                Dim generateAlert As Boolean = False

                If oDate IsNot Nothing AndAlso IsDate(oDate) Then
                    Dim xFreezeDate As Date = oDate

                    If oChecked = 1 AndAlso xFreezeDate.AddDays(oPeriod) < Date.Now.Date Then
                        generateAlert = True
                    End If
                Else
                    generateAlert = True
                End If

                If generateAlert Then
                    Dim oSettings As New roSettings

                    Dim oUserTask As New VTBusiness.UserTask.roUserTask()
                    oUserTask.ID = VTBusiness.UserTask.roUserTask.roUserTaskObject & ":\\CLOSEDATENOTIFICATION"
                    oUserTask.DateCreated = Now
                    oUserTask.TaskType = VTBusiness.UserTask.TaskType.UserTaskRepair
                    oUserTask.ResolverURL = "FN:\\Resolver_CloseDateAlert"
                    oUserTask.Message = Message("CloseDate.Expired", , , oSettings.GetVTSetting(eKeys.DefaultLanguage))

                    bolRet = oUserTask.Save()
                Else
                    Dim strSql = "@DELETE# FROM sysroUserTasks where ID = '" & UserTask.roUserTask.roUserTaskObject & ":\\CLOSEDATENOTIFICATION'"
                    bolRet = ExecuteSql(strSql)
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckCloseDateAlert:SQL error setting close date alert")
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CheckCloseDateAlert :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function ConsolidateVisualTimeData(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim idEmployee As Integer = roTypes.Any2Integer(oTask.Parameters("IDEmployee"))
                Dim xStartRecalcDate As DateTime = DateTime.Parse(oTask.Parameters("RecalcDate")).ToShortDateString()

                Dim oStartupValuesList As Generic.List(Of Integer) = roTypes.Any2String(oTask.Parameters("StartupValueIDs")).Split(",").ToList().ConvertAll(Function(str) roTypes.Any2Integer(str))
                Dim oCauseLimitValuesList As Generic.List(Of Integer) = roTypes.Any2String(oTask.Parameters("CauseLimitValueIDs")).Split(",").ToList().ConvertAll(Function(str) roTypes.Any2Integer(str))
                Dim oLabAgreeRulesList As Generic.List(Of Integer) = roTypes.Any2String(oTask.Parameters("LabAgreeRuleIDs")).Split(",").ToList().ConvertAll(Function(str) roTypes.Any2Integer(str))
                Dim oConceptsList As Generic.List(Of Integer) = roTypes.Any2String(oTask.Parameters("ConceptIDs")).Split(",").ToList().ConvertAll(Function(str) roTypes.Any2Integer(str))
                Dim oShiftsList As Generic.List(Of Integer) = roTypes.Any2String(oTask.Parameters("ShiftIDs")).Split(",").ToList().ConvertAll(Function(str) roTypes.Any2Integer(str))

                ' Miramos si el campo se està utilizando en algún valor inicial del convenio asignado al empleado
                Dim oStateLabAgree As New LabAgree.roLabAgreeState
                roBusinessState.CopyTo(oTask.State, oStateLabAgree)

                ' Lanzamos el recálculo para cada valor inicial relacionado
                Dim oStartupValue As LabAgree.roStartupValue = Nothing
                For Each oId In oStartupValuesList
                    If oId > 0 Then
                        oStartupValue = New LabAgree.roStartupValue(oId, oStateLabAgree)
                        bolRet = oStartupValue.Recalculate(Nothing, idEmployee, xStartRecalcDate)
                        If Not bolRet Then Exit For
                    End If
                Next

                If bolRet Then
                    ' Lanzamos el recálculo para cada limite de justificacion relacionado
                    Dim oCauseLimitValue As LabAgree.roLabAgreeCauseLimitValues = Nothing
                    For Each oId In oCauseLimitValuesList
                        If oId > 0 Then
                            Dim oLabAgreeId = roTypes.Any2Integer(ExecuteScalar("@SELECT# IDLabAgree FROM LabAgreeCauseLimitValues WHERE IDCauseLimitValue=" & oId))

                            oCauseLimitValue = New LabAgree.roLabAgreeCauseLimitValues(oLabAgreeId, oId, oStateLabAgree)
                            bolRet = oCauseLimitValue.Recalculate(Nothing, idEmployee, xStartRecalcDate)
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If

                If bolRet Then
                    ' Lanzamos el recálculo para cada regla de convenio relacionada.
                    Dim oLabAgreeRule As LabAgree.roLabAgreeRule = Nothing
                    For Each oId In oLabAgreeRulesList
                        If oId > 0 Then
                            oLabAgreeRule = New LabAgree.roLabAgreeRule(oId, oStateLabAgree)
                            bolRet = oLabAgreeRule.Recalculate(Nothing, idEmployee, xStartRecalcDate)
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If

                If bolRet Then
                    ' Miramos si el campo se està utilizando en algún saldo

                    ' Lanzamos el recálculo para cada saldo relacionado.
                    Dim oConcept As Concept.roConcept = Nothing
                    Dim oConceptState As New Concept.roConceptState(oStateLabAgree.IDPassport)
                    For Each oID In oConceptsList
                        If oID > 0 Then
                            oConcept = New Concept.roConcept(oID, oConceptState)
                            oConcept.Recalculate(True, idEmployee, xStartRecalcDate)
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If

                If bolRet Then
                    ' Lanzamos el recálculo para cada horario relacionado.
                    Dim oShiftState As New Shift.roShiftState(oStateLabAgree.IDPassport)
                    For Each oID In oShiftsList
                        If oID > 0 Then
                            bolRet = Shift.roShift.Recalculate(oID, idEmployee, xStartRecalcDate, oShiftState)
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If

                roConnector.InitTask(TasksType.DAILYSCHEDULE)

            Catch ex As Exception

                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::ConsolidateVisualTimeData :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function GenerateRoboticsPermissionsV3(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim strSQL As String = String.Empty
                Dim oStateTask As New roLiveTaskState(-1)

                strSQL = "@DELETE# FROM sysroGroupFeatures_PermissionsOverFeatures WHERE IDGroupFeature = (@SELECT# ID FROM sysroGroupFeatures where Name = '@@ROBOTICS@@Consultores')"
                ExecuteSqlWithoutTimeOut(strSQL)

                strSQL = "@INSERT# INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures]([IDGroupFeature],[IDFeature],[Permision])  @SELECT# (@SELECT# ID FROM sysroGroupFeatures where Name = '@@ROBOTICS@@Consultores'), sysroFeatures.ID, (CASE " &
                                    " WHEN CHARINDEX('A',PermissionTypes) > 0 THEN 9 " &
                                    " WHEN CHARINDEX('W',PermissionTypes) > 0 THEN 6 " &
                                    " WHEN CHARINDEX('R',PermissionTypes) > 0 THEN 3 " &
                            " End ) from sysroFeatures where Type = 'U'"

                ExecuteSql(strSQL)

                strSQL = "@DELETE# FROM sysroSecurityGroupFeature_Centers WHERE IDGroupFeature = (@SELECT# ID from sysroGroupFeatures where Name = '@@ROBOTICS@@Consultores')"
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                strSQL = "@INSERT# INTO sysroSecurityGroupFeature_Centers(IDGroupFeature,IDCenter) @SELECT# (@SELECT# ID from sysroGroupFeatures where Name = '@@ROBOTICS@@Consultores'), ID FROM BusinessCenters"
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                ' regeneramos permisos de los usuarios de robotics sobre las empresas del cliente
                strSQL = "@DELETE# FROM sysroPassports_Groups WHERE IDPassport IN(@SELECT# id FROM sysroPassports where  Description like '%@@ROBOTICS@@%' and sysroPassports.IDParentPassport > 0)"
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                strSQL = "@INSERT# INTO sysroPassports_Groups @SELECT# sysroPassports.id, groups.ID FROM groups, sysroPassports where  groups.path not like '%\%' and sysroPassports.Description like '%@@ROBOTICS@@%' and sysroPassports.IDParentPassport > 0"
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                ' Eliminamos los permisos sobre Nivel Medio y Nivel alto
                ' para Consultores, Tecnicos, Soporte
                strSQL = "@UPDATE# sysroGroupFeatures_PermissionsOverFeatures Set Permision = 0 Where IDGroupFeature IN(1,2,3) AND IDFeature IN(1540,1550)"
                ExecuteSql(strSQL)

                strSQL = "@DELETE# FROM sysroGroupFeatures_PermissionsOverFeatures WHERE IDGroupFeature = (@SELECT# ID FROM sysroGroupFeatures where Name = '@@ROBOTICS@@System')"
                ExecuteSqlWithoutTimeOut(strSQL)

                strSQL = "@INSERT# INTO [dbo].[sysroGroupFeatures_PermissionsOverFeatures]([IDGroupFeature],[IDFeature],[Permision])  @SELECT# (@SELECT# ID FROM sysroGroupFeatures where Name = '@@ROBOTICS@@System'), sysroFeatures.ID, (CASE " &
                                    " WHEN CHARINDEX('A',PermissionTypes) > 0 THEN 9 " &
                                    " WHEN CHARINDEX('W',PermissionTypes) > 0 THEN 6 " &
                                    " WHEN CHARINDEX('R',PermissionTypes) > 0 THEN 3 " &
                            " End ) from sysroFeatures where Type = 'U'"

                ExecuteSqlWithoutTimeOut(strSQL)

                strSQL = "@DELETE# FROM sysroSecurityGroupFeature_Centers WHERE IDGroupFeature = (@SELECT# ID from sysroGroupFeatures where Name = '@@ROBOTICS@@System')"
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                strSQL = "@INSERT# INTO sysroSecurityGroupFeature_Centers(IDGroupFeature,IDCenter) @SELECT# (@SELECT# ID from sysroGroupFeatures where Name = '@@ROBOTICS@@System'), ID FROM BusinessCenters"
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                If roTypes.Any2String(roLiveTask.getAdvancedParameterValue("VisualTime.BridgeSupport")) <> "0" Then
                    Dim idSystem As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID from sysroPassports where Description = '@@ROBOTICS@@System'"))

                    Dim oTaskParameters As New roCollection
                    oTaskParameters.Add("Context", 3)
                    oTaskParameters.Add("IDContext", idSystem)
                    oTaskParameters.Add("Action", 1)

                    roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oTaskParameters, oStateTask)

                    Dim idConsultor As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID from sysroPassports where Description = '@@ROBOTICS@@Consultor'"))

                    oTaskParameters = New roCollection
                    oTaskParameters.Add("Context", 3)
                    oTaskParameters.Add("IDContext", idConsultor)
                    oTaskParameters.Add("Action", 1)

                    roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oTaskParameters, oStateTask)

                    Dim idTecnico As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID from sysroPassports where Description = '@@ROBOTICS@@Tecnico'"))

                    oTaskParameters = New roCollection
                    oTaskParameters.Add("Context", 3)
                    oTaskParameters.Add("IDContext", idTecnico)
                    oTaskParameters.Add("Action", 1)

                    roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oTaskParameters, oStateTask)

                    Dim idSporte As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID from sysroPassports where Description = '@@ROBOTICS@@Soporte' AND isnull(GroupType,'') ='' "))

                    oTaskParameters = New roCollection
                    oTaskParameters.Add("Context", 3)
                    oTaskParameters.Add("IDContext", idSporte)
                    oTaskParameters.Add("Action", 1)

                    roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oTaskParameters, oStateTask)
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckCloseDateAlert:SQL error setting close date alert")
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::GenerateRoboticsPermissionsV3 :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function RecalculatePunchDirection(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim errorDescription As String = String.Empty

            Dim dFirstDateTime As DateTime = DateTime.MinValue
            Dim dLastDateTime As DateTime = DateTime.MinValue
            Dim sEmployeeIds As String = String.Empty
            Dim sWhere As String = String.Empty
            Dim sOriginalDirection As String = String.Empty
            Dim sFinalDirection As String = String.Empty
            Dim lEmployeesToRecalculate As New List(Of Integer)

            Try
                Dim oParameters As New roParameters("OPTIONS", True)

                dFirstDateTime = roTypes.Any2DateTime(oTask.Parameters("FirstDateTime"))
                dLastDateTime = roTypes.Any2DateTime(oTask.Parameters("LastDateTime"))
                sEmployeeIds = roTypes.Any2String(oTask.Parameters("EmployeesIds"))

                Dim oPunchState As New VTBusiness.Punch.roPunchState
                Dim dtPunches As New DataTable
                Dim oPunch As VTBusiness.Punch.roPunch
                Dim strSQL As String

                Dim dFreezeDate As Object = Nothing
                Try
                    dFreezeDate = oParameters.Parameter(Parameters.FirstDate)
                Catch ex As Exception
                    dFreezeDate = DateTime.MinValue
                End Try

                If dFirstDateTime <> DateTime.MinValue AndAlso dLastDateTime <> DateTime.MinValue AndAlso dFirstDateTime <= dLastDateTime AndAlso sEmployeeIds.Trim.Length > 0 AndAlso dFirstDateTime.Date >= dFreezeDate Then
                    If sEmployeeIds.Trim <> "*" Then
                        sWhere = "IDEmployee in (" & sEmployeeIds & ") AND "
                    End If

                    sWhere += " DateTime between " & Any2Time(dFirstDateTime).SQLDateTime & " AND " & Any2Time(dLastDateTime).SQLDateTime
                    sWhere += " AND Datetime > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = Punches.IDEmployee)  "
                    sWhere += " AND Type in (3,7) ORDER BY IDEmployee ASC, Datetime ASC"

                    dtPunches = VTBusiness.Punch.roPunch.GetPunches(sWhere, oPunchState)

                    If dtPunches.Rows.Count > 0 Then
                        For Each punchRow In dtPunches.Rows
                            oPunch = New VTBusiness.Punch.roPunch(0, punchRow("ID"), oPunchState)
                            sOriginalDirection = oPunch.ActualType.ToString
                            sFinalDirection = oPunch.CalculateTypeAtt.ToString
                            If sOriginalDirection <> sFinalDirection Then
                                oPunch.Save(, True,,, False)
                                If Not lEmployeesToRecalculate.Contains(oPunch.IDEmployee) Then lEmployeesToRecalculate.Add(oPunch.IDEmployee)
                                Try
                                    strSQL = "@UPDATE# Punches set DebugInfo = '" & sOriginalDirection & "->" & sFinalDirection & "' where ID = " & oPunch.ID
                                    ExecuteSql(strSQL)
                                Catch ex As Exception
                                    'Por si la columna DebugInfo no existe. No debiera pasar
                                End Try
                            End If
                        Next
                    End If

                    ' Marco recálculo si se modificó algo
                    If lEmployeesToRecalculate.Count > 0 Then
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) set Status = 0, [GUID] = '' where IDEmployee in (" & String.Join(",", lEmployeesToRecalculate.ToArray) & ") AND Date BETWEEN " & Any2Time(dFirstDateTime).SQLSmallDateTime & " AND " & Any2Time(dLastDateTime).SQLSmallDateTime & " AND DATE <= GETDATE()"
                        ExecuteSql(strSQL)
                    End If

                    If oPunchState.Result = PunchResultEnum.NoError Then
                        'Audit(oTask.Action, oTask.ID, oTask.XmlParameters, oTask.State)
                        bolRet = True
                        errorDescription = oPunchState.ResultDetail 'Sí. Por alguna razón, en el error de la tarea también se guardan detalles cuando no ha habido error ...
                    Else
                        bolRet = False
                        errorDescription = oPunchState.ErrorText
                    End If
                Else
                    bolRet = False
                    errorDescription = "Invalid parameters or some date in freeze period"
                End If
            Catch ex As Exception
                bolRet = False
                errorDescription = ex.Message

                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::RecalculatePunchDirection :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = errorDescription}
        End Function

        Public Shared Function MigrateDocsToAzure(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try

                ' Vemos si está habilitado el guardado de documentos en Azure
                Dim param As New AdvancedParameter.roAdvancedParameter("VTLive.AzureEnvironment.DocumentsActivated", New AdvancedParameter.roAdvancedParameterState)
                Dim bMigrateToAzure As Boolean = roTypes.Any2Boolean(param.Value)

                If bMigrateToAzure Then
                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    Dim oDocumentManager = New roDocumentManager()
                    Dim oDocument = New roDocument

                    ' Documentos que deben pasar a azure
                    Dim strMigrateSQL As String = "@SELECT# TOP 20 ID, Document FROM Documents WHERE BlobFileName IS NULL AND DATALENGTH(Document) > 1 ORDER BY ID ASC"
                    Dim strCountSQL As String = "@SELECT# COUNT(*) FROM Documents WHERE BlobFileName IS NULL AND DATALENGTH(Document) > 1 "
                    Dim strUpdateSQL As String = String.Empty
                    Dim iLastIdChecked As Integer = 0
                    While (Any2Integer(ExecuteScalar(strCountSQL)) > 0 AndAlso bolRet)
                        Dim dtDocuments = CreateDataTable(strMigrateSQL)
                        If (dtDocuments IsNot Nothing AndAlso dtDocuments.Rows.Count > 0) Then
                            For Each documentRow In dtDocuments.Rows
                                oDocument = oDocumentManager.LoadDocument(documentRow("ID"))
                                Dim strDocumentBlobName As String = String.Empty
                                Dim fileCRC As String = String.Empty
                                oDocument.Document = roEncrypt.Decrypt(documentRow("Document"))
                                If oDocumentManager.SaveDocumentOnAzure(oDocument, strDocumentBlobName, fileCRC) Then
                                    strUpdateSQL = "@UPDATE# Documents SET BlobFileName = '" & strDocumentBlobName & "', CRC = '" & fileCRC & "', Document = dbo.f_Base64ToBinary('') WHERE ID = " & oDocument.Id
                                End If
                                iLastIdChecked = oDocument.Id
                                bolRet = bolRet AndAlso ExecuteSql(strUpdateSQL)
                            Next
                        End If
                        strUpdateSQL = "@SELECT# TOP 20 ID FROM Documents WHERE BlobFileName IS NULL AND DATALENGTH(Document) > 1 AND ID > " & iLastIdChecked.ToString & " ORDER BY ID ASC"
                        strCountSQL = "@SELECT# COUNT(*) FROM Documents WHERE BlobFileName IS NULL AND DATALENGTH(Document) > 1 AND ID > " & iLastIdChecked.ToString
                    End While

                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::MigrateDocsToAzure:SQL error moving documents to Azure Blob Storage")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MigrateDocsToAzure :", ex)
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function AddReportToDocManager(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim oDocState As New roDocumentState(1)
                Dim oDocumentManager = New roDocumentManager(oDocState)
                Dim oDocument = New roDocument
                Dim oDocumentTemplate As roDocumentTemplate
                oDocumentTemplate = oDocumentManager.LoadDocumentTemplate(oTask.Parameters("xIDDocTemplate"))

                bolRet = Not (oDocumentTemplate Is Nothing OrElse oDocumentTemplate.Scope <> DocumentScope.EmployeeContract)

                If bolRet Then
                    oDocument.Id = -1
                    oDocument.IdEmployee = roTypes.Any2Integer(oTask.Parameters("xIDEmployee"))
                    oDocument.DocumentTemplate = oDocumentTemplate
                    oDocument.Document = System.IO.File.ReadAllBytes(oTask.Parameters("xReportPath"))
                    oDocument.DeliveredDate = roTypes.Any2DateTime(oTask.Parameters("xDeliveryDate"))
                    oDocument.Title = roTypes.Any2String(oTask.Parameters("xReportName"))
                    oDocument.IdCompany = -1
                    oDocument.IdPunch = 0
                    oDocument.IdDaysAbsence = 0
                    oDocument.IdHoursAbsence = 0
                    oDocument.IdOvertimeForecast = 0
                    oDocument.DocumentType = roTypes.Any2String(oTask.Parameters("xFileExtension"))
                    oDocument.DeliveryChannel = "Report Server"
                    oDocument.DeliveredBy = "System"
                    oDocument.Status = DocumentStatus.Pending
                    oDocument.StatusLevel = 1
                    oDocument.IdLastStatusSupervisor = 1
                    oDocument.LastStatusChange = oDocument.DeliveredDate
                    oDocument.BeginDate = New Date(1900, 1, 1)
                    oDocument.EndDate = New Date(2079, 1, 1)

                    bolRet = oDocumentManager.SaveDocument(oDocument)
                    If Not bolRet Then oTask.ErrorCode = "SQL adding report to document manager"
                Else
                    oTask.ErrorCode = "Document template is not of expected type (Employee)"
                End If
                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::AddReportToDocManager:" & oTask.ErrorCode)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::AddReportToDocManager :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function CopyAdvancedBudgetPlan(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim errorDescription As String = String.Empty

            Try
                Dim oBudgetState As New roBudgetState(oTask.IDPassport)
                Dim oBudgetManager As New roBudgetManager(oBudgetState)

                oBudgetManager.CopyBudget(oTask.Parameters, oBudgetState)

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{ParametersValues}")

                Dim oAuditState As New AuditState.wscAuditState(oTask.IDPassport)
                Support.roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tCopyBudgetPlanificationAdvanced, "", lstAuditParameterNames, lstAuditParameterValues, oAuditState)

                If Not oBudgetState.Result = BudgetResultEnum.NoError Then
                    bolRet = False
                    errorDescription = oBudgetState.ErrorText
                End If
            Catch ex As Exception
                bolRet = False
                errorDescription = ex.Message

                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CopyAdvancedBudgetPlan :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = errorDescription}
        End Function

        Public Shared Function MassProgrammedAbsence(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strError As String = ""

            Try
                Dim IDCause As Integer = roTypes.Any2Integer(oTask.Parameters("IDCause"))

                Dim BeginDate As Date = DateTime.Parse(oTask.Parameters("BeginDateTime"))
                Dim EndDate As Date = DateTime.Parse(oTask.Parameters("EndDateTime"))

                Dim strDescription As String = roTypes.Any2String(oTask.Parameters("Description"))

                Dim MinDuration As Double = roTypes.Any2Double(oTask.Parameters("MinDuration"))
                Dim MaxDuration As Double = roTypes.Any2Double(oTask.Parameters("MaxDuration"))

                Dim MaxDays As String = roTypes.Any2String(oTask.Parameters("MaxDays"))

                Dim BeginHour As DateTime = DateTime.Parse(oTask.Parameters("BeginHour"))
                Dim EndHour As DateTime = DateTime.Parse(oTask.Parameters("EndHour"))

                Dim forecastType As eRequestType = [Enum].Parse(GetType(eRequestType), roTypes.Any2String(oTask.Parameters("RequestType")))

                Dim strSelectorEmployees As String = roTypes.Any2String(oTask.Parameters("EmployeeGroups"))
                Dim strFeature As String = "Employees"
                Dim strFilter As String = roTypes.Any2String(oTask.Parameters("EmployeeFilter"))
                Dim strFilterUser As String = roTypes.Any2String(oTask.Parameters("EmployeeUserFieldFilter"))

                Dim lstEmployees As List(Of Integer) = roSelector.GetEmployeeList(Any2Integer(oTask.IDPassport), strFeature, "U", Nothing,
                                                                                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)

                Dim empState As New Employee.roEmployeeState

                For Each iIDEmployee As Integer In lstEmployees

                    If forecastType = eRequestType.PlannedAbsences Then

                        Dim oProgAbsence As roProgrammedAbsence = New roProgrammedAbsence

                        With oProgAbsence
                            .IDEmployee = iIDEmployee
                            .BeginDate = BeginDate
                            .FinishDate = IIf(EndDate = Date.MinValue, Nothing, EndDate)
                            .IDCause = roTypes.Any2Integer(IDCause)
                            .MaxLastingDays = Val(MaxDays)
                            .Description = strDescription

                        End With

                        If Not oProgAbsence.Save(False, True) Then
                            strError += oTask.State.Language.Translate("MassProgrammedAbsence.EmployeeSaveError", "") & " " & roBusinessSupport.GetEmployeeName(iIDEmployee, empState) & ": " & oProgAbsence.State.ErrorText + " - "
                            bolRet = False
                        End If
                    ElseIf forecastType = eRequestType.PlannedCauses Then
                        Dim oProgAbsenceH As roProgrammedCause = New roProgrammedCause

                        With oProgAbsenceH
                            .IDEmployee = iIDEmployee
                            .MinDuration = MinDuration
                            .Duration = MaxDuration
                            .BeginTime = BeginHour
                            .EndTime = EndHour
                            .ProgrammedDate = BeginDate
                            .ProgrammedEndDate = EndDate
                            .IDCause = roTypes.Any2Integer(IDCause)
                            .Description = strDescription

                        End With

                        If Not oProgAbsenceH.Save(False, True) Then
                            strError += oTask.State.Language.Translate("MassProgrammedAbsence.EmployeeSaveError", "") & " " & roBusinessSupport.GetEmployeeName(iIDEmployee, empState) & ":" & oProgAbsenceH.State.ErrorText + " - "
                            bolRet = False
                        End If
                    ElseIf forecastType = eRequestType.PlannedOvertimes Then
                        Dim oProgrammedOvertimeManager As New VTHolidays.roProgrammedOvertimeManager
                        Dim oProgAbsenceH As roProgrammedOvertime = New roProgrammedOvertime

                        With oProgAbsenceH
                            .IDEmployee = iIDEmployee
                            .MinDuration = MinDuration
                            .Duration = MaxDuration
                            .BeginTime = BeginHour
                            .EndTime = EndHour
                            .ProgrammedBeginDate = BeginDate
                            .ProgrammedEndDate = EndDate
                            .IDCause = roTypes.Any2Integer(IDCause)
                            .Description = strDescription

                        End With

                        If Not oProgrammedOvertimeManager.SaveProgrammedOvertime(oProgAbsenceH, False, True) Then
                            strError += oTask.State.Language.Translate("MassProgrammedAbsence.EmployeeSaveError", "") & " " & roBusinessSupport.GetEmployeeName(iIDEmployee, empState) & ":" & oProgrammedOvertimeManager.State.ErrorText + " - "
                            bolRet = False
                        End If
                    End If
                Next

            Catch ex As Exception

                bolRet = False
                strError = ex.Message

                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MassIncidences :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}
        End Function

        Public Shared Function MassPunchInput(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strError As String = ""

            Try
                Dim oPunchState As New Punch.roPunchState(oTask.IDPassport)

                Dim strEmployeeGroups As String = roTypes.Any2String(oTask.Parameters("EmployeeGroups"))
                Dim strEmployeeFilters As String = roTypes.Any2String(oTask.Parameters("EmployeeFilter"))
                Dim strUserFieldFilters As String = roTypes.Any2String(oTask.Parameters("EmployeeUserFieldFilter"))
                Dim xDateTime As Date = DateTime.Parse(oTask.Parameters("PunchDateTime"))
                Dim IDTerminal As Integer = roTypes.Any2Integer(oTask.Parameters("IDTerminal"))
                Dim IDCause As Integer = roTypes.Any2Integer(oTask.Parameters("IDCause"))
                Dim tmpDirection As Integer = roTypes.Any2Integer(oTask.Parameters("PunchDirection"))
                Dim direction As PunchTypeEnum = PunchTypeEnum._AUTO
                Dim IDReader As Integer = 0
                Dim IDZone As Integer = 0

                Dim strSelectorEmployees As String = roTypes.Any2String(oTask.Parameters("EmployeeGroups"))
                Dim strFeature As String = "Calendar.Punches.Punches"
                Dim strFilter As String = roTypes.Any2String(oTask.Parameters("EmployeeFilter"))
                Dim strFilterUser As String = roTypes.Any2String(oTask.Parameters("EmployeeUserFieldFilter"))

                Dim lstEmployees As List(Of Integer) = roSelector.GetEmployeeList(Any2Integer(oTask.IDPassport), strFeature, "U", Nothing,
                                                                                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)

                Dim totalActions As Integer = lstEmployees.Count
                If totalActions = 0 Then totalActions = 1
                Dim stepProgress As Double = 100 / totalActions

                Dim oEmpState As New Employee.roEmployeeState(oTask.IDPassport)

                Dim bolNotify As Boolean = False

                Dim oTerminal As New Terminal.roTerminal(IDTerminal, New Terminal.roTerminalState(oTask.IDPassport))
                Dim oTerminalReader As Terminal.roTerminal.roTerminalReader = oTerminal.Readers(0)
                IDReader = oTerminalReader.ID

                If oTerminal.Readers.Count > 0 AndAlso oTerminalReader.IDZone.HasValue AndAlso (oTerminal.Readers(0).Mode.ToString()).Contains("ACC") Then
                    IDZone = oTerminalReader.IDZone
                    If oTerminal.Readers(0).Mode.ToString().Contains("TA") Then
                        direction = PunchTypeEnum._L
                    Else
                        direction = PunchTypeEnum._AV
                    End If
                Else
                    IDZone = -1
                    If tmpDirection = 0 Then
                        direction = PunchTypeEnum._AUTO
                    ElseIf tmpDirection = 1 Then
                        direction = PunchTypeEnum._IN
                    ElseIf tmpDirection = 2 Then
                        direction = PunchTypeEnum._OUT
                    End If
                End If

                For Each iEmployee As Integer In lstEmployees

                    If Robotics.Security.WLHelper.HasFeaturePermissionByEmployeeOnDate(oPunchState.IDPassport, "Calendar.Punches.Punches", Permission.Write, iEmployee, xDateTime.Date) Then
                        Dim oPunch As New Punch.roPunch(oPunchState)

                        oPunch.IDEmployee = iEmployee
                        oPunch.DateTime = xDateTime
                        oPunch.IDTerminal = IDTerminal
                        oPunch.IDReader = IDReader
                        oPunch.Type = direction
                        oPunch.Passport = oTask.IDPassport
                        If IDCause > 0 Then oPunch.TypeData = IDCause
                        oPunch.Source = PunchSource.StandardImport

                        If oPunch.Save(False) Then
                            bolNotify = True
                        Else
                            If oPunch.State.Result <> PunchResultEnum.NoError Then
                                strError = strError & oPunch.State.ErrorText
                            End If
                            bolRet = False
                        End If
                    Else
                        oEmpState.Result = EmployeeResultEnum.AccessDenied
                        strError = strError & vbNewLine & roBusinessSupport.GetEmployeeName(iEmployee, oEmpState) & ": " & oEmpState.ErrorText
                    End If

                    oTask.Progress = oTask.Progress + stepProgress
                    oTask.Save()
                Next

                If bolNotify Then
                    Robotics.VTBase.Extensions.roConnector.InitTask(TasksType.MOVES)
                End If

            Catch ex As Exception
                bolRet = False
                strError = ex.Message

                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::MassIncidences :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}
        End Function

        Public Shared Function GenerateSecurityPermissionsV3(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Dim strError As String = ""
            Dim strSQL As String = ""
            Dim bolretry As Boolean = False

            Try
                Dim intContext As Integer = roTypes.Any2Integer(oTask.Parameters("Context"))
                Dim intIDContext As Double = roTypes.Any2Double(oTask.Parameters("IDContext"))
                Dim intAction As Integer = roTypes.Any2Integer(oTask.Parameters("Action"))

                Dim intIDParentPassports As String = ""
                Dim strFillterRoboticsParentPassports As String = "@SELECT# DISTINCT ID FROM sysroPassports WHERE GROUPTYPE = 'U' AND ID IN(@SELECT# ID FROM sysroPassports WHERE GROUPTYPE = 'U' AND Description LIKE '%@@ROBOTICS@@%')"

                ' OBTENEMOS LOS ID'S DE LOS GRUPOS DE USUARIOS QUE TENEMOS QUE ACTUALIZAR EN FUNCIÓN DE LOS PARAMETROS DE LA TAREA
                Select Case intContext
                    Case 1 'TODO
                        ' Todos los Supervisores
                        intIDParentPassports = ""

                    Case 2 'FUNCIONES
                        ' Obtenemos todos los grupos de usuarios de Supervisores que tengan asignada la función
                        intIDParentPassports = "-1"
                        strSQL = "@SELECT# DISTINCT IDParentPassport FROM sysroPassports WHERE IDParentPassport is not null AND sysroPassports.IDGroupFeature =" & intIDContext.ToString
                        Dim dtPassports As DataTable = CreateDataTable(strSQL)
                        If dtPassports IsNot Nothing AndAlso dtPassports.Rows.Count > 0 Then
                            For Each oPassport As DataRow In dtPassports.Rows
                                intIDParentPassports += "," & oPassport("IDParentPassport").ToString
                            Next
                        End If
                    Case 3 'SUPERVISORES
                        ' El ID del Grupo del Supervisor indicado en la Tarea
                        intIDParentPassports = Any2String(ExecuteScalar("@SELECT# ISNULL(IDParentPassport,0) FROM sysroPassports WHERE ID=" & intIDContext.ToString))
                    Case 4 'GRUPOS
                        ' Todos los Supervisores
                        intIDParentPassports = ""
                    Case Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::GenerateSecurityPermissionsV3 : Invalid Context:" & intContext)
                        bolRet = False
                End Select

                If bolRet Then
                    ' POR DEFECTO QUITAMOS TODOS LOS PERMISOS A TODOS LOS GRUPOS DE SUPERVISORES QUE INDICA LA TAREA
                    ' QUE NO SEAN LOS GRUPOS DE ROBOTICS
                    Dim oSqlArray As New Generic.List(Of String)
                    oSqlArray.Add("sysroPassports_PermissionsOverEmployees")
                    oSqlArray.Add("sysroPassports_PermissionsOverFeatures")
                    oSqlArray.Add("sysroPassports_PermissionsOverGroups")

                    For Each strSQL In oSqlArray
                        strSQL = "@DELETE# " & strSQL & " WHERE "
                        If intIDParentPassports.Length > 0 Then
                            strSQL += " IDPASSPORT IN (" & intIDParentPassports & ")"
                        Else
                            strSQL += " IDPASSPORT Not IN (" & strFillterRoboticsParentPassports & ")"
                        End If
                        If strSQL.Contains("sysroPassports_PermissionsOverFeatures") Then
                            strSQL += "  AND IDFeature IN (@SELECT# ID FROM sysroFeatures WHERE Type like 'U')"
                        End If
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        If Not bolRet Then
                            bolretry = True
                            Exit For
                        End If
                    Next
                End If

                If bolRet Then
                    ' CONFIGURAMOS LOS SUPERVISORES QUE NOS INDICAN EN LA TAREA
                    strSQL = "@SELECT# * FROM sysroPassports WHERE "

                    If intIDParentPassports.Length > 0 Then
                        strSQL += "  IDParentPassport IN (" & intIDParentPassports & ") "
                    Else

                        strSQL += " IDParentPassport NOT IN (" & strFillterRoboticsParentPassports & ") "
                    End If

                    strSQL += "  AND IDParentPassport IS NOT NULL AND IDParentPassport > 0 and IDGroupFeature is not null"

                    Dim dtPassports As DataTable = CreateDataTable(strSQL)
                    If dtPassports IsNot Nothing AndAlso dtPassports.Rows.Count > 0 Then
                        For Each oPassport As DataRow In dtPassports.Rows
                            bolRet = GenerateSecurityPermissionsForPassportV3(oPassport)
                            If Not bolRet Then
                                bolretry = True
                                Exit For
                            End If
                        Next
                    End If
                End If

                If bolRet Then
                    'Debemos buscar los centros de negocio y centros de costo del rol asociado al supervisor para asignarlo a la tabla sysropassport y sysropassport_centers
                    strSQL = ""
                    Select Case intContext
                        Case 1 'TODO
                        ' Todos los Supervisores
                        Case 2 'FUNCIONES
                            ' Obtenemos todos los grupos de usuarios de Supervisores que tengan asignada la función
                            strSQL = "@SELECT# IDParentPassport, srp.Id, sgf.BusinessGroupList,  srp.IDGroupFeature FROM sysroPassports srp" &
                                    " inner join sysroGroupFeatures sgf on sgf.ID = srp.IDGroupFeature" &
                                    " WHERE srp.IDParentPassport is not null and srp.IDGroupFeature = " & intIDContext.ToString
                        Case 3 'SUPERVISORES
                            ' El ID del Grupo del Supervisor indicado en la Tarea
                            strSQL = "@SELECT# ISNULL(srp.IDParentPassport,0) as IDParentPassport, srp.id, sgf.BusinessGroupList, srp.IDGroupFeature FROM sysroPassports srp " &
                                    " inner join sysroGroupFeatures sgf On sgf.ID = srp.IDGroupFeature " &
                                    " WHERE srp.ID= " & intIDContext.ToString
                        Case 4 'GRUPOS
                            ' Todos los Supervisores
                        Case Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::GenerateSecurityPermissions : Invalid Context:" & intContext)
                            bolRet = False
                    End Select

                    If bolRet Then
                        If strSQL.Length > 0 Then
                            Dim dtPassports As DataTable = CreateDataTable(strSQL)
                            If dtPassports IsNot Nothing AndAlso dtPassports.Rows.Count > 0 Then
                                For Each oPassport As DataRow In dtPassports.Rows
                                    Dim strUpdateSQL As String = "@UPDATE# sysropassports SET BusinessGroupList='' WHERE ID IN(" & oPassport("IDParentPassport") & "," & oPassport("Id") & ")"
                                    bolRet = ExecuteSql(strUpdateSQL)
                                    If Not bolRet Then Exit For

                                    strUpdateSQL = "@UPDATE# sysropassports SET BusinessGroupList ='" & oPassport("BusinessGroupList") & "' WHERE ID=" & oPassport("IDParentPassport")
                                    bolRet = ExecuteSql(strUpdateSQL)
                                    If Not bolRet Then Exit For

                                    strUpdateSQL = "@DELETE# FROM sysroPassports_Centers WHERE IDPassport = " & oPassport("IDParentPassport")
                                    bolRet = ExecuteSql(strUpdateSQL)
                                    If Not bolRet Then Exit For

                                    strUpdateSQL = "@INSERT# INTO sysroPassports_Centers(IDPassport,IDCenter) @SELECT# " & oPassport("IDParentPassport") & ", IDcenter FROM sysroSecurityGroupFeature_Centers WHERE IDGroupFeature =" & oPassport("IDGroupFeature")
                                    bolRet = ExecuteSql(strUpdateSQL)
                                    If Not bolRet Then Exit For
                                Next
                            End If
                        End If
                    End If
                End If

                If bolRet Then
                    ' LANZAMOS EL STORED QUE CONSOLIDA LOS PERMISOS DE LOS SUPERVISORES QUE SEAN NECESARIOS
                    Select Case intContext
                        Case 1, 4 'TODO / CREACION GRUPO
                            Dim Command As DbCommand = CreateCommand("ExecuteRequestPassportPermissionsAction")
                            Command.CommandType = CommandType.StoredProcedure
                            Command.CommandTimeout = 0
                            AddParameter(Command, "@IDAction", DbType.Int32, 1).Value = -2
                            AddParameter(Command, "@IDObject", DbType.Int32, 1).Value = -2
                            AddParameter(Command, "@Version", DbType.Int32, 1).Value = 3
                            Command.ExecuteNonQuery()
                        Case 2 'FUNCIONES
                            Dim strIDPassports() As String = roTypes.Any2String(intIDParentPassports).Split(",")
                            For i As Integer = 0 To strIDPassports.Length - 1
                                Dim Command As DbCommand = CreateCommand("ExecuteRequestPassportPermissionsAction")
                                Command.CommandType = CommandType.StoredProcedure
                                Command.CommandTimeout = 0
                                AddParameter(Command, "@IDAction", DbType.Int32, 1).Value = 1
                                AddParameter(Command, "@IDObject", DbType.Int32, 1).Value = strIDPassports(i)
                                AddParameter(Command, "@Version", DbType.Int32, 1).Value = 3
                                Command.ExecuteNonQuery()
                            Next
                        Case 3 'SUPERVISORES
                            Dim Command As DbCommand = CreateCommand("ExecuteRequestPassportPermissionsAction")
                            Command.CommandType = CommandType.StoredProcedure
                            Command.CommandTimeout = 0
                            AddParameter(Command, "@IDAction", DbType.Int32, 1).Value = intAction
                            AddParameter(Command, "@IDObject", DbType.Int32, 1).Value = Any2Integer(intIDParentPassports)
                            AddParameter(Command, "@Version", DbType.Int32, 1).Value = 3
                            Command.ExecuteNonQuery()
                    End Select

                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::GenerateSecurityPermissionsv3 : Security Mode v3 regenerated!! Context=" & intContext.ToString & " ID=" & intIDContext.ToString & " Action=" & intAction.ToString)
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::GenerateSecurityPermissionsv3:" & strError)
                End If
            Catch ex As DbException
                bolretry = True
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::GenerateSecurityPermissionsv3 :", ex)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::GenerateSecurityPermissionsv3 :", ex)
            End Try

            If bolretry Then
                ' Si ha ocurrido un problema de acceso a BBDD volvemos a lanzar la tarea para ejecutarla
                Try
                    Dim oStateTask As New roLiveTaskState(-1)

                    Dim oParameters As New roCollection
                    oParameters.Add("Context", oTask.Parameters("Context"))
                    oParameters.Add("IDContext", oTask.Parameters("IDContext"))
                    oParameters.Add("Action", oTask.Parameters("Action"))

                    roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oParameters, oStateTask)

                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::GenerateSecurityPermissionsv3 : Retry Security Task")
                Catch ex As Exception
                    'do nothing
                End Try
            End If

            Return New BaseTaskResult With {.Result = bolRet, .Description = strError}
        End Function

        Private Shared Function GenerateSecurityPermissionsForPassportV3(ByVal oPassportRow As DataRow) As Boolean
            '
            ' Generamos los permisos del Supervisor en función de su configuración actual
            '
            Dim bolRet As Boolean = False
            Dim strSQL As String = ""
            Dim oSqlArray As New Generic.List(Of String)
            Dim bIsRoboticsUser As Boolean = False

            Try

                ' BORRAMOS TODOS LOS PERMISOS DEL SUPERVISOR
                oSqlArray.Add("sysroPassports_PermissionsOverFeatures")
                oSqlArray.Add("sysroPassports_PermissionsOverGroups")

                bIsRoboticsUser = Any2Boolean(ExecuteScalar("@SELECT#  (CASE WHEN ISNULL(CHARINDEX('@@ROBOTICS@@',Description),0) > 0 THEN 1 WHEN ISNULL(CHARINDEX('@@ROBOTICS@@',Description),0) <= 0 THEN 0 End ) FROM sysroPassports WHERE ID=" & oPassportRow("IDParentPassport")))

                For Each strSQL In oSqlArray
                    If Not strSQL.ToUpper.Contains("DELETE") Then
                        strSQL = "@DELETE# " & strSQL & " WHERE IDPASSPORT =" & oPassportRow("IDParentPassport").ToString
                    End If

                    If strSQL.Contains("sysroPassports_PermissionsOverFeatures") Then
                        strSQL += "  AND IDFeature IN (@SELECT# ID FROM sysroFeatures WHERE Type like 'U')"
                    End If

                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    If Not bolRet Then
                        Exit For
                    End If
                Next

                ' ASIGNAMOS LOS PERMISOS QUE TIENE EL SUPERVISOR SOBRE LAS FUNCIONALIDADES
                ' , A PARTIR DE LA FUNCIÓN QUE TIENE ASIGNADA
                If bolRet Then
                    strSQL = "@INSERT# INTO sysroPassports_PermissionsOverFeatures(IDPassport, IDFeature, Permission) " &
                    "  @SELECT# " & oPassportRow("IDParentPassport").ToString & ", IDFeature, Permision " &
                    " FROM sysroGroupFeatures_PermissionsOverFeatures WHERE IDGroupFeature= " & oPassportRow("IDGroupFeature").ToString &
                      " AND sysroGroupFeatures_PermissionsOverFeatures.IDFeature IN(@SELECT# ID FROM sysroFeatures WHERE Type Like 'U') "
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                If bIsRoboticsUser Then
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup], [IDApplication], [Permission])  @SELECT# " & oPassportRow("IDParentPassport") & ", Groups.ID, 1,6 from Groups where CHARINDEX('\', Path) = 0"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup], [IDApplication], [Permission])  @SELECT# " & oPassportRow("IDParentPassport") & ", Groups.ID, 2,6 from Groups where CHARINDEX('\', Path) = 0"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup], [IDApplication], [Permission])  @SELECT# " & oPassportRow("IDParentPassport") & ", Groups.ID, 9,6 from Groups where CHARINDEX('\', Path) = 0"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup], [IDApplication], [Permission])  @SELECT# " & oPassportRow("IDParentPassport") & ", Groups.ID, 11,6 from Groups where CHARINDEX('\', Path) = 0"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup], [IDApplication], [Permission])  @SELECT# " & oPassportRow("IDParentPassport") & ", Groups.ID, 25,6 from Groups where CHARINDEX('\', Path) = 0"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup], [IDApplication], [Permission])  @SELECT# " & oPassportRow("IDParentPassport") & ", Groups.ID, 31,6 from Groups where CHARINDEX('\', Path) = 0"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_PermissionsOverGroups([IDPassport], [IDGroup], [IDApplication], [Permission])  @SELECT# " & oPassportRow("IDParentPassport") & ", Groups.ID, 32,6 from Groups where CHARINDEX('\', Path) = 0"
                    ExecuteSqlWithoutTimeOut(strSQL)

                    ' Asignamos permisos sobre los tipos de categorias
                    strSQL = "@DELETE# sysroPassports_Categories WHERE IDPassport=" & oPassportRow("id")
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_Categories([IDPassport], [IDCategory], [LevelOfAuthority], [ShowFromLevel])  VALUES (" & oPassportRow("id") & ", 0,1,11)"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_Categories([IDPassport], [IDCategory], [LevelOfAuthority], [ShowFromLevel])  VALUES (" & oPassportRow("id") & ", 1,1,11)"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_Categories([IDPassport], [IDCategory], [LevelOfAuthority], [ShowFromLevel])  VALUES (" & oPassportRow("id") & ", 2,1,11)"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_Categories([IDPassport], [IDCategory], [LevelOfAuthority], [ShowFromLevel])  VALUES (" & oPassportRow("id") & ", 3,1,11)"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_Categories([IDPassport], [IDCategory], [LevelOfAuthority], [ShowFromLevel])  VALUES (" & oPassportRow("id") & ", 4,1,11)"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_Categories([IDPassport], [IDCategory], [LevelOfAuthority], [ShowFromLevel])  VALUES (" & oPassportRow("id") & ", 5,1,11)"
                    ExecuteSqlWithoutTimeOut(strSQL)
                    strSQL = "@INSERT# INTO [dbo].sysroPassports_Categories([IDPassport], [IDCategory], [LevelOfAuthority], [ShowFromLevel])  VALUES (" & oPassportRow("id") & ", 6,1,11)"
                    ExecuteSqlWithoutTimeOut(strSQL)
                Else
                    ' ASIGNAMOS LOS PERMISOS QUE TIENE EL SUPERVISOR SOBRE LOS GRUPOS, A PARTIR DE LOS GRUPOS QUE TIENE ASIGNADOS
                    Dim oDirectGroups As New Generic.List(Of Double)
                    Dim oParentGroups As New Generic.List(Of Double)
                    Dim oDirectGroupsPaths As New Generic.List(Of String)

                    If GetGroupsFromPassportV3(oPassportRow("ID"), oDirectGroups, oParentGroups, oDirectGroupsPaths) Then
                        ' NOS GUARDAMOS TODOS LOS ID'S DE GRUPOS ASIGNADOS DIRECTAMENTE Y SUS PADRES
                        Dim strIDGroups As String = "-1"
                        For intRow As Integer = 0 To oDirectGroups.Count - 1
                            strIDGroups += "," & oDirectGroups(intRow).ToString
                        Next
                        For intRow As Integer = 0 To oParentGroups.Count - 1
                            strIDGroups += "," & oParentGroups(intRow).ToString
                        Next

                        ' PARA CADA FUNCIONALIDAD GENERADA PARA EL SUPERVISOR QUE SEA DE TIPO EMPLEADO
                        ' SE DEBE GENERAR UN PERMISO SOBRE CADA UNO DE LOS GRUPOS QUE TIENE ASIGNADO,
                        ' CON EL MISMO PERMISO QUE TIENE EN LA FUNCIONALIDAD
                        strSQL = "@INSERT# INTO sysroPassports_PermissionsOverGroups (IDPassport,IDGroup,IDApplication,Permission) " &
                        " @SELECT# IDPassport, Groups.ID As IDGroup, sysroPassports_PermissionsOverFeatures.IDFeature, Case When sysroPassports_PermissionsOverFeatures.Permission = 9 Then 6 Else sysroPassports_PermissionsOverFeatures.Permission End " &
                        " FROM sysroPassports_PermissionsOverFeatures, Groups WHERE IDPassport= " & oPassportRow("IDParentPassport").ToString &
                        " And IDFeature In(@SELECT# ID FROM sysroFeatures WHERE AppHasPermissionsOverEmployees = 1)" &
                        " And Groups.ID In(" & strIDGroups & ")"
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                        If bolRet Then
                            ' PARA EL RESTO DE GRUPOS QUE NO TIENE ASIGNADO EL SUPERVISOR SE DEBE GENERAR UN REGISTRO SIN PERMISO
                            strSQL = "@INSERT# INTO sysroPassports_PermissionsOverGroups (IDPassport,IDGroup,IDApplication,Permission) " &
                            " @SELECT# IDPassport, Groups.ID As IDGroup, sysroPassports_PermissionsOverFeatures.IDFeature, 0 " &
                            " FROM sysroPassports_PermissionsOverFeatures, Groups WHERE IDPassport= " & oPassportRow("IDParentPassport").ToString &
                            " And IDFeature In(@SELECT# ID FROM sysroFeatures WHERE AppHasPermissionsOverEmployees = 1)" &
                            " And Groups.ID Not In(" & strIDGroups & ")"
                            For intRow As Integer = 0 To oDirectGroupsPaths.Count - 1
                                strSQL += " And Groups.Path Not Like '" & oDirectGroupsPaths(intRow).ToString & "\%'"
                            Next

                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        End If

                        If bolRet Then
                            ' FINALMENTE DEBEMOS GENERAR EN CASO NECESARIO LA EXCEPCIÓN SOBRE EL MISMO, SI NO SE PUEDE AUTOADMINISTRAR
                            strSQL = "@DELETE# sysroPassports_PermissionsOverEmployees WHERE IDPassport =" & oPassportRow("IDParentPassport").ToString
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                            If Not Any2Boolean(oPassportRow("CanApproveOwnRequests")) And Any2Integer(oPassportRow("IDEmployee")) > 0 Then
                                strSQL = "@INSERT# INTO sysroPassports_PermissionsOverEmployees (IDApplication,IDPassport,IDEmployee,Permission) " & " @SELECT# distinct sysroFeatures.EmployeeFeatureID, " & oPassportRow("IDParentPassport").ToString & ", " & oPassportRow("IDEmployee").ToString & ", 0  From dbo.sysroFeatures inner join sysroRequestType  ON dbo.sysroFeatures.AliasId = sysroRequestType.IdType "
                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                            End If

                        End If
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::GenerateSecurityPermissionsForPassportV3 :", ex)
            End Try

            Return bolRet
        End Function

        Private Shared Function GetGroupsFromPassportV3(ByVal IDPassport As Integer, ByRef _DirectGroups As Generic.List(Of Double), ByRef _ParentGroups As Generic.List(Of Double), ByRef _DirectGroupsPaths As Generic.List(Of String)) As Boolean
            Dim bolRet As Boolean = False
            Try

                ' Obtenemos los Departamentos asignados directamente al Supervisor
                Dim strSQL As String = "@SELECT# ID , PATH FROM GROUPS WHERE ID IN( @SELECT# DISTINCT IDGroup FROM sysroPassports_Groups WHERE " &
                " IDPassport=" & IDPassport.ToString & " )"

                Dim tb As DataTable = CreateDataTableWithoutTimeouts(strSQL, )
                If tb IsNot Nothing Then
                    If tb.Rows.Count > 0 Then
                        For Each oGroup As DataRow In tb.Rows
                            If Not _DirectGroups.Contains(roTypes.Any2Double(oGroup("ID"))) Then
                                ' Nos guardamos el ID del grupo asignado directamente al Supervisor
                                _DirectGroups.Add(roTypes.Any2Double(oGroup("ID")))

                                If Not _DirectGroupsPaths.Contains(roTypes.Any2String(oGroup("Path"))) Then
                                    _DirectGroupsPaths.Add(roTypes.Any2String(oGroup("Path")))
                                End If

                                Dim strPath As String = roTypes.Any2String(oGroup("Path"))
                                Dim dblParentID As Double = 0
                                If strPath.Contains("\") Then
                                    ' De cada grupo, obtenemos todos sus grupos padres
                                    For intPath As Integer = 0 To StringItemsCount(strPath, "\") - 2
                                        dblParentID = Any2Double(String2Item(strPath, intPath, "\"))
                                        If Not _ParentGroups.Contains(dblParentID) Then
                                            _ParentGroups.Add(dblParentID)
                                        End If
                                    Next
                                End If
                            End If
                        Next
                    End If
                End If

                bolRet = True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::GetGroupsFromPassportV3 :", ex)
            End Try

            Return bolRet

        End Function

        Public Shared Function SynchronizeTerminals(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim strSql = "@SELECT# REPLACE(ID, 'USERTASK:\\TERMINAL_BROADCAST_ERROR', '') AS IDTerminal " &
                             "FROM sysroUserTasks " &
                             "WHERE ID LIKE 'USERTASK:\\TERMINAL_BROADCAST_ERROR%'"
                Dim tblAux As DataTable = CreateDataTable(strSql, )

                For Each row As DataRow In tblAux.Rows
                    Dim strParamsXML As String = "<?xml version=""1.0""?><roCollection version=""2.0""><Item key=""Command"" type=""8"">RESET_TERMINAL</Item>" &
                                            "<Item key=""IDTerminal"" type=""2"">" & roTypes.Any2Integer(row.Item(0)) & "</Item></roCollection>"

                    roConnector.InitTask(TasksType.BROADCASTER, New roCollection(strParamsXML))
                Next

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::SynchronizeTerminals:Error launching broadcaster")
                End If
            Catch ex As Exception
                bolRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::SynchronizeTerminals :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function CheckDocuments(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False
            Dim lAlreadyThreated As New List(Of Integer)

            Try
                Dim oServerLicense As New roServerLicense
                Dim doclicense As Boolean = (oServerLicense.FeatureIsInstalled("Feature\Documents") OrElse oServerLicense.FeatureIsInstalled("Feature\Absences"))

                If doclicense Then
                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                    Dim oDocumentManager = New roDocumentManager()
                    Dim oDocument = New roDocument

                    ' Documentos que deben pasar a estado caducado
                    ' Para los docs de ausencias, consideramos que está caducado sólo si caduca dentro del periodo de la ausencia.
                    Dim strUpdateSQL As String
                    Dim strCountSQL As String

                    strUpdateSQL = "@SELECT# TOP 20 doc.Id " &
                                        "FROM Documents doc " &
                                        "LEFT JOIN ProgrammedAbsences pa ON pa.AbsenceID = doc.IdDaysAbsence " &
                                        "LEFT JOIN ProgrammedCauses pc ON pc.AbsenceID = doc.IdHoursAbsence " &
                                        "LEFT JOIN ProgrammedOvertimes po ON po.Id = doc.IdOvertimeForecast " &
                                        "WHERE  " &
                                        "Status = " & DocumentStatus.Validated &
                                        "AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) > doc.EndDate " &
                                        "AND (pa.AbsenceID IS NULL OR doc.EndDate < CASE WHEN pa.AbsenceID IS NULL THEN DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) ELSE isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate)) END)" &
                                        "AND (pc.AbsenceID IS NULL OR doc.EndDate < CASE WHEN pc.AbsenceID IS NULL THEN DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) ELSE pc.FinishDate END)" &
                                        "AND (po.ID Is NULL OR doc.EndDate < CASE WHEN po.ID IS NULL THEN DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) ELSE po.EndDate END)"

                    strCountSQL = "@SELECT# COUNT(*) " &
                                        "FROM Documents doc " &
                                        "LEFT JOIN ProgrammedAbsences pa ON pa.AbsenceID = doc.IdDaysAbsence " &
                                        "LEFT JOIN ProgrammedCauses pc ON pc.AbsenceID = doc.IdHoursAbsence " &
                                        "LEFT JOIN ProgrammedOvertimes po ON po.Id = doc.IdOvertimeForecast " &
                                        "WHERE  " &
                                        "Status = " & DocumentStatus.Validated &
                                        "AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) > doc.EndDate " &
                                        "AND (pa.AbsenceID IS NULL OR doc.EndDate < CASE WHEN pa.AbsenceID IS NULL THEN DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) ELSE isnull(pa.FinishDate,DATEADD(day,pa.maxlastingdays-1,pa.begindate)) END)" &
                                        "AND (pc.AbsenceID IS NULL OR doc.EndDate < CASE WHEN pc.AbsenceID IS NULL THEN DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) ELSE pc.FinishDate END)" &
                                        "AND (po.ID Is NULL OR doc.EndDate < CASE WHEN po.ID IS NULL THEN DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) ELSE po.EndDate END)"
                    lAlreadyThreated = New List(Of Integer)
                    While (Any2Integer(ExecuteScalar(strCountSQL)) > 0 AndAlso bolRet)
                        Dim dtDocuments = CreateDataTable(strUpdateSQL)
                        If (dtDocuments IsNot Nothing AndAlso dtDocuments.Rows.Count > 0) Then
                            For Each documentRow In dtDocuments.Rows
                                If lAlreadyThreated.Contains(Any2Integer(documentRow("Id"))) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckDocuments: Maybe in endless loop. Skiping expiring document with id " & Any2String(documentRow("Id")))
                                    Exit While
                                End If
                                lAlreadyThreated.Add(Any2Integer(documentRow("Id")))
                                bolRet = oDocumentManager.ChangeDocumentState(Any2Integer(documentRow("Id")), DocumentStatus.Expired, "", Date.Now, False)
                            Next
                        End If
                    End While

                    ' Documentos que pueden haberse activado
                    strUpdateSQL = "@SELECT# top 20 Id from Documents where GETDATE() between BeginDate and DATEADD(DAY,1,EndDate) and status = " & DocumentStatus.Validated & " and CurrentlyValid = " & DocumentValidity.ValidOnFuture & " order by id asc"
                    strCountSQL = "@SELECT# count(*) from Documents where GETDATE() between BeginDate and DATEADD(DAY,1,EndDate) and status = " & DocumentStatus.Validated & " and CurrentlyValid = " & DocumentValidity.ValidOnFuture
                    Dim iLastIdChecked As Integer = 0
                    lAlreadyThreated = New List(Of Integer)
                    While (Any2Integer(ExecuteScalar(strCountSQL)) > 0 AndAlso bolRet)
                        Dim dtDocuments = CreateDataTable(strUpdateSQL)
                        If (dtDocuments IsNot Nothing AndAlso dtDocuments.Rows.Count > 0) Then
                            For Each documentRow In dtDocuments.Rows
                                If lAlreadyThreated.Contains(Any2Integer(documentRow("Id"))) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckDocuments: Maybe in endless loop. Skiping activating document with id " & Any2String(documentRow("Id")))
                                    Exit While
                                End If
                                lAlreadyThreated.Add(Any2Integer(documentRow("Id")))
                                oDocument = oDocumentManager.LoadDocument(Any2Integer(documentRow("Id")))
                                iLastIdChecked = Any2Integer(documentRow("Id"))
                                bolRet = oDocumentManager.SaveDocument(oDocument)
                            Next
                            strUpdateSQL = "@SELECT# top 20 Id from Documents where GETDATE() between BeginDate and EndDate and status = " & DocumentStatus.Validated & " and CurrentlyValid = " & DocumentValidity.ValidOnFuture & " and id > " & iLastIdChecked.ToString & " order by id asc"
                            strCountSQL = "@SELECT# count(*) from Documents where GETDATE() between BeginDate and EndDate and status = " & DocumentStatus.Validated & " and CurrentlyValid = " & DocumentValidity.ValidOnFuture & " and id > " & iLastIdChecked.ToString
                        End If
                    End While

                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                End If
                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckDocuments:SQL error updating documents on DB")
                End If
            Catch ex As Exception
                bolRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CheckDocuments :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function DocumentTracking(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False
            Dim iDaysInAdvance As Integer = 0
            Dim param As AdvancedParameter.roAdvancedParameter = Nothing

            Try
                ' Verificamos modo de validación de documentos de absentismo
                Dim oDocumentManager = New roDocumentManager()
                Dim oDocument As roDocument = New roDocument
                Dim oDocAlerts As New DTOs.DocumentAlerts

                Dim oServerLicense As New roServerLicense
                Dim doclicense As Boolean = (oServerLicense.FeatureIsInstalled("Feature\Documents") OrElse oServerLicense.FeatureIsInstalled("Feature\Absences"))

                If doclicense Then

                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    iDaysInAdvance = 2
                    ' Recuperamos días de antelación ...
                    param = New AdvancedParameter.roAdvancedParameter("Documents.DaysInAdvanceForForecastAdvice", New AdvancedParameter.roAdvancedParameterState)
                    If param.Value <> "" AndAlso roTypes.Any2Integer(param.Value) >= 0 Then
                        iDaysInAdvance = roTypes.Any2Integer(param.Value)
                    End If

                    oDocAlerts = oDocumentManager.GetDocumentationFaultAlerts(-1, DTOs.DocumentType.Employee, 0, False, Now.Date.AddDays(iDaysInAdvance), ForecastType.Any)

                    ' Creamos un array con las alertas de documentos de absentismo y de previsión de trabajo
                    Dim oForecastAlertDocs As New Generic.List(Of DocumentAlert)
                    oForecastAlertDocs.AddRange(oDocAlerts.AbsenteeismDocuments)
                    oForecastAlertDocs.AddRange(oDocAlerts.WorkForecastDocuments)

                    ' Documentos de absentismo
                    For Each oDocAlert As DTOs.DocumentAlert In oForecastAlertDocs.ToArray
                        'Recuper documento y/o plantilla
                        oDocument = oDocumentManager.LoadDocument(oDocAlert.IDDocument)

                        If oDocument.Id = -1 Then
                            ' No se entregó el documento
                            oDocument.DocumentTemplate = oDocumentManager.LoadDocumentTemplate(oDocAlert.IDDocumentTemplate)
                        End If

                        Dim sType As String = String.Empty

                        'Si no se ha entregado documento y la plantilla de documento tiene habilitada la notifiación, y esta no existe ya, la creo ahora...
                        'If oDocument.Id = -1 AndAlso oDocument.DocumentTemplate.Notifications.Contains("701") Then
                        If oDocument.Id = -1 AndAlso Not oDocument.DocumentTemplate Is Nothing AndAlso Not oDocument.DocumentTemplate.Notifications Is Nothing AndAlso oDocument.DocumentTemplate.Notifications.Contains("701") Then
                            Dim strSQL As String = "@SELECT# IsNull(Activated,0) As Active from Notifications where ID = 701"
                            Dim isActive As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSQL))
                            Dim idForecast As Integer = 0
                            If isActive Then
                                strSQL = "@SELECT# top 1 * from sysronotificationtasks where idnotification = 701 and Key1Numeric = " & oDocAlert.IdRelatedObject & " " &
                                        " and Key2Numeric = " & oDocument.DocumentTemplate.Id
                                If oDocAlert.IdDaysAbsence > 0 Then
                                    sType = "DAYS"
                                    idForecast = oDocAlert.IdDaysAbsence
                                    strSQL = strSQL & " and Key5Numeric = " & oDocAlert.IdDaysAbsence & " and Parameters like 'DAYS@%' "
                                ElseIf oDocAlert.IdHoursAbsence > 0 Then
                                    sType = "HOURS"
                                    idForecast = oDocAlert.IdHoursAbsence
                                    strSQL = strSQL & " and Key5Numeric = " & oDocAlert.IdHoursAbsence & " and Parameters like 'HOURS@%' "
                                ElseIf oDocAlert.IdOvertimeForecast > 0 Then
                                    sType = "OVERTIME"
                                    idForecast = oDocAlert.IdOvertimeForecast
                                    strSQL = strSQL & " and Key5Numeric = " & oDocAlert.IdOvertimeForecast & " and Parameters like 'OVERTIME@%' "
                                ElseIf oDocAlert.IdRequest > 0 Then
                                    sType = "REQUEST"
                                    idForecast = oDocAlert.IdRequest
                                    strSQL = strSQL & " and Key5Numeric = " & oDocAlert.IdRequest & " and Parameters like 'REQUEST@%' "
                                End If

                                Dim alreadyexists As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSQL))

                                If Not alreadyexists Then
                                    strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Key5Numeric, Key3DateTime, Parameters ) VALUES " &
                                                            "(701, " & oDocAlert.IdRelatedObject & "," & oDocAlert.IDDocumentTemplate & "," & idForecast & "," & roTypes.Any2Time(oDocAlert.DateTime).SQLSmallDateTime & ",'" & sType & "@" & oDocument.DocumentTemplate.Name & "')"
                                    bolRet = ExecuteSql(strSQL)
                                End If
                            End If
                        End If
                    Next

                    ' Resto de documentos (los que no son de absentismo)
                    For Each oDocAlert As DTOs.DocumentAlert In oDocAlerts.MandatoryDocuments
                        'Recuperamos plantilla
                        Dim oDocumentTemplate As roDocumentTemplate = New roDocumentTemplate
                        oDocumentTemplate = oDocumentManager.LoadDocumentTemplate(oDocAlert.IDDocumentTemplate)

                        Dim sType As String = String.Empty

                        'Si la plantilla de documento tiene habilitada la notifiación, y esta no existe ya, la creo ahora...
                        If Not oDocumentTemplate Is Nothing AndAlso Not oDocumentTemplate.Notifications Is Nothing AndAlso oDocumentTemplate.Notifications.Contains("701") Then
                            Dim strSQL As String = "@SELECT# IsNull(Activated,0) As Active from Notifications where ID = 701"
                            Dim isActive As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSQL))
                            Dim idAbsence As Integer = 0
                            If isActive Then
                                strSQL = "@SELECT# top 1 * from sysronotificationtasks where idnotification = 701 and Key1Numeric = " & oDocAlert.IdRelatedObject & " " &
                                " and Key2Numeric = " & oDocAlert.IDDocumentTemplate
                                sType = "OTHER"
                                strSQL = strSQL & " and Parameters like '" & sType & "@%' "

                                Dim alreadyexists As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSQL))
                                If Not alreadyexists Then
                                    strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime, Parameters ) VALUES " &
                                                    "(701, " & oDocAlert.IdRelatedObject & "," & oDocAlert.IDDocumentTemplate & "," & roTypes.Any2Time(oDocAlert.DateTime).SQLSmallDateTime & ",'" & sType & "@" & oDocumentTemplate.Name & "')"
                                    bolRet = ExecuteSql(strSQL)
                                End If
                            End If
                        End If
                    Next

                    ' Documentos de autorizaciones de acceso
                    Dim tmpArray As New List(Of DocumentAlert)
                    iDaysInAdvance = 7
                    ' Recuperamos días de antelación ...
                    param = New AdvancedParameter.roAdvancedParameter("Documents.DaysInAdvanceForPRLAdvice", New AdvancedParameter.roAdvancedParameterState)
                    If roTypes.Any2Integer(param.Value) > 0 Then
                        iDaysInAdvance = roTypes.Any2Integer(param.Value)
                    End If
                    oDocAlerts = oDocumentManager.GetDocumentationFaultAlerts(-1, DTOs.DocumentType.Employee, 0, False, Now.Date.AddDays(iDaysInAdvance))
                    tmpArray.AddRange(oDocAlerts.AccessAuthorizationDocuments)
                    oDocAlerts = oDocumentManager.GetDocumentationFaultAlerts(-1, DTOs.DocumentType.Company, 0, False, Now.Date.AddDays(iDaysInAdvance))
                    tmpArray.AddRange(oDocAlerts.AccessAuthorizationDocuments)
                    For Each oDocAlert As DTOs.DocumentAlert In tmpArray
                        'Recuper documento y/o plantilla
                        oDocument = oDocumentManager.LoadDocument(oDocAlert.IDDocument)

                        If oDocument.Id = -1 Then
                            ' No se entregó el documento
                            oDocument.DocumentTemplate = oDocumentManager.LoadDocumentTemplate(oDocAlert.IDDocumentTemplate)
                        End If

                        Dim sType As String = String.Empty

                        'Si no se ha entregado documento y la plantilla de documento tiene habilitada la notifiación, y esta no existe ya, la creo ahora...
                        'If oDocument.Id = -1 AndAlso oDocument.DocumentTemplate.Notifications.Contains("701") Then
                        If Not oDocument.DocumentTemplate Is Nothing AndAlso Not oDocument.DocumentTemplate.Notifications Is Nothing AndAlso oDocument.DocumentTemplate.Notifications.Contains("702") Then
                            Dim strSQL As String = "@SELECT# IsNull(Activated,0) As Active from Notifications where ID = 702"
                            Dim isActive As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSQL))
                            If isActive Then
                                strSQL = "@SELECT# top 1 * from sysronotificationtasks where idnotification = 702 and Key1Numeric = " & oDocAlert.IdRelatedObject & " " &
                                " and Key2Numeric = " & oDocument.DocumentTemplate.Id
                                If oDocAlert.Scope = DTOs.DocumentScope.EmployeeAccessAuthorization Then
                                    sType = "EMPLOYEE"
                                    strSQL = strSQL & " and Parameters like 'EMPLOYEE@%' "
                                ElseIf oDocAlert.Scope = DTOs.DocumentScope.CompanyAccessAuthorization Then
                                    sType = "COMPANY"
                                    strSQL = strSQL & " and Parameters like 'COMPANY@%' "
                                End If

                                Dim alreadyexists As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSQL))

                                If Not alreadyexists Then
                                    strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime, Parameters ) VALUES " &
                                                    "(702, " & oDocAlert.IdRelatedObject & "," & oDocAlert.IDDocumentTemplate & "," & roTypes.Any2Time(Now.Date.AddDays(iDaysInAdvance)).SQLSmallDateTime & ",'" & sType & "@" & oDocument.DocumentTemplate.Name & "@" & oDocument.Id & "')"
                                    bolRet = ExecuteSql(strSQL)
                                End If
                            End If
                        End If
                    Next

                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                Else
                    bolRet = True
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DocumentTracking:SQL error updating documents on DB")
                End If

            Catch ex As Exception
                bolRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::DocumentTracking :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function CheckScheduleRulesFaults(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strSql As String = String.Empty
            Dim oParam As AdvancedParameter.roAdvancedParameter = Nothing
            Dim tblNotifications As DataTable

            Try
                Dim tblAux As New DataTable
                Dim iRules As Integer = 0

                ' Sólo proceso si hay una notificación activa y hay reglas
                strSql = "@SELECT# * FROM Notifications WHERE Activated=1 AND IDType = 66"
                tblNotifications = CreateDataTable(strSql, )
                If tblNotifications IsNot Nothing AndAlso tblNotifications.Rows.Count > 0 Then

                    ' No proceso nada si hay solicitudes pendientes de procesar, porque esto podría generar nuevos indictments
                    strSql = "@SELECT# ID FROM Requests WHERE AutomaticValidation = 1 AND ((Status=" & eRequestStatus.Pending & " AND RequestType <> " & eRequestType.ExchangeShiftBetweenEmployees & ") OR (Status=" & eRequestStatus.OnGoing & " AND RequestType = " & eRequestType.ExchangeShiftBetweenEmployees & "))" & " AND ValidationDate <= " & Any2Time(Now.Date).SQLSmallDateTime & " ORDER BY isnull(ValidationDate, RequestDate) , RequestDate "
                    tblAux = CreateDataTableWithoutTimeouts(strSql, )

                    If tblAux.Rows.Count = 0 Then

                        strSql = "@SELECT# count(*) from ScheduleRules where enabled = 1"
                        iRules = ExecuteScalar(strSql)

                        If iRules > 0 Then
                            ' Cargamos calendario para todos los empleados y para todo el año actual
                            Dim oCalendarState As Robotics.Base.VTCalendar.roCalendarState = New Robotics.Base.VTCalendar.roCalendarState(1)
                            Dim oCalendarManager As New Robotics.Base.VTCalendar.roCalendarManager(oCalendarState)
                            Dim oCalendarScheduleRulesManager As New Robotics.Base.VTCalendar.roCalendarScheduleRulesManager(New Robotics.Base.VTCalendar.roCalendarScheduleRulesState(oTask.IDPassport))
                            Dim oCalendar As New DTOs.roCalendar
                            Dim oParameters As New roParameters("OPTIONS", True)
                            Dim iMonthIniDay As Integer = 0
                            Dim iYearIniMonth As Integer = 0
                            Dim dYearFirstDate As Date

                            ' Calcularé alertas cargando todo el año en curso ayer
                            dYearFirstDate = oCalendarScheduleRulesManager.GetYearFirstDate(Now.Date.AddDays(-1), oParameters, iMonthIniDay, iYearIniMonth)

                            ' Calculo los empleados que han tenido contrato algún día en el periodo
                            Dim oEmployeesIDs As List(Of Integer) = roSelector.GetEmployeeListByContract(oTask.IDPassport, "", "", Nothing, "ALL", "", False, dYearFirstDate, dYearFirstDate.AddYears(1).AddDays(-1))

                            ' Carga de calendario
                            oCalendar = oCalendarManager.Load(dYearFirstDate, dYearFirstDate.AddYears(1).AddDays(-1), "B" & String.Join(",", oEmployeesIDs).Replace(",", ",B"), DTOs.CalendarView.Planification, DTOs.CalendarDetailLevel.Daily, True)

                            ' Carga de Indictments
                            ' Primero miro qué reglas se deben notificar
                            Dim sRulesToNotify As String = String.Empty
                            oParam = New AdvancedParameter.roAdvancedParameter("VTLive.Notifications.RulesToNotify", New AdvancedParameter.roAdvancedParameterState(oTask.IDPassport))
                            sRulesToNotify = roTypes.Any2String(oParam.Value)

                            Dim oIndictments As New List(Of roCalendarScheduleIndictment)
                            oIndictments = oCalendarScheduleRulesManager.CheckScheduleRules(oCalendar,, sRulesToNotify)

                            Dim idNotification As Integer = 0
                            Dim iKey1Numeric As Integer = 0
                            Dim iKey2Numeric As Integer = 0
                            Dim dKey3DateTime As Date = Date.MinValue
                            Dim dKey4DateTime As Date = Date.MinValue
                            Dim sParameters As String = String.Empty

                            ' Miro si hay fecha de corte de notificación
                            Dim dRulesNotifyFrom As Date = Date.MinValue
                            Dim bNotify As Boolean = True
                            oParam = New AdvancedParameter.roAdvancedParameter("VTLive.Notifications.RulesNotifyFrom", New AdvancedParameter.roAdvancedParameterState(oTask.IDPassport))
                            dRulesNotifyFrom = roTypes.Any2DateTime(oParam.Value)

                            For Each rNotification As DataRow In tblNotifications.Rows
                                idNotification = roTypes.Any2Integer(rNotification("ID"))
                                For Each oIndictment As roCalendarScheduleIndictment In oIndictments
                                    If dRulesNotifyFrom <> Date.MinValue AndAlso oIndictment.Dates.Length > 0 Then
                                        bNotify = (oIndictment.Dates.ToList.Max >= dRulesNotifyFrom)
                                    Else
                                        bNotify = (oIndictment.DateBegin >= dRulesNotifyFrom)
                                    End If
                                    If bNotify Then
                                        iKey1Numeric = oIndictment.IDEmployee
                                        iKey2Numeric = oIndictment.IDScheduleRule
                                        dKey3DateTime = oIndictment.DateBegin
                                        dKey4DateTime = oIndictment.DateEnd
                                        sParameters = oIndictment.RuleName & "@" & oIndictment.ErrorText
                                        strSql = "@SELECT# top 1 * from sysronotificationtasks where idnotification = " & idNotification.ToString & " and Key1Numeric = " & iKey1Numeric.ToString & " " &
                                                    " and Key2Numeric = " & iKey2Numeric.ToString & " and Key3DateTime = " & roTypes.Any2Time(dKey3DateTime).SQLSmallDateTime &
                                                    " and Key4DateTime = " & roTypes.Any2Time(dKey4DateTime).SQLSmallDateTime &
                                                    " and convert(varchar(max),Parameters) = '" & sParameters & "'"
                                        Dim alreadyexists As Boolean = roTypes.Any2Boolean(ExecuteScalar(strSql))
                                        If Not alreadyexists Then
                                            strSql = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime, Key4DateTime, Parameters ) VALUES " &
                                                                        "(" & idNotification.ToString & ", " & iKey1Numeric.ToString & "," & iKey2Numeric.ToString & "," & roTypes.Any2Time(dKey3DateTime).SQLSmallDateTime & "," & roTypes.Any2Time(dKey4DateTime).SQLSmallDateTime & ",'" & sParameters & "')"
                                            bolRet = ExecuteSql(strSql)
                                        End If
                                    End If
                                Next
                            Next
                        End If


                        If Not bolRet Then
                            roLog.GetInstance.logMessage(roLog.EventType.roDebug, "roEogManager::CheckScheduleRulesFaults:Error checking sechedule rules")
                        End If
                    End If
                End If

            Catch ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roEogManager::CheckScheduleRulesFaults :", ex)
            End Try


            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function CheckInvalidEntries(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Try
                Dim tb As DataTable = Punch.roPunch.GetInvalidEntries(New Punch.roPunchState())
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    ' Si no existe la tarea la creo
                    Dim oUsrState As New VTBusiness.UserTask.roUserTaskState()
                    Dim oTaskExist As New VTBusiness.UserTask.roUserTask(UserTask.roUserTask.roUserTaskObject & ":\\BAD_ENTRIES", oUsrState)
                    If oTaskExist.Message = "" Then
                        Dim oSettings As New roSettings

                        Dim oUserTask As New VTBusiness.UserTask.roUserTask()
                        oUserTask.ID = UserTask.roUserTask.roUserTaskObject & ":\\BAD_ENTRIES"
                        oUserTask.DateCreated = Now
                        oUserTask.TaskType = VTBusiness.UserTask.TaskType.UserTaskRepair
                        oUserTask.ResolverURL = "FN:\\Resolver_ParserInvalidEntries"
                        oUserTask.Message = Message("Collector.Process.AddBadLine.Task.Message", , , oSettings.GetVTSetting(eKeys.DefaultLanguage))

                        oUserTask.Save()
                    End If
                End If

                ' Borrado de notificaciones de Fichajes impares (1001)
                Dim sSQL As String = "@delete# sysroNotificationTasks where ID in ( " &
                                               " @SELECT# noti.id from " &
                                               "(@SELECT# * from sysroNotificationTasks with (nolock) where IDNotification = 1001) noti " &
                                               "full outer join " &
                                               "(@SELECT# * from sysrovwIncompletedDays with (nolock) ) as incom " &
                                               "on incom.IDEmployee = noti.Key1Numeric and incom.date = noti.Key3DateTime " &
                                               "where incom.IDEmployee Is Null" &
                                               ")"
                If ExecuteSqlWithoutTimeOut(sSQL) Then
                    ' Actualizo la fecha de último borrado de alertas para el correcto refresco de pantallas
                    sSQL = "@update# notifications set LastTaskDeleted = " & Any2Time(Now).SQLDateTime & " where id = 1001"
                    ExecuteSql(sSQL)
                End If

                ' Borrado de notificaciones de Fichajes no fiables (1002)
                sSQL = "@delete#  sysroNotificationTasks where ID in ( " &
                               "@SELECT# noti.id from " &
                               "(@SELECT# * from sysroNotificationTasks with (nolock) where IDNotification = 1002) noti " &
                               "full outer join " &
                               "(@SELECT# * from punches with (nolock) where IsNotReliable = 1) as punch " &
                               "on punch.IDEmployee = noti.Key1Numeric and punch.shiftdate = noti.Key3DateTime " &
                               "Where punch.IDEmployee Is Null" &
                               ")"
                If ExecuteSqlWithoutTimeOut(sSQL) Then
                    ' Actualizo la fecha de último borrado de alertas para el correcto refresco de pantallas
                    sSQL = "@update#  notifications set LastTaskDeleted = " & Any2Time(Now).SQLDateTime & " where id = 1002"
                    ExecuteSql(sSQL)
                End If

                ' Borrado de notificaciones de Vacaciones en ausencia
                sSQL = "@delete# sysroNotificationTasks " &
                               " From sysroNotificationTasks " &
                               " inner join notifications with (nolock) on notifications.id = sysroNotificationTasks.IDNotification and Notifications.IDType = 65 " &
                               " where Key2Numeric Not In ( " &
                               "@SELECT# distinct(absenceid) from ProgrammedAbsences  pa with (nolock)" &
                               " right join dailyschedule  ds with (nolock) on pa.IDEmployee = ds.IDEmployee " &
                               " WHERE ds.IsHolidays = 1 and ds.Date between pa.BeginDate and CASE WHEN pa.FinishDate IS NULL THEN DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate) ELSE pa.FinishDate END " &
                               " AND pa.BeginDate >=" & Any2Time(Now.Date).Add(-90, "d").SQLSmallDateTime & " " &
                               ") "

                If ExecuteSqlWithoutTimeOut(sSQL) Then
                    ' Actualizo la fecha de último borrado de alertas para el correcto refresco de pantallas
                    sSQL = "@update# notifications set LastTaskDeleted = " & Any2Time(Now).SQLDateTime & " where id = 65"
                    ExecuteSql(sSQL)
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckInvalidEntries:Error checking invalid entries")
                End If
            Catch ex As Exception
                bolRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CheckInvalidEntries :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function DeleteOldAudit(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try

                ' Tambien borramos los datos de los empleados que actualmente estan de baja y que tengan datos fuera de sus contratos
                Dim strDeleteSQL As String = "@SELECT# ID from Employees with (nolock) " &
                                        " LEFT JOIN sysrovwCurrentOrFutureEmployeePeriod with (nolock) on Employees.ID = sysrovwCurrentOrFutureEmployeePeriod.IDEmployee  " &
                                        " WHERE sysrovwCurrentOrFutureEmployeePeriod.IDEmployee IS NULL " &
                                            " AND Employees.ID IN( " &
                                                " @SELECT#  distinct idemployee from DailySchedule with (nolock) where not exists (@SELECT# 1 from EmployeeContracts with (nolock) where EmployeeContracts.IDEmployee = DailySchedule.IDEmployee and DailySchedule.date between EmployeeContracts.BeginDate and EmployeeContracts.EndDate )  )  ORDER BY ID "

                Dim tbUpdatedEmployees As DataTable = CreateDataTable(strDeleteSQL, Nothing, iTimeoutSeconds:=120)
                If tbUpdatedEmployees IsNot Nothing AndAlso tbUpdatedEmployees.Rows.Count > 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldAudit : RemoveDaysWithoutContract: " & tbUpdatedEmployees.Rows.Count & " employees")
                    Dim oContractState As New roContractState(-1)
                    For Each oEmployeeRow As DataRow In tbUpdatedEmployees.Rows
                        bolRet = bolRet AndAlso roContract.RemoveDaysWithoutContract(oEmployeeRow("ID"), oContractState)
                    Next
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldAudit::Could not delete old audit")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::DeleteOldAudit :", ex)
                bolRet = False
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function BlockInactivePassports(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strSQL As String

            Try
                Dim bBlockUser As Boolean = False
                Dim iBlockUserMonthsPeriod As Integer = 3
                Dim dDateMin As Date = Now.Date

                bBlockUser = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "VisualTime.Security.BlockUser"))
                If bBlockUser Then
                    iBlockUserMonthsPeriod = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "VisualTime.Security.BlockUserPeriod"))
                    dDateMin = Now.Date.AddMonths(-1 * iBlockUserMonthsPeriod)

                    ' Bloqueamos si aplica
                    Try
                        strSQL = "@UPDATE# sysroPassports_AuthenticationMethods SET BloquedAccessApp = 1, BlockedAccessByInactivity = 1 " &
                                    " WHERE Method = 1 AND ISNULL(BlockedAccessByInactivity,0) = 0 AND ISNULL(LastAppActionDate,GETDATE()) < " & roTypes.Any2Time(dDateMin).SQLSmallDateTime &
                                    " AND IdPassport NOT IN (@SELECT# ID FROM sysroPassports WHERE Description LIKE '@@ROBOTICS@@%')"
                        bolRet = ExecuteSql(strSQL)
                    Catch Ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roEOGManager::BlockInactivePassports:", Ex)
                    End Try
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::BlockInactivePassports::Could not check inactive passports")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::BlockInactivePassports :", ex)
                bolRet = False
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function CheckConcurrenceData(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim oSecState As New roSecurityState(-1)
                bolRet = SessionHelper.UpdateConcurrenceStatus(oSecState)
                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::CheckConcurrenceData::Could not check multitimezone")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::CheckConcurrenceData :", ex)
                bolRet = False
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function EmployeeSecurtiyActions(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim strErrorMsg As String = String.Empty
            Try
                Dim iAction As Integer = roTypes.Any2Integer(oTask.Parameters("iAction"))

                Dim strEmployeeFilter As String = roTypes.Any2String(oTask.Parameters("strEmployeeFilter"))
                Dim strFilters As String = roTypes.Any2String(oTask.Parameters("strFilters"))
                Dim strUserFieldFilters As String = roTypes.Any2String(oTask.Parameters("strUserFieldFilters"))
                Dim strFeature As String = roTypes.Any2String(oTask.Parameters("strFeature"))
                Dim iSourceEmployee As Integer = roTypes.Any2Integer(oTask.Parameters("iSourceEmployee"))
                Dim bLockAccess As Boolean = roTypes.Any2Boolean(oTask.Parameters("bLockAccess"))

                Dim bState As New roGroupState(oTask.IDPassport)

                Select Case iAction
                    Case 1
                        bolRet = roGroup.SetBloquedAccessAppToEmployees(strEmployeeFilter, strFeature, strFilters, strUserFieldFilters, bLockAccess, bState)
                    Case 2
                        bolRet = roGroup.RegeneratePasswordsToEmployees(strEmployeeFilter, strFeature, strFilters, strUserFieldFilters, bState)
                    Case 3
                        bolRet = roGroup.SetBloquedAccessAppToEmployees(strEmployeeFilter, strFeature, strFilters, strUserFieldFilters, bLockAccess, bState)
                    Case 4
                        bolRet = roGroup.SetPermissionsToEmployees(strEmployeeFilter, strFeature, strFilters, strUserFieldFilters, bLockAccess, bState, iSourceEmployee)
                    Case 5
                        bolRet = roGroup.SetAppConfigurationToEmployees(strEmployeeFilter, strFeature, strFilters, strUserFieldFilters, bLockAccess, bState, iSourceEmployee)
                    Case 6
                        bolRet = roGroup.SendUsernameToEmployees(strEmployeeFilter, strFeature, strFilters, strUserFieldFilters, bState)
                    Case 7
                        bolRet = roGroup.SendPinToEmployees(strEmployeeFilter, strFeature, strFilters, strUserFieldFilters, bState)
                End Select

                If Not bolRet Then
                    bolRet = False
                    strErrorMsg = "Could not handle securityActionTask"
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::BlockInactivePassports :", ex)
                bolRet = False
                strErrorMsg = ex.Message
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = strErrorMsg}
        End Function

        Public Shared Function RecalculateRequestStatus(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim oRequestType As eRequestType = roTypes.Any2Integer(oTask.Parameters("IDRequestType"))
                Dim intIDCause As Integer = roTypes.Any2Integer(oTask.Parameters("IDCause"))

                Dim oRequestState As New Requests.roRequestState(oTask.IDPassport)

                bolRet = Requests.roRequest.RecalculateStatus(oRequestState, oRequestType, intIDCause)

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::RecalculateRequestStatus :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function ChangeRequestPermissions(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim strMode As String = roTypes.Any2String(oTask.Parameters("Mode")).ToUpper

                ' Lanzamos tarea de generacion de permisos
                If strMode = "ALL" Then
                    ' Es el cambio de dia, hay que volver a generar todos los permisos sobre solicitudes
                    Dim Command As DbCommand = CreateCommand("ExecuteRequestPassportPermissionsAction")
                    Command.CommandType = CommandType.StoredProcedure
                    Command.CommandTimeout = 0

                    AddParameter(Command, "@IDAction", DbType.Int32, 1).Value = -1
                    AddParameter(Command, "@IDObject", DbType.Int32, 1).Value = -1
                    AddParameter(Command, "@Version", DbType.Int32, 1).Value = 3
                    Command.ExecuteNonQuery()
                Else
                    ' Revisamos las tareas pendientes sobre permisos de solicitudes
                    Dim sSql As String = "@SELECT# ID, IDAction, IDObject  FROM RequestPassportPermissionsPending Order by ID"
                    Dim dt As DataTable = CreateDataTable(sSql, )

                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                        For Each oRowaux As DataRow In dt.Rows
                            Dim Command As DbCommand = CreateCommand("ExecuteRequestPassportPermissionsAction")
                            Command.CommandType = CommandType.StoredProcedure
                            Command.CommandTimeout = 0
                            AddParameter(Command, "@IDAction", DbType.Int32, 1).Value = oRowaux("IDAction")
                            AddParameter(Command, "@IDObject", DbType.Int32, 1).Value = oRowaux("IDObject")
                            AddParameter(Command, "@Version", DbType.Int32, 1).Value = 3
                            Command.ExecuteNonQuery()

                            sSql = "@DELETE# FROM RequestPassportPermissionsPending where id = " & Any2String(oRowaux("ID"))
                            bolRet = ExecuteSql(sSql)

                            If Not bolRet Then Exit For
                        Next
                    End If
                End If

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::ChangeRequestPermissions::Invalid parameters")
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::ChangeRequestPermissions :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function DeleteOldComplaints(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True

            Try
                Dim fechaLimiteNormal As DateTime = DateTime.Now.AddMonths(-3)
                Dim fechaLimiteAlta As DateTime = DateTime.Now.AddMonths(-6)

                Dim oChannelsManager As New roChannelManager()
                bolRet = oChannelsManager.DeleteOldMessagesByComplexity(ConversationComplexity.High, fechaLimiteAlta)
                If (bolRet) Then bolRet = oChannelsManager.DeleteOldMessagesByComplexity(ConversationComplexity.Normal, fechaLimiteNormal)

                If Not bolRet Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEogManager::DeleteOldComplaints:SQL error deleting old biometric data on DB")
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::DeleteOldComplaints :", ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

#Region "Audit methods"

        Public Shared Function Message(ByVal strKey As String, Optional ByVal oParamList As ArrayList = Nothing, Optional ByVal strFileReference As String = "ProcessEOGServer", Optional ByVal strLanguageKey As String = "ESP") As String
            Static oLanguage As roLanguage = Nothing

            If oLanguage Is Nothing Then
                oLanguage = New roLanguage
            End If

            oLanguage.ClearUserTokens()
            If oParamList IsNot Nothing Then
                For i As Integer = 0 To oParamList.Count - 1
                    oLanguage.AddUserToken(oParamList(i))
                Next
            End If

            oLanguage.SetLanguageReference(strFileReference, strLanguageKey)
            Return oLanguage.Translate(strKey, "")
        End Function

#End Region

    End Class

End Namespace