Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Document_delivered_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Document_delivered, sGUID)
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
        Return Me.GetDefaultDestinationConfig(True, True, False, False, True)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Dim strSubject As String = String.Empty
        Dim strBody As String = String.Empty
        Try
            Dim Params As New ArrayList
            Params.Add(roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric)))
            Dim sDocName As String = _oNotificationTask.Parameters.Split("@)")(0)
            Params.Add(sDocName)
            Dim sRemarks As String = String.Empty
            If _oNotificationTask.Parameters.Split("@)").Length > 1 Then
                sRemarks = _oNotificationTask.Parameters.Split("@)")(1)
            End If
            strSubject = roNotificationHelper.Message("Notification.NewDocumentDelivered.Subject", Nothing, , oConf.Language)
            Params.Add(sRemarks)
            strBody = roNotificationHelper.Message("Notification.NewDocumentDelivered.Body", Params, , oConf.Language)

            ' Detalle sobre la previsión
            Dim iDocID As Integer = _oNotificationTask.key2Numeric
            Dim oDocManager As New VTDocuments.roDocumentManager(New VTDocuments.roDocumentState(-1))
            Dim oDocument As DTOs.roDocument
            oDocument = oDocManager.LoadDocument(iDocID)

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
                    Params.Add(oDocument.DeliveredBy)

                    'sForecastDetail = Message("Notification.NewDocumentDelivered.DaysAbsence", Params, , oEmpConf.Item2)
                    strSubject = roNotificationHelper.Message("Notification.NewDocumentDelivered.DaysAbsence.Subject", Nothing, , oConf.Language)
                    strBody = roNotificationHelper.Message("Notification.NewDocumentDelivered.DaysAbsence.Body", Params, , oConf.Language)
                ElseIf oDocument.IdHoursAbsence > 0 Then
                    Dim oProgCauseState As New VTBusiness.Incidence.roProgrammedCauseState()
                    Dim oProgCause As New VTBusiness.Incidence.roProgrammedCause(oDocument.IdHoursAbsence, oProgCauseState)
                    Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgCause.IDCause))
                    Params.Add(roTypes.Any2DateTime(oProgCause.ProgrammedDate).Date)
                    Params.Add(oDocument.DeliveredBy)

                    'sForecastDetail = Message("Notification.NewDocumentDelivered.HourAbsence", Params, , oEmpConf.Item2)
                    strSubject = roNotificationHelper.Message("Notification.NewDocumentDelivered.HourAbsence.Subject", Nothing, , oConf.Language)
                    strBody = roNotificationHelper.Message("Notification.NewDocumentDelivered.HourAbsence.Body", Params, , oConf.Language)
                ElseIf oDocument.IdOvertimeForecast > 0 Then
                    Dim oProgOvertimeManager As New VTHolidays.roProgrammedOvertimeManager
                    Dim oProgOvertime As New DTOs.roProgrammedOvertime
                    oProgOvertime = oProgOvertimeManager.LoadProgrammedOvertime(oDocument.IdOvertimeForecast)
                    Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgOvertime.IDCause))
                    Params.Add(roTypes.Any2DateTime(oProgOvertime.ProgrammedBeginDate).Date)
                    Params.Add(oDocument.DeliveredBy)

                    'sForecastDetail = Message("Notification.NewDocumentDelivered.Overtime", Params, , oEmpConf.Item2)
                    strSubject = roNotificationHelper.Message("Notification.NewDocumentDelivered.Overtime.Subject", Nothing, , oConf.Language)
                    strBody = roNotificationHelper.Message("Notification.NewDocumentDelivered.Overtime.Body", Params, , oConf.Language)
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