Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_EndPeriodEnterprise_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.End_Period_Enterprise, sGUID)
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
            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Groups where ID = " & _oNotificationTask.Key1Numeric)),
                _oNotificationTask.Conditions("DatePeriodUserfield").ToString(),
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.CompanyFieldExpired.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.CompanyFieldExpired.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEndPeriodEnterprise::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Campos de la ficha de la emrpesa que van a expirar
        '
        Dim SQL As String
        Dim mCondition As New roCollection
        Dim strFinishPeriod As String
        Dim bRet As Boolean = True

        Try
            mCondition.LoadXMLString(ads("Condition"))

            ' Obtenemos todos los empleados que tienen asignado el campo de la ficha indicado
            SQL = "@SELECT# * FROM GROUPS WHERE [USR_" & Strings.Replace(mCondition("DatePeriodUserfield"), "'", "''") & "] Is not null Order by ID"

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado, creamos una nueva notificacion en caso necesario
                For Each dr As DataRow In dt.Rows
                    strFinishPeriod = roTypes.String2Item(roTypes.Any2String(dr("USR_" & Strings.Replace(mCondition("DatePeriodUserfield"), "'", "''"))), 1, "*")
                    If IsDate(strFinishPeriod) Then
                        ' Si la fecha de final es >= a hoy
                        If roTypes.Any2Time(strFinishPeriod).Value >= roTypes.Any2Time(DateTime.Now.Date).Value Then
                            ' Si la diferencia de dias entre hoy y la fecha de fin es <= X dias debemos crear una notificacion
                            If DateDiff("d", DateTime.Now.Date, roTypes.Any2Time(strFinishPeriod).Value) <= roTypes.Any2Double(mCondition("DaysBefore")) Then
                                SQL = "@SELECT# count(*) as Total from sysroNotificationtasks WHERE IDNotification = " & roTypes.Any2Double(ads("ID"))
                                SQL = SQL & " AND Key1Numeric = " & dr("ID")
                                SQL = SQL & " AND Key3DateTime = " & roTypes.Any2Time(strFinishPeriod).SQLDateTime
                                If roTypes.Any2Double(AccessHelper.ExecuteScalar(SQL)) = 0 Then
                                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime) VALUES (" &
                                            roTypes.Any2Double(ads("ID")) & "," & dr("ID") & "," & roTypes.Any2Time(strFinishPeriod).SQLDateTime & ")"
                                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                                End If
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roEndPeriodEnterprise::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class