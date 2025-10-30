Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_IDCard_Not_Assigned_To_Employee_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.IDCard_Not_Assigned_To_Employee, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return -1
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return False
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig(True, False, False, False)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.IDCardnotassignedtoemployee.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.IDCardnotassignedtoemployee.Body", Nothing, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roCardNotAssigned::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Fichajes no asignados a ningun empleado
        '
        Dim SQL As String
        Dim bRet As Boolean = True

        Try
            ' Verificamos si existen fichajes sin empleado asignado
            ' y aun no tiene alerta creada
            SQL = "@SELECT# count(*) as Total FROM Punches WHERE IDCredential > 0 and IDEmployee= 0 " &
                            " AND NOT EXISTS " &
                            "(@SELECT# * " &
                                " From sysroNotificationTasks " &
                                " Where IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

            If roTypes.Any2Double(AccessHelper.ExecuteScalar(SQL)) > 0 Then
                SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key3DateTime) VALUES (" &
                          roTypes.Any2Double(ads("ID")) & "," & roTypes.Any2Time(Now).SQLDateTime & ")"
                bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roCardNotAssigned::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class