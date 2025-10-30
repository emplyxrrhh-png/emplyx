Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Web.UI.WebControls
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTCalendar

    Public Class roCalendarScheduleRulesManager

        Private oState As roCalendarScheduleRulesState = Nothing

        Public ReadOnly Property State
            Get
                Return oState
            End Get
        End Property

        Public Sub New(ByVal _State As roCalendarScheduleRulesState)
            Me.oState = _State
        End Sub

#Region "Methods and functions"

        Public Function GetUserScheduleRulesTypes() As List(Of Integer)
            Dim oRet As New Generic.List(Of Integer)
            Try
                'Para todas las reglas (diferenciadas por su idrule, de existir una a nivel de contrato, no debo considerar más que las de contrato (e ingnorar por tanto las de convenio)
                Dim strSQL As String = String.Empty
                strSQL = "@SELECT# ID from sysroScheduleRulesTypes WHERE System = 0"

                Dim dtTable As New DataTable
                dtTable = CreateDataTable(strSQL)

                For Each oRow As DataRow In dtTable.Rows
                    oRet.Add(roTypes.Any2Integer(oRow("ID")))
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetUserScheduleRulesTypes")
                Return Nothing
            End Try

            Return oRet
        End Function

        Public Function CheckScheduleRules(oCalendar As roCalendar, Optional dLabAgreeRulesToCheck As Dictionary(Of Integer, List(Of Integer)) = Nothing, Optional strTypeRules As String = "", Optional dLabAgreeActualIdRulesToCheck As Dictionary(Of Integer, List(Of Integer)) = Nothing) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oEmployeesScheduleRules As New List(Of EmployeeScheduleRule)
            Dim oLastShiftSequence As New Queue(Of Robotics.Base.DTOs.roCalendarRowDayData)(2)
            Dim iMonthIniDay As Integer
            Dim iYearIniMonth As Integer
            Dim dYearFirstDate As Date = DateSerial(2000, 1, 1)
            Dim dChekRuleDateStart As Date
            Dim dChekRuleDateEnd As Date

            Try

                ' Si no hay datos, salgo
                If oCalendar.CalendarData.Length = 0 Then Return oRet

                ' Calculo días de inicio de año y día de inicio de mes
                Dim oParameters As New roParameters("OPTIONS", True)
                dYearFirstDate = GetYearFirstDate(oCalendar.FirstDay, oParameters, iMonthIniDay, iYearIniMonth)

                ' Recupero Ids de empleados incluidos en el calendario
                Dim strEmployeeIds As String = GetCalendarEmployeesIds(oCalendar)

                ' Cargo todas las reglas activas de los empleados que están en el calendario
                oEmployeesScheduleRules = GetEmployeesScheduleRules(oCalendar.FirstDay, oCalendar.LastDay, strEmployeeIds, dYearFirstDate,, dLabAgreeRulesToCheck, strTypeRules, dLabAgreeActualIdRulesToCheck)

                ' Recopilo información adicional en función de los empleados y reglas
                Dim oEmployeesRulesAuxData As New roEmployeeRulesData(oEmployeesScheduleRules, oCalendar.FirstDay, oCalendar.LastDay, dYearFirstDate, strEmployeeIds, Me.oState)

                Dim oCalendarCompact As New List(Of roCalendarRow)
                oCalendarCompact = CompactCalendar(oCalendar)

                Dim oState = New roCalendarState(Me.oState.IDPassport)
                Dim oCalendarManager = New roCalendarManager(oState)

                dChekRuleDateStart = oCalendar.FirstDay
                dChekRuleDateEnd = oCalendar.LastDay

                For Each oCalRow As Robotics.Base.DTOs.roCalendarRow In oCalendarCompact
                    ' Reviso todas las reglas que el empleado pueda tener asignadas a través de su convenio o contrato.
                    ' KEEP IN MIND: Si el empleado tiene varios contratos en el periodo, habrá varias reglas ...
                    For Each oEmpScheduleRule As EmployeeScheduleRule In oEmployeesScheduleRules.FindAll(Function(o) o.IdEmployee = oCalRow.EmployeeData.IDEmployee AndAlso o.ScheduleRule.Parameters.Enabled)
                        Try
                            Dim oWorkDayData As New List(Of roCalendarRowDayData)
                            Dim oPrevCalendar As New roCalendar
                            Dim dPrevStart As Date = DateSerial(2079, 1, 1)
                            Dim oPostCalendar As New roCalendar
                            Dim dPostEnd As Date = DateSerial(2079, 1, 1)
                            Dim bPeriodLimited As Boolean = False

                            Dim dCalendarFirstDay As Date = oCalendar.FirstDay
                            Dim dCalendarLastDay As Date = oCalendar.LastDay

                            If oEmpScheduleRule.ScheduleRule.Parameters.BeginPeriod.Year <> 1970 AndAlso oEmpScheduleRule.ScheduleRule.Parameters.EndPeriod.Year <> 1970 Then
                                ' El periodo no es el del objeto calendario, sino el  indicado
                                bPeriodLimited = True
                                dCalendarFirstDay = DateSerial(dCalendarFirstDay.Year, oEmpScheduleRule.ScheduleRule.Parameters.BeginPeriod.Month, oEmpScheduleRule.ScheduleRule.Parameters.BeginPeriod.Day)
                                dCalendarLastDay = DateSerial(dCalendarLastDay.Year, oEmpScheduleRule.ScheduleRule.Parameters.EndPeriod.Month, oEmpScheduleRule.ScheduleRule.Parameters.EndPeriod.Day)
                                If dCalendarFirstDay > dCalendarLastDay Then
                                    dCalendarLastDay.AddYears(1)
                                End If
                            End If

                            'Cargo calendario del empleado en días previos y posteriores, en función del tipo de regla
                            If oEmpScheduleRule.ScheduleRule.Parameters.IDRule <> ScheduleRuleType.OneShiftOneDay Then
                                Select Case oEmpScheduleRule.ScheduleRule.Parameters.Scope
                                    Case ScheduleRuleScope.Year 'QCOK
                                        ' Inicio de año en curso para el primer día de periodo
                                        If Not bPeriodLimited Then
                                            dPrevStart = dYearFirstDate
                                            ' Fin del año en curso para el último día del periodo
                                            dPostEnd = GetYearFirstDate(dCalendarLastDay, oParameters).AddYears(1).AddDays(-1)
                                        Else
                                            dPrevStart = dCalendarFirstDay
                                            dPostEnd = dCalendarLastDay
                                        End If
                                    Case ScheduleRuleScope.Month 'QCOK
                                        ' Inicio de mes en curso para el primer día de periodo
                                        If dCalendarFirstDay.Day >= iMonthIniDay Then
                                            dPrevStart = DateSerial(dCalendarFirstDay.Year, dCalendarFirstDay.Month, iMonthIniDay)
                                        Else
                                            dPrevStart = DateSerial(dCalendarFirstDay.Year, dCalendarFirstDay.Month - 1, iMonthIniDay)
                                        End If
                                        ' Fin de mes en curso para el último día del periodo
                                        If dCalendarLastDay.Day >= iMonthIniDay Then
                                            dPostEnd = DateSerial(dCalendarLastDay.Year, dCalendarLastDay.Month, iMonthIniDay).AddMonths(1).AddDays(-1)
                                        Else
                                            dPostEnd = DateSerial(dCalendarLastDay.Year, dCalendarLastDay.Month - 1, iMonthIniDay).AddMonths(1).AddDays(-1)
                                        End If
                                    Case ScheduleRuleScope.Week 'QCOK
                                        ' Lunes de la semana en curso para el primer día de periodo
                                        If dCalendarFirstDay.DayOfWeek <> DayOfWeek.Sunday Then
                                            dPrevStart = dCalendarFirstDay.AddDays(-1 * (dCalendarFirstDay.DayOfWeek - 1))
                                        Else
                                            dPrevStart = dCalendarFirstDay.AddDays(-6)
                                        End If
                                        ' Domingo de la semana en curso para el último día de periodo
                                        If dCalendarLastDay.DayOfWeek <> DayOfWeek.Sunday Then
                                            dPostEnd = dCalendarLastDay.AddDays(7 - (dCalendarLastDay.DayOfWeek))
                                        Else
                                            dPostEnd = dCalendarLastDay
                                        End If
                                    Case ScheduleRuleScope.Selection
                                        ' El periodo se definió directamente en la regla
                                        dPrevStart = DateSerial(dCalendarFirstDay.Year, oEmpScheduleRule.ScheduleRule.Parameters.BeginPeriod.Month, oEmpScheduleRule.ScheduleRule.Parameters.BeginPeriod.Day)
                                        dPostEnd = DateSerial(dCalendarLastDay.Year, oEmpScheduleRule.ScheduleRule.Parameters.EndPeriod.Month, oEmpScheduleRule.ScheduleRule.Parameters.EndPeriod.Day)
                                    Case Else
                                        ' Hay reglas que más allá de su scope, requieren un cierto periodo
                                        Select Case oEmpScheduleRule.ScheduleRule.Parameters.IDRule
                                            Case ScheduleRuleType.RestBetweenShifts, ScheduleRuleType.TwoShiftSequence
                                                dPrevStart = dCalendarFirstDay.AddDays(-1)
                                                dPostEnd = dCalendarLastDay.AddDays(1)
                                            Case ScheduleRuleType.OneShiftOneDay
                                                dPrevStart = dCalendarFirstDay.AddDays(-1)
                                                If CType(oEmpScheduleRule.ScheduleRule.Parameters, roScheduleRule_OneShiftOneDay).NextDayShifts.Count > 0 Then
                                                    dPostEnd = dCalendarLastDay.AddDays(1)
                                                End If
                                            Case ScheduleRuleType.MinMaxDaysSequence
                                                Dim oCastRule = CType(oEmpScheduleRule.ScheduleRule.Parameters, roScheduleRule_MinMaxDaysSequence)
                                                If oCastRule.MaximumDays > 1 Then
                                                    dPrevStart = dCalendarFirstDay.AddDays(-1 * oCastRule.MaximumDays - 1)
                                                    dPostEnd = dCalendarLastDay.AddDays(oCastRule.MaximumDays - 1)
                                                End If
                                        End Select
                                        ' Otras, son anuales pero sólo se evalúa el calendario seleccionado. El resto se precalcula
                                        ' Máximo horas anuales
                                        ' Máximo vacaciones
                                        ' Máximo sin planificar
                                End Select
                            End If
                            If dPrevStart <> DateSerial(2079, 1, 1) Then
                                oPrevCalendar = oCalendarManager.Load(dPrevStart, oCalendar.FirstDay.AddDays(-1), "B" & oCalRow.EmployeeData.IDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)
                            End If
                            Dim oPrevCalendarCompact As List(Of roCalendarRow) = Nothing
                            If oPrevCalendar.CalendarData IsNot Nothing AndAlso oPrevCalendar.CalendarData.Count > 1 Then
                                oPrevCalendarCompact = New List(Of roCalendarRow)
                                oPrevCalendarCompact = CompactCalendar(oPrevCalendar)
                            End If

                            If dPostEnd <> DateSerial(2079, 1, 1) Then
                                oPostCalendar = oCalendarManager.Load(oCalendar.LastDay.AddDays(1), dPostEnd, "B" & oCalRow.EmployeeData.IDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)
                            End If
                            Dim oPostCalendarCompact As List(Of roCalendarRow) = Nothing
                            If oPostCalendar.CalendarData IsNot Nothing AndAlso oPostCalendar.CalendarData.Count > 1 Then
                                oPostCalendarCompact = New List(Of roCalendarRow)
                                oPostCalendarCompact = CompactCalendar(oPostCalendar)
                            End If

                            ' Construyo el array de días a evaluar para el empleado
                            If oPrevCalendarCompact IsNot Nothing AndAlso oPrevCalendarCompact.Count > 0 Then
                                oWorkDayData.AddRange(oPrevCalendarCompact(0).PeriodData.DayData)
                            ElseIf oPrevCalendar.CalendarData IsNot Nothing AndAlso oPrevCalendar.CalendarData.Count > 0 Then
                                oWorkDayData.AddRange(oPrevCalendar.CalendarData(0).PeriodData.DayData)
                            End If

                            If Not bPeriodLimited Then
                                oWorkDayData.AddRange(oCalRow.PeriodData.DayData)
                            Else
                                ' Si la regla es para un periodo entre fechas, descarto cualquier fecha fuera de ese periodo
                                Dim tmpLst As New List(Of roCalendarRowDayData)
                                tmpLst = oCalRow.PeriodData.DayData.ToList.FindAll(Function(x) x.PlanDate >= oEmpScheduleRule.ScheduleRule.Parameters.BeginPeriod AndAlso x.PlanDate <= oEmpScheduleRule.ScheduleRule.Parameters.EndPeriod)
                                oWorkDayData.AddRange(tmpLst.ToArray)
                            End If

                            If oPostCalendarCompact IsNot Nothing AndAlso oPostCalendarCompact.Count > 0 Then
                                oWorkDayData.AddRange(oPostCalendarCompact(0).PeriodData.DayData)
                            ElseIf oPostCalendar.CalendarData IsNot Nothing AndAlso oPostCalendar.CalendarData.Count > 0 Then
                                oWorkDayData.AddRange(oPostCalendar.CalendarData(0).PeriodData.DayData)
                            End If

                            If oWorkDayData.Count > 0 Then

                                ' TODO: Los calendarios 'extendidos' debería guardarse en una colección (oworkdaydata, dateini, dateend), porque si por ejemplo hay varias reglas mensuales,
                                ' sólo es estrictamente necesario calcularlo 1 vez.

                                dChekRuleDateStart = oWorkDayData.First.PlanDate
                                dChekRuleDateEnd = oWorkDayData.Last.PlanDate

                                ' Para cada regla recorro todo el periodo
                                oLastShiftSequence = New Queue(Of Robotics.Base.DTOs.roCalendarRowDayData)(2) ' Aquí debería usarse no un dos, sino oEmpScheduleRule.ScheduleRule.Parameters.DayDepth

                                ' Paso toda la información de planificación por si algún manager la necesita
                                oEmpScheduleRule.ScheduleRule.WorkDayData = oWorkDayData
                                For Each oDayData In oWorkDayData.ToArray
                                    oLastShiftSequence.Enqueue(oDayData)
                                    ' Mantengo una cola con los días que precise la regla
                                    If oLastShiftSequence.Count > oEmpScheduleRule.ScheduleRule.Parameters.DayDepth Then oLastShiftSequence.Dequeue()
                                    'If oEmpScheduleRule.ScheduleRule.Applies(oLastShiftSequence.ToList, New roEmployeeRulesData(oCalRow.EmployeeData, oEmployeesContracts.FindAll(Function(o) o.IdEmployee = oCalRow.EmployeeData.IDEmployee AndAlso oDayData.PlanDate >= o.BeginDate AndAlso oDayData.PlanDate <= o.EndDate), oEmployeesAlreadyPlannedHours, oEmployeesPlannedHolidays, oWeekendDefinitions, oShiftNames)) Then
                                    oEmployeesRulesAuxData.EmployeeData = oCalRow.EmployeeData
                                    oEmployeesRulesAuxData.FilterCurrentDate = oDayData.PlanDate
                                    oEmployeesRulesAuxData.FilterCurrentEmployee = oCalRow.EmployeeData.IDEmployee
                                    If oEmpScheduleRule.ScheduleRule.Applies(oLastShiftSequence.ToList, oEmployeesRulesAuxData) Then
                                        oRet.AddRange(oEmpScheduleRule.ScheduleRule.Check(oLastShiftSequence.ToList, oCalRow.EmployeeData, dChekRuleDateStart, dChekRuleDateEnd, dYearFirstDate))
                                    End If
                                Next
                            Else
                                ' No hay planificados, no lo que no hago nada
                            End If
                        Catch ex As Exception
                            Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetScheduleRules: Error checking schedule rule id " & oEmpScheduleRule.ScheduleRule.Parameters.IDRule & " for employee id " & oEmpScheduleRule.IdEmployee.ToString)
                        End Try
                    Next
                Next

                ' Añado ID's (idea de Isidre)
                If oRet.Count > 0 Then
                    For i As Integer = 0 To oRet.Count - 1
                        oRet(i).ID = i
                    Next
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetScheduleRules")
                Return Nothing
            End Try

            Return oRet
        End Function

        Public Function GetEmployeesScheduleRules(dStartDate As Date, dEndDate As Date, strEmployeeIds As String, dPeriodYearFirstDate As Date, Optional bIncludeContractPeriods As Boolean = False, Optional dLabAgreeRulesToCheck As Dictionary(Of Integer, List(Of Integer)) = Nothing, Optional strTypeRulesIdToCheck As String = "", Optional dLabAgreeIdRulesToCheck As Dictionary(Of Integer, List(Of Integer)) = Nothing) As List(Of EmployeeScheduleRule)
            Dim oRet As New List(Of EmployeeScheduleRule)

            Try

                ' Cargo reglas de planificación. Cargo tanto las asignadas a convenio como las asignadas a contratos. Sólo considero contratos con días dentro del periodo cargado.
                Dim strSQL As String = String.Empty
                strSQL = "@SELECT# IdRule, ec.IdEmployee, Definition, sr.IDLabAgree, sr.IDContract "
                If bIncludeContractPeriods Then
                    strSQL = strSQL & ", ec.begindate, ec.enddate "
                End If
                strSQL = strSQL & "from ScheduleRules sr " &
                        "inner join EmployeeContracts ec On ((ec.IDLabAgree = sr.IDLabAgree) Or (ec.IDContract = sr.IDContract)) " &
                        " And IDEmployee in (" & strEmployeeIds & ") And (sr.Enabled = 1 Or sr.IDRule in (9,10))  "
                ' Indico qué reglas hay que validar
                If strTypeRulesIdToCheck.Length > 0 Then
                    ' Limito por ids de regla
                    strSQL = strSQL & " AND sr.IDRule IN (" & strTypeRulesIdToCheck & ") "
                ElseIf dLabAgreeRulesToCheck IsNot Nothing AndAlso dLabAgreeRulesToCheck.Count > 0 Then
                    ' Limito por convenio (para cuando se pueda indicar mediante regla de solicitud en convenio)
                    strSQL = strSQL & " AND (sr.IdLabAgree = 0 OR "
                    Dim strNonLimitedLabAgrees As String = String.Empty
                    strNonLimitedLabAgrees = String.Join(",", dLabAgreeRulesToCheck.Keys.ToArray)
                    strSQL = strSQL & " (sr.IdLabAgree > 0 AND sr.IdLabAgree NOT IN (" & strNonLimitedLabAgrees & ")) OR "
                    For Each oKeyValue As KeyValuePair(Of Integer, List(Of Integer)) In dLabAgreeRulesToCheck
                        strSQL = strSQL & "(sr.IdLabAgree = " & oKeyValue.Key & " AND sr.IDRule IN (" & String.Join(",", oKeyValue.Value) & ")) OR"
                    Next
                    If strSQL.EndsWith("OR") Then strSQL = strSQL.Substring(0, strSQL.Length - 2)
                    strSQL = strSQL & " )"
                ElseIf dLabAgreeIdRulesToCheck IsNot Nothing AndAlso dLabAgreeIdRulesToCheck.Count > 0 Then
                    strSQL = strSQL & " AND (sr.IdLabAgree = 0 OR "
                    Dim strNonLimitedLabAgrees As String = String.Empty
                    strNonLimitedLabAgrees = String.Join(",", dLabAgreeIdRulesToCheck.Keys.ToArray)
                    For Each oKeyValue As KeyValuePair(Of Integer, List(Of Integer)) In dLabAgreeIdRulesToCheck
                        strSQL = strSQL & "(sr.IdLabAgree = " & oKeyValue.Key & " AND sr.ID IN (" & String.Join(",", oKeyValue.Value) & ")) OR"
                    Next
                    If strSQL.EndsWith("OR") Then strSQL = strSQL.Substring(0, strSQL.Length - 2)
                    strSQL = strSQL & " )"
                End If

                strSQL = strSQL & " group by IdRule, ec.IdEmployee, Definition, sr.IDLabAgree, sr.IDContract"
                If bIncludeContractPeriods Then
                    strSQL = strSQL & ", ec.begindate, ec.enddate"
                End If

                Dim dtTable As New DataTable
                dtTable = CreateDataTable(strSQL)
                Dim oEmpRule As EmployeeScheduleRule = Nothing
                For Each oRow As DataRow In dtTable.Rows
                    Dim tmpManager As ScheduleRuleManager = Nothing
                    Select Case oRow("IDRule")
                        Case ScheduleRuleType.OneShiftOneDay
                            tmpManager = New ScheduleRule_OneShiftOneDayManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_OneShiftOneDay)), oState)
                        Case ScheduleRuleType.RestBetweenShifts
                            tmpManager = New ScheduleRule_RestBetweenShiftsManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_RestBetweenShifts)), oState)
                        Case ScheduleRuleType.MinMaxFreeLabourDaysInPeriod
                            tmpManager = New ScheduleRule_MinMaxFreeLabourDaysInPeriodManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxFreeLabourDaysInPeriod)), oState)
                        Case ScheduleRuleType.MinMaxShiftsInPeriod
                            tmpManager = New ScheduleRule_MinMaxShiftsInPeriodManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxShiftsInPeriod)), oState)
                        Case ScheduleRuleType.MinWeekendsInPeriod
                            tmpManager = New ScheduleRule_MinWeekendsInPeriodManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinWeekendsInPeriod)), oState)
                        Case ScheduleRuleType.MinMaxExpectedHours
                            tmpManager = New ScheduleRule_MinMaxExpectedHoursManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxExpectedHours)), oState)
                        Case ScheduleRuleType.TwoShiftSequence
                            tmpManager = New ScheduleRule_2ShiftSequenceManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_2ShiftSequence)), oState)
                        Case ScheduleRuleType.MinMax2ShiftSequenceOnEmployee
                            tmpManager = New ScheduleRule_MinMax2ShiftSequenceOnEmployeeManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMax2ShiftSequenceOnEmployee)), oState)
                        Case ScheduleRuleType.MaxHolidays
                            tmpManager = New ScheduleRule_MaxHolidaysManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MaxHolidays)), oState)
                        Case ScheduleRuleType.WorkOnFestive
                            tmpManager = New ScheduleRule_WorkOnFestiveManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_WorkOnFestive)), oState)
                        Case ScheduleRuleType.WorkOnWeekend
                            tmpManager = New ScheduleRule_WorkOnWeekendManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_WorkOnWeekend)), oState)
                        Case ScheduleRuleType.MaxNotScheduled
                            tmpManager = New ScheduleRule_MaxNotScheduledManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MaxNotScheduled)), oState)
                        Case ScheduleRuleType.MinMaxDaysSequence
                            tmpManager = New ScheduleRule_MinMaxDaysSequenceManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxDaysSequence)), oState)
                        Case ScheduleRuleType.MinMaxExpectedHoursInPeriod
                            tmpManager = New ScheduleRule_MinMaxExpectedHoursInPeriodManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxExpectedHoursInPeriod)), oState)
                        Case ScheduleRuleType.MinMaxShiftsSequence
                            tmpManager = New ScheduleRule_MinMaxShiftsSequenceManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxShiftsSequence)), oState)
                        Case ScheduleRuleType.Custom
                            tmpManager = New ScheduleRule_CustomManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_Custom)), oState)
                    End Select

                    oEmpRule = New EmployeeScheduleRule(oRow("IdEmployee"), tmpManager)

                    Try
                        If bIncludeContractPeriods Then
                            oEmpRule.StartDate = oRow("BeginDate")
                            oEmpRule.EndDate = oRow("EndDate")
                        End If
                    Catch ex As Exception
                    End Try
                    oRet.Add(oEmpRule)
                Next

                ' Compacto reglas para este empleado.
                ' 1.- Reglas de Usuario: Si para una regla dada existe una asignada a contrato, NO CONSIDERO NINGUNA de las asignadas a nivel de convenio
                For Each oRule As EmployeeScheduleRule In oRet.FindAll(Function(o) o.ScheduleRule.Parameters.RuleType = ScheduleRuleBaseType.User AndAlso Not o.ScheduleRule.Parameters.IdContract Is Nothing AndAlso o.ScheduleRule.Parameters.IdContract.Length > 0)
                    oRet.RemoveAll(Function(x) x.ScheduleRule.Parameters.RuleType = ScheduleRuleBaseType.User AndAlso x.ScheduleRule.Parameters.IdLabAgree > 0 AndAlso x.IdEmployee = oRule.IdEmployee)
                Next
                ' 2.- Reglas de Sistema: Si para una regla dada existe una asignada a contrato, NO CONSIDERO ESA MISMA REGLA de las asignadas a nivel de convenio para ese empleado. El resto de reglas de sistema asignadas a nivel de convenio SI las considero.
                For Each oRule As EmployeeScheduleRule In oRet.FindAll(Function(o) o.ScheduleRule.Parameters.RuleType = ScheduleRuleBaseType.System AndAlso Not o.ScheduleRule.Parameters.IdContract Is Nothing AndAlso o.ScheduleRule.Parameters.IdContract.Length > 0)
                    oRet.RemoveAll(Function(x) x.ScheduleRule.Parameters.RuleType = ScheduleRuleBaseType.System AndAlso x.ScheduleRule.Parameters.IDRule = oRule.ScheduleRule.Parameters.IDRule AndAlso x.ScheduleRule.Parameters.IdLabAgree > 0 AndAlso x.IdEmployee = oRule.IdEmployee)
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetEmployeesScheduleRules")
                Return Nothing
            Finally

            End Try
            Return oRet
        End Function

        Public Function GetEmployeeCurrentScheduleRules(idContract As String) As List(Of roScheduleRule)
            Dim oTemp As New List(Of EmployeeScheduleRule)
            Dim oRet As New List(Of roScheduleRule)
            Try
                ' Cargo reglas de planificación de un empleado (y contrato si se indica). Las que le aplican por el contrato indicado y las que hereda del convenio
                Dim strSQL As String = String.Empty
                strSQL = "@SELECT# IdRule, ec.IdEmployee, Definition, sr.IDLabAgree, sr.IDContract " &
                        " from ScheduleRules sr " &
                        " inner join EmployeeContracts ec On (ec.IDLabAgree = sr.IDLabAgree or ec.IDContract = sr.IDContract) " &
                        " where sr.Enabled = 1 and ec.IDContract = '" & idContract & "' " &
                        " group by IdRule, ec.IdEmployee, Definition, sr.IDLabAgree, sr.IDContract"

                Dim dtTable As New DataTable
                dtTable = CreateDataTable(strSQL)
                Dim oEmpRule As EmployeeScheduleRule = Nothing
                For Each oRow As DataRow In dtTable.Rows
                    Dim tmpManager As ScheduleRuleManager = Nothing
                    Select Case oRow("IDRule")
                        Case ScheduleRuleType.OneShiftOneDay
                            tmpManager = New ScheduleRule_OneShiftOneDayManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_OneShiftOneDay)), oState)
                        Case ScheduleRuleType.RestBetweenShifts
                            tmpManager = New ScheduleRule_RestBetweenShiftsManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_RestBetweenShifts)), oState)
                        Case ScheduleRuleType.MinMaxFreeLabourDaysInPeriod
                            tmpManager = New ScheduleRule_MinMaxFreeLabourDaysInPeriodManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxFreeLabourDaysInPeriod)), oState)
                        Case ScheduleRuleType.MinMaxShiftsInPeriod
                            tmpManager = New ScheduleRule_MinMaxShiftsInPeriodManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxShiftsInPeriod)), oState)
                        Case ScheduleRuleType.MinWeekendsInPeriod
                            tmpManager = New ScheduleRule_MinWeekendsInPeriodManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinWeekendsInPeriod)), oState)
                        Case ScheduleRuleType.MinMaxExpectedHours
                            tmpManager = New ScheduleRule_MinMaxExpectedHoursManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxExpectedHours)), oState)
                        Case ScheduleRuleType.TwoShiftSequence
                            tmpManager = New ScheduleRule_2ShiftSequenceManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_2ShiftSequence)), oState)
                        Case ScheduleRuleType.MinMax2ShiftSequenceOnEmployee
                            tmpManager = New ScheduleRule_MinMax2ShiftSequenceOnEmployeeManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMax2ShiftSequenceOnEmployee)), oState)
                        Case ScheduleRuleType.MaxHolidays
                            tmpManager = New ScheduleRule_MaxHolidaysManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MaxHolidays)), oState)
                        Case ScheduleRuleType.WorkOnFestive
                            tmpManager = New ScheduleRule_WorkOnFestiveManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_WorkOnFestive)), oState)
                        Case ScheduleRuleType.WorkOnWeekend
                            tmpManager = New ScheduleRule_WorkOnWeekendManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_WorkOnWeekend)), oState)
                        Case ScheduleRuleType.MaxNotScheduled
                            tmpManager = New ScheduleRule_MaxNotScheduledManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MaxNotScheduled)), oState)
                        Case ScheduleRuleType.MinMaxDaysSequence
                            tmpManager = New ScheduleRule_MinMaxDaysSequenceManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxDaysSequence)), oState)
                        Case ScheduleRuleType.MinMaxExpectedHoursInPeriod
                            tmpManager = New ScheduleRule_MinMaxExpectedHoursInPeriodManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxExpectedHoursInPeriod)), oState)
                        Case ScheduleRuleType.MinMaxShiftsSequence
                            tmpManager = New ScheduleRule_MinMaxShiftsSequenceManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxShiftsSequence)), oState)
                        Case ScheduleRuleType.Custom
                            tmpManager = New ScheduleRule_CustomManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_Custom)), oState)
                    End Select

                    oEmpRule = New EmployeeScheduleRule(oRow("IdEmployee"), tmpManager)
                    oTemp.Add(oEmpRule)
                Next

                ' 1.- Reglas de Usuario: Si para una regla dada existe una asignada a contrato, NO CONSIDERO NINGUNA de las asignadas a nivel de convenio
                For Each oRule As EmployeeScheduleRule In oTemp.FindAll(Function(o) o.ScheduleRule.Parameters.RuleType = ScheduleRuleBaseType.User AndAlso Not o.ScheduleRule.Parameters.IdContract Is Nothing AndAlso o.ScheduleRule.Parameters.IdContract.Length > 0)
                    oTemp.RemoveAll(Function(x) x.ScheduleRule.Parameters.RuleType = ScheduleRuleBaseType.User AndAlso x.ScheduleRule.Parameters.IdLabAgree > 0 AndAlso x.IdEmployee = oRule.IdEmployee)
                Next
                ' 2.- Reglas de Sistema: Si para una regla dada existe una asignada a contrato, NO CONSIDERO ESA MISMA REGLA de las asignadas a nivel de convenio para ese empleado. El resto de reglas de sistema asignadas a nivel de convenio SI las considero.
                For Each oRule As EmployeeScheduleRule In oTemp.FindAll(Function(o) o.ScheduleRule.Parameters.RuleType = ScheduleRuleBaseType.System AndAlso Not o.ScheduleRule.Parameters.IdContract Is Nothing AndAlso o.ScheduleRule.Parameters.IdContract.Length > 0)
                    oTemp.RemoveAll(Function(x) x.ScheduleRule.Parameters.RuleType = ScheduleRuleBaseType.System AndAlso x.ScheduleRule.Parameters.IDRule = oRule.ScheduleRule.Parameters.IDRule AndAlso x.ScheduleRule.Parameters.IdLabAgree > 0 AndAlso x.IdEmployee = oRule.IdEmployee)
                Next

                ' Finalmente construyo la lista a devolver
                For Each oEmpSchRule In oTemp
                    oRet.Add(oEmpSchRule.ScheduleRule.Parameters)
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetEmployeeCurrentScheduleRules")
                Return Nothing
            Finally

            End Try

            Return oRet
        End Function

        Public Shared Function GetWeekendDefinitions(ByRef oState As roCalendarScheduleRulesState) As List(Of roWeekendDefinition)
            Dim oRet As New List(Of roWeekendDefinition)
            Dim oCastRule As New roScheduleRule_WorkOnWeekend
            Dim oWeekendDefinition As roWeekendDefinition
            Try
                ' Cargo reglas de planificación de trabajo en fines de semana, que son las que tienen la definición de fin de semana.
                ' Las cargo independientemente de que estén activas o no, ya que sólo me interesa la definición.

                Dim strSQL As String = String.Empty
                strSQL = "@SELECT# IdRule, Definition, IDLabAgree, IDContract from ScheduleRules where IDRule = " & ScheduleRuleType.WorkOnWeekend

                Dim dtTable As New DataTable
                dtTable = CreateDataTable(strSQL)
                For Each oRow As DataRow In dtTable.Rows
                    Dim tmpManager As ScheduleRuleManager = Nothing
                    Select Case oRow("IDRule")
                        Case ScheduleRuleType.WorkOnWeekend
                            tmpManager = New ScheduleRule_WorkOnWeekendManager(VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_WorkOnWeekend)), oState)
                    End Select
                    oCastRule = CType(tmpManager.Parameters, roScheduleRule_WorkOnWeekend)
                    oWeekendDefinition = New roWeekendDefinition(oCastRule.IdLabAgree, oCastRule.IdContract, oCastRule.LabourDaysIndex, Not oCastRule.Enabled)
                    oRet.Add(oWeekendDefinition)
                Next
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetWeekendDefinition")
                Return Nothing
            Finally

            End Try
            Return oRet
        End Function

        Public Shared Function GetShiftNames(aShiftsIDs As List(Of Integer), ByRef oState As roCalendarScheduleRulesState) As Dictionary(Of Integer, String)
            Dim oRet As New Dictionary(Of Integer, String)
            Try
                Dim strSQL As String = String.Empty
                If aShiftsIDs.Count > 0 Then
                    strSQL = "@SELECT# Id, Name from Shifts where ID in (" & String.Join(",", aShiftsIDs.ToArray) & ")"
                    Dim dtTable As New DataTable
                    dtTable = CreateDataTable(strSQL)
                    For Each oRow As DataRow In dtTable.Rows
                        oRet.Add(oRow("ID"), oRow("Name"))
                    Next
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetShiftNames")
                Return Nothing
            Finally

            End Try
            Return oRet
        End Function

        Public Function GetScheduleRules(Optional iIDLabAgree As Integer = -1, Optional iIDContract As String = "") As List(Of roScheduleRule)
            Dim oRet As New List(Of roScheduleRule)

            Try
                Dim strSQL As String = String.Empty
                If iIDLabAgree <> -1 Then
                    strSQL = "@SELECT# ID, IDRule, IDLabAgree, Enabled, Weight, Definition from ScheduleRules where IDLabAgree = " & iIDLabAgree.ToString
                ElseIf iIDContract <> "" Then
                    strSQL = "@SELECT# ID, IDRule, IDLabAgree, Enabled, Weight, Definition from ScheduleRules where IDContract = '" & iIDContract.ToString & "'"
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roCalendarScheduleRulesManager::GetScheduleRules::Nor IdLabAgree neither IdContract provided !!")
                    Return oRet
                End If

                Dim dtTable As New DataTable
                dtTable = CreateDataTable(strSQL)
                For Each oRow As DataRow In dtTable.Rows
                    Dim tmpRule As roScheduleRule = Nothing
                    Select Case oRow("IDRule")
                        Case ScheduleRuleType.OneShiftOneDay
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_OneShiftOneDay))
                        Case ScheduleRuleType.RestBetweenShifts
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_RestBetweenShifts))
                        Case ScheduleRuleType.MinMaxFreeLabourDaysInPeriod
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxFreeLabourDaysInPeriod))
                        Case ScheduleRuleType.MinMaxShiftsInPeriod
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxShiftsInPeriod))
                        Case ScheduleRuleType.MinWeekendsInPeriod
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinWeekendsInPeriod))
                        Case ScheduleRuleType.MinMaxExpectedHours
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxExpectedHours))
                        Case ScheduleRuleType.TwoShiftSequence
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_2ShiftSequence))
                        Case ScheduleRuleType.MinMax2ShiftSequenceOnEmployee
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMax2ShiftSequenceOnEmployee))
                        Case ScheduleRuleType.MaxHolidays
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MaxHolidays))
                        Case ScheduleRuleType.WorkOnWeekend
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_WorkOnWeekend))
                        Case ScheduleRuleType.WorkOnFestive
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_WorkOnFestive))
                        Case ScheduleRuleType.MaxNotScheduled
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MaxNotScheduled))
                        Case ScheduleRuleType.MinMaxDaysSequence
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxDaysSequence))
                        Case ScheduleRuleType.MinMaxExpectedHoursInPeriod
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxExpectedHoursInPeriod))
                        Case ScheduleRuleType.MinMaxShiftsSequence
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_MinMaxShiftsSequence))
                        Case ScheduleRuleType.Custom
                            tmpRule = VTBase.roJSONHelper.DeserializeNewtonSoft(oRow("Definition"), GetType(roScheduleRule_Custom))
                    End Select
                    tmpRule.Id = oRow("ID")
                    oRet.Add(tmpRule)
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetScheduleRules")
                Return Nothing
            End Try
            Return oRet
        End Function

        Public Function SaveScheduleRules(oScheduleRules() As roScheduleRule, Optional iIDLabAgree As Integer = -1, Optional iIDContract As String = "") As Boolean
            Dim returnValue As Boolean = False
            Dim strSQL As String
            Dim cmd As DbCommand
            Dim da As DbDataAdapter
            Dim dataRow As DataRow
            Dim isNew As Boolean
            Dim scheduleRulesToKeep As New List(Of Integer)
            Dim scheduleRulesUsedInProcess As New Dictionary(Of String, List(Of roScheduleRulesUsedInProcess))
            Dim modifiedScheduleRulesUsedInProcess As New List(Of Integer)
            Dim ruleUsedInProcess As Boolean
            Dim hasActualChanges As Boolean
            Dim requiresRecalculate As Boolean = False


            Dim bHaveToClose As Boolean = False
            Try

                ' Validaciones
                If iIDLabAgree > 0 Then
                    scheduleRulesUsedInProcess = GetScheduleRulesUsedInProcess()
                End If

                ' Crear la lista aplanada de IDs de reglas usadas en procesos
                Dim allUsedScheduleRuleIdsInProcesses As New List(Of Integer)()
                If iIDLabAgree > 0 AndAlso scheduleRulesUsedInProcess IsNot Nothing AndAlso scheduleRulesUsedInProcess.Count > 0 Then
                    For Each kvp As KeyValuePair(Of String, List(Of roScheduleRulesUsedInProcess)) In scheduleRulesUsedInProcess
                        If kvp.Value IsNot Nothing Then
                            For Each processUsage As roScheduleRulesUsedInProcess In kvp.Value
                                If processUsage.ScheduleRulesIds IsNot Nothing Then
                                    allUsedScheduleRuleIdsInProcesses.AddRange(processUsage.ScheduleRulesIds)
                                End If
                            Next
                        End If
                    Next
                    allUsedScheduleRuleIdsInProcesses = allUsedScheduleRuleIdsInProcesses.Distinct().ToList()
                End If

                Dim validationResult As (Boolean, String) = ValidateScheduleRules(oScheduleRules, scheduleRulesUsedInProcess, iIDLabAgree, iIDContract)
                If Not validationResult.Item1 Then
                    Select Case oState.Result
                        Case ScheduleRulesResultEnum.CannotDeleteScheduleRuleInUse
                            Me.oState.Language.ClearUserTokens()
                            Me.oState.Language.AddUserToken(validationResult.Item2)
                            oState.ErrorText = Me.oState.Language.Translate("CalendarScheduleRules.CannotDeleteScheduleRuleInUse", "")
                    End Select

                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                Dim tb As DataTable

                For Each oScheduleRule As roScheduleRule In oScheduleRules
                    ruleUsedInProcess = False ' Reset para cada regla
                    tb = New DataTable("ScheduleRules")
                    strSQL = "@SELECT# * FROM ScheduleRules WHERE Id = " & oScheduleRule.Id.ToString
                    cmd = CreateCommand(strSQL)
                    da = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    If (tb.Rows.Count.Equals(0)) Then
                        isNew = True
                        dataRow = tb.NewRow
                        ' Asignar nuevo ID para la regla
                        Dim currentIdentVal As Object = ExecuteScalar("@SELECT# IDENT_CURRENT ('ScheduleRules')")
                        If currentIdentVal IsNot DBNull.Value AndAlso currentIdentVal IsNot Nothing Then
                            oScheduleRule.Id = Convert.ToInt32(currentIdentVal) + 1
                        Else
                            ' Fallback si IDENT_CURRENT es NULL (ej. tabla vacía)
                            Dim maxIdVal As Object = ExecuteScalar("@SELECT# ISNULL(MAX(Id), 0) FROM ScheduleRules")
                            oScheduleRule.Id = Convert.ToInt32(maxIdVal) + 1
                        End If
                        dataRow("Id") = oScheduleRule.Id ' Establecer el ID en el DataRow para la nueva fila
                    Else
                        isNew = False
                        dataRow = tb.Rows(0)
                    End If

                    scheduleRulesToKeep.Add(oScheduleRule.Id)

                    hasActualChanges = False

                    ' Comparar valores antes de asignar para evitar cambiar RowState innecesariamente
                    Dim newDefinition As String = VTBase.roJSONHelper.SerializeNewtonSoft(oScheduleRule)
                    Dim newIDRule As Integer = CInt(oScheduleRule.IDRule)
                    Dim newIDLabAgree As Integer = oScheduleRule.IdLabAgree
                    Dim newIDContract As String = If(oScheduleRule.IdContract Is Nothing, "", oScheduleRule.IdContract)
                    Dim newEnabled As Boolean = oScheduleRule.Enabled
                    Dim newWeight As Integer = oScheduleRule.Weight

                    If isNew Then
                        hasActualChanges = True
                    Else
                        ' Comparar cada campo. Usar Object.Equals para manejar DBNull correctamente.
                        If dataRow("Definition") <> newDefinition Then
                            requiresRecalculate = True
                            hasActualChanges = True
                        End If
                        If dataRow("IDRule") <> newIDRule Then hasActualChanges = True
                        If dataRow("IDLabAgree") <> newIDLabAgree Then hasActualChanges = True
                        If dataRow("IDContract") <> newIDContract Then hasActualChanges = True
                        If dataRow("Enabled") <> newEnabled Then hasActualChanges = True
                        If dataRow("Weight") <> newWeight Then hasActualChanges = True
                    End If

                    If hasActualChanges Then
                        ' Vemos si se usa en algún proceso
                        If Not isNew Then
                            Dim currentRuleDbId As Integer = roTypes.Any2Integer(dataRow("ID"))
                            If iIDLabAgree > 0 Then
                                ' Usamos la lista precalculada si estamos en contexto de convenio
                                ruleUsedInProcess = allUsedScheduleRuleIdsInProcesses.Contains(currentRuleDbId)
                            Else
                                ' Para contratos, o si la lista precalculada no está disponible (iIDLabAgree <= 0),
                                ' llamamos a la función original ya que UsedInProcess consulta globalmente.
                                ruleUsedInProcess = UsedInProcess(currentRuleDbId)
                            End If

                            If ruleUsedInProcess AndAlso requiresRecalculate Then
                                modifiedScheduleRulesUsedInProcess.Add(currentRuleDbId)
                            End If
                        End If

                        ' Asignar valores solo si hay cambios reales o es una fila nueva
                        dataRow("Definition") = newDefinition
                        dataRow("IDRule") = newIDRule
                        dataRow("IDLabAgree") = newIDLabAgree
                        dataRow("IDContract") = newIDContract
                        dataRow("Enabled") = newEnabled
                        dataRow("Weight") = newWeight

                        If isNew Then
                            tb.Rows.Add(dataRow)
                        End If

                        da.Update(tb)
                    Else
                        ' No hay cambios reales en esta fila existente.
                        ' Si dataRow.RowState era Unchanged, permanecerá así, y Update no la procesará.
                    End If
                Next

                ' Borramos las reglas que se hayan elimiando en pantalla
                Dim deleteSQL As String = String.Empty
                If iIDLabAgree > 0 Then
                    If scheduleRulesToKeep.Count > 0 Then
                        deleteSQL = "@DELETE# ScheduleRules where IDLabAgree = " & iIDLabAgree.ToString & " and id not in (" & String.Join(",", scheduleRulesToKeep.ToArray) & ")"
                    Else
                        deleteSQL = "@DELETE# ScheduleRules where IDLabAgree = " & iIDLabAgree.ToString
                    End If
                ElseIf Not String.IsNullOrEmpty(iIDContract) Then ' Asegurarse que iIDContract tiene valor
                    If scheduleRulesToKeep.Count > 0 Then
                        deleteSQL = "@DELETE# ScheduleRules where IDContract = '" & iIDContract.ToString & "' and id not in (" & String.Join(",", scheduleRulesToKeep.ToArray) & ")"
                    Else
                        deleteSQL = "@DELETE# ScheduleRules where IDContract = '" & iIDContract.ToString & "'"
                    End If
                End If

                If Not String.IsNullOrEmpty(deleteSQL) Then
                    ExecuteSql(deleteSQL)
                End If

                returnValue = True

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::SaveScheduleRules")
                returnValue = False ' Asegurar que returnValue es False en caso de excepción
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, returnValue)
            End Try


            'Si es necesario notifico actualización de cache de convenio
            If modifiedScheduleRulesUsedInProcess.Any AndAlso iIDLabAgree > -1 Then
                Recalculate(iIDLabAgree, modifiedScheduleRulesUsedInProcess, scheduleRulesUsedInProcess)
            End If

            Return returnValue

        End Function


        Private Function ValidateScheduleRules(newScheduleRules As roScheduleRule(), scheduleRulesUsedInProcess As Dictionary(Of String, List(Of roScheduleRulesUsedInProcess)), labagreeId As Integer, contractId As String) As (Boolean, String)
            Dim returnValue As Boolean = True
            Dim errorText As String = String.Empty

            Try
                ' Reglas asociadas a Convenio
                If labagreeId > 0 Then
                    ' Verificamos que no se vaya a borrar alguna regla que esté en uso en reglas diarias
                    ' Antes validamos que no vamos a borrar una regla que esté en uso

                    ' 0 - Reglas que vamos a mantener
                    Dim rulesToKeep As New List(Of Integer)
                    rulesToKeep = newScheduleRules.Select(Function(x) x.Id).ToList

                    ' 1 - Reglas que vamos a mantener, pero estan deshabilitadas
                    Dim rulesToKeepDisabled As New List(Of Integer)
                    rulesToKeepDisabled = newScheduleRules.ToList.FindAll(Function(x) Not x.Enabled).Select(Function(x) x.Id).ToList

                    ' 2 - Reglas que se van borrar
                    Dim sqlCommand As String
                    If rulesToKeep.Count > 0 Then
                        sqlCommand = $"@SELECT# Id FROM ScheduleRules WHERE IDLabAgree = {labagreeId} AND Id NOT IN ({String.Join(",", rulesToKeep.ToArray)})"
                    Else
                        sqlCommand = $"@SELECT# Id FROM ScheduleRules WHERE IDLabAgree = {labagreeId}"
                    End If
                    Dim rulesToDeleteTable As DataTable = CreateDataTable(sqlCommand)
                    Dim rulesToBeDeleted As List(Of Integer) = rulesToDeleteTable.DefaultView.Cast(Of DataRowView)().Select(Function(row) roTypes.Any2Integer(row("Id"))).ToList

                    ' 3 - Verificamos si alguna de las reglas que se van a borrar está en uso en algún proceso. En ese caso, no dejamos guardar
                    Dim ruleInUseFound As Boolean = False
                    Dim usedRuleId As Integer = 0
                    If rulesToBeDeleted.Count > 0 Then
                        For Each kvp As KeyValuePair(Of String, List(Of roScheduleRulesUsedInProcess)) In scheduleRulesUsedInProcess
                            For Each processUsage As roScheduleRulesUsedInProcess In kvp.Value
                                If processUsage.ScheduleRulesIds IsNot Nothing AndAlso processUsage.ScheduleRulesIds.Intersect(rulesToBeDeleted).Any() Then
                                    ruleInUseFound = True
                                    usedRuleId = processUsage.ScheduleRulesIds.Intersect(rulesToBeDeleted).First()
                                    Exit For
                                End If
                            Next
                            If ruleInUseFound Then Exit For
                        Next
                    End If

                    If ruleInUseFound Then
                        Me.oState.Language.ClearUserTokens()
                        Dim ruleWithError As roScheduleRule = GetScheduleRules(labagreeId).FirstOrDefault(Function(x) x.Id = usedRuleId)
                        If ruleWithError IsNot Nothing Then
                            errorText = ruleWithError.RuleName
                        Else
                            errorText = "ID: " & usedRuleId.ToString()
                        End If

                        Me.oState.Result = ScheduleRulesResultEnum.CannotDeleteScheduleRuleInUse
                        returnValue = False
                    Else
                        ' No se eliminan reglas de planificación usadas. Aseguramos que ninguna de las que están en uso se están desactivando
                        If scheduleRulesUsedInProcess.ContainsKey("DAILYCAUSESRULES") Then
                            Dim dailyCausesRuleList As List(Of roScheduleRulesUsedInProcess) = scheduleRulesUsedInProcess("DAILYCAUSESRULES")
                            For Each processUsage As roScheduleRulesUsedInProcess In dailyCausesRuleList
                                If processUsage.ScheduleRulesIds IsNot Nothing AndAlso processUsage.ScheduleRulesIds.Any Then
                                    Dim disabledRulesUsedInProcess As New List(Of Integer)
                                    disabledRulesUsedInProcess = newScheduleRules.ToList.FindAll(Function(x) processUsage.ScheduleRulesIds.Contains(x.Id)).FindAll(Function(y) Not y.Enabled).Select(Function(z) z.Id).ToList
                                    If disabledRulesUsedInProcess.Any Then
                                        Me.oState.Language.ClearUserTokens()
                                        Dim ruleWithError As roScheduleRule = GetScheduleRules(labagreeId).FirstOrDefault(Function(x) x.Id = usedRuleId)
                                        If ruleWithError IsNot Nothing Then
                                            errorText = ruleWithError.RuleName
                                        Else
                                            errorText = "ID: " & usedRuleId.ToString()
                                        End If

                                        Me.oState.Result = ScheduleRulesResultEnum.CannotDisableScheduleRuleInUse
                                        returnValue = False
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    End If
                ElseIf contractId <> String.Empty Then
                    ' No hay validaciones para reglas de planificación de contrato
                    returnValue = True
                End If



            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::ValidateScheduleRules")
            End Try

            Return (returnValue, errorText)
        End Function

        Private Function Recalculate(labAgreeId As Integer, modifiedScheduleRulesUsedInProcess As List(Of Integer), scheduleRulesUsedInProcess As Dictionary(Of String, List(Of roScheduleRulesUsedInProcess))) As Boolean
            Dim returnValue As Boolean = False
            Try
                '0 - Si hemos tocado alguna regla usada en procesos, mandamos tarea para actualización de caché
                Dim oParamsAux As New roCollection
                oParamsAux.Add("ObjectID", labAgreeId)
                oParamsAux.Add("Action", CacheAction.InsertOrUpdate.ToString)
                roConnector.InitTask(TasksType.LABAGREES, oParamsAux)

                '1 - Buscar horarios que tengan asignada esta regla de planificación
                '    Se puede afinar marcando sólo los días de empleados que tengn ese día el convenio de la regla de planificación en cuestión.
                Dim shiftsToRecalculate As New List(Of Integer)
                For Each scheduleRules As roScheduleRulesUsedInProcess In scheduleRulesUsedInProcess("DAILYCAUSESRULES")
                    If scheduleRules.ScheduleRulesIds IsNot Nothing AndAlso scheduleRules.ScheduleRulesIds.Any Then
                        If scheduleRules.ScheduleRulesIds.Intersect(modifiedScheduleRulesUsedInProcess).Any Then
                            ' Si hay alguna regla modificada, añadimos los ID de turnos afectados
                            shiftsToRecalculate.Add(scheduleRules.ObjectId)
                        End If
                    End If
                Next

                '2 - Marcar todos los días (y el día anterior y el posterior) que tengan asigndos esos horarios entre la fecha de bloqueo y la fecha actual
                '    La fecha de bloqueo depende de cada empleado
                '    Status = 50 siempre que status = 50 o superior
                If shiftsToRecalculate.Any Then
                    Dim commandSql As String = String.Empty
                    Dim shiftsIds As String = String.Join(",", shiftsToRecalculate)
                    commandSql = $"@UPDATE# DailySchedule
                                       SET DailySchedule.Status = 65
                                   FROM DailySchedule 
                                   INNER JOIN sysrovwEmployeeLockDate ON sysrovwEmployeeLockDate.IDEmployee= DailySchedule.IDEmployee
                                   WHERE (IDShift1 IN ({shiftsIds}) OR IDShiftBase IN ({shiftsIds}))
                                        AND DailySchedule.Date > sysrovwEmployeeLockDate.LockDate 
                                        AND DailySchedule.Date <= CAST(GETDATE() AS Date)
	                                    AND DailySchedule.Status > 65"
                    ExecuteSql(commandSql)

                    Extensions.roConnector.InitTask(TasksType.DAILYCAUSES)

                End If

                '3 - Lanzar la tarea de recálculo

                returnValue = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::NotifyScheduleRuleChange")
            End Try

            Return returnValue

        End Function

        Public Shared Function GetEmployeesAlreadyPlannedHoursTable(dStartDate As Date, dEndDate As Date, dBeginYearDate As Date, strEmployeeIds As String, ByRef oState As roCalendarScheduleRulesState) As DataTable
            Dim oRet As New DataTable
            Try

                Dim strSQL As String = String.Empty

                strSQL = "@SELECT# ds.IDEmployee, ec.IDContract, isnull(ec.IDLabAgree,0) as IDLabAgree, SUM(isnull(ds.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)) Value  " &
                "From DailySchedule ds " &
                "inner join Shifts on Shifts.ID = ds.IDShift1 " &
                "inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate " &
                "where ds.IDEmployee in (" & strEmployeeIds & ") " &
                "and Date between " & roTypes.Any2Time(dBeginYearDate).SQLSmallDateTime & " And " & roTypes.Any2Time(dBeginYearDate.AddYears(1).AddDays(-1)).SQLSmallDateTime & " " &
                "And Date Not between " & roTypes.Any2Time(dStartDate).SQLSmallDateTime & " And " & roTypes.Any2Time(dEndDate).SQLSmallDateTime & " " &
                "group by ds.IDEmployee, ec.IDContract, ec.IDLabAgree"

                oRet = CreateDataTable(strSQL)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetEmployeesAlreadyPlannedHoursTable")
            End Try
            Return oRet
        End Function

        Public Shared Function GetEmployeesNotScheduledDays(dStartDate As Date, dEndDate As Date, dBeginYearDate As Date, strEmployeeIds As String, ByRef oState As roCalendarScheduleRulesState) As DataTable
            Dim oRet As New DataTable
            Try

                Dim strSQL As String = String.Empty

                strSQL = "WITH alldays AS (  " &
                        "        @SELECT# " & roTypes.Any2Time(dBeginYearDate).SQLSmallDateTime & " AS date  " &
                        "        UNION ALL  " &
                        "   @SELECT# DATEADD(dd, 1, date)  " &
                        "        FROM alldays s  " &
                        "        WHERE DATEADD(dd, 1, date) <= " & roTypes.Any2Time(dBeginYearDate.AddYears(1).AddDays(-1)).SQLSmallDateTime & ")  " &
                        "@SELECT# emp.ID idemployee, ec.IDContract, isnull(ec.IDLabAgree,0) as IDLabAgree, count(*) value from alldays " &
                        "inner join Employees emp on emp.id in (" & strEmployeeIds & ") " &
                        "inner join EmployeeContracts ec on ec.IDEmployee = emp.ID and alldays.Date between ec.BeginDate and ec.EndDate  " &
                        "left outer join DailySchedule ds on ds.date = alldays.date and ds.IDEmployee = emp.id " &
                        "where idshift1 is null " &
                        "And alldays.date Not between " & roTypes.Any2Time(dStartDate).SQLSmallDateTime & " And " & roTypes.Any2Time(dEndDate).SQLSmallDateTime & " " &
                        "group by emp.id,ec.IDContract, ec.IDLabAgree " &
                        "option (maxrecursion 370) "

                oRet = CreateDataTable(strSQL)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetEmployeesAlreadyPlannedHoursTable")
            End Try
            Return oRet
        End Function

        Public Shared Function GetEmployeesPlannedHolidaysTable(dStartDate As Date, dEndDate As Date, strEmployeeIds As String, ByRef oState As roCalendarScheduleRulesState) As DataTable
            Dim oRet As New DataTable
            Try

                ' Calculo día de inicio de año
                Dim oParameters As New roParameters("OPTIONS")
                Dim intMonthIniDay As Integer
                Dim intYearIniMonth As Integer
                intMonthIniDay = oParameters.Parameter(Parameters.MonthPeriod)
                intYearIniMonth = oParameters.Parameter(Parameters.YearPeriod)
                Dim xBeginPeriod As DateTime
                If dStartDate.Month > intYearIniMonth Then
                    xBeginPeriod = New DateTime(dStartDate.Year, intYearIniMonth, intMonthIniDay)
                ElseIf dStartDate.Month = intYearIniMonth And dStartDate.Day >= intMonthIniDay Then
                    xBeginPeriod = New DateTime(dStartDate.Year, intYearIniMonth, intMonthIniDay)
                Else
                    xBeginPeriod = New DateTime(dStartDate.Year - 1, intYearIniMonth, intMonthIniDay)
                End If

                Dim strSQL As String = String.Empty

                strSQL = "@SELECT# ds.IDEmployee, ec.IDContract, isnull(ec.IDLabAgree,0) as IDLabAgree, COUNT(*) Value  " &
                "From DailySchedule ds " &
                "inner join Shifts on Shifts.ID = ds.IDShift1 and ds.IsHolidays = 1 " &
                "inner join EmployeeContracts ec on ec.IDEmployee = ds.IDEmployee and ds.Date between ec.BeginDate and ec.EndDate " &
                "where ds.IDEmployee in (" & strEmployeeIds & ") " &
                "and Date between " & roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime & " And " & roTypes.Any2Time(xBeginPeriod.AddYears(1).AddDays(-1)).SQLSmallDateTime & " " &
                "And Date Not between " & roTypes.Any2Time(dStartDate).SQLSmallDateTime & " And " & roTypes.Any2Time(dEndDate).SQLSmallDateTime & " " &
                "group by ds.IDEmployee, ec.IDContract, ec.IDLabAgree"

                oRet = CreateDataTable(strSQL)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetEmployeesPlannedHolidaysTable")
            End Try
            Return oRet
        End Function

        Public Shared Function GetEmployeesContractsTable(dStartDate As Date, dEndDate As Date, dFirstYearDay As Date, strEmployeeIds As String, ByRef oState As roCalendarScheduleRulesState) As DataTable
            Dim oRet As New DataTable
            Try
                Dim strSQL As String = String.Empty
                strSQL = "@SELECT# IdEmployee, IDContract, BeginDate, EndDate,  IsNULL(IDLabAgree,0) IDLabAgree from EmployeeContracts ec " &
                        "left join LabAgree la On la.ID = ec.IDLabAgree " &
                        "where (" & roTypes.Any2Time(dFirstYearDay).SQLSmallDateTime & " between ec.Begindate and ec.EndDate " &
                        "  	Or ec.BeginDate >= " & roTypes.Any2Time(dFirstYearDay).SQLSmallDateTime &
                        "	) " &
                        " And IDEmployee In (" & strEmployeeIds & ")"

                'strSQL = "@SELECT# IdEmployee, IDContract, BeginDate, EndDate, IDLabAgree from EmployeeContracts ec " &
                '        "left join LabAgree la On la.ID = ec.IDLabAgree " &
                '        "where (" &
                '           "(ec.BeginDate between " & roTypes.Any2Time(dFirstYearDay).SQLSmallDateTime & " and " & roTypes.Any2Time(dFirstYearDay.AddYears(1).AddDays(-1)).SQLSmallDateTime & ") " &
                '        "Or (ec.EndDate between " & roTypes.Any2Time(dFirstYearDay).SQLSmallDateTime & " and " & roTypes.Any2Time(dFirstYearDay.AddYears(1).AddDays(-1)).SQLSmallDateTime & ") " &
                '        "Or (ec.BeginDate < " & roTypes.Any2Time(dFirstYearDay).SQLSmallDateTime & " and ec.EndDate > " & roTypes.Any2Time(dFirstYearDay.AddYears(1).AddDays(-1)).SQLSmallDateTime & ") " &
                '        "	) " &
                '        " And IDEmployee In (" & strEmployeeIds & ")"
                oRet = CreateDataTable(strSQL)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetEmployeesContractsTable")
                Return Nothing
            Finally

            End Try
            Return oRet
        End Function

        Private Function GetCalendarEmployeesIds(oCalendar As roCalendar) As String
            Try
                Dim lEmployeeIds As New List(Of Integer)
                For Each oCalRow As roCalendarRow In oCalendar.CalendarData
                    lEmployeeIds.Add(oCalRow.EmployeeData.IDEmployee)
                Next
                Return String.Join(",", lEmployeeIds.ToArray)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetCalendarEmployeesIds")
                Return String.Empty
            End Try
        End Function

        ''' <summary>
        ''' Si un calendario tiene más de una línea por empleado (cosa que ocurre si el empleado ha estado en más de un grupo en el periodo), compacta las líneas en una.
        ''' </summary>
        ''' <param name="oCalendar"></param>
        ''' <returns></returns>
        Public Function CompactCalendar(oCalendar As roCalendar) As List(Of roCalendarRow)
            Dim oRet As New List(Of roCalendarRow)
            Try
                Dim oNewCalendarData As roCalendarRow
                Dim lProcessedEmployeesIds As New List(Of Integer)
                For Each oCalRow As roCalendarRow In oCalendar.CalendarData
                    If oCalendar.CalendarData.ToList.FindAll(Function(x) x.EmployeeData.IDEmployee = oCalRow.EmployeeData.IDEmployee).Count >= 1 Then
                        If Not lProcessedEmployeesIds.Contains(oCalRow.EmployeeData.IDEmployee) Then
                            oNewCalendarData = New roCalendarRow
                            oNewCalendarData.EmployeeData = oCalRow.EmployeeData
                            oNewCalendarData.PeriodData = New roCalendarRowPeriodData
                            oNewCalendarData.PeriodData.DayData = {}
                            ReDim oNewCalendarData.PeriodData.DayData(oCalRow.PeriodData.DayData.Count - 1)
                            For j = 0 To oCalRow.PeriodData.DayData.Count - 1
                                For Each oCalDetail As roCalendarRow In oCalendar.CalendarData.ToList.FindAll(Function(x) x.EmployeeData.IDEmployee = oCalRow.EmployeeData.IDEmployee)
                                    If oNewCalendarData.PeriodData.DayData(j) Is Nothing Then
                                        ' Valor por defecto. En un objeto Calendar no hay días a Nothing.
                                        oNewCalendarData.PeriodData.DayData(j) = New roCalendarRowDayData
                                        oNewCalendarData.PeriodData.DayData(j).PlanDate = oCalDetail.PeriodData.DayData(j).PlanDate
                                    End If
                                    If oCalDetail.PeriodData.DayData(j).EmployeeStatusOnDay <> EmployeeStatusOnDayEnum.InOtherDepartment Then
                                        oNewCalendarData.PeriodData.DayData(j) = oCalDetail.PeriodData.DayData(j)
                                        Exit For
                                    End If
                                Next
                            Next
                            oRet.Add(oNewCalendarData)
                        End If
                    Else
                        oRet.Add(oCalRow)
                    End If
                    lProcessedEmployeesIds.Add(oCalRow.EmployeeData.IDEmployee)
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::CompactCalendar")
                Return Nothing
            End Try

            Return oRet
        End Function

        Public Function GetYearFirstDate(dRefDate As Date, oParameters As roParameters, Optional ByRef iMonthIniDay As Integer = 1, Optional ByRef iYearIniMonth As Integer = 1) As Date
            ' Calculo días de inicio de año y día de inicio de mes
            Dim dBeginYearDate As DateTime
            Try
                iMonthIniDay = oParameters.Parameter(Parameters.MonthPeriod)
                iYearIniMonth = oParameters.Parameter(Parameters.YearPeriod)

                If dRefDate.Month > iYearIniMonth Then
                    dBeginYearDate = New DateTime(dRefDate.Year, iYearIniMonth, iMonthIniDay)
                ElseIf dRefDate.Month = iYearIniMonth And dRefDate.Day >= iMonthIniDay Then
                    dBeginYearDate = New DateTime(dRefDate.Year, iYearIniMonth, iMonthIniDay)
                Else
                    dBeginYearDate = New DateTime(dRefDate.Year - 1, iYearIniMonth, iMonthIniDay)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetYearFirstDate")
                Return Nothing
            End Try
            Return dBeginYearDate
        End Function

        Public Function UsedInProcess(scheduleRuleId As Integer) As Boolean
            Dim returnValue As Boolean = False

            Try
                ' Compruebo si la regla de planificación está en uso en algún proceso
                Dim rulesUsedInProcess As Dictionary(Of String, List(Of roScheduleRulesUsedInProcess)) = GetScheduleRulesUsedInProcess()
                If rulesUsedInProcess IsNot Nothing AndAlso rulesUsedInProcess.ContainsKey("DAILYCAUSESRULES") Then
                    Dim shiftRuleProcesses As List(Of roScheduleRulesUsedInProcess) = rulesUsedInProcess("DAILYCAUSESRULES")
                    If shiftRuleProcesses IsNot Nothing Then
                        ' Comprueba si algún proceso en la lista de "DAILYCAUSESRULES" contiene el scheduleRuleId
                        returnValue = shiftRuleProcesses.Any(Function(process) process.ScheduleRulesIds IsNot Nothing AndAlso process.ScheduleRulesIds.Contains(scheduleRuleId))
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::UsedInProcess")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::UsedInProcess")
            End Try

            Return returnValue

        End Function

        Public Function GetScheduleRulesUsedInProcess() As Dictionary(Of String, List(Of roScheduleRulesUsedInProcess))
            Dim returnValue As New Dictionary(Of String, List(Of roScheduleRulesUsedInProcess))

            Try

                ' Reglas de tipo "Descanso entre jornadas" en reglas diarias
                Dim sql As String = $"@SELECT# Value, IdShift FROM (
                                     @SELECT#
                                        IdShift,
		                                CAST(Definition AS XML).value('(/roCollection/Item[@key=""ScheduleRulesValidationRule""]/@type)[1]', 'varchar(10)') AS Type,
		                                CAST(Definition AS XML).value('(/roCollection/Item[@key=""ScheduleRulesValidationRule""])[1]', 'varchar(50)') AS Value
	                                FROM sysroShiftsCausesRules
	                                WHERE RuleType = 3
	                                ) AUX 
                                WHERE AUX.Value IS NOT NULL"

                Dim resultTable As DataTable = CreateDataTable(sql)
                If resultTable IsNot Nothing AndAlso resultTable.Rows.Count > 0 Then
                    For Each dataRow As DataRow In resultTable.Rows
                        Dim scheduleRulesUsedInShifts As New List(Of roScheduleRulesUsedInProcess)
                        Dim currentShiftRuleIds As String = roTypes.Any2String(dataRow("Value"))
                        scheduleRulesUsedInShifts.Add(New roScheduleRulesUsedInProcess With {
                            .ObjectId = roTypes.Any2Integer(dataRow("IdShift")),
                            .ScheduleRulesIds = If(Not String.IsNullOrEmpty(currentShiftRuleIds),
                                                    currentShiftRuleIds.Split(","c).Select(Function(idStr) Integer.Parse(idStr.Trim())).ToList(),
                                                    New List(Of Integer)())
                        })
                        If Not returnValue.ContainsKey("DAILYCAUSESRULES") Then
                            returnValue.Add("DAILYCAUSESRULES", scheduleRulesUsedInShifts)
                        Else
                            returnValue("DAILYCAUSESRULES").AddRange(scheduleRulesUsedInShifts)
                        End If
                    Next
                End If

                ' Reglas de solicitud
                sql = $"@SELECT# Value, IDRule FROM (
                                     @SELECT#
                                        IDRule,
		                                CAST(Definition AS XML).value('(/roCollection/Item[@key=""IDPlanificationRules""]/@type)[1]', 'varchar(10)') AS Type,
		                                CAST(Definition AS XML).value('(/roCollection/Item[@key=""IDPlanificationRules""])[1]', 'varchar(50)') AS Value
	                                FROM RequestsRules
	                                ) AUX 
                                WHERE AUX.Value IS NOT NULL"

                resultTable = CreateDataTable(sql)
                If resultTable IsNot Nothing AndAlso resultTable.Rows.Count > 0 Then
                    For Each dataRow As DataRow In resultTable.Rows
                        Dim scheduleRulesUsedInRequestRules As New List(Of roScheduleRulesUsedInProcess)
                        Dim currentRequestRuleIds As String = roTypes.Any2String(dataRow("Value"))
                        scheduleRulesUsedInRequestRules.Add(New roScheduleRulesUsedInProcess With {
                            .ObjectId = roTypes.Any2Integer(dataRow("IDRule")),
                            .ScheduleRulesIds = If(Not String.IsNullOrEmpty(currentRequestRuleIds),
                                                    currentRequestRuleIds.Split(","c).Select(Function(idStr) Integer.Parse(idStr.Trim())).ToList(),
                                                    New List(Of Integer)())
                        })
                        If Not returnValue.ContainsKey("REQUESTRULES") Then
                            returnValue.Add("REQUESTRULES", scheduleRulesUsedInRequestRules)
                        Else
                            returnValue("REQUESTRULES").AddRange(scheduleRulesUsedInRequestRules)
                        End If
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetScheduleRulesUsedInProcess")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarScheduleRulesManager::GetScheduleRulesUsedInProcess")
            End Try

            Return returnValue
        End Function

#End Region

    End Class

    Public Class roScheduleRulesUsedInProcess
        Public ObjectId As Integer
        Public ScheduleRulesIds As List(Of Integer)
    End Class

    Public Class roMiniContract
        Public BeginDate As Date
        Public EndDate As Date
        Public IdLabAgree As Integer
        Public IdContract As String
        Public IdEmployee As Integer

        Public Sub New(dBeginDate As Date, dEndDate As Date, iIdLabAgree As Integer, iIdContract As String, iIdEmployee As Integer)
            BeginDate = dBeginDate
            EndDate = dEndDate
            IdLabAgree = iIdLabAgree
            IdEmployee = iIdEmployee
            IdContract = iIdContract
        End Sub

    End Class

    Public Class roEmployeeCounter
        Public Value As Long
        Public IdLabAgree As Integer
        Public IdContract As String
        Public IdEmployee As Integer

        Public Sub New(iIdLabAgree As Integer, iIdContract As String, iIdEmployee As Integer, lValue As Long)
            Value = lValue
            IdLabAgree = iIdLabAgree
            IdEmployee = iIdEmployee
            IdContract = iIdContract
        End Sub

    End Class

    Public Class roWeekendDefinition
        Public WeekEndDays As New List(Of Integer)
        Public IdLabAgree As Integer
        Public IdContract As String
        Public CanWorkOn As Boolean

        Public Sub New(iIdLabAgree As Integer, iIdContract As String, sLabourDaysIndex As String, Optional bCanWork As Boolean = False)
            IdLabAgree = iIdLabAgree
            IdContract = iIdContract
            ' De lunes a sábado
            For i = 1 To 6
                If Not sLabourDaysIndex.Split(",").Contains(i) Then WeekEndDays.Add(i)
            Next
            ' El domingo es i=0. Lo reviso el último para que salga ordenado
            If Not sLabourDaysIndex.Split(",").Contains(0) Then WeekEndDays.Add(0)
            CanWorkOn = bCanWork
        End Sub

    End Class

    Public Class roEmployeeRulesData
        Public Contracts As List(Of roMiniContract)
        Public EmployeeData As roCalendarRowEmployeeData
        Public AlreadyPlannedHours As List(Of roEmployeeCounter)
        Public PlannedHolidays As List(Of roEmployeeCounter)
        Public WeekendDefinitions As List(Of roWeekendDefinition)
        Public ShiftsNames As Dictionary(Of Integer, String)
        Public NotScheduledDays As List(Of roEmployeeCounter)
        Public FilterCurrentEmployee As Integer
        Public FilterCurrentDate As Date

        Public ReadOnly Property ContractsFiltered() As List(Of roMiniContract)
            Get
                Return Contracts.FindAll(Function(o) o.IdEmployee = FilterCurrentEmployee AndAlso FilterCurrentDate >= o.BeginDate AndAlso FilterCurrentDate <= o.EndDate)
            End Get
        End Property

        Public Sub New(oEmployeesScheduleRules As List(Of EmployeeScheduleRule), dStartDate As Date, dEndDate As Date, dFirstYearDay As Date, strEmployeeIds As String, ByRef oState As roCalendarScheduleRulesState)
            Contracts = New List(Of roMiniContract)
            AlreadyPlannedHours = New List(Of roEmployeeCounter)
            PlannedHolidays = New List(Of roEmployeeCounter)
            NotScheduledDays = New List(Of roEmployeeCounter)
            WeekendDefinitions = New List(Of roWeekendDefinition)
            ShiftsNames = New Dictionary(Of Integer, String)
            Load(oEmployeesScheduleRules, dStartDate, dEndDate, dFirstYearDay, strEmployeeIds, oState)
        End Sub

        Public Sub New(oEmployeeData As roCalendarRowEmployeeData, oContracts As List(Of roMiniContract), oAlreadyPlannedHours As List(Of roEmployeeCounter), oPlannedHolidays As List(Of roEmployeeCounter), oWeekendDefinitions As List(Of roWeekendDefinition), dShiftNames As Dictionary(Of Integer, String))
            Contracts = oContracts
            AlreadyPlannedHours = oAlreadyPlannedHours
            EmployeeData = oEmployeeData
            PlannedHolidays = oPlannedHolidays
            WeekendDefinitions = oWeekendDefinitions
            ShiftsNames = dShiftNames
        End Sub

        Private Sub Load(oEmployeesScheduleRules As List(Of EmployeeScheduleRule), dStartDate As Date, dEndDate As Date, dFirstYearDay As Date, strEmployeeIds As String, ByRef oState As roCalendarScheduleRulesState)
            Dim dtEmployeesAlreadyPlannedHours As New DataTable
            Dim dtEmployeesPlannedHolidays As New DataTable
            Dim dtEmployeeContracts As New DataTable
            Dim dtEmployeesNotScheduledDays As New DataTable

            ' Cargo información de contratos
            dtEmployeeContracts = VTCalendar.roCalendarScheduleRulesManager.GetEmployeesContractsTable(dStartDate, dEndDate, dFirstYearDay, strEmployeeIds, oState)
            For Each oContractRow In dtEmployeeContracts.Rows
                Contracts.Add(New roMiniContract(oContractRow("BeginDate"), oContractRow("EndDate"), oContractRow("IDLabAgree"), oContractRow("IdContract"), oContractRow("IdEmployee")))
            Next

            ' Si entre las reglas está la de máximo de horas planificadas, cargo información de horas planificadas fuera del periodo de calendario
            If oEmployeesScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.IDRule = ScheduleRuleType.MinMaxExpectedHours).Count > 0 Then
                ' Hay alguna regla de horas teóricas. Cargo horas teóricas trabajadas por empleado
                dtEmployeesAlreadyPlannedHours = VTCalendar.roCalendarScheduleRulesManager.GetEmployeesAlreadyPlannedHoursTable(dStartDate, dEndDate, dFirstYearDay, strEmployeeIds, oState)
                For Each oEmployeeExpHoursRow In dtEmployeesAlreadyPlannedHours.Rows
                    AlreadyPlannedHours.Add(New roEmployeeCounter(oEmployeeExpHoursRow("IDLabAgree"), oEmployeeExpHoursRow("IDContract"), oEmployeeExpHoursRow("IDEmployee"), oEmployeeExpHoursRow("Value")))
                Next
            End If

            ' Si entre las reglas está la de máximo de días de vacaciones, cargo información de vacaciones planificadas fuera del periodo de calendario
            If oEmployeesScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.IDRule = ScheduleRuleType.MaxHolidays).Count > 0 Then
                ' Hay alguna regla de máximo de vacaciones. Cargo vacaciones planificadas fuera del periodo a evaluar
                dtEmployeesPlannedHolidays = VTCalendar.roCalendarScheduleRulesManager.GetEmployeesPlannedHolidaysTable(dStartDate, dEndDate, strEmployeeIds, oState)
                For Each oEmployeeHolidaysRow In dtEmployeesPlannedHolidays.Rows
                    PlannedHolidays.Add(New roEmployeeCounter(oEmployeeHolidaysRow("IDLabAgree"), oEmployeeHolidaysRow("IDContract"), oEmployeeHolidaysRow("IDEmployee"), oEmployeeHolidaysRow("Value")))
                Next
            End If

            ' Si entre las reglas a aplicar hay alguna de mínimo de fines de semana libres, cargo las diferentes definiciones de fines de semana que puede haber. OJO. De momento considero que la definición de días de fin se semana
            ' no puede sobreescribirse a nivel de contrato
            If oEmployeesScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.IDRule = ScheduleRuleType.MinWeekendsInPeriod).Count > 0 OrElse oEmployeesScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.IDRule = ScheduleRuleType.WorkOnWeekend).Count > 0 Then
                WeekendDefinitions = VTCalendar.roCalendarScheduleRulesManager.GetWeekendDefinitions(oState)
            End If

            ' Si entre las reglas hay alguna de máximos y mínimos de horarios en periodo, cargo los nombres de los horarios.
            ' (Para el resto de reglas los nombres los obtengo del propio objeto oCalendar, pero para esta regla se podría dar el caso que se monitirizara un horario que no estuviese planificado)
            If oEmployeesScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.IDRule = ScheduleRuleType.MinMaxShiftsInPeriod).Count > 0 Then
                Dim aShiftsID As New List(Of Integer)
                For Each oSchRule As EmployeeScheduleRule In oEmployeesScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.IDRule = ScheduleRuleType.MinMaxShiftsInPeriod)
                    For Each iIDShift As Integer In CType(oSchRule.ScheduleRule.Parameters, roScheduleRule_MinMaxShiftsInPeriod).CurrentDayShifts
                        If Not aShiftsID.Contains(iIDShift) Then aShiftsID.Add(iIDShift)
                    Next
                Next
                If aShiftsID.Count > 0 Then ShiftsNames = VTCalendar.roCalendarScheduleRulesManager.GetShiftNames(aShiftsID, oState)
            End If

            ' Si entre las reglas está la de máximo núnero de días sin planificar, cargo los días sin planificar fuera del periodo
            If oEmployeesScheduleRules.FindAll(Function(o) o.ScheduleRule.Parameters.IDRule = ScheduleRuleType.MaxNotScheduled).Count > 0 Then
                dtEmployeesNotScheduledDays = VTCalendar.roCalendarScheduleRulesManager.GetEmployeesNotScheduledDays(dStartDate, dEndDate, dFirstYearDay, strEmployeeIds, oState)
                For Each oEmployeeNotScheduledRow In dtEmployeesNotScheduledDays.Rows
                    NotScheduledDays.Add(New roEmployeeCounter(oEmployeeNotScheduledRow("IDLabAgree"), oEmployeeNotScheduledRow("IDContract"), oEmployeeNotScheduledRow("IDEmployee"), oEmployeeNotScheduledRow("Value")))
                Next
            End If

        End Sub

    End Class

    Public Class EmployeeScheduleRule
        Public Property IdEmployee As Integer
        Public Property ScheduleRule As ScheduleRuleManager
        Public Property PeriodYearFirstDate As Date
        Public Property StartDate As Date
        Public Property EndDate As Date

        Public Sub New(idEmp As Integer, oScheduleRuleManager As ScheduleRuleManager)
            IdEmployee = idEmp
            ScheduleRule = oScheduleRuleManager
        End Sub

    End Class

    ' Clase base para modelar reglas de planificación asignadas a un empleado y convenio o contrato
    Public MustInherit Class ScheduleRuleManager
        Public Parameters As roScheduleRule
        Protected _oState As roCalendarScheduleRulesState
        Protected _oWorkDayData As New List(Of roCalendarRowDayData)

        Public ReadOnly Property State
            Get
                Return _oState
            End Get
        End Property

        Public Property WorkDayData As List(Of roCalendarRowDayData)
            Get
                Return _oWorkDayData
            End Get
            Set(value As List(Of roCalendarRowDayData))
                _oWorkDayData = value
            End Set
        End Property

        Public MustOverride Function Applies(empobject As Object, dayobject As Object) As Boolean

        Public MustOverride Function Check(dayobject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)

    End Class

    Public Class ScheduleRule_OneShiftOneDayManager 'QC OK
        Inherits ScheduleRuleManager

        Private oEmpData As roEmployeeRulesData
        Private dIndictmentDates As New List(Of Date)

        Public Sub New(ByVal oRule As roScheduleRule_OneShiftOneDay, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.OneShiftOneDay
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = 2
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_OneShiftOneDayManager::Applies")
                Return Nothing
            End Try
            Return bolRet
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment
            Dim bIndictmentFound As Boolean = False
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData)
            Dim iCurrentDay As Integer = 0

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1

                Dim oCastRule = CType(Me.Parameters, roScheduleRule_OneShiftOneDay)
                ' Checkeo el último día de la secuencia (para comparaciones con el día anterior)
                If Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oCastRule.CurrentDayShifts.ToList().Contains(oShiftsQ(iCurrentDay).MainShift.ID) Then
                    ' Día
                    If oCastRule.Scope <> ScheduleRuleScope.Always Then
                        Select Case oCastRule.Scope
                            Case ScheduleRuleScope.Week
                                bIndictmentFound = oShiftsQ(iCurrentDay).PlanDate.DayOfWeek <> oCastRule.ReferenceDate.DayOfWeek
                            Case ScheduleRuleScope.Month
                                bIndictmentFound = (oShiftsQ(iCurrentDay).PlanDate.Month <> oCastRule.ReferenceDate.Month OrElse oShiftsQ(iCurrentDay).PlanDate.Day <> oCastRule.ReferenceDate.Day)
                            Case ScheduleRuleScope.Day
                                bIndictmentFound = (oShiftsQ(iCurrentDay).PlanDate <> oCastRule.ReferenceDate)
                        End Select

                        If bIndictmentFound Then
                            oIndictment = New roCalendarScheduleIndictment
                            oIndictment.DateBegin = oShiftsQ(iCurrentDay).PlanDate
                            oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                            dIndictmentDates.Add(oIndictment.DateBegin)
                            oIndictment.Dates = dIndictmentDates.ToArray
                            dIndictmentDates.Clear()
                            oIndictment.IDEmployee = oemp.IDEmployee
                            oIndictment.IDScheduleRule = oCastRule.IDRule
                            oIndictment.RuleName = oCastRule.RuleName
                            Me._oState.Language.ClearUserTokens()
                            Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).PlanDate.ToShortDateString)
                            Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).MainShift.Name)
                            If oCastRule.IdContract <> "" Then
                                Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                            Else
                                Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                            End If
                            Me._oState.Language.AddUserToken(oCastRule.RuleName)
                            oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.OneShift.Day", "")
                            oRet.Add(oIndictment)
                        End If
                    End If

                    ' Serie (si el día es correcto)
                    If oShiftsQ.Count > 1 AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing Then
                        ' El día anterior debe tener cierto horario
                        If Not bIndictmentFound AndAlso oCastRule.PreviousDayShifts.Count > 0 Then 'QCOK
                            Select Case oCastRule.Type
                                Case ShiftSequenceType.Unwanted
                                    bIndictmentFound = (Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oCastRule.CurrentDayShifts.Contains(oShiftsQ(iCurrentDay).MainShift.ID)) AndAlso (Not oShiftsQ(iCurrentDay - 1).MainShift Is Nothing AndAlso oCastRule.PreviousDayShifts.Contains(oShiftsQ(iCurrentDay - 1).MainShift.ID))
                                Case ShiftSequenceType.Wanted
                                    bIndictmentFound = (Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oCastRule.CurrentDayShifts.Contains(oShiftsQ(iCurrentDay).MainShift.ID)) AndAlso (Not oShiftsQ(iCurrentDay - 1).MainShift Is Nothing AndAlso Not oCastRule.PreviousDayShifts.Contains(oShiftsQ(iCurrentDay - 1).MainShift.ID))
                            End Select

                            If bIndictmentFound Then
                                oIndictment = New roCalendarScheduleIndictment
                                oIndictment.DateBegin = oShiftsQ(iCurrentDay - 1).PlanDate
                                oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                                dIndictmentDates.Add(oIndictment.DateBegin)
                                dIndictmentDates.Add(oIndictment.DateEnd)
                                oIndictment.Dates = dIndictmentDates.ToArray
                                dIndictmentDates.Clear()
                                oIndictment.IDEmployee = oemp.IDEmployee
                                oIndictment.IDScheduleRule = oCastRule.IDRule
                                oIndictment.RuleName = oCastRule.RuleName
                                Me._oState.Language.ClearUserTokens()
                                Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).PlanDate.ToShortDateString)
                                Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).MainShift.Name)
                                Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).PlanDate.ToShortDateString)
                                Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).MainShift.Name)
                                If oCastRule.IdContract <> "" Then
                                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                                Else
                                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                                End If
                                Me._oState.Language.AddUserToken(oCastRule.RuleName)
                                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.OneShift.Sequence.PreviousDays", "")
                                oRet.Add(oIndictment)
                            End If
                        End If

                        ' Descanso entre jornadas
                        If Not bIndictmentFound AndAlso oCastRule.RestHours <> -1 Then 'QC OK
                            If Not oShiftsQ(iCurrentDay - 1).MainShift Is Nothing AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso Not oShiftsQ(iCurrentDay - 1).MainShift.EndHour = Nothing AndAlso Not oShiftsQ(iCurrentDay - 1).IsHoliday AndAlso Not oShiftsQ(iCurrentDay).IsHoliday AndAlso Not oShiftsQ(iCurrentDay - 1).MainShift.PlannedHours = 0 AndAlso Not oShiftsQ(iCurrentDay).MainShift.PlannedHours = 0 Then
                                bIndictmentFound = (DateDiff(DateInterval.Hour, oShiftsQ(iCurrentDay - 1).MainShift.EndHour, oShiftsQ(iCurrentDay).MainShift.StartHour.AddDays(1)) < oCastRule.RestHours)
                            End If

                            If bIndictmentFound Then
                                oIndictment = New roCalendarScheduleIndictment
                                oIndictment.DateBegin = oShiftsQ(iCurrentDay - 1).PlanDate
                                oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                                dIndictmentDates.Add(oIndictment.DateBegin)
                                dIndictmentDates.Add(oIndictment.DateEnd)
                                oIndictment.Dates = dIndictmentDates.ToArray
                                dIndictmentDates.Clear()
                                oIndictment.IDEmployee = oemp.IDEmployee
                                oIndictment.IDScheduleRule = oCastRule.IDRule
                                oIndictment.RuleName = oCastRule.RuleName
                                Me._oState.Language.ClearUserTokens()
                                Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).PlanDate.ToShortDateString)
                                Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).MainShift.Name)
                                Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).PlanDate.ToShortDateString)
                                Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).MainShift.Name)
                                If oCastRule.IdContract <> "" Then
                                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                                Else
                                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                                End If
                                Me._oState.Language.AddUserToken(oCastRule.RuleName)
                                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.OneShift.RestTooShort", "")
                                oRet.Add(oIndictment)
                            End If
                        End If
                    End If
                End If

                ' Checkeo el primer día de la secuencia (para comparaciones con el día posterior)
                If Not bIndictmentFound AndAlso oCastRule.NextDayShifts.Count > 0 AndAlso oShiftsQ.Count = 2 AndAlso Not oShiftsQ(iCurrentDay - 1).MainShift Is Nothing AndAlso oCastRule.CurrentDayShifts.ToList().Contains(oShiftsQ(iCurrentDay - 1).MainShift.ID) Then
                    ' El día posterior debe tener cierto horario
                    Select Case oCastRule.TypeNextDays
                        Case ShiftSequenceType.Unwanted
                            bIndictmentFound = (Not oShiftsQ(iCurrentDay - 1).MainShift Is Nothing AndAlso oCastRule.CurrentDayShifts.Contains(oShiftsQ(iCurrentDay - 1).MainShift.ID)) AndAlso (Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oCastRule.NextDayShifts.Contains(oShiftsQ(iCurrentDay).MainShift.ID))
                        Case ShiftSequenceType.Wanted
                            bIndictmentFound = (Not oShiftsQ(iCurrentDay - 1).MainShift Is Nothing AndAlso oCastRule.CurrentDayShifts.Contains(oShiftsQ(iCurrentDay - 1).MainShift.ID)) AndAlso (Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso Not oCastRule.NextDayShifts.Contains(oShiftsQ(iCurrentDay).MainShift.ID))
                    End Select

                    If bIndictmentFound Then
                        oIndictment = New roCalendarScheduleIndictment
                        oIndictment.DateBegin = oShiftsQ(iCurrentDay - 1).PlanDate
                        oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                        dIndictmentDates.Add(oIndictment.DateBegin)
                        dIndictmentDates.Add(oIndictment.DateEnd)
                        oIndictment.Dates = dIndictmentDates.ToArray
                        dIndictmentDates.Clear()
                        oIndictment.IDEmployee = oemp.IDEmployee
                        oIndictment.IDScheduleRule = oCastRule.IDRule
                        oIndictment.RuleName = oCastRule.RuleName
                        Me._oState.Language.ClearUserTokens()
                        Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).MainShift.Name)
                        Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).PlanDate.ToShortDateString)
                        Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).MainShift.Name)
                        Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).PlanDate.ToShortDateString)
                        If oCastRule.IdContract <> "" Then
                            Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                        Else
                            Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                        End If
                        Me._oState.Language.AddUserToken(oCastRule.RuleName)
                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.OneShift.Sequence.NextDays", "")
                        oRet.Add(oIndictment)
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_OneShiftOneDayManager::Check")
                Return Nothing
            End Try

            Return oRet
        End Function

    End Class

    Public Class ScheduleRule_RestBetweenShiftsManager 'QCOK
        Inherits ScheduleRuleManager

        Private oEmpData As roEmployeeRulesData
        Private dIndictmentDates As New List(Of Date)

        Public Sub New(ByVal oRule As roScheduleRule_RestBetweenShifts, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.RestBetweenShifts
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = 2
            Me.Parameters.Scope = ScheduleRuleScope.Always
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_RestBetweenShiftsManager::Applies")
                Return Nothing
            End Try
            Return bolRet
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment
            Dim bIndictmentFound As Boolean = False
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData)
            Dim iCurrentDay As Integer = 0
            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1

                Dim oCastRule = CType(Me.Parameters, roScheduleRule_RestBetweenShifts)
                If oShiftsQ.Count > 1 Then
                    ' Descanso entre jornadas
                    If Not oShiftsQ(iCurrentDay - 1).MainShift Is Nothing AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso Not oShiftsQ(iCurrentDay - 1).MainShift.EndHour = Nothing AndAlso Not oShiftsQ(iCurrentDay - 1).IsHoliday AndAlso Not oShiftsQ(iCurrentDay).IsHoliday AndAlso Not oShiftsQ(iCurrentDay - 1).MainShift.PlannedHours = 0 AndAlso Not oShiftsQ(iCurrentDay).MainShift.PlannedHours = 0 Then
                        bIndictmentFound = (DateDiff(DateInterval.Hour, oShiftsQ(iCurrentDay - 1).MainShift.EndHour, oShiftsQ(iCurrentDay).MainShift.StartHour.AddDays(1)) < oCastRule.RestHours)
                    End If

                    If bIndictmentFound Then
                        oIndictment = New roCalendarScheduleIndictment
                        oIndictment.DateBegin = oShiftsQ(iCurrentDay - 1).PlanDate
                        oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                        dIndictmentDates.Add(oIndictment.DateBegin)
                        dIndictmentDates.Add(oIndictment.DateEnd)
                        oIndictment.Dates = dIndictmentDates.ToArray
                        oIndictment.IDEmployee = oemp.IDEmployee
                        oIndictment.IDScheduleRule = oCastRule.IDRule
                        oIndictment.RuleName = oCastRule.RuleName
                        Me._oState.Language.ClearUserTokens()
                        Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).PlanDate.ToShortDateString)
                        Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).MainShift.Name)
                        Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).PlanDate.ToShortDateString)
                        Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).MainShift.Name)
                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.Rest.RestTooShort", "")
                        If oCastRule.IdContract <> "" Then
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                        Else
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                        End If
                        oRet.Add(oIndictment)
                        dIndictmentDates.Clear()
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_RestBetweenShiftsManager::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

    End Class

    Public Class ScheduleRule_MinMaxExpectedHoursManager  'QC OK
        Inherits ScheduleRuleManager

        Private oEmpData As roEmployeeRulesData
        Protected lTotalHours As Double
        Private dIndictmentDates As New List(Of Date)
        Private bCheckDay As Boolean = False

        Public Sub New(ByVal oRule As roScheduleRule_MinMaxExpectedHours, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MinMaxExpectedHours
            Me.Parameters.RuleType = ScheduleRuleBaseType.System
            Me.Parameters.DayDepth = 2
            Me.Parameters.RuleName = Me._oState.Language.Translate("CalendarScheduleRules.MinMaxExpectedHours.RuleName", "")
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxExpectedHoursManager::Applies")
                Return True
            End Try
            Return True
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim oShiftsQ As New List(Of roCalendarRowDayData)
            Dim bIndictmentFound As Boolean = False
            Dim bPeriodChange As Boolean = False
            Dim iCurrentDay As Integer = 0

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1

                Dim oCastRule = CType(Me.Parameters, roScheduleRule_MinMaxExpectedHours)

                If bCheckDay AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing Then
                    If oShiftsQ(iCurrentDay).PlanDate <= dyearfirstdate.AddYears(1).AddDays(-1) Then
                        lTotalHours = lTotalHours + oShiftsQ(iCurrentDay).MainShift.PlannedHours / 60
                        If oShiftsQ(iCurrentDay).MainShift.PlannedHours / 60 > 0 AndAlso (oShiftsQ(iCurrentDay).PlanDate >= datestart AndAlso oShiftsQ(iCurrentDay).PlanDate <= dateend) Then
                            dIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        End If
                    End If
                End If

                If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                    ' En función del origen de la regla
                    If oCastRule.IdContract = "" Then
                        ' Viene de convenio
                        If oEmpData.AlreadyPlannedHours.Exists(Function(x) x.IdLabAgree = oCastRule.IdLabAgree AndAlso x.IdEmployee = oemp.IDEmployee) Then
                            lTotalHours = lTotalHours + oEmpData.AlreadyPlannedHours.Find(Function(x) x.IdLabAgree = oCastRule.IdLabAgree AndAlso x.IdEmployee = oemp.IDEmployee).Value
                        End If
                    Else
                        ' Viene de contrato
                        If oEmpData.AlreadyPlannedHours.Exists(Function(x) x.IdContract = oCastRule.IdContract AndAlso x.IdEmployee = oemp.IDEmployee) Then
                            lTotalHours = lTotalHours + oEmpData.AlreadyPlannedHours.Find(Function(x) x.IdContract = oCastRule.IdContract).Value
                        End If
                    End If

                    Dim iMaxWorkingHours As Integer = 0
                    If oCastRule.MaximumEmployeeField.Trim.Length > 0 Then
                        Dim vEmployeeField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oemp.IDEmployee, oCastRule.MaximumEmployeeField, dyearfirstdate, New UserFields.roUserFieldState(Me._oState.IDPassport), False)
                        If Not vEmployeeField Is Nothing Then
                            'OJO. Si no se informa el campo, siempre dará alerta.
                            iMaxWorkingHours = vEmployeeField.FieldValue
                        End If
                    Else
                        iMaxWorkingHours = oCastRule.MaximumWorkingHours
                    End If

                    If lTotalHours > (iMaxWorkingHours + oCastRule.MaximumWorkingHoursFork) Then
                        oIndictment = New roCalendarScheduleIndictment
                        oIndictment.DateBegin = datestart
                        oIndictment.DateEnd = dateend
                        oIndictment.IDEmployee = oemp.IDEmployee
                        oIndictment.IDScheduleRule = oCastRule.IDRule
                        oIndictment.RuleName = oCastRule.RuleName
                        oIndictment.Dates = dIndictmentDates.ToArray
                        Me._oState.Language.ClearUserTokens()
                        Me._oState.Language.AddUserToken(roConversions.ConvertHoursToTime(lTotalHours))
                        Me._oState.Language.AddUserToken(dyearfirstdate.Year.ToString)
                        Me._oState.Language.AddUserToken(iMaxWorkingHours.ToString)
                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinMaxExpectedHours", "")
                        If oCastRule.IdContract <> "" Then
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                        Else
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                        End If
                        oRet.Add(oIndictment)
                    End If

                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxExpectedHoursManager::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

    End Class

    Public Class ScheduleRule_MinMaxFreeLabourDaysInPeriodManager
        Inherits ScheduleRuleManager

        Protected Property RestDays As Integer = 0
        Protected Property LabourDays As Integer = 0
        Protected Property dLastPeriodBegin As Date
        Private oEmpData As roEmployeeRulesData
        Private dIndictmentDates As New List(Of Date)
        Private dFreeDates As New List(Of Date)
        Private dLabourDates As New List(Of Date)

        Public Sub New(ByVal oRule As roScheduleRule_MinMaxFreeLabourDaysInPeriod, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MinMaxFreeLabourDaysInPeriod
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = 2
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_FreeDaysInPeriodManager::Applies")
                Return Nothing
            End Try
            Return bolRet
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim oShiftsQ As New List(Of roCalendarRowDayData)
            Dim bIndictmentFound As Boolean = False
            Dim bPeriodChange As Boolean = False
            Dim iCurrentDay As Integer = 0

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1

                Dim oCastRule = CType(Me.Parameters, roScheduleRule_MinMaxFreeLabourDaysInPeriod)

                If oShiftsQ.Count = 1 Then
                    ' Primer día del periodo. Si es de descanso lo añado a la colección.
                    If oCastRule.DaysType = DaysType.Free Then
                        If oShiftsQ(iCurrentDay).MainShift Is Nothing OrElse oShiftsQ(iCurrentDay).MainShift.PlannedHours = 0 Then
                            RestDays = RestDays + 1
                            dFreeDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        Else
                            ' Si no, es un candidato a incumplir la regla
                            dLabourDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        End If
                    Else
                        If Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oShiftsQ(iCurrentDay).MainShift.PlannedHours > 0 Then
                            LabourDays = LabourDays + 1
                            dLabourDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        Else
                            ' Si no, es un candidato a incumplir la regla
                            dFreeDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        End If
                    End If

                    ' Si estamos en el último día evaluo el resultado
                    If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                        ' Compruebo Mínimos y Máximos
                        If (oCastRule.DaysType = DaysType.Free AndAlso oCastRule.MinimumRestDays > -1 AndAlso RestDays < oCastRule.MinimumRestDays) OrElse (oCastRule.DaysType = DaysType.Labour AndAlso oCastRule.MinimumLabourDays > -1 AndAlso LabourDays < oCastRule.MinimumLabourDays) OrElse
                           (oCastRule.DaysType = DaysType.Free AndAlso oCastRule.MaximumRestDays > -1 AndAlso RestDays > oCastRule.MaximumRestDays) OrElse (oCastRule.DaysType = DaysType.Labour AndAlso oCastRule.MaximumLabourDays > -1 AndAlso LabourDays > oCastRule.MaximumLabourDays) Then
                            'Creo impacto
                            oIndictment = New roCalendarScheduleIndictment
                            oIndictment.DateBegin = IIf(dLastPeriodBegin >= datestart, dLastPeriodBegin, datestart)
                            oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                            dIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                            oIndictment.IDEmployee = oemp.IDEmployee
                            oIndictment.IDScheduleRule = oCastRule.IDRule
                            oIndictment.RuleName = oCastRule.RuleName
                            Me._oState.Language.ClearUserTokens()
                            Select Case oCastRule.DaysType
                                Case DaysType.Free
                                    oIndictment.Dates = dFreeDates.ToArray
                                    Me._oState.Language.AddUserToken(RestDays)
                                    If RestDays < oCastRule.MinimumRestDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MinimumRestDays.ToString)
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.FreeDays.Minimum", "")
                                    ElseIf RestDays > oCastRule.MaximumRestDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MaximumRestDays.ToString)
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.FreeDays.Maximum", "")
                                    End If
                                Case DaysType.Labour
                                    oIndictment.Dates = dLabourDates.ToArray
                                    Me._oState.Language.AddUserToken(LabourDays)
                                    If LabourDays < oCastRule.MinimumLabourDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MinimumLabourDays.ToString)
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.LabourDays.Minimum", "")
                                    ElseIf LabourDays > oCastRule.MaximumLabourDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MaximumLabourDays.ToString)
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.LabourDays.Maximum", "")
                                    End If
                            End Select

                            If oCastRule.IdContract <> "" Then
                                oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                            Else
                                oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                            End If
                            oRet.Add(oIndictment)
                        End If

                        ' Reseteo el contador para cuando se evalue la regla con otro empleado
                        RestDays = 0
                        LabourDays = 0
                        dFreeDates.Clear()
                        dLabourDates.Clear()
                    Else
                        dLastPeriodBegin = oShiftsQ(iCurrentDay).PlanDate
                    End If
                Else
                    ' Miro si ha habido cambio de ciclo en función del tipo
                    Select Case oCastRule.Scope
                        Case ScheduleRuleScope.Month
                            bPeriodChange = oShiftsQ(iCurrentDay - 1).PlanDate.Month <> oShiftsQ(iCurrentDay).PlanDate.Month
                        Case ScheduleRuleScope.Year
                            bPeriodChange = oShiftsQ(iCurrentDay - 1).PlanDate.Year <> oShiftsQ(iCurrentDay).PlanDate.Year
                        Case ScheduleRuleScope.Week
                            bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.DayOfWeek = 1)
                    End Select

                    ' Si hubo cambio de periodo, evaluo ahora
                    If bPeriodChange Then
                        If (oCastRule.DaysType = DaysType.Free AndAlso oCastRule.MinimumRestDays > -1 AndAlso RestDays < oCastRule.MinimumRestDays) OrElse (oCastRule.DaysType = DaysType.Labour AndAlso oCastRule.MinimumLabourDays > -1 AndAlso LabourDays < oCastRule.MinimumLabourDays) OrElse
                           (oCastRule.DaysType = DaysType.Free AndAlso oCastRule.MaximumRestDays > -1 AndAlso RestDays > oCastRule.MaximumRestDays) OrElse (oCastRule.DaysType = DaysType.Labour AndAlso oCastRule.MaximumLabourDays > -1 AndAlso LabourDays > oCastRule.MaximumLabourDays) Then
                            'If RestDays < oCastRule.MinimumRestDays Then
                            'Creo impacto
                            oIndictment = New roCalendarScheduleIndictment
                            oIndictment.DateBegin = IIf(dLastPeriodBegin >= datestart, dLastPeriodBegin, datestart)
                            oIndictment.DateEnd = oShiftsQ(iCurrentDay - 1).PlanDate
                            oIndictment.IDEmployee = oemp.IDEmployee
                            oIndictment.IDScheduleRule = oCastRule.IDRule
                            oIndictment.RuleName = oCastRule.RuleName
                            Me._oState.Language.ClearUserTokens()
                            Select Case oCastRule.DaysType
                                Case DaysType.Free
                                    Me._oState.Language.AddUserToken(RestDays)
                                    If RestDays < oCastRule.MinimumRestDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MinimumRestDays.ToString)
                                        oIndictment.Dates = dLabourDates.ToArray
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.FreeDays.Minimum", "")
                                    ElseIf RestDays > oCastRule.MaximumRestDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MaximumRestDays.ToString)
                                        oIndictment.Dates = dFreeDates.ToArray
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.FreeDays.Maximum", "")
                                    End If
                                Case DaysType.Labour
                                    Me._oState.Language.AddUserToken(LabourDays)
                                    If LabourDays < oCastRule.MinimumLabourDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MinimumLabourDays.ToString)
                                        oIndictment.Dates = dFreeDates.ToArray
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.LabourDays.Minimum", "")
                                    ElseIf LabourDays > oCastRule.MaximumLabourDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MaximumLabourDays.ToString)
                                        oIndictment.Dates = dLabourDates.ToArray
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.LabourDays.Maximum", "")
                                    End If
                            End Select
                            If oCastRule.IdContract <> "" Then
                                oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                            Else
                                oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                            End If
                            oRet.Add(oIndictment)
                        End If
                        dLastPeriodBegin = oShiftsQ(iCurrentDay).PlanDate
                        RestDays = 0
                        LabourDays = 0
                        dFreeDates.Clear()
                        dLabourDates.Clear()
                    End If

                    If oCastRule.DaysType = DaysType.Free Then
                        If oShiftsQ(iCurrentDay).MainShift Is Nothing OrElse oShiftsQ(iCurrentDay).MainShift.PlannedHours = 0 Then
                            RestDays = RestDays + 1
                            dFreeDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        Else
                            ' Si no, es un candidato a incumplir la regla
                            dLabourDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        End If
                    Else
                        If Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oShiftsQ(iCurrentDay).MainShift.PlannedHours > 0 Then
                            LabourDays = LabourDays + 1
                            dLabourDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        Else
                            ' Si no, es un candidato a incumplir la regla
                            dFreeDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        End If
                    End If

                    ' Si estamos en el último día evaluo el resultado
                    If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                        'If RestDays < oCastRule.MinimumRestDays Then
                        If (oCastRule.DaysType = DaysType.Free AndAlso oCastRule.MinimumRestDays > -1 AndAlso RestDays < oCastRule.MinimumRestDays) OrElse (oCastRule.DaysType = DaysType.Labour AndAlso oCastRule.MinimumLabourDays > -1 AndAlso LabourDays < oCastRule.MinimumLabourDays) OrElse
                           (oCastRule.DaysType = DaysType.Free AndAlso oCastRule.MaximumRestDays > -1 AndAlso RestDays > oCastRule.MaximumRestDays) OrElse (oCastRule.DaysType = DaysType.Labour AndAlso oCastRule.MaximumLabourDays > -1 AndAlso LabourDays > oCastRule.MaximumLabourDays) Then
                            'Creo impacto
                            oIndictment = New roCalendarScheduleIndictment
                            oIndictment.DateBegin = IIf(dLastPeriodBegin >= datestart, dLastPeriodBegin, datestart)
                            oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                            oIndictment.IDEmployee = oemp.IDEmployee
                            oIndictment.IDScheduleRule = oCastRule.IDRule
                            oIndictment.RuleName = oCastRule.RuleName
                            Me._oState.Language.ClearUserTokens()
                            Select Case oCastRule.DaysType
                                Case DaysType.Free
                                    Me._oState.Language.AddUserToken(RestDays)
                                    If RestDays < oCastRule.MinimumRestDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MinimumRestDays.ToString)
                                        oIndictment.Dates = dLabourDates.ToArray
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.FreeDays.Minimum", "")
                                    ElseIf RestDays > oCastRule.MaximumRestDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MaximumRestDays.ToString)
                                        oIndictment.Dates = dFreeDates.ToArray
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.FreeDays.Maximum", "")
                                    End If
                                Case DaysType.Labour
                                    Me._oState.Language.AddUserToken(LabourDays)
                                    If LabourDays < oCastRule.MinimumLabourDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MinimumLabourDays.ToString)
                                        oIndictment.Dates = dFreeDates.ToArray
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.LabourDays.Minimum", "")
                                    ElseIf LabourDays > oCastRule.MaximumLabourDays Then
                                        Me._oState.Language.AddUserToken(oCastRule.MaximumLabourDays.ToString)
                                        oIndictment.Dates = dLabourDates.ToArray
                                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.LabourDays.Maximum", "")
                                    End If
                            End Select
                            If oCastRule.IdContract <> "" Then
                                oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                            Else
                                oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                            End If
                            oRet.Add(oIndictment)
                        End If
                        ' Reseteo el contador para cuando se evalue la regla con otro empleado
                        RestDays = 0
                        LabourDays = 0
                        dFreeDates.Clear()
                        dLabourDates.Clear()
                    End If

                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_FreeDaysInPeriodManager::Check")
                Return Nothing
            End Try
            Return oRet

        End Function

    End Class

    Public Class ScheduleRule_MinMaxDaysSequenceManager
        Inherits ScheduleRuleManager

        Protected Property DaysInCurrentSequence As Integer = 0
        Protected Property InSequence As Boolean = False
        Protected Property CurrentSequenceStart As Date = Date.MinValue
        Protected Property CurrentSequenceEnd As Date = Date.MinValue
        Private oEmpData As roEmployeeRulesData
        Private dTmpIndictmentDates As New List(Of Date)

        Public Sub New(ByVal oRule As roScheduleRule_MinMaxDaysSequence, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MinMaxDaysSequence
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = 2
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxDaysSequenceManager::Applies")
                Return Nothing
            End Try
            Return bolRet
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim oShiftsQ As New List(Of roCalendarRowDayData)
            Dim bIndictmentFound As Boolean = False
            Dim iCurrentDay As Integer = 0
            Dim bCheckSequence As Boolean = False

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1

                Dim oCastRule = CType(Me.Parameters, roScheduleRule_MinMaxDaysSequence)

                If oCastRule.DaysType = DaysType.Labour Then
                    If Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oShiftsQ(iCurrentDay).MainShift.PlannedHours > 0 Then
                        DaysInCurrentSequence = DaysInCurrentSequence + 1
                        dTmpIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        If Not InSequence Then
                            CurrentSequenceStart = oShiftsQ(iCurrentDay).PlanDate
                            InSequence = True
                            bCheckSequence = False
                        End If
                    Else
                        If InSequence Then
                            CurrentSequenceEnd = oShiftsQ(iCurrentDay).PlanDate.AddDays(-1)
                            bCheckSequence = True
                            InSequence = False
                        End If
                    End If
                Else
                    If oShiftsQ(iCurrentDay).MainShift Is Nothing OrElse oShiftsQ(iCurrentDay).MainShift.PlannedHours = 0 Then
                        DaysInCurrentSequence = DaysInCurrentSequence + 1
                        dTmpIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        If Not InSequence Then
                            CurrentSequenceStart = oShiftsQ(iCurrentDay).PlanDate
                            InSequence = True
                            bCheckSequence = False
                        End If
                    Else
                        If InSequence Then
                            CurrentSequenceEnd = oShiftsQ(iCurrentDay).PlanDate.AddDays(-1)
                            bCheckSequence = True
                            InSequence = False
                        End If
                    End If
                End If

                ' Si se acaba el periodo, o bien salgo de secuencia, checkeo ...
                If oShiftsQ(iCurrentDay).PlanDate = dateend OrElse bCheckSequence Then
                    If DaysInCurrentSequence > oCastRule.MaximumDays Then
                        oIndictment = New roCalendarScheduleIndictment
                        oIndictment.DateBegin = CurrentSequenceStart
                        oIndictment.DateEnd = CurrentSequenceEnd
                        oIndictment.Dates = dTmpIndictmentDates.ToArray
                        oIndictment.IDEmployee = oemp.IDEmployee
                        oIndictment.IDScheduleRule = oCastRule.IDRule
                        oIndictment.RuleName = oCastRule.RuleName
                        Me._oState.Language.ClearUserTokens()
                        Me._oState.Language.AddUserToken(DaysInCurrentSequence)
                        Me._oState.Language.AddUserToken(oCastRule.MaximumDays.ToString)
                        Select Case oCastRule.DaysType
                            Case DaysType.Free
                                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.FreeDaysSequence.Maximum", "")
                            Case DaysType.Labour
                                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.LabourDaysSequence.Maximum", "")
                        End Select
                        If oCastRule.IdContract <> "" Then
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                        Else
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                        End If
                        oRet.Add(oIndictment)
                    End If
                    DaysInCurrentSequence = 0
                    dTmpIndictmentDates.Clear()
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxDaysSequenceManager::Check")
                Return Nothing
            End Try
            Return oRet

        End Function

    End Class

    Public Class ScheduleRule_MinMaxShiftsInPeriodManager 'QCOK
        Inherits ScheduleRuleManager

        Private oEmpData As roEmployeeRulesData
        Protected Property dLastPeriodBegin As Date
        Protected Property dLastPeriodEnd As Date
        Protected dPeriodDateStart As Date
        Protected dShiftsTotal As New Dictionary(Of Integer, Integer)
        Protected dShiftsDatesDetail As New Dictionary(Of Integer, List(Of Date))
        Protected dIndictmentDatesForMinimumRule As New List(Of Date)
        Protected dShiftsNames As New Dictionary(Of Integer, String)
        Protected oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData)
        Protected iIDEmployee As Integer = 0
        Protected iCurrentDay As Integer = 0
        Protected oCastRule As roScheduleRule_MinMaxShiftsInPeriod
        Protected bCheckDay As Boolean = False

        Public Sub New(ByVal oRule As roScheduleRule_MinMaxShiftsInPeriod, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MinMaxShiftsInPeriod
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = 2
            Me.oCastRule = CType(Me.Parameters, roScheduleRule_MinMaxShiftsInPeriod)
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            oShiftsQ = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
                ' Y que estemos dentro del periodo de validación
                If bCheckDay AndAlso (oCastRule.BeginPeriod.Year <> 1970 AndAlso oCastRule.EndPeriod.Year <> 1970) Then
                    bCheckDay = (oShiftsQ(oShiftsQ.Count - 1).PlanDate >= oCastRule.BeginPeriod AndAlso oShiftsQ(oShiftsQ.Count - 1).PlanDate <= oCastRule.EndPeriod)
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxShiftsInPeriodManager::Applies")
                Return True
            End Try
            Return True
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim oShiftsQ As New List(Of roCalendarRowDayData)
            Dim bIndictmentFound As Boolean = False
            Dim bPeriodChange As Boolean = False
            Try
                iCurrentDay = 0
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1
                dPeriodDateStart = datestart

                'oCastRule = CType(Me.Parameters, roScheduleRule_MinMaxShiftsInPeriod)

                iIDEmployee = oemp.IDEmployee

                If oShiftsQ.Count = 1 Then
                    ' Primer día del periodo.
                    ' Inicializo los contadores con todos los horarios que voy a monitorizar, por si esos horarios no se han planificado en todo el periodo
                    For Each i As Integer In oCastRule.CurrentDayShifts
                        dShiftsTotal.Add(i, 0)
                    Next

                    ' Trato el horario
                    If bCheckDay AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing Then
                        '  Si no lo tengo en la colección de contadores, lo añado ...
                        If Not dShiftsTotal.ContainsKey(oShiftsQ(iCurrentDay).MainShift.ID) Then
                            dShiftsTotal.Add(oShiftsQ(iCurrentDay).MainShift.ID, 0)
                        End If
                        If Not dShiftsDatesDetail.ContainsKey(oShiftsQ(iCurrentDay).MainShift.ID) Then
                            dShiftsDatesDetail.Add(oShiftsQ(iCurrentDay).MainShift.ID, New List(Of Date))
                        End If
                        dShiftsTotal(oShiftsQ(iCurrentDay).MainShift.ID) = dShiftsTotal(oShiftsQ(iCurrentDay).MainShift.ID) + 1
                        dShiftsDatesDetail(oShiftsQ(iCurrentDay).MainShift.ID).Add(oShiftsQ(iCurrentDay).PlanDate)
                    End If
                    If bCheckDay Then dIndictmentDatesForMinimumRule.Add(oShiftsQ(iCurrentDay).PlanDate)

                    ' Si estamos en el último día evaluo el resultado
                    If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                        dLastPeriodEnd = oShiftsQ(iCurrentDay).PlanDate
                        If Not oCastRule.LogicOr Then
                            ' Todos los horarios deben cumplir el máximo o mínimo
                            For Each iShiftID As Integer In oCastRule.CurrentDayShifts
                                If oCastRule.Maximum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) > oCastRule.Maximum Then
                                    oIndictment = CreateMaxIndictment(iShiftID)
                                    oRet.Add(oIndictment)
                                End If
                                If oCastRule.Minimum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) < oCastRule.Minimum Then
                                    oIndictment = CreateMinIndictment(iShiftID)
                                    oRet.Add(oIndictment)
                                End If
                            Next
                        Else
                            ' Basta que un horario cumpla
                            Dim bFound As Boolean = False
                            For Each iShiftID As Integer In oCastRule.CurrentDayShifts
                                bFound = (oCastRule.Maximum = -1) OrElse (oCastRule.Maximum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) <= oCastRule.Maximum)
                                If bFound Then
                                    bFound = (oCastRule.Minimum = -1) OrElse (oCastRule.Minimum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) >= oCastRule.Minimum)
                                End If
                                If bFound Then Exit For
                            Next
                            If Not bFound Then
                                oIndictment = CreateOrIndictment()
                                oRet.Add(oIndictment)
                            End If
                        End If
                        ' Reseteo el contador para cuando se evalue la regla con otro empleado
                        For Each iKey As Integer In dShiftsTotal.Keys.ToList
                            dShiftsTotal(iKey) = 0
                            dShiftsDatesDetail(iKey) = New List(Of Date)
                        Next
                        dIndictmentDatesForMinimumRule = New List(Of Date)
                    Else
                        dLastPeriodBegin = oShiftsQ(iCurrentDay).PlanDate
                    End If
                Else
                    ' Miro si ha habido cambio de ciclo en función del tipo
                    Select Case oCastRule.Scope
                        Case ScheduleRuleScope.Month
                            bPeriodChange = oShiftsQ(iCurrentDay - 1).PlanDate.Month <> oShiftsQ(iCurrentDay).PlanDate.Month
                            If bPeriodChange AndAlso oCastRule.ScopePeriods > 1 Then
                                bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.Date.Subtract(dLastPeriodBegin).Days) >= 28 * oCastRule.ScopePeriods
                            End If
                        Case ScheduleRuleScope.Year
                            bPeriodChange = oShiftsQ(iCurrentDay - 1).PlanDate.Year <> oShiftsQ(iCurrentDay).PlanDate.Year
                            If bPeriodChange AndAlso oCastRule.ScopePeriods > 1 Then
                                bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.Date.Subtract(dLastPeriodBegin).Days) >= 365 * oCastRule.ScopePeriods
                            End If
                        Case ScheduleRuleScope.Week
                            bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.DayOfWeek = 1)
                            If bPeriodChange AndAlso oCastRule.ScopePeriods > 1 Then
                                bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.Date.Subtract(dLastPeriodBegin).Days) >= 7 * oCastRule.ScopePeriods
                            End If
                    End Select

                    ' Si hubo cambio de periodo, evaluo ahora
                    If bPeriodChange Then
                        dLastPeriodEnd = oShiftsQ(iCurrentDay - 1).PlanDate
                        If Not oCastRule.LogicOr Then
                            For Each iShiftID As Integer In oCastRule.CurrentDayShifts
                                If oCastRule.Maximum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) > oCastRule.Maximum Then
                                    oIndictment = CreateMaxIndictment(iShiftID)
                                    oIndictment.DateEnd = oShiftsQ(iCurrentDay - 1).PlanDate
                                    oRet.Add(oIndictment)
                                End If
                                If oCastRule.Minimum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) < oCastRule.Minimum Then
                                    oIndictment = CreateMinIndictment(iShiftID)
                                    oIndictment.DateEnd = oShiftsQ(iCurrentDay - 1).PlanDate
                                    oRet.Add(oIndictment)
                                End If
                            Next
                        Else
                            ' Basta que un horario cumpla
                            Dim bFound As Boolean = False
                            For Each iShiftID As Integer In oCastRule.CurrentDayShifts
                                bFound = (oCastRule.Maximum = -1) OrElse (oCastRule.Maximum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) <= oCastRule.Maximum)
                                If bFound Then
                                    bFound = (oCastRule.Minimum = -1) OrElse (oCastRule.Minimum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) >= oCastRule.Minimum)
                                End If
                                If bFound Then Exit For
                            Next
                            If Not bFound Then
                                oIndictment = CreateOrIndictment()
                                oRet.Add(oIndictment)
                            End If
                        End If

                        dLastPeriodBegin = oShiftsQ(iCurrentDay).PlanDate
                        For Each iKey As Integer In dShiftsTotal.Keys.ToList
                            dShiftsTotal(iKey) = 0
                            dShiftsDatesDetail(iKey) = New List(Of Date)
                        Next
                        dIndictmentDatesForMinimumRule = New List(Of Date)
                    End If
                    If bCheckDay AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing Then
                        If Not dShiftsTotal.ContainsKey(oShiftsQ(iCurrentDay).MainShift.ID) Then
                            dShiftsTotal.Add(oShiftsQ(iCurrentDay).MainShift.ID, 0)
                        End If
                        If Not dShiftsDatesDetail.ContainsKey(oShiftsQ(iCurrentDay).MainShift.ID) Then
                            dShiftsDatesDetail.Add(oShiftsQ(iCurrentDay).MainShift.ID, New List(Of Date))
                        End If
                        dShiftsTotal(oShiftsQ(iCurrentDay).MainShift.ID) = dShiftsTotal(oShiftsQ(iCurrentDay).MainShift.ID) + 1
                        dShiftsDatesDetail(oShiftsQ(iCurrentDay).MainShift.ID).Add(oShiftsQ(iCurrentDay).PlanDate)
                    End If
                    If bCheckDay Then dIndictmentDatesForMinimumRule.Add(oShiftsQ(iCurrentDay).PlanDate)

                    ' Si estamos en el último día evaluo el resultado
                    If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                        dLastPeriodEnd = oShiftsQ(iCurrentDay).PlanDate
                        If Not oCastRule.LogicOr Then
                            For Each iShiftID As Integer In oCastRule.CurrentDayShifts
                                If oCastRule.Maximum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) > oCastRule.Maximum Then
                                    oIndictment = CreateMaxIndictment(iShiftID)
                                    oRet.Add(oIndictment)
                                End If
                                If oCastRule.Minimum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) < oCastRule.Minimum Then
                                    oIndictment = CreateMinIndictment(iShiftID)
                                    oRet.Add(oIndictment)
                                End If
                            Next
                        Else
                            ' Basta que un horario cumpla
                            Dim bFound As Boolean = False
                            For Each iShiftID As Integer In oCastRule.CurrentDayShifts
                                bFound = (oCastRule.Maximum = -1) OrElse (oCastRule.Maximum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) <= oCastRule.Maximum)
                                If bFound Then
                                    bFound = (oCastRule.Minimum = -1) OrElse (oCastRule.Minimum > -1 AndAlso dShiftsTotal.ContainsKey(iShiftID) AndAlso dShiftsTotal(iShiftID) >= oCastRule.Minimum)
                                End If
                                If bFound Then Exit For
                            Next
                            If Not bFound Then
                                oIndictment = CreateOrIndictment()
                                oRet.Add(oIndictment)
                            End If
                        End If

                        ' Reseteo el contador para cuando se evalue la regla con otro empleado
                        For Each iKey As Integer In dShiftsTotal.Keys.ToList
                            dShiftsTotal(iKey) = 0
                            dShiftsDatesDetail(iKey) = New List(Of Date)
                        Next
                    End If

                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxShiftsInPeriodManager::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

        Private Function CreateMaxIndictment(iShiftID As Integer) As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = IIf(dLastPeriodBegin >= dPeriodDateStart, dLastPeriodBegin, dPeriodDateStart)
                oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                oIndictment.Dates = dShiftsDatesDetail(iShiftID).ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(IIf(oEmpData.ShiftsNames.ContainsKey(iShiftID), oEmpData.ShiftsNames(iShiftID), "?"))
                Me._oState.Language.AddUserToken(dShiftsTotal(iShiftID))
                Me._oState.Language.AddUserToken(dLastPeriodBegin.ToShortDateString)
                Me._oState.Language.AddUserToken(dLastPeriodEnd.ToShortDateString)
                Me._oState.Language.AddUserToken(oCastRule.Maximum.ToString)
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinMaxShifts.Maximum", "")
                If oCastRule.IdContract <> "" Then
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                Else
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxExpectedHoursInPeriod::CreateMaxIndictment")
                Return Nothing
            End Try
            Return oIndictment
        End Function

        Private Function CreateMinIndictment(iShiftID As Integer) As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = IIf(dLastPeriodBegin >= dPeriodDateStart, dLastPeriodBegin, dPeriodDateStart)
                oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                dIndictmentDatesForMinimumRule.Add(oShiftsQ(iCurrentDay).PlanDate)
                oIndictment.Dates = dIndictmentDatesForMinimumRule.ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(IIf(oEmpData.ShiftsNames.ContainsKey(iShiftID), oEmpData.ShiftsNames(iShiftID), "?"))
                Me._oState.Language.AddUserToken(dShiftsTotal(iShiftID))
                Me._oState.Language.AddUserToken(dLastPeriodBegin.ToShortDateString)
                Me._oState.Language.AddUserToken(dLastPeriodEnd.ToShortDateString)
                Me._oState.Language.AddUserToken(oCastRule.Minimum.ToString)
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinMaxShifts.Minimum", "")
                If oCastRule.IdContract <> "" Then
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                Else
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxExpectedHoursInPeriod::CreateMaxIndictment")
                Return Nothing
            End Try
            Return oIndictment
        End Function

        Private Function CreateOrIndictment() As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = IIf(dLastPeriodBegin >= dPeriodDateStart, dLastPeriodBegin, dPeriodDateStart)
                oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                oIndictment.Dates = dIndictmentDatesForMinimumRule.ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(String.Join(", ", oEmpData.ShiftsNames.Select(Function(kvp) String.Format("{1}", kvp.Key, kvp.Value))))
                If oCastRule.Minimum > -1 AndAlso oCastRule.Maximum > -1 Then
                    Me._oState.Language.AddUserToken(oCastRule.Minimum.ToString)
                    Me._oState.Language.AddUserToken(oCastRule.Maximum.ToString)
                    Me._oState.Language.AddUserToken(dLastPeriodBegin.ToShortDateString)
                    Me._oState.Language.AddUserToken(dLastPeriodEnd.ToShortDateString)
                    oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinMaxShifts.Or", "")
                ElseIf oCastRule.Minimum > -1 Then
                    Me._oState.Language.AddUserToken(oCastRule.Minimum.ToString)
                    Me._oState.Language.AddUserToken(dLastPeriodBegin.ToShortDateString)
                    Me._oState.Language.AddUserToken(dLastPeriodEnd.ToShortDateString)
                    oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinMaxShifts.Or.Min", "")
                ElseIf oCastRule.Maximum > -1 Then
                    Me._oState.Language.AddUserToken(oCastRule.Maximum.ToString)
                    Me._oState.Language.AddUserToken(dLastPeriodBegin.ToShortDateString)
                    Me._oState.Language.AddUserToken(dLastPeriodEnd.ToShortDateString)
                    oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinMaxShifts.Or.Max", "")
                End If

                If oCastRule.IdContract <> "" Then
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                Else
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxExpectedHoursInPeriod::CreateOrIndictment")
                Return Nothing
            End Try
            Return oIndictment
        End Function

    End Class

    Public Class ScheduleRule_MinWeekendsInPeriodManager 'QC OK
        Inherits ScheduleRuleManager
        Private oEmpData As roEmployeeRulesData
        Protected lTotalFreeWeekends As Integer
        Private dIndictmentDates As New List(Of Date)
        Private bCheckDay As Boolean = False

        Public Sub New(ByVal oRule As roScheduleRule_MinWeekendsInPeriod, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MinWeekendsInPeriod
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = 7
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                    ' Sólo verifico si es domingo, contando que tengo en la cola una semana entera (DayDepth = 7)
                    bCheckDay = bolRet AndAlso oShiftsQ.Last.PlanDate.DayOfWeek = DayOfWeek.Sunday
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinWeekendsInPeriodManager::Applies")
                Return True
            End Try
            Return True
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment
            Dim bIndictmentFound As Boolean = False
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData)
            Dim oWeekendDefinition As roWeekendDefinition
            Dim bWorkOnWeekend As Boolean = False

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))

                Dim oCastRule = CType(Me.Parameters, roScheduleRule_MinWeekendsInPeriod)

                ' Valido si los días configurados como fin de semana los tiene efectivamente sin planificar como día laborable
                ' Recupero definición del fin de semana
                oWeekendDefinition = oEmpData.WeekendDefinitions.Find(Function(x) x.IdLabAgree = oCastRule.IdLabAgree)

                ' 1.- Debo asegurar que la secuencia de días contiene todos los días que forman el fin de semana
                If bCheckDay Then
                    If oShiftsQ.FindAll(Function(o) oWeekendDefinition.WeekEndDays.Contains(o.PlanDate.DayOfWeek)).Count = oWeekendDefinition.WeekEndDays.Count Then
                        For Each oDayData In oShiftsQ.FindAll(Function(o) oWeekendDefinition.WeekEndDays.Contains(o.PlanDate.DayOfWeek))
                            If oDayData.MainShift IsNot Nothing AndAlso oDayData.MainShift.PlannedHours > 0 Then
                                ' Ha trabajado en un fin de semana. No sumo.
                                bWorkOnWeekend = True
                                dIndictmentDates.Add(oDayData.PlanDate)
                            End If
                        Next
                    End If
                    If Not bWorkOnWeekend Then lTotalFreeWeekends = lTotalFreeWeekends + 1
                End If

                If oShiftsQ.Last.PlanDate = dateend Then
                    ' Valido que no estoy por debajo del mínimo
                    If lTotalFreeWeekends < oCastRule.Minimum Then
                        oIndictment = New roCalendarScheduleIndictment
                        oIndictment.DateBegin = datestart
                        oIndictment.DateEnd = dateend
                        oIndictment.Dates = dIndictmentDates.ToArray
                        oIndictment.IDEmployee = oemp.IDEmployee
                        oIndictment.IDScheduleRule = oCastRule.IDRule
                        oIndictment.RuleName = oCastRule.RuleName
                        Me._oState.Language.ClearUserTokens()
                        Me._oState.Language.AddUserToken(lTotalFreeWeekends.ToString)
                        Me._oState.Language.AddUserToken(oCastRule.Minimum.ToString)
                        Me._oState.Language.AddUserToken(oIndictment.DateBegin.ToShortDateString)
                        Me._oState.Language.AddUserToken(oIndictment.DateEnd.ToShortDateString)
                        If lTotalFreeWeekends = 1 Then
                            oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinWeekendsInPeriod.One", "")
                        Else
                            oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinWeekendsInPeriod.MoreThanOne", "")
                        End If

                        If oCastRule.IdContract <> "" Then
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                        Else
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                        End If
                        oRet.Add(oIndictment)
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_2ShiftSequenceManager::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

    End Class

    Public Class ScheduleRule_2ShiftSequenceManager  'QC OK
        Inherits ScheduleRuleManager
        Private oEmpData As roEmployeeRulesData
        Private dIndictmentDates As New List(Of Date)

        Public Sub New(ByVal oRule As roScheduleRule_2ShiftSequence, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.TwoShiftSequence
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = 2
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_2ShiftSequenceManager::Applies")
                Return Nothing
            End Try
            Return bolRet
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment
            Dim bIndictmentFound As Boolean = False
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData)
            Dim iCurrentDay As Integer = 0

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1

                If oShiftsQ.Count > 1 Then
                    ' Sólo si está planificado alguno de los horarios indicados en la regla

                    Dim oCastRule = CType(Me.Parameters, roScheduleRule_2ShiftSequence)
                    If Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oCastRule.CurrentDayShifts.ToList().Contains(oShiftsQ(iCurrentDay).MainShift.ID) Then
                        ' Serie (si el día es correcto)
                        Select Case oCastRule.Type
                            Case ShiftSequenceType.Unwanted
                                bIndictmentFound = (Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oCastRule.CurrentDayShifts.Contains(oShiftsQ(iCurrentDay).MainShift.ID)) AndAlso (Not oShiftsQ(iCurrentDay - 1).MainShift Is Nothing AndAlso oCastRule.PreviousDayShifts.Contains(oShiftsQ(iCurrentDay - 1).MainShift.ID))
                            Case ShiftSequenceType.Wanted
                                bIndictmentFound = (Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oCastRule.CurrentDayShifts.Contains(oShiftsQ(iCurrentDay).MainShift.ID)) AndAlso (Not oShiftsQ(iCurrentDay - 1).MainShift Is Nothing AndAlso Not oCastRule.PreviousDayShifts.Contains(oShiftsQ(iCurrentDay - 1).MainShift.ID))
                        End Select

                        If bIndictmentFound Then
                            oIndictment = New roCalendarScheduleIndictment
                            oIndictment.DateBegin = oShiftsQ(iCurrentDay - 1).PlanDate
                            oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                            dIndictmentDates.Add(oIndictment.DateBegin)
                            dIndictmentDates.Add(oIndictment.DateEnd)
                            oIndictment.Dates = dIndictmentDates.ToArray
                            dIndictmentDates.Clear()
                            oIndictment.IDEmployee = oemp.IDEmployee
                            oIndictment.IDScheduleRule = oCastRule.IDRule
                            oIndictment.RuleName = oCastRule.RuleName
                            Me._oState.Language.ClearUserTokens()
                            Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).PlanDate.ToShortDateString)
                            Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay - 1).MainShift.Name)
                            Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).PlanDate.ToShortDateString)
                            Me._oState.Language.AddUserToken(oShiftsQ(iCurrentDay).MainShift.Name)
                            oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.2ShiftSequence.Sequence", "")
                            If oCastRule.IdContract <> "" Then
                                oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                            Else
                                oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                            End If
                            oRet.Add(oIndictment)
                        End If
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_2ShiftSequenceManager::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

    End Class

    Public Class ScheduleRule_MinMax2ShiftSequenceOnEmployeeManager
        Inherits ScheduleRuleManager

        Public Sub New(ByVal oRule As roScheduleRule_MinMax2ShiftSequenceOnEmployee, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MinMax2ShiftSequenceOnEmployee
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = 2
        End Sub

        Public Overrides Function Applies(empobject As Object, dayobject As Object) As Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Function Check(dayobject As Object, oemp As roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Throw New NotImplementedException()
        End Function

    End Class

    Public Class ScheduleRule_MaxHolidaysManager  'QC OK
        Inherits ScheduleRuleManager
        Private oEmpData As roEmployeeRulesData
        Protected lTotalDays As Long
        Private dIndictmentDates As New List(Of Date)
        Private bCheckDay As Boolean = False

        Public Sub New(ByVal oRule As roScheduleRule_MaxHolidays, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MaxHolidays
            Me.Parameters.RuleType = ScheduleRuleBaseType.System
            Me.Parameters.DayDepth = 2
            Me.Parameters.RuleName = Me._oState.Language.Translate("CalendarScheduleRules.MaxHolidays.RuleName", "")
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MaxHolidaysManager::Applies")
                Return True
            End Try
            Return True
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim oShiftsQ As New List(Of roCalendarRowDayData)
            Dim iCurrentDay As Integer = 0

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1
                Dim oCastRule = CType(Me.Parameters, roScheduleRule_MaxHolidays)
                If bCheckDay AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing Then
                    ' Si estoy en el mismo año natural que el primer día del periodo mostrado
                    If oShiftsQ(iCurrentDay).PlanDate <= dyearfirstdate.AddYears(1).AddDays(-1) Then
                        lTotalDays = lTotalDays + If(oShiftsQ(iCurrentDay).IsHoliday, 1, 0)
                        If oShiftsQ(iCurrentDay).IsHoliday AndAlso (oShiftsQ(iCurrentDay).PlanDate >= datestart AndAlso oShiftsQ(iCurrentDay).PlanDate <= dateend) Then
                            dIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        End If
                    End If
                End If

                If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                    ' En función del origen de la regla
                    If oCastRule.IdContract = "" Then
                        ' Viene de convenio
                        If oEmpData.PlannedHolidays.Exists(Function(x) x.IdLabAgree = oCastRule.IdLabAgree AndAlso x.IdEmployee = oemp.IDEmployee) Then
                            lTotalDays = lTotalDays + oEmpData.PlannedHolidays.Find(Function(x) x.IdLabAgree = oCastRule.IdLabAgree AndAlso x.IdEmployee = oemp.IDEmployee).Value
                        End If
                    Else
                        ' Viene de contrato
                        If oEmpData.PlannedHolidays.Exists(Function(x) x.IdContract = oCastRule.IdContract AndAlso x.IdEmployee = oemp.IDEmployee) Then
                            lTotalDays = lTotalDays + oEmpData.PlannedHolidays.Find(Function(x) x.IdContract = oCastRule.IdContract AndAlso x.IdEmployee = oemp.IDEmployee).Value
                        End If
                    End If

                    If lTotalDays > oCastRule.MaximumHoliDays Then
                        oIndictment = New roCalendarScheduleIndictment
                        oIndictment.DateBegin = datestart
                        oIndictment.DateEnd = dateend
                        oIndictment.Dates = dIndictmentDates.ToArray
                        oIndictment.IDEmployee = oemp.IDEmployee
                        oIndictment.IDScheduleRule = oCastRule.IDRule
                        oIndictment.RuleName = oCastRule.RuleName
                        Me._oState.Language.ClearUserTokens()
                        Me._oState.Language.AddUserToken(lTotalDays.ToString)
                        Me._oState.Language.AddUserToken(oCastRule.MaximumHoliDays.ToString)
                        Me._oState.Language.AddUserToken(dyearfirstdate.ToShortDateString)
                        Me._oState.Language.AddUserToken(dyearfirstdate.AddYears(1).AddDays(-1).ToShortDateString)
                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MaxHolidays", "")
                        If oCastRule.IdContract <> "" Then
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                        Else
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                        End If
                        oRet.Add(oIndictment)
                    End If

                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MaxHolidaysManager::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

    End Class

    Public Class ScheduleRule_MaxNotScheduledManager
        Inherits ScheduleRuleManager
        Private oEmpData As roEmployeeRulesData
        Protected lTotalDays As Long
        Private dIndictmentDates As New List(Of Date)
        Private bCheckDay As Boolean = False

        Public Sub New(ByVal oRule As roScheduleRule_MaxNotScheduled, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MaxNotScheduled
            Me.Parameters.RuleType = ScheduleRuleBaseType.System
            Me.Parameters.DayDepth = 2
            Me.Parameters.RuleName = Me._oState.Language.Translate("CalendarScheduleRules.MaxNotScheduled.RuleName", "")
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MaxNotScheduledManager::Applies")
                Return True
            End Try
            Return True
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim oShiftsQ As New List(Of roCalendarRowDayData)
            Dim iCurrentDay As Integer = 0

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1
                Dim oCastRule = CType(Me.Parameters, roScheduleRule_MaxNotScheduled)
                If bCheckDay AndAlso (oShiftsQ(iCurrentDay).MainShift Is Nothing OrElse oShiftsQ(iCurrentDay).MainShift.ID = 0) Then
                    ' Si estoy en el mismo año natural que el primer día del periodo mostrado
                    If oShiftsQ(iCurrentDay).PlanDate <= dyearfirstdate.AddYears(1).AddDays(-1) Then
                        lTotalDays = lTotalDays + If(oShiftsQ(iCurrentDay).MainShift Is Nothing OrElse oShiftsQ(iCurrentDay).MainShift.ID = 0, 1, 0)
                        If (oShiftsQ(iCurrentDay).MainShift Is Nothing OrElse oShiftsQ(iCurrentDay).MainShift.ID = 0) AndAlso (oShiftsQ(iCurrentDay).PlanDate >= datestart AndAlso oShiftsQ(iCurrentDay).PlanDate <= dateend) Then
                            dIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                        End If
                    End If
                End If

                If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                    ' En función del origen de la regla
                    If oCastRule.IdContract = "" Then
                        ' Viene de convenio
                        If oEmpData.NotScheduledDays.Exists(Function(x) x.IdLabAgree = oCastRule.IdLabAgree AndAlso x.IdEmployee = oemp.IDEmployee) Then
                            lTotalDays = lTotalDays + oEmpData.NotScheduledDays.Find(Function(x) x.IdLabAgree = oCastRule.IdLabAgree AndAlso x.IdEmployee = oemp.IDEmployee).Value
                        End If
                    Else
                        ' Viene de contrato
                        If oEmpData.NotScheduledDays.Exists(Function(x) x.IdContract = oCastRule.IdContract AndAlso x.IdEmployee = oemp.IDEmployee) Then
                            lTotalDays = lTotalDays + oEmpData.NotScheduledDays.Find(Function(x) x.IdContract = oCastRule.IdContract AndAlso x.IdEmployee = oemp.IDEmployee).Value
                        End If
                    End If

                    If lTotalDays > oCastRule.MaximumNotScheduledDays Then
                        oIndictment = New roCalendarScheduleIndictment
                        oIndictment.DateBegin = datestart
                        oIndictment.DateEnd = dateend
                        oIndictment.Dates = dIndictmentDates.ToArray
                        oIndictment.IDEmployee = oemp.IDEmployee
                        oIndictment.IDScheduleRule = oCastRule.IDRule
                        oIndictment.RuleName = oCastRule.RuleName
                        Me._oState.Language.ClearUserTokens()
                        Me._oState.Language.AddUserToken(dyearfirstdate.Year.ToString)
                        oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MaxNotScheduledDays", "")
                        If oCastRule.IdContract <> "" Then
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                        Else
                            oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                        End If
                        oRet.Add(oIndictment)
                    End If

                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MaxNotScheduledDays::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

    End Class

    Public Class ScheduleRule_WorkOnFestiveManager 'QC OK
        Inherits ScheduleRuleManager
        Private oEmpData As roEmployeeRulesData
        Private dIndictmentDates As New List(Of Date)

        Public Sub New(ByVal oRule As roScheduleRule_WorkOnFestive, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.WorkOnFestive
            Me.Parameters.RuleType = ScheduleRuleBaseType.System
            Me.Parameters.DayDepth = 2
            Me.Parameters.RuleName = Me._oState.Language.Translate("CalendarScheduleRules.WorkOnFestive.RuleName", "")
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_WorkOnFestiveManager::Applies")
                Return Nothing
            End Try
            Return bolRet
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim oShiftsQ As New List(Of roCalendarRowDayData)
            Dim iCurrentDay As Integer = 0

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1
                Dim oCastRule = CType(Me.Parameters, roScheduleRule_WorkOnFestive)
                If oShiftsQ(iCurrentDay).Feast AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oShiftsQ(iCurrentDay).MainShift.PlannedHours > 0 Then
                    oIndictment = New roCalendarScheduleIndictment
                    oIndictment.DateBegin = oShiftsQ(iCurrentDay).PlanDate
                    oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                    dIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                    oIndictment.Dates = dIndictmentDates.ToArray
                    oIndictment.IDEmployee = oemp.IDEmployee
                    oIndictment.IDScheduleRule = oCastRule.IDRule
                    oIndictment.RuleName = oCastRule.RuleName
                    Me._oState.Language.ClearUserTokens()
                    Me._oState.Language.AddUserToken(oIndictment.DateEnd.ToShortDateString)
                    oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.WorkOnFestive", "")
                    If oCastRule.IdContract <> "" Then
                        oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                    Else
                        oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                    End If
                    oRet.Add(oIndictment)
                    dIndictmentDates.Clear()
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_WorkOnFestiveManager::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

    End Class

    Public Class ScheduleRule_WorkOnWeekendManager 'QC OK
        Inherits ScheduleRuleManager
        Private oEmpData As roEmployeeRulesData
        Protected lTotalDays As Long
        Private dIndictmentDates As New List(Of Date)

        Public Sub New(ByVal oRule As roScheduleRule_WorkOnWeekend, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.WorkOnWeekend
            Me.Parameters.RuleType = ScheduleRuleBaseType.System
            Me.Parameters.DayDepth = 2
            Me.Parameters.RuleName = Me._oState.Language.Translate("CalendarScheduleRules.WorkOnWeekend.RuleName", "")
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)
            Dim oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData) = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
            Dim bolRet As Boolean = False

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bolRet = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_WorkOnWeekendManager::Applies")
                Return Nothing
            End Try
            Return bolRet
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim oShiftsQ As New List(Of roCalendarRowDayData)
            Dim iCurrentDay As Integer = 0

            Try
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1
                Dim oCastRule = CType(Me.Parameters, roScheduleRule_WorkOnWeekend)

                If Not oShiftsQ(iCurrentDay).MainShift Is Nothing AndAlso oShiftsQ(iCurrentDay).MainShift.PlannedHours > 0 AndAlso Not oCastRule.LabourDaysIndex.Split(",").Contains(oShiftsQ(iCurrentDay).PlanDate.DayOfWeek) Then
                    oIndictment = New roCalendarScheduleIndictment
                    oIndictment.DateBegin = oShiftsQ(iCurrentDay).PlanDate
                    oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                    dIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)
                    oIndictment.Dates = dIndictmentDates.ToArray
                    oIndictment.IDEmployee = oemp.IDEmployee
                    oIndictment.IDScheduleRule = oCastRule.IDRule
                    oIndictment.RuleName = oCastRule.RuleName
                    Me._oState.Language.ClearUserTokens()
                    Me._oState.Language.AddUserToken(oIndictment.DateEnd.ToShortDateString)
                    oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.WorkOnWeekend", "")
                    If oCastRule.IdContract <> "" Then
                        oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                    Else
                        oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                    End If
                    oRet.Add(oIndictment)
                    dIndictmentDates.Clear()
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_WorkOnWeekendManager::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

    End Class

    Public Class ScheduleRule_MinMaxExpectedHoursInPeriodManager 'QCOK
        Inherits ScheduleRuleManager

        Private oEmpData As roEmployeeRulesData
        Protected Property dLastPeriodBegin As Date
        Protected Property dLastPeriodEnd As Date
        Protected iTotalHoursInPeriod As Double = 0
        Protected dIndictmentDatesForMinimumRule As New List(Of Date)
        Protected dIndictmentDatesForMaximumRule As New List(Of Date)
        Protected dPeriodDateStart As Date
        Protected oCastRule As roScheduleRule_MinMaxExpectedHoursInPeriod
        Protected iIDEmployee As Integer = 0
        Protected iCurrentDay As Integer = 0
        Protected oShiftsQ As List(Of roCalendarRowDayData)
        Private bCheckDay As Boolean = False

        Public Sub New(ByVal oRule As roScheduleRule_MinMaxExpectedHoursInPeriod, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MinMaxExpectedHoursInPeriod
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = 2
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            oEmpData = CType(empobject, roEmployeeRulesData)

            Try
                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bCheckDay = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxExpectedHoursInPeriod::Applies")
                Return True
            End Try
            Return True
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim bIndictmentFound As Boolean = False
            Dim bPeriodChange As Boolean = False

            Try
                iCurrentDay = 0
                dPeriodDateStart = datestart
                oShiftsQ = New List(Of roCalendarRowDayData)
                oShiftsQ = CType(oShiftsQObject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))
                iCurrentDay = oShiftsQ.Count - 1

                oCastRule = CType(Me.Parameters, roScheduleRule_MinMaxExpectedHoursInPeriod)

                iIDEmployee = oemp.IDEmployee

                If oShiftsQ.Count = 1 Then
                    Dim iMaxWorkingHours As Integer = 0
                    If oCastRule.MaximumEmployeeField.Trim.Length > 0 Then
                        Dim vEmployeeField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oemp.IDEmployee, oCastRule.MaximumEmployeeField, datestart, New UserFields.roUserFieldState(Me._oState.IDPassport), False)
                        If Not vEmployeeField Is Nothing Then
                            'OJO. Si no se informa el campo, siempre dará alerta.
                            oCastRule.MaximumWorkingHours = vEmployeeField.FieldValue
                        End If
                    End If
                    ' Primer día del periodo.
                    If bCheckDay AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing Then

                        If oShiftsQ(iCurrentDay).PlanDate <= dyearfirstdate.AddYears(1).AddDays(-1) Then
                            iTotalHoursInPeriod = iTotalHoursInPeriod + oShiftsQ(iCurrentDay).MainShift.PlannedHours / 60
                            If oShiftsQ(iCurrentDay).MainShift.PlannedHours / 60 > 0 AndAlso (oShiftsQ(iCurrentDay).PlanDate >= datestart AndAlso oShiftsQ(iCurrentDay).PlanDate <= dateend) Then
                                ' Los días con horas teóricas pueden colaborar a generar un impacto por exeder el maximo ...
                                dIndictmentDatesForMaximumRule.Add(oShiftsQ(iCurrentDay).PlanDate)
                            End If
                        End If
                    End If
                    ' Todos los días pueden colaborar a generar un impacto por no llegar al mínimo ...
                    dIndictmentDatesForMinimumRule.Add(oShiftsQ(iCurrentDay).PlanDate)

                    ' Si estamos en el último día evaluo el resultado
                    If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                        dLastPeriodEnd = oShiftsQ(iCurrentDay).PlanDate
                        If oCastRule.MaximumWorkingHours > -1 AndAlso iTotalHoursInPeriod > oCastRule.MaximumWorkingHours Then
                            oIndictment = CreateMaxIndictment()
                            oRet.Add(oIndictment)
                        End If
                        If oCastRule.MinimumWorkingHours > -1 AndAlso iTotalHoursInPeriod < oCastRule.MinimumWorkingHours Then
                            oIndictment = CreateMinIndictment()
                            oRet.Add(oIndictment)
                        End If

                        ' Reseteo el contador para cuando se evalue la regla con otro empleado
                        iTotalHoursInPeriod = 0
                        dIndictmentDatesForMinimumRule = New List(Of Date)
                        dIndictmentDatesForMaximumRule = New List(Of Date)
                    Else
                        dLastPeriodBegin = oShiftsQ(iCurrentDay).PlanDate
                    End If
                Else
                    ' Miro si ha habido cambio de ciclo en función del tipo
                    Select Case oCastRule.Scope
                        Case ScheduleRuleScope.Month
                            bPeriodChange = oShiftsQ(iCurrentDay - 1).PlanDate.Month <> oShiftsQ(iCurrentDay).PlanDate.Month
                        Case ScheduleRuleScope.Year
                            bPeriodChange = oShiftsQ(iCurrentDay - 1).PlanDate.Year <> oShiftsQ(iCurrentDay).PlanDate.Year
                        Case ScheduleRuleScope.Week
                            bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.DayOfWeek = 1)
                        Case ScheduleRuleScope.Day
                            bPeriodChange = True
                    End Select

                    ' Si hubo cambio de periodo, evaluo ahora
                    If bPeriodChange Then
                        dLastPeriodEnd = oShiftsQ(iCurrentDay - 1).PlanDate
                        If oCastRule.MaximumWorkingHours > -1 AndAlso iTotalHoursInPeriod > oCastRule.MaximumWorkingHours Then
                            oIndictment = CreateMaxIndictment()
                            oIndictment.DateEnd = oShiftsQ(iCurrentDay - 1).PlanDate
                            oRet.Add(oIndictment)
                        End If
                        If oCastRule.MinimumWorkingHours > -1 AndAlso iTotalHoursInPeriod < oCastRule.MinimumWorkingHours Then
                            oIndictment = CreateMinIndictment()
                            oIndictment.DateEnd = oShiftsQ(iCurrentDay - 1).PlanDate
                            oRet.Add(oIndictment)
                        End If

                        dLastPeriodBegin = oShiftsQ(iCurrentDay).PlanDate

                        ' Reseteo el contador para cuando se evalue la regla con otro empleado
                        iTotalHoursInPeriod = 0
                        dIndictmentDatesForMinimumRule = New List(Of Date)
                        dIndictmentDatesForMaximumRule = New List(Of Date)
                    End If

                    If bCheckDay AndAlso Not oShiftsQ(iCurrentDay).MainShift Is Nothing Then
                        If oShiftsQ(iCurrentDay).PlanDate <= dyearfirstdate.AddYears(1).AddDays(-1) Then
                            iTotalHoursInPeriod = iTotalHoursInPeriod + oShiftsQ(iCurrentDay).MainShift.PlannedHours / 60
                            If oShiftsQ(iCurrentDay).MainShift.PlannedHours / 60 > 0 AndAlso (oShiftsQ(iCurrentDay).PlanDate >= datestart AndAlso oShiftsQ(iCurrentDay).PlanDate <= dateend) Then
                                ' Los días con horas teóricas pueden colaborar a generar un impacto por exeder el maximo ...
                                dIndictmentDatesForMaximumRule.Add(oShiftsQ(iCurrentDay).PlanDate)
                            End If
                        End If
                    End If
                    ' Todos los días pueden colaborar a generar un impacto por no llegar al mínimo ...
                    dIndictmentDatesForMinimumRule.Add(oShiftsQ(iCurrentDay).PlanDate)

                    ' Si estamos en el último día evaluo el resultado
                    If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                        dLastPeriodEnd = oShiftsQ(iCurrentDay).PlanDate
                        If oCastRule.MaximumWorkingHours > -1 AndAlso iTotalHoursInPeriod > oCastRule.MaximumWorkingHours Then
                            oIndictment = CreateMaxIndictment()
                            oRet.Add(oIndictment)
                        End If
                        If oCastRule.MinimumWorkingHours > -1 AndAlso iTotalHoursInPeriod < oCastRule.MinimumWorkingHours Then
                            oIndictment = CreateMinIndictment()
                            oRet.Add(oIndictment)
                        End If

                        dLastPeriodBegin = oShiftsQ(iCurrentDay).PlanDate

                        ' Reseteo el contador para cuando se evalue la regla con otro empleado
                        iTotalHoursInPeriod = 0
                        dIndictmentDatesForMinimumRule = New List(Of Date)
                        dIndictmentDatesForMaximumRule = New List(Of Date)
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxExpectedHoursInPeriod::Check")
                Return Nothing
            End Try
            Return oRet
        End Function

        Private Function CreateMaxIndictment() As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = IIf(dLastPeriodBegin >= dPeriodDateStart, dLastPeriodBegin, dPeriodDateStart)
                oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                oIndictment.Dates = dIndictmentDatesForMaximumRule.ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(roConversions.ConvertHoursToTime(iTotalHoursInPeriod))
                Me._oState.Language.AddUserToken(roConversions.ConvertHoursToTime(oCastRule.MaximumWorkingHours))
                Me._oState.Language.AddUserToken(dLastPeriodBegin.ToShortDateString)
                Me._oState.Language.AddUserToken(dLastPeriodEnd.ToShortDateString)
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MaxExpectedHoursInPeriod", "")
                If oCastRule.IdContract <> "" Then
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                Else
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxExpectedHoursInPeriod::CreateMaxIndictment")
                Return Nothing
            End Try
            Return oIndictment
        End Function

        Private Function CreateMinIndictment() As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = IIf(dLastPeriodBegin >= dPeriodDateStart, dLastPeriodBegin, dPeriodDateStart)
                oIndictment.DateEnd = oShiftsQ(iCurrentDay).PlanDate
                oIndictment.Dates = dIndictmentDatesForMinimumRule.ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(roConversions.ConvertHoursToTime(iTotalHoursInPeriod))
                Me._oState.Language.AddUserToken(roConversions.ConvertHoursToTime(oCastRule.MinimumWorkingHours))
                Me._oState.Language.AddUserToken(dLastPeriodBegin.ToShortDateString)
                Me._oState.Language.AddUserToken(dLastPeriodEnd.ToShortDateString)
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinExpectedHoursInPeriod", "")
                If oCastRule.IdContract <> "" Then
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.Contract", "")
                Else
                    oIndictment.ErrorText &= " " & Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", "")
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxExpectedHoursInPeriod::CreateMaxIndictment")
                Return Nothing
            End Try
            Return oIndictment
        End Function

    End Class

    Public Class ScheduleRule_MinMaxShiftsSequenceManager
        Inherits ScheduleRuleManager

        Private dLastSequenceEnd As Date = Date.MinValue
        Private dLastPeriodBegin As Date = Date.MinValue
        Private dLastPeriodEnd As Date = Date.MinValue
        Private oEmpData As roEmployeeRulesData
        Private lIndictmentDates As New List(Of Date)
        Private oIndictment As roCalendarScheduleIndictment = Nothing
        Private oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData)
        Private oCastRule As roScheduleRule_MinMaxShiftsSequence
        Private bCheckRule As Boolean = False
        Private iTotalSequencesFound As Integer = 0
        Private bSequenceFound As Boolean = False
        Private bPeriodChange As Boolean = False
        Private iIDEmployee As Integer = 0
        Private iCurrentDay As Integer = 0
        Private iFeastDaysInSequence As Integer = 0
        ' Bricomart - Personalizaciones
        Private oCalManager As roCalendarManager = Nothing
        Private oCalRuleState As roCalendarScheduleRulesState = Nothing
        Private oCalState As roCalendarState = Nothing
        Private oCalendarCompact As List(Of roCalendarRow) = Nothing
        Private oCalRulesManager As roCalendarScheduleRulesManager = Nothing
        Private oAuxCalendar As roCalendar = Nothing

        Public Sub New(ByVal oRule As roScheduleRule_MinMaxShiftsSequence, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.oCastRule = oRule
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.MinMaxShiftsSequence
            Me.Parameters.RuleType = ScheduleRuleBaseType.User
            Me.Parameters.DayDepth = oCastRule.Shifts.Count
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            Dim bolRet As Boolean = False

            Try
                oEmpData = CType(empobject, roEmployeeRulesData)
                oShiftsQ = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))

                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bCheckRule = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bCheckRule = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If

                If bCheckRule Then
                    ' No tengo días suficientes para la secuencia. no hace falta evaluar
                    bCheckRule = (oShiftsQ.Count = Me.Parameters.DayDepth)
                End If

                ' Una vez se localiza una secuencia, no hace falta checkear nada hasta que el array empiece por la siguiente fecha
                If bCheckRule AndAlso bSequenceFound Then
                    If oShiftsQ(0).PlanDate <= dLastSequenceEnd Then
                        bCheckRule = False
                    Else
                        ' Envío siguiente secuencia a evaluar
                        bSequenceFound = False
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxDaysSequenceManager::Applies")
                Return True
            End Try
            Return True
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim bIndictmentFound As Boolean = False

            Try
                iCurrentDay = oShiftsQ.Count - 1
                iIDEmployee = oemp.IDEmployee

                ' Añado todas las fechas a la lista de candidatos a impacto. Si encuentro una secuencia, quitaré sus fechas.
                lIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)

                If bCheckRule Then
                    ' Miro si los días cumplen la definición de la secuencia

                    For i = 1 To oCastRule.Shifts.Count
                        ' BricoMart - Personalizaciones: Si la secuencia incluye festivos, hay que añadir tantos FR como festivos a continuación de la secuencia.
                        If (Not oShiftsQ(i - 1).MainShift Is Nothing AndAlso oShiftsQ(i - 1).Feast) Then iFeastDaysInSequence += 1

                        If Not IsNumeric(oCastRule.Shifts(i - 1)) Then
                            ' Es el tag avanzado SCH del horario
                            Dim sParName As String = "SCHTYPE" 'oCastRule.Shifts(i - 1).Split({"{*}"}, StringSplitOptions.None)(0)
                            If oCastRule.Shifts(i - 1).ToUpper.StartsWith(sParName) Then
                                Dim sParValue As String = oCastRule.Shifts(i - 1).ToUpper.Replace(sParName & "_", "")
                                If Not oShiftsQ(i - 1).MainShift Is Nothing AndAlso Not oShiftsQ(i - 1).MainShift.AdvancedParameters Is Nothing AndAlso Not oShiftsQ(i - 1).MainShift.AdvancedParameters.ToList.Find(Function(x) x.Name.ToUpper = sParName) Is Nothing AndAlso oShiftsQ(i - 1).MainShift.AdvancedParameters.ToList.Find(Function(x) x.Name.ToUpper = sParName).Value = oCastRule.Shifts(i - 1).Substring(sParName.Length + 1) Then
                                    ' Vamos bien
                                    bSequenceFound = True
                                Else
                                    bSequenceFound = False
                                    Exit For
                                End If
                            Else
                                ' Secuencia rota
                                bSequenceFound = False
                                Exit For
                            End If
                        Else
                            ' Es un id de horario
                            If Not oShiftsQ(i - 1).MainShift Is Nothing AndAlso oShiftsQ(i - 1).MainShift.ID = oCastRule.Shifts(i - 1) Then
                                ' Vamos bien
                                bSequenceFound = True
                            Else
                                ' Secuencia rota
                                bSequenceFound = False
                                Exit For
                            End If
                        End If
                    Next

                    If bSequenceFound Then
                        iTotalSequencesFound += 1
                        dLastSequenceEnd = oShiftsQ(iCurrentDay).PlanDate
                        ' Quito fechas de la lista de impactos
                        lIndictmentDates.RemoveAll(Function(x) oShiftsQ.Select(Function(y) y.PlanDate).ToArray.Contains(x))

                        ' BricoMart - Personalizaciones
                        ' 1.- Si en el periodo hay festivos nacionales, la secuencia debe acabar en FR
                        If oCastRule.RuleDescription.ToUpper.Contains("COMPENSAFESTIVOSNACIONALES=[SI]") AndAlso iFeastDaysInSequence > 0 Then
                            ' Reviso los próximos iFeastDaysInSequence días del empleado, para revisar si son FR
                            Dim dStartCompensation As Date = oShiftsQ(iCurrentDay).PlanDate.AddDays(1)
                            Dim dEndCompensation As Date = oShiftsQ(iCurrentDay).PlanDate.AddDays(iFeastDaysInSequence)

                            Dim oCalendarRowDayDataAux As New List(Of roCalendarRowDayData)

                            If MyBase.WorkDayData(MyBase.WorkDayData.Count - 1).PlanDate < dEndCompensation Then
                                ' Si en la planificación que estoy examinando no se incluyen suficientes días, los cargo de BBDD
                                If oCalRuleState Is Nothing Then oCalRuleState = New roCalendarScheduleRulesState(Me.State.IDPassport)
                                If oCalState Is Nothing Then oCalState = New roCalendarState(Me.State.IDPassport)
                                If oCalManager Is Nothing Then oCalManager = New roCalendarManager(oCalState)
                                If oCalRulesManager Is Nothing Then oCalRulesManager = New roCalendarScheduleRulesManager(oCalRuleState)

                                oCalendarCompact = New List(Of roCalendarRow)
                                oAuxCalendar = oCalManager.Load(MyBase.WorkDayData(MyBase.WorkDayData.Count - 1).PlanDate.AddDays(1), dEndCompensation, "B" & oemp.IDEmployee.ToString, DTOs.CalendarView.Planification, DTOs.CalendarDetailLevel.Daily, True)
                                oCalendarCompact = oCalRulesManager.CompactCalendar(oAuxCalendar)
                                oCalendarRowDayDataAux = MyBase.WorkDayData
                                oCalendarRowDayDataAux.AddRange(oCalendarCompact(0).PeriodData.DayData.ToList)
                            End If

                            oCalendarRowDayDataAux = MyBase.WorkDayData.FindAll(Function(x) x.PlanDate >= oShiftsQ(iCurrentDay).PlanDate.AddDays(1) AndAlso x.PlanDate <= oShiftsQ(iCurrentDay).PlanDate.AddDays(iFeastDaysInSequence))

                            If oCalendarRowDayDataAux.Count = iFeastDaysInSequence Then
                                ' Sólo habrá una fila
                                If oCalendarRowDayDataAux.FindAll(Function(x) Not x.MainShift Is Nothing AndAlso x.MainShift.ShortName = "FR").Count <> iFeastDaysInSequence Then
                                    oIndictment = CreateSequenceCompensationIndictment("FR", iFeastDaysInSequence)
                                    oRet.Add(oIndictment)
                                End If
                            End If

                        End If
                        iFeastDaysInSequence = 0

                        ' 2.- La secuencia debe empezar en lunes
                        If oCastRule.RuleDescription.ToUpper.Contains("INICIAENLUNES=[SI]") Then
                            If Not oShiftsQ(0).PlanDate.DayOfWeek = DayOfWeek.Monday Then
                                oIndictment = CreateSequenceStartDayIndictment("Lunes")
                                oRet.Add(oIndictment)
                            End If
                        End If
                    End If
                End If

                ' Miro si ha habido cambio de ciclo en función del tipo
                Dim iPeriodPercentage As Double = 1
                If oShiftsQ.Count > 1 Then
                    Select Case oCastRule.Scope
                        Case ScheduleRuleScope.Month
                            bPeriodChange = oShiftsQ(iCurrentDay - 1).PlanDate.Month <> oShiftsQ(iCurrentDay).PlanDate.Month
                            If bPeriodChange AndAlso oCastRule.ScopePeriods > 1 Then
                                bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.Date.Subtract(dLastPeriodBegin).Days) >= 28 * oCastRule.ScopePeriods
                            End If
                            iPeriodPercentage = (DateDiff(DateInterval.Day, oShiftsQ(iCurrentDay).PlanDate, dLastPeriodBegin)) / (28 * oCastRule.ScopePeriods)
                        Case ScheduleRuleScope.Year
                            bPeriodChange = oShiftsQ(iCurrentDay - 1).PlanDate.Year <> oShiftsQ(iCurrentDay).PlanDate.Year
                            If bPeriodChange AndAlso oCastRule.ScopePeriods > 1 Then
                                bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.Date.Subtract(dLastPeriodBegin).Days) >= 365 * oCastRule.ScopePeriods
                            End If
                            iPeriodPercentage = (DateDiff(DateInterval.Day, oShiftsQ(iCurrentDay).PlanDate, dLastPeriodBegin)) / (365 * oCastRule.ScopePeriods)
                        Case ScheduleRuleScope.Week
                            bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.DayOfWeek = 1)
                            If bPeriodChange AndAlso oCastRule.ScopePeriods > 1 Then
                                bPeriodChange = (oShiftsQ(iCurrentDay).PlanDate.Date.Subtract(dLastPeriodBegin).Days) >= 7 * oCastRule.ScopePeriods
                            End If
                            iPeriodPercentage = (DateDiff(DateInterval.Day, dLastPeriodBegin, oShiftsQ(iCurrentDay).PlanDate) + 1) / (7 * oCastRule.ScopePeriods)
                    End Select
                Else
                    dLastPeriodBegin = oShiftsQ(iCurrentDay).PlanDate
                End If

                ' Si se acaba el periodo, o hay cambio de periodo
                If bPeriodChange OrElse oShiftsQ(iCurrentDay).PlanDate = dateend Then
                    If bPeriodChange Then
                        dLastPeriodEnd = oShiftsQ(iCurrentDay - 1).PlanDate
                    Else
                        dLastPeriodEnd = oShiftsQ(iCurrentDay).PlanDate
                    End If

                    ' Si acabó el periodo a evaluar, pero no hay cambio de periodo, prorrateo para evitar falsas alertas
                    Dim iMax As Integer = oCastRule.Maximum
                    Dim iMin As Integer = oCastRule.Minimum
                    If Not bPeriodChange Then
                        iMax = CType((iMax * iPeriodPercentage), Integer)
                        iMin = CType((iMin * iPeriodPercentage), Integer)
                    End If

                    If iTotalSequencesFound < iMin Then
                        oIndictment = CreateMinIndictment()
                        oRet.Add(oIndictment)
                    End If

                    If iTotalSequencesFound > iMax Then
                        oIndictment = CreateMaxIndictment()
                        oRet.Add(oIndictment)
                    End If

                    ' Si hubo cambio de periodo reseteo
                    If bPeriodChange Then
                        lIndictmentDates.Clear()
                        iTotalSequencesFound = 0
                        dLastSequenceEnd = oShiftsQ(iCurrentDay - 1).PlanDate
                        dLastPeriodBegin = oShiftsQ(iCurrentDay).PlanDate
                    End If
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxDaysSequenceManager::Check")
                Return Nothing
            End Try
            Return oRet

        End Function

        Private Function CreateMaxIndictment() As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = dLastPeriodBegin
                oIndictment.DateEnd = dLastPeriodEnd
                oIndictment.Dates = lIndictmentDates.ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(iTotalSequencesFound)
                Me._oState.Language.AddUserToken(oIndictment.DateBegin)
                Me._oState.Language.AddUserToken(oIndictment.DateEnd)
                Me._oState.Language.AddUserToken(oCastRule.Maximum.ToString)
                Me._oState.Language.AddUserToken(oIndictment.RuleName)
                If oCastRule.IdContract <> "" Then
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                Else
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                End If
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MaxShiftsSequence", "")
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxShiftsSequence::CreateMaxIndictment")
                Return Nothing
            End Try
            Return oIndictment
        End Function

        Private Function CreateMinIndictment() As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = dLastPeriodBegin
                oIndictment.DateEnd = dLastPeriodEnd
                oIndictment.Dates = lIndictmentDates.ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(iTotalSequencesFound)
                Me._oState.Language.AddUserToken(oIndictment.DateBegin)
                Me._oState.Language.AddUserToken(oIndictment.DateEnd)
                Me._oState.Language.AddUserToken(oCastRule.Minimum.ToString)
                Me._oState.Language.AddUserToken(oIndictment.RuleName)
                If oCastRule.IdContract <> "" Then
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                Else
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                End If
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MinShiftsSequence", "")
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxShiftsSequence::CreateMinIndictment")
                Return Nothing
            End Try
            Return oIndictment
        End Function

        Private Function CreateSequenceStartDayIndictment(sDia As String) As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = oShiftsQ(0).PlanDate
                oIndictment.DateEnd = oShiftsQ(oShiftsQ.Count - 1).PlanDate
                oIndictment.Dates = oShiftsQ.Select(Function(x) x.PlanDate).ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(oIndictment.DateBegin)
                Me._oState.Language.AddUserToken(oIndictment.DateEnd)
                Me._oState.Language.AddUserToken(sDia)
                Me._oState.Language.AddUserToken(oIndictment.RuleName)
                If oCastRule.IdContract <> "" Then
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                Else
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                End If
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MaxMinShiftsSequence.StartDay", "")
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxShiftsSequence::CreateSequenceStartDayIndictment")
                Return Nothing
            End Try
            Return oIndictment
        End Function

        Private Function CreateSequenceCompensationIndictment(sCompensationShiftName As String, iCompensationDays As Integer) As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = oShiftsQ(0).PlanDate
                oIndictment.DateEnd = oShiftsQ(oShiftsQ.Count - 1).PlanDate
                oIndictment.Dates = oShiftsQ.Select(Function(x) x.PlanDate).ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(oIndictment.DateBegin)
                Me._oState.Language.AddUserToken(oIndictment.DateEnd)
                Me._oState.Language.AddUserToken(iCompensationDays.ToString)
                Me._oState.Language.AddUserToken(sCompensationShiftName)
                Me._oState.Language.AddUserToken(oIndictment.RuleName)
                If oCastRule.IdContract <> "" Then
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                Else
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                End If
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.MaxMinShiftsSequence.Compensation", "")
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_MinMaxShiftsSequence::CreateSequenceCompensationIndictment")
                Return Nothing
            End Try
            Return oIndictment
        End Function

    End Class

    ''' <summary>
    ''' Clase para gestión de reglas personalizadas.
    ''' Funcionan a partir de un código de customización y parámetros indicados en la descripción de la regla
    ''' </summary>
    Public Class ScheduleRule_CustomManager
        Inherits ScheduleRuleManager

        Private oEmpData As roEmployeeRulesData
        Private lIndictmentDates As New List(Of Date)
        Private oIndictment As roCalendarScheduleIndictment = Nothing
        Private oShiftsQ As List(Of Robotics.Base.DTOs.roCalendarRowDayData)
        Private oCastRule As roScheduleRule_Custom
        Private bCheckRule As Boolean = False
        Private iIDEmployee As Integer = 0
        Private iCurrentDay As Integer = 0
        Private dStartDate As Date
        Private dEndDate As Date
        Private iCounter1 As Integer = 0
        Private iCounter2 As Integer = 0
        Private iCounter3 As Integer = 0
        Private iCounter4 As Integer = 0
        Private lDates1 As New List(Of Date)
        Private lDates2 As New List(Of Date)
        Private lDates3 As New List(Of Date)
        Private lDates4 As New List(Of Date)

        Public Sub New(ByVal oRule As roScheduleRule_Custom, ByVal oState As roCalendarScheduleRulesState)
            Me._oState = oState
            Me.oCastRule = oRule
            Me.Parameters = oRule
            Me.Parameters.IDRule = ScheduleRuleType.Custom
            Me.Parameters.RuleType = ScheduleRuleBaseType.Custom
            Me.Parameters.DayDepth = DateDiff(DateInterval.Day, oCastRule.BeginPeriod, oCastRule.EndPeriod) + 1
        End Sub

        Public Overrides Function Applies(dayobject As Object, empobject As Object) As Boolean
            Dim bolRet As Boolean = False

            Try
                oEmpData = CType(empobject, roEmployeeRulesData)
                oShiftsQ = CType(dayobject, List(Of Robotics.Base.DTOs.roCalendarRowDayData))

                ' Vemos que la regla aplique al convenio o contrato del empleado (según la regla) en ese día
                If Not oEmpData.ContractsFiltered Is Nothing Then
                    If Me.Parameters.IdLabAgree > 0 Then
                        bCheckRule = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdLabAgree = Me.Parameters.IdLabAgree))
                    Else
                        bCheckRule = (oEmpData.ContractsFiltered.Count > 0 AndAlso (oEmpData.ContractsFiltered(0).IdContract = Me.Parameters.IdContract))
                    End If
                End If

                If bCheckRule Then
                    ' No tengo días suficientes para la secuencia. no hace falta evaluar
                    bCheckRule = (oShiftsQ.Count = Me.Parameters.DayDepth)
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_CustomManager::Applies")
                Return True
            End Try
            Return True
        End Function

        Public Overrides Function Check(oShiftsQObject As Object, oemp As Robotics.Base.DTOs.roCalendarRowEmployeeData, datestart As Date, dateend As Date, dyearfirstdate As Date) As List(Of roCalendarScheduleIndictment)
            Dim oRet As New List(Of roCalendarScheduleIndictment)
            Dim oIndictment As roCalendarScheduleIndictment = Nothing
            Dim bIndictmentFound As Boolean = False

            Try

                iCurrentDay = oShiftsQ.Count - 1
                iIDEmployee = oemp.IDEmployee
                dStartDate = datestart
                dEndDate = dateend

                ' Añado todas las fechas a la lista de candidatos a impacto. Si encuentro una secuencia, quitaré sus fechas.
                lIndictmentDates.Add(oShiftsQ(iCurrentDay).PlanDate)

                If bCheckRule Then
                    ' Miro si los días cumplen la definición de la secuencia
                    If Not oShiftsQ(iCurrentDay).MainShift Is Nothing Then
                        ' Libranza por día festivo trabajado (FR) dentro de la semana anterior la presente y la siguiente
                        If (oCastRule.RuleDescription.ToUpper.Contains("CODE=[3569:1]") OrElse oCastRule.RuleDescription.ToUpper.Contains("CODE=[3569:2]")) AndAlso oShiftsQ(iCurrentDay).MainShift.PlannedHours > 0 AndAlso oShiftsQ(iCurrentDay).Feast Then
                            iCounter1 += 1
                            lDates1.Add(oShiftsQ(iCurrentDay).PlanDate)
                        End If

                        If oCastRule.RuleDescription.ToUpper.Contains("CODE=[3569:1]") AndAlso oShiftsQ(iCurrentDay).MainShift.ShortName = "FR" Then
                            iCounter2 += 1
                            lDates2.Add(oShiftsQ(iCurrentDay).PlanDate)
                        End If

                        If oCastRule.RuleDescription.ToUpper.Contains("CODE=[3569:2]") AndAlso oShiftsQ(iCurrentDay).MainShift.ShortName = "DC" Then
                            ' Por cada 7 festivos trabajados, mínimo 1 DC
                            iCounter3 += 1
                        End If
                    End If
                End If

                ' Si se acaba el periodo, o hay cambio de periodo
                If oShiftsQ(iCurrentDay).PlanDate = dateend Then
                    If oCastRule.RuleDescription.ToUpper.Contains("CODE=[3569:1]") Then
                        ' Libranza por día festivo trabajado (FR) dentro de la semana anterior la presente y la siguiente
                        ' festivos trabajados -> lDates1
                        ' horarios de libranza FR -> lDates2

                        If lDates2.Count >= lDates1.Count Then
                            Dim dBegin As Date
                            Dim dEnd As Date
                            Dim dTemp As Date
                            For Each oDate As Date In lDates1
                                ' Busco libranza entre el inicio de la semana pasada y el final de la siguiente
                                dBegin = oDate.AddDays(-1 * (oDate.DayOfWeek - 1) - 7)
                                dEnd = dBegin.AddDays(20)
                                If lDates2.FindAll(Function(x) x >= dBegin AndAlso x <= dEnd).Count > 0 Then
                                    dTemp = lDates2.Find(Function(x) x >= dBegin AndAlso x <= dEnd)
                                    lDates2.Remove(dTemp)
                                Else
                                    oIndictment = CreateCustomIndictment_3569_1()
                                    oRet.Add(oIndictment)
                                    Exit For
                                End If
                            Next
                        Else
                            oIndictment = CreateCustomIndictment_3569_1()
                            oRet.Add(oIndictment)
                        End If

                    End If

                    If oCastRule.RuleDescription.ToUpper.Contains("CODE=[3569:2]") Then
                        ' Por cada 7 festivos trabajados, mínimo 1 DC
                        ' iCounter1 -> total festivos trabajados
                        ' iCounter3 -> DC
                        Dim iMinDC As Integer = iCounter1 \ 7
                        If iMinDC > 3 Then iMinDC = 3
                        If iCounter3 < iMinDC Then
                            oIndictment = CreateCustomIndictment_3569_2(iMinDC)
                            oRet.Add(oIndictment)
                        End If
                    End If
                    lIndictmentDates.Clear()
                End If
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_CustomManager::Check")
                Return Nothing
            End Try
            Return oRet

        End Function

        Private Function CreateCustomIndictment_3569_1() As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = dStartDate
                oIndictment.DateEnd = dEndDate
                oIndictment.Dates = lIndictmentDates.ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(iCounter1.ToString)
                Me._oState.Language.AddUserToken(oIndictment.DateBegin)
                Me._oState.Language.AddUserToken(oIndictment.DateEnd)
                Me._oState.Language.AddUserToken(iCounter2.ToString)
                Me._oState.Language.AddUserToken(oIndictment.RuleName)
                If oCastRule.IdContract <> "" Then
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                Else
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                End If
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.Custom.3569.1", "")
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_Custom::CreateCustomIndictment_3569_1")
                Return Nothing
            End Try
            Return oIndictment
        End Function

        Private Function CreateCustomIndictment_3569_2(iMin As Integer) As roCalendarScheduleIndictment
            Dim oIndictment As roCalendarScheduleIndictment
            Try
                oIndictment = New roCalendarScheduleIndictment
                oIndictment.DateBegin = dStartDate
                oIndictment.DateEnd = dEndDate
                oIndictment.Dates = lIndictmentDates.ToArray
                oIndictment.IDEmployee = iIDEmployee
                oIndictment.IDScheduleRule = oCastRule.IDRule
                oIndictment.RuleName = oCastRule.RuleName
                Me._oState.Language.ClearUserTokens()
                Me._oState.Language.AddUserToken(iCounter3.ToString)
                Me._oState.Language.AddUserToken(oIndictment.DateBegin)
                Me._oState.Language.AddUserToken(oIndictment.DateEnd)
                Me._oState.Language.AddUserToken(iCounter1.ToString)
                Me._oState.Language.AddUserToken(iMin.ToString)
                Me._oState.Language.AddUserToken(oIndictment.RuleName)
                If oCastRule.IdContract <> "" Then
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.Contract", ""))
                Else
                    Me._oState.Language.AddUserToken(Me._oState.Language.Translate("CalendarScheduleRules.LabAgree", ""))
                End If
                oIndictment.ErrorText = Me._oState.Language.Translate("CalendarScheduleRules.Custom.3569.2", "")
            Catch ex As Exception
                Me._oState.UpdateStateInfo(ex, "ScheduleRule_Custom::CreateCustomIndictment_3569_2")
                Return Nothing
            End Try
            Return oIndictment
        End Function

    End Class

End Namespace