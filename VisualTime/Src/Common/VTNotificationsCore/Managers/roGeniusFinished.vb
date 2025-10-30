Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_GeniusFinished_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.GeniusFinished, sGUID)
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
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM GeniusViews WHERE ID = " & _oNotificationTask.key2Numeric)),
                $"<a href='{VTBase.roConstants.VTLiveDefaultURL}Main.aspx#/Genius?loadView={_oNotificationTask.Parameters}'>aquí</a>"
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.GeniusFinished.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.GeniusFinished.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roGeniusFinished::Translate::", ex)
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