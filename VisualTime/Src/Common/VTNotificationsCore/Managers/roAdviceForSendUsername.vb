Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Advice_For_Send_username_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Advice_For_Send_Username, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return -1
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return _oNotificationTask.key2Numeric
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return True
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig(False, True, False, True)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            Dim strCredential As String = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# isnull(Credential, '') as credential  FROM sysroPassports_AuthenticationMethods WHERE IDPassport = " & _oNotificationTask.key2Numeric & " AND Method=1 "))
            Dim Params As New ArrayList From {strCredential}

            oItem.Type = NotificationItemType.email
            oItem.Destination = oConf.Email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()

            If strCredential.IndexOf("\") >= 0 Then
                If strCredential.StartsWith("\") OrElse strCredential.StartsWith(".\") Then
                    strCredential = strCredential.Replace(".\", "").Replace("\", "")
                    Params(0) = strCredential
                End If
                oItem.Subject = roNotificationHelper.Message("Notification.AdviceSendUsernameAD.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.AdviceSendUsernameAD.Body", Params, , oConf.Language)
            Else
                Params = New ArrayList From {
                    strCredential
                }

                oItem.Subject = roNotificationHelper.Message("Notification.AdviceSendUsername.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.AdviceSendUsername.Body", Params, , oConf.Language)
            End If
            oItem.Content = String.Empty
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceForSendUsername::Translate::", ex)
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