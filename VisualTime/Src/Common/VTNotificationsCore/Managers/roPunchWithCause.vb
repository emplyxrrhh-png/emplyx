Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Punch_With_Cause_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Punch_with_any_Cause, sGUID)
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
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy"),
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID = " & _oNotificationTask.key2Numeric))
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.PunchesWithIncidences.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.PunchesWithIncidences.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roPunchWithCause::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Aviso fichajes con justificacion
        '
        Dim mCondition As New roCollection
        Dim SQL As String
        Dim bRet As Boolean = True

        Try
            mCondition.LoadXMLString(ads("Condition"))

            SQL = "@SELECT# IDEmployee, Datetime, IDCause FROM " &
                " (@SELECT# IDEmployee, Datetime as DateTime, TypeData as IDCause FROM Punches " &
                " WHERE ShiftDate >= " & roTypes.Any2Time(roNotificationHelper.GetInitialDate()).SQLSmallDateTime &
                " AND  TypeData = " & roTypes.Any2Double(mCondition("IDCause")) &
                " AND  Punches.ActualType = 1" &
                " AND NOT EXISTS " &
                    "(@SELECT# * " &
                    " From sysroNotificationTasks " &
                    " Where sysroNotificationTasks.Key1Numeric = Punches.IDEmployee " &
                    " AND Punches.Datetime  =  sysroNotificationTasks.Key3Datetime " &
                    " AND Punches.TypeData =  sysroNotificationTasks.Key2Numeric " &
                    " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") " &
                " UNION " &
                "@SELECT# IDEmployee, Datetime as Datetime, TypeData as IDCause FROM Punches " &
                " WHERE ShiftDate >= " & roTypes.Any2Time(roNotificationHelper.GetInitialDate()).SQLSmallDateTime &
                "  and Punches.ActualType = 2 AND  TypeData = " & roTypes.Any2Double(mCondition("IDCause")) &
                " AND NOT EXISTS " &
                    "(@SELECT# * " &
                    " From sysroNotificationTasks " &
                    " Where sysroNotificationTasks.Key1Numeric = Punches.IDEmployee " &
                    " AND Punches.DateTime  =  sysroNotificationTasks.Key3Datetime " &
                    " AND Punches.TypeData =  sysroNotificationTasks.Key2Numeric " &
                    " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") " &
                " ) as FICHAJES " &
                " ORDER BY IDEmployee, Datetime asc  "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado, creamos una nueva notificacion
                For Each dr As DataRow In dt.Rows
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Key2Numeric) VALUES (" &
                          roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("DateTime")).SQLDateTime & "," & dr("IDCause") & ")"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roPunchWithCause::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class