Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTDocuments
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Document_Pending_Manager
    Inherits roNotificationTaskManager

    Dim bCheckRepetition As Boolean = False
    Dim iRepeatEveryDays As Integer = -1
    Dim iMaxRepetition As Integer = -1
    Dim dNextRepetition As Date = Date.MinValue

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Document_Pending, sGUID)

        iRepeatEveryDays = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "DocumentPendingNotification.RepeatEachDays"))
        iMaxRepetition = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "DocumentPendingNotification.RepeatTimes"))
        dNextRepetition = Now.Date.AddDays(iRepeatEveryDays).Date
        If iMaxRepetition > 0 AndAlso iRepeatEveryDays > 0 Then
            bCheckRepetition = True
        End If
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
        Return Me.GetDefaultDestinationConfig(True, True, False, True, True)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem

        Dim strSubject As String = String.Empty
        Dim strBody As String = String.Empty

        Dim sType As String = _oNotificationTask.Parameters.Split("@)")(0)
        Dim idEmployeeDoc As Integer = _oNotificationTask.Key1Numeric
        Dim idForecast As Integer = _oNotificationTask.key5Numeric
        Dim idDocTemplate As Integer = _oNotificationTask.key2Numeric
        Dim bSendNotification As Boolean = True
        Try
            If idEmployeeDoc > 0 AndAlso idForecast > 0 AndAlso idDocTemplate > 0 Then
                Dim oDocumentManager As New roDocumentManager(New roDocumentState(-1))
                Dim oDocument As roDocument = New roDocument
                Dim oDocAlerts As New DTOs.DocumentAlerts

                Dim iDaysInAdvance As Integer = 2
                ' Recuperamos días de antelación ...
                Try
                    Dim param = roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "Documents.DaysInAdvanceForForecastAdvice"))
                    If param <> "" AndAlso roTypes.Any2Integer(param) >= 0 Then
                        iDaysInAdvance = roTypes.Any2Integer(param)
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roDocumentPending::Translate::Error recovering days in advance for notification for employee " & idEmployeeDoc.ToString & " and forecastid " & idForecast.ToString & " and templateid " & idDocTemplate.ToString & ". Assuming 2 days ...")
                End Try

                oDocAlerts = oDocumentManager.GetDocumentationFaultAlerts(idEmployeeDoc, DTOs.DocumentType.Employee, idForecast, False, Now.Date.AddDays(iDaysInAdvance), ForecastType.Any)
                Dim oForecastAlertDocs As New Generic.List(Of DocumentAlert)
                oForecastAlertDocs.AddRange(oDocAlerts.AbsenteeismDocuments)
                oForecastAlertDocs.AddRange(oDocAlerts.WorkForecastDocuments)
                Dim oForecastAlertDocsResult As New Generic.List(Of DocumentAlert)

                Select Case sType
                    Case "DAYS"
                        oForecastAlertDocsResult = oForecastAlertDocs.FindAll(Function(x) x.IDDocumentTemplate = idDocTemplate AndAlso x.IdDaysAbsence > 0 AndAlso x.IDDocument = 0)
                    Case "HOURS"
                        oForecastAlertDocsResult = oForecastAlertDocs.FindAll(Function(x) x.IDDocumentTemplate = idDocTemplate AndAlso x.IdHoursAbsence > 0 AndAlso x.IDDocument = 0)
                    Case "REQUEST"
                        oForecastAlertDocsResult = oForecastAlertDocs.FindAll(Function(x) x.IDDocumentTemplate = idDocTemplate AndAlso x.IdRequest > 0 AndAlso x.IDDocument = 0)
                    Case "OVERTIME"
                        oForecastAlertDocsResult = oForecastAlertDocs.FindAll(Function(x) x.IDDocumentTemplate = idDocTemplate AndAlso x.IdOvertimeForecast > 0 AndAlso x.IDDocument = 0)
                End Select

                If oForecastAlertDocsResult.Count = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationCore::roDocumentPending::Translate::Document required alert does not longer apply for employee " & idEmployeeDoc.ToString & " and forecastid " & idForecast.ToString & " and templateid " & idDocTemplate.ToString & ". Skipping ...")
                    strSubject = String.Empty
                    bSendNotification = False
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roDocumentPending::Translate::Error checking if alert is still valid. Assuming it is.")
        End Try

        strSubject = roNotificationHelper.Message("Notification.DocumentNextToBeRequired.Subject", Nothing, , oConf.Language)
        Dim Params As New ArrayList
        Dim sDocName As String = _oNotificationTask.Parameters.Split("@)")(1)
        Params.Add(sDocName)
        Params.Add(roTypes.Any2DateTime(_oNotificationTask.key3Datetime.Value).Date)

        ' Si la notificación ya no aplica, no construyo mensaje y salgo (se marcará como ejecutada)
        If Not bSendNotification Then Return Nothing

        If Not bCheckRepetition Then
            ' Si no hay control, o es el primer mensaje, textos normales
            If DateDiff(DateInterval.Day, Now.Date, roTypes.Any2DateTime(oConf.Language).Date) > 0 Then
                strSubject = roNotificationHelper.Message("Notification.DocumentNextToBeRequired.Subject", Nothing, , oConf.Language)
                strBody = roNotificationHelper.Message("Notification.DocumentNextToBeRequired.Body", Params, , oConf.Language)
            ElseIf DateDiff(DateInterval.Day, Now.Date, roTypes.Any2DateTime(oConf.Language).Date) = 0 Then
                strSubject = roNotificationHelper.Message("Notification.DocumentNextToBeRequired_Today.Subject", Nothing, , oConf.Language)
                strBody = roNotificationHelper.Message("Notification.DocumentNextToBeRequired_Today.Body", Params, , oConf.Language)
            Else
                strSubject = roNotificationHelper.Message("Notification.DocumentNextToBeRequired_Past.Subject", Nothing, , oConf.Language)
                strBody = roNotificationHelper.Message("Notification.DocumentNextToBeRequired_Past.Body", Params, , oConf.Language)
            End If
        Else
            ' Detalles de la ausencia
            Try
                Select Case sType
                    Case "DAYS"
                        Dim oProgAbsenceState As New VTBusiness.Absence.roProgrammedAbsenceState()
                        Dim oProgAbsence As New VTBusiness.Absence.roProgrammedAbsence(idForecast, oProgAbsenceState)
                        Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgAbsence.IDCause))
                        Params.Add(roTypes.Any2DateTime(oProgAbsence.BeginDate).Date)
                        If oProgAbsence.FinishDate Is Nothing Then
                            Params.Add(roTypes.Any2DateTime(oProgAbsence.BeginDate).Date.AddDays(oProgAbsence.MaxLastingDays))
                        Else
                            Params.Add(roTypes.Any2DateTime(oProgAbsence.FinishDate).Date)
                        End If
                    Case "HOURS"
                        Dim oProgCauseState As New VTBusiness.Incidence.roProgrammedCauseState()
                        Dim oProgCause As New VTBusiness.Incidence.roProgrammedCause(idForecast, oProgCauseState)
                        Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgCause.IDCause))
                        Params.Add(roTypes.Any2DateTime(oProgCause.ProgrammedDate).Date)
                        Params.Add(roTypes.Any2DateTime(oProgCause.ProgrammedDate).Date)
                    Case "REQUEST"
                        Dim oRequestState As New roRequestState()
                        Dim oRequest As New roRequest
                        oRequest.ID = idForecast
                        oRequest.Load()
                        Params.Add(AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oRequest.IDCause))
                        Params.Add(roTypes.Any2DateTime(oRequest.Date1).Date)
                        Params.Add(roTypes.Any2DateTime(oRequest.Date2).Date)
                    Case Else
                        ' No debería pasar, pero si pasa añado 3 parámetros vacíos para evitar que el mensaje se componga con parámetros donde no toca
                        Params.Add("?")
                        Params.Add("?")
                        Params.Add("?")
                End Select

                If _oNotificationTask.Repetition < iMaxRepetition - 1 Then
                    ' Si es la última, mensaje especial
                    Params.Add(dNextRepetition)
                    Params.Add(roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric)))
                    strSubject = roNotificationHelper.Message("Notification.DocumentNextToBeRequired_RecurrentAdvice.Subject", Nothing, , oConf.Language)
                    strBody = roNotificationHelper.Message("Notification.DocumentNextToBeRequired_RecurrentAdvice.Body", Params, , oConf.Language)
                Else
                    ' Si es la última, mensaje especial
                    Params.Add(roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric)))
                    strSubject = roNotificationHelper.Message("Notification.DocumentNextToBeRequired_LastAdvice.Subject", Nothing, , oConf.Language)
                    strBody = roNotificationHelper.Message("Notification.DocumentNextToBeRequired_LastAdvice.Body", Params, , oConf.Language)
                End If
            Catch ex As Exception
                ' No debiera pasar. Sería porque no existiese la previsión con el id de key5numeric, pero de ser así, se hubiese descartado la notifiación al validar si aplica ...
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roDocumentPending::Translate::Exception composing message about document required for employee " & idEmployeeDoc.ToString & " and forecastid " & idForecast.ToString & " and templateid " & idDocTemplate.ToString & ". No message will be sent. Skipping ...")
            End Try
        End If

        Dim oItem As New roNotificationItem

        oItem.Type = NotificationItemType.email
        oItem.Destination = oConf.Email
        oItem.CompanyId = RoAzureSupport.GetCompanyName()
        oItem.Subject = strSubject
        oItem.Body = strBody
        oItem.Content = String.Empty

        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean
        Dim bRet As Boolean = True
        Try
            ' APV: Control de repeticiones si aplica con detalle (sólo para ausencias)

            If bCheckRepetition Then

                AccessHelper.ExecuteSql("@UPDATE# sysroNotificationTasks SET Repetition = " & _oNotificationTask.Repetition + 1 & " WHERE ID=" & _oNotificationTask.Id)

                If _oNotificationTask.Repetition < iMaxRepetition - 1 Then
                    AccessHelper.ExecuteSql("@UPDATE# sysroNotificationTasks SET NextRepetition = " & roTypes.Any2Time(dNextRepetition.Date).SQLSmallDateTime & " WHERE ID=" & _oNotificationTask.Id)
                Else
                    AccessHelper.ExecuteSql("@UPDATE# sysroNotificationTasks SET NextRepetition = " & roTypes.Any2Time(New Date(2079, 1, 1)).SQLSmallDateTime & " WHERE ID=" & _oNotificationTask.Id)
                End If
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roDocumentPending::PostSendAction::", ex)
            bRet = False
        End Try

        Return bRet
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Return True
    End Function

End Class