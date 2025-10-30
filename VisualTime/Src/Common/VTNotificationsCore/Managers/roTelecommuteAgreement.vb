Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_TelecommuteAgreement_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.TelecommuteAgreement, sGUID)
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
        Return Me.GetDefaultDestinationConfig(False, True, False, True)
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

            oItem.Subject = roNotificationHelper.Message("Notification.TelecommuteAgreement.Subject", Params, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.TelecommuteAgreement.Body", Params, , oConf.Language)

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTelecommuteAgreement::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Dim SQL As String
        Dim bRet As Boolean = True
        Try
            ' Verificamos si existen tareas que empiezan hoy
            ' y aun no tiene alerta creada
            SQL = "@SELECT# tc.IDEmployee, tc.TelecommutingAgreementEnd,  DATEDIFF(month, GETDATE(), tc.TelecommutingAgreementEnd) from sysrovwTelecommutingAgreement tc " &
                      " inner join sysroEmployeeGroups eg on eg.IDEmployee = tc.IDEmployee and eg.CurrentEmployee = 1 " &
                      " where getdate() between tc.TelecommutingAgreementStart and tc.TelecommutingAgreementEnd and getdate() between tc.ContractStart and tc.ContractEnd " &
                      " and DATEDIFF(day, GETDATE(), tc.TelecommutingAgreementEnd) <= 60 " &
                            " AND NOT EXISTS " &
                            "(@SELECT# * " &
                                " From sysroNotificationTasks " &
                                " Where sysroNotificationTasks.Key1Numeric = tc.IDEmployee" &
                                " AND sysroNotificationTasks.Key3Datetime = tc.TelecommutingAgreementEnd " &
                                " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado
                For Each dr As DataRow In dt.Rows
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime) VALUES (" &
                            roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("TelecommutingAgreementEnd")).SQLDateTime & ")"

                    bRet = bRet AndAlso DataLayer.AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTelecommuteAgreement::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class