Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Advice_ScheduleRule_Indictment_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Advice_ScheduleRule_Indictment, sGUID)
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
        Return Me.GetDefaultDestinationConfig()
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            Dim SubjectParams As New ArrayList
            Dim strRuleName As String = String.Empty
            Dim strDetail As String = String.Empty

            Dim strParameters = roTypes.Any2String(_oNotificationTask.Parameters)
            If strParameters.Contains("@") Then
                strRuleName = strParameters.Split("@")(0)
                strDetail = strParameters.Split("@")(1)
            End If
            SubjectParams.Add(strRuleName)

            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric)),
                roTypes.Any2String(strRuleName),
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy"),
                Format$(_oNotificationTask.key4Datetime, "dd/MM/yyyy"),
                roTypes.Any2String(strDetail)
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.ScheduleRuleIndictment.Subject", SubjectParams, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.ScheduleRuleIndictment.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceScheduleRuleIndictment::Translate::", ex)
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