Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Mobility_Execution_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Mobility_Execution, sGUID)
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
            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric)),
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Groups WHERE ID = " & _oNotificationTask.key2Numeric))
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.EmployeeMovilityExecuted.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.EmployeeMovilityExecuted.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roMobilityExecution::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Inicios de movilidades que empiezan a dia de hoy
        '
        Dim SQL As String
        Dim sFiredDate As String
        Dim bRet As Boolean = True

        Try
            SQL = "@SELECT# IDGroup,IDEmployee FROM EmployeeGroups " &
                         "WHERE BeginDate = " & roTypes.Any2Time(DateValue(Now)).SQLDateTime &
                                " AND NOT EXISTS " &
                                "(@SELECT# * " &
                                    " From sysroNotificationTasks " &
                                    " Where sysroNotificationTasks.Key1Numeric = EmployeeGroups.IDEmployee " &
                                    " AND EmployeeGroups.BeginDate =  sysroNotificationTasks.Key3Datetime " &
                                    " AND EmployeeGroups.IDGroup = sysroNotificationTasks.Key2Numeric " &
                                    " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    If roTypes.Any2Boolean(ads("ShowOnDesktop")) = True Then
                        sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                    Else
                        sFiredDate = "NULL"
                    End If

                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Key2Numeric, FiredDate) VALUES (" &
                              roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(DateValue(Now)).SQLDateTime & "," & roTypes.Any2Double(dr("IDGroup")) & ", " & sFiredDate & ")"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roMobilityExecution::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class