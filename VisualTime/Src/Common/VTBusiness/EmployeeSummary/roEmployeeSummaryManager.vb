Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Absence
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.Security.Base
Imports Robotics.Base.VTBusiness.Concept

Public Class roEmployeeSummaryManager

    Public Shared Function GetEmployeeAccrualSummary(ByVal IdEmployee As Integer, onDate As Date, ByRef oState As Employee.roEmployeeState) As roEmployeeAccrualsSummary

        Dim oRet As New roEmployeeAccrualsSummary

        Try

            'Me traigo el contrato activo del empleado
            Dim oContract As New Contract.roContractState : roBusinessState.CopyTo(oState, oContract)
            Dim oContractConector As Contract.roContract = Contract.roContract.GetActiveContract(IdEmployee, oContract, False)

            oRet.DailyAccruals = GetAccrualEmployeeSummary(IdEmployee, onDate, SummaryType.Daily, oContractConector, oState).ToArray
            oRet.WeekAccruals = GetAccrualEmployeeSummary(IdEmployee, onDate, SummaryType.Semanal, oContractConector, oState).ToArray
            oRet.MonthAccruals = GetAccrualEmployeeSummary(IdEmployee, onDate, SummaryType.Mensual, oContractConector, oState).ToArray
            oRet.YearAccruals = GetAccrualEmployeeSummary(IdEmployee, onDate, SummaryType.Anual, oContractConector, oState).ToArray
            oRet.ContractAccruals = GetAccrualEmployeeSummary(IdEmployee, onDate, SummaryType.Contrato, oContractConector, oState).ToArray
            oRet.YearWorkAccruals = GetAccrualEmployeeSummary(IdEmployee, onDate, SummaryType.ContractAnnualized, oContractConector, oState).ToArray
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roEmployeeSummaryManager::GetEmployeeSummary")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roEmployeeSummaryManager::GetEmployeeSummary")
        Finally

        End Try

        Return oRet
    End Function

    Public Shared Function GetEmployeeAccrualProductivSummary(ByVal IdEmployee As Integer, onDate As Date, ByRef oState As Employee.roEmployeeState) As roEmployeeAccrualsProductivSummary

        Dim oRet As New roEmployeeAccrualsProductivSummary

        Try

            'Me traigo el contrato activo del empleado
            Dim oContract As New Contract.roContractState : roBusinessState.CopyTo(oState, oContract)
            Dim oContractConector As Contract.roContract = Contract.roContract.GetActiveContract(IdEmployee, oContract, False)

            oRet.DailyAccruals = GetTasksEmployeeSummary(IdEmployee, onDate, SummaryType.Daily, oState).ToArray
            oRet.WeekAccruals = GetTasksEmployeeSummary(IdEmployee, onDate, SummaryType.Semanal, oState).ToArray
            oRet.MonthAccruals = GetTasksEmployeeSummary(IdEmployee, onDate, SummaryType.Mensual, oState).ToArray
            oRet.YearAccruals = GetTasksEmployeeSummary(IdEmployee, onDate, SummaryType.Anual, oState).ToArray
            oRet.ContractAccruals = GetTasksEmployeeSummary(IdEmployee, onDate, SummaryType.Contrato, oState).ToArray
            oRet.YearWorkAccruals = GetTasksEmployeeSummary(IdEmployee, onDate, SummaryType.ContractAnnualized, oState).ToArray
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roEmployeeSummaryManager::GetEmployeeSummary")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roEmployeeSummaryManager::GetEmployeeSummary")
        Finally

        End Try

        Return oRet
    End Function

    Public Shared Function GetEmployeeHolidaysSummary(ByVal idEmployee As Integer, ByVal oEmpState As Employee.roEmployeeState) As roEmployeeHolidaysSummary
        Dim lrret As New roEmployeeHolidaysSummary

        Try

            Dim oTmpLst As New Generic.List(Of roHolidaysSummary)

            Dim oShiftState As New Shift.roShiftState
            roBusinessState.CopyTo(oEmpState, oShiftState)

            Dim tbShifts As DataTable = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(idEmployee, oShiftState)
            Dim dv As New DataView(tbShifts)
            dv.RowFilter = "ShiftType = 2"
            dv.Sort = "Name"
            tbShifts = dv.ToTable

            ' Obtenemos los diferentes saldos utilizados en los horarios de vacaciones obtenidos
            Dim strIDShifts As String = "-1"
            For Each cRow As DataRow In tbShifts.Rows
                strIDShifts &= "," & cRow("ID")
            Next
            Dim strSQL As String = "@SELECT#  DISTINCT ID, Name , DefaultQuery, (@SELECT# TOP 1 ID FROM Shifts WHERE IDConceptBalance=  Concepts.ID AND ShiftType = 2 ) as IDShift FROM Concepts WHERE ID IN(@SELECT# IDConceptBalance FROM Shifts WHERE ID IN(" & strIDShifts & ") AND isnull(IDConceptBalance,0) > 0 AND ShiftType = 2) "
            Dim tbConcepts As DataTable = CreateDataTable(strSQL, )
            Dim dvConcepts As New DataView(tbConcepts)
            dvConcepts.Sort = "Name"
            tbConcepts = dvConcepts.ToTable

            For Each cRow As DataRow In tbConcepts.Rows
                Dim cInfoShift As New roHolidaysSummary()
                Dim VacationsResumeValue() As Double = {0, 0, 0, 0, 0, 0, 0}

                ' Obtenemos información resumen días vacaciones
                roBusinessSupport.VacationsResumeQuery(idEmployee, roTypes.Any2Integer(cRow("IDShift")), Now.Date, Now.Date, Now.Date, Now.Date, VacationsResumeValue(0), VacationsResumeValue(1), VacationsResumeValue(2), VacationsResumeValue(3), oEmpState, VacationsResumeValue(5), VacationsResumeValue(6))
                VacationsResumeValue(4) = VacationsResumeValue(3) - (VacationsResumeValue(2) + VacationsResumeValue(1) + VacationsResumeValue(5) + VacationsResumeValue(6))

                cInfoShift.Name = roTypes.Any2String(cRow("Name"))
                cInfoShift.AccrualDefaultQuery = roTypes.Any2String(cRow("DefaultQuery"))
                cInfoShift.IDShift = roTypes.Any2Integer(cRow("Id"))
                cInfoShift.IDCause = -1

                cInfoShift.Done = roTypes.Any2Double(VacationsResumeValue(0))
                cInfoShift.Pending = roTypes.Any2Double(VacationsResumeValue(1))
                cInfoShift.Lasting = roTypes.Any2Double(VacationsResumeValue(2))
                cInfoShift.Available = roTypes.Any2Double(VacationsResumeValue(3))
                cInfoShift.Prevision = roTypes.Any2Double(VacationsResumeValue(4))
                cInfoShift.Expired = roTypes.Any2Double(VacationsResumeValue(5))
                cInfoShift.WithoutEnjoynment = roTypes.Any2Double(VacationsResumeValue(6))
                cInfoShift.ValueFormat = "O"


                oTmpLst.Add(cInfoShift)
            Next

            Dim oCauseState As New Cause.roCauseState()
            roBusinessState.CopyTo(oEmpState, oCauseState)

            Dim tbCauses As DataTable = Cause.roCause.GetCausesByEmployeeVisibilityPermissions(idEmployee, oCauseState)
            Dim dvCauses As New DataView(tbCauses)
            dvCauses.RowFilter = "IsHoliday = 1"
            dvCauses.Sort = "Name"
            tbCauses = dvCauses.ToTable

            For Each cRow As DataRow In tbCauses.Rows
                Dim cInfoShift As New roHolidaysSummary()
                Dim VacationsResumeValue As Double() = {0, 0, 0, 0}
                Dim maxValue As Double = 0
                ' Obtenemos información resumen vacaciones por horas
                roBusinessSupport.ProgrammedHolidaysResumeQuery(idEmployee, roTypes.Any2Integer(cRow("ID")), Now.Date, Now.Date, Now.Date, Now.Date, VacationsResumeValue(0), VacationsResumeValue(1), VacationsResumeValue(2), oEmpState, maxValue)
                VacationsResumeValue(3) = VacationsResumeValue(2) - (VacationsResumeValue(0) + VacationsResumeValue(1))

                cInfoShift.Name = roTypes.Any2String(cRow("Name"))
                cInfoShift.IDCause = roTypes.Any2Integer(cRow("Id"))
                cInfoShift.IDShift = -1

                cInfoShift.Pending = roTypes.Any2Double(VacationsResumeValue(0))
                cInfoShift.Lasting = roTypes.Any2Double(VacationsResumeValue(1))
                cInfoShift.Available = roTypes.Any2Double(VacationsResumeValue(2))
                cInfoShift.Prevision = roTypes.Any2Double(VacationsResumeValue(3))
                cInfoShift.ValueFormat = "H"
                If maxValue <> 0 Then
                    cInfoShift.Done = maxValue - cInfoShift.Available
                Else
                    cInfoShift.Done = -1
                End If

                oTmpLst.Add(cInfoShift)
            Next

            lrret.HolidaysInfo = oTmpLst.ToArray
        Catch ex As Exception
            oEmpState.UpdateStateInfo(ex, "roEmployeeSummaryManager::GetEmployeeSummary")
        End Try

        Return HumanizeEmployeeHolidaySummaryValues(lrret)
    End Function

    Private Shared Function HumanizeEmployeeHolidaySummaryValues(employeeHolidaysSummary As roEmployeeHolidaysSummary) As roEmployeeHolidaysSummary
        Dim retvalue As roEmployeeHolidaysSummary = employeeHolidaysSummary
        Try
            For Each cInfoShift As roHolidaysSummary In retvalue.HolidaysInfo
                cInfoShift.Done = HumanizeDoubleValue(cInfoShift.Done)
                cInfoShift.Pending = HumanizeDoubleValue(cInfoShift.Pending)
                cInfoShift.Lasting = HumanizeDoubleValue(cInfoShift.Lasting)
                cInfoShift.Available = HumanizeDoubleValue(cInfoShift.Available)
                cInfoShift.Prevision = HumanizeDoubleValue(cInfoShift.Prevision)
                cInfoShift.Expired = HumanizeDoubleValue(cInfoShift.Expired)
                cInfoShift.WithoutEnjoynment = HumanizeDoubleValue(cInfoShift.WithoutEnjoynment)
            Next
        Catch ex As Exception
            'Do nothing
        End Try
        Return retvalue
    End Function

    Private Shared Function HumanizeDoubleValue(value As Double) As Double
        If value = Math.Truncate(value) Then
            Return roTypes.Any2Integer(value)
        Else
            Return Math.Round(value, 3)
        End If
    End Function

    Public Shared Function GetEmployeeSummary(ByVal IdEmployee As Integer, onDate As Date, accrualSummaryType As SummaryType, causesSummaryType As SummaryType, tasksSummaryType As SummaryType, centersSummaryType As SummaryType, requestType As SummaryRequestType, ByRef oState As Employee.roEmployeeState) As roEmployeeSummary
        Dim oRet As roEmployeeSummary
        oRet = New roEmployeeSummary

        Try

            Dim presenceDt As roPunch = Nothing

