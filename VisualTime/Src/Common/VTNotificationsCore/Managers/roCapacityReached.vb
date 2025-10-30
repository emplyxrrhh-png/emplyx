Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_CapacityReached_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.CapacityReached, sGUID)
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
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Zones where id = " & _oNotificationTask.key2Numeric)),
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy HH:mm")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            If _oNotificationTask.Parameters = "REACHED" Then
                oItem.Subject = roNotificationHelper.Message("Notification.CapacityReached.Reached.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.CapacityReached.Reached.Body", Params, , oConf.Language)
            ElseIf _oNotificationTask.Parameters = "AVAILABLE" Then
                oItem.Subject = roNotificationHelper.Message("Notification.CapacityReached.Available.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.CapacityReached.Available.Body", Params, , oConf.Language)
            Else
                oItem.Subject = String.Empty
                oItem.Body = String.Empty
            End If

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roCapacityReached::Translate::", ex)
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