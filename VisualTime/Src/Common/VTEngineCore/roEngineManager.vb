Imports System.Runtime.Serialization
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTShiftEngines
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTEngineManager

    <DataContract>
    Public Class roEngineManager

#Region "Methods"

        Public Shared Function ExecuteTask(ByVal oTask As roLiveTask) As Boolean
            Dim bRet As Boolean = False

            ' Ejecutamos tarea indicada
            Try

                Select Case UCase(oTask.Action)
                    Case roLiveTaskTypes.RunEngine.ToString().ToUpper
                        bRet = roEngineManager.RunEngine(oTask)
                    Case roLiveTaskTypes.RunEngineEmployee.ToString().ToUpper
                        bRet = roEngineManager.RunEngineEmployee(oTask)
                    Case roLiveTaskTypes.UpdateEngineCache.ToString().ToUpper
                        bRet = roEngineManager.UpdateEngineCache(oTask)
                End Select
            Catch Ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roEngineManager::ExecuteTask :", Ex)
            End Try

            Return bRet
        End Function

#End Region

        Public Shared Function RunEngine(ByVal oTask As roLiveTask) As Boolean
            Dim bolRet As Boolean = True
            Dim iIDEmployee As Integer = 0
            Dim strWhere As String = ""

            Try
                If roLiveTask.IsTaskTypeAlreadyRunning("RUNENGINE", oTask.ID) Then
                    roTrace.GetInstance().AddTraceEvent("Engine is already running. Task will be ignored.")
                    DataLayer.AccessHelper.ExecuteSql("@UPDATE# sysroParameters SET DATA='1' WHERE ID='RUNENGINE'")
                    Return True
                End If


                If oTask.Parameters.Exists("User.ID") Then
                    iIDEmployee = roTypes.Any2Integer(oTask.Parameters("User.ID"))
                    strWhere = " AND DailySchedule.IDEmployee=" & iIDEmployee.ToString
                End If

                Dim strSQL As String = "@SELECT# DISTINCT IDEmployee FROM DailySchedule with (nolock) INNER JOIN Employees with (nolock) ON DailySchedule.IDEmployee=Employees.ID WHERE (Status < 70 or (Employees.Type='J' AND TaskStatus < 80))  and Date <= GETDATE() " & strWhere & "  Order by IDEmployee"

                Dim dtEmployees As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)

                If dtEmployees IsNot Nothing AndAlso dtEmployees.Rows.Count > 0 Then
                    Dim runningEmployees As List(Of Integer)

                    Try
                        Dim sAlreadyCreated As String = $"@Select# 
                                            CAST(Parameters As XML).value('(/roCollection/Item[@key=(""IDEmployee"")]/text())[1]', 'int') as IDEmployee
                                        FROM 
                                            sysroLiveTasks with (nolock)
                                        WHERE Action =  'RUNENGINEEMPLOYEE' AND Status = 0"

                        Dim dtAlreadyCreated As DataTable = DataLayer.AccessHelper.CreateDataTable(sAlreadyCreated)
                        runningEmployees = dtAlreadyCreated.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row("IDEmployee"))).ToList()
                    Catch ex As Exception
                        runningEmployees = New List(Of Integer)
                    End Try


                    Dim oTasksTypesHash As New Hashtable
                    Dim oTasksKeys As New Hashtable

                    Dim oComanyName As String = Azure.RoAzureSupport.GetCompanyName()
                    Dim idTask As Integer = 0
                    For Each oScheduleRow As DataRow In dtEmployees.Rows

                        If Not runningEmployees.Contains(roTypes.Any2Integer(oScheduleRow("IDEmployee"))) Then
                            Dim oParams As New roCollection
                            oParams.Add("TaskType", oTask.Parameters("TaskType"))
                            oParams.Add("IDEmployee", roTypes.Any2Integer(oScheduleRow("IDEmployee")))
                            idTask = roLiveTask.CreateLiveTask(roLiveTaskTypes.RunEngineEmployee, oParams, New roLiveTaskState(), False)
                            If idTask <> -1 Then oTasksKeys.Add(oComanyName & "@" & idTask, idTask)
                        End If
                    Next

                    If oTasksKeys.Keys.Count > 0 Then
                        oTasksTypesHash.Add(roLiveTaskTypes.RunEngineEmployee, oTasksKeys)
                        Azure.RoAzureSupport.SendTasksToQueueBatch(oTasksTypesHash, 500)
                    End If

                End If

            Catch ex As Exception
                bolRet = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::RunEngine :", ex)
            End Try

            Return bolRet
        End Function

        Public Shared Function RunEngineEmployee(ByVal oTask As roLiveTask) As Boolean
            Dim bolRet As Boolean = True

            Dim sProcess As String = String.Empty
            Dim iIDEmployee As Integer = 0

            Dim oEngineState As New roEngineState
            oEngineState.Result = EngineResultEnum.NoError

            Dim mCurrentDate As Date = Date.MinValue

            Try

                iIDEmployee = roTypes.Any2Integer(oTask.Parameters("IDEmployee"))
                roTrace.GetInstance.AddTraceEvent($"Engine for emp: {iIDEmployee}")

                Dim threadIdentifier As String = VTBase.roConstants.GetManagedThreadGUID()
                Dim bolGUIDChanged As Boolean = False

                Dim tb As New DataTable("DailySchedule")
                Dim strSQL As String = "@SELECT#  Date  FROM  DailySchedule DS with (nolock)  INNER JOIN Employees  ON DS.IDEmployee=Employees.ID  WHERE IDEmployee = " & iIDEmployee & " AND (Status < 70 or (Employees.Type='J' AND TaskStatus < 80)) and Date <= GETDATE() "
                tb = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each arow As DataRow In tb.Rows
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET [GUID] = '" & threadIdentifier & "' WHERE IDEmployee = " & iIDEmployee.ToString & " AND Date=" & roTypes.Any2Time(arow("Date")).SQLSmallDateTime
                        bolRet = DataLayer.AccessHelper.ExecuteSql(strSQL)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet AndAlso Not bolGUIDChanged Then bolRet = ProcessDetector(sProcess, oEngineState, mCurrentDate, oTask, bolGUIDChanged)

                If bolRet AndAlso Not bolGUIDChanged Then bolRet = ProcessTaskAccruals(sProcess, oTask, bolGUIDChanged)

                If bolRet AndAlso Not bolGUIDChanged Then bolRet = ProcessIncidences(sProcess, oEngineState, mCurrentDate, oTask, bolGUIDChanged)

                If bolRet AndAlso Not bolGUIDChanged Then bolRet = ProcessCauses(sProcess, oEngineState, mCurrentDate, oTask, bolGUIDChanged)

                If bolRet AndAlso Not bolGUIDChanged Then bolRet = ProcessAccruals(sProcess, oEngineState, mCurrentDate, oTask, bolGUIDChanged)

                If bolRet Then
                    strSQL = "@SELECT# Date FROM DailySchedule with (nolock) WHERE [GUID] = '" & threadIdentifier & "' AND IDEmployee = " & iIDEmployee.ToString
                    tb = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        For Each arow As DataRow In tb.Rows
                            strSQL = "@UPDATE# DailySchedule  WITH (ROWLOCK) SET [GUID] = NULL WHERE IDEmployee = " & iIDEmployee.ToString & " AND Date=" & roTypes.Any2Time(arow("Date")).SQLSmallDateTime & "  AND [GUID] = '" & threadIdentifier & "'"
                            bolRet = DataLayer.AccessHelper.ExecuteSql(strSQL)
                            If Not bolRet Then Exit For
                        Next
                    End If
                End If

                If Not bolRet Then
                    ' Esto solo se puede dar si se ha generao un error o
                    ' si recibimos una tarea sin idempleado o la tarea ya se ha procesado en otro hilo. con lo cual no tenemos que lanzar el recalculo en este caso
                    If oEngineState.Result <> EngineResultEnum.NoError AndAlso oEngineState.Result <> EngineResultEnum.EmployeeRequired Then
                        ExecuteSql("@UPDATE# sysroParameters SET DATA='1' WHERE ID='RUNENGINE'")
                    End If
                    roTrace.GetInstance().AddTraceEvent("Error processing " & sProcess & " on Employee " & iIDEmployee.ToString & " on date " & mCurrentDate.ToShortDateString & ". Detail -> " & oEngineState.Result.ToString)
                End If
            Catch ex As Exception
                ' Esto solo se puede dar si se ha generao un error o
                ' si recibimos una tarea sin idempleado o la tarea ya se ha procesado en otro hilo. con lo cual no tenemos que lanzar el recalculo en este caso
                If oEngineState.Result <> EngineResultEnum.NoError AndAlso oEngineState.Result <> EngineResultEnum.EmployeeRequired Then
                    ExecuteSql("@UPDATE# sysroParameters SET DATA='1' WHERE ID='RUNENGINE'")
                End If
                roLog.GetInstance.logMessage(roLog.EventType.roError, "roEngineManager::RunEngineEmployee::Resut detail -> " & oEngineState.Result.ToString & ". Exception: " & ex.Message)
            End Try

            Return bolRet
        End Function

        Private Shared Function ProcessAccruals(ByRef sProcess As String, ByRef oEngineState As roEngineState, ByRef mCurrentDate As Date, oTask As roLiveTask, ByRef bolGUIDChanged As Boolean) As Boolean
            Dim bolRet As Boolean

            Dim oAccrualsManager As New VTShiftEngines.roAccrualsManager(oTask.ID)
            sProcess = "Accruals"
            oAccrualsManager.ExecuteBatch(bolGUIDChanged)
            bolRet = (oAccrualsManager.State.Result = EngineResultEnum.NoError)
            oEngineState.Result = oAccrualsManager.State.Result
            mCurrentDate = oAccrualsManager.mCurrentDate

            Return bolRet
        End Function

        Private Shared Function ProcessCauses(ByRef sProcess As String, ByRef oEngineState As roEngineState, ByRef mCurrentDate As Date, oTask As roLiveTask, ByRef bolGUIDChanged As Boolean) As Boolean
            Dim bolRet As Boolean

            Dim oCausesManager As New VTShiftEngines.roCausesManager(oTask.ID)
            sProcess = "Causes"
            oCausesManager.ExecuteBatch(bolGUIDChanged)
            bolRet = (oCausesManager.State.Result = EngineResultEnum.NoError)
            oEngineState.Result = oCausesManager.State.Result
            mCurrentDate = oCausesManager.mCurrentDate

            Return bolRet
        End Function

        Private Shared Function ProcessIncidences(ByRef sProcess As String, ByRef oEngineState As roEngineState, ByRef mCurrentDate As Date, oTask As roLiveTask, ByRef bolGUIDChanged As Boolean) As Boolean
            Dim bolRet As Boolean

            Dim oIncidencesManager As New VTShiftEngines.roIncidencesManager(oTask.ID)
            sProcess = "Incidences"
            oIncidencesManager.ExecuteBatch(bolGUIDChanged)
            bolRet = (oIncidencesManager.State.Result = EngineResultEnum.NoError)
            oEngineState.Result = oIncidencesManager.State.Result
            mCurrentDate = oIncidencesManager.mCurrentDate

            Return bolRet
        End Function

        Private Shared Function ProcessTaskAccruals(ByRef sProcess As String, oTask As roLiveTask, ByRef bolGUIDChanged As Boolean)
            Dim oTaskAccrualsManager As New VTShiftEngines.roTaskAccrualsManager(oTask.ID)
            sProcess = "TaskAccruals"
            oTaskAccrualsManager.ExecuteBatch(bolGUIDChanged)
            Return (oTaskAccrualsManager.State.Result = EngineResultEnum.NoError)
        End Function

        Private Shared Function ProcessDetector(ByRef sProcess As String, ByRef oEngineState As roEngineState, ByRef mCurrentDate As Date, oTask As roLiveTask, ByRef bolGUIDChanged As Boolean)
            Dim bRet As Boolean

            Dim oDetectorManager As New VTShiftEngines.roDetectorManager(oTask.ID)
            sProcess = "Detector"
            oDetectorManager.ExecuteBatch(bolGUIDChanged)
            bRet = (oDetectorManager.State.Result = EngineResultEnum.NoError)
            oEngineState.Result = oDetectorManager.State.Result
            mCurrentDate = oDetectorManager.mCurrentDate

            Return bRet
        End Function

        Public Shared Function UpdateEngineCache(ByVal oTask As roLiveTask) As Boolean
            Dim bolRet As Boolean = True

            Try

                If oTask.Status = 0 Then
                    oTask.ExecutionDate = Now
                End If
                oTask.Status = 1
                oTask.Save()

                Dim oTaskType As TasksType = [Enum].Parse(GetType(TasksType), roTypes.Any2String(oTask.Parameters("TaskType")), True)
                Dim iObjectID As Integer = roTypes.Any2Integer(oTask.Parameters("ObjectID"))
                Dim oTaskAction As CacheAction = [Enum].Parse(GetType(CacheAction), roTypes.Any2String(oTask.Parameters("Action")), True)
                Dim oLastCacheUpdate As DateTime = DateTime.Now

                Select Case oTaskType
                    Case TasksType.CONCEPTS
                        Dim oParam As New VTBusiness.Common.AdvancedParameter.roAdvancedParameter("Engine.Concepts.LastCacheUpdate", New VTBusiness.Common.AdvancedParameter.roAdvancedParameterState())
                        oParam.Value = oLastCacheUpdate.ToString("yyyy-MM-dd HH:mm:ss")
                        oParam.Save()

                        Select Case oTaskAction
                            Case CacheAction.InsertOrUpdate
                                Dim oConceptManager As New VTConcepts.Concepts.roConceptManager()
                                Dim oConcept As roConceptEngine = oConceptManager.Load(iObjectID)
                                If oConcept IsNot Nothing Then DataLayer.roCacheManager.GetInstance().UpdateConceptCache(Azure.RoAzureSupport.GetCompanyName(), oConcept)
                            Case CacheAction.Delete
                                DataLayer.roCacheManager.GetInstance().RemoveConceptFromCache(Azure.RoAzureSupport.GetCompanyName(), iObjectID)
                        End Select

                        DataLayer.roCacheManager.GetInstance().UpdateEngineConceptsCacheLastUpdate(oLastCacheUpdate)
                    Case TasksType.CAUSES
                        Dim oParam As New VTBusiness.Common.AdvancedParameter.roAdvancedParameter("Engine.Causes.LastCacheUpdate", New VTBusiness.Common.AdvancedParameter.roAdvancedParameterState())
                        oParam.Value = oLastCacheUpdate.ToString("yyyy-MM-dd HH:mm:ss")
                        oParam.Save()

                        Select Case oTaskAction
                            Case CacheAction.InsertOrUpdate
                                Dim oCauseManager As New VTCauses.Causes.roCauseManager()
                                Dim oCause As roCauseEngine = oCauseManager.Load(iObjectID)
                                If oCause IsNot Nothing Then DataLayer.roCacheManager.GetInstance().UpdateCauseCache(Azure.RoAzureSupport.GetCompanyName(), oCause)

                            Case CacheAction.Delete
                                DataLayer.roCacheManager.GetInstance().RemoveCauseFromCache(Azure.RoAzureSupport.GetCompanyName(), iObjectID)
                        End Select

                        DataLayer.roCacheManager.GetInstance().UpdateEngineCausesCacheLastUpdate(oLastCacheUpdate)
                    Case TasksType.SHIFTS
                        Dim oParam As New VTBusiness.Common.AdvancedParameter.roAdvancedParameter("Engine.Shifts.LastCacheUpdate", New VTBusiness.Common.AdvancedParameter.roAdvancedParameterState())
                        oParam.Value = oLastCacheUpdate.ToString("yyyy-MM-dd HH:mm:ss")
                        oParam.Save()

                        Select Case oTaskAction
                            Case CacheAction.InsertOrUpdate
                                Dim oShiftManager As New VTShifts.Shifts.roShiftManager()
                                Dim oShift As roShiftEngine = oShiftManager.Load(iObjectID)
                                If oShift IsNot Nothing Then DataLayer.roCacheManager.GetInstance().UpdateShiftCache(Azure.RoAzureSupport.GetCompanyName(), oShift)

                            Case CacheAction.Delete
                                DataLayer.roCacheManager.GetInstance().RemoveShiftFromCache(Azure.RoAzureSupport.GetCompanyName(), iObjectID)
                        End Select

                        DataLayer.roCacheManager.GetInstance().UpdateEngineShiftsCacheLastUpdate(oLastCacheUpdate)
                    Case TasksType.LABAGREES
                        Dim oParam As New VTBusiness.Common.AdvancedParameter.roAdvancedParameter("Engine.LabAgrees.LastCacheUpdate", New VTBusiness.Common.AdvancedParameter.roAdvancedParameterState()) With {
                                .Value = oLastCacheUpdate.ToString("yyyy-MM-dd HH:mm:ss")
                            }
                        oParam.Save()

                        Select Case oTaskAction
                            Case CacheAction.InsertOrUpdate
                                Dim oLabAgreeManager As New VTLabAgrees.LabAgree.roLabAgreeManager()
                                Dim oLabAgree As roLabAgreeEngine = oLabAgreeManager.Load(iObjectID)
                                If oLabAgree IsNot Nothing Then DataLayer.roCacheManager.GetInstance().UpdateLabAgreeCache(Azure.RoAzureSupport.GetCompanyName(), oLabAgree)

                            Case CacheAction.Delete
                                DataLayer.roCacheManager.GetInstance().RemoveLabAgreeFromCache(Azure.RoAzureSupport.GetCompanyName(), iObjectID)
                        End Select

                        DataLayer.roCacheManager.GetInstance().UpdateEngineLabAgreesCacheLastUpdate(oLastCacheUpdate)
                End Select

                roConnector.InitTask(TasksType.DAILYSCHEDULE)

                oTask.Delete()
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roEogManager::UpdateEngineCache :", ex)
            End Try

            Return bolRet
        End Function
    End Class

End Namespace