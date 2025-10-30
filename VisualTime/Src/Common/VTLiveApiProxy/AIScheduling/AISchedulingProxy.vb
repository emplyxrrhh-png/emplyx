Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBudgets
Imports Robotics.Base.VTBusiness.Common

Public Class AISchedulingProxy

#Region "Unidades Productivas"
    Public Function GetProductiveUnits(ByVal oState As roWsState) As roProductiveUnitListResponse
        Return AISchedulingMethods.GetProductiveUnits(oState)
    End Function

    Public Function SaveProductiveUnit(ByVal oProductiveUnit As roProductiveUnit, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProductiveUnitResponse
        Return AISchedulingMethods.SaveProductiveUnit(oProductiveUnit, bAudit, oState)
    End Function

    Function GetProductiveUnitById(ByVal idProductiveUnit As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProductiveUnitResponse
        Return AISchedulingMethods.GetProductiveUnitById(idProductiveUnit, bAudit, oState)
    End Function

    Function GetProductiveUnitSummary(ByVal oProductiveUnit As roProductiveUnit, ByVal _ProductiveUnitSummaryType As ProductiveUnitSummaryType, ByVal oState As roWsState) As roProductiveUnitSummaryResponse
        Return AISchedulingMethods.GetProductiveUnitSummary(oProductiveUnit, _ProductiveUnitSummaryType, oState)
    End Function




    Function DeleteProductiveUnit(ByVal oProductiveUnit As roProductiveUnit, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProductiveUnitStandarResponse
        Return AISchedulingMethods.DeleteProductiveUnit(oProductiveUnit, bAudit, oState)
    End Function

#End Region



#Region "Presupuestos"
    Function GetBudget(ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal strNodeFilter As String, ByVal TypeView As BudgetView, ByVal DetailLevel As BudgetDetailLevel, ByVal bAudit As Boolean, ByVal oState As roWsState, ByVal _IDProductiveUnit As Integer, ByVal strProductiveUnitFilter As String, ByVal bLoadIndictments As Boolean) As roBudgetResponse
        Return AISchedulingMethods.GetBudget(xFirstDay, xLastDay, strNodeFilter, TypeView, DetailLevel, bAudit, oState, _IDProductiveUnit, strProductiveUnitFilter, bLoadIndictments)
    End Function

    Function RemoveBudgetRowData(ByVal intNode As Integer, intIDProductiveUnit As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roStandarResponse
        Return AISchedulingMethods.RemoveBudgetRowData(intNode, intIDProductiveUnit, bAudit, oState)
    End Function

    Public Function SaveBudget(ByVal oBudget As roBudget, ByVal bAudit As Boolean, ByVal oState As roWsState) As roBudgetResponse
        Return AISchedulingMethods.SaveBudget(oBudget, bAudit, oState)
    End Function

    Public Function GetNewBudgetRow(ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, iIDProductiveUnit As Integer, iIDSecurityNode As Integer, bAudit As Boolean, oState As roWsState) As roBudgetResponse
        Return AISchedulingMethods.GetNewBudgetRow(xFirstDay, xLastDay, iIDProductiveUnit, iIDSecurityNode, bAudit, oState)
    End Function

    Public Function GetBudgetPeriodHourDefinition(ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, iIDProductiveUnit As Integer, iIDSecurityNode As Integer, bAudit As Boolean, oState As roWsState, ByVal bLoadIndictments As Boolean) As roBudgetResponse
        Return AISchedulingMethods.GetBudgetPeriodHourDefinition(xFirstDay, xLastDay, iIDProductiveUnit, iIDSecurityNode, bAudit, oState, bLoadIndictments)
    End Function

    Public Function GetProductiveUnitsFromNode(ByVal IDNode As Integer, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, oState As roWsState) As roProductiveUnitListResponse
        Return AISchedulingMethods.GetProductiveUnitsFromNode(IDNode, xFirstDay, xLastDay, oState)
    End Function

    Public Function GetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary(ByVal _IDNode As Integer, ByVal xPlanDate As Date, ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, oState As roWsState) As roBudgetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummaryResponse
        Return AISchedulingMethods.GetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary(_IDNode, xPlanDate, oProductiveUnitModePosition, oState)
    End Function

    Function AddEmployeePlanOnPosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData, oState As roWsState) As roStandarResponse
        Return AISchedulingMethods.AddEmployeePlanOnPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData, oState)
    End Function

    Function AddEmployeesPlanOnPosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData(), oState As roWsState) As roStandarResponse
        Return AISchedulingMethods.AddEmployeesPlanOnPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData, oState)
    End Function

    Function RemoveEmployeeFromPosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData, oState As roWsState) As roStandarResponse
        Return AISchedulingMethods.RemoveEmployeeFromPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData, oState)
    End Function

    Function GetEmployeeAvailableForNode(ByVal _IDNode As Integer, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal oState As roWsState) As roEmployeeAvailableForNodeResponse
        Return AISchedulingMethods.GetEmployeeAvailableForNode(_IDNode, xFirstDay, xLastDay, oState)
    End Function

#End Region

#Region "AI schedule"
    'Function RunAISchedulerForBudget(ByVal oBudget As roBudget, ByVal bAudit As Boolean, ByVal oState As roWsState) As roCalendarResponse Implements IAISchedulingSvc.RunAISchedulerForBudget


    '    If Global_asax.LoggedIn Then

    '        'cambio mi state genérico a un estado especifico
    '        Dim bState = New roBudgetState(Global_asax.Identity.ID)
    '        roWsStateManager.CopyTo(oState, bState)
    '        bState.UpdateStateInfo()

    '        Dim oAIPlannerState As New roAIPlannerState(oState.IDPassport)
    '        Dim oAIPlannerManager As New roAIPlannerManager(oAIPlannerState)

    '        Dim oAISolution As New roAIProblemSolution
    '        Dim bolRet As Boolean = oAIPlannerManager.SolveProblem(oBudget, oAISolution)

    '        'crear el response genérico
    '        Dim genericResponse As New roCalendarResponse

    '        Dim newGState As New roWsState
    '        roWsStateManager.CopyTo(oAIPlannerManager.State, newGState)
    '        genericResponse.Calendar = New roCalendar
    '        genericResponse.oState = newGState
    '        genericResponse.CalendarResult = New roCalendarResult
    '        Return genericResponse
    '    Else
    '        Return Nothing
    '    End If

    'End Function

    'Function RemoveAIScheduledForBudget(ByVal oBudget As roBudget, ByVal bAudit As Boolean, ByVal oState As roWsState) As roCalendarResponse Implements IAISchedulingSvc.RemoveAIScheduledForBudget


    '    If Global_asax.LoggedIn Then

    '        'cambio mi state genérico a un estado especifico
    '        Dim bState = New roBudgetState(Global_asax.Identity.ID)
    '        roWsStateManager.CopyTo(oState, bState)
    '        bState.UpdateStateInfo()

    '        Dim oAIPlannerState As New roAIPlannerState(oState.IDPassport)
    '        Dim oAIPlannerManager As New roAIPlannerManager(oAIPlannerState)


    '        'crear el response genérico
    '        Dim genericResponse As New roCalendarResponse

    '        Dim newGState As New roWsState
    '        roWsStateManager.CopyTo(oAIPlannerManager.State, newGState)
    '        genericResponse.Calendar = Nothing
    '        genericResponse.oState = newGState
    '        genericResponse.CalendarResult = New roCalendarResult
    '        Return genericResponse
    '    Else
    '        Return Nothing
    '    End If

    'End Function

#End Region

End Class
