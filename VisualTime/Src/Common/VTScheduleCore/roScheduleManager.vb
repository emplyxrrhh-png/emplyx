Imports System.Runtime.Serialization
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTScheduleManager

    <DataContract>
    Public Class roScheduleManager


        Public Sub New()
        End Sub

        Public Sub New(ByVal oState As roLiveTaskState)
        End Sub

#Region "Multitenant"

        Private Sub AddLiveTasktoTable(ByVal company As roCompanyConfiguration, ByRef tasksHash As Hashtable, ByVal oLiveTaskTypes As roLiveTaskTypes, ByVal IDTask As Integer)
            Try
                If IDTask = -1 Then Return

                If tasksHash.ContainsKey(oLiveTaskTypes) Then
                    Dim tmpTaskHash As Hashtable = tasksHash(oLiveTaskTypes)

                    If Not tmpTaskHash.ContainsKey(company.Id) Then tmpTaskHash.Add(company.Id, IDTask)
                Else
                    Dim tmpTaskHash As New Hashtable
                    tmpTaskHash.Add(company.Id, IDTask)

                    tasksHash.Add(oLiveTaskTypes, tmpTaskHash)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::AddLiveTasktoTable::Error: ", ex)
            End Try
        End Sub

        Private Function ExistsTaskForCompany(ByVal company As roCompanyConfiguration, ByRef tasksHash As Hashtable, ByVal oLiveTaskTypes As roLiveTaskTypes) As Boolean
            Dim bExists As Boolean = False
            Try

                If tasksHash.ContainsKey(oLiveTaskTypes) Then
                    Dim tmpTaskHash As Hashtable = tasksHash(oLiveTaskTypes)
                    If tmpTaskHash.ContainsKey(company.Id) Then bExists = True
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ExistsTaskForCompany::Error: ", ex)
            End Try

            Return bExists
        End Function

