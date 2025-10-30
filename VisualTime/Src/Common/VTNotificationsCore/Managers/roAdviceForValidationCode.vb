Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Advice_For_validation_code_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Advice_For_validation_code, sGUID)
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
            Dim strCode As String = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# ValidationCode FROM sysroPassports WHERE ID = " & _oNotificationTask.key2Numeric))
            strCode = Robotics.VTBase.CryptographyHelper.Decrypt(strCode)

            Dim strCredential As String = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# isnull(Credential, '') as credential  FROM sysroPassports_AuthenticationMethods WHERE IDPassport = " & _oNotificationTask.key2Numeric & " AND Method=1 "))

            If oConf.IdUser.IsRoboticsUser Then
                oItem.Type = NotificationItemType.sms
                oItem.Destination = String.Empty '
                oItem.Body = String.Empty
                oItem.Subject = String.Empty
                oItem.Content = strCredential & ";" & "" & ";" & oConf.Email.Split("@")(0) & ";" & RoAzureSupport.GetCompanyName() & ";" & strCode & ";"
            Else
                Dim Params As New ArrayList From {strCredential, strCode}

                oItem.Type = NotificationItemType.email
                oItem.Destination = oConf.Email
                oItem.CompanyId = RoAzureSupport.GetCompanyName()
                oItem.Subject = roNotificationHelper.Message("Notification.AdviceValidationCode.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.AdviceValidationCode.Body", Params, , oConf.Language)
                oItem.Content = String.Empty
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceForValidationCode::Translate::", ex)
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