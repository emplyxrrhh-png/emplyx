Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBroadcasterCore
Imports Robotics.Base.VTPunchConnector
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTScheduleManager

    Public Class BroadcasterTask
        Inherits BaseTask

        Protected Overrides Function GetProcessTaskTypes() As List(Of roLiveTaskTypes)
            Return New Generic.List(Of roLiveTaskTypes) From {
                roLiveTaskTypes.BroadcasterTask
            }
        End Function

        Protected Overrides Function ExecuteTaskFromManager(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim broadcasterManager As New BroadcasterTaskExecution()
            Return broadcasterManager.ExecuteTask(oTask)
        End Function


        Protected Overrides Function NeedToKeepTask(ByVal oTaskType As roLiveTaskTypes) As Boolean
            Return False
        End Function

    End Class
End Namespace