#Region "Persist tasks"

        Public Function RunPersistTasks(ByVal companies As roCompanyConfiguration(), ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer, ByVal splitGroupSize As Integer) As Boolean
            Dim bolRet As Boolean = True

            Try
                'Ejecutamos en paralelo la generación de tareas para evitar problemas
                Dim iSplitItems As New Generic.List(Of Integer)

                Dim iPos As Integer = 0
                iSplitItems.Add(iPos)
                iPos += splitGroupSize

                While iPos < companies.Length
                    iSplitItems.Add(iPos)
                    iPos += splitGroupSize
                End While

                roTrace.GetInstance().AddTraceEvent($"Execute persist tasks for {companies.Length} companies")
                Dim o11yDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
                Dim traceDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()
                Dim oThreadData As roThreadData = roConstants.BackupThreadData()

                System.Threading.Tasks.Parallel.ForEach(iSplitItems, Sub(number)
                                                                         Dim tmpCompanies As roCompanyConfiguration() = companies.Skip(number).Take(splitGroupSize).ToArray()
                                                                         ExecutePersistTasksParallel(oThreadData, o11yDic, traceDic, tmpCompanies, defaultLogLevel, defaultTraceLevel)
                                                                     End Sub)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ExecutePersistTasks::Error: ", ex)
            End Try

            Return bolRet

        End Function

        Public Function ExecutePersistTasksParallel(ByVal sourceThreadData As roThreadData, ByVal o11yDic As Dictionary(Of String, String), ByVal traceDic As Dictionary(Of String, String), ByVal companies As roCompanyConfiguration(), ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer) As Boolean
            Dim bolRet As Boolean = True

            Try

                Dim tasksHash As New Hashtable
                For Each oCompany As roCompanyConfiguration In companies
                    If String.IsNullOrEmpty(oCompany.dbconnectionstring) Then Continue For

                    'roTelemetryInfo.GetInstance().UpdateO11yInfo(o11yDic)

                    If Robotics.DataLayer.AccessHelper.SetThreadCompanyInformation(sourceThreadData, o11yDic, traceDic, oCompany) Then
                        ExecutePersistTasksParallelForCompany(oCompany, tasksHash)
                    Else
                        roLog.GetInstance.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roScheduleManager::ExecutePersistTasksParallel::Task delayed due to init::Company::" & oCompany.Id)
                    End If

                    Robotics.DataLayer.AccessHelper.ClearThreadCompanyInformation()
                Next

                'Envio de tareas en hash
                Azure.RoAzureSupport.SendTasksToQueueBatch(tasksHash)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ExecutePersistTasksParallel::Error: ", ex)
            End Try

            Return bolRet

        End Function

        Public Function ExecutePersistTasksParallelForCompany(ByVal company As roCompanyConfiguration, ByRef tasksHash As Hashtable)
            Dim bolRet As Boolean = True
            Try
                Dim strSQL As String = "@SELECT# Id,Action FROM sysroLiveTasks with (nolock) " &
                        "WHERE Status IN (0) AND Action IN(" &
                        "'" & roLiveTaskTypes.SecurityPermissions.ToString().ToUpper & "','" & roLiveTaskTypes.ChangeRequestPermissions.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.BroadcasterTask.ToString().ToUpper & "') " &
                        " And DATEAdd(second,30,TimeStamp) < GETDATE() ORDER BY Status DESC, ID , TimeStamp "

                Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oTaskState As New roLiveTaskState
                    For Each oRow As DataRow In tb.Rows

                        'Revisar si hay una tarea del mismo tipo en estado 1 no lanzo esta hasta que haya terminado o timeout

                        Dim tmpTask As New roLiveTask(oRow("ID"), oTaskState)
                        If tmpTask.Action.Trim <> String.Empty Then
                            tmpTask.Save(Nothing, False)
                            roLiveTask.SendMessageToTask(tmpTask, [Enum].Parse(GetType(roLiveTaskTypes), tmpTask.Action, True), oTaskState, Nothing)
                            roLog.GetInstance.logMessage(roLog.EventType.roDebug, "CEOG::Triggering Task::Action(" & oRow("Action") & ")::Company(" & company.Id & ")")
                        End If
                    Next
                End If


                strSQL = "@SELECT# DATA FROM sysroParameters WHERE ID ='RUNENGINE'"
                Dim iExecuteEngine As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL))

                If iExecuteEngine = 1 AndAlso Not roLiveTask.IsTaskTypeAlreadyRunning("RUNENGINE", -1) Then
                    DataLayer.AccessHelper.ExecuteSql("@UPDATE# sysroParameters SET DATA='0' WHERE ID='RUNENGINE'")
                    VTBase.Extensions.roConnector.InitTask(TasksType.DAILYSCHEDULE)
                End If

                Me.RestartEngineDeadTasks()
                Me.RestartEOGDeadTasks()


            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ExecutePersistTasksParallelForCompany :", ex)
            End Try

            Return bolRet
        End Function



        Private Function RestartEngineDeadTasks() As Boolean
            Dim bolret As Boolean = False

            Try
                Dim iDeadTaskRetry As Integer = roTypes.Any2Integer(roCacheManager.GetInstance.GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "Process.Engine.DeadTaskRetry"))
                Dim iDeadTaskCount As Integer = roTypes.Any2Integer(roCacheManager.GetInstance.GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "Process.Engine.DeadTaskCount"))
                If iDeadTaskRetry < 15 Then iDeadTaskRetry = 15
                If iDeadTaskCount = 0 Then iDeadTaskCount = 100

                Dim strSQL As String = "@SELECT# count(Id) FROM sysroLiveTasks with (nolock) " &
                        "WHERE Status = 1 AND Action IN ('" & roLiveTaskTypes.RunEngine.ToString().ToUpper & "','" & roLiveTaskTypes.RunEngineEmployee.ToString().ToUpper & "'," &
                         "'" & roLiveTaskTypes.UpdateEngineCache.ToString().ToUpper & "') And DATEAdd(minute," & iDeadTaskRetry.ToString & ",IsAliveAt) > GETDATE()"

                Dim iRunningTasks As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL))

                If (iDeadTaskCount - iRunningTasks) > 0 Then
                    strSQL = $"@SELECT# TOP {iDeadTaskCount - iRunningTasks} Id,Action FROM sysroLiveTasks with (nolock) " &
                        "WHERE Status IN (0,1) AND Action IN ('" & roLiveTaskTypes.RunEngine.ToString().ToUpper & "','" & roLiveTaskTypes.RunEngineEmployee.ToString().ToUpper & "'," &
                         "'" & roLiveTaskTypes.UpdateEngineCache.ToString().ToUpper & "') " &
                        " And DATEAdd(minute," & iDeadTaskRetry.ToString & ",IsAliveAt) < GETDATE() ORDER BY Status DESC, ID , TimeStamp "

                    Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                        Dim engineTaskIds As Generic.List(Of Integer) = tb.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row("Id"))).ToList()
                        AccessHelper.ExecuteSql($"@UPDATE# sysrolivetasks set IsAliveAt = GETDATE() where ID in({String.Join(",", engineTaskIds)})")

                        Dim oTasksKeys As New Hashtable
                        Dim oComanyName As String = Azure.RoAzureSupport.GetCompanyName()
                        For Each oTaskId As Integer In engineTaskIds
                            oTasksKeys.Add(oComanyName & "@" & oTaskId, oTaskId)
                        Next

                        Dim oTasksTypesHash As New Hashtable
                        oTasksTypesHash.Add(roLiveTaskTypes.RunEngineEmployee, oTasksKeys)
                        Azure.RoAzureSupport.SendTasksToQueueBatch(oTasksTypesHash)

                        roLog.GetInstance.logMessage(roLog.EventType.roDebug, "CEngine::Dead task found::Action(" & "RUNENGINEEMPLOYEE" & ")::Ids::" & String.Join(",", engineTaskIds) & " restarting")
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roEngineManager::RestartDeadTasks :", ex)
            End Try

            Return bolret
        End Function

        Private Function RestartEOGDeadTasks() As Boolean
            Dim bolret As Boolean = False

            Try
                Dim iDeadTaskRetry As Integer = roTypes.Any2Integer(roCacheManager.GetInstance.GetAdvParametersCache(Robotics.Azure.RoAzureSupport.GetCompanyName(), "Process.DeadTaskRetry"))
                Dim iDeadTaskCount As Integer = roTypes.Any2Integer(roCacheManager.GetInstance.GetAdvParametersCache(Robotics.Azure.RoAzureSupport.GetCompanyName(), "Process.DeadTaskCount"))
                If iDeadTaskRetry < 15 Then iDeadTaskRetry = 15
                If iDeadTaskCount = 0 Then iDeadTaskCount = 10

                Dim strActions As String = "('" & roLiveTaskTypes.AssignTemplate.ToString().ToUpper & "','" & roLiveTaskTypes.AssignTemplatev2.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.AssignWeekPlan.ToString().ToUpper & "','" & roLiveTaskTypes.EmployeeSecurtiyActions.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.CopyAdvancedPlan.ToString().ToUpper & "','" & roLiveTaskTypes.CopyPlan.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.MassCause.ToString().ToUpper & "','" & roLiveTaskTypes.MassCopy.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.JustifiedIncidences.ToString().ToUpper & "','" & roLiveTaskTypes.EmployeeMessage.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.MassProgrammedAbsence.ToString().ToUpper & "','" & roLiveTaskTypes.CopyAdvancedPlanv2.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.CopyAdvancedBudgetPlan.ToString().ToUpper & "','" & roLiveTaskTypes.CompleteTasksAndProjects.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.AIPlannerTask.ToString().ToUpper & "','" & roLiveTaskTypes.MassPunch.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.SecurityPermissions.ToString().ToUpper & "','" & roLiveTaskTypes.MassMarkConsents.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.GenerateRoboticsPermissions.ToString().ToUpper & "','" & roLiveTaskTypes.AssignCenters.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.RecalculatePunchDirection.ToString().ToUpper & "','" & roLiveTaskTypes.MigrateDocsToAzure.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.AddReportToDocManager.ToString().ToUpper & "','" & roLiveTaskTypes.KeepAlive.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.ConsolidateData.ToString().ToUpper & "','" & roLiveTaskTypes.ChangeRequestPermissions.ToString().ToUpper & "'," &
                        "'" & roLiveTaskTypes.ValidateSignStatusDocument.ToString().ToUpper & "','" & roLiveTaskTypes.MassLockDate.ToString().ToUpper & "')"

                Dim strSQL As String = "@SELECT# count(ID) FROM sysroLiveTasks with (nolock)  " &
                        "WHERE Status = 1 AND Action IN " & strActions &
                        " And DATEAdd(minute," & iDeadTaskRetry.ToString & ",IsAliveAt) > GETDATE()"

                If roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL)) = 0 Then
                    strSQL = $"@SELECT# TOP {iDeadTaskCount} Id,Action FROM sysroLiveTasks with (nolock) " &
                        "WHERE Status IN (0,1) AND Action IN " & strActions &
                        " And DATEAdd(minute," & iDeadTaskRetry.ToString & ",IsAliveAt) < GETDATE() ORDER BY Status DESC, ID , TimeStamp "

                    Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                        Dim eogTaskIds As Generic.List(Of Integer) = tb.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row("Id"))).ToList()
                        Dim eogTaskType As Generic.List(Of String) = tb.AsEnumerable().Select(Function(row) roTypes.Any2String(row("Action"))).ToList()
                        AccessHelper.ExecuteSql($"@UPDATE# sysrolivetasks set IsAliveAt = GETDATE() where ID in({String.Join(",", eogTaskIds)})")

                        Dim oTasksKeys As New Generic.List(Of String)
                        Dim oComanyName As String = Azure.RoAzureSupport.GetCompanyName()
                        Dim index As Integer = 0
                        For Each oTaskId As Integer In eogTaskIds
                            oTasksKeys.Add(oComanyName & "@" & oTaskId & "@" & eogTaskType(index))
                            index += 1
                        Next

                        Azure.RoAzureSupport.SendEOGTasksBatch(oTasksKeys)

                        roLog.GetInstance.logMessage(roLog.EventType.roDebug, "CEOG::Dead task found::Ids::" & String.Join(",", eogTaskIds) & " restarting")
                    End If


                End If
            Catch ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roEOGManager::RestartDeadTasks :", ex)
            End Try

            Return bolret
        End Function


