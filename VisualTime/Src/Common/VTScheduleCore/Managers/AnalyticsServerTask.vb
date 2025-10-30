Imports Robotics.Base.DTOs
Imports Robotics.Base.VTAnalyticsManager
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTScheduleManager

    Public Class AnalyticsServerTask
        Inherits BaseTask

        Protected Overrides Function GetProcessTaskTypes() As List(Of roLiveTaskTypes)
            Return New Generic.List(Of roLiveTaskTypes) From {
                roLiveTaskTypes.AnalyticsTask,
                roLiveTaskTypes.GenerateAnalyticsTasks
            }
        End Function

        Protected Overrides Function ExecuteTaskFromManager(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim analyticsManager As New AnalyticsServerTaskExecution()
            Return analyticsManager.ExecuteTask(oTask)
        End Function


        Protected Overrides Function NeedToKeepTask(oTaskType As roLiveTaskTypes) As Boolean
            If oTaskType = roLiveTaskTypes.AnalyticsTask Then Return True

            Return False
        End Function

    End Class

End Namespace