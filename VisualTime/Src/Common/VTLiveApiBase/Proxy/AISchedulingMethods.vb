Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBudgets
Imports Robotics.Base.VTBusiness.Common

Public Class AISchedulingMethods

#Region "Unidades Productivas"

    Public Shared Function GetProductiveUnits(ByVal oState As roWsState) As roProductiveUnitListResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roProductiveUnitState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProductiveUnitManager As New roProductiveUnitManager(bState)
        Dim oProductiveUnit As Generic.List(Of roProductiveUnit) = oProductiveUnitManager.GetProductiveUnits(bState)

        'crear el response genérico
        Dim genericResponse As New roProductiveUnitListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProductiveUnitManager.State, newGState)
        genericResponse.ProductiveUnits = If(oProductiveUnit IsNot Nothing, oProductiveUnit.ToArray, {})
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function SaveProductiveUnit(ByVal oProductiveUnit As roProductiveUnit, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProductiveUnitResponse
        'Recupero los datos de la petición"

        Dim bState = New roProductiveUnitState(-1)

        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProductiveUnitManager As New roProductiveUnitManager(bState)
        If Not oProductiveUnitManager.SaveProductiveUnit(oProductiveUnit, bAudit) Then oProductiveUnit.ID = -1

        'crear el response genérico
        Dim genericResponse As New roProductiveUnitResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProductiveUnitManager.State, newGState)
        genericResponse.ProductiveUnit = oProductiveUnit
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetProductiveUnitById(ByVal idProductiveUnit As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProductiveUnitResponse

        'cambio mi state genérico a un estado especifico
        Dim bState = New roProductiveUnitState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProductiveUnitManager As New roProductiveUnitManager(bState)
        Dim oProductiveUnit = oProductiveUnitManager.LoadProductiveUnit(idProductiveUnit, bAudit, True)

        'crear el response genérico
        Dim genericResponse As New roProductiveUnitResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProductiveUnitManager.State, newGState)
        genericResponse.ProductiveUnit = oProductiveUnit
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetProductiveUnitSummary(ByVal oProductiveUnit As roProductiveUnit, ByVal _ProductiveUnitSummaryType As ProductiveUnitSummaryType, ByVal oState As roWsState) As roProductiveUnitSummaryResponse

        'cambio mi state genérico a un estado especifico
        Dim bState = New roProductiveUnitState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProductiveUnitManager As New roProductiveUnitManager(bState)
        Dim oProductiveUnitSummary = oProductiveUnitManager.GetProductiveUnitSummary(oProductiveUnit, _ProductiveUnitSummaryType)

        'crear el response genérico
        Dim genericResponse As New roProductiveUnitSummaryResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProductiveUnitManager.State, newGState)
        genericResponse.ProductiveUnitSummary = oProductiveUnitSummary
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function DeleteProductiveUnit(ByVal oProductiveUnit As roProductiveUnit, ByVal bAudit As Boolean, ByVal oState As roWsState) As roProductiveUnitStandarResponse
        'Recupero los datos de la petición"
        Dim bState = New roProductiveUnitState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oProductiveUnitManager As New roProductiveUnitManager(bState)
        Dim oRet = oProductiveUnitManager.DeleteProductiveUnit(oProductiveUnit, bAudit)

        'crear el response genérico
        Dim genericResponse As New roProductiveUnitStandarResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oProductiveUnitManager.State, newGState)
        genericResponse.Result = oRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

#End Region

