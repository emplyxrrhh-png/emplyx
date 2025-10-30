Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications

Public Class roNotificationTask_SchedulerAnalyticExecuted_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.SchedulerAnalyticExecuted, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return -1
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return _oNotificationTask.Key1Numeric
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return True
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig(True, True, False, True)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem

        Dim Params As New ArrayList From {
            _oNotificationTask.Parameters.Replace(vbCrLf, "<br>")
        }

        oItem.Type = NotificationItemType.email
        oItem.CompanyId = RoAzureSupport.GetCompanyName()
        oItem.Subject = roNotificationHelper.Message("Notification.SchedulerAnalyticExecuted.Subject", Nothing, , oConf.Language)
        oItem.Body = roNotificationHelper.Message("Notification.SchedulerAnalyticExecuted.Body", Params, , oConf.Language)
        oItem.Content = String.Empty
        oItem.Destination = oConf.Email

        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Return True
    End Function

End Class