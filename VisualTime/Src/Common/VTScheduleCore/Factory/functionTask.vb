
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTScheduleManager


    Public MustInherit Class BaseTask
        Implements IDisposable

        Private disposedValue As Boolean
        Private oTask As roLiveTask
        Private oTaskState As roLiveTaskState

        Protected MustOverride Function GetProcessTaskTypes() As Generic.List(Of roLiveTaskTypes)

        Protected MustOverride Function ExecuteTaskFromManager(ByVal oTask As roLiveTask) As BaseTaskResult

        Protected MustOverride Function NeedToKeepTask(ByVal oTaskType As roLiveTaskTypes) As Boolean

        Protected Overridable Sub RunExtraCommand(ByVal oRow As DataRow)

        End Sub

        Public Function ExecuteTask(idTask As Integer, Optional action As String = "") As Boolean

            If idTask = 0 AndAlso action.ToUpper = "RESETCACHE" Then

                roTrace.GetInstance().AddTraceInfo(idTask.ToString(), action, RoAzureSupport.GetCompanyName())
                roTrace.GetInstance().TraceMessage(roTrace.TraceType.roDebug, roTrace.TraceResult.Init, "Task")

                'Recibimos un reseteo de la cache global por parte de HAmanager
                DataLayer.roCacheManager.GetInstance.UpdateInitCache()

                roTrace.GetInstance().TraceMessage(roTrace.TraceType.roInfo, roTrace.TraceResult.Ok, "")
                Return True
            Else
                Return ProcessTask(idTask)
            End If


        End Function

        Private Function ProcessTask(idTask As Integer) As Boolean
            Dim bRet As Boolean = False
            Dim actionResult As roTrace.TraceResult = roTrace.TraceResult.Ok
            Dim errorMsg As String = String.Empty

            ' Ejecutamos tarea indicada
            Try
                Dim taskFilters As String = String.Join(",", GetProcessTaskTypes().Select(Function(task) $"'{task.ToString().ToUpper()}'"))

                Dim strSQL As String = $"@SELECT# * FROM sysroLiveTasks WHERE 
                                        Action IN({taskFilters}) AND ID = {idTask}
                                        ORDER BY Status DESC, ID , TimeStamp "

                Dim tb As DataTable = AccessHelper.CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    roTrace.GetInstance().AddTraceInfo(idTask.ToString(), oRow("Action"), RoAzureSupport.GetCompanyName(), roTypes.Any2String(oRow("TraceGroup")))
                    roTrace.GetInstance().TraceMessage(roTrace.TraceType.roDebug, roTrace.TraceResult.Init, "Task")

                    RunExtraCommand(oRow)


                    oTaskState = New roLiveTaskState(oRow("IDPassport"))
                    oTask = New roLiveTask(oRow("ID"), oTaskState)
                    If oTask.Action = String.Empty OrElse oTask.State.Result <> LiveTasksResultEnum.NoError Then
                        actionResult = roTrace.TraceResult.NotExists
                    Else

                        If oTask.Status = 0 Then
                            oTask.ExecutionDate = Now
                        End If
                        oTask.Status = 1
                        oTask.Save()

                        ' Procesamos el registro
                        Dim taskResult As BaseTaskResult = ExecuteTaskFromManager(oTask)

                        Dim eAction As roLiveTaskTypes = [Enum].Parse(GetType(roLiveTaskTypes), oTask.Action, True)
                        If Not NeedToKeepTask(eAction) Then
                            If Not taskResult.Result Then
                                actionResult = roTrace.TraceResult.Error
                                errorMsg = taskResult.Description
                            End If

                            bRet = oTask.Delete()
                        Else
                            oTask.EndDate = DateTime.Now
                            oTask.ErrorCode = taskResult.Description

                            If taskResult.Result Then
                                oTask.Status = 2
                                oTask.Progress = 100
                            Else
                                oTask.Status = 3
                                actionResult = roTrace.TraceResult.OkWithErrors
                            End If

                            bRet = oTask.Save()
                        End If
                    End If
                Else
                    actionResult = roTrace.TraceResult.NotExists
                End If

            Catch Ex As Exception
                actionResult = roTrace.TraceResult.Error
                errorMsg = Ex.Message
                roLog.GetInstance.logMessage(roLog.EventType.roError, "ExecuteTask:", Ex)
            End Try


            roTrace.GetInstance().TraceMessage(roTrace.TraceType.roInfo, actionResult, errorMsg)

            Return bRet
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            'If Not Me.disposedValue Then
            'End If
            Me.disposedValue = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class

End Namespace