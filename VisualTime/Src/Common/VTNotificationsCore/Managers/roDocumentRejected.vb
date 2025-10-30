Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Document_rejected_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Document_rejected, sGUID)
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
        Dim oDest As roNotificationDestinationConfig() = {}
        Select Case _oNotificationTask.key5Numeric
            Case 0 'Usuario
                oDest = Me.GetDefaultDestinationConfig(False, True, False, True)
            Case 1 'Supervisor
                oDest = Me.GetDefaultDestinationConfig(False, False, False, False, True)
        End Select

        Return oDest
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Dim strSubject As String = String.Empty
        Dim strBody As String = String.Empty
        Try
            Dim Params As New ArrayList
            If _oNotificationTask.key5Numeric = 1 Then
                Params.Add(roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric)))

                Dim iDocID As Integer = _oNotificationTask.key2Numeric
                Dim oDocManager As New VTDocuments.roDocumentManager(New VTDocuments.roDocumentState(-1))
                Dim oDocument As DTOs.roDocument
                oDocument = oDocManager.LoadDocument(iDocID)
                Params.Add(oDocument.DocumentTemplate.Name)

                Dim sRemarks As String = String.Empty
                If _oNotificationTask.Parameters.Split(";").Length > 1 Then
                    sRemarks = _oNotificationTask.Parameters.Split(";)")(1)
                End If
                Params.Add(sRemarks)

                strSubject = roNotificationHelper.Message("Notification.DocumentRejected.Supervisor.Subject", Nothing, , oConf.Language)
                strBody = roNotificationHelper.Message("Notification.DocumentRejected.Supervisor.Body", Params, , oConf.Language)
                ' Si es relativo a una previsión, recojo texto detalle para la notificación al supervisor
                Dim sForecastDetail As String = String.Empty
                If oDocument.IdHoursAbsence > 0 OrElse oDocument.IdDaysAbsence > 0 OrElse oDocument.IdOvertimeForecast > 0 Then
                    'En función del ámbito documento
                    If oDocument.IdDaysAbsence > 0 Then
                        Dim oProgAbsenceState As New VTBusiness.Absence.roProgrammedAbsenceState()
                        Dim oProgAbsence As New VTBusiness.Absence.roProgrammedAbsence(oDocument.IdDaysAbsence, oProgAbsenceState)
                        Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgAbsence.IDCause))
                        Params.Add(roTypes.Any2DateTime(oProgAbsence.BeginDate).Date)

                        If oProgAbsence.FinishDate Is Nothing Then
                            Params.Add(roTypes.Any2DateTime(oProgAbsence.BeginDate).Date.AddDays(oProgAbsence.MaxLastingDays))
                        Else
                            Params.Add(roTypes.Any2DateTime(oProgAbsence.FinishDate).Date)
                        End If

                        strSubject = roNotificationHelper.Message("Notification.ForecastDocumentRejected.Supervisor.Subject", Nothing, , oConf.Language)
                        strBody = roNotificationHelper.Message("Notification.ForecastDocumentRejected.Supervisor.Body", Params, , oConf.Language)
                    ElseIf oDocument.IdHoursAbsence > 0 Then
                        Dim oProgCauseState As New VTBusiness.Incidence.roProgrammedCauseState()
                        Dim oProgCause As New VTBusiness.Incidence.roProgrammedCause(oDocument.IdHoursAbsence, oProgCauseState)
                        Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgCause.IDCause))
                        Params.Add(roTypes.Any2DateTime(oProgCause.ProgrammedDate).Date)
                        Params.Add(roTypes.Any2DateTime(oProgCause.BeginTime).ToShortTimeString())
                        Params.Add(roTypes.Any2DateTime(oProgCause.EndTime).ToShortTimeString())

                        strSubject = roNotificationHelper.Message("Notification.HoursForecastDocumentRejected.Supervisor.Subject", Nothing, , oConf.Language)
                        strBody = roNotificationHelper.Message("Notification.HoursForecastDocumentRejected.Supervisor.Body", Params, , oConf.Language)
                    ElseIf oDocument.IdOvertimeForecast > 0 Then
                        Dim oProgOvertimeManager As New VTHolidays.roProgrammedOvertimeManager
                        Dim oProgOvertime As New DTOs.roProgrammedOvertime
                        oProgOvertime = oProgOvertimeManager.LoadProgrammedOvertime(oDocument.IdOvertimeForecast)
                        Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgOvertime.IDCause))
                        Params.Add(roTypes.Any2DateTime(oProgOvertime.ProgrammedBeginDate).Date)
                        Params.Add(roTypes.Any2DateTime(oProgOvertime.BeginTime).ToShortTimeString())
                        Params.Add(roTypes.Any2DateTime(oProgOvertime.EndTime).ToShortTimeString())

                        strSubject = roNotificationHelper.Message("Notification.HoursForecastDocumentRejected.Supervisor.Subject", Nothing, , oConf.Language)
                        strBody = roNotificationHelper.Message("Notification.HoursForecastDocumentRejected.Supervisor.Body", Params, , oConf.Language)
                    End If
                End If
            Else

                Dim iDocID As Integer = _oNotificationTask.key2Numeric
                    Dim oDocManager As New VTDocuments.roDocumentManager(New VTDocuments.roDocumentState(-1))
                    Dim oDocument As DTOs.roDocument
                    oDocument = oDocManager.LoadDocument(iDocID)
                    Params.Add(oDocument.DocumentTemplate.Name)

                    Dim sRemarks As String = String.Empty
                    If _oNotificationTask.Parameters.Split(";").Length > 1 Then
                        sRemarks = _oNotificationTask.Parameters.Split(";)")(1)
                    End If
                    Params.Add(sRemarks)

                    strSubject = roNotificationHelper.Message("Notification.DocumentRejected.Employee.Subject", Nothing, , oConf.Language)
                    strBody = roNotificationHelper.Message("Notification.DocumentRejected.Employee.Body", Params, , oConf.Language)
                    ' Si es relativo a una previsión, recojo texto detalle para la notificación al supervisor
                    Dim sForecastDetail As String = String.Empty
                If oDocument.IdHoursAbsence > 0 OrElse oDocument.IdDaysAbsence > 0 OrElse oDocument.IdOvertimeForecast > 0 Then
                    'En función del ámbito documento
                    If oDocument.IdDaysAbsence > 0 Then
                        Dim oProgAbsenceState As New VTBusiness.Absence.roProgrammedAbsenceState()
                        Dim oProgAbsence As New VTBusiness.Absence.roProgrammedAbsence(oDocument.IdDaysAbsence, oProgAbsenceState)
                        Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgAbsence.IDCause))
                        Params.Add(roTypes.Any2DateTime(oProgAbsence.BeginDate).Date)

                        If oProgAbsence.FinishDate Is Nothing Then
                            Params.Add(roTypes.Any2DateTime(oProgAbsence.BeginDate).Date.AddDays(oProgAbsence.MaxLastingDays))
                        Else
                            Params.Add(roTypes.Any2DateTime(oProgAbsence.FinishDate).Date)
                        End If

                        strSubject = roNotificationHelper.Message("Notification.ForecastDocumentRejected.Employee.Subject", Nothing, , oConf.Language)
                        strBody = roNotificationHelper.Message("Notification.ForecastDocumentRejected.Employee.Body", Params, , oConf.Language)
                    ElseIf oDocument.IdHoursAbsence > 0 Then
                        Dim oProgCauseState As New VTBusiness.Incidence.roProgrammedCauseState()
                        Dim oProgCause As New VTBusiness.Incidence.roProgrammedCause(oDocument.IdHoursAbsence, oProgCauseState)
                        Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgCause.IDCause))
                        Params.Add(roTypes.Any2DateTime(oProgCause.ProgrammedDate).Date)
                        Params.Add(roTypes.Any2DateTime(oProgCause.BeginTime).ToShortTimeString())
                        Params.Add(roTypes.Any2DateTime(oProgCause.EndTime).ToShortTimeString())

                        strSubject = roNotificationHelper.Message("Notification.HoursForecastDocumentRejected.Employee.Subject", Nothing, , oConf.Language)
                        strBody = roNotificationHelper.Message("Notification.HoursForecastDocumentRejected.Employee.Body", Params, , oConf.Language)
                    ElseIf oDocument.IdOvertimeForecast > 0 Then
                        Dim oProgOvertimeManager As New VTHolidays.roProgrammedOvertimeManager
                        Dim oProgOvertime As New DTOs.roProgrammedOvertime
                        oProgOvertime = oProgOvertimeManager.LoadProgrammedOvertime(oDocument.IdOvertimeForecast)
                        Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgOvertime.IDCause))
                        Params.Add(roTypes.Any2DateTime(oProgOvertime.ProgrammedBeginDate).Date)
                        Params.Add(roTypes.Any2DateTime(oProgOvertime.BeginTime).ToShortTimeString())
                        Params.Add(roTypes.Any2DateTime(oProgOvertime.EndTime).ToShortTimeString())

                        strSubject = roNotificationHelper.Message("Notification.HoursForecastDocumentRejected.Employee.Subject", Nothing, , oConf.Language)
                        strBody = roNotificationHelper.Message("Notification.HoursForecastDocumentRejected.Employee.Body", Params, , oConf.Language)
                    End If
                End If
            End If
            oItem.Type = NotificationItemType.email
            oItem.Destination = oConf.Email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = strSubject
            oItem.Body = strBody
            oItem.Content = String.Empty
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roDocumentDelivered::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Return True
    End Function

End Class