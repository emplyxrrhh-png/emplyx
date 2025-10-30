Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBroadcasterCore
Imports Robotics.Base.VTDataLink.DatalinkServer
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTScheduleManager

    Public Class DatalinkTask
        Inherits BaseTask

        Protected Overrides Function GetProcessTaskTypes() As List(Of roLiveTaskTypes)
            Return New Generic.List(Of roLiveTaskTypes) From {
                roLiveTaskTypes.Import,
                roLiveTaskTypes.Export,
                roLiveTaskTypes.GenerateDatalinkTasks,
                roLiveTaskTypes.CTAIMA,
                roLiveTaskTypes.Suprema
            }
        End Function



        Protected Overrides Function ExecuteTaskFromManager(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim datalinkManager As New DatalinkTaskExecution()
            Return datalinkManager.ExecuteTask(oTask)
        End Function


        Protected Overrides Function NeedToKeepTask(ByVal oTaskType As roLiveTaskTypes) As Boolean

            If oTaskType = roLiveTaskTypes.Import OrElse
                oTaskType = roLiveTaskTypes.Export Then
                Return True
            End If

            Return False
        End Function
    End Class

End Namespace