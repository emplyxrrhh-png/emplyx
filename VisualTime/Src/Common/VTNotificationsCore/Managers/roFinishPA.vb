Imports System.Data.Common
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class roNotificationTask_Finish_ProgrammedAbsence_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Finish_Programmed_Absence_and_dont_work_later, sGUID)
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
            ' Vemos si el empleado inputa tiempos mediante declaración de la jornada
            Dim bDailyRecord As Boolean = False

            Dim oLicSupport As New roServerLicense()
            Dim dailyRecordInstalled As Boolean = oLicSupport.FeatureIsInstalled("Feature\DailyRecord")

            If dailyRecordInstalled Then
                Dim sqlCommand As String = "@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures WHERE IdEmployee = @idEmployee AND IdFeature = 20002"
                Dim paramaters As New List(Of CommandParameter)
                paramaters.Add(New CommandParameter("@idEmployee", CommandParameter.ParameterType.tInt, _oNotificationTask.Key1Numeric))
                bDailyRecord = (roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, paramaters)) > 0)
            Else
                bDailyRecord = False
            End If

            If Not bDailyRecord Then
                oItem.Subject = roNotificationHelper.Message("Notification.NoWorkingDays.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.NoWorkingDays.Body", Params, , oConf.Language)
            Else
                oItem.Subject = roNotificationHelper.Message("Notification.NoDailyRecord.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.NoDailyRecord.Body", Params, , oConf.Language)
            End If

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roFinishPA::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Empleados sin venir a trabajar durante X dias
        '
        Dim mCondition As New roCollection
        Dim SQL As String
        Dim NextDate As String
        Dim Key4Date As String
        Dim SQLIDEmployee As String
        Dim Working As Boolean
        Dim bSkipCkeck As Boolean
        Dim IDTaskNotification As Double
        Dim dtScheduler As DataTable
        Dim daysBefore As Double
        Dim iDate As String
        Dim fDate As String
        Dim bRet As Boolean = True

        Try
            mCondition.LoadXMLString(ads("Condition"))

            Dim dInitialDate = roTypes.Any2Time(roNotificationHelper.GetInitialDate()).SQLSmallDateTime
            If UCase(roTypes.Any2String(ads("CreatorID"))) = "SYSTEM" Then
                daysBefore = roTypes.Any2Double(mCondition("DaysNoWorking"))
            Else
                daysBefore = roTypes.Any2Double(mCondition("DaysBefore"))
            End If
            iDate = roTypes.Any2Time(DateTime.Now.Date).Add(-1 * (daysBefore + 7), "d").SQLSmallDateTime
            fDate = roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime

            ' Obtenemos los empleados que en los últimos X días naturales tuvieron solo incidencias de ausencia y no tenian ausencia prolongada
            SQL = roNotificationHelper.CO_SQLNOPUNCH
            SQL = Strings.Replace(SQL, "@iDate", iDate)
            SQL = Strings.Replace(SQL, "@fDate", fDate)
            SQL = Strings.Replace(SQL, "@days", daysBefore)

            ' Preparamos la query que obtiene listado de los últimos X días que debería haber venido a trabajr un empleado
            SQLIDEmployee = roNotificationHelper.CO_SQLNOPUNCHEMP
            SQLIDEmployee = Strings.Replace(SQLIDEmployee, "@iDate", roTypes.Any2Time(DateTime.Now.Date).Add(-1 * (daysBefore + 30), "d").SQLSmallDateTime) 'Fecha inicio de un mes atrás para obtener el maximo de resultados
            SQLIDEmployee = Strings.Replace(SQLIDEmployee, "@fDate", fDate)
            SQLIDEmployee = Strings.Replace(SQLIDEmployee, "@days", daysBefore)

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    ' Obtenemos listado de los últimos X días por empleado que debería haber venido a trabajar
                    SQLIDEmployee = Strings.Replace(SQLIDEmployee, "@idEmployee", dr("IDEmployee"))
                    Working = False

                    dtScheduler = AccessHelper.CreateDataTable(SQLIDEmployee)
                    If dtScheduler IsNot Nothing AndAlso dtScheduler.Rows.Count > 0 Then
                        For Each drScheduler As DataRow In dtScheduler.Rows
                            If roTypes.Any2Double(drScheduler("Status")) <= 40 Then
                                bSkipCkeck = True
                            End If
                            If roTypes.Any2Double(drScheduler("Presente")) > 0 Then
                                Working = True
                            End If
                        Next

                        ' Si no ha trabajado a�adimos tarea de notificacion en caso necesario
                        If Not Working AndAlso Not bSkipCkeck Then
                            NextDate = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# LastPunch from EmployeeStatus where IDEmployee = " & dr("IDEmployee")))
                            If (NextDate.Equals("")) Then
                                NextDate = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# top 1 Date from DailySchedule where IDEmployee = " & dr("IDEmployee") & " and isnull(IDShiftUsed, IDShift1) IN (@SELECT# ID FROM SHIFTS WHERE ExpectedWorkingHours > 0)"))
                            End If

                            Key4Date = NextDate
                            If (Key4Date.Contains(" ")) Then
                                Key4Date = Key4Date.Split(" ")(0)
                            End If

                            ' Verificamos si hay que crear la notificacion
                            ' miramos que no se solape la primera fecha de no trabajo con algun periodo de notificacion ya generado
                            SQL = "@SELECT# ID FROM sysroNotificationTasks WHERE Key1Numeric = " & dr("IDEmployee") &
                                      " AND Key3DateTime<=" & roTypes.Any2Time(NextDate).SQLSmallDateTime &
                                      " AND Key4DateTime>=" & roTypes.Any2Time(Key4Date).SQLSmallDateTime &
                                      " AND IDNotification=" & roTypes.Any2Double(ads("ID"))
                            IDTaskNotification = roTypes.Any2Double(AccessHelper.ExecuteScalar(SQL))
                            If IDTaskNotification = 0 Then
                                ' No existe notificación. La creamos
                                SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Key4DateTime, Key6DateTime) VALUES (" &
                                        roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Time(NextDate).SQLSmallDateTime & "," & roTypes.Any2Time(DateAdd("d", -1, DateTime.Now.Date)).SQLSmallDateTime & "," & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime & ")"
                                bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                            Else
                                ' Existe notificaci�n. Marco para que se notifique nuevamente, siempre que no se haya hecho ya en el d�a de hoy (s�lo una notificaci�n cada d�a)
                                SQL = "@SELECT# Key6DateTime FROM sysroNotificationTasks WHERE ID = " & roTypes.Any2String(IDTaskNotification)
                                Dim dLastNotified As Date
                                dLastNotified = roTypes.Any2DateTime(AccessHelper.ExecuteScalar(SQL))
                                If dLastNotified < DateTime.Now.Date Then
                                    SQL = "@UPDATE# sysroNotificationtasks SET Executed = 0, Key4DateTime=" & roTypes.Any2Time(DateAdd("d", -1, DateTime.Now.Date)).SQLSmallDateTime & ", Key6DateTime= " & roTypes.Any2Time(DateTime.Now.Date).SQLSmallDateTime & " WHERE ID=" & IDTaskNotification &
                                    " AND Key3DateTime<=" & roTypes.Any2Time(NextDate).SQLSmallDateTime &
                                    " AND Key4DateTime>=" & roTypes.Any2Time(NextDate).SQLSmallDateTime
                                Else
                                    SQL = "@UPDATE# sysroNotificationtasks SET Key4DateTime=" & roTypes.Any2Time(DateAdd("d", -1, DateTime.Now.Date)).SQLSmallDateTime & " WHERE ID=" & IDTaskNotification &
                                    " AND Key3DateTime<=" & roTypes.Any2Time(NextDate).SQLSmallDateTime &
                                    " AND Key4DateTime>=" & roTypes.Any2Time(NextDate).SQLSmallDateTime
                                End If
                                bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                            End If
                        Else
                            If bSkipCkeck Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationCore::roFinishPA::GenerateNotificationTasks:: Skipped due to days with status less than 50. Will check again later.")
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roFinishPA::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class