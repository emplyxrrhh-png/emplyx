Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBudgets
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Security.Base
Imports Robotics.Base.VTCalendar

Namespace Requests

    <DataContract()>
    Public Class roRequestRuleManager

        Public Shared Function ValidatePeriodEnjoynment(ByVal _Request As roRequest, ByVal _RequestRuleDefinition As roRequestRuleDefinition, ByVal _State As roRequestState, Optional _ApproveRefuse As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim xBeginPeriod As Date = Nothing
            Dim xEndPeriod As Date = Nothing

            Try

                ' Control para vacaciones anuales: sólo pueden ser por periodo de fechas (mes/día). En este caso, tanto BeginPeriod como EndPeriod son relativos al año 1901
                Try
                    xBeginPeriod = DateSerial(Year(Now.Date), Month(_RequestRuleDefinition.BeginPeriod.Value), Day(_RequestRuleDefinition.BeginPeriod.Value))
                    If Year(_RequestRuleDefinition.BeginPeriod.Value) = 1901 Then
                        xBeginPeriod = xBeginPeriod.AddYears(1)
                    End If
                Catch ex As Exception
                    xBeginPeriod = DateSerial(Year(Now.Date), Month(Now.Date), 1)
                End Try

                Try
                    xEndPeriod = DateSerial(Year(Now.Date), Month(_RequestRuleDefinition.EndPeriod.Value), Day(_RequestRuleDefinition.EndPeriod.Value))
                    If Year(_RequestRuleDefinition.EndPeriod.Value) = 1901 Then xEndPeriod = xEndPeriod.AddYears(1)
                Catch ex As Exception
                    xEndPeriod = DateSerial(Year(Now.Date), 12, 31)
                End Try

                For Each oRequestDay As roRequestDay In _Request.RequestDays
                    If Not (oRequestDay.RequestDate >= xBeginPeriod AndAlso oRequestDay.RequestDate <= xEndPeriod) Then
                        Return False
                        Exit Function
                    End If
                Next

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::ValidatePeriodEnjoynment")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::ValidatePeriodEnjoynment")
            End Try

            Return bolRet
        End Function

        Public Shared Function ValidateMinCoverageRequiered(ByVal _Request As Requests.roRequest, ByVal _State As roRequestState) As Boolean
            Dim bolRet As Boolean = True
            Dim bolFinishTrans As Boolean = False

            Try

                ' Obtenemnos el passport de sistema
                Dim intIDPassport As Integer = roTypes.Any2Integer(roConstants.GetSystemUserId())

                If intIDPassport > 0 Then
                    Dim xDate As Date = _Request.Date1.Value
                    Dim oCalendarState = New VTCalendar.roCalendarState(intIDPassport)
                    Dim oCalendarManager As New VTCalendar.roCalendarManager(oCalendarState)

                    'Para cada dia del periodo solicitado
                    While (xDate <= _Request.Date2.Value) And bolRet

                        Dim bolVerifyDate As Boolean = True
                        If _Request.RequestDays IsNot Nothing AndAlso _Request.RequestDays.Count > 0 Then
                            ' Verificamos si la fecha del periodo esta dentro de la lista de fechas solicitadas
                            If _Request.RequestDays.FindAll(Function(x) x.RequestDate = xDate).Count = 0 Then
                                bolVerifyDate = False
                            End If
                        End If

                        If bolVerifyDate Then
                            ' Obtenemos el nodo del empleado
                            'TODO Replace roSecurityNodes For Something
                            Dim intNode As Integer = 0 'VTSecurity.roSecurityNodeManager.GetSecurityNodeFromGroupOrEmployee(New roSecurityNodeState(-1), _Request.IDEmployee, False, xDate)

                            If intNode > 0 Then
                                ' Obtenemos el horario y puesto asignado
                                Dim oEmployeeCalendar As New roCalendar
                                oEmployeeCalendar = oCalendarManager.Load(xDate, xDate, "B" & _Request.IDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)
                                If oEmployeeCalendar IsNot Nothing AndAlso oEmployeeCalendar.CalendarData IsNot Nothing Then
                                    ' Parametro avanzado para indicar si tenemos que tener unicamente en cuenta el horario planificado
                                    ' sin el puesto
                                    Dim oParam As New AdvancedParameter.roAdvancedParameter("VTLive.RequestRules.MinCoverageWithoutAssignment", New AdvancedParameter.roAdvancedParameterState(-1))
                                    Dim bolMinCoverageWithoutAssignment As Boolean = roTypes.Any2Boolean(oParam.Value)

                                    For Each oEmployeeCalendarRowEmployeeData As roCalendarRow In oEmployeeCalendar.CalendarData
                                        If oEmployeeCalendarRowEmployeeData IsNot Nothing AndAlso oEmployeeCalendarRowEmployeeData.PeriodData IsNot Nothing Then
                                            For Each oEmployeeCalendarRowDayData As roCalendarRowDayData In oEmployeeCalendarRowEmployeeData.PeriodData.DayData
                                                If oEmployeeCalendarRowDayData.PlanDate = xDate Then
                                                    If oEmployeeCalendarRowDayData.MainShift IsNot Nothing AndAlso oEmployeeCalendarRowDayData.MainShift.ID > 0 Then
                                                        ' Obtenemos la cobertura minima para ese horario y puesto
                                                        Dim oBudgetManager As New VTBudgets.roBudgetManager(New roBudgetState(intIDPassport))
                                                        Dim oProductiveUnitModePosition As New roProductiveUnitModePosition
                                                        oProductiveUnitModePosition.ShiftData = New roCalendarRowShiftData
                                                        oProductiveUnitModePosition.ShiftData = oEmployeeCalendarRowDayData.MainShift

                                                        If oEmployeeCalendarRowDayData.AssigData IsNot Nothing AndAlso oEmployeeCalendarRowDayData.AssigData.ID > 0 AndAlso Not bolMinCoverageWithoutAssignment Then
                                                            oProductiveUnitModePosition.AssignmentData = New roCalendarAssignmentCellData
                                                            oProductiveUnitModePosition.AssignmentData = oEmployeeCalendarRowDayData.AssigData
                                                        End If

                                                        Dim dblAmount As Double = oBudgetManager.GetMinimumAmountWithSpecificSchedulerInNode(intNode, xDate, oProductiveUnitModePosition)
                                                        If dblAmount > 0 Then
                                                            ' Obtenemos los empleados planificados con el mismo horario/puesto
                                                            Dim oEmployeesAvailable As roEmployeeAvailableForNode = oBudgetManager.GetEmployeesAvailableWithSpecificSchedulerInNode(intNode, xDate, oProductiveUnitModePosition)
                                                            If oEmployeesAvailable IsNot Nothing AndAlso oEmployeesAvailable.BudgetEmployeeAvailableForNode IsNot Nothing AndAlso ((oEmployeesAvailable.BudgetEmployeeAvailableForNode.Count - 1) < dblAmount) Then
                                                                bolRet = False
                                                                Return bolRet
                                                                Exit Function
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            Next
                                        End If
                                    Next
                                End If
                            End If
                        End If
                        xDate = xDate.AddDays(1)
                    End While
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::ValidateMinCoverageRequiered")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::ValidateMinCoverageRequiered")
            End Try

            Return bolRet
        End Function

        Public Shared Function ValidateMaxNotScheduledDays(ByVal _Request As Requests.roRequest, ByVal _State As roRequestState) As Boolean
            Dim bolRet As Boolean = True
            Dim bolFinishTrans As Boolean = False

            Try

                Dim intIDPassport As Integer = roTypes.Any2Integer(roConstants.GetSystemUserId())

                If intIDPassport > 0 Then
                    ' Calculo días de inicio de año y día de inicio de mes
                    Dim oCalRuleState As New VTCalendar.roCalendarScheduleRulesState(intIDPassport)
                    Dim oCalRulesManager As New VTCalendar.roCalendarScheduleRulesManager(oCalRuleState)
                    Dim oParameters As New roParameters("OPTIONS", True)
                    Dim iMonthIniDay As Integer
                    Dim iYearIniMonth As Integer
                    Dim dYearFirstDate As Date = DateSerial(2000, 1, 1)
                    dYearFirstDate = oCalRulesManager.GetYearFirstDate(_Request.Date1.Value, oParameters, iMonthIniDay, iYearIniMonth)

                    Dim dYearFirstDate2 As Date = DateSerial(2000, 1, 1)
                    dYearFirstDate2 = oCalRulesManager.GetYearFirstDate(_Request.Date2.Value, oParameters, iMonthIniDay, iYearIniMonth)
                    Dim intYears As Integer = DateDiff(DateInterval.Year, dYearFirstDate, dYearFirstDate2)

                    ' Para cada año dentro del peiodo de ausencia
                    For x = 0 To intYears
                        ' Cargamos calendario
                        Dim oCalendarState As VTCalendar.roCalendarState
                        Dim oCalendarManager As VTCalendar.roCalendarManager = Nothing
                        Dim oCalendar As DTOs.roCalendar = Nothing
                        oCalendarState = New VTCalendar.roCalendarState(intIDPassport)
                        oCalendarManager = New VTCalendar.roCalendarManager(oCalendarState)
                        oCalendar = New DTOs.roCalendar

                        oCalendar = oCalendarManager.Load(dYearFirstDate, dYearFirstDate.AddYears(1).AddDays(-1), "B" & _Request.IDEmployee, CalendarView.Planification, CalendarDetailLevel.Daily, True)

                        ' Evalúo Impactos de planificación
                        Dim oIndictments As New List(Of roCalendarScheduleIndictment)

                        Try
                            oIndictments = oCalRulesManager.CheckScheduleRules(oCalendar,, Convert.ToInt16(ScheduleRuleType.MaxNotScheduled).ToString)
                            If oIndictments IsNot Nothing AndAlso oIndictments.Count > 0 Then
                                For Each oIndictment As roCalendarScheduleIndictment In oIndictments
                                    ' Si hay impacto de la regla de maximos dias sin planificar , cancelamos
                                    If oIndictment.IDScheduleRule = ScheduleRuleType.MaxNotScheduled Then
                                        bolRet = False
                                        Return bolRet
                                        Exit Function
                                    End If
                                Next
                            End If
                        Catch ex As Exception
                        End Try
                        dYearFirstDate = dYearFirstDate.AddYears(1)
                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::ValidateMaxNotScheduledDays")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::ValidateMaxNotScheduledDays")
            End Try

            Return bolRet
        End Function

        Public Shared Function ValidateScheduleRules(ByVal _Request As Requests.roRequest, ByVal IdLabAgree As Integer, ByVal idRules As List(Of Integer), ByVal _State As roRequestState) As Boolean
            Dim bolRet As Boolean = True
            Dim bolFinishTrans As Boolean = False

            Try

                Dim intIDPassport As Integer = roTypes.Any2Integer(roConstants.GetSystemUserId())

                If intIDPassport > 0 Then
                    Dim oCalRuleState As New roCalendarScheduleRulesState(intIDPassport)
                    Dim oCalendarState = New roCalendarState(intIDPassport)

                    Dim oCalRulesManager As New roCalendarScheduleRulesManager(oCalRuleState)
                    Dim oCalendarManager As New roCalendarManager(oCalendarState)
                    Dim oApplicantCalendar As New DTOs.roCalendar

                    Dim oParameters As New roParameters("OPTIONS", True)
                    Dim iMonthIniDay As Integer
                    Dim iYearIniMonth As Integer
                    Dim dYearFirstDate As Date = DateSerial(2000, 1, 1)
                    Dim dYearLastDate As Date = DateSerial(2000, 1, 1)
                    Dim oIndictments As New List(Of roCalendarScheduleIndictment)
                    Dim oRequestedShiftData As roCalendarRowShiftData = Nothing
                    Dim oExchangeShiftData As roCalendarRowShiftData = Nothing
                    dYearFirstDate = oCalRulesManager.GetYearFirstDate(_Request.Date1.Value, oParameters, iMonthIniDay, iYearIniMonth)
                    dYearLastDate = dYearFirstDate.AddYears(1).AddDays(-1)
                    oRequestedShiftData = oCalendarManager.LoadShiftDayDataByIdShift(_Request.IDShift)

                    If _Request.RequestType = eRequestType.ExchangeShiftBetweenEmployees Then
                        oApplicantCalendar = oCalendarManager.Load(_Request.Date1.Value, _Request.Date1.Value, $"B{_Request.IDEmployee},B{_Request.IDEmployeeExchange}", CalendarView.Planification, CalendarDetailLevel.Daily, True)
                        oExchangeShiftData = oCalendarManager.LoadShiftDayDataByIdShift(_Request.Field4)
                    ElseIf _Request.RequestType = eRequestType.VacationsOrPermissions Then
                        Dim minRequestDate As Date = _Request.RequestDays.Min(Function(d) d.RequestDate)
                        Dim maxRequestDate As Date = _Request.RequestDays.Max(Function(d) d.RequestDate)

                        oApplicantCalendar = oCalendarManager.Load(minRequestDate, maxRequestDate, "B" & _Request.IDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, True)
                    Else
                        oApplicantCalendar = oCalendarManager.Load(_Request.Date1.Value, _Request.Date1.Value, "B" & _Request.IDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, True)
                    End If

                    For Each oCalendarRow As roCalendarRow In oApplicantCalendar.CalendarData
                        If _Request.IDEmployee = oCalendarRow.EmployeeData.IDEmployee Then
                            For Each oCalendarRowDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                                If _Request.RequestType = eRequestType.VacationsOrPermissions Then
                                    Dim matchingRequestDay = _Request.RequestDays.FirstOrDefault(Function(rd) rd.RequestDate = oCalendarRowDayData.PlanDate)
                                    If matchingRequestDay IsNot Nothing Then
                                        oCalendarRowDayData.MainShift = oRequestedShiftData
                                        oCalendarRowDayData.HasChanged = True
                                    End If
                                Else
                                    If oCalendarRowDayData.PlanDate = _Request.Date1 Then
                                        oCalendarRowDayData.MainShift = oRequestedShiftData
                                        oCalendarRowDayData.HasChanged = True
                                        Exit For
                                    End If
                                End If
                            Next
                        ElseIf _Request.RequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso _Request.IDEmployeeExchange = oCalendarRow.EmployeeData.IDEmployee Then
                            For Each oCalendarRowDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                                If oCalendarRowDayData.PlanDate = _Request.Date1 Then
                                    oCalendarRowDayData.MainShift = oExchangeShiftData
                                    oCalendarRowDayData.HasChanged = True
                                    Exit For
                                End If
                            Next
                        End If
                    Next

                    Dim dLabAgreeRulesToCheck As New Dictionary(Of Integer, List(Of Integer))
                    If idRules IsNot Nothing AndAlso idRules.Count = 1 AndAlso idRules(0) = -1 Then
                        If _Request.RequestType = eRequestType.VacationsOrPermissions Then
                            Dim iIDLabAgree As Integer = -1

                            For Each oRequestDay As roRequestDay In _Request.RequestDays
                                Dim strSQL As String = $"@SELECT# IDLabAgree From EmployeeContracts Where {roTypes.Any2Time(oRequestDay).SQLSmallDateTime} BETWEEN BeginDate AND EndDate And IDEmployee = {_Request.IDEmployee}"
                                iIDLabAgree = roTypes.Any2Integer(ExecuteScalar(strSQL))
                                If iIDLabAgree <> -1 Then
                                    Dim ruleIds As List(Of Integer) = oCalRulesManager.GetScheduleRules(iIDLabAgree:=iIDLabAgree).ToList().ConvertAll(Of Integer)(Function(x) x.Id)
                                    If dLabAgreeRulesToCheck.ContainsKey(iIDLabAgree) Then
                                        For Each ruleId In ruleIds
                                            If Not dLabAgreeRulesToCheck(iIDLabAgree).Contains(ruleId) Then
                                                dLabAgreeRulesToCheck(iIDLabAgree).Add(ruleId)
                                            End If
                                        Next
                                    Else
                                        dLabAgreeRulesToCheck.Add(iIDLabAgree, ruleIds)
                                    End If
                                End If
                            Next
                        Else
                            Dim ruleIds As List(Of Integer) = oCalRulesManager.GetScheduleRules(iIDLabAgree:=IdLabAgree).ToList().ConvertAll(Of Integer)(Function(x) x.Id)
                            If dLabAgreeRulesToCheck.ContainsKey(IdLabAgree) Then
                                For Each ruleId In ruleIds
                                    If Not dLabAgreeRulesToCheck(IdLabAgree).Contains(ruleId) Then
                                        dLabAgreeRulesToCheck(IdLabAgree).Add(ruleId)
                                    End If
                                Next
                            Else
                                dLabAgreeRulesToCheck.Add(IdLabAgree, ruleIds)
                            End If
                        End If
                    Else
                        If dLabAgreeRulesToCheck.ContainsKey(IdLabAgree) Then
                            For Each ruleId In idRules
                                If Not dLabAgreeRulesToCheck(IdLabAgree).Contains(ruleId) Then
                                    dLabAgreeRulesToCheck(IdLabAgree).Add(ruleId)
                                End If
                            Next
                        Else
                            dLabAgreeRulesToCheck.Add(IdLabAgree, idRules)
                        End If
                    End If

                    oIndictments = oCalRulesManager.CheckScheduleRules(oApplicantCalendar, dLabAgreeActualIdRulesToCheck:=dLabAgreeRulesToCheck)

                    If oIndictments IsNot Nothing AndAlso oIndictments.Count > 0 Then
                        For Each oIndictment In oIndictments
                            If oIndictment.Dates IsNot Nothing AndAlso oIndictment.Dates.Contains(_Request.Date1) Then
                                bolRet = False
                                Exit For
                            End If
                        Next
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::ValidateMaxNotScheduledDays")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::ValidateMaxNotScheduledDays")
            End Try

            Return bolRet
        End Function

        Public Shared Function RequestRulesValidate(ByRef _Request As roRequest, ByRef _State As roRequestState, Optional _ApproveRefuse As Boolean = False, Optional ByVal _simpleMessage As Boolean = False, Optional ByVal _CheckOnlyAutomaticValidation As Boolean = False, Optional ByVal _IgnoreAutomaticValidationRule As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Try
                If _Request Is Nothing Then
                    Return True
                    Exit Function
                End If

                If Not _Request.Date1.HasValue Then
                    Return True
                    Exit Function
                End If

                If _Request.IDEmployee <= 0 Then
                    Return True
                    Exit Function
                End If

                ' Obtenemos el convenio del empleado para la fecha inicial de solicitud
                Dim IdLabAgree As Integer = -1

                Dim strSQL As String = " @SELECT# IDLabAgree From EmployeeContracts " &
                                       " Where BeginDate <= " & roTypes.Any2Time(_Request.Date1).SQLSmallDateTime & " " &
                                       " And EndDate >= " & roTypes.Any2Time(_Request.Date1).SQLSmallDateTime & " " &
                                       " And IDEmployee = '" & _Request.IDEmployee & "' "
                IdLabAgree = roTypes.Any2Integer(ExecuteScalar(strSQL))

                If IdLabAgree <= 0 Then
                    Return True
                    Exit Function
                End If

                Dim oLabAgreeState As New roLabAgreeState(_State.IDPassport)

                Dim labAgreeRules As Generic.List(Of roRequestRule) = roRequestRule.GetRequestsRules("IDLabAgree=" & IdLabAgree, oLabAgreeState, False)

                ' Cargamos los datos del convenio del empleado
                If labAgreeRules Is Nothing OrElse labAgreeRules.Count = 0 Then
                    Return True
                    Exit Function
                End If

                bolRet = True
                _State.Result = LabAgreeResultEnum.NoError

                Dim strErrorText As String = ""
                'Dim oEmployees As New Employee.roEmployees : Dim oEmployeeState As New Employee.roEmployeeState()
                'roBusinessState.CopyTo(_State, oEmployeeState)

                Dim intIDShift As Integer = -1
                If _Request.IDShift.HasValue Then
                    intIDShift = _Request.IDShift.Value
                End If

                Dim intIDCause As Integer = -1
                If _Request.IDCause.HasValue Then
                    intIDCause = _Request.IDCause.Value
                End If

                Dim intIDShift2 As Double = _Request.Field4

                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                Dim bolApplyAPV As Boolean = False
                Dim bolApplyUPF As Boolean = False

                If roTypes.Any2String(oAdvParam.Value).ToUpper = "VPA" Then
                    ' Verficamos si tienen customizacion de APV
                    bolApplyAPV = True
                End If

                If roTypes.Any2String(oAdvParam.Value).ToUpper = "UEPMOP" Then
                    ' Verficamos si tienen customizacion de UPF
                    bolApplyUPF = True
                End If

                For Each oRequestRule As roRequestRule In labAgreeRules
                    bolRet = True
                    Dim strErrorTextLine As String = ""
                    Dim bolVerifyRule As Boolean = True

                    ' Si debemos verificar solo la regla de validacion automatica
                    If _CheckOnlyAutomaticValidation And oRequestRule.IDRuleType <> eRequestRuleType.AutomaticValidation Then
                        bolVerifyRule = False
                    ElseIf _IgnoreAutomaticValidationRule And oRequestRule.IDRuleType = eRequestRuleType.AutomaticValidation Then
                        bolVerifyRule = False
                    End If

                    If bolVerifyRule Then
                        ' Si existe una regla que aplique a esta solicitud
                        If oRequestRule.Activated And oRequestRule.IDRequestType = _Request.RequestType _
                                        And oRequestRule.Definition IsNot Nothing AndAlso oRequestRule.Definition.EmployeeValidation <> _ApproveRefuse Then

                            Dim bolApplyRule As Boolean = False
                            ' Verificamos si se debe aplicar la regla en funcion del tipo de solicitud y los motivos indicados
                            Select Case oRequestRule.IDRequestType
                            ' Vacaciones por dias
                                Case DTOs.eRequestType.VacationsOrPermissions
                                    If (oRequestRule.Definition.IDReasons.Count > 0 AndAlso _Request.RequestDays IsNot Nothing AndAlso _Request.RequestDays.Count > 0 AndAlso _Request.IDShift.HasValue AndAlso (oRequestRule.Definition.IDReasons.FindAll(Function(o) oRequestRule.Definition.IDReasons.Contains(intIDShift)).Count > 0 OrElse oRequestRule.Definition.IDReasons.Contains(-1))) Then
                                        bolApplyRule = True
                                    End If

                            ' Vacaciones por horas
                                Case DTOs.eRequestType.PlannedHolidays
                                    If oRequestRule.Definition.IDReasons.Count > 0 AndAlso _Request.RequestDays IsNot Nothing AndAlso _Request.RequestDays.Count > 0 AndAlso _Request.IDCause.HasValue AndAlso oRequestRule.Definition.IDReasons.FindAll(Function(o) oRequestRule.Definition.IDReasons.Contains(intIDCause)).Count > 0 Then
                                        bolApplyRule = True
                                    End If

                            ' Ausencias por dias/horas
                                Case DTOs.eRequestType.PlannedAbsences, DTOs.eRequestType.PlannedCauses, DTOs.eRequestType.PlannedOvertimes
                                    If oRequestRule.Definition.IDReasons.Count > 0 AndAlso _Request.IDCause.HasValue AndAlso oRequestRule.Definition.IDReasons.FindAll(Function(o) oRequestRule.Definition.IDReasons.Contains(intIDCause)).Count > 0 Then
                                        bolApplyRule = True
                                    End If

                            ' Inrercambio de horario entre empleados
                                Case DTOs.eRequestType.ExchangeShiftBetweenEmployees
                                    If (oRequestRule.Definition.IDReasons.Count > 0 AndAlso _Request.IDShift.HasValue AndAlso oRequestRule.Definition.IDReasons.FindAll(Function(o) oRequestRule.Definition.IDReasons.Contains(intIDShift)).Count > 0 AndAlso _Request.Field4 > 0 AndAlso (oRequestRule.Definition.IDReasons.FindAll(Function(o) oRequestRule.Definition.IDReasons.Contains(intIDShift2)).Count > 0 _
                                            OrElse oRequestRule.Definition.IDReasons.Contains(-1))) Then
                                        bolApplyRule = True
                                    End If
                                    If Not bolApplyRule AndAlso oRequestRule.IDRuleType = eRequestRuleType.AutomaticValidation Then
                                        ' En el caso de regla de validacion automatica
                                        ' si existe la customizacion de APV se aplica con cualquier horario
                                        If roTypes.Any2String(oAdvParam.Value).ToUpper = "VPA" Then
                                            bolApplyRule = True
                                        End If
                                    End If
                                Case DTOs.eRequestType.ChangeShift
                                    If (oRequestRule.Definition.IDReasons.Count > 0 AndAlso _Request.IDShift.HasValue AndAlso (oRequestRule.Definition.IDReasons.FindAll(Function(o) oRequestRule.Definition.IDReasons.Contains(intIDShift)).Count > 0 OrElse oRequestRule.Definition.IDReasons.Contains(-1))) Then
                                        bolApplyRule = True
                                    End If
                            End Select

                            If bolApplyRule Then
                                If bolApplyAPV AndAlso (_Request.RequestType = eRequestType.ExchangeShiftBetweenEmployees Or _Request.RequestType = eRequestType.VacationsOrPermissions) AndAlso oRequestRule.Description.ToUpper.Contains("VERIFYDAY") Then
                                    ' En el caso de APV
                                    ' si la regla de solicitud tiene indicado el TAG VERIFYDAY
                                    ' debemos validar ademas de la propia regla, si se piden fechas correspondientes al año siguiente , que el dia en el que se está realizando la solicitud sea superior a
                                    ' VERIFYDAY=27/12
                                    Try
                                        bolRet = VerifyDay_APV(_Request, oRequestRule, strErrorTextLine, _State, _simpleMessage)
                                        If Not bolRet Then
                                            Exit For
                                        End If
                                    Catch ex As Exception
                                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roRequestRuleManager::RequestRulesValidate::APV Customization: Error validating date limit", ex)
                                    End Try
                                End If

                                If bolRet Then
                                    ' Validamos la regla
                                    Select Case oRequestRule.IDRuleType
                                        Case eRequestRuleType.NegativeAccrual ' Saldo positivo
                                            If bolApplyAPV And _Request.RequestType = eRequestType.VacationsOrPermissions Then
                                                ' En el caso de APV aplicamos la regla de forma customizada, si son Vacaciones/permisos
                                                bolRet = RequestRuleNegativeAcrual_APV(_Request, _State, _ApproveRefuse)
                                                If Not bolRet Then
                                                    strErrorTextLine = _State.Language.Translate("RequestAction.NegativeAccrual", "RequestAction")
                                                End If
                                            Else
                                                Dim intResult As Double = 0
                                                Dim intDone As Double = 0 : Dim intPending As Double = 0
                                                Dim intLasting As Double = 0 : Dim intDisponible As Double = 0
                                                Dim intPending_P As Double = 0
                                                Dim intLasting_P As Double = 0 : Dim intDisponible_P As Double = 0
                                                Dim intExpiredValue As Double = 0
                                                Dim intWithoutEnjoynmentValue As Double = 0
                                                Dim intIDConceptBalance As Integer = 0

                                                Dim strDefaultQuery As String = ""
                                                Dim lstRequestDates As List(Of Date) = Nothing

                                                If _Request.RequestType = eRequestType.VacationsOrPermissions Then
                                                    ' Vacaciones/permisos por dias

                                                    ' Si el saldo es de tipo L, debemos pasar los dias solicitados a la función en caso necesario
                                                    strDefaultQuery = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from Shifts where id =" & _Request.IDShift.ToString & ") "))
                                                    If strDefaultQuery = "L" AndAlso Not _ApproveRefuse Then
                                                        If _Request.RequestDays.Count > 0 Then
                                                            lstRequestDates = New List(Of Date)
                                                            lstRequestDates = _Request.RequestDays.Select(Function(d) d.RequestDate).ToList()
                                                        End If
                                                    End If

                                                    roBusinessSupport.VacationsResumeQuery(_Request.IDEmployee, _Request.IDShift, Now.Date, Nothing, Nothing, _Request.Date1.Value, intDone, intPending, intLasting, intDisponible, _State, intExpiredValue, intWithoutEnjoynmentValue,, lstRequestDates)

                                                    ' Si el saldo tiene caducidad, obtenemos los valores del saldo que va a caducar desde hoy hasta la ultima fecha solicitada
                                                    intIDConceptBalance = roTypes.Any2Double(ExecuteScalar("@SELECT# ISNULL(ID , 0) FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from Shifts where id =" & _Request.IDShift.ToString & ") AND DefaultQuery = 'C' AND isnull(ApplyExpiredHours,0) = 1"))
                                                    If intIDConceptBalance > 0 Then intExpiredValue = roBusinessSupport.GetExpiredConceptValues(_Request.IDEmployee, intIDConceptBalance, Now.Date, IIf(_Request.Date2.HasValue, _Request.Date2.Value.Date, _Request.Date1.Value), _State)

                                                    intResult = intDisponible - (intLasting + intPending + intExpiredValue + intWithoutEnjoynmentValue)

                                                    ' En los saldos de tipo L, la función VacationsResumeQuery ya devuelve todos los datos incluyendo lo que se esta solicitando en ese momento
                                                    ' por lo que no es necesario volver a añadir despúes los dias solicitados
                                                    If strDefaultQuery <> "L" Then
                                                        If Not _ApproveRefuse Then
                                                            Dim dblFactor As Double = roTypes.Any2Double(ExecuteScalar("@SELECT# isnull(dailyfactor, 1) from shifts where id=" & _Request.IDShift))
                                                            intResult = intResult - (_Request.RequestDays.Count * dblFactor)
                                                        End If
                                                    End If
                                                ElseIf _Request.RequestType = eRequestType.PlannedHolidays Then
                                                    ' Vacaciones/permisos por horas
                                                    VTBusiness.Common.roBusinessSupport.ProgrammedHolidaysResumeQuery(_Request.IDEmployee, _Request.IDCause, Now.Date, Nothing, Nothing, _Request.Date1.Value, intPending, intLasting, intDisponible, _State)
                                                    intResult = intDisponible - intLasting - intPending
                                                    If Not _ApproveRefuse Then
                                                        ' Para cada dia solicitado
                                                        For Each roRequestDay As roRequestDay In _Request.RequestDays
                                                            If roRequestDay.AllDay Then
                                                                ' Si es todo el dia, obtenemos las horas teoricas del horario
                                                                strSQL = "@SELECT# (case when isnull(D.IsHolidays,0) = 1 then 0 else isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours)  end) ExpectedWorkingHours FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & _Request.IDEmployee.ToString
                                                                strSQL += " AND S.ID = D.IDShift1 "
                                                                strSQL += " AND D.Date = " & roTypes.Any2Time(roRequestDay.RequestDate).SQLSmallDateTime

                                                                ' Restamos las horas teoricas del horario planificado
                                                                intResult -= roTypes.Any2Double(ExecuteScalar(strSQL))
                                                            Else
                                                                ' Restamos la duracion solicitada
                                                                intResult -= roRequestDay.Duration
                                                            End If
                                                        Next
                                                    End If
                                                    If bolApplyUPF Then
                                                        ' En el caso de Pompeu Fabra y contega el TAG
                                                        If oRequestRule.Description.Contains("UPF_") Then
                                                            ' Debemos obtener el valor del saldo indicado en la descripcion y solo dejar en caso que sea <= 0
                                                            bolRet = RequestRuleNegativeAcrual_UPF(_Request, oRequestRule.Description, _State)
                                                            If Not bolRet Then
                                                                intResult = -5
                                                            End If
                                                        End If
                                                    End If
                                                ElseIf _Request.RequestType = eRequestType.PlannedAbsences Then
                                                    ' Previsiones de ausencia por dia

                                                    ' Obtenemos los dias que debemos tener en cuenta (laborables o naturales) para realizar los calculos
                                                    Dim bolApplyWorkDaysOnConcept As Boolean = roTypes.Any2Boolean(ExecuteScalar("@SELECT# ISNULL(ApplyWorkDaysOnConcept , 0) FROM Causes WHERE ID=" & _Request.IDCause))

                                                    ' En funcion del tipo de saldo, calculamos horas o días
                                                    Dim strConceptType As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(IDType , 'O') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from causes where id =" & _Request.IDCause & ")"))
                                                    If strConceptType.Length = 0 Then strConceptType = "O"

                                                    ' Obtenemos los datos de las ausencias por dias de la justificación
                                                    roBusinessSupport.PlannedAbsencesResumeQuery(_Request.IDEmployee, _Request.IDCause, Now.Date, Nothing, Nothing, _Request.Date1.Value, intPending, intLasting, intDisponible, _State, bolApplyWorkDaysOnConcept, strConceptType)

                                                    ' Ahora obtenemos los datos de las ausencias por horas de la misma justificación
                                                    roBusinessSupport.PlannedCausesResumeQuery(_Request.IDEmployee, _Request.IDCause, Now.Date, Nothing, Nothing, _Request.Date1.Value, intPending_P, intLasting_P, intDisponible_P, _State, bolApplyWorkDaysOnConcept, strConceptType)

                                                    ' Si el saldo tiene caducidad, obtenemos los valores del saldo que va a caducar desde hoy hasta la ultima fecha solicitada
                                                    intIDConceptBalance = roTypes.Any2Double(ExecuteScalar("@SELECT# ISNULL(ID , 0) FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from Causes where id =" & _Request.IDCause & ") AND DefaultQuery = 'C' AND isnull(ApplyExpiredHours,0) = 1"))
                                                    If intIDConceptBalance > 0 Then intExpiredValue = roBusinessSupport.GetExpiredConceptValues(_Request.IDEmployee, intIDConceptBalance, Now.Date, IIf(_Request.Date2.HasValue, _Request.Date2.Value.Date, _Request.Date1.Value), _State)

                                                    intResult = intDisponible - (intLasting + intPending + intLasting_P + intPending_P + intExpiredValue)
                                                    If Not _ApproveRefuse Then
                                                        Dim xBeginDate As Date = _Request.Date1.Value.Date
                                                        Dim xEndDate As Date = IIf(_Request.Date2.HasValue, _Request.Date2.Value.Date, xBeginDate)

                                                        ' Obtenemos las horas/dias a tener en cuenta y las restamos del resultado final
                                                        intResult -= roBusinessSupport.GetEffectiveDaysHoursAbsence(_Request.IDEmployee, xBeginDate, xEndDate, strConceptType, bolApplyWorkDaysOnConcept, _State)
                                                    End If
                                                ElseIf _Request.RequestType = eRequestType.PlannedOvertimes Then
                                                    ' Previsiones de excesos por horas

                                                    ' Obtenemos los dias que debemos tener en cuenta (laborables o naturales) para realizar los calculos
                                                    Dim bolApplyWorkDaysOnConcept As Boolean = roTypes.Any2Boolean(ExecuteScalar("@SELECT# ISNULL(ApplyWorkDaysOnConcept , 0) FROM Causes WHERE ID=" & _Request.IDCause))

                                                    ' En funcion del tipo de saldo, calculamos horas o días
                                                    Dim strConceptType As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(IDType , 'O') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from causes where id =" & _Request.IDCause & ")"))
                                                    If strConceptType.Length = 0 Then strConceptType = "O"

                                                    ' Obtenemos los datos de las ausencias por horas de la justificación
                                                    roBusinessSupport.PlannedOvertimesResumeQuery(_Request.IDEmployee, _Request.IDCause, Now.Date, Nothing, Nothing, _Request.Date1.Value, intPending, intLasting, intDisponible, _State, bolApplyWorkDaysOnConcept, strConceptType)

                                                    ' Si el saldo tiene caducidad, obtenemos los valores del saldo que va a caducar desde hoy hasta la ultima fecha solicitada
                                                    intIDConceptBalance = roTypes.Any2Double(ExecuteScalar("@SELECT# ISNULL(ID , 0) FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from causes where id =" & _Request.IDCause & ") AND DefaultQuery = 'C' AND isnull(ApplyExpiredHours,0) = 1"))
                                                    If intIDConceptBalance > 0 Then intExpiredValue = roBusinessSupport.GetExpiredConceptValues(_Request.IDEmployee, intIDConceptBalance, Now.Date, IIf(_Request.Date2.HasValue, _Request.Date2.Value.Date, _Request.Date1.Value), _State)

                                                    intResult = intDisponible - (intLasting + intPending + intLasting_P + intPending_P + intExpiredValue)
                                                    intResult = Math.Round(intResult, 6)
                                                    If Not _ApproveRefuse Then
                                                        Dim xBeginDate As Date = _Request.Date1.Value.Date
                                                        Dim xEndDate As Date = IIf(_Request.Date2.HasValue, _Request.Date2.Value.Date, xBeginDate)
                                                        Dim dblDuration As Double = IIf(_Request.Hours.HasValue, Math.Round(CDbl(_Request.Hours), 6), 0)

                                                        ' Obtenemos las horas/dias a tener en cuenta y las restamos del resultado final
                                                        intResult -= roBusinessSupport.GetEffectiveDaysHoursCauses(_Request.IDEmployee, xBeginDate, xEndDate, dblDuration, strConceptType, bolApplyWorkDaysOnConcept, _State)
                                                    End If
                                                ElseIf _Request.RequestType = eRequestType.PlannedCauses Then
                                                    ' Previsiones de ausencia por horas

                                                    ' Obtenemos los dias que debemos tener en cuenta (laborables o naturales) para realizar los calculos
                                                    Dim bolApplyWorkDaysOnConcept As Boolean = roTypes.Any2Boolean(ExecuteScalar("@SELECT# ISNULL(ApplyWorkDaysOnConcept , 0) FROM Causes WHERE ID=" & _Request.IDCause))

                                                    ' En funcion del tipo de saldo, calculamos horas o días
                                                    Dim strConceptType As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(IDType , 'O') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from causes where id =" & _Request.IDCause & ")"))
                                                    If strConceptType.Length = 0 Then strConceptType = "O"

                                                    ' Obtenemos los datos de las ausencias por horas de la justificación
                                                    roBusinessSupport.PlannedCausesResumeQuery(_Request.IDEmployee, _Request.IDCause, Now.Date, Nothing, Nothing, _Request.Date1.Value, intPending, intLasting, intDisponible, _State, bolApplyWorkDaysOnConcept, strConceptType)

                                                    ' Ahora obtenemos los datos de las ausencias por dias  de la misma justificación
                                                    roBusinessSupport.PlannedAbsencesResumeQuery(_Request.IDEmployee, _Request.IDCause, Now.Date, Nothing, Nothing, _Request.Date1.Value, intPending_P, intLasting_P, intDisponible_P, _State, bolApplyWorkDaysOnConcept, strConceptType)

                                                    ' Si el saldo tiene caducidad, obtenemos los valores del saldo que va a caducar desde hoy hasta la ultima fecha solicitada
                                                    intIDConceptBalance = roTypes.Any2Double(ExecuteScalar("@SELECT# ISNULL(ID , 0) FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from Causes where id =" & _Request.IDCause & ") AND DefaultQuery = 'C' AND isnull(ApplyExpiredHours,0) = 1"))
                                                    If intIDConceptBalance > 0 Then intExpiredValue = roBusinessSupport.GetExpiredConceptValues(_Request.IDEmployee, intIDConceptBalance, Now.Date, IIf(_Request.Date2.HasValue, _Request.Date2.Value.Date, _Request.Date1.Value), _State)

                                                    intResult = intDisponible - (intLasting + intPending + intLasting_P + intPending_P + intExpiredValue)
                                                    intResult = Math.Round(intResult, 6)
                                                    If Not _ApproveRefuse Then
                                                        Dim xBeginDate As Date = _Request.Date1.Value.Date
                                                        Dim xEndDate As Date = IIf(_Request.Date2.HasValue, _Request.Date2.Value.Date, xBeginDate)
                                                        Dim dblDuration As Double = IIf(_Request.Hours.HasValue, Math.Round(CDbl(_Request.Hours), 6), 0)

                                                        ' Obtenemos las horas/dias a tener en cuenta y las restamos del resultado final
                                                        intResult -= roBusinessSupport.GetEffectiveDaysHoursCauses(_Request.IDEmployee, xBeginDate, xEndDate, dblDuration, strConceptType, bolApplyWorkDaysOnConcept, _State)
                                                    End If
                                                End If

                                                If intResult < 0 Then
                                                    bolRet = False
                                                    strErrorTextLine = _State.Language.Translate("RequestAction.NegativeAccrual", "RequestAction")
                                                End If
                                            End If

                                        Case eRequestRuleType.MaxNumberDays ' Numero maximo de dias solicitados
                                            ' Obtenemos el numero de dias solicitados
                                            If oRequestRule.Definition.MaxDays.HasValue AndAlso oRequestRule.Definition.TypePlannedDay.HasValue Then
                                                Dim intDays As Integer = 0

                                                If _Request.RequestDays Is Nothing OrElse _Request.RequestDays.Count = 0 Then
                                                    If oRequestRule.Definition.TypePlannedDay.Value = eTypePlannedDay.LaboralDay Then
                                                        intDays = roBusinessSupport.LaboralDaysInPeriod(_Request.IDEmployee, _Request.Date1, _Request.Date2, _State)
                                                    Else
                                                        intDays = Math.Abs(DateDiff(DateInterval.Day, _Request.Date1.Value.Date, _Request.Date2.Value.Date)) + 1
                                                    End If
                                                Else
                                                    If oRequestRule.Definition.TypePlannedDay.Value = eTypePlannedDay.LaboralDay Then
                                                        For Each oRequestDay As roRequestDay In _Request.RequestDays
                                                            intDays += roBusinessSupport.LaboralDaysInPeriod(_Request.IDEmployee, oRequestDay.RequestDate, oRequestDay.RequestDate, _State)
                                                        Next
                                                    Else
                                                        intDays = _Request.RequestDays.Count
                                                    End If
                                                End If

                                                If intDays > oRequestRule.Definition.MaxDays.Value Or intDays = 0 Then
                                                    bolRet = False
                                                    strErrorTextLine = _State.Language.Translate("RequestAction.MaxDaysAbsence", "RequestAction")
                                                End If
                                            End If
                                        Case eRequestRuleType.PeriodEnjoyment ' Periodo de disfrute
                                            If oRequestRule.Definition.BeginPeriod.HasValue AndAlso oRequestRule.Definition.EndPeriod.HasValue Then
                                                If Not ValidatePeriodEnjoynment(_Request, oRequestRule.Definition, _State) Then
                                                    bolRet = False
                                                    strErrorTextLine = _State.Language.Translate("RequestAction.PeriodEnjoynment", "RequestAction")
                                                End If
                                            End If
                                        Case eRequestRuleType.AutomaticValidation ' Validacion automatica de la solicitud
                                            ' En el caso que estemos creando la solicitud o que sea de intercambio de horario entre empleados
                                            ' la marcamos como automática e indicamos la fecha de validación automática,  si exista dicha regla para que posteriromente sea tratada por el motor
                                            ' de validaciones automáticas
                                            If _Request.ID <= 0 Or _Request.RequestType = eRequestType.ExchangeShiftBetweenEmployees Then
                                                _Request.AutomaticValidation = True
                                                _Request.ValidationDate = Now.Date
                                                If oRequestRule.Definition.MaxDays.HasValue AndAlso _Request.Date1.HasValue AndAlso oRequestRule.Definition.MaxDays.Value > 0 Then
                                                    _Request.ValidationDate = DateAdd(DateInterval.Day, oRequestRule.Definition.MaxDays.Value * -1, _Request.Date1.Value)
                                                    If DateDiff(DateInterval.Day, _Request.ValidationDate.Value, _Request.Date1.Value) >= 0 Then
                                                        If oRequestRule.Definition.TypePlannedDay IsNot Nothing _
                                                        AndAlso oRequestRule.Definition.TypePlannedDay.HasValue _
                                                            AndAlso oRequestRule.Definition.TypePlannedDay.Value = eTypePlannedDay.LaboralDay Then

                                                            ' Verificamos si debemos utilizar el calendario de un empleado plantilla
                                                            Dim intIDEmployeeTemplate As Integer = roRequestRuleManager.GetIDEmployeeTemplate(_Request.IDEmployee, _Request.Date1.Value.Date, _State)
                                                            If intIDEmployeeTemplate = -1 Then
                                                                ' en caso contrario el empleado
                                                                intIDEmployeeTemplate = _Request.IDEmployee
                                                            End If

                                                            ' Si el dia tiene que ser laboral, lo verificamos hasta 60 dias antes de la fecha calculada inicial
                                                            Dim intDays As Integer = roBusinessSupport.LaboralDaysInPeriod(intIDEmployeeTemplate, _Request.ValidationDate, _Request.Date1.Value.AddDays(-1), _State)
                                                            While (intDays < oRequestRule.Definition.MaxDays.Value) And (DateDiff(DateInterval.Day, _Request.ValidationDate.Value, _Request.Date1.Value) < (60 + oRequestRule.Definition.MaxDays.Value))
                                                                _Request.ValidationDate = _Request.ValidationDate.Value.AddDays(-1)
                                                                intDays = roBusinessSupport.LaboralDaysInPeriod(intIDEmployeeTemplate, _Request.ValidationDate, _Request.Date1.Value.AddDays(-1), _State)
                                                            End While

                                                            If DateDiff(DateInterval.Day, _Request.ValidationDate.Value, _Request.Date1.Value) >= (60 + oRequestRule.Definition.MaxDays.Value) Then
                                                                ' Si se ha sobrepasado el limite de 60 dias revisados, por defecto dejamos X dias naturales antes,
                                                                ' entendiendo que la planificacion del empleado es erronea
                                                                _Request.ValidationDate = DateAdd(DateInterval.Day, oRequestRule.Definition.MaxDays.Value * -1, _Request.Date1.Value)
                                                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequestRule::RequestRulesValidate::Exceeded the maximum number of days to review, wrong employee's calendar. Employee id:" & _Request.IDEmployee.ToString)
                                                            End If

                                                        End If
                                                    End If
                                                ElseIf oRequestRule.Definition.MaxDays.HasValue AndAlso oRequestRule.Definition.MaxDays.Value = 0 Then
                                                    ' En el caso de validación inmediata, verificamos si tiene el parametro avanzado , que indica
                                                    ' si la validación se retrasa X dias
                                                    Dim oParam As New AdvancedParameter.roAdvancedParameter("VTLive.RequestRules.AutomaticValidation.DelayedDays", New AdvancedParameter.roAdvancedParameterState(_State.IDPassport))
                                                    If roTypes.Any2Double(oParam.Value) > 0 Then
                                                        ' Retrasamos la fecha de validacion X dias
                                                        _Request.ValidationDate = Now.Date.AddDays(roTypes.Any2Double(oParam.Value))
                                                    End If
                                                End If
                                            End If
                                        Case eRequestRuleType.LimitDateRequested ' Limite fecha solicitada
                                            Dim _bolApplyRule As Boolean = True
                                            If oRequestRule.Definition.TypeCriteriaEmployee.HasValue AndAlso oRequestRule.Definition.TypeCriteriaEmployee.Value = eTypeEmployeeCriteria.FilteredEmployees Then
                                                ' Seleccionamos los empleados que cumplan la condicion
                                                Dim strSQLFilter As String = ""

                                                For Each oCondition As VTUserFields.UserFields.roUserFieldCondition In oRequestRule.RuleConditions
                                                    strSQLFilter = " AND " & oCondition.GetFilter(_Request.IDEmployee)
                                                Next
                                                strSQLFilter = " Employees.ID = " & _Request.IDEmployee.ToString & strSQLFilter
                                                Dim tbEmployees As DataTable = roBusinessSupport.GetEmployees(strSQLFilter, "", "", _State)
                                                _bolApplyRule = (tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0)
                                            End If
                                            If _bolApplyRule Then
                                                If oRequestRule.Definition.MaxDays.HasValue Then
                                                    If _Request.Date1.HasValue AndAlso DateDiff(DateInterval.Day, _Request.RequestDate.Date, _Request.Date1.Value) > oRequestRule.Definition.MaxDays.Value Then bolRet = False
                                                    If _Request.Date2.HasValue AndAlso DateDiff(DateInterval.Day, _Request.RequestDate.Date, _Request.Date2.Value) > oRequestRule.Definition.MaxDays.Value Then bolRet = False
                                                End If

                                                If Not bolRet Then strErrorTextLine = _State.Language.Translate("RequestAction.LimitDateRequested", "RequestAction")
                                            End If
                                        Case eRequestRuleType.MinimumDaysBeforeRequestedDate ' Mínimo de dias previos a la fecha solicitada
                                            ' Obtenemos el numero de dias
                                            Dim intDays As Integer = 0

                                            ' Verificamos si debemos utilizar el calendario de un empleado plantilla
                                            Dim intIDEmployeeTemplate As Integer = roRequestRuleManager.GetIDEmployeeTemplate(_Request.IDEmployee, _Request.Date1.Value.Date, _State)
                                            If intIDEmployeeTemplate = -1 Then
                                                ' en caso contrario el empleado
                                                intIDEmployeeTemplate = _Request.IDEmployee
                                            End If

                                            If oRequestRule.Definition.TypePlannedDay.Value = eTypePlannedDay.LaboralDay Then
                                                intDays = roBusinessSupport.LaboralDaysInPeriod(intIDEmployeeTemplate, _Request.RequestDate.Date, _Request.Date1.Value.AddDays(-1), _State)
                                            Else
                                                intDays = DateDiff(DateInterval.Day, _Request.RequestDate.Date, _Request.Date1.Value.Date)
                                            End If
                                            If intDays < oRequestRule.Definition.MaxDays.Value Then
                                                bolRet = False
                                                strErrorTextLine = _State.Language.Translate("RequestAction.MinimumDaysBeforeRequestedDate", "RequestAction")
                                            End If
                                        Case eRequestRuleType.MaxNotScheduledDays ' Calendario anual no planificado
                                            If Not ValidateMaxNotScheduledDays(_Request, _State) Then
                                                bolRet = False
                                                strErrorTextLine = _State.Language.Translate("RequestAction.MaximumNotScheduled", "RequestAction")
                                            End If

                                        Case eRequestRuleType.MinCoverageRequiered ' Mínima cobertura requerida
                                            If Not ValidateMinCoverageRequiered(_Request, _State) Then
                                                bolRet = False
                                                strErrorTextLine = _State.Language.Translate("RequestAction.MinimumCoverageRequiered", "RequestAction")
                                            End If
                                        Case eRequestRuleType.MinimumConsecutiveDays ' Mínimo de días consecutivos solicitados
                                            If oRequestRule.Definition.MinDays.HasValue Then
                                                If Not ValidateMinimumConsecutiveDays(_Request, oRequestRule, _State) Then
                                                    bolRet = False
                                                    strErrorTextLine = _State.Language.Translate("RequestAction.MinimumConsecutiveDays", "RequestAction")
                                                End If
                                            End If

                                        Case eRequestRuleType.AutomaticRejection ' Rechazo automatico de la solicitud
                                            If oRequestRule.Definition.MaxDays.HasValue Then
                                                _Request.RejectedDate = DateAdd(DateInterval.Day, oRequestRule.Definition.MaxDays.Value + 1, _Request.RequestDate.Date)
                                            End If
                                        Case eRequestRuleType.ScheduleRuleToValidate
                                            Dim idRules = oRequestRule.Definition.IDPlanificationRules
                                            If idRules IsNot Nothing AndAlso idRules.Count > 0 Then
                                                If Not ValidateScheduleRules(_Request, IdLabAgree, idRules, _State) Then
                                                    bolRet = False
                                                    strErrorTextLine = _State.Language.Translate("RequestAction.ScheduleRuleToValidate", "RequestAction")
                                                End If
                                            End If
                                    End Select
                                End If
                            End If

                            If Not bolRet Then
                                ' Inidcamos el tipo de accion
                                If oRequestRule.Definition.Action = eActionType.Denied_Action Or _Request.AutomaticValidation = True Then
                                    ' No permitimos continuar
                                    _State.Result = RequestResultEnum.RequestRuleError
                                    If Not _simpleMessage Then
                                        _State.ErrorText = _State.Language.Translate("RequestAction.DeniedAction", "RequestAction") & " " & strErrorTextLine
                                    Else
                                        _State.ErrorText = strErrorTextLine
                                    End If

                                    Exit For
                                Else
                                    ' Preguntamos al empleado/supervisor
                                    _State.Result = RequestResultEnum.NeedConfirmation
                                    strErrorText = IIf(strErrorText.Length = 0, strErrorTextLine, strErrorText & " " & strErrorTextLine)

                                    If Not _simpleMessage Then
                                        _State.ErrorText = strErrorText & " " & _State.Language.Translate("RequestAction.ContinueAction", "RequestAction")
                                    Else
                                        _State.ErrorText = strErrorText
                                    End If
                                End If
                            End If
                        End If
                    Else
                        If bolApplyAPV AndAlso oRequestRule.IDRuleType = eRequestRuleType.AutomaticValidation AndAlso oRequestRule.Description.ToUpper.Contains("VERIFYDAY") Then
                            ' Si es APV y la regla es de validacion automatica,
                            ' solo en el caso que no debamos aplicar la regla, debemos mirar igualmente si contiene el comportamiento VERIFYDAY
                            ' para realizar en ese caso dicha verificacion igualmente
                            bolRet = VerifyDay_APV(_Request, oRequestRule, strErrorTextLine, _State, _simpleMessage)
                            If Not bolRet Then
                                Exit For
                            End If
                        End If

                    End If
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::RequestRulesValidate")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::RequestRulesValidate")
            Finally

                If _State.Result <> LabAgreeResultEnum.NoError Then
                    bolRet = False
                End If
            End Try

            Return bolRet

        End Function

        Public Shared Function IsRuleUsedOnRequest(ByVal _Request As Requests.roRequest, ByVal _IDRuleType As eRequestRuleType, ByVal _State As roRequestState) As Boolean
            ' verificamos si para la solicitud indicada
            ' se aplica una determinada regla
            Dim bolRet As Boolean = False

            Try

                If _Request Is Nothing Then
                    Return bolRet
                    Exit Function
                End If

                If Not _Request.Date1.HasValue Then
                    Return bolRet
                    Exit Function
                End If

                If _Request.IDEmployee <= 0 Then
                    Return bolRet
                    Exit Function
                End If

                ' Obtenemos el convenio del empleado para la fecha inicial de solicitud
                Dim IdLabAgree As Integer = -1

                Dim strSQL As String = " @SELECT# IDLabAgree From EmployeeContracts " &
                                       " Where BeginDate <= " & roTypes.Any2Time(_Request.Date1).SQLSmallDateTime & " " &
                                       " And EndDate >= " & roTypes.Any2Time(_Request.Date1).SQLSmallDateTime & " " &
                                       " And IDEmployee = '" & _Request.IDEmployee & "' "
                IdLabAgree = roTypes.Any2Integer(ExecuteScalar(strSQL))

                If IdLabAgree <= 0 Then
                    Return bolRet
                    Exit Function
                End If

                Dim oLabAgreeState As New roLabAgreeState(_State.IDPassport)

                Dim labAgreeRules As Generic.List(Of roRequestRule) = roRequestRule.GetRequestsRules("IDLabAgree=" & IdLabAgree, oLabAgreeState, False)

                ' Cargamos los datos del convenio del empleado
                If labAgreeRules Is Nothing OrElse labAgreeRules.Count = 0 Then
                    Return bolRet
                    Exit Function
                End If

                _State.Result = LabAgreeResultEnum.NoError

                Dim intIDShift As Integer = -1
                If _Request.IDShift.HasValue Then
                    intIDShift = _Request.IDShift.Value
                End If

                Dim intIDCause As Integer = -1
                If _Request.IDCause.HasValue Then
                    intIDCause = _Request.IDCause.Value
                End If

                For Each oRequestRule As roRequestRule In labAgreeRules
                    ' Si existe una regla que aplique a esta solicitud del mismo tipo que la indicada
                    If oRequestRule.Activated And oRequestRule.IDRequestType = _Request.RequestType _
                                    And oRequestRule.Definition IsNot Nothing AndAlso oRequestRule.IDRuleType = _IDRuleType Then

                        Dim bolApplyRule As Boolean = False
                        ' Verificamos si aplica la regla en funcion de los motivos indicados
                        Select Case oRequestRule.IDRequestType
                            ' Vacaciones por dias
                            Case DTOs.eRequestType.VacationsOrPermissions
                                If oRequestRule.Definition.IDReasons.Count > 0 AndAlso _Request.IDShift.HasValue AndAlso oRequestRule.Definition.IDReasons.FindAll(Function(o) oRequestRule.Definition.IDReasons.Contains(intIDShift)).Count > 0 Then
                                    bolApplyRule = True
                                End If

                            ' Vacaciones por horas
                            Case DTOs.eRequestType.PlannedHolidays
                                If oRequestRule.Definition.IDReasons.Count > 0 AndAlso _Request.IDCause.HasValue AndAlso oRequestRule.Definition.IDReasons.FindAll(Function(o) oRequestRule.Definition.IDReasons.Contains(intIDCause)).Count > 0 Then
                                    bolApplyRule = True
                                End If

                            ' Ausencias por dias, por horas
                            Case DTOs.eRequestType.PlannedAbsences, DTOs.eRequestType.PlannedCauses
                                If oRequestRule.Definition.IDReasons.Count > 0 AndAlso _Request.IDCause.HasValue AndAlso oRequestRule.Definition.IDReasons.FindAll(Function(o) oRequestRule.Definition.IDReasons.Contains(intIDCause)).Count > 0 Then
                                    bolApplyRule = True
                                End If
                        End Select

                        If bolApplyRule Then
                            bolRet = True
                            Exit For
                        End If
                    End If
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::IsRuleUsedOnRequest")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::IsRuleUsedOnRequest")
            End Try

            Return bolRet
        End Function

        Public Shared Function GetIDEmployeeTemplate(ByVal _IDEmployeeOriginal As Integer, ByVal _Date As Date, ByVal _State As roRequestState) As Integer
            ' Obtenemos el empleado que tiene definida la planificacion a utilizar por el empleado original
            Dim intRet As Integer = -1
            Dim bolret As Boolean = False

            Try
                Dim oParam As New AdvancedParameter.roAdvancedParameter("VTLive.RequestRules.EmployeeFieldTemplate", New AdvancedParameter.roAdvancedParameterState(_State.IDPassport))
                Dim strFieldName As String = ""
                If roTypes.Any2String(oParam.Value).Length > 0 Then
                    ' Obtenemos el campo de la ficha que tiene el valor del empleado plantilla
                    strFieldName = roTypes.Any2String(oParam.Value).Trim
                End If

                If strFieldName.Length > 0 Then
                    ' Campo empleado plantilla
                    Dim oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(_IDEmployeeOriginal, strFieldName.Trim, _Date, New VTUserFields.UserFields.roUserFieldState(_State.IDPassport))
                    If oEmployeeUserField IsNot Nothing AndAlso oEmployeeUserField.FieldValue IsNot Nothing AndAlso oEmployeeUserField.FieldValue.ToString.Trim.Length > 0 Then
                        ' Obtenemos el valor del campo primary key del empleado
                        Dim strEmployeePrimaryKeyValue As String = oEmployeeUserField.FieldValue.ToString.Trim
                        Dim xParam As New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState)
                        Dim strImportPrimaryKeyUserField As String = ""
                        If roTypes.Any2String(xParam.Value).Length > 0 Then
                            ' Obtenemos el campo de la ficha que tiene el valor del empleado plantilla
                            strImportPrimaryKeyUserField = roTypes.Any2String(xParam.Value)
                        End If

                        If strImportPrimaryKeyUserField.Length = 0 Then strImportPrimaryKeyUserField = "NIF"
                        Dim tbEmployee As DataTable = VTUserFields.UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue(strImportPrimaryKeyUserField.Trim, strEmployeePrimaryKeyValue, _Date, New VTUserFields.UserFields.roUserFieldState)
                        If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then
                            intRet = tbEmployee.Rows(0).Item("IDEmployee")
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roRequestRule::GetIDEmployeeTemplate::Employee not found with the indicated value. FieldName:" & strImportPrimaryKeyUserField & " Value: " & strEmployeePrimaryKeyValue)
                        End If
                    End If
                End If

                bolret = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::GetIDEmployeeTemplate")
                intRet = -1
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::GetIDEmployeeTemplate")
                intRet = -1
            End Try

            Return intRet

        End Function

        Public Shared Function RequestRuleNegativeAcrual_APV(ByVal _Request As roRequest, ByVal _State As roRequestState, ByVal _ApproveRefuse As Boolean) As Boolean
            'Por REGLA GENERAL se revisaran los saldos negativos agrupados por los dias de cada año
            '           Asi los dias solicitados de 2019 se verificaran con el saldo del 2019
            '           y los dias del 2020 se verificaran con el valor inicial configurado en el convenio

            'COMO EXCEPCION y en los horarios que tengan inidcado CARRYRULE en la descripcion del horario
            '    los dias solictados del año siguiente se verificaran con lo acumulados del año anterior + el valor inicial del 2020
            Dim bolRet As Boolean = True

            Dim strSQL As String = ""

            Try

                If _Request.RequestType = eRequestType.VacationsOrPermissions Then
                    ' Vacaciones/permisos por dias

                    '01 contabilizamos los dias que hay de cada año
                    Dim bolRequestNextYear As Boolean = False
                    Dim xListDayforYear As New roCollection
                    For Each xDay As roRequestDay In _Request.RequestDays.OrderBy(Function(rd) rd.RequestDate)
                        Dim intYear As Integer = 0
                        intYear = IIf(xDay.RequestDate.Year <= Now.Date.Year, Now.Date.Year, xDay.RequestDate.Year)
                        If xListDayforYear.Exists(intYear) Then
                            xListDayforYear(intYear) += 1
                        Else
                            xListDayforYear.Add(intYear, 1)
                        End If
                        If intYear > Now.Year Then
                            bolRequestNextYear = True
                        End If
                    Next

                    ' En funcion del tipo de Horario y si algunos de los dias solicitados son del año siguiente
                    ' aplicamos la validación para cada año por separado o de forma conjunta
                    Dim bolGroupedMode As Boolean = False
                    Dim strDescrption As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(Description,'') FROM Shifts WHERE ID=" & _Request.IDShift.ToString))
                    If strDescrption.ToUpper.Contains("CARRYRULE") And bolRequestNextYear Then
                        bolGroupedMode = True
                    End If

                    Dim intTotalResult As Double = 0
                    Dim intCountDays As Double = 0

                    Dim __ActualYear As Boolean = False

                    ' Para cada año revisamos las solicitudes por separado
                    For i As Integer = 1 To xListDayforYear.Count
                        Dim intResult As Double = 0
                        Dim intDone As Double = 0 : Dim intPending As Double = 0
                        Dim intLasting As Double = 0 : Dim intDisponible As Double = 0
                        Dim _NextYear As Boolean = False

                        ' Si el año es el actual, miramos los valores del año actual
                        If xListDayforYear.Key(i) = Now.Date.Year Then
                            VacationsResumeQuery_APV(_Request.IDEmployee, _Request.IDShift, Now.Date, Nothing, Nothing, _Request.Date1.Value, intDone, intPending, intLasting, intDisponible, _State, False)
                            __ActualYear = True
                        Else
                            ' Si es futuro, tenemos en cuenta el valor inicial calculado del saldo , mas lo que este planificado  y solictado
                            _NextYear = True
                            Dim xDate As Date = DateSerial(xListDayforYear.Key(i), 1, 1)
                            VacationsResumeQuery_APV(_Request.IDEmployee, _Request.IDShift, xDate, Nothing, Nothing, _Request.Date1.Value, intDone, intPending, intLasting, intDisponible, _State, True)
                        End If

                        intResult = intDisponible - (intLasting + intPending)

                        If bolGroupedMode Then
                            ' Si exsiten dias solicitados del año siguiente y el horario tiene el tratamiento  especial
                            '  nos guardamos los totales para posteriormente tenerlos en cuenta en el calculo de los dias solicitados del año siguiente
                            intTotalResult += intResult
                            intCountDays += xListDayforYear(xListDayforYear.Key(i))
                        End If

                        If bolGroupedMode And _NextYear Then
                            ' Si el horario tiene tratamiento agrupado y el año que estamos revisando es el siguiente al actual
                            ' Realizamos la verificacion con el sumatorio de todos los años

                            ' en el caso que no haya solicitado dias del año actual tenemos que obtener ahora los datos del año actual
                            If Not __ActualYear AndAlso xListDayforYear.Count = 1 Then
                                intResult = 0
                                intDone = 0 : intPending = 0
                                intLasting = 0 : intDisponible = 0

                                VacationsResumeQuery_APV(_Request.IDEmployee, _Request.IDShift, Now.Date, Nothing, Nothing, _Request.Date1.Value, intDone, intPending, intLasting, intDisponible, _State, False)

                                intResult = intDisponible - (intLasting + intPending)
                                intTotalResult += intResult

                            End If

                            If Not _ApproveRefuse Then intTotalResult = intTotalResult - intCountDays

                            If intTotalResult < 0 Then
                                ' Tiene saldo negativo
                                bolRet = False
                                Return bolRet
                                Exit Function
                            End If
                            '
                        Else
                            ' Si los dias solicitados son del año actual o el horario no tiene el comportamiento especial de agrupacion
                            ' realizamos la verificacion únicamente de los valores de dicho año
                            If Not _ApproveRefuse Then intResult = intResult - xListDayforYear(xListDayforYear.Key(i))

                            If intResult < 0 Then
                                ' Tiene saldo negativo
                                bolRet = False
                                Return bolRet
                                Exit Function
                            End If
                        End If
                    Next
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                bolRet = True
                _State.UpdateStateInfo(ex, "roRequestRule::RequestRuleNegativeAcrual_APV")
            Catch ex As Exception
                bolRet = True
                _State.UpdateStateInfo(ex, "roRequestRule::RequestRuleNegativeAcrual_APV")
            Finally

            End Try

            Return bolRet
        End Function

        Public Shared Function RequestRuleNegativeAcrual_UPF(ByVal _Request As roRequest, ByVal _RequestDescription As String, ByVal _State As roRequestState) As Boolean
            '
            ' UPF: Se debe revisar el saldo indicado en la descripcion de la regla. En el caso que el valor sea positivo , no se debe realizar la solicitud
            '

            Dim bolRet As Boolean = True

            Dim strSQL As String = ""

            Try

                Dim xCurrentDate As DateTime = Now.Date
                Dim xBeginPeriod As DateTime
                Dim xEndPeriod As DateTime
                Dim xVacationsDate As DateTime = _Request.Date1.Value

                ' Obtenemos el periodo de calculo
                ' Actualmente a nivel de reglas de solicitudes solo puede ser Anual / Por contrato / Mensual
                Dim IDConcept As Integer

                If _RequestDescription.Contains("[UPF_") Then
                    Dim strShortName As String = ""
                    strShortName = _RequestDescription.Substring(_RequestDescription.IndexOf("UPF_") + 4)
                    strShortName = Left(strShortName, strShortName.Length - 1)
                    IDConcept = roTypes.Any2Integer(ExecuteScalar("@SELECT# isnull(ID,0) FROM Concepts WHERE ShortName like '" & strShortName & "' "))
                    If IDConcept = 0 Then
                        Return True
                        Exit Function
                    End If
                Else
                    Return True
                    Exit Function
                End If

                Dim strDefaultQuery As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts WHERE ID =" & IDConcept.ToString))
                If strDefaultQuery.Length = 0 Then strDefaultQuery = "Y"

                ' Determinar inicio i final del periodo anual
                xBeginPeriod = roParameters.BeginYearPeriod(xCurrentDate)
                ' A la fecha final le añadimos un año por si permiten solicitar vacaciones de un año para otro
                xEndPeriod = roParameters.EndYearPeriod(xCurrentDate).AddYears(1)

                If strDefaultQuery = "M" Then
                    ' En el caso de saldos mensuales
                    Dim oParams As New roParameters("OPTIONS", True)
                    Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                    If intMonthIniDay = 0 Then intMonthIniDay = 1

                    If xCurrentDate.Day > intMonthIniDay Then
                        'Si el dia es posterior al inicio del periodo (mismo mes)
                        xBeginPeriod = New Date(xCurrentDate.Year, xCurrentDate.Month, intMonthIniDay)
                    ElseIf xCurrentDate.Day < intMonthIniDay Then
                        'Si el dia es anterior al inicio del periodo (mes anterior)
                        xBeginPeriod = New Date(xCurrentDate.AddMonths(-1).Year, xCurrentDate.AddMonths(-1).Month, intMonthIniDay)
                    Else
                        'Si es el mismo dia
                        xBeginPeriod = xCurrentDate
                    End If
                End If

                Dim lstDates As New Generic.List(Of DateTime)
                strSQL = "@SELECT# BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & _Request.IDEmployee & " AND " &
                                       "BeginDate <= " & roTypes.Any2Time(xVacationsDate).SQLSmallDateTime & " AND EndDate >= " & roTypes.Any2Time(xVacationsDate).SQLSmallDateTime
                Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
                Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
                If rd.Read() Then
                    If strDefaultQuery = "C" Then
                        ' En el caso de saldos por contrato
                        ' la fecha de inicio del periodo es el inicio de contrato
                        ' y la fecha de final del periodo es la misma del final del contrato
                        xBeginPeriod = rd("BeginDate")
                        xEndPeriod = rd("EndDate")
                    End If
                    lstDates.Add(rd("BeginDate"))
                    lstDates.Add(rd("EndDate"))
                End If
                rd.Close()

                If lstDates.Count > 0 And (strDefaultQuery = "Y" Or strDefaultQuery = "M") Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                End If

                strSQL = "@SELECT# isnull(SUM(DailyAccruals.Value), 0) as Total " &
                         "FROM DailyAccruals " &
                         "WHERE DailyAccruals.IDConcept = " & IDConcept.ToString & " AND " &
                               "DailyAccruals.IDEmployee = " & _Request.IDEmployee.ToString & " AND " &
                               "DailyAccruals.[Date] BETWEEN " &
                                    roTypes.Any2Time(xBeginPeriod.Date).SQLSmallDateTime & " AND " &
                                    roTypes.Any2Time(xEndPeriod.Date).SQLSmallDateTime & " AND " &
                               "DailyAccruals.[Date] <= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime

                Dim tb As DataTable = CreateDataTable(strSQL, )
                Dim intRet As Double = -1
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    intRet = roTypes.Any2Double(tb.Rows(0).Item(0))
                    If intRet > 0 Then
                        ' Si el valor del saldo es positivo , no dejamos realizar la solicitud hasta que se gaste el valor de dicho saldo
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                bolRet = True
                _State.UpdateStateInfo(ex, "roRequestRule::RequestRuleNegativeAcrual_UPF")
            Catch ex As Exception
                bolRet = True
                _State.UpdateStateInfo(ex, "roRequestRule::RequestRuleNegativeAcrual_UPF")
            Finally

            End Try

            Return bolRet
        End Function

        Public Shared Function VacationsResumeQuery_APV(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xCurrentDate As DateTime, ByRef xBeginPeriod As DateTime, ByRef xEndPeriod As DateTime,
                                         ByVal xVacationsDate As DateTime, ByRef intDone As Double, ByRef intPending As Double, ByRef intLasting As Double, ByRef intDisponible As Double,
                                         ByVal _State As roBusinessState, ByVal bolCalculateInitialValue As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Try

                ' Determinar inicio i final del periodo anual
                xBeginPeriod = roParameters.BeginYearPeriod(xCurrentDate)
                xEndPeriod = roParameters.EndYearPeriod(xCurrentDate)

                Dim lstDates As New Generic.List(Of DateTime)
                Dim strSQL As String = "@SELECT# BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND " &
                                           "BeginDate <= " & roTypes.Any2Time(xVacationsDate).SQLSmallDateTime & " AND EndDate >= " & roTypes.Any2Time(xVacationsDate).SQLSmallDateTime
                Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
                Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
                If rd.Read() Then
                    lstDates.Add(rd("BeginDate"))
                    lstDates.Add(rd("EndDate"))
                End If
                rd.Close()

                If lstDates.Count > 0 Then
                    If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
                End If

                ' Obtener número días ya disfrutados
                If bolCalculateInitialValue Then
                    ' Si el periodo es futuro, no hemos disfrutado ninguno
                    intDone = 0
                Else
                    intDone = roBusinessSupport.GetAlreadyTakenHollidays(IDEmployee, IDShift, xCurrentDate, xBeginPeriod, _State)
                End If

                Dim bolAreWorkingDays As Boolean = roTypes.Any2Boolean(ExecuteScalar("@SELECT# ISNULL(AreWorkingDays , 0) FROM Shifts WHERE ID=" & IDShift))

                ' Obtener número de días solicitados pendientes de procesar                
                intPending = GetPendingApprovalHollidays_APV(IDEmployee, IDShift, xBeginPeriod, xEndPeriod, bolAreWorkingDays, _State)

                ' Obtener el número de días aprobados pendientes de disfrutar
                If bolCalculateInitialValue Then
                    xCurrentDate = xCurrentDate.AddDays(-1)
                End If
                intLasting = roBusinessSupport.GetApprovedButNotTakenHolidays(IDEmployee, IDShift, xCurrentDate, xEndPeriod, _State)

                ' Obtener los días disponibles de vacaciones
                intDisponible = GetDisponibleHolidays_APV(IDEmployee, IDShift, xBeginPeriod, xEndPeriod, _State, bolCalculateInitialValue)

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::VacationsResumeQuery_APV")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::VacationsResumeQuery_APV")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function GetPendingApprovalHollidays_APV(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime, ByVal bolAreWorkingDays As Boolean,
            ByVal _State As roBusinessState) As Integer

            Dim intRet As Integer = 0

            Try

                Dim strSQL As String
                strSQL = "@SELECT# sysroRequestDays.* " &
                             "FROM Requests INNER JOIN Shifts " &
                                        "ON Requests.IDShift = Shifts.[ID] " &
                                        " INNER JOIN sysroRequestDays " &
                                        "On Requests.ID = sysroRequestDays.[IDRequest] " &
                             "WHERE Requests.IDEmployee = " & IDEmployee.ToString & " And " &
                                   "Shifts.ShiftType = 2 And " &
                                   "Requests.IDShift = " & IDShift & " And " &
                                   "sysroRequestDays.Date >= " & roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime & " And " &
                                   "sysroRequestDays.Date <= " & roTypes.Any2Time(xEndPeriod).SQLSmallDateTime & " And " &
                                   "Requests.RequestType = " & eRequestType.VacationsOrPermissions & " And " &
                                   "Requests.Status In (" & eRequestStatus.Pending & "," & eRequestStatus.OnGoing & ")"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    intRet = tb.Rows.Count
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::GetPendingApprovalHollidays_APV")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::GetPendingApprovalHollidays_APV")
            Finally

            End Try

            Return intRet

        End Function

        Public Shared Function GetDisponibleHolidays_APV(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                          ByVal _State As roBusinessState, ByVal bolCalculateInitialValue As Boolean) As Double

            Dim intRet As Double = 0

            Dim rd As DbDataReader = Nothing

            Try

                If Not bolCalculateInitialValue Then
                    Dim strSQL As String
                    strSQL = "@SELECT# isnull(SUM(DailyAccruals.Value), 0) as Total " &
                             "FROM DailyAccruals INNER JOIN Concepts " &
                                    "ON DailyAccruals.IDConcept = Concepts.[ID] " &
                                    "INNER JOIN Shifts ON Concepts.[ID] = Shifts.IDConceptBalance " &
                             "WHERE Shifts.ShiftType = 2 AND Shifts.ID = " & IDShift & " AND " &
                                   "DailyAccruals.IDEmployee = " & IDEmployee.ToString & " AND " &
                                   "DailyAccruals.[Date] BETWEEN " &
                                        roTypes.Any2Time(xBeginPeriod.Date).SQLSmallDateTime & " AND " &
                                        roTypes.Any2Time(xEndPeriod.Date).SQLSmallDateTime & " AND " &
                                   "DailyAccruals.[Date] <= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        intRet = roTypes.Any2Double(tb.Rows(0).Item(0))
                    End If
                Else
                    ' Calculamos el valor inicial
                    Dim intIDConcept As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# ISNULL(IDConceptBalance,0) FROM Shifts WHERE ID=" & IDShift.ToString))
                    If intIDConcept > 0 Then intRet = GetStartupValue_APV(IDEmployee, intIDConcept, xBeginPeriod.Date, _State)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::GetDisponibleHolidays_APV")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::GetDisponibleHolidays_APV")
            Finally

            End Try

            Return intRet

        End Function

        Public Shared Function GetStartupValue_APV(ByVal IDEmployee As Integer, ByVal IDConcept As Integer, ByVal xDate As DateTime,
                                  ByVal _State As roBusinessState) As Double

            Dim intRet As Double = 0

            Dim rd As DbDataReader = Nothing

            Try

                ' 01. Obtenemos el convenio del contrato a la fecha de inicio del periodo
                'Obtenemos el convenio del contrato en la fecha a procesar
                Dim sSQL As String = "@SELECT# isnull(IDLabAgree,0) FROM EmployeeContracts WHERE IDEmployee =" & IDEmployee.ToString
                sSQL = sSQL & " AND BeginDate <=" & roTypes.Any2Time(xDate).SQLSmallDateTime
                sSQL = sSQL & " AND EndDate >=" & roTypes.Any2Time(xDate).SQLSmallDateTime
                Dim intIDLabAgree As Integer = roTypes.Any2Integer(ExecuteScalar(sSQL))

                ' 02. Obtenemos al definicion del valor inicial del saldo
                If intIDLabAgree > 0 Then
                    sSQL = "@SELECT# isnull(IDStartupValue,0) FROM LabAgreeStartupValues WHERE IDLabAgree=" & intIDLabAgree.ToString & " AND IDConcept=" & IDConcept.ToString
                    Dim intIDStartupValue As Integer = roTypes.Any2Integer(ExecuteScalar(sSQL))
                    If intIDStartupValue > 0 Then
                        ' Cargamos la definicion del valor inicial
                        Dim oStartupValue As New roStartupValue(intIDStartupValue, New roLabAgreeState(-1), False)
                        If oStartupValue IsNot Nothing Then

                            Select Case oStartupValue.StartValueType
                                Case LabAgreeValueType.DirectValue
                                    ' Valor fijo
                                    intRet = oStartupValue.StartValue
                                Case LabAgreeValueType.UserField
                                    ' Valor campo de la ficha
                                    If oStartupValue.StartUserField IsNot Nothing AndAlso oStartupValue.StartUserField.FieldName.Length > 0 Then
                                        Dim oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oStartupValue.StartUserField.FieldName, xDate, New VTUserFields.UserFields.roUserFieldState)
                                        If oEmployeeUserField IsNot Nothing AndAlso oEmployeeUserField.FieldValue IsNot Nothing Then
                                            intRet = roTypes.Any2Double(oEmployeeUserField.FieldValue)
                                        End If
                                    End If
                                Case LabAgreeValueType.CalculatedValue
                                    ' Valor calculado

                                    ' Valor inicial Base
                                    Dim dblStartValueBase As Double = 0
                                    Dim dblTotalPeriodBase As Double = 0

                                    If oStartupValue.StartValueBaseType = LabAgreeValueTypeBase.DirectValue Then
                                        dblStartValueBase = oStartupValue.StartValueBase
                                    Else
                                        If oStartupValue.StartUserFieldBase IsNot Nothing Then
                                            Dim oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oStartupValue.StartUserFieldBase.FieldName, xDate, New VTUserFields.UserFields.roUserFieldState)
                                            If oEmployeeUserField IsNot Nothing AndAlso oEmployeeUserField.FieldValue IsNot Nothing Then
                                                dblStartValueBase = roTypes.Any2Double(oEmployeeUserField.FieldValue)
                                            End If
                                        End If
                                    End If

                                    ' Total Base
                                    If oStartupValue.TotalPeriodBaseType = LabAgreeValueTypeBase.DirectValue Then
                                        dblTotalPeriodBase = oStartupValue.TotalPeriodBase
                                    Else
                                        If oStartupValue.StartUserFieldTotalPeriodBase IsNot Nothing Then
                                            Dim oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oStartupValue.StartUserFieldTotalPeriodBase.FieldName, xDate, New VTUserFields.UserFields.roUserFieldState)
                                            If oEmployeeUserField IsNot Nothing AndAlso oEmployeeUserField.FieldValue IsNot Nothing Then
                                                dblTotalPeriodBase = roTypes.Any2Double(oEmployeeUserField.FieldValue)
                                            End If
                                        End If
                                    End If

                                    Dim bolInvalidConcept As Boolean = False
                                    ' Total dias activos del empleado en el periodo del saldo
                                    ' Solo para saldos anuales y mensuales
                                    Dim StartAccrualDate As Date = Now.Date
                                    Dim EndAccrualDate As Date = Now.Date
                                    Dim ConceptType As String = roTypes.Any2String(ExecuteScalar("@SELECT# isnull(DefaultQuery,'') FROM CONCEPTS WHERE ID = " & IDConcept.ToString))
                                    Select Case ConceptType
                                        Case "Y"
                                            ' Si el acumulado es anual
                                            StartAccrualDate = roParameters.BeginYearPeriod(xDate)
                                            EndAccrualDate = StartAccrualDate.AddYears(1).AddDays(-1)
                                        Case "M"
                                            ' Si el acumulado es mensual
                                            StartAccrualDate = roParameters.BeginYearPeriod(xDate)
                                            EndAccrualDate = StartAccrualDate.AddMonths(1).AddDays(-1)
                                        Case Else
                                            bolInvalidConcept = True
                                    End Select
                                    Dim ActiveDays As Double = 0
                                    If Not bolInvalidConcept Then
                                        Dim oContract As Contract.roContract = Contract.roContract.GetContractInDate(IDEmployee, xDate, New Contract.roContractState(-1), False)
                                        If oContract IsNot Nothing Then
                                            Dim xEndContractDate As Date = oContract.EndDate
                                            If oStartupValue.ApplyEndCustomPeriod Then
                                                ' En el caso que el fin del periodo pueda ser configurable
                                                ' Obtenmos si el campo de la ficha tiene un valor valido
                                                ' en caso afirmativo,asignamos como fecha final la mínima de las 2
                                                Dim oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oStartupValue.EndCustomPeriodUserField.FieldName, xDate, New VTUserFields.UserFields.roUserFieldState)
                                                If oEmployeeUserField IsNot Nothing AndAlso oEmployeeUserField.FieldValue IsNot Nothing AndAlso IsDate(oEmployeeUserField.FieldValue) Then
                                                    Dim EndCustomPeriodUserField As Date = CDate(oEmployeeUserField.FieldValue)
                                                    xEndContractDate = New DateTime(Math.Min(EndCustomPeriodUserField.Ticks, oContract.EndDate.Ticks))
                                                End If
                                            End If

                                            ActiveDays = roTypes.Any2Double(DateDiff("d", New DateTime(Math.Max(StartAccrualDate.Ticks, oContract.BeginDate.Ticks)), New DateTime(Math.Min(xEndContractDate.Ticks, EndAccrualDate.Ticks)))) + 1
                                            If ActiveDays < 0 Then ActiveDays = 0
                                        End If
                                    End If

                                    ' En función del tipo de saldo,
                                    ' realizamos el calculo del valor inicial
                                    Dim dblStartupCalculatedValue As Double = 0
                                    Dim dblAccruedValue As Double = 0
                                    Dim TypeConcept As String = roTypes.Any2String(ExecuteScalar("@SELECT# isnull(IDType,'') FROM CONCEPTS WHERE ID = " & IDConcept.ToString))
                                    Select Case TypeConcept
                                        Case "O" 'Dias/veces/personalizado
                                            If dblTotalPeriodBase <> 0 Then
                                                dblStartupCalculatedValue = (dblStartValueBase * ActiveDays) / dblTotalPeriodBase
                                            End If
                                        Case "H" ' Horas
                                            ' Coeficiente de parcialidad
                                            If oStartupValue.AccruedValueType = LabAgreeValueTypeBase.DirectValue Then
                                                ' Directo
                                                dblAccruedValue = oStartupValue.AccruedValue
                                            Else
                                                ' Campo de la ficha
                                                Dim oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oStartupValue.StartUserFieldAccruedValue.FieldName, xDate, New VTUserFields.UserFields.roUserFieldState)
                                                If oEmployeeUserField IsNot Nothing AndAlso oEmployeeUserField.FieldValue IsNot Nothing Then
                                                    dblAccruedValue = roTypes.Any2Double(oEmployeeUserField.FieldValue)
                                                End If
                                            End If

                                            dblTotalPeriodBase = dblTotalPeriodBase * dblAccruedValue
                                            dblStartValueBase = dblStartValueBase * dblAccruedValue

                                            Dim TotalHours As Double = (ActiveDays * dblTotalPeriodBase) / (roTypes.Any2Double(DateDiff(DateInterval.Day, StartAccrualDate, EndAccrualDate)) + 1)

                                            If dblTotalPeriodBase <> 0 Then
                                                dblStartupCalculatedValue = (dblStartValueBase * TotalHours) / dblTotalPeriodBase
                                            End If

                                    End Select

                                    ' Redondeo del valor resultante en caso necesario
                                    Select Case oStartupValue.RoundingType
                                        Case 1 ' Redondeo por arriba
                                            If dblStartupCalculatedValue - Int(dblStartupCalculatedValue) > 0 Then
                                                dblStartupCalculatedValue = Int(dblStartupCalculatedValue) + 1
                                            End If

                                        Case 2 ' Redondeo por abajo
                                            If dblStartupCalculatedValue - Int(dblStartupCalculatedValue) > 0 Then
                                                dblStartupCalculatedValue = Int(dblStartupCalculatedValue)
                                            End If

                                        Case 3 ' Redondeo por aproximacion
                                            If dblStartupCalculatedValue - Int(dblStartupCalculatedValue) > 0 Then
                                                If (dblStartupCalculatedValue - Int(dblStartupCalculatedValue)) >= 0.5 Then
                                                    dblStartupCalculatedValue = Int(dblStartupCalculatedValue) + 1
                                                Else
                                                    dblStartupCalculatedValue = Int(dblStartupCalculatedValue)
                                                End If
                                            End If
                                    End Select

                                    intRet = dblStartupCalculatedValue
                                    Dim StartValue As Double = intRet
                                    Dim ScalingUserField As String = oStartupValue.ScalingUserField
                                    Dim ScalingCoefficientUserField As String = oStartupValue.ScalingCoefficientUserField
                                    Dim dblScalingStartValue As Double = 0
                                    If Len(ScalingUserField) > 0 Then
                                        Dim ScalingDefinition As List(Of VTUserFields.UserFields.roScalingValues) = oStartupValue.ScalingFieldValues
                                        If ScalingDefinition IsNot Nothing Then
                                            dblScalingStartValue = GetScalingStartupValue_APV(IDEmployee, IDConcept, ScalingUserField, ScalingDefinition, xDate, ScalingCoefficientUserField, _State)
                                        End If
                                    End If
                                    If dblStartupCalculatedValue <> 0 Or dblScalingStartValue <> 0 Then
                                        ' Devolvemos el valor inicial del saldo
                                        intRet = dblStartupCalculatedValue + dblScalingStartValue
                                    End If
                            End Select
                        End If
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::GetStartupValue_APV")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::GetStartupValue_APV")
            Finally

            End Try

            Return intRet

        End Function

        Public Shared Function GetScalingStartupValue_APV(ByVal EmployeeID As Long, ByVal intIDConcept As Integer, ByVal ScalingUserField As String, ByVal ScalingDefinition As List(Of VTUserFields.UserFields.roScalingValues), ByVal TaskDate As Date, ByVal ScalingCoefficientUserField As String,
                                  ByVal _State As roBusinessState) As Double

            Dim dRet As Double = 0
            Dim strScalingUserFieldValue As String = ""
            Dim dateScalingUserFieldValue As Date
            Dim intScalingValue As Integer = 0
            Dim intScalingCoefficientValue As Double = 0
            Dim strScalingCoefficientValue As String = ""
            Dim strScalingDefinition As String = ""
            Dim ConceptType As String = ""
            Dim bolInvalidConcept As Boolean = False

            Try

                ' Recupero año de antigüedad
                Dim oEmployeeUserField As VTUserFields.UserFields.roEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, ScalingUserField, TaskDate, New VTUserFields.UserFields.roUserFieldState)
                If oEmployeeUserField IsNot Nothing AndAlso oEmployeeUserField.FieldValue IsNot Nothing AndAlso IsDate(oEmployeeUserField.FieldValue) Then
                    dateScalingUserFieldValue = CDate(oEmployeeUserField.FieldValue)
                Else
                    Return dRet
                    Exit Function
                End If

                ' Calculo antigüedad
                ' Sólo tengo en cuenta el año
                intScalingValue = TaskDate.Year - dateScalingUserFieldValue.Year

                ' La lista de escalado puede venir desordenada. Me voy a quedar con la diferencia positiva menor
                Dim iDiff As Integer
                Dim iReference As Integer
                iDiff = -1

                For Each oScalingValue As VTUserFields.UserFields.roScalingValues In ScalingDefinition
                    'oScalingValue.UserField & oScalingValue.AccumValue

                    'iReference = String2Item(sTemp, 0, "#")
                    iReference = roTypes.Any2Integer(oScalingValue.UserField)

                    If (intScalingValue - iReference) >= 0 And (iDiff = -1 Or (intScalingValue - iReference) <= iDiff) Then
                        iDiff = intScalingValue - iReference
                        'dRet = Any2Double(StringReplace(String2Item(sTemp, 1, "#"), ".", GetFormatDecimal))
                        Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
                        dRet = roTypes.Any2Double(oScalingValue.AccumValue.ToString.Replace(".", oInfo.CurrencyDecimalSeparator))
                    End If
                Next

                ' Finalmente, si hay campo de coeficiente de proporcionalidad, lo aplico ahora
                If Len(ScalingCoefficientUserField) > 0 Then
                    oEmployeeUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, ScalingCoefficientUserField, TaskDate, New VTUserFields.UserFields.roUserFieldState)
                    If oEmployeeUserField IsNot Nothing AndAlso oEmployeeUserField.FieldValue IsNot Nothing Then
                        intScalingCoefficientValue = roTypes.Any2Double(oEmployeeUserField.FieldValue)
                        If intScalingCoefficientValue > 0 Then
                            ' Como el campo de la ficha es numérico, y puede que acabe teniendo un cero por defecto, lo evito (no tiene sentido)
                            dRet = dRet * (intScalingCoefficientValue / 100)
                            ' Si resultan decimales, redondeo por abajo ...
                            dRet = Math.Round(dRet, 0)
                        End If
                    Else
                        Return dRet
                        Exit Function
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::GetScalingStartupValue_APV")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::GetScalingStartupValue_APV")
            Finally

            End Try

            Return dRet

        End Function

        Public Shared Function VerifyDay_APV(ByVal _Request As roRequest, ByVal oRequestRule As roRequestRule, ByRef strErrorTextLine As String,
                                  ByRef _State As roRequestState, ByVal _simpleMessage As Boolean) As Boolean
            ' En el caso de APV
            ' si la regla de solicitud tiene indicado el TAG VERIFYDAY
            ' debemos validar ademas de la propia regla, si se piden fechas correspondientes al año siguiente , que el dia en el que se está realizando la solicitud sea superior a
            ' VERIFYDAY=27/12

            Dim bolRet As Boolean = True

            Try

                Dim xVerifyDate As Date = Now.Date
                Dim intDayDate As Integer = Mid(oRequestRule.Description, 11, 2)
                Dim intMonthDate As Integer = Mid(oRequestRule.Description, 14, 2)
                xVerifyDate = DateSerial(Now.Year, intMonthDate, intDayDate)

                If _Request.RequestType = eRequestType.VacationsOrPermissions AndAlso _Request.RequestDays IsNot Nothing Then
                    For Each oRequestDay As roRequestDay In _Request.RequestDays
                        If Year(oRequestDay.RequestDate) > Year(Now.Date) Then
                            ' Si el año de la fecha solicitada es superior al actual
                            ' debemos verificar que hoy sea superior o igual a la fecha indicada en la regla para poder hacer la solicitud
                            If xVerifyDate > Now.Date Then
                                bolRet = False
                                Exit For
                            End If
                        End If
                    Next
                ElseIf _Request.RequestType = eRequestType.ExchangeShiftBetweenEmployees Then
                    If Year(_Request.Date1.Value.Date) > Year(Now.Date) Then
                        ' Si el año de la fecha solicitada es superior al actual
                        ' debemos verificar que hoy sea superior o igual a la fecha indicada en la regla para poder hacer la solicitud
                        If xVerifyDate > Now.Date Then
                            bolRet = False
                        End If
                    End If
                End If

                If bolRet = False Then
                    strErrorTextLine = _State.Language.Translate("RequestAction.MinimumDaysBeforeRequestedDate", "RequestAction")
                    ' No permitimos continuar
                    _State.Result = RequestResultEnum.RequestRuleError
                    If Not _simpleMessage Then
                        _State.ErrorText = _State.Language.Translate("RequestAction.DeniedAction", "RequestAction") & " " & strErrorTextLine
                    Else
                        _State.ErrorText = strErrorTextLine
                    End If
                End If
            Catch ex As DbException
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roRequestRuleManager::VerifyDay_APV::APV Customization: Error validating date limit", ex)
                bolRet = True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roRequestRuleManager::VerifyDay_APV::APV Customization: Error validating date limit", ex)
                bolRet = True
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateMinimumConsecutiveDays(ByVal _Request As Requests.roRequest, ByVal _RequestRule As roRequestRule, ByVal _State As roRequestState) As Boolean
            Dim bolRet As Boolean = True

            Try

                ' Obtenemnos el passport de sistema
                Dim intIDPassport As Integer = roTypes.Any2Integer(roConstants.GetSystemUserId())

                If intIDPassport > 0 Then
                    Dim xFirstDate As Date = _Request.Date1.Value
                    Dim xEndDate As Date = _Request.Date2.Value
                    Dim oCalendarState = New VTCalendar.roCalendarState(intIDPassport)
                    Dim oCalendarManager As New VTCalendar.roCalendarManager(oCalendarState)
                    Dim oEmployeeCalendar As New roCalendar

                    ' Cargamos la planificación del usuario del periodo entre el primer y el último día solicitado
                    oEmployeeCalendar = oCalendarManager.Load(xFirstDate, xEndDate, "B" & _Request.IDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)
                    If oEmployeeCalendar Is Nothing OrElse oEmployeeCalendar.CalendarData Is Nothing OrElse oEmployeeCalendar.CalendarData(0).PeriodData Is Nothing OrElse oEmployeeCalendar.CalendarData(0).PeriodData.DayData.Count = 0 Then
                        Return bolRet
                    End If

                    ' Lista de bloques de dias consecutivos solicitados
                    Dim DaysRequestedBlocks As New List(Of List(Of Date))

                    ' Bloque actual de dias consecutivos solicitados
                    Dim CurrentDaysRequestedBlock As New List(Of Date)

                    ' Que tipo de dias se han solicitado (naturales o laborales), en funcion del horario
                    Dim AreLaboralDays = roTypes.Any2Boolean(ExecuteScalar("@SELECT# ISNULL(AreWorkingDays , 0) FROM Shifts WHERE ID=" & _Request.IDShift.ToString))

                    ' Recorremos los dias solicitados
                    For Each xDay As roRequestDay In _Request.RequestDays.OrderBy(Function(rd) rd.RequestDate)
                        If AreLaboralDays Then
                            'Verificar si el día solicitado es laborable, en caso contrario la solicitud no es válida
                            Dim DayData As roCalendarRowDayData = oEmployeeCalendar.CalendarData(0).PeriodData.DayData.FirstOrDefault(Function(x) x.PlanDate = xDay.RequestDate)
                            Dim IsValidDay = DayData IsNot Nothing AndAlso DayData.MainShift IsNot Nothing AndAlso DayData.MainShift.PlannedHours > 0 AndAlso Not DayData.IsHoliday
                            If Not IsValidDay Then Return False
                        End If

                        'Si el bloque actual está vacío, agregar el día al bloque actual
                        If CurrentDaysRequestedBlock.Count = 0 Then
                            CurrentDaysRequestedBlock.Add(xDay.RequestDate)
                        ElseIf CurrentDaysRequestedBlock.Count > 0 Then
                            'Verificar si entre el anterior dia solicitado y el actual 
                            ' existen dias planificados (laborables /naturales en funcion del horario) que no se han solicitado
                            Dim previousDate = CurrentDaysRequestedBlock(CurrentDaysRequestedBlock.Count - 1)
                            Dim hasUnrequestedLaboralDay As Boolean = False
                            If AreLaboralDays Then
                                hasUnrequestedLaboralDay = oEmployeeCalendar.CalendarData(0).PeriodData.DayData.Where(Function(x) x IsNot Nothing _
                                                            AndAlso x.PlanDate > previousDate _
                                                            AndAlso x.PlanDate < xDay.RequestDate AndAlso (x.MainShift IsNot Nothing AndAlso x.MainShift.PlannedHours > 0 _
                                                            AndAlso Not x.IsHoliday)).Any(Function(x) _
                                                            Not _Request.RequestDays.Contains(New roRequestDay With {.RequestDate = x.PlanDate}))
                            Else
                                hasUnrequestedLaboralDay = oEmployeeCalendar.CalendarData(0).PeriodData.DayData.Where(Function(x) x IsNot Nothing _
                                                            AndAlso x.PlanDate > previousDate _
                                                            AndAlso x.PlanDate < xDay.RequestDate).Any(Function(x) _
                                                            Not _Request.RequestDays.Contains(New roRequestDay With {.RequestDate = x.PlanDate}))
                            End If


                            'Si no hay días (laborables/naturales) no solicitados entre el anterior dia solicitado y el actual, agregar el día al bloque actual
                            If Not hasUnrequestedLaboralDay Then
                                CurrentDaysRequestedBlock.Add(xDay.RequestDate)
                            Else
                                'En caso contrario, agregar el bloque actual a la lista de bloques y crear un bloque nuevo con el dia solicitado actual
                                DaysRequestedBlocks.Add(CurrentDaysRequestedBlock)
                                CurrentDaysRequestedBlock = New List(Of Date) From {xDay.RequestDate}
                            End If
                        End If
                    Next

                    'Agregar el último bloque a la lista de bloques
                    If CurrentDaysRequestedBlock.Count > 0 Then
                        DaysRequestedBlocks.Add(CurrentDaysRequestedBlock)
                    End If

                    If DaysRequestedBlocks.Count = 0 OrElse DaysRequestedBlocks.Any(Function(x) x.Count < _RequestRule.Definition.MinDays) Then
                        'Si existe algun bloque con menos de la cantidad minima de dias solicitados consecutivos, no se cumple la regla
                        bolRet = False
                    End If


                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::ValidateMinimumConsecutiveDays")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::ValidateMinimumConsecutiveDays")
            End Try

            Return bolRet
        End Function
    End Class

End Namespace