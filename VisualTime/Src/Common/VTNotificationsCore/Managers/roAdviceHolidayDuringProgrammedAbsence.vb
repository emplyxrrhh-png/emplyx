Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Advice_Holiday_During_ProgrammedAbsence_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Advice_Holiday_During_ProgrammedAbsence, sGUID)
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
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric))
            }

            Dim dt As DataTable = AccessHelper.CreateDataTable("@SELECT# causes.name, ProgrammedAbsences.BeginDate  from ProgrammedAbsences inner join causes On causes.id = ProgrammedAbsences.IDCause where AbsenceID = " & roTypes.Any2String(_oNotificationTask.key2Numeric))
            If dt.Rows.Count > 0 Then
                Params.Add(roTypes.Any2DateTime(dt.Rows(0)("BeginDate")).Date)
                Params.Add(roTypes.Any2String(dt.Rows(0)("name")))
            End If

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.HolidayOnProgrammedAbsence.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.HolidayOnProgrammedAbsence.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceHolidayDuringProgrammedAbsence::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Vacaciones dentro de una prevision de ausencia por dia
        '
        Dim SQL As String
        Dim mCondition As New roCollection
        Dim bRet As Boolean = True

        Try
            mCondition.LoadXMLString(ads("Condition"))

            Dim IDCause As Double

            IDCause = roTypes.Any2Double(mCondition("IDCause"))

            ' Obtenemos todos los empleados que tengan vacaciones en los ultimos 90 dias que
            ' esten dentro de previsiones de ausencia por dias
            ' y ademas no se haya generado la tarea a notificar
            SQL = "@SELECT# distinct(absenceid), pa.idemployee from ProgrammedAbsences pa " &
                    " right join dailyschedule ds on pa.IDEmployee = ds.IDEmployee " &
                    " WHERE ds.IsHolidays = 1 and ds.Date between pa.BeginDate and CASE WHEN pa.FinishDate IS NULL THEN DATEADD(day, pa.MaxLastingDays-1, pa.BeginDate) ELSE pa.FinishDate END " &
                               " AND pa.BeginDate >=" & roTypes.Any2Time(DateTime.Now.Date).Add(-90, "d").SQLSmallDateTime
            If IDCause > 0 Then
                SQL = SQL & " AND pa.IDCause = " & roTypes.Any2String(IDCause) & " "
            End If

            SQL = SQL & " AND NOT EXISTS " &
                "(@SELECT# * " &
                    " From sysroNotificationTasks " &
                    " Where sysroNotificationTasks.Key1Numeric = pa.IDEmployee " &
                    " AND sysroNotificationTasks.Key2Numeric =  pa.absenceid " &
                    " AND IDNotification=" & roTypes.Any2Double(ads("ID")) & ") " &
                " ORDER BY pa.idemployee  "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado, creamos una nueva notificacion
                For Each dr As DataRow In dt.Rows
                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric) VALUES (" &
                              roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & dr("absenceid") & ")"
                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roAdviceHolidayDuringProgrammedAbsence::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class