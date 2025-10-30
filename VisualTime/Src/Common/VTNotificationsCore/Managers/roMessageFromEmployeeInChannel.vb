Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Message_FromEmployee_InChannel
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.NewMessage_FromEmployee_InChannel, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return _oNotificationTask.Key1Numeric
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return False
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Dim oDest As roNotificationDestinationConfig() = {}
        oDest = Me.GetDefaultDestinationConfig(False, False, False, False, False, False, True)

        Return oDest
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem

        Dim oItem As New roNotificationItem
        Try
            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Employees WHERE ID = " & _oNotificationTask.Key1Numeric)),
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Title FROM Channels WHERE ID IN (@SELECT# IdChannel from  ChannelConversations where id=" & _oNotificationTask.key2Numeric & ")")),
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Title FROM ChannelConversations WHERE ID = " & _oNotificationTask.key2Numeric)),
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# ReferenceNumber FROM ChannelConversations WHERE ID = " & _oNotificationTask.key2Numeric))
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()

            Select Case _oNotificationTask.key5Numeric
                Case 0 'Nuevo mensaje al canal
                    oItem.Subject = roNotificationHelper.Message("Notification.NewAnswerFromEmployee.Subject", Params, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.NewAnswerFromEmployee.Body", Params, , oConf.Language)
                Case 1 'Nueva conversación
                    oItem.Subject = roNotificationHelper.Message("Notification.NewAnswerFromEmployee.NewConversation.Subject", Params, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.NewAnswerFromEmployee.NewConversation.Body", Params, , oConf.Language)
                Case 2 'Nueva denuncia
                    oItem.Subject = roNotificationHelper.Message("Notification.newanswerfromemployee.NewComplaint.Subject", Params, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.newanswerfromemployee.NewComplaint.Body", Params, , oConf.Language)
                Case 3 'Nuevo mensaje en denuncia
                    oItem.Subject = roNotificationHelper.Message("Notification.newanswerfromemployee.NewComplaintMessage.Subject", Params, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.newanswerfromemployee.NewComplaintMessage.Body", Params, , oConf.Language)
            End Select

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roMessageFromEmployeeInChannel::Translate::", ex)
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