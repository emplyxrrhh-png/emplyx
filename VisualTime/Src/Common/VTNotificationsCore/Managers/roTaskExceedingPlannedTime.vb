Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Task_Exceeding_Planned_Time_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Task_Exceeding_Planned_Time, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return -1
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return True
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig(False, False, True, False)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Tasks WHERE ID = " & _oNotificationTask.Key1Numeric))
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.Taskexceedingplannedtime.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.Taskexceedingplannedtime.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTaskExceedingPlannedTime::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Tareas que han excedido el tiempo teorico previsto
        '
        Dim SQL As String
        Dim bRet As Boolean = True

        Try
            ' Verificamos si existen tareas que se ha sobrepasado el tiempo teorico previsto
            ' y aun no tiene alerta creada
            SQL = "@SELECT# Tasks.ID " &
                         "FROM Tasks " &
                         "WHERE Tasks.InitialTime is not null AND InitialTime > 0" &
                                " AND Status = 0 " &
                                " AND convert(numeric(18,6), InitialTime) < (@SELECT# convert(numeric(18,6),sum(value)) from DailyTaskAccruals where IDTask= Tasks.ID) " &
                                " AND NOT EXISTS " &
                                "(@SELECT# * " &
                                    " From sysroNotificationTasks " &
                                    " Where sysroNotificationTasks.Key1Numeric = Tasks.ID" &
                                    " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime) VALUES (" &
                              roTypes.Any2Double(ads("ID")) & "," & dr("ID") & "," & roTypes.Any2Time(Now).SQLDateTime & ")"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTaskExceedingPlannedTime::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class