#Region "Seccion Último Fichaje"

            If requestType = SummaryRequestType.All OrElse requestType = SummaryRequestType.Punch OrElse requestType = SummaryRequestType.Schedule Then
                'Último Fichaje
                Dim oPunchState As New roPunchState : roBusinessState.CopyTo(oState, oPunchState)
                Dim lastPunchDt = roPunch.GetEmployeeLastPunchSummary(IdEmployee, oPunchState)
                oRet.employeeLastPunch = DataTableToLastPunchSummary(lastPunchDt)

                If presenceDt Is Nothing Then presenceDt = roPunch.GetLastPunchPres(IdEmployee, oState)
                If (presenceDt IsNot Nothing) Then
                    oRet.employeeLastPunch.PunchPresenceType = presenceDt.ActualType
                End If
            End If

#End Region

#Region "Planificación"

            If requestType = SummaryRequestType.All OrElse requestType = SummaryRequestType.Schedule OrElse requestType = SummaryRequestType.Punch Then
                'Estado actual de la planificación
                Dim oPunchState As New roPunchState : roBusinessState.CopyTo(oState, oPunchState)
                Dim oProgrammedAbsenceState As New roProgrammedAbsenceState : roBusinessState.CopyTo(oState, oPunchState)
                ''Ultimo Fichaje
                If presenceDt Is Nothing Then presenceDt = roPunch.GetLastPunchPres(IdEmployee, oState)
                ''Última Ausencia
                Dim tbAbsences As DataTable = roProgrammedAbsence.GetProgrammedAbsences(IdEmployee, oProgrammedAbsenceState)
                Dim tbCauses As DataTable = roProgrammedAbsence.GetProgrammedCauses(IdEmployee, Date.Now, Date.Now, oProgrammedAbsenceState)
                ''Horario Actual
                Dim tbPlan = Scheduler.roScheduler.GetPlan(IdEmployee, Date.Now, Date.Now, oState)
                oRet.employeePlanification = GetPlanificationSummary(presenceDt, tbAbsences, tbCauses, tbPlan)
            End If

