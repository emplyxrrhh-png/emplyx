Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Request_Shift_Change_Manager
    Inherits roNotificationTaskManager

    Private idEmployee As Integer = -1
    Private idExchangeEmployee As Integer = -1

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Request_Shift_Change, sGUID)
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
        Return {}
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Return Nothing
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Solicitudes de cambio de horario
        '
        Dim SQL As String
        Dim bRet As Boolean = True

        Try
            ' Verificamos si existen tareas que de solicitudes de cambio de horario
            ' y aun no tiene alerta creada

            SQL = "@SELECT# Requests.ID, Requests.IDEmployee , Requests.Date1, Requests.Date2, IDShift   " &
                         "FROM Requests " &
                         "WHERE RequestType=5 AND Status In(0,1) " &
                                " AND NOT EXISTS " &
                                "(@SELECT# * " &
                                    " From sysroNotificationTasks " &
                                    " Where sysroNotificationTasks.Key1Numeric = Requests.ID" &
                                    " AND Requests.IDEmployee = sysroNotificationTasks.Key2Numeric  " &
                                    " AND Requests.Date1 = sysroNotificationTasks.Key3DateTime  " &
                                    " AND Requests.Date2 = sysroNotificationTasks.Key4DateTime  " &
                                    " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime, Key4DateTime, Parameters) VALUES (" &
                              roTypes.Any2Double(ads("ID")) & "," & dr("ID") & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("Date1")).SQLDateTime & "," & roTypes.Any2Time(dr("Date2")).SQLDateTime & ",'" & roTypes.Any2Double(dr("IDShift")) & "')"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roRequestShiftChange::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class