#End Region

#Region "Daily task"

        Public Sub ChangeDayAction(ByVal companies As roCompanyConfiguration(), ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer, ByVal splitGroupSize As Integer)
            Try
                Dim iSplitItems As New Generic.List(Of Integer)

                Dim iPos As Integer = 0
                iSplitItems.Add(iPos)
                iPos += splitGroupSize

                While iPos < companies.Length
                    iSplitItems.Add(iPos)
                    iPos += splitGroupSize
                End While

                roTrace.GetInstance().AddTraceEvent($"Execute change day tasks for {companies.Length} companies")
                Dim o11yDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
                Dim traceDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()
                Dim oThreadData As roThreadData = roConstants.BackupThreadData()

                System.Threading.Tasks.Parallel.ForEach(iSplitItems, Sub(number)
                                                                         Dim tmpCompanies As roCompanyConfiguration() = companies.Skip(number).Take(splitGroupSize).ToArray()
                                                                         ChangeDayActionParallel(oThreadData, o11yDic, traceDic, tmpCompanies, defaultLogLevel, defaultTraceLevel)
                                                                     End Sub)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ChangeDayAction::Error: ", ex)
            End Try

        End Sub

        Public Sub ChangeDayActionParallel(ByVal sourceThreadData As roThreadData, ByVal o11yDic As Dictionary(Of String, String), ByVal traceDic As Dictionary(Of String, String), ByVal companies As roCompanyConfiguration(), ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer)
            Try

                Dim tasksHash As New Hashtable
                For Each oCompany As roCompanyConfiguration In companies
                    If String.IsNullOrEmpty(oCompany.dbconnectionstring) Then Continue For

                    If Robotics.DataLayer.AccessHelper.SetThreadCompanyInformation(sourceThreadData, o11yDic, traceDic, oCompany) Then
                        ChangeDayActionForCompany(oCompany, tasksHash)
                    Else
                        roLog.GetInstance.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roScheduleManager::ChangeDayActionParallel::Task delayed due to init::Company::" & oCompany.Id)
                    End If

                    Robotics.DataLayer.AccessHelper.ClearThreadCompanyInformation()
                Next

                'Envio de tareas en hash
                Azure.RoAzureSupport.SendTasksToQueueBatch(tasksHash)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ChangeDayActionParallel::Error: ", ex)
            End Try

        End Sub

        Public Sub ChangeDayActionForCompany(ByVal company As roCompanyConfiguration, ByRef tasksHash As Hashtable)
            Dim bCloseConnection As Boolean = False

            Try
                Dim oTaskConnection As roBaseConnection

                Dim oTmpConnection As roBaseConnection = roCacheManager.GetInstance.GetConnection()
                If oTmpConnection IsNot Nothing AndAlso oTmpConnection.GetType = GetType(roConnection) Then
                    oTaskConnection = oTmpConnection
                Else
                    oTaskConnection = roBaseConnection.ForceNewConnection(Nothing)
                    bCloseConnection = True
                End If

                'Borramos las tareas antiguas de motores y broadcaster ya que al cambio de dia se volveran a generar
                Dim sSQL As String = "@DELETE# FROM sysrolivetasks WHERE Status = 0 AND Action IN (" &
                        "'" & roLiveTaskTypes.BroadcasterTask.ToString().ToUpper & "','" & roLiveTaskTypes.RunEngine.ToString().ToUpper &
                        "','" & roLiveTaskTypes.RunEngineEmployee.ToString().ToUpper & "','" & roLiveTaskTypes.UpdateEngineCache.ToString().ToUpper &
                        "','" & roLiveTaskTypes.GenerateReportsDxTasks.ToString().ToUpper & "','" & roLiveTaskTypes.GenerateDatalinkTasks.ToString().ToUpper &
                        "','" & roLiveTaskTypes.GenerateNotifications.ToString().ToUpper & "','" & roLiveTaskTypes.GenerateAnalyticsTasks.ToString().ToUpper &
                        "','" & roLiveTaskTypes.SendNotifications.ToString().ToUpper & "')"
                DataLayer.AccessHelper.ExecuteSqlWithoutTimeOut(sSQL, oTaskConnection)

                'Borramos las tareas completadas que esten ahí como histórico
                sSQL = "@DELETE# FROM sysrolivetasks WHERE Status = 2 AND Action NOT IN ('IMPORT','EXPORT', 'ANALYTICSTASK', 'ANALYTICSTASKV2','REPORTTASKDX')"
                DataLayer.AccessHelper.ExecuteSql(sSQL, oTaskConnection)

                'Borramos las tareas que no han finalizado bien con mas de 30 dias de antigüedad
                sSQL = "@DELETE# FROM sysrolivetasks WHERE Status = 3 AND DATEDIFF(DAY,TimeStamp, GETDATE()) > 15"
                DataLayer.AccessHelper.ExecuteSqlWithoutTimeOut(sSQL, oTaskConnection)


                Dim oTaskState As New roLiveTaskState(-1)
                Dim actualDateTime As DateTime = DateTime.Now
                Dim oTaskParameters As New roCollection
                oTaskParameters.Add("xBeginDate", actualDateTime.ToString("yyyy/MM/dd HH:mm"))

                'Lanzamos recálculo de todos los empleados.
                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.RunEngine) Then
                    oTaskParameters.Add("TaskType", TasksType.DAILYSCHEDULE.ToString)
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.RunEngine, roLiveTask.CreateLiveTask(roLiveTaskTypes.RunEngine, oTaskParameters, oTaskState, False, oTaskConnection))
                End If

                ' Programación de terminales a cierta hora
                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.BroadcasterTask) Then
                    oTaskParameters.Add("AllTerminals", "true")
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.BroadcasterTask, roLiveTask.CreateLiveTask(roLiveTaskTypes.BroadcasterTask, oTaskParameters, oTaskState, False, oTaskConnection))
                End If


                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.GenerateRoboticsPermissions) Then
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.GenerateRoboticsPermissions, roLiveTask.CreateLiveTask(roLiveTaskTypes.GenerateRoboticsPermissions, oTaskParameters, oTaskState, False, oTaskConnection))
                End If

                ' Bloqueo por inactividad de cuentas de acceso mediante usuario y contraseña
                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.BlockInactivePassports) Then
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.BlockInactivePassports, roLiveTask.CreateLiveTask(roLiveTaskTypes.BlockInactivePassports, oTaskParameters, oTaskState, False, oTaskConnection))
                End If

                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.ChangeRequestPermissions) Then
                    Dim zParameters As New roCollection
                    zParameters.Add("Mode", "ALL")
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.ChangeRequestPermissions, roLiveTask.CreateLiveTask(roLiveTaskTypes.ChangeRequestPermissions, zParameters, oTaskState, False, oTaskConnection))
                End If

                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DataMonitoring) Then
                    oTaskParameters = New roCollection
                    oTaskParameters.Add("RunEvery", roTypes.Any2String(2))
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DataMonitoring, roLiveTask.CreateLiveTask(roLiveTaskTypes.DataMonitoring, oTaskParameters, oTaskState, False, oTaskConnection))
                End If

                If bCloseConnection AndAlso oTaskConnection IsNot Nothing Then oTaskConnection.CloseIfNeeded()
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ChangeDayActionForCompany::Error: ", ex)
            End Try

        End Sub

