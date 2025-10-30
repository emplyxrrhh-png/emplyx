Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Non_Justified_Incident_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Non_Justified_Incident, sGUID)
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
            oItem.Subject = roNotificationHelper.Message("Notification.Nonjustifiedincident.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.Nonjustifiedincident.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roNotJustifiedIncident::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Dias con incidencias sin justificar
        '
        Dim SQL As String
        Dim bolInsert As Boolean
        Dim bRet As Boolean = True

        Try
            ' Verificamos si existen empleados con incidencias sin justificar
            ' y aun no tiene alerta creada
            SQL = "@SELECT# DailyCauses.IDEmployee, DailyCauses.Date " &
                         "FROM Employees INNER JOIN " &
                                "DailyCauses ON Employees.ID = DailyCauses.IDEmployee " &
                         "WHERE DailyCauses.IDCause = 0 AND " &
                            " Date >=" & roTypes.Any2Time(DateTime.Now.Date).Add(-30, "d").SQLSmallDateTime & " AND " &
                            " Date <" & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime &
                            " AND NOT EXISTS " &
                            "(@SELECT# * " &
                                " From sysroNotificationTasks " &
                                " Where sysroNotificationTasks.Key1Numeric = DailyCauses.IDEmployee " &
                                " AND DailyCauses.Date =  sysroNotificationTasks.Key3Datetime " &
                                " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "
            SQL = SQL & " GROUP BY DailyCauses.Date, DailyCauses.IDEmployee"

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    Dim sFiredDate As String

                    bolInsert = True
                    'Si la fecha es ayer, revisamos que no tenga horario nocturno
                    If roTypes.Any2Time(dr("Date")).Value = roTypes.Any2Time(DateTime.Now.Date).Add(-1, "d").Value Then
                        bolInsert = roTypes.Any2Boolean(AccessHelper.ExecuteScalar("@SELECT# CAST(CASE WHEN CAST(StartLimit as Date) <> CAST(EndLimit as Date) Then 0 Else 1 END as bit) FROM DailySchedule JOIN Shifts ON DailySchedule.IDShiftUsed = Shifts.ID WHERE IDEmployee=" & roTypes.Any2Double(dr("IDEmployee")) & " AND Date=" & roTypes.Any2Time(dr("Date")).SQLSmallDateTime))
                    End If

                    If bolInsert Then
                        If roTypes.Any2Boolean(ads("ShowOnDesktop")) = True Then
                            sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                        Else
                            sFiredDate = "NULL"
                        End If
                        SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, FiredDate) VALUES (" &
                                  roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("Date")).SQLDateTime & ", " & sFiredDate & ")"
                        bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                    End If
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roNotJustifiedIncident::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class