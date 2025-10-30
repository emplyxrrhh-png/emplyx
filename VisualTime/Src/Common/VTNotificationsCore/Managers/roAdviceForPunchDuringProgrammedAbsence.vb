Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Advice_For_Punch_During_ProgrammedAbsence_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Advice_For_Punch_During_ProgrammedAbsence, sGUID)
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
        Return Me.GetDefaultDestinationConfig(,,,,, True)
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
            oItem.Subject = roNotificationHelper.Message("Notification.PunchOnProgrammedAbsence.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.PunchOnProgrammedAbsence.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceForPunchDuringProgrammedAbsence::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Fichaje dentro de una prevision de ausencia por dia
        '
        Dim SQL As String
        Dim mCondition As New roCollection
        Dim bRet As Boolean = True

        Try
            mCondition.LoadXMLString(ads("Condition"))

            Dim IDCause As Double
            Dim strPunchTypeToIgnore As String

            IDCause = roTypes.Any2Double(mCondition("IDCause"))
            strPunchTypeToIgnore = roTypes.Any2String(mCondition("TypeToIgnore"))

            ' Obtenemos todos los empleados que tengan fichajes en los ultimos 30 dias que
            ' esten dentro de previsiones de ausencia por dias
            ' y ademas no se haya generado la tarea a notificar
            SQL = "@SELECT# * FROM Punches " &
                    " WHERE ShiftDate >=" & roTypes.Any2Time(DateTime.Now.Date).Add(-30, "d").SQLSmallDateTime & " AND " &
                               " ShiftDate <=" & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime
            If strPunchTypeToIgnore <> "" Then
                SQL = SQL & " AND Type not in (" & strPunchTypeToIgnore & ") "
            End If

            SQL = SQL &
                    " AND EXISTS " &
                    "(@SELECT# * " &
                        " From ProgrammedAbsences where Punches.IDEmployee = ProgrammedAbsences.IDEmployee AND ProgrammedAbsences.BeginDate <= Punches.ShiftDate AND  CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END >= Punches.ShiftDate "
            If IDCause > 0 Then
                SQL = SQL & " AND IDCause = " & roTypes.Any2String(IDCause) & " "
            End If
            SQL = SQL & ") " &
                " AND NOT EXISTS " &
                "(@SELECT# * " &
                    " From sysroNotificationTasks " &
                    " Where sysroNotificationTasks.Key1Numeric = Punches.IDEmployee " &
                    " AND sysroNotificationTasks.Key3Datetime =  Punches.ShiftDate " &
                    " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") " &
                " ORDER BY IDEmployee  "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado, creamos una nueva notificacion
                For Each dr As DataRow In dt.Rows
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime) VALUES (" &
                              roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(dr("ShiftDate")).SQLSmallDateTime & ")"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceForPunchDuringProgrammedAbsence::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class