#End Region

#Region "Hour task"

        Public Sub ChangeHourAction(ByVal companies As roCompanyConfiguration(), ByVal launchHour As Integer, ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer, ByVal splitGroupSize As Integer)
            Try
                Dim iSplitItems As New Generic.List(Of Integer)

                Dim iPos As Integer = 0
                iSplitItems.Add(iPos)
                iPos += splitGroupSize

                While iPos < companies.Length
                    iSplitItems.Add(iPos)
                    iPos += splitGroupSize
                End While

                roTrace.GetInstance().AddTraceEvent($"Execute change hour tasks for {companies.Length} companies")
                Dim o11yDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
                Dim traceDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()
                Dim oThreadData As roThreadData = roConstants.BackupThreadData()

                System.Threading.Tasks.Parallel.ForEach(iSplitItems, Sub(number)
                                                                         Dim tmpCompanies As roCompanyConfiguration() = companies.Skip(number).Take(splitGroupSize).ToArray()
                                                                         ChangeHourActionParallel(oThreadData, o11yDic, traceDic, tmpCompanies, launchHour, defaultLogLevel, defaultTraceLevel)
                                                                     End Sub)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ChangeHourAction::Error: ", ex)
            End Try

        End Sub

        Public Sub ChangeHourActionParallel(ByVal sourceThreadData As roThreadData, ByVal o11yDic As Dictionary(Of String, String), ByVal traceDic As Dictionary(Of String, String), ByVal companies As roCompanyConfiguration(), ByVal launchHour As Integer, ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer)
            Try

                Dim tasksHash As New Hashtable
                For Each oCompany As roCompanyConfiguration In companies
                    If String.IsNullOrEmpty(oCompany.dbconnectionstring) Then Continue For

                    If Robotics.DataLayer.AccessHelper.SetThreadCompanyInformation(sourceThreadData, o11yDic, traceDic, oCompany) Then
                        ChangeHourActionForCompany(oCompany, tasksHash, launchHour)
                    Else
                        roLog.GetInstance.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roScheduleManager::ChangeHourActionParallel::Task delayed due to init::Company::" & oCompany.Id)
                    End If

                    Robotics.DataLayer.AccessHelper.ClearThreadCompanyInformation()
                Next

                'Envio de tareas en hash
                Azure.RoAzureSupport.SendTasksToQueueBatch(tasksHash)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ChangeHourActionParallel::Error: ", ex)
            End Try

        End Sub

        Public Sub ChangeHourActionForCompany(ByVal company As roCompanyConfiguration, ByRef tasksHash As Hashtable, ByVal launchHour As Integer)
            Dim bCloseConnection As Boolean = False

            Try
                Dim oTaskConnection As roBaseConnection

                Dim oTmpConnection As roBaseConnection = roCacheManager.GetInstance.GetConnection()
                If oTmpConnection IsNot Nothing AndAlso oTmpConnection.GetType = GetType(roConnection) Then
                    oTaskConnection = oTmpConnection
                Else
                    bCloseConnection = True
                    oTaskConnection = roBaseConnection.ForceNewConnection(Nothing)
                End If

                Select Case launchHour
                    Case 1
                        tasksHash = GetCleanOldDataActions(company, tasksHash, oTaskConnection)
                    Case 2
                        tasksHash = GetNotificationsActions(company, tasksHash, oTaskConnection)
                        'Reseteamos la cache de la compañia cada día a las 02:00 AM para forzar sincronización de datos
                        DataLayer.roCacheManager.GetInstance().UpdateInitCache(oTaskConnection)
                    Case 3
                        tasksHash = GetDocumentsActions(company, tasksHash, oTaskConnection)
                    Case Else
                        tasksHash = GetDefaultChangeHourActions(company, tasksHash, oTaskConnection)
                End Select


                If bCloseConnection AndAlso oTaskConnection IsNot Nothing Then oTaskConnection.CloseIfNeeded()
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ChangeHourActionForCompany::Error: ", ex)
            End Try

        End Sub

        Private Function GetDocumentsActions(company As roCompanyConfiguration, tasksHash As Hashtable, oTaskConnection As roBaseConnection) As Hashtable
            Dim actualDateTime As DateTime = DateTime.Now
            Dim oTaskParameters As New roCollection
            oTaskParameters.Add("xBeginDate", actualDateTime.ToString("yyyy/MM/dd HH:mm"))


            Dim oTaskState As New roLiveTaskState(-1)

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DeleteOldDocuments) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DeleteOldDocuments, roLiveTask.CreateLiveTask(roLiveTaskTypes.DeleteOldDocuments, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.ValidityDocuments) Then
                ' Gestor de documentos
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.ValidityDocuments, roLiveTask.CreateLiveTask(roLiveTaskTypes.ValidityDocuments, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DocumentTracking) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DocumentTracking, roLiveTask.CreateLiveTask(roLiveTaskTypes.DocumentTracking, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            ' Control de retención Canal de Denuncias
            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DeleteOldComplaints) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DeleteOldComplaints, roLiveTask.CreateLiveTask(roLiveTaskTypes.DeleteOldComplaints, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            Return tasksHash
        End Function

        Private Function GetNotificationsActions(company As roCompanyConfiguration, tasksHash As Hashtable, oTaskConnection As roBaseConnection) As Hashtable
            Dim actualDateTime As DateTime = DateTime.Now
            Dim oTaskParameters As New roCollection
            oTaskParameters.Add("xBeginDate", actualDateTime.ToString("yyyy/MM/dd HH:mm"))

            Dim oTaskState As New roLiveTaskState(-1)
            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.CheckCloseDate) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.CheckCloseDate, roLiveTask.CreateLiveTask(roLiveTaskTypes.CheckCloseDate, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.PurgeNotifications) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.PurgeNotifications, roLiveTask.CreateLiveTask(roLiveTaskTypes.PurgeNotifications, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.CheckScheduleRulesFaults) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.CheckScheduleRulesFaults, roLiveTask.CreateLiveTask(roLiveTaskTypes.CheckScheduleRulesFaults, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.ManageVisits) Then
                Dim oVisitParameters As New roCollection
                oVisitParameters.Add("xBeginDate", actualDateTime.ToString("yyyy/MM/dd HH:mm"))
                oVisitParameters.Add("xChangeHour", False)
                oVisitParameters.Add("xChangeDay", True)

                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.ManageVisits, roLiveTask.CreateLiveTask(roLiveTaskTypes.ManageVisits, oVisitParameters, oTaskState, False, oTaskConnection))
            End If

            Return tasksHash
        End Function

        Private Function GetCleanOldDataActions(company As roCompanyConfiguration, tasksHash As Hashtable, oTaskConnection As roBaseConnection) As Hashtable
            Dim actualDateTime As DateTime = DateTime.Now
            Dim oTaskParameters As New roCollection
            oTaskParameters.Add("xBeginDate", actualDateTime.ToString("yyyy/MM/dd HH:mm"))

            Dim oTaskState As New roLiveTaskState(-1)
            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DeleteOldPhotos) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DeleteOldPhotos, roLiveTask.CreateLiveTask(roLiveTaskTypes.DeleteOldPhotos, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DeleteOldPunches) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DeleteOldPunches, roLiveTask.CreateLiveTask(roLiveTaskTypes.DeleteOldPunches, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DeleteOldBiometricData) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DeleteOldBiometricData, roLiveTask.CreateLiveTask(roLiveTaskTypes.DeleteOldBiometricData, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DeleteAccessMovesHistory) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DeleteAccessMovesHistory, roLiveTask.CreateLiveTask(roLiveTaskTypes.DeleteAccessMovesHistory, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.RemoveExpiredTasks) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.RemoveExpiredTasks, roLiveTask.CreateLiveTask(roLiveTaskTypes.RemoveExpiredTasks, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            Return tasksHash
        End Function

        Private Function GetDefaultChangeHourActions(company As roCompanyConfiguration, tasksHash As Hashtable, oTaskConnection As roBaseConnection) As Hashtable
            Dim actualDateTime As DateTime = DateTime.Now

            Dim oTaskParameters As New roCollection
            oTaskParameters.Add("xBeginDate", actualDateTime.ToString("yyyy/MM/dd HH:mm"))
            Dim oVisitParameters As New roCollection
            oVisitParameters.Add("xBeginDate", actualDateTime.ToString("yyyy/MM/dd HH:mm"))
            oVisitParameters.Add("xChangeHour", True)
            oVisitParameters.Add("xChangeDay", False)

            Dim oTaskState As New roLiveTaskState(-1)
            'Creamos tarea para que compruebe solicitudes automáticas pendientes de aceptar.
            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.CheckAutomaticRequests) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.CheckAutomaticRequests, roLiveTask.CreateEmptyLiveTask(roLiveTaskTypes.CheckAutomaticRequests, New roLiveTaskState(-1), False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.ManageVisits) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.ManageVisits, roLiveTask.CreateLiveTask(roLiveTaskTypes.ManageVisits, oVisitParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.SynchronizeTerminals) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.SynchronizeTerminals, roLiveTask.CreateLiveTask(roLiveTaskTypes.SynchronizeTerminals, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DeleteOldAudit) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DeleteOldAudit, roLiveTask.CreateLiveTask(roLiveTaskTypes.DeleteOldAudit, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.RunEngine) Then
                'Lanzamos recálculo de todos los empleados solo en el caso que tengo los motores v2
                oTaskParameters.Add("TaskType", TasksType.DAILYSCHEDULE.ToString)
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.RunEngine, roLiveTask.CreateLiveTask(roLiveTaskTypes.RunEngine, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            oTaskParameters = New roCollection
            oTaskParameters.Add("RunEvery", roTypes.Any2String(1))

            If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.DataMonitoring) Then
                AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.DataMonitoring, roLiveTask.CreateLiveTask(roLiveTaskTypes.DataMonitoring, oTaskParameters, oTaskState, False, oTaskConnection))
            End If

            Return tasksHash
        End Function