#End Region

            Dim oContract As Contract.roContract = Nothing

            If (requestType = SummaryRequestType.All OrElse requestType = SummaryRequestType.Accruals OrElse requestType = SummaryRequestType.Causes) Then
                'Me traigo el contrato activo del empleado
                Dim oContractState As New Contract.roContractState(-1)
                roBusinessState.CopyTo(oState, oContractState)

                oContract = Contract.roContract.GetActiveContract(IdEmployee, oContractState, False)

                If oContract Is Nothing Then oContract = Contract.roContract.GetLastContract(IdEmployee, oContractState, False)
            End If

#Region "Saldos"

            If requestType = SummaryRequestType.All OrElse requestType = SummaryRequestType.Accruals Then
                oRet.employeeAccruals = GetAccrualEmployeeSummary(IdEmployee, onDate, accrualSummaryType, oContract, oState).ToArray
            End If

#End Region

#Region "Justificaciones"

            If requestType = SummaryRequestType.All OrElse requestType = SummaryRequestType.Causes Then
                oRet.employeeCauses = GetCausesEmployeeSummary(IdEmployee, onDate, causesSummaryType, oContract, oState).ToArray
            End If

#End Region

#Region "Tareas"

            If requestType = SummaryRequestType.All OrElse requestType = SummaryRequestType.Tasks Then
                oRet.employeeTasks = GetTasksEmployeeSummary(IdEmployee, onDate, tasksSummaryType, oState).ToArray
            End If

