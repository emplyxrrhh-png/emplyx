Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.AIPlanner
Imports Robotics.Base.VTBudgets
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTCalendar
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace AIPlanner

    Public Class roAIPlannerManager

        Private oState As roAIPlannerState = Nothing

        Public ReadOnly Property State As roAIPlannerState
            Get
                Return oState
            End Get
        End Property

        Public Sub New(ByVal _State As roAIPlannerState)
            Me.oState = _State
        End Sub

        Public Function SolveProblem(oBudget As roBudget, ByRef oProblemSolution As roAIProblemSolution) As Boolean
            Dim bolRet As Boolean = True
            Dim oCalendarResult As roCalendarResult

            Try

                If Not oBudget Is Nothing AndAlso Not oBudget.BudgetData Is Nothing Then

                    Dim oProblem As New roAIPlannerProblem
                    Dim strEmployees As String = String.Empty
                    oProblem = GetAIProblem(oBudget, strEmployees)

                    If Not oProblem Is Nothing Then
                        ' Cargamos calendario de empleados afectados
                        Dim oCalManager As VTCalendar.roCalendarManager = Nothing
                        oCalManager = New VTCalendar.roCalendarManager(New VTCalendar.roCalendarState(Me.State.IDPassport))

                        Dim oCalRuleState As New roCalendarScheduleRulesState(oState.IDPassport)
                        Dim oCalRulesManager As New roCalendarScheduleRulesManager(oCalRuleState)
                        Dim oCalendar As DTOs.roCalendar = oCalManager.Load(oBudget.FirstDay, oBudget.LastDay, "B" & strEmployees.Replace(",", ",B"), DTOs.CalendarView.Planification, DTOs.CalendarDetailLevel.Daily, True)

                        ' Compacto calendario para no encontrar más de una fila por empleado
                        Dim oCalendarCompact As New List(Of roCalendarRow)
                        oCalendarCompact = oCalRulesManager.CompactCalendar(oCalendar)

                        ' Cargamos las reglas de planificación para los empleados afectados
                        Dim oEmployeeScheduleRules As New List(Of EmployeeScheduleRule)
                        ' Calculo días de inicio de año y día de inicio de mes
                        Dim oParameters As New roParameters("OPTIONS", True)
                        Dim iMonthIniDay As Integer
                        Dim iYearIniMonth As Integer
                        Dim dYearFirstDate As Date = DateSerial(2000, 1, 1)
                        dYearFirstDate = oCalRulesManager.GetYearFirstDate(oCalendar.FirstDay, oParameters, iMonthIniDay, iYearIniMonth)

                        oEmployeeScheduleRules = oCalRulesManager.GetEmployeesScheduleRules(oCalendar.FirstDay, oCalendar.LastDay, strEmployees, dYearFirstDate, True)

                        ' Recopilo información adicional en función de los empleados y reglas
                        Dim oEmployeesRulesAuxData As New roEmployeeRulesData(oEmployeeScheduleRules, oCalendar.FirstDay, oCalendar.LastDay, dYearFirstDate, strEmployees, oCalRuleState)

                        ' Pasamos los datos al motor de AI para que nos devuelva una solución de planificación
                        Dim mR As New VTORCore.RoboticsOT

                        ' Preparamos escenario
                        mR.Init()
                        mR.Calendario.FechaInicial = oProblem.StartDate

                        ' Añadimos trabajadores
                        Dim tr As VTORCore.RoboticsTrabajador
                        Dim iEmpCostOnDate As Double = 1
                        Dim pRulePeriod As New roAIPeriod
                        Dim oPrevCalendarCompact As New List(Of roCalendarRow)
                        Dim oPostCalendarCompact As New List(Of roCalendarRow)
                        Dim dPrevStart As Date = DateSerial(2079, 1, 1)
                        Dim dPostEnd As Date = DateSerial(2079, 1, 1)
                        Dim dicAIRulesPeriods As New Dictionary(Of ScheduleRuleScope, roAIPeriod)

                        For Each oEmployeeAux As roAIEmployee In oProblem.Employees
                            tr = New VTORCore.RoboticsTrabajador(oEmployeeAux.ID.ToString, oEmployeeAux.Name)
                            ' Primero vemos si hay que añadir días extras fuera del periodo a planificar, para que los tenga en cuenta el planificador
                            ' Esto es a nivel de empleado. Cada empleado puede tener su período previo y posterior de planificación
                            ' Obtenemos si es necesario el calendario del periodo previo y posterior del empleado
                            dPrevStart = DateSerial(2079, 1, 1)
                            dPostEnd = DateSerial(2079, 1, 1)
                            bolRet = GetAuxEmployeeCalendar(oEmployeeAux.ID, oEmployeeScheduleRules, oCalRulesManager, oCalManager, oCalendar, oPrevCalendarCompact, oPostCalendarCompact, dPrevStart, dPostEnd, dYearFirstDate, iMonthIniDay, iYearIniMonth, oParameters, dicAIRulesPeriods)

                            ' Añadimos puestos
                            For Each sAssignment As String In oEmployeeAux.AllowedAssignment
                                tr.Puestos.Add(New VTORCore.RoboticsTrabajadorPuesto(sAssignment))
                            Next

                            ' Añado días a planificar por la AI
                            Dim iDayIndex As Integer = 0
                            For i As Integer = 1 To DateDiff(DateInterval.Day, oProblem.StartDate, oProblem.EndDate) + 1
                                iDayIndex = i
                                For Each oRow As roCalendarRow In oCalendarCompact.FindAll(Function(z) z.EmployeeData.IDEmployee = oEmployeeAux.ID)
                                    'EN TEORÍA POR AQUÍ SOLO DEBE PASAR UNA VEZ, PORQUE EL OBJETO CALENDARIO ESTÁ COMPACTADO
                                    ' Añado definición de posibles horarios necesarios para periodo previo al analizado
                                    If Not oRow.PeriodData.DayData Is Nothing AndAlso Not oRow.PeriodData.DayData(iDayIndex - 1).MainShift Is Nothing AndAlso oProblem.Shifts.FindAll(Function(x) x.UID = GetShiftUID(oRow.PeriodData.DayData(iDayIndex - 1).MainShift)).Count = 0 Then
                                        oProblem.Shifts.Add(New roAIShift(oRow.PeriodData.DayData(iDayIndex - 1).MainShift.ID, GetShiftUID(oRow.PeriodData.DayData(iDayIndex - 1).MainShift), oRow.PeriodData.DayData(iDayIndex - 1).MainShift))
                                    End If

                                    ' Recupero coste del empleado para la fecha
                                    iEmpCostOnDate = 1
                                    If oEmployeeAux.Costs.FindAll(Function(x) x.Date <= oProblem.StartDate.AddDays(iDayIndex - 1)).Count > 0 Then
                                        iEmpCostOnDate = oEmployeeAux.Costs.FindAll(Function(x) x.Date <= oProblem.StartDate.AddDays(iDayIndex - 1)).OrderByDescending(Function(y) y.Date).First.Cost
                                    End If

                                    Dim oEmpStatusOnDay As New roAIEmployeeDayStatus
                                    oEmpStatusOnDay = GetEmployeeStatusOnDay(oRow.PeriodData.DayData(iDayIndex - 1), oEmployeeAux.ID, oEmployeesRulesAuxData, oEmployeeScheduleRules, oProblem.StartDate, oProblem.EndDate)
                                    Select Case oEmpStatusOnDay.Status
                                        Case EmployeeAIDayStatus.Absent
                                            tr.Dias.Add(New VTORCore.RoboticsTrabajadorDia(iEmpCostOnDate, VTORCore.RoboticsTrabajadorDiaModo.ausente, oEmployeeAux.AllowedShifts.ToArray))
                                        Case EmployeeAIDayStatus.Available
                                            ' Disponible, con todo, o si el día está bloqueado, forzando el horario
                                            If oEmpStatusOnDay.Shift = "*" Then
                                                tr.Dias.Add(New VTORCore.RoboticsTrabajadorDia(iEmpCostOnDate, VTORCore.RoboticsTrabajadorDiaModo.disponible, oEmployeeAux.AllowedShifts.ToArray))
                                            Else
                                                tr.Dias.Add(New VTORCore.RoboticsTrabajadorDia(iEmpCostOnDate, VTORCore.RoboticsTrabajadorDiaModo.disponible, {oEmpStatusOnDay.Shift}))
                                            End If
                                        Case EmployeeAIDayStatus.Present
                                            ' Esto corresponde a un empleado que está planificado y asignado a UP
                                            tr.Dias.Add(New VTORCore.RoboticsTrabajadorDia(iEmpCostOnDate, VTORCore.RoboticsTrabajadorDiaModo.presente, {oEmpStatusOnDay.Shift}, {oEmpStatusOnDay.Assignment}))
                                        Case Else
                                            ' Este caso no se debe dar de momento ...
                                            tr.Dias.Add(New VTORCore.RoboticsTrabajadorDia(iEmpCostOnDate, VTORCore.RoboticsTrabajadorDiaModo.ausente, oEmployeeAux.AllowedShifts.ToArray))
                                    End Select
                                Next
                            Next

                            ' Añado días fuera del periodo a planificar, para que los tenga en cuenta para las reglas
                            ' Para estos días, los días en que el empleado no venga (vacaciones, ausencias, ...) se pasan en estado AUSENTE, y el resto PRESENTE (siempre con un horario)
                            oProblem.Employees.Find(Function(f) f.ID = oEmployeeAux.ID).CalendarStartDate = oProblem.StartDate
                            oProblem.Employees.Find(Function(f) f.ID = oEmployeeAux.ID).CalendarEndDate = oProblem.EndDate
                            If dPrevStart <> DateSerial(2079, 1, 1) Then
                                oProblem.Employees.Find(Function(f) f.ID = oEmployeeAux.ID).CalendarStartDate = dPrevStart
                                Dim sShift As String = String.Empty
                                For i As Integer = 1 To DateDiff(DateInterval.Day, dPrevStart, oProblem.StartDate.AddDays(-1)) + 1
                                    iDayIndex = i
                                    For Each oRowPre As roCalendarRow In oPrevCalendarCompact.FindAll(Function(z) z.EmployeeData.IDEmployee = oEmployeeAux.ID)
                                        If Not oRowPre.PeriodData.DayData(iDayIndex - 1).MainShift Is Nothing Then
                                            sShift = oRowPre.PeriodData.DayData(iDayIndex - 1).MainShift.ShortName
                                            ' Añado definición de posibles horarios necesarios para periodo previo al analizado
                                            If oProblem.Shifts.FindAll(Function(x) x.UID = GetShiftUID(oRowPre.PeriodData.DayData(iDayIndex - 1).MainShift)).Count = 0 Then
                                                oProblem.Shifts.Add(New roAIShift(oRowPre.PeriodData.DayData(iDayIndex - 1).MainShift.ID, GetShiftUID(oRowPre.PeriodData.DayData(iDayIndex - 1).MainShift), oRowPre.PeriodData.DayData(iDayIndex - 1).MainShift))
                                            End If
                                        End If
                                        Dim oEmpStatusOnDay As New roAIEmployeeDayStatus
                                        oEmpStatusOnDay = GetEmployeeStatusOnDay(oRowPre.PeriodData.DayData(iDayIndex - 1), oEmployeeAux.ID, oEmployeesRulesAuxData, oEmployeeScheduleRules, oProblem.StartDate, oProblem.EndDate)
                                        Select Case oEmpStatusOnDay.Status
                                            Case EmployeeAIDayStatus.Absent
                                                If oEmpStatusOnDay.Shift = "" Then
                                                    tr.DiasAnteriores.Add(New VTORCore.RoboticsTrabajadorDia(0, VTORCore.RoboticsTrabajadorDiaModo.ausente, oEmployeeAux.AllowedShifts.ToArray))
                                                Else
                                                    tr.DiasAnteriores.Add(New VTORCore.RoboticsTrabajadorDia(0, VTORCore.RoboticsTrabajadorDiaModo.ausente, {GetShiftUID(oRowPre.PeriodData.DayData(iDayIndex - 1).MainShift)}))
                                                End If
                                            Case Else
                                                tr.DiasAnteriores.Add(New VTORCore.RoboticsTrabajadorDia(0, VTORCore.RoboticsTrabajadorDiaModo.presente, {GetShiftUID(oRowPre.PeriodData.DayData(iDayIndex - 1).MainShift)}))
                                        End Select
                                    Next
                                Next
                            End If
                            If dPostEnd <> DateSerial(2079, 1, 1) Then
                                Dim sShift As String = String.Empty
                                oProblem.Employees.Find(Function(f) f.ID = oEmployeeAux.ID).CalendarEndDate = dPostEnd
                                For i As Integer = 1 To DateDiff(DateInterval.Day, oProblem.EndDate.AddDays(1), dPostEnd) + 1
                                    iDayIndex = i
                                    For Each oRowPost As roCalendarRow In oPostCalendarCompact.FindAll(Function(z) z.EmployeeData.IDEmployee = oEmployeeAux.ID)
                                        If Not oRowPost.PeriodData.DayData(iDayIndex - 1).MainShift Is Nothing Then
                                            sShift = oRowPost.PeriodData.DayData(iDayIndex - 1).MainShift.ShortName
                                            ' Añado definición de posibles horarios necesarios para periodo previo al analizado
                                            If oProblem.Shifts.FindAll(Function(x) x.UID = GetShiftUID(oRowPost.PeriodData.DayData(iDayIndex - 1).MainShift)).Count = 0 Then
                                                oProblem.Shifts.Add(New roAIShift(oRowPost.PeriodData.DayData(iDayIndex - 1).MainShift.ID, oRowPost.PeriodData.DayData(iDayIndex - 1).MainShift.StartHour, oRowPost.PeriodData.DayData(iDayIndex - 1).MainShift))
                                            End If
                                        End If
                                        Dim oEmpStatusOnDay As New roAIEmployeeDayStatus
                                        oEmpStatusOnDay = GetEmployeeStatusOnDay(oRowPost.PeriodData.DayData(iDayIndex - 1), oEmployeeAux.ID, oEmployeesRulesAuxData, oEmployeeScheduleRules, oProblem.StartDate, oProblem.EndDate)
                                        Select Case oEmpStatusOnDay.Status
                                            Case EmployeeAIDayStatus.Absent
                                                If oEmpStatusOnDay.Shift = "" Then
                                                    tr.DiasPosteriores.Add(New VTORCore.RoboticsTrabajadorDia(0, VTORCore.RoboticsTrabajadorDiaModo.ausente, oEmployeeAux.AllowedShifts.ToArray))
                                                Else
                                                    tr.DiasPosteriores.Add(New VTORCore.RoboticsTrabajadorDia(0, VTORCore.RoboticsTrabajadorDiaModo.ausente, {GetShiftUID(oRowPost.PeriodData.DayData(iDayIndex - 1).MainShift)}))
                                                End If
                                            Case Else
                                                tr.DiasPosteriores.Add(New VTORCore.RoboticsTrabajadorDia(0, VTORCore.RoboticsTrabajadorDiaModo.presente, {GetShiftUID(oRowPost.PeriodData.DayData(iDayIndex - 1).MainShift)}))
                                        End Select
                                    Next
                                Next
                            End If
                            mR.Trabajadores.Add(tr)
                        Next

                        ' Añadimos reglas por empleados
                        For Each oEmployeeAux As roAIEmployee In oProblem.Employees
                            tr = mR.Trabajadores.Find(Function(x) x.Id = oEmployeeAux.ID.ToString)
                            For Each oRule As EmployeeScheduleRule In oEmployeeScheduleRules.FindAll(Function(f) f.IdEmployee = oEmployeeAux.ID AndAlso f.ScheduleRule.Parameters.Enabled)
                                Select Case oRule.ScheduleRule.Parameters.IDRule
                                    Case ScheduleRuleType.MinMaxFreeLabourDaysInPeriod
                                        Dim oCastRule = CType(oRule.ScheduleRule.Parameters, roScheduleRule_MinMaxFreeLabourDaysInPeriod)
                                        ' Calculo periodo para esta regla dado el periodo a analizar
                                        pRulePeriod = GetIntervalIntersection(dicAIRulesPeriods(oCastRule.Scope).DateStart, dicAIRulesPeriods(oCastRule.Scope).DateEnd, oRule.StartDate, oRule.EndDate)
                                        If pRulePeriod.DateStart > Date.MinValue Then
                                            ' Sólo si hay fechas en la intersección, añado la regla
                                            ' Tantas reglas como hagan falta en función del periodo de la regla
                                            For Each oPI As roAIPeriod In GetRuleDates(pRulePeriod.DateStart, pRulePeriod.DateEnd, oCastRule.Scope, oCastRule.RuleName)
                                                tr.Reglas.Add(New VTORCore.RoboticsRegla(oPI.DateStart, oPI.DateEnd, oCastRule.MinimumRestDays, 0, VTORCore.RoboticsReglaTipo.jornadas_de_descanso))
                                            Next
                                        End If
                                    Case ScheduleRuleType.MaxHolidays
                                        ' No aplica, dado que el ORCore no va aplicar horarios de vacaciones.
                                    Case ScheduleRuleType.MinMax2ShiftSequenceOnEmployee
                                        CreateUserTask(oRule)
                                    Case ScheduleRuleType.MinMaxExpectedHours
                                        Dim iAlreadyPlannedHours As Integer = 0
                                        Dim oCastRule = CType(oRule.ScheduleRule.Parameters, roScheduleRule_MinMaxExpectedHours)
                                        If oCastRule.IdContract = "" Then
                                            ' Viene de convenio
                                            If oEmployeesRulesAuxData.AlreadyPlannedHours.Exists(Function(x) x.IdLabAgree = oCastRule.IdLabAgree AndAlso x.IdEmployee = oEmployeeAux.ID) Then
                                                iAlreadyPlannedHours = oEmployeesRulesAuxData.AlreadyPlannedHours.Find(Function(x) x.IdLabAgree = oCastRule.IdLabAgree).Value
                                            End If
                                        Else
                                            ' Viene de contrato
                                            If oEmployeesRulesAuxData.AlreadyPlannedHours.Exists(Function(x) x.IdContract = oCastRule.IdContract AndAlso x.IdEmployee = oEmployeeAux.ID) Then
                                                iAlreadyPlannedHours = oEmployeesRulesAuxData.AlreadyPlannedHours.Find(Function(x) x.IdContract = oCastRule.IdContract).Value
                                            End If
                                        End If
                                        pRulePeriod = GetIntervalIntersection(oProblem.StartDate, oProblem.EndDate, oRule.StartDate, oRule.EndDate)
                                        If pRulePeriod.DateStart > Date.MinValue Then
                                            tr.Reglas.Add(New VTORCore.RoboticsRegla(pRulePeriod.DateStart, pRulePeriod.DateEnd, oCastRule.MaximumWorkingHours * 60, iAlreadyPlannedHours * 60, VTORCore.RoboticsReglaTipo.maximo_tiempo_de_trabajo))
                                        End If
                                    Case ScheduleRuleType.MinMaxShiftsInPeriod
                                        Dim oCastRule = CType(oRule.ScheduleRule.Parameters, roScheduleRule_MinMaxShiftsInPeriod)
                                        pRulePeriod = GetIntervalIntersection(dicAIRulesPeriods(oCastRule.Scope).DateStart, dicAIRulesPeriods(oCastRule.Scope).DateEnd, oRule.StartDate, oRule.EndDate)
                                        If pRulePeriod.DateStart > Date.MinValue Then
                                            ' Sólo si hay fechas en la intersección, añado la regla
                                            For Each idShift In oCastRule.CurrentDayShifts
                                                ' Miro si el horario está entre los que he añadido por la definición del escenenario
                                                If oProblem.Shifts.FindAll(Function(x) x.VT_Id = idShift).Count = 0 Then
                                                    ' Podría añadir el horario a la colección del problema, pero no debe hacer falta.
                                                    ' Si ya no está en la colección, es que el horario no está en necesidades ni en planificación, por lo que
                                                    ' AI no lo va a planificar.

                                                    ' Si realmente se demostrara que hace falta añadirlo ...
                                                    ' Dim oShiftDataDefinition As New roCalendarShift
                                                    ' oShiftDataDefinition = oCalManager.LoadShiftDataByIdEx(idShift)
                                                    ' Dim oNewShiftData As New roCalendarRowShiftData
                                                    ' oNewShiftData = oCalManager.MergeShiftData(oShiftDataDefinition, oNewShiftData)
                                                    ' oProblem.Shifts.Add(New roAIShift(oNewShiftData.ID, GetShiftUID(oNewShiftData), oNewShiftData))
                                                Else
                                                    ' Recupero UID del horario
                                                    Dim sUID As String = oProblem.Shifts.Find(Function(x) x.VT_Id = idShift).UID
                                                    ' Tantas reglas como hagan falta en función del periodo de la regla
                                                    For Each oPI As roAIPeriod In GetRuleDates(pRulePeriod.DateStart, pRulePeriod.DateEnd, oCastRule.Scope, oCastRule.RuleName)
                                                        If oCastRule.Maximum > 0 Then
                                                            'TODO: Usamos UID como identificador de horario en AI
                                                            tr.Reglas.Add(New VTORCore.RoboticsRegla(oPI.DateStart, oPI.DateEnd, idShift, oCastRule.Maximum, VTORCore.RoboticsReglaTipo.maximo_jornadas_con_turno))
                                                        End If
                                                        If oCastRule.Minimum > 0 Then
                                                            'TODO: Usamos UID como identificador de horario en AI
                                                            tr.Reglas.Add(New VTORCore.RoboticsRegla(oPI.DateStart, oPI.DateEnd, idShift, oCastRule.Minimum, VTORCore.RoboticsReglaTipo.minimo_jornadas_con_turno))
                                                        End If
                                                    Next
                                                End If
                                            Next
                                        End If
                                    Case ScheduleRuleType.MinWeekendsInPeriod
                                        Dim oCastRule = CType(oRule.ScheduleRule.Parameters, roScheduleRule_MinWeekendsInPeriod)
                                        pRulePeriod = GetIntervalIntersection(dicAIRulesPeriods(oCastRule.Scope).DateStart, dicAIRulesPeriods(oCastRule.Scope).DateEnd, oRule.StartDate, oRule.EndDate)

                                        Dim iWeekEndLenght As Integer = 0
                                        Dim iWeekEndStart As Integer = 6 '0=domingo, 1=lunes ...
                                        iWeekEndLenght = oEmployeesRulesAuxData.WeekendDefinitions.Find(Function(x) x.IdContract = oCastRule.IdContract).WeekEndDays.Count
                                        iWeekEndStart = oEmployeesRulesAuxData.WeekendDefinitions.Find(Function(x) x.IdContract = oCastRule.IdContract).WeekEndDays(0)
                                        If pRulePeriod.DateStart > Date.MinValue Then
                                            ' Tantas reglas como hagan falta en función del periodo de la regla
                                            For Each oPI As roAIPeriod In GetRuleDates(pRulePeriod.DateStart, pRulePeriod.DateEnd, oCastRule.Scope, oCastRule.RuleName)
                                                tr.Reglas.Add(New VTORCore.RoboticsRegla(oPI.DateStart, oPI.DateEnd, iWeekEndStart, iWeekEndLenght, VTORCore.RoboticsReglaTipo.establecer_fin_de_semana))
                                                tr.Reglas.Add(New VTORCore.RoboticsRegla(oPI.DateStart, oPI.DateEnd, oCastRule.Minimum, VTORCore.RoboticsReglaTipo.minimo_de_fines_de_semana))
                                            Next
                                        End If
                                    Case ScheduleRuleType.OneShiftOneDay
                                        ' TODO: Pendiente de desarrollo las que contempla ORCore
                                        CreateUserTask(oRule)
                                    Case ScheduleRuleType.RestBetweenShifts
                                        Dim oCastRule = CType(oRule.ScheduleRule.Parameters, roScheduleRule_RestBetweenShifts)
                                        pRulePeriod = GetIntervalIntersection(dicAIRulesPeriods(oCastRule.Scope).DateStart, dicAIRulesPeriods(oCastRule.Scope).DateEnd, oRule.StartDate, oRule.EndDate)
                                        If pRulePeriod.DateStart > Date.MinValue Then
                                            tr.Reglas.Add(New VTORCore.RoboticsRegla(pRulePeriod.DateStart, pRulePeriod.DateEnd, oCastRule.RestHours * 60, 0, VTORCore.RoboticsReglaTipo.descanso_entre_jornadas))
                                        End If
                                    Case ScheduleRuleType.TwoShiftSequence
                                        CreateUserTask(oRule)
                                    Case ScheduleRuleType.WorkOnFestive
                                        'Esta regla la trataré al decidir la disponibilidad del empleado en un día
                                    Case ScheduleRuleType.WorkOnWeekend
                                        'Esta regla la trataré al decidir la disponibilidad del empleado en un día
                                    Case Else
                                        CreateUserTask(oRule)
                                End Select
                            Next
                        Next

                        For Each oAIShift As roAIShift In oProblem.Shifts
                            If oAIShift.ShiftData.StartHour.Day <> oAIShift.ShiftData.EndHour.Day Then
                                mR.Turnos.Add(New VTORCore.RoboticsTurno(oAIShift.UID, oAIShift.VT_Id, oAIShift.ShiftData.StartHour.Hour * 60 + oAIShift.ShiftData.StartHour.Minute, (oAIShift.ShiftData.EndHour.Hour + 24) * 60 + oAIShift.ShiftData.EndHour.Minute, oAIShift.ShiftData.PlannedHours))
                            Else
                                mR.Turnos.Add(New VTORCore.RoboticsTurno(oAIShift.UID, oAIShift.VT_Id, oAIShift.ShiftData.StartHour.Hour * 60 + oAIShift.ShiftData.StartHour.Minute, oAIShift.ShiftData.EndHour.Hour * 60 + oAIShift.ShiftData.EndHour.Minute, oAIShift.ShiftData.PlannedHours))
                            End If
                        Next

                        ' Añadimos coberturas requeridas para cada día del periodo
                        Dim tempDate As Date = oProblem.StartDate
                        Dim dia As VTORCore.RoboticsCalendarioDia
                        For i As Integer = 1 To DateDiff(DateInterval.Day, oProblem.StartDate, oProblem.EndDate) + 1
                            tempDate = oProblem.StartDate.AddDays(i - 1)
                            dia = New VTORCore.RoboticsCalendarioDia
                            For Each oDayCoverAux As roAIDayCover In oProblem.CoverRequirements.FindAll(Function(x) x.Day = tempDate)
                                'dia.Puestos.Add(New VTORCore.RoboticsCalendarioPuesto((oDayCoverAux.Cover.VirtualAssignment.IDDailyBudgetPosition).ToString, oDayCoverAux.Cover.VirtualAssignment.Assignment, oDayCoverAux.Cover.Quantity, oDayCoverAux.Cover.VirtualAssignment.ShiftShortName))
                                dia.Puestos.Add(New VTORCore.RoboticsCalendarioPuesto((oDayCoverAux.Cover.VirtualAssignment.IDNode * 10000 + oDayCoverAux.Cover.VirtualAssignment.IDProductiveUnit).ToString, oDayCoverAux.Cover.VirtualAssignment.Assignment, oDayCoverAux.Cover.Quantity, oDayCoverAux.Cover.VirtualAssignment.ShiftShortName))
                            Next
                            mR.Calendario.Add(dia)
                        Next

                        Dim oSettings As New roSettings()
                        Try
                            oProblemSolution.file = oSettings.GetVTSetting(eKeys.DataLink) & "\aiplanner_problem" & oState.IDPassport & "_" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".xml"
                            mR.ExportarSolucion(oProblemSolution.file)
                        Catch ex As Exception
                        End Try

                        Dim SolverWaitSeconds As Integer() = {300} '{5, 30, 300}
                        Dim bSolutionFound As Boolean = False

                        For i = 1 To SolverWaitSeconds.Count
                            mR.CargarObjetos()
                            If mR.Errores.Count = 0 Then
                                mR.Solucionar()

                                Dim trFalta As New VTORCore.RoboticsTrabajador
                                Dim iExtraEmployees As Integer = 0
                                Dim bRestartSolver As Boolean = False

                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAIPlannerManager::SolveProblem::Lets try to find a solution in " & SolverWaitSeconds(i - 1).ToString & " seconds ...")
                                While Not mR.SiguienteSolucion(SolverWaitSeconds(i - 1)) AndAlso Not bRestartSolver
                                    iExtraEmployees += 1
                                    trFalta = mR.CrearTrabajadorQueFalta("0", 100)
                                    ' Aquí se podrían añadir reglas y demás para el empleado trFalta ...
                                    mR.Init()
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAIPlannerManager::SolveProblem::Extra employees added = " & iExtraEmployees.ToString)

                                    ' Si hemos añadido tantos FALTA como empleados reales, algo va mal ...
                                    If mR.MaximoDeFaltasEstimado < iExtraEmployees Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAIPlannerManager::SolveProblem::Too many extra employees needed. Lets try again thinking some more time ...")
                                        bRestartSolver = True
                                    End If
                                    Console.WriteLine(mR.Log)
                                End While
                                If Not bRestartSolver Then
                                    bSolutionFound = True
                                    Exit For
                                Else
                                    If (i + 1) > SolverWaitSeconds.Count Then
                                        Exit For
                                    Else
                                        ' Antes de volver a probar, elimino los FALTA que se han añadido ...
                                        mR.Trabajadores.RemoveAll(Function(x) x.Nombre.StartsWith("FALTA"))
                                    End If
                                End If
                            Else
                                oState.Result = roAIPlannerState.ResultEnum.NoSolutionAtAll
                                Dim strAICoreDetail As String = String.Empty
                                For Each sDetail As String In mR.Errores
                                    strAICoreDetail = strAICoreDetail & sDetail & vbCrLf
                                Next
                                oState.ErrorText = oState.Language.Translate(roAIPlannerState.ResultEnum.NoSolutionAtAll.ToString, "") & vbCrLf & oState.Language.Translate("NoSolutionAtAll.AICoreDetail", "") & vbCrLf & strAICoreDetail
                                Exit For
                            End If
                        Next

                        If bSolutionFound Then
                            Dim oAIAssignment As DTOs.AIPlanner.roAIAssignment
                            Dim oAISolution As roAIPlannerSolution
                            oAISolution = New roAIPlannerSolution
                            oAISolution.StartDate = oProblem.StartDate
                            oAISolution.EndDate = oProblem.EndDate

                            Try
                                Dim strResultFileDebug As String = String.Empty
                                strResultFileDebug = oSettings.GetVTSetting(eKeys.DataLink) & "\aiplanner_debug" & oState.IDPassport & "_" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".txt"
                                Dim file As System.IO.StreamWriter
                                file = My.Computer.FileSystem.OpenTextFileWriter(strResultFileDebug, True)
                                file.WriteLine(mR.Log)
                                file.Close()
                            Catch ex As Exception
                            End Try

                            ' Recorremos la solución
                            Dim lCurrentEmployeesRequired As New List(Of Integer)
                            Dim lExtraEmployeesRequired As New List(Of String)
                            Dim otr As New VTORCore.RoboticsTrabajadorAsignado
                            For i As Integer = 0 To DateDiff(DateInterval.Day, oProblem.StartDate, oProblem.EndDate)
                                For j As Integer = 0 To (mR.Puestos.Count - 1)
                                    Dim loopIndex As Integer = j
                                    ' TODO: ORCORE me devuelve unidades productivas "virtuales" en blanco. Corresponde a necesides fijadas, y por tanto debo ignorarlas
                                    otr = mR.TrabajadorDiaPuesto(i, j)
                                    If otr.Modo = VTORCore.RoboticsTrabajadorDiaModo.disponible AndAlso otr.Necesario Then
                                        If Not otr.Nombre.StartsWith("FALTA") Then
                                            oAIAssignment = New DTOs.AIPlanner.roAIAssignment
                                            oAIAssignment.Date = oProblem.StartDate.AddDays(i)
                                            oAIAssignment.IdEmployee = otr.Id
                                            oAIAssignment.IdShift = oProblem.Shifts.Find(Function(x) x.UID = otr.Horario).VT_Id
                                            oAIAssignment.ShiftUID = otr.Horario
                                            oAIAssignment.IdAssignment = oProblem.Assignments.Find(Function(x) x.Name = otr.Puesto).Id
                                            'oAIAssignment.IdDailyBudgetPosition = CInt(otr.Datos)
                                            oAIAssignment.IdProductiveUnit = CInt(otr.UP) Mod 10000
                                            oAIAssignment.IdNode = CInt(otr.UP) \ 10000
                                            oAISolution.Assignments.Add(oAIAssignment)
                                            If Not lCurrentEmployeesRequired.Contains(oAIAssignment.IdEmployee) Then lCurrentEmployeesRequired.Add(oAIAssignment.IdEmployee)
                                        Else
                                            ' TODO: Gestión de empleados faltantes, si fuese necesaria
                                            ' ...
                                            If Not lExtraEmployeesRequired.Contains(otr.Nombre) Then lExtraEmployeesRequired.Add(otr.Nombre)
                                        End If
                                    End If
                                Next
                            Next

                            ' Si la solución contiene algún FALTA, o afecta costes (no se ha podido solucionar con los empleados reales), busco una solución mejor durante X segundos
                            Dim iImproveTime As Integer = 5
                            If lExtraEmployeesRequired.Count > 0 Then
                                mR.MejorSolucion(iImproveTime)
                            End If

                            If lCurrentEmployeesRequired.Count > 0 Then

                                Dim oNewCalendar As roCalendar = GetSolutionCalendarEx(oAISolution, oProblem)
                                If lExtraEmployeesRequired.Count > 0 Then
                                    ' El problema requiere empleados nuevos
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(lExtraEmployeesRequired.Count.ToString)
                                    oState.Result = roAIPlannerState.ResultEnum.SomeExtraEmployeeRequired
                                Else
                                    ' Solución con empleados propios
                                    oState.Result = roAIPlannerState.ResultEnum.NoError
                                End If
                                If True Then
                                    oCalendarResult = oCalManager.Save(oNewCalendar, True, True)
                                    If oCalendarResult.Status <> CalendarStatusEnum.OK Then
                                        oState.Result = roAIPlannerState.ResultEnum.ErrorSavingSolution
                                    End If
                                End If
                            Else
                                If lExtraEmployeesRequired.Count > 0 Then
                                    ' Las soluciones requieren 100% de empleados nuevos
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(lExtraEmployeesRequired.Count.ToString)
                                    oState.Result = roAIPlannerState.ResultEnum.NoSolutionIncludesCurrentEmployees
                                Else
                                    ' En teoría esto no ocurrirá nunca. A base de añadir empleados siempre debería haber solución
                                    oState.Result = roAIPlannerState.ResultEnum.NoSolutionNoExplanation
                                End If
                            End If
                        Else
                            ' No se encontró solución en los tiempos establecidos
                            oState.Result = roAIPlannerState.ResultEnum.NoSolutionInTime
                        End If
                    End If
                Else
                    bolRet = False
                    oState.Result = roAIPlannerState.ResultEnum.NoBudgetDataProvided
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAIPlannerManager::SolveProblem")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAIPlannerManager::SolveProblem")
                bolRet = False
            End Try
            oProblemSolution.msg = oState.ErrorText
            Return bolRet
        End Function

        Private Function GetShiftUID(oShiftData As roCalendarRowShiftData) As String
            Return roCalendarRowPeriodDataManager.GetUniqueKey(oShiftData, New roCalendarRowPeriodDataState(Me.State.IDPassport))
        End Function

        Public Function GetAIProblem(oBudget As roBudget, ByRef strEmployees As String) As roAIPlannerProblem
            Dim oBudgedState As New roBudgetState(State.IDPassport)
            Dim oProblem As New roAIPlannerProblem

            Try
                ' Recuperamos información
                ' 1.- Periodo
                oProblem.StartDate = oBudget.FirstDay
                oProblem.EndDate = oBudget.LastDay
                ' 2.- Horarios, puestos y coberturas
                Dim oCovers As New List(Of roAIDayCover)
                Dim oDayCover As roAIDayCover

                ' Recorro las diferentes UP del presupuesto
                Dim sPositionID As String = String.Empty
                Dim sNum As String = String.Empty
                Dim oVirtualAssignment As roAIVirtualAssignment
                Dim iPeriodDaysProcessed As Integer = 0
                ' Obtenemos coberturas requeridas, e información de puestos y horarios
                For Each oBD As roBudgetRow In oBudget.BudgetData
                    For Each oDD As roBudgetRowDayData In oBD.PeriodData.DayData
                        iPeriodDaysProcessed = iPeriodDaysProcessed + 1
                        If Not oDD.ProductiveUnitMode Is Nothing Then
                            For Each oPUM As roProductiveUnitModePosition In oDD.ProductiveUnitMode.UnitModePositions
                                ' Puesto: sólo me quedo con posiciones no cubiertas
                                If oPUM.EmployeesData.Count < oPUM.Quantity Then
                                    oVirtualAssignment = New roAIVirtualAssignment(oPUM.AssignmentData.ShortName, oPUM.ID, GetShiftUID(oPUM.ShiftData), oBD.ProductiveUnitData.IDNode, oBD.ProductiveUnitData.ProductiveUnit.ID, oPUM.AssignmentData.ID)
                                    oDayCover = New roAIDayCover
                                    oDayCover.Cover = New roAICover(oVirtualAssignment, oPUM.Quantity - oPUM.EmployeesData.Count)
                                    oDayCover.Day = oDD.PlanDate
                                    oProblem.CoverRequirements.Add(oDayCover)
                                    If oProblem.Assignments.FindAll(Function(x) x.Name = oPUM.AssignmentData.ShortName).Count = 0 Then
                                        oProblem.Assignments.Add(New DTOs.roAIAssignment(oPUM.AssignmentData.ID, oPUM.AssignmentData.ShortName, oPUM.AssignmentData))
                                    End If
                                    ' Horarios
                                    If oProblem.Shifts.FindAll(Function(x) x.UID = GetShiftUID(oPUM.ShiftData)).Count = 0 Then
                                        oProblem.Shifts.Add(New roAIShift(oPUM.ShiftData.ID, GetShiftUID(oPUM.ShiftData), oPUM.ShiftData))
                                    End If
                                End If
                            Next
                        End If
                    Next
                Next

                ' Si no hay necesidades, no hace falta seguir
                If oProblem.CoverRequirements.Count > 0 AndAlso oProblem.Assignments.Count > 0 AndAlso oProblem.Shifts.Count > 0 Then
                    ' 3.- Empleados del nodo al que se ha asignado el presupuesto, con su ID, conjunto de reglas (el que venga del convenio o el suyo propio) y lista de Puestos
                    '     Contemplo todos los empleados que:
                    '             - tengan uno de los puestos que se requieren
                    '             - tengan contrato algún día del periodo a planificar. Luego, si hay días sin contrato, le añadiré al empleado un DayOff
                    Dim sAssignmentsFilter As String = String.Empty
                    Dim oEmplAssignmentArray() As String
                    ReDim oEmplAssignmentArray(oProblem.Assignments.Count - 1)
                    For i = 0 To oProblem.Assignments.Count - 1
                        oEmplAssignmentArray(i) = oProblem.Assignments(i).Name
                    Next
                    sAssignmentsFilter = "'" & String.Join(",", oEmplAssignmentArray).Replace(",", "','") & "'"

                    strEmployees = VTBudgets.roBudgetManager.GetEmployeeListFromNode(oBudget.BudgetData(0).ProductiveUnitData.IDNode, oBudgedState, oProblem.StartDate, oProblem.EndDate)

                    If strEmployees <> String.Empty Then
                        Dim strSQL As String = String.Empty

                        ' Cargo costes de empleados
                        strSQL = "@SELECT# eufv.idemployee, cast(convert(nvarchar(10),eufv.Value) as decimal) value, eufv.Date CostDate " &
                         "from EmployeeUserFieldValues eufv " &
                         "left outer join sysrouserfields suf on eufv.FieldName = suf.FieldName " &
                         "where suf.Alias = 'sysroCostHour' and eufv.IDEmployee IN (" & strEmployees & ") " &
                         "order by eufv.Date asc"

                        Dim dtEmployeeCosts As New DataTable
                        dtEmployeeCosts = CreateDataTable(strSQL)

                        strSQL = "@SELECT# DISTINCT sysrovwAllEmployeeGroups.EmployeeName, sysrovwAllEmployeeGroups.IDEmployee, Assignments.ShortName " &
                         "FROM sysrovwAllEmployeeGroups  " &
                         "INNER JOIN EmployeeAssignments ON EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee  " &
                         "INNER JOIN EmployeeContracts ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee  " &
                         "INNER JOIN Assignments ON Assignments.ID = EmployeeAssignments.IDAssignment " &
                         "WHERE sysrovwAllEmployeeGroups.IDEmployee IN (" & strEmployees & ") And  sysrovwAllEmployeeGroups.CurrentEmployee = 1 " &
                         "AND Assignments.ShortName IN (" & sAssignmentsFilter & ")"

                        Dim dt As New DataTable
                        dt = CreateDataTable(strSQL)
                        Dim dEmployees As New Dictionary(Of Integer, roAIEmployeeSupport)
                        Dim oEmpSupport As New roAIEmployeeSupport
                        Dim oEmployee As New roAIEmployee
                        For Each oRow As DataRow In dt.Rows
                            oEmpSupport = New roAIEmployeeSupport
                            oEmpSupport.ID = roTypes.Any2Integer(oRow("IDEmployee"))
                            oEmpSupport.Name = roTypes.Any2String(oRow("EmployeeName"))
                            If Not dEmployees.ContainsKey(oEmpSupport.ID) Then
                                oEmpSupport.Assignments.Add(roTypes.Any2String(oRow("ShortName")))
                                dEmployees.Add(oEmpSupport.ID, oEmpSupport)
                            Else
                                dEmployees(oEmpSupport.ID).Assignments.Add(roTypes.Any2String(oRow("ShortName")))
                            End If
                        Next
                        For Each item As KeyValuePair(Of Integer, roAIEmployeeSupport) In dEmployees
                            oEmployee = New roAIEmployee
                            oEmployee.ID = item.Value.ID
                            oEmployee.Name = item.Value.Name
                            For Each sAssignment As String In item.Value.Assignments
                                For Each oDayCoverTemp As roAIDayCover In oProblem.CoverRequirements.FindAll(Function(x) x.Cover.VirtualAssignment.Assignment = sAssignment)
                                    If Not oEmployee.AllowedAssignment.Contains(oDayCoverTemp.Cover.VirtualAssignment.Assignment) Then oEmployee.AllowedAssignment.Add(oDayCoverTemp.Cover.VirtualAssignment.Assignment)
                                    If Not oEmployee.AllowedShifts.Contains(oDayCoverTemp.Cover.VirtualAssignment.ShiftShortName) Then oEmployee.AllowedShifts.Add(oDayCoverTemp.Cover.VirtualAssignment.ShiftShortName)
                                Next
                            Next

                            ' Añado costes
                            Dim dv As New DataView(dtEmployeeCosts)
                            dv.RowFilter = "IDEmployee = " & oEmployee.ID
                            For Each oRow As DataRow In dv.ToTable.Rows
                                oEmployee.Costs.Add(New roAIEmployeeCost(oRow("CostDate"), oRow("Value")))
                            Next

                            oProblem.Employees.Add(oEmployee)
                        Next
                    Else
                        oState.Result = roAIPlannerState.ResultEnum.NoEmployeeAvailable
                        Return Nothing
                    End If
                Else
                    oState.Result = roAIPlannerState.ResultEnum.NoCoverRequired
                    Return Nothing
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAIPlannerManager::getAIProblem")
                Return Nothing
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAIPlannerManager::getAIProblem")
                Return Nothing
            End Try
            Return oProblem
        End Function

        Private Function GetAuxEmployeeCalendar(iIDEmployee As Integer, oEmployeeScheduleRules As List(Of EmployeeScheduleRule), oCalRulesManager As roCalendarScheduleRulesManager, oCalManager As roCalendarManager, oCalendar As roCalendar, ByRef oPrevCalendarCompact As List(Of roCalendarRow), ByRef oPostCalendarCompact As List(Of roCalendarRow), ByRef dPrevStart As Date, ByRef dPostEnd As Date, dYearFirstDate As Date, iMonthIniDay As Integer, iYearIniMonth As Integer, oParameters As roParameters, ByRef dicAIRulesPeriods As Dictionary(Of ScheduleRuleScope, roAIPeriod)) As Boolean
            Dim bolRet As Boolean = False
            Dim oPrevCalendar As New roCalendar
            Dim oPostCalendar As New roCalendar
            Dim dPrevStartAux As Date = DateSerial(2079, 1, 1)
            Dim dPostEndAux As Date = DateSerial(2079, 1, 1)
            Dim oRulePeriod As New roAIPeriod

            ' En base a todas las reglas del empleado, calculo la cantidad de días anteriores y posteriores al periodo analizado que necesito.

            Try
                If oEmployeeScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.Scope = ScheduleRuleScope.Year AndAlso o.IdEmployee = iIDEmployee AndAlso o.ScheduleRule.Parameters.Enabled).Count > 0 Then
                    ' Inicio de año en curso para el primer día de periodo
                    dPrevStart = dYearFirstDate
                    ' Fin del año en curso para el último día del periodo
                    dPostEnd = oCalRulesManager.GetYearFirstDate(oCalendar.LastDay, oParameters).AddYears(1).AddDays(-1)

                    If Not dicAIRulesPeriods.ContainsKey(ScheduleRuleScope.Year) Then
                        oRulePeriod.DateStart = dPrevStart
                        oRulePeriod.DateEnd = dPostEnd
                        dicAIRulesPeriods.Add(ScheduleRuleScope.Year, oRulePeriod)
                    End If
                End If

                If oEmployeeScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.Scope = ScheduleRuleScope.Month AndAlso o.IdEmployee = iIDEmployee AndAlso o.ScheduleRule.Parameters.Enabled).Count > 0 Then
                    If oCalendar.FirstDay.Day >= iMonthIniDay Then
                        dPrevStartAux = DateSerial(oCalendar.FirstDay.Year, oCalendar.FirstDay.Month, iMonthIniDay)
                    Else
                        dPrevStartAux = DateSerial(oCalendar.FirstDay.Year, oCalendar.FirstDay.Month - 1, iMonthIniDay)
                    End If
                    ' Fin de mes en curso para el último día del periodo
                    If oCalendar.LastDay.Day >= iMonthIniDay Then
                        dPostEndAux = DateSerial(oCalendar.LastDay.Year, oCalendar.LastDay.Month, iMonthIniDay).AddMonths(1).AddDays(-1)
                    Else
                        dPostEndAux = DateSerial(oCalendar.LastDay.Year, oCalendar.LastDay.Month - 1, iMonthIniDay).AddMonths(1).AddDays(-1)
                    End If

                    If Not dicAIRulesPeriods.ContainsKey(ScheduleRuleScope.Month) Then
                        oRulePeriod.DateStart = dPrevStartAux
                        oRulePeriod.DateEnd = dPostEndAux
                        dicAIRulesPeriods.Add(ScheduleRuleScope.Month, oRulePeriod)
                    End If

                    If dPrevStartAux < dPrevStart Then dPrevStart = dPrevStartAux
                    If dPostEnd = DateSerial(2079, 1, 1) OrElse dPostEndAux > dPostEnd Then dPostEnd = dPostEndAux

                End If

                If oEmployeeScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.Scope = ScheduleRuleScope.Week AndAlso o.IdEmployee = iIDEmployee AndAlso o.ScheduleRule.Parameters.Enabled).Count > 0 Then
                    ' Lunes de la semana en curso para el primer día de periodo
                    If oCalendar.FirstDay.DayOfWeek <> DayOfWeek.Sunday Then
                        dPrevStartAux = oCalendar.FirstDay.AddDays(-1 * (oCalendar.FirstDay.DayOfWeek - 1))
                    Else
                        dPrevStartAux = oCalendar.FirstDay.AddDays(-6)
                    End If

                    'OJO: Temporal. Pufa para indicar que una regla semanal aplica a ciclos de X semanas
                    'Si alguna regla semanal contiene en su nombre
                    If oEmployeeScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.Scope = ScheduleRuleScope.Week AndAlso o.IdEmployee = iIDEmployee AndAlso o.ScheduleRule.Parameters.Enabled AndAlso o.ScheduleRule.Parameters.RuleName.Contains("#AI:")).Count > 0 Then
                        Dim xMultiplier As Integer = 1
                        ' Me quedo con el multiplicador mayor
                        Try
                            For Each oRuleTemp As EmployeeScheduleRule In oEmployeeScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.Scope = ScheduleRuleScope.Week AndAlso o.IdEmployee = iIDEmployee AndAlso o.ScheduleRule.Parameters.Enabled AndAlso o.ScheduleRule.Parameters.RuleName.Contains("#AI:"))
                                If oRuleTemp.ScheduleRule.Parameters.RuleName.Substring(oRuleTemp.ScheduleRule.Parameters.RuleName.IndexOf("AI:") + 3, 1) > xMultiplier Then xMultiplier = oRuleTemp.ScheduleRule.Parameters.RuleName.Substring(oRuleTemp.ScheduleRule.Parameters.RuleName.IndexOf("AI:") + 3, 1)
                            Next
                        Catch ex As Exception
                        End Try

                        ' Domingo de la semana en curso para el último día de periodo
                        If oCalendar.LastDay.DayOfWeek <> DayOfWeek.Sunday Then
                            dPostEndAux = oCalendar.LastDay.AddDays(7 - (oCalendar.LastDay.DayOfWeek)).AddDays(7 * (xMultiplier - 1))
                        Else
                            dPostEndAux = oCalendar.LastDay.AddDays(7 * (xMultiplier - 1))
                        End If
                    Else
                        ' Domingo de la semana en curso para el último día de periodo
                        If oCalendar.LastDay.DayOfWeek <> DayOfWeek.Sunday Then
                            dPostEndAux = oCalendar.LastDay.AddDays(7 - (oCalendar.LastDay.DayOfWeek))
                        Else
                            dPostEndAux = oCalendar.LastDay
                        End If
                    End If

                    If Not dicAIRulesPeriods.ContainsKey(ScheduleRuleScope.Week) Then
                        oRulePeriod.DateStart = dPrevStartAux
                        oRulePeriod.DateEnd = dPostEndAux
                        dicAIRulesPeriods.Add(ScheduleRuleScope.Week, oRulePeriod)
                    End If

                    If dPrevStartAux < dPrevStart Then dPrevStart = dPrevStartAux
                    If dPostEnd = DateSerial(2079, 1, 1) OrElse dPostEndAux > dPostEnd Then dPostEnd = dPostEndAux
                End If

                ' De cara a la colección dicRules, sólo hay que considerar los scopes anual, mensual y semanal. Para el resto, el periodo de la regla es el del calendario completo
                If oEmployeeScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.Scope = ScheduleRuleScope.Always AndAlso o.IdEmployee = iIDEmployee AndAlso o.ScheduleRule.Parameters.Enabled).Count > 0 Then
                    If Not dicAIRulesPeriods.ContainsKey(ScheduleRuleScope.Always) Then
                        oRulePeriod.DateStart = oCalendar.FirstDay
                        oRulePeriod.DateEnd = oCalendar.LastDay
                        dicAIRulesPeriods.Add(ScheduleRuleScope.Always, oRulePeriod)
                    End If
                End If

                If oEmployeeScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.Scope = ScheduleRuleScope.Day AndAlso o.IdEmployee = iIDEmployee AndAlso o.ScheduleRule.Parameters.Enabled).Count > 0 Then
                    If Not dicAIRulesPeriods.ContainsKey(ScheduleRuleScope.Day) Then
                        oRulePeriod.DateStart = oCalendar.FirstDay
                        oRulePeriod.DateEnd = oCalendar.LastDay
                        dicAIRulesPeriods.Add(ScheduleRuleScope.Day, oRulePeriod)
                    End If
                End If

                If oEmployeeScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.Scope = ScheduleRuleScope.Selection AndAlso o.IdEmployee = iIDEmployee AndAlso o.ScheduleRule.Parameters.Enabled).Count > 0 Then
                    If Not dicAIRulesPeriods.ContainsKey(ScheduleRuleScope.Selection) Then
                        oRulePeriod.DateStart = oCalendar.FirstDay
                        oRulePeriod.DateEnd = oCalendar.LastDay
                        dicAIRulesPeriods.Add(ScheduleRuleScope.Selection, oRulePeriod)
                    End If
                End If

                If dPrevStart <> DateSerial(2079, 1, 1) Then
                    oPrevCalendar = oCalManager.Load(dPrevStart, oCalendar.FirstDay.AddDays(-1), "B" & iIDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)
                End If
                If Not oPrevCalendar.CalendarData Is Nothing AndAlso oPrevCalendar.CalendarData.Count >= 1 Then
                    oPrevCalendarCompact = New List(Of roCalendarRow)
                    oPrevCalendarCompact = oCalRulesManager.CompactCalendar(oPrevCalendar)
                End If

                If dPostEnd <> DateSerial(2079, 1, 1) Then
                    oPostCalendar = oCalManager.Load(oCalendar.LastDay.AddDays(1), dPostEnd, "B" & iIDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)
                End If
                If Not oPostCalendar.CalendarData Is Nothing AndAlso oPostCalendar.CalendarData.Count >= 1 Then
                    oPostCalendarCompact = New List(Of roCalendarRow)
                    oPostCalendarCompact = oCalRulesManager.CompactCalendar(oPostCalendar)
                End If

                Return True
            Catch ex As Exception
                oState.Result = roAIPlannerState.ResultEnum.Exception
                Return False
            End Try
        End Function

        Private Function GetEmployeeStatusOnDay(oDayData As roCalendarRowDayData, idEmployee As Integer, oEmpRulesAuxData As roEmployeeRulesData, oEmpRules As List(Of EmployeeScheduleRule), dStartDate As Date, dEndDate As Date) As roAIEmployeeDayStatus
            Dim oRet As New roAIEmployeeDayStatus

            Try

                oRet.Status = EmployeeAIDayStatus.Available
                oRet.Shift = "*"

                If oDayData.MainShift Is Nothing AndAlso oDayData.EmployeeStatusOnDay <> EmployeeStatusOnDayEnum.NoContract Then
                    If oDayData.PlanDate >= dStartDate AndAlso oDayData.PlanDate <= dEndDate Then
                        ' En el periodo que estamos planificando, un día sin planificar es un día en que el empleado puede venir
                        oRet.Status = EmployeeAIDayStatus.Available
                    Else
                        ' Fuera del periodo que estamos planificando, un día sin planificar es un día en que el empleado no trabajó.
                        oRet.Status = EmployeeAIDayStatus.Absent
                        oRet.Shift = ""
                    End If
                ElseIf oDayData.IDDailyBudgetPosition > 0 Then
                    ' Ya está asignado. Informamos el horario concreto para que se tenga en cuenta en las reglas
                    If Not oDayData.MainShift Is Nothing Then oRet.Shift = GetShiftUID(oDayData.MainShift)
                    oRet.Status = EmployeeAIDayStatus.Present
                    oRet.Assignment = oDayData.AssigData.ShortName
                Else
                    Select Case oDayData.EmployeeStatusOnDay
                        Case EmployeeStatusOnDayEnum.NoContract
                            oRet.Status = EmployeeAIDayStatus.Absent
                        Case EmployeeStatusOnDayEnum.InOtherDepartment
                        ' Este caso no se puede dar, dado que el calendario está compactado
                        Case EmployeeStatusOnDayEnum.Ok
                            'Miramos si tiene alguna previsión
                            If Not oDayData.Alerts Is Nothing AndAlso oDayData.Alerts.OnHolidays Then
                                oRet.Status = EmployeeAIDayStatus.Absent
                            ElseIf Not oDayData.Alerts Is Nothing AndAlso oDayData.Alerts.OnAbsenceDays Then
                                oRet.Status = EmployeeAIDayStatus.Absent
                            ElseIf Not oDayData.Alerts Is Nothing AndAlso oDayData.Alerts.OnAbsenceHours Then
                                oRet.Status = EmployeeAIDayStatus.Absent
                            End If
                    End Select
                End If

                If oRet.Status = EmployeeAIDayStatus.Available Then
                    ' Empleado disponible en principio. Verificamos festivos, fines de semana y bloqueo ...

                    Dim oEmpMiniContract As roMiniContract = Nothing
                    oEmpMiniContract = oEmpRulesAuxData.Contracts.Find(Function(x) x.IdEmployee = idEmployee AndAlso oDayData.PlanDate >= x.BeginDate AndAlso oDayData.PlanDate <= x.EndDate)
                    Dim bCanWorkOnFeast As Boolean = False
                    bCanWorkOnFeast = (oEmpRules.FindAll(Function(z) z.IdEmployee = idEmployee AndAlso oDayData.PlanDate >= z.StartDate AndAlso oDayData.PlanDate <= z.EndDate AndAlso z.ScheduleRule.Parameters.IDRule = ScheduleRuleType.WorkOnFestive AndAlso z.ScheduleRule.Parameters.Enabled).Count = 0)
                    ' Si es festivo y no puedo trabajar ...
                    If oDayData.Feast Then
                        If bCanWorkOnFeast Then
                            oRet.Status = EmployeeAIDayStatus.Available
                        Else
                            oRet.Status = EmployeeAIDayStatus.Absent
                        End If
                        Return oRet
                    End If

                    ' Si no se puede trabajar en fin de semana ...
                    Dim oEmpWeekendDefinition As roWeekendDefinition = Nothing
                    ' Miro si hay definición de fin de semana por contrato ...
                    oEmpWeekendDefinition = oEmpRulesAuxData.WeekendDefinitions.Find(Function(y) y.IdContract = oEmpMiniContract.IdContract)
                    If oEmpWeekendDefinition Is Nothing Then
                        '... y si no por convenio
                        oEmpWeekendDefinition = oEmpRulesAuxData.WeekendDefinitions.Find(Function(y) y.IdLabAgree = oEmpMiniContract.IdLabAgree)
                    End If

                    If Not oEmpWeekendDefinition Is Nothing AndAlso oEmpWeekendDefinition.WeekEndDays.Contains(oDayData.PlanDate.DayOfWeek) Then
                        ' Es fin de semana ...
                        If Not oEmpWeekendDefinition.CanWorkOn Then
                            ' Da
                            oRet.Status = EmployeeAIDayStatus.Absent
                        Else
                            oRet.Status = EmployeeAIDayStatus.Available
                        End If
                    Else
                        ' No es fin de semana o bien no está definido el fin de semana. Independientemente de si el horario del día tiene teóricas o no, dado que el día no es ni festivo ni fin de semana, el empleado está disponible
                        oRet.Status = EmployeeAIDayStatus.Available
                    End If

                    If oDayData.Locked AndAlso oRet.Status = EmployeeAIDayStatus.Available Then
                        oRet.Status = EmployeeAIDayStatus.Available
                        If Not oDayData.AssigData Is Nothing Then
                            oRet.Assignment = oDayData.AssigData.ShortName
                        Else
                            oRet.Assignment = ""
                        End If
                        If Not oDayData.MainShift Is Nothing Then
                            'oRet.Shift = oDayData.MainShift.ShortName
                            oRet.Shift = GetShiftUID(oDayData.MainShift)
                        Else
                            oRet.Shift = ""
                        End If
                    End If
                End If
            Catch ex As Exception
                oState.Result = roAIPlannerState.ResultEnum.Exception
            End Try
            Return oRet
        End Function

        ''' <summary>
        ''' DEPRECADA: No es necesario localizar IDBudgetPosition a partir de Nodo y UnidadProductiva, porque es parte de la necesidad
        ''' </summary>
        ''' <param name="oAISolution"></param>
        ''' <param name="oProblem"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function GetSolutionCalendar(oAISolution As roAIPlannerSolution, oProblem As roAIPlannerProblem) As roCalendar
            Dim oCalendar As New roCalendar
            Dim bolRet As Boolean = False

            Try

                Dim oCalManager As VTCalendar.roCalendarManager = Nothing
                oCalManager = New VTCalendar.roCalendarManager(New VTCalendar.roCalendarState(oState.IDPassport)) 'TOD: Passport
                Dim sShiftID As String = String.Empty
                Dim sShiftStartHour As String = String.Empty
                Dim sAssignmentID As String = String.Empty
                Dim sProductiveUnitID As String = String.Empty
                Dim sIDNode As String = String.Empty
                Dim iPos As Integer = 0
                Dim lAvailablePositions As New List(Of roAIAvailablePosition)

                ' Calculo el ámbito de empleados y fechas que voy a modificar
                Dim dStartDate As Date = oAISolution.Assignments.OrderBy(Function(x) x.Date).First.Date
                Dim dEndDate As Date = oAISolution.Assignments.OrderByDescending(Function(x) x.Date).First.Date
                Dim dEmployees As New List(Of Integer)
                Dim dCompactAvailablePositions As New Dictionary(Of String, roAIAvailablePosition)
                For Each oAssignment As DTOs.AIPlanner.roAIAssignment In oAISolution.Assignments
                    If Not dEmployees.Contains(oAssignment.IdEmployee) Then dEmployees.Add(oAssignment.IdEmployee)
                    Dim oAvPosition As New roAIAvailablePosition(0, oAssignment.Date, oAssignment.IdShift, oAssignment.IdAssignment, oAssignment.IdNode, oAssignment.IdProductiveUnit)
                    If Not dCompactAvailablePositions.ContainsKey(oAvPosition.Key) Then dCompactAvailablePositions.Add(oAvPosition.Key, oAvPosition)
                Next

                ' Calculo posiciones disponibles
                For Each item As KeyValuePair(Of String, roAIAvailablePosition) In dCompactAvailablePositions
                    Dim strSQL As String = String.Empty
                    Dim dt As New DataTable
                    strSQL = "@SELECT# DBP.ID as PosID, DBP.Quantity as Required, count(*) total, min(IDDailyBudgetPosition) idpos from DailyBudgets DB " &
                             "inner join Productiveunits PU on PU.ID = DB.IDProductiveUnit " &
                             "inner join DailyBudget_Positions DBP on DBP.IDDailyBudget = DB.ID  " &
                             "left join DailySchedule DS On DS.IDDailyBudgetPosition = DBP.ID  " &
                             "where  DB.Date = " & roTypes.Any2Time(item.Value.Date).SQLSmallDateTime & " and dbp.IDShift = " & item.Value.IdShift.ToString & " and dbp.IDAssignment = " & item.Value.IdAssignment.ToString & " and DB.IDNode = " & item.Value.IdNode.ToString & " " & " and DB.IdProductiveUnit = " & item.Value.IdProductiveUnit.ToString & " " &
                             " group by DBP.ID, Quantity"
                    dt = CreateDataTable(strSQL, "aux")

                    For Each oRow In dt.Rows
                        If IsDBNull(oRow("idpos")) Then
                            For i = 1 To oRow("Required")
                                lAvailablePositions.Add(New roAIAvailablePosition(oRow("PosID"), item.Value.Date, item.Value.IdShift, item.Value.IdAssignment, item.Value.IdNode, item.Value.IdProductiveUnit))
                            Next
                        Else
                            Dim itotal As Integer = oRow("total")
                            While itotal < oRow("Required")
                                lAvailablePositions.Add(New roAIAvailablePosition(oRow("PosID"), item.Value.Date, item.Value.IdShift, item.Value.IdAssignment, item.Value.IdNode, item.Value.IdProductiveUnit))
                                itotal = itotal + 1
                            End While
                        End If
                    Next
                Next

                ' 2.- Planifico Horario - Puesto - Posición para el Empleado - Fecha
                '     Cargo calendario para el empleado y día en cuestión
                Dim oAssignmentAux As New DTOs.AIPlanner.roAIAssignment
                Dim lAlreadyAssignedPositions As New List(Of String)
                oCalendar = oCalManager.Load(dStartDate, dEndDate, "B" & String.Join(",", dEmployees).Replace(",", ",B"), DTOs.CalendarView.Planification, DTOs.CalendarDetailLevel.Daily, True)
                For Each idEmployee As Integer In dEmployees
                    For Each oCalendarRow As roCalendarRow In oCalendar.CalendarData.ToList.FindAll(Function(x) x.EmployeeData.IDEmployee = idEmployee)
                        For Each oDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAIPlannerManager::GetSolutionCalendar::Employee=" & idEmployee.ToString & " Date= " & oDayData.PlanDate.ToShortDateString)
                            If oAISolution.Assignments.FindAll(Function(x) x.IdEmployee = idEmployee AndAlso x.Date = oDayData.PlanDate).Count > 0 Then
                                oAssignmentAux = oAISolution.Assignments.Find(Function(x) x.IdEmployee = idEmployee AndAlso x.Date = oDayData.PlanDate)
                                If oDayData.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.Ok Then
                                    ' Si no estaba asignada la marco como usada
                                    If Not lAvailablePositions.Find(Function(x) x.Date = oDayData.PlanDate AndAlso x.IdShift = oAssignmentAux.IdShift AndAlso x.IdAssignment = oAssignmentAux.IdAssignment AndAlso x.IdProductiveUnit = oAssignmentAux.IdProductiveUnit AndAlso x.IdNode = oAssignmentAux.IdNode AndAlso x.Assigned = False) Is Nothing Then
                                        oDayData.MainShift = oProblem.Shifts.Find(Function(x) x.UID = oAssignmentAux.ShiftUID).ShiftData
                                        Dim oAssigCellData As New roCalendarAssignmentCellData
                                        oAssigCellData.ID = oAssignmentAux.IdAssignment
                                        oDayData.AssigData = oAssigCellData
                                        oDayData.IDDailyBudgetPosition = lAvailablePositions.Find(Function(x) x.Date = oDayData.PlanDate AndAlso x.IdShift = oAssignmentAux.IdShift AndAlso x.IdAssignment = oAssignmentAux.IdAssignment AndAlso x.IdNode = oAssignmentAux.IdNode AndAlso x.IdProductiveUnit = oAssignmentAux.IdProductiveUnit AndAlso x.Assigned = False).IdPosition
                                        lAvailablePositions.Find(Function(x) x.Date = oDayData.PlanDate AndAlso x.IdShift = oAssignmentAux.IdShift AndAlso x.IdAssignment = oAssignmentAux.IdAssignment AndAlso x.IdNode = oAssignmentAux.IdNode AndAlso x.IdProductiveUnit = oAssignmentAux.IdProductiveUnit AndAlso x.Assigned = False).Assigned = True
                                        oDayData.HasChanged = True
                                    Else
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAIPlannerManager::GetSolutionCalendar::Employee=" & idEmployee.ToString & " Date= " & oDayData.PlanDate.ToShortDateString)
                                    End If
                                End If
                            End If
                        Next
                    Next
                Next

                bolRet = True
            Catch ex As Exception
                bolRet = False
            End Try

            Return oCalendar
        End Function

        Public Function GetSolutionCalendarEx(oAISolution As roAIPlannerSolution, oProblem As roAIPlannerProblem) As roCalendar
            Dim oCalendar As New roCalendar
            Dim bolRet As Boolean = False

            Try
                Dim oCalManager As VTCalendar.roCalendarManager = Nothing
                oCalManager = New VTCalendar.roCalendarManager(New VTCalendar.roCalendarState(oState.IDPassport)) 'TOD: Passport

                ' Calculo el ámbito de empleados y fechas que voy a modificar
                Dim dStartDate As Date = oAISolution.Assignments.OrderBy(Function(x) x.Date).First.Date
                Dim dEndDate As Date = oAISolution.Assignments.OrderByDescending(Function(x) x.Date).First.Date
                Dim dEmployees As New List(Of Integer)

                For Each oAssignment As DTOs.AIPlanner.roAIAssignment In oAISolution.Assignments
                    If Not dEmployees.Contains(oAssignment.IdEmployee) Then dEmployees.Add(oAssignment.IdEmployee)
                Next

                ' 2.- Planifico Horario - Puesto - Posición para el Empleado - Fecha
                '     Cargo calendario para el empleado y día en cuestión
                Dim oAssignmentAux As New DTOs.AIPlanner.roAIAssignment
                Dim lAlreadyAssignedPositions As New List(Of String)
                oCalendar = oCalManager.Load(dStartDate, dEndDate, "B" & String.Join(",", dEmployees).Replace(",", ",B"), DTOs.CalendarView.Planification, DTOs.CalendarDetailLevel.Daily, True)
                For Each idEmployee As Integer In dEmployees
                    For Each oCalendarRow As roCalendarRow In oCalendar.CalendarData.ToList.FindAll(Function(x) x.EmployeeData.IDEmployee = idEmployee)
                        For Each oDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                            If oAISolution.Assignments.FindAll(Function(x) x.IdEmployee = idEmployee AndAlso x.Date = oDayData.PlanDate).Count > 0 Then
                                oAssignmentAux = oAISolution.Assignments.Find(Function(x) x.IdEmployee = idEmployee AndAlso x.Date = oDayData.PlanDate)
                                If oDayData.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.Ok Then
                                    ' Si no estaba asignada la marco como usada

                                    oDayData.MainShift = oProblem.Shifts.Find(Function(x) x.UID = oAssignmentAux.ShiftUID).ShiftData
                                    Dim oAssigCellData As New roCalendarAssignmentCellData
                                    oAssigCellData.ID = oAssignmentAux.IdAssignment
                                    oDayData.AssigData = oAssigCellData
                                    ' Busco el IdDailyBudgetPosition de entre los no asignados en el problema
                                    If Not oProblem.CoverRequirements.Find(Function(x) x.Day = oDayData.PlanDate AndAlso x.Cover.VirtualAssignment.IDNode = oAssignmentAux.IdNode AndAlso x.Cover.VirtualAssignment.IDProductiveUnit = oAssignmentAux.IdProductiveUnit AndAlso x.Cover.VirtualAssignment.ShiftShortName = oAssignmentAux.ShiftUID AndAlso x.Cover.VirtualAssignment.IdAssignment = oAssignmentAux.IdAssignment AndAlso x.Cover.Pending > 0) Is Nothing Then
                                        oDayData.IDDailyBudgetPosition = oProblem.CoverRequirements.Find(Function(x) x.Day = oDayData.PlanDate AndAlso x.Cover.VirtualAssignment.IDNode = oAssignmentAux.IdNode AndAlso x.Cover.VirtualAssignment.IDProductiveUnit = oAssignmentAux.IdProductiveUnit AndAlso x.Cover.VirtualAssignment.ShiftShortName = oAssignmentAux.ShiftUID AndAlso x.Cover.VirtualAssignment.IdAssignment = oAssignmentAux.IdAssignment AndAlso x.Cover.Pending > 0).Cover.VirtualAssignment.IDDailyBudgetPosition
                                        oProblem.CoverRequirements.Find(Function(x) x.Day = oDayData.PlanDate AndAlso x.Cover.VirtualAssignment.IDNode = oAssignmentAux.IdNode AndAlso x.Cover.VirtualAssignment.IDProductiveUnit = oAssignmentAux.IdProductiveUnit AndAlso x.Cover.VirtualAssignment.ShiftShortName = oAssignmentAux.ShiftUID AndAlso x.Cover.VirtualAssignment.IdAssignment = oAssignmentAux.IdAssignment AndAlso x.Cover.Pending > 0).Cover.Pending -= 1
                                    End If
                                    oDayData.HasChanged = True
                                End If
                            End If
                        Next
                    Next
                Next

                bolRet = True
            Catch ex As Exception
                bolRet = False
            End Try

            Return oCalendar
        End Function

        Private Function GetIntervalIntersection(date1begin As Date, date1end As Date, date2begin As Date, date2end As Date) As roAIPeriod
            Dim oRet As New roAIPeriod
            Try
                oRet.DateStart = New DateTime(Math.Max(date1begin.Ticks, date2begin.Ticks))
                oRet.DateEnd = New DateTime(Math.Min(date1end.Ticks, date2end.Ticks))
                If oRet.DateStart <= oRet.DateEnd Then
                    Return oRet
                Else
                    oRet.DateStart = Date.MinValue
                    oRet.DateEnd = Date.MaxValue
                End If
            Catch ex As Exception
                oRet.DateStart = Date.MinValue
                oRet.DateEnd = Date.MaxValue
            End Try
            Return oRet
        End Function

        Private Function GetRuleDates(dStartDate As Date, dEndDate As Date, oScope As ScheduleRuleScope, sRuleName As String) As List(Of roAIPeriod)
            Dim oRet As New List(Of roAIPeriod)
            Dim dFirstDate As Date = dStartDate
            Dim dLastDate As Date = dEndDate
            Dim oPI As roAIPeriod
            Try
                Select Case oScope
                    Case ScheduleRuleScope.Month
                        While dFirstDate <= dEndDate
                            dLastDate = dFirstDate.AddMonths(1).AddDays(-1)
                            oPI = New roAIPeriod()
                            oPI.DateStart = dFirstDate
                            If dFirstDate.AddMonths(1) <= dEndDate Then
                                oPI.DateEnd = dLastDate
                            Else
                                oPI.DateEnd = dEndDate
                            End If
                            oRet.Add(oPI)
                            dFirstDate = dFirstDate.AddMonths(1)
                        End While
                    Case ScheduleRuleScope.Week
                        'OJO: Temporal. Pufa para indicar que una regla semanal aplica a ciclos de X semanas. Ejemplo: "Días libres semanales #AI:2" -> cada 2 semanas
                        ' Recupero el multiplicador
                        Dim xMultiplier As Integer = 1
                        Try
                            If sRuleName.Contains("AI:") Then
                                xMultiplier = sRuleName.Substring(sRuleName.IndexOf("AI:") + 3, 1)
                            End If
                        Catch ex As Exception
                        End Try

                        While dFirstDate <= dEndDate
                            'dLastDate = dFirstDate.AddDays(6)
                            dLastDate = dFirstDate.AddDays((7 * xMultiplier) - 1)
                            oPI = New roAIPeriod()
                            oPI.DateStart = dFirstDate
                            'If dFirstDate.AddDays(7) <= dEndDate Then
                            If dFirstDate.AddDays(7 * xMultiplier) <= dEndDate Then
                                oPI.DateEnd = dLastDate
                            Else
                                oPI.DateEnd = dEndDate
                            End If
                            oRet.Add(oPI)
                            'dFirstDate = dFirstDate.AddDays(7)
                            dFirstDate = dFirstDate.AddDays(7 * xMultiplier)
                        End While
                    Case Else
                        oPI = New roAIPeriod()
                        oPI.DateStart = dStartDate
                        oPI.DateEnd = dEndDate
                        oRet.Add(oPI)
                End Select
            Catch ex As Exception
                oPI = New roAIPeriod()
                oPI.DateStart = dStartDate
                oPI.DateEnd = dEndDate
                oRet.Add(oPI)
            End Try
            Return oRet
        End Function

        Private Sub CreateUserTask(oRule As EmployeeScheduleRule)
            Dim oState As New UserTask.roUserTaskState()
            Dim oTaskExist As New UserTask.roUserTask("USERTASK:\\AISCHEDULING_RULE_NOT_SUPPORTED_" & oRule.ScheduleRule.Parameters.IDRule, oState)
            If oTaskExist.Message = "" Then
                Dim oTask As New UserTask.roUserTask()
                With oTask
                    .ID = "USERTASK:\\AISCHEDULING_RULE_NOT_SUPPORTED_" & oRule.ScheduleRule.Parameters.IDRule
                    Me.oState.Language.ClearUserTokens()
                    Me.oState.Language.AddUserToken(Me.oState.Language.Translate("ScheduleRuleType." & [Enum].GetName(GetType(ScheduleRuleType), oRule.ScheduleRule.Parameters.IDRule), ""))
                    Dim arrList As New ArrayList
                    .Message = Me.oState.Language.Translate("AI.ScheduleRuleNotSupported.Title", "")
                    Me.oState.Language.ClearUserTokens()
                    .DateCreated = Now
                    .TaskType = UserTask.TaskType.UserTaskRepair
                    .ResolverURL = "" '"FN:\\Resolver_Terminal_Unrecognized"
                    '.ResolverVariable1 = "TerminalSN" : .ResolverValue1 = mTerminal.SerialNum.ToString
                    '.ResolverVariable2 = "Type" : .ResolverValue2 = mTerminal.TerminalDBType.ToString
                    '.ResolverVariable3 = "Location" : .ResolverValue3 = mTerminal.IP
                    .Save()
                End With
            End If
        End Sub

        Protected Overridable Sub DelUserTask()
            Dim oState As New UserTask.roUserTaskState()
            Dim oTaskExist As New UserTask.roUserTask("USERTASK:\\AISCHEDULING", oState)
            'Si existe la tarea la borramos
            If oTaskExist.Message <> "" Then
                oTaskExist.Delete()
            End If
        End Sub

    End Class

    Public Class roAIEmployeeSupport
        Public ID As New Integer
        Public Name As String
        Public Assignments As New List(Of String)
    End Class

    Public Class roAIAvailablePosition
        Public IdPosition As Integer
        Public [Date] As Date
        Public IdShift As Integer
        Public IdAssignment As Integer
        Public IdNode As Integer
        Public IdProductiveUnit As Integer
        Public Assigned As Boolean

        Public Sub New(iIDPosition As Integer, dDate As Date, iIDShift As Integer, iIDAssignment As Integer, iIDNode As Integer, iIDProductiveUnit As Integer)
            IdPosition = iIDPosition
            IdShift = iIDShift
            IdAssignment = iIDAssignment
            IdNode = iIDNode
            [Date] = dDate
            Assigned = False
            IdProductiveUnit = iIDProductiveUnit
        End Sub

        Public Function Key() As String
            Return [Date].ToShortDateString & "#" & IdShift.ToString & "#" & IdAssignment.ToString & "#" & IdNode.ToString & "#" & IdProductiveUnit.ToString
        End Function

    End Class

    Public Structure roAIPeriod
        Public DateStart As Date
        Public DateEnd As Date
    End Structure

    Public Class roAIProblemSolution
        Public msg As String
        Public file As String
    End Class

End Namespace