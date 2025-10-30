Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Before_Finish_Contract_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Before_Finish_Contract, sGUID)
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
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.BeforeFinishContract.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.BeforeFinishContract.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roBeforeFinishContract::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Proximo fin de contrato
        '
        Dim mCondition As New roCollection
        Dim SQL As String
        Dim bRet As Boolean = True

        Try
            mCondition.LoadXMLString(ads("Condition"))

            ' Obtenemos todos los empleados que vayan a tener un fin de contrato futuro
            ' y queden X dias o menos para que finalice
            ' y ademas no se haya generado la tarea a notificar
            SQL = "@SELECT# * FROM EmployeeContracts " &
                    " WHERE EndDate > " & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime &
                    " AND  DATEDIFF ( ""d"" , GetDate() , Enddate ) <=" & roTypes.Any2Double(mCondition("DaysBefore")) &
                    " AND NOT EXISTS " &
                    "(@SELECT# * " &
                        " From sysroNotificationTasks " &
                        " Where sysroNotificationTasks.Key1Numeric = EmployeeContracts.IDEmployee " &
                        " AND EmployeeContracts.EndDate  =  sysroNotificationTasks.Key3Datetime " &
                        " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") " &
                    " ORDER BY IDEmployee  "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado, creamos una nueva notificacion
                For Each dr As DataRow In dt.Rows
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime) VALUES (" &
                              roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("EndDate")).SQLSmallDateTime & ")"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roBeforeFinishContract::GenerateNotificationTasks:: Unexpected error:", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class