Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Advice_For_New_Conversation
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Advice_For_NewConversation, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return _oNotificationTask.Key1Numeric
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return True
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Dim oDest As roNotificationDestinationConfig() = {}
        oDest = Me.GetDefaultDestinationConfig(False, True, False, True)
        Return oDest
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem

        Dim oItem As New roNotificationItem
        Try

            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Title FROM Channels WHERE ID IN (@SELECT# IdChannel from  ChannelConversations where id=" & _oNotificationTask.key2Numeric & ")")),
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Title FROM ChannelConversations WHERE ID = " & _oNotificationTask.key2Numeric)),
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# ReferenceNumber FROM ChannelConversations WHERE ID = " & _oNotificationTask.key2Numeric))
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()

            oItem.Subject = roNotificationHelper.Message("Notification.AdviceForNewConversation.Subject", Params, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.AdviceForNewConversation.Body", Params, , oConf.Language)

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceForNewConversation::Translate::", ex)
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