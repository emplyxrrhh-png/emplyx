Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Day_with_Unreliable_Time_Record_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Day_with_Unreliable_Time_Record, sGUID)
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
            oItem.Subject = roNotificationHelper.Message("Notification.Daywithunreliabletimerecord.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.Daywithunreliabletimerecord.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roUnreliableTimeRecord::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Dias con fichajes no fiables
        '
        Dim SQL As String
        Dim bRet As Boolean = True

        Try
            Dim notificationRequired As Boolean = (roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("VTPortal.NotifyUnreliablePunch", New AdvancedParameter.roAdvancedParameterState).Value).ToUpper <> "FALSE")
            If Not notificationRequired Then Return True

            ' Verificamos si existen empleados con fichajes no fiables y aun no tiene alerta creada
            SQL = "@SELECT# Punches.IDEmployee, Punches.ShiftDate AS Date " &
                        "FROM Employees,Punches " &
                        "WHERE Employees.ID = Punches.IDEmployee AND " &
                            " IsNotReliable = 1  AND " &
                            "( ActualType IN(1,2)) AND " &
                            " ShiftDate >=" & roTypes.Any2Time(DateTime.Now.Date).Add(-30, "d").SQLSmallDateTime & " AND " &
                            " ShiftDate <" & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime &
                            " AND NOT EXISTS " &
                            "(@SELECT# * " &
                                " From sysroNotificationTasks " &
                                " Where sysroNotificationTasks.Key1Numeric = Punches.IDEmployee " &
                                " AND Punches.ShiftDate =  sysroNotificationTasks.Key3Datetime " &
                                " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

            SQL = SQL & " GROUP BY Punches.ShiftDate, Punches.IDEmployee "
            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    Dim sFiredDate As String
                    If roTypes.Any2Boolean(ads("ShowOnDesktop")) = True Then
                        sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                    Else
                        sFiredDate = "NULL"
                    End If
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, FiredDate) VALUES (" &
                    roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("Date")).SQLDateTime & ", " & sFiredDate & ")"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roUnreliableTimeRecord::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return True
    End Function

End Class