Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Advice_For_New_password_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Advice_For_New_password, sGUID)
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

            If _oNotificationTask.key5Numeric = AuthenticationMethod.Pin Then
                Params = New ArrayList From {
                    roTypes.Any2String(_oNotificationTask.Key1Numeric),
                    roTypes.Any2String(_oNotificationTask.Parameters)
                }

                oItem.Subject = roNotificationHelper.Message("Notification.AdviceNewPasswordPin.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.AdviceNewPasswordPin.Body", Params, , oConf.Language)
            Else
                If strCredential.IndexOf("\") >= 0 Then
                    oItem.Subject = roNotificationHelper.Message("Notification.AdviceNewPasswordAD.Subject", Nothing, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.AdviceNewPasswordAD.Body", Params, , oConf.Language)
                Else
                    Params = New ArrayList From {
                        Robotics.VTBase.CryptographyHelper.Decrypt(_oNotificationTask.Parameters)
                    }

                    oItem.Subject = roNotificationHelper.Message("Notification.AdviceNewPassword.Subject", Nothing, , oConf.Language)
                    oItem.Body = roNotificationHelper.Message("Notification.AdviceNewPassword.Body", Params, , oConf.Language)
                End If
            End If
            oItem.Content = String.Empty
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceForNewPassword::Translate::", ex)
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