#Region "Presupuestos"

    Public Shared Function GetBudget(ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal strNodeFilter As String, ByVal TypeView As BudgetView, ByVal DetailLevel As BudgetDetailLevel, ByVal bAudit As Boolean, ByVal oState As roWsState, ByVal _IDProductiveUnit As Integer, ByVal strProductiveUnitFilter As String, ByVal bLoadIndictments As Boolean) As roBudgetResponse

        'cambio mi state genérico a un estado especifico
        Dim bState = New roBudgetState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oBudgetManager As New roBudgetManager(bState)
        Dim oBudget = oBudgetManager.Load(xFirstDay, xLastDay, strNodeFilter, TypeView, DetailLevel, bAudit, _IDProductiveUnit, strProductiveUnitFilter, bLoadIndictments)

        'crear el response genérico
        Dim genericResponse As New roBudgetResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oBudgetManager.State, newGState)
        genericResponse.Budget = oBudget
        genericResponse.oState = newGState
        Return genericResponse

    End Function

    Public Shared Function RemoveBudgetRowData(ByVal intNode As Integer, intIDProductiveUnit As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roStandarResponse

        'cambio mi state genérico a un estado especifico
        Dim bState = New roBudgetState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oBudgetManager As New roBudgetManager(bState)
        Dim bolRet = oBudgetManager.RemoveBudgetRowData(intNode, intIDProductiveUnit)

        'crear el response genérico
        Dim genericResponse As New roStandarResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Result = bolRet
        genericResponse.oState = newGState
        Return genericResponse

    End Function

    Public Shared Function SaveBudget(ByVal oBudget As roBudget, ByVal bAudit As Boolean, ByVal oState As roWsState) As roBudgetResponse
        Dim bState = New roBudgetState(-1)

        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oBudgetManager As New roBudgetManager(bState)
        Dim oBudgetResult As roBudgetResult = oBudgetManager.Save(oBudget, bAudit)

        'crear el response genérico
        Dim genericResponse As New roBudgetResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oBudgetManager.State, newGState)
        genericResponse.Budget = oBudget
        genericResponse.BudgetResult = oBudgetResult

        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetNewBudgetRow(ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, iIDProductiveUnit As Integer, iIDSecurityNode As Integer, bAudit As Boolean, oState As roWsState) As roBudgetResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roBudgetState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oBudgetManager As New roBudgetManager(bState)
        Dim oBudget = oBudgetManager.AddBudgetRowData(xFirstDay, xLastDay, iIDSecurityNode, iIDProductiveUnit)

        'crear el response genérico
        Dim genericResponse As New roBudgetResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Budget = oBudget
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetBudgetPeriodHourDefinition(ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, iIDProductiveUnit As Integer, iIDSecurityNode As Integer, bAudit As Boolean, oState As roWsState, ByVal bLoadIndictments As Boolean) As roBudgetResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roBudgetState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oBudgetManager As New roBudgetManager(bState)
        Dim oBudget = oBudgetManager.GetBudgetPeriodHourDefinition(xFirstDay, xLastDay, iIDSecurityNode, iIDProductiveUnit, bLoadIndictments)

        'crear el response genérico
        Dim genericResponse As New roBudgetResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Budget = oBudget
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetProductiveUnitsFromNode(ByVal IDNode As Integer, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, oState As roWsState) As roProductiveUnitListResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roBudgetState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oBudgetManager As New roBudgetManager(bState)
        Dim oProductiveUnits As Generic.List(Of roProductiveUnit) = oBudgetManager.GetProductiveUnitsFromNode(IDNode, xFirstDay, xLastDay)

        'crear el response genérico
        Dim genericResponse As New roProductiveUnitListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.ProductiveUnits = If(oProductiveUnits IsNot Nothing, oProductiveUnits.ToArray, {})
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary(ByVal _IDNode As Integer, ByVal xPlanDate As Date, ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, oState As roWsState) As roBudgetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummaryResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roProductiveUnitState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oManager As New roProductiveUnitModePositionManager(bState)
        Dim oEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary = oManager.GetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary(_IDNode, xPlanDate, oProductiveUnitModePosition)

        'crear el response genérico
        Dim genericResponse As New roBudgetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummaryResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Employees = oEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary.BudgetEmployeeAvailableForPositions
        genericResponse.NodeStatus = oEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary.CurrentStatusEmployeesSummary

        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function AddEmployeePlanOnPosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData, oState As roWsState) As roStandarResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roProductiveUnitState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oManager As New roProductiveUnitModePositionManager(bState)
        Dim oRes = oManager.AddEmployeePlanOnPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData)

        'crear el response genérico
        Dim genericResponse As New roStandarResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Result = oRes
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function AddEmployeesPlanOnPosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData(), oState As roWsState) As roStandarResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roProductiveUnitState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oManager As New roProductiveUnitModePositionManager(bState)
        Dim oRes = oManager.AddEmployeesPlanOnPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData)

        'crear el response genérico
        Dim genericResponse As New roStandarResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Result = oRes
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function RemoveEmployeeFromPosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData, oState As roWsState) As roStandarResponse
        'cambio mi state genérico a un estado especifico
        Dim bState = New roProductiveUnitState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oManager As New roProductiveUnitModePositionManager(bState)
        Dim oRes = oManager.RemoveEmployeeFromPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData)

        'crear el response genérico
        Dim genericResponse As New roStandarResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Result = oRes
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetEmployeeAvailableForNode(ByVal _IDNode As Integer, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal oState As roWsState) As roEmployeeAvailableForNodeResponse

        'cambio mi state genérico a un estado especifico
        Dim bState = New roBudgetState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oBudgetManager As New roBudgetManager(bState)
        Dim oBudget = oBudgetManager.GetEmployeeAvailableForNode(_IDNode, xFirstDay, xLastDay)

        'crear el response genérico
        Dim genericResponse As New roEmployeeAvailableForNodeResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oBudgetManager.State, newGState)
        genericResponse.AvailableEmployees = oBudget
        genericResponse.oState = newGState
        Return genericResponse
    End Function

#End Region

#Region "AI schedule"

    'Function RunAISchedulerForBudget(ByVal oBudget As roBudget, ByVal bAudit As Boolean, ByVal oState As roWsState) As roCalendarResponse Implements IAISchedulingSvc.RunAISchedulerForBudget

    '        'cambio mi state genérico a un estado especifico
    '        Dim bState = New roBudgetState(-1)
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

    'End Function

    'Function RemoveAIScheduledForBudget(ByVal oBudget As roBudget, ByVal bAudit As Boolean, ByVal oState As roWsState) As roCalendarResponse Implements IAISchedulingSvc.RemoveAIScheduledForBudget

    '        'cambio mi state genérico a un estado especifico
    '        Dim bState = New roBudgetState(-1)
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
    'End Function

#End Region

End Class