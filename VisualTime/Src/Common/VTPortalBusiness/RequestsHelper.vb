Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTDailyRecord
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTNotifications
Imports Robotics.Base.VTRequests
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTPortal

    Public Class RequestsHelper

        Public Shared Function GetPortalEnabled() As eRequestType()
            Return {eRequestType.UserFieldsChange,
                    eRequestType.ForbiddenPunch,
                    eRequestType.JustifyPunch,
                    eRequestType.ExternalWorkResumePart,
                    eRequestType.ChangeShift,
                    eRequestType.VacationsOrPermissions,
                    eRequestType.PlannedAbsences,
                    eRequestType.PlannedCauses,
                    eRequestType.ExchangeShiftBetweenEmployees,
                    eRequestType.ForbiddenTaskPunch,
                    eRequestType.CancelHolidays,
                    eRequestType.ForgottenCostCenterPunch,
                    eRequestType.PlannedHolidays,
                    eRequestType.PlannedOvertimes,
                    eRequestType.ExternalWorkWeekResume,
                    eRequestType.Telecommute,
                    eRequestType.DailyRecord}
        End Function

        Public Shared Function BuildTeleworkingFilter(ByVal dateStart As Date) As String
            Dim strResult As String = ""
            Try
                strResult = "(" & roTypes.SQLDateTime(dateStart) & " >= DATEADD(day, DATEDIFF(Day, 0, Date1), 0) AND " & roTypes.SQLDateTime(dateStart) & " <= Date2  " &
                    " OR Date1 = " & roTypes.SQLDateTime(dateStart) & ") AND "
                Dim strFilter As String = "0*1*2|4*15"
                If strFilter <> "" Then
                    Dim arrFilter() As String = strFilter.Split("|")
                    Dim arrStatus() As String = arrFilter(0).Split("*")
                    Dim arrRequestType() As String = arrFilter(1).Split("*")

                    If arrFilter(0).ToString.Trim <> "" Then
                        If arrStatus.Length > 0 Then strResult &= "Status IN("
                        For Each strStat As String In arrStatus
                            strResult &= strStat & ","
                        Next
                        If arrStatus.Length > 0 Then strResult = strResult.Substring(0, strResult.Length - 1) & ") AND "
                    End If

                    If arrFilter(1).ToString.Trim <> "" Then
                        If arrRequestType.Length > 0 Then strResult &= "RequestType IN("
                        For Each strRT As String In arrRequestType
                            strResult &= strRT & ","
                        Next
                        If arrRequestType.Length > 0 Then strResult = strResult.Substring(0, strResult.Length - 1) & ") AND "
                    End If

                End If
                If strResult.EndsWith(" AND ") Then strResult = strResult.Substring(0, strResult.Length - 4)
            Catch ex As Exception

                strResult = String.Empty

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::CheckResultFilter")
            End Try

            Return strResult
        End Function

        Public Shared Function CheckResultFilter(ByVal showAll As Boolean, ByVal dateStart As Date, ByVal dateEnd As Date, ByVal filter As String, ByVal dateRequestedStart As Date, ByVal dateRequestedEnd As Date) As String
            Dim strResult As String = ""
            Try
                If showAll Then Return strResult

                If dateStart = Nothing And dateEnd = Nothing Then 'Sense dates
                ElseIf dateStart = Nothing And dateEnd <> Nothing Then 'Data fi
                    strResult = "RequestDate <= " & roTypes.SQLDateTime(dateEnd) & " AND "
                ElseIf dateStart <> Nothing And dateEnd <> Nothing Then 'Data inici i data fi
                    strResult = "RequestDate Between " & roTypes.SQLDateTime(dateStart) & " AND " & roTypes.SQLDateTime(dateEnd) & " AND "
                ElseIf dateStart <> Nothing And dateEnd = Nothing Then ' data inici
                    strResult = "RequestDate >= " & roTypes.SQLDateTime(dateStart) & " AND "
                End If

                If dateRequestedStart = Nothing And dateRequestedEnd = Nothing Then 'Sense dates
                ElseIf dateRequestedStart = Nothing And dateRequestedEnd <> Nothing Then 'Data fi
                    strResult = "IsNull(Date2,IsNull(Date1,RequestDate)) <= " & roTypes.SQLDateTime(dateRequestedEnd) & " AND "
                ElseIf dateRequestedStart <> Nothing And dateRequestedEnd <> Nothing Then 'Data inici i data fi
                    strResult = "(IsNull(Date1,RequestDate) Between " & roTypes.SQLDateTime(dateRequestedStart) & " AND " & roTypes.SQLDateTime(dateRequestedEnd) & " or IsNull(Date2,IsNull(Date1,RequestDate)) Between " & roTypes.SQLDateTime(dateRequestedStart) & " AND " & roTypes.SQLDateTime(dateRequestedEnd) & ") AND "
                ElseIf dateRequestedStart <> Nothing And dateRequestedEnd = Nothing Then ' data inici
                    strResult = "IsNull(Date1,RequestDate) >= " & roTypes.SQLDateTime(dateRequestedStart) & " AND "
                End If

                If filter <> "" Then
                    Dim arrFilter() As String = filter.Split("|")
                    Dim arrStatus() As String = arrFilter(0).Split("*")
                    Dim arrRequestType() As String = arrFilter(1).Split("*")

                    If arrFilter(0).ToString.Trim <> "" Then
                        If arrStatus.Length > 0 Then strResult &= "Status IN("
                        For Each strStat As String In arrStatus
                            strResult &= strStat & ","
                        Next
                        If arrStatus.Length > 0 Then strResult = strResult.Substring(0, strResult.Length - 1) & ") AND "
                    End If

                    If arrFilter(1).ToString.Trim <> "" Then
                        If arrRequestType.Length > 0 Then strResult &= "RequestType IN("
                        For Each strRT As String In arrRequestType
                            strResult &= strRT & ","
                        Next
                        If arrRequestType.Length > 0 Then strResult = strResult.Substring(0, strResult.Length - 1) & ") AND "
                    End If

                End If
                If strResult.EndsWith(" AND ") Then strResult = strResult.Substring(0, strResult.Length - 4)
            Catch ex As Exception

                strResult = String.Empty

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::CheckResultFilter")
            End Try

            Return strResult
        End Function

        Public Shared Function GetOverlappingEmployees(ByVal passport As roPassportTicket, ByVal idRequest As Integer, ByVal oLng As roLanguage, ByRef oReqState As Requests.roRequestState) As DataTable
            Dim lrret As New DataTable
            Try

                'Dim dtOverlappedAbsences As DataTable = Absence.roProgrammedAbsence.GetMyTeamAbsencesBetweenDates(oRequest.IDEmployee, beginDate, endDate, oProgAbsenceState)
                Dim dtOverlappedAbsences As DataTable = Requests.roRequest.GetOverlappingEmployees(idRequest, oReqState)
                lrret = dtOverlappedAbsences
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::GetOverlappingEmployees")
            Finally
            End Try

            Return lrret
        End Function

        Public Shared Function GetRequestForSupervisor(ByVal requestId As Integer, ByVal supervisorId As Integer, ByVal oLng As roLanguage, ByRef oReqState As Requests.roRequestState) As UserRequestForSupervisor
            Dim lrret As New UserRequestForSupervisor

            Try

                Dim hasPermissionOnRequest As Boolean = Requests.roRequest.HasPermissionOverRequest(supervisorId, requestId, oReqState)
                If hasPermissionOnRequest Then
                    Dim oRequest As New Requests.roRequest(requestId, oReqState)
                    If oRequest IsNot Nothing Then
                        lrret.Id = oRequest.ID
                        lrret.IdEmployee = oRequest.IDEmployee
                        If oRequest.IDShift.HasValue Then
                            lrret.IdShift = oRequest.IDShift.Value
                            Dim oShift As New Shift.roShift(oRequest.IDShift.Value, New Shift.roShiftState)
                            If oShift.IDConceptBalance > 0 Then
                                Dim oConcept As New roConcept(oShift.IDConceptBalance, New roConceptState)
                                If Not String.IsNullOrEmpty(oConcept.DefaultQuery) Then
                                    lrret.IsAnnualWork = oConcept.DefaultQuery.ToString.Equals("L")
                                End If
                            End If

                        Else
                            lrret.IdShift = -1
                        End If

                        If oRequest.IDCause.HasValue Then
                            lrret.IdCause = oRequest.IDCause.Value
                        Else
                            lrret.IdCause = -1
                        End If

                        If oRequest.IDEmployeeExchange.HasValue Then
                            lrret.IdEmployeeExchange = oRequest.IDEmployeeExchange.Value
                        Else
                            lrret.IdEmployeeExchange = -1
                        End If

                        If oRequest.IDTask1.HasValue Then
                            lrret.IdTask = oRequest.IDTask1.Value
                        Else
                            lrret.IdTask = -1
                        End If

                        If oRequest.IDTask2.HasValue Then
                            lrret.IdTask2 = oRequest.IDTask2.Value
                        Else
                            lrret.IdTask2 = -1
                        End If

                        If oRequest.CompletedTask.HasValue Then
                            lrret.CompletedTask = oRequest.CompletedTask.Value
                        Else
                            lrret.CompletedTask = False
                        End If

                        If oRequest.IDCenter.HasValue Then
                            lrret.IdCenter = oRequest.IDCenter.Value
                        Else
                            lrret.IdCenter = -1
                        End If

                        If oRequest.FieldName <> "" Then
                            Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState()
                            roBusinessState.CopyTo(oReqState, oUserFieldState)
                            Dim usrField As New VTUserFields.UserFields.roUserField(oUserFieldState, oRequest.FieldName, Types.EmployeeField, False)
                            lrret.FieldName = usrField.FieldName & "__" & usrField.FieldType & "__" & usrField.History
                        End If

                        lrret.FieldValue = oRequest.FieldValue

                        lrret.RequestType = CInt(oRequest.RequestType)
                        lrret.Date1 = If(oRequest.Date1.HasValue, oRequest.Date1.Value, New Date(1970, 1, 1))
                        lrret.Date2 = If(oRequest.Date2.HasValue, oRequest.Date2.Value, New Date(1970, 1, 1))
                        lrret.strDate1 = oRequest.strDate1
                        lrret.strDate2 = oRequest.strDate2
                        lrret.NotReaded = oRequest.NotReaded

                        lrret.strFromTime = If(oRequest.FromTime IsNot Nothing, oRequest.FromTime.Value.AddYears(100).ToString("yyyy-MM-dd HH:mm"), "") 'oRequest.strFromTime
                        lrret.strHours = oRequest.strHours
                        lrret.strStartShift = oRequest.strStartShift
                        lrret.strToTime = If(oRequest.ToTime IsNot Nothing, oRequest.ToTime.Value.AddYears(100).ToString("yyyy-MM-dd HH:mm"), "") 'oRequest.strToTime
                        lrret.ReqStatus = oRequest.RequestStatus

                        If lrret.ReqStatus = 2 Then
                            Select Case oRequest.RequestType
                                Case eRequestType.PlannedAbsences
                                    lrret.AbsenceId = Requests.roRequest.GetProgrammedAbsencebyIdRequest(requestId)
                                Case eRequestType.PlannedCauses
                                    lrret.AbsenceId = Requests.roRequest.GetProgrammedCausebyIdRequest(requestId)
                                Case eRequestType.PlannedOvertimes
                                    lrret.AbsenceId = Requests.roRequest.GetProgrammedOvertimebyIdRequest(requestId)
                            End Select
                        End If

                        lrret.Comments = roTypes.Any2String(oRequest.Comments)

                        lrret.RequestInfo = oRequest.RequestInfo(True)

                        lrret.Field1 = oRequest.Field1
                        lrret.Field2 = oRequest.Field2
                        lrret.Field3 = oRequest.Field3
                        lrret.Field4 = oRequest.Field4
                        lrret.Field5 = oRequest.Field5
                        lrret.Field6 = oRequest.Field6

                        If oRequest.RequestDays IsNot Nothing AndAlso oRequest.RequestDays.Count > 0 Then
                            Dim otmplist As New Generic.List(Of RequestDays)

                            For Each oDay As Requests.roRequestDay In oRequest.RequestDays
                                Dim oNewDay As New RequestDays
                                oNewDay.RequestDate = oDay.RequestDate
                                oNewDay.AllDay = If(oDay.AllDay.HasValue, oDay.AllDay.Value, False)
                                oNewDay.Duration = If(oDay.Duration.HasValue, oDay.Duration.Value, 0)
                                If oDay.FromTime.HasValue Then oNewDay.FromTime = oDay.FromTime.Value.AddYears(100)
                                If oDay.ToTime.HasValue Then oNewDay.ToTime = oDay.ToTime.Value.AddYears(100)

                                If oDay.IDCause.HasValue Then
                                    oNewDay.IdCause = oDay.IDCause
                                Else
                                    oNewDay.IdCause = -1
                                End If

                                If (oDay.ActualType.HasValue) Then
                                    oNewDay.ActualType = CInt(oDay.ActualType.Value)
                                End If

                                oNewDay.Remark = oDay.Comments

                                otmplist.Add(oNewDay)
                            Next

                            lrret.RequestDays = otmplist.ToArray
                        End If

                        Dim approvalHistory As New Generic.List(Of RequestHistoryEntry)

                        For Each oaproval As Requests.roRequestApproval In oRequest.RequestApprovals
                            Dim rhEntry As New RequestHistoryEntry
                            rhEntry.ActionDate = oaproval.ApprovalDateTime
                            rhEntry.Action = roTypes.Any2Integer(oaproval.Status)
                            rhEntry.Comment = oaproval.Comments

                            Dim oTmpUser As roPassportTicket = roPassportManager.GetPassportTicket(oaproval.IDPassport)

                            If oTmpUser IsNot Nothing Then
                                rhEntry.User = oTmpUser.Name
                            Else

                                rhEntry.User = oLng.Translate("Requests.PassportDeleted", "RequestInfo")
                            End If

                            approvalHistory.Add(rhEntry)
                        Next

                        lrret.RequestsHistoryEntries = approvalHistory.ToArray()
                        lrret.Status = ErrorCodes.OK
                    Else
                        lrret.Status = ErrorCodes.NOT_FOUND
                    End If
                Else

                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.NOT_FOUND

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::GetRequestForSupervisor")
            End Try

            Return lrret
        End Function

        Public Shared Function GetRequestByEmployee(ByVal requestId As Integer, ByVal employeeId As Integer, ByVal oLng As roLanguage, ByRef oReqState As Requests.roRequestState) As UserRequest
            Dim lrret As New UserRequest

            Try
                Dim oRequest As New Requests.roRequest(requestId, oReqState)

                If oRequest IsNot Nothing Then

                    If (oRequest.IDEmployee <> employeeId) AndAlso Not (oRequest.RequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso oRequest.IDEmployeeExchange = employeeId) Then
                        lrret.Status = ErrorCodes.NOT_FOUND
                    Else
                        lrret.Id = oRequest.ID
                        lrret.IdEmployee = oRequest.IDEmployee
                        If oRequest.IDShift.HasValue Then
                            lrret.IdShift = oRequest.IDShift.Value
                        Else
                            lrret.IdShift = -1
                        End If

                        If oRequest.IDCause.HasValue Then
                            lrret.IdCause = oRequest.IDCause.Value
                        Else
                            lrret.IdCause = -1
                        End If

                        If oRequest.IDEmployeeExchange.HasValue Then
                            lrret.IdEmployeeExchange = oRequest.IDEmployeeExchange.Value
                        Else
                            lrret.IdEmployeeExchange = -1
                        End If

                        If oRequest.IDTask1.HasValue Then
                            lrret.IdTask = oRequest.IDTask1.Value
                        Else
                            lrret.IdTask = -1
                        End If

                        If oRequest.IDTask2.HasValue Then
                            lrret.IdTask2 = oRequest.IDTask2.Value
                        Else
                            lrret.IdTask2 = -1
                        End If

                        If oRequest.CompletedTask.HasValue Then
                            lrret.CompletedTask = oRequest.CompletedTask.Value
                        Else
                            lrret.CompletedTask = False
                        End If

                        If oRequest.IDCenter.HasValue Then
                            lrret.IdCenter = oRequest.IDCenter.Value
                        Else
                            lrret.IdCenter = -1
                        End If

                        If oRequest.FieldName <> "" Then
                            Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState()
                            roBusinessState.CopyTo(oReqState, oUserFieldState)
                            Dim usrField As New VTUserFields.UserFields.roUserField(oUserFieldState, oRequest.FieldName, Types.EmployeeField, False)
                            lrret.FieldName = usrField.FieldName & "__" & usrField.FieldType & "__" & usrField.History
                        End If

                        lrret.FieldValue = oRequest.FieldValue

                        lrret.RequestType = CInt(oRequest.RequestType)
                        lrret.Date1 = If(oRequest.Date1.HasValue, oRequest.Date1.Value, New Date(1970, 1, 1))
                        lrret.Date2 = If(oRequest.Date2.HasValue, oRequest.Date2.Value, New Date(1970, 1, 1))
                        lrret.strDate1 = oRequest.strDate1
                        lrret.strDate2 = oRequest.strDate2
                        lrret.NotReaded = oRequest.NotReaded

                        lrret.strFromTime = If(oRequest.FromTime IsNot Nothing, oRequest.FromTime.Value.AddYears(100).ToString("yyyy-MM-dd HH:mm"), "") 'oRequest.strFromTime
                        lrret.strHours = oRequest.strHours
                        lrret.strStartShift = oRequest.strStartShift
                        lrret.strToTime = If(oRequest.ToTime IsNot Nothing, oRequest.ToTime.Value.AddYears(100).ToString("yyyy-MM-dd HH:mm"), "") 'oRequest.strToTime
                        lrret.ReqStatus = oRequest.RequestStatus

                        If lrret.ReqStatus = 2 Then
                            Select Case oRequest.RequestType
                                Case eRequestType.PlannedAbsences
                                    lrret.AbsenceId = Requests.roRequest.GetProgrammedAbsencebyIdRequest(requestId)
                                Case eRequestType.PlannedCauses
                                    lrret.AbsenceId = Requests.roRequest.GetProgrammedCausebyIdRequest(requestId)
                                Case eRequestType.PlannedOvertimes
                                    lrret.AbsenceId = Requests.roRequest.GetProgrammedOvertimebyIdRequest(requestId)
                            End Select
                        End If

                        lrret.Comments = roTypes.Any2String(oRequest.Comments)

                        lrret.Field1 = oRequest.Field1
                        lrret.Field2 = oRequest.Field2
                        lrret.Field3 = oRequest.Field3
                        lrret.Field4 = oRequest.Field4
                        lrret.Field5 = oRequest.Field5
                        lrret.Field6 = oRequest.Field6

                        If oRequest.RequestDays IsNot Nothing AndAlso oRequest.RequestDays.Count > 0 Then
                            Dim otmplist As New Generic.List(Of RequestDays)

                            For Each oDay As Requests.roRequestDay In oRequest.RequestDays
                                Dim oNewDay As New RequestDays
                                oNewDay.RequestDate = oDay.RequestDate
                                oNewDay.AllDay = If(oDay.AllDay.HasValue, oDay.AllDay.Value, False)
                                oNewDay.Duration = If(oDay.Duration.HasValue, oDay.Duration.Value, 0)
                                If oDay.FromTime.HasValue Then oNewDay.FromTime = oDay.FromTime.Value.AddYears(100)
                                If oDay.ToTime.HasValue Then oNewDay.ToTime = oDay.ToTime.Value.AddYears(100)

                                If oDay.IDCause.HasValue Then
                                    oNewDay.IdCause = oDay.IDCause
                                Else
                                    oNewDay.IdCause = -1
                                End If

                                If (oDay.ActualType.HasValue) Then
                                    oNewDay.ActualType = CInt(oDay.ActualType.Value)
                                End If

                                oNewDay.Remark = oDay.Comments

                                otmplist.Add(oNewDay)
                            Next

                            lrret.RequestDays = otmplist.ToArray
                        End If

                        Dim approvalHistory As New Generic.List(Of RequestHistoryEntry)

                        For Each oaproval As Requests.roRequestApproval In oRequest.RequestApprovals
                            Dim rhEntry As New RequestHistoryEntry
                            rhEntry.ActionDate = oaproval.ApprovalDateTime
                            rhEntry.Action = roTypes.Any2Integer(oaproval.Status)
                            rhEntry.Comment = oaproval.Comments

                            Dim oTmpUser As roPassportTicket = roPassportManager.GetPassportTicket(oaproval.IDPassport)

                            If oTmpUser IsNot Nothing Then
                                rhEntry.User = oTmpUser.Name
                            Else

                                rhEntry.User = oLng.Translate("Requests.PassportDeleted", "RequestInfo")
                            End If

                            approvalHistory.Add(rhEntry)
                        Next

                        lrret.RequestsHistoryEntries = approvalHistory.ToArray()
                        lrret.Status = ErrorCodes.OK
                    End If
                Else
                    lrret.Status = ErrorCodes.NOT_FOUND
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.NOT_FOUND

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::GetRequestByEmployee")
            End Try

            Return lrret
        End Function

        Public Shared Function SetNotificationReaded(ByVal notificationId As Integer, ByVal employeeId As Integer, ByRef oReqState As Requests.roRequestState) As StdResponse
            Dim lrret As New StdResponse

            Try
                lrret.Status = ErrorCodes.OK

                Dim strSql As String = "@UPDATE# sysroNotificationTasks SET IsReaded = 1 WHERE ID = " & notificationId

                lrret.Result = DataLayer.AccessHelper.ExecuteSql(strSql)
            Catch ex As Exception
                lrret.Status = ErrorCodes.NOT_FOUND

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SetNotificationReaded")
            End Try
            Return lrret
        End Function

        Public Shared Function SetRequestAsReaded(ByVal requestId As Integer, ByVal employeeId As Integer, ByRef oReqState As Requests.roRequestState) As StdResponse
            Dim lrret As New StdResponse

            Try
                Dim oRequest As New Requests.roRequest(requestId, oReqState)

                If oRequest IsNot Nothing Then

                    lrret.Result = True

                    If oRequest.NotReaded Then
                        ' Marcamos la solicitud cómo leída
                        oRequest.NotReaded = False
                        If Not oRequest.Save(,,, False) Then lrret.Result = False
                    End If

                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.Status = ErrorCodes.NOT_FOUND
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.NOT_FOUND

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SetRequestAsReaded")
            End Try
            Return lrret
        End Function

        Public Shared Function GetEmployeeTeleWorkingRequestsOnDay(ByVal iDate As DateTime, ByVal passportId As Integer, ByVal employeeId As Integer, ByVal oLng As roLanguage, ByVal oReqLang As roLanguage, ByVal oPortalLang As roLanguage, ByRef oReqState As Requests.roRequestState) As RequestsList
            Dim lrret As New RequestsList

            Try

                Dim strArrStatus() As String = {"pending", "ongoing", "accepted", "denied"}
                Dim strArrRequestType() As String = {"", "UserFieldsChange", "ForbiddenPunch", "JustifyPunch", "ExternalWorkResumePart", "ChangeShift", "VacationsOrPermissions", "PlannedAbsences", "ExchangeShiftBetweenEmployees", "PlannedCauses", "ForbiddenTaskPunch", "CancelHolidays", "ForgottenCostCenterPunch", "PlannedHolidays", "PlannedOvertimes", "ExternalWorkWeekResume", "Telecommute", "DailyRecord"}
                Dim resultFilter As String = VTPortal.RequestsHelper.BuildTeleworkingFilter(iDate)
                Dim dtblQuery As DataTable = Requests.roRequest.GetRequestsByEmployee(employeeId, resultFilter, "RequestDate ASC", oReqState, False, oReqLang)
                Dim curRequestList As New Generic.List(Of EmpRequest)
                If dtblQuery IsNot Nothing Then
                    For Each dRow As DataRow In dtblQuery.Rows
                        Dim curRequest = New EmpRequest
                        Dim strNameRequest As String = ""
                        Dim strIcoStatus As String = "pending"
                        Dim strNameStatus As String = "pending"
                        curRequest.IcoStatus = strArrStatus(dRow("Status"))
                        curRequest.Status = roTypes.Any2Integer(dRow("Status"))
                        curRequest.Name = oLng.Translate("Requests.RequestName." & strArrRequestType(dRow("RequestType")), "Requests")
                        curRequest.NameStatus = oLng.Translate("Requests.RequestStatus." & strArrStatus(dRow("Status")), "Requests")
                        curRequest.IdRequestType = roTypes.Any2Integer(dRow("RequestType"))
                        curRequest.RequestType = strArrRequestType(dRow("RequestType"))
                        curRequest.NotReaded = roTypes.Any2Boolean(dRow("NotReaded"))
                        curRequest.Id = dRow("ID")
                        curRequest.Title = oLng.Translate("Requests.ShowDetail", "Requests")
                        curRequest.Description = roTypes.Any2String(dRow("RequestDetailInfo")) 'VTPortal.RequestsHelper.GetRequestInfo(oReqLang, oPortalLang, dRow, dRow("RequestType"), False, oReqState)
                        curRequestList.Add(curRequest)
                    Next
                End If
                lrret.Requests = curRequestList.ToArray()
                ' Mostramos/ocultamos el botón de nueva solcitud en función de los permisos del empleado
                lrret.CanCreateRequest = False
                Dim RequestTypes() As eRequestType = RequestsHelper.GetPortalEnabled()
                Dim oState As New Requests.roRequestState(passportId)
                For Each oRequestTypeSecurity As eRequestType In RequestTypes
                    If VTPortal.SecurityHelper.GetFeaturePermission(passportId, New Requests.roRequestTypeSecurity(oRequestTypeSecurity, oState).EmployeeFeatureName, "E") >= Permission.Write Then
                        lrret.CanCreateRequest = True
                        Exit For
                    End If
                Next
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                lrret.Requests = {}
                lrret.CanCreateRequest = False

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::GetEmployeeRequests")
            End Try

            Return lrret
        End Function

        Public Shared Function GetSupervisedRequests(ByVal showAll As Boolean, ByVal iDate As DateTime, ByVal eDate As DateTime, ByVal iRequestedDate As DateTime, ByVal eRequestedDate As DateTime, ByVal filter As String, ByVal orderBy As String,
                                                   ByVal supervisorId As Integer, ByVal oLng As roLanguage, ByVal oReqLang As roLanguage, ByVal oPortalLang As roLanguage, ByRef oReqState As Requests.roRequestState) As RequestsList
            Dim lrret As New RequestsList

            Try

                Dim strArrStatus() As String = {"pending", "ongoing", "accepted", "denied"}
                Dim strArrRequestType() As String = {"", "UserFieldsChange", "ForbiddenPunch", "JustifyPunch", "ExternalWorkResumePart", "ChangeShift", "VacationsOrPermissions", "PlannedAbsences", "ExchangeShiftBetweenEmployees", "PlannedCauses", "ForbiddenTaskPunch", "CancelHolidays", "ForgottenCostCenterPunch", "PlannedHolidays", "PlannedOvertimes", "ExternalWorkWeekResume", "Telecommute", "DailyRecord"}
                Dim resultFilter As String = VTPortal.RequestsHelper.CheckResultFilter(showAll, iDate, eDate, filter, iRequestedDate, eRequestedDate)
                Dim statusRequested = filter.Split("|")
                Dim levelsBelow = "1"
                If statusRequested(0) <> "0*1" Then
                    levelsBelow = ""
                End If
                Dim dtblQuery As DataTable = Requests.roRequest.GetRequestsSupervisor(supervisorId, resultFilter, orderBy, 0, levelsBelow, 0, 0, False, oReqState, False)
                Dim curRequestList As New Generic.List(Of EmpRequest)
                Dim oDailyRecord As roDailyRecord = Nothing
                Dim oDailyRecordManager As roDailyRecordManager = Nothing
                Dim oEmpState As Employee.roEmployeeState = Nothing
                Dim oEmployee As Employee.roEmployee
                If dtblQuery IsNot Nothing Then
                    For Each dRow As DataRow In dtblQuery.Rows
                        Dim curRequest = New EmpRequest
                        Dim strNameRequest As String = ""
                        Dim strIcoStatus As String = "pending"
                        Dim strNameStatus As String = "pending"
                        curRequest.IcoStatus = strArrStatus(dRow("Status"))
                        curRequest.Status = roTypes.Any2Integer(dRow("Status"))
                        curRequest.Name = roTypes.Any2String(dRow("EmployeeName"))
                        curRequest.NameStatus = oLng.Translate("Requests.RequestStatus." & strArrStatus(dRow("Status")), "Requests")
                        curRequest.IdRequestType = roTypes.Any2Integer(dRow("RequestType"))
                        curRequest.RequestType = strArrRequestType(dRow("RequestType"))
                        curRequest.NotReaded = roTypes.Any2Boolean(dRow("NotReaded"))
                        curRequest.Id = dRow("ID")
                        curRequest.IdEmployee = roTypes.Any2Integer(dRow("IDEmployee"))

                        curRequest.Title = oLng.Translate("Requests.ShowDetail", "Requests")
                        curRequest.Description = oLng.Translate("Requests.RequestName." & strArrRequestType(dRow("RequestType")), "Requests") & ". " & roTypes.Any2String(dRow("RequestInfo"))

                        ' Declaración de jornada
                        If curRequest.IdRequestType = DTOs.eRequestType.DailyRecord Then
                            If oDailyRecordManager Is Nothing Then oDailyRecordManager = New roDailyRecordManager(New roDailyRecordState(oReqState.IDPassport))
                            If oEmpState Is Nothing Then oEmpState = New VTEmployees.Employee.roEmployeeState(oReqState.IDPassport)
                            oDailyRecord = oDailyRecordManager.LoadDailyRecord(curRequest.Id, Nothing, True)
                            oEmployee = Employee.roEmployee.GetEmployee(curRequest.IdEmployee, oEmpState, False)
                            oDailyRecord.EmployeeImage = EmployeesHelper.LoadEmployeeImage(oEmployee.Image, oEmpState)
                            oDailyRecord.EmployeeGroup = oEmployee.GroupName
                            curRequest.DailyRecord = oDailyRecord
                        End If
                        curRequestList.Add(curRequest)
                    Next
                End If
                lrret.Requests = curRequestList.ToArray()
                ' Mostramos/ocultamos el botón de nueva solcitud en función de los permisos del empleado
                lrret.CanCreateRequest = False
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                lrret.Requests = {}
                lrret.CanCreateRequest = False

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::GetEmployeeRequests")
            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeRequests(ByVal showAll As Boolean, ByVal iDate As DateTime, ByVal eDate As DateTime, ByVal iRequestedDate As DateTime, ByVal eRequestedDate As DateTime, ByVal filter As String, ByVal orderBy As String,
                                                   ByVal passportId As Integer, ByVal employeeId As Integer, ByVal oLng As roLanguage, ByVal oReqLang As roLanguage, ByVal oPortalLang As roLanguage, ByRef oReqState As Requests.roRequestState) As RequestsList
            Dim lrret As New RequestsList

            Try

                ' Verificamos si se debe mostrar el siguiente supervisor
                Dim bolShowNextSupervisor As Boolean = False
                bolShowNextSupervisor = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Robotics.Azure.RoAzureSupport.GetCompanyName(), "VTPortal.ShowNextSupervisor"))

                Dim strArrStatus() As String = {"pending", "ongoing", "accepted", "denied", "canceled"}
                Dim strArrRequestType() As String = {"", "UserFieldsChange", "ForbiddenPunch", "JustifyPunch", "ExternalWorkResumePart", "ChangeShift", "VacationsOrPermissions", "PlannedAbsences", "ExchangeShiftBetweenEmployees", "PlannedCauses", "ForbiddenTaskPunch", "CancelHolidays", "ForgottenCostCenterPunch", "PlannedHolidays", "PlannedOvertimes", "ExternalWorkWeekResume"}
                Dim resultFilter As String = VTPortal.RequestsHelper.CheckResultFilter(showAll, iDate, eDate, filter, iRequestedDate, eRequestedDate)
                Dim dtblQuery As DataTable = Requests.roRequest.GetRequestsByEmployee(employeeId, resultFilter, orderBy, oReqState, bolShowNextSupervisor, oReqLang)
                Dim curRequestList As New Generic.List(Of EmpRequest)
                If dtblQuery IsNot Nothing Then
                    For Each dRow As DataRow In dtblQuery.Rows
                        Dim curRequest = New EmpRequest
                        Dim strNameRequest As String = ""
                        Dim strIcoStatus As String = "pending"
                        Dim strNameStatus As String = "pending"
                        curRequest.IcoStatus = strArrStatus(dRow("Status"))
                        curRequest.Status = roTypes.Any2Integer(dRow("Status"))
                        curRequest.Name = oLng.Translate("Requests.RequestName." & strArrRequestType(dRow("RequestType")), "Requests")
                        curRequest.NameStatus = oLng.Translate("Requests.RequestStatus." & strArrStatus(dRow("Status")), "Requests")
                        curRequest.IdRequestType = roTypes.Any2Integer(dRow("RequestType"))
                        curRequest.RequestType = strArrRequestType(dRow("RequestType"))
                        curRequest.NotReaded = roTypes.Any2Boolean(dRow("NotReaded"))
                        curRequest.Id = dRow("ID")
                        curRequest.Title = oLng.Translate("Requests.ShowDetail", "Requests")
                        curRequest.Description = roTypes.Any2String(dRow("RequestDetailInfo")) 'VTPortal.RequestsHelper.GetRequestInfo(oReqLang, oPortalLang, dRow, dRow("RequestType"), False, oReqState)
                        curRequestList.Add(curRequest)
                    Next
                End If
                lrret.Requests = curRequestList.ToArray()
                ' Mostramos/ocultamos el botón de nueva solcitud en función de los permisos del empleado
                lrret.CanCreateRequest = False
                Dim RequestTypes() As eRequestType = RequestsHelper.GetPortalEnabled()
                Dim oState As New Requests.roRequestState(passportId)
                For Each oRequestTypeSecurity As eRequestType In RequestTypes
                    If VTPortal.SecurityHelper.GetFeaturePermission(passportId, New Requests.roRequestTypeSecurity(oRequestTypeSecurity, oState).EmployeeFeatureName, "E") >= Permission.Write Then
                        lrret.CanCreateRequest = True
                        Exit For
                    End If
                Next
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                lrret.Requests = {}
                lrret.CanCreateRequest = False

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::GetEmployeeRequests")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest(ByVal requestType As Integer, ByVal oPassport As roPassportTicket, ByVal employeeId As Integer, ByVal fromDate As String, ByVal toDate As String, ByVal fromTime As String, ByVal toTime As String, ByVal duration As String,
                                           ByVal idRequestObject As Integer, ByVal idRelatedObject As Integer, ByVal strStartShift As String, ByVal requestObs As String, ByVal direction As String,
                                           ByVal completeTask As Boolean, ByVal value1 As String, ByVal value2 As String, ByVal value3 As String, ByVal value4 As String, ByVal value5 As String, ByVal value6 As String,
                                           ByVal fieldName As String, ByVal fieldValue As String, ByVal hasHistory As Boolean, ByVal historyDate As String, ByVal bAllDay As Boolean, ByVal oResumeInfo As ExternalWorkResume(),
                                           ByVal oLng As roLanguage, ByVal TimeZone As TimeZoneInfo, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket,
                                           Optional oFile As Web.HttpPostedFile = Nothing, Optional idDocumentTemplate As Integer = Nothing, Optional isTelecommute As Boolean = False, Optional TCType As TelecommutingTypeEnum = Nothing, Optional oDailyRecord As roDailyRecord = Nothing, Optional comments As String = "") As StdRequestResponse
            Dim lrret As New StdRequestResponse

            Try
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)

                If oPermList.CanCreateRequests Then
                    lrret.Result = True
                    lrret.Status = ErrorCodes.OK

                    Dim bPermission As Boolean = False

                    For Each oPermission As ReqPermission In oPermList.Requests
                        If oPermission.RequestType = requestType Then
                            bPermission = oPermission.Permission
                            Exit For
                        End If
                    Next

                    If bPermission Then
                        Dim oTerminal As Terminal.roTerminal = Nothing
                        Dim dServerDateTime As Date = DateTime.Now

                        Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(employeeId, "LIVEPORTAL", New Terminal.roTerminalState())
                        If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                            oTerminal = oTerminals(0)
                            dServerDateTime = oTerminal.GetCurrentDateTime()
                        End If

                        Dim serverDatetime As DateTime = VTPortal.StatusHelper.GetCurrentTerminalDatetime(oTerminal, TimeZone)

                        If oSupervisor IsNot Nothing Then
                            requestObs &= oLng.Translate("Requests.CreatedBy", "") & oSupervisor.Name
                        End If

                        Select Case requestType
                            Case eRequestType.UserFieldsChange
                                lrret = SaveRequest_UserField(serverDatetime, employeeId, fieldName, fieldValue, hasHistory, historyDate, requestObs, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.ForbiddenPunch
                                lrret = SaveRequest_ForbiddenPunch(serverDatetime, employeeId, fromDate, idRequestObject, requestObs, direction, TimeZone, oReqState, acceptWarning, oSupervisor, oLng, isTelecommute, comments)
                            Case eRequestType.JustifyPunch
                                lrret = SaveRequest_CausePunch(serverDatetime, employeeId, fromDate, idRequestObject, requestObs, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.ExternalWorkResumePart
                                lrret = SaveRequest_ExternalWork(serverDatetime, employeeId, fromDate, duration, idRequestObject, requestObs, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.ChangeShift
                                lrret = SaveRequest_ChangeShift(serverDatetime, employeeId, fromDate, toDate, idRequestObject, idRelatedObject, strStartShift, requestObs, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.VacationsOrPermissions
                                lrret = SaveRequest_Holidays(serverDatetime, employeeId, fromDate, idRequestObject, requestObs, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.PlannedAbsences
                                lrret = SaveRequest_Absences(serverDatetime, employeeId, fromDate, toDate, idRequestObject, requestObs, oReqState, acceptWarning, oSupervisor, oLng, oFile, idDocumentTemplate, oPassport)
                            Case eRequestType.PlannedCauses
                                lrret = SaveRequest_Incidences(serverDatetime, employeeId, fromDate, toDate, fromTime, toTime, duration, idRequestObject, requestObs, oReqState, acceptWarning, oSupervisor, oLng, oFile, idDocumentTemplate, oPassport)
                            Case eRequestType.ForbiddenTaskPunch
                                lrret = SaveRequest_ForbiddenTaskPunch(serverDatetime, employeeId, fromDate, idRequestObject, requestObs, toDate, idRelatedObject, completeTask, value1, value2, value3, value4, value5, value6, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.CancelHolidays
                                lrret = SaveRequest_CancelHolidays(serverDatetime, employeeId, fromDate, idRequestObject, idRelatedObject, requestObs, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.ForgottenCostCenterPunch
                                lrret = SaveRequest_ForbiddenCostCenterPunch(serverDatetime, employeeId, fromDate, idRequestObject, requestObs, TimeZone, oReqState, acceptWarning, oSupervisor, oLng, comments)
                            Case eRequestType.PlannedHolidays
                                lrret = SaveRequest_PlannedHoliday(serverDatetime, employeeId, fromDate, bAllDay, fromTime, toTime, idRequestObject, requestObs, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.PlannedOvertimes
                                lrret = SaveRequest_Overtime(serverDatetime, employeeId, fromDate, toDate, fromTime, toTime, duration, idRequestObject, requestObs, oReqState, acceptWarning, oSupervisor, oLng, oFile, idDocumentTemplate, oPassport)
                            Case eRequestType.ExternalWorkWeekResume
                                lrret = SaveRequest_ExternalResume(serverDatetime, oResumeInfo, employeeId, requestObs, TimeZone, oReqState, acceptWarning, oSupervisor, oLng, comments)
                            Case eRequestType.ExchangeShiftBetweenEmployees
                                lrret = SaveRequest_ExchangeShift(serverDatetime, fromDate, toDate, idRequestObject, employeeId, idRelatedObject, value4, requestObs, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.Telecommute
                                lrret = SaveRequest_Telecommute(serverDatetime, employeeId, fromDate, toDate, TCType, requestObs, oReqState, acceptWarning, oSupervisor, oLng)
                            Case eRequestType.DailyRecord
                                lrret = SaveRequest_DailyRecord(serverDatetime, oDailyRecord, requestObs, TimeZone, oReqState, acceptWarning, oSupervisor, oLng)
                        End Select

                        If lrret.Result AndAlso Not oSupervisor Is Nothing Then
                            ' Crear notificación de suplantación en solicitud, si no exsite. Notificación para el empleado SOLO
                            Dim oNotificationState As New Notifications.roNotificationState(oReqState.IDPassport)
                            If Not Notifications.roNotification.GenerateNotificationTask(False, eNotificationType.Request_Impersonation, oNotificationState, lrret.RequestId, oSupervisor.ID,,,, Now) Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequest::Save::Unable to create notification task for impersonation of request id = " & lrret.RequestId)
                                lrret.Status = ErrorCodes.ERROR_CREATING_NOTIFICATION
                            End If
                        End If
                    Else
                        lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                        lrret.Result = False
                    End If
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                    lrret.Result = False
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                lrret.Result = False

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_ChangeShift(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal fromDate As String, ByVal toDate As String, ByVal idShift As Integer,
                                                       ByVal replaceShift As Integer, ByVal strStartShift As String, ByVal requestObs As String, ByRef oReqState As Requests.roRequestState,
                                                       ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse
            Dim lrret As New StdRequestResponse

            Try

                Dim oRequestChangeShift As New Requests.roRequest(-1, oReqState)

                oRequestChangeShift.ID = -1
                oRequestChangeShift.RequestDate = requestDate
                oRequestChangeShift.IDEmployee = employeeId

                If (idShift <> -1) Then
                    oRequestChangeShift.IDShift = idShift
                End If
                If (replaceShift <> -1) Then
                    oRequestChangeShift.Field4 = replaceShift
                End If
                oRequestChangeShift.strDate1 = fromDate
                oRequestChangeShift.strDate2 = toDate
                oRequestChangeShift.Comments = requestObs

                If (strStartShift <> "") Then
                    oRequestChangeShift.strStartShift = strStartShift
                End If

                oRequestChangeShift.RequestType = eRequestType.ChangeShift
                oRequestChangeShift.RequestStatus = eRequestStatus.Pending

                If Not oRequestChangeShift.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestChangeShift.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestChangeShift.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestChangeShift, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestChangeShift.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_ChangeShift")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_ExchangeShift(ByVal requestDate As DateTime, ByVal fromDate As String, ByVal xCompensateDate As String, ByVal idShift As Integer, ByVal employeeId As Integer,
                                                         ByVal destEmployeeId As Integer, ByVal idSourceShift As String,
                                                         ByVal requestObs As String, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean,
                                                         ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse
            Dim lrret As New StdRequestResponse

            Try

                Dim oRequestChangeShift As New Requests.roRequest(-1, oReqState)

                oRequestChangeShift.ID = -1
                oRequestChangeShift.RequestDate = requestDate
                oRequestChangeShift.strDate1 = fromDate

                If xCompensateDate <> String.Empty Then
                    oRequestChangeShift.strDate2 = xCompensateDate
                End If

                oRequestChangeShift.IDShift = idShift
                oRequestChangeShift.IDEmployee = employeeId
                oRequestChangeShift.IDEmployeeExchange = destEmployeeId
                oRequestChangeShift.Field4 = roTypes.Any2Double(idSourceShift)

                oRequestChangeShift.Comments = requestObs

                oRequestChangeShift.RequestType = eRequestType.ExchangeShiftBetweenEmployees
                oRequestChangeShift.RequestStatus = eRequestStatus.Pending

                If Not oRequestChangeShift.SaveWithParams(acceptWarning, True,,,, If(oSupervisor Is Nothing, True, False)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestChangeShift.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestChangeShift.State.ErrorText
                End If

                'Este tipo de solicitud no la puede aprovar un supervisor. Solo un empleado.
                'If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                '    lrret = ApproveRefuseRequest(oRequestChangeShift, oSupervisor, oReqState, oLng, True, False)
                'End If

                lrret.RequestId = oRequestChangeShift.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_ExchangeShift")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_Holidays(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal strDates As String, ByVal idShift As Integer, ByVal requestObs As String,
                                                    ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse
            Dim lrret As New StdRequestResponse

            Try
                Dim oRequestPermission As New Requests.roRequest(-1, oReqState)

                oRequestPermission.ID = -1
                oRequestPermission.RequestDate = requestDate
                oRequestPermission.IDEmployee = employeeId

                If (idShift <> -1) Then
                    oRequestPermission.IDShift = idShift
                End If

                If strDates <> String.Empty Then
                    Dim oLst As New Generic.List(Of Requests.roRequestDay)

                    Dim allDateStr As String() = strDates.Split("/")
                    Dim index As Integer = 0

                    For Each strDate In allDateStr
                        Dim oDay As New Requests.roRequestDay
                        oDay.AllDay = True
                        oDay.RequestDate = DateTime.ParseExact(strDate, "yyyyMMdd", Nothing)
                        oDay.FromTime = Nothing
                        oDay.ToTime = Nothing
                        oDay.Duration = Nothing

                        If (index = 0) Then
                            oRequestPermission.strDate1 = oDay.RequestDate.ToString("yyyy-MM-dd HH:mm")
                            oRequestPermission.strDate2 = oDay.RequestDate.ToString("yyyy-MM-dd HH:mm")
                        ElseIf (index = allDateStr.Length - 1) Then
                            oRequestPermission.strDate2 = oDay.RequestDate.ToString("yyyy-MM-dd HH:mm")
                        End If

                        index = index + 1

                        oLst.Add(oDay)
                    Next

                    oRequestPermission.RequestDays = oLst
                    oRequestPermission.Comments = requestObs

                    oRequestPermission.RequestType = eRequestType.VacationsOrPermissions
                    oRequestPermission.RequestStatus = eRequestStatus.Pending

                    If Not oRequestPermission.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                        lrret.Result = False
                        lrret.Status = GetErrorCodeFromRoRequestError(oRequestPermission.State.Result)
                        If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestPermission.State.ErrorText
                    End If
                Else
                    lrret.Result = False
                    lrret.Status = ErrorCodes.REQUEST_ERROR_IncorrectDates
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestPermission, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestPermission.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_Holidays")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_CancelHolidays(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal strDates As String, ByVal idCause As Integer, ByVal idShift As Integer,
                                                          ByVal requestObs As String, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse
            Dim lrret As New StdRequestResponse

            Try
                Dim oRequestPermission As New Requests.roRequest(-1, oReqState)

                oRequestPermission.ID = -1
                oRequestPermission.RequestDate = requestDate
                oRequestPermission.IDEmployee = employeeId

                If (idCause <> -1) Then oRequestPermission.IDCause = idCause
                If (idShift <> -1) Then oRequestPermission.IDShift = idShift

                If strDates <> String.Empty Then
                    Dim oLst As New Generic.List(Of Requests.roRequestDay)
                    Dim allDateStr As String() = strDates.Split("/")
                    Dim index As Integer = 0

                    For Each strDate In allDateStr
                        Dim oDay As New Requests.roRequestDay
                        oDay.AllDay = True
                        oDay.RequestDate = DateTime.ParseExact(strDate, "yyyyMMdd", Nothing)
                        oDay.FromTime = Nothing
                        oDay.ToTime = Nothing
                        oDay.Duration = Nothing

                        If (index = 0) Then
                            oRequestPermission.strDate1 = oDay.RequestDate.ToString("yyyy-MM-dd HH:mm")
                            oRequestPermission.strDate2 = oDay.RequestDate.ToString("yyyy-MM-dd HH:mm")
                        ElseIf (index = allDateStr.Length - 1) Then
                            oRequestPermission.strDate2 = oDay.RequestDate.ToString("yyyy-MM-dd HH:mm")
                        End If

                        index = index + 1

                        oLst.Add(oDay)
                    Next
                    oRequestPermission.RequestDays = oLst

                    oRequestPermission.Comments = requestObs

                    oRequestPermission.RequestType = eRequestType.CancelHolidays
                    oRequestPermission.RequestStatus = eRequestStatus.Pending

                    If Not oRequestPermission.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                        lrret.Result = False
                        lrret.Status = GetErrorCodeFromRoRequestError(oRequestPermission.State.Result)
                        If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestPermission.State.ErrorText
                    End If
                Else
                    lrret.Result = False
                    lrret.Status = ErrorCodes.REQUEST_ERROR_IncorrectDates
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestPermission, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestPermission.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_CancelHolidays")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_Telecommute(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal fromDate As String, ByVal toDate As String, ByVal tcType As TelecommutingTypeEnum,
                                                       ByVal requestObs As String, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse
            Dim lrret As New StdRequestResponse

            Try
                Dim oRequestTelecommute As New Requests.roRequest(-1, oReqState)

                oRequestTelecommute.RequestDate = requestDate
                oRequestTelecommute.IDEmployee = employeeId

                oRequestTelecommute.strDate1 = fromDate
                oRequestTelecommute.strDate2 = toDate
                oRequestTelecommute.Comments = requestObs
                oRequestTelecommute.Field1 = tcType

                oRequestTelecommute.RequestType = eRequestType.Telecommute
                oRequestTelecommute.RequestStatus = eRequestStatus.Pending

                If Not oRequestTelecommute.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestTelecommute.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestTelecommute.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestTelecommute, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestTelecommute.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_Telecommute")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_Absences(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal fromDate As String, ByVal toDate As String, ByVal idShift As Integer,
                                                    ByVal requestObs As String, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage,
                                                    Optional oFile As Web.HttpPostedFile = Nothing, Optional idDocumentTemplate As Integer = Nothing, Optional oPassport As roPassportTicket = Nothing) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                Dim oRequestAbsence As New Requests.roRequest(-1, oReqState)

                oRequestAbsence.RequestDate = requestDate
                oRequestAbsence.IDEmployee = employeeId

                oRequestAbsence.IDCause = idShift
                oRequestAbsence.strDate1 = fromDate
                oRequestAbsence.strDate2 = toDate
                oRequestAbsence.Comments = requestObs

                oRequestAbsence.RequestType = eRequestType.PlannedAbsences
                oRequestAbsence.RequestStatus = eRequestStatus.Pending

                If Not oRequestAbsence.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestAbsence.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestAbsence.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestAbsence, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestAbsence.ID

                If lrret.Result Then
                    If oFile IsNot Nothing Then
                        Dim oDocState As New VTDocuments.roDocumentState(-1)
                        roBusinessState.CopyTo(oReqState, oDocState)
                        Dim drret = DocumentsHelper.UploadDocument(oPassport, oFile, requestObs, idDocumentTemplate, oRequestAbsence.ID, "", ForecastType.Any, oDocState, lrret.RequestId)
                    End If
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_Absences")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_PlannedHoliday(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal datesStr As String, ByVal bAllDay As Boolean, ByVal fromTime As String,
                                                          ByVal toTime As String, ByVal idCause As Integer, ByVal requestObs As String, ByRef oReqState As Requests.roRequestState,
                                                          ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                Dim oRequestPlannedHoliday As New Requests.roRequest(-1, oReqState)

                oRequestPlannedHoliday.RequestDate = requestDate
                oRequestPlannedHoliday.IDEmployee = employeeId

                If (idCause <> -1) Then
                    oRequestPlannedHoliday.IDCause = idCause
                End If

                If datesStr <> String.Empty Then
                    Dim oLst As New Generic.List(Of Requests.roRequestDay)
                    Dim allDateStr As String() = datesStr.Split("/")
                    Dim index As Integer = 0
                    For Each strDate In allDateStr
                        Dim oDay As New Requests.roRequestDay
                        oDay.AllDay = bAllDay
                        oDay.RequestDate = DateTime.ParseExact(strDate, "yyyyMMdd", Nothing)
                        If Not bAllDay Then
                            oDay.FromTime = fromTime
                            oDay.ToTime = toTime
                            Dim duration As New DateTime(DateTime.ParseExact(toTime, "yyyy-MM-dd HH:mm", Nothing).Ticks - DateTime.ParseExact(fromTime, "yyyy-MM-dd HH:mm", Nothing).Ticks)
                            oDay.Duration = roTypes.Any2Time(duration.ToShortTimeString).NumericValue
                        End If
                        If (index = 0) Then
                            oRequestPlannedHoliday.strDate1 = oDay.RequestDate.ToString("yyyy-MM-dd HH:mm")
                            oRequestPlannedHoliday.strDate2 = oDay.RequestDate.ToString("yyyy-MM-dd HH:mm")
                        ElseIf (index = allDateStr.Length - 1) Then
                            oRequestPlannedHoliday.strDate2 = oDay.RequestDate.ToString("yyyy-MM-dd HH:mm")
                        End If
                        index = index + 1
                        oLst.Add(oDay)
                    Next
                    'allday
                    'fromTime      --Nomes si no all day '1899-12-30 xx.xx.xx
                    'toTime     --Nomes si no all day '1899-12-29 xx.xx.xx
                    'duration  --Nomes si no all day '1899-12-31 xx.xx.xx
                    oRequestPlannedHoliday.RequestDays = oLst
                    oRequestPlannedHoliday.Comments = requestObs
                    oRequestPlannedHoliday.RequestType = eRequestType.PlannedHolidays
                    oRequestPlannedHoliday.RequestStatus = eRequestStatus.Pending
                    If Not oRequestPlannedHoliday.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                        lrret.Result = False
                        lrret.Status = GetErrorCodeFromRoRequestError(oRequestPlannedHoliday.State.Result)
                        If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestPlannedHoliday.State.ErrorText
                    End If
                Else
                    lrret.Result = False
                    lrret.Status = ErrorCodes.REQUEST_ERROR_IncorrectDates
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestPlannedHoliday, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestPlannedHoliday.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_PlannedHoliday")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_Overtime(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal fromDate As String, ByVal toDate As String, ByVal fromTime As String,
                                                    ByVal toTime As String, ByVal duration As String, ByVal idCause As Integer, ByVal requestObs As String,
                                                    ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage,
                                                    Optional oFile As Web.HttpPostedFile = Nothing, Optional idDocumentTemplate As Integer = Nothing, Optional oPassport As roPassportTicket = Nothing) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                Dim oRequestIncidence As New Requests.roRequest(-1, oReqState)

                oRequestIncidence.RequestDate = requestDate
                oRequestIncidence.IDEmployee = employeeId

                If (idCause <> -1) Then
                    oRequestIncidence.IDCause = idCause
                End If
                oRequestIncidence.strDate1 = fromDate
                oRequestIncidence.strDate2 = toDate
                oRequestIncidence.strHours = duration
                oRequestIncidence.strFromTime = fromTime
                oRequestIncidence.strToTime = toTime
                oRequestIncidence.Comments = requestObs

                oRequestIncidence.RequestType = eRequestType.PlannedOvertimes
                oRequestIncidence.RequestStatus = eRequestStatus.Pending

                If Not oRequestIncidence.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestIncidence.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestIncidence.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestIncidence, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestIncidence.ID

                If lrret.Result Then
                    If oFile IsNot Nothing Then
                        Dim oDocState As New VTDocuments.roDocumentState(-1)
                        roBusinessState.CopyTo(oReqState, oDocState)
                        Dim drret = DocumentsHelper.UploadDocument(oPassport, oFile, requestObs, idDocumentTemplate, oRequestIncidence.ID, "", ForecastType.Any, oDocState, lrret.RequestId)
                    End If
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_Overtime")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_Incidences(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal fromDate As String, ByVal toDate As String, ByVal fromTime As String,
                                                      ByVal toTime As String, ByVal duration As String, ByVal idCause As Integer, ByVal requestObs As String,
                                                      ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage,
                                                       Optional oFile As Web.HttpPostedFile = Nothing, Optional idDocumentTemplate As Integer = Nothing, Optional oPassport As roPassportTicket = Nothing) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                Dim oRequestIncidence As New Requests.roRequest(-1, oReqState)

                oRequestIncidence.RequestDate = requestDate
                oRequestIncidence.IDEmployee = employeeId

                If (idCause <> -1) Then
                    oRequestIncidence.IDCause = idCause
                End If
                oRequestIncidence.strDate1 = fromDate
                oRequestIncidence.strDate2 = toDate
                oRequestIncidence.strHours = duration
                oRequestIncidence.strFromTime = fromTime
                oRequestIncidence.strToTime = toTime
                oRequestIncidence.Comments = requestObs

                oRequestIncidence.RequestType = eRequestType.PlannedCauses
                oRequestIncidence.RequestStatus = eRequestStatus.Pending

                If Not oRequestIncidence.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestIncidence.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestIncidence.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestIncidence, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestIncidence.ID

                If oFile IsNot Nothing Then
                    Dim oDocState As New VTDocuments.roDocumentState(-1)
                    roBusinessState.CopyTo(oReqState, oDocState)
                    Dim drret = DocumentsHelper.UploadDocument(oPassport, oFile, requestObs, idDocumentTemplate, oRequestIncidence.ID, "", ForecastType.Any, oDocState, lrret.RequestId)
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_Incidences")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_ExternalWork(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal strReqDate As String, ByVal duration As String, ByVal idCause As Integer,
                                                        ByVal requestObs As String, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                Dim oRequestExternalWork As New Requests.roRequest(-1, oReqState)

                oRequestExternalWork.strDate1 = strReqDate
                oRequestExternalWork.strHours = duration
                oRequestExternalWork.Comments = requestObs

                Dim oDatePunch As Date
                If strReqDate <> "" Then
                    Dim strDate As String
                    Dim strTime As String = "00:00"
                    Dim strArrDate() As String = oRequestExternalWork.strDate1.Split(" ")
                    strDate = strArrDate(0)
                    If strArrDate.Length = 2 Then strTime = oRequestExternalWork.strDate1.Split(" ")(1)
                    oDatePunch = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
                End If

                ' Verificamos que la fecha/hora no sea futura
                If oDatePunch > requestDate Then
                    lrret.Result = False
                    lrret.Status = ErrorCodes.REQUEST_ERROR_IncorrectDates
                End If

                oRequestExternalWork.ID = -1
                If (idCause <> -1) Then
                    oRequestExternalWork.IDCause = idCause
                Else
                    oRequestExternalWork.IDCause = Nothing
                End If

                oRequestExternalWork.RequestDate = requestDate
                oRequestExternalWork.Date2 = Nothing
                oRequestExternalWork.IDEmployee = employeeId
                oRequestExternalWork.RequestType = eRequestType.ExternalWorkResumePart
                oRequestExternalWork.RequestStatus = eRequestStatus.Pending

                If lrret.Result AndAlso Not oRequestExternalWork.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestExternalWork.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestExternalWork.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestExternalWork, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestExternalWork.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_ExternalWork")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_ExternalResume(ByVal requestDate As DateTime, ByVal oResumeInfo As ExternalWorkResume(), ByVal employeeId As Integer, ByVal requestObs As String,
                                                          ByVal TimeZone As TimeZoneInfo, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage, Optional ByVal comments As String = Nothing) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                Dim oRequestCausePunch As New Requests.roRequest(-1, oReqState)

                oRequestCausePunch.ID = -1
                oRequestCausePunch.RequestDate = requestDate

                Dim iIndex As Integer = 0
                Dim oLst As New Generic.List(Of Requests.roRequestDay)
                For Each oResume As ExternalWorkResume In oResumeInfo
                    Dim oDay As New Requests.roRequestDay
                    oDay.RequestDate = TimeZoneInfo.ConvertTimeFromUtc(oResume.DateTime, TimeZoneInfo.Local)
                    oDay.RequestDate = New Date(oDay.RequestDate.Year, oDay.RequestDate.Month, oDay.RequestDate.Day, oDay.RequestDate.Hour, oDay.RequestDate.Minute, 0)
                    oDay.ActualType = IIf(oResume.Direction.ToUpper = "E", PunchTypeEnum._IN, PunchTypeEnum._OUT)
                    If oResume.IdCause > 0 Then
                        oDay.IDCause = oResume.IdCause
                    End If
                    oDay.Comments = oResume.Remark

                    oLst.Add(oDay)
                    If (iIndex = 0) Then oRequestCausePunch.Date1 = oDay.RequestDate
                    If (iIndex = oResumeInfo.Length - 1) Then oRequestCausePunch.Date2 = oDay.RequestDate
                    iIndex += 1
                Next

                oRequestCausePunch.RequestDays = oLst
                oRequestCausePunch.IDEmployee = employeeId
                oRequestCausePunch.RequestType = eRequestType.ExternalWorkWeekResume
                oRequestCausePunch.RequestStatus = eRequestStatus.Pending
                oRequestCausePunch.Comments = requestObs

                If oRequestCausePunch.SaveWithParams(acceptWarning, True,,,, oSupervisor Is Nothing) Then
                    Dim bolRet As StdResponse = Nothing
                    For Each oReqDay As Requests.roRequestDay In oRequestCausePunch.RequestDays
                        bolRet = PunchHelper.SavePunch(Nothing, Nothing, employeeId, TimeZone, If(oReqDay.IDCause.HasValue, oReqDay.IDCause.Value, -1), If(oReqDay.ActualType.Value = PunchTypeEnum._IN, "E", "S"), -1, -1, "", "", "", Nothing, False, "", oReqDay.RequestDate, True,,,,,,, oReqDay.Comments, DTOs.NotReliableCause.ExternalWork.ToString())

                        If bolRet.Result Then
                            Dim oEmployeeState As New Employee.roEmployeeState(-1)
                            roBusinessState.CopyTo(oReqState, oEmployeeState)

                            If Not VTBusiness.Punch.roPunch.ReorderPunches(employeeId, oReqDay.RequestDate, oEmployeeState) Then
                                lrret.Status = ErrorCodes.FORBID_ERROR_SAVE_PUNCH
                            End If
                        Else
                            lrret.Status = ErrorCodes.FORBID_ERROR_SAVE_PUNCH
                        End If

                    Next
                Else
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestCausePunch.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestCausePunch.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestCausePunch, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestCausePunch.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_ExternalResume")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_CausePunch(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal punchdate As String, ByVal idCause As Integer, ByVal requestObs As String,
                                                      ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                Dim oRequestCausePunch As New Requests.roRequest(-1, oReqState)

                oRequestCausePunch.ID = -1
                oRequestCausePunch.IDCause = idCause
                oRequestCausePunch.Date1 = punchdate
                oRequestCausePunch.RequestDate = requestDate
                oRequestCausePunch.Date2 = Nothing
                oRequestCausePunch.IDEmployee = employeeId
                oRequestCausePunch.RequestType = eRequestType.JustifyPunch
                oRequestCausePunch.RequestStatus = eRequestStatus.Pending
                oRequestCausePunch.Comments = requestObs

                If Not oRequestCausePunch.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestCausePunch.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestCausePunch.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestCausePunch, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestCausePunch.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_CausePunch")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_ForbiddenTaskPunch(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal punchdate As String, ByVal idTask As Integer, ByVal requestObs As String,
                                                               ByVal punchEnddate As String, ByVal cTaskId As Integer, ByVal completeTask As Boolean,
                                                               ByVal value1 As String, ByVal value2 As String, ByVal value3 As String, ByVal value4 As String, ByVal value5 As String, ByVal value6 As String,
                                                               ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                Dim oRequestForbiddenTaskPunch As New Requests.roRequest(-1, oReqState)

                oRequestForbiddenTaskPunch.ID = -1
                oRequestForbiddenTaskPunch.RequestDate = requestDate
                oRequestForbiddenTaskPunch.IDEmployee = employeeId
                oRequestForbiddenTaskPunch.RequestType = eRequestType.ForbiddenTaskPunch
                oRequestForbiddenTaskPunch.RequestStatus = eRequestStatus.Pending

                If (idTask <> -1) Then
                    oRequestForbiddenTaskPunch.IDTask1 = idTask
                End If
                oRequestForbiddenTaskPunch.strDate1 = punchdate

                If cTaskId >= 0 Then
                    oRequestForbiddenTaskPunch.strDate2 = punchEnddate
                    If (cTaskId <> -1) Then
                        oRequestForbiddenTaskPunch.IDTask2 = cTaskId
                    End If

                    If cTaskId > 0 Then oRequestForbiddenTaskPunch.CompletedTask = completeTask
                    oRequestForbiddenTaskPunch.Field1 = ""
                    oRequestForbiddenTaskPunch.Field2 = ""
                    oRequestForbiddenTaskPunch.Field3 = ""
                    oRequestForbiddenTaskPunch.Field4 = -1
                    oRequestForbiddenTaskPunch.Field5 = -1
                    oRequestForbiddenTaskPunch.Field6 = -1

                    If (value1 <> String.Empty) Then oRequestForbiddenTaskPunch.Field1 = value1
                    If (value2 <> String.Empty) Then oRequestForbiddenTaskPunch.Field2 = value2
                    If (value3 <> String.Empty) Then oRequestForbiddenTaskPunch.Field3 = value3
                    If (value4 <> String.Empty) Then oRequestForbiddenTaskPunch.Field4 = roTypes.Any2Double(value4.Replace(".", oReqState.Language.GetDecimalDigitFormat()))
                    If (value5 <> String.Empty) Then oRequestForbiddenTaskPunch.Field5 = roTypes.Any2Double(value5.Replace(".", oReqState.Language.GetDecimalDigitFormat()))
                    If (value6 <> String.Empty) Then oRequestForbiddenTaskPunch.Field6 = roTypes.Any2Double(value6.Replace(".", oReqState.Language.GetDecimalDigitFormat()))
                End If

                oRequestForbiddenTaskPunch.Comments = requestObs

                If Not oRequestForbiddenTaskPunch.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestForbiddenTaskPunch.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestForbiddenTaskPunch.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestForbiddenTaskPunch, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestForbiddenTaskPunch.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_ForbiddenTaskPunch")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_UserField(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal fieldName As String, ByVal fieldValue As String, ByVal hasHistory As Boolean,
                                                     ByVal historyDate As String, ByVal requestObs As String, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage) As StdRequestResponse

            Dim lrret As New StdRequestResponse
            Try
                Dim oUserField As New Requests.roRequest(-1, oReqState)

                oUserField.ID = -1

                oUserField.Comments = requestObs
                oUserField.FieldName = fieldName
                oUserField.FieldValue = fieldValue
                If (hasHistory) Then
                    oUserField.strDate1 = historyDate
                End If

                oUserField.RequestDate = requestDate
                oUserField.Date2 = Nothing
                oUserField.IDEmployee = employeeId
                oUserField.RequestType = eRequestType.UserFieldsChange
                oUserField.RequestStatus = eRequestStatus.Pending

                If Not oUserField.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oUserField.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oUserField.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oUserField, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oUserField.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_UserField")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_ForbiddenPunch(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal punchdate As String, ByVal idCause As Integer, ByVal requestObs As String, ByVal direction As String,
                                                          ByVal TimeZone As TimeZoneInfo, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage, Optional isTelecommute As Boolean = False, Optional comments As String = "") As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                Dim oRequestCausePunch As New Requests.roRequest(-1, oReqState)
                oRequestCausePunch.IDCause = idCause
                oRequestCausePunch.strDate1 = punchdate
                oRequestCausePunch.Comments = requestObs
                If oRequestCausePunch.IDCause = -1 Then oRequestCausePunch.IDCause = Nothing
                oRequestCausePunch.RequestDate = requestDate
                oRequestCausePunch.Date2 = Nothing
                oRequestCausePunch.IDEmployee = employeeId
                oRequestCausePunch.RequestType = eRequestType.ForbiddenPunch
                oRequestCausePunch.RequestStatus = eRequestStatus.Pending
                oRequestCausePunch.Field1 = direction

                Dim oDatePunch As Date

                If oRequestCausePunch.strDate1 <> "" Then
                    Dim strDate As String
                    Dim strTime As String = "00:00"
                    Dim strArrDate() As String = oRequestCausePunch.strDate1.Split(" ")
                    strDate = strArrDate(0)
                    If strArrDate.Length = 2 Then strTime = oRequestCausePunch.strDate1.Split(" ")(1)
                    oDatePunch = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
                End If

                ' Verificamos que la fecha/hora no sea futura
                If oDatePunch > requestDate Then
                    lrret.Result = False
                    lrret.Status = ErrorCodes.FORBID_INCORRECT_DATE_FUTURE
                    Return lrret
                End If

                If oRequestCausePunch.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then
                    Dim bolRet As StdResponse = PunchHelper.SavePunch(Nothing, Nothing, employeeId, TimeZone, If(oRequestCausePunch.IDCause.HasValue, oRequestCausePunch.IDCause.Value, -1), direction, -1, -1, "", "", "", Nothing, False, "", oDatePunch, True, isTelecommute,,,,,, comments, DTOs.NotReliableCause.ForgottenPunch.ToString())

                    If bolRet.Result Then
                        bolRet.Result = Notifications.roNotification.DeleteIncompletePunchNotificationIfExist(New Notifications.roNotificationState(oReqState.IDPassport), employeeId, oDatePunch)
                    End If

                    If bolRet.Result Then
                        Dim oEmployeeState As New Employee.roEmployeeState(-1)
                        roBusinessState.CopyTo(oReqState, oEmployeeState)

                        If Not VTBusiness.Punch.roPunch.ReorderPunches(employeeId, oDatePunch, oEmployeeState) Then
                            lrret.Status = ErrorCodes.FORBID_ERROR_SAVE_PUNCH
                        End If
                    Else
                        lrret.Status = ErrorCodes.FORBID_ERROR_SAVE_PUNCH
                    End If
                Else
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestCausePunch.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestCausePunch.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestCausePunch, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestCausePunch.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_ForbiddenPunch")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_ForbiddenCostCenterPunch(ByVal requestDate As DateTime, ByVal employeeId As Integer, ByVal punchdate As String, ByVal idCenter As Integer, ByVal requestObs As String,
                                                                    ByVal TimeZone As TimeZoneInfo, ByRef oReqState As Requests.roRequestState, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, ByVal oLng As roLanguage, Optional comments As String = Nothing) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try

                Dim oRequestPermission As New Requests.roRequest(-1, oReqState)

                oRequestPermission.ID = -1
                oRequestPermission.RequestDate = requestDate

                oRequestPermission.strDate1 = punchdate
                oRequestPermission.IDCenter = idCenter
                oRequestPermission.Comments = requestObs

                oRequestPermission.IDEmployee = employeeId
                oRequestPermission.RequestType = eRequestType.ForgottenCostCenterPunch
                oRequestPermission.RequestStatus = eRequestStatus.Pending

                If oRequestPermission.SaveWithParams(acceptWarning, True,,,, If(oSupervisor IsNot Nothing, False, True)) Then

                    Dim strDate As String
                    Dim strTime As String = "00:00"
                    Dim strArrDate() As String = oRequestPermission.strDate1.Split(" ")
                    strDate = strArrDate(0)
                    If strArrDate.Length = 2 Then strTime = oRequestPermission.strDate1.Split(" ")(1)
                    Dim oDatePunch As DateTime = New Date(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)

                    Dim bolRet As StdResponse = CostCenterHelper.SaveCostCenterPunch(Nothing, employeeId, TimeZone, oRequestPermission.IDCenter, -1, -1, "", "", "", Nothing, False, oDatePunch,, comments, DTOs.NotReliableCause.CostCenterForgottenPunch.ToString())

                    If bolRet.Result Then
                        Dim oEmployeeState As New Employee.roEmployeeState(-1)
                        roBusinessState.CopyTo(oReqState, oEmployeeState)

                        If Not VTBusiness.Punch.roPunch.ReorderPunches(employeeId, oDatePunch, oEmployeeState) Then
                            lrret.Status = ErrorCodes.FORBID_ERROR_SAVE_PUNCH
                        End If
                    Else
                        lrret.Status = ErrorCodes.FORBID_ERROR_SAVE_PUNCH
                    End If
                Else
                    lrret.Result = False
                    lrret.Status = GetErrorCodeFromRoRequestError(oRequestPermission.State.Result)
                    If (lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError OrElse lrret.Status = ErrorCodes.REQUEST_WARNING_NeedConfirmation) Then lrret.StatusErrorMsg = oRequestPermission.State.ErrorText
                End If

                If lrret.Result AndAlso oSupervisor IsNot Nothing Then
                    lrret = ApproveRefuseRequest(oRequestPermission, oSupervisor, oReqState, oLng, True, False)
                End If

                lrret.RequestId = oRequestPermission.ID
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveRequest_ForbiddenCostCenterPunch")
            End Try

            Return lrret
        End Function

        Public Shared Function SaveRequest_DailyRecord(ByVal requestDate As DateTime,
                                               ByVal dailyRecord As roDailyRecord,
                                               ByVal requestObs As String,
                                               ByVal TimeZone As TimeZoneInfo,
                                               ByRef oReqState As Requests.roRequestState,
                                               ByVal acceptWarning As Boolean,
                                               ByVal oSupervisor As roPassportTicket,
                                               ByVal oLng As roLanguage
                                               ) As StdRequestResponse

            Dim lrret As New StdRequestResponse

            Try
                Dim oDailyRecordManager As roDailyRecordManager = New roDailyRecordManager(New roDailyRecordState(oReqState.IDPassport))

                oDailyRecordManager.SaveDailyRecord(dailyRecord, requestDate, acceptWarning, oSupervisor, True)
                lrret.Result = (oDailyRecordManager.State.Result = DailyRecordResultEnum.NoError)
                lrret.Status = oDailyRecordManager.GetPortalErrorCode(oDailyRecordManager.State.Result)
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::SaveDailyRecord")
            End Try

            Return lrret
        End Function

        Public Shared Function GetDailyRecord(ByVal idDailyRecord As Integer,
                                               ByVal oSupervisor As roPassportTicket,
                                               ByRef oReqState As Requests.roRequestState,
                                               ByVal oLng As roLanguage,
                                               ByRef oUserRequest As UserRequest
                                               ) As roGenericResponse(Of roDailyRecord)

            Dim lrret As New roGenericResponse(Of roDailyRecord)

            Try
                Dim oDailyRecordManager As roDailyRecordManager = New roDailyRecordManager(New roDailyRecordState(oReqState.IDPassport))

                Dim oRequest As Requests.roRequest = Nothing
                lrret.Value = oDailyRecordManager.LoadDailyRecord(idDailyRecord, oRequest, True)
                oUserRequest = GetRequestByEmployee(oRequest.ID, lrret.Value.IdEmployee, oLng, oReqState)
                lrret.Status = oDailyRecordManager.GetPortalErrorCode(oDailyRecordManager.State.Result)
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::GetDailyRecord")
            End Try

            Return lrret
        End Function

        Public Shared Function ApproveRefuseRequest(ByVal oRequest As Requests.roRequest, ByVal oSupervisor As roPassportTicket, ByRef oReqState As Requests.roRequestState, ByVal oLng As roLanguage, ByVal bApprove As Boolean, ByVal bforceApprove As Boolean) As StdRequestResponse
            Dim lrret As New StdRequestResponse
            Try
                lrret.Result = True
                lrret.Status = ErrorCodes.OK

                Dim oSupervisorState As New Requests.roRequestState(oSupervisor.ID)

                oRequest = New Requests.roRequest(oRequest.ID, oSupervisorState)

                If Not oRequest.AutomaticValidation Then
                    Dim strApproveRefuse As String = oLng.Translate("Requests.ApprovedBy", "")
                    If Not bApprove Then
                        strApproveRefuse = oLng.Translate("Requests.RefusedBy", "")
                    End If
                    If Not oRequest.ApproveRefuse(oSupervisor.ID, bApprove, strApproveRefuse & oSupervisor.Name,, bforceApprove, False, True) Then
                        Select Case oRequest.State.Result
                            Case RequestResultEnum.NoApprovePermissions 'Continua tot igual en aquests casos
                            Case RequestResultEnum.NoApproveRefuseLevelOfAuthorityRequired 'Continua tot igual en aquests casos
                                lrret.Result = True
                                lrret.Status = ErrorCodes.OK
                            Case RequestResultEnum.NotEnoughConceptBalance, RequestResultEnum.NeedConfirmation
                                lrret.Result = False
                                lrret.Status = ErrorCodes.REQUEST_PENDING_VALIDATION
                                lrret.StatusErrorMsg = oRequest.State.ErrorText
                            Case RequestResultEnum.RequestRuleError 'Deneguem
                                lrret.Result = oRequest.ApproveRefuse(oSupervisor.ID, False, oRequest.State.ErrorText & "." & oLng.Translate("Requests.RefusedBy", "") & oSupervisor.Name,,, False, True)
                                lrret.Status = ErrorCodes.REQUEST_DENIED_DUE_REQUESTVALIDATION
                                lrret.StatusErrorMsg = oRequest.State.ErrorText
                            Case RequestResultEnum.SaveCalendarException, RequestResultEnum.CalendarGenerateIndictments,
                                 RequestResultEnum.CompensationRequired, RequestResultEnum.EmployeeNotSuitableForShiftExchange,
                                 RequestResultEnum.BlockedDayOnShiftExchange, RequestResultEnum.RequestPendingOnShiftExchange,
                                 RequestResultEnum.NoPermissionOnShiftExchange, RequestResultEnum.OnAbsenceOnShiftExchange,
                                 RequestResultEnum.OnHolidaysOnShiftExchange, RequestResultEnum.ApplicantCantCoverEmployeeOnShiftExchange,
                                 RequestResultEnum.EmployeeCantCoverApplicantOnShiftExchange, RequestResultEnum.NoAssignmentOnShiftExchange,
                                 RequestResultEnum.WrongAssignmentOnShiftExchange, RequestResultEnum.IndictmentOnShiftExchange,
                                 RequestResultEnum.FreezeDateException, RequestResultEnum.VacationsOrPermissionsError,
                                 RequestResultEnum.ExistsLockedDaysInPeriod, RequestResultEnum.NoPunchesForDailyRecord
                                lrret.Result = False
                                lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError
                                lrret.StatusErrorMsg = oRequest.State.ErrorText
                            Case Else
                                lrret.Result = oRequest.ApproveRefuse(oSupervisor.ID, False, oLng.Translate("Requests.RefusedBy", "") & oSupervisor.Name,,, False, True)
                                lrret.Status = ErrorCodes.REQUEST_DENIED_DUE_SERVERDATA
                        End Select
                    End If

                    If lrret.Result Then

                        If oRequest.RequestType <> eRequestType.DailyRecord Then
                            Dim oNotificationState As New Notifications.roNotificationState(oReqState.IDPassport)
                            If Not Notifications.roNotification.GenerateNotificationsForRequest(oRequest.ID, True, oNotificationState, True) Then
                                lrret.Result = True
                                lrret.Status = ErrorCodes.ERROR_CREATING_NOTIFICATION
                            End If
                        End If

                    End If
                Else
                    Dim oNotificationState As New Notifications.roNotificationState(oReqState.IDPassport)
                    Notifications.roNotification.GenerateNotificationsForRequest(oRequest.ID, False, oNotificationState, oRequest.AutomaticValidation)
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::ApproveRefuseRequest")
            End Try

            Return lrret
        End Function

        Public Shared Function GetErrorCodeFromRoRequestError(ByVal roErrorCode As Integer) As Integer
            Dim returnValue As Integer = ErrorCodes.GENERAL_ERROR
            Select Case roErrorCode
                Case RequestResultEnum.Exception
                    returnValue = ErrorCodes.GENERAL_ERROR
                Case RequestResultEnum.ConnectionError
                    returnValue = ErrorCodes.REQUEST_ERROR_ConnectionError
                Case RequestResultEnum.SqlError
                    returnValue = ErrorCodes.REQUEST_ERROR_SqlError
                Case RequestResultEnum.NoDeleteBecauseNotPending
                    returnValue = ErrorCodes.REQUEST_ERROR_NoDeleteBecauseNotPending
                Case RequestResultEnum.IncorrectDates
                    returnValue = ErrorCodes.REQUEST_ERROR_IncorrectDates
                Case RequestResultEnum.NoApprovePermissions
                    returnValue = ErrorCodes.REQUEST_ERROR_NoApprovePermissions
                Case RequestResultEnum.UserFieldNoRequestVisible ' El campo de la ficha no es visible para solicitudes
                    returnValue = ErrorCodes.REQUEST_ERROR_UserFieldNoRequestVisible
                Case RequestResultEnum.NoApproveRefuseLevelOfAuthorityRequired ' No se puede aprobar/denegar la solicitud ya que un nivel de mando de rango superior ya la ha aprobado/denegado
                    returnValue = ErrorCodes.REQUEST_ERROR_NoApproveRefuseLevelOfAuthorityRequired
                Case RequestResultEnum.UserFieldValueSaveError ' No se ha podido guardar el valor del campo de la ficha
                    returnValue = ErrorCodes.REQUEST_ERROR_UserFieldValueSaveError
                Case RequestResultEnum.InvalidPassport
                    returnValue = ErrorCodes.REQUEST_ERROR_InvalidPassport
                Case RequestResultEnum.ChangeShiftError ' No se ha podido cambiar la planificación del horario
                    returnValue = ErrorCodes.REQUEST_ERROR_ChangeShiftError
                Case RequestResultEnum.VacationsOrPermissionsError ' No se ha podido planificar el periodo  de vacaciones
                    returnValue = ErrorCodes.REQUEST_ERROR_VacationsOrPermissionsError
                Case RequestResultEnum.ExistsLockedDaysInPeriod ' Hay días bloqueados en el periodo a planificar
                    returnValue = ErrorCodes.REQUEST_ERROR_ExistsLockedDaysInPeriod
                Case RequestResultEnum.ForbiddenPunchError ' Error al validar el fichaje olvidado
                    returnValue = ErrorCodes.REQUEST_ERROR_ForbiddenPunchError
                Case RequestResultEnum.JustifyPunchError ' Error al validar la justificación del fichaje
                    returnValue = ErrorCodes.REQUEST_ERROR_JustifyPunchError
                Case RequestResultEnum.RequestMoveNotExist ' El fichaje relacionado con la solicitud no existe o se ha modificado
                    returnValue = ErrorCodes.REQUEST_ERROR_RequestMoveNotExist
                Case RequestResultEnum.RequestMoveTooMany  ' Hay más de un fichaje relacionado con la solicitud
                    returnValue = ErrorCodes.REQUEST_ERROR_RequestMoveTooMany
                Case RequestResultEnum.PlannedAbsencesError ' Error al validar la solicitud de ausencia prolongada
                    returnValue = ErrorCodes.REQUEST_ERROR_PlannedAbsencesError
                Case RequestResultEnum.PlannedCausesError ' Error al validar la solicitud de incidencia prevista
                    returnValue = ErrorCodes.REQUEST_ERROR_PlannedCausesError
                Case RequestResultEnum.ExternalWorkResumePartError ' Error al validar la solicitud de parte de trabajo externo
                    returnValue = ErrorCodes.REQUEST_ERROR_ExternalWorkResumePartError
                Case RequestResultEnum.UserFieldRequired ' Campo de la ficha requerido
                    returnValue = ErrorCodes.REQUEST_ERROR_UserFieldRequired
                Case RequestResultEnum.PunchDateTimeRequired ' Fecha y hora del fichaje requerida
                    returnValue = ErrorCodes.REQUEST_ERROR_PunchDateTimeRequired
                Case RequestResultEnum.CauseRequired ' Justificación requerida
                    returnValue = ErrorCodes.REQUEST_ERROR_CauseRequired
                Case RequestResultEnum.DateRequired ' Fecha requerida
                    returnValue = ErrorCodes.REQUEST_ERROR_DateRequired
                Case RequestResultEnum.HoursRequired ' Horas requeridas
                    returnValue = ErrorCodes.REQUEST_ERROR_HoursRequired
                Case RequestResultEnum.ShiftRequired ' Horario requerido
                    returnValue = ErrorCodes.REQUEST_ERROR_ShiftRequired
                Case RequestResultEnum.RequestRepited ' Solicitud repetida (ya existe una solicitud con el mismo tipo de solicitud y el mismo empleado en los dos últimos segundos)
                    returnValue = ErrorCodes.REQUEST_ERROR_RequestRepited
                Case RequestResultEnum.PunchExist ' Ya existe un fichaje con la misma fecha y hora
                    returnValue = ErrorCodes.REQUEST_ERROR_PunchExist
                Case RequestResultEnum.StartShiftRequired ' Inicio de horario flotante requerido
                    returnValue = ErrorCodes.REQUEST_ERROR_StartShiftRequired
                Case RequestResultEnum.PlannedCausesOverlapped 'Solicitudes de Horas de Ausencia solapadas
                    returnValue = ErrorCodes.REQUEST_ERROR_PlannedCausesOverlapped
                Case RequestResultEnum.PlannedAbsencesOverlapped 'Solicitudes de Dias de ausencia solapadas
                    returnValue = ErrorCodes.REQUEST_ERROR_PlannedAbsencesOverlapped
                Case RequestResultEnum.TaskRequiered 'Tarea requerida
                    returnValue = ErrorCodes.REQUEST_ERROR_TaskRequiered
                Case RequestResultEnum.CostCenterRequiered
                    returnValue = ErrorCodes.REQUEST_ERROR_CostCenterRequiered
                Case RequestResultEnum.PlannedHolidaysError ' Error al validar la solicitud de vacaciones por horas
                    returnValue = ErrorCodes.REQUEST_ERROR_PlannedHolidaysError
                Case RequestResultEnum.PlannedHolidaysOverlapped 'Solicitudes de rpevision de vacaciones/permisos por horas solapadas
                    returnValue = ErrorCodes.REQUEST_ERROR_PlannedHolidaysOverlapped
                Case RequestResultEnum.AnotherHolidayExistInDate
                    returnValue = ErrorCodes.REQUEST_ERROR_AnotherHolidayExistInDate
                Case RequestResultEnum.AnotherAbsenceExistInDate
                    returnValue = ErrorCodes.REQUEST_ERROR_AnotherAbsenceExistInDate
                Case RequestResultEnum.InHolidayPlanification
                    returnValue = ErrorCodes.REQUEST_ERROR_InHolidayPlanification
                Case RequestResultEnum.VacationsOrPermissionsOverlapped
                    returnValue = ErrorCodes.REQUEST_ERROR_VacationsOrPermissionsOverlapped
                Case RequestResultEnum.NeedConfirmation
                    returnValue = ErrorCodes.REQUEST_WARNING_NeedConfirmation
                Case Else
                    returnValue = ErrorCodes.REQUEST_ERROR_CustomError
            End Select

            Return returnValue
        End Function

        Public Shared Function DeleteRequest(ByVal idRequest As Integer, ByRef oReqState As Requests.roRequestState) As StdResponse
            Dim lrret As New StdResponse

            Try
                lrret.Result = True
                lrret.Status = ErrorCodes.OK
                Dim oRequest As New Requests.roRequest(idRequest, oReqState, False)
                If oRequest.RequestStatus = eRequestStatus.Pending Then
                    Select Case oRequest.RequestType
                        Case eRequestType.DailyRecord
                            Dim oDailyRecordManager As roDailyRecordManager = Nothing
                            oDailyRecordManager = New roDailyRecordManager(New roDailyRecordState(oReqState.IDPassport))
                            lrret.Result = oDailyRecordManager.DeleteDailyRecord(oRequest.ID, True)
                            lrret.Status = oDailyRecordManager.GetPortalErrorCode(oDailyRecordManager.State.Result)
                        Case Else
                            lrret.Result = oRequest.Delete(True)
                            If Not lrret.Result Then
                                lrret.Status = GetErrorCodeFromRoRequestError(oRequest.State.Result)
                            End If
                    End Select
                Else
                    lrret.Result = False
                    lrret.Status = ErrorCodes.REQUEST_ERROR_NoDeleteBecauseNotPending
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::DeleteRequest")
            End Try

            Return lrret
        End Function

        Public Shared Function ChangeTelecommuting(ByVal idEmployee As Integer, ByVal selectedDay As Date, ByVal type As TelecommutingTypeEnum, ByVal bImpersonating As Boolean) As StdResponse
            Dim lrret As New StdResponse

            Try
                lrret.Result = True
                lrret.Status = ErrorCodes.OK

                lrret.Result = roCalendarManager.ChangeTelecommuting(idEmployee, selectedDay.Date, type, bImpersonating)

                If Not lrret.Result Then
                    lrret.Status = ErrorCodes.CHANGE_TELECOMMUTING_ERROR
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::ChangeTelecommuting")
            End Try

            Return lrret
        End Function

        Public Shared Function GetMyTeamPlanEmployees(ByVal idEmployee As Integer, ByVal BeginDay As Date, ByVal EndDay As Date, ByVal oldShift As Integer, ByVal newShift As Integer, ByVal idPassport As Integer) As MyTeamPlanInfo
            Dim lrret As New MyTeamPlanInfo
            Dim EmployeePlanning As New Generic.List(Of EmployeePlanning)
            Dim oEmpState As New Employee.roEmployeeState(idPassport)
            Try
                lrret.Status = ErrorCodes.OK
                Dim lDays As New Generic.List(Of Days)
                While BeginDay <= EndDay
                    Dim oDay As New Days
                    oDay.DatePlanned = BeginDay
                    Dim lstEmployeesPlanned As New Generic.List(Of EmployeesPlanned)

                    Dim dt As New DataTable
                    dt = roCalendarManager.GetMyTeamPlanEmployees(idEmployee, BeginDay, BeginDay, oldShift)
                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                        Dim idInterno = 0
                        For Each oRow As DataRow In dt.Rows
                            Dim oEmployeesPlanned As New EmployeesPlanned
                            Dim bolAdd As Boolean = False
                            If oldShift > 0 Then
                                If roTypes.Any2Integer(oRow("IDShift1")) = oldShift OrElse roTypes.Any2Integer(oRow("IDShift1")) = newShift Then
                                    bolAdd = True
                                End If
                            Else
                                If roTypes.Any2Integer(oRow("IDShift1")) = roTypes.Any2Integer(oRow("EmployeeShift")) OrElse roTypes.Any2Integer(oRow("IDShift1")) = newShift Then
                                    bolAdd = True
                                End If
                            End If

                            If bolAdd Then
                                oEmployeesPlanned.IdCounter = idInterno + 1
                                oEmployeesPlanned.IdEmployee = oRow("IDEmployee")
                                oEmployeesPlanned.EmployeeName = oRow("EmployeeName")
                                oEmployeesPlanned.IdShift = oRow("IDShift1")
                                ' Ordenamos siempre por el nuevo horario
                                If newShift = oEmployeesPlanned.IdShift Then
                                    oEmployeesPlanned.Pos = 1
                                Else
                                    oEmployeesPlanned.Pos = 2
                                End If

                                oEmployeesPlanned.ShiftName = oRow("ShiftName")
                                oEmployeesPlanned.EmployeeImage = VTPortal.EmployeesHelper.LoadEmployeeImage(oRow("EmployeeImage"), oEmpState)

                                lstEmployeesPlanned.Add(oEmployeesPlanned)

                            End If
                        Next
                        If lstEmployeesPlanned.Count > 0 Then lstEmployeesPlanned = lstEmployeesPlanned.OrderBy(Function(f) f.Pos).ToList()
                    End If
                    oDay.Employees = lstEmployeesPlanned.ToArray()
                    lDays.Add(oDay)

                    If oldShift <= 0 Then
                        Dim dt2 As New DataTable
                        dt2 = roCalendarManager.GetEmployeePlanningBetweenDates(idEmployee, BeginDay, BeginDay)
                        If dt2 IsNot Nothing AndAlso dt2.Rows.Count > 0 Then
                            Dim oShiftState As New Shift.roShiftState
                            For Each oRow As DataRow In dt2.Rows
                                Dim oEmployeePlanning As New EmployeePlanning
                                oEmployeePlanning.IdEmployee = oRow("IDEmployee")
                                oEmployeePlanning.IdShift = oRow("IDShift")
                                oEmployeePlanning.DatePlanned = oRow("DatePlanned")

                                ' Dim oldShift = VTPortal.ShiftsHelper.GetShiftById(oEmployeePlanning.IdShift, oShiftState)
                                oEmployeePlanning.ShiftName = oRow("ShiftName")
                                oEmployeePlanning.ShiftColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRow("ShiftColor"))))

                                EmployeePlanning.Add(oEmployeePlanning)
                            Next
                        End If

                    End If

                    BeginDay = BeginDay.AddDays(1)
                End While

                lrret.EmployeePlanning = EmployeePlanning.ToArray()
                lrret.Days = lDays.ToArray()
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::GetMyTeamPlanEmployees")
            End Try

            Return lrret
        End Function

        Public Shared Function GetRankingForDay(ByVal xDate As DateTime, ByVal IdEmployee As Integer, ByVal idRequestType As Integer, ByVal idReason As Integer, ByRef oReqState As Requests.roRequestState) As roEmployeeRankingInformation
            Dim lrret As New roEmployeeRankingInformation

            Try
                lrret.Status = ErrorCodes.OK

                Dim oRequest As New Requests.roRequest With {
                    .IDEmployee = IdEmployee,
                    .RequestType = idRequestType,
                    .strDate1 = xDate.Year.ToString & "-" & xDate.Month.ToString & "-" & xDate.Day.ToString & " " & xDate.Hour.ToString & ":" & xDate.Minute.ToString & ":" & xDate.Second.ToString,
                    .IDShift = idReason,
                    .IDCause = idReason
                    }

                If Requests.roRequestRuleManager.IsRuleUsedOnRequest(oRequest, eRequestRuleType.MinCoverageRequiered, oReqState) Then
                    lrret.Rankings = Requests.roRequest.GetEmployeeRankingPositions(IdEmployee, xDate, oReqState)
                    Dim oMyPosition = lrret.Rankings.RequestEmployeeRankingPositions.ToList.Find(Function(emp) emp.IDEmployee = IdEmployee)

                    If oMyPosition Is Nothing Then
                        oMyPosition = New roRequestEmployeeRankingPosition() With {
                            .IDEmployee = IdEmployee,
                            .IDPosition = lrret.Rankings.RequestEmployeeRankingPositions.Length + 1,
                            .EmployeeName = VTBusiness.Common.roBusinessSupport.GetEmployeeName(IdEmployee, oReqState)
                            }
                        Dim tmpList = lrret.Rankings.RequestEmployeeRankingPositions.ToList()
                        tmpList.Add(oMyPosition)
                        lrret.Rankings.RequestEmployeeRankingPositions = tmpList.ToArray
                    End If

                    lrret.ActualPosition = oMyPosition.IDPosition
                    lrret.RankingEnabled = True

                    lrret.Coverages = Requests.roRequest.GetCoverageOnEmployeeDate(IdEmployee, xDate, oReqState)
                    lrret.CoverageEnabled = True
                Else
                    lrret.RankingEnabled = False
                    lrret.CoverageEnabled = False
                End If

                If oReqState.Result <> RequestResultEnum.NoError Then
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                Else
                    lrret.Status = ErrorCodes.OK
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::RequestsHelper::DeleteRequest")
            End Try

            Return lrret
        End Function

    End Class

End Namespace