#End Region

#Region "Centros de Coste"

            If requestType = SummaryRequestType.All OrElse requestType = SummaryRequestType.CostCenter Then
                oRet.employeeBussinessCenters = GetBussinessCentersEmployeeSummary(IdEmployee, onDate, centersSummaryType, oState).ToArray
            End If

#End Region

        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roEmployeeSummaryManager::GetEmployeeSummary")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roEmployeeSummaryManager::GetEmployeeSummary")
        End Try

        Return oRet
    End Function

    Public Shared Function DataTableToLastPunchSummary(lastPunchDt As DataTable) As roLastPunchSummary
        If (lastPunchDt IsNot Nothing And lastPunchDt.Rows.Count > 0) Then
            Dim lastPunchSummary = New roLastPunchSummary With
                {
                    .IdPunch = If(Not IsDBNull(lastPunchDt.Rows(0)("ID")), roTypes.Any2Integer(lastPunchDt.Rows(0)("ID")), -1),
                    .PunchType = If(Not IsDBNull(lastPunchDt.Rows(0)("ActualType")), roTypes.Any2Integer(lastPunchDt.Rows(0)("ActualType")), 0),
                    .PunchDateTime = If(Not IsDBNull(lastPunchDt.Rows(0)("DateTime")), roTypes.Any2DateTime(lastPunchDt.Rows(0)("DateTime")), CType(Nothing, Date?)),
                    .PunchTerminal = If(Not IsDBNull(lastPunchDt.Rows(0)("Terminal")), roTypes.Any2String(lastPunchDt.Rows(0)("Terminal")), String.Empty),
                    .PunchZone = If(Not IsDBNull(lastPunchDt.Rows(0)("Zone")), roTypes.Any2String(lastPunchDt.Rows(0)("Zone")), String.Empty),
                    .PunchLocation = If(Not IsDBNull(lastPunchDt.Rows(0)("Location")), roTypes.Any2String(lastPunchDt.Rows(0)("Location")), String.Empty),
                    .PunchLocationZone = If(Not IsDBNull(lastPunchDt.Rows(0)("LocationZone")), roTypes.Any2String(lastPunchDt.Rows(0)("LocationZone")), String.Empty),
                    .PunchImage = If(Not IsDBNull(lastPunchDt.Rows(0)("Capture")), DirectCast(lastPunchDt.Rows(0)("Capture"), Byte()), Nothing),
                    .TaskName = If(Not IsDBNull(lastPunchDt.Rows(0)("Name")), roTypes.Any2String(lastPunchDt.Rows(0)("Name")), String.Empty),
                    .HasPhoto = roTypes.Any2Boolean(lastPunchDt.Rows(0)("HasPhoto")),
                    .PhotoOnAzure = roTypes.Any2Boolean(lastPunchDt.Rows(0)("PhotoOnAzure"))
                }
            Return lastPunchSummary
        End If
        Return Nothing

    End Function

    Public Shared Function GetPlanificationSummary(tbLastPunc As roPunch, tbAbsences As DataTable, tbCauses As DataTable, tbPlan As DataTable) As roPlanificationSummary
        Dim roPlanification As New roPlanificationSummary
        roPlanification.EmployeeState = EmployeeState.Unplanned


        Dim oParams As New List(Of String)

        'Validamos el horario y si está en vacaciones
        If tbPlan IsNot Nothing AndAlso tbPlan.Rows.Count > 0 Then
            Dim oPlanification As DataRow = tbPlan.Rows(0)
            Dim IsHolidays = roTypes.Any2Boolean(oPlanification("IsHolidays"))
            roPlanification.Shift = roTypes.Any2String(oPlanification("NameUsedShift"))
            roPlanification.Asiggment = roTypes.Any2String(oPlanification("AssignmentName"))
            If (IsHolidays) Then
                roPlanification.EmployeeState = EmployeeState.Vacation
                Return roPlanification
            End If
            roPlanification.EmployeeState = EmployeeState.Planned
        End If

        'Tiene una ausencia prevista
        If tbAbsences IsNot Nothing AndAlso tbAbsences.Rows.Count > 0 Then
            Dim oAbsence() As DataRow = tbAbsences.Select("BeginDate <= '" & Format(Now.Date, "yyyy/MM/dd") & "' AND " &
                                         "RealFinishDate >= '" & Format(Now.Date, "yyyy/MM/dd") & "'",
                                         "BeginDate DESC")
            If oAbsence.Length > 0 Then
                oParams.Add(oAbsence(0)("Name"))
                oParams.Add(CDate(oAbsence(0)("BeginDate")))
                oParams.Add(CDate(oAbsence(0)("RealFinishDate")))
                roPlanification.Params = oParams
                roPlanification.EmployeeState = EmployeeState.ProgrammedAbsence
                Return roPlanification
            End If
        End If

        'Tiene una ausencia prevista por horas
        If tbCauses IsNot Nothing AndAlso tbCauses.Rows.Count > 0 Then
            Dim oCause() As DataRow = tbCauses.Select("Date <= '" & Format(Now.Date, "yyyy/MM/dd") & "' AND " &
                                         "Date >= '" & Format(Now.Date, "yyyy/MM/dd") & "'",
                                         "Date DESC")
            If oCause.Length > 0 Then
                oParams.Add(oCause(0)("Name"))
                oParams.Add(CDate(oCause(0)("Date")))
                oParams.Add(CDate(oCause(0)("BeginTime")))
                oParams.Add(CDate(oCause(0)("EndTime")))
                roPlanification.Params = oParams
                roPlanification.EmployeeState = EmployeeState.ProgrammedCause
                Return roPlanification
            End If
        End If

        'Presencia
        If tbLastPunc IsNot Nothing Then
            oParams.Add(tbLastPunc.DateTime.Value.Date)
            oParams.Add(Format(tbLastPunc.DateTime.Value, "HH:mm"))
            If tbLastPunc.ActualType.Equals(PunchTypeEnum._IN) Then roPlanification.EmployeeState = EmployeeState.PresenceIn
            If tbLastPunc.ActualType.Equals(PunchTypeEnum._OUT) Then roPlanification.EmployeeState = EmployeeState.PresenceOut
            roPlanification.Params = oParams
        End If
        Return roPlanification

    End Function

    Public Shared Function GetAccrualEmployeeSummary(idEmployee As Integer, onDate As Date, type As SummaryType, oContractConector As Contract.roContract, ByRef oState As Employee.roEmployeeState) As List(Of roAccrualsSummary)
        Dim employeeAcrrualsDt As New DataTable
        Dim roAccrualsSummaryList As New List(Of roAccrualsSummary)

        Dim labAgreeStartupAccruals As New List(Of LabAgree.roStartupValue)
        If (oContractConector IsNot Nothing AndAlso oContractConector.LabAgree IsNot Nothing) Then
            labAgreeStartupAccruals = oContractConector.LabAgree.StartupValues
        End If

        Select Case type
            Case SummaryType.Anual
                employeeAcrrualsDt = Concept.roConcept.GetAnualAccruals(idEmployee, onDate.Date, oState, ,, True, True)
            Case SummaryType.LastYear
                employeeAcrrualsDt = Concept.roConcept.GetAnualAccruals(idEmployee, onDate.Date, oState, ,, True, True, True)
            Case SummaryType.Mensual
                employeeAcrrualsDt = Concept.roConcept.GetMonthAccruals(idEmployee, onDate.Date, oState, , True, True)
            Case SummaryType.LastMonth
                employeeAcrrualsDt = Concept.roConcept.GetMonthAccruals(idEmployee, onDate.Date, oState, , True, True, True)
            Case SummaryType.Semanal
                employeeAcrrualsDt = Concept.roConcept.GetWeekAccruals(idEmployee, onDate.Date, oState, , True, True)
            Case SummaryType.Daily
                employeeAcrrualsDt = Concept.roConcept.GetDailyAccruals(idEmployee, onDate.Date, oState, True, True)
            Case SummaryType.Contrato
                employeeAcrrualsDt = Concept.roConcept.GetContractAccruals(idEmployee, onDate.Date, oState, ,, True, True)
            Case SummaryType.ContractAnnualized
                employeeAcrrualsDt = Concept.roConcept.GetContractAnnualizedAccruals(idEmployee, onDate.Date, oState, ,, True, True)
        End Select

        If (employeeAcrrualsDt IsNot Nothing AndAlso employeeAcrrualsDt.Rows.Count > 0) Then
            For Each row As DataRow In employeeAcrrualsDt.Rows
                Dim roEmployeeAccrual As New roAccrualsSummary
                roEmployeeAccrual.IDConcept = roTypes.Any2Integer(row("IDConcept"))
                roEmployeeAccrual.Name = roTypes.Any2String(row("Name"))
                roEmployeeAccrual.Total = If(type.Equals(SummaryType.Daily), roTypes.Any2Double(row("Value")), roTypes.Any2Double(row("Total")))
                roEmployeeAccrual.Type = roTypes.Any2String(row("IDType"))
                roEmployeeAccrual.TotalFormat = If(type.Equals(SummaryType.Daily), roTypes.Any2String(row("ValueFormat")), roTypes.Any2String(row("TotalFormat")))
                If (labAgreeStartupAccruals IsNot Nothing And labAgreeStartupAccruals.Count > 0) Then
                    Dim startUpValue As LabAgree.roStartupValue
                    startUpValue = labAgreeStartupAccruals.SingleOrDefault(Function(co) co.IDConcept.Equals(roEmployeeAccrual.IDConcept))
                    If (startUpValue IsNot Nothing) Then
                        If Not IsDBNull(row("MaxValue")) Then roEmployeeAccrual.MaxValue = roTypes.Any2Double(row("MaxValue"))
                    End If
                End If
                roAccrualsSummaryList.Add(roEmployeeAccrual)
            Next
        End If

        Return roAccrualsSummaryList
    End Function

    Public Shared Function GetCausesEmployeeSummary(idEmployee As Integer, onDate As Date, type As SummaryType, oContractConector As Contract.roContract, ByRef oState As Employee.roEmployeeState) As List(Of roCausesSummary)
        Dim employeeCausesDt As New DataTable
        Dim roCausesSummaryList As New List(Of roCausesSummary)

        Dim labCausesLimits As New List(Of LabAgree.roLabAgreeCauseLimitValues)
        If (oContractConector.LabAgree IsNot Nothing) Then labCausesLimits = oContractConector.LabAgree.LabAgreeCauseLimitValues

        Select Case type
            Case SummaryType.Anual
                employeeCausesDt = Cause.roCause.GetAnualCauses(idEmployee, onDate.Date, oState)
            Case SummaryType.LastYear
                employeeCausesDt = Cause.roCause.GetAnualCauses(idEmployee, onDate.Date, oState, , True)
            Case SummaryType.Mensual
                employeeCausesDt = Cause.roCause.GetMonthCauses(idEmployee, onDate.Date, oState)
            Case SummaryType.LastMonth
                employeeCausesDt = Cause.roCause.GetMonthCauses(idEmployee, onDate.Date, oState, , True)
            Case SummaryType.Semanal
                employeeCausesDt = Cause.roCause.GetWeekCauses(idEmployee, onDate.Date, oState)
            Case SummaryType.Daily
                employeeCausesDt = Cause.roCause.GetDailyCauses(idEmployee, onDate.Date, oState)
            Case SummaryType.Contrato
                employeeCausesDt = Cause.roCause.GetContractCauses(idEmployee, onDate.Date, oState)
            Case SummaryType.ContractAnnualized
                employeeCausesDt = Cause.roCause.GetContractAnnualizedCauses(idEmployee, onDate.Date, oState)
        End Select

        If (employeeCausesDt IsNot Nothing AndAlso employeeCausesDt.Rows.Count > 0) Then
            For Each row As DataRow In employeeCausesDt.Rows
                Dim roEmployeeCause As New roCausesSummary
                roEmployeeCause.IDCause = roTypes.Any2Integer(row("IDCause"))
                roEmployeeCause.Name = roTypes.Any2String(row("Name"))
                roEmployeeCause.Total = roTypes.Any2Double(row("Total"))
                roEmployeeCause.WorkingType = roTypes.Any2String(row("WorkingType"))
                roEmployeeCause.TotalFormat = roTypes.Any2String(row("TotalFormat"))
                Dim limitValueSingle As New LabAgree.roLabAgreeCauseLimitValues
                limitValueSingle = labCausesLimits.SingleOrDefault(Function(cl) cl.CauseLimitValue IsNot Nothing _
                                                                           AndAlso cl.CauseLimitValue.IDCause.Equals(roEmployeeCause.IDCause) _
                                                                           AndAlso (cl.BeginDate <= onDate AndAlso cl.EndDate >= onDate))
                If (limitValueSingle IsNot Nothing) Then
                    If (Not limitValueSingle.CauseLimitValue.MaximumAnnualValueType.Equals(LabAgreeValueType.None)) Then
                        roEmployeeCause.Type = SummaryType.Anual
                        Select Case limitValueSingle.CauseLimitValue.MaximumAnnualValueType
                            Case LabAgreeValueType.DirectValue
                                roEmployeeCause.Limit = limitValueSingle.CauseLimitValue.MaximumAnnualValue
                            Case LabAgreeValueType.UserField
                                Dim userField As VTUserFields.UserFields.roEmployeeUserField
                                userField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, limitValueSingle.CauseLimitValue.MaximumAnnualField.FieldName, onDate, New VTUserFields.UserFields.roUserFieldState(oState.IDPassport))
                                If userField.Definition.FieldType = UserFieldsTypes.FieldTypes.tTime Then
                                    Dim dValue As Date = roTypes.Any2DateTime(userField.FieldValue)
                                    roEmployeeCause.Limit = (dValue.Hour + dValue.Minute / 60)
                                Else
                                    roEmployeeCause.Limit = roTypes.Any2Double(userField.FieldValue)
                                End If
                        End Select
                    ElseIf (Not limitValueSingle.CauseLimitValue.MaximumMonthlyType.Equals(LabAgreeValueType.None)) Then
                        roEmployeeCause.Type = SummaryType.Mensual
                        Select Case limitValueSingle.CauseLimitValue.MaximumMonthlyType
                            Case LabAgreeValueType.DirectValue
                                roEmployeeCause.Limit = limitValueSingle.CauseLimitValue.MaximumMonthlyValue
                            Case LabAgreeValueType.UserField
                                Dim userField As VTUserFields.UserFields.roEmployeeUserField
                                userField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, limitValueSingle.CauseLimitValue.MaximumMonthlyField.FieldName, onDate, New VTUserFields.UserFields.roUserFieldState(oState.IDPassport))
                                If userField.Definition.FieldType = UserFieldsTypes.FieldTypes.tTime Then
                                    Dim dValue As Date = roTypes.Any2DateTime(userField.FieldValue)
                                    roEmployeeCause.Limit = (dValue.Hour + dValue.Minute / 60)
                                Else
                                    roEmployeeCause.Limit = roTypes.Any2Double(userField.FieldValue)
                                End If
                        End Select
                    End If
                End If
                roCausesSummaryList.Add(roEmployeeCause)
            Next
        End If
        Return roCausesSummaryList
    End Function

    Public Shared Function GetTasksEmployeeSummary(idEmployee As Integer, onDate As Date, type As SummaryType, ByRef oState As Employee.roEmployeeState) As List(Of roTasksSummary)
        Dim employeeTasksDt As New DataTable
        Dim roTasksSummaryList As New List(Of roTasksSummary)

        Select Case type
            Case SummaryType.Anual
                employeeTasksDt = Concept.roConcept.GetAnualTaskAccruals(idEmployee, onDate.Date, oState, )
            Case SummaryType.LastYear
                employeeTasksDt = Concept.roConcept.GetAnualTaskAccruals(idEmployee, onDate.Date, oState, , True)
            Case SummaryType.Mensual
                employeeTasksDt = Concept.roConcept.GetMonthTaskAccruals(idEmployee, onDate.Date, oState, )
            Case SummaryType.LastMonth
                employeeTasksDt = Concept.roConcept.GetMonthTaskAccruals(idEmployee, onDate.Date, oState, , True)
            Case SummaryType.Semanal
                employeeTasksDt = Concept.roConcept.GetWeekTaskAccruals(idEmployee, onDate.Date, oState, )
            Case SummaryType.Daily
                employeeTasksDt = Concept.roConcept.GetDailyTaskAccruals(idEmployee, onDate.Date, oState)
            Case SummaryType.Contrato
                employeeTasksDt = Concept.roConcept.GetContractTaskAccruals(idEmployee, onDate.Date, oState, )
            Case SummaryType.ContractAnnualized
                employeeTasksDt = Concept.roConcept.GetContractAnnualizedTaskAccruals(idEmployee, onDate.Date, oState, )
        End Select

        If (employeeTasksDt IsNot Nothing AndAlso employeeTasksDt.Rows.Count > 0) Then
            For Each row As DataRow In employeeTasksDt.Rows
                Dim roTasksSummary As New roTasksSummary
                roTasksSummary.IdTask = roTypes.Any2Integer(row("IDTask"))
                roTasksSummary.TaskName = roTypes.Any2String(row("Name"))
                roTasksSummary.TaskValue = If(type.Equals(SummaryType.Daily), roTypes.Any2Double(row("Value")), roTypes.Any2Double(row("Total")))
                roTasksSummary.TaskValueFormat = If(type.Equals(SummaryType.Daily), roTypes.Any2Double(row("ValueFormat")), roTypes.Any2String(row("TotalFormat")))
                roTasksSummaryList.Add(roTasksSummary)
            Next
        End If

        Return roTasksSummaryList
    End Function

    Public Shared Function GetBussinessCentersEmployeeSummary(idEmployee As Integer, onDate As Date, type As SummaryType, ByRef oState As Employee.roEmployeeState) As List(Of roBussinessCentersSummary)
        Dim employeeCentersDt As New DataTable
        Dim roBussinessCentersSummaryList As New List(Of roBussinessCentersSummary)

        If roTypes.Any2Integer(ExecuteScalar("@SELECT# isnull(count(id),0) from BusinessCenters")) > 0 Then
            Select Case type
                Case SummaryType.Anual
                    employeeCentersDt = BusinessCenter.roBusinessCenter.GetAnualBussinessCenters(idEmployee, onDate.Date, oState)
                Case SummaryType.LastYear
                    employeeCentersDt = BusinessCenter.roBusinessCenter.GetAnualBussinessCenters(idEmployee, onDate.Date, oState, , True)
                Case SummaryType.Mensual
                    employeeCentersDt = BusinessCenter.roBusinessCenter.GetMonthBussinessCenters(idEmployee, onDate.Date, oState)
                Case SummaryType.LastMonth
                    employeeCentersDt = BusinessCenter.roBusinessCenter.GetMonthBussinessCenters(idEmployee, onDate.Date, oState)
                Case SummaryType.Semanal
                    employeeCentersDt = BusinessCenter.roBusinessCenter.GetWeekBussinessCenters(idEmployee, onDate.Date, oState)
                Case SummaryType.Daily
                    employeeCentersDt = BusinessCenter.roBusinessCenter.GetDailyBussinessCenters(idEmployee, onDate.Date, oState)
                Case SummaryType.Contrato
                    employeeCentersDt = BusinessCenter.roBusinessCenter.GetContractBussinessCenters(idEmployee, onDate.Date, oState)
                Case SummaryType.ContractAnnualized
                    employeeCentersDt = BusinessCenter.roBusinessCenter.GetContractAnnualizedBussinessCenters(idEmployee, onDate.Date, oState)
            End Select
        End If

        If (employeeCentersDt IsNot Nothing AndAlso employeeCentersDt.Rows.Count > 0) Then
            For Each row As DataRow In employeeCentersDt.Rows
                Dim roEmployeeCenter As New roBussinessCentersSummary
                Dim costHourUserField As VTUserFields.UserFields.roEmployeeUserField
                costHourUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, roTypes.Any2String(ExecuteScalar("@SELECT# Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroCostHour'")), onDate, New VTUserFields.UserFields.roUserFieldState(oState.IDPassport))
                Dim priceHourUserField As VTUserFields.UserFields.roEmployeeUserField
                priceHourUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, roTypes.Any2String(ExecuteScalar("@SELECT# Fieldname  FROM dbo.sysroUserFields WHERE  isnull([Alias], Fieldname) = 'sysroPriceHour'")), onDate, New VTUserFields.UserFields.roUserFieldState(oState.IDPassport))

                roEmployeeCenter.CauseName = roTypes.Any2String(row("CauseName"))
                roEmployeeCenter.CauseType = roTypes.Any2Boolean(row("WorkingType"))
                roEmployeeCenter.CauseCostFactor = roTypes.Any2Double(row("CostFactor"))
                roEmployeeCenter.IdCenter = roTypes.Any2Integer(row("IdCenter"))
                roEmployeeCenter.CenterName = roTypes.Any2String(row("CenterName"))
                roEmployeeCenter.DefaultCenter = roTypes.Any2Boolean(row("DefaultCenter"))
                roEmployeeCenter.Total = roTypes.Any2Double(row("Total"))
                roEmployeeCenter.TotalFormat = roTypes.Any2String(row("TotalFormat"))
                roEmployeeCenter.CostHour = roTypes.Any2Double(If(costHourUserField IsNot Nothing, costHourUserField.FieldValue, 0))
                roEmployeeCenter.PriceHour = roTypes.Any2Double(If(priceHourUserField IsNot Nothing, priceHourUserField.FieldValue, 0))
                Dim xValue As Double
                Dim oCost As Double
                oCost = If(roEmployeeCenter.DefaultCenter, roEmployeeCenter.CostHour, roEmployeeCenter.PriceHour)
                xValue = roTypes.Any2Double(oCost.ToString.Replace(".", roConversions.GetDecimalDigitFormat()))
                roEmployeeCenter.EmployeeCost = roEmployeeCenter.Total * xValue * roEmployeeCenter.CauseCostFactor

                roBussinessCentersSummaryList.Add(roEmployeeCenter)
            Next
        End If

        Return roBussinessCentersSummaryList
    End Function

End Class