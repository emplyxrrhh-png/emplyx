Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Task_Close_to_Start_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Task_Close_to_Start, sGUID)
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
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Tasks WHERE ID = " & _oNotificationTask.Key1Numeric)),
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.Taskclosetostart.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.Taskclosetostart.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTaskCloseToStart::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' tareas que empiezan hoy
        '
        Dim SQL As String
        Dim bRet As Boolean = True

        Try
            ' Verificamos si existen tareas que empiezan hoy
            ' y aun no tiene alerta creada
            SQL = "@SELECT# Tasks.ID, Tasks.ExpectedStartDate " &
                         "FROM Tasks " &
                         "WHERE Tasks.ExpectedStartDate is not null " &
                                " AND ExpectedStartDate >=  " & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime &
                                " AND ExpectedStartDate <  " & roTypes.Any2Time(DateTime.Now.Date).Add(1, "d").SQLSmallDateTime &
                                " AND Status = 0 " &
                                " AND NOT EXISTS " &
                                "(@SELECT# * " &
                                    " From sysroNotificationTasks " &
                                    " Where sysroNotificationTasks.Key1Numeric = Tasks.ID" &
                                    " AND Tasks.ExpectedStartDate =  sysroNotificationTasks.Key3Datetime " &
                                    " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime) VALUES (" &
                              roTypes.Any2Double(ads("ID")) & "," & dr("ID") & "," & roTypes.Any2Time(dr("ExpectedStartDate")).SQLDateTime & ")"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTaskCloseToStart::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class