Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Invalid_Access_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Invalid_Access, sGUID)
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
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Zones WHERE ID IN(@SELECT# IDZone FROM Punches WHERE ID=" & _oNotificationTask.key2Numeric & ")")),
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.InvalidAccess.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.InvalidAccess.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roInvalidAccess::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Empleados con accesos invalidos
        '
        Dim mCondition As New roCollection
        Dim SQL As String
        Dim bRet As Boolean = True

        Try

            SQL = "@SELECT# ID, IDEmployee, DateTime FROM Punches " &
                    " WHERE DateTime >= " & roTypes.Any2Time(DateTime.Now.Date.AddDays(-1)).SQLDateTime &
                    " AND ActualType = 6" &
                    " AND NOT EXISTS " &
                        "(@SELECT# * " &
                        " From sysroNotificationTasks " &
                        " Where sysroNotificationTasks.Key1Numeric = Punches.IDEmployee " &
                        " AND Punches.DateTime  =  sysroNotificationTasks.Key3Datetime " &
                        " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

            ' Obtenemos todos los fichajes invalidos a partir de la fecha en que se inicio por primera vez el proceso
            ' y ademas no se haya generado la tarea a notificar
            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado, creamos una nueva notificacion
                For Each dr As DataRow In dt.Rows
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Key2Numeric) VALUES (" &
                              roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("DateTime")).SQLDateTime & "," & dr("ID") & ")"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roInvalidAccess::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class