Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Incidence
Imports Robotics.Base.VTDailyRecord
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTNotifications
Imports Robotics.Base.VTRequests
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTPortal

    Public Class ShiftsHelper

        Public Shared Function GetEmployeeHolidaysInfo(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal oState As Shift.roShiftState) As HolidaysInfo
            Dim lrret As New HolidaysInfo

            Try
                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)

                If oPermList.Schedule.QuerySchedule Then

                    Dim oEmpState As New Employee.roEmployeeState()
                    roBusinessState.CopyTo(oState, oEmpState)

                    Dim tbShifts As DataTable = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(idEmployee, oState)
                    Dim dv As New DataView(tbShifts)
                    dv.RowFilter = "ShiftType = 2"
                    dv.Sort = "Name"
                    tbShifts = dv.ToTable

                    ' Obtenemos los diferentes saldos utilizados en los horarios de vacaciones obtenidos
                    Dim strIDShifts As String = "-1"
                    For Each cRow As DataRow In tbShifts.Rows
                        strIDShifts &= "," & cRow("ID")
                    Next
                    Dim strSQL As String = "@SELECT#  DISTINCT ID, Name , (@SELECT# TOP 1 ID FROM Shifts WHERE IDConceptBalance=  Concepts.ID AND ShiftType = 2 ) as IDShift FROM Concepts WHERE ID IN(@SELECT# IDConceptBalance FROM Shifts WHERE ID IN(" & strIDShifts & ") AND isnull(IDConceptBalance,0) > 0 AND ShiftType = 2) "
                    Dim tbConcepts As DataTable = CreateDataTable(strSQL)
                    Dim dvConcepts As New DataView(tbConcepts)
                    dvConcepts.Sort = "Name"
                    tbConcepts = dvConcepts.ToTable

                    Dim vacInfoShifts As New Generic.List(Of VacationResumeShifts)
                    For Each cRow As DataRow In tbConcepts.Rows
                        Dim cInfoShift As New VacationResumeShifts()
                        Dim VacationsResumeValue() As Double = {0, 0, 0, 0, 0, 0, 0}

                        ' Obtenemos información resumen días vacaciones
                        VTBusiness.Common.roBusinessSupport.VacationsResumeQuery(idEmployee, roTypes.Any2Integer(cRow("IDShift")), Now.Date, Now.Date, Now.Date, Now.Date, VacationsResumeValue(0), VacationsResumeValue(1), VacationsResumeValue(2), VacationsResumeValue(3), oEmpState, VacationsResumeValue(5), VacationsResumeValue(6))
                        VacationsResumeValue(4) = VacationsResumeValue(3) - (VacationsResumeValue(2) + VacationsResumeValue(1) + VacationsResumeValue(5) + VacationsResumeValue(6))

                        Dim oGroupList As New Generic.List(Of HolidayGroupInfo)

                        Dim oHolidayGroupInfo As New HolidayGroupInfo
                        oHolidayGroupInfo.Description = "Done"
                        oHolidayGroupInfo.Value = FormatNumber(roTypes.Any2Double(VacationsResumeValue(0)), 3)
                        oGroupList.Add(oHolidayGroupInfo)

                        oHolidayGroupInfo = New HolidayGroupInfo
                        oHolidayGroupInfo.Description = "Pending"
                        oHolidayGroupInfo.Value = FormatNumber(roTypes.Any2Double(VacationsResumeValue(1)), 3)
                        oGroupList.Add(oHolidayGroupInfo)

                        oHolidayGroupInfo = New HolidayGroupInfo
                        oHolidayGroupInfo.Description = "Lasting"
                        oHolidayGroupInfo.Value = FormatNumber(roTypes.Any2Double(VacationsResumeValue(2)), 3)
                        oGroupList.Add(oHolidayGroupInfo)

                        oHolidayGroupInfo = New HolidayGroupInfo
                        oHolidayGroupInfo.Description = "Available"
                        oHolidayGroupInfo.Value = FormatNumber(roTypes.Any2Double(VacationsResumeValue(3)), 3)
                        oGroupList.Add(oHolidayGroupInfo)

                        oHolidayGroupInfo = New HolidayGroupInfo
                        oHolidayGroupInfo.Description = "Prevision"
                        oHolidayGroupInfo.Value = FormatNumber(roTypes.Any2Double(VacationsResumeValue(4)), 3)
                        oGroupList.Add(oHolidayGroupInfo)

                        cInfoShift.key = roTypes.Any2String(cRow("Name"))
                        cInfoShift.items = oGroupList.ToArray

                        vacInfoShifts.Add(cInfoShift)
                    Next

                    Dim oCauseState As New Cause.roCauseState()
                    roBusinessState.CopyTo(oState, oCauseState)

                    Dim tbCauses As DataTable = Cause.roCause.GetCausesByEmployeeVisibilityPermissions(idEmployee, oCauseState)
                    Dim dvCauses As New DataView(tbCauses)
                    dvCauses.RowFilter = "IsHoliday = 1"
                    dvCauses.Sort = "Name"
                    tbCauses = dvCauses.ToTable

                    For Each cRow As DataRow In tbCauses.Rows
                        Dim cInfoShift As New VacationResumeShifts()
                        Dim VacationsResumeValue() As Double = {0, 0, 0, 0, 0}

                        ' Obtenemos información resumen días vacaciones
                        VTBusiness.Common.roBusinessSupport.ProgrammedHolidaysResumeQuery(idEmployee, roTypes.Any2Integer(cRow("ID")), Now.Date, Now.Date, Now.Date, Now.Date, VacationsResumeValue(0), VacationsResumeValue(1), VacationsResumeValue(2), oEmpState)
                        VacationsResumeValue(4) = VacationsResumeValue(2) - (VacationsResumeValue(0) + VacationsResumeValue(1))

                        Dim oGroupList As New Generic.List(Of HolidayGroupInfo)

                        Dim oHolidayGroupInfo As New HolidayGroupInfo
                        oHolidayGroupInfo.Description = "Pending"
                        oHolidayGroupInfo.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(VacationsResumeValue(0)))
                        oGroupList.Add(oHolidayGroupInfo)

                        oHolidayGroupInfo = New HolidayGroupInfo
                        oHolidayGroupInfo.Description = "Lasting"
                        oHolidayGroupInfo.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(VacationsResumeValue(1)))
                        oGroupList.Add(oHolidayGroupInfo)

                        oHolidayGroupInfo = New HolidayGroupInfo
                        oHolidayGroupInfo.Description = "Available"
                        oHolidayGroupInfo.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(VacationsResumeValue(2)))
                        oGroupList.Add(oHolidayGroupInfo)

                        oHolidayGroupInfo = New HolidayGroupInfo
                        oHolidayGroupInfo.Description = "Prevision"
                        oHolidayGroupInfo.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(VacationsResumeValue(4)))
                        oGroupList.Add(oHolidayGroupInfo)

                        cInfoShift.key = roTypes.Any2String(cRow("Name"))
                        cInfoShift.items = oGroupList.ToArray

                        vacInfoShifts.Add(cInfoShift)
                    Next

                    lrret.HolidaysGroup = vacInfoShifts.ToArray()
                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.HolidaysGroup = {}
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                lrret.HolidaysGroup = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::GetEmployeeHolidaysInfo")
            End Try

            Return lrret
        End Function

        Public Shared Function DeleteProgrammedAbsence(ByVal forecastId As Integer, ByVal forecastType As String, ByRef oReqState As Absence.roProgrammedAbsenceState) As StdRequestResponse
            Dim lrret As New StdRequestResponse

            Try
                lrret.Result = True
                lrret.Status = ErrorCodes.OK

                Select Case forecastType
                    Case "days"
                        Dim oProgrammedAbsence As New Absence.roProgrammedAbsence(forecastId, oReqState)
                        Dim requestId As Integer
                        Dim requestDate As Date = Date.Now
                        Dim bSendNotificationOnRequest As Boolean = False

                        If (oProgrammedAbsence IsNot Nothing AndAlso oProgrammedAbsence.BeginDate > Date.Now.AddDays(1)) Then

                            If oProgrammedAbsence.RequestId IsNot Nothing AndAlso oProgrammedAbsence.RequestId > 0 Then
                                Dim oRequestState As New Requests.roRequestState
                                Dim oRequest As New Requests.roRequest(oProgrammedAbsence.RequestId, oRequestState)
                                requestId = oRequest.ID
                                requestDate = oRequest.RequestDate
                                oRequest.RequestStatus = eRequestStatus.Canceled
                                oRequest.Save(True, oReqState.Language.Translate("DeleteProgrammedAbsence.CanceledByUser", ""))
                                bSendNotificationOnRequest = True
                            Else
                                requestDate = oProgrammedAbsence.BeginDate
                            End If

                            lrret.Result = oProgrammedAbsence.Delete()
                            If Not lrret.Result Then
                                lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError
                                lrret.StatusErrorMsg = oReqState.Language.Translate("DeleteProgrammedAbsence.ErrorDelete", "")
                            Else
                                If Not bSendNotificationOnRequest Then
                                    Dim oNotificationState As New Notifications.roNotificationState(oReqState.IDPassport)
                                    If Not Notifications.roNotification.GenerateNotificationTask(False, eNotificationType.Absence_Canceled_By_User, oNotificationState, requestId, , requestDate,,, Now) Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequest::Save::Unable to create notification task for absence canceled by user id = " & requestId)
                                        lrret.Status = ErrorCodes.ERROR_CREATING_NOTIFICATION
                                    End If
                                End If

                            End If
                        Else
                            lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError
                            lrret.StatusErrorMsg = oReqState.Language.Translate("DeleteProgrammedAbsence.InvalidDate", "")
                        End If

                    Case "hours"

                        Dim oReqStateCause = New roProgrammedCauseState
                        roBusinessState.CopyTo(oReqState, oReqStateCause)
                        Dim oProgrammedCause As New roProgrammedCause(forecastId, oReqStateCause)
                        Dim bSendNotificationOnRequest As Boolean = False

                        If (oProgrammedCause IsNot Nothing AndAlso oProgrammedCause.ProgrammedDate > Date.Now.AddDays(1)) Then

                            If oProgrammedCause.RequestId IsNot Nothing AndAlso oProgrammedCause.RequestId > 0 Then
                                Dim oRequestState As New Requests.roRequestState
                                Dim oRequest As New Requests.roRequest(oProgrammedCause.RequestId, oRequestState)
                                oRequest.RequestStatus = eRequestStatus.Canceled
                                oRequest.Save(True, oReqState.Language.Translate("DeleteProgrammedAbsence.CanceledByUser", ""))
                                bSendNotificationOnRequest = True
                            End If

                            lrret.Result = oProgrammedCause.Delete()
                            If Not lrret.Result Then
                                lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError
                                lrret.StatusErrorMsg = oReqState.Language.Translate("DeleteProgrammedAbsence.ErrorDelete", "")
                            Else
                                If Not bSendNotificationOnRequest Then
                                    Dim oNotificationState As New Notifications.roNotificationState(oReqState.IDPassport)
                                    If Not Notifications.roNotification.GenerateNotificationTask(False, eNotificationType.Absence_Canceled_By_User, oNotificationState, oProgrammedCause.RequestId, , oProgrammedCause.BeginDate,,, Now) Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequest::Save::Unable to create notification task for absence canceled by user id = " & oProgrammedCause.RequestId)
                                        lrret.Status = ErrorCodes.ERROR_CREATING_NOTIFICATION
                                    End If
                                End If
                            End If
                        Else
                            lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError
                            lrret.StatusErrorMsg = oReqState.Language.Translate("DeleteProgrammedAbsence.InvalidDate", "")
                        End If

                    Case "holidayhours"

                        Dim oReqStateHolidayHours = New roProgrammedHolidayState
                        roBusinessState.CopyTo(oReqState, oReqStateHolidayHours)
                        Dim oProgrammedHolidayManager As New roProgrammedHolidayManager(oReqStateHolidayHours)

                        Dim oProgrammedAbsence = oProgrammedHolidayManager.LoadProgrammedHoliday(forecastId)
                        If (oProgrammedAbsence IsNot Nothing AndAlso oProgrammedAbsence.ProgrammedDate > Date.Now.AddDays(1)) Then

                            lrret.Result = oProgrammedHolidayManager.DeleteProgrammedHoliday(oProgrammedAbsence)
                            If Not lrret.Result Then
                                lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError
                                lrret.StatusErrorMsg = oReqState.Language.Translate("DeleteProgrammedAbsence.ErrorDelete", "")
                            End If
                        Else
                            lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError
                            lrret.StatusErrorMsg = oReqState.Language.Translate("DeleteProgrammedAbsence.InvalidDate", "")
                        End If

                    Case "overtime"

                        Dim oReqStateOvertime = New roProgrammedOvertimeState
                        roBusinessState.CopyTo(oReqState, oReqStateOvertime)
                        Dim oProgrammedOvertimeManager As New roProgrammedOvertimeManager(oReqStateOvertime)

                        Dim oProgrammedAbsence = oProgrammedOvertimeManager.LoadProgrammedOvertime(forecastId)

                        If (oProgrammedAbsence IsNot Nothing AndAlso oProgrammedAbsence.ProgrammedBeginDate > Date.Now.AddDays(1)) Then

                            If oProgrammedAbsence.RequestId IsNot Nothing AndAlso oProgrammedAbsence.RequestId > 0 Then
                                Dim oRequestState As New Requests.roRequestState
                                Dim oRequest As New Requests.roRequest(oProgrammedAbsence.RequestId, oRequestState)
                                oRequest.RequestStatus = eRequestStatus.Canceled
                                oRequest.Save(True, oReqState.Language.Translate("DeleteProgrammedAbsence.CanceledByUser", ""))
                            End If

                            lrret.Result = oProgrammedOvertimeManager.DeleteProgrammedOvertime(oProgrammedAbsence)
                            If Not lrret.Result Then
                                lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError
                                lrret.StatusErrorMsg = oReqState.Language.Translate("DeleteProgrammedAbsence.ErrorDelete", "")
                            End If
                        Else
                            lrret.Status = ErrorCodes.REQUEST_ERROR_CustomError
                            lrret.StatusErrorMsg = oReqState.Language.Translate("DeleteProgrammedAbsence.InvalidDate", "")
                        End If

                End Select
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::DeleteProgrammedAbsence")
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

        Public Shared Function GetEmployeeDayInfo(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal sDate As DateTime, ByVal oCalState As VTCalendar.roCalendarRowPeriodDataState) As EmployeeDayInfo
            Dim lrret As New EmployeeDayInfo

            Try
                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)
                If oPermList.Schedule.QuerySchedule Then
                    Dim oServerLicense As New roServerLicense()
                    Dim bHRScheduling As Boolean = oServerLicense.FeatureIsInstalled("Feature\HRScheduling")

                    Dim iDate As DateTime = sDate.Date

                    Dim oConcept As Concept.roConcept = Nothing
                    Dim oParams As New roParameters("OPTIONS")

                    Dim objID = DataLayer.AccessHelper.ExecuteScalar("@SELECT# ID FROM Concepts WHERE Description like '%#EMPLOYEEREMARK#%'")

                    If Not IsDBNull(objID) AndAlso objID IsNot Nothing Then
                        oConcept = New Concept.roConcept(roTypes.Any2Integer(objID), New Concept.roConceptState(oCalState.IDPassport))
                        If oConcept.EmployeesPremission = 2 AndAlso oConcept.EmployeesConditions IsNot Nothing AndAlso oConcept.EmployeesConditions.Count > 0 Then
                            Dim strSQLFilter As String = "Employees.ID = " & idEmployee.ToString
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oConcept.EmployeesConditions
                                strSQLFilter &= " AND " & oCondition.GetFilter(idEmployee)
                            Next
                            Dim oEmpState As New Employee.roEmployeeState(oCalState.IDPassport)
                            Dim tbEmployees As DataSet = VTBusiness.Common.roBusinessSupport.GetEmployeeFromList(idEmployee.ToString, oEmpState, "", "", strSQLFilter)
                            If tbEmployees.Tables(0).Rows.Count = 0 Then
                                oConcept = Nothing
                            End If
                        ElseIf oConcept.EmployeesPremission = 1 Then
                            oConcept = Nothing
                        End If

                    End If

                    lrret.DayInfo = VTCalendar.roCalendarRowPeriodDataManager.LoadCellsByCalendar(iDate, iDate, idEmployee, -1, 3, oParams, CalendarView.Planification, CalendarDetailLevel.Daily,
                                                                                                  oConcept, Nothing, Nothing, oCalState, bHRScheduling,,,, True)

                    Dim oProgAbsenceState As New Absence.roProgrammedAbsenceState()
                    Dim oProgCauseState As New Incidence.roProgrammedCauseState()
                    Dim oProgrammedHolidaysState As New VTHolidays.roProgrammedHolidayState
                    Dim oProgrammedOvertimesState As New VTHolidays.roProgrammedOvertimeState

                    roBusinessState.CopyTo(oCalState, oProgAbsenceState)
                    roBusinessState.CopyTo(oCalState, oProgCauseState)
                    roBusinessState.CopyTo(oCalState, oProgrammedHolidaysState)
                    roBusinessState.CopyTo(oCalState, oProgrammedOvertimesState)

                    Dim oProgrammedHolidayManager As New VTHolidays.roProgrammedHolidayManager(oProgrammedHolidaysState)
                    Dim oProgrammedOvertimeManager As New VTHolidays.roProgrammedOvertimeManager(oProgrammedOvertimesState)

                    Dim dtProgAbsences As DataTable = Absence.roProgrammedAbsence.GetProgrammedAbsences(idEmployee, iDate, iDate, oProgAbsenceState)
                    Dim dtProgCauses As DataTable = Incidence.roProgrammedCause.GetProgrammedCauses(idEmployee, iDate, iDate, oProgCauseState)
                    Dim lstProgrammedHolidays As Generic.List(Of roProgrammedHoliday) = oProgrammedHolidayManager.GetProgrammedHolidays(idEmployee, oProgrammedHolidaysState).FindAll(Function(x) x.ProgrammedDate = iDate)
                    Dim lstProgrammedOvertimes As Generic.List(Of roProgrammedOvertime) = oProgrammedOvertimeManager.GetProgrammedOvertimes(idEmployee, oProgrammedOvertimesState).FindAll(Function(x) (iDate >= x.ProgrammedBeginDate AndAlso iDate <= x.ProgrammedEndDate))

                    Dim oDocState As New VTDocuments.roDocumentState()
                    Dim oTmpList As New Generic.List(Of EmployeeForecast)
                    roBusinessState.CopyTo(oCalState, oDocState)

                    Dim oDocumentManager As New VTDocuments.roDocumentManager(oDocState)
                    If dtProgAbsences IsNot Nothing AndAlso dtProgAbsences.Rows.Count > 0 Then
                        For Each oRow As DataRow In dtProgAbsences.Rows
                            Dim oTmpForecast As New EmployeeForecast

                            With oTmpForecast
                                .BeginDate = roTypes.Any2DateTime(oRow("BeginDate"))
                                .FinishDate = roTypes.Any2DateTime(oRow("RealFinishDate"))
                                .IdCause = roTypes.Any2Integer(oRow("IDCause"))
                                .IdEmployee = roTypes.Any2Integer(oRow("IDEmployee"))
                                .Description = roTypes.Any2String(oRow("Description"))
                                .Cause = roTypes.Any2String(oRow("Name"))
                                .ForecastID = roTypes.Any2Integer(oRow("AbsenceID"))
                                'Obtengo los documentos faltantes para empleados
                                .DocAlerts = oDocumentManager.GetForecastDocumentationFaultAlerts(.IdEmployee, DocumentType.Employee, .ForecastID, ForecastType.AbsenceDays, False)
                                .ForecastType = "days"
                                .HasDocuments = roTypes.Any2Boolean(oRow("HasDocuments"))
                            End With

                            oTmpList.Add(oTmpForecast)
                        Next
                    End If

                    If dtProgCauses IsNot Nothing AndAlso dtProgCauses.Rows.Count > 0 Then
                        For Each oRow As DataRow In dtProgCauses.Rows
                            Dim oTmpForecast As New EmployeeForecast

                            Dim descripcion = oReqState.Language.Translate("AbsenceDescription.BeginTime", "") & " " & roTypes.Any2DateTime(oRow("BeginTime")).ToString("HH':'mm") & " " & oReqState.Language.Translate("AbsenceDescription.EndTime", "") & " " & roTypes.Any2DateTime(oRow("EndTime")).ToString("HH':'mm")

                            With oTmpForecast
                                .BeginDate = roTypes.Any2DateTime(oRow("Date"))
                                .FinishDate = roTypes.Any2DateTime(oRow("FinishDate"))
                                .IdCause = roTypes.Any2Integer(oRow("IDCause"))
                                .IdEmployee = roTypes.Any2Integer(oRow("IDEmployee"))
                                .Description = roTypes.Any2String(oRow("Description"))
                                .ForecastDetail = descripcion
                                .Cause = roTypes.Any2String(oRow("Name"))
                                .ForecastID = roTypes.Any2Integer(oRow("AbsenceID"))
                                'Obtengo los documentos faltantes para empleados
                                .DocAlerts = oDocumentManager.GetForecastDocumentationFaultAlerts(.IdEmployee, DocumentType.Employee, .ForecastID, ForecastType.AbsenceHours, False)
                                .ForecastType = "hours"
                                .HasDocuments = roTypes.Any2Boolean(oRow("HasDocuments"))
                            End With

                            oTmpList.Add(oTmpForecast)
                        Next
                    End If

                    For Each oProgHoliday As roProgrammedHoliday In lstProgrammedHolidays
                        Dim oTmpForecast As New EmployeeForecast

                        Dim descripcion = oReqState.Language.Translate("AbsenceDescription.BeginTime", "") & " " & roTypes.Any2DateTime(oProgHoliday.BeginTime).ToString("HH':'mm") & " " & oReqState.Language.Translate("AbsenceDescription.EndTime", "") & " " & roTypes.Any2DateTime(oProgHoliday.EndTime).ToString("HH':'mm")

                        With oTmpForecast
                            .BeginDate = oProgHoliday.ProgrammedDate
                            .FinishDate = oProgHoliday.ProgrammedDate
                            .IdCause = oProgHoliday.IDCause
                            .IdEmployee = oProgHoliday.IDEmployee
                            .Description = oProgHoliday.Description
                            .Cause = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgHoliday.IDCause))
                            .ForecastID = oProgHoliday.ID
                            .ForecastDetail = descripcion
                            'Obtengo los documentos faltantes para empleados
                            .DocAlerts = {}
                            .ForecastType = "holidayhours"
                            .HasDocuments = False
                        End With
                        oTmpList.Add(oTmpForecast)
                    Next

                    For Each oProgOvertime As roProgrammedOvertime In lstProgrammedOvertimes

                        Dim oTmpForecast As New EmployeeForecast

                        Dim descripcion = oReqState.Language.Translate("Programmedovertimedescription.BeginTime", "") & " " & roTypes.Any2DateTime(oProgOvertime.BeginTime).ToString("HH':'mm") & " " & oReqState.Language.Translate("AbsenceDescription.EndTime", "") & " " & roTypes.Any2DateTime(oProgOvertime.EndTime).ToString("HH':'mm")

                        With oTmpForecast
                            .BeginDate = oProgOvertime.ProgrammedBeginDate
                            .FinishDate = oProgOvertime.ProgrammedEndDate
                            .IdCause = oProgOvertime.IDCause
                            .IdEmployee = oProgOvertime.IDEmployee
                            .Description = oProgOvertime.Description
                            .Cause = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar("@SELECT# Name FROM Causes WHERE ID=" & oProgOvertime.IDCause))
                            .ForecastID = oProgOvertime.ID
                            .ForecastDetail = descripcion
                            'Obtengo los documentos faltantes para empleados
                            .DocAlerts = oDocumentManager.GetForecastDocumentationFaultAlerts(.IdEmployee, DocumentType.Employee, .ForecastID, ForecastType.OverWork, False)
                            .ForecastType = "overtime"
                            .HasDocuments = oProgOvertime.HasDocuments
                        End With
                        oTmpList.Add(oTmpForecast)
                    Next

                    'Esto se tiene que hacer para evitar que las fechas anteriores a 1900 se les aplique un timezone de -14 minutos
                    If lrret.DayInfo.DayData(0).MainShift IsNot Nothing Then
                        lrret.DayInfo.DayData(0).MainShift.StartHour = New DateTime(lrret.DayInfo.DayData(0).PlanDate.Year, lrret.DayInfo.DayData(0).PlanDate.Month, lrret.DayInfo.DayData(0).PlanDate.Day, lrret.DayInfo.DayData(0).MainShift.StartHour.Hour, lrret.DayInfo.DayData(0).MainShift.StartHour.Minute, 0)
                        lrret.DayInfo.DayData(0).MainShift.EndHour = New DateTime(lrret.DayInfo.DayData(0).PlanDate.Year, lrret.DayInfo.DayData(0).PlanDate.Month, lrret.DayInfo.DayData(0).PlanDate.Day, lrret.DayInfo.DayData(0).MainShift.EndHour.Hour, lrret.DayInfo.DayData(0).MainShift.EndHour.Minute, 0)
                        If lrret.DayInfo.DayData(0).MainShift.ShiftLayers > 0 Then
                            For Each oLayer In lrret.DayInfo.DayData(0).MainShift.ShiftLayersDefinition
                                oLayer.LayerStartTime = New DateTime(lrret.DayInfo.DayData(0).PlanDate.Year, lrret.DayInfo.DayData(0).PlanDate.Month, lrret.DayInfo.DayData(0).PlanDate.Day, oLayer.LayerStartTime.Hour, oLayer.LayerStartTime.Minute, 0)
                            Next
                        End If
                    End If
                    'VariantType hola = New DateTimeOffset()
                    'Fin aproximación

                    lrret.Forecasts = oTmpList.ToArray
                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::GetEmployeeDayInfo")
            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeCalendar(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal sDate As DateTime, ByVal oCalState As VTCalendar.roCalendarRowPeriodDataState, Optional ByVal onlySelectedDate As Boolean = False, Optional ByVal forTCInfo As Boolean = False) As EmployeeCalendar
            Dim lrret As New EmployeeCalendar

            Try
                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)
                If oPermList.Schedule.QuerySchedule Then
                    Dim oServerLicense As New roServerLicense()
                    Dim bHRScheduling As Boolean = oServerLicense.FeatureIsInstalled("Feature\HRScheduling")

                    Dim iDate As DateTime
                    Dim eDate As DateTime

                    If onlySelectedDate Then
                        If forTCInfo Then
                            iDate = sDate
                            eDate = sDate.AddDays(1)
                        Else
                            iDate = sDate
                            eDate = sDate
                        End If
                    Else
                        iDate = sDate.AddDays((sDate.Day - 1) * -1)
                        eDate = iDate.AddMonths(1).AddDays(-1)
                        'iDate = iDate.AddMonths(-2)
                        'eDate = eDate.AddMonths(+2)
                    End If

                    Dim oConcept As Concept.roConcept = Nothing
                    Dim oParams As New roParameters("OPTIONS")

                    Dim objID = DataLayer.AccessHelper.ExecuteScalar("@SELECT# ID FROM Concepts WHERE Description like '%#EMPLOYEEREMARK#%'")

                    If Not IsDBNull(objID) AndAlso objID IsNot Nothing Then
                        oConcept = New Concept.roConcept(roTypes.Any2Integer(objID), New Concept.roConceptState(oCalState.IDPassport))
                        If oConcept.EmployeesPremission = 2 AndAlso oConcept.EmployeesConditions IsNot Nothing AndAlso oConcept.EmployeesConditions.Count > 0 Then
                            Dim strSQLFilter As String = "Employees.ID = " & idEmployee.ToString
                            For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oConcept.EmployeesConditions
                                strSQLFilter &= " AND " & oCondition.GetFilter(idEmployee)
                            Next
                            Dim oEmpState As New Employee.roEmployeeState(oCalState.IDPassport)
                            Dim tbEmployees As DataSet = VTBusiness.Common.roBusinessSupport.GetEmployeeFromList(idEmployee.ToString, oEmpState, "", "", strSQLFilter)
                            If tbEmployees.Tables(0).Rows.Count = 0 Then
                                oConcept = Nothing
                            End If
                        ElseIf oConcept.EmployeesPremission = 1 Then
                            oConcept = Nothing
                        End If
                    End If

                    lrret.oCalendar = VTCalendar.roCalendarRowPeriodDataManager.LoadCellsByCalendar(iDate, eDate, idEmployee, -1, 3, oParams,
                                                                                                                  CalendarView.Planification, CalendarDetailLevel.Daily, oConcept, Nothing, Nothing,
                                                                                                                  oCalState, bHRScheduling,,,, True, , , , , , , , , , , onlySelectedDate)

                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.oCalendar = New roCalendarRowPeriodData
                    lrret.oCalendar.DayData = {}
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                lrret.oCalendar = New roCalendarRowPeriodData
                lrret.oCalendar.DayData = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::GetEmployeeCalendar")
            End Try

            Return lrret
        End Function

        Public Shared Function GetCurrentCapacityOnDate(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal sDate As DateTime, ByVal eDate As DateTime, ByVal oCalState As VTCalendar.roCalendarRowPeriodDataState) As CurrentCapacity
            Dim lrret As New CurrentCapacity

            Try
                Dim calendar = New roCalendar
                calendar.FirstDay = sDate.Date
                calendar.LastDay = eDate.Date
                Dim calendarManager As New VTCalendar.roCalendarManager(New VTCalendar.roCalendarState(-1))
                calendar = calendarManager.Load(sDate, eDate, "B" & idEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, True,,,,,, True)

                lrret.capacities = New roCalendarCapacities

                If calendar.CalendarHeader IsNot Nothing AndAlso calendar.CalendarHeader.PeriodSeatingCapacityData IsNot Nothing Then
                    For Each capacity In calendar.CalendarHeader.PeriodSeatingCapacityData
                        For Each cap In capacity.Capacities
                            lrret.capacities.CurrentSeating = cap.CurrentSeating
                            lrret.capacities.MaxSeatingCapacity = cap.MaxSeatingCapacity
                            lrret.capacities.ZoneName = cap.ZoneName
                            lrret.capacities.ZoneCapacityVisible = cap.ZoneCapacityVisible
                        Next
                    Next
                End If

                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                lrret.capacities = New roCalendarCapacities
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::GetCurrentCapacityOnDate")
            End Try
            Return lrret
        End Function

        Public Shared Function GetEmployeePlan(ByVal idPassport As Integer, ByVal idEmployee As Integer, ByVal sDate As DateTime, ByVal oCalState As VTCalendar.roCalendarRowPeriodDataState, ByVal ProgrammedHoliday_IDCause As Integer) As EmployeeCalendar
            Dim lrret As New EmployeeCalendar

            Try

                Dim iDate As DateTime = sDate.AddDays((sDate.Day - 1) * -1)
                Dim eDate As DateTime = iDate.AddYears(1).AddMonths(1).AddDays(-1)
                lrret.oCalendar = VTCalendar.roCalendarRowPeriodDataManager.LoadSchedulerPlanByCalendar(iDate, eDate, idEmployee, oCalState, ProgrammedHoliday_IDCause)
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                lrret.oCalendar = New roCalendarRowPeriodData
                lrret.oCalendar.DayData = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::GetEmployeePlan")
            End Try
            Return lrret
        End Function

        Public Shared Function IsFloatingShift(ByVal _IDShift As Integer, ByRef oShiftState As Shift.roShiftState) As ShiftFloatingInfo
            Dim lrret As New ShiftFloatingInfo

            Try
                lrret.Status = ErrorCodes.OK

                Dim oShift As New Shift.roShift(_IDShift, oShiftState)

                If oShift IsNot Nothing Then
                    lrret.IDShift = _IDShift
                    lrret.IsFloating = (oShift.ShiftType = ShiftType.NormalFloating)
                    If oShift.StartFloating.HasValue Then
                        lrret.StartFloating = oShift.StartFloating.Value
                    Else
                        lrret.StartFloating = StatusHelper.NULL_DATE
                    End If
                End If

                If oShift.State.Result <> ShiftResultEnum.NoError Then
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::IsFloatingShift")
            End Try

            Return lrret

        End Function

        Public Shared Function GetShiftById(ByVal _IDShift As Integer, ByRef oShiftState As Shift.roShiftState) As Shift.roShift
            Dim lrret As New Shift.roShift

            Try

                Dim oShift As New Shift.roShift(_IDShift, oShiftState)
                lrret = oShift
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::IsFloatingShift")
            End Try

            Return lrret

        End Function

        Public Shared Function IsWorkingdayShift(ByVal _IDShift As Integer, ByRef oShiftState As Shift.roShiftState) As StdResponse
            Dim lrret As New StdResponse
            Try
                lrret.Status = ErrorCodes.OK
                Dim oShift As New Shift.roShift(_IDShift, oShiftState)
                If oShift IsNot Nothing Then
                    lrret.Result = oShift.AreWorkingDays
                End If
                If oShift.State.Result <> ShiftResultEnum.NoError Then
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::IsWorkingdayShift")
            End Try
            Return lrret
        End Function

        Public Shared Function GetEmployeeDailyRecordCalendar(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal dDate As DateTime) As roGenericResponse(Of roDailyRecordCalendar)
            Dim lrret As New roGenericResponse(Of roDailyRecordCalendar)

            Try
                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)

                If oPermList.DailyRecord.QueryDailyRecord Then
                    Dim iDate As DateTime
                    Dim eDate As DateTime

                    iDate = dDate.AddDays((dDate.Day - 1) * -1)
                    eDate = iDate.AddMonths(1).AddDays(-1)

                    Dim oDailyRecordManager As roDailyRecordManager = New roDailyRecordManager(New roDailyRecordState(oReqState.IDPassport))
                    lrret.Value = oDailyRecordManager.LoadDailyRecordCalendar(idEmployee, iDate, eDate, True)

                    Select Case oDailyRecordManager.State.Result
                        Case DailyRecordResultEnum.NoError
                            lrret.Status = ErrorCodes.OK
                        Case DailyRecordResultEnum.ConnectionError
                            lrret.Status = ErrorCodes.REQUEST_ERROR_ConnectionError
                        Case DailyRecordResultEnum.ErrorLoadingDailyRecordCalendar

                        Case DailyRecordResultEnum.Exception
                            lrret.Status = ErrorCodes.GENERAL_ERROR
                    End Select
                Else
                    lrret.Value = New roDailyRecordCalendar
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                lrret.Value = New roDailyRecordCalendar
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::GetEmployeeDailyRecordCalendar")
            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeDailyRecordPunchesPattern(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal dDate As DateTime) As roGenericResponse(Of roDailyRecordPunchesPattern)
            Dim lrret As New roGenericResponse(Of roDailyRecordPunchesPattern)

            Try
                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)

                If oPermList.DailyRecord.QueryDailyRecord Then

                    Dim oDailyRecordManager As roDailyRecordManager = New roDailyRecordManager(New roDailyRecordState(oReqState.IDPassport))
                    lrret.Value = oDailyRecordManager.LoadPunchesPattern(idEmployee, dDate)

                    Select Case oDailyRecordManager.State.Result
                        Case DailyRecordResultEnum.NoError
                            lrret.Status = ErrorCodes.OK
                        Case DailyRecordResultEnum.ConnectionError
                            lrret.Status = ErrorCodes.REQUEST_ERROR_ConnectionError
                        Case DailyRecordResultEnum.ErrorLoadingDailyRecordCalendar

                        Case DailyRecordResultEnum.Exception
                            lrret.Status = ErrorCodes.GENERAL_ERROR
                    End Select
                Else
                    lrret.Value = New roDailyRecordPunchesPattern
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                lrret.Value = New roDailyRecordPunchesPattern
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::GetEmployeeDailyRecordPunchesPattern")
            End Try

            Return lrret
        End Function

        Public Shared Function DeleteDailyRecord(ByVal oPassport As roPassportTicket, ByVal idDailyRecord As Integer) As StdRequestResponse
            Dim lrret As New StdRequestResponse

            Try
                lrret.Result = True
                lrret.Status = ErrorCodes.OK

                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)
                If oPermList.DailyRecord.QueryDailyRecord Then
                    Dim oDailyRecordManager As roDailyRecordManager = New roDailyRecordManager(New roDailyRecordState(oReqState.IDPassport))
                    lrret.Result = oDailyRecordManager.DeleteDailyRecord(idDailyRecord, True)
                    lrret.Status = oDailyRecordManager.GetPortalErrorCode(oDailyRecordManager.State.Result)
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ShiftsHelper::DeleteDailyRecord")
            End Try

            Return lrret
        End Function

    End Class

End Namespace