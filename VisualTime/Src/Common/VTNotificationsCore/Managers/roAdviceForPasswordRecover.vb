Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Advice_For_Password_Recover_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Advice_For_Password_Recover, sGUID)
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
                oItem.Subject = String.Empty
                oItem.Body = String.Empty
            Else
                Params = New ArrayList From {
                    roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# RecoverKey FROM sysroPassports_Data WHERE recoverkey is not null and IDPassport = " & _oNotificationTask.key2Numeric))
                }

                oItem.Subject = roNotificationHelper.Message("Notification.RecoverPassword.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.RecoverPassword.Body", Params, , oConf.Language)
            End If
            oItem.Content = String.Empty
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceForPasswordRecover::Translate::", ex)
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