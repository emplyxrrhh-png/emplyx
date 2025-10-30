Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace Notifications

    Public Class NotificationTaskExecution

#Region "Declarations - Constructor"

        Public Sub New()
        End Sub

#End Region

#Region "Methods"

        Public Function ExecuteTask(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = False

            Try
                Select Case UCase(oTask.Action)
                    Case roLiveTaskTypes.GenerateNotifications.ToString().ToUpper
                        Dim oNotificationManager As New roNotificationCreateManager()
                        oNotificationManager.ExecuteCreateNotifications()
                    Case roLiveTaskTypes.SendNotifications.ToString().ToUpper
                        Dim oNotificationManager As New roNotificationSendManager()
                        oNotificationManager.ExecuteSendNotifications()
                End Select
                bolRet = True
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "NotificationTaskExecution::ExecuteTask :", Ex)
                bolRet = False
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

#End Region


    End Class

End Namespace