Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_HoursAbsenceBreach_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.HoursAbsenceBreach, sGUID)
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
        Dim strSubject As String = String.Empty
        Dim strBody As String = String.Empty
        Dim Params As New ArrayList
        Dim sParameters As String = String.Empty
        Dim iMinMinutes As Integer = 0
        Dim iMaxMinutes As Integer = 0
        Dim iJustifiedMinutes As Integer = 0
        Dim idCause As Integer = 0
        Dim sForecastDate As String = String.Empty

        Try
            Dim eControlType As DTOs.eBreachControlType = eBreachControlType.Both
            sParameters = _oNotificationTask.Parameters
            If sParameters.Split("/").Count = 2 Then
                iMinMinutes = Math.Ceiling(roTypes.Any2Double(sParameters.Split("/")(0)))
                iMaxMinutes = Math.Ceiling(roTypes.Any2Double(sParameters.Split("/")(1)))
            End If
            If iMinMinutes = -1 Then
                eControlType = eBreachControlType.OnlyMaximumExceeded
            ElseIf iMaxMinutes = -1 Then
                eControlType = eBreachControlType.OnlyMinimumNotReached
            Else
                eControlType = eBreachControlType.Both
            End If

            iJustifiedMinutes = roTypes.Any2Integer(_oNotificationTask.key5Numeric)
            idCause = roTypes.Any2Integer(_oNotificationTask.key2Numeric)
            Dim sCauseName As String = String.Empty
            sCauseName = AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & idCause.ToString)
            sForecastDate = Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy")

            Params.Add(roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name from Employees where ID = " & _oNotificationTask.Key1Numeric)))
            Params.Add(roConversions.ConvertMinutesToTime(iJustifiedMinutes))
            Params.Add(sCauseName)
            Params.Add(sForecastDate)

            Select Case eControlType
                Case eBreachControlType.Both
                    Params.Add(roConversions.ConvertMinutesToTime(iMinMinutes))
                    Params.Add(roConversions.ConvertMinutesToTime(iMaxMinutes))
                    strSubject = roNotificationHelper.Message("Notification.HoursAbsenceBreach.Subject", Nothing, , oConf.Language)
                    strBody = roNotificationHelper.Message("Notification.HoursAbsenceBreach.Body", Params, , oConf.Language)
                Case eBreachControlType.OnlyMaximumExceeded
                    Params.Add(roConversions.ConvertMinutesToTime(iMaxMinutes))
                    strSubject = roNotificationHelper.Message("Notification.HoursAbsenceBreach.MaximumExceeded.Subject", Nothing, , oConf.Language)
                    strBody = roNotificationHelper.Message("Notification.HoursAbsenceBreach.MaximumExceeded.Body", Params, , oConf.Language)
                Case eBreachControlType.OnlyMinimumNotReached
                    Params.Add(roConversions.ConvertMinutesToTime(iMinMinutes))
                    strSubject = roNotificationHelper.Message("Notification.HoursAbsenceBreach.MinimumNotReached.Subject", Nothing, , oConf.Language)
                    strBody = roNotificationHelper.Message("Notification.HoursAbsenceBreach.MinimumNotReached.Body", Params, , oConf.Language)
            End Select

            oItem.Type = NotificationItemType.email
            oItem.Destination = oConf.Email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = strSubject
            oItem.Body = strBody
            oItem.Content = String.Empty
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roBreach_Absence::Translate::", ex)
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

            ' Recupero d�as a pasado que hay que validar
            Dim iDays As Long
            iDays = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTLive.Notification.DaysForOvertimeOrHourAbsenceBreach"))
            If iDays > 30 Or iDays <= 0 Then
                iDays = 30
            End If

            Dim sBeginDate As String = String.Empty
            Dim sEndDate As String = String.Empty
            Dim iCourtesyMinutes As Integer = 0
            Dim sCourtesyminutesSQL As String = "0"

            ' Tipo de validación
            Dim mCondition As New roCollection
            mCondition.LoadXMLString(ads("Condition"))

            Dim eControlType As DTOs.eBreachControlType = eBreachControlType.Both

            If mCondition.Exists("BreachControlType") Then
                eControlType = mCondition("BreachControlType")
            End If

            If mCondition.Exists("BreachCourtesy") Then
                iCourtesyMinutes = roTypes.Any2Integer(mCondition("BreachCourtesy"))
                sCourtesyminutesSQL = roTypes.Any2String(iCourtesyMinutes / 60).Replace(",", ".")
            End If

            Dim sTable = "ProgrammedCauses"
            sBeginDate = "ProgrammedCauses.Date"
            sEndDate = "ProgrammedCauses.FinishDate"

            ' Verificamos si existen empleados con previsiones de exceso o ausencias por horas incorrectas sin notificaci�n
            SQL = "@SELECT# dt AS Date, " & sTable & ".IDEmployee," & sTable & ".IDCause,SUM(" & sTable & ".Duration) As Duration, SUM(" & sTable & ".MinDuration) AS MinDuration, " &
                        "ISNULL(AUX.Valor,0) AS ValorJustificado " &
                      "FROM AllDays(" & roTypes.Any2Time(DateTime.Now.Date).Add(-1 * iDays, "d").SQLSmallDateTime & "," & roTypes.Any2Time(DateTime.Now.Date).Add(-1, "d").SQLSmallDateTime & ") AD " &
                        "INNER JOIN " & sTable & " ON AD.dt BETWEEN " & sBeginDate & " AND " & sEndDate & " " &
                        "LEFT JOIN (@SELECT# SUM(Value) Valor, IDEmployee, IDCause, Date as CauseDate FROM DailyCauses GROUP BY IDEmployee, IDCause, Date) AUX ON AUX.IDEmployee = " & sTable & ".IDEmployee AND AUX.CauseDate = dt AND AUX.IDCause = " & sTable & ".IDCause " &
                      "WHERE ("
            Select Case eControlType
                Case eBreachControlType.Both
                    SQL += "ISNULL(Aux.Valor, 0) < (" & sTable & ".MinDuration - " & sCourtesyminutesSQL & ") Or ISNULL(Aux.Valor, 0) > (" & sTable & ".Duration + " & sCourtesyminutesSQL & ")"
                Case eBreachControlType.OnlyMaximumExceeded
                    SQL += "ISNULL(Aux.Valor, 0) > (" & sTable & ".Duration + " & sCourtesyminutesSQL & ")"
                Case eBreachControlType.OnlyMinimumNotReached
                    SQL += "ISNULL(Aux.Valor, 0) < (" & sTable & ".MinDuration - " & sCourtesyminutesSQL & ")"
            End Select
            SQL += ") " &
                       " GROUP BY dt, " & sTable & ".IDEmployee," & sTable & ".IDCause,ISNULL(AUX.Valor,0)"

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado
                For Each dr As DataRow In dt.Rows
                    Dim sFiredDate As String
                    If roTypes.Any2Boolean(ads("ShowOnDesktop")) = True Then
                        sFiredDate = roTypes.Any2Time(DateTime.Now).SQLDateTime
                    Else
                        sFiredDate = "NULL"
                    End If

                    Dim dJustificado As Double
                    dJustificado = roTypes.Any2Double(dr("ValorJustificado") * 60)

                    Dim sParameters As String = ""
                    Select Case eControlType
                        Case eBreachControlType.Both
                            sParameters = roTypes.Any2String(roTypes.Any2Double(dr("MinDuration") * 60)) & "/" & roTypes.Any2String(roTypes.Any2Double(dr("Duration") * 60))
                        Case eBreachControlType.OnlyMaximumExceeded
                            sParameters = "-1/" & roTypes.Any2String(roTypes.Any2Double(dr("Duration") * 60))
                        Case eBreachControlType.OnlyMinimumNotReached
                            sParameters = roTypes.Any2String(roTypes.Any2Double(dr("MinDuration") * 60)) & "/-1"
                    End Select

                    SQL = "IF NOT EXISTS (@SELECT# 1 FROM [dbo].[sysroNotificationTasks] WHERE sysroNotificationTasks.Key1Numeric = " & dr("IDEmployee") &
                                 " AND sysroNotificationTasks.Key3DateTime = " & roTypes.Any2Time(dr("Date")).SQLDateTime &
                                 " AND sysroNotificationTasks.Key2Numeric = " & dr("IDCause") &
                                 " AND sysroNotificationTasks.Key5Numeric = " & dJustificado &
                                 " AND CONVERT(VARCHAR,sysroNotificationTasks.Parameters) = '" & sParameters & "'" &
                                 " AND sysroNotificationTasks.IDNotification = " & roTypes.Any2Double(ads("ID")) &
                                 " ) " &
                              "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime, Key5Numeric, FiredDate, Parameters) VALUES (" &
                              roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & dr("IDCause") & "," & roTypes.Any2Time(dr("Date")).SQLDateTime & "," & dJustificado & "," & sFiredDate & ",'" & sParameters & "')"

                    bRet = bRet AndAlso AccessHelper.ExecuteSql(SQL)
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roBreach_Absence::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

End Class