#End Region

#Region "Programmed tasks"

        Public Function ExecuteProgrammedTasks(ByVal companies As roCompanyConfiguration(), ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer, ByVal splitGroupSize As Integer) As Boolean
            Dim bolRet As Boolean = True

            Try
                'Ejecutamos en paralelo la generación de tareas para evitar problemas
                Dim iSplitItems As New Generic.List(Of Integer)

                Dim iPos As Integer = 0
                iSplitItems.Add(iPos)
                iPos += splitGroupSize

                While iPos < companies.Length
                    iSplitItems.Add(iPos)
                    iPos += splitGroupSize
                End While

                roTrace.GetInstance().AddTraceEvent($"Execute programmed tasks for {companies.Length} companies")
                Dim o11yDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
                Dim traceDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()
                Dim oThreadData As roThreadData = roConstants.BackupThreadData()

                System.Threading.Tasks.Parallel.ForEach(iSplitItems, Sub(number)
                                                                         Dim tmpCompanies As roCompanyConfiguration() = companies.Skip(number).Take(splitGroupSize).ToArray()
                                                                         ExecuteProgrammedTasksParallel(oThreadData, o11yDic, traceDic, tmpCompanies, defaultLogLevel, defaultTraceLevel)
                                                                     End Sub)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ExecuteProgrammedTasks::Error: ", ex)
            End Try

            Return bolRet

        End Function

        Public Function ExecuteProgrammedTasksParallel(ByVal sourceThreadData As roThreadData, ByVal o11yDic As Dictionary(Of String, String), ByVal traceDic As Dictionary(Of String, String), ByVal companies As roCompanyConfiguration(), ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer) As Boolean
            Dim bolRet As Boolean = True

            Try


                Dim tasksHash As New Hashtable
                For Each oCompany As roCompanyConfiguration In companies
                    If String.IsNullOrEmpty(oCompany.dbconnectionstring) Then Continue For

                    If Robotics.DataLayer.AccessHelper.SetThreadCompanyInformation(sourceThreadData, o11yDic, traceDic, oCompany) Then
                        ExecuteProgrammedTasksForCompany(oCompany, tasksHash)
                    Else
                        roLog.GetInstance.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roScheduleManager::ExecuteProgrammedTasksParallel::Task delayed due to init::Company::" & oCompany.Id)
                    End If

                    Robotics.DataLayer.AccessHelper.ClearThreadCompanyInformation()
                Next

                'Envio de tareas en hash
                Azure.RoAzureSupport.SendTasksToQueueBatch(tasksHash)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ExecuteProgrammedTasksParallel::Error: ", ex)
            End Try

            Return bolRet

        End Function

        Public Function ExecuteProgrammedTasksForCompany(ByVal company As roCompanyConfiguration, ByRef tasksHash As Hashtable)
            Dim bolRet As Boolean = True
            Dim bCloseConnection As Boolean = False
            Try

                Dim oTaskConnection As roBaseConnection

                Dim oTmpConnection As roBaseConnection = roCacheManager.GetInstance.GetConnection()
                If oTmpConnection IsNot Nothing AndAlso oTmpConnection.GetType = GetType(roConnection) Then
                    oTaskConnection = oTmpConnection
                Else
                    bCloseConnection = True
                    oTaskConnection = roBaseConnection.ForceNewConnection(Nothing)
                End If

                'Ejectua el lanzador de tareas de enlaces planificados
                Dim oTaskState As New roLiveTaskState(-1)

                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.GenerateDatalinkTasks) Then
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.GenerateDatalinkTasks, roLiveTask.CreateEmptyLiveTask(roLiveTaskTypes.GenerateDatalinkTasks, oTaskState, False, oTaskConnection))
                End If

                'Ejecuta el lanzador de analiticas planificadas
                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.GenerateAnalyticsTasks) Then
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.GenerateAnalyticsTasks, roLiveTask.CreateEmptyLiveTask(roLiveTaskTypes.GenerateAnalyticsTasks, oTaskState, False, oTaskConnection))
                End If

                'Ejectua el lanzador de tareas de informes planificados
                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.GenerateReportsDxTasks) Then
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.GenerateReportsDxTasks, roLiveTask.CreateEmptyLiveTask(roLiveTaskTypes.GenerateReportsDxTasks, oTaskState, False, oTaskConnection))
                End If

                'Creamos tarea para que se envien las notificaciones pendientes a cliente.
                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.GenerateNotifications) Then
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.GenerateNotifications, roLiveTask.CreateEmptyLiveTask(roLiveTaskTypes.GenerateNotifications, oTaskState, False, oTaskConnection))
                End If

                'Creamos tarea para que se envien las notificaciones pendientes a cliente.
                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.SendNotifications) Then
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.SendNotifications, roLiveTask.CreateEmptyLiveTask(roLiveTaskTypes.SendNotifications, oTaskState, False, oTaskConnection))
                End If

                Dim sourceFile As String = roTypes.Any2String(DataLayer.roCacheManager.GetInstance().GetParametersCache(company.Id, Parameters.ConnectorSourceName))
                Dim destFile As String = roTypes.Any2String(DataLayer.roCacheManager.GetInstance().GetParametersCache(company.Id, Parameters.ConnectorReadingsName))

                'Creamos tarea para que inicie el conector de fichajes si esta configurado.
                If Not String.IsNullOrEmpty(sourceFile) AndAlso Not String.IsNullOrEmpty(destFile) AndAlso Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.PunchConnectorTask) Then
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.PunchConnectorTask, roLiveTask.CreateEmptyLiveTask(roLiveTaskTypes.PunchConnectorTask, oTaskState, False, oTaskConnection))
                End If

                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.CheckInvalidEntries) Then
                    Dim oTaskParameters As New roCollection
                    oTaskParameters.Add("xBeginDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm"))
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.CheckInvalidEntries, roLiveTask.CreateLiveTask(roLiveTaskTypes.CheckInvalidEntries, oTaskParameters, oTaskState, False, oTaskConnection))
                End If

                If Not ExistsTaskForCompany(company, tasksHash, roLiveTaskTypes.CheckConcurrenceData) Then
                    AddLiveTasktoTable(company, tasksHash, roLiveTaskTypes.CheckConcurrenceData, roLiveTask.CreateEmptyLiveTask(roLiveTaskTypes.CheckConcurrenceData, oTaskState, False, oTaskConnection))
                End If

                If bCloseConnection AndAlso oTaskConnection IsNot Nothing Then oTaskConnection.CloseIfNeeded()
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::ExecuteProgrammedTasksForCompany :", ex)
            End Try

            Return bolRet
        End Function

