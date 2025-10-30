Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.VTBase

Public Class roNotificationTask_Visit_Update_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Visit_Update, sGUID)
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
        Try
            Dim Params As New ArrayList From {
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy"),
                roTypes.Any2String(_oNotificationTask.Parameters)
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            Select Case _oNotificationTask.key2Numeric
                Case 1
                    oItem.Subject = roNotificationHelper.Message("Notification.Visits.New.Subject", Nothing, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.Visits.New.Body", Params, , oConf.Language)
                Case 2
                    oItem.Subject = roNotificationHelper.Message("Notification.Visits.Update.Subject", Nothing, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.Visits.Update.Body", Params, , oConf.Language)
                Case 3
                    oItem.Subject = roNotificationHelper.Message("Notification.Visits.Delete.Subject", Nothing, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.Visits.Delete.Body", Params, , oConf.Language)
                Case Else
            End Select
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roVisitUpdate::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Return True
    End Function

End Class