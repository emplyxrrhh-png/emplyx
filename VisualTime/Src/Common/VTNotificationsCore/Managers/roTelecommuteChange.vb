Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.VTBase

Public Class roNotificationTask_Telecommuting_Change_For_Employee_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Telecommuting_Change_For_Employee, sGUID)
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
        Return Me.GetDefaultDestinationConfig(False, True, False, True)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            Dim Params As New ArrayList From {
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()

            Dim sChange As TelecommutingTypeEnum = System.Enum.Parse(GetType(TelecommutingTypeEnum), _oNotificationTask.Parameters, True)
            If sChange = TelecommutingTypeEnum._AtHome Then
                oItem.Subject = roNotificationHelper.Message("Notification.EmployeeShouldTelecommute.Subject", Params, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.EmployeeShouldTelecommute.Body", Params, , oConf.Language)
            Else
                oItem.Subject = roNotificationHelper.Message("Notification.EmployeeShouldWorkAtOffice.Subject", Params, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.EmployeeShouldWorkAtOffice.Body", Params, , oConf.Language)
            End If

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTelecommuteChange::Translate::", ex)
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