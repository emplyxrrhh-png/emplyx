Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTScheduleManager

    Public Class NotificationTask
        Inherits BaseTask

        Protected Overrides Function GetProcessTaskTypes() As List(Of roLiveTaskTypes)
            Return New Generic.List(Of roLiveTaskTypes) From {
                roLiveTaskTypes.GenerateNotifications,
                roLiveTaskTypes.SendNotifications
            }
        End Function

        Protected Overrides Function ExecuteTaskFromManager(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim eogManager As New NotificationTaskExecution()
            Return eogManager.ExecuteTask(oTask)
        End Function

        Protected Overrides Function NeedToKeepTask(ByVal oTaskType As roLiveTaskTypes) As Boolean
            Return False
        End Function
    End Class

End Namespace