#End Region

#Region "Update db tasks"

        Public Sub UpdateDBTasks(ByVal companies As roCompanyConfiguration(), ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer, ByVal splitGroupSize As Integer)
            Try
                Dim iSplitItems As New Generic.List(Of Integer)

                Dim iPos As Integer = 0
                iSplitItems.Add(iPos)
                iPos += splitGroupSize

                While iPos < companies.Length
                    iSplitItems.Add(iPos)
                    iPos += splitGroupSize
                End While

                roTrace.GetInstance().AddTraceEvent($"Updating DB for {companies.Length} companies")
                Dim o11yDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
                Dim traceDic As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()
                Dim oThreadData As roThreadData = roConstants.BackupThreadData()

                System.Threading.Tasks.Parallel.ForEach(iSplitItems, Sub(number)
                                                                         Dim tmpCompanies As roCompanyConfiguration() = companies.Skip(number).Take(splitGroupSize).ToArray()
                                                                         UpdateDBTasksParallel(oThreadData, o11yDic, traceDic, tmpCompanies, defaultLogLevel, defaultTraceLevel)
                                                                     End Sub)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::UpdateDBTasks::Error: ", ex)
            End Try

        End Sub

        Public Sub UpdateDBTasksParallel(ByVal sourceThreadData As roThreadData, ByVal o11yDic As Dictionary(Of String, String), ByVal traceDic As Dictionary(Of String, String), ByVal companies As roCompanyConfiguration(), ByVal defaultLogLevel As Integer, ByVal defaultTraceLevel As Integer)
            Try

                Dim tasksHash As New Hashtable
                For Each oCompany As roCompanyConfiguration In companies
                    If String.IsNullOrEmpty(oCompany.dbconnectionstring) Then Continue For

                    If Robotics.DataLayer.AccessHelper.SetThreadCompanyInformation(sourceThreadData, o11yDic, traceDic, oCompany) Then
                        DataLayer.AccessHelper.UpgradeClientDB()
                    Else
                        roLog.GetInstance.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roScheduleManager::UpdateDBTasksParallel::Task delayed due to init::Company::" & oCompany.Id)
                    End If
                    Robotics.DataLayer.AccessHelper.ClearThreadCompanyInformation()
                Next

                'Envio de tareas en hash
                Azure.RoAzureSupport.SendTasksToQueueBatch(tasksHash)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roScheduleManager::UpdateDBTasksParallel::Error: ", ex)
            End Try

        End Sub

#End Region

#End Region

    End Class

End Namespace