Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_ConceptNotReached_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Concept_Not_Reached, sGUID)
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
                Format$(_oNotificationTask.key3Datetime, "dd/MM/yyyy"),
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM Concepts where ID = " & _oNotificationTask.key2Numeric))
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()
            oItem.Subject = roNotificationHelper.Message("Notification.ConceptNotReached.Subject", Nothing, , oConf.Language)
            oItem.Body = roNotificationHelper.Message("Notification.ConceptNotReached.Body", Params, , oConf.Language)
            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roCardNotAssigned::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        '
        ' Empleados con incumplimiento de saldo
        '
        Dim SQL As String
        Dim mCondition As New roCollection
        Dim EndShiftTime As String
        Dim i As Integer
        Dim sDate As String
        Dim bRet As Boolean = True

        Try
            mCondition.LoadXMLString(ads("Condition"))

            ' Solo realizamos el cálculo con los empleados que actualmente estan ausentes
            SQL = "@SELECT#  IDEmployee " &
                        " FROM EmployeeStatus  " &
                        " WHERE IsPresent=0 " &
                        " ORDER BY IDEmployee  "

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada empleado, verificamos el incumplimiento de saldo
                ' en ese caso necesario generamos una notificaci�n
                For Each dr As DataRow In dt.Rows
                    For i = 1 To 2
                        ' Obtenemos la fecha/hora de fin de jornada del horario
                        If i = 1 Then
                            ' planificado para HOY
                            sDate = DateTime.Now.Date 'Añadimos el .date porqué la columna Date de DailySchedule no contempla el tiempo (h,min,sec)
                            EndShiftTime = GetEndShiftTime(dr("IDEmployee"), sDate)
                        Else
                            ' planificado para AYER
                            sDate = DateAdd("d", -1, DateTime.Now.Date) 'Añadimos el .date porqué la columna Date de DailySchedule no contempla el tiempo (h,min,sec)
                            EndShiftTime = GetEndShiftTime(dr("IDEmployee"), sDate)
                        End If

                        If IsDate(EndShiftTime) Then
                            ' Si el dia esta calculado
                            SQL = "@SELECT# count(*) as Total from DailySchedule WHERE IDEmployee = " & roTypes.Any2Double(dr("IDEmployee"))
                            SQL = SQL & " AND date = " & roTypes.Any2Time(sDate).SQLSmallDateTime
                            SQL = SQL & " AND Status >= 70"
                            If roTypes.Any2Double(AccessHelper.ExecuteScalar(SQL)) > 0 Then
                                If roTypes.Any2Time(EndShiftTime).NumericValue < roTypes.Any2Time(Now).NumericValue Then
                                    ' Si el fin de jornada es anterior a la hora actual, revisamos el incumplimiento de saldo
                                    If Not roNotificationHelper.IsEmployeeValidConcept(roTypes.Any2Long(dr("IDEmployee")), sDate, mCondition) Then
                                        SQL = "@SELECT# count(*) as Total from sysroNotificationtasks WHERE IDNotification = " & roTypes.Any2Double(ads("ID"))
                                        SQL = SQL & " AND Key1Numeric = " & dr("IDEmployee")
                                        SQL = SQL & " AND Key2Numeric = " & roTypes.Any2Double(mCondition("IDConcept"))
                                        SQL = SQL & " AND Key3DateTime = " & roTypes.Any2Time(sDate).SQLDateTime
                                        If roTypes.Any2Double(AccessHelper.ExecuteScalar(SQL)) = 0 Then
                                            SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key2Numeric, Key3DateTime) VALUES (" &
                                                roTypes.Any2Double(ads("ID")) & "," & dr("IDEmployee") & "," & roTypes.Any2Double(mCondition("IDConcept")) & "," & roTypes.Any2Time(sDate).SQLDateTime & ")"
                                            bRet = (bRet AndAlso AccessHelper.ExecuteSql(SQL))
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next i
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::ConceptNotReached::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

    Private Function GetEndShiftTime(ByVal IDEmployee As Double, ByVal sDate As String) As String
        Dim SQL As String
        Dim IDShift As Double

        Dim mDefinition As New roCollection
        Dim BeginPeriod As New roTime
        Dim FloatingTime As Double
        Dim StartFloating As Double
        Dim EndLimit As New roTime

        Try

            GetEndShiftTime = ""

            ' Obtenemos el horario utilizado
            SQL = "@SELECT# IDShiftUsed, StartShiftUsed FROM DailySchedule WHERE IDEmployee= " & IDEmployee & " AND Date=" & roTypes.Any2Time(roTypes.Any2Time(sDate).DateOnly).SQLSmallDateTime & " and Status >= 40 "
            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                IDShift = roTypes.Any2Double(dt.Rows(0)("IDShiftUsed"))
                FloatingTime = roTypes.Any2Time(dt.Rows(0)("StartShiftUsed")).NumericValue
            End If

            If IDShift = 0 Then
                GetEndShiftTime = ""
                Exit Function
            End If

            ' Obtenemos el final del horario utilizado
            SQL = "@SELECT# StartLimit, EndLimit, IsFloating, StartFloating FROM Shifts WHERE ID = " & IDShift
            dt = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                If roTypes.Any2Boolean(dt.Rows(0)("IsFloating")) Then
                    StartFloating = roTypes.Any2Time(dt.Rows(0)("StartFloating")).NumericValue
                End If
                EndLimit = roTypes.Any2Time(dt.Rows(0)("EndLimit"))
            End If

            ' Obtenemos tiempo a desplazar del horario a calcular
            FloatingTime = FloatingTime - StartFloating

            ' Calculamos el final del horario
            GetEndShiftTime = roTypes.Any2Time(roTypes.DateTimeAdd(roTypes.DateTimeAdd(roTypes.Any2Time(sDate).Value, EndLimit.Value), roTypes.Any2Time(FloatingTime).Value)).Value
        Catch ex As Exception
            GetEndShiftTime = ""
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::ConceptNotReached::GetEndShiftTime: Unexpected error: ", ex)
        End Try

    End Function

End Class