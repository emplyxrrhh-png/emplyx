Imports Robotics.Base.DTOs
Imports Robotics.Base.VTDataLink.DatalinkServer
Imports Robotics.Base.VTEngineManager
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTScheduleManager

    Public Class EngineServerTask
        Inherits BaseTask

        Protected Overrides Function GetProcessTaskTypes() As List(Of roLiveTaskTypes)
            Return New Generic.List(Of roLiveTaskTypes) From {
                roLiveTaskTypes.RunEngine,
                roLiveTaskTypes.RunEngineEmployee,
                roLiveTaskTypes.UpdateEngineCache
            }
        End Function

        Protected Overrides Sub RunExtraCommand(ByVal oRow As DataRow)
            If oRow("Action") <> roLiveTaskTypes.UpdateEngineCache.ToString().ToUpper Then
                DataLayer.roCacheManager.GetInstance().CheckEngineCacheNeedsUpdate(Azure.RoAzureSupport.GetCompanyName())
            End If
        End Sub

        Protected Overrides Function ExecuteTaskFromManager(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim engineManager As New EngineServerTaskExecution()
            Return engineManager.ExecuteTask(oTask)
        End Function


        Protected Overrides Function NeedToKeepTask(ByVal oTaskType As roLiveTaskTypes) As Boolean
            Return False
        End Function


    End Class

End Namespace